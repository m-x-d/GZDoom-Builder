
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
using System.IO;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Geometry;

#endregion

namespace CodeImp.DoomBuilder.IO
{
	internal class DoomMapSetIO : MapSetIO
	{
		#region ================== Constants

		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		public DoomMapSetIO(WAD wad) : base(wad)
		{
		}

		#endregion
		
		#region ================== Reading

		// This reads a map from the file and returns a MapSet
		public override MapSet Read(MapSet map, string mapname)
		{
			int firstindex;
			Dictionary<int, Vertex> vertexlink;
			Dictionary<int, Sector> sectorlink;
			
			// Find the index where first map lump begins
			firstindex = wad.FindLumpIndex(mapname) + 1;

			// Read vertices
			vertexlink = ReadVertices(map, firstindex);

			// Read sectors
			sectorlink = ReadSectors(map, firstindex);

			// Read linedefs and sidedefs
			ReadLinedefs(map, firstindex, vertexlink, sectorlink);

			// Read things
			ReadThings(map, firstindex);

			// Return result;
			return map;
		}

		// This reads the THINGS from WAD file
		private void ReadThings(MapSet map, int firstindex)
		{
			MemoryStream mem;
			BinaryReader reader;
			int num, i, x, y, type, flags;
			float angle;
			Thing t;
			
			// Get the lump from wad file
			Lump lump = wad.FindLump("THINGS", firstindex);
			if(lump == null) throw new Exception("Could not find required lump THINGS!");
			
			// Prepare to read the items
			mem = new MemoryStream(lump.Stream.ReadAllBytes());
			num = (int)lump.Stream.Length / 10;
			reader = new BinaryReader(mem);
			
			// Read items from the lump
			for(i = 0; i < num; i++)
			{
				// Read properties from stream
				x = reader.ReadInt16();
				y = reader.ReadInt16();
				angle = reader.ReadInt16();		// TODO: Fix this!
				type = reader.ReadUInt16();
				flags = reader.ReadUInt16();
				
				// Create new item
				t = map.CreateThing();
				t.Update(type, new Vector3D(x, y, 0f), angle, flags, 0, 0, Thing.EMPTY_ARGS);
				t.DetermineSector();
			}

			// Done
			mem.Dispose();
		}

		// This reads the VERTICES from WAD file
		// Returns a lookup table with indices
		private Dictionary<int, Vertex> ReadVertices(MapSet map, int firstindex)
		{
			MemoryStream mem;
			Dictionary<int, Vertex> link;
			BinaryReader reader;
			int num, i, x, y;
			Vertex v;
			
			// Get the lump from wad file
			Lump lump = wad.FindLump("VERTEXES", firstindex);
			if(lump == null) throw new Exception("Could not find required lump VERTEXES!");

			// Prepare to read the items
			mem = new MemoryStream(lump.Stream.ReadAllBytes());
			num = (int)lump.Stream.Length / 4;
			reader = new BinaryReader(mem);

			// Create lookup table
			link = new Dictionary<int, Vertex>(num);

			// Read items from the lump
			for(i = 0; i < num; i++)
			{
				// Read properties from stream
				x = reader.ReadInt16();
				y = reader.ReadInt16();

				// Create new item
				v = map.CreateVertex(x, y);
				
				// Add it to the lookup table
				link.Add(i, v);
			}

			// Done
			mem.Dispose();

			// Return lookup table
			return link;
		}

		// This reads the SECTORS from WAD file
		// Returns a lookup table with indices
		private Dictionary<int, Sector> ReadSectors(MapSet map, int firstindex)
		{
			MemoryStream mem;
			Dictionary<int, Sector> link;
			BinaryReader reader;
			int num, i, hfloor, hceil, bright, special, tag;
			string tfloor, tceil;
			Sector s;

			// Get the lump from wad file
			Lump lump = wad.FindLump("SECTORS", firstindex);
			if(lump == null) throw new Exception("Could not find required lump SECTORS!");

			// Prepare to read the items
			mem = new MemoryStream(lump.Stream.ReadAllBytes());
			num = (int)lump.Stream.Length / 26;
			reader = new BinaryReader(mem);

			// Create lookup table
			link = new Dictionary<int, Sector>(num);

			// Read items from the lump
			for(i = 0; i < num; i++)
			{
				// Read properties from stream
				hfloor = reader.ReadInt16();
				hceil = reader.ReadInt16();
				tfloor = Lump.MakeNormalName(reader.ReadBytes(8), WAD.ENCODING);
				tceil = Lump.MakeNormalName(reader.ReadBytes(8), WAD.ENCODING);
				bright = reader.ReadInt16();
				special = reader.ReadUInt16();
				tag = reader.ReadUInt16();
				
				// Create new item
				s = map.CreateSector();
				s.Update(hfloor, hceil, tfloor, tceil, special, tag);

				// Add it to the lookup table
				link.Add(i, s);
			}

			// Done
			mem.Dispose();

			// Return lookup table
			return link;
		}
		
		// This reads the LINEDEFS and SIDEDEFS from WAD file
		private void ReadLinedefs(MapSet map, int firstindex,
			Dictionary<int, Vertex> vertexlink, Dictionary<int, Sector> sectorlink)
		{
			MemoryStream linedefsmem, sidedefsmem;
			BinaryReader readline, readside;
			Lump linedefslump, sidedefslump;
			int num, i, offsetx, offsety, v1, v2;
			int s1, s2, flags, action, tag, sc;
			string thigh, tmid, tlow;
			Linedef l;
			Sidedef s;

			// Get the linedefs lump from wad file
			linedefslump = wad.FindLump("LINEDEFS", firstindex);
			if(linedefslump == null) throw new Exception("Could not find required lump LINEDEFS!");

			// Get the sidedefs lump from wad file
			sidedefslump = wad.FindLump("SIDEDEFS", firstindex);
			if(sidedefslump == null) throw new Exception("Could not find required lump SIDEDEFS!");

			// Prepare to read the items
			linedefsmem = new MemoryStream(linedefslump.Stream.ReadAllBytes());
			sidedefsmem = new MemoryStream(sidedefslump.Stream.ReadAllBytes());
			num = (int)linedefslump.Stream.Length / 14;
			readline = new BinaryReader(linedefsmem);
			readside = new BinaryReader(sidedefsmem);

			// Read items from the lump
			for(i = 0; i < num; i++)
			{
				// Read properties from stream
				v1 = readline.ReadUInt16();
				v2 = readline.ReadUInt16();
				flags = readline.ReadUInt16();
				action = readline.ReadUInt16();
				tag = readline.ReadUInt16();
				s1 = readline.ReadUInt16();
				s2 = readline.ReadUInt16();
				
				// Create new item
				l = map.CreateLinedef(vertexlink[v1], vertexlink[v2]);
				l.Update(flags, tag, action, Linedef.EMPTY_ARGS);

				// Line has a front side?
				if(s1 != ushort.MaxValue)
				{
					// Read front sidedef
					sidedefsmem.Seek(s1 * 30, SeekOrigin.Begin);
					offsetx = readside.ReadInt16();
					offsety = readside.ReadInt16();
					thigh = Lump.MakeNormalName(readside.ReadBytes(8), WAD.ENCODING);
					tmid = Lump.MakeNormalName(readside.ReadBytes(8), WAD.ENCODING);
					tlow = Lump.MakeNormalName(readside.ReadBytes(8), WAD.ENCODING);
					sc = readside.ReadUInt16();

					// Create front sidedef
					s = map.CreateSidedef(l, true, sectorlink[sc]);
					s.Update(offsetx, offsety, thigh, tmid, tlow);
				}

				// Line has a back side?
				if(s2 != ushort.MaxValue)
				{
					// Read back sidedef
					sidedefsmem.Seek(s2 * 30, SeekOrigin.Begin);
					offsetx = readside.ReadInt16();
					offsety = readside.ReadInt16();
					thigh = Lump.MakeNormalName(readside.ReadBytes(8), WAD.ENCODING);
					tmid = Lump.MakeNormalName(readside.ReadBytes(8), WAD.ENCODING);
					tlow = Lump.MakeNormalName(readside.ReadBytes(8), WAD.ENCODING);
					sc = readside.ReadUInt16();

					// Create back sidedef
					s = map.CreateSidedef(l, false, sectorlink[sc]);
					s.Update(offsetx, offsety, thigh, tmid, tlow);
				}
			}

			// Done
			linedefsmem.Dispose();
			sidedefsmem.Dispose();
		}
		
		#endregion

		#region ================== Writing

		// This writes a MapSet to the file
		public override void Write(MapSet map, string mapname, int position)
		{
		}
		
		#endregion
	}
}
