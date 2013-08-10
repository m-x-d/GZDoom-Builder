
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
	internal partial class ThingEditForm : DelayedForm
	{
		#region ================== Events

		public event EventHandler OnValuesChanged; //mxd

		#endregion
		
		#region ================== Variables

		private ICollection<Thing> things;
		private ThingTypeInfo thinginfo;
		private bool preventchanges;
        //mxd
        private Vector3D initialPosition; //initial position of a thing used to fill posX, posY and posZ fields
		private int initialFloorHeight; //floor height of the sector first thing is in
		private static bool useAbsoluteHeight;
		private string arg0str;
		private bool haveArg0Str;

		private List<ThingProperties> thingProps; //mxd

		private struct ThingProperties //mxd
		{
			public int Type;
			public int AngleDoom;
			public float X;
			public float Y;
			public float Z;

			public ThingProperties(Thing t) {
				X = t.Position.x;
				Y = t.Position.y;
				Z = t.Position.z;
				Type = t.Type;
				AngleDoom = t.AngleDoom;
			}
		}

		#endregion

		#region ================== Constructor

		// Constructor
		public ThingEditForm()
		{
			// Initialize
			InitializeComponent();
			
			// Fill flags list
			foreach(KeyValuePair<string, string> tf in General.Map.Config.ThingFlags)
				flags.Add(tf.Value, tf.Key);

			// Fill actions list
			action.GeneralizedCategories = General.Map.Config.GenActionCategories;
			action.AddInfo(General.Map.Config.SortedLinedefActions.ToArray());

			if(General.Map.FormatInterface.HasCustomFields) { //mxd
				// Initialize custom fields editor
				fieldslist.Setup("thing");

				// Fill universal fields list
				fieldslist.ListFixedFields(General.Map.Config.ThingFields);
			} else {
				tabs.TabPages.Remove(tabcustom);

				// Hide unused controls
				labelID.Visible = false; //mxd
				conversationID.Visible = false; //mxd
				labelGravity.Visible = false; //mxd
				gravity.Visible = false; //mxd
			}

			// Tag/Effects?
			if(!General.Map.FormatInterface.HasThingAction && !General.Map.FormatInterface.HasThingTag){
				tabs.TabPages.Remove(tabeffects);
			} else { //mxd. Setup script names and numbers
				scriptNames.Location = arg0.Location;
				scriptNumbers.Location = arg0.Location;

				foreach(ScriptItem nsi in General.Map.NamedScripts)
					scriptNames.Items.Add(nsi);

				foreach(ScriptItem si in General.Map.NumberedScripts)
					scriptNumbers.Items.Add(si);
			}
			
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
		public void Setup(ICollection<Thing> things)
		{
			Thing ft;

			preventchanges = true;

			// Keep this list
			this.things = things;
			if(things.Count > 1) this.Text = "Edit Things (" + things.Count + ")";

			//mxd. Make undo
			string undodesc = "thing";
			if(things.Count > 1) undodesc = things.Count + " things";
			General.Map.UndoRedo.CreateUndo("Edit " + undodesc);
			
			////////////////////////////////////////////////////////////////////////
			// Set all options to the first thing properties
			////////////////////////////////////////////////////////////////////////

			ft = General.GetByIndex(things, 0);
			
			// Set type
			thingtype.SelectType(ft.Type);
			
			// Flags
			foreach(CheckBox c in flags.Checkboxes)
				if(ft.Flags.ContainsKey(c.Tag.ToString())) c.Checked = ft.Flags[c.Tag.ToString()];
			
			// Coordination
			angle.Text = ft.AngleDoom.ToString();
			zlabel.Text = useAbsoluteHeight ? "Z:" : "Height:"; //mxd
			cbAbsoluteHeight.Checked = useAbsoluteHeight; //mxd

            //mxd
            initialPosition = ft.Position;
            if (ft.Sector != null)
			    initialFloorHeight = ft.Sector.FloorHeight;
            posX.Text = ((int)ft.Position.x).ToString();
            posY.Text = ((int)ft.Position.y).ToString();
            posZ.Text = useAbsoluteHeight ? ((int)ft.Position.z + initialFloorHeight).ToString() : ((int)ft.Position.z).ToString();
            posX.ButtonStep = General.Map.Grid.GridSize;
            posY.ButtonStep = General.Map.Grid.GridSize;
			posZ.ButtonStep = General.Map.Grid.GridSize;

			//mxd. Custom fields
			if(General.Map.FormatInterface.HasCustomFields) {
				fieldslist.SetValues(ft.Fields, true);
				conversationID.Text = ft.Fields.GetValue("conversation", 0).ToString();
				gravity.Text = ft.Fields.GetValue("gravity", 1.0f).ToString();
				arg0str = ft.Fields.GetValue("arg0str", string.Empty);
				haveArg0Str = !string.IsNullOrEmpty(arg0str);
			}

			// Action/tags
			action.Value = ft.Action;
			if(General.Map.FormatInterface.HasThingTag) {//mxd
				tagSelector.Setup(); 
				tagSelector.SetTag(ft.Tag);
			}
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
			foreach(Thing t in things)
			{
				// Type does not match?
				ThingTypeInfo info = thingtype.GetSelectedInfo(); //mxd

				if(info != null && info.Index != t.Type)
					thingtype.ClearSelectedType();
				
				// Flags
				foreach(CheckBox c in flags.Checkboxes)
				{
					if(t.Flags.ContainsKey(c.Tag.ToString()))
					{
						if(t.Flags[c.Tag.ToString()] != c.Checked)
						{
							c.ThreeState = true;
							c.CheckState = CheckState.Indeterminate;
						}
					}
				}
				
				// Coordination
				if(t.AngleDoom.ToString() != angle.Text) angle.Text = "";
				
				//mxd
                if (useAbsoluteHeight && t.Sector != null) {
					if(((int)t.Position.z + t.Sector.FloorHeight).ToString() != posZ.Text) posZ.Text = "";
				} else if(((int)t.Position.z).ToString() != posZ.Text){
					posZ.Text = "";
				}

				// Action/tags
				if(t.Action != action.Value) action.Empty = true;
				if(General.Map.FormatInterface.HasThingTag && t.Tag != ft.Tag) tagSelector.ClearTag(); //mxd
				if(t.Args[0] != arg0.GetResult(-1)) arg0.ClearValue();
				if(t.Args[1] != arg1.GetResult(-1)) arg1.ClearValue();
				if(t.Args[2] != arg2.GetResult(-1)) arg2.ClearValue();
				if(t.Args[3] != arg3.GetResult(-1)) arg3.ClearValue();
				if(t.Args[4] != arg4.GetResult(-1)) arg4.ClearValue();

				//mxd. Custom fields
				if(General.Map.FormatInterface.HasCustomFields){
					t.Fields.BeforeFieldsChange(); //mxd
					fieldslist.SetValues(t.Fields, false);

					if(t.Fields.GetValue("conversation", 0).ToString() != conversationID.Text)
						conversationID.Text = "";

					if(t.Fields.GetValue("gravity", 1.0f).ToString() != gravity.Text)
						gravity.Text = "";

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
				bool showNamedScripts = General.Map.UDMF && haveArg0Str;
				cbArgStr.Visible = General.Map.UDMF;
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
		private void action_ValueChanges(object sender, EventArgs e)
		{
			int showaction = 0;
			ArgumentInfo[] arginfo;

			// Only when line type is known, otherwise use the thing arguments
			if(General.Map.Config.LinedefActions.ContainsKey(action.Value)) showaction = action.Value;
			if((showaction == 0) && (thinginfo != null)) arginfo = thinginfo.Args; else arginfo = General.Map.Config.LinedefActions[showaction].Args;
			
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
			if(arg0label.Enabled) arg0.ForeColor = SystemColors.WindowText; else arg0.ForeColor = SystemColors.GrayText;
			if(arg1label.Enabled) arg1.ForeColor = SystemColors.WindowText; else arg1.ForeColor = SystemColors.GrayText;
			if(arg2label.Enabled) arg2.ForeColor = SystemColors.WindowText; else arg2.ForeColor = SystemColors.GrayText;
			if(arg3label.Enabled) arg3.ForeColor = SystemColors.WindowText; else arg3.ForeColor = SystemColors.GrayText;
			if(arg4label.Enabled) arg4.ForeColor = SystemColors.WindowText; else arg4.ForeColor = SystemColors.GrayText;

            arg0.Setup(arginfo[0]);
            arg1.Setup(arginfo[1]);
            arg2.Setup(arginfo[2]);
            arg3.Setup(arginfo[3]);
            arg4.Setup(arginfo[4]);

			// Zero all arguments when linedef action 0 (normal) is chosen
			if(!preventchanges && (showaction == 0))
			{
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
		private void browseaction_Click(object sender, EventArgs e)
		{
			action.Value = ActionBrowserForm.BrowseAction(this, action.Value);
		}

		// Angle text changes
		private void angle_TextChanged(object sender, EventArgs e)
		{
			anglecontrol.Value = angle.GetResult(int.MinValue);
			updateAngle(); //mxd
		}

		// Angle control clicked
		private void anglecontrol_ButtonClicked(object sender, EventArgs e)
		{
			angle.Text = anglecontrol.Value.ToString();
			updateAngle(); //mxd
		}

		// Apply clicked
		private void apply_Click(object sender, EventArgs e)
		{
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
			if(((thingtype.GetResult(0) < General.Map.FormatInterface.MinThingType) || (thingtype.GetResult(0) > General.Map.FormatInterface.MaxThingType)))
			{
				General.ShowWarningMessage("Thing type must be between " + General.Map.FormatInterface.MinThingType + " and " + General.Map.FormatInterface.MaxThingType + ".", MessageBoxButtons.OK);
				return;
			}

			// Verify the action
			if(General.Map.FormatInterface.HasThingAction && ((action.Value < General.Map.FormatInterface.MinAction) || (action.Value > General.Map.FormatInterface.MaxAction)))
			{
				General.ShowWarningMessage("Thing action must be between " + General.Map.FormatInterface.MinAction + " and " + General.Map.FormatInterface.MaxAction + ".", MessageBoxButtons.OK);
				return;
			}

			bool hasAcs = !action.Empty && Array.IndexOf(GZBuilder.GZGeneral.ACS_SPECIALS, action.Value) != -1; //mxd

			// Go for all the things
			foreach(Thing t in things)
			{
				// Coordination
				if(cbRandomAngle.Checked) t.Rotate(General.Random(0, 359)); //mxd

				//mxd. Check position
				float px = General.Clamp(t.Position.x, General.Map.Config.LeftBoundary, General.Map.Config.RightBoundary);
				float py = General.Clamp(t.Position.y, General.Map.Config.BottomBoundary, General.Map.Config.TopBoundary);
				if(t.Position.x != px || t.Position.y != py)
					t.Move(new Vector2D(px, py));
				
				// Apply all flags
				foreach(CheckBox c in flags.Checkboxes)
				{
					if(c.CheckState == CheckState.Checked) t.SetFlag(c.Tag.ToString(), true);
					else if(c.CheckState == CheckState.Unchecked) t.SetFlag(c.Tag.ToString(), false);
				}

				// Action/tags
				t.Tag = tagSelector.GetTag(t.Tag); //mxd
				if (!action.Empty) {
					t.Action = action.Value;

					//mxd. Script name/number handling
					if(hasAcs) {
						if(!cbArgStr.Checked) { //apply script number
							if(!string.IsNullOrEmpty(scriptNumbers.Text)) {
								if(scriptNumbers.SelectedItem != null)
									t.Args[0] = ((ScriptItem)scriptNumbers.SelectedItem).Index;
								else if(!int.TryParse(scriptNumbers.Text.Trim(), out t.Args[0]))
									t.Args[0] = 0;

								if(t.Fields.ContainsKey("arg0str")) t.Fields.Remove("arg0str");
							}
						} else { //apply arg0str
							if(!string.IsNullOrEmpty(scriptNames.Text)) 
								t.Fields["arg0str"] = new UniValue(UniversalType.String, scriptNames.Text);
						}
					} else {
						t.Args[0] = arg0.GetResult(t.Args[0]);
						if(t.Fields.ContainsKey("arg0str")) t.Fields.Remove("arg0str");
					}
				}else{
					t.Args[0] = arg0.GetResult(t.Args[0]);
				}

				t.Args[1] = arg1.GetResult(t.Args[1]);
				t.Args[2] = arg2.GetResult(t.Args[2]);
				t.Args[3] = arg3.GetResult(t.Args[3]);
				t.Args[4] = arg4.GetResult(t.Args[4]);

				//mxd. Custom fields
				if(General.Map.FormatInterface.HasCustomFields){
					fieldslist.Apply(t.Fields);

					if(!string.IsNullOrEmpty(conversationID.Text))
						UDMFTools.SetInteger(t.Fields, "conversation", conversationID.GetResult(t.Fields.GetValue("conversation", 0)), 0);

					if(!string.IsNullOrEmpty(gravity.Text))
						UDMFTools.SetFloat(t.Fields, "gravity", gravity.GetResultFloat(t.Fields.GetValue("gravity", 1.0f)), 1.0f);
				}
				
				// Update settings
				t.UpdateConfiguration();
			}

			// Set as defaults
			foreach(CheckBox c in flags.Checkboxes)
				if(c.CheckState == CheckState.Checked) defaultflags.Add(c.Tag.ToString());
			General.Settings.DefaultThingType = thingtype.GetResult(General.Settings.DefaultThingType);
			General.Settings.DefaultThingAngle = Angle2D.DegToRad((float)angle.GetResult((int)Angle2D.RadToDeg(General.Settings.DefaultThingAngle) - 90) + 90);
			General.Settings.SetDefaultThingFlags(defaultflags);
			
			// Done
			General.Map.IsChanged = true;
			if(OnValuesChanged != null)	OnValuesChanged(this, EventArgs.Empty); //mxd
			this.DialogResult = DialogResult.OK;
			this.Close();
		}

		// Cancel clicked
		private void cancel_Click(object sender, EventArgs e)
		{
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
			zlabel.Text = useAbsoluteHeight ? "Z:" : "Height:";
			posZ.Text = (useAbsoluteHeight ? initialFloorHeight + initialPosition.z : initialPosition.z).ToString();
		}

		//mxd
		private void cbRandomAngle_CheckedChanged(object sender, EventArgs e) {
			angle.Enabled = !cbRandomAngle.Checked;
			anglecontrol.Enabled = !cbRandomAngle.Checked;
			labelAngle.Enabled = !cbRandomAngle.Checked;
		}

		//mxd
		private void tabcustom_MouseEnter(object sender, EventArgs e) {
			fieldslist.Focus();
		}

		// Help
		private void ThingEditForm_HelpRequested(object sender, HelpEventArgs hlpevent)
		{
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
				int delta = posX.GetResult((int)initialPosition.x) - (int)initialPosition.x;

				// Apply position
				foreach(Thing t in things)
					t.Move(new Vector2D(thingProps[i++].X + delta, t.Position.y));
			}

			General.Map.IsChanged = true;
			if(OnValuesChanged != null)	OnValuesChanged(this, EventArgs.Empty);
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
				int delta = posY.GetResult((int)initialPosition.y) - (int)initialPosition.y;

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
			if(string.IsNullOrEmpty(posY.Text)) {
				int i = 0;

				// Apply position
				foreach(Thing t in things)
					t.Move(new Vector3D(t.Position.x, t.Position.y, thingProps[i++].Z));
			} else { //update values
				// Apply position
				foreach(Thing t in things) {
					float z = (float)posZ.GetResult((int)t.Position.z);
					if(useAbsoluteHeight && t.Sector != null)
						z -= t.Sector.FloorHeight;
					t.Move(new Vector3D(t.Position.x, t.Position.y, z));
				}
			}

			General.Map.IsChanged = true;
			if(OnValuesChanged != null) OnValuesChanged(this, EventArgs.Empty);
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
			if(preventchanges) return;
			if(((thingtype.GetResult(0) < General.Map.FormatInterface.MinThingType) || (thingtype.GetResult(0) > General.Map.FormatInterface.MaxThingType)))
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

			//restore values
			if(string.IsNullOrEmpty(angle.Text)) {
				int i = 0;

				// Apply rotation
				foreach(Thing t in things)
					t.Rotate(thingProps[i++].AngleDoom);
			} else { //update values
				// Apply rotation
				foreach(Thing t in things)
					t.Rotate(angle.GetResult(t.AngleDoom));
			}

			General.Map.IsChanged = true;
			if(OnValuesChanged != null)	OnValuesChanged(this, EventArgs.Empty);
		}

		#endregion
	}
}