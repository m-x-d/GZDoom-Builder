using System;
using System.Windows.Forms;

namespace CodeImp.DoomBuilder.BuilderModes
{
	internal partial class DrawRectangleOptionsPanel : UserControl
	{
		public event EventHandler OnValueChanged;
		public event EventHandler OnContinuousDrawingChanged;
		private bool blockevents;

		public int BevelWidth { get { return (int)radius.Value; } set { blockevents = true; radius.Value = value; blockevents = false; } }
		public int MaxBevelWidth { get { return (int)radius.Maximum; } set { radius.Maximum = value; } }
		public int MinBevelWidth { get { return (int)radius.Minimum; } set { radius.Minimum = value; } }
		public int Subdivisions { get { return (int)subdivs.Value; } set { blockevents = true; subdivs.Value = value; blockevents = false; } }
		public int MaxSubdivisions { get { return (int)subdivs.Maximum; } set { subdivs.Maximum = value; } }
		public int MinSubdivisions { get { return (int)subdivs.Minimum; } set { subdivs.Minimum = value; } }
		public bool ContinuousDrawing { get { return continuousdrawing.Checked; } set { continuousdrawing.Checked = value; } }

		public DrawRectangleOptionsPanel() 
		{
			InitializeComponent();
		}

		public void Register() 
		{
			radius.ValueChanged += ValueChanged;
			subdivs.ValueChanged += ValueChanged;

			General.Interface.AddButton(continuousdrawing);
			General.Interface.AddButton(toolStripSeparator1);
			General.Interface.AddButton(radiuslabel);
			General.Interface.AddButton(radius);
			General.Interface.AddButton(subdivslabel);
			General.Interface.AddButton(subdivs);
			General.Interface.AddButton(reset);
		}

		public void Unregister() 
		{
			General.Interface.RemoveButton(reset);
			General.Interface.RemoveButton(subdivs);
			General.Interface.RemoveButton(subdivslabel);
			General.Interface.RemoveButton(radius);
			General.Interface.RemoveButton(radiuslabel);
			General.Interface.RemoveButton(toolStripSeparator1);
			General.Interface.RemoveButton(continuousdrawing);
		}

		private void ValueChanged(object sender, EventArgs e) 
		{
			if(!blockevents && OnValueChanged != null) OnValueChanged(this, EventArgs.Empty);
		}

		private void reset_Click(object sender, EventArgs e) 
		{
			// Reset values
			blockevents = true;
			radius.Value = 0;
			subdivs.Value = 0;
			blockevents = false;

			// Dispatch event
			OnValueChanged(this, EventArgs.Empty);
		}

		private void continuousdrawing_CheckedChanged(object sender, EventArgs e)
		{
			if(OnContinuousDrawingChanged != null) OnContinuousDrawingChanged(continuousdrawing.Checked, EventArgs.Empty);
		}
	}
}
