
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
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Data;
using CodeImp.DoomBuilder.IO;
using System.IO;
using CodeImp.DoomBuilder.Config;
using CodeImp.DoomBuilder.Editing;
using CodeImp.DoomBuilder.Geometry;
using CodeImp.DoomBuilder.Controls;

#endregion

namespace CodeImp.DoomBuilder.Windows
{
	public partial class ThingsFiltersForm : DelayedForm
	{
		#region ================== Variables

		private bool settingup;

		#endregion
		
		#region ================== Constructor

		// Constructor
		public ThingsFiltersForm()
		{
			settingup = true;
			
			// Initialize
			InitializeComponent();

			// Fill the categories combobox
			filtercategory.Items.Add("(any category)");
			filtercategory.Items.AddRange(General.Map.Config.ThingCategories.ToArray());

			// Fill checkboxes list
			foreach(KeyValuePair<string, string> flag in General.Map.Config.ThingFlags)
			{
				CheckBox box = filterfields.Add(flag.Value, flag.Key);
				box.ThreeState = true;
				box.CheckStateChanged += new EventHandler(filterfield_Check);
			}
			
			// Fill list of filters
			foreach(ThingsFilter f in General.Map.ConfigSettings.ThingsFilters)
			{
				// Make a copy (we don't want to modify the filters until OK is clicked)
				ThingsFilter nf = new ThingsFilter(f);

				// Make item in list
				ListViewItem item = new ListViewItem(nf.Name);
				item.Tag = nf;
				listfilters.Items.Add(item);

				// Select item if this is the current filter
				if(General.Map.ThingsFilter == f) item.Selected = true;
			}

			// Sort the list
			listfilters.Sort();

			// Done
			settingup = false;
		}

		#endregion

		#region ================== Management

		// OK clicked
		private void apply_Click(object sender, EventArgs e)
		{
			// Clear all filters and add the new ones
			General.Map.ConfigSettings.ThingsFilters.Clear();
			foreach(ListViewItem item in listfilters.Items)
				General.Map.ConfigSettings.ThingsFilters.Add(item.Tag as ThingsFilter);
			
			// Update stuff
			General.Map.ChangeThingFilter(new NullThingsFilter());
			General.MainWindow.UpdateThingsFilters();
			
			// Close
			this.DialogResult = DialogResult.OK;
			this.Close();
		}

		// Cancel clicked
		private void cancel_Click(object sender, EventArgs e)
		{
			// Close
			this.DialogResult = DialogResult.Cancel;
			this.Close();
		}

		// New Filter clicked
		private void addfilter_Click(object sender, EventArgs e)
		{
			ThingsFilter newf = new ThingsFilter();

			// Make item in list and select it
			ListViewItem item = new ListViewItem(newf.Name);
			item.Tag = newf;
			listfilters.Items.Add(item);
			item.Selected = true;

			// Focus on the name field
			filtername.Focus();
			filtername.SelectAll();
		}

		// Delete Selected clicked
		private void deletefilter_Click(object sender, EventArgs e)
		{
			// Anything selected?
			if(listfilters.SelectedItems.Count > 0)
			{
				// Remove item
				listfilters.Items.Remove(listfilters.SelectedItems[0]);
			}
		}
		
		// Item selected
		private void listfilters_SelectedIndexChanged(object sender, EventArgs e)
		{
			// Anything selected?
			if(listfilters.SelectedItems.Count > 0)
			{
				// Get selected filter
				ThingsFilter f = listfilters.SelectedItems[0].Tag as ThingsFilter;

				// Enable settings
				settingup = true;
				deletefilter.Enabled = true;
				filtergroup.Enabled = true;

				// Show name
				filtername.Text = f.Name;
				
				// Show category
				foreach(object c in filtercategory.Items)
				{
					ThingCategory tc = (c as ThingCategory);
					if((tc != null) && (tc.Name == f.CategoryName)) filtercategory.SelectedItem = tc;
				}
				if(filtercategory.SelectedIndex == -1) filtercategory.SelectedIndex = 0;
				
				// Show fields
				foreach(CheckBox b in filterfields.Checkboxes)
				{
					// Field name forbidden?
					if(f.ForbiddenFields.Contains(b.Tag.ToString()))
					{
						b.CheckState = CheckState.Unchecked;
					}
					// Field name required?
					else if(f.RequiredFields.Contains(b.Tag.ToString()))
					{
						b.CheckState = CheckState.Checked;
					}
					else
					{
						b.CheckState = CheckState.Indeterminate;
					}
				}

				// Done
				settingup = false;
			}
			else
			{
				// Disable filter settings
				deletefilter.Enabled = false;
				filtergroup.Enabled = false;
				filtername.Text = "";
				filtercategory.SelectedIndex = -1;
				foreach(CheckBox c in filterfields.Checkboxes) c.CheckState = CheckState.Indeterminate;
			}
		}

		#endregion	

		#region ================== Filter Settings

		// Category changed
		private void filtercategory_SelectedIndexChanged(object sender, EventArgs e)
		{
			// Anything selected?
			if(listfilters.SelectedItems.Count > 0)
			{
				// Get selected filter
				ThingsFilter f = listfilters.SelectedItems[0].Tag as ThingsFilter;
				
				// Category selected
				if((filtercategory.SelectedIndex > -1) && (filtercategory.SelectedItem is ThingCategory))
				{
					// Set new category name
					f.CategoryName = (filtercategory.SelectedItem as ThingCategory).Name;
				}
				else
				{
					// Unset category name
					f.CategoryName = "";
				}
			}
		}
		
		// Rename filter
		private void filtername_Validating(object sender, CancelEventArgs e)
		{
			// Anything selected?
			if(listfilters.SelectedItems.Count > 0)
			{
				// Get selected filter
				ThingsFilter f = listfilters.SelectedItems[0].Tag as ThingsFilter;

				// Name changed?
				if(f.Name != filtername.Text)
				{
					// Update name
					f.Name = filtername.Text;
					listfilters.SelectedItems[0].Text = f.Name;
					listfilters.Sort();
				}
			}
		}

		// Field clicked
		private void filterfield_Check(object sender, EventArgs e)
		{
			// Get the checkbox
			CheckBox box = (sender as CheckBox);
			
			// Not setting up?
			if(!settingup)
			{
				// Anything selected?
				if(listfilters.SelectedItems.Count > 0)
				{
					// Get selected filter
					ThingsFilter f = listfilters.SelectedItems[0].Tag as ThingsFilter;
					
					// New state is required?
					if(box.CheckState == CheckState.Checked)
					{
						f.ForbiddenFields.Remove(box.Tag.ToString());
						if(!f.RequiredFields.Contains(box.Tag.ToString())) f.RequiredFields.Add(box.Tag.ToString());
					}
					// New state is forbidden?
					else if(box.CheckState == CheckState.Unchecked)
					{
						f.RequiredFields.Remove(box.Tag.ToString());
						if(!f.ForbiddenFields.Contains(box.Tag.ToString())) f.ForbiddenFields.Add(box.Tag.ToString());
					}
					else
					{
						f.ForbiddenFields.Remove(box.Tag.ToString());
						f.RequiredFields.Remove(box.Tag.ToString());
					}
				}
			}
		}
		
		#endregion
	}
}