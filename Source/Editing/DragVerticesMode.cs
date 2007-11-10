
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


// This mode if for quickly dragging vertices without a layer.
// The geometry is merged and the mode returns to VerticesMode when the mouse is released.

namespace CodeImp.DoomBuilder.Editing
{
	public class DragVerticesMode : VerticesMode
	{
		#region ================== Constants

		#endregion

		#region ================== Variables

		// Mouse offset from dragitem
		protected Vector2D dragoffset;

		// Item used as reference for dragging
		protected Vertex dragitem;

		#endregion

		#region ================== Properties

		#endregion

		#region ================== Constructor / Disposer

		// Constructor to start dragging immediately
		public DragVerticesMode(Vertex dragitem, Vector2D dragoffset)
		{
			// Initialize
			this.dragitem = dragitem;
			this.dragoffset = dragoffset;

			// We have no destructor
			GC.SuppressFinalize(this);
		}

		// Diposer
		public override void Dispose()
		{
			// Not already disposed?
			if(!isdisposed)
			{
				// Clean up

				// Done
				isdisposed = true;
			}
		}

		#endregion

		#region ================== Methods

		// Cancelled
		public override void Cancel()
		{
			// Move geometry back to original position

			// Continue cancelling
			base.Cancel();
		}

		// Disenagaging
		public override void Disengage()
		{
			// When not cancelled
			if(!cancelled)
			{
				
				// TODO: Merge geometry
				

				// Map is changed
				General.Map.IsChanged = true;
			}
			
			// Continue disengage
			base.Disengage();
		}

		// Mouse button pressed
		public override void MouseDown(MouseEventArgs e)
		{
			// Do nothing.
		}

		// Mouse moving
		public override void MouseMove(MouseEventArgs e)
		{
			
			// TODO: Move selected geometry and redraw
			
		}

		// Mosue button released
		public override void MouseUp(MouseEventArgs e)
		{
			// Is the editing button released?
			if(e.Button == EditMode.EDIT_BUTTON)
			{
				// Just return to vertices mode, geometry will be merged on disengage.
				General.Map.ChangeMode(new VerticesMode());
			}
		}

		// When dragging starts
		protected override void DragStart(MouseEventArgs e)
		{
			// Do nothing. We're already dragging.
		}
		
		#endregion
	}
}
