using System;
using System.Windows.Forms;

namespace CodeImp.DoomBuilder.AutomapMode
{
	public partial class MenusForm : UserControl
	{
		public event EventHandler OnShowHiddenLinesChanged;
		public event EventHandler OnShowSecretSectorsChanged;
		internal event EventHandler OnColorPresetChanged;

		public bool ShowHiddenLines { get { return showhiddenlines.Checked; } set { showhiddenlines.Checked = value; } }
		public bool ShowSecretSectors { get { return showsecretsectors.Checked; } set { showsecretsectors.Checked = value; } }
		internal AutomapMode.ColorPreset ColorPreset { get { return (AutomapMode.ColorPreset)colorpreset.SelectedIndex; } set { colorpreset.SelectedIndex = (int)value; } }
		
		public MenusForm()
		{
			InitializeComponent();
		}

		public void Register()
		{
			General.Interface.AddButton(showhiddenlines);
			General.Interface.AddButton(showsecretsectors);
			General.Interface.AddButton(colorpresetseparator);
			General.Interface.AddButton(colorpresetlabel);
			General.Interface.AddButton(colorpreset);
		}

		public void Unregister()
		{
			General.Interface.RemoveButton(colorpreset);
			General.Interface.RemoveButton(colorpresetlabel);
			General.Interface.RemoveButton(colorpresetseparator);
			General.Interface.RemoveButton(showsecretsectors);
			General.Interface.RemoveButton(showhiddenlines);
		}

		private void showhiddenlines_CheckedChanged(object sender, EventArgs e)
		{
			if(OnShowHiddenLinesChanged != null) OnShowHiddenLinesChanged(showhiddenlines.Checked, EventArgs.Empty);
		}

		private void showsecretsectors_CheckedChanged(object sender, EventArgs e)
		{
			if(OnShowSecretSectorsChanged != null) OnShowSecretSectorsChanged(showsecretsectors.Checked, EventArgs.Empty);
		}

		private void colorpreset_SelectedIndexChanged(object sender, EventArgs e)
		{
			if(OnColorPresetChanged != null) OnColorPresetChanged(colorpreset.SelectedIndex, EventArgs.Empty);
		}
	}
}
