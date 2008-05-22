
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
using CodeImp.DoomBuilder.Data;
using CodeImp.DoomBuilder.Config;

#endregion

namespace CodeImp.DoomBuilder.Interface
{
	internal partial class ConfigForm : DelayedForm
	{
		// Constructor
		public ConfigForm()
		{
			ListViewItem lvi;
			
			// Initialize
			InitializeComponent();

			// Make list column header full width
			columnname.Width = listconfigs.ClientRectangle.Width - SystemInformation.VerticalScrollBarWidth - 2;
			
			// Fill list of configurations
			foreach(ConfigurationInfo ci in General.Configs)
			{
				// Add a copy
				lvi = listconfigs.Items.Add(ci.Name);
				lvi.Tag = ci.Clone();
			}

			// TODO: Save and test nodebuilders are allowed to be empty
			
			// Fill comboboxes with nodebuilders
			nodebuildersave.Items.AddRange(General.Nodebuilders.ToArray());
			nodebuildertest.Items.AddRange(General.Nodebuilders.ToArray());
		}

		// Configuration item selected
		private void listconfigs_SelectedIndexChanged(object sender, EventArgs e)
		{
			ConfigurationInfo ci;
			NodebuilderInfo ni;
			
			// Item selected?
			if(listconfigs.SelectedItems.Count > 0)
			{
				// Enable panels
				tabs.Enabled = true;
				
				// Get config info of selected item
				ci = listconfigs.SelectedItems[0].Tag as ConfigurationInfo;

				// Fill resources list
				configdata.EditResourceLocationList(ci.Resources);

				// Go for all nodebuilder save items
				nodebuildersave.SelectedIndex = -1;
				for(int i = 0; i < nodebuildersave.Items.Count; i++)
				{
					// Get item
					ni = nodebuildersave.Items[i] as NodebuilderInfo;
					
					// Item matches configuration setting?
					if(string.Compare(ni.Name, ci.NodebuilderSave, false) == 0)
					{
						// Select this item
						nodebuildersave.SelectedIndex = i;
						break;
					}
				}

				// Go for all nodebuilder save items
				nodebuildertest.SelectedIndex = -1;
				for(int i = 0; i < nodebuildertest.Items.Count; i++)
				{
					// Get item
					ni = nodebuildertest.Items[i] as NodebuilderInfo;
					
					// Item matches configuration setting?
					if(string.Compare(ni.Name, ci.NodebuilderTest, false) == 0)
					{
						// Select this item
						nodebuildertest.SelectedIndex = i;
						break;
					}
				}
				
				// Set test application and parameters
				testapplication.Text = ci.TestProgram;
				testparameters.Text = ci.TestParameters;
			}
		}

		// Key released
		private void listconfigs_KeyUp(object sender, KeyEventArgs e)
		{
			// Nothing selected?
			if(listconfigs.SelectedItems.Count == 0)
			{
				// Disable panels
				configdata.FixedResourceLocationList(new DataLocationList());
				configdata.EditResourceLocationList(new DataLocationList());
				nodebuildersave.SelectedIndex = -1;
				nodebuildertest.SelectedIndex = -1;
				testapplication.Text = "";
				testparameters.Text = "";
				tabs.Enabled = false;
			}
		}

		// Mouse released
		private void listconfigs_MouseUp(object sender, MouseEventArgs e)
		{
			listconfigs_KeyUp(sender, new KeyEventArgs(Keys.None));
		}
		
		// Resource locations changed
		private void resourcelocations_OnContentChanged()
		{
			ConfigurationInfo ci;
			
			// Leave when no configuration selected
			if(listconfigs.SelectedItems.Count == 0) return;
			
			// Apply to selected configuration
			ci = listconfigs.SelectedItems[0].Tag as ConfigurationInfo;
			ci.Resources.Clear();
			ci.Resources.AddRange(configdata.GetResources());
		}

		// Nodebuilder selection changed
		private void nodebuildersave_SelectedIndexChanged(object sender, EventArgs e)
		{
			ConfigurationInfo ci;

			// Leave when no configuration selected
			if(listconfigs.SelectedItems.Count == 0) return;

			// Apply to selected configuration
			ci = listconfigs.SelectedItems[0].Tag as ConfigurationInfo;
			if(nodebuildersave.SelectedItem != null)
				ci.NodebuilderSave = (nodebuildersave.SelectedItem as NodebuilderInfo).Name;
		}

		// Nodebuilder selection changed
		private void nodebuildertest_SelectedIndexChanged(object sender, EventArgs e)
		{
			ConfigurationInfo ci;

			// Leave when no configuration selected
			if(listconfigs.SelectedItems.Count == 0) return;

			// Apply to selected configuration
			ci = listconfigs.SelectedItems[0].Tag as ConfigurationInfo;
			if(nodebuildertest.SelectedItem != null)
				ci.NodebuilderTest = (nodebuildertest.SelectedItem as NodebuilderInfo).Name;
		}
		
		// Test application changed
		private void testapplication_TextChanged(object sender, EventArgs e)
		{
			ConfigurationInfo ci;

			// Leave when no configuration selected
			if(listconfigs.SelectedItems.Count == 0) return;

			// Apply to selected configuration
			ci = listconfigs.SelectedItems[0].Tag as ConfigurationInfo;
			ci.TestProgram = testapplication.Text;
		}

		// Test parameters changed
		private void testparameters_TextChanged(object sender, EventArgs e)
		{
			ConfigurationInfo ci;

			// Leave when no configuration selected
			if(listconfigs.SelectedItems.Count == 0) return;

			// Apply to selected configuration
			ci = listconfigs.SelectedItems[0].Tag as ConfigurationInfo;
			ci.TestParameters = testparameters.Text;
		}
		
		// OK clicked
		private void apply_Click(object sender, EventArgs e)
		{
			ConfigurationInfo ci;
			
			// Apply configuration items
			foreach(ListViewItem lvi in listconfigs.Items)
			{
				// Get configuration item
				ci = lvi.Tag as ConfigurationInfo;
				
				// Find same configuration info in originals
				foreach(ConfigurationInfo oci in General.Configs)
				{
					// Apply settings when they match
					if(string.Compare(ci.Filename, oci.Filename) == 0) oci.Apply(ci);
				}
			}
			
			// Close
			this.DialogResult = DialogResult.OK;
			this.Close();
		}

		// Cancel clicked
		private void cancel_Click(object sender, EventArgs e)
		{
			// Close
			this.DialogResult = DialogResult.Cancel;
			this.Close();
		}

		// Browse clicked
		private void browsewad_Click(object sender, EventArgs e)
		{

		}
	}
}