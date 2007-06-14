using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using CodeImp.DoomBuilder.Map;
using System.IO;

namespace CodeImp.DoomBuilder.Interface
{
	internal partial class ResourceOptionsForm : Form
	{
		// Variables
		private ResourceLocation res;
		
		// Properties
		public ResourceLocation ResourceLocation { get { return res; } }
		
		// Constructor
		public ResourceOptionsForm(ResourceLocation settings)
		{
			// Initialize
			InitializeComponent();

			// Apply settings from ResourceLocation
			this.res = settings;
			switch(res.type)
			{
				// Setup for WAD File
				case ResourceLocation.RESOURCE_WAD:
					wadfiletab.Select();
					wadlocation.Text = res.location;
					break;

				// Setup for Directory
				case ResourceLocation.RESOURCE_DIRECTORY:
					directorytab.Select();
					dirlocation.Text = res.location;
					dir_textures.Checked = res.textures;
					dir_flats.Checked = res.flats;
					break;
			}
		}

		// OK clicked
		private void apply_Click(object sender, EventArgs e)
		{
			// Apply settings to ResourceLocation
			switch(tabs.SelectedIndex)
			{
				// Setup WAD File
				case ResourceLocation.RESOURCE_WAD:

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
						res.type = ResourceLocation.RESOURCE_WAD;
						res.location = wadlocation.Text;
						res.textures = false;
						res.flats = false;

						// Done
						this.DialogResult = DialogResult.OK;
						this.Hide();
					}
					break;

				// Setup Directory
				case ResourceLocation.RESOURCE_DIRECTORY:

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
						res.type = ResourceLocation.RESOURCE_WAD;
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
	}
}