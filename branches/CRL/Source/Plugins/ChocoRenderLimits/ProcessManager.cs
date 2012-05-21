#region === Copyright (c) 2010 Pascal van der Heiden ===

using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Text;

#endregion

namespace CodeImp.DoomBuilder.Plugins.ChocoRenderLimits
{
	internal class ProcessManager
	{
		#region ================== Constants

		#endregion

		#region ================== Variables

		// All running tests
		private List<Test> tests = new List<Test>();

		#endregion

		#region ================== Properties

		#endregion

		#region ================== Constructor / Destructor

		// Constructor
		public ProcessManager()
		{
			// Initialize

		}

		#endregion

		#region ================== Private Methods

		#endregion

		#region ================== Public Methods

		// Create a new test
		public Test StartNewTest(int threads, int granularity, Rectangle area)
		{
			Test t = new Test(threads, granularity, area);
			tests.Add(t);
			return t;
		}


		#endregion
	}
}
