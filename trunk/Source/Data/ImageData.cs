
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
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Text;
using System.Drawing;
using SlimDX.Direct3D9;
using System.Drawing.Imaging;
using CodeImp.DoomBuilder.Rendering;
using CodeImp.DoomBuilder.IO;
using System.IO;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using CodeImp.DoomBuilder.Windows;

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
			// Defaults
			usecolorcorrection = true;
		}

		// Destructor
		~ImageData()
		{
			this.Dispose();
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
			if(references < 0) General.Fail("FAIL! (references < 0) Somewhere this image is dereferenced more than it was referenced.");
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

		// This returns the bitmap image
		public Bitmap GetBitmap()
		{
			lock(this)
			{
				// Image loaded successfully?
				if(!loadfailed && (imagestate == ImageLoadState.Ready) && (bitmap != null))
				{
					return bitmap;
				}
				// Image loading failed?
				else if(loadfailed)
				{
					return Properties.Resources.Failed;
				}
				else
				{
					return Properties.Resources.Hourglass;
				}
			}
		}
		
		// This loads the image
		public void LoadImage()
		{
			// Keep original dimensions
			int oldwidth = width;
			int oldheight = height;
			float oldscaledwidth = scaledwidth;
			float oldscaledheight = scaledheight;
			
			// Do the loading
			LocalLoadImage();

			// Anything changed?
			//if((oldwidth != width) || (oldheight != height) ||
			//   (oldscaledwidth != scaledwidth) || (oldscaledheight != scaledheight))
			{
				// Notify the main thread about the change so that sectors can update their buffers
				IntPtr strptr = Marshal.StringToCoTaskMemAuto(this.name);
				General.SendMessage(General.MainWindow.Handle, (int)MainForm.ThreadMessages.ImageDataLoaded, strptr.ToInt32(), 0);
			}
		}
		
		// This requests loading the image
		protected virtual void LocalLoadImage()
		{
			BitmapData bmpdata = null;
			
			lock(this)
			{
				// Bitmap loaded successfully?
				if(bitmap != null)
				{
					// Bitmap has incorrect format?
					if(bitmap.PixelFormat != PixelFormat.Format32bppArgb)
					{
						General.WriteLogLine("WARNING: Image '" + name + "' does not have A8R8G8B8 pixel format. Conversion was needed!");
						Bitmap oldbitmap = bitmap;
						try
						{
							// Convert to desired pixel format
							bitmap = new Bitmap(oldbitmap.Size.Width, oldbitmap.Size.Height, PixelFormat.Format32bppArgb);
							Graphics g = Graphics.FromImage(bitmap);
							g.PageUnit = GraphicsUnit.Pixel;
							g.CompositingQuality = CompositingQuality.HighQuality;
							g.InterpolationMode = InterpolationMode.HighQualityBicubic;
							g.SmoothingMode = SmoothingMode.HighQuality;
							g.PixelOffsetMode = PixelOffsetMode.None;
							g.Clear(Color.Transparent);
							g.DrawImageUnscaled(oldbitmap, new Point(0, 0));
							g.Dispose();
							oldbitmap.Dispose();
						}
						catch(Exception e)
						{
							bitmap = oldbitmap;
							General.WriteLogLine("WARNING: Cannot lock image '" + name + "' for pixel format conversion. " + e.GetType().Name + ": " + e.Message);
						}
					}
					
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
							General.WriteLogLine("WARNING: Cannot lock image '" + name + "' for color correction. " + e.GetType().Name + ": " + e.Message);
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
				}

				if(bitmap != null)
				{
					width = bitmap.Size.Width;
					height = bitmap.Size.Height;
					if((General.Map != null) && (General.Map.Config != null))
					{
						scaledwidth = (float)bitmap.Size.Width * General.Map.Config.DefaultTextureScale;
						scaledheight = (float)bitmap.Size.Height * General.Map.Config.DefaultTextureScale;
					}
					else
					{
						scaledwidth = (float)bitmap.Size.Width;
						scaledheight = (float)bitmap.Size.Height;
					}
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
				if(((texture == null) || (texture.Disposed)) && this.IsImageLoaded && !loadfailed)
				{
					Image img = bitmap;
					if(loadfailed) img = Properties.Resources.Failed;
					
					// Write to memory stream and read from memory
					memstream = new MemoryStream((img.Size.Width * img.Size.Height * 4) + 4096);
					img.Save(memstream, ImageFormat.Bmp);
					memstream.Seek(0, SeekOrigin.Begin);
					texture = Texture.FromStream(General.Map.Graphics.Device, memstream, (int)memstream.Length,
									img.Size.Width, img.Size.Height, mipmaplevels, Usage.None, Format.Unknown,
									Pool.Managed, General.Map.Graphics.PostFilter, General.Map.Graphics.MipGenerateFilter, 0);
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
		public virtual void DrawPreview(Graphics target, Point targetpos)
		{
			lock(this)
			{
				// Preview ready?
				if(!loadfailed && (previewstate == ImageLoadState.Ready))
				{
					// Draw preview
					General.Map.Data.Previews.DrawPreview(previewindex, target, targetpos);
				}
				// Loading failed?
				else if(loadfailed)
				{
					// Draw error bitmap
					targetpos = new Point(targetpos.X + ((General.Map.Data.Previews.MaxImageWidth - Properties.Resources.Hourglass.Width) >> 1),
										  targetpos.Y + ((General.Map.Data.Previews.MaxImageHeight - Properties.Resources.Hourglass.Height) >> 1));
					target.DrawImageUnscaled(Properties.Resources.Failed, targetpos);
				}
				else
				{
					// Draw loading bitmap
					targetpos = new Point(targetpos.X + ((General.Map.Data.Previews.MaxImageWidth - Properties.Resources.Hourglass.Width) >> 1),
										  targetpos.Y + ((General.Map.Data.Previews.MaxImageHeight - Properties.Resources.Hourglass.Height) >> 1));
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
					// Make a copy
					return General.Map.Data.Previews.GetPreviewCopy(previewindex);
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
