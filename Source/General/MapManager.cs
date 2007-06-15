
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
using CodeImp.DoomBuilder.Editing;
using System.Diagnostics;
using CodeImp.DoomBuilder.Rendering;

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
		private EditMode mode;
		private Graphics graphics;
		
		// Disposing
		private bool isdisposed = false;

		#endregion

		#region ================== Properties

		public string FilePathName { get { return filepathname; } }
		public string FileTitle { get { return filetitle; } }
		public MapOptions Options { get { return options; } }
		public MapSet Data { get { return data; } }
		public EditMode Mode { get { return mode; } }
		public bool IsChanged { get { return changed; } set { changed = value; } }
		public bool IsDisposed { get { return isdisposed; } }
		public Graphics Graphics { get { return graphics; } }
		
		#endregion

		#region ================== Constructor / Disposer

		// Constructor for new map
		public MapManager()
		{
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
				mode.Dispose();
				graphics.Dispose();
				
				// Done
				isdisposed = true;
			}
		}

		#endregion

		#region ================== Initialize
		
		// Initializes for a new map
		public bool InitializeNewMap(MapOptions options)
		{
			// Apply settings
			this.filetitle = "unnamed.wad";
			this.filepathname = "";
			this.changed = false;
			this.options = options;
			
			// Create objects
			data = new MapSet();
			graphics = new Graphics(General.MainWindow.Display);
			config = General.LoadGameConfiguration(options.ConfigFile);

			// Initiate graphics
			if(!graphics.Initialize()) return false;
			
			// Set default mode
			ChangeMode(typeof(FrozenOverviewMode));

			// Success
			return true;
		}
		
		#endregion

		#region ================== Methods

		// This changes editing mode
		public void ChangeMode(Type modetype, params object[] args)
		{
			// Dispose current mode
			if(mode != null) mode.Dispose();
			
			try
			{
				// Create new mode
				mode = (EditMode)General.ThisAssembly.CreateInstance(modetype.FullName, false,
					BindingFlags.Default, null, args, CultureInfo.CurrentCulture, new object[0]);
			}
			// Catch errors
			catch(TargetInvocationException e)
			{
				// Throw the actual exception
				Debug.WriteLine(DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString());
				Debug.WriteLine(e.InnerException.Source + " throws " + e.InnerException.GetType().Name + ":");
				Debug.WriteLine(e.InnerException.Message);
				Debug.WriteLine(e.InnerException.StackTrace);
				throw e.InnerException;
			}
		}
		
		#endregion
	}
}
