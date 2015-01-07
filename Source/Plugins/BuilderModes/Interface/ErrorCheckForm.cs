
#region ================== Copyright (c) 2007 Pascal vd Heiden

/*
 * Copyright (c) 2007 Pascal vd Heiden, www.codeimp.com
 * This program is released under GNU General Public License
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 */

#endregion

#region ================== Namespaces

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Windows;
using System.Reflection;
using System.Globalization;
using System.Threading;
using System.IO;

#endregion

namespace CodeImp.DoomBuilder.BuilderModes
{
	public partial class ErrorCheckForm : DelayedForm
	{
		#region ================== Constants

		#endregion

		#region ================== Delegates

		private delegate void CallVoidMethodDeletage();
		private delegate void CallIntMethodDelegate(int i);
		private delegate void CallResultMethodDelegate(ErrorResult r);
		
		#endregion
		
		#region ================== Variables
		
		private volatile bool running;
		private Thread checksthread;
		private BlockMap<BlockEntry> blockmap;
		private static bool applyToAll; //mxd
		private Size initialsize; //mxd
		private static int exportmode; //mxd
		private List<ErrorResult> resultslist; //mxd 
		private List<Type> hiddentresulttypes; //mxd 
		
		#endregion

		#region ================== Properties
		
		public ErrorResult SelectedResult { get { return results.SelectedItem as ErrorResult; } }
		public BlockMap<BlockEntry> BlockMap { get { return blockmap; } }
		
		#endregion
		
		#region ================== Constructor / Show

		// Constructor
		public ErrorCheckForm()
		{
			// Initialize
			InitializeComponent();
			
			// Find all error checkers
			Type[] checkertypes = BuilderPlug.Me.FindClasses(typeof(ErrorChecker));
			foreach(Type t in checkertypes)
			{
				object[] attr = t.GetCustomAttributes(typeof(ErrorCheckerAttribute), true);
				if(attr.Length > 0)
				{
					ErrorCheckerAttribute checkerattr = (attr[0] as ErrorCheckerAttribute);
					
					// Add the type to the checkbox list
					CheckBox c = checks.Add(checkerattr.DisplayName, t);
					c.Checked = checkerattr.DefaultChecked;
				}
			}
			checks.Sort(); //mxd

			//mxd. Store initial height
			initialsize = this.Size;
			resultslist = new List<ErrorResult>();
			hiddentresulttypes = new List<Type>();
		}
		
		// This shows the window
		public void Show(Form owner)
		{
			// Move controls according to the height of the checkers box
			checks.PerformLayout();
			buttoncheck.Top = checks.Bottom + 14;
			exportresults.Top = checks.Bottom + 14; //mxd
			resultspanel.Top = buttoncheck.Bottom + 14;
			cbApplyToAll.Checked = applyToAll; //mxd
			this.Text = "Map Analysis"; //mxd

			// Position at left-top of owner
			this.Location = new Point(owner.Location.X + 20, owner.Location.Y + 90);
			
			// Close results part
			resultspanel.Visible = false;
			this.Size = new Size(initialsize.Width, this.Height - this.ClientSize.Height + resultspanel.Top);
			this.MinimumSize = this.Size; //mxd
			this.MaximumSize = this.Size; //mxd

			//mxd. Restore export mode
			exportresults.CurrentMenuStripItem = exportmode;
			exportresults.Enabled = false;
			
			// Show window
			base.Show(owner);
		}
		
		#endregion
		
		#region ================== Thread Calls

		public void SubmitResult(ErrorResult result)
		{
			if(results.InvokeRequired)
			{
				CallResultMethodDelegate d = SubmitResult;
				try { progress.Invoke(d, result); }
				catch(ThreadInterruptedException) { }
			}
			else
			{
				if (!result.IsHidden && !hiddentresulttypes.Contains(result.GetType())) //mxd
				{
					results.Items.Add(result);
				}
				resultslist.Add(result); //mxd
				UpdateTitle();
			}
		}

		private void SetProgressMaximum(int maximum)
		{
			if(progress.InvokeRequired)
			{
				CallIntMethodDelegate d = SetProgressMaximum;
				try { progress.Invoke(d, maximum); }
				catch(ThreadInterruptedException) { }
			}
			else
			{
				progress.Maximum = maximum;
			}
		}

		public void AddProgressValue(int value)
		{
			if(progress.InvokeRequired)
			{
				CallIntMethodDelegate d = AddProgressValue;
				try { progress.Invoke(d, value); }
				catch(ThreadInterruptedException) { }
			}
			else
			{
				progress.Value += value;
			}
		}

		//mxd
		private void UpdateTitle()
		{
			int hiddencount = resultslist.Count - results.Items.Count;
			this.Text = "Map Analysis [" + resultslist.Count + " results"
				+ (hiddencount > 0 ? ", " + hiddencount + " are hidden]" : "]"); //mxd
		}
		
		// This stops checking (only called from the checking management thread)
		private void StopChecking()
		{
			if(this.InvokeRequired)
			{
				CallVoidMethodDeletage d = StopChecking;
				this.Invoke(d);
			}
			else
			{
				checksthread = null;
				progress.Value = 0;
				buttoncheck.Text = "Start Analysis";
				Cursor.Current = Cursors.Default;
				running = false;
				blockmap.Dispose();
				blockmap = null;
				UpdateTitle(); //mxd
				
				// When no results found, show "no results" and disable the list
				if(resultslist.Count == 0) 
				{
					results.Items.Add(new ResultNoErrors());
					results.Enabled = false;
					exportresults.Enabled = false; //mxd
				} 
				else 
				{ 
					exportresults.Enabled = true; //mxd
					ClearSelectedResult(); //mxd
				}
			}
		}
		
		// This starts checking
		private void StartChecking()
		{
			if(running) return;
			
			Cursor.Current = Cursors.WaitCursor;

			exportresults.Enabled = false; //mxd
			
			// Make blockmap
			RectangleF area = MapSet.CreateArea(General.Map.Map.Vertices);
			area = MapSet.IncreaseArea(area, General.Map.Map.Things);
			blockmap = new BlockMap<BlockEntry>(area);
			blockmap.AddLinedefsSet(General.Map.Map.Linedefs);
			blockmap.AddSectorsSet(General.Map.Map.Sectors);
			blockmap.AddThingsSet(General.Map.Map.Things);
			blockmap.AddVerticesSet(General.Map.Map.Vertices); //mxd
			
			//mxd. Open the results panel
			if (!resultspanel.Visible) 
			{
				this.MinimumSize = new Size();
				this.MaximumSize = new Size();
				this.Size = initialsize;
				resultspanel.Size = new Size(resultspanel.Width, this.ClientSize.Height - resultspanel.Top);
				resultspanel.Visible = true;
			}
			progress.Value = 0;
			results.Items.Clear();
			results.Enabled = true;
			resultslist = new List<ErrorResult>(); //mxd
			ClearSelectedResult();
			buttoncheck.Text = "Abort Analysis";
			General.Interface.RedrawDisplay();
			
			// Start checking
			running = true;
			checksthread = new Thread(RunChecks);
			checksthread.Name = "Error Checking Management";
			checksthread.Priority = ThreadPriority.Normal;
			checksthread.Start();
			
			Cursor.Current = Cursors.Default;
		}

		#endregion

		#region ================== Methods
		
		// This stops the checking
		public void CloseWindow()
		{
			// Currently running?
			if(running)
			{
				Cursor.Current = Cursors.WaitCursor;
				checksthread.Interrupt();
			}

			ClearSelectedResult();
			
			//mxd. Clear results 
			resultslist.Clear();
			results.Items.Clear();

			this.Hide();
		}

		// This clears the selected result
		private void ClearSelectedResult()
		{
			results.SelectedIndex = -1;
			if (results.Items.Count == 0 && resultslist.Count > 0) //mxd
				resultinfo.Text = "All results are hidden. Use context menu to show them.";
			else 
				resultinfo.Text = "Select a result from the list to see more information.\r\nRight-click on a result to show context menu.";
			resultinfo.Enabled = false;
			fix1.Visible = false;
			fix2.Visible = false;
			fix3.Visible = false;
			cbApplyToAll.Visible = false; //mxd
		}
		
		// This runs in a seperate thread to manage the checking threads
		private void RunChecks()
		{
			List<ErrorChecker> checkers = new List<ErrorChecker>();
			List<Thread> threads = new List<Thread>();
			int maxthreads = Environment.ProcessorCount;
			int totalprogress = 0;
			int nextchecker = 0;
			
			// Initiate all checkers
			foreach(CheckBox c in checks.Checkboxes)
			{
				// Include this one?
				if(c.Checked)
				{
					Type t = (c.Tag as Type);
					ErrorChecker checker;
					
					try
					{
						// Create instance
						checker = (ErrorChecker)Assembly.GetExecutingAssembly().CreateInstance(t.FullName, false, BindingFlags.Default, null, null, CultureInfo.CurrentCulture, new object[0]);
					}
					catch(TargetInvocationException ex)
					{
						// Error!
						General.ErrorLogger.Add(ErrorType.Error, "Failed to create class instance '" + t.Name + "'");
						General.WriteLogLine(ex.InnerException.GetType().Name + ": " + ex.InnerException.Message);
						throw ex;
					}
					catch(Exception ex)
					{
						// Error!
						General.ErrorLogger.Add(ErrorType.Error, "Failed to create class instance '" + t.Name + "'");
						General.WriteLogLine(ex.GetType().Name + ": " + ex.Message);
						throw ex;
					}
					
					// Add to list
					if(checker != null)
					{
						checkers.Add(checker);
						totalprogress += checker.TotalProgress;
					}
				}
			}
			
			// Sort the checkers with highest cost first
			// See CompareTo method in ErrorChecker for sorting comparison
			checkers.Sort();
			
			// Setup
			SetProgressMaximum(totalprogress);
			
			// Continue while threads are running or checks are to be done
			while((nextchecker < checkers.Count) || (threads.Count > 0))
			{
				// Start new thread when less than maximum number of
				// threads running and there is more work to be done
				while((threads.Count < maxthreads) && (nextchecker < checkers.Count))
				{
					ErrorChecker c = checkers[nextchecker++];
					Thread t = new Thread(c.Run);
					t.Name = "Error Checker '" + c.GetType().Name + "'";
					t.Priority = ThreadPriority.BelowNormal;
					t.Start();
					threads.Add(t);
				}
				
				// Remove threads that are done
				for(int i = threads.Count - 1; i >= 0; i--)
					if(!threads[i].IsAlive) threads.RemoveAt(i);
				
				// Handle thread interruption
				try { Thread.Sleep(1); }
				catch(ThreadInterruptedException) { break; }
			}
			
			// Stop all running threads
			foreach(Thread t in threads)
			{
				while(t.IsAlive)
				{
					try
					{ 
						t.Interrupt();
						t.Join(1);
					}
					catch(ThreadInterruptedException)
					{
						// We have to continue, we can't just leave the other threads running!
					}
				}
			}
			
			// Dispose all checkers
			checkers = null;
			
			// Done
			StopChecking();
		}
		
		#endregion
		
		#region ================== Events
		
		// Window closing
		private void ErrorCheckForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			//mxd. Store persistent settings
			exportmode = exportresults.CurrentMenuStripItem;
			applyToAll = cbApplyToAll.Checked;

			//mxd. Clear results 
			resultslist.Clear();
			results.Items.Clear();
			
			// If the user closes the form, then just cancel the mode
			if(e.CloseReason == CloseReason.UserClosing)
			{
				e.Cancel = true;
				General.Interface.Focus();
				General.Editing.CancelMode();
			}
		}
		
		// Start/stop
		private void buttoncheck_Click(object sender, EventArgs e)
		{
			// Currently running?
			if(running)
			{
				Cursor.Current = Cursors.WaitCursor;
				checksthread.Interrupt();
			}
			else
			{
				StartChecking();
			}
		}

		// Close
		private void closebutton_Click(object sender, EventArgs e)
		{
			General.Interface.Focus();
			General.Editing.CancelMode();
		}
		
		// Results selection changed
		private void results_SelectedIndexChanged(object sender, EventArgs e)
		{
			// Anything selected?
			if(results.SelectedIndex >= 0)
			{
				ErrorResult r = (results.SelectedItem as ErrorResult);
				resultinfo.Text = r.Description;
				resultinfo.Enabled = true;
				fix1.Text = r.Button1Text;
				fix2.Text = r.Button2Text;
				fix3.Text = r.Button3Text;
				fix1.Visible = (r.Buttons > 0);
				fix2.Visible = (r.Buttons > 1);
				fix3.Visible = (r.Buttons > 2);
				cbApplyToAll.Visible = (r.Buttons > 0); //mxd
				r.ZoomToObject();
			}
			else
			{
				ClearSelectedResult();
			}
			
			General.Interface.RedrawDisplay();
		}

		//mxd
		private void results_MouseDown(object sender, MouseEventArgs e) 
		{
			int index = results.IndexFromPoint(e.Location);
			if(index != results.SelectedIndex) results.SelectedIndex = index;
		}
		
		// First button
		private void fix1_Click(object sender, EventArgs e)
		{
			// Anything selected?
			if(results.SelectedIndex >= 0)
			{
				if(running)
				{
					General.ShowWarningMessage("You must stop the analysis before you can make changes to your map!", MessageBoxButtons.OK);
				}
				else
				{
					ErrorResult r = (results.SelectedItem as ErrorResult);
					if (r.Button1Click(false)) 
					{
						if (cbApplyToAll.Checked) FixSimilarErrors(r.GetType(), 1); //mxd
						StartChecking();
					} 
					else 
					{
						General.Interface.RedrawDisplay();
					}
				}
			}
		}
		
		// Second button
		private void fix2_Click(object sender, EventArgs e)
		{
			// Anything selected?
			if(results.SelectedIndex >= 0)
			{
				if(running)
				{
					General.ShowWarningMessage("You must stop the analysis before you can make changes to your map!", MessageBoxButtons.OK);
				}
				else
				{
					ErrorResult r = (results.SelectedItem as ErrorResult);
					if (r.Button2Click(false)) 
					{
						if (cbApplyToAll.Checked) FixSimilarErrors(r.GetType(), 2); //mxd
						StartChecking();
					} 
					else 
					{
						General.Interface.RedrawDisplay();
					}
				}
			}
		}
		
		// Third button
		private void fix3_Click(object sender, EventArgs e)
		{
			// Anything selected?
			if(results.SelectedIndex >= 0)
			{
				if(running)
				{
					General.ShowWarningMessage("You must stop the analysis before you can make changes to your map!", MessageBoxButtons.OK);
				}
				else
				{
					ErrorResult r = (results.SelectedItem as ErrorResult);
					if (r.Button3Click(false)) 
					{
						if (cbApplyToAll.Checked) FixSimilarErrors(r.GetType(), 3); //mxd
						StartChecking();
					} 
					else 
					{
						General.Interface.RedrawDisplay();
					}
				}
			}
		}

		//mxd
		private void FixSimilarErrors(Type type, int fixIndex) 
		{
			foreach (Object item in results.Items) 
			{
				if (item == results.SelectedItem) continue;
				if (item.GetType() != type) continue;

				ErrorResult r = item as ErrorResult;

				if (fixIndex == 1 && !r.Button1Click(true)) break;
				if (fixIndex == 2 && !r.Button2Click(true)) break;
				if (fixIndex == 3 && !r.Button3Click(true)) break;
			}
		}

		//mxd
		private void exporttofile_Click(object sender, EventArgs e) 
		{
			// Show save dialog
			string path = Path.GetDirectoryName(General.Map.FilePathName);
			saveFileDialog.InitialDirectory = path;
			saveFileDialog.FileName = Path.GetFileNameWithoutExtension(General.Map.FileTitle) + "_errors.txt";
			if(saveFileDialog.ShowDialog(this) == DialogResult.Cancel) return;
			
			// Get results
			StringBuilder sb = new StringBuilder();
			foreach(ErrorResult result in results.Items) sb.AppendLine(result.ToString());

			// Save to file
			try 
			{
				using(StreamWriter sw = File.CreateText(saveFileDialog.FileName)) sw.Write(sb.ToString());
				General.Interface.DisplayStatus(StatusType.Info, "Analysis results saved to '" + path + "'");
			} 
			catch(Exception) 
			{
				General.Interface.DisplayStatus(StatusType.Info, "Failed to save analysis results... Make sure map path is not write-protected.");
			}
		}

		//mxd
		private void copytoclipboard_Click(object sender, EventArgs e) 
		{
			// Get results
			StringBuilder sb = new StringBuilder();
			foreach(ErrorResult result in results.Items) sb.AppendLine(result.ToString());

			// Set on clipboard
			Clipboard.SetText(sb.ToString());

			General.Interface.DisplayStatus(StatusType.Info, "Analysis results copied to clipboard.");
		}

		//mxd
		private void toggleall_CheckedChanged(object sender, EventArgs e) 
		{
			foreach(CheckBox cb in checks.Checkboxes) cb.Checked = toggleall.Checked;
		}

		private void ErrorCheckForm_HelpRequested(object sender, HelpEventArgs hlpevent)
		{
			General.ShowHelp("e_mapanalysis.html");
		}
		
		#endregion

		#region ================== Results Context Menu (mxd)

		private void resultcontextmenustrip_Opening(object sender, System.ComponentModel.CancelEventArgs e) 
		{
			//disable or enable stuff
			bool haveresult = resultslist.Count > 0 && results.SelectedIndex > -1;
			resultshowall.Enabled = (resultslist.Count > 0 && resultslist.Count > results.Items.Count);
			resultcopytoclipboard.Enabled = haveresult;
			resulthidecurrent.Enabled = haveresult;
			resulthidecurrenttype.Enabled = haveresult;
			resultshowonlycurrent.Enabled = haveresult;
		}

		private void resultshowall_Click(object sender, EventArgs e) 
		{
			// Reset ignored items
			foreach(ErrorResult result in resultslist) result.Hide(false);
			
			// Restore items
			results.Items.Clear();
			results.Items.AddRange(resultslist.ToArray());
			hiddentresulttypes.Clear();

			// Do the obvious
			UpdateTitle();
			ClearSelectedResult();
		}

		private void resulthidecurrent_Click(object sender, EventArgs e) 
		{
			ErrorResult r = results.SelectedItem as ErrorResult;
			if(r == null) return;
			r.Hide(true);
			results.Items.Remove(r);

			// Do the obvious
			UpdateTitle();
			ClearSelectedResult();
		}

		private void resulthidecurrenttype_Click(object sender, EventArgs e)
		{
			ErrorResult r = results.SelectedItem as ErrorResult;
			if(r == null) return;
			Type resulttype = r.GetType();
			List<ErrorResult> filtered = new List<ErrorResult>();
			hiddentresulttypes.Add(resulttype);

			// Apply filtering
			foreach(ErrorResult result in results.Items)
			{
				if (result.GetType() != resulttype) filtered.Add(result);
			}

			// Replace items
			results.Items.Clear();
			results.Items.AddRange(filtered.ToArray());

			// Do the obvious
			UpdateTitle();
			ClearSelectedResult();
		}

		private void resultshowonlycurrent_Click(object sender, EventArgs e) 
		{
			ErrorResult r = results.SelectedItem as ErrorResult;
			if(r == null) return;
			Type resulttype = r.GetType();
			List<ErrorResult> filtered = new List<ErrorResult>();
			hiddentresulttypes.Clear();

			// Apply filtering
			foreach(ErrorResult result in results.Items)
			{
				Type curresulttype = result.GetType();
				if (curresulttype != resulttype)
				{
					hiddentresulttypes.Add(curresulttype);
				}
				else
				{
					filtered.Add(result);
				}
			}

			// Replace items
			results.Items.Clear();
			results.Items.AddRange(filtered.ToArray());

			// Do the obvious
			UpdateTitle();
			ClearSelectedResult();
		}

		private void resultcopytoclipboard_Click(object sender, EventArgs e) 
		{
			ErrorResult r = results.SelectedItem as ErrorResult;
			if(r == null) return;
			Clipboard.SetText(r.ToString());
			General.Interface.DisplayStatus(StatusType.Info, "Analysis result copied to clipboard.");
		}

		private void results_KeyUp(object sender, KeyEventArgs e) 
		{
			if (e.Control && e.KeyCode == Keys.C)
			{
				resultcopytoclipboard_Click(sender, EventArgs.Empty);
			}
		}

		#endregion

	}
}