using System;
using SlimDX;
using SlimDX.Direct3D9;
using CodeImp.DoomBuilder.Rendering;

namespace CodeImp.DoomBuilder.GZBuilder.Data
{
    sealed class ThingBoundingBox : IDisposable
    {
        public VertexBuffer cage;
        public VertexBuffer arrow;
        
        public ThingBoundingBox(Device device) {
            WorldVertex v0 = new WorldVertex(-1.0f, -1.0f, 0.0f);
            WorldVertex v1 = new WorldVertex(-1.0f, 1.0f, 0.0f);
            WorldVertex v2 = new WorldVertex(1.0f, 1.0f, 0.0f);
            WorldVertex v3 = new WorldVertex(1.0f, -1.0f, 0.0f);

            WorldVertex v4 = new WorldVertex(-1.0f, -1.0f, 1.0f);
            WorldVertex v5 = new WorldVertex(-1.0f, 1.0f, 1.0f);
            WorldVertex v6 = new WorldVertex(1.0f, 1.0f, 1.0f);
            WorldVertex v7 = new WorldVertex(1.0f, -1.0f, 1.0f);

            //cage
            WorldVertex[] cageVerts = new WorldVertex[] { v0, v1,
                                                          v1, v2,
                                                          v2, v3,
                                                          v3, v0,
                                                          v4, v5, 
                                                          v5, v6,
                                                          v6, v7,
                                                          v7, v4,
                                                          v0, v4,
                                                          v1, v5,
                                                          v2, v6,
                                                          v3, v7 };

            cage = new VertexBuffer(device, WorldVertex.Stride * cageVerts.Length, Usage.WriteOnly | Usage.Dynamic, VertexFormat.None, Pool.Default);
            cage.Lock(0, WorldVertex.Stride * cageVerts.Length, LockFlags.None).WriteRange<WorldVertex>(cageVerts);
            cage.Unlock();
            
            //arrow
            WorldVertex a0 = new WorldVertex(); //start
            WorldVertex a1 = new WorldVertex(1.5f, 0.0f, 0.0f); //end
            WorldVertex a2 = new WorldVertex(1.1f, 0.2f, 0.2f);
            WorldVertex a3 = new WorldVertex(1.1f, -0.2f, 0.2f);
            WorldVertex a4 = new WorldVertex(1.1f, 0.2f, -0.2f);
            WorldVertex a5 = new WorldVertex(1.1f, -0.2f, -0.2f);

            WorldVertex[] arrowVerts = new WorldVertex[] {a0, a1,
                                                          a1, a2,
                                                          a1, a3,
                                                          a1, a4,
                                                          a1, a5};

            arrow = new VertexBuffer(device, WorldVertex.Stride * arrowVerts.Length, Usage.WriteOnly | Usage.Dynamic, VertexFormat.None, Pool.Default);
            arrow.Lock(0, WorldVertex.Stride * arrowVerts.Length, LockFlags.None).WriteRange<WorldVertex>(arrowVerts);
            arrow.Unlock();
        }

        public void Dispose() {
            if (arrow != null) arrow.Dispose();
            if (cage != null) cage.Dispose();
        }
    }
}
