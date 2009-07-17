
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
using Microsoft.Win32;
using System.Diagnostics;
using CodeImp.DoomBuilder.Actions;
using CodeImp.DoomBuilder.Data;
using CodeImp.DoomBuilder.Config;
using CodeImp.DoomBuilder.Rendering;
using SlimDX.Direct3D9;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using CodeImp.DoomBuilder.Map;
using System.Globalization;
using System.Windows.Forms.VisualStyles;
using System.Collections.Specialized;

#endregion

namespace CodeImp.DoomBuilder.Controls
{
	public partial class DockersControl : UserControl
	{
		#region ================== Constants
		
		private const int TAB_ITEM_LENGTH = 150;
		private const int TAB_ITEM_SPACING = 6;
		private const int TAB_ITEM_HEIGHT = 26;
		
		public enum DockerOrientation
		{
			Left,
			Right
		};
		
		#endregion
		
		#region ================== Variables
		
		private DockerOrientation orientation;
		
		#endregion

		#region ================== Properties
		
		public DockerOrientation Orientation { get { return orientation; } set { orientation = value; } }
		
		#endregion
		
		#region ================== Constructor

		// Constructor
		public DockersControl()
		{
			InitializeComponent();
		}
		
		#endregion
		
		#region ================== Methods
		
		#endregion
		
		#region ================== Events
		
		// Draw the tabs
		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);
			
			VisualStyleRenderer tabdrawer = new VisualStyleRenderer(VisualStyleElement.Tab.TabItem.Pressed);
			tabdrawer.DrawBackground(e.Graphics, new Rectangle(0, 0, 150, 26));
			
		}
		
		#endregion
	}
}
