
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
using CodeImp.DoomBuilder.Data;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Config;
using CodeImp.DoomBuilder.Types;
using CodeImp.DoomBuilder.IO;
using System.Globalization;

#endregion

namespace CodeImp.DoomBuilder.Controls
{
	internal partial class ScriptEditorPanel : UserControl
	{
		#region ================== Constants
		
		#endregion
		
		#region ================== Variables
		
		private List<ScriptConfiguration> scriptconfigs;
		
		#endregion
		
		#region ================== Properties
		
		#endregion
		
		#region ================== Constructor
		
		// Constructor
		public ScriptEditorPanel()
		{
			InitializeComponent();
		}
		
		// This initializes the control
		public void Initialize()
		{
			ToolStripMenuItem item;
			
			// Make list of script configs
			scriptconfigs = new List<ScriptConfiguration>(General.ScriptConfigs.Values);
			scriptconfigs.Add(new ScriptConfiguration());
			scriptconfigs.Sort();
			
			// Fill the list of new document types
			foreach(ScriptConfiguration cfg in scriptconfigs)
			{
				// Button for new script menu
				item = new ToolStripMenuItem(cfg.Description);
				//item.Image = buttonnew.Image;
				item.Tag = cfg;
				item.Click += new EventHandler(buttonnew_Click);
				buttonnew.DropDownItems.Add(item);
				
				// Button for script type menu
				item = new ToolStripMenuItem(cfg.Description);
				//item.Image = buttonnew.Image;
				item.Tag = cfg;
				item.Click += new EventHandler(buttonscriptconfig_Click);
				buttonscriptconfig.DropDownItems.Add(item);
			}
			
			// Setup supported extensions
			string filterall = "";
			string filterseperate = "";
			foreach(ScriptConfiguration cfg in scriptconfigs)
			{
				if(cfg.Extensions.Length > 0)
				{
					string exts = "*." + string.Join(";*.", cfg.Extensions);
					if(filterseperate.Length > 0) filterseperate += "|";
					filterseperate += cfg.Description + "|" + exts;
					if(filterall.Length > 0) filterall += ";";
					filterall += exts;
				}
			}
			openfile.Filter = "Script files|" + filterall + "|" + filterseperate + "|All files|*.*";
			
			// Load the script lumps
			foreach(MapLumpInfo maplumpinfo in General.Map.Config.MapLumps.Values)
			{
				// Is this a script lump?
				if(maplumpinfo.script != null)
				{
					// Load this!
					ScriptLumpDocumentTab t = new ScriptLumpDocumentTab(maplumpinfo.name, maplumpinfo.script);
					tabs.TabPages.Add(t);
					tabs.SelectedIndex = 0;
				}
			}
			
			// Done
			UpdateToolbar();
		}
		
		#endregion
		
		#region ================== Methods
		
		// This asks to save files and returns the result
		public bool AskSaveAll()
		{
			foreach(ScriptDocumentTab t in tabs.TabPages)
			{
				if(t.ExplicitSave)
				{
					if(!CloseScript(t, true)) return false;
				}
			}
			
			return true;
		}
		
		// This closes a script and returns true when closed
		private bool CloseScript(ScriptDocumentTab t, bool saveonly)
		{
			if(t.IsChanged)
			{
				// Ask to save
				DialogResult result = MessageBox.Show(this.ParentForm, "Do you want to save changes to " + t.Text + "?", "Close File", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
				if(result == DialogResult.Yes)
				{
					// Save file
					if(!SaveScript(t)) return false;
				}
				else if(result == DialogResult.Cancel)
				{
					// Cancel
					return false;
				}
			}
			
			if(!saveonly)
			{
				// Close file
				tabs.TabPages.Remove(t);
				t.Dispose();
			}
			return true;
		}
		
		// This returns true when any of the implicit-save scripts are changed
		public bool CheckImplicitChanges()
		{
			bool changes = false;
			foreach(ScriptDocumentTab t in tabs.TabPages)
			{
				if(!t.ExplicitSave && t.IsChanged) changes = true;
			}
			return changes;
		}
		
		// This forces the focus to the script editor
		public void ForceFocus()
		{
			ScriptDocumentTab t = (tabs.SelectedTab as ScriptDocumentTab);
			tabs.Focus();
			if(t != null) t.Focus();
		}
		
		// This does an implicit save on all documents that use implicit saving
		// Call this to save the lumps before disposing the panel!
		public void ImplicitSave()
		{
			// Save all scripts
			foreach(ScriptDocumentTab t in tabs.TabPages)
			{
				if(!t.ExplicitSave) t.Save();
			}
			
			UpdateToolbar();
		}
		
		// This updates the toolbar for the current status
		private void UpdateToolbar()
		{
			int numscriptsopen = tabs.TabPages.Count;
			int explicitsavescripts = 0;
			ScriptDocumentTab t = null;
			
			// Any explicit save scripts?
			foreach(ScriptDocumentTab dt in tabs.TabPages)
				if(dt.ExplicitSave) explicitsavescripts++;
			
			// Get current script, if any are open
			if(numscriptsopen > 0)
				t = (tabs.SelectedTab as ScriptDocumentTab);
			
			// Enable/disable buttons
			buttonsave.Enabled = (t != null) && t.ExplicitSave;
			buttonsaveall.Enabled = (explicitsavescripts > 0);
			buttoncompile.Enabled = (t != null) && (t.Config.Compiler != null);
			buttonscriptconfig.Enabled = (t != null) && t.IsReconfigurable;
			buttonundo.Enabled = (t != null);
			buttonredo.Enabled = (t != null);
			buttoncopy.Enabled = (t != null);
			buttoncut.Enabled = (t != null);
			buttonpaste.Enabled = (t != null);
			buttonclose.Enabled = (t != null) && t.IsClosable;
			
			if(t != null)
			{
				// Check the according script config in menu
				foreach(ToolStripMenuItem item in buttonscriptconfig.DropDownItems)
				{
					ScriptConfiguration config = (item.Tag as ScriptConfiguration);
					item.Checked = (config == t.Config);
				}
				
				// Focus to script editor
				ForceFocus();
			}
		}
		
		#endregion
		
		#region ================== Events
		
		// When the user changes the script configuration
		private void buttonscriptconfig_Click(object sender, EventArgs e)
		{
			// Get the tab and new script config
			ScriptDocumentTab t = (tabs.SelectedTab as ScriptDocumentTab);
			ScriptConfiguration scriptconfig = ((sender as ToolStripMenuItem).Tag as ScriptConfiguration);
			
			// Change script config
			t.ChangeScriptConfig(scriptconfig);

			// Done
			UpdateToolbar();
		}
		
		// When new script is clicked
		private void buttonnew_Click(object sender, EventArgs e)
		{
			// Get the script config to use
			ScriptConfiguration scriptconfig = ((sender as ToolStripMenuItem).Tag as ScriptConfiguration);
			
			// Create new document
			ScriptFileDocumentTab t = new ScriptFileDocumentTab(scriptconfig);
			tabs.TabPages.Add(t);
			tabs.SelectedTab = t;
			
			// Done
			UpdateToolbar();
		}
		
		// Open script clicked
		private void buttonopen_Click(object sender, EventArgs e)
		{
			// Show open file dialog
			if(openfile.ShowDialog(this.ParentForm) == DialogResult.OK)
			{
				ScriptConfiguration foundconfig = new ScriptConfiguration();
				
				// Find the most suitable script configuration to use
				foreach(ScriptConfiguration cfg in scriptconfigs)
				{
					foreach(string ext in cfg.Extensions)
					{
						// Use this configuration if the extension matches
						if(openfile.FileName.EndsWith("." + ext, true, CultureInfo.InvariantCulture))
						{
							foundconfig = cfg;
							break;
						}
					}
				}
				
				// Create new document
				ScriptFileDocumentTab t = new ScriptFileDocumentTab(foundconfig);
				if(t.Open(openfile.FileName))
				{
					// Add to tabs
					tabs.TabPages.Add(t);
					tabs.SelectedTab = t;
					
					// Done
					UpdateToolbar();
				}
			}
		}

		// Save script clicked
		private void buttonsave_Click(object sender, EventArgs e)
		{
			// Save the current script
			ScriptDocumentTab t = (tabs.SelectedTab as ScriptDocumentTab);
			SaveScript(t);
			UpdateToolbar();
		}

		// Save All clicked
		private void buttonsaveall_Click(object sender, EventArgs e)
		{
			// Save all scripts
			foreach(ScriptDocumentTab t in tabs.TabPages)
			{
				// Use explicit save for this script?
				if(t.ExplicitSave)
				{
					if(!SaveScript(t)) break;
				}
			}
			
			UpdateToolbar();
		}

		// This is called by Save and Save All to save a script
		// Returns false when cancelled by the user
		private bool SaveScript(ScriptDocumentTab t)
		{
			// Do we have to do a save as?
			if(t.IsSaveAsRequired)
			{
				// Setup save dialog
				string scriptfilter = t.Config.Description + "|*." + string.Join(";*.", t.Config.Extensions);
				savefile.Filter = scriptfilter + "|All files|*.*";
				if(savefile.ShowDialog(this.ParentForm) == DialogResult.OK)
				{
					// Save to new filename
					t.SaveAs(savefile.FileName);
					return true;
				}
				else
				{
					// Cancelled
					return false;
				}
			}
			else
			{
				// Save to same filename
				t.Save();
				return true;
			}
		}

		// A tab is selected
		private void tabs_Selecting(object sender, TabControlCancelEventArgs e)
		{
			UpdateToolbar();
		}
		
		// This closes the current file
		private void buttonclose_Click(object sender, EventArgs e)
		{
			ScriptDocumentTab t = (tabs.SelectedTab as ScriptDocumentTab);
			CloseScript(t, false);
			UpdateToolbar();
		}

		// Undo clicked
		private void buttonundo_Click(object sender, EventArgs e)
		{
			ScriptDocumentTab t = (tabs.SelectedTab as ScriptDocumentTab);
			t.Undo();
			UpdateToolbar();
		}

		// Redo clicked
		private void buttonredo_Click(object sender, EventArgs e)
		{
			ScriptDocumentTab t = (tabs.SelectedTab as ScriptDocumentTab);
			t.Redo();
			UpdateToolbar();
		}

		// Cut clicked
		private void buttoncut_Click(object sender, EventArgs e)
		{
			ScriptDocumentTab t = (tabs.SelectedTab as ScriptDocumentTab);
			t.Cut();
			UpdateToolbar();
		}

		// Copy clicked
		private void buttoncopy_Click(object sender, EventArgs e)
		{
			ScriptDocumentTab t = (tabs.SelectedTab as ScriptDocumentTab);
			t.Copy();
			UpdateToolbar();
		}

		// Paste clicked
		private void buttonpaste_Click(object sender, EventArgs e)
		{
			ScriptDocumentTab t = (tabs.SelectedTab as ScriptDocumentTab);
			t.Paste();
			UpdateToolbar();
		}
		
		// Mouse released on tabs
		private void tabs_MouseUp(object sender, MouseEventArgs e)
		{
			ForceFocus();
		}
		
		#endregion
	}
}
