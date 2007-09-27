using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using CodeImp.DoomBuilder.Images;

namespace CodeImp.DoomBuilder.Interface
{
	internal partial class ResourceListEditor : UserControl
	{
		#region ================== Variables

		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		public ResourceListEditor()
		{
			// Initialize
			InitializeComponent();
			ResizeColumnHeader();

			// Start with a clear list
			resourceitems.Items.Clear();
		}

		#endregion

		#region ================== Methods

		// This will show a fixed list
		public void FixedResourceLocationList(ResourceLocationList list)
		{
			// Start editing list
			resourceitems.BeginUpdate();
			
			// Go for all items
			for(int i = resourceitems.Items.Count; i >= 0; i--)
			{
				// Remove item if fixed
				if(resourceitems.Items[i].ForeColor != SystemColors.WindowText)
					resourceitems.Items.RemoveAt(i);
			}
			
			// Go for all items
			for(int i = list.Count; i >= 0; i--)
			{
				// Add item as fixed
				resourceitems.Items.Insert(0, new ListViewItem(list[i].location));
				resourceitems.Items[0].Tag = list[i];

				// Set icon
				if(list[i].type == ResourceLocation.RESOURCE_DIRECTORY)
					resourceitems.Items[0].ImageIndex = 2;
				else if(list[i].type == ResourceLocation.RESOURCE_WAD)
					resourceitems.Items[0].ImageIndex = 3;

				// Set disabled
				resourceitems.Items[0].ForeColor = SystemColors.GrayText;
			}

			// Done
			resourceitems.EndUpdate();
		}

		// This will edit the given list
		public void EditResourceLocationList(ResourceLocationList list)
		{
			// Start editing list
			resourceitems.BeginUpdate();

			// Go for all items
			for(int i = resourceitems.Items.Count; i >= 0; i--)
			{
				// Remove item unless fixed
				if(resourceitems.Items[i].ForeColor == SystemColors.WindowText)
					resourceitems.Items.RemoveAt(i);
			}

			// Go for all items
			for(int i = 0; i < list.Count; i++)
			{
				// Add item
				AddItem(list[i]);
			}

			// Done
			resourceitems.EndUpdate();
		}

		// This adds a normal item
		public void AddItem(ResourceLocation rl)
		{
			int index;

			// Start editing list
			resourceitems.BeginUpdate();

			// Add item
			index = resourceitems.Items.Count;
			resourceitems.Items.Add(new ListViewItem(rl.location));
			resourceitems.Items[index].Tag = rl;

			// Set icon
			if(rl.type == ResourceLocation.RESOURCE_DIRECTORY)
				resourceitems.Items[index].ImageIndex = 0;
			else if(rl.type == ResourceLocation.RESOURCE_WAD)
				resourceitems.Items[index].ImageIndex = 1;

			// Set normal color
			resourceitems.Items[index].ForeColor = SystemColors.WindowText;

			// Done
			resourceitems.EndUpdate();
		}
		
		// This fixes the column header in the list
		private void ResizeColumnHeader()
		{
			// Resize column header to full extend
			column.Width = resourceitems.ClientSize.Width - 2;
		}

		// When the resources list resizes
		private void resources_SizeChanged(object sender, EventArgs e)
		{
			// Resize column header also
			ResizeColumnHeader();
		}

		// Add a resource
		private void addresource_Click(object sender, EventArgs e)
		{
			ResourceOptionsForm resoptions;
			Rectangle startposition;
			
			// Open resource options dialog
			resoptions = new ResourceOptionsForm(new ResourceLocation(), "Add Resource");
			resoptions.StartPosition = FormStartPosition.Manual;
			startposition = new Rectangle(40, 20, 1, 1);
			startposition = this.RectangleToScreen(startposition);
			resoptions.Location = startposition.Location;
			if(resoptions.ShowDialog(this) == DialogResult.OK)
			{
				// Add resource
				AddItem(resoptions.ResourceLocation);
			}
		}

		// Edit resource
		private void editresource_Click(object sender, EventArgs e)
		{
			ResourceOptionsForm resoptions;
			Rectangle startposition;
			ListViewItem selecteditem;
			ResourceLocation rl;

			// Anything selected?
			if(resourceitems.SelectedItems.Count > 0)
			{
				// Get selected item
				selecteditem = resourceitems.SelectedItems[0];

				// Open resource options dialog
				resoptions = new ResourceOptionsForm((ResourceLocation)selecteditem.Tag, "Resource Options");
				resoptions.StartPosition = FormStartPosition.Manual;
				startposition = new Rectangle(40, 20, 1, 1);
				startposition = this.RectangleToScreen(startposition);
				resoptions.Location = startposition.Location;
				if(resoptions.ShowDialog(this) == DialogResult.OK)
				{
					// Start editing list
					resourceitems.BeginUpdate();

					// Update item
					rl = resoptions.ResourceLocation;
					selecteditem.Text = rl.location;
					selecteditem.Tag = rl;

					// Set icon
					if(rl.type == ResourceLocation.RESOURCE_DIRECTORY)
						selecteditem.ImageIndex = 0;
					else if(rl.type == ResourceLocation.RESOURCE_WAD)
						selecteditem.ImageIndex = 1;

					// Done
					resourceitems.EndUpdate();
				}
			}
		}

		// Remove resource
		private void deleteresource_Click(object sender, EventArgs e)
		{
			// Anything selected?
			if(resourceitems.SelectedItems.Count > 0)
			{
				// Remove it
				resourceitems.Items.Remove(resourceitems.SelectedItems[0]);
			}
		}
		
		// Item selected
		private void resourceitems_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
		{
			// Anything selected
			if(resourceitems.SelectedItems.Count > 0)
			{
				// Go for all selected items
				for(int i = resourceitems.SelectedItems.Count - 1; i >= 0; i--)
				{
					// Item grayed? Then deselect.
					if(resourceitems.SelectedItems[i].ForeColor != SystemColors.WindowText)
						resourceitems.SelectedItems[i].Selected = false;
				}
			}

			// Anything selected
			if(resourceitems.SelectedItems.Count > 0)
			{
				// Enable buttons
				editresource.Enabled = true;
				deleteresource.Enabled = true;
			}
			else
			{
				// Disable buttons
				editresource.Enabled = false;
				deleteresource.Enabled = false;
			}
		}

		// When an item is double clicked
		private void resourceitems_DoubleClick(object sender, EventArgs e)
		{
			// Click the edit resource button
			if(editresource.Enabled) editresource_Click(sender, e);
		}

		// Returns a list of the resources
		public ResourceLocationList GetResources()
		{
			ResourceLocationList list = new ResourceLocationList();

			// Go for all items
			for(int i = 0; i < resourceitems.Items.Count; i++)
			{
				// Item not grayed?
				if(resourceitems.Items[i].ForeColor == SystemColors.WindowText)
				{
					// Add item to list
					list.Add((ResourceLocation)resourceitems.Items[i].Tag);
				}
			}

			// Return result
			return list;
		}

		#endregion
	}
}
