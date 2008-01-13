
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
	public sealed class Linedef
	{
		#region ================== Constants

		public const int NUM_ARGS = 5;
		public static readonly byte[] EMPTY_ARGS = new byte[NUM_ARGS];
		
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
		private float lengthsqinv;
		private float length;
		private float lengthinv;
		private float angle;
		private Rectangle rect;
		
		// Properties
		private int flags;
		private int action;
		private int tag;
		private byte[] args;
		
		// Selections
		private bool selected;
		
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
		public int Flags { get { return flags; } set { flags = value; } }
		public int Action { get { return action; } set { action = value; } }
		public int Tag { get { return tag; } set { tag = value; if((tag < 0) || (tag > MapSet.HIGHEST_TAG)) throw new ArgumentOutOfRangeException("Tag", "Invalid tag number"); } }
		public bool Selected { get { return selected; } set { selected = value; } }
		public float LengthSq { get { return lengthsq; } }
		public float Length { get { return length; } }
		public float LengthInv { get { return lengthinv; } }
		public float Angle { get { return angle; } }
		public int AngleDeg { get { return (int)(angle * Angle2D.PIDEG); } }
		public Rectangle Rect { get { return rect; } }
		public byte[] Args { get { return args; } }

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

		// Disposer
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

		// This sets new start vertex
		public void SetStartVertex(Vertex v)
		{
			// Change start
			start.DetachLinedef(startvertexlistitem);
			start = v;
			startvertexlistitem = start.AttachLinedef(this);
			this.updateneeded = true;
		}

		// This sets new end vertex
		public void SetEndVertex(Vertex v)
		{
			// Change end
			end.DetachLinedef(endvertexlistitem);
			end = v;
			endvertexlistitem = end.AttachLinedef(this);
			this.updateneeded = true;
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
		public void UpdateCache()
		{
			Vector2D delta;
			int l, t, r, b;
			
			// Update if needed
			if(updateneeded)
			{
				// Delta vector
				delta = end.Position - start.Position;

				// Recalculate values
				lengthsq = delta.GetLengthSq();
				length = (float)Math.Sqrt(lengthsq);
				if(length > 0f) lengthinv = 1f / length; else lengthinv = 1f / 0.0000000001f;
				if(lengthsq > 0f) lengthsqinv = 1f / lengthsq; else lengthsqinv = 1f / 0.0000000001f;
				angle = delta.GetAngle();
				l = Math.Min(start.X, end.X);
				t = Math.Min(start.Y, end.Y);
				r = Math.Max(start.X, end.X);
				b = Math.Max(start.Y, end.Y);
				rect = new Rectangle(l, t, r - l, b - t);
				
				// Updated
				updateneeded = false;
			}
		}

		// This flags the line needs an update because it moved
		public void NeedUpdate()
		{
			// Update this line
			updateneeded = true;

			// Update sectors as well
			if(front != null) front.Sector.UpdateNeeded = true;
			if(back != null) back.Sector.UpdateNeeded = true;
		}

		#endregion
		
		#region ================== Methods
		
		// This applies single/double sided flags
		public void ApplySidedFlags()
		{
			// Doublesided?
			if((front != null) && (back != null))
			{
				// Apply or remove flags for doublesided line
				flags &= ~General.Map.Config.SingleSidedFlags;
				flags |= General.Map.Config.DoubleSidedFlags;
			}
			else
			{
				// Apply or remove flags for singlesided line
				flags &= ~General.Map.Config.DoubleSidedFlags;
				flags |= General.Map.Config.SingleSidedFlags;
			}
		}

		// This returns the shortest distance from given coordinates to line
		public float SafeDistanceToSq(Vector2D p, bool bounded)
		{
			Vector2D v1 = start.Position;
			Vector2D v2 = end.Position;

			// Calculate intersection offset
			float u = ((p.x - v1.x) * (v2.x - v1.x) + (p.y - v1.y) * (v2.y - v1.y)) * lengthsqinv;

			// Limit intersection offset to the line
			if(bounded) if(u < lengthinv) u = lengthinv; else if(u > (1f - lengthinv)) u = 1f - lengthinv;

			// Calculate intersection point
			Vector2D i = v1 + u * (v2 - v1);

			// Return distance between intersection and point
			// which is the shortest distance to the line
			float ldx = p.x - i.x;
			float ldy = p.y - i.y;
			return ldx * ldx + ldy * ldy;
		}

		// This returns the shortest distance from given coordinates to line
		public float SafeDistanceTo(Vector2D p, bool bounded)
		{
			return (float)Math.Sqrt(SafeDistanceToSq(p, bounded));
		}

		// This returns the shortest distance from given coordinates to line
		public float DistanceToSq(Vector2D p, bool bounded)
		{
			Vector2D v1 = start.Position;
			Vector2D v2 = end.Position;
			
			// Calculate intersection offset
			float u = ((p.x - v1.x) * (v2.x - v1.x) + (p.y - v1.y) * (v2.y - v1.y)) * lengthsqinv;

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

		// This splits this line by vertex v
		// Returns the new line resulting from the split
		public Linedef Split(Vertex v)
		{
			Linedef nl;
			Sidedef nsd;

			// Copy linedef and change vertices
			nl = map.CreateLinedef(v, end);
			CopyPropertiesTo(nl);
			SetEndVertex(v);

			// Copy front sidedef if exists
			if(front != null)
			{
				nsd = map.CreateSidedef(nl, true, front.Sector);
				front.CopyPropertiesTo(nsd);
			}

			// Copy back sidedef if exists
			if(back != null)
			{
				nsd = map.CreateSidedef(nl, false, back.Sector);
				back.CopyPropertiesTo(nsd);
			}

			// Return result
			return nl;
		}
		
		// This joins the line with another line
		// This line will be disposed
		public void Join(Linedef other)
		{
			Sector l1fs, l1bs, l2fs, l2bs;
			
			// Get sector references
			if(other.front != null) l1fs = other.front.Sector; else l1fs = null;
			if(other.back != null) l1bs = other.back.Sector; else l1bs = null;
			if(this.front != null) l2fs = this.front.Sector; else l2fs = null;
			if(this.back != null) l2bs = this.back.Sector; else l2bs = null;

			// Compare front sectors
			if(l1fs == l2fs)
			{
				// Copy textures
				if(other.front != null) other.front.AddTexturesTo(this.back);
				if(this.front != null) this.front.AddTexturesTo(other.back);

				// Change sidedefs
				JoinChangeSidedefs(other, true, back);
			}
			// Compare back sectors
			else if(l1bs == l2bs)
			{
				// Copy textures
				if(other.back != null) other.back.AddTexturesTo(this.front);
				if(this.back != null) this.back.AddTexturesTo(other.front);
				
				// Change sidedefs
				JoinChangeSidedefs(other, false, front);
			}
			// Compare front and back
			else if(l1fs == l2bs)
			{
				// Copy textures
				if(other.front != null) other.front.AddTexturesTo(this.front);
				if(this.back != null) this.back.AddTexturesTo(other.back);

				// Change sidedefs
				JoinChangeSidedefs(other, true, front);
			}
			// Compare back and front
			else if(l1bs == l2fs)
			{
				// Copy textures
				if(other.back != null) other.back.AddTexturesTo(this.back);
				if(this.front != null) this.front.AddTexturesTo(other.front);

				// Change sidedefs
				JoinChangeSidedefs(other, false, back);
			}
			else
			{
				// Other line single sided?
				if(other.back == null)
				{
					// This line with its back to the other?
					if(this.start == other.end)
					{
						// Copy textures
						if(other.back != null) other.back.AddTexturesTo(this.front);
						if(this.back != null) this.back.AddTexturesTo(other.front);

						// Change sidedefs
						JoinChangeSidedefs(other, false, front);
					}
					else
					{
						// Copy textures
						if(other.back != null) other.back.AddTexturesTo(this.back);
						if(this.front != null) this.front.AddTexturesTo(other.front);

						// Change sidedefs
						JoinChangeSidedefs(other, false, back);
					}
				}
				// This line single sided?
				if(this.back == null)
				{
					// Other line with its back to this?
					if(other.start == this.end)
					{
						// Copy textures
						if(other.back != null) other.back.AddTexturesTo(this.front);
						if(this.back != null) this.back.AddTexturesTo(other.front);

						// Change sidedefs
						JoinChangeSidedefs(other, false, front);
					}
					else
					{
						// Copy textures
						if(other.front != null) other.front.AddTexturesTo(this.front);
						if(this.back != null) this.back.AddTexturesTo(other.back);

						// Change sidedefs
						JoinChangeSidedefs(other, true, front);
					}
				}
				else
				{
					// This line with its back to the other?
					if(this.start == other.end)
					{
						// Copy textures
						if(other.back != null) other.back.AddTexturesTo(this.front);
						if(this.back != null) this.back.AddTexturesTo(other.front);

						// Change sidedefs
						JoinChangeSidedefs(other, false, front);
					}
					else
					{
						// Copy textures
						if(other.back != null) other.back.AddTexturesTo(this.back);
						if(this.front != null) this.front.AddTexturesTo(other.front);

						// Change sidedefs
						JoinChangeSidedefs(other, false, back);
					}
				}
			}

			// If either of the two lines was selected, keep the other selected
			if(this.selected) other.selected = true;

			// Apply single/double sided flags
			other.ApplySidedFlags();
			
			// I got killed by the other.
			this.Dispose();
		}
		
		// This changes sidedefs (used for joining lines)
		private void JoinChangeSidedefs(Linedef other, bool front, Sidedef newside)
		{
			Sidedef sd;
			
			// Change sidedefs
			if(front)
			{
				if(other.front != null) other.front.Dispose();
			}
			else
			{
				if(other.back != null) other.back.Dispose();
			}
			
			if(newside != null)
			{
				sd = map.CreateSidedef(other, front, newside.Sector);
				newside.CopyPropertiesTo(sd);
			}
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
			this.args = new byte[NUM_ARGS];
			args.CopyTo(this.args, 0);
			this.updateneeded = true;
		}

		#endregion
	}
}
