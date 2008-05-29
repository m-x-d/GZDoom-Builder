
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
using CodeImp.DoomBuilder.Windows;
using CodeImp.DoomBuilder.IO;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Rendering;
using CodeImp.DoomBuilder.Geometry;
using System.Drawing;
using CodeImp.DoomBuilder.Editing;

#endregion

namespace CodeImp.DoomBuilder.BuilderModes
{
	public partial class MenusForm : Form
	{
		#region ================== Variables

		// Menus list
		private ToolStripItem[] menus;

		// Buttons list
		private ToolStripItem[] buttons;

		#endregion

		#region ================== Properties

		public ToolStripMenuItem LinedefsMenu { get { return linedefsmenu; } }
		public ToolStripMenuItem SectorsMenu { get { return sectorsmenu; } }
		public ToolStripMenuItem ThingsMenu { get { return thingsmenu; } }

		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		public MenusForm()
		{
			// Initialize
			InitializeComponent();

			// List all menus
			menus = new ToolStripItem[menustrip.Items.Count];
			for(int i = 0; i < menustrip.Items.Count; i++) menus[i] = menustrip.Items[i];

			// List all buttons
			buttons = new ToolStripItem[toolstrip.Items.Count];
			for(int i = 0; i < toolstrip.Items.Count; i++) buttons[i] = toolstrip.Items[i];
		}
		
		#endregion

		#region ================== Methods

		// This registers with the core
		public void Register()
		{
			// Add the menus to the core
			foreach(ToolStripMenuItem m in menus)
				General.Interface.AddMenu(m);
			
			// Add the buttons to the core
			foreach(ToolStripItem b in buttons)
				General.Interface.AddButton(b);
		}

		// This unregisters from the core
		public void Unregister()
		{
			// Remove the menus from the core
			foreach(ToolStripMenuItem m in menus)
				General.Interface.RemoveMenu(m);

			// Remove the buttons from the core
			foreach(ToolStripItem b in buttons)
				General.Interface.RemoveButton(b);
		}

		// This hides all menus
		public void HideAllMenus()
		{
			foreach(ToolStripMenuItem m in menus) m.Visible = false;
		}
		
		// This hides all except one menu
		public void HideAllMenusExcept(ToolStripMenuItem showthis)
		{
			HideAllMenus();
			showthis.Visible = true;
		}
		
		// This shows the menu for the current editing mode
		public void ShowEditingModeMenu(EditMode mode)
		{
			// When these modes are active, then test against the base mode they will return to
			if(mode is DragGeometryMode) mode = (mode as DragGeometryMode).BaseMode;
			if(mode is DragThingsMode) mode = (mode as DragThingsMode).BaseMode;
			if(mode is DrawGeometryMode) mode = (mode as DrawGeometryMode).BaseMode;
			if(mode is CurveLinedefsMode) mode = (mode as CurveLinedefsMode).BaseMode;
			
			// Final decision
			if(mode is LinedefsMode) HideAllMenusExcept(linedefsmenu);
			else if(mode is SectorsMode) HideAllMenusExcept(sectorsmenu);
			else if(mode is ThingsMode) HideAllMenusExcept(thingsmenu);
			else HideAllMenus();
		}

		// This invokes an action from control event
		private void InvokeTaggedAction(object sender, EventArgs e)
		{
			General.Interface.InvokeTaggedAction(sender, e);
		}
		
		#endregion
	}
}