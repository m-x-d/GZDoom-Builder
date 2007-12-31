using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Forms;

namespace CodeImp.DoomBuilder.Controls
{
	public struct KeyControl
	{
		#region ================== Variables

		public int key;
		public string name;

		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		public KeyControl(Keys key, string name)
		{
			// Initialize
			this.key = (int)key;
			this.name = name;
		}

		// Constructor
		public KeyControl(SpecialKeys key, string name)
		{
			// Initialize
			this.key = (int)key;
			this.name = name;
		}

		// Constructor
		public KeyControl(int key, string name)
		{
			// Initialize
			this.key = key;
			this.name = name;
		}

		#endregion

		#region ================== Methods

		// Returns name
		public override string ToString()
		{
			return name;
		}
		
		#endregion
	}
}
