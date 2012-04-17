using System;
using System.Collections.Generic;
using System.Text;

using SlimDX;
using SlimDX.Direct3D9;

using ColladaDotNet.Pipeline.MD3;

namespace CodeImp.DoomBuilder.GZBuilder.Data
{
    public class ModelDefEntry
    {
        public string Name;
        public string Path;
        public List<string> ModelNames;
        public List<string> TextureNames;

        public GZModel Model;

        public Vector3 Scale;
        public float zOffset;

        public ModelDefEntry() {
            Scale = new Vector3(1, 1, 1);
            zOffset = 0;
            ModelNames = new List<string>();
            TextureNames = new List<string>();
        }
    }
}
