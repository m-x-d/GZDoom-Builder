
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
		private Vector2D dragstartmappos;

		// Item used as reference for snapping to the grid
		private Vertex dragitem;
		private Vector2D dragitemposition;

		// List of old vertex positions
		private List<Vector2D> oldpositions;

		// Options
		private bool snaptogrid;

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

			// Also keep old position of the dragged item
			dragitemposition = dragitem.Position;
			
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
		// Returns true when geometry has actually moved
		private bool MoveGeometryRelative(Vector2D offset, bool snap)
		{
			Vector2D oldpos = dragitem.Position;
			int i = 0;
			
			// Snap to grid?
			if(snap)
			{
				// Move the dragged item
				dragitem.Move(dragitemposition + offset);

				// Snap item to grid
				dragitem.SnapToGrid();

				// Adjust the offset
				offset += dragitem.Position - (dragitemposition + offset);
			}

			// Drag item moved?
			if(!snap || (dragitem.Position != oldpos))
			{
				// Move selected geometry
				foreach(Vertex v in General.Map.Selection.Vertices)
				{
					// Move vertex from old position relative to the mouse position change since drag start
					v.Move(oldpositions[i] + offset);

					// Next
					i++;
				}

				// Moved
				return true;
			}
			else
			{
				// No changes
				return false;
			}
		}
		
		// Cancelled
		public override void Cancel()
		{
			// Move geometry back to original position
			MoveGeometryRelative(new Vector2D(0f, 0f), false);

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
				MoveGeometryRelative(new Vector2D(0f, 0f), false);

				// Make undo
				General.Map.UndoRedo.CreateUndo("drag vertices", UndoGroup.None, 0, false);

				// Move selected geometry to final position
				MoveGeometryRelative(mousemappos - dragstartmappos, snaptogrid);

				
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
			snaptogrid = !General.MainWindow.ShiftState;
			
			// Move selected geometry
			if(MoveGeometryRelative(mousemappos - dragstartmappos, snaptogrid))
			{
				// Update cached values
				General.Map.Map.Update();

				// Redraw
				General.MainWindow.RedrawDisplay();
			}
		}

		// Mouse button released
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
