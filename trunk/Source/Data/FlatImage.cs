
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
	internal sealed class FlatImage : ImageData
	{
		#region ================== Constructor / Disposer

		// Constructor
		public FlatImage(string name)
		{
			// Initialize
			SetName(name);
			
			// We have no destructor
			GC.SuppressFinalize(this);
		}

		#endregion

		#region ================== Methods

		// This loads the image
		public override void LoadImage()
		{
			Stream lumpdata;
			MemoryStream mem;
			IImageReader reader;
			byte[] membytes;
			
			// Leave when already loaded
			if(this.IsLoaded) return;

			lock(this)
			{
				// Get the lump data stream
				lumpdata = General.Map.Data.GetPatchData(Name);
				if(lumpdata != null)
				{
					// Copy lump data to memory
					lumpdata.Seek(0, SeekOrigin.Begin);
					membytes = new byte[(int)lumpdata.Length];
					lumpdata.Read(membytes, 0, (int)lumpdata.Length);
					mem = new MemoryStream(membytes);
					mem.Seek(0, SeekOrigin.Begin);

					// Get a reader for the data
					reader = ImageDataFormat.GetImageReader(mem, ImageDataFormat.DOOMFLAT, General.Map.Data.Palette);
					if(reader is UnknownImageReader)
					{
						// Data is in an unknown format!
						General.WriteLogLine("WARNING: Flat lump '" + Name + "' data format could not be read!");
					}

					// Read data as bitmap
					mem.Seek(0, SeekOrigin.Begin);
					bitmap = reader.ReadAsBitmap(mem);
					
					// Get width and height from image
					width = bitmap.Size.Width;
					height = bitmap.Size.Height;
					scaledwidth = (float)width * General.Map.Config.DefaultFlatScale;
					scaledheight = (float)height * General.Map.Config.DefaultFlatScale;
				}
				else
				{
					// Missing a patch lump!
					General.WriteLogLine("WARNING: Missing flat lump '" + Name + "'!");
				}

				// Pass on to base
				base.LoadImage();
			}
		}

		#endregion
	}
}
