
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
using CodeImp.DoomBuilder.Controls;

#endregion

namespace CodeImp.DoomBuilder.Windows
{
	internal partial class TextureSetForm : DelayedForm
	{
		// Variables
		private DefinedTextureSet textureset;
		
		// Constructor
		public TextureSetForm()
		{
			InitializeComponent();
		}
		
		// This initializes the set
		public void Setup(DefinedTextureSet set)
		{
			// Keep reference
			textureset = set;

			// Set name
			name.Text = set.Name;

			// Fill filters list
			foreach(string s in set.Filters)
				filters.Items.Add(s);
		}

		// OK clicked
		private void apply_Click(object sender, EventArgs e)
		{
			// Apply name
			textureset.Name = name.Text;

			// Apply filters
			textureset.Filters.Clear();
			foreach(ListViewItem i in filters.Items) textureset.Filters.Add(i.Text);

			// Done
			this.DialogResult = DialogResult.OK;
			this.Close();
		}

		// Cancel clicked
		private void cancel_Click(object sender, EventArgs e)
		{
			// Be gone.
			this.DialogResult = DialogResult.Cancel;
			this.Close();
		}

	}
}