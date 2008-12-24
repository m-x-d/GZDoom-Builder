
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

#endregion

namespace CodeImp.DoomBuilder.BuilderModes
{
	[EditMode(DisplayName = "Brightness Mode",
			  SwitchAction = "brightnessmode",
			  ButtonImage = "BrightnessMode.png",
			  ButtonOrder = int.MinValue + 201,
			  AllowCopyPaste = false,
			  UseByDefault = true)]
	
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
		
		// Interface
		private bool editpressed;
		
		// The methods GetSelected* and MarkSelected* on the MapSet do not
		// retain the order in which items were selected.
		// This list keeps in order while sectors are selected/deselected.
		protected List<Sector> orderedselection;
		
		// Labels
		private Dictionary<Sector, TextLabel[]> labels;
		
		// Modifying
		private ModifyMode mode;
		private Point editstartpos;
		private List<int> sectorbrightness;
		private int undoticket;
		
		#endregion
		
		#region ================== Properties
		
		#endregion
		
		#region ================== Constructor / Disposer
		
		// Constructor
		public BrightnessMode()
		{
			// Make ordered selection list
			orderedselection = new List<Sector>();
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
					foreach(Sector s in orderedselection)
					{
						// We use the overlay to dim the brightness of the sectors
						PixelColor brightnesscolor = new PixelColor((byte)(255 - s.Brightness), 0, 0, 0);
						int brightnessint = brightnesscolor.ToInt();

						// Render the geometry
						FlatVertex[] verts = new FlatVertex[s.FlatVertices.Length];
						s.FlatVertices.CopyTo(verts, 0);
						for(int i = 0; i < verts.Length; i++) verts[i].c = brightnessint;
						renderer.RenderGeometry(verts, null, true);
					}
				}
				
				// Go for all sectors
				foreach(Sector s in General.Map.Map.Sectors)
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
				
				renderer.Finish();
			}
		}
		
		// This highlights a new item
		protected void Highlight(Sector s)
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
		protected void SelectSector(Sector s, bool selectstate, bool update)
		{
			bool selectionchanged = false;

			if(!s.IsDisposed)
			{
				// Select the sector?
				if(selectstate && !s.Selected)
				{
					orderedselection.Add(s);
					s.Selected = true;
					selectionchanged = true;
					
					// Setup labels
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
					orderedselection.Remove(s);
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
					UpdateOverlay();
					renderer.Present();
				}
			}
			else
			{
				// Remove from list
				orderedselection.Remove(s);
			}
		}
		
		// This updates labels from the selected sectors
		private void UpdateSelectedLabels()
		{
			// Update labels for editing mode?
			if(mode == ModifyMode.Adjusting)
			{
				// Go for all labels in all selected sectors
				for(int i = 0; i < orderedselection.Count; i++)
				{
					Sector s = orderedselection[i];
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
				for(int i = 0; i < orderedselection.Count; i++)
				{
					Sector s = orderedselection[i];
					TextLabel[] labelarray = labels[s];
					foreach(TextLabel l in labelarray)
					{
						// Make sure the text and color are right
						int labelnum = i + 1;
						l.Text = labelnum.ToString();
						l.Color = General.Colors.Selection;
					}
				}
			}
		}
		
		#endregion
		
		#region ================== Events
		
		// Mode engages
		public override void OnEngage()
		{
			base.OnEngage();

			// Add toolbar button
			General.Interface.AddButton(BuilderPlug.Me.MenusForm.MakeGradientBrightness);

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
			General.Map.Map.ClearAllMarks(false);
			General.Map.Map.MarkSelectedVertices(true, true);
			ICollection<Linedef> lines = General.Map.Map.LinedefsFromMarkedVertices(false, true, false);
			foreach(Linedef l in lines) l.Selected = true;
			General.Map.Map.ClearMarkedSectors(true);
			foreach(Linedef l in General.Map.Map.Linedefs)
			{
				if(!l.Selected)
				{
					if(l.Front != null) l.Front.Sector.Marked = false;
					if(l.Back != null) l.Back.Sector.Marked = false;
				}
			}
			General.Map.Map.ClearAllSelected();
			foreach(Sector s in General.Map.Map.Sectors)
			{
				if(s.Marked)
				{
					s.Selected = true;
					foreach(Sidedef sd in s.Sidedefs) sd.Line.Selected = true;
				}
			}
			
			// Fill the list with selected sectors (these are not in order, but we have no other choice)
			ICollection<Sector> selectedsectors = General.Map.Map.GetSelectedSectors(true);
			General.Map.Map.ClearSelectedSectors();
			foreach(Sector s in selectedsectors) SelectSector(s, true, false);
			
			// Update
			UpdateOverlay();
		}

		// When disengaged
		public override void OnDisengage()
		{
			base.OnDisengage();

			// Remove toolbar button
			General.Interface.RemoveButton(BuilderPlug.Me.MenusForm.MakeGradientBrightness);

			// Going to EditSelectionMode?
			if(General.Editing.NewMode is EditSelectionMode)
			{
				// No selection made? But we have a highlight!
				if((General.Map.Map.GetSelectedSectors(true).Count == 0) && (highlighted != null))
				{
					// Make the highlight the selection
					SelectSector(highlighted, true, false);
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
			if((mode == ModifyMode.None) && (e.Button == MouseButtons.None))
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
			// Adjusting mode?
			else if(mode == ModifyMode.Adjusting)
			{
				// Calculate change in position
				Point delta = Cursor.Position - new Size(editstartpos);
				
				// Adjust selected sectors
				for(int i = 0; i < orderedselection.Count; i++)
				{
					Sector s = orderedselection[i];
					int basebrightness = sectorbrightness[i];
					
					// Adjust brightness
					s.Brightness = basebrightness - delta.Y;
					if(s.Brightness > 255) s.Brightness = 255;
					if(s.Brightness < 0) s.Brightness = 0;
				}
				
				// Update
				UpdateSelectedLabels();
				UpdateOverlay();
				renderer.Present();
			}
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
					// Flip selection
					SelectSector(highlighted, !highlighted.Selected, true);

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
				}
			}

			base.OnSelectEnd();
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
				SelectSector(s, allselected, false);
			}
			
			// Make sure all linedefs reflect selected sectors
			foreach(Sidedef sd in General.Map.Map.Sidedefs)
				if(!sd.Sector.Selected && ((sd.Other == null) || !sd.Other.Sector.Selected))
					sd.Line.Selected = false;
			
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
			
			// No selection?
			if(orderedselection.Count == 0)
			{
				// Make the highlight a selection if we have a highlight
				if((highlighted != null) && !highlighted.IsDisposed)
					SelectSector(highlighted, true, false);
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
			
			// Stop editing
			mode = ModifyMode.None;
			sectorbrightness = null;
			
			// Nothing changed? Then writhdraw the undo
			if(editstartpos.Y == Cursor.Position.Y)
				General.Map.UndoRedo.WithdrawUndo(undoticket);
			
			// Update
			General.Map.Map.Update();
			UpdateSelectedLabels();
			General.Interface.RedrawDisplay();
			renderer.Present();
			
			// If only one sector was selected, deselect it
			if(orderedselection.Count == 1) SelectSector(orderedselection[0], false, true);
		}

		// When undo is used
		public override bool OnUndoBegin()
		{
			// Clear selection
			General.Map.Map.ClearAllSelected();
			orderedselection.Clear();
			
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
			orderedselection.Clear();

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
			if(orderedselection.Count > 2)
			{
				float startbrightness = (float)orderedselection[0].Brightness;
				float endbrightness = (float)orderedselection[orderedselection.Count - 1].Brightness;
				float delta = endbrightness - startbrightness;
				
				// Go for all sectors in between first and last
				for(int i = 1; i < (orderedselection.Count - 1); i++)
				{
					float u = (float)i / (float)(orderedselection.Count - 1);
					float b = startbrightness + delta * u;
					orderedselection[i].Brightness = (int)b;
				}
			}
			
			// Update
			UpdateOverlay();
			renderer.Present();
		}
		
		// This clears the selection
		[BeginAction("clearselection", BaseAction = true)]
		public void ClearSelection()
		{
			// Clear selection
			General.Map.Map.ClearAllSelected();
			orderedselection.Clear();
			
			// Clear labels
			foreach(TextLabel[] labelarray in labels.Values)
				foreach(TextLabel l in labelarray) l.Text = "";
			
			// Redraw
			General.Interface.RedrawDisplay();
		}
		
		#endregion
	}
}
