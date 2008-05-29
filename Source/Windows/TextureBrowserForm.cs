
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
	internal partial class TextureBrowserForm : DelayedForm
	{
		// Variables
		private string selectedname;
		private Point lastposition;
		private Size lastsize;
		
		// Properties
		public string SelectedName { get { return selectedname; } }
		
		// Constructor
		public TextureBrowserForm()
		{
			Dictionary<long, long> useditems = new Dictionary<long,long>();
			
			// Initialize
			InitializeComponent();
			browser.ApplyColorSettings();
			
			// Make groups
			ListViewGroup used = browser.AddGroup("Used Textures");
			ListViewGroup avail = browser.AddGroup("Available Textures");
			
			// Go through the map to find the used textures
			foreach(Sidedef sd in General.Map.Map.Sidedefs)
			{
				// Add high texture
				if(sd.HighTexture.Length > 0)
					if(!useditems.ContainsKey(sd.LongHighTexture)) useditems.Add(sd.LongHighTexture, 0);

				// Add mid texture
				if(sd.LowTexture.Length > 0)
					if(!useditems.ContainsKey(sd.LongMiddleTexture)) useditems.Add(sd.LongMiddleTexture, 0);

				// Add low texture
				if(sd.MiddleTexture.Length > 0)
					if(!useditems.ContainsKey(sd.LongLowTexture)) useditems.Add(sd.LongLowTexture, 0);
			}

			// When mixing textures with flats, include flats as well
			if(General.Map.Config.MixTexturesFlats)
			{
				// Go through the map to find the used flats
				foreach(Sector s in General.Map.Map.Sectors)
				{
					// Add floor flat
					if(!useditems.ContainsKey(s.LongFloorTexture)) useditems.Add(s.LongFloorTexture, 0);

					// Add ceil flat
					if(!useditems.ContainsKey(s.LongCeilTexture)) useditems.Add(s.LongCeilTexture, 0);
				}
			}
			
			// Start adding
			browser.BeginAdding();
			
			// Add all available textures and mark the images for temporary loading
			foreach(ImageData img in General.Map.Data.Textures)
			{
				browser.Add(img.Name, img, img, avail);
				img.Temporary = true;
			}

			// Add all used textures and mark the images for permanent loading
			foreach(ImageData img in General.Map.Data.Textures)
			{
				if(useditems.ContainsKey(img.LongName))
				{
					browser.Add(img.Name, img, img, used);
					img.Temporary = false;
				}
			}
			
			// Done adding
			browser.EndAdding();
			
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
		private void TextureBrowserForm_Activated(object sender, EventArgs e)
		{
			// Focus the textbox
			browser.FocusTextbox();
		}

		// Loading
		private void TextureBrowserForm_Load(object sender, EventArgs e)
		{
			// Position window from configuration settings
			this.SuspendLayout();
			this.Location = new Point(General.Settings.ReadSetting("browserwindow.positionx", this.Location.X),
									  General.Settings.ReadSetting("browserwindow.positiony", this.Location.Y));
			this.Size = new Size(General.Settings.ReadSetting("browserwindow.sizewidth", this.Size.Width),
								 General.Settings.ReadSetting("browserwindow.sizeheight", this.Size.Height));
			this.WindowState = (FormWindowState)General.Settings.ReadSetting("browserwindow.windowstate", (int)FormWindowState.Normal);
			this.ResumeLayout(true);

			// Normal windowstate?
			if(this.WindowState == FormWindowState.Normal)
			{
				// Keep last position and size
				lastposition = this.Location;
				lastsize = this.Size;
			}
		}

		// Resized
		private void TextureBrowserForm_ResizeEnd(object sender, EventArgs e)
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
		private void TextureBrowserForm_Move(object sender, EventArgs e)
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
		private void TextureBrowserForm_FormClosing(object sender, FormClosingEventArgs e)
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
			
			// Clean up
			browser.CleanUp();
		}

		// Static method to browse for texture
		// Returns null when cancelled.
		public static string Browse(IWin32Window parent, string select)
		{
			TextureBrowserForm browser = new TextureBrowserForm();
			if(browser.ShowDialog(parent) == DialogResult.OK)
			{
				// Return result
				return browser.SelectedName;
			}
			else
			{
				// Cancelled
				return null;
			}
		}
	}
}