
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
	internal sealed class DirectoryReader : PK3StructuredReader
	{
		#region ================== Constructor / Disposer
		
		// Constructor
		public DirectoryReader(DataLocation dl) : base(dl)
		{
			General.WriteLogLine("Opening directory resource '" + location.location + "'");
			
			// Initialize
			Initialize(dl.location);
			
			// We have no destructor
			GC.SuppressFinalize(this);
		}
		
		// Disposer
		public override void Dispose()
		{
			// Not already disposed?
			if(!isdisposed)
			{
				General.WriteLogLine("Closing directory resource '" + location.location + "'");
				
				// Done
				base.Dispose();
			}
		}
		
		#endregion
		
		#region ================== Methods
		
		// This creates an image
		protected override ImageData CreateImage(string name, string filename, bool flat)
		{
			return new FileImage(name, filename, flat);
		}

		// This returns true if the specified file exists
		protected override bool FileExists(string filename)
		{
			return File.Exists(filename);
		}
		
		// This returns all files in a given directory
		protected override string[] GetAllFiles(string path, bool subfolders)
		{
			if(Directory.Exists(path))
			{
				if(subfolders)
					return Directory.GetFiles(path, "*", SearchOption.AllDirectories);
				else
					return Directory.GetFiles(path, "*", SearchOption.TopDirectoryOnly);
			}
			else
				return new string[0];
		}

		// This returns all files in a given directory that match the given extension
		protected override string[] GetFilesWithExt(string path, string extension, bool subfolders)
		{
			if(Directory.Exists(path))
			{
				if(subfolders)
					return Directory.GetFiles(path, "*." + extension, SearchOption.AllDirectories);
				else
					return Directory.GetFiles(path, "*." + extension, SearchOption.TopDirectoryOnly);
			}
			else
				return new string[0];
		}

		// This finds the first file that has the specific name, regardless of file extension
		protected override string FindFirstFile(string path, string beginswith, bool subfolders)
		{
			string[] files = GetAllFiles(path, subfolders);
			foreach(string f in files)
			{
				if(string.Compare(Path.GetFileNameWithoutExtension(f), beginswith, true) == 0)
					return f;
			}
			
			return null;
		}

		// This finds the first file that has the specific name
		protected override string FindFirstFileWithExt(string path, string beginswith, bool subfolders)
		{
			string[] files = GetAllFiles(path, subfolders);
			foreach(string f in files)
			{
				if(string.Compare(Path.GetFileName(f), beginswith, true) == 0)
					return f;
			}
			
			return null;
		}
		
		// This loads an entire file in memory and returns the stream
		// NOTE: Callers are responsible for disposing the stream!
		protected override MemoryStream LoadFile(string filename)
		{
			return new MemoryStream(File.ReadAllBytes(filename));
		}

		// This creates a temp file for the speciied file and return the absolute path to the temp file
		// NOTE: Callers are responsible for removing the temp file when done!
		protected override string CreateTempFile(string filename)
		{
			// Just copy the file
			string tempfile = General.MakeTempFilename(General.Map.TempPath, "wad");
			File.Copy(filename, tempfile);
			return tempfile;
		}

		#endregion
	}
}
