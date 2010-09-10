
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
using CodeImp.DoomBuilder.Geometry;
using System.Drawing.Imaging;
using CodeImp.DoomBuilder.Data;
using CodeImp.DoomBuilder.Editing;
using CodeImp.DoomBuilder.IO;
using CodeImp.DoomBuilder.Rendering;
using CodeImp.DoomBuilder.VisualModes;

#endregion

namespace CodeImp.DoomBuilder.GZDoomEditing
{
	internal sealed class VisualMiddleSingle : BaseVisualGeometrySidedef
	{
		#region ================== Constants

		#endregion
		
		#region ================== Variables

		#endregion
		
		#region ================== Properties

		#endregion
		
		#region ================== Constructor / Setup
		
		// Constructor
		public VisualMiddleSingle(BaseVisualMode mode, VisualSector vs, Sidedef s) : base(mode, vs, s)
		{
			// We have no destructor
			GC.SuppressFinalize(this);
		}
		
		// This builds the geometry. Returns false when no geometry created.
		public override bool Setup()
		{
			Vector2D vl, vr;
			List<WallPolygon> polygons = new List<WallPolygon>(2);
			
			// Left and right vertices for this sidedef
			if(Sidedef.IsFront)
			{
				vl = new Vector2D(Sidedef.Line.Start.Position.x, Sidedef.Line.Start.Position.y);
				vr = new Vector2D(Sidedef.Line.End.Position.x, Sidedef.Line.End.Position.y);
			}
			else
			{
				vl = new Vector2D(Sidedef.Line.End.Position.x, Sidedef.Line.End.Position.y);
				vr = new Vector2D(Sidedef.Line.Start.Position.x, Sidedef.Line.Start.Position.y);
			}

			// Load sector data
			SectorData sd = mode.GetSectorData(Sidedef.Sector);
			
			// Texture given?
			if((Sidedef.MiddleTexture.Length > 0) && (Sidedef.MiddleTexture[0] != '-'))
			{
				// Load texture
				base.Texture = General.Map.Data.GetTextureImage(Sidedef.LongMiddleTexture);
				if(base.Texture == null)
				{
					base.Texture = General.Map.Data.MissingTexture3D;
					setuponloadedtexture = Sidedef.LongMiddleTexture;
				}
				else
				{
					if(!base.Texture.IsImageLoaded)
						setuponloadedtexture = Sidedef.LongMiddleTexture;
				}
			}
			else
			{
				// Use missing texture
				base.Texture = General.Map.Data.MissingTexture3D;
				setuponloadedtexture = 0;
			}
			
			// Get texture scaled size
			Vector2D tsz = new Vector2D(base.Texture.ScaledWidth, base.Texture.ScaledHeight);
			
			// Determine texture coordinates plane as they would be in normal circumstances.
			// We can then use this plane to find any texture coordinate we need.
			// The logic here is the same as in the original VisualMiddleSingle (except that
			// the values are stored in a TexturePlane)
			// NOTE: I use a small bias for the floor height, because if the difference in
			// height is 0 then the TexturePlane doesn't work!
			TexturePlane tp = new TexturePlane();
			float floorbias = (Sidedef.Sector.CeilHeight == Sidedef.Sector.FloorHeight) ? 1.0f : 0.0f;
			if(Sidedef.Line.IsFlagSet(General.Map.Config.LowerUnpeggedFlag))
			{
				// When lower unpegged is set, the middle texture is bound to the bottom
				tp.tlt.y = tsz.y - (float)(Sidedef.Sector.CeilHeight - Sidedef.Sector.FloorHeight);
			}
			tp.trb.x = tp.tlt.x + Sidedef.Line.Length;
			tp.trb.y = tp.tlt.y + ((float)Sidedef.Sector.CeilHeight - ((float)Sidedef.Sector.FloorHeight + floorbias));
			
			// Apply texture offset
			if (General.Map.Config.ScaledTextureOffsets && !base.Texture.WorldPanning)
			{
				tp.tlt += new Vector2D(Sidedef.OffsetX * base.Texture.Scale.x, Sidedef.OffsetY * base.Texture.Scale.y);
				tp.trb += new Vector2D(Sidedef.OffsetX * base.Texture.Scale.x, Sidedef.OffsetY * base.Texture.Scale.y);
			}
			else
			{
				tp.tlt += new Vector2D(Sidedef.OffsetX, Sidedef.OffsetY);
				tp.trb += new Vector2D(Sidedef.OffsetX, Sidedef.OffsetY);
			}
			
			// Transform pixel coordinates to texture coordinates
			tp.tlt /= tsz;
			tp.trb /= tsz;
			
			// Left top and right bottom of the geometry that
			tp.vlt = new Vector3D(vl.x, vl.y, (float)Sidedef.Sector.CeilHeight);
			tp.vrb = new Vector3D(vr.x, vr.y, (float)Sidedef.Sector.FloorHeight + floorbias);
			
			// Make the right-top coordinates
			tp.trt = new Vector2D(tp.trb.x, tp.tlt.y);
			tp.vrt = new Vector3D(tp.vrb.x, tp.vrb.y, tp.vlt.z);
			
			// Create initial polygon, which is just a quad between floor and ceiling
			WallPolygon poly = new WallPolygon();
			poly.Add(new Vector3D(vl.x, vl.y, sd.Floor.plane.GetZ(vl)));
			poly.Add(new Vector3D(vl.x, vl.y, sd.Ceiling.plane.GetZ(vl)));
			poly.Add(new Vector3D(vr.x, vr.y, sd.Ceiling.plane.GetZ(vr)));
			poly.Add(new Vector3D(vr.x, vr.y, sd.Floor.plane.GetZ(vr)));

			// Determine initial color
			PixelColor wallbrightness = PixelColor.FromInt(mode.CalculateBrightness(sd.Ceiling.brightnessbelow));
			PixelColor wallcolor = PixelColor.Modulate(sd.Ceiling.colorbelow, wallbrightness);
			poly.color = wallcolor.WithAlpha(255).ToInt();
			
			polygons.Add(poly);
			
			// Go for all levels to build geometry
			for(int i = 0; i < sd.Levels.Count; i++)
			{
				SectorLevel l = sd.Levels[i];
				if((l != sd.Floor) && (l != sd.Ceiling) && (l.type != SectorLevelType.Floor))
				{
					// Go for all polygons
					int num = polygons.Count;
					for(int pi = 0; pi < num; pi++)
					{
						// Split by plane
						WallPolygon p = polygons[pi];
						WallPolygon np = SplitPoly(ref p, l.plane, false);
						if(np.Count > 0)
						{
							// Determine color
							wallbrightness = PixelColor.FromInt(mode.CalculateBrightness(l.brightnessbelow));
							wallcolor = PixelColor.Modulate(l.colorbelow, wallbrightness);
							np.color = wallcolor.WithAlpha(255).ToInt();

							if(p.Count == 0)
							{
								polygons[pi] = np;
							}
							else
							{
								polygons[pi] = p;
								polygons.Add(np);
							}
						}
						else
						{
							polygons[pi] = p;
						}
					}
				}
			}
			
			// Go for all polygons to make geometry
			List<WorldVertex> verts = new List<WorldVertex>();
			foreach(WallPolygon p in polygons)
			{
				// Find texture coordinates for each vertex in the polygon
				List<Vector2D> texc = new List<Vector2D>(p.Count);
				foreach(Vector3D v in p)
					texc.Add(tp.GetTextureCoordsAt(v));
				
				// Now we create triangles from the polygon.
				// The polygon is convex and clockwise, so this is a piece of cake.
				if(p.Count >= 3)
				{
					for(int k = 1; k < (p.Count - 1); k++)
					{
						verts.Add(new WorldVertex(p[0], p.color, texc[0]));
						verts.Add(new WorldVertex(p[k], p.color, texc[k]));
						verts.Add(new WorldVertex(p[k + 1], p.color, texc[k + 1]));
					}
				}
			}
			
			if(verts.Count > 0)
			{
				base.SetVertices(verts);
				return true;
			}
			else
			{
				return false;
			}
		}
		
		#endregion
		
		#region ================== Methods

		// Return texture name
		public override string GetTextureName()
		{
			return this.Sidedef.MiddleTexture;
		}

		// This changes the texture
		protected override void SetTexture(string texturename)
		{
			this.Sidedef.SetTextureMid(texturename);
			General.Map.Data.UpdateUsedTextures();
			this.Setup();
		}
		
		#endregion
	}
}
