
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
using CodeImp.DoomBuilder.Map;

#endregion

namespace CodeImp.DoomBuilder.Config
{
	public class UniversalFieldInfo : IComparable<UniversalFieldInfo>
	{
		#region ================== Constants

		#endregion

		#region ================== Variables

		// Properties
		private string name;
		private int type;
		private object defaultvalue;

		#endregion

		#region ================== Properties

		public string Name { get { return name; } }
		public int Type { get { return type; } }
		public object Default { get { return defaultvalue; } }

		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		internal UniversalFieldInfo(string path, string name, Configuration cfg)
		{
			string setting = "universalfields." + path + "." + name;

			// Initialize
			this.name = name.ToLowerInvariant();

			// Read type
			this.type = cfg.ReadSetting(setting + ".type", 0);
			this.defaultvalue = cfg.ReadSettingObject(setting + ".default", null);
			
			// We have no destructor
			GC.SuppressFinalize(this);
		}

		#endregion

		#region ================== Methods

		// This presents the item as string
		public override string ToString()
		{
			return name;
		}

		// This compares against another field
		public int CompareTo(UniversalFieldInfo other)
		{
			return string.Compare(this.name, other.name);
		}

		#endregion
	}
}
