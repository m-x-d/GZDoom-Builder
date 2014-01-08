
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
using System.Drawing;
using System.Windows.Forms;
using CodeImp.DoomBuilder.IO;
using CodeImp.DoomBuilder.Data;
using CodeImp.DoomBuilder.Config;
using System.IO;

#endregion

namespace CodeImp.DoomBuilder.Windows
{
	internal partial class TextureBrowserForm : Form
	{
		// Variables
		private string selectedname;
		private Point lastposition;
		private Size lastsize;
		private ListViewGroup usedgroup;
		private ListViewGroup availgroup;
		private TreeNode selectedset; //mxd
		private string selecttextureonfill;
		private bool browseFlats;
		
		// Properties
		public string SelectedName { get { return selectedname; } }
		
		// Constructor
		public TextureBrowserForm(string selecttexture, bool browseFlats)
		{
			Cursor.Current = Cursors.WaitCursor;
			TreeNode item;//mxd
			long longname = Lump.MakeLongName(selecttexture ?? "");
			selectedset = null;//mxd
			this.browseFlats = browseFlats;
			
			// Initialize
			InitializeComponent();

			//mxd. Set title
			string imgType = (browseFlats ? "flats" : "textures");
			this.Text = "Browse " + imgType;

			browser.ApplySettings();
			
			// Update the used textures
			General.Map.Data.UpdateUsedTextures();

			tvTextureSets.BeginUpdate();//mxd

			// Texture to select when list is filled
			selecttextureonfill = selecttexture;

			// Make groups
			usedgroup = browser.AddGroup("Used " + imgType + ":");
			availgroup = browser.AddGroup("Available " + imgType + ":");

			//mxd. Fill texture sets list with normal texture sets
			foreach(IFilledTextureSet ts in General.Map.Data.TextureSets)
			{
				item = tvTextureSets.Nodes.Add(ts.Name + " [" + ts.Textures.Count + "]");
				item.Name = ts.Name;
				item.Tag = ts;
				item.ImageIndex = 0;
			}

			//mxd. Add container-specific texture sets
			foreach(ResourceTextureSet ts in General.Map.Data.ResourceTextureSets)
			{
				item = tvTextureSets.Nodes.Add(ts.Name + " [" + ts.Textures.Count + "]");
				item.Name = ts.Name;
				item.Tag = ts;
				item.ImageIndex = 2 + ts.Location.type;
				item.SelectedImageIndex = item.ImageIndex;

				if (ts.Location.type != DataLocation.RESOURCE_WAD)
					createNodes(item);
			}

			//mxd. Add All textures set
			item = tvTextureSets.Nodes.Add(General.Map.Data.AllTextureSet.Name + " [" + General.Map.Data.AllTextureSet.Textures.Count + "]");
			item.Name = General.Map.Data.AllTextureSet.Name;
			item.Tag = General.Map.Data.AllTextureSet;
			item.ImageIndex = 1;
			item.SelectedImageIndex = item.ImageIndex;

			//mxd. Select the last one that was selected
			string selectname = General.Settings.ReadSetting("browserwindow.textureset", "");
			TreeNode match;
			if (string.IsNullOrEmpty(selectname)) {
				match = tvTextureSets.Nodes[tvTextureSets.Nodes.Count - 1];
			} else {
				match = findNodeByName(tvTextureSets.Nodes, selectname);
			}

			if (match != null) {
				IFilledTextureSet set = (match.Tag as IFilledTextureSet);
				
				foreach (ImageData img in set.Textures) {
					if (img.LongName == longname) {
						selectedset = match;
						break;
					}
				}
			}

			//mxd. If the selected texture was not found in the last-selected set, try finding it in the other sets
			if (selectedset == null) {
				foreach (TreeNode n in tvTextureSets.Nodes) {
					selectedset = findTextureByLongName(n, longname);
					if (selectedset != null) 
						break;
				}
			}

			//mxd. Texture still now found? Then just select the last used set
			if (selectedset == null && match != null)
				selectedset = match;

			if(tvTextureSets.Nodes.Count > 0)
				tvTextureSets.Nodes[0].Expand();//mxd
			tvTextureSets.EndUpdate();//mxd

			if (selectedset != null) {//mxd
				tvTextureSets.SelectedNode = selectedset;
			}

			// Keep last position and size
			lastposition = this.Location;
			lastsize = this.Size;

			this.SuspendLayout();

			// Position window from configuration settings
			this.Size = new Size(General.Settings.ReadSetting("browserwindow.sizewidth", this.Size.Width),
								 General.Settings.ReadSetting("browserwindow.sizeheight", this.Size.Height));
			this.WindowState = (FormWindowState)General.Settings.ReadSetting("browserwindow.windowstate", (int)FormWindowState.Normal);
			
			//mxd
			if (this.WindowState == FormWindowState.Normal) {
				Point location = new Point(General.Settings.ReadSetting("browserwindow.positionx", int.MaxValue), General.Settings.ReadSetting("browserwindow.positiony", int.MaxValue));

				if (location.X < int.MaxValue && location.Y < int.MaxValue) {
					this.Location = location;
				} else {
					this.StartPosition = FormStartPosition.CenterParent;
				}
			}

			this.ResumeLayout(true);
		}

		//mxd
		private int sortImageData(ImageData img1, ImageData img2) {
			return img1.FullName.CompareTo(img2.FullName);
		}

		//mxd
		private TreeNode findTextureByLongName(TreeNode node, long longname) {
			//first search in child nodes
			TreeNode match = null;

			foreach(TreeNode n in node.Nodes) {
				match = findTextureByLongName(n, longname);
				if(match != null) return match;
			}

			//then - in current node
			IFilledTextureSet set = (node.Tag as IFilledTextureSet);

			foreach (ImageData img in set.Textures)
				if (img.LongName == longname) return node;

			return null;
		}

		//mxd
		private TreeNode findNodeByName(TreeNodeCollection nodes, string selectname) {
			foreach (TreeNode n in nodes) {
				if (n.Name == selectname) return n;

				TreeNode match = findNodeByName(n.Nodes, selectname);
				if(match != null) return match;
			}
			return null;
		}

		//mxd
		private void createNodes(TreeNode root) {
			ResourceTextureSet set = root.Tag as ResourceTextureSet;
			if (set == null) {
				General.ErrorLogger.Add(ErrorType.Error, "Resource " + root.Name + " doesn't have TextureSet!");
				return;
			}

			int imageIndex = set.Location.type + 4;
			string[] separator = new[] { Path.DirectorySeparatorChar.ToString() };
			
			ImageData[] textures = new ImageData[set.Textures.Count];
			set.Textures.CopyTo(textures, 0);
			Array.Sort(textures, sortImageData);

			foreach (ImageData image in textures) {
				string localName = image.FullName.Replace(set.Location.location, "");
				string[] parts = localName.Split(separator, StringSplitOptions.RemoveEmptyEntries);
				TreeNode curNode = root;

				if (parts.Length == 1) continue;

				for (int i = 0; i < parts.Length - 1; i++) {
					string category = parts[i];
					
					//already got such category?
					if (curNode.Nodes.Count > 0 && curNode.Nodes.ContainsKey(category)) {
						curNode = curNode.Nodes[category];

					} else { //create a new one
						TreeNode n = new TreeNode(category) {Name = category, ImageIndex = imageIndex, SelectedImageIndex = imageIndex};

						curNode.Nodes.Add(n);
						curNode = n;

						ResourceTextureSet ts = new ResourceTextureSet(category, set.Location);
						curNode.Tag = ts;
					}

					//add to current and parent nodes
					if (i == parts.Length - 2) {
						TreeNode cn = curNode;
						while (cn != root) {
							ResourceTextureSet curTs = cn.Tag as ResourceTextureSet;
							curTs.AddTexture(image);
							cn = cn.Parent;
						}
					}
				}
			}

			if (root.Nodes.Count == 1 && root.Nodes[0].Nodes.Count > 0) {
				TreeNode[] children = new TreeNode[root.Nodes[0].Nodes.Count];
				root.Nodes[0].Nodes.CopyTo(children, 0);
				root.Nodes.Clear();
				root.Nodes.AddRange(children);
			}

			foreach (TreeNode n in root.Nodes) SetItemsCount(n);
		}

		//mxd
		private void SetItemsCount(TreeNode node) {
			ResourceTextureSet ts = node.Tag as ResourceTextureSet;
			if (ts == null) throw new Exception("Expected IFilledTextureSet, but got null...");
			node.Text += " [" + ts.Textures.Count + "]";

			if (General.Map.Config.MixTexturesFlats)
				ts.MixTexturesAndFlats();

			foreach (TreeNode child in node.Nodes) SetItemsCount(child);
		}

		// Selection changed
		private void browser_SelectedItemChanged()
		{
			apply.Enabled = (browser.SelectedItem != null);
		}

		// OK clicked
		private void apply_Click(object sender, EventArgs e)
		{
			// Set selected name and close
			if(browser.SelectedItem != null)
			{
				selectedname = browser.SelectedItem.Text;
				DialogResult = DialogResult.OK;
			}
			else
			{
				selectedname = "";
				DialogResult = DialogResult.Cancel;
			}
			this.Close();
		}

		// Cancel clicked
		private void cancel_Click(object sender, EventArgs e)
		{
			// No selection, close
			selectedname = "";
			DialogResult = DialogResult.Cancel;
			this.Close();
		}

		// Activated
		private void TextureBrowserForm_Activated(object sender, EventArgs e)
		{
			Cursor.Current = Cursors.Default;
		}

		// Loading
		private void TextureBrowserForm_Load(object sender, EventArgs e)
		{
			// Normal windowstate?
			if(this.WindowState == FormWindowState.Normal)
			{
				// Keep last position and size
				lastposition = this.Location;
				lastsize = this.Size;
			}
		}

		// Resized
		private void TextureBrowserForm_ResizeEnd(object sender, EventArgs e)
		{
			// Normal windowstate?
			if(this.WindowState == FormWindowState.Normal)
			{
				// Keep last position and size
				lastposition = this.Location;
				lastsize = this.Size;
			}
		}

		// Moved
		private void TextureBrowserForm_Move(object sender, EventArgs e)
		{
			// Normal windowstate?
			if(this.WindowState == FormWindowState.Normal)
			{
				// Keep last position and size
				lastposition = this.Location;
				lastsize = this.Size;
			}
		}

		// Closing
		private void TextureBrowserForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			int windowstate;

			// Determine window state to save
			if(this.WindowState != FormWindowState.Minimized)
				windowstate = (int)this.WindowState;
			else
				windowstate = (int)FormWindowState.Normal;

			// Save window settings
			General.Settings.WriteSetting("browserwindow.positionx", lastposition.X);
			General.Settings.WriteSetting("browserwindow.positiony", lastposition.Y);
			General.Settings.WriteSetting("browserwindow.sizewidth", lastsize.Width);
			General.Settings.WriteSetting("browserwindow.sizeheight", lastsize.Height);
			General.Settings.WriteSetting("browserwindow.windowstate", windowstate);

			//mxd. Save last selected texture set
			if(tvTextureSets.SelectedNode != null)
				General.Settings.WriteSetting("browserwindow.textureset", tvTextureSets.SelectedNode.Name);
			
			// Clean up
			browser.CleanUp();
		}

		// Static method to browse for texture or flat
		// Returns null when cancelled.
		public static string Browse(IWin32Window parent, string select, bool browseFlats)
		{
			TextureBrowserForm browser = new TextureBrowserForm(select, browseFlats);
			if(browser.ShowDialog(parent) == DialogResult.OK)
				return browser.SelectedName; // Return result
			
			// Cancelled
			return select;
		}

		// Item double clicked
		private void browser_SelectedItemDoubleClicked()
		{
			if(apply.Enabled) apply_Click(this, EventArgs.Empty);
		}

		// This fills the list of textures, depending on the selected texture set
		private void FillImagesList()
		{
			// Get the selected texture set
			IFilledTextureSet set = (selectedset.Tag as IFilledTextureSet);

			// Start adding
			browser.BeginAdding(false);

			if (browseFlats) {
				// Add all available flats
				foreach(ImageData img in set.Flats)
					browser.Add(img.Name, img, img, availgroup);

				// Add all used flats
				foreach(ImageData img in set.Flats)
					if(img.UsedInMap) browser.Add(img.Name, img, img, usedgroup);
			}else{
				// Add all available textures and mark the images for temporary loading
				foreach (ImageData img in set.Textures)
					browser.Add(img.Name, img, img, availgroup);

				// Add all used textures and mark the images for permanent loading
				foreach (ImageData img in set.Textures)
					if (img.UsedInMap) browser.Add(img.Name, img, img, usedgroup);
			}
			
			// Done adding
			browser.EndAdding();
		}

		// Help
		private void TextureBrowserForm_HelpRequested(object sender, HelpEventArgs hlpevent)
		{
			General.ShowHelp("w_imagesbrowser.html");
			hlpevent.Handled = true;
		}

		private void TextureBrowserForm_Shown(object sender, EventArgs e)
		{
			if (selectedset != null) //mxd. Calling FillImagesList() from constructor leads to TERRIBLE load times. Why? I have no sodding idea...
				FillImagesList();
			
			// Select texture
			if(!string.IsNullOrEmpty(selecttextureonfill))
			{
				browser.SelectItem(selecttextureonfill, usedgroup);
				selecttextureonfill = null;
			}

			//mxd. Focus the textbox. Calling this from TextureBrowserForm_Activated (like it's done in DB2) fails when the form is maximized. Again, I've no idea why...
			browser.FocusTextbox();
		}

		//mxd
		private void tvTextureSets_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e) {
			selectedset = e.Node;
			FillImagesList();
		}

		//mxd
		private void tvTextureSets_KeyUp(object sender, KeyEventArgs e) {
			if(tvTextureSets.SelectedNode != selectedset) {
				selectedset = tvTextureSets.SelectedNode;
				FillImagesList();
			}
		}
	}
}