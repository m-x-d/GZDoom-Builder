
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

#endregion

namespace CodeImp.DoomBuilder.BuilderModes.Editing
{
	internal class VisualCeiling : VisualGeometry
	{
		#region ================== Constants

		#endregion

		#region ================== Variables

		#endregion

		#region ================== Properties

		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		public VisualCeiling(Sector s)
		{
			WorldVertex[] verts;
			WorldVertex v;

			// Load floor texture
			base.Texture = General.Map.Data.GetFlatImage(s.LongCeilTexture);
			base.Texture.LoadImage();

			// Make vertices
			verts = new WorldVertex[s.Vertices.Length];
			for(int i = 0; i < s.Vertices.Length; i++)
			{
				// Use sector brightness for color shading
				//pc = new PixelColor(255, unchecked((byte)s.Brightness), unchecked((byte)s.Brightness), unchecked((byte)s.Brightness));
				//verts[i].c = pc.ToInt();
				verts[i].c = -1;

				// Grid aligned texture coordinates
				verts[i].u = s.Vertices[i].x / base.Texture.ScaledWidth;
				verts[i].v = s.Vertices[i].y / base.Texture.ScaledHeight;

				// Vertex coordinates
				verts[i].x = s.Vertices[i].x;
				verts[i].y = s.Vertices[i].y;
				verts[i].z = (float)s.CeilHeight;
			}

			// The sector triangulation created clockwise triangles that
			// are right up for the floor. For the ceiling we must flip
			// the triangles upside down.
			// Swap some vertices to flip all triangles
			for(int i = 0; i < verts.Length; i += 3)
			{
				// Swap
				v = verts[i];
				verts[i] = verts[i + 1];
				verts[i + 1] = v;
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
