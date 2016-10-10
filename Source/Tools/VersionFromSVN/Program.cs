#region ======================== Namespaces

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;

#endregion

namespace mxd.VersionFromSVN
{
	public static class Program
	{
		#region ======================== Constants

		private const string SVN_INFO = "svn info -r HEAD > versionfromsvn.tmp\r\n";

		#endregion

		#region ======================== Main

		[STAThread]
		private static int Main(string[] args)
		{
			bool showusageinfo = true;
			bool dorevisionlookup = true;
			bool reverttargets = false;
			string revision = "";
			string apppath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			string revisionoutputfile = "";

			List<string> targetfiles = new List<string>();
			Queue<string> queue = new Queue<string>(args);
			
			if(string.IsNullOrEmpty(apppath))
			{
				Console.WriteLine("Invalid working directory: '" + apppath + "'");
				return 2;
			}

			while(queue.Count > 0)
			{
				string arg = queue.Dequeue();
				if(string.Compare(arg, "-F", true) == 0)
				{
					if(queue.Count > 0)
					{
						revision = queue.Dequeue();
						dorevisionlookup = false;
					}
					else
					{
						showusageinfo = true;
						break;
					}
				}
				else if(string.Compare(arg, "-O", true) == 0)
				{
					if(queue.Count > 0) revisionoutputfile = queue.Dequeue();
				}
				else if(string.Compare(arg, "-R", true) == 0)
				{
					dorevisionlookup = false;
					reverttargets = true;
				}
				else
				{
					targetfiles.Add(arg);
					showusageinfo = false;
				}
			}

			if(showusageinfo)
			{
				ShowUsageInfo();
				Console.ReadKey();
				return 0;
			}

			if(dorevisionlookup)
			{
				Console.WriteLine("Looking up latest subversion revision...");

				string tmppath = Path.Combine(apppath, "versionfromsvn.tmp");
				string batpath = Path.Combine(apppath, "versionfromsvn.bat");
				File.WriteAllText(batpath, SVN_INFO);
					
				ProcessStartInfo info = new ProcessStartInfo
				                        {
					                        FileName = batpath,
											CreateNoWindow = false,
											ErrorDialog = false,
											UseShellExecute = true,
											WindowStyle = ProcessWindowStyle.Hidden,
											WorkingDirectory = apppath
				                        };

				Process.Start(info).WaitForExit();

				if(File.Exists(tmppath))
				{
					foreach(string str in File.ReadAllLines(tmppath))
					{
						if(str.StartsWith("Revision", StringComparison.InvariantCultureIgnoreCase))
						{
							string[] parts = str.Split(new[]{ ' ' });
							if(parts.Length > 1)
							{
								revision = parts[1];
								Console.Write(revision);
								dorevisionlookup = false;
							}
						}
					}

					File.Delete(tmppath);
				}

				File.Delete(batpath);
			}


			if(dorevisionlookup)
			{
				Console.WriteLine("Unable to find revision number from Subversion. You must install Subversion from http://subversion.tigris.org/.");
				return 1;
			}

			if(!string.IsNullOrEmpty(revisionoutputfile))
				File.AppendAllText(revisionoutputfile, "SET REVISIONNUMBER=" + revision + "\n");
					
			if(reverttargets)
			{
				Console.WriteLine("Reverting changes in version files...");

				string contents = "";
				foreach(string str in targetfiles)
					contents += "svn revert \"" + str + "\"\r\n";
						
				string path = Path.Combine(apppath, "versionfromsvn.bat");
				File.WriteAllText(path, contents);

				ProcessStartInfo info = new ProcessStartInfo
				                             {
					                             FileName = path,
												 CreateNoWindow = false,
												 ErrorDialog = false,
												 UseShellExecute = true,
												 WindowStyle = ProcessWindowStyle.Hidden,
												 WorkingDirectory = apppath
				                             };

				Process.Start(info).WaitForExit();
				File.Delete(path);
			}
			else
			{
				Console.WriteLine("Writing revision " + revision + " to target files...");
				foreach(string file in targetfiles)
				{
					bool changed = false;
					string[] contents = File.ReadAllLines(file);

					for(int i = 0; i < contents.Length; ++i)
					{
						string line = contents[i];
						if(line.Trim().StartsWith("[assembly: AssemblyVersion", true, CultureInfo.InvariantCulture))
						{
							int startbracepos = line.IndexOf("\"", 0);
							int endbracepos = line.IndexOf("\"", startbracepos + 1);
							int revisiondotpos = line.LastIndexOf(".", endbracepos, endbracepos - startbracepos);

							string linestart = line.Substring(0, revisiondotpos + 1);
							string lineend = line.Substring(endbracepos);
							string result = linestart + revision + lineend;

							if(string.Compare(contents[i], result, true) != 0)
							{
								contents[i] = result;
								changed = true;
							}
						}
					}

					if(changed)
					{
						File.Copy(file, file + ".backup");
						File.WriteAllLines(file, contents);
						File.Delete(file + ".backup");
					}
				}
			}

			Console.WriteLine("Done.");
			return 0;
		}

		#endregion

		#region ======================== Usage info

		private static void ShowUsageInfo()
		{
			Console.WriteLine("Version From SVN is a tool programmed by Pascal vd Heiden.\r\nThis tool requires that Subversion commandline client is installed.\r\nSee http://subversion.tigris.org/\r\n");
			Console.WriteLine("Usage: versionfromsvn.exe targetfile [targetfile ...] [options]");
			Console.WriteLine("Where targetfile is any text file in which the version placeholder %SVNREVISION% must be replaced with the latest revision from the SVN working copy.\r\n");

			Console.WriteLine("Options:\r\n");
			Console.WriteLine("-F number");
			Console.WriteLine("When this is used, the given number will be used as revision number and this program will not use Subversion to look for the real revision number (faster).\r\n");

			Console.WriteLine("-R");
			Console.WriteLine("This will revert all changes in the specified target files (same as SVN revert function). This will not apply any revision number to the target files.\r\n");

			Console.WriteLine("-O filename");
			Console.WriteLine("Creates a bath file, which sets REVISIONNUMBER environment variable to the SVN revision number.\r\n");

			Console.WriteLine("Press any key to quit.");
		}

		#endregion
	}
}
