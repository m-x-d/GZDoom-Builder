#region === Copyright (c) 2010 Pascal van der Heiden ===

using CodeImp.DoomBuilder.Map;

#endregion

namespace CodeImp.DoomBuilder.BuilderModes
{
	internal class EffectBrightnessLevel : SectorEffect
	{
		// Linedef that is used to create this effect
		// The sector can be found by linedef.Front.Sector
		private Linedef linedef;
		
		// Level plane
		private SectorLevel level;
		
		// Constructor
		public EffectBrightnessLevel(SectorData data, Linedef sourcelinedef) : base(data)
		{
			linedef = sourcelinedef;

			// New effect added: This sector needs an update!
			if(data.Mode.VisualSectorExists(data.Sector))
			{
				BaseVisualSector vs = (BaseVisualSector)data.Mode.GetVisualSector(data.Sector);
				vs.UpdateSectorGeometry(false);
			}
		}
		
		// This makes sure we are updated with the source linedef information
		public override void Update()
		{
			SectorData sd = data.Mode.GetSectorData(linedef.Front.Sector);
			if(!sd.Updated) sd.Update();
			sd.AddUpdateSector(data.Sector, false);

			if(level == null)
			{
				level = new SectorLevel(sd.Ceiling);
				data.AddSectorLevel(level);
			}
			
			// Update level
			sd.Ceiling.CopyProperties(level);
			level.type = SectorLevelType.Light;
		}
	}
}
