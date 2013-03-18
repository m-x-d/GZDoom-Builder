using System;
using System.Collections.Generic;
using System.Text;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Geometry;

namespace CodeImp.DoomBuilder.BuilderModes
{
    internal class EffectUDMFVertexOffset : SectorEffect {
        
		public EffectUDMFVertexOffset(SectorData data) : base(data) {
			// New effect added: This sector needs an update!
			if(data.Mode.VisualSectorExists(data.Sector)) {
				BaseVisualSector vs = (BaseVisualSector)data.Mode.GetVisualSector(data.Sector);
				vs.UpdateSectorGeometry(true);
			}
        }

        public override void Update() {
            // Create vertices in clockwise order
            Vector3D[] floorVerts = new Vector3D[3];
            Vector3D[] ceilingVerts = new Vector3D[3];
            bool floorChanged = false;
            bool ceilingChanged = false;
            int index = 0;

            //check vertices
            foreach(Sidedef sd in data.Sector.Sidedefs)	{
				Vertex v = sd.IsFront ? sd.Line.End : sd.Line.Start;
                
                //create "normal" vertices
                floorVerts[index] = new Vector3D(v.Position);
                ceilingVerts[index] = new Vector3D(v.Position);

                //check ceiling
				if(v.Fields.ContainsKey("zceiling")) {
					//vertex offset is absolute
					ceilingVerts[index].z = (float)v.Fields["zceiling"].Value;
					ceilingChanged = true;
				} else {
					ceilingVerts[index].z = data.Ceiling.plane.GetZ(v.Position);
				}

                //and floor
				if(v.Fields.ContainsKey("zfloor")) {
					//vertex offset is absolute
					floorVerts[index].z = (float)v.Fields["zfloor"].Value;
					floorChanged = true;
				} else {
					floorVerts[index].z = data.Floor.plane.GetZ(v.Position);
				}

				VertexData vd = data.Mode.GetVertexData(v);
				vd.AddUpdateSector(data.Sector, true);
				data.Mode.UpdateVertexHandle(v);

                index++;
            }

            //apply changes
            if(ceilingChanged)
                data.Ceiling.plane = new Plane(ceilingVerts[0], ceilingVerts[2], ceilingVerts[1], false);

            if(floorChanged)
                data.Floor.plane = new Plane(floorVerts[0], floorVerts[1], floorVerts[2], true);
        }
    }
}
