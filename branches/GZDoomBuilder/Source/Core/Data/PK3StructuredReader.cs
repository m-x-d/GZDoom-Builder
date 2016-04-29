
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
using System.IO;
using System.Text.RegularExpressions;
using CodeImp.DoomBuilder.GZBuilder.Data;
using CodeImp.DoomBuilder.ZDoom;

#endregion

namespace CodeImp.DoomBuilder.Data
{
	internal abstract class PK3StructuredReader : DataReader
	{
		#region ================== Constants

		protected const string PATCHES_DIR = "patches";
		protected const string TEXTURES_DIR = "textures";
		protected const string FLATS_DIR = "flats";
		protected const string HIRES_DIR = "hires";
		protected const string SPRITES_DIR = "sprites";
		protected const string COLORMAPS_DIR = "colormaps";
		protected const string GRAPHICS_DIR = "graphics"; //mxd
		protected const string VOXELS_DIR = "voxels"; //mxd
		
		#endregion

		#region ================== Variables
		
		// Source
		protected readonly bool roottextures;
		protected readonly bool rootflats;
		
		// WAD files that must be loaded as well
		protected List<WADReader> wads;
		
		#endregion

		#region ================== Properties

		protected readonly string[] PatchLocations = { PATCHES_DIR, TEXTURES_DIR, FLATS_DIR, SPRITES_DIR, GRAPHICS_DIR }; //mxd. Because ZDoom looks for patches and sprites in this order

		#endregion

		#region ================== Constructor / Disposer
		
		// Constructor
		protected PK3StructuredReader(DataLocation dl, bool asreadonly) : base(dl, asreadonly)
		{
			// Initialize
			this.roottextures = dl.option1;
			this.rootflats = dl.option2;
		}
		
		// Call this to initialize this class
		protected virtual void Initialize()
		{
			// Load all WAD files in the root as WAD resources
			string[] wadfiles = GetWadFiles();
			wads = new List<WADReader>(wadfiles.Length);
			foreach(string w in wadfiles)
			{
				string tempfile = CreateTempFile(w);
				DataLocation wdl = new DataLocation(DataLocation.RESOURCE_WAD, tempfile, Path.Combine(location.GetDisplayName(), Path.GetFileName(w)), false, false, true);
				wads.Add(new WADReader(wdl, location.type != DataLocation.RESOURCE_DIRECTORY));
			}
		}
		
		// Disposer
		public override void Dispose()
		{
			// Not already disposed?
			if(!isdisposed)
			{
				// Clean up
				foreach(WADReader wr in wads) wr.Dispose();
				
				// Done
				base.Dispose();
			}
		}
		
		#endregion
		
		#region ================== Management
		
		// This suspends use of this resource
		public override void Suspend()
		{
			foreach(WADReader wr in wads) wr.Suspend();
			base.Suspend();
		}
		
		// This resumes use of this resource
		public override void Resume()
		{
			foreach(WADReader wr in wads) wr.Resume();
			base.Resume();
		}
		
		#endregion
		
		#region ================== Palette

		// This loads the PLAYPAL palette
		public override Playpal LoadPalette()
		{
			// Error when suspended
			if(issuspended) throw new Exception("Data reader is suspended");
			
			// Palette from wad(s)
			Playpal palette = null;
			foreach(WADReader wr in wads)
			{
				Playpal wadpalette = wr.LoadPalette();
				if(wadpalette != null) return wadpalette;
			}
			
			// Find in root directory
			string foundfile = FindFirstFile("PLAYPAL", false);
			if((foundfile != null) && FileExists(foundfile))
			{
				MemoryStream stream = LoadFile(foundfile);

				if(stream.Length > 767) //mxd
					palette = new Playpal(stream);
				else
					General.ErrorLogger.Add(ErrorType.Warning, "Warning: invalid palette \"" + foundfile + "\"");
				stream.Dispose();
			}
			
			// Done
			return palette;
		}

		#endregion
		
		#region ================== Textures

		// This loads the textures
		public override IEnumerable<ImageData> LoadTextures(PatchNames pnames, Dictionary<string, TexturesParser> cachedparsers)
		{
			// Error when suspended
			if(issuspended) throw new Exception("Data reader is suspended");

			Dictionary<long, ImageData> images = new Dictionary<long, ImageData>();
			IEnumerable<ImageData> collection;
			
			// Load from wad files (NOTE: backward order, because the last wad's images have priority)
			for(int i = wads.Count - 1; i >= 0; i--)
			{
				PatchNames wadpnames = wads[i].LoadPatchNames(); //mxd
				collection = wads[i].LoadTextures((wadpnames != null && wadpnames.Length > 0) ? wadpnames : pnames, cachedparsers); //mxd
				AddImagesToList(images, collection);
			}
			
			// Should we load the images in this directory as textures?
			if(roottextures)
			{
				collection = LoadDirectoryImages("", ImageDataFormat.DOOMPICTURE, false);
				AddImagesToList(images, collection);
			}
			
			// Load TEXTURE1 lump file
			List<ImageData> imgset = new List<ImageData>();
			string texture1file = FindFirstFile("TEXTURE1", false);
			if((texture1file != null) && FileExists(texture1file))
			{
				MemoryStream filedata = LoadFile(texture1file);
				WADReader.LoadTextureSet("TEXTURE1", filedata, ref imgset, pnames);
				filedata.Dispose();
			}

			// Load TEXTURE2 lump file
			string texture2file = FindFirstFile("TEXTURE2", false);
			if((texture2file != null) && FileExists(texture2file))
			{
				MemoryStream filedata = LoadFile(texture2file);
				WADReader.LoadTextureSet("TEXTURE2", filedata, ref imgset, pnames);
				filedata.Dispose();
			}
			
			// Add images from TEXTURE1 and TEXTURE2 lump files
			AddImagesToList(images, imgset);
			
			// Load TEXTURES lump files
			imgset.Clear();
			string[] alltexturefiles = GetAllFilesWhichTitleStartsWith("", "TEXTURES", false); //mxd
			foreach(string texturesfile in alltexturefiles)
			{
				//mxd. Added TexturesParser caching
				string fullpath = Path.Combine(this.location.location, texturesfile);
				if(cachedparsers.ContainsKey(fullpath))
				{
					// Make the textures
					foreach(TextureStructure t in cachedparsers[fullpath].Textures)
						imgset.Add(t.MakeImage());
				}
				else
				{
					MemoryStream filedata = LoadFile(texturesfile);
					TextResourceData data = new TextResourceData(this, filedata, texturesfile, true); //mxd
					cachedparsers.Add(fullpath, WADReader.LoadTEXTURESTextures(data, ref imgset)); //mxd
					filedata.Dispose();
				}
			}
			
			// Add images from TEXTURES lump file
			AddImagesToList(images, imgset);

			//mxd. Add images from texture directory. Textures defined in TEXTURES override ones in "textures" folder
			collection = LoadDirectoryImages(TEXTURES_DIR, ImageDataFormat.DOOMPICTURE, true);
			AddImagesToList(images, collection);
			
			// Add images to the container-specific texture set
			foreach(ImageData img in images.Values) textureset.AddTexture(img);
			
			return new List<ImageData>(images.Values);
		}

		//mxd
		public override IEnumerable<HiResImage> LoadHiResTextures()
		{
			// Go for all files
			string[] files = GetAllFiles(HIRES_DIR, true);
			List<HiResImage> result = new List<HiResImage>(files.Length);
			foreach(string f in files)
			{
				if(string.IsNullOrEmpty(Path.GetFileNameWithoutExtension(f)))
				{
					// Can't load image without name
					General.ErrorLogger.Add(ErrorType.Error, "Can't load an unnamed HiRes texture from \"" + Path.Combine(this.location.GetDisplayName(), HIRES_DIR) + "\". Please consider giving names to your resources.");
				}
				else
				{
					// Add image to list
					result.Add(new HiResImage(f));
				}
			}

			return result;
		}

		// This returns the patch names from the PNAMES lump
		// A directory resource does not support this lump, but the wads in the directory may contain this lump
		public override PatchNames LoadPatchNames()
		{
			PatchNames pnames;
			
			// Error when suspended
			if(issuspended) throw new Exception("Data reader is suspended");
			
			// Load from wad files
			// Note the backward order, because the last wad's images have priority
			for(int i = wads.Count - 1; i >= 0; i--)
			{
				pnames = wads[i].LoadPatchNames();
				if(pnames != null) return pnames;
			}
			
			// If none of the wads provides patch names, let's see if we can
			string pnamesfile = FindFirstFile("PNAMES", false);
			if((pnamesfile != null) && FileExists(pnamesfile))
			{
				MemoryStream pnamesdata = LoadFile(pnamesfile);
				pnames = new PatchNames(pnamesdata);
				pnamesdata.Dispose();
				return pnames;
			}
			
			return null;
		}
		
		#endregion

		#region ================== Flats
		
		// This loads the textures
		public override IEnumerable<ImageData> LoadFlats(Dictionary<string, TexturesParser> cachedparsers)
		{
			Dictionary<long, ImageData> images = new Dictionary<long, ImageData>();
			IEnumerable<ImageData> collection;
			List<ImageData> imgset = new List<ImageData>();
			
			// Error when suspended
			if(issuspended) throw new Exception("Data reader is suspended");
			
			// Load from wad files
			// Note the backward order, because the last wad's images have priority
			for(int i = wads.Count - 1; i >= 0; i--)
			{
				collection = wads[i].LoadFlats(cachedparsers);
				AddImagesToList(images, collection);
			}
			
			// Should we load the images in this directory as flats?
			if(rootflats)
			{
				collection = LoadDirectoryImages("", ImageDataFormat.DOOMFLAT, false);
				AddImagesToList(images, collection);
			}

			// Add images from flats directory
			collection = LoadDirectoryImages(FLATS_DIR, ImageDataFormat.DOOMFLAT, true);
			AddImagesToList(images, collection);

			// Load TEXTURES lump file
			string[] alltexturefiles = GetAllFilesWhichTitleStartsWith("", "TEXTURES", false); //mxd
			foreach(string texturesfile in alltexturefiles)
			{
				//mxd. Added TexturesParser caching
				string fullpath = Path.Combine(this.location.location, texturesfile);
				if(cachedparsers.ContainsKey(fullpath))
				{
					// Make the textures
					foreach(TextureStructure t in cachedparsers[fullpath].Flats)
						imgset.Add(t.MakeImage());
				}
				else
				{
					MemoryStream filedata = LoadFile(texturesfile);
					TextResourceData data = new TextResourceData(this, filedata, texturesfile, true); //mxd
					cachedparsers.Add(fullpath, WADReader.LoadTEXTURESFlats(data, ref imgset)); //mxd
					filedata.Dispose();
				}
			}

			// Add images from TEXTURES lump file
			AddImagesToList(images, imgset);

			// Add images to the container-specific texture set
			foreach(ImageData img in images.Values) 
				textureset.AddFlat(img);
			
			return new List<ImageData>(images.Values);
		}

		//mxd.
		public override Stream GetFlatData(string pname, bool longname, ref string flatlocation) 
		{
			// Error when suspended
			if(issuspended) throw new Exception("Data reader is suspended");

			// Find in any of the wad files
			// Note the backward order, because the last wad's images have priority
			if(!longname) //mxd. Flats with long names can't be in wads
			{
				for(int i = wads.Count - 1; i > -1; i--)
				{
					Stream data = wads[i].GetFlatData(pname, false, ref flatlocation);
					if(data != null) return data;
				}
			}

			// Nothing found
			return null;
		}
		
		#endregion

		#region ================== Sprites

		// This loads the sprites
		public override IEnumerable<ImageData> LoadSprites(Dictionary<string, TexturesParser> cachedparsers)
		{
			// Error when suspended
			if(issuspended) throw new Exception("Data reader is suspended");

			Dictionary<long, ImageData> images = new Dictionary<long, ImageData>();
			List<ImageData> imgset = new List<ImageData>();
			
			// Load from wad files
			// Note the backward order, because the last wad's images have priority
			for(int i = wads.Count - 1; i >= 0; i--)
			{
				IEnumerable<ImageData> collection = wads[i].LoadSprites(cachedparsers);
				AddImagesToList(images, collection);
			}
			
			// Load TEXTURES lump file
			imgset.Clear();
			string[] alltexturefiles = GetAllFilesWhichTitleStartsWith("", "TEXTURES", false); //mxd
			foreach(string texturesfile in alltexturefiles)
			{
				//mxd. Added TexturesParser caching
				string fullpath = Path.Combine(this.location.location, texturesfile);
				if(cachedparsers.ContainsKey(fullpath))
				{
					// Make the textures
					foreach(TextureStructure t in cachedparsers[fullpath].Sprites)
						imgset.Add(t.MakeImage());
				}
				else
				{
					MemoryStream filedata = LoadFile(texturesfile);
					TextResourceData data = new TextResourceData(this, filedata, texturesfile, true); //mxd
					cachedparsers.Add(fullpath, WADReader.LoadTEXTURESSprites(data, ref imgset)); //mxd
					filedata.Dispose();
				}
			}
			
			// Add images from TEXTURES lump file
			AddImagesToList(images, imgset);
			
			return new List<ImageData>(images.Values);
		}

		//mxd. Returns all sprites, which name starts with given string
		public override HashSet<string> GetSpriteNames(string startswith)
		{
			// Error when suspended
			if(issuspended) throw new Exception("Data reader is suspended");

			HashSet<string> result = new HashSet<string>();

			// Load from wad files
			// Note the backward order, because the last wad's images have priority
			for(int i = wads.Count - 1; i >= 0; i--)
			{
				result.UnionWith(wads[i].GetSpriteNames(startswith));
			}

			// Load from out own files
			string[] files = GetAllFilesWhichTitleStartsWith(SPRITES_DIR, startswith, true);
			foreach(string file in files)
			{
				result.Add(Path.GetFileNameWithoutExtension(file).ToUpperInvariant());
			}

			return result;
		}

		#endregion

		#region ================== Colormaps

		// This loads the textures
		public override ICollection<ImageData> LoadColormaps()
		{
			// Error when suspended
			if(issuspended) throw new Exception("Data reader is suspended");

			Dictionary<long, ImageData> images = new Dictionary<long, ImageData>();
			ICollection<ImageData> collection;

			// Load from wad files
			// Note the backward order, because the last wad's images have priority
			for(int i = wads.Count - 1; i >= 0; i--)
			{
				collection = wads[i].LoadColormaps();
				AddImagesToList(images, collection);
			}

			// Add images from flats directory
			collection = LoadDirectoryImages(COLORMAPS_DIR, ImageDataFormat.DOOMCOLORMAP, true);
			AddImagesToList(images, collection);

			// Add images to the container-specific texture set
			foreach(ImageData img in images.Values) textureset.AddFlat(img);

			return new List<ImageData>(images.Values);
		}

		#endregion

		#region ================== DECORATE

		// This finds and returns DECORATE streams
		public override IEnumerable<TextResourceData> GetDecorateData(string pname)
		{
			// Error when suspended
			if(issuspended) throw new Exception("Data reader is suspended");

			List<TextResourceData> result = new List<TextResourceData>();
			string[] allfilenames;
			
			// Find in root directory
			string filename = Path.GetFileName(pname);
			string pathname = Path.GetDirectoryName(pname);
			
			if(filename.IndexOf('.') > -1)
			{
				string fullname = Path.Combine(pathname, filename);
				if(FileExists(fullname)) 
				{
					allfilenames = new string[1];
					allfilenames[0] = Path.Combine(pathname, filename);
				} 
				else 
				{
					allfilenames = new string[0];
					General.ErrorLogger.Add(ErrorType.Warning, "Unable to load DECORATE file \"" + fullname + "\"");
				}
			}
			else
				allfilenames = GetAllFilesWithTitle(pathname, filename, false);

			foreach(string foundfile in allfilenames)
				result.Add(new TextResourceData(this, LoadFile(foundfile), foundfile, true));
			
			// Find in any of the wad files
			for(int i = wads.Count - 1; i >= 0; i--)
				result.AddRange(wads[i].GetDecorateData(pname));

			return result;
		}

		#endregion

		#region ================== MODELDEF (mxd)

		//mxd
		public override IEnumerable<TextResourceData> GetModeldefData() 
		{
			// Error when suspended
			if(issuspended) throw new Exception("Data reader is suspended");

			// Modedef should be in root folder
			string[] files = GetAllFilesWhichTitleStartsWith("", "MODELDEF", false);
			List<TextResourceData> result = new List<TextResourceData>();

			// Add to collection
			foreach(string s in files)
				result.Add(new TextResourceData(this, LoadFile(s), s, true));

			// Find in any of the wad files
			foreach(WADReader wr in wads) result.AddRange(wr.GetModeldefData());

			return result;
		}

		#endregion 

		#region ================== VOXELDEF (mxd)

		//mxd. This returns the list of voxels, which can be used without VOXELDEF definition
		public override IEnumerable<string> GetVoxelNames() 
		{
			// Error when suspended
			if(issuspended) throw new Exception("Data reader is suspended");

			string[] files = GetAllFiles("voxels", false);
			List<string> voxels = new List<string>();
			Regex spritename = new Regex(SPRITE_NAME_PATTERN);

			foreach(string t in files)
			{
				string s = Path.GetFileNameWithoutExtension(t).ToUpperInvariant();
				if(spritename.IsMatch(s)) voxels.Add(s);
			}

			return voxels.ToArray();
		}

		//mxd
		public override IEnumerable<TextResourceData> GetVoxeldefData() 
		{
			// Error when suspended
			if(issuspended) throw new Exception("Data reader is suspended");

			// VOXELDEF should be in root folder
			string[] files = GetAllFilesWithTitle("", "VOXELDEF", false);
			List<TextResourceData> result = new List<TextResourceData>();

			// Add to collection
			foreach(string s in files) 
				result.Add(new TextResourceData(this, LoadFile(s), s, true));

			// Find in any of the wad files
			foreach(WADReader wr in wads) result.AddRange(wr.GetVoxeldefData());

			return result;
		}

		#endregion

		#region ================== (Z)MAPINFO (mxd)

		//mxd
		public override IEnumerable<TextResourceData> GetMapinfoData() 
		{
			// Error when suspended
			if(issuspended) throw new Exception("Data reader is suspended");

			// Mapinfo should be in root folder
			List<TextResourceData> result = new List<TextResourceData>();

			// Try to find (z)mapinfo
			string[] files = GetAllFilesWithTitle("", "ZMAPINFO", false);
			if(files.Length == 0) files = GetAllFilesWithTitle("", "MAPINFO", false);

			// Add to collection
			foreach(string s in files)
				result.Add(new TextResourceData(this, LoadFile(s), s, true));

			// Find in any of the wad files
			foreach(WADReader wr in wads) result.AddRange(wr.GetMapinfoData());

			return result;
		}

		#endregion

		#region ================== GLDEFS (mxd)

		//mxd
		public override IEnumerable<TextResourceData> GetGldefsData(GameType gametype) 
		{
			// Error when suspended
			if(issuspended) throw new Exception("Data reader is suspended");

			List<TextResourceData> result = new List<TextResourceData>();

			// At least one of gldefs should be in the root folder
			List<string> files = new List<string>();

			// Try to load game specific GLDEFS first
			if(gametype != GameType.UNKNOWN) 
			{
				string lumpname = Gldefs.GLDEFS_LUMPS_PER_GAME[(int)gametype];
				files.AddRange(GetAllFilesWhichTitleStartsWith("", lumpname, false));
			}

			// Can be several entries
			files.AddRange(GetAllFilesWhichTitleStartsWith("", "GLDEFS", false));

			// Add to collection
			foreach(string s in files)
				result.Add(new TextResourceData(this, LoadFile(s), s, true));

			// Find in any of the wad files
			foreach(WADReader wr in wads) result.AddRange(wr.GetGldefsData(gametype));

			return result;
		}

		#endregion

		#region ================== REVERBS (mxd)

		public override IEnumerable<TextResourceData> GetReverbsData() 
		{
			// Error when suspended
			if(issuspended) throw new Exception("Data reader is suspended");

			List<TextResourceData> result = new List<TextResourceData>();
			string[] files = GetAllFilesWithTitle("", "REVERBS", false);

			// Add to collection
			foreach(string s in files)
				result.Add(new TextResourceData(this, LoadFile(s), s, true));

			// Find in any of the wad files
			foreach(WADReader wr in wads) result.AddRange(wr.GetReverbsData());

			return result;
		}

		#endregion

		#region ================== SNDSEQ (mxd)

		public override IEnumerable<TextResourceData> GetSndSeqData() 
		{
			// Error when suspended
			if(issuspended) throw new Exception("Data reader is suspended");

			List<TextResourceData> result = new List<TextResourceData>();
			string[] files = GetAllFilesWithTitle("", "SNDSEQ", false);

			// Add to collection
			foreach(string s in files)
				result.Add(new TextResourceData(this, LoadFile(s), s, true));

			// Find in any of the wad files
			foreach(WADReader wr in wads) result.AddRange(wr.GetSndSeqData());

			return result;
		}

		#endregion

		#region ================== ANIMDEFS (mxd)

		public override IEnumerable<TextResourceData> GetAnimdefsData()
		{
			// Error when suspended
			if(issuspended) throw new Exception("Data reader is suspended");

			List<TextResourceData> result = new List<TextResourceData>();
			string[] files = GetAllFilesWithTitle("", "ANIMDEFS", false);

			// Add to collection
			foreach(string s in files)
				result.Add(new TextResourceData(this, LoadFile(s), s, true));

			// Find in any of the wad files
			foreach(WADReader wr in wads) result.AddRange(wr.GetAnimdefsData());

			return result;
		}

		#endregion

		#region ================== TERRAIN (mxd)

		public override IEnumerable<TextResourceData> GetTerrainData()
		{
			// Error when suspended
			if(issuspended) throw new Exception("Data reader is suspended");

			List<TextResourceData> result = new List<TextResourceData>();
			string[] files = GetAllFilesWithTitle("", "TERRAIN", false);

			// Add to collection
			foreach(string s in files)
				result.Add(new TextResourceData(this, LoadFile(s), s, true));

			// Find in any of the wad files
			foreach(WADReader wr in wads) result.AddRange(wr.GetTerrainData());

			return result;
		}

		#endregion

		#region ================== XBRSBSBB11 (mxd)

		public override IEnumerable<TextResourceData> GetX11R6RGBData()
		{
			// Error when suspended
			if(issuspended) throw new Exception("Data reader is suspended");

			List<TextResourceData> result = new List<TextResourceData>();
			string[] files = GetAllFilesWithTitle("", "X11R6RGB", false);

			// Add to collection
			foreach(string s in files)
				result.Add(new TextResourceData(this, LoadFile(s), s, true));

			// Find in any of the wad files
			foreach(WADReader wr in wads) result.AddRange(wr.GetX11R6RGBData());

			return result;
		}

		#endregion

		#region ================== CVARINFO (mxd)

		public override IEnumerable<TextResourceData> GetCvarInfoData()
		{
			// Error when suspended
			if(issuspended) throw new Exception("Data reader is suspended");

			List<TextResourceData> result = new List<TextResourceData>();
			string[] files = GetAllFilesWithTitle("", "CVARINFO", false);

			// Add to collection
			foreach(string s in files)
				result.Add(new TextResourceData(this, LoadFile(s), s, true));

			// Find in any of the wad files
			foreach(WADReader wr in wads) result.AddRange(wr.GetCvarInfoData());

			return result;
		}

		#endregion

		#region ================== Methods

		// This loads the images in this directory
		private ICollection<ImageData> LoadDirectoryImages(string path, int imagetype, bool includesubdirs)
		{
			List<ImageData> images = new List<ImageData>();
			
			// Go for all files
			string[] files = GetAllFiles(path, includesubdirs);
			foreach(string f in files)
			{
				if(string.IsNullOrEmpty(Path.GetFileNameWithoutExtension(f))) 
				{
					// Can't load image without name
					General.ErrorLogger.Add(ErrorType.Error, "Can't load an unnamed texture from \"" + path + "\". Please consider giving names to your resources.");
				} 
				else 
				{
					// Add image to list
					images.Add(CreateImage(f, imagetype));
				}
			}
			
			// Return result
			return images;
		}
		
		// This copies images from a collection unless they already exist in the list
		private static void AddImagesToList(Dictionary<long, ImageData> targetlist, IEnumerable<ImageData> sourcelist)
		{
			// Go for all source images
			foreach(ImageData src in sourcelist)
			{
				// Check if exists in target list
				if(!targetlist.ContainsKey(src.LongName))
					targetlist.Add(src.LongName, src);
			}
		}
		
		// This must create an image
		protected abstract ImageData CreateImage(string filename, int imagetype);

		// This must return all files in a given directory
		protected abstract string[] GetAllFiles(string path, bool subfolders);

		// This must return all files in a given directory that have the given file title
		protected abstract string[] GetAllFilesWithTitle(string path, string title, bool subfolders);

		//mxd. This must return all files in a given directory which title starts with given title
		protected abstract string[] GetAllFilesWhichTitleStartsWith(string path, string title, bool subfolders);

		// This must return all files in a given directory that match the given extension
		protected abstract string[] GetFilesWithExt(string path, string extension, bool subfolders);

		//mxd. This must return wad files in the root directory
		protected abstract string[] GetWadFiles();

		// This must find the first file that has the specific name, regardless of file extension
		internal abstract string FindFirstFile(string beginswith, bool subfolders);

		// This must find the first file that has the specific name, regardless of file extension
		protected abstract string FindFirstFile(string path, string beginswith, bool subfolders);

		// This must find the first file that has the specific name
		protected abstract string FindFirstFileWithExt(string path, string beginswith, bool subfolders);
	
		// This must create a temp file for the speciied file and return the absolute path to the temp file
		// NOTE: Callers are responsible for removing the temp file when done!
		protected abstract string CreateTempFile(string filename);

		// This makes the path relative to the directory, if needed
		protected virtual string MakeRelativePath(string anypath)
		{
			if(Path.IsPathRooted(anypath))
			{
				// Make relative
				string lowpath = anypath.ToLowerInvariant();
				string lowlocation = location.location.ToLowerInvariant();
				if((lowpath.Length > (lowlocation.Length + 1)) && lowpath.StartsWith(lowlocation))
					return anypath.Substring(lowlocation.Length + 1);
				else
					return anypath;
			}
			else
			{
				// Path is already relative
				return anypath;
			}
		}

		//mxd. Archives and Folders don't have lump indices
		internal override MemoryStream LoadFile(string name, int unused)
		{
			return LoadFile(name);
		}
		
		#endregion
	}
}
