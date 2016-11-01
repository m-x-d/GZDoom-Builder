
#region ================== Copyright (c) 2007 Pascal vd Heiden

/*
 * Copyright (c) 2007 Pascal vd Heiden, www.codeimp.com
 * This program is released under GNU General Public License
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 */

#endregion

#region ================== Namespaces

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using CodeImp.DoomBuilder.Actions;
using CodeImp.DoomBuilder.BuilderModes.Interface;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Rendering;
using CodeImp.DoomBuilder.Geometry;
using CodeImp.DoomBuilder.Editing;

#endregion

namespace CodeImp.DoomBuilder.BuilderModes
{
	[EditMode(DisplayName = "Curve Linedefs",
			  AllowCopyPaste = false,
			  Volatile = true)]
	public sealed class CurveLinedefsMode : BaseClassicMode
	{
		#region ================== Constants

		internal const int DEFAULT_VERTICES_COUNT = 8; //mxd
		internal const int DEFAULT_DISTANCE = 128; //mxd
		internal const int DEFAULT_ANGLE = 180; //mxd
		private const float LINE_THICKNESS = 0.6f;

		#endregion

		#region ================== Variables

		// Collections
		private ICollection<Linedef> selectedlines;
		private ICollection<Linedef> unselectedlines;
		private Dictionary<Linedef, List<Vector2D>> curves; //mxd

		//mxd. UI and controls
		private HintLabel hintlabel;
		private CurveLinedefsOptionsPanel panel;
		private Linedef closestline;
		private Vector2D mousedownoffset;
		private int prevoffset;
		
		#endregion

		#region ================== Properties

		// Just keep the base mode button checked
		public override string EditModeButtonName { get { return General.Editing.PreviousStableMode.Name; } }

		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		public CurveLinedefsMode()
		{
			// Make collections by selection
			selectedlines = General.Map.Map.GetSelectedLinedefs(true);
			unselectedlines = General.Map.Map.GetSelectedLinedefs(false);
			curves = new Dictionary<Linedef, List<Vector2D>>(selectedlines.Count); //mxd

			//mxd. UI
			panel = new CurveLinedefsOptionsPanel();
			hintlabel = new HintLabel(General.Colors.InfoLine);
		}

		public override void Dispose() //mxd
		{
			// Not already disposed?
			if(!isdisposed)
			{
				// Clean up
				if(hintlabel != null) hintlabel.Dispose();

				// Done
				base.Dispose();
			}
		}

		#endregion

		#region ================== Methods

		//mxd
		private void GenerateCurves()
		{
			foreach(Linedef ld in selectedlines) curves[ld] = GenerateCurve(ld);
		}

		// This generates the vertices to split the line with, from start to end
		private List<Vector2D> GenerateCurve(Linedef line)
		{
			// Fetch settings from the panel
			bool fixedcurve = panel.FixedCurve;
			int vertices = Math.Min(panel.Vertices, (int)Math.Ceiling(line.Length / 4));
			int distance = panel.Distance;
			int angle = (!fixedcurve && distance == 0 ? Math.Max(5, panel.Angle) : panel.Angle);
			float theta = Angle2D.DegToRad(angle);
			if(distance < 0) theta = -theta; //mxd

			// Make list
			List<Vector2D> points = new List<Vector2D>(vertices);

			float segDelta = 1.0f / (vertices + 1); //mxd
			Vector2D linecenter = line.GetCenterPoint(); //mxd

			//mxd. Special cases...
			if(angle == 0)
			{
				for(int v = 1; v <= vertices; v++)
				{
					float x = (line.Length * segDelta) * (vertices - v + 1) - line.Length * 0.5f; // Line segment coord
					
					// Rotate and transform to fit original line
					Vector2D vertex = new Vector2D(x, 0).GetRotated(line.Angle + Angle2D.PIHALF) + linecenter;
					points.Add(vertex);
				}
			}
			else
			{
				//Added by Anders �strand 2008-05-18
				//The formulas used are taken from http://mathworld.wolfram.com/CircularSegment.html
				//c and theta are known (length of line and angle parameter). d, R and h are
				//calculated from those two
				//If the curve is not supposed to be a circular segment it's simply deformed to fit
				//the value set for distance.

				//The vertices are generated to be evenly distributed (by angle) along the curve
				//and lastly they are rotated and moved to fit with the original line

				//calculate some identities of a circle segment (refer to the graph in the url above)
				float c = line.Length;

				float d = (c / (float)Math.Tan(theta / 2)) / 2;
				float R = d / (float)Math.Cos(theta / 2);
				float h = R - d;

				float yDeform = (fixedcurve ? 1 : distance / h);
				float xDelta = Math.Min(1, yDeform); //mxd
				
				for(int v = 1; v <= vertices; v++)
				{
					//calculate the angle for this vertex
					//the curve starts at PI/2 - theta/2 and is segmented into vertices+1 segments
					//this assumes the line is horisontal and on y = 0, the point is rotated and moved later

					float a = (Angle2D.PI - theta) / 2 + v * (theta / (vertices + 1));

					//calculate the coordinates of the point, and distort the y coordinate
					//using the deform factor calculated above
					float xr = (float)Math.Cos(a) * R; //mxd. Circle segment coord
					float xl = (line.Length * segDelta) * (vertices - v + 1) - line.Length * 0.5f; // mxd. Line segment coord
					float x = InterpolationTools.Linear(xl, xr, xDelta); //mxd
					float y = ((float)Math.Sin(a) * R - d) * yDeform;

					//rotate and transform to fit original line
					Vector2D vertex = new Vector2D(x, y).GetRotated(line.Angle + Angle2D.PIHALF) + linecenter;
					points.Add(vertex);
				}
			}

			// Done
			return points;
		}
		
		#endregion

		#region ================== Settings panel (mxd)

		private void AddInterface()
		{
			panel = new CurveLinedefsOptionsPanel();
			int vertices = General.Settings.ReadPluginSetting("curvelinedefsmode.vertices", DEFAULT_VERTICES_COUNT);
			int distance = General.Settings.ReadPluginSetting("curvelinedefsmode.distance", DEFAULT_DISTANCE);
			int angle = General.Settings.ReadPluginSetting("curvelinedefsmode.angle", DEFAULT_ANGLE);
			bool fixedcurve = General.Settings.ReadPluginSetting("curvelinedefsmode.fixedcurve", false);

			panel.SetValues(vertices, distance, angle, fixedcurve);
			panel.Register();
			panel.OnValueChanged += OnValuesChanged;
		}

		private void RemoveInterface()
		{
			panel.OnValueChanged -= OnValuesChanged;
			General.Settings.WritePluginSetting("curvelinedefsmode.vertices", panel.Vertices);
			General.Settings.WritePluginSetting("curvelinedefsmode.distance", panel.Distance);
			General.Settings.WritePluginSetting("curvelinedefsmode.angle", panel.Angle);
			General.Settings.WritePluginSetting("curvelinedefsmode.fixedcurve", panel.FixedCurve);
			panel.Unregister();
		}

		private void OnValuesChanged(object sender, EventArgs e)
		{
			// Update curves
			GenerateCurves();

			// Redraw display
			General.Interface.RedrawDisplay();
		}

		#endregion

		#region ================== Events

		public override void OnHelp()
		{
			General.ShowHelp("e_curvelinedefs.html");
		}

		// Cancelled
		public override void OnCancel()
		{
			// Cancel base class
			base.OnCancel();

			// Return to base mode
			General.Editing.ChangeMode(General.Editing.PreviousStableMode.Name);
		}

		// Mode engages
		public override void OnEngage()
		{
			base.OnEngage();
			renderer.SetPresentation(Presentation.Standard);
			
			//mxd
			AddInterface();
			GenerateCurves();
		}

		// Disenagaging
		public override void OnDisengage()
		{
			base.OnDisengage();

			// Hide toolbox panel
			RemoveInterface();
		}

		// This applies the curves and returns to the base mode
		public override void OnAccept()
		{
			// Create undo
			string rest = (selectedlines.Count == 1 ? "a linedef" : selectedlines.Count + " linedefs"); //mxd
			General.Map.UndoRedo.CreateUndo("Curve " + rest);

			//mxd
			General.Map.Map.ClearAllMarks(false);
			
			// Go for all selected lines
			foreach(Linedef ld in selectedlines)
			{
				if(curves[ld].Count < 1) continue;

				// Make curve for line
				Linedef splitline = ld;

				//mxd. Mark all changed geometry...
				splitline.Marked = true;
				splitline.Start.Marked = true;
				splitline.End.Marked = true;

				// Go for all points to split the line
				foreach(Vector2D p in curves[ld])
				{
					// Make vertex
					Vertex v = General.Map.Map.CreateVertex(p);
					if(v == null)
					{
						General.Map.UndoRedo.WithdrawUndo();
						return;
					}
						
					// Split the line and move on with this line
					splitline = splitline.Split(v);
					if(splitline == null)
					{
						General.Map.UndoRedo.WithdrawUndo();
						return;
					}

					//mxd. Mark all changed geometry...
					splitline.Marked = true;
					splitline.Start.Marked = true;
					splitline.End.Marked = true;
				}
			}

			//mxd
			General.Map.Map.Update();

			//mxd. Stitch geometry
			General.Map.Map.StitchGeometry(General.Settings.MergeGeometryMode);

			// Snap to map format accuracy
			General.Map.Map.SnapAllToAccuracy();
			
			// Update caches
			General.Map.Map.Update();
			General.Map.IsChanged = true;
			
			// Return to base mode
			General.Editing.ChangeMode(General.Editing.PreviousStableMode.Name);
		}

		// Redrawing display
		public override void OnRedrawDisplay()
		{
			renderer.RedrawSurface();

			// Render lines
			if(renderer.StartPlotter(true))
			{
				renderer.PlotLinedefSet(unselectedlines);
				renderer.PlotVerticesSet(General.Map.Map.Vertices);
				renderer.Finish();
			}

			// Render things
			if(renderer.StartThings(true))
			{
				renderer.RenderThingSet(General.Map.Map.Things, General.Settings.ActiveThingsAlpha);
				renderer.Finish();
			}

			// Render overlay
			if(renderer.StartOverlay(true))
			{
				// Go for all selected lines
				float vsize = (renderer.VertexSize + 1.0f) / renderer.Scale; //mxd
				foreach(Linedef ld in selectedlines)
				{
					// Make curve for line
					List<Vector2D> points = curves[ld];
					if(points.Count > 0)
					{
						Vector2D p1 = ld.Start.Position;
						Vector2D p2 = points[0];
						for(int i = 1; i <= points.Count; i++)
						{
							// Draw the line
							renderer.RenderLine(p1, p2, LINE_THICKNESS, General.Colors.Highlight, true);

							// Next points
							p1 = p2;
							if(i < points.Count) p2 = points[i];
						}

						// Draw last line
						renderer.RenderLine(p2, ld.End.Position, LINE_THICKNESS, General.Colors.Highlight, true);

						//mxd. Draw verts
						foreach(Vector2D p in points)
							renderer.RenderRectangleFilled(new RectangleF(p.x - vsize, p.y - vsize, vsize * 2.0f, vsize * 2.0f), General.Colors.Selection, true);
					}
				}

				//mxd. Render hint
				renderer.RenderText(hintlabel);

				renderer.Finish();
			}

			renderer.Present();
		}

		//mxd
		public override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown(e);
			closestline = MapSet.NearestLinedef(selectedlines, mousedownmappos);

			// Special cases...
			int distance;
			if(panel.FixedCurve)
			{
				if(panel.Angle > 0)
				{
					// Calculate diameter for current angle...
					float ma = Angle2D.DegToRad(panel.Angle);
					float d = (closestline.Length / (float)Math.Tan(ma / 2f)) / 2;
					float D = d / (float)Math.Cos(ma / 2f);
					distance = (int)Math.Round(D - d) * Math.Sign(panel.Distance);
				}
				else
				{
					distance = 0; // Special cases...
				}
			}
			else
			{
				distance = panel.Distance;
			}

			// Store offset between intial mouse position and curve top
			Vector2D perpendicular = closestline.Line.GetPerpendicular().GetNormal();
			if(distance != 0) perpendicular *= distance; // Special cases...
			Vector2D curvetop = closestline.GetCenterPoint() - perpendicular;
			mousedownoffset = mousedownmappos - curvetop;
		}

		//mxd
		public override void OnMouseUp(MouseEventArgs e)
		{
			base.OnMouseUp(e);
			closestline = null;
			prevoffset = 0;
		}

		//mxd
		public override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);

			// Anything to do?
			if((!selectpressed && !editpressed) || closestline == null)
			{
				hintlabel.Text = string.Empty;
				return;
			}

			// Do something...
			Vector2D perpendicular = closestline.Line.GetPerpendicular().GetNormal();
			if(panel.Distance != 0) perpendicular *= panel.Distance; // Special cases...
			Vector2D center = closestline.GetCenterPoint();
			Line2D radius = new Line2D(center, center - perpendicular);
			float u = radius.GetNearestOnLine(mousemappos - mousedownoffset);
			int dist = (panel.Distance == 0 ? 1 : panel.Distance); // Special cases...
			int offset = (int)Math.Round(dist * u - dist);
			bool updaterequired = false;

			// Clamp values?
			bool clampvalue = !General.Interface.ShiftState;

			// Change verts amount
			if(selectpressed && editpressed)
			{
				if(prevoffset != 0)
				{
					// Set new verts count without triggering the update...
					panel.SetValues(panel.Vertices + Math.Sign(prevoffset - offset), panel.Distance, panel.Angle, panel.FixedCurve);

					// Update hint text
					hintlabel.Text = "Vertices: " + panel.Vertices;
					updaterequired = true;
				}
			}
			// Change distance
			else if(selectpressed && !panel.FixedCurve)
			{
				if(float.IsNaN(u))
				{
					// Set new distance without triggering the update...
					panel.SetValues(panel.Vertices, 0, panel.Angle, panel.FixedCurve); // Special cases...
				}
				else
				{
					int newoffset;
					if(clampvalue)
						newoffset = (panel.Distance + offset) / panel.DistanceIncrement * panel.DistanceIncrement; // Clamp to 8 mu increments
					else
						newoffset = panel.Distance + offset;

					// Set new distance without triggering the update...
					panel.SetValues(panel.Vertices, newoffset, panel.Angle, panel.FixedCurve);
				}

				// Update hint text
				hintlabel.Text = "Distance: " + panel.Distance;
				updaterequired = true;
			}
			// Change angle
			else if(editpressed && prevoffset != 0)
			{
				int newangle = 0;
				if(panel.FixedCurve)
				{
					// Flip required?
					if(panel.Angle == 0 && (Math.Sign(offset - prevoffset) != Math.Sign(panel.Distance)))
					{
						// Set new distance without triggering the update...
						panel.SetValues(panel.Vertices, -panel.Distance, panel.Angle, panel.FixedCurve);

						// Recalculate affected values...
						perpendicular *= -1;
						radius.v2 = center - perpendicular;
						u = radius.GetNearestOnLine(mousemappos - mousedownoffset);
					}

					//TODO: there surely is a way to get new angle without iteration...
					float targetoffset = radius.GetLength() * u;
					float prevdiff = float.MaxValue;
					int increment = (clampvalue ? panel.AngleIncrement : 1);
					for(int i = 1; i < panel.MaximumAngle; i += increment)
					{
						// Calculate diameter for current angle...
						float ma = Angle2D.DegToRad(i);
						float d = (closestline.Length / (float)Math.Tan(ma / 2f)) / 2;
						float D = d / (float)Math.Cos(ma / 2f);
						float h = D - d;

						float curdiff = Math.Abs(h - targetoffset);

						// This one matches better...
						if(curdiff < prevdiff) newangle = i;
						prevdiff = curdiff;
					}

					// Clamp to 5 deg increments
					if(clampvalue) newangle = (newangle / panel.AngleIncrement) * panel.AngleIncrement; 
				}
				else
				{
					int diff = (int)Math.Round((offset - prevoffset) * renderer.Scale);
					if(panel.Angle + diff > 0)
					{
						if(clampvalue) newangle = (panel.Angle / panel.AngleIncrement + Math.Sign(diff)) * panel.AngleIncrement; // Clamp to 5 deg increments
						else newangle = panel.Angle + diff;
					}
				}

				// Set new angle without triggering the update...
				panel.SetValues(panel.Vertices, panel.Distance, newangle, panel.FixedCurve);

				// Update hint text
				hintlabel.Text = "Angle: " + panel.Angle;
				updaterequired = true;
			}

			// Update UI
			if(updaterequired)
			{
				// Update label position
				float labeldistance;

				if(panel.Angle == 0)
				{
					labeldistance = 0; // Special cases!
				}
				else if(panel.FixedCurve)
				{
					float ma = Angle2D.DegToRad(panel.Angle);
					float d = (closestline.Length / (float)Math.Tan(ma / 2f)) / 2;
					float D = d / (float)Math.Cos(ma / 2f);
					labeldistance = D - d;
				}
				else
				{
					labeldistance = Math.Abs(panel.Distance);
				}

				labeldistance += 16 / renderer.Scale;
				Vector2D labelpos = radius.GetCoordinatesAt(labeldistance / radius.GetLength());
				hintlabel.Move(labelpos, labelpos);

				// Trigger update
				OnValuesChanged(null, EventArgs.Empty);
			}

			// Store current offset
			prevoffset = offset;
		}
		
		#endregion

		#region ================== Actions (mxd)

		[BeginAction("increasesubdivlevel")]
		private void IncreaseSubdivLevel() { panel.Vertices += 1; }

		[BeginAction("decreasesubdivlevel")]
		private void DecreaseSubdivLevel() { panel.Vertices -= 1; }

		[BeginAction("increasebevel")]
		private void IncreaseBevel() { panel.Distance += panel.DistanceIncrement; }

		[BeginAction("decreasebevel")]
		private void DecreaseBevel() { panel.Distance -= panel.DistanceIncrement; }

		[BeginAction("rotateclockwise")]
		private void IncreaseAngle() { panel.Angle += panel.AngleIncrement; }

		[BeginAction("rotatecounterclockwise")]
		private void DecreaseAngle() { panel.Angle -= panel.AngleIncrement; }

		#endregion
	}
}
