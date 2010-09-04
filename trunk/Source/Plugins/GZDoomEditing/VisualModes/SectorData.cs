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
		
		// This is to prevent recursion when sectors need each other to build
		private bool isbuilding;
		
		// Levels sorted by height
		private List<SectorLevel> levels;
		
		// Original floor and ceiling levels
		private SectorLevel floor;
		private SectorLevel ceiling;
		
		// Linedefs and Things of interest when building the levels
		// See RebuildSectorData() in BaseVisualMode.cs for the logic which selects interesting elements
		private List<Linedef> linedefs;
		private List<Thing> things;
		
		#endregion
		
		#region ================== Properties
		
		public Sector Sector { get { return sector; } }
		public bool Built { get { return built; } }
		public List<SectorLevel> Levels { get { return levels; } }
		public SectorLevel Floor { get { return floor; } }
		public SectorLevel Ceiling { get { return ceiling; } }
		
		#endregion
		
		#region ================== Constructor / Destructor
		
		// Constructor
		public SectorData(BaseVisualMode mode, Sector s)
		{
			int color = -1, flight = s.Brightness, clight = s.Brightness;
			bool fabs = true, cabs = true;
			
			// Initialize
			this.sector = s;
			this.built = false;
			this.levels = new List<SectorLevel>(2);
			this.linedefs = new List<Linedef>(1);
			this.things = new List<Thing>(1);
			
			// Create floor and ceiling planes
			floor = new SectorLevel(s, SectorLevelType.Floor);
			ceiling = new SectorLevel(s, SectorLevelType.Ceiling);
			floor.plane = new Plane(new Vector3D(0, 0, 1), sector.FloorHeight);
			ceiling.plane = new Plane(new Vector3D(0, 0, 1), sector.CeilHeight);
			
			// Determine colors
			try
			{
				// Fetch ZDoom fields
				color = sector.Fields.ContainsKey("lightcolor") ? (int)sector.Fields["lightcolor"].Value : -1;
				flight = sector.Fields.ContainsKey("lightfloor") ? (int)sector.Fields["lightfloor"].Value : 0;
				fabs = sector.Fields.ContainsKey("lightfloorabsolute") ? (bool)sector.Fields["lightfloorabsolute"].Value : false;
				clight = sector.Fields.ContainsKey("lightceiling") ? (int)sector.Fields["lightceiling"].Value : 0;
				cabs = sector.Fields.ContainsKey("lightceilingabsolute") ? (bool)sector.Fields["lightceilingabsolute"].Value : false;
			}
			catch(Exception) { }
			PixelColor lightcolor = PixelColor.FromInt(color);
			if(!fabs) flight = sector.Brightness + flight;
			if(!cabs) clight = sector.Brightness + clight;
			PixelColor floorbrightness = PixelColor.FromInt(mode.CalculateBrightness(flight));
			PixelColor ceilingbrightness = PixelColor.FromInt(mode.CalculateBrightness(clight));
			PixelColor floorcolor = PixelColor.Modulate(lightcolor, floorbrightness);
			PixelColor ceilingcolor = PixelColor.Modulate(lightcolor, ceilingbrightness);
			floor.color = floorcolor.WithAlpha(255).ToInt();
			ceiling.color = ceilingcolor.WithAlpha(255).ToInt();
			ceiling.brightnessbelow = sector.Brightness;
			ceiling.colorbelow = lightcolor;
			
			// Add ceiling and floor
			levels.Add(floor);
			levels.Add(ceiling);
		}
		
		#endregion
		
		#region ================== Public Methods
		
		// This adds a linedef that of interest to this sector, because it modifies the sector
		public void AddLinedef(Linedef l) { linedefs.Add(l); }
		
		// This adds a thing that of interest to this sector, because it modifies the sector
		public void AddThing(Thing t) { things.Add(t); }
		
		// This creates additional levels from things and linedefs
		public void BuildLevels(BaseVisualMode mode)
		{
			// Begin
			if(isbuilding) return;
			isbuilding = true;

			foreach(Linedef l in linedefs)
			{
				// Plane Align (see http://zdoom.org/wiki/Plane_Align)
				if(l.Action == 181)
				{
					// Find the vertex furthest from the line
					Vertex foundv = null;
					float founddist = -1.0f;
					foreach(Sidedef sd in sector.Sidedefs)
					{
						Vertex v = sd.IsFront ? sd.Line.Start : sd.Line.End;
						float d = l.DistanceToSq(v.Position, false);
						if(d > founddist)
						{
							foundv = v;
							founddist = d;
						}
					}
					
					// Align floor with back of line
					if((l.Args[0] == 1) && (l.Front.Sector == sector) && (l.Back != null))
					{
						Vector3D v1 = new Vector3D(l.End.Position.x, l.End.Position.y, l.Back.Sector.FloorHeight);
						Vector3D v2 = new Vector3D(l.Start.Position.x, l.Start.Position.y, l.Back.Sector.FloorHeight);
						Vector3D v3 = new Vector3D(foundv.Position.x, foundv.Position.y, sector.FloorHeight);
						if(l.SideOfLine(v3) < 0.0f)
							floor.plane = new Plane(v1, v2, v3, true);
						else
							floor.plane = new Plane(v2, v1, v3, true);
					}
					// Align floor with front of line
					else if((l.Args[0] == 2) && (l.Back.Sector == sector) && (l.Front != null))
					{
						Vector3D v1 = new Vector3D(l.End.Position.x, l.End.Position.y, l.Front.Sector.FloorHeight);
						Vector3D v2 = new Vector3D(l.Start.Position.x, l.Start.Position.y, l.Front.Sector.FloorHeight);
						Vector3D v3 = new Vector3D(foundv.Position.x, foundv.Position.y, sector.FloorHeight);
						if(l.SideOfLine(v3) < 0.0f)
							floor.plane = new Plane(v1, v2, v3, true);
						else
							floor.plane = new Plane(v2, v1, v3, true);
					}
					
					// Align ceiling with back of line
					if((l.Args[1] == 1) && (l.Front.Sector == sector) && (l.Back != null))
					{
						Vector3D v1 = new Vector3D(l.End.Position.x, l.End.Position.y, l.Back.Sector.CeilHeight);
						Vector3D v2 = new Vector3D(l.Start.Position.x, l.Start.Position.y, l.Back.Sector.CeilHeight);
						Vector3D v3 = new Vector3D(foundv.Position.x, foundv.Position.y, sector.CeilHeight);
						if(l.SideOfLine(v3) > 0.0f)
							ceiling.plane = new Plane(v1, v2, v3, false);
						else
							ceiling.plane = new Plane(v2, v1, v3, false);
					}
					// Align ceiling with front of line
					else if((l.Args[1] == 2) && (l.Back.Sector == sector) && (l.Front != null))
					{
						Vector3D v1 = new Vector3D(l.End.Position.x, l.End.Position.y, l.Front.Sector.CeilHeight);
						Vector3D v2 = new Vector3D(l.Start.Position.x, l.Start.Position.y, l.Front.Sector.CeilHeight);
						Vector3D v3 = new Vector3D(foundv.Position.x, foundv.Position.y, sector.CeilHeight);
						if(l.SideOfLine(v3) > 0.0f)
							ceiling.plane = new Plane(v1, v2, v3, false);
						else
							ceiling.plane = new Plane(v2, v1, v3, false);
					}
				}
			}
			
			foreach(Thing t in things)
			{
				// Copy floor slope
				if(t.Type == 9510)
				{
					// Find tagged sector
					Sector ts = null;
					foreach(Sector s in General.Map.Map.Sectors)
					{
						if(s.Tag == t.Args[0])
						{
							ts = s;
							break;
						}
					}
					
					if(ts != null)
					{
						SectorData tsd = mode.GetSectorData(ts);
						if(!tsd.Built) tsd.BuildLevels(mode);
						floor.plane = tsd.floor.plane;
					}
				}
			}
			
			// Sort the levels
			levels.Sort();
			
			// Done
			built = true;
			isbuilding = false;
		}
		
		#endregion
	}
}
