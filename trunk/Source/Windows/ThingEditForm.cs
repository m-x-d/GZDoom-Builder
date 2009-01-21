
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
using CodeImp.DoomBuilder.Controls;

#endregion

namespace CodeImp.DoomBuilder.Windows
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
			foreach(KeyValuePair<string, string> tf in General.Map.Config.ThingFlags)
				flags.Add(tf.Value, tf.Key);

			// Fill actions list
			action.GeneralizedCategories = General.Map.Config.GenActionCategories;
			action.AddInfo(General.Map.Config.SortedLinedefActions.ToArray());

			// Fill universal fields list
			fieldslist.ListFixedFields(General.Map.Config.ThingFields);
			
			// Initialize custom fields editor
			fieldslist.Setup("thing");

			// Not a UDMF map?
			if(!General.Map.IsType(typeof(UniversalMapSetIO)))
			{
				tabs.TabPages.Remove(tabcustom);
			}
			
			// Go for all predefined categories
			typelist.Nodes.Clear();
			nodes = new List<TreeNode>();
			foreach(ThingCategory tc in General.Map.Data.ThingCategories)
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

			// Keep this list
			this.things = things;
			if(things.Count > 1) this.Text = "Edit Things (" + things.Count + ")";
			
			////////////////////////////////////////////////////////////////////////
			// Set all options to the first thing properties
			////////////////////////////////////////////////////////////////////////

			ft = General.GetByIndex(things, 0);
			
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
				if(ft.Flags.ContainsKey(c.Tag.ToString())) c.Checked = ft.Flags[c.Tag.ToString()];
			
			// Coordination
			angle.Text = Angle2D.RealToDoom(ft.Angle).ToString();
			height.Text = ((int)ft.Position.z).ToString();

			// Action/tags
			action.Value = ft.Action;
			tag.Text = ft.Tag.ToString();
			arg0.SetValue(ft.Args[0]);
			arg1.SetValue(ft.Args[1]);
			arg2.SetValue(ft.Args[2]);
			arg3.SetValue(ft.Args[3]);
			arg4.SetValue(ft.Args[4]);

			// Custom fields
			fieldslist.SetValues(ft.Fields, true);

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
					if(t.Flags.ContainsKey(c.Tag.ToString()))
					{
						if(t.Flags[c.Tag.ToString()] != c.Checked)
						{
							c.ThreeState = true;
							c.CheckState = CheckState.Indeterminate;
						}
					}
				}
				
				// Coordination
				int angledeg = Angle2D.RealToDoom(t.Angle);
				if(angledeg.ToString() != angle.Text) angle.Text = "";
				if(((int)t.Position.z).ToString() != height.Text) height.Text = "";

				// Action/tags
				if(t.Action != action.Value) action.Empty = true;
				if(t.Tag.ToString() != tag.Text) tag.Text = "";
				if(t.Args[0] != arg0.GetResult(-1)) arg0.ClearValue();
				if(t.Args[1] != arg1.GetResult(-1)) arg1.ClearValue();
				if(t.Args[2] != arg2.GetResult(-1)) arg2.ClearValue();
				if(t.Args[3] != arg3.GetResult(-1)) arg3.ClearValue();
				if(t.Args[4] != arg4.GetResult(-1)) arg4.ClearValue();

				// Custom fields
				fieldslist.SetValues(t.Fields, false);
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
			arg0label.Text = General.Map.Config.LinedefActions[showaction].Args[0].Title + ":";
			arg1label.Text = General.Map.Config.LinedefActions[showaction].Args[1].Title + ":";
			arg2label.Text = General.Map.Config.LinedefActions[showaction].Args[2].Title + ":";
			arg3label.Text = General.Map.Config.LinedefActions[showaction].Args[3].Title + ":";
			arg4label.Text = General.Map.Config.LinedefActions[showaction].Args[4].Title + ":";
			arg0label.Enabled = General.Map.Config.LinedefActions[showaction].Args[0].Used;
			arg1label.Enabled = General.Map.Config.LinedefActions[showaction].Args[1].Used;
			arg2label.Enabled = General.Map.Config.LinedefActions[showaction].Args[2].Used;
			arg3label.Enabled = General.Map.Config.LinedefActions[showaction].Args[3].Used;
			arg4label.Enabled = General.Map.Config.LinedefActions[showaction].Args[4].Used;
			if(arg0label.Enabled) arg0.ForeColor = SystemColors.WindowText; else arg0.ForeColor = SystemColors.GrayText;
			if(arg1label.Enabled) arg1.ForeColor = SystemColors.WindowText; else arg1.ForeColor = SystemColors.GrayText;
			if(arg2label.Enabled) arg2.ForeColor = SystemColors.WindowText; else arg2.ForeColor = SystemColors.GrayText;
			if(arg3label.Enabled) arg3.ForeColor = SystemColors.WindowText; else arg3.ForeColor = SystemColors.GrayText;
			if(arg4label.Enabled) arg4.ForeColor = SystemColors.WindowText; else arg4.ForeColor = SystemColors.GrayText;
			arg0.Setup(General.Map.Config.LinedefActions[showaction].Args[0]);
			arg1.Setup(General.Map.Config.LinedefActions[showaction].Args[1]);
			arg2.Setup(General.Map.Config.LinedefActions[showaction].Args[2]);
			arg3.Setup(General.Map.Config.LinedefActions[showaction].Args[3]);
			arg4.Setup(General.Map.Config.LinedefActions[showaction].Args[4]);
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
				ThingTypeInfo ti = General.Map.Data.GetThingInfoEx(typeid.GetResult(0));
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
					General.DisplayZoomedImage(spritetex, General.Map.Data.GetSpriteImage(ti.Sprite).GetPreview());
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
			General.Map.UndoRedo.CreateUndo("Edit " + undodesc);
			
			// Go for all the things
			foreach(Thing t in things)
			{
				// Thing type index
				t.Type = typeid.GetResult(t.Type);
				
				// Coordination
				t.Rotate(Angle2D.DoomToReal(angle.GetResult(Angle2D.RealToDoom(t.Angle))));
				t.Move(t.Position.x, t.Position.y, (float)height.GetResult((int)t.Position.z));
				
				// Apply all flags
				foreach(CheckBox c in flags.Checkboxes)
				{
					if(c.CheckState == CheckState.Checked) t.Flags[c.Tag.ToString()] = true;
					else if(c.CheckState == CheckState.Unchecked) t.Flags[c.Tag.ToString()] = false;
				}

				// Action/tags
				if(!action.Empty) t.Action = action.Value;
				t.Tag = tag.GetResult(t.Tag);
				t.Args[0] = arg0.GetResult(t.Args[0]);
				t.Args[1] = arg1.GetResult(t.Args[1]);
				t.Args[2] = arg2.GetResult(t.Args[2]);
				t.Args[3] = arg3.GetResult(t.Args[3]);
				t.Args[4] = arg4.GetResult(t.Args[4]);

				// Custom fields
				fieldslist.Apply(t.Fields);
				
				// Update settings
				t.UpdateConfiguration();
			}

			// Set as defaults
			General.Settings.DefaultThingType = typeid.GetResult(General.Settings.DefaultThingType);
			General.Settings.DefaultThingAngle = Angle2D.DegToRad((float)angle.GetResult((int)Angle2D.RadToDeg(General.Settings.DefaultThingAngle) - 90) + 90);
			
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