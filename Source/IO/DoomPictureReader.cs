
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

#endregion

namespace CodeImp.DoomBuilder.IO
{
	internal unsafe class DoomPictureReader
	{
		#region ================== Constants

		#endregion

		#region ================== Variables

		// Palette to use
		private Playpal palette;
		
		#endregion

		#region ================== Properties

		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		public DoomPictureReader(Playpal palette)
		{
			// Initialize
			this.palette = palette;
		}

		#endregion

		#region ================== Methods
		
		// This creates a Bitmap from the given data
		public Bitmap ReadAsBitmap(Stream stream)
		{
			// TODO: Read as pixel data and copy pixels
			return null;
		}
		
		// This draws the picture to the given pixel color data
		public void DrawToPixelData(Stream stream, PixelColor* target, int targetwidth, int targetheight, int x, int y)
		{
			// TODO: Read as pixel data and copy pixels
		}
		
		// This creates pixel color data from the given data
		public PixelColor* ReadAsPixelData(Stream stream, out int width, out int height, out int offsetx, out int offsety)
		{
			BinaryReader reader = new BinaryReader(stream);
			PixelColor* pixeldata = null;
			uint datalength = 0;
			int y, count, p;

			// Initialize
			width = 0;
			height = 0;
			offsetx = 0;
			offsety = 0;
			
			// Need at least 4 bytes
			if((stream.Length - stream.Position) < 4) return null;
			
			try
			{
				// Read size and offset
				width = reader.ReadInt16();
				height = reader.ReadInt16();
				offsetx = reader.ReadInt16();
				offsety = reader.ReadInt16();

				// Skip the column addresses
				stream.Seek(4 * width, SeekOrigin.Current);
				
				// Allocate memory
				datalength = (uint)(sizeof(PixelColor) * width * height);
				pixeldata = (PixelColor*)General.VirtualAlloc(IntPtr.Zero, new UIntPtr(datalength), General.MEM_COMMIT, General.PAGE_READWRITE);
				General.ZeroMemory(new IntPtr(pixeldata), (int)datalength);
				
				// Go for all columns
				for(int x = 0; x < width; x++)
				{
					// Read first post start
					y = reader.ReadByte();

					// Continue while not end of column reached
					while(y < 255)
					{
						// Read number of pixels in post
						count = reader.ReadByte();

						// Skip unused pixel
						stream.Seek(1, SeekOrigin.Current);

						// Draw post
						for(int yo = 0; yo < count; yo++)
						{
							// Read pixel color index
							p = reader.ReadByte();

							// Draw pixel
							pixeldata[(y + yo) * width + x] = palette[p];
						}
						
						// Skip unused pixel
						stream.Seek(1, SeekOrigin.Current);

						// Read next post start
						y = reader.ReadByte();
					}
				}

				// Return pointer
				return pixeldata;
			}
			catch(Exception)
			{
				// Free memory if allocated
				if(datalength > 0) General.VirtualFree((void*)pixeldata, new UIntPtr(datalength), General.MEM_RELEASE);
				
				// Return nothing
				return null;
			}
		}
		
		#endregion
	}
}
