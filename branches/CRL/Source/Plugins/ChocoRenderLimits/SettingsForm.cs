#region === Copyright (c) 2010 Pascal van der Heiden ===

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.ComponentModel;
using System.Windows.Forms;

#endregion

namespace CodeImp.DoomBuilder.Plugins.ChocoRenderLimits
{
	public partial class SettingsForm : Form
	{
		#region ================== Constants

		#endregion

		#region ================== Variables

		#endregion

		#region ================== Properties

		#endregion

		#region ================== Constructor / Destructor

		// Constructor
		public SettingsForm()
		{
			InitializeComponent();

			exepath.Text = BuilderPlug.Me.ExecutablePath;
		}

		#endregion

		#region ================== Methods

		#endregion

		#region ================== Events

		private void apply_Click(object sender, EventArgs e)
		{
			BuilderPlug.Me.ExecutablePath = exepath.Text;
			DialogResult = DialogResult.OK;
			this.Close();
		}

		private void cancel_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.Cancel;
			this.Close();
		}

		private void browseexebutton_Click(object sender, EventArgs e)
		{
			if(openexefile.ShowDialog(this) == DialogResult.OK)
				exepath.Text = openexefile.FileName;
		}

		#endregion
	}
}
