
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
using System.Windows.Forms;
using CodeImp.DoomBuilder.IO;
using CodeImp.DoomBuilder.Map;
using System.IO;
using System.Collections;
using CodeImp.DoomBuilder.Data;
using CodeImp.DoomBuilder.Config;

#endregion

namespace CodeImp.DoomBuilder.Windows
{
	internal partial class OpenMapOptionsForm : DelayedForm
	{
		// Variables
		private Configuration mapsettings;
		private MapOptions options;
		private WAD wadfile;
		private readonly string filepathname;
		private string selectedmapname;
		
		// Properties
		public string FilePathName { get { return filepathname; } }
		public MapOptions Options { get { return options; } }
		
		// Constructor
		public OpenMapOptionsForm(string filepathname)
		{
			// Initialize
			InitializeComponent();
			this.Text = "Open Map from " + Path.GetFileName(filepathname);
			this.filepathname = filepathname;
			datalocations.StartPath = filepathname; //mxd
			this.options = null;
		}

		// Constructor
		public OpenMapOptionsForm(string filepathname, MapOptions options)
		{
			// Initialize
			InitializeComponent();
			this.Text = "Open Map from " + Path.GetFileName(filepathname);
			this.filepathname = filepathname;
			this.options = options;
			datalocations.StartPath = filepathname; //mxd
			datalocations.EditResourceLocationList(options.Resources);
		}

		// This loads the settings and attempt to find a suitable config
		private void LoadSettings()
		{
			string gameconfig;
			int index;
			
			// Busy
			Cursor.Current = Cursors.WaitCursor;

			// Check if the file exists
			if(!File.Exists(filepathname))
			{
				// WAD file does not exist
				MessageBox.Show(this, "Could not open the WAD file: The file does not exist.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
				this.DialogResult = DialogResult.Cancel;
				this.Close();
				return;
			}
			
			try
			{
				// Open the WAD file
				wadfile = new WAD(filepathname, true);
			}
			catch(Exception)
			{
				// Unable to open WAD file (or its config)
				MessageBox.Show(this, "Could not open the WAD file for reading. Please make sure the file you selected is valid and is not in use by any other application.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
				if(wadfile != null) wadfile.Dispose();
				this.DialogResult = DialogResult.Cancel;
				this.Close();
				return;
			}

			// Open the Map Settings configuration
			string dbsfile = filepathname.Substring(0, filepathname.Length - 4) + ".dbs";
			if(File.Exists(dbsfile))
				try { mapsettings = new Configuration(dbsfile, true); }
				catch(Exception) { mapsettings = new Configuration(true); }
			else
				mapsettings = new Configuration(true);
			
			// Check strict patches box, check what game configuration is preferred
			if (options != null) 
			{
				strictpatches.Checked = options.StrictPatches;
				gameconfig = options.ConfigFile;
			} 
			else 
			{
				strictpatches.Checked = mapsettings.ReadSetting("strictpatches", false);
				gameconfig = mapsettings.ReadSetting("gameconfig", "");
			}

			//mxd. Fill script compilers list
			foreach(KeyValuePair<string, ScriptConfiguration> group in General.CompiledScriptConfigs) 
			{
				scriptcompiler.Items.Add(group.Value);
			}

			//mxd. Go for all enabled configurations
			for(int i = 0; i < General.Configs.Count; i++) 
			{
				if(!General.Configs[i].Enabled) continue;
				
				// Add config name to list
				index = config.Items.Add(General.Configs[i]);

				// Select this item
				if(General.Configs[i].Filename == gameconfig) config.SelectedIndex = index;
			}

			//mxd. No dice? Try disabled ones
			if(config.SelectedIndex == -1) 
			{
				for(int i = 0; i < General.Configs.Count; i++) 
				{
					if(General.Configs[i].Enabled) continue;

					if(General.Configs[i].Filename == gameconfig) 
					{
						//add and Select this item
						config.SelectedIndex = config.Items.Add(General.Configs[i]);
						break;
					}
				}
			}

			// Still no configuration selected?
			if(config.SelectedIndex == -1) 
			{
				// Then go for all configurations to find a suitable one
				for(int i = 0; i < General.Configs.Count; i++)
				{
					// Check if a resource location is set for this configuration, if so, match the wad against this configuration
					if(General.Configs[i].Resources.Count > 0 && MatchConfiguration(General.Configs[i].Configuration, wadfile))
					{
						//mxd. Already added?
						index = config.Items.IndexOf(General.Configs[i]);
						if (index != -1) 
						{
							// Select this item
							config.SelectedIndex = index;
						} 
						else 
						{
							// Add and select this item
							config.SelectedIndex = config.Items.Add(General.Configs[i]);
						}
						break;
					}
				}
			}
			
			// Done
			Cursor.Current = Cursors.Default;
		}
		
		// mxd. This matches a WAD file with the specified game configuration
		// by checking if the specific lumps are detected
		private static bool MatchConfiguration(Configuration cfg, WAD wadfile) 
		{
			int scanindex, checkoffset;
			int lumpsfound, lumpsrequired = 0;
			string lumpname;

			// Get the map lump names
			IDictionary maplumpnames = cfg.ReadSetting("maplumpnames", new Hashtable());

			// Count how many required lumps we have to find
			foreach(DictionaryEntry ml in maplumpnames) 
			{
				// Ignore the map header (it will not be found because the name is different)
				if(ml.Key.ToString() != MapManager.CONFIG_MAP_HEADER) 
				{
					// Read lump setting and count it
					if(cfg.ReadSetting("maplumpnames." + ml.Key + ".required", false))
						lumpsrequired++;
				}
			}

			// Go for all the lumps in the wad
			for(scanindex = 0; scanindex < (wadfile.Lumps.Count - 1); scanindex++) 
			{
				// Make sure this lump is not part of the map.
				if(!maplumpnames.Contains(wadfile.Lumps[scanindex].Name)) 
				{
					// Reset check
					lumpsfound = 0;
					checkoffset = 1;

					// Continue while still within bounds and lumps are still recognized
					while(((scanindex + checkoffset) < wadfile.Lumps.Count) &&
						  maplumpnames.Contains(wadfile.Lumps[scanindex + checkoffset].Name)) 
					{
						lumpname = wadfile.Lumps[scanindex + checkoffset].Name;
						//mxd. Lump cannot present in current map format, fail this check
						if(cfg.ReadSetting("maplumpnames." + lumpname + ".forbidden", false)) 
						{
							lumpsfound = -1;
							break;
						}

						// Count the lump when it is marked as required
						if(cfg.ReadSetting("maplumpnames." + lumpname + ".required", false))
							lumpsfound++;

						// Check the next lump
						checkoffset++;
					}

					// Map found? Let's call it a day :)
					if (lumpsfound >= lumpsrequired) return true;
				}
			}

			return false;
		}

		// Configuration is selected
		private void config_SelectedIndexChanged(object sender, EventArgs e)
		{
			// Anything selected?
			if(config.SelectedIndex < 0) return;

			int scanindex, checkoffset;
			int lumpsfound, lumpsrequired = 0;
			string lumpname, selectedname = "";

			// Keep selected name, if any
			if(mapslist.SelectedItems.Count > 0)
				selectedname = mapslist.SelectedItems[0].Text;

			// Make an array for the map names
			List<ListViewItem> mapnames = new List<ListViewItem>();

			// Get selected configuration info
			ConfigurationInfo ci = (config.SelectedItem as ConfigurationInfo);

			//mxd. Get configuration
			Configuration cfg = ci.Configuration;

			// Get the map lump names
			IDictionary maplumpnames = cfg.ReadSetting("maplumpnames", new Hashtable());

			// Count how many required lumps we have to find
			foreach(DictionaryEntry ml in maplumpnames) 
			{
				// Ignore the map header (it will not be found because the name is different)
				if(ml.Key.ToString() != MapManager.CONFIG_MAP_HEADER) 
				{
					// Read lump setting and count it
					if(cfg.ReadSetting("maplumpnames." + ml.Key + ".required", false))
						lumpsrequired++;
				}
			}

			// Go for all the lumps in the wad
			for(scanindex = 0; scanindex < (wadfile.Lumps.Count - 1); scanindex++) 
			{
				// Make sure this lump is not part of the map.
				if(!maplumpnames.Contains(wadfile.Lumps[scanindex].Name)) 
				{
					// Reset check
					lumpsfound = 0;
					checkoffset = 1;

					// Continue while still within bounds and lumps are still recognized
					while(((scanindex + checkoffset) < wadfile.Lumps.Count) &&
						  maplumpnames.Contains(wadfile.Lumps[scanindex + checkoffset].Name)) 
					{
						lumpname = wadfile.Lumps[scanindex + checkoffset].Name;
						//mxd. Lump cannot present in current map format, fail this check
						if(cfg.ReadSetting("maplumpnames." + lumpname + ".forbidden", false)) 
						{
							lumpsfound = -1;
							break;
						}

						// Count the lump when it is marked as required
						if(cfg.ReadSetting("maplumpnames." + lumpname + ".required", false))
							lumpsfound++;

						// Check the next lump
						checkoffset++;
					}

					// Map found? Then add it to the list
					if(lumpsfound >= lumpsrequired)
						mapnames.Add(new ListViewItem(wadfile.Lumps[scanindex].Name));
				}
			}

			// Clear the list and add the new map names
			mapslist.BeginUpdate();
			mapslist.Items.Clear();
			mapslist.Items.AddRange(mapnames.ToArray());
			mapslist.Sort();

			// Go for all items in the list
			foreach(ListViewItem item in mapslist.Items) 
			{
				// Was this item previously selected?
				if(item.Text == selectedname) 
				{
					// Select it again
					item.Selected = true;
					break;
				}
			}
			if((mapslist.SelectedItems.Count == 0) && (mapslist.Items.Count > 0))
				mapslist.Items[0].Selected = true;
			mapslist.EndUpdate();

			//mxd. Disable script compiler selector when there are no maps detected using current configuration
			if (mapslist.Items.Count == 0) 
			{
				scriptcompiler.Enabled = false;
				scriptcompiler.SelectedIndex = -1;
				scriptcompilerlabel.Enabled = false;
			}

			// Show configuration resources
			datalocations.FixedResourceLocationList(ci.Resources);

			// Update long texture names checkbox (mxd)
			longtexturenames.Enabled = cfg.ReadSetting("longtexturenames", false);
			longtexturenames.Checked = longtexturenames.Enabled && General.Settings.ReadSetting("browserwindow.uselongtexturenames", false);
		}
		
		// OK clicked
		private void apply_Click(object sender, EventArgs e)
		{
			// Configuration selected?
			if(config.SelectedIndex == -1)
			{
				// Select a configuration!
				MessageBox.Show(this, "Please select a game configuration to use for editing your map.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
				config.Focus();
				return;
			}
			
			// Collect information
			ConfigurationInfo configinfo = (config.SelectedItem as ConfigurationInfo); //mxd
			DataLocationList locations = datalocations.GetResources();

			// Resources are valid? (mxd)
			if (!datalocations.ResourcesAreValid())
			{
				MessageBox.Show(this, "Cannot open map: at least one resource doesn't exist!", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
				datalocations.Focus();
				return;
			}
			
			// No map selected?
			if(mapslist.SelectedItems.Count == 0)
			{
				// Choose a map!
				MessageBox.Show(this, "Please select a map to load for editing.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
				mapslist.Focus();
				return;
			}
			
			// Check if we should warn the user for missing resources
			if((wadfile.Type != WAD.TYPE_IWAD) && (locations.Count == 0) && (configinfo.Resources.Count == 0))
			{
				if(MessageBox.Show(this, "You are about to load a map without selecting any resources. Textures, flats and " +
										 "sprites may not be shown correctly or may not show up at all. Do you want to continue?", Application.ProductName,
										 MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.No)
				{
					return;
				}
			}
			
			// Apply changes
			options.ClearResources();
			options.ConfigFile = configinfo.Filename;
			options.CurrentName = mapslist.SelectedItems[0].Text;
			options.StrictPatches = strictpatches.Checked;
			options.CopyResources(locations);

			//mxd. Store script compiler
			if(scriptcompiler.Enabled && scriptcompiler.SelectedIndex > -1) 
			{
				ScriptConfiguration scriptcfg = scriptcompiler.SelectedItem as ScriptConfiguration;

				foreach(KeyValuePair<string, ScriptConfiguration> group in General.CompiledScriptConfigs) 
				{
					if(group.Value == scriptcfg) 
					{
						options.ScriptCompiler = group.Key;
						break;
					}
				}
			}

			//mxd. Use long texture names?
			if (longtexturenames.Enabled) General.Settings.WriteSetting("browserwindow.uselongtexturenames", longtexturenames.Checked);

			// Hide window
			wadfile.Dispose();
			this.DialogResult = DialogResult.OK;
			this.Close();
		}
		
		// Cancel clicked
		private void cancel_Click(object sender, EventArgs e)
		{
			// Just hide window
			wadfile.Dispose();
			this.DialogResult = DialogResult.Cancel;
			this.Close();
		}

		// Window is shown
		private void OpenMapOptionsForm_Shown(object sender, EventArgs e)
		{
			// Update window
			this.Update();
			
			// Load settings
			LoadSettings();
		}

		// Map name doubleclicked
		private void mapslist_DoubleClick(object sender, EventArgs e)
		{
			// Click OK
			if(mapslist.SelectedItems.Count > 0) apply.PerformClick();
		}

		// Map name selected
		private void mapslist_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
		{
			if(!e.IsSelected) return; //mxd. Don't want to trigger this twice
			
			DataLocationList locations;
			DataLocationList listedlocations;
			string scriptconfig = string.Empty;
			
			// Map previously selected?
			if(!string.IsNullOrEmpty(selectedmapname))
			{
				// Get locations from previous selected map settings
				locations = new DataLocationList(mapsettings, "maps." + selectedmapname + ".resources");
				listedlocations = datalocations.GetResources();
				
				// Remove data locations that this map has in its config
				foreach(DataLocation dl in locations)
					listedlocations.Remove(dl);

				// Set new data locations
				datalocations.EditResourceLocationList(listedlocations);

				// Done
				selectedmapname = null;
			}
			
			// Anything selected?
			if(mapslist.SelectedItems.Count > 0)
			{
				// Get the map name
				selectedmapname = mapslist.SelectedItems[0].Text;
				options = new MapOptions(mapsettings, selectedmapname);
				
				// Get locations from previous selected map settings
				locations = new DataLocationList(mapsettings, "maps." + selectedmapname + ".resources");
				listedlocations = datalocations.GetResources();

				// Add data locations that this map has in its config
				foreach(DataLocation dl in locations)
					if(!listedlocations.Contains(dl)) listedlocations.Add(dl);

				// Set new data locations
				datalocations.EditResourceLocationList(listedlocations);

				//mxd. Select script compiler
				if (!string.IsNullOrEmpty(options.ScriptCompiler) && General.CompiledScriptConfigs.ContainsKey(options.ScriptCompiler)) 
				{
					scriptconfig = options.ScriptCompiler;
				} 
				else 
				{
					string defaultscriptconfig = (config.SelectedItem as ConfigurationInfo).Configuration.ReadSetting("defaultscriptcompiler", string.Empty);
					if(!string.IsNullOrEmpty(defaultscriptconfig) && General.CompiledScriptConfigs.ContainsKey(defaultscriptconfig))
						scriptconfig = defaultscriptconfig;
				}
			}

			//mxd. Select proper script compiler
			if (!string.IsNullOrEmpty(scriptconfig)) 
			{
				scriptcompiler.Enabled = true;
				scriptcompiler.SelectedItem = General.CompiledScriptConfigs[scriptconfig];
				scriptcompilerlabel.Enabled = true;
			} 
			else 
			{
				scriptcompiler.Enabled = false;
				scriptcompiler.SelectedIndex = -1;
				scriptcompilerlabel.Enabled = false;
			}
		}

		// Help
		private void OpenMapOptionsForm_HelpRequested(object sender, HelpEventArgs hlpevent)
		{
			General.ShowHelp("w_openmapoptions.html");
			hlpevent.Handled = true;
		}
	}
}