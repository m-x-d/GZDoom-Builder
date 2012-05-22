
#region ================== Copyright (c) 2012 Pascal vd Heiden

/*
 * Copyright (c) 2012 Pascal vd Heiden, www.codeimp.com
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
using System.Reflection;
using CodeImp.DoomBuilder.Actions;
using CodeImp.DoomBuilder.Windows;
using CodeImp.DoomBuilder.IO;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Rendering;
using CodeImp.DoomBuilder.Geometry;
using System.Drawing;
using CodeImp.DoomBuilder.Editing;
using CodeImp.DoomBuilder.Plugins;
using CodeImp.DoomBuilder.Types;
using CodeImp.DoomBuilder.Config;
using CodeImp.DoomBuilder.Data;

#endregion

namespace CodeImp.DoomBuilder.Plugins.ChocoRenderLimits
{
	public class BuilderPlug : Plug
	{
		#region ================== Variables

		// Objects
		private static BuilderPlug me;
		private MenusForm menusform;
		private ProcessManager manager;
		
		// Settings
		private string exepath;

		#endregion

		#region ================== Properties
		
		// Properties
		public static BuilderPlug Me { get { return me; } }
		public override string Name { get { return "ChocoRenderLimits"; } }
		public string ExecutablePath { get { return exepath; } set { exepath = value; } }
		public ProcessManager ProcessManager { get { return manager; } }
		
		#endregion

		#region ================== Initialize / Dispose

		// This event is called when the plugin is initialized
		public override void OnInitialize()
		{
			base.OnInitialize();

			manager = new ProcessManager();
			
			// Load menu items and toolbar buttons
			menusform = new MenusForm();
			menusform.Register();

			General.Actions.BindMethods(this);

			// Read settings
			ReadSettings();

			// Keep a static reference
			me = this;
		}

		// This is called when the plugin is terminated
		public override void Dispose()
		{
			// Clean up
			manager.Dispose();
			General.Actions.UnbindMethods(this);
			menusform.Unregister();
			menusform.Dispose();
			menusform = null;
			base.Dispose();
		}

		// This reads settings from the plugin configuration
		public void ReadSettings()
		{
			exepath = General.Settings.ReadPluginSetting("executablepath", "");
		}

		#endregion

		#region ================== Methods

		[BeginAction("crl_processes")]
		public void ShowProcesses()
		{
			ProcessesForm form = new ProcessesForm();
			form.ShowDialog(General.Interface);
			form.Dispose();
		}

		// This returns a unique temp filename
		public static string MakeTempFilename(string extension)
		{
			string filename;
			string chars = "abcdefghijklmnopqrstuvwxyz1234567890";
			Random rnd = new Random();
			int i;

			do
			{
				// Generate a filename
				filename = "";
				for(i = 0; i < 8; i++) filename += chars[rnd.Next(chars.Length)];
				filename = Path.Combine(General.TempPath, filename + "." + extension);
			}
			// Continue while file is not unique
			while(File.Exists(filename) || Directory.Exists(filename));

			// Return the filename
			return filename;
		}

		#endregion
	}
}
