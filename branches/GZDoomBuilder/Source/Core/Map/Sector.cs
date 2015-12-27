
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

		internal const int SLOPE_DECIMALS = 7;

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
		private List<int> tags; //mxd
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
		private readonly SurfaceEntryCollection surfaceentries;

		//mxd. Rendering
		private Color4 fogColor;
		private bool hasFogColor;
		private bool useOutsideFog;

		//mxd. Slopes
		private Vector3D floorslope;
		private float flooroffset;
		private Vector3D ceilslope;
		private float ceiloffset;
		
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
		public int Tag { get { return tags[0]; } set { BeforePropsChange(); tags[0] = value; if((value < General.Map.FormatInterface.MinTag) || (value > General.Map.FormatInterface.MaxTag)) throw new ArgumentOutOfRangeException("Tag", "Invalid tag number"); } } //mxd
		public List<int> Tags { get { return tags; } set { BeforePropsChange(); tags = value; } } //mxd
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

		//mxd. Slopes
		public Vector3D FloorSlope { get { return floorslope; } set { BeforePropsChange(); floorslope = value; updateneeded = true; } }
		public float FloorSlopeOffset { get { return flooroffset; } set { BeforePropsChange(); flooroffset = value; updateneeded = true; } }
		public Vector3D CeilSlope { get { return ceilslope; } set { BeforePropsChange(); ceilslope = value; updateneeded = true; } }
		public float CeilSlopeOffset { get { return ceiloffset; } set { BeforePropsChange(); ceiloffset = value; updateneeded = true; } }

		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		internal Sector(MapSet map, int listindex, int index)
		{
			// Initialize
			this.elementtype = MapElementType.SECTOR; //mxd
			this.map = map;
			this.listindex = listindex;
			this.sidedefs = new LinkedList<Sidedef>();
			this.fixedindex = index;
			this.floortexname = "-";
			this.ceiltexname = "-";
			this.longfloortexname = MapSet.EmptyLongName;
			this.longceiltexname = MapSet.EmptyLongName;
			this.flags = new Dictionary<string, bool>(StringComparer.Ordinal); //mxd
			this.tags = new List<int> { 0 }; //mxd
			this.updateneeded = true;
			this.triangulationneeded = true;
			this.triangles = new Triangulation(); //mxd
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

				//mxd. Restore isdisposed so base classes can do their disposal job
				isdisposed = false;
				
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
			if(s.IsWriting)
			{
				s.wInt(flags.Count);

				foreach(KeyValuePair<string, bool> f in flags)
				{
					s.wString(f.Key);
					s.wBool(f.Value);
				}
			}
			else
			{
				int c; s.rInt(out c);

				flags = new Dictionary<string, bool>(c, StringComparer.Ordinal);
				for(int i = 0; i < c; i++)
				{
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
			s.rwInt(ref brightness);

			//mxd. (Re)store tags
			if(s.IsWriting) 
			{
				s.wInt(tags.Count);
				foreach(int tag in tags) s.wInt(tag);
			} 
			else 
			{
				int c;
				s.rInt(out c);
				tags = new List<int>(c);
				for(int i = 0; i < c; i++)
				{
					int t;
					s.rInt(out t);
					tags.Add(t);
				}
			}

			//mxd. Slopes
			s.rwFloat(ref flooroffset);
			s.rwVector3D(ref floorslope);
			s.rwFloat(ref ceiloffset);
			s.rwVector3D(ref ceilslope);
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
			s.tags = new List<int>(tags); //mxd
			s.flags = new Dictionary<string, bool>(flags); //mxd
			s.brightness = brightness;
			s.flooroffset = flooroffset; //mxd
			s.floorslope = floorslope; //mxd
			s.ceiloffset = ceiloffset; //mxd
			s.ceilslope = ceilslope; //mxd
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
					labels = Array.AsReadOnly(Tools.FindLabelPositions(this).ToArray());
					
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

		// This removes UDMF stuff (mxd)
		internal void TranslateFromUDMF() 
		{
			// Clear UDMF-related properties (but keep VirtualSectorField!)
			bool isvirtual = this.Fields.ContainsKey(MapSet.VirtualSectorField);
			this.Fields.Clear();
			if(isvirtual) this.Fields.Add(MapSet.VirtualSectorField, MapSet.VirtualSectorValue);
			this.Flags.Clear();
			hasFogColor = false;
			useOutsideFog = false;

			// Reset Slopes
			floorslope = new Vector3D();
			flooroffset = 0;
			ceilslope = new Vector3D();
			ceiloffset = 0;
		}
		
		#endregion
		
		#region ================== Methods

		// This checks and returns a flag without creating it
		public bool IsFlagSet(string flagname)
		{
			return flags.ContainsKey(flagname) && flags[flagname];
		}

		// This sets a flag
		public void SetFlag(string flagname, bool value) 
		{
			if(!flags.ContainsKey(flagname) || (IsFlagSet(flagname) != value)) 
			{
				BeforePropsChange();

				flags[flagname] = value;
			}
		}

		// This returns a copy of the flags dictionary
		public Dictionary<string, bool> GetFlags() 
		{
			return new Dictionary<string, bool>(flags);
		}

		// This clears all flags
		public void ClearFlags() 
		{
			BeforePropsChange();
			flags.Clear();
		}
		
		// This checks if the given point is inside the sector polygon
		public bool Intersect(Vector2D p) 
		{
			//mxd. Check bounding box first
			if(p.x < bbox.Left || p.x > bbox.Right || p.y < bbox.Top || p.y > bbox.Bottom) return false;
			
			uint c = 0;
			
			// Go for all sidedefs
			foreach(Sidedef sd in sidedefs)
			{
				// Get vertices
				Vector2D v1 = sd.Line.Start.Position;
				Vector2D v2 = sd.Line.End.Position;

				//mxd. On top of a vertex?
				if (p == v1 || p == v2) return true;
				
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
			foreach (Sidedef s in sidedefs) 
			{
				//start...
				if (!processed.ContainsKey(s.Line.Start)) 
				{
					if (s.Line.Start.Position.x < left) left = s.Line.Start.Position.x;
					if (s.Line.Start.Position.x > right) right = s.Line.Start.Position.x;
					if (s.Line.Start.Position.y < top) top = s.Line.Start.Position.y;
					if (s.Line.Start.Position.y > bottom) bottom = s.Line.Start.Position.y;
					processed.Add(s.Line.Start, false);
				}

				//end...
				if(!processed.ContainsKey(s.Line.End)) 
				{
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

		//mxd
		public static Geometry.Plane GetFloorPlane(Sector s)
		{
			if(General.Map.UDMF)
			{
				// UDMF Sector slope?
				if(s.FloorSlope.GetLengthSq() > 0 && !float.IsNaN(s.FloorSlopeOffset / s.FloorSlope.z)) 
					return new Geometry.Plane(s.FloorSlope, s.FloorSlopeOffset);

				if(s.sidedefs.Count == 3)
				{
					Geometry.Plane floor = new Geometry.Plane(new Vector3D(0, 0, 1), -s.FloorHeight);
					Vector3D[] verts = new Vector3D[3];
					bool sloped = false;
					int index = 0;
					
					// Check vertices
					foreach(Sidedef sd in s.Sidedefs) 
					{
						Vertex v = sd.IsFront ? sd.Line.End : sd.Line.Start;

						//create "normal" vertices
						verts[index] = new Vector3D(v.Position);

						// Check floor
						if(!float.IsNaN(v.ZFloor)) 
						{
							//vertex offset is absolute
							verts[index].z = v.ZFloor;
							sloped = true;
						} 
						else 
						{
							verts[index].z = floor.GetZ(v.Position);
						}

						index++;
					}

					// Have slope?
					return (sloped ? new Geometry.Plane(verts[0], verts[1], verts[2], true) : floor);
				}
			}

			// Have line slope?
			foreach (Sidedef side in s.sidedefs)
			{
				// Carbon copy of EffectLineSlope class here...
				if(side.Line.Action == 181 && ((side.Line.Args[0] == 1 && side == side.Line.Front) || side.Line.Args[0] == 2) && side.Other != null)
				{
					Linedef l = side.Line;
					
					// Find the vertex furthest from the line
					Vertex foundv = null;
					float founddist = -1.0f;
					foreach(Sidedef sd in s.Sidedefs) 
					{
						Vertex v = sd.IsFront ? sd.Line.Start : sd.Line.End;
						float d = l.DistanceToSq(v.Position, false);
						if(d > founddist) 
						{
							foundv = v;
							founddist = d;
						}
					}

					Vector3D v1 = new Vector3D(l.Start.Position.x, l.Start.Position.y, side.Other.Sector.FloorHeight);
					Vector3D v2 = new Vector3D(l.End.Position.x, l.End.Position.y, side.Other.Sector.FloorHeight);
					Vector3D v3 = new Vector3D(foundv.Position.x, foundv.Position.y, s.FloorHeight);

					return (l.SideOfLine(v3) < 0.0f ? new Geometry.Plane(v1, v2, v3, true) : new Geometry.Plane(v2, v1, v3, true));
				}
			}

			//TODO: other types of slopes...

			// Normal (flat) floor plane
			return new Geometry.Plane(new Vector3D(0, 0, 1), -s.FloorHeight);
		}

		//mxd
		public static Geometry.Plane GetCeilingPlane(Sector s)
		{
			if(General.Map.UDMF) 
			{
				// UDMF Sector slope?
				if(s.CeilSlope.GetLengthSq() > 0 && !float.IsNaN(s.CeilSlopeOffset / s.CeilSlope.z))
					return new Geometry.Plane(s.CeilSlope, s.CeilSlopeOffset);

				if(s.sidedefs.Count == 3) 
				{
					Geometry.Plane ceiling = new Geometry.Plane(new Vector3D(0, 0, -1), s.CeilHeight);
					Vector3D[] verts = new Vector3D[3];
					bool sloped = false;
					int index = 0;

					// Check vertices
					foreach(Sidedef sd in s.Sidedefs) 
					{
						Vertex v = sd.IsFront ? sd.Line.End : sd.Line.Start;

						//create "normal" vertices
						verts[index] = new Vector3D(v.Position);

						// Check floor
						if(!float.IsNaN(v.ZCeiling)) 
						{
							//vertex offset is absolute
							verts[index].z = v.ZCeiling;
							sloped = true;
						} 
						else 
						{
							verts[index].z = ceiling.GetZ(v.Position);
						}

						index++;
					}

					// Have slope?
					return (sloped ? new Geometry.Plane(verts[0], verts[2], verts[1], false) : ceiling);
				}
			}

			// Have line slope?
			foreach(Sidedef side in s.sidedefs) 
			{
				// Carbon copy of EffectLineSlope class here...
				if(side.Line.Action == 181 && ((side.Line.Args[1] == 1 && side == side.Line.Front) || side.Line.Args[1] == 2) && side.Other != null) 
				{
					Linedef l = side.Line;

					// Find the vertex furthest from the line
					Vertex foundv = null;
					float founddist = -1.0f;
					foreach(Sidedef sd in s.Sidedefs) 
					{
						Vertex v = sd.IsFront ? sd.Line.Start : sd.Line.End;
						float d = l.DistanceToSq(v.Position, false);
						if(d > founddist) 
						{
							foundv = v;
							founddist = d;
						}
					}

					Vector3D v1 = new Vector3D(l.Start.Position.x, l.Start.Position.y, side.Other.Sector.CeilHeight);
					Vector3D v2 = new Vector3D(l.End.Position.x, l.End.Position.y, side.Other.Sector.CeilHeight);
					Vector3D v3 = new Vector3D(foundv.Position.x, foundv.Position.y, s.CeilHeight);

					return (l.SideOfLine(v3) > 0.0f ? new Geometry.Plane(v1, v2, v3, false) : new Geometry.Plane(v2, v1, v3, false));
				}
			}

			//TODO: other types of slopes...

			// Normal (flat) ceiling plane
			return new Geometry.Plane(new Vector3D(0, 0, -1), s.CeilHeight);
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
			Update(hfloor, hceil, tfloor, tceil, effect, new Dictionary<string, bool>(StringComparer.Ordinal), new List<int> { tag }, brightness, 0, new Vector3D(), 0, new Vector3D());
		}

		//mxd. This updates all properties (UDMF version)
		public void Update(int hfloor, int hceil, string tfloor, string tceil, int effect, Dictionary<string, bool> flags, List<int> tags, int brightness, float flooroffset, Vector3D floorslope, float ceiloffset, Vector3D ceilslope)
		{
			BeforePropsChange();
			
			// Apply changes
			this.floorheight = hfloor;
			this.ceilheight = hceil;
			//SetFloorTexture(tfloor);
			//SetCeilTexture(tceil);
			this.effect = effect;
			this.tags = new List<int>(tags); //mxd
			this.flags = new Dictionary<string, bool>(flags); //mxd
			this.brightness = brightness;
			this.flooroffset = flooroffset; //mxd
			this.floorslope = floorslope; //mxd
			this.ceiloffset = ceiloffset; //mxd
			this.ceilslope = ceilslope; //mxd

			//mxd. Set ceil texture
			if(string.IsNullOrEmpty(tceil)) tceil = "-";
			ceiltexname = tceil;
			longceiltexname = Lump.MakeLongName(ceiltexname);

			//mxd. Set floor texture
			if(string.IsNullOrEmpty(tfloor)) tfloor = "-"; //mxd
			floortexname = tfloor;
			longfloortexname = Lump.MakeLongName(tfloor);

			//mxd. Map is changed
			General.Map.IsChanged = true;
			updateneeded = true;
		}

		// This sets texture
		public void SetFloorTexture(string name)
		{
			BeforePropsChange();
			
			if(string.IsNullOrEmpty(name)) name = "-"; //mxd
			floortexname = name;
			longfloortexname = General.Map.Data.GetFullLongFlatName(Lump.MakeLongName(name)); //mxd
			updateneeded = true;
			General.Map.IsChanged = true;
		}

		// This sets texture
		public void SetCeilTexture(string name)
		{
			BeforePropsChange();
			
			if(string.IsNullOrEmpty(name)) name = "-"; //mxd
			ceiltexname = name;
			longceiltexname = General.Map.Data.GetFullLongFlatName(Lump.MakeLongName(name)); //mxd
			updateneeded = true;
			General.Map.IsChanged = true;
		}

		//mxd. This sets texture lookup
		internal void SetFloorTexture(long hash)
		{
			BeforePropsChange();

			longfloortexname = hash;
			updateneeded = true;
			General.Map.IsChanged = true;
		}

		//mxd. This sets texture lookup
		internal void SetCeilTexture(long hash)
		{
			BeforePropsChange();

			longceiltexname = hash;
			updateneeded = true;
			General.Map.IsChanged = true;
		}

		//mxd
		public void UpdateFogColor() 
		{
			// Sector uses outisde fog when it's ceiling is sky or Sector_Outside effect (87) is set
			useOutsideFog = (General.Map.Data.MapInfo.HasOutsideFogColor && (ceiltexname == General.Map.Config.SkyFlatName || (effect == 87 && General.Map.Config.SectorEffects.ContainsKey(effect))));

			if(General.Map.UDMF && Fields.ContainsKey("fadecolor"))
				fogColor = new Color4((int)Fields["fadecolor"].Value);
			else if(useOutsideFog)
				fogColor = General.Map.Data.MapInfo.OutsideFogColor;
			else if(General.Map.Data.MapInfo.HasFadeColor)
				fogColor = General.Map.Data.MapInfo.FadeColor;
			else
				fogColor = new Color4();

			hasFogColor = fogColor.Red > 0 || fogColor.Green > 0 || fogColor.Blue > 0;
		}
		
		#endregion
	}
}
