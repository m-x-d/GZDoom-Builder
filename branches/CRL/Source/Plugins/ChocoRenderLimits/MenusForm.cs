#region === Copyright (c) 2010 Pascal van der Heiden ===

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.ComponentModel;
using System.Windows.Forms;
using CodeImp.DoomBuilder.Windows;

#endregion

namespace CodeImp.DoomBuilder.Plugins.ChocoRenderLimits
{
	public partial class MenusForm : Form
	{
		#region ================== Variables

		// Menus list
		private ToolStripItem[] menuitems;

		// Buttons list
		private ToolStripItem[] buttons;

		#endregion

		#region ================== Properties

		#endregion

		#region ================== Constructor / Destructor

		// Constructor
		public MenusForm()
		{
			InitializeComponent();

			// List all menus
			menuitems = new ToolStripItem[toolsmenu.DropDownItems.Count];
			for(int i = 0; i < toolsmenu.DropDownItems.Count; i++) menuitems[i] = toolsmenu.DropDownItems[i];

			// List all buttons
			//buttons = new ToolStripItem[globalstrip.Items.Count];
			//for(int i = 0; i < globalstrip.Items.Count; i++) buttons[i] = globalstrip.Items[i];
		}

		#endregion

		#region ================== Methods

		// This registers with the core
		public void Register()
		{
			// Add the menu items to the core
			foreach(ToolStripItem m in menuitems)
				General.Interface.AddMenu(m as ToolStripMenuItem, MenuSection.ToolsTesting);

			// Add the buttons to the core
			//foreach(ToolStripItem b in buttons)
			//	General.Interface.AddButton(b);
		}

		// This unregisters from the core
		public void Unregister()
		{
			// Remove the menu items from the core
			foreach(ToolStripMenuItem m in menuitems)
				General.Interface.RemoveMenu(m);

			// Remove the buttons from the core
			//foreach(ToolStripItem b in buttons)
			//	General.Interface.RemoveButton(b);
		}

		// This invokes an action from control event
		private void InvokeTaggedAction(object sender, EventArgs e)
		{
			General.Interface.InvokeTaggedAction(sender, e);
		}

		#endregion

		#region ================== Events

		private void settingsitem_Click(object sender, EventArgs e)
		{
			SettingsForm settings = new SettingsForm();
			settings.ShowDialog(General.Interface);
			settings.Dispose();
		}

		#endregion
	}
}
