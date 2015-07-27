
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
using System.Drawing;
using System.Windows.Forms;
using CodeImp.DoomBuilder.Config;
using CodeImp.DoomBuilder.Geometry;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Types;

#endregion

namespace CodeImp.DoomBuilder.Windows
{
	/// <summary>
	/// Dialog window that allows viewing and editing of Thing properties.
	/// </summary>
	internal partial class ThingEditForm : DelayedForm
	{
		#region ================== Events

		public event EventHandler OnValuesChanged; //mxd

		#endregion
		
		#region ================== Variables

		private ICollection<Thing> things;
		private ThingTypeInfo thinginfo;
		private bool preventchanges;
		private bool preventmapchange; //mxd
		private bool undocreated; //mxd
		private static bool useabsoluteheight; //mxd
		private List<ThingProperties> thingprops; //mxd

		//mxd. Window setup stuff
		private static Point location = Point.Empty;

		private struct ThingProperties //mxd
		{
			//public readonly int Type;
			public readonly int AngleDoom;
			public readonly float X;
			public readonly float Y;
			public readonly float Z;

			public ThingProperties(Thing t) 
			{
				X = t.Position.x;
				Y = t.Position.y;
				Z = t.Position.z;
				//Type = t.Type;
				AngleDoom = t.AngleDoom;
			}
		}

		#endregion

		#region ================== Constructor

		// Constructor
		public ThingEditForm()
		{
			// Initialize
			InitializeComponent();

			//mxd. Widow setup
			if(location != Point.Empty) 
			{
				this.StartPosition = FormStartPosition.Manual;
				this.Location = location;
			}
			
			// Fill flags list
			foreach(KeyValuePair<string, string> tf in General.Map.Config.ThingFlags)
				flags.Add(tf.Value, tf.Key);

			// Fill actions list
			action.GeneralizedCategories = General.Map.Config.GenActionCategories;
			action.AddInfo(General.Map.Config.SortedLinedefActions.ToArray());

			// Tag/Effects?
			actiongroup.Visible = General.Map.FormatInterface.HasThingAction;
			idgroup.Visible = General.Map.FormatInterface.HasThingTag;
			
			// Thing height?
			posZ.Visible = General.Map.FormatInterface.HasThingHeight;
			zlabel.Visible = General.Map.FormatInterface.HasThingHeight;
			cbAbsoluteHeight.Visible = General.Map.FormatInterface.HasThingHeight; //mxd

			//mxd. Decimals allowed?
			if(General.Map.FormatInterface.VertexDecimals > 0) 
			{
				posX.AllowDecimal = true;
				posY.AllowDecimal = true;
				posZ.AllowDecimal = true;
			}

			//mxd. Arrange inteface
			int targetheight;
			if(General.Map.FormatInterface.HasThingAction)
				targetheight = actiongroup.Bottom + actiongroup.Margin.Bottom;
			else
				targetheight = typegroup.Bottom + typegroup.Margin.Bottom * 2;

			if(General.Map.FormatInterface.HasThingTag)
			{
				idgroup.Top = targetheight;
				targetheight = idgroup.Bottom + idgroup.Margin.Bottom * 2;
			}

			panel.Height = targetheight;

			//mxd. Arrange Apply/Cancel buttons
			applypanel.Top = panel.Bottom + panel.Margin.Bottom * 2;

			//mxd. Update window height
			this.Height = applypanel.Bottom + applypanel.Margin.Bottom + (this.Height - this.ClientRectangle.Height) + 1;
			
			// Setup types list
			thingtype.Setup();
		}

		#endregion

		#region ================== Methods

		// This sets up the form to edit the given things
		public void Setup(ICollection<Thing> things)
		{
			preventchanges = true;

			// Keep this list
			this.things = things;
			if (things.Count > 1) this.Text = "Edit Things (" + things.Count + ")";
			hint.Visible = things.Count > 1; //mxd
			hintlabel.Visible = things.Count > 1; //mxd
			thingtype.UseMultiSelection = things.Count > 1; //mxd
			
			////////////////////////////////////////////////////////////////////////
			// Set all options to the first thing properties
			////////////////////////////////////////////////////////////////////////

			Thing ft = General.GetByIndex(things, 0);
			
			// Set type
			thingtype.SelectType(ft.Type);
			
			// Flags
			foreach(CheckBox c in flags.Checkboxes)
				if(ft.Flags.ContainsKey(c.Tag.ToString())) c.Checked = ft.Flags[c.Tag.ToString()];
			
			// Coordination
			angle.Text = ft.AngleDoom.ToString();
			zlabel.Text = useabsoluteheight ? "Z:" : "Height:"; //mxd
			cbAbsoluteHeight.Checked = useabsoluteheight; //mxd

			//mxd
			ft.DetermineSector();
			float floorheight = (ft.Sector != null ? Sector.GetFloorPlane(ft.Sector).GetZ(ft.Position) : 0);
			posX.Text = ((int)ft.Position.x).ToString();
			posY.Text = ((int)ft.Position.y).ToString();
			posZ.Text = (useabsoluteheight ? ((int)Math.Round(ft.Position.z + floorheight)).ToString() : ((int)ft.Position.z).ToString());
			posX.ButtonStep = General.Map.Grid.GridSize;
			posY.ButtonStep = General.Map.Grid.GridSize;
			posZ.ButtonStep = General.Map.Grid.GridSize;

			// Action/tags
			action.Value = ft.Action;
			if(General.Map.FormatInterface.HasThingTag) //mxd
			{
				tagSelector.Setup(UniversalType.ThingTag); 
				tagSelector.SetTag(ft.Tag);
			}

			//mxd. Args
			argscontrol.SetValue(ft, true);

			////////////////////////////////////////////////////////////////////////
			// Now go for all lines and change the options when a setting is different
			////////////////////////////////////////////////////////////////////////

			thingprops = new List<ThingProperties>();

			// Go for all things
			foreach(Thing t in things)
			{
				//mxd. Update sector info
				t.DetermineSector();
				
				// Type does not match?
				ThingTypeInfo info = thingtype.GetSelectedInfo(); //mxd

				if(info != null && info.Index != t.Type)
					thingtype.ClearSelectedType();
				
				// Flags
				foreach(CheckBox c in flags.Checkboxes)
				{
					if(c.CheckState == CheckState.Indeterminate) continue; //mxd
					if(t.IsFlagSet(c.Tag.ToString()) != c.Checked) 
					{
						c.ThreeState = true;
						c.CheckState = CheckState.Indeterminate;
					}
				}
				
				// Coordination
				if(t.AngleDoom.ToString() != angle.Text) angle.Text = "";
				
				//mxd. Position
				if(((int)t.Position.x).ToString() != posX.Text) posX.Text = "";
				if(((int)t.Position.y).ToString() != posY.Text) posY.Text = "";
				if (useabsoluteheight && t.Sector != null) 
				{
					if(((int)Math.Round(Sector.GetFloorPlane(t.Sector).GetZ(t.Position) + t.Position.z)).ToString() != posZ.Text)
						posZ.Text = "";
				} 
				else if(((int)t.Position.z).ToString() != posZ.Text)
				{
					posZ.Text = "";
				}

				// Action/tags
				if(t.Action != action.Value) action.Empty = true;
				if(General.Map.FormatInterface.HasThingTag && t.Tag != ft.Tag) tagSelector.ClearTag(); //mxd

				//mxd. Arguments
				argscontrol.SetValue(t, false);

				//mxd. Store initial properties
				thingprops.Add(new ThingProperties(t));
			}

			preventchanges = false;

			//mxd. Trigger updates manually...
			preventmapchange = true;
			angle_WhenTextChanged(angle, EventArgs.Empty);
			flags_OnValueChanged(flags, EventArgs.Empty);
			preventmapchange = false;

			argscontrol.UpdateScriptControls(); //mxd
			actionhelp.UpdateAction(action.GetValue()); //mxd
		}

		//mxd
		private void MakeUndo() 
		{
			if(undocreated) return;
			undocreated = true;

			//mxd. Make undo
			General.Map.UndoRedo.CreateUndo("Edit " + (things.Count > 1 ? things.Count + " things" : "thing"));
		}
		
		#endregion

		#region ================== Events

		//mxd
		private void thingtype_OnTypeDoubleClicked() 
		{
			apply_Click(this, EventArgs.Empty);
		}
		
		// Action changes
		private void action_ValueChanges(object sender, EventArgs e)
		{
			int showaction = 0;

			// Only when line type is known, otherwise use the thing arguments
			if(General.Map.Config.LinedefActions.ContainsKey(action.Value)) showaction = action.Value;
			
			//mxd. Change the argument descriptions
			argscontrol.UpdateAction(showaction, preventchanges, (action.Empty ? null : thinginfo));

			if(!preventchanges)
			{
				MakeUndo();

				//mxd. Update what must be updated
				argscontrol.UpdateScriptControls();
				actionhelp.UpdateAction(showaction);
			}
		}

		// Browse Action clicked
		private void browseaction_Click(object sender, EventArgs e)
		{
			action.Value = ActionBrowserForm.BrowseAction(this, action.Value);
		}

		// Angle text changes
		private void angle_WhenTextChanged(object sender, EventArgs e)
		{
			if(preventchanges) return;
			preventchanges = true;
			anglecontrol.Angle = angle.GetResult(GZBuilder.Controls.AngleControl.NO_ANGLE);
			preventchanges = false;
			if(!preventmapchange) ApplyAngleChange(); //mxd
		}

		//mxd. Angle control clicked
		private void anglecontrol_AngleChanged(object sender, EventArgs e) 
		{
			if(preventchanges) return;
			angle.Text = anglecontrol.Angle.ToString();
			if(!preventmapchange) ApplyAngleChange();
		}

		// Apply clicked
		private void apply_Click(object sender, EventArgs e)
		{
			MakeUndo();
			
			List<string> defaultflags = new List<string>();

			// Verify the tag
			if(General.Map.FormatInterface.HasThingTag) //mxd
			{
				tagSelector.ValidateTag();//mxd
				if(((tagSelector.GetTag(0) < General.Map.FormatInterface.MinTag) || (tagSelector.GetTag(0) > General.Map.FormatInterface.MaxTag))) 
				{
					General.ShowWarningMessage("Thing tag must be between " + General.Map.FormatInterface.MinTag + " and " + General.Map.FormatInterface.MaxTag + ".", MessageBoxButtons.OK);
					return;
				}
			}

			// Verify the type
			if(((thingtype.GetResult(0) < General.Map.FormatInterface.MinThingType) || (thingtype.GetResult(0) > General.Map.FormatInterface.MaxThingType)))
			{
				General.ShowWarningMessage("Thing type must be between " + General.Map.FormatInterface.MinThingType + " and " + General.Map.FormatInterface.MaxThingType + ".", MessageBoxButtons.OK);
				return;
			}

			// Verify the action
			if(General.Map.FormatInterface.HasThingAction && ((action.Value < General.Map.FormatInterface.MinAction) || (action.Value > General.Map.FormatInterface.MaxAction)))
			{
				General.ShowWarningMessage("Thing action must be between " + General.Map.FormatInterface.MinAction + " and " + General.Map.FormatInterface.MaxAction + ".", MessageBoxButtons.OK);
				return;
			}

			// Go for all the things
			int tagoffset = 0; //mxd
			foreach(Thing t in things)
			{
				// Coordination
				if(cbRandomAngle.Checked) t.Rotate(General.Random(0, 359)); //mxd

				//mxd. Check position
				float px = General.Clamp(t.Position.x, General.Map.Config.LeftBoundary, General.Map.Config.RightBoundary);
				float py = General.Clamp(t.Position.y, General.Map.Config.BottomBoundary, General.Map.Config.TopBoundary);
				if(t.Position.x != px || t.Position.y != py) t.Move(new Vector2D(px, py));
				
				// Apply all flags
				foreach(CheckBox c in flags.Checkboxes)
				{
					switch (c.CheckState)
					{
						case CheckState.Checked: t.SetFlag(c.Tag.ToString(), true); break;
						case CheckState.Unchecked: t.SetFlag(c.Tag.ToString(), false); break;
					}
				}

				// Action/tags
				t.Tag = General.Clamp(tagSelector.GetSmartTag(t.Tag, tagoffset++), General.Map.FormatInterface.MinTag, General.Map.FormatInterface.MaxTag); //mxd
				if (!action.Empty) t.Action = action.Value;

				//mxd. Apply args
				argscontrol.Apply(t);
				
				// Update settings
				t.UpdateConfiguration();
			}

			// Set as defaults
			foreach(CheckBox c in flags.Checkboxes)
				if(c.CheckState == CheckState.Checked) defaultflags.Add(c.Tag.ToString());
			General.Settings.DefaultThingType = thingtype.GetResult(General.Settings.DefaultThingType);
			General.Settings.DefaultThingAngle = Angle2D.DegToRad((float)angle.GetResult((int)Angle2D.RadToDeg(General.Settings.DefaultThingAngle) - 90) + 90);
			General.Settings.SetDefaultThingFlags(defaultflags);
			
			// Done
			General.Map.IsChanged = true;
			if(OnValuesChanged != null)	OnValuesChanged(this, EventArgs.Empty); //mxd
			this.DialogResult = DialogResult.OK;
			this.Close();
		}

		// Cancel clicked
		private void cancel_Click(object sender, EventArgs e)
		{
			//mxd. Perform undo?
			if(undocreated) General.Map.UndoRedo.WithdrawUndo();
			
			// Be gone
			this.DialogResult = DialogResult.Cancel;
			this.Close();
		}

		//mxd
		private void cbAbsoluteHeight_CheckedChanged(object sender, EventArgs e) 
		{
			if(preventchanges) return;
			MakeUndo();

			useabsoluteheight = cbAbsoluteHeight.Checked;
			zlabel.Text = (useabsoluteheight ? "Z:" : "Height:");
			
			preventchanges = true;
			
			//update label text
			Thing ft = General.GetByIndex(things, 0);
			float z = ft.Position.z;
			if(useabsoluteheight && ft.Sector != null) z += Sector.GetFloorPlane(ft.Sector).GetZ(ft.Position);
			posZ.Text = ((float)Math.Round(z, General.Map.FormatInterface.VertexDecimals)).ToString();

			foreach(Thing t in things) 
			{
				z = t.Position.z;
				if(useabsoluteheight && t.Sector != null) z += Sector.GetFloorPlane(t.Sector).GetZ(t.Position);
				string ztext = ((float)Math.Round(z, General.Map.FormatInterface.VertexDecimals)).ToString();
				if(posZ.Text != ztext) 
				{
					posZ.Text = "";
					break;
				}
			}

			preventchanges = false;
		}

		//mxd
		private void cbRandomAngle_CheckedChanged(object sender, EventArgs e) 
		{
			angle.Enabled = !cbRandomAngle.Checked;
			anglecontrol.Enabled = !cbRandomAngle.Checked;
		}

		//mxd
		private void ThingEditForm_FormClosing(object sender, FormClosingEventArgs e) 
		{
			location = this.Location;
		}

		// Help
		private void ThingEditForm_HelpRequested(object sender, HelpEventArgs hlpevent)
		{
			General.ShowHelp("w_thingedit.html");
			hlpevent.Handled = true;
		}

		#endregion

		#region ================== mxd. Realtime events

		private void posX_WhenTextChanged(object sender, EventArgs e) 
		{
			if(preventchanges) return;
			MakeUndo(); //mxd
			int i = 0;

			// Update values
			foreach(Thing t in things)
				t.Move(new Vector2D(posX.GetResultFloat(thingprops[i++].X), t.Position.y));

			General.Map.IsChanged = true;
			if(OnValuesChanged != null)	OnValuesChanged(this, EventArgs.Empty);
		}

		private void posY_WhenTextChanged(object sender, EventArgs e) 
		{
			if(preventchanges) return;
			MakeUndo(); //mxd
			int i = 0;

			// Update values
			foreach(Thing t in things)
				t.Move(new Vector2D(t.Position.x, posY.GetResultFloat(thingprops[i++].Y)));

			General.Map.IsChanged = true;
			if(OnValuesChanged != null) OnValuesChanged(this, EventArgs.Empty);
		}

		private void posZ_WhenTextChanged(object sender, EventArgs e) 
		{
			if(preventchanges) return;
			MakeUndo(); //mxd
			int i = 0;

			if(string.IsNullOrEmpty(posZ.Text)) 
			{
				// Restore values
				foreach(Thing t in things)
					t.Move(new Vector3D(t.Position.x, t.Position.y, thingprops[i++].Z));
			} 
			else 
			{ 
				// Update values
				foreach(Thing t in things) 
				{
					float z = posZ.GetResultFloat(thingprops[i++].Z);
					if(useabsoluteheight && !posZ.CheckIsRelative() && t.Sector != null)
						z -= (float)Math.Round(Sector.GetFloorPlane(t.Sector).GetZ(t.Position.x, t.Position.y), General.Map.FormatInterface.VertexDecimals);
					t.Move(new Vector3D(t.Position.x, t.Position.y, z));
				}
			}

			General.Map.IsChanged = true;
			if(OnValuesChanged != null) OnValuesChanged(this, EventArgs.Empty);
		}

		// Selected type changes
		private void thingtype_OnTypeChanged(ThingTypeInfo value) 
		{
			thinginfo = value;

			// Update arguments
			action_ValueChanges(this, EventArgs.Empty);

			//mxd. Update things
			if(preventchanges) return;
			MakeUndo(); //mxd

			if(((thingtype.GetResult(0) < General.Map.FormatInterface.MinThingType) || (thingtype.GetResult(0) > General.Map.FormatInterface.MaxThingType)))
				return;

			foreach(Thing t in things) 
			{
				//Set type
				t.Type = thingtype.GetResult(t.Type);

				// Update settings
				t.UpdateConfiguration();
			}

			General.Map.IsChanged = true;
			if(OnValuesChanged != null) OnValuesChanged(this, EventArgs.Empty);
		}

		//mxd
		private void ApplyAngleChange() 
		{
			if(preventchanges) return;
			MakeUndo(); //mxd
			int i = 0;

			//restore values
			if(string.IsNullOrEmpty(angle.Text)) 
			{
				// Apply rotation
				foreach(Thing t in things)
					t.Rotate(thingprops[i++].AngleDoom);
			}
			else //update values
			{ 
				// Apply rotation
				foreach(Thing t in things)
					t.Rotate(angle.GetResult(thingprops[i++].AngleDoom));
			}

			General.Map.IsChanged = true;
			if(OnValuesChanged != null)	OnValuesChanged(this, EventArgs.Empty);
		}

		//mxd
		private void flags_OnValueChanged(object sender, EventArgs e) 
		{
			if(preventchanges) return;

			string warn = ThingFlagsCompare.CheckThingEditFormFlags(flags.Checkboxes);
			if(!string.IsNullOrEmpty(warn)) 
			{
				//got missing flags
				tooltip.SetToolTip(missingflags, warn);
				missingflags.Visible = true;
				settingsgroup.ForeColor = Color.DarkRed;
				return;
			}

			//everything is OK
			missingflags.Visible = false;
			settingsgroup.ForeColor = SystemColors.ControlText;
		}

		#endregion

	}
}