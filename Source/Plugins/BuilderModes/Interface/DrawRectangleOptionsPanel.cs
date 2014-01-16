using System;
using System.Windows.Forms;
using CodeImp.DoomBuilder.Actions;

namespace CodeImp.DoomBuilder.BuilderModes
{
	public partial class DrawRectangleOptionsPanel : UserControl
	{
		public event EventHandler OnValueChanged;
		private bool blockEvents;

		private static int radiusValue;
		private static int subdivsValue;

		public int BevelWidth { get { return (int)radius.Value; } set { blockEvents = true; radius.Value = value; blockEvents = false; } }
		public int Subdivisions { get { return (int)subdivs.Value; } set { blockEvents = true; subdivs.Value = value; blockEvents = false; } }
		public int MaxSubdivisions { set { subdivs.Maximum = value; } }

		public DrawRectangleOptionsPanel() {
			InitializeComponent();

			radius.Value = radiusValue;
			subdivs.Value = subdivsValue;
			radius.ValueChanged += ValueChanged;
			subdivs.ValueChanged += ValueChanged;

			//set hints
			string help = "Use <b>" + Actions.Action.GetShortcutKeyDesc("buildermodes_increasebevel") + "</b> and <b>" + Actions.Action.GetShortcutKeyDesc("buildermodes_decreasebevel") + "</b> to change corners bevel by current grid size<br>"
						  + "Use <b>" + Actions.Action.GetShortcutKeyDesc("buildermodes_increasesubdivlevel") + "</b> and <b>" + Actions.Action.GetShortcutKeyDesc("buildermodes_decreasesubdivlevel") + "</b> to change bevel detail level";
			hints.SelectedRtf = HintsManager.GetRtfString(help);
		}

		private void ValueChanged(object sender, EventArgs e) {
			radiusValue = (int)radius.Value;
			subdivsValue = (int)subdivs.Value;

			if(blockEvents) return;
			if(OnValueChanged != null) OnValueChanged(this, EventArgs.Empty);
		}

		private void reset_Click(object sender, EventArgs e) {
			radius.Value = 0;
			subdivs.Value = 0;
		}

	}
}
