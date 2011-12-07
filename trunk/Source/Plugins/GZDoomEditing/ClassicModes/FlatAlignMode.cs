
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

namespace CodeImp.DoomBuilder.GZDoomEditing
{
	[EditMode(DisplayName = "Flat Align Mode",
			  SwitchAction = "flatalignmode",
			  ButtonImage = "VisualModeZ.png",
			  ButtonOrder = int.MinValue + 210,
			  ButtonGroup = "000_editing",
			  Volatile = true)]

	public class FlatAlignMode : BaseClassicMode
	{
		#region ================== Constants

		#endregion

		#region ================== Variables

		private ICollection<Sector> selection;

		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		public FlatAlignMode()
		{
		}

		#endregion

		#region ================== Methods

		#endregion

		#region ================== Events

		// Mode engages
		public override void OnEngage()
		{
			base.OnEngage();

			// Presentation
			renderer.SetPresentation(Presentation.Standard);

			// Selection
			General.Map.Map.ConvertSelection(SelectionType.Sectors);
			General.Map.Map.SelectionType = SelectionType.Sectors;
			if(General.Map.Map.SelectedSectorsCount == 0)
			{
				// Find the nearest linedef within highlight range
				Linedef l = General.Map.Map.NearestLinedef(mousemappos);
				if(l != null)
				{
					Sector selectsector = null;
					
					// Check on which side of the linedef the mouse is and which sector there is
					float side = l.SideOfLine(mousemappos);
					if((side > 0) && (l.Back != null))
						selectsector = l.Back.Sector;
					else if((side <= 0) && (l.Front != null))
						selectsector = l.Front.Sector;

					// Select the sector!
					if(selectsector != null)
					{
						selectsector.Selected = true;
						foreach(Sidedef sd in selectsector.Sidedefs)
							sd.Line.Selected = true;
					}
				}
			}
			
			// Get sector selection
			selection = General.Map.Map.GetSelectedSectors(true);
			if(selection.Count == 0)
			{
				General.Interface.MessageBeep(MessageBeepType.Default);
				General.Interface.DisplayStatus(StatusType.Info, "A selected sector is required for this action.");
				General.Editing.CancelMode();
			}

			// Find the nearest texture corner

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
			renderer.RedrawSurface();

			// Render lines
			if(renderer.StartPlotter(true))
			{
				renderer.PlotLinedefSet(General.Map.Map.Linedefs);
				renderer.PlotVerticesSet(General.Map.Map.Vertices);
				renderer.Finish();
			}

			// Render things
			if(renderer.StartThings(true))
			{
				renderer.RenderThingSet(General.Map.ThingsFilter.HiddenThings, Presentation.THINGS_HIDDEN_ALPHA);
				renderer.RenderThingSet(General.Map.ThingsFilter.VisibleThings, 1.0f);
				renderer.Finish();
			}

			// Render overlay
			if(renderer.StartOverlay(true))
			{
				renderer.Finish();
			}

			renderer.Present();
		}

		#endregion
	}
}
