﻿#region === Copyright (c) 2010 Pascal van der Heiden ===

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
		public fixed byte stats[(int)ViewStats.NumStats];
	}
}
