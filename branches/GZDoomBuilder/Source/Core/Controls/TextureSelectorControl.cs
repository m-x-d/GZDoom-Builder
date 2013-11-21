
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
	public class TextureSelectorControl : ImageSelectorControl
	{
		// Variables
		private bool required;
		
		// Properties
		public bool Required { get { return required; } set { required = value; } }

		// Setup
		public override void Initialize()
		{
			base.Initialize();
			
			// Fill autocomplete list
			name.AutoCompleteCustomSource.AddRange(General.Map.Data.TextureNames.ToArray());
		}
		
		// This finds the image we need for the given texture name
		protected override Image FindImage(string imagename)
		{
			timer.Stop(); //mxd
			
			// Check if name is a "none" texture
			if((imagename.Length < 1) || (imagename == "-"))
			{
				DisplayImageSize(0, 0); //mxd
				
				// Determine image to show
				if(required)
					return CodeImp.DoomBuilder.Properties.Resources.MissingTexture;
				else
					return null;
			}
			else
			{
				ImageData texture = General.Map.Data.GetTextureImage(imagename); //mxd

				if(string.IsNullOrEmpty(texture.FullName) || texture is UnknownImage) DisplayImageSize(0, 0); //mxd
				else DisplayImageSize(texture.ScaledWidth, texture.ScaledHeight); //mxd
				if(!texture.IsPreviewLoaded) timer.Start(); //mxd
				
				// Set the image
				return texture.GetPreview();
			}
		}

		// This browses for a texture
		protected override string BrowseImage(string imagename)
		{
			string result;

			// Browse for texture
			result = TextureBrowserForm.Browse(this.ParentForm, imagename, false);
			if(result != null) return result; else return imagename;
		}
	}
}
