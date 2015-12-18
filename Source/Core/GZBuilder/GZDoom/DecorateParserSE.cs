using System.IO;
using System.Collections.Generic;
using CodeImp.DoomBuilder.ZDoom;
using CodeImp.DoomBuilder.GZBuilder.Data;

//mxd. Decorate parser used to create ScriptItems for use in script editor's navigator
//Should be able to parse actor definitions even from invalid DECORATE and should never fail parsing
namespace CodeImp.DoomBuilder.GZBuilder.GZDoom
{
	internal sealed class DecorateParserSE : ZDTextParser
	{
		private readonly List<ScriptItem> actors;
		public List<ScriptItem> Actors { get { return actors; } }

		public DecorateParserSE() 
		{
			actors = new List<ScriptItem>();
		}

		public override bool Parse(Stream stream, string sourcefilename, bool clearerrors) 
		{
			if(!base.Parse(stream, sourcefilename, clearerrors)) return false;

			// Continue until at the end of the stream
			while(SkipWhitespace(true)) 
			{
				string token = ReadToken();
				if(string.IsNullOrEmpty(token) || token.ToUpperInvariant() != "ACTOR") continue;

				SkipWhitespace(true);
				int startpos = (int)stream.Position;

				List<string> definition = new List<string>();

				do
				{
					token = ReadToken(false); // Don't skip newline
					if(string.IsNullOrEmpty(token) || token == "{" || token == "}") break;
					definition.Add(token);
				} while(SkipWhitespace(false)); // Don't skip newline

				string name = string.Join(" ", definition.ToArray());
				if(!string.IsNullOrEmpty(name)) actors.Add(new ScriptItem(name, startpos, false));
			}

			// Sort nodes
			actors.Sort(ScriptItem.SortByName);
			return true;
		}

		protected override string GetLanguageType()
		{
			return "DECORATE";
		}
	}
}
