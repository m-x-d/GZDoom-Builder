
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
using System.Collections.Generic;
using System.Windows.Forms;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Rendering;
using CodeImp.DoomBuilder.Geometry;
using System.Drawing;
using CodeImp.DoomBuilder.Editing;

#endregion

namespace CodeImp.DoomBuilder.BuilderModes
{
	// No action or button for this mode, it is automatic.
	// The EditMode attribute does not have to be specified unless the
	// mode must be activated by class name rather than direct instance.
	// In that case, just specifying the attribute like this is enough:
	// [EditMode]

	[EditMode(DisplayName = "Drag Things",
			  AllowCopyPaste = false,
			  Volatile = true)]

	public sealed class DragThingsMode : BaseClassicMode
	{
		#region ================== Constants

		#endregion

		#region ================== Variables

		// Mode to return to
		private readonly EditMode basemode;
		
		// Mouse position on map where dragging started
		private readonly Vector2D dragstartmappos;

		//mxd. Offset from nearest grid intersection to dragstartmappos
		private Vector2D dragstartoffset;

		// Item used as reference for snapping to the grid
		private readonly Thing dragitem;
		private readonly Vector2D dragitemposition;

		// List of old thing positions
		private readonly List<Vector2D> oldpositions;

		//mxd
		private class AlignData
		{
			public int InitialAngle;
			public int CurrentAngle;
			public float InitialHeight;
			public float CurrentHeight;
			public PointF Position = PointF.Empty;
			public bool Active;

			public AlignData(Thing t){
				InitialAngle = t.AngleDoom;
				InitialHeight = t.Position.z;
			}
		}

		private AlignData alignData;

		// List of selected items
		private readonly ICollection<Thing> selectedthings;

		// List of non-selected items
		private readonly ICollection<Thing> unselectedthings;
		
		// Keep track of view changes
		private float lastoffsetx;
		private float lastoffsety;
		private float lastscale;
		
		// Options
		private bool snaptogrid;		// SHIFT to toggle
		private bool snaptonearest;		// CTRL to enable
		private bool snaptogridincrement; //mxd. ALT to toggle

		#endregion

		#region ================== Properties

		// Just keep the base mode button checked
		public override string EditModeButtonName { get { return basemode.GetType().Name; } }

		//internal EditMode BaseMode { get { return basemode; } }
		
		#endregion

		#region ================== Constructor / Disposer

		// Constructor to start dragging immediately
		public DragThingsMode(EditMode basemode, Vector2D dragstartmappos)
		{
			// Initialize
			this.dragstartmappos = dragstartmappos;
			this.basemode = basemode;

			Cursor.Current = Cursors.AppStarting;

			// Mark what we are dragging
			General.Map.Map.ClearAllMarks(false);
			General.Map.Map.MarkSelectedThings(true, true);
			
			// Get selected things
			selectedthings = General.Map.Map.GetMarkedThings(true);
			unselectedthings = new List<Thing>();
			foreach(Thing t in General.Map.ThingsFilter.VisibleThings) if(!t.Marked) unselectedthings.Add(t);
			
			// Get the nearest thing for snapping
			dragitem = MapSet.NearestThing(selectedthings, dragstartmappos);

			// Make old positions list
			// We will use this as reference to move the vertices, or to move them back on cancel
			oldpositions = new List<Vector2D>(selectedthings.Count);
			foreach(Thing t in selectedthings) oldpositions.Add(t.Position);

			// Also keep old position of the dragged item
			dragitemposition = dragitem.Position;

			//mxd. Get drag offset
			dragstartoffset = dragitemposition - General.Map.Grid.SnappedToGrid(dragitem.Position);

			// Keep view information
			lastoffsetx = renderer.OffsetX;
			lastoffsety = renderer.OffsetY;
			lastscale = renderer.Scale;
			
			Cursor.Current = Cursors.Default;
			
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
		
		// This moves the selected things relatively
		// Returns true when things has actually moved
		private bool MoveThingsRelative(Vector2D offset, bool snapgrid, bool snapgridincrement, bool snapnearest)
		{
			Vector2D oldpos = dragitem.Position;
			Thing nearest;
			Vector2D tl, br;

			// don't move if the offset contains invalid data
			if (!offset.IsFinite())	return false;

			// Find the outmost things
			tl = br = oldpositions[0];
			for (int i = 0; i < oldpositions.Count; i++)
			{
				if (oldpositions[i].x < tl.x) tl.x = (int)oldpositions[i].x;
				if (oldpositions[i].x > br.x) br.x = (int)oldpositions[i].x;
				if (oldpositions[i].y > tl.y) tl.y = (int)oldpositions[i].y;
				if (oldpositions[i].y < br.y) br.y = (int)oldpositions[i].y;
			}

			// Snap to nearest?
			if(snapnearest)
			{
				// Find nearest unselected item within selection range
				nearest = MapSet.NearestThingSquareRange(unselectedthings, mousemappos, BuilderPlug.Me.StitchRange / renderer.Scale);
				if(nearest != null)
				{
					// Move the dragged item
					dragitem.Move((Vector2D)nearest.Position);

					// Adjust the offset
					offset = (Vector2D)nearest.Position - dragitemposition;

					// Do not snap to grid!
					snapgrid = false;
					snapgridincrement = false; //mxd
				}
			}

			// Snap to grid?
			if(snapgrid || snapgridincrement)
			{
				// Move the dragged item
				dragitem.Move(dragitemposition + offset);

				// Snap item to grid increment
				if(snapgridincrement) //mxd
				{
					dragitem.Move(General.Map.Grid.SnappedToGrid(dragitemposition + offset + dragstartoffset) - dragstartoffset);
				} 
				else // Or to the grid itself
				{
					dragitem.SnapToGrid();
				}

				// Adjust the offset
				offset += (Vector2D)dragitem.Position - (dragitemposition + offset);
			}

			// Make sure the offset is inside the map boundaries
			if (offset.x + tl.x < General.Map.Config.LeftBoundary) offset.x = General.Map.Config.LeftBoundary - tl.x;
			if (offset.x + br.x > General.Map.Config.RightBoundary) offset.x = General.Map.Config.RightBoundary - br.x;
			if (offset.y + tl.y > General.Map.Config.TopBoundary) offset.y = General.Map.Config.TopBoundary - tl.y;
			if (offset.y + br.y < General.Map.Config.BottomBoundary) offset.y = General.Map.Config.BottomBoundary - br.y;

			// Drag item moved?
			if((!snapgrid && !snapgridincrement) || ((Vector2D)dragitem.Position != oldpos))
			{
				int i = 0;

				// Move selected geometry
				foreach(Thing t in selectedthings)
				{
					// Move vertex from old position relative to the
					// mouse position change since drag start
					t.Move(oldpositions[i] + offset);

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

		// This redraws the display
		public override void OnRedrawDisplay()
		{
			if(CheckViewChanged())
			{
				renderer.RedrawSurface();

				// Render lines and vertices
				if(renderer.StartPlotter(true))
				{
					renderer.PlotLinedefSet(General.Map.Map.Linedefs);
					renderer.PlotVerticesSet(General.Map.Map.Vertices);
					renderer.Finish();
				}
			}
			
			// Render things
			UpdateRedraw();

			renderer.Present();
		}

		// This redraws only changed things
		private void UpdateRedraw()
		{
			//mxd. Added, so grid can be rendered properly if the user changes grid size while dragging (very useful and important, I know)
			if (renderer.StartPlotter(true)) 
			{
				// Render lines and vertices
				renderer.PlotLinedefSet(General.Map.Map.Linedefs);
				renderer.PlotVerticesSet(General.Map.Map.Vertices);
				
				// Done
				renderer.Finish();
			}

			// Render things
			if(renderer.StartThings(true))
			{
				// Render things
				renderer.RenderThingSet(General.Map.ThingsFilter.HiddenThings, Presentation.THINGS_HIDDEN_ALPHA);
				renderer.RenderThingSet(unselectedthings, 1.0f);
				renderer.RenderThingSet(selectedthings, 1.0f);

				// Draw the dragged item highlighted
				// This is important to know, because this item is used
				// for snapping to the grid and snapping to nearest items
				renderer.RenderThing(dragitem, General.Colors.Highlight, 1.0f);

				// Done
				renderer.Finish();
			}
		}
		
		// Cancelled
		public override void OnCancel()
		{
			// Move geometry back to original position
			MoveThingsRelative(new Vector2D(0f, 0f), false, false, false);

			// If only a single vertex was selected, deselect it now
			if(selectedthings.Count == 1) General.Map.Map.ClearSelectedThings();
			
			// Update cached values
			General.Map.Map.Update();
			
			// Cancel base class
			base.OnCancel();
			
			// Return to vertices mode
			General.Editing.ChangeMode(basemode);
		}

		// Mode engages
		public override void OnEngage()
		{
			base.OnEngage();
			renderer.SetPresentation(Presentation.Things);
		}
		
		// Disenagaging
		public override void OnDisengage()
		{
			base.OnDisengage();
			Cursor.Current = Cursors.AppStarting;
			
			// When not cancelled
			if(!cancelled)
			{
				// Move geometry back to original position
				MoveThingsRelative(new Vector2D(0f, 0f), false, false, false);

				if(alignData != null && alignData.Active){
					alignData.CurrentAngle = dragitem.AngleDoom; //mxd
					dragitem.Rotate(alignData.InitialAngle);
					alignData.CurrentHeight = dragitem.Position.z; //mxd
					dragitem.Move(dragitem.Position.x, dragitem.Position.y, alignData.InitialHeight);
				}

				// Make undo for the dragging
				General.Map.UndoRedo.CreateUndo("Drag things");

				// Move selected geometry to final position
				if(alignData != null && alignData.Active){//mxd
					if(!alignData.Position.IsEmpty) 
						dragitem.Move(alignData.Position.X, alignData.Position.Y, alignData.CurrentHeight);
					else
						dragitem.Move(dragitem.Position.x, dragitem.Position.y, alignData.CurrentHeight);
					dragitem.Rotate(alignData.CurrentAngle);
				} else {
					MoveThingsRelative(mousemappos - dragstartmappos, snaptogrid, snaptogridincrement, snaptonearest);
				}

				// Snap to map format accuracy
				General.Map.Map.SnapAllToAccuracy();
				
				// Update cached values
				General.Map.Map.Update(false, false);

				// Map is changed
				General.Map.IsChanged = true;
			}

			// Hide highlight info
			General.Interface.HideInfo();

			// Done
			Cursor.Current = Cursors.Default;
		}

		// This checks if the view offset/zoom changed and updates the check
		private bool CheckViewChanged()
		{
			// View changed?
			bool viewchanged = (renderer.OffsetX != lastoffsetx || renderer.OffsetY != lastoffsety || renderer.Scale != lastscale);

			// Keep view information
			lastoffsetx = renderer.OffsetX;
			lastoffsety = renderer.OffsetY;
			lastscale = renderer.Scale;

			// Return result
			return viewchanged;
		}

		// This updates the dragging
		private void Update()
		{
			snaptogrid = General.Interface.ShiftState ^ General.Interface.SnapToGrid;
			snaptonearest = General.Interface.CtrlState;
			snaptogridincrement = General.Interface.AltState; //mxd

			//mxd. Snap to nearest linedef
			if(selectedthings.Count == 1 && dragitem.IsModel && snaptonearest && MoveThingsRelative(mousemappos - dragstartmappos, snaptogrid, snaptogridincrement, false)) {
				Linedef l = General.Map.Map.NearestLinedefRange(oldpositions[0] + mousemappos - dragstartmappos, BuilderPlug.Me.StitchRange / renderer.Scale);
				bool restoreSettings = false;

				if(alignData == null)
					alignData = new AlignData(dragitem);

				if(l != null) {
					if(Tools.TryAlignThingToLine(dragitem, l)) {
						dragitem.SnapToAccuracy();
						alignData.Position = new PointF(dragitem.Position.x, dragitem.Position.y);
						alignData.Active = true;
					} else if(dragitem.AngleDoom != alignData.InitialAngle) { //restore initial angle?
						restoreSettings = true;
					}

				} else if(dragitem.AngleDoom != alignData.InitialAngle) { //restore initial angle?
					restoreSettings = true;
				}

				if(restoreSettings) {
					alignData.Position = PointF.Empty;
					alignData.Active = false;
					dragitem.Rotate(alignData.InitialAngle);
				}

				General.Map.Map.Update(); // Update cached values
				UpdateRedraw();// Redraw
				renderer.Present();

				return;
			}
			
			// Move selected geometry
			if(MoveThingsRelative(mousemappos - dragstartmappos, snaptogrid, snaptogridincrement, snaptonearest))
			{
				// Update cached values
				General.Map.Map.Update();

				// Redraw
				UpdateRedraw();
				renderer.Present();
			}
		}

		// When edit button is released
		protected override void OnEditEnd()
		{
			// Just return to vertices mode, geometry will be merged on disengage.
			General.Editing.ChangeMode(basemode);

			base.OnEditEnd();
		}

		// Mouse moving
		public override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);
			Update();
		}

		// When a key is released
		public override void OnKeyUp(KeyEventArgs e)
		{
			base.OnKeyUp(e);
			if(snaptogrid != General.Interface.ShiftState ^ General.Interface.SnapToGrid ||
				snaptonearest != General.Interface.CtrlState || 
				snaptogridincrement != General.Interface.AltState) Update();
		}

		// When a key is pressed
		public override void OnKeyDown(KeyEventArgs e)
		{
			base.OnKeyDown(e);
			if(snaptogrid != General.Interface.ShiftState ^ General.Interface.SnapToGrid ||
				snaptonearest != General.Interface.CtrlState ||
				snaptogridincrement != General.Interface.AltState) Update();
		}
		
		#endregion
	}
}
