
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
	[EditMode(DisplayName = "Make Sectors",
			  SwitchAction = "makesectormode",
			  ButtonDesc = "Make Sectors Mode",		// Description on the button in toolbar/menu
			  ButtonImage = "NewSector2.png",	// Image resource name for the button
			  ButtonOrder = int.MinValue + 202)]	// Position of the button (lower is more to the left)

	public class MakeSectorMode : BaseClassicMode
	{
		#region ================== Constants

		private const double FLASH_DURATION = 500.0f;

		#endregion

		#region ================== Variables

		// Nearest sidedef
		private LinedefSide editside;
		private LinedefSide nearestside;
		private List<LinedefSide> allsides;
		private List<Linedef> alllines;
		
		// Flash polygon
		private FlatVertex[] flashpolygon;
		private float flashintensity;
		private double flashstarttime;
		
		// Interface
		protected bool selectpressed;
		protected bool editpressed;

		#endregion

		#region ================== Properties

		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		public MakeSectorMode()
		{
		}

		// Disposer
		public override void Dispose()
		{
			// Not already disposed?
			if(!isdisposed)
			{
				// Clean up
				nearestside = null;
				allsides = null;
				
				// Dispose base
				base.Dispose();
			}
		}

		#endregion

		#region ================== Methods

		// This draws the geometry
		private void DrawGeometry()
		{
			// Render lines and vertices
			if(renderer.StartPlotter(true))
			{
				renderer.PlotLinedefSet(General.Map.Map.Linedefs);

				// Render highlight
				if(alllines != null)
				{
					foreach(Linedef l in alllines) renderer.PlotLinedef(l, General.Colors.Highlight);
				}

				renderer.PlotVerticesSet(General.Map.Map.Vertices);
				renderer.Finish();
			}
		}

		// This draws the overlay
		private void DrawOverlay()
		{
			// Redraw overlay
			if(renderer.StartOverlay(true))
			{
				if((flashpolygon != null) && (flashintensity > 0.0f))
				{
					renderer.RenderGeometry(flashpolygon, null, true);
				}

				renderer.Finish();
			}
		}

		// This highlights a new region
		protected void Highlight(bool buttonspressed)
		{
			LinedefSide newnearest;

			// Mouse inside?
			if(mouseinside)
			{
				// Highlighting from a new sidedef?
				Linedef nl = General.Map.Map.NearestLinedef(mousemappos);
				float side = nl.SideOfLine(mousemappos);
				newnearest = new LinedefSide(nl, (side <= 0.0f));
				if(newnearest != nearestside)
				{
					// Only change when buttons are not pressed
					if(!buttonspressed || (editside == newnearest))
					{
						// Find new sector
						General.Interface.SetCursor(Cursors.AppStarting);
						nearestside = newnearest;
						allsides = SectorTools.FindPotentialSectorAt(mousemappos);
						if(allsides != null)
						{
							alllines = new List<Linedef>(allsides.Count);
							foreach(LinedefSide sd in allsides) alllines.Add(sd.Line);
						}
						else
						{
							alllines = null;
						}
						General.Interface.SetCursor(Cursors.Default);
					}
					else
					{
						// Don't highlight this one
						nearestside = null;
						allsides = null;
						alllines = null;
					}

					// Redraw overlay
					DrawGeometry();
					renderer.Present();
				}
			}
			else
			{
				// No valid region
				nearestside = null;
				allsides = null;
				alllines = null;

				// Redraw overlay
				DrawGeometry();
				renderer.Present();
			}
		}

		// Start select
		protected override void OnSelect()
		{
			// Select pressed in this mode
			selectpressed = true;
			editside = nearestside;
			base.OnEdit();
		}
		
		#endregion
		
		#region ================== Events

		// Cancel mode
		public override void OnCancel()
		{
			base.OnCancel();
			
			// Return to base mode
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
			p.AddLayer(new PresentLayer(RendererLayer.Things, BlendingMode.Alpha, Presentation.THINGS_BACK_ALPHA, false));
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

			// Stop processing
			General.Interface.SetProcessorState(false);
		}

		// This redraws the display
		public override void OnRedrawDisplay()
		{
			// Render lines and vertices
			DrawGeometry();

			// Render things
			if(renderer.StartThings(true))
			{
				renderer.RenderThingSet(General.Map.ThingsFilter.HiddenThings, Presentation.THINGS_HIDDEN_ALPHA);
				renderer.RenderThingSet(General.Map.ThingsFilter.VisibleThings, 1.0f);
				renderer.Finish();
			}

			// Render overlay
			DrawOverlay();
			renderer.Present();
		}

		// Done selecting
		protected override void OnEndSelect()
		{
			// Select was pressed in this mode?
			if(selectpressed && (editside == nearestside) && (nearestside != null))
			{
				// Possible to make a sector?
				if(allsides != null)
				{
					// Make the sector
					General.Interface.SetCursor(Cursors.WaitCursor);
					General.Settings.FindDefaultDrawSettings();
					General.Map.UndoRedo.CreateUndo("Make Sector", UndoGroup.None, 0);
					Sector s = SectorTools.MakeSector(allsides);
					General.Interface.SetCursor(Cursors.Default);
					
					// Quickly flash this sector to indicate it was created
					General.Map.IsChanged = true;
					General.Map.Map.Update();
					flashpolygon = new FlatVertex[s.Vertices.Length];
					s.Vertices.CopyTo(flashpolygon, 0);
					flashintensity = 1.0f;
					flashstarttime = (double)General.Clock.GetCurrentTime();
					General.Interface.SetProcessorState(true);

					// Redraw overlay
					DrawGeometry();
					DrawOverlay();
					renderer.Present();
				}
			}

			selectpressed = false;
			base.OnEndSelect();
		}
		
		// Start editing
		protected override void OnEdit()
		{
			// Edit pressed in this mode
			editpressed = true;
			editside = nearestside;
			base.OnEdit();
		}

		// Done editing
		protected override void OnEndEdit()
		{
			// Edit was pressed in this mode?
			if(editpressed && (editside == nearestside) && (nearestside != null))
			{
				// Possible to make a sector?
				if(allsides != null)
				{
					// Make the sector
					General.Interface.SetCursor(Cursors.WaitCursor);
					General.Settings.FindDefaultDrawSettings();
					General.Map.UndoRedo.CreateUndo("Make Sector", UndoGroup.None, 0);
					Sector s = SectorTools.MakeSector(allsides);
					General.Interface.SetCursor(Cursors.Default);

					// Edit the sector
					List<Sector> secs = new List<Sector>(); secs.Add(s);
					if(General.Interface.ShowEditSectors(secs) == DialogResult.OK)
					{
						// Quickly flash this sector to indicate it was created
						General.Map.IsChanged = true;
						General.Map.Map.Update();
						flashpolygon = new FlatVertex[s.Vertices.Length];
						s.Vertices.CopyTo(flashpolygon, 0);
						flashintensity = 1.0f;
						flashstarttime = (double)General.Clock.GetCurrentTime();
						General.Interface.SetProcessorState(true);
					}
					else
					{
						// Undo
						General.Map.UndoRedo.PerformUndo();
						General.Map.UndoRedo.ClearAllRedos();
					}

					// Redraw overlay
					DrawGeometry();
					DrawOverlay();
					renderer.Present();
				}
			}
			
			editpressed = false;
			base.OnEndEdit();
		}

		// Mouse moves
		public override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);

			// Highlight the region
			Highlight((e.Button != MouseButtons.None));
		}

		// Mouse leaves
		public override void OnMouseLeave(EventArgs e)
		{
			base.OnMouseLeave(e);

			// Highlight nothing
			Highlight(false);
		}

		// Processing
		public override void OnProcess()
		{
			base.OnProcess();

			// Process flash
			if(flashpolygon != null)
			{
				// Determine the intensity of the flash by time elapsed
				double curtime = (double)General.Clock.GetCurrentTime();;
				flashintensity = 1f - (float)((curtime - flashstarttime) / FLASH_DURATION);
				if(flashintensity > 0.0f)
				{
					// Update vertices in polygon
					PixelColor pc = new PixelColor((byte)(flashintensity * 255.0f), 255, 255, 255);
					int intcolor = pc.ToInt();
					for(int i = 0; i < flashpolygon.Length; i++) flashpolygon[i].c = intcolor;
				}
				else
				{
					// End of flash, trash the polygon
					flashpolygon = null;
					flashintensity = 0.0f;
					General.Interface.SetProcessorState(false);
				}

				// Redraw overlay
				DrawOverlay();
				renderer.Present();
			}
		}
		
		#endregion

		#region ================== Actions

		#endregion
	}
}

