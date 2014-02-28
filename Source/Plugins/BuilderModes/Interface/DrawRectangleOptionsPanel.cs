using System;
using System.Windows.Forms;

namespace CodeImp.DoomBuilder.BuilderModes
{
	internal partial class DrawRectangleOptionsPanel : UserControl
	{
		public event EventHandler OnValueChanged;
		private bool blockEvents;

		private static int radiusValue;
		private static int subdivsValue;

		public int BevelWidth { get { return (int)radius.Value; } set { blockEvents = true; radius.Value = value; blockEvents = false; } }
		public int MaxBevelWidth { get { return (int)radius.Maximum; } set { radius.Maximum = value; } }
		public int MinBevelWidth { get { return (int)radius.Minimum; } set { radius.Minimum = value; } }
		public int Subdivisions { get { return (int)subdivs.Value; } set { blockEvents = true; subdivs.Value = value; blockEvents = false; } }
		public int MaxSubdivisions { get { return (int)subdivs.Maximum; } set { subdivs.Maximum = value; } }
		public int MinSubdivisions { get { return (int)subdivs.Minimum; } set { subdivs.Minimum = value; } }

		public DrawRectangleOptionsPanel() {
			InitializeComponent();

			radius.Value = radiusValue;
			subdivs.Value = subdivsValue;
			radius.ValueChanged += ValueChanged;
			subdivs.ValueChanged += ValueChanged;
		}

		public void Register() {
			General.Interface.AddButton(radiuslabel);
			General.Interface.AddButton(radius);
			General.Interface.AddButton(subdivslabel);
			General.Interface.AddButton(subdivs);
			General.Interface.AddButton(reset);
		}

		public void Unregister() {
			General.Interface.RemoveButton(radiuslabel);
			General.Interface.RemoveButton(radius);
			General.Interface.RemoveButton(subdivslabel);
			General.Interface.RemoveButton(subdivs);
			General.Interface.RemoveButton(reset);
		}

		private void ValueChanged(object sender, EventArgs e) {
			radiusValue = (int)radius.Value;
			subdivsValue = (int)subdivs.Value;
			if(!blockEvents && OnValueChanged != null) OnValueChanged(this, EventArgs.Empty);
		}

		private void reset_Click(object sender, EventArgs e) {
			blockEvents = true;
			radius.Value = 0;
			blockEvents = false;
			subdivs.Value = 0;
		}

	}
}
