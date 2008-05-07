
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
using CodeImp.DoomBuilder.Interface;
using CodeImp.DoomBuilder.IO;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Rendering;
using CodeImp.DoomBuilder.Geometry;
using System.Drawing;
using CodeImp.DoomBuilder.Editing;
using CodeImp.DoomBuilder.Controls;

#endregion

namespace CodeImp.DoomBuilder.BuilderModes.Editing
{
	[EditMode(SwitchAction = "drawlinesmode")]

	public class DrawGeometryMode : ClassicMode
	{
		#region ================== Structures

		private struct DrawnVertex
		{
			public Vector2D pos;
			public bool stitch;
		}

		#endregion
		
		#region ================== Constants

		private const float LINE_THICKNESS = 0.8f;

		#endregion

		#region ================== Variables

		// Mode to return to
		private EditMode basemode;

		// Drawing points
		private List<DrawnVertex> points;

		// Keep track of view changes
		private float lastoffsetx;
		private float lastoffsety;
		private float lastscale;

		// Options
		private bool snaptogrid;		// SHIFT to toggle
		private bool snaptonearest;		// CTRL to enable
		
		#endregion

		#region ================== Properties

		// Just keep the base mode button checked
		public override string EditModeButtonName { get { return basemode.GetType().Name; } }

		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		public DrawGeometryMode()
		{
			// Initialize
			this.basemode = General.Map.Mode;
			points = new List<DrawnVertex>();
			
			// No selection in this mode
			General.Map.Map.ClearAllSelected();
			General.Map.Map.ClearAllMarks();
			
			// We have no destructor
			GC.SuppressFinalize(this);
		}

		// Disposer
		public override void Dispose()
		{
			// Not already disposed?
			if(!isdisposed)
			{
				// Clean up

				// Done
				base.Dispose();
			}
		}

		#endregion

		#region ================== Methods

		// Cancelled
		public override void Cancel()
		{
			// Cancel base class
			base.Cancel();
			
			// Return to original mode
			Type t = basemode.GetType();
			basemode = (EditMode)Activator.CreateInstance(t);
			General.Map.ChangeMode(basemode);
		}

		// Disenagaging
		public override void Disengage()
		{
			List<Vertex> newverts = new List<Vertex>();
			List<Vertex> intersectverts = new List<Vertex>();
			List<Linedef> newlines = new List<Linedef>();
			List<Vertex> mergeverts = new List<Vertex>();
			List<Vertex> nonmergeverts = new List<Vertex>(General.Map.Map.Vertices);
			
			MapSet map = General.Map.Map;

			base.Disengage();

			Cursor.Current = Cursors.AppStarting;

			// When not cancelled and points have been drawn
			if(!cancelled && (points.Count > 0))
			{
				// Make undo for the draw
				General.Map.UndoRedo.CreateUndo("line draw", UndoGroup.None, 0);
				
				/***************************************************\
					STEP 1: Create the new geometry
				\***************************************************/

				// Make first vertex
				Vertex v1 = map.CreateVertex(points[0].pos);
				v1.Marked = true;
				
				// Keep references
				newverts.Add(v1);
				if(points[0].stitch) mergeverts.Add(v1); else nonmergeverts.Add(v1);

				// Go for all other points
				for(int i = 1; i < points.Count; i++)
				{
					// Create vertex for point
					Vertex v2 = map.CreateVertex(points[i].pos);
					v2.Marked = true;
					
					// Keep references
					newverts.Add(v2);
					if(points[i].stitch) mergeverts.Add(v2); else nonmergeverts.Add(v2);

					// Create line between point and previous
					Linedef ld = map.CreateLinedef(v1, v2);
					ld.Marked = true;
					ld.Selected = true;
					newlines.Add(ld);
					ld.UpdateCache();
					
					// Should we split this line to merge with intersecting lines?
					if(points[i - 1].stitch || points[i].stitch)
					{
						// Check if any other lines intersect this line
						List<float> intersections = new List<float>();
						Line2D measureline = ld.Line;
						foreach(Linedef ld2 in map.Linedefs)
						{
							// Intersecting?
							// We only keep the unit length from the start of the line and
							// do the real splitting later, when all intersections are known
							float u;
							if(ld2.Line.GetIntersection(measureline, out u))
							{
								if(!float.IsNaN(u) && (u > 0.0f) && (u < 1.0f) && (ld2 != ld))
									intersections.Add(u);
							}
						}

						// Sort the intersections
						intersections.Sort();

						// Go for all found intersections
						Linedef splitline = ld;
						foreach(float u in intersections)
						{
							// Calculate exact coordinates where to split
							// We use measureline for this, because the original line
							// may already have changed in length due to a previous split
							Vector2D splitpoint = measureline.GetCoordinatesAt(u);

							// Make the vertex
							Vertex splitvertex = map.CreateVertex(splitpoint);
							splitvertex.Marked = true;
							newverts.Add(splitvertex);
							mergeverts.Add(splitvertex);			// <-- add to merge?
							intersectverts.Add(splitvertex);
							
							// The Split method ties the end of the original line to the given
							// vertex and starts a new line at the given vertex, so continue
							// splitting with the new line, because the intersections are sorted
							// from low to high (beginning at the original line start)
							splitline = splitline.Split(splitvertex);
							newlines.Add(splitline);
						}
					}
					
					// Next
					v1 = v2;
				}

				// Join merge vertices so that overlapping vertices in the draw become one.
				MapSet.JoinVertices(mergeverts, mergeverts, false, MapSet.STITCH_DISTANCE);
				
				// Merge intersetion vertices with the new lines. This completes the
				// self intersections for which splits were made above.
				map.Update(true, false);
				MapSet.SplitLinesByVertices(newlines, intersectverts, MapSet.STITCH_DISTANCE, null);

				/***************************************************\
					STEP 2: Merge the new geometry
				\***************************************************/

				// In step 3 we will make sectors on the front sides and join sectors on the
				// back sides, but because the user could have drawn counterclockwise or just
				// some weird polygon this could result in problems. The following code adjusts
				// the direction of all new lines so that their front (right) side is facing
				// the interior of the new drawn polygon.
				map.Update(true, false);
				foreach(Linedef ld in newlines)
				{
					// Find closest path starting with the front of this linedef
					List<LinedefSide> pathlines = SectorTools.FindClosestPath(ld, true);
					if(pathlines != null)
					{
						// Make polygon
						LinedefTracePath tracepath = new LinedefTracePath(pathlines);
						Polygon pathpoly = tracepath.MakePolygon();

						// Check if the front of the line is outside the polygon
						if(!pathpoly.Intersect(ld.GetSidePoint(true)))
						{
							// Now trace from the back side of the line to see if
							// the back side lies in the interior. I don't want to
							// flip the line if it is not helping.

							/*
							// Find closest path starting with the back of this linedef
							pathlines = SectorTools.FindClosestPath(ld, false);
							if(pathlines != null)
							{
								// Make polygon
								tracepath = new LinedefTracePath(pathlines);
								pathpoly = tracepath.MakePolygon();

								// Check if the back of the line is inside the polygon
								if(pathpoly.Intersect(ld.GetSidePoint(false)))
								{
									// We must flip this linedef to face the interior
									ld.FlipVertices();
									ld.FlipSidedefs();
								}
							}
							*/
							
							// We must flip this linedef to face the interior
							ld.FlipVertices();
							ld.FlipSidedefs();
							ld.UpdateCache();
						}
					}
				}

				// Before this point, the new geometry is not linked with the existing geometry.
				// Now perform standard geometry stitching to merge the new geometry with the rest
				// of the map. The marked vertices indicate the new geometry.
				map.StitchGeometry();
				map.Update(true, false);
				
				// Find our new lines again, because they have been merged with the other geometry
				// but their Marked property is copied where they have joined!
				newlines = map.GetMarkedLinedefs(true);
				
				/***************************************************\
					STEP 3: Join and create new sectors
				\***************************************************/

				bool[] frontsdone = new bool[newlines.Count];
				bool[] backsdone = new bool[newlines.Count];
				for(int i = 0; i < newlines.Count; i++)
				{
					Linedef ld = newlines[i];
					
					// Front not marked as done?
					if(!frontsdone[i])
					{
						// Find a way to create a sector here
						List<LinedefSide> sectorlines = SectorTools.FindPotentialSectorAt(ld, true);
						if(sectorlines != null)
						{
							// Make the new sector
							Sector newsector = SectorTools.MakeSector(sectorlines);
							
							// Go for all sidedefs in this new sector
							foreach(Sidedef sd in newsector.Sidedefs)
							{
								// Side matches with a side of our new lines?
								int lineindex = newlines.IndexOf(sd.Line);
								if(lineindex > -1)
								{
									// Mark this side as done
									if(sd.IsFront)
										frontsdone[lineindex] = true;
									else
										backsdone[lineindex] = true;
								}
							}
						}
					}

					// Back not marked as done?
					if(!backsdone[i])
					{
						// Find a way to create a sector here
						List<LinedefSide> sectorlines = SectorTools.FindPotentialSectorAt(ld, false);
						if(sectorlines != null)
						{
							// We don't always want to create a new sector on the back sides
							// So first check if any of the surrounding lines originally have sidedefs
							Sidedef joinsidedef = null;
							foreach(LinedefSide ls in sectorlines)
							{
								if(ls.Front && (ls.Line.Front != null))
								{
									joinsidedef = ls.Line.Front;
									break;
								}
								else if(!ls.Front && (ls.Line.Back != null))
								{
									joinsidedef = ls.Line.Back;
									break;
								}
							}
							
							// Join?
							if(joinsidedef != null)
							{
								// Join the new sector
								Sector newsector = SectorTools.JoinSector(sectorlines, joinsidedef);

								// Go for all sidedefs in this new sector
								foreach(Sidedef sd in newsector.Sidedefs)
								{
									// Side matches with a side of our new lines?
									int lineindex = newlines.IndexOf(sd.Line);
									if(lineindex > -1)
									{
										// Mark this side as done
										if(sd.IsFront)
											frontsdone[lineindex] = true;
										else
											backsdone[lineindex] = true;
									}
								}
							}
						}
					}
				}

				// Make corrections for backward linedefs
				MapSet.FlipBackwardLinedefs(newlines);

				// Snap to map format accuracy
				General.Map.Map.SnapAllToAccuracy();
				
				// Update cached values
				map.Update();

				// Map is changed
				General.Map.IsChanged = true;
			}

			// Hide highlight info
			General.Interface.HideInfo();

			// Done
			Cursor.Current = Cursors.Default;
		}

		// This checks if the view offset/zoom changed and updates the check
		protected bool CheckViewChanged()
		{
			bool viewchanged = false;
			
			// View changed?
			if(renderer.OffsetX != lastoffsetx) viewchanged = true;
			if(renderer.OffsetY != lastoffsety) viewchanged = true;
			if(renderer.Scale != lastscale) viewchanged = true;

			// Keep view information
			lastoffsetx = renderer.OffsetX;
			lastoffsety = renderer.OffsetY;
			lastscale = renderer.Scale;

			// Return result
			return viewchanged;
		}

		// This redraws the display
		public override void RedrawDisplay()
		{
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
				renderer.SetThingsRenderOrder(false);
				renderer.RenderThingSet(General.Map.Map.Things);
				renderer.Finish();
			}

			// Normal update
			Update();
		}
		
		// This updates the dragging
		private void Update()
		{
			PixelColor color;
			
			snaptogrid = General.Interface.ShiftState ^ General.Interface.SnapToGrid;
			snaptonearest = General.Interface.CtrlState ^ General.Interface.AutoMerge;

			DrawnVertex lastp = new DrawnVertex();
			DrawnVertex curp = GetCurrentPosition();
			float vsize = ((float)renderer.VertexSize + 1.0f) / renderer.Scale;
			
			// Render drawing lines
			if(renderer.StartOverlay(true))
			{
				// Go for all points to draw lines
				if(points.Count > 0)
				{
					// Render lines
					lastp = points[0];
					for(int i = 1; i < points.Count; i++)
					{
						// Determine line color
						if(lastp.stitch || points[i].stitch)
							color = General.Colors.Highlight;
						else
							color = General.Colors.Selection;
						
						// Render line
						renderer.RenderLine(lastp.pos, points[i].pos, LINE_THICKNESS, color, true);
						lastp = points[i];
					}

					// Determine line color
					if(lastp.stitch || snaptonearest)
						color = General.Colors.Highlight;
					else
						color = General.Colors.Selection;

					// Render line to cursor
					renderer.RenderLine(lastp.pos, curp.pos, LINE_THICKNESS, color, true);
					
					// Render vertices
					for(int i = 0; i < points.Count; i++)
					{
						if(points[i].stitch)
						{
							renderer.RenderRectangleFilled(new RectangleF(points[i].pos.x - vsize, points[i].pos.y - vsize, vsize * 2.0f, vsize * 2.0f), General.Colors.Highlight, true);
						}
						else
						{
							renderer.RenderRectangleFilled(new RectangleF(points[i].pos.x - vsize, points[i].pos.y - vsize, vsize * 2.0f, vsize * 2.0f), General.Colors.Selection, true);
						}
					}
				}

				// Determine point color
				if(snaptonearest) color = General.Colors.Highlight;
				else color = General.Colors.Selection;
				
				// Render vertex at cursor
				renderer.RenderRectangleFilled(new RectangleF(curp.pos.x - vsize, curp.pos.y - vsize, vsize * 2.0f, vsize * 2.0f), color, true);
				
				// Done
				renderer.Finish();
			}

			// Done
			renderer.Present();
		}
		
		// This gets the aligned and snapped draw position
		private DrawnVertex GetCurrentPosition()
		{
			DrawnVertex p = new DrawnVertex();
			
			// Snap to nearest?
			if(snaptonearest)
			{
				float vrange = VerticesMode.VERTEX_HIGHLIGHT_RANGE / renderer.Scale;
				
				// Go for all drawn points
				foreach(DrawnVertex v in points)
				{
					Vector2D delta = mousemappos - v.pos;
					if(delta.GetLengthSq() < (vrange * vrange))
					{
						p.pos = v.pos;
						p.stitch = true;
						return p;
					}
				}
				
				// Try the nearest vertex
				Vertex nv = General.Map.Map.NearestVertexSquareRange(mousemappos, vrange);
				if(nv != null)
				{
					p.pos = nv.Position;
					p.stitch = true;
					return p;
				}
				
				// Try the nearest linedef
				Linedef nl = General.Map.Map.NearestLinedefRange(mousemappos, LinedefsMode.LINEDEF_HIGHLIGHT_RANGE / renderer.Scale);
				if(nl != null)
				{
					// Snap to grid?
					if(snaptogrid)
					{
						// Get grid intersection coordinates
						List<Vector2D> coords = nl.GetGridIntersections();

						// Find nearest grid intersection
						float found_distance = float.MaxValue;
						Vector2D found_coord = new Vector2D();
						foreach(Vector2D v in coords)
						{
							Vector2D delta = mousemappos - v;
							if(delta.GetLengthSq() < found_distance)
							{
								found_distance = delta.GetLengthSq();
								found_coord = v;
							}
						}
						
						// Align to the closest grid intersection
						p.pos = found_coord;
						p.stitch = true;
						return p;
					}
					else
					{
						// Aligned to line
						p.pos = nl.NearestOnLine(mousemappos);
						p.stitch = true;
						return p;
					}
				}
			}

			// Snap to grid?
			if(snaptogrid)
			{
				// Aligned to grid
				p.pos = General.Map.Grid.SnappedToGrid(mousemappos);
				p.stitch = snaptonearest;
				return p;
			}
			else
			{
				// Normal position
				p.pos = mousemappos;
				p.stitch = snaptonearest;
				return p;
			}
		}
		
		// Mouse moving
		public override void MouseMove(MouseEventArgs e)
		{
			base.MouseMove(e);
			Update();
		}

		// Drawing a point
		[BeginAction("drawpoint")]
		public void DrawPoint()
		{
			// Mouse inside window?
			if(General.Interface.MouseInDisplay)
			{
				points.Add(GetCurrentPosition());
				Update();
			}
		}

		// Remove a point
		[BeginAction("removepoint")]
		public void RemovePoint()
		{
			if(points.Count > 0) points.RemoveAt(points.Count - 1);
			Update();
		}

		// Finish drawing
		[BeginAction("finishdraw")]
		public void FinishDraw()
		{
			// Just return to base mode, Disengage will be called automatically.
			General.Map.ChangeMode(basemode);
		}

		// When a key is released
		public override void KeyUp(KeyEventArgs e)
		{
			base.KeyUp(e);
			if((snaptogrid != (General.Interface.ShiftState ^ General.Interface.SnapToGrid)) ||
			   (snaptonearest != (General.Interface.CtrlState ^ General.Interface.AutoMerge))) Update();
		}

		// When a key is pressed
		public override void KeyDown(KeyEventArgs e)
		{
			base.KeyDown(e);
			if((snaptogrid != (General.Interface.ShiftState ^ General.Interface.SnapToGrid)) ||
			   (snaptonearest != (General.Interface.CtrlState ^ General.Interface.AutoMerge))) Update();
		}
		
		#endregion
	}
}
