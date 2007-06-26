
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

// This Form is a workaround for the slow drawing of the .NET Forms.
// By showing the Form at 0% Opacity it allows the .NET framework to complete
// drawing the Form first, then we set it to 100% Opacity to actually show it.

namespace CodeImp.DoomBuilder.Interface
{
	public class DelayedForm : Form
	{
		// Variables
		private Timer formshowtimer;
		
		// Constructor
		public DelayedForm()
		{
			// Create a timer that we need to show the form
			formshowtimer = new Timer();
			formshowtimer.Interval = 1;
			formshowtimer.Tick += new EventHandler(formshowtimer_Tick);
		}
		
		// When form is shown
		protected override void OnShown(EventArgs e)
		{
			// Let the base class know
			base.OnShown(e);

			// Start the timer to show the form
			formshowtimer.Enabled = true;
		}

		// When the form is to be shown
		private void formshowtimer_Tick(object sender, EventArgs e)
		{
			// Get rid of the timer
			formshowtimer.Dispose();
			formshowtimer = null;
			
			// Make the form visible
			this.Opacity = 100;
		}
	}
}
