
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
	internal class Linedef : IDisposable
	{
		#region ================== Constants

		#endregion

		#region ================== Variables

		// Map
		private MapSet map;

		// List items
		private LinkedListNode<Linedef> mainlistitem;
		private LinkedListNode<Linedef> startvertexlistitem;
		private LinkedListNode<Linedef> endvertexlistitem;
		
		// Vertices
		private Vertex start;
		private Vertex end;
		
		// Sidedefs
		private Sidedef front;
		private Sidedef back;

		// Cache
		private float lengthsq;
		private float length;
		//private float angle;

		// Properties
		private int flags;
		private int action;
		private int tag;
		private byte[] args;

		// Disposing
		private bool isdisposed = false;

		#endregion

		#region ================== Properties

		public Vertex Start { get { return start; } }
		public Vertex End { get { return end; } }
		public Sidedef Front { get { return front; } }
		public Sidedef Back { get { return back; } }
		public bool IsDisposed { get { return isdisposed; } }

		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		public Linedef(MapSet map, LinkedListNode<Linedef> listitem, Vertex start, Vertex end)
		{
			// Initialize
			this.map = map;
			this.mainlistitem = listitem;
			this.start = start;
			this.end = end;

			// Attach to vertices
			startvertexlistitem = start.AttachLinedef(this);
			endvertexlistitem = end.AttachLinedef(this);

			// Calculate values
			Recalculate();
			
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

				// Detach from vertices
				start.DetachLinedef(startvertexlistitem);
				end.DetachLinedef(endvertexlistitem);
				
				// Dispose sidedefs
				front.Dispose();
				back.Dispose();
				
				// Clean up
				mainlistitem = null;
				startvertexlistitem = null;
				endvertexlistitem = null;
				start = null;
				end = null;
				front = null;
				back = null;
				map = null;
			}
		}

		#endregion

		#region ================== Management

		// This attaches a sidedef on the front
		public void AttachFront(Sidedef s) { if(front == null) front = s; else throw new Exception("Linedef already has a front Sidedef."); }

		// This attaches a sidedef on the back
		public void AttachBack(Sidedef s) { if(back == null) back = s; else throw new Exception("Linedef already has a back Sidedef."); }

		// This detaches a sidedef from the front
		public void DetachSidedef(Sidedef s) { if(front == s) front = null; else if(back == s) back = null; else throw new Exception("Specified Sidedef is not attached to this Linedef."); }
		
		// This recalculates cached values
		public void Recalculate()
		{
			// Delta vector
			Vector2D delta = end.Position - start.Position;
			
			// Recalculate values
			lengthsq = delta.GetLengthSq();
			length = (float)Math.Sqrt(lengthsq);
			//angle = delta.GetAngle();
		}

		// This copies all properties to another line
		public void CopyPropertiesTo(Linedef l)
		{
			// Copy properties
			l.action = action;
			l.args = (byte[])args.Clone();
			l.flags = flags;
			l.tag = tag;
		}
		
		#endregion
		
		#region ================== Mathematics

		// This returns the shortest distance from given coordinates to line
		public float DistanceToSq(Vector2D p, bool bounded)
		{
			Vector2D v1 = start.Position;
			Vector2D v2 = end.Position;
			
			// Calculate intersection offset
			float u = ((p.x - v1.x) * (v2.x - v1.x) + (p.y - v1.y) * (v2.y - v1.y)) / lengthsq;

			// Limit intersection offset to the line
			if(bounded) if(u < 0f) u = 0f; else if(u > 1f) u = 1f;
			
			// Calculate intersection point
			Vector2D i = v1 + u * (v2 - v1);

			// Return distance between intersection and point
			// which is the shortest distance to the line
			float ldx = p.x - i.x;
			float ldy = p.y - i.y;
			return ldx * ldx + ldy * ldy;
		}

		// This returns the shortest distance from given coordinates to line
		public float DistanceTo(Vector2D p, bool bounded)
		{
			return (float)Math.Sqrt(DistanceToSq(p, bounded));
		}

		// This tests on which side of the line the given coordinates are
		// returns < 0 for front (right) side, > 0 for back (left) side and 0 if on the line
		public float SideOfLine(Vector2D p)
		{
			Vector2D v1 = start.Position;
			Vector2D v2 = end.Position;
			
			// Calculate and return side information
			return (p.y - v1.y) * (v2.x - v1.x) - (p.x - v1.x) * (v2.y - v1.y);
		}
		
		#endregion
	}
}
