
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
using CodeImp.DoomBuilder.Actions;
using CodeImp.DoomBuilder.Data;
using CodeImp.DoomBuilder.Config;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Controls;

#endregion

namespace CodeImp.DoomBuilder.Windows
{
	internal partial class FlatBrowserForm : DelayedForm
	{
		// Variables
		private string selectedname;
		private Point lastposition;
		private Size lastsize;
		private ListViewGroup usedgroup;
		private ListViewGroup availgroup;
		
		// Properties
		public string SelectedName { get { return selectedname; } }
		
		// Constructor
		public FlatBrowserForm()
		{
			Cursor.Current = Cursors.WaitCursor;
			ListViewItem item;
			
			// Initialize
			InitializeComponent();
			browser.ApplyColorSettings();
			
			// Update the used textures
			General.Map.Data.UpdateUsedTextures();

			// Fill texture sets list with normal texture sets
			foreach(IFilledTextureSet ts in General.Map.Data.TextureSets)
			{
				item = texturesets.Items.Add(ts.Name);
				item.Tag = ts;
				item.ImageIndex = 0;
			}

			// Add special textures sets
			item = texturesets.Items.Add(General.Map.Data.OthersTextureSet.Name);
			item.Tag = General.Map.Data.OthersTextureSet;
			item.ImageIndex = 1;
			item = texturesets.Items.Add(General.Map.Data.AllTextureSet.Name);
			item.Tag = General.Map.Data.AllTextureSet;
			item.ImageIndex = 2;
			
			// Select the last one that was selected
			string selectname = General.Settings.ReadSetting("browserwindow.textureset", "");
			foreach(ListViewItem i in texturesets.Items) if(i.Text == selectname) i.Selected = true;

			// None selected? Then select the first
			if(texturesets.SelectedItems.Count == 0)
				texturesets.Items[0].Selected = true;

			// Make groups
			usedgroup = browser.AddGroup("Used Textures");
			availgroup = browser.AddGroup("Available Textures");
			
			// Keep last position and size
			lastposition = this.Location;
			lastsize = this.Size;
		}

		// Selection changed
		private void browser_SelectedItemChanged()
		{
			apply.Enabled = (browser.SelectedItem != null);
		}

		// OK clicked
		private void apply_Click(object sender, EventArgs e)
		{
			// Set selected name and close
			selectedname = browser.SelectedItem.Text;
			DialogResult = DialogResult.OK;
			this.Close();
		}

		// Cancel clicked
		private void cancel_Click(object sender, EventArgs e)
		{
			// No selection, close
			selectedname = "";
			DialogResult = DialogResult.Cancel;
			this.Close();
		}

		// Activated
		private void FlatBrowserForm_Activated(object sender, EventArgs e)
		{
			// Focus the textbox
			browser.FocusTextbox();
			Cursor.Current = Cursors.Default;
		}

		// Loading
		private void FlatBrowserForm_Load(object sender, EventArgs e)
		{
			/*
			// Position window from configuration settings
			this.SuspendLayout();
			this.Location = new Point(General.Settings.ReadSetting("browserwindow.positionx", this.Location.X),
									  General.Settings.ReadSetting("browserwindow.positiony", this.Location.Y));
			this.Size = new Size(General.Settings.ReadSetting("browserwindow.sizewidth", this.Size.Width),
								 General.Settings.ReadSetting("browserwindow.sizeheight", this.Size.Height));
			this.WindowState = (FormWindowState)General.Settings.ReadSetting("browserwindow.windowstate", (int)FormWindowState.Normal);
			this.ResumeLayout(true);
			*/
			
			// Normal windowstate?
			if(this.WindowState == FormWindowState.Normal)
			{
				// Keep last position and size
				lastposition = this.Location;
				lastsize = this.Size;
			}
		}

		// Resized
		private void FlatBrowserForm_ResizeEnd(object sender, EventArgs e)
		{
			// Normal windowstate?
			if(this.WindowState == FormWindowState.Normal)
			{
				// Keep last position and size
				lastposition = this.Location;
				lastsize = this.Size;
			}
		}

		// Moved
		private void FlatBrowserForm_Move(object sender, EventArgs e)
		{
			// Normal windowstate?
			if(this.WindowState == FormWindowState.Normal)
			{
				// Keep last position and size
				lastposition = this.Location;
				lastsize = this.Size;
			}
		}

		// Closing
		private void FlatBrowserForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			int windowstate;

			// Determine window state to save
			if(this.WindowState != FormWindowState.Minimized)
				windowstate = (int)this.WindowState;
			else
				windowstate = (int)FormWindowState.Normal;

			// Save window settings
			General.Settings.WriteSetting("browserwindow.positionx", lastposition.X);
			General.Settings.WriteSetting("browserwindow.positiony", lastposition.Y);
			General.Settings.WriteSetting("browserwindow.sizewidth", lastsize.Width);
			General.Settings.WriteSetting("browserwindow.sizeheight", lastsize.Height);
			General.Settings.WriteSetting("browserwindow.windowstate", windowstate);
			
			// Save last selected texture set
			if(texturesets.SelectedItems.Count > 0)
				General.Settings.WriteSetting("browserwindow.textureset", texturesets.SelectedItems[0].Text);
			
			// Clean up
			browser.CleanUp();
		}

		// Static method to browse for flats
		// Returns null when cancelled.
		public static string Browse(IWin32Window parent, string select)
		{
			FlatBrowserForm browser = new FlatBrowserForm();
			if(browser.ShowDialog(parent) == DialogResult.OK)
			{
				// Return result
				return browser.SelectedName;
			}
			else
			{
				// Cancelled
				return select;
			}
		}
		
		// Texture set selected
		private void texturesets_SelectedIndexChanged(object sender, EventArgs e)
		{
			// Anything slected?
			if(texturesets.SelectedItems.Count > 0)
			{
				// Get the selected texture set
				IFilledTextureSet set = (texturesets.SelectedItems[0].Tag as IFilledTextureSet);
				
				// Start adding
				browser.BeginAdding(false);

				// Add all used flats
				foreach(ImageData img in set.Flats)
					if(img.UsedInMap) browser.Add(img.Name, img, img, usedgroup);

				// Add all available flats
				foreach(ImageData img in set.Flats)
					browser.Add(img.Name, img, img, availgroup);

				// Done adding
				browser.EndAdding();
			}
		}

		// Item double clicked
		private void browser_SelectedItemDoubleClicked()
		{
			if(apply.Enabled) apply_Click(this, EventArgs.Empty);
		}
	}
}