
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
using System.Drawing;
using System.Windows.Forms;
using CodeImp.DoomBuilder.Data;
using System.Drawing.Drawing2D;
using SlimDX;

#endregion

namespace CodeImp.DoomBuilder.Controls
{
	internal class ImageBrowserItem : ListViewItem, IComparable<ImageBrowserItem>
	{
		#region ================== Constants

		internal const int MAX_NAME_LENGTH = 14; //mxd

		#endregion
		
		#region ================== Variables

		// Display image and text
		public readonly ImageData icon;
		private string imagesize; //mxd
		private bool showfullname; //mxd
		private static readonly StringFormat format = new StringFormat { Alignment = StringAlignment.Center }; //mxd
		
		// Group
		private ListViewGroup listgroup;
		
		// Image cache
		private bool imageloaded;
		
		#endregion

		#region ================== Properties

		public ListViewGroup ListGroup { get { return listgroup; } set { listgroup = value; } }
		public bool IsPreviewLoaded { get { return imageloaded; } }
		public bool ShowFullName { set { showfullname = value; UpdateName(); } }
		public string TextureName { get { return showfullname ? icon.Name : icon.ShortName; } }

		#endregion

		#region ================== Constructor / Disposer

		// Constructors
		public ImageBrowserItem(ImageData icon, object tag, bool showfullname)
		{
			// Initialize
			this.icon = icon;
			this.Tag = tag;
			this.showfullname = showfullname; //mxd
			UpdateName(); //mxd
		}
		
		#endregion
		
		#region ================== Methods
		
		// This checks if a redraw is needed
		public bool CheckRedrawNeeded()
		{
			UpdateName(); //mxd. Update texture size if needed
			return (icon.IsPreviewLoaded != imageloaded);
		}
		
		// This draws the images
		public void Draw(Graphics g, Rectangle bounds)
		{
			Brush forecolor;
			Brush backcolor;

			// Remember if the preview is loaded
			imageloaded = icon.IsPreviewLoaded;

			// Drawing settings
			g.CompositingQuality = CompositingQuality.HighSpeed;
			g.InterpolationMode = InterpolationMode.NearestNeighbor;
			g.SmoothingMode = SmoothingMode.HighSpeed;
			g.PixelOffsetMode = PixelOffsetMode.None;

			// Determine coordinates
			SizeF textsize = g.MeasureString(Text, this.ListView.Font, bounds.Width * 2);
			Rectangle imagerect = new Rectangle(bounds.Left + ((bounds.Width - General.Map.Data.Previews.MaxImageWidth) >> 1),
				bounds.Top + ((bounds.Height - General.Map.Data.Previews.MaxImageHeight - (int)textsize.Height) >> 1),
				General.Map.Data.Previews.MaxImageWidth, General.Map.Data.Previews.MaxImageHeight);
			PointF textpos = new PointF(bounds.Left + (bounds.Width * 0.5f), bounds.Bottom - textsize.Height - 2);

			// Determine colors
			if(this.Selected)
			{
				// Highlighted
				backcolor = new LinearGradientBrush(new Point(0, bounds.Top - 1), new Point(0, bounds.Bottom + 1),
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
			g.FillRectangle(backcolor, bounds);
			icon.DrawPreview(g, imagerect.Location);
			g.DrawString(Text, this.ListView.Font, forecolor, textpos, format);

			//mxd. Draw size label?
			if (ImageBrowserControl.ShowTextureSizes && !string.IsNullOrEmpty(imagesize))
			{
				// Setup
				Font sizefont = new Font(this.ListView.Font.FontFamily, this.ListView.Font.SizeInPoints - 1);
				textsize = g.MeasureString(imagesize, sizefont, bounds.Width * 2);
				textpos = new PointF(bounds.Left + textsize.Width / 2, bounds.Top + 1);
				imagerect = new Rectangle(bounds.Left + 1, bounds.Top + 1, (int)textsize.Width, (int)textsize.Height);
				SolidBrush labelbg = new SolidBrush(Color.FromArgb(196, base.ListView.BackColor));

				// Draw
				g.FillRectangle(labelbg, imagerect);
				g.DrawString(imagesize, sizefont, new SolidBrush(base.ListView.ForeColor), textpos, format);
			}
		}

		// This brightens or darkens a color
		private static Color AdjustedColor(Color c, float amount)
		{
			Color4 cc = new Color4(c);
			
			// Adjust color
			cc.Red = Saturate((cc.Red * (1f + amount)) + (amount * 0.5f));
			cc.Green = Saturate((cc.Green * (1f + amount)) + (amount * 0.5f));
			cc.Blue = Saturate((cc.Blue * (1f + amount)) + (amount * 0.5f));
			
			// Return result
			return Color.FromArgb(cc.ToArgb());
		}

		// This clamps a value between 0 and 1
		private static float Saturate(float v)
		{
			if(v < 0f) return 0f; else if(v > 1f) return 1f; else return v;
		}

		//mxd
		private void UpdateName() 
		{
			Text = (showfullname ? icon.DisplayName : icon.ShortName);
			if(General.Settings.ShowTextureSizes && icon.IsPreviewLoaded)
				imagesize = icon.ScaledWidth + "x" + icon.ScaledHeight;
		}

		// Comparer
		public int CompareTo(ImageBrowserItem other)
		{
			return this.Text.ToUpperInvariant().CompareTo(other.Text.ToUpperInvariant());
		}

		#endregion
	}
}
