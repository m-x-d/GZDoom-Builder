
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
using System.Collections.Generic;
using System.Globalization;
using CodeImp.DoomBuilder.Config;
using CodeImp.DoomBuilder.Data;
using CodeImp.DoomBuilder.Types;

#endregion

namespace CodeImp.DoomBuilder.ZDoom
{
	public sealed class ActorStructure
	{
		#region ================== Constants
		
		private readonly string[] SPRITE_POSTFIXES = new[] {"2C8", "2D8", "2A8", "2B8", "1C1", "1D1", "1A1", "1B1", "A2", "A1", "A0", "2", "1", "0" };
		internal const string ACTOR_CLASS_SPECIAL_TOKENS = ":{}\n;,"; //mxd

		#endregion
		
		#region ================== Variables
		
		// Declaration
		private readonly string classname;
		private readonly string inheritclass;
		private readonly string replaceclass;
		private int doomednum = -1;
		
		// Inheriting
		private ActorStructure baseclass;
		private readonly bool skipsuper;
		
		// Flags
		private Dictionary<string, bool> flags;
		
		// Properties
		private Dictionary<string, List<string>> props;
		private readonly Dictionary<string, UniversalType> uservars; //mxd
		
		// States
		private Dictionary<string, StateStructure> states;
		
		#endregion
		
		#region ================== Properties
		
		public Dictionary<string, bool> Flags { get { return flags; } }
		public Dictionary<string, List<string>> Properties { get { return props; } }
		public string ClassName { get { return classname; } }
		public string InheritsClass { get { return inheritclass; } }
		public string ReplacesClass { get { return replaceclass; } }
		public ActorStructure BaseClass { get { return baseclass; } }
		internal int DoomEdNum { get { return doomednum; } set { doomednum = value; } }
		public Dictionary<string, UniversalType> UserVars { get { return uservars; } } //mxd
		
		#endregion
		
		#region ================== Constructor / Disposer
		
		// Constructor
		internal ActorStructure(DecorateParser parser)
		{
			// Initialize
			flags = new Dictionary<string, bool>(StringComparer.OrdinalIgnoreCase);
			props = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);
			states = new Dictionary<string, StateStructure>(StringComparer.OrdinalIgnoreCase);
			uservars = new Dictionary<string, UniversalType>(StringComparer.OrdinalIgnoreCase);//mxd
			bool done = false; //mxd
			
			// Always define a game property, but default to 0 values
			props["game"] = new List<string>();
			
			inheritclass = "actor";
			replaceclass = null;
			baseclass = null;
			skipsuper = false;
			
			// First next token is the class name
			parser.SkipWhitespace(true);
			classname = parser.StripTokenQuotes(parser.ReadToken(ACTOR_CLASS_SPECIAL_TOKENS));

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

					switch(token) 
					{
						case ":":
							// The next token must be the class to inherit from
							parser.SkipWhitespace(true);
							inheritclass = parser.StripTokenQuotes(parser.ReadToken());
							if(string.IsNullOrEmpty(inheritclass)) 
							{
								parser.ReportError("Expected class name to inherit from");
								return;
							}

							// Find the actor to inherit from
							baseclass = parser.GetArchivedActorByName(inheritclass);
							break;

						case "replaces":
							// The next token must be the class to replace
							parser.SkipWhitespace(true);
							replaceclass = parser.StripTokenQuotes(parser.ReadToken());
							if(string.IsNullOrEmpty(replaceclass)) 
							{
								parser.ReportError("Expected class name to replace");
								return;
							}
							break;

						case "native":
							// Igore this token
							break;

						case "{":
							// Actor scope begins here,
							// break out of this parse loop
							done = true;
							break;

						case "-":
							// This could be a negative doomednum (but our parser sees the - as separate token)
							// So read whatever is after this token and ignore it (negative doomednum indicates no doomednum)
							parser.ReadToken();
							break;

						default:
							//mxd. Property begins with $? Then the whole line is a single value
							if(token.StartsWith("$")) 
							{
								// This is for editor-only properties such as $sprite and $category
								props[token] = new List<string> { (parser.SkipWhitespace(false) ? parser.ReadLine() : "") };
							} 
							else if(!int.TryParse(token, NumberStyles.Integer, CultureInfo.InvariantCulture, out doomednum)) // Check if numeric
							{
								// Not numeric!
								parser.ReportError("Expected editor thing number or start of actor scope while parsing \"" + classname + "\"");
								return;
							}
							break;
					}

					if(done) break; //mxd
				}
				else
				{
					parser.ReportError("Unexpected end of structure");
					return;
				}
			}
			
			// Now parse the contents of actor structure
			string previoustoken = "";
			done = false; //mxd
			while(parser.SkipWhitespace(true))
			{
				string token = parser.ReadToken();
				token = token.ToLowerInvariant();

				switch(token) 
				{
					case "+":
					case "-":
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
						break;

					case "action":
					case "native":
						// We don't need this, ignore up to the first next ;
						while(parser.SkipWhitespace(true)) 
						{
							string t = parser.ReadToken();
							if(string.IsNullOrEmpty(t) || t == ";") break;
						}
						break;

					case "skip_super":
						skipsuper = true;
						break;

					case "states":
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
										StateStructure st = new StateStructure(this, parser);
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
						break;

					case "var": //mxd
						// Type
						parser.SkipWhitespace(true);
						string typestr = parser.ReadToken().ToUpperInvariant();
						UniversalType type = UniversalType.EnumOption; // There is no Unknown type, so let's use something impossiburu...
						switch(typestr)
						{
							case "INT":
								type = UniversalType.Integer;
								break;

							default:
								parser.LogWarning("Unknown user variable type");
								break;
						}

						// Name
						parser.SkipWhitespace(true);
						string name = parser.ReadToken();
						if(string.IsNullOrEmpty(name))
						{
							parser.ReportError("Expected User Variable name");
							return;
						}
						if(!name.StartsWith("user_", StringComparison.OrdinalIgnoreCase))
						{
							parser.ReportError("User Variable name must start with \"user_\" prefix");
							return;
						}
						if(uservars.ContainsKey(name))
						{
							parser.ReportError("User Variable \"" + name + "\" is double defined");
							return;
						}
						if(!skipsuper && baseclass != null && baseclass.uservars.ContainsKey(name))
						{
							parser.ReportError("User variable \"" + name + "\" is already defined in one of the parent classes");
							return;
						}

						// Rest
						parser.SkipWhitespace(true);
						string next = parser.ReadToken();
						if(next == "[") // that's User Array. Let's skip it...
						{
							int arrlen = -1;
							if(!parser.ReadSignedInt(ref arrlen))
							{
								parser.ReportError("Expected User Array length, but got \"" + next + "\"");
								return;
							}
							if(arrlen < 1)
							{
								parser.ReportError("User Array length must be a positive value");
								return;
							}
							if(!parser.NextTokenIs("]") || !parser.NextTokenIs(";"))
							{
								return;
							}
						}
						else if(next != ";")
						{
							parser.ReportError("Expected \";\", but got \"" + next + "\"");
							return;
						}
						else
						{
							// Add to collection
							uservars.Add(name, type);
						}
						break;

					case "}":
						//mxd. Get user vars from the BaseClass, if we have one
						if(!skipsuper && baseclass != null && baseclass.uservars.Count > 0)
						{
							foreach(var group in baseclass.uservars)
								uservars.Add(group.Key, group.Value);
						}

						// Actor scope ends here, break out of this parse loop
						done = true;
						break;

					// Monster property?
					case "monster":
						// This sets certain flags we are interested in
						flags["shootable"] = true;
						flags["countkill"] = true;
						flags["solid"] = true;
						flags["canpushwalls"] = true;
						flags["canusewalls"] = true;
						flags["activatemcross"] = true;
						flags["canpass"] = true;
						flags["ismonster"] = true;
						break;

					// Projectile property?
					case "projectile":
						// This sets certain flags we are interested in
						flags["noblockmap"] = true;
						flags["nogravity"] = true;
						flags["dropoff"] = true;
						flags["missile"] = true;
						flags["activateimpact"] = true;
						flags["activatepcross"] = true;
						flags["noteleport"] = true;
						break;

					// Clearflags property?
					case "clearflags":
						// Clear all flags
						flags.Clear();
						break;

					// Game property?
					case "game":
						// Include all tokens on the same line
						List<string> games = new List<string>();
						while(parser.SkipWhitespace(false)) 
						{
							string v = parser.ReadToken();
							if(string.IsNullOrEmpty(v)) 
							{
								parser.ReportError("Unexpected end of structure");
								return;
							}
							if(v == "\n") break;
							if(v == "}") return; //mxd
							if(v != ",") games.Add(v.ToLowerInvariant());
						}
						props[token] = games;
						break;

					// Property
					default:
						// Property begins with $? Then the whole line is a single value
						if(token.StartsWith("$")) 
						{
							// This is for editor-only properties such as $sprite and $category
							props[token] = new List<string> { (parser.SkipWhitespace(false) ? parser.ReadLine() : "") };
						} 
						else 
						{
							// Next tokens up until the next newline are values
							List<string> values = new List<string>();
							while(parser.SkipWhitespace(false)) 
							{
								string v = parser.ReadToken();
								if(string.IsNullOrEmpty(v)) 
								{
									parser.ReportError("Unexpected end of structure");
									return;
								}
								if(v == "\n") break;
								if(v == "}") return; //mxd
								if(v != ",") values.Add(v);
							}

							//mxd. Translate scale to xscale and yscale
							if(token == "scale")
							{
								props["xscale"] = values;
								props["yscale"] = values;
							}
							else
							{
								props[token] = values;
							}
						}
						break;
				}

				if(done) break; //mxd

				// Keep token
				previoustoken = token;
			}

			//mxd. Check if baseclass is valid
			if(inheritclass.ToLowerInvariant() != "actor" && doomednum > -1 && baseclass == null) 
			{
				//check if this class inherits from a class defined in game configuration
				Dictionary<int, ThingTypeInfo> things = General.Map.Config.GetThingTypes();
				string inheritclasscheck = inheritclass.ToLowerInvariant();

				foreach(KeyValuePair<int, ThingTypeInfo> ti in things) 
				{
					if(!string.IsNullOrEmpty(ti.Value.ClassName) && ti.Value.ClassName.ToLowerInvariant() == inheritclasscheck) 
					{
						//states
						if(states.Count == 0 && !string.IsNullOrEmpty(ti.Value.Sprite))
							states.Add("spawn", new StateStructure(ti.Value.Sprite.Substring(0, 4)));

						//flags
						if(ti.Value.Hangs && !flags.ContainsKey("spawnceiling"))
							flags["spawnceiling"] = true;

						if(ti.Value.Blocking > 0 && !flags.ContainsKey("solid"))
							flags["solid"] = true;
						
						//properties
						if(!props.ContainsKey("height"))
							props["height"] = new List<string> { ti.Value.Height.ToString() };

						if(!props.ContainsKey("radius"))
							props["radius"] = new List<string> { ti.Value.Radius.ToString() };

						return;
					}
				}

				parser.LogWarning("Unable to find \"" + inheritclass + "\" class to inherit from, while parsing \"" + classname + ":" + doomednum + "\"");
			}
		}
		
		// Disposer
		public void Dispose()
		{
			baseclass = null;
			flags = null;
			props = null;
			states = null;
		}
		
		#endregion
		
		#region ================== Methods
		
		/// <summary>
		/// This checks if the actor has a specific property.
		/// </summary>
		public bool HasProperty(string propname)
		{
			if(props.ContainsKey(propname)) return true;
			if(!skipsuper && (baseclass != null)) return baseclass.HasProperty(propname);
			return false;
		}
		
		/// <summary>
		/// This checks if the actor has a specific property with at least one value.
		/// </summary>
		public bool HasPropertyWithValue(string propname)
		{
			if(props.ContainsKey(propname) && (props[propname].Count > 0)) return true;
			if(!skipsuper && (baseclass != null)) return baseclass.HasPropertyWithValue(propname);
			return false;
		}
		
		/// <summary>
		/// This returns values of a specific property as a complete string. Returns an empty string when the propery has no values.
		/// </summary>
		public string GetPropertyAllValues(string propname)
		{
			if(props.ContainsKey(propname) && (props[propname].Count > 0))
				return string.Join(" ", props[propname].ToArray());
			if(!skipsuper && (baseclass != null))
				return baseclass.GetPropertyAllValues(propname);
			return "";
		}
		
		/// <summary>
		/// This returns a specific value of a specific property as a string. Returns an empty string when the propery does not have the specified value.
		/// </summary>
		public string GetPropertyValueString(string propname, int valueindex)
		{
			if(props.ContainsKey(propname) && (props[propname].Count > valueindex))
				return props[propname][valueindex];
			if(!skipsuper && (baseclass != null))
				return baseclass.GetPropertyValueString(propname, valueindex);
			return "";
		}
		
		/// <summary>
		/// This returns a specific value of a specific property as an integer. Returns 0 when the propery does not have the specified value.
		/// </summary>
		public int GetPropertyValueInt(string propname, int valueindex)
		{
			string str = GetPropertyValueString(propname, valueindex);

			//mxd. It can be negative...
			if(str == "-" && props.Count > valueindex + 1) 
				str += GetPropertyValueString(propname, valueindex + 1);
			
			int intvalue;
			if(int.TryParse(str, NumberStyles.Integer, CultureInfo.InvariantCulture, out intvalue))
				return intvalue;
			return 0;
		}
		
		/// <summary>
		/// This returns a specific value of a specific property as a float. Returns 0.0f when the propery does not have the specified value.
		/// </summary>
		public float GetPropertyValueFloat(string propname, int valueindex)
		{
			string str = GetPropertyValueString(propname, valueindex);

			//mxd. It can be negative...
			if(str == "-" && props.Count > valueindex + 1)
				str += GetPropertyValueString(propname, valueindex + 1);

			float fvalue;
			if(float.TryParse(str, NumberStyles.Float, CultureInfo.InvariantCulture, out fvalue))
				return fvalue;
			return 0.0f;
		}
		
		/// <summary>
		/// This returns the status of a flag.
		/// </summary>
		public bool HasFlagValue(string flag)
		{
			if(flags.ContainsKey(flag)) return true;
			if(!skipsuper && (baseclass != null)) return baseclass.HasFlagValue(flag);
			return false;
		}
		
		/// <summary>
		/// This returns the status of a flag.
		/// </summary>
		public bool GetFlagValue(string flag, bool defaultvalue)
		{
			if(flags.ContainsKey(flag)) return flags[flag];
			if(!skipsuper && (baseclass != null)) return baseclass.GetFlagValue(flag, defaultvalue);
			return defaultvalue;
		}
		
		/// <summary>
		/// This checks if a state has been defined.
		/// </summary>
		public bool HasState(string statename)
		{
			if(states.ContainsKey(statename)) return true;
			if(!skipsuper && (baseclass != null)) return baseclass.HasState(statename);
			return false;
		}
		
		/// <summary>
		/// This returns a specific state, or null when the state can't be found.
		/// </summary>
		public StateStructure GetState(string statename)
		{
			if(states.ContainsKey(statename)) return states[statename];
			if(!skipsuper && (baseclass != null)) return baseclass.GetState(statename);
			return null;
		}
		
		/// <summary>
		/// This creates a list of all states, also those inherited from the base class.
		/// </summary>
		public Dictionary<string, StateStructure> GetAllStates()
		{
			Dictionary<string, StateStructure> list = new Dictionary<string, StateStructure>(states, StringComparer.OrdinalIgnoreCase);
			
			if(!skipsuper && (baseclass != null))
			{
				Dictionary<string, StateStructure> baselist = baseclass.GetAllStates();
				foreach(KeyValuePair<string, StateStructure> s in baselist)
					if(!list.ContainsKey(s.Key)) list.Add(s.Key, s.Value);
			}
			
			return list;
		}
		
		/// <summary>
		/// This checks if this actor is meant for the current decorate game support
		/// </summary>
		public bool CheckActorSupported()
		{
			// Check if we want to include this actor
			string includegames = General.Map.Config.DecorateGames.ToLowerInvariant();
			bool includeactor = (props["game"].Count == 0);
			foreach(string g in props["game"])
				includeactor |= includegames.Contains(g);
			
			return includeactor;
		}
		
		/// <summary>
		/// This finds the best suitable sprite to use when presenting this actor to the user.
		/// </summary>
		public string FindSuitableSprite()
		{
			string result = string.Empty;
			
			// Sprite forced?
			if(HasPropertyWithValue("$sprite"))
			{
				string sprite = GetPropertyValueString("$sprite", 0); //mxd
				if((sprite.Length > DataManager.INTERNAL_PREFIX.Length) && 
					sprite.ToLowerInvariant().StartsWith(DataManager.INTERNAL_PREFIX)) return sprite; //mxd
				if(General.Map.Data.GetSpriteExists(sprite)) return sprite; //mxd. Added availability check

				//mxd. Bitch and moan
				General.ErrorLogger.Add(ErrorType.Warning, "DECORATE warning in " + classname + ":" + doomednum + ". The sprite \"" + sprite + "\" assigned by the \"$sprite\" property does not exist.");
			}

			// Try the idle state
			if(HasState("idle"))
			{
				StateStructure s = GetState("idle");
				string spritename = s.GetSprite(0);
				if(!string.IsNullOrEmpty(spritename))
					result = spritename;
			}
			
			// Try the see state
			if(string.IsNullOrEmpty(result) && HasState("see"))
			{
				StateStructure s = GetState("see");
				string spritename = s.GetSprite(0);
				if(!string.IsNullOrEmpty(spritename))
					result = spritename;
			}
			
			// Try the inactive state
			if(string.IsNullOrEmpty(result) && HasState("inactive"))
			{
				StateStructure s = GetState("inactive");
				string spritename = s.GetSprite(0);
				if(!string.IsNullOrEmpty(spritename))
					result = spritename;
			}
			
			// Try the spawn state
			if(string.IsNullOrEmpty(result) && HasState("spawn"))
			{
				StateStructure s = GetState("spawn");
				string spritename = s.GetSprite(0);
				if(!string.IsNullOrEmpty(spritename))
					result = spritename;
			}
			
			// Still no sprite found? then just pick the first we can find
			if(string.IsNullOrEmpty(result))
			{
				Dictionary<string, StateStructure> list = GetAllStates();
				foreach(StateStructure s in list.Values)
				{
					string spritename = s.GetSprite(0);
					if(!string.IsNullOrEmpty(spritename))
					{
						result = spritename;
						break;
					}
				}
			}
			
			if(!string.IsNullOrEmpty(result))
			{
				// The sprite name is not actually complete, we still have to append
				// the direction characters to it. Find an existing sprite with direction.
				foreach(string postfix in SPRITE_POSTFIXES)
				{
					if(General.Map.Data.GetSpriteExists(result + postfix))
						return result + postfix;
				}
			}
			
			// No sprite found
			return string.Empty;
		}

		//mxd. 
		///TODO: rewrite this
		public string FindSuitableVoxel(HashSet<string> voxels) 
		{
			string result = string.Empty;
			
			// Try the idle state
			if(HasState("idle")) 
			{
				StateStructure s = GetState("idle");
				string spritename = s.GetSprite(0);
				if(!string.IsNullOrEmpty(spritename)) result = spritename;
			}

			// Try the see state
			if(string.IsNullOrEmpty(result) && HasState("see")) 
			{
				StateStructure s = GetState("see");
				string spritename = s.GetSprite(0);
				if(!string.IsNullOrEmpty(spritename)) result = spritename;
			}

			// Try the inactive state
			if(string.IsNullOrEmpty(result) && HasState("inactive")) 
			{
				StateStructure s = GetState("inactive");
				string spritename = s.GetSprite(0);
				if(!string.IsNullOrEmpty(spritename)) result = spritename;
			}

			// Try the spawn state
			if(string.IsNullOrEmpty(result) && HasState("spawn")) 
			{
				StateStructure s = GetState("spawn");
				string spritename = s.GetSprite(0);
				if(!string.IsNullOrEmpty(spritename)) result = spritename;
			}

			// Still no sprite found? then just pick the first we can find
			if(string.IsNullOrEmpty(result)) 
			{
				Dictionary<string, StateStructure> list = GetAllStates();
				foreach(StateStructure s in list.Values) 
				{
					string spritename = s.GetSprite(0);
					if(!string.IsNullOrEmpty(spritename)) 
					{
						result = spritename;
						break;
					}
				}
			}

			if(!string.IsNullOrEmpty(result)) 
			{
				if(voxels.Contains(result)) return result;

				// The sprite name may be incomplete. Find an existing sprite with direction.
				foreach(string postfix in SPRITE_POSTFIXES)
					if(voxels.Contains(result + postfix)) return result + postfix;
			}


			// No voxel found
			return "";
		}
		
		#endregion
	}
}
