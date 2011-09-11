
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
using System.Drawing;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Geometry;
using CodeImp.DoomBuilder.Rendering;
using CodeImp.DoomBuilder.Config;   // villsa

#endregion

namespace CodeImp.DoomBuilder.IO
{
	internal class Doom64MapSetIO : MapSetIO
	{
		#region ================== Constants

		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		public Doom64MapSetIO(WAD wad, MapManager manager) : base(wad, manager)
		{
		}

		#endregion

        #region ================== Variables

        internal List<Lights> light;

        #endregion

        #region ================== Properties

        public override int MaxSidedefs { get { return ushort.MaxValue; } }
		public override int MaxVertices { get { return ushort.MaxValue; } }
		public override int MaxLinedefs { get { return ushort.MaxValue; } }
		public override int MaxSectors { get { return ushort.MaxValue; } }
		public override int MaxThings { get { return ushort.MaxValue; } }
		public override int MinTextureOffset { get { return short.MinValue; } }
		public override int MaxTextureOffset { get { return short.MaxValue; } }
		public override int VertexDecimals { get { return 0; } }
		public override string DecimalsFormat { get { return "0"; } }
		public override bool HasLinedefTag { get { return true; } }
		public override bool HasThingTag { get { return true; } }
		public override bool HasThingAction { get { return false; } }
		public override bool HasCustomFields { get { return false; } }
		public override bool HasThingHeight { get { return true; } }
		public override bool HasActionArgs { get { return false; } }
		public override bool HasMixedActivations { get { return false; } }
		public override bool HasPresetActivations { get { return false; } }
        public override bool HasBuiltInActivations { get { return false; } }
		public override bool HasNumericLinedefFlags { get { return true; } }
		public override bool HasNumericThingFlags { get { return true; } }
		public override bool HasNumericLinedefActivations { get { return true; } }
		public override int MaxTag { get { return ushort.MaxValue; } }
		public override int MinTag { get { return ushort.MinValue; } }
        public override int MaxAction { get { return 511; } }
        public override int MinAction { get { return ushort.MinValue; } }
        public override int MaxArgument { get { return 0; } }
        public override int MinArgument { get { return 0; } }
		public override int MaxEffect { get { return ushort.MaxValue; } }
		public override int MinEffect { get { return ushort.MinValue; } }
		public override int MaxBrightness { get { return short.MaxValue; } }
		public override int MinBrightness { get { return short.MinValue; } }
		public override int MaxThingType { get { return ushort.MaxValue; } }
		public override int MinThingType { get { return ushort.MinValue; } }
		public override double MaxCoordinate { get { return (double)short.MaxValue; } }
		public override double MinCoordinate { get { return (double)short.MinValue; } }
        public override bool InDoom64Mode { get { return true; } } // villsa
		
		#endregion

        #region ================== Light Functions

        internal void AddLight(Sector s, Lights slight)
        {
            Lights l;

            l = slight;
            if(s.Tag != 0)
                l.tag = (UInt16)s.Tag;
            slight = l;

            if (slight.color.r == slight.color.g &&
                slight.color.r == slight.color.b &&
                slight.tag == 0)
                return;

            if (!light.Contains(slight))
                light.Add(slight);
        }

        internal UInt16 GetLight(Sector s, Lights slight)
        {
            UInt16 i = 0;
            Lights lgt;

            lgt = slight;
            lgt.tag = (UInt16)s.Tag;
            slight = lgt;

            if (slight.color.r == slight.color.g &&
                slight.color.r == slight.color.b)
            {
                return slight.color.r;
            }

            foreach (Lights l in light)
            {
                if (l.color.r == slight.color.r &&
                    l.color.g == slight.color.g &&
                    l.color.b == slight.color.b &&
                    l.tag == slight.tag)
                {
                    return (UInt16)(256 + i);
                }
                i++;
            }

            return 0;
        }

        #endregion

		#region ================== Reading

		// This reads a map from the file and returns a MapSet
		public override MapSet Read(MapSet map, string mapname)
		{
			int firstindex;
			Dictionary<int, Vertex> vertexlink;
			Dictionary<int, Sector> sectorlink;

            //ReadMapInfo(map, mapname); // DON'T USE
			
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

            // Read macros
            ReadMacros(map, firstindex);
			
			// Remove unused vertices
			map.RemoveUnusedVertices();
			
			// Return result;
			return map;
		}

        // This reads the map info header from WAD file
        /*private void ReadMapInfo(MapSet map, string mapname)
        {
            MemoryStream mem;
            BinaryReader reader;
            PixelColor color;

            if (wad.Lumps[wad.FindLumpIndex(mapname)].Length <= 0)
                return;

            Lump lump = wad.FindLump(mapname);
            mem = new MemoryStream(lump.Stream.ReadAllBytes());
            reader = new BinaryReader(mem);

            General.Map.Map.MapInfo.MapName = General.Map.Map.MapInfo.ReadString(reader);

            for (int i = 0; i < MapInfo.INTERMISSION_LINES; i++)
                map.MapInfo.IntermissionText[i] = map.MapInfo.ReadString(reader);

            General.Map.Map.MapInfo.OverrideSky = reader.ReadBoolean();
            General.Map.Map.MapInfo.Finale = reader.ReadBoolean();
            General.Map.Map.MapInfo.Type = reader.ReadByte();

            color = new PixelColor();

            color.b = reader.ReadByte();
            color.g = reader.ReadByte();
            color.r = reader.ReadByte();
            color.a = reader.ReadByte();

            General.Map.Map.MapInfo.FogColor = color;

            color.b = reader.ReadByte();
            color.g = reader.ReadByte();
            color.r = reader.ReadByte();
            color.a = reader.ReadByte();

            General.Map.Map.MapInfo.TopSkyColor = color;

            color.b = reader.ReadByte();
            color.g = reader.ReadByte();
            color.r = reader.ReadByte();
            color.a = reader.ReadByte();

            General.Map.Map.MapInfo.LowerSkyColor = color;

            General.Map.Map.MapInfo.MusicID = reader.ReadUInt16();

            // Done
            mem.Dispose();
        }*/

		// This reads the THINGS from WAD file
		private void ReadThings(MapSet map, int firstindex)
		{
			MemoryStream mem;
			BinaryReader reader;
			int num, i, tag, z, x, y, type, flags;
			Dictionary<string, bool> stringflags;
			float angle;
			Thing t;
			
			// Get the lump from wad file
			Lump lump = wad.FindLump("THINGS", firstindex);
			if(lump == null) throw new Exception("Could not find required lump THINGS!");
			
			// Prepare to read the items
			mem = new MemoryStream(lump.Stream.ReadAllBytes());
            num = (int)lump.Stream.Length / 14;
			reader = new BinaryReader(mem);
			
			// Read items from the lump
			map.SetCapacity(0, 0, 0, 0, map.Things.Count + num);
			for(i = 0; i < num; i++)
			{
				// Read properties from stream
				x = reader.ReadInt16();
				y = reader.ReadInt16();
				z = reader.ReadInt16();
				angle = Angle2D.DoomToReal(reader.ReadInt16());
				type = reader.ReadUInt16();
				flags = reader.ReadUInt16();
                tag = reader.ReadUInt16();

				// Make string flags
				stringflags = new Dictionary<string, bool>();
				foreach(KeyValuePair<string, string> f in manager.Config.ThingFlags)
				{
					int fnum;
					if(int.TryParse(f.Key, out fnum)) stringflags[f.Key] = ((flags & fnum) == fnum);
				}
				
				// Create new item
				t = map.CreateThing();
                t.Update(type, x, y, z, angle, stringflags, tag, 0, new int[Thing.NUM_ARGS]);
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
			num = (int)lump.Stream.Length / 8;
			reader = new BinaryReader(mem);

			// Create lookup table
			link = new Dictionary<int, Vertex>(num);

			// Read items from the lump
			map.SetCapacity(map.Vertices.Count + num, 0, 0, 0, 0);
			for(i = 0; i < num; i++)
			{
				// Read properties from stream
				x = reader.ReadInt32() / 65536;
				y = reader.ReadInt32() / 65536;

				// Create new item
				v = map.CreateVertex(new Vector2D((float)x, (float)y));
				
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
            MemoryStream lightmem;
			Dictionary<int, Sector> link;
			BinaryReader reader;
            BinaryReader lightreader;
			int num, i, hfloor, hceil, special, tag, flags;
            int lightnum;
            int[] colors = new int[5];
			uint tfloor, tceil;
            Lights[] lightColors;
            Dictionary<string, bool> stringflags;
			Sector s;
            string fname = "-";
            string cname = "-";

			// Get the lump from wad file
			Lump lump = wad.FindLump("SECTORS", firstindex);
			if(lump == null) throw new Exception("Could not find required lump SECTORS!");

            // Get the lights lump from wad file
            Lump lightlump = wad.FindLump("LIGHTS", firstindex);
            if (lightlump == null) throw new Exception("Could not find required lump LIGHTS!");

			// Prepare to read the items
			mem = new MemoryStream(lump.Stream.ReadAllBytes());
            lightmem = new MemoryStream(lightlump.Stream.ReadAllBytes());
			num = (int)lump.Stream.Length / 24;
            lightnum = (int)lightlump.Stream.Length / 6;
			reader = new BinaryReader(mem);
            lightreader = new BinaryReader(lightmem);

			// Create lookup table
			link = new Dictionary<int, Sector>(num);

            // Get light table
            lightColors = new Lights[lightnum];
            for (i = 0; i < lightnum; i++)
            {
                lightColors[i].color.r = lightreader.ReadByte();
                lightColors[i].color.g = lightreader.ReadByte();
                lightColors[i].color.b = lightreader.ReadByte();
                lightColors[i].color.a = lightreader.ReadByte();
                lightColors[i].tag = lightreader.ReadUInt16();
            }

			// Read items from the lump
			map.SetCapacity(0, 0, 0, map.Sectors.Count + num, 0);
			for(i = 0; i < num; i++)
			{
				// Read properties from stream
				hfloor = reader.ReadInt16();
				hceil = reader.ReadInt16();
                tfloor = reader.ReadUInt16();
                tceil = reader.ReadUInt16();
                colors[0] = reader.ReadUInt16();
                colors[1] = reader.ReadUInt16();
                colors[2] = reader.ReadUInt16();
                colors[3] = reader.ReadUInt16();
                colors[4] = reader.ReadUInt16();
				special = reader.ReadUInt16();
				tag = reader.ReadInt16();
                flags = reader.ReadUInt16();

                // Make string flags
                stringflags = new Dictionary<string, bool>();
                foreach (string f in manager.Config.SortedSectorFlags)
                {
                    int fnum;
                    if (int.TryParse(f, out fnum)) stringflags[f] = ((flags & fnum) == fnum);
                }
				
				// Create new item
				s = map.CreateSector();

                s.HashFloor = tfloor;
                s.HashCeiling = tceil;

                /*for (int j = 0; j < General.Map.TextureHashKey.Count; j++)
                {
                    if (tfloor == General.Map.TextureHashKey[j])
                    {
                        fname = General.Map.TextureHashName[j];
                        s.HashFloor = General.Map.TextureHashKey[j];
                        break;
                    }
                }

                for (int j = 0; j < General.Map.TextureHashKey.Count; j++)
                {
                    if (tceil == General.Map.TextureHashKey[j])
                    {
                        cname = General.Map.TextureHashName[j];
                        s.HashCeiling = General.Map.TextureHashKey[j];
                        break;
                    }
                }*/

                s.Update(stringflags, hfloor, hceil, fname, cname, special, tag, lightColors, colors);
                //s.Update(stringflags, hfloor, hceil, General.Map.Config.D64TextureIndex[tfloor].Title,
                //    General.Map.Config.D64TextureIndex[tceil].Title, special, tag, lightColors, colors);

				// Add it to the lookup table
				link.Add(i, s);
			}

			// Done
			mem.Dispose();
            lightmem.Dispose();

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
			int num, numsides, i, offsetx, offsety, v1, v2;
			int s1, s2, action, sc, tag;
            uint flags;
			Dictionary<string, bool> stringflags;
			uint thigh, tmid, tlow;
			Linedef l;
			Sidedef s;
            string hname = "-";
            string lname = "-";
            string mname = "-";

			// Get the linedefs lump from wad file
			linedefslump = wad.FindLump("LINEDEFS", firstindex);
			if(linedefslump == null) throw new Exception("Could not find required lump LINEDEFS!");

			// Get the sidedefs lump from wad file
			sidedefslump = wad.FindLump("SIDEDEFS", firstindex);
			if(sidedefslump == null) throw new Exception("Could not find required lump SIDEDEFS!");

			// Prepare to read the items
			linedefsmem = new MemoryStream(linedefslump.Stream.ReadAllBytes());
			sidedefsmem = new MemoryStream(sidedefslump.Stream.ReadAllBytes());
			num = (int)linedefslump.Stream.Length / 16;
			numsides = (int)sidedefslump.Stream.Length / 12;
			readline = new BinaryReader(linedefsmem);
			readside = new BinaryReader(sidedefsmem);

			// Read items from the lump
			map.SetCapacity(0, map.Linedefs.Count + num, map.Sidedefs.Count + numsides, 0, 0);
			for(i = 0; i < num; i++)
			{
				// Read properties from stream
				v1 = readline.ReadUInt16();
				v2 = readline.ReadUInt16();
				flags = readline.ReadUInt32();
				action = readline.ReadUInt16();
                tag = readline.ReadInt16();
				s1 = readline.ReadUInt16();
				s2 = readline.ReadUInt16();
				
				// Make string flags
				stringflags = new Dictionary<string, bool>();
				foreach(string f in manager.Config.SortedLinedefFlags)
				{
					uint fnum;
					if(uint.TryParse(f, out fnum)) stringflags[f] = ((flags & fnum) == fnum);
				}
				
				// Create new linedef
				if(vertexlink.ContainsKey(v1) && vertexlink.ContainsKey(v2))
				{
					// Check if not zero-length
					if(Vector2D.ManhattanDistance(vertexlink[v1].Position, vertexlink[v2].Position) > 0.0001f)
					{
						l = map.CreateLinedef(vertexlink[v1], vertexlink[v2]);
                        l.Update(stringflags, action, tag, action & 511, new int[5]);
						l.UpdateCache();

						// Line has a front side?
						if(s1 != ushort.MaxValue)
						{
							// Read front sidedef
							sidedefsmem.Seek(s1 * 12, SeekOrigin.Begin);
							if((s1 * 12L) <= (sidedefsmem.Length - 12L))
							{
								offsetx = readside.ReadInt16();
								offsety = readside.ReadInt16();
                                thigh = readside.ReadUInt16();
                                tlow = readside.ReadUInt16();
                                tmid = readside.ReadUInt16();
								sc = readside.ReadUInt16();

								// Create front sidedef
								if(sectorlink.ContainsKey(sc))
								{
                                    /*for (int j = 0; j < General.Map.TextureHashKey.Count; j++)
                                    {
                                        if (thigh == General.Map.TextureHashKey[j])
                                        {
                                            if (j == 0)
                                            {
                                                hname = "-";
                                                break;
                                            }

                                            hname = General.Map.TextureHashName[j];
                                            break;
                                        }
                                    }

                                    for (int j = 0; j < General.Map.TextureHashKey.Count; j++)
                                    {
                                        if (tlow == General.Map.TextureHashKey[j])
                                        {
                                            if (j == 0)
                                            {
                                                lname = "-";
                                                break;
                                            }

                                            lname = General.Map.TextureHashName[j];
                                            break;
                                        }
                                    }

                                    for (int j = 0; j < General.Map.TextureHashKey.Count; j++)
                                    {
                                        if (tmid == General.Map.TextureHashKey[j])
                                        {
                                            if (j == 0)
                                            {
                                                mname = "-";
                                                break;
                                            }

                                            mname = General.Map.TextureHashName[j];
                                            break;
                                        }
                                    }*/

									s = map.CreateSidedef(l, true, sectorlink[sc]);

                                    s.HashTexHigh = thigh;
                                    s.HashTexMid = tmid;
                                    s.HashTexLow = tlow;

                                    s.Update(offsetx, offsety, hname, mname, lname);
                                        //General.Map.Config.D64TextureIndex[thigh].Title,
                                        //General.Map.Config.D64TextureIndex[tmid].Title,
                                        //General.Map.Config.D64TextureIndex[tlow].Title);
								}
								else
								{
									General.ErrorLogger.Add(ErrorType.Warning, "Sidedef " + s1 + " references invalid sector " + sc + ". Sidedef has been removed.");
								}
							}
							else
							{
								General.ErrorLogger.Add(ErrorType.Warning, "Linedef " + i + " references invalid sidedef " + s1 + ". Sidedef has been removed.");
							}
						}

						// Line has a back side?
						if(s2 != ushort.MaxValue)
						{
							// Read back sidedef
							sidedefsmem.Seek(s2 * 12, SeekOrigin.Begin);
							if((s2 * 12L) <= (sidedefsmem.Length - 12L))
							{
								offsetx = readside.ReadInt16();
								offsety = readside.ReadInt16();
                                thigh = readside.ReadUInt16();
                                tlow = readside.ReadUInt16();
                                tmid = readside.ReadUInt16();
								sc = readside.ReadUInt16();

								// Create back sidedef
								if(sectorlink.ContainsKey(sc))
								{
                                    /*for (int j = 0; j < General.Map.TextureHashKey.Count; j++)
                                    {
                                        if (thigh == General.Map.TextureHashKey[j])
                                        {
                                            if (j == 0)
                                            {
                                                hname = "-";
                                                break;
                                            }

                                            hname = General.Map.TextureHashName[j];
                                            break;
                                        }
                                    }

                                    for (int j = 0; j < General.Map.TextureHashKey.Count; j++)
                                    {
                                        if (tlow == General.Map.TextureHashKey[j])
                                        {
                                            if (j == 0)
                                            {
                                                lname = "-";
                                                break;
                                            }

                                            lname = General.Map.TextureHashName[j];
                                            break;
                                        }
                                    }

                                    for (int j = 0; j < General.Map.TextureHashKey.Count; j++)
                                    {
                                        if (tmid == General.Map.TextureHashKey[j])
                                        {
                                            if (j == 0)
                                            {
                                                mname = "-";
                                                break;
                                            }

                                            mname = General.Map.TextureHashName[j];
                                            break;
                                        }
                                    }*/

									s = map.CreateSidedef(l, false, sectorlink[sc]);

                                    s.HashTexHigh = thigh;
                                    s.HashTexMid = tmid;
                                    s.HashTexLow = tlow;

                                    s.Update(offsetx, offsety, hname, mname, lname);
                                        //General.Map.Config.D64TextureIndex[thigh].Title,
                                        //General.Map.Config.D64TextureIndex[tmid].Title,
                                        //General.Map.Config.D64TextureIndex[tlow].Title);
								}
								else
								{
									General.ErrorLogger.Add(ErrorType.Warning, "Sidedef " + s2 + " references invalid sector " + sc + ". Sidedef has been removed.");
								}
							}
							else
							{
								General.ErrorLogger.Add(ErrorType.Warning, "Linedef " + i + " references invalid sidedef " + s2 + ". Sidedef has been removed.");
							}
						}
					}
					else
					{
						General.ErrorLogger.Add(ErrorType.Warning, "Linedef " + i + " is zero-length. Linedef has been removed.");
					}
				}
				else
				{
					General.ErrorLogger.Add(ErrorType.Warning, "Linedef " + i + " references one or more invalid vertices. Linedef has been removed.");
				}
			}

			// Done
			linedefsmem.Dispose();
			sidedefsmem.Dispose();
		}

        // This reads the MACROS from WAD file
        private void ReadMacros(MapSet map, int firstindex)
        {
            MemoryStream mem;
            BinaryReader reader;
            int count;
            int setcount;
            int specials;
            int type, tag, batch;
            int i = 0;

            // Get the lump from wad file
            Lump lump = wad.FindLump("MACROS", firstindex);
            if (lump == null) throw new Exception("Could not find required lump MACROS!");

            if (lump.Length <= 4)
                return;

            // Prepare to read the items
            mem = new MemoryStream(lump.Stream.ReadAllBytes());
            reader = new BinaryReader(mem);

            count = reader.ReadUInt16();
            specials = reader.ReadUInt16();

            map.NumMacros = count;

            if (count > 0)
            {
                // Read items from the lump
                for(i = 0; i < count; i++)
                {
                    setcount = reader.ReadUInt16();
                    map.Macros[i] = new Macro(setcount);
                    for (int j = 0; j < setcount + 1; j++)
                    {
                        batch = reader.ReadInt16();
                        tag = reader.ReadInt16();
                        type = reader.ReadInt16();

                        if(j < setcount)    // avoid reading the 'dummy' macro
                            map.Macros[i].Set(j, type, batch, tag);
                    }
                }
            }

            // Done
            mem.Dispose();
        }
		
		#endregion

		#region ================== Writing

		// This writes a MapSet to the file
		public override void Write(MapSet map, string mapname, int position)
		{
			Dictionary<Vertex, int> vertexids = new Dictionary<Vertex,int>();
			Dictionary<Sidedef, int> sidedefids = new Dictionary<Sidedef,int>();
			Dictionary<Sector, int> sectorids = new Dictionary<Sector,int>();

            //WriteMapInfo(map, mapname);   // DON'T USE

			// First index everything
			foreach(Vertex v in map.Vertices) vertexids.Add(v, vertexids.Count);
			foreach(Sidedef sd in map.Sidedefs) sidedefids.Add(sd, sidedefids.Count);
			foreach(Sector s in map.Sectors) sectorids.Add(s, sectorids.Count);
			
			// Write lumps to wad (note the backwards order because they
			// are all inserted at position+1 when not found)
            WriteMacros(map, position, manager.Config.MapLumpNames);
            WriteLights(map, position, manager.Config.MapLumpNames);
			WriteSectors(map, position, manager.Config.MapLumpNames);
			WriteVertices(map, position, manager.Config.MapLumpNames);
			WriteSidedefs(map, position, manager.Config.MapLumpNames, sectorids);
			WriteLinedefs(map, position, manager.Config.MapLumpNames, sidedefids, vertexids);
			WriteThings(map, position, manager.Config.MapLumpNames);
		}

        // This writes the map information to the map header lump
        /*private void WriteMapInfo(MapSet map, string mapname)
        {
            MemoryStream mem;
            BinaryWriter writer;
            Lump lump;
            string name = General.Map.Map.MapInfo.MapName;
            int musid = General.Map.Map.MapInfo.MusicID;
            PixelColor fogcolor = General.Map.Map.MapInfo.FogColor;
            PixelColor topcolor = General.Map.Map.MapInfo.TopSkyColor;
            PixelColor lowercolor = General.Map.Map.MapInfo.LowerSkyColor;
            string[] intertext = General.Map.Map.MapInfo.IntermissionText;
            bool overridesky = General.Map.Map.MapInfo.OverrideSky;
            bool finale = General.Map.Map.MapInfo.Finale;
            byte type = General.Map.Map.MapInfo.Type;

            mem = new MemoryStream();
            writer = new BinaryWriter(mem, WAD.ENCODING);

            General.Map.Map.MapInfo.WriteString(writer, name, 16);

            for (int i = 0; i < MapInfo.INTERMISSION_LINES; i++)
                map.MapInfo.WriteString(writer, intertext[i], 28);

            writer.Write((bool)overridesky);
            writer.Write((bool)finale);
            writer.Write((byte)type);
            writer.Write((int)fogcolor.ToInt());
            writer.Write((int)topcolor.ToInt());
            writer.Write((int)lowercolor.ToInt());
            writer.Write((Int16)musid);

            MapManager.RemoveSpecificLump(wad, mapname, 0, MapManager.TEMP_MAP_HEADER, manager.Config.MapLumpNames);
            lump = wad.Insert(mapname, 0, (int)mem.Length);
            lump.Stream.Seek(0, SeekOrigin.Begin);
            mem.WriteTo(lump.Stream);
            mem.Flush();
        }*/

		// This writes the THINGS to WAD file
		private void WriteThings(MapSet map, int position, IDictionary maplumps)
		{
			MemoryStream mem;
			BinaryWriter writer;
			Lump lump;
			int insertpos;
			int flags;
			
			// Create memory to write to
			mem = new MemoryStream();
			writer = new BinaryWriter(mem, WAD.ENCODING);
			
			// Go for all things
			foreach(Thing t in map.Things)
			{
				// Convert flags
				flags = 0;
				foreach(KeyValuePair<string, bool> f in t.Flags)
				{
					int fnum;
					if(f.Value && int.TryParse(f.Key, out fnum)) flags |= fnum;
				}

				// Write properties to stream
				// Write properties to stream
				writer.Write((Int16)t.Position.x);
				writer.Write((Int16)t.Position.y);
				writer.Write((Int16)t.Position.z);
				writer.Write((Int16)Angle2D.RealToDoom(t.Angle));
				writer.Write((UInt16)t.Type);
				writer.Write((UInt16)flags);
                writer.Write((UInt16)t.Tag);
			}
			
			// Find insert position and remove old lump
			insertpos = MapManager.RemoveSpecificLump(wad, "THINGS", position, MapManager.TEMP_MAP_HEADER, maplumps);
			if(insertpos == -1) insertpos = position + 1;
			if(insertpos > wad.Lumps.Count) insertpos = wad.Lumps.Count;
			
			// Create the lump from memory
			lump = wad.Insert("THINGS", insertpos, (int)mem.Length);
			lump.Stream.Seek(0, SeekOrigin.Begin);
			mem.WriteTo(lump.Stream);
		}

		// This writes the VERTEXES to WAD file
		private void WriteVertices(MapSet map, int position, IDictionary maplumps)
		{
			MemoryStream mem;
			BinaryWriter writer;
			Lump lump;
			int insertpos;

			// Create memory to write to
			mem = new MemoryStream();
			writer = new BinaryWriter(mem, WAD.ENCODING);

			// Go for all vertices
			foreach(Vertex v in map.Vertices)
			{
				// Write properties to stream
				writer.Write((Int32)((int)Math.Round(v.Position.x) * 65536));
				writer.Write((Int32)((int)Math.Round(v.Position.y) * 65536));
			}

			// Find insert position and remove old lump
			insertpos = MapManager.RemoveSpecificLump(wad, "VERTEXES", position, MapManager.TEMP_MAP_HEADER, maplumps);
			if(insertpos == -1) insertpos = position + 1;
			if(insertpos > wad.Lumps.Count) insertpos = wad.Lumps.Count;

			// Create the lump from memory
			lump = wad.Insert("VERTEXES", insertpos, (int)mem.Length);
			lump.Stream.Seek(0, SeekOrigin.Begin);
			mem.WriteTo(lump.Stream);
		}

		// This writes the LINEDEFS to WAD file
		private void WriteLinedefs(MapSet map, int position, IDictionary maplumps, IDictionary<Sidedef, int> sidedefids, IDictionary<Vertex, int> vertexids)
		{
			MemoryStream mem;
			BinaryWriter writer;
			Lump lump;
			ushort sid;
			int insertpos;
            uint flags;
			
			// Create memory to write to
			mem = new MemoryStream();
			writer = new BinaryWriter(mem, WAD.ENCODING);

			// Go for all lines
			foreach(Linedef l in map.Linedefs)
			{
				// Convert flags
				flags = 0;
				foreach(KeyValuePair<string, bool> f in l.Flags)
				{
					uint fnum;
                    if (f.Value && uint.TryParse(f.Key, out fnum)) flags |= fnum;
				}
				
				// Write properties to stream
				writer.Write((UInt16)vertexids[l.Start]);
				writer.Write((UInt16)vertexids[l.End]);
				writer.Write((UInt32)flags);
				writer.Write((UInt16)(l.Action | l.Activate));
                writer.Write((UInt16)l.Tag);

				// Front sidedef
				if(l.Front == null) sid = ushort.MaxValue;
					else sid = (UInt16)sidedefids[l.Front];
				writer.Write(sid);

				// Back sidedef
				if(l.Back == null) sid = ushort.MaxValue;
					else sid = (UInt16)sidedefids[l.Back];
				writer.Write(sid);
			}

			// Find insert position and remove old lump
			insertpos = MapManager.RemoveSpecificLump(wad, "LINEDEFS", position, MapManager.TEMP_MAP_HEADER, maplumps);
			if(insertpos == -1) insertpos = position + 1;
			if(insertpos > wad.Lumps.Count) insertpos = wad.Lumps.Count;

			// Create the lump from memory
			lump = wad.Insert("LINEDEFS", insertpos, (int)mem.Length);
			lump.Stream.Seek(0, SeekOrigin.Begin);
			mem.WriteTo(lump.Stream);
		}

		// This writes the SIDEDEFS to WAD file
		private void WriteSidedefs(MapSet map, int position, IDictionary maplumps, IDictionary<Sector, int> sectorids)
		{
			MemoryStream mem;
			BinaryWriter writer;
			Lump lump;
            int low, mid, top;
			int insertpos;

			// Create memory to write to
			mem = new MemoryStream();
			writer = new BinaryWriter(mem, WAD.ENCODING);

			// Go for all sidedefs
			foreach(Sidedef sd in map.Sidedefs)
			{
                string ht;
                string lt;
                string mt;

				// Write properties to stream
				writer.Write((Int16)sd.OffsetX);
				writer.Write((Int16)sd.OffsetY);

                low = 0;
                mid = 0;
                top = 0;

                ht = sd.HighTexture;
                lt = sd.LowTexture;
                mt = sd.MiddleTexture;

                if (ht == "-")
                    ht = "?";

                if (lt == "-")
                    lt = "?";

                if (mt == "-")
                    mt = "?";

                for (int i = 0; i < General.Map.TextureHashKey.Count; i++)
                {
                    if (ht == General.Map.TextureHashName[i])
                    {
                        top = (int)General.Map.TextureHashKey[i];
                        break;
                    }
                }

                for (int i = 0; i < General.Map.TextureHashKey.Count; i++)
                {
                    if (lt == General.Map.TextureHashName[i])
                    {
                        low = (int)General.Map.TextureHashKey[i];
                        break;
                    }
                }

                for (int i = 0; i < General.Map.TextureHashKey.Count; i++)
                {
                    if (mt == General.Map.TextureHashName[i])
                    {
                        mid = (int)General.Map.TextureHashKey[i];
                        break;
                    }
                }

                /*foreach (TextureIndexInfo tp in General.Map.Config.D64TextureIndex)
                {
                    if (sd.HighTexture == tp.Title)
                        top = tp.Index;

                    if (sd.LowTexture == tp.Title)
                        low = tp.Index;

                    if (sd.MiddleTexture == tp.Title)
                        mid = tp.Index;
                }*/

                writer.Write((Int16)top);
                writer.Write((Int16)low);
                writer.Write((Int16)mid);
				writer.Write((UInt16)sectorids[sd.Sector]);
			}

			// Find insert position and remove old lump
			insertpos = MapManager.RemoveSpecificLump(wad, "SIDEDEFS", position, MapManager.TEMP_MAP_HEADER, maplumps);
			if(insertpos == -1) insertpos = position + 1;
			if(insertpos > wad.Lumps.Count) insertpos = wad.Lumps.Count;

			// Create the lump from memory
			lump = wad.Insert("SIDEDEFS", insertpos, (int)mem.Length);
			lump.Stream.Seek(0, SeekOrigin.Begin);
			mem.WriteTo(lump.Stream);
        }

        // This writes the LIGHTS to WAD file
        private void WriteLights(MapSet map, int position, IDictionary maplumps)
        {
            MemoryStream mem;
            BinaryWriter writer;
            Lump lump;
            int insertpos;

            // Create memory to write to
            mem = new MemoryStream();
            writer = new BinaryWriter(mem, WAD.ENCODING);

            light = new List<Lights>();

            foreach (Sector s in map.Sectors)
            {
                AddLight(s, s.CeilColor);
                AddLight(s, s.FloorColor);
                AddLight(s, s.ThingColor);
                AddLight(s, s.TopColor);
                AddLight(s, s.LowerColor);
            }

            foreach (Lights l in light)
            {
                // Write properties to stream
                writer.Write((byte)l.color.r);
                writer.Write((byte)l.color.g);
                writer.Write((byte)l.color.b);
                writer.Write((byte)0);
                writer.Write((UInt16)l.tag);
            }

            // Find insert position and remove old lump
            insertpos = MapManager.RemoveSpecificLump(wad, "LIGHTS", position, MapManager.TEMP_MAP_HEADER, maplumps);
            if (insertpos == -1) insertpos = position + 1;
            if (insertpos > wad.Lumps.Count) insertpos = wad.Lumps.Count;

            // Create the lump from memory
            lump = wad.Insert("LIGHTS", insertpos, (int)mem.Length);
            lump.Stream.Seek(0, SeekOrigin.Begin);
            mem.WriteTo(lump.Stream);
        }

		// This writes the SECTORS to WAD file
		private void WriteSectors(MapSet map, int position, IDictionary maplumps)
		{
			MemoryStream mem;
			BinaryWriter writer;
			Lump lump;
            int flr, ceil;
			int insertpos;
            int flags;

			// Create memory to write to
			mem = new MemoryStream();
			writer = new BinaryWriter(mem, WAD.ENCODING);

			// Go for all sectors
			foreach(Sector s in map.Sectors)
			{
                string ft;
                string ct;

				// Write properties to stream
				writer.Write((Int16)s.FloorHeight);
				writer.Write((Int16)s.CeilHeight);

                flr = 0;
                ceil = 0;

                ft = s.FloorTexture;
                ct = s.CeilTexture;

                if (ft == "-")
                    ft = "?";

                if (ct == "-")
                    ct = "?";

                for (int i = 0; i < General.Map.TextureHashKey.Count; i++)
                {
                    if (ft == General.Map.TextureHashName[i])
                    {
                        flr = (int)General.Map.TextureHashKey[i];
                        break;
                    }
                }

                for (int i = 0; i < General.Map.TextureHashKey.Count; i++)
                {
                    if (ct == General.Map.TextureHashName[i])
                    {
                        ceil = (int)General.Map.TextureHashKey[i];
                        break;
                    }
                }

                /*foreach (TextureIndexInfo tp in General.Map.Config.D64TextureIndex)
                {
                    if (s.FloorTexture == tp.Title)
                        flr = tp.Index;

                    if (s.CeilTexture == tp.Title)
                        ceil = tp.Index;
                }*/

                writer.Write((Int16)flr);
                writer.Write((Int16)ceil);

                writer.Write((Int16)GetLight(s, s.FloorColor));
                writer.Write((Int16)GetLight(s, s.CeilColor));
                writer.Write((Int16)GetLight(s, s.ThingColor));
                writer.Write((Int16)GetLight(s, s.TopColor));
                writer.Write((Int16)GetLight(s, s.LowerColor));

				writer.Write((UInt16)s.Effect);
				writer.Write((UInt16)s.Tag);

                // Convert flags
                flags = 0;
                foreach (KeyValuePair<string, bool> f in s.Flags)
                {
                    int fnum;
                    if (f.Value && int.TryParse(f.Key, out fnum)) flags |= fnum;
                }

                writer.Write((UInt16)flags);
			}

			// Find insert position and remove old lump
			insertpos = MapManager.RemoveSpecificLump(wad, "SECTORS", position, MapManager.TEMP_MAP_HEADER, maplumps);
			if(insertpos == -1) insertpos = position + 1;
			if(insertpos > wad.Lumps.Count) insertpos = wad.Lumps.Count;

			// Create the lump from memory
			lump = wad.Insert("SECTORS", insertpos, (int)mem.Length);
			lump.Stream.Seek(0, SeekOrigin.Begin);
			mem.WriteTo(lump.Stream);
		}

        // This writes the MACROS to WAD file
        private void WriteMacros(MapSet map, int position, IDictionary maplumps)
        {
            MemoryStream mem;
            BinaryWriter writer;
            Lump lump;
            int insertpos;
            int count = 0;
            int specials = 0;
            int i = 0;

            // Create memory to write to
            mem = new MemoryStream();
            writer = new BinaryWriter(mem, WAD.ENCODING);

            // find line with highest action type
            foreach (Linedef l in map.Linedefs)
            {
                if (l.Action >= 256)
                {
                    if(i < l.Action)
                        i = l.Action;
                }
            }

            // set number of macros
            if (i <= 0)
                count = 1;
            else
                count = (i - 256) + 1;

            // count macro actions per batch
            for (i = 0; i < count; i++)
            {
                // empty macro? then allocate a blank one
                if (General.Map.Map.Macros[i] == null)
                    General.Map.Map.Macros[i] = new Macro(1);

                specials += General.Map.Map.Macros[i].Count;
            }

            // write macro count and action count
            writer.Write((UInt16)count);
            writer.Write((UInt16)specials);

            // write actual macro data
            for (i = 0; i < count; i++)
            {
                Macro m = General.Map.Map.Macros[i];

                writer.Write((UInt16)m.Count);

                for (int j = 0; j < m.Count; j++)
                {
                    writer.Write((UInt16)m.MacroData[j].batch);
                    writer.Write((UInt16)m.MacroData[j].tag);
                    writer.Write((UInt16)m.MacroData[j].type);
                }

                // write 'dummy' macro padding at end
                writer.Write((UInt16)0);
                writer.Write((UInt16)0);
                writer.Write((UInt16)0);
            }

            // Find insert position and remove old lump
            insertpos = MapManager.RemoveSpecificLump(wad, "MACROS", position, MapManager.TEMP_MAP_HEADER, maplumps);
            if (insertpos == -1) insertpos = position + 1;
            if (insertpos > wad.Lumps.Count) insertpos = wad.Lumps.Count;

            // Create the lump from memory
            lump = wad.Insert("MACROS", insertpos, (int)mem.Length);
            lump.Stream.Seek(0, SeekOrigin.Begin);
            mem.WriteTo(lump.Stream);
        }
		
		#endregion
	}
}
