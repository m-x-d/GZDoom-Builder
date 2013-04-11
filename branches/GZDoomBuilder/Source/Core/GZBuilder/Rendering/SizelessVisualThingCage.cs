using SlimDX.Direct3D9;
using CodeImp.DoomBuilder.Rendering;

namespace CodeImp.DoomBuilder.GZBuilder.Rendering
{
	internal sealed class SizelessVisualThingCage
	{
		public VertexBuffer Shape { get { return shape; } }
		private VertexBuffer shape;

		public SizelessVisualThingCage(Device device) {
			float radius = 1.0f;

			WorldVertex v0 = new WorldVertex(-radius, -radius, 0f);
			WorldVertex v1 = new WorldVertex(radius, radius, 0f);
			WorldVertex v2 = new WorldVertex(radius, -radius, 0f);
			WorldVertex v3 = new WorldVertex(-radius, radius, 0f);
			WorldVertex v4 = new WorldVertex(0f, 0f, radius);
			WorldVertex v5 = new WorldVertex(0f, 0f, -radius);

			WorldVertex[] vs = new WorldVertex[]{ v0, v1, v2, v3, v4, v5 };

			shape = new VertexBuffer(device, WorldVertex.Stride * vs.Length, Usage.WriteOnly | Usage.Dynamic, VertexFormat.None, Pool.Default);
			shape.Lock(0, WorldVertex.Stride * vs.Length, LockFlags.None).WriteRange<WorldVertex>(vs);
			shape.Unlock();
		}

		public void Dispose() {
			if(shape != null) shape.Dispose();
		}
	}
}
