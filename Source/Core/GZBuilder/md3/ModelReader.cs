using System;
using System.IO;
using System.Drawing;
using System.Text;
using System.Collections.Generic;

using CodeImp.DoomBuilder;
using CodeImp.DoomBuilder.Data;
using CodeImp.DoomBuilder.Rendering;
using CodeImp.DoomBuilder.GZBuilder.Data;
using CodeImp.DoomBuilder.GZBuilder.GZDoom;

using SlimDX;
using SlimDX.Direct3D9;

//mxd. Original version taken from here: http://colladadotnet.codeplex.com/SourceControl/changeset/view/40680
namespace CodeImp.DoomBuilder.GZBuilder.MD3
{
    internal class ModelReader
    {
        public static void Parse(ref ModeldefEntry mde, PK3StructuredReader reader, Device D3DDevice) {
            string[] modelNames = new string[mde.ModelNames.Count];
            string[] textureNames = new string[mde.TextureNames.Count];

            mde.ModelNames.CopyTo(modelNames);
            mde.TextureNames.CopyTo(textureNames);

            //should never happen
            /*if (modelNames.Length != textureNames.Length || textureNames.Length == 0 || modelNames.Length == 0) {
                General.ErrorLogger.Add(ErrorType.Warning, "MD3Reader: wrong parse params! (modelPaths=" + modelNames.ToString() + "; texturePaths=" + textureNames.ToString() + ")");
                return;
            }*/

            mde.Model = new GZModel();
            mde.Model.NUM_MESHES = (byte)modelNames.Length;

            BoundingBoxSizes bbs = new BoundingBoxSizes();

            for (int i = 0; i < modelNames.Length; i++) {
                string modelPath = Path.Combine(mde.Path, modelNames[i]);

                if (reader.FileExists(modelPath)) {
                    MemoryStream stream = reader.LoadFile(modelPath);
                    General.WriteLogLine("MD3Reader: loading '" + modelPath + "'");

                    //mesh
                    string ext = modelNames[i].Substring(modelNames[i].Length - 4);
                    string error = "";
                    if (ext == ".md3")
                        error = ReadMD3Model(ref bbs, ref mde, stream, D3DDevice);
                    else if (ext == ".md2")
                        error = ReadMD2Model(ref bbs, ref mde, stream, D3DDevice);

                    //texture
                    if (string.IsNullOrEmpty(error)) {
                        string texturePath = Path.Combine(mde.Path, textureNames[i]);
                        
                        if (textureNames[i] != ModeldefParser.INVALID_TEXTURE && reader.FileExists(texturePath)) {
                            mde.Model.Textures.Add(Texture.FromStream(D3DDevice, reader.LoadFile(texturePath)));
                        } else {
                            mde.Model.Textures.Add(General.Map.Data.UnknownTexture3D.Texture);
                            if (textureNames[i] != ModeldefParser.INVALID_TEXTURE)
                                GZBuilder.GZGeneral.LogAndTraceWarning("MD3Reader: unable to load texture '" + texturePath + "' - no such file");
                        }
                    } else {
                        GZBuilder.GZGeneral.LogAndTraceWarning("MD3Reader: error while loading " + modelPath + ": " + error);
                        mde.Model.NUM_MESHES--;
                    }
                    stream.Dispose();

                } else {
                    GZBuilder.GZGeneral.LogAndTraceWarning("MD3Reader: unable to load model '" + modelPath + "' - no such file");
                    mde.Model.NUM_MESHES--;
                }
            }

            if (mde.Model.NUM_MESHES <= 0) {
                mde.Model = null;
                return;
            }

            mde.Model.BoundingBox = BoundingBoxTools.CalculateBoundingBox(bbs);
        }

        private static string ReadMD3Model(ref BoundingBoxSizes bbs, ref ModeldefEntry mde, MemoryStream s, Device D3DDevice) {
            long start = s.Position;

            using (var br = new BinaryReader(s, Encoding.ASCII)) {
                string magic = ReadString(br, 4);
                if (magic != "IDP3")
                    return "magic should be 'IDP3', not '" + magic + "'";

                s.Position += 80;
                int numSurfaces = br.ReadInt32();
                s.Position += 12;
                int ofsSurfaces = br.ReadInt32();

                if (s.Position != ofsSurfaces + start)
                    s.Position = ofsSurfaces + start;

                List<short> polyIndecesList = new List<short>();
                List<WorldVertex> vertList = new List<WorldVertex>();

                string error = "";
                for (int c = 0; c < numSurfaces; ++c) {
                    error = ReadSurface(ref bbs, br, polyIndecesList, vertList, mde);
                    if (!string.IsNullOrEmpty(error))
                        return error;
                }

                //indeces for rendering current mesh in 2d
                short[] indeces2d_arr = CreateLineListIndeces(polyIndecesList);

                //mesh
                Mesh mesh = new Mesh(D3DDevice, polyIndecesList.Count / 3, vertList.Count, MeshFlags.IndexBufferManaged | MeshFlags.VertexBufferManaged, General.Map.Graphics.Shaders.World3D.VertexElements);

                DataStream stream = mesh.VertexBuffer.Lock(0, 0, LockFlags.None);
                stream.WriteRange(vertList.ToArray());
                mesh.VertexBuffer.Unlock();

                stream = mesh.IndexBuffer.Lock(0, 0, LockFlags.None);
                stream.WriteRange(polyIndecesList.ToArray());
                mesh.IndexBuffer.Unlock();

                mesh.OptimizeInPlace(MeshOptimizeFlags.AttributeSort);
                mde.Model.Meshes.Add(mesh);

                //2d data
                IndexBuffer indeces2d = new IndexBuffer(D3DDevice, 2 * indeces2d_arr.Length, Usage.WriteOnly, Pool.Managed, true);
                stream = indeces2d.Lock(0, 0, LockFlags.None);
                stream.WriteRange(indeces2d_arr);
                indeces2d.Unlock();

                mde.Model.Indeces2D.Add(indeces2d);
                mde.Model.NumIndeces2D.Add((short)polyIndecesList.Count);
            }
            return "";
        }

        private static string ReadSurface(ref BoundingBoxSizes bbs, BinaryReader br, List<short> polyIndecesList, List<WorldVertex> vertList, ModeldefEntry mde) {
            int vertexOffset = vertList.Count;
            long start = br.BaseStream.Position;
            string magic = ReadString(br, 4);
            if (magic != "IDP3")
                return "error while reading surface: Magic should be 'IDP3', not '" + magic + "'";

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
                polyIndecesList.Add( (short)(vertexOffset + br.ReadInt32()) );


            //Vertices
            if (start + ofsST != br.BaseStream.Position)
                br.BaseStream.Position = start + ofsST;

            for (int i = 0; i < numVerts; i++) {
                WorldVertex v = new WorldVertex();
                v.c = -1; //white
                v.u = br.ReadSingle();
                v.v = br.ReadSingle();

                vertList.Add(v);
            }

            //Normals
            if (start + ofsNormal != br.BaseStream.Position)
                br.BaseStream.Position = start + ofsNormal;

            for (int i = vertexOffset; i < vertexOffset + numVerts; i++) {
                WorldVertex v = vertList[i];

                v.y = -(float)br.ReadInt16() / 64 * mde.Scale.X;
                v.x = (float)br.ReadInt16() / 64 * mde.Scale.Y;
                v.z = (float)br.ReadInt16() / 64 * mde.Scale.Z + mde.zOffset;

                //bounding box
                BoundingBoxTools.UpdateBoundingBoxSizes(ref bbs, v);

                var lat = br.ReadByte() * (2 * Math.PI) / 255.0;
                var lng = br.ReadByte() * (2 * Math.PI) / 255.0;

                v.nx = (float)(Math.Sin(lng) * Math.Sin(lat));
                v.ny = -(float)(Math.Cos(lng) * Math.Sin(lat));
                v.nz = (float)(Math.Cos(lat));

                vertList[i] = v;
            }

            if (start + ofsEnd != br.BaseStream.Position)
                br.BaseStream.Position = start + ofsEnd;
            return "";
        }

        private static string ReadMD2Model(ref BoundingBoxSizes bbs, ref ModeldefEntry mde, MemoryStream s, Device D3DDevice) {
            long start = s.Position;

            using (var br = new BinaryReader(s, Encoding.ASCII)) {
                string magic = ReadString(br, 4);
                if (magic != "IDP2")  //magic number: "IDP2"
                    return "magic should be 'IDP2', not '" + magic + "'";

                int modelVersion = br.ReadInt32();
                if (modelVersion != 8)  //MD2 version. Must be equal to 8
                    return "MD2 version must be 8 but is " + modelVersion;

                int texWidth = br.ReadInt32();
                int texHeight = br.ReadInt32();
                s.Position += 8; //Size of one frame in bytes
                //s.Position += 4; //Number of textures
                int num_verts = br.ReadInt32(); //Number of vertices
                int num_uv = br.ReadInt32(); //The number of UV coordinates in the model
                int num_tris = br.ReadInt32(); //Number of triangles
                s.Position += 4; //Number of OpenGL commands

                if (br.ReadInt32() == 0)  //Total number of frames
                    return "model has 0 frames";

                s.Position += 4; //Offset to skin names (each skin name is an unsigned char[64] and are null terminated)
                int ofs_uv = br.ReadInt32();//Offset to s-t texture coordinates
                int ofs_tris = br.ReadInt32(); //Offset to triangles
                int ofs_animFrame = br.ReadInt32(); //An offset to the first animation frame

                List<short> polyIndecesList = new List<short>();
                List<short> uvIndecesList = new List<short>();
                List<Vector2> uvCoordsList = new List<Vector2>();
                List<WorldVertex> vertList = new List<WorldVertex>();

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
                    WorldVertex v = new WorldVertex();

                    v.x = ((float)br.ReadByte() * scale.X + translate.X) * mde.Scale.X;
                    v.y = ((float)br.ReadByte() * scale.Y + translate.Y) * mde.Scale.Y;
                    v.z = ((float)br.ReadByte() * scale.Z + translate.Z) * mde.Scale.Z + mde.zOffset;

                    vertList.Add(v);

                    s.Position += 1; //vertex normal
                }

                for (int i = 0; i < polyIndecesList.Count; i++) {
                    WorldVertex v = vertList[polyIndecesList[i]];
                    
                    //bounding box
                    BoundingBoxTools.UpdateBoundingBoxSizes(ref bbs, new WorldVertex(v.y, v.x, v.z));

                    //uv
                    v.u = uvCoordsList[uvIndecesList[i]].X;
                    v.v = uvCoordsList[uvIndecesList[i]].Y;
                    //color
                    v.c = -1; //white

                    vertList[polyIndecesList[i]] = v;
                }

                //indeces for rendering current mesh in 2d
                short[] indeces2d_arr = CreateLineListIndeces(polyIndecesList);

                //mesh
                Mesh mesh = new Mesh(D3DDevice, polyIndecesList.Count / 3, vertList.Count, MeshFlags.IndexBufferManaged | MeshFlags.VertexBufferManaged, General.Map.Graphics.Shaders.World3D.VertexElements);

                DataStream stream = mesh.VertexBuffer.Lock(0, 0, LockFlags.None);
                stream.WriteRange(vertList.ToArray());
                mesh.VertexBuffer.Unlock();

                stream = mesh.IndexBuffer.Lock(0, 0, LockFlags.None);
                stream.WriteRange(polyIndecesList.ToArray());
                mesh.IndexBuffer.Unlock();

                mesh.OptimizeInPlace(MeshOptimizeFlags.AttributeSort);
                mde.Model.Meshes.Add(mesh);

                //2d data
                IndexBuffer indeces2d = new IndexBuffer(D3DDevice, 2 * indeces2d_arr.Length, Usage.WriteOnly, Pool.Managed, true);
                stream = indeces2d.Lock(0, 0, LockFlags.None);
                stream.WriteRange(indeces2d_arr);
                indeces2d.Unlock();

                mde.Model.Indeces2D.Add(indeces2d);
                mde.Model.NumIndeces2D.Add((short)polyIndecesList.Count);
                mde.Model.Angle = -90.0f * (float)Math.PI / 180.0f;
            }
            return "";
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