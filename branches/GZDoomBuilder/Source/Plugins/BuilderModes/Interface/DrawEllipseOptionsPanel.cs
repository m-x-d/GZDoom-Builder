using System;
using System.Windows.Forms;

namespace CodeImp.DoomBuilder.BuilderModes
{
	internal partial class DrawEllipseOptionsPanel : UserControl
	{
		public event EventHandler OnValueChanged;
		public event EventHandler OnContinuousDrawingChanged;
		private bool blockevents;

		public int Spikiness { get { return (int)spikiness.Value; } set { blockevents = true; spikiness.Value = value; blockevents = false; } }
		public int Subdivisions { get { return (int)subdivs.Value; } set { blockevents = true; subdivs.Value = value; blockevents = false; } }
		public int Angle { get { return (int)angle.Value; } set { blockevents = true; angle.Value = value; blockevents = false; } }
		public int MaxSubdivisions { get { return (int)subdivs.Maximum; } set { subdivs.Maximum = value; } }
		public int MinSubdivisions { get { return (int)subdivs.Minimum;  } set { subdivs.Minimum = value; } }
		public int MaxSpikiness { get { return (int)spikiness.Maximum; } set { spikiness.Maximum = value; } }
		public int MinSpikiness { get { return (int)spikiness.Minimum; } set { spikiness.Minimum = value; } }
		public bool ContinuousDrawing { get { return continuousdrawing.Checked; } set { continuousdrawing.Checked = value; } }
		
		public DrawEllipseOptionsPanel() 
		{
			InitializeComponent();
		}

		public void Register() 
		{
			spikiness.ValueChanged += ValueChanged;
			subdivs.ValueChanged += ValueChanged;
			angle.ValueChanged += ValueChanged;

			General.Interface.AddButton(continuousdrawing);
			General.Interface.AddButton(toolStripSeparator1);
			General.Interface.AddButton(subdivslabel);
			General.Interface.AddButton(subdivs);
			General.Interface.AddButton(spikinesslabel);
			General.Interface.AddButton(spikiness);
			General.Interface.AddButton(anglelabel);
			General.Interface.AddButton(angle);
			General.Interface.AddButton(reset);
		}

		public void Unregister() 
		{
			General.Interface.RemoveButton(reset);
			General.Interface.RemoveButton(angle);
			General.Interface.RemoveButton(anglelabel);
			General.Interface.RemoveButton(spikiness);
			General.Interface.RemoveButton(spikinesslabel);
			General.Interface.RemoveButton(subdivs);
			General.Interface.RemoveButton(subdivslabel);
			General.Interface.RemoveButton(toolStripSeparator1);
			General.Interface.RemoveButton(continuousdrawing);
		}

		private void ValueChanged(object sender, EventArgs e) 
		{
			if(!blockevents && OnValueChanged != null) OnValueChanged(this, EventArgs.Empty);
		}

		private void reset_Click(object sender, EventArgs e) 
		{
			blockevents = true;
			spikiness.Value = 0;
			angle.Value = 0;
			blockevents = false;
			subdivs.Value = 6;
		}

		private void continuousdrawing_CheckedChanged(object sender, EventArgs e)
		{
			if(OnContinuousDrawingChanged != null) OnContinuousDrawingChanged(continuousdrawing.Checked, EventArgs.Empty);
		}
	}
}
