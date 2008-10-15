
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
using CodeImp.DoomBuilder.Rendering;
using CodeImp.DoomBuilder.IO;
using System.IO;

#endregion

namespace CodeImp.DoomBuilder.Data
{
	internal class SimpleTextureImage : ImageData
	{
		#region ================== Constants

		#endregion

		#region ================== Variables

		private string lumpname;
		private float scalex;
		private float scaley;

		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		public SimpleTextureImage(string name, string lumpname, float scalex, float scaley)
		{
			// Initialize
			this.scalex = scalex;
			this.scaley = scaley;
			this.scaledwidth = (float)width * scalex;
			this.scaledheight = (float)height * scaley;
			this.lumpname = lumpname;
			SetName(name);
			
			// We have no destructor
			GC.SuppressFinalize(this);
		}

		#endregion

		#region ================== Methods

		// This loads the image
		protected override void LocalLoadImage()
		{
			IImageReader reader;
			MemoryStream mem;
			Stream patchdata;
			byte[] membytes;

			// Checks
			if(this.IsImageLoaded) return;

			lock(this)
			{
				// Get the patch data stream
				patchdata = General.Map.Data.GetPatchData(lumpname);
				if(patchdata != null)
				{
					// Copy patch data to memory
					patchdata.Seek(0, SeekOrigin.Begin);
					membytes = new byte[(int)patchdata.Length];
					patchdata.Read(membytes, 0, (int)patchdata.Length);
					mem = new MemoryStream(membytes);
					mem.Seek(0, SeekOrigin.Begin);

					// Get a reader for the data
					reader = ImageDataFormat.GetImageReader(mem, ImageDataFormat.DOOMPICTURE, General.Map.Data.Palette);
					if(reader is UnknownImageReader)
					{
						// Data is in an unknown format!
						General.WriteLogLine("WARNING: Image lump '" + lumpname + "' data format could not be read, while loading texture '" + this.Name + "'!");
					}
					
					// Load the image
					mem.Seek(0, SeekOrigin.Begin);
					bitmap = reader.ReadAsBitmap(mem);
					if(bitmap == null) return;

					// Done
					mem.Dispose();
				}

				// Pass on to base
				base.LocalLoadImage();
			}
		}
		
		#endregion
	}
}
