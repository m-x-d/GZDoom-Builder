
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

#endregion

namespace CodeImp.DoomBuilder.Map
{
	internal class MapSet : IDisposable
	{
		#region ================== Constants

		// Minimum size for primitives in buffers
		private const int MIN_PRIMITIVE_COUNT = 500;
		private const int VERTS_PER_LINEDEF = 2;

		#endregion

		#region ================== Variables

		// Map structures
		private LinkedList<Vertex> vertices;
		private LinkedList<Linedef> linedefs;
		private LinkedList<Sidedef> sidedefs;
		private LinkedList<Sector> sectors;
		private LinkedList<Thing> things;

		// Rendering
		private bool renderenabled = false;
		private int updating = 0;
		private ManagedVertexBuffer verts;
		private ManagedVertexBuffer lines;
		
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
		public bool IsRenderEnabled { get { return renderenabled; } }
		public bool IsUpdating { get { return updating > 0; } }
		public ManagedVertexBuffer VerticesBuffer { get { return verts; } }
		public ManagedVertexBuffer LinedefsBuffer { get { return lines; } }
		
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

				// No more rendering
				DisableRendering();
				updating = 0;
				
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
				Vertex nv = newset.CreateVertex(v.Position);
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

			// Add vertex to rendering bufer
			if(renderenabled) v.BufferIndex = verts.AddItem();
			
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

			// Add linedef to rendering bufer
			if(renderenabled) l.BufferIndex = lines.AddItem();
			
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

		#region ================== Resources

		// This reloads vertices into rendering buffer
		private void ReloadVertices()
		{
			// Update all vertices to buffer
			foreach(Vertex v in vertices) v.UpdateToBuffer();
		}

		// This reloads linedefs into rendering buffer
		private void ReloadLinedefs()
		{
			// Update all linedefs to buffer
			foreach(Linedef l in linedefs) l.UpdateToBuffer();
		}

		#endregion
		
		#region ================== Rendering

		// This enables rendering of map structures
		public void EnableRendering()
		{
			// Not already enabled?
			if(!renderenabled)
			{
				// Enable rendering
				renderenabled = true;
				
				// Create buffers
				verts = new ManagedVertexBuffer(PTVertex.Stride * Vertex.BUFFERVERTICES, vertices.Count);
				lines = new ManagedVertexBuffer(PTVertex.Stride * Linedef.BUFFERVERTICES, linedefs.Count);

				// Go for all vertices to add to the buffer
				foreach(Vertex v in vertices) v.BufferIndex = verts.AddItem();

				// Go for all linedefs to add to the buffer
				foreach(Linedef l in linedefs) l.BufferIndex = lines.AddItem();

				// Attach events
				verts.ReloadResources += new ReloadResourceDelegate(ReloadVertices);
				lines.ReloadResources += new ReloadResourceDelegate(ReloadLinedefs);
			}
		}

		// This disables rendering of map structures
		public void DisableRendering()
		{
			// Disable rendering
			renderenabled = false;

			// Stop any updating
			while(updating > 0) EndUpdate();
			
			// Trash buffers
			if(verts != null) verts.Dispose();
			if(lines != null) lines.Dispose();
			verts = null;
			lines = null;
		}
		
		// This locks the buffers for updates
		public void BeginUpdate()
		{
			// Not already updating
			if(updating == 0)
			{
				// Lock buffers for updating
				verts.BeginUpdate();
				lines.BeginUpdate();
			}

			// Now updating
			updating++;
		}
		
		// This unlocks the buffers
		public void EndUpdate()
		{
			// Updating?
			if(updating > 0)
			{
				// No longer updating
				updating--;

				// Done updating?
				if(updating == 0)
				{
					// Unlock buffers
					verts.EndUpdate();
					lines.EndUpdate();
				}
			}
		}
		
		// This updates all structures if needed
		public void Update()
		{
			// Updating begins now
			BeginUpdate();

			// Update all vertices
			foreach(Vertex v in vertices) v.Update();

			// Update all linedefs
			foreach(Linedef l in linedefs) l.Update();

			// Updating has finished
			EndUpdate();
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
				d = l.DistanceToSq(pos, true);
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
		
		#endregion

		#region ================== Tools

		// This finds the line closest to the specified position
		public Linedef NearestLinedef(Vector2D pos) { return MapSet.NearestLinedef(linedefs, pos); }

		// This finds the vertex closest to the specified position
		public Vertex NearestVertex(Vector2D pos) { return MapSet.NearestVertex(vertices, pos); }

		#endregion
	}
}
