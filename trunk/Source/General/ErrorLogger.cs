
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

#endregion

namespace CodeImp.DoomBuilder
{
	public class ErrorLogger
	{
		#region ================== Constants

		#endregion

		#region ================== Variables
		
		private List<ErrorItem> errors;
		private volatile bool changed;
		
		#endregion

		#region ================== Properties
		
		public bool HasErrors { get { return (errors.Count > 0); } }
		public bool HasChanged { get { return changed; } set { changed = value; } }
		
		#endregion

		#region ================== Constructor / Disposer
		
		// Constructor
		internal ErrorLogger()
		{
			errors = new List<ErrorItem>();
		}
		
		#endregion
		
		#region ================== Methods

		// This clears the errors
		public void Clear()
		{
			lock(this)
			{
				errors.Clear();
				changed = false;
			}
		}
		
		// This adds a new error
		public void AddError(ErrorType type, string message)
		{
			lock(this)
			{
				errors.Add(new ErrorItem(type, message));
				changed = true;
			}
		}
		
		// This returns the list of errors
		internal List<ErrorItem> GetErrors()
		{
			lock(this)
			{
				List<ErrorItem> copylist = new List<ErrorItem>(errors);
				return copylist;
			}
		}
		
		#endregion
	}
}
