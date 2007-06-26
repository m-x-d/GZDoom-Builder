
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
		
		#endregion

		#region ================== Properties
		
		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		public ViewClassicMode()
		{
			// Initialize
			this.renderer = graphics.Renderer2D;
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
		public void ScrollNorth()
		{
			// Scroll
			ScrollBy(0f, 100f / renderer.Scale);
		}

		// This scrolls the view south
		[Action(Action.SCROLLSOUTH)]
		public void ScrollSouth()
		{
			// Scroll
			ScrollBy(0f, -100f / renderer.Scale);
		}

		// This scrolls the view west
		[Action(Action.SCROLLWEST)]
		public void ScrollWest()
		{
			// Scroll
			ScrollBy(-100f / renderer.Scale, 0f);
		}

		// This scrolls the view east
		[Action(Action.SCROLLEAST)]
		public void ScrollEast()
		{
			// Scroll
			ScrollBy(100f / renderer.Scale, 0f);
		}

		// This zooms in
		[Action(Action.ZOOMIN)]
		public void ZoomIn()
		{
			// Zoom
			ZoomBy(0.1f);
		}

		// This zooms out
		[Action(Action.ZOOMOUT)]
		public void ZoomOut()
		{
			// Zoom
			ZoomBy(-0.1f);
		}

		// This scrolls anywhere
		private void ScrollBy(float deltax, float deltay)
		{
			// Scroll now
			renderer.PositionView(renderer.OffsetX + deltax, renderer.OffsetY + deltay);
			RedrawDisplay();
		}

		// This zooms
		private void ZoomBy(float deltaz)
		{
			// Zoom now
			renderer.ScaleView(renderer.Scale + deltaz);
			RedrawDisplay();
		}
		
		#endregion
	}
}
