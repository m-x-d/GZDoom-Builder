
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
	public class DragVerticesMode : ClassicMode
	{
		#region ================== Constants

		#endregion

		#region ================== Variables

		// Mouse position on map where dragging started
		protected Vector2D dragstartmappos;

		// Item used as reference for snapping to the grid
		protected Vertex dragitem;

		// List of old vertex positions
		protected List<Vector2D> oldpositions;

		#endregion

		#region ================== Properties

		#endregion

		#region ================== Constructor / Disposer

		// Constructor to start dragging immediately
		public DragVerticesMode(Vertex dragitem, Vector2D dragstartmappos)
		{
			// Initialize
			this.dragitem = dragitem;
			this.dragstartmappos = dragstartmappos;

			// Make old positions list
			// We will use this as reference to move the vertices, or to move them back on cancel
			oldpositions = new List<Vector2D>(General.Map.Selection.Vertices.Count);
			foreach(Vertex v in General.Map.Selection.Vertices) oldpositions.Add(v.Position);
			
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

		// This moves the selected geometry relatively
		private void MoveGeometryRelative(Vector2D offset)
		{
			int i = 0;
			
			// Move selected geometry
			foreach(Vertex v in General.Map.Selection.Vertices)
			{
				// Move vertex from old position relative to the mouse position change since drag start
				v.Move(oldpositions[i] + offset);

				// Next
				i++;
			}
		}
		
		// Cancelled
		public override void Cancel()
		{
			// Move geometry back to original position
			MoveGeometryRelative(new Vector2D(0f, 0f));

			// Update cached values
			General.Map.Map.Update();
			
			// Cancel base class
			base.Cancel();
			
			// Return to vertices mode
			General.Map.ChangeMode(new VerticesMode());
		}

		// Mode engages
		public override void Engage()
		{
			base.Engage();

			// Check vertices button on main window
			General.MainWindow.SetVerticesChecked(true);
		}
		
		// Disenagaging
		public override void Disengage()
		{
			base.Disengage();
			
			// When not cancelled
			if(!cancelled)
			{
				// Move geometry back to original position
				MoveGeometryRelative(new Vector2D(0f, 0f));

				// Make undo
				General.Map.UndoRedo.CreateUndo("drag vertices", UndoGroup.None, 0, false);

				// Move selected geometry to final position
				MoveGeometryRelative(mousemappos - dragstartmappos);

				
				// TODO: Merge geometry


				// Update cached values
				General.Map.Map.Update();

				// Map is changed
				General.Map.IsChanged = true;
			}

			// Hide highlight info
			General.MainWindow.HideInfo();

			// Uncheck vertices button on main window
			General.MainWindow.SetVerticesChecked(false);
		}

		// This redraws the display
		public unsafe override void RedrawDisplay()
		{
			// Start with a clear display
			if(renderer.StartRendering(true, true))
			{
				// Render things
				renderer.SetThingsRenderOrder(false);
				renderer.RenderThingSet(General.Map.Map.Things);

				// Render lines and vertices
				renderer.RenderLinedefSet(General.Map.Map.Linedefs);
				renderer.RenderVerticesSet(General.Map.Map.Vertices);

				// Done
				renderer.FinishRendering();
			}
		}

		// Mouse moving
		public override void MouseMove(MouseEventArgs e)
		{
			base.MouseMove(e);

			// Move selected geometry
			MoveGeometryRelative(mousemappos - dragstartmappos);

			// Update cached values
			General.Map.Map.Update();
			
			// Redraw
			General.MainWindow.RedrawDisplay();
		}

		// Mosue button released
		public override void MouseUp(MouseEventArgs e)
		{
			base.MouseUp(e);
			
			// Is the editing button released?
			if(e.Button == EditMode.EDIT_BUTTON)
			{
				// Just return to vertices mode, geometry will be merged on disengage.
				General.Map.ChangeMode(new VerticesMode());
			}
		}
		
		#endregion
	}
}
