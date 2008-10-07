
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
using System.Drawing;
using CodeImp.DoomBuilder.Rendering;
using System.Collections.ObjectModel;

#endregion

namespace CodeImp.DoomBuilder.Map
{
	public sealed class Sector : MapElement
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
		private int index;
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
		private bool selected;
		private bool marked;

		// Cloning
		private Sector clone;
		
		// Triangulation
		private bool updateneeded;
		private bool triangulationneeded;
		private Triangulation triangles;
		private FlatVertex[] flatvertices;
		private ReadOnlyCollection<LabelPositionInfo> labels;
		
		#endregion

		#region ================== Properties

		public MapSet Map { get { return map; } }
		public ICollection<Sidedef> Sidedefs { get { return sidedefs; } }
		public ICollection<Thing> Things { get { return things; } }
		public int Index { get { return index; } }
		public int FloorHeight { get { return floorheight; } set { floorheight = value; } }
		public int CeilHeight { get { return ceilheight; } set { ceilheight = value; } }
		public string FloorTexture { get { return floortexname; } }
		public string CeilTexture { get { return ceiltexname; } }
		public long LongFloorTexture { get { return longfloortexname; } }
		public long LongCeilTexture { get { return longceiltexname; } }
		public int Effect { get { return effect; } set { effect = value; } }
		public int Tag { get { return tag; } set { tag = value; if((tag < 0) || (tag > MapSet.HIGHEST_TAG)) throw new ArgumentOutOfRangeException("Tag", "Invalid tag number"); } }
		public int Brightness { get { return brightness; } set { brightness = value; updateneeded = true; } }
		public bool Selected { get { return selected; } set { selected = value; } }
		public bool Marked { get { return marked; } set { marked = value; } }
		public bool UpdateNeeded { get { return updateneeded; } set { updateneeded |= value; triangulationneeded |= value; } }
		public Sector Clone { get { return clone; } set { clone = value; } }
		public Triangulation Triangles { get { return triangles; } }
		public FlatVertex[] FlatVertices { get { return flatvertices; } }
		public ReadOnlyCollection<LabelPositionInfo> Labels { get { return labels; } }
		
		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		internal Sector(MapSet map, LinkedListNode<Sector> listitem, int index)
		{
			// Initialize
			this.map = map;
			this.mainlistitem = listitem;
			this.sidedefs = new LinkedList<Sidedef>();
			this.things = new LinkedList<Thing>();
			this.index = index;
			this.floortexname = "-";
			this.ceiltexname = "-";
			this.longfloortexname = MapSet.EmptyLongName;
			this.longceiltexname = MapSet.EmptyLongName;
			this.triangulationneeded = true;
			
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

				// Register the index as free
				map.AddSectorIndexHole(index);
				
				// Dispose the sidedefs that are attached to this sector
				// because a sidedef cannot exist without reference to its sector.
				foreach(Sidedef sd in sidedefs) sd.Dispose();

				// Clean up
				mainlistitem = null;
				sidedefs = null;
				things = null;
				map = null;

				// Dispose base
				base.Dispose();
			}
		}

		#endregion

		#region ================== Management

		// This copies all properties to another sector
		public void CopyPropertiesTo(Sector s)
		{
			// Copy properties
			s.ceilheight = ceilheight;
			s.ceiltexname = ceiltexname;
			s.longceiltexname = longceiltexname;
			s.floorheight = floorheight;
			s.floortexname = floortexname;
			s.longfloortexname = longfloortexname;
			s.effect = effect;
			s.tag = tag;
			s.brightness = brightness;
			s.selected = selected;
			s.updateneeded = true;
			CopyFieldsTo(s);
		}
		
		// This attaches a sidedef and returns the listitem
		public LinkedListNode<Sidedef> AttachSidedef(Sidedef sd)
		{
			updateneeded = true;
			triangulationneeded = true;
			return sidedefs.AddLast(sd);
		}

		// This detaches a sidedef
		public void DetachSidedef(LinkedListNode<Sidedef> l)
		{
			// Not disposing?
			if(!isdisposed)
			{
				// Remove sidedef
				updateneeded = true;
				triangulationneeded = true;
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
		
		// This updates the sector when changes have been made
		public void UpdateCache()
		{
			// Update if needed
			if(updateneeded)
			{
				// Triangulate again?
				if(triangulationneeded || (triangles == null))
				{
					// Triangulate sector
					triangles = Triangulation.Create(this);
					
					// Make label positions
					labels = Array.AsReadOnly<LabelPositionInfo>(Tools.FindLabelPositions(this).ToArray());
				}
				
				// Brightness color (alpha is opaque)
				byte clampedbright = 0;
				if((brightness >= 0) && (brightness <= 255)) clampedbright = (byte)brightness;
				else if(brightness > 255) clampedbright = 255;
				PixelColor brightcolor = new PixelColor(255, clampedbright, clampedbright, clampedbright);
				int brightint = brightcolor.ToInt();
				
				// Make vertices
				flatvertices = new FlatVertex[triangles.Vertices.Count];
				for(int i = 0; i < triangles.Vertices.Count; i++)
				{
					flatvertices[i].x = triangles.Vertices[i].x;
					flatvertices[i].y = triangles.Vertices[i].y;
					flatvertices[i].z = 1.0f;
					flatvertices[i].c = brightint;
					flatvertices[i].u = triangles.Vertices[i].x;
					flatvertices[i].v = triangles.Vertices[i].y;
				}
				
				// Updated
				updateneeded = false;
			}
		}
		
		#endregion

		#region ================== Methods
		
		// This creates a bounding box rectangle
		// This requires the sector triangulation to be up-to-date!
		public RectangleF CreateBBox()
		{
			// Setup
			float left = float.MaxValue;
			float top = float.MaxValue;
			float right = float.MinValue;
			float bottom = float.MinValue;
			
			// Go for vertices
			foreach(Vector2D v in triangles.Vertices)
			{
				// Update rect
				if(v.x < left) left = v.x;
				if(v.y < top) top = v.y;
				if(v.x > right) right = v.x;
				if(v.y > bottom) bottom = v.y;
			}
			
			// Return rectangle
			return new RectangleF(left, top, right - left, bottom - top);
		}
		
		// This joins the sector with another sector
		// This sector will be disposed
		public void Join(Sector other)
		{
			// Any sidedefs to move?
			if(sidedefs.Count > 0)
			{
				// Change secter reference on my sidedefs
				// This automatically disposes this sector
				while(sidedefs != null)
					sidedefs.First.Value.ChangeSector(other);
			}
			else
			{
				// No sidedefs attached
				// Dispose manually
				this.Dispose();
			}
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
			updateneeded = true;
		}

		// This sets texture
		public void SetFloorTexture(string name)
		{
			floortexname = name;
			longfloortexname = Lump.MakeLongName(name);
			updateneeded = true;
		}

		// This sets texture
		public void SetCeilTexture(string name)
		{
			ceiltexname = name;
			longceiltexname = Lump.MakeLongName(name);
			updateneeded = true;
		}
		
		#endregion
	}
}
