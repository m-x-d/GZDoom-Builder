
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

		// Disposing
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

		#endregion
	}
}
