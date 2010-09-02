#region === Copyright (c) 2010 Pascal van der Heiden ===

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using CodeImp.DoomBuilder.Geometry;

#endregion

namespace CodeImp.DoomBuilder.GZDoomEditing
{
	internal struct SectorLevel
	{
		// Type of level
		public SectorLevelType type;
		
		// Plane in the sector
		public Plane plane;

		// Color below the plane (includes brightness)
		public int color;
	}
}
