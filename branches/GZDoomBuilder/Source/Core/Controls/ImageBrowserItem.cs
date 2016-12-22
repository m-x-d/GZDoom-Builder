#region ================== Namespaces

using System;
using System.Drawing;
using CodeImp.DoomBuilder.Data;

#endregion

namespace CodeImp.DoomBuilder.Controls
{
	#region ================== mxd. ImageBrowserItemType

	internal enum ImageBrowserItemType
	{
		// Values order is used when sorting ImageBrowserItems!
		FOLDER_UP,
		FOLDER,
		IMAGE,
	}

	#endregion

	internal class ImageBrowserItem : IComparable<ImageBrowserItem>
	{
		#region ================== Variables

		protected ImageData icon;
		private bool imageloaded;
		private bool showfullname;
		protected ImageBrowserItemType itemtype;
		private string tooltip;

		#endregion

		#region ================== Properties

		public ImageData Icon { get { return icon; } }
		public ImageBrowserItemType ItemType { get { return itemtype; } }
		public virtual bool IsPreviewLoaded { get { return imageloaded; } }
		public bool ShowFullName { set { showfullname = value; } }
		public virtual string TextureName { get { return (showfullname ? icon.Name : icon.ShortName); } }
		public string ToolTip { get { return tooltip; } }

		#endregion

		#region ================== Constructor

		// Constructors
		protected ImageBrowserItem() { } //mxd. Needed for inheritance...
		public ImageBrowserItem(ImageData icon, string tooltip, bool showfullname)
		{
			// Initialize
			this.icon = icon;
			this.itemtype = ImageBrowserItemType.IMAGE; //mxd
			this.showfullname = showfullname; //mxd
			this.imageloaded = icon.IsPreviewLoaded; //mxd
			this.tooltip = tooltip; //mxd
		}

		#endregion

		#region ================== Methods

		internal bool CheckRedrawNeeded()
		{
			if(icon.IsPreviewLoaded != imageloaded)
			{
				imageloaded = icon.IsPreviewLoaded;
				return true;
			}
			return false;
		}

		internal void Draw(Graphics g, Image bmp, int x, int y, int w, int h, bool selected)
		{
			if(bmp == null) return;

			var iw = bmp.Width;
			var ih = bmp.Height;

			if(iw > w && iw >= ih)
			{
				ih = (int)Math.Floor(h * (ih / (float)iw));
				iw = w;
			}
			else if(ih > h)
			{
				iw = (int)Math.Floor(w * (iw / (float)ih));
				ih = h;
			}

			int ix = (iw < w ? x + (w - iw) / 2 : x + 1);
			int iy = (ih < h ? y + (h - ih) / 2 : y + 1);

			// Pick colors and brushes
			Brush bgbrush, fgbrush, selectedbgbrush, selectionbrush;
			Color bgcolor;
			Pen selection;
			if(General.Settings.BlackBrowsers)
			{
				bgcolor = Color.Black;
				bgbrush = Brushes.Black;
				fgbrush = Brushes.White;
				selectedbgbrush = Brushes.Gray;
				selection = Pens.Red;
				selectionbrush = Brushes.Red;
			}
			else
			{
				bgcolor = SystemColors.Window;
				bgbrush = SystemBrushes.Window;
				fgbrush = SystemBrushes.ControlText;
				selectedbgbrush = SystemBrushes.ActiveCaption;
				selection = SystemPens.HotTrack;
				selectionbrush = SystemBrushes.HotTrack;
			}

			// Item bg
			g.FillRectangle(bgbrush, x - 3, y - 3, w + 6, h + 10 + SystemFonts.MessageBoxFont.Height);

			// Selected image bg
			if(selected) g.FillRectangle(selectedbgbrush, x - 1, y - 1, w + 2, h + 2);

			// Image
			g.DrawImage(bmp, ix, iy, iw, ih);

			// Frame
			if(selected)
			{
				g.DrawRectangle(selection, x - 1, y - 1, w + 1, h + 1);
				g.DrawRectangle(selection, x - 2, y - 2, w + 3, h + 3);
			}
			else
			{
				g.DrawRectangle(Pens.Gray, x - 2, y - 2, w + 3, h + 3);
			}

			// Image name
			g.DrawString(TextureName, SystemFonts.MessageBoxFont, (selected ? selectionbrush : fgbrush), x - 2, y + h + 3);

			// Image size
			if(General.Settings.ShowTextureSizes && icon.IsPreviewLoaded && itemtype == ImageBrowserItemType.IMAGE)
			{
				string imagesize = Math.Abs(icon.ScaledWidth) + "x" + Math.Abs(icon.ScaledHeight);
				SizeF textsize = g.MeasureString(imagesize, SystemFonts.MessageBoxFont);
				textsize.Width += 2;
				textsize.Height -= 2;

				// Draw bg
				using(Brush bg = new SolidBrush(Color.FromArgb(192, bgcolor)))
				{
					g.FillRectangle(bg, x, y, textsize.Width, textsize.Height);
				}

				// Draw text
				g.DrawString(imagesize, SystemFonts.MessageBoxFont, (selected ? selectionbrush : fgbrush), x, y - 1);
			}
		}

		// Comparer
		public int CompareTo(ImageBrowserItem other)
		{
			if(itemtype != other.itemtype) return ((int)itemtype).CompareTo((int)other.itemtype);
			return this.TextureName.ToUpperInvariant().CompareTo(other.TextureName.ToUpperInvariant());
		}

		#endregion
	}
}
