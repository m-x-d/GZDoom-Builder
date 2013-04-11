using SlimDX.Direct3D9;
using CodeImp.DoomBuilder.Rendering;

namespace CodeImp.DoomBuilder.GZBuilder.Rendering
{
	internal sealed class VisualVertexHandle
	{
		public VertexBuffer Upper { get { return upper; } }
		private VertexBuffer upper;

		public VertexBuffer Lower { get { return lower; } }
		private VertexBuffer lower;

		public VisualVertexHandle(Device device) {
			float radius = General.Settings.GZVisualVertexSize;

			WorldVertex c = new WorldVertex();
			WorldVertex v0 = new WorldVertex(-radius, -radius, -radius);
			WorldVertex v1 = new WorldVertex(-radius, radius, -radius);
			WorldVertex v2 = new WorldVertex(radius, radius, -radius);
			WorldVertex v3 = new WorldVertex(radius, -radius, -radius);

			WorldVertex v4 = new WorldVertex(-radius, -radius, radius);
			WorldVertex v5 = new WorldVertex(-radius, radius, radius);
			WorldVertex v6 = new WorldVertex(radius, radius, radius);
			WorldVertex v7 = new WorldVertex(radius, -radius, radius);

			WorldVertex[] vu = new WorldVertex[]{ c, v0,
												  c, v1,
												  c, v2,
												  c, v3,

												  v0, v1,
                                                  v1, v2,
                                                  v2, v3,
                                                  v3, v0 };

			upper = new VertexBuffer(device, WorldVertex.Stride * vu.Length, Usage.WriteOnly | Usage.Dynamic, VertexFormat.None, Pool.Default);
			upper.Lock(0, WorldVertex.Stride * vu.Length, LockFlags.None).WriteRange<WorldVertex>(vu);
			upper.Unlock();

			WorldVertex[] vl = new WorldVertex[]{ c, v4,
												  c, v5,
												  c, v6,
												  c, v7,

												  v4, v5, 
                                                  v5, v6,
                                                  v6, v7,
                                                  v7, v4, };

			lower = new VertexBuffer(device, WorldVertex.Stride * vl.Length, Usage.WriteOnly | Usage.Dynamic, VertexFormat.None, Pool.Default);
			lower.Lock(0, WorldVertex.Stride * vl.Length, LockFlags.None).WriteRange<WorldVertex>(vl);
			lower.Unlock();
		}

		public void Dispose() {
			if(upper != null) upper.Dispose();
			if(lower != null) lower.Dispose();
		}
	}
}