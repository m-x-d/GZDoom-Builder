
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
using System.Windows.Forms;
using CodeImp.DoomBuilder.Editing;
using CodeImp.DoomBuilder.Windows;

#endregion

namespace CodeImp.DoomBuilder.BuilderModes
{
	public partial class MenusForm : Form
	{
		#region ================== Variables

		// Menus list
		private readonly ToolStripItem[] menus;

		// mxd. More menus
		private readonly ToolStripItem[] exportmenuitems;

		// mxd. Even more menus!
		private readonly ToolStripItem[] propsmenuitems;

		// Buttons list
		private readonly ToolStripItem[] buttons;

		//mxd
		public struct BrightnessGradientModes
		{
			public const string Sectors = "Sector Brightness";
			public const string Light = "Light Color";
			public const string Fade = "Fade Color";
			public const string LightAndFade = "Light and Fade Colors";
			public const string Floors = "Floor Brightness";
			public const string Ceilings = "Ceiling Brightness";
		}

		//mxd
		internal struct GradientInterpolationModes
		{
			public const string Linear = "Linear";
			public const string EaseInOutSine = "EaseInOutSine";
			public const string EaseInSine = "EaseInSine";
			public const string EaseOutSine = "EaseOutSine";
		}

		#endregion

		#region ================== Properties

		public ToolStripButton ViewSelectionNumbers { get { return buttonselectionnumbers; } }
		public ToolStripButton ViewSelectionEffects { get { return buttonselectioneffects; } }
		public ToolStripSeparator SeparatorSectors1 { get { return separatorsectors1; } }
		public ToolStripSeparator SeparatorSectors2 { get { return separatorsectors2; } } //mxd
		public ToolStripSeparator SeparatorSectors3 { get { return separatorsectors3; } } //mxd
		public ToolStripButton MakeGradientBrightness { get { return buttonbrightnessgradient; } }
		public ToolStripButton MakeGradientFloors { get { return buttonfloorgradient; } }
		public ToolStripButton MakeGradientCeilings { get { return buttonceilinggradient; } }
		public ToolStripButton FlipSelectionV { get { return buttonflipselectionv; } }
		public ToolStripButton FlipSelectionH { get { return buttonflipselectionh; } }
		public ToolStripButton CurveLinedefs { get { return buttoncurvelinedefs; } }
		public ToolStripButton CopyProperties { get { return buttoncopyproperties; } }
		public ToolStripButton PasteProperties { get { return buttonpasteproperties; } }
		public ToolStripButton PastePropertiesOptions { get { return buttonpastepropertiesoptions; } } //mxd
		public ToolStripSeparator SeparatorCopyPaste { get { return seperatorcopypaste; } }
		public ToolStripComboBox GradientModeMenu { get { return gradientModeMenu; } } //mxd
		public ToolStripComboBox GradientInterpolationMenu { get { return gradientInterpolationMenu; } } //mxd
		public ToolStripButton MarqueSelectTouching { get { return buttonMarqueSelectTouching; } } //mxd
		public ToolStripButton AlignThingsToWall { get { return buttonAlignThingsToWall; } } //mxd
		public ToolStripButton TextureOffsetLock { get { return buttonTextureOffsetLock; } } //mxd
		public ToolStripButton DragThingsInSelectedSectors { get { return buttonDragThingsInSelectedSectors; } } //mxd
		public ToolStripButton MakeDoor { get { return buttonMakeDoor; } } //mxd

		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		public MenusForm()
		{
			// Initialize
			InitializeComponent();

			// Apply settings
			buttonselectionnumbers.Checked = BuilderPlug.Me.ViewSelectionNumbers;
			buttonselectioneffects.Checked = BuilderPlug.Me.ViewSelectionEffects; //mxd

			//mxd
			gradientModeMenu.Items.AddRange(new[] { BrightnessGradientModes.Sectors, BrightnessGradientModes.Light, BrightnessGradientModes.Fade, BrightnessGradientModes.LightAndFade, BrightnessGradientModes.Ceilings, BrightnessGradientModes.Floors });
			gradientModeMenu.SelectedIndex = 0;
			gradientInterpolationMenu.Items.AddRange(new[] { GradientInterpolationModes.Linear, GradientInterpolationModes.EaseInOutSine, GradientInterpolationModes.EaseInSine, GradientInterpolationModes.EaseOutSine });
			gradientInterpolationMenu.SelectedIndex = 0;
			
			// List all menus
			menus = new ToolStripItem[menustrip.Items.Count];
			for(int i = 0; i < menustrip.Items.Count; i++) menus[i] = menustrip.Items[i];

			//mxd. Export menu
			exportmenuitems = new ToolStripItem[exportStripMenuItem.DropDownItems.Count];
			for(int i = 0; i < exportStripMenuItem.DropDownItems.Count; i++)
				exportmenuitems[i] = exportStripMenuItem.DropDownItems[i];

			//mxd. Copy-paste propserties items
			propsmenuitems = new ToolStripItem[] { separatorcopyprops, itemcopyprops, itempasteprops, itempastepropsoptions };

			// List all buttons
			buttons = new ToolStripItem[globalstrip.Items.Count];
			for(int i = 0; i < globalstrip.Items.Count; i++) buttons[i] = globalstrip.Items[i];
		}
		
		#endregion

		#region ================== Methods

		// This registers with the core
		public void Register()
		{
			// Add the menus to the core
			foreach(ToolStripMenuItem i in menus)
				General.Interface.AddMenu(i);
			
			// Add the buttons to the core
			foreach(ToolStripItem b in buttons)
				General.Interface.AddButton(b);

			//mxd. Export menu
			foreach(ToolStripMenuItem i in exportmenuitems)
				General.Interface.AddMenu(i, MenuSection.FileExport);

			//mxd. Copy-paste propserties items
			foreach(ToolStripItem i in propsmenuitems)
				General.Interface.AddMenu(i, MenuSection.EditCopyPaste);
		}

		// This unregisters from the core
		public void Unregister()
		{
			// Remove the menus from the core
			foreach(ToolStripMenuItem i in menus)
				General.Interface.RemoveMenu(i);

			// Remove the buttons from the core
			foreach(ToolStripItem b in buttons)
				General.Interface.RemoveButton(b);

			//mxd. Export menu
			foreach(ToolStripMenuItem i in exportmenuitems)
				General.Interface.RemoveMenu(i);

			//mxd. Copy-paste propserties items
			foreach(ToolStripItem i in propsmenuitems)
				General.Interface.RemoveMenu(i);
		}

		// This hides all menus
		public void HideAllMenus()
		{
			foreach(ToolStripMenuItem m in menus) m.Visible = false;
		}
		
		// This hides all except one menu
		public void HideAllMenusExcept(ToolStripMenuItem showthis)
		{
			HideAllMenus();
			showthis.Visible = true;
		}
		
		// This shows the menu for the current editing mode
		public void ShowEditingModeMenu(EditMode mode)
		{
			Type sourcemode = typeof(object);
			if(mode != null)
			{
				sourcemode = mode.GetType();

				// When in a volatile mode, check against the last stable mode
				if(mode.Attributes.Volatile) sourcemode = General.Editing.PreviousStableMode;
			}
			
			// Final decision
			bool showcopyprops = true; //mxd
			if(sourcemode == typeof(LinedefsMode)) HideAllMenusExcept(linedefsmenu);
			else if(sourcemode == typeof(SectorsMode)) HideAllMenusExcept(sectorsmenu);
			else if(sourcemode == typeof(ThingsMode)) HideAllMenusExcept(thingsmenu); //mxd
			else if(sourcemode == typeof(VerticesMode)) HideAllMenusExcept(vertsmenu); //mxd
			else
			{
				HideAllMenus();
				showcopyprops = false; //mxd
			}

			//mxd. Copy-paste properties items
			foreach(ToolStripItem i in propsmenuitems)
				i.Visible = showcopyprops;
		}

		// This invokes an action from control event
		private void InvokeTaggedAction(object sender, EventArgs e)
		{
			General.Interface.InvokeTaggedAction(sender, e);
		}

		// View selection numbers clicked
		private void buttonselectionnumbers_Click(object sender, EventArgs e)
		{
			BuilderPlug.Me.ViewSelectionNumbers = buttonselectionnumbers.Checked;
			General.Interface.RedrawDisplay();
			General.Interface.DisplayStatus(StatusType.Info, (buttonselectionnumbers.Checked ?
				"Show selection numbers" :
				"Don't show selection numbers"));
		}

		//mxd
		private void buttonselectioneffects_Click(object sender, EventArgs e) 
		{
			BuilderPlug.Me.ViewSelectionEffects = buttonselectioneffects.Checked;
			General.Interface.RedrawDisplay();
			General.Interface.DisplayStatus(StatusType.Info, (buttonselectioneffects.Checked ?
				"Show sector tags and effects" :
				"Don't show sector tags and effects"));
		}

		//mxd
		private void buttonMarqueSelectTouching_Click(object sender, EventArgs e) 
		{
			BuilderPlug.Me.MarqueSelectTouching = buttonMarqueSelectTouching.Checked;
			General.Interface.DisplayStatus(StatusType.Info, (buttonMarqueSelectTouching.Checked ? 
				"Select map elements touching selection rectangle" :
				"Select map elements inside of selection rectangle"));
		}

		//mxd
		private void buttonTextureOffsetLock_Click(object sender, EventArgs e) 
		{
			BuilderPlug.Me.LockSectorTextureOffsetsWhileDragging = buttonTextureOffsetLock.Checked;
			General.Interface.DisplayStatus(StatusType.Info, (buttonTextureOffsetLock.Checked ? 
				"Lock texture offsets when dragging sectors" : 
				"Don't lock texture offsets when dragging sectors"));
		}

		//mxd
		private void linedefsmenu_DropDownOpening(object sender, EventArgs e) 
		{
			aligntexturesitem.Enabled = General.Map.UDMF;
			updatelightfogitem.Enabled = General.Map.UDMF;
		}

		//mxd
		private void gradientMode_DropDownClosed(object sender, EventArgs e) 
		{
			General.Interface.FocusDisplay();
		}

		//mxd
		private void buttonDragThingsInSelectedSectors_Click(object sender, EventArgs e) 
		{
			BuilderPlug.Me.DragThingsInSectorsMode = buttonDragThingsInSelectedSectors.Checked;
			General.Interface.DisplayStatus(StatusType.Info, (buttonDragThingsInSelectedSectors.Checked ? 
				"Drag things in selected sectors" : 
				"Don't drag things in selected sectors"));
		}

		#endregion
	}
}