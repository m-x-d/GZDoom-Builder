using System.Collections.Generic;

namespace CodeImp.DoomBuilder.Config
{
	internal struct ScriptDocumentSettings
	{
		public Dictionary<int, HashSet<int>> FoldLevels; // <fold level, line numbers>
		public int CaretPosition;
		public int FirstVisibleLine;
		public string Filename;
		public bool IsActiveTab;
		public long Hash;
	}
}
