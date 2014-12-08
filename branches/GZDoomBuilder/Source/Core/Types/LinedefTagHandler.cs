
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
using CodeImp.DoomBuilder.Config;
using CodeImp.DoomBuilder.Map;

#endregion

namespace CodeImp.DoomBuilder.Types
{
	[TypeHandler(UniversalType.LinedefTag, "Linedef Tag", true)]
	internal class LinedefTagHandler : SectorTagHandler
	{
		#region ================== Setup

		protected override EnumList CreateTagList()
		{
			//collect tags
			List<int> tags = new List<int>();
			EnumList taglist = new EnumList();

			foreach(Linedef t in General.Map.Map.Linedefs) 
			{
				if(t.Tag == 0 || tags.Contains(t.Tag)) continue;
				tags.Add(t.Tag);
			}

			//now sort them
			tags.Sort();

			//create enum items
			foreach(int tag in tags) 
			{
				if(General.Map.Options.TagLabels.ContainsKey(tag)) //tag labels
					taglist.Add(new EnumItem(tag.ToString(), General.Map.Options.TagLabels[tag]));
				else
					taglist.Add(new EnumItem(tag.ToString(), tag.ToString()));
			}

			return taglist;
		}

		#endregion
	}
}
