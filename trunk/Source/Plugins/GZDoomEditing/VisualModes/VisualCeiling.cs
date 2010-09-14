
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
using System.Collections.ObjectModel;
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
using CodeImp.DoomBuilder.Windows;

#endregion

namespace CodeImp.DoomBuilder.GZDoomEditing
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
		public override bool Setup(SectorLevel level)
		{
			WorldVertex[] verts;
			WorldVertex v;
			Sector s = level.sector;
			float xpan, ypan, xscale, yscale, rotate;
			Vector2D texscale;

			base.Setup(level);

			try
			{
				// Fetch ZDoom fields
				xpan = s.Fields.ContainsKey("xpanningceiling") ? (float)s.Fields["xpanningceiling"].Value : 0.0f;
				ypan = s.Fields.ContainsKey("ypanningceiling") ? (float)s.Fields["ypanningceiling"].Value : 0.0f;
				xscale = s.Fields.ContainsKey("xscaleceiling") ? (float)s.Fields["xscaleceiling"].Value : 1.0f;
				yscale = s.Fields.ContainsKey("yscaleceiling") ? (float)s.Fields["yscaleceiling"].Value : 1.0f;
				rotate = s.Fields.ContainsKey("rotationceiling") ? (float)s.Fields["rotationceiling"].Value : 0.0f;
			}
			catch(Exception) { return false; }
			
			// Load floor texture
			base.Texture = General.Map.Data.GetFlatImage(s.LongCeilTexture);
			if(base.Texture == null)
			{
				base.Texture = General.Map.Data.MissingTexture3D;
				setuponloadedtexture = s.LongCeilTexture;
			}
			else
			{
				if(!base.Texture.IsImageLoaded)
					setuponloadedtexture = s.LongCeilTexture;
			}

			// Determine texture scale
			if(base.Texture.IsImageLoaded)
				texscale = new Vector2D(1.0f / base.Texture.ScaledWidth, 1.0f / base.Texture.ScaledHeight);
			else
				texscale = new Vector2D(1.0f / 64.0f, 1.0f / 64.0f);

			// Prepare for math!
			rotate = Angle2D.DegToRad(rotate);
			Vector2D scale = new Vector2D(xscale, yscale);
			Vector2D offset = new Vector2D(xpan, ypan);

			// Make vertices
			ReadOnlyCollection<Vector2D> triverts = base.Sector.Sector.Triangles.Vertices;
			verts = new WorldVertex[triverts.Count];
			for(int i = 0; i < triverts.Count; i++)
			{
				// Color shading
				PixelColor c = PixelColor.FromInt(level.color);
				verts[i].c = c.WithAlpha((byte)General.Clamp(level.alpha, 0, 255)).ToInt();
				
				// Vertex coordinates
				verts[i].x = triverts[i].x;
				verts[i].y = triverts[i].y;
				verts[i].z = level.plane.GetZ(triverts[i]); //(float)s.CeilHeight;

				// Texture coordinates
				Vector2D pos = triverts[i];
				pos = pos.GetRotated(rotate);
				pos.y = -pos.y;
				pos = (pos + offset) * scale * texscale;
				verts[i].u = pos.x;
				verts[i].v = pos.y;
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
			
			// Determine render pass
			if(level.alpha < 255)
				this.RenderPass = RenderPass.Alpha;
			else
				this.RenderPass = RenderPass.Solid;
			
			// Apply vertices
			base.SetVertices(verts);
			return (verts.Length > 0);
		}
		
		#endregion

		#region ================== Methods

		// Paste texture
		public override void OnPasteTexture()
		{
			if(BuilderPlug.Me.CopiedFlat != null)
			{
				mode.CreateUndo("Paste ceiling " + BuilderPlug.Me.CopiedFlat);
				mode.SetActionResult("Pasted flat " + BuilderPlug.Me.CopiedFlat + " on ceiling.");
				SetTexture(BuilderPlug.Me.CopiedFlat);
				this.Setup();
			}
		}
		
		// This changes the height
		protected override void ChangeHeight(int amount)
		{
			if(level.sector == Sector.Sector)
			{
				mode.CreateUndo("Change ceiling height", UndoGroup.CeilingHeightChange, level.sector.FixedIndex);
				level.sector.CeilHeight += amount;
				mode.SetActionResult("Changed ceiling height to " + level.sector.CeilHeight + ".");
			}
			else
			{
				mode.CreateUndo("Change floor height", UndoGroup.FloorHeightChange, level.sector.FixedIndex);
				level.sector.FloorHeight += amount;
				mode.SetActionResult("Changed floor height to " + level.sector.FloorHeight + ".");
			}
		}
		
		// This performs a fast test in object picking
		public override bool PickFastReject(Vector3D from, Vector3D to, Vector3D dir)
		{
			// Check if our ray starts at the correct side of the plane
			if(level.plane.Distance(from) > 0.0f)
			{
				// Calculate the intersection
				if(level.plane.GetIntersection(from, to, ref pickrayu))
				{
					if(pickrayu > 0.0f)
					{
						pickintersect = from + (to - from) * pickrayu;

						// Intersection point within bbox?
						RectangleF bbox = Sector.Sector.BBox;
						return ((pickintersect.x >= bbox.Left) && (pickintersect.x <= bbox.Right) &&
								(pickintersect.y >= bbox.Top) && (pickintersect.y <= bbox.Bottom));
					}
				}
			}

			return false;
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
			return level.sector.CeilTexture;
		}

		// This changes the texture
		protected override void SetTexture(string texturename)
		{
			level.sector.SetCeilTexture(texturename);
			General.Map.Data.UpdateUsedTextures();
			if(level.sector == this.Sector.Sector)
			{
				this.Setup();
			}
			else if(mode.VisualSectorExists(level.sector))
			{
				BaseVisualSector vs = (BaseVisualSector)mode.GetVisualSector(level.sector);
				vs.UpdateSectorGeometry(false);
			}
		}
		
		#endregion
	}
}
