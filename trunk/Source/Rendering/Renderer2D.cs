
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
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using System.Drawing;
using System.ComponentModel;
using CodeImp.DoomBuilder.Map;
using SlimDX.Direct3D;
using SlimDX.Direct3D9;
using SlimDX;
using CodeImp.DoomBuilder.Geometry;
using System.Drawing.Imaging;

#endregion

namespace CodeImp.DoomBuilder.Rendering
{
	internal unsafe class Renderer2D : IDisposable
	{
		#region ================== Constants

		#endregion

		#region ================== Variables

		// Owner
		private D3DGraphics graphics;

		// Rendering memory
		private Bitmap image;
		private BitmapData pixeldata;
		private PixelColor* pixels;
		private int width;
		private int height;
		
		// View settings (world coordinates)
		private float scale;
		private float offsetx;
		private float offsety;
		
		// Disposing
		private bool isdisposed = false;

		#endregion

		#region ================== Properties

		public float OffsetX { get { return offsetx; } }
		public float OffsetY { get { return offsety; } }
		public float Scale { get { return scale; } }
		public bool IsDisposed { get { return isdisposed; } }

		#endregion

		#region ================== Constructor / Disposer
		
		// Constructor
		public Renderer2D(D3DGraphics graphics)
		{
			// Initialize
			this.graphics = graphics;

			// Create image memory
			CreateMemory();
			
			// We have no destructor
			GC.SuppressFinalize(this);
		}

		// Diposer
		public void Dispose()
		{
			// Not already disposed?
			if(!isdisposed)
			{
				// Clean up
				graphics.RenderTarget.BackgroundImage = null;
				if(image != null) image.Dispose();
				pixels = null;
				
				// Done
				isdisposed = true;
			}
		}

		#endregion

		#region ================== Control
		
		// This is called resets when the device is reset
		// (when resized or display adapter was changed)
		public void Reset()
		{
			// Re-create image memory
			CreateMemory();
		}

		// Allocates new image memory to render on
		public void CreateMemory()
		{
			// Get new width and height
			width = graphics.RenderTarget.ClientSize.Width;
			height = graphics.RenderTarget.ClientSize.Height;
			
			// Trash old image
			graphics.RenderTarget.BackgroundImage = null;
			if(image != null) image.Dispose();
			
			// Allocate memory
			image = new Bitmap(width, height, PixelFormat.Format32bppArgb);
			graphics.RenderTarget.BackgroundImage = image;
		}

		// This begins a drawing session
		public unsafe bool StartRendering()
		{
			// Lock memory
			pixeldata = image.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
			pixels = (PixelColor*)pixeldata.Scan0.ToPointer();
			
			// Erase image
			General.ZeroMemory(pixeldata.Scan0, width * height * 4);
			
			// Ready for rendering
			return true;
		}
		
		// This ends a drawing session
		public void FinishRendering()
		{
			// Unlock memory
			image.UnlockBits(pixeldata);
			
			// Refresh
			graphics.RenderTarget.Invalidate();
		}
		
		// This changes view position
		public void PositionView(float x, float y)
		{
			// Change position in world coordinates
			offsetx = x;
			offsety = y;
		}
		
		// This changes zoom
		public void ScaleView(float scale)
		{
			// Change zoom scale
			this.scale = scale;
			
			// Recalculate linedefs (normal lengths must be adjusted)
			foreach(Linedef l in General.Map.Data.Linedefs) l.NeedUpdate();
		}

		// This unprojects mouse coordinates into map coordinates
		public Vector2D GetMapCoordinates(Vector2D mousepos)
		{
			Vector3 mp, res;

			// FIXME!
			
			// Get mouse position in Vector3
			//mp = new Vector3(mousepos.x, mousepos.y, 1f);
			
			// Unproject
			//res = mp.Unproject(graphics.Viewport, matproj, matview, matworld);

			// Return result
			//return new Vector2D(res.X, res.Y);
			return new Vector2D();
		}

		#endregion

		#region ================== Pixel Rendering

		// This draws a pixel normally
		private void DrawPixelSolid(int x, int y, PixelColor c)
		{
			// Draw pixel when within range
			if((x >= 0) && (x < width) && (y >= 0) && (y < height))
				pixels[y * width + x] = c;
		}

		// This draws a pixel alpha blended
		private void DrawPixelAlpha(int x, int y, PixelColor c)
		{
			// Draw only when within range
			if((x >= 0) && (x < width) && (y >= 0) && (y < height))
			{
				// Get the target pixel
				PixelColor* p = pixels + (y * width + x);
				
				// Not drawn on target yet?
				if(*(int*)p == 0)
				{
					// Simply apply color to pixel
					*p = c;
				}
				else
				{
					// Blend with pixel
					if((int)p->a + (int)c.a > 255) p->a = 255; else p->a += c.a;
					p->r = (byte)((float)p->r * (1f - c.a) + (float)c.r * c.a);
					p->g = (byte)((float)p->g * (1f - c.a) + (float)c.g * c.a);
					p->b = (byte)((float)p->b * (1f - c.a) + (float)c.b * c.a);
				}
			}
		}

		#endregion

		// This draws a line normally
		private void DrawLineSolid(int x0, int y0, int x1, int y1, PixelColor c)
		{
			// TODO: See http://en.wikipedia.org/wiki/Xiaolin_Wu's_line_algorithm
		}
		
		#region ================== Map Rendering

		// This renders a set of Linedefs
		public unsafe void RenderLinedefs(MapSet map, ICollection<Linedef> linedefs)
		{
			// TODO
		}

		// This renders a set of Linedefs
		public void RenderVertices(MapSet map, ICollection<Vertex> vertices)
		{
			Vector2D nv;
			Vector2D voffset = new Vector2D(-offsetx + (width * 0.5f) / scale, -offsety - (height * 0.5f) / scale);
			Vector2D vscale = new Vector2D(scale, -scale);
			PixelColor c = PixelColor.FromInt(-1);
			int x, y;
			
			// Go for all vertices
			foreach(Vertex v in vertices)
			{
				// Transform vertex coordinates
				nv = v.Position;
				nv.Transform(voffset, vscale);
				x = (int)nv.x;
				y = (int)nv.y;
				
				// Draw pixel here
				DrawPixelSolid(x, y, c);
				DrawPixelSolid(x + 1, y, c);
				DrawPixelSolid(x, y + 1, c);
				DrawPixelSolid(x - 1, y, c);
				DrawPixelSolid(x, y - 1, c);
				DrawPixelSolid(x + 1, y - 1, c);
				DrawPixelSolid(x + 1, y + 1, c);
				DrawPixelSolid(x - 1, y - 1, c);
				DrawPixelSolid(x - 1, y + 1, c);
			}
		}

		#endregion
	}
}
