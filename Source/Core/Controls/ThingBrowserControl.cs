
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
using System.Windows.Forms;
using CodeImp.DoomBuilder.Config;
using CodeImp.DoomBuilder.Data;
using CodeImp.DoomBuilder.GZBuilder.Controls;

#endregion

namespace CodeImp.DoomBuilder.Controls
{
	public partial class ThingBrowserControl : UserControl
	{
		#region ================== Events

		public delegate void TypeChangedDeletegate(ThingTypeInfo value);
		public delegate void TypeDoubleClickDeletegate();

		public event TypeChangedDeletegate OnTypeChanged;
		public event TypeDoubleClickDeletegate OnTypeDoubleClicked;

		#endregion

		#region ================== Variables

		private List<TreeNode> nodes;
		private ThingTypeInfo thinginfo;
		private bool doupdatenode;
		private bool doupdatetextbox;
		private TreeNode doubleclickednode; //mxd
		
		#endregion

		#region ================== Properties

		public string TypeStringValue { get { return typeid.Text; } }
		public bool UseMultiSelection { get { return typelist.SelectionMode == TreeViewSelectionMode.MultiSelectSameLevel; } set { typelist.SelectionMode = (value ? TreeViewSelectionMode.MultiSelectSameLevel : TreeViewSelectionMode.SingleSelect); } }

		#endregion

		#region ================== Constructor

		// Constructor
		public ThingBrowserControl()
		{
			InitializeComponent();
		}

		// This sets up the control
		public void Setup()
		{
			// Go for all predefined categories
			typelist.Nodes.Clear();
			nodes = new List<TreeNode>();
			foreach(ThingCategory tc in General.Map.Data.ThingCategories)
			{
				// Create category
				TreeNode cn = typelist.Nodes.Add(tc.Name, tc.Title);
				if((tc.Color >= 0) && (tc.Color < thingimages.Images.Count)) cn.ImageIndex = tc.Color;
				cn.SelectedImageIndex = cn.ImageIndex;
				foreach(ThingTypeInfo ti in tc.Things)
				{
					// Create thing
					TreeNode n = cn.Nodes.Add(ti.Title);
					if((ti.Color >= 0) && (ti.Color < thingimages.Images.Count)) n.ImageIndex = ti.Color;
					n.SelectedImageIndex = n.ImageIndex;
					n.Tag = ti;
					nodes.Add(n);
				}
			}

			doupdatenode = true;
			doupdatetextbox = true;
		}

		#endregion
		
		#region ================== Methods

		// Select a type
		public void SelectType(int type)
		{
			// Set type index
			typeid.Text = type.ToString();
			typeid_TextChanged(this, EventArgs.Empty);
		}

		// Return selected type info
		public ThingTypeInfo GetSelectedInfo()
		{
			return thinginfo;
		}

		// This clears the type
		public void ClearSelectedType()
		{
			doupdatenode = false;

			// Clear selection
			typelist.SelectedNodes.Clear(); //mxd
			typeid.Text = "";

			// Collapse nodes
			foreach(TreeNode n in nodes)
				if(n.Parent.IsExpanded) n.Parent.Collapse();
			
			doupdatenode = true;
		}

		// Result
		public int GetResult(int original)
		{
			//mxd
			if(typelist.SelectionMode == TreeViewSelectionMode.MultiSelectSameLevel && typelist.SelectedNodes.Count > 1) 
			{
				List<TreeNode> validNodes = GetValidNodes();

				//get a random ThingTypeInfo from valid nodes
				if(validNodes.Count > 1) 
				{
					ThingTypeInfo ti = validNodes[General.Random(0, validNodes.Count - 1)].Tag as ThingTypeInfo;
					return ti.Index;
				}
			}
			
			return typeid.GetResult(original);
		}

		//mxd
		private List<TreeNode> GetValidNodes() 
		{
			List<TreeNode> validNodes = new List<TreeNode>();

			foreach(TreeNode n in typelist.SelectedNodes) 
			{
				if((n.Nodes.Count == 0) && (n.Tag is ThingTypeInfo)) validNodes.Add(n);
			}

			return validNodes;
		}

		// Update preview image (mxd)
		private void UpdateThingSprite() 
		{
			if(General.Map == null) return;
			if(thinginfo != null) 
			{
				if(thinginfo.Sprite.ToLowerInvariant().StartsWith(DataManager.INTERNAL_PREFIX) &&
				   (thinginfo.Sprite.Length > DataManager.INTERNAL_PREFIX.Length)) 
				{
					General.DisplayZoomedImage(spritetex, General.Map.Data.GetSpriteImage(thinginfo.Sprite).GetBitmap());
				} 
				else if((thinginfo.Sprite.Length < 9) && (thinginfo.Sprite.Length > 0))
				{
					ImageData sprite = General.Map.Data.GetSpriteImage(thinginfo.Sprite);
					General.DisplayZoomedImage(spritetex, sprite.GetPreview());
					if(!sprite.IsPreviewLoaded) updatetimer.Start();
				}
				else 
				{
					spritetex.BackgroundImage = null;
				}
			} 
			else 
			{
				spritetex.BackgroundImage = null;
			}
		}

		#endregion

		#region ================== Events

		// List double-clicked. e.Node and typelist.SelectedNodes[0] may contain incorrect node, 
		// so we set the correct one in typelist_AfterSelect handler (mxd)
		private void typelist_MouseDoubleClick(object sender, MouseEventArgs e) 
		{
			if (typelist.SelectedNodes.Count == 1
			    && doubleclickednode != null
			    && doubleclickednode.Nodes.Count == 0
			    && doubleclickednode.Tag is ThingTypeInfo
			    && OnTypeDoubleClicked != null
			    && typeid.Text.Length > 0)
			{
				OnTypeDoubleClicked();
			}
		}
		
		// Thing type selection changed
		private void typelist_AfterSelect(object sender, TreeViewEventArgs e)
		{
			doubleclickednode = null; //mxd
			if (!doupdatetextbox) return;

			//mxd. Got a valid multiselection? Well, can't show any useful info about that...
			if(typelist.SelectionMode == TreeViewSelectionMode.MultiSelectSameLevel && typelist.SelectedNodes.Count > 1 && GetValidNodes().Count > 1) 
			{
				doupdatenode = false;
				if (!string.IsNullOrEmpty(typeid.Text))
				{
					// Event will be raised in typeid_OnTextChanged
					typeid.Text = "";
				}
				else if (OnTypeChanged != null)
				{
					// Or raise event here
					UpdateThingSprite();
					OnTypeChanged(thinginfo);
				}
				doupdatenode = true;
			}
			else if(typelist.SelectedNodes.Count > 0) //Anything selected?
			{
				TreeNode n = typelist.SelectedNodes[0];

				// Node is a child node?
				if ((n.Nodes.Count == 0) && (n.Tag is ThingTypeInfo))
				{
					ThingTypeInfo ti = (n.Tag as ThingTypeInfo);

					// Show info
					doupdatenode = false;
					typeid.Text = ti.Index.ToString();
					doupdatenode = true;
					doubleclickednode = n; //mxd
				}
			}
		}

		// Thing type index changed
		private void typeid_TextChanged(object sender, EventArgs e)
		{
			bool knownthing = false;

			// Any text?
			if(typeid.Text.Length > 0)
			{
				// Get the info
				int typeindex = typeid.GetResult(0);
				thinginfo = General.Map.Data.GetThingInfoEx(typeindex);
				if(thinginfo != null)
				{
					knownthing = true;

					// Size
					sizelabel.Text = (thinginfo.Radius * 2) + " x " + thinginfo.Height;

					// Hangs from ceiling
					if(thinginfo.Hangs) positionlabel.Text = "Ceiling"; else positionlabel.Text = "Floor";

					// Blocking
					switch(thinginfo.Blocking)
					{
						case ThingTypeInfo.THING_BLOCKING_NONE: blockinglabel.Text = "No"; break;
						case ThingTypeInfo.THING_BLOCKING_FULL: blockinglabel.Text = "Completely"; break;
						case ThingTypeInfo.THING_BLOCKING_HEIGHT: blockinglabel.Text = "True-Height"; break;
						default: blockinglabel.Text = "Unknown"; break;
					}
				}

				if(doupdatenode)
				{
					doupdatetextbox = false;
					typelist.SelectedNodes.Clear();
					foreach(TreeNode n in nodes)
					{
						// Matching node?
						if((n.Tag as ThingTypeInfo).Index == typeindex)
						{
							// Select this
							if(typelist.Nodes.Contains(n.Parent)) //mxd. Tree node may've been removed during filtering
							{
								n.Parent.Expand();
								typelist.SelectedNodes.Add(n);
								n.EnsureVisible();
								break;
							}
						}
					}
					doupdatetextbox = true;
				}
			}
			else
			{
				thinginfo = null;
				if(doupdatenode) typelist.SelectedNodes.Clear();
			}

			// No known thing?
			if(!knownthing)
			{
				sizelabel.Text = "-";
				positionlabel.Text = "-";
				blockinglabel.Text = "-";
			}

			// Update icon (mxd)
			UpdateThingSprite();

			// Raise event
			if(OnTypeChanged != null) OnTypeChanged(thinginfo);
		}

		private void updatetimer_Tick(object sender, EventArgs e) 
		{
			updatetimer.Stop();
			UpdateThingSprite();
		}

		//mxd
		private void typelist_MouseEnter(object sender, EventArgs e) 
		{
			typelist.Focus();
		}

		//mxd
		private void bClear_Click(object sender, EventArgs e) 
		{
			tbFilter.Clear();
		}

		//mxd
		private void tbFilter_TextChanged(object sender, EventArgs e) 
		{
			typelist.SuspendLayout();

			if(string.IsNullOrEmpty(tbFilter.Text)) 
			{
				Setup();
				typeid_TextChanged(this, EventArgs.Empty);
			}
			else
			{
				// Go for all predefined categories
				typelist.SelectedNodes.Clear();
				typelist.Nodes.Clear();

				string match = tbFilter.Text.ToUpperInvariant();
				foreach(TreeNode node in nodes)
				{
					if(node.Text.ToUpperInvariant().Contains(match)) 
					{
						typelist.Nodes.Add(node);
					}
				}

				doupdatenode = true;
				doupdatetextbox = true;
			}

			typelist.ResumeLayout();
		}
		
		#endregion
	}
}
