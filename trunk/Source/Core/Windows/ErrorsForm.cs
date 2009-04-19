
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

#endregion

namespace CodeImp.DoomBuilder.Windows
{
	public partial class ErrorsForm : Form
	{
		#region ================== Variables

		#endregion
		
		#region ================== Constructor / Disposer

		// Constructor
		public ErrorsForm()
		{
			InitializeComponent();
			list.Items.Clear();
			FillList();
			checkerrors.Start();
			checkshow.Checked = General.Settings.ShowErrorsWindow;
		}

		#endregion

		#region ================== Methods

		// This sets up the list
		private void FillList()
		{
			// Fill the list with the items we don't have yet
			list.BeginUpdate();
			General.ErrorLogger.HasChanged = false;
			List<ErrorItem> errors = General.ErrorLogger.GetErrors();
			int startindex = list.Items.Count;
			for(int i = startindex; i < errors.Count; i++)
			{
				ErrorItem e = errors[i];
				int icon = (e.type == ErrorType.Error) ? 0 : 1;
				ListViewItem item = new ListViewItem(e.message, icon);
				list.Items.Add(item);
			}
			list.EndUpdate();
		}

		#endregion
		
		#region ================== Events

		// Close clicked
		private void close_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		// Closing
		private void ErrorsForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			checkerrors.Stop();
			General.Settings.ShowErrorsWindow = checkshow.Checked;
		}

		// Checking for more errors
		private void checkerrors_Tick(object sender, EventArgs e)
		{
			// If errors have been added, update the list
			if(General.ErrorLogger.HasChanged)
			{
				FillList();
			}
		}

		// This clears all errors
		private void clearlist_Click(object sender, EventArgs e)
		{
			General.ErrorLogger.Clear();
			list.Items.Clear();
		}
		
		// Copy selection
		private void copyselected_Click(object sender, EventArgs e)
		{
			StringBuilder str = new StringBuilder("");
			if(list.SelectedItems.Count > 0)
			{
				Clipboard.Clear();
				foreach(ListViewItem lvi in list.SelectedItems)
				{
					if(str.Length > 0) str.Append("\r\n");
					str.Append(lvi.Text);
				}
				Clipboard.SetText(str.ToString());
			}
		}

		// Help requested
		private void ErrorsForm_HelpRequested(object sender, HelpEventArgs hlpevent)
		{
			General.ShowHelp("w_errorsandwarnings.html");
			hlpevent.Handled = true;
		}
		
		#endregion
	}
}