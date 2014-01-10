
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
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Config;

#endregion

namespace CodeImp.DoomBuilder.BuilderModes
{
	[FindReplace("Sector Tags", BrowseButton = false)]
	internal class FindSectorTags : BaseFindSector
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

			// Interpret the replacement
			int replacetag = 0;
			if(replacewith != null)
			{
				// If it cannot be interpreted, set replacewith to null (not replacing at all)
				if(!int.TryParse(replacewith, out replacetag)) replacewith = null;
				if(replacetag < General.Map.FormatInterface.MinTag) replacewith = null;
				if(replacetag > General.Map.FormatInterface.MaxTag) replacewith = null;
				if(replacewith == null)
				{
					MessageBox.Show("Invalid replace value for this search type!", "Find and Replace", MessageBoxButtons.OK, MessageBoxIcon.Error);
					return objs.ToArray();
				}
			}
			
			// Interpret the number given
			int tag = 0;
			if(int.TryParse(value, out tag))
			{
				// Where to search?
				ICollection<Sector> list = withinselection ? General.Map.Map.GetSelectedSectors(true) : General.Map.Map.Sectors;

				// Go for all sectors
				foreach(Sector s in list)
				{
					// Tag matches?
					if(s.Tag == tag)
					{
						// Replace
						if(replacewith != null) s.Tag = replacetag;
						
						SectorEffectInfo info = General.Map.Config.GetSectorEffectInfo(s.Effect);
						if(!info.IsNull)
							objs.Add(new FindReplaceObject(s, "Sector " + s.Index + " (" + info.Title + ")"));
						else
							objs.Add(new FindReplaceObject(s, "Sector " + s.Index));
					}
				}
			}
			
			return objs.ToArray();
		}
		
		#endregion
	}
}
