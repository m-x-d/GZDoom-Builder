
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
using System.Diagnostics;
using CodeImp.DoomBuilder.Data;
using CodeImp.DoomBuilder.Map;

#endregion

namespace CodeImp.DoomBuilder.Interface
{
	public partial class LinedefInfoPanel : UserControl
	{
		// Constructor
		public LinedefInfoPanel()
		{
			// Initialize
			InitializeComponent();
		}
		
		// This shows the info
		public void ShowInfo(Linedef l)
		{
			// TODO: Get line action information
			
			// Linedef info
			action.Text = l.Action.ToString();
			length.Text = l.Length.ToString("0.##");
			angle.Text = l.AngleDeg.ToString() + "\u00B0";
			tag.Text = l.Tag.ToString();

			// Front side available?
			if(l.Front != null)
			{
				// Show sidedef info
				frontoffset.Text = l.Front.OffsetX + ", " + l.Front.OffsetY;
				fronthighname.Text = l.Front.HighTexture;
				frontmidname.Text = l.Front.MiddleTexture;
				frontlowname.Text = l.Front.LowTexture;
				fronthightex.BackgroundImage = General.Map.Data.GetTextureBitmap(l.Front.HighTexture);
				frontmidtex.BackgroundImage = General.Map.Data.GetTextureBitmap(l.Front.MiddleTexture);
				frontlowtex.BackgroundImage = General.Map.Data.GetTextureBitmap(l.Front.LowTexture);
				frontoffsetlabel.Enabled = true;
				frontoffset.Enabled = true;
				frontpanel.Enabled = true;
			}
			else
			{
				// Show no info
				frontoffsetlabel.Enabled = false;
				frontoffset.Enabled = false;
				frontpanel.Enabled = false;
				frontoffset.Text = "--, --";
				fronthighname.Text = "";
				frontmidname.Text = "";
				frontlowname.Text = "";
				fronthightex.BackgroundImage = null;
				frontmidtex.BackgroundImage = null;
				frontlowtex.BackgroundImage = null;
			}

			// Back size available?
			if(l.Back != null)
			{
				// Show sidedef info
				backoffset.Text = l.Back.OffsetX + ", " + l.Back.OffsetY;
				backhighname.Text = l.Back.HighTexture;
				backmidname.Text = l.Back.MiddleTexture;
				backlowname.Text = l.Back.LowTexture;
				backhightex.BackgroundImage = General.Map.Data.GetTextureBitmap(l.Back.HighTexture);
				backmidtex.BackgroundImage = General.Map.Data.GetTextureBitmap(l.Back.MiddleTexture);
				backlowtex.BackgroundImage = General.Map.Data.GetTextureBitmap(l.Back.LowTexture);
				backoffsetlabel.Enabled = true;
				backoffset.Enabled = true;
				backpanel.Enabled = true;
			}
			else
			{
				// Show no info
				backoffsetlabel.Enabled = false;
				backoffset.Enabled = false;
				backpanel.Enabled = false;
				backoffset.Text = "--, --";
				backhighname.Text = "";
				backmidname.Text = "";
				backlowname.Text = "";
				backhightex.BackgroundImage = null;
				backmidtex.BackgroundImage = null;
				backlowtex.BackgroundImage = null;
			}

			// Show the whole thing
			this.Show();
			this.Update();
		}

		// When visible changed
		protected override void OnVisibleChanged(EventArgs e)
		{
			// Hiding panels
			if(!this.Visible)
			{
				fronthightex.BackgroundImage = null;
				frontmidtex.BackgroundImage = null;
				frontlowtex.BackgroundImage = null;
				backhightex.BackgroundImage = null;
				backmidtex.BackgroundImage = null;
				backlowtex.BackgroundImage = null;
			}

			// Call base
			base.OnVisibleChanged(e);
		}
	}
}
