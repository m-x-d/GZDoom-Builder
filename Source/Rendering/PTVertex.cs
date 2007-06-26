
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
using SlimDX.Direct3D;
using System.ComponentModel;

#endregion

namespace CodeImp.DoomBuilder.Rendering
{
	// PTVertex
	public struct PTVertex
	{
		// Vertex format
		public static readonly VertexFormat Format = VertexFormat.Position | VertexFormat.Diffuse | VertexFormat.Texture1;
		public static readonly int Stride = 6 * 4;

		// Members
		public float x;
		public float y;
		public float z;
		public int c;
		public float u;
		public float v;
	}
}
