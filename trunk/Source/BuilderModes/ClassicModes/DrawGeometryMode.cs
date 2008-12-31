
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
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using CodeImp.DoomBuilder.Windows;
using CodeImp.DoomBuilder.IO;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Rendering;
using CodeImp.DoomBuilder.Geometry;
using System.Drawing;
using CodeImp.DoomBuilder.Editing;
using CodeImp.DoomBuilder.Actions;

#endregion

namespace CodeImp.DoomBuilder.BuilderModes
{
	[EditMode(DisplayName = "Drawing Mode",
			  SwitchAction = "drawlinesmode",
			  Volatile = true,
			  UseByDefault = true,
			  Optional = false)]

	public class DrawGeometryMode : BaseClassicMode
	{
		#region ================== Constants

		private const float LINE_THICKNESS = 0.8f;

		#endregion

		#region ================== Variables

		// Drawing points
		private List<DrawnVertex> points;
		private List<LineLengthLabel> labels;

		// Keep track of view changes
		private float lastoffsetx;
		private float lastoffsety;
		private float lastscale;

		// Options
		private bool snaptogrid;		// SHIFT to toggle
		private bool snaptonearest;		// CTRL to enable
		
		#endregion

		#region ================== Properties

		// Just keep the base mode button checked
		public override string EditModeButtonName { get { return General.Editing.PreviousStableMode.Name; } }

		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		public DrawGeometryMode()
		{
			// Initialize
			points = new List<DrawnVertex>();
			labels = new List<LineLengthLabel>();
			
			// No selection in this mode
			General.Map.Map.ClearAllSelected();
			General.Map.Map.ClearAllMarks(false);
			
			// We have no destructor
			GC.SuppressFinalize(this);
		}

		// Disposer
		public override void Dispose()
		{
			// Not already disposed?
			if(!isdisposed)
			{
				// Clean up
				if(labels != null)
					foreach(LineLengthLabel l in labels) l.Dispose();
				
				// Done
				base.Dispose();
			}
		}

		#endregion

		#region ================== Methods

		// This checks if the view offset/zoom changed and updates the check
		protected bool CheckViewChanged()
		{
			bool viewchanged = false;

			// View changed?
			if(renderer.OffsetX != lastoffsetx) viewchanged = true;
			if(renderer.OffsetY != lastoffsety) viewchanged = true;
			if(renderer.Scale != lastscale) viewchanged = true;

			// Keep view information
			lastoffsetx = renderer.OffsetX;
			lastoffsety = renderer.OffsetY;
			lastscale = renderer.Scale;

			// Return result
			return viewchanged;
		}
		
		// This updates the dragging
		private void Update()
		{
			PixelColor stitchcolor = General.Colors.Highlight;
			PixelColor losecolor = General.Colors.Selection;
			PixelColor color;

			snaptogrid = General.Interface.ShiftState ^ General.Interface.SnapToGrid;
			snaptonearest = General.Interface.CtrlState ^ General.Interface.AutoMerge;

			DrawnVertex lastp = new DrawnVertex();
			DrawnVertex curp = GetCurrentPosition();
			float vsize = ((float)renderer.VertexSize + 1.0f) / renderer.Scale;
			float vsizeborder = ((float)renderer.VertexSize + 3.0f) / renderer.Scale;

			// The last label's end must go to the mouse cursor
			if(labels.Count > 0) labels[labels.Count - 1].End = curp.pos;

			// Render drawing lines
			if(renderer.StartOverlay(true))
			{
				// Go for all points to draw lines
				if(points.Count > 0)
				{
					// Render lines
					lastp = points[0];
					for(int i = 1; i < points.Count; i++)
					{
						// Determine line color
						if(lastp.stitchline && points[i].stitchline) color = stitchcolor;
						else color = losecolor;

						// Render line
						renderer.RenderLine(lastp.pos, points[i].pos, LINE_THICKNESS, color, true);
						lastp = points[i];
					}

					// Determine line color
					if(lastp.stitchline && snaptonearest) color = stitchcolor;
					else color = losecolor;

					// Render line to cursor
					renderer.RenderLine(lastp.pos, curp.pos, LINE_THICKNESS, color, true);

					// Render vertices
					for(int i = 0; i < points.Count; i++)
					{
						// Determine vertex color
						if(points[i].stitch) color = stitchcolor;
						else color = losecolor;

						// Render vertex
						renderer.RenderRectangleFilled(new RectangleF(points[i].pos.x - vsize, points[i].pos.y - vsize, vsize * 2.0f, vsize * 2.0f), color, true);
					}
				}

				// Determine point color
				if(snaptonearest) color = stitchcolor;
				else color = losecolor;

				// Render vertex at cursor
				renderer.RenderRectangleFilled(new RectangleF(curp.pos.x - vsize, curp.pos.y - vsize, vsize * 2.0f, vsize * 2.0f), color, true);

				// Go for all labels
				foreach(LineLengthLabel l in labels) renderer.RenderText(l.TextLabel);

				// Done
				renderer.Finish();
			}

			// Done
			renderer.Present();
		}
		
		// This returns the aligned and snapped draw position
		public static DrawnVertex GetCurrentPosition(Vector2D mousemappos, bool snaptonearest, bool snaptogrid, IRenderer2D renderer, List<DrawnVertex> points)
		{
			DrawnVertex p = new DrawnVertex();
			float vrange = VerticesMode.VERTEX_HIGHLIGHT_RANGE / renderer.Scale;

			// Snap to nearest?
			if(snaptonearest)
			{
				// Go for all drawn points
				foreach(DrawnVertex v in points)
				{
					if(Vector2D.DistanceSq(mousemappos, v.pos) < (vrange * vrange))
					{
						p.pos = v.pos;
						p.stitch = true;
						p.stitchline = true;
						return p;
					}
				}

				// Try the nearest vertex
				Vertex nv = General.Map.Map.NearestVertexSquareRange(mousemappos, vrange);
				if(nv != null)
				{
					p.pos = nv.Position;
					p.stitch = true;
					p.stitchline = true;
					return p;
				}

				// Try the nearest linedef
				Linedef nl = General.Map.Map.NearestLinedefRange(mousemappos, LinedefsMode.LINEDEF_HIGHLIGHT_RANGE / renderer.Scale);
				if(nl != null)
				{
					// Snap to grid?
					if(snaptogrid)
					{
						// Get grid intersection coordinates
						List<Vector2D> coords = nl.GetGridIntersections();

						// Find nearest grid intersection
						float found_distance = float.MaxValue;
						Vector2D found_coord = new Vector2D();
						foreach(Vector2D v in coords)
						{
							Vector2D delta = mousemappos - v;
							if(delta.GetLengthSq() < found_distance)
							{
								found_distance = delta.GetLengthSq();
								found_coord = v;
							}
						}

						// Align to the closest grid intersection
						p.pos = found_coord;
						p.stitch = true;
						p.stitchline = true;
						return p;
					}
					else
					{
						// Aligned to line
						p.pos = nl.NearestOnLine(mousemappos);
						p.stitch = true;
						p.stitchline = true;
						return p;
					}
				}
			}
			else
			{
				// Always snap to the first drawn vertex so that the user can finish a complete sector without stitching
				if(points.Count > 0)
				{
					if(Vector2D.DistanceSq(mousemappos, points[0].pos) < (vrange * vrange))
					{
						p.pos = points[0].pos;
						p.stitch = true;
						p.stitchline = false;
						return p;
					}
				}
			}

			// Snap to grid?
			if(snaptogrid)
			{
				// Aligned to grid
				p.pos = General.Map.Grid.SnappedToGrid(mousemappos);
				p.stitch = snaptonearest;
				p.stitchline = snaptonearest;
				return p;
			}
			else
			{
				// Normal position
				p.pos = mousemappos;
				p.stitch = snaptonearest;
				p.stitchline = snaptonearest;
				return p;
			}
		}
		
		// This gets the aligned and snapped draw position
		private DrawnVertex GetCurrentPosition()
		{
			return GetCurrentPosition(mousemappos, snaptonearest, snaptogrid, renderer, points);
		}
		
		// This draws a point at a specific location
		public void DrawPointAt(DrawnVertex p)
		{
			DrawPointAt(p.pos, p.stitch, p.stitchline);
		}
		
		// This draws a point at a specific location
		public void DrawPointAt(Vector2D pos, bool stitch, bool stitchline)
		{
			DrawnVertex newpoint = new DrawnVertex();
			newpoint.pos = pos;
			newpoint.stitch = stitch;
			newpoint.stitchline = stitchline;
			points.Add(newpoint);
			labels.Add(new LineLengthLabel());
			labels[labels.Count - 1].Start = newpoint.pos;
			if(labels.Count > 1) labels[labels.Count - 2].End = newpoint.pos;
			Update();

			// Check if point stitches with the first
			if((points.Count > 1) && points[points.Count - 1].stitch)
			{
				Vector2D p1 = points[0].pos;
				Vector2D p2 = points[points.Count - 1].pos;
				Vector2D delta = p1 - p2;
				if((Math.Abs(delta.x) <= 0.001f) && (Math.Abs(delta.y) <= 0.001f))
				{
					// Finish drawing
					FinishDraw();
				}
			}
		}
		
		#endregion

		#region ================== Events

		// Engaging
		public override void OnEngage()
		{
			base.OnEngage();
			renderer.SetPresentation(Presentation.Standard);
			
			// Set cursor
			General.Interface.SetCursor(Cursors.Cross);
		}
		
		// Cancelled
		public override void OnCancel()
		{
			// Cancel base class
			base.OnCancel();
			
			// Return to original mode
			General.Editing.ChangeMode(General.Editing.PreviousStableMode.Name);
		}

		// Accepted
		public override void OnAccept()
		{
			Cursor.Current = Cursors.AppStarting;

			General.Settings.FindDefaultDrawSettings();

			// When points have been drawn
			if(points.Count > 0)
			{
				// Make undo for the draw
				General.Map.UndoRedo.CreateUndo("Line draw");

				// Make the drawing
				Tools.DrawLines(points);
				
				// Make selection from marked (new) geometry
				General.Map.Map.ClearAllSelected();
				General.Map.Map.SelectMarkedGeometry(true, true);
				
				// Update cached values
				General.Map.Map.Update();

				// Map is changed
				General.Map.IsChanged = true;
			}

			// Done
			Cursor.Current = Cursors.Default;
			
			// Return to original mode
			General.Editing.ChangeMode(General.Editing.PreviousStableMode.Name);
		}

		// This redraws the display
		public override void OnRedrawDisplay()
		{
			renderer.RedrawSurface();

			// Render lines
			if(renderer.StartPlotter(true))
			{
				renderer.PlotLinedefSet(General.Map.Map.Linedefs);
				renderer.PlotVerticesSet(General.Map.Map.Vertices);
				renderer.Finish();
			}

			// Render things
			if(renderer.StartThings(true))
			{
				renderer.RenderThingSet(General.Map.Map.Things, 1.0f);
				renderer.Finish();
			}

			// Normal update
			Update();
		}
		
		// Mouse moving
		public override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);
			Update();
		}

		// When a key is released
		public override void OnKeyUp(KeyEventArgs e)
		{
			base.OnKeyUp(e);
			if((snaptogrid != (General.Interface.ShiftState ^ General.Interface.SnapToGrid)) ||
			   (snaptonearest != (General.Interface.CtrlState ^ General.Interface.AutoMerge))) Update();
		}

		// When a key is pressed
		public override void OnKeyDown(KeyEventArgs e)
		{
			base.OnKeyDown(e);
			if((snaptogrid != (General.Interface.ShiftState ^ General.Interface.SnapToGrid)) ||
			   (snaptonearest != (General.Interface.CtrlState ^ General.Interface.AutoMerge))) Update();
		}
		
		#endregion
		
		#region ================== Actions
		
		// Drawing a point
		[BeginAction("drawpoint")]
		public void DrawPoint()
		{
			// Mouse inside window?
			if(General.Interface.MouseInDisplay)
			{
				DrawnVertex newpoint = GetCurrentPosition();
				DrawPointAt(newpoint);
			}
		}
		
		// Remove a point
		[BeginAction("removepoint")]
		public void RemovePoint()
		{
			if(points.Count > 0) points.RemoveAt(points.Count - 1);
			if(labels.Count > 0)
			{
				labels[labels.Count - 1].Dispose();
				labels.RemoveAt(labels.Count - 1);
			}
			
			Update();
		}
		
		// Finish drawing
		[BeginAction("finishdraw")]
		public void FinishDraw()
		{
			// Accept the changes
			General.Editing.AcceptMode();
		}
		
		#endregion
	}
}
