
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

		// Rendering memory
		private Texture tex;
		private int width, height;
		private int pwidth, pheight;
		private Plotter plotter;

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
		public Renderer2D(D3DGraphics graphics) : base(graphics)
		{
			// Initialize
			
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
				if(tex != null) tex.Dispose();
				tex = null;
				
				// Done
				base.Dispose();
			}
		}

		#endregion

		#region ================== Control
		
		// This draws the image on screen
		public void Present()
		{
			FlatVertex[] verts = new FlatVertex[4];

			// Start drawing
			if(graphics.StartRendering())
			{
				// Left top
				verts[0].x = -0.5f;
				verts[0].y = -0.5f;
				verts[0].w = 1f;
				verts[0].u = 0f;
				verts[0].v = 0f;

				// Right top
				verts[1].x = pwidth - 0.5f;
				verts[1].y = -0.5f;
				verts[1].w = 1f;
				verts[1].u = 1f;
				verts[1].v = 0f;

				// Left bottom
				verts[2].x = -0.5f;
				verts[2].y = pheight - 0.5f;
				verts[2].w = 1f;
				verts[2].u = 0f;
				verts[2].v = 1f;

				// Right bottom
				verts[3].x = pwidth - 0.5f;
				verts[3].y = pheight - 0.5f;
				verts[3].w = 1f;
				verts[3].u = 1f;
				verts[3].v = 1f;
				
				// Set renderstates AND shader settings
				graphics.Device.SetTexture(0, tex);
				graphics.Device.SetRenderState(RenderState.CullMode, Cull.None);
				graphics.Shaders.Base2D.Texture1 = tex;
				graphics.Shaders.Base2D.SetSettings(1f / pwidth, 1f / pheight, 0.3f);
				
				// Draw
				graphics.Shaders.Base2D.Begin();
				graphics.Shaders.Base2D.BeginPass(0);
				try { graphics.Device.DrawUserPrimitives<FlatVertex>(PrimitiveType.TriangleStrip, 0, 2, verts); }
				catch(Exception) { }
				graphics.Shaders.Base2D.EndPass();
				graphics.Shaders.Base2D.End();

				// Done
				graphics.FinishRendering();
			}
		}

		// This is called before a device is reset
		// (when resized or display adapter was changed)
		public override void UnloadResource()
		{
			// Trash old texture
			if(tex != null) tex.Dispose();
			tex = null;
		}
		
		// This is called resets when the device is reset
		// (when resized or display adapter was changed)
		public override void ReloadResource()
		{
			// Re-create texture
			CreateTexture();
		}

		// This resets the graphics
		public override void Reset()
		{
			UnloadResource();
			ReloadResource();
		}

		// Allocates new image memory to render on
		public void CreateTexture()
		{
			SurfaceDescription sd;
			
			// Get new width and height
			width = graphics.RenderTarget.ClientSize.Width;
			height = graphics.RenderTarget.ClientSize.Height;
			
			// Trash old texture
			if(tex != null) tex.Dispose();
			tex = null;
			
			// Create new texture
			tex = new Texture(graphics.Device, width, height, 1, Usage.Dynamic, Format.A8R8G8B8, Pool.Default);

			// Get the real surface size
			sd = tex.GetLevelDescription(0);
			pwidth = sd.Width;
			pheight = sd.Height;
		}

		// This begins a drawing session
		public unsafe bool StartRendering(bool cleardisplay)
		{
			LockedRect rect;
			
			// Do we have a texture?
			if(tex != null)
			{
				// Lock memory
				rect = tex.LockRectangle(0, LockFlags.Discard);

				// Create plotter
				plotter = new Plotter((PixelColor*)rect.Data.DataPointer.ToPointer(), rect.Pitch / sizeof(PixelColor), pheight, width, height);
				if(cleardisplay) plotter.Clear();
				
				// Ready for rendering
				return true;
			}
			else
			{
				// Can't render!
				return false;
			}
		}
		
		// This ends a drawing session
		public void FinishRendering()
		{
			// Unlock memory
			tex.UnlockRectangle(0);

			// Present new image
			Present();
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
			float ox = -offsetx + (width * 0.5f) / scale;
			float oy = -offsety - (height * 0.5f) / scale;
			return mousepos.GetTransformed(ox, oy, scale, -scale);
		}

		#endregion

		#region ================== Map Rendering

		// This renders a set of Linedefs
		public void RenderLinedefs(MapSet map, ICollection<Linedef> linedefs)
		{
			float ox = -offsetx + (width * 0.5f) / scale;
			float oy = -offsety - (height * 0.5f) / scale;
			PixelColor c = PixelColor.FromColor(Color.White);
			Vector2D v1, v2;
			
			// Go for all linedefs
			foreach(Linedef l in linedefs)
			{
				// Transform vertex coordinates
				v1 = l.Start.Position.GetTransformed(ox, oy, scale, -scale);
				v2 = l.End.Position.GetTransformed(ox, oy, scale, -scale);
				
				// Draw line
				plotter.DrawLineSolid((int)v1.x, (int)v1.y, (int)v2.x, (int)v2.y, c);
			}
		}

		// This renders a set of Linedefs
		public void RenderVertices(MapSet map, ICollection<Vertex> vertices)
		{
			Vector2D nv;
			float ox = -offsetx + (width * 0.5f) / scale;
			float oy = -offsety - (height * 0.5f) / scale;
			PixelColor c = PixelColor.FromColor(Color.DeepSkyBlue);
			int x, y;
			
			// Go for all vertices
			foreach(Vertex v in vertices)
			{
				// Transform vertex coordinates
				nv = v.Position.GetTransformed(ox, oy, scale, -scale);
				x = (int)nv.x;
				y = (int)nv.y;
				
				// Draw pixel here
				plotter.DrawVertexSolid(x, y, 2, c);
			}
		}

		#endregion
	}
}
