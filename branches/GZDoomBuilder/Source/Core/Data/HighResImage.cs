
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
using System.IO;
using CodeImp.DoomBuilder.Controls;
using CodeImp.DoomBuilder.IO;
using CodeImp.DoomBuilder.Rendering;

#endregion

namespace CodeImp.DoomBuilder.Data
{
	internal sealed unsafe class HighResImage : ImageData
	{
		#region ================== Variables

		private readonly List<TexturePatch> patches; //mxd
		
		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		public HighResImage(string name, string virtualpath, int width, int height, float scalex, float scaley, bool worldpanning, bool isflat)
		{
			// Initialize
			this.width = width;
			this.height = height;
			this.scale.x = scalex;
			this.scale.y = scaley;
			this.worldpanning = worldpanning;
			this.patches = new List<TexturePatch>(1);

			//mxd
			SetName(name);
			this.virtualname = "[TEXTURES]" + Path.AltDirectorySeparatorChar + (!string.IsNullOrEmpty(virtualpath) ? virtualpath + Path.AltDirectorySeparatorChar : "") + this.name;
			this.level = virtualname.Split(new[] { Path.AltDirectorySeparatorChar }).Length - 1;
			this.isFlat = isflat;
			
			// We have no destructor
			GC.SuppressFinalize(this);
		}

		#endregion

		#region ================== Methods

		//mxd
		protected override void SetName(string name) 
		{
			if(!General.Map.Config.UseLongTextureNames) 
			{
				if(name.Length > DataManager.CLASIC_IMAGE_NAME_LENGTH)
					name = name.Substring(0, DataManager.CLASIC_IMAGE_NAME_LENGTH);
				name = name.ToUpperInvariant();
			}
			
			base.SetName(name);

			if(General.Settings.CapitalizeTextureNames && !string.IsNullOrEmpty(this.displayname)) 
			{
				this.displayname = this.displayname.ToUpperInvariant();
			}

			if(this.displayname.Length > ImageBrowserItem.MAX_NAME_LENGTH) 
			{
				this.displayname = this.displayname.Substring(0, ImageBrowserItem.MAX_NAME_LENGTH);
			}

			this.shortname = this.displayname.ToUpperInvariant();
			if(this.shortname.Length > DataManager.CLASIC_IMAGE_NAME_LENGTH) 
			{
				this.shortname = this.shortname.Substring(0, DataManager.CLASIC_IMAGE_NAME_LENGTH);
			}
		}

		// This adds a patch to the texture
		public void AddPatch(TexturePatch patch)
		{
			// Add it
			patches.Add(patch);
			if(patch.lumpname == Name) hasPatchWithSameName = true; //mxd
		}
		
		// This loads the image
		protected override void LocalLoadImage()
		{
			// Checks
			if(this.IsImageLoaded || width == 0 || height == 0) return;

			Graphics g = null;
			
			lock(this)
			{
				// Create texture bitmap
				try
				{
					if(bitmap != null) bitmap.Dispose();
					bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb);
					BitmapData bitmapdata = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
					PixelColor* pixels = (PixelColor*)bitmapdata.Scan0.ToPointer();
					General.ZeroMemory(new IntPtr(pixels), width * height * sizeof(PixelColor));
					bitmap.UnlockBits(bitmapdata);
					g = Graphics.FromImage(bitmap);
				}
				catch(Exception e)
				{
					// Unable to make bitmap
					General.ErrorLogger.Add(ErrorType.Error, "Unable to load texture image '" + this.Name + "'. " + e.GetType().Name + ": " + e.Message);
					loadfailed = true;
				}

				int missingpatches = 0; //mxd

				if(!loadfailed)
				{
					// Go for all patches
					foreach(TexturePatch p in patches)
					{
						// Get the patch data stream
						Stream patchdata = General.Map.Data.GetPatchData(p.lumpname, p.haslongname);

						if(patchdata != null)
						{
							// Copy patch data to memory
							patchdata.Seek(0, SeekOrigin.Begin);
							byte[] membytes = new byte[(int)patchdata.Length];
							patchdata.Read(membytes, 0, (int)patchdata.Length);
							MemoryStream mem = new MemoryStream(membytes);
							mem.Seek(0, SeekOrigin.Begin);

							// Get a reader for the data
							IImageReader reader = ImageDataFormat.GetImageReader(mem, ImageDataFormat.DOOMPICTURE, General.Map.Data.Palette);
							if(reader is UnknownImageReader)
							{
								//mxd. Probably that's a flat?..
								if(General.Map.Config.MixTexturesFlats) 
								{
									reader = ImageDataFormat.GetImageReader(mem, ImageDataFormat.DOOMFLAT, General.Map.Data.Palette);
								}

								if(reader is UnknownImageReader) 
								{
									// Data is in an unknown format!
									General.ErrorLogger.Add(ErrorType.Error, "Patch lump '" + p.lumpname + "' data format could not be read, while loading texture '" + this.Name + "'");
									missingpatches++; //mxd
								}
							}

							if(!(reader is UnknownImageReader)) 
							{
								// Get the patch
								mem.Seek(0, SeekOrigin.Begin);
								Bitmap patchbmp = null;
								try { patchbmp = reader.ReadAsBitmap(mem); }
								catch(InvalidDataException)
								{
									// Data cannot be read!
									General.ErrorLogger.Add(ErrorType.Error, "Patch lump '" + p.lumpname + "' data format could not be read, while loading texture '" + this.Name + "'");
									missingpatches++; //mxd
								}

								if(patchbmp != null)
								{
									//mxd. Apply transformations from TexturePatch 
									patchbmp = TransformPatch(p, patchbmp);

									// Draw the patch on the texture image
									Rectangle tgtrect = new Rectangle(p.x, p.y, patchbmp.Size.Width, patchbmp.Size.Height);
									g.DrawImageUnscaledAndClipped(patchbmp, tgtrect);
									patchbmp.Dispose();
								}
							}

							// Done
							mem.Dispose();
						}
						else
						{
							//mxd. ZDoom can use any known graphic as patch
							if(General.Map.Config.MixTexturesFlats)
							{
								ImageData img = General.Map.Data.GetTextureImage(p.lumpname);
								if(!(img is UnknownImage) && img != this)
								{
									if(!img.IsImageLoaded) img.LoadImage();

									//mxd. Apply transformations from TexturePatch. We don't want to modify the original bitmap here, so make a copy
									Bitmap patchbmp = TransformPatch(p, new Bitmap(img.GetBitmap()));

									// Draw the patch on the texture image
									Rectangle tgtrect = new Rectangle(p.x, p.y, patchbmp.Size.Width, patchbmp.Size.Height);
									g.DrawImageUnscaledAndClipped(patchbmp, tgtrect);
									patchbmp.Dispose();

									continue;
								}
							}
							
							// Missing a patch lump!
							General.ErrorLogger.Add(ErrorType.Error, "Missing patch lump '" + p.lumpname + "' while loading texture '" + this.Name + "'");
							missingpatches++; //mxd
						}
					}
				}
				
				// Dispose bitmap if load failed
				if((bitmap != null) && (loadfailed || missingpatches >= patches.Count)) //mxd. We can still display texture if at least one of the patches was loaded
				{
					bitmap.Dispose();
					bitmap = null;
					loadfailed = true;
				}

				// Pass on to base
				base.LocalLoadImage();
			}
		}

		//mxd
		private Bitmap TransformPatch(TexturePatch p, Bitmap patchbmp)
		{
			//mxd. Flip
			if(p.flipx || p.flipy)
			{
				RotateFlipType flip;
				if(p.flipx && !p.flipy)
					flip = RotateFlipType.RotateNoneFlipX;
				else if(!p.flipx && p.flipy)
					flip = RotateFlipType.RotateNoneFlipY;
				else
					flip = RotateFlipType.RotateNoneFlipXY;
				patchbmp.RotateFlip(flip);
			}

			//mxd. Then rotate. I do it this way because RotateFlip function rotates THEN flips, and GZDoom does it the other way around.
			if(p.rotate != 0)
			{
				RotateFlipType rotate;
				switch(p.rotate)
				{
					case 90:
						rotate = RotateFlipType.Rotate90FlipNone;
						break;
					case 180:
						rotate = RotateFlipType.Rotate180FlipNone;
						break;
					default:
						rotate = RotateFlipType.Rotate270FlipNone;
						break;
				}
				patchbmp.RotateFlip(rotate);
			}

			// Adjust patch alpha, apply tint or blend
			if(p.blendstyle != TexturePathBlendStyle.None || p.style != TexturePathRenderStyle.Copy)
			{
				BitmapData bmpdata = null;

				try
				{
					bmpdata = patchbmp.LockBits(new Rectangle(0, 0, patchbmp.Size.Width, patchbmp.Size.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
				}
				catch(Exception e)
				{
					General.ErrorLogger.Add(ErrorType.Error, "Cannot lock image '" + p.lumpname + "' for alpha adjustment. " + e.GetType().Name + ": " + e.Message);
				}

				if(bmpdata != null)
				{
					PixelColor* pixels = (PixelColor*)(bmpdata.Scan0.ToPointer());
					int numpixels = bmpdata.Width * bmpdata.Height;
					int patchalpha = (int)Math.Round(General.Clamp(p.alpha, 0f, 1f) * 255); //convert alpha to [0-255] range

					//mxd. Blend/Tint support
					if(p.blendstyle == TexturePathBlendStyle.Blend)
					{
						for(PixelColor* cp = pixels + numpixels - 1; cp >= pixels; cp--)
						{
							cp->r = (byte)((cp->r * p.blend.r) * PixelColor.BYTE_TO_FLOAT);
							cp->g = (byte)((cp->g * p.blend.g) * PixelColor.BYTE_TO_FLOAT);
							cp->b = (byte)((cp->b * p.blend.b) * PixelColor.BYTE_TO_FLOAT);
						}
					}
					else if(p.blendstyle == TexturePathBlendStyle.Tint)
					{
						float tintammount = p.tintammount - 0.1f;

						if(tintammount > 0)
						{
							float br = p.blend.r * PixelColor.BYTE_TO_FLOAT * tintammount;
							float bg = p.blend.g * PixelColor.BYTE_TO_FLOAT * tintammount;
							float bb = p.blend.b * PixelColor.BYTE_TO_FLOAT * tintammount;
							float invTint = 1.0f - tintammount;

							for(PixelColor* cp = pixels + numpixels - 1; cp >= pixels; cp--)
							{
								cp->r = (byte)(((cp->r * PixelColor.BYTE_TO_FLOAT) * invTint + br) * 255.0f);
								cp->g = (byte)(((cp->g * PixelColor.BYTE_TO_FLOAT) * invTint + bg) * 255.0f);
								cp->b = (byte)(((cp->b * PixelColor.BYTE_TO_FLOAT) * invTint + bb) * 255.0f);
							}
						}
					}

					//mxd. Apply RenderStyle
					if(p.style == TexturePathRenderStyle.Blend)
					{
						for(PixelColor* cp = pixels + numpixels - 1; cp >= pixels; cp--)
							cp->a = (byte)((cp->a * patchalpha) * PixelColor.BYTE_TO_FLOAT);
					}
					//mxd. We need a copy of underlying part of texture for these styles
					else if(p.style != TexturePathRenderStyle.Copy)
					{
						// Copy portion of texture
						int lockWidth = (p.x + patchbmp.Size.Width > bitmap.Width) ? bitmap.Width - p.x : patchbmp.Size.Width;
						int lockHeight = (p.y + patchbmp.Size.Height > bitmap.Height) ? bitmap.Height - p.y : patchbmp.Size.Height;

						Bitmap source = new Bitmap(patchbmp.Size.Width, patchbmp.Size.Height);
						using(Graphics sg = Graphics.FromImage(source))
							sg.DrawImageUnscaled(bitmap, new Rectangle(-p.x, -p.y, lockWidth, lockHeight));

						// Lock texture
						BitmapData texturebmpdata = null;

						try
						{
							texturebmpdata = source.LockBits(new Rectangle(0, 0, source.Width, source.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
						}
						catch(Exception e)
						{
							General.ErrorLogger.Add(ErrorType.Error, "Cannot lock texture '" + this.Name + "' to apply render style. " + e.GetType().Name + ": " + e.Message);
						}

						if(texturebmpdata != null)
						{
							PixelColor* texturepixels = (PixelColor*)(texturebmpdata.Scan0.ToPointer());
							PixelColor* tcp = texturepixels + numpixels - 1;

							switch(p.style)
							{
								case TexturePathRenderStyle.Add:
									for(PixelColor* cp = pixels + numpixels - 1; cp >= pixels; cp--)
									{
										cp->r = (byte)Math.Min(255, cp->r + tcp->r);
										cp->g = (byte)Math.Min(255, cp->g + tcp->g);
										cp->b = (byte)Math.Min(255, cp->b + tcp->b);
										cp->a = (byte)((cp->a * patchalpha) * PixelColor.BYTE_TO_FLOAT);
										tcp--;
									}
									break;

								case TexturePathRenderStyle.Subtract:
									for(PixelColor* cp = pixels + numpixels - 1; cp >= pixels; cp--)
									{
										cp->r = (byte)Math.Max(0, tcp->r - cp->r);
										cp->g = (byte)Math.Max(0, tcp->g - cp->g);
										cp->b = (byte)Math.Max(0, tcp->b - cp->b);
										cp->a = (byte)((cp->a * patchalpha) * PixelColor.BYTE_TO_FLOAT);
										tcp--;
									}
									break;

								case TexturePathRenderStyle.ReverseSubtract:
									for(PixelColor* cp = pixels + numpixels - 1; cp >= pixels; cp--)
									{
										cp->r = (byte)Math.Max(0, cp->r - tcp->r);
										cp->g = (byte)Math.Max(0, cp->g - tcp->g);
										cp->b = (byte)Math.Max(0, cp->b - tcp->b);
										cp->a = (byte)((cp->a * patchalpha) * PixelColor.BYTE_TO_FLOAT);
										tcp--;
									}
									break;

								case TexturePathRenderStyle.Modulate:
									for(PixelColor* cp = pixels + numpixels - 1; cp >= pixels; cp--)
									{
										cp->r = (byte)((cp->r * tcp->r) * PixelColor.BYTE_TO_FLOAT);
										cp->g = (byte)((cp->g * tcp->g) * PixelColor.BYTE_TO_FLOAT);
										cp->b = (byte)((cp->b * tcp->b) * PixelColor.BYTE_TO_FLOAT);
										tcp--;
									}
									break;

							}

							source.UnlockBits(texturebmpdata);
						}
					}

					patchbmp.UnlockBits(bmpdata);
				}
			}

			return patchbmp;
		}

		#endregion
	}
}
