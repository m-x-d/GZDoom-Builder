
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
using System.Runtime.InteropServices;
using System.Diagnostics;

#endregion

namespace CodeImp.DoomBuilder.Compilers
{
	public struct CompilerError
	{
		// Members
		public string description;
		public string filename;
		public int linenumber;
		
		// Constructor
		public CompilerError(string description, string filename, int linenumber)
		{
			this.description = description;
			this.filename = filename;
			this.linenumber = linenumber;
		}
	}
}

