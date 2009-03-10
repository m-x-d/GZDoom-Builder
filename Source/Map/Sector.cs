
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
using SlimDX.Direct3D9;
using SlimDX;

#endregion

namespace CodeImp.DoomBuilder.Map
{
	public sealed class Sector : SelectableElement, ID3DResource
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
		
		// Properties
		private int fixedindex;
		private int floorheight;
		private int ceilheight;
		private string floortexname;
		private string ceiltexname;
		private long longfloortexname;
		private long longceiltexname;
		private int effect;
		private int tag;
		private int brightness;

		// Cloning
		private Sector clone;
		private int serializedindex;
		
		// Triangulation
		private bool updateneeded;
		private bool triangulationneeded;
		private RectangleF bbox;
		private Triangulation triangles;
		private FlatVertex[] flatvertices;
		private ReadOnlyCollection<LabelPositionInfo> labels;
		private VertexBuffer flatceilingbuffer;
		private VertexBuffer flatfloorbuffer;
		
		#endregion

		#region ================== Properties

		public MapSet Map { get { return map; } }
		public ICollection<Sidedef> Sidedefs { get { return sidedefs; } }

		/// <summary>
		/// An unique index that does not change when other sectors are removed.
		/// </summary>
		public int FixedIndex { get { return fixedindex; } }
		public int FloorHeight { get { return floorheight; } set { floorheight = value; } }
		public int CeilHeight { get { return ceilheight; } set { ceilheight = value; } }
		public string FloorTexture { get { return floortexname; } }
		public string CeilTexture { get { return ceiltexname; } }
		public long LongFloorTexture { get { return longfloortexname; } }
		public long LongCeilTexture { get { return longceiltexname; } }
		public int Effect { get { return effect; } set { effect = value; } }
		public int Tag { get { return tag; } set { tag = value; if((tag < 0) || (tag > MapSet.HIGHEST_TAG)) throw new ArgumentOutOfRangeException("Tag", "Invalid tag number"); } }
		public int Brightness { get { return brightness; } set { brightness = value; updateneeded = true; } }
		public bool UpdateNeeded { get { return updateneeded; } set { updateneeded |= value; triangulationneeded |= value; } }
		public RectangleF BBox { get { return bbox; } }
		internal Sector Clone { get { return clone; } set { clone = value; } }
		internal int SerializedIndex { get { return serializedindex; } set { serializedindex = value; } }
		public Triangulation Triangles { get { return triangles; } }
		public FlatVertex[] FlatVertices { get { return flatvertices; } }
		internal VertexBuffer FlatCeilingBuffer { get { return flatceilingbuffer; } }
		internal VertexBuffer FlatFloorBuffer { get { return flatfloorbuffer; } }
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
			this.fixedindex = index;
			this.floortexname = "-";
			this.ceiltexname = "-";
			this.longfloortexname = MapSet.EmptyLongName;
			this.longceiltexname = MapSet.EmptyLongName;
			this.triangulationneeded = true;

			General.Map.Graphics.RegisterResource(this);

			// We have no destructor
			GC.SuppressFinalize(this);
		}

		// Constructor
		internal Sector(MapSet map, LinkedListNode<Sector> listitem, IReadWriteStream stream)
		{
			// Initialize
			this.map = map;
			this.mainlistitem = listitem;
			this.sidedefs = new LinkedList<Sidedef>();
			this.triangulationneeded = true;

			ReadWrite(stream);
			
			General.Map.Graphics.RegisterResource(this);

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
				map.AddSectorIndexHole(fixedindex);
				
				// Dispose the sidedefs that are attached to this sector
				// because a sidedef cannot exist without reference to its sector.
				foreach(Sidedef sd in sidedefs) sd.Dispose();

				General.Map.Graphics.UnregisterResource(this);

				// Clean up
				if(flatceilingbuffer != null) flatceilingbuffer.Dispose();
				if(flatfloorbuffer != null) flatfloorbuffer.Dispose();
				flatceilingbuffer = null;
				flatfloorbuffer = null;
				mainlistitem = null;
				sidedefs = null;
				map = null;

				// Dispose base
				base.Dispose();
			}
		}

		#endregion

		#region ================== Management

		// Serialize / deserialize
		internal void ReadWrite(IReadWriteStream s)
		{
			base.ReadWrite(s);
			
			s.rwInt(ref fixedindex);
			s.rwInt(ref floorheight);
			s.rwInt(ref ceilheight);
			s.rwString(ref floortexname);
			s.rwString(ref ceiltexname);
			//s.rwLong(ref longfloortexname);
			//s.rwLong(ref longceiltexname);
			s.rwInt(ref effect);
			s.rwInt(ref tag);
			s.rwInt(ref brightness);

			// Use a new triangulator when reading from stream
			if(!s.IsWriting && (triangles == null)) triangles = new Triangulation();
			triangles.ReadWrite(s);
			
			if(s.IsWriting)
			{
				s.wInt(labels.Count);
				for(int i = 0; i < labels.Count; i++)
				{
					s.wVector2D(labels[i].position);
					s.wFloat(labels[i].radius);
				}
			}
			else
			{
				longfloortexname = Lump.MakeLongName(floortexname);
				longceiltexname = Lump.MakeLongName(ceiltexname);
				
				int c; s.rInt(out c);
				LabelPositionInfo[] labelsarray = new LabelPositionInfo[c];
				for(int i = 0; i < c; i++)
				{
					s.rVector2D(out labelsarray[i].position);
					s.rFloat(out labelsarray[i].radius);
				}
				labels = Array.AsReadOnly<LabelPositionInfo>(labelsarray);
			}
		}
		
		// After deserialization
		internal void PostDeserialize(MapSet map)
		{
			triangles.PostDeserialize(map);
			
			// We need to rebuild the vertex buffer,
			// but the triangulation was deserialized
			updateneeded = true;
			triangulationneeded = false;
		}
		
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
			s.updateneeded = true;
			base.CopyPropertiesTo(s);
		}

		/// <summary>
		/// Returns the index of this sector. This is a O(n) operation.
		/// </summary>
		public int GetIndex()
		{
			return map.GetIndexForSector(this);
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
					triangulationneeded = false;
					
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

				// Create bounding box
				bbox = CreateBBox();
				
				// Updated
				updateneeded = false;

				// Update buffers
				UpdateFloorSurface();
				UpdateCeilingSurface();
			}
		}

		// This updates the buffer with flat vertices
		public void UpdateFloorSurface()
		{
			// Trash buffer, if any
			if(flatfloorbuffer != null)
			{
				flatfloorbuffer.Dispose();
				flatfloorbuffer = null;
			}

			// Not updated?
			if(updateneeded)
			{
				// Make sure the sector is up-to-date
				// This will automatically call this function again
				UpdateCache();
			}
			// Any vertices?
			else if(flatvertices.Length > 0)
			{
				if(General.Map.Graphics.CheckAvailability())
				{
					FlatVertex[] buffervertices = new FlatVertex[triangles.Vertices.Count];
					flatvertices.CopyTo(buffervertices, 0);

					// Raise event to allow plugins to modify this data
					General.Plugins.OnSectorFloorSurfaceUpdate(this, ref buffervertices);

					// Make the buffer
					flatfloorbuffer = new VertexBuffer(General.Map.Graphics.Device, FlatVertex.Stride * buffervertices.Length,
												  Usage.WriteOnly | Usage.Dynamic, VertexFormat.None, Pool.Default);

					// Fill it
					DataStream bufferstream = flatfloorbuffer.Lock(0, FlatVertex.Stride * buffervertices.Length, LockFlags.Discard);
					bufferstream.WriteRange<FlatVertex>(buffervertices);
					flatfloorbuffer.Unlock();
					bufferstream.Dispose();
				}
			}
		}

		// This updates the buffer with flat vertices
		public void UpdateCeilingSurface()
		{
			// Trash buffer, if any
			if(flatceilingbuffer != null)
			{
				flatceilingbuffer.Dispose();
				flatceilingbuffer = null;
			}

			// Not updated?
			if(updateneeded)
			{
				// Make sure the sector is up-to-date
				// This will automatically call this function again
				UpdateCache();
			}
			// Any vertices?
			else if(flatvertices.Length > 0)
			{
				if(General.Map.Graphics.CheckAvailability())
				{
					FlatVertex[] buffervertices = new FlatVertex[triangles.Vertices.Count];
					flatvertices.CopyTo(buffervertices, 0);

					// Raise event to allow plugins to modify this data
					General.Plugins.OnSectorCeilingSurfaceUpdate(this, ref buffervertices);

					// Make the buffer
					flatceilingbuffer = new VertexBuffer(General.Map.Graphics.Device, FlatVertex.Stride * buffervertices.Length,
												  Usage.WriteOnly | Usage.Dynamic, VertexFormat.None, Pool.Default);

					// Fill it
					DataStream bufferstream = flatceilingbuffer.Lock(0, FlatVertex.Stride * buffervertices.Length, LockFlags.Discard);
					bufferstream.WriteRange<FlatVertex>(buffervertices);
					flatceilingbuffer.Unlock();
					bufferstream.Dispose();
				}
			}
		}

		// Unload unstable resources
		public void UnloadResource()
		{
			// Trash buffer, if any
			if(flatfloorbuffer != null)
			{
				flatfloorbuffer.Dispose();
				flatfloorbuffer = null;
			}
			
			// Trash buffer, if any
			if(flatceilingbuffer != null)
			{
				flatceilingbuffer.Dispose();
				flatceilingbuffer = null;
			}
		}

		// Reload unstable resources
		public void ReloadResource()
		{
			UpdateFloorSurface();
			UpdateCeilingSurface();
		}
		
		#endregion
		
		#region ================== Methods
		
		// This checks if the given point is inside the sector polygon
		public bool Intersect(Vector2D p)
		{
			uint c = 0;
			
			// Go for all sidedefs
			foreach(Sidedef sd in sidedefs)
			{
				// Get vertices
				Vector2D v1 = sd.Line.Start.Position;
				Vector2D v2 = sd.Line.End.Position;
				
				// Determine min/max values
				float miny = Math.Min(v1.y, v2.y);
				float maxy = Math.Max(v1.y, v2.y);
				float maxx = Math.Max(v1.x, v2.x);
				
				// Check for intersection
				if((p.y > miny) && (p.y <= maxy))
				{
					if(p.x <= maxx)
					{
						if(v1.y != v2.y)
						{
							float xint = (p.y - v1.y) * (v2.x - v1.x) / (v2.y - v1.y) + v1.x;
							if((v1.x == v2.x) || (p.x <= xint)) c++;
						}
					}
				}
			}
			
			// Inside this polygon?
			return ((c & 0x00000001UL) != 0);
		}
		
		// This creates a bounding box rectangle
		// This requires the sector triangulation to be up-to-date!
		private RectangleF CreateBBox()
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
			
			General.Map.IsChanged = true;
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
			General.Map.IsChanged = true;
		}

		// This sets texture
		public void SetCeilTexture(string name)
		{
			ceiltexname = name;
			longceiltexname = Lump.MakeLongName(name);
			updateneeded = true;
			General.Map.IsChanged = true;
		}
		
		#endregion
	}
}
