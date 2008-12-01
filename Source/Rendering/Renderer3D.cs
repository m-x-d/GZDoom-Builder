
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
using CodeImp.DoomBuilder.VisualModes;
using CodeImp.DoomBuilder.Map;

#endregion

namespace CodeImp.DoomBuilder.Rendering
{
	internal sealed class Renderer3D : Renderer, IRenderer3D
	{
		#region ================== Constants

		private const int RENDER_PASSES = 4;
		private const float PROJ_NEAR_PLANE = 1f;

		#endregion

		#region ================== Variables

		// Matrices
		private Matrix projection;
		private Matrix view;
		private Matrix billboard;
		private Matrix viewproj;
		
		// Frustum
		private ProjectedFrustum2D frustum;
		
		// Geometry to be rendered.
		// Each Dictionary in the array is a render pass.
		// Each BinaryHeap in the Dictionary contains all geometry that needs
		// to be rendered with the associated ImageData.
		// The BinaryHeap sorts the geometry by sector to minimize stream switchs.
		private Dictionary<ImageData, BinaryHeap<VisualGeometry>>[] geometry;
		
		#endregion

		#region ================== Properties

		public ProjectedFrustum2D Frustum2D { get { return frustum; } }

		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		internal Renderer3D(D3DDevice graphics) : base(graphics)
		{
			// Initialize
			CreateProjection();

			// Dummy frustum
			frustum = new ProjectedFrustum2D(new Vector2D(), 0.0f, 0.0f, PROJ_NEAR_PLANE,
				General.Settings.ViewDistance, Angle2D.DegToRad((float)General.Settings.VisualFOV));
			
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
			// Calculate aspect
			float aspect = (float)General.Map.Graphics.RenderTarget.ClientSize.Width /
						   (float)General.Map.Graphics.RenderTarget.ClientSize.Height;

			// The DirectX PerspectiveFovRH matrix method calculates the scaling in X and Y as follows:
			// yscale = 1 / tan(fovY / 2)
			// xscale = yscale / aspect
			// The fov specified in the method is the FOV over Y, but we want the user to specify the FOV
			// over X, so calculate what it would be over Y first;
			float fov = Angle2D.DegToRad((float)General.Settings.VisualFOV);
			float reversefov = 1.0f / (float)Math.Tan(fov / 2.0f);
			float reversefovy = reversefov * aspect;
			float fovy = (float)Math.Atan(1.0f / reversefovy) * 2.0f;
			
			// Make the projection matrix
			projection = Matrix.PerspectiveFovRH(fovy, aspect, PROJ_NEAR_PLANE, General.Settings.ViewDistance);

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

			// Create frustum
			frustum = new ProjectedFrustum2D(pos, anglexy, anglez, PROJ_NEAR_PLANE,
				General.Settings.ViewDistance, Angle2D.DegToRad((float)General.Settings.VisualFOV));
			
			// Make the view matrix
			view = Matrix.LookAtRH(D3DDevice.V3(pos), D3DDevice.V3(lookat), new Vector3(0f, 0f, 1f));

			// Make the billboard matrix
			billboard = Matrix.RotationYawPitchRoll(0f, anglexy, anglez - Angle2D.PI);
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
			// Make collection
			geometry = new Dictionary<ImageData, BinaryHeap<VisualGeometry>>[RENDER_PASSES];
			for(int i = 0; i < RENDER_PASSES; i++) geometry[i] = new Dictionary<ImageData, BinaryHeap<VisualGeometry>>();
		}

		// This ends rendering world geometry
		public void FinishGeometry()
		{
			// Initial renderstates
			graphics.Device.SetRenderState(RenderState.CullMode, Cull.Counterclockwise);
			graphics.Device.SetRenderState(RenderState.ZEnable, true);
			graphics.Device.SetRenderState(RenderState.ZWriteEnable, true);
			graphics.Device.SetRenderState(RenderState.AlphaBlendEnable, false);
			graphics.Device.SetRenderState(RenderState.AlphaTestEnable, false);
			graphics.Device.SetRenderState(RenderState.TextureFactor, -1);

			// SOLID PASS
			graphics.Shaders.World3D.Begin();
			graphics.Shaders.World3D.WorldViewProj = viewproj;
			graphics.Shaders.World3D.BeginPass(0);
			RenderSinglePass((int)RenderPass.Solid);
			graphics.Shaders.World3D.EndPass();
			graphics.Shaders.World3D.End();

			// MASK PASS
			graphics.Device.SetRenderState(RenderState.AlphaTestEnable, true);
			graphics.Shaders.World3D.Begin();
			graphics.Shaders.World3D.WorldViewProj = viewproj;
			graphics.Shaders.World3D.BeginPass(0);
			RenderSinglePass((int)RenderPass.Mask);
			graphics.Shaders.World3D.EndPass();
			graphics.Shaders.World3D.End();

			// ALPHA PASS
			graphics.Device.SetRenderState(RenderState.AlphaBlendEnable, true);
			graphics.Device.SetRenderState(RenderState.AlphaTestEnable, false);
			graphics.Device.SetRenderState(RenderState.ZWriteEnable, false);
			graphics.Device.SetRenderState(RenderState.SourceBlend, Blend.SourceAlpha);
			graphics.Device.SetRenderState(RenderState.DestinationBlend, Blend.InverseSourceAlpha);
			graphics.Shaders.World3D.Begin();
			graphics.Shaders.World3D.WorldViewProj = viewproj;
			graphics.Shaders.World3D.BeginPass(0);
			RenderSinglePass((int)RenderPass.Alpha);
			graphics.Shaders.World3D.EndPass();
			graphics.Shaders.World3D.End();

			// ADDITIVE PASS
			graphics.Device.SetRenderState(RenderState.DestinationBlend, Blend.One);
			graphics.Shaders.World3D.Begin();
			graphics.Shaders.World3D.WorldViewProj = viewproj;
			graphics.Shaders.World3D.BeginPass(0);
			RenderSinglePass((int)RenderPass.Additive);
			graphics.Shaders.World3D.EndPass();
			graphics.Shaders.World3D.End();
			
			// Remove references
			graphics.Shaders.World3D.Texture1 = null;
			
			// Done
			geometry = null;
		}

		// This performs a single render pass
		private void RenderSinglePass(int pass)
		{
			// Get geometry for this pass
			Dictionary<ImageData, BinaryHeap<VisualGeometry>> geo = geometry[pass];
			
			// Render the geometry collected
			foreach(KeyValuePair<ImageData, BinaryHeap<VisualGeometry>> group in geo)
			{
				ImageData curtexture;

				// What texture to use?
				if((group.Key != null) && group.Key.IsImageLoaded && !group.Key.IsDisposed)
					curtexture = group.Key;
				else
					curtexture = General.Map.Data.Hourglass3D;

				// Create Direct3D texture if still needed
				if((curtexture.Texture == null) || curtexture.Texture.Disposed)
					curtexture.CreateTexture();

				// Apply texture
				graphics.Device.SetTexture(0, curtexture.Texture);
				graphics.Shaders.World3D.Texture1 = curtexture.Texture;
				graphics.Shaders.World3D.ApplySettings();
				
				// Go for all geometry that uses this texture
				VisualSector sector = null;
				foreach(VisualGeometry g in group.Value)
				{
					// Changing sector?
					if(!object.ReferenceEquals(g.Sector, sector))
					{
						// Update the sector if needed
						if(g.Sector.NeedsUpdateGeo) g.Sector.Update();

						// Only do this sector when a vertexbuffer is created
						if(g.Sector.GeometryBuffer != null)
						{
							// Change current sector
							sector = g.Sector;

							// Set stream source
							graphics.Device.SetStreamSource(0, sector.GeometryBuffer, 0, WorldVertex.Stride);
						}
						else
						{
							sector = null;
						}
					}	
					
					// Render!
					if(sector != null)
					{
						graphics.Device.DrawPrimitives(PrimitiveType.TriangleList, g.VertexOffset, g.Triangles);
					}
				}
			}
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
		
		// This collects a visual sector's geometry for rendering
		public void AddGeometry(VisualGeometry g)
		{
			// Must have a texture!
			if(g.Texture != null)
			{
				// Texture group not yet collected?
				if(!geometry[g.RenderPassInt].ContainsKey(g.Texture))
				{
					// Create texture group
					geometry[g.RenderPassInt].Add(g.Texture, new BinaryHeap<VisualGeometry>());
				}

				// Add geometry to texture group
				geometry[g.RenderPassInt][g.Texture].Add(g);
			}
		}

		#endregion
	}
}
