
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

#endregion

namespace CodeImp.DoomBuilder.Interface
{
	public partial class MainForm : Form
	{
		#region ================== Constants

		private const string STATUS_READY_TEXT = "Ready.";

		#endregion

		#region ================== Variables

		// Position/size
		private Point lastposition;
		private Size lastsize;

		#endregion

		#region ================== Properties

		public PictureBox Display { get { return display; } }

		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		public MainForm()
		{
			// Setup controls
			InitializeComponent();
			
			// Keep last position and size
			lastposition = this.Location;
			lastsize = this.Size;
		}
		
		#endregion

		#region ================== Window

		// Window is loaded
		private void MainForm_Load(object sender, EventArgs e)
		{
			// Position window from configuration settings
			this.SuspendLayout();
			this.Location = new Point(General.Settings.ReadSetting("mainwindow.positionx", this.Location.X),
									  General.Settings.ReadSetting("mainwindow.positiony", this.Location.Y));
			this.Size = new Size(General.Settings.ReadSetting("mainwindow.sizewidth", this.Size.Width),
								 General.Settings.ReadSetting("mainwindow.sizeheight", this.Size.Height));
			this.WindowState = (FormWindowState)General.Settings.ReadSetting("mainwindow.windowstate", (int)FormWindowState.Maximized);
			this.ResumeLayout(true);
			
			// Normal windowstate?
			if(this.WindowState == FormWindowState.Normal)
			{
				// Keep last position and size
				lastposition = this.Location;
				lastsize = this.Size;
			}
		}
		
		// Window is moved
		private void MainForm_Move(object sender, EventArgs e)
		{
			// Normal windowstate?
			if(this.WindowState == FormWindowState.Normal)
			{
				// Keep last position and size
				lastposition = this.Location;
				lastsize = this.Size;
			}
		}

		// Window was resized
		private void MainForm_ResizeEnd(object sender, EventArgs e)
		{
			// Normal windowstate?
			if(this.WindowState == FormWindowState.Normal)
			{
				// Keep last position and size
				lastposition = this.Location;
				lastsize = this.Size;
			}
		}

		// Window is being closed
		private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			int windowstate;
			
			// Determine window state to save
			if(this.WindowState != FormWindowState.Minimized) windowstate = (int)this.WindowState; else windowstate = (int)FormWindowState.Normal;
			
			// Save settings to configuration
			General.Settings.WriteSetting("mainwindow.positionx", lastposition.X);
			General.Settings.WriteSetting("mainwindow.positiony", lastposition.Y);
			General.Settings.WriteSetting("mainwindow.sizewidth", lastsize.Width);
			General.Settings.WriteSetting("mainwindow.sizeheight", lastsize.Height);
			General.Settings.WriteSetting("mainwindow.windowstate", windowstate);

			// Terminate the program
			General.Terminate();
		}

		#endregion

		#region ================== Statusbar

		// This changes status text
		public void DisplayStatus(string status)
		{
			// Update status description
			if(statuslabel.Text != status)
				statuslabel.Text = status;
			
			// Refresh if needed
			statusbar.Invalidate();
			this.Update();
		}

		// This changes status text to Ready
		public void DisplayReady()
		{
			// Display ready status description
			DisplayStatus(STATUS_READY_TEXT);
		}

		#endregion

		#region ================== Display

		// This shows the splash screen on display
		public void ShowSplashDisplay()
		{
			// Change display to show splash logo
			display.BackColor = System.Drawing.SystemColors.AppWorkspace;
			display.BackgroundImage = global::CodeImp.DoomBuilder.Properties.Resources.Splash2;
			display.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this.Update();
		}
		
		// This clears the display
		public void ClearDisplay()
		{
			// Clear the display
			display.BackColor = Color.Black;
			display.BackgroundImage = null;
			display.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Tile;
			this.Update();
		}

		// Display needs repainting
		private void display_Paint(object sender, PaintEventArgs e)
		{
			// Repaint on demand
			if(General.Map != null) redrawtimer.Enabled = true;
		}

		// Redraw requested
		private void redrawtimer_Tick(object sender, EventArgs e)
		{
			// Disable timer (only redraw once)
			redrawtimer.Enabled = false;
			
			// Redraw now
			if(General.Map != null) General.Map.Mode.RedrawDisplay();
		}
		
		// Mouse click
		private void display_MouseClick(object sender, MouseEventArgs e) { if(General.Map != null) General.Map.Mode.MouseClick(e); }

		// Mouse doubleclick
		private void display_MouseDoubleClick(object sender, MouseEventArgs e) { if(General.Map != null) General.Map.Mode.MouseDoubleClick(e); }

		// Mouse down
		private void display_MouseDown(object sender, MouseEventArgs e) { if(General.Map != null) General.Map.Mode.MouseDown(e); }

		// Mouse enters
		private void display_MouseEnter(object sender, EventArgs e) { if(General.Map != null) General.Map.Mode.MouseEnter(e); }

		// Mouse leaves
		private void display_MouseLeave(object sender, EventArgs e) { if(General.Map != null) General.Map.Mode.MouseLeave(e); }

		// Mouse moves
		private void display_MouseMove(object sender, MouseEventArgs e) { if(General.Map != null) General.Map.Mode.MouseMove(e); }

		// Mouse up
		private void display_MouseUp(object sender, MouseEventArgs e) { if(General.Map != null) General.Map.Mode.MouseUp(e); }
		
		#endregion

		#region ================== Menus

		// This updates all menus for the current status
		public void UpdateMenus()
		{
			// Update them all
			UpdateFileMenu();
		}

		#endregion

		#region ================== File Menu

		// New map clicked
		private void itemnewmap_Click(object sender, EventArgs e) { General.NewMap(); }

		// Close map clicked
		private void itemclosemap_Click(object sender, EventArgs e) { General.CloseMap(); }

		// Exit clicked
		private void itemexit_Click(object sender, EventArgs e) { this.Close(); }

		// This sets up the file menu
		private void UpdateFileMenu()
		{
			// Enable/disable items
			itemclosemap.Enabled = (General.Map != null);
			itemsavemap.Enabled = (General.Map != null);
			itemsavemapas.Enabled = (General.Map != null);
			itemsavemapinto.Enabled = (General.Map != null);
		}

		#endregion

		#region ================== Help Menu

		// About clicked
		private void itemhelpabout_Click(object sender, EventArgs e)
		{
			AboutForm aboutform;
			
			// Show about dialog
			aboutform = new AboutForm();
			aboutform.ShowDialog(this);
		}

		#endregion
	}
}