
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
using SlimDX.Direct3D9;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using CodeImp.DoomBuilder.Map;

#endregion

namespace CodeImp.DoomBuilder.Interface
{
	public class FlatSelectorControl : ImageSelectorControl
	{
		// This finds the image we need for the given flat name
		protected override Image FindImage(string name)
		{
			// Check if name is a "none" texture
			if((name.Length < 1) || (name[0] == '-'))
			{
				// Flat required!
				return CodeImp.DoomBuilder.Properties.Resources.MissingTexture;
			}
			else
			{
				// Set the image
				return General.Map.Data.GetFlatBitmap(name);
			}
		}

		// This browses for a flat
		protected override string BrowseImage(string name)
		{
			string result;

			// Browse for texture
			result = FlatBrowserForm.Browse(this.ParentForm, name);
			if(result != null) return result; else return name;
		}
	}
}
