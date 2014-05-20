using CodeImp.DoomBuilder.Geometry;

namespace CodeImp.DoomBuilder.BuilderModes
{
	internal class EffectSectorSlope : SectorEffect
	{
		private readonly bool ceilingslope;
		
		public EffectSectorSlope(SectorData data, bool ceilingslope) : base(data) 
		{
			this.ceilingslope = ceilingslope;

			// New effect added: This sector needs an update!
			if(data.Mode.VisualSectorExists(data.Sector)) 
			{
				BaseVisualSector vs = (BaseVisualSector)data.Mode.GetVisualSector(data.Sector);
				vs.UpdateSectorGeometry(true);
			}
		}

		// This makes sure we are updated
		public override void Update() 
		{
			if (ceilingslope) {
				float a = data.Sector.Fields.GetValue("ceilingplane_a", 0f);
				float b = data.Sector.Fields.GetValue("ceilingplane_b", 0f);
				float c = data.Sector.Fields.GetValue("ceilingplane_c", 0f);
				float d = data.Sector.Fields.GetValue("ceilingplane_d", 0f);

				Vector3D normal = new Vector3D(a, b, c).GetNormal();
				if (normal.x != 0 || normal.y != 0 || normal.z != 0) 
				{
					if(normal.z > 0) normal = -normal; //flip the plane if it's facing the wrong direction
					data.Ceiling.plane = new Plane(normal, d);
				}
			} 
			else 
			{
				float a = data.Sector.Fields.GetValue("floorplane_a", 0f);
				float b = data.Sector.Fields.GetValue("floorplane_b", 0f);
				float c = data.Sector.Fields.GetValue("floorplane_c", 0f);
				float d = data.Sector.Fields.GetValue("floorplane_d", 0f);
				
				Vector3D normal = new Vector3D(a, b, c).GetNormal();
				if (normal.x != 0 || normal.y != 0 || normal.z != 0) 
				{
					if(normal.z < 0) normal = -normal; //flip the plane if it's facing the wrong direction
					data.Floor.plane = new Plane(normal, d);
				}
			}
		}
	}
}
