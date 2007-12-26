
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

		#endregion

		#region ================== Properties

		public int Index { get { return index; } }
		public string Prefix { get { return prefix; } }
		public string Category { get { return category; } }
		public string Name { get { return name; } }
		public string Title { get { return title; } }
		public string[] ArgTitle { get { return argtitle; } }
		public TagType[] ArgTagType { get { return argtagtype; } }

		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		public LinedefActionInfo(int index, string desc)
		{
			string[] parts;
			int p = 0;
			
			// Initialize
			this.index = index;
			this.argtitle = new string[Linedef.NUM_ARGS];
			this.argtagtype = new TagType[Linedef.NUM_ARGS];
			
			// Find the parts by splitting on spaces
			parts = desc.Split(new char[] {' '}, 3);
			if(parts.Length >= 3) this.prefix = parts[p++]; else this.prefix = "";
			if(parts.Length >= 2) this.category = parts[p++]; else this.category = "Unknown";
			if(parts.Length >= 1) this.name = this.category + " " + parts[p++]; else this.name = this.category;
			this.name = this.name.Trim();
			this.title = this.prefix + " " + this.name;
			this.title = this.title.Trim();
			
			// No args/marks
			for(int i = 0; i < Linedef.NUM_ARGS; i++)
			{
				this.argtitle[i] = "";
				this.argtagtype[i] = TagType.None;
			}

			// We have no destructor
			GC.SuppressFinalize(this);
		}

		// Constructor
		public LinedefActionInfo(int index, Configuration cfg)
		{
			string desc;
			string[] parts;
			int p = 0;
			int argindex;
			
			// Initialize
			this.index = index;
			this.argtitle = new string[Linedef.NUM_ARGS];
			this.argtagtype = new TagType[Linedef.NUM_ARGS];
			
			// Read description
			desc = cfg.ReadSetting("linedeftypes." + index.ToString(CultureInfo.InvariantCulture) + ".title", "  Unknown");
			
			// Find the parts by splitting on spaces
			parts = desc.Split(new char[] { ' ' }, 3);
			if(parts.Length >= 3) this.prefix = parts[p++]; else this.prefix = "";
			if(parts.Length >= 2) this.category = parts[p++]; else this.category = "Unknown";
			if(parts.Length >= 1) this.name = this.category + " " + parts[p++]; else this.name = this.category;
			this.name = this.name.Trim();
			this.title = this.prefix + " " + this.name;
			this.title = this.title.Trim();
			
			// Read the args and marks
			for(int i = 0; i < Linedef.NUM_ARGS; i++)
			{
				argindex = i + 1;
				this.argtitle[i] = cfg.ReadSetting("linedeftypes." + index.ToString(CultureInfo.InvariantCulture) +
					".arg" + argindex.ToString(CultureInfo.InvariantCulture), "");
				this.argtagtype[i] = (TagType)cfg.ReadSetting("linedeftypes." + index.ToString(CultureInfo.InvariantCulture) +
					".mark" + argindex.ToString(CultureInfo.InvariantCulture), (int)TagType.None);
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
