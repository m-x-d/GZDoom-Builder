#region ================== Namespaces

using System;
using System.Windows.Forms;

#endregion

namespace CodeImp.DoomBuilder.Controls
{
	#region ================== Enums

	internal enum SlopePivotMode
	{
		ORIGIN, // pivot around 0, 0
		GLOBAL, // pivot around selection center
		LOCAL,  // pivot around sector center
	}

	#endregion

	public partial class SectorSlopeControl : UserControl
	{

		#region ================== Events

		public event EventHandler OnAnglesChanged;
		public event EventHandler OnUseLineAnglesChanged;
		public event EventHandler OnPivotModeChanged;
		public event EventHandler OnResetClicked;

		#endregion

		#region ================== Variables

		private bool blockUpdate;
		
		// Slope values
		private float anglexy;
		private float anglez;
		private float offset;

		#endregion

		#region ================== Properties

		public StepsList StepValues { set { sloperotation.StepValues = value; } }
		public bool UseLineAngles { get { return cbuselineangles.Checked; } set { blockUpdate = true; cbuselineangles.Checked = value; blockUpdate = false; } }

		internal SlopePivotMode PivotMode 
		{
			get 
			{
				return (SlopePivotMode)pivotmodeselector.SelectedIndex;
			}
			set 
			{
				blockUpdate = true;
				pivotmodeselector.SelectedIndex = (int)value;
				blockUpdate = false;
			}
		}

		#endregion

		#region ================== Constructor

		public SectorSlopeControl() 
		{
			InitializeComponent();
		}

		#endregion

		#region ================== Property accessors

		public float GetAngleXY(float defaultvalue) 
		{
			return sloperotation.GetResultFloat(defaultvalue);
		}

		public float GetAngleZ(float defaultvalue) 
		{
			return slopeangle.GetResultFloat(defaultvalue);
		}

		public float GetOffset(float defaultvalue) 
		{
			return slopeoffset.GetResultFloat(defaultvalue);
		}

		#endregion

		#region ================== Methods

		public void SetValues(float anglexy, float anglez, float offset, bool first) 
		{
			if (first) 
			{
				// Set values
				this.anglexy = anglexy;
				this.anglez = anglez;
				this.offset = offset;
			} 
			else 
			{
				// Or update values
				if(!float.IsNaN(this.anglexy) && this.anglexy != anglexy) this.anglexy = float.NaN;
				if(!float.IsNaN(this.anglez) && this.anglez != anglez) this.anglez = float.NaN;
				if(!float.IsNaN(this.offset) && this.offset != offset) this.offset = float.NaN;
			}
		}

		public void SetOffset(float offset, bool first) 
		{
			if(first) 
			{
				this.offset = offset;
			} 
			else if(!float.IsNaN(this.offset) && this.offset != offset)
			{
				this.offset = float.NaN;
			}
		}

		public void UpdateControls() 
		{
			blockUpdate = true;

			if(float.IsNaN(anglexy)) 
			{
				sloperotation.Text = "";
				rotationcontrol.Angle = GZBuilder.Controls.AngleControl.NO_ANGLE;
			} 
			else 
			{
				sloperotation.Text = anglexy.ToString();
				rotationcontrol.Angle = (int)Math.Round(anglexy + 90);
			}

			if(float.IsNaN(anglez)) 
			{
				slopeangle.Text = "";
				angletrackbar.Value = 0;
			} 
			else 
			{
				//clamp value to [-85 .. 85]
				anglez = General.Clamp(anglez, angletrackbar.Minimum, angletrackbar.Maximum);

				slopeangle.Text = anglez.ToString();
				angletrackbar.Value = (int)General.Clamp(anglez, angletrackbar.Minimum, angletrackbar.Maximum);
			}

			slopeoffset.Text = (float.IsNaN(offset) ? "" : offset.ToString());

			blockUpdate = false;
		}

		public void UpdateOffset() 
		{
			blockUpdate = true;
			slopeoffset.Text = (float.IsNaN(offset) ? "" : offset.ToString());
			blockUpdate = false;
		}

		#endregion

		#region ================== Events

		private void sloperotation_WhenTextChanged(object sender, EventArgs e) 
		{
			if(blockUpdate) return;
			blockUpdate = true;

			anglexy = General.ClampAngle(sloperotation.GetResultFloat(float.NaN));
			rotationcontrol.Angle = (float.IsNaN(anglexy) ? GZBuilder.Controls.AngleControl.NO_ANGLE : (int)Math.Round(anglexy + 90));

			if(OnAnglesChanged != null) OnAnglesChanged(this, EventArgs.Empty);
			blockUpdate = false;
		}

		private void rotationcontrol_AngleChanged(object sender, EventArgs e) 
		{
			if(blockUpdate) return;
			blockUpdate = true;

			anglexy = General.ClampAngle(rotationcontrol.Angle - 90);
			sloperotation.Text = anglexy.ToString();

			if(OnAnglesChanged != null) OnAnglesChanged(this, EventArgs.Empty);
			blockUpdate = false;
		}

		private void slopeangle_WhenTextChanged(object sender, EventArgs e) 
		{
			if(blockUpdate) return;
			blockUpdate = true;

			anglez = General.Clamp((int)Math.Round(slopeangle.GetResultFloat(0f)), angletrackbar.Minimum, angletrackbar.Maximum);
			angletrackbar.Value = (int)anglez;

			if(OnAnglesChanged != null) OnAnglesChanged(this, EventArgs.Empty);
			blockUpdate = false;
		}

		private void angletrackbar_ValueChanged(object sender, EventArgs e) 
		{
			if(blockUpdate) return;
			blockUpdate = true;

			slopeangle.Text = angletrackbar.Value.ToString();
			anglez = angletrackbar.Value;

			if(OnAnglesChanged != null) OnAnglesChanged(this, EventArgs.Empty);
			blockUpdate = false;
		}

		private void slopeoffset_WhenTextChanged(object sender, EventArgs e) 
		{
			offset = slopeoffset.GetResultFloat(float.NaN);
			if(OnAnglesChanged != null) OnAnglesChanged(this, EventArgs.Empty);
		}

		private void reset_Click(object sender, EventArgs e) 
		{
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

		private void pivotmodeselector_SelectedIndexChanged(object sender, EventArgs e) 
		{
			if(blockUpdate) return;
			if(OnPivotModeChanged != null) OnPivotModeChanged(this, EventArgs.Empty);
		}

		private void cbuselineangles_CheckedChanged(object sender, EventArgs e) 
		{
			sloperotation.ButtonStepsWrapAround = cbuselineangles.Checked;
			if(blockUpdate) return;
			if(OnUseLineAnglesChanged != null) OnUseLineAnglesChanged(this, EventArgs.Empty);
		}

		#endregion

	}
}
