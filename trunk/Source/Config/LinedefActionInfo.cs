
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
	public class LinedefActionInfo : INumberedTitle, IComparable<LinedefActionInfo>
	{
		#region ================== Constants

		#endregion

		#region ================== Variables

		// Properties
		private int index;
		private string prefix;
		private string category;
		private string name;
		private string title;
		private string[] argtitle;
		private TagType[] argtagtype;
		private bool[] argused;
		private string[] argenum;
		
		#endregion

		#region ================== Properties

		public int Index { get { return index; } }
		public string Prefix { get { return prefix; } }
		public string Category { get { return category; } }
		public string Name { get { return name; } }
		public string Title { get { return title; } }
		public string[] ArgTitle { get { return argtitle; } }
		public TagType[] ArgTagType { get { return argtagtype; } }
		public bool[] ArgUsed { get { return argused; } }
		public string[] ArgEnum { get { return argenum; } }

		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		internal LinedefActionInfo(int index, Configuration cfg, string categoryname, IDictionary<string, EnumList> enums)
		{
			string actionsetting = "linedeftypes." + categoryname + "." + index.ToString(CultureInfo.InvariantCulture);
			
			// Initialize
			this.index = index;
			this.category = categoryname;
			this.argtitle = new string[Linedef.NUM_ARGS];
			this.argtagtype = new TagType[Linedef.NUM_ARGS];
			this.argused = new bool[Linedef.NUM_ARGS];
			this.argenum = new string[Linedef.NUM_ARGS];

			// Read settings
			this.name = cfg.ReadSetting(actionsetting + ".title", "Unnamed");
			this.prefix = cfg.ReadSetting(actionsetting + ".prefix", "");
			this.title = this.prefix + " " + this.name;
			this.title = this.title.Trim();

			// Read the args and marks
			for(int i = 0; i < Linedef.NUM_ARGS; i++)
			{
				// Read
				string istr = i.ToString(CultureInfo.InvariantCulture);
				this.argused[i] = cfg.SettingExists(actionsetting + ".arg" + istr);
				this.argtitle[i] = cfg.ReadSetting(actionsetting + ".arg" + istr + ".title", "Argument " + (i + 1));
				this.argtagtype[i] = (TagType)cfg.ReadSetting(actionsetting + ".arg" + istr + ".tag", (int)TagType.None);
				this.argenum[i] = cfg.ReadSetting(actionsetting + ".arg" + istr + ".enum", "");

				// Verify enums
				if((this.argenum[i].Length > 0) && !enums.ContainsKey(this.argenum[i]))
				{
					General.WriteLogLine("WARNING: Linedef type enumeration '" + this.argenum[i] + "' does not exist! (found on linedef type " + index + ")");
					this.argenum[i] = "";
				}
			}
			
			// We have no destructor
			GC.SuppressFinalize(this);
		}

		#endregion

		#region ================== Methods

		// This presents the item as string
		public override string ToString()
		{
			return index + " - " + title;
		}

		// This compares against another action info
		public int CompareTo(LinedefActionInfo other)
		{
			if(this.index < other.index) return -1;
			else if(this.index > other.index) return 1;
			else return 0;
		}

		#endregion
	}
}
