using System;
using System.IO;
using System.Windows.Forms;
using System.Threading;

namespace CodeImp.DoomBuilder.GZBuilder.Windows
{
	public partial class ExceptionDialog : Form
	{
		private bool cannotContinue;
		private string logPath;
		
		public ExceptionDialog(UnhandledExceptionEventArgs e) {
			InitializeComponent();

			logPath = Path.Combine(General.SettingsPath, @"GZCrash.txt");
			Exception ex = (Exception)e.ExceptionObject;
			errorDescription.Text = "Error in " + ex.Source + ":";
			using(StreamWriter sw = File.CreateText(logPath)) {
				sw.Write(ex.Source + ": " + ex.Message + Environment.NewLine + ex.StackTrace);
			}

			errorMessage.Text = ex.Message + Environment.NewLine + ex.StackTrace;
			cannotContinue = true;  //cannot recover from this...
		}

		public ExceptionDialog(ThreadExceptionEventArgs e) {
			InitializeComponent();

			logPath = Path.Combine(General.SettingsPath, @"GZCrash.txt");
			errorDescription.Text = "Error in " + e.Exception.Source + ":";
			using(StreamWriter sw = File.CreateText(logPath)) {
				sw.Write(e.Exception.Source + ": " + e.Exception.Message + Environment.NewLine + e.Exception.StackTrace);
			}

			errorMessage.Text = e.Exception.Message + Environment.NewLine + e.Exception.StackTrace;
		}

		public void Setup() {
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
								  "You are making progress. I'm afraid that's something I cannot allow to happen",
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
							  };
			this.Text = titles[new Random().Next(0, titles.Length - 1)];
		}

		private void reportLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
			if(!File.Exists(logPath)) return;
			System.Diagnostics.Process.Start("explorer.exe", @"/select, " + logPath);
			reportLink.LinkVisited = true;
		}

		private void threadLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
			try {
				System.Diagnostics.Process.Start("http://forum.zdoom.org/viewtopic.php?f=3&t=32392&start=9999999");
			} catch(Exception) {
				MessageBox.Show("Unable to open URL...");
			}
			
			threadLink.LinkVisited = true;
		}

		private void bContinue_Click(object sender, EventArgs e) {
			this.DialogResult = DialogResult.OK;
			this.Close();
		}

		private void bToClipboard_Click(object sender, EventArgs e) {
			errorMessage.SelectAll();
			errorMessage.Copy();
			errorMessage.DeselectAll();
		}
	}
}
