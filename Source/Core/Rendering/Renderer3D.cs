
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

//mxd
using ColladaDotNet.Pipeline.MD3;
using CodeImp.DoomBuilder.GZBuilder.Data;

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
		//private VertexBuffer thingcage;
    private bool renderthingcages;
    //mxd
    private ThingBoundingBox bbox;
    private List<VisualThing> thingsWithLight;
    private int[] lightOffsets;
    private List<VisualGeometry> litGeometry;
    private Dictionary<ModelDefEntry, List<VisualThing>> thingsWithModel;
		
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
        
		#endregion

		#region ================== Properties

		public ProjectedFrustum2D Frustum2D { get { return frustum; } }
		public bool DrawThingCages { get { return renderthingcages; } set { renderthingcages = value; } }
		public bool FullBrightness { get { return fullbrightness; } set { fullbrightness = value; } }
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
			SetupThingCage();
			SetupTextures();
			renderthingcages = true;
			showselection = true;
			showhighlight = true;
			
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
                //mxd
				//if(thingcage != null) thingcage.Dispose();
                if (bbox != null) bbox.Dispose();

				if(selectionimage != null) selectionimage.Dispose();
				if(highlightimage != null) highlightimage.Dispose();
                
                //mxd
                //thingcage = null;
                bbox = null;

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
            //mxd
			//if(thingcage != null) thingcage.Dispose();
            if (bbox != null) bbox.Dispose();

			if(selectionimage != null) selectionimage.Dispose();
			if(highlightimage != null) highlightimage.Dispose();
			
            //mxd
            //thingcage = null;
            bbox = null;

			selectionimage = null;
			highlightimage = null;
		}
		
		// This is called resets when the device is reset
		// (when resized or display adapter was changed)
		public override void ReloadResource()
		{
			CreateMatrices2D();
			SetupThingCage();
			SetupTextures();
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
		
		// This sets up the thing cage
		/*private void SetupThingCage()
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
		}*/

        //mxd
        private void SetupThingCage() {
            bbox = new ThingBoundingBox(General.Map.Graphics.Device);
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
			// Create texture
            //mxd
			//if(General.Map.Data.ThingBox.Texture == null)
				//General.Map.Data.ThingBox.CreateTexture();
			
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
					double time = General.Clock.GetCurrentTime();
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
            thingsWithModel = new Dictionary<ModelDefEntry, List<VisualThing>>();

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

            //mxd. dbg
            //GZBuilder.GZGeneral.ClearTrace();
            //GZBuilder.GZGeneral.Trace("CamPos: " + General.Map.VisualCamera.Position.ToString());
            
            // Initial renderstates
			graphics.Device.SetRenderState(RenderState.CullMode, Cull.Counterclockwise);
			graphics.Device.SetRenderState(RenderState.ZEnable, true);
			graphics.Device.SetRenderState(RenderState.ZWriteEnable, true);
			graphics.Device.SetRenderState(RenderState.AlphaBlendEnable, false);
			graphics.Device.SetRenderState(RenderState.AlphaTestEnable, false);
			graphics.Device.SetRenderState(RenderState.TextureFactor, -1);
			graphics.Shaders.World3D.Begin();

            //mxd
            litGeometry = new List<VisualGeometry>();

			// SOLID PASS
			world = Matrix.Identity;
			ApplyMatrices3D();
			RenderSinglePass((int)RenderPass.Solid);

            //mxd
            litGeometry = new List<VisualGeometry>();

			// MASK PASS
            world = Matrix.Identity;
            ApplyMatrices3D();
            graphics.Device.SetRenderState(RenderState.AlphaTestEnable, true);
            RenderSinglePass((int)RenderPass.Mask);

            //mxd
            litGeometry = new List<VisualGeometry>();

            // ALPHA PASS
            world = Matrix.Identity;
            ApplyMatrices3D();
            graphics.Device.SetRenderState(RenderState.AlphaBlendEnable, true);
            graphics.Device.SetRenderState(RenderState.AlphaTestEnable, false);
            graphics.Device.SetRenderState(RenderState.ZWriteEnable, false);
            graphics.Device.SetRenderState(RenderState.SourceBlend, Blend.SourceAlpha);
            graphics.Device.SetRenderState(RenderState.DestinationBlend, Blend.InverseSourceAlpha);
            RenderSinglePass((int)RenderPass.Alpha);

            //mxd. Something in model rendering process screws Additive pass badly. so I moved it here to avoid this.
            graphics.Device.SetRenderState(RenderState.SourceBlend, Blend.One);
            graphics.Device.SetRenderState(RenderState.DestinationBlend, Blend.Zero);
            RenderModels();
            //graphics.Device.SetRenderState(RenderState.CullMode, Cull.Counterclockwise);
            graphics.Device.SetRenderState(RenderState.ZEnable, true);
            graphics.Device.SetRenderState(RenderState.ZWriteEnable, false);
            graphics.Device.SetRenderState(RenderState.AlphaBlendEnable, true);
            graphics.Device.SetRenderState(RenderState.AlphaTestEnable, false);
            graphics.Device.SetRenderState(RenderState.SourceBlend, Blend.SourceAlpha);
            graphics.Device.SetRenderState(RenderState.DestinationBlend, Blend.InverseSourceAlpha);
            graphics.Device.SetRenderState(RenderState.TextureFactor, -1);
            

            // THINGS
			if(renderthingcages) RenderThingCages();

            //mxd
            litGeometry = new List<VisualGeometry>();

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
                //add light to apropriate array. can't clear lights now, since it's Count is used to check if dynamic light rendering pass is needed and I don't want to add special variable for that...
                if (t.LightRenderStyle == (int)GZDoomLightRenderStyle.NORMAL)
                    lightOffsets[0]++;
                else if (t.LightRenderStyle == (int)GZDoomLightRenderStyle.ADDITIVE)
                    lightOffsets[1]++;
                else
                    lightOffsets[2]++;
            }
        }

        //mxd
        private int sortThingsByCameraDistance(VisualThing t1, VisualThing t2) {
            if (t1.CameraDistance3D == t2.CameraDistance3D) return 0;
            if (t1.CameraDistance3D > t2.CameraDistance3D) return 1;
            return -1;
        }
        //mxd
        private int sortThingsByLightRenderStyle(VisualThing t1, VisualThing t2) {
            if (t1.LightRenderStyle == t2.LightRenderStyle) return 0;
            if (t1.LightRenderStyle > t2.LightRenderStyle) return 1;
            return -1;
        }


		// This renders all thing cages
		/*private void RenderThingCages()
		{
			int currentshaderpass = shaderpass;
			int highshaderpass = shaderpass + 2;

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
				// Determine the shader pass we want to use for this object
				int wantedshaderpass = (((t == highlighted) && showhighlight) || (t.Selected && showselection)) ? highshaderpass : shaderpass;

				// Switch shader pass?
				if(currentshaderpass != wantedshaderpass)
				{
					graphics.Shaders.World3D.EndPass();
					graphics.Shaders.World3D.BeginPass(wantedshaderpass);
					currentshaderpass = wantedshaderpass;
				}

				// Setup matrix
				world = Matrix.Multiply(t.CageScales, t.Position);
				ApplyMatrices3D();

				// Setup color
				if(currentshaderpass == highshaderpass)
				{
					Color4 highcolor = CalculateHighlightColor((t == highlighted) && showhighlight, t.Selected && showselection);
					graphics.Shaders.World3D.SetHighlightColor(highcolor.ToArgb());
					highcolor.Alpha = 1.0f;
					graphics.Shaders.World3D.SetModulateColor(highcolor.ToArgb());
					graphics.Device.SetRenderState(RenderState.TextureFactor, highcolor.ToArgb());
				}
				else
				{
					graphics.Shaders.World3D.SetModulateColor(t.CageColor);
					graphics.Device.SetRenderState(RenderState.TextureFactor, t.CageColor);
				}

				// Render!
				graphics.Shaders.World3D.ApplySettings();
				graphics.Device.DrawPrimitives(PrimitiveType.TriangleList, 0, 12);
			}

			// Done
			graphics.Shaders.World3D.EndPass();
			graphics.Shaders.World3D.SetModulateColor(-1);
			graphics.Device.SetRenderState(RenderState.TextureFactor, -1);
		}*/

        //mxd. By adding model rendering section I somehow screwed DoomBuilder's ThingCages rendering process.
        //I never particularly liked old ThingCages, so I wrote this instead of fixing them.
        //It should render faster and it has fancy arrow! :)
        private void RenderThingCages() {
            graphics.Device.SetRenderState(RenderState.AlphaBlendEnable, true);
            graphics.Device.SetRenderState(RenderState.AlphaTestEnable, false);
            graphics.Device.SetRenderState(RenderState.ZWriteEnable, false);
            graphics.Device.SetRenderState(RenderState.SourceBlend, Blend.SourceAlpha);
            graphics.Device.SetRenderState(RenderState.DestinationBlend, Blend.SourceAlpha);

            //graphics.Device.SetRenderState(RenderState.AntialiasedLineEnable, General.Settings.QualityDisplay);

            graphics.Shaders.World3D.BeginPass(8);

            foreach (VisualThing t in thingsbydistance) {
                // Setup matrix
                world = Matrix.Multiply(t.CageScales, t.Position);
                ApplyMatrices3D();

                // Setup color
                Color4 thingColor = new Color4();
                if (t.Selected && showselection) {
                    thingColor = General.Colors.Selection3D.ToColorValue();
                } else {
                    //thingColor = Color4.Modulate(t.Thing.Color.ToColorValue(), new Color4(t.VertexColor));
                    thingColor = t.Thing.Color.ToColorValue();
                    if (t != highlighted) thingColor.Alpha = 0.6f;
                }
                graphics.Shaders.World3D.VertexColor = thingColor;

                //Render cage
                graphics.Shaders.World3D.ApplySettings();
                graphics.Device.SetStreamSource(0, bbox.cage, 0, WorldVertex.Stride);
                graphics.Device.DrawPrimitives(PrimitiveType.LineList, 0, 12);

                //and arrow
                float sx = t.CageScales.M11;
                Matrix arrowScaler = Matrix.Scaling(sx, sx, sx); //scale arrow evenly based on thing width\depth
                world = Matrix.Multiply(arrowScaler, t.Position * Matrix.Translation(0.0f, 0.0f, t.CageScales.M33 / 2));
                Matrix rot = Matrix.RotationZ(t.Thing.Angle - Angle2D.PI / 2);
                world = Matrix.Multiply(rot, world);
                ApplyMatrices3D();

                graphics.Shaders.World3D.ApplySettings();
                graphics.Device.SetStreamSource(0, bbox.arrow, 0, WorldVertex.Stride);
                graphics.Device.DrawPrimitives(PrimitiveType.LineList, 0, 5);
            }

            // Done
            graphics.Shaders.World3D.EndPass();
            graphics.Shaders.World3D.SetModulateColor(-1);
            graphics.Device.SetRenderState(RenderState.TextureFactor, -1);
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
						// Determine the shader pass we want to use for this object
						int wantedshaderpass = (((g == highlighted) && showhighlight) || (g.Selected && showselection)) ? highshaderpass : shaderpass;
						
                        //mxd
                        if (General.Settings.GZDrawLights && !fullbrightness && thingsWithLight.Count > 0)
                            litGeometry.Add(g);

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
                            graphics.Shaders.World3D.SetHighlightColor(CalculateHighlightColor((g == highlighted) && showhighlight, (g.Selected && showselection)).ToArgb());
							graphics.Shaders.World3D.ApplySettings();
						}
						
						// Render!
						graphics.Device.DrawPrimitives(PrimitiveType.TriangleList, g.VertexOffset, g.Triangles);
					}
				}
			}

            //mxd. Dynamic lights pass!
            if (General.Settings.GZDrawLights && !fullbrightness && thingsWithLight.Count > 0 && litGeometry.Count > 0) {
                graphics.Shaders.World3D.World = world;
                if (currentshaderpass != 9) {
                    currentshaderpass = 9;
                    graphics.Shaders.World3D.EndPass();
                    graphics.Shaders.World3D.BeginPass(currentshaderpass);
                }

                int i, count;
                Vector4 lpr;

                //dbg
                //int gcount = 0;

                foreach (VisualGeometry g in litGeometry) {
                    i = 0;
                    count = 0;

                    //dbg
                    //gcount++;

                    graphics.Device.SetStreamSource(0, g.Sector.GeometryBuffer, 0, WorldVertex.Stride);

                    //normal lights
                    count = lightOffsets[0];
                    if (lightOffsets[0] > 0) {
                        graphics.Device.SetRenderState(RenderState.SourceBlend, Blend.SourceAlpha);
                        graphics.Device.SetRenderState(RenderState.DestinationBlend, Blend.BlendFactor);
                        for (i = 0; i < count; i++) {
                            lpr = thingsWithLight[i].LightPositionAndRadius;
                            if (lpr.W == 0)
                                continue;
                            graphics.Shaders.World3D.LightColor = thingsWithLight[i].LightColor;
                            graphics.Shaders.World3D.LightPositionAndRadius = thingsWithLight[i].LightPositionAndRadius;
                            graphics.Shaders.World3D.ApplySettings();
                            graphics.Device.DrawPrimitives(PrimitiveType.TriangleList, g.VertexOffset, g.Triangles);
                        }
                    }

                    //additive lights.
                    if (lightOffsets[1] > 0) {
                        count += lightOffsets[1];
                        graphics.Device.SetRenderState(RenderState.SourceBlend, Blend.SourceAlpha);
                        graphics.Device.SetRenderState(RenderState.DestinationBlend, Blend.One);
                        for (i = lightOffsets[0]; i < count; i++) {
                            lpr = thingsWithLight[i].LightPositionAndRadius;
                            if (lpr.W == 0)
                                continue;
                            graphics.Shaders.World3D.LightColor = thingsWithLight[i].LightColor;
                            graphics.Shaders.World3D.LightPositionAndRadius = thingsWithLight[i].LightPositionAndRadius;
                            graphics.Shaders.World3D.ApplySettings();
                            graphics.Device.DrawPrimitives(PrimitiveType.TriangleList, g.VertexOffset, g.Triangles);
                        }
                    }

                    //negative lights
                    if (lightOffsets[2] > 0) {
                        count += lightOffsets[2];
                        graphics.Device.SetRenderState(RenderState.SourceBlend, Blend.InverseBlendFactor);
                        graphics.Device.SetRenderState(RenderState.DestinationBlend, Blend.InverseSourceColor);
                        for (i = lightOffsets[0] + lightOffsets[1]; i < count; i++) {
                            lpr = thingsWithLight[i].LightPositionAndRadius;
                            if (lpr.W == 0)
                                continue;
                            graphics.Shaders.World3D.LightColor = thingsWithLight[i].LightColor;
                            graphics.Shaders.World3D.LightPositionAndRadius = thingsWithLight[i].LightPositionAndRadius;
                            graphics.Shaders.World3D.ApplySettings();
                            graphics.Device.DrawPrimitives(PrimitiveType.TriangleList, g.VertexOffset, g.Triangles);
                        }
                    }
                }
                graphics.Device.SetRenderState(RenderState.AlphaBlendEnable, false); //disable AlphaBlending which is turned on in shader

                //dbg
                //GZBuilder.GZGeneral.Trace("Pass: " + pass + "; Lit " + gcount + " geometries; Lights: [" + lightOffsets[0] + ";" + lightOffsets[1] + ";" + lightOffsets[2] + "]" + Environment.NewLine + "Models count: " + thingsWithModel.Count + Environment.NewLine);
            }

			// Get things for this pass
			Dictionary<ImageData, List<VisualThing>> thingspass = things[pass];
			if(thingspass.Count > 0)
			{
				// Texture addressing
				graphics.Device.SetSamplerState(0, SamplerState.AddressU, TextureAddress.Clamp);
				graphics.Device.SetSamplerState(0, SamplerState.AddressV, TextureAddress.Clamp);
				graphics.Device.SetSamplerState(0, SamplerState.AddressW, TextureAddress.Clamp);

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
						if(!graphics.Shaders.Enabled) graphics.Device.SetTexture(0, curtexture.Texture);
						graphics.Shaders.World3D.Texture1 = curtexture.Texture;

						// Render all things with this texture
						foreach(VisualThing t in group.Value)
						{
                            //mxd
                            if (General.Settings.GZDrawModels && (!General.Settings.GZDrawSelectedModelsOnly || t.Selected) && t.Thing.IsModel)
                                continue;
                            
                            // Update buffer if needed
                                t.Update();

                                // Only do this sector when a vertexbuffer is created
                                if (t.GeometryBuffer != null) {
                                    // Determine the shader pass we want to use for this object
                                    int wantedshaderpass = (((t == highlighted) && showhighlight) || (t.Selected && showselection)) ? highshaderpass : shaderpass;

                                    //mxd. if current thing is light - set it's color to light color
                                    if (t.LightType != -1) {
                                        wantedshaderpass += 4; //render using one of passes, which uses World3D.VertexColor
                                        Color4 c = t.LightColor;
                                        if (t.LightRenderStyle == (int)GZDoomLightRenderStyle.NEGATIVE) {
                                            c.Red = 1.0f - t.LightColor.Blue;
                                            c.Green = 1.0f - t.LightColor.Green;
                                            c.Blue = 1.0f - t.LightColor.Red;
                                        } 
                                        graphics.Shaders.World3D.VertexColor = c;
                                    //mxd. check if Thing is affected by dynamic lights and set color accordingly
                                    }else if (General.Settings.GZDrawLights && !fullbrightness && thingsWithLight.Count > 0) {
                                        //Color4 litColor = getLitColor(new Vector3(t.Thing.Position.x, t.Thing.Position.y, t.Thing.Position.z + t.Thing.Sector.FloorHeight));
                                        Color4 litColor = getLitColor(t.PositionV3);
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
                                    world = t.Orientation;
                                    if (t.Billboard) world = Matrix.Multiply(world, billboard);
                                    world = Matrix.Multiply(world, t.Position);
                                    ApplyMatrices3D();
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
			}

			// Done rendering with this shader
			graphics.Shaders.World3D.EndPass();
		}

        //mxd. render models
        private void RenderModels() {
            int shaderpass = 4;
            if (fullbrightness) shaderpass++;
            int currentshaderpass = shaderpass;
            int highshaderpass = shaderpass + 2;

            // Begin rendering with this shader
            graphics.Shaders.World3D.BeginPass(currentshaderpass);

            foreach (KeyValuePair<ModelDefEntry, List<VisualThing>> group in thingsWithModel) {
                foreach (VisualThing t in group.Value) {
                    Color4 vertexColor = new Color4(t.VertexColor);
                    vertexColor.Alpha = 1.0f;
                    //check if model is affected by dynamic lights and set color accordingly
                    if (General.Settings.GZDrawLights && !fullbrightness && thingsWithLight.Count > 0) {
                        Color4 litColor = getLitColor(t.PositionV3);
                        graphics.Shaders.World3D.VertexColor = vertexColor + litColor;
                    } else {
                        graphics.Shaders.World3D.VertexColor = vertexColor;
                    }

                    for (int i = 0; i < group.Key.Model.NUM_MESHES; i++) {
                        if (!graphics.Shaders.Enabled) graphics.Device.SetTexture(0, group.Key.Model.Textures[i]);
                        graphics.Shaders.World3D.Texture1 = group.Key.Model.Textures[i];

                        // Determine the shader pass we want to use for this object
                        int wantedshaderpass = ((((t == highlighted) && showhighlight) || (t.Selected && showselection)) ? highshaderpass : shaderpass);
                        //dbg
                        //GZBuilder.GZGeneral.Trace("Rendering mesh with "+wantedshaderpass+" pass."+Environment.NewLine);

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
                        world = Matrix.Multiply(t.Orientation, Matrix.RotationZ(t.Thing.Angle));
                        world = Matrix.Multiply(world, t.Position);
                        ApplyMatrices3D();
                        graphics.Shaders.World3D.ApplySettings();

                        // Render!
                        group.Key.Model.Meshes[i].DrawSubset(0);
                    }
                }
            }
            graphics.Shaders.World3D.EndPass();
        }

        //mxd. This gets color from dynamic lights based on distance to thing. 
        //WARNING: thing position must be in absolute cordinates 
        //(thing.Position.Z value is actually relative to floor of the sector the thing is in)
        //so use visualThing.PositionV3 instead
        private Color4 getLitColor(Vector3 thingPosition) {
            Color4 litColor = new Color4();
            float distSquared, scaler;
            int radius, radiusSquared, sign;

            for (int i = 0; i < thingsWithLight.Count; i++ ) {
                distSquared = Vector3.DistanceSquared(thingsWithLight[i].Center, thingPosition);
                radius = thingsWithLight[i].LightRadius;
                radiusSquared = radius * radius;
                if (distSquared < radiusSquared) {
                    sign = thingsWithLight[i].LightRenderStyle == (int)GZDoomLightRenderStyle.NEGATIVE ? -1 : 1;
                    scaler = 1 - distSquared / radiusSquared;
                    litColor.Red += thingsWithLight[i].LightColor.Red * scaler * sign;
                    litColor.Green += thingsWithLight[i].LightColor.Green * scaler * sign;
                    litColor.Blue += thingsWithLight[i].LightColor.Blue * scaler * sign;
                }
            }
            return litColor;
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
            if (General.Settings.GZDrawLights && !fullbrightness && t.LightType != -1) {
                t.UpdateLightRadius();
                //dbg
                //GZBuilder.GZGeneral.TraceLine("LightRadius = "+t.LightRadius);
                if (t.LightRadius > 0) {
                    t.CalculateCameraDistance3D(D3DDevice.V3(cameraposition));
                    //t.CameraDistance3D is actually squared distance, hence (t.LightRadius * t.LightRadius)
                    if (t.CameraDistance3D < (t.LightRadius * t.LightRadius) || isThingOnScreen(t.BoundingBox)) { //always render light if camera is within it's radius 
                        //dbg
                        //GZBuilder.GZGeneral.Trace("Light is on screen.");
                        thingsWithLight.Add(t);
                    }
                }
            //mxd. gather models
            } else if (General.Settings.GZDrawModels && (!General.Settings.GZDrawSelectedModelsOnly || t.Selected) && t.Thing.IsModel) {
                ModelDefEntry mde = GZBuilder.GZGeneral.ModelDefEntries[t.Thing.Type];

                if (!isThingOnScreen(t.BoundingBox)) {
                    //dbg
                    //GZBuilder.GZGeneral.Trace("Model is not on screen");
                    return;
                }
                //dbg
                //GZBuilder.GZGeneral.Trace("Model is on screen");

                if (!thingsWithModel.ContainsKey(mde)) 
                    thingsWithModel.Add(mde, new List<VisualThing>());
                thingsWithModel[mde].Add(t);
            }else if (!isThingOnScreen(t.BoundingBox)) {
                return;
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
        private bool isThingOnScreen(Vector3[] bbox) {
            Vector3D camNormal = Vector3D.FromAngleXYZ(General.Map.VisualCamera.AngleXY, General.Map.VisualCamera.AngleZ);
            Vector3D thingNormal = D3DDevice.V3D(bbox[0]) - cameraposition; //bbox[0] is always thing center

            if (Vector3D.DotProduct(camNormal, thingNormal) < 0)
                return false; 

            int len = bbox.Length;
            Vector3 screenPos;
            for (int i = 0; i < len; i++) {
                //check visibility
                screenPos = Vector3.Project(bbox[i], 0, 0, 1, 1, PROJ_NEAR_PLANE, General.Settings.ViewDistance, worldviewproj);
                if (screenPos.X > 0 && screenPos.X < 1 && screenPos.Y > 0 && screenPos.Y < 1)
                    return true;
            }
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
		
		#endregion
	}
}
