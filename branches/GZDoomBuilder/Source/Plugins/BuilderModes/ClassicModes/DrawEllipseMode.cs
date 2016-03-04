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

		// Interface
		private DrawEllipseOptionsPanel panel;

		#endregion

		#region ================== Constructor

		public DrawEllipseMode() 
		{
			undoname = "Ellipse draw";
			shapename = "ellipse";
			usefourcardinaldirections = true;
		}

		#endregion

		#region ================== Settings panel

		override protected void SetupInterface() 
		{
			maxsubdivisions = 512;
			minsubdivisions = 6;

			// Load stored settings
			subdivisions = General.Clamp(General.Settings.ReadPluginSetting("drawellipsemode.subdivisions", 8), minsubdivisions, maxsubdivisions);
			bevelwidth = General.Settings.ReadPluginSetting("drawellipsemode.bevelwidth", 0);
			currentbevelwidth = bevelwidth;

			//Add options docker
			panel = new DrawEllipseOptionsPanel();
			panel.MaxSubdivisions = maxsubdivisions;
			panel.MinSubdivisions = minsubdivisions;
			panel.MinSpikiness = (int)General.Map.FormatInterface.MinCoordinate;
			panel.MaxSpikiness = (int)General.Map.FormatInterface.MaxCoordinate;
			panel.Spikiness = bevelwidth;
			panel.Subdivisions = subdivisions;
			panel.OnValueChanged += OptionsPanelOnValueChanged;
			panel.OnContinuousDrawingChanged += OnContinuousDrawingChanged;

			// Needs to be set after adding the OnContinuousDrawingChanged event...
			panel.ContinuousDrawing = General.Settings.ReadPluginSetting("drawellipsemode.continuousdrawing", false);
		}

		override protected void AddInterface() 
		{
			panel.Register();
		}

		override protected void RemoveInterface() 
		{
			// Store settings
			General.Settings.WritePluginSetting("drawellipsemode.subdivisions", subdivisions);
			General.Settings.WritePluginSetting("drawellipsemode.bevelwidth", bevelwidth);
			General.Settings.WritePluginSetting("drawellipsemode.continuousdrawing", panel.ContinuousDrawing);

			// Remove the buttons
			panel.Unregister();
		}

		#endregion

		#region ================== Methods

		override protected Vector2D[] GetShape(Vector2D pStart, Vector2D pEnd) 
		{
			//no shape
			if(pEnd.x == pStart.x && pEnd.y == pStart.y) return new Vector2D[0];

			//line
			if(pEnd.x == pStart.x || pEnd.y == pStart.y) return new[] { pStart, pEnd };

			//got shape
			if(bevelwidth < 0) 
			{
				currentbevelwidth = -Math.Min(Math.Abs(bevelwidth), Math.Min(width, height) / 2) + 1;
			} 
			else 
			{
				currentbevelwidth = bevelwidth;
			}

			Vector2D[] shape = new Vector2D[subdivisions + 1];

			bool doBevel = false;
			int hw = width / 2;
			int hh = height / 2;

			Vector2D center = new Vector2D(pStart.x + hw, pStart.y + hh);
			float curAngle = 0;
			float angleStep = -Angle2D.PI / subdivisions * 2;

			for(int i = 0; i < subdivisions; i++) 
			{
				int px, py;
				if(doBevel) 
				{
					px = (int)(center.x - (float)Math.Sin(curAngle) * (hw + currentbevelwidth));
					py = (int)(center.y - (float)Math.Cos(curAngle) * (hh + currentbevelwidth));
				} 
				else 
				{
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

		protected override string GetHintText() 
		{
			return "BVL: " + bevelwidth + "; VERTS: " + subdivisions;
		}

		#endregion

		#region ================== Events

		private void OptionsPanelOnValueChanged(object sender, EventArgs eventArgs) 
		{
			bevelwidth = panel.Spikiness;
			subdivisions = Math.Min(maxsubdivisions, panel.Subdivisions);
			Update();
		}

		public override void OnHelp() 
		{
			General.ShowHelp("/gzdb/features/classic_modes/mode_drawellipse.html");
		}

		#endregion

		#region ================== Actions

		override protected void IncreaseSubdivLevel() 
		{
			if(maxsubdivisions - subdivisions > 1) 
			{
				subdivisions += 2;
				panel.Subdivisions = subdivisions;
				Update();
			}
		}

		override protected void DecreaseSubdivLevel() 
		{
			if(subdivisions - minsubdivisions > 1) 
			{
				subdivisions -= 2;
				panel.Subdivisions = subdivisions;
				Update();
			}
		}

		protected override void IncreaseBevel() 
		{
			if(points.Count < 2 || currentbevelwidth == bevelwidth || bevelwidth < 0) 
			{
				bevelwidth = Math.Min(bevelwidth + General.Map.Grid.GridSize, panel.MaxSpikiness);
				panel.Spikiness = bevelwidth;
				Update();
			}
		}

		protected override void DecreaseBevel() 
		{
			if(bevelwidth > 0 || currentbevelwidth <= bevelwidth + 1) 
			{
				bevelwidth = Math.Max(bevelwidth - General.Map.Grid.GridSize, panel.MinSpikiness);
				panel.Spikiness = bevelwidth;
				Update();
			}
		}

		#endregion
	}
}
