using System;
using System.Collections.Generic;
using CodeImp.DoomBuilder.Config;
using CodeImp.DoomBuilder.Data;
using CodeImp.DoomBuilder.ZDoom;
using CodeImp.DoomBuilder.GZBuilder.Data;

namespace CodeImp.DoomBuilder.GZBuilder.GZDoom 
{
	internal class ModeldefParser : ZDTextParser
	{
		internal override ScriptType ScriptType { get { return ScriptType.MODELDEF; } }

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
		public override bool Parse(TextResourceData data, bool clearerrors)
		{
			entries = new Dictionary<string, ModelData>(StringComparer.Ordinal);

			//mxd. Already parsed?
			if(!base.AddTextResource(data))
			{
				if(clearerrors) ClearError();
				return true;
			}

			// Cannot process?
			if(!base.Parse(data, clearerrors)) return false;

			// Continue until at the end of the stream
			while(SkipWhitespace(true)) 
			{
				string token = ReadToken();
				if(!string.IsNullOrEmpty(token)) 
				{
					token = StripTokenQuotes(token).ToLowerInvariant();
					if(token == "model") //model structure start
					{ 
						// Find classname
						SkipWhitespace(true);
						string displayclassname = StripTokenQuotes(ReadToken(ActorStructure.ACTOR_CLASS_SPECIAL_TOKENS));
						string classname = displayclassname.ToLowerInvariant();

						if(!string.IsNullOrEmpty(classname) && !entries.ContainsKey(classname)) 
						{
							// Now find opening brace
							if(!NextTokenIs("{")) return false;

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
	}
}
