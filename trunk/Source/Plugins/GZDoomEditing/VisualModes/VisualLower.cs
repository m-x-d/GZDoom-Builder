
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
	internal sealed class VisualLower : BaseVisualGeometrySidedef
	{
		#region ================== Constants

		#endregion

		#region ================== Variables
		
		#endregion

		#region ================== Properties

		#endregion

		#region ================== Constructor / Setup

		// Constructor
		public VisualLower(BaseVisualMode mode, VisualSector vs, Sidedef s) : base(mode, vs, s)
		{
			// We have no destructor
			GC.SuppressFinalize(this);
		}

		// This builds the geometry. Returns false when no geometry created.
		public override bool Setup()
		{
			SectorData sd = Sector.Data;
			SectorData osd = mode.GetSectorData(Sidedef.Other.Sector);
			if(!osd.Built) osd.BuildLevels(mode);
			
			// Texture given?
			if((Sidedef.LowTexture.Length > 0) && (Sidedef.LowTexture[0] != '-'))
			{
				// Load texture
				base.Texture = General.Map.Data.GetTextureImage(Sidedef.LongLowTexture);
				if(base.Texture == null)
				{
					base.Texture = General.Map.Data.MissingTexture3D;
					setuponloadedtexture = Sidedef.LongLowTexture;
				}
				else
				{
					if(!base.Texture.IsImageLoaded)
						setuponloadedtexture = Sidedef.LongLowTexture;
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
			TexturePlane tp = new TexturePlane();
			if(Sidedef.Line.IsFlagSet(General.Map.Config.LowerUnpeggedFlag))
			{
				// When lower unpegged is set, the lower texture is bound to the bottom
				tp.tlt.y = (float)Sidedef.Sector.CeilHeight - (float)Sidedef.Other.Sector.FloorHeight;
			}
			tp.trb.x = tp.tlt.x + Sidedef.Line.Length;
			tp.trb.y = tp.tlt.y + (float)(Sidedef.Other.Sector.FloorHeight - Sidedef.Sector.FloorHeight);

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
			if(Sidedef.IsFront)
			{
				tp.vlt = new Vector3D(Sidedef.Line.Start.Position.x, Sidedef.Line.Start.Position.y, Sidedef.Other.Sector.FloorHeight);
				tp.vrb = new Vector3D(Sidedef.Line.End.Position.x, Sidedef.Line.End.Position.y, Sidedef.Sector.FloorHeight);
			}
			else
			{
				tp.vlt = new Vector3D(Sidedef.Line.End.Position.x, Sidedef.Line.End.Position.y, Sidedef.Other.Sector.FloorHeight);
				tp.vrb = new Vector3D(Sidedef.Line.Start.Position.x, Sidedef.Line.Start.Position.y, Sidedef.Sector.FloorHeight);
			}
			
			// Make the right-top coordinates
			tp.trt = new Vector2D(tp.trb.x, tp.tlt.y);
			tp.vrt = new Vector3D(tp.vrb.x, tp.vrb.y, tp.vlt.z);
			
			// Heights of the floor on the other side
			float ol = osd.Floor.plane.GetZ(tp.vlt);
			float or = osd.Floor.plane.GetZ(tp.vrt);
			Vector3D vol = new Vector3D(tp.vlt.x, tp.vlt.y, ol);
			Vector3D vor = new Vector3D(tp.vrt.x, tp.vrt.y, or);
			
			if(Sidedef.Index == 215)
			{
				int g = 5;
			}
			
			// Go for all levels to build geometry
			List<WorldVertex> verts = new List<WorldVertex>();
			for(int i = 0; i < (sd.Levels.Count - 1); i++)
			{
				SectorLevel lb = sd.Levels[i];
				SectorLevel lt = sd.Levels[i + 1];
				
				PixelColor wallbrightness = PixelColor.FromInt(mode.CalculateBrightness(lt.brightnessbelow));
				PixelColor wallcolor = PixelColor.Modulate(lt.colorbelow, wallbrightness);
				int c = wallcolor.WithAlpha(255).ToInt();
				
				// Create initial polygon between the two planes
				List<Vector3D> poly = new List<Vector3D>();
				poly.Add(new Vector3D(tp.vlt.x, tp.vlt.y, lb.plane.GetZ(tp.vlt)));
				poly.Add(new Vector3D(tp.vlt.x, tp.vlt.y, lt.plane.GetZ(tp.vlt)));
				poly.Add(new Vector3D(tp.vrt.x, tp.vrt.y, lt.plane.GetZ(tp.vrt)));
				poly.Add(new Vector3D(tp.vrb.x, tp.vrb.y, lb.plane.GetZ(tp.vrt)));
				
				// Slice off the part above the other plane
				SlicePoly(poly, osd.Floor.plane, false);
				
				// Now we go for all planes to splice this polygon
				for(int k = 0; k < sd.Levels.Count; k++)
				{
					if((k != i) && (k != (i + 1)))
						SlicePoly(poly, sd.Levels[k].plane, (k > i) || (sd.Levels[k].type == SectorLevelType.Floor));
				}
				
				// Find texture coordinates for each vertex in the polygon
				List<Vector2D> texc = new List<Vector2D>(poly.Count);
				foreach(Vector3D v in poly)
					texc.Add(tp.GetTextureCoordsAt(v));
				
				// Now we create triangles from the polygon
				if(poly.Count >= 3)
				{
					for(int k = 1; k < (poly.Count - 1); k++)
					{
						verts.Add(new WorldVertex(poly[0], c, texc[0]));
						verts.Add(new WorldVertex(poly[k], c, texc[k]));
						verts.Add(new WorldVertex(poly[k + 1], c, texc[k + 1]));
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
		
		// This slices a polygon with a plane and keeps only a certain part of the polygon
		private void SlicePoly(List<Vector3D> poly, Plane p, bool keepfront)
		{
			const float NEAR_ZERO = 0.0001f;
			
			// TODO: We can optimize this by making a list of vertices in the first iteration which
			// indicates which vertices are on the back side. Then we don't need to calculate p.Distance(v)
			// again in the second iteration.
			
			// First split lines that cross the plane so that we have vertices on the plane where the lines cross
			for(int i = 0; i < poly.Count; i++)
			{
				Vector3D v1 = poly[i];
				Vector3D v2 = (i == (poly.Count - 1)) ? poly[0] : poly[i+1];
				
				// Determine side of plane
				float side0 = p.Distance(v1);
				float side1 = p.Distance(v2);
				
				// Vertices on different side of plane?
				if((side0 < -NEAR_ZERO) && (side1 > NEAR_ZERO) ||
				   (side0 > NEAR_ZERO) && (side1 < -NEAR_ZERO))
				{
					// Split line with plane and insert the vertex
					float u = 0.0f;
					p.GetIntersection(v1, v2, ref u);
					Vector3D v3 = v1 + (v2 - v1) * u;
					poly.Insert(++i, v3);
				}
			}
			
			// Now we discard all vertices on the back side of the plane
			int k = poly.Count - 1;
			while(k >= 0)
			{
				float side = p.Distance(poly[k]);
				if(((side < -NEAR_ZERO) && keepfront) || ((side > NEAR_ZERO) && !keepfront))
					poly.RemoveAt(k);
				k--;
			}
		}
		
		#endregion

		#region ================== Methods

		// Return texture name
		public override string GetTextureName()
		{
			return this.Sidedef.LowTexture;
		}
		
		// This changes the texture
		protected override void SetTexture(string texturename)
		{
			this.Sidedef.SetTextureLow(texturename);
			General.Map.Data.UpdateUsedTextures();
			this.Setup();
		}
		
		#endregion
	}
}
