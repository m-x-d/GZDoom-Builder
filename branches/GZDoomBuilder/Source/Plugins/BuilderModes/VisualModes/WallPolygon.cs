#region === Copyright (c) 2010 Pascal van der Heiden ===

using System.Collections.Generic;
using CodeImp.DoomBuilder.Geometry;

#endregion

namespace CodeImp.DoomBuilder.BuilderModes
{
	internal class WallPolygon : List<Vector3D>
	{
		// The color that the wall should have
		public int color;
		
		// Constructor
		public WallPolygon()
		{
		}

		// Constructor
		public WallPolygon(int capacity) : base(capacity)
		{
		}

		// Constructor
		public WallPolygon(IEnumerable<Vector3D> collection) : base(collection)
		{
		}
		
		// This copies all the wall properties
		public void CopyProperties(WallPolygon target)
		{
			target.color = this.color;
		}
	}
}
