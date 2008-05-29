
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
using CodeImp.DoomBuilder.Geometry;
using CodeImp.DoomBuilder.Editing;
using System.Drawing;
using CodeImp.DoomBuilder.Actions;

#endregion

namespace CodeImp.DoomBuilder.BuilderModes
{
	[EditMode(SwitchAction = "sectorsmode",		// Action name used to switch to this mode
			  ButtonDesc = "Sectors Mode",		// Description on the button in toolbar/menu
		      ButtonImage = "SectorsMode.png",	// Image resource name for the button
			  ButtonOrder = int.MinValue + 2)]	// Position of the button (lower is more to the left)

	public class SectorsMode : ClassicMode
	{
		#region ================== Constants

		#endregion

		#region ================== Variables

		// Highlighted item
		protected Sector highlighted;

		// Interface
		protected bool editpressed;

		// The methods GetSelected* and MarkSelected* on the MapSet do not
		// retain the order in which items were selected.
		// This list keeps in order while sectors are selected/deselected.
		protected List<Sector> orderedselection;
		
		#endregion

		#region ================== Properties

		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		public SectorsMode()
		{
			// Make ordered selection list
			orderedselection = new List<Sector>();

			// Fill the list with selected sectors (these are not in order, but we have no other choice)
			ICollection<Sector> selectedsectors = General.Map.Map.GetSelectedSectors(true);
			foreach(Sector s in selectedsectors) orderedselection.Add(s);
		}

		// Disposer
		public override void Dispose()
		{
			// Not already disposed?
			if(!isdisposed)
			{
				// Clean up
				orderedselection = null;
				
				// Dispose base
				base.Dispose();
			}
		}

		#endregion

		#region ================== Methods

		// When undo is used
		[EndAction("undo", BaseAction = true)]
		public void Undo()
		{
			// Clear ordered selection
			orderedselection.Clear();
		}

		// When redo is used
		[EndAction("redo", BaseAction = true)]
		public void Redo()
		{
			// Clear ordered selection
			orderedselection.Clear();
		}

		// This selectes or deselects a sector
		protected void SelectSector(Sector s, bool selectstate)
		{
			bool selectionchanged = false;
			
			// Select the sector?
			if(selectstate && !s.Selected)
			{
				orderedselection.Add(s);
				s.Selected = true;
				selectionchanged = true;
			}
			// Deselect the sector?
			else if(!selectstate && s.Selected)
			{
				orderedselection.Remove(s);
				s.Selected = false;
				selectionchanged = true;
			}

			// Selection changed?
			if(selectionchanged)
			{
				// Make update lines selection
				foreach(Sidedef sd in s.Sidedefs)
				{
					bool front, back;
					if(sd.Line.Front != null) front = sd.Line.Front.Sector.Selected; else front = false;
					if(sd.Line.Back != null) back = sd.Line.Back.Sector.Selected; else back = false;
					sd.Line.Selected = front | back;
				}
			}
		}
		
		// Cancel mode
		public override void OnCancel()
		{
			base.OnCancel();

			// Return to this mode
			General.Map.ChangeMode(new SectorsMode());
		}

		// Mode engages
		public override void OnEngage()
		{
			base.OnEngage();
			renderer.SetPresentation(Presentation.Standard);
		}

		// Mode disengages
		public override void OnDisengage()
		{
			base.OnDisengage();

			// Check which mode we are switching to
			if(General.Map.NewMode is VerticesMode)
			{
				// Convert selection to vertices

				// Clear selected sectors
				General.Map.Map.ClearSelectedSectors();
			}
			else if(General.Map.NewMode is LinedefsMode)
			{
				// Convert selection to linedefs

				// Clear selected sectors
				General.Map.Map.ClearSelectedSectors();
			}
			
			// Hide highlight info
			General.Interface.HideInfo();
		}

		// This redraws the display
		public override void OnRedrawDisplay()
		{
			// Render lines and vertices
			if(renderer.StartPlotter(true))
			{
				renderer.PlotLinedefSet(General.Map.Map.Linedefs);
				renderer.PlotVerticesSet(General.Map.Map.Vertices);
				if((highlighted != null) && !highlighted.IsDisposed)
					renderer.PlotSector(highlighted, General.Colors.Highlight);
				renderer.Finish();
			}

			// Render things
			if(renderer.StartThings(true))
			{
				renderer.RenderThingSet(General.Map.ThingsFilter.HiddenThings, Presentation.THINGS_HIDDEN_ALPHA);
				renderer.RenderThingSet(General.Map.ThingsFilter.VisibleThings, 1.0f);
				renderer.Finish();
			}

			// Selecting?
			if(selecting)
			{
				// Render selection
				if(renderer.StartOverlay(true))
				{
					RenderMultiSelection();
					renderer.Finish();
				}
			}

			renderer.Present();
		}

		// This highlights a new item
		protected void Highlight(Sector s)
		{
			// Update display
			if(renderer.StartPlotter(false))
			{
				// Undraw previous highlight
				if((highlighted != null) && !highlighted.IsDisposed)
					renderer.PlotSector(highlighted);

				/*
				// Undraw highlighted things
				if(highlighted != null)
					foreach(Thing t in highlighted.Things)
						renderer.RenderThing(t, renderer.DetermineThingColor(t));
				*/
				
				// Set new highlight
				highlighted = s;

				// Render highlighted item
				if((highlighted != null) && !highlighted.IsDisposed)
					renderer.PlotSector(highlighted, General.Colors.Highlight);

				/*
				// Render highlighted things
				if(highlighted != null)
					foreach(Thing t in highlighted.Things)
						renderer.RenderThing(t, General.Colors.Highlight);
				*/
				
				// Done
				renderer.Finish();
				renderer.Present();
			}

			// Show highlight info
			if((highlighted != null) && !highlighted.IsDisposed)
				General.Interface.ShowSectorInfo(highlighted);
			else
				General.Interface.HideInfo();
		}

		// Selection
		protected override void OnSelect()
		{
			// Item highlighted?
			if((highlighted != null) && !highlighted.IsDisposed)
			{
				// Flip selection
				SelectSector(highlighted, !highlighted.Selected);

				// Update display
				if(renderer.StartPlotter(false))
				{
					// Redraw highlight to show selection
					renderer.PlotSector(highlighted);
					renderer.Finish();
					renderer.Present();
				}
			}
			else
			{
				// Start making a selection
				StartMultiSelection();
			}

			base.OnSelect();
		}

		// End selection
		protected override void OnEndSelect()
		{
			// Not stopping from multiselection?
			if(!selecting)
			{
				// Item highlighted?
				if((highlighted != null) && !highlighted.IsDisposed)
				{
					// Update display
					if(renderer.StartPlotter(false))
					{
						// Render highlighted item
						renderer.PlotSector(highlighted, General.Colors.Highlight);
						renderer.Finish();
						renderer.Present();
					}
				}
			}

			base.OnEndSelect();
		}

		// Start editing
		protected override void OnEdit()
		{
			// Item highlighted?
			if((highlighted != null) && !highlighted.IsDisposed)
			{
				// Edit pressed in this mode
				editpressed = true;

				// Highlighted item not selected?
				if(!highlighted.Selected)
				{
					// Make this the only selection
					General.Map.Map.ClearSelectedSectors();
					General.Map.Map.ClearSelectedLinedefs();
					SelectSector(highlighted, true);
					General.Interface.RedrawDisplay();
				}

				// Update display
				if(renderer.StartPlotter(false))
				{
					// Redraw highlight to show selection
					renderer.PlotSector(highlighted);
					renderer.Finish();
					renderer.Present();
				}
			}
			else
			{
				// Start drawing mode
				DrawGeometryMode drawmode = new DrawGeometryMode();
				drawmode.DrawPointAt(mousemappos, true);
				General.Map.ChangeMode(drawmode);
			}
			
			base.OnEdit();
		}

		// Done editing
		protected override void OnEndEdit()
		{
			// Edit pressed in this mode?
			if(editpressed)
			{
				// Anything selected?
				ICollection<Sector> selected = General.Map.Map.GetSelectedSectors(true);
				if(selected.Count > 0)
				{
					// Show sector edit dialog
					General.Interface.ShowEditSectors(selected);

					// When a single sector was selected, deselect it now
					if(selected.Count == 1)
					{
						General.Map.Map.ClearSelectedSectors();
						General.Map.Map.ClearSelectedLinedefs();
					}

					// Update entire display
					General.Interface.RedrawDisplay();
				}
			}

			editpressed = false;
			base.OnEndEdit();
		}
		
		// Mouse moves
		public override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);

			// Not holding any buttons?
			if(e.Button == MouseButtons.None)
			{
				// Find the nearest linedef within highlight range
				Linedef l = General.Map.Map.NearestLinedef(mousemappos);
				if(l != null)
				{
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
				else
				{
					// Highlight nothing
					if(highlighted != null) Highlight(null);
				}
			}
		}

		// Mouse leaves
		public override void OnMouseLeave(EventArgs e)
		{
			base.OnMouseLeave(e);

			// Highlight nothing
			Highlight(null);
		}

		// Mouse wants to drag
		protected override void OnDragStart(MouseEventArgs e)
		{
			base.OnDragStart(e);

			// Edit button used?
			if(General.Interface.CheckActionActive(null, "classicedit"))
			{
				// Anything highlighted?
				if((highlighted != null) && !highlighted.IsDisposed)
				{
					// Highlighted item not selected?
					if(!highlighted.Selected)
					{
						// Select only this sector for dragging
						General.Map.Map.ClearSelectedSectors();
						SelectSector(highlighted, true);
					}

					// Start dragging the selection
					General.Map.ChangeMode(new DragSectorsMode(mousedownmappos));
				}
			}
		}

		// This is called wheh selection ends
		protected override void OnEndMultiSelection()
		{
			// Go for all lines
			foreach(Linedef l in General.Map.Map.Linedefs)
			{
				l.Selected = ((l.Start.Position.x >= selectionrect.Left) &&
							  (l.Start.Position.y >= selectionrect.Top) &&
							  (l.Start.Position.x <= selectionrect.Right) &&
							  (l.Start.Position.y <= selectionrect.Bottom) &&
							  (l.End.Position.x >= selectionrect.Left) &&
							  (l.End.Position.y >= selectionrect.Top) &&
							  (l.End.Position.x <= selectionrect.Right) &&
							  (l.End.Position.y <= selectionrect.Bottom));
			}

			// Go for all sectors
			foreach(Sector s in General.Map.Map.Sectors)
			{
				// Go for all sidedefs
				bool allselected = true;
				foreach(Sidedef sd in s.Sidedefs)
				{
					if(!sd.Line.Selected)
					{
						allselected = false;
						break;
					}
				}

				// Sector completely selected?
				s.Selected = allselected;
			}

			// Make sure all linedefs reflect selected sectors
			foreach(Sector s in General.Map.Map.Sectors)
				SelectSector(s, s.Selected);

			base.OnEndMultiSelection();
			if(renderer.StartOverlay(true)) renderer.Finish();
			General.Interface.RedrawDisplay();
		}

		// This is called when the selection is updated
		protected override void OnUpdateMultiSelection()
		{
			base.OnUpdateMultiSelection();

			// Render selection
			if(renderer.StartOverlay(true))
			{
				RenderMultiSelection();
				renderer.Finish();
				renderer.Present();
			}
		}
		
		#endregion

		#region ================== Actions

		[BeginAction("deleteitem", BaseAction = true)]
		public void DeleteItem()
		{
			// Make list of selected sectors
			ICollection<Sector> selected = General.Map.Map.GetSelectedSectors(true);
			if((selected.Count == 0) && (highlighted != null) && !highlighted.IsDisposed) selected.Add(highlighted);

			// Anything to do?
			if(selected.Count > 0)
			{
				// Make undo
				if(selected.Count > 1)
					General.Map.UndoRedo.CreateUndo("Delete " + selected.Count + " sectors", UndoGroup.None, 0);
				else
					General.Map.UndoRedo.CreateUndo("Delete sector", UndoGroup.None, 0);

				// Dispose selected sectors
				foreach(Sector s in selected)
				{
					// Get all the linedefs
					General.Map.Map.ClearMarkedLinedefs(false);
					foreach(Sidedef sd in s.Sidedefs) sd.Line.Marked = true;
					List<Linedef> lines = General.Map.Map.GetMarkedLinedefs(true);
					
					// Dispose the sector
					s.Dispose();

					// Check all the lines
					for(int i = lines.Count - 1; i >= 0; i--)
					{
						// If the line has become orphaned, remove it
						if((lines[i].Front == null) && (lines[i].Back == null))
						{
							// Remove line
							lines[i].Dispose();
						}
						else
						{
							// Update sided flags
							lines[i].ApplySidedFlags();
						}
					}
				}

				// Update cache values
				General.Map.IsChanged = true;
				General.Map.Map.Update();

				// Redraw screen
				General.Interface.RedrawDisplay();
			}
		}
		
		// This joins sectors together and keeps all lines
		[BeginAction("joinsectors")]
		public void JoinSectors()
		{
			// Worth our money?
			int count = General.Map.Map.GetSelectedSectors(true).Count;
			if(count > 1)
			{
				// Make undo
				General.Map.UndoRedo.CreateUndo("Join " + count + " sectors", UndoGroup.None, 0);

				// Merge
				JoinMergeSectors(false);

				// Deselect
				General.Map.Map.ClearSelectedSectors();
				General.Map.Map.ClearSelectedLinedefs();
				General.Map.IsChanged = true;

				// Redraw display
				General.Interface.RedrawDisplay();
			}
		}

		// This joins sectors together and removes the lines in between
		[BeginAction("mergesectors")]
		public void MergeSectors()
		{
			// Worth our money?
			int count = General.Map.Map.GetSelectedSectors(true).Count;
			if(count > 1)
			{
				// Make undo
				General.Map.UndoRedo.CreateUndo("Merge " + count + " sectors", UndoGroup.None, 0);

				// Merge
				JoinMergeSectors(true);

				// Deselect
				General.Map.Map.ClearSelectedSectors();
				General.Map.Map.ClearSelectedLinedefs();
				General.Map.IsChanged = true;

				// Redraw display
				General.Interface.RedrawDisplay();
			}
		}

		// Support function for joining and merging sectors
		private void JoinMergeSectors(bool removelines)
		{
			// Remove lines in betwen joining sectors?
			if(removelines)
			{
				// Go for all selected linedefs
				ICollection<Linedef> selectedlines = General.Map.Map.GetSelectedLinedefs(true);
				foreach(Linedef ld in selectedlines)
				{
					// Front and back side?
					if((ld.Front != null) && (ld.Back != null))
					{
						// Both a selected sector, but not the same?
						if(ld.Front.Sector.Selected && ld.Back.Sector.Selected &&
						   (ld.Front.Sector != ld.Back.Sector))
						{
							// Remove this line
							ld.Dispose();
						}
					}
				}
			}
			
			// Join all selected sectors with the first
			for(int i = 1; i < orderedselection.Count; i++)
				orderedselection[i].Join(orderedselection[0]);
		}

		#endregion
	}
}
