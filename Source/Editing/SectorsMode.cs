
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

#endregion

namespace CodeImp.DoomBuilder.Editing
{
	internal class SectorsMode : ClassicMode
	{
		#region ================== Constants

		#endregion

		#region ================== Variables

		// Highlighted item
		private Sector highlighted;

		#endregion

		#region ================== Properties

		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		public SectorsMode()
		{
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

		#region ================== Methods

		// Mode engages
		public override void Engage()
		{
			base.Engage();

			// Check sectors button on main window
			General.MainWindow.SetSectorsChecked(true);
		}

		// Mode disengages
		public override void Disengage()
		{
			base.Disengage();

			// Check sectors button on main window
			General.MainWindow.SetSectorsChecked(false);
		}

		// This redraws the display
		public unsafe override void RedrawDisplay()
		{
			// Start with a clear display
			if(renderer.StartRendering(true))
			{
				// Render things
				renderer.SetThingsRenderOrder(false);
				renderer.RenderThingSet(General.Map.Map.Things);
				
				// Render lines and vertices
				renderer.RenderLinedefSet(General.Map.Map.Linedefs);
				renderer.RenderVerticesSet(General.Map.Map.Vertices);

				// Render highlighted item
				if(highlighted != null)
					renderer.RenderSector(highlighted, General.Colors.Highlight);

				// Done
				renderer.FinishRendering();
			}
		}

		// This highlights a new item
		protected void Highlight(Sector s)
		{
			// Update display
			if(renderer.StartRendering(false))
			{
				// Undraw previous highlight
				if(highlighted != null)
					renderer.RenderSector(highlighted);

				// Set new highlight
				highlighted = s;

				// Render highlighted item
				if(highlighted != null)
					renderer.RenderSector(highlighted, General.Colors.Highlight);

				// Done
				renderer.FinishRendering();
			}
		}
		
		// Mouse moves
		public override void MouseMove(MouseEventArgs e)
		{
			base.MouseMove(e);

			// Find the nearest linedef within highlight range
			Linedef l = General.Map.Map.NearestLinedef(mousemappos);

			// Check on which side of the linedef the mouse is
			float side = l.SideOfLine(mousemappos);
			if(side > 0)
			{
				// Is there a sidedef here?
				if(l.Back != null)
				{
					// Highlight if not the same
					if(l.Back.Sector != highlighted) Highlight(l.Back.Sector);
				}
				else
				{
					// Highlight nothing
					if(highlighted != null) Highlight(null);
				}
			}
			else
			{
				// Is there a sidedef here?
				if(l.Front != null)
				{
					// Highlight if not the same
					if(l.Front.Sector != highlighted) Highlight(l.Front.Sector);
				}
				else
				{
					// Highlight nothing
					if(highlighted != null) Highlight(null);
				}
			}
		}

		// Mouse leaves
		public override void MouseLeave(EventArgs e)
		{
			base.MouseLeave(e);

			// Highlight nothing
			Highlight(null);
		}

		#endregion
	}
}
