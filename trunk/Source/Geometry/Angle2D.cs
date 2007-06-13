
/*
 * Copyright (c) 2007 Pascal vd Heiden, www.codeimp.com
 * This program is released under GNU General Public License
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace CodeImp.DoomBuilder.Geometry
{
	internal struct Angle2D
	{
		#region ================== Constants

		public const float PI = (float)Math.PI;
		public const float PI2 = (float)Math.PI * 2f;

		#endregion

		#region ================== Methods

		// This normalizes an angle
		public static float Normalized(float a)
		{
			while(a < 0f) a += PI2;
			while(a > PI2) a -= PI2;
			return a;
		}

		// This returns the difference between two angles
		public static float Difference(float a, float b)
		{
			float d;

			// Calculate delta angle
			d = Normalized(a) - Normalized(b);

			// Make corrections for zero barrier
			if(d < 0f) d += PI2;
			if(d > PI) d -= PI2;

			// Return result
			return d;
		}
		
		#endregion
	}
}
