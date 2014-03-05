
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
using System.Collections.Generic;
using CodeImp.DoomBuilder.IO;
using CodeImp.DoomBuilder.Geometry;
using System.Drawing;
using CodeImp.DoomBuilder.Rendering;
using System.Collections.ObjectModel;
using SlimDX;

#endregion

namespace CodeImp.DoomBuilder.Map
{
	public sealed class Sector : SelectableElement
	{
		#region ================== Constants

		#endregion

		#region ================== Variables

		// Map
		private MapSet map;

		// List items
		private LinkedListNode<Sector> selecteditem;
		
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

		//mxd. UDMF properties
		private Dictionary<string, bool> flags;

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
		private SurfaceEntryCollection surfaceentries;

		//mxd. Rendering
		protected Color4 fogColor; //mxd
		protected bool hasFogColor; //mxd
		protected bool useOutsideFog; //mxd
		
		#endregion

		#region ================== Properties

		public MapSet Map { get { return map; } }
		public ICollection<Sidedef> Sidedefs { get { return sidedefs; } }

		/// <summary>
		/// An unique index that does not change when other sectors are removed.
		/// </summary>
		public int FixedIndex { get { return fixedindex; } }
		public int FloorHeight { get { return floorheight; } set { BeforePropsChange(); floorheight = value; } }
		public int CeilHeight { get { return ceilheight; } set { BeforePropsChange(); ceilheight = value; } }
		public string FloorTexture { get { return floortexname; } }
		public string CeilTexture { get { return ceiltexname; } }
		public long LongFloorTexture { get { return longfloortexname; } }
		public long LongCeilTexture { get { return longceiltexname; } }
		internal Dictionary<string, bool> Flags { get { return flags; } } //mxd
		public int Effect { get { return effect; } set { BeforePropsChange(); effect = value; } }
		public int Tag { get { return tag; } set { BeforePropsChange(); tag = value; if((tag < General.Map.FormatInterface.MinTag) || (tag > General.Map.FormatInterface.MaxTag)) throw new ArgumentOutOfRangeException("Tag", "Invalid tag number"); } }
		public int Brightness { get { return brightness; } set { BeforePropsChange(); brightness = value; updateneeded = true; } }
		public bool UpdateNeeded { get { return updateneeded; } set { updateneeded |= value; triangulationneeded |= value; } }
		public RectangleF BBox { get { return bbox; } }
		internal Sector Clone { get { return clone; } set { clone = value; } }
		internal int SerializedIndex { get { return serializedindex; } set { serializedindex = value; } }
		public Triangulation Triangles { get { return triangles; } }
		public FlatVertex[] FlatVertices { get { return flatvertices; } }
		public ReadOnlyCollection<LabelPositionInfo> Labels { get { return labels; } }

		//mxd. Rednering
		public Color4 FogColor { get { return fogColor; } }
		public bool HasFogColor { get { return hasFogColor; } }
		public bool UsesOutsideFog { get { return useOutsideFog; } }

		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		internal Sector(MapSet map, int listindex, int index)
		{
			// Initialize
			this.map = map;
			this.listindex = listindex;
			this.sidedefs = new LinkedList<Sidedef>();
			this.fixedindex = index;
			this.floortexname = "-";
			this.ceiltexname = "-";
			this.longfloortexname = MapSet.EmptyLongName;
			this.longceiltexname = MapSet.EmptyLongName;
			this.flags = new Dictionary<string, bool>(StringComparer.Ordinal); //mxd
			this.updateneeded = true;
			this.triangulationneeded = true;
			this.surfaceentries = new SurfaceEntryCollection();

			if(map == General.Map.Map)
				General.Map.UndoRedo.RecAddSector(this);

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
				
				// Dispose the sidedefs that are attached to this sector
				// because a sidedef cannot exist without reference to its sector.
				if(map.AutoRemove)
					foreach(Sidedef sd in sidedefs) sd.Dispose();
				else
					foreach(Sidedef sd in sidedefs) sd.SetSectorP(null);
				
				if(map == General.Map.Map)
					General.Map.UndoRedo.RecRemSector(this);

				// Remove from main list
				map.RemoveSector(listindex);
				
				// Register the index as free
				map.AddSectorIndexHole(fixedindex);
				
				// Free surface entry
				General.Map.CRenderer2D.Surfaces.FreeSurfaces(surfaceentries);

				// Clean up
				sidedefs = null;
				map = null;
				
				// Dispose base
				base.Dispose();
			}
		}

		#endregion

		#region ================== Management

		// Call this before changing properties
		protected override void BeforePropsChange()
		{
			if(map == General.Map.Map)
				General.Map.UndoRedo.RecPrpSector(this);
		}

		// Serialize / deserialize (passive: this doesn't record)
		new internal void ReadWrite(IReadWriteStream s)
		{
			if(!s.IsWriting)
			{
				BeforePropsChange();
				updateneeded = true;
			}
			
			base.ReadWrite(s);

			//mxd
			if(s.IsWriting) {
				s.wInt(flags.Count);

				foreach(KeyValuePair<string, bool> f in flags) {
					s.wString(f.Key);
					s.wBool(f.Value);
				}
			} else {
				int c; s.rInt(out c);

				flags = new Dictionary<string, bool>(c, StringComparer.Ordinal);
				for(int i = 0; i < c; i++) {
					string t; s.rString(out t);
					bool b; s.rBool(out b);
					flags.Add(t, b);
				}
			}

			s.rwInt(ref fixedindex);
			s.rwInt(ref floorheight);
			s.rwInt(ref ceilheight);
			s.rwString(ref floortexname);
			s.rwString(ref ceiltexname);
			s.rwLong(ref longfloortexname);
			s.rwLong(ref longceiltexname);
			s.rwInt(ref effect);
			s.rwInt(ref tag);
			s.rwInt(ref brightness);
		}
		
		// After deserialization
		internal void PostDeserialize(MapSet map)
		{
			triangles.PostDeserialize(map);
			updateneeded = true;
			triangulationneeded = true;
		}
		
		// This copies all properties to another sector
		public void CopyPropertiesTo(Sector s)
		{
			s.BeforePropsChange();
			
			// Copy properties
			s.ceilheight = ceilheight;
			s.ceiltexname = ceiltexname;
			s.longceiltexname = longceiltexname;
			s.floorheight = floorheight;
			s.floortexname = floortexname;
			s.longfloortexname = longfloortexname;
			s.effect = effect;
			s.tag = tag;
			s.flags = new Dictionary<string, bool>(flags); //mxd
			s.brightness = brightness;
			s.updateneeded = true;
			base.CopyPropertiesTo(s);
		}

		// This attaches a sidedef and returns the listitem
		internal LinkedListNode<Sidedef> AttachSidedefP(Sidedef sd)
		{
			updateneeded = true;
			triangulationneeded = true;
			return sidedefs.AddLast(sd);
		}

		// This detaches a sidedef
		internal void DetachSidedefP(LinkedListNode<Sidedef> l)
		{
			// Not disposing?
			if(!isdisposed)
			{
				// Remove sidedef
				updateneeded = true;
				triangulationneeded = true;
				sidedefs.Remove(l);

				// No more sidedefs left?
				if((sidedefs.Count == 0) && map.AutoRemove)
				{
					// This sector is now useless, dispose it
					this.Dispose();
				}
			}
		}
		
		// This triangulates the sector geometry
		internal void Triangulate()
		{
			if(updateneeded)
			{
				// Triangulate again?
				if(triangulationneeded || (triangles == null))
				{
					// Triangulate sector
					triangles = Triangulation.Create(this);
					triangulationneeded = false;
					updateneeded = true;
					
					// Make label positions
					labels = Array.AsReadOnly<LabelPositionInfo>(Tools.FindLabelPositions(this).ToArray());
					
					// Number of vertices changed?
					if(triangles.Vertices.Count != surfaceentries.totalvertices)
						General.Map.CRenderer2D.Surfaces.FreeSurfaces(surfaceentries);
				}
			}
		}
		
		// This makes new vertices as well as floor and ceiling surfaces
		internal void CreateSurfaces()
		{
			if(updateneeded)
			{
				// Brightness color
				int brightint = General.Map.Renderer2D.CalculateBrightness(brightness);
				
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
				
				// Make update info (this lets the plugin fill in texture coordinates and such)
				SurfaceUpdate updateinfo = new SurfaceUpdate(flatvertices.Length, true, true);
				flatvertices.CopyTo(updateinfo.floorvertices, 0);
				General.Plugins.OnSectorFloorSurfaceUpdate(this, ref updateinfo.floorvertices);
				flatvertices.CopyTo(updateinfo.ceilvertices, 0);
				General.Plugins.OnSectorCeilingSurfaceUpdate(this, ref updateinfo.ceilvertices);
				updateinfo.floortexture = longfloortexname;
				updateinfo.ceiltexture = longceiltexname;

				// Update surfaces
				General.Map.CRenderer2D.Surfaces.UpdateSurfaces(surfaceentries, updateinfo);

				// Updated
				updateneeded = false;
			}
		}

		// This updates the floor surface
		public void UpdateFloorSurface()
		{
			if(flatvertices == null) return;
			
			// Create floor vertices
			SurfaceUpdate updateinfo = new SurfaceUpdate(flatvertices.Length, true, false);
			flatvertices.CopyTo(updateinfo.floorvertices, 0);
			General.Plugins.OnSectorFloorSurfaceUpdate(this, ref updateinfo.floorvertices);
			updateinfo.floortexture = longfloortexname;
			
			// Update entry
			General.Map.CRenderer2D.Surfaces.UpdateSurfaces(surfaceentries, updateinfo);
			General.Map.CRenderer2D.Surfaces.UnlockBuffers();
		}

		// This updates the ceiling surface
		public void UpdateCeilingSurface()
		{
			if(flatvertices == null) return;

			// Create ceiling vertices
			SurfaceUpdate updateinfo = new SurfaceUpdate(flatvertices.Length, false, true);
			flatvertices.CopyTo(updateinfo.ceilvertices, 0);
			General.Plugins.OnSectorCeilingSurfaceUpdate(this, ref updateinfo.ceilvertices);
			updateinfo.ceiltexture = longceiltexname;
			
			// Update entry
			General.Map.CRenderer2D.Surfaces.UpdateSurfaces(surfaceentries, updateinfo);
			General.Map.CRenderer2D.Surfaces.UnlockBuffers();
		}
		
		// This updates the sector when changes have been made
		public void UpdateCache()
		{
			// Update if needed
			if(updateneeded)
			{
				Triangulate();
				
				CreateSurfaces();

				General.Map.CRenderer2D.Surfaces.UnlockBuffers();
			}
		}

		// Selected
		protected override void DoSelect()
		{
			base.DoSelect();
			selecteditem = map.SelectedSectors.AddLast(this);
		}

		// Deselect
		protected override void DoUnselect()
		{
			base.DoUnselect();
			if(selecteditem.List != null) selecteditem.List.Remove(selecteditem);
			selecteditem = null;
		}
		
		#endregion
		
		#region ================== Methods

		// This checks and returns a flag without creating it
		public bool IsFlagSet(string flagname) {
			if(flags.ContainsKey(flagname))
				return flags[flagname];
			else
				return false;
		}

		// This sets a flag
		public void SetFlag(string flagname, bool value) {
			if(!flags.ContainsKey(flagname) || (IsFlagSet(flagname) != value)) {
				BeforePropsChange();

				flags[flagname] = value;
			}
		}

		// This returns a copy of the flags dictionary
		public Dictionary<string, bool> GetFlags() {
			return new Dictionary<string, bool>(flags);
		}

		// This clears all flags
		public void ClearFlags() {
			BeforePropsChange();

			flags.Clear();
		}
		
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
			if(sidedefs.Count == 0) return  new RectangleF(); //mxd
			
			// Setup
			float left = float.MaxValue;
			float top = float.MaxValue;
			float right = float.MinValue;
			float bottom = float.MinValue;

			Dictionary<Vertex, bool> processed = new Dictionary<Vertex, bool>(); //mxd

			//mxd. This way bbox will be created even if triangulation failed (sector with 2 or less sidedefs and 2 vertices)
			foreach (Sidedef s in sidedefs) {
				//start...
				if (!processed.ContainsKey(s.Line.Start)) {
					if (s.Line.Start.Position.x < left) left = s.Line.Start.Position.x;
					if (s.Line.Start.Position.x > right) right = s.Line.Start.Position.x;
					if (s.Line.Start.Position.y < top) top = s.Line.Start.Position.y;
					if (s.Line.Start.Position.y > bottom) bottom = s.Line.Start.Position.y;
					processed.Add(s.Line.Start, false);
				}

				//end...
				if(!processed.ContainsKey(s.Line.End)) {
					if(s.Line.End.Position.x < left) left = s.Line.End.Position.x;
					if(s.Line.End.Position.x > right) right = s.Line.End.Position.x;
					if(s.Line.End.Position.y < top) top = s.Line.End.Position.y;
					if(s.Line.End.Position.y > bottom) bottom = s.Line.End.Position.y;
					processed.Add(s.Line.End, false);
				}
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
					sidedefs.First.Value.SetSector(other);
			}
			else
			{
				// No sidedefs attached
				// Dispose manually
				this.Dispose();
			}
			
			General.Map.IsChanged = true;
		}

		// String representation
		public override string ToString()
		{
			return "Sector " + listindex;
		}
		
		#endregion

		#region ================== Changes

		//mxd. This updates all properties (Doom/Hexen version)
		public void Update(int hfloor, int hceil, string tfloor, string tceil, int effect, int tag, int brightness) 
		{
			Update(hfloor, hceil, tfloor, tceil, effect, new Dictionary<string, bool>(StringComparer.Ordinal), tag, brightness);
		}

		//mxd. This updates all properties (UDMF version)
		public void Update(int hfloor, int hceil, string tfloor, string tceil, int effect, Dictionary<string, bool> flags, int tag, int brightness)
		{
			BeforePropsChange();
			
			// Apply changes
			this.floorheight = hfloor;
			this.ceilheight = hceil;
			SetFloorTexture(tfloor);
			SetCeilTexture(tceil);
			this.effect = effect;
			this.tag = tag;
			this.flags = new Dictionary<string, bool>(flags); //mxd
			this.brightness = brightness;
			updateneeded = true;
		}

		// This sets texture
		public void SetFloorTexture(string name)
		{
			BeforePropsChange();
			
			if(string.IsNullOrEmpty(name)) name = "-"; //mxd
			floortexname = name;
			longfloortexname = Lump.MakeLongName(name);
			updateneeded = true;
			General.Map.IsChanged = true;
		}

		// This sets texture
		public void SetCeilTexture(string name)
		{
			BeforePropsChange();
			
			if(string.IsNullOrEmpty(name)) name = "-"; //mxd
			ceiltexname = name;
			longceiltexname = Lump.MakeLongName(name);
			updateneeded = true;
			General.Map.IsChanged = true;
		}

		//mxd
		public void UpdateFogColor() {
			useOutsideFog = General.Map.Data.MapInfo.HasOutsideFogColor && ceiltexname == General.Map.Config.SkyFlatName;

			if(General.Map.UDMF && Fields.ContainsKey("fadecolor")) {
				fogColor = new Color4((int)Fields["fadecolor"].Value);
			} else if(useOutsideFog) {
				fogColor = General.Map.Data.MapInfo.OutsideFogColor;
			} else if(General.Map.Data.MapInfo.HasFadeColor) {
				fogColor = General.Map.Data.MapInfo.FadeColor;
			} else {
				fogColor = new Color4();
			}

			hasFogColor = fogColor.Red > 0 || fogColor.Green > 0 || fogColor.Blue > 0;
		}
		
		#endregion
	}
}
