using CodeImp.DoomBuilder.Rendering;

namespace CodeImp.DoomBuilder.GZBuilder.Data
{
	public class GlowingFlatData
	{
		public PixelColor Color;
		public int Height;
		public int Brightness = 255;
		public bool Fullbright;
		public bool Fullblack; // GLOOME only
		public bool Subtractive; // GLOOME only
		public bool CalculateTextureColor;
	}
}
