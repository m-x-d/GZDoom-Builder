using System;
using System.Collections.Generic;
using System.Text;
using CodeImp.DoomBuilder.Geometry;

namespace CodeImp.DoomBuilder.GZBuilder.Geometry {
    public class Line3D {
        
        // Coordinates
        public Vector3D v1;
        public Vector3D v2;

        // Constructor
		public Line3D(Vector3D v1, Vector3D v2)	{
			this.v1 = v1;
			this.v2 = v2;
		}

        public Vector3D GetDelta() { return v2 - v1; }

        // This calculates the angle
        public float GetAngle() {
            // Calculate and return the angle
            Vector2D d = GetDelta();
            return -(float)Math.Atan2(-d.y, d.x) + (float)Math.PI * 0.5f;
        }
    }
}
