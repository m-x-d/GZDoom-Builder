
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
	internal class VisualUpper : VisualSidedef
	{
		#region ================== Constants

		#endregion

		#region ================== Variables

		#endregion

		#region ================== Properties

		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		public VisualUpper(Sidedef s) : base(s)
		{
			WorldVertex[] verts;
			float geotop;
			float geobottom;
			float geoheight;
			Vector2D v1, v2;

			// Calculate size of this wall part
			geotop = (float)s.Sector.CeilHeight;
			geobottom = (float)s.Other.Sector.CeilHeight;
			geoheight = geotop - geobottom;
			if(geoheight > 0.001f)
			{
				// Texture given?
				if((s.HighTexture.Length > 0) && (s.HighTexture[0] != '-'))
				{
					// Load texture
					base.Texture = General.Map.Data.GetTextureImage(s.LongHighTexture);
					if(base.Texture == null) base.Texture = General.Map.Data.MissingTexture3D;
				}
				else
				{
					// Use missing texture
					base.Texture = General.Map.Data.MissingTexture3D;
				}

				// Get coordinates
				if(s.IsFront)
				{
					v1 = s.Line.Start.Position;
					v2 = s.Line.End.Position;
				}
				else
				{
					v1 = s.Line.End.Position;
					v2 = s.Line.Start.Position;
				}

				// Use sector brightness for color shading
				PixelColor pc = new PixelColor(255, unchecked((byte)s.Sector.Brightness), unchecked((byte)s.Sector.Brightness), unchecked((byte)s.Sector.Brightness));

				// Make vertices
				verts = new WorldVertex[6];
				verts[0] = new WorldVertex(v1.x, v1.y, geobottom, pc.ToInt(), 0.0f, 1.0f);
				verts[1] = new WorldVertex(v1.x, v1.y, geotop, pc.ToInt(), 0.0f, 0.0f);
				verts[2] = new WorldVertex(v2.x, v2.y, geotop, pc.ToInt(), 1.0f, 0.0f);
				verts[3] = verts[0];
				verts[4] = verts[2];
				verts[5] = new WorldVertex(v2.x, v2.y, geobottom, pc.ToInt(), 1.0f, 1.0f);
			}
			else
			{
				// No geometry for invisible wall
				verts = new WorldVertex[0];
			}

			// Apply vertices
			base.SetVertices(verts);

			// We have no destructor
			GC.SuppressFinalize(this);
		}

		#endregion

		#region ================== Methods

		#endregion
	}
}
