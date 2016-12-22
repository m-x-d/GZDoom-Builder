
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
		
		private static readonly HashSet<char> AllowedSpecialChars = new HashSet<char>("!@#$%^&*()-_=+<>,.?/'\"\\;:[]{}`~".ToCharArray()); //mxd

		#endregion
		
		#region ================== Delegates / Events

		public delegate void SelectedItemChangedDelegate(ImageBrowserItem item);
		public delegate void SelectedItemDoubleClickDelegate(ImageBrowserItem item);

		public event SelectedItemChangedDelegate SelectedItemChanged;
		public event SelectedItemDoubleClickDelegate SelectedItemDoubleClicked;
		
		#endregion

		#region ================== Variables
		
		// Properties
		private bool preventselection;
		
		// States
		private int keepselected;
		private bool browseflats; //mxd
		private bool uselongtexturenames; //mxd
		private bool blockupdate; //mxd
		
		//mxd. All items
		private Dictionary<string, List<ImageBrowserItem>> items; // <group, <items>>
		private List<string> groups;
		private string selectedgroup;

		// Filtered items
		private List<ImageBrowserItem> visibleitems;

		//mxd
		private int texturetype;
		
		#endregion

		#region ================== Properties

		public bool PreventSelection { get { return preventselection; } set { preventselection = value; } }
		public bool HideInputBox { get { return splitter.Panel2Collapsed; } set { splitter.Panel2Collapsed = value; } }
		public List<ImageBrowserItem> SelectedItems { get { return list.SelectedItems; } } //mxd
		public ImageBrowserItem SelectedItem { get { return (list.SelectedItems.Count > 0 ? list.SelectedItems[0] : null); } }
		public string SelectedGroup { get { return selectedgroup; } } //mxd

		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		public ImageBrowserControl()
		{
			// Initialize
			InitializeComponent();
			items = new Dictionary<string, List<ImageBrowserItem>>();
			groups = new List<string>();

			//mxd
			StepsList sizes = new StepsList { 4, 8, 16, 32, 48, 64, 96, 128, 196, 256, 512, 1024 };
			filterWidth.StepValues = sizes;
			filterHeight.StepValues = sizes;

			//mxd. Looks like SplitterDistance is unaffected by DPI scaling. Let's fix that...
			if(MainForm.DPIScaler.Height != 1.0f)
			{
				splitter.SplitterDistance = splitter.Height - splitter.Panel2.Height - (int)Math.Round(splitter.SplitterWidth * MainForm.DPIScaler.Height);
			}

			//mxd
			list.SelectionChanged += list_SelectionChanged;
		}

		// This applies the application settings
		public void ApplySettings(string settingpath, bool browseflats)
		{
			blockupdate = true;

			//TODO: group handling doesn't couple well with usedtexturesonly checkbox...
			this.browseflats = browseflats;
			uselongtexturenames = General.Map.Options.UseLongTextureNames;
			selectedgroup = General.Settings.ReadSetting(settingpath + ".selectedgroup", string.Empty);
			
			if(string.IsNullOrEmpty(selectedgroup) || (groups.Count > 0 && !groups.Contains(selectedgroup)))
			{
				selectedgroup = groups[0];
			}

			texturetype = General.Settings.ReadSetting(settingpath + ".texturetype", 0);
			usedtexturesonly.Checked = (groups.IndexOf(selectedgroup) == 1);
			
			int imagesize = General.Settings.ReadSetting(settingpath + ".imagesize", 128);
			sizecombo.Text = (imagesize == 0 ? sizecombo.Items[0].ToString() : imagesize.ToString());
			list.ImageSize = imagesize;

			ApplySettings();

			blockupdate = false;
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
				//mxd
				if(General.Map.Config.MixTexturesFlats) 
				{
					texturetypecombo.SelectedIndex = texturetype;
				} 
				else 
				{
					labelMixMode.Enabled = false;
					texturetypecombo.Enabled = false;
					texturetype = 0;
				}

				//mxd. Use long texture names?
				longtexturenames.Checked = (uselongtexturenames && General.Map.Config.UseLongTextureNames);
				longtexturenames.Enabled = General.Map.Config.UseLongTextureNames;
			}
			else
			{
				longtexturenames.Enabled = false; //mxd
				uselongtexturenames = false; //mxd
			}

			//mxd
			objectname.CharacterCasing = (longtexturenames.Checked ? CharacterCasing.Normal : CharacterCasing.Upper);
		}

		//mxd. Save settings
		public virtual void OnClose(string settingpath)
		{
			General.Settings.WriteSetting(settingpath + ".selectedgroup", selectedgroup);
			General.Settings.WriteSetting(settingpath + ".imagesize", list.ImageSize);
			if(General.Map.Config.UseLongTextureNames) General.Map.Options.UseLongTextureNames = uselongtexturenames;

			CleanUp();
		}

		// This cleans everything up
		public virtual void CleanUp()
		{
			// Stop refresh timer
			refreshtimer.Enabled = false;
		}

		#endregion

		#region ================== Rendering

		// Refresher
		private void refreshtimer_Tick(object sender, EventArgs e)
		{
			bool allpreviewsloaded = true;
			bool redrawneeded = false; //mxd
			
			// Go for all items
			foreach(ImageBrowserItem i in list.Items)
			{
				// Check if there are still previews that are not loaded
				allpreviewsloaded &= i.IsPreviewLoaded;

				//mxd. Item needs to be redrawn?
				redrawneeded |= i.CheckRedrawNeeded();
			}

			// If all previews were loaded, stop this timer
			if(allpreviewsloaded) refreshtimer.Stop();

			// Redraw the list if needed
			if(redrawneeded) list.Invalidate();
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
			// Let the list handle arrow keys and such
			if(list.ProcessKeyDown(e))
			{
				e.SuppressKeyPress = true;
			}
			// Toggle groups
			else if(e.KeyData == Keys.Tab)
			{
				ShowNextGroup();
				e.SuppressKeyPress = true;
			}
		}

		//mxd
		private void objectclear_Click(object sender, EventArgs e)
		{
			objectname.Clear();
			list.Focus();
		}

		//mxd
		private void filterSize_WhenTextChanged(object sender, EventArgs e) 
		{
			objectname_TextChanged(sender, e);
		}

		//mxd
		protected override bool ProcessTabKey(bool forward)
		{
			usedtexturesonly.Checked = !usedtexturesonly.Checked;
			return false;
		}
		
		// Selection changed
		private void list_SelectionChanged(object sender, List<ImageBrowserItem> selection)
		{
			// Prevent selecting?
			if(preventselection)
			{
				if(selection.Count > 0) list.ClearSelection(); //mxd
			}
			else
			{
				// Raise event
				if(SelectedItemChanged != null)
					SelectedItemChanged(list.SelectedItems.Count > 0 ? list.SelectedItems[0] : null);
			}
		}
		
		// Doublelicking an item
		private void list_ItemDoubleClicked(object sender, ImageBrowserItem item)
		{
			if(!preventselection && (list.SelectedItems.Count > 0))
				if(SelectedItemDoubleClicked != null) SelectedItemDoubleClicked(item);
		}

		//mxd. Transfer input to Filter textbox
		private void list_KeyPress(object sender, KeyPressEventArgs e)
		{
			if(e.KeyChar == '\b') // Any better way to check for Backspace?..
			{
				if(!string.IsNullOrEmpty(objectname.Text) && objectname.SelectionStart > 0 && objectname.SelectionLength == 0)
				{
					int s = objectname.SelectionStart - 1;
					objectname.Text = objectname.Text.Remove(s, 1);
					objectname.SelectionStart = s;
				}
			}
			if(e.KeyChar == 8 && objectname.Text.Length > 0)
			{
				if(objectname.SelectionLength > 0)
				{
					objectname.Text = objectname.Text.Substring(0, objectname.SelectionStart) +
										 objectname.Text.Substring(objectname.SelectionStart + objectname.SelectionLength);
				}
				else
				{
					objectname.Text = objectname.Text.Substring(0, objectname.Text.Length - 1);
				}
			}
			else if((e.KeyChar >= 'a' && e.KeyChar <= 'z') || (e.KeyChar >= '0' && e.KeyChar <= '9') || AllowedSpecialChars.Contains(e.KeyChar))
			{
				if(objectname.SelectionLength > 0)
				{
					objectname.Text = objectname.Text.Substring(0, objectname.SelectionStart) +
										 e.KeyChar +
										 objectname.Text.Substring(objectname.SelectionStart + objectname.SelectionLength);
				}
				else
				{
					objectname.Text += e.KeyChar;
				}
			}
		}

		//mxd
		private void texturetypecombo_SelectedIndexChanged(object sender, EventArgs e) 
		{
			texturetype = texturetypecombo.SelectedIndex;
			RefillList(false);
		}

		//mxd
		private void sizecombo_SelectedIndexChanged(object sender, EventArgs e)
		{
			if(blockupdate) return;
			list.ImageSize = (sizecombo.SelectedIndex == 0 ? 0 : Convert.ToInt32(sizecombo.SelectedItem));
			list.Focus();
		}

		//mxd
		private void longtexturenames_CheckedChanged(object sender, EventArgs e)
		{
			if(blockupdate) return;
			uselongtexturenames = longtexturenames.Checked;
			objectname.CharacterCasing = (uselongtexturenames ? CharacterCasing.Normal : CharacterCasing.Upper);
			RefillList(false);
		}

		//mxd
		private void usedtexturesonly_CheckedChanged(object sender, EventArgs e)
		{
			if(blockupdate) return;
			ShowNextGroup();
		}
		
		#endregion

		#region ================== Methods

		// This selects an item by longname (mxd - changed from name to longname)
		public void SelectItem(long longname, string preferredgroup)
		{
			// Not when selecting is prevented
			if(preventselection) return;

			// Assemble group order
			List<string> searchorder;
			if(string.IsNullOrEmpty(preferredgroup) || !groups.Contains(preferredgroup))
			{
				searchorder = new List<string>(groups);
			}
			else
			{
				searchorder = new List<string> { preferredgroup };
				List<string> othergroups = new List<string>(groups);
				othergroups.Remove(preferredgroup);
				searchorder.AddRange(othergroups);
			}

			// Search for item
			ImageBrowserItem target = null; //mxd
			string targetgroup = string.Empty;
			foreach(string group in searchorder)
			{
				foreach(ImageBrowserItem item in items[group])
				{
					if(item.Icon.LongName == longname) //mxd
					{
						target = item;
						targetgroup = group;
						break;
					}
				}
			}
			
			if(target != null)
			{
				// Group switching required?
				SelectGroup(targetgroup);

				// Select the item
				list.SetSelectedItem(target);
			}
		}
		
		// This selectes the first item
		private void SelectFirstItem()
		{
			// Not when selecting is prevented
			if(preventselection) return;
			
			// Select first
			if(list.Items.Count > 0) list.SetSelectedItem(list.Items[0]);
		}
		
		// This adds a group
		public void AddGroup(string name)
		{
			if(groups.Contains(name)) return;
			groups.Add(name);
			items.Add(name, new List<ImageBrowserItem>());
		}

		// This selects a group
		public void SelectGroup(string groupname)
		{
			if(string.IsNullOrEmpty(groupname) || groupname == selectedgroup || !groups.Contains(groupname)) return;
			selectedgroup = groupname;
			list.SetItems(items[groupname]);

			blockupdate = true;
			usedtexturesonly.Checked = (groups.IndexOf(selectedgroup) == 1);
			blockupdate = false;

			RefillList(false);
		}

		// This toggles between groups
		private void ShowNextGroup()
		{
			if(groups.Count < 2) return;
			int nextgroupindex = groups.IndexOf(selectedgroup) + 1;
			if(nextgroupindex >= items.Count) nextgroupindex = 0;
			SelectGroup(groups[nextgroupindex]);
		}
		
		// This begins adding items
		public void BeginAdding(bool keepselectedindex)
		{
			if(keepselectedindex && (list.SelectedItems.Count > 0))
				keepselected = list.Items.IndexOf(list.SelectedItems[0]);
			else
				keepselected = -1;
			
			// Clean list
			items.Clear();

			// Re-add groups...
			foreach(string s in groups)
				items.Add(s, new List<ImageBrowserItem>());
			
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

		//mxd. This adds a category item
		public void AddFolder(ImageBrowserItemType itemtype, string group, string categoryname)
		{
			if(string.IsNullOrEmpty(group)) group = ImageSelectorPanel.DEFAULT_GROUP;
			if(!items.ContainsKey(group))
			{
				items.Add(group, new List<ImageBrowserItem>());
				groups.Add(group);
			}

			switch(itemtype)
			{
				case ImageBrowserItemType.FOLDER: case ImageBrowserItemType.FOLDER_UP:
					items[group].Add(new ImageBrowserCategoryItem(itemtype, categoryname));
					break;

				default: throw new Exception("Unsupported ImageBrowserItemType");
			}
		}
		
		// This adds an item
		public void AddItem(ImageData image, string group, string tooltip)
		{
			if(string.IsNullOrEmpty(group)) group = ImageSelectorPanel.DEFAULT_GROUP;
			if(!items.ContainsKey(group))
			{
				items.Add(group, new List<ImageBrowserItem>());
				groups.Add(group);
			}
			items[group].Add(new ImageBrowserItem(image, tooltip, uselongtexturenames));
		}

		public void AddItem(ImageData image, string group)
		{
			AddItem(image, group, string.Empty);
		}

		// This fills the list based on the objectname filter
		private void RefillList(bool selectfirst)
		{
			if(groups.Count == 0) return;
			visibleitems = new List<ImageBrowserItem>();

			//mxd. Check group name...
			if(string.IsNullOrEmpty(selectedgroup))
			{
				if(groups.Count == 0) throw new Exception("No groups defined...");
				selectedgroup = groups[0];
			}

			//mxd. Store info about currently selected item
			string selectedname = string.Empty;
			if(!selectfirst && keepselected == -1 && list.SelectedItems.Count > 0)
			{
				selectedname = list.SelectedItems[0].Icon.Name;
			}

			// Clear list first
			list.Clear();
			list.Title = selectedgroup;

			//mxd. Filtering by texture size?
			int w = filterWidth.GetResult(-1);
			int h = filterHeight.GetResult(-1);
			
			// Go for all items
			ImageBrowserItem previtem = null; //mxd
			for(int i = items[selectedgroup].Count - 1; i > -1; i--)
			{
				// Add item if valid
				items[selectedgroup][i].ShowFullName = uselongtexturenames; //mxd
				if(ValidateItem(items[selectedgroup][i], previtem) && ValidateItemSize(items[selectedgroup][i], w, h)) 
				{
					visibleitems.Add(items[selectedgroup][i]);
					previtem = items[selectedgroup][i];
				}
			}
			
			// Fill list
			visibleitems.Sort();
			list.SetItems(visibleitems);
			
			// Make selection?
			if(!preventselection && list.Items.Count > 0)
			{
				// Select specific item?
				if(keepselected > -1)
				{
					list.SetSelectedItem(list.Items[keepselected]);
				}
				// Select first item?
				else if(selectfirst)
				{
					SelectFirstItem();
				}
				//mxd. Try reselecting the same/next closest item
				else if(!string.IsNullOrEmpty(selectedname))
				{
					ImageBrowserItem bestmatch = null;
					int charsmatched = 1;
					foreach(ImageBrowserItem item in list.Items)
					{
						if(item.ItemType == ImageBrowserItemType.IMAGE && item.Icon.Name[0] == selectedname[0])
						{
							if(item.Icon.Name == selectedname)
							{
								bestmatch = item;
								break;
							}

							for(int i = 1; i < Math.Min(item.Icon.Name.Length, selectedname.Length); i++)
							{
								if(item.Icon.Name[i] != selectedname[i])
								{
									if(i > charsmatched)
									{
										bestmatch = item;
										charsmatched = i;
									}
									break;
								}
							}
						}
					}

					// Select found item
					if(bestmatch != null)
					{
						list.SetSelectedItem(bestmatch);
					}
					else
					{
						SelectFirstItem();
					}
				}
			}
			
			// Raise event
			if((SelectedItemChanged != null) && !preventselection)
				SelectedItemChanged(list.SelectedItems.Count > 0 ? list.SelectedItems[0] : null);
		}

		// This validates an item
		private bool ValidateItem(ImageBrowserItem item, ImageBrowserItem previtem)
		{
			//mxd. Don't show duplicate items
			if(previtem != null && item.TextureName == previtem.TextureName) return false; //mxd
			
			//mxd. mixMode: 0 = All, 1 = Textures, 2 = Flats, 3 = Based on BrowseFlats
			if(!splitter.Panel2Collapsed) 
			{
				if(texturetype == 1 && item.Icon.IsFlat) return false;
				if(texturetype == 2 && !item.Icon.IsFlat) return false;
				if(texturetype == 3 && (browseflats != item.Icon.IsFlat)) return false;
			}

			return item.TextureName.ToUpperInvariant().Contains(objectname.Text.ToUpperInvariant());
		}

		//mxd. This validates an item's texture size
		private static bool ValidateItemSize(ImageBrowserItem i, int w, int h) 
		{
			if(!i.Icon.IsPreviewLoaded) return true;
			if(w > 0 && i.Icon.Width != w) return false;
			if(h > 0 && i.Icon.Height != h) return false;
			return true;
		}
		
		//mxd. This sends the focus to the textures list
		public void FocusList()
		{
			list.Focus();
		}
		
		#endregion
	}
}
