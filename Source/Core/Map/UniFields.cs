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
		// Owner of this list
		protected MapElement owner;
		public MapElement Owner { get { return owner; } internal set { owner = value; } }
		
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
		
		// New constructor
		///<summary></summary>
		public UniFields(MapElement owner) : base(2)
		{
			this.owner = owner;
		}

		// New constructor
		///<summary></summary>
		public UniFields(MapElement owner, int capacity) : base(capacity)
		{
			this.owner = owner;
		}

		// Copy constructor
		///<summary></summary>
		public UniFields(MapElement owner, UniFields copyfrom) : base(copyfrom)
		{
			this.owner = owner;
		}
		
		/// <summary>Call this before making changes to the fields, or they may not be updated correctly with undo/redo!</summary>
		public void BeforeFieldsChange()
		{
			if(owner != null)
				owner.BeforeFieldsChange();
		}
	}
}
