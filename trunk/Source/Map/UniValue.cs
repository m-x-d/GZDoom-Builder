
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
				if(!(value is int) || !(value is float) || !(value is string) || !(value is bool))
					throw new ArgumentException("Universal field values can only be of type int, float, string or bool.");
				
				this.value = value;
			}
		}
		
		public int Type { get { return this.type; } set { this.type = value; } }

		#endregion

		#region ================== Constructor

		// Constructor
		public UniValue()
		{
			// We have no destructor
			GC.SuppressFinalize(this);
		}
		
		#endregion

		#region ================== Methods

		#endregion
	}
}
