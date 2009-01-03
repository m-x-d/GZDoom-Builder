
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

#endregion

namespace CodeImp.DoomBuilder.Data
{
	internal abstract class PK3StructuredReader : DataReader
	{
		#region ================== Constants

		private const string PATCHES_DIR = "patches";
		private const string TEXTURES_DIR = "textures";
		private const string FLATS_DIR = "flats";
		private const string HIRES_DIR = "hires";
		private const string SPRITES_DIR = "sprites";
		
		#endregion

		#region ================== Variables
		
		// Source
		private bool roottextures;
		private bool rootflats;
		
		// Paths
		protected string rootpath;
		protected string patchespath;
		protected string texturespath;
		protected string flatspath;
		protected string hirespath;
		protected string spritespath;
		
		// WAD files that must be loaded as well
		private List<WADReader> wads;
		
		#endregion

		#region ================== Properties

		#endregion

		#region ================== Constructor / Disposer
		
		// Constructor
		public PK3StructuredReader(DataLocation dl) : base(dl)
		{
			// Initialize
			this.roottextures = dl.textures;
			this.rootflats = dl.flats;
		}
		
		// Call this to initialize this class
		protected virtual void Initialize(string rootpath)
		{
			// Initialize
			this.rootpath = rootpath;
			this.patchespath = Path.Combine(rootpath, PATCHES_DIR);
			this.texturespath = Path.Combine(rootpath, TEXTURES_DIR);
			this.flatspath = Path.Combine(rootpath, FLATS_DIR);
			this.hirespath = Path.Combine(rootpath, HIRES_DIR);
			this.spritespath = Path.Combine(rootpath, SPRITES_DIR);
			
			// Load all WAD files in the root as WAD resources
			string[] wadfiles = GetFilesWithExt(rootpath, "wad");
			wads = new List<WADReader>(wadfiles.Length);
			foreach(string w in wadfiles)
			{
				string tempfile = CreateTempFile(w);
				DataLocation wdl = new DataLocation(DataLocation.RESOURCE_WAD, tempfile, false, false);
				wads.Add(new WADReader(wdl));
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
				
				// Remove temp files
				foreach(WADReader wr in wads)
				{
					try { File.Delete(wr.Location.location); }
					catch(Exception) { }
				}
				
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
				if(wadpalette != null) palette = wadpalette;
			}
			
			// Done
			return palette;
		}

		#endregion
		
		#region ================== Textures

		// This loads the textures
		public override ICollection<ImageData> LoadTextures(PatchNames pnames)
		{
			List<ImageData> images = new List<ImageData>();
			ICollection<ImageData> collection;
			
			// Error when suspended
			if(issuspended) throw new Exception("Data reader is suspended");
			
			// Load from wad files (NOTE: backward order, because the last wad's images have priority)
			for(int i = wads.Count - 1; i >= 0; i--)
			{
				collection = wads[i].LoadTextures(pnames);
				AddImagesToList(images, collection);
			}
			
			// Should we load the images in this directory as textures?
			if(roottextures)
			{
				collection = LoadDirectoryImages(rootpath, false);
				AddImagesToList(images, collection);
			}
			
			// TODO: Add support for hires texture here
			
			// Add images from texture directory
			collection = LoadDirectoryImages(texturespath, false);
			AddImagesToList(images, collection);

			// Load TEXTURE1 lump file
			List<ImageData> imgset = new List<ImageData>();
			string texture1file = FindFirstFile(rootpath, "TEXTURE1");
			if((texture1file != null) && FileExists(texture1file))
			{
				MemoryStream filedata = LoadFile(texture1file);
				WADReader.LoadTextureSet(filedata, ref imgset, pnames);
				filedata.Dispose();
			}

			// Load TEXTURE2 lump file
			string texture2file = FindFirstFile(rootpath, "TEXTURE2");
			if((texture2file != null) && FileExists(texture2file))
			{
				MemoryStream filedata = LoadFile(texture2file);
				WADReader.LoadTextureSet(filedata, ref imgset, pnames);
				filedata.Dispose();
			}

			// Add images from TEXTURE1 and TEXTURE2 lump files
			AddImagesToList(images, imgset);
			
			return images;
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
			string pnamesfile = FindFirstFile(rootpath, "PNAMES");
			if((pnamesfile != null) && FileExists(pnamesfile))
			{
				MemoryStream pnamesdata = LoadFile(pnamesfile);
				pnames = new PatchNames(pnamesdata);
				pnamesdata.Dispose();
				return pnames;
			}
			
			return null;
		}
		
		// This finds and returns a patch stream
		public override Stream GetPatchData(string pname)
		{
			// Error when suspended
			if(issuspended) throw new Exception("Data reader is suspended");
			
			// Find in any of the wad files
			// Note the backward order, because the last wad's images have priority
			for(int i = wads.Count - 1; i >= 0; i--)
			{
				Stream data = wads[i].GetPatchData(pname);
				if(data != null) return data;
			}
			
			// Find in patches directory
			string filename = FindFirstFile(patchespath, pname);
			if((filename != null) && FileExists(filename))
			{
				return LoadFile(filename);
			}
			
			// Nothing found
			return null;
		}

		#endregion

		#region ================== Flats
		
		// This loads the textures
		public override ICollection<ImageData> LoadFlats()
		{
			List<ImageData> images = new List<ImageData>();
			ICollection<ImageData> collection;
			
			// Error when suspended
			if(issuspended) throw new Exception("Data reader is suspended");
			
			// Load from wad files
			// Note the backward order, because the last wad's images have priority
			for(int i = wads.Count - 1; i >= 0; i--)
			{
				collection = wads[i].LoadFlats();
				AddImagesToList(images, collection);
			}
			
			// Should we load the images in this directory as flats?
			if(rootflats)
			{
				collection = LoadDirectoryImages(rootpath, true);
				AddImagesToList(images, collection);
			}
			
			// Add images from flats directory
			collection = LoadDirectoryImages(flatspath, true);
			AddImagesToList(images, collection);
			
			return images;
		}
		
		#endregion
		
		#region ================== Sprites

		// This finds and returns a sprite stream
		public override Stream GetSpriteData(string pname)
		{
			string pfilename = pname.Replace('\\', '^');
			
			// Error when suspended
			if(issuspended) throw new Exception("Data reader is suspended");

			// Find in any of the wad files
			for(int i = wads.Count - 1; i >= 0; i--)
			{
				Stream sprite = wads[i].GetSpriteData(pname);
				if(sprite != null) return sprite;
			}
			
			// Find in sprites directory
			string filename = FindFirstFile(spritespath, pfilename);
			if((filename != null) && FileExists(filename))
			{
				return LoadFile(filename);
			}
			
			// Nothing found
			return null;
		}
		
		#endregion
		
		#region ================== Methods
		
		// This loads the images in this directory
		private ICollection<ImageData> LoadDirectoryImages(string path, bool flats)
		{
			List<ImageData> images = new List<ImageData>();
			string[] files;
			string name;

			// Go for all files
			files = GetAllFiles(path);
			foreach(string f in files)
			{
				// Make the texture name from filename without extension
				name = Path.GetFileNameWithoutExtension(f).ToUpperInvariant();
				if(name.Length > 8) name = name.Substring(0, 8);
				
				// Add image to list
				images.Add(CreateImage(name, f, flats));
			}
			
			// Return result
			return images;
		}
		
		// This copies images from a collection unless they already exist in the list
		private void AddImagesToList(List<ImageData> targetlist, ICollection<ImageData> sourcelist)
		{
			// Go for all source images
			foreach(ImageData src in sourcelist)
			{
				// Check if exists in target list
				bool alreadyexists = false;
				foreach(ImageData tgt in targetlist)
				{
					if(tgt.LongName == src.LongName)
					{
						alreadyexists = true;
						break;
					}
				}
				
				// Add source image to target list
				if(!alreadyexists) targetlist.Add(src);
			}
		}
		
		// This must create an image
		protected abstract ImageData CreateImage(string name, string filename, bool flat);

		// This must return true if the specified file exists
		protected abstract bool FileExists(string filename);
		
		// This must return all files in a given directory
		protected abstract string[] GetAllFiles(string path);

		// This must return all files in a given directory that match the given extension
		protected abstract string[] GetFilesWithExt(string path, string extension);

		// This must find the first file that has the specific name, regardless of file extension
		protected abstract string FindFirstFile(string path, string beginswith);
		
		// This must load an entire file in memory and returns the stream
		// NOTE: Callers are responsible for disposing the stream!
		protected abstract MemoryStream LoadFile(string filename);

		// This must create a temp file for the speciied file and return the absolute path to the temp file
		// NOTE: Callers are responsible for removing the temp file when done!
		protected abstract string CreateTempFile(string filename);
		
		#endregion
	}
}
