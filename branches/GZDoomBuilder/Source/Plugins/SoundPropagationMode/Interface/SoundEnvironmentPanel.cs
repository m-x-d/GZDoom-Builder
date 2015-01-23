using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Editing;
using CodeImp.DoomBuilder.Geometry;

namespace CodeImp.DoomBuilder.SoundPropagationMode
{
	public partial class SoundEnvironmentPanel : UserControl
	{
		public BufferedTreeView SoundEnvironments { get { return soundenvironments; } set { soundenvironments = value; } }

		public SoundEnvironmentPanel()
		{
			InitializeComponent();

			soundenvironments.ImageList = new ImageList();
			soundenvironments.ImageList.Images.Add(global::SoundPropagationMode.Properties.Resources.Status0);
			soundenvironments.ImageList.Images.Add(global::SoundPropagationMode.Properties.Resources.Warning);
		}

		public void AddSoundEnvironment(SoundEnvironment se)
		{
			TreeNode topnode = new TreeNode("Sound environment " + se.ID);
			TreeNode thingsnode = new TreeNode("Things (" + se.Things.Count + ")");
			TreeNode linedefsnode = new TreeNode("Linedefs (" + se.Linedefs.Count + ")");
			int notdormant = 0;
			int topindex = 0;

			// Add things
			foreach (Thing t in se.Things)
			{
				TreeNode thingnode = new TreeNode("Thing " + t.Index);
				thingnode.Tag = t;
				thingsnode.Nodes.Add(thingnode);

				if(!ThingDormant(t))
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
				thingsnode.ImageIndex = 1;
				thingsnode.SelectedImageIndex = 1;
				topindex = 1;

				foreach (TreeNode tn in thingsnode.Nodes)
				{
					if (!ThingDormant((Thing)tn.Tag))
					{
						tn.ImageIndex = 1;
						tn.SelectedImageIndex = 1;
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
					linedefnode.ImageIndex = 1;
					linedefnode.SelectedImageIndex = 1;

					linedefsnode.ImageIndex = 1;
					linedefsnode.SelectedImageIndex = 1;

					topindex = 1;
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
			Regex seid = new Regex(@"\d+$");
			int insertionplace = 0;

			foreach (TreeNode tn in soundenvironments.Nodes)
			{
				Match match = seid.Match(tn.Text);
				int num = int.Parse(match.Value);

				if (se.ID < num) break;

				insertionplace++;
			}

			soundenvironments.Nodes.Insert(insertionplace, topnode);
		}

		public void HighlightSoundEnvironment(SoundEnvironment se)
		{
			soundenvironments.BeginUpdate();

			foreach (TreeNode tn in soundenvironments.Nodes)
			{
				if (se != null && tn.Text == "Sound environment " + se.ID)
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

		private static bool ThingDormant(Thing thing)
		{
			return thing.IsFlagSet(General.Map.UDMF ? "dormant" : "14");
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
