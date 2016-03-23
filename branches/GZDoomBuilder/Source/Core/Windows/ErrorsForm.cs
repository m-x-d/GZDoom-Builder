
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
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

#endregion

namespace CodeImp.DoomBuilder.Windows
{
	public partial class ErrorsForm : DelayedForm
	{
		#region ================== Variables

		//mxd. Window setup stuff
		private static Point location = Point.Empty;
		private static Size size = Size.Empty;

		#endregion
		
		#region ================== Constructor / Disposer

		// Constructor
		public ErrorsForm()
		{
			InitializeComponent();

			//mxd. Apply window settings
			if(location != Point.Empty)
			{
				this.StartPosition = FormStartPosition.Manual;
				this.Location = location;
			}
			if(size != Size.Empty) this.Size = size;

			FillList();
			checkerrors.Start();
			checkshow.Checked = General.Settings.ShowErrorsWindow;
			grid.Focus(); //mxd
		}

		#endregion

		#region ================== Methods

		// This sets up the list
		private void FillList()
		{
			// Fill the list with the items we don't have yet
			General.ErrorLogger.HasChanged = false;

			//mxd. Rewritten to get only the new items from the ErrorLogger
			int startindex = grid.Rows.Count;
			IEnumerable<ErrorItem> errors = General.ErrorLogger.GetErrors(startindex);
			foreach(ErrorItem e in errors)
			{
				Image icon = (e.type == ErrorType.Error) ? Properties.Resources.ErrorLarge : Properties.Resources.WarningLarge;
				int index = grid.Rows.Add();
				DataGridViewRow row = grid.Rows[index];
				row.Cells[0].Value = icon;
				row.Cells[0].Style.Alignment = DataGridViewContentAlignment.TopCenter;
				row.Cells[0].Style.Padding = new Padding(0, 5, 0, 0);
				row.Cells[1].Value = e.message;
				row.Cells[1].Style.WrapMode = DataGridViewTriState.True;
			}
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

			//mxd. Save window settings
			location = this.Location;
			size = this.Size;
		}

		// Checking for more errors
		private void checkerrors_Tick(object sender, EventArgs e)
		{
			// If errors have been added, update the list
			if(General.ErrorLogger.HasChanged) FillList();
		}

		// This clears all errors
		private void clearlist_Click(object sender, EventArgs e)
		{
			General.ErrorLogger.Clear();
			grid.Rows.Clear();
		}
		
		// Copy selection
		private void copyselected_Click(object sender, EventArgs e)
		{
			StringBuilder str = new StringBuilder("");
			if(grid.SelectedCells.Count > 0)
			{
				Clipboard.Clear();
				foreach(DataGridViewCell c in grid.SelectedCells)
				{
					if(c.ValueType != typeof(Image))
					{
						if(str.Length > 0) str.Append("\r\n");
						str.Append(c.Value);
					}
				}

				//mxd
				try 
				{
					Clipboard.SetDataObject(str.ToString(), true);
				} 
				catch(ExternalException) 
				{
					General.Interface.DisplayStatus(StatusType.Warning, "Failed to perform a Clipboard operation...");
				}
			}
		}

		// Help requested
		private void ErrorsForm_HelpRequested(object sender, HelpEventArgs hlpevent)
		{
			General.ShowHelp("w_errorsandwarnings.html");
			hlpevent.Handled = true;
		}
		
		#endregion

		private void ErrorsForm_Shown(object sender, EventArgs e)
		{
			if(grid.Rows.Count > 0) grid.Rows[0].Selected = false;
		}

		private void grid_CellContentClick(object sender, DataGridViewCellEventArgs e) 
		{
			copyselected.Enabled = true;
		}
	}
}