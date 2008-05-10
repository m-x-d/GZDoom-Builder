
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

		public const float SIDE_POINT_DISTANCE = 0.001f;
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
		private RectangleF rect;
		
		// Properties
		private int flags;
		private int action;
		private int tag;
		private byte[] args;
		
		// Additional fields
		private SortedList<string, object> fields;
		
		// Selections
		private bool selected;
		private bool marked;
		
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
		public Line2D Line { get { return new Line2D(start.Position, end.Position); } }
		public int Flags { get { return flags; } set { flags = value; } }
		public int Action { get { return action; } set { action = value; } }
		public int Tag { get { return tag; } set { tag = value; if((tag < 0) || (tag > MapSet.HIGHEST_TAG)) throw new ArgumentOutOfRangeException("Tag", "Invalid tag number"); } }
		public bool Selected { get { return selected; } set { selected = value; } }
		public bool Marked { get { return marked; } set { marked = value; } }
		public float LengthSq { get { return lengthsq; } }
		public float Length { get { return length; } }
		public float LengthInv { get { return lengthinv; } }
		public float Angle { get { return angle; } }
		public int AngleDeg { get { return (int)(angle * Angle2D.PIDEG); } }
		public RectangleF Rect { get { return rect; } }
		public byte[] Args { get { return args; } }
		public SortedList<string, object> Fields { get { return fields; } }

		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		internal Linedef(MapSet map, LinkedListNode<Linedef> listitem, Vertex start, Vertex end)
		{
			// Initialize
			this.map = map;
			this.mainlistitem = listitem;
			this.start = start;
			this.end = end;
			this.updateneeded = true;
			this.args = new byte[NUM_ARGS];
			
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
				startvertexlistitem = null;
				endvertexlistitem = null;
				
				// Dispose sidedefs
				if(front != null) front.Dispose();
				if(back != null) back.Dispose();
				
				// Clean up
				mainlistitem = null;
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
			if(startvertexlistitem != null) start.DetachLinedef(startvertexlistitem);
			startvertexlistitem = null;
			start = v;
			startvertexlistitem = start.AttachLinedef(this);
			this.updateneeded = true;
		}

		// This sets new end vertex
		public void SetEndVertex(Vertex v)
		{
			// Change end
			if(endvertexlistitem != null) end.DetachLinedef(endvertexlistitem);
			endvertexlistitem = null;
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
			if(fields != null) l.MakeFields(fields);
			l.selected = selected;
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
			float l, t, r, b;
			
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
				l = Math.Min(start.Position.x, end.Position.x);
				t = Math.Min(start.Position.y, end.Position.y);
				r = Math.Max(start.Position.x, end.Position.x);
				b = Math.Max(start.Position.y, end.Position.y);
				rect = new RectangleF(l, t, r - l, b - t);
				
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

		#region ================== Fields

		// This makes new fields
		public void MakeFields()
		{
			if(fields != null) fields = new SortedList<string, object>();
		}

		// This makes fields from another list of fields
		public void MakeFields(SortedList<string, object> copyfrom)
		{
			if(fields != null) fields = new SortedList<string, object>();
			foreach(KeyValuePair<string, object> f in copyfrom) fields[f.Key] = f.Value;
		}

		#endregion
		
		#region ================== Methods

		// This flips the linedef's vertex attachments
		public void FlipVertices()
		{
			// Flip vertices
			Vertex v = start;
			start = end;
			end = v;

			// Flip tickets accordingly
			LinkedListNode<Linedef> vn = startvertexlistitem;
			startvertexlistitem = endvertexlistitem;
			endvertexlistitem = vn;

			// Update required (angle changed)
			NeedUpdate();
		}

		// This flips the sidedefs
		public void FlipSidedefs()
		{
			// Flip sidedefs
			Sidedef sd = front;
			front = back;
			back = sd;
		}
		
		// This returns a point for testing on one side
		public Vector2D GetSidePoint(bool front)
		{
			Vector2D n = new Vector2D();
			n.x = (end.Position.x - start.Position.x) * lengthinv * SIDE_POINT_DISTANCE;
			n.y = (end.Position.y - start.Position.y) * lengthinv * SIDE_POINT_DISTANCE;

			if(front)
			{
				n.x = -n.x;
				n.y = -n.y;
			}

			Vector2D p = new Vector2D();
			p.x = start.Position.x + (end.Position.x - start.Position.x) * 0.5f - n.y;
			p.y = start.Position.y + (end.Position.y - start.Position.y) * 0.5f + n.x;

			return p;
		}
		
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

		// This returns all points at which the line intersects with the grid
		public List<Vector2D> GetGridIntersections()
		{
			List<Vector2D> coords = new List<Vector2D>();
			Vector2D v = new Vector2D();
			float gx, gy, minx, maxx, miny, maxy;
			bool reversex, reversey;
			
			if(start.Position.x > end.Position.x)
			{
				minx = end.Position.x;
				maxx = start.Position.x;
				reversex = true;
			}
			else
			{
				minx = start.Position.x;
				maxx = end.Position.x;
				reversex = false;
			}

			if(start.Position.y > end.Position.y)
			{
				miny = end.Position.y;
				maxy = start.Position.y;
				reversey = true;
			}
			else
			{
				miny = start.Position.y;
				maxy = end.Position.y;
				reversey = false;
			}

			// Go for all vertical grid lines in between line start and end
			gx = General.Map.Grid.GetHigher(minx);
			if(gx < maxx)
			{
				for(; gx < maxx; gx += General.Map.Grid.GridSizeF)
				{
					// Add intersection point at this x coordinate
					float u = (gx - minx) / (maxx - minx);
					if(reversex) u = 1.0f - u;
					v.x = gx;
					v.y = start.Position.y + (end.Position.y - start.Position.y) * u;
					coords.Add(v);
				}
			}
			
			// Go for all horizontal grid lines in between line start and end
			gy = General.Map.Grid.GetHigher(miny);
			if(gy < maxy)
			{
				for(; gy < maxy; gy += General.Map.Grid.GridSizeF)
				{
					// Add intersection point at this y coordinate
					float u = (gy - miny) / (maxy - miny);
					if(reversey) u = 1.0f - u;
					v.x = start.Position.x + (end.Position.x - start.Position.x) * u;
					v.y = gy;
					coords.Add(v);
				}
			}
			
			// Profit
			return coords;
		}
		
		// This returns the closest coordinates ON the line
		public Vector2D NearestOnLine(Vector2D pos)
		{
			float u = Line2D.GetNearestOnLine(start.Position, end.Position, pos);
			if(u < 0f) u = 0f; else if(u > 1f) u = 1f;
			return Line2D.GetCoordinatesAt(start.Position, end.Position, u);
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
			nl.selected = this.selected;
			nl.marked = this.marked;
			
			// Copy front sidedef if exists
			if(front != null)
			{
				nsd = map.CreateSidedef(nl, true, front.Sector);
				front.CopyPropertiesTo(nsd);
				nsd.Marked = front.Marked;
			}

			// Copy back sidedef if exists
			if(back != null)
			{
				nsd = map.CreateSidedef(nl, false, back.Sector);
				back.CopyPropertiesTo(nsd);
				nsd.Marked = back.Marked;
			}

			// Return result
			return nl;
		}
		
		// This joins the line with another line
		// This line will be disposed
		public void Join(Linedef other)
		{
			Sector l1fs, l1bs, l2fs, l2bs;
			bool l1was2s, l2was2s;
			
			// Check which lines were 2 sided
			l1was2s = ((other.Front != null) && (other.Back != null));
			l2was2s = ((this.Front != null) && (this.Back != null));
			
			// Get sector references
			if(other.front != null) l1fs = other.front.Sector; else l1fs = null;
			if(other.back != null) l1bs = other.back.Sector; else l1bs = null;
			if(this.front != null) l2fs = this.front.Sector; else l2fs = null;
			if(this.back != null) l2bs = this.back.Sector; else l2bs = null;

			// This line has no sidedefs?
			if((l2fs == null) && (l2bs == null))
			{
				// We have no sidedefs, so we have no influence
				// Nothing to change on the other line
			}
			// Other line has no sidedefs?
			else if((l1fs == null) && (l1bs == null))
			{
				// The other has no sidedefs, so it has no influence
				// Copy my sidedefs to the other
				if(this.Start == other.Start)
				{
					JoinChangeSidedefs(other, true, front);
					JoinChangeSidedefs(other, false, back);
				}
				else
				{
					JoinChangeSidedefs(other, false, front);
					JoinChangeSidedefs(other, true, back);
				}
			}
			else
			{
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
			}
			
			// If either of the two lines was selected, keep the other selected
			if(this.selected) other.selected = true;
			if(this.marked) other.marked = true;
			
			// Apply single/double sided flags
			other.ApplySidedFlags();
			
			// Remove unneeded textures
			if(other.front != null) other.front.RemoveUnneededTextures(!(l1was2s && l2was2s));
			if(other.back != null) other.back.RemoveUnneededTextures(!(l1was2s && l2was2s));
			
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
				sd.Marked = newside.Marked;
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
