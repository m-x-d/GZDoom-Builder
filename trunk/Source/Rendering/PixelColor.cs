using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Drawing;

namespace CodeImp.DoomBuilder.Rendering
{
	public struct PixelColor
	{
		#region ================== Variables

		// Members
		public byte b;
		public byte g;
		public byte r;
		public byte a;

		#endregion

		#region ================== Constructors

		// Constructor
		public PixelColor(byte a, byte r, byte g, byte b)
		{
			// Initialize
			this.a = a;
			this.r = r;
			this.g = g;
			this.b = b;
		}

		// Constructor
		public PixelColor(PixelColor p, byte a)
		{
			// Initialize
			this.a = a;
			this.r = p.r;
			this.g = p.g;
			this.b = p.b;
		}

		#endregion

		#region ================== Static Methods

		// Construct from color
		public static PixelColor FromColor(Color c)
		{
			return new PixelColor(c.A, c.R, c.G, c.B);
		}

		// Construct from int
		public static PixelColor FromInt(int c)
		{
			return FromColor(Color.FromArgb(c));
		}

		#endregion

		#region ================== Methods

		#endregion
	}
}
