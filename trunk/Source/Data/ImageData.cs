
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
	public abstract unsafe class ImageData : IDisposable
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
		
		// GDI bitmap
		protected Bitmap bitmap;

		// 2D rendering data
		private PixelColorBlock pixeldata;
		
		// Direct3D texture
		private Texture texture;

		// Disposing
		protected bool isdisposed = false;

		#endregion

		#region ================== Properties

		public string Name { get { return name; } }
		public long LongName { get { return longname; } }
		public PixelColorBlock PixelData { get { lock(this) { return pixeldata; } } }
		public Bitmap Bitmap { get { lock(this) { if(bitmap != null) return bitmap; else return CodeImp.DoomBuilder.Properties.Resources.Hourglass; } } }
		public Texture Texture { get { lock(this) { return texture; } } }
		public bool IsLoaded { get { return (bitmap != null); } }
		public bool IsDisposed { get { return isdisposed; } }
		public int Width { get { return width; } }
		public int Height { get { return height; } }
		public float ScaledWidth { get { return scaledwidth; } }
		public float ScaledHeight { get { return scaledheight; } }

		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		public ImageData()
		{
			// We have no destructor
			GC.SuppressFinalize(this);
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
					pixeldata = null;
					
					// Done
					isdisposed = true;
				}
			}
		}

		#endregion

		#region ================== Management

		// This sets the name
		protected void SetName(string name)
		{
			this.name = name;
			this.longname = Lump.MakeLongName(name);
		}
		
		// This requests loading the image
		public virtual void LoadImage()
		{
		}
		
		// This creates the 2D pixel data
		public virtual void CreatePixelData()
		{
			BitmapData bmpdata;

			lock(this)
			{
				// Only do this when data is not created yet
				if((pixeldata == null) && IsLoaded)
				{
					// Make a data copy of the bits for the 2D renderer
					bmpdata = bitmap.LockBits(new Rectangle(0, 0, bitmap.Size.Width, bitmap.Size.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
					pixeldata = new PixelColorBlock(bitmap.Size.Width, bitmap.Size.Height);
					General.CopyMemory((void*)pixeldata.Pointer, bmpdata.Scan0.ToPointer(), new UIntPtr(pixeldata.Length));
					bitmap.UnlockBits(bmpdata);
				}
			}
		}
		
		// This creates the Direct3D texture
		public virtual void CreateTexture()
		{
			MemoryStream memstream;
			
			lock(this)
			{
				// Only do this when texture is not created yet
				if((texture == null) && IsLoaded)
				{
					// Write to memory stream and read from memory
					memstream = new MemoryStream();
					bitmap.Save(memstream, ImageFormat.Bmp);
					memstream.Seek(0, SeekOrigin.Begin);
					texture = Texture.FromStream(General.Map.Graphics.Device, memstream, (int)memstream.Length, bitmap.Size.Width, bitmap.Size.Height, 0,
						Usage.None, Format.Unknown, Pool.Managed, Filter.Box, Filter.Box, 0);
					memstream.Dispose();
				}
			}
		}
		
		// This destroys the Direct3D texture
		public void ReleaseTexture()
		{
			lock(this)
			{
				// Trash it
				if(texture != null) texture.Dispose();
				texture = null;
			}
		}
		
		#endregion
	}
}
