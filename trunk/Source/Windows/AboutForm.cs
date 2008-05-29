
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
using System.Reflection;
using CodeImp.DoomBuilder.Controls;

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
			string postfix = "";
			if(General.DebugBuild) postfix = "(debug)";
			version.Text = Application.ProductName + " version " + Application.ProductVersion + " " + postfix;
		}

		// Launch Doom Builder website
		private void builderlink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			OpenWebsite("http://" + builderlink.Text);
		}

		// Clicking SlimDX logo
		private void slimdxlogo_Click(object sender, EventArgs e)
		{
			OpenWebsite("http://slimdx.mdxinfo.com/");
		}

		// This opens a URL in the default browser
		private void OpenWebsite(string url)
		{
			RegistryKey key = null;
			Process p = null;
			string browser;

			try
			{
				// Get the registry key where default browser is stored
				key = Registry.ClassesRoot.OpenSubKey(@"HTTP\shell\open\command", false);

				// Trim off quotes
				browser = key.GetValue(null).ToString().ToLower().Replace("\"", "");

				// String doesnt end in EXE?
				if(!browser.EndsWith("exe"))
				{
					// Get rid of everything after the ".exe"
					browser = browser.Substring(0, browser.LastIndexOf(".exe") + 4);
				}
			}
			finally
			{
				// Clean up
				if(key != null) key.Close();
			}

			try
			{
				// Fork a process
				p = new Process();
				p.StartInfo.FileName = browser;
				p.StartInfo.Arguments = url;
				p.Start();
			}
			catch(Exception) { }

			// Clean up
			if(p != null) p.Dispose();
		}
	}
}