
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
	public class GeneralActionOption
	{
		#region ================== Constants

		#endregion

		#region ================== Variables

		// Properties
		private string name;
		private List<GeneralActionBit> bits;
		
		#endregion

		#region ================== Properties

		public string Name { get { return name; } }
		public List<GeneralActionBit> Bits { get { return bits; } }

		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		public GeneralActionOption(string cat, string name, IDictionary bitslist)
		{
			int index;
			
			// Initialize
			this.name = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(name);
			this.bits = new List<GeneralActionBit>();

			// Go for all bits
			foreach(DictionaryEntry de in bitslist)
			{
				// Check if the item key is numeric
				if(int.TryParse(de.Key.ToString(), NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite, CultureInfo.InvariantCulture, out index))
				{
					// Add to list
					this.bits.Add(new GeneralActionBit(index, de.Value.ToString()));
				}
				else
				{
					General.WriteLogLine("WARNING: Structure 'gen_linedefflags." + cat + "." + name + "' contains invalid entries!");
				}
			}
			
			// Sort the list
			bits.Sort();
			
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
		
		#endregion
	}
}
