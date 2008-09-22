
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
using CodeImp.DoomBuilder.Actions;
using CodeImp.DoomBuilder.Config;
using CodeImp.DoomBuilder.Types;

#endregion

namespace CodeImp.DoomBuilder.BuilderModes
{
	[EditMode(DisplayName = "Things",
			  SwitchAction = "thingsmode",		// Action name used to switch to this mode
			  ButtonDesc = "Things Mode",		// Description on the button in toolbar/menu
		      ButtonImage = "ThingsMode.png",	// Image resource name for the button
			  ButtonOrder = int.MinValue + 300)]	// Position of the button (lower is more to the left)

	public class ThingsMode : BaseClassicMode
	{
		#region ================== Constants

		public const float THING_HIGHLIGHT_RANGE = 10f;
		
		#endregion

		#region ================== Variables

		// Highlighted item
		private Thing highlighted;
		private Association[] association = new Association[Thing.NUM_ARGS];
		private Association highlightasso = new Association();

		// Interface
		private bool editpressed;
		
		#endregion

		#region ================== Properties

		#endregion

		#region ================== Constructor / Disposer

		#endregion

		#region ================== Methods

		// Cancel mode
		public override void OnCancel()
		{
			base.OnCancel();

			// Return to this mode
			General.Map.ChangeMode(new ThingsMode());
		}

		// Mode engages
		public override void OnEngage()
		{
			base.OnEngage();
			renderer.SetPresentation(Presentation.Things);

			// Convert geometry selection to linedefs selection
			General.Map.Map.ClearAllMarks(false);
			General.Map.Map.MarkSelectedVertices(true, true);
			ICollection<Linedef> lines = General.Map.Map.LinedefsFromMarkedVertices(false, true, false);
			foreach(Linedef l in lines) l.Selected = true;
			General.Map.Map.ClearSelectedSectors();
			General.Map.Map.ClearSelectedVertices();
		}

		// Mode disengages
		public override void OnDisengage()
		{
			base.OnDisengage();

			// Going to EditSelectionMode?
			if(General.Map.NewMode is EditSelectionMode)
			{
				// No selection made? But we have a highlight!
				if((General.Map.Map.GetSelectedThings(true).Count == 0) && (highlighted != null))
				{
					// Make the highlight the selection
					highlighted.Selected = true;
				}
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
				for(int i = 0; i < Thing.NUM_ARGS; i++) BuilderPlug.Me.PlotAssociations(renderer, association[i]);
				if((highlighted != null) && !highlighted.IsDisposed) BuilderPlug.Me.PlotReverseAssociations(renderer, highlightasso);
				renderer.Finish();
			}

			// Render things
			if(renderer.StartThings(true))
			{
				renderer.RenderThingSet(General.Map.ThingsFilter.HiddenThings, Presentation.THINGS_HIDDEN_ALPHA);
				renderer.RenderThingSet(General.Map.ThingsFilter.VisibleThings, 1.0f);
				for(int i = 0; i < Thing.NUM_ARGS; i++) BuilderPlug.Me.RenderAssociations(renderer, association[i]);
				if((highlighted != null) && !highlighted.IsDisposed)
				{
					BuilderPlug.Me.RenderReverseAssociations(renderer, highlightasso);
					renderer.RenderThing(highlighted, General.Colors.Highlight, 1.0f);
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
				highlightasso.Set(t.Tag, UniversalType.ThingTag);
			else
				highlightasso.Set(0, 0);

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
					association[i].Set(t.Args[i], action.Args[i].Type);
				else
					association[i].Set(0, 0);
				
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
		protected override void OnSelect()
		{
			// Item highlighted?
			if((highlighted != null) && !highlighted.IsDisposed)
			{
				// Flip selection
				highlighted.Selected = !highlighted.Selected;

				// Update display
				if(renderer.StartThings(false))
				{
					// Redraw highlight to show selection
					renderer.RenderThing(highlighted, renderer.DetermineThingColor(highlighted), 1.0f);
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
			// Not ending from a multi-selection?
			if(!selecting)
			{
				// Item highlighted?
				if((highlighted != null) && !highlighted.IsDisposed)
				{
					// Update display
					if(renderer.StartThings(false))
					{
						// Render highlighted item
						renderer.RenderThing(highlighted, General.Colors.Highlight, 1.0f);
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

			base.OnEdit();
		}

		// Done editing
		protected override void OnEndEdit()
		{
			// Edit pressed in this mode?
			if(editpressed)
			{
				// Anything selected?
				ICollection<Thing> selected = General.Map.Map.GetSelectedThings(true);
				if(selected.Count > 0)
				{
					// Show thing edit dialog
					General.Interface.ShowEditThings(selected);

					// When a single thing was selected, deselect it now
					if(selected.Count == 1) General.Map.Map.ClearSelectedThings();

					// Update things filter
					General.Map.ThingsFilter.Update();
					
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
				// Find the nearest vertex within highlight range
				Thing t = MapSet.NearestThingSquareRange(General.Map.ThingsFilter.VisibleThings, mousemappos, THING_HIGHLIGHT_RANGE / renderer.Scale);
				
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
						General.Map.Map.ClearSelectedThings();
						highlighted.Selected = true;
					}

					// Start dragging the selection
					General.Map.ChangeMode(new DragThingsMode(new ThingsMode(), mousedownmappos));
				}
			}
		}

		// This is called wheh selection ends
		protected override void OnEndMultiSelection()
		{
			// Go for all things
			foreach(Thing t in General.Map.ThingsFilter.VisibleThings)
			{
				t.Selected = ((t.Position.x >= selectionrect.Left) &&
							  (t.Position.y >= selectionrect.Top) &&
							  (t.Position.x <= selectionrect.Right) &&
							  (t.Position.y <= selectionrect.Bottom));
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
			}

			return base.OnCopyBegin();
		}

		// When pasting
		public override bool OnPasteBegin()
		{
			// No selection made? But we have a highlight!
			if((General.Map.Map.GetSelectedThings(true).Count == 0) && (highlighted != null))
			{
				// Make the highlight the selection
				highlighted.Selected = true;
			}

			return base.OnPasteBegin();
		}
		
		#endregion

		#region ================== Actions

		// This clears the selection
		[BeginAction("clearselection", BaseAction = true)]
		public void ClearSelection()
		{
			// Clear selection
			General.Map.Map.ClearAllSelected();

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
				// Create things at mouse position
				Thing t = General.Map.Map.CreateThing();
				General.Settings.ApplyDefaultThingSettings(t);
				t.Move(mousemappos);
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

				// Redraw screen
				General.Interface.RedrawDisplay();
			}
		}

		[BeginAction("deleteitem", BaseAction = true)]
		public void DeleteItem()
		{
			// Make list of selected things
			ICollection<Thing> selected = General.Map.Map.GetSelectedThings(true);
			if((selected.Count == 0) && (highlighted != null) && !highlighted.IsDisposed) selected.Add(highlighted);

			// Anything to do?
			if(selected.Count > 0)
			{
				// Make undo
				if(selected.Count > 1)
					General.Map.UndoRedo.CreateUndo("Delete " + selected.Count + " things", UndoGroup.None, 0);
				else
					General.Map.UndoRedo.CreateUndo("Delete thing", UndoGroup.None, 0);

				// Dispose selected things
				foreach(Thing t in selected) t.Dispose();
				
				// Update cache values
				General.Map.IsChanged = true;
				General.Map.Map.Update();

				// Invoke a new mousemove so that the highlighted item updates
				MouseEventArgs e = new MouseEventArgs(MouseButtons.None, 0, (int)mousepos.x, (int)mousepos.y, 0);
				OnMouseMove(e);

				// Redraw screen
				General.Interface.RedrawDisplay();
			}
		}
		
		#endregion
	}
}
