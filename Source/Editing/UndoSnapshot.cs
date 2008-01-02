
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
using CodeImp.DoomBuilder.Interface;
using CodeImp.DoomBuilder.IO;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Rendering;
using System.Diagnostics;
using CodeImp.DoomBuilder.Controls;

#endregion

namespace CodeImp.DoomBuilder.Editing
{
	internal class UndoSnapshot
	{
		public MapSet map;
		public string description;
		public bool allow3dchange;		// True when allowed to change in 3D mode
		public int ticketid;			// For safe withdrawing

		// Constructor
		public UndoSnapshot(string description, bool allow3dchange, MapSet map, int ticketid)
		{
			this.ticketid = ticketid;
			this.description = description;
			this.allow3dchange = allow3dchange;
			this.map = map;
		}

		// Constructor
		public UndoSnapshot(UndoSnapshot info, MapSet map)
		{
			this.ticketid = info.ticketid;
			this.description = info.description;
			this.allow3dchange = info.allow3dchange;
			this.map = map;
		}
	}
}
