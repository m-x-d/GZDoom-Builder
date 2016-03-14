﻿using System;
using System.Windows.Forms;

namespace CodeImp.DoomBuilder.BuilderModes
{
	internal partial class DrawCurveOptionsPanel : UserControl
	{
		public event EventHandler OnValueChanged;
		public event EventHandler OnContinuousDrawingChanged;
		public event EventHandler OnAutoCloseDrawingChanged;
		private bool blockevents;

		public int SegmentLength { get { return (int)seglen.Value; } set { blockevents = true; seglen.Value = value; blockevents = false; } }
		public bool ContinuousDrawing { get { return continuousdrawing.Checked; } set { continuousdrawing.Checked = value; } }
		public bool AutoCloseDrawing { get { return autoclosedrawing.Checked; } set { autoclosedrawing.Checked = value; } }

		public DrawCurveOptionsPanel(int minLength, int maxLength) 
		{
			InitializeComponent();

			seglen.Minimum = minLength;
			seglen.Maximum = maxLength;
		}

		private DrawCurveOptionsPanel() { InitializeComponent(); }

		public void Register() 
		{
			General.Interface.AddButton(continuousdrawing);
			General.Interface.AddButton(autoclosedrawing);
			General.Interface.AddButton(toolStripSeparator1);
			General.Interface.AddButton(seglabel);
			General.Interface.AddButton(seglen);
			General.Interface.AddButton(reset);
		}

		public void Unregister() 
		{
			General.Interface.RemoveButton(reset);
			General.Interface.RemoveButton(seglen);
			General.Interface.RemoveButton(seglabel);
			General.Interface.RemoveButton(toolStripSeparator1);
			General.Interface.RemoveButton(autoclosedrawing);
			General.Interface.RemoveButton(continuousdrawing);
		}

		private void seglen_ValueChanged(object sender, EventArgs e) 
		{
			if(!blockevents && OnValueChanged != null) OnValueChanged(this, EventArgs.Empty);
		}

		private void reset_Click(object sender, EventArgs e) 
		{
			seglen.Value = seglen.Minimum;
		}

		private void continuousdrawing_CheckedChanged(object sender, EventArgs e)
		{
			if(OnContinuousDrawingChanged != null) OnContinuousDrawingChanged(continuousdrawing.Checked, EventArgs.Empty);
		}

		private void autoclosedrawing_CheckedChanged(object sender, EventArgs e)
		{
			if(OnAutoCloseDrawingChanged != null) OnAutoCloseDrawingChanged(autoclosedrawing.Checked, EventArgs.Empty);
		}
	}
}
