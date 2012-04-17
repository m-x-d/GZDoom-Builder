using System;
using System.Runtime.InteropServices;

using SlimDX;
using SlimDX.Direct3D9;

namespace ColladaDotNet.Pipeline.MD3 {

    [StructLayout(LayoutKind.Sequential)]
    public struct ModelVertex
    {
        public Vector3 Position;
        public Vector3 Normal;
        public int Color;
        public float Tu;
        public float Tv;

        public static int SizeBytes {
            get { return Marshal.SizeOf(typeof(ModelVertex)); }
        }

        public static VertexFormat Format {
            get { return VertexFormat.Position | VertexFormat.Diffuse | VertexFormat.Texture1 | VertexFormat.Normal; }
        }
    }
}
