
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
	public partial class FindReplaceForm : DelayedForm
	{
		#region ================== Constants
		
		// Constants
		private const int RESULTS_WINDOW_HEIGHT = 447;

		#endregion

		#region ================== Variables

		private FindReplaceType finder;

		#endregion

		#region ================== Properties

		internal FindReplaceType Finder { get { return finder; } }

		#endregion

		#region ================== Constructor / Show

		// Constructor
		public FindReplaceForm()
		{
			// Initialize
			InitializeComponent();

			// Find all find/replace types
			Type[] findtypes = BuilderPlug.Me.FindClasses(typeof(FindReplaceType));
			foreach(Type t in findtypes)
			{
				FindReplaceType finderinst;
				object[] attr = t.GetCustomAttributes(typeof(FindReplaceAttribute), true);
				if(attr.Length > 0)
				{
					try
					{
						// Create instance
						finderinst = (FindReplaceType)Assembly.GetExecutingAssembly().CreateInstance(t.FullName, false, BindingFlags.Default, null, null, CultureInfo.CurrentCulture, new object[0]);
					}
					catch(TargetInvocationException ex)
					{
						// Error!
						General.WriteLogLine("ERROR: Failed to create class instance '" + t.Name + "'!");
						General.WriteLogLine(ex.InnerException.GetType().Name + ": " + ex.InnerException.Message);
						throw ex;
					}
					catch(Exception ex)
					{
						// Error!
						General.WriteLogLine("ERROR: Failed to create class instance '" + t.Name + "'!");
						General.WriteLogLine(ex.GetType().Name + ": " + ex.Message);
						throw ex;
					}

					// Add the finder to the list
					searchtypes.Items.Add(finderinst);
				}
			}

			// Select first
			searchtypes.SelectedIndex = 0;
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
			resultspanel.Visible = false;
			this.Size = new Size(this.Width, this.Height - this.ClientSize.Height + resultspanel.Top);
			
			// Show window
			base.Show(owner);
		}
		
		#endregion

		#region ================== Events

		// Replace (un)checked
		private void doreplace_CheckedChanged(object sender, EventArgs e)
		{
			if(doreplace.Checked)
			{
				findbutton.Text = "Replace";
				groupreplace.Enabled = true;
			}
			else
			{
				findbutton.Text = "Find";
				groupreplace.Enabled = false;
			}
		}

		// Search type selected
		private void searchtypes_SelectedIndexChanged(object sender, EventArgs e)
		{
			// Create instance of the selected type
			finder  = (FindReplaceType)searchtypes.SelectedItem;
			
			// Now setup the interface
			browsefind.Enabled = finder.Attributes.BrowseButton;
			browsereplace.Enabled = finder.Attributes.BrowseButton;
		}

		// Browse find button clicked
		private void browsefind_Click(object sender, EventArgs e)
		{
			findinput.Text = finder.Browse(findinput.Text);
		}

		// Browse replacement clicked
		private void browsereplace_Click(object sender, EventArgs e)
		{
			replaceinput.Text = finder.Browse(replaceinput.Text);
		}

		// Find / Replace clicked
		private void findbutton_Click(object sender, EventArgs e)
		{
			// Reset results
			resultslist.Items.Clear();

			// Hide object information
			General.Interface.HideInfo();

			// Perform the search / replace and show the results
			if(doreplace.Checked)
			{
				resultslist.Items.AddRange(finder.Find(findinput.Text, withinselection.Checked, replaceinput.Text, keepselection.Checked));
				resultscount.Text = resultslist.Items.Count + " items found and replaced.";
			}
			else
			{
				resultslist.Items.AddRange(finder.Find(findinput.Text, withinselection.Checked, null, false));
				resultscount.Text = resultslist.Items.Count + " items found.";
			}
			
			// Open results part of window
			this.Size = new Size(this.Width, this.Height - this.ClientSize.Height + resultspanel.Top + resultspanel.Height);
			resultspanel.Visible = true;
			
			// Redraw the screen, this will show the selection
			General.Interface.RedrawDisplay();
		}

		// Found item selected
		private void resultslist_SelectedIndexChanged(object sender, EventArgs e)
		{
			// Anything selected?
			if(resultslist.SelectedIndex > -1)
			{
				// Let the finder know
				finder.ObjectSelected((FindReplaceObject)resultslist.SelectedItem);
				
				// Redraw the screen, this will show the selection
				General.Interface.RedrawDisplay();
			}
		}

		// Window closing
		private void FindReplaceForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			// If the user closes the form, then just cancel the mode
			if(e.CloseReason == CloseReason.UserClosing)
			{
				e.Cancel = true;
				General.Map.CancelMode();
			}
		}

		// Close button clicked
		private void closebutton_Click(object sender, EventArgs e)
		{
			General.Map.CancelMode();
		}

		#endregion

		#region ================== Methods

		// This returns the selected item(s)
		internal FindReplaceObject[] GetSelection()
		{
			// Anything selected?
			if(resultslist.SelectedIndex > -1)
			{
				// Return selected object
				FindReplaceObject[] list = new FindReplaceObject[1];
				list[0] = (FindReplaceObject)resultslist.SelectedItem;
				return list;
			}
			else
			{
				// Return all objects
				FindReplaceObject[] list = new FindReplaceObject[resultslist.Items.Count];
				resultslist.Items.CopyTo(list, 0);
				return list;
			}
		}
		
		#endregion
	}
}