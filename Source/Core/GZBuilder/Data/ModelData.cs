#region ================== Namespaces

using System.Collections.Generic;
using CodeImp.DoomBuilder.GZBuilder.MD3;
using CodeImp.DoomBuilder.Rendering;
using SlimDX;
using SlimDX.Direct3D9;

#endregion

namespace CodeImp.DoomBuilder.GZBuilder.Data
{
	internal sealed class ModelData
	{
		#region ================== Variables

		private ModelLoadState loadstate;
		private Vector3 scale;
		private Matrix transform;
		private Matrix transformstretched;

		#endregion

		#region ================== Properties

		internal List<string> ModelNames;
		internal List<string> TextureNames;
		internal List<string> FrameNames;
		internal List<int> FrameIndices;

		internal GZModel Model;

		internal Vector3 Scale { get { return scale; } }
		internal Matrix Transform { get { return (General.Settings.GZStretchView ? transformstretched : transform); } }
		internal bool OverridePalette; //used for voxel models only 
		internal bool InheritActorPitch;
		internal bool InheritActorRoll;

		internal bool IsVoxel;

		// Hashing
		private static int hashcounter;
		private readonly int hashcode;

		// Disposing
		private bool isdisposed;

		public ModelLoadState LoadState { get { return loadstate; } internal set { loadstate = value; } }

		#endregion

		#region ================== Constructor / Disposer

		internal ModelData() 
		{
			ModelNames = new List<string>();
			TextureNames = new List<string>();
			FrameNames = new List<string>();
			FrameIndices = new List<int>();
			transform = Matrix.Identity;
			transformstretched = Matrix.Identity;
			hashcode = hashcounter++;
		}

		internal void Dispose() 
		{
			// Not already disposed?
			if(!isdisposed) 
			{
				// Clean up
				if(Model != null)
				{
					foreach (Mesh mesh in Model.Meshes) mesh.Dispose();
					foreach (Texture t in Model.Textures) t.Dispose();
					loadstate = ModelLoadState.None;
				}

				// Done
				isdisposed = true;
			}
		}

		internal void SetTransform(Matrix rotation, Matrix offset, Vector3 scale)
		{
			this.scale = scale;
			this.transform = rotation * Matrix.Scaling(scale) * offset;
			this.transformstretched = rotation * Matrix.Scaling(scale.X, scale.Y, scale.Z * Renderer3D.GZDOOM_INVERTED_VERTICAL_VIEW_STRETCH) * offset;
		}

		//mxd. This greatly speeds up Dictionary lookups
		public override int GetHashCode()
		{
			return hashcode;
		}

		#endregion
	}
}
