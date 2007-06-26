
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

#endregion

namespace CodeImp.DoomBuilder.Map
{
	internal class Linedef : IDisposable
	{
		#region ================== Constants

		public static readonly byte[] EMPTY_ARGS = new byte[5];
		
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
		private PTVertex[] lineverts;

		// Properties
		private int flags;
		private int action;
		private int tag;
		private byte[] args;

		// Disposing
		private bool isdisposed = false;

		#endregion

		#region ================== Properties

		public PTVertex[] LineVertices { get { return lineverts; } }
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
			this.lineverts = new PTVertex[4];

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
				if(front != null) front.Dispose();
				if(back != null) back.Dispose();
				
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
		public void AttachFront(Sidedef s)
		{
			// No sidedef here yet?
			if(front == null)
			{
				// Attach and recalculate
				front = s;
				Recalculate();
			}
			else throw new Exception("Linedef already has a front Sidedef.");
		}

		// This attaches a sidedef on the back
		public void AttachBack(Sidedef s)
		{
			// No sidedef here yet?
			if(back == null)
			{
				// Attach and recalculate
				back = s;
				Recalculate();
			}
			else throw new Exception("Linedef already has a back Sidedef.");
		}

		// This detaches a sidedef from the front
		public void DetachSidedef(Sidedef s) { if(front == s) front = null; else if(back == s) back = null; else throw new Exception("Specified Sidedef is not attached to this Linedef."); }
		
		// This recalculates cached values
		public void Recalculate()
		{
			Vector2D delta;
			Vector2D normal;
			int color;
			
			// Delta vector
			delta = end.Position - start.Position;
			
			// Recalculate values
			lengthsq = delta.GetLengthSq();
			length = (float)Math.Sqrt(lengthsq);
			normal = new Vector2D(delta.x / length, delta.y / length);
			//angle = delta.GetAngle();

			// Single sided?
			if((front == null) || (back == null))
			{
				// Line has an action?
				if(action != 0)
					color = Graphics.RGB(140, 255, 140);
				else
					color = Graphics.RGB(255, 255, 255);
			}
			else
			{
				// Line has an action?
				if(action != 0)
					color = Graphics.RGB(50, 140, 50);
				else
					color = Graphics.RGB(140, 140, 140);
			}
			
			// Create line normal
			lineverts[0].x = start.Position.x + delta.x * 0.5f;
			lineverts[0].y = start.Position.y + delta.y * 0.5f;
			lineverts[1].x = lineverts[0].x + normal.y * 100f;
			lineverts[1].y = lineverts[0].y - normal.x * 100f;
			lineverts[0].c = color;
			lineverts[1].c = color;
			
			// Create line vertices
			lineverts[2].x = start.Position.x;
			lineverts[2].y = start.Position.y;
			lineverts[3].x = end.Position.x;
			lineverts[3].y = end.Position.y;
			lineverts[2].c = color;
			lineverts[3].c = color;
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
		
		#region ================== Methods

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

		#region ================== Changes
		
		// This updates all properties
		public void Update(int flags, int tag, int action, byte[] args)
		{
			// Apply changes
			this.flags = flags;
			this.tag = tag;
			this.action = action;
			this.args = args;
		}

		#endregion
	}
}
