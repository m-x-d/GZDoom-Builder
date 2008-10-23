
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
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;
using CodeImp.DoomBuilder.Actions;
using CodeImp.DoomBuilder.Data;
using CodeImp.DoomBuilder.Config;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Controls;
using CodeImp.DoomBuilder.Windows;
using System.Reflection;
using System.Globalization;
using System.Threading;

#endregion

namespace CodeImp.DoomBuilder.BuilderModes
{
	public partial class ErrorCheckForm : DelayedForm
	{
		#region ================== Constants

		// Constants
		private const int RESULTS_WINDOW_HEIGHT = 536;

		#endregion

		#region ================== Variables
		
		private bool running = false;
		private Thread checksthread;
		
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
		}
		
		// This shows the window
		public void Show(Form owner)
		{
			// First time showing?
			//if((this.Location.X == 0) && (this.Location.Y == 0))
			{
				// Position at left-top of owner
				this.Location = new Point(owner.Location.X + 20, owner.Location.Y + 90);
			}
			
			// Close results part
			resultspanel.Visible = false;
			this.Size = new Size(this.Width, this.Height - this.ClientSize.Height + resultspanel.Top);
			
			// Show window
			base.Show(owner);
		}
		
		#endregion
		
		#region ================== Methods
		
		// This runs in a seperate thread to manage the checking threads
		private void RunChecks()
		{
			List<ErrorChecker> checkers = new List<ErrorChecker>();
			int totalprogress = 0;
			int nextchecker = 0;
			
			// Initiate all checkers
			foreach(CheckBox c in checks.Checkboxes)
			{
				// Include this one?
				if(c.Checked)
				{
					Type t = (c.Tag as Type);
					ErrorChecker checker = null;
					
					try
					{
						// Create instance
						checker = (ErrorChecker)Assembly.GetExecutingAssembly().CreateInstance(t.FullName, false, BindingFlags.Default, null, null, CultureInfo.CurrentCulture, new object[0]);
					}
					catch(TargetInvocationException ex)
					{
						// Error!
						General.WriteLogLine("ERROR: Failed to create class instance '" + t.Name + "'!");
						General.WriteLogLine(ex.InnerException.GetType().Name + ": " + ex.InnerException.Message);
						throw ex;
					}
					catch(Exception ex)
					{
						// Error!
						General.WriteLogLine("ERROR: Failed to create class instance '" + t.Name + "'!");
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
			
			// Setup
			progress.Maximum = totalprogress;
			const int maxthreads = Environment.ProcessorCount;
			List<Thread> threads = new List<Thread>(maxthreads);
			
			// Continue while threads are running or checks are to be done
			while((nextchecker < checkers.Count) || (threads.Count > 0))
			{
				// Start new thread when less than maximum number of
				// threads running and there is more work to be done
				while((threads.Count < maxthreads) && (checkers.Count > 0))
				{
					ErrorChecker c = checkers[nextchecker++];
					Thread t = new Thread(new ThreadStart(c.Run));
					t.Name = "Error Checker '" + c.GetType().Name + "'";
					t.Priority = ThreadPriority.BelowNormal;
					t.Start();
					threads.Add(t);
				}
				
				// Remove threads that are done
				for(int i = threads.Count; i >= 0; i--)
					if(!threads[i].IsAlive) threads.RemoveAt(i);
				
				// Handle thread interruption
				try { Thread.Sleep(1); }
				catch(ThreadInterruptedException) { break; }
			}
			
			// Stop all running threads
			foreach(Thread t in threads)
			{
				t.Interrupt();
				t.Join();
			}
			
			// Dispose all checkers
			checkers = null;
			
			// Done
			progress.Value = 0;
		}
		
		#endregion
		
		#region ================== Events
		
		// Window closing
		private void ErrorCheckForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			// If the user closes the form, then just cancel the mode
			if(e.CloseReason == CloseReason.UserClosing)
			{
				e.Cancel = true;
				General.Map.CancelMode();
			}
		}
		
		// Start/stop
		private void buttoncheck_Click(object sender, EventArgs e)
		{
			Cursor.Current = Cursors.WaitCursor;
			
			// Currently running?
			if(running)
			{
				// Stop checking
				checksthread.Interrupt();
				checksthread.Join();
				checksthread = null;
				running = false;
			}
			else
			{
				// Open the results panel
				this.Size = new Size(this.Width, this.Height - this.ClientSize.Height + resultspanel.Top + resultspanel.Height);
				progress.Value = 0;
				results.Items.Clear();
				resultspanel.Visible = true;
				
				// Start checking
				running = true;
				checksthread = new Thread(new ThreadStart(RunChecks));
				checksthread.Name = "Checking Management";
				checksthread.Priority = ThreadPriority.Normal;
				checksthread.Start();
			}
			
			Cursor.Current = Cursors.Default;
		}
		
		#endregion
	}
}