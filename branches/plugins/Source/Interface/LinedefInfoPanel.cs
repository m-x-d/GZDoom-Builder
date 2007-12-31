
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
	internal partial class LinedefInfoPanel : UserControl
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
			string actioninfo = "";
			
			// Get line action information
			if(General.Map.Config.LinedefActions.ContainsKey(l.Action))
				actioninfo = General.Map.Config.LinedefActions[l.Action].ToString();
			else if(l.Action == 0)
				actioninfo = l.Action.ToString() + " - None";
			else
				actioninfo = l.Action.ToString() + " - Unknown";
			
			// Linedef info
			action.Text = actioninfo;
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
				DisplaySidedefTexture(fronthightex, l.Front.HighTexture, l.Front.HighRequired());
				DisplaySidedefTexture(frontmidtex, l.Front.MiddleTexture, l.Front.MiddleRequired());
				DisplaySidedefTexture(frontlowtex, l.Front.LowTexture, l.Front.LowRequired());
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
				DisplaySidedefTexture(backhightex, l.Back.HighTexture, l.Back.HighRequired());
				DisplaySidedefTexture(backmidtex, l.Back.MiddleTexture, l.Back.MiddleRequired());
				DisplaySidedefTexture(backlowtex, l.Back.LowTexture, l.Back.LowRequired());
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

		// This shows a sidedef texture in a panel
		private void DisplaySidedefTexture(Panel panel, string name, bool required)
		{
			// Check if name is a "none" texture
			if((name.Length < 1) || (name[0] == '-'))
			{
				// Determine image to show
				if(required)
					panel.BackgroundImage = CodeImp.DoomBuilder.Properties.Resources.MissingTexture;
				else
					panel.BackgroundImage = null;
			}
			else
			{
				// Set the image
				panel.BackgroundImage = General.Map.Data.GetTextureBitmap(name);
			}
			
			// Image not null?
			if(panel.BackgroundImage != null)
			{
				// Small enough to fit in panel?
				if((panel.BackgroundImage.Size.Width < panel.ClientRectangle.Width) &&
				   (panel.BackgroundImage.Size.Height < panel.ClientRectangle.Height))
				{
					// Display centered
					panel.BackgroundImageLayout = ImageLayout.Center;
				}
				else
				{
					// Display zoomed
					panel.BackgroundImageLayout = ImageLayout.Zoom;
				}
			}
		}
	}
}
