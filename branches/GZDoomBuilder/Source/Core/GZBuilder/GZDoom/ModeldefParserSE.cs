using System.IO;
using System.Collections.Generic;
using CodeImp.DoomBuilder.ZDoom;
using CodeImp.DoomBuilder.GZBuilder.Data;

//mxd. Modeldef parser used to create ScriptItems for use in script editor's navigator
namespace CodeImp.DoomBuilder.GZBuilder.GZDoom {
	internal sealed class ModeldefParserSE : ZDTextParser {
		private List<ScriptItem> models;
		internal List<ScriptItem> Models { get { return models; } }

		public ModeldefParserSE() {
			models = new List<ScriptItem>();
		}

		public override bool Parse(Stream stream, string sourcefilename) {
			base.Parse(stream, sourcefilename);

			// Continue until at the end of the stream
			while (SkipWhitespace(true)) {
				string token = ReadToken();

				if (!string.IsNullOrEmpty(token)) {
					token = token.ToUpperInvariant();

					if(token == "MODEL"){
						int startPos = (int)stream.Position - 6;
						SkipWhitespace(true);
						string modelName = ReadToken();
						SkipWhitespace(true);
						token = ReadToken(); //this should be "{"

						if (token == "{") {
							ScriptItem i = new ScriptItem(0, modelName, startPos, (int)stream.Position - 2);
							models.Add(i);
						}
					}
				}
			}

			//sort nodes
			models.Sort(ScriptItem.SortByName);
			return true;
		}
	}
}
