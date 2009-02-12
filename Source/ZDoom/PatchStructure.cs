
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
using CodeImp.DoomBuilder.Compilers;

#endregion

namespace CodeImp.DoomBuilder.ZDoom
{
	public sealed class PatchStructure
	{
		#region ================== Constants

		#endregion

		#region ================== Variables

		// Declaration
		private string name;
		private int offsetx;
		private int offsety;
		private bool flipx;
		private bool flipy;
		
		#endregion

		#region ================== Properties

		public string Name { get { return name; } }
		public int OffsetX { get { return offsetx; } }
		public int OffsetY { get { return offsety; } }
		public bool FlipX { get { return flipx; } }
		public bool FlipY { get { return flipy; } }

		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		internal PatchStructure(TexturesParser parser)
		{
			string tokenstr;
			
			// Initialize
			
			// There should be 3 tokens separated by 2 commas now:
			// Name, Width, Height

			// First token is the class name
			parser.SkipWhitespace(true);
			name = parser.ReadToken();
			if(string.IsNullOrEmpty(name))
			{
				parser.ReportError("Expected patch name");
				return;
			}

			// Now we should find a comma
			parser.SkipWhitespace(true);
			tokenstr = parser.ReadToken();
			if(tokenstr != ",")
			{
				parser.ReportError("Expected a comma");
				return;
			}

			// Next is the patch width
			parser.SkipWhitespace(true);
			tokenstr = parser.ReadToken();
			if(string.IsNullOrEmpty(tokenstr) || !int.TryParse(tokenstr, out offsetx))
			{
				parser.ReportError("Expected offset in pixels");
				return;
			}

			// Now we should find a comma again
			parser.SkipWhitespace(true);
			tokenstr = parser.ReadToken();
			if(tokenstr != ",")
			{
				parser.ReportError("Expected a comma");
				return;
			}

			// Next is the patch height
			parser.SkipWhitespace(true);
			tokenstr = parser.ReadToken();
			if(string.IsNullOrEmpty(tokenstr) || !int.TryParse(tokenstr, out offsety))
			{
				parser.ReportError("Expected offset in pixels");
				return;
			}

			// Next token should be the beginning of the texture scope
			parser.SkipWhitespace(true);
			tokenstr = parser.ReadToken();
			if(tokenstr != "{")
			{
				parser.ReportError("Expected begin of structure");
				return;
			}

			// Now parse the contents of texture structure
			while(parser.SkipWhitespace(true))
			{
				string token = parser.ReadToken();
				token = token.ToLowerInvariant();
				if(token == "flipx")
				{
					flipx = true;
				}
				else if(token == "flipy")
				{
					flipy = true;
				}
				else if(token == "}")
				{
					// Patch scope ends here,
					// break out of this parse loop
					break;
				}
			}
		}

		#endregion
	}
}
