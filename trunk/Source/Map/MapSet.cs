
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
using SlimDX.Direct3D9;
using CodeImp.DoomBuilder.Rendering;
using SlimDX;
using System.Drawing;
using CodeImp.DoomBuilder.Editing;
using CodeImp.DoomBuilder.IO;
using CodeImp.DoomBuilder.Types;
using System.IO;

#endregion

namespace CodeImp.DoomBuilder.Map
{
	public sealed class MapSet
	{
		#region ================== Constants

		// Stiching distance
		public const float STITCH_DISTANCE = 0.001f;
		
		// Virtual sector identification
		// This contains a character that is invalid in the UDMF standard, but valid
		// in our parser, so that it can only be used by Doom Builder and will never
		// conflict with any other valid UDMF field.
		internal const string VIRTUAL_SECTOR_FIELD = "!virtual_sector";
		
		#endregion

		#region ================== Variables

		// Sector indexing
		private List<int> indexholes;
		private int lastsectorindex;
		
		// Sidedef indexing for (de)serialization
		private Sidedef[] sidedefindices;
		
		// Map structures
		private LinkedList<Vertex> vertices;
		private LinkedList<Linedef> linedefs;
		private LinkedList<Sidedef> sidedefs;
		private LinkedList<Sector> sectors;
		private LinkedList<Thing> things;

		// Selected elements
		private LinkedList<Vertex> sel_vertices;
		private LinkedList<Linedef> sel_linedefs;
		private LinkedList<Sector> sel_sectors;
		private LinkedList<Thing> sel_things;
		private SelectionType sel_type;

		// Statics
		private static long emptylongname;
		private static UniValue virtualsectorvalue;

		// Disposing
		private bool isdisposed = false;

		#endregion

		#region ================== Properties

		public ICollection<Vertex> Vertices { get { return vertices; } }
		public ICollection<Linedef> Linedefs { get { return linedefs; } }
		public ICollection<Sidedef> Sidedefs { get { return sidedefs; } }
		public ICollection<Sector> Sectors { get { return sectors; } }
		public ICollection<Thing> Things { get { return things; } }
		public bool IsDisposed { get { return isdisposed; } }

		internal LinkedList<Vertex> SelectedVertices { get { return sel_vertices; } }
		internal LinkedList<Linedef> SelectedLinedefs { get { return sel_linedefs; } }
		internal LinkedList<Sector> SelectedSectors { get { return sel_sectors; } }
		internal LinkedList<Thing> SelectedThings { get { return sel_things; } }
		public SelectionType SelectionType { get { return sel_type; } set { sel_type = value; } }
		
		public static long EmptyLongName { get { return emptylongname; } }
		public static string VirtualSectorField { get { return VIRTUAL_SECTOR_FIELD; } }
		public static UniValue VirtualSectorValue { get { return virtualsectorvalue; } }
		
		internal Sidedef[] SidedefIndices { get { return sidedefindices; } }
		
		#endregion

		#region ================== Constructor / Disposer

		// Constructor for new empty map
		internal MapSet()
		{
			// Initialize
			vertices = new LinkedList<Vertex>();
			linedefs = new LinkedList<Linedef>();
			sidedefs = new LinkedList<Sidedef>();
			sectors = new LinkedList<Sector>();
			things = new LinkedList<Thing>();
			sel_vertices = new LinkedList<Vertex>();
			sel_linedefs = new LinkedList<Linedef>();
			sel_sectors = new LinkedList<Sector>();
			sel_things = new LinkedList<Thing>();
			indexholes = new List<int>();
			lastsectorindex = 0;
			
			// We have no destructor
			GC.SuppressFinalize(this);
		}

		// Constructor for map to deserialize
		internal MapSet(MemoryStream stream)
		{
			// Initialize
			vertices = new LinkedList<Vertex>();
			linedefs = new LinkedList<Linedef>();
			sidedefs = new LinkedList<Sidedef>();
			sectors = new LinkedList<Sector>();
			things = new LinkedList<Thing>();
			sel_vertices = new LinkedList<Vertex>();
			sel_linedefs = new LinkedList<Linedef>();
			sel_sectors = new LinkedList<Sector>();
			sel_things = new LinkedList<Thing>();
			indexholes = new List<int>();
			lastsectorindex = 0;

			// Deserialize
			Deserialize(stream);

			// We have no destructor
			GC.SuppressFinalize(this);
		}

		// Disposer
		internal void Dispose()
		{
			ArrayList list;
			
			// Not already disposed?
			if(!isdisposed)
			{
				// Already set isdisposed so that changes can be prohibited
				isdisposed = true;

				// Dispose all things
				list = new ArrayList(things);
				foreach(Thing t in list) t.Dispose();

				// Dispose all sectors
				list = new ArrayList(sectors);
				foreach(Sector s in list) s.Dispose();

				// Dispose all sidedefs
				list = new ArrayList(sidedefs);
				foreach(Sidedef sd in list) sd.Dispose();

				// Dispose all linedefs
				list = new ArrayList(linedefs);
				foreach(Linedef l in list) l.Dispose();

				// Dispose all vertices
				list = new ArrayList(vertices);
				foreach(Vertex v in list) v.Dispose();

				// Clean up
				vertices = null;
				linedefs = null;
				sidedefs = null;
				sectors = null;
				things = null;
				sel_vertices = null;
				sel_linedefs = null;
				sel_sectors = null;
				sel_things = null;
				indexholes = null;
				
				// Done
				isdisposed = true;
			}
		}

		// Static initializer
		internal static void Initialize()
		{
			emptylongname = Lump.MakeLongName("-");
			virtualsectorvalue = new UniValue((int)UniversalType.Integer, (int)0);
		}

		#endregion

		#region ================== Management

		// This makes a deep copy and returns a new MapSet
		public MapSet Clone()
		{
			Linedef nl;
			Sidedef nd;
			
			// Create the map set
			MapSet newset = new MapSet();

			// Go for all vertices
			foreach(Vertex v in vertices)
			{
				// Make new vertex
				v.Clone = newset.CreateVertex(v.Position);
				v.CopyPropertiesTo(v.Clone);
			}

			// Go for all sectors
			foreach(Sector s in sectors)
			{
				// Make new sector
				s.Clone = newset.CreateSector();
				s.CopyPropertiesTo(s.Clone);
			}

			// Go for all linedefs
			foreach(Linedef l in linedefs)
			{
				// Make new linedef
				nl = newset.CreateLinedef(l.Start.Clone, l.End.Clone);
				l.CopyPropertiesTo(nl);

				// Linedef has a front side?
				if(l.Front != null)
				{
					// Make new sidedef
					nd = newset.CreateSidedef(nl, true, l.Front.Sector.Clone);
					l.Front.CopyPropertiesTo(nd);
				}

				// Linedef has a back side?
				if(l.Back != null)
				{
					// Make new sidedef
					nd = newset.CreateSidedef(nl, false, l.Back.Sector.Clone);
					l.Back.CopyPropertiesTo(nd);
				}
			}

			// Go for all things
			foreach(Thing t in things)
			{
				// Make new thing
				Thing nt = newset.CreateThing();
				t.CopyPropertiesTo(nt);
			}

			// Remove clone references
			foreach(Vertex v in vertices) v.Clone = null;
			foreach(Sector s in sectors) s.Clone = null;
			
			// Return the new set
			return newset;
		}

		// This makes a deep copy of the marked geometry and binds missing sectors to a virtual sector
		internal MapSet CloneMarked()
		{
			Sector virtualsector = null;
			
			// Create the map set
			MapSet newset = new MapSet();

			// Get marked geometry
			ICollection<Vertex> mvertices = GetMarkedVertices(true);
			ICollection<Linedef> mlinedefs = GetMarkedLinedefs(true);
			ICollection<Sector> msectors = GetMarkedSectors(true);
			ICollection<Thing> mthings = GetMarkedThings(true);
			
			// Go for all vertices
			foreach(Vertex v in mvertices)
			{
				// Make new vertex
				v.Clone = newset.CreateVertex(v.Position);
				v.CopyPropertiesTo(v.Clone);
			}

			// Go for all sectors
			foreach(Sector s in msectors)
			{
				// Make new sector
				s.Clone = newset.CreateSector();
				s.CopyPropertiesTo(s.Clone);
			}

			// Go for all linedefs
			foreach(Linedef l in mlinedefs)
			{
				// Make new linedef
				Linedef nl = newset.CreateLinedef(l.Start.Clone, l.End.Clone);
				l.CopyPropertiesTo(nl);

				// Linedef has a front side?
				if(l.Front != null)
				{
					Sidedef nd;
					
					// Sector on front side marked?
					if(l.Front.Sector.Marked)
					{
						// Make new sidedef
						nd = newset.CreateSidedef(nl, true, l.Front.Sector.Clone);
					}
					else
					{
						// Make virtual sector if needed
						if(virtualsector == null)
						{
							virtualsector = newset.CreateSector();
							l.Front.Sector.CopyPropertiesTo(virtualsector);
							virtualsector.Fields[VIRTUAL_SECTOR_FIELD] = new UniValue(virtualsectorvalue);
						}
						
						// Make new sidedef that links to the virtual sector
						nd = newset.CreateSidedef(nl, true, virtualsector);
					}
					
					l.Front.CopyPropertiesTo(nd);
				}

				// Linedef has a back side?
				if(l.Back != null)
				{
					Sidedef nd;

					// Sector on front side marked?
					if(l.Back.Sector.Marked)
					{
						// Make new sidedef
						nd = newset.CreateSidedef(nl, false, l.Back.Sector.Clone);
					}
					else
					{
						// Make virtual sector if needed
						if(virtualsector == null)
						{
							virtualsector = newset.CreateSector();
							l.Back.Sector.CopyPropertiesTo(virtualsector);
							virtualsector.Fields[VIRTUAL_SECTOR_FIELD] = new UniValue(virtualsectorvalue);
						}

						// Make new sidedef that links to the virtual sector
						nd = newset.CreateSidedef(nl, false, virtualsector);
					}
					
					l.Back.CopyPropertiesTo(nd);
				}
			}

			// Go for all things
			foreach(Thing t in mthings)
			{
				// Make new thing
				Thing nt = newset.CreateThing();
				t.CopyPropertiesTo(nt);
			}

			// Remove clone references
			foreach(Vertex v in vertices) v.Clone = null;
			foreach(Sector s in sectors) s.Clone = null;

			// Return the new set
			return newset;
		}
		
		// This creates a new vertex
		public Vertex CreateVertex(Vector2D pos)
		{
			LinkedListNode<Vertex> listitem;
			Vertex v;
			
			// Make a list item
			listitem = new LinkedListNode<Vertex>(null);

			// Make the vertex
			v = new Vertex(this, listitem, pos);
			listitem.Value = v;

			// Add vertex to the list
			vertices.AddLast(listitem);

			// Return result
			return v;
		}

		// This creates a new vertex
		private Vertex CreateVertex(IReadWriteStream stream)
		{
			LinkedListNode<Vertex> listitem;
			Vertex v;

			// Make a list item
			listitem = new LinkedListNode<Vertex>(null);

			// Make the vertex
			v = new Vertex(this, listitem, stream);
			listitem.Value = v;

			// Add vertex to the list
			vertices.AddLast(listitem);

			// Return result
			return v;
		}

		// This creates a new linedef
		public Linedef CreateLinedef(Vertex start, Vertex end)
		{
			LinkedListNode<Linedef> listitem;
			Linedef l;

			// Make a list item
			listitem = new LinkedListNode<Linedef>(null);

			// Make the linedef
			l = new Linedef(this, listitem, start, end);
			listitem.Value = l;

			// Add linedef to the list
			linedefs.AddLast(listitem);

			// Return result
			return l;
		}

		// This creates a new linedef
		private Linedef CreateLinedef(Vertex start, Vertex end, IReadWriteStream stream)
		{
			LinkedListNode<Linedef> listitem;
			Linedef l;

			// Make a list item
			listitem = new LinkedListNode<Linedef>(null);

			// Make the linedef
			l = new Linedef(this, listitem, start, end, stream);
			listitem.Value = l;

			// Add linedef to the list
			linedefs.AddLast(listitem);

			// Return result
			return l;
		}

		// This creates a new sidedef
		public Sidedef CreateSidedef(Linedef l, bool front, Sector s)
		{
			LinkedListNode<Sidedef> listitem;
			Sidedef sd;

			// Make a list item
			listitem = new LinkedListNode<Sidedef>(null);

			// Make the sidedef
			sd = new Sidedef(this, listitem, l, front, s);
			listitem.Value = sd;

			// Add sidedef to the list
			sidedefs.AddLast(listitem);

			// Return result
			return sd;
		}

		// This creates a new sidedef
		private Sidedef CreateSidedef(Linedef l, bool front, Sector s, IReadWriteStream stream)
		{
			LinkedListNode<Sidedef> listitem;
			Sidedef sd;

			// Make a list item
			listitem = new LinkedListNode<Sidedef>(null);

			// Make the sidedef
			sd = new Sidedef(this, listitem, l, front, s, stream);
			listitem.Value = sd;

			// Add sidedef to the list
			sidedefs.AddLast(listitem);

			// Return result
			return sd;
		}

		// This creates a new sector
		public Sector CreateSector()
		{
			int index;
			
			// Do we have any index holes we can use?
			if(indexholes.Count > 0)
			{
				// Take one of the index holes
				index = indexholes[indexholes.Count - 1];
				indexholes.RemoveAt(indexholes.Count - 1);
			}
			else
			{
				// Make a new index
				index = lastsectorindex++;
			}
			
			// Make the sector
			return CreateSector(index);
		}
		
		// This creates a new sector
		public Sector CreateSector(int index)
		{
			LinkedListNode<Sector> listitem;
			Sector s;

			// Make a list item
			listitem = new LinkedListNode<Sector>(null);

			// Make the sector
			s = new Sector(this, listitem, index);
			listitem.Value = s;

			// Add sector to the list
			sectors.AddLast(listitem);

			// Return result
			return s;
		}

		// This creates a new sector
		private Sector CreateSector(IReadWriteStream stream)
		{
			LinkedListNode<Sector> listitem;
			Sector s;

			// Make a list item
			listitem = new LinkedListNode<Sector>(null);

			// Make the sector
			s = new Sector(this, listitem, stream);
			listitem.Value = s;

			// Add sector to the list
			sectors.AddLast(listitem);

			// Return result
			return s;
		}

		// This creates a new thing
		public Thing CreateThing()
		{
			LinkedListNode<Thing> listitem;
			Thing t;

			// Make a list item
			listitem = new LinkedListNode<Thing>(null);

			// Make the thing
			t = new Thing(this, listitem);
			listitem.Value = t;

			// Add thing to the list
			things.AddLast(listitem);

			// Return result
			return t;
		}

		// This adds a sector index hole
		public void AddSectorIndexHole(int index)
		{
			indexholes.Add(index);
		}

		#endregion

		#region ================== Serialization

		// This serializes the MapSet
		internal MemoryStream Serialize()
		{
			MemoryStream stream = new MemoryStream(20000000);	// Yes that is about 20 MB.
			SerializerStream serializer = new SerializerStream(stream);
			
			// Index the sidedefs
			int sidedefindex = 0;
			foreach(Sidedef sd in sidedefs)
				sd.SerializedIndex = sidedefindex++;

			serializer.Begin();

			// Write private data
			serializer.wInt(lastsectorindex);
			serializer.wInt(indexholes.Count);
			foreach(int i in indexholes) serializer.wInt(i);

			// Write map data
			WriteVertices(serializer);
			WriteSectors(serializer);
			WriteLinedefs(serializer);
			WriteSidedefs(serializer);
			WriteThings(serializer);

			serializer.End();

			// Reallocate to keep only the used memory
			stream.Capacity = (int)stream.Length;
			
			return stream;
		}

		// This serializes things
		private void WriteThings(SerializerStream stream)
		{
			stream.wInt(things.Count);
			
			// Go for all things
			foreach(Thing t in things)
			{
				t.ReadWrite(stream);
			}
		}

		// This serializes vertices
		private void WriteVertices(SerializerStream stream)
		{
			stream.wInt(vertices.Count);

			// Go for all vertices
			int index = 0;
			foreach(Vertex v in vertices)
			{
				v.SerializedIndex = index++;
				
				v.ReadWrite(stream);
			}
		}

		// This serializes linedefs
		private void WriteLinedefs(SerializerStream stream)
		{
			stream.wInt(linedefs.Count);

			// Go for all lines
			int index = 0;
			foreach(Linedef l in linedefs)
			{
				l.SerializedIndex = index++;
				
				stream.wInt(l.Start.SerializedIndex);
				
				stream.wInt(l.End.SerializedIndex);

				l.ReadWrite(stream);
			}
		}

		// This serializes sidedefs
		private void WriteSidedefs(SerializerStream stream)
		{
			stream.wInt(sidedefs.Count);

			// Go for all sidedefs
			foreach(Sidedef sd in sidedefs)
			{
				stream.wInt(sd.Line.SerializedIndex);
				
				stream.wInt(sd.Sector.SerializedIndex);

				stream.wBool(sd.IsFront);

				sd.ReadWrite(stream);
			}
		}

		// This serializes sectors
		private void WriteSectors(SerializerStream stream)
		{
			stream.wInt(sectors.Count);

			// Go for all sectors
			int index = 0;
			foreach(Sector s in sectors)
			{
				s.SerializedIndex = index++;

				s.ReadWrite(stream);
			}
		}

		#endregion

		#region ================== Deserialization

		// This serializes the MapSet
		private void Deserialize(MemoryStream stream)
		{
			stream.Seek(0, SeekOrigin.Begin);
			DeserializerStream deserializer = new DeserializerStream(stream);

			deserializer.Begin();

			// Read private data
			int c;
			deserializer.rInt(out lastsectorindex);
			deserializer.rInt(out c);
			indexholes = new List<int>(c);
			for(int i = 0; i < c; i++)
			{
				int index; deserializer.rInt(out index);
				indexholes.Add(index);
			}

			// Read map data
			Vertex[] verticesarray = ReadVertices(deserializer);
			Sector[] sectorsarray = ReadSectors(deserializer);
			Linedef[] linedefsarray = ReadLinedefs(deserializer, verticesarray);
			ReadSidedefs(deserializer, linedefsarray, sectorsarray);
			ReadThings(deserializer);

			deserializer.End();

			// Make table of sidedef indices
			sidedefindices = new Sidedef[sidedefs.Count];
			foreach(Sidedef sd in sidedefs)
				sidedefindices[sd.SerializedIndex] = sd;
				
			// Call PostDeserialize
			foreach(Sector s in sectors)
				s.PostDeserialize(this);
		}
		
		// This deserializes things
		private void ReadThings(DeserializerStream stream)
		{
			int c; stream.rInt(out c);

			// Go for all things
			for(int i = 0; i < c; i++)
			{
				Thing t = CreateThing();
				t.ReadWrite(stream);
			}
		}

		// This deserializes vertices
		private Vertex[] ReadVertices(DeserializerStream stream)
		{
			int c; stream.rInt(out c);

			Vertex[] array = new Vertex[c];

			// Go for all vertices
			for(int i = 0; i < c; i++)
			{
				array[i] = CreateVertex(stream);
			}

			return array;
		}

		// This deserializes linedefs
		private Linedef[] ReadLinedefs(DeserializerStream stream, Vertex[] verticesarray)
		{
			int c; stream.rInt(out c);

			Linedef[] array = new Linedef[c];

			// Go for all lines
			for(int i = 0; i < c; i++)
			{
				int start, end;
				
				stream.rInt(out start);

				stream.rInt(out end);

				array[i] = CreateLinedef(verticesarray[start], verticesarray[end], stream);
			}

			return array;
		}

		// This deserializes sidedefs
		private void ReadSidedefs(DeserializerStream stream, Linedef[] linedefsarray, Sector[] sectorsarray)
		{
			int c; stream.rInt(out c);

			// Go for all sidedefs
			for(int i = 0; i < c; i++)
			{
				int lineindex, sectorindex;
				bool front;
				
				stream.rInt(out lineindex);

				stream.rInt(out sectorindex);

				stream.rBool(out front);

				CreateSidedef(linedefsarray[lineindex], front, sectorsarray[sectorindex], stream);
			}
		}

		// This deserializes sectors
		private Sector[] ReadSectors(DeserializerStream stream)
		{
			int c; stream.rInt(out c);

			Sector[] array = new Sector[c];

			// Go for all sectors
			for(int i = 0; i < c; i++)
			{
				array[i] = CreateSector(stream);
			}

			return array;
		}

		#endregion

		#region ================== Updating

		// This updates all structures if needed
		public void Update()
		{
			// Update all!
			Update(true, true);
		}

		// This updates all structures if needed
		public void Update(bool dolines, bool dosectors)
		{
			// Update all linedefs
			if(dolines) foreach(Linedef l in linedefs) l.UpdateCache();

			// Update all sectors
			if(dosectors) foreach(Sector s in sectors) s.UpdateCache();
		}

		// This updates all structures after a
		// configuration or settings change
		public void UpdateConfiguration()
		{
			// Update all things
			foreach(Thing t in things) t.UpdateConfiguration();
		}
		
		#endregion

		#region ================== Selection
		
		// This checks a flag in a selection type
		private bool InSelectionType(SelectionType value, SelectionType bits)
		{
			return (value & bits) == bits;
		}
		
		// This converts the selection to a different selection
		// NOTE: This function uses the markings to convert the selection
		public void ConvertSelection(SelectionType target)
		{
			ConvertSelection(SelectionType.All, target);
		}
		
		// This converts the selection to a different selection
		// NOTE: This function uses the markings to convert the selection
		public void ConvertSelection(SelectionType source, SelectionType target)
		{
			ICollection<Linedef> lines;
			ICollection<Vertex> verts;
			
			ClearAllMarks(false);
			
			switch(target)
			{
				// Convert geometry selection to vertices only
				case SelectionType.Vertices:
					if(InSelectionType(source, SelectionType.Linedefs)) MarkSelectedLinedefs(true, true);
					if(InSelectionType(source, SelectionType.Sectors)) General.Map.Map.MarkSelectedSectors(true, true);
					verts = General.Map.Map.GetVerticesFromLinesMarks(true);
					foreach(Vertex v in verts) v.Selected = true;
					verts = General.Map.Map.GetVerticesFromSectorsMarks(true);
					foreach(Vertex v in verts) v.Selected = true;
					General.Map.Map.ClearSelectedSectors();
					General.Map.Map.ClearSelectedLinedefs();
					break;
					
				// Convert geometry selection to linedefs only
				case SelectionType.Linedefs:
					if(InSelectionType(source, SelectionType.Vertices)) MarkSelectedVertices(true, true);
					if(!InSelectionType(source, SelectionType.Linedefs)) ClearSelectedLinedefs();
					lines = General.Map.Map.LinedefsFromMarkedVertices(false, true, false);
					foreach(Linedef l in lines) l.Selected = true;
					if(InSelectionType(source, SelectionType.Sectors))
					{
						foreach(Sector s in General.Map.Map.Sectors)
						{
							if(s.Selected)
							{
								foreach(Sidedef sd in s.Sidedefs)
									sd.Line.Selected = true;
							}
						}
					}
					General.Map.Map.ClearSelectedSectors();
					General.Map.Map.ClearSelectedVertices();
					break;
					
				// Convert geometry selection to sectors only
				case SelectionType.Sectors:
					if(InSelectionType(source, SelectionType.Vertices)) MarkSelectedVertices(true, true);
					if(!InSelectionType(source, SelectionType.Linedefs)) ClearSelectedLinedefs();
					lines = LinedefsFromMarkedVertices(false, true, false);
					foreach(Linedef l in lines) l.Selected = true;
					ClearMarkedSectors(true);
					foreach(Linedef l in linedefs)
					{
						if(!l.Selected)
						{
							if(l.Front != null) l.Front.Sector.Marked = false;
							if(l.Back != null) l.Back.Sector.Marked = false;
						}
					}
					ClearSelectedLinedefs();
					ClearSelectedVertices();
					if(InSelectionType(source, SelectionType.Sectors))
					{
						foreach(Sector s in General.Map.Map.Sectors)
						{
							if(s.Marked || s.Selected)
							{
								s.Selected = true;
								foreach(Sidedef sd in s.Sidedefs)
									sd.Line.Selected = true;
							}
						}
					}
					else
					{
						foreach(Sector s in General.Map.Map.Sectors)
						{
							if(s.Marked)
							{
								s.Selected = true;
								foreach(Sidedef sd in s.Sidedefs)
									sd.Line.Selected = true;
							}
							else
							{
								s.Selected = false;
							}
						}
					}
					break;
					
				default:
					throw new ArgumentException("Unsupported selection target conversion");
					break;
			}
			
			// New selection type
			sel_type = target;
		}
		
		// This clears all selected items
		public void ClearAllSelected()
		{
			ClearSelectedVertices();
			ClearSelectedThings();
			ClearSelectedLinedefs();
			ClearSelectedSectors();
		}

		// This clears selected vertices
		public void ClearSelectedVertices()
		{
			sel_vertices.Clear();
			foreach(Vertex v in vertices) v.Selected = false;
		}

		// This clears selected things
		public void ClearSelectedThings()
		{
			sel_things.Clear();
			foreach(Thing t in things) t.Selected = false;
		}

		// This clears selected linedefs
		public void ClearSelectedLinedefs()
		{
			sel_linedefs.Clear();
			foreach(Linedef l in linedefs) l.Selected = false;
		}

		// This clears selected sectors
		public void ClearSelectedSectors()
		{
			sel_sectors.Clear();
			foreach(Sector s in sectors) s.Selected = false;
		}

		// Returns a collection of vertices that match a selected state
		public ICollection<Vertex> GetSelectedVertices(bool selected)
		{
			if(selected)
			{
				return new List<Vertex>(sel_vertices);
			}
			else
			{
				List<Vertex> list = new List<Vertex>(vertices.Count - sel_vertices.Count);
				foreach(Vertex v in vertices) if(!v.Selected) list.Add(v);
				return list;
			}
		}

		// Returns a collection of things that match a selected state
		public ICollection<Thing> GetSelectedThings(bool selected)
		{
			if(selected)
			{
				return new List<Thing>(sel_things);
			}
			else
			{
				List<Thing> list = new List<Thing>(things.Count - sel_things.Count);
				foreach(Thing t in things) if(!t.Selected) list.Add(t);
				return list;
			}
		}

		// Returns a collection of linedefs that match a selected state
		public ICollection<Linedef> GetSelectedLinedefs(bool selected)
		{
			if(selected)
			{
				return new List<Linedef>(sel_linedefs);
			}
			else
			{
				List<Linedef> list = new List<Linedef>(linedefs.Count - sel_linedefs.Count);
				foreach(Linedef l in linedefs) if(!l.Selected) list.Add(l);
				return list;
			}
		}

		// Returns a collection of sectors that match a selected state
		public ICollection<Sector> GetSelectedSectors(bool selected)
		{
			if(selected)
			{
				return new List<Sector>(sel_sectors);
			}
			else
			{
				List<Sector> list = new List<Sector>(sectors.Count - sel_sectors.Count);
				foreach(Sector s in sectors) if(!s.Selected) list.Add(s);
				return list;
			}
		}

		// This selects geometry based on the marking
		public void SelectMarkedGeometry(bool mark, bool select)
		{
			SelectMarkedVertices(mark, select);
			SelectMarkedLinedefs(mark, select);
			SelectMarkedSectors(mark, select);
			SelectMarkedThings(mark, select);
		}

		// This selects geometry based on the marking
		public void SelectMarkedVertices(bool mark, bool select)
		{
			foreach(Vertex v in vertices) if(v.Marked == mark) v.Selected = select;
		}

		// This selects geometry based on the marking
		public void SelectMarkedLinedefs(bool mark, bool select)
		{
			foreach(Linedef l in linedefs) if(l.Marked == mark) l.Selected = select;
		}

		// This selects geometry based on the marking
		public void SelectMarkedSectors(bool mark, bool select)
		{
			foreach(Sector s in sectors) if(s.Marked == mark) s.Selected = select;
		}

		// This selects geometry based on the marking
		public void SelectMarkedThings(bool mark, bool select)
		{
			foreach(Thing t in things) if(t.Marked == mark) t.Selected = select;
		}
		
		// This selects geometry by group
		public void SelectVerticesByGroup(int groupmask)
		{
			foreach(SelectableElement e in vertices) e.SelectByGroup(groupmask);
		}

		// This selects geometry by group
		public void SelectLinedefsByGroup(int groupmask)
		{
			foreach(SelectableElement e in linedefs) e.SelectByGroup(groupmask);
		}

		// This selects geometry by group
		public void SelectSectorsByGroup(int groupmask)
		{
			foreach(SelectableElement e in sectors) e.SelectByGroup(groupmask);
		}

		// This selects geometry by group
		public void SelectThingsByGroup(int groupmask)
		{
			foreach(SelectableElement e in things) e.SelectByGroup(groupmask);
		}
		
		// This adds the current selection to a group
		public void AddSelectionToGroup(int groupmask)
		{
			foreach(SelectableElement e in vertices)
				if(e.Selected) e.AddToGroup(groupmask);
			
			foreach(SelectableElement e in linedefs)
				if(e.Selected) e.AddToGroup(groupmask);
			
			foreach(SelectableElement e in sectors)
				if(e.Selected) e.AddToGroup(groupmask);
			
			foreach(SelectableElement e in things)
				if(e.Selected) e.AddToGroup(groupmask);
		}
		
		#endregion

		#region ================== Marking

		// This clears all marks
		public void ClearAllMarks(bool mark)
		{
			ClearMarkedVertices(mark);
			ClearMarkedThings(mark);
			ClearMarkedLinedefs(mark);
			ClearMarkedSectors(mark);
			ClearMarkedSidedefs(mark);
		}

		// This clears marked vertices
		public void ClearMarkedVertices(bool mark)
		{
			foreach(Vertex v in vertices) v.Marked = mark;
		}

		// This clears marked things
		public void ClearMarkedThings(bool mark)
		{
			foreach(Thing t in things) t.Marked = mark;
		}

		// This clears marked linedefs
		public void ClearMarkedLinedefs(bool mark)
		{
			foreach(Linedef l in linedefs) l.Marked = mark;
		}

		// This clears marked sidedefs
		public void ClearMarkedSidedefs(bool mark)
		{
			foreach(Sidedef s in sidedefs) s.Marked = mark;
		}

		// This clears marked sectors
		public void ClearMarkedSectors(bool mark)
		{
			foreach(Sector s in sectors) s.Marked = mark;
		}

		// This inverts all marks
		public void InvertAllMarks()
		{
			InvertMarkedVertices();
			InvertMarkedThings();
			InvertMarkedLinedefs();
			InvertMarkedSectors();
			InvertMarkedSidedefs();
		}

		// This inverts marked vertices
		public void InvertMarkedVertices()
		{
			foreach(Vertex v in vertices) v.Marked = !v.Marked;
		}

		// This inverts marked things
		public void InvertMarkedThings()
		{
			foreach(Thing t in things) t.Marked = !t.Marked;
		}

		// This inverts marked linedefs
		public void InvertMarkedLinedefs()
		{
			foreach(Linedef l in linedefs) l.Marked = !l.Marked;
		}

		// This inverts marked sidedefs
		public void InvertMarkedSidedefs()
		{
			foreach(Sidedef s in sidedefs) s.Marked = !s.Marked;
		}

		// This inverts marked sectors
		public void InvertMarkedSectors()
		{
			foreach(Sector s in sectors) s.Marked = !s.Marked;
		}

		// Returns a collection of vertices that match a marked state
		public List<Vertex> GetMarkedVertices(bool mark)
		{
			List<Vertex> list = new List<Vertex>(vertices.Count >> 1);
			foreach(Vertex v in vertices) if(v.Marked == mark) list.Add(v);
			return list;
		}

		// Returns a collection of things that match a marked state
		public List<Thing> GetMarkedThings(bool mark)
		{
			List<Thing> list = new List<Thing>(things.Count >> 1);
			foreach(Thing t in things) if(t.Marked == mark) list.Add(t);
			return list;
		}

		// Returns a collection of linedefs that match a marked state
		public List<Linedef> GetMarkedLinedefs(bool mark)
		{
			List<Linedef> list = new List<Linedef>(linedefs.Count >> 1);
			foreach(Linedef l in linedefs) if(l.Marked == mark) list.Add(l);
			return list;
		}

		// Returns a collection of sidedefs that match a marked state
		public List<Sidedef> GetMarkedSidedefs(bool mark)
		{
			List<Sidedef> list = new List<Sidedef>(sidedefs.Count >> 1);
			foreach(Sidedef s in sidedefs) if(s.Marked == mark) list.Add(s);
			return list;
		}

		// Returns a collection of sectors that match a marked state
		public List<Sector> GetMarkedSectors(bool mark)
		{
			List<Sector> list = new List<Sector>(sectors.Count >> 1);
			foreach(Sector s in sectors) if(s.Marked == mark) list.Add(s);
			return list;
		}

		// This creates a marking from selection
		public void MarkSelectedVertices(bool selected, bool mark)
		{
			foreach(Vertex v in vertices) if(v.Selected == selected) v.Marked = mark;
		}

		// This creates a marking from selection
		public void MarkSelectedLinedefs(bool selected, bool mark)
		{
			foreach(Linedef l in linedefs) if(l.Selected == selected) l.Marked = mark;
		}

		// This creates a marking from selection
		public void MarkSelectedSectors(bool selected, bool mark)
		{
			foreach(Sector s in sectors) if(s.Selected == selected) s.Marked = mark;
		}

		// This creates a marking from selection
		public void MarkSelectedThings(bool selected, bool mark)
		{
			foreach(Thing t in things) if(t.Selected == selected) t.Marked = mark;
		}

		/// <summary>
		/// This marks the front and back sidedefs on linedefs with the matching mark
		/// </summary>
		public void MarkSidedefsFromLinedefs(bool matchmark, bool setmark)
		{
			foreach(Linedef l in linedefs)
			{
				if(l.Marked == matchmark)
				{
					if(l.Front != null) l.Front.Marked = setmark;
					if(l.Back != null) l.Back.Marked = setmark;
				}
			}
		}

		/// <summary>
		/// This marks the sidedefs that make up the sectors with the matching mark
		/// </summary>
		public void MarkSidedefsFromSectors(bool matchmark, bool setmark)
		{
			foreach(Sidedef sd in sidedefs)
			{
				if(sd.Sector.Marked == matchmark) sd.Marked = setmark;
			}
		}

		/// <summary>
		/// Returns a collection of vertices that match a marked state on the linedefs
		/// </summary>
		public ICollection<Vertex> GetVerticesFromLinesMarks(bool mark)
		{
			List<Vertex> list = new List<Vertex>(vertices.Count >> 1);
			foreach(Vertex v in vertices)
			{
				foreach(Linedef l in v.Linedefs)
				{
					if(l.Marked == mark)
					{
						list.Add(v);
						break;
					}
				}
			}
			return list;
		}

		/// <summary>
		/// Returns a collection of vertices that match a marked state on the linedefs
		/// The difference with GetVerticesFromLinesMarks is that in this method
		/// ALL linedefs of a vertex must match the specified marked state.
		/// </summary>
		public ICollection<Vertex> GetVerticesFromAllLinesMarks(bool mark)
		{
			List<Vertex> list = new List<Vertex>(vertices.Count >> 1);
			foreach(Vertex v in vertices)
			{
				bool qualified = true;
				foreach(Linedef l in v.Linedefs)
				{
					if(l.Marked != mark)
					{
						qualified = false;
						break;
					}
				}
				if(qualified) list.Add(v);
			}
			return list;
		}

		/// <summary>
		/// Returns a collection of vertices that match a marked state on the linedefs
		/// </summary>
		public ICollection<Vertex> GetVerticesFromSectorsMarks(bool mark)
		{
			List<Vertex> list = new List<Vertex>(vertices.Count >> 1);
			foreach(Vertex v in vertices)
			{
				foreach(Linedef l in v.Linedefs)
				{
					if(((l.Front != null) && (l.Front.Sector.Marked == mark)) ||
						((l.Back != null) && (l.Back.Sector.Marked == mark)))
					{
						list.Add(v);
						break;
					}
				}
			}
			return list;
		}

		/// <summary>
		/// This marks all selected geometry, including sidedefs from sectors.
		/// When sidedefsfromsectors is true, then the sidedefs are marked according to the
		/// marked sectors. Otherwise the sidedefs are marked according to the marked linedefs.
		/// </summary>
		public void MarkAllSelectedGeometry(bool mark, bool sidedefsfromsectors)
		{
			General.Map.Map.ClearAllMarks(!mark);

			// Direct vertices
			General.Map.Map.MarkSelectedVertices(true, mark);

			// Direct linedefs
			General.Map.Map.MarkSelectedLinedefs(true, mark);

			// Linedefs from vertices
			// We do this before "vertices from lines" because otherwise we get lines marked that we didn't select
			ICollection<Linedef> lines = General.Map.Map.LinedefsFromMarkedVertices(!mark, mark, !mark);
			foreach(Linedef l in lines) l.Marked = mark;

			// Vertices from linedefs
			ICollection<Vertex> verts = General.Map.Map.GetVerticesFromLinesMarks(mark);
			foreach(Vertex v in verts) v.Marked = mark;

			// Mark sectors from linedefs (note: this must be the first to mark
			// sectors, because this clears the sector marks!)
			General.Map.Map.ClearMarkedSectors(mark);
			foreach(Linedef l in General.Map.Map.Linedefs)
			{
				if(!l.Selected)
				{
					if(l.Front != null) l.Front.Sector.Marked = !mark;
					if(l.Back != null) l.Back.Sector.Marked = !mark;
				}
			}

			// Direct sectors
			General.Map.Map.MarkSelectedSectors(true, mark);

			// Direct things
			General.Map.Map.MarkSelectedThings(true, mark);

			// Sidedefs from linedefs or sectors
			if(sidedefsfromsectors)
				General.Map.Map.MarkSidedefsFromSectors(true, mark);
			else
				General.Map.Map.MarkSidedefsFromLinedefs(true, mark);
		}
		
		#endregion

		#region ================== Indexing
		
		/// <summary>
		/// Returns the vertex at the specified index. Returns null when index is out of range. This is a O(n) operation.
		/// </summary>
		public Vertex GetVertexByIndex(int index)
		{
			if(index < vertices.Count)
				return General.GetByIndex<Vertex>(vertices, index);
			else
				return null;
		}

		/// <summary>
		/// Returns the linedef at the specified index. Returns null when index is out of range. This is a O(n) operation.
		/// </summary>
		public Linedef GetLinedefByIndex(int index)
		{
			if(index < linedefs.Count)
				return General.GetByIndex<Linedef>(linedefs, index);
			else
				return null;
		}

		/// <summary>
		/// Returns the sidedef at the specified index. Returns null when index is out of range. This is a O(n) operation.
		/// </summary>
		public Sidedef GetSidedefByIndex(int index)
		{
			if(index < sidedefs.Count)
				return General.GetByIndex<Sidedef>(sidedefs, index);
			else
				return null;
		}

		/// <summary>
		/// Returns the sector at the specified index. Returns null when index is out of range. This is a O(n) operation.
		/// </summary>
		public Sector GetSectorByIndex(int index)
		{
			if(index < sectors.Count)
				return General.GetByIndex<Sector>(sectors, index);
			else
				return null;
		}

		/// <summary>
		/// Returns the thing at the specified index. Returns null when index is out of range. This is a O(n) operation.
		/// </summary>
		public Thing GetThingByIndex(int index)
		{
			if(index < things.Count)
				return General.GetByIndex<Thing>(things, index);
			else
				return null;
		}

		/// <summary>
		/// Returns the index of the specified vertex. Returns -1 when the vertex is not in this map. This is a O(n) operation.
		/// </summary>
		public int GetIndexForVertex(Vertex v)
		{
			int index = 0;
			foreach(Vertex vn in vertices)
			{
				if(object.ReferenceEquals(vn, v)) return index;
				index++;
			}
			return -1;
		}

		/// <summary>
		/// Returns the index of the specified linedef. Returns -1 when the linedef is not in this map. This is a O(n) operation.
		/// </summary>
		public int GetIndexForLinedef(Linedef l)
		{
			int index = 0;
			foreach(Linedef ln in linedefs)
			{
				if(object.ReferenceEquals(ln, l)) return index;
				index++;
			}
			return -1;
		}

		/// <summary>
		/// Returns the index of the specified sidedef. Returns -1 when the sidedef is not in this map. This is a O(n) operation.
		/// </summary>
		public int GetIndexForSidedef(Sidedef sd)
		{
			int index = 0;
			foreach(Sidedef sn in sidedefs)
			{
				if(object.ReferenceEquals(sn, sd)) return index;
				index++;
			}
			return -1;
		}

		/// <summary>
		/// Returns the index of the specified sector. Returns -1 when the sector is not in this map. This is a O(n) operation.
		/// </summary>
		public int GetIndexForSector(Sector s)
		{
			int index = 0;
			foreach(Sector sn in sectors)
			{
				if(object.ReferenceEquals(sn, s)) return index;
				index++;
			}
			return -1;
		}

		/// <summary>
		/// Returns the index of the specified thing. Returns -1 when the thing is not in this map. This is a O(n) operation.
		/// </summary>
		public int GetIndexForThing(Thing t)
		{
			int index = 0;
			foreach(Thing tn in things)
			{
				if(object.ReferenceEquals(tn, t)) return index;
				index++;
			}
			return -1;
		}
		
		#endregion
		
		#region ================== Areas

		// This creates an initial, undefined area
		public static RectangleF CreateEmptyArea()
		{
			return new RectangleF(float.MaxValue / 2, float.MaxValue / 2, -float.MaxValue, -float.MaxValue);
		}
		
		// This creates an area from vertices
		public static RectangleF CreateArea(ICollection<Vertex> verts)
		{
			float l = float.MaxValue;
			float t = float.MaxValue;
			float r = float.MinValue;
			float b = float.MinValue;

			// Go for all vertices
			foreach(Vertex v in verts)
			{
				// Adjust boundaries by vertices
				if(v.Position.x < l) l = v.Position.x;
				if(v.Position.x > r) r = v.Position.x;
				if(v.Position.y < t) t = v.Position.y;
				if(v.Position.y > b) b = v.Position.y;
			}

			// Return a rect
			return new RectangleF(l, t, r - l, b - t);
		}

		// This increases and existing area with the given vertices
		public static RectangleF IncreaseArea(RectangleF area, ICollection<Vertex> verts)
		{
			float l = area.Left;
			float t = area.Top;
			float r = area.Right;
			float b = area.Bottom;
			
			// Go for all vertices
			foreach(Vertex v in verts)
			{
				// Adjust boundaries by vertices
				if(v.Position.x < l) l = v.Position.x;
				if(v.Position.x > r) r = v.Position.x;
				if(v.Position.y < t) t = v.Position.y;
				if(v.Position.y > b) b = v.Position.y;
			}
			
			// Return a rect
			return new RectangleF(l, t, r - l, b - t);
		}

		// This increases and existing area with the given vertices
		public static RectangleF IncreaseArea(RectangleF area, ICollection<Thing> things)
		{
			float l = area.Left;
			float t = area.Top;
			float r = area.Right;
			float b = area.Bottom;
			
			// Go for all vertices
			foreach(Thing th in things)
			{
				// Adjust boundaries by vertices
				if(th.Position.x < l) l = th.Position.x;
				if(th.Position.x > r) r = th.Position.x;
				if(th.Position.y < t) t = th.Position.y;
				if(th.Position.y > b) b = th.Position.y;
			}
			
			// Return a rect
			return new RectangleF(l, t, r - l, b - t);
		}

		// This increases and existing area with the given vertices
		public static RectangleF IncreaseArea(RectangleF area, ICollection<Vector2D> verts)
		{
			float l = area.Left;
			float t = area.Top;
			float r = area.Right;
			float b = area.Bottom;
			
			// Go for all vertices
			foreach(Vector2D v in verts)
			{
				// Adjust boundaries by vertices
				if(v.x < l) l = v.x;
				if(v.x > r) r = v.x;
				if(v.y < t) t = v.y;
				if(v.y > b) b = v.y;
			}
			
			// Return a rect
			return new RectangleF(l, t, r - l, b - t);
		}

		// This increases and existing area with the given vertices
		public static RectangleF IncreaseArea(RectangleF area, Vector2D vert)
		{
			float l = area.Left;
			float t = area.Top;
			float r = area.Right;
			float b = area.Bottom;
			
			// Adjust boundaries by vertices
			if(vert.x < l) l = vert.x;
			if(vert.x > r) r = vert.x;
			if(vert.y < t) t = vert.y;
			if(vert.y > b) b = vert.y;
			
			// Return a rect
			return new RectangleF(l, t, r - l, b - t);
		}

		// This creates an area from linedefs
		public static RectangleF CreateArea(ICollection<Linedef> lines)
		{
			float l = float.MaxValue;
			float t = float.MaxValue;
			float r = float.MinValue;
			float b = float.MinValue;

			// Go for all linedefs
			foreach(Linedef ld in lines)
			{
				// Adjust boundaries by vertices
				if(ld.Start.Position.x < l) l = ld.Start.Position.x;
				if(ld.Start.Position.x > r) r = ld.Start.Position.x;
				if(ld.Start.Position.y < t) t = ld.Start.Position.y;
				if(ld.Start.Position.y > b) b = ld.Start.Position.y;
				if(ld.End.Position.x < l) l = ld.End.Position.x;
				if(ld.End.Position.x > r) r = ld.End.Position.x;
				if(ld.End.Position.y < t) t = ld.End.Position.y;
				if(ld.End.Position.y > b) b = ld.End.Position.y;
			}

			// Return a rect
			return new RectangleF(l, t, r - l, b - t);
		}
		
		// This filters lines by a square area
		public static ICollection<Linedef> FilterByArea(ICollection<Linedef> lines, ref RectangleF area)
		{
			ICollection<Linedef> newlines = new List<Linedef>(lines.Count);
			
			// Go for all lines
			foreach(Linedef l in lines)
			{
				// Check the cs field bits
				if((GetCSFieldBits(l.Start, ref area) & GetCSFieldBits(l.End, ref area)) == 0)
				{
					// The line could be in the area
					newlines.Add(l);
				}
			}
			
			// Return result
			return newlines;
		}

		// This returns the cohen-sutherland field bits for a vertex in a rectangle area
		private static int GetCSFieldBits(Vertex v, ref RectangleF area)
		{
			int bits = 0;
			if(v.Position.y < area.Top) bits |= 0x01;
			if(v.Position.y > area.Bottom) bits |= 0x02;
			if(v.Position.x < area.Left) bits |= 0x04;
			if(v.Position.x > area.Right) bits |= 0x08;
			return bits;
		}

		// This filters vertices by a square area
		public static ICollection<Vertex> FilterByArea(ICollection<Vertex> verts, ref RectangleF area)
		{
			ICollection<Vertex> newverts = new List<Vertex>(verts.Count);

			// Go for all verts
			foreach(Vertex v in verts)
			{
				// Within rect?
				if((v.Position.x >= area.Left) &&
				   (v.Position.x <= area.Right) &&
				   (v.Position.y >= area.Top) &&
				   (v.Position.y <= area.Bottom))
				{
					// The vertex is in the area
					newverts.Add(v);
				}
			}

			// Return result
			return newverts;
		}

		#endregion

		#region ================== Stitching

		/// <summary>
		/// Stitches marked geometry with non-marked geometry. Returns the number of stitches made.
		/// </summary>
		public int StitchGeometry()
		{
			ICollection<Linedef> movinglines;
			ICollection<Linedef> fixedlines;
			ICollection<Vertex> nearbyfixedverts;
			ICollection<Vertex> movingverts;
			ICollection<Vertex> fixedverts;
			RectangleF editarea;
			int stitches = 0;
			int stitchundo;
			
			// Find vertices
			movingverts = General.Map.Map.GetMarkedVertices(true);
			fixedverts = General.Map.Map.GetMarkedVertices(false);
			
			// Find lines that moved during the drag
			movinglines = LinedefsFromMarkedVertices(false, true, true);
			
			// Find all non-moving lines
			fixedlines = LinedefsFromMarkedVertices(true, false, false);
			
			// Determine area in which we are editing
			editarea = MapSet.CreateArea(movinglines);
			editarea = MapSet.IncreaseArea(editarea, movingverts);
			editarea.Inflate(1.0f, 1.0f);
			
			// Join nearby vertices
			stitches += MapSet.JoinVertices(fixedverts, movingverts, true, MapSet.STITCH_DISTANCE);
			
			// Update cached values of lines because we need their length/angle
			Update(true, false);
			
			// Split moving lines with unselected vertices
			nearbyfixedverts = MapSet.FilterByArea(fixedverts, ref editarea);
			stitches += MapSet.SplitLinesByVertices(movinglines, nearbyfixedverts, MapSet.STITCH_DISTANCE, movinglines);
			
			// Split non-moving lines with selected vertices
			fixedlines = MapSet.FilterByArea(fixedlines, ref editarea);
			stitches += MapSet.SplitLinesByVertices(fixedlines, movingverts, MapSet.STITCH_DISTANCE, movinglines);
			
			// Remove looped linedefs
			stitches += MapSet.RemoveLoopedLinedefs(movinglines);
			
			// Join overlapping lines
			stitches += MapSet.JoinOverlappingLines(movinglines);
			
			return stitches;
		}
		
		#endregion
		
		#region ================== Geometry Tools

		// This removes any virtual sectors in the map
		// Returns the number of sectors removed
		public int RemoveVirtualSectors()
		{
			int count = 0;
			LinkedListNode<Sector> n = sectors.First;
			
			// Go for all sectors
			while(n != null)
			{
				LinkedListNode<Sector> nn = n.Next;
				
				// Remove when virtual
				if(n.Value.Fields.ContainsKey(VIRTUAL_SECTOR_FIELD))
				{
					n.Value.Dispose();
					count++;
				}
				
				n = nn;
			}

			return count;
		}
		
		// This joins overlapping lines together
		// Returns the number of joins made
		public static int JoinOverlappingLines(ICollection<Linedef> lines)
		{
			int joinsdone = 0;
			bool joined;
			
			do
			{
				// No joins yet
				joined = false;

				// Go for all the lines
				foreach(Linedef l1 in lines)
				{
					// Check if these vertices have lines that overlap
					foreach(Linedef l2 in l1.Start.Linedefs)
					{
						// Sharing vertices?
						if((l1.End == l2.End) ||
						   (l1.End == l2.Start))
						{
							// Not the same line?
							if(l1 != l2)
							{
								// Merge these two linedefs
								//while(lines.Remove(l1));
								//l1.Join(l2);
								while(lines.Remove(l2)) ;
								l2.Join(l1);
								joinsdone++;
								joined = true;
								break;
							}
						}
					}
					
					// Will have to restart when joined
					if(joined) break;
					
					// Check if these vertices have lines that overlap
					foreach(Linedef l2 in l1.End.Linedefs)
					{
						// Sharing vertices?
						if((l1.Start == l2.End) ||
						   (l1.Start == l2.Start))
						{
							// Not the same line?
							if(l1 != l2)
							{
								// Merge these two linedefs
								//while(lines.Remove(l1));
								//l1.Join(l2);
								while(lines.Remove(l2)) ;
								l2.Join(l1);
								joinsdone++;
								joined = true;
								break;
							}
						}
					}
					
					// Will have to restart when joined
					if(joined) break;
				}
			}
			while(joined);

			// Return result
			return joinsdone;
		}
		
		// This removes looped linedefs (linedefs which reference the same vertex for start and end)
		// Returns the number of linedefs removed
		public static int RemoveLoopedLinedefs(ICollection<Linedef> lines)
		{
			int linesremoved = 0;
			bool removedline;

			do
			{
				// Nothing removed yet
				removedline = false;

				// Go for all the lines
				foreach(Linedef l in lines)
				{
					// Check if referencing the same vertex twice
					if(l.Start == l.End)
					{
						// Remove this line
						while(lines.Remove(l));
						l.Dispose();
						linesremoved++;
						removedline = true;
						break;
					}
				}
			}
			while(removedline);

			// Return result
			return linesremoved;
		}

		// This joins nearby vertices from two collections. This does NOT join vertices
		// within the same collection, only if they exist in both collections.
		// The vertex from the second collection is moved to match the first vertex.
		// When keepsecond is true, the vertex in the second collection is kept,
		// otherwise the vertex in the first collection is kept.
		// Returns the number of joins made
		public static int JoinVertices(ICollection<Vertex> set1, ICollection<Vertex> set2, bool keepsecond, float joindist)
		{
			float joindist2 = joindist * joindist;
			int joinsdone = 0;
			bool joined;

			do
			{
				// No joins yet
				joined = false;

				// Go for all vertices in the first set
				foreach(Vertex v1 in set1)
				{
					// Go for all vertices in the second set
					foreach(Vertex v2 in set2)
					{
						// Check if vertices are close enough
						if(v1.DistanceToSq(v2.Position) <= joindist2)
						{
							// Check if not the same vertex
							if(v1 != v2)
							{
								// Move the second vertex to match the first
								v2.Move(v1.Position);
								
								// Check which one to keep
								if(keepsecond)
								{
									// Join the first into the second
									// Second is kept, first is removed
									v1.Join(v2);
									set1.Remove(v1);
									set2.Remove(v1);
								}
								else
								{
									// Join the second into the first
									// First is kept, second is removed
									v2.Join(v1);
									set1.Remove(v2);
									set2.Remove(v2);
								}
								
								// Count the join
								joinsdone++;
								joined = true;
								break;
							}
						}
					}

					// Will have to restart when joined
					if(joined) break;
				}
			}
			while(joined);

			// Return result
			return joinsdone;
		}

		// This corrects lines that have a back sidedef but no front
		// sidedef by flipping them. Returns the number of flips made.
		public static int FlipBackwardLinedefs(ICollection<Linedef> lines)
		{
			int flipsdone = 0;
			
			// Examine all lines
			foreach(Linedef l in lines)
			{
				// Back side but no front side?
				if((l.Back != null) && (l.Front == null))
				{
					// Flip that linedef!
					l.FlipVertices();
					l.FlipSidedefs();
					flipsdone++;
				}
			}

			// Return result
			return flipsdone;
		}

		// This splits the given lines with the given vertices
		// All affected lines will be added to changedlines
		// Returns the number of splits made
		public static int SplitLinesByVertices(ICollection<Linedef> lines, ICollection<Vertex> verts, float splitdist, ICollection<Linedef> changedlines)
		{
			float splitdist2 = splitdist * splitdist;
			int splitsdone = 0;
			bool splitted;

			do
			{
				// No split yet
				splitted = false;
				
				// Go for all the lines
				foreach(Linedef l in lines)
				{
					// Go for all the vertices
					foreach(Vertex v in verts)
					{
						// Check if v is close enough to l for splitting
						if(l.DistanceToSq(v.Position, true) <= splitdist2)
						{
							// Line is not already referencing v?
							Vector2D deltastart = l.Start.Position - v.Position;
							Vector2D deltaend = l.End.Position - v.Position;
							if(((Math.Abs(deltastart.x) > 0.001f) ||
							    (Math.Abs(deltastart.y) > 0.001f)) &&
							   ((Math.Abs(deltaend.x) > 0.001f) ||
							    (Math.Abs(deltaend.y) > 0.001f)))
							{
								// Split line l with vertex v
								Linedef nl = l.Split(v);

								// Add the new line to the list
								lines.Add(nl);

								// Both lines must be updated because their new length
								// is relevant for next iterations!
								l.UpdateCache();
								nl.UpdateCache();

								// Add both lines to changedlines
								if(changedlines != null)
								{
									changedlines.Add(l);
									changedlines.Add(nl);
								}

								// Count the split
								splitsdone++;
								splitted = true;
								break;
							}
						}
					}

					// Will have to restart when splitted
					// TODO: If we make (linked) lists from the collections first,
					// we don't have to restart when splitted?
					if(splitted) break;
				}
			}
			while(splitted);

			// Return result
			return splitsdone;
		}
		
		// This finds the side closest to the specified position
		public static Sidedef NearestSidedef(ICollection<Sidedef> selection, Vector2D pos)
		{
			Sidedef closest = null;
			float distance = float.MaxValue;
			
			// Go for all sidedefs in selection
			foreach(Sidedef sd in selection)
			{
				// Calculate distance and check if closer than previous find
				float d = sd.Line.SafeDistanceToSq(pos, true);
				if(d < distance)
				{
					// This one is closer
					closest = sd;
					distance = d;
				}
			}
			
			// Return result
			return closest;
		}
		
		// This finds the line closest to the specified position
		public static Linedef NearestLinedef(ICollection<Linedef> selection, Vector2D pos)
		{
			Linedef closest = null;
			float distance = float.MaxValue;

			// Go for all linedefs in selection
			foreach(Linedef l in selection)
			{
				// Calculate distance and check if closer than previous find
				float d = l.SafeDistanceToSq(pos, true);
				if(d < distance)
				{
					// This one is closer
					closest = l;
					distance = d;
				}
			}

			// Return result
			return closest;
		}

		// This finds the line closest to the specified position
		public static Linedef NearestLinedefRange(ICollection<Linedef> selection, Vector2D pos, float maxrange)
		{
			Linedef closest = null;
			float distance = float.MaxValue;
			float maxrangesq = maxrange * maxrange;
			float d;

			// Go for all linedefs in selection
			foreach(Linedef l in selection)
			{
				// Calculate distance and check if closer than previous find
				d = l.SafeDistanceToSq(pos, true);
				if((d <= maxrangesq) && (d < distance))
				{
					// This one is closer
					closest = l;
					distance = d;
				}
			}
			
			// Return result
			return closest;
		}

		// This finds the vertex closest to the specified position
		public static Vertex NearestVertex(ICollection<Vertex> selection, Vector2D pos)
		{
			Vertex closest = null;
			float distance = float.MaxValue;
			float d;
			
			// Go for all vertices in selection
			foreach(Vertex v in selection)
			{
				// Calculate distance and check if closer than previous find
				d = v.DistanceToSq(pos);
				if(d < distance)
				{
					// This one is closer
					closest = v;
					distance = d;
				}
			}

			// Return result
			return closest;
		}

		// This finds the thing closest to the specified position
		public static Thing NearestThing(ICollection<Thing> selection, Vector2D pos)
		{
			Thing closest = null;
			float distance = float.MaxValue;
			float d;

			// Go for all things in selection
			foreach(Thing t in selection)
			{
				// Calculate distance and check if closer than previous find
				d = t.DistanceToSq(pos);
				if(d < distance)
				{
					// This one is closer
					closest = t;
					distance = d;
				}
			}

			// Return result
			return closest;
		}

		// This finds the vertex closest to the specified position
		public static Vertex NearestVertexSquareRange(ICollection<Vertex> selection, Vector2D pos, float maxrange)
		{
			RectangleF range = RectangleF.FromLTRB(pos.x - maxrange, pos.y - maxrange, pos.x + maxrange, pos.y + maxrange);
			Vertex closest = null;
			float distance = float.MaxValue;
			float d;

			// Go for all vertices in selection
			foreach(Vertex v in selection)
			{
				// Within range?
				if((v.Position.x >= range.Left) && (v.Position.x <= range.Right))
				{
					if((v.Position.y >= range.Top) && (v.Position.y <= range.Bottom))
					{
						// Close than previous find?
						d = Math.Abs(v.Position.x - pos.x) + Math.Abs(v.Position.y - pos.y);
						if(d < distance)
						{
							// This one is closer
							closest = v;
							distance = d;
						}
					}
				}
			}

			// Return result
			return closest;
		}

		// This finds the thing closest to the specified position
		public static Thing NearestThingSquareRange(ICollection<Thing> selection, Vector2D pos, float maxrange)
		{
			RectangleF range = RectangleF.FromLTRB(pos.x - maxrange, pos.y - maxrange, pos.x + maxrange, pos.y + maxrange);
			Thing closest = null;
			float distance = float.MaxValue;
			float d;

			// Go for all vertices in selection
			foreach(Thing t in selection)
			{
				// Within range?
				if((t.Position.x >= (range.Left - t.Size)) && (t.Position.x <= (range.Right + t.Size)))
				{
					if((t.Position.y >= (range.Top - t.Size)) && (t.Position.y <= (range.Bottom + t.Size)))
					{
						// Close than previous find?
						d = Math.Abs(t.Position.x - pos.x) + Math.Abs(t.Position.y - pos.y);
						if(d < distance)
						{
							// This one is closer
							closest = t;
							distance = d;
						}
					}
				}
			}

			// Return result
			return closest;
		}
		
		#endregion

		#region ================== Tools

		// This snaps all vertices to the map format accuracy
		public void SnapAllToAccuracy()
		{
			foreach(Vertex v in vertices) v.SnapToAccuracy();
			foreach(Thing t in things) t.SnapToAccuracy();
		}
		
		// This returns the next unused tag number
		public int GetNewTag()
		{
			Dictionary<int, bool> usedtags = new Dictionary<int, bool>();
			
			// Check all sectors
			foreach(Sector s in sectors) usedtags[s.Tag] = true;
			
			// Check all lines
			foreach(Linedef l in linedefs) usedtags[l.Tag] = true;

			// Check all things
			foreach(Thing t in things) usedtags[t.Tag] = true;
			
			// Now find the first unused index
			for(int i = 1; i <= General.Map.FormatInterface.MaxTag; i++)
				if(!usedtags.ContainsKey(i)) return i;
			
			// Problem: all tags used!
			// Lets ignore this problem for now, who needs 65-thousand tags?!
			return 0;
		}
		
		// This makes a list of lines related to marked vertices
		// A line is unstable when one vertex is marked and the other isn't.
		public ICollection<Linedef> LinedefsFromMarkedVertices(bool includeunselected, bool includestable, bool includeunstable)
		{
			List<Linedef> list = new List<Linedef>((linedefs.Count / 2) + 1);
			
			// Go for all lines
			foreach(Linedef l in linedefs)
			{
				// Check if this is to be included
				if((includestable && (l.Start.Marked && l.End.Marked)) ||
				   (includeunstable && (l.Start.Marked ^ l.End.Marked)) ||
				   (includeunselected && (!l.Start.Marked && !l.End.Marked)))
				{
					// Add to list
					list.Add(l);
				}
			}

			// Return result
			return list;
		}

		// This makes a list of unstable lines from the given vertices.
		// A line is unstable when one vertex is selected and the other isn't.
		public static ICollection<Linedef> UnstableLinedefsFromVertices(ICollection<Vertex> verts)
		{
			Dictionary<Linedef, Linedef> lines = new Dictionary<Linedef, Linedef>();

			// Go for all vertices
			foreach(Vertex v in verts)
			{
				// Go for all lines
				foreach(Linedef l in v.Linedefs)
				{
					// If the line exists in the list
					if(lines.ContainsKey(l))
					{
						// Remove it
						lines.Remove(l);
					}
					// Otherwise add it
					else
					{
						// Add the line
						lines.Add(l, l);
					}
				}
			}
			
			// Return result
			return new List<Linedef>(lines.Values);
		}
		
		// This finds the line closest to the specified position
		public Linedef NearestLinedef(Vector2D pos) { return MapSet.NearestLinedef(linedefs, pos); }

		// This finds the line closest to the specified position
		public Linedef NearestLinedefRange(Vector2D pos, float maxrange) { return MapSet.NearestLinedefRange(linedefs, pos, maxrange); }

		// This finds the vertex closest to the specified position
		public Vertex NearestVertex(Vector2D pos) { return MapSet.NearestVertex(vertices, pos); }

		// This finds the vertex closest to the specified position
		public Vertex NearestVertexSquareRange(Vector2D pos, float maxrange) { return MapSet.NearestVertexSquareRange(vertices, pos, maxrange); }

		// This finds the thing closest to the specified position
		public Thing NearestThingSquareRange(Vector2D pos, float maxrange) { return MapSet.NearestThingSquareRange(things, pos, maxrange); }

		// This finds the closest unselected linedef that is not connected to the given vertex
		public Linedef NearestUnselectedUnreferencedLinedef(Vector2D pos, float maxrange, Vertex v, out float distance)
		{
			Linedef closest = null;
			distance = float.MaxValue;
			float maxrangesq = maxrange * maxrange;
			float d;

			// Go for all linedefs in selection
			foreach(Linedef l in linedefs)
			{
				// Calculate distance and check if closer than previous find
				d = l.SafeDistanceToSq(pos, true);
				if((d <= maxrangesq) && (d < distance))
				{
					// Check if not selected

					// Check if linedef is not connected to v
					if((l.Start != v) && (l.End != v))
					{
						// This one is closer
						closest = l;
						distance = d;
					}
				}
			}

			// Return result
			return closest;
		}
		
		// This performs sidedefs compression
		// Note: Only use this for saving, because this messes up the expected data structure horribly.
		internal void CompressSidedefs()
		{
			Dictionary<uint, List<Sidedef>> storedsides = new Dictionary<uint, List<Sidedef>>(sidedefs.Count);
			int originalsidescount = sidedefs.Count;
			double starttime = General.Clock.GetCurrentTime();
			
			LinkedListNode<Sidedef> sn = sidedefs.First;
			while(sn != null)
			{
				Sidedef stored = null;
				LinkedListNode<Sidedef> nextsn = sn.Next;
				
				// Check if checksum is stored
				bool samesidedef = false;
				uint checksum = sn.Value.GetChecksum();
				bool checksumstored = storedsides.ContainsKey(checksum);
				if(checksumstored)
				{
					List<Sidedef> othersides = storedsides[checksum];
					foreach(Sidedef os in othersides)
					{
						// They must be in the same sector
						if(sn.Value.Sector == os.Sector)
						{
							// Check if sidedefs are really the same
							stored = os;
							MemoryStream sidemem = new MemoryStream(1024);
							SerializerStream sidedata = new SerializerStream(sidemem);
							MemoryStream othermem = new MemoryStream(1024);
							SerializerStream otherdata = new SerializerStream(othermem);
							sn.Value.ReadWrite(sidedata);
							os.ReadWrite(otherdata);
							if(sidemem.Length == othermem.Length)
							{
								samesidedef = true;
								sidemem.Seek(0, SeekOrigin.Begin);
								othermem.Seek(0, SeekOrigin.Begin);
								for(int i = 0; i < sidemem.Length; i++)
								{
									if(sidemem.ReadByte() != othermem.ReadByte())
									{
										samesidedef = false;
										break;
									}
								}
							}
							
							if(samesidedef) break;
						}
					}
				}

				// Same sidedef?
				if(samesidedef)
				{
					// Replace with stored sidedef
					bool isfront = sn.Value.IsFront;
					sn.Value.Line.DetachSidedef(sn.Value);
					if(isfront)
						sn.Value.Line.AttachFront(stored);
					else
						sn.Value.Line.AttachBack(stored);
					
					// Remove the sidedef
					sn.Value.ChangeSector(null);
					sidedefs.Remove(sn);
				}
				else
				{
					// Store this new one
					if(checksumstored)
					{
						storedsides[checksum].Add(sn.Value);
					}
					else
					{
						List<Sidedef> newlist = new List<Sidedef>(4);
						newlist.Add(sn.Value);
						storedsides.Add(checksum, newlist);
					}
				}
				
				// Next
				sn = nextsn;
			}

			// Output info
			double endtime = General.Clock.GetCurrentTime();
			double deltatimesec = (endtime - starttime) / 1000.0d;
			float ratio = 100.0f - (((float)sidedefs.Count / (float)originalsidescount) * 100.0f);
			General.WriteLogLine("Sidedefs compressed: " + sidedefs.Count + " remaining out of " + originalsidescount + " (" + ratio.ToString("########0.00") + "%) in " + deltatimesec.ToString("########0.00") + " seconds");
		}

		// This converts flags and activations to UDMF fields
		internal void TranslateToUDMF()
		{
			foreach(Linedef l in linedefs) l.TranslateToUDMF();
			foreach(Thing t in things) t.TranslateToUDMF();
		}

		// This converts UDMF fields back into flags and activations
		// NOTE: Only converts the marked items
		internal void TranslateFromUDMF()
		{
			foreach(Linedef l in linedefs) if(l.Marked) l.TranslateFromUDMF();
			foreach(Thing t in things) if(t.Marked) t.TranslateFromUDMF();
		}
		
		// This removes unused vertices
		public void RemoveUnusedVertices()
		{
			LinkedListNode<Vertex> vn, vc;
			
			// Go for all vertices
			vn = vertices.First;
			while(vn != null)
			{
				vc = vn;
				vn = vc.Next;
				if(vc.Value.Linedefs.Count == 0) vertices.Remove(vc);
			}
		}
		
		#endregion
	}
}
