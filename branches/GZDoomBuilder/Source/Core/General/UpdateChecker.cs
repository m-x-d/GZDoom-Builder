#region ================== Namespaces

using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Text;
using System.Windows.Forms;
using System.Xml;

#endregion

namespace CodeImp.DoomBuilder
{
	internal static class UpdateChecker
	{
		private delegate DialogResult ShowWarningMessageDelegate(string message, MessageBoxButtons buttons);
		
		private const string NO_UPDATE_REQUIRED = "Your version is up to date.";
		
		private static BackgroundWorker worker;
		private static bool verbose;

		internal static void PerformCheck(bool verbosemode)
		{
			// Update check already runing?
			if(worker != null && worker.IsBusy)
			{
				if(verbosemode) General.ShowWarningMessage("Update check is already running!", MessageBoxButtons.OK);
				return;
			}

			// Start checking
			verbose = verbosemode;
			worker = new BackgroundWorker();
			worker.DoWork += DoWork;
			worker.RunWorkerCompleted += RunWorkerCompleted;
			worker.WorkerSupportsCancellation = true;
			worker.RunWorkerAsync();
		}

		private static void DoWork(object sender, DoWorkEventArgs e)
		{
			string updaterpath = Path.Combine(General.AppPath, "Updater.exe");
			if(!File.Exists(updaterpath))
			{
				ShowResult("Update check failed: \"" + updaterpath + "\" does not exist!");
				e.Cancel = true;
				return;
			} 
			
			string inipath = Path.Combine(General.AppPath, "Updater.ini");
			if(!File.Exists(inipath))
			{
				ShowResult("Update check failed: \"" + inipath + "\" does not exist!");
				e.Cancel = true;
				return;
			}

			string url = GetDownloadUrl(inipath);
			if(string.IsNullOrEmpty(url))
			{
				ShowResult("Update check failed: failed to get update url from Updater.ini!");
				e.Cancel = true;
				return;
			}

			// Get local revision number
			int localrev = General.ThisAssembly.GetName().Version.Revision;
			int actuallocalrev = localrev;
			if(!verbose) localrev = Math.Max(localrev, General.Settings.IgnoredRemoteRevision);

			// Get remote revision number
			int remoterev;
			using(MemoryStream stream = DownloadWebFile(Path.Combine(url, "Version.txt")))
			{
				if(stream == null)
				{
					ShowResult("Update check failed: failed to retrieve remote revision info.\nCheck your Internet connection and try again.");
					e.Cancel = true;
					return;
				}

				string s;
				using(StreamReader reader = new StreamReader(stream))
				{
					s = reader.ReadToEnd();
				}

				if(!int.TryParse(s, out remoterev))
				{
					ShowResult("Update check failed: failed to retrieve remote revision number.");
					e.Cancel = true;
					return;
				}
			}

			if(remoterev > localrev)
			{
				// Get changelog info
				string changelog = GetChangelog(url, actuallocalrev);

				if(string.IsNullOrEmpty(changelog))
				{
					ShowResult("Update check failed: failed to retrieve changelog.\nCheck your Internet connection and try again.");
					e.Cancel = true;
					return;
				}

				// Pass data to MainForm
				General.MainWindow.UpdateAvailable(remoterev, changelog);
			}
			else if(verbose)
			{
				ShowResult(NO_UPDATE_REQUIRED);
			}
		}

		private static void ShowResult(string message)
		{
			if(!string.IsNullOrEmpty(message))
			{
				if(verbose)
				{
					if(General.MainWindow.InvokeRequired)
						General.MainWindow.Invoke(new ShowWarningMessageDelegate(General.ShowWarningMessage), new object[] { message, MessageBoxButtons.OK });
					else
						General.ShowWarningMessage(message, MessageBoxButtons.OK);
				}
				else if(message != NO_UPDATE_REQUIRED)
				{
					General.ErrorLogger.Add(ErrorType.Error, message);
				}
			}
		}

		private static void RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			worker = null;
		}

		private static string GetChangelog(string url, int localrev)
		{
			StringBuilder sb = new StringBuilder(@"{\rtf1");
			
			using(MemoryStream stream = DownloadWebFile(Path.Combine(url, "Changelog.xml")))
			{
				if(stream == null) return string.Empty;
				
				XmlDocument doc = new XmlDocument();
				doc.Load(stream);

				// Revision infos go in descending order
				if(doc.ChildNodes.Count == 0) return string.Empty;
				foreach(XmlNode log in doc.ChildNodes)
				{
					if(log.ChildNodes.Count == 0) continue;
					foreach(XmlNode logentry in log.ChildNodes)
					{
						int noderev;
						if(logentry.Attributes == null || !int.TryParse(logentry.Attributes.GetNamedItem("revision").Value, out noderev)) continue;
						if(noderev <= localrev) break;

						// Add info
						sb.Append(@"{\b R" + noderev + @":}\par ");

						foreach(XmlNode prop in logentry.ChildNodes)
						{
							if(prop.Name == "msg")
							{
								sb.Append(prop.InnerText.Trim().Replace(Environment.NewLine, @"\par ")).Append(@"\par\par ");
								break;
							}
						}
					}
				}
			}

			sb.Append("}");
			return sb.ToString();
		}

		private static MemoryStream DownloadWebFile(string url)
		{
			// Open a data stream from the supplied URL
			WebRequest request = WebRequest.Create(url);
			WebResponse response;

			try
			{
				response = request.GetResponse();
			}
			catch(WebException)
			{
				return null;
			}
			
			Stream source = response.GetResponseStream();
			if(source == null) return null;

			// Download the data in chuncks
			byte[] buffer = new byte[1024];

			// Download the data
			MemoryStream result = new MemoryStream();
			while(!General.MainWindow.IsDisposed)
			{
				// Let's try and read the data
				int numbytes = source.Read(buffer, 0, buffer.Length);
				if(numbytes == 0) break; // Download complete

				// Write the downloaded data
				result.Write(buffer, 0, numbytes);
			}

			// Release resources
			source.Close();

			// Rewind and return the stream
			result.Position = 0;
			return result;
		}

		private static string GetDownloadUrl(string filename)
		{
			string[] lines = File.ReadAllLines(filename);
			foreach(string line in lines)
			{
				if(line.StartsWith("URL")) return line.Substring(3).Trim();
			}

			return string.Empty;
		}
	}
}
