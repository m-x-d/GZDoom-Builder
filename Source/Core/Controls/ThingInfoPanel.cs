
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
using System.Windows.Forms;
using CodeImp.DoomBuilder.Data;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Config;
using CodeImp.DoomBuilder.Types;
using CodeImp.DoomBuilder.GZBuilder; //mxd
using System.Collections.Generic;

#endregion

namespace CodeImp.DoomBuilder.Controls
{
	internal partial class ThingInfoPanel : UserControl
	{
		private int hexenformatwidth;
		private int doomformatwidth;

		// Constructor
		public ThingInfoPanel()
		{
			// Initialize
			InitializeComponent();

			// Hide stuff when in Doom format
			hexenformatwidth = infopanel.Width;
			doomformatwidth = infopanel.Width - 190;
		}

		// This shows the info
		public void ShowInfo(Thing t)
		{
			ThingTypeInfo ti;
			LinedefActionInfo act = null;
			string actioninfo = "";
			string zinfo;
			float zvalue;

			// Show/hide stuff depending on format
			bool hasArgs = General.Map.FormatInterface.HasActionArgs;
			arglbl1.Visible = hasArgs;
			arglbl2.Visible = hasArgs;
			arglbl3.Visible = hasArgs;
			arglbl4.Visible = hasArgs;
			arglbl5.Visible = hasArgs;
			arg1.Visible = hasArgs;
			arg2.Visible = hasArgs;
			arg3.Visible = hasArgs;
			arg4.Visible = hasArgs;
			arg5.Visible = hasArgs;
			infopanel.Width = (hasArgs ? hexenformatwidth : doomformatwidth);

			//mxd
			bool hasAction = General.Map.FormatInterface.HasThingAction;
			action.Visible = hasAction;
			labelaction.Visible = hasAction;

			// Move panel
			spritepanel.Left = infopanel.Left + infopanel.Width + infopanel.Margin.Right + spritepanel.Margin.Left;
			flagsPanel.Left = spritepanel.Left + spritepanel.Width + spritepanel.Margin.Right + flagsPanel.Margin.Left; //mxd
			
			// Lookup thing info
			ti = General.Map.Data.GetThingInfo(t.Type);

			// Get thing action information
			if(General.Map.Config.LinedefActions.ContainsKey(t.Action))
			{
				act = General.Map.Config.LinedefActions[t.Action];
				actioninfo = act.ToString();
			}
			else if(t.Action == 0)
				actioninfo = t.Action.ToString() + " - None";
			else
				actioninfo = t.Action.ToString() + " - Unknown";
			
			// Determine z info to show
			t.DetermineSector();
			if(ti.AbsoluteZ)
			{
				zvalue = t.Position.z;
				zinfo = zvalue.ToString();
			}
			else
			{
				if(t.Sector != null)
				{
					// Hangs from ceiling?
					if(ti.Hangs)
					{
						zvalue = t.Sector.CeilHeight - t.Position.z - ti.Height; //mxd
						zinfo = zvalue.ToString();
					}
					else
					{
						zvalue = t.Sector.FloorHeight + t.Position.z;
						zinfo = zvalue.ToString();
					}
				}
				else
				{
					zvalue = t.Position.z;
					if(zvalue >= 0.0f) zinfo = "+" + zvalue.ToString(); else zinfo = zvalue.ToString();
				}
			}

			// Thing info
			infopanel.Text = " Thing " + t.Index + " ";
			type.Text = t.Type + " - " + ti.Title;
			action.Text = actioninfo;
			position.Text = t.Position.x + ", " + t.Position.y + ", " + zinfo;
			tag.Text = t.Tag + (General.Map.Options.TagLabels.ContainsKey(t.Tag) ? " (" + General.Map.Options.TagLabels[t.Tag] + ")" : string.Empty);
			angle.Text = t.AngleDoom + "\u00B0";
			anglecontrol.Angle = t.AngleDoom;
			anglecontrol.Left = angle.Right + 1;
			
			// Sprite
			if(ti.Sprite.ToLowerInvariant().StartsWith(DataManager.INTERNAL_PREFIX) && (ti.Sprite.Length > DataManager.INTERNAL_PREFIX.Length))
			{
				spritename.Text = "";
				General.DisplayZoomedImage(spritetex, General.Map.Data.GetSpriteImage(ti.Sprite).GetBitmap());
			}
			else if((ti.Sprite.Length <= 8) && (ti.Sprite.Length > 0))
			{
				spritename.Text = ti.Sprite;
				General.DisplayZoomedImage(spritetex, General.Map.Data.GetSpriteImage(ti.Sprite).GetPreview());
			}
			else
			{
				spritename.Text = "";
				spritetex.BackgroundImage = null;
			}
			

			// Arguments
			ArgumentInfo[] arginfo = (((t.Action == 0 || act == null) && ti.Args[0] != null) ? ti.Args : act.Args); //mxd

			//mxd
			bool hasArg0Str = General.Map.UDMF && Array.IndexOf(GZGeneral.ACS_SPECIALS, t.Action) != -1 && t.Fields.ContainsKey("arg0str");

			arglbl1.Text = hasArg0Str ? "Script name:" : arginfo[0].Title + ":"; //mxd
			arglbl2.Text = arginfo[1].Title + ":";
			arglbl3.Text = arginfo[2].Title + ":";
			arglbl4.Text = arginfo[3].Title + ":";
			arglbl5.Text = arginfo[4].Title + ":";
			arglbl1.Enabled = arginfo[0].Used;
			arglbl2.Enabled = arginfo[1].Used;
			arglbl3.Enabled = arginfo[2].Used;
			arglbl4.Enabled = arginfo[3].Used;
			arglbl5.Enabled = arginfo[4].Used;
			arg1.Enabled = arginfo[0].Used;
			arg2.Enabled = arginfo[1].Used;
			arg3.Enabled = arginfo[2].Used;
			arg4.Enabled = arginfo[3].Used;
			arg5.Enabled = arginfo[4].Used;

			//mxd
			if(hasArg0Str) {
				arg1.Text = '"' + t.Fields["arg0str"].Value.ToString() + '"';
			} else {
				setArgumentText(arginfo[0], arg1, t.Args[0]);
			}
			setArgumentText(arginfo[1], arg2, t.Args[1]);
			setArgumentText(arginfo[2], arg3, t.Args[2]);
			setArgumentText(arginfo[3], arg4, t.Args[3]);
			setArgumentText(arginfo[4], arg5, t.Args[4]);

			//mxd. Flags
			flags.Items.Clear();
			foreach(KeyValuePair<string, bool> group in t.Flags){
				if(group.Value) {
					ListViewItem item = new ListViewItem(General.Map.Config.ThingFlags.ContainsKey(group.Key) ? General.Map.Config.ThingFlags[group.Key] : group.Key);
					item.Checked = true;
					flags.Items.Add(item);
				}
			}

			//mxd. Flags panel visibility and size
			flagsPanel.Visible = (flags.Items.Count > 0);
			if(flags.Items.Count > 0) {
				int itemWidth = flags.Items[0].GetBounds(ItemBoundsPortion.Entire).Width;
				if(itemWidth == 0) itemWidth = 96;
				flags.Width = itemWidth * (int)Math.Ceiling(flags.Items.Count / 5.0f);
				flagsPanel.Width = flags.Width + flags.Left * 2;
			}

			// Show the whole thing
			this.Show();
			this.Update();
		}

		//mxd
		private void setArgumentText(ArgumentInfo info, Label label, int value) {
			TypeHandler th = General.Types.GetArgumentHandler(info);
			th.SetValue(value);
			label.Text = th.GetStringValue();

			if(value < 1 || !General.Map.Options.TagLabels.ContainsKey(value)) return;

			if (th is ThingTagHandler || th is LinedefTagHandler || th is SectorTagHandler) {
				label.Text += " (" + General.Map.Options.TagLabels[value] + ")";
			}
		}

		// When visible changed
		protected override void OnVisibleChanged(EventArgs e)
		{
			// Hiding panels
			if(!this.Visible)
			{
				spritetex.BackgroundImage = null;
			}

			// Call base
			base.OnVisibleChanged(e);
		}
	}
}
