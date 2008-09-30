
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
using System.Text.RegularExpressions;
using System.Collections.Specialized;

#endregion

namespace CodeImp.DoomBuilder.Config
{
	internal class DefinedTextureSet : TextureSet
	{
		#region ================== Variables
		
		// Never stored, only used at run-time
		private Regex regex;
		
		#endregion
		
		#region ================== Constructor / Destructor
		
		// Texture set from configuration
		public DefinedTextureSet(Configuration cfg, string path)
		{
			// Read the name
			name = cfg.ReadSetting(path + ".name", "Unnamed Set");
			
			// Read the filters
			IDictionary dic = cfg.ReadSetting(path, new Hashtable());
			filters = new List<string>(dic.Count);
			foreach(DictionaryEntry de in dic)
			{
				// If not the name of this texture set, add value as filter
				if(de.Key.ToString() != "name") filters.Add(de.Value.ToString());
			}
		}
		
		// New texture set constructor
		public DefinedTextureSet(string name)
		{
			this.name = name;
			this.filters = new List<string>();
		}
		
		#endregion
		
		#region ================== Methods
		
		// This writes the texture set to configuration
		internal override void WriteToConfig(Configuration cfg, string path)
		{
			IDictionary dic;
			
			// Fill structure
			dic = new ListDictionary();
			
			// Add name
			dic.Add("name", name);
			
			for(int i = 0; i < filters.Count; i++)
			{
				// Add filters
				dic.Add(i.ToString(), filters[i]);
			}
			
			// Write to config
			cfg.WriteSetting(path, dic);
		}
		
		// This resets the matches and recreates the regex
		internal override void Reset()
		{
			base.Reset();
			
			// Make the regex string that handles all filters
			StringBuilder regexstr = new StringBuilder("");
			foreach(string s in filters)
			{
				// Replace the * with the regex code
				string ss = s.Replace("*", ".*?");

				// Escape other regex characters, except the ?
				ss = ss.Replace("+", "\\+");
				ss = ss.Replace("\\", "\\\\");
				ss = ss.Replace("|", "\\|");
				ss = ss.Replace("{", "\\{");
				ss = ss.Replace("[", "\\[");
				ss = ss.Replace("(", "\\(");
				ss = ss.Replace(")", "\\)");
				ss = ss.Replace("^", "\\^");
				ss = ss.Replace("$", "\\$");
				ss = ss.Replace(".", "\\.");
				ss = ss.Replace("#", "\\#");
				ss = ss.Replace(" ", "\\ ");

				// When a filter has already added, insert a conditional OR operator
				if(regexstr.Length > 0) regexstr.Append("|");

				// Open group without backreferencing
				regexstr.Append("(?:");

				// Add the filter
				regexstr.Append(ss);

				// Close group
				regexstr.Append(")");
			}

			// Make the regex
			regex = new Regex(regexstr.ToString(), RegexOptions.Compiled |
												   RegexOptions.CultureInvariant |
												   RegexOptions.IgnoreCase);
		}
		
		// This matches a name against the regex and adds a texture to
		// the list if it matches. Returns true when matched and added.
		internal virtual bool Add(ImageData image)
		{
			// Check against regex
			if(regex.IsMatch(image.Name))
			{
				// Matches! Add it.
				return base.Add(image);
			}
			else
			{
				// Doesn't match
				return false;
			}
		}
		
		#endregion
	}
}
