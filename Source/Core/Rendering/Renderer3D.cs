
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
		private Matrix worldviewproj;
		private Matrix view2d;
		private Matrix world;
		private Vector3D cameraposition;
		private int shaderpass;
		
		// Options
		private bool fullbrightness;
		
		// Window size
		private Size windowsize;
		
		// Frustum
		private ProjectedFrustum2D frustum;
		
		// Thing cage
		private VertexBuffer thingcage;
		private bool renderthingcages;
		
		// Crosshair
		private FlatVertex[] crosshairverts;
		private bool crosshairbusy;

		// Highlighting
		private IVisualPickable highlighted;
		private float highlightglow;
		
		// Geometry to be rendered.
		// Each Dictionary in the array is a render pass.
		// Each BinaryHeap in the Dictionary contains all geometry that needs
		// to be rendered with the associated ImageData.
		// The BinaryHeap sorts the geometry by sector to minimize stream switchs.
		private Dictionary<ImageData, BinaryHeap<VisualGeometry>>[] geometry;

		// Things to be rendered.
		// Each Dictionary in the array is a render pass.
		// Each VisualThing is inserted in the Dictionary by their texture image.
		private Dictionary<ImageData, List<VisualThing>>[] things;

		// Things to be rendered, sorted by distance from camera
		private BinaryHeap<VisualThing> thingsbydistance;

		#endregion

		#region ================== Properties

		public ProjectedFrustum2D Frustum2D { get { return frustum; } }
		public bool DrawThingCages { get { return renderthingcages; } set { renderthingcages = value; } }
		public bool FullBrightness { get { return fullbrightness; } set { fullbrightness = value; } }
		
		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		internal Renderer3D(D3DDevice graphics) : base(graphics)
		{
			// Initialize
			CreateProjection();
			CreateMatrices2D();
			SetupThingCage();
			renderthingcages = true;
			
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
				if(thingcage != null) thingcage.Dispose();
				thingcage = null;
				
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
			if(thingcage != null) thingcage.Dispose();
			thingcage = null;
		}
		
		// This is called resets when the device is reset
		// (when resized or display adapter was changed)
		public override void ReloadResource()
		{
			CreateMatrices2D();
			SetupThingCage();
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

		#region ================== Thing Cage

		// This sets up the thing cage
		private void SetupThingCage()
		{
			const int totalvertices = 36;
			WorldVertex[] tv = new WorldVertex[totalvertices];
			float x0 = -1.0f;
			float x1 = 1.0f;
			float y0 = -1.0f;
			float y1 = 1.0f;
			float z0 = 0.0f;
			float z1 = 1.0f;
			float u0 = 0.0f;
			float u1 = 1.0f;
			float v0 = 0.0f;
			float v1 = 1.0f;
			int c = -1;
			
			// Front
			tv[0] = new WorldVertex(x0, y0, z0, c, u0, v0);
			tv[1] = new WorldVertex(x0, y0, z1, c, u0, v1);
			tv[2] = new WorldVertex(x1, y0, z0, c, u1, v0);
			tv[3] = new WorldVertex(x1, y0, z0, c, u1, v0);
			tv[4] = new WorldVertex(x0, y0, z1, c, u0, v1);
			tv[5] = new WorldVertex(x1, y0, z1, c, u1, v1);

			// Right
			tv[6] = new WorldVertex(x1, y0, z0, c, u0, v0);
			tv[7] = new WorldVertex(x1, y0, z1, c, u0, v1);
			tv[8] = new WorldVertex(x1, y1, z0, c, u1, v0);
			tv[9] = new WorldVertex(x1, y1, z0, c, u1, v0);
			tv[10] = new WorldVertex(x1, y0, z1, c, u0, v1);
			tv[11] = new WorldVertex(x1, y1, z1, c, u1, v1);

			// Back
			tv[12] = new WorldVertex(x1, y1, z0, c, u0, v0);
			tv[13] = new WorldVertex(x1, y1, z1, c, u0, v1);
			tv[14] = new WorldVertex(x0, y1, z0, c, u1, v0);
			tv[15] = new WorldVertex(x0, y1, z0, c, u1, v0);
			tv[16] = new WorldVertex(x1, y1, z1, c, u0, v1);
			tv[17] = new WorldVertex(x0, y1, z1, c, u1, v1);

			// Left
			tv[18] = new WorldVertex(x0, y1, z0, c, u0, v1);
			tv[19] = new WorldVertex(x0, y1, z1, c, u0, v0);
			tv[20] = new WorldVertex(x0, y0, z1, c, u1, v0);
			tv[21] = new WorldVertex(x0, y1, z0, c, u1, v0);
			tv[22] = new WorldVertex(x0, y0, z1, c, u0, v1);
			tv[23] = new WorldVertex(x0, y0, z0, c, u1, v1);

			// Top
			tv[24] = new WorldVertex(x0, y0, z1, c, u0, v0);
			tv[25] = new WorldVertex(x0, y1, z1, c, u0, v1);
			tv[26] = new WorldVertex(x1, y0, z1, c, u1, v0);
			tv[27] = new WorldVertex(x1, y0, z1, c, u1, v0);
			tv[28] = new WorldVertex(x0, y1, z1, c, u0, v1);
			tv[29] = new WorldVertex(x1, y1, z1, c, u1, v1);

			// Bottom
			tv[30] = new WorldVertex(x1, y0, z0, c, u1, v0);
			tv[31] = new WorldVertex(x0, y1, z0, c, u0, v1);
			tv[32] = new WorldVertex(x0, y0, z0, c, u0, v0);
			tv[33] = new WorldVertex(x1, y0, z0, c, u1, v0);
			tv[34] = new WorldVertex(x1, y1, z0, c, u1, v1);
			tv[35] = new WorldVertex(x0, y1, z0, c, u0, v1);

			// Create vertexbuffer
			thingcage = new VertexBuffer(General.Map.Graphics.Device, WorldVertex.Stride * totalvertices,
										 Usage.WriteOnly | Usage.Dynamic, VertexFormat.None, Pool.Default);
			DataStream bufferstream = thingcage.Lock(0, WorldVertex.Stride * totalvertices, LockFlags.Discard);
			bufferstream.WriteRange<WorldVertex>(tv);
			thingcage.Unlock();
			bufferstream.Dispose();
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
			cameraposition = pos;
			delta = lookat - pos;
			anglexy = delta.GetAngleXY();
			anglez = delta.GetAngleZ();

			// Create frustum
			frustum = new ProjectedFrustum2D(pos, anglexy, anglez, PROJ_NEAR_PLANE,
				General.Settings.ViewDistance, Angle2D.DegToRad((float)General.Settings.VisualFOV));
			
			// Make the view matrix
			view3d = Matrix.LookAtRH(D3DDevice.V3(pos), D3DDevice.V3(lookat), new Vector3(0f, 0f, 1f));
			
			// Make the billboard matrix
			billboard = Matrix.RotationZ(anglexy + Angle2D.PI);
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
			worldviewproj = world * view3d * projection;
			graphics.Shaders.World3D.WorldViewProj = worldviewproj;
			graphics.Device.SetTransform(TransformState.World, world);
			graphics.Device.SetTransform(TransformState.Projection, projection);
			graphics.Device.SetTransform(TransformState.View, view3d);
		}

		// This sets the appropriate view matrix
		public void ApplyMatrices2D()
		{
			graphics.Device.SetTransform(TransformState.World, world);
			graphics.Device.SetTransform(TransformState.Projection, Matrix.Identity);
			graphics.Device.SetTransform(TransformState.View, view2d);
		}
		
		#endregion

		#region ================== Start / Finish

		// This starts rendering
		public bool Start()
		{
			// Create thing box texture if needed
			if(General.Map.Data.ThingBox.Texture == null)
				General.Map.Data.ThingBox.CreateTexture();
			
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
				graphics.Device.SetRenderState(RenderState.FogEnable, false);
				graphics.Device.SetRenderState(RenderState.FogDensity, 1.0f);
				graphics.Device.SetRenderState(RenderState.FogColor, General.Colors.Background.ToInt());
				graphics.Device.SetRenderState(RenderState.FogStart, General.Settings.ViewDistance * FOG_RANGE);
				graphics.Device.SetRenderState(RenderState.FogEnd, General.Settings.ViewDistance);
				graphics.Device.SetRenderState(RenderState.FogTableMode, FogMode.Linear);
				graphics.Device.SetRenderState(RenderState.RangeFogEnable, false);
				graphics.Device.SetRenderState(RenderState.TextureFactor, -1);
				graphics.Shaders.World3D.SetModulateColor(-1);
				graphics.Shaders.World3D.SetHighlightColor(0);

				// Matrices
				world = Matrix.Identity;
				ApplyMatrices3D();

				// Highlight
				double time = General.Clock.GetCurrentTime();
				highlightglow = (float)Math.Sin(time / 100.0f) * 0.3f + 0.4f;

				// Determine shader pass to use
				if(fullbrightness) shaderpass = 1; else shaderpass = 0;

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
			things = new Dictionary<ImageData, List<VisualThing>>[RENDER_PASSES];
			thingsbydistance = new BinaryHeap<VisualThing>();
			for(int i = 0; i < RENDER_PASSES; i++)
			{
				geometry[i] = new Dictionary<ImageData, BinaryHeap<VisualGeometry>>();
				things[i] = new Dictionary<ImageData, List<VisualThing>>();
			}
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

			// SOLID PASS
			world = Matrix.Identity;
			ApplyMatrices3D();
			RenderSinglePass((int)RenderPass.Solid);

			// MASK PASS
			world = Matrix.Identity;
			ApplyMatrices3D();
			graphics.Device.SetRenderState(RenderState.AlphaTestEnable, true);
			RenderSinglePass((int)RenderPass.Mask);

			// ALPHA PASS
			world = Matrix.Identity;
			ApplyMatrices3D();
			graphics.Device.SetRenderState(RenderState.AlphaBlendEnable, true);
			graphics.Device.SetRenderState(RenderState.AlphaTestEnable, false);
			graphics.Device.SetRenderState(RenderState.ZWriteEnable, false);
			graphics.Device.SetRenderState(RenderState.SourceBlend, Blend.SourceAlpha);
			graphics.Device.SetRenderState(RenderState.DestinationBlend, Blend.InverseSourceAlpha);
			RenderSinglePass((int)RenderPass.Alpha);

			// THINGS
			if(renderthingcages) RenderThingCages();
			
			// ADDITIVE PASS
			world = Matrix.Identity;
			ApplyMatrices3D();
			graphics.Device.SetRenderState(RenderState.DestinationBlend, Blend.One);
			RenderSinglePass((int)RenderPass.Additive);
			
			// Remove references
			graphics.Shaders.World3D.Texture1 = null;
			
			// Done
			graphics.Shaders.World3D.End();
			geometry = null;
		}

		// This renders all thing cages
		private void RenderThingCages()
		{
			// Set renderstates
			graphics.Device.SetRenderState(RenderState.AlphaBlendEnable, true);
			graphics.Device.SetRenderState(RenderState.AlphaTestEnable, false);
			graphics.Device.SetRenderState(RenderState.ZWriteEnable, false);
			graphics.Device.SetRenderState(RenderState.SourceBlend, Blend.SourceAlpha);
			graphics.Device.SetRenderState(RenderState.DestinationBlend, Blend.InverseSourceAlpha);
			graphics.Device.SetStreamSource(0, thingcage, 0, WorldVertex.Stride);
			graphics.Device.SetTexture(0, General.Map.Data.ThingBox.Texture);
			graphics.Shaders.World3D.Texture1 = General.Map.Data.ThingBox.Texture;

			graphics.Shaders.World3D.BeginPass(shaderpass);
			foreach(VisualThing t in thingsbydistance)
			{
				// Setup matrix
				world = Matrix.Multiply(t.CageScales, t.Position);
				ApplyMatrices3D();

				// Setup color
				graphics.Shaders.World3D.SetModulateColor(t.CageColor);
				graphics.Device.SetRenderState(RenderState.TextureFactor, t.CageColor);

				// Render!
				graphics.Shaders.World3D.ApplySettings();
				graphics.Device.DrawPrimitives(PrimitiveType.TriangleList, 0, 12);
			}

			// Done
			graphics.Shaders.World3D.EndPass();
			graphics.Shaders.World3D.SetModulateColor(-1);
			graphics.Device.SetRenderState(RenderState.TextureFactor, -1);
		}

		// This performs a single render pass
		private void RenderSinglePass(int pass)
		{
			// Get geometry for this pass
			Dictionary<ImageData, BinaryHeap<VisualGeometry>> geopass = geometry[pass];

			// Begin rendering with this shader
			graphics.Shaders.World3D.BeginPass(shaderpass);
			
			// Render the geometry collected
			foreach(KeyValuePair<ImageData, BinaryHeap<VisualGeometry>> group in geopass)
			{
				ImageData curtexture;

				// What texture to use?
				if(group.Key is UnknownImage)
					curtexture = General.Map.Data.UnknownTexture3D;
				else if((group.Key != null) && group.Key.IsImageLoaded && !group.Key.IsDisposed)
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
					
					if(sector != null)
					{
						// Highlight this object?
						if(g == highlighted)
						{
							// Temporarely switch shader and use a highlight color
							graphics.Shaders.World3D.EndPass();
							Color4 highlight = General.Colors.Highlight.ToColorValue();
							highlight.Alpha = highlightglow;
							graphics.Shaders.World3D.SetHighlightColor(highlight.ToArgb());
							graphics.Shaders.World3D.BeginPass(shaderpass + 2);
						}
						
						// Render!
						graphics.Device.DrawPrimitives(PrimitiveType.TriangleList, g.VertexOffset, g.Triangles);
						
						// Reset highlight settings
						if(g == highlighted)
						{
							graphics.Shaders.World3D.EndPass();
							graphics.Shaders.World3D.SetHighlightColor(0);
							graphics.Shaders.World3D.BeginPass(shaderpass);
						}
					}
				}
			}

			// Get things for this pass
			Dictionary<ImageData, List<VisualThing>> thingspass = things[pass];
			
			// Render things collected
			foreach(KeyValuePair<ImageData, List<VisualThing>> group in thingspass)
			{
				ImageData curtexture;

				if(!(group.Key is UnknownImage))
				{
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

					// Render all things with this texture
					foreach(VisualThing t in group.Value)
					{
						// Update buffer if needed
						t.Update();

						// Only do this sector when a vertexbuffer is created
						if(t.GeometryBuffer != null)
						{
							// Highlight this object?
							if(t == highlighted)
							{
								// Temporarely switch shader and use a highlight color
								graphics.Shaders.World3D.EndPass();
								Color4 highlight = General.Colors.Highlight.ToColorValue();
								highlight.Alpha = highlightglow;
								graphics.Shaders.World3D.SetHighlightColor(highlight.ToArgb());
								graphics.Shaders.World3D.BeginPass(shaderpass + 2);
							}

							// Create the matrix for positioning / rotation
							world = t.Orientation;
							if(t.Billboard) world = Matrix.Multiply(world, billboard);
							world = Matrix.Multiply(world, t.Position);
							ApplyMatrices3D();
							graphics.Shaders.World3D.ApplySettings();

							// Apply buffer
							graphics.Device.SetStreamSource(0, t.GeometryBuffer, 0, WorldVertex.Stride);

							// Render!
							graphics.Device.DrawPrimitives(PrimitiveType.TriangleList, 0, t.Triangles);

							// Reset highlight settings
							if(t == highlighted)
							{
								graphics.Shaders.World3D.EndPass();
								graphics.Shaders.World3D.SetHighlightColor(0);
								graphics.Shaders.World3D.BeginPass(shaderpass);
							}
						}
					}
				}
			}

			// Done rendering with this shader
			graphics.Shaders.World3D.EndPass();
		}
		
		// This finishes rendering
		public void Finish()
		{
			// Done
			graphics.FinishRendering();
			graphics.Present();
			highlighted = null;
		}
		
		#endregion
		
		#region ================== Rendering
		
		// This sets the highlighted object for the rendering
		public void SetHighlightedObject(IVisualPickable obj)
		{
			highlighted = obj;
		}
		
		// This collects a visual sector's geometry for rendering
		public void AddSectorGeometry(VisualGeometry g)
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

		// This collects a visual sector's geometry for rendering
		public void AddThingGeometry(VisualThing t)
		{
			// Make sure the distance to camera is calculated
			t.CalculateCameraDistance(cameraposition);
			thingsbydistance.Add(t);
			
			// Must have a texture!
			if(t.Texture != null)
			{
				// Texture group not yet collected?
				if(!things[t.RenderPassInt].ContainsKey(t.Texture))
				{
					// Create texture group
					things[t.RenderPassInt].Add(t.Texture, new List<VisualThing>());
				}

				// Add geometry to texture group
				things[t.RenderPassInt][t.Texture].Add(t);
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
			
			// Texture
			if(crosshairbusy)
			{
				if(General.Map.Data.CrosshairBusy3D.Texture == null) General.Map.Data.CrosshairBusy3D.CreateTexture();
				graphics.Device.SetTexture(0, General.Map.Data.CrosshairBusy3D.Texture);
				graphics.Shaders.Display2D.Texture1 = General.Map.Data.CrosshairBusy3D.Texture;
			}
			else
			{
				if(General.Map.Data.Crosshair3D.Texture == null) General.Map.Data.Crosshair3D.CreateTexture();
				graphics.Device.SetTexture(0, General.Map.Data.Crosshair3D.Texture);
				graphics.Shaders.Display2D.Texture1 = General.Map.Data.Crosshair3D.Texture;
			}
			
			// Draw
			graphics.Shaders.Display2D.Begin();
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

		// This siwtches crosshair busy icon on and off
		public void SetCrosshairBusy(bool busy)
		{
			crosshairbusy = busy;
		}
		
		#endregion
	}
}
