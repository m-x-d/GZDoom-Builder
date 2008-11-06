
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
				item = new ToolStripMenuItem(cfg.Description);
				//item.Image = buttonnew.Image;
				item.Tag = cfg;
				item.Click += new EventHandler(buttonnew_Click);
				buttonnew.DropDownItems.Add(item);
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
			openfile.Filter = "Script files|" + filterall + "|" + filterseperate;
		}
		
		#endregion
		
		#region ================== Methods
		
		#endregion
		
		#region ================== Events
		
		// When new script is clicked
		private void buttonnew_Click(object sender, EventArgs e)
		{
			// Get the script config to use
			ScriptConfiguration scriptconfig = ((sender as ToolStripMenuItem).Tag as ScriptConfiguration);
			
			// Create new document
			ScriptFileDocumentTab t = new ScriptFileDocumentTab(scriptconfig);
			tabs.TabPages.Add(t);
			tabs.SelectedTab = t;
			
			// Focus to script editor
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
					
					// Focus to script editor
					t.Focus();
				}
			}
		}
		
		#endregion
	}
}
