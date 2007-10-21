
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
	internal class MapSet : IDisposable
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

				// We may spend some time to clean things up here
				GC.Collect();
				
				// Done
				isdisposed = true;
			}
		}

		#endregion

		#region ================== Management

		// This makes a deep copy and returns a new MapSet
		public MapSet Clone()
		{
			Dictionary<Vertex, Vertex> vertexlink = new Dictionary<Vertex,Vertex>(vertices.Count);
			Dictionary<Linedef, Linedef> linedeflink = new Dictionary<Linedef, Linedef>(linedefs.Count);
			Dictionary<Sector, Sector> sectorlink = new Dictionary<Sector, Sector>(sectors.Count);
			
			// Create the map set
			MapSet newset = new MapSet();

			// Go for all vertices
			foreach(Vertex v in vertices)
			{
				// Make new vertex
				Vertex nv = newset.CreateVertex(v.X, v.Y);
				vertexlink.Add(v, nv);
			}

			// Go for all linedefs
			foreach(Linedef l in linedefs)
			{
				// Make new linedef
				Linedef nl = newset.CreateLinedef(vertexlink[l.Start], vertexlink[l.End]);
				linedeflink.Add(l, nl);
				
				// Copy properties
				l.CopyPropertiesTo(nl);

				// Recalculate
				l.Update();
			}

			// Go for all sectors
			foreach(Sector s in sectors)
			{
				// Make new sector
				Sector ns = newset.CreateSector();
				sectorlink.Add(s, ns);
				
				// Copy properties
				s.CopyPropertiesTo(ns);
			}

			// Go for all sidedefs
			foreach(Sidedef d in sidedefs)
			{
				// Make new sidedef
				Sidedef nd = newset.CreateSidedef(linedeflink[d.Line], d.IsFront, sectorlink[d.Sector]);
				
				// Copy properties
				d.CopyPropertiesTo(nd);
			}

			// Go for all things
			foreach(Thing t in things)
			{
				// Make new thing
				Thing nt = newset.CreateThing();

				// Copy properties
				t.CopyPropertiesTo(nt);
			}

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

		#region ================== Static Tools

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
		
		#endregion

		#region ================== Tools

		// This finds the line closest to the specified position
		public Linedef NearestLinedef(Vector2D pos) { return MapSet.NearestLinedef(linedefs, pos); }

		// This finds the line closest to the specified position
		public Linedef NearestLinedefRange(Vector2D pos, float maxrange) { return MapSet.NearestLinedefRange(linedefs, pos, maxrange); }

		// This finds the vertex closest to the specified position
		public Vertex NearestVertex(Vector2D pos) { return MapSet.NearestVertex(vertices, pos); }

		// This finds the vertex closest to the specified position
		public Vertex NearestVertexSquareRange(Vector2D pos, float maxrange) { return MapSet.NearestVertexSquareRange(vertices, pos, maxrange); }

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
