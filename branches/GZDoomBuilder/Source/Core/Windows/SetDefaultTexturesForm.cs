using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace CodeImp.DoomBuilder.Windows
{
	public partial class SetCurrentTexturesForm : Form
	{
		public SetCurrentTexturesForm() {
			this.Location = new Point(Cursor.Position.X - this.Width / 2, Cursor.Position.Y - this.Height / 2);
			InitializeComponent();

			// Initialize image selectors
			ceiling.Initialize();
			floor.Initialize();
			walls.Initialize();

			ceiling.TextureName = General.Settings.DefaultCeilingTexture;
			floor.TextureName = General.Settings.DefaultFloorTexture;
			walls.TextureName = General.Settings.DefaultTexture;
			cbForceDefault.Checked = General.Settings.GZForceDefaultTextures;
			cbForceDefault_CheckedChanged(this, EventArgs.Empty);
		}

		private void bCancel_Click(object sender, EventArgs e) {
			Close();
		}

		private void bApply_Click(object sender, EventArgs e) {
			if(General.Map.Data.GetTextureExists(ceiling.TextureName))
				General.Settings.DefaultCeilingTexture = ceiling.TextureName;

			if(General.Map.Data.GetTextureExists(floor.TextureName))
				General.Settings.DefaultFloorTexture = floor.TextureName;

			if(General.Map.Data.GetTextureExists(walls.TextureName))
				General.Settings.DefaultTexture = walls.TextureName;

			General.Settings.GZForceDefaultTextures = cbForceDefault.Checked;
			Close();
		}

		private void cbForceDefault_CheckedChanged(object sender, EventArgs e) {
			ceiling.Enabled = cbForceDefault.Checked;
			floor.Enabled = cbForceDefault.Checked;
			walls.Enabled = cbForceDefault.Checked;
			labelCeiling.Enabled = cbForceDefault.Checked;
			labelFloor.Enabled = cbForceDefault.Checked;
			labelWalls.Enabled = cbForceDefault.Checked;
		}
	}
}
