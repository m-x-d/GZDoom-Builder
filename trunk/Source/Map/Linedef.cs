
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

		public const int BUFFERVERTICES = 4;
		public const int RENDERPRIMITIVES = 2;
		public static readonly byte[] EMPTY_ARGS = new byte[5];
		private const float NORMAL_LENGTH = 6f;
		
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
		private bool updateneeded;
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

		public MapSet Map { get { return map; } }
		public Vertex Start { get { return start; } }
		public Vertex End { get { return end; } }
		public Sidedef Front { get { return front; } }
		public Sidedef Back { get { return back; } }
		public bool IsDisposed { get { return isdisposed; } }
		public int Flags { get { return flags; } }
		public int Action { get { return action; } }
		public int Tag { get { return tag; } }

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
			this.updateneeded = true;
			
			// Attach to vertices
			startvertexlistitem = start.AttachLinedef(this);
			endvertexlistitem = end.AttachLinedef(this);
			
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
				updateneeded = true;
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
				updateneeded = true;
			}
			else throw new Exception("Linedef already has a back Sidedef.");
		}

		// This detaches a sidedef from the front
		public void DetachSidedef(Sidedef s)
		{
			// Sidedef is on the front?
			if(front == s)
			{
				// Remove sidedef reference
				front = null;
				updateneeded = true;
			}
			// Sidedef is on the back?
			else if(back == s)
			{
				// Remove sidedef reference
				back = null;
				updateneeded = true;
			}
			else throw new Exception("Specified Sidedef is not attached to this Linedef.");
		}
		
		// This updates the line when changes have been made
		public void Update()
		{
			Vector2D delta;
			
			// Update if needed
			if(updateneeded)
			{
				// Delta vector
				delta = end.Position - start.Position;

				// Recalculate values
				lengthsq = delta.GetLengthSq();
				length = (float)Math.Sqrt(lengthsq);
				//angle = delta.GetAngle();

				// Updated
				updateneeded = false;
			}
		}

		// This flags the line needs an update
		public void NeedUpdate()
		{
			updateneeded = true;
		}

		// This copies all properties to another line
		public void CopyPropertiesTo(Linedef l)
		{
			// Copy properties
			l.action = action;
			l.args = (byte[])args.Clone();
			l.flags = flags;
			l.tag = tag;
			l.updateneeded = true;
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
			this.updateneeded = true;
		}

		#endregion
	}
}
