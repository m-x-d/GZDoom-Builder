
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
using CodeImp.DoomBuilder.IO;
using CodeImp.DoomBuilder.Data;
using System.IO;
using System.Diagnostics;
using System.Windows.Forms;
using CodeImp.DoomBuilder.Map;

#endregion

namespace CodeImp.DoomBuilder.Config
{
	public class LinedefActivateInfo : INumberedTitle, IComparable<LinedefActivateInfo>
	{
		#region ================== Constants

		#endregion

		#region ================== Variables

		// Properties
		private int index;
		private string title;
		
		#endregion

		#region ================== Properties

		public int Index { get { return index; } }
		public string Title { get { return title; } }

		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		internal LinedefActivateInfo(int index, string title)
		{
			// Initialize
			this.index = index;
			this.title = title;
			
			// We have no destructor
			GC.SuppressFinalize(this);
		}

		#endregion

		#region ================== Methods

		// This presents the item as string
		public override string ToString()
		{
			return title;
		}

		// This compares against another activate info
		public int CompareTo(LinedefActivateInfo other)
		{
			if(this.index < other.index) return -1;
			else if(this.index > other.index) return 1;
			else return 0;
		}
		
		#endregion
	}
}
