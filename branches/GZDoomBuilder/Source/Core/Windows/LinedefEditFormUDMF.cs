
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
using CodeImp.DoomBuilder.GZBuilder.Tools;
using CodeImp.DoomBuilder.GZBuilder.Controls;
using CodeImp.DoomBuilder.Controls;

#endregion

namespace CodeImp.DoomBuilder.Windows
{
	internal partial class LinedefEditFormUDMF : DelayedForm
	{
		#region ================== Events

		public event EventHandler OnValuesChanged; //mxd

		#endregion

		#region ================== Constants


		#endregion

		#region ================== Variables

		private ICollection<Linedef> lines;
		private List<LinedefProperties> linedefprops; //mxd
		private bool preventchanges;
		private bool undocreated; //mxd
		private string arg0str; //mxd
		private bool haveArg0Str; //mxd
		private readonly string[] renderstyles; //mxd
		private readonly List<int> keynumbers; //mxd 

		//mxd. Persistent settings
		private static bool linkFrontTopScale;
		private static bool linkFrontMidScale;
		private static bool linkFrontBottomScale;
		private static bool linkBackTopScale;
		private static bool linkBackMidScale;
		private static bool linkBackBottomScale;

		private readonly List<PairedFieldsControl> frontUdmfControls; //mxd
		private readonly List<PairedFieldsControl> backUdmfControls; //mxd

		//mxd. Window setup stuff
		private static Point location = Point.Empty;
		private static int activetab;

		private struct LinedefProperties //mxd
		{
			public readonly Dictionary<string, bool> Flags;
			public readonly float Alpha;

			public readonly SidedefProperties Front;
			public readonly SidedefProperties Back;

			public LinedefProperties(Linedef line) 
			{
				Front = (line.Front != null ? new SidedefProperties(line.Front) : null);
				Back = (line.Back != null ? new SidedefProperties(line.Back) : null);
				Alpha = UDMFTools.GetFloat(line.Fields, "alpha", 1.0f);
				Flags = line.GetFlags();
			}
		}

		private class SidedefProperties //mxd
		{
			public readonly Dictionary<string, bool> Flags;

			public readonly float ScaleTopX;
			public readonly float ScaleTopY;
			public readonly float ScaleMidX;
			public readonly float ScaleMidY;
			public readonly float ScaleBottomX;
			public readonly float ScaleBottomY;

			public readonly int OffsetX;
			public readonly int OffsetY;

			public readonly float OffsetTopX;
			public readonly float OffsetTopY;
			public readonly float OffsetMidX;
			public readonly float OffsetMidY;
			public readonly float OffsetBottomX;
			public readonly float OffsetBottomY;

			public readonly int Brightness;
			public readonly bool AbsoluteBrightness;

			public readonly string TextureTop;
			public readonly string TextureMid;
			public readonly string TextureLow;

			public SidedefProperties(Sidedef side) 
			{
				Flags = side.GetFlags();

				//offset
				OffsetX = side.OffsetX;
				OffsetY = side.OffsetY;

				Brightness = UDMFTools.GetInteger(side.Fields, "light", 0);
				AbsoluteBrightness = side.Fields.GetValue("lightabsolute", false);

				//scales
				ScaleTopX = UDMFTools.GetFloat(side.Fields, "scalex_top", 1.0f);
				ScaleTopY = UDMFTools.GetFloat(side.Fields, "scaley_top", 1.0f);
				ScaleMidX = UDMFTools.GetFloat(side.Fields, "scalex_mid", 1.0f);
				ScaleMidY = UDMFTools.GetFloat(side.Fields, "scaley_mid", 1.0f);
				ScaleBottomX = UDMFTools.GetFloat(side.Fields, "scalex_bottom", 1.0f);
				ScaleBottomY = UDMFTools.GetFloat(side.Fields, "scaley_bottom", 1.0f);

				//offsets
				OffsetTopX = UDMFTools.GetFloat(side.Fields, "offsetx_top", 0f);
				OffsetTopY = UDMFTools.GetFloat(side.Fields, "offsety_top", 0f);
				OffsetMidX = UDMFTools.GetFloat(side.Fields, "offsetx_mid", 0f);
				OffsetMidY = UDMFTools.GetFloat(side.Fields, "offsety_mid", 0f);
				OffsetBottomX = UDMFTools.GetFloat(side.Fields, "offsetx_bottom", 0f);
				OffsetBottomY = UDMFTools.GetFloat(side.Fields, "offsety_bottom", 0f);

				//textures
				TextureTop = side.HighTexture;
				TextureMid = side.MiddleTexture;
				TextureLow = side.LowTexture;
			}
		}

		#endregion

		#region ================== Constructor

		public LinedefEditFormUDMF(bool selectfront, bool selectback)
		{
			// Initialize
			InitializeComponent();

			//mxd. Widow setup
			if(location != Point.Empty) 
			{
				this.StartPosition = FormStartPosition.Manual;
				this.Location = location;
				if(General.Settings.StoreSelectedEditTab && activetab > 0)
				{
					// When front or back tab was previously selected, switch to appropriate side (selectfront/selectback are set in BaseVisualGeometrySidedef.OnEditEnd)
					if((selectfront || selectback) && (activetab == 1 || activetab == 2))
						tabs.SelectTab(selectfront ? 1 : 2);
					else
						tabs.SelectTab(activetab);
				}
			}
			
			// Fill flags lists
			foreach(KeyValuePair<string, string> lf in General.Map.Config.LinedefFlags)
				flags.Add(lf.Value, lf.Key);
			flags.Enabled = General.Map.Config.LinedefFlags.Count > 0;

			//mxd
			foreach(KeyValuePair<string, string> lf in General.Map.Config.SidedefFlags) 
			{
				flagsFront.Add(lf.Value, lf.Key);
				flagsBack.Add(lf.Value, lf.Key);
			}
			flagsFront.Enabled = General.Map.Config.SidedefFlags.Count > 0;
			flagsBack.Enabled = General.Map.Config.SidedefFlags.Count > 0;

			// Fill actions list
			action.AddInfo(General.Map.Config.SortedLinedefActions.ToArray());

			// Fill activations list
			foreach(LinedefActivateInfo ai in General.Map.Config.LinedefActivates) udmfactivates.Add(ai.Title, ai);

			//mxd. Fill keys list
			keynumbers = new List<int>();
			if (General.Map.Config.Enums.ContainsKey("keys"))
			{
				foreach (EnumItem item in General.Map.Config.Enums["keys"])
				{
					keynumbers.Add(item.GetIntValue());
					lockpick.Items.Add(item);
				}
			}
			lockpick.Enabled = (keynumbers.Count > 0);
			labellockpick.Enabled = (keynumbers.Count > 0);
			
			// Initialize image selectors
			fronthigh.Initialize();
			frontmid.Initialize();
			frontlow.Initialize();
			backhigh.Initialize();
			backmid.Initialize();
			backlow.Initialize();

			//mxd. Setup script numbers
			scriptNumbers.Location = new Point(arg0.Location.X, arg0.Location.Y + 2);
			foreach (ScriptItem si in General.Map.NumberedScripts)
				scriptNumbers.Items.Add(new ColoredComboBoxItem(si, si.IsInclude ? SystemColors.HotTrack : SystemColors.WindowText));
			scriptNumbers.DropDownWidth = Tools.GetDropDownWidth(scriptNumbers);

			//mxd. Setup script names
			scriptNames.Location = scriptNumbers.Location;
			foreach (ScriptItem nsi in General.Map.NamedScripts)
				scriptNames.Items.Add(new ColoredComboBoxItem(nsi, nsi.IsInclude ? SystemColors.HotTrack : SystemColors.WindowText));
			scriptNames.DropDownWidth = Tools.GetDropDownWidth(scriptNames);

			// Initialize custom fields editor
			fieldslist.Setup("linedef");

			// Fill universal fields list
			fieldslist.ListFixedFields(General.Map.Config.LinedefFields);

			//initialize controls
			frontUdmfControls = new List<PairedFieldsControl> { pfcFrontOffsetTop, pfcFrontOffsetMid, pfcFrontOffsetBottom, pfcFrontScaleTop, pfcFrontScaleMid, pfcFrontScaleBottom };
			backUdmfControls = new List<PairedFieldsControl> { pfcBackOffsetTop, pfcBackOffsetMid, pfcBackOffsetBottom, pfcBackScaleTop, pfcBackScaleMid, pfcBackScaleBottom };

			// Setup renderstyles
			renderstyles = new string[General.Map.Config.LinedefRenderStyles.Count];
			General.Map.Config.LinedefRenderStyles.Keys.CopyTo(renderstyles, 0);
			renderStyle.Enabled = (General.Map.Config.LinedefRenderStyles.Count > 0);
			labelrenderstyle.Enabled = (General.Map.Config.LinedefRenderStyles.Count > 0);

			// Fill renderstyles
			foreach(KeyValuePair<string, string> lf in General.Map.Config.LinedefRenderStyles)
				renderStyle.Items.Add(lf.Value);

			//Restore value linking
			pfcFrontScaleTop.LinkValues = linkFrontTopScale;
			pfcFrontScaleMid.LinkValues = linkFrontMidScale;
			pfcFrontScaleBottom.LinkValues = linkFrontBottomScale;
			pfcBackScaleTop.LinkValues = linkBackTopScale;
			pfcBackScaleMid.LinkValues = linkBackMidScale;
			pfcBackScaleBottom.LinkValues = linkBackBottomScale;
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
			linedefprops = new List<LinedefProperties>();
			
			////////////////////////////////////////////////////////////////////////
			// Set all options to the first linedef properties
			////////////////////////////////////////////////////////////////////////

			// Get first line
			Linedef fl = General.GetByIndex(lines, 0);
			
			// Flags
			foreach(CheckBox c in flags.Checkboxes)
				if(fl.Flags.ContainsKey(c.Tag.ToString())) c.Checked = fl.Flags[c.Tag.ToString()];

			// UDMF Activations
			foreach(CheckBox c in udmfactivates.Checkboxes)
			{
				LinedefActivateInfo ai = (c.Tag as LinedefActivateInfo);
				if(fl.Flags.ContainsKey(ai.Key)) c.Checked = fl.Flags[ai.Key];
			}

			fieldslist.SetValues(fl.Fields, true); // Custom fields
			commenteditor.SetValues(fl.Fields, true); //mxd. Comments
			renderStyle.SelectedIndex = Array.IndexOf(renderstyles, fl.Fields.GetValue("renderstyle", "translucent"));
			alpha.Text = General.Clamp(fl.Fields.GetValue("alpha", 1.0f), 0f, 1f).ToString();
			arg0str = fl.Fields.GetValue("arg0str", string.Empty);
			haveArg0Str = !string.IsNullOrEmpty(arg0str);

			// Locknumber
			int locknumber = fl.Fields.GetValue("locknumber", 0);
			lockpick.SelectedIndex = keynumbers.IndexOf(locknumber);
			if(lockpick.SelectedIndex == -1) lockpick.Text = locknumber.ToString();

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

				//flags
				foreach(CheckBox c in flagsFront.Checkboxes)
					if(fl.Front.Flags.ContainsKey(c.Tag.ToString())) c.Checked = fl.Front.Flags[c.Tag.ToString()];

				//front settings
				foreach(PairedFieldsControl pfc in frontUdmfControls)
					pfc.SetValuesFrom(fl.Front.Fields, true);

				lightFront.Text = UDMFTools.GetInteger(fl.Front.Fields, "light", 0).ToString();
				cbLightAbsoluteFront.Checked = fl.Front.Fields.GetValue("lightabsolute", false);

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

				//flags
				foreach(CheckBox c in flagsBack.Checkboxes)
					if(fl.Back.Flags.ContainsKey(c.Tag.ToString())) c.Checked = fl.Back.Flags[c.Tag.ToString()];

				//back settings
				foreach(PairedFieldsControl pfc in backUdmfControls)
					pfc.SetValuesFrom(fl.Back.Fields, true);

				lightBack.Text = UDMFTools.GetInteger(fl.Back.Fields, "light", 0).ToString();
				cbLightAbsoluteBack.Checked = fl.Back.Fields.GetValue("lightabsolute", false);
 
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

				// UDMF Activations
				foreach(CheckBox c in udmfactivates.Checkboxes)
				{
					if(c.CheckState == CheckState.Indeterminate) continue; //mxd

					LinedefActivateInfo ai = (c.Tag as LinedefActivateInfo);
					if(l.IsFlagSet(ai.Key) != c.Checked) 
					{
						c.ThreeState = true;
						c.CheckState = CheckState.Indeterminate;
					}
				}

				//mxd. UDMF Settings

				// Render style
				if(renderStyle.SelectedIndex > -1 && renderStyle.SelectedIndex != Array.IndexOf(renderstyles, l.Fields.GetValue("renderstyle", "translucent")))
					renderStyle.SelectedIndex = -1;

				// Alpha
				if(!string.IsNullOrEmpty(alpha.Text) && General.Clamp(alpha.GetResultFloat(1.0f), 0f, 1f) != l.Fields.GetValue("alpha", 1.0f))
					alpha.Text = string.Empty;

				// Locknumber
				if(!string.IsNullOrEmpty(lockpick.Text)) 
				{
					if(lockpick.SelectedIndex == -1) 
					{
						if(int.TryParse(lockpick.Text, out locknumber) && locknumber != l.Fields.GetValue("locknumber", 0)) 
						{
							lockpick.SelectedIndex = -1;
							lockpick.Text = string.Empty;
						}
					} 
					else if(keynumbers[lockpick.SelectedIndex] != l.Fields.GetValue("locknumber", 0)) 
					{
						lockpick.SelectedIndex = -1;
						lockpick.Text = string.Empty;
					}
				}

				// arg0str
				if(arg0str != l.Fields.GetValue("arg0str", string.Empty)) 
				{
					haveArg0Str = true;
					arg0str = string.Empty;
				}

				// Custom fields
				fieldslist.SetValues(l.Fields, false);

				//mxd. Comments
				commenteditor.SetValues(l.Fields, false);

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

					//flags
					foreach(CheckBox c in flagsFront.Checkboxes) 
					{
						if(c.CheckState == CheckState.Indeterminate) continue;
						if(l.Front.IsFlagSet(c.Tag.ToString()) != c.Checked) 
						{
							c.ThreeState = true;
							c.CheckState = CheckState.Indeterminate;
						}
					}

					//mxd
					foreach(PairedFieldsControl pfc in frontUdmfControls)
						pfc.SetValuesFrom(l.Front.Fields, false);

					if(!string.IsNullOrEmpty(lightFront.Text)) 
					{
						int light = UDMFTools.GetInteger(l.Front.Fields, "light", 0);
						if(light != lightFront.GetResult(light)) lightFront.Text = string.Empty;
					}

					if(l.Front.Fields.GetValue("lightabsolute", false) != cbLightAbsoluteFront.Checked) 
					{
						cbLightAbsoluteFront.ThreeState = true;
						cbLightAbsoluteFront.CheckState = CheckState.Indeterminate;
					}

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

					//flags
					foreach(CheckBox c in flagsBack.Checkboxes) 
					{
						if(c.CheckState == CheckState.Indeterminate) continue;
						if(l.Back.IsFlagSet(c.Tag.ToString()) != c.Checked) 
						{
							c.ThreeState = true;
							c.CheckState = CheckState.Indeterminate;
						}
					}

					//mxd
					foreach(PairedFieldsControl pfc in backUdmfControls)
						pfc.SetValuesFrom(l.Back.Fields, false);

					if(!string.IsNullOrEmpty(lightBack.Text)) 
					{
						int light = UDMFTools.GetInteger(l.Back.Fields, "light", 0);
						if(light != lightBack.GetResult(light))
							lightBack.Text = string.Empty;
					}

					if(l.Back.Fields.GetValue("lightabsolute", false) != cbLightAbsoluteBack.Checked) 
					{
						cbLightAbsoluteBack.ThreeState = true;
						cbLightAbsoluteBack.CheckState = CheckState.Indeterminate;
					}

					backTextureOffset.SetValues(l.Back.OffsetX, l.Back.OffsetY, false); //mxd
				}

				//mxd
				linedefprops.Add(new LinedefProperties(l));
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
			CheckActivationFlagsRequired(); //mxd
			actionhelp.UpdateAction(action.GetValue()); //mxd
			commenteditor.FinishSetup(); //mxd

			//mxd. Update some labels
			if (frontside.CheckState != CheckState.Unchecked)
			{
				// Update Offset labels
				labelFrontTextureOffset.Enabled = frontTextureOffset.NonDefaultValue;
				labelFrontOffsetTop.Enabled = pfcFrontOffsetTop.NonDefaultValue;
				labelFrontOffsetMid.Enabled = pfcFrontOffsetMid.NonDefaultValue;
				labelFrontOffsetBottom.Enabled = pfcFrontOffsetBottom.NonDefaultValue;

				// Update Scale labels
				labelFrontScaleTop.Enabled = pfcFrontScaleTop.NonDefaultValue;
				labelFrontScaleMid.Enabled = pfcFrontScaleMid.NonDefaultValue;
				labelFrontScaleBottom.Enabled = pfcFrontScaleBottom.NonDefaultValue;
			}
			if (backside.CheckState != CheckState.Unchecked)
			{
				// Update Offset labels
				labelBackTextureOffset.Enabled = backTextureOffset.NonDefaultValue;
				labelBackOffsetTop.Enabled = pfcBackOffsetTop.NonDefaultValue;
				labelBackOffsetMid.Enabled = pfcBackOffsetMid.NonDefaultValue;
				labelBackOffsetBottom.Enabled = pfcBackOffsetBottom.NonDefaultValue;

				// Update Scale labels
				labelBackScaleTop.Enabled = pfcBackScaleTop.NonDefaultValue;
				labelBackScaleMid.Enabled = pfcBackScaleMid.NonDefaultValue;
				labelBackScaleBottom.Enabled = pfcBackScaleBottom.NonDefaultValue;
			}

			//mxd. Set intial script-related values, if required
			if(Array.IndexOf(GZBuilder.GZGeneral.ACS_SPECIALS, action.Value) != -1) 
			{
				if(haveArg0Str) 
				{
					scriptNames.Text = arg0str;
					arg0label.Text = "Script Name:";
				} 
				else 
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
					} 

					if(scriptNumbers.SelectedIndex == -1)
					{
						scriptNumbers.Text = a0.ToString();
					}
				}
			} 
			else 
			{
				scriptNumbers.Text = "0";
			}
		}

		//mxd
		private void MakeUndo() 
		{
			if(undocreated) return;
			undocreated = true;

			//mxd. Make undo
			General.Map.UndoRedo.CreateUndo("Edit " + (lines.Count > 1 ? lines.Count + " linedefs" : "linedef"));

			if(General.Map.FormatInterface.HasCustomFields) 
			{
				foreach (Linedef l in lines)
				{
					l.Fields.BeforeFieldsChange();
					if(l.Front != null) l.Front.Fields.BeforeFieldsChange();
					if(l.Back != null) l.Back.Fields.BeforeFieldsChange();
				}
			}
		}

		//mxd
		private void UpdateScriptControls() 
		{
			if(Array.IndexOf(GZBuilder.GZGeneral.ACS_SPECIALS, action.Value) != -1) 
			{
				bool showNamedScripts = General.Map.UDMF && haveArg0Str;
				cbArgStr.Visible = General.Map.UDMF;
				cbArgStr.Checked = showNamedScripts;
				scriptNames.Visible = showNamedScripts;
				scriptNumbers.Visible = !showNamedScripts;
			} 
			else 
			{
				cbArgStr.Visible = false;
				scriptNames.Visible = false;
				scriptNumbers.Visible = false;
				cbArgStr.Checked = false;
			}

			arg0.Visible = (!scriptNames.Visible && !scriptNumbers.Visible);
		}

		//mxd
		private void UpdateArgument(ArgumentBox arg, Label label, ArgumentInfo info) 
		{
			label.Text = info.Title + ":";
			label.Enabled = info.Used;
			arg.ForeColor = (label.Enabled ? SystemColors.WindowText : SystemColors.GrayText);
			arg.Setup(info);

			// Update tooltip
			if(info.Used && !string.IsNullOrEmpty(info.ToolTip)) 
			{
				tooltip.SetToolTip(label, info.ToolTip);
				label.Font = new Font(label.Font, FontStyle.Underline);
				label.ForeColor = SystemColors.HotTrack;
			} 
			else 
			{
				tooltip.SetToolTip(label, null);
				label.Font = new Font(label.Font, FontStyle.Regular);
				label.ForeColor = SystemColors.WindowText;
			}
		}

		//mxd
		private void CheckActivationFlagsRequired()
		{
			// Display a warning if we have an action and no activation flags
			if(action.Value != 0 
				&& General.Map.Config.LinedefActions.ContainsKey(action.Value) 
				&& General.Map.Config.LinedefActions[action.Value].RequiresActivation) 
			{
				bool haveactivationflag = false;
				foreach(CheckBox c in udmfactivates.Checkboxes) 
				{
					if(c.CheckState != CheckState.Unchecked) 
					{
						haveactivationflag = true;
						break;
					}
				}

				missingactivation.Visible = !haveactivationflag;
				activationGroup.ForeColor = (!haveactivationflag ? Color.DarkRed : SystemColors.ControlText);
			} 
			else 
			{
				missingactivation.Visible = false;
				activationGroup.ForeColor = SystemColors.ControlText;
			}
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

			MakeUndo();

			//mxd
			bool hasAcs = !action.Empty && Array.IndexOf(GZBuilder.GZGeneral.ACS_SPECIALS, action.Value) != -1;
			int locknumber = 0;
			bool setlocknumber = false;
			if(!string.IsNullOrEmpty(lockpick.Text)) 
			{
				if(lockpick.SelectedIndex == -1) 
				{
					setlocknumber = int.TryParse(lockpick.Text, out locknumber);
				} 
				else 
				{
					locknumber = keynumbers[lockpick.SelectedIndex];
					setlocknumber = true;
				}
			}
			
			// Go for all the lines
			int tagoffset = 0; //mxd
			foreach(Linedef l in lines)
			{
				// UDMF activations
				foreach(CheckBox c in udmfactivates.Checkboxes)
				{
					LinedefActivateInfo ai = (c.Tag as LinedefActivateInfo);
					switch (c.CheckState)
					{
						case CheckState.Checked: l.SetFlag(ai.Key, true); break;
						case CheckState.Unchecked: l.SetFlag(ai.Key, false); break;
					}
				}
				
				// Action/tags
				l.Tag = General.Clamp(tagSelector.GetSmartTag(l.Tag, tagoffset++), General.Map.FormatInterface.MinTag, General.Map.FormatInterface.MaxTag); //mxd
				if(!action.Empty) 
				{
					l.Action = action.Value;

					//mxd. Script name/number handling
					if(hasAcs) 
					{
						//apply script number
						if(!cbArgStr.Checked) 
						{ 
							if(!string.IsNullOrEmpty(scriptNumbers.Text)) 
							{
								if(scriptNumbers.SelectedItem != null)
									l.Args[0] = ((ScriptItem)((ColoredComboBoxItem)scriptNumbers.SelectedItem).Value).Index;
								else if(!int.TryParse(scriptNumbers.Text.Trim(), out l.Args[0]))
									l.Args[0] = 0;

								if(l.Fields.ContainsKey("arg0str")) l.Fields.Remove("arg0str");
							}
						} 
						else //apply arg0str
						{ 
							if(!string.IsNullOrEmpty(scriptNames.Text)) 
								l.Fields["arg0str"] = new UniValue(UniversalType.String, scriptNames.Text);
						}
					} 
					else 
					{
						l.Args[0] = arg0.GetResult(l.Args[0]);
						if(l.Fields.ContainsKey("arg0str")) l.Fields.Remove("arg0str");
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
						s = General.Map.Map.GetSectorByIndex(index);
						if(s == null) s = General.Map.Map.CreateSector();
						
						if(s != null)
						{
							// Create new sidedef?
							if(l.Back == null) General.Map.Map.CreateSidedef(l, false, s);
							
							// Change sector?
							if(l.Back != null && l.Back.Sector != s) l.Back.SetSector(s);
						}
					}
				}

				//mxd. UDMF Settings
				fieldslist.Apply(l.Fields);
				if(setlocknumber) UDMFTools.SetInteger(l.Fields, "locknumber", locknumber, 0);
				commenteditor.Apply(l.Fields);
			}

			//mxd. Store value linking
			linkFrontTopScale = pfcFrontScaleTop.LinkValues;
			linkFrontMidScale = pfcFrontScaleMid.LinkValues;
			linkFrontBottomScale = pfcFrontScaleBottom.LinkValues;
			linkBackTopScale = pfcBackScaleTop.LinkValues;
			linkBackMidScale = pfcBackScaleMid.LinkValues;
			linkBackBottomScale = pfcBackScaleBottom.LinkValues;

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
			if(undocreated) General.Map.UndoRedo.WithdrawUndo();
			
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
			tabfront.ImageIndex = (frontside.CheckState == CheckState.Unchecked ? 1 : 0);
		}

		// Back side (un)checked
		private void backside_CheckStateChanged(object sender, EventArgs e) 
		{
			// Enable/disable panel
			// NOTE: Also enabled when checkbox is grayed!
			backgroup.Enabled = (backside.CheckState != CheckState.Unchecked);
			tabback.ImageIndex = (backside.CheckState == CheckState.Unchecked ? 1 : 0);
		}

		// Action changes
		private void action_ValueChanges(object sender, EventArgs e)
		{
			int showaction = 0;
			
			// Only when line type is known
			if(General.Map.Config.LinedefActions.ContainsKey(action.Value)) showaction = action.Value;
			
			// Change the argument descriptions
			UpdateArgument(arg0, arg0label, General.Map.Config.LinedefActions[showaction].Args[0]); //mxd
			UpdateArgument(arg1, arg1label, General.Map.Config.LinedefActions[showaction].Args[1]); //mxd
			UpdateArgument(arg2, arg2label, General.Map.Config.LinedefActions[showaction].Args[2]); //mxd
			UpdateArgument(arg3, arg3label, General.Map.Config.LinedefActions[showaction].Args[3]); //mxd
			UpdateArgument(arg4, arg4label, General.Map.Config.LinedefActions[showaction].Args[4]); //mxd

			if(!preventchanges) 
			{
				MakeUndo(); //mxd
				
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
				CheckActivationFlagsRequired();
				actionhelp.UpdateAction(showaction);
			}
		}

		// Browse Action clicked
		private void browseaction_Click(object sender, EventArgs e)
		{
			action.Value = ActionBrowserForm.BrowseAction(this, action.Value);
		}

		//mxd
		private void cbArgStr_CheckedChanged(object sender, EventArgs e) 
		{
			if(!cbArgStr.Visible) return;
			scriptNames.Visible = cbArgStr.Checked;
			scriptNumbers.Visible = !cbArgStr.Checked;
			arg0label.Text = cbArgStr.Checked ? "Script Name:" : "Script Number:";
		}

		//mxd
		private void tabcustom_MouseEnter(object sender, EventArgs e) 
		{
			fieldslist.Focus();
		}

		//mxd. Store window location
		private void LinedefEditForm_FormClosing(object sender, FormClosingEventArgs e) 
		{
			location = this.Location;
			activetab = tabs.SelectedIndex;
		}

		// Help!
		private void LinedefEditForm_HelpRequested(object sender, HelpEventArgs hlpevent)
		{
			General.ShowHelp("w_linedefedit.html");
			hlpevent.Handled = true;
		}

		#endregion

		#region ================== mxd. Realtime events (linedef)

		private void cbRenderStyle_SelectedIndexChanged(object sender, EventArgs e) 
		{
			if(preventchanges) return;
			MakeUndo(); //mxd

			//update values
			foreach(Linedef l in lines)
				UDMFTools.SetString(l.Fields, "renderstyle", renderstyles[renderStyle.SelectedIndex], "translucent");

			General.Map.IsChanged = true;
			if(OnValuesChanged != null) OnValuesChanged(this, EventArgs.Empty);
		}
		
		private void alpha_WhenTextChanged(object sender, EventArgs e) 
		{
			if(preventchanges) return;
			MakeUndo(); //mxd
			int i = 0;

			//restore values
			if(string.IsNullOrEmpty(alpha.Text)) 
			{
				foreach(Linedef l in lines) 
					UDMFTools.SetFloat(l.Fields, "alpha", linedefprops[i++].Alpha, 1.0f);
			} 
			else //update values
			{
				foreach(Linedef l in lines) 
				{
					float value = General.Clamp(alpha.GetResultFloat(l.Fields.GetValue("alpha", 1.0f)), 0f, 1.0f);
					UDMFTools.SetFloat(l.Fields, "alpha", value, 1.0f);
				}
			}

			General.Map.IsChanged = true;
			if(OnValuesChanged != null)	OnValuesChanged(this, EventArgs.Empty);
		}

		private void flags_OnValueChanged(object sender, EventArgs e) 
		{
			if(preventchanges) return;
			MakeUndo(); //mxd
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
					else if(linedefprops[i].Flags.ContainsKey(c.Tag.ToString()))
						l.SetFlag(c.Tag.ToString(), linedefprops[i].Flags[c.Tag.ToString()]);
					else //linedefs created in the editor have empty Flags by default
						l.SetFlag(c.Tag.ToString(), false);
				}

				i++;
			}
			
			General.Map.IsChanged = true;
			if(OnValuesChanged != null)	OnValuesChanged(this, EventArgs.Empty);
		}

		//mxd
		private void udmfactivates_OnValueChanged(object sender, EventArgs e) 
		{
			if(preventchanges) return;
			CheckActivationFlagsRequired();
		}

		#endregion

		#region ================== mxd. Realtime events (sides)

		#region Custom fields changed

		// Custom fields on front sides
		private void customfrontbutton_Click(object sender, EventArgs e) 
		{
			// Make collection of front sides
			List<MapElement> sides = new List<MapElement>(lines.Count);
			foreach(Linedef l in lines) if(l.Front != null) sides.Add(l.Front);

			if(!CustomFieldsForm.ShowDialog(this, "Front side custom fields", "sidedef", sides, General.Map.Config.SidedefFields)) return;

			//Apply values
			Sidedef fs = General.GetByIndex(sides, 0) as Sidedef;

			//..to the first side
			foreach(PairedFieldsControl pfc in frontUdmfControls)
				pfc.SetValuesFrom(fs.Fields, true);

			lightFront.Text = UDMFTools.GetInteger(fs.Fields, "light", 0).ToString();
			cbLightAbsoluteFront.ThreeState = false;
			cbLightAbsoluteFront.Checked = fs.Fields.GetValue("lightabsolute", false);
					
			//flags
			foreach(CheckBox c in flagsFront.Checkboxes)
				if(fs.Flags.ContainsKey(c.Tag.ToString())) c.Checked = fs.Flags[c.Tag.ToString()];

			//..then to all of them
			foreach(Sidedef s in sides)
			{
				foreach(PairedFieldsControl pfc in frontUdmfControls)
					pfc.SetValuesFrom(s.Fields, false);

				if(!string.IsNullOrEmpty(lightFront.Text)) 
				{
					int light = UDMFTools.GetInteger(s.Fields, "light", 0);
					if(light != lightFront.GetResult(light)) lightFront.Text = string.Empty;
				}

				if(s.Fields.GetValue("lightabsolute", false) != cbLightAbsoluteFront.Checked) 
				{
					cbLightAbsoluteFront.ThreeState = true;
					cbLightAbsoluteFront.CheckState = CheckState.Indeterminate;
				}

				//flags
				foreach(CheckBox c in flagsFront.Checkboxes) 
				{
					if(c.CheckState == CheckState.Indeterminate) continue;

					if(s.Flags.ContainsKey(c.Tag.ToString())) 
					{
						if(s.Flags[c.Tag.ToString()] != c.Checked) 
						{
							c.ThreeState = true;
							c.CheckState = CheckState.Indeterminate;
						}
					}
				}
			}

			General.Map.IsChanged = true;
			if(OnValuesChanged != null) OnValuesChanged(this, EventArgs.Empty);
		}

		// Custom fields on back sides
		private void custombackbutton_Click(object sender, EventArgs e) 
		{
			// Make collection of back sides
			List<MapElement> sides = new List<MapElement>(lines.Count);
			foreach(Linedef l in lines) if(l.Back != null) sides.Add(l.Back);

			// Edit these
			if(!CustomFieldsForm.ShowDialog(this, "Back side custom fields", "sidedef", sides, General.Map.Config.SidedefFields)) return;

			//Apply values
			Sidedef fs = General.GetByIndex(sides, 0) as Sidedef;

			//..to the first side
			foreach(PairedFieldsControl pfc in backUdmfControls)
				pfc.SetValuesFrom(fs.Fields, true);

			lightBack.Text = UDMFTools.GetInteger(fs.Fields, "light", 0).ToString();
			cbLightAbsoluteBack.ThreeState = false;
			cbLightAbsoluteBack.Checked = fs.Fields.GetValue("lightabsolute", false);

			//flags
			foreach(CheckBox c in flagsBack.Checkboxes)
				if(fs.Flags.ContainsKey(c.Tag.ToString())) c.Checked = fs.Flags[c.Tag.ToString()];

			//..then to all of them
			foreach(Sidedef s in sides) 
			{
				foreach(PairedFieldsControl pfc in backUdmfControls)
					pfc.SetValuesFrom(s.Fields, false);

				if(!string.IsNullOrEmpty(lightBack.Text)) 
				{
					int light = UDMFTools.GetInteger(s.Fields, "light", 0);
					if(light != lightBack.GetResult(light)) lightBack.Text = string.Empty;
				}

				if(s.Fields.GetValue("lightabsolute", false) != cbLightAbsoluteBack.Checked) 
				{
					cbLightAbsoluteBack.ThreeState = true;
					cbLightAbsoluteBack.CheckState = CheckState.Indeterminate;
				}

				//flags
				foreach(CheckBox c in flagsBack.Checkboxes) 
				{
					if(c.CheckState == CheckState.Indeterminate) continue;

					if(s.Flags.ContainsKey(c.Tag.ToString()) && s.Flags[c.Tag.ToString()] != c.Checked) 
					{
						c.ThreeState = true;
						c.CheckState = CheckState.Indeterminate;
					}
				}
			}

			General.Map.IsChanged = true;
			if(OnValuesChanged != null) OnValuesChanged(this, EventArgs.Empty);
		}

			#endregion

			#region Texture changed

		private void fronthigh_OnValueChanged(object sender, EventArgs e) 
		{
			if(preventchanges) return;
			MakeUndo(); //mxd

			//restore values
			if(string.IsNullOrEmpty(fronthigh.TextureName)) 
			{
				int i = 0;

				foreach(Linedef l in lines) 
				{
					if(l.Front != null) l.Front.SetTextureHigh(linedefprops[i].Front != null ? linedefprops[i].Front.TextureTop : "-");
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
			MakeUndo(); //mxd

			//restore values
			if(string.IsNullOrEmpty(frontmid.TextureName)) 
			{
				int i = 0;

				foreach(Linedef l in lines) 
				{
					if(l.Front != null) l.Front.SetTextureMid(linedefprops[i].Front != null ? linedefprops[i].Front.TextureMid : "-");
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
			MakeUndo(); //mxd

			//restore values
			if(string.IsNullOrEmpty(frontlow.TextureName)) 
			{
				int i = 0;

				foreach(Linedef l in lines) 
				{
					if(l.Front != null) l.Front.SetTextureLow(linedefprops[i].Front != null ? linedefprops[i].Front.TextureLow : "-");
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
			MakeUndo(); //mxd

			//restore values
			if(string.IsNullOrEmpty(backhigh.TextureName)) 
			{
				int i = 0;

				foreach(Linedef l in lines) 
				{
					if(l.Back != null) l.Back.SetTextureHigh(linedefprops[i].Back != null ? linedefprops[i].Back.TextureTop : "-");
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
			MakeUndo(); //mxd

			//restore values
			if(string.IsNullOrEmpty(backmid.TextureName)) 
			{
				int i = 0;

				foreach(Linedef l in lines) 
				{
					if(l.Back != null) l.Back.SetTextureMid(linedefprops[i].Back != null ? linedefprops[i].Back.TextureMid : "-");
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
			MakeUndo(); //mxd

			//restore values
			if(string.IsNullOrEmpty(backlow.TextureName)) 
			{
				int i = 0;

				foreach(Linedef l in lines) 
				{
					if(l.Back != null) l.Back.SetTextureLow(linedefprops[i].Back != null ? linedefprops[i].Back.TextureLow : "-");
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

			#endregion

			#region Brightness changed

		private void lightFront_WhenTextChanged(object sender, EventArgs e) 
		{
			if(preventchanges) return;
			MakeUndo(); //mxd
			int i = 0;

			//restore values
			if(string.IsNullOrEmpty(lightFront.Text)) 
			{
				foreach(Linedef l in lines) 
				{
					if(l.Front != null)
						UDMFTools.SetInteger(l.Front.Fields, "light", (linedefprops[i].Front != null ? linedefprops[i].Front.Brightness : 0), 0);
					i++;
				}
			} 
			else //update values
			{
				foreach(Linedef l in lines) 
				{
					if(l.Front != null) 
					{
						bool absolute = false;
						switch (cbLightAbsoluteFront.CheckState)
						{
							case CheckState.Indeterminate:
								absolute = l.Front.Fields.GetValue("lightabsolute", false);
								break;
							case CheckState.Checked:
								absolute = true;
								break;
						}

						int value = General.Clamp(lightFront.GetResult((linedefprops[i].Front != null ? linedefprops[i].Front.Brightness : 0)), (absolute ? 0 : -255), 255);
						UDMFTools.SetInteger(l.Front.Fields, "light", value, 0);
					}
					i++;
				}
			}

			General.Map.IsChanged = true;
			if(OnValuesChanged != null) OnValuesChanged(this, EventArgs.Empty);
		}

		private void lightBack_WhenTextChanged(object sender, EventArgs e) 
		{
			if(preventchanges) return;
			MakeUndo(); //mxd
			int i = 0;

			//restore values
			if(string.IsNullOrEmpty(lightBack.Text)) 
			{
				foreach(Linedef l in lines) 
				{
					if(l.Back != null)
						UDMFTools.SetInteger(l.Back.Fields, "light", (linedefprops[i].Back != null ? linedefprops[i].Back.Brightness : 0), 0);
					i++;
				}
			} 
			else //update values
			{
				foreach(Linedef l in lines) 
				{
					if(l.Back != null) 
					{
						bool absolute = false;
						switch (cbLightAbsoluteBack.CheckState)
						{
							case CheckState.Indeterminate:
								absolute = l.Back.Fields.GetValue("lightabsolute", false);
								break;
							case CheckState.Checked:
								absolute = true;
								break;
						}

						int value = General.Clamp(lightBack.GetResult((linedefprops[i].Back != null ? linedefprops[i].Back.Brightness : 0)), (absolute ? 0 : -255), 255);
						UDMFTools.SetInteger(l.Back.Fields, "light", value, 0);
					}
					i++;
				}
			}

			General.Map.IsChanged = true;
			if(OnValuesChanged != null) OnValuesChanged(this, EventArgs.Empty);
		}

		private void cbLightAbsoluteFront_CheckedChanged(object sender, EventArgs e) 
		{
			if(preventchanges) return;
			MakeUndo(); //mxd

			if(cbLightAbsoluteFront.Checked) 
			{
				foreach(Linedef l in lines) 
				{
					if(l.Front == null) continue;
					l.Front.Fields["lightabsolute"] = new UniValue(UniversalType.Boolean, true);
				}
			} 
			else if(cbLightAbsoluteFront.CheckState == CheckState.Indeterminate) 
			{
				int i = 0;

				foreach(Linedef l in lines) 
				{
					if(l.Front != null) 
					{
						if(linedefprops[i].Front != null && linedefprops[i].Front.AbsoluteBrightness) 
						{
							l.Front.Fields["lightabsolute"] = new UniValue(UniversalType.Boolean, true);
						} 
						else if(l.Front.Fields.ContainsKey("lightabsolute")) 
						{
							l.Front.Fields.Remove("lightabsolute");
						}
					}
					i++;
				}
			} 
			else 
			{
				foreach(Linedef l in lines) 
				{
					if(l.Front == null) continue;
					if(l.Front.Fields.ContainsKey("lightabsolute")) l.Front.Fields.Remove("lightabsolute");
				}
			}

			General.Map.IsChanged = true;
			if(OnValuesChanged != null) OnValuesChanged(this, EventArgs.Empty);
		}

		private void cbLightAbsoluteBack_CheckedChanged(object sender, EventArgs e) 
		{
			if(preventchanges) return;
			MakeUndo(); //mxd

			if(cbLightAbsoluteBack.Checked) 
			{
				foreach(Linedef l in lines) 
				{
					if(l.Back == null) continue;
					l.Back.Fields["lightabsolute"] = new UniValue(UniversalType.Boolean, true);
				}
			} 
			else if(cbLightAbsoluteBack.CheckState == CheckState.Indeterminate) 
			{
				int i = 0;
				
				foreach(Linedef l in lines) 
				{
					if(l.Back != null) 
					{
						if(linedefprops[i].Back != null && linedefprops[i].Back.AbsoluteBrightness) 
						{
							l.Back.Fields["lightabsolute"] = new UniValue(UniversalType.Boolean, true);
						} 
						else if(l.Back.Fields.ContainsKey("lightabsolute")) 
						{
							l.Back.Fields.Remove("lightabsolute");
						}
					}
					i++;
				}
			} 
			else 
			{
				foreach(Linedef l in lines) 
				{
					if(l.Back == null) continue;
					if(l.Back.Fields.ContainsKey("lightabsolute")) l.Back.Fields.Remove("lightabsolute");
				}
			}

			General.Map.IsChanged = true;
			if(OnValuesChanged != null) OnValuesChanged(this, EventArgs.Empty);
		}

			#endregion

			#region Global texture offsets changed

		private void frontTextureOffset_OnValuesChanged(object sender, EventArgs e)
		{
			if(preventchanges) return;
			MakeUndo(); //mxd
			int i = 0;

			foreach(Linedef l in lines) 
			{
				if(l.Front != null) 
				{
					if(linedefprops[i].Front != null) 
					{
						l.Front.OffsetX = frontTextureOffset.GetValue1(linedefprops[i].Front.OffsetX);
						l.Front.OffsetY = frontTextureOffset.GetValue2(linedefprops[i].Front.OffsetY);
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
			labelFrontTextureOffset.Enabled = frontTextureOffset.NonDefaultValue;
			if(OnValuesChanged != null) OnValuesChanged(this, EventArgs.Empty);
		}

		private void backTextureOffset_OnValuesChanged(object sender, EventArgs e) 
		{
			if(preventchanges) return;
			MakeUndo(); //mxd
			int i = 0;

			foreach(Linedef l in lines) 
			{
				if(l.Back != null) 
				{
					if(linedefprops[i].Back != null) 
					{
						l.Back.OffsetX = backTextureOffset.GetValue1(linedefprops[i].Back.OffsetX);
						l.Back.OffsetY = backTextureOffset.GetValue2(linedefprops[i].Back.OffsetY);
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
			labelBackTextureOffset.Enabled = backTextureOffset.NonDefaultValue;
			if(OnValuesChanged != null) OnValuesChanged(this, EventArgs.Empty);
		}

			#endregion

			#region Texture offsets changed

		private void pfcFrontOffsetTop_OnValuesChanged(object sender, EventArgs e)
		{
			if(preventchanges) return;
			MakeUndo(); //mxd
			int i = 0;

			foreach(Linedef l in lines) 
			{
				if(l.Front != null) 
				{
					float oldX = linedefprops[i].Front != null ? linedefprops[i].Front.OffsetTopX : 0f;
					float oldY = linedefprops[i].Front != null ? linedefprops[i].Front.OffsetTopY : 0f;
					pfcFrontOffsetTop.ApplyTo(l.Front.Fields, General.Map.FormatInterface.MinTextureOffset, General.Map.FormatInterface.MaxTextureOffset, oldX, oldY);
				}
				i++;
			}

			General.Map.IsChanged = true;
			labelFrontOffsetTop.Enabled = pfcFrontOffsetTop.NonDefaultValue;
			if(OnValuesChanged != null) OnValuesChanged(this, EventArgs.Empty);
		}

		private void pfcFrontOffsetMid_OnValuesChanged(object sender, EventArgs e)
		{
			if(preventchanges) return;
			MakeUndo(); //mxd
			int i = 0;

			foreach(Linedef l in lines) 
			{
				if(l.Front != null) 
				{
					float oldX = linedefprops[i].Front != null ? linedefprops[i].Front.OffsetMidX : 0f;
					float oldY = linedefprops[i].Front != null ? linedefprops[i].Front.OffsetMidY : 0f;
					pfcFrontOffsetMid.ApplyTo(l.Front.Fields, General.Map.FormatInterface.MinTextureOffset, General.Map.FormatInterface.MaxTextureOffset, oldX, oldY);
				}

				i++;
			}

			General.Map.IsChanged = true;
			labelFrontOffsetMid.Enabled = pfcFrontOffsetMid.NonDefaultValue;
			if(OnValuesChanged != null) OnValuesChanged(this, EventArgs.Empty);
		}

		private void pfcFrontOffsetBottom_OnValuesChanged(object sender, EventArgs e)
		{
			if(preventchanges) return;
			MakeUndo(); //mxd
			int i = 0;

			foreach(Linedef l in lines) 
			{
				if(l.Front != null) 
				{
					float oldX = linedefprops[i].Front != null ? linedefprops[i].Front.OffsetBottomX : 0f;
					float oldY = linedefprops[i].Front != null ? linedefprops[i].Front.OffsetBottomY : 0f;
					pfcFrontOffsetBottom.ApplyTo(l.Front.Fields, General.Map.FormatInterface.MinTextureOffset, General.Map.FormatInterface.MaxTextureOffset, oldX, oldY);
				}

				i++;
			}

			General.Map.IsChanged = true;
			labelFrontOffsetBottom.Enabled = pfcFrontOffsetBottom.NonDefaultValue;
			if(OnValuesChanged != null) OnValuesChanged(this, EventArgs.Empty);
		}

		private void pfcBackOffsetTop_OnValuesChanged(object sender, EventArgs e) 
		{
			if(preventchanges) return;
			MakeUndo(); //mxd
			int i = 0;

			foreach(Linedef l in lines) 
			{
				if(l.Back != null) 
				{
					float oldX = linedefprops[i].Back != null ? linedefprops[i].Back.OffsetTopX : 0f;
					float oldY = linedefprops[i].Back != null ? linedefprops[i].Back.OffsetTopY : 0f;
					pfcBackOffsetTop.ApplyTo(l.Back.Fields, General.Map.FormatInterface.MinTextureOffset, General.Map.FormatInterface.MaxTextureOffset, oldX, oldY);
				}

				i++;
			}

			General.Map.IsChanged = true;
			labelBackOffsetTop.Enabled = pfcBackOffsetTop.NonDefaultValue;
			if(OnValuesChanged != null) OnValuesChanged(this, EventArgs.Empty);
		}

		private void pfcBackOffsetMid_OnValuesChanged(object sender, EventArgs e) 
		{
			if(preventchanges) return;
			MakeUndo(); //mxd
			int i = 0;

			foreach(Linedef l in lines) 
			{
				if(l.Back != null) 
				{
					float oldX = linedefprops[i].Back != null ? linedefprops[i].Back.OffsetMidX : 0f;
					float oldY = linedefprops[i].Back != null ? linedefprops[i].Back.OffsetMidY : 0f;
					pfcBackOffsetMid.ApplyTo(l.Back.Fields, General.Map.FormatInterface.MinTextureOffset, General.Map.FormatInterface.MaxTextureOffset, oldX, oldY);
				}

				i++;
			}

			General.Map.IsChanged = true;
			labelBackOffsetMid.Enabled = pfcBackOffsetMid.NonDefaultValue;
			if(OnValuesChanged != null) OnValuesChanged(this, EventArgs.Empty);
		}

		private void pfcBackOffsetBottom_OnValuesChanged(object sender, EventArgs e) 
		{
			if(preventchanges) return;
			MakeUndo(); //mxd
			int i = 0;

			foreach(Linedef l in lines) 
			{
				if(l.Back != null) 
				{
					float oldX = linedefprops[i].Back != null ? linedefprops[i].Back.OffsetBottomX : 0f;
					float oldY = linedefprops[i].Back != null ? linedefprops[i].Back.OffsetBottomY : 0f;
					pfcBackOffsetBottom.ApplyTo(l.Back.Fields, General.Map.FormatInterface.MinTextureOffset, General.Map.FormatInterface.MaxTextureOffset, oldX, oldY);
				}

				i++;
			}

			General.Map.IsChanged = true;
			labelBackOffsetBottom.Enabled = pfcBackOffsetBottom.NonDefaultValue;
			if(OnValuesChanged != null) OnValuesChanged(this, EventArgs.Empty);
		}

			#endregion

			#region Scale changed

		private void pfcFrontScaleTop_OnValuesChanged(object sender, EventArgs e) 
		{
			if(preventchanges) return;
			MakeUndo(); //mxd
			int i = 0;

			foreach(Linedef l in lines) 
			{
				if(l.Front != null) 
				{
					float oldX = linedefprops[i].Front != null ? linedefprops[i].Front.ScaleTopX : 1.0f;
					float oldY = linedefprops[i].Front != null ? linedefprops[i].Front.ScaleTopY : 1.0f;
					pfcFrontScaleTop.ApplyTo(l.Front.Fields, General.Map.FormatInterface.MinTextureOffset, General.Map.FormatInterface.MaxTextureOffset, oldX, oldY);
				}

				i++;
			}

			General.Map.IsChanged = true;
			labelFrontScaleTop.Enabled = pfcFrontScaleTop.NonDefaultValue;
			if(OnValuesChanged != null) OnValuesChanged(this, EventArgs.Empty);
		}

		private void pfcFrontScaleMid_OnValuesChanged(object sender, EventArgs e) 
		{
			if(preventchanges) return;
			MakeUndo(); //mxd
			int i = 0;

			foreach(Linedef l in lines) 
			{
				if(l.Front != null) 
				{
					float oldX = linedefprops[i].Front != null ? linedefprops[i].Front.ScaleMidX : 1.0f;
					float oldY = linedefprops[i].Front != null ? linedefprops[i].Front.ScaleMidY : 1.0f;
					pfcFrontScaleMid.ApplyTo(l.Front.Fields, General.Map.FormatInterface.MinTextureOffset, General.Map.FormatInterface.MaxTextureOffset, oldX, oldY);
				}

				i++;
			}

			General.Map.IsChanged = true;
			labelFrontScaleMid.Enabled = pfcFrontScaleMid.NonDefaultValue;
			if(OnValuesChanged != null) OnValuesChanged(this, EventArgs.Empty);
		}

		private void pfcFrontScaleBottom_OnValuesChanged(object sender, EventArgs e) 
		{
			if(preventchanges) return;
			MakeUndo(); //mxd
			int i = 0;

			foreach(Linedef l in lines) 
			{
				if(l.Front != null) 
				{
					float oldX = linedefprops[i].Front != null ? linedefprops[i].Front.ScaleBottomX : 1.0f;
					float oldY = linedefprops[i].Front != null ? linedefprops[i].Front.ScaleBottomY : 1.0f;
					pfcFrontScaleBottom.ApplyTo(l.Front.Fields, General.Map.FormatInterface.MinTextureOffset, General.Map.FormatInterface.MaxTextureOffset, oldX, oldY);
				}

				i++;
			}

			General.Map.IsChanged = true;
			labelFrontScaleBottom.Enabled = pfcFrontScaleBottom.NonDefaultValue;
			if(OnValuesChanged != null) OnValuesChanged(this, EventArgs.Empty);
		}

		private void pfcBackScaleTop_OnValuesChanged(object sender, EventArgs e) 
		{
			if(preventchanges) return;
			MakeUndo(); //mxd
			int i = 0;

			foreach(Linedef l in lines) 
			{
				if(l.Back != null) 
				{
					float oldX = linedefprops[i].Back != null ? linedefprops[i].Back.ScaleTopX : 1.0f;
					float oldY = linedefprops[i].Back != null ? linedefprops[i].Back.ScaleTopY : 1.0f;
					pfcBackScaleTop.ApplyTo(l.Back.Fields, General.Map.FormatInterface.MinTextureOffset, General.Map.FormatInterface.MaxTextureOffset, oldX, oldY);
				}

				i++;
			}

			General.Map.IsChanged = true;
			labelBackScaleTop.Enabled = pfcBackScaleTop.NonDefaultValue;
			if(OnValuesChanged != null) OnValuesChanged(this, EventArgs.Empty);
		}

		private void pfcBackScaleMid_OnValuesChanged(object sender, EventArgs e) 
		{
			if(preventchanges) return;
			MakeUndo(); //mxd
			int i = 0;

			foreach(Linedef l in lines) 
			{
				if(l.Back != null) 
				{
					float oldX = linedefprops[i].Back != null ? linedefprops[i].Back.ScaleMidX : 1.0f;
					float oldY = linedefprops[i].Back != null ? linedefprops[i].Back.ScaleMidY : 1.0f;
					pfcBackScaleMid.ApplyTo(l.Back.Fields, General.Map.FormatInterface.MinTextureOffset, General.Map.FormatInterface.MaxTextureOffset, oldX, oldY);
				}

				i++;
			}

			General.Map.IsChanged = true;
			labelBackScaleMid.Enabled = pfcBackScaleMid.NonDefaultValue;
			if(OnValuesChanged != null) OnValuesChanged(this, EventArgs.Empty);
		}

		private void pfcBackScaleBottom_OnValuesChanged(object sender, EventArgs e) 
		{
			if(preventchanges) return;
			MakeUndo(); //mxd
			int i = 0;

			foreach(Linedef l in lines) 
			{
				if(l.Back != null) 
				{
					float oldX = linedefprops[i].Back != null ? linedefprops[i].Back.ScaleBottomX : 1.0f;
					float oldY = linedefprops[i].Back != null ? linedefprops[i].Back.ScaleBottomY : 1.0f;
					pfcBackScaleBottom.ApplyTo(l.Back.Fields, General.Map.FormatInterface.MinTextureOffset, General.Map.FormatInterface.MaxTextureOffset, oldX, oldY);
				}

				i++;
			}

			General.Map.IsChanged = true;
			labelBackScaleBottom.Enabled = pfcBackScaleBottom.NonDefaultValue;
			if(OnValuesChanged != null) OnValuesChanged(this, EventArgs.Empty);
		}

			#endregion

			#region Flags cahnged

		private void flagsFront_OnValueChanged(object sender, EventArgs e) 
		{
			if(preventchanges) return;
			MakeUndo(); //mxd
			int i = 0;

			foreach(Linedef l in lines) 
			{
				if(l.Front == null) continue;
				
				// Apply all flags
				foreach(CheckBox c in flagsFront.Checkboxes) 
				{
					if(c.CheckState == CheckState.Checked)
						l.Front.SetFlag(c.Tag.ToString(), true);
					else if(c.CheckState == CheckState.Unchecked)
						l.Front.SetFlag(c.Tag.ToString(), false);
					else if(linedefprops[i].Front.Flags.ContainsKey(c.Tag.ToString()))
						l.Front.SetFlag(c.Tag.ToString(), linedefprops[i].Front.Flags[c.Tag.ToString()]);
					else //linedefs created in the editor have empty Flags by default
						l.Front.SetFlag(c.Tag.ToString(), false);
				}

				i++;
			}

			General.Map.IsChanged = true;
			if(OnValuesChanged != null) OnValuesChanged(this, EventArgs.Empty);
		}

		private void flagsBack_OnValueChanged(object sender, EventArgs e) 
		{
			if(preventchanges) return;
			MakeUndo(); //mxd
			int i = 0;

			foreach(Linedef l in lines) 
			{
				if(l.Back == null) continue;

				// Apply all flags
				foreach(CheckBox c in flagsBack.Checkboxes) 
				{
					if(c.CheckState == CheckState.Checked)
						l.Back.SetFlag(c.Tag.ToString(), true);
					else if(c.CheckState == CheckState.Unchecked)
						l.Back.SetFlag(c.Tag.ToString(), false);
					else if(linedefprops[i].Back.Flags.ContainsKey(c.Tag.ToString()))
						l.Back.SetFlag(c.Tag.ToString(), linedefprops[i].Back.Flags[c.Tag.ToString()]);
					else //linedefs created in the editor have empty Flags by default
						l.Back.SetFlag(c.Tag.ToString(), false);
				}

				i++;
			}

			General.Map.IsChanged = true;
			if(OnValuesChanged != null) OnValuesChanged(this, EventArgs.Empty);
		}

			#endregion

		#endregion

	}
}
