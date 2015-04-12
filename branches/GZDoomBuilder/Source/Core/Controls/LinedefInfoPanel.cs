
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
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using CodeImp.DoomBuilder.GZBuilder.Tools;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Config;
using CodeImp.DoomBuilder.Types;
using CodeImp.DoomBuilder.GZBuilder;
using CodeImp.DoomBuilder.Data;

#endregion

namespace CodeImp.DoomBuilder.Controls
{
	internal partial class LinedefInfoPanel : UserControl
	{
		private readonly int hexenformatwidth;
		private readonly int doomformatwidth;
		
		// Constructor
		public LinedefInfoPanel()
		{
			// Initialize
			InitializeComponent();

			// Hide stuff when in Doom format
			hexenformatwidth = infopanel.Width;
			doomformatwidth = infopanel.Width - 190;

			//mxd
			labelTextureFrontTop.BackColor = Color.FromArgb(128, labelTextureFrontTop.BackColor);
			labelTextureFrontMid.BackColor = Color.FromArgb(128, labelTextureFrontMid.BackColor);
			labelTextureFrontBottom.BackColor = Color.FromArgb(128, labelTextureFrontBottom.BackColor);

			labelTextureBackTop.BackColor = Color.FromArgb(128, labelTextureBackTop.BackColor);
			labelTextureBackMid.BackColor = Color.FromArgb(128, labelTextureBackMid.BackColor);
			labelTextureBackBottom.BackColor = Color.FromArgb(128, labelTextureBackBottom.BackColor);
		}
		
		// This shows the info
		public void ShowInfo(Linedef l, Sidedef highlightside)
		{
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

			//mxd. Hide activation or tag and rearrange labels 
			if(!General.Map.FormatInterface.HasBuiltInActivations && General.Map.FormatInterface.HasNumericLinedefActivations) //Hexen map format?
			{ 
				activation.Visible = true;
				activationlabel.Visible = true;
				taglabel.Visible = false;
				tag.Visible = false;
				
				//set activation
				foreach(LinedefActivateInfo ai in General.Map.Config.LinedefActivates) 
				{
					if(l.Activate == ai.Index) 
					{
						activation.Text = ai.Title;
						break;
					}
				}

				activation.Enabled = (l.Activate != 0); //mxd
				activationlabel.Enabled = (l.Activate != 0); //mxd
			} 
			else 
			{
				activation.Visible = false;
				activationlabel.Visible = false;
				taglabel.Visible = true;
				tag.Visible = true;

				//set tag
				tag.Text = l.Tag + (General.Map.Options.TagLabels.ContainsKey(l.Tag) ? " - " + General.Map.Options.TagLabels[l.Tag] : string.Empty);
				tag.Enabled = (l.Tag != 0);
				taglabel.Enabled = (l.Tag != 0);
			}
			
			// Get line action information
			LinedefActionInfo act = General.Map.Config.GetLinedefActionInfo(l.Action);
			
			// Determine peggedness
			bool upperunpegged = l.IsFlagSet(General.Map.Config.UpperUnpeggedFlag);
			bool lowerunpegged = l.IsFlagSet(General.Map.Config.LowerUnpeggedFlag);
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
			angle.Text = l.AngleDeg + "\u00B0";
			unpegged.Text = peggedness;
			unpegged.Enabled = (peggedness != "None"); //mxd
			peglabel.Enabled = (peggedness != "None"); //mxd
			action.Enabled = (act.Index != 0);
			actionlabel.Enabled = (act.Index != 0);

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
			if (hasArg0Str) 
				arg1.Text = '"' + l.Fields["arg0str"].Value.ToString() + '"';
			else 
				SetArgumentText(act.Args[0], arg1, l.Args[0]);
			SetArgumentText(act.Args[1], arg2, l.Args[1]);
			SetArgumentText(act.Args[2], arg3, l.Args[2]);
			SetArgumentText(act.Args[3], arg4, l.Args[3]);
			SetArgumentText(act.Args[4], arg5, l.Args[4]);

			// Front side available?
			if(l.Front != null)
			{
				//mxd. Extended info shown?
				bool hasTopFields = false;
				bool hasMiddleFields = false;
				bool hasBottomFields = false;

				//mxd. Highlight this side?
				bool highlight = (l.Front == highlightside);
				frontpanel.ForeColor = (highlight ? SystemColors.HotTrack : SystemColors.WindowText); //mxd
				
				// Show sidedef info
				frontpanel.Visible = true; //mxd
				frontpanel.Text = " Front Sidedef " + l.Front.Index;
				
				//mxd
				if(General.Map.UDMF) 
				{
					//light
					frontoffsetlabel.Text = "Front light:";
					SetUDMFLight(l.Front, frontoffsetlabel, frontoffset, highlight);

					//global offset, sector index
					frontpanel.Text += ". Offset " + l.Front.OffsetX + ", " + l.Front.OffsetY + ". Sector " + l.Front.Sector.Index + " ";
					
					//sidedef top
					hasTopFields = SetPairedUDMFFieldsLabel(l.Front.Fields, "offsetx_top", "offsety_top", 0.0f, frontTopUDMFOffsetLabel, frontTopUDMFOffset, highlight);
					hasTopFields |= SetPairedUDMFFieldsLabel(l.Front.Fields, "scalex_top", "scaley_top", 1.0f, frontTopUDMFScaleLabel, frontTopUDMFScale, highlight);

					//sidedef middle
					hasMiddleFields = SetPairedUDMFFieldsLabel(l.Front.Fields, "offsetx_mid", "offsety_mid", 0.0f, frontMidUDMFOffsetLabel, frontMidUDMFOffset, highlight);
					hasMiddleFields |= SetPairedUDMFFieldsLabel(l.Front.Fields, "scalex_mid", "scaley_mid", 1.0f, frontMidUDMFScaleLabel, frontMidUDMFScale, highlight);

					//sidedef bottom
					hasBottomFields = SetPairedUDMFFieldsLabel(l.Front.Fields, "offsetx_bottom", "offsety_bottom", 0.0f, frontBottomUDMFOffsetLabel, frontBottomUDMFOffset, highlight);
					hasBottomFields |= SetPairedUDMFFieldsLabel(l.Front.Fields, "scalex_bottom", "scaley_bottom", 1.0f, frontBottomUDMFScaleLabel, frontBottomUDMFScale, highlight);

					//visibility
					frontTopUDMFOffset.Visible = hasTopFields;
					frontTopUDMFOffsetLabel.Visible = hasTopFields;
					frontTopUDMFScale.Visible = hasTopFields;
					frontTopUDMFScaleLabel.Visible = hasTopFields;

					frontMidUDMFOffset.Visible = hasMiddleFields;
					frontMidUDMFOffsetLabel.Visible = hasMiddleFields;
					frontMidUDMFScale.Visible = hasMiddleFields;
					frontMidUDMFScaleLabel.Visible = hasMiddleFields;

					frontBottomUDMFOffset.Visible = hasBottomFields;
					frontBottomUDMFOffsetLabel.Visible = hasBottomFields;
					frontBottomUDMFScale.Visible = hasBottomFields;
					frontBottomUDMFScaleLabel.Visible = hasBottomFields;
				} 
				else 
				{
					frontoffsetlabel.Text = "Front offset:";
					if (l.Front.OffsetX != 0 || l.Front.OffsetY != 0)
					{
						frontoffset.Text = l.Front.OffsetX + ", " + l.Front.OffsetY;
						frontoffsetlabel.Enabled = true;
						frontoffset.Enabled = true;

						frontoffset.ForeColor = (highlight ? SystemColors.HotTrack : SystemColors.WindowText);
						frontoffsetlabel.ForeColor = frontoffset.ForeColor;
					}
					else
					{
						frontoffset.Text = "--, --";
						frontoffsetlabel.Enabled = false;
						frontoffset.Enabled = false;
					}

					//mxd. Sector index
					frontpanel.Text += ". Sector " + l.Front.Sector.Index + " ";
				}

				//mxd. Set texture names, update panel sizes
				UpdateTexturePanel(panelFrontTop, l.Front.HighTexture, fronthighname, labelTextureFrontTop,
					Math.Max(frontTopUDMFOffset.Right, frontTopUDMFScale.Right) + 4, fronthightex,
					frontTopUDMFOffsetLabel.Left, hasTopFields, l.Front.HighRequired());

				UpdateTexturePanel(panelFrontMid, l.Front.MiddleTexture, frontmidname, labelTextureFrontMid,
					Math.Max(frontMidUDMFOffset.Right, frontMidUDMFScale.Right) + 4, frontmidtex,
					frontMidUDMFOffsetLabel.Left, hasMiddleFields, l.Front.MiddleRequired());

				UpdateTexturePanel(panelFrontLow, l.Front.LowTexture, frontlowname, labelTextureFrontBottom,
					Math.Max(frontBottomUDMFOffset.Right, frontBottomUDMFScale.Right) + 4, frontlowtex,
					frontBottomUDMFOffsetLabel.Left, hasBottomFields, l.Front.LowRequired());

				//mxd. Resize panel
				flowLayoutPanelFront.Width = panelFrontLow.Right;
				frontpanel.Width = flowLayoutPanelFront.Width + flowLayoutPanelFront.Left * 2 - 4;
			}
			else
			{
				// Show no info
				if(General.Map.UDMF) //mxd
				{
					frontoffsetlabel.Text = "Front light:";
					frontoffset.Text = "--";
				} 
				else 
				{
					frontoffsetlabel.Text = "Front offset:";
					frontoffset.Text = "--, --";
				}

				frontoffsetlabel.Enabled = false;
				frontoffset.Enabled = false;

				fronthightex.BackgroundImage = null;
				frontmidtex.BackgroundImage = null;
				frontlowtex.BackgroundImage = null;

				frontpanel.Visible = false; //mxd
			}

			// Back size available?
			if(l.Back != null)
			{
				//mxd. Extended info shown?
				bool hasTopFields = false;
				bool hasMiddleFields = false;
				bool hasBottomFields = false;

				//mxd. Highlight this side?
				bool highlight = l.Back == highlightside;
				backpanel.ForeColor = (highlight ? SystemColors.HotTrack : SystemColors.WindowText); //mxd

				// Show sidedef info
				backpanel.Visible = true; //mxd
				backpanel.Text = " Back Sidedef " + l.Back.Index;

				//mxd
				if(General.Map.UDMF) 
				{
					//light
					backoffsetlabel.Text = "Back light:";
					SetUDMFLight(l.Back, backoffsetlabel, backoffset, highlight);

					//global offset, sector index
					backpanel.Text += ". Offset " + l.Back.OffsetX + ", " + l.Back.OffsetY + ". Sector " + l.Back.Sector.Index + " ";

					//sidedef top
					hasTopFields = SetPairedUDMFFieldsLabel(l.Back.Fields, "offsetx_top", "offsety_top", 0f, backTopUDMFOffsetLabel, backTopUDMFOffset, highlight);
					hasTopFields |= SetPairedUDMFFieldsLabel(l.Back.Fields, "scalex_top", "scaley_top", 1.0f, backTopUDMFScaleLabel, backTopUDMFScale, highlight);

					//sidedef middle
					hasMiddleFields = SetPairedUDMFFieldsLabel(l.Back.Fields, "offsetx_mid", "offsety_mid", 0f, backMidUDMFOffsetLabel, backMidUDMFOffset, highlight);
					hasMiddleFields |= SetPairedUDMFFieldsLabel(l.Back.Fields, "scalex_mid", "scaley_mid", 1.0f, backMidUDMFScaleLabel, backMidUDMFScale, highlight);

					//sidedef bottom
					hasBottomFields = SetPairedUDMFFieldsLabel(l.Back.Fields, "offsetx_bottom", "offsety_bottom", 0f, backBottomUDMFOffsetLabel, backBottomUDMFOffset, highlight);
					hasBottomFields |= SetPairedUDMFFieldsLabel(l.Back.Fields, "scalex_bottom", "scaley_bottom", 1.0f, backBottomUDMFScaleLabel, backBottomUDMFScale, highlight);

					//visibility
					backTopUDMFOffset.Visible = hasTopFields;
					backTopUDMFOffsetLabel.Visible = hasTopFields;
					backTopUDMFScale.Visible = hasTopFields;
					backTopUDMFScaleLabel.Visible = hasTopFields;

					backMidUDMFOffset.Visible = hasMiddleFields;
					backMidUDMFOffsetLabel.Visible = hasMiddleFields;
					backMidUDMFScale.Visible = hasMiddleFields;
					backMidUDMFScaleLabel.Visible = hasMiddleFields;

					backBottomUDMFOffset.Visible = hasBottomFields;
					backBottomUDMFOffsetLabel.Visible = hasBottomFields;
					backBottomUDMFScale.Visible = hasBottomFields;
					backBottomUDMFScaleLabel.Visible = hasBottomFields;
				}
				else
				{
					backoffsetlabel.Text = "Back offset:";
					if (l.Back.OffsetX != 0 || l.Back.OffsetY != 0)
					{
						backoffset.Text = l.Back.OffsetX + ", " + l.Back.OffsetY;
						backoffsetlabel.Enabled = true;
						backoffset.Enabled = true;

						backoffset.ForeColor = (highlight ? SystemColors.HotTrack : SystemColors.WindowText);
						backoffsetlabel.ForeColor = backoffset.ForeColor;
					}
					else
					{
						backoffset.Text = "--, --";
						backoffsetlabel.Enabled = false;
						backoffset.Enabled = false;
					}

					// Sector index
					backpanel.Text += ". Sector " + l.Back.Sector.Index + " ";
				}

				//mxd. Set texture names, update panel sizes
				UpdateTexturePanel(panelBackTop, l.Back.HighTexture, backhighname, labelTextureBackTop, 
					Math.Max(backTopUDMFOffset.Right, backTopUDMFScale.Right) + 4, backhightex,
					backTopUDMFOffsetLabel.Left, hasTopFields, l.Back.HighRequired());

				UpdateTexturePanel(panelBackMid, l.Back.MiddleTexture, backmidname, labelTextureBackMid,
					Math.Max(backMidUDMFOffset.Right, backMidUDMFScale.Right) + 4, backmidtex,
					backMidUDMFOffsetLabel.Left, hasMiddleFields, l.Back.MiddleRequired());

				UpdateTexturePanel(panelBackLow, l.Back.LowTexture, backlowname, labelTextureBackBottom,
					Math.Max(backBottomUDMFOffset.Right, backBottomUDMFScale.Right) + 4, backlowtex,
					backBottomUDMFOffsetLabel.Left, hasBottomFields, l.Back.LowRequired());

				//mxd. Resize panel
				flowLayoutPanelBack.Width = panelBackLow.Right;
				backpanel.Width = flowLayoutPanelBack.Width + flowLayoutPanelBack.Left * 2 - 4;
			}
			else
			{
				// Show no info
				if(General.Map.UDMF) //mxd
				{ 
					backoffsetlabel.Text = "Back light:";
					backoffset.Text = "--";
				} 
				else 
				{
					backoffsetlabel.Text = "Back offset:";
					backoffset.Text = "--, --";
				}

				backoffsetlabel.Enabled = false;
				backoffset.Enabled = false;

				backhightex.BackgroundImage = null;
				backmidtex.BackgroundImage = null;
				backlowtex.BackgroundImage = null;

				backpanel.Visible = false; //mxd
			}

			//mxd. Flags and activations
			flags.Items.Clear();
			
			// Add activations
			foreach(LinedefActivateInfo ai in General.Map.Config.LinedefActivates)
			{
				if(l.Flags.ContainsKey(ai.Key) && l.Flags[ai.Key])
					flags.Items.Add(new ListViewItem(ai.Title) { Checked = true, ForeColor = SystemColors.HotTrack });
			}

			// And flags
			foreach(KeyValuePair<string, string> group in General.Map.Config.LinedefFlags) 
			{
				if(l.Flags.ContainsKey(group.Key) && l.Flags[group.Key])
					flags.Items.Add(new ListViewItem(group.Value) { Checked = true });
			}

			// And front flags
			if(l.Front != null)
			{
				foreach(KeyValuePair<string, string> group in General.Map.Config.SidedefFlags) 
				{
					if(l.Front.Flags.ContainsKey(group.Key) && l.Front.Flags[group.Key])
						flags.Items.Add(new ListViewItem("Front: " + group.Value) { Checked = true });
				}
			}

			// And back flags
			if(l.Back != null) 
			{
				foreach(KeyValuePair<string, string> group in General.Map.Config.SidedefFlags) 
				{
					if(l.Back.Flags.ContainsKey(group.Key) && l.Back.Flags[group.Key])
						flags.Items.Add(new ListViewItem("Back: " + group.Value) { Checked = true });
				}
			}

			//mxd. Flags panel visibility and size
			flagsPanel.Visible = (flags.Items.Count > 0);
			if(flags.Items.Count > 0) 
			{
				flags.Width = flags.GetItemRect(0).Width * (int)Math.Ceiling(flags.Items.Count / 5.0f);
				flagsPanel.Width = flags.Width + flags.Left * 2;
			}

			// Show the whole thing
			this.Show();
			this.Update();
		}

		private static void UpdateTexturePanel(Panel panel, string texturename, Label texturenamelabel, Label sizelabel, int maxlabelright, Panel image, int sizeref, bool extendedinfoshown, bool required)
		{
			// Set texture name
			texturenamelabel.Text = texturename;

			// And image
			DisplayTextureImage(image, sizelabel, texturename, required);
			
			//Reposition texture name label?
			if(texturenamelabel.Width < image.Width + 2)
				texturenamelabel.Location = new Point(image.Location.X + (image.Width - texturenamelabel.Width) / 2, texturenamelabel.Location.Y);
			else
				texturenamelabel.Location = new Point(image.Location.X, texturenamelabel.Location.Y);

			// Resize panel
			if(!extendedinfoshown)
				panel.Width = Math.Max(texturenamelabel.Right + image.Location.X + 1, sizeref);
			else
				panel.Width = Math.Max(texturenamelabel.Right, maxlabelright) + image.Location.X;
		}

		//mxd
		private static bool SetPairedUDMFFieldsLabel(UniFields fields, string paramX, string paramY, float defaultvalue, Label namelabel, Label valuelabel, bool highlight)
		{
			float x = UDMFTools.GetFloat(fields, paramX, defaultvalue);
			float y = UDMFTools.GetFloat(fields, paramY, defaultvalue);

			if(fields.ContainsKey(paramX)) x = (float)fields[paramX].Value;
			if(fields.ContainsKey(paramY)) y = (float)fields[paramY].Value;

			if(x != defaultvalue || y != defaultvalue)
			{
				valuelabel.Text = x.ToString(CultureInfo.InvariantCulture) + ", " + y.ToString(CultureInfo.InvariantCulture);
				valuelabel.Enabled = true;
				namelabel.Enabled = true;
				return true;
			}
			else
			{
				valuelabel.Text = "--, --";
				valuelabel.Enabled = highlight;
				namelabel.Enabled = highlight;
				return false;
			}
		}

		//mxd
		private static void SetUDMFLight(Sidedef sd, Label label, Label value, bool highlight) 
		{
			if(sd.Fields.ContainsKey("light")) 
			{
				int light = (int)sd.Fields["light"].Value;
				
				if (sd.Fields.GetValue("lightabsolute", false))
					value.Text = light + " (abs.)";
				else
					value.Text = light + " (" + Math.Min(255, Math.Max(0, (light + sd.Sector.Brightness))) + ")";

				value.Enabled = true;
				label.Enabled = true;
			} 
			else 
			{
				value.Text = "-- (" + sd.Sector.Brightness + ")";
				label.Enabled = highlight;
				value.Enabled = highlight;
			}

			label.ForeColor = (highlight ? SystemColors.HotTrack : SystemColors.WindowText);
			value.ForeColor = label.ForeColor;
		}

		//mxd
		private static void SetArgumentText(ArgumentInfo info, Label label, int value) 
		{
			TypeHandler th = General.Types.GetArgumentHandler(info);
			th.SetValue(value);
			label.Text = th.GetStringValue();
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
		private static void DisplayTextureImage(Panel panel, Label sizelabel, string name, bool required)
		{
			// Check if name is a "none" texture
			if((name.Length < 1) || (name == "-"))
			{
				sizelabel.Visible = false; //mxd
				
				// Determine image to show
				if(required) 
					General.DisplayZoomedImage(panel, Properties.Resources.MissingTexture);
				else
					panel.BackgroundImage = null;
			}
			else
			{
				//mxd
				ImageData texture = General.Map.Data.GetTextureImage(name);
				if(General.Settings.ShowTextureSizes && texture.ImageState == ImageLoadState.Ready && !(texture is UnknownImage)) 
				{
					sizelabel.Visible = true;
					sizelabel.Text = Math.Abs(texture.ScaledWidth) + "x" + Math.Abs(texture.ScaledHeight);
				} 
				else 
				{
					sizelabel.Visible = false;
				}
				
				// Set the image
				General.DisplayZoomedImage(panel, texture.GetPreview());
			}
		}
	}
}
