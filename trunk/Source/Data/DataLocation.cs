
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

#endregion

namespace CodeImp.DoomBuilder.Data
{
	internal struct DataLocation : IComparable<DataLocation>, IComparable, IEquatable<DataLocation>
	{
		// Constants
		public const int RESOURCE_WAD = 0;
		public const int RESOURCE_DIRECTORY = 1;
		
		// Members
		public int type;
		public string location;
		public bool textures;
		public bool flats;
		
		// Constructor
		public DataLocation(int type, string location, bool textures, bool flats)
		{
			// Initialize
			this.type = type;
			this.location = location;
			this.textures = textures;
			this.flats = flats;
		}

		// This displays the struct as string
		public override string ToString()
		{
			// Simply show location
			return location;
		}

		// This compares two locations
		public int CompareTo(DataLocation other)
		{
			return string.Compare(this.location, other.location, true);
		}
		
		// This compares two locations
		public int CompareTo(object obj)
		{
			return string.Compare(this.location, ((DataLocation)obj).location, true);
		}
		
		// This compares two locations
		public bool Equals(DataLocation other)
		{
			return (this.CompareTo(other) == 0);
		}
	}
}
