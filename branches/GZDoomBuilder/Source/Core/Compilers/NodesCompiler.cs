
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
using System.Diagnostics;
using System.IO;
using CodeImp.DoomBuilder.Config;
using System.Windows.Forms;

#endregion

namespace CodeImp.DoomBuilder.Compilers
{
	internal sealed class NodesCompiler : Compiler
	{
		#region ================== Constants

		#endregion

		#region ================== Variables

		#endregion

		#region ================== Properties

		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		public NodesCompiler(CompilerInfo info) : base(info)
		{
			// Initialize

			// We have no destructor
			GC.SuppressFinalize(this);
		}

		// Disposer
		public override void Dispose()
		{
			// Not already disposed?
			if(!isdisposed)
			{
				// Clean up

				// Done
				base.Dispose();
			}
		}

		#endregion

		#region ================== Methods

		// This runs the compiler with a file as input.
		public override bool Run()
		{
			ProcessStartInfo processinfo;
			Process process;
			TimeSpan deltatime;
			
			// Create parameters
			string args = this.parameters;
			args = args.Replace("%FI", inputfile);
			args = args.Replace("%FO", outputfile);
			
			// Setup process info
			processinfo = new ProcessStartInfo();
			processinfo.Arguments = args;
			processinfo.FileName = Path.Combine(this.tempdir.FullName, info.ProgramFile);
			processinfo.CreateNoWindow = true; //mxd. was false
			processinfo.ErrorDialog = false;
			processinfo.UseShellExecute = false; //mxd. was true
			processinfo.WindowStyle = ProcessWindowStyle.Hidden;
			processinfo.WorkingDirectory = this.workingdir;

			//mxd
			processinfo.RedirectStandardError = true;
			processinfo.RedirectStandardOutput = true;
			
			// Output info
			General.WriteLogLine("Running compiler...");
			General.WriteLogLine("Program:    " + processinfo.FileName);
			General.WriteLogLine("Arguments:  " + processinfo.Arguments);
			
			try
			{
				// Start the compiler
				process = Process.Start(processinfo);
			}
			catch(Exception e)
			{
				// Unable to start the compiler
				General.ShowErrorMessage("Unable to start the compiler (" + info.Name + "). " + e.GetType().Name + ": " + e.Message, MessageBoxButtons.OK);
				return false;
			}

			//mxd
			string outErr = process.StandardError.ReadToEnd();
			string outMsg = process.StandardOutput.ReadToEnd();
			
			// Wait for compiler to complete
			process.WaitForExit();

			//mxd
			bool errorsInNormalOurput = (outMsg.Length > 0 && outMsg.ToLowerInvariant().IndexOf("error") != -1);
			//zdbsp actually writes building process here, not error info
			bool errorsInErrorOutput = (outErr.Length > 0 && outErr.ToLowerInvariant().IndexOf("error") != -1);

			deltatime = TimeSpan.FromTicks(process.ExitTime.Ticks - process.StartTime.Ticks);
			General.WriteLogLine("Compiler process has finished " + (errorsInNormalOurput || errorsInErrorOutput ? "with errors." : ".")); //mxd
			General.WriteLogLine("Compile time: " + deltatime.TotalSeconds.ToString("########0.00") + " seconds");

			//mxd
			if (process.ExitCode > 0 || errorsInNormalOurput || errorsInErrorOutput) {
				if (errorsInNormalOurput) {
					ReportError(new CompilerError(outMsg));
					General.WriteLogLine("Normal output: " + outMsg);
				}

				if (errorsInErrorOutput) {
					ReportError(new CompilerError(outErr));
					General.WriteLogLine("Error output: " + outErr);
				}
				return false;
			}

			return true;
		}
		
		#endregion
	}
}
