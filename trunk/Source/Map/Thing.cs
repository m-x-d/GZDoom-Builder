
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
	internal class Thing : IDisposable
	{
		#region ================== Constants

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
		private Vector2D pos;
		private float angle;
		
		// Disposing
		private bool isdisposed = false;

		#endregion

		#region ================== Properties

		public int Type { get { return type; } }
		public Vector2D Position { get { return pos; } }
		public bool IsDisposed { get { return isdisposed; } }

		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		public Thing(MapSet map, LinkedListNode<Thing> listitem, int type, Vector2D pos)
		{
			// Initialize
			this.map = map;
			this.mainlistitem = listitem;
			this.type = type;
			this.pos = pos;
			
			// Determine current sector
			DetermineSector();
			
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

		// This copies all properties to another thing
		public void CopyPropertiesTo(Thing t)
		{
			// Copy properties
			t.type = type;
			t.angle = angle;
		}
		
		#endregion
	}
}
