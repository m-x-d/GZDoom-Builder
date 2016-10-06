#region ======================== Namespaces

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Threading;
using SharpCompress.Archive;
using SharpCompress.Common;
using SharpCompress.Reader;
using System.Security.AccessControl;
using System.Security.Principal;

#endregion

namespace mxd.GZDBUpdater
{
    public partial class MainForm : Form
	{
		#region ======================== Variables

		private string processToEnd = string.Empty;
		private string downloadFile = string.Empty;
		private const string revisionwildcard = "[REVNUM]";
		private string URL = string.Empty;
        private readonly string updateFolder = Application.StartupPath + @"\_update\";
		private string appFileName = string.Empty;
	    private static BackgroundWorker worker;
	    private static bool appclosing;
	    private static MainForm me;
	    private const string ERROR_TITLE = "Updater";

		#endregion

		#region ======================== Delegates

		private delegate void SetLabelCallback(Label label, string text);
		private delegate void UpdateProgressBarCallback(ByteArgs args, int step, int totalsteps);
		private delegate void CloseDelegate();

		#endregion

		#region ======================== Properties

		public static string ErrorDescription;
	    public static bool AppClosing { get { return appclosing; } }

		#endregion

		#region ======================== Constructor

		public MainForm()
        {
            if(!CheckPremissions(Application.StartupPath))
            {
                ErrorDescription = "Update failed: your account does not have write access to the destination folder \"" + Application.StartupPath + "\"";
                InvokeClose();
            }
            else if(!File.Exists("Updater.ini"))
			{
				ErrorDescription = "Unable to locate 'Updater.ini'...";
				InvokeClose();
			}
            else if(!LoadConfig("Updater.ini"))
			{
				InvokeClose();
			}
    		else
			{
				me = this;
				InitializeComponent();
			}
		}

		#endregion

		#region ======================== Updater thread

	    private void BackgroundWorker(object sender, DoWorkEventArgs e)
        {
			UpdateLabel(label1, "1/6: Checking revisions...");
			if(!UpdateRequired())
			{
				e.Cancel = true;
				return;
			}
            PreDownload();

			UpdateLabel(label1, "2/6: Downloading Update...");
			Webdata.BytesDownloaded += WebdataOnBytesDownloaded;
            if(!Webdata.SaveWebFile(URL, downloadFile, updateFolder))
            {
                e.Cancel = true;
				Webdata.BytesDownloaded -= WebdataOnBytesDownloaded;
				return;
            }

			// Check if the editor is running...
			try
			{
				Process[] processes = Process.GetProcesses();
				List<Process> toclose = new List<Process>();
				
				// Gather all running editor processes...
				foreach(Process process in processes)
				{
					if(process.ProcessName == processToEnd 
						&& Path.GetDirectoryName(process.MainModule.FileName) == Application.StartupPath)
					{
						toclose.Add(process);
					}
				}

				// Close them
				if(toclose.Count > 0)
				{
					TaskbarProgress.SetState(this.Handle, TaskbarProgress.TaskbarStates.Paused);
					if(MessageBox.Show(this, "The editor needs to be closed.\n\nPress OK to close it and proceed with update.\nPress Cancel to cancel update.",
						ERROR_TITLE, MessageBoxButtons.OKCancel) == DialogResult.OK)
					{
						UpdateLabel(label1, "3/6: Stopping " + processToEnd);
						Thread.Sleep(500);

						foreach(Process p in toclose)
						{
							if(p != null) p.Kill();
						}
					}
					else
					{
						e.Cancel = true;
						return;
					}
				}
			}
			catch(Exception ex)
			{
				ErrorDescription = "Failed to stop the main process...\n" + ex.Message;
				e.Cancel = true;
				return;
			}

			UpdateLabel(label1, "4/6: Decompressing package...");
            Thread.Sleep(500);
			if(!Unpack(updateFolder + downloadFile, Application.StartupPath))
			{
				e.Cancel = true;
				return;
			}

			UpdateLabel(label1, "5/6: Moving files...");
            Thread.Sleep(500);
            MoveFiles();
            
			UpdateLabel(label1, "6/6: Wrapping up...");
            Thread.Sleep(500);
			PostDownload();
		}

	    private static void StopBackgroundWorker()
		{
			if(worker != null && !worker.CancellationPending)
			{
				me.UpdateLabel(me.label1, "Stopping Background Thread...");
				worker.CancelAsync();
				while(worker.IsBusy) Application.DoEvents();
			}
		}

		private void WorkerOnRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			InvokeClose();
		}

		#endregion

		#region ======================== Methods

		private void UpdateLabel(Label label, string text)
		{
			if(label.InvokeRequired)
			{
				SetLabelCallback d = UpdateLabel;
				label.Invoke(d, new object[] { label, text });
			}
			else
			{
				label.Text = text;
				label.Refresh();
				Invalidate();
			}
		}

		private void InvokeClose()
		{
			if(this.Disposing || this.IsDisposed) return;
			if(this.InvokeRequired)
			{
				CloseDelegate d = Close;
				this.Invoke(d);
			}
			else
			{
				if(!appclosing && !string.IsNullOrEmpty(ErrorDescription))
				{
					if(!string.IsNullOrEmpty(URL))
					{
						ErrorDescription += Environment.NewLine + Environment.NewLine + "Would you like to download the update manually?";
						TaskbarProgress.SetState(this.Handle, TaskbarProgress.TaskbarStates.Error);
						if(MessageBox.Show(this, ErrorDescription, ERROR_TITLE, MessageBoxButtons.YesNo) == DialogResult.Yes)
							Process.Start(URL);
					}
					else
					{
						MessageBox.Show(this, ErrorDescription, ERROR_TITLE, MessageBoxButtons.OK);
					}
				}

				WrapUp();
				Close();
			}
		}
		
		private bool UpdateRequired()
		{
			// Get local revision number
			int localrev = -1;
			if(File.Exists(appFileName))
			{
				var info = FileVersionInfo.GetVersionInfo(appFileName);
				localrev = info.ProductPrivatePart;
			}
			
			// Get remote revision number
			int remoterev;
			using(MemoryStream stream = Webdata.DownloadWebFile(Path.Combine(URL, "Version.txt")))
			{
				if(stream == null)
				{
					ErrorDescription = "Failed to retrieve remote revision info.";
					return false;
				}

				string s;
				using(StreamReader reader = new StreamReader(stream))
				{
					s = reader.ReadToEnd();
				}

				if(!int.TryParse(s, out remoterev))
				{
					ErrorDescription = "Failed to retrieve remote revision number.";
					return false;
				}
			}

			// Replace wildcard with remoterev
			downloadFile = downloadFile.Replace(revisionwildcard, remoterev.ToString());

			if(remoterev > 0 && remoterev <= localrev)
			{
				URL = string.Empty;
				ErrorDescription = "Your version is up to date!";
			}
			return remoterev > localrev;
		}

		private bool LoadConfig(string filename)
		{
			string[] lines = File.ReadAllLines(filename);
			foreach (string line in lines)
			{
				if(line.StartsWith("URL"))
				{
					URL = line.Substring(3).Trim();
				} 
				else if(line.StartsWith("FileName"))
				{
					appFileName = line.Substring(8).Trim();
					processToEnd = Path.GetFileNameWithoutExtension(appFileName);
				}
				else if(line.StartsWith("UpdateName"))
				{
					downloadFile = line.Substring(10).Trim();
				}
			}

			// Sanity cheks
			if(string.IsNullOrEmpty(URL))
			{
				ErrorDescription = "URL is not specified in " + filename + "!";
				return false;
			}

			if(string.IsNullOrEmpty(appFileName) || string.IsNullOrEmpty(processToEnd))
			{
				ErrorDescription = "FileName is not specified in " + filename + "!";
				return false;
			}

			if(string.IsNullOrEmpty(downloadFile) || !downloadFile.Contains(revisionwildcard))
			{
				ErrorDescription = "UpdateName is invalid or not specified in " + filename + "!";
				return false;
			}

			return true;
		}

        private bool Unpack(string file, string unZipTo)
        {
            try
            {
				using(IArchive arc = ArchiveFactory.Open(file))
				{
					if(!arc.IsComplete)
					{
						ErrorDescription = "Update failed: downloaded file is not complete...";
						return false;
					}

					IReader reader = arc.ExtractAllEntries();

					// Get number of files...
					int curentry = 0;
					int totalentries = 0;
					foreach(var entry in arc.Entries) totalentries++;

					string ourname = Path.GetFileName(Process.GetCurrentProcess().MainModule.FileName);

					// Unpack all
					while(reader.MoveToNextEntry())
					{
						if(appclosing) break;
						if(reader.Entry.IsDirectory || Path.GetFileName(reader.Entry.Key) == ourname) continue; // Don't try to overrite ourselves...
						reader.WriteEntryToDirectory(unZipTo, ExtractOptions.ExtractFullPath | ExtractOptions.Overwrite);
						UpdateProgressBar(new ByteArgs { Downloaded = curentry++, Total = totalentries }, 1, 2);
					}
				}
            } 
			catch(Exception e) 
			{ 
				ErrorDescription = "Update failed: failed to unpack the update...\n" + e.Message;
				return false;
			}

	        return true;
        }

        private static bool CheckPremissions(string path)
        {
            try
            {
                DirectoryInfo di = new DirectoryInfo(path);
                DirectorySecurity acl = di.GetAccessControl();
                AuthorizationRuleCollection rules = acl.GetAccessRules(true, true, typeof(NTAccount));

                WindowsIdentity currentUser = WindowsIdentity.GetCurrent();
                WindowsPrincipal principal = new WindowsPrincipal(currentUser);
                foreach(AuthorizationRule rule in rules)
                {
                    FileSystemAccessRule fsAccessRule = rule as FileSystemAccessRule;
                    if(fsAccessRule == null) continue;

                    if((fsAccessRule.FileSystemRights & FileSystemRights.WriteData) > 0)
                    {
                        NTAccount ntAccount = rule.IdentityReference as NTAccount;
                        if(ntAccount == null) continue;
                        if(principal.IsInRole(ntAccount.Value)) return true;
                    }
                }
            }
            catch(UnauthorizedAccessException) { }

            return false;
        }

	    private void PreDownload()
        {
            if(!Directory.Exists(updateFolder)) Directory.CreateDirectory(updateFolder);
        }

        private void PostDownload()
        {
            if(!File.Exists(appFileName))
            {
	            ErrorDescription = "Unable to located updated executable ('" + appFileName + "')";
				return;
            }

			if(appclosing) return;
			Process.Start(new ProcessStartInfo { FileName = appFileName });
        }

        private void WrapUp()
        {
			if(Directory.Exists(updateFolder)) Directory.Delete(updateFolder, true);
        }

        private void MoveFiles()
        {
			DirectoryInfo di = new DirectoryInfo(updateFolder);
            FileInfo[] files = di.GetFiles();

            foreach (FileInfo fi in files)
            {
				if(fi.Name != downloadFile) File.Copy(updateFolder + fi.Name, Application.StartupPath + fi.Name, true);
            }
        }

		private void UpdateProgressBar(ByteArgs e, int step, int totalsteps)
        {
			if(progressbar.InvokeRequired)
			{
				UpdateProgressBarCallback d = UpdateProgressBar;
				progressbar.Invoke(d, new object[] { e, step, totalsteps });
			} 
			else 
			{
				int stepsize = (int)Math.Round((float)progressbar.Maximum / totalsteps);
				float ratio = (float)e.Downloaded / e.Total;
				int val = (int)Math.Floor(stepsize * step + stepsize * ratio);

				if(val <= progressbar.Maximum)
				{
					progressbar.Value = val;
					TaskbarProgress.SetValue(this.Handle, progressbar.Value, progressbar.Maximum);
				}
				progressbar.Refresh();
				Invalidate();
			}
		}

		#endregion

		#region ======================== Events

		private void MainForm_Load(object sender, EventArgs e)
		{
			Version version = Assembly.GetEntryAssembly().GetName().Version;
			this.Text += " v" + version.Major + "." + version.Minor;

			worker = new BackgroundWorker();
			worker.DoWork += BackgroundWorker;
			worker.RunWorkerCompleted += WorkerOnRunWorkerCompleted;
			worker.WorkerSupportsCancellation = true;
			worker.RunWorkerAsync();
		}

		private void cancel_Click(object sender, EventArgs e)
		{
			ErrorDescription = string.Empty;
			appclosing = true;
			StopBackgroundWorker();
			InvokeClose();
		}

		private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			appclosing = true;
			StopBackgroundWorker();
		}

		private void WebdataOnBytesDownloaded(ByteArgs ba)
		{
			UpdateProgressBar(ba, 0, 2);
		}

		#endregion
	}
}