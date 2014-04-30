
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
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Data;
using CodeImp.DoomBuilder.Config;
using CodeImp.DoomBuilder.Geometry;
using CodeImp.DoomBuilder.GZBuilder.Data;
using CodeImp.DoomBuilder.GZBuilder.Tools;
using CodeImp.DoomBuilder.Types;

#endregion

namespace CodeImp.DoomBuilder.Windows
{
	/// <summary>
	/// Dialog window that allows viewing and editing of Thing properties.
	/// </summary>
	internal partial class ThingEditFormUDMF : DelayedForm
	{
		#region ================== Events

		public event EventHandler OnValuesChanged; //mxd

		#endregion

		#region ================== Variables

		private ICollection<Thing> things;
		private ThingTypeInfo thinginfo;
		private bool preventchanges;
		private Vector3D initialPosition; //initial position of a thing used to fill posX, posY and posZ fields
		private int initialFloorHeight; //floor height of the sector first thing is in
		private static bool useAbsoluteHeight;
		private string arg0str;
		private bool haveArg0Str;

		private List<ThingProperties> thingProps; //mxd
		//TODO: move this into game configuration!
		private readonly List<string> renderStyles = new List<string>() { "normal", "translucent", "soultrans", "translucentstencil", "add", "subtract", "stencil", "fuzzy", "optfuzzy", "none" };

		//mxd. Window setup stuff
		private static Point location = Point.Empty;
		private static int activeTab;

		private struct ThingProperties //mxd
		{
			public readonly int Type;
			public readonly int AngleDoom;
			public readonly int Pitch;
			public readonly int Roll;
			public readonly float ScaleX;
			public readonly float ScaleY;
			public readonly float X;
			public readonly float Y;
			public readonly float Z;

			public ThingProperties(Thing t) {
				X = t.Position.x;
				Y = t.Position.y;
				Z = t.Position.z;
				Type = t.Type;
				AngleDoom = t.AngleDoom;
				Pitch = t.Pitch;
				Roll = t.Roll;
				ScaleX = t.ScaleX;
				ScaleY = t.ScaleY;
			}
		}

		#endregion

		#region ================== Constructor

		// Constructor
		public ThingEditFormUDMF() {
			// Initialize
			InitializeComponent();

			//mxd. Widow setup
			if(location != Point.Empty) {
				this.StartPosition = FormStartPosition.Manual;
				this.Location = location;
				if(activeTab > 0 && activeTab < tabs.TabCount) {
					tabs.SelectTab(activeTab);
				} else {
					activeTab = 0;
				}
			}

			// Fill flags list
			foreach(KeyValuePair<string, string> tf in General.Map.Config.ThingFlags)
				flags.Add(tf.Value, tf.Key);

			// Fill actions list
			action.GeneralizedCategories = General.Map.Config.GenActionCategories;
			action.AddInfo(General.Map.Config.SortedLinedefActions.ToArray());

			if (General.Map.FormatInterface.HasCustomFields) {
				// Initialize custom fields editor
				fieldslist.Setup("thing");

				// Fill universal fields list
				fieldslist.ListFixedFields(General.Map.Config.ThingFields);
			}

			// Tag/Effects
			scriptNames.Location = arg0.Location;
			scriptNumbers.Location = arg0.Location;
			cbArgStr.Visible = General.Map.FormatInterface.HasCustomFields;

			foreach(ScriptItem nsi in General.Map.NamedScripts)
				scriptNames.Items.Add(nsi);

			foreach(ScriptItem si in General.Map.NumberedScripts)
				scriptNumbers.Items.Add(si);

			scriptNames.DropDownWidth = Tools.GetDropDownWidth(scriptNames);
			scriptNumbers.DropDownWidth = Tools.GetDropDownWidth(scriptNumbers);

			// Thing height?
			posZ.Visible = General.Map.FormatInterface.HasThingHeight;
			zlabel.Visible = General.Map.FormatInterface.HasThingHeight;
			cbAbsoluteHeight.Visible = General.Map.FormatInterface.HasThingHeight; //mxd

			//mxd. Decimals allowed?
			if(General.Map.FormatInterface.VertexDecimals > 0) {
				posX.AllowDecimal = true;
				posY.AllowDecimal = true;
				posZ.AllowDecimal = true;
			}

			// Setup types list
			thingtype.Setup();
		}

		// This sets up the form to edit the given things
		public void Setup(ICollection<Thing> things) {
			preventchanges = true;

			// Keep this list
			this.things = things;
			if(things.Count > 1) this.Text = "Edit Things (" + things.Count + ")";
			hint.Visible = things.Count > 1; //mxd
			hintlabel.Visible = things.Count > 1; //mxd

			//mxd. Make undo
			string undodesc = "thing";
			if(things.Count > 1) undodesc = things.Count + " things";
			General.Map.UndoRedo.CreateUndo("Edit " + undodesc);

			////////////////////////////////////////////////////////////////////////
			// Set all options to the first thing properties
			////////////////////////////////////////////////////////////////////////

			Thing ft = General.GetByIndex(things, 0);

			// Set type
			thingtype.SelectType(ft.Type);

			// Flags
			foreach (CheckBox c in flags.Checkboxes) {
				if (ft.Flags.ContainsKey(c.Tag.ToString())) 
					c.Checked = ft.Flags[c.Tag.ToString()];
			}

			// Coordination
			angle.Text = ft.AngleDoom.ToString();
			zlabel.Text = useAbsoluteHeight ? "Abs. Z:" : "Z:"; //mxd
			cbAbsoluteHeight.Checked = useAbsoluteHeight; //mxd

			//mxd
			initialPosition = ft.Position;
			if(ft.Sector != null) initialFloorHeight = ft.Sector.FloorHeight;
			posX.Text = ((int)ft.Position.x).ToString();
			posY.Text = ((int)ft.Position.y).ToString();
			posZ.Text = useAbsoluteHeight ? ((int)ft.Position.z + initialFloorHeight).ToString() : ((int)ft.Position.z).ToString();
			posX.ButtonStep = General.Map.Grid.GridSize;
			posY.ButtonStep = General.Map.Grid.GridSize;
			posZ.ButtonStep = General.Map.Grid.GridSize;

			// Custom fields
			if (General.Map.FormatInterface.HasCustomFields) {
				fieldslist.SetValues(ft.Fields, true);
				conversationID.Text = ft.Fields.GetValue("conversation", 0).ToString();
				gravity.Text = ft.Fields.GetValue("gravity", 1.0f).ToString();
				score.Text = ft.Fields.GetValue("score", 0).ToString();
				health.Text = ft.Fields.GetValue("health", 1).ToString();
				alpha.Text = ft.Fields.GetValue("alpha", 1.0f).ToString();
				color.SetValueFrom(ft.Fields);
				arg0str = ft.Fields.GetValue("arg0str", string.Empty);
				haveArg0Str = !string.IsNullOrEmpty(arg0str);
				scale.SetValues(ft.ScaleX, ft.ScaleY, true);
				pitch.Text = ft.Pitch.ToString();
				roll.Text = ft.Roll.ToString();
				int renderstyle = renderStyles.IndexOf(ft.Fields.GetValue("renderstyle", string.Empty));
				renderStyle.SelectedIndex = (renderstyle == -1 ? 0 : renderstyle);
			}

			// Action/tags
			action.Value = ft.Action;
			tagSelector.Setup(UniversalType.ThingTag);
			tagSelector.SetTag(ft.Tag);
			arg0.SetValue(ft.Args[0]);
			arg1.SetValue(ft.Args[1]);
			arg2.SetValue(ft.Args[2]);
			arg3.SetValue(ft.Args[3]);
			arg4.SetValue(ft.Args[4]);

			////////////////////////////////////////////////////////////////////////
			// Now go for all lines and change the options when a setting is different
			////////////////////////////////////////////////////////////////////////

			thingProps = new List<ThingProperties>();

			// Go for all things
			foreach(Thing t in things) {
				// Type does not match?
				ThingTypeInfo info = thingtype.GetSelectedInfo(); //mxd
				if(info != null && info.Index != t.Type) thingtype.ClearSelectedType();

				// Flags
				foreach(CheckBox c in flags.Checkboxes) {
					if(t.Flags.ContainsKey(c.Tag.ToString())) {
						if(t.Flags[c.Tag.ToString()] != c.Checked) {
							c.ThreeState = true;
							c.CheckState = CheckState.Indeterminate;
						}
					}
				}

				// Coordination
				if(t.AngleDoom.ToString() != angle.Text) angle.Text = "";

				//mxd
				if(useAbsoluteHeight && t.Sector != null) {
					if(((int)t.Position.z + t.Sector.FloorHeight).ToString() != posZ.Text) posZ.Text = "";
				} else if(((int)t.Position.z).ToString() != posZ.Text) {
					posZ.Text = "";
				}

				// Action/tags
				if(t.Action != action.Value) action.Empty = true;
				if(t.Tag != ft.Tag) tagSelector.ClearTag(); //mxd
				if(t.Args[0] != arg0.GetResult(-1)) arg0.ClearValue();
				if(t.Args[1] != arg1.GetResult(-1)) arg1.ClearValue();
				if(t.Args[2] != arg2.GetResult(-1)) arg2.ClearValue();
				if(t.Args[3] != arg3.GetResult(-1)) arg3.ClearValue();
				if(t.Args[4] != arg4.GetResult(-1)) arg4.ClearValue();

				//mxd. Custom fields
				if (General.Map.FormatInterface.HasCustomFields) {
					t.Fields.BeforeFieldsChange(); //mxd
					fieldslist.SetValues(t.Fields, false);

					if (t.Fields.GetValue("conversation", 0).ToString() != conversationID.Text)
						conversationID.Text = "";

					if (t.Fields.GetValue("gravity", 1.0f).ToString() != gravity.Text)
						gravity.Text = "";

					if (t.Fields.GetValue("score", 0).ToString() != score.Text)
						score.Text = "";

					if (t.Fields.GetValue("health", 1).ToString() != health.Text)
						health.Text = "";

					if (t.Fields.GetValue("alpha", 1.0f).ToString() != alpha.Text)
						alpha.Text = "";

					scale.SetValues(t.ScaleX, t.ScaleY, false);
					color.SetValueFrom(t.Fields);

					if (t.Pitch.ToString() != pitch.Text) pitch.Text = "";
					if (t.Roll.ToString() != roll.Text) roll.Text = "";

					int i = Math.Max(0, renderStyles.IndexOf(t.Fields.GetValue("renderstyle", string.Empty)));
					if (renderStyle.SelectedIndex != -1 && i != renderStyle.SelectedIndex)
						renderStyle.SelectedIndex = -1;

					if (arg0str != t.Fields.GetValue("arg0str", string.Empty)) {
						haveArg0Str = true;
						arg0str = string.Empty;
					}
				}

				//mxd. Store initial properties
				thingProps.Add(new ThingProperties(t));

				//mxd. add user vars
				/*if(info != null && info.Actor != null && info.Actor.UserVars.Count > 0) {
					foreach(string s in info.Actor.UserVars) {
						if(!t.Fields.ContainsKey(s))
							fieldslist.SetValue(s, 0, CodeImp.DoomBuilder.Types.UniversalType.Integer);
					}
				}*/
			}

			preventchanges = false;

			updateScriptControls(); //mxd

			//mxd. Set intial script-related values, if required
			if(Array.IndexOf(GZBuilder.GZGeneral.ACS_SPECIALS, action.Value) != -1) {
				if(haveArg0Str) {
					scriptNames.Text = arg0str;
					arg0label.Text = "Script Name:";
				} else {
					int a0 = arg0.GetResult(0);
					if(a0 > 0) {
						for(int i = 0; i < General.Map.NumberedScripts.Count; i++) {
							if(General.Map.NumberedScripts[i].Index == a0) {
								scriptNumbers.SelectedIndex = i;
								break;
							}
						}

						if(scriptNumbers.SelectedIndex == -1) {
							scriptNumbers.Items.Add(new ScriptItem(a0, "Script " + a0));
							scriptNumbers.SelectedIndex = scriptNumbers.Items.Count - 1;
						}
					} else {
						scriptNumbers.Text = arg0.Text;
					}
				}
			} else {
				scriptNumbers.Text = "0";
			}
		}

		//mxd
		private void updateScriptControls() {
			if(Array.IndexOf(GZBuilder.GZGeneral.ACS_SPECIALS, action.Value) != -1) {
				bool showNamedScripts = haveArg0Str;
				cbArgStr.Visible = true;
				cbArgStr.Checked = showNamedScripts;
				scriptNames.Visible = showNamedScripts;
				scriptNumbers.Visible = !showNamedScripts;
			} else {
				cbArgStr.Visible = false;
				scriptNames.Visible = false;
				scriptNumbers.Visible = false;
				cbArgStr.Checked = false;
			}
		}

		#endregion

		#region ================== Events

		//mxd
		private void thingtype_OnTypeDoubleClicked() {
			apply_Click(this, EventArgs.Empty);
		}

		// Action changes
		private void action_ValueChanges(object sender, EventArgs e) {
			int showaction = 0;
			ArgumentInfo[] arginfo;

			// Only when line type is known, otherwise use the thing arguments
			if(General.Map.Config.LinedefActions.ContainsKey(action.Value)) showaction = action.Value;
			if((showaction == 0) && (thinginfo != null)) arginfo = thinginfo.Args;
			else arginfo = General.Map.Config.LinedefActions[showaction].Args;

			// Change the argument descriptions
			arg0label.Text = arginfo[0].Title + ":";
			arg1label.Text = arginfo[1].Title + ":";
			arg2label.Text = arginfo[2].Title + ":";
			arg3label.Text = arginfo[3].Title + ":";
			arg4label.Text = arginfo[4].Title + ":";
			arg0label.Enabled = arginfo[0].Used;
			arg1label.Enabled = arginfo[1].Used;
			arg2label.Enabled = arginfo[2].Used;
			arg3label.Enabled = arginfo[3].Used;
			arg4label.Enabled = arginfo[4].Used;
			arg0.ForeColor = (arg0label.Enabled ? SystemColors.WindowText : SystemColors.GrayText);
			arg1.ForeColor = (arg1label.Enabled ? SystemColors.WindowText : SystemColors.GrayText);
			arg2.ForeColor = (arg2label.Enabled ? SystemColors.WindowText : SystemColors.GrayText);
			arg3.ForeColor = (arg3label.Enabled ? SystemColors.WindowText : SystemColors.GrayText);
			arg4.ForeColor = (arg4label.Enabled ? SystemColors.WindowText : SystemColors.GrayText);
			arg0.Setup(arginfo[0]);
			arg1.Setup(arginfo[1]);
			arg2.Setup(arginfo[2]);
			arg3.Setup(arginfo[3]);
			arg4.Setup(arginfo[4]);

			// Zero all arguments when linedef action 0 (normal) is chosen
			if(!preventchanges && (showaction == 0)) {
				//mxd
				arg0.SetDefaultValue();
				arg1.SetDefaultValue();
				arg2.SetDefaultValue();
				arg3.SetDefaultValue();
				arg4.SetDefaultValue();
			}

			if(!preventchanges) updateScriptControls(); //mxd
		}

		// Browse Action clicked
		private void browseaction_Click(object sender, EventArgs e) {
			action.Value = ActionBrowserForm.BrowseAction(this, action.Value);
		}

		// Angle text changes
		private void angle_TextChanged(object sender, EventArgs e) {
			anglecontrol.Angle = angle.GetResult(int.MinValue);
			updateAngle(); //mxd
		}

		//mxd. Angle control clicked
		private void anglecontrol_AngleChanged() {
			angle.Text = anglecontrol.Angle.ToString();
			updateAngle();
		}

		private void pitch_WhenTextChanged(object sender, EventArgs e) {
			pitchControl.Angle = pitch.GetResult(int.MinValue);
			updatePitch();
		}

		private void pitchControl_AngleChanged() {
			pitch.Text = pitchControl.Angle.ToString();
			updatePitch();
		}

		private void roll_WhenTextChanged(object sender, EventArgs e) {
			rollControl.Angle = roll.GetResult(int.MinValue);
			updateRoll();
		}

		private void rollControl_AngleChanged() {
			roll.Text = rollControl.Angle.ToString();
			updateRoll();
		}

		// Apply clicked
		private void apply_Click(object sender, EventArgs e) {
			List<string> defaultflags = new List<string>();

			// Verify the tag
			if(General.Map.FormatInterface.HasThingTag) //mxd
			{
				tagSelector.ValidateTag();//mxd
				if(((tagSelector.GetTag(0) < General.Map.FormatInterface.MinTag) || (tagSelector.GetTag(0) > General.Map.FormatInterface.MaxTag))) {
					General.ShowWarningMessage("Thing tag must be between " + General.Map.FormatInterface.MinTag + " and " + General.Map.FormatInterface.MaxTag + ".", MessageBoxButtons.OK);
					return;
				}
			}

			// Verify the type
			if(((thingtype.GetResult(0) < General.Map.FormatInterface.MinThingType) || (thingtype.GetResult(0) > General.Map.FormatInterface.MaxThingType))) {
				General.ShowWarningMessage("Thing type must be between " + General.Map.FormatInterface.MinThingType + " and " + General.Map.FormatInterface.MaxThingType + ".", MessageBoxButtons.OK);
				return;
			}

			// Verify the action
			if(General.Map.FormatInterface.HasThingAction && ((action.Value < General.Map.FormatInterface.MinAction) || (action.Value > General.Map.FormatInterface.MaxAction))) {
				General.ShowWarningMessage("Thing action must be between " + General.Map.FormatInterface.MinAction + " and " + General.Map.FormatInterface.MaxAction + ".", MessageBoxButtons.OK);
				return;
			}

			bool hasAcs = !action.Empty && Array.IndexOf(GZBuilder.GZGeneral.ACS_SPECIALS, action.Value) != -1; //mxd

			// Go for all the things
			foreach(Thing t in things) {
				// Coordination
				//mxd. Check position
				float px = General.Clamp(t.Position.x, General.Map.Config.LeftBoundary, General.Map.Config.RightBoundary);
				float py = General.Clamp(t.Position.y, General.Map.Config.BottomBoundary, General.Map.Config.TopBoundary);
				if(t.Position.x != px || t.Position.y != py) t.Move(new Vector2D(px, py));

				// Apply all flags
				foreach(CheckBox c in flags.Checkboxes) {
					if(c.CheckState == CheckState.Checked)
						t.SetFlag(c.Tag.ToString(), true);
					else if(c.CheckState == CheckState.Unchecked)
						t.SetFlag(c.Tag.ToString(), false);
				}

				// Action/tags
				t.Tag = tagSelector.GetTag(t.Tag); //mxd
				if(!action.Empty) {
					t.Action = action.Value;

					//mxd. Script name/number handling
					if(hasAcs) {
						if(!cbArgStr.Checked) { //apply script number
							if(!string.IsNullOrEmpty(scriptNumbers.Text)) {
								if(scriptNumbers.SelectedItem != null)
									t.Args[0] = ((ScriptItem)scriptNumbers.SelectedItem).Index;
								else if(!int.TryParse(scriptNumbers.Text.Trim(), out t.Args[0]))
									t.Args[0] = 0;

								if(General.Map.FormatInterface.HasCustomFields && t.Fields.ContainsKey("arg0str")) 
									t.Fields.Remove("arg0str");
							}
						} else { //apply arg0str
							if(!string.IsNullOrEmpty(scriptNames.Text))
								t.Fields["arg0str"] = new UniValue(UniversalType.String, scriptNames.Text);
						}
					} else {
						t.Args[0] = arg0.GetResult(t.Args[0]);
						if(General.Map.FormatInterface.HasCustomFields && t.Fields.ContainsKey("arg0str"))
							t.Fields.Remove("arg0str");
					}
				} else {
					t.Args[0] = arg0.GetResult(t.Args[0]);
				}

				t.Args[1] = arg1.GetResult(t.Args[1]);
				t.Args[2] = arg2.GetResult(t.Args[2]);
				t.Args[3] = arg3.GetResult(t.Args[3]);
				t.Args[4] = arg4.GetResult(t.Args[4]);

				//mxd. Custom fields
				if (General.Map.FormatInterface.HasCustomFields) {
					fieldslist.Apply(t.Fields);
					if (!string.IsNullOrEmpty(conversationID.Text))
						UDMFTools.SetInteger(t.Fields, "conversation", conversationID.GetResult(t.Fields.GetValue("conversation", 0)), 0);
					if (!string.IsNullOrEmpty(gravity.Text))
						UDMFTools.SetFloat(t.Fields, "gravity", gravity.GetResultFloat(t.Fields.GetValue("gravity", 1.0f)), 1.0f);
					if (!string.IsNullOrEmpty(health.Text))
						UDMFTools.SetInteger(t.Fields, "health", health.GetResult(t.Fields.GetValue("health", 1)), 1);
					if (!string.IsNullOrEmpty(score.Text))
						UDMFTools.SetInteger(t.Fields, "score", score.GetResult(t.Fields.GetValue("score", 0)), 0);
					if (!string.IsNullOrEmpty(alpha.Text))
						UDMFTools.SetFloat(t.Fields, "alpha", alpha.GetResultFloat(t.Fields.GetValue("alpha", 1.0f)), 1.0f);
					
					color.ApplyTo(t.Fields, t.Fields.GetValue("fillcolor", 0));
					if (renderStyle.SelectedIndex > -1) UDMFTools.SetString(t.Fields, "renderstyle", renderStyles[renderStyle.SelectedIndex], "normal");
				}

				// Update settings
				t.UpdateConfiguration();
			}

			// Set as defaults
			foreach (CheckBox c in flags.Checkboxes) {
				if (c.CheckState == CheckState.Checked)
					defaultflags.Add(c.Tag.ToString());
			}
			General.Settings.DefaultThingType = thingtype.GetResult(General.Settings.DefaultThingType);
			General.Settings.DefaultThingAngle = Angle2D.DegToRad((float)angle.GetResult((int)Angle2D.RadToDeg(General.Settings.DefaultThingAngle) - 90) + 90);
			General.Settings.SetDefaultThingFlags(defaultflags);

			// Done
			General.Map.IsChanged = true;
			if(OnValuesChanged != null) OnValuesChanged(this, EventArgs.Empty); //mxd
			this.DialogResult = DialogResult.OK;
			this.Close();
		}

		// Cancel clicked
		private void cancel_Click(object sender, EventArgs e) {
			//mxd. perform undo
			General.Map.UndoRedo.WithdrawUndo();

			// Be gone
			this.DialogResult = DialogResult.Cancel;
			this.Close();
		}

		//mxd
		private void cbArgStr_CheckedChanged(object sender, EventArgs e) {
			if(!cbArgStr.Visible) return;
			scriptNames.Visible = cbArgStr.Checked;
			scriptNumbers.Visible = !cbArgStr.Checked;
			arg0label.Text = cbArgStr.Checked ? "Script Name:" : "Script Number:";
		}

		//mxd
		private void cbAbsoluteHeight_CheckedChanged(object sender, EventArgs e) {
			useAbsoluteHeight = cbAbsoluteHeight.Checked;
			zlabel.Text = useAbsoluteHeight ? "Abs. Z:" : "Z:";
			posZ.Text = (useAbsoluteHeight ? initialFloorHeight + initialPosition.z : initialPosition.z).ToString();
		}

		//mxd
		private void tabcustom_MouseEnter(object sender, EventArgs e) {
			fieldslist.Focus();
		}

		//mxd
		private void ThingEditForm_FormClosing(object sender, FormClosingEventArgs e) {
			location = this.Location;
			activeTab = tabs.SelectedIndex;
		}

		// Help
		private void ThingEditForm_HelpRequested(object sender, HelpEventArgs hlpevent) {
			General.ShowHelp("w_thingeditor.html");
			hlpevent.Handled = true;
		}

		#endregion

		#region ================== mxd. Realtime events

		private void posX_WhenTextChanged(object sender, EventArgs e) {
			if(preventchanges) return;
			int i = 0;

			//restore values
			if(string.IsNullOrEmpty(posX.Text)) {
				// Apply position
				foreach(Thing t in things)
					t.Move(new Vector2D(thingProps[i++].X, t.Position.y));
			} else { //update values
				float delta = posX.GetResultFloat(initialPosition.x) - initialPosition.x;

				// Apply position
				foreach(Thing t in things)
					t.Move(new Vector2D(thingProps[i++].X + delta, t.Position.y));
			}

			General.Map.IsChanged = true;
			if(OnValuesChanged != null) OnValuesChanged(this, EventArgs.Empty);
		}

		private void posY_WhenTextChanged(object sender, EventArgs e) {
			if(preventchanges) return;
			int i = 0;

			//restore values
			if(string.IsNullOrEmpty(posY.Text)) {
				// Apply position
				foreach(Thing t in things)
					t.Move(new Vector2D(t.Position.x, thingProps[i++].Y));
			} else { //update values
				float delta = posY.GetResultFloat(initialPosition.y) - initialPosition.y;

				// Apply position
				foreach(Thing t in things)
					t.Move(new Vector2D(t.Position.x, thingProps[i++].Y + delta));
			}

			General.Map.IsChanged = true;
			if(OnValuesChanged != null) OnValuesChanged(this, EventArgs.Empty);
		}

		private void posZ_WhenTextChanged(object sender, EventArgs e) {
			if(preventchanges) return;

			//restore values
			if(string.IsNullOrEmpty(posZ.Text)) {
				int i = 0;

				// Apply position
				foreach(Thing t in things)
					t.Move(new Vector3D(t.Position.x, t.Position.y, thingProps[i++].Z));
			} else { //update values
				// Apply position
				foreach(Thing t in things) {
					float z = posZ.GetResultFloat((int)t.Position.z);
					if(useAbsoluteHeight && t.Sector != null)
						z -= t.Sector.FloorHeight;
					t.Move(new Vector3D(t.Position.x, t.Position.y, z));
				}
			}

			General.Map.IsChanged = true;
			if(OnValuesChanged != null) OnValuesChanged(this, EventArgs.Empty);
		}

		private void scale_OnValuesChanged(object sender, EventArgs e) {
			if (preventchanges) return;
			int i = 0;

			foreach (Thing t in things){
				float sx = scale.GetValue1(thingProps[i].ScaleX);
				float sy = scale.GetValue2(thingProps[i].ScaleY);
				t.SetScale((sx == 0 ? 1.0f : sx), (sy == 0 ? 1.0f : sy));
				i++;
			}

			General.Map.IsChanged = true;
			if (OnValuesChanged != null) OnValuesChanged(this, EventArgs.Empty);
		}

		// Selected type changes
		private void thingtype_OnTypeChanged(ThingTypeInfo value) {
			thinginfo = value;

			// Update preview image
			if(thinginfo != null) {
				if(thinginfo.Sprite.ToLowerInvariant().StartsWith(DataManager.INTERNAL_PREFIX) &&
				   (thinginfo.Sprite.Length > DataManager.INTERNAL_PREFIX.Length)) {
					General.DisplayZoomedImage(spritetex, General.Map.Data.GetSpriteImage(thinginfo.Sprite).GetBitmap());
				} else if((thinginfo.Sprite.Length <= 8) && (thinginfo.Sprite.Length > 0)) {
					General.DisplayZoomedImage(spritetex, General.Map.Data.GetSpriteImage(thinginfo.Sprite).GetPreview());
				} else {
					spritetex.BackgroundImage = null;
				}
			} else {
				spritetex.BackgroundImage = null;
			}

			// Update arguments
			action_ValueChanges(this, EventArgs.Empty);

			//mxd. Update things
			if (preventchanges 
					|| (thingtype.GetResult(0) < General.Map.FormatInterface.MinThingType) 
					|| (thingtype.GetResult(0) > General.Map.FormatInterface.MaxThingType))
				return;

			foreach(Thing t in things) {
				//Set type
				t.Type = thingtype.GetResult(t.Type);

				// Update settings
				t.UpdateConfiguration();
			}

			General.Map.IsChanged = true;
			if(OnValuesChanged != null) OnValuesChanged(this, EventArgs.Empty);
		}

		//mxd
		private void updateAngle() {
			if(preventchanges) return;
			int i = 0;

			//restore values
			if(string.IsNullOrEmpty(angle.Text)) {
				foreach(Thing t in things)
					t.Rotate(thingProps[i++].AngleDoom);
			} else { //update values
				foreach(Thing t in things)
					t.Rotate(angle.GetResult(thingProps[i++].AngleDoom));
			}

			General.Map.IsChanged = true;
			if(OnValuesChanged != null) OnValuesChanged(this, EventArgs.Empty);
		}

		private void updatePitch() {
			if (preventchanges) return;
			int i = 0;

			//restore values
			if (string.IsNullOrEmpty(pitch.Text)) {
				foreach (Thing t in things)
					t.SetPitch(thingProps[i++].Pitch);
			} else { //update values
				foreach (Thing t in things)
					t.SetPitch(pitch.GetResult(thingProps[i++].Pitch));
			}

			General.Map.IsChanged = true;
			if (OnValuesChanged != null) OnValuesChanged(this, EventArgs.Empty);
		}

		private void updateRoll() {
			if (preventchanges) return;
			int i = 0;

			//restore values
			if (string.IsNullOrEmpty(roll.Text)) {
				foreach (Thing t in things)
					t.SetRoll(thingProps[i++].Roll);
			} else { //update values
				foreach (Thing t in things)
					t.SetRoll(roll.GetResult(thingProps[i++].Roll));
			}

			General.Map.IsChanged = true;
			if (OnValuesChanged != null) OnValuesChanged(this, EventArgs.Empty);
		}

		#endregion

	}
}