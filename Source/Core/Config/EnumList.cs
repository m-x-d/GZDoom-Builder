
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

using System.Collections;
using System.Collections.Generic;
using CodeImp.DoomBuilder.IO;

#endregion

namespace CodeImp.DoomBuilder.Config
{
	public class EnumList : List<EnumItem>
	{
		#region ================== Constants

		#endregion

		#region ================== Variables

		#endregion

		#region ================== Properties

		#endregion

		#region ================== Constructor

		// Constructor for custom list
		internal EnumList()
		{
		}

		// Constructor to load from dictionary
		internal EnumList(IDictionary dic)
		{
			// Read the dictionary
			foreach(DictionaryEntry de in dic)
			{
				// Add item
				EnumItem item = new EnumItem(de.Key.ToString(), de.Value.ToString());
				base.Add(item);
			}
		}

		// Constructor to load from configuration
		internal EnumList(string name, Configuration cfg)
		{
			// Read the list from configuration
			IDictionary dic = cfg.ReadSetting("enums." + name, new Hashtable());
			foreach(DictionaryEntry de in dic)
			{
				// Add item
				EnumItem item = new EnumItem(de.Key.ToString(), de.Value.ToString());
				base.Add(item);
			}
		}

		#endregion

		#region ================== Methods

		// This gets an item by value
		// Returns null when item could not be found
		public EnumItem GetByEnumIndex(string value)
		{
			// Find the item
			foreach(EnumItem i in this)
			{
				if(i.Value == value) return i;
			}

			// Nothing found
			return null;
		}

		#endregion
	}
}
