
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
	internal class ViewClassicMode : EditMode
	{
		#region ================== Constants

		#endregion

		#region ================== Variables

		// Graphics
		protected Renderer2D renderer;
		
		// Mouse status
		protected Vector2D mousepos;
		protected Vector2D mousemappos;
		protected bool mouseinside;
		
		#endregion

		#region ================== Properties
		
		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		public ViewClassicMode()
		{
			// Initialize
			this.renderer = General.Map.Graphics.Renderer2D;
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
		[Action(Action.SCROLLNORTH)]
		public virtual void ScrollNorth()
		{
			// Scroll
			ScrollBy(0f, 100f / renderer.Scale);
		}

		// This scrolls the view south
		[Action(Action.SCROLLSOUTH)]
		public virtual void ScrollSouth()
		{
			// Scroll
			ScrollBy(0f, -100f / renderer.Scale);
		}

		// This scrolls the view west
		[Action(Action.SCROLLWEST)]
		public virtual void ScrollWest()
		{
			// Scroll
			ScrollBy(-100f / renderer.Scale, 0f);
		}

		// This scrolls the view east
		[Action(Action.SCROLLEAST)]
		public virtual void ScrollEast()
		{
			// Scroll
			ScrollBy(100f / renderer.Scale, 0f);
		}

		// This zooms in
		[Action(Action.ZOOMIN)]
		public virtual void ZoomIn()
		{
			// Zoom
			ZoomBy(1.2f);
		}

		// This zooms out
		[Action(Action.ZOOMOUT)]
		public virtual void ZoomOut()
		{
			// Zoom
			ZoomBy(0.8f);
		}

		// This scrolls anywhere
		private void ScrollBy(float deltax, float deltay)
		{
			// Scroll now
			renderer.PositionView(renderer.OffsetX + deltax, renderer.OffsetY + deltay);
			General.MainWindow.RedrawDisplay();

			// Determine new unprojected mouse coordinates
			mousemappos = renderer.GetMapCoordinates(mousepos);
			General.MainWindow.UpdateCoordinates(mousemappos);
		}

		// This zooms
		private void ZoomBy(float deltaz)
		{
			Vector2D zoompos, clientsize, diff;
			float newscale;
			
			// This will be the new zoom scale
			newscale = renderer.Scale * deltaz;

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
			diff = ((clientsize / newscale) - (clientsize / renderer.Scale)) * zoompos;

			// Zoom now
			renderer.PositionView(renderer.OffsetX - diff.x, renderer.OffsetY + diff.y);
			renderer.ScaleView(newscale);
			General.Map.Data.Update();
			General.MainWindow.RedrawDisplay();
			
			// Determine new unprojected mouse coordinates
			mousemappos = renderer.GetMapCoordinates(mousepos);
			General.MainWindow.UpdateCoordinates(mousemappos);
		}

		// This zooms to a specific level
		public void SetZoom(float newscale)
		{
			// Zoom now
			renderer.ScaleView(newscale);
			General.Map.Data.Update();
			General.MainWindow.RedrawDisplay();

			// Determine new unprojected mouse coordinates
			mousemappos = renderer.GetMapCoordinates(mousepos);
			General.MainWindow.UpdateCoordinates(mousemappos);
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
			foreach(Vertex v in General.Map.Data.Vertices)
			{
				// Adjust boundaries by vertices
				if(v.Position.x < left) left = v.Position.x;
				if(v.Position.x > right) right = v.Position.x;
				if(v.Position.y < top) top = v.Position.y;
				if(v.Position.y > bottom) bottom = v.Position.y;
			}

			// Calculate width/height
			width = (right - left);
			height = (bottom - top);

			// Calculate scale to view map at
			scalew = (float)General.Map.Graphics.RenderTarget.ClientSize.Width / (width * 1.1f);
			scaleh = (float)General.Map.Graphics.RenderTarget.ClientSize.Height / (height * 1.1f);
			if(scalew < scaleh) scale = scalew; else scale = scaleh;

			// Change the view to see the whole map
			renderer.ScaleView(scale);
			renderer.PositionView(left + (right - left) * 0.5f, top + (bottom - top) * 0.5f);
			General.Map.Data.Update();
			General.MainWindow.RedrawDisplay();
			
			// Determine new unprojected mouse coordinates
			mousemappos = renderer.GetMapCoordinates(mousepos);
			General.MainWindow.UpdateCoordinates(mousemappos);
		}
		
		#endregion
		
		#region ================== Mouse input

		// Mouse leaves the display
		public override void MouseLeave(EventArgs e)
		{
			// Mouse is outside the display
			mouseinside = false;
			mousepos = new Vector2D(float.NaN, float.NaN);
			mousemappos = mousepos;
			
			// Determine new unprojected mouse coordinates
			General.MainWindow.UpdateCoordinates(mousemappos);
			
			// Let the base class know
			base.MouseLeave(e);
		}

		// Mouse moved inside the display
		public override void MouseMove(MouseEventArgs e)
		{
			// Record last position
			mouseinside = true;
			mousepos = new Vector2D(e.X, e.Y);
			mousemappos = renderer.GetMapCoordinates(mousepos);
			
			// Update labels in main window
			General.MainWindow.UpdateCoordinates(mousemappos);
			
			// Let the base class know
			base.MouseMove(e);
		}

		#endregion
	}
}
