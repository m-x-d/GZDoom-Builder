
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
		public VisualCeiling(BaseVisualMode mode, VisualSector vs, SectorLevel level) : base(mode, vs, level)
		{
			// We have no destructor
			GC.SuppressFinalize(this);
		}

		// This builds the geometry. Returns false when no geometry created.
		public override bool Setup()
		{
			WorldVertex[] verts;
			WorldVertex v;
			Sector s = level.sector;
			float xpan, ypan, xscale, yscale, rotate;
			int color, light;
			bool absolute;
			Vector2D texscale;

			try
			{
				// Fetch ZDoom fields
				xpan = s.Fields.ContainsKey("xpanningceiling") ? (float)s.Fields["xpanningceiling"].Value : 0.0f;
				ypan = s.Fields.ContainsKey("ypanningceiling") ? (float)s.Fields["ypanningceiling"].Value : 0.0f;
				xscale = s.Fields.ContainsKey("xscaleceiling") ? (float)s.Fields["xscaleceiling"].Value : 1.0f;
				yscale = s.Fields.ContainsKey("yscaleceiling") ? (float)s.Fields["yscaleceiling"].Value : 1.0f;
				rotate = s.Fields.ContainsKey("rotationceiling") ? (float)s.Fields["rotationceiling"].Value : 0.0f;
				color = s.Fields.ContainsKey("lightcolor") ? (int)s.Fields["lightcolor"].Value : -1;
				light = s.Fields.ContainsKey("lightceiling") ? (int)s.Fields["lightceiling"].Value : 0;
				absolute = s.Fields.ContainsKey("lightceilingabsolute") ? (bool)s.Fields["lightceilingabsolute"].Value : false;
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
			if(!absolute) light = s.Brightness + light;
			PixelColor lightcolor = PixelColor.FromInt(color);
			PixelColor brightness = PixelColor.FromInt(mode.CalculateBrightness(light));
			PixelColor finalcolor = PixelColor.Modulate(lightcolor, brightness);
			color = finalcolor.WithAlpha(255).ToInt();

			// Make vertices
			ReadOnlyCollection<Vector2D> triverts = base.Sector.Sector.Triangles.Vertices;
			verts = new WorldVertex[triverts.Count];
			for(int i = 0; i < triverts.Count; i++)
			{
				// Color shading
				verts[i].c = color;

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
			mode.CreateUndo("Change ceiling height", UndoGroup.CeilingHeightChange, this.Sector.Sector.FixedIndex);
			this.Sector.Sector.CeilHeight += amount;
			mode.SetActionResult("Changed ceiling height to " + Sector.Sector.CeilHeight + ".");
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
