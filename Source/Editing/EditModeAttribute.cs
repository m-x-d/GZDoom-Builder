
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

namespace CodeImp.DoomBuilder.Editing
{
	/// <summary>
	/// This registers an EditMode derived class as a known editing mode within Doom Builder.
	/// Allows automatic binding with an action and a button on the toolbar/menu.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = true)]
	public class EditModeAttribute : Attribute
	{
		#region ================== Variables

		// Properties
		private string switchaction = null;
		private string buttonimage = null;
		private string buttondesc = null;
		private int buttonorder = 0;
		private bool configspecific = false;
		private bool isvolatile = false;
		private string displayname = "Unnamed Mode";
		private bool allowcopypaste = true;
		
		#endregion

		#region ================== Properties

		/// <summary>
		/// Sets the action name (as defined in the Actions.cfg resource) to
		/// switch to this mode by using a shortcut key, toolbar button or menu item.
		/// </summary>
		public string SwitchAction { get { return switchaction; } set { switchaction = value; } }

		/// <summary>
		/// Image resource name of the embedded resource that will be used for the
		/// toolbar button and menu item. Leave this property out or set to null to
		/// display no button for this mode.
		/// </summary>
		public string ButtonImage { get { return buttonimage; } set { buttonimage = value; } }

		/// <summary>
		/// Toolbar button and menu item description of this mode.
		/// </summary>
		public string ButtonDesc { get { return buttondesc; } set { buttondesc = value; } }

		/// <summary>
		/// Sorting number for the order of buttons on the toolbar. Buttons with
		/// lower values will be more to the left than buttons with higher values.
		/// </summary>
		public int ButtonOrder { get { return buttonorder; } set { buttonorder = value; } }

		/// <summary>
		/// When set to true, this mode is only accessible from
		/// the toolbar/menu when the game configuration specifies this mode by
		/// class name in the "additionalmodes" structure.
		/// </summary>
		public bool ConfigSpecific { get { return configspecific; } set { configspecific = value; } }

		/// <summary>
		/// When set to true, this mode is cancelled when core actions like
		/// undo and save are performed. The editing mode should then return to
		/// a non-volatile mode.
		/// </summary>
		public bool Volatile { get { return isvolatile; } set { isvolatile = value; } }

		/// <summary>
		/// Name to display on the button information bar when no specific element
		/// information is displayed.
		/// </summary>
		public string DisplayName { get { return displayname; } set { displayname = value; } }

		/// <summary>
		/// When set to false, the actions Cut, Copy and Paste cannot be used
		/// in this mode. Default for this property is true.
		/// </summary>
		public bool AllowCopyPaste { get { return allowcopypaste; } set { allowcopypaste = value; } }
		
		#endregion

		#region ================== Constructor / Disposer

		/// <summary>
		/// This registers an EditMode derived class as a known editing mode within Doom Builder.
		/// Allows automatic binding with an action and a button on the toolbar/menu.
		/// </summary>
		public EditModeAttribute()
		{
			// Initialize
		}

		#endregion

		#region ================== Methods

		#endregion
	}
}
