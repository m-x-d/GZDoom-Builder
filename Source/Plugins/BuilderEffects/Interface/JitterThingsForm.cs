using System;
using System.Collections.Generic;
using System.Windows.Forms;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.VisualModes;
using CodeImp.DoomBuilder.Geometry;

namespace CodeImp.DoomBuilder.BuilderEffects
{
	public partial class JitterThingsForm : Form
	{
		private string editingModeName;
		private List<Thing> selection;
		private List<VisualThing> visualSelection;
		private List<ThingData> thingData;
		private int MaxSafeDistance;
		private int MaxSafeHeightDistance;

		private static bool relativePosition;
		private static bool relativeHeight;

		private struct ThingData {
			public Vector3D Position;
			public int Angle;
			public int SectorHeight;
			public int ZOffset;
			public int SafeDistance;
			public int JitterAngle; //position jitter angle, not Thing angle!
			public float JitterRotation; //Thing angle
			public float JitterHeight;
		}

		public JitterThingsForm(string editingModeName) {
			this.editingModeName = editingModeName;
			this.HelpRequested += new HelpEventHandler(JitterThingsForm_HelpRequested);

			InitializeComponent();

			//have thing height?
			heightJitterAmmount.Enabled = General.Map.FormatInterface.HasThingHeight;
			bUpdateHeight.Enabled = General.Map.FormatInterface.HasThingHeight;

			//get selection
			selection = new List<Thing>();

			if(editingModeName == "BaseVisualMode") {
				visualSelection = ((VisualMode)General.Editing.Mode).GetSelectedVisualThings(false);
				foreach(VisualThing t in visualSelection)
					selection.Add(t.Thing);
			} else {
				ICollection<Thing> list = General.Map.Map.GetSelectedThings(true);
				foreach(Thing t in list)
					selection.Add(t);
			}

			//update window header
			this.Text = "Jitter Transform (" + selection.Count + " thing" + (selection.Count > 1 ? "s" : "") + ")";

			//store intial properties
			thingData = new List<ThingData>();

			foreach(Thing t in selection){
				ThingData d = new ThingData();

				Thing closest = MapSet.NearestThing(General.Map.Map.Things, t);

				if(closest != null){
					d.SafeDistance = (int)Math.Round(Vector2D.Distance(t.Position, closest.Position));
				}else{
					d.SafeDistance = 512;
				}

				if(d.SafeDistance > 0) d.SafeDistance /= 2;
				if(MaxSafeDistance < d.SafeDistance) MaxSafeDistance = d.SafeDistance;
				d.Position = t.Position;
				d.Angle = t.AngleDoom;

				if(General.Map.FormatInterface.HasThingHeight) {
					if(t.Sector == null) t.DetermineSector();
					if(t.Sector == null) continue;

					d.SectorHeight = Math.Max(0, t.Sector.CeilHeight - (int)General.Map.Data.GetThingInfo(t.Type).Height - t.Sector.FloorHeight);
					if(MaxSafeHeightDistance < d.SectorHeight) MaxSafeHeightDistance = d.SectorHeight;
					d.ZOffset = (int)t.Position.z;
				}

				thingData.Add(d);
			}

			updateAngles();
			updateHeights();
			updateRotationAngles();

			//set editing mode
			cbRelativePos.Checked = relativePosition;
			cbRelativePos_CheckedChanged(this, EventArgs.Empty);
			cbRelativeHeight.Checked = relativeHeight;
			cbRelativeHeight_CheckedChanged(this, EventArgs.Empty);

			//create undo
			General.Map.UndoRedo.ClearAllRedos();
			General.Map.UndoRedo.CreateUndo("Jitter Transform (" + selection.Count + " thing" + (selection.Count > 1 ? "s)" : ")"));

			//tricky way to actually store undo information...
			foreach(Thing t in selection) t.Move(t.Position);
		}

//utility
		private void applyTranslationJitter(int ammount) {
			Random rndX = new Random();
			Random rndY = new Random();
			int curAmmount;

			if(relativePosition) {
				for(int i = 0; i < selection.Count; i++) {
					curAmmount = (int)Math.Round(ammount * (thingData[i].SafeDistance / 100f));
					selection[i].Move(new Vector2D(thingData[i].Position.x + (int)(Math.Sin(thingData[i].JitterAngle) * curAmmount), thingData[i].Position.y + (int)(Math.Cos(thingData[i].JitterAngle) * curAmmount)));
					selection[i].DetermineSector();
				}
			} else {
				for(int i = 0; i < selection.Count; i++) {
					curAmmount = ammount > thingData[i].SafeDistance ? thingData[i].SafeDistance : ammount;
					selection[i].Move(new Vector2D(thingData[i].Position.x + (int)(Math.Sin(thingData[i].JitterAngle) * curAmmount), thingData[i].Position.y + (int)(Math.Cos(thingData[i].JitterAngle) * curAmmount)));
					selection[i].DetermineSector();
				}
			}

			updateGeometry();
		}

		private void applyRotationJitter(int ammount) {
			Random rnd = new Random();

			for(int i = 0; i < selection.Count; i++)
				selection[i].Rotate((int)((thingData[i].Angle + ammount * thingData[i].JitterRotation) % 360));

			//update view
			if(editingModeName == "ThingsMode")
				General.Interface.RedrawDisplay();
		}

		private void applyHeightJitter(int ammount) {
			int curAmmount;
			float a = ammount / 100f;

			if(relativePosition) {
				for(int i = 0; i < selection.Count; i++) {
					curAmmount = Math.Min(thingData[i].SectorHeight, Math.Max(0, thingData[i].ZOffset + (int)(thingData[i].SectorHeight * a)));
					selection[i].Move(selection[i].Position.x, selection[i].Position.y, curAmmount * thingData[i].JitterHeight);
				}
			} else {
				for(int i = 0; i < selection.Count; i++) {
					curAmmount = Math.Min(thingData[i].SectorHeight, Math.Max(0, thingData[i].ZOffset + ammount));
					selection[i].Move(selection[i].Position.x, selection[i].Position.y, curAmmount * thingData[i].JitterHeight);
				}
			}

			updateGeometry();
		}

		private void updateGeometry() {
			// Update what must be updated
			if(editingModeName == "BaseVisualMode") {
				VisualMode vm = ((VisualMode)General.Editing.Mode);

				for(int i = 0; i < selection.Count; i++) {
					visualSelection[i].SetPosition(new Vector3D(selection[i].Position.x, selection[i].Position.y, selection[i].Sector.FloorHeight + selection[i].Position.z));

					if(vm.VisualSectorExists(visualSelection[i].Thing.Sector))
						vm.GetVisualSector(visualSelection[i].Thing.Sector).UpdateSectorGeometry(true);
				}
			} else {
				//update view
				General.Interface.RedrawDisplay();
			}
		}

		private void updateAngles() {
			Random rnd = new Random();

			for(int i = 0; i < thingData.Count; i++) {
				ThingData td = thingData[i];
				td.JitterAngle = rnd.Next(359); //(float)(rnd.Next(359) * Math.PI / 180f);
				thingData[i] = td;
			}
		}

		private void updateHeights() {
			Random rnd = new Random();

			for(int i = 0; i < thingData.Count; i++) {
				ThingData td = thingData[i];
				td.JitterHeight = (rnd.Next(100) / 100f);
				thingData[i] = td;
			}
		}

		private void updateRotationAngles() {
			Random rnd = new Random();

			for(int i = 0; i < thingData.Count; i++) {
				ThingData td = thingData[i];
				td.JitterRotation = (rnd.Next(-100, 100) / 100f);
				thingData[i] = td;
			}
		}

//EVENTS
		private void bApply_Click(object sender, EventArgs e) {
			foreach(Thing t in selection)
				t.DetermineSector();

			this.DialogResult = DialogResult.OK;
			Close();
		}

		private void bCancel_Click(object sender, EventArgs e) {
			this.DialogResult = DialogResult.Cancel;
			Close();
		}

		private void JitterThingsForm_FormClosing(object sender, FormClosingEventArgs e) {
			if(this.DialogResult == DialogResult.Cancel)
				General.Map.UndoRedo.WithdrawUndo();//undo changes
		}

		private void positionJitterAmmount_OnValueChanged(object sender, EventArgs e) {
			applyTranslationJitter(positionJitterAmmount.Value);
		}

		private void rotationJitterAmmount_OnValueChanged(object sender, EventArgs e) {
			applyRotationJitter(rotationJitterAmmount.Value);
		}

		private void heightJitterAmmount_OnValueChanging(object sender, EventArgs e) {
			applyHeightJitter(heightJitterAmmount.Value);
		}

//buttons & checkboxes
		private void bUpdateTranslation_Click(object sender, EventArgs e) {
			updateAngles();
			applyTranslationJitter(positionJitterAmmount.Value);
		}

		private void bUpdateHeight_Click(object sender, EventArgs e) {
			updateHeights();
			applyHeightJitter(heightJitterAmmount.Value);
		}

		private void bUpdateAngle_Click(object sender, EventArgs e) {
			updateRotationAngles();
			applyRotationJitter(rotationJitterAmmount.Value);
		}

		private void cbRelativeHeight_CheckedChanged(object sender, EventArgs e) {
			heightJitterAmmount.Label = "Height" + (cbRelativePos.Checked ? " (%):" : ":");
			relativeHeight = cbRelativeHeight.Checked;

			heightJitterAmmount.Maximum = relativeHeight ? 100 : MaxSafeHeightDistance;
			applyHeightJitter(heightJitterAmmount.Value);
		}

		private void cbRelativePos_CheckedChanged(object sender, EventArgs e) {
			positionJitterAmmount.Label = "Position" + (cbRelativePos.Checked ? " (%):" : ":");
			relativePosition = cbRelativePos.Checked;

			positionJitterAmmount.Maximum = relativePosition ? 100 : MaxSafeDistance;
			applyTranslationJitter(positionJitterAmmount.Value);
		}

//HALP!
		private void JitterThingsForm_HelpRequested(object sender, HelpEventArgs hlpevent) {
			General.ShowHelp("gzdb\\features\\general\\jitter.html");
			hlpevent.Handled = true;
		}
	}
}
