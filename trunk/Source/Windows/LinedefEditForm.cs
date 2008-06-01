
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
	internal partial class LinedefEditForm : DelayedForm
	{
		// Variables
		private ICollection<Linedef> lines;
		
		// Constructor
		public LinedefEditForm()
		{
			// Initialize
			InitializeComponent();
			
			// Fill flags list
			foreach(KeyValuePair<string, string> lf in General.Map.Config.LinedefFlags)
				flags.Add(lf.Value, lf.Key);

			// Fill actions list
			action.GeneralizedCategories = General.Map.Config.GenActionCategories;
			action.AddInfo(General.Map.Config.SortedLinedefActions.ToArray());

			// Fill activations list
			activation.Items.AddRange(General.Map.Config.LinedefActivates.ToArray());
			foreach(LinedefActivateInfo ai in General.Map.Config.LinedefActivates) udmfactivates.Add(ai.Title, ai);
			
			// Fill universal fields list
			fieldslist.ListFixedFields(General.Map.Config.LinedefFields);
			
			// Initialize image selectors
			fronthigh.Initialize();
			frontmid.Initialize();
			frontlow.Initialize();
			backhigh.Initialize();
			backmid.Initialize();
			backlow.Initialize();

			// Initialize custom fields editor
			fieldslist.Setup();
			
			// THE CODE BELOW IS ABSOLUTELY UGLY
			// I should make different controls for each format
			// that handle linedef properties
			
			// UDMF map?
			if(General.Map.IsType(typeof(UniversalMapSetIO)))
			{
				udmfpanel.Visible = true;
				argspanel.Visible = true;
			}
			// Hexen map?
			else if(General.Map.IsType(typeof(HexenMapSetIO)))
			{
				hexenpanel.Visible = true;
				argspanel.Visible = true;
				actiongroup.Height = 210;
				this.Height = 510;
			}
			// Doom map?
			else
			{
				actiongroup.Height = 68;
				this.Height = 470;
			}

			// ID group?
			if(!General.Map.IsType(typeof(HexenMapSetIO)))
			{
				// Match position after the action group
				idgroup.Top = actiongroup.Bottom + actiongroup.Margin.Bottom + idgroup.Margin.Top;
			}
			else
			{
				idgroup.Visible = false;
			}
		}

		// This sets up the form to edit the given lines
		public void Setup(ICollection<Linedef> lines)
		{
			LinedefActivateInfo sai;
			Linedef fl;
			
			// Keep this list
			this.lines = lines;
			if(lines.Count > 1) this.Text = "Edit Linedefs (" + lines.Count + ")";
			
			////////////////////////////////////////////////////////////////////////
			// Set all options to the first linedef properties
			////////////////////////////////////////////////////////////////////////

			// Get first line
			fl = General.GetByIndex<Linedef>(lines, 0);
			
			// Flags
			foreach(CheckBox c in flags.Checkboxes)
				if(fl.Flags.ContainsKey(c.Tag.ToString())) c.Checked = fl.Flags[c.Tag.ToString()];
			
			// Activations
			foreach(LinedefActivateInfo ai in activation.Items)
				if((fl.Activate & ai.Index) == ai.Index) activation.SelectedItem = ai;

			// UDMF Activations
			foreach(CheckBox c in udmfactivates.Checkboxes)
			{
				LinedefActivateInfo ai = (c.Tag as LinedefActivateInfo);
				if(fl.Flags.ContainsKey(ai.Key)) c.Checked = fl.Flags[ai.Key];
			}

			// Action/tags
			action.Value = fl.Action;
			tag.Text = fl.Tag.ToString();
			arg0.SetValue(fl.Args[0]);
			arg1.SetValue(fl.Args[1]);
			arg2.SetValue(fl.Args[2]);
			arg3.SetValue(fl.Args[3]);
			arg4.SetValue(fl.Args[4]);
			
			// Front side and back side checkboxes
			frontside.Checked = (fl.Front != null);
			backside.Checked = (fl.Back != null);

			// Front settings
			if(fl.Front != null)
			{
				fronthigh.TextureName = fl.Front.HighTexture;
				frontmid.TextureName = fl.Front.MiddleTexture;
				frontlow.TextureName = fl.Front.LowTexture;
				frontsector.Text = fl.Front.Sector.Index.ToString();
				frontoffsetx.Text = fl.Front.OffsetX.ToString();
				frontoffsety.Text = fl.Front.OffsetY.ToString();
			}

			// Back settings
			if(fl.Back != null)
			{
				backhigh.TextureName = fl.Back.HighTexture;
				backmid.TextureName = fl.Back.MiddleTexture;
				backlow.TextureName = fl.Back.LowTexture;
				backsector.Text = fl.Back.Sector.Index.ToString();
				backoffsetx.Text = fl.Back.OffsetX.ToString();
				backoffsety.Text = fl.Back.OffsetY.ToString();
			}

			// Custom fields
			fieldslist.SetValues(fl.Fields, true);

			////////////////////////////////////////////////////////////////////////
			// Now go for all lines and change the options when a setting is different
			////////////////////////////////////////////////////////////////////////

			// Go for all lines
			foreach(Linedef l in lines)
			{
				// Flags
				foreach(CheckBox c in flags.Checkboxes)
				{
					if(l.Flags.ContainsKey(c.Tag.ToString()))
					{
						if(l.Flags[c.Tag.ToString()] != c.Checked)
						{
							c.ThreeState = true;
							c.CheckState = CheckState.Indeterminate;
						}
					}
				}

				// Activations
				if(activation.Items.Count > 0)
				{
					sai = (activation.Items[0] as LinedefActivateInfo);
					foreach(LinedefActivateInfo ai in activation.Items)
						if((l.Activate & ai.Index) == ai.Index) sai = ai;
					if(sai != activation.SelectedItem) activation.SelectedIndex = -1;
				}

				// UDMF Activations
				foreach(CheckBox c in udmfactivates.Checkboxes)
				{
					LinedefActivateInfo ai = (c.Tag as LinedefActivateInfo);
					if(l.Flags.ContainsKey(ai.Key))
					{
						if(c.Checked != l.Flags[ai.Key])
						{
							c.ThreeState = true;
							c.CheckState = CheckState.Indeterminate;
						}
					}
				}

				// Action/tags
				if(l.Action != action.Value) action.Empty = true;
				if(l.Tag.ToString() != tag.Text) tag.Text = "";
				if(l.Args[0] != arg0.GetResult(-1)) arg0.ClearValue();
				if(l.Args[1] != arg1.GetResult(-1)) arg1.ClearValue();
				if(l.Args[2] != arg2.GetResult(-1)) arg2.ClearValue();
				if(l.Args[3] != arg3.GetResult(-1)) arg3.ClearValue();
				if(l.Args[4] != arg4.GetResult(-1)) arg4.ClearValue();
				
				// Front side checkbox
				if((l.Front != null) != frontside.Checked)
				{
					frontside.ThreeState = true;
					frontside.CheckState = CheckState.Indeterminate;
					frontside.AutoCheck = false;
				}

				// Back side checkbox
				if((l.Back != null) != backside.Checked)
				{
					backside.ThreeState = true;
					backside.CheckState = CheckState.Indeterminate;
					backside.AutoCheck = false;
				}

				// Front settings
				if(l.Front != null)
				{
					if(fronthigh.TextureName != l.Front.HighTexture) fronthigh.TextureName = "";
					if(frontmid.TextureName != l.Front.MiddleTexture) frontmid.TextureName = "";
					if(frontlow.TextureName != l.Front.LowTexture) frontlow.TextureName = "";
					if(frontsector.Text != l.Front.Sector.Index.ToString()) frontsector.Text = "";
					if(frontoffsetx.Text != l.Front.OffsetX.ToString()) frontoffsetx.Text = "";
					if(frontoffsety.Text != l.Front.OffsetY.ToString()) frontoffsety.Text = "";
					if(General.Map.IsType(typeof(UniversalMapSetIO))) customfrontbutton.Visible = true;
				}

				// Back settings
				if(l.Back != null)
				{
					if(backhigh.TextureName != l.Back.HighTexture) backhigh.TextureName = "";
					if(backmid.TextureName != l.Back.MiddleTexture) backmid.TextureName = "";
					if(backlow.TextureName != l.Back.LowTexture) backlow.TextureName = "";
					if(backsector.Text != l.Back.Sector.Index.ToString()) backsector.Text = "";
					if(backoffsetx.Text != l.Back.OffsetX.ToString()) backoffsetx.Text = "";
					if(backoffsety.Text != l.Back.OffsetY.ToString()) backoffsety.Text = "";
					if(General.Map.IsType(typeof(UniversalMapSetIO))) custombackbutton.Visible = true;
				}
				
				// Custom fields
				fieldslist.SetValues(l.Fields, false);
			}
		}
		
		// Front side (un)checked
		private void frontside_CheckStateChanged(object sender, EventArgs e)
		{
			// Enable/disable panel
			// NOTE: Also enabled when checkbox is grayed!
			frontgroup.Enabled = (frontside.CheckState != CheckState.Unchecked);
		}

		// Back side (un)checked
		private void backside_CheckStateChanged(object sender, EventArgs e)
		{
			// Enable/disable panel
			// NOTE: Also enabled when checkbox is grayed!
			backgroup.Enabled = (backside.CheckState != CheckState.Unchecked);
		}

		// This selects all text in a textbox
		private void SelectAllText(object sender, EventArgs e)
		{
			(sender as TextBox).SelectAll();
		}

		// Apply clicked
		private void apply_Click(object sender, EventArgs e)
		{
			string undodesc = "linedef";
			Sector s;
			int index;
			
			// Make undo
			if(lines.Count > 1) undodesc = lines.Count + " linedefs";
			General.Map.UndoRedo.CreateUndo("Edit " + undodesc, UndoGroup.None, 0);

			// Go for all the lines
			foreach(Linedef l in lines)
			{
				// Apply all flags
				foreach(CheckBox c in flags.Checkboxes)
				{
					if(c.CheckState == CheckState.Checked) l.Flags[c.Tag.ToString()] = true;
					else if(c.CheckState == CheckState.Unchecked) l.Flags[c.Tag.ToString()] = false;
				}
				
				// Apply chosen activation flag
				if(activation.SelectedIndex > -1)
					l.Activate = (activation.SelectedItem as LinedefActivateInfo).Index;
				
				// UDMF activations
				foreach(CheckBox c in udmfactivates.Checkboxes)
				{
					LinedefActivateInfo ai = (c.Tag as LinedefActivateInfo);
					if(c.CheckState == CheckState.Checked) l.Flags[ai.Key] = true;
					else if(c.CheckState == CheckState.Unchecked) l.Flags[ai.Key] = false;
				}
				
				// Action/tags
				if(!action.Empty) l.Action = action.Value;
				l.Tag = tag.GetResult(l.Tag);
				l.Args[0] = arg0.GetResult(l.Args[0]);
				l.Args[1] = arg1.GetResult(l.Args[1]);
				l.Args[2] = arg2.GetResult(l.Args[2]);
				l.Args[3] = arg3.GetResult(l.Args[3]);
				l.Args[4] = arg4.GetResult(l.Args[4]);

				// Remove front side?
				if((l.Front != null) && (frontside.CheckState == CheckState.Unchecked))
				{
					l.Front.Dispose();
				}
				// Create or modify front side?
				else if(frontside.CheckState == CheckState.Checked)
				{
					// Make sure we have a valid sector (make a new one if needed)
					if(l.Front != null) index = l.Front.Sector.Index; else index = -1;
					s = General.Map.Map.GetSectorByIndex(frontsector.GetResult(index));
					if(s == null) s = General.Map.Map.CreateSector();
					
					// Create new sidedef?
					if(l.Front == null) General.Map.Map.CreateSidedef(l, true, s);

					// Change sector?
					if(l.Front.Sector != s) l.Front.ChangeSector(s);

					// Apply settings
					l.Front.OffsetX = frontoffsetx.GetResult(l.Front.OffsetX);
					l.Front.OffsetY = frontoffsety.GetResult(l.Front.OffsetY);
					l.Front.SetTextureHigh(fronthigh.GetResult(l.Front.HighTexture));
					l.Front.SetTextureMid(frontmid.GetResult(l.Front.MiddleTexture));
					l.Front.SetTextureLow(frontlow.GetResult(l.Front.LowTexture));
				}

				// Remove back side?
				if((l.Back != null) && (backside.CheckState == CheckState.Unchecked))
				{
					l.Back.Dispose();
				}
				// Create or modify back side?
				else if(backside.CheckState == CheckState.Checked)
				{
					// Make sure we have a valid sector (make a new one if needed)
					if(l.Back != null) index = l.Back.Sector.Index; else index = -1;
					s = General.Map.Map.GetSectorByIndex(backsector.GetResult(index));
					if(s == null) s = General.Map.Map.CreateSector();

					// Create new sidedef?
					if(l.Back == null) General.Map.Map.CreateSidedef(l, false, s);

					// Change sector?
					if(l.Back.Sector != s) l.Back.ChangeSector(s);

					// Apply settings
					l.Back.OffsetX = backoffsetx.GetResult(l.Back.OffsetX);
					l.Back.OffsetY = backoffsety.GetResult(l.Back.OffsetY);
					l.Back.SetTextureHigh(backhigh.GetResult(l.Back.HighTexture));
					l.Back.SetTextureMid(backmid.GetResult(l.Back.MiddleTexture));
					l.Back.SetTextureLow(backlow.GetResult(l.Back.LowTexture));
				}

				// Custom fields
				fieldslist.Apply(l.Fields);
			}

			// Done
			General.Map.IsChanged = true;
			this.DialogResult = DialogResult.OK;
			this.Close();
		}

		// Cancel clicked
		private void cancel_Click(object sender, EventArgs e)
		{
			// Be gone
			this.DialogResult = DialogResult.Cancel;
			this.Close();
		}

		// This finds a new (unused) tag
		private void newtag_Click(object sender, EventArgs e)
		{
			tag.Text = General.Map.Map.GetNewTag().ToString();
		}

		// Action changes
		private void action_ValueChanges(object sender, EventArgs e)
		{
			int showaction = 0;
			
			// Only when line type is known
			if(General.Map.Config.LinedefActions.ContainsKey(action.Value)) showaction = action.Value;
			
			// Change the argument descriptions
			arg0label.Text = General.Map.Config.LinedefActions[showaction].Args[0].Title + ":";
			arg1label.Text = General.Map.Config.LinedefActions[showaction].Args[1].Title + ":";
			arg2label.Text = General.Map.Config.LinedefActions[showaction].Args[2].Title + ":";
			arg3label.Text = General.Map.Config.LinedefActions[showaction].Args[3].Title + ":";
			arg4label.Text = General.Map.Config.LinedefActions[showaction].Args[4].Title + ":";
			arg0label.Enabled = General.Map.Config.LinedefActions[showaction].Args[0].Used;
			arg1label.Enabled = General.Map.Config.LinedefActions[showaction].Args[1].Used;
			arg2label.Enabled = General.Map.Config.LinedefActions[showaction].Args[2].Used;
			arg3label.Enabled = General.Map.Config.LinedefActions[showaction].Args[3].Used;
			arg4label.Enabled = General.Map.Config.LinedefActions[showaction].Args[4].Used;
			if(arg0label.Enabled) arg0.ForeColor = SystemColors.WindowText; else arg0.ForeColor = SystemColors.GrayText;
			if(arg1label.Enabled) arg1.ForeColor = SystemColors.WindowText; else arg1.ForeColor = SystemColors.GrayText;
			if(arg2label.Enabled) arg2.ForeColor = SystemColors.WindowText; else arg2.ForeColor = SystemColors.GrayText;
			if(arg3label.Enabled) arg3.ForeColor = SystemColors.WindowText; else arg3.ForeColor = SystemColors.GrayText;
			if(arg4label.Enabled) arg4.ForeColor = SystemColors.WindowText; else arg4.ForeColor = SystemColors.GrayText;
			arg0.Setup(General.Map.Config.LinedefActions[showaction].Args[0]);
			arg1.Setup(General.Map.Config.LinedefActions[showaction].Args[1]);
			arg2.Setup(General.Map.Config.LinedefActions[showaction].Args[2]);
			arg3.Setup(General.Map.Config.LinedefActions[showaction].Args[3]);
			arg4.Setup(General.Map.Config.LinedefActions[showaction].Args[4]);
		}

		// Browse Action clicked
		private void browseaction_Click(object sender, EventArgs e)
		{
			action.Value = ActionBrowserForm.BrowseAction(this, action.Value);
		}

		// Custom fields on front sides
		private void customfrontbutton_Click(object sender, EventArgs e)
		{
			// Make collection of front sides
			List<MapElement> sides = new List<MapElement>(lines.Count);
			foreach(Linedef l in lines) if(l.Front != null) sides.Add(l.Front);
			
			// Edit these
			CustomFieldsForm.ShowDialog(this, "Front side custom fields", sides, General.Map.Config.SidedefFields);
		}

		// Custom fields on back sides
		private void custombackbutton_Click(object sender, EventArgs e)
		{
			// Make collection of back sides
			List<MapElement> sides = new List<MapElement>(lines.Count);
			foreach(Linedef l in lines) if(l.Back != null) sides.Add(l.Back);

			// Edit these
			CustomFieldsForm.ShowDialog(this, "Back side custom fields", sides, General.Map.Config.SidedefFields);
		}
	}
}
