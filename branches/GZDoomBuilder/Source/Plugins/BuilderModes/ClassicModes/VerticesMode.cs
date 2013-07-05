
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
using System.Windows.Forms;
using CodeImp.DoomBuilder.Windows;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Rendering;
using CodeImp.DoomBuilder.Geometry;
using CodeImp.DoomBuilder.Editing;
using CodeImp.DoomBuilder.Actions;

#endregion

namespace CodeImp.DoomBuilder.BuilderModes
{
	[EditMode(DisplayName = "Vertices Mode",
			  SwitchAction = "verticesmode",	// Action name used to switch to this mode
		      ButtonImage = "VerticesMode.png",	// Image resource name for the button
			  ButtonOrder = int.MinValue + 0,	// Position of the button (lower is more to the left)
			  ButtonGroup = "000_editing",
			  UseByDefault = true,
			  SafeStartMode = true)]

	public class VerticesMode : BaseClassicMode
	{
		#region ================== Constants

		#endregion

		#region ================== Variables

		// Highlighted item
		protected Vertex highlighted;
		private Linedef highlightedLine;

		// Interface
		private bool editpressed;

		#endregion

		#region ================== Properties

		public override object HighlightedObject { get { return highlighted; } }

		#endregion

		#region ================== Constructor / Disposer

		#endregion

		#region ================== Methods

		public override void OnHelp()
		{
			General.ShowHelp("e_vertices.html");
		}

		// Cancel mode
		public override void OnCancel()
		{
			base.OnCancel();
			
			// Return to this mode
			General.Editing.ChangeMode(new VerticesMode());
		}

		// Mode engages
		public override void OnEngage()
		{
			base.OnEngage();
			renderer.SetPresentation(Presentation.Standard);

			// Add toolbar buttons
			General.Interface.AddButton(BuilderPlug.Me.MenusForm.CopyProperties);
			General.Interface.AddButton(BuilderPlug.Me.MenusForm.PasteProperties);
			if(General.Map.UDMF) General.Interface.AddButton(BuilderPlug.Me.MenusForm.TextureOffsetLock, ToolbarSection.Geometry); //mxd
			
			// Convert geometry selection to vertices only
			General.Map.Map.ConvertSelection(SelectionType.Vertices);
		}

		// Mode disengages
		public override void OnDisengage()
		{
			base.OnDisengage();

			// Remove toolbar buttons
			General.Interface.RemoveButton(BuilderPlug.Me.MenusForm.CopyProperties);
			General.Interface.RemoveButton(BuilderPlug.Me.MenusForm.PasteProperties);
			if(General.Map.UDMF) General.Interface.RemoveButton(BuilderPlug.Me.MenusForm.TextureOffsetLock); //mxd
			
			// Going to EditSelectionMode?
			if(General.Editing.NewMode is EditSelectionMode)
			{
				// Not pasting anything?
				EditSelectionMode editmode = (General.Editing.NewMode as EditSelectionMode);
				if(!editmode.Pasting)
				{
					// No selection made? But we have a highlight!
					if((General.Map.Map.GetSelectedVertices(true).Count == 0) && (highlighted != null))
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

			// Render lines and vertices
			if(renderer.StartPlotter(true))
			{
				renderer.PlotLinedefSet(General.Map.Map.Linedefs);
				renderer.PlotVerticesSet(General.Map.Map.Vertices);
				if((highlighted != null) && !highlighted.IsDisposed)
					renderer.PlotVertex(highlighted, ColorCollection.HIGHLIGHT);
				renderer.Finish();
			}

			// Render things
			if(renderer.StartThings(true))
			{
				renderer.RenderThingSet(General.Map.ThingsFilter.HiddenThings, Presentation.THINGS_HIDDEN_ALPHA);
				renderer.RenderThingSet(General.Map.ThingsFilter.VisibleThings, 1.0f);
				renderer.Finish();
			}

			// Selecting?
			if(selecting)
			{
				// Render selection
				if(renderer.StartOverlay(true))
				{
					RenderMultiSelection();
					renderer.Finish();
				}
			}

			renderer.Present();
		}
		
		// This highlights a new item
		protected void Highlight(Vertex v)
		{
			// Update display
			if(renderer.StartPlotter(false))
			{
				// Undraw previous highlight
				if(highlighted != null && !highlighted.IsDisposed)
					renderer.PlotVertex(highlighted, renderer.DetermineVertexColor(highlighted));

				// Set new highlight
				highlighted = v;

				// Render highlighted item
				if(highlighted != null && !highlighted.IsDisposed)
					renderer.PlotVertex(highlighted, ColorCollection.HIGHLIGHT);
				
				// Done
				renderer.Finish();
				renderer.Present();
			}

			// Show highlight info
			if((highlighted != null) && !highlighted.IsDisposed)
				General.Interface.ShowVertexInfo(highlighted);
			else
				General.Interface.HideInfo();
		}

		//mxd
		private void HighlightLine(Linedef l) {
			// Update display
			if(renderer.StartPlotter(false)) {
				// Undraw previous highlight
				if((highlightedLine != null) && !highlightedLine.IsDisposed) {
					renderer.PlotLinedef(highlightedLine, renderer.DetermineLinedefColor(highlightedLine));

					if(highlighted == null) {
						renderer.PlotVertex(highlightedLine.Start, renderer.DetermineVertexColor(highlightedLine.Start));
						renderer.PlotVertex(highlightedLine.End, renderer.DetermineVertexColor(highlightedLine.End));
					}
				}

				// Set new highlight
				highlightedLine = l;

				// Render highlighted item
				if((highlightedLine != null) && !highlightedLine.IsDisposed) {
					renderer.PlotLinedef(highlightedLine, General.Colors.InfoLine);

					if(highlighted != null && !highlighted.IsDisposed) {
						renderer.PlotVertex(highlightedLine.Start, highlightedLine.Start == highlighted ? ColorCollection.HIGHLIGHT : renderer.DetermineVertexColor(highlightedLine.Start));
						renderer.PlotVertex(highlightedLine.End, highlightedLine.End == highlighted ? ColorCollection.HIGHLIGHT : renderer.DetermineVertexColor(highlightedLine.End));
					}else{
						renderer.PlotVertex(highlightedLine.Start, renderer.DetermineVertexColor(highlightedLine.Start));
						renderer.PlotVertex(highlightedLine.End, renderer.DetermineVertexColor(highlightedLine.End));
					}
				}

				// Done
				renderer.Finish();
				renderer.Present();
			}
		}
		
		// Selection
		protected override void OnSelectBegin()
		{
			// Item highlighted?
			if((highlighted != null) && !highlighted.IsDisposed)
			{
				// Redraw highlight to show selection
				if(renderer.StartPlotter(false))
				{
					renderer.PlotVertex(highlighted, renderer.DetermineVertexColor(highlighted));
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
					
					// Render highlighted item
					if(renderer.StartPlotter(false))
					{
						renderer.PlotVertex(highlighted, ColorCollection.HIGHLIGHT);
						renderer.Finish();
						renderer.Present();
					}
				//mxd
				} else if(BuilderPlug.Me.AutoClearSelection && General.Map.Map.SelectedVerticessCount > 0) {
					General.Map.Map.ClearSelectedVertices();
					General.Interface.RedrawDisplay();
				}
			}

			base.OnSelectEnd();
		}
		
		// Start editing
		protected override void OnEditBegin()
		{
			bool snaptogrid = General.Interface.ShiftState ^ General.Interface.SnapToGrid;
			bool snaptonearest = General.Interface.CtrlState ^ General.Interface.AutoMerge;
			
			// Vertex highlighted?
			if((highlighted != null) && !highlighted.IsDisposed)
			{
				// Edit pressed in this mode
				editpressed = true;

				// Highlighted item not selected?
				if(!highlighted.Selected && (BuilderPlug.Me.AutoClearSelection || (General.Map.Map.SelectedVerticessCount == 0)))
				{
					// Make this the only selection
					General.Map.Map.ClearSelectedVertices();
					highlighted.Selected = true;
					General.Interface.RedrawDisplay();
				}

				// Update display
				if(renderer.StartPlotter(false))
				{
					// Redraw highlight to show selection
					renderer.PlotVertex(highlighted, renderer.DetermineVertexColor(highlighted));
					renderer.Finish();
					renderer.Present();
				}
			}
			else if(!selecting) //mxd. We don't want to do this stuff while multiselecting
			{
				// Find the nearest linedef within highlight range
				Linedef l = General.Map.Map.NearestLinedefRange(mousemappos, BuilderPlug.Me.SplitLinedefsRange / renderer.Scale);
				if(l != null)
				{
					// Create undo
					General.Map.UndoRedo.CreateUndo("Split linedef");

					Vector2D insertpos;

					// Snip to grid also?
					if(snaptogrid)
					{
						// Find all points where the grid intersects the line
						List<Vector2D> points = l.GetGridIntersections();
						insertpos = mousemappos;
						float distance = float.MaxValue;
						foreach(Vector2D p in points)
						{
							float pdist = Vector2D.DistanceSq(p, mousemappos);
							if(pdist < distance)
							{
								insertpos = p;
								distance = pdist;
							}
						}
					}
					else
					{
						// Just use the nearest point on line
						insertpos = l.NearestOnLine(mousemappos);
					}

					// Make the vertex
					Vertex v = General.Map.Map.CreateVertex(insertpos);
					if(v == null)
					{
						General.Map.UndoRedo.WithdrawUndo();
						return;
					}
					
					// Snap to map format accuracy
					v.SnapToAccuracy();

					// Split the line with this vertex
					Linedef sld = l.Split(v);
					if(sld == null)
					{
						General.Map.UndoRedo.WithdrawUndo();
						return;
					}
					BuilderPlug.Me.AdjustSplitCoordinates(l, sld);
					
					// Update
					General.Map.Map.Update();

					// Highlight it
					Highlight(v);

					// Redraw display
					General.Interface.RedrawDisplay();
				}
				else
				{
					// Start drawing mode
					DrawGeometryMode drawmode = new DrawGeometryMode();
					DrawnVertex v = DrawGeometryMode.GetCurrentPosition(mousemappos, snaptonearest, snaptogrid, renderer, new List<DrawnVertex>());

					if (drawmode.DrawPointAt(v))
						General.Editing.ChangeMode(drawmode);
					else
						General.Interface.DisplayStatus(StatusType.Warning, "Failed to draw point: outside of map boundaries.");
				}
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
				ICollection<Vertex> selected = General.Map.Map.GetSelectedVertices(true);
				if(selected.Count > 0)
				{
					if(General.Interface.IsActiveWindow)
					{
						// Show line edit dialog
						General.Interface.ShowEditVertices(selected);
						General.Map.Map.Update();

						// When a single vertex was selected, deselect it now
						if(selected.Count == 1) General.Map.Map.ClearSelectedVertices();

						// Update entire display
						General.Map.Renderer2D.Update3dFloorTagsList(); //mxd
						General.Interface.RedrawDisplay();
					}
				}
			}

			editpressed = false;
			base.OnEditEnd();
		}
		
		// Mouse moves
		public override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);

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
				Vertex v = General.Map.Map.NearestVertexSquareRange(mousemappos, BuilderPlug.Me.HighlightRange / renderer.Scale);

				if(v != null) {
					if(v != highlighted) {
						//toggle selected state
						if(General.Interface.ShiftState ^ BuilderPlug.Me.AdditiveSelect)
							v.Selected = true;
						else if(General.Interface.CtrlState)
							v.Selected = false;
						else
							v.Selected = !v.Selected;
						highlighted = v;

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
				//mxd
				// Find the nearest linedef within split linedefs range
				Linedef l = General.Map.Map.NearestLinedefRange(mousemappos, BuilderPlug.Me.SplitLinedefsRange / renderer.Scale);

				// Highlight if not the same
				if(l != highlightedLine) HighlightLine(l);
				
				// Find the nearest vertex within highlight range
				Vertex v = General.Map.Map.NearestVertexSquareRange(mousemappos, BuilderPlug.Me.HighlightRange / renderer.Scale);

				// Highlight if not the same
				if(v != highlighted) Highlight(v);
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
						// Select only this vertex for dragging
						General.Map.Map.ClearSelectedVertices();
						highlighted.Selected = true;
					}

					// Start dragging the selection
					if(!BuilderPlug.Me.DontMoveGeometryOutsideMapBoundary || canDrag()) //mxd
						General.Editing.ChangeMode(new DragVerticesMode(highlighted, mousedownmappos));
				}
			}
		}

		//mxd. Check if any selected vertex is outside of map boundary
		private bool canDrag() {
			ICollection<Vertex> selectedverts = General.Map.Map.GetSelectedVertices(true);
			int unaffectedCount = 0;

			foreach(Vertex v in selectedverts) {
				// Make sure the vertex is inside the map boundary
				if(v.Position.x < General.Map.Config.LeftBoundary || v.Position.x > General.Map.Config.RightBoundary
					|| v.Position.y > General.Map.Config.TopBoundary || v.Position.y < General.Map.Config.BottomBoundary) {

					v.Selected = false;
					unaffectedCount++;
				}
			}

			if(unaffectedCount == selectedverts.Count) {
				General.Interface.DisplayStatus(StatusType.Warning, "Unable to drag selection: " + (selectedverts.Count == 1 ? "selected vertex is" : "all of selected vertices are") + " outside of map boundary!");
				General.Interface.RedrawDisplay();
				return false;
			}
			
			if(unaffectedCount > 0)
				General.Interface.DisplayStatus(StatusType.Warning, unaffectedCount + " of selected vertices " + (unaffectedCount == 1 ? "is" : "are") + " outside of map boundary!");

			return true;
		}

		// This is called wheh selection ends
		protected override void OnEndMultiSelection()
		{
			bool selectionvolume = ((Math.Abs(base.selectionrect.Width) > 0.1f) && (Math.Abs(base.selectionrect.Height) > 0.1f));

			if(selectionvolume)
			{
				//mxd
				if(marqueSelectionMode == MarqueSelectionMode.SELECT) {
					// Go for all vertices
					foreach(Vertex v in General.Map.Map.Vertices)
						v.Selected = selectionrect.Contains(v.Position.x, v.Position.y);
				} else if(marqueSelectionMode == MarqueSelectionMode.ADD) {
					// Go for all vertices
					foreach(Vertex v in General.Map.Map.Vertices)
						v.Selected |= selectionrect.Contains(v.Position.x, v.Position.y);
				} else if(marqueSelectionMode == MarqueSelectionMode.SUBTRACT) {
					// Go for all vertices
					foreach(Vertex v in General.Map.Map.Vertices)
						if(selectionrect.Contains(v.Position.x, v.Position.y)) v.Selected = false;
				} else { //should be Intersect
					// Go for all vertices
					foreach(Vertex v in General.Map.Map.Vertices)
						if(!selectionrect.Contains(v.Position.x, v.Position.y)) v.Selected = false;
				}
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
			if((General.Map.Map.GetSelectedVertices(true).Count == 0) && (highlighted != null))
			{
				// Make the highlight the selection
				highlighted.Selected = true;
			}
			
			return base.OnCopyBegin();
		}
		
		#endregion

		#region ================== Actions

		// This copies the properties
		[BeginAction("classiccopyproperties")]
		public void CopyProperties()
		{
			// Determine source vertices
			ICollection<Vertex> sel = null;
			if(General.Map.Map.SelectedVerticessCount > 0)
				sel = General.Map.Map.GetSelectedVertices(true);
			else if(highlighted != null)
			{
				sel = new List<Vertex>();
				sel.Add(highlighted);
			}

			if(sel != null)
			{
				// Copy properties from first source vertex
				BuilderPlug.Me.CopiedVertexProps = new VertexProperties(General.GetByIndex(sel, 0));
				General.Interface.DisplayStatus(StatusType.Action, "Copied vertex properties.");
			}
		}

		// This pastes the properties
		[BeginAction("classicpasteproperties")]
		public void PasteProperties()
		{
			if(BuilderPlug.Me.CopiedVertexProps != null)
			{
				// Determine target vertices
				ICollection<Vertex> sel = null;
				if(General.Map.Map.SelectedVerticessCount > 0)
					sel = General.Map.Map.GetSelectedVertices(true);
				else if(highlighted != null)
				{
					sel = new List<Vertex>();
					sel.Add(highlighted);
				}

				if(sel != null)
				{
					// Apply properties to selection
					General.Map.UndoRedo.CreateUndo("Paste vertex properties");
					foreach(Vertex v in sel)
					{
						BuilderPlug.Me.CopiedVertexProps.Apply(v);
					}
					General.Interface.DisplayStatus(StatusType.Action, "Pasted vertex properties.");

					// Update and redraw
					General.Map.IsChanged = true;
					General.Interface.RefreshInfo();
					General.Interface.RedrawDisplay();
				}
			}
		}

		// This clears the selection
		[BeginAction("clearselection", BaseAction = true)]
		public void ClearSelection()
		{
			// Clear selection
			General.Map.Map.ClearAllSelected();

			// Redraw
			General.Interface.RedrawDisplay();
		}
		
		// This creates a new vertex at the mouse position
		[BeginAction("insertitem", BaseAction = true)]
		public virtual void InsertVertexAction() { VerticesMode.InsertVertex(mousemappos, renderer.Scale); }
		public static void InsertVertex(Vector2D mousemappos, float rendererscale)
		{
			bool snaptogrid = General.Interface.ShiftState ^ General.Interface.SnapToGrid;
			bool snaptonearest = General.Interface.CtrlState ^ General.Interface.AutoMerge;

			// Mouse in window?
			if(General.Interface.MouseInDisplay)
			{
				Vector2D insertpos;
				Linedef l = null;
				
				// Create undo
				General.Map.UndoRedo.CreateUndo("Insert vertex");

				// Snap to geometry?
				l = General.Map.Map.NearestLinedefRange(mousemappos, BuilderPlug.Me.SplitLinedefsRange / rendererscale);
				if(snaptonearest && (l != null))
				{
					// Snip to grid also?
					if(snaptogrid)
					{
						// Find all points where the grid intersects the line
						List<Vector2D> points = l.GetGridIntersections();
						insertpos = mousemappos;
						float distance = float.MaxValue;
						foreach(Vector2D p in points)
						{
							float pdist = Vector2D.DistanceSq(p, mousemappos);
							if(pdist < distance)
							{
								insertpos = p;
								distance = pdist;
							}
						}
					}
					else
					{
						// Just use the nearest point on line
						insertpos = l.NearestOnLine(mousemappos);
					}
				}
				// Snap to grid?
				else if(snaptogrid)
				{
					// Snap to grid
					insertpos = General.Map.Grid.SnappedToGrid(mousemappos);
				}
				else
				{
					// Just insert here, don't snap to anything
					insertpos = mousemappos;
				}

				// Make the vertex
				Vertex v = General.Map.Map.CreateVertex(insertpos);
				if(v == null)
				{
					General.Map.UndoRedo.WithdrawUndo();
					return;
				}

				// Snap to map format accuracy
				v.SnapToAccuracy();

				// Split the line with this vertex
				if(snaptonearest)
				{
					//mxd. Check if snapped vertex is on top of a linedef
					l = General.Map.Map.NearestLinedefRange(insertpos, 1 / rendererscale);
					if(l != null && l.SideOfLine(insertpos) != 0)
						l = null;
					
					if(l != null) {
						General.Interface.DisplayStatus(StatusType.Action, "Split a linedef.");
						Linedef sld = l.Split(v);
						if(sld == null) {
							General.Map.UndoRedo.WithdrawUndo();
							return;
						}
						BuilderPlug.Me.AdjustSplitCoordinates(l, sld);
					}
				}
				else
				{
					General.Interface.DisplayStatus(StatusType.Action, "Inserted a vertex.");
				}

				// Update
				General.Map.Map.Update();

				// Redraw screen
				General.Interface.RedrawDisplay();
			}
		}

		[BeginAction("deleteitem", BaseAction = true)] //mxd
		public void DeleteItem() {
			// Make list of selected vertices
			ICollection<Vertex> selected = General.Map.Map.GetSelectedVertices(true);
			if(selected.Count == 0) {
				if(highlighted != null && !highlighted.IsDisposed)
					selected.Add(highlighted);
				else
					return;
			}

			// Make undo
			if(selected.Count > 1) {
				General.Map.UndoRedo.CreateUndo("Delete " + selected.Count + " vertices");
				General.Interface.DisplayStatus(StatusType.Action, "Deleted " + selected.Count + " vertices.");
			} else {
				General.Map.UndoRedo.CreateUndo("Delete vertex");
				General.Interface.DisplayStatus(StatusType.Action, "Deleted a vertex.");
			}

			//collect linedefs count per vertex
			Dictionary<Vertex, int> linesPerVertex = new Dictionary<Vertex, int>();
			List<Sector> affectedSectors = new List<Sector>();
			foreach(Vertex v in selected) {
				linesPerVertex.Add(v, v.Linedefs.Count);

				foreach(Linedef l in v.Linedefs) {
					if(l.Front != null && l.Front.Sector != null && !affectedSectors.Contains(l.Front.Sector))
						affectedSectors.Add(l.Front.Sector);
					if(l.Back != null && l.Back.Sector != null && !affectedSectors.Contains(l.Back.Sector))
						affectedSectors.Add(l.Back.Sector);
				}
			}

			List<Linedef> changedLines = new List<Linedef>();

			// Go for all vertices that need to be removed
			foreach(Vertex v in selected) {
				// Not already removed automatically?
				if(!v.IsDisposed) {
					// If the vertex only had 2 linedefs attached, then merge the linedefs
					if(linesPerVertex[v] == 2) {
						Linedef ld1 = General.GetByIndex(v.Linedefs, 0);
						Linedef ld2 = General.GetByIndex(v.Linedefs, 1);
						Vertex v1 = (ld1.Start == v) ? ld1.End : ld1.Start;
						Vertex v2 = (ld2.Start == v) ? ld2.End : ld2.Start;

						//don't merge if it will collapse 3-sided sector
						bool dontMerge = false;
						foreach(Linedef l in v1.Linedefs) {
							if(l == ld2) continue;
							if(l.Start == v2 || l.End == v2) {
								tryJoinSectors(l);
								dontMerge = true;
								break;
							}
						}

						if(!dontMerge) {
							if(ld1.Start == v) ld1.SetStartVertex(v2);
							else ld1.SetEndVertex(v2);
							ld2.Dispose();

							if(!changedLines.Contains(ld1)) changedLines.Add(ld1);
						}
					}

					// Trash vertex
					v.Dispose();
				}
			}

			// Update cache values
			General.Map.Map.Update();
			General.Map.IsChanged = true;

			//redraw changed lines
			foreach(Linedef l in changedLines) {
				if(l.IsDisposed) continue;
				drawLine(l.Start.Position, l.End.Position);

				if(l.IsDisposed) continue;
				drawLine(l.Start.Position, l.End.Position);
			}

			//redraw all affected sectors
			List<Linedef> redrawnLines = new List<Linedef>();
			foreach(Sector sector in affectedSectors) {
				if(sector.IsDisposed) continue;

				List<Linedef> lines = new List<Linedef>();
				foreach(Sidedef side in sector.Sidedefs) {
					if(!lines.Contains(side.Line)) 
						lines.Add(side.Line);
				}

				if(sector.Triangles.Vertices.Count == 0) {
					sector.Dispose();
				}

				foreach(Linedef line in lines) {
					if(line.IsDisposed || redrawnLines.Contains(line)) continue;

					if(line.Front == null && line.Back != null) {
						//Flip linedef
						line.FlipVertices();
						line.FlipSidedefs();
						line.SetFlag(General.Map.Config.ImpassableFlag, true);
					} else if(line.Front != null && line.Back == null) {
						line.SetFlag(General.Map.Config.ImpassableFlag, true);
					}

					line.ApplySidedFlags();

					drawLine(line.Start.Position, line.End.Position);
					redrawnLines.Add(line);
				}
			}

			// Update cache values
			General.Map.Map.Update();
			General.Map.IsChanged = true;

			// Invoke a new mousemove so that the highlighted item updates
			MouseEventArgs e = new MouseEventArgs(MouseButtons.None, 0, (int)mousepos.x, (int)mousepos.y, 0);
			OnMouseMove(e);

			// Redraw screen
			General.Interface.RedrawDisplay();
		}

		private void drawLine(Vector2D start, Vector2D end) {
			DrawnVertex dv1 = new DrawnVertex();
			DrawnVertex dv2 = new DrawnVertex();
			dv1.stitchline = true;
			dv2.stitchline = true;
			dv1.stitch = true;
			dv2.stitch = true;
			dv1.pos = start;
			dv2.pos = end;
			Tools.DrawLines(new List<DrawnVertex>() { dv1, dv2 });

			// Update cache values
			General.Map.Map.Update();
			General.Map.IsChanged = true;
		}

		//mxd. If there are different sectors on both sides of given linedef, join them
		private void tryJoinSectors(Linedef ld) {
			if(ld.IsDisposed) return;

			if(ld.Front != null && ld.Front.Sector != null && ld.Back != null && ld.Back.Sector != null && ld.Front.Sector.Index != ld.Back.Sector.Index) {
				if(ld.Front.Sector.BBox.Width * ld.Front.Sector.BBox.Height > ld.Back.Sector.BBox.Width * ld.Back.Sector.BBox.Height)
					ld.Back.Sector.Join(ld.Front.Sector);
				else
					ld.Front.Sector.Join(ld.Back.Sector);
			}
		}
		
		#endregion
	}
}
