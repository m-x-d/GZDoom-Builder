
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
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using CodeImp.DoomBuilder.Rendering;
using CodeImp.DoomBuilder.IO;
using System.IO;

#endregion

namespace CodeImp.DoomBuilder.Data
{
	internal class PreviewManager : IDisposable
	{
		#region ================== Constants

		// Image format
		private const PixelFormat IMAGE_FORMAT = PixelFormat.Format16bppArgb1555;
		
		// Dimensions of a single preview image
		private const int IMAGE_WIDTH = 64;
		private const int IMAGE_HEIGHT = 64;

		// How many previews on a single atlas?
		private const int PREVIEWS_X = 8;
		private const int PREVIEWS_Y = 8;

		#endregion

		#region ================== Variables

		// Atlases
		private List<Bitmap> atlases;
		
		// Next preview index
		private int nextpreviewindex;
		
		// Disposing
		private bool isdisposed = false;

		#endregion

		#region ================== Properties

		// Disposing
		public bool IsDisposed { get { return isdisposed; } }

		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		public PreviewManager()
		{
			// Initialize
			atlases = new List<Bitmap>();
			nextpreviewindex = 0;
			
			// We have no destructor
			GC.SuppressFinalize(this);
		}

		// Disposer
		public void Dispose()
		{
			// Not already disposed?
			if(!isdisposed)
			{
				// Clean up
				foreach(Bitmap b in atlases) b.Dispose();
				atlases = null;
				
				// Done
				isdisposed = true;
			}
		}

		#endregion

		#region ================== Calculations
		
		// Returns atlas index for the given preview index
		private int GetAtlasIndex(int previewindex)
		{
			return previewindex / (PREVIEWS_X * PREVIEWS_Y);
		}

		// Returns local X index for the given preview index
		private int GetLocalXIndex(int previewindex)
		{
			int localindex = previewindex - GetAtlasIndex(previewindex) * (PREVIEWS_X * PREVIEWS_Y);
			return localindex % PREVIEWS_Y;
		}

		// Returns local Y index for the given preview index
		private int GetLocalYIndex(int previewindex)
		{
			int localindex = previewindex - GetAtlasIndex(previewindex) * (PREVIEWS_X * PREVIEWS_Y);
			return localindex / PREVIEWS_X;
		}

		#endregion
		
		#region ================== Loading

		// This makes a new atlas
		private void MakeNewAtlas()
		{
			Bitmap b = new Bitmap(IMAGE_WIDTH * PREVIEWS_X, IMAGE_HEIGHT * PREVIEWS_Y, IMAGE_FORMAT);
			atlases.Add(b);
		}
		
		// This makes a preview for the given image and updates the image settings
		private void MakeImagePreview(Image img)
		{
			// Do we have to make a new atlas?
			if(GetAtlasIndex(nextpreviewindex) > (atlases.Count - 1)) MakeNewAtlas();
			

		}

		#endregion
	}
}
