
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
using System.Diagnostics;
using System.IO;
using CodeImp.DoomBuilder.Config;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using CodeImp.DoomBuilder.Data;

#endregion

namespace CodeImp.DoomBuilder.Compilers
{
	internal sealed class AccCompiler : Compiler
	{
		#region ================== Constants
		
		private const string ACS_ERROR_FILE = "acs.err";
		
		#endregion
		
		#region ================== Variables
		
		#endregion
		
		#region ================== Constructor
		
		// Constructor
		public AccCompiler(CompilerInfo info) : base(info)
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
			Process process;
			TimeSpan deltatime;
			int line = 0;
			string sourcedir = Path.GetDirectoryName(sourcefile);

			//xabis
			// Copy includes from the resources into the compiler's folder, preserving relative pathing and naming
			if(CopyIncludesToWorkingDirectory) //mxd
			{
				foreach(string include in includes)
				{
					// Grab the script text from the resources
					MemoryStream s = General.Map.Data.LoadFile(include);

					if(s != null)
					{
						// Pull the pk3 or directory sub folder out if applicable
						FileInfo fi = new FileInfo(Path.Combine(this.tempdir.FullName, include));

						// Do not allow files to be overwritten, either accidentally or maliciously
						if(!fi.Exists)
						{
							General.WriteLogLine("Copying script include: " + include);

							// Create the directory path as needed
							if(!string.IsNullOrEmpty(fi.DirectoryName)) Directory.CreateDirectory(fi.DirectoryName);

							// Dump the script into the target file
							BinaryReader reader = new BinaryReader(s);
							File.WriteAllBytes(fi.FullName, reader.ReadBytes((int)s.Length));
						}
					}
				}
			}

			// Create parameters
			string args = this.parameters;
			args = args.Replace("%FI", inputfile);
			args = args.Replace("%FO", outputfile);
			args = args.Replace("%FS", sourcefile);
			args = args.Replace("%PT", this.tempdir.FullName);
			args = args.Replace("%PS", sourcedir);
			args = args.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar); //mxd. This fixes include path when the map is in a root directory
			
			// Setup process info
			ProcessStartInfo processinfo = new ProcessStartInfo();
			processinfo.Arguments = args;
			//processinfo.FileName = Path.Combine(this.tempdir.FullName, info.ProgramFile);
			processinfo.FileName = Path.Combine(info.Path, info.ProgramFile); //mxd
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
			string errfile = Path.Combine(this.workingdir, ACS_ERROR_FILE);
			if(File.Exists(errfile))
			{
				try
				{
					// Regex to find error lines
					Regex errlinematcher = new Regex(":[0-9]+: ", RegexOptions.Compiled | RegexOptions.CultureInvariant);
					
					// Read all lines
					bool erroradded = false; //mxd
					string[] errlines = File.ReadAllLines(errfile);
					while(line < errlines.Length)
					{
						// Check line
						string linestr = errlines[line];
						Match match = errlinematcher.Match(linestr);
						if(match.Success && (match.Index > 0))
						{
							CompilerError err = new CompilerError();
							
							// The match without spaces and semicolon is the line number
							string linenr = match.Value.Replace(":", "").Trim();
							if(!int.TryParse(linenr, out err.linenumber))
								err.linenumber = CompilerError.NO_LINE_NUMBER;
							else
								err.linenumber--;
							
							// Everything before the match is the filename
							err.filename = linestr.Substring(0, match.Index).Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
							if(!Path.IsPathRooted(err.filename))
							{
								//mxd. If the error is in an include file, try to find it in loaded resources
								if(includes.Contains(err.filename.ToLowerInvariant()))
								{
									foreach(DataReader dr in General.Map.Data.Containers)
									{
										if(dr is DirectoryReader && dr.FileExists(err.filename))
										{
											err.filename = Path.Combine(dr.Location.location, err.filename);
											break;
										}
									}
								}
								else
								{
									// Add working directory to filename, so it could be recognized as map namespace lump in MapManager.CompileLump()
									err.filename = Path.Combine(processinfo.WorkingDirectory, err.filename);
								}
							}
							
							// Everything after the match is the description
							err.description = linestr.Substring(match.Index + match.Length).Trim();
							
							// Report the error
							ReportError(err);
							erroradded = true; //mxd
						}
						
						// Next line
						line++;
					}

					//mxd. Some ACC errors are not properly formatted. If that's the case, threat the whole acs.err as an error...
					if(!erroradded && errlines.Length > 0)
					{
						ReportError(new CompilerError(string.Join(Environment.NewLine, errlines)));
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

