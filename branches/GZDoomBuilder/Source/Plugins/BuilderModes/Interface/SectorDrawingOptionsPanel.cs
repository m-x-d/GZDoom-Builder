#region Namespaces

using System;
using System.Windows.Forms;

#endregion

namespace CodeImp.DoomBuilder.BuilderModes.Interface
{
	public partial class SectorDrawingOptionsPanel : UserControl
	{
		#region Constructor / Setup

		public SectorDrawingOptionsPanel() {
			InitializeComponent();
		}

		public void Setup() {
			ceilHeight.Text = General.Map.Options.CustomCeilingHeight.ToString();
			floorHeight.Text = General.Map.Options.CustomFloorHeight.ToString();
			brightness.StepValues = General.Map.Config.BrightnessLevels;
			brightness.Text = General.Map.Options.CustomBrightness.ToString();
			ceiling.TextureName = General.Map.Options.DefaultCeilingTexture;
			floor.TextureName = General.Map.Options.DefaultFloorTexture;
			walls.TextureName = General.Map.Options.DefaultWallTexture;

			cbOverrideCeilingTexture.Checked = General.Map.Options.OverrideCeilingTexture;
			cbOverrideFloorTexture.Checked = General.Map.Options.OverrideFloorTexture;
			cbOverrideWallTexture.Checked = General.Map.Options.OverrideWallTexture;
			cbCeilHeight.Checked = General.Map.Options.OverrideCeilingHeight;
			cbFloorHeight.Checked = General.Map.Options.OverrideFloorHeight;
			cbBrightness.Checked = General.Map.Options.OverrideBrightness;

			ceiling.Enabled = cbOverrideCeilingTexture.Checked;
			floor.Enabled = cbOverrideFloorTexture.Checked;
			walls.Enabled = cbOverrideWallTexture.Checked;
			ceilHeight.Enabled = cbCeilHeight.Checked;
			floorHeight.Enabled = cbFloorHeight.Checked;
			brightness.Enabled = cbBrightness.Checked;
		}

		#endregion

		#region Checkbox Events

		private void cbOverrideCeilingTexture_CheckedChanged(object sender, EventArgs e) {
			ceiling.Enabled = cbOverrideCeilingTexture.Checked;
			General.Map.Options.OverrideCeilingTexture = cbOverrideCeilingTexture.Checked;
		}

		private void cbOverrideFloorTexture_CheckedChanged(object sender, EventArgs e) {
			floor.Enabled = cbOverrideFloorTexture.Checked;
			General.Map.Options.OverrideFloorTexture = cbOverrideFloorTexture.Checked;
		}

		private void cbOverrideWallTexture_CheckedChanged(object sender, EventArgs e) {
			walls.Enabled = cbOverrideWallTexture.Checked;
			General.Map.Options.OverrideWallTexture = cbOverrideWallTexture.Checked;
		}

		private void cbCeilHeight_CheckedChanged(object sender, EventArgs e) {
			ceilHeight.Enabled = cbCeilHeight.Checked;
			General.Map.Options.OverrideCeilingHeight = cbCeilHeight.Checked;
		}

		private void cbFloorHeight_CheckedChanged(object sender, EventArgs e) {
			floorHeight.Enabled = cbFloorHeight.Checked;
			General.Map.Options.OverrideFloorHeight = cbFloorHeight.Checked;
		}

		private void cbBrightness_CheckedChanged(object sender, EventArgs e) {
			brightness.Enabled = cbBrightness.Checked;
			General.Map.Options.OverrideBrightness = cbBrightness.Checked;
		}

		#endregion

		#region Inputs Events

		private void ceilHeight_WhenTextChanged(object sender, EventArgs e) {
			General.Map.Options.CustomCeilingHeight = ceilHeight.GetResult(General.Map.Options.CustomCeilingHeight);
		}

		private void floorHeight_WhenTextChanged(object sender, EventArgs e) {
			General.Map.Options.CustomFloorHeight = floorHeight.GetResult(General.Map.Options.CustomFloorHeight);
		}

		private void brightness_WhenTextChanged(object sender, EventArgs e) {
			General.Map.Options.CustomBrightness = General.Clamp(brightness.GetResult(General.Map.Options.CustomBrightness), 0, 255);
		}

		private void ceiling_OnValueChanged(object sender, EventArgs e) {
			General.Map.Options.DefaultCeilingTexture = ceiling.TextureName;
		}

		private void floor_OnValueChanged(object sender, EventArgs e) {
			General.Map.Options.DefaultFloorTexture = floor.TextureName;
		}

		private void walls_OnValueChanged(object sender, EventArgs e) {
			General.Map.Options.DefaultWallTexture = walls.TextureName;
		}

		#endregion

	}
}
