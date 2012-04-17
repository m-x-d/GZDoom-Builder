using System;
using System.IO;
using System.Drawing;
using System.Text;
using System.Collections.Generic;

using CodeImp.DoomBuilder;
using CodeImp.DoomBuilder.GZBuilder.Data;

using SlimDX;
using SlimDX.Direct3D9;

//mxd. Original version taken from here: http://colladadotnet.codeplex.com/SourceControl/changeset/view/40680
namespace ColladaDotNet.Pipeline.MD3 {
    public class ModelReader {
        public static GZModel Parse(ModelDefEntry mde, Device D3DDevice) {
            string[] modelPaths = new string[mde.ModelNames.Count];
            string[] texturePaths = new string[mde.TextureNames.Count];

            mde.ModelNames.CopyTo(modelPaths);
            mde.TextureNames.CopyTo(texturePaths);

            if (modelPaths.Length != texturePaths.Length || texturePaths.Length == 0 || modelPaths.Length == 0) {
                General.ErrorLogger.Add(ErrorType.Warning, "MD3Reader: wrong parse params! (modelPaths=" + modelPaths.ToString() + "; texturePaths=" + texturePaths.ToString() + ")");
                return null;
            }

            GZModel model = new GZModel();
            model.NUM_MESHES = (byte)modelPaths.Length;

            BoundingBoxSizes bbs = new BoundingBoxSizes();

            for (int i = 0; i < modelPaths.Length; i++) {
                if (File.Exists(mde.Path + "\\" + modelPaths[i])) {
                    General.WriteLogLine("MD3Reader: loading '" + mde.Path + "\\" + modelPaths[i] + "'");
                    //mesh
                    string ext = modelPaths[i].Substring(modelPaths[i].Length - 4);
                    bool loaded = false;
                    if (ext == ".md3") {
                        //loaded = ReadMD3Model(mde, model, mde.Path + "\\" + modelPaths[i], D3DDevice);
                        loaded = ReadMD3Model(ref bbs, mde, model, mde.Path + "\\" + modelPaths[i], D3DDevice);
                    } else if (ext == ".md2") {
                        //loaded = ReadMD2Model(mde, model, mde.Path + "\\" + modelPaths[i], D3DDevice);
                        loaded = ReadMD2Model(ref bbs, mde, model, mde.Path + "\\" + modelPaths[i], D3DDevice);
                    }

                    //texture
                    if (loaded) {
                        if (File.Exists(mde.Path + "\\" + texturePaths[i])) {
                            model.Textures.Add(Texture.FromFile(D3DDevice, mde.Path + "\\" + texturePaths[i]));
                        } else {
                            model.Textures.Add(General.Map.Data.UnknownTexture3D.Texture);
                            General.ErrorLogger.Add(ErrorType.Warning, "MD3Reader: unable to load texture '" + mde.Path + "\\" + texturePaths[i] + "' - no such file");
                        }
                    } else {
                        model.NUM_MESHES--;
                    }
                } else {
                    General.ErrorLogger.Add(ErrorType.Warning, "MD3Reader: unable to load model '" + mde.Path + "\\" + modelPaths[i] + "' - no such file");
                    model.NUM_MESHES--;
                }
            }

            if (model.NUM_MESHES <= 0)
                return null;

            model.BoundingBox = CalculateBoundingBox(bbs);

            return model;
        }

        private static bool ReadMD3Model(ref BoundingBoxSizes bbs, ModelDefEntry mde, GZModel model, string modelPath, Device D3DDevice) {
            FileStream s = new FileStream(modelPath, FileMode.Open);
            long start = s.Position;

            using (var br = new BinaryReader(s, Encoding.ASCII)) {
                if (ReadString(br, 4) != "IDP3") {
                    General.ErrorLogger.Add(ErrorType.Warning, "MD3Reader: Error while loading '" + modelPath + "': Magic should be IDP3");
                    return false;
                }

                s.Position += 80;
                int numSurfaces = br.ReadInt32();
                s.Position += 12;
                int ofsSurfaces = br.ReadInt32();

                if (s.Position != ofsSurfaces + start)
                    s.Position = ofsSurfaces + start;

                List<short> polyIndecesList = new List<short>();
                List<ModelVertex> vertList = new List<ModelVertex>();

                for (int c = 0; c < numSurfaces; ++c)
                    ReadSurface(ref bbs, br, polyIndecesList, vertList, mde);

                //indeces for rendering current mesh in 2d
                short[] indeces2d_arr = CreateLineListIndeces(polyIndecesList);

                //mesh
                Mesh mesh = new Mesh(D3DDevice, polyIndecesList.Count / 3, vertList.Count, MeshFlags.IndexBufferManaged | MeshFlags.VertexBufferManaged, ModelVertex.Format);

                DataStream stream = mesh.VertexBuffer.Lock(0, 0, LockFlags.None);
                stream.WriteRange(vertList.ToArray());
                mesh.VertexBuffer.Unlock();

                stream = mesh.IndexBuffer.Lock(0, 0, LockFlags.None);
                stream.WriteRange(polyIndecesList.ToArray());
                mesh.IndexBuffer.Unlock();

                //2d data
                IndexBuffer indeces2d = new IndexBuffer(D3DDevice, 2 * indeces2d_arr.Length, Usage.WriteOnly, Pool.Managed, true);
                stream = indeces2d.Lock(0, 0, LockFlags.None);
                stream.WriteRange(indeces2d_arr);
                indeces2d.Unlock();

                model.Indeces2D.Add(indeces2d);
                model.NumIndeces2D.Add((short)polyIndecesList.Count);

                model.Meshes.Add(mesh);
            }
            return true;
        }

        private static void ReadSurface(ref BoundingBoxSizes bbs, BinaryReader br, List<short> polyIndecesList, List<ModelVertex> vertList, ModelDefEntry mde) {
            var start = br.BaseStream.Position;

            if (ReadString(br, 4) != "IDP3") {
                General.ErrorLogger.Add(ErrorType.Warning, "MD3Reader: Error while reading surface: Magic should be IDP3");
                return;
            }

            br.BaseStream.Position += 76;
            int numVerts = br.ReadInt32(); //Number of Vertex objects defined in this Surface, up to MD3_MAX_VERTS. Current value of MD3_MAX_VERTS is 4096.
            int numTriangles = br.ReadInt32(); //Number of Triangle objects defined in this Surface, maximum of MD3_MAX_TRIANGLES. Current value of MD3_MAX_TRIANGLES is 8192.
            int ofsTriangles = br.ReadInt32(); //Relative offset from SURFACE_START where the list of Triangle objects starts.
            br.BaseStream.Position += 4;
            int ofsST = br.ReadInt32(); //Relative offset from SURFACE_START where the list of ST objects (s-t texture coordinates) starts.
            int ofsNormal = br.ReadInt32(); //Relative offset from SURFACE_START where the list of Vertex objects (X-Y-Z-N vertices) starts.
            int ofsEnd = br.ReadInt32(); //Relative offset from SURFACE_START to where the Surface object ends.

            //polygons
            if (start + ofsTriangles != br.BaseStream.Position)
                br.BaseStream.Position = start + ofsTriangles;

            for (int i = 0; i < numTriangles * 3; i++)
                polyIndecesList.Add( (short)br.ReadInt32() );


            //Vertices
            if (start + ofsST != br.BaseStream.Position)
                br.BaseStream.Position = start + ofsST;

            for (int i = 0; i < numVerts; i++) {
                ModelVertex v = new ModelVertex();
                v.Color = 0xffffff;
                v.Tu = br.ReadSingle();
                v.Tv = br.ReadSingle();

                vertList.Add(v);
            }

            //Normals
            if (start + ofsNormal != br.BaseStream.Position)
                br.BaseStream.Position = start + ofsNormal;

            for (int i = 0; i < numVerts; i++) {
                ModelVertex v = vertList[i];
                short[] coords = new short[] { br.ReadInt16(), br.ReadInt16(), br.ReadInt16() };

                v.Position = new Vector3((float)coords[1] / 64, -(float)coords[0] / 64, (float)coords[2] / 64);
                v.Position.X *= mde.Scale.Y;
                v.Position.Y *= mde.Scale.X;
                v.Position.Z *= mde.Scale.Z;
                v.Position.Z += mde.zOffset;

                //bounding box
                UpdateBoundingBoxSizes(ref bbs, v);

                var lat = br.ReadByte() * (2 * Math.PI) / 255.0;
                var lng = br.ReadByte() * (2 * Math.PI) / 255.0;

                float nx = (float)(Math.Cos(lng) * Math.Sin(lat));
                float ny = (float)(Math.Sin(lng) * Math.Sin(lat));
                float nz = (float)(Math.Cos(lat));

                v.Normal = new Vector3(ny, -nx, nz);
                vertList[i] = v;
            }

            if (start + ofsEnd != br.BaseStream.Position)
                br.BaseStream.Position = start + ofsEnd;
        }

        private static bool ReadMD2Model(ref BoundingBoxSizes bbs, ModelDefEntry mde, GZModel model, string modelPath, Device D3DDevice) {
            FileStream s = new FileStream(modelPath, FileMode.Open);
            long start = s.Position;

            using (var br = new BinaryReader(s, Encoding.ASCII)) {
                if (ReadString(br, 4) != "IDP2") { //magic number: "IDP2"
                    General.ErrorLogger.Add(ErrorType.Warning, "MD3Reader: Error while loading '" + modelPath + "': Magic should be IDP2");
                    return false;
                }
                int modelVersion = br.ReadInt32();
                if (modelVersion != 8) { //MD2 version. Must be equal to 8
                    General.ErrorLogger.Add(ErrorType.Warning, "MD3Reader: Error while loading '" + modelPath + "': MD2 version must be equal to 8 but is " + modelVersion);
                    return false;
                }

                int texWidth = br.ReadInt32();
                int texHeight = br.ReadInt32();
                s.Position += 8; //Size of one frame in bytes
                //s.Position += 4; //Number of textures
                int num_verts = br.ReadInt32(); //Number of vertices
                int num_uv = br.ReadInt32(); //The number of UV coordinates in the model
                int num_tris = br.ReadInt32(); //Number of triangles
                s.Position += 4; //Number of OpenGL commands

                if (br.ReadInt32() == 0) { //Total number of frames
                    General.ErrorLogger.Add(ErrorType.Warning, "MD3Reader: Error while loading '" + modelPath + "': Model has 0 frames");
                    return false;
                }

                s.Position += 4; //Offset to skin names (each skin name is an unsigned char[64] and are null terminated)
                int ofs_uv = br.ReadInt32();//Offset to s-t texture coordinates
                int ofs_tris = br.ReadInt32(); //Offset to triangles
                int ofs_animFrame = br.ReadInt32(); //An offset to the first animation frame

                List<short> polyIndecesList = new List<short>();
                List<short> uvIndecesList = new List<short>();
                List<Vector2> uvCoordsList = new List<Vector2>();
                List<ModelVertex> vertList = new List<ModelVertex>();

                //polygons
                if (s.Position != ofs_tris + start)
                    s.Position = ofs_tris + start;

                for (int i = 0; i < num_tris; i++) {
                    polyIndecesList.Add((short)br.ReadInt16());
                    polyIndecesList.Add((short)br.ReadInt16());
                    polyIndecesList.Add((short)br.ReadInt16());

                    uvIndecesList.Add((short)br.ReadInt16());
                    uvIndecesList.Add((short)br.ReadInt16());
                    uvIndecesList.Add((short)br.ReadInt16());
                }

                //UV coords
                if (s.Position != ofs_uv + start)
                    s.Position = ofs_uv + start;

                for (int i = 0; i < num_uv; i++) {
                    uvCoordsList.Add(new Vector2((float)br.ReadInt16() / texWidth, (float)br.ReadInt16() / texHeight));
                }

                //first frame
                //header
                if (s.Position != ofs_animFrame + start)
                    s.Position = ofs_animFrame + start;

                Vector3 scale = new Vector3((float)br.ReadSingle(), (float)br.ReadSingle(), (float)br.ReadSingle());
                Vector3 translate = new Vector3((float)br.ReadSingle(), (float)br.ReadSingle(), (float)br.ReadSingle());

                s.Position += 16; //frame name

                //verts
                for (int i = 0; i < num_verts; i++) {
                    //pos
                    ModelVertex v = new ModelVertex();
                    Vector3 vPos = new Vector3(br.ReadByte(), br.ReadByte(), br.ReadByte());

                    vPos.X = scale.X * vPos.X + translate.X;
                    vPos.Y = scale.Y * vPos.Y + translate.Y;
                    vPos.Z = scale.Z * vPos.Z + translate.Z;

                    v.Position.X = vPos.X * mde.Scale.X;
                    v.Position.Y = vPos.Y * mde.Scale.Y;
                    v.Position.Z = vPos.Z * mde.Scale.Z;
                    v.Position.Z += mde.zOffset;

                    vertList.Add(v);
                    //set data for rendering in 2D mode
                    //model.verts2D.Add(new CodeImp.DoomBuilder.Geometry.Vector2D(v.Position.X, -v.Position.Y));

                    s.Position += 1; //br.ReadByte(); //vertex normal
                }

                for (int i = 0; i < polyIndecesList.Count; i++) {
                    ModelVertex v = vertList[polyIndecesList[i]];
                    
                    //bounding box
                    UpdateBoundingBoxSizes(ref bbs, v);

                    //uv
                    v.Tu = uvCoordsList[uvIndecesList[i]].X;
                    v.Tv = uvCoordsList[uvIndecesList[i]].Y;

                    vertList[polyIndecesList[i]] = v;
                }

                //indeces for rendering current mesh in 2d
                short[] indeces2d_arr = CreateLineListIndeces(polyIndecesList);

                //mesh
                Mesh mesh = new Mesh(D3DDevice, polyIndecesList.Count / 3, vertList.Count, MeshFlags.IndexBufferManaged | MeshFlags.VertexBufferManaged, ModelVertex.Format);

                DataStream stream = mesh.VertexBuffer.Lock(0, 0, LockFlags.None);
                stream.WriteRange(vertList.ToArray());
                mesh.VertexBuffer.Unlock();

                stream = mesh.IndexBuffer.Lock(0, 0, LockFlags.None);
                stream.WriteRange(polyIndecesList.ToArray());
                mesh.IndexBuffer.Unlock();

                model.Meshes.Add(mesh);

                //2d data
                IndexBuffer indeces2d = new IndexBuffer(D3DDevice, 2 * indeces2d_arr.Length, Usage.WriteOnly, Pool.Managed, true);
                stream = indeces2d.Lock(0, 0, LockFlags.None);
                stream.WriteRange(indeces2d_arr);
                indeces2d.Unlock();

                model.Indeces2D.Add(indeces2d);
                model.NumIndeces2D.Add((short)polyIndecesList.Count);

                return true;
            }
        }

        //this creates list of vertex indeces for rendering using LineList method
        private static short[] CreateLineListIndeces(List<short> polyIndecesList) {
            short[] indeces2d_arr = new short[polyIndecesList.Count * 2];
            short ind1, ind2, ind3;
            for (short i = 0; i < polyIndecesList.Count; i += 3) {
                ind1 = polyIndecesList[i];
                ind2 = polyIndecesList[i + 1];
                ind3 = polyIndecesList[i + 2];

                indeces2d_arr[i * 2] = ind1;
                indeces2d_arr[i * 2 + 1] = ind2;
                indeces2d_arr[i * 2 + 2] = ind2;
                indeces2d_arr[i * 2 + 3] = ind3;
                indeces2d_arr[i * 2 + 4] = ind3;
                indeces2d_arr[i * 2 + 5] = ind1;
            }
            return indeces2d_arr;
        }

        //this creates array of vectors resembling bounding box
        private static Vector3[] CalculateBoundingBox(BoundingBoxSizes bbs) {
            //center
            Vector3 v0 = new Vector3(bbs.MinX + (bbs.MaxX - bbs.MinX) / 2, bbs.MinY + (bbs.MaxY - bbs.MinY) / 2, bbs.MinZ + (bbs.MaxZ - bbs.MinZ) / 2);

            //corners
            Vector3 v1 = new Vector3(bbs.MinX, bbs.MinY, bbs.MinZ);
            Vector3 v2 = new Vector3(bbs.MaxX, bbs.MinY, bbs.MinZ);
            Vector3 v3 = new Vector3(bbs.MinX, bbs.MaxY, bbs.MinZ);
            Vector3 v4 = new Vector3(bbs.MaxX, bbs.MaxY, bbs.MinZ);
            Vector3 v5 = new Vector3(bbs.MinX, bbs.MinY, bbs.MaxZ);
            Vector3 v6 = new Vector3(bbs.MaxX, bbs.MinY, bbs.MaxZ);
            Vector3 v7 = new Vector3(bbs.MinX, bbs.MaxY, bbs.MaxZ);
            Vector3 v8 = new Vector3(bbs.MaxX, bbs.MaxY, bbs.MaxZ);

            return new Vector3[] { v0, v1, v2, v3, v4, v5, v6, v7, v8 };
        }

        private static void UpdateBoundingBoxSizes(ref BoundingBoxSizes bbs, ModelVertex v) {
            if (v.Position.X < bbs.MinX)
                bbs.MinX = (short)v.Position.X;
            else if (v.Position.X > bbs.MaxX)
                bbs.MaxX = (short)v.Position.X;

            if (v.Position.Z < bbs.MinZ)
                bbs.MinZ = (short)v.Position.Z;
            else if (v.Position.Z > bbs.MaxZ)
                bbs.MaxZ = (short)v.Position.Z;

            if (v.Position.Y < bbs.MinY)
                bbs.MinY = (short)v.Position.Y;
            else if (v.Position.Y > bbs.MaxY)
                bbs.MaxY = (short)v.Position.Y;
        }

        private static string ReadString(BinaryReader br, int len) {
            var NAME = string.Empty;
            int i = 0;
            for (i = 0; i < len; ++i) {
                var c = br.ReadChar();
                if (c == '\0') {
                    ++i;
                    break;
                }
                NAME += c;
            }
            for (; i < len; ++i) {
                br.ReadChar();
            }
            return NAME;
        }
    }
}

struct BoundingBoxSizes
{
    public short MinX;
    public short MaxX;
    public short MinY;
    public short MaxY;
    public short MinZ;
    public short MaxZ;
}