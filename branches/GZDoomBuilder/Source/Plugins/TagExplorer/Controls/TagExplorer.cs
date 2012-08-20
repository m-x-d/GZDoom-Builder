using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using CodeImp.DoomBuilder.Config;
using CodeImp.DoomBuilder.Geometry;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Editing;
using CodeImp.DoomBuilder.Types;

namespace CodeImp.DoomBuilder.TagExplorer
{
    public sealed partial class TagExplorer : UserControl
    {
        private const string DISPLAY_TAGS_AND_ACTIONS = "Tags and Action specials";
        private const string DISPLAY_TAGS = "Tags";
        private const string DISPLAY_ACTIONS = "Action specials";
        private readonly object[] DISPLAY_MODES = new object[] { DISPLAY_TAGS_AND_ACTIONS, DISPLAY_TAGS, DISPLAY_ACTIONS };
        
        private string currentDisplayMode;
        private string currentSortMode;

        private const string CAT_THINGS = "Things:";
        private const string CAT_SECTORS = "Sectors:";
        private const string CAT_LINEDEFS = "Linedefs:";

        private Color commentColor = Color.DarkMagenta;
        private SelectedNode selection;

        private static bool udmf;
        internal static bool UDMF { get { return udmf; } } 

        public TagExplorer() {
            InitializeComponent();

            selection = new SelectedNode();

            cbDisplayMode.Items.AddRange(DISPLAY_MODES);
            cbDisplayMode.SelectedIndex = General.Settings.ReadPluginSetting("displaymode", 0);
            cbDisplayMode.SelectedIndexChanged += new EventHandler(cbDisplayMode_SelectedIndexChanged);
            currentDisplayMode = cbDisplayMode.SelectedItem.ToString();

            cbSortMode.Items.AddRange(SortMode.SORT_MODES);
            cbSortMode.SelectedIndex = General.Settings.ReadPluginSetting("sortmode", 0);
            cbSortMode.SelectedIndexChanged += new EventHandler(cbSortMode_SelectedIndexChanged);
            currentSortMode = cbSortMode.SelectedItem.ToString();

            cbCenterOnSelected.Checked = General.Settings.ReadPluginSetting("centeronselected", false);
            cbSelectOnClick.Checked = General.Settings.ReadPluginSetting("doselect", false);

            udmf = (General.Map.Config.FormatInterface == "UniversalMapSetIO");

            if (udmf) {
                cbCommentsOnly.Checked = General.Settings.ReadPluginSetting("commentsonly", false);
                treeView.LabelEdit = true;
                toolTip1.SetToolTip(tbSearch, "Enter text to find comment\r\nEnter # + tag number to show only specified tag. Example: #667\r\nEnter $ + effect number to show only specified effect. Example: $80");
                toolTip1.SetToolTip(treeView, "Double-click item to edit item's comment\r\nRight-click item to open item's Properties");
            } else {
                cbCommentsOnly.Enabled = false;
                toolTip1.SetToolTip(tbSearch, "Enter # + tag number to show only specified tag. Example: #667\r\nEnter $ + effect number to show only specified effect. Example: $80");
                toolTip1.SetToolTip(treeView, "Right-click item to open item's Properties");
            }
        }

        // Disposer
        protected override void Dispose(bool disposing) {
            General.Settings.WritePluginSetting("sortmode", cbSortMode.SelectedIndex);
            General.Settings.WritePluginSetting("displaymode", cbDisplayMode.SelectedIndex);
            General.Settings.WritePluginSetting("centeronselected", cbCenterOnSelected.Checked);
            General.Settings.WritePluginSetting("doselect", cbSelectOnClick.Checked);

            if (udmf) General.Settings.WritePluginSetting("commentsonly", cbCommentsOnly.Checked);
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        public void Setup() {
            if (this.ParentForm != null) this.ParentForm.Activated += ParentForm_Activated;
            updateTree(true);
        }

        public void Terminate() {
            if (this.ParentForm != null) this.ParentForm.Activated -= ParentForm_Activated;
        }

        // This sets the timer to update the list very soon (because we'll have problems if we just call updateTree now)
        public void UpdateTreeSoon() {
            updatetimer.Stop();
            updatetimer.Start();
        }

        private void updateTree(bool focusDisplay) {
            bool showTags = (currentDisplayMode == DISPLAY_TAGS || currentDisplayMode == DISPLAY_TAGS_AND_ACTIONS);
            bool showActions = (currentDisplayMode == DISPLAY_ACTIONS || currentDisplayMode == DISPLAY_TAGS_AND_ACTIONS);
            bool hasComment = false;
            string comment = "";
            string serachStr = serachStr = tbSearch.Text.ToLowerInvariant();

            int filteredTag = -1;
            int filteredAction = -1;
            getSpecialValues(serachStr, ref filteredTag, ref filteredAction);

            if (!udmf || filteredTag != -1 || filteredAction != -1) serachStr = "";

            TreeNode selectedNode = null;

            this.SuspendLayout();
            treeView.Nodes.Clear();

//add things
            List<TreeNode> nodes = new List<TreeNode>();
            ICollection<Thing> things = General.Map.Map.Things;

            if (!(things is MapElementCollection<Linedef>)) { //don't want to enumerate when array is locked
                foreach (Thing t in things) {
                    if ((showTags && t.Tag > 0) || (showActions && t.Action > 0)) {
                        if (filteredTag != -1 && t.Tag != filteredTag)
                            continue;
                        if (filteredAction != -1 && t.Action != filteredAction)
                            continue;

                        NodeInfo info = new NodeInfo(t);
                        string name = info.GetName(ref comment, currentSortMode);
                        hasComment = comment.Length > 0;

                        if (!hasComment && cbCommentsOnly.Checked)
                            continue;

                        if (!udmf || serachStr.Length == 0 || (hasComment && comment.ToLowerInvariant().IndexOf(serachStr) != -1)) {
                            TreeNode node = new TreeNode(name, 1, 1);
                            node.Tag = info;
                            if (hasComment) node.ForeColor = commentColor;
                            nodes.Add(node);

                            if (info.Index == selection.Index && info.Type == selection.Type)
                                selectedNode = node;
                        }
                    }
                }

                //sort nodes
                sort(ref nodes, currentSortMode);

                //add "things" category
                if (nodes.Count > 0) {
                    if (currentSortMode == SortMode.SORT_BY_ACTION) { //create action categories
                        Dictionary<int, TreeNode> categories = new Dictionary<int, TreeNode>();
                        TreeNode noAction = new TreeNode("No Action", 0, 0);

                        foreach (TreeNode node in nodes) {
                            NodeInfo nodeInfo = node.Tag as NodeInfo;

                            if (nodeInfo.Action == 0) {
                                noAction.Nodes.Add(node);
                                continue;
                            }

                            LinedefActionInfo lai = General.Map.Config.GetLinedefActionInfo(nodeInfo.Action);

                            if (!categories.ContainsKey(lai.Index))
                                categories.Add(lai.Index, new TreeNode(lai.Index + " - " + lai.Name, 0, 0, new TreeNode[] { node }));
                            else
                                categories[lai.Index].Nodes.Add(node);
                        }

                        TreeNode[] catNodes = new TreeNode[categories.Values.Count];
                        categories.Values.CopyTo(catNodes, 0);

                        TreeNode category = new TreeNode(CAT_THINGS, 0, 0, catNodes);
                        if (noAction.Nodes.Count > 0)
                            category.Nodes.Add(noAction);

                        treeView.Nodes.Add(category);

                    } else if (currentSortMode == SortMode.SORT_BY_INDEX) { //create thing categories
                        Dictionary<string, TreeNode> categories = new Dictionary<string, TreeNode>();
                        foreach (TreeNode node in nodes) {
                            NodeInfo nodeInfo = node.Tag as NodeInfo;
                            ThingTypeInfo tti = General.Map.Data.GetThingInfoEx(General.Map.Map.GetThingByIndex(nodeInfo.Index).Type);

                            if (tti != null) {
                                if (!categories.ContainsKey(tti.Category.Title))
                                    categories.Add(tti.Category.Title, new TreeNode(tti.Category.Title, 0, 0, new TreeNode[] { node }));
                                else
                                    categories[tti.Category.Title].Nodes.Add(node);
                            } else {
                                if (!categories.ContainsKey("UNKNOWN"))
                                    categories.Add("UNKNOWN", new TreeNode("UNKNOWN", 0, 0, new TreeNode[] { node }));
                                else
                                    categories["UNKNOWN"].Nodes.Add(node);
                            }
                        }
                        TreeNode[] catNodes = new TreeNode[categories.Values.Count];
                        categories.Values.CopyTo(catNodes, 0);

                        treeView.Nodes.Add(new TreeNode(CAT_THINGS, 0, 0, catNodes));

                    }
                    else { //sort by tag
                        Dictionary<int, TreeNode> categories = new Dictionary<int, TreeNode>();
                        TreeNode noTag = new TreeNode("No Tag", 0, 0);

                        foreach (TreeNode node in nodes) {
                            NodeInfo nodeInfo = node.Tag as NodeInfo;

                            if (nodeInfo.Tag == 0) {
                                noTag.Nodes.Add(node);
                                continue;
                            }

                            if (!categories.ContainsKey(nodeInfo.Tag))
                                categories.Add(nodeInfo.Tag, new TreeNode("Tag " + nodeInfo.Tag, 0, 0, new TreeNode[] { node }));
                            else
                                categories[nodeInfo.Tag].Nodes.Add(node);
                        }

                        TreeNode[] catNodes = new TreeNode[categories.Values.Count];
                        categories.Values.CopyTo(catNodes, 0);

                        TreeNode category = new TreeNode(CAT_THINGS, 0, 0, catNodes);
                        if (noTag.Nodes.Count > 0)
                            category.Nodes.Add(noTag);

                        treeView.Nodes.Add(category);
                    }
                }
            }

//add sectors
            nodes = new List<TreeNode>();
            ICollection<Sector> sectors = General.Map.Map.Sectors;

            if (!(sectors is MapElementCollection<Linedef>)) { //don't want to enumerate when array is locked
                foreach (Sector s in sectors) {
                    if ((showTags && s.Tag > 0) || (showActions && s.Effect > 0)) {
                        if (filteredTag != -1 && s.Tag != filteredTag)
                            continue;
                        if (filteredAction != -1 && s.Effect != filteredAction)
                            continue;

                        NodeInfo info = new NodeInfo(s);
                        string name = info.GetName(ref comment, currentSortMode);
                        hasComment = comment.Length > 0;

                        if (!hasComment && cbCommentsOnly.Checked)
                            continue;

                        if (!udmf || serachStr.Length == 0 || (hasComment && comment.ToLowerInvariant().IndexOf(serachStr) != -1)) {
                            TreeNode node = new TreeNode(name, 3, 3);
                            node.Tag = info;
                            if (hasComment) node.ForeColor = commentColor;
                            nodes.Add(node);

                            if (info.Index == selection.Index && info.Type == selection.Type)
                                selectedNode = node;
                        }
                    }
                }

                //sort nodes
                sort(ref nodes, currentSortMode);

                //add category
                if (nodes.Count > 0) {
                    if (currentSortMode == SortMode.SORT_BY_ACTION) {
                        Dictionary<int, TreeNode> categories = new Dictionary<int, TreeNode>();
                        TreeNode noAction = new TreeNode("No Effect", 2, 2);

                        foreach (TreeNode node in nodes) {
                            NodeInfo nodeInfo = node.Tag as NodeInfo;

                            if (nodeInfo.Action == 0) {
                                noAction.Nodes.Add(node);
                                continue;
                            }

                            SectorEffectInfo sei = General.Map.Config.GetSectorEffectInfo(nodeInfo.Action);

                            if (!categories.ContainsKey(sei.Index))
                                categories.Add(sei.Index, new TreeNode(sei.Index + " - " + sei.Title, 2, 2, new TreeNode[] { node }));
                            else
                                categories[sei.Index].Nodes.Add(node);
                        }
                        TreeNode[] catNodes = new TreeNode[categories.Values.Count];
                        categories.Values.CopyTo(catNodes, 0);

                        TreeNode category = new TreeNode(CAT_SECTORS, 2, 2, catNodes);
                        if (noAction.Nodes.Count > 0)
                            category.Nodes.Add(noAction);

                        treeView.Nodes.Add(category);
                    } else if (currentSortMode == SortMode.SORT_BY_TAG) {
                        Dictionary<int, TreeNode> categories = new Dictionary<int, TreeNode>();
                        TreeNode noTag = new TreeNode("No Tag", 2, 2);

                        foreach (TreeNode node in nodes) {
                            NodeInfo nodeInfo = node.Tag as NodeInfo;

                            if (nodeInfo.Tag == 0) {
                                noTag.Nodes.Add(node);
                                continue;
                            }

                            if (!categories.ContainsKey(nodeInfo.Tag))
                                categories.Add(nodeInfo.Tag, new TreeNode("Tag " + nodeInfo.Tag, 2, 2, new TreeNode[] { node }));
                            else
                                categories[nodeInfo.Tag].Nodes.Add(node);
                        }
                        TreeNode[] catNodes = new TreeNode[categories.Values.Count];
                        categories.Values.CopyTo(catNodes, 0);

                        TreeNode category = new TreeNode(CAT_SECTORS, 2, 2, catNodes);
                        if (noTag.Nodes.Count > 0)
                            category.Nodes.Add(noTag);

                        treeView.Nodes.Add(category);
                    } else {//just add them as they are
                        treeView.Nodes.Add(new TreeNode(CAT_SECTORS, 2, 2, nodes.ToArray()));
                    }
                }
            }

//add linedefs
            nodes = new List<TreeNode>();
            ICollection<Linedef> linedefs = General.Map.Map.Linedefs;

            if (!(linedefs is MapElementCollection<Linedef>)) { //don't want to enumerate when array is locked
                foreach (Linedef l in linedefs) {
                    if ((showTags && l.Tag > 0) || (showActions && l.Action > 0)) {
                        if (filteredTag != -1 && l.Tag != filteredTag)
                            continue;
                        if (filteredAction != -1 && l.Action != filteredAction)
                            continue;

                        NodeInfo info = new NodeInfo(l);
                        string name = info.GetName(ref comment, currentSortMode);
                        hasComment = comment.Length > 0;

                        if (!hasComment && cbCommentsOnly.Checked)
                            continue;

                        if (!udmf || serachStr.Length == 0 || (hasComment && comment.ToLowerInvariant().IndexOf(serachStr) != -1)) {
                            TreeNode node = new TreeNode(name, 5, 5);
                            node.Tag = info;
                            if (hasComment) node.ForeColor = commentColor;
                            nodes.Add(node);

                            if (info.Index == selection.Index && info.Type == selection.Type)
                                selectedNode = node;
                        }
                    }
                }

                //sort nodes
                sort(ref nodes, currentSortMode);

                //add category
                if (nodes.Count > 0) {
                    if (currentSortMode == SortMode.SORT_BY_ACTION) {
                        Dictionary<int, TreeNode> categories = new Dictionary<int, TreeNode>();
                        TreeNode noAction = new TreeNode("No Action", 4, 4);

                        foreach (TreeNode node in nodes) {
                            NodeInfo nodeInfo = node.Tag as NodeInfo;

                            if (nodeInfo.Action == 0) {
                                noAction.Nodes.Add(node);
                                continue;
                            }

                            LinedefActionInfo lai = General.Map.Config.GetLinedefActionInfo(nodeInfo.Action);

                            if (!categories.ContainsKey(lai.Index))
                                categories.Add(lai.Index, new TreeNode(lai.Index + " - " + lai.Name, 4, 4, new TreeNode[] { node }));
                            else
                                categories[lai.Index].Nodes.Add(node);
                        }
                        TreeNode[] catNodes = new TreeNode[categories.Values.Count];
                        categories.Values.CopyTo(catNodes, 0);

                        TreeNode category = new TreeNode(CAT_LINEDEFS, 4, 4, catNodes);
                        if (noAction.Nodes.Count > 0)
                            category.Nodes.Add(noAction);

                        treeView.Nodes.Add(category);

                    } else if (currentSortMode == SortMode.SORT_BY_TAG) {
                        Dictionary<int, TreeNode> categories = new Dictionary<int, TreeNode>();
                        TreeNode noTag = new TreeNode("No Tag", 4, 4);

                        foreach (TreeNode node in nodes) {
                            NodeInfo nodeInfo = node.Tag as NodeInfo;

                            if (nodeInfo.Tag == 0) {
                                noTag.Nodes.Add(node);
                                continue;
                            }

                            if (!categories.ContainsKey(nodeInfo.Tag))
                                categories.Add(nodeInfo.Tag, new TreeNode("Tag " + nodeInfo.Tag, 4, 4, new TreeNode[] { node }));
                            else
                                categories[nodeInfo.Tag].Nodes.Add(node);
                        }
                        TreeNode[] catNodes = new TreeNode[categories.Values.Count];
                        categories.Values.CopyTo(catNodes, 0);

                        TreeNode category = new TreeNode(CAT_LINEDEFS, 4, 4, catNodes);
                        if (noTag.Nodes.Count > 0)
                            category.Nodes.Add(noTag);

                        treeView.Nodes.Add(category);
                    } else { //just add them as they are
                        treeView.Nodes.Add(new TreeNode(CAT_LINEDEFS, 4, 4, nodes.ToArray()));
                    }
                }
            }

            //expand top level nodes
            foreach (TreeNode t in treeView.Nodes)
                t.Expand();

            if (selectedNode != null)
                treeView.SelectedNode = selectedNode;
            else if (treeView.Nodes.Count > 0)
                treeView.SelectedNode = treeView.Nodes[0];

            this.ResumeLayout();

            //loose focus
            if(focusDisplay) General.Interface.FocusDisplay();
        }

//tag/action search
        private void getSpecialValues(string serachStr, ref int filteredTag, ref int filteredAction) {
            if (serachStr.Length == 0) return;

            int pos = serachStr.IndexOf("#");
            if (pos != -1)
                filteredTag = readNumber(serachStr, pos+1);

            pos = serachStr.IndexOf("$");
            if (pos != -1)
                filteredAction = readNumber(serachStr, pos+1);
        }

        private int readNumber(string serachStr, int startPoition) {
            string token = "";
            int pos = startPoition;

            while (pos < serachStr.Length && "1234567890".IndexOf(serachStr[pos]) != -1) {
                token += serachStr[pos];
                pos++;
            }

            if (token.Length > 0) {
                int result = -1;
                if(int.TryParse(token, NumberStyles.Integer, CultureInfo.InvariantCulture, out result))
                    return result;
            }

            return -1;
        }

//sorting
        private void sort(ref List<TreeNode> nodes, string sortMode) {
            if(sortMode == SortMode.SORT_BY_ACTION)
                nodes.Sort(sortByAction);
            else if (sortMode == SortMode.SORT_BY_TAG)
                nodes.Sort(sortByTag);
            else
                nodes.Sort(sortByIndex);
        }

        private int sortByAction(TreeNode t1, TreeNode t2) {
            NodeInfo i1 = t1.Tag as NodeInfo;
            NodeInfo i2 = t2.Tag as NodeInfo;

            if (i1.Action == i2.Action) return 0;
            if (i1.Action == 0) return 1; //push items with no action to the end of the list
            if (i2.Action == 0) return -1; //push items with no action to the end of the list
            if (i1.Action > i2.Action) return 1;
            return -1; //should be i1 < i2
        }

        private int sortByTag(TreeNode t1, TreeNode t2) {
            NodeInfo i1 = t1.Tag as NodeInfo;
            NodeInfo i2 = t2.Tag as NodeInfo;

            if (i1.Tag == i2.Tag) return 0;
            if (i1.Tag == 0) return 1; //push items with no tag to the end of the list
            if (i2.Tag == 0) return -1; //push items with no tag to the end of the list
            if (i1.Tag > i2.Tag) return 1;
            return -1; //should be i1 < i2
        }

        private int sortByIndex(TreeNode t1, TreeNode t2) {
            NodeInfo i1 = t1.Tag as NodeInfo;
            NodeInfo i2 = t2.Tag as NodeInfo;

            if (i1.Index > i2.Index) return 1;
            if (i1.Index == i2.Index) return 0;
            return -1;
        }

//EVENTS
        private void cbDisplayMode_SelectedIndexChanged(object sender, EventArgs e) {
            currentDisplayMode = cbDisplayMode.SelectedItem.ToString();
            updateTree(true);
        }

        private void cbSortMode_SelectedIndexChanged(object sender, EventArgs e) {
            currentSortMode = cbSortMode.SelectedItem.ToString();
            updateTree(true);
        }

        private void treeView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e) {
            NodeInfo info = e.Node.Tag as NodeInfo;
            if (info == null) return;

            //store selection
            selection.Type = info.Type;
            selection.Index = info.Index;

            if (e.Button == MouseButtons.Right) { //open element properties
                switch (info.Type) {
                    case NodeInfoType.THING:
                        Thing t = General.Map.Map.GetThingByIndex(info.Index);
                        if (t != null) General.Interface.ShowEditThings(new List<Thing>() { t });
                        break;

                    case NodeInfoType.SECTOR:
                        Sector s = General.Map.Map.GetSectorByIndex(info.Index);
                        if (s != null) General.Interface.ShowEditSectors(new List<Sector>() { s });
                        break;

                    case NodeInfoType.LINEDEF:
                        Linedef l = General.Map.Map.GetLinedefByIndex(info.Index);
                        if (l != null) General.Interface.ShowEditLinedefs(new List<Linedef>() { l });
                        break;

                    default:
                        General.ErrorLogger.Add(ErrorType.Warning, "Tag Explorer: got unknown category: " + info.Type);
                        break;
                }

                General.Map.Map.Update();
                updateTree(true);

            } else {
                //select element?
                if (cbSelectOnClick.Checked) {
                    // Leave any volatile mode
                    General.Editing.CancelVolatileMode();
                    General.Map.Map.ClearAllSelected();

                    //make selection
                    if (info.Type == NodeInfoType.THING) {
                        General.Editing.ChangeMode("ThingsMode");
                        Thing t = General.Map.Map.GetThingByIndex(info.Index);
                        if (t != null) t.Selected = true;
                    } else if (info.Type == NodeInfoType.LINEDEF) {
                        General.Editing.ChangeMode("LinedefsMode");
                        Linedef l = General.Map.Map.GetLinedefByIndex(info.Index);
                        if (l != null) l.Selected = true;
                    } else {
                        General.Editing.ChangeMode("SectorsMode");
                        Sector s = General.Map.Map.GetSectorByIndex(info.Index);
                        if (s != null) {
                            s.Selected = true;

                            foreach (Sidedef sd in s.Sidedefs)
                                sd.Line.Selected = true;
                        }
                    }
                }

                //focus on element?
                if (cbCenterOnSelected.Checked) {
                    List<Vector2D> points = new List<Vector2D>();
                    RectangleF area = MapSet.CreateEmptyArea();

                    if (info.Type == NodeInfoType.LINEDEF) {
                        Linedef l = General.Map.Map.GetLinedefByIndex(info.Index);
                        points.Add(l.Start.Position);
                        points.Add(l.End.Position);
                    } else if (info.Type == NodeInfoType.SECTOR) {
                        Sector s = General.Map.Map.GetSectorByIndex(info.Index);
                        foreach (Sidedef sd in s.Sidedefs) {
                            points.Add(sd.Line.Start.Position);
                            points.Add(sd.Line.End.Position);
                        }
                    } else if (info.Type == NodeInfoType.THING) {
                        Thing t = General.Map.Map.GetThingByIndex(info.Index);
                        Vector2D p = (Vector2D)t.Position;
                        points.Add(p);
                        points.Add(p + new Vector2D(t.Size * 2.0f, t.Size * 2.0f));
                        points.Add(p + new Vector2D(t.Size * 2.0f, -t.Size * 2.0f));
                        points.Add(p + new Vector2D(-t.Size * 2.0f, t.Size * 2.0f));
                        points.Add(p + new Vector2D(-t.Size * 2.0f, -t.Size * 2.0f));
                    } else {
                        General.Fail("Tag Explorer: unknown object type given to zoom in on!");
                    }

                    // Make a view area from the points
                    foreach (Vector2D p in points) area = MapSet.IncreaseArea(area, p);

                    // Make the area square, using the largest side
                    if (area.Width > area.Height) {
                        float delta = area.Width - area.Height;
                        area.Y -= delta * 0.5f;
                        area.Height += delta;
                    } else {
                        float delta = area.Height - area.Width;
                        area.X -= delta * 0.5f;
                        area.Width += delta;
                    }

                    // Add padding
                    area.Inflate(100f, 100f);

                    // Zoom to area
                    ClassicMode editmode = (General.Editing.Mode as ClassicMode);
                    editmode.CenterOnArea(area, 0.6f);
                }

                //update view
                General.Interface.RedrawDisplay();
            }
        }

        private void treeView_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e) {
            //edit comment
            if (udmf) {
                NodeInfo info = e.Node.Tag as NodeInfo;
                if (info == null) return;

                e.Node.Text = info.Comment; //set node text to comment
                e.Node.BeginEdit(); //begin editing
            }
        }

        //we don't want to edit categories if we are in UDMF
        private void treeView_BeforeLabelEdit(object sender, NodeLabelEditEventArgs e) {
            if (!udmf || e.Node.Tag == null) {
                e.CancelEdit = true;
                return;
            }
            treeView.MouseLeave -= treeView_MouseLeave;
        }

        //map should be in UDMF format, or we wouldn't be here
        private void treeView_AfterLabelEdit(object sender, NodeLabelEditEventArgs e) {
            NodeInfo info = e.Node.Tag as NodeInfo;
            string comment = "";

            //to set comment manually we actually have to cancel edit...
            e.CancelEdit = true;
            e.Node.EndEdit(true);

            if (e.Label != null && e.Label.Length > 1) {
                //apply comment
                info.Comment = e.Label;
                e.Node.Text = info.GetName(ref comment, currentSortMode);
                e.Node.ForeColor = commentColor;
            } else { //Edit cancelled.
                info.Comment = ""; //Remove comment
                e.Node.Text = info.GetName(ref comment, currentSortMode);
                e.Node.ForeColor = Color.Black;
            }
            treeView.MouseLeave += new EventHandler(treeView_MouseLeave);
        }

        private void treeView_MouseLeave(object sender, EventArgs e) {
            General.Interface.FocusDisplay();
        }

        //It is called every time a dialog window closes.
		private void ParentForm_Activated(object sender, EventArgs e){
            UpdateTreeSoon();
		}

        private void btnClearSearch_Click(object sender, EventArgs e) {
            tbSearch.Clear();
            General.Interface.FocusDisplay();
        }

        private void tbSearch_TextChanged(object sender, EventArgs e) {
            if (tbSearch.Text.Length > 1 || tbSearch.Text.Length == 0) updateTree(false);
        }

        private void cbCommentsOnly_CheckedChanged(object sender, EventArgs e) {
            updateTree(true);
        }

        private void updatetimer_Tick(object sender, EventArgs e) {
            updatetimer.Stop();
			updateTree(true);
        }
    }

    internal struct SortMode
    {
        public const string SORT_BY_INDEX = "By Index";
        public const string SORT_BY_TAG = "By Tag";
        public const string SORT_BY_ACTION = "By Action special";
        public static object[] SORT_MODES = new object[] { SORT_BY_INDEX, SORT_BY_TAG, SORT_BY_ACTION };
    }

    internal struct SelectedNode
    {
        public NodeInfoType Type;
        public int Index;
    }
}
