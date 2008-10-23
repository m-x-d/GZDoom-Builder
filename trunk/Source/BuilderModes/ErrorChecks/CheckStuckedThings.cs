
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
using CodeImp.DoomBuilder.Editing;
using CodeImp.DoomBuilder.Actions;
using CodeImp.DoomBuilder.Types;
using CodeImp.DoomBuilder.Config;
using System.Threading;

#endregion

namespace CodeImp.DoomBuilder.BuilderModes
{
	[ErrorChecker("Check for stucked things", true)]
	public class CheckStuckedThings : ErrorChecker
	{
		#region ================== Constructor / Destructor
		
		// Constructor
		public CheckStuckedThings()
		{
			// Total progress is done when all things are checked
			SetTotalProgress(General.Map.Map.Things.Count);
		}
		
		#endregion
		
		#region ================== Methods
		
		// This runs the check
		public override void Run()
		{
			// Go for all the things
			foreach(Thing t in General.Map.Map.Things)
			{
				// Make square from thing
				
				// Go for all the lines
				foreach(Linedef l in General.Map.Map.Linedefs)
				{
					// Test if the line intersects the square
					
				}
				
				// Handle thread interruption
				try { Thread.Sleep(0); }
				catch(ThreadInterruptedException) { return; }
			}
		}
		
		#endregion
	}
}
