
#region ================== Copyright (c) 2007 Pascal vd Heiden, 2014 Boris Iwanski

/*
 * Copyright (c) 2007 Pascal vd Heiden, www.codeimp.com
 * Copyright (c) 2014 Boris Iwanski
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
using CodeImp.DoomBuilder.Actions;

#endregion

namespace CodeImp.DoomBuilder.SoundPropagationMode
{
	[EditMode(DisplayName = "Sound Propagation Mode",
			  SwitchAction = "soundpropagationmode",		// Action name used to switch to this mode
			  ButtonImage = "SoundPropagationIcon.png",	// Image resource name for the button
			  ButtonOrder = int.MinValue + 501,	// Position of the button (lower is more to the left)
			  ButtonGroup = "000_editing",
			  UseByDefault = true,
			  SafeStartMode = false,
			  Volatile = false)]

	public class SoundPropagationMode : ClassicMode
	{
		#region ================== Variables
		
		// Highlighted item
		private Sector highlighted;
		private Linedef highlightedline; //mxd

		private FlatVertex[] overlayGeometry;

		private List<Thing> huntingThings;
		private List<SoundPropagationDomain> propagationdomains;
		private Dictionary<Sector, SoundPropagationDomain> sector2domain;

		#endregion

		#region ================== Properties

		public override object HighlightedObject { get { return highlighted; } }
		internal static string BlockSoundFlag { get { return (General.Map.UDMF ? "blocksound" : "64"); } } //mxd

		#endregion

		#region ================== Constructor / Disposer

		// Disposer
		public override void Dispose()
		{
			// Not already disposed?
			if(!isdisposed)
			{
				// Dispose base
				base.Dispose();
			}
		}

		#endregion

		#region ================== Methods

		private void UpdateData()
		{
			BuilderPlug.Me.DataIsDirty = false;
			List<FlatVertex> vertsList = new List<FlatVertex>();

			// Go for all selected sectors
			foreach (Sector s in General.Map.Map.Sectors) vertsList.AddRange(s.FlatVertices);
			overlayGeometry = vertsList.ToArray();

			for (int i = 0; i < overlayGeometry.Length; i++)
				overlayGeometry[i].c = BuilderPlug.Me.NoSoundColor.WithAlpha(128).ToInt();
		}

		// This highlights a new item
		private void Highlight(Sector s)
		{
			// Set new highlight
			highlighted = s;

			UpdateSoundPropagation();

			// Show highlight info
			if ((highlighted != null) && !highlighted.IsDisposed)
				General.Interface.ShowSectorInfo(highlighted);
			else
				General.Interface.HideInfo();

			General.Interface.RedrawDisplay();
		}

		//mxd
		private void ResetSoundPropagation()
		{
			sector2domain.Clear();
			propagationdomains.Clear();
			BuilderPlug.Me.BlockingLinedefs.Clear();
			UpdateSoundPropagation();
		}

		private void UpdateSoundPropagation()
		{
			huntingThings.Clear();

			BuilderPlug.Me.BlockingLinedefs.Clear();

			foreach (Linedef ld in General.Map.Map.Linedefs)
				if (ld.IsFlagSet(BlockSoundFlag))
					BuilderPlug.Me.BlockingLinedefs.Add(ld);

			if (highlighted == null || highlighted.IsDisposed) return;

			if (!sector2domain.ContainsKey(highlighted))
			{
				SoundPropagationDomain spd = new SoundPropagationDomain(highlighted);
				foreach (Sector s in spd.Sectors) sector2domain[s] = spd;
				propagationdomains.Add(spd);
			}

			foreach (Sector adjacent in sector2domain[highlighted].AdjacentSectors)
			{
				if(!sector2domain.ContainsKey(adjacent))
				{
					SoundPropagationDomain aspd = new SoundPropagationDomain(adjacent);
					foreach (Sector s in aspd.Sectors) sector2domain[s] = aspd;
				}
			}

			//mxd. Create the list of sectors, which will be affected by noise made in highlighted sector
			SoundPropagationDomain curdomain = sector2domain[highlighted];
			Dictionary<int, Sector> noisysectors = new Dictionary<int, Sector>(curdomain.Sectors.Count);
			foreach(Sector s in curdomain.Sectors)
			{
				noisysectors.Add(s.Index, s);
			}

			foreach (Sector s in curdomain.AdjacentSectors)
			{
				SoundPropagationDomain aspd = sector2domain[s];
				foreach (Sector adjs in aspd.Sectors)
				{
					if(!noisysectors.ContainsKey(adjs.Index)) noisysectors.Add(adjs.Index, adjs);
				}
			}

			// Update the list of things that will actually go for the player when hearing a noise
			foreach (Thing thing in General.Map.Map.Things)
			{
				if(!General.Map.ThingsFilter.VisibleThings.Contains(thing)) continue;
				if(thing.IsFlagSet(General.Map.UDMF ? "ambush" : "8")) continue;
				if(thing.Sector == null) thing.DetermineSector();
				if(thing.Sector != null && noisysectors.ContainsKey(thing.Sector.Index)) huntingThings.Add(thing);
			}
		}

		#endregion
		
		#region ================== Events

		public override void OnHelp()
		{
			General.ShowHelp("gzdb/features/classic_modes/mode_soundpropagation.html");
		}

		// Cancel mode
		public override void OnCancel()
		{
			// Cancel base class
			base.OnCancel();

			// Return to previous mode
			General.Editing.ChangeMode(new SoundPropagationMode());
		}

		// Mode engages
		public override void OnEngage()
		{
			base.OnEngage();

			huntingThings = new List<Thing>();
			propagationdomains = new List<SoundPropagationDomain>();
			sector2domain = new Dictionary<Sector, SoundPropagationDomain>();
			BuilderPlug.Me.BlockingLinedefs = new List<Linedef>();

			UpdateData();

			General.Interface.AddButton(BuilderPlug.Me.MenusForm.ColorConfiguration);

			CustomPresentation presentation = new CustomPresentation();
			presentation.AddLayer(new PresentLayer(RendererLayer.Background, BlendingMode.Mask, General.Settings.BackgroundAlpha));
			presentation.AddLayer(new PresentLayer(RendererLayer.Grid, BlendingMode.Mask));
			presentation.AddLayer(new PresentLayer(RendererLayer.Overlay, BlendingMode.Alpha, 1.0f, true));
			presentation.AddLayer(new PresentLayer(RendererLayer.Things, BlendingMode.Alpha, 1.0f));
			presentation.AddLayer(new PresentLayer(RendererLayer.Geometry, BlendingMode.Alpha, 1.0f, true));
			renderer.SetPresentation(presentation);

			// Convert geometry selection to sectors only
			General.Map.Map.ConvertSelection(SelectionType.Sectors);

			UpdateSoundPropagation();
			General.Interface.RedrawDisplay();
		}

		// Mode disengages
		public override void OnDisengage()
		{
			base.OnDisengage();
			General.Interface.RemoveButton(BuilderPlug.Me.MenusForm.ColorConfiguration);

			// Hide highlight info
			General.Interface.HideInfo();
		}

		// This redraws the display
		public override void OnRedrawDisplay()
		{
			List<SoundPropagationDomain> renderedspds = new List<SoundPropagationDomain>();
			if (BuilderPlug.Me.DataIsDirty) UpdateData();

			// Render lines and vertices
			if(renderer.StartPlotter(true))
			{

				// Plot lines by hand, so that no coloring (line specials, 3D floors etc.) distracts from
				// the sound propagation. Also don't draw the line's normal. They are not needed here anyway
				// and can make it harder to see the sound environment propagation
				foreach (Linedef ld in General.Map.Map.Linedefs)
				{
					PixelColor c;

					if(ld.IsFlagSet(General.Map.Config.ImpassableFlag))
						c = General.Colors.Linedefs;
					else
						c = General.Colors.Linedefs.WithAlpha(General.Settings.DoubleSidedAlphaByte);

					renderer.PlotLine(ld.Start.Position, ld.End.Position, c);
				}

				// Since there will usually be way less blocking linedefs than total linedefs, it's presumably
				// faster to draw them on their own instead of checking if each linedef is in BlockingLinedefs
				foreach (Linedef ld in BuilderPlug.Me.BlockingLinedefs)
				{
					renderer.PlotLine(ld.Start.Position, ld.End.Position, BuilderPlug.Me.BlockSoundColor);
				}

				//mxd. Render highlighted line
				if(highlightedline != null)
				{
					renderer.PlotLine(highlightedline.Start.Position, highlightedline.End.Position, General.Colors.Highlight);
				}

				renderer.PlotVerticesSet(General.Map.Map.Vertices);

				renderer.Finish();
			}

			// Render things
			if(renderer.StartThings(true))
			{
				renderer.RenderThingSet(General.Map.ThingsFilter.HiddenThings, Presentation.THINGS_BACK_ALPHA);
				renderer.RenderThingSet(General.Map.ThingsFilter.VisibleThings, Presentation.THINGS_HIDDEN_ALPHA);
				foreach (Thing thing in huntingThings)
				{
					renderer.RenderThing(thing, General.Colors.Selection, Presentation.THINGS_ALPHA);
				}

				renderer.Finish();
			}

			if (renderer.StartOverlay(true))
			{
				renderer.RenderGeometry(overlayGeometry, General.Map.Data.WhiteTexture, true);

				if (highlighted != null && !highlighted.IsDisposed)
				{
					SoundPropagationDomain spd = sector2domain[highlighted];
					renderer.RenderGeometry(spd.Level1Geometry, General.Map.Data.WhiteTexture, true);

					foreach (Sector s in spd.AdjacentSectors)
					{
						SoundPropagationDomain aspd = sector2domain[s];

						if (!renderedspds.Contains(aspd))
						{
							renderer.RenderGeometry(aspd.Level2Geometry, General.Map.Data.WhiteTexture, true);
							renderedspds.Add(aspd);
						}
					}

					RenderColoredSector(highlighted, BuilderPlug.Me.HighlightColor.WithAlpha(128));
				}

				renderer.Finish();
			}
			
			renderer.Present();
		}

		private void RenderColoredSector(Sector sector, PixelColor color)
		{
			RenderColoredSector(sector.FlatVertices, color);
		}

		private void RenderColoredSector(FlatVertex[] flatvertices, PixelColor color)
		{
			FlatVertex[] fv = new FlatVertex[flatvertices.Length];
			flatvertices.CopyTo(fv, 0);
			for (int i = 0; i < fv.Length; i++) fv[i].c = color.ToInt();
			renderer.RenderGeometry(fv, General.Map.Data.WhiteTexture, true);
		}

		//mxd. If a linedef is highlighted, toggle the sound blocking flag 
		protected override void OnSelectEnd()
		{
			if(highlightedline == null) return;

			// Make undo
			General.Map.UndoRedo.CreateUndo("Toggle Linedef Sound Blocking");

			// Toggle flag
			highlightedline.SetFlag(BlockSoundFlag, !highlightedline.IsFlagSet(BlockSoundFlag));
			
			// Update
			ResetSoundPropagation();
			General.Interface.RedrawDisplay();
		}

		// Mouse moves
		public override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);

			// Not holding any buttons?
			if (e.Button == MouseButtons.None)
			{
				General.Interface.SetCursor(Cursors.Default);

				// Find the nearest linedef within highlight range
				Linedef l = General.Map.Map.NearestLinedef(mousemappos);
				if (l != null)
				{
					// Check on which side of the linedef the mouse is
					float side = l.SideOfLine(mousemappos);
					if (side > 0)
					{
						// Is there a sidedef here?
						if (l.Back != null)
						{
							// Highlight if not the same
							if (l.Back.Sector != highlighted) Highlight(l.Back.Sector);
						}
						else
						{
							// Highlight nothing
							Highlight(null);
						}
					}
					else
					{
						// Is there a sidedef here?
						if (l.Front != null)
						{
							// Highlight if not the same
							if (l.Front.Sector != highlighted) Highlight(l.Front.Sector);
						}
						else
						{
							// Highlight nothing
							Highlight(null);
						}
					}
				}
				else
				{
					// Highlight nothing
					Highlight(null);
				}

				//mxd. Find the nearest linedef within default highlight range
				l = General.Map.Map.NearestLinedefRange(mousemappos, 20 / renderer.Scale);
				//mxd. We are not interested in single-sided lines (unless they have "blocksound" flag set)...
				if(l != null && (l.Front == null || l.Back == null) && !l.IsFlagSet(BlockSoundFlag)) l = null;
				
				//mxd. Set as highlighted
				if(highlightedline != l)
				{
					highlightedline = l;
					General.Interface.RedrawDisplay();
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

		#endregion

		#region ================== Actions

		[BeginAction("soundpropagationcolorconfiguration")]
		public void ConfigureColors()
		{
			ColorConfiguration cc = new ColorConfiguration();
			if(cc.ShowDialog((Form)General.Interface) == DialogResult.OK) 
			{
				General.Interface.RedrawDisplay();
			}
		}

		#endregion
	}
}
