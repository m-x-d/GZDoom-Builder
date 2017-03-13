﻿using SlimDX;

namespace CodeImp.DoomBuilder.GZBuilder.Data
{
	public sealed class DynamicLightData 
	{
		public DynamicLightType Type; //holds DynamicLightType
		public Color3 Color;
		public int PrimaryRadius;
		public int SecondaryRadius;
		public int Interval;
		public Vector3 Offset;
        public DynamicLightRenderStyle Style;
        public bool DontLightSelf;

		public DynamicLightData() 
		{
            Style = DynamicLightRenderStyle.NORMAL;
			Color = new Color3();
			Offset = new Vector3();
		}
	}

	public enum DynamicLightType
	{
		NONE = -1,
		NORMAL = 0,
		PULSE = 1,
		FLICKER = 2,
		SECTOR = 3,
		RANDOM = 4,
		VAVOOM = 1502,
		VAVOOM_COLORED = 1503,
	}

	//divide these by 100 to get light color alpha
	public enum DynamicLightRenderStyle
	{
        NEGATIVE = 100,
        NORMAL = 99,
        ATTENUATED = 98,
        VAVOOM = 50,
		ADDITIVE = 25,
        NONE = 0,
    }
}
