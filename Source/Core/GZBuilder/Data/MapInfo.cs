using System;
using SlimDX;
using SlimDX.Direct3D9;

namespace CodeImp.DoomBuilder.GZBuilder.Data {
    public class MapInfo {
        public string Sky1;
        public float Sky1ScrollSpeed;
        public string Sky2;
        public float Sky2ScrollSpeed;
        public bool DoubleSky;
        public bool HasFadeColor;
        public Color4 FadeColor;
        public bool HasOutsideFogColor;
        public Color4 OutsideFogColor;

        public bool EvenLighting;
        public bool SmoothLighting;
        public int VertWallShade;
        public int HorizWallShade;

        public MapInfo() {
            VertWallShade = 16;
            HorizWallShade = -16;
        }
    }
}
