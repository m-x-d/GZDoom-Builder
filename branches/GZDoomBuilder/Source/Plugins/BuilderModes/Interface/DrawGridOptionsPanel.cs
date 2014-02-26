using System;
using System.Windows.Forms;
using CodeImp.DoomBuilder.Actions;

namespace CodeImp.DoomBuilder.BuilderModes
{
	public partial class DrawGridOptionsPanel : UserControl
	{
		public event EventHandler OnValueChanged;
		public event EventHandler OnGridLockChanged;
		private bool blockEvents;
		private readonly string help;
		private readonly string gridlockhelp;


		public bool Triangulate { get { return triangulate.Checked; } set { blockEvents = true; triangulate.Checked = value; blockEvents = false; } }
		public bool LockToGrid { get { return gridlock.Checked; } set { blockEvents = true; gridlock.Checked = value; blockEvents = false; } }
		public int HorizontalSlices { get { return (int)slicesH.Value; } set { blockEvents = true; slicesH.Value = value; blockEvents = false; } }
		public int VerticalSlices { get { return (int)slicesV.Value; } set { blockEvents = true; slicesV.Value = value; blockEvents = false; } }

		public DrawGridOptionsPanel() {
			InitializeComponent();

			//set hints
			help = HintsManager.GetRtfString("Use <b>" + Actions.Action.GetShortcutKeyDesc("buildermodes_increasebevel") + "</b> and <b>" + Actions.Action.GetShortcutKeyDesc("buildermodes_decreasebevel") + "</b> to change the number of horizontal slices<br>"
						  + "Use <b>" + Actions.Action.GetShortcutKeyDesc("buildermodes_increasesubdivlevel") + "</b> and <b>" + Actions.Action.GetShortcutKeyDesc("buildermodes_decreasesubdivlevel") + "</b> to change the number of vertical slices");
			gridlockhelp = HintsManager.GetRtfString("Use <b>" + Actions.Action.GetShortcutKeyDesc("builder_griddec") + "</b> and <b>" + Actions.Action.GetShortcutKeyDesc("builder_gridinc") + "</b> to change grid size.");
			hints.SelectedRtf = help;
		}

		private void ValueChanged(object sender, EventArgs e) {
			if(!blockEvents && OnValueChanged != null) OnValueChanged(this, EventArgs.Empty);
		}

		private void gridlock_CheckedChanged(object sender, EventArgs e) {
			slicesH.Enabled = !gridlock.Checked;
			slicesV.Enabled = !gridlock.Checked;
			hints.Clear();
			hints.SelectedRtf = (gridlock.Checked ? gridlockhelp : help);

			if(blockEvents) return;
			if(OnGridLockChanged != null) OnGridLockChanged(this, EventArgs.Empty);
		}

	}
}
