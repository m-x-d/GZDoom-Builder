
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
using CodeImp.DoomBuilder.IO;

#endregion

namespace CodeImp.DoomBuilder.Data
{
	internal sealed class DirectoryReader : PK3StructuredReader
	{
		#region ================== Variables

		private readonly DirectoryFilesList files;

		#endregion
		
		#region ================== Constructor / Disposer

		// Constructor
		public DirectoryReader(DataLocation dl) : base(dl)
		{
			General.WriteLogLine("Opening directory resource \"" + location.location + "\"");
			
			// Initialize
			files = new DirectoryFilesList(dl.location, true);
			Initialize();
			
			// We have no destructor
			GC.SuppressFinalize(this);
		}

		//mxd. Don't move directory wads anywhere
		protected override void Initialize()
		{
			// Load all WAD files in the root as WAD resources
			string[] wadfiles = GetFilesWithExt("", "wad", false);
			wads = new List<WADReader>(wadfiles.Length);
			foreach(string wadfile in wadfiles)
			{
				DataLocation wdl = new DataLocation(DataLocation.RESOURCE_WAD, Path.Combine(location.location, wadfile), false, false, true);
				wads.Add(new WADReader(wdl));
			}
		}

		// Disposer
		public override void Dispose()
		{
			// Not already disposed?
			if(!isdisposed)
			{
				General.WriteLogLine("Closing directory resource \"" + location.location + "\"");
				
				// Done
				base.Dispose();
			}
		}
		
		#endregion

		#region ================== Textures

		// This finds and returns a patch stream
		public override Stream GetPatchData(string pname, bool longname, ref string patchlocation)
		{
			// Error when suspended
			if(issuspended) throw new Exception("Data reader is suspended");

			// Find in any of the wad files
			// Note the backward order, because the last wad's images have priority
			if(!longname) //mxd. Patches with long names can't be in wads
			{
				for(int i = wads.Count - 1; i > -1; i--)
				{
					Stream data = wads[i].GetPatchData(pname, false, ref patchlocation);
					if(data != null) return data;
				}
			}

			try
			{
				if(longname)
				{
					//mxd. Long names are absolute
					pname = pname.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
					if(FileExists(pname))
					{
						patchlocation = location.GetDisplayName();
						return LoadFile(pname);
					}
					return null;
				}
				
				if(General.Map.Config.MixTexturesFlats)
				{
					//mxd. Find in directories ZDoom expects them to be
					string dir = Path.GetDirectoryName(pname);
					string name = Path.GetFileName(pname);
					foreach(string loc in PatchLocations)
					{
						string path = Path.Combine(loc, dir);
						string filename = FindFirstFile(path, name, true);
						if(!string.IsNullOrEmpty(filename) && FileExists(filename))
						{
							patchlocation = location.GetDisplayName(); //mxd
							return LoadFile(filename);
						}
					}
				}
				else
				{
					// Find in patches directory
					string path = Path.Combine(PATCHES_DIR, Path.GetDirectoryName(pname));
					string filename = FindFirstFile(path, Path.GetFileName(pname), true);
					if(!string.IsNullOrEmpty(filename) && FileExists(filename))
					{
						patchlocation = location.GetDisplayName(); //mxd
						return LoadFile(filename);
					}
				}
			}
			catch(Exception e)
			{
				General.ErrorLogger.Add(ErrorType.Error, e.GetType().Name + " while loading patch \"" + pname + "\" from directory: " + e.Message);
			}

			// Nothing found
			return null;
		}

		// This finds and returns a textue stream
		public override Stream GetTextureData(string pname, bool longname, ref string texturelocation)
		{
			// Error when suspended
			if(issuspended) throw new Exception("Data reader is suspended");

			// Find in any of the wad files
			// Note the backward order, because the last wad's images have priority
			if(!longname) //mxd. Textures with long names can't be in wads
			{
				for(int i = wads.Count - 1; i >= 0; i--)
				{
					Stream data = wads[i].GetTextureData(pname, false, ref texturelocation);
					if(data != null) return data;
				}
			}
			
			try
			{
				//mxd. Long names are absolute
				if(longname)
				{
					pname = pname.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
					if(FileExists(pname))
					{
						texturelocation = location.GetDisplayName();
						return LoadFile(pname);
					}
					return null;
				}
				else
				{
					// Find in textures directory
					string path = Path.Combine(TEXTURES_DIR, Path.GetDirectoryName(pname));
					string filename = FindFirstFile(path, Path.GetFileName(pname), true);
					if(!string.IsNullOrEmpty(filename) && FileExists(filename))
					{
						texturelocation = location.GetDisplayName(); //mxd
						return LoadFile(filename);
					}
				}
			}
			catch(Exception e)
			{
				General.ErrorLogger.Add(ErrorType.Error, e.GetType().Name + " while loading texture \"" + pname + "\" from directory: " + e.Message);
			}

			// Nothing found
			return null;
		}

		//mxd. This finds and returns a HiRes textue stream
		public override Stream GetHiResTextureData(string name, ref string hireslocation)
		{
			// Error when suspended
			if(issuspended) throw new Exception("Data reader is suspended");

			// Find in any of the wad files
			// Note the backward order, because the last wad's images have priority
			for(int i = wads.Count - 1; i >= 0; i--)
			{
				Stream data = wads[i].GetHiResTextureData(name, ref hireslocation);
				if(data != null) return data;
			}

			try
			{
				// Find in hires directory
				string filename = FindFirstFile(HIRES_DIR, name, true);
				if(!string.IsNullOrEmpty(filename) && FileExists(filename))
				{
					hireslocation = location.GetDisplayName();
					return LoadFile(filename);
				}
			}
			catch(Exception e)
			{
				General.ErrorLogger.Add(ErrorType.Error, e.GetType().Name + " while loading HiRes texture \"" + name + "\" from directory: " + e.Message);
			}

			// Nothing found
			return null;
		}

		// This finds and returns a colormap stream
		public override Stream GetColormapData(string pname)
		{
			// Error when suspended
			if(issuspended) throw new Exception("Data reader is suspended");

			// Find in any of the wad files
			// Note the backward order, because the last wad's images have priority
			for(int i = wads.Count - 1; i >= 0; i--)
			{
				Stream data = wads[i].GetColormapData(pname);
				if(data != null) return data;
			}

			try
			{
				// Find in patches directory
				string path = Path.Combine(COLORMAPS_DIR, Path.GetDirectoryName(pname));
				string filename = FindFirstFile(path, Path.GetFileName(pname), true);
				if((filename != null) && FileExists(filename))
				{
					return LoadFile(filename);
				}
			}
			catch(Exception e)
			{
				General.ErrorLogger.Add(ErrorType.Error, e.GetType().Name + " while loading colormap \"" + pname + "\" from directory: " + e.Message);
			}

			// Nothing found
			return null;
		}

		#endregion

		#region ================== Sprites

		// This finds and returns a sprite stream
		public override Stream GetSpriteData(string pname, ref string spritelocation)
		{
			// Error when suspended
			if(issuspended) throw new Exception("Data reader is suspended");

			// Find in any of the wad files
			for(int i = wads.Count - 1; i >= 0; i--)
			{
				Stream sprite = wads[i].GetSpriteData(pname, ref spritelocation);
				if(sprite != null) return sprite;
			}
			
			try
			{
				// Find in sprites directory
				string path = Path.Combine(SPRITES_DIR, Path.GetDirectoryName(pname));
				string filename = FindFirstFile(path, Path.GetFileName(pname), true);
				if((filename != null) && FileExists(filename))
				{
					spritelocation = location.GetDisplayName(); //mxd
					return LoadFile(filename);
				}
			}
			catch(Exception e)
			{
				General.ErrorLogger.Add(ErrorType.Error, e.GetType().Name + " while loading sprite \"" + pname + "\" from directory: " + e.Message);
			}
			
			// Nothing found
			return null;
		}

		// This checks if the given sprite exists
		public override bool GetSpriteExists(string pname)
		{
			// Error when suspended
			if(issuspended) throw new Exception("Data reader is suspended");

			// Find in any of the wad files
			for(int i = wads.Count - 1; i >= 0; i--)
			{
				if(wads[i].GetSpriteExists(pname)) return true;
			}

			// Find in sprites directory
			try
			{
				string path = Path.Combine(SPRITES_DIR, Path.GetDirectoryName(pname));
				string filename = FindFirstFile(path, Path.GetFileName(pname), true);
				if((filename != null) && FileExists(filename))
				{
					return true;
				}
			}
			catch(Exception e)
			{
				General.ErrorLogger.Add(ErrorType.Error, e.GetType().Name + " while checking sprite \"" + pname + "\" existance in directory: " + e.Message);
			}
			
			// Nothing found
			return false;
		}

		#endregion

		#region ================== Voxels (mxd)

		//mxd.  This finds and returns a voxel stream
		public override Stream GetVoxelData(string name)
		{
			// Error when suspended
			if(issuspended) throw new Exception("Data reader is suspended");

			// Find in any of the wad files
			for(int i = wads.Count - 1; i >= 0; i--)
			{
				Stream voxel = wads[i].GetVoxelData(name);
				if(voxel != null) return voxel;
			}

			try
			{
				// Find in voxels directory
				string path = Path.Combine(VOXELS_DIR, Path.GetDirectoryName(name));
				string filename = FindFirstFile(path, Path.GetFileName(name), true);
				if((filename != null) && FileExists(filename))
				{
					return LoadFile(filename);
				}
			}
			catch(Exception e)
			{
				General.ErrorLogger.Add(ErrorType.Error, e.GetType().Name + " while loading voxel \"" + name + "\" from directory: " + e.Message);
			}

			// Nothing found
			return null;
		}

		#endregion

		#region ================== Methods

		// Return a short name for this data location
		public override string GetTitle()
		{
			return Path.GetFileName(location.location);
		}

		// This creates an image
		protected override ImageData CreateImage(string filename, int imagetype)
		{
			switch(imagetype)
			{
				case ImageDataFormat.DOOMFLAT:
					return new FileImage(filename, Path.Combine(location.location, filename), true);
				
				case ImageDataFormat.DOOMPICTURE:
					return new FileImage(filename, Path.Combine(location.location, filename), false);
					
				case ImageDataFormat.DOOMCOLORMAP:
					return new ColormapImage(Path.GetFileNameWithoutExtension(filename));

				default:
					throw new ArgumentException("Invalid image format specified!");
			}
		}

		// This returns true if the specified file exists
		internal override bool FileExists(string filename)
		{
			return files.FileExists(filename);
		}
		
		// This returns all files in a given directory
		protected override string[] GetAllFiles(string path, bool subfolders)
		{
			return files.GetAllFiles(path, subfolders).ToArray();
		}
		
		// This returns all files in a given directory that have the given file title
		protected override string[] GetAllFilesWithTitle(string path, string title, bool subfolders)
		{
			return files.GetAllFilesWithTitle(path, title, subfolders).ToArray();
		}

		//mxd. This returns all files in a given directory which title starts with given title
		protected override string[] GetAllFilesWhichTitleStartsWith(string path, string title, bool subfolders) 
		{
			return files.GetAllFilesWhichTitleStartsWith(path, title, subfolders).ToArray();
		}
		
		// This returns all files in a given directory that match the given extension
		protected override string[] GetFilesWithExt(string path, string extension, bool subfolders)
		{
			return files.GetAllFiles(path, extension, subfolders).ToArray();
		}

		// This finds the first file that has the specific name, regardless of file extension
		internal override string FindFirstFile(string beginswith, bool subfolders)
		{
			return files.GetFirstFile(beginswith, subfolders);
		}

		// This finds the first file that has the specific name, regardless of file extension
		protected override string FindFirstFile(string path, string beginswith, bool subfolders)
		{
			return files.GetFirstFile(path, beginswith, subfolders);
		}

		// This finds the first file that has the specific name
		protected override string FindFirstFileWithExt(string path, string beginswith, bool subfolders)
		{
			string title = Path.GetFileNameWithoutExtension(beginswith);
			string ext = Path.GetExtension(beginswith);
			ext = (!string.IsNullOrEmpty(ext) && ext.Length > 1 ? ext.Substring(1) : string.Empty);
			return files.GetFirstFile(path, title, subfolders, ext);
		}
		
		// This loads an entire file in memory and returns the stream
		// NOTE: Callers are responsible for disposing the stream!
		internal override MemoryStream LoadFile(string filename)
		{
			MemoryStream s = null;

			try 
			{
				lock(this)
				{
					s = new MemoryStream(File.ReadAllBytes(Path.Combine(location.location, filename)));
				}
			} 
			catch(Exception e) 
			{
				General.ErrorLogger.Add(ErrorType.Error, "Unable to load file: " + e.Message);
			}
			return s;
		}

		// This creates a temp file for the speciied file and return the absolute path to the temp file
		// NOTE: Callers are responsible for removing the temp file when done!
		protected override string CreateTempFile(string filename)
		{
			// Just copy the file
			string tempfile = General.MakeTempFilename(General.Map.TempPath, "wad");
			File.Copy(Path.Combine(location.location, filename), tempfile);
			return tempfile;
		}

		#endregion
	}
}
