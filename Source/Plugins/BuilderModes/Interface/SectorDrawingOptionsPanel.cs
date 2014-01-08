#region Namespaces

using System;
using System.Windows.Forms;
using System.Collections.Generic;
using CodeImp.DoomBuilder.Map;

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
			top.TextureName = General.Map.Options.DefaultTopTexture;
			middle.TextureName = General.Map.Options.DefaultWallTexture;
			bottom.TextureName = General.Map.Options.DefaultBottomTexture;

			cbOverrideCeilingTexture.Checked = General.Map.Options.OverrideCeilingTexture;
			cbOverrideFloorTexture.Checked = General.Map.Options.OverrideFloorTexture;
			cbOverrideTopTexture.Checked = General.Map.Options.OverrideTopTexture;
			cbOverrideMiddleTexture.Checked = General.Map.Options.OverrideMiddleTexture;
			cbOverrideBottomTexture.Checked = General.Map.Options.OverrideBottomTexture;
			cbCeilHeight.Checked = General.Map.Options.OverrideCeilingHeight;
			cbFloorHeight.Checked = General.Map.Options.OverrideFloorHeight;
			cbBrightness.Checked = General.Map.Options.OverrideBrightness;

			ceiling.Enabled = cbOverrideCeilingTexture.Checked;
			floor.Enabled = cbOverrideFloorTexture.Checked;
			top.Enabled = cbOverrideTopTexture.Checked;
			middle.Enabled = cbOverrideMiddleTexture.Checked;
			bottom.Enabled = cbOverrideBottomTexture.Checked;
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

		private void cbOverrideTopTexture_CheckedChanged(object sender, EventArgs e) {
			top.Enabled = cbOverrideTopTexture.Checked;
			General.Map.Options.OverrideTopTexture = cbOverrideTopTexture.Checked;
		}

		private void cbOverrideMiddleTexture_CheckedChanged(object sender, EventArgs e) {
			middle.Enabled = cbOverrideMiddleTexture.Checked;
			General.Map.Options.OverrideMiddleTexture = cbOverrideMiddleTexture.Checked;
		}

		private void cbOverrideBottomTexture_CheckedChanged(object sender, EventArgs e) {
			bottom.Enabled = cbOverrideBottomTexture.Checked;
			General.Map.Options.OverrideBottomTexture = cbOverrideBottomTexture.Checked;
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

		private void top_OnValueChanged(object sender, EventArgs e) {
			General.Map.Options.DefaultTopTexture = top.TextureName;
		}

		private void middle_OnValueChanged(object sender, EventArgs e) {
			General.Map.Options.DefaultWallTexture = middle.TextureName;
		}

		private void bottom_OnValueChanged(object sender, EventArgs e) {
			General.Map.Options.DefaultBottomTexture = bottom.TextureName;
		}

		#endregion

		#region Texture Fill

		private void fillceiling_Click(object sender, EventArgs e) {
			ICollection<Sector> sectors = General.Map.Map.GetSelectedSectors(true);
			if(sectors.Count == 0) return;

			string undodesc = "sector";
			if(sectors.Count > 1) undodesc = sectors.Count + " sectors";
			General.Map.UndoRedo.CreateUndo("Clear ceiling texture from " + undodesc);

			foreach(Sector s in sectors) {
				s.SetCeilTexture(ceiling.TextureName);
			}

			// Update the used textures
			General.Map.Data.UpdateUsedTextures();
			General.Map.IsChanged = true;

			if(General.Map.Renderer2D.ViewMode == CodeImp.DoomBuilder.Rendering.ViewMode.CeilingTextures)
				General.Interface.RedrawDisplay();
		}

		private void fillfloor_Click(object sender, EventArgs e) {
			ICollection<Sector> sectors = General.Map.Map.GetSelectedSectors(true);
			if(sectors.Count == 0) return;

			string undodesc = "sector";
			if(sectors.Count > 1) undodesc = sectors.Count + " sectors";
			General.Map.UndoRedo.CreateUndo("Clear ceiling texture from " + undodesc);

			foreach(Sector s in sectors) {
				s.SetFloorTexture(floor.TextureName);
			}

			// Update the used textures
			General.Map.Data.UpdateUsedTextures();
			General.Map.IsChanged = true;

			if(General.Map.Renderer2D.ViewMode == CodeImp.DoomBuilder.Rendering.ViewMode.FloorTextures)
				General.Interface.RedrawDisplay();
		}

		private void fillupper_Click(object sender, EventArgs e) {
			ICollection<Linedef> lines = General.Map.Map.GetSelectedLinedefs(true);
			if(lines.Count == 0) return;

			string undodesc = "linedef";
			if(lines.Count > 1) undodesc = lines.Count + " linedefs";
			General.Map.UndoRedo.CreateUndo("Fill upper texture for " + undodesc);

			foreach(Linedef l in lines) {
				if(l.Front != null && l.Front.HighRequired()) l.Front.SetTextureHigh(top.TextureName);
				if(l.Back != null && l.Back.HighRequired()) l.Back.SetTextureHigh(top.TextureName);
			}

			// Update the used textures
			General.Map.Data.UpdateUsedTextures();
			General.Map.IsChanged = true;
		}

		private void fillmiddle_Click(object sender, EventArgs e) {
			ICollection<Linedef> lines = General.Map.Map.GetSelectedLinedefs(true);
			if(lines.Count == 0) return;

			string undodesc = "linedef";
			if(lines.Count > 1) undodesc = lines.Count + " linedefs";
			General.Map.UndoRedo.CreateUndo("Fill middle texture for " + undodesc);

			foreach(Linedef l in lines) {
				if(l.Front != null && l.Front.MiddleRequired()) l.Front.SetTextureMid(middle.TextureName);
				if(l.Back != null && l.Back.MiddleRequired()) l.Back.SetTextureMid(middle.TextureName);
			}

			// Update the used textures
			General.Map.Data.UpdateUsedTextures();
			General.Map.IsChanged = true;
		}

		private void filllower_Click(object sender, EventArgs e) {
			ICollection<Linedef> lines = General.Map.Map.GetSelectedLinedefs(true);
			if(lines.Count == 0) return;

			string undodesc = "linedef";
			if(lines.Count > 1) undodesc = lines.Count + " linedefs";
			General.Map.UndoRedo.CreateUndo("Fill lower texture for " + undodesc);

			foreach(Linedef l in lines) {
				if(l.Front != null && l.Front.LowRequired()) l.Front.SetTextureLow(bottom.TextureName);
				if(l.Back != null && l.Back.LowRequired()) l.Back.SetTextureLow(bottom.TextureName);
			}

			// Update the used textures
			General.Map.Data.UpdateUsedTextures();
			General.Map.IsChanged = true;
		}

		#endregion

		#region Clear Textures

		private void clearceiling_Click(object sender, EventArgs e) {
			ICollection<Sector> sectors = General.Map.Map.GetSelectedSectors(true);
			if(sectors.Count == 0) return;

			string undodesc = "sector";
			if(sectors.Count > 1) undodesc = sectors.Count + " sectors";
			General.Map.UndoRedo.CreateUndo("Clear ceiling texture from " + undodesc);

			foreach(Sector s in sectors){
				s.SetCeilTexture("-");
			}

			// Update the used textures
			General.Map.Data.UpdateUsedTextures();
			General.Map.IsChanged = true;

			// Update entire display
			General.Map.Map.Update();
			if(General.Map.Renderer2D.ViewMode == CodeImp.DoomBuilder.Rendering.ViewMode.CeilingTextures)
				General.Interface.RedrawDisplay();
		}

		private void clearfloor_Click(object sender, EventArgs e) {
			ICollection<Sector> sectors = General.Map.Map.GetSelectedSectors(true);
			if(sectors.Count == 0) return;

			string undodesc = "sector";
			if(sectors.Count > 1) undodesc = sectors.Count + " sectors";
			General.Map.UndoRedo.CreateUndo("Clear floor texture from " + undodesc);

			foreach(Sector s in sectors) {
				s.SetFloorTexture("-");
			}

			// Update the used textures
			General.Map.Data.UpdateUsedTextures();
			General.Map.IsChanged = true;

			// Update entire display
			General.Map.Map.Update();
			if(General.Map.Renderer2D.ViewMode == CodeImp.DoomBuilder.Rendering.ViewMode.FloorTextures)
				General.Interface.RedrawDisplay();
		}

		private void clearupper_Click(object sender, EventArgs e) {
			ICollection<Linedef> lines = General.Map.Map.GetSelectedLinedefs(true);
			if(lines.Count == 0) return;

			string undodesc = "linedef";
			if(lines.Count > 1) undodesc = lines.Count + " linedefs";
			General.Map.UndoRedo.CreateUndo("Clear upper texture from " + undodesc);

			foreach(Linedef l in lines) {
				if(l.Front != null && l.Front.HighTexture != "-") l.Front.SetTextureHigh("-");
				if(l.Back != null && l.Back.HighTexture != "-") l.Back.SetTextureHigh("-");
			}

			// Update the used textures
			General.Map.Data.UpdateUsedTextures();
			General.Map.IsChanged = true;
		}

		private void clearmiddle_Click(object sender, EventArgs e) {
			ICollection<Linedef> lines = General.Map.Map.GetSelectedLinedefs(true);
			if(lines.Count == 0) return;

			string undodesc = "linedef";
			if(lines.Count > 1) undodesc = lines.Count + " linedefs";
			General.Map.UndoRedo.CreateUndo("Clear middle texture from " + undodesc);

			foreach(Linedef l in lines) {
				if(l.Front != null && l.Front.MiddleTexture != "-") l.Front.SetTextureMid("-");
				if(l.Back != null && l.Back.MiddleTexture != "-") l.Back.SetTextureMid("-");
			}

			// Update the used textures
			General.Map.Data.UpdateUsedTextures();
			General.Map.IsChanged = true;
		}

		private void clearlower_Click(object sender, EventArgs e) {
			ICollection<Linedef> lines = General.Map.Map.GetSelectedLinedefs(true);
			if(lines.Count == 0) return;

			string undodesc = "linedef";
			if(lines.Count > 1) undodesc = lines.Count + " linedefs";
			General.Map.UndoRedo.CreateUndo("Clear lower texture from " + undodesc);

			foreach(Linedef l in lines) {
				if(l.Front != null && l.Front.LowTexture != "-") l.Front.SetTextureLow("-");
				if(l.Back != null && l.Back.LowTexture != "-") l.Back.SetTextureLow("-");
			}

			// Update the used textures
			General.Map.Data.UpdateUsedTextures();
			General.Map.IsChanged = true;
		}

		#endregion

	}
}
