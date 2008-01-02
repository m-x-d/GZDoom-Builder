
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
			foreach(KeyValuePair<int, string> lf in General.Map.Config.LinedefFlags) flags.Add(lf.Value, lf.Key);

			// Fill actions list
			action.AddInfo(General.Map.Config.SortedLinedefActions.ToArray());

			// Fill activations list
			activation.Items.AddRange(General.Map.Config.LinedefActivates.ToArray());
			
			// Initialize image selectors
			fronthigh.Initialize();
			frontmid.Initialize();
			frontlow.Initialize();
			backhigh.Initialize();
			backmid.Initialize();
			backlow.Initialize();

			// Show appropriate panel
			doompanel.Visible = General.Map.IsType(typeof(DoomMapSetIO));
			hexenpanel.Visible = General.Map.IsType(typeof(HexenMapSetIO));
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
				c.Checked = (fl.Flags & (int)c.Tag) != 0;
			
			// Activations
			foreach(LinedefActivateInfo ai in activation.Items)
				if((fl.Flags & ai.Index) == ai.Index) activation.SelectedItem = ai;
			
			// Action/tags
			action.Value = fl.Action;
			tag.Text = fl.Tag.ToString();
			arg0.Text = fl.Args[0].ToString();
			arg1.Text = fl.Args[1].ToString();
			arg2.Text = fl.Args[2].ToString();
			arg3.Text = fl.Args[3].ToString();
			arg4.Text = fl.Args[4].ToString();
			
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

			////////////////////////////////////////////////////////////////////////
			// Now go for all lines and change the options when a setting is different
			////////////////////////////////////////////////////////////////////////

			// Go for all lines
			foreach(Linedef l in lines)
			{
				// Flags
				foreach(CheckBox c in flags.Checkboxes)
				{
					if(((l.Flags & (int)c.Tag) != 0) != c.Checked)
					{
						c.ThreeState = true;
						c.CheckState = CheckState.Indeterminate;
					}
				}

				// Activations
				if(activation.Items.Count > 0)
				{
					sai = (activation.Items[0] as LinedefActivateInfo);
					foreach(LinedefActivateInfo ai in activation.Items)
						if((l.Flags & ai.Index) == ai.Index) sai = ai;
					if(sai != activation.SelectedItem) activation.SelectedIndex = -1;
				}
				
				// Action/tags
				if(l.Action != action.Value) action.Empty = true;
				if(l.Tag.ToString() != tag.Text) tag.Text = "";
				if(l.Args[0].ToString() != arg0.Text) arg0.Text = "";
				if(l.Args[1].ToString() != arg1.Text) arg1.Text = "";
				if(l.Args[2].ToString() != arg2.Text) arg2.Text = "";
				if(l.Args[3].ToString() != arg3.Text) arg3.Text = "";
				if(l.Args[4].ToString() != arg4.Text) arg4.Text = "";
				
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
				}
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
			General.Map.UndoRedo.CreateUndo("edit " + undodesc, UndoGroup.None, 0, false);

			// Go for all the lines
			foreach(Linedef l in lines)
			{
				// Remove activation flags
				if(activation.SelectedIndex > -1)
					foreach(LinedefActivateInfo ai in activation.Items) l.Flags &= ~ai.Index;

				// Apply all flags
				foreach(CheckBox c in flags.Checkboxes)
				{
					if(c.CheckState == CheckState.Checked) l.Flags |= (int)c.Tag;
					else if(c.CheckState == CheckState.Unchecked) l.Flags &= ~(int)c.Tag;
				}
				
				// Apply chosen activation flag
				if(activation.SelectedIndex > -1)
					l.Flags |= (activation.SelectedItem as LinedefActivateInfo).Index;
				
				// Action/tags
				if(!action.Empty) l.Action = action.Value;
				l.Tag = tag.GetResult(l.Tag);
				l.Args[0] = (byte)arg0.GetResult(l.Args[0]);
				l.Args[1] = (byte)arg1.GetResult(l.Args[1]);
				l.Args[2] = (byte)arg2.GetResult(l.Args[2]);
				l.Args[3] = (byte)arg3.GetResult(l.Args[3]);
				l.Args[4] = (byte)arg4.GetResult(l.Args[4]);

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
			}

			// Done
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
			arg0label.Text = General.Map.Config.LinedefActions[showaction].ArgTitle[0] + ":";
			arg1label.Text = General.Map.Config.LinedefActions[showaction].ArgTitle[1] + ":";
			arg2label.Text = General.Map.Config.LinedefActions[showaction].ArgTitle[2] + ":";
			arg3label.Text = General.Map.Config.LinedefActions[showaction].ArgTitle[3] + ":";
			arg4label.Text = General.Map.Config.LinedefActions[showaction].ArgTitle[4] + ":";
			arg0label.Enabled = General.Map.Config.LinedefActions[showaction].ArgUsed[0];
			arg1label.Enabled = General.Map.Config.LinedefActions[showaction].ArgUsed[1];
			arg2label.Enabled = General.Map.Config.LinedefActions[showaction].ArgUsed[2];
			arg3label.Enabled = General.Map.Config.LinedefActions[showaction].ArgUsed[3];
			arg4label.Enabled = General.Map.Config.LinedefActions[showaction].ArgUsed[4];
			if(arg0label.Enabled) arg0.ForeColor = SystemColors.WindowText; else arg0.ForeColor = SystemColors.GrayText;
			if(arg1label.Enabled) arg1.ForeColor = SystemColors.WindowText; else arg1.ForeColor = SystemColors.GrayText;
			if(arg2label.Enabled) arg2.ForeColor = SystemColors.WindowText; else arg2.ForeColor = SystemColors.GrayText;
			if(arg3label.Enabled) arg3.ForeColor = SystemColors.WindowText; else arg3.ForeColor = SystemColors.GrayText;
			if(arg4label.Enabled) arg4.ForeColor = SystemColors.WindowText; else arg4.ForeColor = SystemColors.GrayText;
		}

		// Browse Action clicked
		private void browseaction_Click(object sender, EventArgs e)
		{
			action.Value = ActionBrowserForm.BrowseAction(this, action.Value);
		}
	}
}
