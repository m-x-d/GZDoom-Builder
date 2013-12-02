
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
		
		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		public TextureImage(string name, int width, int height, float scalex, float scaley)
		{
			// Initialize
			this.width = width;
			this.height = height;
			this.scale.x = scalex;
			this.scale.y = scaley;
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
		protected override void LocalLoadImage()
		{
			IImageReader reader;
			BitmapData bitmapdata = null;
			MemoryStream mem;
			PixelColor* pixels = (PixelColor*)0;
			Stream patchdata;
			byte[] membytes;
			
			// Checks
			if(this.IsImageLoaded) return;
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
					General.ErrorLogger.Add(ErrorType.Error, "Unable to load texture image '" + this.Name + "'. " + e.GetType().Name + ": " + e.Message);
					loadfailed = true;
				}

				int failCount = 0; //mxd

				if(!loadfailed)
				{
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
								//mxd. Probably that's a flat?..
								if (General.Map.Config.MixTexturesFlats) {
									reader = ImageDataFormat.GetImageReader(mem, ImageDataFormat.DOOMFLAT, General.Map.Data.Palette);
								}
								if (reader is UnknownImageReader) {
									// Data is in an unknown format!
									General.ErrorLogger.Add(ErrorType.Error, "Patch lump '" + p.lumpname + "' data format could not be read, while loading texture '" + this.Name + "'. Does this lump contain valid picture data at all?");
									loadfailed = true;
									failCount++; //mxd
								}
							}

							if(!(reader is UnknownImageReader)) {
								// Draw the patch
								mem.Seek(0, SeekOrigin.Begin);
								try { reader.DrawToPixelData(mem, pixels, width, height, p.x, p.y); }
								catch(InvalidDataException)
								{
									// Data cannot be read!
									General.ErrorLogger.Add(ErrorType.Error, "Patch lump '" + p.lumpname + "' data format could not be read, while loading texture '" + this.Name + "'. Does this lump contain valid picture data at all?");
									loadfailed = true;
									failCount++; //mxd
								}
							}

							// Done
							mem.Dispose();
						}
						else
						{
							// Missing a patch lump!
							General.ErrorLogger.Add(ErrorType.Error, "Missing patch lump '" + p.lumpname + "' while loading texture '" + this.Name + "'. Did you forget to include required resources?");
							loadfailed = true;
							failCount++; //mxd
						}
					}

					// Done
					bitmap.UnlockBits(bitmapdata);
				}
				
				// Dispose bitmap if load failed
				if((bitmap != null) && (loadfailed || failCount >= patches.Count)) //mxd. We can still display texture if at least one of the patches was loaded
				{
					bitmap.Dispose();
					bitmap = null;
					loadfailed = true;
				}

				// Pass on to base
				base.LocalLoadImage();
			}
		}

		#endregion
	}
}
