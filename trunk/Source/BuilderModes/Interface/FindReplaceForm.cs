
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
		
		#endregion

		#region ================== Variables

		private FindReplaceType newfinder;
		private FindReplaceType finder;
		bool controlpressed = false;
		bool shiftpressed = false;
		bool suppressevents = false;
		
		#endregion

		#region ================== Properties

		internal FindReplaceType Finder { get { return finder; } }

		#endregion

		#region ================== Constructor

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
			newfinder = (FindReplaceType)searchtypes.SelectedItem;
			
			// Now setup the interface
			browsefind.Enabled = newfinder.Attributes.BrowseButton;
			browsereplace.Enabled = newfinder.Attributes.BrowseButton;
		}

		// Browse find button clicked
		private void browsefind_Click(object sender, EventArgs e)
		{
			findinput.Text = newfinder.Browse(findinput.Text);
		}

		// Browse replacement clicked
		private void browsereplace_Click(object sender, EventArgs e)
		{
			replaceinput.Text = newfinder.Browse(replaceinput.Text);
		}

		// Find / Replace clicked
		private void findbutton_Click(object sender, EventArgs e)
		{
			// Reset results
			resultslist.Items.Clear();
			
			// Hide object information
			General.Interface.HideInfo();
			General.Map.Map.ClearAllSelected();
			
			// Keep the finder we used for the search
			finder = newfinder;
			
			// Enable/disable buttons
			editbutton.Enabled = false;
			deletebutton.Visible = finder.AllowDelete;
			deletebutton.Enabled = false;
			suppressevents = true;
			resultslist.BeginUpdate();
			
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
			
			// Select all results
			for(int i = 0; i < resultslist.Items.Count; i++)
				resultslist.SelectedIndices.Add(i);
			
			// Open results part of window
			this.Size = new Size(this.Width, this.Height - this.ClientSize.Height + resultspanel.Top + resultspanel.Height);
			resultspanel.Visible = true;
			resultslist.EndUpdate();
			suppressevents = false;
			
			// Redraw the screen, this will show the selection
			General.Interface.RedrawDisplay();
		}
		
		// Found item selected
		private void resultslist_SelectedIndexChanged(object sender, EventArgs e)
		{
			if(!suppressevents)
			{
				// Let the finder know about the selection
				FindReplaceObject[] selection = GetSelection();
				finder.ObjectSelected(selection);
				
				// Enable/disable buttons
				editbutton.Enabled = (resultslist.SelectedIndex > -1);
				deletebutton.Enabled = (resultslist.SelectedIndex > -1);

				// Redraw the screen, this will show the selection
				General.Interface.RedrawDisplay();
			}
		}

		// Mouse released
		private void resultslist_MouseUp(object sender, MouseEventArgs e)
		{
			// Right-clicked?
			if(e.Button == MouseButtons.Right)
			{
				int index = resultslist.IndexFromPoint(e.Location);
				if(index != ListBox.NoMatches)
				{
					// Select the right-clicked item, if not selected
					if(!resultslist.SelectedIndices.Contains(index))
					{
						if(!controlpressed && !shiftpressed) resultslist.SelectedItems.Clear();
						resultslist.SelectedIndices.Add(index);
						Update();
					}

					// Edit selected objects
					editbutton_Click(this, EventArgs.Empty);
				}
			}
			
			// Enable/disable buttons
			editbutton.Enabled = (resultslist.SelectedIndex > -1);
			deletebutton.Enabled = (resultslist.SelectedIndex > -1);
		}
		
		// Window closing
		private void FindReplaceForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			// If the user closes the form, then just cancel the mode
			if(e.CloseReason == CloseReason.UserClosing)
			{
				e.Cancel = true;
				General.Editing.CancelMode();
			}
		}

		// Close button clicked
		private void closebutton_Click(object sender, EventArgs e)
		{
			General.Interface.Focus();
			General.Editing.CancelMode();
		}

		// Key pressed
		private void FindReplaceForm_KeyDown(object sender, KeyEventArgs e)
		{
			// Keep modifiers
			controlpressed = e.Control;
			shiftpressed = e.Shift;

			// Enable/disable buttons
			editbutton.Enabled = (resultslist.SelectedIndex > -1);
			deletebutton.Enabled = (resultslist.SelectedIndex > -1);
		}

		// Key released
		private void FindReplaceForm_KeyUp(object sender, KeyEventArgs e)
		{
			// Keep modifiers
			controlpressed = e.Control;
			shiftpressed = e.Shift;

			// Enable/disable buttons
			editbutton.Enabled = (resultslist.SelectedIndex > -1);
			deletebutton.Enabled = (resultslist.SelectedIndex > -1);
		}

		// Edit Selection
		private void editbutton_Click(object sender, EventArgs e)
		{
			FindReplaceObject[] items = GetSelection();
			if(items.Length > 0)
			{
				// Edit selected objects
				suppressevents = true;
				finder.EditObjects(items);
				suppressevents = false;
				resultslist_SelectedIndexChanged(sender, e);
				
				// Redraw the screen, this will show the selection
				General.Interface.RedrawDisplay();
			}
		}

		// Delete Selection
		private void deletebutton_Click(object sender, EventArgs e)
		{
			// Delete selected objects
			FindReplaceObject[] items = GetSelection();
			if(items.Length > 0)
			{
				suppressevents = true;
				finder.DeleteObjects(items);
				foreach(FindReplaceObject obj in items)
				{
					resultslist.Items.Remove(obj);
				}
				suppressevents = false;
				resultslist_SelectedIndexChanged(sender, e);
				
				// Redraw the screen, this will show the selection
				General.Interface.RedrawDisplay();
			}
		}
		
		#endregion

		#region ================== Methods

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

		// This hides the window
		new public void Hide()
		{
			base.Hide();
			
			// Clear search results
			resultslist.Items.Clear();
		}
		
		// This returns the selected item(s)
		internal FindReplaceObject[] GetSelection()
		{
			// Return selected objects
			FindReplaceObject[] list = new FindReplaceObject[resultslist.SelectedItems.Count];
			int index = 0;
			foreach(object obj in resultslist.SelectedItems)
			{
				list[index++] = (FindReplaceObject)obj;
			}
			return list;
		}
		
		#endregion
	}
}