using System;
using System.Windows.Forms;
using CodeImp.DoomBuilder.Actions;

namespace CodeImp.DoomBuilder.BuilderModes
{
	public partial class DrawEllipseOptionsPanel : UserControl
	{
		public event EventHandler OnValueChanged;
		private bool blockEvents;

		private static int aquityValue;
		private static int subdivsValue = 8;

		public int Aquity { get { return (int)spikiness.Value; } set { blockEvents = true; spikiness.Value = value; blockEvents = false; } }
		public int Subdivisions { get { return (int)subdivs.Value; } set { blockEvents = true; subdivs.Value = value; blockEvents = false; } }
		public int MaxSubdivisions { set { subdivs.Maximum = value; } }
		public int MinSubdivisions { set { subdivs.Minimum = value; } }
		
		public DrawEllipseOptionsPanel() {
			InitializeComponent();

			spikiness.Value = aquityValue;
			subdivs.Value = subdivsValue;
			spikiness.ValueChanged += ValueChanged;
			subdivs.ValueChanged += ValueChanged;

			//set hints
			string help = "Use <b>" + Actions.Action.GetShortcutKeyDesc("buildermodes_increasebevel") + "</b> and <b>" + Actions.Action.GetShortcutKeyDesc("buildermodes_decreasebevel") + "</b> to change ellipse spikiness<br>"
						  + "Use <b>" + Actions.Action.GetShortcutKeyDesc("buildermodes_increasesubdivlevel") + "</b> and <b>" + Actions.Action.GetShortcutKeyDesc("buildermodes_decreasesubdivlevel") + "</b> to change the number of points in ellipse";
			hints.SelectedRtf = HintsManager.GetRtfString(help);
		}

		private void ValueChanged(object sender, EventArgs e) {
			aquityValue = (int)spikiness.Value;
			subdivsValue = (int)subdivs.Value;
			if(!blockEvents && OnValueChanged != null) OnValueChanged(this, EventArgs.Empty);
		}

		private void reset_Click(object sender, EventArgs e) {
			spikiness.Value = 0;
			subdivs.Value = subdivs.Minimum;
		}
	}
}
