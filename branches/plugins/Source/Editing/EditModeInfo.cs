
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
using CodeImp.DoomBuilder.Controls;
using CodeImp.DoomBuilder.Plugins;

#endregion

namespace CodeImp.DoomBuilder.Editing
{
	internal class EditModeInfo
	{
		#region ================== Constants

		#endregion

		#region ================== Variables

		// Mode type
		private Plugin plugin;
		private Type type;
		
		// Mode switching
		private ActionAttribute switchactionattr = null;
		private ActionDelegate switchactiondel = null;

		#endregion

		#region ================== Properties

		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		public EditModeInfo(Plugin plugin, Type type, EditModeAttribute attr)
		{
			// Initialize
			this.plugin = plugin;
			this.type = type;

			// Make switch action info
			if((attr.SwitchAction != null) && (attr.SwitchAction.Length > 0))
			{
				switchactionattr = new ActionAttribute(attr.SwitchAction);
				switchactiondel = new ActionDelegate(SwitchToMode);

				// Bind switch action
				ActionAttribute.BindDelegate(plugin.Assembly, switchactiondel, switchactionattr);
			}
			
			// We have no destructor
			GC.SuppressFinalize(this);
		}

		// Disposer
		public void Dispose()
		{
			// Unbind switch action
			if(switchactiondel != null) ActionAttribute.UnbindDelegate(plugin.Assembly, switchactiondel, switchactionattr);

			// Clean up
			plugin = null;
		}
		
		#endregion

		#region ================== Methods

		// This switches to the mode
		public void SwitchToMode()
		{
			EditMode newmode;
			
			// Create instance
			newmode = plugin.CreateObject<EditMode>(type);

			// Switch mode
			General.Map.ChangeMode(newmode);
		}

		// This switches to the mode with arguments
		public void SwitchToMode(object[] args)
		{
			EditMode newmode;

			// Create instance
			newmode = plugin.CreateObjectA<EditMode>(type, args);

			// Switch mode
			General.Map.ChangeMode(newmode);
		}

		// String representation
		public override string ToString()
		{
			return type.Name;
		}
		
		#endregion
	}
}
