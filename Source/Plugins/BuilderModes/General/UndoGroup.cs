
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

namespace CodeImp.DoomBuilder.BuilderModes
{
	public class UndoGroup
	{
		public const int None = 0;
		public const int FloorHeightChange = 1;
		public const int CeilingHeightChange = 2;
		public const int SectorBrightnessChange = 3;
		public const int TextureOffsetChange = 4;
		public const int SectorHeightChange = 5;
		public const int ThingMove = 6; //mxd
		public const int ThingRotate = 7; //mxd
		public const int SurfaceBrightnessChange = 8; //mxd
		public const int TextureScaleChange = 9; //mxd
	}
}
