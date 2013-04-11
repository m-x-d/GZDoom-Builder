using System.Collections.Generic;
using SlimDX;
using SlimDX.Direct3D9;
using CodeImp.DoomBuilder.GZBuilder.MD3;

namespace CodeImp.DoomBuilder.GZBuilder.Data
{
    internal sealed class ModeldefEntry
    {
        internal string ClassName;
        internal List<string> ModelNames;
        internal List<string> TextureNames;

        internal GZModel Model;

        internal Vector3 Scale;
        internal float zOffset;

        internal float AngleOffset; //in radians
        internal float PitchOffset; //in radians
        internal float RollOffset; //in radians

        internal ModeldefEntry() {
            ModelNames = new List<string>();
            TextureNames = new List<string>();
        }

        internal void Dispose() {
            if (Model != null) {
                foreach (Mesh mesh in Model.Meshes)
                    mesh.Dispose();

                foreach (Texture t in Model.Textures)
                    t.Dispose();
            }
        }
    }
}
