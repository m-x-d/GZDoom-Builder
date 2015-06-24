
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
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using CodeImp.DoomBuilder.Actions;
using CodeImp.DoomBuilder.BuilderModes.Interface;
using CodeImp.DoomBuilder.Config;
using CodeImp.DoomBuilder.Editing;
using CodeImp.DoomBuilder.Geometry;
using CodeImp.DoomBuilder.GZBuilder.Geometry;
using CodeImp.DoomBuilder.GZBuilder.Tools;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Rendering;
using CodeImp.DoomBuilder.Types;
using CodeImp.DoomBuilder.Windows;

#endregion

namespace CodeImp.DoomBuilder.BuilderModes
{
	[EditMode(DisplayName = "Sectors Mode",
			  SwitchAction = "sectorsmode",		// Action name used to switch to this mode
			  ButtonImage = "SectorsMode.png",	// Image resource name for the button
			  ButtonOrder = int.MinValue + 200,	// Position of the button (lower is more to the left)
			  ButtonGroup = "000_editing",
			  UseByDefault = true,
			  SafeStartMode = true)]

	public class SectorsMode : BaseClassicMode
	{
		#region ================== Constants

		#endregion

		#region ================== Variables

		// Highlighted item
		protected Sector highlighted;
		private Association highlightasso = new Association();

		// Interface
		protected bool editpressed;

		// Labels
		private Dictionary<Sector, TextLabel[]> labels;

		//mxd. Effects
		private readonly Dictionary<int, string[]> effects;
		
		//mxd. Cached overlays stuff
		private FlatVertex[] overlayGeometry;
		private Dictionary<Sector, string[]> selectedEffectLabels;
		private Dictionary<Sector, string[]> unselectedEffectLabels; 
		
		//mxd. "Make Door" textures
		private static string doortex = "-";
		private static string tracktex = "-";
		private static bool resetoffsets = true;
		
		#endregion

		#region ================== Properties

		public override object HighlightedObject { get { return highlighted; } }

		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		public SectorsMode()
		{
			//mxd
			effects = new Dictionary<int, string[]>();
			foreach (SectorEffectInfo info in General.Map.Config.SortedSectorEffects) 
			{
				string name = info.Index + ": " + info.Title;
				effects.Add(info.Index, new[] { name, "E" + info.Index });
			}
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

		// This makes a CRC for the selection
		public int CreateSelectionCRC()
		{
			CRC crc = new CRC();
			ICollection<Sector> orderedselection = General.Map.Map.GetSelectedSectors(true);
			crc.Add(orderedselection.Count);
			foreach(Sector s in orderedselection)
			{
				crc.Add(s.FixedIndex);
			}
			return (int)(crc.Value & 0xFFFFFFFF);
		}

		//mxd. This makes a CRC for given selection
		private static int CreateSelectionCRC(ICollection<Sector> selection) 
		{
			CRC crc = new CRC();
			crc.Add(selection.Count);
			foreach(Sector s in selection) crc.Add(s.FixedIndex);
			return (int)(crc.Value & 0xFFFFFFFF);
		}

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
				TextLabel[] labelarray = new TextLabel[s.Labels.Count];
				for(int i = 0; i < s.Labels.Count; i++)
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
				// Go for all selected sectors
				ICollection<Sector> orderedselection = General.Map.Map.GetSelectedSectors(true);
				
				//mxd. Render selected sectors
				if (BuilderPlug.Me.UseHighlight) 
				{
					renderer.RenderHighlight(overlayGeometry, General.Colors.Selection.WithAlpha(64).ToInt());
				}

				//mxd. Render highlighted sector
				if(BuilderPlug.Me.UseHighlight && highlighted != null) 
				{
					renderer.RenderHighlight(highlighted.FlatVertices, General.Colors.Highlight.WithAlpha(64).ToInt());
				}

				if (BuilderPlug.Me.ViewSelectionNumbers) 
				{
					foreach (Sector s in orderedselection) 
					{
						// Render labels
						TextLabel[] labelarray = labels[s];
						for (int i = 0; i < s.Labels.Count; i++) 
						{
							TextLabel l = labelarray[i];

							// Render only when enough space for the label to see
							float requiredsize = (l.TextSize.Height / 2) / renderer.Scale;
							if (requiredsize < s.Labels[i].radius) renderer.RenderText(l);
						}
					}
				}

				//mxd. Render effect labels
				if (BuilderPlug.Me.ViewSelectionEffects) 
				{
					if(!BuilderPlug.Me.ViewSelectionNumbers) RenderEffectLabels(selectedEffectLabels);
					RenderEffectLabels(unselectedEffectLabels);
				}
				
				renderer.Finish();
			}
		}

		//mxd
		private void RenderEffectLabels(Dictionary<Sector, string[]> labelsGroup) 
		{
			foreach(KeyValuePair<Sector, string[]> group in labelsGroup) 
			{
				// Render labels
				TextLabel[] labelarray = labels[group.Key];
				for(int i = 0; i < group.Key.Labels.Count; i++) 
				{
					TextLabel l = labelarray[i];
					l.Color = General.Colors.InfoLine;

					// Render only when enough space for the label to see
					float requiredsize = (General.Map.GetTextSize(group.Value[0], l.Scale).Width) / renderer.Scale;
					if(requiredsize > group.Key.Labels[i].radius) 
					{
						requiredsize = (General.Map.GetTextSize(group.Value[1], l.Scale).Width) / renderer.Scale;
						l.Text = (requiredsize > group.Key.Labels[i].radius ? "+" : group.Value[1]);
					} 
					else 
					{
						l.Text = group.Value[0];
					}

					renderer.RenderText(l);
				}
			}
		}

		//mxd
		private string[] GetEffectText(Sector s) 
		{
			string[] result = new []{string.Empty, string.Empty};

			if(s.Effect != 0) 
			{
				if(effects.ContainsKey(s.Effect)) 
				{
					if(s.Tag != 0) 
					{
						result[0] = "Tag " + s.Tag + ", " + effects[s.Effect][0];
						result[1] = "T" + s.Tag + " " + "E" + s.Effect;
					} 
					else 
					{
						result[0] = effects[s.Effect][0];
						result[1] = "E" + s.Effect;
					}
				} 
				else 
				{
					string effect = s.Effect + " - " + General.Map.Config.GetGeneralizedSectorEffectName(s.Effect);
					if(s.Tag != 0) 
					{
						result[0] = "Tag " + s.Tag + ", Effect " + effect;
						result[1] = "T" + s.Tag + " " + "E" + s.Effect;
					} 
					else 
					{
						result[0] = "Effect " + effect;
						result[1] = "E" + s.Effect;
					}
				}
			} 
			else if(s.Tag != 0) 
			{
				result[0] = "Tag " + s.Tag;
				result[1] = "T" + s.Tag;
			}

			return result;
		}

		//mxd
		private void UpdateOverlaySurfaces() 
		{
			ICollection<Sector> orderedselection = General.Map.Map.GetSelectedSectors(true);
			List<FlatVertex> vertsList = new List<FlatVertex>();
			
			// Go for all selected sectors
			foreach(Sector s in orderedselection) vertsList.AddRange(s.FlatVertices);
			overlayGeometry = vertsList.ToArray();
		}

		//mxd
		private void UpdateEffectLabels() 
		{
			selectedEffectLabels = new Dictionary<Sector, string[]>();
			unselectedEffectLabels = new Dictionary<Sector, string[]>();

			//update effect labels for selected sectors
			ICollection<Sector> orderedselection = General.Map.Map.GetSelectedSectors(true);
			foreach(Sector s in orderedselection) 
			{
				string[] labelText = GetEffectText(s);
				if(!string.IsNullOrEmpty(labelText[0])) 
					selectedEffectLabels.Add(s, labelText);
			}

			//update effect labels for unselected sectors
			orderedselection = General.Map.Map.GetSelectedSectors(false);
			foreach(Sector s in orderedselection) 
			{
				string[] labelText = GetEffectText(s);
				if(!string.IsNullOrEmpty(labelText[0]))
					unselectedEffectLabels.Add(s, labelText);
			}
		}
		
		// Support function for joining and merging sectors
		private void JoinMergeSectors(bool removelines)
		{
			// Remove lines in betwen joining sectors?
			if(removelines)
			{
				// Go for all selected linedefs
				List<Linedef> selectedlines = new List<Linedef>(General.Map.Map.GetSelectedLinedefs(true));
				foreach(Linedef ld in selectedlines)
				{
					// Front and back side?
					if((ld.Front != null) && (ld.Back != null))
					{
						// Both a selected sector, but not the same?
						if(ld.Front.Sector.Selected && ld.Back.Sector.Selected &&
						   (ld.Front.Sector != ld.Back.Sector))
						{
							// Remove this line
							ld.Dispose();
						}
					}
				}
			}

			// Find the first sector that is not disposed
			List<Sector> orderedselection = new List<Sector>(General.Map.Map.GetSelectedSectors(true));
			Sector first = null;
			foreach(Sector s in orderedselection)
				if(!s.IsDisposed) { first = s; break; }
			
			// Join all selected sectors with the first
			for(int i = 0; i < orderedselection.Count; i++)
				if((orderedselection[i] != first) && !orderedselection[i].IsDisposed)
					orderedselection[i].Join(first);

			// Clear selection
			General.Map.Map.ClearAllSelected();
			
			// Update
			General.Map.Map.Update();
			
			// Make text labels for sectors
			SetupLabels();
			UpdateSelectedLabels();
		}

		// This highlights a new item
		protected void Highlight(Sector s)
		{
			// Often we can get away by simply undrawing the previous
			// highlight and drawing the new highlight. But if associations
			// are or were drawn we need to redraw the entire display.

			// Previous association highlights something?
			bool completeredraw = (highlighted != null) && (highlighted.Tag > 0);

			// Set highlight association
			if (s != null) 
			{
				Vector2D center = (s.Labels.Count > 0 ? s.Labels[0].position : new Vector2D(s.BBox.X + s.BBox.Width / 2, s.BBox.Y + s.BBox.Height / 2));
				highlightasso.Set(center, s.Tag, UniversalType.SectorTag);
			} 
			else 
			{
				highlightasso.Set(new Vector2D(), 0, 0);
			}

			// New association highlights something?
			if((s != null) && (s.Tag > 0)) completeredraw = true;

			// Change label color
			if((highlighted != null) && !highlighted.IsDisposed)
			{
				TextLabel[] labelarray = labels[highlighted];
				foreach(TextLabel l in labelarray) l.Color = General.Colors.Selection;
			}
			
			// Change label color
			if((s != null) && !s.IsDisposed)
			{
				TextLabel[] labelarray = labels[s];
				foreach(TextLabel l in labelarray) l.Color = General.Colors.Highlight;
			}
			
			// If we're changing associations, then we
			// need to redraw the entire display
			if(completeredraw)
			{
				// Set new highlight and redraw completely
				highlighted = s;
				General.Interface.RedrawDisplay();
			}
			else
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
		protected void SelectSector(Sector s, bool selectstate, bool update)
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
					if(update) 
					{ 
						//mxd
						string selectedCount = General.Map.Map.SelectedSectorsCount.ToString();
						TextLabel[] labelarray = labels[s];
						foreach(TextLabel l in labelarray) 
						{
							l.Text = selectedCount;
							l.Color = General.Colors.Selection;
						}

						UpdateEffectLabels();
					}
				}
				// Deselect the sector?
				else if(!selectstate && s.Selected)
				{
					s.Selected = false;
					selectionchanged = true;

					// Clear labels
					if(update) 
					{
						TextLabel[] labelarray = labels[s];
						foreach(TextLabel l in labelarray) l.Text = "";

						// Update all other labels
						UpdateSelectedLabels();
					}
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

					//mxd. Also (de)select things?
					if (General.Interface.AltState) 
					{
						foreach(Thing t in General.Map.ThingsFilter.VisibleThings) 
						{
							t.DetermineSector();
							if(t.Sector != s) continue;
							t.Selected = s.Selected;
						}
					}

					if(update) 
					{
						UpdateOverlay();
						renderer.Present();
					}
				}
			}
		}

		// This updates labels from the selected sectors
		private void UpdateSelectedLabels()
		{
			// Go for all labels in all selected sectors
			ICollection<Sector> orderedselection = General.Map.Map.GetSelectedSectors(true);
			int index = 0;
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

			//mxd
			UpdateEffectLabels();
		}

		//mxd
		private bool IsInSelectionRect(Sector s, List<Line2D> selectionOutline) 
		{
			if(selectionrect.Contains(s.BBox)) return true;

			if(BuilderPlug.Me.MarqueSelectTouching && s.BBox.IntersectsWith(selectionrect)) 
			{
				//check endpoints
				foreach(Sidedef side in s.Sidedefs) 
				{
					if((selectionrect.Contains(side.Line.Start.Position.x, side.Line.Start.Position.y)
						|| selectionrect.Contains(side.Line.End.Position.x, side.Line.End.Position.y))) 
						return true;
				}

				//check line intersections
				foreach(Sidedef side in s.Sidedefs) 
				{
					foreach(Line2D line in selectionOutline) 
					{
						if(Line2D.GetIntersection(side.Line.Line, line)) return true;
					}
				}
			}

			return false;
		}

		//mxd
		public override void SelectMapElement(SelectableElement element) 
		{
			if(element is Sector) 
			{
				SelectSector(element as Sector, true, true);

				// Update overlay
				UpdateOverlaySurfaces();
				UpdateSelectionInfo();
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
			General.Editing.ChangeMode(new SectorsMode());
		}

		// Mode engages
		public override void OnEngage()
		{
			base.OnEngage();
			renderer.SetPresentation(Presentation.Standard);

			// Add toolbar buttons
			General.Interface.AddButton(BuilderPlug.Me.MenusForm.CopyProperties);
			General.Interface.AddButton(BuilderPlug.Me.MenusForm.PasteProperties);
			General.Interface.AddButton(BuilderPlug.Me.MenusForm.PastePropertiesOptions); //mxd
			General.Interface.AddButton(BuilderPlug.Me.MenusForm.SeparatorCopyPaste);
			General.Interface.AddButton(BuilderPlug.Me.MenusForm.ViewSelectionNumbers);
			General.Interface.AddButton(BuilderPlug.Me.MenusForm.ViewSelectionEffects);
			General.Interface.AddButton(BuilderPlug.Me.MenusForm.SeparatorSectors1);
			General.Interface.AddButton(BuilderPlug.Me.MenusForm.MakeDoor); //mxd
			General.Interface.AddButton(BuilderPlug.Me.MenusForm.SeparatorSectors2); //mxd
			General.Interface.AddButton(BuilderPlug.Me.MenusForm.MakeGradientBrightness);
			if(General.Map.UDMF) General.Interface.AddButton(BuilderPlug.Me.MenusForm.GradientModeMenu); //mxd
			General.Interface.AddButton(BuilderPlug.Me.MenusForm.GradientInterpolationMenu); //mxd
			General.Interface.AddButton(BuilderPlug.Me.MenusForm.MakeGradientFloors);
			General.Interface.AddButton(BuilderPlug.Me.MenusForm.MakeGradientCeilings);
			General.Interface.AddButton(BuilderPlug.Me.MenusForm.SeparatorSectors3); //mxd
			General.Interface.AddButton(BuilderPlug.Me.MenusForm.MarqueSelectTouching); //mxd
			General.Interface.AddButton(BuilderPlug.Me.MenusForm.DragThingsInSelectedSectors); //mxd
			if(General.Map.UDMF) General.Interface.AddButton(BuilderPlug.Me.MenusForm.TextureOffsetLock, ToolbarSection.Geometry); //mxd
			
			// Convert geometry selection to sectors only
			General.Map.Map.ConvertSelection(SelectionType.Sectors);

			// Make text labels for sectors
			SetupLabels();
			
			// Update
			UpdateSelectedLabels();
			UpdateOverlaySurfaces();//mxd
			UpdateSelectionInfo(); //mxd
			UpdateOverlay();
		}
		
		// Mode disengages
		public override void OnDisengage()
		{
			base.OnDisengage();

			// Remove toolbar buttons
			General.Interface.RemoveButton(BuilderPlug.Me.MenusForm.CopyProperties);
			General.Interface.RemoveButton(BuilderPlug.Me.MenusForm.PasteProperties);
			General.Interface.RemoveButton(BuilderPlug.Me.MenusForm.PastePropertiesOptions); //mxd
			General.Interface.RemoveButton(BuilderPlug.Me.MenusForm.SeparatorCopyPaste);
			General.Interface.RemoveButton(BuilderPlug.Me.MenusForm.ViewSelectionNumbers);
			General.Interface.RemoveButton(BuilderPlug.Me.MenusForm.ViewSelectionEffects);
			General.Interface.RemoveButton(BuilderPlug.Me.MenusForm.SeparatorSectors1);
			General.Interface.RemoveButton(BuilderPlug.Me.MenusForm.MakeDoor); //mxd
			General.Interface.RemoveButton(BuilderPlug.Me.MenusForm.SeparatorSectors2); //mxd
			General.Interface.RemoveButton(BuilderPlug.Me.MenusForm.MakeGradientBrightness);
			if(General.Map.UDMF) General.Interface.RemoveButton(BuilderPlug.Me.MenusForm.GradientModeMenu); //mxd
			General.Interface.RemoveButton(BuilderPlug.Me.MenusForm.GradientInterpolationMenu); //mxd
			General.Interface.RemoveButton(BuilderPlug.Me.MenusForm.MakeGradientFloors);
			General.Interface.RemoveButton(BuilderPlug.Me.MenusForm.MakeGradientCeilings);
			General.Interface.RemoveButton(BuilderPlug.Me.MenusForm.SeparatorSectors3); //mxd
			General.Interface.RemoveButton(BuilderPlug.Me.MenusForm.MarqueSelectTouching); //mxd
			General.Interface.RemoveButton(BuilderPlug.Me.MenusForm.DragThingsInSelectedSectors); //mxd
			if(General.Map.UDMF) General.Interface.RemoveButton(BuilderPlug.Me.MenusForm.TextureOffsetLock); //mxd
			
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
						UpdateSelectedLabels(); //mxd
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
			List<Line3D> eventlines = new List<Line3D>(); //mxd
			
			// Render lines and vertices
			if(renderer.StartPlotter(true))
			{
				renderer.PlotLinedefSet(General.Map.Map.Linedefs);
				renderer.PlotVerticesSet(General.Map.Map.Vertices);
				if((highlighted != null) && !highlighted.IsDisposed)
				{
					renderer.PlotSector(highlighted, General.Colors.Highlight);
					BuilderPlug.PlotReverseAssociations(renderer, highlightasso, eventlines);
				}
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

			// Render selection
			if(renderer.StartOverlay(false)) 
			{
				if(highlighted != null && !highlighted.IsDisposed) BuilderPlug.RenderReverseAssociations(renderer, highlightasso, eventlines); //mxd
				if(selecting) RenderMultiSelection();
				renderer.RenderArrows(eventlines); //mxd
				renderer.Finish();
			}
			
			renderer.Present();
		}

		// Selection
		protected override void OnSelectBegin()
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

			base.OnSelectBegin();
		}

		// End selection
		protected override void OnSelectEnd()
		{
			// Not stopping from multiselection?
			if(!selecting)
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
						renderer.Present();
					}

					// Update overlay
					TextLabel[] labelarray = labels[highlighted];
					foreach(TextLabel l in labelarray) l.Color = General.Colors.Highlight;
					UpdateOverlaySurfaces(); //mxd
					UpdateOverlay();
					renderer.Present();

					//mxd. Thing selection state may've changed
					if(General.Interface.AltState) General.Interface.RedrawDisplay();
				} 
				else if(BuilderPlug.Me.AutoClearSelection && General.Map.Map.SelectedSectorsCount > 0) //mxd
				{
					General.Map.Map.ClearSelectedLinedefs();
					General.Map.Map.ClearSelectedSectors();
					UpdateOverlaySurfaces(); //mxd
					General.Interface.RedrawDisplay();
				}

				UpdateSelectionInfo(); //mxd
			}

			base.OnSelectEnd();
		}

		// Start editing
		protected override void OnEditBegin()
		{
			// Item highlighted?
			if((highlighted != null) && !highlighted.IsDisposed)
			{
				// Edit pressed in this mode
				editpressed = true;

				// Highlighted item not selected?
				if(!highlighted.Selected && (BuilderPlug.Me.AutoClearSelection || (General.Map.Map.SelectedSectorsCount == 0)))
				{
					// Make this the only selection
					General.Map.Map.ClearSelectedSectors();
					General.Map.Map.ClearSelectedLinedefs();
					SelectSector(highlighted, true, false);
					UpdateSelectedLabels(); //mxd
					UpdateOverlaySurfaces(); //mxd
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
			else if(!selecting && BuilderPlug.Me.AutoDrawOnEdit) //mxd. We don't want to draw while multiselecting
			{
				// Start drawing mode
				DrawGeometryMode drawmode = new DrawGeometryMode();
				bool snaptogrid = General.Interface.ShiftState ^ General.Interface.SnapToGrid;
				bool snaptonearest = General.Interface.CtrlState ^ General.Interface.AutoMerge;
				DrawnVertex v = DrawGeometryMode.GetCurrentPosition(mousemappos, snaptonearest, snaptogrid, renderer, new List<DrawnVertex>());
				
				if(drawmode.DrawPointAt(v))
					General.Editing.ChangeMode(drawmode);
				else
					General.Interface.DisplayStatus(StatusType.Warning, "Failed to draw point: outside of map boundaries.");
			}
			
			base.OnEditBegin();
		}

		// Done editing
		protected override void OnEditEnd()
		{
			// Edit pressed in this mode?
			if(editpressed)
			{
				// Anything selected?
				ICollection<Sector> selected = General.Map.Map.GetSelectedSectors(true);
				if(selected.Count > 0)
				{
					if(General.Interface.IsActiveWindow)
					{
						//mxd. Show realtime vertex edit dialog
						General.Interface.OnEditFormValuesChanged += sectorEditForm_OnValuesChanged;
						DialogResult result = General.Interface.ShowEditSectors(selected);
						General.Interface.OnEditFormValuesChanged -= sectorEditForm_OnValuesChanged;

						General.Map.Renderer2D.UpdateExtraFloorFlag(); //mxd

						// When a single sector was selected, deselect it now
						if (selected.Count == 1) 
						{
							General.Map.Map.ClearSelectedSectors();
							General.Map.Map.ClearSelectedLinedefs();
							UpdateEffectLabels(); //mxd
						} 
						else if(result == DialogResult.Cancel) //mxd. Restore selection...
						{ 
							foreach (Sector s in selected) SelectSector(s, true, false);
							UpdateSelectedLabels(); //mxd
						}

						UpdateOverlaySurfaces(); //mxd
						General.Interface.RedrawDisplay();
					}
				}

				UpdateSelectionInfo(); //mxd
			}

			editpressed = false;
			base.OnEditEnd();
		}

		//mxd
		private void sectorEditForm_OnValuesChanged(object sender, EventArgs e) 
		{
			// Update entire display
			General.Map.Map.Update();
			General.Interface.RedrawDisplay();
		}
		
		// Mouse moves
		public override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);
			if(panning) return; //mxd. Skip all this jazz while panning

			//mxd
			if(selectpressed && !editpressed && !selecting) 
			{
				// Check if moved enough pixels for multiselect
				Vector2D delta = mousedownpos - mousepos;
				if((Math.Abs(delta.x) > MULTISELECT_START_MOVE_PIXELS) ||
				   (Math.Abs(delta.y) > MULTISELECT_START_MOVE_PIXELS)) 
				{
					// Start multiselecting
					StartMultiSelection();
				}
			}
			else if(paintselectpressed && !editpressed && !selecting) //mxd. Drag-select
			{
				// Find the nearest linedef within highlight range
				Linedef l = General.Map.Map.NearestLinedefRange(mousemappos, BuilderPlug.Me.HighlightRange / renderer.Scale);
				Sector s = null;

				if(l != null) 
				{
					// Check on which side of the linedef the mouse is
					float side = l.SideOfLine(mousemappos);
					if(side > 0) 
					{
						// Is there a sidedef here?
						if(l.Back != null) s = l.Back.Sector;
					} 
					else 
					{
						// Is there a sidedef here?
						if(l.Front != null) s = l.Front.Sector;
					}

					if(s != null) 
					{
						if(s != highlighted) 
						{
							//toggle selected state
							highlighted = s;
							if(General.Interface.ShiftState ^ BuilderPlug.Me.AdditiveSelect)
								SelectSector(highlighted, true, true);
							else if(General.Interface.CtrlState)
								SelectSector(highlighted, false, true);
							else
								SelectSector(highlighted, !highlighted.Selected, true);

							// Update entire display
							UpdateOverlaySurfaces();//mxd
							General.Interface.RedrawDisplay();
						}
					} 
					else if(highlighted != null) 
					{
						highlighted = null;
						Highlight(null);

						// Update entire display
						General.Interface.RedrawDisplay();
					}

					UpdateSelectionInfo(); //mxd
				}
			} 
			else if(e.Button == MouseButtons.None) // Not holding any buttons?
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

		//mxd
		protected override void OnPaintSelectBegin() 
		{
			if(highlighted != null) 
			{
				if(General.Interface.ShiftState ^ BuilderPlug.Me.AdditiveSelect)
					SelectSector(highlighted, true, true);
				else if(General.Interface.CtrlState)
					SelectSector(highlighted, false, true);
				else
					SelectSector(highlighted, !highlighted.Selected, true);

				// Update entire display
				UpdateOverlaySurfaces();//mxd
				General.Interface.RedrawDisplay();
			}

			base.OnPaintSelectBegin();
		}

		// Mouse wants to drag
		protected override void OnDragStart(MouseEventArgs e)
		{
			base.OnDragStart(e);

			// Edit button used?
			if(General.Actions.CheckActionActive(null, "classicedit"))
			{
				// Anything highlighted?
				if((highlighted != null) && !highlighted.IsDisposed)
				{
					// Highlighted item not selected?
					if(!highlighted.Selected)
					{
						// Select only this sector for dragging
						General.Map.Map.ClearSelectedSectors();
						SelectSector(highlighted, true, true);
						UpdateOverlaySurfaces(); //mxd
					}

					// Start dragging the selection
					if(!BuilderPlug.Me.DontMoveGeometryOutsideMapBoundary || CanDrag()) //mxd
						General.Editing.ChangeMode(new DragSectorsMode(mousedownmappos));
				}
			}
		}

		//mxd. Check if any selected sector is outside of map boundary
		private bool CanDrag() 
		{
			ICollection<Sector> selectedsectors = General.Map.Map.GetSelectedSectors(true);
			int unaffectedCount = 0;

			foreach(Sector s in selectedsectors) 
			{
				// Make sure the sector is inside the map boundary
				foreach(Sidedef sd in s.Sidedefs) 
				{
					if(sd.Line.Start.Position.x < General.Map.Config.LeftBoundary || sd.Line.Start.Position.x > General.Map.Config.RightBoundary
						|| sd.Line.Start.Position.y > General.Map.Config.TopBoundary || sd.Line.Start.Position.y < General.Map.Config.BottomBoundary
						|| sd.Line.End.Position.x < General.Map.Config.LeftBoundary || sd.Line.End.Position.x > General.Map.Config.RightBoundary
						|| sd.Line.End.Position.y > General.Map.Config.TopBoundary || sd.Line.End.Position.y < General.Map.Config.BottomBoundary) 
					{
						SelectSector(s, false, false);
						unaffectedCount++;
						break;
					}
				}
			}

			if(unaffectedCount == selectedsectors.Count) 
			{
				General.Interface.DisplayStatus(StatusType.Warning, "Unable to drag selection: " + (selectedsectors.Count == 1 ? "selected sector is" : "all of selected sectors are") + " outside of map boundary!");
				General.Interface.RedrawDisplay();
				return false;
			}

			if(unaffectedCount > 0)
				General.Interface.DisplayStatus(StatusType.Warning, unaffectedCount + " of selected sectors " + (unaffectedCount == 1 ? "is" : "are") + " outside of map boundary!");

			UpdateSelectedLabels(); //mxd
			return true;
		}

		// This is called wheh selection ends
		protected override void OnEndMultiSelection()
		{
			bool selectionvolume = ((Math.Abs(base.selectionrect.Width) > 0.1f) && (Math.Abs(base.selectionrect.Height) > 0.1f));

			if(selectionvolume)
			{
				List<Line2D> selectionOutline = new List<Line2D> {
					new Line2D(selectionrect.Left, selectionrect.Top, selectionrect.Right, selectionrect.Top),
					new Line2D(selectionrect.Right, selectionrect.Top, selectionrect.Right, selectionrect.Bottom),
					new Line2D(selectionrect.Left, selectionrect.Bottom, selectionrect.Right, selectionrect.Bottom),
					new Line2D(selectionrect.Left, selectionrect.Bottom, selectionrect.Left, selectionrect.Top)
				};
				
				//mxd. collect changed sectors
				switch(marqueSelectionMode) 
				{
					case MarqueSelectionMode.SELECT:
						bool select;
						foreach(Sector s in General.Map.Map.Sectors) 
						{
							select = IsInSelectionRect(s, selectionOutline);

							if(select && !s.Selected) SelectSector(s, true, false);
							else if(!select && s.Selected) SelectSector(s, false, false);
						}
						if (marqueSelectionIncludesThings) 
						{
							ICollection<Sector> selected = General.Map.Map.GetSelectedSectors(true);

							foreach (Thing t in General.Map.ThingsFilter.VisibleThings) 
							{
								t.DetermineSector();
								if(t.Sector == null) continue;
								t.Selected = selectionrect.Contains(t.Position.x, t.Position.y) && selected.Contains(t.Sector);
							}
						}
						break;

					case MarqueSelectionMode.ADD:
						foreach(Sector s in General.Map.Map.Sectors) 
						{
							if(!s.Selected && IsInSelectionRect(s, selectionOutline))
								SelectSector(s, true, false);
						}
						if (marqueSelectionIncludesThings) 
						{
							ICollection<Sector> selected = General.Map.Map.GetSelectedSectors(true);

							foreach (Thing t in General.Map.ThingsFilter.VisibleThings) 
							{
								t.DetermineSector();
								if(t.Sector == null) continue;
								t.Selected |= selectionrect.Contains(t.Position.x, t.Position.y) && selected.Contains(t.Sector);
							}
						}
						break;

					case MarqueSelectionMode.SUBTRACT:
						foreach(Sector s in General.Map.Map.Sectors) 
						{
							if(!s.Selected) continue;
							if(IsInSelectionRect(s, selectionOutline))
								SelectSector(s, false, false);
						}
						if (marqueSelectionIncludesThings) 
						{
							foreach (Thing t in General.Map.ThingsFilter.VisibleThings)
								if (selectionrect.Contains(t.Position.x, t.Position.y)) t.Selected = false;
						}
						break;

					default: //should be Intersect
						foreach(Sector s in General.Map.Map.Sectors) 
						{
							if(!s.Selected) continue;
							if(!IsInSelectionRect(s, selectionOutline))
								SelectSector(s, false, false);
						}
						if (marqueSelectionIncludesThings) 
						{
							foreach(Thing t in General.Map.ThingsFilter.VisibleThings)
								if(!selectionrect.Contains(t.Position.x, t.Position.y)) t.Selected = false;
						}
						break;
				}

				// Make sure all linedefs reflect selected sectors
				foreach(Sidedef sd in General.Map.Map.Sidedefs)
					sd.Line.Selected = sd.Sector.Selected || (sd.Other != null && sd.Other.Sector.Selected);

				//mxd. Clear labels for unselected sectors
				if(marqueSelectionMode != MarqueSelectionMode.ADD) 
				{
					ICollection<Sector> orderedselection = General.Map.Map.GetSelectedSectors(false);
					foreach(Sector s in orderedselection)
					{
						TextLabel[] labelarray = labels[s];
						foreach(TextLabel l in labelarray) l.Text = "";
					}
				}

				UpdateSelectedLabels(); //mxd
				UpdateSelectionInfo(); //mxd
				UpdateOverlaySurfaces(); //mxd
			}
			
			base.OnEndMultiSelection();
			if(renderer.StartOverlay(true)) renderer.Finish();
			General.Interface.RedrawDisplay();
		}

		// This is called when the selection is updated
		protected override void OnUpdateMultiSelection()
		{
			base.OnUpdateMultiSelection();

			// Render selection
			if(renderer.StartOverlay(true))
			{
				RenderMultiSelection();
				renderer.Finish();
				renderer.Present();
			}
		}

		// When copying
		public override bool OnCopyBegin()
		{
			// No selection made? But we have a highlight!
			if((General.Map.Map.GetSelectedSectors(true).Count == 0) && (highlighted != null))
			{
				// Make the highlight the selection
				SelectSector(highlighted, true, false);

				//mxd. Actually, we want it marked, not selected
				bool result = base.OnCopyBegin();
				SelectSector(highlighted, false, false);
				return result;
			}

			return base.OnCopyBegin();
		}

		// When undo is used
		public override bool OnUndoBegin()
		{
			// Clear ordered selection
			General.Map.Map.ClearAllSelected();

			return base.OnUndoBegin();
		}

		// When undo is performed
		public override void OnUndoEnd()
		{
			// Clear labels
			SetupLabels();
			UpdateEffectLabels(); //mxd
			UpdateOverlaySurfaces(); //mxd
			base.OnUndoEnd(); //mxd
		}
		
		// When redo is used
		public override bool OnRedoBegin()
		{
			// Clear ordered selection
			General.Map.Map.ClearAllSelected();

			return base.OnRedoBegin();
		}

		// When redo is performed
		public override void OnRedoEnd()
		{
			// Clear labels
			SetupLabels();
			UpdateEffectLabels(); //mxd
			UpdateOverlaySurfaces(); //mxd
			base.OnRedoEnd(); //mxd
		}

		//mxd
		public override void UpdateSelectionInfo() 
		{
			if(General.Map.Map.SelectedSectorsCount > 0)
				General.Interface.DisplayStatus(StatusType.Selection, General.Map.Map.SelectedSectorsCount + (General.Map.Map.SelectedSectorsCount == 1 ? " sector" : " sectors") + " selected.");
			else
				General.Interface.DisplayStatus(StatusType.Selection, string.Empty);
		}
		
		#endregion

		#region ================== Actions

		// This copies the properties
		[BeginAction("classiccopyproperties")]
		public void CopyProperties()
		{
			// Determine source sectors
			ICollection<Sector> sel = null;
			if(General.Map.Map.SelectedSectorsCount > 0)
				sel = General.Map.Map.GetSelectedSectors(true);
			else if(highlighted != null)
				sel = new List<Sector> {highlighted};
			
			if(sel != null)
			{
				// Copy properties from first source sectors
				BuilderPlug.Me.CopiedSectorProps = new SectorProperties(General.GetByIndex(sel, 0));
				General.Interface.DisplayStatus(StatusType.Action, "Copied sector properties.");
			}
		}

		// This pastes the properties
		[BeginAction("classicpasteproperties")]
		public void PasteProperties()
		{
			if(BuilderPlug.Me.CopiedSectorProps != null)
			{
				// Determine target sectors
				ICollection<Sector> sel = null;
				if(General.Map.Map.SelectedSectorsCount > 0)
					sel = General.Map.Map.GetSelectedSectors(true);
				else if(highlighted != null)
				{
					sel = new List<Sector>();
					sel.Add(highlighted);
				}
				
				if(sel != null)
				{
					// Apply properties to selection
					General.Map.UndoRedo.CreateUndo("Paste sector properties");
					foreach(Sector s in sel)
					{
						BuilderPlug.Me.CopiedSectorProps.Apply(s);
						s.UpdateCeilingSurface();
						s.UpdateFloorSurface();
					}
					General.Interface.DisplayStatus(StatusType.Action, "Pasted sector properties.");
					
					// Update and redraw
					General.Map.IsChanged = true;
					General.Interface.RefreshInfo();
					General.Map.Renderer2D.UpdateExtraFloorFlag(); //mxd
					UpdateEffectLabels(); //mxd
					General.Interface.RedrawDisplay();
				}
			}
		}

		// This creates a new vertex at the mouse position
		[BeginAction("insertitem", BaseAction = true)]
		public virtual void InsertVertexAction()
		{
			// Start drawing mode
			DrawGeometryMode drawmode = new DrawGeometryMode();
			if(mouseinside)
			{
				bool snaptogrid = General.Interface.ShiftState ^ General.Interface.SnapToGrid;
				bool snaptonearest = General.Interface.CtrlState ^ General.Interface.AutoMerge;
				DrawnVertex v = DrawGeometryMode.GetCurrentPosition(mousemappos, snaptonearest, snaptogrid, renderer, new List<DrawnVertex>());
				drawmode.DrawPointAt(v);
			}
			General.Editing.ChangeMode(drawmode);
		}

		[BeginAction("makedoor")]
		public void MakeDoor()
		{
			// Anything selected?
			ICollection<Sector> orderedselection = General.Map.Map.GetSelectedSectors(true);
			
			//mxd. Should we use highlighted item?
			if(orderedselection.Count == 0 && highlighted != null)
			{
				orderedselection.Add(highlighted);
			}
			
			if(orderedselection.Count > 0)
			{
				if(doortex == "-") doortex = General.Map.Config.MakeDoorDoor; //mxd
				if(tracktex == "-") tracktex = General.Map.Config.MakeDoorTrack; //mxd
				string floortex = null;
				string ceiltex = null;
				
				// Find ceiling and floor textures
				foreach(Sector s in orderedselection)
				{
					if(floortex == null) floortex = s.FloorTexture; else if(floortex != s.FloorTexture) floortex = "";
					if(ceiltex == null) ceiltex = s.CeilTexture; else if(ceiltex != s.CeilTexture) ceiltex = "";
				}
				
				// Show the dialog
				MakeDoorForm form = new MakeDoorForm();
				if(form.Show(General.Interface, doortex, tracktex, ceiltex, floortex, resetoffsets) == DialogResult.OK)
				{
					doortex = form.DoorTexture;
					tracktex = form.TrackTexture;
					ceiltex = form.CeilingTexture;
					floortex = form.FloorTexture;
					resetoffsets = form.ResetOffsets;
					
					// Create undo
					General.Map.UndoRedo.CreateUndo("Make door (" + doortex + ")");
					General.Interface.DisplayStatus(StatusType.Action, "Created a " + doortex + " door.");
					
					// Go for all selected sectors
					foreach(Sector s in orderedselection)
					{
						// Lower the ceiling down to the floor
						s.CeilHeight = s.FloorHeight;

						// Make a unique tag (not sure if we need it yet, depends on the args)
						int tag = General.Map.Map.GetNewTag();

						// Go for all it's sidedefs
						foreach(Sidedef sd in s.Sidedefs)
						{
							// Singlesided?
							if(sd.Other == null)
							{
								// Make this a doortrak
								sd.SetTextureHigh("-");
								sd.SetTextureMid(tracktex);
								sd.SetTextureLow("-");

								// Set upper/lower unpegged flags
								sd.Line.SetFlag(General.Map.Config.UpperUnpeggedFlag, false);
								sd.Line.SetFlag(General.Map.Config.LowerUnpeggedFlag, true);
							}
							else
							{
								// Set textures
								if(floortex.Length > 0) s.SetFloorTexture(floortex);
								if(ceiltex.Length > 0) s.SetCeilTexture(ceiltex);
								if(doortex.Length > 0) sd.Other.SetTextureHigh(doortex);

								// Set upper/lower unpegged flags
								sd.Line.SetFlag(General.Map.Config.UpperUnpeggedFlag, false);
								sd.Line.SetFlag(General.Map.Config.LowerUnpeggedFlag, false);
								
								// Get door linedef type from config
								sd.Line.Action = General.Map.Config.MakeDoorAction;

								// Set activation type
								sd.Line.Activate = General.Map.Config.MakeDoorActivate;

								// Set the flags
								foreach(var flagpair in General.Map.Config.MakeDoorFlags)
									sd.Line.SetFlag(flagpair.Key, flagpair.Value);

								// Set the linedef args
								for(int i = 0; i < Linedef.NUM_ARGS; i++)
								{
									// A -1 arg indicates that the arg must be set to the new sector tag
									// and only in this case we set the tag on the sector, because only
									// then we know for sure that we need a tag.
									if(General.Map.Config.MakeDoorArgs[i] == -1)
									{
										sd.Line.Args[i] = tag;
										s.Tag = tag;
									}
									else
									{
										sd.Line.Args[i] = General.Map.Config.MakeDoorArgs[i];
									}
								}

								// Make sure the line is facing outwards
								if(sd.IsFront)
								{
									sd.Line.FlipVertices();
									sd.Line.FlipSidedefs();
								}
							}

							// Reset the texture offsets if required
							if (resetoffsets)
							{
								sd.OffsetX = 0;
								sd.OffsetY = 0;

								if (sd.Other != null)
								{
									sd.Other.OffsetX = 0;
									sd.Other.OffsetY = 0;
								}
							}
						}
					}
					
					// When a single sector was selected, deselect it now
					if(orderedselection.Count == 1) ClearSelection(); //mxd

					General.Map.Data.UpdateUsedTextures(); //mxd
				}
				
				// Done
				form.Dispose();
				General.Interface.RedrawDisplay();
			}
			else //mxd
			{
				General.Interface.DisplayStatus(StatusType.Warning, "This action requires a selection!");
			}
		}
		
		[BeginAction("deleteitem", BaseAction = true)]
		public void DeleteItem()
		{
			// Make list of selected sectors
			List<Sector> selected = new List<Sector>(General.Map.Map.GetSelectedSectors(true));
			if((selected.Count == 0) && (highlighted != null) && !highlighted.IsDisposed) selected.Add(highlighted);

			// Anything to do?
			if(selected.Count > 0)
			{
				// Make undo
				if(selected.Count > 1)
				{
					General.Map.UndoRedo.CreateUndo("Delete " + selected.Count + " sectors");
					General.Interface.DisplayStatus(StatusType.Action, "Deleted " + selected.Count + " sectors.");
				}
				else
				{
					General.Map.UndoRedo.CreateUndo("Delete sector");
					General.Interface.DisplayStatus(StatusType.Action, "Deleted sector.");
				}

				General.Map.Map.BeginAddRemove(); //mxd

				// Dispose selected sectors
				foreach(Sector s in selected)
				{
					//mxd. Get all the linedefs
					List<Linedef> lines = new List<Linedef>(s.Sidedefs.Count);
					foreach(Sidedef side in s.Sidedefs) lines.Add(side.Line);
					
					// Dispose the sector
					s.Dispose();

					// Check all the lines
					for(int i = lines.Count - 1; i >= 0; i--)
					{
						// If the line has become orphaned, remove it
						if((lines[i].Front == null) && (lines[i].Back == null))
						{
							// Remove line
							lines[i].Dispose();
						}
						else
						{
							// If the line only has a back side left, flip the line and sides
							if((lines[i].Front == null) && (lines[i].Back != null))
							{
								lines[i].FlipVertices();
								lines[i].FlipSidedefs();
							}

							//mxd. Check textures.
							if(lines[i].Front.MiddleRequired() && lines[i].Front.LongMiddleTexture == MapSet.EmptyLongName) 
							{
								if(lines[i].Front.LongHighTexture != MapSet.EmptyLongName) 
								{
									lines[i].Front.SetTextureMid(lines[i].Front.HighTexture);
								} 
								else if(lines[i].Front.LongLowTexture != MapSet.EmptyLongName) 
								{
									lines[i].Front.SetTextureMid(lines[i].Front.LowTexture);
								}
							}

							//mxd. Do we still need high/low textures?
							lines[i].Front.RemoveUnneededTextures(false);
							
							// Update sided flags
							lines[i].ApplySidedFlags();
						}
					}
				}

				General.Map.Map.EndAddRemove(); //mxd

				// Update cache values
				General.Map.IsChanged = true;
				General.Map.Map.Update();
				UpdateOverlaySurfaces(); //mxd
				
				// Make text labels for sectors
				SetupLabels();
				UpdateSelectedLabels();
				
				// Redraw screen
				General.Map.Renderer2D.UpdateExtraFloorFlag(); //mxd
				General.Interface.RedrawDisplay();
			}
		}

		[BeginAction("dissolveitem", BaseAction = true)] //mxd
		public void DissolveItem() 
		{
			//TODO handle this differently?..
			DeleteItem();
		}
		
		// This joins sectors together and keeps all lines
		[BeginAction("joinsectors")]
		public void JoinSectors()
		{
			// Worth our money?
			int count = General.Map.Map.GetSelectedSectors(true).Count;
			if(count > 1)
			{
				// Make undo
				General.Map.UndoRedo.CreateUndo("Join " + count + " sectors");
				General.Interface.DisplayStatus(StatusType.Action, "Joined " + count + " sectors.");

				// Merge
				JoinMergeSectors(false);

				// Deselect
				General.Map.Map.ClearSelectedSectors();
				General.Map.Map.ClearSelectedLinedefs();
				General.Map.IsChanged = true;

				//mxd. Clear selection info
				General.Interface.DisplayStatus(StatusType.Selection, string.Empty);

				//mxd. Update
				UpdateOverlaySurfaces();
				UpdateEffectLabels();

				// Redraw display
				General.Interface.RedrawDisplay();
			}
		}

		// This joins sectors together and removes the lines in between
		[BeginAction("mergesectors")]
		public void MergeSectors()
		{
			// Worth our money?
			int count = General.Map.Map.GetSelectedSectors(true).Count;
			if(count > 1)
			{
				// Make undo
				General.Map.UndoRedo.CreateUndo("Merge " + count + " sectors");
				General.Interface.DisplayStatus(StatusType.Action, "Merged " + count + " sectors.");

				// Merge
				JoinMergeSectors(true);

				// Deselect
				General.Map.Map.ClearSelectedSectors();
				General.Map.Map.ClearSelectedLinedefs();
				General.Map.IsChanged = true;

				//mxd. Clear selection info
				General.Interface.DisplayStatus(StatusType.Selection, string.Empty);
				
				//mxd. Update
				UpdateOverlaySurfaces();
				UpdateEffectLabels();

				// Redraw display
				General.Interface.RedrawDisplay();
			}
		}

		// Make gradient brightness
		[BeginAction("gradientbrightness")]
		public void MakeGradientBrightness()
		{
			// Need at least 3 selected sectors
			// The first and last are not modified
			ICollection<Sector> orderedselection = General.Map.Map.GetSelectedSectors(true);
			if(orderedselection.Count > 2) 
			{
				General.Interface.DisplayStatus(StatusType.Action, "Created gradient brightness over selected sectors.");
				General.Map.UndoRedo.CreateUndo("Gradient brightness");

				//mxd
				Sector start = General.GetByIndex(orderedselection, 0);
				Sector end = General.GetByIndex(orderedselection, orderedselection.Count - 1);

				//mxd. Use UDMF light?
				string mode = (string)BuilderPlug.Me.MenusForm.GradientModeMenu.SelectedItem;
				InterpolationTools.Mode interpolationmode = (InterpolationTools.Mode) BuilderPlug.Me.MenusForm.GradientInterpolationMenu.SelectedIndex;
				if(General.Map.UDMF && mode != MenusForm.BrightnessGradientModes.Sectors) 
				{
					if(mode == MenusForm.BrightnessGradientModes.Ceilings || mode == MenusForm.BrightnessGradientModes.Floors) 
					{
						string lightKey;
						string lightAbsKey;
						int startbrightness, endbrightness;

						if(mode == MenusForm.BrightnessGradientModes.Ceilings) 
						{
							lightKey = "lightceiling";
							lightAbsKey = "lightceilingabsolute";
						}
						else //should be floors...
						{
							lightKey = "lightfloor";
							lightAbsKey = "lightfloorabsolute";
						}

						//get total brightness of start sector
						if(start.Fields.GetValue(lightAbsKey, false))
							startbrightness = start.Fields.GetValue(lightKey, 0);
						else
							startbrightness = Math.Min(255, Math.Max(0, start.Brightness + start.Fields.GetValue(lightKey, 0)));

						//get total brightness of end sector
						if(end.Fields.GetValue(lightAbsKey, false))
							endbrightness = end.Fields.GetValue(lightKey, 0);
						else
							endbrightness = Math.Min(255, Math.Max(0, end.Brightness + end.Fields.GetValue(lightKey, 0)));

						// Go for all sectors in between first and last
						int index = 0;
						foreach(Sector s in orderedselection) 
						{
							s.Fields.BeforeFieldsChange();
							float u = index / (float) (orderedselection.Count - 1);
							float b = InterpolationTools.Interpolate(startbrightness, endbrightness, u, interpolationmode);

							//absolute flag set?
							if(s.Fields.GetValue(lightAbsKey, false)) 
							{
								if(s.Fields.ContainsKey(lightKey))
									s.Fields[lightKey].Value = (int) b;
								else
									s.Fields.Add(lightKey, new UniValue(UniversalType.Integer, (int) b));
							}
							else 
							{
								UDMFTools.SetInteger(s.Fields, lightKey, (int) b - s.Brightness, 0);
							}

							s.UpdateNeeded = true;
							index++;
						}
					} 
					else if(mode == MenusForm.BrightnessGradientModes.Fade) 
					{
						ApplyColorGradient(orderedselection, start, end, interpolationmode, "fadecolor", 0);
					} 
					else if(mode == MenusForm.BrightnessGradientModes.Light) 
					{
						ApplyColorGradient(orderedselection, start, end, interpolationmode, "lightcolor", 0xFFFFFF);
					} 
					else if(mode == MenusForm.BrightnessGradientModes.LightAndFade) 
					{
						ApplyColorGradient(orderedselection, start, end, interpolationmode, "fadecolor", 0);
						ApplyColorGradient(orderedselection, start, end, interpolationmode, "lightcolor", 0xFFFFFF);
					}
				}
				else 
				{
					// Go for all sectors in between first and last
					int index = 0;
					foreach(Sector s in orderedselection) 
					{
						float u = index / (float)(orderedselection.Count - 1);
						s.Brightness = InterpolationTools.Interpolate(start.Brightness, end.Brightness, u, interpolationmode); //mxd
						index++;
					}
				}

				// Update
				General.Map.Map.Update();
				UpdateOverlay();
				renderer.Present();
				General.Interface.RedrawDisplay();
				General.Interface.RefreshInfo();
				General.Map.IsChanged = true;
			} 
			else 
			{
				General.Interface.DisplayStatus(StatusType.Warning, "Select at least 3 sectors first!");
			}
		}

		//mxd
		private static void ApplyColorGradient(ICollection<Sector> orderedselection, Sector start, Sector end, InterpolationTools.Mode interpolationmode, string key, int defaultvalue) 
		{
			if(!start.Fields.ContainsKey(key) && !end.Fields.ContainsKey(key)) 
			{
				General.Interface.DisplayStatus(StatusType.Warning, "First or last selected sector must have the '" + key + "' property!");
			} 
			else 
			{
				Color startColor, endColor;
				if(key == "fadecolor")
				{
					startColor = Tools.GetSectorFadeColor(start);
					endColor = Tools.GetSectorFadeColor(end);
				}
				else
				{
					startColor = PixelColor.FromInt(start.Fields.GetValue(key, defaultvalue)).ToColor();
					endColor = PixelColor.FromInt(end.Fields.GetValue(key, defaultvalue)).ToColor();
				}

				// Go for all sectors in between first and last
				int index = 0;
				foreach(Sector s in orderedselection) 
				{
					if(index > 0 && index < orderedselection.Count - 1)
					{
						s.Fields.BeforeFieldsChange();
						float u = index / (float) (orderedselection.Count - 1);
						Color c = Color.FromArgb(0, General.Clamp(InterpolationTools.Interpolate(startColor.R, endColor.R, u, interpolationmode), 0, 255),
						                         General.Clamp(InterpolationTools.Interpolate(startColor.G, endColor.G, u, interpolationmode), 0, 255),
						                         General.Clamp(InterpolationTools.Interpolate(startColor.B, endColor.B, u, interpolationmode), 0, 255));

						UDMFTools.SetInteger(s.Fields, key, c.ToArgb(), defaultvalue);
						s.UpdateNeeded = true;
					}
					index++;
				}
			}
		}

		// Make gradient floors
		[BeginAction("gradientfloors")]
		public void MakeGradientFloors()
		{
			// Need at least 3 selected sectors
			// The first and last are not modified
			ICollection<Sector> orderedselection = General.Map.Map.GetSelectedSectors(true);
			if(orderedselection.Count > 2) 
			{
				General.Interface.DisplayStatus(StatusType.Action, "Created gradient floor heights over selected sectors.");
				General.Map.UndoRedo.CreateUndo("Gradient floor heights");

				int startlevel = General.GetByIndex(orderedselection, 0).FloorHeight;
				int endlevel = General.GetByIndex(orderedselection, orderedselection.Count - 1).FloorHeight;
				InterpolationTools.Mode interpolationmode = (InterpolationTools.Mode)BuilderPlug.Me.MenusForm.GradientInterpolationMenu.SelectedIndex; //mxd

				// Go for all sectors in between first and last
				int index = 0;
				foreach(Sector s in orderedselection) 
				{
					float u = index / (float)(orderedselection.Count - 1);
					s.FloorHeight = InterpolationTools.Interpolate(startlevel, endlevel, u, interpolationmode); //mxd
					index++;
				}

				// Update
				General.Interface.RefreshInfo();
				General.Map.IsChanged = true;
			} 
			else 
			{
				General.Interface.DisplayStatus(StatusType.Warning, "Select at least 3 sectors first!");
			}
		}

		// Make gradient ceilings
		[BeginAction("gradientceilings")]
		public void MakeGradientCeilings()
		{
			// Need at least 3 selected sectors
			// The first and last are not modified
			ICollection<Sector> orderedselection = General.Map.Map.GetSelectedSectors(true);
			if(orderedselection.Count > 2) 
			{
				General.Interface.DisplayStatus(StatusType.Action, "Created gradient ceiling heights over selected sectors.");
				General.Map.UndoRedo.CreateUndo("Gradient ceiling heights");

				int startlevel = General.GetByIndex(orderedselection, 0).CeilHeight;
				int endlevel = General.GetByIndex(orderedselection, orderedselection.Count - 1).CeilHeight;
				InterpolationTools.Mode interpolationmode = (InterpolationTools.Mode)BuilderPlug.Me.MenusForm.GradientInterpolationMenu.SelectedIndex; //mxd

				// Go for all sectors in between first and last
				int index = 0;
				foreach(Sector s in orderedselection) 
				{
					float u = (float)index / (orderedselection.Count - 1);
					s.CeilHeight = InterpolationTools.Interpolate(startlevel, endlevel, u, interpolationmode);
					index++;
				}

				// Update
				General.Interface.RefreshInfo();
				General.Map.IsChanged = true;
			} 
			else 
			{
				General.Interface.DisplayStatus(StatusType.Warning, "Select at least 3 sectors first!");
			}
		}

		// Change heights
		[BeginAction("lowerfloor8")]
		public void LowerFloors8() 
		{
			// Get selection
			ICollection<Sector> selected = General.Map.Map.GetSelectedSectors(true);
			if(selected.Count == 0 && highlighted != null && !highlighted.IsDisposed)
				selected.Add(highlighted);

			if(selected.Count == 0) 
			{
				General.Interface.DisplayStatus(StatusType.Warning, "This action requires a selection!");
				return;
			}

			// Make undo
			General.Interface.DisplayStatus(StatusType.Action, "Lowered floor heights by 8mp.");
			General.Map.UndoRedo.CreateUndo("Floor heights change", this, UndoGroup.FloorHeightChange, CreateSelectionCRC(selected));

			// Change heights
			foreach(Sector s in selected) s.FloorHeight -= 8;

			// Update
			General.Interface.RefreshInfo();
			General.Map.IsChanged = true;
		}

		// Change heights
		[BeginAction("raisefloor8")]
		public void RaiseFloors8() 
		{
			// Get selection
			ICollection<Sector> selected = General.Map.Map.GetSelectedSectors(true);
			if(selected.Count == 0 && highlighted != null && !highlighted.IsDisposed)
				selected.Add(highlighted);

			if(selected.Count == 0) 
			{
				General.Interface.DisplayStatus(StatusType.Warning, "This action requires a selection!");
				return;
			}

			// Make undo
			General.Interface.DisplayStatus(StatusType.Action, "Raised floor heights by 8mp.");
			General.Map.UndoRedo.CreateUndo("Floor heights change", this, UndoGroup.FloorHeightChange, CreateSelectionCRC(selected));

			// Change heights
			foreach(Sector s in selected) s.FloorHeight += 8;

			// Update
			General.Interface.RefreshInfo();
			General.Map.IsChanged = true;
		}

		// Change heights
		[BeginAction("lowerceiling8")]
		public void LowerCeilings8() 
		{
			// Get selection
			ICollection<Sector> selected = General.Map.Map.GetSelectedSectors(true);
			if((selected.Count == 0) && (highlighted != null) && !highlighted.IsDisposed)
				selected.Add(highlighted);

			if(selected.Count == 0) 
			{
				General.Interface.DisplayStatus(StatusType.Warning, "This action requires a selection!");
				return;
			}

			// Make undo
			General.Interface.DisplayStatus(StatusType.Action, "Lowered ceiling heights by 8mp.");
			General.Map.UndoRedo.CreateUndo("Ceiling heights change", this, UndoGroup.CeilingHeightChange, CreateSelectionCRC(selected));

			// Change heights
			foreach(Sector s in selected) s.CeilHeight -= 8;

			// Update
			General.Interface.RefreshInfo();
			General.Map.IsChanged = true;
		}

		// Change heights
		[BeginAction("raiseceiling8")]
		public void RaiseCeilings8() 
		{
			// Get selection
			ICollection<Sector> selected = General.Map.Map.GetSelectedSectors(true);
			if(selected.Count == 0 && highlighted != null && !highlighted.IsDisposed)
				selected.Add(highlighted);

			if(selected.Count == 0) 
			{
				General.Interface.DisplayStatus(StatusType.Warning, "This action requires a selection!");
				return;
			}

			// Make undo
			General.Interface.DisplayStatus(StatusType.Action, "Raised ceiling heights by 8mp.");
			General.Map.UndoRedo.CreateUndo("Ceiling heights change", this, UndoGroup.CeilingHeightChange, CreateSelectionCRC(selected));

			// Change heights
			foreach(Sector s in selected) s.CeilHeight += 8;

			// Update
			General.Interface.RefreshInfo();
			General.Map.IsChanged = true;
		}

		//mxd. Raise brightness
		[BeginAction("raisebrightness8")]
		public void RaiseBrightness8() 
		{
			// Get selection
			ICollection<Sector> selected = General.Map.Map.GetSelectedSectors(true);
			if((selected.Count == 0) && (highlighted != null) && !highlighted.IsDisposed) 
				selected.Add(highlighted);

			if(selected.Count == 0) 
			{
				General.Interface.DisplayStatus(StatusType.Warning, "This action requires a selection!");
				return;
			}

			// Make undo
			Sector first = General.GetByIndex(selected, 0); //mxd
			int diff = General.Map.Config.BrightnessLevels.GetNextHigher(first.Brightness) - first.Brightness; //mxd
			General.Interface.DisplayStatus(StatusType.Action, "Raised sector brightness by " + diff + ".");
			General.Map.UndoRedo.CreateUndo("Sector brightness change", this, UndoGroup.SectorBrightnessChange, CreateSelectionCRC(selected));

			// Change heights
			foreach(Sector s in selected) 
			{
				s.Brightness = General.Map.Config.BrightnessLevels.GetNextHigher(s.Brightness);
				s.UpdateNeeded = true;
				s.UpdateCache();
			}

			// Update
			General.Interface.RefreshInfo();
			General.Map.IsChanged = true;

			// Redraw
			General.Interface.RedrawDisplay();
		}

		//mxd. Lower brightness
		[BeginAction("lowerbrightness8")]
		public void LowerBrightness8() 
		{
			// Get selection
			ICollection<Sector> selected = General.Map.Map.GetSelectedSectors(true);
			if(selected.Count == 0 && highlighted != null && !highlighted.IsDisposed)
				selected.Add(highlighted);

			if (selected.Count == 0) 
			{
				General.Interface.DisplayStatus(StatusType.Warning, "This action requires a selection!");
				return;
			}

			// Make undo
			Sector first = General.GetByIndex(selected, 0); //mxd
			int diff = first.Brightness - General.Map.Config.BrightnessLevels.GetNextLower(first.Brightness); //mxd
			General.Interface.DisplayStatus(StatusType.Action, "Lowered sector brightness by " + diff + ".");
			General.Map.UndoRedo.CreateUndo("Sector brightness change", this, UndoGroup.SectorBrightnessChange, CreateSelectionCRC(selected));

			// Change heights
			foreach(Sector s in selected) 
			{
				s.Brightness = General.Map.Config.BrightnessLevels.GetNextLower(s.Brightness);
				s.UpdateNeeded = true;
				s.UpdateCache();
			}

			// Update
			General.Interface.RefreshInfo();
			General.Map.IsChanged = true;

			// Redraw
			General.Interface.RedrawDisplay();
		}

		// This clears the selection
		[BeginAction("clearselection", BaseAction = true)]
		public void ClearSelection()
		{
			// Clear selection
			General.Map.Map.ClearAllSelected();

			//mxd. Clear selection info
			General.Interface.DisplayStatus(StatusType.Selection, string.Empty); 

			// Clear labels
			foreach(TextLabel[] labelarray in labels.Values)
				foreach(TextLabel l in labelarray) l.Text = "";
			UpdateOverlaySurfaces(); //mxd
			UpdateEffectLabels(); //mxd
			
			// Redraw
			General.Interface.RedrawDisplay();
		}

		[BeginAction("placethings")] //mxd
		public void PlaceThings() 
		{
			// Make list of selected sectors
			ICollection<Sector> sectors = General.Map.Map.GetSelectedSectors(true);
			List<Vector2D> positions = new List<Vector2D>();
			
			if(sectors.Count == 0) 
			{
				if(highlighted != null && !highlighted.IsDisposed) 
				{
					sectors.Add(highlighted);
				} 
				else 
				{
					General.Interface.DisplayStatus(StatusType.Warning, "This action requires selection of some description!");
					return;
				}
			}

			// Make list of suitable positions
			foreach(Sector s in sectors) 
			{
				Vector2D pos = (s.Labels.Count > 0 ? s.Labels[0].position : new Vector2D(s.BBox.X + s.BBox.Width / 2, s.BBox.Y + s.BBox.Height / 2));
				if(!positions.Contains(pos)) positions.Add(pos);
			}

			if(positions.Count < 1) 
			{
				General.Interface.DisplayStatus(StatusType.Warning, "Unable to get vertex positions from selection!");
				return;
			}

			PlaceThingsAtPositions(positions);
		}

		//mxd. rotate clockwise
		[BeginAction("rotateclockwise")]
		public void RotateCW() 
		{
			RotateTextures(5);
		}

		//mxd. rotate counterclockwise
		[BeginAction("rotatecounterclockwise")]
		public void RotateCCW() 
		{
			RotateTextures(-5);
		}

		//mxd
		private void RotateTextures(float increment) 
		{
			if (!General.Map.UDMF) 
			{
				General.Interface.DisplayStatus(StatusType.Warning, "This action works only in UDMF map format!");
				return;
			}

			// Make list of selected sectors
			ICollection<Sector> sectors = General.Map.Map.GetSelectedSectors(true);

			if(sectors.Count == 0 && highlighted != null && !highlighted.IsDisposed)
				sectors.Add(highlighted);

			if(sectors.Count == 0) 
			{
				General.Interface.DisplayStatus(StatusType.Warning, "This action requires a selection!");
				return;
			}

			string targets;
			string target;
			switch (General.Map.Renderer2D.ViewMode) 
			{
				case ViewMode.FloorTextures:
					target = " a floor";
					targets = " floors";
					break;

				case ViewMode.CeilingTextures:
					target = " a ceiling";
					targets = " ceilings";
					break;

				default:
					target = " a floor and a ceiling";
					targets = " floors and ceilings";
					break;
			}

			// Make undo
			if(sectors.Count > 1) 
			{
				General.Map.UndoRedo.CreateUndo("Rotate " + sectors.Count + targets, this, UndoGroup.TextureRotationChange, CreateSelectionCRC(sectors));
				General.Interface.DisplayStatus(StatusType.Action, "Rotated " + sectors.Count + targets + ".");
			} 
			else 
			{
				General.Map.UndoRedo.CreateUndo("Rotate" + target, this, UndoGroup.TextureRotationChange, CreateSelectionCRC(sectors));
				General.Interface.DisplayStatus(StatusType.Action, "Rotated" + target + ".");
			}

			//rotate stuff
			foreach (Sector s in sectors) 
			{
				s.Fields.BeforeFieldsChange();

				//floor
				if(General.Map.Renderer2D.ViewMode == ViewMode.FloorTextures || General.Map.Renderer2D.ViewMode != ViewMode.CeilingTextures)
				{
					UDMFTools.SetFloat(s.Fields, "rotationfloor", General.ClampAngle(UDMFTools.GetFloat(s.Fields, "rotationfloor") + increment));
					s.UpdateNeeded = true;
				}

				//ceiling
				if(General.Map.Renderer2D.ViewMode == ViewMode.CeilingTextures || General.Map.Renderer2D.ViewMode != ViewMode.FloorTextures) 
				{
					UDMFTools.SetFloat(s.Fields, "rotationceiling", General.ClampAngle(UDMFTools.GetFloat(s.Fields, "rotationceiling") + increment));
					s.UpdateNeeded = true;
				}
			}

			// Redraw screen
			General.Map.Map.Update();
			General.Interface.RedrawDisplay();
			General.Interface.RefreshInfo();
		}

		//mxd
		[BeginAction("selectsimilar")]
		public void SelectSimilar() 
		{
			ICollection<Sector> selection = General.Map.Map.GetSelectedSectors(true);

			if(selection.Count == 0) 
			{
				General.Interface.DisplayStatus(StatusType.Warning, "This action requires a selection!");
				return;
			}

			var form = new SelectSimilarElementOptionsPanel();
			if(form.Setup(this)) form.ShowDialog();
		}
		
		//mxd
		[BeginAction("fliplinedefs")]
		public void FlipLinedefs() 
		{
			// Get selection
			ICollection<Sector> selection = General.Map.Map.GetSelectedSectors(true);

			if(selection.Count == 0 && highlighted != null && !highlighted.IsDisposed)
				selection.Add(highlighted);

			if(selection.Count == 0) 
			{
				General.Interface.DisplayStatus(StatusType.Warning, "This action requires a selection!");
				return;
			}

			// Make undo
			if(selection.Count > 1) 
			{
				General.Map.UndoRedo.CreateUndo("Flip linedefs of " + selection.Count + " sectors");
				General.Interface.DisplayStatus(StatusType.Action, "Flipped linedefs of " + selection.Count + "sectors.");
			} 
			else 
			{
				General.Map.UndoRedo.CreateUndo("Flip sector linedefs");
				General.Interface.DisplayStatus(StatusType.Action, "Flipped sector linedefs.");
			}

			// Flip lines
			Tools.FlipSectorLinedefs(selection, false);
			
			// Redraw
			General.Map.Map.Update();
			General.Map.IsChanged = true;
			General.Interface.RefreshInfo();
			General.Interface.RedrawDisplay();
		}

		#endregion
	}
}
