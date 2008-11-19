
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
using CodeImp.DoomBuilder.Actions;
using CodeImp.DoomBuilder.Geometry;
using System.Drawing;
using SlimDX;
using CodeImp.DoomBuilder.Editing;

#endregion

namespace CodeImp.DoomBuilder.VisualModes
{
	public sealed class VisualBlockEntry
	{
		#region ================== Variables
		
		// Members
		private List<Linedef> lines;
		
		#endregion

		#region ================== Properties
		
		public List<Linedef> Lines { get { return lines; } }
		
		#endregion
		
		#region ================== Constructor

		// Constructor for empty block
		internal VisualBlockEntry()
		{
			lines = new List<Linedef>();
		}
		
		#endregion
	}
}
