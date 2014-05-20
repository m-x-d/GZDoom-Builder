using System;
using System.Collections.Generic;
using System.Windows.Forms;
using CodeImp.DoomBuilder.Geometry;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.VisualModes;
using CodeImp.DoomBuilder.Windows;

namespace CodeImp.DoomBuilder.BuilderEffects
{
	public partial class JitterSectorsForm : Form
	{
		private readonly string editingModeName;
		private readonly List<VisualSector> visualSectors;
		private readonly VertexData[] vertexData;
		private readonly List<SectorData> sectorData;
		private readonly List<SidedefData> sidedefData;
		private readonly int MaxSafeDistance;
		private readonly int MaxSafeHeightDistance;

		//settings
		private static bool keepExistingSideTextures = true;

		private struct VertexData
		{
			public Vertex Vertex;
			public Vector2D InitialPosition;
			public int SafeDistance;
			public float JitterAngle;
		}

		private struct SectorData
		{
			public Sector Sector;
			public int InitialCeilingHeight;
			public int InitialFloorHeight;
			public int SafeDistance;
			public float JitterFloorHeight;
			public float JitterCeilingHeight;
		}

		private struct SidedefData
		{
			public Sidedef Side;
			public string HighTexture;
			public string OtherLowTexture;
			public string OtherHighTexture;
			public string LowTexture;
			public bool PegTop;
			public bool PegBottom;
			public bool UpdateTextureOnOtherSide;
		}
		
		public JitterSectorsForm(string editingModeName) {
			this.editingModeName = editingModeName;

			InitializeComponent();

			//get selection
			List<Vertex> verts = new List<Vertex>();
			List<Sector> sectors = new List<Sector>();

			if(editingModeName == "BaseVisualMode") {
				VisualMode vm = (VisualMode)General.Editing.Mode;
				List<VisualGeometry> visualGeometry = vm.GetSelectedSurfaces();
				visualSectors = new List<VisualSector>();

				//get selected visual and regular sectors
				foreach(VisualGeometry vg in visualGeometry) {
					if(vg.GeometryType != VisualGeometryType.CEILING && vg.GeometryType != VisualGeometryType.FLOOR)
						continue;

					if(vg.Sector != null && vg.Sector.Sector != null) {
						foreach(Sidedef sd in vg.Sector.Sector.Sidedefs) {
							if(!verts.Contains(sd.Line.Start))
								verts.Add(sd.Line.Start);
							if(!verts.Contains(sd.Line.End))
								verts.Add(sd.Line.End);
						}

						sectors.Add(vg.Sector.Sector);
						visualSectors.Add(vg.Sector);
					}
				}

				//also get visual sectors around selected ones (because they also may be affected)
				List<Vertex> affectedVerts = new List<Vertex>();

				foreach(Sector s in sectors) {
					foreach(Sidedef sd in s.Sidedefs) {
						if(!affectedVerts.Contains(sd.Line.Start))
							affectedVerts.Add(sd.Line.Start);
						if(!affectedVerts.Contains(sd.Line.End))
							affectedVerts.Add(sd.Line.End);
					}
				}

				List<Sector> affectedSectors = new List<Sector>();
				foreach(Vertex v in affectedVerts) {
					foreach(Linedef l in v.Linedefs) {
						if(l.Front != null && !sectors.Contains(l.Front.Sector) && !affectedSectors.Contains(l.Front.Sector) && vm.VisualSectorExists(l.Front.Sector)) {
							visualSectors.Add(vm.GetVisualSector(l.Front.Sector));
							affectedSectors.Add(l.Front.Sector);
						}
						if(l.Back != null && !sectors.Contains(l.Back.Sector) && !affectedSectors.Contains(l.Back.Sector) && vm.VisualSectorExists(l.Back.Sector)) {
							visualSectors.Add(vm.GetVisualSector(l.Back.Sector));
							affectedSectors.Add(l.Back.Sector);
						}
					}
				}

			} else if(editingModeName == "SectorsMode") {
				ICollection<Sector> list = General.Map.Map.GetSelectedSectors(true);

				foreach(Sector s in list) {
					foreach(Sidedef sd in s.Sidedefs) {
						if(!verts.Contains(sd.Line.Start))
							verts.Add(sd.Line.Start);
						if(!verts.Contains(sd.Line.End))
							verts.Add(sd.Line.End);
					}
					sectors.Add(s);
				}

			}

			if(verts.Count == 0 || sectors.Count == 0) {
				General.Interface.DisplayStatus(StatusType.Warning, "Unable to get sectors from selection!");
				return;
			}

			//update window header
			this.Text = "Randomize " + sectors.Count + (sectors.Count > 1 ? " sectors" : " sector");

			//store intial properties
//process verts...
			Dictionary<Vertex, VertexData> data = new Dictionary<Vertex, VertexData>();

			foreach(Vertex v in verts) {
				VertexData vd = new VertexData();
				vd.Vertex = v;
				vd.InitialPosition = v.Position;
				data.Add(v, vd);
			}

			foreach(Vertex v in verts) {
				if(v.Linedefs == null) continue;

				//get nearest linedef
				Linedef closestLine = null;
				float distance = float.MaxValue;

				// Go for all linedefs in selection
				foreach(Linedef l in General.Map.Map.Linedefs) {
					if(v.Linedefs.Contains(l)) continue;

					// Calculate distance and check if closer than previous find
					float d = l.SafeDistanceToSq(v.Position, true);
					if(d < distance) {
						// This one is closer
						closestLine = l;
						distance = d;
					}
				}

				if(closestLine == null) continue;

				float closestLineDistance = Vector2D.Distance(v.Position, closestLine.NearestOnLine(v.Position));

				//check SafeDistance of closest line
				if(data.ContainsKey(closestLine.Start) && data[closestLine.Start].SafeDistance > closestLineDistance) {
					VertexData vd = data[closestLine.Start];
					vd.SafeDistance = (int)Math.Floor(closestLineDistance);
					data[closestLine.Start] = vd;
				}
				if(data.ContainsKey(closestLine.End) && data[closestLine.End].SafeDistance > closestLineDistance) {
					VertexData vd = data[closestLine.End];
					vd.SafeDistance = (int)Math.Floor(closestLineDistance);
					data[closestLine.End] = vd;
				}

				//save SafeDistance
				int dist = (int)Math.Floor(closestLineDistance);
				if(data[v].SafeDistance == 0 || data[v].SafeDistance > dist) {
					VertexData vd = data[v];
					vd.SafeDistance = dist;
					data[v] = vd;
				}
			}

			//store properties
			vertexData = new VertexData[data.Values.Count];
			data.Values.CopyTo(vertexData, 0);

			for(int i = 0; i < vertexData.Length; i++) {
				if(vertexData[i].SafeDistance > 0)
					vertexData[i].SafeDistance /= 2;
				if(MaxSafeDistance < vertexData[i].SafeDistance)
					MaxSafeDistance = vertexData[i].SafeDistance;
			}

//process sectors and linedes
			sectorData = new List<SectorData>();
			sidedefData = new List<SidedefData>();

			foreach(Sector s in sectors){
				SectorData sd = new SectorData();

				sd.Sector = s;
				sd.InitialCeilingHeight = s.CeilHeight;
				sd.InitialFloorHeight = s.FloorHeight;
				sd.SafeDistance = (s.CeilHeight - s.FloorHeight) / 2;
				if(sd.SafeDistance > MaxSafeHeightDistance)	MaxSafeHeightDistance = sd.SafeDistance;
				sectorData.Add(sd);

				foreach(Sidedef side in s.Sidedefs) {
					//store initial sidedef properties
					SidedefData sdd = new SidedefData();

					sdd.Side = side;
					sdd.LowTexture = side.LowTexture;
					sdd.HighTexture = side.HighTexture;
					sdd.PegBottom = side.Line.IsFlagSet(General.Map.Config.LowerUnpeggedFlag);
					sdd.PegTop = side.Line.IsFlagSet(General.Map.Config.UpperUnpeggedFlag);

					if(side.Other != null && !sectors.Contains(side.Other.Sector)) {
						sdd.UpdateTextureOnOtherSide = true;
						sdd.OtherHighTexture = side.Other.HighTexture;
						sdd.OtherLowTexture = side.Other.LowTexture;
					}

					sidedefData.Add(sdd);
				}
			}

			positionJitterAmmount.Maximum = MaxSafeDistance;
			floorHeightAmmount.Maximum = MaxSafeHeightDistance;
			ceilingHeightAmmount.Maximum = MaxSafeHeightDistance;

			//set editing settings
			cbKeepExistingTextures.Checked = keepExistingSideTextures;

			//texture pickers
			textureLower.Initialize();
			textureUpper.Initialize();
			textureLower.TextureName = General.Settings.DefaultFloorTexture;
			textureUpper.TextureName = General.Settings.DefaultCeilingTexture;

			cbUpperTexStyle.SelectedIndex = 0;
			cbLowerTexStyle.SelectedIndex = 0;
			updateTextureSelectors(); //update interface

			//create random values
			updateAngles(); 
			updateFloorHeights();
			updateCeilingHeights();

			//create undo
			General.Map.UndoRedo.ClearAllRedos();
			General.Map.UndoRedo.CreateUndo("Randomize " + sectors.Count + (sectors.Count > 1 ? " sectors" : " sector"));
		}

//utility
		private void applyTranslationJitter(int ammount) {
			int curAmmount;

			for(int i = 0; i < vertexData.Length; i++) {
				curAmmount = ammount > vertexData[i].SafeDistance ? vertexData[i].SafeDistance : ammount;
				vertexData[i].Vertex.Move(new Vector2D(vertexData[i].InitialPosition.x + (int)(Math.Sin(vertexData[i].JitterAngle) * curAmmount), vertexData[i].InitialPosition.y + (int)(Math.Cos(vertexData[i].JitterAngle) * curAmmount)));
			}

			//update view
			if(editingModeName == "BaseVisualMode") {
				General.Map.Map.Update();
				General.Map.IsChanged = true;
				updateVisualGeometry();
			} else {
				General.Interface.RedrawDisplay();
			}
		}

		private void applyCeilingHeightJitter(int ammount) {
			int curAmmount;

			for(int i = 0; i < sectorData.Count; i++) {
				curAmmount = ammount > sectorData[i].SafeDistance ? sectorData[i].SafeDistance : ammount;
				sectorData[i].Sector.CeilHeight = sectorData[i].InitialCeilingHeight - (int)Math.Floor(curAmmount * sectorData[i].JitterCeilingHeight);
			}

			//update view
			if(editingModeName == "BaseVisualMode") {
				General.Map.Map.Update();
				General.Map.IsChanged = true;
				updateVisualGeometry();
			}

			updateUpperTextures(cbUpperTexStyle.SelectedIndex, false);
		}

		private void applyFloorHeightJitter(int ammount) {
			int curAmmount;

			for(int i = 0; i < sectorData.Count; i++) {
				curAmmount = ammount > sectorData[i].SafeDistance ? sectorData[i].SafeDistance : ammount;
				sectorData[i].Sector.FloorHeight = sectorData[i].InitialFloorHeight + (int)Math.Floor(curAmmount * sectorData[i].JitterFloorHeight);
			}

			//update view
			if(editingModeName == "BaseVisualMode") {
				General.Map.Map.Update();
				General.Map.IsChanged = true;
				updateVisualGeometry();
			}

			updateLowerTextures(cbLowerTexStyle.SelectedIndex, false);
		}

		private void updateVisualGeometry() {
			foreach(VisualSector vs in visualSectors) vs.UpdateSectorGeometry(true);
			foreach(VisualSector vs in visualSectors) vs.UpdateSectorData();
			foreach(VisualSector vs in visualSectors) vs.UpdateSectorData();
		}

		private void updateTextureSelectors() {
			cbLowerTexStyle.Enabled = floorHeightAmmount.Value > 0;
			cbUpperTexStyle.Enabled = ceilingHeightAmmount.Value > 0;
			gbLowerTexture.Enabled = floorHeightAmmount.Value > 0 && cbLowerTexStyle.SelectedIndex > 0;
			gbUpperTexture.Enabled = ceilingHeightAmmount.Value > 0 && cbUpperTexStyle.SelectedIndex > 0;
			textureLower.Enabled = floorHeightAmmount.Value > 0 && cbLowerTexStyle.SelectedIndex == 2;
			textureUpper.Enabled = ceilingHeightAmmount.Value > 0 && cbUpperTexStyle.SelectedIndex == 2;
		}

		private void updateUpperTextures(int index, bool updateGeometry) {
			if(index == -1) return;

			if(index == 0) { //revert
				foreach(SidedefData sd in sidedefData)
					setUpperTexture(sd, sd.HighTexture);
			} else if(index == 1) { //use ceiling texture
				foreach(SidedefData sd in sidedefData) {
					if(sd.Side.Sector != null) {
						if(sd.UpdateTextureOnOtherSide && sd.Side.Other.Sector != null)
							setUpperTexture(sd, sd.Side.Sector.CeilTexture, sd.Side.Other.Sector.CeilTexture);
						else
							setUpperTexture(sd, sd.Side.Sector.CeilTexture);
					}
				}
			} else if(index == 2) { //use given texture
				foreach(SidedefData sd in sidedefData)
					setUpperTexture(sd, textureUpper.TextureName);
			}

			General.Map.Data.UpdateUsedTextures();
			if(updateGeometry && editingModeName == "BaseVisualMode") updateVisualGeometry();
		}

		private void updateLowerTextures(int index, bool updateGeometry) {
			if(index == -1) return;

			if(index == 0) { //revert
				foreach(SidedefData sd in sidedefData)
					setLowerTexture(sd, sd.LowTexture);
			} else if(index == 1) { //use floor texture
				foreach(SidedefData sd in sidedefData) {
					if(sd.Side.Sector != null) {
						if(sd.UpdateTextureOnOtherSide && sd.Side.Other.Sector != null)
							setLowerTexture(sd, sd.Side.Sector.FloorTexture, sd.Side.Other.Sector.FloorTexture);
						else
							setLowerTexture(sd, sd.Side.Sector.FloorTexture);
					}
				}
			} else if(index == 2) { //use given texture
				foreach(SidedefData sd in sidedefData)
					setLowerTexture(sd, textureLower.TextureName);
			}

			General.Map.Data.UpdateUsedTextures();
			if(updateGeometry && editingModeName == "BaseVisualMode") updateVisualGeometry();
		}

//set textures
		private void setUpperTexture(SidedefData sd, string textureName) {
			setUpperTexture(sd, textureName, textureName);
		}

		private static void setUpperTexture(SidedefData sd, string textureName, string otherTextureName) {
			if(!keepExistingSideTextures || string.IsNullOrEmpty(sd.HighTexture) || sd.HighTexture == "-")
				sd.Side.SetTextureHigh(textureName);

			if(sd.UpdateTextureOnOtherSide && sd.Side.Other != null && (!keepExistingSideTextures || string.IsNullOrEmpty(sd.OtherHighTexture) || sd.OtherHighTexture == "-"))
				sd.Side.Other.SetTextureHigh(otherTextureName);
		}

		private void setLowerTexture(SidedefData sd, string textureName) {
			setLowerTexture(sd, textureName, textureName);
		}

		private static void setLowerTexture(SidedefData sd, string textureName, string otherTextureName) {
			if(!keepExistingSideTextures || string.IsNullOrEmpty(sd.LowTexture) || sd.LowTexture == "-")
				sd.Side.SetTextureLow(textureName);

			if(sd.UpdateTextureOnOtherSide && sd.Side.Other != null && (!keepExistingSideTextures || string.IsNullOrEmpty(sd.OtherLowTexture) || sd.OtherLowTexture == "-")) 
				sd.Side.Other.SetTextureLow(otherTextureName);
		}

//jitter generation
		private void updateAngles() {
			for(int i = 0; i < vertexData.Length; i++) {
				VertexData vd = vertexData[i];
				vd.JitterAngle = (float)(General.Random(0, 359) * Math.PI / 180f);
				vertexData[i] = vd;
			}
		}

		private void updateFloorHeights() {
			for(int i = 0; i < sectorData.Count; i++) {
				SectorData sd = sectorData[i];
				sd.JitterFloorHeight = General.Random(-100, 100) / 100f;
				sectorData[i] = sd;
			}
		}

		private void updateCeilingHeights() {
			for(int i = 0; i < sectorData.Count; i++) {
				SectorData sd = sectorData[i];
				sd.JitterCeilingHeight = General.Random(-100, 100) / 100f;
				sectorData[i] = sd;
			}
		}

//EVENTS
		private void bApply_Click(object sender, EventArgs e) {
			//clean unused sidedef textures
			foreach(SidedefData sd in sidedefData) {
				sd.Side.RemoveUnneededTextures(false);

				if(sd.UpdateTextureOnOtherSide)
					sd.Side.Other.RemoveUnneededTextures(false);
			}

			General.Map.Map.ClearAllSelected();

			// Update cached values
			General.Map.Map.Update();
			General.Map.IsChanged = true;

			if(editingModeName != "BaseVisualMode")
				General.Interface.RedrawDisplay();

			this.DialogResult = DialogResult.OK;
			Close();
		}

		private void bCancel_Click(object sender, EventArgs e) {
			this.DialogResult = DialogResult.Cancel;
			Close();
		}

		private void JitterSectorsForm_FormClosing(object sender, FormClosingEventArgs e) {
			if(this.DialogResult == DialogResult.Cancel)
				General.Map.UndoRedo.WithdrawUndo(); //undo changes
		}

		private void positionJitterAmmount_OnValueChanging(object sender, EventArgs e) {
			applyTranslationJitter(positionJitterAmmount.Value);
		}

		private void ceilingHeightAmmount_OnValueChanging(object sender, EventArgs e) {
			applyCeilingHeightJitter(ceilingHeightAmmount.Value);
			updateTextureSelectors();
		}

		private void floorHeightAmmount_OnValueChanging(object sender, EventArgs e) {
			applyFloorHeightJitter(floorHeightAmmount.Value);
			updateTextureSelectors();
		}

		private void cbKeepExistingTextures_CheckedChanged(object sender, EventArgs e) {
			keepExistingSideTextures = cbKeepExistingTextures.Checked;

			//revert possible changes
			if(keepExistingSideTextures) {
				foreach(SidedefData sd in sidedefData) {
					if(!string.IsNullOrEmpty(sd.HighTexture))
						sd.Side.SetTextureHigh(sd.HighTexture);

					if(sd.UpdateTextureOnOtherSide && !string.IsNullOrEmpty(sd.OtherHighTexture))
						sd.Side.Other.SetTextureHigh(sd.OtherHighTexture);
				}

				if(editingModeName == "BaseVisualMode")	updateVisualGeometry();
			} else {
				updateLowerTextures(cbLowerTexStyle.SelectedIndex, false);
				updateUpperTextures(cbUpperTexStyle.SelectedIndex, true);
			}
		}

//update buttons
		private void bUpdateTranslation_Click(object sender, EventArgs e) {
			updateAngles();
			applyTranslationJitter(positionJitterAmmount.Value);
		}

		private void bUpdateCeilingHeight_Click(object sender, EventArgs e) {
			updateCeilingHeights();
			applyCeilingHeightJitter(ceilingHeightAmmount.Value);
		}

		private void bUpdateFloorHeight_Click(object sender, EventArgs e) {
			updateFloorHeights();
			applyFloorHeightJitter(floorHeightAmmount.Value);
		}

//texture pegging
		private void cbPegTop_CheckedChanged(object sender, EventArgs e) {
			if(cbPegTop.Checked) { //apply flag
				foreach(SidedefData sd in sidedefData)
					sd.Side.Line.SetFlag(General.Map.Config.UpperUnpeggedFlag, true);
			} else { //revert to initial setting
				foreach(SidedefData sd in sidedefData)
					sd.Side.Line.SetFlag(General.Map.Config.UpperUnpeggedFlag, sd.PegTop);
			}

			if(editingModeName == "BaseVisualMode") {
				General.Map.Data.UpdateUsedTextures();
				updateVisualGeometry();
			}
		}

		private void cbPegBottom_CheckedChanged(object sender, EventArgs e) {
			if(cbPegBottom.Checked) { //apply flag
				foreach(SidedefData sd in sidedefData)
					sd.Side.Line.SetFlag(General.Map.Config.LowerUnpeggedFlag, true);
			} else { //revert to initial setting
				foreach(SidedefData sd in sidedefData)
					sd.Side.Line.SetFlag(General.Map.Config.LowerUnpeggedFlag, sd.PegBottom);
			}

			if(editingModeName == "BaseVisualMode") {
				General.Map.Data.UpdateUsedTextures();
				updateVisualGeometry();
			}
		}

//texture pickers
		private void textureLower_OnValueChanged(object sender, EventArgs e) {
			updateLowerTextures(cbLowerTexStyle.SelectedIndex, true);
		}

		private void textureUpper_OnValueChanged(object sender, EventArgs e) {
			updateUpperTextures(cbUpperTexStyle.SelectedIndex, true);
		}

//texture style selectors
		private void cbUpperTexStyle_SelectedIndexChanged(object sender, EventArgs e) {
			updateUpperTextures(cbUpperTexStyle.SelectedIndex, true);
			updateTextureSelectors();
		}

		private void cbLowerTexStyle_SelectedIndexChanged(object sender, EventArgs e) {
			updateLowerTextures(cbLowerTexStyle.SelectedIndex, true);
			updateTextureSelectors();
		}

//HALP!
		private void JitterSectorsForm_HelpRequested(object sender, HelpEventArgs hlpevent) {
			General.ShowHelp("gzdb/features/all_modes/jitter.html");
			hlpevent.Handled = true;
		}
	}
}
