using System;
using System.Collections.Generic;
using System.IO;
using CodeImp.DoomBuilder.ZDoom;
using CodeImp.DoomBuilder.GZBuilder.Data;

namespace CodeImp.DoomBuilder.GZBuilder.GZDoom 
{
	internal class ModeldefParser : ZDTextParser 
	{
		private Dictionary<string, ModelData> entries; //classname, entry
		internal Dictionary<string, ModelData> Entries { get { return entries; } }

		internal ModeldefParser() 
		{
			entries = new Dictionary<string, ModelData>(StringComparer.Ordinal);
		}

		//should be called after all decorate actors are parsed 
		public override bool Parse(Stream stream, string sourcefilename) 
		{
			base.Parse(stream, sourcefilename);
			entries = new Dictionary<string, ModelData>(StringComparer.Ordinal);

			// Continue until at the end of the stream
			while(SkipWhitespace(true)) 
			{
				string token = ReadToken();
				if(!string.IsNullOrEmpty(token)) 
				{
					token = StripTokenQuotes(token).ToLowerInvariant();
					if(token == "model") //model structure start
					{ 
						//find classname
						SkipWhitespace(true);
						string displayclassname = StripTokenQuotes(ReadToken(ActorStructure.ACTOR_CLASS_SPECIAL_TOKENS));
						string classname = displayclassname.ToLowerInvariant();

						if(!string.IsNullOrEmpty(classname) && !entries.ContainsKey(classname)) 
						{
							//now find opening brace
							SkipWhitespace(true);
							token = ReadToken();
							if(token != "{") 
							{
								General.ErrorLogger.Add(ErrorType.Error, "Unexpected token found in '" + sourcefilename + "' at line " + GetCurrentLineNumber() + ": expected '{', but got '" + token + "'");
								continue; //something wrong with modeldef declaration, continue to next one
							}

							ModeldefStructure mds = new ModeldefStructure();
							if(mds.Parse(this, displayclassname) && mds.ModelData != null)
							{
								entries.Add(classname, mds.ModelData);
							}
							
							if(HasError)
							{
								LogError();
								ClearError();
							}

							// Skip untill current structure end
							if(!mds.ParsingFinished)
							{
								while(SkipWhitespace(true))
								{
									token = ReadToken();
									if(string.IsNullOrEmpty(token) || token == "}") break;
								}
							}
						}
					} 
					else 
					{
						// Unknown structure!
						string token2;
						if(token != "{") 
						{
							do
							{
								if(!SkipWhitespace(true)) break;
								token2 = ReadToken();
								if(string.IsNullOrEmpty(token2)) break;
							} 
							while(token2 != "{");
						}

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
				}
			}

			return entries.Count > 0;
		}

		protected override string GetLanguageType()
		{
			return "MODELDEF";
		}
	}
}
