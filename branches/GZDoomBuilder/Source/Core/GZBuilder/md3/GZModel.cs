using System.Collections.Generic;
using SlimDX;
using SlimDX.Direct3D9;
using CodeImp.DoomBuilder.Geometry;


namespace CodeImp.DoomBuilder.GZBuilder.MD3
{
    internal class GZModel {
        internal List<Mesh> Meshes;
        internal List<Texture> Textures;
        internal Vector3[] BoundingBox;

        internal GZModel() {
            Meshes = new List<Mesh>();
            Textures = new List<Texture>();
        }
    }

}