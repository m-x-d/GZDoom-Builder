#region ================== Namespaces

using System;
using CodeImp.DoomBuilder.Editing;
using CodeImp.DoomBuilder.Geometry;

#endregion

namespace CodeImp.DoomBuilder.BuilderModes
{
	[EditMode(DisplayName = "Draw Ellipse Mode",
			  SwitchAction = "drawellipsemode",
			  ButtonImage = "DrawEllipseMode.png", //mxd	
			  ButtonOrder = int.MinValue + 4, //mxd
			  ButtonGroup = "000_drawing", //mxd
			  AllowCopyPaste = false,
			  Volatile = true,
			  Optional = false)]
	
	public class DrawEllipseMode : DrawRectangleMode
	{
		#region ================== Variables

		//interface
		private DrawEllipseOptionsPanel panel;

		#endregion

		#region ================== Constructor

		public DrawEllipseMode() {
			undoName = "Ellipse draw";
			shapeName = "ellipse";
		}

		#endregion

		#region ================== Settings panel

		override protected void setupInterface() {
			maxSubdivisions = 512;
			minSubdivisions = 6;

			//Add options docker
			panel = new DrawEllipseOptionsPanel();
			panel.MaxSubdivisions = maxSubdivisions;
			panel.MinSubdivisions = minSubdivisions;
			panel.MinSpikiness = (int)General.Map.FormatInterface.MinCoordinate;
			panel.MaxSpikiness = (int)General.Map.FormatInterface.MaxCoordinate;
			panel.OnValueChanged += OptionsPanelOnValueChanged;
		}

		override protected void addInterface() {
			panel.Register();
			bevelWidth = panel.Spikiness;
			subdivisions = panel.Subdivisions;
		}

		override protected void removeInterface() {
			panel.Unregister();
		}

		#endregion

		#region ================== Methods

		override protected Vector2D[] getShape(Vector2D pStart, Vector2D pEnd) {
			//no shape
			if (pEnd.x == pStart.x && pEnd.y == pStart.y) return new Vector2D[0];

			//line
			if(pEnd.x == pStart.x || pEnd.y == pStart.y) return new[] { pStart, pEnd };

			//got shape
			if (bevelWidth < 0) {
				currentBevelWidth = -Math.Min(Math.Abs(bevelWidth), Math.Min(width, height) / 2) + 1;
			} else {
				currentBevelWidth = bevelWidth;
			}

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
			return "BVL: " + bevelWidth + "; VERTS: " + subdivisions;
		}

		#endregion

		#region ================== Events

		private void OptionsPanelOnValueChanged(object sender, EventArgs eventArgs) {
			bevelWidth = panel.Spikiness;
			subdivisions = Math.Min(maxSubdivisions, panel.Subdivisions);
			Update();
		}

		public override void OnHelp() {
			General.ShowHelp("/gzdb/features/classic_modes/mode_drawellipse.html");
		}

		#endregion

		#region ================== Actions

		override protected void increaseSubdivLevel() {
			if(maxSubdivisions - subdivisions > 1) {
				subdivisions += 2;
				panel.Subdivisions = subdivisions;
				Update();
			}
		}

		override protected void decreaseSubdivLevel() {
			if (subdivisions - minSubdivisions > 1) {
				subdivisions -= 2;
				panel.Subdivisions = subdivisions;
				Update();
			}
		}

		protected override void increaseBevel() {
			if(points.Count < 2 || currentBevelWidth == bevelWidth || bevelWidth < 0) {
				bevelWidth = Math.Min(bevelWidth + General.Map.Grid.GridSize, panel.MaxSpikiness);
				panel.Spikiness = bevelWidth;
				Update();
			}
		}

		protected override void decreaseBevel() {
			if(bevelWidth > 0 || currentBevelWidth <= bevelWidth + 1) {
				bevelWidth = Math.Max(bevelWidth - General.Map.Grid.GridSize, panel.MinSpikiness);
				panel.Spikiness = bevelWidth;
				Update();
			}
		}

		#endregion
	}
}
