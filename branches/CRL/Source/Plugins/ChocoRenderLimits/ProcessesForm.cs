#region === Copyright (c) 2010 Pascal van der Heiden ===

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.ComponentModel;
using System.Windows.Forms;
using CodeImp.DoomBuilder.Windows;

#endregion

namespace CodeImp.DoomBuilder.Plugins.ChocoRenderLimits
{
	public partial class ProcessesForm : DelayedForm
	{
		#region ================== Constants

		#endregion

		#region ================== Variables

		#endregion

		#region ================== Properties

		#endregion

		#region ================== Constructor / Destructor

		// Constructor
		public ProcessesForm()
		{
			InitializeComponent();
		}

		#endregion

		#region ================== Methods

		#endregion

		#region ================== Events

		private void closebutton_Click(object sender, EventArgs e)
		{
			Close();
		}

		#endregion
	}
}
