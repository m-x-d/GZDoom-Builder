
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
using System.IO;

#endregion

namespace CodeImp.DoomBuilder.ZDoom
{
	public sealed class DecorateParser : ZDTextParser
	{
		#region ================== Delegates

		public delegate void IncludeDelegate(DecorateParser parser, string includefile);
		
		public IncludeDelegate OnInclude;
		
		#endregion
		
		#region ================== Constants
		
		#endregion
		
		#region ================== Variables
		
		// These are actors we want to keep
		private Dictionary<string, ActorStructure> actors;
		
		// These are all parsed actors, also those from other games
		private Dictionary<string, ActorStructure> archivedactors;
		
		#endregion
		
		#region ================== Properties
		
		/// <summary>
		/// All actors that are supported by the current game.
		/// </summary>
		public ICollection<ActorStructure> Actors { get { return actors.Values; } }

		/// <summary>
		/// All actors defined in the loaded DECORATE structures. This includes actors not supported in the current game.
		/// </summary>
		public ICollection<ActorStructure> AllActors { get { return archivedactors.Values; } }

		/// <summary>
		/// mxd. All actors that are supported by the current game.
		/// </summary>
		internal Dictionary<string, ActorStructure> ActorsByClass { get { return actors; } }

		/// <summary>
		/// mxd. All actors defined in the loaded DECORATE structures. This includes actors not supported in the current game.
		/// </summary>
		internal Dictionary<string, ActorStructure> AllActorsByClass { get { return archivedactors; } }

		#endregion
		
		#region ================== Constructor / Disposer
		
		// Constructor
		public DecorateParser()
		{
			// Syntax
			whitespace = "\n \t\r\u00A0"; //mxd. non-breaking space is also space :)
			specialtokens = ":{}+-\n;,";
			
			// Initialize
			actors = new Dictionary<string, ActorStructure>(StringComparer.Ordinal);
			archivedactors = new Dictionary<string, ActorStructure>(StringComparer.Ordinal);
		}
		
		// Disposer
		public void Dispose()
		{
			foreach(KeyValuePair<string, ActorStructure> a in archivedactors)
				a.Value.Dispose();
			
			actors = null;
			archivedactors = null;
		}
		
		#endregion

		#region ================== Parsing

		// This parses the given decorate stream
		// Returns false on errors
		public override bool Parse(Stream stream, string sourcefilename, bool clearerrors)
		{
			if(!base.Parse(stream, sourcefilename, clearerrors)) return false;
			
			// Keep local data
			Stream localstream = datastream;
			string localsourcename = sourcename;
			BinaryReader localreader = datareader;
			
			// Continue until at the end of the stream
			while(SkipWhitespace(true))
			{
				// Read a token
				string objdeclaration = ReadToken();
				if(!string.IsNullOrEmpty(objdeclaration))
				{
					objdeclaration = objdeclaration.ToLowerInvariant();
					switch(objdeclaration)
					{
						case "actor":
						{
							// Read actor structure
							ActorStructure actor = new ActorStructure(this);
							if(this.HasError) return false;
						
							// Add the actor
							archivedactors[actor.ClassName.ToLowerInvariant()] = actor;
							if(actor.CheckActorSupported())
								actors[actor.ClassName.ToLowerInvariant()] = actor;
						
							// Replace an actor?
							if(actor.ReplacesClass != null)
							{
								if(GetArchivedActorByName(actor.ReplacesClass) != null)
									archivedactors[actor.ReplacesClass.ToLowerInvariant()] = actor;
								else
									LogWarning("Unable to find '" + actor.ReplacesClass + "' class to replace, while parsing '" + actor.ClassName + "'");
							
								if(actor.CheckActorSupported() && GetActorByName(actor.ReplacesClass) != null)
									actors[actor.ReplacesClass.ToLowerInvariant()] = actor;
							}
						}
						break;

						case "#include":
						{
							// Include a file
							SkipWhitespace(true);
							string filename = ReadToken(false); //mxd. Don't skip newline

							//mxd. Sanity checks
							if(!filename.StartsWith("\"") || !filename.EndsWith("\""))
							{
								ReportError("#include filename should be quoted");
								return false;
							}

							// Strip the quotes
							filename = filename.Replace("\"", "");

							//mxd. More sanity checks
							if(string.IsNullOrEmpty(filename))
							{
								ReportError("Expected file name to include");
								return false;
							}

							// Callback to parse this file now
							if(OnInclude != null) OnInclude(this, filename);

							// Set our buffers back to continue parsing
							datastream = localstream;
							datareader = localreader;
							sourcename = localsourcename;
							if(this.HasError) return false;
						}
						break;

						case "enum":
						case "native":
						case "const":
							while(SkipWhitespace(true))
							{
								string t = ReadToken();
								if(string.IsNullOrEmpty(t) || t == ";") break;
							}
							break;

						case "$gzdb_skip": break;

						default:
						{
							// Unknown structure!
							// Best we can do now is just find the first { and then
							// follow the scopes until the matching } is found
							string token2;
							do
							{
								if(!SkipWhitespace(true)) break;
								token2 = ReadToken();
								if(string.IsNullOrEmpty(token2)) break;
							}
							while(token2 != "{");

							int scopelevel = 1;
							do
							{
								if(!SkipWhitespace(true)) break;
								token2 = ReadToken();
								if(string.IsNullOrEmpty(token2)) break;
								if(token2 == "{") scopelevel++;
								if(token2 == "}") scopelevel--;
							}
							while(scopelevel > 0);
						}
						break;
					}
				}
			}
			
			// Return true when no errors occurred
			return (ErrorDescription == null);
		}
		
		#endregion
		
		#region ================== Methods
		
		/// <summary>
		/// This returns a supported actor by name. Returns null when no supported actor with the specified name can be found. This operation is of O(1) complexity.
		/// </summary>
		public ActorStructure GetActorByName(string name)
		{
			name = name.ToLowerInvariant();
			return actors.ContainsKey(name) ? actors[name] : null;
		}

		/// <summary>
		/// This returns a supported actor by DoomEdNum. Returns null when no supported actor with the specified name can be found. Please note that this operation is of O(n) complexity!
		/// </summary>
		public ActorStructure GetActorByDoomEdNum(int doomednum)
		{
			foreach(ActorStructure a in actors.Values)
				if(a.DoomEdNum == doomednum) return a;
			return null;
		}

		// This returns an actor by name
		// Returns null when actor cannot be found
		internal ActorStructure GetArchivedActorByName(string name)
		{
			name = name.ToLowerInvariant();
			return (archivedactors.ContainsKey(name) ? archivedactors[name] : null);
		}

		//mxd
		protected override string GetLanguageType()
		{
			return "DECORATE";
		}
		
		#endregion
	}
}
