#region ================== Namespaces

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

#endregion

namespace CodeImp.DoomBuilder.Map
{
	public class UniFields : SortedList<string, UniValue>
	{
		// New constructor
		public UniFields() : base(2)
		{
		}

		// New constructor
		public UniFields(int capacity) : base(capacity)
		{
		}

		// Copy constructor
		public UniFields(UniFields copyfrom) : base(copyfrom)
		{
		}
	}
}
