
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
using SlimDX.Direct3D9;
using SlimDX;
using CodeImp.DoomBuilder.Geometry;
using System.Drawing.Imaging;
using CodeImp.DoomBuilder.Data;
using CodeImp.DoomBuilder.Editing;
using CodeImp.DoomBuilder.IO;
using CodeImp.DoomBuilder.Rendering;

//mxd
using CodeImp.DoomBuilder.GZBuilder.Data;

#endregion

namespace CodeImp.DoomBuilder.VisualModes
{
	public abstract class VisualThing : IVisualPickable, ID3DResource, IComparable<VisualThing>
	{
		#region ================== Constants
		
		#endregion
		
		#region ================== Variables
		
		// Thing
		private Thing thing;
		
		// Texture
		private ImageData texture;
		
		// Geometry
		private WorldVertex[] vertices;
		private VertexBuffer geobuffer;
		private bool updategeo;
		private int triangles;
		
		// Rendering
		private int renderpass;
		private Matrix orientation;
		private Matrix position;
		private Matrix cagescales;
		private bool billboard;
		private Vector2D pos2d;
		private float cameradistance;
		private int cagecolor;

		// Selected?
		protected bool selected;

		// Disposing
		private bool isdisposed = false;

        //mxd
        private int cameraDistance3D;
        private int thingHeight;

        //mxd. light properties
        private int lightType;
        private int lightRenderStyle;
        private Color4 lightColor;
        private int lightRadius; //current radius. used in light animation
        private int lightRadiusMin;
        private int lightRadiusMax;
        private Vector3 position_v3;
        private float lightDelta; //used in light animation
        private Vector3[] boundingBox;

        //mxd. model
        private bool checkedIfModel;
		
		#endregion
		
		#region ================== Properties
		
		internal VertexBuffer GeometryBuffer { get { return geobuffer; } }
		internal bool NeedsUpdateGeo { get { return updategeo; } }
		internal int Triangles { get { return triangles; } }
		internal int RenderPassInt { get { return renderpass; } }
		internal Matrix Orientation { get { return orientation; } }
		internal Matrix Position { get { return position; } }
		internal Matrix CageScales { get { return cagescales; } }
		internal int CageColor { get { return cagecolor; } }
        
        //mxd
        internal int VertexColor { get { return vertices.Length > 0 ? vertices[0].c : 0;} }
        public int CameraDistance3D { get { return cameraDistance3D; } }
        //public int HalfHeight { get { return thingHalfHeight; } }
        public Vector3 Center { get { return new Vector3(position_v3.X, position_v3.Y, position_v3.Z + thingHeight / 2); } }
        public Vector3 PositionV3 { get { return position_v3; } }
        public Vector3[] BoundingBox { get { return boundingBox; } }
        //mxd. light properties
        public int LightType { get { return lightType; } }
        public int LightRadius { get { return lightRadius; } }
        public int LightRenderStyle { get { return lightRenderStyle; } }
        public Color4 LightColor { get { return lightColor; } }
        public Vector4 LightPositionAndRadius { get { return new Vector4(Center, lightRadius);} }

		/// <summary>
		/// Set to True to use billboarding for this thing. When using billboarding,
		/// the geometry will be rotated on the XY plane to face the camera.
		/// </summary>
		public bool Billboard { get { return billboard; } set { billboard = value; } }

		/// <summary>
		/// Returns the Thing that this VisualThing is created for.
		/// </summary>
		public Thing Thing { get { return thing; } }

		/// <summary>
		/// Render pass in which this geometry must be rendered. Default is Solid.
		/// </summary>
		public RenderPass RenderPass { get { return (RenderPass)renderpass; } set { renderpass = (int)value; } }
		
		/// <summary>
		/// Image to use as texture on the geometry.
		/// </summary>
		public ImageData Texture { get { return texture; } set { texture = value; } }

		/// <summary>
		/// Disposed or not?
		/// </summary>
		public bool IsDisposed { get { return isdisposed; } }

		/// <summary>
		/// Selected or not? This is only used by the core to determine what color to draw it with.
		/// </summary>
		public bool Selected { get { return selected; } set { selected = value; } }
		
		#endregion
		
		#region ================== Constructor / Destructor
		
		// Constructor
		public VisualThing(Thing t)
		{
			// Initialize
			this.thing = t;
			this.renderpass = (int)RenderPass.Mask;
			this.billboard = true;
			this.orientation = Matrix.Identity;
			this.position = Matrix.Identity;
			this.cagescales = Matrix.Identity;

            //mxd
            lightType = -1;
            lightRenderStyle = -1;
            lightRadiusMin = -1;
            lightRadiusMax = -1;
            lightColor = new Color4();
            boundingBox = new Vector3[9];
			
			// Register as resource
			General.Map.Graphics.RegisterResource(this);
		}

		// Disposer
		public virtual void Dispose()
		{
			// Not already disposed?
			if(!isdisposed)
			{
				// Clean up
				if(geobuffer != null) geobuffer.Dispose();
				geobuffer = null;

				// Unregister resource
				General.Map.Graphics.UnregisterResource(this);

				// Done
				isdisposed = true;
			}
		}
		
		#endregion
		
		#region ================== Methods
		
		// This sets the distance from the camera
		internal void CalculateCameraDistance(Vector2D campos)
		{
			cameradistance = Vector2D.DistanceSq(pos2d, campos);
		}

        //mxd
        internal void CalculateCameraDistance3D(Vector3 campos) {
            cameraDistance3D = (int)Vector3.DistanceSquared(D3DDevice.V3(thing.Position), campos);
        }
		
		// This is called before a device is reset
		// (when resized or display adapter was changed)
		public void UnloadResource()
		{
			// Trash geometry buffer
			if(geobuffer != null) geobuffer.Dispose();
			geobuffer = null;
			updategeo = true;
            checkedIfModel = false;
		}
		
		// This is called resets when the device is reset
		// (when resized or display adapter was changed)
		public void ReloadResource()
		{
			// Make new geometry
			//Update();
		}

		/// <summary>
		/// Sets the size of the cage around the thing geometry.
		/// </summary>
		public void SetCageSize(float radius, float height)
		{
			cagescales = Matrix.Scaling(radius, radius, height);
            //mxd
            thingHeight = (int)height;
		}

		/// <summary>
		/// Sets the color of the cage around the thing geometry.
		/// </summary>
		public void SetCageColor(PixelColor color)
		{
			cagecolor = color.ToInt();
		}

		/// <summary>
		/// This sets the position to use for the thing geometry.
		/// </summary>
		public void SetPosition(Vector3D pos)
		{
			pos2d = new Vector2D(pos);
            //mxd
            position_v3 = D3DDevice.V3(pos);
            position = Matrix.Translation(position_v3);

            //mxd. update bounding box
            if (thing.IsModel) {
                updateBoundingBoxForModel();
            } else if (lightType != -1 && lightRadius > thing.Size) {
                updateBoundingBox(lightRadius, lightRadius);
            } else {
                updateBoundingBox((int)thing.Size, thingHeight);
            }
		}

		/// <summary>
		/// This sets the orientation to use for the thing geometry. When using this, you may want to turn off billboarding.
		/// </summary>
		public void SetOrientation(Vector3D angles)
		{
			orientation = Matrix.RotationYawPitchRoll(angles.z, angles.y, angles.x);
		}
		
		// This sets the vertices for the thing sprite
		protected void SetVertices(ICollection<WorldVertex> verts)
		{
			// Copy vertices
			vertices = new WorldVertex[verts.Count];
			verts.CopyTo(vertices, 0);
			triangles = vertices.Length / 3;
			updategeo = true;
		}
		
		// This updates the visual thing
		public virtual void Update()
		{
            // Do we need to update the geometry buffer?
            if (updategeo)
			{
                //mxd
                if (!checkedIfModel) {
                    //check if thing is model
                    if (GZBuilder.GZGeneral.ModelDefEntries.ContainsKey(thing.Type)) {
                        ModelDefEntry mde = GZBuilder.GZGeneral.ModelDefEntries[thing.Type];
                        if (mde.Model == null)
                            thing.IsModel = GZBuilder.GZGeneral.LoadModelForThing(thing);
                        else
                            thing.IsModel = true;
                    }
                    if (thing.IsModel)
                        updateBoundingBoxForModel();
                    checkedIfModel = true;
                }

                // Trash geometry buffer
                if (geobuffer != null) geobuffer.Dispose();
                geobuffer = null;

                // Any vertics?
                if (vertices.Length > 0) {
                    // Make a new buffer
                    geobuffer = new VertexBuffer(General.Map.Graphics.Device, WorldVertex.Stride * vertices.Length,
                                                 Usage.WriteOnly | Usage.Dynamic, VertexFormat.None, Pool.Default);

                    // Fill the buffer
                    DataStream bufferstream = geobuffer.Lock(0, WorldVertex.Stride * vertices.Length, LockFlags.Discard);
                    bufferstream.WriteRange<WorldVertex>(vertices);
                    geobuffer.Unlock();
                    bufferstream.Dispose();
                }

                //mxd. Check if thing is light
                int light_id = Array.IndexOf(GZBuilder.GZGeneral.GZ_LIGHTS, thing.Type);
                if (light_id != -1) {
                    updateLight(light_id);
                    updateBoundingBox(lightRadius, lightRadius);
                } else {
                    if (thing.IsModel) {
                        updateBoundingBoxForModel();
                    } else {
                        updateBoundingBox((int)thing.Size, thingHeight);
                    }
                    lightType = -1;
                    lightRadius = -1;
                    lightRadiusMin = -1;
                    lightRadiusMax = -1;
                    lightRenderStyle = -1;
                }

				// Done
				updategeo = false;
			}
		}

        //mxd update light info
        private void updateLight(int light_id) {
            if (light_id < GZBuilder.GZGeneral.GZ_LIGHT_TYPES[2]) { //if it's gzdoom light
                int n;
                if (light_id < GZBuilder.GZGeneral.GZ_LIGHT_TYPES[0]) {
                    n = 0;
                    lightRenderStyle = (int)GZDoomLightRenderStyle.NORMAL;
                    lightColor = new Color4((float)lightRenderStyle / 100.0f, (float)thing.Args[0] / 255, (float)thing.Args[1] / 255, (float)thing.Args[2] / 255);
                } else if (light_id < GZBuilder.GZGeneral.GZ_LIGHT_TYPES[1]) {
                    n = 10;
                    lightRenderStyle = (int)GZDoomLightRenderStyle.ADDITIVE;
                    lightColor = new Color4((float)lightRenderStyle / 100.0f, (float)thing.Args[0] / 255, (float)thing.Args[1] / 255, (float)thing.Args[2] / 255);
                } else {
                    n = 20;
                    lightRenderStyle = (int)GZDoomLightRenderStyle.NEGATIVE;
                    lightColor = new Color4((float)lightRenderStyle / 100.0f, (float)thing.Args[2] / 255, (float)thing.Args[1] / 255, (float)thing.Args[0] / 255);
                }
                lightType = thing.Type - 9800 - n;

                if (lightType == (int)GZDoomLightType.SECTOR) {
                    lightRadiusMin = thing.Args[3] * 4;
                } else {
                    lightRadiusMin = thing.Args[3] * 2; //works... that.. way in GZDoom
                    if (lightType > 0)
                        lightRadiusMax = thing.Args[4] * 2;
                }
            } else { //it's one of vavoom lights
                lightRenderStyle = (int)GZDoomLightRenderStyle.NORMAL;
                lightType = thing.Type;
                if (lightType == (int)GZDoomLightType.VAVOOM_COLORED)
                    lightColor = new Color4((float)lightRenderStyle / 100.0f, (float)thing.Args[1] / 255, (float)thing.Args[2] / 255, (float)thing.Args[3] / 255);
                else
                    lightColor = new Color4((float)lightRenderStyle / 100.0f, 1.0f, 1.0f, 1.0f);
                lightRadiusMin = thing.Args[0] * 4;
            }
            UpdateLightRadius();
        }

        //mxd
        public void UpdateLightRadius() {
            if (lightType == -1) {
                General.ErrorLogger.Add(ErrorType.Error, "Please check that thing is light before accessing it's PositionAndRadius! You can use lightType, which is -1 if thing isn't light");
                return;
            }

            if (!General.Settings.GZAnimateLights || lightType == (int)GZDoomLightType.NORMAL
                                                  || lightType == (int)GZDoomLightType.SECTOR
                                                  || lightType == (int)GZDoomLightType.VAVOOM
                                                  || lightType == (int)GZDoomLightType.VAVOOM_COLORED) {
                lightRadius = lightRadiusMin;
                return;
            }

            //if (lightRadiusMax < lightRadiusMin) lightRadiusMax = lightRadiusMin;
            double time = General.Clock.GetCurrentTime();
            int diff = lightRadiusMax - lightRadiusMin;

            //pulse
            if (lightType == (int)GZDoomLightType.PULSE) {
                lightDelta = (float)Math.Sin(time / (100.0f * thing.Angle * 2.3f)); //just playing by the eye here...
                lightRadius = Math.Abs((int)(lightRadiusMin + diff + diff * lightDelta));

            //flicker
            } else if (lightType == (int)GZDoomLightType.FLICKER) {
                float delta = (float)Math.Sin(time / (100.0f * thing.Angle * 0.02f)); //just playing by the eye here...
                if (Math.Sign(delta) != Math.Sign(lightDelta)) {
                    lightDelta = delta;
                    int angle = (int)(thing.Angle * 180 / Math.PI);
                    if (new Random().Next(0, 360) > angle) {
                        lightRadius = lightRadiusMax;
                    } else {
                        lightRadius = lightRadiusMin;
                    }
                }
            //random
            } else if (lightType == (int)GZDoomLightType.RANDOM) {
                float delta = (float)Math.Sin(time / (100.0f * thing.Angle * 4.6f)); //just playing by the eye here...
                if (Math.Sign(delta) != Math.Sign(lightDelta))
                    lightRadius = lightRadiusMin + new Random().Next(0, diff);
                lightDelta = delta;
            }

            //return lightRadius;
        }

        //mxd. update bounding box
        private void updateBoundingBox(int width, int height) {
            boundingBox = new Vector3[9];
            boundingBox[0] = Center;

            boundingBox[1] = new Vector3(position_v3.X - width, position_v3.Y - width, position_v3.Z);
            boundingBox[2] = new Vector3(position_v3.X + width, position_v3.Y - width, position_v3.Z);
            boundingBox[3] = new Vector3(position_v3.X - width, position_v3.Y + width, position_v3.Z);
            boundingBox[4] = new Vector3(position_v3.X + width, position_v3.Y + width, position_v3.Z);

            boundingBox[5] = new Vector3(position_v3.X - width, position_v3.Y - width, position_v3.Z + height);
            boundingBox[6] = new Vector3(position_v3.X + width, position_v3.Y - width, position_v3.Z + height);
            boundingBox[7] = new Vector3(position_v3.X - width, position_v3.Y + width, position_v3.Z + height);
            boundingBox[8] = new Vector3(position_v3.X + width, position_v3.Y + width, position_v3.Z + height);
        }

        //mxd. update bounding box from model bounding box
        private void updateBoundingBoxForModel() {
            ModelDefEntry mde = GZBuilder.GZGeneral.ModelDefEntries[thing.Type];
            int len = mde.Model.BoundingBox.Length;
            boundingBox = new Vector3[len];
            for (int i = 0; i < len; i++) {
                Vector3 v = mde.Model.BoundingBox[i];
                boundingBox[i] = new Vector3(v.X + position_v3.X, v.Y + position_v3.Y, v.Z + position_v3.Z);
            }
        }
		
		/// <summary>
		/// This is called when the thing must be tested for line intersection. This should reject
		/// as fast as possible to rule out all geometry that certainly does not touch the line.
		/// </summary>
		public virtual bool PickFastReject(Vector3D from, Vector3D to, Vector3D dir)
		{
			return false;
		}
		
		/// <summary>
		/// This is called when the thing must be tested for line intersection. This should perform
		/// accurate hit detection and set u_ray to the position on the ray where this hits the geometry.
		/// </summary>
		public virtual bool PickAccurate(Vector3D from, Vector3D to, Vector3D dir, ref float u_ray)
		{
			return false;
		}
		
		/// <summary>
		/// This sorts things by distance from the camera. Farthest first.
		/// </summary>
		public int CompareTo(VisualThing other)
		{
			return Math.Sign(other.cameradistance - this.cameradistance);
		}
		
		#endregion
	}
}
