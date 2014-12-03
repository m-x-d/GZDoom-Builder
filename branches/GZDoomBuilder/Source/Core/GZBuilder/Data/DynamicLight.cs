using SlimDX;

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
		public bool Subtractive;
		public bool DontLightSelf;

		public DynamicLightData() 
		{
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
		NONE = 0,
		NORMAL = 99,
		VAVOOM = 50,
		ADDITIVE = 25,
		NEGATIVE = 100,
	}
}
