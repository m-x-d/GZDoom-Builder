
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
using CodeImp.DoomBuilder.Windows;
using System.Reflection;
using System.Globalization;

#endregion

namespace CodeImp.DoomBuilder.BuilderModes
{
	public partial class ErrorCheckForm : DelayedForm
	{
		#region ================== Constructor / Show

		// Constructor
		public ErrorCheckForm()
		{
			InitializeComponent();
		}

		// This shows the window
		public void Show(Form owner)
		{
			// First time showing?
			//if((this.Location.X == 0) && (this.Location.Y == 0))
			{
				// Position at left-top of owner
				this.Location = new Point(owner.Location.X + 20, owner.Location.Y + 90);
			}

			// Close results part
			//resultspanel.Visible = false;
			//this.Size = new Size(this.Width, this.Height - this.ClientSize.Height + resultspanel.Top);

			// Show window
			base.Show(owner);
		}
		
		#endregion

		#region ================== Events

		// Window closing
		private void ErrorCheckForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			// If the user closes the form, then just cancel the mode
			if(e.CloseReason == CloseReason.UserClosing)
			{
				e.Cancel = true;
				General.Map.CancelMode();
			}
		}

		#endregion

		#region ================== Methods

		#endregion
	}
}