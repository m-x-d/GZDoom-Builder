using System;
using System.Collections.Generic;
using System.Text;

using CodeImp.DoomBuilder.Config;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Geometry;
using CodeImp.DoomBuilder.Types;

namespace CodeImp.DoomBuilder.TagExplorer
{
    internal sealed class NodeInfo
    {
        private NodeInfoType type;

        private int index;
        private int action;
        private int tag;
        private string defaultName;

        public int Index { get { return index; } }
        public int Tag { get { return tag; } }
        public int Action { get { return action; } }
        public NodeInfoType Type { get { return type; } }
        public string Comment { get { return getComment(); } set { setComment(value); } }

        //constructor
        public NodeInfo(Thing t) {
            type = NodeInfoType.THING;
            index = t.Index;
            action = t.Action;
            tag = t.Tag;
            ThingTypeInfo tti = General.Map.Data.GetThingInfoEx(t.Type);
            defaultName = (tti != null ? tti.Title : NodeInfoDefaultName.THING);
        }

        public NodeInfo(Sector s) {
            type = NodeInfoType.SECTOR;
            index = s.Index;
            action = s.Effect;
            tag = s.Tag;
        }

        public NodeInfo(Linedef l) {
            type = NodeInfoType.LINEDEF;
            index = l.Index;
            action = l.Action;
            tag = l.Tag;
        }

        //methods
        private UniFields getFields() {
            if (type == NodeInfoType.THING) {
                Thing t = General.Map.Map.GetThingByIndex(index);
                if (t == null) return null;
                return t.Fields;
            }

            if (type == NodeInfoType.SECTOR) {
                Sector s = General.Map.Map.GetSectorByIndex(index);
                if (s == null) return null;
                return s.Fields;
            }

            Linedef l = General.Map.Map.GetLinedefByIndex(index);
            if (l == null) return null;
            return l.Fields;
        }

//comment
        private void setComment(string comment) {
            UniFields fields = getFields();

            if (comment.Length == 0) {
                if (fields.ContainsKey("comment")) {
                    General.Map.UndoRedo.CreateUndo("Remove comment");
                    fields.BeforeFieldsChange();
                    fields.Remove("comment");
                }
                return;
            }

            //create undo stuff
            General.Map.UndoRedo.CreateUndo("Set comment");
            fields.BeforeFieldsChange();

            if (!fields.ContainsKey("comment"))
                fields.Add("comment", new UniValue((int)UniversalType.String, comment));
            else
                fields["comment"].Value = comment;
        }

        private string getComment() {
            UniFields fields = getFields();
            if (fields == null) return "";
            if (!fields.ContainsKey("comment")) return "";
            return fields["comment"].Value.ToString();
        }

//naming
        public string GetName(ref string comment, string sortMode) {
            if (type == NodeInfoType.THING) {
                Thing t = General.Map.Map.GetThingByIndex(index);
                if (t == null) return "<invalid thing>";
                return getThingName(t, ref comment, sortMode);
            }

            if (type == NodeInfoType.SECTOR) {
                Sector s = General.Map.Map.GetSectorByIndex(index);
                if (s == null) return "<invalid sector>";
                return getSectorName(s, ref comment, sortMode);
            }

            Linedef l = General.Map.Map.GetLinedefByIndex(index);
            if (l == null) return "<invalid linedef>";
            return getLinedefName(l, ref comment, sortMode);
        }

        private string getThingName(Thing t, ref string comment, string sortMode) {
            bool isDefaultName = true;
            comment = "";
            if (TagExplorer.UDMF && t.Fields.ContainsKey("comment")) {
                comment = t.Fields["comment"].Value.ToString();
                isDefaultName = false;
            }
            return combineName(comment.Length == 0 ? defaultName : comment, t.Tag, t.Action, t.Index, sortMode, isDefaultName);
        }

        private string getSectorName(Sector s, ref string comment, string sortMode) {
            bool isDefaultName = true;
            comment = "";
            if (TagExplorer.UDMF && s.Fields.ContainsKey("comment")) {
                comment = s.Fields["comment"].Value.ToString();
                isDefaultName = false;
            }
            return combineName(comment.Length == 0 ? NodeInfoDefaultName.SECTOR : comment, s.Tag, s.Effect, s.Index, sortMode, isDefaultName);
        }

        private string getLinedefName(Linedef l, ref string comment, string sortMode) {
            bool isDefaultName = true;
            comment = "";
            if (TagExplorer.UDMF && l.Fields.ContainsKey("comment")) {
                comment = l.Fields["comment"].Value.ToString();
                isDefaultName = false;
            }
            return combineName(comment.Length == 0 ? NodeInfoDefaultName.LINEDEF : comment, l.Tag, l.Action, l.Index, sortMode, isDefaultName);
        }

        private string combineName(string name, int tag, int action, int index, string sortMode, bool isDefaultName) {
            string combinedName = "";
            switch (sortMode) {
                case SortMode.SORT_BY_ACTION:
                    combinedName = (tag > 0 ? "Tag:" + tag + "; " : "") + name + (isDefaultName ? " " + index : "");
                    break;

                case SortMode.SORT_BY_INDEX:
                    combinedName = index + (tag > 0 ? ": Tag:" + tag + "; " : ": ") + (action > 0 ? "Action:" + action + "; " : "") + name;
                    break;

                case SortMode.SORT_BY_TAG:
                    combinedName = (action > 0 ? "Action:" + action + "; " : "") + name + (isDefaultName ? " " + index : "");
                    break;

                default:
                    combinedName = name;
                    break;
            }
            return combinedName;
        }
    }

    internal enum NodeInfoType
    {
        THING,
        SECTOR,
        LINEDEF
    }

    internal struct NodeInfoDefaultName
    {
        public const string THING = "Thing";
        public const string SECTOR = "Sector";
        public const string LINEDEF = "Linedef";
    }

}
