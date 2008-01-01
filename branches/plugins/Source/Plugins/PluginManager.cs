
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
using System.IO;
using CodeImp.DoomBuilder.Editing;

#endregion

namespace CodeImp.DoomBuilder.Plugins
{
	internal class PluginManager
	{
		#region ================== Constants

		#endregion

		#region ================== Variables

		// Plugins
		private List<Plugin> plugins;
		
		// Disposing
		private bool isdisposed = false;

		#endregion

		#region ================== Properties

		public bool IsDisposed { get { return isdisposed; } }

		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		public PluginManager()
		{
			string[] filenames;
			Plugin p;
			
			// Make plugins list
			this.plugins = new List<Plugin>();
			
			// Find all .dll files
			filenames = Directory.GetFiles(General.PluginsPath, "*.dll", SearchOption.TopDirectoryOnly);
			foreach(string fn in filenames)
			{
				// Load plugin from this file
				General.MainWindow.DisplayStatus("Loading plugin '" + Path.GetFileName(fn) + "'...");
				p = new Plugin(fn);
				if(!p.IsDisposed) this.plugins.Add(p);
			}

			// We have no destructor
			GC.SuppressFinalize(this);
		}

		// Diposer
		public void Dispose()
		{
			// Not already disposed?
			if(!isdisposed)
			{
				// Clean up
				foreach(Plugin p in plugins) p.Dispose();
				
				// Done
				isdisposed = true;
			}
		}

		#endregion

		#region ================== Methods

		// This creates a list of all editing modes in all plugins
		public List<EditModeInfo> GetEditModes()
		{
			List<EditModeInfo> modes = new List<EditModeInfo>();
			Type[] editclasses;
			EditModeAttribute[] attribs;
			
			// Go for all plugins
			foreach(Plugin p in plugins)
			{
				// For all classes that inherit from EditMode
				editclasses = p.FindClasses(typeof(EditMode));
				foreach(Type t in editclasses)
				{
					// For all defined EditMode attributes
					attribs = (EditModeAttribute[])t.GetCustomAttributes(typeof(EditModeAttribute), true);
					foreach(EditModeAttribute attr in attribs)
					{
						// Make edit mode information
						modes.Add(new EditModeInfo(p, t, attr));
					}
				}
			}

			// Return list
			return modes;
		}

		#endregion
	}
}
