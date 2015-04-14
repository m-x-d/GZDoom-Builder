using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Rendering;

namespace CodeImp.DoomBuilder.BuilderModes
{
	internal class EffectGlowingFlat : SectorEffect
	{
		private readonly Sector sector;

		// Level planes
		private SectorLevel ceillevel;
		private SectorLevel floorlevel;
		
		// Constructor
		public EffectGlowingFlat(SectorData data, Sector sourcesector) : base(data)
		{
			sector = sourcesector;

			// New effect added: This sector needs an update!
			if(data.Mode.VisualSectorExists(data.Sector))
			{
				BaseVisualSector vs = (BaseVisualSector)data.Mode.GetVisualSector(data.Sector);
				vs.UpdateSectorGeometry(false);
			}
		}

		public override void Update() 
		{
			// Create ceiling glow effect?
			if(General.Map.Data.GlowingFlats.ContainsKey(sector.LongCeilTexture))
			{
				// Create ceiling level?
				if(ceillevel == null)
				{
					ceillevel = new SectorLevel(data.Ceiling);
					ceillevel.type = SectorLevelType.Glow;
					ceillevel.disablelighting = true;
					data.AddSectorLevel(ceillevel);
				}

				// Update ceiling level
				data.CeilingGlow = General.Map.Data.GlowingFlats[sector.LongCeilTexture];
				ceillevel.brightnessbelow = -1; // We need this plane for clipping only,
				ceillevel.color = 0;            // so we need to reset all shading and coloring
				ceillevel.plane = data.Ceiling.plane;
				ceillevel.plane.Offset -= data.CeilingGlow.Height;
				data.CeilingGlowPlane = ceillevel.plane;
			}
			else
			{
				data.CeilingGlow = null;
			}

			// Create floor glow effect?
			if(General.Map.Data.GlowingFlats.ContainsKey(sector.LongFloorTexture))
			{
				// Create floor level?
				if(floorlevel == null)
				{
					floorlevel = new SectorLevel(data.Floor);
					floorlevel.type = SectorLevelType.Glow;
					floorlevel.disablelighting = true;
					data.AddSectorLevel(floorlevel);
				}

				// Update floor level
				data.FloorGlow = General.Map.Data.GlowingFlats[sector.LongFloorTexture];
				floorlevel.plane = data.Floor.plane.GetInverted();
				floorlevel.plane.Offset += data.FloorGlow.Height;

				if(floorlevel.plane.Offset < data.Ceiling.plane.Offset)
				{
					floorlevel.brightnessbelow = -1; // We need this plane for clipping only,
					floorlevel.color = 0;            // so we need to reset all shading and coloring
					floorlevel.colorbelow = new PixelColor(0, 0, 0, 0);
				}
				else
				{
					// If glow plane is above real ceiling, apply ceiling colouring
					floorlevel.brightnessbelow = data.Ceiling.brightnessbelow;
					floorlevel.color = data.Ceiling.color;
					floorlevel.colorbelow = data.Ceiling.colorbelow;
				}

				data.FloorGlowPlane = floorlevel.plane;
			}
			else
			{
				data.FloorGlow = null;
			}
		}
	}
}
