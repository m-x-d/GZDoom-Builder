
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
	public partial class LinedefEditForm : DelayedForm
	{
		// Variables
		private ICollection<Linedef> lines;
		
		// Constructor
		public LinedefEditForm()
		{
			// Initialize
			InitializeComponent();
			
			// Fill linedef flags list
			foreach(KeyValuePair<int, string> lf in General.Map.Config.LinedefFlags) flags.Add(lf.Value, lf.Key);

			// Fill linedef actions list
			action.AddInfo(General.Map.Config.SortedLinedefActions.ToArray());

			// Initialize image selectors
			fronthigh.Initialize();
			frontmid.Initialize();
			frontlow.Initialize();
			backhigh.Initialize();
			backmid.Initialize();
			backlow.Initialize();
		}

		// This sets up the form to edit the given lines
		public void Setup(ICollection<Linedef> lines)
		{
			Linedef fl;
			
			// Keep this list
			this.lines = lines;

			////////////////////////////////////////////////////////////////////////
			// Set all options to the first linedef properties
			////////////////////////////////////////////////////////////////////////

			// Get first line
			fl = General.GetByIndex<Linedef>(lines, 0);
			
			// Flags
			foreach(CheckBox c in flags.Checkboxes)
				c.Checked = (fl.Flags & (int)c.Tag) != 0;
			
			// Action/activation/tags
			action.Value = fl.Action;
			tag.Text = fl.Tag.ToString();

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

				// Action/activation/tags
				if(l.Action != action.Value) action.Empty = true;
				if(l.Tag.ToString() != tag.Text) tag.Text = "";
				
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
				// Apply all flags
				foreach(CheckBox c in flags.Checkboxes)
				{
					if(c.CheckState == CheckState.Checked) l.Flags |= (int)c.Tag;
					else if(c.CheckState == CheckState.Unchecked) l.Flags &= ~(int)c.Tag;
				}
				
				// Action/activation/tags
				if(!action.Empty) l.Action = action.Value;
				l.Tag = tag.GetResult(l.Tag);

				// Remove front side?
				if((l.Front != null) && (frontside.CheckState == CheckState.Unchecked))
				{
					l.Front.Dispose();
				}
				// Create or modify front side?
				if(frontside.CheckState == CheckState.Checked)
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
				if(backside.CheckState == CheckState.Checked)
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
	}
}
