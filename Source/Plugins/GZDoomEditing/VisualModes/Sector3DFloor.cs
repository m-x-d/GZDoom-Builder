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
	internal class Sector3DFloor
	{
		// Floor and ceiling planes
		public SectorLevel floor;
		public SectorLevel ceiling;
		
		// Linedef that is used to create this 3D floor
		public Linedef linedef;
		
		// Constructor
		public Sector3DFloor(SectorData controlsector, Linedef sourcelinedef)
		{
			linedef = sourcelinedef;
			int argtype = (sourcelinedef.Args[1] & 0x03);
			
			// For non-vavoom types, we must switch the level types
			if(argtype != 0)
			{
				floor = new SectorLevel(controlsector.Ceiling);
				ceiling = new SectorLevel(controlsector.Floor);
				floor.type = SectorLevelType.Floor;
				floor.plane = floor.plane.GetInverted();
				ceiling.type = SectorLevelType.Ceiling;
				ceiling.plane = ceiling.plane.GetInverted();
				floor.alpha = sourcelinedef.Args[3];
				ceiling.alpha = sourcelinedef.Args[3];
			}
			else
			{
				floor = new SectorLevel(controlsector.Floor);
				ceiling = new SectorLevel(controlsector.Ceiling);
			}
			
			// A 3D floor's color is always that of the sector it is placed in
			floor.color = 0;
			
			// Do not adjust light? (works only for non-vavoom types)
			if(((sourcelinedef.Args[2] & 1) != 0) && (argtype != 0))
			{
				floor.brightnessbelow = -1;
				floor.colorbelow = PixelColor.FromInt(0);
				ceiling.color = 0;
				ceiling.brightnessbelow = -1;
				ceiling.colorbelow = PixelColor.FromInt(0);
			}
		}
	}
}
