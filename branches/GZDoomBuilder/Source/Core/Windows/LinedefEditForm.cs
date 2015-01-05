
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
using System.Windows.Forms;
using CodeImp.DoomBuilder.Geometry;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Config;
using CodeImp.DoomBuilder.GZBuilder.Data; //mxd
using CodeImp.DoomBuilder.Types;

#endregion

namespace CodeImp.DoomBuilder.Windows
{
	internal partial class LinedefEditForm : DelayedForm
	{
		#region ================== Events

		public event EventHandler OnValuesChanged; //mxd

		#endregion

		#region ================== Constants


		#endregion

		#region ================== Variables

		private ICollection<Linedef> lines;
		private List<LinedefProperties> linedefProps; //mxd
		private bool preventchanges;

		//mxd. Window setup stuff
		private static Point location = Point.Empty;

		private struct LinedefProperties //mxd
		{
			public readonly Dictionary<string, bool> Flags;
			public readonly SidedefProperties Front;
			public readonly SidedefProperties Back;

			public LinedefProperties(Linedef line) 
			{
				Front = (line.Front != null ? new SidedefProperties(line.Front) : null);
				Back = (line.Back != null ? new SidedefProperties(line.Back) : null);
				Flags = line.GetFlags();
			}
		}

		private class SidedefProperties //mxd
		{
			public readonly int OffsetX;
			public readonly int OffsetY;

			public readonly string TextureTop;
			public readonly string TextureMid;
			public readonly string TextureLow;

			public SidedefProperties(Sidedef side) 
			{
				//offset
				OffsetX = side.OffsetX;
				OffsetY = side.OffsetY;

				//textures
				TextureTop = side.HighTexture;
				TextureMid = side.MiddleTexture;
				TextureLow = side.LowTexture;
			}
		}

		#endregion

		#region ================== Constructor

		public LinedefEditForm()
		{
			// Initialize
			InitializeComponent();

			//mxd. Widow setup
			if(location != Point.Empty) 
			{
				this.StartPosition = FormStartPosition.Manual;
				this.Location = location;
			}
			
			// Fill flags lists
			foreach(KeyValuePair<string, string> lf in General.Map.Config.LinedefFlags)
				flags.Add(lf.Value, lf.Key);

			// Fill actions list
			action.GeneralizedCategories = General.Map.Config.GenActionCategories;
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

			//mxd. Setup script numbers
			scriptNumbers.Location = arg0.Location;

			foreach(ScriptItem si in General.Map.NumberedScripts)
				scriptNumbers.Items.Add(si);

			scriptNumbers.DropDownWidth = Tools.GetDropDownWidth(scriptNumbers);
			
			// Mixed activations?
			if(General.Map.FormatInterface.HasPresetActivations)
				hexenpanel.Visible = true;
			
			// Action arguments?
			if(General.Map.FormatInterface.HasActionArgs)
				argspanel.Visible = true;
			
			// Arrange panels
			if(!General.Map.FormatInterface.HasMixedActivations &&
					!General.Map.FormatInterface.HasActionArgs &&
					!General.Map.FormatInterface.HasPresetActivations)
			{
				actiongroup.Height = argspanel.Top + argspanel.Margin.Top; //mxd
			}
			
			// Arrange or hide Identification panel
			if(General.Map.FormatInterface.HasLinedefTag)
			{
				// Match position after the action group
				idgroup.Top = actiongroup.Bottom + actiongroup.Margin.Bottom + idgroup.Margin.Top;
				panel.Height = idgroup.Bottom + idgroup.Margin.Bottom * 2;
			}
			else
			{
				idgroup.Visible = false;
				panel.Height = actiongroup.Bottom + actiongroup.Margin.Bottom * 2;
			}

			// Arrange Apply/Cancel buttons
			apply.Top = panel.Bottom + panel.Margin.Bottom + apply.Margin.Top;
			cancel.Top = apply.Top;

			// Update window height
			this.Height = apply.Bottom + apply.Margin.Bottom * 2 + (this.Height - this.ClientRectangle.Height) + 1;
		}

		#endregion

		#region ================== Methods

		// This sets up the form to edit the given lines
		public void Setup(ICollection<Linedef> lines)
		{
			preventchanges = true;
			
			// Keep this list
			this.lines = lines;
			if(lines.Count > 1) this.Text = "Edit Linedefs (" + lines.Count + ")";
			linedefProps = new List<LinedefProperties>();

			//mxd. Make undo
			string undodesc = "linedef";
			if(lines.Count > 1)	undodesc = lines.Count + " linedefs";
			General.Map.UndoRedo.CreateUndo("Edit " + undodesc);
			
			////////////////////////////////////////////////////////////////////////
			// Set all options to the first linedef properties
			////////////////////////////////////////////////////////////////////////

			// Get first line
			Linedef fl = General.GetByIndex(lines, 0);
			
			// Flags
			foreach(CheckBox c in flags.Checkboxes)
				if(fl.Flags.ContainsKey(c.Tag.ToString())) c.Checked = fl.Flags[c.Tag.ToString()];
			
			// Activations
			foreach(LinedefActivateInfo ai in activation.Items)
				if((fl.Activate & ai.Index) == ai.Index) activation.SelectedItem = ai;

			// Action/tags
			action.Value = fl.Action;

			if(General.Map.FormatInterface.HasLinedefTag) //mxd
			{
				tagSelector.Setup(UniversalType.LinedefTag);
				tagSelector.SetTag(fl.Tag);
			}

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
				fronthigh.Required = fl.Front.HighRequired();
				frontmid.Required = fl.Front.MiddleRequired();
				frontlow.Required = fl.Front.LowRequired();
				frontsector.Text = fl.Front.Sector.Index.ToString();
				frontTextureOffset.SetValues(fl.Front.OffsetX, fl.Front.OffsetY, true); //mxd
			}

			// Back settings
			if(fl.Back != null)
			{
				backhigh.TextureName = fl.Back.HighTexture;
				backmid.TextureName = fl.Back.MiddleTexture;
				backlow.TextureName = fl.Back.LowTexture;
				backhigh.Required = fl.Back.HighRequired();
				backmid.Required = fl.Back.MiddleRequired();
				backlow.Required = fl.Back.LowRequired();
				backsector.Text = fl.Back.Sector.Index.ToString();
				backTextureOffset.SetValues(fl.Back.OffsetX, fl.Back.OffsetY, true); //mxd
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
					if(c.CheckState == CheckState.Indeterminate) continue; //mxd
					if(l.IsFlagSet(c.Tag.ToString()) != c.Checked) 
					{
						c.ThreeState = true;
						c.CheckState = CheckState.Indeterminate;
					}
				}

				// Activations
				if(activation.Items.Count > 0)
				{
					LinedefActivateInfo sai = (activation.Items[0] as LinedefActivateInfo);
					foreach(LinedefActivateInfo ai in activation.Items)
						if((l.Activate & ai.Index) == ai.Index) sai = ai;
					if(sai != activation.SelectedItem) activation.SelectedIndex = -1;
				}

				// Action/tags
				if(l.Action != action.Value) action.Empty = true;
				if(General.Map.FormatInterface.HasLinedefTag && l.Tag != fl.Tag) tagSelector.ClearTag(); //mxd
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
					//mxd
					if (fronthigh.TextureName != l.Front.HighTexture) 
					{
						if(!fronthigh.Required && l.Front.HighRequired()) fronthigh.Required = true;
						fronthigh.MultipleTextures = true; //mxd
						fronthigh.TextureName = string.Empty;
					}
					if (frontmid.TextureName != l.Front.MiddleTexture) 
					{
						if(!frontmid.Required && l.Front.MiddleRequired()) frontmid.Required = true;
						frontmid.MultipleTextures = true; //mxd
						frontmid.TextureName = string.Empty;
					}
					if (frontlow.TextureName != l.Front.LowTexture) 
					{
						if(!frontlow.Required && l.Front.LowRequired()) frontlow.Required = true;
						frontlow.MultipleTextures = true; //mxd
						frontlow.TextureName = string.Empty;
					}
					if(frontsector.Text != l.Front.Sector.Index.ToString()) frontsector.Text = string.Empty;

					frontTextureOffset.SetValues(l.Front.OffsetX, l.Front.OffsetY, false); //mxd
				}

				// Back settings
				if(l.Back != null)
				{
					//mxd
					if (backhigh.TextureName != l.Back.HighTexture) 
					{
						if(!backhigh.Required && l.Back.HighRequired()) backhigh.Required = true;
						backhigh.MultipleTextures = true; //mxd
						backhigh.TextureName = string.Empty;
					}
					if (backmid.TextureName != l.Back.MiddleTexture) 
					{
						if(!backmid.Required && l.Back.MiddleRequired()) backmid.Required = true;
						backmid.MultipleTextures = true; //mxd
						backmid.TextureName = string.Empty;
					}
					if (backlow.TextureName != l.Back.LowTexture) 
					{
						if(!backlow.Required && l.Back.LowRequired()) backlow.Required = true;
						backlow.MultipleTextures = true; //mxd
						backlow.TextureName = string.Empty;
					}
					if(backsector.Text != l.Back.Sector.Index.ToString()) backsector.Text = string.Empty;

					backTextureOffset.SetValues(l.Back.OffsetX, l.Back.OffsetY, false); //mxd
				}

				//mxd
				linedefProps.Add(new LinedefProperties(l));
			}
			
			// Refresh controls so that they show their image
			backhigh.Refresh();
			backmid.Refresh();
			backlow.Refresh();
			fronthigh.Refresh();
			frontmid.Refresh();
			frontlow.Refresh();

			preventchanges = false;

			UpdateScriptControls(); //mxd
			actionhelp.UpdateAction(action.GetValue()); //mxd

			//mxd. Set intial script-related values, if required
			if(Array.IndexOf(GZBuilder.GZGeneral.ACS_SPECIALS, action.Value) != -1) 
			{
				int a0 = arg0.GetResult(0);
				if(a0 > 0) 
				{
					for(int i = 0; i < General.Map.NumberedScripts.Count; i++) 
					{
						if(General.Map.NumberedScripts[i].Index == a0) 
						{
							scriptNumbers.SelectedIndex = i;
							break;
						}
					}

					if(scriptNumbers.SelectedIndex == -1) 
					{
						scriptNumbers.Items.Add(new ScriptItem(a0, "Script " + a0));
						scriptNumbers.SelectedIndex = scriptNumbers.Items.Count - 1;
					}
				} 
				else 
				{
					scriptNumbers.Text = arg0.Text;
				}
			} 
			else 
			{
				scriptNumbers.Text = "0";
			}
		}

		//mxd
		private void UpdateScriptControls()
		{
			scriptNumbers.Visible = (Array.IndexOf(GZBuilder.GZGeneral.ACS_SPECIALS, action.Value) != -1);
		}

		#endregion

		#region ================== Events

		// Apply clicked
		private void apply_Click(object sender, EventArgs e)
		{
			Sector s;
			int index;
			
			// Verify the tag
			if(General.Map.FormatInterface.HasLinedefTag)
			{
				tagSelector.ValidateTag(); //mxd
				if(((tagSelector.GetTag(0) < General.Map.FormatInterface.MinTag) || (tagSelector.GetTag(0) > General.Map.FormatInterface.MaxTag))) 
				{
					General.ShowWarningMessage("Linedef tag must be between " + General.Map.FormatInterface.MinTag + " and " + General.Map.FormatInterface.MaxTag + ".", MessageBoxButtons.OK);
					return;
				}
			}
			
			// Verify the action
			if((action.Value < General.Map.FormatInterface.MinAction) || (action.Value > General.Map.FormatInterface.MaxAction))
			{
				General.ShowWarningMessage("Linedef action must be between " + General.Map.FormatInterface.MinAction + " and " + General.Map.FormatInterface.MaxAction + ".", MessageBoxButtons.OK);
				return;
			}

			//mxd
			bool hasAcs = !action.Empty && Array.IndexOf(GZBuilder.GZGeneral.ACS_SPECIALS, action.Value) != -1;
			
			// Go for all the lines
			int tagoffset = 0; //mxd
			foreach(Linedef l in lines)
			{
				// Apply chosen activation flag
				if(activation.SelectedIndex > -1)
					l.Activate = (activation.SelectedItem as LinedefActivateInfo).Index;
				
				// Action/tags
				l.Tag = General.Clamp(tagSelector.GetSmartTag(l.Tag, tagoffset++), General.Map.FormatInterface.MinTag, General.Map.FormatInterface.MaxTag); //mxd
				if(!action.Empty) 
				{
					l.Action = action.Value;

					//mxd. Script name/number handling
					if(hasAcs) 
					{
						if(!string.IsNullOrEmpty(scriptNumbers.Text)) 
						{
							if(scriptNumbers.SelectedItem != null)
								l.Args[0] = ((ScriptItem)scriptNumbers.SelectedItem).Index;
							else if(!int.TryParse(scriptNumbers.Text.Trim(), out l.Args[0]))
								l.Args[0] = 0;
						}
					} 
					else 
					{
						l.Args[0] = arg0.GetResult(l.Args[0]);
					}
				}
				else
				{
					l.Args[0] = arg0.GetResult(l.Args[0]);
				}

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
					index = frontsector.GetResult(index);
					if((index > -1) && (index < General.Map.Map.Sectors.Count))
					{
						s = General.Map.Map.GetSectorByIndex(index);
						if(s == null) s = General.Map.Map.CreateSector();
						
						if(s != null)
						{
							// Create new sidedef?
							if(l.Front == null) General.Map.Map.CreateSidedef(l, true, s);

							// Change sector?
							if(l.Front != null && l.Front.Sector != s) l.Front.SetSector(s);
						}
					}
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
					index = backsector.GetResult(index);
					if((index > -1) && (index < General.Map.Map.Sectors.Count))
					{
						s = General.Map.Map.GetSectorByIndex(index) ?? General.Map.Map.CreateSector();

						if(s != null)
						{
							// Create new sidedef?
							if(l.Back == null) General.Map.Map.CreateSidedef(l, false, s);
							
							// Change sector?
							if(l.Back != null && l.Back.Sector != s) l.Back.SetSector(s);
						}
					}
				}
			}

			// Update the used textures
			General.Map.Data.UpdateUsedTextures();
			
			// Done
			General.Map.IsChanged = true;
			if(OnValuesChanged != null)	OnValuesChanged(this, EventArgs.Empty); //mxd
			this.DialogResult = DialogResult.OK;
			this.Close();
		}

		// Cancel clicked
		private void cancel_Click(object sender, EventArgs e)
		{
			//mxd. Let's pretend nothing of this really happened...
			General.Map.UndoRedo.WithdrawUndo();
			
			// Be gone
			this.DialogResult = DialogResult.Cancel;
			this.Close();
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
			arg0.ForeColor = (arg0label.Enabled ? SystemColors.WindowText : SystemColors.GrayText);
			arg1.ForeColor = (arg1label.Enabled ? SystemColors.WindowText : SystemColors.GrayText);
			arg2.ForeColor = (arg2label.Enabled ? SystemColors.WindowText : SystemColors.GrayText);
			arg3.ForeColor = (arg3label.Enabled ? SystemColors.WindowText : SystemColors.GrayText);
			arg4.ForeColor = (arg4label.Enabled ? SystemColors.WindowText : SystemColors.GrayText);
			arg0.Setup(General.Map.Config.LinedefActions[showaction].Args[0]);
			arg1.Setup(General.Map.Config.LinedefActions[showaction].Args[1]);
			arg2.Setup(General.Map.Config.LinedefActions[showaction].Args[2]);
			arg3.Setup(General.Map.Config.LinedefActions[showaction].Args[3]);
			arg4.Setup(General.Map.Config.LinedefActions[showaction].Args[4]);

			if(!preventchanges) 
			{
				// mxd. Apply action's default arguments 
				if(showaction != 0) 
				{
					arg0.SetDefaultValue();
					arg1.SetDefaultValue();
					arg2.SetDefaultValue();
					arg3.SetDefaultValue();
					arg4.SetDefaultValue();
				} 
				else //or set them to 0
				{ 
					arg0.SetValue(0);
					arg1.SetValue(0);
					arg2.SetValue(0);
					arg3.SetValue(0);
					arg4.SetValue(0);
				}

				//mxd. Update what must be updated
				UpdateScriptControls();
				actionhelp.UpdateAction(showaction);
			} 
		}

		// Browse Action clicked
		private void browseaction_Click(object sender, EventArgs e)
		{
			action.Value = ActionBrowserForm.BrowseAction(this, action.Value);
		}

		//mxd. Store window location
		private void LinedefEditForm_FormClosing(object sender, FormClosingEventArgs e) 
		{
			location = this.Location;
		}

		// Help!
		private void LinedefEditForm_HelpRequested(object sender, HelpEventArgs hlpevent)
		{
			General.ShowHelp("w_linedefedit.html");
			hlpevent.Handled = true;
		}

		#endregion

		#region ================== Linedef realtime events (mxd)

		private void flags_OnValueChanged(object sender, EventArgs e) 
		{
			if(preventchanges) return;
			int i = 0;

			foreach(Linedef l in lines) 
			{
				// Apply all flags
				foreach(CheckBox c in flags.Checkboxes) 
				{
					if(c.CheckState == CheckState.Checked)
						l.SetFlag(c.Tag.ToString(), true);
					else if(c.CheckState == CheckState.Unchecked)
						l.SetFlag(c.Tag.ToString(), false);
					else if(linedefProps[i].Flags.ContainsKey(c.Tag.ToString()))
						l.SetFlag(c.Tag.ToString(), linedefProps[i].Flags[c.Tag.ToString()]);
					else //linedefs created in the editor have empty Flags by default
						l.SetFlag(c.Tag.ToString(), false);
				}

				i++;
			}
			
			General.Map.IsChanged = true;
			if(OnValuesChanged != null)	OnValuesChanged(this, EventArgs.Empty);
		}

		#endregion

		#region ================== Sidedef reltime events (mxd)

		private void fronthigh_OnValueChanged(object sender, EventArgs e) 
		{
			if(preventchanges) return;

			//restore values
			if(string.IsNullOrEmpty(fronthigh.TextureName)) 
			{
				int i = 0;

				foreach(Linedef l in lines) 
				{
					if(l.Front != null) l.Front.SetTextureHigh(linedefProps[i].Front != null ? linedefProps[i].Front.TextureTop : "-");
					i++;
				}
			
			} 
			else //update values
			{
				foreach(Linedef l in lines) 
					if(l.Front != null)	l.Front.SetTextureHigh(fronthigh.GetResult(l.Front.HighTexture));
			}

			// Update the used textures
			General.Map.Data.UpdateUsedTextures();

			General.Map.IsChanged = true;
			if(OnValuesChanged != null) OnValuesChanged(this, EventArgs.Empty);
		}

		private void frontmid_OnValueChanged(object sender, EventArgs e) 
		{
			if(preventchanges) return;

			//restore values
			if(string.IsNullOrEmpty(frontmid.TextureName)) 
			{
				int i = 0;

				foreach(Linedef l in lines) 
				{
					if(l.Front != null) l.Front.SetTextureMid(linedefProps[i].Front != null ? linedefProps[i].Front.TextureMid : "-");
					i++;
				}
			
			} 
			else //update values
			{
				foreach(Linedef l in lines) 
					if(l.Front != null)	l.Front.SetTextureMid(frontmid.GetResult(l.Front.MiddleTexture));
			}

			// Update the used textures
			General.Map.Data.UpdateUsedTextures();

			General.Map.IsChanged = true;
			if(OnValuesChanged != null) OnValuesChanged(this, EventArgs.Empty);
		}

		private void frontlow_OnValueChanged(object sender, EventArgs e) 
		{
			if(preventchanges) return;

			//restore values
			if(string.IsNullOrEmpty(frontlow.TextureName)) 
			{
				int i = 0;

				foreach(Linedef l in lines) 
				{
					if(l.Front != null) l.Front.SetTextureLow(linedefProps[i].Front != null ? linedefProps[i].Front.TextureLow : "-");
					i++;
				}
			
			} 
			else //update values
			{
				foreach(Linedef l in lines)
					if(l.Front != null) l.Front.SetTextureLow(frontlow.GetResult(l.Front.LowTexture));
			}

			// Update the used textures
			General.Map.Data.UpdateUsedTextures();

			General.Map.IsChanged = true;
			if(OnValuesChanged != null) OnValuesChanged(this, EventArgs.Empty);
		}

		private void backhigh_OnValueChanged(object sender, EventArgs e) 
		{
			if(preventchanges) return;

			//restore values
			if(string.IsNullOrEmpty(backhigh.TextureName)) 
			{
				int i = 0;

				foreach(Linedef l in lines) 
				{
					if(l.Back != null) l.Back.SetTextureHigh(linedefProps[i].Back != null ? linedefProps[i].Back.TextureTop : "-");
					i++;
				}
			
			} 
			else //update values
			{
				foreach(Linedef l in lines)
					if(l.Back != null) l.Back.SetTextureHigh(backhigh.GetResult(l.Back.HighTexture));
			}

			// Update the used textures
			General.Map.Data.UpdateUsedTextures();

			General.Map.IsChanged = true;
			if(OnValuesChanged != null) OnValuesChanged(this, EventArgs.Empty);
		}

		private void backmid_OnValueChanged(object sender, EventArgs e) 
		{
			if(preventchanges) return;

			//restore values
			if(string.IsNullOrEmpty(backmid.TextureName)) 
			{
				int i = 0;

				foreach(Linedef l in lines) 
				{
					if(l.Back != null) l.Back.SetTextureMid(linedefProps[i].Back != null ? linedefProps[i].Back.TextureMid : "-");
					i++;
				}
			
			} 
			else //update values
			{
				foreach(Linedef l in lines)
					if(l.Back != null) l.Back.SetTextureMid(backmid.GetResult(l.Back.MiddleTexture));
			}

			// Update the used textures
			General.Map.Data.UpdateUsedTextures();

			General.Map.IsChanged = true;
			if(OnValuesChanged != null) OnValuesChanged(this, EventArgs.Empty);
		}

		private void backlow_OnValueChanged(object sender, EventArgs e) 
		{
			if(preventchanges) return;

			//restore values
			if(string.IsNullOrEmpty(backlow.TextureName)) 
			{
				int i = 0;

				foreach(Linedef l in lines) 
				{
					if(l.Back != null) l.Back.SetTextureLow(linedefProps[i].Back != null ? linedefProps[i].Back.TextureLow : "-");
					i++;
				}
			
			} 
			else //update values 
			{
				foreach(Linedef l in lines)
					if(l.Back != null) l.Back.SetTextureLow(backlow.GetResult(l.Back.LowTexture));
			}

			// Update the used textures
			General.Map.Data.UpdateUsedTextures();

			General.Map.IsChanged = true;
			if(OnValuesChanged != null) OnValuesChanged(this, EventArgs.Empty);
		}

		private void frontTextureOffset_OnValuesChanged(object sender, EventArgs e) 
		{
			if(preventchanges) return;

			int i = 0;

			foreach(Linedef l in lines) 
			{
				if(l.Front != null) 
				{
					if(linedefProps[i].Front != null) 
					{
						l.Front.OffsetX = frontTextureOffset.GetValue1(linedefProps[i].Front.OffsetX);
						l.Front.OffsetY = frontTextureOffset.GetValue2(linedefProps[i].Front.OffsetY);
					} 
					else 
					{
						l.Front.OffsetX = frontTextureOffset.GetValue1(0);
						l.Front.OffsetY = frontTextureOffset.GetValue2(0);
					}
				}

				i++;
			}

			General.Map.IsChanged = true;
			if(OnValuesChanged != null) OnValuesChanged(this, EventArgs.Empty);
		}

		private void backTextureOffset_OnValuesChanged(object sender, EventArgs e) 
		{
			if(preventchanges) return;

			int i = 0;

			foreach(Linedef l in lines) 
			{
				if(l.Back != null) 
				{
					if(linedefProps[i].Back != null) 
					{
						l.Back.OffsetX = backTextureOffset.GetValue1(linedefProps[i].Back.OffsetX);
						l.Back.OffsetY = backTextureOffset.GetValue2(linedefProps[i].Back.OffsetY);
					} 
					else
					{
						l.Back.OffsetX = backTextureOffset.GetValue1(0);
						l.Back.OffsetY = backTextureOffset.GetValue2(0);
					}
				}

				i++;
			}
			
			General.Map.IsChanged = true;
			if(OnValuesChanged != null) OnValuesChanged(this, EventArgs.Empty);
		}

		#endregion

	}
}
