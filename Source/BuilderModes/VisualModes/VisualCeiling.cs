
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
	internal sealed class VisualCeiling : BaseVisualGeometrySector
	{
		#region ================== Constants

		#endregion

		#region ================== Variables

		#endregion

		#region ================== Properties

		#endregion

		#region ================== Constructor / Setup

		// Constructor
		public VisualCeiling(BaseVisualMode mode, VisualSector vs) : base(mode, vs)
		{
			// We have no destructor
			GC.SuppressFinalize(this);
		}

		// This builds the geometry. Returns false when no geometry created.
		public bool Setup()
		{
			WorldVertex[] verts;
			WorldVertex v;
			Sector s = base.Sector.Sector;
			
			// Load floor texture
			base.Texture = General.Map.Data.GetFlatImage(s.LongCeilTexture);
			if(base.Texture == null) base.Texture = General.Map.Data.MissingTexture3D;

			// Make vertices
			verts = new WorldVertex[s.Triangles.Vertices.Count];
			for(int i = 0; i < s.Triangles.Vertices.Count; i++)
			{
				// Use sector brightness for color shading
				PixelColor pc = new PixelColor(255, unchecked((byte)s.Brightness),
													unchecked((byte)s.Brightness),
													unchecked((byte)s.Brightness));
				verts[i].c = pc.ToInt();
				//verts[i].c = -1;

				// Grid aligned texture coordinates
				if(base.Texture.IsImageLoaded)
				{
					verts[i].u = s.Triangles.Vertices[i].x / base.Texture.ScaledWidth;
					verts[i].v = -s.Triangles.Vertices[i].y / base.Texture.ScaledHeight;
				}
				else
				{
					verts[i].u = s.Triangles.Vertices[i].x / 64;
					verts[i].v = -s.Triangles.Vertices[i].y / 64;
				}
				
				// Vertex coordinates
				verts[i].x = s.Triangles.Vertices[i].x;
				verts[i].y = s.Triangles.Vertices[i].y;
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
			return (verts.Length > 0);
		}
		
		#endregion

		#region ================== Methods

		// Paste texture
		public override void OnPasteTexture()
		{
			if(mode.CopiedFlat != null)
			{
				General.Map.UndoRedo.CreateUndo("Paste ceiling " + mode.CopiedFlat);
				SetTexture(mode.CopiedFlat);
				this.Setup();
			}
		}
		
		// This changes the height
		protected override void ChangeHeight(int amount)
		{
			General.Map.UndoRedo.CreateUndo("Change ceiling height", UndoGroup.CeilingHeightChange, this.Sector.Sector.Index);
			this.Sector.Sector.CeilHeight += amount;
		}
		
		// This performs a fast test in object picking
		public override bool PickFastReject(Vector3D from, Vector3D to, Vector3D dir)
		{
			float planez = (float)Sector.Sector.CeilHeight;

			// Check if line crosses the z height
			if((from.z < planez) && (to.z > planez))
			{
				// Calculate intersection point using the z height
				pickrayu = (planez - from.z) / (to.z - from.z);
				pickintersect = from + (to - from) * pickrayu;
				
				// Intersection point within bbox?
				RectangleF bbox = Sector.Sector.BBox;
				return ((pickintersect.x >= bbox.Left) && (pickintersect.x <= bbox.Right) &&
						(pickintersect.y >= bbox.Top) && (pickintersect.y <= bbox.Bottom));
			}
			else
			{
				// Not even crossing the z height (or not in the right direction)
				return false;
			}
		}
		
		// This performs an accurate test for object picking
		public override bool PickAccurate(Vector3D from, Vector3D to, Vector3D dir, ref float u_ray)
		{
			u_ray = pickrayu;
			
			// Check on which side of the nearest sidedef we are
			Sidedef sd = MapSet.NearestSidedef(Sector.Sector.Sidedefs, pickintersect);
			float side = sd.Line.SideOfLine(pickintersect);
			return (((side <= 0.0f) && sd.IsFront) || ((side > 0.0f) && !sd.IsFront));
		}

		// Return texture name
		public override string GetTextureName()
		{
			return this.Sector.Sector.CeilTexture;
		}

		// This changes the texture
		protected override void SetTexture(string texturename)
		{
			this.Sector.Sector.SetCeilTexture(texturename);
			General.Map.Data.UpdateUsedTextures();
			this.Setup();
		}
		
		#endregion
	}
}
