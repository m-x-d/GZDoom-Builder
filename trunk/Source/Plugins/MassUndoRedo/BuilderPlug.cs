
#region ================== Copyright (c) 2009 Pascal vd Heiden

/*
 * Copyright (c) 2009 Pascal vd Heiden, www.codeimp.com
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
using CodeImp.DoomBuilder.Plugins;
using CodeImp.DoomBuilder.Types;
using CodeImp.DoomBuilder.Config;
using CodeImp.DoomBuilder.Data;

#endregion

namespace CodeImp.DoomBuilder.Statistics
{
	public class BuilderPlug : Plug
	{
		#region ================== Variables

		private static BuilderPlug me;
		
		#endregion

		#region ================== Properties

		public static BuilderPlug Me { get { return me; } }
		public override string Name { get { return "Mass Undo/Redo Plugin"; } }

		#endregion

		#region ================== Initialize / Dispose

		// This event is called when the plugin is initialized
		public override void OnInitialize()
		{
			base.OnInitialize();

			// Keep a static reference
			me = this;
		}

		// This is called when the plugin is terminated
		public override void Dispose()
		{
			// Time to clean everything up
			
			base.Dispose();
		}

		#endregion
		
		#region ================== Events

		#endregion
	}
}
