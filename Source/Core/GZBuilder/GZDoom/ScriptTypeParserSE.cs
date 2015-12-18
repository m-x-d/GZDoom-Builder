#region ================== Namespaces

using System.IO;
using CodeImp.DoomBuilder.Config;
using CodeImp.DoomBuilder.ZDoom;

#endregion

//mxd. Parser used to determine which script type given text is.
namespace CodeImp.DoomBuilder.GZBuilder.GZDoom 
{
	internal sealed class ScriptTypeParserSE :ZDTextParser 
	{
		private ScriptType scriptType;
		internal ScriptType ScriptType { get { return scriptType; } }

		internal ScriptTypeParserSE() 
		{
			scriptType = ScriptType.UNKNOWN;
		}
		
		public override bool Parse(Stream stream, string sourcefilename, bool clearerrors) 
		{
			if(!base.Parse(stream, sourcefilename, clearerrors)) return false;

			// Continue until at the end of the stream
			while(SkipWhitespace(true)) 
			{
				string token = ReadToken();

				if(!string.IsNullOrEmpty(token)) 
				{
					token = token.ToUpperInvariant();

					if(token == "MODEL") 
					{
						SkipWhitespace(true);
						ReadToken(); //should be model name
						SkipWhitespace(true);
						token = ReadToken();//should be opening brace
						
						if(token == "{") 
						{
							scriptType = ScriptType.MODELDEF;
							return true;
						}

					}
					else if(token == "SCRIPT")
					{
						SkipWhitespace(true);
						ReadToken(); //should be script name or number
						SkipWhitespace(true);
						ReadToken(); //should be script parameters/type
						SkipWhitespace(true);
						token = ReadToken(); //should be opening brace
						
						if(token == "{") 
						{
							scriptType = ScriptType.ACS;
							return true;
						}

					}
					else if(token == "ACTOR")
					{
						SkipWhitespace(true);
						ReadToken(); //should be actor name

						SkipWhitespace(true);
						token = ReadToken();

						if(token == ":" || token == "{" || token == "REPLACES") 
						{
							scriptType = ScriptType.DECORATE;
							return true;
						}

						SkipWhitespace(true);
						token = ReadToken(); //should be actor name

						if(token == "{") 
						{
							scriptType = ScriptType.DECORATE;
							return true;
						}
					}
				}
			}

			return false;
		}

		protected override string GetLanguageType()
		{
			return "SCRIPT TYPE CHECKER";
		}
	}
}
