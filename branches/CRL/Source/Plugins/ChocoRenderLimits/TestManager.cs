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

		// Points map
		private Rectangle area;
		private PointData[][] points;
		
		#endregion

		#region ================== Properties

		public List<Test> Tests { get { return tests; } }

		#endregion

		#region ================== Constructor / Destructor

		// Constructor
		public TestManager()
		{
			// Initialize
			ClearPointsMap();
		}

		// Dispose
		public void Dispose()
		{
			foreach(Test t in tests)
				t.Dispose();
		}

		#endregion

		#region ================== Private Methods

		// This resets the points map
		private void ClearPointsMap()
		{
			points = new PointData[0][];
			area = new Rectangle(0, 0, 0, 0);
		}

		// This resizes the points map
		private void ResizePointsMap(Rectangle fitarea)
		{
			if(!area.Contains(fitarea))
			{
				Rectangle newarea = Rectangle.Union(area, fitarea);
				int deltapointsx = (area.X >> 2) - (newarea.X >> 2);
				int deltapointsy = (area.Y >> 2) - (newarea.Y >> 2);
				PointData[][] oldpoints = points;
				points = new PointData[newarea.Height >> 2][];
				for(int i = 0; i < (newarea.Height >> 2); i++) points[i] = new PointData[newarea.Width >> 2];
				for(int y = 0; y < (area.Height >> 2); y++) Array.Copy(oldpoints[y], 0, points[y + deltapointsy], deltapointsx, area.Width >> 2);
				area = newarea;
			}
		}

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
			t.Dispose();
			tests.Remove(t);
		}

		// Update tests
		public void Update()
		{
			foreach(Test t in tests)
				t.Update();
		}

		// Submit test data
		public void ImportTestData(Rectangle pointsarea, List<KeyValuePair<Point, PointData>> addpoints)
		{
			ResizePointsMap(pointsarea);

			for(int i = 0; i < addpoints.Count; i++)
			{
				int arrayx = (addpoints[i].Key.X - area.X) >> 2;
				int arrayy = (addpoints[i].Key.Y - area.Y) >> 2;
				points[arrayy][arrayx] = addpoints[i].Value;
			}
		}

		#endregion
	}
}
