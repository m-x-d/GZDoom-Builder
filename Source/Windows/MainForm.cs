
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
using CodeImp.DoomBuilder.Actions;
using CodeImp.DoomBuilder.Geometry;
using CodeImp.DoomBuilder.Rendering;
using CodeImp.DoomBuilder.Editing;
using System.Collections;
using System.IO;
using CodeImp.DoomBuilder.Map;
using System.Reflection;
using CodeImp.DoomBuilder.Plugins;
using CodeImp.DoomBuilder.Controls;
using CodeImp.DoomBuilder.IO;
using CodeImp.DoomBuilder.Properties;
using CodeImp.DoomBuilder.Config;
using CodeImp.DoomBuilder.Data;
using System.Threading;
using System.Runtime.InteropServices;

#endregion

namespace CodeImp.DoomBuilder.Windows
{
	public partial class MainForm : DelayedForm, IMainForm
	{
		#region ================== Constants
		
		private const string STATUS_READY_TEXT = "Ready.";
		private const int MAX_RECENT_FILES = 8;
		private const int MAX_RECENT_FILES_PIXELS = 250;
		private const int WARNING_FLASH_COUNT = 5;
		
		private enum StatusType : int
		{
			Ready,
			Busy,
			Warning
		}
		
		// Message pump
		public enum ThreadMessages : int
		{
			// Sent by the background threat to update the status icon
			UpdateStatusIcon = General.WM_USER + 1,
			
			// This is sent by the background thread when images are loaded
			// but only when first loaded or when dimensions were changed
			ImageDataLoaded = General.WM_USER + 2
		}
		
		#endregion

		#region ================== Delegates

		private delegate void CallUpdateStatusIcon();
		private delegate void CallImageDataLoaded(ImageData img);

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
		private MouseButtons mousebuttons;
		private MouseInput mouseinput;
		private Rectangle originalclip;
		private bool mouseexclusive;
		private int mouseexclusivebreaklevel;
		
		// Skills
		private ToolStripMenuItem[] skills;
		
		// Recent files
		private ToolStripMenuItem[] recentitems;
		
		// View modes
		private ToolStripButton[] viewmodesbuttons;
		private ToolStripMenuItem[] viewmodesitems;
		
		// Edit modes
		private List<ToolStripItem> editmodeitems;
		
		// Toolbar
		private EventHandler buttonvisiblechangedhandler;
		private bool updatingfilters;
		
		// Statusbar
		private StatusType statustype;
		private int warningflashcount;
		private bool warningsignon;
		
		// Properties
		private IntPtr windowptr;
		
		// Processing
		private double lastupdatetime;
		
		#endregion

		#region ================== Properties

		public bool ShiftState { get { return shift; } }
		public bool CtrlState { get { return ctrl; } }
		public bool AltState { get { return alt; } }
		public MouseButtons MouseButtons { get { return mousebuttons; } }
		public bool MouseInDisplay { get { return mouseinside; } }
		internal RenderTargetControl Display { get { return display; } }
		public bool SnapToGrid { get { return buttonsnaptogrid.Checked; } }
		public bool AutoMerge { get { return buttonautomerge.Checked; } }
		public bool MouseExclusive { get { return mouseexclusive; } }
		new public IntPtr Handle { get { return windowptr; } }
		
		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		internal MainForm()
		{
			// Setup controls
			InitializeComponent();
			editmodeitems = new List<ToolStripItem>();
			
			// Fetch pointer
			windowptr = base.Handle;
			
			// Make array for view modes
			viewmodesbuttons = new ToolStripButton[Renderer2D.NUM_VIEW_MODES];
			viewmodesbuttons[(int)ViewMode.Normal] = buttonviewnormal;
			viewmodesbuttons[(int)ViewMode.Brightness] = buttonviewbrightness;
			viewmodesbuttons[(int)ViewMode.FloorTextures] = buttonviewfloors;
			viewmodesbuttons[(int)ViewMode.CeilingTextures] = buttonviewceilings;
			viewmodesitems = new ToolStripMenuItem[Renderer2D.NUM_VIEW_MODES];
			viewmodesitems[(int)ViewMode.Normal] = itemviewnormal;
			viewmodesitems[(int)ViewMode.Brightness] = itemviewbrightness;
			viewmodesitems[(int)ViewMode.FloorTextures] = itemviewfloors;
			viewmodesitems[(int)ViewMode.CeilingTextures] = itemviewceilings;
			
			// Visual Studio IDE doesn't let me set these in the designer :(
			buttonzoom.Font = menufile.Font;
			buttonzoom.DropDownDirection = ToolStripDropDownDirection.AboveLeft;
			buttongrid.Font = menufile.Font;
			buttongrid.DropDownDirection = ToolStripDropDownDirection.AboveLeft;

			// Event handlers
			buttonvisiblechangedhandler = new EventHandler(ToolbarButtonVisibleChanged);
			
			// Bind any methods
			General.Actions.BindMethods(this);
			
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

		// Editing mode changed!
		internal void EditModeChanged()
		{
			// Check appropriate button on interface
			// And show the mode name
			if(General.Editing.Mode != null)
			{
				General.MainWindow.CheckEditModeButton(General.Editing.Mode.EditModeButtonName);
				General.MainWindow.DisplayModeName(General.Editing.Mode.Attributes.DisplayName);
			}
			else
			{
				General.MainWindow.CheckEditModeButton("");
				General.MainWindow.DisplayModeName("");
			}

			// View mode only matters in classic editing modes
			for(int i = 0; i < Renderer2D.NUM_VIEW_MODES; i++)
			{
				viewmodesitems[i].Enabled = (General.Editing.Mode is ClassicMode);
				viewmodesbuttons[i].Enabled = (General.Editing.Mode is ClassicMode);
			}
		}

		// This makes a beep sound
		public void MessageBeep(MessageBeepType type)
		{
			General.MessageBeep(type);
		}
		
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
			UpdateViewMenu();
			UpdateModeMenu();
			UpdateToolsMenu();
			UpdateToolbar();
			UpdateSkills();
		}
		
		// Generic event that invokes the tagged action
		public void InvokeTaggedAction(object sender, EventArgs e)
		{
			string asmname;
			
			this.Update();
			General.Actions.InvokeAction((sender as ToolStripItem).Tag.ToString());
			this.Update();
		}
		
		#endregion
		
		#region ================== Window
		
		// This sets the focus on the display for correct key input
		public bool FocusDisplay()
		{
			return display.Focus();
		}

		// Window is first shown
		private void MainForm_Shown(object sender, EventArgs e)
		{
			// Perform auto mapo loading action when the window is not delayed
			if(!General.DelayMainWindow) PerformAutoMapLoading();
		}

		// Auto map loading that must be done when the window is first shown after loading
		// but also before the window is shown when the -delaywindow parameter is given
		internal void PerformAutoMapLoading()
		{
			// Check if the command line arguments tell us to load something
			if(General.AutoLoadFile != null)
			{
				bool showdialog = false;
				MapOptions options = new MapOptions();
				Configuration mapsettings;
				
				// Any of the options already given?
				if(General.AutoLoadMap != null)
				{
					// Try to find existing options in the settings file
					string dbsfile = General.AutoLoadFile.Substring(0, General.AutoLoadFile.Length - 4) + ".dbs";
					if(File.Exists(dbsfile))
						try { mapsettings = new Configuration(dbsfile, true); }
						catch(Exception) { mapsettings = new Configuration(true); }
					else
						mapsettings = new Configuration(true);

					// Set map name and other options
					options = new MapOptions(mapsettings, General.AutoLoadMap);

					// Set configuration file (constructor already does this, but we want this info from the cmd args if possible)
					options.ConfigFile = General.AutoLoadConfig;
					if(options.ConfigFile == null) options.ConfigFile = mapsettings.ReadSetting("gameconfig", "");
					if(options.ConfigFile.Trim().Length == 0) showdialog = true;
				}
				else
				{
					// No options given
					showdialog = true;
				}

				// Show open map dialog?
				if(showdialog)
				{
					// Show open dialog
					General.OpenMapFile(General.AutoLoadFile);
				}
				else
				{
					// Open with options
					General.OpenMapFileWithOptions(General.AutoLoadFile, options);
				}
			}
		}

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

		// Window receives focus
		private void MainForm_Activated(object sender, EventArgs e)
		{
			// Resume any exclusive mouse input
			ResumeExclusiveMouseInput();
			display.Focus();
		}
		
		// Window loses focus
		private void MainForm_Deactivate(object sender, EventArgs e)
		{
			// Release all pressed keys
			General.Actions.ReleaseAllKeys();
			mousebuttons = MouseButtons.None;

			// Stop exclusive mouse input
			BreakExclusiveMouseInput();
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

			if(e.CloseReason != CloseReason.ApplicationExitCall)
			{
				// Close the map
				if(General.CloseMap())
				{
					General.WriteLogLine("Closing main interface window...");
					
					// Hide warning to stop timers
					HideWarning();

					// Stop exclusive mode, if any is active
					StopExclusiveMouseInput();
					SetProcessorState(false);

					// Unbind methods
					General.Actions.UnbindMethods(this);

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
				else
				{
					// Cancel the close
					e.Cancel = true;
				}
			}
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
				configlabel.Text = General.Map.Config.Name;
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
				configlabel.Text = "";
			}
			
			UpdateStatusIcon();
		}
		
		// This returns the current status text
		internal string GetCurrentSatus()
		{
			if(statustype == StatusType.Busy)
				return statuslabel.Text;
			else
				return null;
		}
		
		// This shows a warning
		public void DisplayWarning(string warning)
		{
			MessageBeep(MessageBeepType.Warning);
			if(statuslabel.Text != warning) statuslabel.Text = warning;
			statustype = StatusType.Warning;
			statuslabel.Image = Resources.Warning;
			warningflashcount = 0;
			warningsignon = true;
			warningtimer.Stop();
			warningtimer.Interval = 3000;
			warningtimer.Start();
			warningflasher.Start();
			
			// Refresh if needed
			statusbar.Invalidate();
			this.Update();
		}
		
		// This hides any warning
		public void HideWarning()
		{
			if(statustype == StatusType.Warning)
			{
				warningtimer.Stop();
				warningflasher.Stop();
				DisplayReady();
			}
		}
		
		// This flashes the warning sign
		private void warningflasher_Tick(object sender, EventArgs e)
		{
			if(statustype == StatusType.Warning)
			{
				// Warning sign on?
				if(warningsignon)
				{
					// Turn it off or should we stop?
					if(warningflashcount < WARNING_FLASH_COUNT)
					{
						statuslabel.Image = Resources.WarningOff;
						warningsignon = false;
					}
					else
					{
						warningflasher.Stop();
					}
				}
				else
				{
					// Turn it on and count the flash
					statuslabel.Image = Resources.Warning;
					warningsignon = true;
					warningflashcount++;
				}
			}
		}
		
		// Warning timed out
		private void warningtimer_Tick(object sender, EventArgs e)
		{
			HideWarning();
		}
		
		// This changes status text
		public void DisplayStatus(string status)
		{
			// Null is no busy status
			if(status == null)
			{
				DisplayReady();
			}
			else
			{
				// Stop warning timers
				warningtimer.Stop();
				warningflasher.Stop();

				// Update status description
				statustype = StatusType.Busy;
				if(statuslabel.Text != status)
					statuslabel.Text = status;

				// Update icon as well
				UpdateStatusIcon();

				// Refresh if needed
				statusbar.Invalidate();
				this.Update();
			}
		}

		// This changes status text to Ready
		public void DisplayReady()
		{
			// Stop warning timers
			warningtimer.Stop();
			warningflasher.Stop();

			// Display ready status description
			statustype = StatusType.Ready;
			if(statuslabel.Text != STATUS_READY_TEXT)
				statuslabel.Text = STATUS_READY_TEXT;
			
			// Update icon as well
			UpdateStatusIcon();
			
			// Refresh if needed
			statusbar.Invalidate();
			this.Update();
		}
		
		// This updates the status icon
		internal void UpdateStatusIcon()
		{
			// From another thread?
			if(statusbar.InvokeRequired)
			{
				// Call to form thread
				// This may not be called from a different thread. Send a WM_NOTIFY message instead!
				throw new Exception("This should not be called from any other thread than the main application thread!");
			}
			else
			{
				// Ready status?
				if(statustype == StatusType.Ready)
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
				else if(statustype == StatusType.Busy)
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
			if(General.Editing.Mode is ClassicMode)
			{
				// Requested from menu?
				if(sender is ToolStripMenuItem)
				{
					// Get integral zoom level
					zoom = int.Parse((sender as ToolStripMenuItem).Tag.ToString(), CultureInfo.InvariantCulture);

					// Zoom now
					(General.Editing.Mode as ClassicMode).SetZoom((float)zoom / 100f);
				}
			}
		}

		// Zoom to fit in screen
		private void itemzoomfittoscreen_Click(object sender, EventArgs e)
		{
			if(General.Map == null) return;
			
			// In classic mode?
			if(General.Editing.Mode is ClassicMode)
				(General.Editing.Mode as ClassicMode).CenterInScreen();
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
			if(General.Editing.Mode is ClassicMode)
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
			if(General.Map == null) return;

			General.Map.Grid.ShowGridSetup();
		}
		
		#endregion

		#region ================== Display

		// This shows the splash screen on display
		internal void ShowSplashDisplay()
		{
			// Change display to show splash logo
			display.SetSplashLogoDisplay();
			display.Cursor = Cursors.Default;
			this.Update();
		}
		
		// This clears the display
		internal void ClearDisplay()
		{
			// Clear the display
			display.SetManualRendering();
			this.Update();
		}

		// This sets the display cursor
		public void SetCursor(Cursor cursor)
		{
			// Only when a map is open
			if(General.Map != null) display.Cursor = cursor;
		}

		// This redraws the display on the next paint event
		public void RedrawDisplay()
		{
			if((General.Map != null) && (General.Editing.Mode != null))
			{
				General.Editing.Mode.OnRedrawDisplay();
			}
			else
			{
				display.Invalidate();
			}
		}

		// This event is called when a repaint is needed
		private void display_Paint(object sender, PaintEventArgs e)
		{
			if(General.Map != null)
			{
				if(General.Editing.Mode != null)
				{
					if(!displayresized) General.Editing.Mode.OnPresentDisplay();
				}
				else
				{
					if(General.Colors != null)
						e.Graphics.Clear(Color.FromArgb(General.Colors.Background.ToInt()));
					else
						e.Graphics.Clear(SystemColors.AppWorkspace);
				}
			}
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
		
		// This requests a delayed redraw
		public void DelayedRedraw()
		{
			// Request redraw
			if(!redrawtimer.Enabled) redrawtimer.Enabled = true;
		}
		
		// Mouse click
		private void display_MouseClick(object sender, MouseEventArgs e)
		{
			if((General.Map != null) && (General.Editing.Mode != null)) General.Editing.Mode.OnMouseClick(e);
		}

		// Mouse doubleclick
		private void display_MouseDoubleClick(object sender, MouseEventArgs e)
		{
			if((General.Map != null) && (General.Editing.Mode != null)) General.Editing.Mode.OnMouseDoubleClick(e);
		}

		// Mouse down
		private void display_MouseDown(object sender, MouseEventArgs e)
		{
			int key = 0;
			
			// Apply button
			mousebuttons |= e.Button;
			
			// Create key
			switch(e.Button)
			{
				case MouseButtons.Left: key = (int)Keys.LButton; break;
				case MouseButtons.Middle: key = (int)Keys.MButton; break;
				case MouseButtons.Right: key = (int)Keys.RButton; break;
				case MouseButtons.XButton1: key = (int)Keys.XButton1; break;
				case MouseButtons.XButton2: key = (int)Keys.XButton2; break;
			}
			
			// Invoke any actions associated with this key
			General.Actions.KeyPressed(key);
			
			// Invoke on editing mode
			if((General.Map != null) && (General.Editing.Mode != null)) General.Editing.Mode.OnMouseDown(e);
		}

		// Mouse enters
		private void display_MouseEnter(object sender, EventArgs e)
		{
			mouseinside = true;
			if((General.Map != null) && (mouseinput == null) && (General.Editing.Mode != null)) General.Editing.Mode.OnMouseEnter(e);
		}

		// Mouse leaves
		private void display_MouseLeave(object sender, EventArgs e)
		{
			mouseinside = false;
			if((General.Map != null) && (mouseinput == null) && (General.Editing.Mode != null)) General.Editing.Mode.OnMouseLeave(e);
		}

		// Mouse moves
		private void display_MouseMove(object sender, MouseEventArgs e)
		{
			if((General.Map != null) && (mouseinput == null) && (General.Editing.Mode != null)) General.Editing.Mode.OnMouseMove(e);
		}

		// Mouse up
		private void display_MouseUp(object sender, MouseEventArgs e)
		{
			int key = 0;

			// Apply button
			mousebuttons &= ~e.Button;
			
			// Create key
			switch(e.Button)
			{
				case MouseButtons.Left: key = (int)Keys.LButton; break;
				case MouseButtons.Middle: key = (int)Keys.MButton; break;
				case MouseButtons.Right: key = (int)Keys.RButton; break;
				case MouseButtons.XButton1: key = (int)Keys.XButton1; break;
				case MouseButtons.XButton2: key = (int)Keys.XButton2; break;
			}

			// Invoke any actions associated with this key
			General.Actions.KeyReleased(key);

			// Invoke on editing mode
			if((General.Map != null) && (General.Editing.Mode != null)) General.Editing.Mode.OnMouseUp(e);
		}
		
		#endregion

		#region ================== Input

		// This checks if a given action is active
		public bool CheckActionActive(Assembly assembly, string actionname)
		{
			if(assembly == null)
				return General.Actions.CheckActionActive(General.ThisAssembly, actionname);
			else
				return General.Actions.CheckActionActive(assembly, actionname);
		}
		
		// This is a tool to lock the mouse in exclusive mode
		private void StartMouseExclusive()
		{
			// Not already locked?
			if(mouseinput == null)
			{
				// Start special input device
				mouseinput = new MouseInput(this);

				// Lock and hide the mouse in window
				originalclip = Cursor.Clip;
				Cursor.Clip = display.RectangleToScreen(display.ClientRectangle);
				Cursor.Hide();
			}
		}

		// This is a tool to unlock the mouse
		private void StopMouseExclusive()
		{
			// Locked?
			if(mouseinput != null)
			{
				// Stop special input device
				mouseinput.Dispose();
				mouseinput = null;

				// Release and show the mouse
				Cursor.Clip = originalclip;
				Cursor.Position = display.PointToScreen(new Point(display.ClientSize.Width / 2, display.ClientSize.Height / 2));
				Cursor.Show();
			}
		}
		
		// This requests exclusive mouse input
		public void StartExclusiveMouseInput()
		{
			// Only when not already in exclusive mode
			if(!mouseexclusive)
			{
				General.WriteLogLine("Starting exclusive mouse input mode...");
				
				// Start special input device
				StartMouseExclusive();
				mouseexclusive = true;
				mouseexclusivebreaklevel = 0;
			}
		}
		
		// This stops exclusive mouse input
		public void StopExclusiveMouseInput()
		{
			// Only when in exclusive mode
			if(mouseexclusive)
			{
				General.WriteLogLine("Stopping exclusive mouse input mode...");

				// Stop special input device
				StopMouseExclusive();
				mouseexclusive = false;
				mouseexclusivebreaklevel = 0;
			}
		}

		// This temporarely breaks exclusive mode and counts the break level
		public void BreakExclusiveMouseInput()
		{
			// Only when in exclusive mode
			if(mouseexclusive)
			{
				// Stop special input device
				StopMouseExclusive();
				
				// Count the break level
				mouseexclusivebreaklevel++;
			}
		}

		// This resumes exclusive mode from a break when all breaks have been called to resume
		public void ResumeExclusiveMouseInput()
		{
			// Only when in exclusive mode
			if(mouseexclusive && (mouseexclusivebreaklevel > 0))
			{
				// Decrease break level
				mouseexclusivebreaklevel--;

				// All break levels resumed? Then lock the mouse again.
				if(mouseexclusivebreaklevel == 0)
					StartMouseExclusive();
			}
		}
		
		// When the mouse wheel is changed
		protected override void OnMouseWheel(MouseEventArgs e)
		{
			// Scrollwheel up?
			if(e.Delta > 0)
			{
				// Invoke actions for scrollwheel
				//for(int i = 0; i < e.Delta; i += 120)
				General.Actions.KeyPressed((int)SpecialKeys.MScrollUp);
				General.Actions.KeyReleased((int)SpecialKeys.MScrollUp);
			}
			// Scrollwheel down?
			else if(e.Delta < 0)
			{
				// Invoke actions for scrollwheel
				//for(int i = 0; i > e.Delta; i -= 120)
				General.Actions.KeyPressed((int)SpecialKeys.MScrollDown);
				General.Actions.KeyReleased((int)SpecialKeys.MScrollDown);
			}
			
			// Let the base know
			base.OnMouseWheel(e);
		}
		
		// When a key is pressed
		private void MainForm_KeyDown(object sender, KeyEventArgs e)
		{
			int mod = 0;
			
			// Keep key modifiers
			alt = e.Alt;
			shift = e.Shift;
			ctrl = e.Control;
			if(alt) mod |= (int)Keys.Alt;
			if(shift) mod |= (int)Keys.Shift;
			if(ctrl) mod |= (int)Keys.Control;
			
			// Invoke any actions associated with this key
			General.Actions.UpdateModifiers(mod);
			General.Actions.KeyPressed((int)e.KeyData);

			// Invoke on editing mode
			if((General.Map != null) && (General.Editing.Mode != null)) General.Editing.Mode.OnKeyDown(e);
		}

		// When a key is released
		private void MainForm_KeyUp(object sender, KeyEventArgs e)
		{
			int mod = 0;

			// Keep key modifiers
			alt = e.Alt;
			shift = e.Shift;
			ctrl = e.Control;
			if(alt) mod |= (int)Keys.Alt;
			if(shift) mod |= (int)Keys.Shift;
			if(ctrl) mod |= (int)Keys.Control;

			// Invoke any actions associated with this key
			General.Actions.UpdateModifiers(mod);
			General.Actions.KeyReleased((int)e.KeyData);

			// Invoke on editing mode
			if((General.Map != null) && (General.Editing.Mode != null)) General.Editing.Mode.OnKeyUp(e);
		}

		#endregion

		#region ================== Toolbar
		
		// This updates the skills list
		private void UpdateSkills()
		{
			// Clear list
			buttontest.DropDownItems.Clear();
			
			// Map loaded?
			if(General.Map != null)
			{
				// Make the new skills list
				skills = new ToolStripMenuItem[General.Map.Config.Skills.Count];
				for(int i = 0; i < General.Map.Config.Skills.Count; i++)
				{
					skills[i] = new ToolStripMenuItem(General.Map.Config.Skills[i].ToString());
					skills[i].Image = buttontest.Image;
					skills[i].Click += new EventHandler(TestSkill_Click);
					skills[i].Tag = General.Map.Config.Skills[i].Index;
				}
				
				// Add to list
				buttontest.DropDownItems.AddRange(skills);
			}
		}
		
		// Event handler for testing at a specific skill
		private void TestSkill_Click(object sender, EventArgs e)
		{
			int skill = (int)((sender as ToolStripMenuItem).Tag);
			General.Map.Launcher.TestAtSkill(skill);
		}

		// Toggle monster testing
		private void buttontestmonsters_Click(object sender, EventArgs e)
		{
			General.Settings.TestMonsters = !General.Settings.TestMonsters;
			buttontestmonsters.Checked = General.Settings.TestMonsters;
		}
		
		// This loses focus
		private void LoseFocus(object sender, EventArgs e)
		{
			// Lose focus!
			this.ActiveControl = null;
		}

		// Things filter selected
		private void thingfilters_SelectedIndexChanged(object sender, EventArgs e)
		{
			// Only possible when a map is open
			if((General.Map != null) && !updatingfilters)
			{
				// Change filter
				General.Map.ChangeThingFilter(thingfilters.SelectedItem as ThingsFilter);
			}

			// Lose focus
			LoseFocus(sender, e);
		}
		
		// This updates the things filter on the toolbar
		internal void UpdateThingsFilters()
		{
			// Only possible to list filters when a map is open
			if(General.Map != null)
			{
				ThingsFilter oldfilter = null;
				if(thingfilters.SelectedIndex > -1)
					oldfilter = thingfilters.SelectedItem as ThingsFilter;
				
				updatingfilters = true;

				// Clear the list
				thingfilters.Items.Clear();

				// Add null filter
				if(General.Map.ThingsFilter is NullThingsFilter)
					thingfilters.Items.Add(General.Map.ThingsFilter);
				else
					thingfilters.Items.Add(new NullThingsFilter());

				// Add all filters
				foreach(ThingsFilter f in General.Map.ConfigSettings.ThingsFilters)
					thingfilters.Items.Add(f);

				// Select current filter
				foreach(ThingsFilter f in thingfilters.Items)
					if(f == General.Map.ThingsFilter) thingfilters.SelectedItem = f;

				updatingfilters = false;
				
				// No filter selected?
				if(thingfilters.SelectedIndex == -1)
				{
					// Select the first and update
					thingfilters.SelectedIndex = 0;
				}
				// Another filter got selected?
				else if(oldfilter != (thingfilters.SelectedItem as ThingsFilter))
				{
					// Update!
					thingfilters_SelectedIndexChanged(this, EventArgs.Empty);
				}
			}
			else
			{
				// Clear the list
				thingfilters.Items.Clear();
			}
		}
		
		// This adds a button to the toolbar
		public void AddButton(ToolStripItem button)
		{
			// Find the plugin that called this method
			Plugin plugin = General.Plugins.FindPluginByAssembly(Assembly.GetCallingAssembly());

			// Fix tags to full action names
			ToolStripItemCollection items = new ToolStripItemCollection(toolbar, new ToolStripItem[0]);
			items.Add(button);
			RenameTagsToFullActions(items, plugin);
			
			// Bind visible changed event
			if(!(button is ToolStripSeparator)) button.VisibleChanged += buttonvisiblechangedhandler;
			
			// Insert the button at the end of the toolbar
			toolbar.Items.Add(button);
			UpdateSeperators();
		}

		// Removes a button
		public void RemoveButton(ToolStripItem button)
		{
			// Unbind visible changed event
			if(!(button is ToolStripSeparator)) button.VisibleChanged -= buttonvisiblechangedhandler;

			// Remove button
			toolbar.Items.Remove(button);
			UpdateSeperators();
		}

		// This handle visibility changes in the toolbar buttons
		private void ToolbarButtonVisibleChanged(object sender, EventArgs e)
		{
			// Update the seeprators
			UpdateSeperators();
		}

		// This updates the seperators
		// Hides redundant seperators and shows single seperators
		private void UpdateSeperators()
		{
			ToolStripItem pvi = null;
			foreach(ToolStripItem i in toolbar.Items)
			{
				// This is a seperator?
				if(i is ToolStripSeparator)
				{
					// Make visible when previous item was not a seperator
					i.Visible = !(pvi is ToolStripSeparator);
				}

				// Keep as previous visible item
				if(i.Visible) pvi = i;
			}

			// Hide last item if it is a seperator
			if(pvi is ToolStripSeparator) pvi.Visible = false;
		}
		
		// This enables or disables all editing mode items and toolbar buttons
		private void UpdateToolbar()
		{
			// Enable/disable all edit mode items
			foreach(ToolStripItem i in editmodeitems) i.Enabled = (General.Map != null);

			// Update buttons
			buttontestmonsters.Enabled = (General.Map != null);
			buttontestmonsters.Checked = General.Settings.TestMonsters;
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
		internal void RemoveEditModeButtons()
		{
			// Go for all items
			foreach(ToolStripItem i in editmodeitems)
			{
				// Remove it and restart
				toolbar.Items.Remove(i);
				menumode.DropDownItems.Remove(i);
			}
			
			// Done
			editmodeitems.Clear();
			UpdateSeperators();
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
			UpdateSeperators();
			
			// Create menu item
			index = menumode.DropDownItems.Count;
			item = new ToolStripMenuItem(modeinfo.ButtonDesc, modeinfo.ButtonImage, new EventHandler(EditModeButtonHandler));
			item.Tag = modeinfo;
			item.Enabled = (General.Map != null);
			menumode.DropDownItems.Insert(index, item);
			editmodeitems.Add(item);
			ApplyShortcutKeys(menumode.DropDownItems);
		}

		// This handles edit mode button clicks
		private void EditModeButtonHandler(object sender, EventArgs e)
		{
			EditModeInfo modeinfo;
			
			this.Update();
			modeinfo = (EditModeInfo)((sender as ToolStripItem).Tag);
			General.Actions.InvokeAction(modeinfo.SwitchAction.GetFullActionName(modeinfo.Plugin.Assembly));
			this.Update();
		}

		#endregion

		#region ================== Menus

		// This adds a menu to the menus bar
		public void AddMenu(ToolStripMenuItem menu)
		{
			// Find the plugin that called this method
			Plugin plugin = General.Plugins.FindPluginByAssembly(Assembly.GetCallingAssembly());

			// Fix tags to full action names
			RenameTagsToFullActions(menu.DropDownItems, plugin);
			
			// Insert the menu before the Tools menu
			menumain.Items.Insert(menumain.Items.IndexOf(menutools), menu);
			ApplyShortcutKeys(menu.DropDownItems);
		}
		
		// Removes a menu
		public void RemoveMenu(ToolStripMenuItem menu)
		{
			menumain.Items.Remove(menu);
		}

		// Public method to apply shortcut keys
		internal void ApplyShortcutKeys()
		{
			// Apply shortcut keys to menus
			ApplyShortcutKeys(menumain.Items);
		}
		
		// This sets the shortcut keys on menu items
		private void ApplyShortcutKeys(ToolStripItemCollection items)
		{
			// Go for all controls to find menu items
			foreach(ToolStripItem item in items)
			{
				// This is a menu item?
				if(item is ToolStripMenuItem)
				{
					// Get the item in proper type
					ToolStripMenuItem menuitem = (item as ToolStripMenuItem);

					// Tag set for this item?
					if(menuitem.Tag is string)
					{
						// Action with this name available?
						string actionname = menuitem.Tag.ToString();
						if(General.Actions.Exists(actionname))
						{
							// Put the action shortcut key on the menu item
							menuitem.ShortcutKeyDisplayString = Action.GetShortcutKeyDesc(General.Actions[actionname].ShortcutKey);
						}
					}
					// Edit mode info set for this item?
					else if(menuitem.Tag is EditModeInfo)
					{
						// Action with this name available?
						EditModeInfo modeinfo = (menuitem.Tag as EditModeInfo);
						string actionname = modeinfo.SwitchAction.GetFullActionName(modeinfo.Plugin.Assembly);
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

		// This fixes short action names to fully qualified
		// action names on menu item tags
		private void RenameTagsToFullActions(ToolStripItemCollection items, Plugin plugin)
		{
			// Go for all controls to find menu items
			foreach(ToolStripItem item in items)
			{
				// Tag set for this item?
				if((item.Tag != null) && (item.Tag is string))
				{
					// Get the action name
					string actionname = item.Tag.ToString();

					// Check if the tag doe not already begin with the assembly name
					if(!(item.Tag as string).StartsWith(plugin.Name + "_", StringComparison.InvariantCultureIgnoreCase))
					{
						// Change the tag to a fully qualified action name
						item.Tag = plugin.Name.ToLowerInvariant() + "_" + (item.Tag as string);
					}
				}

				// This is a menu item?
				if(item is ToolStripMenuItem)
				{
					// Get the item in proper type
					ToolStripMenuItem menuitem = (item as ToolStripMenuItem);
					
					// Recursively perform operation on child menu items
					RenameTagsToFullActions(menuitem.DropDownItems, plugin);
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
			itemtestmap.Enabled = (General.Map != null);

			// Toolbar icons
			buttonnewmap.Enabled = itemnewmap.Enabled;
			buttonopenmap.Enabled = itemopenmap.Enabled;
			buttonsavemap.Enabled = itemsavemap.Enabled;
			buttontest.Enabled = itemtestmap.Enabled;
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
			itemcut.Enabled = (General.Map != null) && (General.Editing.Mode != null) && General.Editing.Mode.Attributes.AllowCopyPaste;
			itemcopy.Enabled = (General.Map != null) && (General.Editing.Mode != null) && General.Editing.Mode.Attributes.AllowCopyPaste;
			itempaste.Enabled = (General.Map != null) && (General.Editing.Mode != null) && General.Editing.Mode.Attributes.AllowCopyPaste;
			itemmapoptions.Enabled = (General.Map != null);
			itemsnaptogrid.Enabled = (General.Map != null);
			itemautomerge.Enabled = (General.Map != null);
			itemgridsetup.Enabled = (General.Map != null);
			itemgridinc.Enabled = (General.Map != null);
			itemgriddec.Enabled = (General.Map != null);

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
			buttoncut.Enabled = itemcut.Enabled;
			buttoncopy.Enabled = itemcopy.Enabled;
			buttonpaste.Enabled = itempaste.Enabled;
		}

		// Action to toggle snap to grid
		[BeginAction("togglesnap")]
		internal void ToggleSnapToGrid()
		{
			buttonsnaptogrid.Checked = !buttonsnaptogrid.Checked;
			itemsnaptogrid.Checked = buttonsnaptogrid.Checked;
		}

		// Action to toggle auto merge
		[BeginAction("toggleautomerge")]
		internal void ToggleAutoMerge()
		{
			buttonautomerge.Checked = !buttonautomerge.Checked;
			itemautomerge.Checked = buttonautomerge.Checked;
		}
		
		#endregion

		#region ================== View Menu

		// This sets up the modes menu
		private void UpdateViewMenu()
		{
			// Menu items
			itemthingsfilter.Enabled = (General.Map != null);
			itemscripteditor.Enabled = (General.Map != null);
			
			// View mode items
			for(int i = 0; i < Renderer2D.NUM_VIEW_MODES; i++)
			{
				// NOTE: We only disable them when no map is loaded, because they may
				// need to be disabled for non-classic modes
				if(General.Map == null)
				{
					viewmodesbuttons[i].Enabled = false;
					viewmodesbuttons[i].Checked = false;
					viewmodesitems[i].Enabled = false;
					viewmodesitems[i].Checked = false;
				}
				else
				{
					// Check the correct item
					viewmodesbuttons[i].Checked = (i == (int)General.Map.CRenderer2D.ViewMode);
					viewmodesitems[i].Checked = (i == (int)General.Map.CRenderer2D.ViewMode);
				}
			}
			
			// Toolbar icons
			thingfilters.Enabled = (General.Map != null);
			buttonthingsfilter.Enabled = (General.Map != null);
			buttonscripteditor.Enabled = (General.Map != null);
			UpdateThingsFilters();
		}

		#endregion

		#region ================== Mode Menu

		// This sets up the modes menu
		private void UpdateModeMenu()
		{
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
		}
		
		// Game Configuration action
		[BeginAction("configuration")]
		internal void ShowConfiguration()
		{
			// Show configuration dialog
			ShowConfigurationPage(-1);
		}

		// This shows the configuration on a specific page
		internal void ShowConfigurationPage(int pageindex)
		{
			// Show configuration dialog
			ConfigForm cfgform = new ConfigForm();
			if(pageindex > -1) cfgform.ShowTab(pageindex);
			if(cfgform.ShowDialog(this) == DialogResult.OK)
			{
				// Update interface
				UpdateInterface();

				// Let the plugins know
				General.Plugins.ProgramReconfigure();

				// Reload resources if a map is open
				if(General.Map != null) General.Actions.InvokeAction("builder_reloadresources");

				// Redraw display
				RedrawDisplay();
			}

			// Done
			cfgform.Dispose();
		}

		// Preferences action
		[BeginAction("preferences")]
		internal void ShowPreferences()
		{
			// Show preferences dialog
			PreferencesForm prefform = new PreferencesForm();
			if(prefform.ShowDialog(this) == DialogResult.OK)
			{
				// Update shortcut keys in menus
				ApplyShortcutKeys();
				
				// Generate new color correction table
				General.Colors.CreateCorrectionTable();
				
				// Let the plugins know
				General.Plugins.ProgramReconfigure();

				// Map opened?
				if(General.Map != null)
				{
					// Reload resources!
					if(General.Map.ScriptEditor != null) General.Map.ScriptEditor.Editor.RefreshSettings();
					General.Map.Graphics.SetupSettings();
					General.Map.UpdateConfiguration();
					General.Actions.InvokeAction("builder_reloadresources");
				}
				
				// Redraw display
				RedrawDisplay();
			}

			// Done
			prefform.Dispose();
		}
		
		#endregion
		
		#region ================== Info Panels

		// This displays the current mode name
		internal void DisplayModeName(string name)
		{
			modename.Text = name;
			modename.Refresh();
		}
		
		// This hides all info panels
		public void HideInfo()
		{
			// Hide them all
			if(linedefinfo.Visible) linedefinfo.Hide();
			if(vertexinfo.Visible) vertexinfo.Hide();
			if(sectorinfo.Visible) sectorinfo.Hide();
			if(thinginfo.Visible) thinginfo.Hide();
			modename.Visible = (General.Map != null);
			modename.Refresh();
		}
		
		// Show linedef info
		public void ShowLinedefInfo(Linedef l)
		{
			modename.Visible = false;
			linedefinfo.ShowInfo(l);
		}

		// Show vertex info
		public void ShowVertexInfo(Vertex v)
		{
			modename.Visible = false;
			vertexinfo.ShowInfo(v);
		}

		// Show sector info
		public void ShowSectorInfo(Sector s)
		{
			modename.Visible = false;
			sectorinfo.ShowInfo(s);
		}

		// Show thing info
		public void ShowThingInfo(Thing t)
		{
			modename.Visible = false;
			thinginfo.ShowInfo(t);
		}

		#endregion

		#region ================== Dialogs

		// This browses for a texture
		// Returns the new texture name or the same texture name when cancelled
		public string BrowseTexture(IWin32Window owner, string initialvalue)
		{
			return TextureBrowserForm.Browse(owner, initialvalue);
		}

		// This browses for a flat
		// Returns the new flat name or the same flat name when cancelled
		public string BrowseFlat(IWin32Window owner, string initialvalue)
		{
			return FlatBrowserForm.Browse(owner, initialvalue);
		}
		
		// This browses the lindef types
		// Returns the new action or the same action when cancelled
		public int BrowseLinedefActions(IWin32Window owner, int initialvalue)
		{
			return ActionBrowserForm.BrowseAction(owner, initialvalue);
		}

		// This browses sector effects
		// Returns the new effect or the same effect when cancelled
		public int BrowseSectorEffect(IWin32Window owner, int initialvalue)
		{
			return EffectBrowserForm.BrowseEffect(owner, initialvalue);
		}
		
		// This shows the dialog to edit lines
		public DialogResult ShowEditLinedefs(ICollection<Linedef> lines)
		{
			DialogResult result;

			// Show line edit dialog
			LinedefEditForm f = new LinedefEditForm();
			f.Setup(lines);
			result = f.ShowDialog(this);
			f.Dispose();

			return result;
		}

		// This shows the dialog to edit sectors
		public DialogResult ShowEditSectors(ICollection<Sector> sectors)
		{
			DialogResult result;

			// Show sector edit dialog
			SectorEditForm f = new SectorEditForm();
			f.Setup(sectors);
			result = f.ShowDialog(this);
			f.Dispose();

			return result;
		}

		// This shows the dialog to edit things
		public DialogResult ShowEditThings(ICollection<Thing> things)
		{
			DialogResult result;

			// Show thing edit dialog
			ThingEditForm f = new ThingEditForm();
			f.Setup(things);
			result = f.ShowDialog(this);
			f.Dispose();
			
			return result;
		}

		#endregion

		#region ================== Message Pump
		
		// This handles messages
		protected override unsafe void WndProc(ref Message m)
		{
			// Notify message?
			switch(m.Msg)
			{
				case (int)ThreadMessages.UpdateStatusIcon:
					UpdateStatusIcon();
					break;
					
				case (int)ThreadMessages.ImageDataLoaded:
					string imagename = Marshal.PtrToStringAuto(m.WParam);
					Marshal.FreeCoTaskMem(m.WParam);
					if(General.Map != null)
					{
						ImageData img = General.Map.Data.GetFlatImage(imagename);
						if(img != null) ImageDataLoaded(img);
					}
					break;
					
				default:
					// Let the base handle the message
					base.WndProc(ref m);
					break;
			}
		}
		
		#endregion
		
		#region ================== Processing
		
		// This is called from the background thread when images are loaded
		// but only when first loaded or when dimensions were changed
		internal void ImageDataLoaded(ImageData img)
		{
			// Image is used in the map?
			if((img != null) && img.UsedInMap && !img.IsDisposed)
			{
				// Go for all setors
				bool updated = false;
				foreach(Sector s in General.Map.Map.Sectors)
				{
					// Update floor buffer if needed
					if(s.LongFloorTexture == img.LongName)
					{
						s.UpdateFloorSurface();
						updated = true;
					}
					
					// Update ceiling buffer if needed
					if(s.LongCeilTexture == img.LongName)
					{
						s.UpdateCeilingSurface();
						updated = true;
					}
				}
				
				// If we made updates, redraw the screen
				if(updated) DelayedRedraw();
			}
		}

		// This toggles the processor
		public void SetProcessorState(bool on)
		{
			processor.Enabled = on;
			if(on) lastupdatetime = General.Clock.GetCurrentTime();
		}
		
		// Processor event
		private void processor_Tick(object sender, EventArgs e)
		{
			Vector2D deltamouse;
			double curtime = General.Clock.GetCurrentTime();
			double deltatime = curtime - lastupdatetime;
			lastupdatetime = curtime;
			
			// In exclusive mouse mode?
			if(mouseinput != null)
			{
				// Process mouse input
				deltamouse = mouseinput.Process();
				if((General.Map != null) && (General.Editing.Mode != null))
					General.Editing.Mode.OnMouseInput(deltamouse);
			}
			
			// Process signal
			if((General.Map != null) && (General.Editing.Mode != null))
				General.Editing.Mode.OnProcess(deltatime);
		}

		#endregion
	}
}