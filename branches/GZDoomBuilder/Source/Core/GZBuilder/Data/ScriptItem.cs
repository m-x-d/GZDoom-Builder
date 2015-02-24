using System;

namespace CodeImp.DoomBuilder.GZBuilder.Data 
{
	internal sealed class ScriptItem : Object 
	{
		private readonly string name;
		private readonly int index;
		private readonly int cursorposition;
		private readonly bool isinclude;

		internal string Name { get { return name; } }
		internal int Index { get { return index; } }
		internal int CursorPosition { get { return cursorposition; } }
		internal bool IsInclude { get { return isinclude; } }

		internal ScriptItem(int index, string name, int cursorPosition, bool isinclude) 
		{
			this.name = name;
			this.index = index;
			this.cursorposition = cursorPosition;
			this.isinclude = isinclude;
		}

		internal ScriptItem(int index, string name) 
		{
			this.name = name;
			this.index = index;
		}

		internal static int SortByIndex(ScriptItem i1, ScriptItem i2) 
		{
			if (i1.isinclude && !i2.isinclude) return 1;
			if (!i1.isinclude && i2.isinclude) return -1;
			if (i1.Index > i2.Index) return 1;
			if (i1.Index == i2.Index) return 0;
			return -1;
		}

		internal static int SortByName(ScriptItem i1, ScriptItem i2)
		{
			if (i1.isinclude && !i2.isinclude) return 1;
			if (!i1.isinclude && i2.isinclude) return -1;
			
			if (i1.Name == i2.Name) return 0;
			if (i1.Name.ToUpper()[0] > i2.Name.ToUpper()[0]) return 1;
			if (i1.Name.ToUpper()[0] == i2.Name.ToUpper()[0]) 
			{
				int len = Math.Min(i1.Name.Length, i2.Name.Length);
				for (int i = 0; i < len; i++) 
				{
					if (i1.Name.ToUpper()[i] > i2.Name.ToUpper()[i]) return 1;
					if (i1.Name.ToUpper()[i] < i2.Name.ToUpper()[i]) return -1;
				}
				if (i1.Name.Length > i2.Name.Length) return 1;
				return -1;
			} 
			return -1;
		}

		public override string ToString() 
		{
			return name;
		}
	}
}
