#region === Copyright (c) 2010 Pascal van der Heiden ===

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text;

#endregion

namespace CodeImp.DoomBuilder.Plugins.ChocoRenderLimits
{
	internal class Test
	{
		// Members
		private static int nextid;
		private int id;
		private int threads;
		private int granularity;
		private Rectangle area;
		private TestState state;
		private int progress;
		private List<string> datfiles;
		private List<Process> processes;
		
		// Properties
		public int ID { get { return id; } }
		public int Threads { get { return threads; } }
		public int Granularity { get { return granularity; } }
		public int Progress { get { return progress; } }
		public Rectangle Area { get { return area; } }
		public TestState State { get { return state; } set { state = value; } }

		// Constructor
		public Test(int threads, int granularity, Rectangle area)
		{
			// Initialize
			this.id = nextid++;
			this.threads = threads;
			this.area = area;
			this.granularity = granularity;
			this.state = TestState.NotStarted;
			this.processes = new List<Process>();
			
			// Create DAT filenames
			this.datfiles = new List<string>(threads);
			for(int i = 0; i < threads; i++)
				datfiles.Add(BuilderPlug.MakeTempFilename("dat"));
		}

		// This starts the test
		public void Start()
		{
			if(this.state == TestState.Running)
				throw new Exception("Test is already running!");

			// Write the map
			General.Map.TempPath

			for(int i = 0; i < threads; i++)
			{
				ProcessStartInfo startinfo = new ProcessStartInfo();
				//startinfo.Arguments = "
				Process p = Process.Start(startinfo);
				processes.Add(p);
			}

			this.state = TestState.Running;
		}
	}
}
