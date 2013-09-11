
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
	internal partial class EffectBrowserForm : DelayedForm
	{
		// Constants
		private const int MAX_OPTIONS = 8;
		
		// Variables
		private int selectedeffect;
		private ComboBox[] options;
		private Label[] optionlbls;
		private ListViewItem[] allItems; //mxd
		
		// Properties
		public int SelectedEffect { get { return selectedeffect; } }
		
		// Constructor
		public EffectBrowserForm(int effect)
		{
			GeneralizedOption o;
			//ListViewItem n;
			bool selected;
			
			// Initialize
			InitializeComponent();

			// Make array references for controls
			options = new ComboBox[] { option0, option1, option2, option3, option4, option5, option6, option7 };
			optionlbls = new Label[] { option0label, option1label, option2label, option3label, option4label,
									   option5label, option6label, option7label };
			
			// Go for all predefined effects
			selected = createEffects(effect); //mxd
			allItems = new ListViewItem[effects.Items.Count]; //mxd
			effects.Items.CopyTo(allItems, 0); //mxd
			
			// Using generalized effects?
			if(General.Map.Config.GeneralizedEffects)
			{
				// Go for all options
				for(int i = 0; i < MAX_OPTIONS; i++)
				{
					// Option used in selected category?
					if(i < General.Map.Config.GenEffectOptions.Count)
					{
						o = General.Map.Config.GenEffectOptions[i];
						
						// Setup controls
						optionlbls[i].Text = o.Name + ":";
						options[i].Items.Clear();
						options[i].Items.AddRange(o.Bits.ToArray());
						
						// Show option
						options[i].Visible = true;
						optionlbls[i].Visible = true;

						if(effects.SelectedItems.Count == 0)
						{
							// Go for all bits
							foreach(GeneralizedBit ab in o.Bits)
							{
								// Select this setting if matches
								if((effect & ab.Index) == ab.Index) options[i].SelectedItem = ab;
							}
						}
					}
					else
					{
						// Hide option
						options[i].Visible = false;
						optionlbls[i].Visible = false;
					}
				}
				
				// Open the generalized tab when given effect is generalized
				if(!selected) tabs.SelectedTab = tabgeneralized;
			}
			else
			{
				// Remove generalized tab
				tabs.TabPages.Remove(tabgeneralized);
			}
		}
		
		// This browses for an effect
		// Returns the new effect or the same effect when cancelled
		public static int BrowseEffect(IWin32Window owner, int effect)
		{
			EffectBrowserForm f = new EffectBrowserForm(effect);
			if(f.ShowDialog(owner) == DialogResult.OK) effect = f.SelectedEffect;
			f.Dispose();
			return effect;
		}

		//mxd
		private bool createEffects(int effect) {
			bool selected = false;
			ListViewItem n;

			foreach (SectorEffectInfo si in General.Map.Config.SortedSectorEffects) {
				// Create effect
				n = effects.Items.Add(si.Index.ToString());
				n.SubItems.Add(si.Title);
				n.Tag = si;
				if (si.Index == effect) {
					selected = true;
					n.Selected = true;
				}
			}
			return selected;
		}

		//mxd
		private void filterEffects(string p) {
			List<ListViewItem> filteredItems = new List<ListViewItem>();

			foreach (ListViewItem i in allItems) {
				SectorEffectInfo si = i.Tag as SectorEffectInfo;

				if (si.Title.ToLowerInvariant().IndexOf(p) != -1)
					filteredItems.Add(i);
			}

			effects.BeginUpdate();
			effects.Items.Clear();
			effects.Items.AddRange(filteredItems.ToArray());
			effects.EndUpdate();
		}
		
		// OK clicked
		private void apply_Click(object sender, EventArgs e)
		{
			// Presume no result
			selectedeffect = 0;
			
			// Predefined action?
			if(tabs.SelectedTab == tabeffects)
			{
				// Effect selected?
				if((effects.SelectedItems.Count > 0) && (effects.SelectedItems[0].Tag is SectorEffectInfo))
				{
					// Our result
					selectedeffect = (effects.SelectedItems[0].Tag as SectorEffectInfo).Index;
				}
			}
			// Generalized action
			else
			{
				// Go for all options
				for(int i = 0; i < MAX_OPTIONS; i++)
				{
					// Option used?
					if(i < General.Map.Config.GenEffectOptions.Count)
					{
						// Add selected bits
						if(options[i].SelectedIndex > -1)
							selectedeffect += (options[i].SelectedItem as GeneralizedBit).Index;
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
		
		// Double-clicked on item
		private void effects_DoubleClick(object sender, EventArgs e)
		{
			// Effect selected?
			if((effects.SelectedItems.Count > 0) && (effects.SelectedItems[0].Tag is SectorEffectInfo))
			{
				if(apply.Enabled) apply_Click(this, EventArgs.Empty);
			}
		}

		//mxd
		private void tbFilter_TextChanged(object sender, EventArgs e) {
			if (tbFilter.Text.Length > 1) {
				filterEffects(tbFilter.Text);
			} else if (String.IsNullOrEmpty(tbFilter.Text.ToLowerInvariant())) {
				effects.Items.Clear();
				createEffects(effects.SelectedItems.Count > 0 ? ((SectorEffectInfo)effects.SelectedItems[0].Tag).Index : 0);
			}
		}

		//mxd
		private void btnClearFilter_Click(object sender, EventArgs e) {
			tbFilter.Clear();
		}
	}
}