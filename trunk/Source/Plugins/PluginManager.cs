
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
using System.Reflection;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Rendering;

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
		
		internal List<Plugin> Plugins { get { return plugins; } }
		public bool IsDisposed { get { return isdisposed; } }
		
		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		public PluginManager()
		{
			// Make lists
			this.plugins = new List<Plugin>();

			// We have no destructor
			GC.SuppressFinalize(this);
		}

		// Disposer
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
		
		// This creates a list of assemblies
		public List<Assembly> GetPluginAssemblies()
		{
			List<Assembly> asms = new List<Assembly>(plugins.Count);
			foreach(Plugin p in plugins)
				asms.Add(p.Assembly);
			return asms;
		}
		
		
		// This loads all plugins
		public void LoadAllPlugins()
		{
			string[] filenames;
			Type[] editclasses;
			EditModeAttribute[] emattrs;
			EditModeInfo editmodeinfo;
			Plugin p;

			// Find all .dll files
			filenames = Directory.GetFiles(General.PluginsPath, "*.dll", SearchOption.TopDirectoryOnly);
			foreach(string fn in filenames)
			{
				// Load plugin from this file
				General.MainWindow.DisplayStatus("Loading plugin '" + Path.GetFileName(fn) + "'...");
				try
				{
					p = new Plugin(fn);
				}
				catch(InvalidProgramException)
				{
					General.WriteLogLine("WARNING: Plugin file '" + Path.GetFileName(fn) + "' was not loaded.");
					p = null;
				}

				// Continue if no errors
				if((p != null) && (!p.IsDisposed))
				{
					// Add to plugins
					this.plugins.Add(p);

					// Load actions
					General.Actions.LoadActions(p.Assembly);
					
					// Plugin is now initialized
					p.Plug.OnInitialize();
				}
			}
		}
		
		// This returns a plugin by assembly, or null when plugin cannot be found
		public Plugin FindPluginByAssembly(Assembly assembly)
		{
			// Go for all plugins the find the one with matching assembly
			foreach(Plugin p in plugins)
			{
				if(p.Assembly == assembly) return p;
			}

			// Nothing found
			return null;
		}

		#endregion

		#region ================== Events


		public void ReloadResources()
		{
			foreach(Plugin p in plugins) p.Plug.OnReloadResources();
		}


		public bool ModeChanges(EditMode oldmode, EditMode newmode)
		{
			bool result = true;
			foreach(Plugin p in plugins) result &= p.Plug.OnModeChange(oldmode, newmode);
			return result;
		}


		public void ProgramReconfigure()
		{
			foreach(Plugin p in plugins) p.Plug.OnProgramReconfigure();
		}


		public void MapReconfigure()
		{
			foreach(Plugin p in plugins) p.Plug.OnMapReconfigure();
		}


		public bool OnCopyBegin()
		{
			bool result = true;
			foreach(Plugin p in plugins) result &= p.Plug.OnCopyBegin(result);
			return result;
		}


		public void OnCopyEnd()
		{
			foreach(Plugin p in plugins) p.Plug.OnCopyEnd();
		}


		public bool OnPasteBegin()
		{
			bool result = true;
			foreach(Plugin p in plugins) result &= p.Plug.OnPasteBegin(result);
			return result;
		}


		public void OnPasteEnd()
		{
			foreach(Plugin p in plugins) p.Plug.OnPasteEnd();
		}


		public bool OnUndoBegin()
		{
			bool result = true;
			foreach(Plugin p in plugins) result &= p.Plug.OnUndoBegin(result);
			return result;
		}


		public void OnUndoEnd()
		{
			foreach(Plugin p in plugins) p.Plug.OnUndoEnd();
		}


		public bool OnRedoBegin()
		{
			bool result = true;
			foreach(Plugin p in plugins) result &= p.Plug.OnRedoBegin(result);
			return result;
		}


		public void OnRedoEnd()
		{
			foreach(Plugin p in plugins) p.Plug.OnRedoEnd();
		}


		public void OnMapOpenBegin()
		{
			foreach(Plugin p in plugins) p.Plug.OnMapOpenBegin();
		}


		public void OnMapOpenEnd()
		{
			foreach(Plugin p in plugins) p.Plug.OnMapOpenEnd();
		}


		public void OnMapNewBegin()
		{
			foreach(Plugin p in plugins) p.Plug.OnMapNewBegin();
		}


		public void OnMapNewEnd()
		{
			foreach(Plugin p in plugins) p.Plug.OnMapNewEnd();
		}


		public void OnSectorCeilingSurfaceUpdate(Sector s, ref FlatVertex[] vertices)
		{
			foreach(Plugin p in plugins) p.Plug.OnSectorCeilingSurfaceUpdate(s, ref vertices);
		}


		public void OnSectorFloorSurfaceUpdate(Sector s, ref FlatVertex[] vertices)
		{
			foreach(Plugin p in plugins) p.Plug.OnSectorFloorSurfaceUpdate(s, ref vertices);
		}

		
		#endregion
	}
}
