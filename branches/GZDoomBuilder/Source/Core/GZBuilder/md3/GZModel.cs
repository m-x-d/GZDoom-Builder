using System.Collections.Generic;
using SlimDX;
using SlimDX.Direct3D9;
using CodeImp.DoomBuilder.Geometry;


namespace ColladaDotNet.Pipeline.MD3
{
    public class GZModel {
        public List<Mesh> Meshes;
        public List<Texture> Textures;
        public byte NUM_MESHES = 0; //can't be greater than 255, can it?
        public Vector3[] BoundingBox;
        public List<IndexBuffer> Indeces2D;
        public List<short> NumIndeces2D;
        public float Angle = 0; //crappy way to set rotation to md2 models...

        public GZModel() {
            Meshes = new List<Mesh>();
            Textures = new List<Texture>();
            Indeces2D = new List<IndexBuffer>();
            NumIndeces2D = new List<short>();
        }
    }

}