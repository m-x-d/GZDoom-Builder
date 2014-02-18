using System;
using System.Collections.Generic;
using System.Windows.Forms;
using CodeImp.DoomBuilder.Rendering;
using CodeImp.DoomBuilder.GZBuilder.Data;
using CodeImp.DoomBuilder.Config;

namespace CodeImp.DoomBuilder.GZBuilder.Controls
{
	public partial class CustomLinedefColorProperties : UserControl
	{
		public event EventHandler PresetChanged;

		public PixelColor DefaultColor { get { return defaultColor; } }
		private PixelColor defaultColor;
		private bool presetUpdating;
		private LinedefColorPreset preset;

		public bool UDMF { get { return udmf; } }
		private bool udmf;
		
		private struct FlagData
		{
			public string Title { get { return title; } }
			private string title;

			public string Flag { get { return flag; } }
			private string flag;

			public FlagData(string title, string flag) {
				this.title = title;
				this.flag = flag;
			}

			public override string ToString() {
				return Title;
			}
		}

		public CustomLinedefColorProperties() {
			InitializeComponent();

			defaultColor = lineColor.Color;

			//disable controls
			this.Enabled = false;
			labelErrors.Text = "";
		}

		public void Setup(GameConfiguration config) {
			udmf = (config.FormatInterface == "UniversalMapSetIO");

			presetUpdating = true;
			//add flags
			flags.Items.Clear();
			foreach(KeyValuePair<string, string> lf in config.LinedefFlags)
				flags.Items.Add(new FlagData(lf.Value, lf.Key), CheckState.Indeterminate);

			// Fill actions list
			action.GeneralizedCategories = config.GenActionCategories;

			LinedefActionInfo anyAction = new LinedefActionInfo(-1, "Any action", true, false);
			List<LinedefActionInfo> infos = config.SortedLinedefActions;
			infos.Insert(0, anyAction);

			action.AddInfo(infos.ToArray());
			action.Value = -1;

			//activations
			udmfactivates.Visible = udmf;
			activation.Visible = !udmf;
			activation.Items.Clear();
			udmfactivates.Items.Clear();

			//get activates
			List<LinedefActivateInfo> activations = config.LinedefActivates;

			if(activations.Count > 0) {
				activations.Insert(0, new LinedefActivateInfo("-1", "Any activation"));

				if(udmf) {
					foreach(LinedefActivateInfo ai in config.LinedefActivates)
						udmfactivates.Items.Add(new FlagData(ai.Title, ai.Key), CheckState.Indeterminate);
				} else {
					activation.Items.AddRange(activations.ToArray());
				}

				if(!tcLineSettings.TabPages.Contains(tabActivation))
					tcLineSettings.TabPages.Add(tabActivation);
			} else {
				tcLineSettings.TabPages.Remove(tabActivation);
			}

			//disable controls
			flags.Enabled = false;
			action.Enabled = false;
			activation.Enabled = false;
			udmfactivates.Enabled = false;

			presetUpdating = false;
		}

		public void SetPreset(LinedefColorPreset preset) {
			this.Enabled = true;
			presetUpdating = true;

			this.preset = preset;

			//set color
			lineColor.Color = this.preset.Color;

			//set flags
			applyFlagsToControl(flags, cbUseFlags);

			//set activation
			if(udmf) {
				applyFlagsToControl(udmfactivates, cbUseActivation);
			} else if(tcLineSettings.TabPages.Contains(tabActivation)) {
				if(this.preset.Activation == 0) {
					activation.SelectedIndex = 1;
					cbUseActivation.Checked = false;
					activation.Enabled = false;
				} else {
					for(int i = 0; i < activation.Items.Count; i++) {
						if(((LinedefActivateInfo)activation.Items[i]).Index == this.preset.Activation) {
							activation.SelectedIndex = i;
							cbUseActivation.Checked = true;
							activation.Enabled = true;
							break;
						}
					}
				}
			}

			//set action
			action.Value = this.preset.Action;
			cbUseAction.Checked = this.preset.Action != 0;
			action.Enabled = this.preset.Action != 0;

			//warnings/errors?
			UpdateMessages();

			presetUpdating = false;
		}

		public LinedefColorPreset GetPreset() {
			return preset;
		}

		public void UpdateMessages() {
			//warnings/errors?
			List<string> errors = new List<string>();

			if(!preset.Valid)
				errors.Add(preset.ErrorDescription);

			if(!string.IsNullOrEmpty(preset.WarningDescription))
				errors.Add(preset.WarningDescription);

			labelErrors.Text = (errors.Count > 0 ? string.Join(Environment.NewLine, errors.ToArray()) : "");
		}

		private void raiseEvent() {
			if(PresetChanged != null)
				PresetChanged(this, EventArgs.Empty);
		}

		private void applyFlagsToPreset(CheckedListBox source) {
			if(source.Enabled) {
				for(int i = 0; i < source.Items.Count; i++) {
					string flag = ((FlagData)source.Items[i]).Flag;
					CheckState state = source.GetItemCheckState(i);

					if(state == CheckState.Checked) {
						if(!preset.Flags.Contains(flag))
							preset.Flags.Add(flag);
						if(preset.RestrictedFlags.Contains(flag))
							preset.RestrictedFlags.Remove(flag);
					} else if(state == CheckState.Unchecked) {
						if(preset.Flags.Contains(flag))
							preset.Flags.Remove(flag);
						if(!preset.RestrictedFlags.Contains(flag))
							preset.RestrictedFlags.Add(flag);
					} else {
						if(preset.Flags.Contains(flag))
							preset.Flags.Remove(flag);
						if(preset.RestrictedFlags.Contains(flag))
							preset.RestrictedFlags.Remove(flag);
					}
				}
			} else {
				for(int i = 0; i < source.Items.Count; i++) {
					string flag = ((FlagData)source.Items[i]).Flag;

					if(preset.Flags.Contains(flag))
						preset.Flags.Remove(flag);
					if(preset.RestrictedFlags.Contains(flag))
						preset.RestrictedFlags.Remove(flag);
				}
			}
		}

		private void applyFlagsToControl(CheckedListBox target, CheckBox cb) {
			if(preset.Flags.Count == 0 && preset.RestrictedFlags.Count == 0) {
				cb.Checked = false;
				target.Enabled = false;

				for(int i = 0; i < target.Items.Count; i++)
					target.SetItemCheckState(i, CheckState.Indeterminate);
			} else {
				bool hasFlags = false;
				CheckState flagState;

				for(int i = 0; i < target.Items.Count; i++) {
					string flag = ((FlagData)target.Items[i]).Flag;

					if(preset.Flags.Contains(flag))
						flagState = CheckState.Checked;
					else if(preset.RestrictedFlags.Contains(flag))
						flagState = CheckState.Unchecked;
					else
						flagState = CheckState.Indeterminate;

					target.SetItemCheckState(i, flagState);
					if(flagState != CheckState.Indeterminate) hasFlags = true;
				}

				cb.Checked = hasFlags;
				target.Enabled = hasFlags;
			}
		}

//EVENTS
		private void cbUseFlags_CheckedChanged(object sender, EventArgs e) {
			if(presetUpdating) return;
			flags.Enabled = cbUseFlags.Checked;
			applyFlagsToPreset(flags);

			if(!flags.Enabled) raiseEvent();
		}

		private void flags_SelectedValueChanged(object sender, EventArgs e) {
			if(presetUpdating) return;
			applyFlagsToPreset(flags);

			raiseEvent();
		}

		private void cbUseAction_CheckedChanged(object sender, EventArgs e) {
			if(presetUpdating) return;

			action.Enabled = cbUseAction.Checked;
			action.Value = 0;
			//raiseEvent();
		}

		private void action_ValueChanges(object sender, EventArgs e) {
			if(presetUpdating) return;
			preset.Action = action.Value;
			raiseEvent();
		}

		private void cbUseActivation_CheckedChanged(object sender, EventArgs e) {
			if(presetUpdating) return;
			activation.Enabled = cbUseActivation.Checked;
			udmfactivates.Enabled = cbUseActivation.Checked;

			if(udmf) {
				applyFlagsToPreset(udmfactivates);
			} else {
				if(!cbUseActivation.Checked)
					activation.SelectedIndex = 1;
			}

			if(!cbUseActivation.Checked) raiseEvent();
		}

		private void activation_SelectedIndexChanged(object sender, EventArgs e) {
			if(presetUpdating) return;
			
			preset.Activation = ((LinedefActivateInfo)activation.SelectedItem).Index;
			raiseEvent();
		}

		private void udmfactivates_SelectedValueChanged(object sender, EventArgs e) {
			if(presetUpdating) return;
			applyFlagsToPreset(udmfactivates);
			raiseEvent();
		}

		private void lineColor_ColorChanged(object sender, EventArgs e) {
			if(presetUpdating) return;
			
			preset.Color = lineColor.Color;
			raiseEvent();
		}

		private void flags_ItemCheck(object sender, ItemCheckEventArgs e) {
			if(presetUpdating)	return;
			
			if(e.CurrentValue == CheckState.Checked)
				e.NewValue = CheckState.Indeterminate;
			else if(e.CurrentValue == CheckState.Indeterminate)
				e.NewValue = CheckState.Unchecked;
			else
				e.NewValue = CheckState.Checked;
		}
	}
}
