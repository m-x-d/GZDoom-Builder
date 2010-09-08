
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
			SectorData sd = mode.GetSectorData(Sidedef.Sector);
			SectorData osd = mode.GetSectorData(Sidedef.Other.Sector);

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
			
			//
			// - Geometry is horizontally split and ranges from one layer down to the next layer.
			//   This is repeated for all layers.
			//
			// - We create geometry from the floor up to the floor of the other sector.
			//
			// - When the two layers intersect over Z, the geometry should not span the entire
			//   width, but only up to the split position. The back side will handle the other
			//   side of the split, if needed.
			//

			// Heights of the floor on the other side
			float ol = osd.Floor.plane.GetZ(tp.vlt);
			float or = osd.Floor.plane.GetZ(tp.vrt);
			
			// Go for all levels to build geometry
			List<WorldVertex> verts = new List<WorldVertex>();
			for(int i = 0; i < (sd.Levels.Count - 1); i++)
			{
				SectorLevel lb = sd.Levels[i];
				SectorLevel lt = sd.Levels[i + 1];

				PixelColor wallbrightness = PixelColor.FromInt(mode.CalculateBrightness(lt.brightnessbelow));
				PixelColor wallcolor = PixelColor.Modulate(lt.colorbelow, wallbrightness);
				int c = wallcolor.WithAlpha(255).ToInt();
				
				// Get corner heights on the two planes
				float lbl = lb.plane.GetZ(tp.vlt);
				float lbr = lb.plane.GetZ(tp.vrt);
				float ltl = lt.plane.GetZ(tp.vlt);
				float ltr = lt.plane.GetZ(tp.vrt);

				// When both corners are above the heights of the floor on
				// the other side, then we can stop building.
				if((lbl > ol) && (lbr > or))
					break;
				
				// Make coordinates for the corners
				Vector3D vlb = new Vector3D(tp.vlt.x, tp.vlt.y, lbl);
				Vector3D vlt = new Vector3D(tp.vlt.x, tp.vlt.y, ltl);
				Vector3D vrb = new Vector3D(tp.vrb.x, tp.vrb.y, lbr);
				Vector3D vrt = new Vector3D(tp.vrt.x, tp.vrt.y, ltr);
				
				// Compare corner heights to see if we should split
				if((lbl < ltl) && (lbr >= ltr))
				{
					// Split vertically with geometry on the left
					float u_ray = 1.0f;
					lb.plane.GetIntersection(vlt, vrt, ref u_ray);
					Vector3D vs = vlt + (vrt - vlt) * u_ray;
					Vector2D tlb = tp.GetTextureCoordsAt(vlb);
					Vector2D tlt = tp.GetTextureCoordsAt(vlt);
					Vector2D ts = tp.GetTextureCoordsAt(vs);
					verts.Add(new WorldVertex(vlb.x, vlb.y, vlb.z, c, tlb.x, tlb.y));
					verts.Add(new WorldVertex(vlt.x, vlt.y, vlt.z, c, tlt.x, tlt.y));
					verts.Add(new WorldVertex(vs.x, vs.y, vs.z, c, ts.x, ts.y));
				}
				else if((lbl >= ltl) && (lbr < ltr))
				{
					// Split vertically with geometry on the right
					float u_ray = 0.0f;
					lb.plane.GetIntersection(vlt, vrt, ref u_ray);
					Vector3D vs = vlt + (vrt - vlt) * u_ray;
					Vector2D trb = tp.GetTextureCoordsAt(vrb);
					Vector2D trt = tp.GetTextureCoordsAt(vrt);
					Vector2D ts = tp.GetTextureCoordsAt(vs);
					verts.Add(new WorldVertex(vs.x, vs.y, vs.z, c, ts.x, ts.y));
					verts.Add(new WorldVertex(vrt.x, vrt.y, vrt.z, c, trt.x, trt.y));
					verts.Add(new WorldVertex(vrb.x, vrb.y, vrb.z, c, trb.x, trb.y));
				}
				else if((lbl < ltl) && (lbr < ltr))
				{
					// Span entire width
					Vector2D tlb = tp.GetTextureCoordsAt(vlb);
					Vector2D tlt = tp.GetTextureCoordsAt(vlt);
					Vector2D trb = tp.GetTextureCoordsAt(vrb);
					Vector2D trt = tp.GetTextureCoordsAt(vrt);
					verts.Add(new WorldVertex(vlb.x, vlb.y, vlb.z, c, tlb.x, tlb.y));
					verts.Add(new WorldVertex(vlt.x, vlt.y, vlt.z, c, tlt.x, tlt.y));
					verts.Add(new WorldVertex(vrt.x, vrt.y, vrt.z, c, trt.x, trt.y));
					verts.Add(new WorldVertex(vlb.x, vlb.y, vlb.z, c, tlb.x, tlb.y));
					verts.Add(new WorldVertex(vrt.x, vrt.y, vrt.z, c, trt.x, trt.y));
					verts.Add(new WorldVertex(vrb.x, vrb.y, vrb.z, c, trb.x, trb.y));
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
