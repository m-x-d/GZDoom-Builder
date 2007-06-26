
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
using Microsoft.DirectX.Direct3D;
using System.ComponentModel;
using CodeImp.DoomBuilder.Map;
using Microsoft.DirectX;

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

		#region ================== Methods
		
		// This rebuilds matrices according to view settings
		private void SetupMatrices()
		{
			Matrix proj;
			float width, height, left, top, right, bottom;

			// Build projection matrix
			width = (float)graphics.RenderTarget.ClientSize.Width / scale;
			height = (float)graphics.RenderTarget.ClientSize.Height / scale;
			left = offsetx - width * 0.5f;
			top = offsety - height * 0.5f;
			right = offsetx + width * 0.5f;
			bottom = offsety + height * 0.5f;
			proj = Matrix.OrthoOffCenterLH(left, right, top, bottom, -1f, 1f);

			// Apply matrices
			graphics.Device.Transform.Projection = proj;
			graphics.Device.Transform.View = Matrix.Identity;
			graphics.Device.Transform.World = Matrix.Identity;
		}
		
		// This begins a drawing session
		public bool StartRendering()
		{
			// Can we render?
			if(graphics.StartRendering())
			{
				// Setup matrices
				SetupMatrices();

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
		}
		
		// This changes zoom
		public void ScaleView(float scale)
		{
			// Change zoom scale
			this.scale = scale;
		}

		// This renders a set of Linedefs
		public void RenderLinedefs(ICollection<Linedef> linedefs)
		{
			PTVertex[] verts = new PTVertex[linedefs.Count * 4];
			int i = 0;

			// Any linedefs?
			if(linedefs.Count > 0)
			{
				graphics.Device.RenderState.TextureFactor = -1;

				// Go for all linedefs
				foreach(Linedef l in linedefs)
				{
					// Make vertices
					verts[i++] = l.LineVertices[0];
					verts[i++] = l.LineVertices[1];
					verts[i++] = l.LineVertices[2];
					verts[i++] = l.LineVertices[3];
				}

				// Draw lines
				graphics.Device.DrawUserPrimitives(PrimitiveType.LineList, linedefs.Count * 2, verts);
			}
		}

		// This renders a set of Linedefs
		public void RenderVertices(MapSet map, ICollection<Vertex> vertices)
		{
		}

		#endregion
	}
}
