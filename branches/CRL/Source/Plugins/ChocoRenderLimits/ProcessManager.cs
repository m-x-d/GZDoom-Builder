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
	public class ProcessManager
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

		// Dispose
		public void Dispose()
		{
		}

		#endregion

		#region ================== Private Methods

		#endregion

		#region ================== Public Methods

		// Create a new test
		public Test CreateNewTest(Rectangle area)
		{
			Test t = new Test(area);
			tests.Add(t);
			return t;
		}

		// Remove a test
		public void RemoveTest(Test t)
		{
			tests.Remove(t);
		}

		// Update tests
		public void Update()
		{
			foreach(Test t in tests)
				t.Update();
		}

		#endregion
	}
}
