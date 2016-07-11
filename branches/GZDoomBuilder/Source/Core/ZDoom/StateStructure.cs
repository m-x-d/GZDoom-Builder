
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
using System.IO;

#endregion

namespace CodeImp.DoomBuilder.ZDoom
{
	public sealed class StateStructure
	{
		#region ================== FrameInfo (mxd)

		public class FrameInfo
		{
			public string Sprite;
			public string LightName;
			public bool Bright;
		}
		
		#endregion

		#region ================== Variables
		
		// All we care about is the first sprite in the sequence
		private readonly List<FrameInfo> sprites;
		private readonly StateGoto gotostate;
		private readonly DecorateParser parser;
		
		#endregion

		#region ================== Properties

		public int SpritesCount { get { return sprites.Count; } }

		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		internal StateStructure(ActorStructure actor, DecorateParser parser)
		{
			string lasttoken = "";
			
			this.gotostate = null;
			this.parser = parser;
			this.sprites = new List<FrameInfo>();
			
			// Skip whitespace
			while(parser.SkipWhitespace(true))
			{
				// Read first token
				string token = parser.ReadToken().ToLowerInvariant();
				
				// One of the flow control statements?
				if((token == "loop") || (token == "stop") || (token == "wait") || (token == "fail"))
				{
					// Ignore flow control
				}
				// Goto?
				else if(token == "goto")
				{
					gotostate = new StateGoto(actor, parser);
					if(parser.HasError) return;
				}
				// Label?
				else if(token == ":")
				{
					// Rewind so that this label can be read again
					if(!string.IsNullOrEmpty(lasttoken)) 
						parser.DataStream.Seek(-(lasttoken.Length + 1), SeekOrigin.Current);
					
					// Done here
					return;
				}
				//mxd. Start of inner scope?
				else if(token == "{")
				{
					int bracelevel = 1;
					while(!string.IsNullOrEmpty(token) && bracelevel > 0)
					{
						parser.SkipWhitespace(false);
						token = parser.ReadToken();
						switch(token)
						{
							case "{": bracelevel++; break;
							case "}": bracelevel--; break;
						}
					}
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
					token = parser.StripTokenQuotes(token); //mxd. First part of the sprite name can be quoted
					if(string.IsNullOrEmpty(token)) 
					{
						parser.ReportError("Expected sprite name");
						return;
					}
					
					// Frames of the sprite name
					parser.SkipWhitespace(true);
					string spriteframes = parser.StripTokenQuotes(parser.ReadToken()); //mxd. Frames can be quoted
					if(string.IsNullOrEmpty(spriteframes))
					{
						parser.ReportError("Expected sprite frame");
						return;
					}
					
					// Label?
					if(spriteframes == ":")
					{
						// Rewind so that this label can be read again
						parser.DataStream.Seek(-(token.Length + 1), SeekOrigin.Current);

						// Done here
						return;
					}
					
					// No first sprite yet?
					FrameInfo info = new FrameInfo(); //mxd
					if(spriteframes.Length > 0)
					{
						//mxd. I'm not even 50% sure the parser handles all bizzare cases without shifting sprite name / frame blocks,
						// so let's log it as a warning, not an error...
						if(token.Length != 4)
						{
							parser.LogWarning("Invalid sprite name \"" + token.ToUpperInvariant() + "\". Sprite names must be exactly 4 characters long");
						}
						else
						{
							// Make the sprite name
							string spritename = (token + spriteframes[0]).ToUpperInvariant();

							// Ignore some odd ZDoom things
							if(!spritename.StartsWith("TNT1") && !spritename.StartsWith("----") && !spritename.Contains("#"))
							{
								info.Sprite = spritename; //mxd
								sprites.Add(info);
							}
						}
					}
					
					// Continue until the end of the line
					parser.SkipWhitespace(false);
					string t = parser.ReadToken();
					while(!string.IsNullOrEmpty(t) && t != "\n")
					{
						//mxd. Bright keyword support...
						if(t == "bright")
						{
							info.Bright = true;
						}
						//mxd. Light() expression support...
						else if(t == "light")
						{
							if(!parser.NextTokenIs("(")) return;

							if(!parser.SkipWhitespace(true))
							{
								parser.ReportError("Unexpected end of the structure");
								return;
							}
							
							info.LightName = parser.StripTokenQuotes(parser.ReadToken());
							if(string.IsNullOrEmpty(info.LightName))
							{
								parser.ReportError("Expected dynamic light name");
								return;
							}

							if(!parser.SkipWhitespace(true))
							{
								parser.ReportError("Unexpected end of the structure");
								return;
							}

							if(!parser.NextTokenIs(")")) return;
						}
						//mxd. Inner scope start. Step back and reparse using parent loop
						else if(t == "{")
						{
							// Rewind so that this scope end can be read again
							parser.DataStream.Seek(-1, SeekOrigin.Current);

							// Break out of this loop
							break;
						}
						//mxd. Function params start (those can span multiple lines)
						else if(t == "(")
						{
							int bracelevel = 1;
							while(!string.IsNullOrEmpty(token) && bracelevel > 0)
							{
								parser.SkipWhitespace(true);
								token = parser.ReadToken();
								switch(token)
								{
									case "(": bracelevel++; break;
									case ")": bracelevel--; break;
								}
							}
						}
						//mxd. Because stuff like this is also valid: "Actor Oneliner { States { Spawn: WOOT A 1 A_FadeOut(0.1) Loop }}"
						else if(t == "}")
						{
							// Rewind so that this scope end can be read again
							parser.DataStream.Seek(-1, SeekOrigin.Current);

							// Done here
							return;
						}

						// Read next token
						parser.SkipWhitespace(false);
						t = parser.ReadToken().ToLowerInvariant();
					}
				}
				
				lasttoken = token;
			}
		}

		//mxd
		internal StateStructure(string spritename) 
		{
			this.gotostate = null;
			this.sprites = new List<FrameInfo> { new FrameInfo { Sprite = spritename } };
		}

		#endregion

		#region ================== Methods
		
		// This finds the first valid sprite and returns it
		public FrameInfo GetSprite(int index)
		{
			return GetSprite(index, new HashSet<StateStructure>());
		}
		
		// This version of GetSprite uses a callstack to check if it isn't going into an endless loop
		private FrameInfo GetSprite(int index, HashSet<StateStructure> prevstates)
		{
			// If we have sprite of our own, see if we can return this index
			if(index < sprites.Count) return sprites[index];
			
			// Otherwise, continue searching where goto tells us to go
			if(gotostate != null)
			{
				// Find the class
				ActorStructure a = parser.GetArchivedActorByName(gotostate.ClassName);
				if(a != null)
				{
					StateStructure s = a.GetState(gotostate.StateName);
					if((s != null) && !prevstates.Contains(s))
					{
						prevstates.Add(this);
						return s.GetSprite(gotostate.SpriteOffset, prevstates);
					}
				}
			}
			
			// If there is no goto keyword used, just give us one of our sprites if we can
			if(sprites.Count > 0)
			{
				// The following behavior should really depend on the flow control keyword (loop or stop) but who cares.
				return sprites[0];
			}
			
			return new FrameInfo();
		}
		
		#endregion
	}
}
