
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

#endregion

namespace CodeImp.DoomBuilder.Data
{
	internal abstract unsafe class ImageData : IDisposable
	{
		#region ================== Constants

		#endregion

		#region ================== Variables
		
		// Properties
		private string name;
		private long longname;

		// GDI bitmap
		protected Bitmap bitmap;

		// 2D rendering data
		private PixelColor* pixeldata;
		private uint pixeldatasize;
		
		// Direct3D texture
		private Texture texture;

		// Disposing
		protected bool isdisposed = false;

		#endregion

		#region ================== Properties

		public string Name { get { return name; } }
		public PixelColor* PixelData { get { return pixeldata; } }
		public Bitmap Bitmap { get { return bitmap; } }
		public Texture Texture { get { return texture; } }
		public bool IsLoaded { get { return (bitmap == null); } }
		public bool IsDisposed { get { return isdisposed; } }

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
				// Clean up
				if(bitmap != null) bitmap.Dispose();
				if(texture != null) texture.Dispose();
				if(pixeldata != null) General.VirtualFree((void*)pixeldata, new UIntPtr(pixeldatasize), General.MEM_RELEASE);
				pixeldata = null;
				GC.RemoveMemoryPressure(pixeldatasize);

				// Done
				isdisposed = true;
			}
		}

		#endregion

		#region ================== Management

		// This sets the name
		protected void SetName(string name)
		{
			this.name = name;
			this.longname = General.GetTextureLongName(name);
		}
		
		// This requests loading the image
		public virtual void LoadImage()
		{
			BitmapData bmpdata;
			
			// Check if loading worked
			if(bitmap != null)
			{
				// Make a data copy of the bits for the 2D renderer
				bmpdata = bitmap.LockBits(new Rectangle(0, 0, bitmap.Size.Width, bitmap.Size.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
				pixeldatasize = (uint)(bmpdata.Width * bmpdata.Height);
				pixeldata = (PixelColor*)General.VirtualAlloc(IntPtr.Zero, new UIntPtr(pixeldatasize), General.MEM_COMMIT, General.PAGE_READWRITE);
				General.CopyMemory((void*)pixeldata, bmpdata.Scan0.ToPointer(), new UIntPtr(pixeldatasize));
				bitmap.UnlockBits(bmpdata);
				GC.AddMemoryPressure(pixeldatasize);
			}
		}
		
		// This creates the Direct3D texture
		public void CreateTexture()
		{
			// TODO: Write to memory stream and read with Texture.FromStream
		}
		
		// This destroys the Direct3D texture
		public void ReleaseTexture()
		{
			// Trash it
			if(texture != null) texture.Dispose();
			texture = null;
		}
		
		#endregion
	}
}
