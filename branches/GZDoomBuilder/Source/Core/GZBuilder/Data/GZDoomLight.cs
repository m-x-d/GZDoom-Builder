using System;
using SlimDX;

namespace CodeImp.DoomBuilder.GZBuilder.Data
{
    public class GZDoomLight {
        public int Type; //holds GZDoomLightType
        public Color3 Color;
        public int PrimaryRadius;
        public int SecondaryRadius;
        public int Interval;
        public Vector3 Offset;
        public bool Subtractive;
        public bool DontLightSelf;

        public GZDoomLight() {
            Color = new Color3();
            Offset = new Vector3();
        }

        public static int[] GetDefaultLightSettings(int type) {
            int light_id = Array.IndexOf(GZBuilder.GZGeneral.GZ_LIGHTS, type);
            if (light_id != -1) {
                int[] args = new int[5];
                
                if (light_id == (int)GZDoomLightType.VAVOOM_COLORED) {
                    args[0] = 16;
                    args[1] = 255;
                    args[2] = 255;
                    args[3] = 255;
                } else if (light_id == (int)GZDoomLightType.VAVOOM) {
                    args[0] = 16;
                } else {
                    int n;
                    if (light_id < GZBuilder.GZGeneral.GZ_LIGHT_TYPES[0]) {
                        n = 0;
                    } else if (light_id < GZBuilder.GZGeneral.GZ_LIGHT_TYPES[1]) {
                        n = 10;
                    } else {
                        n = 20;
                    }
                    light_id = type - 9800 - n;

                    args[0] = 255;
                    args[1] = 255;
                    args[2] = 255;

                    if (light_id == (int)GZDoomLightType.SECTOR)
                        args[3] = 4;
                    else
                        args[3] = 64;

                    if (Array.IndexOf(GZBuilder.GZGeneral.GZ_ANIMATED_LIGHT_TYPES, light_id) != -1) {
                        args[4] = 32;
                    }
                }
                return args;
            }
            return null;
        }
    }

    public enum GZDoomLightType : int
    {
        NORMAL = 0,
        PULSE = 1,
        FLICKER = 2,
        SECTOR = 3,
        RANDOM = 4,
        VAVOOM = 1502,
        VAVOOM_COLORED = 1503,
    }

    //divide these by 100 to get light color alpha
    public enum GZDoomLightRenderStyle : int
    {
        NORMAL = 99,
        VAVOOM = 50,
        ADDITIVE = 25,
        NEGATIVE = 100,
    }
}
