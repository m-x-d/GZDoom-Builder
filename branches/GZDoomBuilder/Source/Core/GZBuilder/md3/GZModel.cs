using System.Collections.Generic;
using SlimDX;
using SlimDX.Direct3D9;
using CodeImp.DoomBuilder.Geometry;


namespace ColladaDotNet.Pipeline.MD3
{
    public class GZModel {
        /*public List<Mesh> meshes;
        public List<Texture> textures;
        public List<short> indeces2D;
        public List<Vector2D> verts2D;
        public int NUM_MESHES = 0;*/
        public List<Mesh> Meshes;
        public List<Texture> Textures;
        public byte NUM_MESHES = 0; //can't be greater than 255, can it?
        public Vector3[] BoundingBox;
        public List<IndexBuffer> Indeces2D;
        public List<short> NumIndeces2D;

        public GZModel() {
            Meshes = new List<Mesh>();
            Textures = new List<Texture>();
            //indeces2D = new List<short>();
            //verts2D = new List<Vector2D>();
            Indeces2D = new List<IndexBuffer>();
            NumIndeces2D = new List<short>();
        }
    }

}