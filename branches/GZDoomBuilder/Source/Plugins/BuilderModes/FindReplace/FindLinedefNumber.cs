
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
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Config;

#endregion

namespace CodeImp.DoomBuilder.BuilderModes
{
	[FindReplace("Linedef Index", BrowseButton = false, Replacable = false)]
	internal class FindLinedefNumber : BaseFindLinedef
	{
		#region ================== Constants

		#endregion

		#region ================== Variables

		#endregion

		#region ================== Properties

		#endregion

		#region ================== Constructor / Destructor

		#endregion

		#region ================== Methods

		// This is called to perform a search (and replace)
		// Returns a list of items to show in the results list
		// replacewith is null when not replacing
		public override FindReplaceObject[] Find(string value, bool withinselection, string replacewith, bool keepselection)
		{
			List<FindReplaceObject> objs = new List<FindReplaceObject>();

			// Interpret the number given
			int index = 0;
			if(int.TryParse(value, out index))
			{
				Linedef l = General.Map.Map.GetLinedefByIndex(index);
				if(l != null)
				{
					LinedefActionInfo info = General.Map.Config.GetLinedefActionInfo(l.Action);
					if(!info.IsNull)
						objs.Add(new FindReplaceObject(l, "Linedef " + index + " (" + info.Title + ")"));
					else
						objs.Add(new FindReplaceObject(l, "Linedef " + index));
				}
			}
			
			return objs.ToArray();
		}

		#endregion
	}
}
