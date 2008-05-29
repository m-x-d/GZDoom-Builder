
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

#endregion

namespace CodeImp.DoomBuilder.Types
{
	internal class TypeHandlerAttribute : Attribute
	{
		#region ================== Constants

		#endregion

		#region ================== Variables

		private int index;

		#endregion

		#region ================== Properties

		public int Index { get { return index; } }

		#endregion

		#region ================== Constructor / Destructor

		// Constructor
		public TypeHandlerAttribute(int index)
		{
			// Initialize
			this.index = index;
		}

		#endregion

		#region ================== Methods

		#endregion
	}
}
