#region ================== Namespaces

using System.Collections.Generic;
using CodeImp.DoomBuilder.Config;
using CodeImp.DoomBuilder.Data;

#endregion

namespace CodeImp.DoomBuilder.ZDoom
{
	internal sealed class SndInfoParser : ZDTextParser
	{
		#region ================== Variables

		private Dictionary<int, string> ambientsounds;

		#endregion

		#region ================== Properties

		internal override ScriptType ScriptType { get { return ScriptType.SNDINFO; } }

		internal Dictionary<int, string> AmbientSounds { get { return ambientsounds; } }

		#endregion

		#region ================== Constructor

		public SndInfoParser()
		{
			specialtokens = "";
			ambientsounds = new Dictionary<int, string>();
			skipeditorcomments = true; // otherwise //$AMBIENT will be treated like one...
		}

		#endregion

		#region ================== Parsing

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
				string token = ReadToken().ToLowerInvariant();
				if(string.IsNullOrEmpty(token)) continue;

				switch(token)
				{
					//$ambient <index> <logicalsound> [type] <mode> <volume>
					case "$ambient":
						// Read index
						SkipWhitespace(true);
						int index = -1;
						if(!ReadSignedInt(ref index) || index < 0)
						{
							// Not numeric!
							ReportError("Expected ambient sound index");
							return false;
						}

						// Read name
						SkipWhitespace(true);
						string logicalsound = StripQuotes(ReadToken(false));
						if(string.IsNullOrEmpty(logicalsound))
						{
							ReportError("Expected ambient sound logicalname");
							return false;
						}

						// Add to collection
						if(ambientsounds.ContainsKey(index))
							LogWarning("Ambient sound " + index + " is double-defined as \"" + ambientsounds[index] + "\" and \"" + logicalsound + "\"");

						ambientsounds[index] = logicalsound;
						break;
				}
			}

			return true;
		}

		#endregion
	}
}
