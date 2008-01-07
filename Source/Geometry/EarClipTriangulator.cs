
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
using SlimDX.Direct3D;
using System.Drawing;
using CodeImp.DoomBuilder.Map;

#endregion

namespace CodeImp.DoomBuilder.Geometry
{
	/// <summary>
	/// Responsible for creating and caching sector polygons.
	/// Performs triangulation of sectors by using ear clipping.
	/// </summary>
	/// See: http://www.geometrictools.com/Documentation/TriangulationByEarClipping.pdf
	public sealed class EarClipTriangulator : Triangulator
	{
		#region ================== Delegates

		// For debugging purpose only!
		// These are not called in a release build
		public delegate void ShowLine(Vector2D v1, Vector2D v2, PixelColor c);
		public delegate void ShowPolygon(Polygon p, PixelColor c);
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

		#endregion

		#region ================== Properties

		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		public EarClipTriangulator()
		{
			// Initialize
			
			// We have no destructor
			GC.SuppressFinalize(this);
		}

		// Diposer
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

		// This triangulates a sector and stores it
		protected override void PerformTriangulation(Sector sector)
		{
			TriangleList triangles = new TriangleList();
			List<Polygon> polys;
			
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
			polys = DoTrace(sector);
			
			// TODO: CUTTING
			
			// EAR-CLIPPING
			foreach(Polygon p in polys) triangles.AddRange(DoEarClip(p));

			// STORE
			base.StoreTriangles(sector, triangles);
		}

		#endregion
		
		#region ================== Tracing

		// This traces sector lines to create a polygon tree
		private List<Polygon> DoTrace(Sector s)
		{
			Dictionary<Sidedef, bool> todosides = new Dictionary<Sidedef, bool>(s.Sidedefs.Count);
			List<Polygon> root = new List<Polygon>();
			TracePath path;
			Polygon newpoly;
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
				start = FindRightMostVertex(todosides);

				// Trace to find a polygon
				path = DoTracePath(new TracePath(), start, null, s, todosides);

				// If tracing is not possible (sector not closed?)
				// then leave with what we have up till now
				if(path == null) break;
				
				// Remove the sides found in the path
				foreach(Sidedef sd in path) todosides.Remove(sd);

				// Create the polygon
				newpoly = path.MakePolygon();
				
				// Determine where this polygon goes in our tree
				foreach(Polygon p in root)
				{
					// Insert if it belongs as a child
					if(p.InsertChild(newpoly))
					{
						#if DEBUG
						if(newpoly.Inner)
						{
							if(OnShowPolygon != null) OnShowPolygon(newpoly, PixelColor.FromColor(Color.DodgerBlue));
						}
						else
						{
							if(OnShowPolygon != null) OnShowPolygon(newpoly, PixelColor.FromColor(Color.OrangeRed));
						}
						#endif
						
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
					
					#if DEBUG
					if(OnShowPolygon != null) OnShowPolygon(newpoly, General.Colors.Selection);
					#endif
				}
			}

			// Return result
			return root;
		}

		// This recursively traces a path
		// Returns the resulting TracePath when the search is complete
		// or returns null when no path found.
		private TracePath DoTracePath(TracePath history, Vertex fromhere, Vertex findme, Sector sector, Dictionary<Sidedef, bool> sides)
		{
			TracePath nextpath;
			TracePath result;
			Vertex nextvertex;
			
			// Found the vertex we are tracing to?
			if(fromhere == findme) return history;

			// On the first run, findme is null (otherwise the trace would end
			// immeditely when it starts) so set findme here on the first run.
			if(findme == null) findme = fromhere;
			
			// Go for all lines connected to this vertex
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
						if(sides.ContainsKey(l.Front) && !sides[l.Front])
						{
							// Mark sidedef as visited and move to next vertex
							sides[l.Front] = true;
							nextpath = new TracePath(history, l.Front);
							if(l.Start == fromhere) nextvertex = l.End; else nextvertex = l.Start;
							result = DoTracePath(nextpath, nextvertex, findme, sector, sides);
							if(result != null) return result;
						}
					}
				}
				else
				{
					// Back side of line connected to sector?
					if((l.Back != null) && (l.Back.Sector == sector))
					{
						// Visit here when not visited yet
						if(sides.ContainsKey(l.Back) && !sides[l.Back])
						{
							// Mark sidedef as visited and move to next vertex
							sides[l.Back] = true;
							nextpath = new TracePath(history, l.Back);
							if(l.Start == fromhere) nextvertex = l.End; else nextvertex = l.Start;
							result = DoTracePath(nextpath, nextvertex, findme, sector, sides);
							if(result != null) return result;
						}
					}
				}
			}

			// Nothing found
			return null;
		}

		// This removes all sidedefs which has a sidedefs on the other side
		// of the same line that refers to the same sector. These are removed
		// because they are useless and make the triangulation inefficient.
		private void RemoveDoubleSidedefReferences(Dictionary<Sidedef, bool> todosides, ICollection<Sidedef> sides)
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
		private Vertex FindRightMostVertex(Dictionary<Sidedef, bool> sides)
		{
			Vertex found = General.GetByIndex<Sidedef>(sides.Keys, 0).Line.Start;
			
			// Go for all sides to find the right-most side
			foreach(KeyValuePair<Sidedef, bool> sd in sides)
			{
				// Check if more to the right than the last found
				if(sd.Key.Line.Start.X > found.X) found = sd.Key.Line.Start;
				if(sd.Key.Line.End.X > found.X) found = sd.Key.Line.End;
			}

			// Return result
			return found;
		}
		
		#endregion

		#region ================== Cutting

		#endregion

		#region ================== Ear Clipping

		// This clips a polygon and returns the triangles
		// The polygon may not have any holes or islands
		private TriangleList DoEarClip(Polygon poly)
		{
			LinkedList<EarClipVertex> verts = new LinkedList<EarClipVertex>();
			List<EarClipVertex> convexes = new List<EarClipVertex>(poly.Count);
			LinkedList<EarClipVertex> reflexes = new LinkedList<EarClipVertex>();
			LinkedList<EarClipVertex> eartips = new LinkedList<EarClipVertex>();
			TriangleList result = new TriangleList();
			EarClipVertex v, v1, v2;
			EarClipVertex[] t, t1, t2;
			
			// Go for all vertices to fill list
			foreach(Vector2D vec in poly)
			{
				// Add to main list
				v = new EarClipVertex(vec);
				v.SetVertsLink(verts.AddLast(v));
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
				// Add when this a valid ear
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
				result.Add(t);
				
				// Remove this ear from all lists
				v.Remove();
				v1 = t[0];
				v2 = t[2];

				#if DEBUG
				if(OnShowEarClip != null) OnShowEarClip(t, verts);
				#endif
				
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
			
			// Return result
			return result;
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
		private EarClipVertex[] GetTriangle(EarClipVertex v)
		{
			EarClipVertex[] t = new EarClipVertex[3];
			if(v.MainListNode.Previous == null) t[0] = v.MainListNode.List.Last.Value; else t[0] = v.MainListNode.Previous.Value;
			t[1] = v;
			if(v.MainListNode.Next == null) t[2] = v.MainListNode.List.First.Value; else t[2] = v.MainListNode.Next.Value;
			return t;
		}
		
		// This checks if a vertex is reflex (corner > 180 deg) or convex (corner < 180 deg)
		private bool IsReflex(EarClipVertex[] t)
		{
			// Return true when corner is > 180 deg
			return (Line2D.GetSideOfLine(t[0].Position, t[2].Position, t[1].Position) < 0.00001f);
		}
		
		// This checks if a point is inside a triangle
		// NOTE: vertices in t must be in clockwise order!
		private bool PointInsideTriangle(EarClipVertex[] t, Vector2D p)
		{
			#if DEBUG
			
			float a = Line2D.GetSideOfLine(t[0].Position, t[1].Position, p);
			float b = Line2D.GetSideOfLine(t[1].Position, t[2].Position, p);
			float c = Line2D.GetSideOfLine(t[2].Position, t[0].Position, p);
			
			return (a < 0.00001f) && (b < 0.00001f) && (c < 0.00001f);
			
			#else
			
			return (Line2D.GetSideOfLine(t[0].Position, t[1].Position, p) < 0.00001f) &&
				   (Line2D.GetSideOfLine(t[1].Position, t[2].Position, p) < 0.00001f) &&
				   (Line2D.GetSideOfLine(t[2].Position, t[0].Position, p) < 0.00001f);
			
			#endif
		}

		#endregion
	}
}
