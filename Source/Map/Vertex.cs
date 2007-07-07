
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

#endregion

namespace CodeImp.DoomBuilder.Map
{
	internal class Vertex : IDisposable
	{
		#region ================== Constants
		
		public const int BUFFERVERTICES = 1;
		public const int RENDERPRIMITIVES = 1;
		
		#endregion

		#region ================== Variables

		// Map
		private MapSet map;
		
		// List items
		private LinkedListNode<Vertex> mainlistitem;

		// Position
		private Vector2D pos;

		// References
		private LinkedList<Linedef> linedefs;

		// Rendering
		private bool updateneeded;
		private int bufferindex;
		
		// Disposing
		private bool isdisposed = false;

		#endregion

		#region ================== Properties

		public MapSet Map { get { return map; } }
		public ICollection<Linedef> Linedefs { get { return linedefs; } }
		public Vector2D Position { get { return pos; } }
		public int BufferIndex { get { return bufferindex; } set { bufferindex = value; } }
		public bool IsDisposed { get { return isdisposed; } }

		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		public Vertex(MapSet map, LinkedListNode<Vertex> listitem, Vector2D pos)
		{
			// Initialize
			this.map = map;
			this.linedefs = new LinkedList<Linedef>();
			this.mainlistitem = listitem;
			this.pos = pos;
			this.updateneeded = true;
			
			// We have no destructor
			GC.SuppressFinalize(this);
		}

		// Diposer
		public void Dispose()
		{
			// Not already disposed?
			if(!isdisposed)
			{
				// Already set isdisposed so that changes can be prohibited
				isdisposed = true;
				
				// Remove from main list
				mainlistitem.List.Remove(mainlistitem);

				// Remove from rendering buffer
				if(map.IsRenderEnabled) map.VerticesBuffer.FreeItem(bufferindex);
				
				// Dispose the lines that are attached to this vertex
				// because a linedef cannot exist without 2 vertices.
				foreach(Linedef l in linedefs) l.Dispose();
				
				// Clean up
				linedefs = null;
				mainlistitem = null;
				map = null;
			}
		}

		#endregion

		#region ================== Management

		// This attaches a linedef and returns the listitem
		public LinkedListNode<Linedef> AttachLinedef(Linedef l) { return linedefs.AddLast(l); }

		// This detaches a linedef
		public void DetachLinedef(LinkedListNode<Linedef> l)
		{
			// Not disposing?
			if(!isdisposed)
			{
				// Remove linedef
				linedefs.Remove(l);

				// No more linedefs left?
				if(linedefs.Count == 0)
				{
					// This vertex is now useless, dispose it
					this.Dispose();
				}
			}
		}
		
		// This rounds the coordinates to integrals
		public void Round()
		{
			// Round to integrals
			pos.x = (float)Math.Round(pos.x);
			pos.y = (float)Math.Round(pos.y);
			updateneeded = true;
		}
		
		// This updates the vertex when changes have been made
		public void Update()
		{
			// Update if needed
			if(updateneeded)
			{
				// Updated
				updateneeded = false;
				
				// If rendering is enabled, then update to buffer as well
				if(map.IsRenderEnabled && map.IsUpdating) UpdateToBuffer();
			}
		}

		#endregion

		#region ================== Rendering

		// This writes the vertex to buffer
		public void UpdateToBuffer()
		{
			PTVertex v = new PTVertex();

			// Not up to date? Then do that first (Update will call this method again)
			if(updateneeded) { Update(); return; }

			// Seek to start of item
			map.VerticesBuffer.SeekToItem(bufferindex);

			// Write vertices to buffer
			v.x = pos.x;
			v.y = pos.y;
			v.z = 0f;
			v.c = Color.SlateBlue.ToArgb();
			map.VerticesBuffer.WriteItem(v);
		}
		
		#endregion
		
		#region ================== Methods

		// This returns the distance from given coordinates
		public float DistanceToSq(Vector2D p)
		{
			Vector2D delta = p - pos;
			return delta.GetLengthSq();
		}
		
		// This returns the distance from given coordinates
		public float DistanceTo(Vector2D p)
		{
			Vector2D delta = p - pos;
			return delta.GetLength();
		}

		// This finds the line closest to the specified position
		public Linedef NearestLinedef(Vector2D pos) { return MapSet.NearestLinedef(linedefs, pos); }

		#endregion

		#region ================== Changes

		// This moves the vertex
		public void Move(Vector2D newpos)
		{
			// Change position
			pos = newpos;
			updateneeded = true;
			
			// Let all lines know they need an update
			foreach(Linedef l in linedefs) l.NeedUpdate();
		}

		#endregion
	}
}
