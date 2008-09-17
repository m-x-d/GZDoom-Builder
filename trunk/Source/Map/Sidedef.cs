
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
using CodeImp.DoomBuilder.Geometry;

#endregion

namespace CodeImp.DoomBuilder.Map
{
	public sealed class Sidedef : MapElement
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
		private long longtexnamehigh;
		private long longtexnamemid;
		private long longtexnamelow;

		// Selections
		private bool marked;

		#endregion

		#region ================== Properties

		public MapSet Map { get { return map; } }
		public bool IsFront { get { return (this == linedef.Front); } }
		public Linedef Line { get { return linedef; } }
		public Sidedef Other { get { if(this == linedef.Front) return linedef.Back; else return linedef.Front; } }
		public Sector Sector { get { return sector; } }
		public float Angle { get { if(IsFront) return linedef.Angle; else return Angle2D.Normalized(linedef.Angle + Angle2D.PI); } }
		public int OffsetX { get { return offsetx; } set { offsetx = value; } }
		public int OffsetY { get { return offsety; } set { offsety = value; } }
		public string HighTexture { get { return texnamehigh; } }
		public string MiddleTexture { get { return texnamemid; } }
		public string LowTexture { get { return texnamelow; } }
		public long LongHighTexture { get { return longtexnamehigh; } }
		public long LongMiddleTexture { get { return longtexnamemid; } }
		public long LongLowTexture { get { return longtexnamelow; } }
		public bool Marked { get { return marked; } set { marked = value; } }
		
		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		internal Sidedef(MapSet map, LinkedListNode<Sidedef> listitem, Linedef l, bool front, Sector s)
		{
			// Initialize
			this.map = map;
			this.mainlistitem = listitem;
			this.linedef = l;
			this.sector = s;
			this.texnamehigh = "-";
			this.texnamemid = "-";
			this.texnamelow = "-";
			this.longtexnamehigh = MapSet.EmptyLongName;
			this.longtexnamemid = MapSet.EmptyLongName;
			this.longtexnamelow = MapSet.EmptyLongName;
			
			// Attach to the linedef
			if(front) l.AttachFront(this); else l.AttachBack(this);
			
			// Attach to sector
			sectorlistitem = s.AttachSidedef(this);

			// We have no destructor
			GC.SuppressFinalize(this);
		}

		// Disposer
		public override void Dispose()
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

				// Dispose base
				base.Dispose();
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
			s.texnamemid = texnamemid;
			s.texnamelow = texnamelow;
			s.longtexnamehigh = longtexnamehigh;
			s.longtexnamemid = longtexnamemid;
			s.longtexnamelow = longtexnamelow;
			CopyFieldsTo(s);
		}
		
		// This copies textures to another sidedef
		// And possibly also the offsets
		public void AddTexturesTo(Sidedef s)
		{
			int copyoffsets = 0;

			// s cannot be null
			if(s == null) return;
			
			// Upper texture set?
			if((texnamehigh.Length > 0) && (texnamehigh[0] != '-'))
			{
				// Copy upper texture
				s.texnamehigh = texnamehigh;
				s.longtexnamehigh = longtexnamehigh;

				// Counts as a half coice for copying offsets
				copyoffsets += 1;
			}

			// Middle texture set?
			if((texnamemid.Length > 0) && (texnamemid[0] != '-'))
			{
				// Copy middle texture
				s.texnamemid = texnamemid;
				s.longtexnamemid = longtexnamemid;

				// Counts for copying offsets
				copyoffsets += 2;
			}

			// Lower texture set?
			if((texnamelow.Length > 0) && (texnamelow[0] != '-'))
			{
				// Copy middle texture
				s.texnamelow = texnamelow;
				s.longtexnamelow = longtexnamelow;

				// Counts as a half coice for copying offsets
				copyoffsets += 1;
			}

			// Copy offsets also?
			if(copyoffsets >= 2)
			{
				// Copy offsets
				s.offsetx = offsetx;
				s.offsety = offsety;
			}
		}
		
		#endregion

		#region ================== Methods

		// This removes textures that are not required
		public void RemoveUnneededTextures(bool removemiddle)
		{
			if(!HighRequired())
			{
				this.texnamehigh = "-";
				this.longtexnamehigh = MapSet.EmptyLongName;
			}

			if(!MiddleRequired() && removemiddle)
			{
				this.texnamemid = "-";
				this.longtexnamemid = MapSet.EmptyLongName;
			}
			
			if(!LowRequired())
			{
				this.texnamelow = "-";
				this.longtexnamelow = MapSet.EmptyLongName;
			}
		}
		
		// This checks if a texture is required
		public bool HighRequired()
		{
			// Doublesided?
			if(Other != null)
			{
				// Texture is required when ceiling of other side is lower
				return (Other.sector.CeilHeight < this.sector.CeilHeight);
			}
			else
			{
				return false;
			}
		}

		// This checks if a texture is required
		public bool MiddleRequired()
		{
			// Texture is required when the line is singlesided
			return (Other == null);
		}

		// This checks if a texture is required
		public bool LowRequired()
		{
			// Doublesided?
			if(Other != null)
			{
				// Texture is required when floor of other side is higher
				return (Other.sector.FloorHeight > this.sector.FloorHeight);
			}
			else
			{
				return false;
			}
		}
		
		#endregion

		#region ================== Changes

		// This updates all properties
		public void Update(int offsetx, int offsety, string thigh, string tmid, string tlow)
		{
			// Apply changes
			this.offsetx = offsetx;
			this.offsety = offsety;
			SetTextureHigh(thigh);
			SetTextureMid(tmid);
			SetTextureLow(tlow);
		}

		// This sets texture
		public void SetTextureHigh(string name)
		{
			texnamehigh = name;
			longtexnamehigh = Lump.MakeLongName(name);
		}

		// This sets texture
		public void SetTextureMid(string name)
		{
			texnamemid = name;
			longtexnamemid = Lump.MakeLongName(name);
		}

		// This sets texture
		public void SetTextureLow(string name)
		{
			texnamelow = name;
			longtexnamelow = Lump.MakeLongName(name);
		}

		// This changes sector
		public void ChangeSector(Sector newsector)
		{
			// Detach from sector
			sector.DetachSidedef(sectorlistitem);

			// Change sector
			sector = newsector;
			
			// Attach to sector
			sectorlistitem = sector.AttachSidedef(this);
		}
		
		#endregion
	}
}
