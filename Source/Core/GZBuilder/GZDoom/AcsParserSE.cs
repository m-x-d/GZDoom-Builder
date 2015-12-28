using System;
using System.IO;
using System.Collections.Generic;
using System.Globalization;
using CodeImp.DoomBuilder.ZDoom;
using CodeImp.DoomBuilder.GZBuilder.Data;

//mxd. ACS parser used to create ScriptItems for use in script editor's navigator
namespace CodeImp.DoomBuilder.GZBuilder.GZDoom
{
	internal sealed class AcsParserSE : ZDTextParser
	{
		internal delegate void IncludeDelegate(AcsParserSE parser, string includefile, IncludeType includetype);
		internal IncludeDelegate OnInclude;

		private readonly HashSet<string> parsedlumps;
		private readonly HashSet<string> includes;
		private HashSet<string> includestoskip;
		private string libraryname;

		private readonly List<ScriptItem> namedscripts;
		private readonly List<ScriptItem> numberedscripts;
		private readonly List<ScriptItem> functions;

		internal List<ScriptItem> NamedScripts { get { return namedscripts; } }
		internal List<ScriptItem> NumberedScripts { get { return numberedscripts; } }
		internal List<ScriptItem> Functions { get { return functions; } }
		internal HashSet<string> Includes { get { return includes; } }
		internal bool IsLibrary { get { return !string.IsNullOrEmpty(libraryname); } }
		internal string LibraryName { get { return libraryname; } }

		internal bool AddArgumentsToScriptNames;
		internal bool IsMapScriptsLump;

		internal enum IncludeType
		{
			NONE,
			INCLUDE,
			LIBRARY
		}

		internal AcsParserSE() 
		{
			namedscripts = new List<ScriptItem>();
			numberedscripts = new List<ScriptItem>();
			functions = new List<ScriptItem>();
			parsedlumps = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
			includes = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
			includestoskip = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
			specialtokens += "(,)";
		}

		public override bool Parse(Stream stream, string sourcefilename, bool clearerrors) 
		{
			return Parse(stream, sourcefilename, new HashSet<string>(), false, IncludeType.NONE, clearerrors);
		}

		public bool Parse(Stream stream, string sourcefilename, bool processincludes, IncludeType includetype, bool clearerrors)
		{
			return Parse(stream, sourcefilename, includestoskip, processincludes, includetype, clearerrors);
		}

		public bool Parse(Stream stream, string sourcefilename, HashSet<string> configincludes, bool processincludes, IncludeType includetype, bool clearerrors) 
		{
			string source = sourcefilename.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);

			// Duplicate checks
			if(parsedlumps.Contains(source))
			{
				ReportError("Already parsed '" + source + "'. Check your #include directives");
				return false;
			}
			
			parsedlumps.Add(source);
			includestoskip = configincludes;
			int bracelevel = 0;

			if(!base.Parse(stream, sourcefilename, clearerrors)) return false;

			// Keep local data
			Stream localstream = datastream;
			string localsourcename = sourcename;
			BinaryReader localreader = datareader;

			// Continue until at the end of the stream
			while(SkipWhitespace(true)) 
			{
				string token = ReadToken();
				if(string.IsNullOrEmpty(token)) continue;

				// Ignore inner scope stuff
				if(token == "{") { bracelevel++; continue; }
				if(token == "}") { bracelevel--; continue; }
				if(bracelevel > 0) continue;

				switch(token.ToLowerInvariant())
				{
					case "script":
					{
						SkipWhitespace(true);
						int startpos = (int)stream.Position;
						token = ReadToken();

						//is it named script?
						if(token.IndexOf('"') != -1) 
						{
							startpos += 1;
							string scriptname = StripTokenQuotes(token);

							// Try to parse argument names
							List<KeyValuePair<string, string>> args = ParseArgs();
							List<string> argnames = new List<string>();
							foreach(KeyValuePair<string, string> group in args) argnames.Add(group.Value);

							// Make full name
							if(AddArgumentsToScriptNames) scriptname += " " + GetArgumentNames(args);

							// Add to collection
							namedscripts.Add(new ScriptItem(scriptname, argnames, startpos, includetype != IncludeType.NONE));
						} 
						else //should be numbered script
						{ 
							int n;
							if(int.TryParse(token, NumberStyles.Integer, CultureInfo.InvariantCulture, out n)) 
							{
								// Try to parse argument names
								List<KeyValuePair<string, string>> args = ParseArgs();
								
								// Now find the opening brace
								do 
								{
									if(!SkipWhitespace(true)) break;
									token = ReadToken();
								} while(!string.IsNullOrEmpty(token) && token != "{");

								token = ReadLine();
								string name = "";
								bracelevel = 1;

								if(!string.IsNullOrEmpty(token))
								{
									int commentstart = token.IndexOf("//", System.StringComparison.Ordinal);
									if(commentstart != -1) //found comment
									{ 
										commentstart += 2;
										name = token.Substring(commentstart, token.Length - commentstart).Trim();
									}
								}

								bool customname = (name.Length > 0);
								name = (customname ? name + " [Script " + n + "]" : "Script " + n);
								
								List<string> argnames = new List<string>();
								foreach(KeyValuePair<string, string> group in args) argnames.Add(group.Value);

								// Make full name
								if(AddArgumentsToScriptNames) name += " " + GetArgumentNames(args);

								// Add to collection
								numberedscripts.Add(new ScriptItem(n, name, argnames, startpos, includetype != IncludeType.NONE, customname));
							}
						}
					}
					break;

					case "function":
					{
						SkipWhitespace(true);
						int startpos = (int)stream.Position;
						string funcname = ReadToken(); //read return type
						SkipWhitespace(true);
						funcname += " " + ReadToken(); //read function name

						// Try to parse argument names
						List<KeyValuePair<string, string>> args = ParseArgs();
						List<string> argnames = new List<string>();
						foreach(KeyValuePair<string, string> group in args) argnames.Add(group.Value);

						// Make full name
						if(AddArgumentsToScriptNames) funcname += GetArgumentNames(args);

						// Add to collection
						functions.Add(new ScriptItem(funcname, argnames, startpos, includetype != IncludeType.NONE));
					}
					break;

					case "#library":
						if(IsMapScriptsLump)
						{
							ReportError("SCRIPTS lump can not be compiled as a library");
							return false;
						}
						
						SkipWhitespace(true);
						string libname = ReadToken(false); // Don't skip newline

						if(!libname.StartsWith("\"") || !libname.EndsWith("\""))
						{
							ReportError("#library name should be quoted");
							return false;
						}

						libname = StripTokenQuotes(libname);

						if(string.IsNullOrEmpty(libname))
						{
							ReportError("Expected library name");
							return false;
						}

						// Store only when the script compiling was executed for is library
						if(includetype == IncludeType.NONE)
						{
							libraryname = libname;
							includetype = IncludeType.LIBRARY;
						}

						break;

					default:
						if(processincludes && (token == "#include" || token == "#import")) 
						{
							//INFO: ZDoom ACC include paths can be absolute ("d:\stuff\coollib.acs"), relative ("../coollib.acs") 
							//and can use forward and backward slashes ("acs\map01/script.acs")
							//also include paths must be quoted
							//long filenames are supported
							bool islibrary = (token == "#import" || includetype == IncludeType.LIBRARY);
							SkipWhitespace(true);
							string includelump = ReadToken(false); // Don't skip newline

							if(!includelump.StartsWith("\"") || !includelump.EndsWith("\""))
							{
								ReportError(token + " filename should be quoted");
								return false;
							}

							includelump = StripTokenQuotes(includelump);

							if(string.IsNullOrEmpty(includelump))
							{
								ReportError("Expected file name to " + token);
								return false;
							}

							includelump = includelump.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);

							// Compiler files?
							if(includestoskip.Contains(includelump)) continue;

							// Convert to a path we can use
							string includelumppath = GetRootedPath(source, includelump);

							// Rooting succeeded?
							if(HasError || string.IsNullOrEmpty(includelumppath)) return false;

							// Already parsed?
							if(includes.Contains(includelumppath))
							{
								//INFO: files included or imported inside a library are not visible to the code outside it 
								//and must be included/imported separately
								if(!islibrary)
								{
									ReportError("Already parsed '" + includelump + "'. Check your " + token + " directives");
									return false;
								}
							}
							else
							{
								// Add to collections
								includes.Add(includelumppath);

								// Callback to parse this file
								if(OnInclude != null)
								{
									OnInclude(this, includelumppath, islibrary ? IncludeType.LIBRARY : IncludeType.INCLUDE);
								}

								// Bail out on error
								if(this.HasError) return false;

								// Set our buffers back to continue parsing
								datastream = localstream;
								datareader = localreader;
								sourcename = localsourcename;
							}
						}
						break;
				}
			}
			return true;
		}

		private List<KeyValuePair<string, string>> ParseArgs() //type, name
		{
			List<KeyValuePair<string, string>> argnames = new List<KeyValuePair<string, string>>();
			SkipWhitespace(true);
			string token = ReadToken();
			
			// Should be ENTER/OPEN etc. script type
			if(token != "(")
			{
				argnames.Add(new KeyValuePair<string, string>(token.ToUpperInvariant(), string.Empty));
				return argnames;
			}

			while(SkipWhitespace(true))
			{
				string argtype = ReadToken(); // should be type
				if(IsSpecialToken(argtype)) break;
				if(argtype.ToUpperInvariant() == "VOID")
				{
					argnames.Add(new KeyValuePair<string, string>("void", string.Empty));
					break;
				}

				SkipWhitespace(true);
				token = ReadToken(); // should be arg name
				argnames.Add(new KeyValuePair<string, string>(argtype, token));

				SkipWhitespace(true);
				token = ReadToken(); // should be comma or ")"
				if(token != ",") break;
			}

			return argnames;
		}

		private static string GetArgumentNames(List<KeyValuePair<string, string>> args)
		{
			// Make full name
			if(args.Count > 0)
			{
				List<string> argdescs = new List<string>(args.Count);
				foreach(KeyValuePair<string, string> group in args)
					argdescs.Add((group.Key + " " + group.Value).TrimEnd());

				return "(" + string.Join(", ", argdescs.ToArray()) + ")";
			}

			return "(void)";
		}

		protected override string GetLanguageType()
		{
			return "ACS";
		}
	}
}