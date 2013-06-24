
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
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Geometry;
using CodeImp.DoomBuilder.Rendering;
using CodeImp.DoomBuilder.Types;
using CodeImp.DoomBuilder.VisualModes;

#endregion

namespace CodeImp.DoomBuilder.BuilderModes
{
	internal sealed class VisualMiddle3D : BaseVisualGeometrySidedef
	{
		#region ================== Constants

		#endregion
		
		#region ================== Variables

		private Effect3DFloor extrafloor;
		
		#endregion
		
		#region ================== Properties

		#endregion
		
		#region ================== Constructor / Setup
		
		// Constructor
		public VisualMiddle3D(BaseVisualMode mode, VisualSector vs, Sidedef s) : base(mode, vs, s)
		{
            //mxd
            geoType = VisualGeometryType.WALL_MIDDLE_3D;
            
            // We have no destructor
			GC.SuppressFinalize(this);
		}
		
		// This builds the geometry. Returns false when no geometry created.
		public override bool Setup() { return this.Setup(this.extrafloor); }
		public bool Setup(Effect3DFloor extrafloor)
		{
			Vector2D vl, vr;
			Sidedef sourceside = extrafloor.Linedef.Front;
			this.extrafloor = extrafloor;

            //mxd. lightfog flag support
            bool lightabsolute = Sidedef.Fields.GetValue("lightabsolute", false);
            bool ignoreUDMFLight = (!Sidedef.Fields.GetValue("lightfog", false) || !lightabsolute) && Sector.Sector.Fields.ContainsKey("fadecolor");
            int lightvalue = ignoreUDMFLight ? 0 : Sidedef.Fields.GetValue("light", 0); //mxd
            if (ignoreUDMFLight) lightabsolute = false;

			Vector2D tscale = new Vector2D(sourceside.Fields.GetValue("scalex_mid", 1.0f),
										   sourceside.Fields.GetValue("scaley_mid", 1.0f));
			Vector2D toffset1 = new Vector2D(Sidedef.Fields.GetValue("offsetx_mid", 0.0f),
											 Sidedef.Fields.GetValue("offsety_mid", 0.0f));
			Vector2D toffset2 = new Vector2D(sourceside.Fields.GetValue("offsetx_mid", 0.0f),
											 sourceside.Fields.GetValue("offsety_mid", 0.0f));
			
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
			
			//mxd. which texture we must use?
			long textureLong = 0;
			if((sourceside.Line.Args[2] & (int)Effect3DFloor.Flags.UseUpperTexture) != 0) {
				if(Sidedef.HighTexture.Length > 0 && Sidedef.HighTexture[0] != '-')
					textureLong = Sidedef.LongHighTexture;
			} else if((sourceside.Line.Args[2] & (int)Effect3DFloor.Flags.UseLowerTexture) != 0) {
				if(Sidedef.LowTexture.Length > 0 && Sidedef.LowTexture[0] != '-')
					textureLong = Sidedef.LongLowTexture;
			} else if((sourceside.MiddleTexture.Length > 0) && (sourceside.MiddleTexture[0] != '-')) {
				textureLong = sourceside.LongMiddleTexture;
			}

			// Texture given?
			//if((sourceside.MiddleTexture.Length > 0) && (sourceside.MiddleTexture[0] != '-'))
			if(textureLong != 0)
			{
				// Load texture
				base.Texture = General.Map.Data.GetTextureImage(textureLong);
				if(base.Texture == null)
				{
					base.Texture = General.Map.Data.MissingTexture3D;
					setuponloadedtexture = textureLong;
				}
				else
				{
					if(!base.Texture.IsImageLoaded)
						setuponloadedtexture = textureLong;
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
			tsz = tsz / tscale;
			
			// Get texture offsets
			Vector2D tof = new Vector2D(Sidedef.OffsetX, Sidedef.OffsetY) + new Vector2D(sourceside.OffsetX, sourceside.OffsetY);
			tof = tof + toffset1 + toffset2;
			tof = tof / tscale;
			if(General.Map.Config.ScaledTextureOffsets && !base.Texture.WorldPanning)
				tof = tof * base.Texture.Scale;
			
			// For Vavoom type 3D floors the ceiling is lower than floor and they are reversed.
			// We choose here.
			float sourcetopheight = extrafloor.VavoomType ? sourceside.Sector.FloorHeight : sourceside.Sector.CeilHeight;
			float sourcebottomheight = extrafloor.VavoomType ? sourceside.Sector.CeilHeight : sourceside.Sector.FloorHeight;
			
			// Determine texture coordinates plane as they would be in normal circumstances.
			// We can then use this plane to find any texture coordinate we need.
			// The logic here is the same as in the original VisualMiddleSingle (except that
			// the values are stored in a TexturePlane)
			// NOTE: I use a small bias for the floor height, because if the difference in
			// height is 0 then the TexturePlane doesn't work!
			TexturePlane tp = new TexturePlane();
			float floorbias = (sourcetopheight == sourcebottomheight) ? 1.0f : 0.0f;

			tp.trb.x = tp.tlt.x + Sidedef.Line.Length;
			tp.trb.y = tp.tlt.y + (sourcetopheight - sourcebottomheight) + floorbias;
			
			// Apply texture offset
			tp.tlt += tof;
			tp.trb += tof;
			
			// Transform pixel coordinates to texture coordinates
			tp.tlt /= tsz;
			tp.trb /= tsz;
			
			// Left top and right bottom of the geometry that
			tp.vlt = new Vector3D(vl.x, vl.y, sourcetopheight);
			tp.vrb = new Vector3D(vr.x, vr.y, sourcebottomheight + floorbias);
			
			// Make the right-top coordinates
			tp.trt = new Vector2D(tp.trb.x, tp.tlt.y);
			tp.vrt = new Vector3D(tp.vrb.x, tp.vrb.y, tp.vlt.z);
			
			//mxd. Get ceiling and floor heights. Use our and neighbour sector's data
			SectorData sdo = mode.GetSectorData(Sidedef.Other.Sector); 

			float flo = sdo.Floor.plane.GetZ(vl);
			float fro = sdo.Floor.plane.GetZ(vr);
			float clo = sdo.Ceiling.plane.GetZ(vl);
			float cro = sdo.Ceiling.plane.GetZ(vr);

			float fle = sd.Floor.plane.GetZ(vl);
			float fre = sd.Floor.plane.GetZ(vr);
			float cle = sd.Ceiling.plane.GetZ(vl);
			float cre = sd.Ceiling.plane.GetZ(vr);

			float fl = flo > fle ? flo : fle;
			float fr = fro > fre ? fro : fre;
			float cl = clo < cle ? clo : cle;
			float cr = cro < cre ? cro : cre;
			
			// Anything to see?
			if(((cl - fl) > 0.01f) || ((cr - fr) > 0.01f))
			{
				// Keep top and bottom planes for intersection testing
				top = extrafloor.Floor.plane;
				bottom = extrafloor.Ceiling.plane;
				
				// Create initial polygon, which is just a quad between floor and ceiling
				WallPolygon poly = new WallPolygon();
				poly.Add(new Vector3D(vl.x, vl.y, fl));
				poly.Add(new Vector3D(vl.x, vl.y, cl));
				poly.Add(new Vector3D(vr.x, vr.y, cr));
				poly.Add(new Vector3D(vr.x, vr.y, fr));
				
				// Determine initial color
				int lightlevel = lightabsolute ? lightvalue : sd.Ceiling.brightnessbelow + lightvalue;
				//mxd. This calculates light with doom-style wall shading
                PixelColor wallbrightness = PixelColor.FromInt(mode.CalculateBrightness(lightlevel, Sidedef));
				PixelColor wallcolor = PixelColor.Modulate(sd.Ceiling.colorbelow, wallbrightness);
				poly.color = wallcolor.WithAlpha(255).ToInt();

				// Cut off the part above the 3D floor and below the 3D ceiling
				CropPoly(ref poly, extrafloor.Floor.plane, false);
				CropPoly(ref poly, extrafloor.Ceiling.plane, false);

				// Cut out pieces that overlap 3D floors in this sector
				List<WallPolygon> polygons = new List<WallPolygon>(1);
				polygons.Add(poly);
				foreach(Effect3DFloor ef in sd.ExtraFloors)
				{
					//mxd. Walls of solid 3D floors shouldn't be clipped by translucent 3D floors
					if(extrafloor.RenderInside || (!extrafloor.RenderInside && !ef.RenderInside)){
						int num = polygons.Count;
						for(int pi = 0; pi < num; pi++)
						{
							// Split by floor plane of 3D floor
							WallPolygon p = polygons[pi];
							WallPolygon np = SplitPoly(ref p, ef.Ceiling.plane, true);
							
							if(np.Count > 0)
							{
								// Split part below floor by the ceiling plane of 3D floor
								// and keep only the part below the ceiling (front)
								SplitPoly(ref np, ef.Floor.plane, true);

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
				
				// Process the polygon and create vertices
				List<WorldVertex> verts = CreatePolygonVertices(polygons, tp, sd, lightvalue, lightabsolute);
				if(verts.Count > 0)
				{
					if ((extrafloor.Linedef.Args[2] & (int)Effect3DFloor.Flags.RenderAdditive) != 0)//mxd
						this.RenderPass = RenderPass.Additive;
					else if (extrafloor.Alpha < 255)
						this.RenderPass = RenderPass.Alpha;
					else
						this.RenderPass = RenderPass.Mask;
					
					if(extrafloor.Alpha < 255)
					{
						// Apply alpha to vertices
						byte alpha = (byte)General.Clamp(extrafloor.Alpha, 0, 255);
						if(alpha < 255)
						{
							for(int i = 0; i < verts.Count; i++)
							{
								WorldVertex v = verts[i];
								PixelColor c = PixelColor.FromInt(v.c);
								v.c = c.WithAlpha(alpha).ToInt();
								verts[i] = v;
							}
						}
					}
					
					base.SetVertices(verts);
					return true;
				}
			}
			
			return false;
		}
		
		#endregion
		
		#region ================== Methods

		// Return texture name
		public override string GetTextureName()
		{
			//mxd
			if((extrafloor.Linedef.Args[2] & (int)Effect3DFloor.Flags.UseUpperTexture) != 0)
				return Sidedef.HighTexture;
			if((extrafloor.Linedef.Args[2] & (int)Effect3DFloor.Flags.UseLowerTexture) != 0)
				return Sidedef.LowTexture;
			return extrafloor.Linedef.Front.MiddleTexture;
		}

		// This changes the texture
		protected override void SetTexture(string texturename)
		{
			//mxd
			if ((extrafloor.Linedef.Args[2] & (int)Effect3DFloor.Flags.UseUpperTexture) != 0)
				Sidedef.SetTextureHigh(texturename);
			if ((extrafloor.Linedef.Args[2] & (int)Effect3DFloor.Flags.UseLowerTexture) != 0)
				Sidedef.SetTextureLow(texturename);
			else
				extrafloor.Linedef.Front.SetTextureMid(texturename);

			General.Map.Data.UpdateUsedTextures();
			this.Sector.Rebuild();

			//mxd. Other sector also may require updating
			SectorData sd = mode.GetSectorData(Sidedef.Other.Sector);
			if(sd.ExtraFloors.Count > 0)
				((BaseVisualSector)mode.GetVisualSector(Sidedef.Other.Sector)).Rebuild();

			//mxd. As well as model sector
			((BaseVisualSector)mode.GetVisualSector(extrafloor.Linedef.Front.Sector)).UpdateSectorGeometry(false);
		}

		protected override void SetTextureOffsetX(int x)
		{
			Sidedef.Fields.BeforeFieldsChange();
			Sidedef.Fields["offsetx_mid"] = new UniValue(UniversalType.Float, (float)x);
		}

		protected override void SetTextureOffsetY(int y)
		{
			Sidedef.Fields.BeforeFieldsChange();
			Sidedef.Fields["offsety_mid"] = new UniValue(UniversalType.Float, (float)y);
		}

		protected override void MoveTextureOffset(Point xy)
		{
			Sidedef.Fields.BeforeFieldsChange();
			float oldx = Sidedef.Fields.GetValue("offsetx_mid", 0.0f);
			float oldy = Sidedef.Fields.GetValue("offsety_mid", 0.0f);
			Sidedef.Fields["offsetx_mid"] = new UniValue(UniversalType.Float, oldx + (float)xy.X);
			Sidedef.Fields["offsety_mid"] = new UniValue(UniversalType.Float, oldy + (float)xy.Y);
		}

		protected override Point GetTextureOffset()
		{
			float oldx = Sidedef.Fields.GetValue("offsetx_mid", 0.0f);
			float oldy = Sidedef.Fields.GetValue("offsety_mid", 0.0f);
			return new Point((int)oldx, (int)oldy);
		}

        //mxd
        public override Linedef GetControlLinedef() {
            return extrafloor.Linedef;
        }
		
		#endregion
	}
}
