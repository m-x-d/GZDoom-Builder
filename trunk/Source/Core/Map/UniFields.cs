#region ================== Namespaces

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

#endregion

namespace CodeImp.DoomBuilder.Map
{
	/// <summary>
	/// List of universal fields and their values.
	/// </summary>
	public class UniFields : SortedList<string, UniValue>
	{
		// New constructor
		///<summary></summary>
		public UniFields() : base(2)
		{
		}

		// New constructor
		///<summary></summary>
		public UniFields(int capacity) : base(capacity)
		{
		}

		// Copy constructor
		///<summary></summary>
		public UniFields(UniFields copyfrom) : base(copyfrom)
		{
		}
	}
}
