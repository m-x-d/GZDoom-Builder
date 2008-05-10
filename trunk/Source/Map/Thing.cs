
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
using CodeImp.DoomBuilder.Config;
using System.Drawing;

#endregion

namespace CodeImp.DoomBuilder.Map
{
	public sealed class Thing
	{
		#region ================== Constants

		public const int NUM_ARGS = 5;
		public static readonly byte[] EMPTY_ARGS = new byte[NUM_ARGS];

		#endregion

		#region ================== Variables

		// Map
		private MapSet map;

		// Sector
		private Sector sector = null;

		// List items
		private LinkedListNode<Thing> mainlistitem;
		private LinkedListNode<Thing> sectorlistitem;
		
		// Properties
		private int type;
		private Vector3D pos;
		private float angle;
		private int flags;
		private int tag;
		private int action;
		private byte[] args;
		private int x, y, zoffset;

		// Configuration
		private float size;
		private PixelColor color;
		private float iconoffset;	// Arrow or dot coordinate offset on the texture
		
		// Selections
		private bool selected;
		private bool marked;

		// Additional fields
		private SortedList<string, object> fields;
		
		// Disposing
		private bool isdisposed = false;

		#endregion

		#region ================== Properties

		public MapSet Map { get { return map; } }
		public int Type { get { return type; } }
		public Vector3D Position { get { return pos; } }
		public bool IsDisposed { get { return isdisposed; } }
		public float Angle { get { return angle; } }
		public int AngleDeg { get { return (int)(angle * Angle2D.PIDEG); } }
		public int Flags { get { return flags; } }
		public int Action { get { return action; } }
		public byte[] Args { get { return args; } }
		public bool Selected { get { return selected; } set { selected = value; } }
		public bool Marked { get { return marked; } set { marked = value; } }
		public float Size { get { return size; } }
		public float IconOffset { get { return iconoffset; } }
		public PixelColor Color { get { return color; } }
		public int X { get { return x; } }
		public int Y { get { return y; } }
		public int ZOffset { get { return zoffset; } }
		public int Tag { get { return tag; } set { tag = value; if((tag < 0) || (tag > MapSet.HIGHEST_TAG)) throw new ArgumentOutOfRangeException("Tag", "Invalid tag number"); } }
		public Sector Sector { get { return sector; } }
		public SortedList<string, object> Fields { get { return fields; } }

		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		internal Thing(MapSet map, LinkedListNode<Thing> listitem)
		{
			// Initialize
			this.map = map;
			this.mainlistitem = listitem;
			
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

				// Remove from sector
				if(sector != null) sector.DetachThing(sectorlistitem);
				
				// Clean up
				mainlistitem = null;
				sectorlistitem = null;
				map = null;
				sector = null;
			}
		}

		#endregion

		#region ================== Management

		// This copies all properties to another thing
		public void CopyPropertiesTo(Thing t)
		{
			// Copy properties
			t.type = type;
			t.angle = angle;
			t.pos = pos;
			t.flags = flags;
			t.tag = tag;
			t.action = action;
			t.x = x;
			t.y = y;
			t.zoffset = zoffset;
			t.args = EMPTY_ARGS;
			t.size = size;
			t.color = color;
			t.iconoffset = iconoffset;
			args.CopyTo(t.args, 0);
			if(fields != null) t.MakeFields(fields);
			t.selected = selected;
		}
		
		// This determines which sector the thing is in and links it
		public void DetermineSector()
		{
			Sector newsector = null;
			Linedef nl;

			// Find the nearest linedef on the map
			nl = map.NearestLinedef(pos);
			if(nl != null)
			{
				// Check what side of line we are at
				if(nl.SideOfLine(pos) < 0f)
				{
					// Front side
					if(nl.Front != null) newsector = nl.Front.Sector;
				}
				else
				{
					// Back side
					if(nl.Back != null) newsector = nl.Back.Sector;
				}
			}

			// Currently attached to a sector and sector changes?
			if((sector != null) && (newsector != sector))
			{
				// Remove from current sector
				sector.DetachThing(sectorlistitem);
				sectorlistitem = null;
				sector = null;
			}

			// Attach to new sector?
			if((newsector != null) && (newsector != sector))
			{
				// Attach to new sector
				sector = newsector;
				sectorlistitem = sector.AttachThing(this);
			}
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
		
		#region ================== Changes

		// This moves the thing
		// NOTE: This does not update sector! (call DetermineSector)
		public void Move(Vector2D newpos)
		{
			// Change position
			this.x = (int)newpos.x;
			this.y = (int)newpos.y;
			this.pos = new Vector3D(newpos.x, newpos.y, zoffset);
		}

		// This moves the thing
		// NOTE: This does not update sector! (call DetermineSector)
		public void Move(int x, int y, int zoffset)
		{
			// Change position
			this.x = x;
			this.y = y;
			this.zoffset = zoffset;
			this.pos = new Vector3D(x, y, zoffset);
		}
		
		// This rotates the thing
		public void Rotate(float newangle)
		{
			// Change angle
			this.angle = newangle;
		}
		
		// This updates all properties
		// NOTE: This does not update sector! (call DetermineSector)
		public void Update(int type, int x, int y, int zoffset, float angle,
						   int flags, int tag, int action, byte[] args)
		{
			// Apply changes
			this.type = type;
			this.angle = angle;
			this.flags = flags;
			this.tag = tag;
			this.action = action;
			this.args = new byte[NUM_ARGS];
			args.CopyTo(this.args, 0);
			this.Move(x, y, zoffset);
		}
		
		// This updates the settings from configuration
		public void UpdateConfiguration()
		{
			ThingTypeInfo ti;
			
			// Lookup settings
			ti = General.Map.Config.GetThingInfo(type);

			// Apply size
			size = ti.Width;

			// Color valid?
			if((ti.Color >= 0) && (ti.Color < ColorCollection.NUM_THING_COLORS))
			{
				// Apply color
				color = General.Colors.Colors[ti.Color + ColorCollection.THING_COLORS_OFFSET];
			}
			else
			{
				// Unknown thing color
				color = General.Colors.Colors[ColorCollection.THING_COLORS_OFFSET];
			}
			
			// Apply icon offset (arrow or dot)
			if(ti.Arrow) iconoffset = 0f; else iconoffset = 0.25f;
		}
		
		#endregion

		#region ================== Methods

		// This snaps the vertex to the grid
		public void SnapToGrid()
		{
			// Calculate nearest grid coordinates
			this.Move(General.Map.Grid.SnappedToGrid((Vector2D)pos));
		}

		// This returns the distance from given coordinates
		public float DistanceToSq(Vector2D p)
		{
			Vector2D delta = p - (Vector2D)pos;
			return delta.GetLengthSq();
		}

		// This returns the distance from given coordinates
		public float DistanceTo(Vector2D p)
		{
			Vector2D delta = p - (Vector2D)pos;
			return delta.GetLength();
		}

		#endregion
	}
}
