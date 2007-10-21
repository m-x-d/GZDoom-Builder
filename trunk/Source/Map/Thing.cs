
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
	internal class Thing : IDisposable
	{
		#region ================== Constants

		public static readonly byte[] EMPTY_ARGS = new byte[5];

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

		// Configuration
		private float size;
		private PixelColor color;
		private float iconoffset;
		
		// Selections
		private int selected;
		
		// Disposing
		private bool isdisposed = false;

		#endregion

		#region ================== Properties

		public MapSet Map { get { return map; } }
		public int Type { get { return type; } }
		public Vector3D Position { get { return pos; } }
		public bool IsDisposed { get { return isdisposed; } }
		public float Angle { get { return angle; } }
		public int Flags { get { return flags; } }
		public int Selected { get { return selected; } set { selected = value; } }
		public float Size { get { return size; } }
		public float IconOffset { get { return iconoffset; } }
		public PixelColor Color { get { return color; } }
		
		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		public Thing(MapSet map, LinkedListNode<Thing> listitem)
		{
			// Initialize
			this.map = map;
			this.mainlistitem = listitem;
			
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
			t.args = EMPTY_ARGS;
			args.CopyTo(t.args, 0);
		}
		
		// This determines which sector the thing is in and links it
		public void DetermineSector()
		{
			Sector newsector = null;
			Vertex nv;
			Linedef nl;

			// Find the nearest vertex on the map
			nv = map.NearestVertex(pos);
			if(nv != null)
			{
				// Find the nearest linedef on the vertex
				nl = nv.NearestLinedef(pos);
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
		
		#region ================== Changes

		// This moves the thing
		// NOTE: This does not update sector! (call DetermineSector)
		public void Move(Vector3D newpos)
		{
			// Change position
			pos = newpos;
		}
		
		// This rotates the thing
		public void Rotate(float newangle)
		{
			// Change angle
			angle = newangle;
		}
		
		// This updates all properties
		// NOTE: This does not update sector! (call DetermineSector)
		public void Update(int type, Vector3D pos, float angle,
						   int flags, int tag, int action, byte[] args)
		{
			// Apply changes
			this.type = type;
			this.pos = pos;
			this.angle = angle;
			this.flags = flags;
			this.tag = tag;
			this.action = action;
			this.args = args;
		}
		
		// This updates the settings from configuration
		public void UpdateConfiguration()
		{
			ThingTypeInfo ti;
			
			// Lookup settings
			ti = General.Map.Configuration.GetThingInfo(type);

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
	}
}
