
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
	[EditMode(SwitchAction = "linedefsmode",	// Action name used to switch to this mode
			  ButtonDesc = "Linedefs Mode",		// Description on the button in toolbar/menu
			  ButtonImage = "LinesMode.png",	// Image resource name for the button
			  ButtonOrder = int.MinValue + 1)]	// Position of the button (lower is more to the left)

	public class LinedefsMode : ClassicMode
	{
		#region ================== Constants

		public const float LINEDEF_HIGHLIGHT_RANGE = 20f;

		#endregion

		#region ================== Variables

		// Highlighted item
		private Linedef highlighted;

		#endregion

		#region ================== Properties

		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		public LinedefsMode()
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
			General.Map.ChangeMode(new LinedefsMode());
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

			// Check which mode we are switching to
			if(General.Map.NewMode is VerticesMode)
			{
				// Convert selection to vertices

				// Clear selected linedefs
				General.Map.Map.ClearSelectedLinedefs();
			}
			else if(General.Map.NewMode is SectorsMode)
			{
				// Convert selection to sectors

				// Clear selected linedefs
				General.Map.Map.ClearSelectedLinedefs();
			}
			
			// Hide highlight info
			General.Interface.HideInfo();
		}

		// This redraws the display
		public override void OnRedrawDisplay()
		{
			// Render lines
			if(renderer.StartPlotter(true))
			{
				renderer.PlotLinedefSet(General.Map.Map.Linedefs);
				if((highlighted != null) && !highlighted.IsDisposed)
					renderer.PlotLinedef(highlighted, General.Colors.Highlight);
				renderer.PlotVerticesSet(General.Map.Map.Vertices);
				renderer.Finish();
			}

			// Render things
			if(renderer.StartThings(true))
			{
				renderer.SetThingsRenderOrder(false);
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
		protected void Highlight(Linedef l)
		{
			// Update display
			if(renderer.StartPlotter(false))
			{
				// Undraw previous highlight
				if((highlighted != null) && !highlighted.IsDisposed)
				{
					renderer.PlotLinedef(highlighted, renderer.DetermineLinedefColor(highlighted));
					renderer.PlotVertex(highlighted.Start, renderer.DetermineVertexColor(highlighted.Start));
					renderer.PlotVertex(highlighted.End, renderer.DetermineVertexColor(highlighted.End));
				}
				
				// Set new highlight
				highlighted = l;

				// Render highlighted item
				if((highlighted != null) && !highlighted.IsDisposed)
				{
					renderer.PlotLinedef(highlighted, General.Colors.Highlight);
					renderer.PlotVertex(highlighted.Start, renderer.DetermineVertexColor(highlighted.Start));
					renderer.PlotVertex(highlighted.End, renderer.DetermineVertexColor(highlighted.End));
				}
				
				// Done
				renderer.Finish();
				renderer.Present();
			}
			
			// Show highlight info
			if((highlighted != null) && !highlighted.IsDisposed)
				General.Interface.ShowLinedefInfo(highlighted);
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
				if(renderer.StartPlotter(false))
				{
					// Redraw highlight to show selection
					renderer.PlotLinedef(highlighted, renderer.DetermineLinedefColor(highlighted));
					renderer.PlotVertex(highlighted.Start, renderer.DetermineVertexColor(highlighted.Start));
					renderer.PlotVertex(highlighted.End, renderer.DetermineVertexColor(highlighted.End));
					renderer.Finish();
					renderer.Present();
				}
			}
			else
			{
				// Start rectangular selection
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
						renderer.PlotLinedef(highlighted, General.Colors.Highlight);
						renderer.PlotVertex(highlighted.Start, renderer.DetermineVertexColor(highlighted.Start));
						renderer.PlotVertex(highlighted.End, renderer.DetermineVertexColor(highlighted.End));
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
				// Highlighted item not selected?
				if(!highlighted.Selected)
				{
					// Make this the only selection
					General.Map.Map.ClearSelectedLinedefs();
					highlighted.Selected = true;
					General.Interface.RedrawDisplay();
				}

				// Update display
				if(renderer.StartPlotter(false))
				{
					// Redraw highlight to show selection
					renderer.PlotLinedef(highlighted, renderer.DetermineLinedefColor(highlighted));
					renderer.PlotVertex(highlighted.Start, renderer.DetermineVertexColor(highlighted.Start));
					renderer.PlotVertex(highlighted.End, renderer.DetermineVertexColor(highlighted.End));
					renderer.Finish();
					renderer.Present();
				}
			}

			base.OnEdit();
		}
		
		// Done editing
		protected override void OnEndEdit()
		{
			// Anything selected?
			ICollection<Linedef> selected = General.Map.Map.GetLinedefsSelection(true);
			if(selected.Count > 0)
			{
				// Show line edit dialog
				General.Interface.ShowEditLinedefs(selected);

				// When a single line was selected, deselect it now
				if(selected.Count == 1) General.Map.Map.ClearSelectedLinedefs();

				// Update entire display
				General.Interface.RedrawDisplay();
			}

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
				Linedef l = General.Map.Map.NearestLinedefRange(mousemappos, LINEDEF_HIGHLIGHT_RANGE / renderer.Scale);

				// Highlight if not the same
				if(l != highlighted) Highlight(l);
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
						// Select only this linedef for dragging
						General.Map.Map.ClearSelectedLinedefs();
						highlighted.Selected = true;
					}

					// Start dragging the selection
					General.Map.ChangeMode(new DragLinedefsMode(new LinedefsMode(), mousedownmappos));
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
