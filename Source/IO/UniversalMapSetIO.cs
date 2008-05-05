
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
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Geometry;

#endregion

namespace CodeImp.DoomBuilder.IO
{
	internal class UniversalMapSetIO : MapSetIO
	{
		#region ================== Constants

		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		public UniversalMapSetIO(WAD wad, MapManager manager)
			: base(wad, manager)
		{
		}

		#endregion

		#region ================== Properties

		public override int MaxSidedefs { get { return int.MaxValue; } }
		public override int VertexDecimals { get { return 3; } }

		#endregion

		#region ================== Parsing

		#endregion
		
		#region ================== Reading

		// This reads a map from the file and returns a MapSet
		public override MapSet Read(MapSet map, string mapname)
		{
			// Return result
			return map;
		}

		#endregion

		#region ================== Writing

		// This writes a MapSet to the file
		public override void Write(MapSet map, string mapname, int position)
		{

		}

		#endregion
	}
}
