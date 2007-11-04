
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
using CodeImp.DoomBuilder.Map;

#endregion

namespace CodeImp.DoomBuilder.Interface
{
	public partial class TextureBrowserForm : DelayedForm
	{
		// Constructor
		public TextureBrowserForm()
		{
			Dictionary<long, long> usedtextures = new Dictionary<long,long>();
			
			// Initialize
			InitializeComponent();

			// Make groups
			ListViewGroup used = browser.AddGroup("Used Textures");
			ListViewGroup avail = browser.AddGroup("Available Textures");
			
			// Go through the map to find the used textures
			foreach(Sidedef sd in General.Map.Map.Sidedefs)
			{
				// Add high texture
				if(sd.HighTexture.Length > 0)
					if(!usedtextures.ContainsKey(sd.LongHighTexture)) usedtextures.Add(sd.LongHighTexture, 0);

				// Add mid texture
				if(sd.LowTexture.Length > 0)
					if(!usedtextures.ContainsKey(sd.LongMiddleTexture)) usedtextures.Add(sd.LongMiddleTexture, 0);

				// Add low texture
				if(sd.MiddleTexture.Length > 0)
					if(!usedtextures.ContainsKey(sd.LongLowTexture)) usedtextures.Add(sd.LongLowTexture, 0);
			}

			// Start adding
			browser.BeginAdding();

			// Add all used textures
			foreach(ImageData img in General.Map.Data.Textures)
				if(usedtextures.ContainsKey(img.LongName))
					browser.Add(img.Name, img, img, used);

			// Add all available textures
			foreach(ImageData img in General.Map.Data.Textures)
				browser.Add(img.Name, img, img, avail);

			// Done adding
			browser.EndAdding();
		}

		// Selection changed
		private void browser_SelectedItemChanged()
		{
			apply.Enabled = (browser.SelectedItem != null);
		}
	}
}