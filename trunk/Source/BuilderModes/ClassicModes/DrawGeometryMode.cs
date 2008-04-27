
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
using CodeImp.DoomBuilder.Interface;
using CodeImp.DoomBuilder.IO;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Rendering;
using CodeImp.DoomBuilder.Geometry;
using System.Drawing;
using CodeImp.DoomBuilder.Editing;
using CodeImp.DoomBuilder.Controls;

#endregion

namespace CodeImp.DoomBuilder.BuilderModes.Editing
{
	[EditMode(SwitchAction = "drawlinesmode")]

	public class DrawGeometryMode : ClassicMode
	{
		#region ================== Constants

		private const float LINE_THICKNESS = 0.6f;

		#endregion

		#region ================== Variables

		// Mode to return to
		private EditMode basemode;

		// Drawing points
		private List<Vector2D> points;

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
		public override string EditModeButtonName { get { return basemode.GetType().Name; } }

		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		public DrawGeometryMode()
		{
			// Initialize
			this.basemode = General.Map.Mode;
			points = new List<Vector2D>();
			
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

				// Done
				base.Dispose();
			}
		}

		#endregion

		#region ================== Methods

		// Cancelled
		public override void Cancel()
		{
			// Cancel base class
			base.Cancel();
			
			// Return to original mode
			Type t = basemode.GetType();
			basemode = (EditMode)Activator.CreateInstance(t);
			General.Map.ChangeMode(basemode);
		}

		// Disenagaging
		public override void Disengage()
		{
			base.Disengage();
			Cursor.Current = Cursors.AppStarting;

			// When not cancelled and points have been drawn
			if(!cancelled && (points.Count > 0))
			{
				// Make undo for the draw
				General.Map.UndoRedo.CreateUndo("line draw", UndoGroup.None, 0);
				


				// Update cached values
				General.Map.Map.Update();

				// Map is changed
				General.Map.IsChanged = true;
			}

			// Hide highlight info
			General.Interface.HideInfo();

			// Done
			Cursor.Current = Cursors.Default;
		}

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

		// This redraws the display
		public override void RedrawDisplay()
		{
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
				renderer.SetThingsRenderOrder(false);
				renderer.RenderThingSet(General.Map.Map.Things);
				renderer.Finish();
			}

			// Normal update
			Update();
		}
		
		// This updates the dragging
		private void Update()
		{
			snaptogrid = General.Interface.ShiftState ^ General.Interface.SnapToGrid;
			snaptonearest = General.Interface.CtrlState;

			Vector2D lastp = new Vector2D(0, 0);
			Vector2D curp = GetCurrentPosition();
			float vsize = ((float)renderer.VertexSize + 1.0f) / renderer.Scale;
			
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
						renderer.RenderLine(lastp, points[i], LINE_THICKNESS, General.Colors.Selection, true);
						lastp = points[i];
					}
					
					// Render line to cursor
					renderer.RenderLine(lastp, curp, LINE_THICKNESS, General.Colors.Highlight, true);
					
					// Render vertices
					for(int i = 0; i < points.Count; i++)
						renderer.RenderRectangleFilled(new RectangleF(points[i].x - vsize, points[i].y - vsize, vsize * 2.0f, vsize * 2.0f), General.Colors.Selection, true);
				}

				// Render vertex at cursor
				renderer.RenderRectangleFilled(new RectangleF(curp.x - vsize, curp.y - vsize, vsize * 2.0f, vsize * 2.0f), General.Colors.Highlight, true);
				
				// Done
				renderer.Finish();
			}

			// Done
			renderer.Present();
		}
		
		// This gets the aligned and snapped draw position
		private Vector2D GetCurrentPosition()
		{
			// Snap to nearest?
			if(snaptonearest)
			{
				float vrange = VerticesMode.VERTEX_HIGHLIGHT_RANGE / renderer.Scale;
				
				// Go for all drawn points
				foreach(Vector2D v in points)
				{
					Vector2D delta = mousemappos - v;
					if(delta.GetLengthSq() < (vrange * vrange)) return v;
				}
				
				// Try the nearest vertex
				Vertex nv = General.Map.Map.NearestVertexSquareRange(mousemappos, vrange);
				if(nv != null) return nv.Position;
				
				// Try the nearest linedef
				Linedef nl = General.Map.Map.NearestLinedefRange(mousemappos, LinedefsMode.LINEDEF_HIGHLIGHT_RANGE / renderer.Scale);
				if(nl != null)
				{
					// Snap to grid?
					if(snaptogrid)
					{
						// Aligned to line and grid
						// TODO: Find nearest horzontal and vertical grid intersections and align there
						return nl.NearestOnLine(mousemappos);
					}
					else
					{
						// Aligned to line
						return nl.NearestOnLine(mousemappos);
					}
				}
			}

			// Snap to grid?
			if(snaptogrid)
			{
				// Aligned to grid
				return General.Map.Grid.SnappedToGrid(mousemappos);
			}
			else
			{
				// Normal position
				return mousemappos;
			}
		}
		
		// Mouse moving
		public override void MouseMove(MouseEventArgs e)
		{
			base.MouseMove(e);
			Update();
		}

		// Drawing a point
		[BeginAction("drawpoint")]
		public void DrawPoint()
		{
			// Mouse inside window?
			if(General.Interface.MouseInDisplay)
			{
				points.Add(GetCurrentPosition());
				Update();
			}
		}

		// Remove a point
		[BeginAction("removepoint")]
		public void RemovePoint()
		{
			if(points.Count > 0) points.RemoveAt(points.Count - 1);
			Update();
		}

		// Finish drawing
		[BeginAction("finishdraw")]
		public void FinishDraw()
		{
			// Just return to base mode, Disengage will be called automatically.
			General.Map.ChangeMode(basemode);
		}

		// When a key is released
		public override void KeyUp(KeyEventArgs e)
		{
			base.KeyUp(e);
			Update();
		}

		// When a key is pressed
		public override void KeyDown(KeyEventArgs e)
		{
			base.KeyDown(e);
			Update();
		}
		
		#endregion
	}
}
