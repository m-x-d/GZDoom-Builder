
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
using SharpCompress.Archive; //mxd
using SharpCompress.Common; //mxd
using SharpCompress.Reader; //mxd

#endregion

namespace CodeImp.DoomBuilder.Data
{
	internal sealed class PK3Reader : PK3StructuredReader
	{
		#region ================== Variables

		private readonly DirectoryFilesList files;
		private IArchive archive; //mxd
		private readonly ArchiveType archivetype; //mxd
		private readonly Dictionary<string, byte[]> sevenzipentries; //mxd
		private bool bathmode = true; //mxd

		#endregion

		#region ================== Properties (mxd)

		public bool BathMode { get { return bathmode; } set { bathmode = value; UpdateArchive(bathmode); } }

		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		public PK3Reader(DataLocation dl) : base(dl)
		{
			General.WriteLogLine("Opening " + Path.GetExtension(location.location).ToUpper().Replace(".", "") + " resource \"" + location.location + "\"");

			if(!File.Exists(location.location))
				throw new FileNotFoundException("Could not find the file \"" + location.location + "\"", location.location);

			// Make list of all files
			List<DirectoryFileEntry> fileentries = new List<DirectoryFileEntry>();

			// Create archive
			archive = ArchiveFactory.Open(location.location, Options.KeepStreamsOpen);
			archivetype = archive.Type;

			// Random access of 7z archives works TERRIBLY slow in SharpCompress
			if(archivetype == ArchiveType.SevenZip) 
			{
				sevenzipentries = new Dictionary<string, byte[]>(StringComparer.Ordinal);

				IReader reader = archive.ExtractAllEntries();
				while(reader.MoveToNextEntry()) 
				{
					if(reader.Entry.IsDirectory || !CheckInvalidPathChars(reader.Entry.Key)) continue;

					MemoryStream s = new MemoryStream();
					reader.WriteEntryTo(s);
					sevenzipentries.Add(reader.Entry.Key.ToLowerInvariant(), s.ToArray());
					fileentries.Add(new DirectoryFileEntry(reader.Entry.Key));
				}
			} 
			else 
			{
				foreach(IArchiveEntry entry in archive.Entries) 
				{
					if(!entry.IsDirectory && CheckInvalidPathChars(entry.Key))
						fileentries.Add(new DirectoryFileEntry(entry.Key));
				}
			}

			// Get rid of archive
			archive.Dispose();
			archive = null;

			// Make files list
			files = new DirectoryFilesList(fileentries);
			
			// Initialize without path (because we use paths relative to the PK3 file)
			Initialize();

			// We have no destructor
			GC.SuppressFinalize(this);
		}

		// Disposer
		public override void Dispose()
		{
			// Not already disposed?
			if(!isdisposed)
			{
				General.WriteLogLine("Closing " + Path.GetExtension(location.location).ToUpper().Replace(".", "") + " resource \"" + location.location + "\"");

				//mxd. Remove temp files
				foreach(WADReader wr in wads)
				{
					try { File.Delete(wr.Location.location); }
					catch(Exception) { }
				}

				//mxd
				if(archive != null)
				{
					archive.Dispose();
					archive = null;
				}

				// Done
				base.Dispose();
			}
		}

		//mxd
		private void UpdateArchive(bool enable) 
		{
			if(archivetype == ArchiveType.SevenZip) return;

			if(enable && archive == null)
			{
				archive = ArchiveFactory.Open(location.location);
			} 
			else if(!enable && !bathmode && archive != null)
			{
				archive.Dispose();
				archive = null;
			}
		}

		#endregion

		#region ================== Textures

		// This finds and returns a patch stream
		public override Stream GetPatchData(string pname, bool longname)
		{
			// Error when suspended
			if(issuspended) throw new Exception("Data reader is suspended");

			// Find in any of the wad files
			// Note the backward order, because the last wad's images have priority
			if(!longname) //mxd. Patches with long names can't be in wads
			{
				for(int i = wads.Count - 1; i >= 0; i--)
				{
					Stream data = wads[i].GetPatchData(pname, false);
					if(data != null) return data;
				}
			}
			else
			{
				//mxd. Long names are absolute
				return (FileExists(pname) ? LoadFile(pname) : null);
			}

			if(General.Map.Config.MixTexturesFlats)
			{
				//mxd. Find in directories ZDoom expects them to be
				foreach(string loc in PatchLocations)
				{
					string filename = FindFirstFile(loc, pname, true);
					if((filename != null) && FileExists(filename)) 
						return LoadFile(filename);
				}
			}
			else
			{
				// Find in patches directory
				string filename = FindFirstFile(PATCHES_DIR, pname, true);
				if((filename != null) && FileExists(filename))
					return LoadFile(filename);
			}

			// Nothing found
			return null;
		}

		// This finds and returns a textue stream
		public override Stream GetTextureData(string pname, bool longname)
		{
			// Error when suspended
			if(issuspended) throw new Exception("Data reader is suspended");

			// Find in any of the wad files
			// Note the backward order, because the last wad's images have priority
			if(!longname) //mxd. Textures with long names can't be in wads
			{
				for(int i = wads.Count - 1; i >= 0; i--)
				{
					Stream data = wads[i].GetTextureData(pname, false);
					if(data != null) return data;
				}
			}
			else
			{
				//mxd. Long names are absolute
				return (FileExists(pname) ? LoadFile(pname) : null);
			}

			// Find in textures directory
			string filename = FindFirstFile(TEXTURES_DIR, pname, true);
			if(!string.IsNullOrEmpty(filename) && FileExists(filename))
				return LoadFile(filename);

			// Nothing found
			return null;
		}

		//mxd. This finds and returns a HiRes textue stream
		public override Stream GetHiResTextureData(string pname)
		{
			// Error when suspended
			if(issuspended) throw new Exception("Data reader is suspended");

			// Find in any of the wad files
			// Note the backward order, because the last wad's images have priority
			for(int i = wads.Count - 1; i >= 0; i--)
			{
				Stream data = wads[i].GetTextureData(pname, false);
				if(data != null) return data;
			}

			// Find in HiRes directory
			string filename = FindFirstFile(HIRES_DIR, pname, false);
			if(!string.IsNullOrEmpty(filename) && FileExists(filename))
				return LoadFile(filename);

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

			// Find in patches directory
			string filename = FindFirstFile(COLORMAPS_DIR, pname, true);
			if((filename != null) && FileExists(filename))
			{
				return LoadFile(filename);
			}

			// Nothing found
			return null;
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
			string filename = FindFirstFile(SPRITES_DIR, pfilename, true);
			if((filename != null) && FileExists(filename))
			{
				return LoadFile(filename);
			}

			// Nothing found
			return null;
		}

		// This checks if the given sprite exists
		public override bool GetSpriteExists(string pname)
		{
			// Error when suspended
			if(issuspended) throw new Exception("Data reader is suspended");

			string pfilename = pname.Replace('\\', '^');

			// Find in any of the wad files
			for(int i = wads.Count - 1; i >= 0; i--)
			{
				if(wads[i].GetSpriteExists(pname)) return true;
			}

			// Find in sprites directory
			string filename = FindFirstFile(SPRITES_DIR, pfilename, true);
			if((filename != null) && FileExists(filename))
			{
				return true;
			}

			// Nothing found
			return false;
		}

		#endregion

		#region ================== Voxels (mxd)

		//mxd. This finds and returns a voxel stream or null if no voxel was found
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

			string pfilename = name.Replace('\\', '^');

			// Find in sprites directory
			string filename = FindFirstFile(VOXELS_DIR, pfilename, true);
			if((filename != null) && FileExists(filename)) 
			{
				return LoadFile(filename);
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
					return new PK3FileImage(this, filename, true);

				case ImageDataFormat.DOOMPICTURE:
					return new PK3FileImage(this, filename, false);

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
		
		// This returns all files in a given directory that have the given title
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
			ext = (!string.IsNullOrEmpty(ext) && ext.Length > 1 ? ext.Substring(1) : "");
			return files.GetFirstFile(path, title, subfolders, ext);
		}

		// This loads an entire file in memory and returns the stream
		// NOTE: Callers are responsible for disposing the stream!
		internal override MemoryStream LoadFile(string filename)
		{
			MemoryStream filedata = null;
			string fn = filename.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar); //mxd

			//mxd. This works waaaaaay faster with 7z archive
			if(archivetype == ArchiveType.SevenZip)
			{
				fn = fn.ToLowerInvariant();
				if(sevenzipentries.ContainsKey(fn)) filedata = new MemoryStream(sevenzipentries[fn]);
			} 
			else 
			{
				lock(this)
				{
					UpdateArchive(true);

					foreach(var entry in archive.Entries)
					{
						if(entry.IsDirectory) continue;

						// Is this the entry we are looking for?
						if(string.Compare(entry.Key, fn, true) == 0)
						{
							filedata = new MemoryStream();
							entry.WriteTo(filedata);
							break;
						}
					}

					UpdateArchive(false);
				}
			}
			
			// Nothing found?
			if(filedata == null)
			{
				//mxd
				General.ErrorLogger.Add(ErrorType.Error, "Cannot find the file \"" + filename + "\" in archive \"" + location.location + "\".");
				return null;
			}

			filedata.Position = 0; //mxd. rewind before use
			return filedata;
		}

		// This creates a temp file for the speciied file and return the absolute path to the temp file
		// NOTE: Callers are responsible for removing the temp file when done!
		protected override string CreateTempFile(string filename)
		{
			// Just copy the file
			string tempfile = General.MakeTempFilename(General.Map.TempPath, "wad");
			MemoryStream filedata = LoadFile(filename);
			File.WriteAllBytes(tempfile, filedata.ToArray());
			filedata.Dispose();
			return tempfile;
		}

		//mxd. This replicates System.IO.Path.CheckInvalidPathChars() internal function
		private bool CheckInvalidPathChars(string path)
		{
			foreach(char c in path)
			{
				int num = c;
				switch(num)
				{
					case 34:
					case 60:
					case 62:
					case 124:
						General.ErrorLogger.Add(ErrorType.Error, "Error in \"" + location.location + "\": unsupported character \"" + c + "\" in path \"" + path + "\". File loading was skipped.");
						return false;

					default:
						if(num >= 32) continue;
						else goto case 34;
				}
			}

			return true;
		}

		#endregion
	}
}
