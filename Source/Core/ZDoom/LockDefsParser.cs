using System.Collections.Generic;
using CodeImp.DoomBuilder.Config;
using CodeImp.DoomBuilder.Data;

namespace CodeImp.DoomBuilder.ZDoom
{
	internal sealed class LockDefsParser :ZDTextParser
	{
		internal override ScriptType ScriptType { get { return ScriptType.LOCKDEFS; } }

		private Dictionary<int, string> locks;

		public LockDefsParser()
		{
			locks = new Dictionary<int, string>();
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
			int locknum = -1;
			string locktitle = string.Empty;
			string game = string.Empty;
			int bracelevel = 0;

			while(SkipWhitespace(true))
			{
				string token = ReadToken().ToLowerInvariant();
				if(string.IsNullOrEmpty(token)) continue;

				switch(token)
				{
					case "clearlocks":
						if(bracelevel == 0)
						{
							locks.Clear();
						}
						else
						{
							ReportError("Unexpected \"CLEARLOCKS\" keyword");
							return false;
						}
						break;

					// LOCK locknumber [game]
					case "lock":
						SkipWhitespace(false);
						if(!ReadSignedInt(ref locknum))
						{
							ReportError("Expected lock number");
							return false;
						}

						//wiki: The locknumber must be in the 1-to-255 range
						if(locknum < 1 || locknum > 255)
						{
							ReportError("The locknumber must be in the 1-to-255 range, but is " + locknum);
							return false;
						}

						SkipWhitespace(true);
						token = ReadToken().ToLowerInvariant();
						if(!string.IsNullOrEmpty(token))
						{
							if(token == "{")
							{
								bracelevel++;
							}
							//Should be game
							else if(!GameType.GameTypes.Contains(token))
							{
								LogWarning("Lock " + locknum + " is defined for unknown game \"" + token + "\"");
							}
							else
							{
								game = token;
							}
						}
						break;

					case "$title":
						SkipWhitespace(false);
						locktitle = StripQuotes(ReadToken(false));
						break;

					case "{":
						bracelevel++;
						break;

					case "}":
						if(--bracelevel > 0) continue;

						// Add to collection?
						if(locknum > 0 && (string.IsNullOrEmpty(game) || General.Map.Config.BaseGame == game))
						{
							// No custom title given?
							if(string.IsNullOrEmpty(locktitle)) locktitle = "Lock " + locknum;

							if(locks.ContainsKey(locknum))
								LogWarning("Lock " + locknum + " is double-defined as \"" + locks[locknum] + "\" and \"" + locktitle + "\"");

							locks[locknum] = locktitle;
						}

						// Reset values
						locknum = -1;
						locktitle = string.Empty;
						game = string.Empty;
						break;
				}
			}


			return true;
		}

		public EnumList GetLockDefs()
		{
			EnumList result = new EnumList(locks.Count);

			foreach(KeyValuePair<int, string> group in locks)
				result.Add(new EnumItem(group.Key.ToString(), group.Value));

			return result;
		}
	}
}
