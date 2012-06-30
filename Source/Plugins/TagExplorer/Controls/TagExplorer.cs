using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Types;

namespace CodeImp.DoomBuilder.TagExplorer
{
    public partial class TagExplorer : UserControl
    {
        private const string DISPLAY_TAGS_AND_ACTIONS = "Tags and Actions";
        private const string DISPLAY_TAGS = "Tags";
        private const string DISPLAY_ACTIONS = "Actions";
        private object[] DISPLAY_MODES = new object[] { DISPLAY_TAGS_AND_ACTIONS, DISPLAY_TAGS, DISPLAY_ACTIONS };

        private const string SORT_BY_INDEX = "By Index";
        private const string SORT_BY_TAG = "By Tag";
        private const string SORT_BY_ACTION = "By Action";
        private object[] SORT_MODES = new object[] { SORT_BY_INDEX, SORT_BY_TAG, SORT_BY_ACTION };

        private const string CAT_THINGS = "Things:";
        private const string CAT_SECTORS = "Sectors:";
        private const string CAT_LINEDEFS = "Linedefs:";
        private object[] CATEGORIES = new object[] { CAT_THINGS, CAT_SECTORS, CAT_LINEDEFS };

        private const string defaultThingName = "Thing";
        private const string defaultSectorName = "Sector";
        private const string defaultLinedefName = "Linedef";

        private Color commentColor = Color.DarkMagenta;

        public TagExplorer() {
            InitializeComponent();

            treeView.LabelEdit = true;

            cbDisplayMode.Items.AddRange(DISPLAY_MODES);
            cbDisplayMode.SelectedIndex = General.Settings.ReadPluginSetting("displaymode", 0);
            cbDisplayMode.SelectedIndexChanged += new EventHandler(cbDisplayMode_SelectedIndexChanged);

            cbSortMode.Items.AddRange(SORT_MODES);
            cbSortMode.SelectedIndex = General.Settings.ReadPluginSetting("sortmode", 0);
            cbSortMode.SelectedIndexChanged += new EventHandler(cbSortMode_SelectedIndexChanged);

            cbCenterOnSelected.Checked = General.Settings.ReadPluginSetting("centeronselected", false);
        }

        // Disposer
        protected override void Dispose(bool disposing) {
            General.Settings.WritePluginSetting("sortmode", cbSortMode.SelectedIndex);
            General.Settings.WritePluginSetting("displaymode", cbDisplayMode.SelectedIndex);
            General.Settings.WritePluginSetting("centeronselected", cbCenterOnSelected.Checked);

            if (disposing && (components != null))
                components.Dispose();

            base.Dispose(disposing);
        }

        public void Setup() {
            if (this.ParentForm != null)
                this.ParentForm.Activated += ParentForm_Activated;

            UpdateTree();
        }

        public void UpdateTree() {
            treeView.Nodes.Clear();

            bool showTags = (cbDisplayMode.SelectedItem.ToString() == DISPLAY_TAGS || cbDisplayMode.SelectedItem.ToString() == DISPLAY_TAGS_AND_ACTIONS);
            bool showActions = (cbDisplayMode.SelectedItem.ToString() == DISPLAY_ACTIONS || cbDisplayMode.SelectedItem.ToString() == DISPLAY_TAGS_AND_ACTIONS);
            bool hasComment = false;
            string comment = "";
            string serachStr = tbSearch.Text.ToLowerInvariant();

//add things
            List<TreeNode> nodes = new List<TreeNode>();

            ICollection<Thing> things = General.Map.Map.Things;
            foreach (Thing t in things) {
                if ((t.Tag > 0 && showTags) || (t.Action > 0 && showActions)) {
                    TreeNode node = new TreeNode(getThingName(t, ref hasComment, ref comment), 1, 1);

                    if (serachStr.Length == 0 || (hasComment && comment.ToLowerInvariant().IndexOf(serachStr) != -1)) {
                        node.Tag = new NodeInfo(t.Index, t.Action, t.Tag);
                        if (hasComment) node.ForeColor = commentColor;
                        nodes.Add(node);
                    }
                } 
            }

            //sort nodes
            sort(ref nodes, cbSortMode.SelectedItem.ToString());

            //add category
            if (nodes.Count > 0) {
                treeView.Nodes.Add(new TreeNode(CAT_THINGS, 0, 0, nodes.ToArray()));
            }

//add sectors
            nodes = new List<TreeNode>();
            ICollection<Sector> sectors = General.Map.Map.Sectors;
            foreach (Sector s in sectors) {
                if ((s.Tag > 0 && showTags) || (s.Effect > 0 && showActions)) {
                    TreeNode node = new TreeNode(getSectorName(s, ref hasComment, ref comment), 3, 3);

                    if (serachStr.Length == 0 || (hasComment && comment.ToLowerInvariant().IndexOf(serachStr) != -1)) {
                        node.Tag = new NodeInfo(s.FixedIndex, s.Effect, s.Tag);
                        if (hasComment) node.ForeColor = commentColor;
                        nodes.Add(node);
                    }
                }
            }

            //sort nodes
            sort(ref nodes, cbSortMode.SelectedItem.ToString());

            //add category
            if (nodes.Count > 0)
                treeView.Nodes.Add(new TreeNode(CAT_SECTORS, 2, 2, nodes.ToArray()));

//add linedefs
            nodes = new List<TreeNode>();
            ICollection<Linedef> linedefs = General.Map.Map.Linedefs;
            foreach (Linedef l in linedefs) {
                if ((l.Tag > 0 && showTags) || (l.Action > 0 && showActions)) {
                    TreeNode node = new TreeNode(getLinedefName(l, ref hasComment, ref comment), 5, 5);

                    if (serachStr.Length == 0 || (hasComment && comment.ToLowerInvariant().IndexOf(serachStr) != -1)) {
                        node.Tag = new NodeInfo(l.Index, l.Action, l.Tag);
                        if (hasComment) node.ForeColor = commentColor;
                        nodes.Add(node);
                    }
                }
            }

            //sort nodes
            sort(ref nodes, cbSortMode.SelectedItem.ToString());

            //add category
            if (nodes.Count > 0)
                treeView.Nodes.Add(new TreeNode(CAT_LINEDEFS, 4, 4, nodes.ToArray()));

            treeView.ExpandAll();
        }

//sorting
        private void sort(ref List<TreeNode> nodes, string sortMode) {
            switch (sortMode) {
                case SORT_BY_ACTION:
                    nodes.Sort(sortByAction);
                    break;

                case SORT_BY_INDEX:
                    nodes.Sort(sortByIndex);
                    break;

                case SORT_BY_TAG:
                    nodes.Sort(sortByTag);
                    break;

                default://dbg
                    GZBuilder.GZGeneral.Trace("Got unknown sort mode: " + cbSortMode.SelectedItem.ToString());
                    break;
            }
        }

        private int sortByAction(TreeNode t1, TreeNode t2) {
            NodeInfo i1 = t1.Tag as NodeInfo;
            NodeInfo i2 = t2.Tag as NodeInfo;

            if (i1.Action > i2.Action) return 1;
            else if (i1.Action == i2.Action) return 0;
            else if (i1.Action == 0) return 1; //push items with no action to the end of the list
            else return -1;
        }

        private int sortByTag(TreeNode t1, TreeNode t2) {
            NodeInfo i1 = t1.Tag as NodeInfo;
            NodeInfo i2 = t2.Tag as NodeInfo;

            if (i1.Tag > i2.Tag) return 1;
            else if (i1.Tag == i2.Tag) return 0;
            else if (i1.Tag == 0) return 1; //push items with no tag to the end of the list
            else return -1;
        }

        private int sortByIndex(TreeNode t1, TreeNode t2) {
            NodeInfo i1 = t1.Tag as NodeInfo;
            NodeInfo i2 = t2.Tag as NodeInfo;

            if (i1.Index > i2.Index) return 1;
            else if (i1.Index == i2.Index) return 0;
            return -1;
        }

//naming
        private string getThingName(Thing t, ref bool hasComment, ref string comment) {
            if(GZBuilder.GZGeneral.UDMF && t.Fields.ContainsKey("comment")){
                comment = t.Fields["comment"].Value.ToString();
                hasComment = true;
            }else{
                comment = defaultThingName;
                hasComment = false;
            }

            return combineName(comment, t.Tag, t.Action, t.Index, cbSortMode.SelectedItem.ToString(), false, !hasComment);
        }

        private string getSectorName(Sector s, ref bool hasComment, ref string comment) {
            if(GZBuilder.GZGeneral.UDMF && s.Fields.ContainsKey("comment")){
                comment = s.Fields["comment"].Value.ToString();
                hasComment = true;
            }else{
                comment = defaultSectorName;
                hasComment = false;
            }
            return combineName(comment, s.Tag, s.Effect, s.FixedIndex, cbSortMode.SelectedItem.ToString(), true, !hasComment);
        }

        private string getLinedefName(Linedef l, ref bool hasComment, ref string comment) {
            if(GZBuilder.GZGeneral.UDMF && l.Fields.ContainsKey("comment")){
               comment = l.Fields["comment"].Value.ToString();
               hasComment = true;
            }else{
               comment = defaultLinedefName;
               hasComment = false;
            }
            return combineName(comment, l.Tag, l.Action, l.Index, cbSortMode.SelectedItem.ToString(), false, !hasComment);
        }

        private string combineName(string name, int tag, int action, int index, string sortMode, bool isSector, bool isDefaultName) {
            string combinedName = "";
            switch(sortMode){
                case SORT_BY_ACTION:
                    combinedName = (action > 0 ? (isSector ? "Effect:" : "Action:") + action + "; " : "") + (tag > 0 ? "Tag:" + tag + "; " : "") + name + (isDefaultName ? " " + index : ""); 
                break;

                case SORT_BY_INDEX:
                    combinedName = index + (tag > 0 ? ": Tag:" + tag + "; " : ": ") + (action > 0 ? (isSector ? "Effect:" : "Action:") + action + "; " : "") + name;
                break;

                case SORT_BY_TAG:
                    combinedName = (tag > 0 ? "Tag:" + tag + "; " : "") + (action > 0 ? (isSector ? "Effect:" : "Action:") + action + "; " : "") + name + (isDefaultName ? " " + index : "");
                break;

                default:
                    combinedName = name;
                break;
            }
            return combinedName;
        }

        private void setComment(UniFields fields, string comment) {
            if (!fields.ContainsKey("comment"))
                fields.Add("comment", new UniValue((int)UniversalType.String, comment));
            else
                fields["comment"].Value = comment;
        }

        private string getComment(UniFields fields) {
            if (!fields.ContainsKey("comment"))
                return "";
            else
                return fields["comment"].Value.ToString();
        }

//EVENTS
        private void cbDisplayMode_SelectedIndexChanged(object sender, EventArgs e) {
            UpdateTree();
        }

        private void cbSortMode_SelectedIndexChanged(object sender, EventArgs e) {
            UpdateTree();
        }

        private void treeView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e) {
            if (e.Node.Parent == null) return;

            NodeInfo info = e.Node.Tag as NodeInfo;

            if (e.Button == MouseButtons.Right) { //open element properties
                switch (e.Node.Parent.Text) {
                    case CAT_THINGS:
                        Thing t = General.Map.Map.GetThingByIndex(info.Index);
                        if (t != null) General.Interface.ShowEditThings(new List<Thing>() { t });
                        break;

                    case CAT_SECTORS:
                        Sector s = General.Map.Map.GetSectorByIndex(info.Index);
                        if (s != null) General.Interface.ShowEditSectors(new List<Sector>() { s });
                        break;

                    case CAT_LINEDEFS:
                        Linedef l = General.Map.Map.GetLinedefByIndex(info.Index);
                        if (l != null) General.Interface.ShowEditLinedefs(new List<Linedef>() { l });
                        break;

                    default:
                        GZBuilder.GZGeneral.Trace("Got unknown category: " + e.Node.Parent.Text);
                        break;
                }

                General.Map.Map.Update();
                UpdateTree();

            } else { //focus on element
                if (!cbCenterOnSelected.Checked) return;

                switch (e.Node.Parent.Text) {
                    case CAT_THINGS:
                        Thing t = General.Map.Map.GetThingByIndex(info.Index);
                        if (t != null) General.Map.Renderer2D.PositionView(t.Position.x, t.Position.y);
                        break;

                    case CAT_SECTORS:
                        Sector s = General.Map.Map.GetSectorByIndex(info.Index);
                        if (s != null) General.Map.Renderer2D.PositionView(s.BBox.Location.X + s.BBox.Width / 2, s.BBox.Location.Y + s.BBox.Height / 2);
                        break;

                    case CAT_LINEDEFS:
                        Linedef l = General.Map.Map.GetLinedefByIndex(info.Index);
                        if (l != null) General.Map.Renderer2D.PositionView(l.Rect.Location.X + l.Rect.Width / 2, l.Rect.Location.Y + l.Rect.Height / 2);
                        break;

                    default:
                        GZBuilder.GZGeneral.Trace("Got unknown category: " + e.Node.Parent.Text);
                        break;
                }
                //update view
                General.Interface.RedrawDisplay();
            }
        }

        private void treeView_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e) {
            if (e.Node.Parent == null) return;
            
            //edit comment
            if (GZBuilder.GZGeneral.UDMF) {
                //store current label...
                NodeInfo info = e.Node.Tag as NodeInfo;
                info.Label = e.Node.Text;

                //if we don't have comment - clear text
                switch (e.Node.Parent.Text) {
                    case CAT_THINGS:
                        Thing t = General.Map.Map.GetThingByIndex(info.Index);
                        if (t != null) e.Node.Text = getComment(t.Fields);
                        break;

                    case CAT_SECTORS:
                        Sector s = General.Map.Map.GetSectorByIndex(info.Index);
                        if (s != null) e.Node.Text = getComment(s.Fields);
                        break;

                    case CAT_LINEDEFS:
                        Linedef l = General.Map.Map.GetLinedefByIndex(info.Index);
                        if (l != null) e.Node.Text = getComment(l.Fields);
                        break;

                    default:
                        GZBuilder.GZGeneral.Trace("Got unknown category: " + e.Node.Parent.Text);
                        break;
                }

                //begin editing
                e.Node.BeginEdit();
            }
        }

        //map should be in UDMF format, or we wouldn't be here
        private void treeView_AfterLabelEdit(object sender, NodeLabelEditEventArgs e) {
            NodeInfo info = e.Node.Tag as NodeInfo;
            string comment = "";

            e.CancelEdit = true;
            e.Node.EndEdit(true);

            //to set comment manually we actually have to cancel edit...
            if (e.Label != null && e.Label.Length > 1) {
                bool hasComment = false;

                //apply comment
                switch (e.Node.Parent.Text) {
                    case CAT_THINGS:
                        Thing t = General.Map.Map.GetThingByIndex(info.Index);
                        if (t != null) {
                            setComment(t.Fields, e.Label);
                            e.Node.Text = getThingName(t, ref hasComment, ref comment);
                            e.Node.ForeColor = hasComment ? commentColor : Color.Black;
                        }
                        break;

                    case CAT_SECTORS:
                        Sector s = General.Map.Map.GetSectorByIndex(info.Index);
                        if (s != null) {
                            setComment(s.Fields, e.Label);
                            e.Node.Text = getSectorName(s, ref hasComment, ref comment);
                            e.Node.ForeColor = hasComment ? commentColor : Color.Black;
                        }
                        break;

                    case CAT_LINEDEFS:
                        Linedef l = General.Map.Map.GetLinedefByIndex(info.Index);
                        if (l != null) {
                            setComment(l.Fields, e.Label);
                            e.Node.Text = getLinedefName(l, ref hasComment, ref comment);
                            e.Node.ForeColor = hasComment ? commentColor : Color.Black;
                        }
                        break;

                    default://dbg
                        GZBuilder.GZGeneral.Trace("Got unknown category: " + e.Node.Parent.Text);
                        break;
                }
            } else { //Edit cancelled. Remove comment
                switch (e.Node.Parent.Text) {
                    case CAT_THINGS:
                        Thing t = General.Map.Map.GetThingByIndex(info.Index);
                        if (t != null) {
                            e.Node.Text = defaultThingName;
                            e.Node.ForeColor = Color.Black;
                            if (t.Fields.ContainsKey("comment")) t.Fields.Remove("comment");
                        }
                        break;

                    case CAT_SECTORS:
                        Sector s = General.Map.Map.GetSectorByIndex(info.Index);
                        if (s != null) {
                            e.Node.Text = defaultSectorName;
                            e.Node.ForeColor = Color.Black;
                            if (s.Fields.ContainsKey("comment")) s.Fields.Remove("comment");
                        }
                        break;

                    case CAT_LINEDEFS:
                        Linedef l = General.Map.Map.GetLinedefByIndex(info.Index);
                        if (l != null) {
                            e.Node.Text = defaultLinedefName;
                            e.Node.ForeColor = Color.Black;
                            if (l.Fields.ContainsKey("comment")) l.Fields.Remove("comment");
                        }
                        break;
                }
            }
        }

        //it is called every time a dialog window closes.
		private void ParentForm_Activated(object sender, EventArgs e){
            UpdateTree();
		}

        private void btnClearSearch_Click(object sender, EventArgs e) {
            if (tbSearch.Text != "") {
                tbSearch.Clear();
                UpdateTree();
            }
        }

        private void tbSearch_TextChanged(object sender, EventArgs e) {
            if(tbSearch.Text.Length > 2) UpdateTree();
        }
    }

    internal class NodeInfo
    {
        private int index;
        private int action;
        private int tag;

        public int Index { get { return index; } }
        public int Tag { get { return tag; } }
        public int Action { get { return action; } }
        public string Label; //holds TreeNode text while it's edited

        public NodeInfo(int index, int action, int tag) {
            this.index = index;
            this.action = action;
            this.tag = tag;
        }
    }
}
