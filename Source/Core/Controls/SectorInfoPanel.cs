
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
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Data;
using CodeImp.DoomBuilder.Rendering;
using System.Collections.Generic;

#endregion

namespace CodeImp.DoomBuilder.Controls
{
	internal partial class SectorInfoPanel : UserControl
	{
		private readonly List<Label> floorinfolabels;
		private readonly List<Label> ceilinfolabels;
		private readonly List<Label> floorlabels;
		private readonly List<Label> ceillabels;
		
		// Constructor
		public SectorInfoPanel()
		{
			// Initialize
			InitializeComponent();

			//mxd
			labelFloorTextureSize.BackColor = Color.FromArgb(128, labelFloorTextureSize.BackColor);
			labelCeilTextureSize.BackColor = Color.FromArgb(128, labelCeilTextureSize.BackColor);

			floorinfolabels = new List<Label> { floorAngle, floorLight, floorOffset, floorScale };
			ceilinfolabels = new List<Label> { ceilingAngle, ceilingLight, ceilingOffset, ceilingScale };
			floorlabels = new List<Label> { floorAngleLabel, floorLightLabel, floorOffsetLabel, floorScaleLabel };
			ceillabels = new List<Label> { ceilingAngleLabel, ceilingLightLabel, ceilingOffsetLabel, ceilingScaleLabel };
		}

		// This shows the info
		public void ShowInfo(Sector s)
		{
			int sheight = s.CeilHeight - s.FloorHeight;

			// Lookup effect description in config
			string effectinfo = s.Effect + " - " + General.Map.Config.GetSectorEffectInfo(s.Effect).Title; //mxd

			// Sector info
			sectorinfo.Text = " Sector " + s.Index + " (" + (s.Sidedefs == null ? "no" : s.Sidedefs.Count.ToString()) + " sidedefs)"; //mxd
			effect.Text = effectinfo;
			ceiling.Text = s.CeilHeight.ToString();
			floor.Text = s.FloorHeight.ToString();
			tag.Text = s.Tag + (General.Map.Options.TagLabels.ContainsKey(s.Tag) ? " - " + General.Map.Options.TagLabels[s.Tag] : string.Empty);
			height.Text = sheight.ToString();
			brightness.Text = s.Brightness.ToString();
			floorname.Text = s.FloorTexture;
			ceilingname.Text = s.CeilTexture;

			//mxd
			effect.Enabled = (s.Effect != 0);
			labelEffect.Enabled = (s.Effect != 0);
			tag.Enabled = (s.Tag != 0);
			labelTag.Enabled = (s.Tag != 0);

			//mxd. Texture size
			if (s.LongFloorTexture == MapSet.EmptyLongName)
			{
				labelFloorTextureSize.Visible = false;
				General.DisplayZoomedImage(floortex, Properties.Resources.MissingTexture);
			} 
			else 
			{
				ImageData image = General.Map.Data.GetFlatImage(s.FloorTexture);
				DisplayTextureSize(labelFloorTextureSize, image);
				General.DisplayZoomedImage(floortex, image.GetPreview());
			}

			if (s.LongCeilTexture == MapSet.EmptyLongName) 
			{
				labelCeilTextureSize.Visible = false;
				General.DisplayZoomedImage(ceilingtex, Properties.Resources.MissingTexture);
			} 
			else 
			{
				ImageData image = General.Map.Data.GetFlatImage(s.CeilTexture);
				DisplayTextureSize(labelCeilTextureSize, image); //mxd
				General.DisplayZoomedImage(ceilingtex, image.GetPreview());
			}

			//mxd
			bool showExtededFloorInfo = false;
			bool showExtededCeilingInfo = false;
			if(General.Map.UDMF) 
			{
				if(s.Fields != null) 
				{
					//sector colors
					labelLight.Visible = true;
					labelFade.Visible = true;
					panelLightColor.Visible = true;
					panelFadeColor.Visible = true;

					if(s.Fields.ContainsKey("lightcolor")) 
					{
						panelLightColor.BackColor = PixelColor.FromInt(s.Fields.GetValue("lightcolor", 0xFFFFFF)).WithAlpha(255).ToColor();
						labelLight.Enabled = true;
					} 
					else 
					{
						panelLightColor.BackColor = SystemColors.Control;
						labelLight.Enabled = false;
					}

					if(s.Fields.ContainsKey("fadecolor")) 
					{
						panelFadeColor.BackColor = PixelColor.FromInt(s.Fields.GetValue("fadecolor", 0)).WithAlpha(255).ToColor();
						labelFade.Enabled = true;
					} 
					else 
					{
						panelFadeColor.BackColor = SystemColors.Control;
						labelFade.Enabled = false;
					}

					//light
					if(s.Fields.ContainsKey("lightceiling") || s.Fields.ContainsKey("lightceilingabsolute")) 
					{
						showExtededCeilingInfo = true;
						ceilingLight.Enabled = true;
						ceilingLightLabel.Enabled = true;

						int cl = s.Fields.GetValue("lightceiling", 0);

						if(s.Fields.GetValue("lightceilingabsolute", false))
							ceilingLight.Text = cl + " (abs.)";
						else
							ceilingLight.Text = cl + " (" + Math.Min(255, Math.Max(0, (cl + s.Brightness))) + ")";

					} 
					else 
					{
						ceilingLight.Text = "--";
						ceilingLight.Enabled = false;
						ceilingLightLabel.Enabled = false;
					}

					if(s.Fields.ContainsKey("lightfloor") || s.Fields.ContainsKey("lightfloorabsolute")) 
					{
						showExtededFloorInfo = true;
						floorLight.Enabled = true;
						floorLightLabel.Enabled = true;

						int fl = s.Fields.GetValue("lightfloor", 0);

						if(s.Fields.GetValue("lightfloorabsolute", false))
							floorLight.Text = fl + " (abs.)";
						else
							floorLight.Text = fl + " (" + Math.Min(255, Math.Max(0, (fl + s.Brightness))) + ")";

					} 
					else 
					{
						floorLight.Text = "--";
						floorLight.Enabled = false;
						floorLightLabel.Enabled = false;
					}

					//ceiling offsets
					float panX = s.Fields.GetValue("xpanningceiling", 0f);
					float panY = s.Fields.GetValue("ypanningceiling", 0f);

					if(panX != 0 || panY != 0) 
					{
						showExtededCeilingInfo = true;
						ceilingOffset.Enabled = true;
						ceilingOffsetLabel.Enabled = true;
						ceilingOffset.Text = panX.ToString(CultureInfo.InvariantCulture) + ", " + panY.ToString(CultureInfo.InvariantCulture);
					} 
					else 
					{
						ceilingOffset.Text = "--, --";
						ceilingOffset.Enabled = false;
						ceilingOffsetLabel.Enabled = false;
					}

					//floor offsets
					panX = s.Fields.GetValue("xpanningfloor", 0f);
					panY = s.Fields.GetValue("ypanningfloor", 0f);

					if(panX != 0 || panY != 0) 
					{
						showExtededFloorInfo = true;
						floorOffset.Enabled = true;
						floorOffsetLabel.Enabled = true;
						floorOffset.Text = panX.ToString(CultureInfo.InvariantCulture) + ", " + panY.ToString(CultureInfo.InvariantCulture);
					} 
					else 
					{
						floorOffset.Text = "--, --";
						floorOffset.Enabled = false;
						floorOffsetLabel.Enabled = false;
					}

					//ceiling scale
					float scaleX = s.Fields.GetValue("xscaleceiling", 1.0f);
					float scaleY = s.Fields.GetValue("yscaleceiling", 1.0f);

					if(scaleX != 1.0f || scaleY != 1.0f) 
					{
						showExtededCeilingInfo = true;
						ceilingScale.Enabled = true;
						ceilingScaleLabel.Enabled = true;
						ceilingScale.Text = scaleX.ToString(CultureInfo.InvariantCulture) + ", " + scaleY.ToString(CultureInfo.InvariantCulture);
					} 
					else 
					{
						ceilingScale.Text = "--, --";
						ceilingScale.Enabled = false;
						ceilingScaleLabel.Enabled = false;
					}

					//floor scale
					scaleX = s.Fields.GetValue("xscalefloor", 1.0f);
					scaleY = s.Fields.GetValue("yscalefloor", 1.0f);

					if(scaleX != 1.0f || scaleY != 1.0f) 
					{
						showExtededFloorInfo = true;
						floorScale.Enabled = true;
						floorScaleLabel.Enabled = true;
						floorScale.Text = scaleX.ToString(CultureInfo.InvariantCulture) + ", " + scaleY.ToString(CultureInfo.InvariantCulture);
					} 
					else 
					{
						floorScale.Text = "--, --";
						floorScale.Enabled = false;
						floorScaleLabel.Enabled = false;
					}

					//rotation
					if(s.Fields.ContainsKey("rotationceiling")) 
					{
						showExtededCeilingInfo = true;
						ceilingAngle.Enabled = true;
						ceilingAngleLabel.Enabled = true;
						ceilingAngle.Text = s.Fields["rotationceiling"].Value + "\u00B0";
					} 
					else 
					{
						ceilingAngle.Text = "--";
						ceilingAngle.Enabled = false;
						ceilingAngleLabel.Enabled = false;
					}

					if(s.Fields.ContainsKey("rotationfloor")) 
					{
						showExtededFloorInfo = true;
						floorAngle.Enabled = true;
						floorAngleLabel.Enabled = true;
						floorAngle.Text = s.Fields["rotationfloor"].Value + "\u00B0";
					} 
					else 
					{
						floorAngle.Text = "--";
						floorAngle.Enabled = false;
						floorAngleLabel.Enabled = false;
					}
				}

				//Flags
				flags.Items.Clear();
				foreach(KeyValuePair<string, bool> group in s.Flags) 
				{
					if(group.Value) 
					{
						ListViewItem item = new ListViewItem(General.Map.Config.SectorFlags.ContainsKey(group.Key) ? General.Map.Config.SectorFlags[group.Key] : group.Key);
						item.Checked = true;
						flags.Items.Add(item);
					}
				}

				//mxd. Flags panel visibility and size
				flagsPanel.Visible = (flags.Items.Count > 0);
				if(flags.Items.Count > 0) 
				{
					flags.Width = flags.GetItemRect(0).Width * (int)Math.Ceiling(flags.Items.Count / 5.0f);
					flagsPanel.Width = flags.Width + flags.Left * 2;
				}

				//mxd. Toggle visibility
				foreach(Label label in floorinfolabels) label.Visible = showExtededFloorInfo;
				foreach(Label label in floorlabels) label.Visible = showExtededFloorInfo;
				foreach(Label label in ceilinfolabels) label.Visible = showExtededCeilingInfo;
				foreach(Label label in ceillabels) label.Visible = showExtededCeilingInfo;
			} 
			else 
			{
				panelFadeColor.Visible = false;
				panelLightColor.Visible = false;
				labelFade.Visible = false;
				labelLight.Visible = false;
				flagsPanel.Visible = false;
			}

			//mxd. Resize panels
			UpdateTexturePanel(ceilingpanel, ceilingname, ceilinfolabels, ceilingtex, ceilingOffsetLabel.Location.X - 1, showExtededCeilingInfo);
			UpdateTexturePanel(floorpanel, floorname, floorinfolabels, floortex, floorOffsetLabel.Location.X - 1, showExtededFloorInfo);

			// Show the whole thing
			this.Show();
			this.Update();
		}

		//mxd
		private static void UpdateTexturePanel(GroupBox panel, Label texturename, List<Label> proplabels, Panel image, int sizeref, bool extendedinfoshown)
		{
			//Reposition texture name label?
			if(texturename.Width < image.Width + 2) 
				texturename.Location = new Point(image.Location.X + (image.Width - texturename.Width) / 2, texturename.Location.Y);
			else 
				texturename.Location = new Point(image.Location.X - 1, texturename.Location.Y);
			
			// Resize panel
			if(!extendedinfoshown)
				panel.Width = Math.Max(texturename.Right + image.Location.X - 1, sizeref);
			else
				panel.Width = Math.Max(texturename.Right, GetMaxRight(proplabels)) + image.Location.X;
		}

		//mxd
		private static int GetMaxRight(IEnumerable<Label> labels)
		{
			int max = 0;
			foreach (Label label in labels) if (label.Right > max) max = label.Right;
			return max;
		}

		//mxd
		private static void DisplayTextureSize(Label label, ImageData texture) 
		{
			if(General.Settings.ShowTextureSizes && texture.ImageState == ImageLoadState.Ready 
				&& !string.IsNullOrEmpty(texture.Name) && !(texture is UnknownImage)) 
			{
				label.Visible = true;
				label.Text = Math.Abs(texture.ScaledWidth) + "x" + Math.Abs(texture.ScaledHeight);
			} 
			else 
			{
				label.Visible = false;
			}
		}

		// When visible changed
		protected override void OnVisibleChanged(EventArgs e)
		{
			// Hiding panels
			if(!this.Visible)
			{
				floortex.BackgroundImage = null;
				ceilingtex.BackgroundImage = null;
			}

			// Call base
			base.OnVisibleChanged(e);
		}
	}
}
