using System.IO;
using System.Collections.Generic;
using CodeImp.DoomBuilder.ZDoom;
using CodeImp.DoomBuilder.GZBuilder.Data;

//mxd. Decorate parser used to create ScriptItems for use in script editor's navigator
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

		public override bool Parse(Stream stream, string sourcefilename) 
		{
			base.Parse(stream, sourcefilename);

			// Continue until at the end of the stream
			while (SkipWhitespace(true)) 
			{
				string token = ReadToken();

				if (!string.IsNullOrEmpty(token)) 
				{
					token = token.ToLowerInvariant();

					if (token == "actor") 
					{
						SkipWhitespace(true);
						int startPos = (int)stream.Position;

						List<string> definition = new List<string>();

						do
						{
							token = ReadToken();
							if(string.IsNullOrEmpty(token) || token == "{") break;
							definition.Add(token);
						} while(SkipWhitespace(true));

						string name = "";
						foreach (string s in definition) name += s + " ";
						actors.Add(new ScriptItem(0, name.TrimEnd(), startPos, false));
					}
				}
			}

			//sort nodes
			actors.Sort(ScriptItem.SortByName);
			return true;
		}
	}
}
