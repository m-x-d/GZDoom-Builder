
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
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Data;
using CodeImp.DoomBuilder.IO;
using System.IO;
using CodeImp.DoomBuilder.Config;

#endregion

namespace CodeImp.DoomBuilder.Interface
{
	public partial class LinedefEditForm : DelayedForm
	{
		// Variables
		private ICollection<Linedef> lines;
		
		// Constructor
		public LinedefEditForm()
		{
			// Initialize
			InitializeComponent();
			
			// Fill linedef flags list
			foreach(KeyValuePair<int, string> lf in General.Map.Config.LinedefFlags) flags.Add(lf.Value, lf.Key);

			// Fill linedef actions list
			action.AddInfo(General.Map.Config.SortedLinedefActions.ToArray());
		}

		// This sets up the form to edit the given lines
		public void Setup(ICollection<Linedef> lines)
		{
			// Keep this list
			this.lines = lines;

			// Go for all flags
			foreach(CheckBox c in flags.Checkboxes)
			{
				// Set the option with the first line's setting
				c.Checked = (General.GetByIndex<Linedef>(lines, 0).Flags & (int)c.Tag) != 0;
				
				// Go for all lines
				foreach(Linedef l in lines)
				{
					// Make the option gray if it is different
					if(((l.Flags & (int)c.Tag) != 0) != c.Checked)
						c.CheckState = CheckState.Indeterminate;
				}
			}

			// Set the action to the first line's action
			action.Value = General.GetByIndex<Linedef>(lines, 0).Action;

			// Go for all lines
			foreach(Linedef l in lines)
			{
				// Erase the option if it is different
				if(l.Action != action.Value)
					action.Empty = true;
			}
		}
	}
}
