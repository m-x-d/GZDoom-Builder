
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
using CodeImp.DoomBuilder.Controls;
using CodeImp.DoomBuilder.Data;

#endregion

namespace CodeImp.DoomBuilder.BuilderModes
{
	[EditMode(SwitchAction = "brightnessmode",
			  ButtonDesc = "Brightness Mode",
			  ButtonImage = "BrightnessMode.png",
			  ButtonOrder = int.MinValue + 3)]

	public class BrightnessMode : ClassicMode
	{
		#region ================== Constants

		#endregion

		#region ================== Variables

		// Highlighted item
		private Sector highlighted;

		// Interface
		private bool editpressed;

		// The methods GetSelected* and MarkSelected* on the MapSet do not
		// retain the order in which items were selected.
		// This list keeps in order while sectors are selected/deselected.
		private List<Sector> orderedselection;
		
		#endregion

		#region ================== Properties

		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		public BrightnessMode()
		{
			// Make lists
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

			// Make customized presentation
			CustomPresentation p = new CustomPresentation();
			p.AddLayer(new PresentLayer(RendererLayer.Background, BlendingMode.Mask));
			p.AddLayer(new PresentLayer(RendererLayer.Grid, BlendingMode.Mask));
			p.AddLayer(new PresentLayer(RendererLayer.Overlay, BlendingMode.Alpha, 1f, true));
			p.AddLayer(new PresentLayer(RendererLayer.Things, BlendingMode.Alpha, 0.3f, false));
			p.AddLayer(new PresentLayer(RendererLayer.Geometry, BlendingMode.Alpha, 1f, true));
			renderer.SetPresentation(p);
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
				renderer.RenderThingSet(General.Map.Map.Things);
				renderer.Finish();
			}

			// Render overlay
			UpdateOverlay();

			renderer.Present();
		}

		// This updates the overlay
		private void UpdateOverlay()
		{
			if(renderer.StartOverlay(true))
			{
				// Go for all sectors
				foreach(Sector s in General.Map.Map.Sectors)
				{
					// Determine color by brightness
					PixelColor brightnesscolor = new PixelColor(255, (byte)s.Brightness, (byte)s.Brightness, (byte)s.Brightness);
					int brightnessint = brightnesscolor.ToInt();

					// Load texture image
					ImageData texture = General.Map.Data.GetFlatImage(s.LongFloorTexture);
					if(!texture.IsLoaded) texture.LoadImage();

					// Make vertices
					FlatVertex[] verts = new FlatVertex[s.Vertices.Length];
					s.Vertices.CopyTo(verts, 0);
					for(int i = 0; i < verts.Length; i++)
					{
						verts[i].u = verts[i].x / texture.ScaledWidth;
						verts[i].v = verts[i].y / texture.ScaledHeight;
					}

					// Render the geometry
					renderer.RenderGeometry(verts, texture, true);
				}

				if(selecting) RenderMultiSelection();

				renderer.Finish();
			}
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
			
			base.OnEdit();
		}

		// Done editing
		protected override void OnEndEdit()
		{
			// Edit pressed in this mode?
			if(editpressed)
			{
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

			// Render overlay
			UpdateOverlay();
			renderer.Present();
		}
		
		#endregion

		#region ================== Actions

		#endregion
	}
}
