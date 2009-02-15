
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
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using CodeImp.DoomBuilder.IO;
using CodeImp.DoomBuilder.ZDoom;
using CodeImp.DoomBuilder.Rendering;

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
		private bool strictpatches;
		
		// Lump ranges
		private List<LumpRange> flatranges;
		private List<LumpRange> patchranges;
		private List<LumpRange> spriteranges;
		private List<LumpRange> textureranges;
		
		#endregion

		#region ================== Properties

		public bool IsIWAD { get { return is_iwad; } }

		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		public WADReader(DataLocation dl) : base(dl)
		{
			General.WriteLogLine("Opening WAD resource '" + location.location + "'");

			// Initialize
			file = new WAD(location.location, true);
			is_iwad = (file.Type == WAD.TYPE_IWAD);
			strictpatches = dl.option1;
			patchranges = new List<LumpRange>();
			spriteranges = new List<LumpRange>();
			flatranges = new List<LumpRange>();
			textureranges = new List<LumpRange>();
			
			// Find ranges
			FindRanges(patchranges, General.Map.Config.PatchRanges, "patches");
			FindRanges(spriteranges, General.Map.Config.SpriteRanges, "sprites");
			FindRanges(flatranges, General.Map.Config.FlatRanges, "flats");
			FindRanges(textureranges, General.Map.Config.TextureRanges, "textures");

			// We have no destructor
			GC.SuppressFinalize(this);
		}

		// Disposer
		public override void Dispose()
		{
			// Not already disposed?
			if(!isdisposed)
			{
				General.WriteLogLine("Closing WAD resource '" + location.location + "'");

				// Clean up
				file.Dispose();
				
				// Done
				base.Dispose();
			}
		}

		#endregion

		#region ================== Management

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
			is_iwad = (file.Type == WAD.TYPE_IWAD);
			base.Resume();
		}

		// This fills a ranges list
		private void FindRanges(List<LumpRange> ranges, IDictionary rangeinfos, string rangename)
		{
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
						LumpRange range = new LumpRange();
						range.start = startindex;
						range.end = file.FindLumpIndex(rangeend, startindex);
						if(range.end > -1)
						{
							ranges.Add(range);
							startindex = file.FindLumpIndex(rangestart, range.end);
						}
						else
						{
							startindex = -1;
						}
					}
				}
			}
		}
		
		#endregion

		#region ================== Palette

		// This loads the PLAYPAL palette
		public override Playpal LoadPalette()
		{
			Lump lump;
			
			// Error when suspended
			if(issuspended) throw new Exception("Data reader is suspended");
			
			// Look for a lump named PLAYPAL
			lump = file.FindLump("PLAYPAL");
			if(lump != null)
			{
				// Read the PLAYPAL from stream
				return new Playpal(lump.Stream);
			}
			else
			{
				// No palette
				return null;
			}
		}

		#endregion

		#region ================== Textures

		// This loads the textures
		public override ICollection<ImageData> LoadTextures(PatchNames pnames)
		{
			List<ImageData> images = new List<ImageData>();
			string rangestart, rangeend;
			Lump lump;

			// Error when suspended
			if(issuspended) throw new Exception("Data reader is suspended");

			// Load two sets of textures, if available
			lump = file.FindLump("TEXTURE1");
			if(lump != null) LoadTextureSet(lump.Stream, ref images, pnames);
			lump = file.FindLump("TEXTURE2");
			if(lump != null) LoadTextureSet(lump.Stream, ref images, pnames);
			
			// Read ranges from configuration
			foreach(LumpRange range in textureranges)
			{
				// Load texture range
				LoadTexturesRange(range.start, range.end, ref images, pnames);
			}

			// Load TEXTURES lump file
			lump = file.FindLump("TEXTURES");
			if(lump != null)
			{
				MemoryStream filedata = new MemoryStream(lump.Stream.ReadAllBytes());
				WADReader.LoadHighresTextures(filedata, "TEXTURES", ref images, null, null);
				filedata.Dispose();
			}

			// Return result
			return images;
		}
		
		// This loads a range of textures
		private void LoadTexturesRange(int startindex, int endindex, ref List<ImageData> images, PatchNames pnames)
		{
			// Determine default scale
			float defaultscale = General.Map.Config.DefaultTextureScale;
			
			// Go for all lumps between start and end exclusive
			for(int i = startindex + 1; i < endindex; i++)
			{
				// Lump not zero length?
				if(file.Lumps[i].Length > 0)
				{
					// Make the image
					SimpleTextureImage image = new SimpleTextureImage(file.Lumps[i].Name, file.Lumps[i].Name, defaultscale, defaultscale);
					
					// Add image to collection
					images.Add(image);
				}
			}
		}

		// This loads the texture definitions from a TEXTURES lump
		public static void LoadHighresTextures(Stream stream, string filename, ref List<ImageData> images, Dictionary<long, ImageData> textures, Dictionary<long, ImageData> flats)
		{
			// Parse the data
			TexturesParser parser = new TexturesParser();
			parser.Parse(stream, filename);

			// Make the textures
			foreach(TextureStructure t in parser.Textures)
			{
				// Add the texture
				ImageData img = t.MakeImage(textures, flats);
				images.Add(img);
			}
		}
		
		// This loads a set of textures
		public static void LoadTextureSet(Stream texturedata, ref List<ImageData> images, PatchNames pnames)
		{
			BinaryReader reader = new BinaryReader(texturedata);
			int flags, width, height, patches, px, py, pi;
			uint numtextures;
			byte scalebytex, scalebytey;
			float scalex, scaley, defaultscale;
			byte[] namebytes;
			TextureImage image;
			bool strifedata;

			// Determine default scale
			defaultscale = General.Map.Config.DefaultTextureScale;
			
			// Get number of textures
			texturedata.Seek(0, SeekOrigin.Begin);
			numtextures = reader.ReadUInt32();
			
			// Skip offset bytes (we will read all textures sequentially)
			texturedata.Seek(4 * numtextures, SeekOrigin.Current);

			// Go for all textures defined in this lump
			for(uint i = 0; i < numtextures; i++)
			{
				// Read texture properties
				namebytes = reader.ReadBytes(8);
				flags = reader.ReadUInt16();
				scalebytex = reader.ReadByte();
				scalebytey = reader.ReadByte();
				width = reader.ReadInt16();
				height = reader.ReadInt16();
				patches = reader.ReadInt16();
				
				// Check for doom or strife data format
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
				if(scalebytex == 0) scalex = defaultscale; else scalex = 1f / ((float)scalebytex / 8f);
				if(scalebytey == 0) scaley = defaultscale; else scaley = 1f / ((float)scalebytey / 8f);
				
				// Validate data
				if((width > 0) && (height > 0) && (patches > 0) &&
				   (scalex != 0) || (scaley != 0))
				{
					// Make the image object
					image = new TextureImage(Lump.MakeNormalName(namebytes, WAD.ENCODING),
											 width, height, scalex, scaley);

					// Go for all patches in texture
					for(int p = 0; p < patches; p++)
					{
						// Read patch properties
						px = reader.ReadInt16();
						py = reader.ReadInt16();
						pi = reader.ReadUInt16();
						if(!strifedata) texturedata.Seek(4, SeekOrigin.Current);
						
						// Validate data
						if((pi >= 0) && (pi < pnames.Length) && (pnames[pi].Length > 0))
						{
							// Create patch on image
							image.AddPatch(new TexturePatch(pnames[pi], px, py));
						}
					}
					
					// Add image to collection
					images.Add(image);
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
			Lump lump;

			// Error when suspended
			if(issuspended) throw new Exception("Data reader is suspended");

			// Look for a lump named PNAMES
			lump = file.FindLump("PNAMES");
			if(lump != null)
			{
				// Read the PNAMES from stream
				return new PatchNames(lump.Stream);
			}
			else
			{
				// No palette
				return null;
			}
		}

		// This finds and returns a patch stream
		public override Stream GetPatchData(string pname)
		{
			Lump lump;

			// Error when suspended
			if(issuspended) throw new Exception("Data reader is suspended");

			// Strictly read patches only between P_START and P_END?
			if(strictpatches)
			{
				// Find the lump in ranges
				foreach(LumpRange range in patchranges)
				{
					lump = file.FindLump(pname, range.start, range.end);
					if(lump != null) return lump.Stream;
				}
			}
			else
			{
				// Find the lump anywhere
				lump = file.FindLump(pname);
				if(lump != null) return lump.Stream;
			}
			
			return null;
		}

		// This finds and returns a texture stream
		public override Stream GetTextureData(string pname)
		{
			Lump lump;

			// Error when suspended
			if(issuspended) throw new Exception("Data reader is suspended");

			// Find the lump in ranges
			foreach(LumpRange range in textureranges)
			{
				lump = file.FindLump(pname, range.start, range.end);
				if(lump != null) return lump.Stream;
			}

			return null;
		}
		
		#endregion
		
		#region ================== Flats

		// This loads the textures
		public override ICollection<ImageData> LoadFlats()
		{
			List<ImageData> images = new List<ImageData>();
			string rangestart, rangeend;

			// Error when suspended
			if(issuspended) throw new Exception("Data reader is suspended");

			// Read ranges from configuration
			foreach(DictionaryEntry r in General.Map.Config.FlatRanges)
			{
				// Read start and end
				rangestart = General.Map.Config.ReadSetting("flats." + r.Key + ".start", "");
				rangeend = General.Map.Config.ReadSetting("flats." + r.Key + ".end", "");
				if((rangestart.Length > 0) && (rangeend.Length > 0))
				{
					// Load texture range
					LoadFlatsRange(rangestart, rangeend, ref images);
				}
			}

			// Return result
			return images;
		}

		// This loads a range of flats
		private void LoadFlatsRange(string startlump, string endlump, ref List<ImageData> images)
		{
			int startindex, endindex;
			float defaultscale;
			FlatImage image;

			// Determine default scale
			defaultscale = General.Map.Config.DefaultTextureScale;

			// Continue until no more start can be found
			startindex = file.FindLumpIndex(startlump);
			while(startindex > -1)
			{
				// Find end index
				endindex = file.FindLumpIndex(endlump, startindex + 1);
				if(endindex > -1)
				{
					// Go for all lumps between start and end exclusive
					for(int i = startindex + 1; i < endindex; i++)
					{
						// Lump not zero-length?
						if(file.Lumps[i].Length > 0)
						{
							// Make the image object
							image = new FlatImage(file.Lumps[i].Name);

							// Add image to collection
							images.Add(image);
						}
					}
				}
				
				// Find the next start
				startindex = file.FindLumpIndex(startlump, startindex + 1);
			}
		}
		
		// This finds and returns a patch stream
		public override Stream GetFlatData(string pname)
		{
			Lump lump;

			// Error when suspended
			if(issuspended) throw new Exception("Data reader is suspended");

			// Find the lump in ranges
			foreach(LumpRange range in flatranges)
			{
				lump = file.FindLump(pname, range.start, range.end);
				if(lump != null) return lump.Stream;
			}
			
			return null;
		}
		
		#endregion

		#region ================== Sprite

		// This finds and returns a sprite stream
		public override Stream GetSpriteData(string pname)
		{
			Lump lump;

			// Error when suspended
			if(issuspended) throw new Exception("Data reader is suspended");

			// Find the lump in ranges
			foreach(LumpRange range in spriteranges)
			{
				lump = file.FindLump(pname, range.start, range.end);
				if(lump != null) return lump.Stream;
			}

			return null;
		}
		
		// This checks if the given sprite exists
		public override bool GetSpriteExists(string pname)
		{
			Lump lump;
			
			// Error when suspended
			if(issuspended) throw new Exception("Data reader is suspended");

			// Find the lump in ranges
			foreach(LumpRange range in spriteranges)
			{
				lump = file.FindLump(pname, range.start, range.end);
				if(lump != null) return true;
			}

			return false;
		}
		
		#endregion

		#region ================== Things

		// This finds and returns a sprite stream
		public override Stream GetDecorateData(string pname)
		{
			Lump lump;

			// Error when suspended
			if(issuspended) throw new Exception("Data reader is suspended");

			// Find the lump
			lump = file.FindLump(pname);
			if(lump != null) return lump.Stream; else return null;
		}

		#endregion
	}
}
