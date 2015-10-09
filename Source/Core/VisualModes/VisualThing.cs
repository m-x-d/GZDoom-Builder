
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
using CodeImp.DoomBuilder.Config;
using CodeImp.DoomBuilder.Data;
using CodeImp.DoomBuilder.Geometry;
using CodeImp.DoomBuilder.GZBuilder.Data; //mxd
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Rendering;
using SlimDX;
using SlimDX.Direct3D9;
using Plane = CodeImp.DoomBuilder.Geometry.Plane;

#endregion

namespace CodeImp.DoomBuilder.VisualModes
{
	public abstract class VisualThing : IVisualPickable, ID3DResource
	{
		#region ================== Constants

		protected const int FIXED_RADIUS = 8; //mxd. Used to render things with zero width and radius
		internal const float LIT_FOG_DENSITY_SCALER = 255 * VisualGeometry.FOG_DENSITY_SCALER; //mxd

		#endregion
		
		#region ================== Variables
		
		// Thing
		private readonly Thing thing;

		//mxd. Info
		protected ThingTypeInfo info;
		
		// Texture
		private ImageData texture;
		
		// Geometry
		private WorldVertex[] vertices;
		private VertexBuffer geobuffer;
		private VertexBuffer cagebuffer; //mxd
		private int cagelength; //mxd
		private bool updategeo;
		private int triangles;
		
		// Rendering
		private RenderPass renderpass;
		private Matrix position;
		private int cameradistance;
		private Color4 cagecolor;
		protected bool sizeless; //mxd. Used to render visual things with 0 width and height
		protected float fogfactor; //mxd

		// Selected?
		protected bool selected;

		// Disposing
		private bool isdisposed;

		//mxd
		protected float thingheight;

		//mxd. light properties
		private DynamicLightType lightType;
		private DynamicLightRenderStyle lightRenderStyle;
		private Color4 lightColor;
		private float lightRadius; //current radius. used in light animation
		private float lightPrimaryRadius;
		private float lightSecondaryRadius;
		private Vector3 position_v3;
		private float lightDelta; //used in light animation
		private Vector3D[] boundingBox;
		
		//gldefs light
		private Vector3 lightOffset;
		private int lightInterval;
		private bool isGldefsLight;
		
		#endregion
		
		#region ================== Properties
		
		internal VertexBuffer GeometryBuffer { get { return geobuffer; } }
		internal VertexBuffer CageBuffer { get { return cagebuffer; } } //mxd
		internal int CageLength { get { return cagelength; } } //mxd
		internal bool NeedsUpdateGeo { get { return updategeo; } }
		internal int Triangles { get { return triangles; } }
		internal Matrix Position { get { return position; } }
		internal Color4 CageColor { get { return cagecolor; } }
		public ThingTypeInfo Info { get { return info; } } //mxd
		
		//mxd
		internal int VertexColor { get { return vertices.Length > 0 ? vertices[0].c : 0;} }
		public int CameraDistance { get { return cameradistance; } }
		public float FogFactor { get { return fogfactor; } }
		public Vector3 Center
		{ 
			get
			{
				if(isGldefsLight) return position_v3 + lightOffset;
				return new Vector3(position_v3.X, position_v3.Y, position_v3.Z + thingheight / 2f); 
			} 
		}
		public Vector3D CenterV3D { get { return D3DDevice.V3D(Center); } }
		public float LocalCenterZ { get { return thingheight / 2f; } } //mxd
		public Vector3 PositionV3 { get { return position_v3; } }
		public Vector3D[] BoundingBox { get { return boundingBox; } }
		
		//mxd. light properties
		public DynamicLightType LightType { get { return lightType; } }
		public float LightRadius { get { return lightRadius; } }
		public DynamicLightRenderStyle LightRenderStyle { get { return lightRenderStyle; } }
		public Color4 LightColor { get { return lightColor; } }

		/// <summary>
		/// Returns the Thing that this VisualThing is created for.
		/// </summary>
		public Thing Thing { get { return thing; } }

		/// <summary>
		/// Render pass in which this geometry must be rendered. Default is Mask.
		/// </summary>
		public RenderPass RenderPass { get { return renderpass; } set { renderpass = value; } }
		
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
		protected VisualThing(Thing t)
		{
			// Initialize
			this.thing = t;
			this.renderpass = RenderPass.Mask;
			this.position = Matrix.Identity;

			//mxd
			lightType = DynamicLightType.NONE;
			lightRenderStyle = DynamicLightRenderStyle.NONE;
			lightPrimaryRadius = -1;
			lightSecondaryRadius = -1;
			lightInterval = -1;
			lightColor = new Color4();
			boundingBox = new Vector3D[9];
			
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
				if(cagebuffer != null) cagebuffer.Dispose(); //mxd
				cagebuffer = null; //mxd

				// Unregister resource
				General.Map.Graphics.UnregisterResource(this);

				// Done
				isdisposed = true;
			}
		}
		
		#endregion
		
		#region ================== Methods

		//mxd
		internal void CalculateCameraDistance(Vector3D campos) 
		{
			cameradistance = (int)((CenterV3D - campos).GetLengthSq());
		}
		
		// This is called before a device is reset (when resized or display adapter was changed)
		public void UnloadResource()
		{
			// Trash geometry buffer
			if(geobuffer != null) geobuffer.Dispose();
			geobuffer = null;
			updategeo = true;
		}
		
		// This is called resets when the device is reset
		// (when resized or display adapter was changed)
		public void ReloadResource()
		{
			// Make new geometry
			//Update();
		}

		/// <summary>
		/// Sets the color of the cage around the thing geometry and rebuilds the thing cage.
		/// </summary>
		protected void UpdateThingCage(PixelColor color)
		{
			cagecolor = color.ToColorValue();

			// Trash cage buffer
			if(cagebuffer != null) cagebuffer.Dispose();
			cagebuffer = null;

			// Make a new cage
			List<WorldVertex> cageverts;
			if(sizeless)
			{
				WorldVertex v0 = new WorldVertex(-info.Radius + position_v3.X, -info.Radius + position_v3.Y, position_v3.Z);
				WorldVertex v1 = new WorldVertex(info.Radius + position_v3.X, info.Radius + position_v3.Y, position_v3.Z);
				WorldVertex v2 = new WorldVertex(info.Radius + position_v3.X, -info.Radius + position_v3.Y, position_v3.Z);
				WorldVertex v3 = new WorldVertex(-info.Radius + position_v3.X, info.Radius + position_v3.Y, position_v3.Z);
				WorldVertex v4 = new WorldVertex(position_v3.X, position_v3.Y, info.Radius + position_v3.Z);
				WorldVertex v5 = new WorldVertex(position_v3.X, position_v3.Y, -info.Radius + position_v3.Z);

				cageverts = new List<WorldVertex>(new[] { v0, v1, v2, v3, v4, v5 });
			}
			else
			{
				float top = position_v3.Z + info.Height;
				float bottom = position_v3.Z;

				WorldVertex v0 = new WorldVertex(-info.Radius + position_v3.X, -info.Radius + position_v3.Y, bottom);
				WorldVertex v1 = new WorldVertex(-info.Radius + position_v3.X, info.Radius + position_v3.Y, bottom);
				WorldVertex v2 = new WorldVertex(info.Radius + position_v3.X, info.Radius + position_v3.Y, bottom);
				WorldVertex v3 = new WorldVertex(info.Radius + position_v3.X, -info.Radius + position_v3.Y, bottom);

				WorldVertex v4 = new WorldVertex(-info.Radius + position_v3.X, -info.Radius + position_v3.Y, top);
				WorldVertex v5 = new WorldVertex(-info.Radius + position_v3.X, info.Radius + position_v3.Y, top);
				WorldVertex v6 = new WorldVertex(info.Radius + position_v3.X, info.Radius + position_v3.Y, top);
				WorldVertex v7 = new WorldVertex(info.Radius + position_v3.X, -info.Radius + position_v3.Y, top);

				cageverts = new List<WorldVertex>(new[] { v0, v1,
														  v1, v2,
														  v2, v3,
														  v3, v0,
														  v4, v5, 
														  v5, v6,
														  v6, v7,
														  v7, v4,
														  v0, v4,
														  v1, v5,
														  v2, v6,
														  v3, v7 });
			}

			// Make new arrow
			if(Thing.IsDirectional)
			{
				Matrix transform = Matrix.Scaling(info.Radius, info.Radius, info.Radius)
					* (Matrix.RotationY(-Thing.RollRad) * Matrix.RotationX(-Thing.PitchRad) * Matrix.RotationZ(Thing.Angle))
					* (sizeless ? position : position * Matrix.Translation(0.0f, 0.0f, thingheight / 2f));

				WorldVertex a0 = new WorldVertex(Vector3D.Transform(0.0f, 0.0f, 0.0f, transform)); //start
				WorldVertex a1 = new WorldVertex(Vector3D.Transform(0.0f, -1.5f, 0.0f, transform)); //end
				WorldVertex a2 = new WorldVertex(Vector3D.Transform(0.2f, -1.1f, 0.2f, transform));
				WorldVertex a3 = new WorldVertex(Vector3D.Transform(-0.2f, -1.1f, 0.2f, transform));
				WorldVertex a4 = new WorldVertex(Vector3D.Transform(0.2f, -1.1f, -0.2f, transform));
				WorldVertex a5 = new WorldVertex(Vector3D.Transform(-0.2f, -1.1f, -0.2f, transform));

				cageverts.AddRange(new[] { a0, a1,
										   a1, a2,
										   a1, a3,
										   a1, a4,
										   a1, a5 });
			}

			// Create buffer
			WorldVertex[] cv = cageverts.ToArray();
			cagelength = cv.Length / 2;
			cagebuffer = new VertexBuffer(General.Map.Graphics.Device, WorldVertex.Stride * cv.Length, Usage.WriteOnly | Usage.Dynamic, VertexFormat.None, Pool.Default);
			cagebuffer.Lock(0, WorldVertex.Stride * cv.Length, LockFlags.None).WriteRange(cv);
			cagebuffer.Unlock();
		}

		/// <summary>
		/// This sets the position to use for the thing geometry.
		/// </summary>
		public void SetPosition(Vector3D pos)
		{
			position_v3 = D3DDevice.V3(pos); //mxd
			position = Matrix.Translation(position_v3);
			updategeo = true;

			//mxd. update bounding box?
			if(lightType != DynamicLightType.NONE && lightRadius > thing.Size) 
			{
				UpdateBoundingBox(lightRadius, lightRadius * 2);
			} 
		}

		// This sets the vertices for the thing sprite
		protected void SetVertices(ICollection<WorldVertex> verts, Plane floor, Plane ceiling)
		{
			// Copy vertices
			vertices = new WorldVertex[verts.Count];
			verts.CopyTo(vertices, 0);
			triangles = vertices.Length / 3;
			updategeo = true;
			
			//mxd. Do some GLOOME shenanigans...
			if(triangles < 2) return;
			float localcenterz = vertices[1].z * 0.5f;
			Matrix m;

			switch(info.RenderMode)
			{
				// TODO: Currently broken in GLOOME...
				case Thing.SpriteRenderMode.WALL_SPRITE:
					m = Matrix.Translation(0f, 0f, -localcenterz) * Matrix.RotationY(Thing.RollRad) * Matrix.RotationZ(thing.Angle) * Matrix.Translation(0f, 0f, localcenterz);
					for(int i = 0; i < vertices.Length; i++)
					{
						Vector4 transformed = Vector3.Transform(new Vector3(vertices[i].x, vertices[i].y, vertices[i].z), m);
						vertices[i].x = transformed.X;
						vertices[i].y = transformed.Y;
						vertices[i].z = transformed.Z;
					}
					break;

				case Thing.SpriteRenderMode.FLOOR_SPRITE:
					Matrix floorrotation = Matrix.RotationZ(info.RollSprite ? Thing.RollRad : 0f)
										 * Matrix.RotationY(Thing.Angle) 
										 * Matrix.RotationX(Angle2D.PIHALF);

					m = Matrix.Translation(0f, 0f, -localcenterz) * floorrotation * Matrix.Translation(0f, 0f, localcenterz);
					
					for(int i = 0; i < vertices.Length; i++)
					{
						Vector4 transformed = Vector3.Transform(new Vector3(vertices[i].x, vertices[i].y, vertices[i].z), m);
						vertices[i].x = transformed.X;
						vertices[i].y = transformed.Y;
						vertices[i].z = transformed.Z;
					}

					// TODO: this won't work on things with AbsoluteZ flag
					// TODO: +ROLLSPRITE implies +STICKTOPLANE?
					if(info.StickToPlane || info.RollSprite)
					{
						// Calculate vertical offset
						float floorz = floor.GetZ(Thing.Position);
						float ceilz = ceiling.GetZ(Thing.Position);

						if(!float.IsNaN(floorz) && !float.IsNaN(ceilz))
						{
							float voffset;
							if(info.Hangs)
							{
								float thingz = ceilz - Thing.Position.z + Thing.Height;
								voffset = 0.01f - floorz - General.Clamp(thingz, 0, ceilz - floorz);
							}
							else
							{
								voffset = 0.01f - floorz - General.Clamp(Thing.Position.z, 0, ceilz - floorz);
							}

							// Apply it
							for(int i = 0; i < vertices.Length; i++)
								vertices[i].z = floor.GetZ(vertices[i].x + Thing.Position.x, vertices[i].y + Thing.Position.y) + voffset;
						}
					}
					break;

				case Thing.SpriteRenderMode.CEILING_SPRITE:
					Matrix ceilrotation = Matrix.RotationZ(info.RollSprite ? Thing.RollRad : 0f)
										* Matrix.RotationY(Thing.Angle)
										* Matrix.RotationX(Angle2D.PIHALF);

					m = Matrix.Translation(0f, 0f, -localcenterz) * ceilrotation * Matrix.Translation(0f, 0f, localcenterz);
					
					for(int i = 0; i < vertices.Length; i++)
					{
						Vector4 transformed = Vector3.Transform(new Vector3(vertices[i].x, vertices[i].y, vertices[i].z), m);
						vertices[i].x = transformed.X;
						vertices[i].y = transformed.Y;
						vertices[i].z = transformed.Z;
					}

					// TODO: this won't work on things with AbsoluteZ flag
					// TODO: +ROLLSPRITE implies +STICKTOPLANE?
					if(info.StickToPlane || info.RollSprite)
					{
						// Calculate vertical offset
						float floorz = floor.GetZ(Thing.Position);
						float ceilz = ceiling.GetZ(Thing.Position);
						
						if(!float.IsNaN(floorz) && !float.IsNaN(ceilz))
						{
							float voffset;
							if(info.Hangs)
							{
								float thingz = ceilz - Math.Max(0, Thing.Position.z) - Thing.Height;
								voffset = -0.01f - General.Clamp(thingz, 0, ceilz - floorz);
							}
							else
							{
								voffset = -0.01f - floorz - General.Clamp(Thing.Position.z, 0, ceilz - floorz);
							}

							// Apply it
							for(int i = 0; i < vertices.Length; i++)
								vertices[i].z = ceiling.GetZ(vertices[i].x + Thing.Position.x, vertices[i].y + Thing.Position.y) + voffset;
						}
					}
					break;

				default:
					if(info.RollSprite)
					{
						m = Matrix.Translation(0f, 0f, -localcenterz) * Matrix.RotationY(Thing.RollRad) * Matrix.Translation(0f, 0f, localcenterz);
						for(int i = 0; i < vertices.Length; i++)
						{
							Vector4 transformed = Vector3.Transform(new Vector3(vertices[i].x, vertices[i].y, vertices[i].z), m);
							vertices[i].x = transformed.X;
							vertices[i].y = transformed.Y;
							vertices[i].z = transformed.Z;
						}
					}
					break;
			}
		}
		
		// This updates the visual thing
		public virtual void Update()
		{
			// Do we need to update the geometry buffer?
			if(updategeo)
			{
				// Trash geometry buffer
				if(geobuffer != null) geobuffer.Dispose();
				geobuffer = null;

				// Any vertics?
				if(vertices.Length > 0) 
				{
					// Make a new buffer
					geobuffer = new VertexBuffer(General.Map.Graphics.Device, WorldVertex.Stride * vertices.Length,
												 Usage.WriteOnly | Usage.Dynamic, VertexFormat.None, Pool.Default);

					// Fill the buffer
					DataStream bufferstream = geobuffer.Lock(0, WorldVertex.Stride * vertices.Length, LockFlags.Discard);
					bufferstream.WriteRange(vertices);
					geobuffer.Unlock();
					bufferstream.Dispose();
				}
				
				//mxd. Check if thing is light
				CheckLightState();

				// Done
				updategeo = false;
			}
		}

		//mxd
		protected void CheckLightState() 
		{
			//mxd. Check if thing is light
			int light_id = Array.IndexOf(GZBuilder.GZGeneral.GZ_LIGHTS, thing.Type);
			if (light_id != -1) 
			{
				isGldefsLight = false;
				lightInterval = -1;
				UpdateLight(light_id);
				UpdateBoundingBox(lightRadius, lightRadius * 2);
			}
			//check if we have light from GLDEFS
			else if (General.Map.Data.GldefsEntries.ContainsKey(thing.Type)) 
			{
				isGldefsLight = true;
				UpdateGldefsLight();
				UpdateBoundingBox(lightRadius, lightRadius * 2);
			} 
			else 
			{
				UpdateBoundingBox((int)thing.Size, thingheight);

				lightType = DynamicLightType.NONE;
				lightRadius = -1;
				lightPrimaryRadius = -1;
				lightSecondaryRadius = -1;
				lightRenderStyle = DynamicLightRenderStyle.NONE;
				lightInterval = -1;
				isGldefsLight = false;
			}
		}

		//mxd. Used in ColorPicker to update light 
		public void UpdateLight() 
		{
			int light_id = Array.IndexOf(GZBuilder.GZGeneral.GZ_LIGHTS, thing.Type);
			if (light_id != -1) 
			{
				UpdateLight(light_id);
				UpdateBoundingBox(lightRadius, lightRadius * 2);
			}
		}

		//mxd. Update light info
		private void UpdateLight(int lightId) 
		{
			float scaled_intensity = 255.0f / General.Settings.GZDynamicLightIntensity;

			if (lightId < GZBuilder.GZGeneral.GZ_LIGHT_TYPES[2]) //if it's gzdoom light
			{ 
				int n;
				if (lightId < GZBuilder.GZGeneral.GZ_LIGHT_TYPES[0]) 
				{
					n = 0;
					lightRenderStyle = DynamicLightRenderStyle.NORMAL;
					//lightColor.Alpha used in shader to perform some calculations based on light type
					lightColor = new Color4((float)lightRenderStyle / 100.0f, thing.Args[0] / scaled_intensity, thing.Args[1] / scaled_intensity, thing.Args[2] / scaled_intensity);
				} 
				else if (lightId < GZBuilder.GZGeneral.GZ_LIGHT_TYPES[1]) 
				{
					n = 10;
					lightRenderStyle = DynamicLightRenderStyle.ADDITIVE;
					lightColor = new Color4((float)lightRenderStyle / 100.0f, thing.Args[0] / scaled_intensity, thing.Args[1] / scaled_intensity, thing.Args[2] / scaled_intensity);
				} 
				else 
				{
					n = 20;
					lightRenderStyle = DynamicLightRenderStyle.NEGATIVE;
					lightColor = new Color4((float)lightRenderStyle / 100.0f, thing.Args[0] / scaled_intensity, thing.Args[1] / scaled_intensity, thing.Args[2] / scaled_intensity);
				}
				lightType = (DynamicLightType)(thing.Type - 9800 - n);

				if (lightType == DynamicLightType.SECTOR) 
				{
					int scaler = 1;
					if (thing.Sector != null) scaler = thing.Sector.Brightness / 4;
					lightPrimaryRadius = (thing.Args[3] * scaler) * General.Settings.GZDynamicLightRadius;
				} 
				else 
				{
					lightPrimaryRadius = (thing.Args[3] * 2) * General.Settings.GZDynamicLightRadius; //works... that.. way in GZDoom
					if (lightType > 0) lightSecondaryRadius = (thing.Args[4] * 2) * General.Settings.GZDynamicLightRadius;
				}
			}
			else //it's one of vavoom lights
			{ 
				lightRenderStyle = DynamicLightRenderStyle.VAVOOM;
				lightType = (DynamicLightType)thing.Type;
				if (lightType == DynamicLightType.VAVOOM_COLORED)
					lightColor = new Color4((float)lightRenderStyle / 100.0f, thing.Args[1] / scaled_intensity, thing.Args[2] / scaled_intensity, thing.Args[3] / scaled_intensity);
				else
					lightColor = new Color4((float)lightRenderStyle / 100.0f, General.Settings.GZDynamicLightIntensity, General.Settings.GZDynamicLightIntensity, General.Settings.GZDynamicLightIntensity);
				lightPrimaryRadius = (thing.Args[0] * 8) * General.Settings.GZDynamicLightRadius;
			}

			UpdateLightRadius();
		}

		//mxd
		private void UpdateGldefsLight() 
		{
			DynamicLightData light = General.Map.Data.GldefsEntries[thing.Type];
			float intensity_mod = General.Settings.GZDynamicLightIntensity;
			float scale_mod = General.Settings.GZDynamicLightRadius;

			//apply settings
			lightRenderStyle = light.Subtractive ? DynamicLightRenderStyle.NEGATIVE : DynamicLightRenderStyle.NORMAL;
			lightColor = new Color4((float)lightRenderStyle / 100.0f, light.Color.Red * intensity_mod, light.Color.Green * intensity_mod, light.Color.Blue * intensity_mod);
			Vector2D o = new Vector2D(light.Offset.X, light.Offset.Y).GetRotated(thing.Angle - Angle2D.PIHALF);
			lightOffset = new Vector3(o.x, o.y, light.Offset.Z);
			lightType = light.Type;

			if (lightType == DynamicLightType.SECTOR) 
			{
				lightPrimaryRadius = light.Interval * thing.Sector.Brightness / 5.0f;
			} 
			else 
			{
				lightPrimaryRadius = light.PrimaryRadius * scale_mod;
				lightSecondaryRadius = light.SecondaryRadius * scale_mod;
			}

			lightInterval = light.Interval;
			UpdateLightRadius(lightInterval);
		}

		//mxd
		public void UpdateLightRadius() 
		{
			UpdateLightRadius( (lightInterval != -1 ? lightInterval : thing.AngleDoom) );
		}

		//mxd
		private void UpdateLightRadius(int interval) 
		{
			if (lightType == DynamicLightType.NONE) 
			{
				General.ErrorLogger.Add(ErrorType.Error, "Please check that thing is light before accessing it's PositionAndRadius! You can use lightType, which is -1 if thing isn't light");
				return;
			}

			if (General.Settings.GZDrawLightsMode == LightRenderMode.ALL || Array.IndexOf(GZBuilder.GZGeneral.GZ_ANIMATED_LIGHT_TYPES, lightType) == -1) 
			{
				lightRadius = lightPrimaryRadius;
				return;
			}

			if(interval == 0) 
			{
				lightRadius = 0;
				return;
			}

			float time = Clock.CurrentTime;
			float rMin = Math.Min(lightPrimaryRadius, lightSecondaryRadius);
			float rMax = Math.Max(lightPrimaryRadius, lightSecondaryRadius);
			float diff = rMax - rMin;

			switch (lightType) 
			{
				case DynamicLightType.PULSE:
					lightDelta = ((float)Math.Sin(time / (interval * 4.0f)) + 1.0f) / 2.0f; //just playing by the eye here... in [0.0 ... 1.0] interval
					lightRadius = rMin + diff * lightDelta;
					break;

				case DynamicLightType.FLICKER: 
					float fdelta = (float)Math.Sin(time / 0.1f); //just playing by the eye here...
					if (Math.Sign(fdelta) != Math.Sign(lightDelta)) 
					{
						lightDelta = fdelta;
						lightRadius = (General.Random(0, 359) < interval ? rMax : rMin);
					}
					break;

				case DynamicLightType.RANDOM:
					float rdelta = (float)Math.Sin(time / (interval * 9.0f)); //just playing by the eye here...
					if (Math.Sign(rdelta) != Math.Sign(lightDelta)) 
					{
						lightRadius = rMin + (General.Random(0, (int) (diff * 10))) / 10.0f;
					}
					lightDelta = rdelta;
					break;
			}
		}

		//mxd. update bounding box
		public void UpdateBoundingBox() 
		{
			if(lightType != DynamicLightType.NONE && lightRadius > thing.Size)
				UpdateBoundingBox(lightRadius, lightRadius * 2);
		}

		private void UpdateBoundingBox(float width, float height) 
		{
			boundingBox = new Vector3D[9];
			boundingBox[0] = CenterV3D;
			float h2 = height / 2.0f;

			boundingBox[1] = new Vector3D(position_v3.X - width, position_v3.Y - width, Center.Z - h2);
			boundingBox[2] = new Vector3D(position_v3.X + width, position_v3.Y - width, Center.Z - h2);
			boundingBox[3] = new Vector3D(position_v3.X - width, position_v3.Y + width, Center.Z - h2);
			boundingBox[4] = new Vector3D(position_v3.X + width, position_v3.Y + width, Center.Z - h2);

			boundingBox[5] = new Vector3D(position_v3.X - width, position_v3.Y - width, Center.Z + h2);
			boundingBox[6] = new Vector3D(position_v3.X + width, position_v3.Y - width, Center.Z + h2);
			boundingBox[7] = new Vector3D(position_v3.X - width, position_v3.Y + width, Center.Z + h2);
			boundingBox[8] = new Vector3D(position_v3.X + width, position_v3.Y + width, Center.Z + h2);
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
		
		#endregion
	}
}
