#region === Copyright (c) 2010 Pascal van der Heiden ===

using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
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

		// Create a new test
		private void newbutton_Click(object sender, EventArgs e)
		{
			Test t = BuilderPlug.Me.ProcessManager.CreateNewTest(Rectangle.Empty);
			TestSetupForm form = new TestSetupForm();
			form.Setup(t);
			if(form.ShowDialog(this) == DialogResult.OK)
			{
				// Start!
				t.Start();
			}
			else
			{
				// Remove the test
				BuilderPlug.Me.ProcessManager.RemoveTest(t);
			}
		}

		#endregion
	}
}
