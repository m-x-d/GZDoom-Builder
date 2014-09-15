using System;
using System.Windows.Forms;

namespace CodeImp.DoomBuilder.Controls
{
	#region ================== Enums

	internal enum SlopePivotMode
	{
		ORIGIN, // pivot around 0, 0
		GLOBAL, // pivot around globalslopepivot
		LOCAL,  // pivot around localslopepivots
	}

	#endregion

	public partial class SectorSlopeControl : UserControl
	{

		#region ================== Events

		public event EventHandler OnAnglesChanged;
		public event EventHandler OnUseLineAnglesChanged;
		public event EventHandler OnOffsetChanged;
		public event EventHandler OnPivotModeChanged;
		public event EventHandler OnResetClicked;

		#endregion

		#region ================== Variables

		private static SlopePivotMode pivotmode = SlopePivotMode.ORIGIN; //DBG SlopePivotMode.LOCAL
		internal SlopePivotMode PivotMode { get { return pivotmode; } }

		private bool blockUpdate;
		
		//slope values
		private float anglexy;
		private float anglez;
		private float offset;

		public float AngleXY { get { return anglexy; } } //in dergrees
		public float AngleZ { get { return anglez; } } //in dergrees, add 90
		public float Offset { get { return offset; } }
		public StepsList StepValues { set { sloperotation.StepValues = value; } }
		public bool UseLineAngles { get { return cbuselineangles.Checked; } set { blockUpdate = true; cbuselineangles.Checked = value; blockUpdate = false; } }

		#endregion

		public SectorSlopeControl() {
			InitializeComponent();
		}

		#region ================== Methods

		public void SetValues(float anglexy, float anglez, float offset, bool first) {
			if (first) {
				// Set values
				this.anglexy = anglexy;
				this.anglez = anglez;
				this.offset = offset;
			} else {
				// Or update values
				if(!float.IsNaN(this.anglexy) && this.anglexy != anglexy) this.anglexy = float.NaN;
				if(!float.IsNaN(this.anglez) && this.anglez != anglez) this.anglez = float.NaN;
				if(!float.IsNaN(this.offset) && this.offset != offset) this.offset = float.NaN;
			}
		}

		public void SetOffset(float offset, bool first) {
			if(first) {
				this.offset = offset;
			} else {
				if(!float.IsNaN(this.offset) && this.offset != offset) this.offset = float.NaN;
			}
		}

		public void UpdateControls() {
			blockUpdate = true;

			if(float.IsNaN(anglexy)) {
				sloperotation.Text = "";
				rotationcontrol.Angle = 0;
			} else {
				sloperotation.Text = anglexy.ToString();
				rotationcontrol.Angle = (int)Math.Round(anglexy + 90); //(int)Math.Round(Angle2D.RadToDeg(this.anglexy));
			}

			if(float.IsNaN(anglez)) {
				slopeangle.Text = "";
				angletrackbar.Value = 0;
			} else {
				//clamp value to [-85 .. 85]
				anglez = General.Clamp(anglez, angletrackbar.Minimum, angletrackbar.Maximum);

				slopeangle.Text = anglez.ToString();
				//angletrackbar.Value = (int)Math.Round(anglez);
				angletrackbar.Value = (int)General.Clamp(anglez, angletrackbar.Minimum, angletrackbar.Maximum);
			}

			slopeoffset.Text = (float.IsNaN(offset) ? "" : offset.ToString());

			blockUpdate = false;
		}

		public void UpdateOffset() {
			blockUpdate = true;
			slopeoffset.Text = (float.IsNaN(offset) ? "" : offset.ToString());
			blockUpdate = false;
		}

		#endregion

		#region ================== Events

		private void sloperotation_WhenTextChanged(object sender, EventArgs e) {
			if(blockUpdate) return;
			blockUpdate = true;

			anglexy = General.ClampAngle(sloperotation.GetResultFloat(0f));
			rotationcontrol.Angle = (int)Math.Round(anglexy + 90);

			if(OnAnglesChanged != null) OnAnglesChanged(this, EventArgs.Empty);
			blockUpdate = false;
		}

		private void rotationcontrol_AngleChanged() {
			if(blockUpdate) return;
			blockUpdate = true;

			anglexy = General.ClampAngle(rotationcontrol.Angle - 90);
			sloperotation.Text = anglexy.ToString();

			if(OnAnglesChanged != null) OnAnglesChanged(this, EventArgs.Empty);
			blockUpdate = false;
		}

		private void slopeangle_WhenTextChanged(object sender, EventArgs e) {
			if(blockUpdate) return;
			blockUpdate = true;

			anglez = General.Clamp((int)Math.Round(slopeangle.GetResultFloat(0f)), angletrackbar.Minimum, angletrackbar.Maximum);
			angletrackbar.Value = (int)anglez;

			if(OnAnglesChanged != null) OnAnglesChanged(this, EventArgs.Empty);
			blockUpdate = false;
		}

		private void angletrackbar_ValueChanged(object sender, EventArgs e) {
			if(blockUpdate) return;
			blockUpdate = true;

			slopeangle.Text = angletrackbar.Value.ToString();
			anglez = angletrackbar.Value;

			if(OnAnglesChanged != null) OnAnglesChanged(this, EventArgs.Empty);
			blockUpdate = false;
		}

		private void slopeoffset_WhenTextChanged(object sender, EventArgs e) {
			offset = slopeoffset.GetResultFloat(0f);
			if(OnOffsetChanged != null) OnOffsetChanged(this, EventArgs.Empty);
		}

		private void reset_Click(object sender, EventArgs e) {
			blockUpdate = true;

			sloperotation.Text = "0";
			rotationcontrol.Angle = 90;
			slopeangle.Text = "0";
			angletrackbar.Value = 0;
			slopeoffset.Text = "0";
			anglexy = 0f;
			anglez = 0f;
			offset = 0f;

			if(OnResetClicked != null) OnResetClicked(this, EventArgs.Empty);
			blockUpdate = false;
		}

		private void pivotmodeselector_SelectedIndexChanged(object sender, EventArgs e) {
			pivotmode = (SlopePivotMode)pivotmodeselector.SelectedIndex;

			if(blockUpdate) return;
			if (OnPivotModeChanged != null) OnPivotModeChanged(this, EventArgs.Empty);
		}

		private void SectorSlopeControl_Load(object sender, EventArgs e) {
			pivotmodeselector.SelectedIndex = (int)pivotmode;
		}

		private void cbuselineangles_CheckedChanged(object sender, EventArgs e) {
			sloperotation.ButtonStepsWrapAround = cbuselineangles.Checked;
			if(blockUpdate) return;
			if(OnUseLineAnglesChanged != null) OnUseLineAnglesChanged(this, EventArgs.Empty);
		}

		#endregion

	}
}
