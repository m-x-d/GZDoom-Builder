using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeImp.DoomBuilder.GZBuilder.Data {
    public sealed class ScriptItem : Object {
        private string name;
        private int index;
        private int selectionStart;
        private int selectionEnd;

        public string Name { get { return name; } }
        public int Index { get { return index; } }
        public int SelectionStart { get { return selectionStart; } }
        public int SelectionEnd { get { return selectionEnd; } }

        public ScriptItem(int index, string name, int selectionStart, int selectionEnd) {
            this.name = name;
            this.index = index;
            this.selectionStart = selectionStart;
            this.selectionEnd = selectionEnd;
        }

        public ScriptItem(int index, string name) {
            this.name = name;
            this.index = index;
        }

        public override string ToString() {
            return name;
        }
    }
}
