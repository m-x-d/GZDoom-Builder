using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using SlimDX.Direct3D9;

namespace CodeImp.DoomBuilder.Rendering
{
	// FlatVertex
	public struct FlatVertex
	{
		// Vertex format
		public static readonly int Stride = 7 * 4;

		// Members
		public float x;
		public float y;
		public float z;
		public float w;
		public int c;
		public float u;
		public float v;
	}
}
