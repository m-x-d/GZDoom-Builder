
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
using CodeImp.DoomBuilder.Data;
using CodeImp.DoomBuilder.Types;

#endregion

namespace CodeImp.DoomBuilder.BuilderModes
{
	[EditMode(DisplayName = "Brightness Mode",
			  SwitchAction = "brightnessmode",
			  ButtonImage = "BrightnessMode.png",
			  ButtonOrder = int.MinValue + 201,
			  ButtonGroup = "000_editing",
			  UseByDefault = true,
			  SafeStartMode = true)]
	
	public sealed class BrightnessMode : BaseClassicMode
	{
		#region ================== Enums

		private enum ModifyMode : int
		{
			None,
			Adjusting
		}

		#endregion

		#region ================== Constants

		#endregion

		#region ================== Variables
		
		// Highlighted item
		private Sector highlighted;
		
		// Labels
		private Dictionary<Sector, TextLabel[]> labels;

		// Interface
		private bool editpressed; //mxd
		
		// Modifying
		private ModifyMode mode;
		private Point editstartpos;
		private List<int> sectorbrightness;
		private int undoticket;
		
		#endregion
		
		#region ================== Properties

		public override object HighlightedObject { get { return highlighted; } }
		
		#endregion
		
		#region ================== Constructor / Disposer
		
		// Constructor
		public BrightnessMode()
		{
		}
		
		// Disposer
		public override void Dispose()
		{
			// Not already disposed?
			if(!isdisposed)
			{
				// Dispose old labels
				foreach(KeyValuePair<Sector, TextLabel[]> lbl in labels)
					foreach(TextLabel l in lbl.Value) l.Dispose();
				
				// Dispose base
				base.Dispose();
			}
		}

		#endregion

		#region ================== Methods
		
		// This sets up new labels
		private void SetupLabels()
		{
			if(labels != null)
			{
				// Dispose old labels
				foreach(KeyValuePair<Sector, TextLabel[]> lbl in labels)
					foreach(TextLabel l in lbl.Value) l.Dispose();
			}
			
			// Make text labels for sectors
			labels = new Dictionary<Sector, TextLabel[]>(General.Map.Map.Sectors.Count);
			foreach(Sector s in General.Map.Map.Sectors)
			{
				// Setup labels
				TextLabel[] labelarray = new TextLabel[s.Triangles.IslandVertices.Count];
				for(int i = 0; i < s.Triangles.IslandVertices.Count; i++)
				{
					Vector2D v = s.Labels[i].position;
					labelarray[i] = new TextLabel(20);
					labelarray[i].TransformCoords = true;
					labelarray[i].Rectangle = new RectangleF(v.x, v.y, 0.0f, 0.0f);
					labelarray[i].AlignX = TextAlignmentX.Center;
					labelarray[i].AlignY = TextAlignmentY.Middle;
					labelarray[i].Scale = 14f;
					labelarray[i].Color = General.Colors.Highlight.WithAlpha(255);
					labelarray[i].Backcolor = General.Colors.Background.WithAlpha(255);
				}
				labels.Add(s, labelarray);
			}
		}
		
		// This updates the overlay
		private void UpdateOverlay()
		{
			if(renderer.StartOverlay(true))
			{
				// Editing a selection?
				if(mode == ModifyMode.Adjusting)
				{
					// Go for all sectors that are being edited
					ICollection<Sector> orderedselection = General.Map.Map.GetSelectedSectors(true);
					foreach(Sector s in orderedselection)
					{
						// We use the overlay to dim the brightness of the sectors
						PixelColor c = PixelColor.FromInt(General.Map.Renderer2D.CalculateBrightness(s.Brightness));
						PixelColor brightnesscolor = new PixelColor((byte)(255 - c.r), 0, 0, 0);
						int brightnessint = brightnesscolor.ToInt();

						// Render the geometry
						FlatVertex[] verts = new FlatVertex[s.FlatVertices.Length];
						s.FlatVertices.CopyTo(verts, 0);
						for(int i = 0; i < verts.Length; i++) verts[i].c = brightnessint;
						renderer.RenderGeometry(verts, null, true);
					}
				}

				if(BuilderPlug.Me.ViewSelectionNumbers)
				{
					// Go for all sectors
					ICollection<Sector> orderedselection = General.Map.Map.GetSelectedSectors(true);
					foreach(Sector s in orderedselection)
					{
						// Render labels
						TextLabel[] labelarray = labels[s];
						for(int i = 0; i < s.Labels.Count; i++)
						{
							TextLabel l = labelarray[i];

							// Render only when enough space for the label to see
							float requiredsize = (l.TextSize.Height / 2) / renderer.Scale;
							if(requiredsize < s.Labels[i].radius) renderer.RenderText(l);
						}
					}
				}
				
				renderer.Finish();
			}
		}
		
		// This highlights a new item
		private void Highlight(Sector s)
		{
			// Highlight actually changes?
			if(s != highlighted)
			{
				// Update display
				if(renderer.StartPlotter(false))
				{
					if((highlighted != null) && !highlighted.IsDisposed)
					{
						// Undraw previous highlight
						renderer.PlotSector(highlighted);

						// Change label color
						TextLabel[] labelarray = labels[highlighted];
						foreach(TextLabel l in labelarray) l.Color = General.Colors.Selection;
					}

					// Set new highlight
					highlighted = s;

					if((highlighted != null) && !highlighted.IsDisposed)
					{
						// Render highlighted item
						renderer.PlotSector(highlighted, General.Colors.Highlight);

						// Change label color
						TextLabel[] labelarray = labels[highlighted];
						foreach(TextLabel l in labelarray) l.Color = General.Colors.Highlight;
					}

					renderer.Finish();
				}

				UpdateOverlay();
				renderer.Present();
			}
			
			// Show highlight info
			if((highlighted != null) && !highlighted.IsDisposed)
				General.Interface.ShowSectorInfo(highlighted);
			else
				General.Interface.HideInfo();
		}

		// This selectes or deselects a sector
		private void SelectSector(Sector s, bool selectstate, bool update)
		{
			bool selectionchanged = false;

			if(!s.IsDisposed)
			{
				// Select the sector?
				if(selectstate && !s.Selected)
				{
					s.Selected = true;
					selectionchanged = true;
					
					// Setup labels
					ICollection<Sector> orderedselection = General.Map.Map.GetSelectedSectors(true);
					TextLabel[] labelarray = labels[s];
					foreach(TextLabel l in labelarray)
					{
						l.Text = orderedselection.Count.ToString();
						l.Color = General.Colors.Selection;
					}
				}
				// Deselect the sector?
				else if(!selectstate && s.Selected)
				{
					s.Selected = false;
					selectionchanged = true;
					
					// Clear labels
					TextLabel[] labelarray = labels[s];
					foreach(TextLabel l in labelarray) l.Text = "";
					
					// Update all other labels
					UpdateSelectedLabels();
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
				
				if(update)
				{
					UpdateOverlay();
					renderer.Present();
				}
			}
		}
		
		// This updates labels from the selected sectors
		private void UpdateSelectedLabels()
		{
			// Update labels for editing mode?
			if(mode == ModifyMode.Adjusting)
			{
				// Go for all labels in all selected sectors
				ICollection<Sector> orderedselection = General.Map.Map.GetSelectedSectors(true);
				foreach(Sector s in orderedselection)
				{
					TextLabel[] labelarray = labels[s];
					foreach(TextLabel l in labelarray)
					{
						// Make sure the text and color are right
						int labelnum = s.Brightness;
						l.Text = labelnum.ToString();
						l.Color = General.Colors.Indication;
					}
				}
			}
			// Updating for normal mode
			else
			{
				// Go for all labels in all selected sectors
				int index = 0;
				ICollection<Sector> orderedselection = General.Map.Map.GetSelectedSectors(true);
				foreach(Sector s in orderedselection)
				{
					TextLabel[] labelarray = labels[s];
					foreach(TextLabel l in labelarray)
					{
						// Make sure the text and color are right
						int labelnum = index + 1;
						l.Text = labelnum.ToString();
						l.Color = General.Colors.Selection;
					}
					index++;
				}
			}
		}
		
		#endregion
		
		#region ================== Events

		public override void OnHelp()
		{
			General.ShowHelp("e_brightness.html");
		}

		// Mode engages
		public override void OnEngage()
		{
			base.OnEngage();

			// Add toolbar buttons
			General.Interface.AddButton(BuilderPlug.Me.MenusForm.ViewSelectionNumbers);
			General.Interface.AddButton(BuilderPlug.Me.MenusForm.SeparatorSectors1);
			General.Interface.AddButton(BuilderPlug.Me.MenusForm.MakeGradientBrightness);
			if(General.Map.UDMF) General.Interface.AddButton(BuilderPlug.Me.MenusForm.BrightnessGradientMode); //mxd
			General.Interface.AddButton(BuilderPlug.Me.MenusForm.MarqueSelectTouching); //mxd

			// Make custom presentation
			CustomPresentation p = new CustomPresentation();
			p.AddLayer(new PresentLayer(RendererLayer.Background, BlendingMode.Mask, General.Settings.BackgroundAlpha));
			p.AddLayer(new PresentLayer(RendererLayer.Grid, BlendingMode.Mask));
			p.AddLayer(new PresentLayer(RendererLayer.Surface, BlendingMode.Mask));
			p.AddLayer(new PresentLayer(RendererLayer.Overlay, BlendingMode.Alpha, 1f, true));
			//p.AddLayer(new PresentLayer(RendererLayer.Things, BlendingMode.Alpha, Presentation.THINGS_BACK_ALPHA, false));
			p.AddLayer(new PresentLayer(RendererLayer.Geometry, BlendingMode.Alpha, 1f, true));
			renderer.SetPresentation(p);
			
			// Make text labels for sectors
			SetupLabels();

			// Convert geometry selection to sectors only
			General.Map.Map.ConvertSelection(SelectionType.Sectors);
			
			// Update
			General.Map.Map.SelectionType = SelectionType.Sectors;
			UpdateSelectedLabels();
			UpdateOverlay();
		}

		// When disengaged
		public override void OnDisengage()
		{
			base.OnDisengage();

			// Remove toolbar buttons
			General.Interface.RemoveButton(BuilderPlug.Me.MenusForm.ViewSelectionNumbers);
			General.Interface.RemoveButton(BuilderPlug.Me.MenusForm.SeparatorSectors1);
			General.Interface.RemoveButton(BuilderPlug.Me.MenusForm.MakeGradientBrightness);
			if(General.Map.UDMF) General.Interface.RemoveButton(BuilderPlug.Me.MenusForm.BrightnessGradientMode); //mxd
			General.Interface.RemoveButton(BuilderPlug.Me.MenusForm.MarqueSelectTouching); //mxd

			// Keep only sectors selected
			General.Map.Map.ClearSelectedLinedefs();

			// Going to EditSelectionMode?
			if(General.Editing.NewMode is EditSelectionMode)
			{
				// Not pasting anything?
				EditSelectionMode editmode = (General.Editing.NewMode as EditSelectionMode);
				if(!editmode.Pasting)
				{
					// No selection made? But we have a highlight!
					if((General.Map.Map.GetSelectedSectors(true).Count == 0) && (highlighted != null))
					{
						// Make the highlight the selection
						SelectSector(highlighted, true, false);
					}
				}
			}

			// Hide highlight info
			General.Interface.HideInfo();
		}
		
		// This redraws the display
		public override void OnRedrawDisplay()
		{
			renderer.RedrawSurface();
			
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
				renderer.RenderThingSet(General.Map.ThingsFilter.HiddenThings, Presentation.THINGS_HIDDEN_ALPHA);
				renderer.RenderThingSet(General.Map.ThingsFilter.VisibleThings, 1.0f);
				renderer.Finish();
			}

			// Render overlay
			UpdateOverlay();

			renderer.Present();
		}
		
		// Mouse moves
		public override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);
			
			// Not in any editing mode?
			if(mode == ModifyMode.None) {

				//mxd
				if(selectpressed && !editpressed && !selecting) {
					// Check if moved enough pixels for multiselect
					Vector2D delta = mousedownpos - mousepos;
					if((Math.Abs(delta.x) > MULTISELECT_START_MOVE_PIXELS) ||
					   (Math.Abs(delta.y) > MULTISELECT_START_MOVE_PIXELS)) {
						// Start multiselecting
						StartMultiSelection();
					}
				} 
				else if(paintselectpressed && !editpressed && !selecting) //mxd. Drag-select
				{
					// Find the nearest linedef within highlight range
					Linedef l = General.Map.Map.NearestLinedefRange(mousemappos, BuilderPlug.Me.HighlightRange / renderer.Scale);
					Sector s = null;

					if(l != null) {
						// Check on which side of the linedef the mouse is
						float side = l.SideOfLine(mousemappos);
						if(side > 0) {
							// Is there a sidedef here?
							if(l.Back != null)
								s = l.Back.Sector;
						} else {
							// Is there a sidedef here?
							if(l.Front != null)
								s = l.Front.Sector;
						}

						if(s != null) {
							if(s != highlighted) {
								//toggle selected state
								highlighted = s;
								if(General.Interface.ShiftState ^ BuilderPlug.Me.AdditiveSelect)
									SelectSector(highlighted, true, true);
								else if(General.Interface.CtrlState)
									SelectSector(highlighted, false, true);
								else
									SelectSector(highlighted, !highlighted.Selected, true);

								// Update entire display
								General.Interface.RedrawDisplay();
							}
						} else if(highlighted != null) {
							highlighted = null;
							Highlight(null);

							// Update entire display
							General.Interface.RedrawDisplay();
						}
					}
				} 
				else if(e.Button == MouseButtons.None) 
				{
					// Find the nearest linedef within highlight range
					Linedef l = General.Map.Map.NearestLinedef(mousemappos);
					if(l != null) {
						// Check on which side of the linedef the mouse is
						float side = l.SideOfLine(mousemappos);
						if(side > 0) {
							// Is there a sidedef here?
							if(l.Back != null) {
								// Highlight if not the same
								if(l.Back.Sector != highlighted)
									Highlight(l.Back.Sector);
							} else {
								// Highlight nothing
								if(highlighted != null)
									Highlight(null);
							}
						} else {
							// Is there a sidedef here?
							if(l.Front != null) {
								// Highlight if not the same
								if(l.Front.Sector != highlighted)
									Highlight(l.Front.Sector);
							} else {
								// Highlight nothing
								if(highlighted != null)	Highlight(null);
							}
						}
					} 
					else 
					{
						// Highlight nothing
						if(highlighted != null)	Highlight(null);
					}
				}
			}
			// Adjusting mode?
			else if(mode == ModifyMode.Adjusting)
			{
				// Calculate change in position
				Point delta = Cursor.Position - new Size(editstartpos);

				if(General.Interface.ShiftState)
				{
					// Adjust selected sectors
					int index = 0;
					ICollection<Sector> orderedselection = General.Map.Map.GetSelectedSectors(true);
					foreach(Sector s in orderedselection)
					{
						int basebrightness = sectorbrightness[index];

						// Adjust brightness
						s.Brightness = basebrightness - delta.Y;
						if(s.Brightness > 255) s.Brightness = 255;
						if(s.Brightness < 0) s.Brightness = 0;
						index++;
					}
				}
				else
				{
					// Adjust selected sectors
					int index = 0;
					ICollection<Sector> orderedselection = General.Map.Map.GetSelectedSectors(true);
					foreach(Sector s in orderedselection)
					{
						int basebrightness = sectorbrightness[index];

						// Adjust brightness
						s.Brightness = General.Map.Config.BrightnessLevels.GetNearest(basebrightness - delta.Y);
						index++;
					}
				}
				
				// Update
				General.Interface.RefreshInfo();
				UpdateSelectedLabels();
				UpdateOverlay();
				renderer.Present();
			}
		}

		//mxd
		protected override void OnPaintSelectBegin() {
			highlighted = null;
			base.OnPaintSelectBegin();
		}
		
		// Selecting with mouse
		protected override void OnSelectBegin()
		{
			// Not modifying?
			if(mode == ModifyMode.None)
			{
				// Item highlighted?
				if((highlighted != null) && !highlighted.IsDisposed)
				{
					// Update display
					if(renderer.StartPlotter(false))
					{
						// Redraw highlight to show selection
						renderer.PlotSector(highlighted);
						renderer.Finish();
						renderer.Present();
					}
				}
			}
			
			base.OnSelectBegin();
		}
		
		// End selection
		protected override void OnSelectEnd()
		{
			// Not stopping from multiselection or modifying
			if(!selecting && (mode == ModifyMode.None))
			{
				// Item highlighted?
				if((highlighted != null) && !highlighted.IsDisposed)
				{
					//mxd. Flip selection
					SelectSector(highlighted, !highlighted.Selected, true);
					
					// Update display
					if(renderer.StartPlotter(false))
					{
						// Render highlighted item
						renderer.PlotSector(highlighted, General.Colors.Highlight);
						renderer.Finish();
					}

					// Update overlay
					TextLabel[] labelarray = labels[highlighted];
					foreach(TextLabel l in labelarray) l.Color = General.Colors.Highlight;
					UpdateOverlay();
					renderer.Present();
				//mxd
				} else if(BuilderPlug.Me.AutoClearSelection && General.Map.Map.SelectedSectorsCount > 0) {
					General.Map.Map.ClearSelectedLinedefs();
					General.Map.Map.ClearSelectedSectors();
					General.Interface.RedrawDisplay();
				}
			}

			base.OnSelectEnd();
		}
		
		// This is called wheh selection ends
		protected override void OnEndMultiSelection()
		{
			bool selectionvolume = ((Math.Abs(base.selectionrect.Width) > 0.1f) && (Math.Abs(base.selectionrect.Height) > 0.1f));

			if(selectionvolume)
			{
				//mxd. collect changed sectors
				if(marqueSelectionMode == MarqueSelectionMode.SELECT) {
					if(BuilderPlug.Me.MarqueSelectTouching) {
						//select sectors fully and partially inside selection, deselect all other sectors
						foreach(Sector s in General.Map.Map.Sectors) {
							bool select = false;

							foreach(Sidedef sd in s.Sidedefs) {
								if(selectionrect.Contains(sd.Line.Start.Position.x, sd.Line.Start.Position.y) || selectionrect.Contains(sd.Line.End.Position.x, sd.Line.End.Position.y)) {
									select = true;
									break;
								}
							}

							if(select && !s.Selected)
								SelectSector(s, true, false);
							else if(!select && s.Selected)
								SelectSector(s, false, false);
						}
					} else {
						//select sectors fully inside selection, deselect all other sectors
						foreach(Sector s in General.Map.Map.Sectors) {
							bool select = true;

							foreach(Sidedef sd in s.Sidedefs) {
								if(!selectionrect.Contains(sd.Line.Start.Position.x, sd.Line.Start.Position.y) || !selectionrect.Contains(sd.Line.End.Position.x, sd.Line.End.Position.y)) {
									select = false;
									break;
								}
							}

							if(select && !s.Selected)
								SelectSector(s, true, false);
							else if(!select && s.Selected)
								SelectSector(s, false, false);
						}
					}
				} else if(marqueSelectionMode == MarqueSelectionMode.ADD) { //additive selection
					if(BuilderPlug.Me.MarqueSelectTouching) {
						//select sectors fully and partially inside selection, leave others untouched 
						foreach(Sector s in General.Map.Map.Sectors) {
							if(s.Selected) continue;
							bool select = false;

							foreach(Sidedef sd in s.Sidedefs) {
								if(selectionrect.Contains(sd.Line.Start.Position.x, sd.Line.Start.Position.y) || selectionrect.Contains(sd.Line.End.Position.x, sd.Line.End.Position.y)) {
									select = true;
									break;
								}
							}

							if(select) SelectSector(s, true, false);
						}
					} else {
						//select sectors fully inside selection, leave others untouched 
						foreach(Sector s in General.Map.Map.Sectors) {
							if(s.Selected) continue;
							bool select = true;

							foreach(Sidedef sd in s.Sidedefs) {
								if(!selectionrect.Contains(sd.Line.Start.Position.x, sd.Line.Start.Position.y) || !selectionrect.Contains(sd.Line.End.Position.x, sd.Line.End.Position.y)) {
									select = false;
									break;
								}
							}

							if(select) SelectSector(s, true, false);
						}
					}

				} else if(marqueSelectionMode == MarqueSelectionMode.SUBTRACT) {
					if(BuilderPlug.Me.MarqueSelectTouching) {
						//deselect sectors fully and partially inside selection, leave others untouched 
						foreach(Sector s in General.Map.Map.Sectors) {
							if(!s.Selected) continue;
							bool deselect = false;

							foreach(Sidedef sd in s.Sidedefs) {
								if(selectionrect.Contains(sd.Line.Start.Position.x, sd.Line.Start.Position.y) || selectionrect.Contains(sd.Line.End.Position.x, sd.Line.End.Position.y)) {
									deselect = true;
									break;
								}
							}

							if(deselect) SelectSector(s, false, false);
						}
					} else {
						//deselect sectors fully inside selection, leave others untouched 
						foreach(Sector s in General.Map.Map.Sectors) {
							if(!s.Selected) continue;
							bool deselect = true;

							foreach(Sidedef sd in s.Sidedefs) {
								if(!selectionrect.Contains(sd.Line.Start.Position.x, sd.Line.Start.Position.y) || !selectionrect.Contains(sd.Line.End.Position.x, sd.Line.End.Position.y)) {
									deselect = false;
									break;
								}
							}

							if(deselect) SelectSector(s, false, false);
						}
					}

				} else { //should be Intersect
					if(BuilderPlug.Me.MarqueSelectTouching) {
						//deselect sectors which are fully outside selection
						foreach(Sector s in General.Map.Map.Sectors) {
							if(!s.Selected) continue;
							bool keep = false;

							foreach(Sidedef sd in s.Sidedefs) {
								if(selectionrect.Contains(sd.Line.Start.Position.x, sd.Line.Start.Position.y) || selectionrect.Contains(sd.Line.End.Position.x, sd.Line.End.Position.y)) {
									keep = true;
									break;
								}
							}

							if(!keep) SelectSector(s, false, false);
						}
					} else {
						//deselect sectors which are fully and partially outside selection
						foreach(Sector s in General.Map.Map.Sectors) {
							if(!s.Selected) continue;
							bool keep = true;

							foreach(Sidedef sd in s.Sidedefs) {
								if(!selectionrect.Contains(sd.Line.Start.Position.x, sd.Line.Start.Position.y) || !selectionrect.Contains(sd.Line.End.Position.x, sd.Line.End.Position.y)) {
									keep = false;
									break;
								}
							}

							if(!keep) SelectSector(s, false, false);
						}
					}
				}
				
				// Make sure all linedefs reflect selected sectors
				foreach(Sidedef sd in General.Map.Map.Sidedefs)
					if(!sd.Sector.Selected && ((sd.Other == null) || !sd.Other.Sector.Selected))
						sd.Line.Selected = false;
			}
			
			base.OnEndMultiSelection();
			UpdateOverlay();
			General.Interface.RedrawDisplay();
		}

		// This is called when the selection is updated
		protected override void OnUpdateMultiSelection()
		{
			base.OnUpdateMultiSelection();

			// Render selection
			UpdateOverlay();
			if(renderer.StartOverlay(false))
			{
				RenderMultiSelection();
				renderer.Finish();
				renderer.Present();
			}
		}
		
		
		// Editing
		protected override void OnEditBegin()
		{
			base.OnEditBegin();

			// Edit pressed in this mode
			editpressed = true; //mxd
			
			// No selection?
			ICollection<Sector> orderedselection = General.Map.Map.GetSelectedSectors(true);
			if(orderedselection.Count == 0)
			{
				// Make the highlight a selection if we have a highlight
				if((highlighted != null) && !highlighted.IsDisposed)
				{
					SelectSector(highlighted, true, false);
					orderedselection.Add(highlighted);
				}
			}
			
			// Anything selected?
			if(orderedselection.Count > 0)
			{
				// Create undo
				undoticket = General.Map.UndoRedo.CreateUndo("Adjust brightness");
				
				// Start editing
				mode = ModifyMode.Adjusting;
				editstartpos = Cursor.Position;
				
				// Keep sector brightness offsets and make the sector full brightness so we can use
				// the overlay to adjust the brightness. The surface is only updated here and again
				// with correct brightness when editing is done.
				sectorbrightness = new List<int>(orderedselection.Count);
				foreach(Sector s in orderedselection)
				{
					int realbrightness = s.Brightness;
					sectorbrightness.Add(realbrightness);
					s.Brightness = 255;
					s.UpdateCache();
					s.Brightness = realbrightness;
				}

				// Update surface to render full bright sectors
				renderer.RedrawSurface();

				// Update
				UpdateSelectedLabels();
				UpdateOverlay();
				renderer.Present();
			}
		}
		
		// Done editing
		protected override void OnEditEnd()
		{
			base.OnEditEnd();
			editpressed = false; //mxd
			
			// Stop editing
			mode = ModifyMode.None;
			sectorbrightness = null;
			
			// Nothing changed? Then withdraw the undo
			if(editstartpos.Y == Cursor.Position.Y)
			{
				if((General.Map.UndoRedo.NextUndo != null) &&
				   (General.Map.UndoRedo.NextUndo.TicketID == undoticket))
					General.Map.UndoRedo.WithdrawUndo();
			}
			
			// Update
			General.Map.Map.Update();
			UpdateSelectedLabels();
			General.Interface.RefreshInfo();
			General.Interface.RedrawDisplay();
			renderer.Present();
			
			// If only one sector was selected, deselect it
			ICollection<Sector> orderedselection = General.Map.Map.GetSelectedSectors(true);
			if(orderedselection.Count == 1) SelectSector(General.GetByIndex(orderedselection, 0), false, true);
		}

		// When undo is used
		public override bool OnUndoBegin()
		{
			// Clear selection
			General.Map.Map.ClearAllSelected();
			
			return base.OnUndoBegin();
		}
		
		// When undo is performed
		public override void OnUndoEnd()
		{
			// Clear labels
			SetupLabels();
		}
		
		// When redo is used
		public override bool OnRedoBegin()
		{
			// Clear selection
			General.Map.Map.ClearAllSelected();

			return base.OnRedoBegin();
		}
		
		// When redo is performed
		public override void OnRedoEnd()
		{
			// Clear labels
			SetupLabels();
		}
		
		#endregion
		
		#region ================== Actions
		
		[BeginAction("gradientbrightness")]
		public void MakeGradientBrightness()
		{
			// Need at least 3 selected sectors
			// The first and last are not modified
			ICollection<Sector> orderedselection = General.Map.Map.GetSelectedSectors(true);
			if(orderedselection.Count > 2) {
				General.Interface.DisplayStatus(StatusType.Action, "Created gradient brightness over selected sectors.");
				General.Map.UndoRedo.CreateUndo("Gradient brightness");

				//mxd
				Sector start = General.GetByIndex(orderedselection, 0);
				Sector end = General.GetByIndex(orderedselection, orderedselection.Count - 1);

				//mxd. Use UDMF light?
				if(General.Map.UDMF && (string)BuilderPlug.Me.MenusForm.BrightnessGradientMode.SelectedItem != MenusForm.BrightnessGradientModes.Sectors) {
					string lightKey = string.Empty;
					string lightAbsKey = string.Empty;
					float startbrightness, endbrightness;

					if((string)BuilderPlug.Me.MenusForm.BrightnessGradientMode.SelectedItem == MenusForm.BrightnessGradientModes.Ceilings) {
						lightKey = "lightceiling";
						lightAbsKey = "lightceilingabsolute";
					} else { //should be floors...
						lightKey = "lightfloor";
						lightAbsKey = "lightfloorabsolute";
					}

					//get total brightness of start sector
					if(start.Fields.GetValue(lightAbsKey, false))
						startbrightness = (float)start.Fields.GetValue(lightKey, 0);
					else
						startbrightness = Math.Min(255, Math.Max(0, (float)start.Brightness + start.Fields.GetValue(lightKey, 0)));

					//get total brightness of end sector
					if(end.Fields.GetValue(lightAbsKey, false))
						endbrightness = (float)end.Fields.GetValue(lightKey, 0);
					else
						endbrightness = Math.Min(255, Math.Max(0, (float)end.Brightness + end.Fields.GetValue(lightKey, 0)));

					float delta = endbrightness - startbrightness;

					// Go for all sectors in between first and last
					int index = 0;
					foreach(Sector s in orderedselection) {
						s.Fields.BeforeFieldsChange();
						float u = (float)index / (float)(orderedselection.Count - 1);
						float b = startbrightness + delta * u;

						//absolute flag set?
						if(s.Fields.GetValue(lightAbsKey, false)) {
							if(s.Fields.ContainsKey(lightKey))
								s.Fields[lightKey].Value = (int)b;
							else
								s.Fields.Add(lightKey, new UniValue(UniversalType.Integer, (int)b));
						} else {
							if(s.Fields.ContainsKey(lightKey))
								s.Fields[lightKey].Value = (int)b - s.Brightness;
							else
								s.Fields.Add(lightKey, new UniValue(UniversalType.Integer, (int)b - s.Brightness));
						}

						index++;
					}

				} else {
					float startbrightness = (float)start.Brightness;
					float endbrightness = (float)end.Brightness;
					float delta = endbrightness - startbrightness;

					// Go for all sectors in between first and last
					int index = 0;
					foreach(Sector s in orderedselection) {
						float u = (float)index / (float)(orderedselection.Count - 1);
						float b = startbrightness + delta * u;
						s.Brightness = (int)b;
						index++;
					}
				}
			} else {
				General.Interface.DisplayStatus(StatusType.Warning, "Select at least 3 sectors first!");
			}

			// Update
			General.Map.Map.Update();
			UpdateOverlay();
			renderer.Present();
			General.Interface.RedrawDisplay();
			General.Interface.RefreshInfo();
			General.Map.IsChanged = true;
		}
		
		// This clears the selection
		[BeginAction("clearselection", BaseAction = true)]
		public void ClearSelection()
		{
			// Clear selection
			General.Map.Map.ClearAllSelected();
			
			// Clear labels
			foreach(TextLabel[] labelarray in labels.Values)
				foreach(TextLabel l in labelarray) l.Text = "";
			
			// Redraw
			General.Interface.RedrawDisplay();
		}
		
		#endregion
	}
}
