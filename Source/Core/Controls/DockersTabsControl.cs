
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
using System.Windows.Forms.VisualStyles;
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
using CodeImp.DoomBuilder.Types;

#endregion

namespace CodeImp.DoomBuilder.Controls
{
	internal class DockersTabsControl : TabControl
	{
		#region ================== Constants

		#endregion

		#region ================== Variables

		private Bitmap tabsimage;

		#endregion

		#region ================== Properties

		#endregion

		#region ================== Constructor

		// Constructor
		public DockersTabsControl()
		{
			if(VisualStyleInformation.IsSupportedByOS && VisualStyleInformation.IsEnabledByUser)
			{
				this.SetStyle(ControlStyles.SupportsTransparentBackColor, false);
				this.SetStyle(ControlStyles.UserPaint, true);
				this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
				this.SetStyle(ControlStyles.Opaque, true);
				this.UpdateStyles();
			}
		}

		// Disposer
		protected override void Dispose(bool disposing)
		{
			if(tabsimage != null)
			{
				tabsimage.Dispose();
				tabsimage = null;
			}
			
			base.Dispose(disposing);
		}
		
		#endregion

		#region ================== Methods

		// This redraws the tabs
		protected unsafe void RedrawTabs()
		{
			// Determine length and width in pixels
			int tabslength = this.ItemSize.Width * this.TabPages.Count;
			int tabswidth = this.ItemSize.Height;
			
			// Dispose old image
			if(tabsimage != null)
			{
				tabsimage.Dispose();
				tabsimage = null;
			}
			
			// Create images
			tabsimage = new Bitmap(tabswidth, tabslength, PixelFormat.Format32bppArgb);
			Bitmap drawimage = new Bitmap(tabslength, tabswidth, PixelFormat.Format32bppArgb);
			Graphics g = Graphics.FromImage(drawimage);

			if(VisualStyleInformation.IsSupportedByOS && VisualStyleInformation.IsEnabledByUser)
			{
				// Render the tabs
				for(int i = 0; i < this.TabPages.Count; i++)
				{
					VisualStyleRenderer renderer = new VisualStyleRenderer(VisualStyleElement.Tab.TabItem.Normal);
					Rectangle tabrect = this.GetTabRect(i);
					renderer.DrawBackground(g, new Rectangle(i * this.ItemSize.Width, 0, this.ItemSize.Width, this.ItemSize.Height));
				}
				
				// Rotate the image and copy to tabsimage
				BitmapData drawndata = drawimage.LockBits(new Rectangle(0, 0, drawimage.Size.Width, drawimage.Size.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
				BitmapData targetdata = tabsimage.LockBits(new Rectangle(0, 0, tabsimage.Size.Width, tabsimage.Size.Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
				int* dd = (int*)drawndata.Scan0.ToPointer();
				int* td = (int*)targetdata.Scan0.ToPointer();
				for(int y = 0; y < drawndata.Height; y++)
				{
					for(int x = 0; x < drawndata.Width; x++)
					{
						td[x * targetdata.Width + y] = *dd;
						dd++;
					}
				}
				drawimage.UnlockBits(drawndata);
				tabsimage.UnlockBits(targetdata);
			}

			// Clean up
			g.Dispose();
			drawimage.Dispose();
		}

		#endregion

		#region ================== Events

		protected override void OnPaint(PaintEventArgs e)
		{
			Point p;

			if(VisualStyleInformation.IsSupportedByOS && VisualStyleInformation.IsEnabledByUser)
			{
				e.Graphics.Clear(SystemColors.Control);

				RedrawTabs();

				if(this.Alignment == TabAlignment.Left)
				{
					p = new Point(0, 0);
				}
				else
				{
					int left = this.ClientSize.Width - tabsimage.Size.Width;
					if(left < 0) left = 0;
					p = new Point(left, 0);
				}

				e.Graphics.DrawImage(tabsimage, p);
			}
			else
			{
				base.OnPaint(e);
			}
		}
		
		#endregion
	}
}
