
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
using CodeImp.DoomBuilder.Windows;
using System.Windows.Forms;

#endregion

namespace CodeImp.DoomBuilder.Types
{
	[TypeHandler(UniversalType.SectorEffect, "Sector Effect", true)]
	internal class SectorEffectHandler : TypeHandler
	{
		#region ================== Constants

		#endregion

		#region ================== Variables

		private int value;

		#endregion

		#region ================== Properties

		public override bool IsBrowseable { get { return true; } }

		#endregion

		#region ================== Constructor

		#endregion

		#region ================== Methods

		public override void Browse(IWin32Window parent)
		{
			this.value = EffectBrowserForm.BrowseEffect(parent, this.value);
		}

		public override void SetValue(object value)
		{
			int result;

			// Null?
			if(value == null)
			{
				this.value = 0;
			}
			// Already an int or float?
			else if((value is int) || (value is float))
			{
				// Set directly
				this.value = (int)value;
			}
			else
			{
				// Try parsing as string
				if(int.TryParse(value.ToString(), NumberStyles.Integer, CultureInfo.CurrentCulture, out result))
				{
					this.value = result;
				}
				else
				{
					this.value = 0;
				}
			}
		}

		public override object GetValue()
		{
			return this.value;
		}

		public override int GetIntValue()
		{
			return this.value;
		}

		public override string GetStringValue()
		{
			return this.value.ToString();
		}

		#endregion
	}
}
