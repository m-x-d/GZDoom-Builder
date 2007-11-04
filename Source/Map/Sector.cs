
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
using CodeImp.DoomBuilder.IO;

#endregion

namespace CodeImp.DoomBuilder.Map
{
	public class Sector : IDisposable
	{
		#region ================== Constants

		#endregion

		#region ================== Variables

		// Map
		private MapSet map;

		// List items
		private LinkedListNode<Sector> mainlistitem;
		
		// Sidedefs
		private LinkedList<Sidedef> sidedefs;

		// Things
		private LinkedList<Thing> things;
		
		// Properties
		private int floorheight;
		private int ceilheight;
		private string floortexname;
		private string ceiltexname;
		private long longfloortexname;
		private long longceiltexname;
		private int effect;
		private int tag;
		private int brightness;

		// Selections
		private int selected;

		// Disposing
		private bool isdisposed = false;

		#endregion

		#region ================== Properties

		public MapSet Map { get { return map; } }
		public ICollection<Sidedef> Sidedefs { get { return sidedefs; } }
		public ICollection<Thing> Things { get { return things; } }
		public bool IsDisposed { get { return isdisposed; } }
		public int FloorHeight { get { return floorheight; } }
		public int CeilHeight { get { return ceilheight; } }
		public string FloorTexture { get { return floortexname; } }
		public string CeilTexture { get { return ceiltexname; } }
		public long LongFloorTexture { get { return longfloortexname; } }
		public long LongCeilTexture { get { return longceiltexname; } }
		public int Effect { get { return effect; } }
		public int Tag { get { return tag; } }
		public int Brightness { get { return brightness; } }
		public int Selected { get { return selected; } set { selected = value; } }

		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		public Sector(MapSet map, LinkedListNode<Sector> listitem)
		{
			// Initialize
			this.map = map;
			this.mainlistitem = listitem;
			this.sidedefs = new LinkedList<Sidedef>();
			this.things = new LinkedList<Thing>();
			
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
				
				// Dispose the sidedefs that are attached to this sector
				// because a sidedef cannot exist without reference to its sector.
				foreach(Sidedef sd in sidedefs) sd.Dispose();

				// Clean up
				mainlistitem = null;
				sidedefs = null;
				things = null;
				map = null;
			}
		}

		#endregion

		#region ================== Management

		// This attaches a sidedef and returns the listitem
		public LinkedListNode<Sidedef> AttachSidedef(Sidedef sd) { return sidedefs.AddLast(sd); }

		// This detaches a sidedef
		public void DetachSidedef(LinkedListNode<Sidedef> l)
		{
			// Not disposing?
			if(!isdisposed)
			{
				// Remove sidedef
				sidedefs.Remove(l);

				// No more sidedefs left?
				if(sidedefs.Count == 0)
				{
					// This sector is now useless, dispose it
					this.Dispose();
				}
			}
		}

		// This attaches a thing and returns the listitem
		public LinkedListNode<Thing> AttachThing(Thing t) { return things.AddLast(t); }

		// This detaches a thing
		public void DetachThing(LinkedListNode<Thing> l) { if(!isdisposed) things.Remove(l); }
		
		// This copies all properties to another sector
		public void CopyPropertiesTo(Sector s)
		{
			// Copy properties
			s.ceilheight = ceilheight;
			s.SetCeilTexture(ceiltexname);
			s.floorheight = floorheight;
			s.SetFloorTexture(floortexname);
			s.effect = effect;
			s.tag = tag;
			s.brightness = brightness;
		}
		
		#endregion

		#region ================== Changes

		// This updates all properties
		public void Update(int hfloor, int hceil, string tfloor, string tceil, int effect, int tag, int brightness)
		{
			// Apply changes
			this.floorheight = hfloor;
			this.ceilheight = hceil;
			SetFloorTexture(tfloor);
			SetCeilTexture(tceil);
			this.effect = effect;
			this.tag = tag;
			this.brightness = brightness;
		}

		// This sets texture
		public void SetFloorTexture(string name)
		{
			floortexname = name;
			longfloortexname = Lump.MakeLongName(name);
		}

		// This sets texture
		public void SetCeilTexture(string name)
		{
			ceiltexname = name;
			longceiltexname = Lump.MakeLongName(name);
		}
		
		#endregion
	}
}
