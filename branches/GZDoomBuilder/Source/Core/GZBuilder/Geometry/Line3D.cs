using System;
using System.Collections.Generic;
using System.Text;
using CodeImp.DoomBuilder.Geometry;

namespace CodeImp.DoomBuilder.GZBuilder.Geometry {
	public enum Line3DType
	{
		DEFAULT,
		ACTIVATOR,
	}
	
	public class Line3D {
        // Coordinates
        public Vector3D v1;
        public Vector3D v2;
		public Line3DType LineType { get { return lineType; } }
		private Line3DType lineType;

        // Constructors
		public Line3D(Vector3D v1, Vector3D v2) {
			this.v1 = v1;
			this.v2 = v2;
			this.lineType = Line3DType.DEFAULT;
		}

		public Line3D(Vector3D v1, Vector3D v2, Line3DType lineType) {
			this.v1 = v1;
			this.v2 = v2;
			this.lineType = lineType;
		}

        public Vector3D GetDelta() { return v2 - v1; }

        // This calculates the angle
        public float GetAngle() {
            // Calculate and return the angle
            Vector2D d = GetDelta();
			return -(float)Math.Atan2(-d.y, d.x) + Angle2D.PIHALF;//mxd //  (float)Math.PI * 0.5f;
        }
    }
}
