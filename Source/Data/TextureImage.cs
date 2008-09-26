
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
	internal sealed unsafe class TextureImage : ImageData
	{
		#region ================== Variables

		private List<TexturePatch> patches;
		private float scalex;
		private float scaley;
		
		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		public TextureImage(string name, int width, int height, float scalex, float scaley)
		{
			// Initialize
			this.width = width;
			this.height = height;
			this.scalex = scalex;
			this.scaley = scaley;
			this.scaledwidth = (float)width * scalex;
			this.scaledheight = (float)height * scaley;
			this.patches = new List<TexturePatch>();
			SetName(name);
			
			// We have no destructor
			GC.SuppressFinalize(this);
		}

		#endregion

		#region ================== Methods

		// This adds a patch to the texture
		public void AddPatch(TexturePatch patch)
		{
			// Add it
			patches.Add(patch);
		}
		
		// This loads the image
		public override void LoadImage()
		{
			uint datalength = (uint)(width * height * sizeof(PixelColor));
			IImageReader reader;
			BitmapData bitmapdata;
			MemoryStream mem;
			PixelColor* pixels;
			Stream patchdata;
			byte[] membytes;
			bool failed = false;
			
			// Checks
			if(this.IsLoaded) return;
			if((width == 0) || (height == 0)) return;
			
			lock(this)
			{
				// Create texture bitmap
				try
				{
					if(bitmap != null) bitmap.Dispose();
					bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb);
					bitmapdata = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
					pixels = (PixelColor*)bitmapdata.Scan0.ToPointer();
					General.ZeroMemory(new IntPtr(pixels), width * height * sizeof(PixelColor));
				}
				catch(Exception e)
				{
					// Unable to make bitmap
					General.WriteLogLine("ERROR: Unable to load texture image '" + this.Name + "'. " + e.GetType().Name + ": " + e.Message);
					return;
				}
				
				// Go for all patches
				foreach(TexturePatch p in patches)
				{
					// Get the patch data stream
					patchdata = General.Map.Data.GetPatchData(p.lumpname);
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
							General.WriteLogLine("WARNING: Patch lump '" + p.lumpname + "' data format could not be read, while loading texture '" + this.Name + "'!");
							failed = true;
						}
						else
						{
							// Draw the patch
							mem.Seek(0, SeekOrigin.Begin);
							reader.DrawToPixelData(mem, pixels, width, height, p.x, p.y);
						}
					}
					else
					{
						// Missing a patch lump!
						General.WriteLogLine("WARNING: Missing patch lump '" + p.lumpname + "' while loading texture '" + this.Name + "'!");
					}
				}

				// Done
				bitmap.UnlockBits(bitmapdata);

				// When failed, use the error picture
				if(failed)
				{
					bitmap.Dispose();
					bitmap = UnknownImageReader.ReadAsBitmap();
				}

				// Pass on to base
				base.LoadImage();
			}
		}

		#endregion
	}
}
