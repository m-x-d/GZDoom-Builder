
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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using CodeImp.DoomBuilder.Geometry;

namespace CodeImp.DoomBuilder.Map
{
	internal class Vertex : IDisposable
	{
		#region ================== Constants

		#endregion

		#region ================== Variables

		// Map
		private MapManager map;
		
		// List items
		private LinkedListNode<Vertex> mainlistitem;

		// Position
		private Vector2D pos;

		// References
		private LinkedList<Linedef> linedefs;

		// Disposing
		private bool isdisposed = false;

		#endregion

		#region ================== Properties

		public ICollection<Linedef> Linedefs { get { return linedefs; } }
		public Vector2D Position { get { return pos; } }
		public bool IsDisposed { get { return isdisposed; } }

		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		public Vertex(MapManager map, LinkedListNode<Vertex> listitem, Vector2D pos)
		{
			// Initialize
			this.map = map;
			this.linedefs = new LinkedList<Linedef>();
			this.mainlistitem = listitem;
			this.pos = pos;
			
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
		}
		
		#endregion

		#region ================== Mathematics

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

		#endregion

		#region ================== Tools

		// This finds the line closest to the specified position
		public Linedef NearestLinedef(Vector2D pos) { return MapManager.NearestLinedef(linedefs, pos); }

		#endregion
	}
}
