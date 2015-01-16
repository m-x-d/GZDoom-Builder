using System;
using System.Windows.Forms;
using CodeImp.DoomBuilder.Geometry;

namespace CodeImp.DoomBuilder.BuilderModes
{
	internal partial class DrawGridOptionsPanel : UserControl
	{
		public event EventHandler OnValueChanged;
		public event EventHandler OnGridLockChanged;
		private bool blockEvents;

		public bool Triangulate { get { return triangulate.Checked; } set { blockEvents = true; triangulate.Checked = value; blockEvents = false; } }
		public bool LockToGrid { get { return gridlock.Checked; } set { blockEvents = true; gridlock.Checked = value; blockEvents = false; } }
		public int HorizontalSlices { get { return (int)slicesH.Value; } set { blockEvents = true; slicesH.Value = value; blockEvents = false; } }
		public int MaxHorizontalSlices { get { return (int)slicesH.Maximum; } set { slicesH.Maximum = value; } }
		public int VerticalSlices { get { return (int)slicesV.Value; } set { blockEvents = true; slicesV.Value = value; blockEvents = false; } }
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
			interphmode.Items.AddRange(new[] { MenusForm.GradientInterpolationModes.Linear, MenusForm.GradientInterpolationModes.EaseInOutSine, MenusForm.GradientInterpolationModes.EaseInSine, MenusForm.GradientInterpolationModes.EaseOutSine });
			interphmode.SelectedIndex = 0;
			interpvmode.Items.AddRange(new[] { MenusForm.GradientInterpolationModes.Linear, MenusForm.GradientInterpolationModes.EaseInOutSine, MenusForm.GradientInterpolationModes.EaseInSine, MenusForm.GradientInterpolationModes.EaseOutSine });
			interpvmode.SelectedIndex = 0;
		}

		public void Register() 
		{
			General.Interface.AddButton(sliceshlabel);
			General.Interface.AddButton(slicesH);
			General.Interface.AddButton(slicesvlabel);
			General.Interface.AddButton(slicesV);
			General.Interface.AddButton(reset);
			General.Interface.AddButton(cbseparator1);
			General.Interface.AddButton(interphlabel);
			General.Interface.AddButton(interphmode);
			General.Interface.AddButton(interpvlabel);
			General.Interface.AddButton(interpvmode);
			General.Interface.AddButton(cbseparator2);
			General.Interface.AddButton(gridlock);
			General.Interface.AddButton(triangulate);
		}

		public void Unregister() 
		{
			General.Interface.RemoveButton(triangulate);
			General.Interface.RemoveButton(gridlock);
			General.Interface.RemoveButton(cbseparator2);
			General.Interface.RemoveButton(interpvmode);
			General.Interface.RemoveButton(interpvlabel);
			General.Interface.RemoveButton(interphmode);
			General.Interface.RemoveButton(interphlabel);
			General.Interface.RemoveButton(cbseparator1);
			General.Interface.RemoveButton(reset);
			General.Interface.RemoveButton(slicesV);
			General.Interface.RemoveButton(slicesvlabel);
			General.Interface.RemoveButton(slicesH);
			General.Interface.RemoveButton(sliceshlabel);
		}

		private void ValueChanged(object sender, EventArgs e) 
		{
			if(!blockEvents && OnValueChanged != null) OnValueChanged(this, EventArgs.Empty);
		}

		private void gridlock_CheckedChanged(object sender, EventArgs e)
		{
			slicesH.Enabled = !gridlock.Checked;
			slicesV.Enabled = !gridlock.Checked;
			interpvmode.Enabled = !gridlock.Checked;
			interpvlabel.Enabled = !gridlock.Checked;
			interphmode.Enabled = !gridlock.Checked;
			interphlabel.Enabled = !gridlock.Checked;
			reset.Enabled = !gridlock.Checked;
			if(!blockEvents && OnGridLockChanged != null) OnGridLockChanged(this, EventArgs.Empty);
		}

		private void interpmode_DropDownClosed(object sender, EventArgs e) 
		{
			General.Interface.FocusDisplay();
		}

	}
}
