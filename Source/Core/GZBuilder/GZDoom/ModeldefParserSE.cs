#region ================== Namespaces

using System.Collections.Generic;
using System.IO;
using CodeImp.DoomBuilder.GZBuilder.Data;
using CodeImp.DoomBuilder.ZDoom;

#endregion

//mxd. Modeldef parser used to create ScriptItems for use in script editor's navigator
//Should be parse model definitions even from invalid MODELDEF and should never fail parsing
namespace CodeImp.DoomBuilder.GZBuilder.GZDoom 
{
	internal sealed class ModeldefParserSE : ZDTextParser 
	{
		private readonly List<ScriptItem> models;
		internal List<ScriptItem> Models { get { return models; } }

		public ModeldefParserSE() 
		{
			models = new List<ScriptItem>();
		}

		public override bool Parse(Stream stream, string sourcefilename, bool clearerrors) 
		{
			if(!base.Parse(stream, sourcefilename, clearerrors)) return false;

			// Continue until at the end of the stream
			while(SkipWhitespace(true)) 
			{
				string token = ReadToken();
				if(string.IsNullOrEmpty(token) || token.ToUpperInvariant() != "MODEL") continue; 

				SkipWhitespace(true);
				int startpos = (int)stream.Position;
				string modelname = ReadToken();

				SkipWhitespace(true);
				token = ReadToken(); //this should be "{"

				if(token == "{") 
				{
					ScriptItem i = new ScriptItem(modelname, startpos, false);
					models.Add(i);
				}

				while(SkipWhitespace(true))
				{
					token = ReadToken();
					if(string.IsNullOrEmpty(token) || token == "}") break;
				}
			}

			// Sort nodes
			models.Sort(ScriptItem.SortByName);
			return true;
		}

		protected override string GetLanguageType()
		{
			return "MODELDEF";
		}
	}
}
