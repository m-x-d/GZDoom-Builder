
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

using System.Collections.Generic;
using System.Windows.Forms;
using CodeImp.DoomBuilder.Windows;
using CodeImp.DoomBuilder.Map;
using System.Drawing;
using CodeImp.DoomBuilder.Config;

#endregion

namespace CodeImp.DoomBuilder.BuilderModes
{
	[FindReplace("Linedef Flags", BrowseButton = true, Replacable = false)]
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
		public override FindReplaceObject[] Find(string value, bool withinselection, string replacewith, bool keepselection)
		{
			List<FindReplaceObject> objs = new List<FindReplaceObject>();

			// Where to search?
			ICollection<Linedef> list = withinselection ? General.Map.Map.GetSelectedLinedefs(true) : General.Map.Map.Linedefs;

			// Go for all linedefs
			foreach (Linedef l in list)
			{
				bool match = true;

				// Parse the value string...
				foreach (string s in value.Split(','))
				{
					string str = s.Trim();

					// ... and check if the flags don't match
					if (General.Map.Config.LinedefFlags.ContainsKey(str) && !l.IsFlagSet(str))
					{
						match = false;
						break;
					}
				}

				// Flags matches?
				if (match)
				{
					// Add to list
					LinedefActionInfo info = General.Map.Config.GetLinedefActionInfo(l.Action);
					if (!info.IsNull)
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
