#region === Copyright (c) 2010 Pascal van der Heiden ===

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text;
using CodeImp.DoomBuilder.Data;

#endregion

namespace CodeImp.DoomBuilder.Plugins.ChocoRenderLimits
{
	public class Test
	{
		// Members
		private static int nextid;
		private int id;
		private int threads;
		private int granularity;
		private Rectangle area;
		private TestState state = TestState.NotStarted;
		private int progress;
		private string tempwadfile;
		private List<string> datfiles = new List<string>();
		private List<Process> processes = new List<Process>();
		
		// Properties
		public int ID { get { return id; } }
		public int Threads { get { return threads; } set { threads = value; } }
		public int Granularity { get { return granularity; } set { granularity = value; } }
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
		}

		// Constructor
		public Test(Rectangle area)
		{
			// Initialize
			this.id = nextid++;
			this.area = area;
		}

		// This starts the test
		public void Start()
		{
			if(this.state == TestState.Running)
				throw new Exception("Test is already running!");

			datfiles.Clear();
			
			// Write the map
			tempwadfile = BuilderPlug.MakeTempFilename("wad");
			General.Map.ExportToFile(tempwadfile);

			// Find the resource file locations
			DataLocation iwadlocation;
			General.Map.Data.FindFirstIWAD(out iwadlocation);
			DataLocation maplocation = new DataLocation(DataLocation.RESOURCE_WAD, tempwadfile, false, false, false);
			DataLocationList locations = new DataLocationList();
			locations.AddRange(General.Map.ConfigSettings.GetResources());
			locations.AddRange(General.Map.Options.GetResources());
			locations.Add(maplocation);

			// Try finding the L1 and L2 numbers from the map name
			string numstr = "";
			bool first = true;
			string p_l1 = "", p_l2 = "";
			const string NUMBERS = "0123456789";
			foreach(char c in General.Map.Options.LevelName)
			{
				// Character is a number?
				if(NUMBERS.IndexOf(c) > -1)
				{
					// Include it
					numstr += c;
				}
				else
				{
					// Store the number if we found one
					if(numstr.Length > 0)
					{
						int num = 0;
						int.TryParse(numstr, out num);
						if(first) p_l1 = num.ToString(); else p_l2 = num.ToString();
						numstr = "";
						first = false;
					}
				}
			}

			// Store the number if we found one
			if(numstr.Length > 0)
			{
				int num = 0;
				int.TryParse(numstr, out num);
				if(first) p_l1 = num.ToString(); else p_l2 = num.ToString();
			}
			
			for(int i = 0; i < threads; i++)
			{
				// Create output filename
				string tempdatfile = BuilderPlug.MakeTempFilename("dat");
				datfiles.Add(tempdatfile);
				Console.WriteLine("Temp dat file: " + tempdatfile);
				
				// Setup launch parameters
				ProcessStartInfo startinfo = new ProcessStartInfo();
				startinfo.Arguments = "-bruteforce -dummysdl -noblit -nomouse -nosound -nosfx -nomusic -nomonsters";
				startinfo.Arguments += " -iwad \"" + iwadlocation.location + "\"";
				foreach(DataLocation dl in locations)
				{
					if((dl.type != DataLocation.RESOURCE_WAD) || (dl.location != iwadlocation.location))
					{
						if(!dl.notfortesting)
							startinfo.Arguments += " -file \"" + dl.location + "\"";
					}
				}
				startinfo.Arguments += " -warp " + p_l1 + " " + p_l2;
				startinfo.Arguments += " -brutedatfile \"" + tempdatfile + "\"";
				startinfo.Arguments += " -bruteppmfile \"" + tempdatfile + ".ppm\"";
				startinfo.Arguments += " -brutegran " + granularity;
				startinfo.Arguments += " -brutestep " + threads;
				startinfo.Arguments += " -brutestepoffset " + i;
				if(!area.IsEmpty) startinfo.Arguments += " -brutebounds " + area.Left + " " + area.Top + " " + area.Right + " " + area.Bottom;
				startinfo.CreateNoWindow = true;
				startinfo.ErrorDialog = false;
				startinfo.UseShellExecute = false;
				startinfo.FileName = BuilderPlug.Me.ExecutablePath;
				startinfo.WorkingDirectory = Path.GetDirectoryName(startinfo.FileName);
				startinfo.RedirectStandardOutput = true;
				startinfo.RedirectStandardError = true;
				
				// Start process!
				Process p = new Process();
				p.StartInfo = startinfo;
				processes.Add(p);
				p.OutputDataReceived += p_OutputDataReceived;
				p.ErrorDataReceived += p_OutputDataReceived;
				p.EnableRaisingEvents = true;
				p.Start();
				p.BeginOutputReadLine();
			}

			this.state = TestState.Running;
		}

		// A process outputs data
		private void p_OutputDataReceived(object sender, DataReceivedEventArgs e)
		{
			int index = processes.IndexOf(sender as Process);
			Console.WriteLine(index + ": " + e.Data);
		}

		// Update test status
		public void Update()
		{
			if(this.state == TestState.Running)
			{
				float percents = 0f;
				bool alldone = true;
				for(int i = 0; i < threads; i++)
				{
					Process p = processes[i];
					if(p.HasExited)
					{
						percents += 100f;
					}
					else
					{
						alldone = false;
						
						// Set affinity
						long af = (long)p.ProcessorAffinity;
						af = (i >= Environment.ProcessorCount) || (threads == 1) ? 255 : (long)Math.Pow(2, i);
						p.ProcessorAffinity = (IntPtr)af;
					}
				}

				if(alldone)
				{
					// Clean up
					for(int i = 0; i < threads; i++)
						processes[i].Dispose();
					processes.Clear();
					progress = 100;
					this.state = TestState.Complete;
				}
				else
				{
					progress = (int)(percents / (float)threads);
				}
			}
		}
	}
}
