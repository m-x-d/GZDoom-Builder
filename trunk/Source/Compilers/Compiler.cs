
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
using System.Reflection;
using System.IO;
using CodeImp.DoomBuilder.Config;
using CodeImp.DoomBuilder.IO;

#endregion

namespace CodeImp.DoomBuilder.Compilers
{
	public abstract class Compiler : IDisposable
	{
		#region ================== Variables
		
		// Parameters
		protected CompilerInfo info;
		protected string parameters;
		protected string workingdir;
		
		// Files
		protected DirectoryInfo tempdir;

		// Errors
		private List<CompilerError> errors;
		
		// Disposing
		protected bool isdisposed;
		
		#endregion
		
		#region ================== Properties

		public string Parameters { get { return parameters; } set { parameters = value; } }
		public string WorkingDirectory { get { return workingdir; } set { workingdir = value; } }
		public string Location { get { return tempdir.FullName; } }
		public bool IsDisposed { get { return isdisposed; } }
		public CompilerError[] Errors { get { return errors.ToArray(); } }
		
		#endregion
		
		#region ================== Constructor / Disposer
		
		// Constructor
		public Compiler(CompilerInfo info)
		{
			// Initialize
			this.info = info;
			this.errors = new List<CompilerError>();
			
			General.WriteLogLine("Creating compiler '" + info.Name + "' on interface '" + this.GetType().Name + "'...");
			
			// Create temporary directory
			tempdir = Directory.CreateDirectory(General.MakeTempDirname());
			workingdir = tempdir.FullName;
			
			// Copy required files to the temp directory
			General.WriteLogLine("Copying required files for compiler...");
			CopyRequiredFiles();
		}
		
		// Disposer
		public virtual void Dispose()
		{
			if(!isdisposed)
			{
				// Remove temporary directory
				General.WriteLogLine("Removing temporary compiler files...");
				tempdir.Delete(true);
				
				// Disposed
				isdisposed = true;
			}
		}
		
		#endregion
		
		#region ================== Methods

		// This copies all compiler files to a given destination
		private void CopyRequiredFiles()
		{
			// Copy files
			foreach(string f in info.Files)
			{
				string sourcefile = Path.Combine(General.CompilersPath, f);
				string targetfile = Path.Combine(tempdir.FullName, f);
				if(!File.Exists(sourcefile)) General.WriteLogLine("ERROR: The file '" + f + "' required by the '" + info.Name + "' compiler is missing!");
				File.Copy(sourcefile, targetfile, true);
			}
		}
		
		/// <summary>
		/// This runs the compiler with a file as input.
		/// </summary>
		/// <returns>Returns false when failed to start.</returns>
		public virtual bool CompileFile(string filename) { return false; }

		/// <summary>
		/// This runs the compiler with lump data as input.
		/// </summary>
		/// <returns>Returns false when failed to start.</returns>
		public virtual bool CompileLump(Stream lumpdata) { return false; }

		/// <summary>
		/// Use this to report an error.
		/// </summary>
		protected void ReportError(CompilerError err)
		{
			this.errors.Add(err);
		}
		
		// This creates a compiler by interface name
		internal static Compiler Create(CompilerInfo info)
		{
			Compiler result;
			
			// Make list of assemblies to search in
			List<Assembly> asms = General.Plugins.GetPluginAssemblies();
			asms.Add(General.ThisAssembly);
			
			// Constructor arguments
			object[] args = new object[1];
			args[0] = info;
			
			try
			{
				// Go for all assemblies
				foreach(Assembly a in asms)
				{
					// Find the class
					Type[] types = a.GetExportedTypes();
					foreach(Type t in types)
					{
						if(t.IsSubclassOf(typeof(Compiler)) && (t.Name == info.ProgramInterface))
						{
							// Create instance
							result = (Compiler)a.CreateInstance(t.FullName, false, BindingFlags.Default,
												null, args, CultureInfo.CurrentCulture, new object[0]);
							return result;
						}
					}
				}
			}
			// Catch errors
			catch(TargetInvocationException e)
			{
				// Throw the actual exception
				Debug.WriteLine(DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString());
				Debug.WriteLine(e.InnerException.Source + " throws " + e.InnerException.GetType().Name + ":");
				Debug.WriteLine(e.InnerException.Message);
				Debug.WriteLine(e.InnerException.StackTrace);
				throw e.InnerException;
			}

			// No such compiler
			return null;
		}
		
		#endregion
	}
}

