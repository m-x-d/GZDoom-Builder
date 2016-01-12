
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
using System.Collections.ObjectModel;
using System.Drawing;
using System.Globalization;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Geometry;
using CodeImp.DoomBuilder.Rendering;
using CodeImp.DoomBuilder.Types;
using CodeImp.DoomBuilder.VisualModes;
using CodeImp.DoomBuilder.Data;

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
			geometrytype = VisualGeometryType.CEILING;
			partname = "ceiling";

			//mxd
			if(mode.UseSelectionFromClassicMode && vs != null && vs.Sector.Selected && (General.Map.ViewMode == ViewMode.CeilingTextures || General.Map.ViewMode == ViewMode.Normal)) 
			{
				this.selected = true;
				mode.AddSelectedObject(this);
			}
			
			// We have no destructor
			GC.SuppressFinalize(this);
		}

		// This builds the geometry. Returns false when no geometry created.
		public override bool Setup(SectorLevel level, Effect3DFloor extrafloor) 
		{
			return Setup(level, extrafloor, innerSide);
		}

		//mxd
		public bool Setup(SectorLevel level, Effect3DFloor extrafloor, bool innerSide)
		{
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
			if(s.LongCeilTexture != MapSet.EmptyLongName) 
			{
				base.Texture = General.Map.Data.GetFlatImage(s.LongCeilTexture);
				if(base.Texture == null || base.Texture is UnknownImage) 
				{
					base.Texture = General.Map.Data.UnknownTexture3D;
					setuponloadedtexture = s.LongCeilTexture;
				} 
				else 
				{
					if(!base.Texture.IsImageLoaded) setuponloadedtexture = s.LongCeilTexture;
				}
			} 
			else 
			{
				// Use missing texture
				base.Texture = General.Map.Data.MissingTexture3D;
				setuponloadedtexture = 0;
			}

			// Determine texture scale
			if(base.Texture.IsImageLoaded)
				texscale = new Vector2D(1.0f / base.Texture.ScaledWidth, 1.0f / base.Texture.ScaledHeight);
			else
				texscale = new Vector2D(1.0f / 64.0f, 1.0f / 64.0f);

			// Determine brightness
			int color = PixelColor.FromInt(level.color).WithAlpha((byte)General.Clamp(level.alpha, 0, 255)).ToInt();

			//mxd. Top extrafloor level should calculate fogdensity
			//from the brightness of the level above it
			int targetbrightness;
			if(extrafloor != null && !extrafloor.VavoomType && !level.disablelighting)
			{
				targetbrightness = 0;
				SectorData sd = mode.GetSectorData(this.Sector.Sector);
				for(int i = 0; i < sd.LightLevels.Count - 1; i++)
				{
					if(sd.LightLevels[i] == level)
					{
						targetbrightness = sd.LightLevels[i + 1].brightnessbelow;
						break;
					}
				}
			}
			else
			{
				targetbrightness = level.brightnessbelow;
			}

			//mxd. Determine fog density
			fogfactor = CalculateFogDensity(targetbrightness);

			// Make vertices
			ReadOnlyCollection<Vector2D> triverts = Sector.Sector.Triangles.Vertices;
			WorldVertex[] verts = new WorldVertex[triverts.Count];
			for(int i = 0; i < triverts.Count; i++)
			{
				// Color shading
				verts[i].c = color; //mxd
				
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
				if(extrafloor.Sloped3dFloor) //mxd
					this.RenderPass = RenderPass.Mask;
				else if(extrafloor.RenderAdditive) //mxd
					this.RenderPass = RenderPass.Additive;
				else if(level.alpha < 255)
					this.RenderPass = RenderPass.Alpha;
				else
					this.RenderPass = RenderPass.Mask;
			}
			else
			{
				this.RenderPass = RenderPass.Solid;
			}

			//mxd. Update sky render flag
			UpdateSkyRenderFlag();
			
			// Apply vertices
			base.SetVertices(verts);
			return (verts.Length > 0);
		}

		//mxd
		protected override void UpdateSkyRenderFlag()
		{
			bool isrenderedassky = renderassky;
			renderassky = (level.sector.CeilTexture == General.Map.Config.SkyFlatName);
			if(isrenderedassky != renderassky && Sector.Sides != null)
			{
				// Upper/middle geometry may need updating...
				foreach(Sidedef side in level.sector.Sidedefs)
				{
					VisualSidedefParts parts = Sector.GetSidedefParts(side);
					if(parts.upper != null) parts.upper.UpdateSkyRenderFlag();
					else if(parts.middlesingle != null) parts.middlesingle.UpdateSkyRenderFlag();

					// On the other side as well...
					if(side.Other != null && side.Other.Sector != null &&
					   side.Other.Sector.CeilTexture == General.Map.Config.SkyFlatName)
					{
						BaseVisualSector other = (BaseVisualSector)mode.GetVisualSector(side.Other.Sector);
						if(other != null && other.Sides != null)
						{
							parts = other.GetSidedefParts(side.Other);
							if(parts.upper != null) parts.upper.UpdateSkyRenderFlag();
						}
					}
				}
			}
		}

		#endregion

		#region ================== Methods

		// Return texture coordinates
		protected override Point GetTextureOffset()
		{
			return new Point { X = (int)Sector.Sector.Fields.GetValue("xpanningceiling", 0.0f), 
							   Y = (int)Sector.Sector.Fields.GetValue("ypanningceiling", 0.0f) };
		}

		// Move texture coordinates
		protected override void MoveTextureOffset(Point xy)
		{
			//mxd
			Sector s = GetControlSector();
			s.Fields.BeforeFieldsChange();
			float nx = (s.Fields.GetValue("xpanningceiling", 0.0f) + xy.X) % (Texture.ScaledWidth / s.Fields.GetValue("xscaleceiling", 1.0f));
			float ny = (s.Fields.GetValue("ypanningceiling", 0.0f) + xy.Y) % (Texture.ScaledHeight / s.Fields.GetValue("yscaleceiling", 1.0f));
			s.Fields["xpanningceiling"] = new UniValue(UniversalType.Float, nx);
			s.Fields["ypanningceiling"] = new UniValue(UniversalType.Float, ny);
			s.UpdateNeeded = true;

			mode.SetActionResult("Changed ceiling texture offsets to " + nx + ", " + ny + ".");
		}

		//mxd. Texture scale change
		protected override void ChangeTextureScale(int incrementX, int incrementY) 
		{
			Sector s = GetControlSector();
			float scaleX = s.Fields.GetValue("xscaleceiling", 1.0f);
			float scaleY = s.Fields.GetValue("yscaleceiling", 1.0f);

			s.Fields.BeforeFieldsChange();

			if(incrementX != 0) 
			{
				float pix = (int)Math.Round(Texture.Width * scaleX) - incrementX;
				float newscaleX = (float)Math.Round(pix / Texture.Width, 3);
				scaleX = (newscaleX == 0 ? scaleX * -1 : newscaleX);
				UniFields.SetFloat(s.Fields, "xscaleceiling", scaleX, 1.0f);
			}

			if(incrementY != 0) 
			{
				float pix = (int)Math.Round(Texture.Height * scaleY) - incrementY;
				float newscaleY = (float)Math.Round(pix / Texture.Height, 3);
				scaleY = (newscaleY == 0 ? scaleY * -1 : newscaleY);
				UniFields.SetFloat(s.Fields, "yscaleceiling", scaleY, 1.0f);
			}

			//update geometry
			OnTextureChanged();

			s.UpdateNeeded = true;
			s.UpdateCache();
			if(s.Index != Sector.Sector.Index) 
			{
				Sector.Sector.UpdateNeeded = true;
				Sector.Sector.UpdateCache();
			}

			mode.SetActionResult("Ceiling scale changed to " + scaleX.ToString("F03", CultureInfo.InvariantCulture) + ", " + scaleY.ToString("F03", CultureInfo.InvariantCulture) + " (" + (int)Math.Round(Texture.Width / scaleX) + " x " + (int)Math.Round(Texture.Height / scaleY) + ").");
		}

		//mxd
		public override void OnResetTextureOffset() 
		{
			ClearFields(new[] { "xpanningceiling", "ypanningceiling" }, "Reset texture offsets", "Texture offsets reset.");
		}

		//mxd
		public override void OnResetLocalTextureOffset() 
		{
			ClearFields(new[] { "xpanningceiling", "ypanningceiling", "xscaleceiling", "yscaleceiling", "rotationceiling", "lightceiling", "lightceilingabsolute" },
				"Reset texture offsets, scale, rotation and brightness", "Texture offsets, scale, rotation and brightness reset.");
		}

		// Paste texture
		public override void OnPasteTexture()
		{
			if(BuilderPlug.Me.CopiedFlat != null)
			{
				mode.CreateUndo("Paste ceiling '" + BuilderPlug.Me.CopiedFlat + "'");
				mode.SetActionResult("Pasted flat '" + BuilderPlug.Me.CopiedFlat + "' on ceiling.");

				//mxd. Glow effect may require SectorData and geometry update
				bool prevtextureglows = General.Map.Data.GlowingFlats.ContainsKey(Sector.Sector.LongCeilTexture);

				SetTexture(BuilderPlug.Me.CopiedFlat);

				//mxd. Glow effect may require SectorData and geometry update
				if(prevtextureglows && !General.Map.Data.GlowingFlats.ContainsKey(Sector.Sector.LongCeilTexture))
				{
					SectorData sd = mode.GetSectorData(level.sector);
					sd.UpdateForced();

					if(mode.VisualSectorExists(level.sector))
					{
						BaseVisualSector vs = (BaseVisualSector)mode.GetVisualSector(level.sector);
						vs.UpdateSectorGeometry(false);
					}
				}

				//mxd. 3D floors may need updating...
				OnTextureChanged();
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

			if(General.Map.UDMF)
			{
				//mxd. Modify vertex offsets?
				if(level.sector.Sidedefs.Count == 3)
				{
					ChangeVertexHeight(amount);
				}

				//mxd. Modify slope offset?
				if(level.sector.CeilSlope.GetLengthSq() > 0) 
				{
					Vector3D center = new Vector3D(level.sector.BBox.X + level.sector.BBox.Width / 2,
												   level.sector.BBox.Y + level.sector.BBox.Height / 2,
												   level.sector.CeilHeight);
					
					Plane p = new Plane(center,
										level.sector.CeilSlope.GetAngleXY() - Angle2D.PIHALF,
										level.sector.CeilSlope.GetAngleZ(),
										false);

					level.sector.CeilSlopeOffset = p.Offset;
				}
			}

			mode.SetActionResult("Changed ceiling height to " + level.sector.CeilHeight + ".");
		}

		//mxd
		private void ChangeVertexHeight(int amount) 
		{
			List<Vertex> verts = new List<Vertex>(3);

			//do this only if all 3 verts have offsets
			foreach(Sidedef side in level.sector.Sidedefs) 
			{
				if(float.IsNaN(side.Line.Start.ZCeiling) || float.IsNaN(side.Line.End.ZCeiling)) return;
				if(!verts.Contains(side.Line.Start)) verts.Add(side.Line.Start);
				if(!verts.Contains(side.Line.End)) verts.Add(side.Line.End);
			}

			foreach(Vertex v in verts) 
				mode.GetVisualVertex(v, false).OnChangeTargetHeight(amount);
		}

		//mxd. Sector brightness change
		public override void OnChangeTargetBrightness(bool up) 
		{
			if(level != null && level.sector != Sector.Sector) 
			{
				int index = -1;
				for(int i = 0; i < Sector.ExtraCeilings.Count; i++) 
				{
					if(Sector.ExtraCeilings[i] == this) 
					{
						index = i + 1;
						break;
					}
				}

				if(index > -1 && index < Sector.ExtraCeilings.Count)
					((BaseVisualSector)mode.GetVisualSector(Sector.ExtraCeilings[index].level.sector)).Floor.OnChangeTargetBrightness(up);
				else
					base.OnChangeTargetBrightness(up);
			} 
			else 
			{
				//if a map is not in UDMF format, or this ceiling is part of 3D-floor...
				if(!General.Map.UDMF || (level != null && Sector.Sector != level.sector)) 
				{
					base.OnChangeTargetBrightness(up);
					return;
				}

				int light = Sector.Sector.Fields.GetValue("lightceiling", 0);
				bool absolute = Sector.Sector.Fields.GetValue("lightceilingabsolute", false);
				int newLight;

				if(up)
					newLight = General.Map.Config.BrightnessLevels.GetNextHigher(light, absolute);
				else
					newLight = General.Map.Config.BrightnessLevels.GetNextLower(light, absolute);

				if(newLight == light) return;

				//create undo
				mode.CreateUndo("Change ceiling brightness", UndoGroup.SurfaceBrightnessChange, Sector.Sector.FixedIndex);
				Sector.Sector.Fields.BeforeFieldsChange();

				//apply changes
				UniFields.SetInteger(Sector.Sector.Fields, "lightceiling", newLight, (absolute ? int.MinValue : 0));
				mode.SetActionResult("Changed ceiling brightness to " + newLight + ".");
				Sector.Sector.UpdateNeeded = true;
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
		public override void SelectNeighbours(bool select, bool withSameTexture, bool withSameHeight) 
		{
			if(!withSameTexture && !withSameHeight) return;

			if(select && !selected)
			{
				selected = true;
				mode.AddSelectedObject(this);
			}
			else if(!select && selected) 
			{
				selected = false;
				mode.RemoveSelectedObject(this);
			}

			List<Sector> neighbours = new List<Sector>();
			bool regularorvavoom = extrafloor == null || (extrafloor != null && extrafloor.VavoomType);

			//collect neighbour sectors
			foreach(Sidedef side in Sector.Sector.Sidedefs)
			{
				if(side.Other != null && side.Other.Sector != Sector.Sector && !neighbours.Contains(side.Other.Sector)) 
				{
					BaseVisualSector vs = mode.GetVisualSector(side.Other.Sector) as BaseVisualSector;
					if(vs == null) continue;

					// When current ceiling is part of a 3d floor, it looks like a floor, so we need to select adjacent floors
					if(level.sector != Sector.Sector && !regularorvavoom)
					{
						if((withSameTexture && side.Other.Sector.LongFloorTexture == level.sector.LongCeilTexture) ||
							(withSameHeight && side.Other.Sector.FloorHeight == level.sector.CeilHeight)) 
						{
							neighbours.Add(side.Other.Sector);

							//(de)select regular visual floor?
							if(select != vs.Floor.Selected) 
								vs.Floor.SelectNeighbours(select, withSameTexture, withSameHeight);
						}
					} 
					else // Regular ceiling or vavoom-type extra ceiling
					{
						// (De)select adjacent ceilings
						if((withSameTexture && side.Other.Sector.LongCeilTexture == level.sector.LongCeilTexture) ||
							(withSameHeight && side.Other.Sector.CeilHeight == level.sector.CeilHeight)) 
						{
							neighbours.Add(side.Other.Sector);

							//(de)select regular visual ceiling?
							if(select != vs.Ceiling.Selected) 
								vs.Ceiling.SelectNeighbours(select, withSameTexture, withSameHeight);
						}
					}

					// (De)select adjacent extra ceilings
					foreach(VisualCeiling ec in vs.ExtraCeilings)
					{
						if(select == ec.Selected || ec.extrafloor.VavoomType != regularorvavoom) continue;
						if((withSameTexture && level.sector.LongCeilTexture == ec.level.sector.LongCeilTexture) ||
							(withSameHeight && level.sector.CeilHeight == ec.level.sector.CeilHeight)) 
						{
							ec.SelectNeighbours(select, withSameTexture, withSameHeight);
						}
					}

					// (De)select adjacent extra floors
					foreach(VisualFloor ef in vs.ExtraFloors)
					{
						if(select == ef.Selected || ef.ExtraFloor.VavoomType == regularorvavoom) continue;
						if((withSameTexture && level.sector.LongCeilTexture == ef.Level.sector.LongFloorTexture) ||
							(withSameHeight && level.sector.CeilHeight == ef.Level.sector.FloorHeight)) 
						{
							ef.SelectNeighbours(select, withSameTexture, withSameHeight);
						}
					}
				}
			}
		}

		//mxd
		public void AlignTexture(bool alignx, bool aligny) 
		{
			if(!General.Map.UDMF) return;

			//is is a surface with line slope?
			float slopeAngle = level.plane.Normal.GetAngleZ() - Angle2D.PIHALF;

			if(slopeAngle == 0) //it's a horizontal plane
			{
				AlignTextureToClosestLine(alignx, aligny);
			} 
			else //it can be a surface with line slope
			{ 
				Linedef slopeSource = null;
				bool isFront = false;

				foreach(Sidedef side in Sector.Sector.Sidedefs) 
				{
					if(side.Line.Action == 181) 
					{
						if(side.Line.Args[1] == 1 && side.Line.Front != null && side.Line.Front == side) 
						{
							slopeSource = side.Line;
							isFront = true;
							break;
						}

						if(side.Line.Args[1] == 2 && side.Line.Back != null && side.Line.Back == side) 
						{
							slopeSource = side.Line;
							break;
						}
					}
				}

				if(slopeSource != null && slopeSource.Front != null && slopeSource.Front.Sector != null && slopeSource.Back != null && slopeSource.Back.Sector != null)
					AlignTextureToSlopeLine(slopeSource, slopeAngle, isFront, alignx, aligny);
				else
					AlignTextureToClosestLine(alignx, aligny);
			}
		}
		
		#endregion
	}
}
