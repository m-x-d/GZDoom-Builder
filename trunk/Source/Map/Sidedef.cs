
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

#endregion

namespace CodeImp.DoomBuilder.Map
{
	internal class Sidedef : IDisposable
	{
		#region ================== Constants

		#endregion

		#region ================== Variables

		// Map
		private MapSet map;

		// List items
		private LinkedListNode<Sidedef> mainlistitem;
		private LinkedListNode<Sidedef> sectorlistitem;

		// Owner
		private Linedef linedef;
		
		// Sector
		private Sector sector;

		// Properties
		private int offsetx;
		private int offsety;
		private string texnamehigh;
		private string texnamemid;
		private string texnamelow;
		
		// Disposing
		private bool isdisposed = false;

		#endregion

		#region ================== Properties

		public MapSet Map { get { return map; } }
		public bool IsFront { get { return (this == linedef.Front); } }
		public Linedef Line { get { return linedef; } }
		public Sidedef Other { get { if(this == linedef.Front) return linedef.Back; else return linedef.Front; } }
		public Sector Sector { get { return sector; } }
		public bool IsDisposed { get { return isdisposed; } }
		public int OffsetX { get { return offsetx; } }
		public int OffsetY { get { return offsety; } }
		public string HighTexture { get { return texnamehigh; } }
		public string MiddleTexture { get { return texnamemid; } }
		public string LowTexture { get { return texnamelow; } }
		
		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		public Sidedef(MapSet map, LinkedListNode<Sidedef> listitem, Linedef l, bool front, Sector s)
		{
			// Initialize
			this.map = map;
			this.mainlistitem = listitem;
			this.linedef = l;
			this.sector = s;
			
			// Attach to the linedef
			if(front) l.AttachFront(this); else l.AttachBack(this);
			
			// Attach to sector
			sectorlistitem = s.AttachSidedef(this);

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

				// Detach from linedef
				linedef.DetachSidedef(this);
				
				// Detach from sector
				sector.DetachSidedef(sectorlistitem);
				
				// Clean up
				mainlistitem = null;
				sectorlistitem = null;
				linedef = null;
				map = null;
				sector = null;
			}
		}

		#endregion

		#region ================== Management

		// This copies all properties to another sidedef
		public void CopyPropertiesTo(Sidedef s)
		{
			// Copy properties
			s.offsetx = offsetx;
			s.offsety = offsety;
			s.texnamehigh = texnamehigh;
			s.texnamelow = texnamelow;
			s.texnamemid = texnamemid;
		}
		
		#endregion

		#region ================== Changes

		// This updates all properties
		public void Update(int offsetx, int offsety, string thigh, string tmid, string tlow)
		{
			// Apply changes
			this.offsetx = offsetx;
			this.offsety = offsety;
			this.texnamehigh = thigh;
			this.texnamemid = tmid;
			this.texnamelow = tlow;
		}

		#endregion
	}
}
