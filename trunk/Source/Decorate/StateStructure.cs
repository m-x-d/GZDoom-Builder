
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

namespace CodeImp.DoomBuilder.Decorate
{
	public sealed class StateStructure
	{
		#region ================== Constants

		#endregion

		#region ================== Variables
		
		// All we care about is the first sprite in the sequence
		private string firstsprite;
		
		#endregion

		#region ================== Properties
		
		public string FirstSprite { get { return firstsprite; } }
		
		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		internal StateStructure(DecorateParser parser)
		{
			string lasttoken = "";
			firstsprite = null;
			
			// Skip whitespace
			while(parser.SkipWhitespace(true))
			{
				// Read first token
				string token = parser.ReadToken();
				token = token.ToLowerInvariant();
				
				// One of the flow control statements?
				if((token == "loop") || (token == "stop") || (token == "wait") || (token == "fail"))
				{
					// Then we leave
					break;
				}
				// Label?
				else if(token == ":")
				{
					// Rewind so that this label can be read again
					parser.DataStream.Seek(-(lasttoken.Length + 1), SeekOrigin.Current);
					
					// Done here
					return;
				}
				// End of scope?
				else if(token == "}")
				{
					// Done here
					return;
				}
				else
				{
					// First part of the sprite name
					if(token == null)
					{
						parser.ReportError("Unexpected end of structure");
						return;
					}
					
					// Frames of the sprite name
					parser.SkipWhitespace(true);
					string spriteframes = parser.ReadToken();
					if(spriteframes == null)
					{
						parser.ReportError("Unexpected end of structure");
						return;
					}
					
					// No first sprite yet?
					if((firstsprite == null) && (spriteframes.Length > 0))
					{
						// Make the sprite name
						firstsprite = token + spriteframes[0];
						firstsprite = firstsprite.ToUpperInvariant();
					}
					
					// Continue until the end of the line
					string t = "";
					while((t != "\n") && (t != null))
					{
						parser.SkipWhitespace(false);
						t = parser.ReadToken();
					}
				}
				
				lasttoken = token;
			}
		}

		#endregion

		#region ================== Methods

		#endregion
	}
}
