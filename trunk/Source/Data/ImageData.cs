
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
using SlimDX.Direct3D9;
using System.Drawing.Imaging;
using CodeImp.DoomBuilder.Rendering;
using CodeImp.DoomBuilder.IO;
using System.IO;
using System.Windows.Forms;

#endregion

namespace CodeImp.DoomBuilder.Data
{
	public abstract unsafe class ImageData
	{
		#region ================== Constants
		
		#endregion
		
		#region ================== Variables
		
		// Properties
		private string name;
		private long longname;
		protected int width;
		protected int height;
		protected float scaledwidth;
		protected float scaledheight;
		protected bool usecolorcorrection;
		
		// Loading
		private volatile ImageLoadState previewstate;
		private volatile ImageLoadState imagestate;
		private volatile int previewindex;
		protected volatile bool loadfailed;
		
		// References
		private volatile bool usedinmap;
		private volatile int references;
		
		// GDI bitmap
		protected Bitmap bitmap;
		
		// Direct3D texture
		private int mipmaplevels = 0;	// 0 = all mipmaps
		private Texture texture;
		
		// Disposing
		protected bool isdisposed = false;
		
		#endregion
		
		#region ================== Properties
		
		public string Name { get { return name; } }
		public long LongName { get { return longname; } }
		public bool UseColorCorrection { get { return usecolorcorrection; } set { usecolorcorrection = value; } }
		//public Bitmap Bitmap { get { lock(this) { if(bitmap != null) return new Bitmap(bitmap); else return CodeImp.DoomBuilder.Properties.Resources.Hourglass; } } }
		public Bitmap Bitmap { get { lock(this) { if(bitmap != null) return bitmap; else return CodeImp.DoomBuilder.Properties.Resources.Hourglass; } } }
		public Texture Texture { get { lock(this) { return texture; } } }
		public bool IsPreviewLoaded { get { return (previewstate == ImageLoadState.Ready); } }
		public bool IsImageLoaded { get { return (imagestate == ImageLoadState.Ready); } }
		public bool LoadFailed { get { return loadfailed; } }
		public bool IsDisposed { get { return isdisposed; } }
		public ImageLoadState ImageState { get { return imagestate; } internal set { imagestate = value; } }
		public ImageLoadState PreviewState { get { return previewstate; } internal set { previewstate = value; } }
		public bool IsReferenced { get { return (references > 0) || usedinmap; } }
		public bool UsedInMap { get { return usedinmap; } }
		public int MipMapLevels { get { return mipmaplevels; } set { mipmaplevels = value; } }
		public int Width { get { return width; } }
		public int Height { get { return height; } }
		internal int PreviewIndex { get { return previewindex; } set { previewindex = value; } }
		public float ScaledWidth { get { return scaledwidth; } }
		public float ScaledHeight { get { return scaledheight; } }
		
		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		public ImageData()
		{
			// We have no destructor
			GC.SuppressFinalize(this);

			// Defaults
			usecolorcorrection = true;
		}

		// Disposer
		public virtual void Dispose()
		{
			// Not already disposed?
			if(!isdisposed)
			{
				lock(this)
				{
					// Clean up
					if(bitmap != null) bitmap.Dispose();
					if(texture != null) texture.Dispose();
					bitmap = null;
					texture = null;
					
					// Done
					usedinmap = false;
					references = 0;
					imagestate = ImageLoadState.None;
					previewstate = ImageLoadState.None;
					isdisposed = true;
				}
			}
		}
		
		#endregion
		
		#region ================== Management
		
		// This sets the status of the texture usage in the map
		internal void SetUsedInMap(bool used)
		{
			if(used != usedinmap)
			{
				usedinmap = used;
				General.Map.Data.ProcessImage(this);
			}
		}
		
		// This adds a reference
		public void AddReference()
		{
			references++;
			if(references == 1) General.Map.Data.ProcessImage(this);
		}
		
		// This removes a reference
		public void RemoveReference()
		{
			references--;
			if(references < 0) General.Fail("FAIL! (references < 0)", "Somewhere this image is dereferenced more than it was referenced.");
			if(references == 0) General.Map.Data.ProcessImage(this);
		}
		
		// This sets the name
		protected void SetName(string name)
		{
			this.name = name;
			this.longname = Lump.MakeLongName(name);
		}
		
		// This unloads the image
		public virtual void UnloadImage()
		{
			lock(this)
			{
				if(bitmap != null) bitmap.Dispose();
				bitmap = null;
				imagestate = ImageLoadState.None;
			}
		}
		
		// This requests loading the image
		public virtual void LoadImage()
		{
			BitmapData bmpdata = null;
			
			lock(this)
			{
				// Bitmap loaded successfully?
				if(bitmap != null)
				{
					// This applies brightness correction on the image
					if(usecolorcorrection)
					{
						try
						{
							// Try locking the bitmap
							bmpdata = bitmap.LockBits(new Rectangle(0, 0, bitmap.Size.Width, bitmap.Size.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
						}
						catch(Exception e)
						{
							General.WriteLogLine("ERROR: Cannot lock image '" + name + "' for color correction. " + e.GetType().Name + ": " + e.Message);
						}

						// Bitmap locked?
						if(bmpdata != null)
						{
							// Apply color correction
							PixelColor* pixels = (PixelColor*)(bmpdata.Scan0.ToPointer());
							General.Colors.ApplColorCorrection(pixels, bmpdata.Width * bmpdata.Height);
							bitmap.UnlockBits(bmpdata);
						}
					}
				}
				else
				{
					// Loading failed
					// We still mark the image as ready so that it will
					// not try loading again until Reload Resources is used
					loadfailed = true;
					bitmap = new Bitmap(Properties.Resources.Failed);
					width = bitmap.Size.Width;
					height = bitmap.Size.Height;
				}
				
				// Image is ready
				imagestate = ImageLoadState.Ready;
			}
		}
		
		// This creates the Direct3D texture
		internal virtual void CreateTexture()
		{
			MemoryStream memstream;
			
			lock(this)
			{
				// Only do this when texture is not created yet
				if(((texture == null) || (texture.Disposed)) && this.IsImageLoaded)
				{
					Image img = bitmap;
					if(loadfailed) img = Properties.Resources.Failed;
					
					// Write to memory stream and read from memory
					memstream = new MemoryStream();
					img.Save(memstream, ImageFormat.Bmp);
					memstream.Seek(0, SeekOrigin.Begin);
					texture = Texture.FromStream(General.Map.Graphics.Device, memstream, (int)memstream.Length,
									img.Size.Width, img.Size.Height, mipmaplevels, Usage.None, Format.Unknown,
									Pool.Managed, Filter.Linear, Filter.Linear, 0);
					memstream.Dispose();
				}
			}
		}
		
		// This destroys the Direct3D texture
		internal void ReleaseTexture()
		{
			lock(this)
			{
				// Trash it
				if(texture != null) texture.Dispose();
				texture = null;
			}
		}
		
		// This draws a preview
		public virtual void DrawPreviewCentered(Graphics target, Rectangle targetview)
		{
			lock(this)
			{
				// Preview ready?
				if(previewstate == ImageLoadState.Ready)
				{
					// Draw preview
					General.Map.Data.Previews.DrawPreviewCentered(previewindex, target, targetview);
				}
				// Loading failed?
				else if(loadfailed)
				{
					// Draw error bitmap
					RectangleF targetpos = General.MakeZoomedRect(Properties.Resources.Failed.Size, targetview);
					target.DrawImage(Properties.Resources.Hourglass, targetpos.Location);
				}
				else
				{
					// Draw loading bitmap
					RectangleF targetpos = General.MakeZoomedRect(Properties.Resources.Hourglass.Size, targetview);
					target.DrawImage(Properties.Resources.Hourglass, targetpos.Location);
				}
			}
		}

		// This draws a preview
		public virtual void DrawPreview(Graphics target, Point targetpos)
		{
			lock(this)
			{
				// Preview ready?
				if(previewstate == ImageLoadState.Ready)
				{
					// Draw preview
					General.Map.Data.Previews.DrawPreview(previewindex, target, targetpos);
				}
				// Loading failed?
				else if(loadfailed)
				{
					// Draw error bitmap
					target.DrawImageUnscaled(Properties.Resources.Failed, targetpos);
				}
				else
				{
					// Draw loading bitmap
					target.DrawImageUnscaled(Properties.Resources.Hourglass, targetpos);
				}
			}
		}
		
		// This returns a preview image
		public virtual Image GetPreview()
		{
			lock(this)
			{
				// Preview ready?
				if(previewstate == ImageLoadState.Ready)
				{
					// Make a bitmap and return it
					Bitmap bmp = new Bitmap(PreviewManager.IMAGE_WIDTH, PreviewManager.IMAGE_HEIGHT);
					Graphics g = Graphics.FromImage(bmp);
					g.Clear(Color.Transparent);
					General.Map.Data.Previews.DrawPreview(previewindex, g, new Point(0, 0));
					g.Dispose();
					return bmp;
				}
				// Loading failed?
				else if(loadfailed)
				{
					// Return error bitmap
					return Properties.Resources.Failed;
				}
				else
				{
					// Return loading bitmap
					return Properties.Resources.Hourglass;
				}
			}
		}
		
		#endregion
	}
}
