//using SlimDX;

namespace CodeImp.DoomBuilder.GZBuilder.Data
{
    /*public struct GZDoomLight
    {
        public Color3 Color;
        public Vector4 PosAndRadius;
        public int MinRadius;
        public int MaxRadius;
        public int Type; //listed in GZDoomLightType
        public int RenderStyle; //normal, additive, negative
        public int CameraDistance;
        public float AnimationSpeed;
    }*/

    public enum GZDoomLightType : int
    {
        NORMAL = 0,
        PULSE = 1,
        FLICKER = 2,
        SECTOR = 3,
        RANDOM = 4,
        VAVOOM = 1502,
        VAVOOM_COLORED = 1503
    }

    //divide these by 100 to get light color alpha
    public enum GZDoomLightRenderStyle : int
    {
        NORMAL = 75,
        ADDITIVE = 25,
        NEGATIVE = 100
    }
}
