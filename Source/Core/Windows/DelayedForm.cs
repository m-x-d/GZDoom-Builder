
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
		
		// Constructor
		public DelayedForm()
		{
			this.Opacity = 0; //mxd
			
			//mxd. And now, let's hope this doesn't horribly break anything...
			if(!(this is MainForm))
			{
				this.KeyPreview = true;
				this.KeyUp += OnKeyUp;
			}
			
			// Create a timer that we need to show the form
			formshowtimer = new Timer();
			formshowtimer.Interval = 1;
			formshowtimer.Tick += formshowtimer_Tick;
		}
		
		// When form is shown
		protected override void OnShown(EventArgs e)
		{
			// Let the base class know
			base.OnShown(e);

			// Start the timer to show the form
			formshowtimer.Enabled = true;

			// Bind any methods (mxd)
			if(!DesignMode) General.Actions.BindMethods(this);
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
				this.Opacity = 100;
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
		internal static bool ProcessSaveScreenshotAction(int key)
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
