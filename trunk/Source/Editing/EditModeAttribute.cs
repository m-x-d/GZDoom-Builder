
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
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

#endregion

namespace CodeImp.DoomBuilder.Editing
{
	[AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = true)]
	public class EditModeAttribute : Attribute
	{
		#region ================== Variables

		// Properties
		private string switchaction;
		private string buttonimage;
		private string buttondesc;
		private int buttonorder;
		private bool configspecific;
		
		#endregion

		#region ================== Properties

		public string SwitchAction { get { return switchaction; } set { switchaction = value; } }
		public string ButtonImage { get { return buttonimage; } set { buttonimage = value; } }
		public string ButtonDesc { get { return buttondesc; } set { buttondesc = value; } }
		public int ButtonOrder { get { return buttonorder; } set { buttonorder = value; } }
		public bool ConfigSpecific { get { return configspecific; } set { configspecific = value; } }

		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		public EditModeAttribute()
		{
			// Initialize
		}

		#endregion

		#region ================== Methods

		#endregion
	}
}
