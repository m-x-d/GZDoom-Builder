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
		public Vector3D center;
		
		// Type of level
		public SectorLevelType type;
		
		// Plane in the sector
		public Plane plane;
		
		// Color of the plane (includes brightness)
		public int color;
		
		// Color and brightness below the plane
		public int brightnessbelow;
		public PixelColor colorbelow;
		
		// Constructor
		public SectorLevel(Sector s, SectorLevelType type)
		{
			this.type = type;
			this.center = new Vector3D(s.BBox.Left + s.BBox.Width / 2, s.BBox.Top + s.BBox.Height / 2, (s.FloorHeight + s.CeilHeight) / 2);
		}
		
		// Comparer
		public int CompareTo(SectorLevel other)
		{
			float delta = this.plane.ClosestOnPlane(center).z - other.plane.ClosestOnPlane(center).z;
			
			if(delta > 0.0f)
				return 1;
			else if(delta < 0.0f)
				return -1;
			else
				return 0;
		}
	}
}
