
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

#endregion

namespace CodeImp.DoomBuilder.Rendering
{
	internal class Renderer2D : IDisposable
	{
		#region ================== Constants

		#endregion

		#region ================== Variables

		// Owner
		private Graphics graphics;

		// View settings (world coordinates)
		private float scale;
		private float offsetx;
		private float offsety;
		
		// Matrices
		private Matrix matproj, matview, matworld;
		
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
		public Renderer2D(Graphics graphics)
		{
			// Initialize
			this.graphics = graphics;

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
				
				// Done
				isdisposed = true;
			}
		}

		#endregion

		#region ================== Control
		
		// This rebuilds matrices according to view settings
		private void SetupMatrices()
		{
			float width, height, left, top, right, bottom;

			// Build projection matrix
			width = (float)graphics.RenderTarget.ClientSize.Width / scale;
			height = (float)graphics.RenderTarget.ClientSize.Height / scale;
			left = offsetx - width * 0.5f;
			top = offsety - height * 0.5f;
			right = offsetx + width * 0.5f;
			bottom = offsety + height * 0.5f;
			matproj = Matrix.OrthoOffCenterLH(left, right, top, bottom, -1f, 1f);

			// World and view are fixed
			matview = Matrix.Identity;
			matworld = Matrix.Identity;
		}
		
		// This begins a drawing session
		public bool StartRendering()
		{
			// Can we render?
			if(graphics.StartRendering())
			{
				// Apply matrices
				graphics.Device.SetTransform(TransformState.Projection, matproj);
				graphics.Device.SetTransform(TransformState.View, matview);
				graphics.Device.SetTransform(TransformState.World, matworld);

				// Success
				return true;
			}
			else
			{
				// Cannot render
				return false;
			}
		}
		
		// This ends a drawing session
		public void FinishRendering()
		{
			// Finish and present our drawing
			graphics.FinishRendering();
		}
		
		// This changes view position
		public void PositionView(float x, float y)
		{
			// Change position in world coordinates
			offsetx = x;
			offsety = y;

			// Setup new matrices
			SetupMatrices();
		}
		
		// This changes zoom
		public void ScaleView(float scale)
		{
			// Change zoom scale
			this.scale = scale;

			// Setup new matrices
			SetupMatrices();
			
			// Recalculate linedefs (normal lengths must be adjusted)
			foreach(Linedef l in General.Map.Data.Linedefs) l.NeedUpdate();
		}

		// This unprojects mouse coordinates into map coordinates
		public Vector2D GetMapCoordinates(Vector2D mousepos)
		{
			Vector3 mp, res;

			// Get mouse position in Vector3
			mp = new Vector3(mousepos.x, mousepos.y, 1f);
			
			// Unproject
			res = mp.Unproject(graphics.Viewport, matproj, matview, matworld);

			// Return result
			return new Vector2D(res.X, res.Y);
		}

		#endregion
		
		#region ================== Map Rendering

		// This renders a set of Linedefs
		public void RenderLinedefs(MapSet map, ICollection<Linedef> linedefs)
		{
			// Any linedefs?
			if(linedefs.Count > 0)
			{
				graphics.Device.SetRenderState(RenderState.TextureFactor, -1);
				graphics.Device.SetStreamSource(0, map.LinedefsBuffer.VertexBuffer, 0, PTVertex.Stride);

				foreach(Linedef l in linedefs)
				{
					graphics.Device.DrawPrimitives(PrimitiveType.LineList, l.BufferIndex * Linedef.BUFFERVERTICES, Linedef.RENDERPRIMITIVES);
				}
			}
		}

		// This renders a set of Linedefs
		public void RenderVertices(MapSet map, ICollection<Vertex> vertices)
		{
		}

		#endregion
	}
}
