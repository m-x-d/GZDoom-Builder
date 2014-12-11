#region ================== Namespaces

using System.Collections.Generic;
using CodeImp.DoomBuilder.Geometry;
using CodeImp.DoomBuilder.GZBuilder.MD3;
using SlimDX;
using SlimDX.Direct3D9;

#endregion

namespace CodeImp.DoomBuilder.GZBuilder.Data
{
	internal sealed class ModelData
	{
		#region ================== Variables

		private ModelLoadState loadstate;

		#endregion

		#region ================== Properties

		internal List<string> ModelNames;
		internal List<string> TextureNames;

		internal GZModel Model;

		internal Matrix Scale;
		internal Vector2D OffsetXY;
		internal float OffsetZ;

		internal float AngleOffset; //in radians
		internal float PitchOffset; //in radians
		internal float RollOffset; //in radians
		internal bool OverridePalette; //used for voxel models only
		internal bool InheritActorPitch;
		internal bool InheritActorRoll;

		internal bool IsVoxel;

		public ModelLoadState LoadState { get { return loadstate; } internal set { loadstate = value; } }

		#endregion

		#region ================== Constructor / Disposer

		internal ModelData() 
		{
			ModelNames = new List<string>();
			TextureNames = new List<string>();
			Scale = Matrix.Identity;
			OffsetXY = new Vector2D();
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

		#endregion
	}
}
