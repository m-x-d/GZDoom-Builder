
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

#endregion

namespace CodeImp.DoomBuilder.Windows
{
	internal partial class ActionBrowserForm : DelayedForm
	{
		// Constants
		private const int MAX_OPTIONS = 8;
		
		// Variables
		private int selectedaction;
		private readonly ComboBox[] options;
		private readonly Label[] optionlbls;
		private readonly TreeNode[] allNodes; //mxd
		
		// Properties
		public int SelectedAction { get { return selectedaction; } }
		
		// Constructor
		public ActionBrowserForm(int action)
		{
			GeneralizedCategory sc;
			int actionbits;
			
			// Initialize
			InitializeComponent();

			// Make array references for controls
			options = new[] { option0, option1, option2, option3, option4, option5, option6, option7 };
			optionlbls = new[] { option0label, option1label, option2label, option3label, option4label,
									   option5label, option6label, option7label };
			
			// Show prefixes panel only for doom type maps
			if(!General.Map.FormatInterface.HasBuiltInActivations)
			{
				prefixespanel.Visible = false;
				actions.Height += prefixespanel.Height + 3; //mxd
			}
			
			// Go for all predefined categories
			createActionCategories(action);
			allNodes = new TreeNode[actions.Nodes.Count];
			actions.Nodes.CopyTo(allNodes, 0);

			// Using generalized actions?
			if(General.Map.Config.GeneralizedActions)
			{
				// Add for all generalized categories to the combobox
				category.Items.AddRange(General.Map.Config.GenActionCategories.ToArray());

				// Given action is generalized?
				if(GameConfiguration.IsGeneralized(action, General.Map.Config.GenActionCategories))
				{
					// Open the generalized tab
					tabs.SelectedTab = tabgeneralized;

					// Select category
					foreach(GeneralizedCategory ac in category.Items)
						if((action >= ac.Offset) && (action < (ac.Offset + ac.Length))) category.SelectedItem = ac;

					// Anything selected?
					if(category.SelectedIndex > -1)
					{
						// Go for all options in selected category
						sc = category.SelectedItem as GeneralizedCategory;
						actionbits = action - sc.Offset;
						for(int i = 0; i < MAX_OPTIONS; i++)
						{
							// Option used?
							if(i < sc.Options.Count)
							{
								// Go for all bits
								foreach(GeneralizedBit ab in sc.Options[i].Bits)
								{
									// Select this setting if matches
									if((actionbits & ab.Index) == ab.Index) options[i].SelectedItem = ab;
								}
							}
						}
					}
				}
			}
			else
			{
				// Remove generalized tab
				tabs.TabPages.Remove(tabgeneralized);
			}

			//tbFilter.Focus(); //mxd
		}
		
		// This browses for an action
		// Returns the new action or the same action when cancelled
		public static int BrowseAction(IWin32Window owner, int action)
		{
			ActionBrowserForm f = new ActionBrowserForm(action);
			if(f.ShowDialog(owner) == DialogResult.OK) action = f.SelectedAction;
			f.Dispose();
			return action;
		}

		//mxd
		private void createActionCategories(int action) {
			TreeNode cn, n;

			actions.BeginUpdate();
			actions.ShowLines = true;
			foreach (LinedefActionCategory ac in General.Map.Config.ActionCategories) {
				// Empty category names will not be created
				// (those actions will go in the root of the tree)
				if (ac.Title.Length > 0) {
					// Create category
					cn = actions.Nodes.Add(ac.Title);
					foreach (LinedefActionInfo ai in ac.Actions) {
						// Create action
						n = cn.Nodes.Add(ai.Title);
						n.Tag = ai;

						// This is the given action?
						if (ai.Index == action) {
							// Select this and expand the category
							cn.Expand();
							actions.SelectedNode = n;
							n.EnsureVisible();
						}
					}
				} else {
					// Put actions in the tree root
					foreach (LinedefActionInfo ai in ac.Actions) {
						// Create action
						n = actions.Nodes.Add(ai.Title);
						n.Tag = ai;
					}
				}
			}
			actions.EndUpdate();
		}

		//mxd
		private void filterActions(string p) {
			List<TreeNode> filteredNodes = new List<TreeNode>();

			foreach (TreeNode n in allNodes) {
				foreach (TreeNode cn in n.Nodes) {
					LinedefActionInfo ai = cn.Tag as LinedefActionInfo;

					if (ai.Title.ToLowerInvariant().IndexOf(p) != -1)
						filteredNodes.Add(cn);
				}
			}

			actions.BeginUpdate();
			actions.Nodes.Clear();
			actions.ShowLines = false;
			actions.Nodes.AddRange(filteredNodes.ToArray());
			actions.EndUpdate();
		}
		
		// OK clicked
		private void apply_Click(object sender, EventArgs e)
		{
			GeneralizedCategory sc;
			
			// Presume no result
			selectedaction = 0;
			
			// Predefined action?
			if(tabs.SelectedTab == tabactions)
			{
				// Action node selected?
				if((actions.SelectedNode != null) && (actions.SelectedNode.Tag is LinedefActionInfo))
				{
					// Our result
					selectedaction = (actions.SelectedNode.Tag as LinedefActionInfo).Index;
				}
			}
			// Generalized action
			else
			{
				// Category selected?
				if(category.SelectedIndex > -1)
				{
					// Add category bits and go for all options
					sc = category.SelectedItem as GeneralizedCategory;
					selectedaction = sc.Offset;
					for(int i = 0; i < MAX_OPTIONS; i++)
					{
						// Option used?
						if(i < sc.Options.Count)
						{
							// Add selected bits
							if(options[i].SelectedIndex > -1)
								selectedaction += (options[i].SelectedItem as GeneralizedBit).Index;
						}
					}
				}
			}
			
			// Done
			this.DialogResult = DialogResult.OK;
			this.Close();
		}

		// Cancel clicked
		private void cancel_Click(object sender, EventArgs e)
		{
			// Leave
			this.DialogResult = DialogResult.Cancel;
			this.Close();
		}

		// Generalized category selected
		private void category_SelectedIndexChanged(object sender, EventArgs e)
		{
			GeneralizedCategory ac;
			
			// Category selected?
			if(category.SelectedIndex > -1)
			{
				// Get the category
				ac = category.SelectedItem as GeneralizedCategory;
				
				// Go for all options
				for(int i = 0; i < MAX_OPTIONS; i++)
				{
					// Option used in selected category?
					if(i < ac.Options.Count)
					{
						// Setup controls
						optionlbls[i].Text = ac.Options[i].Name + ":";
						options[i].Items.Clear();
						options[i].Items.AddRange(ac.Options[i].Bits.ToArray());
						
						// Show option
						options[i].Visible = true;
						optionlbls[i].Visible = true;
					}
					else
					{
						// Hide option
						options[i].Visible = false;
						optionlbls[i].Visible = false;
					}
				}
			}
			else
			{
				// Hide all options
				for(int i = 0; i < MAX_OPTIONS; i++)
				{
					options[i].Visible = false;
					optionlbls[i].Visible = false;
				}
			}
		}
		
		// Double clicking on item
		private void actions_DoubleClick(object sender, EventArgs e)
		{
			// Action node selected?
			if((actions.SelectedNode != null) && (actions.SelectedNode.Tag is LinedefActionInfo))
			{
				if(apply.Enabled) apply_Click(this, EventArgs.Empty);
			}
		}

		//mxd
		private void tbFilter_TextChanged(object sender, EventArgs e) {
			if (tbFilter.Text.Length > 1) {
				filterActions(tbFilter.Text);
			} else if (String.IsNullOrEmpty(tbFilter.Text.ToLowerInvariant())) {
				actions.Nodes.Clear();
				createActionCategories(actions.SelectedNode != null ? ((LinedefActionInfo)actions.SelectedNode.Tag).Index : 0);
			}
		}

		//mxd
		private void btnClearFilter_Click(object sender, EventArgs e) {
			tbFilter.Clear();
		}

		//mxd
		private void ActionBrowserForm_Shown(object sender, EventArgs e) {
			tbFilter.Focus();
		}
	}
}
