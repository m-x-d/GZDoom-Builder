
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
using CodeImp.DoomBuilder.Actions;
using CodeImp.DoomBuilder.Config;
using CodeImp.DoomBuilder.Editing;
using CodeImp.DoomBuilder.Geometry;
using CodeImp.DoomBuilder.GZBuilder.Geometry;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Rendering;
using CodeImp.DoomBuilder.Types;
using CodeImp.DoomBuilder.Windows;

#endregion

namespace CodeImp.DoomBuilder.BuilderModes
{
	[EditMode(DisplayName = "Things Mode",
			  SwitchAction = "thingsmode",		// Action name used to switch to this mode
			  ButtonImage = "ThingsMode.png",	// Image resource name for the button
			  ButtonOrder = int.MinValue + 300,	// Position of the button (lower is more to the left)
			  ButtonGroup = "000_editing",
			  UseByDefault = true,
			  SafeStartMode = true)]

	public class ThingsMode : BaseClassicMode
	{
		#region ================== Constants

		#endregion

		#region ================== Variables

		// Highlighted item
		private Thing highlighted;
		private Association[] association = new Association[Thing.NUM_ARGS];
		private Association highlightasso = new Association();

		// Interface
		private bool editpressed;
		private bool thinginserted;
		private bool awaitingMouseClick; //mxd
		
		#endregion

		#region ================== Properties

		public override object HighlightedObject { get { return highlighted; } }

		#endregion

		#region ================== Constructor / Disposer

		#endregion

		#region ================== Methods

		//mxd. This makes a CRC for given selection
		private static int CreateSelectionCRC(ICollection<Thing> selection) 
		{
			CRC crc = new CRC();
			crc.Add(selection.Count);
			foreach(Thing t in selection) crc.Add(t.Index);
			return (int)(crc.Value & 0xFFFFFFFF);
		}

		public override void OnHelp()
		{
			General.ShowHelp("e_things.html");
		}

		// Cancel mode
		public override void OnCancel()
		{
			base.OnCancel();

			// Return to this mode
			General.Editing.ChangeMode(new ThingsMode());
		}

		// Mode engages
		public override void OnEngage()
		{
			base.OnEngage();
			renderer.SetPresentation(Presentation.Things);

			// Add toolbar buttons
			General.Interface.AddButton(BuilderPlug.Me.MenusForm.CopyProperties);
			General.Interface.AddButton(BuilderPlug.Me.MenusForm.PasteProperties);
			General.Interface.AddButton(BuilderPlug.Me.MenusForm.PastePropertiesOptions); //mxd
			General.Interface.AddButton(BuilderPlug.Me.MenusForm.SeparatorCopyPaste); //mxd
			General.Interface.AddButton(BuilderPlug.Me.MenusForm.AlignThingsToWall); //mxd
			
			// Convert geometry selection to linedefs selection
			General.Map.Map.ConvertSelection(SelectionType.Linedefs);
			General.Map.Map.SelectionType = SelectionType.Things;
			UpdateSelectionInfo(); //mxd
		}

		// Mode disengages
		public override void OnDisengage()
		{
			base.OnDisengage();

			// Remove toolbar buttons
			General.Interface.RemoveButton(BuilderPlug.Me.MenusForm.CopyProperties);
			General.Interface.RemoveButton(BuilderPlug.Me.MenusForm.PasteProperties);
			General.Interface.RemoveButton(BuilderPlug.Me.MenusForm.PastePropertiesOptions); //mxd
			General.Interface.RemoveButton(BuilderPlug.Me.MenusForm.SeparatorCopyPaste); //mxd
			General.Interface.RemoveButton(BuilderPlug.Me.MenusForm.AlignThingsToWall); //mxd
			
			// Going to EditSelectionMode?
			if(General.Editing.NewMode is EditSelectionMode)
			{
				// Not pasting anything?
				EditSelectionMode editmode = (General.Editing.NewMode as EditSelectionMode);
				if(!editmode.Pasting)
				{
					// No selection made? But we have a highlight!
					if((General.Map.Map.GetSelectedThings(true).Count == 0) && (highlighted != null))
					{
						// Make the highlight the selection
						highlighted.Selected = true;
					}
				}
			}

			// Hide highlight info
			General.Interface.HideInfo();
		}

		// This redraws the display
		public override void OnRedrawDisplay()
		{
			renderer.RedrawSurface();
			List<Line3D> eventlines = new List<Line3D>(); //mxd

			// Render lines and vertices
			if (renderer.StartPlotter(true)) 
			{
				renderer.PlotLinedefSet(General.Map.Map.Linedefs);
				renderer.PlotVerticesSet(General.Map.Map.Vertices);

				for(int i = 0; i < Thing.NUM_ARGS; i++) BuilderPlug.PlotAssociations(renderer, association[i], eventlines);
				if((highlighted != null) && !highlighted.IsDisposed) BuilderPlug.PlotReverseAssociations(renderer, highlightasso, eventlines);
				
				renderer.Finish();
			}

			// Render things
			if(renderer.StartThings(true))
			{
				renderer.RenderThingSet(General.Map.ThingsFilter.HiddenThings, Presentation.THINGS_HIDDEN_ALPHA);
				renderer.RenderThingSet(General.Map.ThingsFilter.VisibleThings, 1.0f);
				for(int i = 0; i < Thing.NUM_ARGS; i++) BuilderPlug.RenderAssociations(renderer, association[i], eventlines);
				
				if((highlighted != null) && !highlighted.IsDisposed)
				{
					renderer.RenderThing(highlighted, General.Colors.Highlight, 1.0f);
					BuilderPlug.RenderReverseAssociations(renderer, highlightasso, eventlines); //mxd
				}

				//mxd
				if(General.Settings.GZShowEventLines) 
				{
					List<List<Line3D>> lines = GZBuilder.Data.LinksCollector.GetThingLinks(General.Map.ThingsFilter.VisibleThings);
					if(lines != null)
					{
						lines[0].AddRange(eventlines);
						foreach(List<Line3D> list in lines) if(list.Count > 0) renderer.RenderArrows(list);
					} 
					else
					{
						renderer.RenderArrows(eventlines);
					}
				}
 
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
		protected void Highlight(Thing t)
		{
			bool completeredraw = false;
			LinedefActionInfo action = null;

			// Often we can get away by simply undrawing the previous
			// highlight and drawing the new highlight. But if associations
			// are or were drawn we need to redraw the entire display.

			// Previous association highlights something?
			if((highlighted != null) && (highlighted.Tag > 0)) completeredraw = true;
			
			// Set highlight association
			if(t != null)
				highlightasso.Set(t.Position, t.Tag, UniversalType.ThingTag);
			else
				highlightasso.Set(new Vector2D(), 0, 0);

			// New association highlights something?
			if((t != null) && (t.Tag > 0)) completeredraw = true;

			if(t != null)
			{
				// Check if we can find the linedefs action
				if((t.Action > 0) && General.Map.Config.LinedefActions.ContainsKey(t.Action))
					action = General.Map.Config.LinedefActions[t.Action];
			}
			
			// Determine linedef associations
			for(int i = 0; i < Thing.NUM_ARGS; i++)
			{
				// Previous association highlights something?
				if((association[i].type == UniversalType.SectorTag) ||
				   (association[i].type == UniversalType.LinedefTag) ||
				   (association[i].type == UniversalType.ThingTag)) completeredraw = true;
				
				// Make new association
				if(action != null)
					association[i].Set(t.Position, t.Args[i], action.Args[i].Type);
				else
					association[i].Set(new Vector2D(), 0, 0);
				
				// New association highlights something?
				if((association[i].type == UniversalType.SectorTag) ||
				   (association[i].type == UniversalType.LinedefTag) ||
				   (association[i].type == UniversalType.ThingTag)) completeredraw = true;
			}
			
			// If we're changing associations, then we
			// need to redraw the entire display
			if(completeredraw)
			{
				// Set new highlight and redraw completely
				highlighted = t;
				General.Interface.RedrawDisplay();
			}
			else
			{
				// Update display
				if(renderer.StartThings(false))
				{
					// Undraw previous highlight
					if((highlighted != null) && !highlighted.IsDisposed)
						renderer.RenderThing(highlighted, renderer.DetermineThingColor(highlighted), 1.0f);

					// Set new highlight
					highlighted = t;

					// Render highlighted item
					if((highlighted != null) && !highlighted.IsDisposed)
						renderer.RenderThing(highlighted, General.Colors.Highlight, 1.0f);

					// Done
					renderer.Finish();
					renderer.Present();
				}
			}
			
			// Show highlight info
			if((highlighted != null) && !highlighted.IsDisposed)
				General.Interface.ShowThingInfo(highlighted);
			else
				General.Interface.HideInfo();
		}

		// Selection
		protected override void OnSelectBegin()
		{
			//mxd. Yep, it's kinda hackish...
			if(awaitingMouseClick) 
			{
				awaitingMouseClick = false;
				ThingPointAtCursor();
				return;
			}
			
			// Item highlighted?
			if((highlighted != null) && !highlighted.IsDisposed)
			{
				// Update display
				if(renderer.StartThings(false))
				{
					// Redraw highlight to show selection
					renderer.RenderThing(highlighted, renderer.DetermineThingColor(highlighted), 1.0f);
					renderer.Finish();
					renderer.Present();
				}
			}

			base.OnSelectBegin();
		}

		// End selection
		protected override void OnSelectEnd()
		{
			// Not ending from a multi-selection?
			if(!selecting)
			{
				// Item highlighted?
				if((highlighted != null) && !highlighted.IsDisposed)
				{
					//mxd. Flip selection
					highlighted.Selected = !highlighted.Selected;
					
					// Update display
					if(renderer.StartThings(false))
					{
						// Render highlighted item
						renderer.RenderThing(highlighted, General.Colors.Highlight, 1.0f);
						renderer.Finish();
						renderer.Present();
					}
				
				}
				//mxd
				else if(BuilderPlug.Me.AutoClearSelection && General.Map.Map.SelectedThingsCount > 0) 
				{
					General.Map.Map.ClearSelectedThings();
					General.Interface.RedrawDisplay();
				}

				UpdateSelectionInfo(); //mxd
			}

			base.OnSelectEnd();
		}

		// Start editing
		protected override void OnEditBegin()
		{
			thinginserted = false;
			
			// Item highlighted?
			if((highlighted != null) && !highlighted.IsDisposed)
			{
				// Edit pressed in this mode
				editpressed = true;
				
				// Highlighted item not selected?
				if(!highlighted.Selected && (BuilderPlug.Me.AutoClearSelection || (General.Map.Map.SelectedThingsCount == 0)))
				{
					// Make this the only selection
					General.Map.Map.ClearSelectedThings();
					highlighted.Selected = true;
					General.Interface.RedrawDisplay();
				}

				// Update display
				if(renderer.StartThings(false))
				{
					// Redraw highlight to show selection
					renderer.RenderThing(highlighted, renderer.DetermineThingColor(highlighted), 1.0f);
					renderer.Finish();
					renderer.Present();
				}
			}
			else if(mouseinside && !selecting && BuilderPlug.Me.AutoDrawOnEdit) //mxd. We don't want to insert a thing when multiselecting
			{
				// Edit pressed in this mode
				editpressed = true;
				thinginserted = true;

				// Insert a new item and select it for dragging
				General.Map.UndoRedo.CreateUndo("Insert thing");
				Thing t = InsertThing(mousemappos);

				if(t == null) 
				{
					General.Map.UndoRedo.WithdrawUndo();
				} 
				else 
				{
					General.Map.Map.ClearSelectedThings();
					t.Selected = true;
					Highlight(t);
					General.Interface.RedrawDisplay();
				}
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
				ICollection<Thing> selected = General.Map.Map.GetSelectedThings(true);
				if(selected.Count > 0)
				{
					if(General.Interface.IsActiveWindow)
					{
						// Edit only when preferred
						if(!thinginserted || BuilderPlug.Me.EditNewThing)
						{
							//mxd. Show realtime thing edit dialog
							General.Interface.OnEditFormValuesChanged += thingEditForm_OnValuesChanged;
							DialogResult result = General.Interface.ShowEditThings(selected);
							General.Interface.OnEditFormValuesChanged -= thingEditForm_OnValuesChanged;

							// When a single thing was selected, deselect it now
							if (selected.Count == 1) 
							{
								General.Map.Map.ClearSelectedThings();
							} 
							else if(result == DialogResult.Cancel) //mxd. Restore selection...
							{ 
								foreach (Thing t in selected) t.Selected = true;
							}
							General.Interface.RedrawDisplay();
						}
					}
				}

				UpdateSelectionInfo(); //mxd
			}

			editpressed = false;
			base.OnEditEnd();
		}

		//mxd. Otherwise event lines won't be drawn after panning finishes.
		protected override void EndViewPan()
		{
			base.EndViewPan();
			if(General.Settings.GZShowEventLines) General.Interface.RedrawDisplay(); 
		}

		//mxd
		private void thingEditForm_OnValuesChanged(object sender, EventArgs e) 
		{
			// Update things filter
			General.Map.ThingsFilter.Update();

			// Update entire display
			General.Interface.RedrawDisplay();
		}

		// Mouse moves
		public override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);
			if(panning) return; //mxd. Skip all this jazz while panning

			//mxd
			if(selectpressed && !editpressed && !selecting) 
			{
				// Check if moved enough pixels for multiselect
				Vector2D delta = mousedownpos - mousepos;
				if((Math.Abs(delta.x) > MULTISELECT_START_MOVE_PIXELS) ||
				   (Math.Abs(delta.y) > MULTISELECT_START_MOVE_PIXELS)) 
				{
					// Start multiselecting
					StartMultiSelection();
				}
			}
			else if(paintselectpressed && !editpressed && !selecting) //mxd. Drag-select
			{
				// Find the nearest thing within highlight range
				Thing t = MapSet.NearestThingSquareRange(General.Map.ThingsFilter.VisibleThings, mousemappos, BuilderPlug.Me.HighlightThingsRange / renderer.Scale);

				if(t != null) 
				{
					if(t != highlighted) 
					{
						//toggle selected state
						if(General.Interface.ShiftState ^ BuilderPlug.Me.AdditiveSelect)
							t.Selected = true;
						else if (General.Interface.CtrlState)
							t.Selected = false;
						else
							t.Selected = !t.Selected;
						highlighted = t;

						UpdateSelectionInfo(); //mxd

						// Update entire display
						General.Interface.RedrawDisplay();
					}
				} 
				else if(highlighted != null) 
				{
					highlighted = null;
					Highlight(null);
					
					// Update entire display
					General.Interface.RedrawDisplay();
				}
			}
			else if(e.Button == MouseButtons.None) // Not holding any buttons?
			{
				// Find the nearest thing within highlight range
				Thing t = MapSet.NearestThingSquareRange(General.Map.ThingsFilter.VisibleThings, mousemappos, BuilderPlug.Me.HighlightThingsRange / renderer.Scale);

				// Highlight if not the same
				if(t != highlighted) Highlight(t);
			} 
		}

		// Mouse leaves
		public override void OnMouseLeave(EventArgs e)
		{
			base.OnMouseLeave(e);

			// Highlight nothing
			Highlight(null);
		}

		//mxd
		protected override void OnPaintSelectBegin() 
		{
			highlighted = null;
			base.OnPaintSelectBegin();
		}

		// Mouse wants to drag
		protected override void OnDragStart(MouseEventArgs e)
		{
			base.OnDragStart(e);

			// Edit button used?
			if(General.Actions.CheckActionActive(null, "classicedit"))
			{
				// Anything highlighted?
				if((highlighted != null) && !highlighted.IsDisposed)
				{
					// Highlighted item not selected?
					if(!highlighted.Selected)
					{
						// Select only this thing for dragging
						General.Map.Map.ClearSelectedThings();
						highlighted.Selected = true;
					}

					// Start dragging the selection
					if (!BuilderPlug.Me.DontMoveGeometryOutsideMapBoundary || CanDrag()) //mxd
					{ 
						// Shift pressed? Clone things!
						if (General.Interface.ShiftState) 
						{
							ICollection<Thing> selection = General.Map.Map.GetSelectedThings(true);
							foreach(Thing t in selection) 
							{
								Thing clone = InsertThing(t.Position);
								t.CopyPropertiesTo(clone);
								t.Selected = false;
								clone.Selected = true;
							}
						}

						General.Editing.ChangeMode(new DragThingsMode(new ThingsMode(), mousedownmappos));
					}
				}
			}
		}

		//mxd. Check if any selected thing is outside of map boundary
		private static bool CanDrag() 
		{
			ICollection<Thing> selectedthings = General.Map.Map.GetSelectedThings(true);
			int unaffectedCount = 0;

			foreach(Thing t in selectedthings) 
			{
				// Make sure the vertex is inside the map boundary
				if(t.Position.x < General.Map.Config.LeftBoundary || t.Position.x > General.Map.Config.RightBoundary
					|| t.Position.y > General.Map.Config.TopBoundary || t.Position.y < General.Map.Config.BottomBoundary) 
				{
					t.Selected = false;
					unaffectedCount++;
				}
			}

			if(unaffectedCount == selectedthings.Count) 
			{
				General.Interface.DisplayStatus(StatusType.Warning, "Unable to drag selection: " + (selectedthings.Count == 1 ? "selected thing is" : "all of selected things are") + " outside of map boundary!");
				General.Interface.RedrawDisplay();
				return false;
			}

			if(unaffectedCount > 0)
				General.Interface.DisplayStatus(StatusType.Warning, unaffectedCount + " of selected vertices " + (unaffectedCount == 1 ? "is" : "are") + " outside of map boundary!");

			return true;
		}

		// This is called wheh selection ends
		protected override void OnEndMultiSelection()
		{
			bool selectionvolume = ((Math.Abs(selectionrect.Width) > 0.1f) && (Math.Abs(selectionrect.Height) > 0.1f));

			if(selectionvolume)
			{
				//mxd
				switch(marqueSelectionMode) 
				{
					case MarqueSelectionMode.SELECT:
						foreach(Thing t in General.Map.ThingsFilter.VisibleThings)
							t.Selected = selectionrect.Contains(t.Position.x, t.Position.y);
						break;

					case MarqueSelectionMode.ADD:
						foreach(Thing t in General.Map.ThingsFilter.VisibleThings)
							t.Selected |= selectionrect.Contains(t.Position.x, t.Position.y);
						break;

					case MarqueSelectionMode.SUBTRACT:
						foreach(Thing t in General.Map.ThingsFilter.VisibleThings)
							if(selectionrect.Contains(t.Position.x, t.Position.y)) t.Selected = false;
						break;

					default: //should be Intersect
						foreach(Thing t in General.Map.ThingsFilter.VisibleThings)
							if(!selectionrect.Contains(t.Position.x, t.Position.y)) t.Selected = false;
						break;
				}

				UpdateSelectionInfo(); //mxd
			}
			
			base.OnEndMultiSelection();

			// Clear overlay
			if(renderer.StartOverlay(true)) renderer.Finish();

			// Redraw
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
			if((General.Map.Map.GetSelectedThings(true).Count == 0) && (highlighted != null))
			{
				// Make the highlight the selection
				highlighted.Selected = true;

				//mxd. Actually, we want it marked, not selected
				bool result = base.OnCopyBegin();
				highlighted.Selected = false;
				return result;
			}

			return base.OnCopyBegin();
		}

		//mxd
		public override void UpdateSelectionInfo() 
		{
			if(General.Map.Map.SelectedThingsCount > 0)
				General.Interface.DisplayStatus(StatusType.Selection, General.Map.Map.SelectedThingsCount + (General.Map.Map.SelectedThingsCount == 1 ? " thing" : " things") + " selected.");
			else
				General.Interface.DisplayStatus(StatusType.Selection, string.Empty);
		}

		#endregion

		#region ================== Actions

		// This copies the properties
		[BeginAction("classiccopyproperties")]
		public void CopyProperties()
		{
			// Determine source things
			ICollection<Thing> sel = null;
			if(General.Map.Map.SelectedThingsCount > 0)
				sel = General.Map.Map.GetSelectedThings(true);
			else if(highlighted != null)
				sel = new List<Thing> {highlighted};
			
			if(sel != null)
			{
				// Copy properties from first source thing
				BuilderPlug.Me.CopiedThingProps = new ThingProperties(General.GetByIndex(sel, 0));
				General.Interface.DisplayStatus(StatusType.Action, "Copied thing properties.");
			}
		}

		// This pastes the properties
		[BeginAction("classicpasteproperties")]
		public void PasteProperties()
		{
			if(BuilderPlug.Me.CopiedThingProps != null)
			{
				// Determine target things
				ICollection<Thing> sel = null;
				if(General.Map.Map.SelectedThingsCount > 0)
					sel = General.Map.Map.GetSelectedThings(true);
				else if(highlighted != null)
				{
					sel = new List<Thing>();
					sel.Add(highlighted);
				}
				
				if(sel != null)
				{
					// Apply properties to selection
					General.Map.UndoRedo.CreateUndo("Paste thing properties");
					foreach(Thing t in sel)
					{
						BuilderPlug.Me.CopiedThingProps.Apply(t);
						t.UpdateConfiguration();
					}
					General.Interface.DisplayStatus(StatusType.Action, "Pasted thing properties.");
					
					// Update and redraw
					General.Map.IsChanged = true;
					General.Map.ThingsFilter.Update();
					General.Interface.RefreshInfo();
					General.Interface.RedrawDisplay();
				}
			}
		}

		// This clears the selection
		[BeginAction("clearselection", BaseAction = true)]
		public void ClearSelection()
		{
			// Clear selection
			General.Map.Map.ClearAllSelected();

			//mxd. Clear selection info
			General.Interface.DisplayStatus(StatusType.Selection, string.Empty);

			// Redraw
			General.Interface.RedrawDisplay();
		}
		
		// This creates a new thing at the mouse position
		[BeginAction("insertitem", BaseAction = true)]
		public virtual void InsertThing()
		{
			// Mouse in window?
			if(mouseinside)
			{
				// Insert new thing
				General.Map.UndoRedo.CreateUndo("Insert thing");
				Thing t = InsertThing(mousemappos);

				if (t == null)
				{
					General.Map.UndoRedo.WithdrawUndo();
					return;
				}
				
				// Edit the thing?
				if(BuilderPlug.Me.EditNewThing)
				{
					// Redraw screen
					General.Interface.RedrawDisplay();
					General.Interface.ShowEditThings(new List<Thing> { t });
				}

				General.Interface.DisplayStatus(StatusType.Action, "Inserted a new thing.");

				// Update things filter
				General.Map.ThingsFilter.Update();

				// Redraw screen
				General.Interface.RedrawDisplay();
			}
		}

		// This creates a new thing
		private static Thing InsertThing(Vector2D pos)
		{
			if (pos.x < General.Map.Config.LeftBoundary || pos.x > General.Map.Config.RightBoundary ||
				pos.y > General.Map.Config.TopBoundary || pos.y < General.Map.Config.BottomBoundary)
			{
				General.Interface.DisplayStatus(StatusType.Warning, "Failed to insert thing: outside of map boundaries.");
				return null;
			}

			// Create thing
			Thing t = General.Map.Map.CreateThing();
			if(t != null)
			{
				General.Settings.ApplyDefaultThingSettings(t);
				
				t.Move(pos);
				
				t.UpdateConfiguration();

				// Update things filter so that it includes this thing
				General.Map.ThingsFilter.Update();

				// Snap to grid enabled?
				if(General.Interface.SnapToGrid)
				{
					// Snap to grid
					t.SnapToGrid();
				}
				else
				{
					// Snap to map format accuracy
					t.SnapToAccuracy();
				}
			}
			
			return t;
		}

		[BeginAction("deleteitem", BaseAction = true)]
		public void DeleteItem()
		{
			// Make list of selected things
			List<Thing> selected = new List<Thing>(General.Map.Map.GetSelectedThings(true));
			if((selected.Count == 0) && (highlighted != null) && !highlighted.IsDisposed) selected.Add(highlighted);
			
			// Anything to do?
			if(selected.Count > 0)
			{
				// Make undo
				if(selected.Count > 1)
				{
					General.Map.UndoRedo.CreateUndo("Delete " + selected.Count + " things");
					General.Interface.DisplayStatus(StatusType.Action, "Deleted " + selected.Count + " things.");
				}
				else
				{
					General.Map.UndoRedo.CreateUndo("Delete thing");
					General.Interface.DisplayStatus(StatusType.Action, "Deleted a thing.");
				}

				General.Map.Map.BeginAddRemove(); //mxd

				// Dispose selected things
				foreach(Thing t in selected) t.Dispose();

				General.Map.Map.EndAddRemove(); //mxd
				
				// Update cache values
				General.Map.IsChanged = true;
				General.Map.ThingsFilter.Update();

				// Invoke a new mousemove so that the highlighted item updates
				MouseEventArgs e = new MouseEventArgs(MouseButtons.None, 0, (int)mousepos.x, (int)mousepos.y, 0);
				OnMouseMove(e);

				// Redraw screen
				General.Interface.RedrawDisplay();
			}
		}

		//mxd
		[BeginAction("thingaligntowall")]
		public void AlignThingsToWall() 
		{
			// Make list of selected things
			List<Thing> selected = new List<Thing>(General.Map.Map.GetSelectedThings(true));
			if((selected.Count == 0) && (highlighted != null) && !highlighted.IsDisposed) selected.Add(highlighted);

			if(selected.Count == 0) 
			{
				General.Interface.DisplayStatus(StatusType.Warning, "This action requires a selection!");
				return;
			}

			List<Thing> toAlign = new List<Thing>();

			foreach(Thing t in selected) if(t.IsModel) toAlign.Add(t);

			if(toAlign.Count == 0) 
			{
				General.Interface.DisplayStatus(StatusType.Warning, "This action only works for things with models!");
				return;
			}
 
			// Make undo
			if(toAlign.Count > 1) 
			{
				General.Map.UndoRedo.CreateUndo("Align " + toAlign.Count + " things");
				General.Interface.DisplayStatus(StatusType.Action, "Aligned " + toAlign.Count + " things.");
			} 
			else 
			{
				General.Map.UndoRedo.CreateUndo("Align thing");
				General.Interface.DisplayStatus(StatusType.Action, "Aligned a thing.");
			}

			//align things
			int thingsCount = General.Map.Map.Things.Count;

			foreach(Thing t in toAlign) 
			{
				List<Linedef> excludedLines = new List<Linedef>();
				bool aligned;

				do
				{
					Linedef l = General.Map.Map.NearestLinedef(t.Position, excludedLines);
					aligned = Tools.TryAlignThingToLine(t, l);
					
					if(!aligned) 
					{
						excludedLines.Add(l);
						if(excludedLines.Count == thingsCount) 
						{
							ThingTypeInfo tti = General.Map.Data.GetThingInfo(t.Type);
							General.ErrorLogger.Add(ErrorType.Warning, "Unable to align Thing ¹" + t.Index + " (" + tti.Title + ") to any linedef in a map!");
							aligned = true;
						}
					}
				} while(!aligned);
			}

			// Update cache values
			General.Map.IsChanged = true;

			// Redraw screen
			General.Interface.RedrawDisplay();
		}

		[BeginAction("thinglookatcursor")]
		public void ThingPointAtCursor() 
		{
			// Make list of selected things
			List<Thing> selected = new List<Thing>(General.Map.Map.GetSelectedThings(true));
			if((selected.Count == 0) && (highlighted != null) && !highlighted.IsDisposed)
				selected.Add(highlighted);

			if(selected.Count == 0) 
			{
				General.Interface.DisplayStatus(StatusType.Warning, "This action requires a selection!");
				return;
			}

			//check mouse position
			if(!mousemappos.IsFinite()) 
			{
				awaitingMouseClick = true;
				General.Interface.DisplayStatus(StatusType.Warning, "Now click in the editing area!");
				return;
			}

			awaitingMouseClick = false;

			// Make undo
			if(selected.Count > 1) 
			{
				General.Map.UndoRedo.CreateUndo("Rotate " + selected.Count + " things");
				General.Interface.DisplayStatus(StatusType.Action, "Rotated " + selected.Count + " things.");
			} 
			else 
			{
				General.Map.UndoRedo.CreateUndo("Rotate thing");
				General.Interface.DisplayStatus(StatusType.Action, "Rotated a thing.");
			}

			//change angle
			if(General.Interface.CtrlState) //point away
			{ 
				foreach(Thing t in selected) 
				{
					ThingTypeInfo info = General.Map.Data.GetThingInfo(t.Type);
					if(info == null || info.Category == null || info.Category.Arrow == 0)
						continue;
					t.Rotate(Vector2D.GetAngle(mousemappos, t.Position) + Angle2D.PI);
				}
			} 
			else //point at
			{ 
				foreach(Thing t in selected) 
				{
					ThingTypeInfo info = General.Map.Data.GetThingInfo(t.Type);
					if(info == null || info.Category == null || info.Category.Arrow == 0)
						continue;
					t.Rotate(Vector2D.GetAngle(mousemappos, t.Position));
				}
			}

			// Redraw screen
			General.Interface.RedrawDisplay();
		}

		//mxd. rotate clockwise
		[BeginAction("rotateclockwise")]
		public void RotateCW() 
		{
			RotateThings(-5);
		}

		//mxd. rotate counterclockwise
		[BeginAction("rotatecounterclockwise")]
		public void RotateCCW() 
		{
			RotateThings(5);
		}

		//mxd
		private void RotateThings(int increment) 
		{
			// Make list of selected things
			List<Thing> selected = new List<Thing>(General.Map.Map.GetSelectedThings(true));
			if(selected.Count == 0 && highlighted != null && !highlighted.IsDisposed)
				selected.Add(highlighted);

			if(selected.Count == 0) 
			{
				General.Interface.DisplayStatus(StatusType.Warning, "This action requires a selection!");
				return;
			}

			// Make undo
			if(selected.Count > 1) 
			{
				General.Map.UndoRedo.CreateUndo("Rotate " + selected.Count + " things", this, UndoGroup.ThingAngleChange, CreateSelectionCRC(selected));
				General.Interface.DisplayStatus(StatusType.Action, "Rotated " + selected.Count + " things.");
			} 
			else 
			{
				General.Map.UndoRedo.CreateUndo("Rotate thing", this, UndoGroup.ThingAngleChange, CreateSelectionCRC(selected));
				General.Interface.DisplayStatus(StatusType.Action, "Rotated a thing.");
			}

			//change angle
			foreach(Thing t in selected) t.Rotate(General.ClampAngle(t.AngleDoom + increment));

			// Redraw screen
			General.Interface.RedrawDisplay();
			General.Interface.RefreshInfo();
		}

		//mxd
		[BeginAction("filterselectedthings")]
		public void ShowFilterDialog() 
		{
			ICollection<Thing> selection = General.Map.Map.GetSelectedThings(true);

			if (selection.Count == 0) 
			{
				General.Interface.DisplayStatus(StatusType.Warning, "This action requires a selection!");
				return;
			}

			new FilterSelectedThingsForm(selection, this).ShowDialog();
		}

		//mxd
		[BeginAction("selectsimilar")]
		public void SelectSimilar() 
		{
			ICollection<Thing> selection = General.Map.Map.GetSelectedThings(true);

			if(selection.Count == 0) 
			{
				General.Interface.DisplayStatus(StatusType.Warning, "This action requires a selection!");
				return;
			}

			var form = new SelectSimilarElementOptionsPanel();
			if (form.Setup(this)) form.ShowDialog();
		}

		#endregion
	}
}
