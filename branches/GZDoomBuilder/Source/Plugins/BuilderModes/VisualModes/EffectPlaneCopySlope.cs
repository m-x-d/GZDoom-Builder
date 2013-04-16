using CodeImp.DoomBuilder.Map;

namespace CodeImp.DoomBuilder.BuilderModes
{
	internal class EffectPlaneCopySlope : SectorEffect
	{
		// Linedef that is used to create this effect
		private Linedef linedef;
		private bool isFront;

		public EffectPlaneCopySlope(SectorData data, Linedef sourcelinedef, bool front)	: base(data) {
			linedef = sourcelinedef;
			isFront = front;

			// New effect added: This sector needs an update!
			if(data.Mode.VisualSectorExists(data.Sector)) {
				BaseVisualSector vs = (BaseVisualSector)data.Mode.GetVisualSector(data.Sector);
				vs.UpdateSectorGeometry(true);
			}
		}
		
		// This makes sure we are updated with the source linedef information
		public override void Update() {
			Sector sourcesector = null;
			SectorData sourcesectordata = null;

			//check flags
			bool floorCopyToBack = false;
			bool floorCopyToFront = false;
			bool ceilingCopyToBack = false;
			bool ceilingCopyToFront = false;

			if(linedef.Args[4] > 0 && linedef.Args[4] != 3 && linedef.Args[4] != 12) {
				floorCopyToBack = linedef.Args[0] > 0 && (linedef.Args[4] & 1) == 1;
				floorCopyToFront = linedef.Args[2] > 0 && (linedef.Args[4] & 2) == 2;
				ceilingCopyToBack = linedef.Args[1] > 0 && (linedef.Args[4] & 4) == 4;
				ceilingCopyToFront = linedef.Args[3] > 0 && (linedef.Args[4] & 8) == 8;
			}

			//check which arguments we must use
			int floorArg = -1;
			int ceilingArg = -1;

			if(isFront) {
				floorArg = floorCopyToFront ? 2 : 0;
				ceilingArg = ceilingCopyToFront ? 3 : 1;
			} else {
				floorArg = floorCopyToBack ? 0 : 2;
				ceilingArg = ceilingCopyToBack ? 1 : 3;
			}

			//find sector to align floor to
			if(linedef.Args[floorArg] > 0) {
				foreach(Sector s in General.Map.Map.Sectors) {
					if(s.Tag == linedef.Args[floorArg]) {
						sourcesector = s;
						break;
					}
				}

				if(sourcesector != null) {
					sourcesectordata = data.Mode.GetSectorData(sourcesector);
					if(!sourcesectordata.Updated) sourcesectordata.Update();

					data.Floor.plane = sourcesectordata.Floor.plane;
					sourcesectordata.AddUpdateSector(data.Sector, true);
				}
			}

			if(linedef.Args[ceilingArg] > 0) {
				//find sector to align ceiling to
				if(linedef.Args[ceilingArg] != linedef.Args[floorArg]) {
					sourcesector = null;

					foreach(Sector s in General.Map.Map.Sectors) {
						if(s.Tag == linedef.Args[ceilingArg]) {
							sourcesector = s;
							break;
						}
					}

					if(sourcesector != null) {
						sourcesectordata = data.Mode.GetSectorData(sourcesector);
						if(!sourcesectordata.Updated) sourcesectordata.Update();

						data.Floor.plane = sourcesectordata.Floor.plane;
						sourcesectordata.AddUpdateSector(data.Sector, true);
					}

				} else if(sourcesector != null) { //ceiling uses the same sector as floor 
					data.Ceiling.plane = sourcesectordata.Ceiling.plane;
				}
			}
		}
	}
}
