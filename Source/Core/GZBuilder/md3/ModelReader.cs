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
using CodeImp.DoomBuilder.Geometry;

//mxd. Original version taken from here: http://colladadotnet.codeplex.com/SourceControl/changeset/view/40680
namespace CodeImp.DoomBuilder.GZBuilder.MD3
{
	internal static class ModelReader
    {
		private const float VERTICAL_STRETCH = 1 / 1.2f;

		private class MD3LoadResult
		{
			public List<string> Skins;
			public List<Mesh> Meshes;
			public string Errors;

			public MD3LoadResult() {
				Skins = new List<string>();
				Meshes = new List<Mesh>();
			}
		}
		
        public static void Load(ref ModeldefEntry mde, List<DataReader> containers, Device device) {
            mde.Model = new GZModel();
            BoundingBoxSizes bbs = new BoundingBoxSizes();
			MD3LoadResult result = new MD3LoadResult();

            //load models and textures
			for(int i = 0; i < mde.ModelNames.Count; i++) {
				//need to use model skins?
				bool useSkins = string.IsNullOrEmpty(mde.TextureNames[i]);
			
				//load mesh
				MemoryStream ms = LoadFile(containers, mde.ModelNames[i], true);
				if(ms == null) continue;

				string ext = Path.GetExtension(mde.ModelNames[i]);

				if(ext == ".md3")
					result = ReadMD3Model(ref bbs, mde, useSkins, ms, device);
				else if(ext == ".md2")
					result = ReadMD2Model(ref bbs, mde, ms, device);
				else
					result.Errors = "model format is not supported";

				ms.Close();
				ms.Dispose();

				//got errors?
				if(!String.IsNullOrEmpty(result.Errors)) {
					General.ErrorLogger.Add(ErrorType.Error, "ModelLoader: error while loading " + mde.ModelNames[i] + ": " + result.Errors);
					continue;
				} else {
					//add loaded data to ModeldefEntry
					mde.Model.Meshes.AddRange(result.Meshes);

					//prepare UnknownTexture3D... just in case :)
					if(General.Map.Data.UnknownTexture3D.Texture == null || General.Map.Data.UnknownTexture3D.Texture.Disposed)
						General.Map.Data.UnknownTexture3D.CreateTexture();

					//load texture
					List<string> errors = new List<string>();

					//texture has unsupported extension?
					if(mde.TextureNames[i] == TextureData.INVALID_TEXTURE) {
						foreach(Mesh m in result.Meshes)
							mde.Model.Textures.Add(General.Map.Data.UnknownTexture3D.Texture);
					
					//texture not defined in MODELDEF?
					} else if(useSkins) {
						//try to use model's own skins 
						for(int m = 0; m < result.Meshes.Count; m++) {
							if(string.IsNullOrEmpty(result.Skins[m])) {
								mde.Model.Textures.Add(General.Map.Data.UnknownTexture3D.Texture);
								errors.Add("texture not found in MODELDEF or model skin.");
								continue;
							}

							string path = result.Skins[m].Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
							ext = Path.GetExtension(path);

							if(Array.IndexOf(TextureData.SUPPORTED_TEXTURE_EXTENSIONS, ext) == -1) {
								mde.Model.Textures.Add(General.Map.Data.UnknownTexture3D.Texture);
								errors.Add("image format '" + ext + "' is not supported!");
								continue;
							}

							//relative path?
							if(path.IndexOf(Path.DirectorySeparatorChar) == -1)
								path = Path.Combine(Path.GetDirectoryName(mde.ModelNames[m]), path);

							Texture t = LoadTexture(containers, path, device);

							if(t == null) {
								mde.Model.Textures.Add(General.Map.Data.UnknownTexture3D.Texture);
								errors.Add("unable to load skin '" + result.Skins[m] + "'");
								continue;
							} 

							mde.Model.Textures.Add(t);
						}

					//try to use texture loaded from MODELDEFS
					} else {
						Texture t = LoadTexture(containers, mde.TextureNames[i], device);
						if(t == null) {
							mde.Model.Textures.Add(General.Map.Data.UnknownTexture3D.Texture);
							errors.Add("unable to load texture '" + mde.TextureNames[i] + "'");
						} else {
							mde.Model.Textures.Add(t);
						}
					}

					//report errors
					if(errors.Count > 0) {
						foreach(string e in errors)
							General.ErrorLogger.Add(ErrorType.Error, "ModelLoader: error while loading '" + mde.ModelNames[i] + "':" + e);
					}
				}
			}

			//clear unneeded data
			mde.TextureNames = null;
			mde.ModelNames = null;

			if(mde.Model.Meshes == null || mde.Model.Meshes.Count == 0) {
                mde.Model = null;
                return;
            }

            mde.Model.BoundingBox = BoundingBoxTools.CalculateBoundingBox(bbs);
        }

		private static MD3LoadResult ReadMD3Model(ref BoundingBoxSizes bbs, ModeldefEntry mde, bool useSkins, MemoryStream s, Device device) {
            long start = s.Position;
			MD3LoadResult result = new MD3LoadResult();

            using (var br = new BinaryReader(s, Encoding.ASCII)) {
                string magic = ReadString(br, 4);
                if (magic != "IDP3"){
					result.Errors = "magic should be 'IDP3', not '" + magic + "'";
					return result;
				}

                s.Position += 80;
                int numSurfaces = br.ReadInt32();
                s.Position += 12;
                int ofsSurfaces = br.ReadInt32();

                if (s.Position != ofsSurfaces + start)
                    s.Position = ofsSurfaces + start;

                List<int> polyIndecesList = new List<int>();
                List<WorldVertex> vertList = new List<WorldVertex>();

				Dictionary<string, List<List<int>>> polyIndecesListsPerTexture = new Dictionary<string, List<List<int>>>();
				Dictionary<string, List<WorldVertex>> vertListsPerTexture = new Dictionary<string, List<WorldVertex>>();
				Dictionary<string, List<int>> vertexOffsets = new Dictionary<string, List<int>>();

                string error = "";
                for (int c = 0; c < numSurfaces; c++) {
					string skin = "";
					error = ReadSurface(ref bbs, ref skin, br, polyIndecesList, vertList, mde);

					if(!string.IsNullOrEmpty(error)) {
						result.Errors = error;
						return result;
					}

					if(useSkins) {
						if(polyIndecesListsPerTexture.ContainsKey(skin)) {
							polyIndecesListsPerTexture[skin].Add(polyIndecesList);
							vertListsPerTexture[skin].AddRange(vertList.ToArray());
							vertexOffsets[skin].Add(vertList.Count);
						} else {
							polyIndecesListsPerTexture.Add(skin, new List<List<int>>() { polyIndecesList } );
							vertListsPerTexture.Add(skin, vertList);
							vertexOffsets.Add(skin, new List<int>() { vertList.Count });
						}

						//reset lists
						polyIndecesList = new List<int>();
						vertList = new List<WorldVertex>();
					}
                }

				if(!useSkins) { //create mesh
					CreateMesh(device, ref result, vertList, polyIndecesList);
					result.Skins.Add("");
				} else {
					//create a mesh for each surface texture
					foreach(KeyValuePair<string, List<List<int>>> group in polyIndecesListsPerTexture) {
						polyIndecesList = new List<int>();
						
						//collect indices, fix vertex offsets
						for(int i = 0; i < group.Value.Count; i++) {
							if(i > 0) {
								for(int c = 0; c < group.Value[i].Count; c++ )
									group.Value[i][c] += vertexOffsets[group.Key][i-1];
							}
							polyIndecesList.AddRange(group.Value[i].ToArray());
						}

						CreateMesh(device, ref result, vertListsPerTexture[group.Key], polyIndecesList);
						result.Skins.Add(group.Key.ToLowerInvariant());
					}
				}
            }

            return result;
        }

        private static string ReadSurface(ref BoundingBoxSizes bbs, ref string skin, BinaryReader br, List<int> polyIndecesList, List<WorldVertex> vertList, ModeldefEntry mde) {
            int vertexOffset = vertList.Count;
            long start = br.BaseStream.Position;
            string magic = ReadString(br, 4);
            if (magic != "IDP3")
                return "error while reading surface: Magic should be 'IDP3', not '" + magic + "'";

            br.BaseStream.Position += 76;
            int numVerts = br.ReadInt32(); //Number of Vertex objects defined in this Surface, up to MD3_MAX_VERTS. Current value of MD3_MAX_VERTS is 4096.
            int numTriangles = br.ReadInt32(); //Number of Triangle objects defined in this Surface, maximum of MD3_MAX_TRIANGLES. Current value of MD3_MAX_TRIANGLES is 8192.
            int ofsTriangles = br.ReadInt32(); //Relative offset from SURFACE_START where the list of Triangle objects starts.
			int ofsShaders = br.ReadInt32();
            int ofsST = br.ReadInt32(); //Relative offset from SURFACE_START where the list of ST objects (s-t texture coordinates) starts.
            int ofsNormal = br.ReadInt32(); //Relative offset from SURFACE_START where the list of Vertex objects (X-Y-Z-N vertices) starts.
            int ofsEnd = br.ReadInt32(); //Relative offset from SURFACE_START to where the Surface object ends.

            //polygons
            if (start + ofsTriangles != br.BaseStream.Position)
                br.BaseStream.Position = start + ofsTriangles;

            for (int i = 0; i < numTriangles * 3; i++)
                polyIndecesList.Add( vertexOffset + br.ReadInt32() );

			//shaders
			if(start + ofsShaders != br.BaseStream.Position)
				br.BaseStream.Position = start + ofsShaders;

			skin = ReadString(br, 64); //we are interested only in the first one

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

		private static void CreateMesh(Device device, ref MD3LoadResult result, List<WorldVertex> vertList, List<int> polyIndecesList) {
			//create mesh
			Mesh mesh = new Mesh(device, polyIndecesList.Count / 3, vertList.Count, MeshFlags.Use32Bit | MeshFlags.IndexBufferManaged | MeshFlags.VertexBufferManaged, General.Map.Graphics.Shaders.World3D.VertexElements);

			using(DataStream stream = mesh.LockVertexBuffer(LockFlags.None)) {
				stream.WriteRange(vertList.ToArray());
			}
			mesh.UnlockVertexBuffer();

			using(DataStream stream = mesh.LockIndexBuffer(LockFlags.None)) {
				stream.WriteRange(polyIndecesList.ToArray());
			}

			mesh.OptimizeInPlace(MeshOptimizeFlags.AttributeSort);

			//store in result
			result.Meshes.Add(mesh);
		}

		private static MD3LoadResult ReadMD2Model(ref BoundingBoxSizes bbs, ModeldefEntry mde, MemoryStream s, Device D3DDevice) {
            long start = s.Position;
			MD3LoadResult result = new MD3LoadResult();

            using (var br = new BinaryReader(s, Encoding.ASCII)) {
                string magic = ReadString(br, 4);
				if(magic != "IDP2") {  //magic number: "IDP2"
					result.Errors = "magic should be 'IDP2', not '" + magic + "'";
					return result;
				}

                int modelVersion = br.ReadInt32();
				if(modelVersion != 8) { //MD2 version. Must be equal to 8
					//General.ErrorLogger.Add(ErrorType.Error, "Unable to load model '" + path + "': MD2 version must be 8 but is " + modelVersion);
					result.Errors = "MD2 version must be 8 but is " + modelVersion;
					return result;
				}

                int texWidth = br.ReadInt32();
                int texHeight = br.ReadInt32();
                s.Position += 8; //Size of one frame in bytes
                //s.Position += 4; //Number of textures
                int num_verts = br.ReadInt32(); //Number of vertices
                int num_uv = br.ReadInt32(); //The number of UV coordinates in the model
                int num_tris = br.ReadInt32(); //Number of triangles
                s.Position += 4; //Number of OpenGL commands

				if(br.ReadInt32() == 0) {  //Total number of frames
					//General.ErrorLogger.Add(ErrorType.Error, "Unable to load model '" + path + "': model has 0 frames.");
					result.Errors = "model has 0 frames.";
					return result;
				}

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
				float angle = mde.AngleOffset - Angle2D.PIHALF;// 0.5f * (float)Math.PI; //subtract 90 degrees to get correct rotation
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

				//store in result
				result.Meshes.Add(mesh);
				result.Skins.Add(""); //no skin support for MD2
            }

            return result;
        }

		//util
		private static MemoryStream LoadFile(List<DataReader> containers, string path, bool isModel) {
			foreach(DataReader dr in containers) {
				if(isModel && dr is WADReader) continue;  //models cannot be stored in WADs

				//load file
				if(dr.FileExists(path))
					return dr.LoadFile(path);
			}
			return null;
		}

		private static Texture LoadTexture(List<DataReader> containers, string path, Device device) {
			if(string.IsNullOrEmpty(path)) return null;

			MemoryStream ms = LoadFile(containers, path, false);
			if(ms == null) return null;

			Texture texture = null;

			//create texture
			if(Path.GetExtension(path) == ".pcx") { //pcx format requires special handling...
				FileImageReader fir = new FileImageReader();
				Bitmap bitmap = fir.ReadAsBitmap(ms);

				ms.Close();
				ms.Dispose();

				if(bitmap == null) return null;

				BitmapData bmlock = bitmap.LockBits(new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, bitmap.PixelFormat);
				texture = new Texture(device, bitmap.Width, bitmap.Height, 1, Usage.None, Format.A8R8G8B8, Pool.Managed);

				DataRectangle textureLock = texture.LockRectangle(0, LockFlags.None);
				textureLock.Data.WriteRange(bmlock.Scan0, bmlock.Height * bmlock.Stride);

				bitmap.UnlockBits(bmlock);
				texture.UnlockRectangle(0);

				return texture;
			}

			texture = Texture.FromStream(device, ms);

			ms.Close();
			ms.Dispose();

			return texture;
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