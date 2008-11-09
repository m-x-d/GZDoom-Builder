
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
			ToolStripMenuItem item;
			
			InitializeComponent();
			
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

			// Done
			UpdateToolbar();
		}
		
		#endregion
		
		#region ================== Methods

		// This updates the toolbar for the current status
		private void UpdateToolbar()
		{
			int numscriptsopen = tabs.TabPages.Count;
			ScriptDocumentTab t = null;
			
			// Get current script, if any are open
			if(numscriptsopen > 0)
				t = (tabs.SelectedTab as ScriptDocumentTab);
			
			// Enable/disable buttons
			buttonsave.Enabled = (t != null) && t.ExplicitSave;
			buttonsaveall.Enabled = (numscriptsopen > 0);
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
			t.Focus();
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
			t.Focus();
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
					t.Focus();
				}
			}
		}

		// Save script clicked
		private void buttonsave_Click(object sender, EventArgs e)
		{
			// Save the current script
			ScriptDocumentTab t = (tabs.SelectedTab as ScriptDocumentTab);
			SaveScript(t);
		}

		// Save All clicked
		private void buttonsaveall_Click(object sender, EventArgs e)
		{
			// Save all scripts
			foreach(ScriptDocumentTab t in tabs.TabPages)
			{
				if(!SaveScript(t)) break;
			}
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
			if(t.IsChanged)
			{
				// Ask to save
				DialogResult result = MessageBox.Show(this.ParentForm, "Do you want to save changes to " + t.Text + "?", "Close File", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
				if(result == DialogResult.Yes)
				{
					// Save file
					if(!SaveScript(t)) return;
				}
				else if(result == DialogResult.Cancel)
				{
					// Cancel
					return;
				}
			}
			
			// Close file
			tabs.TabPages.Remove(t);
			t.Dispose();
		}

		// Undo clicked
		private void buttonundo_Click(object sender, EventArgs e)
		{
			ScriptDocumentTab t = (tabs.SelectedTab as ScriptDocumentTab);
			t.Undo();
		}

		// Redo clicked
		private void buttonredo_Click(object sender, EventArgs e)
		{
			ScriptDocumentTab t = (tabs.SelectedTab as ScriptDocumentTab);
			t.Redo();
		}

		// Cut clicked
		private void buttoncut_Click(object sender, EventArgs e)
		{
			ScriptDocumentTab t = (tabs.SelectedTab as ScriptDocumentTab);
			t.Cut();
		}

		// Copy clicked
		private void buttoncopy_Click(object sender, EventArgs e)
		{
			ScriptDocumentTab t = (tabs.SelectedTab as ScriptDocumentTab);
			t.Copy();
		}

		// Paste clicked
		private void buttonpaste_Click(object sender, EventArgs e)
		{
			ScriptDocumentTab t = (tabs.SelectedTab as ScriptDocumentTab);
			t.Paste();
		}
		
		#endregion
	}
}
