
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
using System.Windows.Forms;
using CodeImp.DoomBuilder.Data;
using CodeImp.DoomBuilder.Windows;

#endregion

namespace CodeImp.DoomBuilder.Controls
{
	internal partial class ImageBrowserControl : UserControl
	{
		#region ================== Constants
		
		#endregion
		
		#region ================== Delegates / Events

		public delegate void SelectedItemChangedDelegate();
		public delegate void SelectedItemDoubleClickDelegate();

		public event SelectedItemChangedDelegate SelectedItemChanged;
		public event SelectedItemDoubleClickDelegate SelectedItemDoubleClicked;
		
		#endregion

		#region ================== Variables
		
		// Properties
		private bool preventselection;
		
		// States
		private bool updating;
		private int keepselected;
		private bool browseFlats; //mxd
		private static bool uselongtexturenames; //mxd
		private static bool showtexturesizes; //mxd
		
		// All items
		private readonly List<ImageBrowserItem> items;

		// Items visible in the list
		private List<ImageBrowserItem> visibleitems;

		//mxd
		private static int mixMode;
		
		#endregion

		#region ================== Properties

		public bool PreventSelection { get { return preventselection; } set { preventselection = value; } }
		public bool HideInputBox { get { return splitter.Panel2Collapsed; } set { splitter.Panel2Collapsed = value; } }
		public bool BrowseFlats { get { return browseFlats; } set { browseFlats = value; } } //mxd
		public static bool ShowTextureSizes { get { return showtexturesizes; } internal set { showtexturesizes = value; } } //mxd
		public static bool UseLongTextureNames { get { return uselongtexturenames; } internal set { uselongtexturenames = value; } } //mxd
		public ListViewItem SelectedItem { get { if(list.SelectedItems.Count > 0) return list.SelectedItems[0]; else return null; } }
		
		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		public ImageBrowserControl()
		{
			// Initialize
			InitializeComponent();
			items = new List<ImageBrowserItem>();

			//mxd
			StepsList sizes = new StepsList { 4, 8, 16, 32, 48, 64, 96, 128, 196, 256, 512, 1024 };
			filterWidth.StepValues = sizes;
			filterHeight.StepValues = sizes;

			//mxd. Looks like SplitterDistance is unaffected by DPI scaling. Let's fix that...
			if(MainForm.DPIScaler.Height != 1.0f)
			{
				splitter.SplitterDistance = splitter.Height - splitter.Panel2.Height - (int)Math.Round(splitter.SplitterWidth * MainForm.DPIScaler.Height);
			}
		}
		
		// This applies the application settings
		public void ApplySettings()
		{
			// Force black background?
			if(General.Settings.BlackBrowsers)
			{
				list.BackColor = Color.Black;
				list.ForeColor = Color.White;
			}

			// Set the size of preview images
			if(General.Map != null)
			{
				int itemwidth = General.Map.Data.Previews.MaxImageWidth + 26;
				int itemheight = General.Map.Data.Previews.MaxImageHeight + 26;
				list.TileSize = new Size(itemwidth, itemheight);

				//mxd
				if(General.Map.Config.MixTexturesFlats) 
				{
					cbMixMode.SelectedIndex = mixMode;
				} 
				else 
				{
					labelMixMode.Visible = false;
					cbMixMode.Visible = false;
					
					int offset = label.Left - labelMixMode.Left;
					label.Left -= offset;
					objectname.Left -= offset;
					filterWidth.Left -= offset;
					filterwidthlabel.Left -= offset;
					filterHeight.Left -= offset;
					filterheightlabel.Left -= offset;
					showtexturesize.Left -= offset;
					longtexturenames.Left -= offset;
					
					mixMode = 0;
				}

				//mxd. Use long texture names?
				longtexturenames.Checked = (uselongtexturenames && General.Map.Config.UseLongTextureNames);
				longtexturenames.Visible = General.Map.Config.UseLongTextureNames;
			}
			else
			{
				longtexturenames.Visible = false; //mxd
				uselongtexturenames = false; //mxd
			}

			//mxd
			if(!General.Settings.CapitalizeTextureNames)
				objectname.CharacterCasing = CharacterCasing.Normal;

			//mxd. Show texture sizes?
			showtexturesize.Checked = showtexturesizes;
		}

		// This cleans everything up
		public virtual void CleanUp()
		{
			// Stop refresh timer
			refreshtimer.Enabled = false;
		}

		#endregion

		#region ================== Rendering

		// Draw item
		private void list_DrawItem(object sender, DrawListViewItemEventArgs e)
		{
			if(!updating) (e.Item as ImageBrowserItem).Draw(e.Graphics, e.Bounds);
		}

		// Refresher
		private void refreshtimer_Tick(object sender, EventArgs e)
		{
			bool allpreviewsloaded = true;
			
			// Go for all items
			foreach(ImageBrowserItem i in list.Items)
			{
				// Check if there are still previews that are not loaded
				allpreviewsloaded &= i.IsPreviewLoaded;
				
				// Items needs to be redrawn?
				if(i.CheckRedrawNeeded())
				{
					// Refresh item in list
					//list.RedrawItems(i.Index, i.Index, false);
					list.Invalidate();
				}
			}

			// If all previews were loaded, stop this timer
			if(allpreviewsloaded) refreshtimer.Stop();
		}

		#endregion

		#region ================== Events

		// Name typed
		private void objectname_TextChanged(object sender, EventArgs e)
		{
			// Update list
			RefillList(false);

			// No item selected?
			if(list.SelectedItems.Count == 0)
			{
				// Select first
				SelectFirstItem();
			}
		}

		// Key pressed in textbox
		private void objectname_KeyDown(object sender, KeyEventArgs e)
		{
			// Check what key is pressed
			switch(e.KeyData)
			{
				// Cursor keys
				case Keys.Left: SelectNextItem(SearchDirectionHint.Left); e.SuppressKeyPress = true; break;
				case Keys.Right: SelectNextItem(SearchDirectionHint.Right); e.SuppressKeyPress = true; break;
				case Keys.Up: SelectNextItem(SearchDirectionHint.Up); e.SuppressKeyPress = true;  break;
				case Keys.Down: SelectNextItem(SearchDirectionHint.Down); e.SuppressKeyPress = true; break;

				// Tab
				case Keys.Tab: GoToNextSameTexture(); e.SuppressKeyPress = true; break;
			}
		}

		//mxd
		private void filterSize_WhenTextChanged(object sender, EventArgs e) 
		{
			objectname_TextChanged(sender, e);
		}

		//mxd
		protected override bool ProcessTabKey(bool forward)
		{
			GoToNextSameTexture();
			return false;
		}
		
		// Selection changed
		private void list_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
		{
			if(!e.IsSelected) return; //mxd. Don't want to trigger this twice
			
			// Prevent selecting?
			if(preventselection)
			{
				foreach(ListViewItem i in list.SelectedItems) i.Selected = false;
			}
			else
			{
				// Raise event
				if(SelectedItemChanged != null) SelectedItemChanged();
			}
		}
		
		// Doublelicking an item
		private void list_DoubleClick(object sender, EventArgs e)
		{
			if(!preventselection && (list.SelectedItems.Count > 0))
				if(SelectedItemDoubleClicked != null) SelectedItemDoubleClicked();
		}

		//mxd
		private void cbMixMode_SelectedIndexChanged(object sender, EventArgs e) 
		{
			mixMode = cbMixMode.SelectedIndex;
			RefillList(false);
		}

		//mxd
		private void longtexturenames_CheckedChanged(object sender, EventArgs e)
		{
			uselongtexturenames = longtexturenames.Checked;
			RefillList(false);
		}

		//mxd
		private void showtexturesize_CheckedChanged(object sender, EventArgs e)
		{
			showtexturesizes = showtexturesize.Checked;
			list.Invalidate();
		}
		
		#endregion

		#region ================== Methods

		// This selects the next texture with the same name as the selected texture
		public void GoToNextSameTexture()
		{
			if(list.SelectedItems.Count > 0)
			{
				ListViewItem selected = list.SelectedItems[0];

				//mxd
				foreach(ImageBrowserItem n in visibleitems) 
				{
					if(n == selected) continue;
					if(n.Text == selected.Text) 
					{
						if(list.IsGroupCollapsed(n.Group)) list.SetGroupCollapsed(n.Group, false);
						n.Selected = true;
						n.Focused = true;
						n.EnsureVisible();
						return;
					}
				}
			}
		}

		// This selects an item by name
		public void SelectItem(string name, ListViewGroup preferredgroup)
		{
			ImageBrowserItem lvi = null; //mxd

			// Not when selecting is prevented
			if(preventselection) return;

			// Search in preferred group first
			if(preferredgroup != null)
			{
				foreach(ListViewItem item in list.Items)
				{
					ImageBrowserItem curitem = item as ImageBrowserItem;
					if(curitem != null && string.Compare(curitem.Icon.Name, name, true) == 0)
					{
						lvi = curitem;
						if(item.Group == preferredgroup) break;
					}
				}
			}
			
			// Select the item
			if(lvi != null)
			{
				// Select this item
				list.SelectedItems.Clear();
				lvi.Selected = true;
				lvi.EnsureVisible();
			}
		}
		
		// This performs item sleection by keys
		private void SelectNextItem(SearchDirectionHint dir)
		{
			// Not when selecting is prevented
			if(preventselection) return;
			
			// Nothing selected?
			if(list.SelectedItems.Count == 0)
			{
				// Select first
				SelectFirstItem();
			}
			else
			{
				// Get selected item
				ListViewItem lvi = list.SelectedItems[0];
				Rectangle lvirect = list.GetItemRect(lvi.Index, ItemBoundsPortion.Entire);
				Point spos = new Point(lvirect.Location.X + lvirect.Width / 2, lvirect.Y + lvirect.Height / 2);
				
				// Try finding 5 times in the given direction
				for(int i = 0; i < 5; i++)
				{
					// Move point in given direction
					switch(dir)
					{
						case SearchDirectionHint.Left: spos.X -= list.TileSize.Width / 2; break;
						case SearchDirectionHint.Right: spos.X += list.TileSize.Width / 2; break;
						case SearchDirectionHint.Up: spos.Y -= list.TileSize.Height / 2; break;
						case SearchDirectionHint.Down: spos.Y += list.TileSize.Height / 2; break;
					}
					
					// Test position
					lvi = list.GetItemAt(spos.X, spos.Y);
					if(lvi != null)
					{
						// Select item
						list.SelectedItems.Clear();
						lvi.Selected = true;
						break;
					}
				}
				
				// Make selection visible
				if(list.SelectedItems.Count > 0) list.SelectedItems[0].EnsureVisible();
			}
		}
		
		// This selectes the first item
		private void SelectFirstItem()
		{
			// Not when selecting is prevented
			if(preventselection) return;
			
			// Select first
			if(list.Items.Count > 0)
			{
				list.SelectedItems.Clear();
				ListViewItem lvi = list.GetItemAt(list.TileSize.Width / 2, list.TileSize.Height / 2);
				if(lvi != null)
				{
					lvi.Selected = true;
					lvi.EnsureVisible();
				}
			}
		}
		
		// This adds a group
		public ListViewGroup AddGroup(string name)
		{
			ListViewGroup grp = new ListViewGroup(name);
			list.Groups.Add(grp);
			return grp;
		}

		//mxd
		public bool IsGroupCollapsed(ListViewGroup group)
		{
			if(!list.Groups.Contains(group)) return false;
			return list.IsGroupCollapsed(group);
		}

		//mxd. This enables group collapsability and optionally collapses it
		public void SetGroupCollapsed(ListViewGroup group, bool collapse)
		{
			if(!list.Groups.Contains(group)) return;
			list.SetGroupCollapsed(group, collapse);
		}
		
		// This begins adding items
		public void BeginAdding(bool keepselectedindex)
		{
			if(keepselectedindex && (list.SelectedItems.Count > 0))
				keepselected = list.SelectedIndices[0];
			else
				keepselected = -1;
			
			// Clean list
			items.Clear();
			
			// Stop updating
			refreshtimer.Enabled = false;
		}

		// This ends adding items
		public void EndAdding()
		{
			// Fill list with items
			RefillList(true);

			// Start updating
			refreshtimer.Enabled = true;
		}
		
		// This adds an item
		public void Add(ImageData image, object tag, ListViewGroup group)
		{
			ImageBrowserItem i = new ImageBrowserItem(image, tag, uselongtexturenames); //mxd
			i.ListGroup = group;
			i.Group = group;
			i.ToolTipText = image.Name; //mxd
			items.Add(i);
		}
		
		// This adds an item
		public void Add(ImageData image, object tag, ListViewGroup group, string tooltiptext)
		{
			ImageBrowserItem i = new ImageBrowserItem(image, tag, uselongtexturenames); //mxd
			i.ListGroup = group;
			i.Group = group;
			i.ToolTipText = tooltiptext;
			items.Add(i);
		}

		// This fills the list based on the objectname filter
		private void RefillList(bool selectfirst)
		{
			visibleitems = new List<ImageBrowserItem>();
			
			// Begin updating list
			updating = true;
			//list.SuspendLayout();
			list.BeginUpdate();
			
			// Clear list first
			// Group property of items will be set to null, we will restore it later
			list.Items.Clear();

			//mxd. Filtering by texture size?
			int w = filterWidth.GetResult(-1);
			int h = filterHeight.GetResult(-1);
			
			// Go for all items
			ImageBrowserItem previtem = null; //mxd
			for(int i = items.Count - 1; i > -1; i--)
			{
				// Add item if valid
				items[i].ShowFullName = uselongtexturenames; //mxd
				if(ValidateItem(items[i], previtem) && ValidateItemSize(items[i], w, h)) 
				{
					items[i].Group = items[i].ListGroup;
					items[i].Selected = false;
					visibleitems.Add(items[i]);
					previtem = items[i];
				}
			}
			
			// Fill list
			visibleitems.Sort();
			ListViewItem[] array = new ListViewItem[visibleitems.Count];
			for(int i = 0; i < visibleitems.Count; i++) array[i] = visibleitems[i];
			list.Items.AddRange(array);
			
			// Done updating list
			updating = false;
			list.EndUpdate();
			list.Invalidate();
			//list.ResumeLayout();
			
			// Make selection?
			if(!preventselection && (list.Items.Count > 0))
			{
				// Select specific item?
				if(keepselected > -1)
				{
					list.Items[keepselected].Selected = true;
					list.Items[keepselected].EnsureVisible();
				}
				// Select first item?
				else if(selectfirst)
				{
					SelectFirstItem();
				}
			}
			
			// Raise event
			if((SelectedItemChanged != null) && !preventselection) SelectedItemChanged();
		}

		// This validates an item
		private bool ValidateItem(ImageBrowserItem item, ImageBrowserItem previtem)
		{
			//mxd. Don't show duplicate items
			if(previtem != null && item.TextureName == previtem.TextureName && item.Group == previtem.Group) return false; //mxd
			
			//mxd. mixMode: 0 = All, 1 = Textures, 2 = Flats, 3 = Based on BrowseFlats
			if(!splitter.Panel2Collapsed) 
			{
				if(mixMode == 1 && item.Icon.IsFlat) return false;
				if(mixMode == 2 && !item.Icon.IsFlat) return false;
				if(mixMode == 3 && (browseFlats != item.Icon.IsFlat)) return false;
			}

			return item.Text.ToUpperInvariant().Contains(objectname.Text.ToUpperInvariant());
		}

		//mxd. This validates an item's texture size
		private static bool ValidateItemSize(ImageBrowserItem i, int w, int h) 
		{
			if (!i.Icon.IsPreviewLoaded) return true;
			if (w > 0 && i.Icon.Width != w) return false;
			if (h > 0 && i.Icon.Height != h) return false;
			return true;
		}
		
		// This sends the focus to the textbox
		public void FocusTextbox()
		{
			objectname.Focus();
		}
		
		#endregion
	}
}
