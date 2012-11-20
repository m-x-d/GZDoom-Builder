using System;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using System.Collections.Generic;

using CodeImp.DoomBuilder;
using CodeImp.DoomBuilder.IO;
using CodeImp.DoomBuilder.Data;
using CodeImp.DoomBuilder.Rendering;
using CodeImp.DoomBuilder.GZBuilder.Data;
using CodeImp.DoomBuilder.GZBuilder.GZDoom;

using SlimDX;
using SlimDX.Direct3D9;

//mxd. Original version taken from here: http://colladadotnet.codeplex.com/SourceControl/changeset/view/40680
namespace CodeImp.DoomBuilder.GZBuilder.MD3
{
    internal static class ModelReader
    {
		private const float VERTICAL_STRETCH = 1 / 1.2f;
		
		public static void Parse(ref ModeldefEntry mde, PK3StructuredReader reader, Device D3DDevice) {
            string[] modelNames = new string[mde.ModelNames.Count];
            string[] textureNames = new string[mde.TextureNames.Count];

            mde.ModelNames.CopyTo(modelNames);
            mde.TextureNames.CopyTo(textureNames);

            mde.Model = new GZModel();
            mde.Model.NUM_MESHES = (byte)modelNames.Length;

            BoundingBoxSizes bbs = new BoundingBoxSizes();

            for (int i = 0; i < modelNames.Length; i++) {
                string modelPath = Path.Combine(mde.Path, modelNames[i]);

                if (reader.FileExists(modelPath)) {
                    MemoryStream stream = reader.LoadFile(modelPath);
                    General.WriteLogLine("ModelLoader: loading '" + modelPath + "'");

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
                        
                        if (textureNames[i] != TextureData.INVALID_TEXTURE && reader.FileExists(texturePath)) {
                            if (Path.GetExtension(texturePath) == ".pcx") { //pcx format requires special handling...
                                FileImageReader fir = new FileImageReader();
                                Bitmap bitmap = fir.ReadAsBitmap(reader.LoadFile(texturePath));

                                if (bitmap != null) {
                                    BitmapData bmlock = bitmap.LockBits(new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, bitmap.PixelFormat);
                                    Texture texture = new Texture(D3DDevice, bitmap.Width, bitmap.Height, 1, Usage.None, Format.A8R8G8B8, Pool.Managed);

                                    DataRectangle textureLock = texture.LockRectangle(0, LockFlags.None);
                                    textureLock.Data.WriteRange(bmlock.Scan0, bmlock.Height * bmlock.Stride);

                                    bitmap.UnlockBits(bmlock);
                                    texture.UnlockRectangle(0);

                                    mde.Model.Textures.Add(texture);
                                } else {
                                    mde.Model.Textures.Add(General.Map.Data.UnknownTexture3D.Texture);
                                    GZBuilder.GZGeneral.LogAndTraceWarning("ModelLoader: unable to load texture '" + texturePath + "'");
                                }
                            } else {
                                mde.Model.Textures.Add(Texture.FromStream(D3DDevice, reader.LoadFile(texturePath)));
                            }
                        } else {
                            mde.Model.Textures.Add(General.Map.Data.UnknownTexture3D.Texture);
                            if (textureNames[i] != TextureData.INVALID_TEXTURE)
                                GZBuilder.GZGeneral.LogAndTraceWarning("ModelLoader: unable to load texture '" + texturePath + "' - no such file");
                        }
                    } else {
                        GZBuilder.GZGeneral.LogAndTraceWarning("ModelLoader: error while loading " + modelPath + ": " + error);
                        mde.Model.NUM_MESHES--;
                    }
                    stream.Dispose();

                } else {
                    GZBuilder.GZGeneral.LogAndTraceWarning("ModelLoader: unable to load model '" + modelPath + "' - no such file");
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

                List<int> polyIndecesList = new List<int>();
                List<WorldVertex> vertList = new List<WorldVertex>();

                string error = "";
                for (int c = 0; c < numSurfaces; ++c) {
                    error = ReadSurface(ref bbs, br, polyIndecesList, vertList, mde);
                    if (!string.IsNullOrEmpty(error))
                        return error;
                }

                //indeces for rendering current mesh in 2d
                int[] indeces2d_arr = CreateLineListIndeces(polyIndecesList);

                //mesh
                Mesh mesh = new Mesh(D3DDevice, polyIndecesList.Count / 3, vertList.Count, MeshFlags.Use32Bit | MeshFlags.IndexBufferManaged | MeshFlags.VertexBufferManaged, General.Map.Graphics.Shaders.World3D.VertexElements);

                using (DataStream stream = mesh.LockVertexBuffer(LockFlags.None)) {
                    stream.WriteRange(vertList.ToArray());
                }
                mesh.UnlockVertexBuffer();

                using (DataStream stream = mesh.LockIndexBuffer(LockFlags.None)) {
                    stream.WriteRange(polyIndecesList.ToArray());
                }

                mesh.OptimizeInPlace(MeshOptimizeFlags.AttributeSort);
                mde.Model.Meshes.Add(mesh);

                //2d data
                IndexBuffer indeces2d = new IndexBuffer(D3DDevice, 4 * indeces2d_arr.Length, Usage.WriteOnly, Pool.Managed, false);
                DataStream stream2d = indeces2d.Lock(0, 0, LockFlags.None);
                stream2d.WriteRange(indeces2d_arr);
                indeces2d.Unlock();

                mde.Model.Indeces2D.Add(indeces2d);
                mde.Model.NumIndeces2D.Add(polyIndecesList.Count);
            }
            return "";
        }

        private static string ReadSurface(ref BoundingBoxSizes bbs, BinaryReader br, List<int> polyIndecesList, List<WorldVertex> vertList, ModeldefEntry mde) {
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
                polyIndecesList.Add( vertexOffset + br.ReadInt32() );


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

            //rotation angles
            float angleOfsetCos = (float)Math.Cos(mde.AngleOffset);
            float angleOfsetSin = (float)Math.Sin(mde.AngleOffset);
            float pitchOfsetCos = (float)Math.Cos(-mde.PitchOffset);
            float pitchOfsetSin = (float)Math.Sin(-mde.PitchOffset);
            float rollOfsetCos = (float)Math.Cos(mde.RollOffset);
            float rollOfsetSin = (float)Math.Sin(mde.RollOffset);

            for (int i = vertexOffset; i < vertexOffset + numVerts; i++) {
                WorldVertex v = vertList[i];

                //read vertex
                v.y = -(float)br.ReadInt16() / 64;
                v.x = (float)br.ReadInt16() / 64;
                v.z = (float)br.ReadInt16() / 64;

                //rotate it
                if (mde.AngleOffset != 0) {
                    float rx = angleOfsetCos * v.x - angleOfsetSin * v.y;
                    float ry = angleOfsetSin * v.x + angleOfsetCos * v.y;
                    v.y = ry;
                    v.x = rx;
                }
                if (mde.PitchOffset != 0) {
                    float ry = pitchOfsetCos * v.y - pitchOfsetSin * v.z;
                    float rz = pitchOfsetSin * v.y + pitchOfsetCos * v.z;
                    v.z = rz;
                    v.y = ry;
                }
                if (mde.RollOffset != 0) {
                    float rx = rollOfsetCos * v.x - rollOfsetSin * v.z;
                    float rz = rollOfsetSin * v.x + rollOfsetCos * v.z;
                    v.z = rz;
                    v.x = rx;
                }

                //scale it
                v.y *= mde.Scale.X;
                v.x *= mde.Scale.Y;
				v.z *= mde.Scale.Z;
				if(General.Settings.GZStretchModels) v.z *= VERTICAL_STRETCH; //GZDoom vertical stretch hack

                //add zOffset
                v.z += mde.zOffset;

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

                List<int> polyIndecesList = new List<int>();
                List<int> uvIndecesList = new List<int>();
                List<Vector2> uvCoordsList = new List<Vector2>();
                List<WorldVertex> vertList = new List<WorldVertex>();

                //polygons
                if (s.Position != ofs_tris + start)
                    s.Position = ofs_tris + start;

                for (int i = 0; i < num_tris; i++) {
                    polyIndecesList.Add((int)br.ReadInt16());
                    polyIndecesList.Add((int)br.ReadInt16());
                    polyIndecesList.Add((int)br.ReadInt16());

                    uvIndecesList.Add((int)br.ReadInt16());
                    uvIndecesList.Add((int)br.ReadInt16());
                    uvIndecesList.Add((int)br.ReadInt16());
                }

                //UV coords
                if (s.Position != ofs_uv + start)
                    s.Position = ofs_uv + start;

                for (int i = 0; i < num_uv; i++) 
                    uvCoordsList.Add(new Vector2((float)br.ReadInt16() / texWidth, (float)br.ReadInt16() / texHeight));

                //first frame
                //header
                if (s.Position != ofs_animFrame + start)
                    s.Position = ofs_animFrame + start;

                Vector3 scale = new Vector3((float)br.ReadSingle(), (float)br.ReadSingle(), (float)br.ReadSingle());
                Vector3 translate = new Vector3((float)br.ReadSingle(), (float)br.ReadSingle(), (float)br.ReadSingle());

                s.Position += 16; //frame name

                //rotation angles
                float angle = mde.AngleOffset - 0.5f * (float)Math.PI; //subtract 90 degrees to get correct rotation
                float angleOfsetCos = (float)Math.Cos(angle);
                float angleOfsetSin = (float)Math.Sin(angle);
                float pitchOfsetCos = (float)Math.Cos(-mde.PitchOffset);
                float pitchOfsetSin = (float)Math.Sin(-mde.PitchOffset);
                float rollOfsetCos = (float)Math.Cos(mde.RollOffset);
                float rollOfsetSin = (float)Math.Sin(mde.RollOffset);

                //verts
                for (int i = 0; i < num_verts; i++) {
                    WorldVertex v = new WorldVertex();

                    v.x = ((float)br.ReadByte() * scale.X + translate.X);
                    v.y = ((float)br.ReadByte() * scale.Y + translate.Y);
                    v.z = ((float)br.ReadByte() * scale.Z + translate.Z);

                    //rotate it
                    float rx = angleOfsetCos * v.x - angleOfsetSin * v.y;
                    float ry = angleOfsetSin * v.x + angleOfsetCos * v.y;
                    v.y = ry;
                    v.x = rx;

                    if (mde.PitchOffset != 0) {
                        ry = pitchOfsetCos * v.y - pitchOfsetSin * v.z;
                        float rz = pitchOfsetSin * v.y + pitchOfsetCos * v.z;
                        v.z = rz;
                        v.y = ry;
                    }
                    if (mde.RollOffset != 0) {
                        rx = rollOfsetCos * v.x - rollOfsetSin * v.z;
                        float rz = rollOfsetSin * v.x + rollOfsetCos * v.z;
                        v.z = rz;
                        v.x = rx;
                    }

                    //scale it
                    v.x *= mde.Scale.X;
                    v.y *= mde.Scale.Y;
					v.z *= mde.Scale.Z;
					if(General.Settings.GZStretchModels) v.z *= VERTICAL_STRETCH; //GZDoom vertical stretch hack

                    //add zOffset
                    v.z += mde.zOffset;

                    vertList.Add(v);

                    s.Position += 1; //vertex normal
                }

                for (int i = 0; i < polyIndecesList.Count; i++) {
                    WorldVertex v = vertList[polyIndecesList[i]];
                    
                    //bounding box
                    BoundingBoxTools.UpdateBoundingBoxSizes(ref bbs, new WorldVertex(v.y, v.x, v.z));

                    //uv
					float tu = uvCoordsList[uvIndecesList[i]].X;
                    float tv = uvCoordsList[uvIndecesList[i]].Y;

					//uv-coordinates already set?
					if(v.c == -1 && (v.u != tu || v.v != tv)) { 
						//add a new vertex
						vertList.Add(new WorldVertex(v.x, v.y, v.z, -1, tu, tv));
						polyIndecesList[i] = vertList.Count - 1;
					} else {
						v.u = tu;
						v.v = tv;
						v.c = -1; //set color to white

						//return to proper place
						vertList[polyIndecesList[i]] = v;
					}
                }

                //indeces for rendering current mesh in 2d
                int[] indeces2d_arr = CreateLineListIndeces(polyIndecesList);

                //mesh
                Mesh mesh = new Mesh(D3DDevice, polyIndecesList.Count / 3, vertList.Count, MeshFlags.Use32Bit | MeshFlags.IndexBufferManaged | MeshFlags.VertexBufferManaged, General.Map.Graphics.Shaders.World3D.VertexElements);

                using (DataStream stream = mesh.LockVertexBuffer(LockFlags.None)) {
                    stream.WriteRange(vertList.ToArray());
                }
                mesh.UnlockVertexBuffer();

                using (DataStream stream = mesh.LockIndexBuffer(LockFlags.None)) {
                    stream.WriteRange(polyIndecesList.ToArray());
                }
                mesh.UnlockIndexBuffer();

                mesh.OptimizeInPlace(MeshOptimizeFlags.AttributeSort);
                mde.Model.Meshes.Add(mesh);

                //2d data
                IndexBuffer indeces2d = new IndexBuffer(D3DDevice, 4 * indeces2d_arr.Length, Usage.WriteOnly, Pool.Managed, false);
                DataStream stream2d = indeces2d.Lock(0, 0, LockFlags.None);
                stream2d.WriteRange(indeces2d_arr);
                indeces2d.Unlock();

                mde.Model.Indeces2D.Add(indeces2d);
                mde.Model.NumIndeces2D.Add((int)polyIndecesList.Count);
            }
            return "";
        }

        //this creates list of vertex indeces for rendering using LineList method
        private static int[] CreateLineListIndeces(List<int> polyIndecesList) {
            int[] indeces2d_arr = new int[polyIndecesList.Count * 2];
            int ind1, ind2, ind3;
            for (int i = 0; i < polyIndecesList.Count; i += 3) {
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