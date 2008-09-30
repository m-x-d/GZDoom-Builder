
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
	internal abstract class TextureSet
	{
		#region ================== Variables

		protected string name;
		protected List<string> filters;
		
		// Never stored, only used at run-time
		protected Dictionary<long, ImageData> matches;
		
		#endregion
		
		#region ================== Properties
		
		public string Name { get { return name; } set { name = value; } }
		internal List<string> Filters { get { return filters; } }
		
		#endregion
		
		#region ================== Constructor / Destructor
		
		public TextureSet()
		{
			this.name = "Unnamed Set";
			this.filters = new List<string>();
		}
		
		#endregion
		
		#region ================== Methods
		
		// This writes the texture set to configuration
		internal abstract void WriteToConfig(Configuration cfg, string path);
		
		// This resets the matches and recreates the regex
		internal virtual void Reset()
		{
			// Clear matches
			matches = new Dictionary<long, ImageData>();
		}
		
		// This adds a texture to this set
		internal virtual bool Add(ImageData image)
		{
			// Can we add it?
			if(!matches.ContainsKey(image.LongName))
			{
				// Add it
				matches.Add(image.LongName, image);
				return true;
			}
			else
			{
				// Can't add it
				return false;
			}
		}
		
		// This tests if a texture is in this texturset
		public virtual bool Exists(long longname)
		{
			return matches.ContainsKey(longname);
		}
		
		// This returns the name
		public override string ToString()
		{
			return name;
		}
		
		#endregion
	}
}
