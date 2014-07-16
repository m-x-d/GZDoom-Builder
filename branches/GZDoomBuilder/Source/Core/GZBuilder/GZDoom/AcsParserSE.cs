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

		private readonly List<string> parsedLumps;
		private readonly List<string> includes;

		private readonly List<ScriptItem> namedScripts;
		private readonly List<ScriptItem> numberedScripts;
		private readonly List<ScriptItem> functions;

		internal List<ScriptItem> NamedScripts { get { return namedScripts; } }
		internal List<ScriptItem> NumberedScripts { get { return numberedScripts; } }
		internal List<ScriptItem> Functions { get { return functions; } }

		internal AcsParserSE() {
			namedScripts = new List<ScriptItem>();
			numberedScripts = new List<ScriptItem>();
			functions = new List<ScriptItem>();
			parsedLumps = new List<string>();
			includes = new List<string>();
		}

		internal List<string> Includes {
			get { return includes; }
		}

		public override bool Parse(Stream stream, string sourcefilename) {
			return Parse(stream, sourcefilename, false, false);
		}

		public bool Parse(Stream stream, string sourcefilename, bool processIncludes) {
			return Parse(stream, sourcefilename, processIncludes, false);
		}

		public bool Parse(Stream stream, string sourcefilename, bool processIncludes, bool isinclude) {
			base.Parse(stream, sourcefilename);

			//already parsed this?
			if (parsedLumps.Contains(sourcefilename)) return false;
			parsedLumps.Add(sourcefilename);
			if (isinclude) includes.Add(sourcefilename);

			// Keep local data
			Stream localstream = datastream;
			string localsourcename = sourcename;
			BinaryReader localreader = datareader;

			// Continue until at the end of the stream
			while (SkipWhitespace(true)) {
				string token = ReadToken();

				if (!string.IsNullOrEmpty(token)) {
					token = token.ToLowerInvariant();

					if (token == "script") {
						int startPos = (int)stream.Position - 7;
						SkipWhitespace(true);
						token = ReadToken();

						//is it named script?
						if (token.IndexOf('"') != -1) {
							//check if we have something like '"mycoolscript"(void)' as a token
							if(token.LastIndexOf('"') != token.Length - 1)
								token = token.Substring(0, token.LastIndexOf('"'));

							token = StripTokenQuotes(token);
							ScriptItem i = new ScriptItem(0, token, startPos, (int)stream.Position-1);
							namedScripts.Add(i);
						} else { //should be numbered script
							//check if we have something like "999(void)" as a token
							if (token.Contains("(")) token = token.Substring(0, token.IndexOf("("));

							int n;
							if (int.TryParse(token, NumberStyles.Integer, CultureInfo.InvariantCulture, out n)) {
								int endPos = (int)stream.Position - 1;

								//now find opening brace
								do {
									SkipWhitespace(true);
									token = ReadToken();
								} while (token != "{");

								token = ReadLine();
								string name = "";

								if (token.Length > 0) {
									int commentStart = token.IndexOf("//");
									if (commentStart != -1) { //found comment
										commentStart += 2;
										name = token.Substring(commentStart, token.Length - commentStart).Trim();
									}
								}

								name = (name.Length > 0 ? name + " [" + n + "]" : "Script " + n);
								ScriptItem i = new ScriptItem(n, name, startPos, endPos);
								numberedScripts.Add(i);
							}
						}

					} else if(token == "function") {
						int startPos = (int)stream.Position - 9;
						SkipWhitespace(true);
						string funcname = ReadToken(); //read return type
						SkipWhitespace(true);
						funcname += " " + ReadToken(); //read function name

						//look for opening brace
						if (!funcname.Contains("(")) {
							SkipWhitespace(true);
							funcname += " " + ReadToken();
						} else {
							funcname = funcname.Replace("(", " (");
						}

						//look for closing brace
						if(!funcname.Contains(")")) {
							do {
								SkipWhitespace(true);
								token = ReadToken();
								funcname += " " + token;
							} while(!token.Contains(")"));
						}

						ScriptItem i = new ScriptItem(0, funcname, startPos, (int)stream.Position - 1);
						functions.Add(i);

					} else if (processIncludes && (token == "#include" || token == "#import")) {
						SkipWhitespace(true);
						string includeLump = StripTokenQuotes(ReadToken()).ToLowerInvariant();

						if (!string.IsNullOrEmpty(includeLump)) {
							string includeName = Path.GetFileName(includeLump);

							if (includeName == "zcommon.acs" || includeName == "common.acs" || includes.Contains(includeName))
								continue;
							
							// Callback to parse this file
							if (OnInclude != null) OnInclude(this, includeLump.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar));

							// Set our buffers back to continue parsing
							datastream = localstream;
							datareader = localreader;
							sourcename = localsourcename;
						} else {
							General.ErrorLogger.Add(ErrorType.Error, "Error in '" + sourcefilename + "' at line " + GetCurrentLineNumber() + ": got #include directive without include path!");
						}
					}
				}
			}
			return true;
		}
	}
}