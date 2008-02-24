
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
using SlimDX.Direct3D;
using SlimDX.Direct3D9;
using SlimDX;
using CodeImp.DoomBuilder.Geometry;
using System.Drawing.Imaging;
using CodeImp.DoomBuilder.Data;
using CodeImp.DoomBuilder.Editing;

#endregion

namespace CodeImp.DoomBuilder.Rendering
{
	internal unsafe sealed class Renderer2D : Renderer, IRenderer2D
	{
		#region ================== Constants

		private const byte DOUBLESIDED_LINE_ALPHA = 130;
		private const float FSAA_BLEND_FACTOR = 0.6f;
		private const float THING_ARROW_SIZE = 1.5f;
		private const float THING_ARROW_SHRINK = 2f;
		private const float THING_CIRCLE_SIZE = 1f;
		private const float THING_CIRCLE_SHRINK = 2f;
		private const int THING_BUFFER_STEP = 100;
		private const float THINGS_BACK_ALPHA = 0.3f;

		#endregion

		#region ================== Variables

		// Rendertargets
		private Texture backtex;
		private Texture structtex;
		private Texture thingstex;

		// Locking data
		private LockedRect structlocked;
		private Surface thingssurface;

		// Rendertarget sizes
		private Size windowsize;
		private Size structsize;
		private Size thingssize;
		private Size backsize;
		
		// Geometry plotter
		private Plotter plotter;

		// Vertices to present the textures
		private VertexBuffer screenverts;
		private FlatVertex[] backimageverts;
		
		// Batch buffer for things rendering
		private VertexBuffer thingsvertices;
		private int maxthings, numthings;
		
		// Render settings
		private bool thingsfront;
		private int vertexsize;

		// Images
		private ResourceImage thingtexture;
		private ResourceImage thingtexturesimple;
		
		// View settings (world coordinates)
		private float scale;
		private float scaleinv;
		private float offsetx;
		private float offsety;
		private float translatex;
		private float translatey;
		private float linenormalsize;
		private float lastgridscale = -1f;
		private float lastgridx;
		private float lastgridy;
		
		#endregion

		#region ================== Properties

		public float OffsetX { get { return offsetx; } }
		public float OffsetY { get { return offsety; } }
		public float Scale { get { return scale; } }

		#endregion

		#region ================== Constructor / Disposer
		
		// Constructor
		internal Renderer2D(D3DDevice graphics) : base(graphics)
		{
			// Initialize
			thingtexturesimple = new ResourceImage("Thing2D_Simple.png");
			thingtexturesimple.UseColorCorrection = false;
			thingtexturesimple.LoadImage();
			thingtexturesimple.CreateTexture();
			thingtexture = new ResourceImage("Thing2D.png");
			thingtexture.UseColorCorrection = false;
			thingtexture.LoadImage();
			thingtexture.CreateTexture();

			// Create rendertargets
			CreateRendertargets();
			
			// We have no destructor
			GC.SuppressFinalize(this);
		}

		// Disposer
		internal override void Dispose()
		{
			// Not already disposed?
			if(!isdisposed)
			{
				// Destroy rendertargets
				DestroyRendertargets();
				thingtexture.Dispose();
				thingtexturesimple.Dispose();
				
				// Done
				base.Dispose();
			}
		}

		#endregion

		#region ================== Presenting

		// This draws the image on screen
		public void Present()
		{
			// Start drawing
			if(graphics.StartRendering(true, General.Colors.Background.ToInt(), graphics.BackBuffer, graphics.DepthBuffer))
			{
				// Renderstates that count for this whole sequence
				graphics.Device.SetRenderState(RenderState.CullMode, Cull.None);
				graphics.Device.SetRenderState(RenderState.ZEnable, false);
				graphics.Shaders.Display2D.Begin();

				// Render a background image?
				if((backimageverts != null) && (General.Map.Grid.Background.Texture != null))
				{
					// Set renderstates
					graphics.Device.SetRenderState(RenderState.AlphaBlendEnable, false);
					graphics.Device.SetRenderState(RenderState.AlphaTestEnable, true);
					graphics.Device.SetRenderState(RenderState.TextureFactor, -1);
					graphics.Device.SetTexture(0, General.Map.Grid.Background.Texture);
					graphics.Shaders.Display2D.Texture1 = General.Map.Grid.Background.Texture;
					graphics.Shaders.Display2D.SetSettings(1f / windowsize.Width, 1f / windowsize.Height, FSAA_BLEND_FACTOR, 1f);

					// Draw the background image
					graphics.Shaders.Display2D.BeginPass(1);
					graphics.Device.DrawUserPrimitives<FlatVertex>(PrimitiveType.TriangleStrip, 0, 2, backimageverts);
					graphics.Shaders.Display2D.EndPass();
				}
				
				// From here on only using screen vertices
				graphics.Device.SetStreamSource(0, screenverts, 0, sizeof(FlatVertex));
				
				// Render things in back?
				if(!thingsfront) PresentThings(THINGS_BACK_ALPHA);

				// Set renderstates
				graphics.Device.SetRenderState(RenderState.AlphaBlendEnable, false);
				graphics.Device.SetRenderState(RenderState.AlphaTestEnable, true);
				graphics.Device.SetRenderState(RenderState.TextureFactor, -1);
				graphics.Device.SetTexture(0, backtex);
				graphics.Shaders.Display2D.Texture1 = backtex;
				graphics.Shaders.Display2D.SetSettings(1f / backsize.Width, 1f / backsize.Height, 0f, 1f);

				// Draw the background grid
				graphics.Shaders.Display2D.BeginPass(1);
				//graphics.Device.DrawUserPrimitives<FlatVertex>(PrimitiveType.TriangleStrip, 0, 2, backverts);
				graphics.Device.DrawPrimitives(PrimitiveType.TriangleStrip, 0, 2);
				graphics.Shaders.Display2D.EndPass();
				
				// Set renderstates
				graphics.Device.SetRenderState(RenderState.AlphaBlendEnable, true);
				graphics.Device.SetRenderState(RenderState.AlphaTestEnable, false);
				graphics.Device.SetRenderState(RenderState.SourceBlend, Blend.SourceAlpha);
				graphics.Device.SetRenderState(RenderState.DestBlend, Blend.InvSourceAlpha);
				graphics.Device.SetRenderState(RenderState.TextureFactor, -1);
				graphics.Device.SetTexture(0, structtex);
				graphics.Shaders.Display2D.Texture1 = structtex;
				graphics.Shaders.Display2D.SetSettings(1f / structsize.Width, 1f / structsize.Height, FSAA_BLEND_FACTOR, 1f);
				
				// Draw the lines and vertices texture
				graphics.Shaders.Display2D.BeginPass(0);
				//try { graphics.Device.DrawUserPrimitives<FlatVertex>(PrimitiveType.TriangleStrip, 0, 2, structverts); } catch(Exception) { }
				//graphics.Device.DrawUserPrimitives<FlatVertex>(PrimitiveType.TriangleStrip, 0, 2, structverts);
				graphics.Device.DrawPrimitives(PrimitiveType.TriangleStrip, 0, 2);
				graphics.Shaders.Display2D.EndPass();
				
				// Render things in front?
				if(thingsfront) PresentThings(1f);
				
				// Done
				graphics.Shaders.Display2D.End();
				graphics.FinishRendering();
				graphics.Present();
			}
		}

		// This presents the things
		private void PresentThings(float alpha)
		{
			// Set renderstates
			//graphics.Device.SetRenderState(RenderState.AlphaBlendEnable, false);
			//graphics.Device.SetRenderState(RenderState.AlphaTestEnable, true);
			//graphics.Device.SetRenderState(RenderState.AlphaFunc, Compare.GreaterEqual);
			graphics.Device.SetRenderState(RenderState.AlphaBlendEnable, true);
			graphics.Device.SetRenderState(RenderState.AlphaTestEnable, false);
			graphics.Device.SetRenderState(RenderState.SourceBlend, Blend.SourceAlpha);
			graphics.Device.SetRenderState(RenderState.DestBlend, Blend.InvSourceAlpha);
			graphics.Device.SetRenderState(RenderState.TextureFactor, (new ColorValue(alpha, 1f, 1f, 1f)).ToArgb());
			graphics.Device.SetTexture(0, thingstex);
			graphics.Shaders.Display2D.Texture1 = thingstex;
			graphics.Shaders.Display2D.SetSettings(1f / thingssize.Width, 1f / thingssize.Height, FSAA_BLEND_FACTOR, alpha);

			// Draw the things texture
			graphics.Shaders.Display2D.BeginPass(0);
			//try { graphics.Device.DrawUserPrimitives<FlatVertex>(PrimitiveType.TriangleStrip, 0, 2, thingsverts); } catch(Exception) { }
			//graphics.Device.DrawUserPrimitives<FlatVertex>(PrimitiveType.TriangleStrip, 0, 2, thingsverts);
			graphics.Device.DrawPrimitives(PrimitiveType.TriangleStrip, 0, 2);
			graphics.Shaders.Display2D.EndPass();
		}
		
		#endregion

		#region ================== Management

		// This is called before a device is reset
		// (when resized or display adapter was changed)
		/// <summary>
		/// DO NOT USE.
		/// </summary>
		public override void UnloadResource()
		{
			// Destroy rendertargets
			DestroyRendertargets();
		}
		
		// This is called resets when the device is reset
		// (when resized or display adapter was changed)
		/// <summary>
		/// DO NOT USE.
		/// </summary>
		public override void ReloadResource()
		{
			// Re-create rendertargets
			CreateRendertargets();
		}

		// This resets the graphics
		public override void Reset()
		{
			UnloadResource();
			ReloadResource();
		}

		// This destroys the rendertargets
		public void DestroyRendertargets()
		{
			// Trash rendertargets
			if(structtex != null) structtex.Dispose();
			if(thingstex != null) thingstex.Dispose();
			if(backtex != null) backtex.Dispose();
			if(screenverts != null) screenverts.Dispose();
			structtex = null;
			thingstex = null;
			backtex = null;
			screenverts = null;
			
			// Trash things batch buffer
			if(thingsvertices != null) thingsvertices.Dispose();
			thingsvertices = null;
			numthings = 0;
			maxthings = 0;
			lastgridscale = -1f;
		}
		
		// Allocates new image memory to render on
		public void CreateRendertargets()
		{
			SurfaceDescription sd;
			DataStream stream;
			FlatVertex[] verts;
			
			// Destroy rendertargets
			DestroyRendertargets();
			
			// Get new width and height
			windowsize.Width = graphics.RenderTarget.ClientSize.Width;
			windowsize.Height = graphics.RenderTarget.ClientSize.Height;

			// Create rendertargets textures
			structtex = new Texture(graphics.Device, windowsize.Width, windowsize.Height, 1, Usage.None, Format.A8R8G8B8, Pool.Managed);
			thingstex = new Texture(graphics.Device, windowsize.Width, windowsize.Height, 1, Usage.RenderTarget, Format.A8R8G8B8, Pool.Default);
			backtex = new Texture(graphics.Device, windowsize.Width, windowsize.Height, 1, Usage.None, Format.A8R8G8B8, Pool.Managed);
			
			// Get the real surface sizes
			sd = structtex.GetLevelDescription(0);
			structsize.Width = sd.Width;
			structsize.Height = sd.Height;
			sd = thingstex.GetLevelDescription(0);
			thingssize.Width = sd.Width;
			thingssize.Height = sd.Height;
			sd = backtex.GetLevelDescription(0);
			backsize.Width = sd.Width;
			backsize.Height = sd.Height;

			// Create vertex buffers
			screenverts = new VertexBuffer(graphics.Device, 4 * sizeof(FlatVertex), Usage.Dynamic | Usage.WriteOnly, VertexFormat.None, Pool.Default);

			// Make screen vertices
			stream = screenverts.Lock(0, 4 * sizeof(FlatVertex), LockFlags.Discard | LockFlags.NoSystemLock);
			verts = CreateScreenVerts(structsize);
			stream.WriteRange<FlatVertex>(verts);
			screenverts.Unlock();
			stream.Dispose();
		}

		// This makes screen vertices for display
		private FlatVertex[] CreateScreenVerts(Size texturesize)
		{
			FlatVertex[] screenverts = new FlatVertex[4];
			screenverts[0].x = 0.5f;
			screenverts[0].y = 0.5f;
			screenverts[0].w = 1f;
			screenverts[0].c = -1;
			screenverts[0].u = 1f / texturesize.Width;
			screenverts[0].v = 1f / texturesize.Height;
			screenverts[1].x = texturesize.Width - 1.5f;
			screenverts[1].y = 0.5f;
			screenverts[1].w = 1f;
			screenverts[1].c = -1;
			screenverts[1].u = 1f - 1f / texturesize.Width;
			screenverts[1].v = 1f / texturesize.Height;
			screenverts[2].x = 0.5f;
			screenverts[2].y = texturesize.Height - 1.5f;
			screenverts[2].c = -1;
			screenverts[2].w = 1f;
			screenverts[2].u = 1f / texturesize.Width;
			screenverts[2].v = 1f - 1f / texturesize.Height;
			screenverts[3].x = texturesize.Width - 1.5f;
			screenverts[3].y = texturesize.Height - 1.5f;
			screenverts[3].w = 1f;
			screenverts[3].c = -1;
			screenverts[3].u = 1f - 1f / texturesize.Width;
			screenverts[3].v = 1f - 1f / texturesize.Height;
			return screenverts;
		}

		#endregion
		
		#region ================== Coordination

		// This changes view position
		public void PositionView(float x, float y)
		{
			// Change position in world coordinates
			offsetx = x;
			offsety = y;
			UpdateTransformations();
		}
		
		// This changes zoom
		public void ScaleView(float scale)
		{
			// Change zoom scale
			this.scale = scale;
			UpdateTransformations();
			
			// Show zoom on main window
			General.MainWindow.UpdateZoom(scale);
			
			// Recalculate linedefs (normal lengths must be adjusted)
			foreach(Linedef l in General.Map.Map.Linedefs) l.NeedUpdate();
		}

		// This updates some maths
		private void UpdateTransformations()
		{
			scaleinv = 1f / scale;
			translatex = -offsetx + (windowsize.Width * 0.5f) * scaleinv;
			translatey = -offsety - (windowsize.Height * 0.5f) * scaleinv;
			linenormalsize = 10f * scaleinv;
			vertexsize = (int)(1.7f * scale + 0.5f);
			if(vertexsize < 0) vertexsize = 0;
			if(vertexsize > 4) vertexsize = 4;
		}
		
		// This unprojects mouse coordinates into map coordinates
		public Vector2D GetMapCoordinates(Vector2D mousepos)
		{
			return mousepos.GetInvTransformed(-translatex, -translatey, scaleinv, -scaleinv);
		}

		#endregion

		#region ================== Things

		// This ensures there is enough place in the things buffer
		private void ReserveThingsMemory(int newnumthings, bool preserve)
		{
			int newmaxthings;
			DataStream stream;
			FlatVertex[] verts = null;
			
			// Do we need to make changes?
			if((newnumthings > maxthings) || !preserve)
			{
				// Read old things data if we want to keep it
				if(preserve && (thingsvertices != null) && (numthings > 0))
				{
					stream = thingsvertices.Lock(0, numthings * 12 * FlatVertex.Stride, LockFlags.ReadOnly | LockFlags.NoSystemLock);
					verts = stream.ReadRange<FlatVertex>(numthings * 12);
					thingsvertices.Unlock();
					stream.Dispose();
				}
				
				// Buffer needs to be reallocated?
				if(newnumthings > maxthings)
				{
					// Calculate new size
					newmaxthings = newnumthings + THING_BUFFER_STEP;
					
					// Trash old buffer
					if(thingsvertices != null) thingsvertices.Dispose();
					
					// Create new buffer
					thingsvertices = new VertexBuffer(graphics.Device, newmaxthings * 12 * FlatVertex.Stride, Usage.None, VertexFormat.None, Pool.Managed);
					maxthings = newmaxthings;
				}
				else
				{
					// Buffer stays the same
					newmaxthings = maxthings;
				}
				
				// Keep old things?
				if(preserve && (verts != null))
				{
					// Write old things into new buffer
					stream = thingsvertices.Lock(0, maxthings * 12 * FlatVertex.Stride, LockFlags.Discard | LockFlags.NoSystemLock);
					stream.WriteRange<FlatVertex>(verts);
					thingsvertices.Unlock();
					stream.Dispose();
				}
				else
				{
					// Things were trashed
					numthings = 0;
				}
			}
		}

		// This makes vertices for a thing
		// Returns false when not on the screen
		private bool CreateThingVerts(Thing t, ref FlatVertex[] verts, int offset, PixelColor c)
		{
			float circlesize;
			float arrowsize;
			int color;
			
			// Transform to screen coordinates
			Vector2D screenpos = ((Vector2D)t.Position).GetTransformed(translatex, translatey, scale, -scale);
			
			// Determine sizes
			circlesize = (t.Size - THING_CIRCLE_SHRINK) * scale * THING_CIRCLE_SIZE;
			arrowsize = (t.Size - THING_ARROW_SHRINK) * scale * THING_ARROW_SIZE;
			
			// Check if the thing is actually on screen
			if(((screenpos.x + circlesize) > 0.0f) && ((screenpos.x - circlesize) < (float)windowsize.Width) &&
				((screenpos.y + circlesize) > 0.0f) && ((screenpos.y - circlesize) < (float)windowsize.Height))
			{
				// Get integral color
				color = c.ToInt();

				// Setup fixed rect for circle
				verts[offset].x = screenpos.x - circlesize;
				verts[offset].y = screenpos.y - circlesize;
				verts[offset].w = 1f;
				verts[offset].c = color;
				verts[offset].u = 1f / 512f;
				verts[offset].v = 1f / 128f;
				offset++;
				verts[offset].x = screenpos.x + circlesize;
				verts[offset].y = screenpos.y - circlesize;
				verts[offset].w = 1f;
				verts[offset].c = color;
				verts[offset].u = 0.25f - 1f / 512f;
				verts[offset].v = 1f / 128f;
				offset++;
				verts[offset].x = screenpos.x - circlesize;
				verts[offset].y = screenpos.y + circlesize;
				verts[offset].w = 1f;
				verts[offset].c = color;
				verts[offset].u = 1f / 512f;
				verts[offset].v = 1f - 1f / 128f;
				offset++;
				verts[offset] = verts[offset - 2];
				offset++;
				verts[offset] = verts[offset - 2];
				offset++;
				verts[offset].x = screenpos.x + circlesize;
				verts[offset].y = screenpos.y + circlesize;
				verts[offset].w = 1f;
				verts[offset].c = color;
				verts[offset].u = 0.25f - 1f / 512f;
				verts[offset].v = 1f - 1f / 128f;
				offset++;

				// Setup rotated rect for arrow
				verts[offset].x = screenpos.x + (float)Math.Sin(t.Angle - Angle2D.PI * 0.25f) * arrowsize;
				verts[offset].y = screenpos.y + (float)Math.Cos(t.Angle - Angle2D.PI * 0.25f) * arrowsize;
				verts[offset].w = 1f;
				verts[offset].c = -1;
				verts[offset].u = 0.50f + t.IconOffset;
				verts[offset].v = 0f;
				offset++;
				verts[offset].x = screenpos.x + (float)Math.Sin(t.Angle + Angle2D.PI * 0.25f) * arrowsize;
				verts[offset].y = screenpos.y + (float)Math.Cos(t.Angle + Angle2D.PI * 0.25f) * arrowsize;
				verts[offset].w = 1f;
				verts[offset].c = -1;
				verts[offset].u = 0.75f + t.IconOffset;
				verts[offset].v = 0f;
				offset++;
				verts[offset].x = screenpos.x + (float)Math.Sin(t.Angle - Angle2D.PI * 0.75f) * arrowsize;
				verts[offset].y = screenpos.y + (float)Math.Cos(t.Angle - Angle2D.PI * 0.75f) * arrowsize;
				verts[offset].w = 1f;
				verts[offset].c = -1;
				verts[offset].u = 0.50f + t.IconOffset;
				verts[offset].v = 1f;
				offset++;
				verts[offset] = verts[offset - 2];
				offset++;
				verts[offset] = verts[offset - 2];
				offset++;
				verts[offset].x = screenpos.x + (float)Math.Sin(t.Angle + Angle2D.PI * 0.75f) * arrowsize;
				verts[offset].y = screenpos.y + (float)Math.Cos(t.Angle + Angle2D.PI * 0.75f) * arrowsize;
				verts[offset].w = 1f;
				verts[offset].c = -1;
				verts[offset].u = 0.75f + t.IconOffset;
				verts[offset].v = 1f;

				// Done
				return true;
			}
			else
			{
				// Not on screen
				return false;
			}
		}
		
		// This draws a set of things
		private void RenderThingsBatch(int offset, int count)
		{
			// Anything to render?
			if(count > 0)
			{
				// Set renderstates for things rendering
				graphics.Device.SetRenderState(RenderState.CullMode, Cull.None);
				graphics.Device.SetRenderState(RenderState.ZEnable, false);
				graphics.Device.SetRenderState(RenderState.AlphaBlendEnable, false);
				graphics.Device.SetRenderState(RenderState.AlphaTestEnable, true);
				graphics.Device.SetRenderState(RenderState.TextureFactor, -1);
				graphics.Device.SetStreamSource(0, thingsvertices, 0, FlatVertex.Stride);

				if(General.Settings.QualityDisplay)
				{
					graphics.Device.SetTexture(0, thingtexture.Texture);
					graphics.Shaders.Things2D.Texture1 = thingtexture.Texture;
				}
				else
				{
					graphics.Device.SetTexture(0, thingtexturesimple.Texture);
					graphics.Shaders.Things2D.Texture1 = thingtexturesimple.Texture;
				}

				// Draw the things batched
				graphics.Shaders.Things2D.Begin();
				graphics.Shaders.Things2D.BeginPass(0);
				//try { graphics.Device.DrawPrimitives(PrimitiveType.TriangleList, offset * 12, count * 4); } catch(Exception) { }
				graphics.Device.DrawPrimitives(PrimitiveType.TriangleList, offset * 12, count * 4);
				graphics.Shaders.Things2D.EndPass();
				graphics.Shaders.Things2D.End();
			}
		}
		
		#endregion

		#region ================== Colors

		// This returns the color for a thing
		public PixelColor DetermineThingColor(Thing t)
		{
			// Determine color
			if(t.Selected) return General.Colors.Selection;
			else return t.Color;
		}

		// This returns the color for a vertex
		public int DetermineVertexColor(Vertex v)
		{
			// Determine color
			if(v.Selected) return ColorCollection.SELECTION;
			else return ColorCollection.VERTICES;
		}

		// This returns the color for a linedef
		public PixelColor DetermineLinedefColor(Linedef l)
		{
			// Impassable lines
			if((l.Flags & General.Map.Config.ImpassableFlags) != 0)
			{
				// Determine color
				if(l.Selected) return General.Colors.Selection;
				else if(l.Action != 0) return General.Colors.Actions;
				else return General.Colors.Linedefs;
			}
			else
			{
				// Determine color
				if(l.Selected) return General.Colors.Selection;
				else if(l.Action != 0) return General.Colors.Actions.WithAlpha(DOUBLESIDED_LINE_ALPHA);
				else if((l.Flags & General.Map.Config.SoundLinedefFlags) != 0) return General.Colors.Sounds.WithAlpha(DOUBLESIDED_LINE_ALPHA);
				else return General.Colors.Linedefs.WithAlpha(DOUBLESIDED_LINE_ALPHA);
			}
		}

		#endregion

		#region ================== Settings
		
		// This sets the things in front or back
		public void SetThingsRenderOrder(bool front)
		{
			// Set things render order
			this.thingsfront = front;
		}

		#endregion

		#region ================== Background

		// This sets up background image vertices
		private void SetupBackground()
		{
			Vector2D ltpos, rbpos;
			Vector2D backoffset = new Vector2D((float)General.Map.Grid.BackgroundX, (float)General.Map.Grid.BackgroundY);
			Vector2D backimagesize = new Vector2D((float)General.Map.Grid.Background.Width, (float)General.Map.Grid.Background.Height);
			
			// Only if a background image is set
			if((General.Map.Grid.Background != null) &&
			   !(General.Map.Grid.Background is NullImage))
			{
				// Make vertices
				backimageverts = CreateScreenVerts(windowsize);

				// Determine map coordinates for view window
				ltpos = GetMapCoordinates(new Vector2D(0f, 0f));
				rbpos = GetMapCoordinates(new Vector2D(windowsize.Width, windowsize.Height));
				
				// Offset by given background offset
				ltpos -= backoffset;
				rbpos -= backoffset;
				
				// Calculate UV coordinates
				// NOTE: backimagesize.y is made negative to match Doom's coordinate system
				backimageverts[0].u = ltpos.x / backimagesize.x;
				backimageverts[0].v = ltpos.y / -backimagesize.y;
				backimageverts[1].u = rbpos.x / backimagesize.x;
				backimageverts[1].v = ltpos.y / -backimagesize.y;
				backimageverts[2].u = ltpos.x / backimagesize.x;
				backimageverts[2].v = rbpos.y / -backimagesize.y;
				backimageverts[3].u = rbpos.x / backimagesize.x;
				backimageverts[3].v = rbpos.y / -backimagesize.y;
			}
			else
			{
				// No background image
				backimageverts = null;
			}
		}

		// This renders all grid
		private void RenderBackgroundGrid()
		{
			Plotter gridplotter;
			LockedRect lockedrect;
			
			// Do we need to redraw grid?
			if((lastgridscale != scale) || (lastgridx != offsetx) || (lastgridy != offsety))
			{
				// Lock background rendertarget memory
				lockedrect = backtex.LockRectangle(0, LockFlags.NoSystemLock);

				// Create a plotter
				gridplotter = new Plotter((PixelColor*)lockedrect.Data.DataPointer.ToPointer(), lockedrect.Pitch / sizeof(PixelColor), backsize.Height, backsize.Width, backsize.Height);
				gridplotter.Clear();

				// Render normal grid
				RenderGrid(General.Map.Grid.GridSize, General.Colors.Grid, gridplotter);

				// Render 64 grid
				if(General.Map.Grid.GridSize <= 64) RenderGrid(64f, General.Colors.Grid64, gridplotter);

				// Done
				backtex.UnlockRectangle(0);
				lockedrect.Data.Dispose();
				lastgridscale = scale;
				lastgridx = offsetx;
				lastgridy = offsety;
			}
		}
		
		// This renders the grid
		private void RenderGrid(float size, PixelColor c, Plotter gridplotter)
		{
			Vector2D ltpos, rbpos;
			Vector2D pos = new Vector2D();
			float sizeinv = 1f / size;
			
			// Only render grid when not screen-filling
			if((size * scale) > 6f)
			{
				// Determine map coordinates for view window
				ltpos = GetMapCoordinates(new Vector2D(0, 0));
				rbpos = GetMapCoordinates(new Vector2D(windowsize.Width, windowsize.Height));

				// Clip to nearest grid
				ltpos = GridSetup.SnappedToGrid(ltpos, size, sizeinv);
				rbpos = GridSetup.SnappedToGrid(rbpos, size, sizeinv);
				
				// Draw all horizontal grid lines
				for(float y = ltpos.y + size; y > rbpos.y - size; y -= size)
				{
					pos.y = y;
					pos = pos.GetTransformed(translatex, translatey, scale, -scale);
					gridplotter.DrawGridLineH((int)pos.y, c);
				}
				
				// Draw all vertical grid lines
				for(float x = ltpos.x - size; x < rbpos.x + size; x += size)
				{
					pos.x = x;
					pos = pos.GetTransformed(translatex, translatey, scale, -scale);
					gridplotter.DrawGridLineV((int)pos.x, c);
				}
			}
		}

		#endregion

		#region ================== Rendering

		// This begins a drawing session
		public unsafe bool Start(bool clearstructs, bool clearthings)
		{
			// Rendertargets available?
			if((structtex != null) && (thingstex != null))
			{
				// Lock structures rendertarget memory
				structlocked = structtex.LockRectangle(0, LockFlags.NoSystemLock);

				// Create structures plotter
				plotter = new Plotter((PixelColor*)structlocked.Data.DataPointer.ToPointer(), structlocked.Pitch / sizeof(PixelColor), structsize.Height, structsize.Width, structsize.Height);
				if(clearstructs) plotter.Clear();

				// Redraw grid when structures image was cleared
				if(clearstructs) RenderBackgroundGrid();

				// Always trash things batch buffer
				if(thingsvertices != null) thingsvertices.Dispose();
				thingsvertices = null;
				numthings = 0;
				maxthings = 0;
				
				// Setup vertices for background image
				SetupBackground();
				
				// Set the rendertarget to the things texture
				thingssurface = thingstex.GetSurfaceLevel(0);
				if(graphics.StartRendering(clearthings, 0, thingssurface, null))
				{
					// Ready for rendering
					return true;
				}
				else
				{
					// Can't render!
					Finish();
					return false;
				}
			}
			else
			{
				// Can't render!
				Finish();
				return false;
			}
		}

		// This ends a drawing session
		public void Finish()
		{
			// Stop rendering
			graphics.FinishRendering();
			
			// Release rendertarget
			try
			{
				graphics.Device.SetDepthStencilSurface(graphics.DepthBuffer);
				graphics.Device.SetRenderTarget(0, graphics.BackBuffer);
			}
			catch(Exception) { }

			// Clean up
			if(structtex != null) structtex.UnlockRectangle(0);
			if(structlocked.Data != null) structlocked.Data.Dispose();
			if(thingssurface != null) thingssurface.Dispose();
			thingssurface = null;
			plotter = null;
			
			// Present new image
			Present();
		}
		
		// This adds a thing in the things buffer for rendering
		public void RenderThing(Thing t, PixelColor c)
		{
			FlatVertex[] verts = new FlatVertex[12];
			DataStream stream;

			// Create vertices
			if(CreateThingVerts(t, ref verts, 0, c))
			{
				// Make sure there is enough memory reserved
				ReserveThingsMemory(numthings + 1, true);

				// Store vertices in buffer
				if(thingsvertices != null)
				{
					stream = thingsvertices.Lock(numthings * 12 * FlatVertex.Stride, 12 * FlatVertex.Stride, LockFlags.NoSystemLock);
					stream.WriteRange<FlatVertex>(verts);
					thingsvertices.Unlock();
					stream.Dispose();
				}
				
				// Thing added, render it
				RenderThingsBatch(numthings, 1);
				numthings++;
			}
		}

		// This adds a thing in the things buffer for rendering
		public void RenderThingSet(ICollection<Thing> things)
		{
			FlatVertex[] verts = new FlatVertex[things.Count * 12];
			DataStream stream;
			int addcount = 0;
			
			// Make sure there is enough memory reserved
			ReserveThingsMemory(numthings + things.Count, true);

			// Go for all things
			foreach(Thing t in things)
			{
				// Create vertices
				if(CreateThingVerts(t, ref verts, addcount * 12, DetermineThingColor(t)))
				{
					// Next
					addcount++;
				}
			}
			
			// Store vertices in buffer
			if(thingsvertices != null)
			{
				stream = thingsvertices.Lock(numthings * 12 * FlatVertex.Stride, things.Count * 12 * FlatVertex.Stride, LockFlags.NoSystemLock);
				stream.WriteRange<FlatVertex>(verts);
				thingsvertices.Unlock();
				stream.Dispose();
			}
			
			// Things added, render them
			RenderThingsBatch(numthings, addcount);
			numthings += addcount;
		}
		
		// This renders the linedefs of a sector with special color
		public void RenderSector(Sector s, PixelColor c)
		{
			// Go for all sides in the sector
			foreach(Sidedef sd in s.Sidedefs)
			{
				// Render this linedef
				RenderLinedef(sd.Line, c);

				// Render the two vertices on top
				RenderVertex(sd.Line.Start, DetermineVertexColor(sd.Line.Start));
				RenderVertex(sd.Line.End, DetermineVertexColor(sd.Line.End));
			}
		}

		// This renders the linedefs of a sector
		public void RenderSector(Sector s)
		{
			// Go for all sides in the sector
			foreach(Sidedef sd in s.Sidedefs)
			{
				// Render this linedef
				RenderLinedef(sd.Line, DetermineLinedefColor(sd.Line));

				// Render the two vertices on top
				RenderVertex(sd.Line.Start, DetermineVertexColor(sd.Line.Start));
				RenderVertex(sd.Line.End, DetermineVertexColor(sd.Line.End));
			}
		}	

		// This renders a simple line
		public void RenderLine(Vector2D start, Vector2D end, PixelColor c)
		{
			// Transform coordinates
			Vector2D v1 = start.GetTransformed(translatex, translatey, scale, -scale);
			Vector2D v2 = end.GetTransformed(translatex, translatey, scale, -scale);

			// Draw line
			plotter.DrawLineSolid((int)v1.x, (int)v1.y, (int)v2.x, (int)v2.y, c);
		}
		
		// This renders a single linedef
		public void RenderLinedef(Linedef l, PixelColor c)
		{
			// Transform vertex coordinates
			Vector2D v1 = l.Start.Position.GetTransformed(translatex, translatey, scale, -scale);
			Vector2D v2 = l.End.Position.GetTransformed(translatex, translatey, scale, -scale);

			// Draw line
			plotter.DrawLineSolid((int)v1.x, (int)v1.y, (int)v2.x, (int)v2.y, c);

			// Calculate normal indicator
			float mx = (v2.x - v1.x) * 0.5f;
			float my = (v2.y - v1.y) * 0.5f;

			// Draw normal indicator
			plotter.DrawLineSolid((int)(v1.x + mx), (int)(v1.y + my),
								  (int)((v1.x + mx) - (my * l.LengthInv) * linenormalsize),
								  (int)((v1.y + my) + (mx * l.LengthInv) * linenormalsize), c);
		}
		
		// This renders a set of linedefs
		public void RenderLinedefSet(ICollection<Linedef> linedefs)
		{
			// Go for all linedefs
			foreach(Linedef l in linedefs) RenderLinedef(l, DetermineLinedefColor(l));
		}

		// This renders a single vertex
		public void RenderVertex(Vertex v, int colorindex)
		{
			// Transform vertex coordinates
			Vector2D nv = v.Position.GetTransformed(translatex, translatey, scale, -scale);

			// Draw pixel here
			plotter.DrawVertexSolid((int)nv.x, (int)nv.y, vertexsize, General.Colors.Colors[colorindex], General.Colors.BrightColors[colorindex], General.Colors.DarkColors[colorindex]);
		}

		// This renders a single vertex at specified coordinates
		public void RenderVertexAt(Vector2D v, int colorindex)
		{
			// Transform vertex coordinates
			Vector2D nv = v.GetTransformed(translatex, translatey, scale, -scale);

			// Draw pixel here
			plotter.DrawVertexSolid((int)nv.x, (int)nv.y, vertexsize, General.Colors.Colors[colorindex], General.Colors.BrightColors[colorindex], General.Colors.DarkColors[colorindex]);
		}
		
		// This renders a set of vertices
		public void RenderVerticesSet(ICollection<Vertex> vertices)
		{
			// Go for all vertices
			foreach(Vertex v in vertices) RenderVertex(v, DetermineVertexColor(v));
		}

		#endregion
	}
}
