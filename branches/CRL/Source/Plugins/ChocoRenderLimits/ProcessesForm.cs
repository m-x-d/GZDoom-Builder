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
			UpdateTestsList();
		}

		#endregion

		#region ================== Methods

		// This updates the list of tests
		private void UpdateTestsList()
		{
			List<Test> tests = BuilderPlug.Me.ProcessManager.Tests;
			
			list.BeginUpdate();
			
			foreach(Test t in tests)
			{
				bool itemfound = false;
				foreach(ListViewItem item in list.Items)
				{
					if((int)item.Tag == t.ID)
					{
						// Update item
						itemfound = true;
						item.SubItems[1].Text = t.Granularity.ToString();
						item.SubItems[2].Text = t.Threads.ToString();
						item.SubItems[3].Text = t.GetStatusDescription();
					}
				}

				if(!itemfound)
				{
					// Create item
					ListViewItem item = list.Items.Add(Test.GetAreaDescription(t.Area));
					item.Tag = t.ID;
					item.SubItems.Add(t.Granularity.ToString());
					item.SubItems.Add(t.Threads.ToString());
					item.SubItems.Add(t.GetStatusDescription());
				}
			}

			// Remove items that are no longer in the list of tests
			for(int i = list.Items.Count - 1; i >= 0; i--)
			{
				ListViewItem item = list.Items[i];
				foreach(Test t in tests)
				{
					if((int)item.Tag == t.ID)
					{
						item = null;
						break;
					}
				}
				if(item != null)
					list.Items.RemoveAt(i);
			}

			list.EndUpdate();
		}

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

		// Update list
		private void updatetimer_Tick(object sender, EventArgs e)
		{
			UpdateTestsList();
		}

		#endregion
	}
}
