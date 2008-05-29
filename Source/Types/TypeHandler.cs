
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
using CodeImp.DoomBuilder.Config;

#endregion

namespace CodeImp.DoomBuilder.Types
{
	/// <summary>
	/// Type Handler base class. A Type Handler takes care of editing, validating and
	/// displaying values of different types for UDMF fields and hexen arguments.
	/// </summary>
	internal abstract class TypeHandler
	{
		#region ================== Constants

		#endregion

		#region ================== Variables

		protected int index;
		protected ArgumentInfo arginfo;
		
		#endregion

		#region ================== Properties

		public int Index { get { return index; } }

		public virtual bool IsBrowseable { get { return false; } }
		public virtual bool IsEnumerable { get { return false; } }
		public virtual bool IsCustomType { get { return false; } }

		#endregion

		#region ================== Constructor

		// Constructor
		public TypeHandler()
		{
			// Get my attributes
			object[] attrs = this.GetType().GetCustomAttributes(typeof(TypeHandlerAttribute), false);
			if(attrs.Length > 0)
			{
				// Set index from attribute
				this.index = (attrs[0] as TypeHandlerAttribute).Index;
			}
			else
			{
				// Indexless
				this.index = -1;
			}
		}

		// This sets up the handler for arguments
		public virtual void SetupArgument(ArgumentInfo arginfo)
		{
			// Setup
			this.arginfo = arginfo;
		}
		
		#endregion

		#region ================== Methods

		// This must set the value
		// How the value is actually validated and stored is up to the implementation
		public abstract void SetValue(object value);

		// This must return the value as one of the primitive data types
		// supported by UDMF: int, string, float or bool
		public abstract object GetValue();
		
		// This must return the value as integer (for arguments)
		public virtual int GetIntValue()
		{
			throw new NotSupportedException("Override this method to support it as integer for arguments");
		}

		// This must return the value as a string for displaying
		public abstract string GetStringValue();

		// This is called when the user presses the browse button
		public virtual void Browse(IWin32Window parent)
		{
		}
		
		// This must returns an enum list when IsEnumerable is true
		// When the user chooses an enum from this list, it will be
		// set using SetValue with the EnumItem as value.
		public virtual EnumList GetEnumList()
		{
			return null;
		}
		
		// String representation
		public override string ToString()
		{
			return this.GetStringValue();
		}

		#endregion
	}
}
