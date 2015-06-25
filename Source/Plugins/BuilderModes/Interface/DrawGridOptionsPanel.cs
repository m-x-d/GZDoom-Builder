using System;
using System.Windows.Forms;
using CodeImp.DoomBuilder.Geometry;

namespace CodeImp.DoomBuilder.BuilderModes
{
	internal partial class DrawGridOptionsPanel : UserControl
	{
		public event EventHandler OnValueChanged;
		public event EventHandler OnGridLockChanged;
		private bool blockevents;

		public bool Triangulate { get { return triangulate.Checked; } set { blockevents = true; triangulate.Checked = value; blockevents = false; } }
		public bool LockToGrid { get { return gridlock.Checked; } set { blockevents = true; gridlock.Checked = value; blockevents = false; } }
		public int HorizontalSlices { get { return (int)slicesH.Value; } set { blockevents = true; slicesH.Value = value; blockevents = false; } }
		public int MaxHorizontalSlices { get { return (int)slicesH.Maximum; } set { slicesH.Maximum = value; } }
		public int VerticalSlices { get { return (int)slicesV.Value; } set { blockevents = true; slicesV.Value = value; blockevents = false; } }
		public int MaxVerticalSlices { get { return (int)slicesV.Maximum; } set { slicesV.Maximum = value; } }
		public InterpolationTools.Mode HorizontalInterpolationMode 
		{
			get { return gridlock.Checked ? InterpolationTools.Mode.LINEAR : (InterpolationTools.Mode)interphmode.SelectedIndex; }
			set { interphmode.SelectedIndex = (int)value; }
		}
		public InterpolationTools.Mode VerticalInterpolationMode 
		{
			get { return gridlock.Checked ? InterpolationTools.Mode.LINEAR : (InterpolationTools.Mode)interpvmode.SelectedIndex; }
			set { interpvmode.SelectedIndex = (int)value; }
		}

		public DrawGridOptionsPanel() 
		{
			InitializeComponent();

			// Fill them menus
			interphmode.Items.AddRange(new object[] { MenusForm.GradientInterpolationModes.Linear, MenusForm.GradientInterpolationModes.EaseInOutSine, MenusForm.GradientInterpolationModes.EaseInSine, MenusForm.GradientInterpolationModes.EaseOutSine });
			interphmode.SelectedIndex = 0;
			interpvmode.Items.AddRange(new object[] { MenusForm.GradientInterpolationModes.Linear, MenusForm.GradientInterpolationModes.EaseInOutSine, MenusForm.GradientInterpolationModes.EaseInSine, MenusForm.GradientInterpolationModes.EaseOutSine });
			interpvmode.SelectedIndex = 0;
		}

		private void ValueChanged(object sender, EventArgs e) 
		{
			if(!blockevents && OnValueChanged != null) OnValueChanged(this, EventArgs.Empty);
		}

		private void gridlock_CheckedChanged(object sender, EventArgs e)
		{
			slicesH.Enabled = !gridlock.Checked;
			slicesV.Enabled = !gridlock.Checked;
			interpvmode.Enabled = !gridlock.Checked;
			interphmode.Enabled = !gridlock.Checked;
			reset.Enabled = !gridlock.Checked;

			if(!blockevents && OnGridLockChanged != null) OnGridLockChanged(this, EventArgs.Empty);
		}

		private void interpmode_DropDownClosed(object sender, EventArgs e) 
		{
			General.Interface.FocusDisplay();
		}

		private void reset_Click(object sender, EventArgs e)
		{
			blockevents = true;
			slicesH.Value = 3;
			slicesV.Value = 3;
			blockevents = false;

			if(OnValueChanged != null) OnValueChanged(this, EventArgs.Empty);
		}
	}
}
