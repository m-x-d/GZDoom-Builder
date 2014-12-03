
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

using System.Drawing;
using System.Windows.Forms;

#endregion

namespace CodeImp.DoomBuilder.Controls
{
	public class RenderTargetControl : Panel
	{
		#region ================== Constants

		#endregion

		#region ================== Variables

		#endregion

		#region ================== Properties
		public event KeyEventHandler OnKeyReleased; //mxd. Sometimes it's handeled here, not by MainForm
		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		internal RenderTargetControl()
		{
			// Initialize
			this.SetStyle(ControlStyles.FixedWidth, true);
			this.SetStyle(ControlStyles.FixedHeight, true);
		}

		#endregion

		#region ================== Overrides
		
		// Paint method
		protected override void OnPaint(PaintEventArgs pe)
		{
			// Pass on to base
			// Do we really want this?
			base.RaisePaintEvent(this, pe);
		}

		//mxd
		protected override void OnKeyUp(KeyEventArgs e) 
		{
			if(OnKeyReleased != null) OnKeyReleased(this, e);
		}
		
		#endregion

		#region ================== Methods

		// This sets up the control to display the splash logo
		public void SetSplashLogoDisplay()
		{
			// Change display to show splash logo
			this.SetStyle(ControlStyles.SupportsTransparentBackColor, false);
			this.SetStyle(ControlStyles.ContainerControl, true);
			this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
			this.SetStyle(ControlStyles.UserPaint, true);
			this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			this.SetStyle(ControlStyles.Opaque, false);
			this.UpdateStyles();
			this.BackColor = SystemColors.AppWorkspace;
			this.BackgroundImage = Properties.Resources.Splash3_trans;
			this.BackgroundImageLayout = ImageLayout.Center;
		}
		
		// This sets up the control for manual rendering
		public void SetManualRendering()
		{
			// Change display for rendering
			this.SetStyle(ControlStyles.SupportsTransparentBackColor, false);
			this.SetStyle(ControlStyles.ContainerControl, true);
			this.SetStyle(ControlStyles.OptimizedDoubleBuffer, false);
			this.SetStyle(ControlStyles.UserPaint, true);
			this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			this.SetStyle(ControlStyles.Opaque, true);
			this.UpdateStyles();
			this.BackColor = Color.Black;
			this.BackgroundImage = null;
			this.BackgroundImageLayout = ImageLayout.None;
		}

		#endregion
	}
}
