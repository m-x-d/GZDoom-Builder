
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
using CodeImp.DoomBuilder.Windows;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Rendering;
using CodeImp.DoomBuilder.Geometry;
using CodeImp.DoomBuilder.Editing;
using CodeImp.DoomBuilder.Actions;
using CodeImp.DoomBuilder.Types;
using CodeImp.DoomBuilder.Config;
using CodeImp.DoomBuilder.GZBuilder.Tools;
using CodeImp.DoomBuilder.Data;

#endregion

namespace CodeImp.DoomBuilder.BuilderModes
{
	[EditMode(DisplayName = "Linedefs Mode",
			  SwitchAction = "linedefsmode",	// Action name used to switch to this mode
			  ButtonImage = "LinesMode.png",	// Image resource name for the button
			  ButtonOrder = int.MinValue + 100,	// Position of the button (lower is more to the left)
			  ButtonGroup = "000_editing",
			  UseByDefault = true,
			  SafeStartMode = true)]

	public class LinedefsMode : BaseClassicMode
	{
		#region ================== Constants

		#endregion

		#region ================== Variables

		// Highlighted item
		private Linedef highlighted;
		private Association[] association = new Association[Linedef.NUM_ARGS];
		private Association highlightasso = new Association();
		private Vector2D insertPreview = new Vector2D(float.NaN, float.NaN); //mxd
		
		// Interface
		private bool editpressed;
		
		#endregion

		#region ================== Properties

		public override object HighlightedObject { get { return highlighted; } }
		
		#endregion

		#region ================== Constructor / Disposer

		#endregion

		#region ================== Methods

		// This highlights a new item
		protected void Highlight(Linedef l)
		{
			bool completeredraw = false;
			LinedefActionInfo action = null;

			// Often we can get away by simply undrawing the previous
			// highlight and drawing the new highlight. But if associations
			// are or were drawn we need to redraw the entire display.
			
			// Previous association highlights something?
			if((highlighted != null) && (highlighted.Tag > 0)) completeredraw = true;
			
			// Set highlight association
			if(l != null)
				highlightasso.Set(new Vector2D((l.Start.Position  + l.End.Position)/2), l.Tag, UniversalType.LinedefTag);
			else
				highlightasso.Set(new Vector2D(), 0, 0);

			// New association highlights something?
			if((l != null) && (l.Tag > 0)) completeredraw = true;

			// Use the line tag to highlight sectors (Doom style)
			if(General.Map.Config.LineTagIndicatesSectors)
			{
				if(l != null)
					association[0].Set(new Vector2D((l.Start.Position  + l.End.Position)/2), l.Tag, UniversalType.SectorTag);
				else
					association[0].Set(new Vector2D(), 0, 0);
			}
			else
			{
				if(l != null)
				{
					// Check if we can find the linedefs action
					if((l.Action > 0) && General.Map.Config.LinedefActions.ContainsKey(l.Action))
						action = General.Map.Config.LinedefActions[l.Action];
				}
				
				// Determine linedef associations
				for(int i = 0; i < Linedef.NUM_ARGS; i++)
				{
					// Previous association highlights something?
					if((association[i].type == UniversalType.SectorTag) ||
					   (association[i].type == UniversalType.LinedefTag) ||
					   (association[i].type == UniversalType.ThingTag)) completeredraw = true;

					// Make new association
					if(action != null)
						association[i].Set(new Vector2D((l.Start.Position  + l.End.Position)/2), l.Args[i], action.Args[i].Type);
					else
						association[i].Set(new Vector2D(), 0, 0);

					// New association highlights something?
					if((association[i].type == UniversalType.SectorTag) ||
					   (association[i].type == UniversalType.LinedefTag) ||
					   (association[i].type == UniversalType.ThingTag)) completeredraw = true;
				}
			}
			
			// If we're changing associations, then we
			// need to redraw the entire display
			if(completeredraw)
			{
				// Set new highlight and redraw completely
				highlighted = l;
				General.Interface.RedrawDisplay();
			}
			else
			{
				// Update display
				if(renderer.StartPlotter(false))
				{
					// Undraw previous highlight
					if((highlighted != null) && !highlighted.IsDisposed)
					{
						renderer.PlotLinedef(highlighted, renderer.DetermineLinedefColor(highlighted));
						renderer.PlotVertex(highlighted.Start, renderer.DetermineVertexColor(highlighted.Start));
						renderer.PlotVertex(highlighted.End, renderer.DetermineVertexColor(highlighted.End));
					}

					// Set new highlight
					highlighted = l;

					// Render highlighted item
					if((highlighted != null) && !highlighted.IsDisposed)
					{
						renderer.PlotLinedef(highlighted, General.Colors.Highlight);
						renderer.PlotVertex(highlighted.Start, renderer.DetermineVertexColor(highlighted.Start));
						renderer.PlotVertex(highlighted.End, renderer.DetermineVertexColor(highlighted.End));
					}

					// Done
					renderer.Finish();
					renderer.Present();
				}
			}

			// Show highlight info
			if((highlighted != null) && !highlighted.IsDisposed)
				General.Interface.ShowLinedefInfo(highlighted);
			else
				General.Interface.HideInfo();
		}

		//mxd
		private void alignTextureToLine(bool alignFloors, bool alignToFrontSide) {
			ICollection<Linedef> lines = General.Map.Map.GetSelectedLinedefs(true);

			if(lines.Count == 0 && highlighted != null && !highlighted.IsDisposed)
				lines.Add(highlighted);

			if(lines.Count == 0) {
				General.Interface.DisplayStatus(StatusType.Warning, "This action requires a selection");
				return;
			}

			//Create Undo
			string rest = (alignFloors ? "Floors" : "Ceilings") + " to " + (alignToFrontSide ? "Front" : "Back")+ " Side";
			General.Map.UndoRedo.CreateUndo("Align " + rest);
			int counter = 0;

			foreach(Linedef l in lines){
				Sector s = null;

				if(alignToFrontSide) {
					if(l.Front != null && l.Front.Sector != null) s = l.Front.Sector;
				} else {
					if(l.Back != null && l.Back.Sector != null)	s = l.Back.Sector;
				}

				if(s == null) continue;
				counter++;

				s.Fields.BeforeFieldsChange();

				float sourceAngle = (float)Math.Round(General.ClampAngle(alignToFrontSide ? -Angle2D.RadToDeg(l.Angle) + 90 : -Angle2D.RadToDeg(l.Angle) - 90), 1);
				if(!alignToFrontSide) sourceAngle = General.ClampAngle(sourceAngle + 180);

				//update angle
				UDMFTools.SetFloat(s.Fields, (alignFloors ? "rotationfloor" : "rotationceiling"), sourceAngle, 0f);

				//update offset
				Vector2D offset = (alignToFrontSide ? l.Start.Position : l.End.Position).GetRotated(Angle2D.DegToRad(sourceAngle));
				ImageData texture = General.Map.Data.GetFlatImage(s.LongFloorTexture);

				if((texture == null) || (texture == General.Map.Data.WhiteTexture) ||
				   (texture.Width <= 0) || (texture.Height <= 0) || !texture.IsImageLoaded) {
				}else{
					offset.x %= texture.Width / s.Fields.GetValue((alignFloors ? "xscalefloor" : "xscaleceiling"), 1.0f);
					offset.y %= texture.Height / s.Fields.GetValue((alignFloors ? "yscalefloor" : "yscaleceiling"), 1.0f);
				}

				UDMFTools.SetFloat(s.Fields, (alignFloors ? "xpanningfloor" : "xpanningceiling"), (float)Math.Round(-offset.x), 0f);
				UDMFTools.SetFloat(s.Fields, (alignFloors ? "ypanningfloor" : "ypanningceiling"), (float)Math.Round(offset.y), 0f);

				//update
				s.UpdateNeeded = true;
				s.UpdateCache();
			}

			General.Interface.DisplayStatus(StatusType.Info, "Aligned " +counter + " " + rest);

			//update
			General.Map.Map.Update();
			General.Interface.RedrawDisplay();
			General.Interface.RefreshInfo();
			General.Map.IsChanged = true;
		}

		//mxd
		private bool isInSelectionRect(Linedef l, List<Line2D> selectionOutline) {
			if(BuilderPlug.Me.MarqueSelectTouching) {
				bool selected = selectionrect.Contains(l.Start.Position.x, l.Start.Position.y) || selectionrect.Contains(l.End.Position.x, l.End.Position.y);

				//check intersections with outline
				if(!selected) {
					foreach(Line2D line in selectionOutline) {
						if(Line2D.GetIntersection(l.Line, line))
							return true;
					}
				}
				return selected;
			}

			return selectionrect.Contains(l.Start.Position.x, l.Start.Position.y) && selectionrect.Contains(l.End.Position.x, l.End.Position.y);
		}
		
		#endregion
		
		#region ================== Events

		public override void OnHelp()
		{
			General.ShowHelp("e_linedefs.html");
		}

		// Cancel mode
		public override void OnCancel()
		{
			base.OnCancel();

			// Return to this mode
			General.Editing.ChangeMode(new LinedefsMode());
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
			if(General.Map.UDMF) General.Interface.AddButton(BuilderPlug.Me.MenusForm.MakeGradientBrightness);//mxd
			General.Interface.AddButton(BuilderPlug.Me.MenusForm.MarqueSelectTouching); //mxd
			General.Interface.AddButton(BuilderPlug.Me.MenusForm.CurveLinedefs);
			if(General.Map.UDMF) General.Interface.AddButton(BuilderPlug.Me.MenusForm.TextureOffsetLock, ToolbarSection.Geometry); //mxd
			
			// Convert geometry selection to linedefs selection
			General.Map.Map.ConvertSelection(SelectionType.Linedefs);
			UpdateSelectionInfo(); //mxd
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
			if(General.Map.UDMF) General.Interface.RemoveButton(BuilderPlug.Me.MenusForm.MakeGradientBrightness);//mxd
			General.Interface.RemoveButton(BuilderPlug.Me.MenusForm.MarqueSelectTouching); //mxd
			General.Interface.RemoveButton(BuilderPlug.Me.MenusForm.CurveLinedefs);
			if(General.Map.UDMF) General.Interface.RemoveButton(BuilderPlug.Me.MenusForm.TextureOffsetLock); //mxd

			// Going to EditSelectionMode?
			if(General.Editing.NewMode is EditSelectionMode)
			{
				// Not pasting anything?
				EditSelectionMode editmode = (General.Editing.NewMode as EditSelectionMode);
				if(!editmode.Pasting)
				{
					// No selection made? But we have a highlight!
					if((General.Map.Map.GetSelectedLinedefs(true).Count == 0) && (highlighted != null))
					{
						// Make the highlight the selection
						highlighted.Selected = true;
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

			// Render lines
			if(renderer.StartPlotter(true))
			{
				renderer.PlotLinedefSet(General.Map.Map.Linedefs);
				if(!panning) //mxd
					for(int i = 0; i < Linedef.NUM_ARGS; i++) BuilderPlug.Me.PlotAssociations(renderer, association[i]);
				
				if((highlighted != null) && !highlighted.IsDisposed)
				{
					if(!panning) BuilderPlug.Me.PlotReverseAssociations(renderer, highlightasso);
					renderer.PlotLinedef(highlighted, General.Colors.Highlight);
				}
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

			// Render selection
			if(renderer.StartOverlay(true))
			{
				if(!panning && !selecting) { //mxd
					for (int i = 0; i < Linedef.NUM_ARGS; i++) BuilderPlug.Me.RenderAssociations(renderer, association[i]);
					if ((highlighted != null) && !highlighted.IsDisposed) BuilderPlug.Me.RenderReverseAssociations(renderer, highlightasso); //mxd
				}
				if(selecting) RenderMultiSelection();
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
					renderer.PlotLinedef(highlighted, renderer.DetermineLinedefColor(highlighted));
					renderer.PlotVertex(highlighted.Start, renderer.DetermineVertexColor(highlighted.Start));
					renderer.PlotVertex(highlighted.End, renderer.DetermineVertexColor(highlighted.End));
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
					highlighted.Selected = !highlighted.Selected;
					
					// Update display
					if(renderer.StartPlotter(false))
					{
						// Render highlighted item
						renderer.PlotLinedef(highlighted, General.Colors.Highlight);
						renderer.PlotVertex(highlighted.Start, renderer.DetermineVertexColor(highlighted.Start));
						renderer.PlotVertex(highlighted.End, renderer.DetermineVertexColor(highlighted.End));
						renderer.Finish();
						renderer.Present();
					}
				//mxd
				} else if(BuilderPlug.Me.AutoClearSelection && General.Map.Map.SelectedLinedefsCount > 0) {
					General.Map.Map.ClearSelectedLinedefs();
					General.Interface.RedrawDisplay();
				}

				//mxd
				UpdateSelectionInfo();
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
				if(!highlighted.Selected && (BuilderPlug.Me.AutoClearSelection || (General.Map.Map.SelectedLinedefsCount == 0)))
				{
					// Make this the only selection
					General.Map.Map.ClearSelectedLinedefs();
					highlighted.Selected = true;
					General.Interface.RedrawDisplay();
				}

				// Update display
				if(renderer.StartPlotter(false))
				{
					// Redraw highlight to show selection
					renderer.PlotLinedef(highlighted, renderer.DetermineLinedefColor(highlighted));
					renderer.PlotVertex(highlighted.Start, renderer.DetermineVertexColor(highlighted.Start));
					renderer.PlotVertex(highlighted.End, renderer.DetermineVertexColor(highlighted.End));
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

				if (drawmode.DrawPointAt(v))
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
				ICollection<Linedef> selected = General.Map.Map.GetSelectedLinedefs(true);
				if(selected.Count > 0)
				{
					if(General.Interface.IsActiveWindow)
					{
						// Show line edit dialog
						DialogResult result = General.Interface.ShowEditLinedefs(selected);
						General.Map.Map.Update();
						
						// When a single line was selected, deselect it now
						if (selected.Count == 1) {
							General.Map.Map.ClearSelectedLinedefs();
						} else if(result == DialogResult.Cancel) { //mxd. Restore selection...
							foreach (Linedef l in selected) l.Selected = true;
						}

						// Update entire display
						General.Map.Renderer2D.UpdateExtraFloorFlag(); //mxd
						General.Interface.RedrawDisplay();
					}
				}

				UpdateSelectionInfo(); //mxd
			}

			editpressed = false;
			base.OnEditEnd();
		}
		
		// Mouse moves
		public override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);
			if(panning) return; //mxd. Skip all this jass while panning

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
			else if(paintselectpressed && !editpressed && !selecting)  //mxd. Drag-select
			{
				// Find the nearest thing within highlight range
				Linedef l = General.Map.Map.NearestLinedefRange(mousemappos, BuilderPlug.Me.HighlightRange / renderer.Scale);

				if(l != null) {
					if(l != highlighted) {
						//toggle selected state
						if(General.Interface.ShiftState ^ BuilderPlug.Me.AdditiveSelect)
							l.Selected = true;
						else if(General.Interface.CtrlState)
							l.Selected = false;
						else
							l.Selected = !l.Selected;
						highlighted = l;

						UpdateSelectionInfo(); //mxd

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
			else if(e.Button == MouseButtons.None) // Not holding any buttons?
			{
				// Find the nearest linedef within highlight range
				Linedef l = General.Map.Map.NearestLinedefRange(mousemappos, BuilderPlug.Me.StitchRange / renderer.Scale);

				//mxd. Render insert vertex preview
				if(l != null) {
					bool snaptogrid = General.Interface.ShiftState ^ General.Interface.SnapToGrid;
					bool snaptonearest = General.Interface.CtrlState ^ General.Interface.AutoMerge;
					insertPreview = DrawGeometryMode.GetCurrentPosition(mousemappos, snaptonearest, snaptogrid, renderer, new List<DrawnVertex>()).pos;

					//render preview
					if(renderer.StartOverlay(true)) {
						if(!panning) {
							for(int i = 0; i < Linedef.NUM_ARGS; i++) BuilderPlug.Me.RenderAssociations(renderer, association[i]);
							if((highlighted != null) && !highlighted.IsDisposed) BuilderPlug.Me.RenderReverseAssociations(renderer, highlightasso); //mxd
						}
						float dist = Math.Min(Vector2D.Distance(mousemappos, insertPreview), BuilderPlug.Me.SplitLinedefsRange);
						byte alpha = (byte)(255 - (dist / BuilderPlug.Me.SplitLinedefsRange) * 128);
						float vsize = (renderer.VertexSize + 1.0f) / renderer.Scale;
						renderer.RenderRectangleFilled(new RectangleF(insertPreview.x - vsize, insertPreview.y - vsize, vsize * 2.0f, vsize * 2.0f), General.Colors.InfoLine.WithAlpha(alpha), true);
						renderer.Finish();
						renderer.Present();
					}
				} else if(insertPreview.IsFinite()) {
					insertPreview.x = float.NaN;

					//undraw preveiw
					if(renderer.StartOverlay(true)) {
						if(!panning) {
							for(int i = 0; i < Linedef.NUM_ARGS; i++) BuilderPlug.Me.RenderAssociations(renderer, association[i]);
							if((highlighted != null) && !highlighted.IsDisposed) BuilderPlug.Me.RenderReverseAssociations(renderer, highlightasso); //mxd
						}
						renderer.Finish();
						renderer.Present();
					}
				}

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

		//mxd
		protected override void BeginViewPan() {
			if(insertPreview.IsFinite()) {
				insertPreview.x = float.NaN;

				//undraw preveiw
				if(renderer.StartOverlay(true)) {
					renderer.Finish();
					renderer.Present();
				}
			}

			base.BeginViewPan();
		}

		//mxd
		protected override void OnPaintSelectBegin() {
			highlighted = null;
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
						// Select only this linedef for dragging
						General.Map.Map.ClearSelectedLinedefs();
						highlighted.Selected = true;
					}

					// Start dragging the selection
					if(!BuilderPlug.Me.DontMoveGeometryOutsideMapBoundary || canDrag()) //mxd
						General.Editing.ChangeMode(new DragLinedefsMode(mousedownmappos));
				}
			}
		}

		//mxd. Check if any selected linedef is outside of map boundary
		private bool canDrag() {
			ICollection<Linedef> selectedlines = General.Map.Map.GetSelectedLinedefs(true);
			int unaffectedCount = 0;

			foreach(Linedef l in selectedlines) {
				// Make sure the linedef is inside the map boundary
				if(l.Start.Position.x < General.Map.Config.LeftBoundary || l.Start.Position.x > General.Map.Config.RightBoundary
					|| l.Start.Position.y > General.Map.Config.TopBoundary || l.Start.Position.y < General.Map.Config.BottomBoundary
					|| l.End.Position.x < General.Map.Config.LeftBoundary || l.End.Position.x > General.Map.Config.RightBoundary
					|| l.End.Position.y > General.Map.Config.TopBoundary || l.End.Position.y < General.Map.Config.BottomBoundary) {

					l.Selected = false;
					unaffectedCount++;
				}
			}

			if(unaffectedCount == selectedlines.Count) {
				General.Interface.DisplayStatus(StatusType.Warning, "Unable to drag selection: " + (selectedlines.Count == 1 ? "selected linedef is" : "all of selected linedefs are") + " outside of map boundary!");
				General.Interface.RedrawDisplay();
				return false;
			}

			if(unaffectedCount > 0)
				General.Interface.DisplayStatus(StatusType.Warning, unaffectedCount + " of selected linedefs " + (unaffectedCount == 1 ? "is" : "are") + " outside of map boundary!");

			return true;
		}

		// This is called wheh selection ends
		protected override void OnEndMultiSelection()
		{
			bool selectionvolume = ((Math.Abs(selectionrect.Width) > 0.1f) && (Math.Abs(selectionrect.Height) > 0.1f));
			   
			if(selectionvolume)
			{
				List<Line2D> selectionOutline = new List<Line2D>() {
					new Line2D(selectionrect.Left, selectionrect.Top, selectionrect.Right, selectionrect.Top),
					new Line2D(selectionrect.Right, selectionrect.Top, selectionrect.Right, selectionrect.Bottom),
					new Line2D(selectionrect.Left, selectionrect.Bottom, selectionrect.Right, selectionrect.Bottom),
					new Line2D(selectionrect.Left, selectionrect.Bottom, selectionrect.Left, selectionrect.Top)
				                                                   };
				
				//mxd
				switch(marqueSelectionMode) {
					case MarqueSelectionMode.SELECT:
						foreach(Linedef l in General.Map.Map.Linedefs)
							l.Selected = isInSelectionRect(l, selectionOutline);
						break;

					case MarqueSelectionMode.ADD:
						foreach(Linedef l in General.Map.Map.Linedefs)
							l.Selected |= isInSelectionRect(l, selectionOutline);
						break;

					case MarqueSelectionMode.SUBTRACT:
						foreach(Linedef l in General.Map.Map.Linedefs)
							if(isInSelectionRect(l, selectionOutline)) l.Selected = false;
						break;

					default:
						foreach(Linedef l in General.Map.Map.Linedefs)
							if(!isInSelectionRect(l, selectionOutline)) l.Selected = false;
						break;
				}

				//mxd
				UpdateSelectionInfo();
			}
			
			base.OnEndMultiSelection();

			// Clear overlay
			if(renderer.StartOverlay(true)) renderer.Finish();

			// Redraw
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
			if((General.Map.Map.GetSelectedLinedefs(true).Count == 0) && (highlighted != null))
			{
				// Make the highlight the selection
				highlighted.Selected = true;
			}

			return base.OnCopyBegin();
		}

		//mxd
		public override void UpdateSelectionInfo() {
			if(General.Map.Map.SelectedLinedefsCount > 0)
				General.Interface.DisplayStatus(StatusType.Selection, General.Map.Map.SelectedLinedefsCount + (General.Map.Map.SelectedLinedefsCount == 1 ? " linedef" : " linedefs") + " selected.");
			else
				General.Interface.DisplayStatus(StatusType.Selection, string.Empty);
		}

		#endregion

		#region ================== Actions

		// This copies the properties
		[BeginAction("classiccopyproperties")]
		public void CopyProperties()
		{
			// Determine source linedefs
			ICollection<Linedef> sel = null;
			if(General.Map.Map.SelectedLinedefsCount > 0)
				sel = General.Map.Map.GetSelectedLinedefs(true);
			else if(highlighted != null)
				sel = new List<Linedef> {highlighted};
			
			if(sel != null)
			{
				// Copy properties from first source linedef
				BuilderPlug.Me.CopiedLinedefProps = new LinedefProperties(General.GetByIndex(sel, 0));
				General.Interface.DisplayStatus(StatusType.Action, "Copied linedef properties.");
			}
		}

		// This pastes the properties
		[BeginAction("classicpasteproperties")]
		public void PasteProperties()
		{
			if(BuilderPlug.Me.CopiedLinedefProps != null)
			{
				// Determine target linedefs
				ICollection<Linedef> sel = null;
				if(General.Map.Map.SelectedLinedefsCount > 0)
					sel = General.Map.Map.GetSelectedLinedefs(true);
				else if(highlighted != null)
				{
					sel = new List<Linedef>();
					sel.Add(highlighted);
				}
				
				if(sel != null)
				{
					// Apply properties to selection
					General.Map.UndoRedo.CreateUndo("Paste linedef properties");
					foreach(Linedef l in sel)
					{
						BuilderPlug.Me.CopiedLinedefProps.Apply(l);
						l.UpdateCache();
					}
					General.Interface.DisplayStatus(StatusType.Action, "Pasted linedef properties.");
					
					// Update and redraw
					General.Map.IsChanged = true;
					General.Interface.RefreshInfo();
					General.Map.Renderer2D.UpdateExtraFloorFlag(); //mxd
					General.Interface.RedrawDisplay();
				}
			}
		}

		// This keeps only the single-sided lines selected
		[BeginAction("selectsinglesided")]
		public void SelectSingleSided()
		{
			int counter = 0;
			ICollection<Linedef> selected = General.Map.Map.GetSelectedLinedefs(true);
			foreach(Linedef ld in selected)
			{
				if((ld.Front != null) && (ld.Back != null))
					ld.Selected = false;
				else
					counter++;
			}
			
			General.Interface.DisplayStatus(StatusType.Action, "Selected only single-sided linedefs (" + counter + ")");
			General.Interface.RedrawDisplay();
		}

		// This keeps only the double-sided lines selected
		[BeginAction("selectdoublesided")]
		public void SelectDoubleSided()
		{
			int counter = 0;
			ICollection<Linedef> selected = General.Map.Map.GetSelectedLinedefs(true);
			foreach(Linedef ld in selected)
			{
				if((ld.Front == null) || (ld.Back == null))
					ld.Selected = false;
				else
					counter++;
			}

			General.Interface.DisplayStatus(StatusType.Action, "Selected only double-sided linedefs (" + counter + ")");
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

			// Redraw
			General.Interface.RedrawDisplay();
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

		[BeginAction("deleteitem", BaseAction = true)]
		public void DeleteItem() {
			// Make list of selected linedefs
			ICollection<Linedef> selected = General.Map.Map.GetSelectedLinedefs(true);
			if((selected.Count == 0) && (highlighted != null) && !highlighted.IsDisposed) selected.Add(highlighted);
			if(selected.Count == 0) return;

			// Make undo
			if(selected.Count > 1) {
				General.Map.UndoRedo.CreateUndo("Delete " + selected.Count + " linedefs");
				General.Interface.DisplayStatus(StatusType.Action, "Deleted " + selected.Count + " linedefs.");
			} else {
				General.Map.UndoRedo.CreateUndo("Delete linedef");
				General.Interface.DisplayStatus(StatusType.Action, "Deleted a linedef.");
			}

			// Dispose selected linedefs
			foreach(Linedef ld in selected) ld.Dispose();

			// Update cache values
			General.Map.IsChanged = true;
			General.Map.Map.Update();

			// Invoke a new mousemove so that the highlighted item updates
			MouseEventArgs e = new MouseEventArgs(MouseButtons.None, 0, (int)mousepos.x, (int)mousepos.y, 0);
			OnMouseMove(e);

			// Redraw screen
			General.Interface.RedrawDisplay();
		}

		[BeginAction("dissolveitem", BaseAction = true)] //mxd
		public void DissolveItem()
		{
			// Make list of selected linedefs
			ICollection<Linedef> selected = General.Map.Map.GetSelectedLinedefs(true);
			if((selected.Count == 0) && (highlighted != null) && !highlighted.IsDisposed) selected.Add(highlighted);

			// Anything to do?
			if(selected.Count > 0)
			{
				// Make undo
				if(selected.Count > 1)
				{
					General.Map.UndoRedo.CreateUndo("Dissolve " + selected.Count + " linedefs");
					General.Interface.DisplayStatus(StatusType.Action, "Dissolved " + selected.Count + " linedefs.");
				}
				else
				{
					General.Map.UndoRedo.CreateUndo("Dissolve linedef");
					General.Interface.DisplayStatus(StatusType.Action, "Dissolved a linedef.");
				}

				//mxd. Find sectors, which will become invalid after linedefs removal.
				Dictionary<Sector, Vector2D> toMerge = new Dictionary<Sector, Vector2D>();

				foreach(Linedef l in selected) {
					if(l.Front != null && l.Front.Sector.Sidedefs.Count < 4 && !toMerge.ContainsKey(l.Front.Sector))
						toMerge.Add(l.Front.Sector, new Vector2D(l.Front.Sector.BBox.Location.X + l.Front.Sector.BBox.Width / 2, l.Front.Sector.BBox.Location.Y + l.Front.Sector.BBox.Height / 2));

					if(l.Back != null && l.Back.Sector.Sidedefs.Count < 4 && !toMerge.ContainsKey(l.Back.Sector))
						toMerge.Add(l.Back.Sector, new Vector2D(l.Back.Sector.BBox.Location.X + l.Back.Sector.BBox.Width / 2, l.Back.Sector.BBox.Location.Y + l.Back.Sector.BBox.Height / 2));
				}

				General.Map.Map.BeginAddRemove(); //mxd
				
				// Dispose selected linedefs
				foreach(Linedef ld in selected) {
					//mxd. If there are different sectors on both sides, join them
					if(ld.Front != null && ld.Front.Sector != null && ld.Back != null && ld.Back.Sector != null && ld.Front.Sector.Index != ld.Back.Sector.Index) {
						if(ld.Front.Sector.BBox.Width * ld.Front.Sector.BBox.Height > ld.Back.Sector.BBox.Width * ld.Back.Sector.BBox.Height)
							ld.Back.Sector.Join(ld.Front.Sector);
						else
							ld.Front.Sector.Join(ld.Back.Sector);
					}

					ld.Dispose();
				}

				//mxd
				General.Map.Map.EndAddRemove();
				Tools.MergeInvalidSectors(toMerge);
				
				// Update cache values
				General.Map.IsChanged = true;
				General.Map.Map.Update();

				// Invoke a new mousemove so that the highlighted item updates
				MouseEventArgs e = new MouseEventArgs(MouseButtons.None, 0, (int)mousepos.x, (int)mousepos.y, 0);
				OnMouseMove(e);
				
				// Redraw screen
				General.Interface.RedrawDisplay();
			}
		}
		
		[BeginAction("splitlinedefs")]
		public void SplitLinedefs()
		{
			// Make list of selected linedefs
			ICollection<Linedef> selected = General.Map.Map.GetSelectedLinedefs(true);
			if((selected.Count == 0) && (highlighted != null) && !highlighted.IsDisposed) selected.Add(highlighted);

			// Anything to do?
			if(selected.Count > 0)
			{
				// Make undo
				if(selected.Count > 1)
				{
					General.Map.UndoRedo.CreateUndo("Split " + selected.Count + " linedefs");
					General.Interface.DisplayStatus(StatusType.Action, "Split " + selected.Count + " linedefs.");
				}
				else
				{
					General.Map.UndoRedo.CreateUndo("Split linedef");
					General.Interface.DisplayStatus(StatusType.Action, "Split a linedef.");
				}
				
				// Go for all linedefs to split
				foreach(Linedef ld in selected)
				{
					Vertex splitvertex;

					// Linedef highlighted?
					if(ld == highlighted)
					{
						// Split at nearest position on the line
						Vector2D nearestpos = ld.NearestOnLine(mousemappos);
						splitvertex = General.Map.Map.CreateVertex(nearestpos);
					}
					else
					{
						// Split in middle of line
						splitvertex = General.Map.Map.CreateVertex(ld.GetCenterPoint());
					}

					if(splitvertex == null)
					{
						General.Map.UndoRedo.WithdrawUndo();
						break;
					}
					
					// Snap to map format accuracy
					splitvertex.SnapToAccuracy();

					// Split the line
					Linedef sld = ld.Split(splitvertex);
					if(sld == null)
					{
						General.Map.UndoRedo.WithdrawUndo();
						break;
					}
					//BuilderPlug.Me.AdjustSplitCoordinates(ld, sld);
				}

				// Update cache values
				General.Map.IsChanged = true;
				General.Map.Map.Update();
				
				// Redraw screen
				General.Interface.RedrawDisplay();
			}
		}
		
		[BeginAction("curvelinesmode")]
		public void CurveLinedefs()
		{
			// No selected lines?
			ICollection<Linedef> selected = General.Map.Map.GetSelectedLinedefs(true);
			if(selected.Count == 0)
			{
				// Anything highlighted?
				if(highlighted != null)
				{
					// Select the highlighted item
					highlighted.Selected = true;
					selected.Add(highlighted);
				}
			}

			// Any selected lines?
			if(selected.Count > 0)
			{
				// Go into curve linedefs mode
				General.Editing.ChangeMode(new CurveLinedefsMode(new LinedefsMode()));
			}
		}
		
		[BeginAction("fliplinedefs")]
		public void FlipLinedefs()
		{
			// No selected lines?
			ICollection<Linedef> selected = General.Map.Map.GetSelectedLinedefs(true);
			if(selected.Count == 0)
			{
				// Anything highlighted?
				if(highlighted != null)
				{
					// Select the highlighted item
					highlighted.Selected = true;
					selected.Add(highlighted);
				}
			}

			// Any selected lines?
			if(selected.Count > 0)
			{
				// Make undo
				if(selected.Count > 1)
				{
					General.Map.UndoRedo.CreateUndo("Flip " + selected.Count + " linedefs");
					General.Interface.DisplayStatus(StatusType.Action, "Flipped " + selected.Count + " linedefs.");
				}
				else
				{
					General.Map.UndoRedo.CreateUndo("Flip linedef");
					General.Interface.DisplayStatus(StatusType.Action, "Flipped a linedef.");
				}

				// Flip all selected linedefs
				foreach(Linedef l in selected)
				{
					l.FlipVertices();
					l.FlipSidedefs();
				}
				
				// Remove selection if only one was selected
				if(selected.Count == 1)
				{
					foreach(Linedef ld in selected) ld.Selected = false;
					selected.Clear();
				}

				// Redraw
				General.Map.Map.Update();
				General.Map.IsChanged = true;
				General.Interface.RefreshInfo();
				General.Interface.RedrawDisplay();
			}
		}

		[BeginAction("flipsidedefs")]
		public void FlipSidedefs()
		{
			// No selected lines?
			ICollection<Linedef> selected = General.Map.Map.GetSelectedLinedefs(true);
			if(selected.Count == 0)
			{
				// Anything highlighted?
				if(highlighted != null)
				{
					// Select the highlighted item
					highlighted.Selected = true;
					selected.Add(highlighted);
				}
			}

			// Any selected lines?
			if(selected.Count > 0)
			{
				// Make undo
				if(selected.Count > 1)
				{
					General.Map.UndoRedo.CreateUndo("Flip " + selected.Count + " sidedefs");
					General.Interface.DisplayStatus(StatusType.Action, "Flipped " + selected.Count + " sidedefs.");
				}
				else
				{
					General.Map.UndoRedo.CreateUndo("Flip sidedefs");
					General.Interface.DisplayStatus(StatusType.Action, "Flipped sidedefs.");
				}

				// Flip sidedefs in all selected linedefs
				foreach(Linedef l in selected)
				{
					l.FlipSidedefs();
					if(l.Front != null) l.Front.Sector.UpdateNeeded = true;
					if(l.Back != null) l.Back.Sector.UpdateNeeded = true;
				}

				// Remove selection if only one was selected
				if(selected.Count == 1)
				{
					foreach(Linedef ld in selected) ld.Selected = false;
					selected.Clear();
				}

				// Redraw
				General.Map.Map.Update();
				General.Map.IsChanged = true;
				General.Interface.RefreshInfo();
				General.Interface.RedrawDisplay();
			}
		}

		//mxd. Make gradient brightness
		[BeginAction("gradientbrightness")]
		public void MakeGradientBrightness() {
			if(!General.Map.UDMF) return;

			// Need at least 3 selected linedefs
			// The first and last are not modified
			ICollection<Linedef> orderedselection = General.Map.Map.GetSelectedLinedefs(true);
			if(orderedselection.Count > 2) {
				General.Interface.DisplayStatus(StatusType.Action, "Created gradient brightness over selected linedefs.");
				General.Map.UndoRedo.CreateUndo("Linedefs gradient brightness");

				Linedef start = General.GetByIndex(orderedselection, 0);
				Linedef end = General.GetByIndex(orderedselection, orderedselection.Count - 1);

				const string lightKey = "light";
				const string lightAbsKey = "lightabsolute";
				float startbrightness = float.NaN;
				float endbrightness = float.NaN;

				//get total brightness of start sidedef(s)
				if(start.Front != null) {
					if(start.Front.Fields.GetValue(lightAbsKey, false)) {
						startbrightness = start.Front.Fields.GetValue(lightKey, 0);
					} else {
						startbrightness = Math.Min(255, Math.Max(0, (float)start.Front.Sector.Brightness + start.Front.Fields.GetValue(lightKey, 0)));
					}
				}

				if(start.Back != null) {
					float b;

					if(start.Back.Fields.GetValue(lightAbsKey, false)) {
						b = start.Back.Fields.GetValue(lightKey, 0);
					} else {
						b = Math.Min(255, Math.Max(0, (float)start.Back.Sector.Brightness + start.Back.Fields.GetValue(lightKey, 0)));
					}

					startbrightness = (float.IsNaN(startbrightness) ? b : (startbrightness + b) / 2);
				}

				//get total brightness of end sidedef(s)
				if(end.Front != null) {
					if(end.Front.Fields.GetValue(lightAbsKey, false)) {
						endbrightness = end.Front.Fields.GetValue(lightKey, 0);
					} else {
						endbrightness = Math.Min(255, Math.Max(0, (float)end.Front.Sector.Brightness + end.Front.Fields.GetValue(lightKey, 0)));
					}
				}

				if(end.Back != null) {
					float b;

					if(end.Back.Fields.GetValue(lightAbsKey, false)) {
						b = end.Back.Fields.GetValue(lightKey, 0);
					} else {
						b = Math.Min(255, Math.Max(0, (float)end.Back.Sector.Brightness + end.Back.Fields.GetValue(lightKey, 0)));
					}

					endbrightness = (float.IsNaN(endbrightness) ? b : (endbrightness + b) / 2);
				}

				float delta = endbrightness - startbrightness;

				// Go for all sectors in between first and last
				int index = 0;
				foreach(Linedef l in orderedselection) {
					float u = index / (float)(orderedselection.Count - 1);
					float b = startbrightness + delta * u;

					if(l.Front != null) {
						l.Front.Fields.BeforeFieldsChange();

						//absolute flag set?
						if(l.Front.Fields.GetValue(lightAbsKey, false)) {
							if(l.Front.Fields.ContainsKey(lightKey))
								l.Front.Fields[lightKey].Value = (int)b;
							else
								l.Front.Fields.Add(lightKey, new UniValue(UniversalType.Integer, (int)b));
						} else {
							if(l.Front.Fields.ContainsKey(lightKey))
								l.Front.Fields[lightKey].Value = (int)b - l.Front.Sector.Brightness;
							else
								l.Front.Fields.Add(lightKey, new UniValue(UniversalType.Integer, (int)b - l.Front.Sector.Brightness));
						}
					}

					if(l.Back != null) {
						l.Back.Fields.BeforeFieldsChange();

						//absolute flag set?
						if(l.Back.Fields.GetValue(lightAbsKey, false)) {
							if(l.Back.Fields.ContainsKey(lightKey))
								l.Back.Fields[lightKey].Value = (int)b;
							else
								l.Back.Fields.Add(lightKey, new UniValue(UniversalType.Integer, (int)b));
						} else {
							if(l.Back.Fields.ContainsKey(lightKey))
								l.Back.Fields[lightKey].Value = (int)b - l.Back.Sector.Brightness;
							else
								l.Back.Fields.Add(lightKey, new UniValue(UniversalType.Integer, (int)b - l.Back.Sector.Brightness));
						}
					}

					index++;
				}

				// Update
				General.Map.Map.Update();
				General.Interface.RedrawDisplay();
				General.Interface.RefreshInfo();
				General.Map.IsChanged = true;
			} else {
				General.Interface.DisplayStatus(StatusType.Warning, "Select at least 3 linedefs first!");
			}
		}

		[BeginAction("placethings")] //mxd
		public void PlaceThings() {
			// Make list of selected linedefs
			ICollection<Linedef> lines = General.Map.Map.GetSelectedLinedefs(true);
			List<Vector2D> positions = new List<Vector2D>();

			if(lines.Count == 0) {
				if (highlighted != null && !highlighted.IsDisposed) {
					lines.Add(highlighted);
				} else {
					General.Interface.DisplayStatus(StatusType.Warning, "This action requires selection of some description!");
					return;
				}
			}

			// Make list of vertex positions
			foreach(Linedef l in lines) {
				if(!positions.Contains(l.Start.Position)) positions.Add(l.Start.Position);
				if(!positions.Contains(l.End.Position)) positions.Add(l.End.Position);
			}

			if(positions.Count < 1) {
				General.Interface.DisplayStatus(StatusType.Warning, "Unable to get vertex positions from selection!");
				return;
			}

			placeThingsAtPositions(positions);
		}

		//mxd
		[BeginAction("alignfloortofront")]
		public void AlignFloorToFront() {
			if(!General.Map.UDMF) return;
			alignTextureToLine(true, true);
		}

		//mxd
		[BeginAction("alignfloortoback")]
		public void AlignFloorToBack() {
			if(!General.Map.UDMF) return;
			alignTextureToLine(true, false);
		}

		//mxd
		[BeginAction("alignceilingtofront")]
		public void AlignCeilingToFront() {
			if(!General.Map.UDMF) return;
			alignTextureToLine(false, true);
		}

		//mxd
		[BeginAction("alignceilingtoback")]
		public void AlignCeilingToBack() {
			if(!General.Map.UDMF) return;
			alignTextureToLine(false, false);
		}

		//mxd
		[BeginAction("selectsimilar")]
		public void SelectSimilar() {
			ICollection<Linedef> selection = General.Map.Map.GetSelectedLinedefs(true);

			if(selection.Count == 0) {
				General.Interface.DisplayStatus(StatusType.Warning, "This action requires a selection!");
				return;
			}

			var form = new SelectSimilarElementOptionsPanel();
			if(form.Setup(this)) form.ShowDialog();
		}

		#endregion
	}
}
