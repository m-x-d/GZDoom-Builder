
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
	internal sealed class DirectoryReader : DataReader
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
		private string rootpath;
		private string patchespath;
		private string texturespath;
		private string flatspath;
		private string hirespath;
		private string spritespath;
		
		// WAD files that must be loaded as well
		private List<WADReader> wads;
		
		#endregion

		#region ================== Properties

		#endregion

		#region ================== Constructor / Disposer
		
		// Constructor
		public DirectoryReader(DataLocation dl) : base(dl)
		{
			// Initialize
			this.roottextures = dl.textures;
			this.rootflats = dl.flats;
			this.rootpath = dl.location;
			this.patchespath = Path.Combine(rootpath, PATCHES_DIR);
			this.texturespath = Path.Combine(rootpath, TEXTURES_DIR);
			this.flatspath = Path.Combine(rootpath, FLATS_DIR);
			this.hirespath = Path.Combine(rootpath, HIRES_DIR);
			this.spritespath = Path.Combine(rootpath, SPRITES_DIR);
			
			General.WriteLogLine("Opening directory resource '" + location.location + "'");
			
			// Load all WAD files in the root as WAD resources
			string[] wadfiles = Directory.GetFiles(rootpath, "*.wad", SearchOption.TopDirectoryOnly);
			wads = new List<WADReader>(wadfiles.Length);
			foreach(string w in wadfiles)
			{
				DataLocation wdl = new DataLocation(DataLocation.RESOURCE_WAD, w, false, false);
				wads.Add(new WADReader(wdl));
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
				// Clean up
				foreach(WADReader wr in wads) wr.Dispose();
				
				General.WriteLogLine("Closing directory resource '" + location.location + "'");
				
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
				collection = LoadDirectoryImages(rootpath);
				AddImagesToList(images, collection);
			}
			
			// TODO: Add support for hires texture here
			
			// Add images from texture directory
			collection = LoadDirectoryImages(texturespath);
			AddImagesToList(images, collection);
			
			return images;
		}
		
		// This returns the patch names from the PNAMES lump
		// A directory resource does not support this lump, but the wads in the directory may contain this lump
		public override PatchNames LoadPatchNames()
		{
			// Error when suspended
			if(issuspended) throw new Exception("Data reader is suspended");
			
			// Load from wad files (NOTE: backward order, because the last wad's images have priority)
			for(int i = wads.Count - 1; i >= 0; i--)
			{
				PatchNames pnames = wads[i].LoadPatchNames();
				if(pnames != null) return pnames;
			}
			
			return null;
		}
		
		// This finds and returns a patch stream
		public override Stream GetPatchData(string pname)
		{
			string filename;
			
			// Error when suspended
			if(issuspended) throw new Exception("Data reader is suspended");
			
			// Find in any of the wad files
			for(int i = wads.Count - 1; i >= 0; i--)
			{
				Stream data = wads[i].GetPatchData(pname);
				if(data != null) return data;
			}
			
			// Find in patches directory
			string datafile = null;
			filename = Path.Combine(patchespath, pname + ".bmp");
			if(File.Exists(filename)) datafile = filename;
			filename = Path.Combine(patchespath, pname + ".gif");
			if(File.Exists(filename)) datafile = filename;
			filename = Path.Combine(patchespath, pname + ".png");
			if(File.Exists(filename)) datafile = filename;
			filename = Path.Combine(patchespath, pname);
			if(File.Exists(filename)) datafile = filename;
			
			// Found anything?
			if(datafile != null)
			{
				byte[] filedata = File.ReadAllBytes(datafile);
				MemoryStream mem = new MemoryStream(filedata);
				return mem;
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
			
			// Load from wad files (NOTE: backward order, because the last wad's images have priority)
			for(int i = wads.Count - 1; i >= 0; i--)
			{
				collection = wads[i].LoadFlats();
				AddImagesToList(images, collection);
			}
			
			// Should we load the images in this directory as flats?
			if(rootflats)
			{
				collection = LoadDirectoryImages(rootpath);
				AddImagesToList(images, collection);
			}
			
			// Add images from flats directory
			collection = LoadDirectoryImages(flatspath);
			AddImagesToList(images, collection);
			
			return images;
		}
		
		#endregion
		
		#region ================== Sprites

		// This finds and returns a sprite stream
		public override Stream GetSpriteData(string pname)
		{
			string pfilename = pname.Replace('\\', '^');
			string filename;
			
			// Error when suspended
			if(issuspended) throw new Exception("Data reader is suspended");

			// Find in any of the wad files
			for(int i = wads.Count - 1; i >= 0; i--)
			{
				Stream sprite = wads[i].GetSpriteData(pname);
				if(sprite != null) return sprite;
			}
			
			// Find in sprites directory
			string spritefoundfile = null;
			filename = Path.Combine(spritespath, pfilename + ".bmp");
			if(File.Exists(filename)) spritefoundfile = filename;
			filename = Path.Combine(spritespath, pfilename + ".gif");
			if(File.Exists(filename)) spritefoundfile = filename;
			filename = Path.Combine(spritespath, pfilename + ".png");
			if(File.Exists(filename)) spritefoundfile = filename;
			
			// Found anything?
			if(spritefoundfile != null)
			{
				byte[] filedata = File.ReadAllBytes(spritefoundfile);
				MemoryStream mem = new MemoryStream(filedata);
				return mem;
			}
			
			// Nothing found
			return null;
		}
		
		#endregion
		
		#region ================== Methods
		
		// This loads the images in this directory
		private ICollection<ImageData> LoadDirectoryImages(string path)
		{
			List<ImageData> images = new List<ImageData>();
			string[] files;
			string name;

			// Find all BMP files
			files = Directory.GetFiles(path, "*.bmp", SearchOption.TopDirectoryOnly);

			// Find all GIF files and append to files array
			AddToArray(ref files, Directory.GetFiles(path, "*.gif", SearchOption.TopDirectoryOnly));

			// Find all PNG files and append to files array
			AddToArray(ref files, Directory.GetFiles(path, "*.png", SearchOption.TopDirectoryOnly));
			
			// Go for all files
			foreach(string f in files)
			{
				// Make the texture name from filename without extension
				name = Path.GetFileNameWithoutExtension(f).ToUpperInvariant();
				if(name.Length > 8) name = name.Substring(0, 8);
				
				// Add image to list
				images.Add(new FileImage(name, f));
			}
			
			// Return result
			return images;
		}
		
		// This resizes a string array and adds to it
		private void AddToArray(ref string[] array, string[] add)
		{
			int insertindex = array.Length;
			Array.Resize<string>(ref array, array.Length + add.Length);
			add.CopyTo(array, insertindex);
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
		
		#endregion
	}
}
