
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
using CodeImp.DoomBuilder.IO;
using CodeImp.DoomBuilder.Data;
using System.IO;
using System.Diagnostics;
using System.Windows.Forms;

#endregion

namespace CodeImp.DoomBuilder.Config
{
	internal class NodebuilderInfo : IComparable<NodebuilderInfo>
	{
		#region ================== Constants

		#endregion

		#region ================== Variables

		private string name;
		private string title;
		private CompilerInfo compiler;
		private string parameters;
		private bool specialoutputfile;
		
		#endregion

		#region ================== Properties

		public string Name { get { return name; } }
		public string Title { get { return title; } }
		public CompilerInfo Compiler { get { return compiler; } }
		public string Parameters { get { return parameters; } }
		public bool HasSpecialOutputFile { get { return specialoutputfile; } }

		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		public NodebuilderInfo(string filename, string name, Configuration cfg)
		{
			string compilername;
			
			General.WriteLogLine("Registered nodebuilder configuration '" + name + "' from '" + filename + "'");

			// Initialize
			this.name = name;
			this.compiler = null;
			this.title = cfg.ReadSetting("nodebuilders." + name + ".title", "<untitled configuration>");
			compilername = cfg.ReadSetting("nodebuilders." + name + ".compiler", "");
			this.parameters = cfg.ReadSetting("nodebuilders." + name + ".parameters", "");
			
			// Check for special output filename
			this.specialoutputfile = this.parameters.Contains("%T");
			
			// Find compiler
			foreach(CompilerInfo c in General.Compilers)
			{
				// Compiler name matches?
				if(c.Name == compilername)
				{
					// Apply compiler
					this.compiler = c;
					break;
				}
			}

			// No compiler found?
			if(this.compiler == null) throw new Exception("No such compiler defined: '" + compilername + "'");
		}
		
		#endregion

		#region ================== Methods

		// This compares it to other ConfigurationInfo objects
		public int CompareTo(NodebuilderInfo other)
		{
			// Compare
			return name.CompareTo(other.name);
		}

		// String representation
		public override string ToString()
		{
			return title;
		}
		
		// This runs the nodebuilder
		public bool Run(string targetpath, string inputfile, string outputfile)
		{
			ProcessStartInfo processinfo;
			Process process;
			TimeSpan deltatime;
			string args;

			try
			{
				// Copy required files
				General.WriteLogLine("Copying required files for compiler '" + compiler.Name + "'...");
				//compiler.CopyRequiredFiles(targetpath);
			}
			catch(Exception e)
			{
				// Unable to copy files
				General.ShowErrorMessage("Unable to copy the required files for the compiler (" + compiler.Name + "). " + e.GetType().Name + ": " + e.Message, MessageBoxButtons.OK);
				return false;
			}
			
			// Make arguments
			args = parameters;
			args = args.Replace("%F", inputfile);
			args = args.Replace("%H", outputfile);
			
			// Setup process info
			processinfo = new ProcessStartInfo();
			processinfo.Arguments = args;
			processinfo.FileName = Path.Combine(targetpath, compiler.ProgramFile);
			processinfo.CreateNoWindow = false;
			processinfo.ErrorDialog = false;
			processinfo.UseShellExecute = true;
			processinfo.WindowStyle = ProcessWindowStyle.Hidden;
			processinfo.WorkingDirectory = targetpath;
			
			// Output info
			General.WriteLogLine("Running nodebuilder compiler '" + compiler.Name + "' with configuration '" + name + "'...");
			General.WriteLogLine("Program:    " + processinfo.FileName);
			General.WriteLogLine("Arguments:  " + processinfo.Arguments);

			try
			{
				// Start the nodebuilder
				process = Process.Start(processinfo);
			}
			catch(Exception e)
			{
				// Unable to start the nodebuilder
				General.ShowErrorMessage("Unable to start the nodebuilder compiler (" + compiler.Name + ") . " + e.GetType().Name + ": " + e.Message, MessageBoxButtons.OK);
				return false;
			}
			
			// Wait for nodebuilder to complete
			process.WaitForExit();
			deltatime = TimeSpan.FromTicks(process.ExitTime.Ticks - process.StartTime.Ticks);
			General.WriteLogLine("Nodebuilder compiler has finished.");
			General.WriteLogLine("Compile time: " + deltatime.TotalSeconds.ToString("########0.00") + " seconds");
			
			// Success!
			return true;
		}
		
		#endregion
	}
}
