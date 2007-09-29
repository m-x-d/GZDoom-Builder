
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
			}

			// Fill combobox with special controls
			actioncontrol.Items.Add(Keys.LButton);
			actioncontrol.Items.Add(Keys.MButton);
			actioncontrol.Items.Add(Keys.RButton);
			actioncontrol.Items.Add(Keys.XButton1);
			actioncontrol.Items.Add(Keys.XButton2);
			actioncontrol.Items.Add(SpecialKeys.MScrollUp);
			actioncontrol.Items.Add(SpecialKeys.MScrollDown);

			// Fill list of configurations
			foreach(ConfigurationInfo ci in General.Configs)
			{
				// Add a copy
				listconfigs.Items.Add(ci.Clone());
			}

			// Fill combobox with nodebuilders
			confignodebuilder.Items.AddRange(General.Nodebuilders.ToArray());
		}

		#endregion

		#region ================== Controls Panel

		// Item selected
		private void listactions_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
		{
			Action action;
			
			// Anything selected?
			if(listactions.SelectedItems.Count > 0)
			{
				// Get the selected action
				action = General.Actions[listactions.SelectedItems[0].Name];
				
				// Enable panel
				actioncontrolpanel.Enabled = true;
				actiontitle.Text = action.Title;
				actiondescription.Text = action.Description;
				
				// See if the key is in the combobox
				
				// Otherwise display the key in the textbox
				
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