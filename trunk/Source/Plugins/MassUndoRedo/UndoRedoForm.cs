
#region ================== Copyright (c) 2009 Pascal vd Heiden

/*
 * Copyright (c) 2009 Pascal vd Heiden, www.codeimp.com
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
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CodeImp.DoomBuilder.Windows;
using CodeImp.DoomBuilder.Editing;

#endregion

namespace CodeImp.DoomBuilder.MassUndoRedo
{
	public partial class UndoRedoForm : DelayedForm
	{
		#region ================== Variables
		
		private bool ignoreevents;
		private int currentselection;
		private bool ignorefirstevent;
		
		#endregion
		
		#region ================== Constructor
		
		// Constructor
		public UndoRedoForm()
		{
			InitializeComponent();
			ignorefirstevent = true;
		}
		
		#endregion
		
		#region ================== Methods
		
		// This sets up the form for display
		public void Setup(Form owner)
		{
			List<UndoSnapshot> levels;
			
			ignoreevents = true;
			
			// Position window on the left side of the main window
			SizeF scalefactor = new SizeF(owner.CurrentAutoScaleDimensions.Width / owner.AutoScaleDimensions.Width,
										  owner.CurrentAutoScaleDimensions.Height / owner.AutoScaleDimensions.Height);
			int topoffset = SystemInformation.CaptionHeight + (int)(80 * scalefactor.Height);
			this.Location = new Point(owner.Location.X + (int)(20 * scalefactor.Width), owner.Location.Y + topoffset);
			this.Height = owner.Height - (topoffset + (int)(50 * scalefactor.Height));
			
			// Reset the list
			list.Items.Clear();
			list.Items.Add("Begin");
			
			// Add undo levels
			levels = General.Map.UndoRedo.GetUndoList();
			levels.Reverse();
			foreach(UndoSnapshot u in levels)
			{
				ListViewItem item = list.Items.Add(u.Description);
			}
			
			// Select the last undo level: that's where we currently are at.
			list.Items[list.Items.Count - 1].Selected = true;
			currentselection = list.Items.Count - 1;
			
			// Add redo levels
			levels = General.Map.UndoRedo.GetRedoList();
			foreach(UndoSnapshot r in levels)
			{
				ListViewItem item = list.Items.Add(r.Description);
				item.ForeColor = SystemColors.GrayText;
				item.BackColor = SystemColors.Control;
			}
			
			ignoreevents = false;
		}
		
		#endregion
		
		#region ================== Events
		
		// Item selected
		private void list_SelectedIndexChanged(object sender, EventArgs e)
		{
			if(ignoreevents) return;
			if(ignorefirstevent)
			{
				ignorefirstevent = false;
				return;
			}
			
			ignoreevents = true;
			
			// We must have something selected
			if(list.SelectedIndices.Count > 0)
			{
				// Not the same as last selected?
				int selectedindex = list.SelectedIndices[0];
				if(selectedindex != currentselection)
				{
					// Perform the undo/redos
					int delta = currentselection - selectedindex;
					if(delta < 0)
						General.Map.UndoRedo.PerformRedo(-delta);
					else
						General.Map.UndoRedo.PerformUndo(delta);
					
					// Update list
					list.BeginUpdate();
					foreach(ListViewItem item in list.Items)
					{
						if(item.Index <= selectedindex)
						{
							item.ForeColor = SystemColors.WindowText;
							item.BackColor = SystemColors.Window;
						}
						else
						{
							item.ForeColor = SystemColors.GrayText;
							item.BackColor = SystemColors.Control;
						}
					}
					list.EndUpdate();
					currentselection = selectedindex;
				}
			}
			
			ignoreevents = false;
		}
		
		// Mouse released
		private void list_MouseUp(object sender, MouseEventArgs e)
		{
			ignoreevents = true;
			
			// If selection was removed, then keep the last selected
			if(list.SelectedIndices.Count == 0)
				list.Items[currentselection].Selected = true;
			
			ignoreevents = false;
		}
		
		// Key released
		private void list_KeyUp(object sender, KeyEventArgs e)
		{
			ignoreevents = true;

			// If selection was removed, then keep the last selected
			if(list.SelectedIndices.Count == 0)
				list.Items[currentselection].Selected = true;

			ignoreevents = false;
		}
		
		#endregion
	}
}