
#region ================== Copyright (c) 2016 Boris Iwanski

/*
 * Copyright (c) 2016 Boris Iwanski https://github.com/biwa/automapmode
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
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Rendering;
using CodeImp.DoomBuilder.Editing;

#endregion

namespace CodeImp.DoomBuilder.AutomapMode
{
	[EditMode(DisplayName = "Automap Mode",
			  SwitchAction = "automapmode",	// Action name used to switch to this mode
			  ButtonImage = "automap.png",	// Image resource name for the button
			  ButtonOrder = int.MinValue + 503,	// Position of the button (lower is more to the bottom)
			  ButtonGroup = "000_editing",
			  UseByDefault = true,
			  SafeStartMode = true)]

	public class AutomapMode : ClassicMode
	{
		#region ================== Constants

		private const float LINE_LENGTH_SCALER = 0.001f; //mxd

		#endregion

		#region ================== Variables

		private CustomPresentation automappresentation;
		private List<Linedef> validlinedefs;

		// Highlighted item
		private Linedef highlighted;
		
		#endregion

		#region ================== Properties

		public override object HighlightedObject { get { return highlighted; } }
		
		#endregion

		#region ================== Constructor / Disposer

		#endregion

		#region ================== Methods

		// This highlights a new item
		private void Highlight(Linedef l)
		{
			// Update display
			if(renderer.StartPlotter(false))
			{
				// Undraw previous highlight
				if((highlighted != null) && !highlighted.IsDisposed)
				{
					PixelColor c = LinedefIsValid(highlighted) ? DetermineLinedefColor(highlighted) : new PixelColor(255, 0, 0, 0);
					renderer.PlotLine(highlighted.Start.Position, highlighted.End.Position, c, LINE_LENGTH_SCALER);
				}

				// Set new highlight
				highlighted = l;

				// Render highlighted item
				if((highlighted != null) && !highlighted.IsDisposed && LinedefIsValid(highlighted))
				{
					renderer.PlotLine(highlighted.Start.Position, highlighted.End.Position, General.Colors.InfoLine, LINE_LENGTH_SCALER);
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

		//mxd
		internal void UpdateValidLinedefs()
		{
			validlinedefs = new List<Linedef>();
			foreach(Linedef ld in General.Map.Map.Linedefs)
				if(LinedefIsValid(ld)) validlinedefs.Add(ld);
		}

		private static PixelColor DetermineLinedefColor(Linedef ld)
		{
			if(ld.IsFlagSet(BuilderPlug.Me.HiddenFlag))
				return new PixelColor(255, 192, 192, 192);

			if(ld.Back == null || ld.IsFlagSet(BuilderPlug.Me.SecretFlag))
				return new PixelColor(255, 252, 0, 0);

			if(ld.Front.Sector.FloorHeight != ld.Back.Sector.FloorHeight)
				return new PixelColor(255, 188, 120, 72);

			if(ld.Front.Sector.CeilHeight != ld.Back.Sector.CeilHeight)
				return new PixelColor(255, 252, 252, 0);

			if(ld.Front.Sector.CeilHeight == ld.Back.Sector.CeilHeight && ld.Front.Sector.FloorHeight == ld.Back.Sector.FloorHeight)
				return new PixelColor(255, 128, 128, 128);

			if(General.Interface.CtrlState)
				return new PixelColor(255, 192, 192, 192);

			return new PixelColor(255, 255, 255, 255);
		}

		private static bool LinedefIsValid(Linedef ld)
		{
			if(General.Interface.CtrlState) return true;
			if(ld.IsFlagSet(BuilderPlug.Me.HiddenFlag)) return false;
			if(ld.Back == null || ld.IsFlagSet(BuilderPlug.Me.SecretFlag)) return true;
			if(ld.Back != null && (ld.Front.Sector.FloorHeight != ld.Back.Sector.FloorHeight || ld.Front.Sector.CeilHeight != ld.Back.Sector.CeilHeight)) return true;

			return false;
		}

		#endregion
		
		#region ================== Events

		public override void OnHelp()
		{
			General.ShowHelp("/gzdb/features/classic_modes/mode_automap.html");
		}

		// Cancel mode
		public override void OnCancel()
		{
			base.OnCancel();

			// Return to this mode
			General.Editing.ChangeMode(new AutomapMode());
		}

		// Mode engages
		public override void OnEngage()
		{
			base.OnEngage();
			renderer.DrawMapCenter = false; //mxd

			// Automap presentation without the surfaces
			automappresentation = new CustomPresentation();
			automappresentation.AddLayer(new PresentLayer(RendererLayer.Background, BlendingMode.Mask, General.Settings.BackgroundAlpha));
			automappresentation.AddLayer(new PresentLayer(RendererLayer.Grid, BlendingMode.Mask));
			automappresentation.AddLayer(new PresentLayer(RendererLayer.Geometry, BlendingMode.Alpha, 1f, true));
			renderer.SetPresentation(automappresentation);

			UpdateValidLinedefs();
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
				foreach(Linedef ld in General.Map.Map.Linedefs)
				{
					if(LinedefIsValid(ld))
						renderer.PlotLine(ld.Start.Position, ld.End.Position, DetermineLinedefColor(ld), LINE_LENGTH_SCALER);
				}

				if((highlighted != null) && !highlighted.IsDisposed && LinedefIsValid(highlighted))
				{
					renderer.PlotLine(highlighted.Start.Position, highlighted.End.Position, General.Colors.InfoLine, LINE_LENGTH_SCALER);
				}

				renderer.Finish();
			}

			renderer.Present();
		}

		protected override void OnSelectEnd()
		{
			// Item highlighted?
			if((highlighted != null) && !highlighted.IsDisposed)
			{
				General.Map.UndoRedo.CreateUndo("Toggle linedef show as 1-sided on automap flag");

				// Toggle flag
				highlighted.SetFlag(BuilderPlug.Me.SecretFlag, !highlighted.IsFlagSet(BuilderPlug.Me.SecretFlag));
				UpdateValidLinedefs();
			}

			base.OnSelectEnd();
		}
		
		protected override void OnEditEnd()
		{
			// Item highlighted?
			if((highlighted != null) && !highlighted.IsDisposed)
			{
				General.Map.UndoRedo.CreateUndo("Toggle linedef not shown on automap flag");

				// Toggle flag
				highlighted.SetFlag(BuilderPlug.Me.HiddenFlag, !highlighted.IsFlagSet(BuilderPlug.Me.HiddenFlag));
				UpdateValidLinedefs();
				General.Interface.RedrawDisplay();
			}

			base.OnEditEnd();
		}
		
		// Mouse moves
		public override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);

			// Not holding any buttons?
			if(e.Button == MouseButtons.None)
			{
				// Find the nearest linedef within highlight range
				Linedef l = MapSet.NearestLinedefRange(validlinedefs, mousemappos, BuilderPlug.Me.HighlightRange / renderer.Scale);

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

		public override void OnKeyDown(KeyEventArgs e)
		{
			base.OnKeyDown(e);

			if(e.Control)
			{
				UpdateValidLinedefs();
				General.Interface.RedrawDisplay();
			}
		}

		public override void OnKeyUp(KeyEventArgs e)
		{
			base.OnKeyUp(e);

			if(!e.Control)
			{
				UpdateValidLinedefs();
				General.Interface.RedrawDisplay();
			}
		}

		#endregion
	}
}
