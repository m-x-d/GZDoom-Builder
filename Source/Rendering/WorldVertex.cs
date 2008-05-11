
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
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using System.Drawing;
using System.ComponentModel;
using CodeImp.DoomBuilder.Map;
using SlimDX.Direct3D9;
using SlimDX;
using CodeImp.DoomBuilder.Geometry;
using System.Drawing.Imaging;

#endregion

namespace CodeImp.DoomBuilder.Rendering
{
	// WorldVertex
	public struct WorldVertex
	{
		// Vertex format
		public static readonly int Stride = 6 * 4;

		// Members
		public float x;
		public float y;
		public float z;
		public int c;
		public float u;
		public float v;

		// Constructor
		public WorldVertex(float x, float y, float z, int c, float u, float v)
		{
			this.x = x;
			this.y = y;
			this.z = z;
			this.c = c;
			this.u = u;
			this.v = v;
		}

		// Constructor
		public WorldVertex(float x, float y, float z, float u, float v)
		{
			this.x = x;
			this.y = y;
			this.z = z;
			this.c = -1;
			this.u = u;
			this.v = v;
		}

		// Constructor
		public WorldVertex(float x, float y, float z, int c)
		{
			this.x = x;
			this.y = y;
			this.z = z;
			this.c = c;
			this.u = 0.0f;
			this.v = 0.0f;
		}

		// Constructor
		public WorldVertex(float x, float y, float z)
		{
			this.x = x;
			this.y = y;
			this.z = z;
			this.c = -1;
			this.u = 0.0f;
			this.v = 0.0f;
		}
	}
}
