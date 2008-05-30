
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
using System.Drawing;

#endregion

namespace CodeImp.DoomBuilder.Types
{
	[TypeHandler(10, "Color", true)]
	internal class ColorHandler : TypeHandler
	{
		#region ================== Constants

		#endregion

		#region ================== Variables

		private int value;		// XXRRGGBB color

		#endregion

		#region ================== Properties

		public override bool IsBrowseable { get { return true; } }

		#endregion

		#region ================== Constructor

		#endregion

		#region ================== Methods

		public override void Browse(IWin32Window parent)
		{
			ColorDialog dialog = new ColorDialog();
			dialog.AllowFullOpen = true;
			dialog.AnyColor = true;
			dialog.Color = Color.FromArgb((value >> 16) & 0xFF, (value >> 8) & 0xFF, value & 0xFF);
			dialog.FullOpen = true;
			if(dialog.ShowDialog(parent) == DialogResult.OK)
			{
				value = dialog.Color.ToArgb() & 0x00FFFFFF;
			}
		}

		public override void SetValue(object value)
		{
			int result;

			// Already an int or float?
			if(value is int)
			{
				// Set directly
				this.value = (int)value;
			}
			// String?
			else if(value is string)
			{
				// Try parsing as string
				if(int.TryParse(value.ToString(), NumberStyles.HexNumber, CultureInfo.CurrentCulture, out result))
				{
					this.value = result;
				}
				else
				{
					this.value = 0;
				}
			}
			else
			{
				this.value = 0;
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
			return this.value.ToString("X6");
		}

		#endregion
	}
}
