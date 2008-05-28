
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

#endregion

namespace CodeImp.DoomBuilder.Config
{
	public class EnumList : List<EnumItem>
	{
		#region ================== Constants

		#endregion

		#region ================== Variables

		private string name;

		#endregion

		#region ================== Properties

		public string Name { get { return name; } }

		#endregion

		#region ================== Constructor

		// Constructor to load from configuration
		internal EnumList(string name, Configuration cfg)
		{
			int index;
			
			// Initialize
			this.name = name;

			// Read the list from configuration
			IDictionary dic = cfg.ReadSetting("enums." + name, new Hashtable());
			foreach(DictionaryEntry de in dic)
			{
				// Try paring the bit value
				if(int.TryParse(de.Key.ToString(),
					NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite,
					CultureInfo.InvariantCulture, out index))
				{
					// Add item
					EnumItem item = new EnumItem(index, de.Value.ToString());
					base.Add(item);
				}
				else
				{
					General.WriteLogLine("WARNING: Enum structure '" + name + "' contains invalid keys!");
				}
			}
		}

		#endregion

		#region ================== Methods

		// This gets an item by enum index
		// Returns null when item could not be found
		public EnumItem GetByEnumIndex(int enumindex)
		{
			// Find the item
			foreach(EnumItem i in this)
			{
				if(i.Index == enumindex) return i;
			}

			// Nothing found
			return null;
		}

		#endregion
	}
}
