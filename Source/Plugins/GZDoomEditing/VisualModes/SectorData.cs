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
		
		// First level is the sector's absolute ceiling
		// Last level is the sector's absolute floor
		private List<SectorLevel> levels;

		#endregion

		#region ================== Properties

		public Sector Sector { get { return sector; } }
		public List<SectorLevel> Levels { get { return levels; } }

		#endregion

		#region ================== Constructor / Destructor

		// Constructor
		public SectorData(BaseVisualMode mode, Sector s)
		{
			int color = -1, light = s.Brightness;
			bool absolute = true;
			
			// Initialize
			this.sector = s;
			this.levels = new List<SectorLevel>(2);

			// Create floor
			SectorLevel fl = new SectorLevel();
			fl.type = SectorLevelType.Floor;
			fl.plane = new Plane(new Vector3D(0, 0, 1), s.FloorHeight);
			fl.color = -1;
			this.levels.Add(fl);

			// Create ceiling
			SectorLevel cl = new SectorLevel();
			cl.type = SectorLevelType.Ceiling;
			cl.plane = new Plane(new Vector3D(0, 0, -1), s.CeilHeight);
			try
			{
				// Fetch ZDoom fields
				color = s.Fields.ContainsKey("lightcolor") ? (int)s.Fields["lightcolor"].Value : -1;
				light = s.Fields.ContainsKey("lightfloor") ? (int)s.Fields["lightfloor"].Value : 0;
				absolute = s.Fields.ContainsKey("lightfloorabsolute") ? (bool)s.Fields["lightfloorabsolute"].Value : false;
			}
			catch(Exception) { }
			if(!absolute) light = s.Brightness + light;
			PixelColor lightcolor = PixelColor.FromInt(color);
			PixelColor brightness = PixelColor.FromInt(mode.CalculateBrightness(light));
			PixelColor finalcolor = PixelColor.Modulate(lightcolor, brightness);
			cl.color = finalcolor.WithAlpha(255).ToInt();
			this.levels.Add(cl);
		}

		#endregion
		
		#region ================== Public Methods

		#endregion
	}
}
