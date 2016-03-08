
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
using System.Drawing;
using System.Windows.Forms;
using CodeImp.DoomBuilder.Config;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Windows;

#endregion

namespace CodeImp.DoomBuilder.BuilderModes
{
	[FindReplace("Linedef Flags", BrowseButton = true)]
	internal class FindLinedefFlags : BaseFindLinedef
	{
		#region ================== Constants

		#endregion

		#region ================== Variables

		#endregion

		#region ================== Properties

		public override Image BrowseImage { get { return Properties.Resources.List; } }

		#endregion

		#region ================== Constructor / Destructor

		#endregion

		#region ================== Methods

		// This is called when the browse button is pressed
		public override string Browse(string initialvalue)
		{
			return FlagsForm.ShowDialog(Form.ActiveForm, initialvalue, General.Map.Config.LinedefFlags);
		}

		// This is called to perform a search (and replace)
		// Returns a list of items to show in the results list
		// replacewith is null when not replacing
		public override FindReplaceObject[] Find(string value, bool withinselection, bool replace, string replacewith, bool keepselection)
		{
			List<FindReplaceObject> objs = new List<FindReplaceObject>();

			// Where to search?
			ICollection<Linedef> list = withinselection ? General.Map.Map.GetSelectedLinedefs(true) : General.Map.Map.Linedefs;

			// Find what? (mxd)
			List<string> findflagslist = new List<string>();
			foreach(string flag in value.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)) 
			{
				string f = flag.Trim();
				if(General.Map.Config.LinedefFlags.ContainsKey(f)) findflagslist.Add(f);
			}
			if(findflagslist.Count == 0) 
			{
				MessageBox.Show("Invalid value for this search type!", "Find and Replace", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return objs.ToArray();
			}

			// Replace with what? (mxd)
			List<string> replaceflagslist = new List<string>();
			if(replace) 
			{
				string[] replaceflags = replacewith.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
				foreach(string flag in replaceflags) 
				{
					string f = flag.Trim();
					if(!General.Map.Config.LinedefFlags.ContainsKey(f))
					{
						MessageBox.Show("Invalid replace value \"" + f + "\" for this search type!", "Find and Replace", MessageBoxButtons.OK, MessageBoxIcon.Error);
						return objs.ToArray();
					}
					replaceflagslist.Add(f);
				}
			}

			// Go for all linedefs
			foreach(Linedef l in list)
			{
				bool match = true;

				// Parse the value string...
				foreach(string flag in findflagslist)
				{
					// ... and check if the flags don't match
					if(!l.IsFlagSet(flag))
					{
						match = false;
						break;
					}
				}

				// Flags matches?
				if(match)
				{
					// Replace flags (mxd)
					if(replace) 
					{
						// Clear all flags
						l.ClearFlags();

						// Set new flags
						foreach(string flag in replaceflagslist) l.SetFlag(flag, true);
					}
					
					// Add to list
					LinedefActionInfo info = General.Map.Config.GetLinedefActionInfo(l.Action);
					if(!info.IsNull)
						objs.Add(new FindReplaceObject(l, "Linedef " + l.Index + " (" + info.Title + ")"));
					else
						objs.Add(new FindReplaceObject(l, "Linedef " + l.Index));
				}
			}

			return objs.ToArray();
		}

		#endregion
	}
}
