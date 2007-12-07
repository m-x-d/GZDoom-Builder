
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
using SlimDX.Direct3D;
using CodeImp.DoomBuilder.Rendering;
using SlimDX;
using System.Drawing;

#endregion

namespace CodeImp.DoomBuilder.Map
{
	public sealed class MapSet : IDisposable
	{
		#region ================== Variables

		// Map structures
		private LinkedList<Vertex> vertices;
		private LinkedList<Linedef> linedefs;
		private LinkedList<Sidedef> sidedefs;
		private LinkedList<Sector> sectors;
		private LinkedList<Thing> things;
		
		// Disposing
		private bool isdisposed = false;

		#endregion

		#region ================== Properties

		public ICollection<Vertex> Vertices { get { return vertices; } }
		public ICollection<Linedef> Linedefs { get { return linedefs; } }
		public ICollection<Sidedef> Sidedefs { get { return sidedefs; } }
		public ICollection<Sector> Sectors { get { return sectors; } }
		public ICollection<Thing> Things { get { return things; } }
		public bool IsDisposed { get { return isdisposed; } }
		
		#endregion

		#region ================== Constructor / Disposer

		// Constructor for new empty map
		public MapSet()
		{
			// Initialize
			vertices = new LinkedList<Vertex>();
			linedefs = new LinkedList<Linedef>();
			sidedefs = new LinkedList<Sidedef>();
			sectors = new LinkedList<Sector>();
			things = new LinkedList<Thing>();

			// We have no destructor
			GC.SuppressFinalize(this);
		}

		// Diposer
		public void Dispose()
		{
			ArrayList list;
			
			// Not already disposed?
			if(!isdisposed)
			{
				// Already set isdisposed so that changes can be prohibited
				isdisposed = true;

				// Dispose all things
				list = new ArrayList(things);
				foreach(Thing t in list) t.Dispose();

				// Dispose all sectors
				list = new ArrayList(sectors);
				foreach(Sector s in list) s.Dispose();

				// Dispose all sidedefs
				list = new ArrayList(sidedefs);
				foreach(Sidedef sd in list) sd.Dispose();

				// Dispose all linedefs
				list = new ArrayList(linedefs);
				foreach(Linedef l in list) l.Dispose();

				// Dispose all vertices
				list = new ArrayList(vertices);
				foreach(Vertex v in list) v.Dispose();

				// Clean up
				vertices = null;
				linedefs = null;
				sidedefs = null;
				sectors = null;
				things = null;
				
				// Done
				isdisposed = true;
			}
		}

		#endregion

		#region ================== Management

		// This makes a deep copy and returns a new MapSet
		public MapSet Clone()
		{
			Linedef nl;
			Sidedef nd;
			
			// Create the map set
			MapSet newset = new MapSet();

			// Go for all vertices
			foreach(Vertex v in vertices)
			{
				// Make new vertex
				v.Clone = newset.CreateVertex(v.X, v.Y);
			}

			// Go for all sectors
			foreach(Sector s in sectors)
			{
				// Make new sector
				Sector ns = newset.CreateSector();
				s.Clone = ns;
				
				// Copy properties
				s.CopyPropertiesTo(ns);
			}

			// Go for all linedefs
			foreach(Linedef l in linedefs)
			{
				// Make new linedef
				nl = newset.CreateLinedef(l.Start.Clone, l.End.Clone);
				
				// Copy properties
				l.CopyPropertiesTo(nl);

				// Linedef has a front side?
				if(l.Front != null)
				{
					// Make new sidedef
					nd = newset.CreateSidedef(nl, true, l.Front.Sector.Clone);

					// Copy properties
					l.Front.CopyPropertiesTo(nd);
				}

				// Linedef has a back side?
				if(l.Back != null)
				{
					// Make new sidedef
					nd = newset.CreateSidedef(nl, false, l.Back.Sector.Clone);

					// Copy properties
					l.Back.CopyPropertiesTo(nd);
				}
			}

			// Go for all things
			foreach(Thing t in things)
			{
				// Make new thing
				Thing nt = newset.CreateThing();

				// Copy properties
				t.CopyPropertiesTo(nt);
			}

			// Remove clone references
			foreach(Vertex v in vertices) v.Clone = null;
			foreach(Sector s in sectors) s.Clone = null;
			
			// Return the new set
			return newset;
		}
		
		// This creates a new vertex
		public Vertex CreateVertex(int x, int y)
		{
			LinkedListNode<Vertex> listitem;
			Vertex v;
			
			// Make a list item
			listitem = new LinkedListNode<Vertex>(null);

			// Make the vertex
			v = new Vertex(this, listitem, x, y);
			listitem.Value = v;

			// Add vertex to the list
			vertices.AddLast(listitem);

			// Return result
			return v;
		}

		// This creates a new linedef
		public Linedef CreateLinedef(Vertex start, Vertex end)
		{
			LinkedListNode<Linedef> listitem;
			Linedef l;

			// Make a list item
			listitem = new LinkedListNode<Linedef>(null);

			// Make the linedef
			l = new Linedef(this, listitem, start, end);
			listitem.Value = l;

			// Add linedef to the list
			linedefs.AddLast(listitem);

			// Return result
			return l;
		}

		// This creates a new sidedef
		public Sidedef CreateSidedef(Linedef l, bool front, Sector s)
		{
			LinkedListNode<Sidedef> listitem;
			Sidedef sd;

			// Make a list item
			listitem = new LinkedListNode<Sidedef>(null);

			// Make the sidedef
			sd = new Sidedef(this, listitem, l, front, s);
			listitem.Value = sd;

			// Add sidedef to the list
			sidedefs.AddLast(listitem);

			// Return result
			return sd;
		}

		// This creates a new sector
		public Sector CreateSector()
		{
			LinkedListNode<Sector> listitem;
			Sector s;

			// Make a list item
			listitem = new LinkedListNode<Sector>(null);

			// Make the sector
			s = new Sector(this, listitem);
			listitem.Value = s;

			// Add sector to the list
			sectors.AddLast(listitem);

			// Return result
			return s;
		}

		// This creates a new thing
		public Thing CreateThing()
		{
			LinkedListNode<Thing> listitem;
			Thing t;

			// Make a list item
			listitem = new LinkedListNode<Thing>(null);

			// Make the thing
			t = new Thing(this, listitem);
			listitem.Value = t;

			// Add thing to the list
			things.AddLast(listitem);

			// Return result
			return t;
		}

		#endregion

		#region ================== Updating

		// This updates all structures if needed
		public void Update()
		{
			// Update all linedefs
			foreach(Linedef l in linedefs) l.Update();
		}

		// This updates all structures after a
		// configuration or settings change
		public void UpdateConfiguration()
		{
			// Update all things
			foreach(Thing t in things) t.UpdateConfiguration();
		}
		
		#endregion

		#region ================== Selection

		// This clears all selected items
		public void ClearAllSelected()
		{
			ClearSelectedVertices();
			ClearSelectedThings();
			ClearSelectedLinedefs();
			ClearSelectedSectors();
		}

		// This clears selected vertices
		public void ClearSelectedVertices()
		{
			foreach(Vertex v in vertices) v.Selected = false;
		}

		// This clears selected things
		public void ClearSelectedThings()
		{
			foreach(Thing t in things) t.Selected = false;
		}

		// This clears selected linedefs
		public void ClearSelectedLinedefs()
		{
			foreach(Linedef l in linedefs) l.Selected = false;
		}

		// This clears selected sectors
		public void ClearSelectedSectors()
		{
			foreach(Sector s in sectors) s.Selected = false;
		}

		// Returns a collection of vertices that match a selected state
		public ICollection<Vertex> GetVerticesSelection(bool selected)
		{
			List<Vertex> list = new List<Vertex>();
			foreach(Vertex v in vertices) if(v.Selected == selected) list.Add(v);
			return list;
		}

		// Returns a collection of things that match a selected state
		public ICollection<Thing> GetThingsSelection(bool selected)
		{
			List<Thing> list = new List<Thing>();
			foreach(Thing t in things) if(t.Selected == selected) list.Add(t);
			return list;
		}

		// Returns a collection of linedefs that match a selected state
		public ICollection<Linedef> GetLinedefsSelection(bool selected)
		{
			List<Linedef> list = new List<Linedef>();
			foreach(Linedef l in linedefs) if(l.Selected == selected) list.Add(l);
			return list;
		}

		// Returns a collection of sectors that match a selected state
		public ICollection<Sector> GetSectorsSelection(bool selected)
		{
			List<Sector> list = new List<Sector>();
			foreach(Sector s in sectors) if(s.Selected == selected) list.Add(s);
			return list;
		}

		#endregion

		#region ================== Areas

		// This creates an area from vertices
		public static Rectangle AreaFromVertices(ICollection<Vertex> verts)
		{
			int l = int.MaxValue;
			int t = int.MaxValue;
			int r = int.MinValue;
			int b = int.MinValue;

			// Go for all vertices
			foreach(Vertex v in verts)
			{
				// Adjust boundaries by vertices
				if(v.X < l) l = v.X;
				if(v.X > r) r = v.X;
				if(v.Y < t) t = v.Y;
				if(v.Y > b) b = v.Y;
			}

			// Return a rect
			return new Rectangle(l, t, r - l, b - t);
		}

		// This creates an area from vertices
		public static Rectangle AreaFromLines(ICollection<Linedef> lines)
		{
			int l = int.MaxValue;
			int t = int.MaxValue;
			int r = int.MinValue;
			int b = int.MinValue;

			// Go for all vertices
			foreach(Linedef ld in lines)
			{
				// Adjust boundaries by vertices
				if(ld.Start.X < l) l = ld.Start.X;
				if(ld.Start.X > r) r = ld.Start.X;
				if(ld.Start.Y < t) t = ld.Start.Y;
				if(ld.Start.Y > b) b = ld.Start.Y;
				if(ld.End.X < l) l = ld.End.X;
				if(ld.End.X > r) r = ld.End.X;
				if(ld.End.Y < t) t = ld.End.Y;
				if(ld.End.Y > b) b = ld.End.Y;
			}

			// Return a rect
			return new Rectangle(l, t, r - l, b - t);
		}
		
		// This filters lines by a square area
		public static ICollection<Linedef> FilterArea(ICollection<Linedef> lines, ref Rectangle area)
		{
			ICollection<Linedef> newlines = new List<Linedef>(lines.Count);
			
			// Go for all lines
			foreach(Linedef l in lines)
			{
				// Check the cs field bits
				if((GetCSFieldBits(l.Start, ref area) & GetCSFieldBits(l.End, ref area)) == 0)
				{
					// The line could be in the area
					newlines.Add(l);
				}
			}
			
			// Return result
			return newlines;
		}

		// This returns the cohen-sutherland field bits for a vertex in a rectangle area
		private static byte GetCSFieldBits(Vertex v, ref Rectangle area)
		{
			byte bits = 0;
			if(v.Y < area.Top) bits |= 0x01;
			if(v.Y > area.Bottom) bits |= 0x02;
			if(v.X < area.Left) bits |= 0x04;
			if(v.X > area.Right) bits |= 0x08;
			return bits;
		}

		// This filters vertices by a square area
		public static ICollection<Vertex> FilterArea(ICollection<Vertex> verts, ref Rectangle area)
		{
			ICollection<Vertex> newverts = new List<Vertex>(verts.Count);

			// Go for all verts
			foreach(Vertex v in verts)
			{
				// Within rect?
				if((v.X >= area.Left) &&
				   (v.X <= area.Right) &&
				   (v.Y >= area.Top) &&
				   (v.Y <= area.Bottom))
				{
					// The vertex is in the area
					newverts.Add(v);
				}
			}

			// Return result
			return newverts;
		}

		#endregion

		#region ================== Geometry Tools

		// This joins overlapping lines together
		// Returns the number of joins made
		public static int JoinOverlappingLines(ICollection<Linedef> lines)
		{
			int joinsdone = 0;
			bool joined;
			
			do
			{
				// No joins yet
				joined = false;

				// Go for all the lines
				foreach(Linedef l1 in lines)
				{
					// Check if these vertices have lines that overlap
					foreach(Linedef l2 in l1.Start.Linedefs)
					{
						// Sharing vertices?
						if((l1.End == l2.End) ||
						   (l1.End == l2.Start))
						{
							// Not the same line?
							if(l1 != l2)
							{
								// Merge these two linedefs
								while(lines.Remove(l1));
								l1.Join(l2);
								joinsdone++;
								joined = true;
								break;
							}
						}
					}
					
					// Will have to restart when joined
					if(joined) break;
					
					// Check if these vertices have lines that overlap
					foreach(Linedef l2 in l1.End.Linedefs)
					{
						// Sharing vertices?
						if((l1.Start == l2.End) ||
						   (l1.Start == l2.Start))
						{
							// Not the same line?
							if(l1 != l2)
							{
								// Merge these two linedefs
								while(lines.Remove(l1));
								l1.Join(l2);
								joinsdone++;
								joined = true;
								break;
							}
						}
					}
					
					// Will have to restart when joined
					if(joined) break;
				}
			}
			while(joined);

			// Return result
			return joinsdone;
		}
		
		// This removes looped linedefs (linedefs which reference the same vertex for start and end)
		// Returns the number of linedefs removed
		public static int RemoveLoopedLinedefs(ICollection<Linedef> lines)
		{
			int linesremoved = 0;
			bool removedline;

			do
			{
				// Nothing removed yet
				removedline = false;

				// Go for all the lines
				foreach(Linedef l in lines)
				{
					// Check if referencing the same vertex twice
					if(l.Start == l.End)
					{
						// Remove this line
						while(lines.Remove(l));
						l.Dispose();
						linesremoved++;
						removedline = true;
						break;
					}
				}
			}
			while(removedline);

			// Return result
			return linesremoved;
		}

		// This joins nearby vertices from two collections. This does NOT join vertices
		// within the same collection, only if they exist in both collections.
		// The vertex from the second collection is moved to match the first vertex.
		// When keepsecond is true, the vertex in the second collection is kept,
		// otherwise the vertex in the first collection is kept.
		// Returns the number of joins made
		public static int JoinVertices(ICollection<Vertex> set1, ICollection<Vertex> set2, bool keepsecond, float joindist)
		{
			float joindist2 = joindist * joindist;
			int joinsdone = 0;
			bool joined;

			do
			{
				// No joins yet
				joined = false;

				// Go for all vertices in the first set
				foreach(Vertex v1 in set1)
				{
					// Go for all vertices in the second set
					foreach(Vertex v2 in set2)
					{
						// Check if vertices are close enough
						if(v1.DistanceToSq(v2.Position) <= joindist2)
						{
							// Check if not the same vertex
							if(v1 != v2)
							{
								// Move the second vertex to match the first
								v2.Move(v1.Position);
								
								// Check which one to keep
								if(keepsecond)
								{
									// Join the first into the second
									// Second is kept, first is removed
									v1.Join(v2);
									set1.Remove(v1);
									set2.Remove(v1);
								}
								else
								{
									// Join the second into the first
									// First is kept, second is removed
									v2.Join(v1);
									set1.Remove(v2);
									set2.Remove(v2);
								}
								
								// Count the join
								joinsdone++;
								joined = true;
								break;
							}
						}
					}

					// Will have to restart when joined
					if(joined) break;
				}
			}
			while(joined);

			// Return result
			return joinsdone;
		}
		
		// This splits the given lines with the given vertices
		// All affected lines will be added to changedlines
		// Returns the number of splits made
		public static int SplitLinesByVertices(ICollection<Linedef> lines, ICollection<Vertex> verts, float splitdist, ICollection<Linedef> changedlines)
		{
			float splitdist2 = splitdist * splitdist;
			int splitsdone = 0;
			bool splitted;
			Linedef nl;

			do
			{
				// No split yet
				splitted = false;
				
				// Go for all the lines
				foreach(Linedef l in lines)
				{
					// Go for all the vertices
					foreach(Vertex v in verts)
					{
						// Check if v is close enough to l for splitting
						if(l.DistanceToSq(v.Position, true) <= splitdist2)
						{
							// Line is not already referencing v?
							if((l.Start != v) && (l.End != v))
							{
								// Split line l with vertex v
								nl = l.Split(v);

								// Add the new line to the list
								lines.Add(nl);

								// Both lines must be updated because their new length
								// is relevant for next iterations!
								l.Update();
								nl.Update();

								// Add both lines to changedlines
								if(changedlines != null) changedlines.Add(l);
								if(changedlines != null) changedlines.Add(nl);

								// Count the split
								splitsdone++;
								splitted = true;
								break;
							}
						}
					}

					// Will have to restart when splitted
					if(splitted) break;
				}
			}
			while(splitted);

			// Return result
			return splitsdone;
		}
		
		// This finds the line closest to the specified position
		public static Linedef NearestLinedef(ICollection<Linedef> selection, Vector2D pos)
		{
			Linedef closest = null;
			float distance = float.MaxValue;
			float d;

			// Go for all linedefs in selection
			foreach(Linedef l in selection)
			{
				// Calculate distance and check if closer than previous find
				d = l.SafeDistanceToSq(pos, true);
				if(d < distance)
				{
					// This one is closer
					closest = l;
					distance = d;
				}
			}

			// Return result
			return closest;
		}

		// This finds the line closest to the specified position
		public static Linedef NearestLinedefRange(ICollection<Linedef> selection, Vector2D pos, float maxrange)
		{
			Linedef closest = null;
			float distance = float.MaxValue;
			float maxrangesq = maxrange * maxrange;
			float d;

			// Go for all linedefs in selection
			foreach(Linedef l in selection)
			{
				// Calculate distance and check if closer than previous find
				d = l.SafeDistanceToSq(pos, true);
				if((d <= maxrangesq) && (d < distance))
				{
					// This one is closer
					closest = l;
					distance = d;
				}
			}
			
			// Return result
			return closest;
		}

		// This finds the vertex closest to the specified position
		public static Vertex NearestVertex(ICollection<Vertex> selection, Vector2D pos)
		{
			Vertex closest = null;
			float distance = float.MaxValue;
			float d;
			
			// Go for all vertices in selection
			foreach(Vertex v in selection)
			{
				// Calculate distance and check if closer than previous find
				d = v.DistanceToSq(pos);
				if(d < distance)
				{
					// This one is closer
					closest = v;
					distance = d;
				}
			}

			// Return result
			return closest;
		}

		// This finds the vertex closest to the specified position
		public static Vertex NearestVertexSquareRange(ICollection<Vertex> selection, Vector2D pos, float maxrange)
		{
			RectangleF range = RectangleF.FromLTRB(pos.x - maxrange, pos.y - maxrange, pos.x + maxrange, pos.y + maxrange);
			Vertex closest = null;
			float distance = float.MaxValue;
			float d;

			// Go for all vertices in selection
			foreach(Vertex v in selection)
			{
				// Within range?
				if((v.Position.x >= range.Left) && (v.Position.x <= range.Right))
				{
					if((v.Position.y >= range.Top) && (v.Position.y <= range.Bottom))
					{
						// Close than previous find?
						d = Math.Abs(v.Position.x - pos.x) + Math.Abs(v.Position.y - pos.y);
						if(d < distance)
						{
							// This one is closer
							closest = v;
							distance = d;
						}
					}
				}
			}

			// Return result
			return closest;
		}

		// This finds the thing closest to the specified position
		public static Thing NearestThingSquareRange(ICollection<Thing> selection, Vector2D pos, float maxrange)
		{
			RectangleF range = RectangleF.FromLTRB(pos.x - maxrange, pos.y - maxrange, pos.x + maxrange, pos.y + maxrange);
			Thing closest = null;
			float distance = float.MaxValue;
			float d;

			// Go for all vertices in selection
			foreach(Thing t in selection)
			{
				// Within range?
				if((t.Position.x >= (range.Left - t.Size)) && (t.Position.x <= (range.Right + t.Size)))
				{
					if((t.Position.y >= (range.Top - t.Size)) && (t.Position.y <= (range.Bottom + t.Size)))
					{
						// Close than previous find?
						d = Math.Abs(t.Position.x - pos.x) + Math.Abs(t.Position.y - pos.y);
						if(d < distance)
						{
							// This one is closer
							closest = t;
							distance = d;
						}
					}
				}
			}

			// Return result
			return closest;
		}
		
		#endregion

		#region ================== Tools

		// This makes a list of lines related to vertex selection
		// A line is unstable when one vertex is selected and the other isn't.
		public ICollection<Linedef> LinedefsFromSelectedVertices(bool includeunselected, bool includestable, bool includeunstable)
		{
			List<Linedef> list = new List<Linedef>((linedefs.Count / 2) + 1);
			
			// Go for all lines
			foreach(Linedef l in linedefs)
			{
				// Check if this is to be included
				if((includestable && (l.Start.Selected && l.End.Selected)) ||
				   (includeunstable && (l.Start.Selected ^ l.End.Selected)) ||
				   (includeunselected && (!l.Start.Selected && !l.End.Selected)) )
				{
					// Add to list
					list.Add(l);
				}
			}

			// Return result
			return list;
		}
		
		// This finds the line closest to the specified position
		public Linedef NearestLinedef(Vector2D pos) { return MapSet.NearestLinedef(linedefs, pos); }

		// This finds the line closest to the specified position
		public Linedef NearestLinedefRange(Vector2D pos, float maxrange) { return MapSet.NearestLinedefRange(linedefs, pos, maxrange); }

		// This finds the vertex closest to the specified position
		public Vertex NearestVertex(Vector2D pos) { return MapSet.NearestVertex(vertices, pos); }

		// This finds the vertex closest to the specified position
		public Vertex NearestVertexSquareRange(Vector2D pos, float maxrange) { return MapSet.NearestVertexSquareRange(vertices, pos, maxrange); }

		// This finds the thing closest to the specified position
		public Thing NearestThingSquareRange(Vector2D pos, float maxrange) { return MapSet.NearestThingSquareRange(things, pos, maxrange); }

		// This finds the closest unselected linedef that is not connected to the given vertex
		public Linedef NearestUnselectedUnreferencedLinedef(Vector2D pos, float maxrange, Vertex v, out float distance)
		{
			Linedef closest = null;
			distance = float.MaxValue;
			float maxrangesq = maxrange * maxrange;
			float d;

			// Go for all linedefs in selection
			foreach(Linedef l in linedefs)
			{
				// Calculate distance and check if closer than previous find
				d = l.SafeDistanceToSq(pos, true);
				if((d <= maxrangesq) && (d < distance))
				{
					// Check if not selected

					// Check if linedef is not connected to v
					if((l.Start != v) && (l.End != v))
					{
						// This one is closer
						closest = l;
						distance = d;
					}
				}
			}

			// Return result
			return closest;
		}
		
		// This performs sidedefs compression
		public void CompressSidedefs()
		{
			// TODO: Make this happen
		}
		
		// This removes unused vertices
		public void RemoveUnusedVertices()
		{
			LinkedListNode<Vertex> vn, vc;
			
			// Go for all vertices
			vn = vertices.First;
			while(vn != null)
			{
				vc = vn;
				vn = vc.Next;
				if(vc.Value.Linedefs.Count == 0) vertices.Remove(vc);
			}
		}
		
		#endregion
	}
}
