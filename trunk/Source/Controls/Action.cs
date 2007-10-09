
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
using System.Windows.Forms;

#endregion

namespace CodeImp.DoomBuilder.Controls
{
	internal class Action
	{
		#region ================== Constants

		public const string NEWMAP = "newmap";
		public const string OPENMAP = "openmap";
		public const string CLOSEMAP = "closemap";
		public const string SAVEMAP = "savemap";
		public const string SAVEMAPAS = "savemapas";
		public const string SHOWOVERVIEW = "showoverview";
		public const string SCROLLNORTH = "scrollnorth";
		public const string SCROLLSOUTH = "scrollsouth";
		public const string SCROLLWEST = "scrollwest";
		public const string SCROLLEAST = "scrolleast";
		public const string ZOOMIN = "zoomin";
		public const string ZOOMOUT = "zoomout";
		public const string CONFIGURATION = "configuration";
		public const string PREFERENCES = "preferences";
		
		#endregion
		
		#region ================== Variables

		// Description
		private string name;
		private string title;
		private string description;

		// Shortcut key
		private int key;

		// Shortcut options
		private bool allowkeys;
		private bool allowmouse;
		private bool allowscroll;
		
		// Delegate
		private List<ActionDelegate> delegates;
		
		#endregion

		#region ================== Properties

		public string Name { get { return name; } }
		public string Title { get { return title; } }
		public string Description { get { return description; } }
		public int ShortcutKey { get { return key; } }
		public bool AllowKeys { get { return allowkeys; } }
		public bool AllowMouse { get { return allowmouse; } }
		public bool AllowScroll { get { return allowscroll; } }

		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		public Action(string name, string title, string description, int key,
					  bool allowkeys, bool allowmouse, bool allowscroll)
		{
			// Initialize
			this.name = name;
			this.title = title;
			this.description = description;
			this.delegates = new List<ActionDelegate>();
			this.allowkeys = allowkeys;
			this.allowmouse = allowmouse;
			this.allowscroll = allowscroll;
			this.key = key;
		}

		// Destructor
		~Action()
		{
			// Moo.
		}
		
		#endregion

		#region ================== Static Methods

		// This returns the shortcut key description for a key
		public static string GetShortcutKeyDesc(int key)
		{
			KeysConverter conv = new KeysConverter();
			int ctrl, button;
			string ctrlprefix = "";
			
			// When key is 0, then return an empty string
			if(key == 0) return "";

			// Split the key in Control and Button
			ctrl = key & ((int)Keys.Control | (int)Keys.Shift | (int)Keys.Alt);
			button = key & ~((int)Keys.Control | (int)Keys.Shift | (int)Keys.Alt);

			// When the button is a control key, then remove the control itsself
			if((button == (int)Keys.ControlKey) ||
			   (button == (int)Keys.ShiftKey))
			{
				ctrl = 0;
				key = key & ~((int)Keys.Control | (int)Keys.Shift | (int)Keys.Alt);
			}
			
			// Determine control prefix
			if(ctrl != 0) ctrlprefix = conv.ConvertToString(key);
			
			// Check if button is special
			switch(button)
			{
				// Scroll down
				case (int)SpecialKeys.MScrollDown:
					
					// Make string representation
					return ctrlprefix + "ScrollDown";

				// Scroll up
				case (int)SpecialKeys.MScrollUp:

					// Make string representation
					return ctrlprefix + "ScrollUp";

				default:
					
					// Use standard key-string conversion
					return conv.ConvertToString(key);
			}
		}

		#endregion

		#region ================== Methods

		// This sets a new key for the action
		public void SetShortcutKey(int key)
		{
			// Make it so.
			this.key = key;
		}
		
		// This binds a delegate to this action
		public void Bind(ActionDelegate method)
		{
			delegates.Add(method);
		}

		// This removes a delegate from this action
		public void Unbind(ActionDelegate method)
		{
			delegates.Remove(method);
		}

		// This raises events for this action
		public void Invoke()
		{
			// No method bound?
			if(delegates.Count == 0)
			{
				General.WriteLogLine("Called action '" + name + "' has no methods bound");
			}
			else
			{
				// Invoke all the delegates
				foreach(ActionDelegate ad in delegates) ad.Invoke();
			}
		}

		#endregion
	}
}
