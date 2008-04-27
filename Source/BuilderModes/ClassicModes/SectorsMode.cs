
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
using System.Drawing;

#endregion

namespace CodeImp.DoomBuilder.BuilderModes.Editing
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
		private Sector highlighted;

		#endregion

		#region ================== Properties

		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		public SectorsMode()
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

		// This selectes or deselects a sector
		protected void SelectSector(Sector s, bool selectstate)
		{
			// Flip selection
			s.Selected = selectstate;

			// Make update lines selection
			foreach(Sidedef sd in s.Sidedefs)
			{
				bool front, back;
				if(sd.Line.Front != null) front = sd.Line.Front.Sector.Selected; else front = false;
				if(sd.Line.Back != null) back = sd.Line.Back.Sector.Selected; else back = false;
				sd.Line.Selected = front | back;
			}
		}
		
		// Cancel mode
		public override void Cancel()
		{
			base.Cancel();

			// Return to this mode
			General.Map.ChangeMode(new SectorsMode());
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
		public override void RedrawDisplay()
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
		protected override void Select()
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

			base.Select();
		}

		// End selection
		protected override void EndSelect()
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

			base.EndSelect();
		}

		// Start editing
		protected override void Edit()
		{
			// Item highlighted?
			if((highlighted != null) && !highlighted.IsDisposed)
			{
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

			base.Edit();
		}

		// Done editing
		protected override void EndEdit()
		{
			// Anything selected?
			ICollection<Sector> selected = General.Map.Map.GetSectorsSelection(true);
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

			base.EndEdit();
		}
		
		// Mouse moves
		public override void MouseMove(MouseEventArgs e)
		{
			base.MouseMove(e);

			// Not holding any buttons?
			if(e.Button == MouseButtons.None)
			{
				// Find the nearest linedef within highlight range
				Linedef l = General.Map.Map.NearestLinedef(mousemappos);

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
		}

		// Mouse leaves
		public override void MouseLeave(EventArgs e)
		{
			base.MouseLeave(e);

			// Highlight nothing
			Highlight(null);
		}

		// Mouse wants to drag
		protected override void DragStart(MouseEventArgs e)
		{
			base.DragStart(e);

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
					General.Map.ChangeMode(new DragSectorsMode(new SectorsMode(), mousedownmappos));
				}
			}
		}

		// This is called wheh selection ends
		protected override void EndMultiSelection()
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

			base.EndMultiSelection();
			if(renderer.StartOverlay(true)) renderer.Finish();
			General.Interface.RedrawDisplay();
		}

		// This is called when the selection is updated
		protected override void UpdateMultiSelection()
		{
			base.UpdateMultiSelection();

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
