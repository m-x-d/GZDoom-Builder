using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using CodeImp.DoomBuilder.Map;

namespace CodeImp.DoomBuilder.Interface
{
	internal partial class MapOptionsForm : Form
	{
		// Variables
		private MapOptions options;
		
		// Properties
		public MapOptions Options { get { return options; } }
		
		// Constructor
		public MapOptionsForm(MapOptions options)
		{
			// Initialize
			InitializeComponent();

			// Keep settings
			this.options = options;

			// Go for all configurations
			for(int i = 0; i < General.Configs.Count; i++)
			{
				// Add config name to list
				config.Items.Add(General.Configs[i].name);

				// Is this configuration currently selected?
				if(string.Compare(General.Configs[i].filename, options.ConfigFile, true) == 0)
				{
					// Select this item
					config.SelectedIndex = config.Items.Count - 1;
				}
			}

			// Set the level name
			levelname.Text = options.CurrentName;

			// Fill the resources list
			foreach(ResourceLocation res in options.Resources)
				resources.Items.Add(res);
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
			
			// Level name empty?
			if(levelname.Text.Length == 0)
			{
				// Enter a level name!
				MessageBox.Show(this, "Please enter a level name for your map.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
				levelname.Focus();
				return;
			}

			// Apply changes
			options.ClearResources();
			options.ConfigFile = General.Configs[config.SelectedIndex].filename;
			options.CurrentName = levelname.Text.Trim().ToUpper();
			foreach(ResourceLocation res in resources.Items) options.AddResource(res);

			// Hide window
			this.DialogResult = DialogResult.OK;
			this.Hide();
		}

		// Cancel clicked
		private void cancel_Click(object sender, EventArgs e)
		{
			// Just hide window
			this.DialogResult = DialogResult.Cancel;
			this.Hide();
		}

		// Add Resource clicked
		private void addresource_Click(object sender, EventArgs e)
		{
			ResourceOptionsForm resoptions;
			
			// Open resource options dialog
			resoptions = new ResourceOptionsForm(new ResourceLocation(), "Add Resource");
			if(resoptions.ShowDialog(this) == DialogResult.OK)
			{
				// Add resource
				resources.Items.Add(resoptions.ResourceLocation);
			}
		}

		// Resource Options clicked
		private void editresource_Click(object sender, EventArgs e)
		{
			ResourceOptionsForm resoptions;

			// Anything selected?
			if(resources.SelectedIndex > -1)
			{
				// Open resource options dialog
				resoptions = new ResourceOptionsForm((ResourceLocation)resources.SelectedItem, "Resource Options");
				if(resoptions.ShowDialog(this) == DialogResult.OK)
				{
					// Replace resource
					resources.Items[resources.SelectedIndex] = resoptions.ResourceLocation;
				}
			}
		}

		// Remove resource clicked
		private void deleteresource_Click(object sender, EventArgs e)
		{
			// Anything selected?
			if(resources.SelectedIndex > -1)
			{
				// Remove it
				resources.Items.RemoveAt(resources.SelectedIndex);
			}
		}

		// Resource selection changes
		private void resources_SelectedIndexChanged(object sender, EventArgs e)
		{
			// Enable/disable buttons
			editresource.Enabled = (resources.SelectedIndex > -1);
			deleteresource.Enabled = (resources.SelectedIndex > -1);
		}
	}
}