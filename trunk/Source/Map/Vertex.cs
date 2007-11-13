
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
	public sealed class Vertex : IDisposable
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
		private int x, y;
		private Vector2D pos;

		// References
		private LinkedList<Linedef> linedefs;

		// Selections
		private int selected;
		
		// Disposing
		private bool isdisposed = false;

		#endregion

		#region ================== Properties

		public MapSet Map { get { return map; } }
		public ICollection<Linedef> Linedefs { get { return linedefs; } }
		public Vector2D Position { get { return pos; } }
		public int X { get { return x; } }
		public int Y { get { return y; } }
		public bool IsDisposed { get { return isdisposed; } }
		public int Selected { get { return selected; } set { selected = value; } }

		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		public Vertex(MapSet map, LinkedListNode<Vertex> listitem, int x, int y)
		{
			// Initialize
			this.map = map;
			this.linedefs = new LinkedList<Linedef>();
			this.mainlistitem = listitem;
			this.pos = new Vector2D(x, y);
			this.x = x;
			this.y = y;
			
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

		// This moves the vertex
		public void Move(Vector2D newpos)
		{
			// Change position
			this.Move((int)Math.Round(newpos.x), (int)Math.Round(newpos.y));
		}

		// This moves the vertex
		public void Move(int newx, int newy)
		{
			// Change position
			x = newx;
			y = newy;
			pos = new Vector2D(newx, newy);
			
			// Let all lines know they need an update
			foreach(Linedef l in linedefs) l.NeedUpdate();
		}

		// This snaps the vertex to the grid
		public void SnapToGrid()
		{
			// Calculate nearest grid coordinates
			this.Move(General.Map.Grid.SnappedToGrid(pos));
		}
		
		#endregion
	}
}
