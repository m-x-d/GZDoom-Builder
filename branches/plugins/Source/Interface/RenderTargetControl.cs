
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
using System.Globalization;
using System.Windows.Forms;
using CodeImp.DoomBuilder.Controls;
using CodeImp.DoomBuilder.Geometry;
using CodeImp.DoomBuilder.Rendering;
using CodeImp.DoomBuilder.Editing;

#endregion

namespace CodeImp.DoomBuilder.Interface
{
	public class RenderTargetControl : Panel
	{
		#region ================== Constants

		#endregion

		#region ================== Variables

		// Reference to image to render from
		private Image img = null;

		#endregion

		#region ================== Properties

		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		public RenderTargetControl()
		{
			// Initialize
			this.SetStyle(ControlStyles.FixedWidth, true);
			this.SetStyle(ControlStyles.FixedHeight, true);
		}
		
		// Diposer
		protected override void Dispose(bool disposing)
		{
			// Clean up
			
			// Done
			base.Dispose(disposing);
		}

		#endregion

		#region ================== Overrides
		
		// Paint method
		protected override void OnPaint(PaintEventArgs pe)
		{
			/*
			// Copy area that needs to be redrawn
			if(img != null)
			{
				pe.Graphics.FillRectangle(Brushes.Black, pe.ClipRectangle);
				pe.Graphics.DrawImage(img, pe.ClipRectangle, pe.ClipRectangle, GraphicsUnit.Pixel);
				
			}
			*/
			
			// Pass on to base
			// Do we really want this?
			base.RaisePaintEvent(this, pe);
		}

		#endregion

		#region ================== Methods

		// This sets the render source
		public void SetImageSource(Image srcimg)
		{
			// Set new source image
			img = srcimg;
			//this.Image = img;
		}

		// This sets up the control to display the splash logo
		public void SetSplashLogoDisplay()
		{
			// Remove render image
			this.img = null;
			
			// Change display to show splash logo
			this.SetStyle(ControlStyles.SupportsTransparentBackColor, false);
			this.SetStyle(ControlStyles.ContainerControl, true);
			this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
			this.SetStyle(ControlStyles.UserPaint, true);
			this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			this.SetStyle(ControlStyles.Opaque, false);
			this.UpdateStyles();
			this.BackColor = System.Drawing.SystemColors.AppWorkspace;
			this.BackgroundImage = global::CodeImp.DoomBuilder.Properties.Resources.Splash2;
			this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			//this.Image = null;
		}
		
		// This sets up the control for manual rendering
		public void SetManualRendering()
		{
			// Change display for rendering
			/*
			this.SetStyle(ControlStyles.SupportsTransparentBackColor, false);
			this.SetStyle(ControlStyles.ContainerControl, true);
			this.SetStyle(ControlStyles.OptimizedDoubleBuffer, false);
			this.SetStyle(ControlStyles.UserPaint, true);
			this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			this.SetStyle(ControlStyles.Opaque, true);
			*/
			this.SetStyle(ControlStyles.SupportsTransparentBackColor, false);
			this.SetStyle(ControlStyles.ContainerControl, true);
			this.SetStyle(ControlStyles.OptimizedDoubleBuffer, false);
			this.SetStyle(ControlStyles.UserPaint, true);
			this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			this.SetStyle(ControlStyles.Opaque, true);
			this.UpdateStyles();
			this.BackColor = Color.Black;
			this.BackgroundImage = null;
			this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
			//this.BackgroundImage = global::CodeImp.DoomBuilder.Properties.Resources.floor0_3;
			//this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Tile;
			//this.Image = img;
		}

		#endregion
	}
}
