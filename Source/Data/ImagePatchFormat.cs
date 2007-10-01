
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

#endregion

namespace CodeImp.DoomBuilder.Data
{
	internal enum ImagePatchFormat : int
	{
		Unknown = 0,			// Not determined yet
		Invalid = 1,			// Considered invalid
		DoomImage = 2,			// Doom Image format  (column list rendered data)
		DoomFlat = 3,			// Doom Flat format   (raw 8-bit pixel data)
		PNG = 4,				// Portable Network Graphic
		Bitmap_P8 = 5,			// Bitmap 8-bit Paletted
		Bitmap_B5G6R5 = 6,		// Bitmap 16-bit
		Bitmap_B8G8R8 = 7,		// Bitmap 24-bit
		Bitmap_A8B8G8R8 = 8,	// Bitmap 32-bit
	}
}
