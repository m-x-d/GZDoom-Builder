#region === Copyright (c) 2010 Pascal van der Heiden ===

using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Text;
using System.Windows.Forms;
using CodeImp.DoomBuilder.Data;
using CodeImp.DoomBuilder.Editing;

#endregion

namespace CodeImp.DoomBuilder.Plugins.ChocoRenderLimits
{
	[EditMode(DisplayName = "ChocoRenderLimits View",
			  SwitchAction = "crl_viewmode",
			  ButtonImage = "CRL.png",
			  ButtonOrder = 300,
			  ButtonGroup = "002_tools",
			  UseByDefault = true)]
	public class ChocoRenderLimitsMode : EditMode
	{
		#region ================== Constants

		#endregion

		#region ================== Variables

		private BitmapImage image;
		private TestManager manager;
		
		#endregion

		#region ================== Properties

		#endregion

		#region ================== Constructor / Destructor

		// Constructor
		public ChocoRenderLimitsMode()
		{
			// Initialize
			manager = BuilderPlug.Me.TestManager;
		}

		#endregion

		#region ================== Methods

		#endregion

		#region ================== Events

		// Mode starts
		public override void OnEngage()
		{
			base.OnEngage();

			// If no test was done yet, do a test now.
			if((manager.PointMap.Length == 0) || manager.Area.IsEmpty)
			{
				Test t = manager.CreateNewTest(Rectangle.Empty);
				TestSetupForm form = new TestSetupForm();
				form.Setup(t);
				if(form.ShowDialog() == DialogResult.OK)
				{
					// Start!
					t.Start();
				}
				else
				{
					// Remove the test
					manager.RemoveTest(t);
				}
			}
		}

		#endregion
	}
}
