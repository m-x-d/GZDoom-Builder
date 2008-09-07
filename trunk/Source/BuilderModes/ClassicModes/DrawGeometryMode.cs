
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
using System.Drawing;
using CodeImp.DoomBuilder.Editing;
using CodeImp.DoomBuilder.Actions;

#endregion

namespace CodeImp.DoomBuilder.BuilderModes
{
	[EditMode(DisplayName = "Drawing",
			  SwitchAction = "drawlinesmode",
			  Volatile = true)]

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
		private List<LineLengthLabel> labels;

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

		internal EditMode BaseMode { get { return basemode; } }

		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		public DrawGeometryMode()
		{
			// Initialize
			this.basemode = General.Map.Mode;
			points = new List<DrawnVertex>();
			labels = new List<LineLengthLabel>();
			
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
				if(labels != null)
					foreach(LineLengthLabel l in labels) l.Dispose();
				
				// Done
				base.Dispose();
			}
		}

		#endregion

		#region ================== Methods

		// Engaging
		public override void OnEngage()
		{
			base.OnEngage();
			renderer.SetPresentation(Presentation.Standard);
			
			// Set cursor
			General.Interface.SetCursor(Cursors.Cross);
		}
		
		// Cancelled
		public override void OnCancel()
		{
			// Cancel base class
			base.OnCancel();
			
			// Return to original mode
			Type t = basemode.GetType();
			basemode = (EditMode)Activator.CreateInstance(t);
			General.Map.ChangeMode(basemode);
		}

		// Accepted
		public override void OnAccept()
		{
			List<Vertex> newverts = new List<Vertex>();
			List<Vertex> intersectverts = new List<Vertex>();
			List<Linedef> newlines = new List<Linedef>();
			List<Linedef> oldlines = new List<Linedef>(General.Map.Map.Linedefs);
			List<Sidedef> insidesides = new List<Sidedef>();
			List<Vertex> mergeverts = new List<Vertex>();
			List<Vertex> nonmergeverts = new List<Vertex>(General.Map.Map.Vertices);
			MapSet map = General.Map.Map;
			
			Cursor.Current = Cursors.AppStarting;

			General.Settings.FindDefaultDrawSettings();

			// When points have been drawn
			if(points.Count > 0)
			{
				// Make undo for the draw
				General.Map.UndoRedo.CreateUndo("Line draw", UndoGroup.None, 0);

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
					ld.ApplySidedFlags();
					ld.UpdateCache();
					newlines.Add(ld);

					// Should we split this line to merge with intersecting lines?
					if(points[i - 1].stitch && points[i].stitch)
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
							splitline.ApplySidedFlags();
							newlines.Add(splitline);
						}
					}

					// Next
					v1 = v2;
				}

				// Join merge vertices so that overlapping vertices in the draw become one.
				MapSet.JoinVertices(mergeverts, mergeverts, false, MapSet.STITCH_DISTANCE);

				// We prefer a closed polygon, because then we can determine the interior properly
				// Check if the two ends of the polygon are closed
				bool drawingclosed = false;
				if(newlines.Count > 0)
				{
					// When not closed, we will try to find a path to close it
					Linedef firstline = newlines[0];
					Linedef lastline = newlines[newlines.Count - 1];
					drawingclosed = (firstline.Start == lastline.End);
					if(!drawingclosed)
					{
						// First and last vertex stitch with geometry?
						if(points[0].stitch && points[points.Count - 1].stitch)
						{
							// Find out where they will stitch
							Linedef l1 = MapSet.NearestLinedefRange(oldlines, firstline.Start.Position, MapSet.STITCH_DISTANCE);
							Linedef l2 = MapSet.NearestLinedefRange(oldlines, lastline.End.Position, MapSet.STITCH_DISTANCE);
							if((l1 != null) && (l2 != null))
							{
								List<LinedefSide> shortestpath = null;

								// Same line?
								if(l1 == l2)
								{
									// Then just connect the two
									shortestpath = new List<LinedefSide>();
									shortestpath.Add(new LinedefSide(l1, true));
								}
								else
								{
									// Find the shortest, closest path between these lines
									List<List<LinedefSide>> paths = new List<List<LinedefSide>>(8);
									paths.Add(SectorTools.FindClosestPath(l1, true, l2, true, true));
									paths.Add(SectorTools.FindClosestPath(l1, true, l2, false, true));
									paths.Add(SectorTools.FindClosestPath(l1, false, l2, true, true));
									paths.Add(SectorTools.FindClosestPath(l1, false, l2, false, true));
									paths.Add(SectorTools.FindClosestPath(l2, true, l1, true, true));
									paths.Add(SectorTools.FindClosestPath(l2, true, l1, false, true));
									paths.Add(SectorTools.FindClosestPath(l2, false, l1, true, true));
									paths.Add(SectorTools.FindClosestPath(l2, false, l1, false, true));

									foreach(List<LinedefSide> p in paths)
										if((p != null) && ((shortestpath == null) || (p.Count < shortestpath.Count))) shortestpath = p;
								}

								// Found a path?
								if(shortestpath != null)
								{
									// Check which direction the path goes in
									if(shortestpath[0].Line == l1)
									{
										// Begin at start
										v1 = firstline.Start;
									}
									else
									{
										// Begin at end
										v1 = lastline.End;
									}
									
									// Go for all vertices in the path to make additional lines
									for(int i = 1; i < shortestpath.Count; i++)
									{
										// Get the next position
										Vector2D v2pos = shortestpath[i].Front ? shortestpath[i].Line.Start.Position : shortestpath[i].Line.End.Position;

										// Make the new vertex
										Vertex v2 = map.CreateVertex(v2pos);
										v2.Marked = true;
										mergeverts.Add(v2);

										// Make the line
										Linedef ld = map.CreateLinedef(v1, v2);
										ld.Marked = true;
										ld.Selected = true;
										ld.ApplySidedFlags();
										ld.UpdateCache();
										newlines.Add(ld);

										// Next
										v1 = v2;
									}

									// Make the final line
									Linedef lld;
									
									// Check which direction the path goes in
									if(shortestpath[0].Line == l1)
									{
										// Path stops at end
										lld = map.CreateLinedef(v1, lastline.End);
									}
									else
									{
										// Path stops at begin
										lld = map.CreateLinedef(v1, firstline.Start);
									}

									// Setup line
									lld.Marked = true;
									lld.Selected = true;
									lld.ApplySidedFlags();
									lld.UpdateCache();
									newlines.Add(lld);

									// Drawing is now closed
									drawingclosed = true;

									// Join merge vertices so that overlapping vertices in the draw become one.
									MapSet.JoinVertices(mergeverts, mergeverts, false, MapSet.STITCH_DISTANCE);
								}
							}
						}
					}
				}

				// Merge intersetion vertices with the new lines. This completes the
				// self intersections for which splits were made above.
				map.Update(true, false);
				MapSet.SplitLinesByVertices(newlines, intersectverts, MapSet.STITCH_DISTANCE, null);
				MapSet.SplitLinesByVertices(newlines, mergeverts, MapSet.STITCH_DISTANCE, null);

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
					List<LinedefSide> pathlines = SectorTools.FindClosestPath(ld, true, true);
					if(pathlines != null)
					{
						// Make polygon
						LinedefTracePath tracepath = new LinedefTracePath(pathlines);
						EarClipPolygon pathpoly = tracepath.MakePolygon();

						// Check if the front of the line is outside the polygon
						if(!pathpoly.Intersect(ld.GetSidePoint(true)))
						{
							// Now trace from the back side of the line to see if
							// the back side lies in the interior. I don't want to
							// flip the line if it is not helping.

							// Find closest path starting with the back of this linedef
							pathlines = SectorTools.FindClosestPath(ld, false, true);
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
									ld.UpdateCache();
								}
							}
						}
					}
				}

				// Mark only the vertices that should be merged
				map.ClearMarkedVertices(false);
				foreach(Vertex v in mergeverts) v.Marked = true;

				// Before this point, the new geometry is not linked with the existing geometry.
				// Now perform standard geometry stitching to merge the new geometry with the rest
				// of the map. The marked vertices indicate the new geometry.
				map.StitchGeometry();
				map.Update(true, false);

				// Find our new lines again, because they have been merged with the other geometry
				// but their Marked property is copied where they have joined.
				newlines = map.GetMarkedLinedefs(true);

				/***************************************************\
					STEP 3: Join and create new sectors
				\***************************************************/

				// The code below atempts to create sectors on the front sides of the drawn
				// geometry and joins sectors on the back sides of the drawn geometry.
				// This code does not change any geometry, it only makes/updates sidedefs.
				bool sidescreated = false;
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
							sidescreated = true;
							
							// Make the new sector
							Sector newsector = SectorTools.MakeSector(sectorlines);

							// Go for all sidedefs in this new sector
							foreach(Sidedef sd in newsector.Sidedefs)
							{
								// Keep list of sides inside created sectors
								insidesides.Add(sd);

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
								sidescreated = true;

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

				// Remove all unneeded textures
				// Shouldn't this already be done by the
				// makesector/joinsector functions?
				foreach(Linedef ld in newlines)
				{
					if(ld.Front != null) ld.Front.RemoveUnneededTextures(true);
					if(ld.Back != null) ld.Back.RemoveUnneededTextures(true);
				}
				foreach(Sidedef sd in insidesides)
				{
					sd.RemoveUnneededTextures(true);
				}

				// Check if any of our new lines have sides
				if(sidescreated)
				{
					// Then remove the lines which have no sides at all
					for(int i = newlines.Count - 1; i >= 0; i--)
					{
						// Remove the line if it has no sides
						if((newlines[i].Front == null) && (newlines[i].Back == null)) newlines[i].Dispose();
					}
				}
				
				// Snap to map format accuracy
				General.Map.Map.SnapAllToAccuracy();

				// Update cached values
				map.Update();

				// Map is changed
				General.Map.IsChanged = true;
			}

			// Done
			Cursor.Current = Cursors.Default;
			
			// Return to original mode
			Type t = basemode.GetType();
			basemode = (EditMode)Activator.CreateInstance(t);
			General.Map.ChangeMode(basemode);
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
		public override void OnRedrawDisplay()
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
				renderer.RenderThingSet(General.Map.Map.Things, 1.0f);
				renderer.Finish();
			}

			// Normal update
			Update();
		}
		
		// This updates the dragging
		private void Update()
		{
			PixelColor stitchcolor = General.Colors.Highlight;
			PixelColor losecolor = General.Colors.Selection;
			PixelColor color;
			
			snaptogrid = General.Interface.ShiftState ^ General.Interface.SnapToGrid;
			snaptonearest = General.Interface.CtrlState ^ General.Interface.AutoMerge;

			DrawnVertex lastp = new DrawnVertex();
			DrawnVertex curp = GetCurrentPosition();
			float vsize = ((float)renderer.VertexSize + 1.0f) / renderer.Scale;
			float vsizeborder = ((float)renderer.VertexSize + 3.0f) / renderer.Scale;
			
			// The last label's end must go to the mouse cursor
			if(labels.Count > 0) labels[labels.Count - 1].End = curp.pos;
			
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
						if(lastp.stitch && points[i].stitch) color = stitchcolor;
							else color = losecolor;
						
						// Render line
						renderer.RenderLine(lastp.pos, points[i].pos, LINE_THICKNESS, color, true);
						lastp = points[i];
					}

					// Determine line color
					if(lastp.stitch && snaptonearest) color = stitchcolor;
						else color = losecolor;

					// Render line to cursor
					renderer.RenderLine(lastp.pos, curp.pos, LINE_THICKNESS, color, true);
					
					// Render vertices
					for(int i = 0; i < points.Count; i++)
					{
						// Determine line color
						if(points[i].stitch) color = stitchcolor;
							else color = losecolor;
						
						// Render line
						renderer.RenderRectangleFilled(new RectangleF(points[i].pos.x - vsize, points[i].pos.y - vsize, vsize * 2.0f, vsize * 2.0f), color, true);
					}
				}
				
				// Determine point color
				if(snaptonearest) color = stitchcolor;
					else color = losecolor;
				
				// Render vertex at cursor
				renderer.RenderRectangleFilled(new RectangleF(curp.pos.x - vsize, curp.pos.y - vsize, vsize * 2.0f, vsize * 2.0f), color, true);
				
				// Go for all labels
				foreach(LineLengthLabel l in labels) renderer.RenderText(l.TextLabel);
				
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
		public override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);
			Update();
		}

		// This draws a point at a specific location
		public void DrawPointAt(Vector2D pos, bool stitch)
		{
			DrawnVertex newpoint = new DrawnVertex();
			newpoint.pos = pos;
			newpoint.stitch = stitch;
			points.Add(newpoint);
			labels.Add(new LineLengthLabel());
			labels[labels.Count - 1].Start = newpoint.pos;
			if(labels.Count > 1) labels[labels.Count - 2].End = newpoint.pos;
			Update();

			// Check if point stitches with the first
			if((points.Count > 1) && (points[points.Count - 1].stitch))
			{
				Vector2D p1 = points[0].pos;
				Vector2D p2 = points[points.Count - 1].pos;
				Vector2D delta = p1 - p2;
				if((Math.Abs(delta.x) <= 0.001f) && (Math.Abs(delta.y) <= 0.001f))
				{
					// Finish drawing
					FinishDraw();
				}
			}
		}
		
		// Drawing a point
		[BeginAction("drawpoint")]
		public void DrawPoint()
		{
			// Mouse inside window?
			if(General.Interface.MouseInDisplay)
			{
				DrawnVertex newpoint = GetCurrentPosition();
				DrawPointAt(newpoint.pos, newpoint.stitch);
			}
		}

		// Remove a point
		[BeginAction("removepoint")]
		public void RemovePoint()
		{
			if(points.Count > 0) points.RemoveAt(points.Count - 1);
			if(labels.Count > 0)
			{
				labels[labels.Count - 1].Dispose();
				labels.RemoveAt(labels.Count - 1);
			}
			
			Update();
		}

		// Finish drawing
		[BeginAction("finishdraw")]
		public void FinishDraw()
		{
			// Accept the changes
			General.Map.AcceptMode();
		}

		// When a key is released
		public override void OnKeyUp(KeyEventArgs e)
		{
			base.OnKeyUp(e);
			if((snaptogrid != (General.Interface.ShiftState ^ General.Interface.SnapToGrid)) ||
			   (snaptonearest != (General.Interface.CtrlState ^ General.Interface.AutoMerge))) Update();
		}

		// When a key is pressed
		public override void OnKeyDown(KeyEventArgs e)
		{
			base.OnKeyDown(e);
			if((snaptogrid != (General.Interface.ShiftState ^ General.Interface.SnapToGrid)) ||
			   (snaptonearest != (General.Interface.CtrlState ^ General.Interface.AutoMerge))) Update();
		}
		
		#endregion
	}
}
