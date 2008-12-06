
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
using CodeImp.DoomBuilder.Windows;
using CodeImp.DoomBuilder.IO;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Rendering;
using System.Diagnostics;
using CodeImp.DoomBuilder.Actions;

#endregion

namespace CodeImp.DoomBuilder.Editing
{
	internal class UndoSnapshot
	{
		public MemoryStream mapdata;
		public string description;
		public int ticketid;			// For safe withdrawing

		// Constructor
		public UndoSnapshot(string description, MemoryStream mapdata, int ticketid)
		{
			this.ticketid = ticketid;
			this.description = description;
			this.mapdata = mapdata;
		}

		// Constructor
		public UndoSnapshot(UndoSnapshot info, MemoryStream mapdata)
		{
			this.ticketid = info.ticketid;
			this.description = info.description;
			this.mapdata = mapdata;
		}
	}
}
