﻿#region ================== Namespaces

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Windows.Forms;

#endregion

namespace CodeImp.DoomBuilder.Controls
{
	//mxd. Based on (but heavily reworked since) TextureListPanel from Sledge (https://github.com/LogicAndTrick/sledge)
	internal class ImageSelectorPanel : Panel
	{
		#region ================== Constants

		#endregion

		#region ================== Variables

		private VScrollBar scrollbar;
		private List<ImageBrowserItem> items;
		private List<ImageBrowserItem> selection;
		private List<Rectangle> rectangles;
		private ImageBrowserItem lastselecteditem;
		private int imagesize = 128;
        private bool classicview = false;
        private bool usedtexturesfirst = false;
        private string contenttype = "Textures";
		private string title;
		private int titleheight
        {
            get
            {
                return classicview ? 0 : SystemFonts.MessageBoxFont.Height + 6;
            }
        }

		//mxd. Tooltips
		private ToolTip tooltip;
		private Point lasttooltippos;
		private const int tooltipreshowdistance = 48;

		//mxd. Textures cache
		private static Dictionary<int, Dictionary<long, Image>> texturecache = new Dictionary<int, Dictionary<long, Image>>(); // <imagesize, < texture longname, preview image>>

		// Selection
		private bool allowselection;
		private bool allowmultipleselection;

		#endregion

		#region ================== Event handlers

		public delegate void ItemSelectedEventHandler(object sender, ImageBrowserItem item);
		public delegate void SelectionChangedEventHandler(object sender, List<ImageBrowserItem> selection);

		/*public event ItemSelectedEventHandler ItemSelected;
		private void OnItemSelected(ImageBrowserItem item)
		{
			if(ItemSelected != null) ItemSelected(this, item);
		}*/

		public event SelectionChangedEventHandler SelectionChanged;
		private void OnSelectionChanged(List<ImageBrowserItem> selection)
		{
			if(SelectionChanged != null) SelectionChanged(this, selection);
		}

		public event ItemSelectedEventHandler ItemDoubleClicked;
		private void OnItemDoubleClicked(ImageBrowserItem item)
		{
			if(ItemDoubleClicked != null) ItemDoubleClicked(this, item);
		}

		#endregion

		#region ================== Properties

		public bool HideSelection
		{
			get { return !allowselection; }
			set
			{
				allowselection = !value;
				if(!allowselection && selection.Count > 0)
				{
					selection.Clear();
					Refresh();
				}
			}
		}

		public bool MultiSelect
		{
			get { return allowmultipleselection; }
			set
			{
				allowmultipleselection = value;
				if(!allowmultipleselection && selection.Count > 0)
				{
					var first = selection[0];
					selection.Clear();
					selection.Add(first);
					Refresh();
				}
			}
		}

		public int ImageSize
		{
			get { return imagesize; }
			set
			{
				imagesize = value;
				UpdateRectangles();
				if(selection.Count > 0) ScrollToItem(selection[0]);
			}
		}

        public bool ClassicView
        {
            get { return classicview; }
            set
            {
                classicview = value;
                UpdateRectangles();
                if (selection.Count > 0) ScrollToItem(selection[0]);
            }
        }

        public bool UsedTexturesFirst
        {
            get { return usedtexturesfirst; }
            set
            {
                usedtexturesfirst = value;
                UpdateRectangles();
                if (selection.Count > 0) ScrollToItem(selection[0]);
            }
        }

		public List<ImageBrowserItem> Items { get { return items; } }
		public List<ImageBrowserItem> SelectedItems { get { return selection; } }
		public string Title { get { return title; } set { title = value; } }
        public string ContentType { get { return contenttype; } set { contenttype = value; } } // why do encapsulation when it's not needed?

		#endregion

		#region ================== Constructor / Disposer

		public ImageSelectorPanel()
		{
			VScroll = true;
			AutoScroll = true;
			DoubleBuffered = true;

			scrollbar = new VScrollBar { Dock = DockStyle.Right };
			scrollbar.ValueChanged += (sender, e) => Refresh();
			tooltip = new ToolTip(); //mxd
			items = new List<ImageBrowserItem>();
			selection = new List<ImageBrowserItem>();
			imagesize = 128;
			rectangles = new List<Rectangle>();
			title = "All images:";

			Controls.Add(scrollbar);
		}

		protected override void Dispose(bool disposing)
		{
			if(disposing) Clear();
			base.Dispose(disposing);
		}

		#endregion

		#region ================== Add/Remove/Get Textures

		//mxd. Clears the list without redrawing it
		public void Clear()
		{
			selection.Clear();
			items.Clear();
			lastselecteditem = null;
			rectangles.Clear();
		}

		//mxd
		public void ClearSelection()
		{
			selection.Clear();
			lastselecteditem = null;

			OnSelectionChanged(selection);
			Refresh();
		}

		public void SetItems(IEnumerable<ImageBrowserItem> items)
		{
			this.items.Clear();
			lastselecteditem = null;
			selection.Clear();
			this.items.AddRange(items);

			OnSelectionChanged(selection);
			UpdateRectangles();
		}

		public void SetSelectedItem(ImageBrowserItem item)
		{
			SetSelectedItems(new List<ImageBrowserItem> { item } );
		}

		public void SetSelectedItems(List<ImageBrowserItem> items)
		{
			selection.Clear();
			if(items.Count > 0)
			{
				selection.AddRange(items);
				ScrollToItem(items[0]); //mxd
				Refresh(); //mxd
			}
			OnSelectionChanged(selection);
		}

		public void ScrollToItem(ImageBrowserItem item)
		{
			int index = items.IndexOf(item);
			if(index < 0) return;

			Rectangle rec = rectangles[index];

			//mxd. Already visible?
			int ymin = scrollbar.Value - titleheight;
			int ymax = ymin + this.ClientRectangle.Height + titleheight;
			if(rec.Top - 3 >= ymin && rec.Bottom + 3 <= ymax) return;

			int yscroll = Math.Max(0, Math.Min(rec.Top - titleheight - 3, scrollbar.Maximum - ClientRectangle.Height));
			scrollbar.Value = yscroll;
			Refresh();
		}

		public void SelectNextItem(SearchDirectionHint dir)
		{
			if(!allowselection) return;

			if(selection.Count == 0)
			{
				if(items.Count > 0) SetSelectedItem(items[0]);
				return;
			}

			int targetindex = items.IndexOf(selection[0]);
			Rectangle rect = rectangles[targetindex];
			int index, newindex, tx, cx, cy;

			switch(dir)
			{
				case SearchDirectionHint.Right:
					// Just select the next item
					if(targetindex < items.Count - 1) SetSelectedItem(items[targetindex + 1]);
					break;

				case SearchDirectionHint.Left:
					// Just select the previous item
					if(targetindex > 0) SetSelectedItem(items[targetindex - 1]);
					break;

				case SearchDirectionHint.Up:
					// Skip current row...
					index = targetindex - 1;
					if(index < 0) break;
					while(index > 0)
					{
						if(rectangles[index].Y != rect.Y) break;
						index--;
					}

					// Check upper row for best match
					tx = rect.X + rect.Width / 2;
					cx = int.MaxValue;
					cy = rectangles[index].Y;
					newindex = int.MaxValue;

					while(index > 0 && rectangles[index].Y == cy)
					{
						int ccx = Math.Abs(rectangles[index].X + rectangles[index].Width / 2 - tx);
						if(ccx < cx)
						{
							cx = ccx;
							newindex = index;
						}
						index--;
					}

					// Select item
					if(newindex != int.MaxValue) SetSelectedItem(items[newindex]);
					break;

				case SearchDirectionHint.Down:
					// Skip current row...
					index = targetindex + 1;
					if(index > rectangles.Count - 1) break;
					while(index < rectangles.Count - 1)
					{
						if(rectangles[index].Y != rect.Y) break;
						index++;
					}

					// Check upper row for best match
					tx = rect.X + rect.Width / 2;
					cx = int.MaxValue;
					cy = rectangles[index].Y;
					newindex = int.MaxValue;

					while(index < rectangles.Count - 1 && rectangles[index].Y == cy)
					{
						int ccx = Math.Abs(rectangles[index].X + rectangles[index].Width / 2 - tx);
						if(ccx < cx)
						{
							cx = ccx;
							newindex = index;
						}
						index++;
					}

					// Select item
					if(newindex != int.MaxValue) SetSelectedItem(items[newindex]);
					break;
			}
		}

		protected override void OnMouseDoubleClick(MouseEventArgs e)
		{
			base.OnMouseDoubleClick(e);
			if(General.Interface.CtrlState || General.Interface.ShiftState || selection.Count != 1)
				return;

			int index = GetIndexAt(e.X, scrollbar.Value + e.Y);
			if(index == -1) return;

			OnItemDoubleClicked(items[index]);
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown(e);
			this.Focus();

			if(!allowselection) return;
			if(!allowmultipleselection || !General.Interface.CtrlState)
				selection.Clear();

			int x = e.X;
			int y = scrollbar.Value + e.Y;

			int clickedIndex = GetIndexAt(x, y);
			var item = (clickedIndex >= 0 && clickedIndex < items.Count ? items[clickedIndex] : null);

			if(item == null)
			{
				selection.Clear();
			}
			else if(allowmultipleselection && General.Interface.CtrlState && selection.Contains(item))
			{
				selection.Remove(item);
				lastselecteditem = null;
			}
			else if(allowmultipleselection && General.Interface.ShiftState && lastselecteditem != null)
			{
				int bef = items.IndexOf(lastselecteditem);
				var start = Math.Min(bef, clickedIndex);
				var count = Math.Abs(clickedIndex - bef) + 1;
				selection.AddRange(items.GetRange(start, count).Where(i => !selection.Contains(i)));
			}
			else
			{
				selection.Add(item);
				lastselecteditem = item;
			}

			OnSelectionChanged(selection);
			Refresh();
		}

		protected override void OnMouseEnter(EventArgs e)
		{
			Focus();
			base.OnMouseEnter(e);
		}

		//mxd
		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);

			int index = GetIndexAt(e.X, scrollbar.Value + e.Y);
			if(index == -1 || items[index].ItemType != ImageBrowserItemType.IMAGE || string.IsNullOrEmpty(items[index].ToolTip))
			{
				if(tooltip.Active) tooltip.Hide(this);
			}
			else if(!tooltip.Active || tooltip.GetToolTip(this) != items[index].ToolTip
				|| Math.Abs(lasttooltippos.X - e.Location.X) > tooltipreshowdistance
				|| Math.Abs(lasttooltippos.Y - e.Location.Y) > tooltipreshowdistance)
			{
				Point pos = new Point(e.Location.X, e.Location.Y + Cursor.Size.Height + 4);
				tooltip.Show(items[index].ToolTip, this, pos, 999999);
				lasttooltippos = e.Location;
			}
		}

		public int GetIndexAt(int x, int y)
		{
			int padhorz = classicview?13:2;
            int padvert = 2;
			int font = 4 + SystemFonts.MessageBoxFont.Height;

			for(var i = 0; i < rectangles.Count; i++)
			{
				var rec = rectangles[i];
				if(rec.Left - padhorz <= x
					&& rec.Right + padhorz >= x
					&& rec.Top - padvert <= y
					&& rec.Bottom + padvert + font >= y)
				{
					return i;
				}
			}

			return -1;
		}

		#endregion

		#region ================== Scrolling

		private void ScrollByAmount(int value)
		{
			int newvalue = Math.Max(0, scrollbar.Value + value);
			scrollbar.Value = Math.Min(newvalue, Math.Max(0, scrollbar.Maximum - ClientRectangle.Height));
		}

		protected override void OnMouseWheel(MouseEventArgs e)
		{
			base.OnMouseWheel(e);
			ScrollByAmount(scrollbar.SmallChange * (e.Delta / -120));
		}

		//mxd. Otherwise arrow keys won't be handled by OnKeyDown
		protected override bool IsInputKey(Keys keyData)
		{
			switch(keyData)
			{
				case Keys.Right: case Keys.Left: 
				case Keys.Up: case Keys.Down: 
				case Keys.Return: return true;
			}

			return base.IsInputKey(keyData);
		}

		protected override void OnKeyDown(KeyEventArgs e)
		{
			switch(e.KeyCode)
			{
				//mxd. Cursor keys
				case Keys.Left: SelectNextItem(SearchDirectionHint.Left); break;
				case Keys.Right: SelectNextItem(SearchDirectionHint.Right); break;
				case Keys.Up: SelectNextItem(SearchDirectionHint.Up); break;
				case Keys.Down: SelectNextItem(SearchDirectionHint.Down); break;

				case Keys.PageDown: ScrollByAmount(scrollbar.LargeChange); break;
				case Keys.PageUp: ScrollByAmount(-scrollbar.LargeChange); break;
				case Keys.End: ScrollByAmount(int.MaxValue); break;
				case Keys.Home: ScrollByAmount(-int.MaxValue); break;

				case Keys.Enter: if(selection.Count > 0) OnItemDoubleClicked(selection[0]); break;
			}
			
			base.OnKeyDown(e);
		}

		#endregion

		#region ================== Updating Rectangles & Dimensions

		protected override void OnResize(EventArgs e)
		{
			base.OnResize(e);
			UpdateRectangles();

			//mxd
			if(selection.Count > 0) ScrollToItem(selection[0]);
		}

		internal void UpdateRectangles()
		{
			int w = ClientRectangle.Width - scrollbar.Width;
			int padhorz = classicview?13:2;
            int padvert = 2;
			int font = 4 + SystemFonts.MessageBoxFont.Height;
			int cx = 0;
			int cy = titleheight;
			int my = 0;
			rectangles.Clear();

            //
            ImageBrowserItemType currentType = ImageBrowserItemType.IMAGE; // initialized to random value
            bool currentUsedInMap = false;
            var firstItem = (items.Count > 0) ? items[0] : null;

            foreach (var ti in items)
			{
				Image preview = GetPreview(ti, imagesize);
                if (classicview && (ti == firstItem || ((currentType == ImageBrowserItemType.IMAGE) != (ti.ItemType == ImageBrowserItemType.IMAGE)) || (usedtexturesfirst && currentUsedInMap != ti.Icon.UsedInMap)))
                {
                    // new row, also provide space for category name.
                    cx = 0;
                    cy += SystemFonts.MessageBoxFont.Height + 6 + my + ((ti != firstItem) ? 16 : 0);
                    my = 0;
                    currentType = ti.ItemType;
                    currentUsedInMap = ti.Icon.UsedInMap;
                }
				
				int rw = w - cx;
				int wid = Math.Max((imagesize > 0 ? imagesize : preview.Width), ti.TextureNameWidth) + padhorz + padhorz;
				int hei = (imagesize > 0 ? imagesize : preview.Height) + padvert + padvert + font;
				
				if(rw < wid)
				{
					// New row
					cx = 0;
					cy += my;
					my = 0;
				}

				my = Math.Max(my, hei);
				var rect = new Rectangle(cx + padhorz, cy + padvert, wid - padhorz - padhorz, hei - padvert - padvert - font);
				rectangles.Add(rect);
				cx += wid;
			}

			if(rectangles.Count > 0)
			{
				scrollbar.Maximum = cy + my;
				scrollbar.SmallChange = (imagesize > 0 ? imagesize : 128) + padvert + padvert + font;
				scrollbar.LargeChange = ClientRectangle.Height;
				scrollbar.Visible = (scrollbar.Maximum > ClientRectangle.Height);

				if(scrollbar.Value > scrollbar.Maximum - ClientRectangle.Height)
				{
					scrollbar.Value = Math.Max(0, scrollbar.Maximum - ClientRectangle.Height);
				}
			}
			else
			{
				scrollbar.Visible = false;
			}

			Refresh();
		}

		#endregion

		#region ================== Rendering

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);
			DrawTextures(e.Graphics);
		}

        private void DrawTextureHeader(Graphics g, string text, Rectangle rec, bool background)
        {
            bool blackbrowsers = (General.Settings != null && General.Settings.BlackBrowsers);

            if (background)
            {
                // Draw group name bg
                Color bgcolor = (blackbrowsers ? Color.Gray : SystemColors.Control);
                using (Brush bg = new SolidBrush(Color.FromArgb(192, bgcolor)))
                {
                    g.FillRectangle(bg, rec.X, rec.Y, rec.Width, rec.Height);
                }
            }
            else
            {
                // Draw underline
                Color underlinecolor = blackbrowsers ? Color.FromArgb(0x7FFFFFFF) : Color.FromArgb((SystemColors.ControlText.ToArgb()&0xFFFFFF)|0x7F000000);
                g.DrawLine(new Pen(underlinecolor), rec.Left, rec.Bottom, rec.Right - 2, rec.Bottom);
            }

            // Draw group name
            Brush fgbrush = (blackbrowsers ? Brushes.White : SystemBrushes.ControlText);
            Font bf = new Font(SystemFonts.MessageBoxFont, FontStyle.Bold);
            g.DrawString(text, bf, fgbrush, rec.X, rec.Y);
        }

		private void DrawTextures(Graphics g)
		{
            // Draw items
            int scrollwidth = (scrollbar.Visible ? scrollbar.Width : 0);

            if (items.Count > 0)
			{
				int y = scrollbar.Value;
				int height = ClientRectangle.Height - titleheight;

                ImageBrowserItemType currentType = ImageBrowserItemType.IMAGE; // initialized to random value
                bool currentUsedInMap = false;

                for (var i = 0; i < items.Count; i++)
				{
                    if (classicview && (i == 0 || ((currentType == ImageBrowserItemType.IMAGE) != (items[i].ItemType == ImageBrowserItemType.IMAGE)) || (usedtexturesfirst && currentUsedInMap != items[i].Icon.UsedInMap)))
                    {
                        // draw corresponding title right above this item.
                        string hdrname;
                        if (items[i].ItemType == ImageBrowserItemType.IMAGE)
                        {
                            if (usedtexturesfirst && items[i].Icon.UsedInMap) hdrname = "Used " + contenttype + ":";
                            else hdrname = "All " + contenttype + ":";
                        }
                        else hdrname = "Directories:";
                        DrawTextureHeader(g, hdrname, new Rectangle(2, rectangles[i].Y - (SystemFonts.MessageBoxFont.Height + 6) - y, ClientRectangle.Width - scrollwidth - 4, SystemFonts.MessageBoxFont.Height), false);
                        currentType = items[i].ItemType;
                        currentUsedInMap = items[i].Icon.UsedInMap;
                    }

					Rectangle rec = rectangles[i];
					if(rec.Bottom < y) continue;
					if(rec.Top > y + height) break;

					Image bmp = GetPreview(items[i], imagesize);
					items[i].Draw(g, bmp, rec.X, rec.Y - y, rec.Width, rec.Height, selection.Contains(items[i]), items[i].Icon.UsedInMap, classicview);
				}
			}

			// Draw title on top of items
			if(!string.IsNullOrEmpty(title) && !classicview)
			{
                DrawTextureHeader(g, title, new Rectangle(2, 2, ClientRectangle.Width - scrollwidth - 4, SystemFonts.MessageBoxFont.Height), true);
			}
		}

		#endregion

		#region ================== Image Caching

		private static Image GetPreview(ImageBrowserItem item, int imagesize)
		{
			if(!item.IsPreviewLoaded) return item.Icon.GetPreview();
			if(!texturecache.ContainsKey(imagesize)) texturecache.Add(imagesize, new Dictionary<long, Image>());

			// Generate preview?
			if(!texturecache[imagesize].ContainsKey(item.Icon.LongName))
			{
				Image img = item.Icon.GetPreview();
				
				// Determine preview size
				float scalex, scaley;
				if(item.ItemType == ImageBrowserItemType.IMAGE)
				{
					scalex = (imagesize == 0 ? 1.0f : (imagesize / (float)img.Width));
					scaley = (imagesize == 0 ? 1.0f :(imagesize / (float)img.Height));
				}
				else
				{
					// Don't upscale folder icons
					scalex = (imagesize == 0 ? 1.0f : ((img.Width > imagesize) ? (imagesize / (float)img.Width) : 1.0f));
					scaley = (imagesize == 0 ? 1.0f : ((img.Height > imagesize) ? (imagesize / (float)img.Height) : 1.0f));
				}
				
				float scale = Math.Min(scalex, scaley);
				int previewwidth = (int)(img.Width * scale);
				int previewheight = (int)(img.Height * scale);
				if(previewwidth < 1) previewwidth = 1;
				if(previewheight < 1) previewheight = 1;

				// Make new image
				Bitmap preview = new Bitmap(previewwidth, previewheight, PixelFormat.Format32bppArgb);
				using(Graphics g = Graphics.FromImage(preview))
				{
					g.PageUnit = GraphicsUnit.Pixel;
					g.InterpolationMode = InterpolationMode.NearestNeighbor;
					g.PixelOffsetMode = PixelOffsetMode.Half;

					g.DrawImage(img, new Rectangle(0, 0, previewwidth, previewheight));
				}

				texturecache[imagesize][item.Icon.LongName] = preview;
			}

			// Get preview
			return texturecache[imagesize][item.Icon.LongName];
		}

		#endregion
	}
}
