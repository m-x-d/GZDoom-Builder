
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
using CodeImp.DoomBuilder.Editing;

#endregion

namespace CodeImp.DoomBuilder.BuilderModes
{
	[EditMode(SwitchAction = "thingsmode",		// Action name used to switch to this mode
			  ButtonDesc = "Things Mode",		// Description on the button in toolbar/menu
		      ButtonImage = "ThingsMode.png",	// Image resource name for the button
			  ButtonOrder = int.MinValue + 3)]	// Position of the button (lower is more to the left)

	public class ThingsMode : ClassicMode
	{
		#region ================== Constants

		public const float THING_HIGHLIGHT_RANGE = 10f;

		#endregion

		#region ================== Variables

		// Highlighted item
		private Thing highlighted;

		// Interface
		private bool editpressed;
		
		#endregion

		#region ================== Properties

		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		public ThingsMode()
		{
		}

		// Disposer
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
		}

		// Mode disengages
		public override void OnDisengage()
		{
			base.OnDisengage();

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
					renderer.RenderThing(highlighted, General.Colors.Highlight);
				renderer.Finish();
			}

			// Render things
			if(renderer.StartThings(true))
			{
				renderer.SetThingsRenderOrder(true);
				renderer.RenderThingSet(General.Map.Map.Things);
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
			// Update display
			if(renderer.StartThings(false))
			{
				// Undraw previous highlight
				if((highlighted != null) && !highlighted.IsDisposed)
					renderer.RenderThing(highlighted, renderer.DetermineThingColor(highlighted));

				// Set new highlight
				highlighted = t;

				// Render highlighted item
				if((highlighted != null) && !highlighted.IsDisposed)
					renderer.RenderThing(highlighted, General.Colors.Highlight);

				// Done
				renderer.Finish();
				renderer.Present();
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
					renderer.RenderThing(highlighted, renderer.DetermineThingColor(highlighted));
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
						renderer.RenderThing(highlighted, General.Colors.Highlight);
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
					renderer.RenderThing(highlighted, renderer.DetermineThingColor(highlighted));
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
					// TODO

					// When a single thing was selected, deselect it now
					if(selected.Count == 1) General.Map.Map.ClearSelectedThings();

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
				Thing t = General.Map.Map.NearestThingSquareRange(mousemappos, THING_HIGHLIGHT_RANGE / renderer.Scale);

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
			foreach(Thing t in General.Map.Map.Things)
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
		
		#endregion
	}
}
