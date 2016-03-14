using System;
using System.Windows.Forms;

namespace CodeImp.DoomBuilder.BuilderModes
{
	internal partial class DrawLineOptionsPanel : UserControl
	{
		public event EventHandler OnContinuousDrawingChanged;
		public event EventHandler OnAutoCloseDrawingChanged;

		public bool ContinuousDrawing { get { return continuousdrawing.Checked; } set { continuousdrawing.Checked = value; } }
		public bool AutoCloseDrawing { get { return autoclosedrawing.Checked; } set { autoclosedrawing.Checked = value; } }
		
		public DrawLineOptionsPanel()
		{
			InitializeComponent();
		}

		public void Register()
		{
			General.Interface.AddButton(continuousdrawing);
			General.Interface.AddButton(autoclosedrawing);
		}

		public void Unregister()
		{
			General.Interface.RemoveButton(autoclosedrawing);
			General.Interface.RemoveButton(continuousdrawing);
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
