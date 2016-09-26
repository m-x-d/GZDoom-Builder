
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
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using CodeImp.DoomBuilder.Actions;
using Action = CodeImp.DoomBuilder.Actions.Action;

#endregion

// This Form is a workaround for the slow drawing of the .NET Forms.
// By showing the Form at 0% Opacity it allows the .NET framework to complete
// drawing the Form first, then we set it to 100% Opacity to actually show it.
// To use this class properly, set the initial Opacity of your Form to 0.

namespace CodeImp.DoomBuilder.Windows
{
	public class DelayedForm : Form
	{
		// Variables
		private Timer formshowtimer;
		protected readonly string configname; //mxd
		
		// Constructor
		protected DelayedForm()
		{
			//mxd. And now, let's hope this doesn't horribly break anything...
			if(!(this is MainForm))
			{
				this.KeyPreview = true;
				this.KeyUp += OnKeyUp;
			}

			//mxd
			configname = this.GetType().Name.ToLowerInvariant();
			
			// Create a timer that we need to show the form
			formshowtimer = new Timer { Interval = 1 };
			formshowtimer.Tick += formshowtimer_Tick;
		}
		
		// When form is shown
		protected override void OnShown(EventArgs e)
		{
			//mxd
			if(this.DesignMode) return;
			
			//mxd. Restore location and size
			this.SuspendLayout();

			// Restore location
			Point validlocation = Point.Empty;
			Point location = new Point(General.Settings.ReadSetting("windows." + configname + ".positionx", int.MaxValue),
									   General.Settings.ReadSetting("windows." + configname + ".positiony", int.MaxValue));

			if(location.X < int.MaxValue && location.Y < int.MaxValue)
			{
				// Location withing screen bounds?
				Rectangle bounds = new Rectangle(location, this.Size);
				bounds.Inflate(16, 16); // Add some safety padding
				if(SystemInformation.VirtualScreen.IntersectsWith(bounds))
				{
					validlocation = location;
				}
			}

			if(validlocation == Point.Empty && !(this is MainForm))
			{
				// Do the manual CenterParent...
				validlocation = new Point(General.MainWindow.Location.X + General.MainWindow.Width / 2 - this.Width / 2,
										  General.MainWindow.Location.Y + General.MainWindow.Height / 2 - this.Height / 2);
			}

			// Apply location
			if(validlocation == Point.Empty)
			{
				this.StartPosition = FormStartPosition.CenterParent;
			}
			else
			{
				this.StartPosition = FormStartPosition.Manual;
				this.Location = validlocation;
			}

			// Restore windowstate
			if(this.MaximizeBox)
			{
				this.WindowState = (FormWindowState)General.Settings.ReadSetting("windows." + configname + ".windowstate", (int)FormWindowState.Normal);
			}

			// Form size matters?
			if(this.WindowState == FormWindowState.Normal
				&& (this.FormBorderStyle == FormBorderStyle.Sizable || this.FormBorderStyle == FormBorderStyle.SizableToolWindow))
			{
				this.Size = new Size(General.Settings.ReadSetting("windows." + configname + ".sizewidth", this.Size.Width),
									 General.Settings.ReadSetting("windows." + configname + ".sizeheight", this.Size.Height));
			}

			this.ResumeLayout();
			//mxd end

			// Let the base class know
			base.OnShown(e);

			// Start the timer to show the form
			formshowtimer.Enabled = true;

			// Bind any methods (mxd)
			if(!DesignMode) General.Actions.BindMethods(this);
		}

		//mxd. When form is closing
		protected override void OnClosing(CancelEventArgs e)
		{
			if(e.Cancel) return;
			
			// Let the base class know
			base.OnClosing(e);

			// Determine window state to save
			if(this.MaximizeBox)
			{
				int windowstate;
				if(this.WindowState != FormWindowState.Minimized)
					windowstate = (int)this.WindowState;
				else
					windowstate = (int)FormWindowState.Normal;

				General.Settings.WriteSetting("windows." + configname + ".windowstate", windowstate);
			}

			// Form size matters?
			if(this.FormBorderStyle == FormBorderStyle.Sizable || this.FormBorderStyle == FormBorderStyle.SizableToolWindow)
			{
				General.Settings.WriteSetting("windows." + configname + ".sizewidth", this.Size.Width);
				General.Settings.WriteSetting("windows." + configname + ".sizeheight", this.Size.Height);
			}

			// Save location
			General.Settings.WriteSetting("windows." + configname + ".positionx", this.Location.X);
			General.Settings.WriteSetting("windows." + configname + ".positiony", this.Location.Y);
		}

		// When the form is to be shown
		private void formshowtimer_Tick(object sender, EventArgs e)
		{
			// Get rid of the timer
			formshowtimer.Dispose();
			formshowtimer = null;
			
			if(!this.IsDisposed)
			{
				// Make the form visible
				this.Opacity = 1.0;
			}
		}

		//mxd. Special handling to call "save screenshot" actions from any form, 
		//which inherits form DelayedForm and either doesn't override OnKeyUp, or calls base.OnKeyUp
		private void OnKeyUp(object sender, KeyEventArgs e)
		{
			e.Handled = (Form.ActiveForm == this && ProcessSaveScreenshotAction((int)e.KeyData));
			if(e.Handled) e.SuppressKeyPress = true;
		}

		// mxd. Handle screenshot saving from any form
		private static bool ProcessSaveScreenshotAction(int key)
		{
			Action[] actions = General.Actions.GetActionsByKey(key);
			foreach(Action action in actions) 
			{
				if(action.ShortName == "savescreenshot" || action.ShortName == "saveeditareascreenshot")
				{
					General.Actions.InvokeAction(action.Name);
					return true;
				}
			}

			return false;
		}

		//mxd
		[EndAction("savescreenshot", BaseAction = true)]
		internal void SaveScreenshot() 
		{
			if(Form.ActiveForm == this) General.MainWindow.SaveScreenshot(false);
		}

		//mxd
		[EndAction("saveeditareascreenshot", BaseAction = true)]
		internal void SaveEditAreaScreenshot() 
		{
			if(Form.ActiveForm == this) General.MainWindow.SaveScreenshot(true);
		}
	}
}
