#region === Copyright (c) 2010 Pascal van der Heiden ===

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

#endregion

namespace CodeImp.DoomBuilder.Plugins.VisplaneExplorer
{
	internal unsafe struct TileData
	{
		// Members
		public fixed byte stats[(int)ViewStats.NumStats];

		// Static instances
		public static TileData VoidTile = new TileData(Tile.STAT_VOID);

		// Constructor
		public TileData(byte fill)
		{
			fixed(byte* s = stats)
			{
				for(int i = 0; i < (int)ViewStats.NumStats; i++)
					s[i] = fill;
			}
		}

		// Compare
		public static bool operator ==(TileData c1, TileData c2)
		{
			bool isequal = true;
			for(int i = 0; i < (int)ViewStats.NumStats; i++)
				if(c1.stats[i] != c2.stats[i]) isequal = false;
			return isequal;
		}

		// Compare
		public static bool operator !=(TileData c1, TileData c2)
		{
			bool isequal = true;
			for(int i = 0; i < (int)ViewStats.NumStats; i++)
				if(c1.stats[i] != c2.stats[i]) isequal = false;
			return !isequal;
		}
	}
}
