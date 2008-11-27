
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
using CodeImp.DoomBuilder.Actions;
using CodeImp.DoomBuilder.Geometry;
using System.Drawing;

#endregion

namespace CodeImp.DoomBuilder.Editing
{
	/// <summary>
	/// Provides specialized functionality for a classic (2D) Doom Builder editing mode.
	/// </summary>
	public abstract class ClassicMode : EditMode
	{
		#region ================== Constants

		private const float SCALE_MAX = 20f;
		private const float SCALE_MIN = 0.01f;
		private const float SELECTION_BORDER_SIZE = 2f;
		private const int SELECTION_ALPHA = 200;

		#endregion

		#region ================== Variables

		// Cancelled?
		protected bool cancelled;

		// Graphics
		protected IRenderer2D renderer;
		private Renderer2D renderer2d;
		
		// Mouse status
		protected Vector2D mousepos;
        protected Vector2D mouselastpos;
		protected Vector2D mousemappos;
		protected Vector2D mousedownpos;
		protected Vector2D mousedownmappos;
		protected MouseButtons mousebuttons;
		protected bool mouseinside;
		protected MouseButtons mousedragging = MouseButtons.None;
		
		// Selection
		protected bool selecting;
		private Vector2D selectstart;
		protected RectangleF selectionrect;

        // View panning
        protected bool panning;
		
		#endregion

		#region ================== Properties
		
		#endregion

		#region ================== Constructor / Disposer

		/// <summary>
		/// Provides specialized functionality for a classic (2D) Doom Builder editing mode.
		/// </summary>
		public ClassicMode()
		{
			// Initialize
			this.renderer = General.Map.Renderer2D;
			this.renderer2d = (Renderer2D)General.Map.Renderer2D;

			// If the current mode is a ClassicMode, copy mouse properties
			if(General.Editing.Mode is ClassicMode)
			{
				ClassicMode oldmode = General.Editing.Mode as ClassicMode;

				// Copy mouse properties
				mousepos = oldmode.mousepos;
				mousemappos = oldmode.mousemappos;
				mousedownpos = oldmode.mousedownpos;
				mousedownmappos = oldmode.mousedownmappos;
				mousebuttons = oldmode.mousebuttons;
				mouseinside = oldmode.mouseinside;
				mousedragging = oldmode.mousedragging;
			}
		}

		// Disposer
		public override void Dispose()
		{
			// Not already disposed?
			if(!isdisposed)
			{
				// Clean up

				// Dispose base
				base.Dispose();
			}
		}

		#endregion

		#region ================== Scroll / Zoom

		// This scrolls the view north
		[BeginAction("scrollnorth", BaseAction = true)]
		public virtual void ScrollNorth()
		{
			// Scroll
			ScrollBy(0f, 100f / renderer2d.Scale);
		}

		// This scrolls the view south
		[BeginAction("scrollsouth", BaseAction = true)]
		public virtual void ScrollSouth()
		{
			// Scroll
			ScrollBy(0f, -100f / renderer2d.Scale);
		}

		// This scrolls the view west
		[BeginAction("scrollwest", BaseAction = true)]
		public virtual void ScrollWest()
		{
			// Scroll
			ScrollBy(-100f / renderer2d.Scale, 0f);
		}

		// This scrolls the view east
		[BeginAction("scrolleast", BaseAction = true)]
		public virtual void ScrollEast()
		{
			// Scroll
			ScrollBy(100f / renderer2d.Scale, 0f);
		}

		// This zooms in
		[BeginAction("zoomin", BaseAction = true)]
		public virtual void ZoomIn()
		{
			// Zoom
			ZoomBy(1.3f);
		}

		// This zooms out
		[BeginAction("zoomout", BaseAction = true)]
		public virtual void ZoomOut()
		{
			// Zoom
			ZoomBy(0.7f);
		}

		// This scrolls anywhere
		private void ScrollBy(float deltax, float deltay)
		{
			// Scroll now
			renderer2d.PositionView(renderer2d.OffsetX + deltax, renderer2d.OffsetY + deltay);
			this.OnViewChanged();
			
			// Redraw
			General.MainWindow.RedrawDisplay();

			// Determine new unprojected mouse coordinates
			mousemappos = renderer2d.GetMapCoordinates(mousepos);
			General.MainWindow.UpdateCoordinates(mousemappos);
		}

        // This sets the view to be centered at x,y
        private void ScrollTo(float x, float y)
        {
            // Scroll now
            renderer2d.PositionView(x, y);
            this.OnViewChanged();

            // Redraw
            General.MainWindow.RedrawDisplay();

            // Determine new unprojected mouse coordinates
            mousemappos = renderer2d.GetMapCoordinates(mousepos);
            General.MainWindow.UpdateCoordinates(mousemappos);
        }

		// This zooms
		private void ZoomBy(float deltaz)
		{
			Vector2D zoompos, clientsize, diff;
			float newscale;
			
			// This will be the new zoom scale
			newscale = renderer2d.Scale * deltaz;

			// Limit scale
			if(newscale > SCALE_MAX) newscale = SCALE_MAX;
			if(newscale < SCALE_MIN) newscale = SCALE_MIN;
			
			// Get the dimensions of the display
			clientsize = new Vector2D(General.Map.Graphics.RenderTarget.ClientSize.Width,
									  General.Map.Graphics.RenderTarget.ClientSize.Height);
			
			// When mouse is inside display
			if(mouseinside)
			{
				// Zoom into or from mouse position
				zoompos = (mousepos / clientsize) - new Vector2D(0.5f, 0.5f);
			}
			else
			{
				// Zoom into or from center
				zoompos = new Vector2D(0f, 0f);
			}

			// Calculate view position difference
			diff = ((clientsize / newscale) - (clientsize / renderer2d.Scale)) * zoompos;

			// Zoom now
			renderer2d.PositionView(renderer2d.OffsetX - diff.x, renderer2d.OffsetY + diff.y);
			renderer2d.ScaleView(newscale);
			this.OnViewChanged();

			// Redraw
			//General.Map.Map.Update();
			General.MainWindow.RedrawDisplay();
			
			// Give a new mousemove event to update coordinates
			if(mouseinside) OnMouseMove(new MouseEventArgs(mousebuttons, 0, (int)mousepos.x, (int)mousepos.y, 0));
		}

		// This zooms to a specific level
		public void SetZoom(float newscale)
		{
			// Zoom now
			renderer2d.ScaleView(newscale);
			this.OnViewChanged();

			// Redraw
			//General.Map.Map.Update();
			General.MainWindow.RedrawDisplay();

			// Give a new mousemove event to update coordinates
			if(mouseinside) OnMouseMove(new MouseEventArgs(mousebuttons, 0, (int)mousepos.x, (int)mousepos.y, 0));
		}
		
		// This zooms and scrolls to fit the map in the window
		[BeginAction("centerinscreen", BaseAction = true)]
		public void CenterInScreen()
		{
			float left = float.MaxValue;
			float top = float.MaxValue;
			float right = float.MinValue;
			float bottom = float.MinValue;
			float scalew, scaleh, scale;
			float width, height;

			// Go for all vertices
			foreach(Vertex v in General.Map.Map.Vertices)
			{
				// Vertex used?
				if(v.Linedefs.Count > 0)
				{
					// Adjust boundaries by vertices
					if(v.Position.x < left) left = v.Position.x;
					if(v.Position.x > right) right = v.Position.x;
					if(v.Position.y < top) top = v.Position.y;
					if(v.Position.y > bottom) bottom = v.Position.y;
				}
			}

			// Calculate width/height
			width = (right - left);
			height = (bottom - top);

			// Calculate scale to view map at
			scalew = (float)General.Map.Graphics.RenderTarget.ClientSize.Width / (width * 1.1f);
			scaleh = (float)General.Map.Graphics.RenderTarget.ClientSize.Height / (height * 1.1f);
			if(scalew < scaleh) scale = scalew; else scale = scaleh;

			// Change the view to see the whole map
			renderer2d.ScaleView(scale);
			renderer2d.PositionView(left + (right - left) * 0.5f, top + (bottom - top) * 0.5f);
			this.OnViewChanged();
			
			// Redraw
			//General.Map.Map.Update();
			General.MainWindow.RedrawDisplay();

			// Give a new mousemove event to update coordinates
			if(mouseinside) OnMouseMove(new MouseEventArgs(mousebuttons, 0, (int)mousepos.x, (int)mousepos.y, 0));
		}

		/// <summary>
		/// This is called when the view changes (scroll/zoom), before the display is redrawn.
		/// </summary>
		protected virtual void OnViewChanged()
		{
		}
		
		#endregion
		
		#region ================== Input

		// Mouse leaves the display
		public override void OnMouseLeave(EventArgs e)
		{
			// Mouse is outside the display
			mouseinside = false;
			mousepos = new Vector2D(float.NaN, float.NaN);
			mousemappos = mousepos;
			mousebuttons = MouseButtons.None;
			
			// Determine new unprojected mouse coordinates
			General.MainWindow.UpdateCoordinates(mousemappos);
			
			// Let the base class know
			base.OnMouseLeave(e);
		}

		// Mouse moved inside the display
		public override void OnMouseMove(MouseEventArgs e)
		{
			Vector2D delta;

			// Record last position
			mouseinside = true;
            mouselastpos = mousepos;
			mousepos = new Vector2D(e.X, e.Y);
			mousemappos = renderer2d.GetMapCoordinates(mousepos);
			mousebuttons = e.Button;
			
			// Update labels in main window
			General.MainWindow.UpdateCoordinates(mousemappos);
			
			// Holding a button?
			if(e.Button != MouseButtons.None)
			{
				// Not dragging?
				if(mousedragging == MouseButtons.None)
				{
					// Check if moved enough pixels for dragging
					delta = mousedownpos - mousepos;
					if((Math.Abs(delta.x) > DRAG_START_MOVE_PIXELS) ||
					   (Math.Abs(delta.y) > DRAG_START_MOVE_PIXELS))
					{
						// Dragging starts now
						mousedragging = e.Button;
						OnDragStart(e);
					}
				}
			}
			
			// Selecting?
			if(selecting) OnUpdateMultiSelection();

            // Panning?
            if (panning) OnUpdateViewPanning();
			
			// Let the base class know
			base.OnMouseMove(e);
		}

		// Mouse button pressed
		public override void OnMouseDown(MouseEventArgs e)
		{
			// Save mouse down position
			mousedownpos = mousepos;
			mousedownmappos = mousemappos;
			
			// Let the base class know
			base.OnMouseDown(e);
		}

		// Mouse button released
		public override void OnMouseUp(MouseEventArgs e)
		{
			// Releasing drag button?
			if(e.Button == mousedragging)
			{
				// No longer dragging
				OnDragStop(e);
				mousedragging = MouseButtons.None;
			}
			
			// Let the base class know
			base.OnMouseUp(e);
		}

		/// <summary>
		/// Automatically called when dragging operation starts.
		/// </summary>
		protected virtual void OnDragStart(MouseEventArgs e)
		{
		}

		/// <summary>
		/// Automatically called when dragging operation stops.
		/// </summary>
		protected virtual void OnDragStop(MouseEventArgs e)
		{
		}
		
		#endregion

		#region ================== Display

		// This just refreshes the display
		public override void OnPresentDisplay()
		{
			renderer2d.Present();
		}

		// This sets the view mode
		private void SetViewMode(ViewMode mode)
		{
			General.Map.CRenderer2D.SetViewMode(mode);
			General.MainWindow.UpdateInterface();
			General.MainWindow.RedrawDisplay();
		}
		
		#endregion

		#region ================== Methods

		/// <summary>
		/// Automatically called by the core when this editing mode is engaged.
		/// </summary>
		public override void OnEngage()
		{
			// Clear display overlay
			renderer.StartOverlay(true);
			renderer.Finish();
			base.OnEngage();
		}

		/// <summary>
		/// Called when the user requests to cancel this editing mode.
		/// </summary>
		public override void OnCancel()
		{
			cancelled = true;
			base.OnCancel();
		}

		/// <summary>
		/// This is called automatically when the Edit button is pressed.
		/// (in Doom Builder 1, this was always the right mousebutton)
		/// </summary>
		[BeginAction("classicedit", BaseAction = true)]
		protected virtual void OnEditBegin()
		{
		}

		/// <summary>
		/// This is called automatically when the Edit button is released.
		/// (in Doom Builder 1, this was always the right mousebutton)
		/// </summary>
		[EndAction("classicedit", BaseAction = true)]
		protected virtual void OnEditEnd()
		{
		}

		/// <summary>
		/// This is called automatically when the Select button is pressed.
		/// (in Doom Builder 1, this was always the left mousebutton)
		/// </summary>
		[BeginAction("classicselect", BaseAction = true)]
		protected virtual void OnSelectBegin()
		{
		}

		/// <summary>
		/// This is called automatically when the Select button is released.
		/// (in Doom Builder 1, this was always the left mousebutton)
		/// </summary>
		[EndAction("classicselect", BaseAction = true)]
		protected virtual void OnSelectEnd()
		{
			if(selecting) OnEndMultiSelection();
		}

		/// <summary>
		/// This is called automatically when a rectangular multi-selection ends.
		/// </summary>
		protected virtual void OnEndMultiSelection()
		{
			selecting = false;
		}

		/// <summary>
		/// Call this to initiate a rectangular multi-selection.
		/// </summary>
		protected virtual void StartMultiSelection()
		{
			selecting = true;
			selectstart = mousemappos;
			selectionrect = new RectangleF(selectstart.x, selectstart.y, 0, 0);
		}

		/// <summary>
		/// This is called automatically when a multi-selection is updated.
		/// </summary>
		protected virtual void OnUpdateMultiSelection()
		{
			selectionrect.X = selectstart.x;
			selectionrect.Y = selectstart.y;
			selectionrect.Width = mousemappos.x - selectstart.x;
			selectionrect.Height = mousemappos.y - selectstart.y;
			
			if(selectionrect.Width < 0f)
			{
				selectionrect.Width = -selectionrect.Width;
				selectionrect.X -= selectionrect.Width;
			}
			
			if(selectionrect.Height < 0f)
			{
				selectionrect.Height = -selectionrect.Height;
				selectionrect.Y -= selectionrect.Height;
			}
		}

		/// <summary>
		/// Call this to draw the selection on the overlay layer.
		/// Must call renderer.StartOverlay first!
		/// </summary>
		protected virtual void RenderMultiSelection()
		{
			renderer.RenderRectangle(selectionrect, SELECTION_BORDER_SIZE,
				General.Colors.Highlight.WithAlpha(SELECTION_ALPHA), true);
		}

        /// <summary>
        /// This is called automatically when the mouse is moved while panning
        /// </summary>
        protected virtual void OnUpdateViewPanning()
        {
			// We can only drag the map when the mouse pointer is inside
			// otherwise we don't have coordinates where to drag the map to
			if(mouseinside && !float.IsNaN(mouselastpos.x) && !float.IsNaN(mouselastpos.y))
			{
				// Get the map coordinates of the last mouse posision (before it moved)
				Vector2D lastmappos = renderer2d.GetMapCoordinates(mouselastpos);
				
				// Do the scroll
				ScrollBy(lastmappos.x - mousemappos.x, lastmappos.y - mousemappos.y);
			}
        }
		
		#endregion
		
		#region ================== Actions

        [BeginAction("pan_view", BaseAction = true)]
        protected virtual void BeginViewPan()
        {
            panning = true;
        }

        [EndAction("pan_view", BaseAction = true)]
        protected virtual void EndViewPan()
        {
            panning = false;
        }

		[BeginAction("viewmodenormal", BaseAction = true)]
		protected virtual void ViewModeNormal()
		{
			SetViewMode(ViewMode.Normal);
		}

		[BeginAction("viewmodebrightness", BaseAction = true)]
		protected virtual void ViewModeBrightness()
		{
			SetViewMode(ViewMode.Brightness);
		}

		[BeginAction("viewmodefloors", BaseAction = true)]
		protected virtual void ViewModeFloors()
		{
			SetViewMode(ViewMode.FloorTextures);
		}

		[BeginAction("viewmodeceilings", BaseAction = true)]
		protected virtual void ViewModeCeilings()
		{
			SetViewMode(ViewMode.CeilingTextures);
		}

		#endregion
	}
}
