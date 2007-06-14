using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace CodeImp.DoomBuilder.Map
{
	internal class Sector : IDisposable
	{
		#region ================== Constants

		#endregion

		#region ================== Variables

		// Map
		private MapManager map;

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
		private int special;
		private int tag;

		// Disposing
		private bool isdisposed = false;

		#endregion

		#region ================== Properties

		// Disposing
		public bool IsDisposed { get { return isdisposed; } }

		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		public Sector(MapManager map, LinkedListNode<Sector> listitem)
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
				
				// Determine new sector references on things
				foreach(Thing t in things) t.DetermineSector();

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
		
		#endregion
	}
}
