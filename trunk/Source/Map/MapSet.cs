
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
using SlimDX.Direct3D9;
using CodeImp.DoomBuilder.Rendering;
using SlimDX;
using System.Drawing;
using CodeImp.DoomBuilder.Editing;
using CodeImp.DoomBuilder.IO;

#endregion

namespace CodeImp.DoomBuilder.Map
{
	public sealed class MapSet
	{
		#region ================== Constants

		// Highest tag
		public const int HIGHEST_TAG = 65534;

		// Stiching distance
		public const float STITCH_DISTANCE = 0.001f;
		
		#endregion

		#region ================== Variables

		// Sector indexing
		private List<int> indexholes;
		private int lastsectorindex;
		
		// Map structures
		private LinkedList<Vertex> vertices;
		private LinkedList<Linedef> linedefs;
		private LinkedList<Sidedef> sidedefs;
		private LinkedList<Sector> sectors;
		private LinkedList<Thing> things;

		// Optimization
		private long emptylongname;

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
		public long EmptyLongName { get { return emptylongname; } }
		
		#endregion

		#region ================== Constructor / Disposer

		// Constructor for new empty map
		internal MapSet()
		{
			// Initialize
			vertices = new LinkedList<Vertex>();
			linedefs = new LinkedList<Linedef>();
			sidedefs = new LinkedList<Sidedef>();
			sectors = new LinkedList<Sector>();
			things = new LinkedList<Thing>();
			indexholes = new List<int>();
			lastsectorindex = 0;
			emptylongname = Lump.MakeLongName("-");
			
			// We have no destructor
			GC.SuppressFinalize(this);
		}

		// Disposer
		internal void Dispose()
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
				indexholes = null;
				
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
				v.Clone = newset.CreateVertex(v.Position);
				v.CopyPropertiesTo(v.Clone);
			}

			// Go for all sectors
			foreach(Sector s in sectors)
			{
				// Make new sector
				s.Clone = newset.CreateSector();
				s.CopyPropertiesTo(s.Clone);
			}

			// Go for all linedefs
			foreach(Linedef l in linedefs)
			{
				// Make new linedef
				nl = newset.CreateLinedef(l.Start.Clone, l.End.Clone);
				l.CopyPropertiesTo(nl);

				// Linedef has a front side?
				if(l.Front != null)
				{
					// Make new sidedef
					nd = newset.CreateSidedef(nl, true, l.Front.Sector.Clone);
					l.Front.CopyPropertiesTo(nd);
				}

				// Linedef has a back side?
				if(l.Back != null)
				{
					// Make new sidedef
					nd = newset.CreateSidedef(nl, false, l.Back.Sector.Clone);
					l.Back.CopyPropertiesTo(nd);
				}
			}

			// Go for all things
			foreach(Thing t in things)
			{
				// Make new thing
				Thing nt = newset.CreateThing();
				t.CopyPropertiesTo(nt);
			}

			// Remove clone references
			foreach(Vertex v in vertices) v.Clone = null;
			foreach(Sector s in sectors) s.Clone = null;
			
			// Return the new set
			return newset;
		}
		
		// This creates a new vertex
		public Vertex CreateVertex(Vector2D pos)
		{
			LinkedListNode<Vertex> listitem;
			Vertex v;
			
			// Make a list item
			listitem = new LinkedListNode<Vertex>(null);

			// Make the vertex
			v = new Vertex(this, listitem, pos);
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
			int index;
			
			// Do we have any index holes we can use?
			if(indexholes.Count > 0)
			{
				// Take one of the index holes
				index = indexholes[indexholes.Count - 1];
				indexholes.RemoveAt(indexholes.Count - 1);
			}
			else
			{
				// Make a new index
				index = lastsectorindex++;
			}
			
			// Make the sector
			return CreateSector(index);
		}
		
		// This creates a new sector
		public Sector CreateSector(int index)
		{
			LinkedListNode<Sector> listitem;
			Sector s;

			// Make a list item
			listitem = new LinkedListNode<Sector>(null);

			// Make the sector
			s = new Sector(this, listitem, index);
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

		// This adds a sector index hole
		public void AddSectorIndexHole(int index)
		{
			indexholes.Add(index);
		}

		#endregion

		#region ================== Updating

		// This updates all structures if needed
		public void Update()
		{
			// Update all!
			Update(true, true);
		}

		// This updates all structures if needed
		public void Update(bool dolines, bool dosectors)
		{
			// Update all linedefs
			if(dolines) foreach(Linedef l in linedefs) l.UpdateCache();

			// Update all sectors
			if(dosectors) foreach(Sector s in sectors) s.UpdateCache();
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
		public ICollection<Vertex> GetSelectedVertices(bool selected)
		{
			List<Vertex> list = new List<Vertex>(things.Count >> 1);
			foreach(Vertex v in vertices) if(v.Selected == selected) list.Add(v);
			return list;
		}

		// Returns a collection of things that match a selected state
		public ICollection<Thing> GetSelectedThings(bool selected)
		{
			List<Thing> list = new List<Thing>(things.Count >> 1);
			foreach(Thing t in things) if(t.Selected == selected) list.Add(t);
			return list;
		}

		// Returns a collection of linedefs that match a selected state
		public ICollection<Linedef> GetSelectedLinedefs(bool selected)
		{
			List<Linedef> list = new List<Linedef>(linedefs.Count >> 1);
			foreach(Linedef l in linedefs) if(l.Selected == selected) list.Add(l);
			return list;
		}

		// Returns a collection of sectors that match a selected state
		public ICollection<Sector> GetSelectedSectors(bool selected)
		{
			List<Sector> list = new List<Sector>(sectors.Count >> 1);
			foreach(Sector s in sectors) if(s.Selected == selected) list.Add(s);
			return list;
		}

		#endregion

		#region ================== Marking

		// This clears all marks
		public void ClearAllMarks()
		{
			ClearMarkedVertices(false);
			ClearMarkedThings(false);
			ClearMarkedLinedefs(false);
			ClearMarkedSectors(false);
			ClearMarkedSidedefs(false);
		}

		// This clears marked vertices
		public void ClearMarkedVertices(bool mark)
		{
			foreach(Vertex v in vertices) v.Marked = mark;
		}

		// This clears marked things
		public void ClearMarkedThings(bool mark)
		{
			foreach(Thing t in things) t.Marked = mark;
		}

		// This clears marked linedefs
		public void ClearMarkedLinedefs(bool mark)
		{
			foreach(Linedef l in linedefs) l.Marked = mark;
		}

		// This clears marked sidedefs
		public void ClearMarkedSidedefs(bool mark)
		{
			foreach(Sidedef s in sidedefs) s.Marked = mark;
		}

		// This clears marked sectors
		public void ClearMarkedSectors(bool mark)
		{
			foreach(Sector s in sectors) s.Marked = mark;
		}

		// Returns a collection of vertices that match a marked state
		public List<Vertex> GetMarkedVertices(bool mark)
		{
			List<Vertex> list = new List<Vertex>(vertices.Count >> 1);
			foreach(Vertex v in vertices) if(v.Marked == mark) list.Add(v);
			return list;
		}

		// Returns a collection of things that match a marked state
		public List<Thing> GetMarkedThings(bool mark)
		{
			List<Thing> list = new List<Thing>(things.Count >> 1);
			foreach(Thing t in things) if(t.Marked == mark) list.Add(t);
			return list;
		}

		// Returns a collection of linedefs that match a marked state
		public List<Linedef> GetMarkedLinedefs(bool mark)
		{
			List<Linedef> list = new List<Linedef>(linedefs.Count >> 1);
			foreach(Linedef l in linedefs) if(l.Marked == mark) list.Add(l);
			return list;
		}

		// Returns a collection of sectors that match a marked state
		public List<Sector> GetMarkedSectors(bool mark)
		{
			List<Sector> list = new List<Sector>(sectors.Count >> 1);
			foreach(Sector s in sectors) if(s.Marked == mark) list.Add(s);
			return list;
		}

		// This creates a marking from selection
		public void MarkSelectedVertices(bool selected, bool mark)
		{
			foreach(Vertex v in vertices) if(v.Selected == selected) v.Marked = mark;
		}

		// This creates a marking from selection
		public void MarkSelectedLinedefs(bool selected, bool mark)
		{
			foreach(Linedef l in linedefs) if(l.Selected == selected) l.Marked = mark;
		}

		// This creates a marking from selection
		public void MarkSelectedSectors(bool selected, bool mark)
		{
			foreach(Sector s in sectors) if(s.Selected == selected) s.Marked = mark;
		}

		// This creates a marking from selection
		public void MarkSelectedThings(bool selected, bool mark)
		{
			foreach(Thing t in things) if(t.Selected == selected) t.Marked = mark;
		}

		/// <summary>
		/// This marks the front and back sidedefs on linedefs with the matching mark
		/// </summary>
		public void MarkSidedefsFromLinedefs(bool matchmark, bool setmark)
		{
			foreach(Linedef l in linedefs)
			{
				if(l.Marked == matchmark)
				{
					if(l.Front != null) l.Front.Marked = setmark;
					if(l.Back != null) l.Back.Marked = setmark;
				}
			}
		}

		/// <summary>
		/// Returns a collection of vertices that match a marked state on the linedefs
		/// </summary>
		public ICollection<Vertex> GetVerticesFromLinesMarks(bool mark)
		{
			List<Vertex> list = new List<Vertex>(vertices.Count >> 1);
			foreach(Vertex v in vertices)
			{
				foreach(Linedef l in v.Linedefs)
				{
					if(l.Marked == mark)
					{
						list.Add(v);
						break;
					}
				}
			}
			return list;
		}

		/// <summary>
		/// Returns a collection of vertices that match a marked state on the linedefs
		/// The difference with GetVerticesFromLinesMarks is that in this method
		/// ALL linedefs of a vertex must match the specified marked state.
		/// </summary>
		public ICollection<Vertex> GetVerticesFromAllLinesMarks(bool mark)
		{
			List<Vertex> list = new List<Vertex>(vertices.Count >> 1);
			foreach(Vertex v in vertices)
			{
				bool qualified = true;
				foreach(Linedef l in v.Linedefs)
				{
					if(l.Marked != mark)
					{
						qualified = false;
						break;
					}
				}
				if(qualified) list.Add(v);
			}
			return list;
		}

		/// <summary>
		/// Returns a collection of vertices that match a marked state on the linedefs
		/// </summary>
		public ICollection<Vertex> GetVerticesFromSectorsMarks(bool mark)
		{
			List<Vertex> list = new List<Vertex>(vertices.Count >> 1);
			foreach(Vertex v in vertices)
			{
				foreach(Linedef l in v.Linedefs)
				{
					if(((l.Front != null) && (l.Front.Sector.Marked == mark)) ||
						((l.Back != null) && (l.Back.Sector.Marked == mark)))
					{
						list.Add(v);
						break;
					}
				}
			}
			return list;
		}
		
		#endregion

		#region ================== Areas

		// This creates an area from vertices
		public static RectangleF CreateArea(ICollection<Vertex> verts)
		{
			float l = float.MaxValue;
			float t = float.MaxValue;
			float r = float.MinValue;
			float b = float.MinValue;

			// Go for all vertices
			foreach(Vertex v in verts)
			{
				// Adjust boundaries by vertices
				if(v.Position.x < l) l = v.Position.x;
				if(v.Position.x > r) r = v.Position.x;
				if(v.Position.y < t) t = v.Position.y;
				if(v.Position.y > b) b = v.Position.y;
			}

			// Return a rect
			return new RectangleF(l, t, r - l, b - t);
		}

		// This creates an area from linedefs
		public static RectangleF CreateArea(ICollection<Linedef> lines)
		{
			float l = float.MaxValue;
			float t = float.MaxValue;
			float r = float.MinValue;
			float b = float.MinValue;

			// Go for all linedefs
			foreach(Linedef ld in lines)
			{
				// Adjust boundaries by vertices
				if(ld.Start.Position.x < l) l = ld.Start.Position.x;
				if(ld.Start.Position.x > r) r = ld.Start.Position.x;
				if(ld.Start.Position.y < t) t = ld.Start.Position.y;
				if(ld.Start.Position.y > b) b = ld.Start.Position.y;
				if(ld.End.Position.x < l) l = ld.End.Position.x;
				if(ld.End.Position.x > r) r = ld.End.Position.x;
				if(ld.End.Position.y < t) t = ld.End.Position.y;
				if(ld.End.Position.y > b) b = ld.End.Position.y;
			}

			// Return a rect
			return new RectangleF(l, t, r - l, b - t);
		}
		
		// This filters lines by a square area
		public static ICollection<Linedef> FilterByArea(ICollection<Linedef> lines, ref RectangleF area)
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
		private static int GetCSFieldBits(Vertex v, ref RectangleF area)
		{
			int bits = 0;
			if(v.Position.y < area.Top) bits |= 0x01;
			if(v.Position.y > area.Bottom) bits |= 0x02;
			if(v.Position.x < area.Left) bits |= 0x04;
			if(v.Position.x > area.Right) bits |= 0x08;
			return bits;
		}

		// This filters vertices by a square area
		public static ICollection<Vertex> FilterByArea(ICollection<Vertex> verts, ref RectangleF area)
		{
			ICollection<Vertex> newverts = new List<Vertex>(verts.Count);

			// Go for all verts
			foreach(Vertex v in verts)
			{
				// Within rect?
				if((v.Position.x >= area.Left) &&
				   (v.Position.x <= area.Right) &&
				   (v.Position.y >= area.Top) &&
				   (v.Position.y <= area.Bottom))
				{
					// The vertex is in the area
					newverts.Add(v);
				}
			}

			// Return result
			return newverts;
		}

		#endregion

		#region ================== Stitching

		/// <summary>
		/// Stitches marked geometry with non-marked geometry. Returns the number of stitches made.
		/// </summary>
		public int StitchGeometry()
		{
			ICollection<Linedef> movinglines;
			ICollection<Linedef> fixedlines;
			ICollection<Vertex> nearbyfixedverts;
			ICollection<Vertex> movingverts;
			ICollection<Vertex> fixedverts;
			RectangleF editarea;
			int stitches = 0;
			int stitchundo;

			// Find vertices
			movingverts = General.Map.Map.GetMarkedVertices(true);
			fixedverts = General.Map.Map.GetMarkedVertices(false);
			
			// Find lines that moved during the drag
			movinglines = LinedefsFromMarkedVertices(false, true, true);

			// Find all non-moving lines
			fixedlines = LinedefsFromMarkedVertices(true, false, false);

			// Determine area in which we are editing
			editarea = MapSet.CreateArea(movinglines);
			editarea.Inflate(MapSet.STITCH_DISTANCE * 2.0f,
							 MapSet.STITCH_DISTANCE * 2.0f);

			// Join nearby vertices
			stitches += MapSet.JoinVertices(fixedverts, movingverts, true, MapSet.STITCH_DISTANCE);

			// Update cached values of lines because we need their length/angle
			Update(true, false);

			// Split moving lines with unselected vertices
			nearbyfixedverts = MapSet.FilterByArea(fixedverts, ref editarea);
			stitches += MapSet.SplitLinesByVertices(movinglines, nearbyfixedverts, MapSet.STITCH_DISTANCE, movinglines);

			// Split non-moving lines with selected vertices
			fixedlines = MapSet.FilterByArea(fixedlines, ref editarea);
			stitches += MapSet.SplitLinesByVertices(fixedlines, movingverts, MapSet.STITCH_DISTANCE, movinglines);

			// Remove looped linedefs
			stitches += MapSet.RemoveLoopedLinedefs(movinglines);

			// Join overlapping lines
			stitches += MapSet.JoinOverlappingLines(movinglines);

			return stitches;
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
								//while(lines.Remove(l1));
								//l1.Join(l2);
								while(lines.Remove(l2)) ;
								l2.Join(l1);
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
								//while(lines.Remove(l1));
								//l1.Join(l2);
								while(lines.Remove(l2)) ;
								l2.Join(l1);
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

		// This corrects lines that have a back sidedef but no front
		// sidedef by flipping them. Returns the number of flips made.
		public static int FlipBackwardLinedefs(ICollection<Linedef> lines)
		{
			int flipsdone = 0;
			
			// Examine all lines
			foreach(Linedef l in lines)
			{
				// Back side but no front side?
				if((l.Back != null) && (l.Front == null))
				{
					// Flip that linedef!
					l.FlipVertices();
					l.FlipSidedefs();
					flipsdone++;
				}
			}

			// Return result
			return flipsdone;
		}

		// This splits the given lines with the given vertices
		// All affected lines will be added to changedlines
		// Returns the number of splits made
		public static int SplitLinesByVertices(ICollection<Linedef> lines, ICollection<Vertex> verts, float splitdist, ICollection<Linedef> changedlines)
		{
			float splitdist2 = splitdist * splitdist;
			int splitsdone = 0;
			bool splitted;

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
							Vector2D deltastart = l.Start.Position - v.Position;
							Vector2D deltaend = l.End.Position - v.Position;
							if(((Math.Abs(deltastart.x) > 0.001f) ||
							    (Math.Abs(deltastart.y) > 0.001f)) &&
							   ((Math.Abs(deltaend.x) > 0.001f) ||
							    (Math.Abs(deltaend.y) > 0.001f)))
							{
								// Split line l with vertex v
								Linedef nl = l.Split(v);

								// Add the new line to the list
								lines.Add(nl);

								// Both lines must be updated because their new length
								// is relevant for next iterations!
								l.UpdateCache();
								nl.UpdateCache();

								// Add both lines to changedlines
								if(changedlines != null)
								{
									changedlines.Add(l);
									changedlines.Add(nl);
								}

								// Count the split
								splitsdone++;
								splitted = true;
								break;
							}
						}
					}

					// Will have to restart when splitted
					// TODO: If we make (linked) lists from the collections first,
					// we don't have to restart when splitted?
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

		// This finds the thing closest to the specified position
		public static Thing NearestThing(ICollection<Thing> selection, Vector2D pos)
		{
			Thing closest = null;
			float distance = float.MaxValue;
			float d;

			// Go for all things in selection
			foreach(Thing t in selection)
			{
				// Calculate distance and check if closer than previous find
				d = t.DistanceToSq(pos);
				if(d < distance)
				{
					// This one is closer
					closest = t;
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

		// This snaps all vertices to the map format accuracy
		public void SnapAllToAccuracy()
		{
			foreach(Vertex v in vertices) v.SnapToAccuracy();
		}
		
		// This returns the next unused tag number
		public int GetNewTag()
		{
			bool[] usedtags = new bool[HIGHEST_TAG+1];
			usedtags.Initialize();
			
			// Check all sectors
			foreach(Sector s in sectors) usedtags[s.Tag] = true;
			
			// Check all lines
			foreach(Linedef l in linedefs) usedtags[l.Tag] = true;

			// Check all things
			foreach(Thing t in things) usedtags[t.Tag] = true;
			
			// Now find the first unused index
			for(int i = 1; i <= HIGHEST_TAG; i++)
				if(usedtags[i] == false) return i;
			
			// Problem: all tags used!
			// Lets ignore this problem for now, who needs 65-thousand tags?!
			return 0;
		}
		
		// This returns the sector with the given index or null when the index is not in use
		// TODO: Speed this up by keeping sector references with indices in a dictionary?
		public Sector GetSectorByIndex(int index)
		{
			// Go for all sectors
			foreach(Sector s in sectors)
			{
				// Return sector when index matches
				if(s.Index == index) return s;
			}
			
			// Nothing found
			return null;
		}

		// This makes a list of lines related to marked vertices
		// A line is unstable when one vertex is marked and the other isn't.
		public ICollection<Linedef> LinedefsFromMarkedVertices(bool includeunselected, bool includestable, bool includeunstable)
		{
			List<Linedef> list = new List<Linedef>((linedefs.Count / 2) + 1);
			
			// Go for all lines
			foreach(Linedef l in linedefs)
			{
				// Check if this is to be included
				if((includestable && (l.Start.Marked && l.End.Marked)) ||
				   (includeunstable && (l.Start.Marked ^ l.End.Marked)) ||
				   (includeunselected && (!l.Start.Marked && !l.End.Marked)))
				{
					// Add to list
					list.Add(l);
				}
			}

			// Return result
			return list;
		}

		// This makes a list of unstable lines from the given vertices.
		// A line is unstable when one vertex is selected and the other isn't.
		public static ICollection<Linedef> UnstableLinedefsFromVertices(ICollection<Vertex> verts)
		{
			Dictionary<Linedef, Linedef> lines = new Dictionary<Linedef, Linedef>();

			// Go for all vertices
			foreach(Vertex v in verts)
			{
				// Go for all lines
				foreach(Linedef l in v.Linedefs)
				{
					// If the line exists in the list
					if(lines.ContainsKey(l))
					{
						// Remove it
						lines.Remove(l);
					}
					// Otherwise add it
					else
					{
						// Add the line
						lines.Add(l, l);
					}
				}
			}
			
			// Return result
			return new List<Linedef>(lines.Values);
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
