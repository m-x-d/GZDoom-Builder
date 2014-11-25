
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
			timer.Stop(); //mxd

			// Check if name is a "none" texture
			if(string.IsNullOrEmpty(imagename))
			{
				DisplayImageSize(0, 0); //mxd
				
				//mxd. Flat required?
				return multipletextures ? Properties.Resources.ImageStack : Properties.Resources.MissingTexture;
			} 
			else if(imagename == "-") //mxd
			{
				DisplayImageSize(0, 0);

				// Flat required!
				return Properties.Resources.MissingTexture;
			}
			else
			{
				ImageData texture = General.Map.Data.GetFlatImage(imagename); //mxd

				if(string.IsNullOrEmpty(texture.FullName) || texture is UnknownImage) DisplayImageSize(0, 0); //mxd
				else DisplayImageSize(texture.ScaledWidth, texture.ScaledHeight); //mxd
				if(usepreviews ? !texture.IsPreviewLoaded : !texture.IsImageLoaded) timer.Start(); //mxd
				
				// Set the image
				return (usepreviews ? texture.GetPreview() : texture.GetBitmap());
			}
		}

		// This browses for a flat
		protected override string BrowseImage(string imagename)
		{
			// Browse for texture
			string result = TextureBrowserForm.Browse(this.ParentForm, imagename, true);
			if(result != null) return result; else return imagename;
		}
	}
}
