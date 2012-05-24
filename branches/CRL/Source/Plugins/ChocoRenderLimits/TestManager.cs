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

		public static readonly int[] GRANULARITIES = new int[] { 1, 2, 4, 8, 16, 32, 64, 128 };

		public static int POINTMAP_GRAN = 4;
		public static int POINTMAP_SHIFT = 2;
		
		#endregion

		#region ================== Variables

		// All tests
		private List<Test> tests = new List<Test>();

		// Points map
		private Rectangle area;
		private PointData[][] pointmap;
		
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
			pointmap = new PointData[0][];
			area = new Rectangle(0, 0, 0, 0);
		}

		// This resizes the points map
		private void ResizePointsMap(Rectangle fitarea)
		{
			if(!area.Contains(fitarea))
			{
				Rectangle newarea = Rectangle.Union(area, fitarea);
				int deltapointsx = (area.X >> POINTMAP_SHIFT) - (newarea.X >> POINTMAP_SHIFT);
				int deltapointsy = (area.Y >> POINTMAP_SHIFT) - (newarea.Y >> POINTMAP_SHIFT);
				PointData[][] oldpoints = pointmap;
				pointmap = new PointData[newarea.Height >> POINTMAP_SHIFT][];
				for(int i = 0; i < (newarea.Height >> POINTMAP_SHIFT); i++) pointmap[i] = new PointData[newarea.Width >> POINTMAP_SHIFT];
				for(int y = 0; y < (area.Height >> POINTMAP_SHIFT); y++) Array.Copy(oldpoints[y], 0, pointmap[y + deltapointsy], deltapointsx, area.Width >> POINTMAP_SHIFT);
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
		public void ImportTestData(Rectangle pointsarea, List<KeyValuePair<Point, PointData>> addpoints, int granularity)
		{
			if(granularity < POINTMAP_GRAN) throw new Exception("The fixed points map is for granularities " + POINTMAP_GRAN + " and higher only.");
			
			ResizePointsMap(pointsarea);

			// Resample and apply the points to the granularity of the pointsmap
			int pointsize = granularity / POINTMAP_GRAN;
			int pointsizeadd = pointsize / 2;
			int pointsizesub = pointsize - pointsizeadd;
			foreach(KeyValuePair<Point, PointData> t in addpoints)
			{
				int arrayx = (t.Key.X - area.X) >> POINTMAP_SHIFT;
				int arrayy = (t.Key.Y - area.Y) >> POINTMAP_SHIFT;
				for(int x = arrayx - pointsizesub; x <= arrayx + pointsizeadd; x++)
					for(int y = arrayy - pointsizesub; y <= arrayy + pointsizeadd; y++)
						pointmap[y][x] = t.Value;
			}
		}

		#endregion
	}
}
