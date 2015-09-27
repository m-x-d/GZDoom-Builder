
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
using System.Collections.Generic;
using System.Drawing;
using CodeImp.DoomBuilder.Config;
using SlimDX;
using CodeImp.DoomBuilder.Geometry;
using SlimDX.Direct3D9;
using CodeImp.DoomBuilder.Data;
using CodeImp.DoomBuilder.VisualModes;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.GZBuilder.Data; //mxd
using CodeImp.DoomBuilder.GZBuilder.Geometry; //mxd
using CodeImp.DoomBuilder.GZBuilder.Rendering; //mxd

#endregion

namespace CodeImp.DoomBuilder.Rendering
{
	internal sealed class Renderer3D : Renderer, IRenderer3D
	{
		#region ================== Constants

		private const float PROJ_NEAR_PLANE = 1f;
		private const float FOG_RANGE = 0.9f;
		internal const float GZDOOM_VERTICAL_VIEW_STRETCH = 1.2f;
		internal const float GZDOOM_INVERTED_VERTICAL_VIEW_STRETCH = 1.0f / GZDOOM_VERTICAL_VIEW_STRETCH;
		
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
		
		// Window size
		private Size windowsize;
		
		// Frustum
		private ProjectedFrustum2D frustum;
		
		// Thing cage
		private bool renderthingcages;
		//mxd
		private ThingBoundingBox bbox;
		private VisualVertexHandle vertexHandle;
		private SizelessVisualThingCage sizelessThingHandle;
		private List<VisualThing> lightthings;
		private int[] lightOffsets;
		private Dictionary<ModelData, List<VisualThing>> modelthings;
		
		// Crosshair
		private FlatVertex[] crosshairverts;
		private bool crosshairbusy;

		// Highlighting
		private IVisualPickable highlighted;
		private float highlightglow;
		private float highlightglowinv;
		private ColorImage highlightimage;
		private ColorImage selectionimage;
		private bool showselection;
		private bool showhighlight;
		
		//mxd. Solid geometry to be rendered. Must be sorted by sector.
		private Dictionary<ImageData, List<VisualGeometry>> solidgeo;

		//mxd. Masked geometry to be rendered. Must be sorted by sector.
		private Dictionary<ImageData, List<VisualGeometry>> maskedgeo;

		// mxd. Translucent geometry to be rendered. Must be sorted by camera distance.
		private List<VisualGeometry> translucentgeo;

		//mxd. Solid things to be rendered (currently(?) there won't be any). Must be sorted by sector.
		private Dictionary<ImageData, List<VisualThing>> solidthings;

		//mxd. Masked things to be rendered. Must be sorted by sector.
		private Dictionary<ImageData, List<VisualThing>> maskedthings;

		//mxd. Translucent things to be rendered. Must be sorted by camera distance.
		private List<VisualThing> translucentthings;

		//mxd. All things. Used to render thing cages
		private List<VisualThing> allthings;

		//mxd. Visual vertices
		private VisualVertex[] visualvertices;
		
		#endregion

		#region ================== Properties

		public ProjectedFrustum2D Frustum2D { get { return frustum; } }
		public bool DrawThingCages { get { return renderthingcages; } set { renderthingcages = value; } }
		public bool ShowSelection { get { return showselection; } set { showselection = value; } }
		public bool ShowHighlight { get { return showhighlight; } set { showhighlight = value; } }
		
		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		internal Renderer3D(D3DDevice graphics) : base(graphics)
		{
			// Initialize
			CreateProjection();
			CreateMatrices2D();
			SetupHelperObjects(); //mxd
			SetupTextures();
			renderthingcages = true;
			showselection = true;
			showhighlight = true;
			
			// Dummy frustum
			frustum = new ProjectedFrustum2D(new Vector2D(), 0.0f, 0.0f, PROJ_NEAR_PLANE,
				General.Settings.ViewDistance, Angle2D.DegToRad(General.Settings.VisualFOV));

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
				if(selectionimage != null) selectionimage.Dispose();
				if(highlightimage != null) highlightimage.Dispose();
				selectionimage = null;
				highlightimage = null;
				
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

			if(selectionimage != null) selectionimage.Dispose();
			if(highlightimage != null) highlightimage.Dispose();
			selectionimage = null;
			highlightimage = null;
		}
		
		// This is called resets when the device is reset
		// (when resized or display adapter was changed)
		public override void ReloadResource()
		{
			CreateMatrices2D();
			SetupTextures();
		}

		// This makes screen vertices for display
		private void CreateCrosshairVerts(Size texturesize)
		{
			// Determine coordinates
			float width = windowsize.Width;
			float height = windowsize.Height;
			RectangleF rect = new RectangleF((float)Math.Round((width - texturesize.Width) * 0.5f), (float)Math.Round((height - texturesize.Height) * 0.5f), texturesize.Width, texturesize.Height);
			
			// Make vertices
			crosshairverts = new FlatVertex[4];
			crosshairverts[0].x = rect.Left;
			crosshairverts[0].y = rect.Top;
			crosshairverts[0].c = -1;
			crosshairverts[1].x = rect.Right;
			crosshairverts[1].y = rect.Top;
			crosshairverts[1].c = -1;
			crosshairverts[1].u = 1.0f;
			crosshairverts[2].x = rect.Left;
			crosshairverts[2].y = rect.Bottom;
			crosshairverts[2].c = -1;
			crosshairverts[2].v = 1.0f;
			crosshairverts[3].x = rect.Right;
			crosshairverts[3].y = rect.Bottom;
			crosshairverts[3].c = -1;
			crosshairverts[3].u = 1.0f;
			crosshairverts[3].v = 1.0f;
		}
		
		#endregion

		#region ================== Resources

		// This loads the textures for highlight and selection if we need them
		private void SetupTextures()
		{
			if(!graphics.Shaders.Enabled)
			{
				highlightimage = new ColorImage(General.Colors.Highlight, 32, 32);
				highlightimage.LoadImage();
				highlightimage.CreateTexture();
				
				selectionimage = new ColorImage(General.Colors.Selection, 32, 32);
				selectionimage.LoadImage();
				selectionimage.CreateTexture();
			}
		}

		//mxd
		private void SetupHelperObjects() 
		{
			bbox = new ThingBoundingBox();
			sizelessThingHandle = new SizelessVisualThingCage();
			vertexHandle = new VisualVertexHandle();
		}

		//mxd
		internal void UpdateVertexHandle()
		{
			vertexHandle.UnloadResource();
			vertexHandle.ReloadResource();
		}

		#endregion
		
		#region ================== Presentation

		// This creates the projection
		internal void CreateProjection()
		{
			// Calculate aspect
			float screenheight = General.Map.Graphics.RenderTarget.ClientSize.Height * (General.Settings.GZStretchView ? GZDOOM_INVERTED_VERTICAL_VIEW_STRETCH : 1.0f); //mxd
			float aspect = General.Map.Graphics.RenderTarget.ClientSize.Width / screenheight;
			
			// The DirectX PerspectiveFovRH matrix method calculates the scaling in X and Y as follows:
			// yscale = 1 / tan(fovY / 2)
			// xscale = yscale / aspect
			// The fov specified in the method is the FOV over Y, but we want the user to specify the FOV
			// over X, so calculate what it would be over Y first;
			float fov = Angle2D.DegToRad(General.Settings.VisualFOV);
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
			// Calculate delta vector
			cameraposition = pos;
			Vector3D delta = lookat - pos;
			float anglexy = delta.GetAngleXY();
			float anglez = delta.GetAngleZ();

			// Create frustum
			frustum = new ProjectedFrustum2D(pos, anglexy, anglez, PROJ_NEAR_PLANE,
				General.Settings.ViewDistance, Angle2D.DegToRad(General.Settings.VisualFOV));
			
			// Make the view matrix
			view3d = Matrix.LookAtRH(D3DDevice.V3(pos), D3DDevice.V3(lookat), new Vector3(0f, 0f, 1f));
			
			// Make the billboard matrix
			billboard = Matrix.RotationZ(anglexy + Angle2D.PI);
		}
		
		// This creates 2D view matrix
		private void CreateMatrices2D()
		{
			windowsize = graphics.RenderTarget.ClientSize;
			Matrix scaling = Matrix.Scaling((1f / windowsize.Width) * 2f, (1f / windowsize.Height) * -2f, 1f);
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
				graphics.Shaders.World3D.SetHighlightColor(0);

				// Texture addressing
				graphics.Device.SetSamplerState(0, SamplerState.AddressU, TextureAddress.Wrap);
				graphics.Device.SetSamplerState(0, SamplerState.AddressV, TextureAddress.Wrap);
				graphics.Device.SetSamplerState(0, SamplerState.AddressW, TextureAddress.Wrap);

				// Matrices
				world = Matrix.Identity;
				ApplyMatrices3D();

				// Highlight
				if(General.Settings.AnimateVisualSelection)
				{
					float time = Clock.CurrentTime;
					highlightglow = (float)Math.Sin(time / 100.0f) * 0.1f + 0.4f;
					highlightglowinv = -highlightglow + 0.8f;
				}
				else
				{
					highlightglow = 0.4f;
					highlightglowinv = 0.3f;
				}
				
				// Determine shader pass to use
				shaderpass = (fullbrightness ? 1 : 0);

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
			// Make collections
			solidgeo = new Dictionary<ImageData, List<VisualGeometry>>(); //mxd
			maskedgeo = new Dictionary<ImageData, List<VisualGeometry>>(); //mxd
			translucentgeo = new List<VisualGeometry>(); //mxd

			solidthings = new Dictionary<ImageData, List<VisualThing>>(); //mxd
			maskedthings = new Dictionary<ImageData, List<VisualThing>>(); //mxd
			translucentthings = new List<VisualThing>(); //mxd
			
			modelthings = new Dictionary<ModelData, List<VisualThing>>(); //mxd
			lightthings = new List<VisualThing>(); //mxd
			allthings = new List<VisualThing>(); //mxd
		}

		// This ends rendering world geometry
		public void FinishGeometry()
		{
			//mxd. Sort lights
			if(General.Settings.GZDrawLightsMode != LightRenderMode.NONE && !fullbrightness && lightthings.Count > 0)
				UpdateLights();
			
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
			RenderSinglePass(solidgeo, solidthings);

			//mxd. Render models, without culling.
			graphics.Device.SetRenderState(RenderState.AlphaTestEnable, true);
			graphics.Device.SetRenderState(RenderState.CullMode, Cull.None);
			RenderModels();
			graphics.Device.SetRenderState(RenderState.CullMode, Cull.Counterclockwise);

			// MASK PASS
			world = Matrix.Identity;
			ApplyMatrices3D();
			RenderSinglePass(maskedgeo, maskedthings);

			//mxd. LIGHT PASS
			world = Matrix.Identity;
			ApplyMatrices3D();
			graphics.Device.SetRenderState(RenderState.AlphaBlendEnable, true);
			graphics.Device.SetRenderState(RenderState.AlphaTestEnable, false);
			graphics.Device.SetRenderState(RenderState.ZWriteEnable, false);
			graphics.Device.SetRenderState(RenderState.DestinationBlend, Blend.One);
			if(General.Settings.GZDrawLightsMode != LightRenderMode.NONE && !fullbrightness && lightthings.Count > 0)
			{
				RenderLights(solidgeo, lightthings);
				RenderLights(maskedgeo, lightthings);
			}

			// ALPHA AND ADDITIVE PASS
			world = Matrix.Identity;
			ApplyMatrices3D();
			graphics.Device.SetRenderState(RenderState.SourceBlend, Blend.SourceAlpha);
			RenderTranslucentPass(translucentgeo, translucentthings);

			// THING CAGES
			if(renderthingcages) RenderThingCages();

			//mxd. Visual vertices
			RenderVertices();

			//mxd. Event lines
			if(General.Settings.GZShowEventLines) RenderArrows(LinksCollector.GetThingLinks(allthings));
			
			// Remove references
			graphics.Shaders.World3D.Texture1 = null;
			
			// Done
			graphics.Shaders.World3D.End();

			//mxd. Trash collections
			solidgeo = null;
			maskedgeo = null;
			translucentgeo = null;

			solidthings = null;
			maskedthings = null;
			translucentthings = null;
			
			allthings = null;
			lightthings = null;
			modelthings = null;

			visualvertices = null;
		}

		//mxd
		private void UpdateLights()
		{
			if(lightthings.Count > General.Settings.GZMaxDynamicLights) 
			{
				// Calculate distance to camera
				foreach(VisualThing t in lightthings) t.CalculateCameraDistance(cameraposition);

				// Sort by it, closer ones first
				lightthings.Sort((t1, t2) => Math.Sign(t1.CameraDistance - t2.CameraDistance));
				
				// Gather the closest
				List<VisualThing> tl = new List<VisualThing>(General.Settings.GZMaxDynamicLights);
				for(int i = 0; i < General.Settings.GZMaxDynamicLights; i++) tl.Add(lightthings[i]);
				lightthings = tl;
			}

			// Sort things by light render style
			lightthings.Sort((t1, t2) => Math.Sign(t1.LightRenderStyle - t2.LightRenderStyle));
			lightOffsets = new int[3];

			foreach(VisualThing t in lightthings) 
			{
				//add light to apropriate array.
				switch(t.LightRenderStyle) 
				{
					case DynamicLightRenderStyle.NORMAL:
					case DynamicLightRenderStyle.VAVOOM: lightOffsets[0]++; break;
					case DynamicLightRenderStyle.ADDITIVE: lightOffsets[1]++; break;
					default: lightOffsets[2]++; break;
				}
			}
		}

		//mxd.
		//I never particularly liked old ThingCages, so I wrote this instead.
		//It should render faster and it has fancy arrow! :)
		private void RenderThingCages() 
		{
			graphics.Device.SetRenderState(RenderState.AlphaBlendEnable, true);
			graphics.Device.SetRenderState(RenderState.AlphaTestEnable, false);
			graphics.Device.SetRenderState(RenderState.ZWriteEnable, false);
			graphics.Device.SetRenderState(RenderState.SourceBlend, Blend.SourceAlpha);
			graphics.Device.SetRenderState(RenderState.DestinationBlend, Blend.SourceAlpha);

			graphics.Shaders.World3D.BeginPass(16);

			foreach(VisualThing t in allthings)
			{
				// Setup matrix
				world = Matrix.Multiply(t.CageScales, t.Position);
				ApplyMatrices3D();

				// Setup color
				Color4 thingcolor;
				if(t.Selected && showselection) 
				{
					thingcolor = General.Colors.Selection3D.ToColorValue();
				} 
				else
				{
					thingcolor = t.CageColor;
					if(t != highlighted) thingcolor.Alpha = 0.6f;
				}
				graphics.Shaders.World3D.VertexColor = thingcolor;

				//Render cage
				graphics.Shaders.World3D.ApplySettings();

				if(t.Sizeless) 
				{
					graphics.Device.SetStreamSource(0, sizelessThingHandle.Shape, 0, WorldVertex.Stride);
					graphics.Device.DrawPrimitives(PrimitiveType.LineList, 0, 3);
				} 
				else 
				{
					graphics.Device.SetStreamSource(0, bbox.Cage, 0, WorldVertex.Stride);
					graphics.Device.DrawPrimitives(PrimitiveType.LineList, 0, 12);
				}

				//and arrow
				if(t.Thing.IsDirectional) 
				{
					float sx = t.CageScales.M11;
					Matrix arrowScaler = Matrix.Scaling(sx, sx, sx); //scale arrow evenly based on thing width\depth
					if(t.Sizeless) 
						world = Matrix.Multiply(arrowScaler, t.Position);
					else 
						world = Matrix.Multiply(arrowScaler, t.Position * Matrix.Translation(0.0f, 0.0f, t.CageScales.M33 / 2));
					Matrix rot = Matrix.RotationY(-t.Thing.RollRad) * Matrix.RotationX(-t.Thing.PitchRad) * Matrix.RotationZ(t.Thing.Angle);
					world = Matrix.Multiply(rot, world);
					ApplyMatrices3D();

					graphics.Shaders.World3D.ApplySettings();
					graphics.Device.SetStreamSource(0, bbox.Arrow, 0, WorldVertex.Stride);
					graphics.Device.DrawPrimitives(PrimitiveType.LineList, 0, 5);
				}
			}

			// Done
			graphics.Shaders.World3D.EndPass();
			graphics.Device.SetRenderState(RenderState.TextureFactor, -1);
		}

		//mxd
		private void RenderVertices() 
		{
			if(visualvertices == null) return;

			graphics.Device.SetRenderState(RenderState.AlphaBlendEnable, true);
			graphics.Device.SetRenderState(RenderState.AlphaTestEnable, false);
			graphics.Device.SetRenderState(RenderState.ZWriteEnable, false);
			graphics.Device.SetRenderState(RenderState.SourceBlend, Blend.SourceAlpha);
			graphics.Device.SetRenderState(RenderState.DestinationBlend, Blend.SourceAlpha);

			graphics.Shaders.World3D.BeginPass(16);

			foreach(VisualVertex v in visualvertices) 
			{
				world = v.Position;
				ApplyMatrices3D();

				// Setup color
				Color4 color;
				if(v.Selected && showselection) 
				{
					color = General.Colors.Selection3D.ToColorValue();
				} 
				else 
				{
					color = v.HaveHeightOffset ? General.Colors.InfoLine.ToColorValue() : General.Colors.Vertices.ToColorValue();
					if(v != highlighted) color.Alpha = 0.6f;
				}
				graphics.Shaders.World3D.VertexColor = color;

				//Commence drawing!!11
				graphics.Shaders.World3D.ApplySettings();
				graphics.Device.SetStreamSource(0, v.CeilingVertex ? vertexHandle.Upper : vertexHandle.Lower, 0, WorldVertex.Stride);
				graphics.Device.DrawPrimitives(PrimitiveType.LineList, 0, 8);
			}

			// Done
			graphics.Shaders.World3D.EndPass();
			graphics.Device.SetRenderState(RenderState.TextureFactor, -1);
		}

		//mxd
		private void RenderArrows(ICollection<Line3D> lines) 
		{
			// Calculate required points count
			if(lines.Count == 0) return;
			int pointscount = 0;
			foreach(Line3D line in lines) pointscount += (line.renderarrowhead ? 6 : 2); // 4 extra points for the arrowhead
			if(pointscount < 2) return;
			
			//create vertices
			WorldVertex[] verts = new WorldVertex[pointscount];
			const float scaler = 20f;
			int color;
			pointscount = 0;

			foreach(Line3D line in lines)
			{
				color = line.color.ToInt();

				// Add regular points
				verts[pointscount].x = line.v1.x;
				verts[pointscount].y = line.v1.y;
				verts[pointscount].z = line.v1.z;
				verts[pointscount].c = color;
				pointscount++;

				verts[pointscount].x = line.v2.x;
				verts[pointscount].y = line.v2.y;
				verts[pointscount].z = line.v2.z;
				verts[pointscount].c = color;
				pointscount++;

				// Add arrowhead
				if(line.renderarrowhead)
				{
					float nz = line.GetDelta().GetNormal().z * scaler;
					float angle = line.GetAngle();
					Vector3D a1 = new Vector3D(line.v2.x - scaler * (float)Math.Sin(angle - 0.46f), line.v2.y + scaler * (float)Math.Cos(angle - 0.46f), line.v2.z - nz);
					Vector3D a2 = new Vector3D(line.v2.x - scaler * (float)Math.Sin(angle + 0.46f), line.v2.y + scaler * (float)Math.Cos(angle + 0.46f), line.v2.z - nz);

					verts[pointscount] = verts[pointscount - 1];
					verts[pointscount + 1].x = a1.x;
					verts[pointscount + 1].y = a1.y;
					verts[pointscount + 1].z = a1.z;
					verts[pointscount + 1].c = color;

					verts[pointscount + 2] = verts[pointscount - 1];
					verts[pointscount + 3].x = a2.x;
					verts[pointscount + 3].y = a2.y;
					verts[pointscount + 3].z = a2.z;
					verts[pointscount + 3].c = color;

					pointscount += 4;
				}
			}

			VertexBuffer vb = new VertexBuffer(General.Map.Graphics.Device, WorldVertex.Stride * verts.Length, Usage.WriteOnly | Usage.Dynamic, VertexFormat.None, Pool.Default);
			DataStream s = vb.Lock(0, WorldVertex.Stride * verts.Length, LockFlags.Discard);
			s.WriteRange(verts);
			vb.Unlock();
			s.Dispose();
			
			//begin rendering
			graphics.Device.SetRenderState(RenderState.AlphaBlendEnable, true);
			graphics.Device.SetRenderState(RenderState.AlphaTestEnable, false);
			graphics.Device.SetRenderState(RenderState.ZWriteEnable, false);
			graphics.Device.SetRenderState(RenderState.SourceBlend, Blend.SourceAlpha);
			graphics.Device.SetRenderState(RenderState.DestinationBlend, Blend.SourceAlpha);

			graphics.Shaders.World3D.BeginPass(15);

			world = Matrix.Identity;
			ApplyMatrices3D();

			//render
			graphics.Shaders.World3D.ApplySettings();
			graphics.Device.SetStreamSource(0, vb, 0, WorldVertex.Stride);
			graphics.Device.DrawPrimitives(PrimitiveType.LineList, 0, pointscount / 2);

			// Done
			graphics.Shaders.World3D.EndPass();
			graphics.Device.SetRenderState(RenderState.TextureFactor, -1);
			vb.Dispose();
		}

		// This performs a single render pass
		private void RenderSinglePass(Dictionary<ImageData, List<VisualGeometry>> geopass, Dictionary<ImageData, List<VisualThing>> thingspass)
		{
			//mxd. Anything to render?
			if(geopass.Count == 0 && thingspass.Count == 0) return;
			
			int currentshaderpass = shaderpass;
			int highshaderpass = shaderpass + 2;

			// Begin rendering with this shader
			graphics.Shaders.World3D.BeginPass(shaderpass);

			// Render the geometry collected
			foreach(KeyValuePair<ImageData, List<VisualGeometry>> group in geopass)
			{
				ImageData curtexture;

				// What texture to use?
				if(group.Key is UnknownImage)
					curtexture = General.Map.Data.UnknownTexture3D;
				else if(group.Key.IsImageLoaded && !group.Key.IsDisposed)
					curtexture = group.Key;
				else
					curtexture = General.Map.Data.Hourglass3D;

				// Create Direct3D texture if still needed
				if((curtexture.Texture == null) || curtexture.Texture.Disposed)
					curtexture.CreateTexture();

				// Apply texture
				if(!graphics.Shaders.Enabled) graphics.Device.SetTexture(0, curtexture.Texture);
				graphics.Shaders.World3D.Texture1 = curtexture.Texture;
				
				//mxd. Sort geometry by sector index
				group.Value.Sort((g1, g2) => g1.Sector.Sector.FixedIndex - g2.Sector.Sector.FixedIndex);

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
						//mxd. no Map means that sector was deleted recently, I suppose
						if (g.Sector.GeometryBuffer != null && g.Sector.Sector.Map != null) 
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

					if (sector != null) 
					{
						// Determine the shader pass we want to use for this object
						int wantedshaderpass = (((g == highlighted) && showhighlight) || (g.Selected && showselection)) ? highshaderpass : shaderpass;

						//mxd. Render fog?
						if( !(!General.Settings.GZDrawFog || fullbrightness || sector.Sector.Brightness > 247) )
							wantedshaderpass += 8;

						// Switch shader pass?
						if(currentshaderpass != wantedshaderpass)
						{
							graphics.Shaders.World3D.EndPass();
							graphics.Shaders.World3D.BeginPass(wantedshaderpass);
							currentshaderpass = wantedshaderpass;
						}
						
						// Set the colors to use
						if(!graphics.Shaders.Enabled)
						{
							graphics.Device.SetTexture(2, (g.Selected && showselection) ? selectionimage.Texture : null);
							graphics.Device.SetTexture(3, ((g == highlighted) && showhighlight) ? highlightimage.Texture : null);
						}
						else
						{
							//mxd. set variables for fog rendering
							if (wantedshaderpass > 7) 
							{
								graphics.Shaders.World3D.World = world;
								graphics.Shaders.World3D.LightColor = sector.Sector.FogColor;
								graphics.Shaders.World3D.CameraPosition = new Vector4(cameraposition.x, cameraposition.y, cameraposition.z, sector.FogDistance);
							}
							
							graphics.Shaders.World3D.SetHighlightColor(CalculateHighlightColor((g == highlighted) && showhighlight, (g.Selected && showselection)).ToArgb());
							graphics.Shaders.World3D.ApplySettings();
						}
						
						// Render!
						graphics.Device.DrawPrimitives(PrimitiveType.TriangleList, g.VertexOffset, g.Triangles);
					}
				}
			}

			// Get things for this pass
			if(thingspass.Count > 0)
			{
				// Texture addressing
				graphics.Device.SetSamplerState(0, SamplerState.AddressU, TextureAddress.Clamp);
				graphics.Device.SetSamplerState(0, SamplerState.AddressV, TextureAddress.Clamp);
				graphics.Device.SetSamplerState(0, SamplerState.AddressW, TextureAddress.Clamp);
				graphics.Device.SetRenderState(RenderState.CullMode, Cull.None); //mxd. Disable backside culling, because otherwise sprites with positive ScaleY and negative ScaleX will be facing away from the camera...

				// Render things collected
				foreach(KeyValuePair<ImageData, List<VisualThing>> group in thingspass)
				{
					if(group.Key is UnknownImage) continue;
					
					ImageData curtexture;
						
					// What texture to use?
					if(!group.Key.IsImageLoaded || group.Key.IsDisposed)
						curtexture = General.Map.Data.Hourglass3D;
					else 
						curtexture = group.Key;

					// Create Direct3D texture if still needed
					if((curtexture.Texture == null) || curtexture.Texture.Disposed)
						curtexture.CreateTexture();

					// Apply texture
					if(!graphics.Shaders.Enabled) graphics.Device.SetTexture(0, curtexture.Texture);
					graphics.Shaders.World3D.Texture1 = curtexture.Texture;

					// Render all things with this texture
					foreach(VisualThing t in group.Value)
					{
						//mxd
						if(t.Thing.IsModel && 
						   (General.Settings.GZDrawModelsMode == ModelRenderMode.ALL ||
						    General.Settings.GZDrawModelsMode == ModelRenderMode.ACTIVE_THINGS_FILTER ||
						    (General.Settings.GZDrawModelsMode == ModelRenderMode.SELECTION && t.Selected))) 
							continue;

						// Update buffer if needed
						t.Update();

						// Only do this sector when a vertexbuffer is created
						if(t.GeometryBuffer != null) 
						{
							// Determine the shader pass we want to use for this object
							int wantedshaderpass = (((t == highlighted) && showhighlight) || (t.Selected && showselection)) ? highshaderpass : shaderpass;

							//mxd. if fog is enagled, switch to shader, which calculates it
							if(General.Settings.GZDrawFog && !fullbrightness && t.Thing.Sector != null && (t.Thing.Sector.HasFogColor || t.Thing.Sector.Brightness < 248))
								wantedshaderpass += 8;

							//mxd. if current thing is light - set it's color to light color
							Color4 litcolor = new Color4();
							if(Array.IndexOf(GZBuilder.GZGeneral.GZ_LIGHTS, t.Thing.Type) != -1 && !fullbrightness) 
							{
								wantedshaderpass += 4; //render using one of passes, which uses World3D.VertexColor
								graphics.Shaders.World3D.VertexColor = t.LightColor;
								litcolor = t.LightColor;
							}
							//mxd. check if Thing is affected by dynamic lights and set color accordingly
							else if(General.Settings.GZDrawLightsMode != LightRenderMode.NONE && !fullbrightness && lightthings.Count > 0) 
							{
								litcolor = GetLitColorForThing(t);
								if(litcolor.ToArgb() != 0) 
								{
									wantedshaderpass += 4; //render using one of passes, which uses World3D.VertexColor
									graphics.Shaders.World3D.VertexColor = new Color4(t.VertexColor) + litcolor;
								}
							}

							// Switch shader pass?
							if(currentshaderpass != wantedshaderpass) 
							{
								graphics.Shaders.World3D.EndPass();
								graphics.Shaders.World3D.BeginPass(wantedshaderpass);
								currentshaderpass = wantedshaderpass;
							}

							// Set the colors to use
							if(!graphics.Shaders.Enabled) 
							{
								graphics.Device.SetTexture(2, (t.Selected && showselection) ? selectionimage.Texture : null);
								graphics.Device.SetTexture(3, ((t == highlighted) && showhighlight) ? highlightimage.Texture : null);
							} 
							else 
							{
								graphics.Shaders.World3D.SetHighlightColor(CalculateHighlightColor((t == highlighted) && showhighlight, (t.Selected && showselection)).ToArgb());
							}

							//mxd. Create the matrix for positioning 
							if(t.Info.RenderMode == Thing.SpriteRenderMode.NORMAL) // Apply billboarding?
							{
								if(t.Info.XYBillboard)
								{
									world = Matrix.Translation(0f, 0f, -t.LocalCenterZ) 
										* Matrix.RotationX(Angle2D.PI - General.Map.VisualCamera.AngleZ)
										* Matrix.Translation(0f, 0f, t.LocalCenterZ)
									    * billboard
									    * Matrix.Scaling(t.Thing.ScaleX, t.Thing.ScaleX, t.Thing.ScaleY)
									    * t.Position;
								}
								else
								{
									world = billboard
									        * Matrix.Scaling(t.Thing.ScaleX, t.Thing.ScaleX, t.Thing.ScaleY)
									        * t.Position;
								}
							}
							else
							{
								world = Matrix.Scaling(t.Thing.ScaleX, t.Thing.ScaleX, t.Thing.ScaleY)
								        * t.Position;
							}

							ApplyMatrices3D();

							//mxd. Set variables for fog rendering
							if(wantedshaderpass > 7) 
							{
								graphics.Shaders.World3D.World = world;
								graphics.Shaders.World3D.LightColor = t.Thing.Sector.FogColor;
								float fogdistance = (litcolor.ToArgb() != 0 ? VisualSector.MAXIMUM_FOG_DISTANCE : t.FogDistance);
								graphics.Shaders.World3D.CameraPosition = new Vector4(cameraposition.x, cameraposition.y, cameraposition.z, fogdistance);
							}

							graphics.Shaders.World3D.ApplySettings();

							// Apply buffer
							graphics.Device.SetStreamSource(0, t.GeometryBuffer, 0, WorldVertex.Stride);

							// Render!
							graphics.Device.DrawPrimitives(PrimitiveType.TriangleList, 0, t.Triangles);
						}
					}
				}

				// Texture addressing
				graphics.Device.SetSamplerState(0, SamplerState.AddressU, TextureAddress.Wrap);
				graphics.Device.SetSamplerState(0, SamplerState.AddressV, TextureAddress.Wrap);
				graphics.Device.SetSamplerState(0, SamplerState.AddressW, TextureAddress.Wrap);
				graphics.Device.SetRenderState(RenderState.CullMode, Cull.Counterclockwise); //mxd
			}

			// Done rendering with this shader
			graphics.Shaders.World3D.EndPass();
		}

		//mxd
		private void RenderTranslucentPass(List<VisualGeometry> geopass, List<VisualThing> thingspass)
		{
			// Anything to render?
			if(geopass.Count == 0 && thingspass.Count == 0) return;
			
			int currentshaderpass = shaderpass;
			int highshaderpass = shaderpass + 2;

			// Sort geometry by camera distance. First vertex of the BoundingBox is it's center
			if(General.Map.VisualCamera.Sector != null)
			{
				// If the camera is inside a sector, compare z coordinates
				translucentgeo.Sort(delegate(VisualGeometry vg1, VisualGeometry vg2)
				{
					float camdist1, camdist2;

					if((vg1.GeometryType == VisualGeometryType.FLOOR || vg1.GeometryType == VisualGeometryType.CEILING)
						&& General.Map.VisualCamera.Sector.Index == vg1.Sector.Sector.Index)
					{
						camdist1 = Math.Abs(General.Map.VisualCamera.Position.z - vg1.BoundingBox[0].z);
					}
					else
					{
						camdist1 = (General.Map.VisualCamera.Position - vg1.BoundingBox[0]).GetLengthSq();
					}

					if((vg2.GeometryType == VisualGeometryType.FLOOR || vg2.GeometryType == VisualGeometryType.CEILING)
						&& General.Map.VisualCamera.Sector.Index == vg2.Sector.Sector.Index)
					{
						camdist2 = Math.Abs(General.Map.VisualCamera.Position.z - vg2.BoundingBox[0].z);
					}
					else
					{
						camdist2 = (General.Map.VisualCamera.Position - vg2.BoundingBox[0]).GetLengthSq();
					}

					return (int)(camdist2 - camdist1);
				});
			}
			else
			{
				translucentgeo.Sort((vg1, vg2) => (int)((General.Map.VisualCamera.Position - vg2.BoundingBox[0]).GetLengthSq()
														- (General.Map.VisualCamera.Position - vg1.BoundingBox[0]).GetLengthSq()));
			}

			// Begin rendering with this shader
			graphics.Shaders.World3D.BeginPass(shaderpass);

			VisualSector sector = null;
			RenderPass currentpass = RenderPass.Solid;
			long curtexturename = 0;
			ImageData curtexture;

			// Go for all geometry
			foreach(VisualGeometry g in geopass)
			{
				// Change blend mode?
				if(g.RenderPass != currentpass)
				{
					switch(g.RenderPass)
					{
						case RenderPass.Additive:
							graphics.Device.SetRenderState(RenderState.DestinationBlend, Blend.One);
							break;

						case RenderPass.Alpha:
							graphics.Device.SetRenderState(RenderState.DestinationBlend, Blend.InverseSourceAlpha);
							break;
					}

					currentpass = g.RenderPass;
				}

				// Change texture?
				if(g.Texture.LongName != curtexturename)
				{
					// What texture to use?
					if(g.Texture is UnknownImage)
						curtexture = General.Map.Data.UnknownTexture3D;
					else if(g.Texture.IsImageLoaded && !g.Texture.IsDisposed)
						curtexture = g.Texture;
					else
						curtexture = General.Map.Data.Hourglass3D;

					// Create Direct3D texture if still needed
					if((curtexture.Texture == null) || curtexture.Texture.Disposed)
						curtexture.CreateTexture();

					// Apply texture
					if(!graphics.Shaders.Enabled) graphics.Device.SetTexture(0, curtexture.Texture);
					graphics.Shaders.World3D.Texture1 = curtexture.Texture;

					curtexturename = g.Texture.LongName;
				}

				// Changing sector?
				if(!object.ReferenceEquals(g.Sector, sector))
				{
					// Update the sector if needed
					if(g.Sector.NeedsUpdateGeo) g.Sector.Update();

					// Only do this sector when a vertexbuffer is created
					//mxd. no Map means that sector was deleted recently, I suppose
					if(g.Sector.GeometryBuffer != null && g.Sector.Sector.Map != null)
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
					// Determine the shader pass we want to use for this object
					int wantedshaderpass = (((g == highlighted) && showhighlight) || (g.Selected && showselection)) ? highshaderpass : shaderpass;

					//mxd. Render fog?
					if(!(!General.Settings.GZDrawFog || fullbrightness || sector.Sector.Brightness > 247))
						wantedshaderpass += 8;

					// Switch shader pass?
					if(currentshaderpass != wantedshaderpass)
					{
						graphics.Shaders.World3D.EndPass();
						graphics.Shaders.World3D.BeginPass(wantedshaderpass);
						currentshaderpass = wantedshaderpass;
					}

					// Set the colors to use
					if(!graphics.Shaders.Enabled)
					{
						graphics.Device.SetTexture(2, (g.Selected && showselection) ? selectionimage.Texture : null);
						graphics.Device.SetTexture(3, ((g == highlighted) && showhighlight) ? highlightimage.Texture : null);
					}
					else
					{
						//mxd. set variables for fog rendering
						if(wantedshaderpass > 7)
						{
							graphics.Shaders.World3D.World = world;
							graphics.Shaders.World3D.LightColor = sector.Sector.FogColor;
							graphics.Shaders.World3D.CameraPosition = new Vector4(cameraposition.x, cameraposition.y, cameraposition.z, sector.FogDistance);
						}

						graphics.Shaders.World3D.SetHighlightColor(CalculateHighlightColor((g == highlighted) && showhighlight, (g.Selected && showselection)).ToArgb());
						graphics.Shaders.World3D.ApplySettings();
					}

					// Render!
					graphics.Device.DrawPrimitives(PrimitiveType.TriangleList, g.VertexOffset, g.Triangles);
				}
			}

			//TODO: render things

			// Done rendering with this shader
			graphics.Shaders.World3D.EndPass();
		}

		//mxd. Dynamic lights pass!
		private void RenderLights(Dictionary<ImageData, List<VisualGeometry>> geometrytolit, List<VisualThing> lights)
		{
			// Anything to do?
			if(geometrytolit.Count == 0) return;
			
			graphics.Shaders.World3D.World = Matrix.Identity;
			graphics.Shaders.World3D.BeginPass(17);

			int i, count;
			Vector4 lpr;
			VisualSector sector = null;

			graphics.Device.SetRenderState(RenderState.SourceBlend, Blend.One);
			graphics.Device.SetRenderState(RenderState.DestinationBlend, Blend.BlendFactor);

			foreach(KeyValuePair<ImageData, List<VisualGeometry>> group in geometrytolit) 
			{
				if(group.Key.Texture == null) continue;
				graphics.Shaders.World3D.Texture1 = group.Key.Texture;

				foreach(VisualGeometry g in group.Value) 
				{
					// Changing sector?
					if(!object.ReferenceEquals(g.Sector, sector))
					{
						// Only do this sector when a vertexbuffer is created
						// mxd. no Map means that sector was deleted recently, I suppose
						if(g.Sector.GeometryBuffer != null && g.Sector.Sector.Map != null)
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

					if(sector == null) continue;

					//normal lights
					count = lightOffsets[0];
					if(lightOffsets[0] > 0) 
					{
						graphics.Device.SetRenderState(RenderState.BlendOperation, BlendOperation.Add);

						for (i = 0; i < count; i++) 
						{
							if (BoundingBoxesIntersect(g.BoundingBox, lights[i].BoundingBox)) 
							{
								lpr = new Vector4(lights[i].Center, lights[i].LightRadius);
								if (lpr.W == 0) continue;
								graphics.Shaders.World3D.LightColor = lights[i].LightColor;
								graphics.Shaders.World3D.LightPositionAndRadius = lpr;
								graphics.Shaders.World3D.ApplySettings();
								graphics.Device.DrawPrimitives(PrimitiveType.TriangleList, g.VertexOffset, g.Triangles);
							}
						}
					}

					//additive lights
					if(lightOffsets[1] > 0) 
					{
						count += lightOffsets[1];
						graphics.Device.SetRenderState(RenderState.BlendOperation, BlendOperation.Add);

						for (i = lightOffsets[0]; i < count; i++) 
						{
							if (BoundingBoxesIntersect(g.BoundingBox, lights[i].BoundingBox)) 
							{
								lpr = new Vector4(lights[i].Center, lights[i].LightRadius);
								if (lpr.W == 0) continue;
								graphics.Shaders.World3D.LightColor = lights[i].LightColor;
								graphics.Shaders.World3D.LightPositionAndRadius = lpr;
								graphics.Shaders.World3D.ApplySettings();
								graphics.Device.DrawPrimitives(PrimitiveType.TriangleList, g.VertexOffset, g.Triangles);
						   }
						}
					}

					//negative lights
					if(lightOffsets[2] > 0) 
					{
						count += lightOffsets[2];
						graphics.Device.SetRenderState(RenderState.BlendOperation, BlendOperation.ReverseSubtract);

						for (i = lightOffsets[0] + lightOffsets[1]; i < count; i++) 
						{
							if (BoundingBoxesIntersect(g.BoundingBox, lights[i].BoundingBox)) 
							{
								lpr = new Vector4(lights[i].Center, lights[i].LightRadius);
								if (lpr.W == 0) continue;
								Color4 lc = lights[i].LightColor;
								graphics.Shaders.World3D.LightColor = new Color4(lc.Alpha, (lc.Green + lc.Blue) / 2, (lc.Red + lc.Blue) / 2, (lc.Green + lc.Red) / 2);
								graphics.Shaders.World3D.LightPositionAndRadius = lpr;
								graphics.Shaders.World3D.ApplySettings();
								graphics.Device.DrawPrimitives(PrimitiveType.TriangleList, g.VertexOffset, g.Triangles);
							}
						}
					}
				}
			}

			graphics.Shaders.World3D.EndPass();
			graphics.Device.SetRenderState(RenderState.BlendOperation, BlendOperation.Add);
		}

		//mxd. render models
		private void RenderModels() 
		{
			int shaderpass = fullbrightness ? 1 : 4;
			int currentshaderpass = shaderpass;
			int highshaderpass = shaderpass + 2;

			// Begin rendering with this shader
			graphics.Shaders.World3D.BeginPass(currentshaderpass);

			foreach (KeyValuePair<ModelData, List<VisualThing>> group in modelthings) 
			{
				foreach (VisualThing t in group.Value) 
				{
					t.Update();
					
					Color4 vertexcolor = new Color4(t.VertexColor);
					vertexcolor.Alpha = 1.0f;
					
					//check if model is affected by dynamic lights and set color accordingly
					Color4 litcolor = new Color4();
					if (General.Settings.GZDrawLightsMode != LightRenderMode.NONE && !fullbrightness && lightthings.Count > 0) 
					{
						litcolor = GetLitColorForThing(t);
						graphics.Shaders.World3D.VertexColor = vertexcolor + litcolor;
					} 
					else 
					{
						graphics.Shaders.World3D.VertexColor = vertexcolor;
						litcolor = vertexcolor;
					}

					// Determine the shader pass we want to use for this object
					int wantedshaderpass = ((((t == highlighted) && showhighlight) || (t.Selected && showselection)) ? highshaderpass : shaderpass);

					//mxd. if fog is enagled, switch to shader, which calculates it
					if (General.Settings.GZDrawFog && !fullbrightness && t.Thing.Sector != null && (t.Thing.Sector.HasFogColor || t.Thing.Sector.Brightness < 248))
						wantedshaderpass += 8;

					// Switch shader pass?
					if (currentshaderpass != wantedshaderpass) 
					{
						graphics.Shaders.World3D.EndPass();
						graphics.Shaders.World3D.BeginPass(wantedshaderpass);
						currentshaderpass = wantedshaderpass;
					}

					// Set the colors to use
					if (!graphics.Shaders.Enabled) 
					{
						graphics.Device.SetTexture(2, (t.Selected && showselection) ? selectionimage.Texture : null);
						graphics.Device.SetTexture(3, ((t == highlighted) && showhighlight) ? highlightimage.Texture : null);
					} 
					else 
					{
						graphics.Shaders.World3D.SetHighlightColor(CalculateHighlightColor((t == highlighted) && showhighlight, (t.Selected && showselection)).ToArgb());
					}

					// Create the matrix for positioning / rotation
					float sx = t.Thing.ScaleX * t.Thing.ActorScale.Width;
					float sy = t.Thing.ScaleY * t.Thing.ActorScale.Height;

					Matrix modelscale = Matrix.Scaling(sx, sx, sy);
					Matrix modelrotation = Matrix.RotationY(-t.Thing.RollRad) * Matrix.RotationX(-t.Thing.PitchRad) * Matrix.RotationZ(t.Thing.Angle);

					world = General.Map.Data.ModeldefEntries[t.Thing.Type].Transform * modelscale * modelrotation * t.Position;
					ApplyMatrices3D();

					//mxd. set variables for fog rendering
					if (wantedshaderpass > 7) 
					{
						graphics.Shaders.World3D.World = world;
						graphics.Shaders.World3D.LightColor = t.Thing.Sector.FogColor;
						float fogdistance = (litcolor.ToArgb() != 0 ? VisualSector.MAXIMUM_FOG_DISTANCE : t.FogDistance);
						graphics.Shaders.World3D.CameraPosition = new Vector4(cameraposition.x, cameraposition.y, cameraposition.z, fogdistance);
					}

					for(int i = 0; i < group.Key.Model.Meshes.Count; i++) 
					{
						if (!graphics.Shaders.Enabled) graphics.Device.SetTexture(0, group.Key.Model.Textures[i]);
						graphics.Shaders.World3D.Texture1 = group.Key.Model.Textures[i];
						graphics.Shaders.World3D.ApplySettings();

						// Render!
						group.Key.Model.Meshes[i].DrawSubset(0);
					}
				}
			}
			graphics.Shaders.World3D.EndPass();
		}

		//mxd. This gets color from dynamic lights based on distance to thing. 
		//thing position must be in absolute cordinates 
		//(thing.Position.Z value is relative to floor of the sector the thing is in)
		private Color4 GetLitColorForThing(VisualThing t) 
		{
			Color4 litColor = new Color4();
			float radius, radiusSquared, distSquared, scaler;
			int sign;

			for(int i = 0; i < lightthings.Count; i++ ) 
			{
				//don't light self
				if(General.Map.Data.GldefsEntries.ContainsKey(t.Thing.Type) && General.Map.Data.GldefsEntries[t.Thing.Type].DontLightSelf && t.Thing.Index == lightthings[i].Thing.Index)
					continue;

				distSquared = Vector3.DistanceSquared(lightthings[i].Center, t.PositionV3);
				radius = lightthings[i].LightRadius;
				radiusSquared = radius * radius;
				if(distSquared < radiusSquared) 
				{
					sign = lightthings[i].LightRenderStyle == DynamicLightRenderStyle.NEGATIVE ? -1 : 1;
					scaler = 1 - distSquared / radiusSquared * lightthings[i].LightColor.Alpha;
					litColor.Red += lightthings[i].LightColor.Red * scaler * sign;
					litColor.Green += lightthings[i].LightColor.Green * scaler * sign;
					litColor.Blue += lightthings[i].LightColor.Blue * scaler * sign;
				}
			}
			return litColor;
		}

		// This calculates the highlight/selection color
		private Color4 CalculateHighlightColor(bool ishighlighted, bool isselected)
		{
			Color4 highlightcolor = isselected ? General.Colors.Selection.ToColorValue() : General.Colors.Highlight.ToColorValue();
			highlightcolor.Alpha = ishighlighted ? highlightglowinv : highlightglow;
			return highlightcolor;
		}
		
		// This finishes rendering
		public void Finish()
		{
			General.Plugins.OnPresentDisplayBegin();

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
			// Must have a texture and vertices
			if(g.Texture != null && g.Triangles > 0)
			{
				switch(g.RenderPass)
				{
					case RenderPass.Solid:
						if(!solidgeo.ContainsKey(g.Texture)) solidgeo.Add(g.Texture, new List<VisualGeometry>());
						solidgeo[g.Texture].Add(g);
						break;

					case RenderPass.Mask:
						if(!maskedgeo.ContainsKey(g.Texture)) maskedgeo.Add(g.Texture, new List<VisualGeometry>());
						maskedgeo[g.Texture].Add(g);
						break;

					case RenderPass.Additive: case RenderPass.Alpha:
						translucentgeo.Add(g);
						break;

					default:
						throw new NotImplementedException("Geometry rendering of " + g.RenderPass + " render pass is not implemented!");
				}
			}
		}

		// This collects a visual sector's geometry for rendering
		public void AddThingGeometry(VisualThing t)
		{
			//mxd. gater lights
			if(General.Settings.GZDrawLightsMode != LightRenderMode.NONE && !fullbrightness && t.LightType != DynamicLightType.NONE) 
			{
				t.UpdateLightRadius();
				if(t.LightRadius > 0)
				{
					if(Array.IndexOf(GZBuilder.GZGeneral.GZ_ANIMATED_LIGHT_TYPES, t.LightType) != -1)
						t.UpdateBoundingBox();
					lightthings.Add(t);
				}
			}

			//mxd. gather models
			if(t.Thing.IsModel && 
				(General.Settings.GZDrawModelsMode == ModelRenderMode.ALL ||
				 General.Settings.GZDrawModelsMode == ModelRenderMode.ACTIVE_THINGS_FILTER ||
				(General.Settings.GZDrawModelsMode == ModelRenderMode.SELECTION && t.Selected))) 
			{
				ModelData mde = General.Map.Data.ModeldefEntries[t.Thing.Type];
				if (!modelthings.ContainsKey(mde))
					modelthings.Add(mde, new List<VisualThing> { t });
				else
					modelthings[mde].Add(t);
			}

			//mxd. Add to the plain list
			allthings.Add(t);

			// Must have a texture!
			if(t.Texture != null) 
			{
				//mxd
				switch(t.RenderPass)
				{
					case RenderPass.Solid:
						if(!solidthings.ContainsKey(t.Texture)) solidthings.Add(t.Texture, new List<VisualThing>());
						solidthings[t.Texture].Add(t);
						break;

					case RenderPass.Mask:
						if(!maskedthings.ContainsKey(t.Texture)) maskedthings.Add(t.Texture, new List<VisualThing>());
						maskedthings[t.Texture].Add(t);
						break;

					case RenderPass.Additive: case RenderPass.Alpha:
						translucentthings.Add(t);
						break;

					default:
						throw new NotImplementedException("Thing rendering of " + t.RenderPass + " render pass is not implemented!");
				}
			}
		}

		//mxd
		public void AddVisualVertices(VisualVertex[] verts) 
		{
			visualvertices = verts;
		}

		//mxd
		private static bool BoundingBoxesIntersect(Vector3D[] bbox1, Vector3D[] bbox2) 
		{
			Vector3D dist = bbox1[0] - bbox2[0];

			Vector3D halfSize1 = bbox1[0] - bbox1[1];
			Vector3D halfSize2 = bbox2[0] - bbox2[1];

			return (halfSize1.x + halfSize2.x >= Math.Abs(dist.x) && halfSize1.y + halfSize2.y >= Math.Abs(dist.y) && halfSize1.z + halfSize2.z >= Math.Abs(dist.z));
		}

		// This renders the crosshair
		public void RenderCrosshair()
		{
			//mxd
			world = Matrix.Identity;
			ApplyMatrices3D();
			
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
			graphics.Device.DrawUserPrimitives(PrimitiveType.TriangleStrip, 0, 2, crosshairverts);
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
