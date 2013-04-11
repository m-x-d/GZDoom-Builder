
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

using System.Windows.Forms;
using CodeImp.DoomBuilder.Windows;

#endregion

namespace CodeImp.DoomBuilder.BuilderModes
{
	public partial class PreferencesForm : Form
	{
		#region ================== Variables

		private PreferencesController controller;

		#endregion

		#region ================== Properties

		#endregion

		#region ================== Constructor / Disposer

		// Contrustor
		public PreferencesForm()
		{
			InitializeComponent();

			// Apply current settings to interface
			heightbysidedef.SelectedIndex = General.Settings.ReadPluginSetting("changeheightbysidedef", 0);
			splitbehavior.SelectedIndex = General.Settings.ReadPluginSetting("splitlinebehavior", 0);
			editnewthing.Checked = General.Settings.ReadPluginSetting("editnewthing", true);
			editnewsector.Checked = General.Settings.ReadPluginSetting("editnewsector", false);
			additiveselect.Checked = General.Settings.ReadPluginSetting("additiveselect", false);
			stitchrange.Text = General.Settings.ReadPluginSetting("stitchrange", 20).ToString();
			highlightrange.Text = General.Settings.ReadPluginSetting("highlightrange", 20).ToString();
			highlightthingsrange.Text = General.Settings.ReadPluginSetting("highlightthingsrange", 10).ToString();
			splitlinedefsrange.Text = General.Settings.ReadPluginSetting("splitlinedefsrange", 10).ToString();
			autoclearselection.Checked = BuilderPlug.Me.AutoClearSelection;
			visualmodeclearselection.Checked = BuilderPlug.Me.VisualModeClearSelection;
			autodragonpaste.Checked = BuilderPlug.Me.AutoDragOnPaste;
			autoaligntexturesoncreate.Checked = BuilderPlug.Me.AutoAlignTextureOffsetsOnCreate; //mxd
			autoalignDraggedSidedefsOffsetX.Checked = BuilderPlug.Me.AutoAlignTextureOffsetsOnDrag; //mxd
			dontMoveGeometryOutsideBounds.Checked = BuilderPlug.Me.DontMoveGeometryOutsideMapBoundary; //mxd
			syncSelection.Checked = BuilderPlug.Me.SyncSelection; //mxd
		}

		#endregion

		#region ================== Events

		// When OK is pressed on the preferences dialog
		public void OnAccept(PreferencesController controller)
		{
			// Write preferred settings
			General.Settings.WritePluginSetting("changeheightbysidedef", heightbysidedef.SelectedIndex);
			General.Settings.WritePluginSetting("splitlinebehavior", splitbehavior.SelectedIndex);
			General.Settings.WritePluginSetting("editnewthing", editnewthing.Checked);
			General.Settings.WritePluginSetting("editnewsector", editnewsector.Checked);
			General.Settings.WritePluginSetting("additiveselect", additiveselect.Checked);
			General.Settings.WritePluginSetting("stitchrange", stitchrange.GetResult(0));
			General.Settings.WritePluginSetting("highlightrange", highlightrange.GetResult(0));
			General.Settings.WritePluginSetting("highlightthingsrange", highlightthingsrange.GetResult(0));
			General.Settings.WritePluginSetting("splitlinedefsrange", splitlinedefsrange.GetResult(0));
			General.Settings.WritePluginSetting("autoclearselection", autoclearselection.Checked);
			General.Settings.WritePluginSetting("visualmodeclearselection", visualmodeclearselection.Checked);
			General.Settings.WritePluginSetting("autodragonpaste", autodragonpaste.Checked);
			General.Settings.WritePluginSetting("autoaligntextureoffsetsoncreate", autoaligntexturesoncreate.Checked);//mxd
			General.Settings.WritePluginSetting("autoaligntextureoffsetsondrag", autoalignDraggedSidedefsOffsetX.Checked);//mxd
			General.Settings.WritePluginSetting("dontmovegeometryoutsidemapboundary", dontMoveGeometryOutsideBounds.Checked);//mxd
			General.Settings.WritePluginSetting("syncselection", syncSelection.Checked);//mxd
		}
		
		// When Cancel is pressed on the preferences dialog
		public void OnCancel(PreferencesController controller)
		{
		}

		#endregion

		#region ================== Methods

		// This sets up the form with the preferences controller
		public void Setup(PreferencesController controller)
		{
			this.controller = controller;

			// Add tab pages
			foreach(TabPage p in tabs.TabPages)
			{
				controller.AddTab(p);
			}

			// Bind events
			controller.OnAccept += OnAccept;
			controller.OnCancel += OnCancel;
		}

		#endregion
	}
}