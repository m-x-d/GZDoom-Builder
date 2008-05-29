
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
	[TypeHandler(11)]
	internal class EnumOptionHandler : TypeHandler
	{
		#region ================== Constants

		#endregion

		#region ================== Variables

		private EnumList list;
		private EnumItem value;
		
		#endregion

		#region ================== Properties

		public virtual bool IsBrowseable { get { return true; } }
		public virtual bool IsEnumerable { get { return true; } }
		
		#endregion

		#region ================== Constructor

		// When set up for an argument
		public override void SetupArgument(ArgumentInfo arginfo)
		{
			base.SetupArgument(arginfo);

			// Keep enum list reference
			list = arginfo.Enum;
		}
		
		#endregion
		
		#region ================== Methods

		public override void SetValue(object value)
		{
			this.value = null;
			
			// First try to match the value against the enum values
			foreach(EnumItem item in list)
			{
				// Matching value?
				if(item.Value == value.ToString())
				{
					// Set this value
					this.value = item;
				}
			}

			// No match found yet?
			if(this.value == null)
			{
				// Try to match against the titles
				foreach(EnumItem item in list)
				{
					// Matching value?
					if(item.Title.ToLowerInvariant() == value.ToString().ToLowerInvariant())
					{
						// Set this value
						this.value = item;
					}
				}
			}

			// Still no match found?
			if(this.value == null)
			{
				// Make a dummy value
				this.value = new EnumItem(value.ToString(), value.ToString());
			}
		}

		public override int GetIntValue()
		{
			if(this.value != null)
			{
				// Parse the value to integer
				int result;
				if(int.TryParse(this.value.Value, NumberStyles.Integer,
								CultureInfo.InvariantCulture, out result))
				{
					return result;
				}
				else
				{
					return 0;
				}
			}
			else
			{
				return 0;
			}
		}
		
		public override string GetStringValue()
		{
			if(this.value != null) return this.value.Title; else return "NULL";
		}

		// This returns an enum list
		public override EnumList GetEnumList()
		{
			return list;
		}
		
		#endregion
	}
}
