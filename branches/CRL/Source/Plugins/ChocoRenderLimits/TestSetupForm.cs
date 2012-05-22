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
		private int[] granularities = new int[] { 2, 4, 8, 16, 32, 64, 128 };

		// Constructor
		public TestSetupForm()
		{
			InitializeComponent();
		}

		// Setup settings
		public void Setup(Test settings)
		{
			this.settings = settings;
			if(settings.Area.IsEmpty)
				arealabel.Text = "Full map";
			else
				arealabel.Text = "(" + settings.Area.Left + ", " + settings.Area.Top + ")   \x0336   (" + settings.Area.Right + ", " + settings.Area.Bottom + ")";
			if(Array.IndexOf(granularities, settings.Granularity) > -1)
				granularity.Value = Array.IndexOf(granularities, settings.Granularity);
			if((settings.Threads >= processes.Minimum) && (settings.Threads <= processes.Maximum))
				processes.Value = settings.Threads;
		}

		// Apply settings
		private void startbutton_Click(object sender, EventArgs e)
		{
			settings.Granularity = granularities[granularity.Value];
			settings.Threads = processes.Value;
			
			DialogResult = DialogResult.OK;
			this.Close();
		}
		
		private void granularity_ValueChanged(object sender, EventArgs e)
		{
			granularitylabel.Text = granularities[granularity.Value].ToString() + " mp";
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
