
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
	internal unsafe class Plotter
	{
		#region ================== Constants

		#endregion

		#region ================== Variables

		// Memory
		private PixelColor* pixels;
		private int width;
		private int height;
		private int visiblewidth;
		private int visibleheight;

		#endregion

		#region ================== Properties

		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		public Plotter(PixelColor* pixels, int width, int height, int visiblewidth, int visibleheight)
		{
			// Initialize
			this.pixels = pixels;
			this.width = width;
			this.height = height;
			this.visiblewidth = width;
			this.visibleheight = height;
			
			// We have no destructor
			GC.SuppressFinalize(this);
		}

		#endregion

		#region ================== Pixel Rendering

		// This clears all pixels black
		public void Clear()
		{
			// Clear memory
			General.ZeroMemory(new IntPtr(pixels), width * height * sizeof(PixelColor));
		}
		
		// This draws a pixel normally
		public void DrawPixelSolid(int x, int y, PixelColor c)
		{
			// Draw pixel when within range
			if((x >= 0) && (x < visiblewidth) && (y >= 0) && (y < visibleheight))
				pixels[y * width + x] = c;
		}

		// This draws a pixel normally
		public void DrawVertexSolid(int x, int y, int size, PixelColor c)
		{
			int xp, yp;
			
			// Do unchecked?
			if((x - size >= 0) && (x + size < visiblewidth) && (y - size >= 0) && (y + size < visibleheight))
			{
				for(yp = y - size; yp <= y + size; yp++)
					for(xp = x - size; xp <= x + size; xp++)
						pixels[yp * width + xp] = c;
			}
			else
			{
				for(yp = y - size; yp <= y + size; yp++)
					for(xp = x - size; xp <= x + size; xp++)
						if((xp >= 0) && (xp < visiblewidth) && (yp >= 0) && (yp < visibleheight))
							pixels[yp * width + xp] = c;
			}
		}

		// This draws a pixel alpha blended
		public void DrawPixelAlpha(int x, int y, PixelColor c)
		{
			float a;

			// Draw only when within range
			if((x >= 0) && (x < visiblewidth) && (y >= 0) && (y < visibleheight))
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
		public void DrawLineAlpha(int x1, int y1, int x2, int y2, PixelColor c)
		{
			int i;

			// Check if the line is outside the screen for sure.
			// This is quickly done by checking in which area both points are. When this
			// is above, below, right or left of the screen, then skip drawing the line.
			if(((x1 < 0) && (x2 < 0)) ||
			   ((x1 > visiblewidth) && (x2 > visiblewidth)) ||
			   ((y1 < 0) && (y2 < 0)) ||
			   ((y1 > visibleheight) && (y2 > visibleheight))) return;

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
		public void DrawLineSolid(int x1, int y1, int x2, int y2, PixelColor c)
		{
			int i;

			// Check if the line is outside the screen for sure.
			// This is quickly done by checking in which area both points are. When this
			// is above, below, right or left of the screen, then skip drawing the line.
			if(((x1 < 0) && (x2 < 0)) ||
			   ((x1 > visiblewidth) && (x2 > visiblewidth)) ||
			   ((y1 < 0) && (y2 < 0)) ||
			   ((y1 > visibleheight) && (y2 > visibleheight))) return;

			// When the line is completely inside screen,
			// then do an unchecked draw, because all of its pixels are
			// guaranteed to be within the memory range
			if((x1 >= 0) && (x2 >= 0) && (x1 < visiblewidth) && (x2 < visiblewidth) &&
			   (y1 >= 0) && (y2 >= 0) && (y1 < visibleheight) && (y2 < visibleheight))
			{
				// Do an unchecked draw
				DrawLineSolidUnchecked(x1, y1, x2, y2, c);
				return;
			}
			
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

		// This draws a line normally
		// See: http://en.wikipedia.org/wiki/Bresenham%27s_line_algorithm
		private void DrawLineSolidUnchecked(int x1, int y1, int x2, int y2, PixelColor c)
		{
			int i;

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
			pixels[py * width + px] = c;

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
					pixels[py * width + px] = c;
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
					pixels[py * width + px] = c;
				}
			}
		}

		#endregion
	}
}
