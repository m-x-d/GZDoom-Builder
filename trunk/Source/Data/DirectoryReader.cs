
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

		#endregion

		#region ================== Variables

		// Source
		private bool readtextures;
		private bool readflats;

		#endregion

		#region ================== Properties

		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		public DirectoryReader(DataLocation dl) : base(dl)
		{
			// Initialize
			this.readtextures = dl.textures;
			this.readflats = dl.flats;

			General.WriteLogLine("Opening directory resource '" + location.location + "'");

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

				// Clean up
				
				// Done
				base.Dispose();
			}
		}

		#endregion

		#region ================== Palette

		// This loads the PLAYPAL palette
		public override Playpal LoadPalette()
		{
			// Error when suspended
			if(issuspended) throw new Exception("Data reader is suspended");
			
			// Not yet implemented
			return null;
		}

		#endregion
		
		#region ================== Textures

		// This loads the textures
		public override ICollection<ImageData> LoadTextures(PatchNames pnames)
		{
			// Error when suspended
			if(issuspended) throw new Exception("Data reader is suspended");

			// Should we load the images in this directory as textures?
			if(readtextures) return LoadDirectoryImages(); else return null;
		}

		#endregion

		#region ================== Flats
		
		// This loads the textures
		public override ICollection<ImageData> LoadFlats()
		{
			// Error when suspended
			if(issuspended) throw new Exception("Data reader is suspended");

			// Should we load the images in this directory as flats?
			if(readflats) return LoadDirectoryImages(); else return null;
		}

		#endregion
		
		#region ================== Methods
		
		// This loads the images in this directory
		private ICollection<ImageData> LoadDirectoryImages()
		{
			List<ImageData> images = new List<ImageData>();
			string[] files;
			string name;

			// Find all BMP files
			files = Directory.GetFiles(location.location, "*.bmp", SearchOption.TopDirectoryOnly);

			// Find all GIF files and append to files array
			AddToArray(ref files, Directory.GetFiles(location.location, "*.gif", SearchOption.TopDirectoryOnly));

			// Find all PNG files and append to files array
			AddToArray(ref files, Directory.GetFiles(location.location, "*.png", SearchOption.TopDirectoryOnly));
			
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

		#endregion
	}
}
