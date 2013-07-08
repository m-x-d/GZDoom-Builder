
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

using System.Drawing;
using CodeImp.DoomBuilder.Windows;
using CodeImp.DoomBuilder.Data;

#endregion

namespace CodeImp.DoomBuilder.Controls
{
	public class FlatSelectorControl : ImageSelectorControl
	{
		// Setup
		public override void Initialize()
		{
			base.Initialize();
			
			// Fill autocomplete list
			name.AutoCompleteCustomSource.AddRange(General.Map.Data.FlatNames.ToArray());
		}
		
		// This finds the image we need for the given flat name
		protected override Image FindImage(string imagename)
		{
			// Check if name is a "none" texture
			if((imagename.Length < 1) || (imagename[0] == '-'))
			{
				DisplayImageSize(0, 0); //mxd
				
				// Flat required!
				return CodeImp.DoomBuilder.Properties.Resources.MissingTexture;
			}
			else
			{
				ImageData texture = General.Map.Data.GetFlatImage(imagename); //mxd

				if(string.IsNullOrEmpty(texture.FullName)) DisplayImageSize(0, 0); //mxd
				else DisplayImageSize(texture.ScaledWidth, texture.ScaledHeight); //mxd
				
				// Set the image
				return texture.GetPreview();
			}
		}

		// This browses for a flat
		protected override string BrowseImage(string imagename)
		{
			string result;

			// Browse for texture
			//result = FlatBrowserForm.Browse(this.ParentForm, imagename);
            result = TextureBrowserForm.Browse(this.ParentForm, imagename, true); //mxd
			if(result != null) return result; else return imagename;
		}
	}
}
