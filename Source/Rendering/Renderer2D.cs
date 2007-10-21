
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

#endregion

namespace CodeImp.DoomBuilder.Rendering
{
	internal unsafe class Renderer2D : Renderer
	{
		#region ================== Constants

		private const byte DOUBLESIDED_LINE_ALPHA = 130;
		private const float FSAA_BLEND_FACTOR = 0.6f;
		private const float THING_ARROW_SIZE = 15f;
		private const float THING_CIRCLE_SIZE = 10f;
		private const int THING_BUFFER_STEP = 100;

		#endregion

		#region ================== Variables

		// Rendering memory for lines and vertices
		private Texture tex;
		private int width, height;
		private int pwidth, pheight;
		private Plotter plotter;
		private FlatVertex[] screenverts = new FlatVertex[4];
		private LockedRect lockedrect;
		
		// Buffers for rendering things
		private VertexBuffer thingsvertices;
		private PixelColor[] thingcolors;
		private int numthings;
		private int maxthings;
		
		// Images
		private ResourceImage thingtexture;
		
		// View settings (world coordinates)
		private float scale;
		private float scaleinv;
		private float offsetx;
		private float offsety;
		private float translatex;
		private float translatey;
		private float linenormalsize;
		
		#endregion

		#region ================== Properties

		public float OffsetX { get { return offsetx; } }
		public float OffsetY { get { return offsety; } }
		public float Scale { get { return scale; } }

		#endregion

		#region ================== Constructor / Disposer
		
		// Constructor
		public Renderer2D(D3DGraphics graphics) : base(graphics)
		{
			// Initialize
			thingtexture = new ResourceImage("Thing2D.png");
			thingtexture.LoadImage();
			thingtexture.CreateTexture();

			// Create texture for rendering lines/vertices
			CreateTexture();
			
			// We have no destructor
			GC.SuppressFinalize(this);
		}

		// Diposer
		public override void Dispose()
		{
			// Not already disposed?
			if(!isdisposed)
			{
				// Clean up
				if(tex != null) tex.Dispose();
				tex = null;
				if(thingsvertices != null) thingsvertices.Dispose();
				thingsvertices = null;
				thingtexture.Dispose();
				
				// Done
				base.Dispose();
			}
		}

		#endregion

		#region ================== Control
		
		// This draws the image on screen
		public void Present()
		{
			// Start drawing
			if(graphics.StartRendering(General.Colors.Background.ToInt()))
			{
				// Set renderstates
				graphics.Device.SetTexture(0, tex);
				graphics.Device.SetRenderState(RenderState.CullMode, Cull.None);
				graphics.Device.SetRenderState(RenderState.AlphaBlendEnable, true);
				graphics.Device.SetRenderState(RenderState.SourceBlend, Blend.SourceAlpha);
				graphics.Device.SetRenderState(RenderState.DestBlend, Blend.InvSourceAlpha);
				graphics.Device.SetRenderState(RenderState.TextureFactor, -1);
				graphics.Shaders.Base2D.Texture1 = tex;
				graphics.Shaders.Base2D.SetSettings(1f / pwidth, 1f / pheight, FSAA_BLEND_FACTOR);
				
				// Draw the lines and vertices texture
				graphics.Shaders.Base2D.Begin();
				graphics.Shaders.Base2D.BeginPass(0);
				try { graphics.Device.DrawUserPrimitives<FlatVertex>(PrimitiveType.TriangleStrip, 0, 2, screenverts); } catch(Exception) { }
				graphics.Shaders.Base2D.EndPass();
				graphics.Shaders.Base2D.End();

				// Do we have things to render?
				if((numthings > 0) && (thingsvertices != null))
				{
					// Set renderstates
					graphics.Device.SetRenderState(RenderState.CullMode, Cull.None);
					graphics.Device.SetRenderState(RenderState.AlphaBlendEnable, true);
					graphics.Device.SetRenderState(RenderState.SourceBlend, Blend.SourceAlpha);
					graphics.Device.SetRenderState(RenderState.DestBlend, Blend.InvSourceAlpha);
					graphics.Device.SetTexture(0, thingtexture.Texture);
					graphics.Shaders.Things2D.Texture1 = thingtexture.Texture;
					
					// Set the vertex buffer
					graphics.Device.SetStreamSource(0, thingsvertices, 0, FlatVertex.Stride);
					
					// Go for all things
					for(int i = 0; i < numthings; i++)
					{
						// Set renderstates
						graphics.Device.SetRenderState(RenderState.TextureFactor, thingcolors[i].ToInt());
						graphics.Shaders.Things2D.SetColors(thingcolors[i]);
						
						// Draw the thing circle
						graphics.Shaders.Things2D.Begin();
						graphics.Shaders.Things2D.BeginPass(0);
						try { graphics.Device.DrawPrimitives(PrimitiveType.TriangleStrip, i * 8, 2); } catch(Exception) { }
						graphics.Shaders.Things2D.EndPass();
						
						// Draw the thing icon
						graphics.Shaders.Things2D.BeginPass(1);
						try { graphics.Device.DrawPrimitives(PrimitiveType.TriangleStrip, i * 8 + 4, 2); } catch(Exception) { }
						graphics.Shaders.Things2D.EndPass();
						graphics.Shaders.Things2D.End();
					}
				}
				
				// Done
				graphics.FinishRendering();
			}
		}

		// This is called before a device is reset
		// (when resized or display adapter was changed)
		public override void UnloadResource()
		{
			// Trash old texture
			if(tex != null) tex.Dispose();
			tex = null;

			// Trash things buffer
			if(thingsvertices != null) thingsvertices.Dispose();
			thingsvertices = null;
			maxthings = 0;
			numthings = 0;
		}
		
		// This is called resets when the device is reset
		// (when resized or display adapter was changed)
		public override void ReloadResource()
		{
			// Re-create texture
			CreateTexture();
		}

		// This resets the graphics
		public override void Reset()
		{
			UnloadResource();
			ReloadResource();
		}

		// Allocates new image memory to render on
		public void CreateTexture()
		{
			SurfaceDescription sd;
			
			// Get new width and height
			width = graphics.RenderTarget.ClientSize.Width;
			height = graphics.RenderTarget.ClientSize.Height;
			
			// Trash old texture
			if(tex != null) tex.Dispose();
			tex = null;
			
			// Create new texture
			tex = new Texture(graphics.Device, width, height, 1, Usage.Dynamic, Format.A8R8G8B8, Pool.Default);

			// Get the real surface size
			sd = tex.GetLevelDescription(0);
			pwidth = sd.Width;
			pheight = sd.Height;

			// Setup screen vertices
			screenverts[0].x = 0.5f;
			screenverts[0].y = 0.5f;
			screenverts[0].w = 1f;
			screenverts[0].u = 1f / pwidth;
			screenverts[0].v = 1f / pheight;
			screenverts[1].x = pwidth - 1.5f;
			screenverts[1].y = 0.5f;
			screenverts[1].w = 1f;
			screenverts[1].u = 1f - 1f / pwidth;
			screenverts[1].v = 1f / pheight;
			screenverts[2].x = 0.5f;
			screenverts[2].y = pheight - 1.5f;
			screenverts[2].w = 1f;
			screenverts[2].u = 1f / pwidth;
			screenverts[2].v = 1f - 1f / pheight;
			screenverts[3].x = pwidth - 1.5f;
			screenverts[3].y = pheight - 1.5f;
			screenverts[3].w = 1f;
			screenverts[3].u = 1f - 1f / pwidth;
			screenverts[3].v = 1f - 1f / pheight;
		}

		// This begins a drawing session
		public unsafe bool StartRendering(bool cleardisplay)
		{
			LockFlags lockflags;
			
			// Do we have a texture?
			if(tex != null)
			{
				// Determine lock requirements
				if(cleardisplay) lockflags = LockFlags.Discard | LockFlags.NoSystemLock;
							else lockflags = LockFlags.NoSystemLock;

				// Lock memory
				lockedrect = tex.LockRectangle(0, lockflags);

				// Create plotter
				plotter = new Plotter((PixelColor*)lockedrect.Data.DataPointer.ToPointer(), lockedrect.Pitch / sizeof(PixelColor), pheight, width, height);
				if(cleardisplay) plotter.Clear();
				
				// Reset things buffer when display is cleared
				if(cleardisplay) ReserveThingsMemory(0, false);
				
				// Ready for rendering
				return true;
			}
			else
			{
				// Can't render!
				return false;
			}
		}
		
		// This ends a drawing session
		public void FinishRendering()
		{
			// Unlock memory
			tex.UnlockRectangle(0);
			lockedrect.Data.Dispose();
			
			// Present new image
			Present();
		}
		
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
			translatex = -offsetx + (width * 0.5f) * scaleinv;
			translatey = -offsety - (height * 0.5f) * scaleinv;
			linenormalsize = 10f * scaleinv;
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
			PixelColor[] oldcolors = null;
			
			// Do we need to make changes?
			if((newnumthings > maxthings) || !preserve)
			{
				// Read old things data if we want to keep it
				if(preserve && (thingsvertices != null) && (numthings > 0))
				{
					stream = thingsvertices.Lock(0, numthings * 8 * FlatVertex.Stride, LockFlags.ReadOnly);
					verts = stream.ReadRange<FlatVertex>(numthings * 8);
					thingsvertices.Unlock();
					stream.Dispose();
					oldcolors = thingcolors;
				}
				
				// Buffer needs to be reallocated?
				if(newnumthings > maxthings)
				{
					// Calculate new size
					newmaxthings = newnumthings + THING_BUFFER_STEP;
					
					// Trash old buffer
					if(thingsvertices != null) thingsvertices.Dispose();
					
					// Create new buffer
					thingsvertices = new VertexBuffer(graphics.Device, newmaxthings * 8 * FlatVertex.Stride, Usage.Dynamic, VertexFormat.None, Pool.Default);
					thingcolors = new PixelColor[newmaxthings];
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
					stream = thingsvertices.Lock(0, maxthings * 8 * FlatVertex.Stride, LockFlags.Discard);
					stream.WriteRange<FlatVertex>(verts);
					thingsvertices.Unlock();
					stream.Dispose();
					oldcolors.CopyTo(thingcolors, 0);
				}
				else
				{
					// Things were trashed
					numthings = 0;
				}
			}
		}

		// This makes vertices for a thing
		private void CreateThingVerts(Thing t, ref FlatVertex[] verts, int offset)
		{
			// Transform to screen coordinates
			Vector2D screenpos = ((Vector2D)t.Position).GetTransformed(translatex, translatey, scale, -scale);
			
			// Setup fixed rect for circle
			verts[offset].x = screenpos.x - THING_CIRCLE_SIZE;
			verts[offset].y = screenpos.y - THING_CIRCLE_SIZE;
			verts[offset].w = 1f;
			verts[offset].u = 1f / 512f;
			verts[offset].v = 1f / 128f;
			offset++;
			verts[offset].x = screenpos.x + THING_CIRCLE_SIZE;
			verts[offset].y = screenpos.y - THING_CIRCLE_SIZE;
			verts[offset].w = 1f;
			verts[offset].u = 0.25f - 1f / 512f;
			verts[offset].v = 1f / 128f;
			offset++;
			verts[offset].x = screenpos.x - THING_CIRCLE_SIZE;
			verts[offset].y = screenpos.y + THING_CIRCLE_SIZE;
			verts[offset].w = 1f;
			verts[offset].u = 1f / 512f;
			verts[offset].v = 1f - 1f / 128f;
			offset++;
			verts[offset].x = screenpos.x + THING_CIRCLE_SIZE;
			verts[offset].y = screenpos.y + THING_CIRCLE_SIZE;
			verts[offset].w = 1f;
			verts[offset].u = 0.25f - 1f / 512f;
			verts[offset].v = 1f - 1f / 128f;
			offset++;
			
			// Setup rotated rect for arrow
			verts[offset].x = screenpos.x + (float)Math.Sin(t.Angle - Angle2D.PI * 0.25f) * THING_ARROW_SIZE;
			verts[offset].y = screenpos.y + (float)Math.Cos(t.Angle - Angle2D.PI * 0.25f) * THING_ARROW_SIZE;
			verts[offset].w = 1f;
			verts[offset].u = 0.50f;
			verts[offset].v = 0f;
			offset++;
			verts[offset].x = screenpos.x + (float)Math.Sin(t.Angle + Angle2D.PI * 0.25f) * THING_ARROW_SIZE;
			verts[offset].y = screenpos.y + (float)Math.Cos(t.Angle + Angle2D.PI * 0.25f) * THING_ARROW_SIZE;
			verts[offset].w = 1f;
			verts[offset].u = 0.75f;
			verts[offset].v = 0f;
			offset++;
			verts[offset].x = screenpos.x + (float)Math.Sin(t.Angle - Angle2D.PI * 0.75f) * THING_ARROW_SIZE;
			verts[offset].y = screenpos.y + (float)Math.Cos(t.Angle - Angle2D.PI * 0.75f) * THING_ARROW_SIZE;
			verts[offset].w = 1f;
			verts[offset].u = 0.50f;
			verts[offset].v = 1f;
			offset++;
			verts[offset].x = screenpos.x + (float)Math.Sin(t.Angle + Angle2D.PI * 0.75f) * THING_ARROW_SIZE;
			verts[offset].y = screenpos.y + (float)Math.Cos(t.Angle + Angle2D.PI * 0.75f) * THING_ARROW_SIZE;
			verts[offset].w = 1f;
			verts[offset].u = 0.75f;
			verts[offset].v = 1f;
		}
		
		#endregion

		#region ================== Colors

		// This returns the color for a thing
		public PixelColor DetermineThingColor(Thing t)
		{
			// Determine color
			if(t.Selected > 0) return General.Colors.Selection;
			else return PixelColor.FromColor(Color.Tomato);

			// TODO: Check against game configuration or embed color into thing
		}

		// This returns the color for a vertex
		public int DetermineVertexColor(Vertex v)
		{
			// Determine color
			if(v.Selected > 0) return ColorCollection.SELECTION;
			else return ColorCollection.VERTICES;
		}

		// This returns the color for a linedef
		public PixelColor DetermineLinedefColor(Linedef l)
		{
			// Sinlgesided lines
			if((l.Back == null) || (l.Front == null))
			{
				// Determine color
				if(l.Selected > 0) return General.Colors.Selection;
				else if(l.Action != 0) return General.Colors.Actions;
				else return General.Colors.Linedefs;
			}
			// Doublesided lines
			else
			{
				// Determine color
				if(l.Selected > 0) return General.Colors.Selection;
				else if(l.Action != 0) return General.Colors.Actions.WithAlpha(DOUBLESIDED_LINE_ALPHA);
				else if((l.Flags & General.Map.Settings.SoundLinedefFlags) != 0) return General.Colors.Sounds.WithAlpha(DOUBLESIDED_LINE_ALPHA);
				else return General.Colors.Linedefs.WithAlpha(DOUBLESIDED_LINE_ALPHA);
			}
		}

		#endregion

		#region ================== Map Rendering

		// This adds a thing in the things buffer for rendering
		public void RenderThing(Thing t, PixelColor c)
		{
			FlatVertex[] verts = new FlatVertex[8];
			DataStream stream;

			// TODO: Check if the thing is actually on screen

			// Make sure there is enough memory reserved
			ReserveThingsMemory(numthings + 1, true);

			// Store the thing color
			thingcolors[numthings] = c;

			// Store vertices in buffer
			stream = thingsvertices.Lock(numthings * 8 * FlatVertex.Stride, 8 * FlatVertex.Stride, LockFlags.NoSystemLock);
			CreateThingVerts(t, ref verts, 0);
			stream.WriteRange<FlatVertex>(verts);
			thingsvertices.Unlock();
			stream.Dispose();

			// Thing added!
			numthings++;
		}

		// This adds a thing in the things buffer for rendering
		public void RenderThingSet(ICollection<Thing> things)
		{
			FlatVertex[] verts = new FlatVertex[things.Count * 8];
			DataStream stream;
			int offset = 0;
			int added = 0;
			
			// Make sure there is enough memory reserved
			ReserveThingsMemory(numthings + things.Count, true);

			// Go for all things
			foreach(Thing t in things)
			{
				// TODO: Check if the thing is actually on screen
				
				// Store the thing color
				thingcolors[numthings + offset] = DetermineThingColor(t);

				// Create vertices
				CreateThingVerts(t, ref verts, offset * 8);

				// Next
				added++;
				offset++;
			}
			
			// Store vertices in buffer
			stream = thingsvertices.Lock(numthings * 8 * FlatVertex.Stride, things.Count * 8 * FlatVertex.Stride, LockFlags.NoSystemLock);
			stream.WriteRange<FlatVertex>(verts);
			thingsvertices.Unlock();
			stream.Dispose();

			// Things added!
			numthings += added;
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
			plotter.DrawVertexSolid((int)nv.x, (int)nv.y, 2, General.Colors.Colors[colorindex], General.Colors.BrightColors[colorindex], General.Colors.DarkColors[colorindex]);
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
