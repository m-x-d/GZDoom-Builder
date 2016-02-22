
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
using System.IO;
using System.Windows.Forms;
using CodeImp.DoomBuilder.Compilers;
using CodeImp.DoomBuilder.Config;
using CodeImp.DoomBuilder.Data;
using CodeImp.DoomBuilder.GZBuilder.GZDoom;

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
		public ScriptFileDocumentTab(ScriptEditorPanel panel, ScriptConfiguration config) : base(panel)
		{
			string ext = "";
			
			// Initialize
			this.filepathname = "";
			this.config = config;
			editor.SetupStyles(config);
			if(config.Extensions.Length > 0) ext = "." + config.Extensions[0];
			SetTitle("Untitled" + ext);
			editor.ClearUndoRedo();
			editor.FunctionBar.Enabled = (config.ScriptType != ScriptType.UNKNOWN); //mxd
		}
		
		#endregion
		
		#region ================== Methods
		
		// This compiles the script file
		public override void Compile()
		{
			//mxd. ACS requires special handling...
			if(config.ScriptType == ScriptType.ACS)
			{
				CompileACS();
				return;
			}

			Compiler compiler;
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
				panel.ShowErrors(errors); //mxd
				return;
			}
			
			// Copy the source file into the temporary directory
			string inputfile = Path.Combine(compiler.Location, Path.GetFileName(filepathname));
			File.Copy(filepathname, inputfile);
			
			// Make random output filename
			string outputfile = General.MakeTempFilename(compiler.Location, "tmp");

			// Run compiler
			compiler.Parameters = config.Parameters;
			compiler.InputFile = Path.GetFileName(inputfile);
			compiler.OutputFile = Path.GetFileName(outputfile);
			compiler.SourceFile = filepathname;
			compiler.WorkingDirectory = Path.GetDirectoryName(inputfile);
			if(compiler.Run())
			{
				// Fetch errors
				foreach(CompilerError e in compiler.Errors)
				{
					CompilerError newerr = e;

					// If the error's filename equals our temporary file,
					// replace it with the original source filename
					if(string.Compare(e.filename, inputfile, true) == 0)
						newerr.filename = filepathname;

					errors.Add(newerr);
				}
			}
			
			// Dispose compiler
			compiler.Dispose();

			//mxd. Update script navigator
			errors.AddRange(UpdateNavigator());
			
			// Feed errors to panel
			panel.ShowErrors(errors);
		}

		//mxd. ACS requires special handling...
		private void CompileACS()
		{
			Compiler compiler;
			List<CompilerError> errors = new List<CompilerError>();
			string inputfile = Path.GetFileName(filepathname);

			// Which compiler to use?
			ScriptConfiguration scriptconfig;
			if(!string.IsNullOrEmpty(General.Map.Options.ScriptCompiler))
			{
				// Boilderplate
				if(!General.CompiledScriptConfigs.ContainsKey(General.Map.Options.ScriptCompiler))
				{
					General.ShowErrorMessage("Unable to compile \"" + inputfile + "\". Unable to find required script compiler configuration (\"" + General.Map.Options.ScriptCompiler + "\").", MessageBoxButtons.OK);
					return;
				}

				scriptconfig = General.CompiledScriptConfigs[General.Map.Options.ScriptCompiler];
			}
			else
			{
				scriptconfig = config;
			}

			// Initialize compiler
			try
			{
				compiler = scriptconfig.Compiler.Create();
			}
			catch(Exception e)
			{
				// Fail
				errors.Add(new CompilerError("Unable to initialize compiler. " + e.GetType().Name + ": " + e.Message));
				panel.ShowErrors(errors);
				return;
			}

			// Preprocess the file
			AcsParserSE parser = new AcsParserSE
			{
				OnInclude = delegate(AcsParserSE se, string includefile, AcsParserSE.IncludeType includetype)
				{
					TextResourceData data = General.Map.Data.LoadFile(includefile);
					if(data == null)
					{
						// Fial
						errors.Add(new CompilerError("Unable to find include file \"" + includefile + "\""));
						panel.ShowErrors(errors);
					}
					else
					{
						se.Parse(data, true, includetype, false);
					}
				}
			};
			using(FileStream stream = File.OpenRead(filepathname))
			{
				TextResourceData data = new TextResourceData(stream, new DataLocation(), filepathname, false);
				if(!parser.Parse(data, scriptconfig.Compiler.Files, true, AcsParserSE.IncludeType.NONE, false))
				{
					// Check for errors
					if(parser.HasError)
					{
						errors.Add(new CompilerError(parser.ErrorDescription, parser.ErrorSource, parser.ErrorLine));
						panel.ShowErrors(errors);
					}

					compiler.Dispose();
					return;
				}
			}

			//mxd. Only works for libraries
			if(!parser.IsLibrary)
			{
				errors.Add(new CompilerError("External ACS files can only be compiled as libraries!", filepathname));
				panel.ShowErrors(errors);
				compiler.Dispose();
				return;
			}

			// Make random output filename
			string outputfile = General.MakeTempFilename(compiler.Location, "tmp");

			// Run compiler
			compiler.Parameters = config.Parameters;
			compiler.InputFile = inputfile;
			compiler.OutputFile = outputfile;
			compiler.SourceFile = filepathname;
			compiler.WorkingDirectory = Path.GetDirectoryName(filepathname);
			compiler.Includes = parser.Includes;
			compiler.CopyIncludesToWorkingDirectory = false;
			if(compiler.Run())
			{
				// Fetch errors
				foreach(CompilerError e in compiler.Errors)
				{
					CompilerError newerr = e;

					// If the error's filename equals our temporary file,
					// replace it with the original source filename
					if(string.Compare(e.filename, inputfile, true) == 0)
						newerr.filename = filepathname;

					errors.Add(newerr);
				}

				// No errors and output file exists?
				if(compiler.Errors.Length == 0)
				{
					// Output file exists?
					if(!File.Exists(outputfile))
					{
						// Fail
						compiler.Dispose();
						errors.Add(new CompilerError("Output file \"" + outputfile + "\" doesn't exist."));
						panel.ShowErrors(errors);
						return;
					}

					// Rename and copy to source file directory
					string targetfilename = Path.Combine(Path.GetDirectoryName(filepathname), parser.LibraryName + ".o");
					try
					{
						File.Copy(outputfile, targetfilename, true);
					}
					catch(Exception e)
					{
						// Fail
						compiler.Dispose();
						errors.Add(new CompilerError("Unable to create library file \"" + targetfilename + "\". " + e.GetType().Name + ": " + e.Message));
						panel.ShowErrors(errors);
						return;
					}
				}
			}

			// Dispose compiler
			compiler.Dispose();

			// Update script navigator
			errors.AddRange(UpdateNavigator());

			// Feed errors to panel
			panel.ShowErrors(errors);
		}

		// This checks if a script error applies to this script
		public override bool VerifyErrorForScript(CompilerError e)
		{
			return (string.Compare(e.filename, filepathname, true) == 0);
		}
		
		// This saves the document (used for both explicit and implicit)
		// Return true when successfully saved
		public override bool Save()
		{
			try
			{
				// Write the file
				File.WriteAllBytes(filepathname, editor.GetText());
			}
			catch(Exception e)
			{
				// Failed
				General.ErrorLogger.Add(ErrorType.Error, "Cannot open file \"" + filepathname + "\" for writing. Make sure the path exists and that the file is not in use by another application.");
				General.WriteLogLine(e.GetType().Name + ": " + e.Message);
				General.ShowErrorMessage("Unable to open file \"" + filepathname + "\" for writing. Make sure the path exists and that the file is not in use by another application.", MessageBoxButtons.OK);
				return false;
			}
			
			// Done
			editor.SetSavePoint(); //mxd
			UpdateTitle(); //mxd
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
				editor.Text = File.ReadAllText(filepathname); //mxd
			}
			catch(Exception e)
			{
				// Failed
				General.ErrorLogger.Add(ErrorType.Error, "Cannot open file \"" + filepathname + "\" for reading. Make sure the path exists and that the file is not in use by another application.");
				General.WriteLogLine(e.GetType().Name + ": " + e.Message);
				General.ShowErrorMessage("Unable to open file \"" + filepathname + "\" for reading. Make sure the path exists and that the file is not in use by another application.", MessageBoxButtons.OK);
				return false;
			}
			
			// Setup
			this.filepathname = filepathname;
			editor.ClearUndoRedo();
			SetTitle(Path.GetFileName(filepathname));
			panel.ShowErrors(UpdateNavigator()); //mxd

			return true;
		}

		// This changes the script configurations
		public override void ChangeScriptConfig(ScriptConfiguration newconfig)
		{
			this.config = newconfig;
			editor.SetupStyles(config);

			if(filepathname.Length == 0)
			{
				string ext = (config.Extensions.Length > 0 ? "." + config.Extensions[0] : "");
				SetTitle("Untitled" + ext);
			}
			else
			{
				UpdateTitle(); //mxd
			}

			//mxd
			base.ChangeScriptConfig(newconfig);
		}
		
		#endregion
		
		#region ================== Events
		
		#endregion
	}
}
