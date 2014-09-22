#region ================== Namespaces

using System;
using System.Drawing;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.VisualModes;

#endregion

namespace CodeImp.DoomBuilder.BuilderModes
{
	public static class BuilderModesTools
	{
		#region ================== Sidedef

		internal static Rectangle GetSidedefPartSize(BaseVisualGeometrySidedef side, VisualGeometryType type) 
		{
			if(type == VisualGeometryType.WALL_MIDDLE_3D) 
			{
				Rectangle rect = new Rectangle(0, 0, 1, 0);
				Linedef cl = side.GetControlLinedef();
				if(cl.Front != null && cl.Front.Sector != null) {
					// Use ceiling height for vavoom-type 3d floors. Also, FloorHeight is > CeilHeight for these...
					if (cl.Args[1] == 0)
					{
						rect.Y = cl.Front.Sector.CeilHeight;
						rect.Height = cl.Front.Sector.FloorHeight - cl.Front.Sector.CeilHeight;
					}
					else
					{
						rect.Y = cl.Front.Sector.FloorHeight;
						rect.Height = cl.Front.GetMiddleHeight();
					}
					
				} else {
					rect.Y = side.Sidedef.Sector.FloorHeight;
					rect.Height = side.Sidedef.GetMiddleHeight();
				}

				return rect;
			}

			return GetSidedefPartSize(side.Sidedef, type);
		}

		public static Rectangle GetSidedefPartSize(Sidedef side, VisualGeometryType type) 
		{
			Rectangle rect = new Rectangle(0, 0, 1, 0);

			switch(type) 
			{
				case VisualGeometryType.WALL_LOWER:
					rect.Y = side.Sector.FloorHeight;
					rect.Height = side.GetLowHeight();
					break;

				case VisualGeometryType.WALL_UPPER:
					if(side.Other != null && side.Other.Sector != null) 
					{
						rect.Y = side.Other.Sector.CeilHeight;
						rect.Height = side.GetHighHeight();
					} 
					else 
					{
						rect.Height = 0;
					}
					break;

				case VisualGeometryType.WALL_MIDDLE:
					rect.Y = side.Sector.FloorHeight;
					rect.Height = side.GetMiddleHeight();
					break;

				default:
					throw new NotImplementedException("GetSidedefPartSize: got unsupported geometry type: '" + type + "'");
			}

			return rect;
		}

		#endregion
	}
}
