
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

using System.Globalization;

#endregion

namespace CodeImp.DoomBuilder.Config
{
	public class EnumItem
	{
		#region ================== Constants

		#endregion

		#region ================== Variables

		private string value;
		private string title;

		#endregion

		#region ================== Properties

		public string Value { get { return value; } }
		public string Title { get { return title; } }

		#endregion

		#region ================== Constructor

		// Constructor
		public EnumItem(string value, string title)
		{
			// Initialize
			this.value = value;
			this.title = title;
		}
		
		#endregion

		#region ================== Methods

		// String representation
		public override string ToString()
		{
			return title;
		}

		// This returns the value as int
		public int GetIntValue()
		{
			int result;
			if(int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out result))
				return result;
			else
				return 0;
		}
		
		#endregion
	}
}
