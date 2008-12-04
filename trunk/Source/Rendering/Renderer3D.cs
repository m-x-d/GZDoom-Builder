
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
		private const float CROSSHAIR_SCALE = 0.06f;
		private const float FOG_RANGE = 0.9f;
		
		#endregion

		#region ================== Variables

		// Matrices
		private Matrix projection;
		private Matrix view3d;
		private Matrix billboard;
		private Matrix viewproj;
		private Matrix view2d;
		
		// Window size
		private Size windowsize;
		
		// Frustum
		private ProjectedFrustum2D frustum;
		
		// Crosshair
		private FlatVertex[] crosshairverts;
		
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
			CreateMatrices2D();
			
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

		#region ================== Management

		// This is called before a device is reset
		// (when resized or display adapter was changed)
		public override void UnloadResource()
		{
			crosshairverts = null;
		}
		
		// This is called resets when the device is reset
		// (when resized or display adapter was changed)
		public override void ReloadResource()
		{
			CreateMatrices2D();
		}

		// This makes screen vertices for display
		private void CreateCrosshairVerts(Size texturesize)
		{
			// Determine coordinates
			float width = (float)windowsize.Width;
			float height = (float)windowsize.Height;
			float size = (float)height * CROSSHAIR_SCALE;
			RectangleF rect = new RectangleF((width - size) / 2, (height - size) / 2, size, size);
			
			// Make vertices
			crosshairverts = new FlatVertex[4];
			crosshairverts[0].x = rect.Left;
			crosshairverts[0].y = rect.Top;
			crosshairverts[0].c = -1;
			crosshairverts[0].u = 1f / texturesize.Width;
			crosshairverts[0].v = 1f / texturesize.Height;
			crosshairverts[1].x = rect.Right;
			crosshairverts[1].y = rect.Top;
			crosshairverts[1].c = -1;
			crosshairverts[1].u = 1f - 1f / texturesize.Width;
			crosshairverts[1].v = 1f / texturesize.Height;
			crosshairverts[2].x = rect.Left;
			crosshairverts[2].y = rect.Bottom;
			crosshairverts[2].c = -1;
			crosshairverts[2].u = 1f / texturesize.Width;
			crosshairverts[2].v = 1f - 1f / texturesize.Height;
			crosshairverts[3].x = rect.Right;
			crosshairverts[3].y = rect.Bottom;
			crosshairverts[3].c = -1;
			crosshairverts[3].u = 1f - 1f / texturesize.Width;
			crosshairverts[3].v = 1f - 1f / texturesize.Height;
		}
		
		#endregion

		#region ================== Presentation

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
			ApplyMatrices3D();
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
			view3d = Matrix.LookAtRH(D3DDevice.V3(pos), D3DDevice.V3(lookat), new Vector3(0f, 0f, 1f));

			// Make the billboard matrix
			billboard = Matrix.RotationYawPitchRoll(0f, anglexy, anglez - Angle2D.PI);
		}

		// This creates 2D view matrix
		private void CreateMatrices2D()
		{
			windowsize = graphics.RenderTarget.ClientSize;
			Matrix scaling = Matrix.Scaling((1f / (float)windowsize.Width) * 2f, (1f / (float)windowsize.Height) * -2f, 1f);
			Matrix translate = Matrix.Translation(-(float)windowsize.Width * 0.5f, -(float)windowsize.Height * 0.5f, 0f);
			view2d = Matrix.Multiply(translate, scaling);
		}
		
		// This applies the matrices
		private void ApplyMatrices3D()
		{
			viewproj = view3d * projection;
			graphics.Device.SetTransform(TransformState.World, Matrix.Identity);
			graphics.Device.SetTransform(TransformState.Projection, projection);
			graphics.Device.SetTransform(TransformState.View, view3d);
		}

		// This sets the appropriate view matrix
		public void ApplyMatrices2D()
		{
			graphics.Device.SetTransform(TransformState.World, Matrix.Identity);
			graphics.Device.SetTransform(TransformState.Projection, Matrix.Identity);
			graphics.Device.SetTransform(TransformState.View, view2d);
		}
		
		#endregion

		#region ================== Start / Finish

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
				graphics.Device.SetRenderState(RenderState.FogEnable, false);
				graphics.Device.SetRenderState(RenderState.FogDensity, 1.0f);
				graphics.Device.SetRenderState(RenderState.FogColor, General.Colors.Background.ToInt());
				graphics.Device.SetRenderState(RenderState.FogStart, General.Settings.ViewDistance * FOG_RANGE);
				graphics.Device.SetRenderState(RenderState.FogEnd, General.Settings.ViewDistance);
				graphics.Device.SetRenderState(RenderState.FogTableMode, FogMode.Linear);
				graphics.Device.SetRenderState(RenderState.RangeFogEnable, false);

				// Matrices
				ApplyMatrices3D();

				// Create crosshair vertices
				if(crosshairverts == null)
					CreateCrosshairVerts(new Size(General.Map.Data.Crosshair3D.Width, General.Map.Data.Crosshair3D.Height));
				
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
			graphics.Shaders.World3D.Begin();
			graphics.Shaders.World3D.WorldViewProj = viewproj;

			// Matrices
			ApplyMatrices3D();
			
			// SOLID PASS
			graphics.Shaders.World3D.BeginPass(0);
			RenderSinglePass((int)RenderPass.Solid);
			graphics.Shaders.World3D.EndPass();

			// MASK PASS
			graphics.Device.SetRenderState(RenderState.AlphaTestEnable, true);
			graphics.Shaders.World3D.BeginPass(0);
			RenderSinglePass((int)RenderPass.Mask);
			graphics.Shaders.World3D.EndPass();

			// ALPHA PASS
			graphics.Device.SetRenderState(RenderState.AlphaBlendEnable, true);
			graphics.Device.SetRenderState(RenderState.AlphaTestEnable, false);
			graphics.Device.SetRenderState(RenderState.ZWriteEnable, false);
			graphics.Device.SetRenderState(RenderState.SourceBlend, Blend.SourceAlpha);
			graphics.Device.SetRenderState(RenderState.DestinationBlend, Blend.InverseSourceAlpha);
			graphics.Shaders.World3D.BeginPass(0);
			RenderSinglePass((int)RenderPass.Alpha);
			graphics.Shaders.World3D.EndPass();

			// ADDITIVE PASS
			graphics.Device.SetRenderState(RenderState.DestinationBlend, Blend.One);
			graphics.Shaders.World3D.BeginPass(0);
			RenderSinglePass((int)RenderPass.Additive);
			graphics.Shaders.World3D.EndPass();
			
			// Remove references
			graphics.Shaders.World3D.Texture1 = null;
			
			// Done
			graphics.Shaders.World3D.End();
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
		
		#region ================== Rendering
		
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

		// This renders the crosshair
		public void RenderCrosshair()
		{
			// Set renderstates
			graphics.Device.SetRenderState(RenderState.CullMode, Cull.None);
			graphics.Device.SetRenderState(RenderState.ZEnable, false);
			graphics.Device.SetRenderState(RenderState.AlphaBlendEnable, true);
			graphics.Device.SetRenderState(RenderState.AlphaTestEnable, false);
			graphics.Device.SetRenderState(RenderState.SourceBlend, Blend.SourceAlpha);
			graphics.Device.SetRenderState(RenderState.DestinationBlend, Blend.InverseSourceAlpha);
			graphics.Device.SetRenderState(RenderState.TextureFactor, -1);
			graphics.Device.SetTransform(TransformState.World, Matrix.Identity);
			graphics.Device.SetTransform(TransformState.Projection, Matrix.Identity);
			ApplyMatrices2D();
			
			// Draw
			graphics.Shaders.Display2D.Begin();
			if(General.Map.Data.Crosshair3D.Texture == null) General.Map.Data.Crosshair3D.CreateTexture();
			graphics.Device.SetTexture(0, General.Map.Data.Crosshair3D.Texture);
			graphics.Shaders.Display2D.Texture1 = General.Map.Data.Crosshair3D.Texture;
			graphics.Shaders.Display2D.SetSettings(1.0f, 1.0f, 0.0f, 1.0f, true);
			graphics.Shaders.Display2D.BeginPass(1);
			graphics.Device.DrawUserPrimitives<FlatVertex>(PrimitiveType.TriangleStrip, 0, 2, crosshairverts);
			graphics.Shaders.Display2D.EndPass();
			graphics.Shaders.Display2D.End();
		}

		// This switches fog on and off
		public void SetFogMode(bool usefog)
		{
			graphics.Device.SetRenderState(RenderState.FogEnable, usefog);
		}
		
		#endregion
	}
}
