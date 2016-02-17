using System;
using System.Windows.Forms;

namespace CodeImp.DoomBuilder.BuilderModes
{
	internal partial class DrawLineOptionsPanel : UserControl
	{
		public event EventHandler OnContinuousDrawingChanged;

		public bool ContinuousDrawing { get { return continuousdrawing.Checked; } set { continuousdrawing.Checked = value; } }
		
		public DrawLineOptionsPanel()
		{
			InitializeComponent();
		}

		public void Register()
		{
			General.Interface.AddButton(continuousdrawing);
		}

		public void Unregister()
		{
			General.Interface.RemoveButton(continuousdrawing);
		}

		private void continuousdrawing_CheckedChanged(object sender, EventArgs e)
		{
			if(OnContinuousDrawingChanged != null) OnContinuousDrawingChanged(continuousdrawing.Checked, EventArgs.Empty);
		}
	}
}
