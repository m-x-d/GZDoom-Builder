
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

		private const int RENDER_PASSES = 4;
		private const float PROJ_NEAR_PLANE = 1f;
		private const float CROSSHAIR_SCALE = 0.06f;
		private const float FOG_RANGE = 0.9f;
		private const float INVERTED_VERTICAL_STRETCH = 1.0f / 1.2f;
		
		#endregion

		#region ================== Variables

		// Matrices
		private Matrix projection;
		private Matrix view3d;
		private Matrix billboard;
		private Matrix worldviewproj;
		private Matrix view2d;
		private Matrix world;
		private float viewstretch; //mxd
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
		private List<VisualThing> thingsWithLight;
		private int[] lightOffsets;
		private Dictionary<Texture, List<VisualGeometry>> litGeometry;
		private Dictionary<ModelData, List<VisualThing>> thingsWithModel;
		
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

		//mxd. Visual vertices
		private VisualVertex[] visualvertices;
		
		#endregion

		#region ================== Properties

		public ProjectedFrustum2D Frustum2D { get { return frustum; } }
		public bool DrawThingCages { get { return renderthingcages; } set { renderthingcages = value; } }
		new public bool FullBrightness { get { return fullbrightness; } set { fullbrightness = value; } }
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
			float size = height * CROSSHAIR_SCALE;
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
			float screenheight = General.Map.Graphics.RenderTarget.ClientSize.Height * (General.Settings.GZStretchView ? INVERTED_VERTICAL_STRETCH : 1.0f); //mxd
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

			// Update the view stretch scaler (mxd)
			viewstretch = (General.Settings.GZStretchView ? INVERTED_VERTICAL_STRETCH : 1.0f);

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
				graphics.Shaders.World3D.SetModulateColor(-1);
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
					//mxd. WHY?!
					//highlightglowinv = -(float)Math.Sin(time / 100.0f) * 0.1f + 0.4f;
					highlightglowinv = -highlightglow + 0.8f;
				}
				else
				{
					highlightglow = 0.4f;
					highlightglowinv = 0.3f;
				}
				
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
			//mxd 
			thingsWithModel = new Dictionary<ModelData, List<VisualThing>>();
			litGeometry = new Dictionary<Texture, List<VisualGeometry>>();

			for(int i = 0; i < RENDER_PASSES; i++)
			{
				geometry[i] = new Dictionary<ImageData, BinaryHeap<VisualGeometry>>();
				things[i] = new Dictionary<ImageData, List<VisualThing>>();
			}

			//mxd
			if (General.Settings.GZDrawLights) {
				thingsWithLight = new List<VisualThing>();
			}
		}

		// This ends rendering world geometry
		public void FinishGeometry()
		{
			//mxd. sort lights
			if (General.Settings.GZDrawLights && !fullbrightness && thingsWithLight.Count > 0)
				updateLights();
			
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

			//mxd. Render models, without culling. The way it's done in GZDoom... Fixes Rendering of things like grass and models with negative Scale
			graphics.Device.SetRenderState(RenderState.AlphaTestEnable, true);
			graphics.Device.SetRenderState(RenderState.CullMode, Cull.None);
			RenderModels();
			graphics.Device.SetRenderState(RenderState.CullMode, Cull.Counterclockwise);

			// MASK PASS
			world = Matrix.Identity;
			ApplyMatrices3D();
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

			//mxd. Visual vertices
			RenderVertices();

			//mxd. LINKS
			if (General.Settings.GZShowEventLines) {
				//mxd. gather links
				List<Line3D> lines = LinksCollector.GetThingLinks(thingsbydistance);
				if(lines.Count > 0) {
					List<Line3D> normalLines = new List<Line3D>();
					List<Line3D> activatorLines = new List<Line3D>();
					
					foreach(Line3D l in lines){
						if(l.LineType == Line3DType.DEFAULT)
							normalLines.Add(l);
						else
							activatorLines.Add(l);
					}

					if(normalLines.Count > 0)
						renderArrows(normalLines, General.Colors.InfoLine.ToColorValue());
					if(activatorLines.Count > 0)
						renderArrows(activatorLines, General.Colors.Selection3D.ToColorValue());
				}
			}

			// ADDITIVE PASS
			world = Matrix.Identity;
			ApplyMatrices3D();
			graphics.Device.SetRenderState(RenderState.DestinationBlend, Blend.One);
			RenderSinglePass((int)RenderPass.Additive);

			//mxd. LIGHT PASS
			if( !(!General.Settings.GZDrawLights || fullbrightness || thingsWithLight.Count == 0 || litGeometry.Count == 0) ) {
				RenderLights(litGeometry, thingsWithLight);
			}
			
			// Remove references
			graphics.Shaders.World3D.Texture1 = null;
			
			// Done
			graphics.Shaders.World3D.End();
			geometry = null;
			visualvertices = null;//mxd
		}

		//mxd
		private void updateLights() {
			thingsWithLight.Sort(sortThingsByCameraDistance);
			if (thingsWithLight.Count > General.Settings.GZMaxDynamicLights) {
				List<VisualThing> tl = new List<VisualThing>();
				for (int i = 0; i < General.Settings.GZMaxDynamicLights; i++)
					tl.Add(thingsWithLight[i]);
				thingsWithLight = tl;
			}

			thingsWithLight.Sort(sortThingsByLightRenderStyle);
			lightOffsets = new int[3];
			foreach (VisualThing t in thingsWithLight) {
				//add light to apropriate array.
				switch(t.LightRenderStyle) {
					case DynamicLightRenderStyle.NORMAL:
					case DynamicLightRenderStyle.VAVOOM:
						lightOffsets[0]++;
						break;

					case DynamicLightRenderStyle.ADDITIVE:
						lightOffsets[1]++;
						break;

					default:
						lightOffsets[2]++;
						break;
				}
			}
		}

		//mxd
		private static int sortThingsByCameraDistance(VisualThing t1, VisualThing t2) 
		{
			if (t1.CameraDistance3D == t2.CameraDistance3D) return 0;
			if (t1.CameraDistance3D > t2.CameraDistance3D) return 1;
			return -1;
		}

		//mxd
		private static int sortThingsByLightRenderStyle(VisualThing t1, VisualThing t2) 
		{
			if (t1.LightRenderStyle == t2.LightRenderStyle) return 0;
			if (t1.LightRenderStyle > t2.LightRenderStyle) return 1;
			return -1;
		}

		//mxd.
		//I never particularly liked old ThingCages, so I wrote this instead.
		//It should render faster and it has fancy arrow! :)
		private void RenderThingCages() {
			graphics.Device.SetRenderState(RenderState.AlphaBlendEnable, true);
			graphics.Device.SetRenderState(RenderState.AlphaTestEnable, false);
			graphics.Device.SetRenderState(RenderState.ZWriteEnable, false);
			graphics.Device.SetRenderState(RenderState.SourceBlend, Blend.SourceAlpha);
			graphics.Device.SetRenderState(RenderState.DestinationBlend, Blend.SourceAlpha);

			graphics.Shaders.World3D.BeginPass(16);

			foreach (VisualThing t in thingsbydistance) {
				// Setup matrix
				world = Matrix.Multiply(t.CageScales, t.Position);
				ApplyMatrices3D();

				// Setup color
				Color4 thingColor;
				if (t.Selected && showselection) {
					thingColor = General.Colors.Selection3D.ToColorValue();
				} else {
					thingColor = t.Thing.Color.ToColorValue();
					if (t != highlighted) thingColor.Alpha = 0.6f;
				}
				graphics.Shaders.World3D.VertexColor = thingColor;

				//Render cage
				graphics.Shaders.World3D.ApplySettings();

				if(t.Sizeless) {
					graphics.Device.SetStreamSource(0, sizelessThingHandle.Shape, 0, WorldVertex.Stride);
					graphics.Device.DrawPrimitives(PrimitiveType.LineList, 0, 3);
				} else {
					graphics.Device.SetStreamSource(0, bbox.Cage, 0, WorldVertex.Stride);
					graphics.Device.DrawPrimitives(PrimitiveType.LineList, 0, 12);
				}

				//and arrow
				if (t.Thing.IsDirectional) {
					float sx = t.CageScales.M11;
					Matrix arrowScaler = Matrix.Scaling(sx, sx, sx); //scale arrow evenly based on thing width\depth
					if (t.Sizeless) {
						world = Matrix.Multiply(arrowScaler, t.Position);
					} else {
						world = Matrix.Multiply(arrowScaler, t.Position * Matrix.Translation(0.0f, 0.0f, t.CageScales.M33 / 2));
					}
					Matrix rot = Matrix.RotationZ(t.Thing.Angle - Angle2D.PI / 2);
					world = Matrix.Multiply(rot, world);
					ApplyMatrices3D();

					graphics.Shaders.World3D.ApplySettings();
					graphics.Device.SetStreamSource(0, bbox.Arrow, 0, WorldVertex.Stride);
					graphics.Device.DrawPrimitives(PrimitiveType.LineList, 0, 5);
				}
			}

			// Done
			graphics.Shaders.World3D.EndPass();
			graphics.Shaders.World3D.SetModulateColor(-1);
			graphics.Device.SetRenderState(RenderState.TextureFactor, -1);
		}

		//mxd
		private void RenderVertices() {
			if(visualvertices == null) return;

			graphics.Device.SetRenderState(RenderState.AlphaBlendEnable, true);
			graphics.Device.SetRenderState(RenderState.AlphaTestEnable, false);
			graphics.Device.SetRenderState(RenderState.ZWriteEnable, false);
			graphics.Device.SetRenderState(RenderState.SourceBlend, Blend.SourceAlpha);
			graphics.Device.SetRenderState(RenderState.DestinationBlend, Blend.SourceAlpha);

			graphics.Shaders.World3D.BeginPass(16);

			foreach(VisualVertex v in visualvertices) {
				world = v.Position;
				ApplyMatrices3D();

				// Setup color
				Color4 color;
				if(v.Selected && showselection) {
					color = General.Colors.Selection3D.ToColorValue();
				} else {
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
			graphics.Shaders.World3D.SetModulateColor(-1);
			graphics.Device.SetRenderState(RenderState.TextureFactor, -1);
		}

		//mxd
		private void renderArrows(List<Line3D> lines, Color4 color) {
			//create vertices
			WorldVertex[] verts = new WorldVertex[lines.Count * 6];
			float scaler = 20f;
			
			for (int i = 0; i < lines.Count; i++) {
				WorldVertex endPoint = new WorldVertex(lines[i].v2);
				float nz = lines[i].GetDelta().GetNormal().z * scaler;
				float angle = lines[i].GetAngle();
				verts[i * 6] = new WorldVertex(lines[i].v1);
				verts[i * 6 + 1] = endPoint;
				verts[i * 6 + 2] = endPoint;
				verts[i * 6 + 3] = new WorldVertex(new Vector3D(lines[i].v2.x - scaler * (float)Math.Sin(angle - 0.46f), lines[i].v2.y + scaler * (float)Math.Cos(angle - 0.46f), lines[i].v2.z - nz));
				verts[i * 6 + 4] = endPoint;
				verts[i * 6 + 5] = new WorldVertex(new Vector3D(lines[i].v2.x - scaler * (float)Math.Sin(angle + 0.46f), lines[i].v2.y + scaler * (float)Math.Cos(angle + 0.46f), lines[i].v2.z - nz));
			}

			VertexBuffer vb = new VertexBuffer(General.Map.Graphics.Device, WorldVertex.Stride * verts.Length, Usage.WriteOnly | Usage.Dynamic, VertexFormat.None, Pool.Default);
			DataStream s = vb.Lock(0, WorldVertex.Stride * verts.Length, LockFlags.Discard);
			s.WriteRange<WorldVertex>(verts);
			vb.Unlock();
			s.Dispose();
			
			//begin rendering
			graphics.Device.SetRenderState(RenderState.AlphaBlendEnable, true);
			graphics.Device.SetRenderState(RenderState.AlphaTestEnable, false);
			graphics.Device.SetRenderState(RenderState.ZWriteEnable, false);
			graphics.Device.SetRenderState(RenderState.SourceBlend, Blend.SourceAlpha);
			graphics.Device.SetRenderState(RenderState.DestinationBlend, Blend.SourceAlpha);

			graphics.Shaders.World3D.BeginPass(16);

			world = Matrix.Identity;
			ApplyMatrices3D();

			// Setup color
			graphics.Shaders.World3D.VertexColor = color;

			//render
			graphics.Shaders.World3D.ApplySettings();
			graphics.Device.SetStreamSource(0, vb, 0, WorldVertex.Stride);
			graphics.Device.DrawPrimitives(PrimitiveType.LineList, 0, lines.Count * 3);

			// Done
			graphics.Shaders.World3D.EndPass();
			graphics.Shaders.World3D.SetModulateColor(-1);
			graphics.Device.SetRenderState(RenderState.TextureFactor, -1);
			vb.Dispose();
		}

		// This performs a single render pass
		private void RenderSinglePass(int pass)
		{
			int currentshaderpass = shaderpass;
			int highshaderpass = shaderpass + 2;
			
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
				if(!graphics.Shaders.Enabled) graphics.Device.SetTexture(0, curtexture.Texture);
				graphics.Shaders.World3D.Texture1 = curtexture.Texture;
				
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

						//mxd
						if( !(!General.Settings.GZDrawFog || fullbrightness || sector.Sector.Brightness > 247) )
							wantedshaderpass += 8;

						//mxd. Seems that lines rendered with RenderPass.Alpha or RenderPass.Additive aren't affected by dynamic lights in GZDoom
						if ( !(g.RenderPass == RenderPass.Alpha || g.RenderPass == RenderPass.Additive || !General.Settings.GZDrawLights || fullbrightness || thingsWithLight.Count == 0) ) {
							if (curtexture.Texture != null) {
								if (!litGeometry.ContainsKey(curtexture.Texture))
									litGeometry[curtexture.Texture] = new List<VisualGeometry>();
								litGeometry[curtexture.Texture].Add(g);
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
							graphics.Device.SetTexture(2, (g.Selected && showselection) ? selectionimage.Texture : null);
							graphics.Device.SetTexture(3, ((g == highlighted) && showhighlight) ? highlightimage.Texture : null);
						}
						else
						{
							//mxd. set variables for fog rendering
							if (wantedshaderpass > 7) {
								graphics.Shaders.World3D.World = world;
								graphics.Shaders.World3D.LightColor = sector.Sector.FogColor;
								graphics.Shaders.World3D.CameraPosition = new Vector4(cameraposition.x, cameraposition.y, cameraposition.z, getFogEnd(sector.Sector));
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
			Dictionary<ImageData, List<VisualThing>> thingspass = things[pass];
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
					if(!(group.Key is UnknownImage))
					{
						ImageData curtexture;
						
						// What texture to use?
						if((group.Key == null) || !group.Key.IsImageLoaded || group.Key.IsDisposed)
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
							if (t.Thing.IsModel && General.Settings.GZDrawModels && (!General.Settings.GZDrawSelectedModelsOnly || t.Selected))
								continue;

							// Update buffer if needed
							t.Update();

							// Only do this sector when a vertexbuffer is created
							if (t.GeometryBuffer != null) {
								// Determine the shader pass we want to use for this object
								int wantedshaderpass = (((t == highlighted) && showhighlight) || (t.Selected && showselection)) ? highshaderpass : shaderpass;

								//mxd. if fog is enagled, switch to shader, which calculates it
								if(General.Settings.GZDrawFog && !fullbrightness && t.Thing.Sector != null && (t.Thing.Sector.HasFogColor || t.Thing.Sector.Brightness < 248))
									wantedshaderpass += 8;

								//mxd. if current thing is light - set it's color to light color
								if (Array.IndexOf(GZBuilder.GZGeneral.GZ_LIGHTS, t.Thing.Type) != -1 && !fullbrightness) {
									wantedshaderpass += 4; //render using one of passes, which uses World3D.VertexColor
									graphics.Shaders.World3D.VertexColor = t.LightColor;
									//mxd. check if Thing is affected by dynamic lights and set color accordingly
								} else if (General.Settings.GZDrawLights && !fullbrightness && thingsWithLight.Count > 0) {
									Color4 litColor = getLitColorForThing(t);
									if (litColor.ToArgb() != 0) {
										wantedshaderpass += 4; //render using one of passes, which uses World3D.VertexColor
										graphics.Shaders.World3D.VertexColor = new Color4(t.VertexColor) + litColor;
									}
								}

								// Switch shader pass?
								if (currentshaderpass != wantedshaderpass) {
									graphics.Shaders.World3D.EndPass();
									graphics.Shaders.World3D.BeginPass(wantedshaderpass);
									currentshaderpass = wantedshaderpass;
								}

								// Set the colors to use
								if (!graphics.Shaders.Enabled) {
									graphics.Device.SetTexture(2, (t.Selected && showselection) ? selectionimage.Texture : null);
									graphics.Device.SetTexture(3, ((t == highlighted) && showhighlight) ? highlightimage.Texture : null);
								} else {
									graphics.Shaders.World3D.SetHighlightColor(CalculateHighlightColor((t == highlighted) && showhighlight, (t.Selected && showselection)).ToArgb());
								}

								// Create the matrix for positioning / rotation
								world = billboard
										* Matrix.Scaling(t.Thing.ScaleX, t.Thing.ScaleX, t.Thing.ScaleY) 
										* t.Position; //mxd

								ApplyMatrices3D();

								//mxd. set variables for fog rendering
								if (wantedshaderpass > 7) {
									graphics.Shaders.World3D.World = world;

									graphics.Shaders.World3D.LightColor = t.Thing.Sector.FogColor;
									graphics.Shaders.World3D.CameraPosition = new Vector4(cameraposition.x, cameraposition.y, cameraposition.z, getFogEnd(t.Thing.Sector));
								}

								graphics.Shaders.World3D.ApplySettings();

								// Apply buffer
								graphics.Device.SetStreamSource(0, t.GeometryBuffer, 0, WorldVertex.Stride);

								// Render!
								graphics.Device.DrawPrimitives(PrimitiveType.TriangleList, 0, t.Triangles);
							}
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

		//mxd. Dynamic lights pass!
		private void RenderLights(Dictionary<Texture, List<VisualGeometry>> geometry_to_lit, List<VisualThing> lights) {
			graphics.Shaders.World3D.World = Matrix.Identity;
			graphics.Shaders.World3D.BeginPass(17);

			int i, count;
			Vector4 lpr;

			graphics.Device.SetRenderState(RenderState.SourceBlend, Blend.One);
			graphics.Device.SetRenderState(RenderState.DestinationBlend, Blend.BlendFactor);

			foreach (KeyValuePair<Texture, List<VisualGeometry>> group in geometry_to_lit) {
				graphics.Shaders.World3D.Texture1 = group.Key;

				foreach (VisualGeometry g in group.Value) {
					graphics.Device.SetStreamSource(0, g.Sector.GeometryBuffer, 0, WorldVertex.Stride);

					//normal lights
					count = lightOffsets[0];
					if (lightOffsets[0] > 0) {
						graphics.Device.SetRenderState(RenderState.BlendOperation, BlendOperation.Add);

						for (i = 0; i < count; i++) {
							if (checkBBoxIntersection(g.BoundingBox, lights[i].BoundingBox)) {
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
					if (lightOffsets[1] > 0) {
						count += lightOffsets[1];
						graphics.Device.SetRenderState(RenderState.BlendOperation, BlendOperation.Add);

						for (i = lightOffsets[0]; i < count; i++) {
							if (checkBBoxIntersection(g.BoundingBox, lights[i].BoundingBox)) {
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
					if (lightOffsets[2] > 0) {
						count += lightOffsets[2];
						graphics.Device.SetRenderState(RenderState.BlendOperation, BlendOperation.ReverseSubtract);

						for (i = lightOffsets[0] + lightOffsets[1]; i < count; i++) {
							if (checkBBoxIntersection(g.BoundingBox, lights[i].BoundingBox)) {
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
		private void RenderModels() {
			int shaderpass = fullbrightness ? 1 : 4;
			int currentshaderpass = shaderpass;
			int highshaderpass = shaderpass + 2;

			// Begin rendering with this shader
			graphics.Shaders.World3D.BeginPass(currentshaderpass);

			foreach (KeyValuePair<ModelData, List<VisualThing>> group in thingsWithModel) {
				foreach (VisualThing t in group.Value) {
					t.Update();
					
					Color4 vertexColor = new Color4(t.VertexColor);
					vertexColor.Alpha = 1.0f;
					//check if model is affected by dynamic lights and set color accordingly
					if (General.Settings.GZDrawLights && !fullbrightness && thingsWithLight.Count > 0) {
						Color4 litColor = getLitColorForThing(t);
						graphics.Shaders.World3D.VertexColor = vertexColor + litColor;
					} else {
						graphics.Shaders.World3D.VertexColor = vertexColor;
					}

					// Determine the shader pass we want to use for this object
					int wantedshaderpass = ((((t == highlighted) && showhighlight) || (t.Selected && showselection)) ? highshaderpass : shaderpass);

					//mxd. if fog is enagled, switch to shader, which calculates it
					if (General.Settings.GZDrawFog && !fullbrightness && t.Thing.Sector != null && (t.Thing.Sector.HasFogColor || t.Thing.Sector.Brightness < 248))
						wantedshaderpass += 8;

					// Switch shader pass?
					if (currentshaderpass != wantedshaderpass) {
						graphics.Shaders.World3D.EndPass();
						graphics.Shaders.World3D.BeginPass(wantedshaderpass);
						currentshaderpass = wantedshaderpass;
					}

					// Set the colors to use
					if (!graphics.Shaders.Enabled) {
						graphics.Device.SetTexture(2, (t.Selected && showselection) ? selectionimage.Texture : null);
						graphics.Device.SetTexture(3, ((t == highlighted) && showhighlight) ? highlightimage.Texture : null);
					} else {
						graphics.Shaders.World3D.SetHighlightColor(CalculateHighlightColor((t == highlighted) && showhighlight, (t.Selected && showselection)).ToArgb());
					}

					// Create the matrix for positioning / rotation
					float sx = t.Thing.ScaleX * t.Thing.ActorScale.Width;
					float sy = t.Thing.ScaleY * t.Thing.ActorScale.Height * viewstretch;
					Matrix rotation = Matrix.RotationY(-(t.Thing.RollRad - General.Map.Data.ModeldefEntries[t.Thing.Type].RollOffset))
						* Matrix.RotationX(-(t.Thing.PitchRad + General.Map.Data.ModeldefEntries[t.Thing.Type].PitchOffset))
						* Matrix.RotationZ(t.Thing.Angle);

					world = Matrix.Scaling(sx, sx, sy) * rotation * t.Position;
					ApplyMatrices3D();

					//mxd. set variables for fog rendering
					if (wantedshaderpass > 7) {
						graphics.Shaders.World3D.World = world;

						graphics.Shaders.World3D.LightColor = t.Thing.Sector.FogColor;
						graphics.Shaders.World3D.CameraPosition = new Vector4(cameraposition.x, cameraposition.y, cameraposition.z, getFogEnd(t.Thing.Sector));
					}

					for(int i = 0; i < group.Key.Model.Meshes.Count; i++) {
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
		private Color4 getLitColorForThing(VisualThing t) {
			Color4 litColor = new Color4();
			float radius, radiusSquared, distSquared, scaler;
			int sign;

			for (int i = 0; i < thingsWithLight.Count; i++ ) {
				//don't light self
				if (General.Map.Data.GldefsEntries.ContainsKey(t.Thing.Type) && General.Map.Data.GldefsEntries[t.Thing.Type].DontLightSelf && t.Thing.Index == thingsWithLight[i].Thing.Index) {
					continue;
				}

				distSquared = Vector3.DistanceSquared(thingsWithLight[i].Center, t.PositionV3);
				radius = thingsWithLight[i].LightRadius;
				radiusSquared = radius * radius;
				if (distSquared < radiusSquared) {
					sign = thingsWithLight[i].LightRenderStyle == DynamicLightRenderStyle.NEGATIVE ? -1 : 1;
					scaler = 1 - distSquared / radiusSquared * thingsWithLight[i].LightColor.Alpha;
					litColor.Red += thingsWithLight[i].LightColor.Red * scaler * sign;
					litColor.Green += thingsWithLight[i].LightColor.Green * scaler * sign;
					litColor.Blue += thingsWithLight[i].LightColor.Blue * scaler * sign;
				}
			}
			return litColor;
		}

		//mxd. This returns distance, at which fog color completely replaces texture color for given sector
		private static float getFogEnd(Sector s) 
		{
			float brightness = Math.Max(30, s.Brightness);
			
			if (s.HasFogColor) {
				if(s.UsesOutsideFog && General.Map.Data.MapInfo.OutsideFogDensity > 0)
					return General.Map.Data.MapInfo.OutsideFogDensity;
				if(!s.UsesOutsideFog && General.Map.Data.MapInfo.FogDensity > 0)
					return General.Map.Data.MapInfo.FogDensity;
				return brightness * 11.0f;
			}

			return (float)Math.Pow(2.0f, brightness / 11.0f);
		}

		// This calculates the highlight/selection color
		public Color4 CalculateHighlightColor(bool ishighlighted, bool isselected)
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
			if((g.Texture != null) && (g.Triangles > 0))
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
			//mxd. gater lights
			if (General.Settings.GZDrawLights && !fullbrightness && t.LightType != DynamicLightType.NONE) {
				t.UpdateLightRadius();

				if (t.LightRadius > 0) {
					t.CalculateCameraDistance3D(D3DDevice.V3(cameraposition));
					//t.CameraDistance3D is actually squared distance, hence (t.LightRadius * t.LightRadius)
					if (t.CameraDistance3D < (t.LightRadius * t.LightRadius) || isThingOnScreen(t.BoundingBox)) { //always render light if camera is within it's radius 
						if (Array.IndexOf(GZBuilder.GZGeneral.GZ_ANIMATED_LIGHT_TYPES, t.LightType) != -1)
							t.UpdateBoundingBox();
						thingsWithLight.Add(t);
					}
				}
			}

			if (!isThingOnScreen(t.BoundingBox)) return;

			//mxd. gather models
			if(t.Thing.IsModel && General.Settings.GZDrawModels && (!General.Settings.GZDrawSelectedModelsOnly || t.Selected)) {
				ModelData mde = General.Map.Data.ModeldefEntries[t.Thing.Type];
				if (!thingsWithModel.ContainsKey(mde))
					thingsWithModel.Add(mde, new List<VisualThing>() { t });
				else
					thingsWithModel[mde].Add(t);
			}
			
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

		//mxd
		public void AddVisualVertices(VisualVertex[] verts) {
			visualvertices = verts;
		}

		//mxd
		private bool isThingOnScreen(Vector3[] bbox) {
			Vector3D camNormal = Vector3D.FromAngleXYZ(General.Map.VisualCamera.AngleXY, General.Map.VisualCamera.AngleZ);
			Vector3D thingNormal = D3DDevice.V3D(bbox[0]) - cameraposition; //bbox[0] is always thing center

			if (Vector3D.DotProduct(camNormal, thingNormal) < 0) return false; //behind camera plane

			int len = bbox.Length;
			Vector3 screenPos;
			int behindCount = 0;
			int leftCount = 0;
			int rightCount = 0;
			int topCount = 0;
			int bottomCount = 0;

			for (int i = 0; i < len; i++) {
				//check visibility
				screenPos = Vector3.Project(bbox[i], 0, 0, 1, 1, PROJ_NEAR_PLANE, General.Settings.ViewDistance, worldviewproj);

				if (screenPos.X > 0 && screenPos.X < 1 && screenPos.Y > 0 && screenPos.Y < 1)
					return true;

				if (screenPos.Z < 0)
					behindCount++;

				if (screenPos.X < 0)
					leftCount++;
				else if (screenPos.X > 1)
					rightCount++;
				if (screenPos.Y < 0)
					topCount++;
				else if (screenPos.Y > 1)
					bottomCount++;
			}

			bool notOnScreen = (behindCount == len || leftCount == len || rightCount == len || topCount == len || bottomCount == len);
			return !notOnScreen;
		}

		//mxd
		private static bool checkBBoxIntersection(Vector3[] bbox1, Vector3[] bbox2) {
			Vector3 dist = bbox1[0] - bbox2[0];

			Vector3 halfSize1 = bbox1[0] - bbox1[1];
			Vector3 halfSize2 = bbox2[0] - bbox2[1];

			if (halfSize1.X + halfSize2.X >= Math.Abs(dist.X) && halfSize1.Y + halfSize2.Y >= Math.Abs(dist.Y) && halfSize1.Z + halfSize2.Z >= Math.Abs(dist.Z))
				return true;
			return false;
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

		//mxd. dbg
		//private int lastTick, lastFrameRate, frameRate;

		/*private int calculateFrameRate() {
			if (System.Environment.TickCount - lastTick >= 1000) {
				lastFrameRate = frameRate;
				frameRate = 0;
				lastTick = System.Environment.TickCount;
			}
			frameRate++;
			return lastFrameRate;
		}*/
		
		#endregion
	}
}
