
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
using System.IO;

#endregion

namespace CodeImp.DoomBuilder.Data
{
	public sealed class FileImage : ImageData
	{
		#region ================== Variables

		private string filepathname;
		
		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		public FileImage(string filepathname)
		{
			// Initialize
			this.filepathname = filepathname;
			SetName(Path.GetFileNameWithoutExtension(filepathname));
			
			// We have no destructor
			GC.SuppressFinalize(this);
		}

		#endregion

		#region ================== Methods

		// This loads the image
		public override void LoadImage()
		{
			// Leave when already loaded
			if(this.IsLoaded) return;

			// Load file
			bitmap = (Bitmap)Bitmap.FromFile(filepathname);

			// Pass on to base
			base.LoadImage();
		}
		
		#endregion
	}
}
