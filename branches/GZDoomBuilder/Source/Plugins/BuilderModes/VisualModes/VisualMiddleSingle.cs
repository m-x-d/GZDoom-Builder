
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

namespace CodeImp.DoomBuilder.BuilderModes
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
            //mxd
            geoType = VisualGeometryType.WALL_MIDDLE;
            
            // We have no destructor
			GC.SuppressFinalize(this);
		}

		// This builds the geometry. Returns false when no geometry created.
		public override bool Setup()
		{
			//mxd
            //int brightness = mode.CalculateBrightness(Sidedef.Sector.Brightness);
            int brightness = mode.CalculateBrightness(Sidedef.Sector.Brightness, Sidedef);

			// Calculate size of this wall part
			float geotop = (float)Sidedef.Sector.CeilHeight;
			float geobottom = (float)Sidedef.Sector.FloorHeight;
			float geoheight = geotop - geobottom;
			if(geoheight > 0.001f)
			{
				Vector2D t1 = new Vector2D();
				Vector2D t2 = new Vector2D();

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
				
				// Determine texture coordinates
				// See http://doom.wikia.com/wiki/Texture_alignment
				// We just use pixels for coordinates for now
				if(Sidedef.Line.IsFlagSet(General.Map.Config.LowerUnpeggedFlag))
				{
					// When lower unpegged is set, the middle texture is bound to the bottom
					t1.y = tsz.y - geoheight;
				}
				t2.x = t1.x + Sidedef.Line.Length;
				t2.y = t1.y + geoheight;

				// Apply texture offset
				if (General.Map.Config.ScaledTextureOffsets && !base.Texture.WorldPanning)
				{
					t1 += new Vector2D(Sidedef.OffsetX * base.Texture.Scale.x, Sidedef.OffsetY * base.Texture.Scale.y);
					t2 += new Vector2D(Sidedef.OffsetX * base.Texture.Scale.x, Sidedef.OffsetY * base.Texture.Scale.y);
				}
				else
				{
					t1 += new Vector2D(Sidedef.OffsetX, Sidedef.OffsetY);
					t2 += new Vector2D(Sidedef.OffsetX, Sidedef.OffsetY);
				}

				// Transform pixel coordinates to texture coordinates
				t1 /= tsz;
				t2 /= tsz;

				// Get world coordinates for geometry
				Vector2D v1, v2;
				if(Sidedef.IsFront)
				{
					v1 = Sidedef.Line.Start.Position;
					v2 = Sidedef.Line.End.Position;
				}
				else
				{
					v1 = Sidedef.Line.End.Position;
					v2 = Sidedef.Line.Start.Position;
				}

				// Make vertices
				WorldVertex[] verts = new WorldVertex[6];
				verts[0] = new WorldVertex(v1.x, v1.y, geobottom, brightness, t1.x, t2.y);
				verts[1] = new WorldVertex(v1.x, v1.y, geotop, brightness, t1.x, t1.y);
				verts[2] = new WorldVertex(v2.x, v2.y, geotop, brightness, t2.x, t1.y);
				verts[3] = verts[0];
				verts[4] = verts[2];
				verts[5] = new WorldVertex(v2.x, v2.y, geobottom, brightness, t2.x, t2.y);

				// Keep properties
				base.top = geotop;
				base.bottom = geobottom;
				
				// Apply vertices
				base.SetVertices(verts);
				return true;
			}
			else
			{
				// No geometry for invisible wall
				base.top = geotop;
				base.bottom = geobottom;
				WorldVertex[] verts = new WorldVertex[0];
				base.SetVertices(verts);
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
