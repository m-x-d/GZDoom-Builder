
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
		private ToolStripItem[] menus;

		// mxd. More menus
		private ToolStripItem[] exportmenuitems;

		// Buttons list
		private ToolStripItem[] buttons;

		//mxd
		public struct BrightnessGradientModes
		{
			public static string Sectors = "Sectors";
			public static string Light = "Light";
			public static string Fade = "Fade";
			public static string Floors = "Floors";
			public static string Ceilings = "Ceilings";
		}

		#endregion

		#region ================== Properties

		public ToolStripMenuItem LinedefsMenu { get { return linedefsmenu; } }
		public ToolStripMenuItem SectorsMenu { get { return sectorsmenu; } }
		public ToolStripButton ViewSelectionNumbers { get { return buttonselectionnumbers; } }
		public ToolStripButton ViewSelectionEffects { get { return buttonselectioneffects; } }
		public ToolStripSeparator SeparatorSectors1 { get { return separatorsectors1; } }
		public ToolStripButton MakeGradientBrightness { get { return buttonbrightnessgradient; } }
		public ToolStripButton MakeGradientFloors { get { return buttonfloorgradient; } }
		public ToolStripButton MakeGradientCeilings { get { return buttonceilinggradient; } }
		public ToolStripButton FlipSelectionV { get { return buttonflipselectionv; } }
		public ToolStripButton FlipSelectionH { get { return buttonflipselectionh; } }
		public ToolStripButton CurveLinedefs { get { return buttoncurvelinedefs; } }
		public ToolStripButton CopyProperties { get { return buttoncopyproperties; } }
		public ToolStripButton PasteProperties { get { return buttonpasteproperties; } }
		public ToolStripSeparator SeparatorCopyPaste { get { return seperatorcopypaste; } }
		public ToolStripComboBox BrightnessGradientMode { get { return brightnessGradientMode; } } //mxd
		public ToolStripButton MarqueSelectTouching { get { return buttonMarqueSelectTouching; } } //mxd
		public ToolStripButton AlignThingsToWall { get { return buttonAlignThingsToWall; } } //mxd
		public ToolStripButton TextureOffsetLock { get { return buttonTextureOffsetLock; } } //mxd
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
			brightnessGradientMode.Items.AddRange(new[] { BrightnessGradientModes.Sectors, BrightnessGradientModes.Light, BrightnessGradientModes.Fade, BrightnessGradientModes.Ceilings, BrightnessGradientModes.Floors });
			brightnessGradientMode.SelectedIndex = 0;
			
			// List all menus
			menus = new ToolStripItem[menustrip.Items.Count];
			for(int i = 0; i < menustrip.Items.Count; i++) menus[i] = menustrip.Items[i];

			//mxd
			exportmenuitems = new ToolStripItem[exportStripMenuItem.DropDownItems.Count];
			for(int i = 0; i < exportStripMenuItem.DropDownItems.Count; i++)
				exportmenuitems[i] = exportStripMenuItem.DropDownItems[i];

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
			foreach(ToolStripMenuItem m in menus)
				General.Interface.AddMenu(m);
			
			// Add the buttons to the core
			foreach(ToolStripItem b in buttons)
				General.Interface.AddButton(b);

			//mxd
			foreach(ToolStripMenuItem menu in exportmenuitems)
				General.Interface.AddMenu(menu, MenuSection.FileExport);
		}

		// This unregisters from the core
		public void Unregister()
		{
			// Remove the menus from the core
			foreach(ToolStripMenuItem m in menus)
				General.Interface.RemoveMenu(m);

			// Remove the buttons from the core
			foreach(ToolStripItem b in buttons)
				General.Interface.RemoveButton(b);

			//mxd
			foreach(ToolStripMenuItem menu in exportmenuitems)
				General.Interface.RemoveMenu(menu);
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
			if(sourcemode == typeof(LinedefsMode)) HideAllMenusExcept(linedefsmenu);
			else if(sourcemode == typeof(SectorsMode)) HideAllMenusExcept(sectorsmenu);
			else if(sourcemode == typeof(ThingsMode)) HideAllMenusExcept(thingsmenu); //mxd
			else if(sourcemode == typeof(VerticesMode)) HideAllMenusExcept(vertsmenu); //mxd
			else HideAllMenus();
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
		}

		//mxd
		private void buttonselectioneffects_Click(object sender, EventArgs e) {
			BuilderPlug.Me.ViewSelectionEffects = buttonselectioneffects.Checked;
			General.Interface.RedrawDisplay();
		}

		//mxd
		private void buttonMarqueSelectTouching_Click(object sender, EventArgs e) {
			BuilderPlug.Me.MarqueSelectTouching = buttonMarqueSelectTouching.Checked;
		}

		//mxd
		private void buttonTextureOffsetLock_Click(object sender, EventArgs e) {
			BuilderPlug.Me.LockSectorTextureOffsetsWhileDragging = buttonTextureOffsetLock.Checked;
		}

		//mxd
		private void linedefsmenu_DropDownOpening(object sender, EventArgs e) {
			alignLinedefsItem.Enabled = General.Map.UDMF;
		}

		//mxd
		private void brightnessGradientMode_DropDownClosed(object sender, EventArgs e) {
			General.Interface.FocusDisplay();
		}

		#endregion
	}
}