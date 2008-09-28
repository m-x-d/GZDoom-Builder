
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
using SlimDX;
using CodeImp.DoomBuilder.Geometry;
using SlimDX.Direct3D9;
using CodeImp.DoomBuilder.Data;

#endregion

namespace CodeImp.DoomBuilder.Rendering
{
	internal sealed class Renderer3D : Renderer, IRenderer3D
	{
		#region ================== Constants

		private const float PROJ_NEAR_PLANE = 1f;
		private const float PROJ_FAR_PLANE = 2000f;

		#endregion

		#region ================== Variables

		// Matrices
		private Matrix projection;
		private Matrix view;
		private Matrix billboard;
		private Matrix viewproj;
		
		#endregion

		#region ================== Properties

		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		internal Renderer3D(D3DDevice graphics) : base(graphics)
		{
			// Initialize
			CreateProjection();
			
			// We have no destructor
			GC.SuppressFinalize(this);
		}

		// Disposer
		internal override void Dispose()
		{
			// Not already disposed?
			if(!isdisposed)
			{
				// Clean up

				// Done
				base.Dispose();
			}
		}

		#endregion

		#region ================== Methods

		// This is called before a device is reset
		// (when resized or display adapter was changed)
		public override void UnloadResource()
		{
		}
		
		// This is called resets when the device is reset
		// (when resized or display adapter was changed)
		public override void ReloadResource()
		{
		}

		#endregion

		#region ================== General

		// This creates the projection
		internal void CreateProjection()
		{
			// Calculate FOV
			float fov = Angle2D.DegToRad((float)General.Settings.VisualFOV);
			
			// Calculate aspect
			float aspect = (float)General.Map.Graphics.RenderTarget.ClientSize.Width /
						   (float)General.Map.Graphics.RenderTarget.ClientSize.Height;
			
			// Make the projection matrix
			projection = Matrix.PerspectiveFovRH(fov, aspect, PROJ_NEAR_PLANE, PROJ_FAR_PLANE);

			// Apply matrices
			ApplyMatrices();
		}
		
		// This creates matrices for a camera view
		public void PositionAndLookAt(Vector3D pos, Vector3D lookat)
		{
			Vector3D delta;
			float anglexy, anglez;
			
			// Calculate delta vector
			delta = lookat - pos;
			anglexy = delta.GetAngleXY();
			anglez = delta.GetAngleZ();
			
			// Make the view matrix
			view = Matrix.LookAtRH(D3DDevice.V3(pos), D3DDevice.V3(lookat), new Vector3(0f, 0f, 1f));

			// Make the billboard matrix
			billboard = Matrix.RotationYawPitchRoll(0f, anglexy, anglez - Angle2D.PIHALF);
		}
		
		// This applies the matrices
		private void ApplyMatrices()
		{
			viewproj = view * projection;
			graphics.Device.SetTransform(TransformState.World, Matrix.Identity);
			graphics.Device.SetTransform(TransformState.Projection, projection);
			graphics.Device.SetTransform(TransformState.View, view);
		}

		#endregion

		#region ================== Presenting

		// This starts rendering
		public bool Start()
		{
			// Start drawing
			if(graphics.StartRendering(true, General.Colors.Background.ToColorValue(), graphics.BackBuffer, graphics.DepthBuffer))
			{
				// Beginning renderstates
				graphics.Device.SetRenderState(RenderState.CullMode, Cull.None);
				graphics.Device.SetRenderState(RenderState.ZEnable, false);
				graphics.Device.SetRenderState(RenderState.AlphaBlendEnable, false);
				graphics.Device.SetRenderState(RenderState.AlphaTestEnable, false);
				graphics.Device.SetRenderState(RenderState.SourceBlend, Blend.SourceAlpha);
				graphics.Device.SetRenderState(RenderState.DestinationBlend, Blend.InverseSourceAlpha);
				graphics.Device.SetRenderState(RenderState.TextureFactor, -1);

				// Matrices
				ApplyMatrices();
				
				// Ready
				return true;
			}
			else
			{
				// Can't render now
				return false;
			}
		}
		
		// This begins rendering world geometry
		public void StartGeometry()
		{
			// Renderstates
			graphics.Device.SetRenderState(RenderState.CullMode, Cull.Counterclockwise);
			graphics.Device.SetRenderState(RenderState.ZEnable, true);
			graphics.Device.SetRenderState(RenderState.ZWriteEnable, true);
			graphics.Device.SetRenderState(RenderState.AlphaBlendEnable, false);
			graphics.Device.SetRenderState(RenderState.AlphaTestEnable, true);
			graphics.Device.SetRenderState(RenderState.TextureFactor, -1);
			
			// Setup shader
			graphics.Shaders.World3D.Begin();
			graphics.Shaders.World3D.WorldViewProj = viewproj;
			graphics.Shaders.World3D.BeginPass(0);
		}

		// This ends rendering world geometry
		public void FinishGeometry()
		{
			// Done
			graphics.Shaders.World3D.EndPass();
			graphics.Shaders.World3D.End();
		}

		// This finishes rendering
		public void Finish()
		{
			// Done
			graphics.FinishRendering();
			graphics.Present();
		}
		
		#endregion
		
		#region ================== Geometry
		
		// This renders a visual sector's geometry
		public void RenderGeometry(VisualSector s)
		{
			ImageData lasttexture = null;
			
			// Update the sector if needed
			if(s.NeedsUpdateGeo) s.Update();
			
			// Only render when a vertexbuffer exists
			if(s.GeometryBuffer != null)
			{
				// Set the buffer
				graphics.Device.SetStreamSource(0, s.GeometryBuffer, 0, WorldVertex.Stride);

				// Go for all geometry in this sector
				foreach(VisualGeometry g in s.GeometryList)
				{
					ImageData curtexture;

					// What texture to use?
					if((g.Texture != null) && g.Texture.IsImageLoaded && !g.Texture.IsDisposed)
						curtexture = g.Texture;
					else
						curtexture = General.Map.Data.Hourglass3D;
					
					// Change texture?
					if(curtexture != lasttexture)
					{
						// Now using this texture
						lasttexture = curtexture;

						// Create Direct3D texture if still needed
						if((curtexture.Texture == null) || curtexture.Texture.Disposed)
							curtexture.CreateTexture();
						
						// Apply texture
						graphics.Device.SetTexture(0, curtexture.Texture);
						graphics.Shaders.World3D.Texture1 = curtexture.Texture;
						graphics.Shaders.World3D.ApplySettings();
					}

					// Render it!
					graphics.Device.DrawPrimitives(PrimitiveType.TriangleList, g.VertexOffset, g.Triangles);
				}

				// Remove references
				graphics.Shaders.World3D.Texture1 = null;
			}
		}

		#endregion
	}
}
