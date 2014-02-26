using System;
using System.Windows.Forms;
using CodeImp.DoomBuilder.Actions;

namespace CodeImp.DoomBuilder.BuilderModes
{
	public partial class DrawCurveOptionsPanel : UserControl
	{
		public event EventHandler OnValueChanged;
		private bool blockEvents;

		public int SegmentLength { get { return (int)seglen.Value; } set { blockEvents = true; seglen.Value = value; blockEvents = false; } }

		public DrawCurveOptionsPanel(int minLength, int maxLength) {
			InitializeComponent();

			seglen.Minimum = minLength;
			seglen.Maximum = maxLength;

			//set hints
			string help = "Use <b>" + Actions.Action.GetShortcutKeyDesc("buildermodes_increasesubdivlevel") + "</b> and <b>" + Actions.Action.GetShortcutKeyDesc("buildermodes_decreasesubdivlevel") + "</b> to change curve detail level";
			hints.SelectedRtf = HintsManager.GetRtfString(help);
		}

		private DrawCurveOptionsPanel() { InitializeComponent(); }

		private void seglen_ValueChanged(object sender, EventArgs e) {
			if(!blockEvents && OnValueChanged != null) OnValueChanged(this, EventArgs.Empty);
		}
	}
}
