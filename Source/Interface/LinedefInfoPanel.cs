
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
			frontoffset.Visible = (l.Front != null);
			backoffset.Visible = (l.Front != null);

			// Front side available?
			if(l.Front != null)
			{
				// Show sidedef info
				frontoffset.Text = l.Front.OffsetX + ", " + l.Front.OffsetY;
				fronthighname.Text = l.Front.HighTexture;
				frontmidname.Text = l.Front.MiddleTexture;
				frontlowname.Text = l.Front.LowTexture;
				fronthightex.BackgroundImage = FindTexture(l.Front.HighTexture);
				frontmidtex.BackgroundImage = FindTexture(l.Front.MiddleTexture);
				frontlowtex.BackgroundImage = FindTexture(l.Front.LowTexture);
			}

			// Back size available?
			if(l.Back != null)
			{
				// Show sidedef info
				backoffset.Text = l.Back.OffsetX + ", " + l.Back.OffsetY;
				backhighname.Text = l.Back.HighTexture;
				backmidname.Text = l.Back.MiddleTexture;
				backlowname.Text = l.Back.LowTexture;
				backhightex.BackgroundImage = FindTexture(l.Back.HighTexture);
				backmidtex.BackgroundImage = FindTexture(l.Back.MiddleTexture);
				backlowtex.BackgroundImage = FindTexture(l.Back.LowTexture);
			}

			// Show panels
			frontpanel.Visible = (l.Front != null);
			backpanel.Visible = (l.Back != null);

			// Show the whole thing
			this.Show();
			this.Update();
		}

		// When visible changed
		protected override void OnVisibleChanged(EventArgs e)
		{
			// Hide panels
			if(!this.Visible)
			{
				frontpanel.Visible = false;
				backpanel.Visible = false;
			}

			// Call base
			base.OnVisibleChanged(e);
		}

		// This loads and returns the texture image if possible
		private Image FindTexture(string name)
		{
			ImageData img;

			// Get it
			img = General.Map.Data.GetTextureByName(name);
			img.LoadImage();
			return img.Bitmap;
		}
	}
}
