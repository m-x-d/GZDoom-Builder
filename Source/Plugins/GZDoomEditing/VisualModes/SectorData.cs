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
	internal class SectorData
	{
		#region ================== Variables

		// Sector for which this data is
		private Sector sector;

		// Levels have been built?
		private bool built;
		
		// First level is the sector's absolute ceiling
		// Last level is the sector's absolute floor
		private List<SectorLevel> levels;

		// Linedefs and Things of interest when building the levels
		// See RebuildSectorData() in BaseVisualMode.cs for the logic which selects interesting elements
		private List<Linedef> linedefs;
		private List<Thing> things;

		#endregion

		#region ================== Properties

		public Sector Sector { get { return sector; } }
		public bool Built { get { return built; } }
		public List<SectorLevel> Levels { get { return levels; } }

		#endregion

		#region ================== Constructor / Destructor

		// Constructor
		public SectorData(Sector s)
		{
			// Initialize
			this.sector = s;
			this.built = false;
			this.levels = new List<SectorLevel>(2);
			this.linedefs = new List<Linedef>(1);
			this.things = new List<Thing>(1);
		}

		#endregion
		
		#region ================== Public Methods

		// This adds a linedef that of interest to this sector, because it modifies the sector
		public void AddLinedef(Linedef l) { linedefs.Add(l); }

		// This adds a thing that of interest to this sector, because it modifies the sector
		public void AddThing(Thing t) { things.Add(t); }

		// This creates the levels with the things and linedefs of interest
		public void BuildLevels(BaseVisualMode mode)
		{
			int color = -1, light = sector.Brightness;
			bool absolute = true;
			
			// Create floor
			SectorLevel fl = new SectorLevel();
			fl.type = SectorLevelType.Floor;
			fl.plane = new Plane(new Vector3D(0, 0, 1), sector.FloorHeight);
			fl.color = -1;
			levels.Add(fl);
			
			// Create ceiling
			SectorLevel cl = new SectorLevel();
			cl.type = SectorLevelType.Ceiling;
			cl.plane = new Plane(new Vector3D(0, 0, -1), sector.CeilHeight);
			try
			{
				// Fetch ZDoom fields
				color = sector.Fields.ContainsKey("lightcolor") ? (int)sector.Fields["lightcolor"].Value : -1;
				light = sector.Fields.ContainsKey("lightfloor") ? (int)sector.Fields["lightfloor"].Value : 0;
				absolute = sector.Fields.ContainsKey("lightfloorabsolute") ? (bool)sector.Fields["lightfloorabsolute"].Value : false;
			}
			catch(Exception) { }
			if(!absolute) light = sector.Brightness + light;
			PixelColor lightcolor = PixelColor.FromInt(color);
			PixelColor brightness = PixelColor.FromInt(mode.CalculateBrightness(light));
			PixelColor finalcolor = PixelColor.Modulate(lightcolor, brightness);
			cl.color = finalcolor.WithAlpha(255).ToInt();
			levels.Add(cl);

			// Done
			built = true;
		}

		#endregion
	}
}
