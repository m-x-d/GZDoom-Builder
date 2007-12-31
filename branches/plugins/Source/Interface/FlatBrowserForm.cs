
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
using CodeImp.DoomBuilder.Controls;
using CodeImp.DoomBuilder.Data;
using CodeImp.DoomBuilder.Config;
using CodeImp.DoomBuilder.Map;

#endregion

namespace CodeImp.DoomBuilder.Interface
{
	public partial class FlatBrowserForm : DelayedForm
	{
		// Variables
		private string selectedname;
		private Point lastposition;
		private Size lastsize;
		
		// Properties
		public string SelectedName { get { return selectedname; } }
		
		// Constructor
		public FlatBrowserForm()
		{
			Dictionary<long, long> useditems = new Dictionary<long,long>();
			
			// Initialize
			InitializeComponent();
			browser.ApplyColorSettings();
			
			// Make groups
			ListViewGroup used = browser.AddGroup("Used Flats");
			ListViewGroup avail = browser.AddGroup("Available Flats");
			
			// Go through the map to find the used flats
			foreach(Sector s in General.Map.Map.Sectors)
			{
				// Add floor flat
				if(!useditems.ContainsKey(s.LongFloorTexture)) useditems.Add(s.LongFloorTexture, 0);

				// Add ceiling flat
				if(!useditems.ContainsKey(s.LongCeilTexture)) useditems.Add(s.LongCeilTexture, 0);
			}

			// When mixing textures with flats, include textures as well
			if(General.Map.Config.MixTexturesFlats)
			{
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
			}
			
			// Start adding
			browser.BeginAdding();

			// Add all used flats
			foreach(ImageData img in General.Map.Data.Flats)
				if(useditems.ContainsKey(img.LongName))
					browser.Add(img.Name, img, img, used);
			
			// Add all available flats
			foreach(ImageData img in General.Map.Data.Flats)
				browser.Add(img.Name, img, img, avail);
			
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
		private void FlatBrowserForm_Activated(object sender, EventArgs e)
		{
			// Focus the textbox
			browser.FocusTextbox();
		}

		// Loading
		private void FlatBrowserForm_Load(object sender, EventArgs e)
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
				return null;
			}
		}
	}
}