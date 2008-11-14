
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
using System.Diagnostics;
using CodeImp.DoomBuilder.Data;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Config;
using CodeImp.DoomBuilder.Types;
using CodeImp.DoomBuilder.IO;
using System.IO;
using CodeImp.DoomBuilder.Compilers;

#endregion

namespace CodeImp.DoomBuilder.Controls
{
	internal sealed class ScriptFileDocumentTab : ScriptDocumentTab
	{
		#region ================== Constants

		#endregion
		
		#region ================== Variables
		
		private string filepathname;
		
		#endregion
		
		#region ================== Properties
		
		public override bool IsSaveAsRequired { get { return (filepathname.Length == 0); } }
		public override string Filename { get { return filepathname; } }
		
		#endregion
		
		#region ================== Constructor / Disposer
		
		// Constructor
		public ScriptFileDocumentTab(ScriptConfiguration config)
		{
			string ext = "";
			
			// Initialize
			this.filepathname = "";
			this.config = config;
			editor.SetupStyles(config);
			if(config.Extensions.Length > 0) ext = "." + config.Extensions[0];
			SetTitle("Untitled" + ext);
			editor.ClearUndoRedo();
		}
		
		// Disposer
		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
		}
		
		#endregion
		
		#region ================== Methods
		
		// This compiles the script file
		public override void Compile()
		{
			DirectoryInfo tempdir;
			Compiler compiler;
			string inputfile, outputfile;
			
			// List of errors
			List<CompilerError> errors = new List<CompilerError>();
			
			try
			{
				// Initialize compiler
				compiler = config.Compiler.Create();
			}
			catch(Exception e)
			{
				// Fail
				errors.Add(new CompilerError("Unable to initialize compiler. " + e.GetType().Name + ": " + e.Message));
				return;
			}
			
			// Make random output filename
			outputfile = General.MakeTempFilename(compiler.Location, "tmp");
				
			// Run compiler
			compiler.Parameters = config.Parameters;
			compiler.InputFile = Path.GetFileName(filepathname);
			compiler.OutputFile = Path.GetFileName(outputfile);
			compiler.WorkingDirectory = Path.GetDirectoryName(filepathname);
			if(compiler.Run())
			{
				// Fetch errors
				errors.AddRange(compiler.Errors);
			}
			
			// Dispose compiler
			compiler.Dispose();
			
			// TODO: Feed errors to panel
			
		}
		
		// This saves the document (used for both explicit and implicit)
		// Return true when successfully saved
		public override bool Save()
		{
			try
			{
				// Write the file
				File.WriteAllText(filepathname, editor.Text);
			}
			catch(Exception e)
			{
				// Failed
				General.WriteLogLine("ERROR: Cannot open file '" + filepathname + "' for writing.");
				General.WriteLogLine(e.GetType().Name + ": " + e.Message);
				General.ShowErrorMessage("Unable to open file \"" + filepathname + "\" for writing. Make sure the path exists and that the file is not in use by another application.", MessageBoxButtons.OK);
				return false;
			}
			
			// Done
			editor.ClearUndoRedo();
			return true;
		}
		
		// This saves the document to a new file
		// Return true when successfully saved
		public override bool SaveAs(string filename)
		{
			string oldfilename = filepathname;
			filepathname = filename;
			if(this.Save())
			{
				SetTitle(Path.GetFileName(filepathname));
				return true;
			}
			else
			{
				this.filepathname = oldfilename;
				return false;
			}
		}
		
		// This opens a file and returns true when successful
		public bool Open(string filepathname)
		{
			try
			{
				// Read the file
				editor.Text = File.ReadAllText(filepathname);
			}
			catch(Exception e)
			{
				// Failed
				General.WriteLogLine("ERROR: Cannot open file '" + filepathname + "' for reading.");
				General.WriteLogLine(e.GetType().Name + ": " + e.Message);
				General.ShowErrorMessage("Unable to open file \"" + filepathname + "\" for reading. Make sure the path exists and that the file is not in use by another application.", MessageBoxButtons.OK);
				return false;
			}
			
			// Setup
			this.filepathname = filepathname;
			SetTitle(Path.GetFileName(filepathname));
			editor.ClearUndoRedo();
			return true;
		}

		// This changes the script configurations
		public override void ChangeScriptConfig(ScriptConfiguration newconfig)
		{
			string ext = "";

			this.config = newconfig;
			editor.SetupStyles(config);

			if(filepathname.Length == 0)
			{
				if(config.Extensions.Length > 0) ext = "." + config.Extensions[0];
				SetTitle("Untitled" + ext);
			}
		}
		
		#endregion
		
		#region ================== Events
		
		#endregion
	}
}
