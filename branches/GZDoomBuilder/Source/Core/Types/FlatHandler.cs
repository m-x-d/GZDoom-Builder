
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
using System.Drawing;
using System.Globalization;
using System.Text;
using CodeImp.DoomBuilder.IO;
using CodeImp.DoomBuilder.Data;
using System.IO;
using System.Diagnostics;
using System.Windows.Forms;
using CodeImp.DoomBuilder.Windows;

#endregion

namespace CodeImp.DoomBuilder.Types
{
	[TypeHandler(UniversalType.Flat, "Flat", true)]
	internal class FlatHandler : TypeHandler
	{
		#region ================== Constants

		#endregion

		#region ================== Variables

		private string value = "";

		#endregion

		#region ================== Properties

		public override bool IsBrowseable { get { return true; } }

		public override Image BrowseImage { get { return Properties.Resources.List_Images; } }
		
		#endregion

		#region ================== Methods

		public override void Browse(IWin32Window parent)
		{
            this.value = TextureBrowserForm.Browse(parent, this.value, true); //mxd. was FlatBrowserForm
		}

		public override void SetValue(object value)
		{
			if(value != null)
				this.value = value.ToString();
			else
				this.value = "";
		}

		public override object GetValue()
		{
			return this.value;
		}

		public override string GetStringValue()
		{
			return this.value;
		}

		#endregion
	}
}
