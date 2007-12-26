
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
using System.Drawing.Drawing2D;
using CodeImp.DoomBuilder.Config;

#endregion

namespace CodeImp.DoomBuilder.Interface
{
	public partial class ActionSelectorControl : UserControl
	{
		// Constants
		private const string NUMBER_SEPERATOR = "\t";
		
		// Properties
		public bool Empty { get { return (number.Text.Length == 0); } set { if(value) number.Text = ""; } }
		public int Value { get { if(number.Text.Length > 0) return int.Parse(number.Text); else return 0; } set { number.Text = value.ToString(); } }
		
		// Constructor
		public ActionSelectorControl()
		{
			// Initialize
			InitializeComponent();
		}

		// This adds information to display
		public void AddInfo(INumberedTitle[] infolist)
		{
			// Add to list
			list.Items.AddRange(infolist);
		}

		// Resized
		private void ActionSelectorControl_Resize(object sender, EventArgs e)
		{
			list.Width = ClientRectangle.Width - list.Left;
			ClientSize = new Size(ClientSize.Width, list.Height);
		}

		// Layout change
		private void ActionSelectorControl_Layout(object sender, LayoutEventArgs e)
		{
			ActionSelectorControl_Resize(sender, e);
		}

		// This draws an item in the combobox
		private void list_DrawItem(object sender, DrawItemEventArgs e)
		{
			INumberedTitle item;
			Brush displaybrush;
			Brush backbrush;
			string displayname;
			
			// Unknow item?
			if(e.Index < 0)
			{
				// Grayed
				displaybrush = new SolidBrush(SystemColors.GrayText);
				backbrush = new SolidBrush(SystemColors.Window);
				
				// Check what to display
				if(number.Text.Length == 0)
					displayname = "";
				else if(number.Text == "0")
					displayname = "None";
				else
					displayname = "Unknown";
			}
			// In the display part of the combobox?
			else if((e.State & DrawItemState.ComboBoxEdit) != 0)
			{
				// Show without number
				item = (INumberedTitle)list.Items[e.Index];
				displayname = item.Title.Trim();
				
				// Determine colors to use
				if(item.Index == 0)
				{
					// Grayed
					displaybrush = new SolidBrush(SystemColors.GrayText);
					backbrush = new SolidBrush(SystemColors.Window);
				}
				else
				{
					// Normal color
					displaybrush = new SolidBrush(list.ForeColor);
					backbrush = new SolidBrush(SystemColors.Window);
				}
			}
			else
			{
				// Use number and description
				item = (INumberedTitle)list.Items[e.Index];
				displayname = item.Index + NUMBER_SEPERATOR + item.Title;

				// Determine colors to use
				if((e.State & DrawItemState.Focus) != 0)
				{
					displaybrush = new SolidBrush(SystemColors.HighlightText);
					backbrush = new SolidBrush(SystemColors.Highlight);
				}
				else
				{
					displaybrush = new SolidBrush(list.ForeColor);
					backbrush = new SolidBrush(SystemColors.Window);
				}
			}

			// Draw item
			e.Graphics.FillRectangle(backbrush, e.Bounds);
			e.Graphics.DrawString(displayname, list.Font, displaybrush, e.Bounds.X, e.Bounds.Y);
		}

		// List closed
		private void list_DropDownClosed(object sender, EventArgs e)
		{
			// Focus to number box
			number.SelectAll();
			number.Focus();
		}

		// Number changes
		private void number_TextChanged(object sender, EventArgs e)
		{
			int itemindex = -1;
			INumberedTitle item;
			
			// Not nothing?
			if(number.Text.Length > 0)
			{
				// Find the index in the list
				for(int i = 0; i < list.Items.Count; i++)
				{
					// This is the item we're looking for?
					item = (INumberedTitle)list.Items[i];
					if(item.Index.ToString() == number.Text)
					{
						// Found it
						itemindex = i;
						break;
					}
				}
			}

			// Select item
			if(list.SelectedIndex != itemindex) list.SelectedIndex = itemindex;
			list.Refresh();
		}

		// Keys pressed in number box
		private void number_KeyDown(object sender, KeyEventArgs e)
		{
			// Not numeric or control key?
			if(((e.KeyValue < 48) || (e.KeyValue > 57)) &&
			   (e.KeyCode != Keys.Back) && (e.KeyCode != Keys.Left) &&
			   (e.KeyCode != Keys.Right) && (e.KeyCode != Keys.Delete))
			{
				// Cancel this
				e.Handled = true;
			}
		}

		// Selection made
		private void list_SelectionChangeCommitted(object sender, EventArgs e)
		{
			INumberedTitle item = (INumberedTitle)list.SelectedItem;
			number.Text = item.Index.ToString();
		}
	}
}
