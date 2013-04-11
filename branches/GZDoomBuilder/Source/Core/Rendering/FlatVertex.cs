
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

#endregion

namespace CodeImp.DoomBuilder.Rendering
{
	// FlatVertex
	public struct FlatVertex
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
	}
}
