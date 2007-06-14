
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
using CodeImp.DoomBuilder.Interface;
using CodeImp.DoomBuilder.IO;
using CodeImp.DoomBuilder.Map;

#endregion

namespace CodeImp.DoomBuilder
{
	internal class MapManager : IDisposable
	{
		#region ================== Constants

		#endregion

		#region ================== Variables

		// Status
		private bool changed;
		
		// Map information
		private string filetitle;
		private string filepathname;
		private MapSet data;
		private MapOptions options;
		private Configuration config;

		// Disposing
		private bool isdisposed = false;

		#endregion

		#region ================== Properties

		public string FilePathName { get { return filepathname; } }
		public string FileTitle { get { return filetitle; } }
		public MapOptions Options { get { return options; } }
		public bool IsChanged { get { return changed; } set { changed = value; } }
		public bool IsDisposed { get { return isdisposed; } }

		#endregion

		#region ================== Constructor / Disposer

		// Constructor for new map
		public MapManager(MapOptions options)
		{
			// Initialize
			this.filetitle = "unnamed.wad";
			this.filepathname = "";
			this.changed = false;
			this.options = options;
			this.config = General.LoadGameConfiguration(options.ConfigFile);
			this.data = new MapSet();
			
			// We have no destructor
			GC.SuppressFinalize(this);
		}

		// Diposer
		public void Dispose()
		{
			// Not already disposed?
			if(!isdisposed)
			{
				// Dispose
				data.Dispose();
				
				// Done
				isdisposed = true;
			}
		}

		#endregion

		#region ================== Methods
		
		#endregion
	}
}
