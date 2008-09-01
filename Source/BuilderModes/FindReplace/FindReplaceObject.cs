
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
using CodeImp.DoomBuilder.Geometry;
using System.Drawing;
using CodeImp.DoomBuilder.Editing;

#endregion

namespace CodeImp.DoomBuilder.BuilderModes
{
	internal class FindReplaceObject
	{
		#region ================== Variables

		private object obj;
		private string title;

		#endregion

		#region ================== Properties

		public object Object { get { return obj; } set { obj = value; } }
		public Sector Sector { get { return (Sector)obj; } }
		public Linedef Linedef { get { return (Linedef)obj; } }
		public Thing Thing { get { return (Thing)obj; } }
		public string Title { get { return title; } set { title = value; } }

		#endregion

		#region ================== Constructor / Destructor

		// Constructor
		public FindReplaceObject(object obj, string title)
		{
			// Initialize
			this.obj = obj;
			this.title = title;
		}

		#endregion

		#region ================== Methods

		// String representation
		public override string ToString()
		{
			return title;
		}
		
		#endregion
	}
}
