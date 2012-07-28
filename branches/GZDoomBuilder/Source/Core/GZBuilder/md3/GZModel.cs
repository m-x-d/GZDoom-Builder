using System.Collections.Generic;
using SlimDX;
using SlimDX.Direct3D9;
using CodeImp.DoomBuilder.Geometry;


namespace CodeImp.DoomBuilder.GZBuilder.MD3
{
    internal class GZModel {
        internal List<Mesh> Meshes;
        internal List<Texture> Textures;
        internal byte NUM_MESHES; //can't be greater than 255, can it?
        internal Vector3[] BoundingBox;
        internal List<IndexBuffer> Indeces2D;
        internal List<short> NumIndeces2D;

        internal GZModel() {
            Meshes = new List<Mesh>();
            Textures = new List<Texture>();
            Indeces2D = new List<IndexBuffer>();
            NumIndeces2D = new List<short>();
        }
    }

}