
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

		// List of non-selected items
		private ICollection<Vertex> unselectedverts;

		// List of unstable lines
		private ICollection<Linedef> unstablelines;
		
		// Options
		private bool snaptogrid;		// SHIFT to toggle
		private bool snaptonearest;		// CTRL to enable

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

			Cursor.Current = Cursors.AppStarting;
			
			// Make old positions list
			// We will use this as reference to move the vertices, or to move them back on cancel
			oldpositions = new List<Vector2D>(General.Map.Selection.Vertices.Count);
			foreach(Vertex v in General.Map.Selection.Vertices) oldpositions.Add(v.Position);

			// Also keep old position of the dragged item
			dragitemposition = dragitem.Position;

			// Make list of non-selected vertices
			// This will be used for snapping to nearest items
			unselectedverts = General.Map.Map.InvertedCollection(General.Map.Selection.Vertices);
			
			// Make list of unstable lines only
			// These will have their length displayed during the drag
			unstablelines = General.Map.Map.LinedefsFromSelectedVertices(false, true);

			Cursor.Current = Cursors.Default;

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
		private bool MoveGeometryRelative(Vector2D offset, bool snapgrid, bool snapnearest)
		{
			Vector2D oldpos = dragitem.Position;
			Vertex nearest;
			int i = 0;
			
			// Snap to nearest?
			if(snapnearest)
			{
				// Find nearest unselected item within selection range
				nearest = MapSet.NearestVertexSquareRange(unselectedverts, mousemappos, VerticesMode.VERTEX_HIGHLIGHT_RANGE / renderer.Scale);
				if(nearest != null)
				{
					// Move the dragged item
					dragitem.Move(nearest.Position);

					// Adjust the offset
					offset = nearest.Position - dragitemposition;

					// Do not snap to grid!
					snapgrid = false;
				}
			}

			// Snap to grid?
			if(snapgrid)
			{
				// Move the dragged item
				dragitem.Move(dragitemposition + offset);

				// Snap item to grid
				dragitem.SnapToGrid();

				// Adjust the offset
				offset += dragitem.Position - (dragitemposition + offset);
			}

			// Drag item moved?
			if(!snapgrid || (dragitem.Position != oldpos))
			{
				// Move selected geometry
				foreach(Vertex v in General.Map.Selection.Vertices)
				{
					// Move vertex from old position relative to the
					// mouse position change since drag start
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
			MoveGeometryRelative(new Vector2D(0f, 0f), false, false);

			// If only a single vertex was selected, deselect it now
			if(General.Map.Selection.Vertices.Count == 1) General.Map.Selection.ClearVertices();
			
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
			ICollection<Linedef> movinglines;
			ICollection<Linedef> fixedlines;
			int stitches = 0;
			int stitchundo;
			
			base.Disengage();
			Cursor.Current = Cursors.WaitCursor;
			
			// When not cancelled
			if(!cancelled)
			{
				// Move geometry back to original position
				MoveGeometryRelative(new Vector2D(0f, 0f), false, false);

				// Make undo for the dragging
				General.Map.UndoRedo.CreateUndo("drag vertices", UndoGroup.None, 0, false);

				// Move selected geometry to final position
				MoveGeometryRelative(mousemappos - dragstartmappos, snaptogrid, snaptonearest);

				// ===== BEGIN GEOMETRY STITCHING
				
				// Make undo for the stitching
				stitchundo = General.Map.UndoRedo.CreateUndo("stitch geometry", UndoGroup.None, 0, false);

				// Find lines that moved during the drag
				movinglines = General.Map.Map.LinedefsFromSelectedVertices(true, true);

				// Find all non-moving lines (inverse of movinglines)
				fixedlines = General.Map.Map.InvertedCollection(movinglines);

				// Join nearby vertices
				stitches += MapSet.JoinVertices(unselectedverts, General.Map.Selection.Vertices, true, General.Settings.StitchDistance);

				// Update cached values
				General.Map.Map.Update();
				
				// Split moving lines with unselected vertices
				stitches += MapSet.SplitLinesByVertices(movinglines, unselectedverts, General.Settings.StitchDistance);
				
				// Split non-moving lines with selected vertices
				stitches += MapSet.SplitLinesByVertices(fixedlines, General.Map.Selection.Vertices, General.Settings.StitchDistance);
				

				// TODO: Join overlapping lines and remove looped lines


				// No stitching done? then withdraw undo
				if(stitches == 0) General.Map.UndoRedo.WithdrawUndo(stitchundo);

				// ===== END GEOMETRY STITCHING

				// If only a single vertex was selected, deselect it now
				if(General.Map.Selection.Vertices.Count == 1) General.Map.Selection.ClearVertices();
				
				// Update cached values
				General.Map.Map.Update();

				// Map is changed
				General.Map.IsChanged = true;
			}

			// Hide highlight info
			General.MainWindow.HideInfo();

			// Uncheck vertices button on main window
			General.MainWindow.SetVerticesChecked(false);
			Cursor.Current = Cursors.Default;
		}

		// This redraws the display
		public unsafe override void RedrawDisplay()
		{
			// Start rendering
			if(renderer.Start(true, false))
			{
				// Render lines and vertices
				renderer.RenderLinedefSet(General.Map.Map.Linedefs);
				renderer.RenderVerticesSet(General.Map.Map.Vertices);

				// Draw the dragged item highlighted
				// This is important to know, because this item is used
				// for snapping to the grid and snapping to nearest items
				renderer.RenderVertex(dragitem, ColorCollection.HIGHLIGHT);
				
				// Done
				renderer.Finish();
			}
		}

		// This updates the dragging
		private void Update()
		{
			snaptogrid = General.MainWindow.ShiftState ^ General.MainWindow.SnapToGrid;
			snaptonearest = General.MainWindow.CtrlState;
			
			// Move selected geometry
			if(MoveGeometryRelative(mousemappos - dragstartmappos, snaptogrid, snaptonearest))
			{
				// Update cached values
				General.Map.Map.Update();

				// Redraw
				General.MainWindow.RedrawDisplay();
			}
		}

		// Mouse moving
		public override void MouseMove(MouseEventArgs e)
		{
			base.MouseMove(e);
			Update();
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

		// When a key is released
		public override void KeyUp(KeyEventArgs e)
		{
			base.KeyUp(e);
			if(snaptogrid != General.MainWindow.ShiftState ^ General.MainWindow.SnapToGrid) Update();
			if(snaptonearest != General.MainWindow.CtrlState) Update();
		}

		// When a key is pressed
		public override void KeyDown(KeyEventArgs e)
		{
			base.KeyDown(e);
			if(snaptogrid != General.MainWindow.ShiftState ^ General.MainWindow.SnapToGrid) Update();
			if(snaptonearest != General.MainWindow.CtrlState) Update();
		}
		
		#endregion
	}
}
