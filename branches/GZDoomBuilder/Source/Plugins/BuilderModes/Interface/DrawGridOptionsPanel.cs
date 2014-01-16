using System;
using System.Windows.Forms;
using CodeImp.DoomBuilder.Actions;

namespace CodeImp.DoomBuilder.BuilderModes
{
	public partial class DrawGridOptionsPanel : UserControl
	{
		public event EventHandler OnValueChanged;
		private bool blockEvents;

		public bool Triangulate { get { return triangulate.Checked; } set { blockEvents = true; triangulate.Checked = value; blockEvents = false; } }
		public int HorizontalSlices { get { return (int)slicesH.Value; } set { blockEvents = true; slicesH.Value = value; blockEvents = false; } }
		public int VerticalSlices { get { return (int)slicesV.Value; } set { blockEvents = true; slicesV.Value = value; blockEvents = false; } }

		public DrawGridOptionsPanel() {
			InitializeComponent();

			//set hints
			string help = "Use <b>" + Actions.Action.GetShortcutKeyDesc("buildermodes_increasebevel") + "</b> and <b>" + Actions.Action.GetShortcutKeyDesc("buildermodes_decreasebevel") + "</b> to change the number of horizontal slices<br>"
						  + "Use <b>" + Actions.Action.GetShortcutKeyDesc("buildermodes_increasesubdivlevel") + "</b> and <b>" + Actions.Action.GetShortcutKeyDesc("buildermodes_decreasesubdivlevel") + "</b> to change the number of vertical slices";
			hints.SelectedRtf = HintsManager.GetRtfString(help);
		}

		private void ValueChanged(object sender, EventArgs e) {
			if(blockEvents) return;
			if(OnValueChanged != null) OnValueChanged(this, EventArgs.Empty);
		}

	}
}
