
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
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Data;
using CodeImp.DoomBuilder.IO;
using System.IO;
using CodeImp.DoomBuilder.Config;
using CodeImp.DoomBuilder.Controls;

#endregion

namespace CodeImp.DoomBuilder.Windows
{
	internal partial class MapOptionsForm : DelayedForm
	{
		// Variables
		private MapOptions options;
		private bool newmap;
		
		// Properties
		public MapOptions Options { get { return options; } }
		
		// Constructor
		public MapOptionsForm(MapOptions options, bool newmap)
		{
			this.newmap = newmap;
			int index;
			
			// Initialize
			InitializeComponent();

			// Keep settings
			this.options = options;

			// Go for all configurations
			for(int i = 0; i < General.Configs.Count; i++)
			{
				// Add config name to list
				index = config.Items.Add(General.Configs[i]);

				//mxd. 
				if(newmap && !string.IsNullOrEmpty(General.Settings.LastUsedConfigName) && General.Configs[i].Name == General.Settings.LastUsedConfigName) 
				{
					// Select this item
					config.SelectedIndex = index;

				} // Is this configuration currently selected?
				else if(string.Compare(General.Configs[i].Filename, options.ConfigFile, true) == 0) // Is this configuration currently selected?
				{
					// Select this item
					config.SelectedIndex = index;
				}
			}

            //mxd. Still better than nothing :)
            if (config.SelectedIndex == -1 && General.Configs.Count > 0) config.SelectedIndex = 0;

			//mxd
			if(General.Map != null)
				datalocations.StartPath = General.Map.FilePathName;

			// Set the level name
            if (options.CurrentName.Length > 0) //mxd
			    levelname.Text = options.CurrentName;

			// Set strict patches loading
			strictpatches.Checked = options.StrictPatches;

			// Fill the resources list
			datalocations.EditResourceLocationList(options.Resources);
		}

		// OK clicked
		private void apply_Click(object sender, EventArgs e)
		{
			Configuration newcfg;
			WAD sourcewad;
			bool conflictingname;
			
			// Configuration selected?
			if(config.SelectedIndex == -1)
			{
				// Select a configuration!
				MessageBox.Show(this, "Please select a game configuration to use for editing your map.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
				config.Focus();
				return;
			}
			
			// Level name empty?
			if(levelname.Text.Length == 0)
			{
				// Enter a level name!
				MessageBox.Show(this, "Please enter a level name for your map.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
				levelname.Focus();
				return;
			}

			// Collect information
			ConfigurationInfo configinfo = General.Configs[config.SelectedIndex];
			DataLocationList locations = datalocations.GetResources();
			
			// When making a new map, check if we should warn the user for missing resources
			if(newmap)
			{
				General.Settings.LastUsedConfigName = configinfo.Name; //mxd
				
				if((locations.Count == 0) && (configinfo.Resources.Count == 0) && 
					MessageBox.Show(this, "You are about to make a map without selecting any resources. Textures, flats and " +
										 "sprites may not be shown correctly or may not show up at all. Do you want to continue?", Application.ProductName,
										 MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.No)
				{
					return;
				}
			}

			// Next checks are only for maps that are already opened
			if(!newmap)
			{
				// Now we check if the map name the user has given does already exist in the source WAD file
				// We have to warn the user about that, because it would create a level name conflict in the WAD

				// Level name changed and the map exists in a source wad?
				if((levelname.Text != options.CurrentName) && (General.Map != null) &&
				   (General.Map.FilePathName != "") && File.Exists(General.Map.FilePathName))
				{
					// Open the source wad file to check for conflicting name
					sourcewad = new WAD(General.Map.FilePathName, true);
					conflictingname = (sourcewad.FindLumpIndex(levelname.Text) > -1);
					sourcewad.Dispose();

					// Names conflict?
					if(conflictingname)
					{
						// Show warning!
						if(General.ShowWarningMessage("The map name \"" + levelname.Text + "\" is already in use by another map or data lump in the source WAD file. Saving your map with this name will cause conflicting data lumps in the WAD file. Do you want to continue?", MessageBoxButtons.YesNo, MessageBoxDefaultButton.Button2) == DialogResult.No)
						{
							return;
						}
					}
				}

				// When the user changed the configuration to one that has a different read/write interface,
				// we have to warn the user that the map may not be compatible.
				
				// Configuration changed?
				if((options.ConfigFile != "") && (General.Configs[config.SelectedIndex].Filename != options.ConfigFile))
				{
					// Load the new cfg file
					newcfg = General.LoadGameConfiguration(General.Configs[config.SelectedIndex].Filename);
					if(newcfg == null) return;

					// Check if the config uses a different IO interface
					if(newcfg.ReadSetting("formatinterface", "") != General.Map.Config.FormatInterface)
					{
						// Warn the user about IO interface change
						if(General.ShowWarningMessage("The game configuration you selected uses a different file format than your current map. Because your map was not designed for this format it may cause the map to work incorrectly in the game. Do you want to continue?", MessageBoxButtons.YesNo, MessageBoxDefaultButton.Button2) == DialogResult.No)
						{
							// Reset to old configuration
							for(int i = 0; i < config.Items.Count; i++)
							{
								// Is this configuration the old config?
								if(string.Compare(General.Configs[i].Filename, options.ConfigFile, true) == 0)
								{
									// Select this item
									config.SelectedIndex = i;
								}
							}
							return;
						}
					}
				}
			}
			
			// Apply changes
			options.ClearResources();
			options.ConfigFile = General.Configs[config.SelectedIndex].Filename;
			options.CurrentName = levelname.Text.Trim().ToUpper();
			options.StrictPatches = strictpatches.Checked;
			options.CopyResources(datalocations.GetResources());

			// Reset default drawing textures
			General.Settings.DefaultTexture = null;
			General.Settings.DefaultFloorTexture = null;
			General.Settings.DefaultCeilingTexture = null;
			
			// Hide window
			this.DialogResult = DialogResult.OK;
			this.Close();
		}

		// Cancel clicked
		private void cancel_Click(object sender, EventArgs e)
		{
			// Just hide window
			this.DialogResult = DialogResult.Cancel;
			this.Close();
		}

		// Game configuration chosen
		private void config_SelectedIndexChanged(object sender, EventArgs e)
		{
			ConfigurationInfo ci;
			
			// Anything selected?
			if(config.SelectedIndex > -1)
			{
				// Get the info
				ci = (ConfigurationInfo)config.SelectedItem;

				// No lump name in the name field?
                if (levelname.Text.Trim().Length == 0) 
                {
                    // Get default lump name from configuration
                    levelname.Text = ci.DefaultLumpName;
                }
				
				// Show resources
				datalocations.FixedResourceLocationList(ci.Resources);
			}
		}

		// When keys are pressed in the level name field
		private void levelname_KeyPress(object sender, KeyPressEventArgs e)
		{
			string allowedchars = Lump.MAP_LUMP_NAME_CHARS + Lump.MAP_LUMP_NAME_CHARS.ToLowerInvariant() + "\b";

			// Check if key is not allowed
			if(allowedchars.IndexOf(e.KeyChar) == -1)
			{
				// Cancel this
				e.Handled = true;
			}
		}

		// Help
		private void MapOptionsForm_HelpRequested(object sender, HelpEventArgs hlpevent)
		{
			General.ShowHelp("w_mapoptions.html");
			hlpevent.Handled = true;
		}
	}
}