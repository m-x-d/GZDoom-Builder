
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
	internal sealed class StateStructure
	{
		#region ================== Constants
		
		// Some odd thing in ZDoom
		private const string IGNORE_SPRITE = "TNT1A0";
		
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
		internal StateStructure(ActorStructure actor, DecorateParser parser, string statename)
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
				if((token == "loop") || (token == "stop") || (token == "wait") || (token == "fail") || (token == "goto"))
				{
					// Ignore flow control
				}
				/*
				// Read some other state sprites?
				else if(token == "goto")
				{
					string firsttarget = "";
					string secondtarget = "";
					bool commentreached = false;
					bool offsetreached = false;
					string offsetstr = "";
					int cindex = 0;
					
					// This is a bitch to parse because for some bizarre reason someone thought it
					// was funny to allow quotes here. Read the whole line and start parsing this manually.
					string line = parser.ReadLine();
					
					// Skip whitespace
					while((cindex < line.Length) && ((line[cindex] == ' ') || (line[cindex] == '\t')))
						cindex++;
					
					// Parse first target
					while((cindex < line.Length) && (line[cindex] != ':'))
					{
						// When a comment is reached, we're done here
						if(line[cindex] == '/')
						{
							if((cindex + 1 < line.Length) && ((line[cindex + 1] == '/') || (line[cindex + 1] == '*')))
							{
								commentreached = true;
								break;
							}
						}
						
						// Whitespace ends the string
						if((line[cindex] == ' ') || (line[cindex] == '\t'))
							break;
						
						// + sign indicates offset start
						if(line[cindex] == '+')
						{
							cindex++;
							offsetreached = true;
							break;
						}
						
						// Ignore quotes
						if(line[cindex] != '"')
							firsttarget += line[cindex];
						
						cindex++;
					}
					
					if(!commentreached && !offsetreached)
					{
						// Skip whitespace
						while((cindex < line.Length) && ((line[cindex] == ' ') || (line[cindex] == '\t')))
							cindex++;
						
						// Parse second target
						while(cindex < line.Length)
						{
							// When a comment is reached, we're done here
							if(line[cindex] == '/')
							{
								if((cindex + 1 < line.Length) && ((line[cindex + 1] == '/') || (line[cindex + 1] == '*')))
								{
									commentreached = true;
									break;
								}
							}
							
							// Whitespace ends the string
							if((line[cindex] == ' ') || (line[cindex] == '\t'))
								break;
							
							// + sign indicates offset start
							if(line[cindex] == '+')
							{
								cindex++;
								offsetreached = true;
								break;
							}

							// Ignore quotes and semicolons
							if((line[cindex] != '"') && (line[cindex] != ':'))
								secondtarget += line[cindex];
								
							cindex++;
						}
					}
					
					// Try to find the offset if we still haven't found it yet
					if(!offsetreached)
					{
						// Skip whitespace
						while((cindex < line.Length) && ((line[cindex] == ' ') || (line[cindex] == '\t')))
							cindex++;
						
						if((cindex < line.Length) && (line[cindex] == '+'))
						{
							cindex++;
							offsetreached = true;
						}
					}
					
					if(offsetreached)
					{
						// Parse offset
						while(cindex < line.Length)
						{
							// When a comment is reached, we're done here
							if(line[cindex] == '/')
							{
								if((cindex + 1 < line.Length) && ((line[cindex + 1] == '/') || (line[cindex + 1] == '*')))
								{
									commentreached = true;
									break;
								}
							}
							
							// Whitespace ends the string
							if((line[cindex] == ' ') || (line[cindex] == '\t'))
								break;
							
							// Ignore quotes and semicolons
							if((line[cindex] != '"') && (line[cindex] != ':'))
								offsetstr += line[cindex];
							
							cindex++;
						}
					}
					
					// We should now have a first target, optionally a second target and optionally a sprite offset
					
					// Check if we don't have the class specified
					if(string.IsNullOrEmpty(secondtarget))
					{
						// First target is the state to go to
						// TODO: Make this happen?
					}
					else
					{
						// First target is the base class to use
						// Second target is the state to go to
						// TODO: Make this happen?
					}
				}
				*/
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
					// Rewind so that this scope end can be read again
					parser.DataStream.Seek(-1, SeekOrigin.Current);

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
					// Label?
					else if(spriteframes == ":")
					{
						// Rewind so that this label can be read again
						parser.DataStream.Seek(-(token.Length + 1), SeekOrigin.Current);

						// Done here
						return;
					}
					
					// No first sprite yet?
					if((firstsprite == null) && (spriteframes.Length > 0))
					{
						// Make the sprite name
						firstsprite = token + spriteframes[0];
						firstsprite = firstsprite.ToUpperInvariant();
						
						// Ignore some odd ZDoom thing
						if(IGNORE_SPRITE.StartsWith(firstsprite))
							firstsprite = null;
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
