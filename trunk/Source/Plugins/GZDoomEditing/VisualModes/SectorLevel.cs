#region === Copyright (c) 2010 Pascal van der Heiden ===

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using CodeImp.DoomBuilder.Geometry;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Rendering;

#endregion

namespace CodeImp.DoomBuilder.GZDoomEditing
{
	internal class SectorLevel : IComparable<SectorLevel>
	{
		// Center of sector to use for plane comparison
		public Vector2D center;

		// Type of level
		public SectorLevelType type;

		// Sector where this level originates from
		public Sector sector;
		
		// Plane in the sector
		public Plane plane;
		
		// Alpha for translucency (255=opaque)
		public int alpha;
		
		// Color of the plane (includes brightness)
		// When this is 0, it takes the color from the sector above
		public int color;
		
		// Color and brightness below the plane
		// When this is 0, it takes the color from the sector above
		public int brightnessbelow;
		public PixelColor colorbelow;
		
		// Constructor
		public SectorLevel(Sector s, SectorLevelType type)
		{
			this.type = type;
			this.sector = s;
			this.alpha = 255;
			this.center = new Vector2D(s.BBox.Left + s.BBox.Width / 2, s.BBox.Top + s.BBox.Height / 2);
		}
		
		// Copy constructor
		public SectorLevel(SectorLevel source)
		{
			source.CopyProperties(this);
		}

		// Copy properties
		public void CopyProperties(SectorLevel target)
		{
			target.sector = this.sector;
			target.center = this.center;
			target.type = this.type;
			target.plane = this.plane;
			target.alpha = this.alpha;
			target.color = this.color;
			target.brightnessbelow = this.brightnessbelow;
			target.colorbelow = this.colorbelow;
		}
		
		// Comparer
		public int CompareTo(SectorLevel other)
		{
			float delta = this.plane.GetZ(center) - other.plane.GetZ(center);
			
			if(delta > 0.0f)
				return 1;
			else if(delta < 0.0f)
				return -1;
			else
				return 0;
		}
	}
}
