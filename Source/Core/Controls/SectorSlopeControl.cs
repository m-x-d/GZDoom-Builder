using System;
using System.Windows.Forms;
using CodeImp.DoomBuilder.Geometry;

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

		public event EventHandler OnValuesChanged; //mxd

		#endregion

		#region ================== Variables

		private static SlopePivotMode pivotmode = SlopePivotMode.LOCAL;
		internal SlopePivotMode PivotMode { get { return pivotmode; } }

		private bool blockUpdate;
		
		//slope values
		private float anglexy;
		private float anglez;
		private float offset;

		public float AngleXY { get { return anglexy; } }
		public float AngleZ { get { return anglez; } }
		public float Offset { get { return offset; } }

		#endregion

		public SectorSlopeControl() {
			InitializeComponent();
			pivotmodeselector.SelectedIndex = (int) pivotmode;
		}

		#region ================== Methods

		public void SetValues(float anglexy, float anglez, float offset, bool first) {
			//update values
			if (first) {
				this.anglexy = anglexy;
				this.anglez = anglez;
				this.offset = offset;
			} else {
				if(!float.IsNaN(this.anglexy) && this.anglexy != anglexy) this.anglexy = float.NaN;
				if(!float.IsNaN(this.anglez) && this.anglez != anglez) this.anglez = float.NaN;
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
				rotationcontrol.Angle = (int)Math.Round(anglexy); //(int)Math.Round(Angle2D.RadToDeg(this.anglexy));
			}

			if(float.IsNaN(anglez)) {
				slopeangle.Text = "";
				angletrackbar.Value = 0;
			} else {
				slopeangle.Text = anglez.ToString();
				angletrackbar.Value = General.Clamp((int)Math.Round(anglez - 90), angletrackbar.Minimum, angletrackbar.Maximum);
			}

			slopeoffset.Text = float.IsNaN(this.offset) ? "" : this.offset.ToString();

			blockUpdate = false;
		}

		#endregion

		#region ================== Events

		private void sloperotation_WhenTextChanged(object sender, EventArgs e) {
			if(blockUpdate) return;
			blockUpdate = true;

			anglexy = sloperotation.GetResultFloat(0f); //Angle2D.DegToRad(sloperotation.GetResultFloat(0f));
			rotationcontrol.Angle = (int)Math.Round(sloperotation.GetResultFloat(0f));

			if(OnValuesChanged != null) OnValuesChanged(this, EventArgs.Empty);
			blockUpdate = false;
		}

		private void rotationcontrol_AngleChanged() {
			if(blockUpdate) return;
			blockUpdate = true;

			sloperotation.Text = rotationcontrol.Angle.ToString();
			anglexy = Angle2D.DegToRad(rotationcontrol.Angle);

			if(OnValuesChanged != null) OnValuesChanged(this, EventArgs.Empty);
			blockUpdate = false;
		}

		private void slopeangle_WhenTextChanged(object sender, EventArgs e) {
			if(blockUpdate) return;
			blockUpdate = true;

			int anglezdeg = General.Clamp((int)Math.Round(slopeangle.GetResultFloat(0f)), angletrackbar.Minimum, angletrackbar.Maximum);
			angletrackbar.Value = anglezdeg;
			anglez = Angle2D.DegToRad(anglezdeg - 90);

			if(OnValuesChanged != null) OnValuesChanged(this, EventArgs.Empty);
			blockUpdate = false;
		}

		private void angletrackbar_ValueChanged(object sender, EventArgs e) {
			if(blockUpdate) return;
			blockUpdate = true;

			slopeangle.Text = angletrackbar.Value.ToString();
			anglez = Angle2D.DegToRad(angletrackbar.Value);

			if(OnValuesChanged != null) OnValuesChanged(this, EventArgs.Empty);
			blockUpdate = false;
		}

		private void slopeoffset_WhenTextChanged(object sender, EventArgs e) {
			offset = slopeoffset.GetResultFloat(0f);
			if(OnValuesChanged != null) OnValuesChanged(this, EventArgs.Empty);
		}

		private void reset_Click(object sender, EventArgs e) {
			blockUpdate = true;

			sloperotation.Text = "0";
			rotationcontrol.Angle = 0;
			slopeangle.Text = "0";
			angletrackbar.Value = 0;
			slopeoffset.Text = "0";
			anglexy = 0f;
			anglez = 0f;
			offset = 0f;

			if (OnValuesChanged != null) OnValuesChanged(this, EventArgs.Empty);
			blockUpdate = false;
		}

		private void pivotmodeselector_SelectedIndexChanged(object sender, EventArgs e) {
			pivotmode = (SlopePivotMode)pivotmodeselector.SelectedIndex;
		}

		#endregion

	}
}
