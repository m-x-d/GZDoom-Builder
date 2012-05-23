#region === Copyright (c) 2010 Pascal van der Heiden ===

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows.Forms;
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
		private string tempfile;
		private Process[] processes;
		private StreamReader[] logreaders;
		private float[] percents;
		
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

		// Disposer
		public void Dispose()
		{
			CleanUp();
		}

		// This makes a description for an area
		public static string GetAreaDescription(Rectangle area)
		{
			if(area.IsEmpty)
				return "Full map";
			else
				return "(" + area.Left + ", " + area.Top + ")   \x0336   (" + area.Right + ", " + area.Bottom + ")";
		}

		// This makes a status description
		public string GetStatusDescription()
		{
			switch(state)
			{
				case TestState.NotStarted: return "Not started";
				case TestState.Running: return progress.ToString() + "%";
				case TestState.Complete: return "Complete";
				default: throw new NotImplementedException();
			}
		}

		// Stop processes and clean up
		private void CleanUp()
		{
			if(processes != null)
			{
				for(int i = 0; i < threads; i++)
				{
					try
					{
						if(!processes[i].HasExited)
							processes[i].Kill();

						if(logreaders[i] != null)
						{
							logreaders[i].Close();
							logreaders[i].Dispose();
							logreaders[i] = null;
						}

						string tempdatfile = tempfile + i.ToString(CultureInfo.InvariantCulture) + ".dat";
						string tempppmfile = tempfile + i.ToString(CultureInfo.InvariantCulture) + ".ppm";
						string templogfile = tempfile + i.ToString(CultureInfo.InvariantCulture) + ".log";
						if(File.Exists(tempdatfile)) File.Delete(tempdatfile);
						if(File.Exists(tempppmfile)) File.Delete(tempppmfile);
						if(File.Exists(templogfile)) File.Delete(templogfile);
					}
					catch(Exception)
					{
					}
				}

				try
				{
					string tempwadfile = tempfile + ".wad";
					if(File.Exists(tempwadfile)) File.Delete(tempwadfile);
				}
				catch(Exception)
				{
				}

			}
		}

		// This starts the test
		public void Start()
		{
			if(this.state == TestState.Running)
				throw new Exception("Test is already running!");

			// Make the temp filename. We use this as prefix for all files in each process.
			tempfile = BuilderPlug.MakeTempFilename("");
			
			// Write the map
			string tempwadfile = tempfile + ".wad";
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
			
			// Start the processes!
			processes = new Process[threads];
			logreaders = new StreamReader[threads];
			percents = new float[threads];
			for(int i = 0; i < threads; i++)
			{
				// Create output filename
				string tempdatfile = tempfile + i.ToString(CultureInfo.InvariantCulture) + ".dat";
				string tempppmfile = tempfile + i.ToString(CultureInfo.InvariantCulture) + ".ppm";
				string templogfile = tempfile + i.ToString(CultureInfo.InvariantCulture) + ".log";
				
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
				startinfo.Arguments += " -bruteppmfile \"" + tempppmfile + "\"";
				startinfo.Arguments += " -brutelogfile \"" + templogfile + "\"";
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
				processes[i] = new Process();
				processes[i].StartInfo = startinfo;
				processes[i].Start();
			}

			this.state = TestState.Running;
		}

		// Update test status
		public void Update()
		{
			if(this.state == TestState.Running)
			{
				float totalpercents = 0f;
				bool alldone = true;
				for(int i = 0; i < threads; i++)
				{
					Process p = processes[i];
					if(p.HasExited)
					{
						// This process has exited
						percents[i] = 100f;
						
						// Close the log reader
						if(logreaders[i] != null)
						{
							logreaders[i].Close();
							logreaders[i].Dispose();
							logreaders[i] = null;
						}

						ImportData(i);
					}
					else
					{
						alldone = false;
						
						// Set affinity
						long af = (long)p.ProcessorAffinity;
						if(threads == 1)
							af = 15;
						else
							af = (long)Math.Pow(2, i % Environment.ProcessorCount);
						p.ProcessorAffinity = (IntPtr)af;

						// If we didn't find the log file yet, try finding it now
						if(logreaders[i] == null)
						{
							string templogfile = tempfile + i.ToString(CultureInfo.InvariantCulture) + ".log";
							if(File.Exists(templogfile))
							{
								FileStream fs = File.Open(templogfile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
								logreaders[i] = new StreamReader(fs);
							}
						}

						if(logreaders[i] != null)
						{
							// Parse lines from the log file
							string line = logreaders[i].ReadLine();
							while(!string.IsNullOrEmpty(line))
							{
								// Each line should look like this:
								// CRL: Sector 50 of 88

								line = line.Trim();
								line = line.Replace("CRL: Sector ", "");
								line = line.Replace(" of ", ",");

								// Now the line looks like this: 50,88

								string[] parts = line.Split(',');

								if(parts.Length == 2)
								{
									int sector, max;
									int.TryParse(parts[0], out sector);
									int.TryParse(parts[1], out max);
									percents[i] = ((float)sector / (float)max) * 100f;
								}

								// Next line
								line = logreaders[i].ReadLine();
							}
						}
					}

					totalpercents += percents[i];
				}

				if(alldone)
				{
					// Clean up
					for(int i = 0; i < threads; i++)
						processes[i].Dispose();
					processes = null;
					logreaders = null;
					progress = 100;
					this.state = TestState.Complete;
				}
				else
				{
					progress = (int)(totalpercents / (float)threads);
				}
			}
		}

		// Import the data from one thread
		private void ImportData(int thread)
		{
			Cursor.Current = Cursors.WaitCursor;
			
			string tempdatfile = tempfile + thread.ToString(CultureInfo.InvariantCulture) + ".dat";
			if(File.Exists(tempdatfile))
			{
				// Open the DAT file
				FileStream fs = File.Open(tempdatfile, FileMode.Open, FileAccess.Read, FileShare.Read);
				BinaryReader datreader = new BinaryReader(fs);

				// Read header
				uint width = datreader.ReadUInt32();
				uint height = datreader.ReadUInt32();
				float x = (float)datreader.ReadInt32() / 65536.0f;
				float y = (float)datreader.ReadInt32() / 65536.0f;
				float gran = (float)datreader.ReadInt32() / 65536.0f;

				// We ignore the above read values and calculate the area ourself from the points
				Rectangle pointsarea = Rectangle.Empty;

				// Read point data
				List<KeyValuePair<Point, PointData>> points = new List<KeyValuePair<Point, PointData>>();
				while(fs.Position < fs.Length - 1)
				{
					Point p = new Point();
					p.X = datreader.ReadInt16();
					p.Y = datreader.ReadInt16();
					bool isvalid = datreader.ReadByte() != 0;
					if(isvalid)
					{
						PointData pd = new PointData();
						pd.visplanes = datreader.ReadUInt32();
						pd.drawsegs = datreader.ReadUInt32();
						pd.openings = datreader.ReadUInt32();
						pd.sprites = datreader.ReadUInt32();
						points.Add(new KeyValuePair<Point, PointData>(p, pd));

						if(pointsarea.IsEmpty)
						{
							// First point
							pointsarea = new Rectangle(p.X, p.Y, 1, 1);
						}
						else
						{
							// Enlarge the area to contain this point
							if(p.X < pointsarea.X) pointsarea.X = p.X;
							if(p.Y < pointsarea.Y) pointsarea.Y = p.Y;
							if(p.X > pointsarea.Right) pointsarea.Width = p.X - pointsarea.X + 1;
							if(p.Y > pointsarea.Bottom) pointsarea.Height = p.Y - pointsarea.Y + 1;
						}
					}
				}

				// Close the file
				datreader.Close();
				fs.Close();
				fs.Dispose();

				// Our area could differ from area in other threads and we don't want to
				// resize our point grid more than once, so we enlarge our area alightly
				// to ensure it will contain everything in this test.
				if(!area.IsEmpty) pointsarea = Rectangle.Union(pointsarea, area);
				pointsarea.Inflate(granularity, granularity);

				// Apply to point grid
				BuilderPlug.Me.TestManager.ImportTestData(pointsarea, points);
			}
			else
			{
				// Test failed!
				CleanUp();
				state = TestState.Failed;
			}
			
			Cursor.Current = Cursors.Default;
		}

		// Cancel the test
		public void Abort()
		{
			CleanUp();
			state = TestState.NotStarted;
		}
	}
}
