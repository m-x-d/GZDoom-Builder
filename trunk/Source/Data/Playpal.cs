
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
using System.IO;
using CodeImp.DoomBuilder.Rendering;

#endregion

namespace CodeImp.DoomBuilder.Data
{
	public sealed class Playpal
	{
		#region ================== Constants

		#endregion

		#region ================== Variables

		private PixelColor[] colors;

		#endregion

		#region ================== Properties

		public PixelColor this[int index] { get { return colors[index]; } }

		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		public Playpal()
		{
			// Create array
			colors = new PixelColor[256];

			// Set all palette entries
			for(int i = 0; i < 256; i++)
			{
				// Set colors to gray
				colors[i].r = 127;
				colors[i].g = 127;
				colors[i].b = 127;
				colors[i].a = 255;
			}
		}

		// Constructor
		public Playpal(Stream stream)
		{
			BinaryReader reader = new BinaryReader(stream);
			
			// Create array
			colors = new PixelColor[256];

			// Read all palette entries
			stream.Seek(0, SeekOrigin.Begin);
			for(int i = 0; i < 256; i++)
			{
				// Read colors
				colors[i].r = reader.ReadByte();
				colors[i].g = reader.ReadByte();
				colors[i].b = reader.ReadByte();
				colors[i].a = 255;
			}
		}

		#endregion

		#region ================== Methods

		#endregion
	}
}
