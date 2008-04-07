
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

namespace CodeImp.DoomBuilder.BuilderModes.Editing
{
	[EditMode(SwitchAction = "linedefsmode",	// Action name used to switch to this mode
			  ButtonDesc = "Linedefs Mode",		// Description on the button in toolbar/menu
			  ButtonImage = "LinesMode.png",	// Image resource name for the button
			  ButtonOrder = int.MinValue + 1)]	// Position of the button (lower is more to the left)

	public class LinedefsMode : ClassicMode
	{
		#region ================== Constants

		protected const float LINEDEF_HIGHLIGHT_RANGE = 20f;

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
		public override void Cancel()
		{
			base.Cancel();

			// Return to this mode
			General.Map.ChangeMode(new LinedefsMode());
		}

		// Mode engages
		public override void Engage()
		{
			base.Engage();
		}

		// Mode disengages
		public override void Disengage()
		{
			base.Disengage();

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
		public unsafe override void RedrawDisplay()
		{
			// Start with a clear display
			if(renderer.Start(true, true))
			{
				// Render things
				renderer.SetThingsRenderOrder(false);
				renderer.RenderThingSet(General.Map.Map.Things);
				
				// Render lines
				renderer.RenderLinedefSet(General.Map.Map.Linedefs);

				// Render highlighted item
				if((highlighted != null) && !highlighted.IsDisposed)
					renderer.RenderLinedef(highlighted, General.Colors.Highlight);

				// Render vertices
				renderer.RenderVerticesSet(General.Map.Map.Vertices);

				// Done
				renderer.Finish();
			}
		}

		// This highlights a new item
		protected void Highlight(Linedef l)
		{
			// Update display
			if(renderer.Start(false, false))
			{
				// Undraw previous highlight
				if((highlighted != null) && !highlighted.IsDisposed)
				{
					renderer.RenderLinedef(highlighted, renderer.DetermineLinedefColor(highlighted));
					renderer.RenderVertex(highlighted.Start, renderer.DetermineVertexColor(highlighted.Start));
					renderer.RenderVertex(highlighted.End, renderer.DetermineVertexColor(highlighted.End));
				}
				
				// Set new highlight
				highlighted = l;

				// Render highlighted item
				if((highlighted != null) && !highlighted.IsDisposed)
				{
					renderer.RenderLinedef(highlighted, General.Colors.Highlight);
					renderer.RenderVertex(highlighted.Start, renderer.DetermineVertexColor(highlighted.Start));
					renderer.RenderVertex(highlighted.End, renderer.DetermineVertexColor(highlighted.End));
				}
				
				// Done
				renderer.Finish();
			}
			
			// Show highlight info
			if((highlighted != null) && !highlighted.IsDisposed)
				General.Interface.ShowLinedefInfo(highlighted);
			else
				General.Interface.HideInfo();
		}

		// Mouse moves
		public override void MouseMove(MouseEventArgs e)
		{
			base.MouseMove(e);

			// Find the nearest linedef within highlight range
			Linedef l = General.Map.Map.NearestLinedefRange(mousemappos, LINEDEF_HIGHLIGHT_RANGE / renderer.Scale);

			// Highlight if not the same
			if(l != highlighted) Highlight(l);
		}

		// Mouse leaves
		public override void MouseLeave(EventArgs e)
		{
			base.MouseLeave(e);

			// Highlight nothing
			Highlight(null);
		}

		// Mouse button pressed
		public override void MouseDown(MouseEventArgs e)
		{
			base.MouseDown(e);

			// Select button?
			if(e.Button == EditMode.SELECT_BUTTON)
			{
				// Item highlighted?
				if((highlighted != null) && !highlighted.IsDisposed)
				{
					// Flip selection
					highlighted.Selected = !highlighted.Selected;

					// Update display
					if(renderer.Start(false, false))
					{
						// Redraw highlight to show selection
						renderer.RenderLinedef(highlighted, renderer.DetermineLinedefColor(highlighted));
						renderer.RenderVertex(highlighted.Start, renderer.DetermineVertexColor(highlighted.Start));
						renderer.RenderVertex(highlighted.End, renderer.DetermineVertexColor(highlighted.End));
						renderer.Finish();
					}
				}
			}
			// Edit button?
			else if(e.Button == EditMode.EDIT_BUTTON)
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
					if(renderer.Start(false, false))
					{
						// Redraw highlight to show selection
						renderer.RenderLinedef(highlighted, renderer.DetermineLinedefColor(highlighted));
						renderer.RenderVertex(highlighted.Start, renderer.DetermineVertexColor(highlighted.Start));
						renderer.RenderVertex(highlighted.End, renderer.DetermineVertexColor(highlighted.End));
						renderer.Finish();
					}
				}
			}
		}

		// Mouse released
		public override void MouseUp(MouseEventArgs e)
		{
			ICollection<Linedef> selected;

			base.MouseUp(e);

			// Item highlighted?
			if((highlighted != null) && !highlighted.IsDisposed)
			{
				// Update display
				if(renderer.Start(false, false))
				{
					// Render highlighted item
					renderer.RenderLinedef(highlighted, General.Colors.Highlight);
					renderer.RenderVertex(highlighted.Start, renderer.DetermineVertexColor(highlighted.Start));
					renderer.RenderVertex(highlighted.End, renderer.DetermineVertexColor(highlighted.End));
					renderer.Finish();
				}

				// Edit button?
				if(e.Button == EditMode.EDIT_BUTTON)
				{
					// Anything selected?
					selected = General.Map.Map.GetLinedefsSelection(true);
					if(selected.Count > 0)
					{
						// Show line edit dialog
						General.Interface.ShowEditLinedefs(selected);
						
						// When a single line was selected, deselect it now
						if(selected.Count == 1) General.Map.Map.ClearSelectedLinedefs();

						// Update entire display
						General.Interface.RedrawDisplay();
					}
				}
			}
		}

		// Mouse wants to drag
		protected override void DragStart(MouseEventArgs e)
		{
			base.DragStart(e);

			// Which button is used?
			if(e.Button == EditMode.SELECT_BUTTON)
			{
				// Make selection

			}
			else if(e.Button == EditMode.EDIT_BUTTON)
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
		
		#endregion
	}
}
