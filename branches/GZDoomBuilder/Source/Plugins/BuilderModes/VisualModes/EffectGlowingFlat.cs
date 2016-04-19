using CodeImp.DoomBuilder.Rendering;

namespace CodeImp.DoomBuilder.BuilderModes
{
	internal class EffectGlowingFlat
	{
		private readonly SectorData data;

		// Level planes
		private SectorLevel ceillevel;
		private SectorLevel floorlevel;
		
		// Constructor
		public EffectGlowingFlat(SectorData sourcedata)
		{
			data = sourcedata;
		}

		public void Update() 
		{
			// Create ceiling glow effect?
			if(General.Map.Data.GlowingFlats.ContainsKey(data.Sector.LongCeilTexture))
			{
				// Create ceiling level?
				if(ceillevel == null)
				{
					ceillevel = new SectorLevel(data.Ceiling) { type = SectorLevelType.Glow, disablelighting = true };
					data.AddSectorLevel(ceillevel);
				}

				// Update ceiling level
				data.CeilingGlow = General.Map.Data.GlowingFlats[data.Sector.LongCeilTexture];
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
			if(General.Map.Data.GlowingFlats.ContainsKey(data.Sector.LongFloorTexture))
			{
				// Create floor level?
				if(floorlevel == null)
				{
					floorlevel = new SectorLevel(data.Floor) { type = SectorLevelType.Glow, disablelighting = true };
					data.AddSectorLevel(floorlevel);
				}

				// Update floor level
				data.FloorGlow = General.Map.Data.GlowingFlats[data.Sector.LongFloorTexture];
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
