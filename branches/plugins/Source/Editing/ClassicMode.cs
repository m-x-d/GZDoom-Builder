
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
using CodeImp.DoomBuilder.Controls;
using CodeImp.DoomBuilder.Geometry;
using System.Drawing;

#endregion

namespace CodeImp.DoomBuilder.Editing
{
	public abstract class ClassicMode : EditMode
	{
		#region ================== Constants

		private const float SCALE_MAX = 20f;
		private const float SCALE_MIN = 0.01f;

		#endregion

		#region ================== Variables

		// Cancelled?
		protected bool cancelled;

		// Graphics
		protected IRenderer2D renderer;
		private Renderer2D renderer2d;
		
		// Mouse status
		protected Vector2D mousepos;
		protected Vector2D mousemappos;
		protected Vector2D mousedownpos;
		protected Vector2D mousedownmappos;
		protected MouseButtons mousebuttons;
		protected bool mouseinside;
		protected MouseButtons mousedragging = MouseButtons.None;
		
		#endregion

		#region ================== Properties
		
		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		public ClassicMode()
		{
			// Initialize
			this.renderer = General.Map.Renderer2D;
			this.renderer2d = (Renderer2D)General.Map.Renderer2D;
		}

		// Diposer
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
		[Action("scrollnorth", BaseAction = true)]
		public virtual void ScrollNorth()
		{
			// Scroll
			ScrollBy(0f, 100f / renderer2d.Scale);
		}

		// This scrolls the view south
		[Action("scrollsouth", BaseAction = true)]
		public virtual void ScrollSouth()
		{
			// Scroll
			ScrollBy(0f, -100f / renderer2d.Scale);
		}

		// This scrolls the view west
		[Action("scrollwest", BaseAction = true)]
		public virtual void ScrollWest()
		{
			// Scroll
			ScrollBy(-100f / renderer2d.Scale, 0f);
		}

		// This scrolls the view east
		[Action("scrolleast", BaseAction = true)]
		public virtual void ScrollEast()
		{
			// Scroll
			ScrollBy(100f / renderer2d.Scale, 0f);
		}

		// This zooms in
		[Action("zoomin", BaseAction = true)]
		public virtual void ZoomIn()
		{
			// Zoom
			ZoomBy(1.3f);
		}

		// This zooms out
		[Action("zoomout", BaseAction = true)]
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
			this.ViewChanged();
			
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
			this.ViewChanged();

			// Redraw
			//General.Map.Map.Update();
			General.MainWindow.RedrawDisplay();
			
			// Give a new mousemove event to update coordinates
			if(mouseinside) MouseMove(new MouseEventArgs(mousebuttons, 0, (int)mousepos.x, (int)mousepos.y, 0));
		}

		// This zooms to a specific level
		public void SetZoom(float newscale)
		{
			// Zoom now
			renderer2d.ScaleView(newscale);
			this.ViewChanged();

			// Redraw
			//General.Map.Map.Update();
			General.MainWindow.RedrawDisplay();

			// Give a new mousemove event to update coordinates
			if(mouseinside) MouseMove(new MouseEventArgs(mousebuttons, 0, (int)mousepos.x, (int)mousepos.y, 0));
		}
		
		// This zooms and scrolls to fit the map in the window
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
			this.ViewChanged();
			
			// Redraw
			//General.Map.Map.Update();
			General.MainWindow.RedrawDisplay();

			// Give a new mousemove event to update coordinates
			if(mouseinside) MouseMove(new MouseEventArgs(mousebuttons, 0, (int)mousepos.x, (int)mousepos.y, 0));
		}

		// This is called when the view changes (scroll/zoom)
		protected virtual void ViewChanged()
		{
		}
		
		#endregion
		
		#region ================== Input

		// Mouse leaves the display
		public override void MouseLeave(EventArgs e)
		{
			// Mouse is outside the display
			mouseinside = false;
			mousepos = new Vector2D(float.NaN, float.NaN);
			mousemappos = mousepos;
			mousebuttons = MouseButtons.None;
			
			// Determine new unprojected mouse coordinates
			General.MainWindow.UpdateCoordinates(mousemappos);
			
			// Let the base class know
			base.MouseLeave(e);
		}

		// Mouse moved inside the display
		public override void MouseMove(MouseEventArgs e)
		{
			Vector2D delta;

			// Record last position
			mouseinside = true;
			mousepos = new Vector2D(e.X, e.Y);
			mousemappos = renderer2d.GetMapCoordinates(mousepos);
			mousebuttons = e.Button;
			
			// Update labels in main window
			General.MainWindow.UpdateCoordinates(mousemappos);
			
			// Holding a button?
			if((e.Button == EditMode.EDIT_BUTTON) ||
			   (e.Button == EditMode.SELECT_BUTTON))
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
						DragStart(e);
					}
				}
			}
			
			// Let the base class know
			base.MouseMove(e);
		}

		// Mouse button pressed
		public override void MouseDown(MouseEventArgs e)
		{
			// Save mouse down position
			mousedownpos = mousepos;
			mousedownmappos = mousemappos;
			
			// Let the base class know
			base.MouseDown(e);
		}

		// Mouse button released
		public override void MouseUp(MouseEventArgs e)
		{
			// Releasing drag button?
			if(e.Button == mousedragging)
			{
				// No longer dragging
				DragStop(e);
				mousedragging = MouseButtons.None;
			}

			// Let the base class know
			base.MouseUp(e);
		}
		
		// This is called when the mouse is moved enough pixels and holding one or more buttons
		protected virtual void DragStart(MouseEventArgs e)
		{

		}

		// This is called when a drag is ended because the mouse buton is released
		protected virtual void DragStop(MouseEventArgs e)
		{

		}
		
		#endregion

		#region ================== Display

		// This just refreshes the display
		public override void RefreshDisplay()
		{
			renderer2d.Present();
		}

		#endregion

		#region ================== Methods

		// Override cancel method to bind it with its action
		[Action("cancelmode", BaseAction = true)]
		public override void Cancel()
		{
			cancelled = true;
			base.Cancel();
		}
		
		#endregion
	}
}
