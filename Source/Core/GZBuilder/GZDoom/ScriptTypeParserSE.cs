
#region ================== Namespaces

using CodeImp.DoomBuilder.Config;
using CodeImp.DoomBuilder.Data;
using CodeImp.DoomBuilder.ZDoom;

#endregion

//mxd. Parser used to determine which script type given text is.
namespace CodeImp.DoomBuilder.GZBuilder.GZDoom 
{
	internal sealed class ScriptTypeParserSE :ZDTextParser 
	{
		internal override ScriptType ScriptType { get { return scripttype; } }
		private ScriptType scripttype;

		internal ScriptTypeParserSE() 
		{
			scripttype = ScriptType.UNKNOWN;
		}
		
		public override bool Parse(TextResourceData data, bool clearerrors) 
		{
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
					token = token.ToUpperInvariant();

					if(token == "MODEL") 
					{
						SkipWhitespace(true);
						ReadToken(); //should be model name
						SkipWhitespace(true);
						token = ReadToken();//should be opening brace
						
						if(token == "{") 
						{
							scripttype = ScriptType.MODELDEF;
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
							scripttype = ScriptType.ACS;
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
							scripttype = ScriptType.DECORATE;
							return true;
						}

						SkipWhitespace(true);
						token = ReadToken(); //should be actor name

						if(token == "{") 
						{
							scripttype = ScriptType.DECORATE;
							return true;
						}
					}
				}
			}

			return false;
		}
	}
}
