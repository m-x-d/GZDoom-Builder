
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

#endregion

namespace CodeImp.DoomBuilder.Windows
{
	internal partial class AboutForm : DelayedForm
	{
		// Constructor
		public AboutForm()
		{
			// Initialize
			InitializeComponent();

			// Show version
			version.Text = Application.ProductName + " v" + Application.ProductVersion + (General.DebugBuild ? " (debug)" : "");
		}

		// Launch Doom Builder website
		private void builderlink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			General.OpenWebsite("http://" + builderlink.Text);
		}

		// Launch CodeImp website
		private void codeimplink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			General.OpenWebsite("http://" + codeimplink.Text);
		}

		//mxd
		private void gzdbLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
			General.OpenWebsite("http://forum.zdoom.org/viewtopic.php?f=3&t=32392");
		}

		// This copies the version number to clipboard
		private void copyversion_Click(object sender, EventArgs e)
		{
			Clipboard.Clear();
			Clipboard.SetText(Application.ProductVersion);
		}
	}
}