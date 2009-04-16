
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
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using CodeImp.DoomBuilder.IO;
using SevenZip;

#endregion

namespace CodeImp.DoomBuilder.Data
{
	internal sealed class PK3Reader : PK3StructuredReader
	{
		#region ================== Variables

		private DirectoryFilesList files;

		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		public PK3Reader(DataLocation dl) : base(dl)
		{
			General.WriteLogLine("Opening PK3 resource '" + location.location + "'");
			
			// Open the zip file
			SevenZipExtractor zipstream = OpenPK3File();

            // Make list of all files
            ReadOnlyCollection<ArchiveFileInfo> filedata = zipstream.ArchiveFileData;
            List<DirectoryFileEntry> fileentries = new List<DirectoryFileEntry>(filedata.Count);
			
			foreach(ArchiveFileInfo entry in filedata)
			{
				if(!entry.IsDirectory) fileentries.Add(new DirectoryFileEntry(entry.FileName));
			}

			// Make files list
			files = new DirectoryFilesList(fileentries);

			// Done with the zip file
			zipstream.Dispose();
			
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
				General.WriteLogLine("Closing PK3 resource '" + location.location + "'");
				
				// Done
				base.Dispose();
			}
		}

		#endregion

		#region ================== Management

		// This opens the zip file for reading
		private SevenZipExtractor OpenPK3File()
		{
			FileStream filestream = File.Open(location.location, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            InArchiveFormat tryas;
			filestream.Seek(0, SeekOrigin.Begin);
            tryas = InArchiveFormat.Zip;
            if (filestream.ReadByte() == (byte)'7' &&
                filestream.ReadByte() == (byte)'z' &&
                filestream.ReadByte() == 0xBC &&
                filestream.ReadByte() == 0xAF &&
                filestream.ReadByte() == 0x27 &&
                filestream.ReadByte() == 0x1C)
            {
                tryas = InArchiveFormat.SevenZip;
            }
			filestream.Seek(0, SeekOrigin.Begin);
			return new SevenZipExtractor(filestream, tryas);
		}

		#endregion

		#region ================== Textures

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
			string filename = FindFirstFile(PATCHES_DIR, pname, true);
			if((filename != null) && FileExists(filename))
			{
				return LoadFile(filename);
			}

			// Nothing found
			return null;
		}

		// This finds and returns a textue stream
		public override Stream GetTextureData(string pname)
		{
			// Error when suspended
			if(issuspended) throw new Exception("Data reader is suspended");

			// Find in any of the wad files
			// Note the backward order, because the last wad's images have priority
			for(int i = wads.Count - 1; i >= 0; i--)
			{
				Stream data = wads[i].GetTextureData(pname);
				if(data != null) return data;
			}

			// Find in patches directory
			string filename = FindFirstFile(TEXTURES_DIR, pname, true);
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
			string pfilename = pname.Replace('\\', '^');

			// Error when suspended
			if(issuspended) throw new Exception("Data reader is suspended");

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
		
		#region ================== Methods

		// Return a short name for this data location
		public override string GetTitle()
		{
			return Path.GetFileName(location.location);
		}

		// This creates an image
		protected override ImageData CreateImage(string name, string filename, bool flat)
		{
			return new PK3FileImage(this, name, filename, flat);
		}

		// This returns true if the specified file exists
		protected override bool FileExists(string filename)
		{
			return files.FileExists(filename);
		}

		// This returns all files in a given directory
		protected override string[] GetAllFiles(string path, bool subfolders)
		{
			return files.GetAllFiles(path, subfolders).ToArray();
		}

		// This returns all files in a given directory that match the given extension
		protected override string[] GetFilesWithExt(string path, string extension, bool subfolders)
		{
			return files.GetAllFiles(path, extension, subfolders).ToArray();
		}

		// This finds the first file that has the specific name, regardless of file extension
		protected override string FindFirstFile(string beginswith, bool subfolders)
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
			if(ext.Length > 1) ext = ext.Substring(1); else ext = "";
			return files.GetFirstFile(path, title, subfolders, ext);
		}

		// This loads an entire file in memory and returns the stream
		// NOTE: Callers are responsible for disposing the stream!
		protected override MemoryStream LoadFile(string filename)
		{
			MemoryStream filedata = null;

			// Open the zip file
			SevenZipExtractor zipstream = OpenPK3File();
            ReadOnlyCollection<ArchiveFileInfo> fileinfo = zipstream.ArchiveFileData;

			foreach(ArchiveFileInfo entry in fileinfo)
			{
				if(!entry.IsDirectory)
				{
					string entryname = entry.FileName.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
					
					// Is this the entry we are looking for?
					if(string.Compare(entryname, filename, true) == 0)
					{
						filedata = new MemoryStream((int)entry.Size);
                        zipstream.ExtractFile(entry.Index, filedata, true);
						break;
					}
				}
			}

			// Done with the zip file
			zipstream.Dispose();
			
			// Nothing found?
			if(filedata == null)
			{
				throw new FileNotFoundException("Cannot find the file " + filename + " in PK3 file " + location.location + ".");
			}
			else
			{
				return filedata;
			}
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

		// Public version to load a file
		internal MemoryStream ExtractFile(string filename)
		{
			return LoadFile(filename);
		}
		
		#endregion
	}
}
