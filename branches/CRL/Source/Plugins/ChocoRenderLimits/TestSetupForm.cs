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
	internal partial class TestSetupForm : Form
	{
		private Test settings;

		// Constructor
		public TestSetupForm()
		{
			InitializeComponent();
		}

		// Setup settings
		public void Setup(Test settings)
		{
			this.settings = settings;
			arealabel.Text = Test.GetAreaDescription(settings.Area);
			if(Array.IndexOf(TestManager.GRANULARITIES, settings.Granularity) > -1)
				granularity.Value = Array.IndexOf(TestManager.GRANULARITIES, settings.Granularity);
			if((settings.Threads >= processes.Minimum) && (settings.Threads <= processes.Maximum))
				processes.Value = settings.Threads;
			granularity_ValueChanged(this, EventArgs.Empty);
			processes_ValueChanged(this, EventArgs.Empty);
		}

		// Apply settings
		private void startbutton_Click(object sender, EventArgs e)
		{
			settings.Granularity = TestManager.GRANULARITIES[granularity.Value];
			settings.Threads = processes.Value;
			
			DialogResult = DialogResult.OK;
			this.Close();
		}
		
		private void granularity_ValueChanged(object sender, EventArgs e)
		{
			granularitylabel.Text = TestManager.GRANULARITIES[granularity.Value].ToString() + " mp";
		}

		private void processes_ValueChanged(object sender, EventArgs e)
		{
			processeslabel.Text = processes.Value.ToString();
		}

		private void cancelbutton_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.Cancel;
			this.Close();
		}
	}
}
