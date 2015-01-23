
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
using System.Windows.Forms;
using System.ComponentModel;
using CodeImp.DoomBuilder.Actions;
using CodeImp.DoomBuilder.Windows;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Rendering;
using CodeImp.DoomBuilder.Editing;
using CodeImp.DoomBuilder.Controls;

#endregion

namespace CodeImp.DoomBuilder.SoundPropagationMode
{
	[EditMode(DisplayName = "Sound Environment Mode",
			  SwitchAction = "soundenvironmentmode",		// Action name used to switch to this mode
			  ButtonImage = "ZDoomSoundEnvironment.png",	// Image resource name for the button
			  ButtonOrder = int.MinValue + 501,	// Position of the button (lower is more to the left)
			  ButtonGroup = "000_editing",
			  UseByDefault = true,
			  SafeStartMode = false,
			  Volatile = false)]

	public class SoundEnvironmentMode : ClassicMode
	{
		#region ================== Variables

		// Highlighted item
		private Sector highlighted;
		private SoundEnvironment highlightedsoundenvironment;
		private Linedef highlightedline; //mxd

		// Interface
		private SoundEnvironmentPanel panel;
		private Docker docker;

		private BackgroundWorker worker;

		#endregion


		#region ================== Properties

		public override object HighlightedObject { get { return highlighted; } }

		#endregion

		#region ================== Constructor / Disposer

		// Disposer
		public override void Dispose()
		{
			// Not already disposed?
			if (!isdisposed)
			{
				// Dispose base
				base.Dispose();
			}
		}

		#endregion

		#region ================== Methods

		// This highlights a new item
		private void Highlight(Sector s)
		{
			// Set new highlight
			highlighted = s;
			highlightedsoundenvironment = null;

			if (highlighted != null)
			{
				foreach (SoundEnvironment se in BuilderPlug.Me.SoundEnvironments)
				{
					if (se.Sectors.Contains(highlighted))
					{
						highlightedsoundenvironment = se;
						break;
					}
				}
			}
			
			if (highlightedsoundenvironment != null)
			{
				panel.HighlightSoundEnvironment(highlightedsoundenvironment);
			}
			else
			{
				panel.HighlightSoundEnvironment(null);
			}

			// Show highlight info
			if ((highlighted != null) && !highlighted.IsDisposed)
				General.Interface.ShowSectorInfo(highlighted);
			else
				General.Interface.HideInfo();
		}

		private void UpdateData() 
		{
			BuilderPlug.Me.DataIsDirty = false;

			panel.SoundEnvironments.Nodes.Clear();

			// Only update if map has changed or the sound environments were never updated at all (i.e. first time engaging this mode)
			if((General.Map.IsChanged || !BuilderPlug.Me.SoundEnvironmentIsUpdated) && !worker.IsBusy) 
			{
				General.Interface.DisplayStatus(StatusType.Busy, "Updating sound environments");
				worker.RunWorkerAsync();
			} 
			else if(!worker.IsBusy) 
			{
				foreach(SoundEnvironment se in BuilderPlug.Me.SoundEnvironments)
					panel.AddSoundEnvironment(se);
			}
		}

		#endregion

		#region ================== Events

		public override void OnHelp()
		{
			General.ShowHelp("e_sectors.html");
		}

		// Cancel mode
		public override void OnCancel()
		{
			base.OnCancel();

			// Return to this mode
			General.Editing.ChangeMode(new SoundEnvironmentMode());
		}

		// Mode engages
		public override void OnEngage()
		{
			base.OnEngage();

			General.Interface.AddButton(BuilderPlug.Me.MenusForm.ColorConfiguration);

			panel = new SoundEnvironmentPanel();
			docker = new Docker("soundenvironments", "Sound Environments", panel);
			General.Interface.AddDocker(docker);
			General.Interface.SelectDocker(docker);

			worker = new BackgroundWorker();
			worker.WorkerReportsProgress = true;
			worker.WorkerSupportsCancellation = true;

			worker.DoWork += BuilderPlug.Me.UpdateSoundEnvironments;
			worker.ProgressChanged += worker_ProgressChanged;
			worker.RunWorkerCompleted += worker_RunWorkerCompleted;

			UpdateData();

			CustomPresentation presentation = new CustomPresentation();
			presentation.AddLayer(new PresentLayer(RendererLayer.Background, BlendingMode.Mask, General.Settings.BackgroundAlpha));
			presentation.AddLayer(new PresentLayer(RendererLayer.Grid, BlendingMode.Mask));
			presentation.AddLayer(new PresentLayer(RendererLayer.Overlay, BlendingMode.Alpha, 1f, true));
			presentation.AddLayer(new PresentLayer(RendererLayer.Things, BlendingMode.Alpha, 1.0f));
			presentation.AddLayer(new PresentLayer(RendererLayer.Geometry, BlendingMode.Alpha, 1f, true));
			renderer.SetPresentation(presentation);
		}

		//mxd. If a linedef is highlighted, toggle the sound blocking flag 
		protected override void OnSelectEnd() 
		{
			if(highlightedline == null || !General.Map.UDMF) return;

			// Make undo
			General.Map.UndoRedo.CreateUndo("Toggle Sound Zone Boundary");

			// Toggle flag
			highlightedline.SetFlag("zoneboundary", !highlightedline.IsFlagSet("zoneboundary"));

			// Update
			UpdateData();
			General.Interface.RedrawDisplay();
		}

		private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			General.Interface.DisplayStatus(StatusType.Ready, "Finished updating sound environments");
		}

		private void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
		{
			SoundEnvironment se = e.UserState as SoundEnvironment;
			General.Interface.DisplayStatus(StatusType.Busy, "Updating sound environments (" + e.ProgressPercentage + "%)");
			panel.AddSoundEnvironment(se);
			General.Interface.RedrawDisplay();
		}

		// Mode disengages
		public override void OnDisengage()
		{
			base.OnDisengage();

			worker.CancelAsync();
			General.Interface.RemoveButton(BuilderPlug.Me.MenusForm.ColorConfiguration);
			General.Interface.RemoveDocker(docker);

			// Hide highlight info
			General.Interface.HideInfo();
		}

		// This redraws the display
		public override void OnRedrawDisplay()
		{
			if (BuilderPlug.Me.DataIsDirty) UpdateData();

			// Render lines and vertices
			if (renderer.StartPlotter(true))
			{
				// Plot lines by hand, so that no coloring (line specials, 3D floors etc.) distracts from
				// the sound environments. Also don't draw the line's normal. They are not needed here anyway
				// and can make it harder to see the sound environment colors
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
				lock (BuilderPlug.Me.BlockingLinedefs)
				{
					foreach (Linedef ld in BuilderPlug.Me.BlockingLinedefs)
					{
						renderer.PlotLine(ld.Start.Position, ld.End.Position, BuilderPlug.Me.BlockSoundColor);
					}
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
			if (renderer.StartThings(true))
			{
				renderer.RenderThingSet(General.Map.ThingsFilter.HiddenThings, Presentation.THINGS_BACK_ALPHA);
				renderer.RenderThingSet(General.Map.ThingsFilter.VisibleThings, Presentation.THINGS_HIDDEN_ALPHA);

				lock (BuilderPlug.Me.SoundEnvironments)
				{
					foreach (SoundEnvironment se in BuilderPlug.Me.SoundEnvironments)
					{
						if (se.Things.Count > 0) renderer.RenderThingSet(se.Things, 1.0f);
					}
				}

				renderer.Finish();
			}

			// Render overlay geometry (sectors)
			if (BuilderPlug.Me.OverlayGeometry != null)
			{
				lock (BuilderPlug.Me.OverlayGeometry)
				{
					if (BuilderPlug.Me.OverlayGeometry.Length > 0 && renderer.StartOverlay(true))
					{
						renderer.RenderGeometry(BuilderPlug.Me.OverlayGeometry, General.Map.Data.WhiteTexture, true);
						renderer.Finish();
					}
				}
			}

			renderer.Present();
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
				//mxd. We are not interested in single-sided lines, unless they have zoneboundary flag...
				if(l != null && ((l.Front == null || l.Back == null) && (General.Map.UDMF && !l.IsFlagSet("zoneboundary"))))
				{
					l = null;
				}

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
