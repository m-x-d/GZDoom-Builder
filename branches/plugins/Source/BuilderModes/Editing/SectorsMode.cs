
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
	[EditMode(SwitchAction = "sectorsmode",
			  ButtonDesc = "Sectors Mode",
		      ButtonImage = "SectorsMode.png",
			  ButtonOrder = int.MinValue + 2)]
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
			General.MainWindow.HideInfo();
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
				
				// Render lines and vertices
				renderer.RenderLinedefSet(General.Map.Map.Linedefs);
				renderer.RenderVerticesSet(General.Map.Map.Vertices);

				// Render highlighted item
				if((highlighted != null) && !highlighted.IsDisposed)
					renderer.RenderSector(highlighted, General.Colors.Highlight);

				// Done
				renderer.Finish();
			}
		}

		// This highlights a new item
		protected void Highlight(Sector s)
		{
			// Update display
			if(renderer.Start(false, false))
			{
				// Undraw previous highlight
				if((highlighted != null) && !highlighted.IsDisposed)
					renderer.RenderSector(highlighted);

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
					renderer.RenderSector(highlighted, General.Colors.Highlight);

				/*
				// Render highlighted things
				if(highlighted != null)
					foreach(Thing t in highlighted.Things)
						renderer.RenderThing(t, General.Colors.Highlight);
				*/
				
				// Done
				renderer.Finish();
			}

			// Show highlight info
			if((highlighted != null) && !highlighted.IsDisposed)
				General.MainWindow.ShowSectorInfo(highlighted);
			else
				General.MainWindow.HideInfo();
		}
		
		// Mouse moves
		public override void MouseMove(MouseEventArgs e)
		{
			base.MouseMove(e);

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
			bool front, back;
			
			// Which button is used?
			if(e.Button == EditMode.SELECT_BUTTON)
			{
				// Item highlighted?
				if((highlighted != null) && !highlighted.IsDisposed)
				{
					// Flip selection
					highlighted.Selected = !highlighted.Selected;

					// Make update lines selection
					foreach(Sidedef sd in highlighted.Sidedefs)
					{
						if(sd.Line.Front != null) front = sd.Line.Front.Sector.Selected; else front = false;
						if(sd.Line.Back != null) back = sd.Line.Back.Sector.Selected; else back = false;
						sd.Line.Selected = front | back;
					}
					
					// Update display
					if(renderer.Start(false, false))
					{
						// Redraw highlight to show selection
						renderer.RenderSector(highlighted);
						renderer.Finish();
					}
				}
			}
		}

		// Mouse released
		public override void MouseUp(MouseEventArgs e)
		{
			base.MouseUp(e);

			// Item highlighted?
			if((highlighted != null) && !highlighted.IsDisposed)
			{
				// Update display
				if(renderer.Start(false, false))
				{
					// Render highlighted item
					renderer.RenderSector(highlighted, General.Colors.Highlight);
					renderer.Finish();
				}
			}
		}
		
		#endregion
	}
}
