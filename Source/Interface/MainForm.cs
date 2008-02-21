
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
using System.Globalization;
using System.Windows.Forms;
using CodeImp.DoomBuilder.Controls;
using CodeImp.DoomBuilder.Geometry;
using CodeImp.DoomBuilder.Rendering;
using CodeImp.DoomBuilder.Editing;
using System.Collections;
using System.IO;
using CodeImp.DoomBuilder.Map;

#endregion

namespace CodeImp.DoomBuilder.Interface
{
	public partial class MainForm : DelayedForm, IMainForm
	{
		#region ================== Constants

		private const string STATUS_READY_TEXT = "Ready.";
		private const int MAX_RECENT_FILES = 8;
		private const int MAX_RECENT_FILES_PIXELS = 250;

		#endregion

		#region ================== Delegates

		private delegate void CallUpdateStatusIcon();
		
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
		private MouseInput mouseinput;
		private Rectangle originalclip;
		
		// Recent files
		private ToolStripMenuItem[] recentitems;
		
		// Edit modes
		private List<ToolStripItem> editmodeitems;
		
		#endregion

		#region ================== Properties

		public bool ShiftState { get { return shift; } }
		public bool CtrlState { get { return ctrl; } }
		public bool AltState { get { return alt; } }
		public bool MouseInDisplay { get { return mouseinside; } }
		internal RenderTargetControl Display { get { return display; } }
		public bool SnapToGrid { get { return buttonsnaptogrid.Checked; } }
		public bool AutoMerge { get { return buttonautomerge.Checked; } }

		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		internal MainForm()
		{
			// Setup controls
			InitializeComponent();
			editmodeitems = new List<ToolStripItem>();
			
			// Visual Studio IDE doesn't let me set these in the designer :(
			buttonzoom.Font = menufile.Font;
			buttonzoom.DropDownDirection = ToolStripDropDownDirection.AboveLeft;
			buttongrid.Font = menufile.Font;
			buttongrid.DropDownDirection = ToolStripDropDownDirection.AboveLeft;

			// Bind any methods
			ActionAttribute.BindMethods(this);
			
			// Apply shortcut keys
			ApplyShortcutKeys();
			
			// Make recent items list
			CreateRecentFiles();
			
			// Show splash
			ShowSplashDisplay();
			
			// Keep last position and size
			lastposition = this.Location;
			lastsize = this.Size;
		}
		
		#endregion
		
		#region ================== General

		// This updates all menus for the current status
		internal void UpdateInterface()
		{
			// Map opened?
			if(General.Map != null)
			{
				// Show map name and filename in caption
				this.Text = General.Map.FileTitle + " (" + General.Map.Options.CurrentName + ") - " + Application.ProductName;
			}
			else
			{
				// Show normal caption
				this.Text = Application.ProductName;
			}

			// Update the status bar
			UpdateStatusbar();
			
			// Update menus and toolbar icons
			UpdateFileMenu();
			UpdateEditMenu();
			UpdateToolsMenu();
			UpdateEditModeItems();
		}
		
		// Generic event that invokes the tagged action
		private void InvokeTaggedAction(object sender, EventArgs e)
		{
			string asmname;
			
			this.Update();
			asmname = General.ThisAssembly.GetName().Name.ToLowerInvariant();
			General.Actions[asmname + "_" + (sender as ToolStripItem).Tag.ToString()].Invoke();
			this.Update();
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

			General.WriteLogLine("Closing main interface window...");

			// Stop exclusive mode, if any is active
			StopExclusiveMouseInput();
			SetProcessorState(false);
			
			// Unbind methods
			ActionAttribute.UnbindMethods(this);
			
			// Determine window state to save
			if(this.WindowState != FormWindowState.Minimized)
				windowstate = (int)this.WindowState;
			else
				windowstate = (int)FormWindowState.Normal;
			
			// Save window settings
			General.Settings.WriteSetting("mainwindow.positionx", lastposition.X);
			General.Settings.WriteSetting("mainwindow.positiony", lastposition.Y);
			General.Settings.WriteSetting("mainwindow.sizewidth", lastsize.Width);
			General.Settings.WriteSetting("mainwindow.sizeheight", lastsize.Height);
			General.Settings.WriteSetting("mainwindow.windowstate", windowstate);

			// Save recent files
			SaveRecentFiles();
			
			// Terminate the program
			General.Terminate(true);
		}

		#endregion

		#region ================== Statusbar

		// This updates the status bar
		private void UpdateStatusbar()
		{
			// Map open?
			if(General.Map != null)
			{
				// Enable items
				xposlabel.Enabled = true;
				yposlabel.Enabled = true;
				poscommalabel.Enabled = true;
				zoomlabel.Enabled = true;
				buttonzoom.Enabled = true;
				gridlabel.Enabled = true;
				buttongrid.Enabled = true;
			}
			else
			{
				// Disable items
				xposlabel.Text = "--";
				yposlabel.Text = "--";
				xposlabel.Enabled = false;
				yposlabel.Enabled = false;
				poscommalabel.Enabled = false;
				zoomlabel.Enabled = false;
				buttonzoom.Enabled = false;
				gridlabel.Enabled = false;
				buttongrid.Enabled = false;
			}
		}
		
		// This returns the current status text
		internal string GetCurrentSatus()
		{
			return statuslabel.Text;
		}
		
		// This changes status text
		public void DisplayStatus(string status)
		{
			// Update status description
			if(statuslabel.Text != status)
				statuslabel.Text = status;
			
			// Update icon as well
			UpdateStatusIcon();
			
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

		// This updates the status icon
		internal void UpdateStatusIcon()
		{
			// From another thread?
			if(statusbar.InvokeRequired)
			{
				// Call to form thread
				CallUpdateStatusIcon call = new CallUpdateStatusIcon(UpdateStatusIcon);
				this.Invoke(call);
			}
			else
			{
				// Ready status?
				if(statuslabel.Text == STATUS_READY_TEXT)
				{
					// Map open?
					if((General.Map != null) && (General.Map.Data != null))
					{
						// Check if loading in the background
						if(General.Map.Data.IsLoading)
						{
							// Display semi-ready icon
							statuslabel.Image = CodeImp.DoomBuilder.Properties.Resources.Status1;
						}
						else
						{
							// Display ready icon
							statuslabel.Image = CodeImp.DoomBuilder.Properties.Resources.Status0;
						}
					}
					else
					{
						// Display ready icon
						statuslabel.Image = CodeImp.DoomBuilder.Properties.Resources.Status0;
					}
				}
				else
				{
					// Display busy icon
					statuslabel.Image = CodeImp.DoomBuilder.Properties.Resources.Status2;
				}
			}
		}

		// This changes coordinates display
		public void UpdateCoordinates(Vector2D coords)
		{
			// X position
			if(float.IsNaN(coords.x))
				xposlabel.Text = "--";
			else
				xposlabel.Text = coords.x.ToString("####0");

			// Y position
			if(float.IsNaN(coords.y))
				yposlabel.Text = "--";
			else
				yposlabel.Text = coords.y.ToString("####0");
			
			// Update status bar
			//statusbar.Update();
		}

		// This changes zoom display
		internal void UpdateZoom(float scale)
		{
			// Update scale label
			if(float.IsNaN(scale))
				zoomlabel.Text = "--";
			else
			{
				scale *= 100;
				zoomlabel.Text = scale.ToString("##0") + "%";
			}

			// Update status bar
			//statusbar.Update();
		}

		// Zoom to a specified level
		private void itemzoomto_Click(object sender, EventArgs e)
		{
			int zoom;

			if(General.Map == null) return;

			// In classic mode?
			if(General.Map.Mode is ClassicMode)
			{
				// Requested from menu?
				if(sender is ToolStripMenuItem)
				{
					// Get integral zoom level
					zoom = int.Parse((sender as ToolStripMenuItem).Tag.ToString(), CultureInfo.InvariantCulture);

					// Zoom now
					(General.Map.Mode as ClassicMode).SetZoom((float)zoom / 100f);
				}
			}
		}

		// Zoom to fit in screen
		private void itemzoomfittoscreen_Click(object sender, EventArgs e)
		{
			if(General.Map == null) return;
			
			// In classic mode?
			if(General.Map.Mode is ClassicMode)
				(General.Map.Mode as ClassicMode).CenterInScreen();
		}

		// This changes grid display
		internal void UpdateGrid(int gridsize)
		{
			// Update grid label
			if(gridsize == 0)
				gridlabel.Text = "--";
			else
				gridlabel.Text = gridsize.ToString("###0") + " mp";

			// Update status bar
			//statusbar.Update();
		}

		// Set grid to a specified size
		private void itemgridsize_Click(object sender, EventArgs e)
		{
			int size;

			if(General.Map == null) return;

			// In classic mode?
			if(General.Map.Mode is ClassicMode)
			{
				// Requested from menu?
				if(sender is ToolStripMenuItem)
				{
					// Get integral zoom level
					size = int.Parse((sender as ToolStripMenuItem).Tag.ToString(), CultureInfo.InvariantCulture);

					// Change grid size
					General.Map.Grid.SetGridSize(size);
					
					// Redraw display
					RedrawDisplay();
				}
			}
		}

		// Show grid setup
		private void itemgridcustom_Click(object sender, EventArgs e)
		{
			ShowGridSetup();
		}
		
		// This shows the grid setup dialog
		[Action("gridsetup")]
		internal void ShowGridSetup()
		{
			// Only when a map is open
			if(General.Map == null) return;
			
			// Show preferences dialog
			GridSetupForm gridform = new GridSetupForm();
			if(gridform.ShowDialog(this) == DialogResult.OK)
			{
				// Redraw display
				RedrawDisplay();
			}

			// Done
			gridform.Dispose();
		}
		
		#endregion

		#region ================== Toolbar

		// This enables or disables all editing mode items
		private void UpdateEditModeItems()
		{
			// Enable/disable all items
			foreach(ToolStripItem i in editmodeitems) i.Enabled = (General.Map != null);
		}

		// This checks one of the edit mode items (and unchecks all others)
		internal void CheckEditModeButton(string modeclassname)
		{
			// Go for all items
			foreach(ToolStripItem i in editmodeitems)
			{
				// Check what type it is
				if(i is ToolStripMenuItem)
				{
					// Check if mode type matches with given name
					(i as ToolStripMenuItem).Checked = ((i.Tag as EditModeInfo).Type.Name == modeclassname);
				}
				else if(i is ToolStripButton)
				{
					// Check if mode type matches with given name
					(i as ToolStripButton).Checked = ((i.Tag as EditModeInfo).Type.Name == modeclassname);
				}
			}
		}
		
		// This removes the config-specific editing mode buttons
		internal void RemoveSpecificEditModeButtons()
		{
			bool removed;

			do
			{
				// Go for all items
				removed = false;
				foreach(ToolStripItem i in editmodeitems)
				{
					// Only remove the button if it is for a config-specific editing mode
					if((i.Tag as EditModeInfo).ConfigSpecific)
					{
						// Remove it and restart
						editmodeitems.Remove(i);
						toolbar.Items.Remove(i);
						menuedit.DropDownItems.Remove(i);
						removed = true;
						break;
					}
				}
			}
			while(removed);
		}
		
		// This adds an editing mode button to the toolbar and edit menu
		internal void AddEditModeButton(EditModeInfo modeinfo)
		{
			ToolStripItem item;
			int index;
			
			// Create a button
			index = toolbar.Items.IndexOf(buttoneditmodesseperator);
			item = new ToolStripButton(modeinfo.ButtonDesc, modeinfo.ButtonImage, new EventHandler(EditModeButtonHandler));
			item.DisplayStyle = ToolStripItemDisplayStyle.Image;
			item.Tag = modeinfo;
			item.Enabled = (General.Map != null);
			toolbar.Items.Insert(index, item);
			editmodeitems.Add(item);
			
			// Create menu item
			index = menuedit.DropDownItems.IndexOf(itemeditmodesseperator);
			item = new ToolStripMenuItem(modeinfo.ButtonDesc, modeinfo.ButtonImage, new EventHandler(EditModeButtonHandler));
			item.Tag = modeinfo;
			item.Enabled = (General.Map != null);
			menuedit.DropDownItems.Insert(index, item);
			editmodeitems.Add(item);
		}

		// This handles edit mode button clicks
		private void EditModeButtonHandler(object sender, EventArgs e)
		{
			EditModeInfo modeinfo;
			
			this.Update();
			modeinfo = (EditModeInfo)((sender as ToolStripItem).Tag);
			General.Actions[modeinfo.SwitchAction.GetFullActionName(modeinfo.Plugin.Assembly)].Invoke();
			this.Update();
		}
		
		#endregion
		
		#region ================== Display

		// This shows the splash screen on display
		internal void ShowSplashDisplay()
		{
			// Change display to show splash logo
			display.SetSplashLogoDisplay();
			this.Update();
		}
		
		// This clears the display
		internal void ClearDisplay()
		{
			// Clear the display
			display.SetManualRendering();
			this.Update();
		}

		// This redraws the display on the next paint event
		public void RedrawDisplay()
		{
			if((General.Map != null) && (General.Map.Mode != null)) General.Map.Mode.RedrawDisplay();
			//display.Invalidate();
		}

		// This event is called when a repaint is needed
		private void display_Paint(object sender, PaintEventArgs e)
		{
			if((General.Map != null) && (General.Map.Mode != null) && !displayresized) General.Map.Mode.RefreshDisplay();
		}
		
		// Redraw requested
		private void redrawtimer_Tick(object sender, EventArgs e)
		{
			// Disable timer (only redraw once)
			redrawtimer.Enabled = false;

			// Resume control layouts
			//if(displayresized) General.LockWindowUpdate(IntPtr.Zero);

			// Map opened?
			if(General.Map != null)
			{
				// Display was resized?
				if(displayresized)
				{
					// Reset graphics to match changes
					General.Map.Graphics.Reset();
				}

				// Redraw now
				RedrawDisplay();
			}

			// Display resize is done
			displayresized = false;
		}

		// Display size changes
		private void display_Resize(object sender, EventArgs e)
		{
			// Resizing
			//if(!displayresized) General.LockWindowUpdate(display.Handle);
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
			if((General.Map != null) && (mouseinput == null)) General.Map.Mode.MouseEnter(e);
		}

		// Mouse leaves
		private void display_MouseLeave(object sender, EventArgs e)
		{
			mouseinside = false;
			if((General.Map != null) && (mouseinput == null)) General.Map.Mode.MouseLeave(e);
		}

		// Mouse moves
		private void display_MouseMove(object sender, MouseEventArgs e)
		{
			if((General.Map != null) && (mouseinput == null)) General.Map.Mode.MouseMove(e);
		}

		// Mouse up
		private void display_MouseUp(object sender, MouseEventArgs e) { if(General.Map != null) General.Map.Mode.MouseUp(e); }
		
		#endregion

		#region ================== Input

		// This requests exclusive mouse input
		public void StartExclusiveMouseInput()
		{
			// Only when not already in exclusive mode
			if(mouseinput == null)
			{
				General.WriteLogLine("Starting exclusive mouse input mode...");
				
				// Start special input device
				mouseinput = new MouseInput(this);

				// Lock and hide the mouse in window
				originalclip = Cursor.Clip;
				Cursor.Clip = display.RectangleToScreen(display.ClientRectangle);
				Cursor.Hide();
			}
		}
		
		// This stops exclusive mouse input
		public void StopExclusiveMouseInput()
		{
			// Only when in exclusive mode
			if(mouseinput != null)
			{
				General.WriteLogLine("Stopping exclusive mouse input mode...");

				// Stop special input device
				mouseinput.Dispose();
				mouseinput = null;

				// Release and show the mouse
				Cursor.Clip = originalclip;
				Cursor.Show();
			}
		}
		
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
				//for(int i = 0; i < e.Delta; i += 120)
				General.Actions.InvokeByKey(mod | (int)SpecialKeys.MScrollUp);
			}
			// Scrollwheel down?
			else if(e.Delta < 0)
			{
				// Invoke actions for scrollwheel
				//for(int i = 0; i > e.Delta; i -= 120)
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

			// Invoke on editing mode
			if(General.Map != null) General.Map.Mode.KeyDown(e);
		}

		// When a key is released
		private void MainForm_KeyUp(object sender, KeyEventArgs e)
		{
			// Keep key modifiers
			alt = e.Alt;
			shift = e.Shift;
			ctrl = e.Control;

			// Invoke on editing mode
			if(General.Map != null) General.Map.Mode.KeyUp(e);
		}

		#endregion

		#region ================== Menus

		// Public method to apply shortcut keys
		internal void ApplyShortcutKeys()
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

		#endregion

		#region ================== File Menu

		// This sets up the file menu
		private void UpdateFileMenu()
		{
			// Enable/disable items
			itemclosemap.Enabled = (General.Map != null);
			itemsavemap.Enabled = (General.Map != null);
			itemsavemapas.Enabled = (General.Map != null);
			itemsavemapinto.Enabled = (General.Map != null);

			// Toolbar icons
			buttonnewmap.Enabled = itemnewmap.Enabled;
			buttonopenmap.Enabled = itemopenmap.Enabled;
			buttonsavemap.Enabled = itemsavemap.Enabled;
		}

		// This sets the recent files from configuration
		private void CreateRecentFiles()
		{
			int insertindex;
			bool anyitems = false;
			string filename;
			
			// Where to insert
			insertindex = menufile.DropDownItems.IndexOf(itemnorecent);
			
			// Create all items
			recentitems = new ToolStripMenuItem[MAX_RECENT_FILES];
			for(int i = 0; i < MAX_RECENT_FILES; i++)
			{
				// Create item
				recentitems[i] = new ToolStripMenuItem("");
				recentitems[i].Tag = "";
				recentitems[i].Click += new EventHandler(recentitem_Click);
				menufile.DropDownItems.Insert(insertindex + i, recentitems[i]);

				// Get configuration setting
				filename = General.Settings.ReadSetting("recentfiles.file" + i, "");
				if(filename != "")
				{
					// Set up item
					recentitems[i].Text = GetDisplayFilename(filename);
					recentitems[i].Tag = filename;
					recentitems[i].Visible = true;
					anyitems = true;
				}
				else
				{
					// Hide item
					recentitems[i].Visible = false;
				}
			}

			// Hide the no recent item when there are items
			itemnorecent.Visible = !anyitems;
		}
		
		// This saves the recent files list
		private void SaveRecentFiles()
		{
			// Go for all items
			for(int i = 0; i < MAX_RECENT_FILES; i++)
			{
				// Recent file set?
				if(recentitems[i].Text != "")
				{
					// Save to configuration
					General.Settings.WriteSetting("recentfiles.file" + i, recentitems[i].Tag.ToString());
				}
			}
		}
		
		// This adds a recent file to the list
		internal void AddRecentFile(string filename)
		{
			int movedownto = MAX_RECENT_FILES - 1;
			
			// Check if this file is already in the list
			for(int i = 0; i < MAX_RECENT_FILES; i++)
			{
				// File same as this item?
				if(string.Compare(filename, recentitems[i].Tag.ToString(), true) == 0)
				{
					// Move down to here so that this item will disappear
					movedownto = i;
					break;
				}
			}
			
			// Go for all items, except the last one, backwards
			for(int i = movedownto - 1; i >= 0; i--)
			{
				// Move recent file down the list
				recentitems[i + 1].Text = recentitems[i].Text;
				recentitems[i + 1].Tag = recentitems[i].Tag.ToString();
				recentitems[i + 1].Visible = (recentitems[i + 1].Text != "");
			}

			// Add new file at the top
			recentitems[0].Text = GetDisplayFilename(filename);
			recentitems[0].Tag = filename;
			recentitems[0].Visible = true;

			// Hide the no recent item
			itemnorecent.Visible = false;
		}

		// This returns the trimmed file/path string
		private string GetDisplayFilename(string filename)
		{
			string newname;
			
			// String doesnt fit?
			if(GetStringWidth(filename) > MAX_RECENT_FILES_PIXELS)
			{
				// Start chopping off characters
				for(int i = filename.Length - 6; i >= 0; i--)
				{
					// Does it fit now?
					newname = filename.Substring(0, 3) + "..." + filename.Substring(filename.Length - i, i);
					if(GetStringWidth(newname) <= MAX_RECENT_FILES_PIXELS) return newname;
				}

				// Cant find anything that fits (most unlikely!)
				return "wtf?!";
			}
			else
			{
				// The whole string fits
				return filename;
			}
		}
		
		// This returns the width of a string
		private float GetStringWidth(string str)
		{
			Graphics g = Graphics.FromHwndInternal(this.Handle);
			SizeF strsize = g.MeasureString(str, this.Font);
			return strsize.Width;
		}
		
		// Exit clicked
		private void itemexit_Click(object sender, EventArgs e) { this.Close(); }

		// Recent item clicked
		private void recentitem_Click(object sender, EventArgs e)
		{
			// Get the item that was clicked
			ToolStripItem item = (sender as ToolStripItem);

			// Open this file
			General.OpenMapFile(item.Tag.ToString());
		}
		
		#endregion

		#region ================== Edit Menu

		// This sets up the edit menu
		private void UpdateEditMenu()
		{
			// No edit menu when no map open
			//menuedit.Visible = (General.Map != null);
			
			// Enable/disable items
			itemundo.Enabled = (General.Map != null) && (General.Map.UndoRedo.NextUndo != null);
			itemredo.Enabled = (General.Map != null) && (General.Map.UndoRedo.NextRedo != null);
			itemmapoptions.Enabled = (General.Map != null);
			itemsnaptogrid.Enabled = (General.Map != null);
			itemautomerge.Enabled = (General.Map != null);

			// Determine undo description
			if(itemundo.Enabled)
				itemundo.Text = "Undo " + General.Map.UndoRedo.NextUndo.description;
			else
				itemundo.Text = "Undo";

			// Determine redo description
			if(itemredo.Enabled)
				itemredo.Text = "Redo " + General.Map.UndoRedo.NextRedo.description;
			else
				itemredo.Text = "Redo";
			
			// Toolbar icons
			buttonmapoptions.Enabled = (General.Map != null);
			buttonundo.Enabled = itemundo.Enabled;
			buttonredo.Enabled = itemredo.Enabled;
			buttonundo.ToolTipText = itemundo.Text;
			buttonredo.ToolTipText = itemredo.Text;
			buttonsnaptogrid.Enabled = (General.Map != null);
			buttonautomerge.Enabled = (General.Map != null);
		}

		// Action to toggle snap to grid
		[Action("togglesnap")]
		internal void ToggleSnapToGrid()
		{
			buttonsnaptogrid.Checked = !buttonsnaptogrid.Checked;
			itemsnaptogrid.Checked = buttonsnaptogrid.Checked;
		}

		// Action to toggle auto merge
		[Action("toggleautomerge")]
		internal void ToggleAutoMerge()
		{
			buttonautomerge.Checked = !buttonautomerge.Checked;
			itemautomerge.Checked = buttonautomerge.Checked;
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

		#region ================== Tools Menu

		// This sets up the tools menu
		private void UpdateToolsMenu()
		{
			// Enable/disable items
			itemreloadresources.Enabled = (General.Map != null);
			
			// Toolbar icons
		}
		
		// Game Configuration action
		[Action("configuration")]
		internal void ShowConfiguration()
		{
			// Show configuration dialog
			ConfigForm cfgform = new ConfigForm();
			if(cfgform.ShowDialog(this) == DialogResult.OK)
			{
				// Update interface
				UpdateInterface();
				
				// Reload resources if a map is open
				if(General.Map != null) General.Map.ReloadResources();
				
				// Redraw display
				RedrawDisplay();
			}
			
			// Done
			cfgform.Dispose();
		}

		// Preferences action
		[Action("preferences")]
		internal void ShowPreferences()
		{
			// Show preferences dialog
			PreferencesForm prefform = new PreferencesForm();
			if(prefform.ShowDialog(this) == DialogResult.OK)
			{
				// Update shortcut keys in menus
				ApplyShortcutKeys();

				// Apply new settings if a map is open
				if(General.Map != null) General.Map.Map.UpdateConfiguration();
				
				// Redraw display
				RedrawDisplay();
			}

			// Done
			prefform.Dispose();
		}

		#endregion
		
		#region ================== Info Panels

		// This hides all info panels
		public void HideInfo()
		{
			// Hide them all
			if(linedefinfo.Visible) linedefinfo.Hide();
			if(vertexinfo.Visible) vertexinfo.Hide();
			if(sectorinfo.Visible) sectorinfo.Hide();
			if(thinginfo.Visible) thinginfo.Hide();
		}
		
		// Show linedef info
		public void ShowLinedefInfo(Linedef l) { linedefinfo.ShowInfo(l); }

		// Show vertex info
		public void ShowVertexInfo(Vertex v) { vertexinfo.ShowInfo(v); }

		// Show sector info
		public void ShowSectorInfo(Sector s) { sectorinfo.ShowInfo(s); }

		// Show thing info
		public void ShowThingInfo(Thing t) { thinginfo.ShowInfo(t); }

		#endregion

		#region ================== Dialogs

		// This shows the dialog to edit lines
		public void ShowEditLinedefs(ICollection<Linedef> lines)
		{
			// Show line edit dialog
			LinedefEditForm f = new LinedefEditForm();
			f.Setup(lines);
			f.ShowDialog(this);
			f.Dispose();
		}

		// This shows the dialog to edit sectors
		public void ShowEditSectors(ICollection<Sector> sectors)
		{
			// Show line edit dialog
			SectorEditForm f = new SectorEditForm();
			f.Setup(sectors);
			f.ShowDialog(this);
			f.Dispose();
		}

		#endregion

		#region ================== Processor

		// This toggles the processor
		public void SetProcessorState(bool on)
		{
			processor.Enabled = on;
		}
		
		// Processor event
		private void processor_Tick(object sender, EventArgs e)
		{
			Vector2D deltamouse;
			
			// In exclusive mouse mode?
			if(mouseinput != null)
			{
				// Process mouse input
				deltamouse = mouseinput.Process();
				if((General.Map != null) && (General.Map.Mode != null))
					General.Map.Mode.MouseInput(deltamouse);
			}
			
			// Process signal
			if((General.Map != null) && (General.Map.Mode != null))
				General.Map.Mode.Process();
		}

		#endregion
	}
}