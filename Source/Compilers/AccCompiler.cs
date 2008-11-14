
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

#endregion

namespace CodeImp.DoomBuilder.Compilers
{
	public sealed class AccCompiler : Compiler
	{
		#region ================== Constants
		
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
		
		// This runs the compiler with a file as input.
		public override bool CompileFile(string filename)
		{
			return true;
		}

		// This runs the compiler with lump data as input.
		public override bool CompileLump(Stream lumpdata)
		{
			return true;
		}
		
		#endregion
	}
}

