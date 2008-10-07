
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
using CodeImp.DoomBuilder.Geometry;
using CodeImp.DoomBuilder.Rendering;
using SlimDX.Direct3D9;
using System.Drawing;
using CodeImp.DoomBuilder.Map;
using System.Collections.ObjectModel;

#endregion

namespace CodeImp.DoomBuilder.Geometry
{
	/// <summary>
	/// Responsible for creating sector polygons.
	/// Performs triangulation of sectors by using ear clipping.
	/// </summary>
	public sealed class Triangulation
	{
		#region ================== Delegates

		// For debugging purpose only!
		// These are not called in a release build
		public delegate void ShowLine(Vector2D v1, Vector2D v2, PixelColor c);
		public delegate void ShowPolygon(EarClipPolygon p, PixelColor c);
		public delegate void ShowPoint(Vector2D v, int c);
		public delegate void ShowEarClip(EarClipVertex[] found, LinkedList<EarClipVertex> remaining);
		
		// For debugging purpose only!
		// These are not called in a release build
		public ShowLine OnShowLine;
		public ShowPolygon OnShowPolygon;
		public ShowPoint OnShowPoint;
		public ShowEarClip OnShowEarClip;
		
		#endregion

		#region ================== Constants

		#endregion

		#region ================== Variables

		// Number of vertices per island
		private ReadOnlyCollection<int> islandvertices;
		
		// Vertices that result from the triangulation, 3 per triangle.
		private ReadOnlyCollection<Vector2D> vertices;

		// These sidedefs match with the vertices. If a vertex is not the start
		// along a sidedef, this list contains a null entry for that vertex.
		private ReadOnlyCollection<Sidedef> sidedefs;
		
		#endregion

		#region ================== Properties
		
		public ReadOnlyCollection<int> IslandVertices { get { return islandvertices; } }
		public ReadOnlyCollection<Vector2D> Vertices { get { return vertices; } }
		public ReadOnlyCollection<Sidedef> Sidedefs { get { return sidedefs; } }
		
		#endregion

		#region ================== Constructor / Disposer

		// I don't like using constructors that do more than simple initialization work
		public static Triangulation Create(Sector sector)
		{
			return new Triangulation(sector);
		}

		// Constructor
		private Triangulation(Sector s)
		{
			// Initialize
			TriangleList triangles = new TriangleList();
			List<EarClipPolygon> polys;
			List<int> islandslist = new List<int>();
			List<Vector2D> verticeslist = new List<Vector2D>();
			List<Sidedef> sidedefslist = new List<Sidedef>();

			// We have no destructor
			GC.SuppressFinalize(this);
			
			/*
			 * This process is divided into several steps:
			 * 
			 * 1) Tracing the sector lines to find clockwise outer polygons
			 * and counter-clockwise inner polygons. These are arranged in a
			 * polygon tree for the next step.
			 * 
			 * 2) Cutting the inner polygons to make a flat list of only
			 * outer polygons.
			 * 
			 * 3) Ear-clipping the polygons to create triangles.
			 * 
			 */

			// TRACING
			polys = DoTrace(s);
			
			// CUTTING
			DoCutting(polys);
			
			// EAR-CLIPPING
			foreach(EarClipPolygon p in polys)
				islandslist.Add(DoEarClip(p, verticeslist, sidedefslist));

			// Make arrays
			islandvertices = Array.AsReadOnly<int>(islandslist.ToArray());
			vertices = Array.AsReadOnly<Vector2D>(verticeslist.ToArray());
			sidedefs = Array.AsReadOnly<Sidedef>(sidedefslist.ToArray());
		}

		#endregion

		#region ================== Tracing

		// This traces sector lines to create a polygon tree
		private List<EarClipPolygon> DoTrace(Sector s)
		{
			Dictionary<Sidedef, bool> todosides = new Dictionary<Sidedef, bool>(s.Sidedefs.Count);
			Dictionary<Vertex, Vertex> ignores = new Dictionary<Vertex,Vertex>();
			List<EarClipPolygon> root = new List<EarClipPolygon>();
			SidedefsTracePath path;
			EarClipPolygon newpoly;
			Vertex start;
			
			// Fill the dictionary
			// The bool value is used to indicate lines which has been visited in the trace
			foreach(Sidedef sd in s.Sidedefs) todosides.Add(sd, false);
			
			// First remove all sides that refer to the same sector on both sides of the line
			RemoveDoubleSidedefReferences(todosides, s.Sidedefs);

			// Continue until all sidedefs have been processed
			while(todosides.Count > 0)
			{
				// Reset all visited indicators
				foreach(Sidedef sd in s.Sidedefs) if(todosides.ContainsKey(sd)) todosides[sd] = false;
				
				// Find the right-most vertex to start a trace with.
				// This guarantees that we start out with an outer polygon and we just
				// have to check if it is inside a previously found polygon.
				start = FindRightMostVertex(todosides, ignores);

				// No more possible start vertex found?
				// Then leave with what we have up till now.
				if(start == null) break;
				
				// Trace to find a polygon
				path = DoTracePath(new SidedefsTracePath(), start, null, s, todosides);

				// If tracing is not possible (sector not closed?)
				// then add the start to the ignore list and try again later
				if(path == null)
				{
					// Ignore vertex as start
					ignores.Add(start, start);
				}
				else
				{
					// Remove the sides found in the path
					foreach(Sidedef sd in path) todosides.Remove(sd);

					// Create the polygon
					newpoly = path.MakePolygon();
					#if DEBUG
					if(OnShowPolygon != null) OnShowPolygon(newpoly, General.Colors.Selection);
					#endif
					
					// Determine where this polygon goes in our tree
					foreach(EarClipPolygon p in root)
					{
						// Insert if it belongs as a child
						if(p.InsertChild(newpoly))
						{
							// Done
							newpoly = null;
							break;
						}
					}

					// Still not inserted in our tree?
					if(newpoly != null)
					{
						// Then add it at root level as outer polygon
						newpoly.Inner = false;
						root.Add(newpoly);
					}
				}
			}

			// Return result
			return root;
		}

		// This recursively traces a path
		// Returns the resulting TracePath when the search is complete
		// or returns null when no path found.
		private SidedefsTracePath DoTracePath(SidedefsTracePath history, Vertex fromhere, Vertex findme, Sector sector, Dictionary<Sidedef, bool> sides)
		{
			SidedefsTracePath nextpath;
			SidedefsTracePath result;
			Vertex nextvertex;
			List<Sidedef> allsides;
			SidedefAngleSorter sorter;
			
			// Found the vertex we are tracing to?
			if(fromhere == findme) return history;

			// On the first run, findme is null (otherwise the trace would end
			// immeditely when it starts) so set findme here on the first run.
			if(findme == null) findme = fromhere;

			// Make a list of sides referring to the same sector
			allsides = new List<Sidedef>(fromhere.Linedefs.Count * 2);
			foreach(Linedef l in fromhere.Linedefs)
			{
				// Should we go along the front or back side?
				// This is very important for clockwise polygon orientation!
				if(l.Start == fromhere)
				{
					// Front side of line connected to sector?
					if((l.Front != null) && (l.Front.Sector == sector))
					{
						// Visit here when not visited yet
						if(sides.ContainsKey(l.Front) && !sides[l.Front]) allsides.Add(l.Front);
					}
				}
				else
				{
					// Back side of line connected to sector?
					if((l.Back != null) && (l.Back.Sector == sector))
					{
						// Visit here when not visited yet
						if(sides.ContainsKey(l.Back) && !sides[l.Back]) allsides.Add(l.Back);
					}
				}
			}

			// Previous line available?
			if(history.Count > 0)
			{
				// This is done to ensure the tracing works along vertices that are shared by
				// more than 2 lines/sides of the same sector. We must continue tracing along
				// the first next smallest delta angle! This sorts the smallest delta angle to
				// the top of the list.
				sorter = new SidedefAngleSorter(history[history.Count - 1], fromhere);
				allsides.Sort(sorter);
			}
			
			// Go for all lines connected to this vertex
			foreach(Sidedef s in allsides)
			{
				// Mark sidedef as visited and move to next vertex
				sides[s] = true;
				nextpath = new SidedefsTracePath(history, s);
				if(s.Line.Start == fromhere) nextvertex = s.Line.End; else nextvertex = s.Line.Start;
				
				// TEST
				#if DEBUG
				if(s.IsFront)
				{
					if(OnShowLine != null) OnShowLine(s.Line.Start.Position, s.Line.End.Position, PixelColor.FromColor(Color.Chartreuse));
				}
				else
				{
					if(OnShowLine != null) OnShowLine(s.Line.Start.Position, s.Line.End.Position, PixelColor.FromColor(Color.DeepSkyBlue));
				}
				#endif
				
				result = DoTracePath(nextpath, nextvertex, findme, sector, sides);
				if(result != null) return result;
			}

			// Nothing found
			return null;
		}

		// This removes all sidedefs which has a sidedefs on the other side
		// of the same line that refers to the same sector. These are removed
		// because they are useless and make the triangulation inefficient.
		private static void RemoveDoubleSidedefReferences(Dictionary<Sidedef, bool> todosides, ICollection<Sidedef> sides)
		{
			// Go for all sides
			foreach(Sidedef sd in sides)
			{
				// Double sided?
				if(sd.Other != null)
				{
					// Referring to the same sector on both sides?
					if(sd.Sector == sd.Other.Sector)
					{
						// Remove this one
						todosides.Remove(sd);
					}
				}
			}
		}

		// This finds the right-most vertex to start tracing with
		private static Vertex FindRightMostVertex(Dictionary<Sidedef, bool> sides, Dictionary<Vertex, Vertex> ignores)
		{
			Vertex found = null;
			
			// Go for all sides to find the right-most side
			foreach(KeyValuePair<Sidedef, bool> sd in sides)
			{
				// First found?
				if((found == null) && !ignores.ContainsKey(sd.Key.Line.Start)) found = sd.Key.Line.Start;
				if((found == null) && !ignores.ContainsKey(sd.Key.Line.End)) found = sd.Key.Line.End;
				
				// Compare?
				if(found != null)
				{
					// Check if more to the right than the previous found
					if((sd.Key.Line.Start.Position.x > found.Position.x) && !ignores.ContainsKey(sd.Key.Line.Start)) found = sd.Key.Line.Start;
					if((sd.Key.Line.End.Position.x > found.Position.x) && !ignores.ContainsKey(sd.Key.Line.End)) found = sd.Key.Line.End;
				}
			}
			
			// Return result
			return found;
		}
		
		#endregion

		#region ================== Cutting

		// This cuts into outer polygons to solve inner polygons and make the polygon tree flat
		private void DoCutting(List<EarClipPolygon> polys)
		{
			Queue<EarClipPolygon> todo = new Queue<EarClipPolygon>(polys);
			
			// Begin processing outer polygons
			while(todo.Count > 0)
			{
				// Get outer polygon to process
				EarClipPolygon p = todo.Dequeue();

				// Any inner polygons to work with?
				if(p.Children.Count > 0)
				{
					// Go for all the children
					foreach(EarClipPolygon c in p.Children)
					{
						// The children of the children are outer polygons again,
						// so move them to the root and add for processing
						polys.AddRange(c.Children);
						foreach(EarClipPolygon sc in c.Children) todo.Enqueue(sc);

						// Remove from inner polygon
						c.Children.Clear();
					}

					// Now do some cutting on this polygon to merge the inner polygons
					MergeInnerPolys(p);
				}
			}
		}

		// This takes an outer polygon and a set of inner polygons to start cutting on
		private void MergeInnerPolys(EarClipPolygon p)
		{
			LinkedList<EarClipPolygon> todo = new LinkedList<EarClipPolygon>(p.Children);
			LinkedListNode<EarClipVertex> start;
			LinkedListNode<EarClipPolygon> ip;
			LinkedListNode<EarClipPolygon> found;
			LinkedListNode<EarClipVertex> foundstart;
			
			// Continue until no more inner polygons to process
			while(todo.Count > 0)
			{
				// Find the inner polygon with the highest x vertex
				found = null;
				foundstart = null;
				ip = todo.First;
				while(ip != null)
				{
					start = FindRightMostVertex(ip.Value);
					if((foundstart == null) || (start.Value.Position.x > foundstart.Value.Position.x))
					{
						// Found a better start
						found = ip;
						foundstart = start;
					}
					
					// Next!
					ip = ip.Next;
				}
				
				// Remove from todo list
				todo.Remove(found);

				// Get cut start and end
				SplitOuterWithInner(foundstart, p, found.Value);
			}
			
			// Remove the children, they should be merged in the polygon by now
			p.Children.Clear();
		}

		// This finds the right-most vertex in an inner polygon to use for cut startpoint.
		private static LinkedListNode<EarClipVertex> FindRightMostVertex(EarClipPolygon p)
		{
			LinkedListNode<EarClipVertex> found = p.First;
			LinkedListNode<EarClipVertex> v = found.Next;
			
			// Go for all vertices to find the on with the biggest x value
			while(v != null)
			{
				if(v.Value.Position.x > found.Value.Position.x) found = v;
				v = v.Next;
			}

			// Return result
			return found;
		}
		
		// This finds the cut coordinates and splits the other poly with inner vertices
		private static void SplitOuterWithInner(LinkedListNode<EarClipVertex> start, EarClipPolygon p, EarClipPolygon inner)
		{
			Line2D starttoright = new Line2D(start.Value.Position, start.Value.Position + new Vector2D(1000.0f, 0.0f));
			LinkedListNode<EarClipVertex> v1, v2;
			LinkedListNode<EarClipVertex> insertbefore = null;
			float u, ul, foundu = float.MaxValue;
			EarClipVertex split;
			
			// Go for all lines in the outer polygon
			v1 = p.Last;
			v2 = p.First;
			while(v2 != null)
			{
				// Check if the line is to the right of start
				if((v1.Value.Position.x > start.Value.Position.x) ||
				   (v2.Value.Position.x > start.Value.Position.x))
				{
					// Find intersection
					Line2D pl = new Line2D(v1.Value.Position, v2.Value.Position);
					pl.GetIntersection(starttoright, out u, out ul);
					if(float.IsNaN(u))
					{
						// We have found a line that is perfectly horizontal
						// (parallel to the cut scan line) Check if the line
						// is overlapping the cut scan line.
						if(v1.Value.Position.y == start.Value.Position.y)
						{
							// Calculate distance of each vertex in units
							u = starttoright.GetNearestOnLine(v1.Value.Position);
							ul = starttoright.GetNearestOnLine(v2.Value.Position);

							// Rule out vertices before the scan line
							if(u < 0) u = float.MaxValue;
							if(ul < 0) ul = float.MaxValue;

							// Choose closest of both vertices
							if((u < ul) && (u < foundu))
							{
								insertbefore = v2;
								foundu = u;
							}
							else if((u > ul) && (ul < foundu))
							{
								insertbefore = v2;
								foundu = ul;
							}
						}
					}
					// Found a closer match?
					else if((ul >= 0) && (ul <= 1) && (u > 0) && (u < foundu))
					{
						// Found a closer intersection
						insertbefore = v2;
						foundu = u;
					}
				}
				
				// Next
				v1 = v2;
				v2 = v2.Next;
			}
			
			// Found anything?
			if(insertbefore != null)
			{
				Sidedef sd = (insertbefore.Previous == null) ? insertbefore.List.Last.Value.Sidedef : insertbefore.Previous.Value.Sidedef;
				
				// Find the position where we have to split the outer polygon
				split = new EarClipVertex(starttoright.GetCoordinatesAt(foundu), null);
				
				// Insert manual split vertices
				p.AddBefore(insertbefore, new EarClipVertex(split, sd));
				
				// Start inserting from the start (do I make sense this time?)
				v1 = start;
				do
				{
					// Insert inner polygon vertex
					p.AddBefore(insertbefore, new EarClipVertex(v1.Value));
					if(v1.Next != null) v1 = v1.Next; else v1 = v1.List.First;
				}
				while(v1 != start);
				
				// Insert manual split vertices
				p.AddBefore(insertbefore, new EarClipVertex(start.Value, sd));
				p.AddBefore(insertbefore, new EarClipVertex(split, sd));
			}
		}

		#endregion

		#region ================== Ear Clipping

		// This clips a polygon and returns the triangles
		// The polygon may not have any holes or islands
		/// See: http://www.geometrictools.com/Documentation/TriangulationByEarClipping.pdf
		private int DoEarClip(EarClipPolygon poly, List<Vector2D> verticeslist, List<Sidedef> sidedefslist)
		{
			LinkedList<EarClipVertex> verts = new LinkedList<EarClipVertex>();
			List<EarClipVertex> convexes = new List<EarClipVertex>(poly.Count);
			LinkedList<EarClipVertex> reflexes = new LinkedList<EarClipVertex>();
			LinkedList<EarClipVertex> eartips = new LinkedList<EarClipVertex>();
			EarClipVertex v, v1, v2;
			EarClipVertex[] t, t1, t2;
			int countvertices = 0;
			
			// Go for all vertices to fill list
			foreach(EarClipVertex vec in poly)
				vec.SetVertsLink(verts.AddLast(vec));
			
			// Optimization: Vertices which have lines with the
			// same angle are useless. Remove them!
			v = verts.First.Value;
			while(v != null)
			{
				// Get the next vertex
				if(v.MainListNode.Next != null) v1 = v.MainListNode.Next.Value; else v1 = null;
				
				// Get triangle for v
				t = GetTriangle(v);
				
				// Check if both lines have the same angle
				Line2D a = new Line2D(t[0].Position, t[1].Position);
				Line2D b = new Line2D(t[1].Position, t[2].Position);
				if(Math.Abs(Angle2D.Difference(a.GetAngle(), b.GetAngle())) < 0.00001f)
				{
					// Same angles, remove vertex
					v.Remove();
				}
				
				// Next!
				v = v1;
			}
			
			// Go for all vertices to determine reflex or convex
			foreach(EarClipVertex vv in verts)
			{
				// Add to reflex or convex list
				if(IsReflex(GetTriangle(vv))) vv.AddReflex(reflexes); else convexes.Add(vv);
			}

			// Go for all convex vertices to see if they are ear tips
			foreach(EarClipVertex cv in convexes)
			{
				// Add when this is a valid ear
				t = GetTriangle(cv);
				if(CheckValidEar(t, reflexes)) cv.AddEarTip(eartips);
			}

			// Process ears until done
			while((eartips.Count > 0) && (verts.Count > 2))
			{
				// Get next ear
				v = eartips.First.Value;
				t = GetTriangle(v);

				// Add ear as triangle
				AddTriangleToList(t, verticeslist, sidedefslist, (verts.Count == 3));
				countvertices += 3;
				
				// Remove this ear from all lists
				v.Remove();
				v1 = t[0];
				v2 = t[2];

				// Test first neighbour
				t1 = GetTriangle(v1);
				if(IsReflex(t1))
				{
					// List as reflex if not listed yet
					if(!v1.IsReflex) v1.AddReflex(reflexes);
					v1.RemoveEarTip();
				}
				else
				{
					// Remove from reflexes
					v1.RemoveReflex();
				}
				
				// Test second neighbour
				t2 = GetTriangle(v2);
				if(IsReflex(t2))
				{
					// List as reflex if not listed yet
					if(!v2.IsReflex) v2.AddReflex(reflexes);
					v2.RemoveEarTip();
				}
				else
				{
					// Remove from reflexes
					v2.RemoveReflex();
				}
				
				// Check if any neightbour have become a valid or invalid ear
				if(!v1.IsReflex && CheckValidEar(t1, reflexes)) v1.AddEarTip(eartips); else v1.RemoveEarTip();
				if(!v2.IsReflex && CheckValidEar(t2, reflexes)) v2.AddEarTip(eartips); else v2.RemoveEarTip();
			}
			
			// Dispose remaining vertices
			foreach(EarClipVertex ecv in verts) ecv.Dispose();

			// Return the number of vertices in the result
			return countvertices;
		}

		// This checks if a given ear is a valid (no intersections from reflex vertices)
		private bool CheckValidEar(EarClipVertex[] t, LinkedList<EarClipVertex> reflexes)
		{
			// Go for all reflex vertices
			foreach(EarClipVertex rv in reflexes)
			{
				// Return false on intersection
				if(PointInsideTriangle(t, rv.Position) &&
				   (rv != t[0]) && (rv != t[1]) && (rv != t[2])) return false;
			}

			// Valid ear!
			return true;
		}
		
		// This returns the 3-vertex array triangle for an ear
		private static EarClipVertex[] GetTriangle(EarClipVertex v)
		{
			EarClipVertex[] t = new EarClipVertex[3];
			if(v.MainListNode.Previous == null) t[0] = v.MainListNode.List.Last.Value; else t[0] = v.MainListNode.Previous.Value;
			t[1] = v;
			if(v.MainListNode.Next == null) t[2] = v.MainListNode.List.First.Value; else t[2] = v.MainListNode.Next.Value;
			return t;
		}
		
		// This checks if a vertex is reflex (corner > 180 deg) or convex (corner < 180 deg)
		private static bool IsReflex(EarClipVertex[] t)
		{
			// Return true when corner is > 180 deg
			//return (Line2D.GetSideOfLine(t[0].Position, t[2].Position, t[1].Position) < 0.00001f);
			return (Line2D.GetSideOfLine(t[0].Position, t[2].Position, t[1].Position) < 0);
		}
		
		// This checks if a point is inside a triangle
		// NOTE: vertices in t must be in clockwise order!
		private static bool PointInsideTriangle(EarClipVertex[] t, Vector2D p)
		{
			return (Line2D.GetSideOfLine(t[0].Position, t[1].Position, p) < 0.00001f) &&
				   (Line2D.GetSideOfLine(t[1].Position, t[2].Position, p) < 0.00001f) &&
				   (Line2D.GetSideOfLine(t[2].Position, t[0].Position, p) < 0.00001f);
		}

		// This adds an array of vertices
		private void AddTriangleToList(EarClipVertex[] triangle, List<Vector2D> verticeslist, List<Sidedef> sidedefslist, bool last)
		{
			// Create triangle
			verticeslist.Add(triangle[0].Position);
			sidedefslist.Add(triangle[0].Sidedef);
			verticeslist.Add(triangle[1].Position);
			sidedefslist.Add(triangle[1].Sidedef);
			verticeslist.Add(triangle[2].Position);
			if(!last) sidedefslist.Add(null); else sidedefslist.Add(triangle[2].Sidedef);
			
			// Modify the first earclipvertex of this triangle, it no longer lies along a sidedef
			triangle[0].Sidedef = null;
		}
		
		#endregion
	}
}
