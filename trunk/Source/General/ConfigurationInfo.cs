using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace CodeImp.DoomBuilder
{
	internal struct ConfigurationInfo : IComparable<ConfigurationInfo>
	{
		// Members
		public string name;
		public string filename;
		
		// Constructor
		public ConfigurationInfo(string name, string filename)
		{
			// Initialize
			this.name = name;
			this.filename = filename;
		}

		// This compares it to other ConfigurationInfo objects
		public int CompareTo(ConfigurationInfo other)
		{
			// Compare
			return name.CompareTo(other.name);
		}
	}
}
