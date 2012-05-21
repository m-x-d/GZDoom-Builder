using System;
using System.Collections.Generic;
using System.Text;

using SlimDX;
using SlimDX.Direct3D9;

using CodeImp.DoomBuilder.GZBuilder.MD3;

namespace CodeImp.DoomBuilder.GZBuilder.Data
{
    public class ModeldefEntry
    {
        public string ClassName;
        public string Path; //this holds Path parameter of MODELDEF entry
        public List<string> ModelNames;
        public List<string> TextureNames;
        public string Location; //this holds location of resource, from which modeldef was loaded

        public GZModel Model;

        public Vector3 Scale;
        public float zOffset;

        public ModeldefEntry() {
            ModelNames = new List<string>();
            TextureNames = new List<string>();
        }

        public void Dispose() {
            if (Model != null) {
                foreach (IndexBuffer ib in Model.Indeces2D)
                    ib.Dispose();

                foreach (Mesh mesh in Model.Meshes)
                    mesh.Dispose();

                foreach (Texture t in Model.Textures)
                    t.Dispose();
            }
        }
    }
}
