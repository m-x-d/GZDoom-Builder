
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
	internal sealed class VisualUpper : BaseVisualGeometrySidedef
	{
		#region ================== Constants

		#endregion

		#region ================== Variables

		#endregion

		#region ================== Properties

		#endregion

		#region ================== Constructor / Setup

		// Constructor
		public VisualUpper(VisualSector vs, Sidedef s) : base(vs, s)
		{
			// We have no destructor
			GC.SuppressFinalize(this);
		}

		// This builds the geometry. Returns false when no geometry created.
		public bool Setup()
		{
			// Calculate size of this wall part
			float geotop = (float)Sidedef.Sector.CeilHeight;
			float geobottom = (float)Sidedef.Other.Sector.CeilHeight;
			float geoheight = geotop - geobottom;
			if(geoheight > 0.001f)
			{
				Vector2D t1 = new Vector2D();
				Vector2D t2 = new Vector2D();

				// Texture given?
				if((Sidedef.HighTexture.Length > 0) && (Sidedef.HighTexture[0] != '-'))
				{
					// Load texture
					base.Texture = General.Map.Data.GetTextureImage(Sidedef.LongHighTexture);
					if(base.Texture == null) base.Texture = General.Map.Data.MissingTexture3D;
				}
				else
				{
					// Use missing texture
					base.Texture = General.Map.Data.MissingTexture3D;
				}

				// Get texture scaled size
				Vector2D tsz = new Vector2D(base.Texture.ScaledWidth, base.Texture.ScaledHeight);

				// Determine texture coordinates
				// See http://doom.wikia.com/wiki/Texture_alignment
				// We just use pixels for coordinates for now
				if(!Sidedef.Line.IsFlagSet(General.Map.Config.UpperUnpeggedFlag))
				{
					// When upper unpegged is NOT set, the upper texture is bound to the bottom
					t1.y = tsz.y - geoheight;
				}
				t2.x = t1.x + Sidedef.Line.Length;
				t2.y = t1.y + geoheight;

				// Apply texture offset
				t1 += new Vector2D(Sidedef.OffsetX, Sidedef.OffsetY);
				t2 += new Vector2D(Sidedef.OffsetX, Sidedef.OffsetY);

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

				// Use sector brightness for color shading
				PixelColor pc = new PixelColor(255, unchecked((byte)Sidedef.Sector.Brightness),
													unchecked((byte)Sidedef.Sector.Brightness),
													unchecked((byte)Sidedef.Sector.Brightness));

				// Make vertices
				WorldVertex[] verts = new WorldVertex[6];
				verts[0] = new WorldVertex(v1.x, v1.y, geobottom, pc.ToInt(), t1.x, t2.y);
				verts[1] = new WorldVertex(v1.x, v1.y, geotop, pc.ToInt(), t1.x, t1.y);
				verts[2] = new WorldVertex(v2.x, v2.y, geotop, pc.ToInt(), t2.x, t1.y);
				verts[3] = verts[0];
				verts[4] = verts[2];
				verts[5] = new WorldVertex(v2.x, v2.y, geobottom, pc.ToInt(), t2.x, t2.y);
				
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
				return false;
			}
		}
		
		#endregion

		#region ================== Methods
		
		#endregion
	}
}
