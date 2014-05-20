﻿#region ================== Namespaces

using System;
using System.Collections.Generic;
using System.IO;
using CodeImp.DoomBuilder.Config;
using CodeImp.DoomBuilder.Geometry;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Types;

#endregion

namespace CodeImp.DoomBuilder.IO
{
	internal class ClipboardStreamReader
	{

		#region ================== Variables

		private struct SidedefData
		{
			public int OffsetX;
			public int OffsetY;
			public int SectorID;
			public string HighTexture;
			public string MiddleTexture;
			public string LowTexture;
			public Dictionary<string, UniValue> Fields;
			public Dictionary<string, bool> Flags;
		}

		#endregion

		#region ================== Reading

		// This reads from a stream
		public MapSet Read(MapSet map, Stream stream) {
			BinaryReader reader = new BinaryReader(stream);

			// Read the map
			Dictionary<int, Vertex> vertexlink = ReadVertices(map, reader);
			Dictionary<int, Sector> sectorlink = ReadSectors(map, reader);
			Dictionary<int, SidedefData> sidedeflink = ReadSidedefs(reader);
			ReadLinedefs(map, reader, vertexlink, sectorlink, sidedeflink);
			ReadThings(map, reader);

			return map;
		}

		private Dictionary<int, Vertex> ReadVertices(MapSet map, BinaryReader reader) {
			int count = reader.ReadInt32();

			// Create lookup table
			Dictionary<int, Vertex> link = new Dictionary<int, Vertex>(count);

			// Go for all collections
			map.SetCapacity(map.Vertices.Count + count, 0, 0, 0, 0);
			for(int i = 0; i < count; i++) {
				float x = reader.ReadSingle();
				float y = reader.ReadSingle();
				float zc = reader.ReadSingle();
				float zf = reader.ReadSingle();

				// Create new item
				Dictionary<string, UniValue> fields = ReadCustomFields(reader);
				Vertex v = map.CreateVertex(new Vector2D(x, y));
				if(v != null) {
					//zoffsets
					v.ZCeiling = zc;
					v.ZFloor = zf;

					// Add custom fields
					v.Fields.BeforeFieldsChange();
					foreach (KeyValuePair<string, UniValue> group in fields) {
						v.Fields.Add(group.Key, group.Value);
					}

					// Add it to the lookup table
					link.Add(i, v);
				}
			}

			// Return lookup table
			return link;
		}

		private Dictionary<int, Sector> ReadSectors(MapSet map, BinaryReader reader) {
			int count = reader.ReadInt32();

			// Create lookup table
			Dictionary<int, Sector> link = new Dictionary<int, Sector>(count);

			// Go for all collections
			map.SetCapacity(0, 0, 0, map.Sectors.Count + count, 0);

			for (int i = 0; i < count; i++) {
				int tag = reader.ReadInt32();
				int effect = reader.ReadInt32();
				int hfloor = reader.ReadInt32();
				int hceil = reader.ReadInt32();
				int bright = reader.ReadInt32();
				string tfloor = ReadString(reader);
				string tceil = ReadString(reader);

				//flags
				Dictionary<string, bool> stringflags = new Dictionary<string, bool>(StringComparer.Ordinal);
				int numFlags = reader.ReadInt32();
				for(int f = 0; f < numFlags; f++) 
					stringflags.Add(ReadString(reader), true);

				//add missing flags
				foreach (KeyValuePair<string, string> flag in General.Map.Config.SectorFlags) {
					if(stringflags.ContainsKey(flag.Key)) continue;
					stringflags.Add(flag.Key, false);
				}

				// Create new item
				Dictionary<string, UniValue> fields = ReadCustomFields(reader);
				Sector s = map.CreateSector();
				if(s != null) {
					s.Update(hfloor, hceil, tfloor, tceil, effect, stringflags, tag, bright);

					// Add custom fields
					s.Fields.BeforeFieldsChange();
					foreach(KeyValuePair<string, UniValue> group in fields) {
						s.Fields.Add(group.Key, group.Value);
					}

					// Add it to the lookup table
					link.Add(i, s);
				}
			}

			// Return lookup table
			return link;
		}

		// This reads the linedefs and sidedefs
		private void ReadLinedefs(MapSet map, BinaryReader reader, Dictionary<int, Vertex> vertexlink, Dictionary<int, Sector> sectorlink, Dictionary<int, SidedefData> sidedeflink) {
			int count = reader.ReadInt32();

			// Go for all lines
			map.SetCapacity(0, map.Linedefs.Count + count, map.Sidedefs.Count + sidedeflink.Count, 0, 0);
			for(int i = 0; i < count; i++) {
				int[] args = new int[Linedef.NUM_ARGS];
				int tag = reader.ReadInt32();
				int v1 = reader.ReadInt32();
				int v2 = reader.ReadInt32();
				int s1 = reader.ReadInt32();
				int s2 = reader.ReadInt32();
				int special = reader.ReadInt32();
				for(int a = 0; a < Linedef.NUM_ARGS; a++) {
					args[a] = reader.ReadInt32();
				}

				//flags
				Dictionary<string, bool> stringflags = new Dictionary<string, bool>(StringComparer.Ordinal);
				int numFlags = reader.ReadInt32();
				for(int f = 0; f < numFlags; f++)
					stringflags.Add(ReadString(reader), true);

				//add missing flags
				foreach(KeyValuePair<string, string> flag in General.Map.Config.LinedefFlags) {
					if(stringflags.ContainsKey(flag.Key)) continue;
					stringflags.Add(flag.Key, false);
				}

				//add missing activations
				foreach (LinedefActivateInfo activate in General.Map.Config.LinedefActivates) {
					if(stringflags.ContainsKey(activate.Key)) continue;
					stringflags.Add(activate.Key, false);
				}

				// Read custom fields
				Dictionary<string, UniValue> fields = ReadCustomFields(reader);

				// Check if not zero-length
				if (Vector2D.ManhattanDistance(vertexlink[v1].Position, vertexlink[v2].Position) > 0.0001f) {
					// Create new linedef
					Linedef l = map.CreateLinedef(vertexlink[v1], vertexlink[v2]);
					if (l != null) {
						l.Update(stringflags, 0, tag, special, args);
						l.UpdateCache();

						// Add custom fields
						l.Fields.BeforeFieldsChange();
						foreach(KeyValuePair<string, UniValue> group in fields) {
							l.Fields.Add(group.Key, group.Value);
						}

						// Connect sidedefs to the line
						if(s1 > -1) {
							if(s1 < sidedeflink.Count)
								AddSidedef(map, sidedeflink[s1], l, true, sectorlink);
							else
								General.ErrorLogger.Add(ErrorType.Warning, "Linedef " + i + " references invalid front sidedef " + s1 + ". Sidedef has been removed.");
						}

						if(s2 > -1) {
							if(s2 < sidedeflink.Count)
								AddSidedef(map, sidedeflink[s2], l, false, sectorlink);
							else
								General.ErrorLogger.Add(ErrorType.Warning, "Linedef " + i + " references invalid back sidedef " + s1 + ". Sidedef has been removed.");
						}
					}
				} else {
					General.ErrorLogger.Add(ErrorType.Warning, "Linedef " + i + " is zero-length. Linedef has been removed.");
				}
			}
		}

		private static void AddSidedef(MapSet map, SidedefData data, Linedef ld, bool front, Dictionary<int, Sector> sectorlink) {
			// Create sidedef
			if(sectorlink.ContainsKey(data.SectorID)) {
				Sidedef s = map.CreateSidedef(ld, front, sectorlink[data.SectorID]);
				if(s != null) {
					s.Update(data.OffsetX, data.OffsetY, data.HighTexture, data.MiddleTexture, data.LowTexture, data.Flags);

					// Add custom fields
					foreach (KeyValuePair<string, UniValue> group in data.Fields) {
						s.Fields.Add(group.Key, group.Value);
					}
				}
			} else {
				General.ErrorLogger.Add(ErrorType.Warning, "Sidedef references invalid sector " + data.SectorID + ". Sidedef has been removed.");
			}
		}

		private Dictionary<int, SidedefData> ReadSidedefs(BinaryReader reader) {
			Dictionary<int, SidedefData> sidedeflink = new Dictionary<int, SidedefData>();
			int count = reader.ReadInt32();

			for(int i = 0; i < count; i++) {
				SidedefData data = new SidedefData();
				data.OffsetX = reader.ReadInt32();
				data.OffsetY = reader.ReadInt32();
				data.SectorID = reader.ReadInt32();

				data.HighTexture = ReadString(reader);
				data.MiddleTexture = ReadString(reader);
				data.LowTexture = ReadString(reader);

				//flags
				data.Flags = new Dictionary<string, bool>(StringComparer.Ordinal);
				int numFlags = reader.ReadInt32();
				for(int f = 0; f < numFlags; f++)
					data.Flags.Add(ReadString(reader), true);

				//add missing flags
				foreach(KeyValuePair<string, string> flag in General.Map.Config.SidedefFlags) {
					if(data.Flags.ContainsKey(flag.Key)) continue;
					data.Flags.Add(flag.Key, false);
				}

				//custom fields
				data.Fields = ReadCustomFields(reader);

				sidedeflink.Add(i, data);
			}

			return sidedeflink;
		}

		private void ReadThings(MapSet map, BinaryReader reader) {
			int count = reader.ReadInt32();

			// Go for all collections
			map.SetCapacity(0, 0, 0, 0, map.Things.Count + count);
			for(int i = 0; i < count; i++) {
				int[] args = new int[Linedef.NUM_ARGS];
				int tag = reader.ReadInt32();
				float x = reader.ReadSingle();
				float y = reader.ReadSingle();
				float height = reader.ReadSingle();
				int angledeg = reader.ReadInt32();
				int pitch = reader.ReadInt32(); //mxd
				int roll = reader.ReadInt32(); //mxd
				float scaleX = reader.ReadSingle(); //mxd
				float scaleY = reader.ReadSingle(); //mxd
				int type = reader.ReadInt32();
				int special = reader.ReadInt32();
				for(int a = 0; a < Linedef.NUM_ARGS; a++) {
					args[a] = reader.ReadInt32();
				}

				//flags
				Dictionary<string, bool> stringflags = new Dictionary<string, bool>(StringComparer.Ordinal);
				int numFlags = reader.ReadInt32();
				for(int f = 0; f < numFlags; f++)
					stringflags.Add(ReadString(reader), true);

				//add missing flags
				foreach(KeyValuePair<string, string> flag in General.Map.Config.ThingFlags) {
					if(stringflags.ContainsKey(flag.Key)) continue;
					stringflags.Add(flag.Key, false);
				}

				// Create new item
				Dictionary<string, UniValue> fields = ReadCustomFields(reader);
				Thing t = map.CreateThing();
				if(t != null) {
					t.Update(type, x, y, height, angledeg, pitch, roll, scaleX, scaleY, stringflags, tag, special, args);

					// Add custom fields
					t.Fields.BeforeFieldsChange();
					foreach(KeyValuePair<string, UniValue> group in fields) {
						t.Fields.Add(group.Key, group.Value);
					}
				}
			}
		}

		private Dictionary<string, UniValue> ReadCustomFields(BinaryReader reader) {
			Dictionary<string, UniValue> fields = new Dictionary<string, UniValue>(StringComparer.Ordinal);
			int fieldscount = reader.ReadInt32();

			for(int f = 0; f < fieldscount; f++) {
				string name = ReadString(reader);
				UniversalType type = (UniversalType)reader.ReadInt32();
				UniversalType valueType = (UniversalType)reader.ReadInt32();

				switch(valueType) {
					case UniversalType.Float:
						fields.Add(name, new UniValue(type, reader.ReadSingle()));
						break;

					case UniversalType.Boolean:
						fields.Add(name, new UniValue(type, reader.ReadBoolean()));
						break;

					case UniversalType.Integer:
						fields.Add(name, new UniValue(type, reader.ReadInt32()));
						break;

					case UniversalType.String:
						fields.Add(name, new UniValue(type, ReadString(reader)));
						break;

					default: //WOLOLO! ERRORS!
						throw new Exception("Got unknown value type while reading custom fields from clipboard data! Field '" + name + "', type '" + type + "', primitive type '" + valueType + "'");
				}
			}

			return fields;
		}

		private static string ReadString(BinaryReader reader) {
			int len = reader.ReadInt32();
			if (len == 0) return string.Empty;
			char[] chars = new char[len];
			for (int i = 0; i < len; ++i) chars[i] = reader.ReadChar();
			return new string(chars);
		}

		#endregion

	}
}
