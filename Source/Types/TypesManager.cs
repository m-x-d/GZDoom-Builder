
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
using CodeImp.DoomBuilder.Config;

#endregion

namespace CodeImp.DoomBuilder.Types
{
	internal class TypesManager : IDisposable
	{
		#region ================== Constants

		#endregion

		#region ================== Variables

		// List of handler types
		private Dictionary<int, Type> handlertypes;
		
		// Disposing
		private bool isdisposed = false;

		#endregion

		#region ================== Properties

		public bool IsDisposed { get { return isdisposed; } }

		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		public TypesManager()
		{
			// Initialize
			handlertypes = new Dictionary<int, Type>();

			// Go for all types in this assembly
			Type[] types = General.ThisAssembly.GetTypes();
			foreach(Type tp in types)
			{
				// Check if this type is a class
				if(tp.IsClass && !tp.IsAbstract && !tp.IsArray)
				{
					// Check if class has an TypeHandler attribute
					if(Attribute.IsDefined(tp, typeof(TypeHandlerAttribute), false))
					{
						// Add the type to the list
						object[] attribs = tp.GetCustomAttributes(typeof(TypeHandlerAttribute), false);
						TypeHandlerAttribute attr = (attribs[0] as TypeHandlerAttribute);
						handlertypes.Add(attr.Index, tp);
					}
				}
			}
			
			// We have no destructor
			GC.SuppressFinalize(this);
		}

		// Disposer
		public void Dispose()
		{
			// Not already disposed?
			if(!isdisposed)
			{
				// Clean up
				handlertypes.Clear();
				
				// Done
				isdisposed = true;
			}
		}

		#endregion

		#region ================== Methods

		// This returns the type handler for the given argument
		public TypeHandler GetArgumentHandler(ArgumentInfo arginfo)
		{
			Type t = typeof(NullHandler);

			// Do we have a handler type for this?
			if(handlertypes.ContainsKey(arginfo.Type)) t = handlertypes[arginfo.Type];

			// Create instance
			TypeHandler th = (TypeHandler)General.ThisAssembly.CreateInstance(t.FullName);
			th.SetupArgument(arginfo);
			return th;
		}

		#endregion
	}
}
