
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
using CodeImp.DoomBuilder.Controls;
using CodeImp.DoomBuilder.Data;
using CodeImp.DoomBuilder.Config;
using CodeImp.DoomBuilder.Rendering;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using SlimDX.Direct3D;

#endregion

namespace CodeImp.DoomBuilder.Interface
{
	internal class ImageBrowserItem : ListViewItem
	{
		#region ================== Variables

		// Display image
		public ImageData icon;

		// Group
		private ListViewGroup listgroup;
		
		// Image cache
		private Image image;
		private bool imageloaded;
		private bool imageselected;
		
		#endregion

		#region ================== Properties

		public ListViewGroup ListGroup { get { return listgroup; } set { listgroup = value; } }

		#endregion

		#region ================== Constructor / Disposer

		// Constructors
		public ImageBrowserItem(string text, ImageData icon, object tag)
		{
			// Initialize
			this.Text = text;
			this.icon = icon;
			this.Tag = tag;
		}
		
		#endregion
		
		#region ================== Methods
		
		// This checks if a redraw is needed
		public bool CheckRedrawNeeded(Rectangle bounds)
		{
			return ((image == null) || (image.Size != bounds.Size) ||
				(this.Selected != imageselected) || (icon.IsLoaded && !imageloaded));
		}
		
		// This requests the cached image and redraws it if needed
		public Image GetImage(Rectangle bounds)
		{
			Brush forecolor;
			Brush backcolor;
			
			// Do we need to redraw?
			if(CheckRedrawNeeded(bounds))
			{
				// Keep settings
				this.imageloaded = icon.IsLoaded;
				this.imageselected = this.Selected;
				
				// Trash old image
				if(image != null) image.Dispose();

				// Make a new image and graphics to draw with
				image = new Bitmap(bounds.Width, bounds.Height, PixelFormat.Format32bppArgb);
				Graphics g = Graphics.FromImage(image);
				g.CompositingQuality = CompositingQuality.HighSpeed;
				g.InterpolationMode = InterpolationMode.Bilinear;
				g.SmoothingMode = SmoothingMode.HighQuality;
				g.PixelOffsetMode = PixelOffsetMode.None;
				
				// Determine coordinates
				SizeF textsize = g.MeasureString(this.Text, this.ListView.Font, bounds.Width);
				Size bordersize = new Size((bounds.Width - 64) >> 1, (bounds.Height - 64 - (int)textsize.Height) >> 1);
				Rectangle imagerect = new Rectangle(bordersize.Width, bordersize.Height, 64, 64);
				PointF textpos = new PointF(((float)bounds.Width - textsize.Width) * 0.5f, bounds.Height - textsize.Height - 2);

				// Determine colors
				if(this.Selected)
				{
					// Highlighted
					backcolor = new LinearGradientBrush(new Point(0, 0), new Point(0, bounds.Height),
						AdjustedColor(SystemColors.Highlight, 0.2f),
						AdjustedColor(SystemColors.Highlight, -0.1f));
					forecolor = SystemBrushes.HighlightText;
				}
				else
				{
					// Normal
					backcolor = new SolidBrush(base.ListView.BackColor);
					forecolor = new SolidBrush(base.ListView.ForeColor);
				}

				// Draw!
				g.FillRectangle(backcolor, 0, 0, bounds.Width, bounds.Height);
				g.DrawImage(icon.Bitmap, General.MakeZoomedRect(icon.Bitmap.Size, imagerect));
				g.DrawString(this.Text, this.ListView.Font, forecolor, textpos);
				
				// Done
				g.Dispose();
			}

			// Return image
			return image;
		}

		// This brightens or darkens a color
		private Color AdjustedColor(Color c, float amount)
		{
			ColorValue cc = ColorValue.FromColor(c);
			
			// Adjust color
			cc.Red = Saturate((cc.Red * (1f + amount)) + (amount * 0.5f));
			cc.Green = Saturate((cc.Green * (1f + amount)) + (amount * 0.5f));
			cc.Blue = Saturate((cc.Blue * (1f + amount)) + (amount * 0.5f));
			
			// Return result
			return Color.FromArgb(cc.ToArgb());
		}

		// This clamps a value between 0 and 1
		private float Saturate(float v)
		{
			if(v < 0f) return 0f; else if(v > 1f) return 1f; else return v;
		}
		
		#endregion
	}
}
