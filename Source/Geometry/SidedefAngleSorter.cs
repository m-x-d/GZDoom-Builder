
#region ================== Copyright (c) 2007 Pascal vd Heiden

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

#endregion

#region ================== Namespaces

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using CodeImp.DoomBuilder.Map;

#endregion

namespace CodeImp.DoomBuilder.Geometry
{
	internal class SidedefAngleSorter : IComparer<Sidedef>
	{
		// Variables
		private float baseangle;
		private Vertex basevertex;
		
		// Constructor
		public SidedefAngleSorter(Sidedef baseside, Vertex basev)
		{
			// Initialize
			baseangle = baseside.Angle;
			if(baseside.Line.Start == basev) basevertex = baseside.Line.End;
				else basevertex = baseside.Line.Start;
			
			// We have no destructor
			GC.SuppressFinalize(this);
		}

		// Comparer
		public int Compare(Sidedef x, Sidedef y)
		{
			float ax, ay;
			float sx, sy;
			
			// Calculate x angle
			ax = Angle2D.Difference(baseangle, x.Angle);
			sx = x.Line.SideOfLine(basevertex.Position);
			if((sx < 0) && x.IsFront) ax += Angle2D.PI;
			if((sx > 0) && !x.IsFront) ax += Angle2D.PI;
			ax = Angle2D.Normalized(ax);
			
			// Calculate y angle
			ay = Angle2D.Difference(baseangle, y.Angle);
			sy = y.Line.SideOfLine(basevertex.Position);
			if((sy < 0) && x.IsFront) ay += Angle2D.PI;
			if((sy > 0) && !x.IsFront) ay += Angle2D.PI;
			ay = Angle2D.Normalized(ay);
			
			// Compare results
			if(ax < ay) return 1;
			else if(ax > ay) return -1;
			else return 0;
		}
	}
}
