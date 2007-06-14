using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

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
	}
}