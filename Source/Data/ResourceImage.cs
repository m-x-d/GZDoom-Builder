
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
	public class ResourceImage : ImageData
	{
		#region ================== Constructor / Disposer

		// Constructor
		public ResourceImage(string resourcename)
		{
			// Initialize
			SetName(resourcename);

			// Temporarily load resource from memory
			Stream bitmapdata = General.ThisAssembly.GetManifestResourceStream("CodeImp.DoomBuilder.Resources." + Name);
			Bitmap bmp = (Bitmap)Image.FromStream(bitmapdata);

			// Get width and height from image
			width = bmp.Size.Width;
			height = bmp.Size.Height;
			scaledwidth = (float)bmp.Size.Width;
			scaledheight = (float)bmp.Size.Height;
			
			// Done
			bmp.Dispose();
			bitmapdata.Dispose();

			// We have no destructor
			GC.SuppressFinalize(this);
		}

		#endregion

		#region ================== Methods

		// This loads the image
		protected override void LocalLoadImage()
		{
			Stream bitmapdata;

			lock(this)
			{
				// Get resource from memory
				bitmapdata = General.ThisAssembly.GetManifestResourceStream("CodeImp.DoomBuilder.Resources." + Name);
				if(bitmap != null) bitmap.Dispose();
				bitmap = (Bitmap)Image.FromStream(bitmapdata);
				bitmapdata.Dispose();
				
				// Pass on to base
				base.LocalLoadImage();
			}
		}
		
		#endregion
	}
}
