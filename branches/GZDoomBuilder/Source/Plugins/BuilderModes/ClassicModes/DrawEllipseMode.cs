using System;
using CodeImp.DoomBuilder.Editing;
using CodeImp.DoomBuilder.Geometry;

namespace CodeImp.DoomBuilder.BuilderModes.ClassicModes {
	[EditMode(DisplayName = "Draw Ellipse Mode",
			  SwitchAction = "drawellipsemode",
			  AllowCopyPaste = false,
			  Volatile = true,
			  Optional = false)]
	
	public class DrawEllipseMode : DrawRectangleMode {
		private static int storedSubdivisions;

		public DrawEllipseMode() : base() {
			maxSubdivisions = 32;
			minSubdivisions = 6;
			undoName = "Ellipse draw";
			shapeName = "ellipse";

			if(storedSubdivisions == 0)
				storedSubdivisions = minSubdivisions + 2;
			subdivisions = storedSubdivisions;
		}

		public override void OnDisengage() {
			base.OnDisengage();
			storedSubdivisions = subdivisions;
		}

		override protected Vector2D[] getShape(Vector2D pStart, Vector2D pEnd) {
			//no shape
			if (pEnd.x == pStart.x && pEnd.y == pStart.y)
				return new Vector2D[0];

			//line
			if(pEnd.x == pStart.x || pEnd.y == pStart.y)
				return new Vector2D[] { pStart, pEnd };

			//got shape
			int bevelSign = (bevelWidth > 0 ? 1 : -1);
			currentBevelWidth = Math.Min(Math.Abs(bevelWidth), Math.Min(width, height) / 2) * bevelSign;

			Vector2D[] shape = new Vector2D[subdivisions + 1];

			bool doBevel = false;
			int hw = width / 2;
			int hh = height / 2;

			Vector2D center = new Vector2D(pStart.x + hw, pStart.y + hh);
			float curAngle = 0;
			float angleStep = -Angle2D.PI / subdivisions * 2;
			int px, py;

			for (int i = 0; i < subdivisions; i++) {
				if (doBevel) {
					px = (int)(center.x - (float)Math.Sin(curAngle) * (hw + currentBevelWidth));
					py = (int)(center.y - (float)Math.Cos(curAngle) * (hh + currentBevelWidth));
				} else {
					px = (int)(center.x - (float)Math.Sin(curAngle) * hw);
					py = (int)(center.y - (float)Math.Cos(curAngle) * hh);
				}
				doBevel = !doBevel;
				shape[i] = new Vector2D(px, py);
				curAngle += angleStep;
			}
			//add final point
			shape[subdivisions] = shape[0];
			return shape;
		}

		protected override string getHintText() {
			return "BVL: "+bevelWidth+"; VERTS: "+subdivisions;
		}

//ACTIONS
		override protected void increaseSubdivLevel() {
			if (subdivisions < maxSubdivisions) {
				subdivisions += 2;
				Update();
			}
		}

		override protected void decreaseSubdivLevel() {
			if (subdivisions > minSubdivisions) {
				subdivisions -= 2;
				Update();
			}
		}
	}
}
