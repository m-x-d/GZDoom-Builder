
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
using CodeImp.DoomBuilder.Types;
using CodeImp.DoomBuilder.VisualModes;
using CodeImp.DoomBuilder.Windows;

#endregion

namespace CodeImp.DoomBuilder.BuilderModes
{
	internal sealed class VisualCeiling : BaseVisualGeometrySector
	{
		#region ================== Constants

		#endregion

		#region ================== Variables

		public bool innerSide; //mxd

		#endregion

		#region ================== Properties

		#endregion

		#region ================== Constructor / Setup

		// Constructor
		public VisualCeiling(BaseVisualMode mode, VisualSector vs) : base(mode, vs)
		{
            //mxd
            geoType = VisualGeometryType.CEILING;

			//mxd
			if(mode.UseSelectionFromClassicMode && vs != null && vs.Sector.Selected && (General.Map.ViewMode == ViewMode.CeilingTextures || General.Map.ViewMode == ViewMode.Normal)) {
				this.selected = true;
				mode.AddSelectedObject(this);
			}
            
            // We have no destructor
			GC.SuppressFinalize(this);
		}

		// This builds the geometry. Returns false when no geometry created.
		public override bool Setup(SectorLevel level, Effect3DFloor extrafloor) {
			return Setup(level, extrafloor, innerSide);
		}

		//mxd
		public bool Setup(SectorLevel level, Effect3DFloor extrafloor, bool innerSide)
		{
			WorldVertex[] verts;
			Sector s = level.sector;
			Vector2D texscale;
			this.innerSide = innerSide; //mxd
			
			base.Setup(level, extrafloor);
			
			// Fetch ZDoom fields
			float rotate = Angle2D.DegToRad(s.Fields.GetValue("rotationceiling", 0.0f));
			Vector2D offset = new Vector2D(s.Fields.GetValue("xpanningceiling", 0.0f),
			                               s.Fields.GetValue("ypanningceiling", 0.0f));
			Vector2D scale = new Vector2D(s.Fields.GetValue("xscaleceiling", 1.0f),
			                              s.Fields.GetValue("yscaleceiling", 1.0f));
			
			//Load ceiling texture
			if((s.CeilTexture.Length > 0) && (s.CeilTexture[0] != '-')) {
				base.Texture = General.Map.Data.GetFlatImage(s.LongCeilTexture);
				if(base.Texture == null) {
					base.Texture = General.Map.Data.MissingTexture3D;
					setuponloadedtexture = s.LongCeilTexture;
				} else {
					if(!base.Texture.IsImageLoaded) {
						setuponloadedtexture = s.LongCeilTexture;
					}
				}
			} else {
				// Use missing texture
				base.Texture = General.Map.Data.MissingTexture3D;
				setuponloadedtexture = 0;
			}

			// Determine texture scale
			if(base.Texture.IsImageLoaded)
				texscale = new Vector2D(1.0f / base.Texture.ScaledWidth, 1.0f / base.Texture.ScaledHeight);
			else
				texscale = new Vector2D(1.0f / 64.0f, 1.0f / 64.0f);

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
				verts[i].z = level.plane.GetZ(triverts[i]);

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
			if(extrafloor == null || extrafloor.VavoomType || innerSide)
				SwapTriangleVertices(verts);

			// Determine render pass
			if(extrafloor != null)
			{
				if ((extrafloor.Linedef.Args[2] & (int)Effect3DFloor.Flags.RenderAdditive) != 0) //mxd
					this.RenderPass = RenderPass.Additive;
				else if (level.alpha < 255)
					this.RenderPass = RenderPass.Alpha;
				else
					this.RenderPass = RenderPass.Mask;
			}
			else
			{
				this.RenderPass = RenderPass.Solid;
			}
			
			// Apply vertices
			base.SetVertices(verts);
			return (verts.Length > 0);
		}
		
		#endregion

		#region ================== Methods

		// Return texture coordinates
		protected override Point GetTextureOffset()
		{
			Point p = new Point();
			p.X = (int)Sector.Sector.Fields.GetValue("xpanningceiling", 0.0f);
			p.Y = (int)Sector.Sector.Fields.GetValue("ypanningceiling", 0.0f);
			return p;
		}

		// Move texture coordinates
		protected override void MoveTextureOffset(Point xy)
		{
            //mxd
            Sector s = GetControlSector();
			s.Fields.BeforeFieldsChange();
            float oldx = s.Fields.GetValue("xpanningceiling", 0.0f);
            float oldy = s.Fields.GetValue("ypanningceiling", 0.0f);
            xy = getTranslatedTextureOffset(xy);
            s.Fields["xpanningceiling"] = new UniValue(UniversalType.Float, oldx + (float)xy.X);
            s.Fields["ypanningceiling"] = new UniValue(UniversalType.Float, oldy + (float)xy.Y);
            s.UpdateNeeded = true;
		}

		// Paste texture
		public override void OnPasteTexture()
		{
			if(BuilderPlug.Me.CopiedFlat != null)
			{
				mode.CreateUndo("Paste ceiling " + BuilderPlug.Me.CopiedFlat);
				mode.SetActionResult("Pasted flat " + BuilderPlug.Me.CopiedFlat + " on ceiling.");
				SetTexture(BuilderPlug.Me.CopiedFlat);
				this.Setup();

				//mxd. 3D floors may need updating...
				onTextureChanged();
			}
		}

		// Call to change the height
		public override void OnChangeTargetHeight(int amount)
		{
			// Only do this when not done yet in this call
			// Because we may be able to select the same 3D floor multiple times through multiple sectors
			SectorData sd = mode.GetSectorData(level.sector);
			if(!sd.CeilingChanged)
			{
				sd.CeilingChanged = true;
				base.OnChangeTargetHeight(amount);
			}
		}

		// This changes the height
		protected override void ChangeHeight(int amount)
		{
			mode.CreateUndo("Change ceiling height", UndoGroup.CeilingHeightChange, level.sector.FixedIndex);
			level.sector.CeilHeight += amount;
			mode.SetActionResult("Changed ceiling height to " + level.sector.CeilHeight + ".");
		}

        //mxd. Sector brightness change
        public override void OnChangeTargetBrightness(bool up) {
            if (level != null && level.sector != Sector.Sector) {
                int index = -1;
                for (int i = 0; i < Sector.ExtraCeilings.Count; i++) {
                    if (Sector.ExtraCeilings[i] == this) {
                        index = i + 1;
                        break;
                    }
                }

                if (index > -1 && index < Sector.ExtraCeilings.Count) {
					((BaseVisualSector)mode.GetVisualSector(Sector.ExtraCeilings[index].level.sector)).Floor.OnChangeTargetBrightness(up);
                } else {
                    base.OnChangeTargetBrightness(up);
                }
            } else {
                //if a map is not in UDMF format, or this ceiling is part of 3D-floor...
                if(!General.Map.UDMF || Sector.Sector != level.sector) {
					base.OnChangeTargetBrightness(up);
					return;
				}

				int light = Sector.Sector.Fields.GetValue("lightceiling", 0);
				bool absolute = Sector.Sector.Fields.GetValue("lightceilingabsolute", false);
				int newLight = 0;

				if(up)
					newLight = General.Map.Config.BrightnessLevels.GetNextHigher(light, absolute);
				else
					newLight = General.Map.Config.BrightnessLevels.GetNextLower(light, absolute);

				if(newLight == light) return;

				//create undo
				mode.CreateUndo("Change ceiling brightness", UndoGroup.SurfaceBrightnessChange, Sector.Sector.FixedIndex);
				Sector.Sector.Fields.BeforeFieldsChange();

				//apply changes
				Sector.Sector.Fields["lightceiling"] = new UniValue(UniversalType.Integer, newLight);
				mode.SetActionResult("Changed ceiling brightness to " + newLight + ".");
				Sector.Sector.UpdateCache();

				//rebuild sector
				Sector.UpdateSectorGeometry(false);
            }
        }
		
		// This performs a fast test in object picking
		public override bool PickFastReject(Vector3D from, Vector3D to, Vector3D dir)
		{
			// Check if our ray starts at the correct side of the plane
			if((innerSide && level.plane.Distance(from) < 0.0f) || (!innerSide && level.plane.Distance(from) > 0.0f)) //mxd
			//if(level.plane.Distance(from) > 0.0f)
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
		}

		//mxd
		public override void SelectNeighbours(bool select, bool withSameTexture, bool withSameHeight) {
			if(!withSameTexture && !withSameHeight)	return;

			if(select && !selected) {
				selected = true;
				mode.AddSelectedObject(this);
			} else if(!select && selected) {
				selected = false;
				mode.RemoveSelectedObject(this);
			}

			List<Sector> neighbours = new List<Sector>();

			//collect neighbour sectors
			foreach(Sidedef side in level.sector.Sidedefs) {
				if(side.Other != null && side.Other.Sector != level.sector && !neighbours.Contains(side.Other.Sector)) {
					bool add = false;

					if(withSameTexture && side.Other.Sector.CeilTexture == level.sector.CeilTexture) {
						add = true;
					}

					if(withSameHeight) {
						add = ((withSameTexture && add) || !withSameTexture) && side.Other.Sector.CeilHeight == level.sector.CeilHeight;
					}

					if(add) neighbours.Add(side.Other.Sector);
				}
			}

			//(de)select neighbour sectors
			foreach(Sector s in neighbours) {
				BaseVisualSector vs = mode.GetVisualSector(s) as BaseVisualSector;
				if((select && !vs.Ceiling.Selected) || (!select && vs.Ceiling.Selected))
					vs.Ceiling.SelectNeighbours(select, withSameTexture, withSameHeight);
			}
		}
		
		#endregion
	}
}
