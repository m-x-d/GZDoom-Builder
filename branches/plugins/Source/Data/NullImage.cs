
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
	public sealed class NullImage : ImageData
	{
		#region ================== Constructor / Disposer

		// Constructor
		public NullImage()
		{
			// Initialize
			this.width = 0;
			this.height = 0;
			SetName("");

			// We have no destructor
			GC.SuppressFinalize(this);
		}

		#endregion

		#region ================== Methods
		
		// Dont do anything
		public override void LoadImage() { bitmap = CodeImp.DoomBuilder.Properties.Resources.UnknownImage; }
		internal override void CreatePixelData() { }
		internal override void CreateTexture() { }

		#endregion
	}
}
