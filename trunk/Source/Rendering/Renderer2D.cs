
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
	internal unsafe class Renderer2D : Renderer
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

		#endregion

		#region ================== Properties

		public float OffsetX { get { return offsetx; } }
		public float OffsetY { get { return offsety; } }
		public float Scale { get { return scale; } }

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
		public override void Dispose()
		{
			// Not already disposed?
			if(!isdisposed)
			{
				// Clean up
				graphics.RenderTarget.BackgroundImage = null;
				if(image != null) image.Dispose();
				pixels = null;
				
				// Done
				base.Dispose();
			}
		}

		#endregion

		#region ================== Control
		
		// This is called resets when the device is reset
		// (when resized or display adapter was changed)
		public override void Reset()
		{
			// Trash old image
			graphics.RenderTarget.BackgroundImage = null;
			if(image != null) image.Dispose();

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

			// Show zoom on main window
			General.MainWindow.UpdateZoom(scale);
			
			// Recalculate linedefs (normal lengths must be adjusted)
			foreach(Linedef l in General.Map.Map.Linedefs) l.NeedUpdate();
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
			float a;

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
					a = (float)c.a * 0.003921568627450980392156862745098f;
					if((int)p->a + (int)c.a > 255) p->a = 255; else p->a += c.a;
					p->r = (byte)((float)p->r * (1f - a) + (float)c.r * a);
					p->g = (byte)((float)p->g * (1f - a) + (float)c.g * a);
					p->b = (byte)((float)p->b * (1f - a) + (float)c.b * a);
				}
			}
		}

		// This draws a line alpha blended
		// See: http://en.wikipedia.org/wiki/Bresenham%27s_line_algorithm
		private void DrawLineAlpha(int x1, int y1, int x2, int y2, PixelColor c)
		{
			int i;

			// Check if the line is outside the screen for sure.
			// This is quickly done by checking in which area both points are. When this
			// is above, below, right or left of the screen, then skip drawing the line.
			if(((x1 < 0) && (x2 < 0)) ||
			   ((x1 > width) && (x2 > width)) ||
			   ((y1 < 0) && (y2 < 0)) ||
			   ((y1 > height) && (y2 > height))) return;
			
			// Distance of the line
			int dx = x2 - x1;
			int dy = y2 - y1;

			// Positive (absolute) distance
			int dxabs = Math.Abs(dx);
			int dyabs = Math.Abs(dy);

			// Half distance
			int x = dyabs >> 1;
			int y = dxabs >> 1;

			// Direction
			int sdx = Math.Sign(dx);
			int sdy = Math.Sign(dy);

			// Start position
			int px = x1;
			int py = y1;

			// Draw first pixel
			DrawPixelAlpha(px, py, c);
			
			// Check if the line is more horizontal than vertical
			if(dxabs >= dyabs)
			{
				for(i = 0; i < dxabs; i++)
				{
					y += dyabs;
					if(y >= dxabs)
					{
						y -= dxabs;
						py += sdy;
					}
					px += sdx;

					// Draw pixel
					DrawPixelAlpha(px, py, c);
				}
			}
			// Else the line is more vertical than horizontal
			else
			{
				for(i = 0; i < dyabs; i++)
				{
					x += dxabs;
					if(x >= dyabs)
					{
						x -= dyabs;
						px += sdx;
					}
					py += sdy;

					// Draw pixel
					DrawPixelAlpha(px, py, c);
				}
			}
		}

		// This draws a line normally
		// See: http://en.wikipedia.org/wiki/Bresenham%27s_line_algorithm
		private void DrawLineSolid(int x1, int y1, int x2, int y2, PixelColor c)
		{
			int i;

			// Check if the line is outside the screen for sure.
			// This is quickly done by checking in which area both points are. When this
			// is above, below, right or left of the screen, then skip drawing the line.
			if(((x1 < 0) && (x2 < 0)) ||
			   ((x1 > width) && (x2 > width)) ||
			   ((y1 < 0) && (y2 < 0)) ||
			   ((y1 > height) && (y2 > height))) return;

			// Distance of the line
			int dx = x2 - x1;
			int dy = y2 - y1;

			// Positive (absolute) distance
			int dxabs = Math.Abs(dx);
			int dyabs = Math.Abs(dy);

			// Half distance
			int x = dyabs >> 1;
			int y = dxabs >> 1;

			// Direction
			int sdx = Math.Sign(dx);
			int sdy = Math.Sign(dy);

			// Start position
			int px = x1;
			int py = y1;

			// Draw first pixel
			DrawPixelSolid(px, py, c);

			// Check if the line is more horizontal than vertical
			if(dxabs >= dyabs)
			{
				for(i = 0; i < dxabs; i++)
				{
					y += dyabs;
					if(y >= dxabs)
					{
						y -= dxabs;
						py += sdy;
					}
					px += sdx;

					// Draw pixel
					DrawPixelSolid(px, py, c);
				}
			}
			// Else the line is more vertical than horizontal
			else
			{
				for(i = 0; i < dyabs; i++)
				{
					x += dxabs;
					if(x >= dyabs)
					{
						x -= dyabs;
						px += sdx;
					}
					py += sdy;

					// Draw pixel
					DrawPixelSolid(px, py, c);
				}
			}
		}

		#endregion

		#region ================== Map Rendering

		// This renders a set of Linedefs
		public unsafe void RenderLinedefs(MapSet map, ICollection<Linedef> linedefs)
		{
			Vector2D voffset = new Vector2D(-offsetx + (width * 0.5f) / scale, -offsety - (height * 0.5f) / scale);
			Vector2D vscale = new Vector2D(scale, -scale);
			PixelColor c = PixelColor.FromColor(Color.SkyBlue);
			Vector2D v1, v2;

			// Go for all linedefs
			foreach(Linedef l in linedefs)
			{
				// Transform vertex coordinates
				v1 = l.Start.Position.GetTransformed(voffset, vscale);
				v2 = l.End.Position.GetTransformed(voffset, vscale);

				// Draw line
				DrawLineSolid((int)v1.x, (int)v1.y, (int)v2.x, (int)v2.y, c);
			}
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
				nv = v.Position.GetTransformed(voffset, vscale);
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
