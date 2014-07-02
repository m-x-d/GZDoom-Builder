using System;
using System.Windows.Forms;

namespace CodeImp.DoomBuilder.BuilderModes
{
	internal partial class DrawCurveOptionsPanel : UserControl
	{
		public event EventHandler OnValueChanged;
		private bool blockEvents;

		public int SegmentLength { get { return (int)seglen.Value; } set { blockEvents = true; seglen.Value = value; blockEvents = false; } }

		public DrawCurveOptionsPanel(int minLength, int maxLength) {
			InitializeComponent();

			seglen.Minimum = minLength;
			seglen.Maximum = maxLength;
		}

		private DrawCurveOptionsPanel() { InitializeComponent(); }

		public void Register() {
			General.Interface.AddButton(seglabel);
			General.Interface.AddButton(seglen);
			General.Interface.AddButton(reset);
		}

		public void Unregister() {
			General.Interface.RemoveButton(reset);
			General.Interface.RemoveButton(seglen);
			General.Interface.RemoveButton(seglabel);
		}

		private void seglen_ValueChanged(object sender, EventArgs e) {
			if(!blockEvents && OnValueChanged != null) OnValueChanged(this, EventArgs.Empty);
		}

		private void reset_Click(object sender, EventArgs e) {
			seglen.Value = seglen.Minimum;
		}
	}
}
