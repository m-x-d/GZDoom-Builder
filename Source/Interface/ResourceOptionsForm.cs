
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
using System.IO;
using CodeImp.DoomBuilder.Data;

#endregion

namespace CodeImp.DoomBuilder.Interface
{
	internal partial class ResourceOptionsForm : DelayedForm
	{
		// Variables
		private DataLocation res;
		
		// Properties
		public DataLocation ResourceLocation { get { return res; } }
		
		// Constructor
		public ResourceOptionsForm(DataLocation settings, string caption)
		{
			// Initialize
			InitializeComponent();

			// Set caption
			this.Text = caption;
			
			// Apply settings from ResourceLocation
			this.res = settings;
			switch(res.type)
			{
				// Setup for WAD File
				case DataLocation.RESOURCE_WAD:
					wadfiletab.Select();
					wadlocation.Text = res.location;
					break;

				// Setup for Directory
				case DataLocation.RESOURCE_DIRECTORY:
					directorytab.Select();
					dirlocation.Text = res.location;
					dir_textures.Checked = res.textures;
					dir_flats.Checked = res.flats;
					break;
			}

			// Select appropriate tab
			tabs.SelectedIndex = res.type;
		}

		// OK clicked
		private void apply_Click(object sender, EventArgs e)
		{
			// Apply settings to ResourceLocation
			switch(tabs.SelectedIndex)
			{
				// Setup WAD File
				case DataLocation.RESOURCE_WAD:

					// Check if directory is specified
					if((wadlocation.Text.Length == 0) ||
					   (!File.Exists(wadlocation.Text)))
					{
						// No valid wad file specified
						MessageBox.Show(this, "Please select a valid WAD File resource.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
					}
					else
					{
						// Apply settings
						res.type = DataLocation.RESOURCE_WAD;
						res.location = wadlocation.Text;
						res.textures = false;
						res.flats = false;

						// Done
						this.DialogResult = DialogResult.OK;
						this.Hide();
					}
					break;

				// Setup Directory
				case DataLocation.RESOURCE_DIRECTORY:

					// Check if directory is specified
					if((dirlocation.Text.Length == 0) ||
					   (!Directory.Exists(dirlocation.Text)))
					{
						// No valid directory specified
						MessageBox.Show(this, "Please select a valid directory resource.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
					}
					// At least one of the checkboxes must be checked
					else if(!dir_flats.Checked && !dir_textures.Checked)
					{
						// Must select one of the checkboxes
						MessageBox.Show(this, "Please choose to load the images as texture or flats, or both.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
					}
					else
					{
						// Apply settings
						res.type = DataLocation.RESOURCE_DIRECTORY;
						res.location = dirlocation.Text;
						res.textures = dir_textures.Checked;
						res.flats = dir_flats.Checked;

						// Done
						this.DialogResult = DialogResult.OK;
						this.Hide();
					}
					break;
			}
		}

		// Cancel clicked
		private void cancel_Click(object sender, EventArgs e)
		{
			// Just hide
			this.DialogResult = DialogResult.Cancel;
			this.Hide();
		}

		// Browse WAD File clicked
		private void browsewad_Click(object sender, EventArgs e)
		{
			// Browse for WAD File
			if(wadfiledialog.ShowDialog(this) == DialogResult.OK)
			{
				// Use this file
				wadlocation.Text = wadfiledialog.FileName;
			}
		}

		// Browse Directory clicked
		private void browsedir_Click(object sender, EventArgs e)
		{
			// Browse for Directory
			if(dirdialog.ShowDialog(this) == DialogResult.OK)
			{
				// Use this directory
				dirlocation.Text = dirdialog.SelectedPath;
			}
		}
	}
}