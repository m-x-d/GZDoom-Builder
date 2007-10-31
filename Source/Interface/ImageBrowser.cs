
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

#endregion

namespace CodeImp.DoomBuilder.Interface
{
	public partial class ImageBrowser : UserControl
	{
		#region ================== Constants

		private const int ITEM_WIDTH = 80;
		private const int ITEM_HEIGHT = 92;

		#endregion

		#region ================== Variables

		// Graphics device for rendering
		private D3DDevice graphics;
		
		// Items list
		private List<ImageBrowserItem> items;
		
		#endregion

		#region ================== Properties

		public int Count { get { return items.Count; } }

		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		public ImageBrowser()
		{
			// Initialize
			InitializeComponent();
			
			// Make items list
			items = new List<ImageBrowserItem>();

			rendertarget.SetBounds(0, 0, 0, 10000, BoundsSpecified.Height);
			rendertarget.ClientSize = new Size(0, 10000);
		}

		// Destructor
		~ImageBrowser()
		{
			// Clean up
			if(graphics != null) graphics.Dispose();
		}

		#endregion

		#region ================== Events

		// When resized
		protected override void OnResize(EventArgs e)
		{
			// Redraw
			Redraw();
			
			// Call base
			base.OnResize(e);
		}

		#endregion
		
		#region ================== Rendering

		// Initialize
		public void InitializeGraphics()
		{
			// Make graphics device
			graphics = new D3DDevice(rendertarget);
			graphics.Initialize();
		}
		
		// This redraws the list
		public void Redraw()
		{
			int numitemswide;

			// Calculate number of items wide
			numitemswide = (int)Math.Floor((float)ClientSize.Width / (float)ITEM_WIDTH);
		}
		
		#endregion
		
		#region ================== Methods

		// This adds an item
		public void Add(string text, ImageData image, object tag)
		{
			ImageBrowserItem i;

			// Make new item
			i = new ImageBrowserItem(text, image, tag);
			
			// Add item to list
			items.Add(i);
		}

		#endregion
	}
}
