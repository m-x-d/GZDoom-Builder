
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
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.IO;
using CodeImp.DoomBuilder.Config;
using CodeImp.DoomBuilder.IO;
using System.Windows.Forms;
using System.Text.RegularExpressions;

#endregion

namespace CodeImp.DoomBuilder.Compilers
{
	internal sealed class BlamCompiler : Compiler
	{
		#region ================== Constants
		
		private const string BLAM_ERROR_FILE = "blam_err.txt";
		
		#endregion
		
		#region ================== Variables
		
		#endregion
		
		#region ================== Constructor
		
		// Constructor
		public BlamCompiler(CompilerInfo info) : base(info)
		{
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
		
		// This runs the compiler
		public override bool Run()
		{
			ProcessStartInfo processinfo;
			Process process;
			TimeSpan deltatime;
			int line = 0;
			string sourcedir = Path.GetDirectoryName(sourcefile);
			
			// Create parameters
			string args = this.parameters;
			args = args.Replace("%FI", inputfile);
			args = args.Replace("%FO", outputfile);
			args = args.Replace("%FS", sourcefile);
			args = args.Replace("%PT", this.tempdir.FullName);
			args = args.Replace("%PS", sourcedir);
			
			// Setup process info
			processinfo = new ProcessStartInfo();
			processinfo.Arguments = args;
			processinfo.FileName = Path.Combine(this.tempdir.FullName, info.ProgramFile);
			processinfo.CreateNoWindow = false;
			processinfo.ErrorDialog = false;
			processinfo.UseShellExecute = true;
			processinfo.WindowStyle = ProcessWindowStyle.Hidden;
			processinfo.WorkingDirectory = this.workingdir;
			
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
			
			// Wait for compiler to complete
			process.WaitForExit();
			deltatime = TimeSpan.FromTicks(process.ExitTime.Ticks - process.StartTime.Ticks);
			General.WriteLogLine("Compiler process has finished.");
			General.WriteLogLine("Compile time: " + deltatime.TotalSeconds.ToString("########0.00") + " seconds");
			
			// Now find the error file
			string errfile = Path.Combine(this.workingdir, BLAM_ERROR_FILE);
			if(File.Exists(errfile))
			{
				try
				{
					// Read all lines
					string[] errlines = File.ReadAllLines(errfile);
					while(line < errlines.Length)
					{
						// Check line
						string linestr = errlines[line];
                        CompilerError err = new CompilerError();

                        err.filename = inputfile;
                        err.description = linestr;
                        ReportError(err);
						
						// Next line
						line++;
					}
				}
				catch(Exception e)
				{
					// Error reading errors (ironic, isn't it)
					ReportError(new CompilerError("Failed to retrieve compiler error report. " + e.GetType().Name + ": " + e.Message));
				}
			}
			
			return true;
		}
		
		#endregion
	}
}

