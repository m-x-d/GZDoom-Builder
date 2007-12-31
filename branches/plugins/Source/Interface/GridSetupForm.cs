
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

#endregion

namespace CodeImp.DoomBuilder.Interface
{
	public partial class GridSetupForm : DelayedForm
	{
		// Variables
		private string backgroundname;
		private int backgroundsource;
		
		// Constructor
		public GridSetupForm()
		{
			// Initialize
			InitializeComponent();

			// Show grid size
			gridsize.Value = General.Map.Grid.GridSize;
			
			// Background image?
			if((General.Map.Grid.Background != null) &&
			   !(General.Map.Grid.Background is NullImage))
			{
				// Show background image
				showbackground.Checked = true;
				backgroundname = General.Map.Grid.BackgroundName;
				backgroundsource = General.Map.Grid.BackgroundSource;
				General.DisplayZoomedImage(backgroundimage, General.Map.Grid.Background.Bitmap);
			}
			else
			{
				// No background image
				showbackground.Checked = false;
			}

			// Show background offset
			backoffsetx.Value = General.Map.Grid.BackgroundX;
			backoffsety.Value = General.Map.Grid.BackgroundY;
		}

		// Show Background changed
		private void showbackground_CheckedChanged(object sender, EventArgs e)
		{
			// Enable/disable controls
			selecttexture.Enabled = showbackground.Checked;
			selectflat.Enabled = showbackground.Checked;
			backoffset.Enabled = showbackground.Checked;
			backoffsetx.Enabled = showbackground.Checked;
			backoffsety.Enabled = showbackground.Checked;
		}

		// Browse texture
		private void selecttexture_Click(object sender, EventArgs e)
		{
			string result;
			
			// Browse for texture
			result = TextureBrowserForm.Browse(this, backgroundname);
			if(result != null)
			{
				// Set this texture as background
				backgroundname = result;
				backgroundsource = GridSetup.SOURCE_TEXTURES;
				General.DisplayZoomedImage(backgroundimage, General.Map.Data.GetTextureBitmap(result));
			}
		}

		// Browse flat
		private void selectflat_Click(object sender, EventArgs e)
		{
			string result;

			// Browse for flat
			result = FlatBrowserForm.Browse(this, backgroundname);
			if(result != null)
			{
				// Set this flat as background
				backgroundname = result;
				backgroundsource = GridSetup.SOURCE_FLATS;
				General.DisplayZoomedImage(backgroundimage, General.Map.Data.GetFlatBitmap(result));
			}
		}
		
		// Cancelled
		private void cancel_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.Cancel;
			this.Close();
		}

		// Apply
		private void apply_Click(object sender, EventArgs e)
		{
			// Apply
			General.Map.Grid.SetGridSize((int)gridsize.Value);
			General.Map.Grid.SetBackgroundOffset((int)backoffsetx.Value, (int)backoffsety.Value);
			
			// Background image?
			if(showbackground.Checked)
			{
				// Set background image
				General.Map.Grid.SetBackground(backgroundname, backgroundsource);
			}
			else
			{
				// No background image
				General.Map.Grid.SetBackground(null, 0);
			}

			// Done
			DialogResult = DialogResult.OK;
			this.Close();
		}
	}
}