using System;
using System.Collections.Generic;
using System.Text;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Geometry;

namespace CodeImp.DoomBuilder.GZDoomEditing {
    internal class EffectUDMFVertexOffset : SectorEffect {

        private Vertex[] vertices;

        public EffectUDMFVertexOffset(SectorData data, Vertex[] vertices)
            : base(data) {

            this.vertices = vertices;

            // New effect added: This sector needs an update!
            if (data.Mode.VisualSectorExists(data.Sector)) {
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
                floorVerts[index] = new Vector3D(v.Position.x, v.Position.y, data.Floor.plane.GetZ(v.Position));
                ceilingVerts[index] = new Vector3D(v.Position.x, v.Position.y, data.Ceiling.plane.GetZ(v.Position));

                if (vertices[index] == null){
                    index++;
                    continue;
                }

                //check ceiling
                if (vertices[index].Fields.ContainsKey("zceiling")) {
                    //yes, some things work in strange and mysterious ways in zdoom...
                    ceilingVerts[index].z = (float)vertices[index].Fields["zceiling"].Value;
                    ceilingChanged = true;
                }

                //and floor ceiling
                if (vertices[index].Fields.ContainsKey("zfloor")) {
                    //yes, some things work in strange and mysterious ways in zdoom...
                    floorVerts[index].z = (float)vertices[index].Fields["zfloor"].Value;
                    floorChanged = true;
                }

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
