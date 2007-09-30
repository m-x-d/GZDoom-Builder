
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
using Microsoft.Win32;
using System.Diagnostics;
using CodeImp.DoomBuilder.Controls;

#endregion

namespace CodeImp.DoomBuilder.Interface
{
	public partial class ConfigForm : DelayedForm
	{
		#region ================== Variables

		private bool allowapplycontrol = false;

		#endregion

		#region ================== Constructor

		// Constructor
		public ConfigForm()
		{
			Action[] actions;
			ListViewItem item;
			
			// Initialize
			InitializeComponent();

			// Fill list of actions
			actions = General.Actions.GetAllActions();
			foreach(Action a in actions)
			{
				// Create item
				item = listactions.Items.Add(a.Name, a.Title, 0);
				item.SubItems.Add(Action.GetShortcutKeyDesc(a.ShortcutKey));
				item.SubItems[1].Tag = a.ShortcutKey;
			}

			// Fill combobox with special controls
			actioncontrol.Items.Add(new KeyControl(Keys.LButton, "LButton"));
			actioncontrol.Items.Add(new KeyControl(Keys.MButton, "MButton"));
			actioncontrol.Items.Add(new KeyControl(Keys.RButton, "RButton"));
			actioncontrol.Items.Add(new KeyControl(Keys.XButton1, "XButton1"));
			actioncontrol.Items.Add(new KeyControl(Keys.XButton2, "XButton2"));
			actioncontrol.Items.Add(new KeyControl(SpecialKeys.MScrollUp, "ScrollUp"));
			actioncontrol.Items.Add(new KeyControl(SpecialKeys.MScrollDown, "ScrollDown"));
			actioncontrol.Items.Add(new KeyControl(Keys.LButton | Keys.Shift, "Shift+LButton"));
			actioncontrol.Items.Add(new KeyControl(Keys.MButton | Keys.Shift, "Shift+MButton"));
			actioncontrol.Items.Add(new KeyControl(Keys.RButton | Keys.Shift, "Shift+RButton"));
			actioncontrol.Items.Add(new KeyControl(Keys.XButton1 | Keys.Shift, "Shift+XButton1"));
			actioncontrol.Items.Add(new KeyControl(Keys.XButton2 | Keys.Shift, "Shift+XButton2"));
			actioncontrol.Items.Add(new KeyControl((int)SpecialKeys.MScrollUp | (int)Keys.Shift, "Shift+ScrollUp"));
			actioncontrol.Items.Add(new KeyControl((int)SpecialKeys.MScrollDown | (int)Keys.Shift, "Shift+ScrollDown"));
			actioncontrol.Items.Add(new KeyControl(Keys.LButton | Keys.Control, "Ctrl+LButton"));
			actioncontrol.Items.Add(new KeyControl(Keys.MButton | Keys.Control, "Ctrl+MButton"));
			actioncontrol.Items.Add(new KeyControl(Keys.RButton | Keys.Control, "Ctrl+RButton"));
			actioncontrol.Items.Add(new KeyControl(Keys.XButton1 | Keys.Control, "Ctrl+XButton1"));
			actioncontrol.Items.Add(new KeyControl(Keys.XButton2 | Keys.Control, "Ctrl+XButton2"));
			actioncontrol.Items.Add(new KeyControl((int)SpecialKeys.MScrollUp | (int)Keys.Control, "Ctrl+ScrollUp"));
			actioncontrol.Items.Add(new KeyControl((int)SpecialKeys.MScrollDown | (int)Keys.Control, "Ctrl+ScrollDown"));
			actioncontrol.Items.Add(new KeyControl(Keys.LButton | Keys.Shift | Keys.Control, "Ctrl+Shift+LButton"));
			actioncontrol.Items.Add(new KeyControl(Keys.MButton | Keys.Shift | Keys.Control, "Ctrl+Shift+MButton"));
			actioncontrol.Items.Add(new KeyControl(Keys.RButton | Keys.Shift | Keys.Control, "Ctrl+Shift+RButton"));
			actioncontrol.Items.Add(new KeyControl(Keys.XButton1 | Keys.Shift | Keys.Control, "Ctrl+Shift+XButton1"));
			actioncontrol.Items.Add(new KeyControl(Keys.XButton2 | Keys.Shift | Keys.Control, "Ctrl+Shift+XButton2"));
			actioncontrol.Items.Add(new KeyControl((int)SpecialKeys.MScrollUp | (int)Keys.Shift | (int)Keys.Control, "Ctrl+Shift+ScrollUp"));
			actioncontrol.Items.Add(new KeyControl((int)SpecialKeys.MScrollDown | (int)Keys.Shift | (int)Keys.Control, "Ctrl+Shift+ScrollDown"));
			
			// Fill list of configurations
			foreach(ConfigurationInfo ci in General.Configs)
			{
				// Add a copy
				listconfigs.Items.Add(ci.Clone());
			}

			// Fill combobox with nodebuilders
			confignodebuilder.Items.AddRange(General.Nodebuilders.ToArray());

			// Done
			allowapplycontrol = true;
		}

		#endregion

		#region ================== Controls Panel

		// Item selected
		private void listactions_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
		{
			Action action;
			KeyControl keycontrol;
			int key;
			
			// Anything selected?
			if(listactions.SelectedItems.Count > 0)
			{
				// Begin updating
				allowapplycontrol = false;
				
				// Get the selected action
				action = General.Actions[listactions.SelectedItems[0].Name];
				key = (int)listactions.SelectedItems[0].SubItems[1].Tag;
				
				// Enable panel
				actioncontrolpanel.Enabled = true;
				actiontitle.Text = action.Title;
				actiondescription.Text = action.Description;
				actioncontrol.SelectedIndex = -1;
				actionkey.Text = "";
				
				// See if the key is in the combobox
				for(int i = 0; i < actioncontrol.Items.Count; i++)
				{
					// Select it when the key is found here
					keycontrol = (KeyControl)actioncontrol.Items[i];
					if(keycontrol.key == key) actioncontrol.SelectedIndex = i;
				}
				
				// Otherwise display the key in the textbox
				if(actioncontrol.SelectedIndex == -1)
					actionkey.Text = Action.GetShortcutKeyDesc(key);

				// Focus to the input box
				actionkey.Focus();
				
				// Done
				allowapplycontrol = true;
			}
		}

		// Key released
		private void listactions_KeyUp(object sender, KeyEventArgs e)
		{
			// Nothing selected?
			if(listactions.SelectedItems.Count == 0)
			{
				// Disable panel
				actioncontrolpanel.Enabled = false;
				actiontitle.Text = "(select an action from the list)";
				actiondescription.Text = "";
				actionkey.Text = "";
				actioncontrol.SelectedIndex = -1;
			}
		}

		// Mouse released
		private void listactions_MouseUp(object sender, MouseEventArgs e)
		{
			listactions_KeyUp(sender, new KeyEventArgs(Keys.None));
			
			// Focus to the input box
			actionkey.Focus();
		}

		// Key combination pressed
		private void actionkey_KeyDown(object sender, KeyEventArgs e)
		{
			int key = (int)e.KeyData;
			e.SuppressKeyPress = true;

			// Leave when not allowed to update
			if(!allowapplycontrol) return;

			// Anything selected?
			if(listactions.SelectedItems.Count > 0)
			{
				// Begin updating
				allowapplycontrol = false;

				// Deselect anything from the combobox
				actioncontrol.SelectedIndex = -1;
				
				// Apply the key combination
				listactions.SelectedItems[0].SubItems[1].Text = Action.GetShortcutKeyDesc(key);
				listactions.SelectedItems[0].SubItems[1].Tag = key;
				actionkey.Text = Action.GetShortcutKeyDesc(key);

				// Done
				allowapplycontrol = true;
			}
		}

		// Key combination displayed
		private void actionkey_TextChanged(object sender, EventArgs e)
		{
			// Cursor to the end
			actionkey.SelectionStart = actionkey.Text.Length;
			actionkey.SelectionLength = 0;
		}

		// Special key selected
		private void actioncontrol_SelectedIndexChanged(object sender, EventArgs e)
		{
			KeyControl key;
			
			// Leave when not allowed to update
			if(!allowapplycontrol) return;
			
			// Anything selected?
			if((actioncontrol.SelectedIndex > -1) && (listactions.SelectedItems.Count > 0))
			{
				// Begin updating
				allowapplycontrol = false;

				// Remove text from textbox
				actionkey.Text = "";
				
				// Get the key control
				key = (KeyControl)actioncontrol.SelectedItem;
				
				// Apply the key combination
				listactions.SelectedItems[0].SubItems[1].Text = Action.GetShortcutKeyDesc(key.key);
				listactions.SelectedItems[0].SubItems[1].Tag = key.key;

				// Focus to the input box
				actionkey.Focus();
				
				// Done
				allowapplycontrol = true;
			}
		}

		// Clear clicked
		private void actioncontrolclear_Click(object sender, EventArgs e)
		{
			// Begin updating
			allowapplycontrol = false;

			// Clear textbox and combobox
			actionkey.Text = "";
			actioncontrol.SelectedIndex = -1;
			
			// Apply the key combination
			listactions.SelectedItems[0].SubItems[1].Text = "";
			listactions.SelectedItems[0].SubItems[1].Tag = (int)0;
			
			// Focus to the input box
			actionkey.Focus();
			
			// Done
			allowapplycontrol = true;
		}
		
		#endregion

		#region ================== Configuration Panel

		// Configuration item selected
		private void listconfigs_SelectedIndexChanged(object sender, EventArgs e)
		{
			ConfigurationInfo ci;
			NodebuilderInfo ni;
			
			// Item selected?
			if(listconfigs.SelectedIndex > -1)
			{
				// Enable panels
				panelresources.Enabled = true;
				panelnodebuilder.Enabled = true;
				paneltesting.Enabled = true;

				// Get config info of selected item
				ci = listconfigs.SelectedItem as ConfigurationInfo;

				// Fill resources list
				configresources.EditResourceLocationList(ci.Resources);

				// Go for all nodebuilder items
				confignodebuilder.SelectedIndex = -1;
				for(int i = 0; i < confignodebuilder.Items.Count; i++)
				{
					// Get item
					ni = confignodebuilder.Items[i] as NodebuilderInfo;

					// Item matches configuration setting?
					if(string.Compare(ni.Filename, ci.Nodebuilder, false) == 0)
					{
						// Select this item
						confignodebuilder.SelectedIndex = i;
						break;
					}
				}
				
				// Nodebuilder settings
				configbuildonsave.Checked = ci.BuildOnSave;
				
				// Set test application and parameters
				testapplication.Text = ci.TestProgram;
				testparameters.Text = ci.TestParameters;
			}
			else
			{
				// Disable panels
				panelresources.Enabled = false;
				panelnodebuilder.Enabled = false;
				paneltesting.Enabled = false;
			}
		}

		// Resource locations changed
		private void resourcelocations_OnContentChanged()
		{
			ConfigurationInfo ci;
			
			// Apply to selected configuration
			ci = listconfigs.SelectedItem as ConfigurationInfo;
			ci.Resources.Clear();
			ci.Resources.AddRange(configresources.GetResources());
		}

		// Nodebuilder selection changed
		private void confignodebuilder_SelectedIndexChanged(object sender, EventArgs e)
		{
			ConfigurationInfo ci;

			// Apply to selected configuration
			ci = listconfigs.SelectedItem as ConfigurationInfo;
			if(confignodebuilder.SelectedItem != null)
				ci.Nodebuilder = (confignodebuilder.SelectedItem as NodebuilderInfo).Filename;
		}

		// Build on save selection changed
		private void configbuildonsave_CheckedChanged(object sender, EventArgs e)
		{
			ConfigurationInfo ci;

			// Apply to selected configuration
			ci = listconfigs.SelectedItem as ConfigurationInfo;
			ci.BuildOnSave = configbuildonsave.Checked;
		}

		// Test application changed
		private void testapplication_TextChanged(object sender, EventArgs e)
		{
			ConfigurationInfo ci;

			// Apply to selected configuration
			ci = listconfigs.SelectedItem as ConfigurationInfo;
			ci.TestProgram = testapplication.Text;
		}

		// Test parameters changed
		private void testparameters_TextChanged(object sender, EventArgs e)
		{
			ConfigurationInfo ci;

			// Apply to selected configuration
			ci = listconfigs.SelectedItem as ConfigurationInfo;
			ci.TestParameters = testparameters.Text;
		}
		
		#endregion
		
		#region ================== OK / Cancel

		// OK clicked
		private void apply_Click(object sender, EventArgs e)
		{
			// Apply control keys to actions
			foreach(ListViewItem item in listactions.Items)
				General.Actions[item.Name].SetShortcutKey((int)item.SubItems[1].Tag);
			
			// Apply configuration items
			foreach(ConfigurationInfo ci in listconfigs.Items)
			{
				// Find same configuration info in originals
				foreach(ConfigurationInfo oci in General.Configs)
				{
					// Apply settings when they match
					if(string.Compare(ci.Filename, oci.Filename) == 0) oci.Apply(ci);
				}
			}
			
			// Close
			this.DialogResult = DialogResult.OK;
			this.Hide();
		}

		// Cancel clicked
		private void cancel_Click(object sender, EventArgs e)
		{
			// Close
			this.DialogResult = DialogResult.Cancel;
			this.Hide();
		}

		#endregion
	}
}