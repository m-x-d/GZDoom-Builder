/*#region ================== Namespaces

using System;
using SlimDX.Direct3D9;
using CodeImp.DoomBuilder.Rendering;

#endregion

namespace CodeImp.DoomBuilder.GZBuilder.Rendering
{
	internal sealed class SizelessVisualThingCage : IDisposable, ID3DResource
	{
		#region ================== Constants

		private const float RADIUS = 1.0f;

		#endregion

		#region ================== Variables

		private VertexBuffer shape;
		private bool isdisposed;

		#endregion

		#region ================== Properties

		public VertexBuffer Shape { get { return shape; } }

		#endregion

		#region ================== Constructor / Disposer

		public SizelessVisualThingCage() 
		{
			// Create geometry
			ReloadResource();

			// Register as resource
			General.Map.Graphics.RegisterResource(this);
		}

		public void Dispose() 
		{
			// Not already disposed?
			if (!isdisposed)
			{
				// Clean up
				if(shape != null) shape.Dispose();
				shape = null;

				// Unregister resource
				General.Map.Graphics.UnregisterResource(this);

				// Done
				isdisposed = true;
			}
		}

		#endregion

		#region ================== Methods

		// This is called resets when the device is reset
		// (when resized or display adapter was changed)
		public void ReloadResource()
		{
			WorldVertex v0 = new WorldVertex(-RADIUS, -RADIUS, 0f);
			WorldVertex v1 = new WorldVertex(RADIUS, RADIUS, 0f);
			WorldVertex v2 = new WorldVertex(RADIUS, -RADIUS, 0f);
			WorldVertex v3 = new WorldVertex(-RADIUS, RADIUS, 0f);
			WorldVertex v4 = new WorldVertex(0f, 0f, RADIUS);
			WorldVertex v5 = new WorldVertex(0f, 0f, -RADIUS);

			WorldVertex[] vs = new[] { v0, v1, v2, v3, v4, v5 };

			shape = new VertexBuffer(General.Map.Graphics.Device, WorldVertex.Stride * vs.Length, Usage.WriteOnly | Usage.Dynamic, VertexFormat.None, Pool.Default);
			shape.Lock(0, WorldVertex.Stride * vs.Length, LockFlags.None).WriteRange(vs);
			shape.Unlock();
		}

		// This is called before a device is reset
		// (when resized or display adapter was changed)
		public void UnloadResource() 
		{
			// Trash geometry buffer
			if(shape != null) shape.Dispose();
			shape = null;
		}

		#endregion
	}
}*/
