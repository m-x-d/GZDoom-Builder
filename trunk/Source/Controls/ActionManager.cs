
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
using System.Windows.Forms;

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
		
		// Keys state
		private int modifiers;
		private List<int> pressedkeys;
		
		// Begun actions
		private List<Action> activeactions;
		
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
			pressedkeys = new List<int>();
			activeactions = new List<Action>();
			
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
			string name, title, desc, shortname;
			bool amouse, akeys, ascroll, debugonly, noshift, repeat;
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
						shortname = a.Key.ToString();
						name = asmname.Name.ToLowerInvariant() + "_" + shortname;
						title = cfg.ReadSetting(a.Key + ".title", "[" + name + "]");
						desc = cfg.ReadSetting(a.Key + ".description", "");
						akeys = cfg.ReadSetting(a.Key + ".allowkeys", true);
						amouse = cfg.ReadSetting(a.Key + ".allowmouse", true);
						ascroll = cfg.ReadSetting(a.Key + ".allowscroll", false);
						noshift = cfg.ReadSetting(a.Key + ".disregardshift", false);
						repeat = cfg.ReadSetting(a.Key + ".repeat", false);
						debugonly = cfg.ReadSetting(a.Key + ".debugonly", false);

						// Check if action should be included
						if(General.DebugBuild || !debugonly)
						{
							// Create an action
							CreateAction(name, shortname, title, desc, akeys, amouse, ascroll, noshift, repeat);
						}
					}
				}
			}
		}

		// This manually creates an action
		private void CreateAction(string name, string shortname, string title, string desc, bool allowkeys, bool allowmouse, bool allowscroll, bool disregardshift, bool repeat)
		{
			// Action does not exist yet?
			if(!actions.ContainsKey(name))
			{
				// Read the key from configuration
				int key = General.Settings.ReadSetting("shortcuts." + name, 0);

				// Create an action
				actions.Add(name, new Action(name, shortname, title, desc, key, allowkeys, allowmouse, allowscroll, disregardshift, repeat));
			}
			else
			{
				// Action already exists!
				General.WriteLogLine("WARNING: Action '" + name + "' already exists. Action names must be unique!");
			}
		}

		// This binds all methods marked with this attribute
		internal void BindMethods(Type type)
		{
			// Bind static methods
			BindMethods(null, type);
		}

		// This binds all methods marked with this attribute
		internal void BindMethods(object obj)
		{
			// Bind instance methods
			BindMethods(obj, obj.GetType());
		}

		// This binds all methods marked with this attribute
		private void BindMethods(object obj, Type type)
		{
			MethodInfo[] methods;
			ActionAttribute[] attrs;
			ActionDelegate del;
			string actionname;

			if(obj == null)
				General.WriteLogLine("Binding static action methods for class " + type.Name + "...");
			else
				General.WriteLogLine("Binding action methods for " + type.Name + " object...");

			// Go for all methods on obj
			methods = type.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
			foreach(MethodInfo m in methods)
			{
				// Check if the method has this attribute
				attrs = (ActionAttribute[])m.GetCustomAttributes(typeof(BeginActionAttribute), true);

				// Go for all attributes
				foreach(ActionAttribute a in attrs)
				{
					// Create a delegate for this method
					del = (ActionDelegate)Delegate.CreateDelegate(typeof(ActionDelegate), obj, m);

					// Make proper name
					actionname = a.GetFullActionName(type.Assembly);

					// Bind method to action
					if(Exists(actionname))
						actions[actionname].BindBegin(del);
					else
						throw new ArgumentException("Could not bind " + m.ReflectedType.Name + "." + m.Name + " to action \"" + actionname + "\", that action does not exist! Refer to, or edit Actions.cfg for all available application actions.");
				}
				
				// Check if the method has this attribute
				attrs = (ActionAttribute[])m.GetCustomAttributes(typeof(EndActionAttribute), true);

				// Go for all attributes
				foreach(ActionAttribute a in attrs)
				{
					// Create a delegate for this method
					del = (ActionDelegate)Delegate.CreateDelegate(typeof(ActionDelegate), obj, m);

					// Make proper name
					actionname = a.GetFullActionName(type.Assembly);

					// Bind method to action
					if(Exists(actionname))
						actions[actionname].BindEnd(del);
					else
						throw new ArgumentException("Could not bind " + m.ReflectedType.Name + "." + m.Name + " to action \"" + actionname + "\", that action does not exist! Refer to, or edit Actions.cfg for all available application actions.");
				}
			}
		}

		// This binds a delegate manually
		internal void BindBeginDelegate(Assembly asm, ActionDelegate d, BeginActionAttribute a)
		{
			string actionname;

			// Make proper name
			actionname = a.GetFullActionName(asm);

			// Bind delegate to action
			if(Exists(actionname))
				actions[actionname].BindBegin(d);
			else
				General.WriteLogLine("WARNING: Could not bind delegate for " + d.Method.Name + " to action \"" + a.ActionName + "\" (" + actionname + "), that action does not exist! Refer to, or edit Actions.cfg for all available application actions.");
		}

		// This binds a delegate manually
		internal void BindEndDelegate(Assembly asm, ActionDelegate d, EndActionAttribute a)
		{
			string actionname;

			// Make proper name
			actionname = a.GetFullActionName(asm);

			// Bind delegate to action
			if(Exists(actionname))
				actions[actionname].BindEnd(d);
			else
				General.WriteLogLine("WARNING: Could not bind delegate for " + d.Method.Name + " to action \"" + a.ActionName + "\" (" + actionname + "), that action does not exist! Refer to, or edit Actions.cfg for all available application actions.");
		}

		// This unbinds all methods marked with this attribute
		internal void UnbindMethods(Type type)
		{
			// Unbind static methods
			UnbindMethods(null, type);
		}

		// This unbinds all methods marked with this attribute
		internal void UnbindMethods(object obj)
		{
			// Unbind instance methods
			UnbindMethods(obj, obj.GetType());
		}

		// This unbinds all methods marked with this attribute
		private void UnbindMethods(object obj, Type type)
		{
			MethodInfo[] methods;
			ActionAttribute[] attrs;
			ActionDelegate del;
			string actionname;

			if(obj == null)
				General.WriteLogLine("Unbinding static action methods for class " + type.Name + "...");
			else
				General.WriteLogLine("Unbinding action methods for " + type.Name + " object...");

			// Go for all methods on obj
			methods = type.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
			foreach(MethodInfo m in methods)
			{
				// Check if the method has this attribute
				attrs = (ActionAttribute[])m.GetCustomAttributes(typeof(BeginActionAttribute), true);

				// Go for all attributes
				foreach(ActionAttribute a in attrs)
				{
					// Create a delegate for this method
					del = (ActionDelegate)Delegate.CreateDelegate(typeof(ActionDelegate), obj, m);

					// Make proper name
					actionname = a.GetFullActionName(type.Assembly);

					// Unbind method from action
					actions[actionname].UnbindBegin(del);
				}
				
				// Check if the method has this attribute
				attrs = (ActionAttribute[])m.GetCustomAttributes(typeof(EndActionAttribute), true);

				// Go for all attributes
				foreach(ActionAttribute a in attrs)
				{
					// Create a delegate for this method
					del = (ActionDelegate)Delegate.CreateDelegate(typeof(ActionDelegate), obj, m);

					// Make proper name
					actionname = a.GetFullActionName(type.Assembly);

					// Unbind method from action
					actions[actionname].UnbindEnd(del);
				}
			}
		}

		// This unbinds a delegate manually
		internal void UnbindBeginDelegate(Assembly asm, ActionDelegate d, BeginActionAttribute a)
		{
			string actionname;

			// Make proper name
			actionname = a.GetFullActionName(asm);

			// Unbind delegate to action
			actions[actionname].UnbindBegin(d);
		}

		// This unbinds a delegate manually
		internal void UnbindEndDelegate(Assembly asm, ActionDelegate d, EndActionAttribute a)
		{
			string actionname;

			// Make proper name
			actionname = a.GetFullActionName(asm);

			// Unbind delegate to action
			actions[actionname].UnbindEnd(d);
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
		
		// This checks if a given action is active
		public bool CheckActionActive(Assembly asm, string actionname)
		{
			// Find active action
			string fullname = asm.GetName().Name.ToLowerInvariant() + "_" + actionname;
			foreach(Action a in activeactions)
			{
				if(a.Name == fullname) return true;
			}

			// No such active action
			return false;
		}
		
		// Removes all shortcut keys
		public void RemoveShortcutKeys()
		{
			// Clear all keys
			foreach(KeyValuePair<string, Action> a in actions)
				a.Value.SetShortcutKey(0);
		}
		
		// This notifies a key has been pressed
		public void KeyPressed(int key)
		{
			int strippedkey = key & ~((int)Keys.Alt | (int)Keys.Shift | (int)Keys.Control);
			if((strippedkey == (int)Keys.ShiftKey) || (strippedkey == (int)Keys.ControlKey)) key = strippedkey;
			bool repeat = pressedkeys.Contains(strippedkey);
			
			// Update pressed keys
			if(!repeat) pressedkeys.Add(strippedkey);
			
			// Add action to active list
			Action[] acts = GetActionsByKey(key);
			foreach(Action a in acts) if(!activeactions.Contains(a)) activeactions.Add(a);

			// Invoke actions
			BeginActionByKey(key, repeat);
		}

		// This notifies a key has been released
		public void KeyReleased(int key)
		{
			int strippedkey = key & ~((int)Keys.Alt | (int)Keys.Shift | (int)Keys.Control);
			List<Action> keepactions = new List<Action>();
			
			// Update pressed keys
			if(pressedkeys.Contains(strippedkey)) pressedkeys.Remove(strippedkey);

			// End actions that no longer match
			EndActiveActions();
		}

		// This releases all pressed keys
		public void ReleaseAllKeys()
		{
			// Clear pressed keys
			pressedkeys.Clear();

			// End actions
			EndActiveActions();
		}
		
		// This updates the modifiers
		public void UpdateModifiers(int mods)
		{
			// Update modifiers
			modifiers = mods;

			// End actions that no longer match
			EndActiveActions();
		}
		
		// This will call the associated actions for a keypress
		private void BeginActionByKey(int key, bool repeated)
		{
			// Go for all actions
			foreach(KeyValuePair<string, Action> a in actions)
			{
				// This action is associated with this key?
				if(a.Value.KeyMatches(key))
				{
					// Allowed to repeat?
					if(a.Value.Repeat || !repeated)
					{
						// Invoke action
						a.Value.Begin();
					}
					else
					{
						//General.WriteLogLine("Action \"" + a.Value.Name + "\" failed because it does not support repeating activation!");
					}
				}
			}
		}

		// This will end active actions for which the pressed keys do not match
		private void EndActiveActions()
		{
			List<Action> keepactions = new List<Action>();

			// Go for all active actions
			foreach(Action a in activeactions)
			{
				// Go for all pressed keys
				bool stillactive = false;
				foreach(int k in pressedkeys)
				{
					if((k == (int)Keys.ShiftKey) || (k == (int)Keys.ControlKey))
						stillactive |= a.KeyMatches(k);
					else
						stillactive |= a.KeyMatches(k | modifiers);
				}

				// End the action if no longer matches any of the keys
				if(!stillactive)
				{
					a.End();
				}
				else
				{
					keepactions.Add(a);
				}
			}

			// Update list of activate actions
			activeactions = keepactions;
		}
		
		// This returns all action names for a given key
		public string[] GetActionNamesByKey(int key)
		{
			List<string> actionnames = new List<string>();
			
			// Go for all actions
			foreach(KeyValuePair<string, Action> a in actions)
			{
				// This action is associated with this key?
				if(a.Value.KeyMatches(key))
				{
					// List short name
					actionnames.Add(a.Value.ShortName);
				}
			}

			// Return result;
			return actionnames.ToArray();
		}

		// This returns all action names for a given key
		public Action[] GetActionsByKey(int key)
		{
			List<Action> actionnames = new List<Action>();

			// Go for all actions
			foreach(KeyValuePair<string, Action> a in actions)
			{
				// This action is associated with this key?
				if(a.Value.KeyMatches(key))
				{
					// List short name
					actionnames.Add(a.Value);
				}
			}

			// Return result;
			return actionnames.ToArray();
		}
		
		#endregion
	}
}
