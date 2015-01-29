using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Editing;
using CodeImp.DoomBuilder.Geometry;

namespace CodeImp.DoomBuilder.SoundPropagationMode
{
	public partial class SoundEnvironmentPanel : UserControl
	{
		public BufferedTreeView SoundEnvironments { get { return soundenvironments; } set { soundenvironments = value; } }
		private readonly int warningiconindex; //mxd

		public SoundEnvironmentPanel()
		{
			InitializeComponent();

			soundenvironments.ImageList = new ImageList();
			foreach(Bitmap icon in BuilderPlug.Me.DistinctIcons)
			{
				soundenvironments.ImageList.Images.Add(icon);
			}
			soundenvironments.ImageList.Images.Add(Properties.Resources.Warning);
			warningiconindex = soundenvironments.ImageList.Images.Count - 1; //mxd
		}

		public void AddSoundEnvironment(SoundEnvironment se) 
		{
			TreeNode topnode = new TreeNode(se.Name);
			topnode.Tag = se; //mxd
			TreeNode thingsnode = new TreeNode("Things (" + se.Things.Count + ")");
			TreeNode linedefsnode = new TreeNode("Linedefs (" + se.Linedefs.Count + ")");
			int notdormant = 0;
			int iconindex = BuilderPlug.Me.DistinctColors.IndexOf(se.Color); //mxd
			int topindex = iconindex; //mxd

			thingsnode.ImageIndex = iconindex; //mxd
			thingsnode.SelectedImageIndex = iconindex; //mxd
			linedefsnode.ImageIndex = iconindex; //mxd
			linedefsnode.SelectedImageIndex = iconindex; //mxd

			// Add things
			foreach (Thing t in se.Things)
			{
				TreeNode thingnode = new TreeNode("Thing " + t.Index);
				thingnode.Tag = t;
				thingnode.ImageIndex = iconindex; //mxd
				thingnode.SelectedImageIndex = iconindex; //mxd
				thingsnode.Nodes.Add(thingnode);

				if(!BuilderPlug.ThingDormant(t))
				{
					notdormant++;
				}
				else
				{
					thingnode.Text += " (dormant)";
				}
			}

			// Set the icon to warning sign and add the tooltip when there are more than 1 non-dormant things
			if (notdormant > 1)
			{
				thingsnode.ImageIndex = warningiconindex;
				thingsnode.SelectedImageIndex = warningiconindex;
				topindex = warningiconindex;

				foreach (TreeNode tn in thingsnode.Nodes)
				{
					if (!BuilderPlug.ThingDormant((Thing)tn.Tag))
					{
						tn.ImageIndex = warningiconindex;
						tn.SelectedImageIndex = warningiconindex;
						tn.ToolTipText = "More than one thing in this\nsound environment is set to be\nactive. Set all but one thing\nto dormant.";
					}
				}
			}

			// Add linedefs
			foreach (Linedef ld in se.Linedefs)
			{
				bool showwarning = false;
				TreeNode linedefnode = new TreeNode("Linedef " + ld.Index);
				linedefnode.Tag = ld;
				linedefnode.ImageIndex = iconindex; //mxd
				linedefnode.SelectedImageIndex = iconindex; //mxd

				if (ld.Back == null)
				{
					showwarning = true;
					linedefnode.ToolTipText = "This line is single-sided, but has\nthe sound boundary flag set.";
				}
				else if (se.Sectors.Contains(ld.Front.Sector) && se.Sectors.Contains(ld.Back.Sector))
				{
					showwarning = true;
					linedefnode.ToolTipText = "More than one thing in this\nThe sectors on both sides of\nthe line belong to the same\nsound environment.";
				}

				if (showwarning)
				{
					linedefnode.ImageIndex = warningiconindex;
					linedefnode.SelectedImageIndex = warningiconindex;

					linedefsnode.ImageIndex = warningiconindex;
					linedefsnode.SelectedImageIndex = warningiconindex;

					topindex = warningiconindex;
				}

				linedefsnode.Nodes.Add(linedefnode);
			}

			topnode.Nodes.Add(thingsnode);
			topnode.Nodes.Add(linedefsnode);

			topnode.Tag = se;
			topnode.ImageIndex = topindex;
			topnode.SelectedImageIndex = topindex;
			topnode.Expand();

			// Sound environments will no be added in consecutive order, so we'll have to find
			// out where in the tree to add the node
			int insertionplace = 0;

			foreach (TreeNode tn in soundenvironments.Nodes)
			{
				if(se.ID < ((SoundEnvironment)tn.Tag).ID) break;
				insertionplace++;
			}

			soundenvironments.Nodes.Insert(insertionplace, topnode);
		}

		public void HighlightSoundEnvironment(SoundEnvironment se)
		{
			soundenvironments.BeginUpdate();

			foreach (TreeNode tn in soundenvironments.Nodes)
			{
				if(se != null && ((SoundEnvironment)tn.Tag).ID == se.ID)
				{
					if (tn.NodeFont == null || tn.NodeFont.Style != FontStyle.Bold)
					{
						tn.NodeFont = new Font(soundenvironments.Font.FontFamily, soundenvironments.Font.Size, FontStyle.Bold);
						tn.Text += string.Empty;
					}
				}
				else
				{
					if(tn.NodeFont == null || tn.NodeFont.Style != FontStyle.Regular)
						tn.NodeFont = new Font(soundenvironments.Font.FontFamily, soundenvironments.Font.Size);
				}
			}

			soundenvironments.EndUpdate();
		}

		private static bool IsClickOnText(TreeView treeView, TreeNode node, Point location)
		{
			var hitTest = treeView.HitTest(location);
			return hitTest.Node == node	&& (hitTest.Location == TreeViewHitTestLocations.Label || hitTest.Location == TreeViewHitTestLocations.Image);
		}

		private static void ProcessNodeClick(TreeNode node)
		{
			if (node == null) return;

			List<Vector2D> points = new List<Vector2D>();
			RectangleF area = MapSet.CreateEmptyArea();

			if (node.Parent == null)
			{
				if (node.Text.StartsWith("Sound environment"))
				{
					SoundEnvironment se = (SoundEnvironment)node.Tag;

					foreach (Sector s in se.Sectors)
					{
						foreach (Sidedef sd in s.Sidedefs)
						{
							points.Add(sd.Line.Start.Position);
							points.Add(sd.Line.End.Position);
						}
					}
				}
				else
				{
					// Don't zoom if the wrong nodes are selected
					return;
				}
			}
			else
			{
				if (node.Parent.Text.StartsWith("Things"))
				{
					Thing t = (Thing)node.Tag;

					// We don't want to be zoomed too closely, so add somepadding
					points.Add(t.Position - 200);
					points.Add(t.Position + 200);
				}
				else if (node.Parent.Text.StartsWith("Linedefs"))
				{
					Linedef ld = (Linedef)node.Tag;

					points.Add(ld.Start.Position);
					points.Add(ld.End.Position);
				}
				else
				{
					// Don't zoom if the wrong nodes are selected
					return;
				}
			}

			area = MapSet.IncreaseArea(area, points);

			// Add padding
			area.Inflate(100f, 100f);

			// Zoom to area
			ClassicMode editmode = (General.Editing.Mode as ClassicMode);
			editmode.CenterOnArea(area, 0.0f);
		}

		#region ================== Events

		private void soundenvironments_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
		{
			if (IsClickOnText(soundenvironments, e.Node, e.Location))
			{
				ProcessNodeClick(e.Node);
			}
		}

		private void soundenvironments_BeforeSelect(object sender, TreeViewCancelEventArgs e)
		{
			if(e.Action != TreeViewAction.ByMouse) return;
			var position = soundenvironments.PointToClient(Cursor.Position);
			e.Cancel = !IsClickOnText(soundenvironments, e.Node, position);
		}

		#endregion
	}
}
