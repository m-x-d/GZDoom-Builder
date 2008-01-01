
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
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using CodeImp.DoomBuilder.Properties;
using System.IO;
using CodeImp.DoomBuilder.IO;
using System.Collections;
using System.Reflection;

#endregion

namespace CodeImp.DoomBuilder.Controls
{
	internal class ActionManager
	{
		#region ================== Constants

		private const string ACTIONS_RESOURCE = "Actions.cfg";

		#endregion

		#region ================== Variables

		// Actions
		private Dictionary<string, Action> actions;
		
		// Disposing
		private bool isdisposed = false;

		#endregion

		#region ================== Properties

		public Action this[string action] { get { if(actions.ContainsKey(action)) return actions[action]; else throw new ArgumentException("There is no such action \"" + action + "\""); } }
		public bool IsDisposed { get { return isdisposed; } }

		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		public ActionManager()
		{
			// Initialize
			General.WriteLogLine("Starting action manager...");
			actions = new Dictionary<string, Action>();

			// Load all actions in this assembly
			LoadActions(General.ThisAssembly);
			
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
				
				// Done
				isdisposed = true;
			}
		}

		#endregion

		#region ================== Actions

		// This loads all actions from an assembly
		public void LoadActions(Assembly asm)
		{
			Stream actionsdata;
			StreamReader actionsreader;
			Configuration cfg;
			string name, title, desc;
			bool amouse, akeys, ascroll;
			string[] resnames;
			AssemblyName asmname = asm.GetName();

			// Find a resource named Actions.cfg
			resnames = asm.GetManifestResourceNames();
			foreach(string rn in resnames)
			{
				// Found one?
				if(rn.EndsWith(ACTIONS_RESOURCE, StringComparison.InvariantCultureIgnoreCase))
				{
					// Get a stream from the resource
					actionsdata = asm.GetManifestResourceStream(rn);
					actionsreader = new StreamReader(actionsdata, Encoding.ASCII);

					// Load configuration from stream
					cfg = new Configuration();
					cfg.InputConfiguration(actionsreader.ReadToEnd());

					// Done with the resource
					actionsreader.Dispose();
					actionsdata.Dispose();

					// Go for all objects in the configuration
					foreach(DictionaryEntry a in cfg.Root)
					{
						// Get action properties
						name = asmname.Name.ToLowerInvariant() + "_" + a.Key.ToString();
						title = cfg.ReadSetting(a.Key + ".title", "[" + name + "]");
						desc = cfg.ReadSetting(a.Key + ".description", "");
						akeys = cfg.ReadSetting(a.Key + ".allowkeys", false);
						amouse = cfg.ReadSetting(a.Key + ".allowmouse", false);
						ascroll = cfg.ReadSetting(a.Key + ".allowscroll", false);

						// Create an action
						CreateAction(name, title, desc, akeys, amouse, ascroll);
					}
				}
			}
		}

		// This manually creates an action
		private void CreateAction(string name, string title, string desc, bool allowkeys, bool allowmouse, bool allowscroll)
		{
			// Action does not exist yet?
			if(!actions.ContainsKey(name))
			{
				// Read the key from configuration
				int key = General.Settings.ReadSetting("shortcuts." + name, 0);

				// Create an action
				actions.Add(name, new Action(name, title, desc, key, allowkeys, allowmouse, allowscroll));
			}
			else
			{
				// Action already exists!
				General.WriteLogLine("WARNING: Action '" + name + "' already exists. Action names must be unique!");
			}
		}
		
		// This checks if a given action exists
		public bool Exists(string action)
		{
			return actions.ContainsKey(action);
		}

		// This returns a list of all actions
		public Action[] GetAllActions()
		{
			Action[] list = new Action[actions.Count];
			actions.Values.CopyTo(list, 0);
			return list;
		}
		
		// This saves the control settings
		public void SaveSettings()
		{
			// Go for all actions
			foreach(KeyValuePair<string, Action> a in actions)
			{
				// Write to configuration
				General.Settings.WriteSetting("shortcuts." + a.Key, a.Value.ShortcutKey);
			}
		}
		
		#endregion

		#region ================== Shortcut Keys

		// Removes all shortcut keys
		public void RemoveShortcutKeys()
		{
			// Clear all keys
			foreach(KeyValuePair<string, Action> a in actions)
				a.Value.SetShortcutKey(0);
		}
		
		// This will call the associated action for a keypress
		public void InvokeByKey(int key)
		{
			// Go for all actions
			foreach(KeyValuePair<string, Action> a in actions)
			{
				// This action is associated with this key?
				if(a.Value.ShortcutKey == key)
				{
					// Invoke action
					a.Value.Invoke();
				}
			}
		}
		
		#endregion
	}
}
