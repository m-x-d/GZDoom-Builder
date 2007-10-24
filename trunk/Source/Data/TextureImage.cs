
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
	public sealed unsafe class TextureImage : ImageData
	{
		#region ================== Constants

		#endregion

		#region ================== Variables

		private List<TexturePatch> patches;
		private float scalex;
		private float scaley;
		
		#endregion

		#region ================== Properties

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

		// Diposer
		public override void Dispose()
		{
			// Not already disposed?
			if(!isdisposed)
			{
				// Clean up

				// Done
				base.Dispose();
			}
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
			DoomPictureReader reader = new DoomPictureReader(General.Map.Data.Palette);
			BitmapData bitmapdata;
			MemoryStream mem;
			PixelColor* pixels;
			Stream patchdata;
			byte[] membytes;
			
			// Leave when already loaded
			if(this.IsLoaded) return;
			
			// Create texture bitmap
			bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb);
			bitmapdata = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
			pixels = (PixelColor*)bitmapdata.Scan0.ToPointer();
			General.ZeroMemory(new IntPtr(pixels), width * height * sizeof(PixelColor));

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

					// Draw the patch
					reader.DrawToPixelData(mem, pixels, width, height, p.x, p.y);
				}
			}
			
			// Done
			bitmap.UnlockBits(bitmapdata);
			
			// Pass on to base
			base.LoadImage();
		}

		#endregion
	}
}
