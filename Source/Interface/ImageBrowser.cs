
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

		// Number of items horizontally
		private int numitemswidth;
		
		#endregion

		#region ================== Properties

		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		public ImageBrowser()
		{
			// Make items list
			
			// Initialize
			InitializeComponent();
		}

		#endregion

		#region ================== Events

		// When resized
		protected override void OnResize(EventArgs e)
		{
			
			// Call base
			base.OnResize(e);
		}

		#endregion
		
		#region ================== Controls

		#endregion
		
		#region ================== Methods

		// This adds an item
		public void Add(string name, ImageData image, object tag)
		{
			// Make new item
			
			// Add item to list
			
		}

		#endregion
	}
}
