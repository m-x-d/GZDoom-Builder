
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
		private UniversalFieldType type;
		private string defstring;
		private int defint;
		private float deffloat;

		#endregion

		#region ================== Properties

		public string Name { get { return name; } }
		public UniversalFieldType Type { get { return type; } }
		public string DefaultStr { get { return defstring; } }
		public int DefaultInt { get { return defint; } }
		public float DefaultFloat { get { return deffloat; } }

		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		internal UniversalFieldInfo(string path, string name, Configuration cfg)
		{
			string setting = "universalfields." + path + "." + name;

			// Initialize
			this.name = name;

			// Read type
			this.type = (UniversalFieldType)cfg.ReadSetting(setting + ".type", 0);
			switch(this.type)
			{
				case UniversalFieldType.Integer:
				case UniversalFieldType.LinedefAction:
				case UniversalFieldType.SectorEffect:
					defint = cfg.ReadSetting(setting + ".default", 0);
					deffloat = (float)defint;
					defstring = DefaultInt.ToString(CultureInfo.InvariantCulture);
					break;

				case UniversalFieldType.Float:
					deffloat = cfg.ReadSetting(setting + ".default", 0.0f);
					defint = (int)Math.Round(deffloat);
					defstring = DefaultFloat.ToString(CultureInfo.InvariantCulture);
					break;

				case UniversalFieldType.String:
				case UniversalFieldType.Flat:
				case UniversalFieldType.Texture:
					defstring = cfg.ReadSetting(setting + ".default", "");
					float.TryParse(DefaultStr, NumberStyles.Number, CultureInfo.InvariantCulture, out deffloat);
					int.TryParse(DefaultStr, NumberStyles.Number, CultureInfo.InvariantCulture, out defint);
					break;
					
				default:
					General.WriteLogLine("WARNING: Universal field '" + path + "." + name + "' is defined as unknown type " + (int)this.type + "!");
					break;
			}
			
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
