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
		internal delegate void IncludeDelegate(AcsParserSE parser, string includefile);
		internal IncludeDelegate OnInclude;

		private readonly List<string> parsedlumps;
		private readonly List<string> includes;

		private readonly List<ScriptItem> namedscripts;
		private readonly List<ScriptItem> numberedscripts;
		private readonly List<ScriptItem> functions;

		internal List<ScriptItem> NamedScripts { get { return namedscripts; } }
		internal List<ScriptItem> NumberedScripts { get { return numberedscripts; } }
		internal List<ScriptItem> Functions { get { return functions; } }
		internal IEnumerable<string> Includes { get { return includes; } }

		internal AcsParserSE() 
		{
			namedscripts = new List<ScriptItem>();
			numberedscripts = new List<ScriptItem>();
			functions = new List<ScriptItem>();
			parsedlumps = new List<string>();
			includes = new List<string>();
		}

		public override bool Parse(Stream stream, string sourcefilename) 
		{
			return Parse(stream, sourcefilename, false, false);
		}

		public bool Parse(Stream stream, string sourcefilename, bool processincludes, bool isinclude) 
		{
			base.Parse(stream, sourcefilename);

			//already parsed this?
			if (parsedlumps.Contains(sourcefilename)) return false;
			parsedlumps.Add(sourcefilename);
			if (isinclude) includes.Add(sourcefilename);
			int bracelevel = 0;

			// Keep local data
			Stream localstream = datastream;
			string localsourcename = sourcename;
			BinaryReader localreader = datareader;

			// Continue until at the end of the stream
			while (SkipWhitespace(true)) 
			{
				string token = ReadToken();
				if(string.IsNullOrEmpty(token)) continue;

				// Ignore inner scope stuff
				if(token == "{") { bracelevel++; continue; }
				if(token == "}") { bracelevel--; continue; }
				if(bracelevel > 0) continue;

				switch (token.ToLowerInvariant())
				{
					case "script":
					{
						SkipWhitespace(true);
						int startpos = (int)stream.Position;
						token = ReadToken();

						//is it named script?
						if (token.IndexOf('"') != -1) 
						{
							startpos += 1;
							//check if we have something like '"mycoolscript"(void)' as a token
							if(token.LastIndexOf('"') != token.Length - 1)
								token = token.Substring(0, token.LastIndexOf('"'));

							token = StripTokenQuotes(token);
							ScriptItem i = new ScriptItem(0, token, startpos, isinclude);
							namedscripts.Add(i);
						} 
						else //should be numbered script
						{ 
							//check if we have something like "999(void)" as a token
							if (token.Contains("(")) token = token.Substring(0, token.IndexOf("("));

							int n;
							if (int.TryParse(token, NumberStyles.Integer, CultureInfo.InvariantCulture, out n)) 
							{
								//now find opening brace
								do 
								{
									if(!SkipWhitespace(true)) break;
									token = ReadToken();
								} while (!string.IsNullOrEmpty(token) && token != "{");

								token = ReadLine();
								string name = "";
								bracelevel = 1;

								if (!string.IsNullOrEmpty(token))
								{
									int commentstart = token.IndexOf("//");
									if (commentstart != -1) //found comment
									{ 
										commentstart += 2;
										name = token.Substring(commentstart, token.Length - commentstart).Trim();
									}
								}

								name = (name.Length > 0 ? name + " [" + n + "]" : "Script " + n);
								ScriptItem i = new ScriptItem(n, name, startpos, isinclude);
								numberedscripts.Add(i);
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

						//look for opening brace
						if (!funcname.Contains("(")) 
						{
							SkipWhitespace(true);
							funcname += " " + ReadToken();
						} 
						else 
						{
							funcname = funcname.Replace("(", " (");
						}

						//look for closing brace
						if(!funcname.Contains(")")) 
						{
							do 
							{
								if(!SkipWhitespace(true)) break;
								token = ReadToken();
								funcname += " " + token;
							} while(!string.IsNullOrEmpty(token) && !token.Contains(")"));
						}

						ScriptItem i = new ScriptItem(0, funcname, startpos, isinclude);
						functions.Add(i);
					}
					break;

					default:
						if (processincludes && (token == "#include" || token == "#import")) 
						{
							SkipWhitespace(true);
							string includelump = StripTokenQuotes(ReadToken()).ToLowerInvariant();

							if (!string.IsNullOrEmpty(includelump)) 
							{
								string includename = Path.GetFileName(includelump);

								if (includename == "zcommon.acs" || includename == "common.acs" || includes.Contains(includename))
									continue;
							
								// Callback to parse this file
								if (OnInclude != null) OnInclude(this, includelump.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar));

								// Set our buffers back to continue parsing
								datastream = localstream;
								datareader = localreader;
								sourcename = localsourcename;
							} 
							else 
							{
								General.ErrorLogger.Add(ErrorType.Error, "Error in '" + sourcefilename + "' at line " + GetCurrentLineNumber() + ": got #include directive without include path!");
							}
						}
						break;
				}
			}
			return true;
		}
	}
}