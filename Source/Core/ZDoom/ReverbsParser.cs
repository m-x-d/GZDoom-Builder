using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace CodeImp.DoomBuilder.ZDoom
{
	internal sealed class ReverbsParser : ZDTextParser
	{
		private readonly List<string> names;
		private readonly List<int> firstargs;
		private readonly List<int> secondargs;
		private readonly List<int> combinedargs;

		public ReverbsParser() 
		{
			names = new List<string>();
			firstargs = new List<int>();
			secondargs = new List<int>();
			combinedargs = new List<int>();
		}
		
		public override bool Parse(Stream stream, string sourcefilename)
		{
			base.Parse(stream, sourcefilename);

			// Continue until at the end of the stream
			while(SkipWhitespace(true)) 
			{
				string token = ReadToken();

				if(!string.IsNullOrEmpty(token)) 
				{
					if(token == "{") 
					{
						// Skip inner properties
						do 
						{
							SkipWhitespace(true);
							token = ReadToken();
						} while(!string.IsNullOrEmpty(token) && token != "}");
					}
					else 
					{
						//this should be reverb name and args
						string name = StripTokenQuotes(token);

						if(string.IsNullOrEmpty(name))
						{
							ReportError("Got empty sound environment name!");
							break;
						}

						// Read first part of the ID
						SkipWhitespace(true);
						token = StripTokenQuotes(ReadToken());
						int arg1;
						if(!int.TryParse(token, NumberStyles.Integer, CultureInfo.InvariantCulture, out arg1))
						{
							ReportError("Failed to parse the first part of '" + name + "' sound environment ID!");
							break;
						}

						// Read second part of the ID
						SkipWhitespace(true);
						token = StripTokenQuotes(ReadToken());
						int arg2;
						if(!int.TryParse(token, NumberStyles.Integer, CultureInfo.InvariantCulture, out arg2)) 
						{
							ReportError("Failed to parse the second part of '" + name + "' sound environment ID!");
							break;
						}

						int combined = arg1 * 1000000 + arg2 * 1000;
						int combinedindex = combinedargs.IndexOf(combined);
						if(combinedindex != -1)
						{
							General.ErrorLogger.Add(ErrorType.Warning, "'" + names[combinedindex] + "' and '" + name + "' sound environments share the same ID (" + arg1 + " " + arg2 + ")!");
						}
						else
						{
							combinedargs.Add(combined);
							
							// Add to collections
							if(names.Contains(name)) 
							{
								General.ErrorLogger.Add(ErrorType.Warning, "'" + name + "' sound environment is double-defined in '" + sourcefilename + "'!");
								int index = names.IndexOf(name);
								firstargs[index] = arg1;
								secondargs[index] = arg2;
							} 
							else 
							{
								names.Add(name);
								firstargs.Add(arg1);
								secondargs.Add(arg2);
							}
						}
					}
				}
			}

			return true;
		}

		internal Dictionary<string, KeyValuePair<int, int>> GetReverbs() 
		{
			string[] sortednames = new string[names.Count];
			names.CopyTo(sortednames);
			Array.Sort(sortednames);

			Dictionary<string, KeyValuePair<int, int>> result = new Dictionary<string, KeyValuePair<int, int>>(names.Count);

			foreach(string name in sortednames) 
			{
				int index = names.IndexOf(name);
				result.Add(name, new KeyValuePair<int, int>(firstargs[index], secondargs[index]));
			}

			return result;
		}
	}
}
