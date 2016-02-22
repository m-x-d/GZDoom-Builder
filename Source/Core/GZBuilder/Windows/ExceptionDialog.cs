#region ================== Namespaces

using System;
using System.IO;
using System.Management;
using System.Windows.Forms;
using System.Threading;

#endregion

namespace CodeImp.DoomBuilder.GZBuilder.Windows
{
	public partial class ExceptionDialog : Form
	{
		private readonly bool cannotContinue;
		private readonly string logPath;
		
		public ExceptionDialog(UnhandledExceptionEventArgs e) 
		{
			InitializeComponent();

			logPath = Path.Combine(General.SettingsPath, @"GZCrash.txt");
			Exception ex = (Exception)e.ExceptionObject;
			errorDescription.Text = "Error in " + ex.Source + ":";
			string sysinfo = GetSystemInfo();
			using(StreamWriter sw = File.CreateText(logPath)) 
			{
				sw.Write(sysinfo + GetExceptionDescription(ex));
			}

			errorMessage.Text = ex.Message + Environment.NewLine + ex.StackTrace;
			cannotContinue = true;  //cannot recover from this...
		}

		public ExceptionDialog(ThreadExceptionEventArgs e) 
		{
			InitializeComponent();

			logPath = Path.Combine(General.SettingsPath, @"GZCrash.txt");
			errorDescription.Text = "Error in " + e.Exception.Source + ":";
			string sysinfo = GetSystemInfo();
			using(StreamWriter sw = File.CreateText(logPath)) 
			{
				sw.Write(sysinfo + GetExceptionDescription(e.Exception));
			}

			errorMessage.Text = sysinfo + "********EXCEPTION DETAILS********" + Environment.NewLine 
				+ e.Exception.Message + Environment.NewLine + e.Exception.StackTrace;
		}

		public void Setup() 
		{
			bContinue.Enabled = !cannotContinue;

			string[] titles = {
								  "0x000000 at 0xFFFFFF. That's probaby bad",
								  "Here we go again...",
								  "Uh oh, you're screwed",
								  "All is lost!",
								  "Achievement unlocked: CRASH TIME!",
								  "OH NOES! TEH ERROR!",
								  "0001000001111011000000000011001101011110110111",
								  "Nuclear launch detected!",
								  "Don't send this to Microsoft",
								  "You. Shall. Not. Pass!!!",
								  "Yep, we have bugs",
								  "It's dangerous to go alone. Take this!",
								  "The operation completed successfully",
								  "Security Alert – Moving cursor is not as safe as you thought",
								  "Random error appears from north",
								  "ERROR: NO_ERROR",
								  "Epic fail",
								  "At least it's not BSoD...",
								  "User Error. Please Replace User",
								  "Brought to you by MaxED!",
								  "GZDoom Builder proudly presents:",
								  "You aren't expected to understand this",
								  "Back to the drawing board...",
								  "I'm sorry... :(",
								  "This is a horrbble day for you, and of course, the world",
								  "Abort, Retry, Fail?",
								  "You are making progress. I'm afraid that's something I can't allow to happen",
								  "You are making progress. That's not OK",
								  "No errors found, restarting computer",
								  "Does Not Compute!",
								  "I’m sorry, Dave, I’m afraid I can’t do that",
								  "What's that? Chicken?",
								  "It can only be attributable to human error",
								  "It's now safe to turn off your computer",
								  "I've got a bad feeling about this",
								  "YOU CAN’T DO THAT!",
								  "Man the Lifeboats! Women and children first!",
								  "IMPOSSIBURU!!!",
								  "Now deleting all files. Goodbye",
								  "General Failure",
								  "Invalid Error",
								  "Beam me up Scotty, there’s no life out here",
								  "Well, you ran into something and the game is over",
								  "I'm good at writing bad code",
								  "$FUNNY_ERROR_CAPTION",
								  "In Soviet Russia, exception throws YOU!",
								  "...and then GZDB was the demons!",
								  "B U S T E D",
								  "Freeze mode enabled",
								  "You feel strange...",
								  "That doesn't seem to work",
								  "This function is only available in the retail version of GZDoom Builder",
								  "You picked up the Random Exception.",
								  "Pinky says that you're the new hope. Bear that in mind.",
								  "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA",
								  "Deal with it",
								  "Error 47",
								  "YOU DIED",
								  "Thanks, Obama",
								  "The God Of Exceptions Demands MORE Exceptions!",
								  "Good. It's boring here anyway.",
								  "Shameful display!",
								  "It's CRASHENING!",
								  "W-W-W-WIPEOUT!",
								  "EVERYTHING IS LOST!",
								  "Your empty is full!",
								  "Let's see how far this infinite loop goes...",
							  };
			this.Text = titles[new Random().Next(0, titles.Length - 1)];
		}

		private static string GetSystemInfo()
		{
			string result = "***********SYSTEM INFO***********" + Environment.NewLine;
			
			// Get OS name
			ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT Caption FROM Win32_OperatingSystem");
			foreach(ManagementBaseObject mo in searcher.Get())
			{
				result += "OS: " + mo["Caption"] + Environment.NewLine;
				break;
			}

			// Get GPU name
			searcher = new ManagementObjectSearcher("SELECT * FROM Win32_VideoController");
			foreach(ManagementBaseObject mo in searcher.Get())
			{
				PropertyData bpp = mo.Properties["CurrentBitsPerPixel"];
				PropertyData description = mo.Properties["Description"];
				if(bpp != null && description != null && bpp.Value != null)
				{
					result += "GPU: " + description.Value + Environment.NewLine;
					break;
				}
			}

			// Get GZDB version
			result += "GZDB: R" + General.ThisAssembly.GetName().Version.Revision + Environment.NewLine + Environment.NewLine;

			return result;
		}

		private static string GetExceptionDescription(Exception ex) 
		{
			// Add to error logger
			General.WriteLogLine("***********************************************************");
			General.ErrorLogger.Add(ErrorType.Error, ex.Source + ": " + ex.Message);
			General.WriteLogLine("***********************************************************");

			string message = "********EXCEPTION DETAILS********"
							 + Environment.NewLine + ex.Source + ": " + ex.Message + Environment.NewLine + ex.StackTrace;

			if(File.Exists(General.LogFile)) 
			{
				try 
				{
					string[] lines = File.ReadAllLines(General.LogFile);
					message += Environment.NewLine + Environment.NewLine + "***********ACTIONS LOG***********";
					for(int i = lines.Length - 1; i > -1; i--) 
						message += Environment.NewLine + lines[i];
				} catch(Exception) { }
			}

			return message;
		}

		private void reportLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) 
		{
			if(!File.Exists(logPath)) return;
			System.Diagnostics.Process.Start("explorer.exe", @"/select, " + logPath);
			reportLink.LinkVisited = true;
		}

		private void threadLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) 
		{
			try 
			{
				System.Diagnostics.Process.Start("http://forum.zdoom.org/viewtopic.php?f=3&t=32392&start=9999999");
			} 
			catch(Exception) 
			{
				MessageBox.Show("Unable to open URL...");
			}
			
			threadLink.LinkVisited = true;
		}

		private void bContinue_Click(object sender, EventArgs e) 
		{
			this.DialogResult = DialogResult.OK;
			this.Close();
		}

		private void bQuit_Click(object sender, EventArgs e)
		{
			if(General.Map != null) General.Map.SaveMapBackup();
			this.DialogResult = DialogResult.Cancel;
			this.Close();
		}

		private void bToClipboard_Click(object sender, EventArgs e) 
		{
			errorMessage.SelectAll();
			errorMessage.Copy();
			errorMessage.DeselectAll();
		}
	}
}
