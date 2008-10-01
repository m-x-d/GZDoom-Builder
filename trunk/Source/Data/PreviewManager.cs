
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
using System.Drawing.Drawing2D;

#endregion

namespace CodeImp.DoomBuilder.Data
{
	public class PreviewManager
	{
		#region ================== Constants

		// Image format
		private const PixelFormat IMAGE_FORMAT = PixelFormat.Format32bppArgb;
		
		// Dimensions of a single preview image
		internal const int IMAGE_WIDTH = 64;
		internal const int IMAGE_HEIGHT = 64;

		// How many previews on a single atlas?
		private const int PREVIEWS_X = 1;
		private const int PREVIEWS_Y = 1;

		#endregion

		#region ================== Variables

		// Atlases
		private List<Bitmap> atlases;
		
		// Next preview index
		private int nextpreviewindex;
		
		// Processing
		private Queue<ImageData> imageque;
		
		// Disposing
		private bool isdisposed = false;

		#endregion

		#region ================== Properties

		// Constants
		public int ImageWidth { get { return IMAGE_WIDTH; } }
		public int ImageHeight { get { return IMAGE_HEIGHT; } }
		
		// Disposing
		internal bool IsDisposed { get { return isdisposed; } }
		
		// Loading
		internal bool IsLoading
		{
			get
			{
				return (imageque.Count > 0);
			}
		}
		
		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		internal PreviewManager()
		{
			// Initialize
			atlases = new List<Bitmap>();
			imageque = new Queue<ImageData>();
			nextpreviewindex = 0;
			
			// We have no destructor
			GC.SuppressFinalize(this);
		}

		// Disposer
		internal void Dispose()
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
		
		#region ================== Private Methods
		
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
		
		// This makes a new atlas
		private void MakeNewAtlas()
		{
			lock(atlases)
			{
				Bitmap b = new Bitmap(IMAGE_WIDTH * PREVIEWS_X, IMAGE_HEIGHT * PREVIEWS_Y, IMAGE_FORMAT);
				Graphics g = Graphics.FromImage(b);
				g.Clear(Color.Transparent);
				g.Dispose();
				atlases.Add(b);
			}
		}
		
		// This makes a preview for the given image and updates the image settings
		private void MakeImagePreview(ImageData img)
		{
			Bitmap atlas;
			
			// Numbers
			int atlasindex = GetAtlasIndex(nextpreviewindex);
			int localx = GetLocalXIndex(nextpreviewindex);
			int localy = GetLocalYIndex(nextpreviewindex);

			lock(atlases)
			{
				// Do we have to make a new atlas?
				if(atlasindex > (atlases.Count - 1)) MakeNewAtlas();
				
				// Get the atlas we need
				atlas = atlases[atlasindex];
			}
			
			lock(img)
			{
				// Load image if needed
				if(!img.IsImageLoaded) img.LoadImage();

				lock(atlas)
				{
					// Draw image onto atlas
					Graphics g = Graphics.FromImage(atlas);
					g.PageUnit = GraphicsUnit.Pixel;
					g.CompositingQuality = CompositingQuality.HighQuality;
					g.InterpolationMode = InterpolationMode.HighQualityBicubic;
					g.SmoothingMode = SmoothingMode.HighQuality;
					g.PixelOffsetMode = PixelOffsetMode.None;
					Rectangle atlasrect = new Rectangle(localx * IMAGE_WIDTH, localy * IMAGE_HEIGHT, IMAGE_WIDTH, IMAGE_HEIGHT);
					RectangleF imgrect = General.MakeZoomedRect(new Size(img.Width, img.Height), atlasrect);
					g.DrawImage(img.Bitmap, imgrect);
					g.Dispose();
				}
				
				// Unload image if no longer needed
				if(!img.IsReferenced) img.UnloadImage();
				
				// Set numbers
				img.PreviewIndex = nextpreviewindex;
				img.PreviewState = ImageLoadState.Ready;
				nextpreviewindex++;
			}
		}

		#endregion
		
		#region ================== Public Methods

		// This draws a preview centered in a target
		internal void DrawPreview(int previewindex, Graphics target, Point targetpos)
		{
			Bitmap atlas;

			// Get the atlas we need
			lock(atlases) { atlas = atlases[GetAtlasIndex(previewindex)]; }

			// Draw from atlas to target
			lock(atlas)
			{
				if((PREVIEWS_X == 1) && (PREVIEWS_Y == 1))
				{
					target.DrawImageUnscaled(atlas, targetpos.X, targetpos.Y);
				}
				else
				{
					RectangleF trect = new RectangleF((float)targetpos.X, (float)targetpos.Y,
													  (float)IMAGE_WIDTH, (float)IMAGE_HEIGHT);
					RectangleF srect = new RectangleF((float)GetLocalXIndex(previewindex) * IMAGE_WIDTH,
													  (float)GetLocalYIndex(previewindex) * IMAGE_HEIGHT,
													  (float)IMAGE_WIDTH, (float)IMAGE_HEIGHT);
					target.DrawImage(atlas, trect, srect, GraphicsUnit.Pixel);
				}
			}
		}
		
		// This draws a preview centered in a target
		internal void DrawPreviewCentered(int previewindex, Graphics target, Rectangle targetview)
		{
			Bitmap atlas;

			// Get the atlas we need
			lock(atlases) { atlas = atlases[GetAtlasIndex(previewindex)]; }

			// Draw from atlas to target
			lock(atlas)
			{
				RectangleF trect = General.MakeZoomedRect(new Size(64, 64), targetview);
				RectangleF srect = new RectangleF((float)GetLocalXIndex(previewindex) * IMAGE_WIDTH,
												  (float)GetLocalYIndex(previewindex) * IMAGE_HEIGHT,
												  (float)IMAGE_WIDTH, (float)IMAGE_HEIGHT);
				target.DrawImage(atlas, trect, srect, GraphicsUnit.Pixel);
			}
		}

		// Background loading
		// Return true when we have more work to do, so that the
		// thread will not wait too long before calling again
		internal bool BackgroundLoad()
		{
			// Get next item
			ImageData image = null;
			lock(imageque)
			{
				// Fetch next image to process
				if(imageque.Count > 0) image = imageque.Dequeue();
			}

			// Any image to process?
			if(image != null)
			{
				// Make image preview?
				if(!image.IsPreviewLoaded) MakeImagePreview(image);
			}

			return (image != null);
		}
		
		// This adds an image for preview creation
		internal void AddImage(ImageData image)
		{
			lock(imageque)
			{
				// Add to list
				image.PreviewState = ImageLoadState.Loading;
				imageque.Enqueue(image);
			}
		}


		#if DEBUG
		internal void DumpAtlases()
		{
			lock(atlases)
			{
				int index = 0;
				foreach(Bitmap a in atlases)
				{
					lock(a)
					{
						string file = Path.Combine(General.AppPath, "atlas" + index++ + ".png");
						a.Save(file, ImageFormat.Png);
					}
				}
			}
		}
		#endif
		
		#endregion
	}
}
