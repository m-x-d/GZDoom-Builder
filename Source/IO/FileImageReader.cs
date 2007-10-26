
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
using System.IO;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Geometry;
using System.Drawing;
using CodeImp.DoomBuilder.Data;
using CodeImp.DoomBuilder.Rendering;
using System.Drawing.Imaging;

#endregion

namespace CodeImp.DoomBuilder.IO
{
	public unsafe class FileImageReader : IImageReader
	{
		#region ================== Constructor / Disposer

		// Constructor
		public FileImageReader()
		{
			// We have no destructor
			GC.SuppressFinalize(this);
		}

		#endregion

		#region ================== Methods

		// This reads the image and returns a Bitmap
		public Bitmap ReadAsBitmap(Stream stream)
		{
			return (Bitmap)Bitmap.FromStream(stream);
		}

		// This draws the picture to the given pixel color data
		// Throws exception on failure
		public unsafe void DrawToPixelData(Stream stream, PixelColor* target, int targetwidth, int targetheight, int x, int y)
		{
			Bitmap bmp;
			BitmapData bmpdata;
			PixelColor* pixels;
			int ox, oy, tx, ty;
			int width, height;
			
			// Get bitmap
			bmp = ReadAsBitmap(stream);
			width = bmp.Size.Width;
			height = bmp.Size.Height;

			// Lock bitmap pixels
			bmpdata = bmp.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
			pixels = (PixelColor*)bmpdata.Scan0.ToPointer();

			// Go for all pixels in the original image
			for(ox = 0; ox < width; ox++)
			{
				for(oy = 0; oy < height; oy++)
				{
					// Copy this pixel?
					if(pixels[oy * width + ox].a > 0.5f)
					{
						// Calculate target pixel and copy when within bounds
						tx = x + ox;
						ty = y + oy;
						if((tx >= 0) && (tx < targetwidth) && (ty >= 0) && (ty < targetheight))
							target[ty * targetwidth + tx] = pixels[oy * width + ox];
					}
				}
			}

			// Done
			bmp.UnlockBits(bmpdata);
		}
		
		#endregion
	}
}
