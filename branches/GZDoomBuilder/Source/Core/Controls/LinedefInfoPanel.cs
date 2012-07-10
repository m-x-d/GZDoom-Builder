
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
using CodeImp.DoomBuilder.Config;
using CodeImp.DoomBuilder.Types;
using CodeImp.DoomBuilder.IO;
using CodeImp.DoomBuilder.GZBuilder;

#endregion

namespace CodeImp.DoomBuilder.Controls
{
	internal partial class LinedefInfoPanel : UserControl
	{
		private int hexenformatwidth;
		private int doomformatwidth;
		
		// Constructor
		public LinedefInfoPanel()
		{
			// Initialize
			InitializeComponent();

			// Hide stuff when in Doom format
			hexenformatwidth = infopanel.Width;
			doomformatwidth = infopanel.Width - 190;
		}
		
		// This shows the info
		public void ShowInfo(Linedef l)
		{
			TypeHandler th;
			bool upperunpegged, lowerunpegged;
			string peggedness;
			
			// Show/hide stuff depending on format
			if(!General.Map.FormatInterface.HasActionArgs)
			{
				arglbl1.Visible = false;
				arglbl2.Visible = false;
				arglbl3.Visible = false;
				arglbl4.Visible = false;
				arglbl5.Visible = false;
				arg1.Visible = false;
				arg2.Visible = false;
				arg3.Visible = false;
				arg4.Visible = false;
				arg5.Visible = false;
				infopanel.Width = doomformatwidth;
			}
			else
			{
				arglbl1.Visible = true;
				arglbl2.Visible = true;
				arglbl3.Visible = true;
				arglbl4.Visible = true;
				arglbl5.Visible = true;
				arg1.Visible = true;
				arg2.Visible = true;
				arg3.Visible = true;
				arg4.Visible = true;
				arg5.Visible = true;
				infopanel.Width = hexenformatwidth;
			}

			// Move panels
			frontpanel.Left = infopanel.Left + infopanel.Width + infopanel.Margin.Right + frontpanel.Margin.Left;
			backpanel.Left = frontpanel.Left + frontpanel.Width + frontpanel.Margin.Right + backpanel.Margin.Left;
			
			// Get line action information
			LinedefActionInfo act = General.Map.Config.GetLinedefActionInfo(l.Action);
			
			// Determine peggedness
			upperunpegged = l.IsFlagSet(General.Map.Config.UpperUnpeggedFlag);
			lowerunpegged = l.IsFlagSet(General.Map.Config.LowerUnpeggedFlag);
			if(upperunpegged && lowerunpegged)
				peggedness = "Upper & Lower";
			else if(upperunpegged)
				peggedness = "Upper";
			else if(lowerunpegged)
				peggedness = "Lower";
			else
				peggedness = "None";
			
			// Linedef info
			infopanel.Text = " Linedef " + l.Index + " ";
			action.Text = act.ToString();
			length.Text = l.Length.ToString("0.##");
			angle.Text = l.AngleDeg.ToString() + "\u00B0";
			tag.Text = l.Tag.ToString();
			unpegged.Text = peggedness;

            //mxd
            bool hasArg0Str = General.Map.UDMF && Array.IndexOf(GZGeneral.ACS_SPECIALS, l.Action) != -1 && l.Fields.ContainsKey("arg0str");
			
			// Arguments
            arglbl1.Text = hasArg0Str ? "Script name:" : act.Args[0].Title + ":"; //mxd
			arglbl2.Text = act.Args[1].Title + ":";
			arglbl3.Text = act.Args[2].Title + ":";
			arglbl4.Text = act.Args[3].Title + ":";
			arglbl5.Text = act.Args[4].Title + ":";
			arglbl1.Enabled = act.Args[0].Used;
			arglbl2.Enabled = act.Args[1].Used;
			arglbl3.Enabled = act.Args[2].Used;
			arglbl4.Enabled = act.Args[3].Used;
			arglbl5.Enabled = act.Args[4].Used;
			arg1.Enabled = act.Args[0].Used;
			arg2.Enabled = act.Args[1].Used;
			arg3.Enabled = act.Args[2].Used;
			arg4.Enabled = act.Args[3].Used;
			arg5.Enabled = act.Args[4].Used;

            //mxd
            if (hasArg0Str) {
                arg1.Text = '"' + l.Fields["arg0str"].Value.ToString() + '"';
            } else {
                th = General.Types.GetArgumentHandler(act.Args[0]);
                th.SetValue(l.Args[0]); arg1.Text = th.GetStringValue();
            }
			th = General.Types.GetArgumentHandler(act.Args[1]);
			th.SetValue(l.Args[1]); arg2.Text = th.GetStringValue();
			th = General.Types.GetArgumentHandler(act.Args[2]);
			th.SetValue(l.Args[2]); arg3.Text = th.GetStringValue();
			th = General.Types.GetArgumentHandler(act.Args[3]);
			th.SetValue(l.Args[3]); arg4.Text = th.GetStringValue();
			th = General.Types.GetArgumentHandler(act.Args[4]);
			th.SetValue(l.Args[4]); arg5.Text = th.GetStringValue();

			// Front side available?
			if(l.Front != null)
			{
				// Show sidedef info
				frontpanel.Text = " Front Sidedef " + l.Front.Index + " ";
				frontsector.Text = " Sector " + l.Front.Sector.Index;
				frontsector.Visible = true;
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
				frontpanel.Text = " Front Sidedef ";
				frontsector.Text = "";
				frontsector.Visible = false;
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
				backpanel.Text = " Back Sidedef " + l.Back.Index + " ";
				backsector.Text = " Sector " + l.Back.Sector.Index;
				backsector.Visible = true;
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
				backpanel.Text = " Back Sidedef ";
				backsector.Text = "";
				backsector.Visible = false;
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
			
			// Position labels
			frontsector.Left = frontlowtex.Right - frontsector.Width;
			backsector.Left = backlowtex.Right - backsector.Width;

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
				panel.BackgroundImage = General.Map.Data.GetTextureImage(name).GetPreview();
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
