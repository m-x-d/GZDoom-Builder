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
	internal class Effect3DFloor : SectorEffect
	{
		// Linedef that is used to create this effect
		// The sector can be found by linedef.Front.Sector
		private Linedef linedef;
		
		// Floor and ceiling planes
		private SectorLevel floor;
		private SectorLevel ceiling;

		// Alpha transparency
		private int alpha;

		// Properties
		public int Alpha { get { return alpha; } }
		public SectorLevel Floor { get { return floor; } }
		public SectorLevel Ceiling { get { return ceiling; } }
		public Linedef Linedef { get { return linedef; } }

		// Constructor
		public Effect3DFloor(SectorData data, Linedef sourcelinedef) : base(data)
		{
			linedef = sourcelinedef;
		}

		// This makes sure we are updated with the source linedef information
		public override void Update()
		{
			SectorData sd = data.Mode.GetSectorData(linedef.Front.Sector);
			if(!sd.Updated) sd.Update();
			sd.AddUpdateSector(data.Sector, true);

			if(floor == null)
			{
				floor = new SectorLevel(sd.Floor);
				data.AddSectorLevel(floor);
			}

			if(ceiling == null)
			{
				ceiling = new SectorLevel(sd.Ceiling);
				data.AddSectorLevel(ceiling);
			}

			// For non-vavoom types, we must switch the level types
			int argtype = (linedef.Args[1] & 0x03);
			if(argtype != 0)
			{
				alpha = linedef.Args[3];
				sd.Ceiling.CopyProperties(floor);
				sd.Floor.CopyProperties(ceiling);
				floor.type = SectorLevelType.Floor;
				floor.plane = floor.plane.GetInverted();
				ceiling.type = SectorLevelType.Ceiling;
				ceiling.plane = ceiling.plane.GetInverted();
			}
			else
			{
				sd.Ceiling.CopyProperties(ceiling);
				sd.Floor.CopyProperties(floor);
				alpha = 255;
			}
			
			// A 3D floor's color is always that of the sector it is placed in
			floor.color = 0;

			// Apply alpha
			floor.alpha = alpha;
			ceiling.alpha = alpha;
			
			// Do not adjust light? (works only for non-vavoom types)
			if(((linedef.Args[2] & 1) != 0) && (argtype != 0))
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
