using System;
using System.Windows.Forms;

namespace CodeImp.DoomBuilder.BuilderModes
{
	internal partial class DrawEllipseOptionsPanel : UserControl
	{
		public event EventHandler OnValueChanged;
		private bool blockEvents;

		private static int aquityValue;
		private static int subdivsValue = 8;

		public int Spikiness { get { return (int)spikiness.Value; } set { blockEvents = true; spikiness.Value = value; blockEvents = false; } }
		public int Subdivisions { get { return (int)subdivs.Value; } set { blockEvents = true; subdivs.Value = value; blockEvents = false; } }
		public int MaxSubdivisions { get { return (int)subdivs.Maximum; } set { subdivs.Maximum = value; } }
		public int MinSubdivisions { get { return (int)subdivs.Minimum;  } set { subdivs.Minimum = value; } }
		public int MaxSpikiness { get { return (int)spikiness.Maximum; } set { spikiness.Maximum = value; } }
		public int MinSpikiness { get { return (int)spikiness.Minimum; } set { spikiness.Minimum = value; } }
		
		public DrawEllipseOptionsPanel() {
			InitializeComponent();

			spikiness.Value = aquityValue;
			subdivs.Value = subdivsValue;
			spikiness.ValueChanged += ValueChanged;
			subdivs.ValueChanged += ValueChanged;
		}

		public void Register() {
			General.Interface.AddButton(subdivslabel);
			General.Interface.AddButton(subdivs);
			General.Interface.AddButton(spikinesslabel);
			General.Interface.AddButton(spikiness);
			General.Interface.AddButton(reset);
		}

		public void Unregister() {
			General.Interface.RemoveButton(subdivslabel);
			General.Interface.RemoveButton(subdivs);
			General.Interface.RemoveButton(spikinesslabel);
			General.Interface.RemoveButton(spikiness);
			General.Interface.RemoveButton(reset);
		}

		private void ValueChanged(object sender, EventArgs e) {
			aquityValue = (int)spikiness.Value;
			subdivsValue = (int)subdivs.Value;
			if(!blockEvents && OnValueChanged != null) OnValueChanged(this, EventArgs.Empty);
		}

		private void reset_Click(object sender, EventArgs e) {
			blockEvents = true;
			spikiness.Value = 0;
			blockEvents = false;
			subdivs.Value = subdivs.Minimum;
		}
	}
}
