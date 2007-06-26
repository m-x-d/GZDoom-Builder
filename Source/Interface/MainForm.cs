
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
using CodeImp.DoomBuilder.Controls;

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
		private bool displayresized = true;
		
		// Mouse in display
		private bool mouseinside;
		
		// Input
		private bool shift, ctrl, alt;
		
		#endregion

		#region ================== Properties

		public bool MouseInDisplay { get { return mouseinside; } }
		public Panel Display { get { return display; } }

		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		public MainForm()
		{
			// Setup controls
			InitializeComponent();
			
			// Apply shortcut keys
			ApplyShortcutKeys();
			
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

		// Window resizes
		private void MainForm_Resize(object sender, EventArgs e)
		{
			// Resizing
			//this.SuspendLayout();
			//resized = true;
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
			if(this.WindowState != FormWindowState.Minimized)
				windowstate = (int)this.WindowState;
			else
				windowstate = (int)FormWindowState.Normal;
			
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
			//display.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this.Update();
		}
		
		// This clears the display
		public void ClearDisplay()
		{
			// Clear the display
			display.BackColor = Color.Black;
			display.BackgroundImage = null;
			//display.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Tile;
			this.Update();
		}

		// Display needs repainting
		private void display_Paint(object sender, PaintEventArgs e)
		{
			// Request redraw
			if(!redrawtimer.Enabled) redrawtimer.Enabled = true;
		}

		// Redraw requested
		private void redrawtimer_Tick(object sender, EventArgs e)
		{
			// Disable timer (only redraw once)
			redrawtimer.Enabled = false;

			// Resume control layouts
			if(displayresized) General.LockWindowUpdate(IntPtr.Zero);
			
			// Map opened?
			if(General.Map != null)
			{
				// Display was resized?
				if(displayresized)
				{
					// Reset graphics to match changes
					General.Map.Graphics.Reset();

					// Make sure control is repainted
					display.Update();
				}
				
				// Redraw now
				General.Map.Mode.RedrawDisplay();
			}

			// Display resize is done
			displayresized = false;
		}

		// Display size changes
		private void display_Resize(object sender, EventArgs e)
		{
			// Resizing
			if(!displayresized) General.LockWindowUpdate(display.Handle);
			displayresized = true;
			
			// Request redraw
			if(!redrawtimer.Enabled) redrawtimer.Enabled = true;
		}
		
		// Mouse click
		private void display_MouseClick(object sender, MouseEventArgs e) { if(General.Map != null) General.Map.Mode.MouseClick(e); }

		// Mouse doubleclick
		private void display_MouseDoubleClick(object sender, MouseEventArgs e) { if(General.Map != null) General.Map.Mode.MouseDoubleClick(e); }

		// Mouse down
		private void display_MouseDown(object sender, MouseEventArgs e) { if(General.Map != null) General.Map.Mode.MouseDown(e); }

		// Mouse enters
		private void display_MouseEnter(object sender, EventArgs e)
		{
			mouseinside = true;
			if(General.Map != null) General.Map.Mode.MouseEnter(e);
		}

		// Mouse leaves
		private void display_MouseLeave(object sender, EventArgs e)
		{
			mouseinside = false;
			if(General.Map != null) General.Map.Mode.MouseLeave(e);
		}

		// Mouse moves
		private void display_MouseMove(object sender, MouseEventArgs e) { if(General.Map != null) General.Map.Mode.MouseMove(e); }

		// Mouse up
		private void display_MouseUp(object sender, MouseEventArgs e) { if(General.Map != null) General.Map.Mode.MouseUp(e); }
		
		#endregion

		#region ================== Input

		// When the mouse wheel is changed
		protected override void OnMouseWheel(MouseEventArgs e)
		{
			int mod = 0;

			// Create modifiers
			if(alt) mod |= (int)Keys.Alt;
			if(shift) mod |= (int)Keys.Shift;
			if(ctrl) mod |= (int)Keys.Control;

			// Scrollwheel up?
			if(e.Delta > 0)
			{
				// Invoke actions for scrollwheel
				General.Actions.InvokeByKey(mod | (int)SpecialKeys.MScrollUp);
			}
			// Scrollwheel down?
			else if(e.Delta < 0)
			{
				// Invoke actions for scrollwheel
				General.Actions.InvokeByKey(mod | (int)SpecialKeys.MScrollDown);
			}

			// Let the base know
			base.OnMouseWheel(e);
		}
		
		// When a key is pressed
		private void MainForm_KeyDown(object sender, KeyEventArgs e)
		{
			// Keep key modifiers
			alt = e.Alt;
			shift = e.Shift;
			ctrl = e.Control;
			
			// Invoke any actions associated with this key
			General.Actions.InvokeByKey((int)e.KeyData);
		}

		// When a key is released
		private void MainForm_KeyUp(object sender, KeyEventArgs e)
		{
			// Keep key modifiers
			alt = e.Alt;
			shift = e.Shift;
			ctrl = e.Control;
		}

		#endregion

		#region ================== Menus

		// Public method to apply shortcut keys
		public void ApplyShortcutKeys()
		{
			// Apply shortcut keys to menus
			ApplyShortcutKeys(menumain.Items);
		}
		
		// This sets the shortcut keys on menu items
		private void ApplyShortcutKeys(ToolStripItemCollection items)
		{
			ToolStripMenuItem menuitem;
			string actionname;
			
			// Go for all controls to find menu items
			foreach(ToolStripItem item in items)
			{
				// This is a menu item?
				if(item is ToolStripMenuItem)
				{
					// Get the item in proper type
					menuitem = (item as ToolStripMenuItem);

					// Tag set for this item?
					if(menuitem.Tag != null)
					{
						// Get the action name
						actionname = menuitem.Tag.ToString();

						// Action with this name available?
						if(General.Actions.Exists(actionname))
						{
							// Put the action shortcut key on the menu item
							menuitem.ShortcutKeyDisplayString = Action.GetShortcutKeyDesc(General.Actions[actionname].ShortcutKey);
						}
					}

					// Recursively apply shortcut keys to child menu items as well
					ApplyShortcutKeys(menuitem.DropDownItems);
				}
			}
		}
		
		// This updates all menus for the current status
		public void UpdateMenus()
		{
			// Update them all
			UpdateFileMenu();
		}

		#endregion

		#region ================== File Menu

		// New map clicked
		private void itemnewmap_Click(object sender, EventArgs e) { General.Actions[Action.NEWMAP].Invoke(); }

		// Open map clicked
		private void itemopenmap_Click(object sender, EventArgs e) { General.Actions[Action.OPENMAP].Invoke(); }

		// Close map clicked
		private void itemclosemap_Click(object sender, EventArgs e) { General.Actions[Action.CLOSEMAP].Invoke(); }

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