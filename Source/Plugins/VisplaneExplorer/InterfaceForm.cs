#region === Copyright (c) 2010 Pascal van der Heiden ===

using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using CodeImp.DoomBuilder.Windows;

#endregion

namespace CodeImp.DoomBuilder.Plugins.VisplaneExplorer
{
	public partial class InterfaceForm : DelayedForm
	{
		#region ================== Constants

		#endregion

		#region ================== Variables

		private ViewStats viewstats;
		private static bool opendoors; //mxd
		private static bool showheatmap; //mxd
		private Point oldttposition;

		#endregion

		#region ================== Properties

		internal ViewStats ViewStats { get { return viewstats; } }
		internal bool OpenDoors { get { return opendoors; } } //mxd
		internal bool ShowHeatmap { get { return showheatmap; } } //mxd

		#endregion

		#region ================== Constructor / Destructor

		// Constructor
		public InterfaceForm()
		{
			InitializeComponent();
			cbopendoors.Checked = opendoors; //mxd
			cbheatmap.Checked = showheatmap; //mxd
		}

		#endregion

		#region ================== Methods

		// This adds the buttons to the toolbar
		public void AddToInterface()
		{
			General.Interface.AddButton(statsbutton);
			General.Interface.AddButton(separator); //mxd
			General.Interface.AddButton(cbopendoors); //mxd
			General.Interface.AddButton(cbheatmap); //mxd
		}

		// This removes the buttons from the toolbar
		public void RemoveFromInterface()
		{
			General.Interface.RemoveButton(cbheatmap); //mxd
			General.Interface.RemoveButton(cbopendoors); //mxd
			General.Interface.RemoveButton(separator); //mxd
			General.Interface.RemoveButton(statsbutton);
		}

		// This shows a tooltip
		public void ShowTooltip(string text, Point p)
		{
			Point sp = General.Interface.Display.PointToScreen(p);
			Point fp = (General.Interface as Form).Location;
			Point tp = new Point(sp.X - fp.X, sp.Y - fp.Y);

			if(oldttposition != tp)
			{
				tooltip.Show(text, General.Interface, tp);
				oldttposition = tp;
			}
		}

		// This hides the tooltip
		public void HideTooltip()
		{
			tooltip.Hide(General.Interface);
		}

		#endregion

		#region ================== Events

		// Selecting a type of stats to view
		private void stats_Click(object sender, EventArgs e)
		{
			foreach(ToolStripMenuItem i in statsbutton.DropDownItems)
				i.Checked = false;
			
			ToolStripMenuItem item = (ToolStripMenuItem)sender;
			viewstats = (ViewStats)int.Parse(item.Tag.ToString(), CultureInfo.InvariantCulture);
			item.Checked = true;
			statsbutton.Image = item.Image;

			General.Interface.RedrawDisplay();
		}

		//mxd
		private void cbheatmap_Click(object sender, EventArgs e)
		{
			showheatmap = cbheatmap.Checked;
			General.Interface.RedrawDisplay();
		}

		//mxd
		private void cbopendoors_Click(object sender, EventArgs e)
		{
			opendoors = cbopendoors.Checked;
			
			// Restart processing 
			BuilderPlug.VPO.Restart();
			General.Interface.RedrawDisplay();
		}

		#endregion
	}
}
