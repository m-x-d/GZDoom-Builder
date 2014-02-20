using System;
using System.Windows.Forms;
using CodeImp.DoomBuilder.Windows;

namespace CodeImp.DoomBuilder.ColorPicker
{
	public partial class ToolsForm : Form
	{
		public ToolsForm() {
			InitializeComponent();

			General.Interface.AddButton(cpButton, ToolbarSection.Modes);
		}

		private void InvokeTaggedAction(object sender, EventArgs e) {
			General.Interface.InvokeTaggedAction(sender, e);
		}
	}
}
