using System.Collections.Generic;
using SlimDX;
using SlimDX.Direct3D9;
using CodeImp.DoomBuilder.GZBuilder.MD3;

namespace CodeImp.DoomBuilder.GZBuilder.Data
{
	internal sealed class ModelData
	{
		internal List<string> ModelNames;
		internal List<string> TextureNames;

		internal GZModel Model;

		private ModelLoadState loadstate;
		public ModelLoadState LoadState { get { return loadstate; } internal set { loadstate = value; } }

		internal Matrix Scale;
		internal float zOffset;

		internal float AngleOffset; //in radians
		internal float PitchOffset; //in radians
		internal float RollOffset; //in radians
		internal bool OverridePalette; //used for voxel models only
		internal bool IsVoxel;
		internal bool InheritActorPitch;
		internal bool InheritActorRoll;

		internal ModelData() 
		{
			ModelNames = new List<string>();
			TextureNames = new List<string>();
			Scale = Matrix.Identity;
		}

		internal void Dispose() 
		{
			if (Model != null) 
			{
				foreach (Mesh mesh in Model.Meshes) mesh.Dispose();
				foreach (Texture t in Model.Textures) t.Dispose();
				loadstate = ModelLoadState.None;
			}
		}
	}
}
