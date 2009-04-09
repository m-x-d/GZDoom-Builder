
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
	public sealed class ActorStructure
	{
		#region ================== Constants
		
		private readonly string[] SPRITE_POSTFIXES = new string[] {"2C8", "2D8", "2A8", "2B8", "1C1", "1D1", "1A1", "1B1", "A2", "A1", "A0", "2", "1", "0" };
		
		#endregion
		
		#region ================== Variables
		
		// Declaration
		private string classname;
		private string inheritclass;
		private string replaceclass;
		private List<string> games;
		private int doomednum = -1;
		
		// Flags
		private Dictionary<string, bool> flags;
		
		// Properties
		// We only parse the properties we know about
		// because this format doesn't allow parsing in a generic way
		// (properties can have zero, one or two values and there is
		// nothing that tells you if it is a value or another property)
		private int radius;
		private int height;
		private bool radiusfound;
		private bool heightfound;
		private string tag;
		private string category;
		private string sprite;
		
		// States
		private Dictionary<string, StateStructure> states;
		
		#endregion
		
		#region ================== Properties
		
		public Dictionary<string, bool> Flags { get { return flags; } }
		public string ClassName { get { return classname; } }
		public string InheritsClass { get { return inheritclass; } }
		public string ReplacesClass { get { return replaceclass; } }
		public List<string> Games { get { return games; } }
		public int DoomEdNum { get { return doomednum; } }
		public int Radius { get { return radius; } }
		public int Height { get { return height; } }
		public bool RadiusFound { get { return radiusfound; } }
		public bool HeightFound { get { return heightfound; } }
		public string Tag { get { return tag; } }
		public string Category { get { return category; } }
		
		#endregion
		
		#region ================== Constructor / Disposer
		
		// Constructor
		internal ActorStructure(DecorateParser parser)
		{
			// Initialize
			flags = new Dictionary<string, bool>();
			states = new Dictionary<string, StateStructure>();
			inheritclass = "actor";
			replaceclass = null;
			category = "Decorate";
			sprite = null;
			games = new List<string>();
			radius = 10;
			height = 20;
			tag = null;
			
			// First next token is the class name
			parser.SkipWhitespace(true);
			classname = parser.StripTokenQuotes(parser.ReadToken());
			if(string.IsNullOrEmpty(classname))
			{
				parser.ReportError("Expected actor class name");
				return;
			}

			// Parse tokens before entering the actor scope
			while(parser.SkipWhitespace(true))
			{
				string token = parser.ReadToken();
				if(!string.IsNullOrEmpty(token))
				{
					token = token.ToLowerInvariant();
					if(token == ":")
					{
						// The next token must be the class to inherit from
						parser.SkipWhitespace(true);
						inheritclass = parser.StripTokenQuotes(parser.ReadToken());
						if(string.IsNullOrEmpty(inheritclass) || parser.IsSpecialToken(inheritclass))
						{
							parser.ReportError("Expected class name to inherit from");
							return;
						}
						else
						{
							// Find the actor to inherit from
							ActorStructure other = parser.GetArchivedActorByName(inheritclass);
							if(other != null)
								InheritFrom(other);
							else
								General.ErrorLogger.Add(ErrorType.Warning, "Unable to find the DECORATE class '" + inheritclass + "' to inherit from, while parsing '" + classname + "'");
						}
					}
					else if(token == "replaces")
					{
						// The next token must be the class to replace
						parser.SkipWhitespace(true);
						replaceclass = parser.StripTokenQuotes(parser.ReadToken());
						if(string.IsNullOrEmpty(replaceclass) || parser.IsSpecialToken(replaceclass))
						{
							parser.ReportError("Expected class name to replace");
							return;
						}
					}
					else if(token == "native")
					{
						// Igore this token
					}
					else if(token == "{")
					{
						// Actor scope begins here,
						// break out of this parse loop
						break;
					}
					else
					{
						// Check if numeric
						if(!int.TryParse(token, out doomednum))
						{
							// Not numeric!
							parser.ReportError("Expected numeric editor thing number or start of actor scope");
							return;
						}
					}
				}
				else
				{
					parser.ReportError("Unexpected end of structure");
					return;
				}
			}
			
			// Now parse the contents of actor structure
			string previoustoken = "";
			while(parser.SkipWhitespace(true))
			{
				string token = parser.ReadToken();
				token = token.ToLowerInvariant();
				if((token == "+") || (token == "-"))
				{
					// Next token is a flag (option) to set or remove
					bool flagvalue = (token == "+");
					parser.SkipWhitespace(true);
					string flagname = parser.ReadToken();
					if(!string.IsNullOrEmpty(flagname))
					{
						// Add the flag with its value
						flagname = flagname.ToLowerInvariant();
						flags[flagname] = flagvalue;
					}
					else
					{
						parser.ReportError("Expected flag name");
						return;
					}
				}
				else if((token == "action") || (token == "native"))
				{
					// We don't need this, ignore up to the first next ;
					while(parser.SkipWhitespace(true))
					{
						string t = parser.ReadToken();
						if((t == ";") || (t == null)) break;
					}
				}
				else if(token == "states")
				{
					// Now parse actor states until we reach the end of the states structure
					while(parser.SkipWhitespace(true))
					{
						string statetoken = parser.ReadToken();
						if(!string.IsNullOrEmpty(statetoken))
						{
							// Start of scope?
							if(statetoken == "{")
							{
								// This is fine
							}
							// End of scope?
							else if(statetoken == "}")
							{
								// Done with the states,
								// break out of this parse loop
								break;
							}
							// State label?
							else if(statetoken == ":")
							{
								if(!string.IsNullOrEmpty(previoustoken))
								{
									// Parse actor state
									StateStructure st = new StateStructure(parser, previoustoken);
									if(parser.HasError) return;
									states[previoustoken.ToLowerInvariant()] = st;
								}
								else
								{
									parser.ReportError("Unexpected end of structure");
									return;
								}
							}
							else
							{
								// Keep token
								previoustoken = statetoken;
							}
						}
						else
						{
							parser.ReportError("Unexpected end of structure");
							return;
						}
					}
				}
				else if(token == "}")
				{
					// Actor scope ends here,
					// break out of this parse loop
					break;
				}
				// Known property with a single value?
				else if((token == "radius") || (token == "height") || (token == "tag"))
				{
					// Next token is the property value to set
					parser.SkipWhitespace(true);
					string value = parser.ReadToken();
					if(!string.IsNullOrEmpty(value))
					{
						// Try parsing as integer value
						int intvalue;
						int.TryParse(value, out intvalue);
						
						// Set the property
						switch(token)
						{
							case "radius": radius = intvalue; radiusfound = true;  break;
							case "height": height = intvalue; heightfound = true; break;
							case "tag": tag = value; break;
						}
					}
					else
					{
						// Can't find the property value!
						parser.ReportError("Expected a value for property '" + token + "'");
						return;
					}
				}
				// Monster property?
				else if(token == "monster")
				{
					// This sets certain flags we are interested in
					flags["solid"] = true;
				}
				// Game property?
				else if(token == "game")
				{
					// Include all tokens on the same line
					games.Clear();
					while(parser.SkipWhitespace(false))
					{
						string v = parser.ReadToken();
						if(v == null)
						{
							parser.ReportError("Unexpected end of structure");
							return;
						}
						if(v == "\n") break;
						games.Add(v.ToLowerInvariant());
					}
				}
				// Sprite property?
				else if(token == "$sprite")
				{
					// The rest of the line is the sprite name
					if(parser.SkipWhitespace(false))
						sprite = parser.ReadLine();
				}
				// Category property?
				else if(token == "$category")
				{
					// The rest of the line is the category name
					if(parser.SkipWhitespace(false))
						category = parser.ReadLine();
				}
				
				// Keep token
				previoustoken = token;
			}
		}
		
		#endregion
		
		#region ================== Methods
		
		// This is called to inherit properties from another actor
		private void InheritFrom(ActorStructure baseactor)
		{
			this.flags = new Dictionary<string, bool>(baseactor.flags);
			this.height = baseactor.height;
			this.radius = baseactor.radius;
			this.games = new List<string>(baseactor.games);
			this.states = new Dictionary<string, StateStructure>(baseactor.states);
		}

		// This returns the status of a flag
		public bool HasFlagValue(string flag)
		{
			return flags.ContainsKey(flag);
		}
		
		// This returns the status of a flag
		public bool GetFlagValue(string flag, bool defaultvalue)
		{
			if(flags.ContainsKey(flag))
				return flags[flag];
			else
				return defaultvalue;
		}
		
		// This checks if this actor is meant for the current decorate game support
		public bool CheckActorSupported()
		{
			// Check if we want to include this actor
			string includegames = General.Map.Config.DecorateGames.ToLowerInvariant();
			bool includeactor = (games.Count == 0);
			foreach(string g in games)
				includeactor |= includegames.Contains(g);
			
			return includeactor;
		}
		
		// This finds the best suitable sprite to use
		public string FindSuitableSprite()
		{
			string sprite = "";
			
			// Sprite forced?
			if(!string.IsNullOrEmpty(sprite))
			{
				return sprite;
			}
			else
			{
				// Try the idle state
				if(states.ContainsKey("idle"))
				{
					StateStructure s = states["idle"];
					if(!string.IsNullOrEmpty(s.FirstSprite))
						sprite = s.FirstSprite;
				}
				
				// Try the see state
				if(string.IsNullOrEmpty(sprite) && states.ContainsKey("see"))
				{
					StateStructure s = states["see"];
					if(!string.IsNullOrEmpty(s.FirstSprite))
						sprite = s.FirstSprite;
				}

				// Try the inactive state
				if(string.IsNullOrEmpty(sprite) && states.ContainsKey("inactive"))
				{
					StateStructure s = states["inactive"];
					if(!string.IsNullOrEmpty(s.FirstSprite))
						sprite = s.FirstSprite;
				}
				
				// Still no sprite found? then just pick the first we can find
				if(string.IsNullOrEmpty(sprite))
				{
					foreach(StateStructure s in states.Values)
					{
						if(!string.IsNullOrEmpty(s.FirstSprite))
						{
							sprite = s.FirstSprite;
							break;
						}
					}
				}
				
				if(!string.IsNullOrEmpty(sprite))
				{
					// The sprite name is not actually complete, we still have to append
					// the direction characters to it. Find an existing sprite with direction.
					foreach(string postfix in SPRITE_POSTFIXES)
					{
						if(General.Map.Data.GetSpriteExists(sprite + postfix))
							return sprite + postfix;
					}
				}
			}
			
			// No sprite found
			return "";
		}
		
		#endregion
	}
}
