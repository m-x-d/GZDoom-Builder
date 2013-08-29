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
			errorDescription.Text = "Error in " + ex.Source + ": " + ex.Message;
			using(StreamWriter sw = File.CreateText(logPath)) {
				sw.Write(ex.Source + ": " + ex.Message + Environment.NewLine + ex.StackTrace);
			}

			errorMessage.Text = ex.StackTrace;
			cannotContinue = true;  //cannot recover from this...
		}

		public ExceptionDialog(ThreadExceptionEventArgs e) {
			InitializeComponent();

			logPath = Path.Combine(General.SettingsPath, @"GZCrash.txt");
			errorDescription.Text = "Error in " + e.Exception.Source + ": " + e.Exception.Message;
			using(StreamWriter sw = File.CreateText(logPath)) {
				sw.Write(e.Exception.Source + ": " + e.Exception.Message + Environment.NewLine + e.Exception.StackTrace);
			}

			errorMessage.Text = e.Exception.StackTrace;
		}

		public void Setup() {
			bContinue.Enabled = !cannotContinue;
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
