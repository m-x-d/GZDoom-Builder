using System.Collections.Generic;
using SlimDX;
using SlimDX.Direct3D9;
using CodeImp.DoomBuilder.GZBuilder.MD3;
using CodeImp.DoomBuilder.Data;
using System.IO;
using System;
using System.Text;
using CodeImp.DoomBuilder.Rendering;
using CodeImp.DoomBuilder.Geometry;
using CodeImp.DoomBuilder.IO;
using System.Drawing;
using System.Drawing.Imaging;

namespace CodeImp.DoomBuilder.GZBuilder.Data
{
    internal sealed class ModelData
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
		
		internal string ClassName;
        internal List<string> ModelNames;
        internal List<string> TextureNames;

        internal GZModel Model;

		private ModelLoadState loadstate;
		public ModelLoadState LoadState { get { return loadstate; } internal set { loadstate = value; } }

        internal Vector3 Scale;
        internal float zOffset;

        internal float AngleOffset; //in radians
        internal float PitchOffset; //in radians
        internal float RollOffset; //in radians

        internal ModelData() {
            ModelNames = new List<string>();
            TextureNames = new List<string>();
        }

        internal void Dispose() {
            if (Model != null) {
                foreach (Mesh mesh in Model.Meshes)
                    mesh.Dispose();

                foreach (Texture t in Model.Textures)
                    t.Dispose();

				loadstate = ModelLoadState.None;
            }
		}
	}
}
