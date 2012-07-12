using System;
using System.Collections.Generic;
using System.Text;

using SlimDX;
using SlimDX.Direct3D9;

using CodeImp.DoomBuilder.GZBuilder.MD3;

namespace CodeImp.DoomBuilder.GZBuilder.Data
{
    internal sealed class ModeldefEntry
    {
        internal string ClassName;
        internal string Path; //this holds Path parameter of MODELDEF entry
        internal List<string> ModelNames;
        internal List<string> TextureNames;
        internal string Location; //this holds location of resource, from which modeldef was loaded

        internal GZModel Model;

        internal Vector3 Scale;
        internal float zOffset;

        internal ModeldefEntry() {
            ModelNames = new List<string>();
            TextureNames = new List<string>();
        }

        internal void Dispose() {
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
