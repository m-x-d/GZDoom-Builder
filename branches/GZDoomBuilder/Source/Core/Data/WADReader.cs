
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
using System.IO;
using CodeImp.DoomBuilder.IO;
using CodeImp.DoomBuilder.ZDoom;
using CodeImp.DoomBuilder.GZBuilder.Data;
using System.Text.RegularExpressions;

#endregion

namespace CodeImp.DoomBuilder.Data
{
	internal sealed class WADReader : DataReader
	{
		#region ================== Constants

		#endregion

		#region ================== Structures

		private struct LumpRange
		{
			public int start;
			public int end;
		}

		#endregion

		#region ================== Variables

		// Source
		private WAD file;
		private bool is_iwad;
		private readonly bool strictpatches;
		
		// Lump ranges
		private readonly List<LumpRange> flatranges;
		private readonly List<LumpRange> invertedflatranges; //mxd
		private readonly List<LumpRange> patchranges;
		private readonly List<LumpRange> spriteranges;
		private readonly List<LumpRange> textureranges;
		private readonly List<LumpRange> hiresranges; //mxd
		private readonly List<LumpRange> colormapranges;
		private readonly List<LumpRange> voxelranges; //mxd
		
		#endregion

		#region ================== Properties

		public bool IsIWAD { get { return is_iwad; } }
		internal WAD WadFile { get { return file; } } //mxd

		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		public WADReader(DataLocation dl) : base(dl)
		{
			General.WriteLogLine("Opening WAD resource \"" + location.location + "\"");

			if(!File.Exists(location.location))
				throw new FileNotFoundException("Could not find the file \"" + location.location + "\"", location.location);
			
			// Initialize
			file = new WAD(location.location, true);
			is_iwad = file.IsIWAD;
			strictpatches = dl.option1;
			patchranges = new List<LumpRange>();
			spriteranges = new List<LumpRange>();
			flatranges = new List<LumpRange>();
			textureranges = new List<LumpRange>();
			hiresranges = new List<LumpRange>(); //mxd
			colormapranges = new List<LumpRange>();
			voxelranges = new List<LumpRange>(); //mxd
			
			// Find ranges
			FindRanges(patchranges, General.Map.Config.PatchRanges, "patches", "Patch");
			FindRanges(spriteranges, General.Map.Config.SpriteRanges, "sprites", "Sprite");
			FindRanges(flatranges, General.Map.Config.FlatRanges, "flats", "Flat");
			FindRanges(textureranges, General.Map.Config.TextureRanges, "textures", "Texture");
			FindRanges(hiresranges, General.Map.Config.HiResRanges, "hires", "HiRes texture");
			FindRanges(colormapranges, General.Map.Config.ColormapRanges, "colormaps", "Colormap");
			FindRanges(voxelranges, General.Map.Config.VoxelRanges, "voxels", "Voxel");

			//mxd
			invertedflatranges = new List<LumpRange>();

			if(flatranges.Count > 0) 
			{
				//add range before the first flatrange
				if(flatranges[0].start > 0) 
				{
					LumpRange range = new LumpRange {start = 0, end = flatranges[0].start - 1};
					invertedflatranges.Add(range);
				}

				//add ranges between flatranges
				for(int i = 1; i < flatranges.Count; i++) 
				{
					LumpRange range = new LumpRange { start = flatranges[i - 1].end + 1, end = flatranges[i].start - 1 };
					invertedflatranges.Add(range);
				}

				//add range after the last flatrange
				if(flatranges[flatranges.Count - 1].end < file.Lumps.Count - 1) 
				{
					LumpRange range = new LumpRange { start = flatranges[flatranges.Count - 1].end + 1, end = file.Lumps.Count - 1 };
					invertedflatranges.Add(range);
				}
			} 
			else // No flat ranges? Make one giant range then... 
			{ 
				LumpRange range = new LumpRange {start = 0, end = file.Lumps.Count - 1};
				invertedflatranges.Add(range);
			}

			// We have no destructor
			GC.SuppressFinalize(this);
		}

		// Disposer
		public override void Dispose()
		{
			// Not already disposed?
			if(!isdisposed)
			{
				General.WriteLogLine("Closing WAD resource \"" + location.location + "\"");

				// Clean up
				file.Dispose();
				
				// Done
				base.Dispose();
			}
		}

		#endregion

		#region ================== Management

		// Return a short name for this data location
		public override string GetTitle()
		{
			return Path.GetFileName(location.location);
		}

		// This suspends use of this resource
		public override void Suspend()
		{
			file.Dispose();
			base.Suspend();
		}

		// This resumes use of this resource
		public override void Resume()
		{
			file = new WAD(location.location, true);
			is_iwad = file.IsIWAD;
			base.Resume();
		}

		// This fills a ranges list
		private void FindRanges(List<LumpRange> ranges, IDictionary rangeinfos, string rangename, string elementname)
		{
			Dictionary<LumpRange, KeyValuePair<string, string>> failedranges = new Dictionary<LumpRange, KeyValuePair<string, string>>(); //mxd
			Dictionary<int, bool> successfulrangestarts = new Dictionary<int, bool>(); //mxd
			Dictionary<int, bool> failedrangestarts = new Dictionary<int, bool>(); //mxd
			
			foreach(DictionaryEntry r in rangeinfos)
			{
				// Read start and end
				string rangestart = General.Map.Config.ReadSetting(rangename + "." + r.Key + ".start", "");
				string rangeend = General.Map.Config.ReadSetting(rangename + "." + r.Key + ".end", "");
				if((rangestart.Length > 0) && (rangeend.Length > 0))
				{
					// Find ranges
					int startindex = file.FindLumpIndex(rangestart);
					while(startindex > -1)
					{
						LumpRange range = new LumpRange { start = startindex, end = file.FindLumpIndex(rangeend, startindex) };
						if(range.end > -1)
						{
							if(!successfulrangestarts.ContainsKey(startindex)) successfulrangestarts.Add(startindex, false); //mxd
							ranges.Add(range);
							startindex = file.FindLumpIndex(rangestart, range.end);
						}
						else
						{
							//mxd
							if(!failedrangestarts.ContainsKey(startindex))
							{
								failedranges.Add(range, new KeyValuePair<string, string>(rangestart, rangeend)); //mxd
								failedrangestarts.Add(startindex, false);
							}
							
							startindex = -1;
						}
					}
				}
			}

			// Don't check official IWADs
			if(!file.IsOfficialIWAD)
			{
				//mxd. Display warnings for unclosed ranges
				foreach(KeyValuePair<LumpRange, KeyValuePair<string, string>> group in failedranges)
				{
					if(successfulrangestarts.ContainsKey(group.Key.start)) continue;
					General.ErrorLogger.Add(ErrorType.Warning, "\"" + group.Value.Key + "\" range at index " + group.Key.start + " is not closed in resource \"" + location.location + "\" (\"" + group.Value.Value + "\" marker is missing).");
				}

				//mxd. Check duplicates
				foreach(LumpRange range in ranges)
				{
					HashSet<string> names = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
					for(int i = range.start + 1; i < range.end; i++)
					{
						if(names.Contains(file.Lumps[i].Name))
							General.ErrorLogger.Add(ErrorType.Warning, elementname + " \"" + file.Lumps[i].Name + "\", index " + i + " is double defined in resource \"" + location.location + "\".");
						else
							names.Add(file.Lumps[i].Name);
					}
				}
			}
		}
		
		#endregion

		#region ================== Palette

		// This loads the PLAYPAL palette
		public override Playpal LoadPalette()
		{
			// Error when suspended
			if(issuspended) throw new Exception("Data reader is suspended");
			
			// Look for a lump named PLAYPAL
			Lump lump = file.FindLump("PLAYPAL");
			if(lump != null) return new Playpal(lump.Stream); // Read the PLAYPAL from stream
			return null; // No palette
		}

		#endregion

		#region ================== Colormaps

		// This loads the textures
		public override ICollection<ImageData> LoadColormaps()
		{
			List<ImageData> images = new List<ImageData>();

			// Error when suspended
			if(issuspended) throw new Exception("Data reader is suspended");

			// Read ranges from configuration
			foreach(DictionaryEntry r in General.Map.Config.ColormapRanges)
			{
				// Read start and end
				string rangestart = General.Map.Config.ReadSetting("colormaps." + r.Key + ".start", "");
				string rangeend = General.Map.Config.ReadSetting("colormaps." + r.Key + ".end", "");
				if((rangestart.Length > 0) && (rangeend.Length > 0))
				{
					// Load texture range
					LoadColormapsRange(rangestart, rangeend, ref images);
				}
			}

			// Add images to the container-specific texture set
			foreach(ImageData img in images) textureset.AddFlat(img);

			// Return result
			return images;
		}

		// This loads a range of colormaps
		private void LoadColormapsRange(string startlump, string endlump, ref List<ImageData> images)
		{
			// Continue until no more start can be found
			int startindex = file.FindLumpIndex(startlump);
			while(startindex > -1)
			{
				// Find end index
				int endindex = file.FindLumpIndex(endlump, startindex + 1);
				if(endindex > -1)
				{
					// Go for all lumps between start and end exclusive
					for(int i = startindex + 1; i < endindex; i++)
					{
						// Lump not zero-length?
						if(file.Lumps[i].Length > 0)
						{
							// Make the image object
							ColormapImage image = new ColormapImage(file.Lumps[i].Name);

							// Add image to collection
							images.Add(image);
						}
					}
				}

				// Find the next start
				startindex = file.FindLumpIndex(startlump, startindex + 1);
			}
		}

		// This finds and returns a colormap stream
		public override Stream GetColormapData(string pname)
		{
			// Error when suspended
			if(issuspended) throw new Exception("Data reader is suspended");

			// Strictly read patches only between C_START and C_END?
			if(strictpatches)
			{
				// Find the lump in ranges
				foreach(LumpRange range in colormapranges)
				{
					Lump lump = file.FindLump(pname, range.start, range.end);
					if(lump != null) return lump.Stream;
				}
			}
			else
			{
				// Find the lump anywhere
				Lump lump = file.FindLump(pname);
				if(lump != null) return lump.Stream;
			}

			return null;
		}

		#endregion

		#region ================== Textures

		// This loads the textures
		public override IEnumerable<ImageData> LoadTextures(PatchNames pnames, Dictionary<string, TexturesParser> cachedparsers)
		{
			// Error when suspended
			if(issuspended) throw new Exception("Data reader is suspended");

			List<ImageData> images = new List<ImageData>();
			float defaultscale = General.Map.Config.DefaultTextureScale; //mxd

			// Load two sets of textures, if available
			Lump lump = file.FindLump("TEXTURE1");
			if(lump != null) 
			{
				LoadTextureSet("TEXTURE1", lump.Stream, ref images, pnames);
				if(images.Count > 0) images.RemoveAt(0); //mxd. The first TEXTURE1 texture cannot be used. Let's remove it here
			}
			lump = file.FindLump("TEXTURE2");
			if(lump != null) LoadTextureSet("TEXTURE2", lump.Stream, ref images, pnames);
			
			// Read ranges from configuration
			foreach(LumpRange range in textureranges)
			{
				// Go for all lumps between start and end exclusive
				for(int i = range.start + 1; i < range.end; i++) 
				{
					// Lump not zero length?
					if(file.Lumps[i].Length > 0) 
					{
						// Make the image
						SimpleTextureImage image = new SimpleTextureImage(file.Lumps[i].Name, file.Lumps[i].Name, defaultscale, defaultscale);

						// Add image to collection
						images.Add(image);
					} 
					else 
					{
						// Can't load image without size
						General.ErrorLogger.Add(ErrorType.Error, "Can't load texture \"" + file.Lumps[i].Name + "\" because it doesn't contain any data.");
					}
				}
			}
			
			// Load TEXTURES lump file
			int lumpindex = file.FindLumpIndex("TEXTURES");
			while(lumpindex > -1)
			{
				//mxd. Added TexturesParser caching
				string fullpath = Path.Combine(this.location.location, "TEXTURES#" + lumpindex);
				if(cachedparsers.ContainsKey(fullpath))
				{
					// Make the textures
					foreach(TextureStructure t in cachedparsers[fullpath].Textures)
						images.Add(t.MakeImage());
				}
				else
				{
					MemoryStream filedata = new MemoryStream(file.Lumps[lumpindex].Stream.ReadAllBytes());
					cachedparsers.Add(fullpath, LoadTEXTURESTextures(new TextResourceData(this, filedata, "TEXTURES", lumpindex, true), ref images)); //mxd
					filedata.Dispose();
				}

				// Find next
				lumpindex = file.FindLumpIndex("TEXTURES", lumpindex + 1);
			}
			
			// Add images to the container-specific texture set
			foreach(ImageData img in images) textureset.AddTexture(img);

			// Return result
			return images;
		}

		//mxd. This loads the HiRes textures
		public override IEnumerable<HiResImage> LoadHiResTextures()
		{
			List<HiResImage> images = new List<HiResImage>();
			
			// Read ranges from configuration
			foreach(LumpRange range in hiresranges)
			{
				// Go for all lumps between start and end exclusive
				for(int i = range.start + 1; i < range.end; i++)
				{
					// Lump not zero length?
					if(file.Lumps[i].Length > 0)
					{
						// Add image to collection
						images.Add(new HiResImage(file.Lumps[i].Name));
					}
					else
					{
						// Can't load image without size
						General.ErrorLogger.Add(ErrorType.Error, "Can't load HiRes texture \"" + file.Lumps[i].Name + "\" because it doesn't contain any data.");
					}
				}
			}

			return images;
		}

		// This loads the texture definitions from a TEXTURES lump
		public static TexturesParser LoadTEXTURESTextures(TextResourceData data, ref List<ImageData> images)
		{
			// Parse the data
			TexturesParser parser = new TexturesParser();
			parser.Parse(data, false);
			if(parser.HasError) parser.LogError(); //mxd

			// Make the textures
			foreach(TextureStructure t in parser.Textures)
			{
				// Add the texture
				images.Add(t.MakeImage());
			}

			//mxd. Add to text resources collection
			if(!General.Map.Data.TextResources.ContainsKey(parser.ScriptType))
				General.Map.Data.TextResources[parser.ScriptType] = new HashSet<TextResource>();

			foreach(KeyValuePair<string, TextResource> group in parser.TextResources)
				General.Map.Data.TextResources[parser.ScriptType].Add(group.Value);

			return parser; //mxd
		}
		
		// This loads a set of textures
		public static void LoadTextureSet(string sourcename, Stream texturedata, ref List<ImageData> images, PatchNames pnames)
		{
			if(texturedata.Length == 0) return;
			BinaryReader reader = new BinaryReader(texturedata);

			// Determine default scale
			float defaultscale = General.Map.Config.DefaultTextureScale;
			
			// Get number of textures
			texturedata.Seek(0, SeekOrigin.Begin);
			uint numtextures = reader.ReadUInt32();
			
			// Skip offset bytes (we will read all textures sequentially)
			texturedata.Seek(4 * numtextures, SeekOrigin.Current);

			// Go for all textures defined in this lump
			for(uint i = 0; i < numtextures; i++)
			{
				// Read texture properties
				byte[] namebytes = reader.ReadBytes(8);
				int flags = reader.ReadUInt16();
				byte scalebytex = reader.ReadByte();
				byte scalebytey = reader.ReadByte();
				int width = reader.ReadInt16();
				int height = reader.ReadInt16();
				int patches = reader.ReadInt16();
				
				// Check for doom or strife data format
				bool strifedata;
				if(patches == 0)
				{
					// Ignore 2 bytes and then read number of patches
					texturedata.Seek(2, SeekOrigin.Current);
					patches = reader.ReadInt16();
					strifedata = false;
				}
				else
				{
					// Texture data is in strife format
					strifedata = true;
				}

				// Determine actual scales
				float scalex = (scalebytex == 0 ? defaultscale : 1f / (scalebytex / 8f));
				float scaley = (scalebytey == 0 ? defaultscale : 1f / (scalebytey / 8f));
				
				// Validate data
				if((width > 0) && (height > 0) && (patches > 0) && (scalex != 0) || (scaley != 0))
				{
					string texname = Lump.MakeNormalName(namebytes, WAD.ENCODING);
					TextureImage image = null;
					if(texname.Length > 0)
					{
						// Make the image object
						image = new TextureImage(sourcename, Lump.MakeNormalName(namebytes, WAD.ENCODING),
												 width, height, scalex, scaley);
					}
					else
					{
						// Can't load image without name
						General.ErrorLogger.Add(ErrorType.Error, "Can't load an unnamed texture from \"" + sourcename + "\". Please consider giving names to your resources.");
					}
					
					// Go for all patches in texture
					for(int p = 0; p < patches; p++)
					{
						// Read patch properties
						int px = reader.ReadInt16();
						int py = reader.ReadInt16();
						int pi = reader.ReadUInt16();
						if(!strifedata) texturedata.Seek(4, SeekOrigin.Current);
						
						// Validate data
						if((pi >= 0) && (pi < pnames.Length))
						{
							if(pnames[pi].Length > 0)
							{
								// Create patch on image
								if(image != null) image.AddPatch(new TexturePatch(pnames[pi], px, py));
							}
							else
							{
								// Can't load image without name
								General.ErrorLogger.Add(ErrorType.Error, "Can't use an unnamed patch referenced in \"" + sourcename + "\". Please consider giving names to your resources.");
							}
						}
					}
					
					// Add image to collection
					if(image != null) images.Add(image);
				}
				else
				{
					// Skip patches data
					texturedata.Seek(6 * patches, SeekOrigin.Current);
					if(!strifedata) texturedata.Seek(4 * patches, SeekOrigin.Current);
				}
			}
		}

		// This returns the patch names from the PNAMES lump
		public override PatchNames LoadPatchNames()
		{
			// Error when suspended
			if(issuspended) throw new Exception("Data reader is suspended");

			// Look for a lump named PNAMES
			Lump lump = file.FindLump("PNAMES");
			if(lump != null) return new PatchNames(lump.Stream); // Read the PNAMES from stream
			return null; // No patch names found
		}

		// This finds and returns a patch stream
		public override Stream GetPatchData(string pname, bool longname)
		{
			// Error when suspended
			if(issuspended) throw new Exception("Data reader is suspended");
			if(longname) return null; //mxd
			Lump lump;

			// mxd. First strictly read patches between P_START and P_END
			foreach(LumpRange range in patchranges) 
			{
				lump = file.FindLump(pname, range.start, range.end);
				if(lump != null) return lump.Stream;
			}
			
			if(!strictpatches) 
			{
				//mxd. Find the lump anywhere EXCEPT flat ranges (the way it's done in ZDoom)
				foreach(LumpRange range in invertedflatranges) 
				{
					lump = file.FindLump(pname, range.start, range.end);
					if(lump != null) return lump.Stream;
				}

				// Find the lump anywhere IN flat ranges
				foreach(LumpRange range in flatranges) 
				{
					lump = file.FindLump(pname, range.start, range.end);
					if(lump != null) return lump.Stream;
				}
			}
			
			return null;
		}

		// This finds and returns a texture stream
		public override Stream GetTextureData(string pname, bool longname)
		{
			// Error when suspended
			if(issuspended) throw new Exception("Data reader is suspended");
			if(longname) return null; //mxd

			// Find the lump in ranges
			foreach(LumpRange range in textureranges)
			{
				Lump lump = file.FindLump(pname, range.start, range.end);
				if(lump != null) return lump.Stream;
			}

			return null;
		}

		//mxd. This finds and returns a HiRes texture stream
		public override Stream GetHiResTextureData(string name)
		{
			// Error when suspended
			if(issuspended) throw new Exception("Data reader is suspended");

			// Find the lump in ranges
			foreach(LumpRange range in hiresranges)
			{
				Lump lump = file.FindLump(name, range.start, range.end);
				if(lump != null) return lump.Stream;
			}

			return null;
		}

		#endregion
		
		#region ================== Flats

		//mxd. This loads the flats
		public override IEnumerable<ImageData> LoadFlats(Dictionary<string, TexturesParser> cachedparsers) 
		{
			// Error when suspended
			if(issuspended) throw new Exception("Data reader is suspended");

			List<ImageData> images = new List<ImageData>();
			foreach(LumpRange range in flatranges)
			{
				if(range.end < range.start + 2) continue;

				for(int i = range.start + 1; i < range.end; i++) 
				{
					// Lump not zero-length?
					if(file.Lumps[i].Length > 0)
					{
						// Make the image object
						FlatImage image = new FlatImage(file.Lumps[i].Name);

						// Add image to collection
						images.Add(image);
					}
				}
			}

			// Load TEXTURES lump file
			int lumpindex = file.FindLumpIndex("TEXTURES");
			while(lumpindex > -1) 
			{
				//mxd. Added TexturesParser caching
				string fullpath = Path.Combine(this.location.location, "TEXTURES#" + lumpindex);
				if(cachedparsers.ContainsKey(fullpath))
				{
					// Make the textures
					foreach(TextureStructure t in cachedparsers[fullpath].Flats)
						images.Add(t.MakeImage());
				}
				else
				{
					MemoryStream filedata = new MemoryStream(file.Lumps[lumpindex].Stream.ReadAllBytes());
					cachedparsers.Add(fullpath, LoadTEXTURESFlats(new TextResourceData(this, filedata, "TEXTURES", lumpindex, true), ref images)); //mxd
					filedata.Dispose();
				}

				// Find next
				lumpindex = file.FindLumpIndex("TEXTURES", lumpindex + 1);
			}

			// Add images to the container-specific texture set
			foreach(ImageData img in images) textureset.AddFlat(img);

			// Return result
			return images;
		}

		// This loads the flat definitions from a TEXTURES lump
		public static TexturesParser LoadTEXTURESFlats(TextResourceData data, ref List<ImageData> images)
		{
			// Parse the data
			TexturesParser parser = new TexturesParser();
			parser.Parse(data, false);
			if(parser.HasError) parser.LogError(); //mxd

			// Make the flat
			foreach(TextureStructure t in parser.Flats)
			{
				// Add the flat
				images.Add(t.MakeImage());
			}

			//mxd. Add to text resources collection
			if(!General.Map.Data.TextResources.ContainsKey(parser.ScriptType))
				General.Map.Data.TextResources[parser.ScriptType] = new HashSet<TextResource>();

			foreach(KeyValuePair<string, TextResource> group in parser.TextResources)
				General.Map.Data.TextResources[parser.ScriptType].Add(group.Value);

			return parser; //mxd
		}
		
		// This finds and returns a patch stream
		public override Stream GetFlatData(string pname, bool longname)
		{
			// Error when suspended
			if(issuspended) throw new Exception("Data reader is suspended");
			if(longname) return null; //mxd

			// Find the lump in ranges
			foreach(LumpRange range in flatranges)
			{
				Lump lump = file.FindLump(pname, range.start, range.end);
				if(lump != null) return lump.Stream;
			}
			
			return null;
		}
		
		#endregion

		#region ================== Sprite

		// This loads the textures
		public override IEnumerable<ImageData> LoadSprites(Dictionary<string, TexturesParser> cachedparsers)
		{
			// Error when suspended
			if(issuspended) throw new Exception("Data reader is suspended");

			List<ImageData> images = new List<ImageData>();
			
			// Load TEXTURES lump file
			int lumpindex = file.FindLumpIndex("TEXTURES");
			while(lumpindex > -1)
			{
				//mxd. Added TexturesParser caching
				string fullpath = Path.Combine(this.location.location, "TEXTURES#" + lumpindex);
				if(cachedparsers.ContainsKey(fullpath))
				{
					// Make the textures
					foreach(TextureStructure t in cachedparsers[fullpath].Sprites)
						images.Add(t.MakeImage());
				}
				else
				{
					MemoryStream filedata = new MemoryStream(file.Lumps[lumpindex].Stream.ReadAllBytes());
					cachedparsers.Add(fullpath, LoadTEXTURESSprites(new TextResourceData(this, filedata, "TEXTURES", lumpindex, true), ref images)); //mxd
					filedata.Dispose();
				}

				// Find next
				lumpindex = file.FindLumpIndex("TEXTURES", lumpindex + 1);
			}
			
			// Return result
			return images;
		}

		// This loads the sprites definitions from a TEXTURES lump
		public static TexturesParser LoadTEXTURESSprites(TextResourceData data, ref List<ImageData> images)
		{
			// Parse the data
			TexturesParser parser = new TexturesParser();
			parser.Parse(data, false);
			if(parser.HasError) parser.LogError(); //mxd
			
			// Make the sprites
			foreach(TextureStructure t in parser.Sprites)
			{
				// Add the sprite
				images.Add(t.MakeImage());
			}

			//mxd. Add to text resources collection
			if(!General.Map.Data.TextResources.ContainsKey(parser.ScriptType))
				General.Map.Data.TextResources[parser.ScriptType] = new HashSet<TextResource>();

			foreach(KeyValuePair<string, TextResource> group in parser.TextResources)
				General.Map.Data.TextResources[parser.ScriptType].Add(group.Value);

			return parser; //mxd
		}
		
		// This finds and returns a sprite stream
		public override Stream GetSpriteData(string pname)
		{
			// Error when suspended
			if(issuspended) throw new Exception("Data reader is suspended");

			// Find the lump in ranges
			foreach(LumpRange range in spriteranges)
			{
				Lump lump = file.FindLump(pname, range.start, range.end);
				if(lump != null) return lump.Stream;
			}

			return null;
		}
		
		// This checks if the given sprite exists
		public override bool GetSpriteExists(string pname)
		{
			// Error when suspended
			if(issuspended) throw new Exception("Data reader is suspended");

			// Find the lump in ranges
			foreach(LumpRange range in spriteranges)
			{
				Lump lump = file.FindLump(pname, range.start, range.end);
				if(lump != null) return true;
			}

			return false;
		}
		
		#endregion

		#region ================== Voxels (mxd)

		//mxd. This returns the list of voxels, which can be used without VOXELDEF definition
		public override IEnumerable<string> GetVoxelNames() 
		{
			// Error when suspended
			if(issuspended) throw new Exception("Data reader is suspended");

			List<string> voxels = new List<string>();
			Regex spriteName = new Regex(SPRITE_NAME_PATTERN);

			foreach(LumpRange range in voxelranges) 
			{
				if(range.start == range.end) continue;

				for(int i = range.start + 1; i < range.end; i++) 
				{
					if(spriteName.IsMatch(file.Lumps[i].Name)) voxels.Add(file.Lumps[i].Name);
				}
			}

			return voxels.ToArray();
		}

		//mxd
		public override IEnumerable<TextResourceData> GetVoxeldefData() 
		{
			if(issuspended) throw new Exception("Data reader is suspended");
			return GetFirstLump("VOXELDEF");
		}

		//mxd. This finds and returns a voxel stream or null if no voxel was found
		public override Stream GetVoxelData(string name) 
		{
			// Error when suspended
			if(issuspended) throw new Exception("Data reader is suspended");

			foreach(LumpRange range in voxelranges) 
			{
				if(range.start == range.end) continue;
				Lump lump = file.FindLump(name, range.start, range.end);
				if(lump != null) return lump.Stream;
			}

			return null;
		}

		#endregion

		#region ================== Decorate, Gldefs, Mapinfo, etc...

		// This finds and returns DECORATE streams
		public override IEnumerable<TextResourceData> GetDecorateData(string pname)
		{
			if(issuspended) throw new Exception("Data reader is suspended");
			List<TextResourceData> result = GetAllLumps(pname); //mxd

			//mxd. Return ALL DECORATE lumps
			if(result.Count == 0 || string.Compare(pname, "DECORATE", StringComparison.OrdinalIgnoreCase) == 0)
				return result;

			//mxd. Return THE LAST include lump, because that's the way ZDoom seems to operate
			return new List<TextResourceData> {result[result.Count - 1]};
		}

		//mxd. Should be only one entry per wad
		public override IEnumerable<TextResourceData> GetMapinfoData() 
		{
			if(issuspended) throw new Exception("Data reader is suspended");
			
			// First look for ZMAPINFO
			List<TextResourceData> result = new List<TextResourceData>();
			result.AddRange(GetFirstLump("ZMAPINFO"));

			// Then for MAPINFO
			if(result.Count == 0) result.AddRange(GetFirstLump("MAPINFO"));
			return result;
		}

		//mxd
		public override IEnumerable<TextResourceData> GetGldefsData(GameType gametype) 
		{
			if(issuspended) throw new Exception("Data reader is suspended");

			List<TextResourceData> result = new List<TextResourceData>();

			// Try to load game specific GLDEFS first
			if(gametype != GameType.UNKNOWN)
			{
				string lumpname = Gldefs.GLDEFS_LUMPS_PER_GAME[(int)gametype];
				result.AddRange(GetFirstLump(lumpname));
			}

			// Should be only one entry per wad
			result.AddRange(GetFirstLump("GLDEFS"));
			return result;
		}

		//mxd
		public override IEnumerable<TextResourceData> GetModeldefData() 
		{
			if(issuspended) throw new Exception("Data reader is suspended");
			return GetFirstLump("MODELDEF");
		}

		//mxd
		public override IEnumerable<TextResourceData> GetReverbsData() 
		{
			if(issuspended) throw new Exception("Data reader is suspended");
			return GetFirstLump("REVERBS");
		}

		//mxd
		public override IEnumerable<TextResourceData> GetSndSeqData() 
		{
			if(issuspended) throw new Exception("Data reader is suspended");
			return GetFirstLump("SNDSEQ");
		}

		//mxd
		public override IEnumerable<TextResourceData> GetAnimdefsData()
		{
			if(issuspended) throw new Exception("Data reader is suspended");
			return GetAllLumps("ANIMDEFS");
		}

		//mxd
		public override IEnumerable<TextResourceData> GetTerrainData()
		{
			if(issuspended) throw new Exception("Data reader is suspended");
			return GetAllLumps("TERRAIN");
		}

		//mxd
		private IEnumerable<TextResourceData> GetFirstLump(string name)
		{
			List<TextResourceData> result = new List<TextResourceData>();
			int lumpindex = file.FindLumpIndex(name);
			if(lumpindex != -1)
				result.Add(new TextResourceData(this, file.Lumps[lumpindex].Stream, name, lumpindex, true));

			return result;
		}

		//mxd
		private List<TextResourceData> GetAllLumps(string name)
		{
			List<TextResourceData> result = new List<TextResourceData>();

			// Find all lumps with given name
			int lumpindex = file.FindLumpIndex(name);
			while(lumpindex > -1)
			{
				// Add to collection
				result.Add(new TextResourceData(this, file.Lumps[lumpindex].Stream, name, lumpindex, true));

				// Find next entry
				lumpindex = file.FindLumpIndex(name, lumpindex + 1);
			}

			return result;
		}

		//mxd
		internal override MemoryStream LoadFile(string name) 
		{
			Lump l = file.FindLump(name);

			if(l != null) 
			{
				l.Stream.Seek(0, SeekOrigin.Begin);
				return new MemoryStream(l.Stream.ReadAllBytes());
			}

			return null;
		}

		//mxd
		internal override bool FileExists(string name) 
		{
			return file.FindLumpIndex(name) != -1;
		}

		#endregion
	}
}
