using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using CodeImp.DoomBuilder.GZBuilder.Data;
using CodeImp.DoomBuilder.Config;

namespace CodeImp.DoomBuilder.GZBuilder.Controls
{
	public partial class CustomLinedefColorsControl : UserControl
	{
		public event EventHandler PresetsChanged;

		private const string DEFAULT_PRESET_NAME = "Enter preset name";
		private const string NO_PRESET_NAME = "ENTER PRESET NAME!";
		
		public CustomLinedefColorsControl() {
			InitializeComponent();

			colorProperties.PresetChanged += new EventHandler(colorProperties_PresetChanged);
			//disable controls
			gbPresets.Enabled = false;
		}

		public void Setup(GameConfiguration config, ConfigurationInfo configInfo) {
			colorProperties.Setup(config, configInfo);
			lbColorPresets.Items.Clear();

			if(configInfo.LinedefColorPresets.Length > 0) {
				//validate
				for(int i = 0; i < configInfo.LinedefColorPresets.Length; i++) {
					validatePreset(configInfo.LinedefColorPresets[i]);
					checkDuplicates(configInfo.LinedefColorPresets[i]);
				}

				lbColorPresets.Items.AddRange(configInfo.LinedefColorPresets);
				lbColorPresets.SelectedIndex = 0;
			}

			updatePresetListControls();
			gbPresets.Enabled = true;
		}

		public LinedefColorPreset[] GetPresets() {
			List<LinedefColorPreset> presets = new List<LinedefColorPreset>();

			foreach(LinedefColorPreset preset in lbColorPresets.Items)
				if(preset.Valid) presets.Add(preset);

			return presets.ToArray();
		}

		private void validatePreset(LinedefColorPreset preset) {
			bool hasAction = preset.Action != 0;
			bool hasFlags = preset.Flags.Count > 0 || preset.RestrictedFlags.Count > 0;
			bool hasActivation = preset.Activation != 0;

			//validate
			if(!hasAction && !hasFlags && !hasActivation) {
				if(colorProperties.UDMF) {
					preset.SetInvalid("Invalid preset: no flags, action or activation type selected!");
				} else {
					preset.SetInvalid("Invalid preset: no flags or action selected!");
				}
				return;
			}

			preset.SetValid();
		}

		private bool validatePresetName() {
			if(string.IsNullOrEmpty(tbNewPresetName.Text) || tbNewPresetName.Text == DEFAULT_PRESET_NAME || tbNewPresetName.Text == NO_PRESET_NAME) {
				tbNewPresetName.ForeColor = Color.DarkRed;
				tbNewPresetName.Text = string.IsNullOrEmpty(tbNewPresetName.Text) ? DEFAULT_PRESET_NAME : NO_PRESET_NAME;
				return false;
			}

			foreach(LinedefColorPreset preset in lbColorPresets.Items) {
				if(preset.Name.ToLowerInvariant() == tbNewPresetName.Text.ToLowerInvariant()) {
					General.ShowWarningMessage("Preset with this name already exists!", MessageBoxButtons.OK);
					return false;
				}
			}

			tbNewPresetName.ForeColor = Color.Black;
			return true;
		}

		private void checkDuplicates(LinedefColorPreset preset)
		{
			foreach(LinedefColorPreset p in lbColorPresets.Items) {
				if(preset.Name == p.Name) continue;
				if(p.Action != preset.Action) continue;
				if(p.Activation != preset.Activation) continue;
				if(p.Flags.Count != preset.Flags.Count) continue;
				if(p.RestrictedFlags.Count != preset.RestrictedFlags.Count)	continue;

				bool gotMismatch = false;
				foreach(string flag in p.Flags) {
					if(!preset.Flags.Contains(flag)) {
						gotMismatch = true;
						break;
					}
				}

				foreach(string flag in p.RestrictedFlags) {
					if(!preset.RestrictedFlags.Contains(flag)) {
						gotMismatch = true;
						break;
					}
				}

				if(gotMismatch) continue;

				//we have a match
				preset.WarningDescription = "Preset matches '" + p.Name + "'";
				return;
			}
		}

		private void updatePresetListControls() {
			int c = lbColorPresets.Items.Count;

			bRemovePreset.Enabled = c > 0;
			colorProperties.Enabled = c > 0;

			if(c < 2) {
				bMoveDown.Enabled = false;
				bMoveUp.Enabled = false;
			} else {
				bMoveDown.Enabled = lbColorPresets.SelectedIndex < c - 1;
				bMoveUp.Enabled = lbColorPresets.SelectedIndex > 0;
			}
		}

//EVENTS
		private void bMoveDown_Click(object sender, EventArgs e) {
			if(lbColorPresets.SelectedIndex == -1) return;

			//I like to move it, move it!
			LinedefColorPreset preset = (LinedefColorPreset)lbColorPresets.SelectedItem;
			lbColorPresets.Items[lbColorPresets.SelectedIndex] = lbColorPresets.Items[lbColorPresets.SelectedIndex + 1];
			lbColorPresets.Items[lbColorPresets.SelectedIndex + 1] = preset;

			lbColorPresets.SelectedIndex++;

			if(PresetsChanged != null)
				PresetsChanged(this, EventArgs.Empty);
		}

		private void bMoveUp_Click(object sender, EventArgs e) {
			if(lbColorPresets.SelectedIndex == -1) return;

			LinedefColorPreset preset = (LinedefColorPreset)lbColorPresets.SelectedItem;
			lbColorPresets.Items[lbColorPresets.SelectedIndex] = lbColorPresets.Items[lbColorPresets.SelectedIndex - 1];
			lbColorPresets.Items[lbColorPresets.SelectedIndex - 1] = preset;

			lbColorPresets.SelectedIndex--;

			if(PresetsChanged != null)
				PresetsChanged(this, EventArgs.Empty);
		}

		private void bAddPreset_Click(object sender, EventArgs e) {
			if(!validatePresetName()) return;

			//add new item
			lbColorPresets.Items.Insert(0, new LinedefColorPreset(tbNewPresetName.Text, colorProperties.DefaultColor));
			tbNewPresetName.Text = "";

			//(re)select added preset
			if(lbColorPresets.SelectedIndex != 0)
				lbColorPresets.SelectedIndex = 0;
			else
				lbColorPresets_SelectedIndexChanged(this, EventArgs.Empty);

			updatePresetListControls();

			if(PresetsChanged != null)
				PresetsChanged(this, EventArgs.Empty);
		}

		private void bRemovePreset_Click(object sender, EventArgs e) {
			if(lbColorPresets.Items.Count == 0 || lbColorPresets.SelectedIndex == -1) return; //sanity check

			//remove item
			int index = lbColorPresets.SelectedIndex;
			lbColorPresets.Items.RemoveAt(index);
			lbColorPresets.SelectedIndex = (index >= lbColorPresets.Items.Count ? lbColorPresets.Items.Count - 1 : index);

			updatePresetListControls();

			if(PresetsChanged != null)
				PresetsChanged(this, EventArgs.Empty);
		}

		private void lbColorPresets_SelectedIndexChanged(object sender, EventArgs e) {
			if(lbColorPresets.SelectedIndex == -1) {
				colorProperties.Enabled = false;
				return;
			}
			
			colorProperties.SetPreset((LinedefColorPreset)lbColorPresets.SelectedItem);
			updatePresetListControls();
		}

		private void colorProperties_PresetChanged(object sender, EventArgs e) {
			LinedefColorPreset preset = (LinedefColorPreset)lbColorPresets.SelectedItem;
			preset.SetValid(); //clear error/warning messages
			validatePreset(preset); //validate it
			checkDuplicates(preset);
			colorProperties.UpdateMessages(); //update error/warning messages
			lbColorPresets.Invalidate(); //redraw icons

			if(PresetsChanged != null)
				PresetsChanged(this, EventArgs.Empty);
		}

		private void tbNewPresetName_Click(object sender, EventArgs e) {
			if(tbNewPresetName.Text == DEFAULT_PRESET_NAME || tbNewPresetName.Text == NO_PRESET_NAME) {
				tbNewPresetName.Text = "";
				tbNewPresetName.ForeColor = Color.Black;
			}
		}

		private void tbNewPresetName_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e) {
			if(e.KeyCode == Keys.Enter) {
				bAddPreset_Click(this, EventArgs.Empty);
				e.IsInputKey = true;
			}
		}
	}
}
