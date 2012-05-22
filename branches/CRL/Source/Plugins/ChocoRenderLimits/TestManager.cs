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
	public class TestManager
	{
		#region ================== Constants

		public static readonly int[] GRANULARITIES = new int[] { 2, 4, 8, 16, 32, 64, 128 };

		#endregion

		#region ================== Variables

		// All tests
		private List<Test> tests = new List<Test>();

		// Point maps
		private Dictionary<Point, PointData>[] points = new Dictionary<Point, PointData>[GRANULARITIES.Length];
		
		#endregion

		#region ================== Properties

		public List<Test> Tests { get { return tests; } }

		#endregion

		#region ================== Constructor / Destructor

		// Constructor
		public TestManager()
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
