using System;
using System.Windows.Forms;

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

		public DrawGridOptionsPanel() {
			InitializeComponent();
		}

		public void Register() {
			General.Interface.AddButton(sliceshlabel);
			General.Interface.AddButton(slicesH);
			General.Interface.AddButton(slicesvlabel);
			General.Interface.AddButton(slicesV);
			General.Interface.AddButton(cbseparator);
			General.Interface.AddButton(gridlock);
			General.Interface.AddButton(triangulate);
		}

		public void Unregister() {
			General.Interface.RemoveButton(sliceshlabel);
			General.Interface.RemoveButton(slicesH);
			General.Interface.RemoveButton(slicesvlabel);
			General.Interface.RemoveButton(slicesV);
			General.Interface.RemoveButton(cbseparator);
			General.Interface.RemoveButton(gridlock);
			General.Interface.RemoveButton(triangulate);
		}

		private void ValueChanged(object sender, EventArgs e) {
			if(!blockEvents && OnValueChanged != null) OnValueChanged(this, EventArgs.Empty);
		}

		private void gridlock_CheckedChanged(object sender, EventArgs e) {
			slicesH.Enabled = !gridlock.Checked;
			slicesV.Enabled = !gridlock.Checked;
			if(!blockEvents && OnGridLockChanged != null) OnGridLockChanged(this, EventArgs.Empty);
		}

	}
}
