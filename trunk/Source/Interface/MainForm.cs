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

		#region ================== Constructor / Disposer

		// Constructor
		public MainForm()
		{
			// Setup controls
			InitializeComponent();
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