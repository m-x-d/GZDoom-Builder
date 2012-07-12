using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeImp.DoomBuilder.GZBuilder.Data {
    internal sealed class ScriptItem : Object {
        private string name;
        private int index;
        private int selectionStart;
        private int selectionEnd;

        internal string Name { get { return name; } }
        internal int Index { get { return index; } }
        internal int SelectionStart { get { return selectionStart; } }
        internal int SelectionEnd { get { return selectionEnd; } }

        internal ScriptItem(int index, string name, int selectionStart, int selectionEnd) {
            this.name = name;
            this.index = index;
            this.selectionStart = selectionStart;
            this.selectionEnd = selectionEnd;
        }

        internal ScriptItem(int index, string name) {
            this.name = name;
            this.index = index;
        }

        internal static int SortByIndex(ScriptItem i1, ScriptItem i2) {
            if (i1.Index > i2.Index) return 1;
            if (i1.Index == i2.Index) return 0;
            return -1;
        }

        internal static int SortByName(ScriptItem i1, ScriptItem i2) {
            if (i1.Name.ToUpper()[0] > i2.Name.ToUpper()[0]) return 1;
            if (i1.Name.ToUpper()[0] == i2.Name.ToUpper()[0]) return 0;
            return -1;
        }

        public override string ToString() {
            return name;
        }
    }
}
