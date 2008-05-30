
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
using CodeImp.DoomBuilder.Geometry;
using CodeImp.DoomBuilder.Rendering;
using SlimDX.Direct3D9;
using System.Drawing;
using CodeImp.DoomBuilder.Types;

#endregion

namespace CodeImp.DoomBuilder.Map
{
	public class UniValue
	{
		#region ================== Constants

		private const string NAME_CHARS = "abcdefghijklmnopqrstuvwxyz0123456789_";
		
		#endregion

		#region ================== Variables
		
		private object value;
		private int type;

		#endregion

		#region ================== Properties

		public object Value
		{
			get
			{
				return this.value;
			}
			
			set
			{
				// Value may only be a primitive type
				if((!(value is int) && !(value is float) && !(value is string) && !(value is bool)) || (value == null))
					throw new ArgumentException("Universal field values can only be of type int, float, string or bool.");
				
				this.value = value;
			}
		}
		
		public int Type { get { return this.type; } set { this.type = value; } }

		#endregion

		#region ================== Constructor

		// Constructor
		public UniValue(int type, object value)
		{
			this.type = type;
			this.value = value;
			
			// We have no destructor
			GC.SuppressFinalize(this);
		}

		// Constructor
		public UniValue()
		{
			// We have no destructor
			GC.SuppressFinalize(this);
		}
		
		#endregion

		#region ================== Methods

		// This validates a UDMF field name and returns the valid part
		public static string ValidateName(string name)
		{
			// Keep only valid characters
			string fieldname = name.Trim().ToLowerInvariant();
			string validname = "";
			for(int c = 0; c < fieldname.Length; c++)
			{
				if(NAME_CHARS.IndexOf(fieldname[c]) > -1) validname += fieldname[c];
			}
			return validname;
		}

		#endregion
	}
}
