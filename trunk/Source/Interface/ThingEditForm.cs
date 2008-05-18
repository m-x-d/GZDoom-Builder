
#region ================== Copyright (c) 2007 Pascal vd Heiden

/*
 * Copyright (c) 2007 Pascal vd Heiden, www.codeimp.com
 * This program is released under GNU General Public License
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 */

#endregion

#region ================== Namespaces

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Data;
using CodeImp.DoomBuilder.IO;
using System.IO;
using CodeImp.DoomBuilder.Config;
using CodeImp.DoomBuilder.Editing;
using CodeImp.DoomBuilder.Geometry;

#endregion

namespace CodeImp.DoomBuilder.Interface
{
	public partial class ThingEditForm : DelayedForm
	{
		#region ================== Variables

		private ICollection<Thing> things;
		private List<TreeNode> nodes;
		
		#endregion

		#region ================== Properties

		#endregion

		#region ================== Constructor

		// Constructor
		public ThingEditForm()
		{
			// Initialize
			InitializeComponent();

			// Fill flags list
			foreach(KeyValuePair<int, string> tf in General.Map.Config.ThingFlags) flags.Add(tf.Value, tf.Key);

			// Fill actions list
			action.GeneralizedCategories = General.Map.Config.GenActionCategories;
			action.AddInfo(General.Map.Config.SortedLinedefActions.ToArray());
			
			// Go for all predefined categories
			typelist.Nodes.Clear();
			nodes = new List<TreeNode>();
			foreach(ThingCategory tc in General.Map.Config.ThingCategories)
			{
				// Create category
				TreeNode cn = typelist.Nodes.Add(tc.Name, tc.Title);
				if((tc.Color >= 0) && (tc.Color < thingimages.Images.Count)) cn.ImageIndex = tc.Color;
				cn.SelectedImageIndex = cn.ImageIndex;
				foreach(ThingTypeInfo ti in tc.Things)
				{
					// Create thing
					TreeNode n = cn.Nodes.Add(ti.Title);
					if((ti.Color >= 0) && (ti.Color < thingimages.Images.Count)) n.ImageIndex = ti.Color;
					n.SelectedImageIndex = n.ImageIndex;
					n.Tag = ti;
					nodes.Add(n);
				}
			}
		}

		// This sets up the form to edit the given things
		public void Setup(ICollection<Thing> things)
		{
			Thing ft;
			int angledeg;

			// Keep this list
			this.things = things;
			if(things.Count > 1) this.Text = "Edit Things (" + things.Count + ")";
			
			////////////////////////////////////////////////////////////////////////
			// Set all options to the first thing properties
			////////////////////////////////////////////////////////////////////////

			ft = General.GetByIndex<Thing>(things, 0);
			
			// Set type index
			typeid.Text = ft.Type.ToString();
			
			// Select node
			typelist.SelectedNode = null;
			foreach(TreeNode n in nodes)
			{
				// Matching node?
				if((n.Tag as ThingTypeInfo).Index == ft.Type)
				{
					// Select this
					n.Parent.Expand();
					typelist.SelectedNode = n;
					n.EnsureVisible();
				}
			}

			// Flags
			foreach(CheckBox c in flags.Checkboxes)
				c.Checked = (ft.Flags & (int)c.Tag) != 0;
			
			// Coordination
			angledeg = ft.AngleDeg - 90;
			if(angledeg < 0) angledeg += 360;
			if(angledeg >= 360) angledeg -= 360;
			angle.Text = angledeg.ToString();
			height.Text = ft.ZOffset.ToString();

			// Action/tags
			action.Value = ft.Action;
			tag.Text = ft.Tag.ToString();
			arg0.Text = ft.Args[0].ToString();
			arg1.Text = ft.Args[1].ToString();
			arg2.Text = ft.Args[2].ToString();
			arg3.Text = ft.Args[3].ToString();
			arg4.Text = ft.Args[4].ToString();

			////////////////////////////////////////////////////////////////////////
			// Now go for all lines and change the options when a setting is different
			////////////////////////////////////////////////////////////////////////
			
			// Go for all things
			foreach(Thing t in things)
			{
				// Selected node does not match?
				if((typelist.SelectedNode != null) &&
				   ((typelist.SelectedNode.Tag as ThingTypeInfo).Index != t.Type))
					typelist.SelectedNode = null;

				// Type index
				if(t.Type.ToString() != typeid.Text) typeid.Text = "";
				
				// Flags
				foreach(CheckBox c in flags.Checkboxes)
				{
					if(((t.Flags & (int)c.Tag) != 0) != c.Checked)
					{
						c.ThreeState = true;
						c.CheckState = CheckState.Indeterminate;
					}
				}
				
				// Coordination
				angledeg = t.AngleDeg - 90;
				if(angledeg < 0) angledeg += 360;
				if(angledeg >= 360) angledeg -= 360;
				if(angledeg.ToString() != angle.Text) angle.Text = "";
				if(t.ZOffset.ToString() != height.Text) height.Text = "";

				// Action/tags
				if(t.Action != action.Value) action.Empty = true;
				if(t.Tag.ToString() != tag.Text) tag.Text = "";
				if(t.Args[0].ToString() != arg0.Text) arg0.Text = "";
				if(t.Args[1].ToString() != arg1.Text) arg1.Text = "";
				if(t.Args[2].ToString() != arg2.Text) arg2.Text = "";
				if(t.Args[3].ToString() != arg3.Text) arg3.Text = "";
				if(t.Args[4].ToString() != arg4.Text) arg4.Text = "";
			}
		}
		
		#endregion

		#region ================== Interface

		// This finds a new (unused) tag
		private void newtag_Click(object sender, EventArgs e)
		{
			tag.Text = General.Map.Map.GetNewTag().ToString();
		}

		// Action changes
		private void action_ValueChanges(object sender, EventArgs e)
		{
			int showaction = 0;

			// Only when line type is known
			if(General.Map.Config.LinedefActions.ContainsKey(action.Value)) showaction = action.Value;

			// Change the argument descriptions
			arg0label.Text = General.Map.Config.LinedefActions[showaction].ArgTitle[0] + ":";
			arg1label.Text = General.Map.Config.LinedefActions[showaction].ArgTitle[1] + ":";
			arg2label.Text = General.Map.Config.LinedefActions[showaction].ArgTitle[2] + ":";
			arg3label.Text = General.Map.Config.LinedefActions[showaction].ArgTitle[3] + ":";
			arg4label.Text = General.Map.Config.LinedefActions[showaction].ArgTitle[4] + ":";
			arg0label.Enabled = General.Map.Config.LinedefActions[showaction].ArgUsed[0];
			arg1label.Enabled = General.Map.Config.LinedefActions[showaction].ArgUsed[1];
			arg2label.Enabled = General.Map.Config.LinedefActions[showaction].ArgUsed[2];
			arg3label.Enabled = General.Map.Config.LinedefActions[showaction].ArgUsed[3];
			arg4label.Enabled = General.Map.Config.LinedefActions[showaction].ArgUsed[4];
			if(arg0label.Enabled) arg0.ForeColor = SystemColors.WindowText; else arg0.ForeColor = SystemColors.GrayText;
			if(arg1label.Enabled) arg1.ForeColor = SystemColors.WindowText; else arg1.ForeColor = SystemColors.GrayText;
			if(arg2label.Enabled) arg2.ForeColor = SystemColors.WindowText; else arg2.ForeColor = SystemColors.GrayText;
			if(arg3label.Enabled) arg3.ForeColor = SystemColors.WindowText; else arg3.ForeColor = SystemColors.GrayText;
			if(arg4label.Enabled) arg4.ForeColor = SystemColors.WindowText; else arg4.ForeColor = SystemColors.GrayText;
		}

		// Browse Action clicked
		private void browseaction_Click(object sender, EventArgs e)
		{
			action.Value = ActionBrowserForm.BrowseAction(this, action.Value);
		}

		// Angle text changes
		private void angle_TextChanged(object sender, EventArgs e)
		{
			anglecontrol.Value = angle.GetResult(int.MinValue);
		}

		// Angle control clicked
		private void anglecontrol_ButtonClicked(object sender, EventArgs e)
		{
			angle.Text = anglecontrol.Value.ToString();
		}

		// Thing type selection changed
		private void typelist_AfterSelect(object sender, TreeViewEventArgs e)
		{
			// Anything selected?
			if(typelist.SelectedNode != null)
			{
				TreeNode n = typelist.SelectedNode;
				
				// Node is a child node?
				if((n.Nodes.Count == 0) && (n.Tag != null) && (n.Tag is ThingTypeInfo))
				{
					ThingTypeInfo ti = (n.Tag as ThingTypeInfo);

					// Show info
					typeid.Text = ti.Index.ToString();
				}
			}
		}

		// Thing type index changed
		private void typeid_TextChanged(object sender, EventArgs e)
		{
			bool knownthing = false;

			// Any text?
			if(typeid.Text.Length > 0)
			{
				// Get the info
				ThingTypeInfo ti = General.Map.Config.GetThingInfoEx(typeid.GetResult(0));
				if(ti != null)
				{
					knownthing = true;

					// Size
					sizelabel.Text = ti.Width + " x " + ti.Height;

					// Hangs from ceiling
					if(ti.Hangs) positionlabel.Text = "Ceiling"; else positionlabel.Text = "Floor";

					// Blocking
					switch(ti.Blocking)
					{
						case ThingTypeInfo.THING_BLOCKING_NONE: blockinglabel.Text = "No"; break;
						case ThingTypeInfo.THING_BLOCKING_FULL: blockinglabel.Text = "Completely"; break;
						case ThingTypeInfo.THING_BLOCKING_HEIGHT: blockinglabel.Text = "True-Height"; break;
						default: blockinglabel.Text = "Unknown"; break;
					}

					// Show image
					General.DisplayZoomedImage(spritetex, General.Map.Data.GetSpriteBitmap(ti.Sprite));
				}
			}

			// No known thing?
			if(!knownthing)
			{
				sizelabel.Text = "-";
				positionlabel.Text = "-";
				blockinglabel.Text = "-";
				General.DisplayZoomedImage(spritetex, null);
			}
		}

		// Apply clicked
		private void apply_Click(object sender, EventArgs e)
		{
			string undodesc = "thing";

			// Make undo
			if(things.Count > 1) undodesc = things.Count + " things";
			General.Map.UndoRedo.CreateUndo("Edit " + undodesc, UndoGroup.None, 0);
			
			// Go for all the things
			foreach(Thing t in things)
			{
				// Thing type index
				t.Type = typeid.GetResult(t.Type);
				
				// Coordination
				t.Rotate(Angle2D.DegToRad((float)(angle.GetResult(t.AngleDeg - 90) + 90)));
				t.ZOffset = height.GetResult(t.ZOffset);
				
				// Apply all flags
				foreach(CheckBox c in flags.Checkboxes)
				{
					if(c.CheckState == CheckState.Checked) t.Flags |= (int)c.Tag;
					else if(c.CheckState == CheckState.Unchecked) t.Flags &= ~(int)c.Tag;
				}

				// Action/tags
				if(!action.Empty) t.Action = action.Value;
				t.Tag = tag.GetResult(t.Tag);
				t.Args[0] = (byte)arg0.GetResult(t.Args[0]);
				t.Args[1] = (byte)arg1.GetResult(t.Args[1]);
				t.Args[2] = (byte)arg2.GetResult(t.Args[2]);
				t.Args[3] = (byte)arg3.GetResult(t.Args[3]);
				t.Args[4] = (byte)arg4.GetResult(t.Args[4]);

				// Update settings
				t.UpdateConfiguration();
			}

			// Done
			General.Map.IsChanged = true;
			this.DialogResult = DialogResult.OK;
			this.Close();
		}

		// Cancel clicked
		private void cancel_Click(object sender, EventArgs e)
		{
			// Be gone
			this.DialogResult = DialogResult.Cancel;
			this.Close();
		}
		
		#endregion
	}
}