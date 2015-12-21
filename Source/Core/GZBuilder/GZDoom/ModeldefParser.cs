using System;
using System.Collections.Generic;
using System.IO;
using CodeImp.DoomBuilder.ZDoom;
using CodeImp.DoomBuilder.GZBuilder.Data;

namespace CodeImp.DoomBuilder.GZBuilder.GZDoom 
{
	internal class ModeldefParser : ZDTextParser
	{
		private readonly Dictionary<string, int> actorsbyclass;
		internal Dictionary<string, int> ActorsByClass { get { return actorsbyclass; } }

		private Dictionary<string, ModelData> entries; //classname, entry
		internal Dictionary<string, ModelData> Entries { get { return entries; } }

		internal ModeldefParser(Dictionary<string, int> actorsbyclass)
		{
			this.actorsbyclass = actorsbyclass;
			this.entries = new Dictionary<string, ModelData>(StringComparer.Ordinal);
		}

		//should be called after all decorate actors are parsed 
		public override bool Parse(Stream stream, string sourcefilename, bool clearerrors)
		{
			entries = new Dictionary<string, ModelData>(StringComparer.Ordinal);
			if(!base.Parse(stream, sourcefilename, clearerrors)) return false;

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
							if(!mds.ParsingFinished) SkipStructure(1);
						}
					} 
					else 
					{
						// Unknown structure!
						if(token != "{")
						{
							string token2;
							do
							{
								if(!SkipWhitespace(true)) break;
								token2 = ReadToken();
								if(string.IsNullOrEmpty(token2)) break;
							} 
							while(token2 != "{");
						}

						SkipStructure(1);
					}
				}
			}

			return entries.Count > 0;
		}

		// Skips untill current structure end
		private void SkipStructure(int scopelevel)
		{
			do
			{
				if(!SkipWhitespace(true)) break;
				string token = ReadToken();
				if(string.IsNullOrEmpty(token)) break;
				if(token == "{") scopelevel++;
				if(token == "}") scopelevel--;
			}
			while(scopelevel > 0);
		}

		protected override string GetLanguageType()
		{
			return "MODELDEF";
		}
	}
}
