
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

#endregion

namespace CodeImp.DoomBuilder.Compilers
{
	public abstract class Compiler
	{
		#region ================== Variables
		
		// Errors
		private List<CompilerError> errors;
		
		#endregion
		
		#region ================== Properties
		
		public CompilerError[] Errors { get { return errors.ToArray(); } }
		
		#endregion
		
		#region ================== Constructor
		
		// Constructor
		public Compiler()
		{
			// Initialize
			this.errors = new List<CompilerError>();
		}
		
		#endregion
		
		#region ================== Methods
		
		/// <summary>
		/// This runs the compiler.
		/// </summary>
		/// <returns>Returns false when failed to start.</returns>
		public abstract bool Run();
		
		// This reports an error
		protected void ReportError(CompilerError err)
		{
			this.errors.Add(err);
		}
		
		// This creates a compiler by interface name
		internal static Compiler Create(string name)
		{
			// Make list of assemblies to search in
			List<Assembly> asms = General.Plugins.GetPluginAssemblies();
			asms.Add(General.ThisAssembly);
			
			try
			{
				
				// TODO
				
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
		}
		
		#endregion
	}
}

