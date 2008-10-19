
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
using CodeImp.DoomBuilder.Types;
using CodeImp.DoomBuilder.BuilderModes.Interface;

#endregion

namespace CodeImp.DoomBuilder.BuilderModes
{
	[EditMode(DisplayName = "Sectors",
			  SwitchAction = "sectorsmode",		// Action name used to switch to this mode
			  ButtonDesc = "Sectors Mode",		// Description on the button in toolbar/menu
		      ButtonImage = "SectorsMode.png",	// Image resource name for the button
			  ButtonOrder = int.MinValue + 200)]	// Position of the button (lower is more to the left)

	public class SectorsMode : BaseClassicMode
	{
		#region ================== Constants

		#endregion

		#region ================== Variables

		// Highlighted item
		protected Sector highlighted;
		private Association highlightasso = new Association();

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

		// This highlights a new item
		protected void Highlight(Sector s)
		{
			bool completeredraw = false;

			// Often we can get away by simply undrawing the previous
			// highlight and drawing the new highlight. But if associations
			// are or were drawn we need to redraw the entire display.

			// Previous association highlights something?
			if((highlighted != null) && (highlighted.Tag > 0)) completeredraw = true;

			// Set highlight association
			if(s != null)
				highlightasso.Set(s.Tag, UniversalType.SectorTag);
			else
				highlightasso.Set(0, 0);

			// New association highlights something?
			if((s != null) && (s.Tag > 0)) completeredraw = true;

			// If we're changing associations, then we
			// need to redraw the entire display
			if(completeredraw)
			{
				// Set new highlight and redraw completely
				highlighted = s;
				General.Interface.RedrawDisplay();
			}
			else
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
			}

			// Show highlight info
			if((highlighted != null) && !highlighted.IsDisposed)
				General.Interface.ShowSectorInfo(highlighted);
			else
				General.Interface.HideInfo();
		}

		// This selectes or deselects a sector
		protected void SelectSector(Sector s, bool selectstate)
		{
			bool selectionchanged = false;

			if(!s.IsDisposed)
			{
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
			else
			{
				// Remove from list
				orderedselection.Remove(s);
			}
		}

		#endregion
		
		#region ================== Events
		
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
			
			// Convert geometry selection to sectors only
			General.Map.Map.ClearAllMarks(false);
			General.Map.Map.MarkSelectedVertices(true, true);
			ICollection<Linedef> lines = General.Map.Map.LinedefsFromMarkedVertices(false, true, false);
			foreach(Linedef l in lines) l.Selected = true;
			General.Map.Map.ClearMarkedSectors(true);
			foreach(Linedef l in General.Map.Map.Linedefs)
			{
				if(!l.Selected)
				{
					if(l.Front != null) l.Front.Sector.Marked = false;
					if(l.Back != null) l.Back.Sector.Marked = false;
				}
			}
			General.Map.Map.ClearAllSelected();
			foreach(Sector s in General.Map.Map.Sectors)
			{
				if(s.Marked)
				{
					s.Selected = true;
					foreach(Sidedef sd in s.Sidedefs) sd.Line.Selected = true;
				}
			}
			
			// Fill the list with selected sectors (these are not in order, but we have no other choice)
			ICollection<Sector> selectedsectors = General.Map.Map.GetSelectedSectors(true);
			foreach(Sector s in selectedsectors) orderedselection.Add(s);
		}
		
		// Mode disengages
		public override void OnDisengage()
		{
			base.OnDisengage();
			
			// Going to EditSelectionMode?
			if(General.Map.NewMode is EditSelectionMode)
			{
				// No selection made? But we have a highlight!
				if((General.Map.Map.GetSelectedSectors(true).Count == 0) && (highlighted != null))
				{
					// Make the highlight the selection
					SelectSector(highlighted, true);
				}
			}
			
			// Hide highlight info
			General.Interface.HideInfo();
		}

		// This redraws the display
		public override void OnRedrawDisplay()
		{
			renderer.RedrawSurface();
			
			// Render lines and vertices
			if(renderer.StartPlotter(true))
			{
				renderer.PlotLinedefSet(General.Map.Map.Linedefs);
				renderer.PlotVerticesSet(General.Map.Map.Vertices);
				if((highlighted != null) && !highlighted.IsDisposed)
				{
					renderer.PlotSector(highlighted, General.Colors.Highlight);
					BuilderPlug.Me.PlotReverseAssociations(renderer, highlightasso);
				}
				renderer.Finish();
			}

			// Render things
			if(renderer.StartThings(true))
			{
				renderer.RenderThingSet(General.Map.ThingsFilter.HiddenThings, Presentation.THINGS_HIDDEN_ALPHA);
				renderer.RenderThingSet(General.Map.ThingsFilter.VisibleThings, 1.0f);
				renderer.Finish();
			}

			// Render selection
			if(renderer.StartOverlay(true))
			{
				if((highlighted != null) && !highlighted.IsDisposed) BuilderPlug.Me.RenderReverseAssociations(renderer, highlightasso);
				if(selecting) RenderMultiSelection();
				renderer.Finish();
			}

			renderer.Present();
		}

		// Selection
		protected override void OnSelectBegin()
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

			base.OnSelectBegin();
		}

		// End selection
		protected override void OnSelectEnd()
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

			base.OnSelectEnd();
		}

		// Start editing
		protected override void OnEditBegin()
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
			
			base.OnEditBegin();
		}

		// Done editing
		protected override void OnEditEnd()
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
						orderedselection.Clear();
						General.Map.Map.ClearSelectedSectors();
						General.Map.Map.ClearSelectedLinedefs();
					}

					// Update entire display
					General.Interface.RedrawDisplay();
				}
			}

			editpressed = false;
			base.OnEditEnd();
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
				SelectSector(s, allselected);
			}
			
			// Make sure all linedefs reflect selected sectors
			foreach(Sidedef sd in General.Map.Map.Sidedefs)
				if(!sd.Sector.Selected && ((sd.Other == null) || !sd.Other.Sector.Selected))
					sd.Line.Selected = false;
			
			// Fill the list with selected sectors (these are not in order, but we have no other choice)
			orderedselection.Clear();
			ICollection<Sector> selectedsectors = General.Map.Map.GetSelectedSectors(true);
			foreach(Sector s in selectedsectors) orderedselection.Add(s);
			
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

		// When copying
		public override bool OnCopyBegin()
		{
			// No selection made? But we have a highlight!
			if((General.Map.Map.GetSelectedSectors(true).Count == 0) && (highlighted != null))
			{
				// Make the highlight the selection
				SelectSector(highlighted, true);
			}

			return base.OnCopyBegin();
		}

		// When pasting
		public override bool OnPasteBegin()
		{
			// No selection made? But we have a highlight!
			if((General.Map.Map.GetSelectedSectors(true).Count == 0) && (highlighted != null))
			{
				// Make the highlight the selection
				SelectSector(highlighted, true);
			}

			return base.OnPasteBegin();
		}

		// When undo is used
		public override bool OnUndoBegin()
		{
			// Clear ordered selection
			orderedselection.Clear();

			return base.OnUndoBegin();
		}

		// When redo is used
		public override bool OnRedoBegin()
		{
			// Clear ordered selection
			orderedselection.Clear();

			return base.OnRedoBegin();
		}
		
		#endregion

		#region ================== Actions

		// This creates a new vertex at the mouse position
		[BeginAction("insertitem", BaseAction = true)]
		public virtual void InsertVertex()
		{
			// Mouse in window?
			if(mouseinside)
			{
				// Create vertex at mouse position
				Vertex v = General.Map.Map.CreateVertex(mousemappos);

				// Snap to grid enabled?
				if(General.Interface.SnapToGrid)
				{
					// Snap to grid
					v.SnapToGrid();
				}
				else
				{
					// Snap to map format accuracy
					v.SnapToAccuracy();
				}

				// Redraw screen
				General.Interface.RedrawDisplay();
			}
		}

		[BeginAction("makedoor")]
		public void MakeDoor()
		{
			// Highlighted item not selected?
			if(!highlighted.Selected)
			{
				if(highlighted != null)
				{
					// Make this the only selection
					General.Map.Map.ClearSelectedSectors();
					General.Map.Map.ClearSelectedLinedefs();
					SelectSector(highlighted, true);
					General.Interface.RedrawDisplay();
				}
			}
			
			// Anything selected?
			if(orderedselection.Count > 0)
			{
				string doortex = "";
				string floortex = null;
				string ceiltex = null;
				
				// Find ceiling and floor textures
				foreach(Sector s in orderedselection)
				{
					if(floortex == null) floortex = s.FloorTexture; else if(floortex != s.FloorTexture) floortex = "";
					if(ceiltex == null) ceiltex = s.CeilTexture; else if(ceiltex != s.CeilTexture) ceiltex = "";
				}
				
				// Show the dialog
				MakeDoorForm form = new MakeDoorForm();
				if(form.Show(General.Interface, doortex, ceiltex, floortex) == DialogResult.OK)
				{
					doortex = form.DoorTexture;
					ceiltex = form.CeilingTexture;
					floortex = form.FloorTexture;
					
					// Create undo
					General.Map.UndoRedo.CreateUndo("Make door (" + doortex + ")", UndoGroup.None, 0);
					
					// Go for all selected sectors
					foreach(Sector s in orderedselection)
					{
						// Lower the ceiling down to the floor
						s.CeilHeight = s.FloorHeight;

						// Make a unique tag (not sure if we need it yet, depends on the args)
						int tag = General.Map.Map.GetNewTag();

						// Go for all it's sidedefs
						foreach(Sidedef sd in s.Sidedefs)
						{
							// Singlesided?
							if(sd.Other == null)
							{
								// Make this a doortrak
								sd.SetTextureHigh("-");
								sd.SetTextureMid(General.Map.Config.MakeDoorTrack);
								sd.SetTextureLow("-");

								// Set upper/lower unpegged flags
								sd.Line.Flags[General.Map.Config.UpperUnpeggedFlag] = false;
								sd.Line.Flags[General.Map.Config.LowerUnpeggedFlag] = true;
							}
							else
							{
								// Set textures
								if(floortex.Length > 0) s.SetFloorTexture(floortex);
								if(ceiltex.Length > 0) s.SetCeilTexture(ceiltex);
								if(doortex.Length > 0) sd.Other.SetTextureHigh(doortex);

								// Set upper/lower unpegged flags
								sd.Line.Flags[General.Map.Config.UpperUnpeggedFlag] = false;
								sd.Line.Flags[General.Map.Config.LowerUnpeggedFlag] = false;
								
								// Get door linedef type from config
								sd.Line.Action = General.Map.Config.MakeDoorAction;

								// Set the linedef args
								for(int i = 0; i < Linedef.NUM_ARGS; i++)
								{
									// A -1 arg indicates that the arg must be set to the new sector tag
									// and only in this case we set the tag on the sector, because only
									// then we know for sure that we need a tag.
									if(General.Map.Config.MakeDoorArgs[i] == -1)
									{
										sd.Line.Args[i] = tag;
										s.Tag = tag;
									}
									else
									{
										sd.Line.Args[i] = General.Map.Config.MakeDoorArgs[i];
									}
								}

								// Make sure the line is facing outwards
								if(sd.IsFront)
								{
									sd.Line.FlipVertices();
									sd.Line.FlipSidedefs();
								}
							}
						}
					}
					
					// When a single sector was selected, deselect it now
					if(orderedselection.Count == 1)
					{
						orderedselection.Clear();
						General.Map.Map.ClearSelectedSectors();
						General.Map.Map.ClearSelectedLinedefs();
					}
				}
				
				// Done
				form.Dispose();
				General.Interface.RedrawDisplay();
			}
		}
		
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

		// This clears the selection
		[BeginAction("clearselection", BaseAction = true)]
		public void ClearSelection()
		{
			// Clear selection
			General.Map.Map.ClearAllSelected();
			orderedselection.Clear();

			// Redraw
			General.Interface.RedrawDisplay();
		}
		
		#endregion
	}
}
