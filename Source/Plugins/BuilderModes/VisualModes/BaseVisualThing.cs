
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
using System.Globalization;
using System.Windows.Forms;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Rendering;
using CodeImp.DoomBuilder.Geometry;
using CodeImp.DoomBuilder.VisualModes;
using CodeImp.DoomBuilder.Config;
using CodeImp.DoomBuilder.Data;

#endregion

namespace CodeImp.DoomBuilder.BuilderModes
{
	internal class BaseVisualThing : VisualThing, IVisualEventReceiver
	{
		#region ================== Constants
		
		#endregion
		
		#region ================== Variables

		private readonly BaseVisualMode mode;
		
		private bool isloaded;
		private bool nointeraction; //mxd
		private ImageData sprite;
		private float cageradius2;
		private Vector2D pos2d;
		private Vector3D boxp1;
		private Vector3D boxp2;
		private static List<BaseVisualThing> updateList; //mxd
		
		// Undo/redo
		private int undoticket;

		// If this is set to true, the thing will be rebuilt after the action is performed.
		private bool changed;

		#endregion
		
		#region ================== Properties

		public bool Changed { get { return changed; } set { changed |= value; } }

		#endregion
		
		#region ================== Constructor / Setup
		
		// Constructor
		public BaseVisualThing(BaseVisualMode mode, Thing t) : base(t)
		{
			this.mode = mode;

			// Find thing information
			info = General.Map.Data.GetThingInfo(Thing.Type);

			//mxd. When true, the thing can be moved below floor/above ceiling
			nointeraction = (info.Actor != null && info.Actor.GetFlagValue("nointeraction", false));

			// Find sprite texture
			if(info.Sprite.Length > 0)
			{
				sprite = General.Map.Data.GetSpriteImage(info.Sprite);
				if(sprite != null) sprite.AddReference();
			}

			//mxd
			if(mode.UseSelectionFromClassicMode && t.Selected)
			{
				this.selected = true;
				mode.AddSelectedObject(this);
			}

			// We have no destructor
			GC.SuppressFinalize(this);
		}
		
		// This builds the thing geometry. Returns false when nothing was created.
		public bool Setup()
		{
			int sectorcolor = new PixelColor(255, 255, 255, 255).ToInt();
			fogdistance = VisualSector.MAXIMUM_FOG_DISTANCE; //mxd
			
			//mxd. Check thing size 
			float thingradius = Thing.Size; // Thing.Size has ThingRadius arg override applied
			float thingheight = Thing.Height; // Thing.Height has ThingHeight arg override applied

			if(thingradius < 0.1f || thingheight < 0.1f) 
			{
				thingradius = FIXED_RADIUS;
				thingheight = FIXED_RADIUS;
				sizeless = true;
			} 
			else 
			{
				sizeless = false;
			}

			// Find the sector in which the thing resides
			Thing.DetermineSector(mode.BlockMap);

			if(sprite != null)
			{
				Plane floor = new Plane(); //mxd
				Plane ceiling = new Plane(); //mxd
				if(Thing.Sector != null)
				{
					SectorData sd = mode.GetSectorData(Thing.Sector);
					floor = sd.Floor.plane; //mxd
					ceiling = sd.Ceiling.plane; //mxd
					SectorLevel level = sd.GetLevelAboveOrAt(new Vector3D(Thing.Position.x, Thing.Position.y, Thing.Position.z + sd.Floor.plane.GetZ(Thing.Position)));
					
					//mxd. Let's use point on floor plane instead of Thing.Sector.FloorHeight;
					if(nointeraction && level == null && sd.LightLevels.Count > 0) level = sd.LightLevels[sd.LightLevels.Count - 1];
					
					//mxd. Use the light level of the highest surface when a thing is above highest sector level.
					if(level != null)
					{
						// Use sector brightness for color shading
						PixelColor areabrightness = PixelColor.FromInt(mode.CalculateBrightness(level.brightnessbelow));
						PixelColor areacolor = PixelColor.Modulate(level.colorbelow, areabrightness);
						sectorcolor = areacolor.WithAlpha(255).ToInt();

						//mxd. Calculate fogdistance
						int brightness = Math.Max((byte)30, areabrightness.r); // R, G and B of areabrightness are the same
						if(Thing.Sector.HasFogColor)
						{
							if(Thing.Sector.UsesOutsideFog && General.Map.Data.MapInfo.OutsideFogDensity > 0)
								fogdistance = General.Map.Data.MapInfo.OutsideFogDensity;
							else if(!Thing.Sector.UsesOutsideFog && General.Map.Data.MapInfo.FogDensity > 0)
								fogdistance = General.Map.Data.MapInfo.FogDensity;
							else
								fogdistance = brightness * 11.0f;
						}
						// Thing is affected by floor glow
						else if(level.affectedbyglow)
						{
							fogdistance = (float)Math.Pow(2.0f, brightness / 9.0f);
						}
						// Thing is not affected by floor glow
						else
						{
							fogdistance = (float)Math.Pow(2.0f, brightness / 9.0f) * 2.0f;
						}
					}
				}
				
				// Check if the texture is loaded
				sprite.LoadImage();
				isloaded = sprite.IsImageLoaded;
				if(isloaded)
				{
					float offsetx = 0.0f;
					float offsety = 0.0f;
					
					base.Texture = sprite;

					// Determine sprite size and offset
					float radius = sprite.ScaledWidth * 0.5f;
					float height = sprite.ScaledHeight;
					if(sprite is SpriteImage)
					{
						offsetx = (sprite as SpriteImage).OffsetX - radius;
						offsety = (sprite as SpriteImage).OffsetY - height;
					}

					// Scale by thing type/actor scale
					// We do this after the offset x/y determination above, because that is entirely in sprite pixels space
					radius *= info.SpriteScale.Width;
					height *= info.SpriteScale.Height;
					offsetx *= info.SpriteScale.Width;
					offsety *= info.SpriteScale.Height;

					// Make vertices
					WorldVertex[] verts = new WorldVertex[6];

					if(sizeless) //mxd
					{ 
						float hh = height / 2;
						verts[0] = new WorldVertex(-radius + offsetx, 0.0f, offsety - hh, sectorcolor, 0.0f, 1.0f);
						verts[1] = new WorldVertex(-radius + offsetx, 0.0f, hh + offsety, sectorcolor, 0.0f, 0.0f);
						verts[2] = new WorldVertex(+radius + offsetx, 0.0f, hh + offsety, sectorcolor, 1.0f, 0.0f);
						verts[3] = verts[0];
						verts[4] = verts[2];
						verts[5] = new WorldVertex(+radius + offsetx, 0.0f, offsety - hh, sectorcolor, 1.0f, 1.0f);
					} 
					else 
					{
						verts[0] = new WorldVertex(-radius + offsetx, 0.0f, offsety, sectorcolor, 0.0f, 1.0f);
						verts[1] = new WorldVertex(-radius + offsetx, 0.0f, height + offsety, sectorcolor, 0.0f, 0.0f);
						verts[2] = new WorldVertex(+radius + offsetx, 0.0f, height + offsety, sectorcolor, 1.0f, 0.0f);
						verts[3] = verts[0];
						verts[4] = verts[2];
						verts[5] = new WorldVertex(+radius + offsetx, 0.0f, offsety, sectorcolor, 1.0f, 1.0f);
					}
					SetVertices(verts, floor, ceiling);
				}
				else
				{
					base.Texture = General.Map.Data.Hourglass3D;

					// Determine sprite size
					float radius = Math.Min(thingradius, thingheight / 2f);
					float height = Math.Min(thingradius * 2f, thingheight);

					// Make vertices
					WorldVertex[] verts = new WorldVertex[6];
					verts[0] = new WorldVertex(-radius, 0.0f, 0.0f, sectorcolor, 0.0f, 1.0f);
					verts[1] = new WorldVertex(-radius, 0.0f, height, sectorcolor, 0.0f, 0.0f);
					verts[2] = new WorldVertex(+radius, 0.0f, height, sectorcolor, 1.0f, 0.0f);
					verts[3] = verts[0];
					verts[4] = verts[2];
					verts[5] = new WorldVertex(+radius, 0.0f, 0.0f, sectorcolor, 1.0f, 1.0f);
					SetVertices(verts, floor, ceiling);
				}
			}
			
			// Determine position
			Vector3D pos = Thing.Position;
			if(Thing.Type == 9501)
			{
				if(Thing.Sector != null) //mxd
				{ 
					// This is a special thing that needs special positioning
					SectorData sd = mode.GetSectorData(Thing.Sector);
					pos.z = sd.Ceiling.sector.CeilHeight + Thing.Position.z;
				}
			}
			else if(Thing.Type == 9500)
			{
				if(Thing.Sector != null) //mxd
				{ 
					// This is a special thing that needs special positioning
					SectorData sd = mode.GetSectorData(Thing.Sector);
					pos.z = sd.Floor.sector.FloorHeight + Thing.Position.z;
				}
			}
			else if(info.AbsoluteZ)
			{
				// Absolute Z position
				pos.z = Thing.Position.z;
			}
			else if(info.Hangs)
			{
				// Hang from ceiling
				if(Thing.Sector != null)
				{
					SectorData sd = mode.GetSectorData(Thing.Sector);
					float maxz = sd.Ceiling.plane.GetZ(Thing.Position) - info.Height;
					pos.z = maxz;

					if(Thing.Position.z > 0 || nointeraction) pos.z -= Thing.Position.z;

					// Check if below floor
					if(!nointeraction)
					{
						float minz = sd.Floor.plane.GetZ(Thing.Position);
						if(pos.z < minz) pos.z = Math.Min(minz, maxz);
					}
				}
			}
			else
			{
				// Stand on floor
				if(Thing.Sector != null)
				{
					SectorData sd = mode.GetSectorData(Thing.Sector);
					float minz = sd.Floor.plane.GetZ(Thing.Position);
					pos.z = minz;

					if(Thing.Position.z > 0 || nointeraction) pos.z += Thing.Position.z;

					// Check if above ceiling
					if(!nointeraction)
					{
						float maxz = sd.Ceiling.plane.GetZ(Thing.Position) - info.Height;
						if(pos.z > maxz) pos.z = Math.Max(minz, maxz);
					}
				}
			}
			
			// Apply settings
			SetPosition(pos);
			SetCageSize(thingradius, thingheight);
			SetCageColor(Thing.Color);

			// Keep info for object picking
			cageradius2 = thingradius * Angle2D.SQRT2;
			cageradius2 = cageradius2 * cageradius2;
			pos2d = pos;

			if(sizeless) //mxd
			{ 
				boxp1 = new Vector3D(pos.x - thingradius, pos.y - thingradius, pos.z - thingradius/2);
				boxp2 = new Vector3D(pos.x + thingradius, pos.y + thingradius, pos.z + thingradius/2);
			} 
			else 
			{
				boxp1 = new Vector3D(pos.x - thingradius, pos.y - thingradius, pos.z);
				boxp2 = new Vector3D(pos.x + thingradius, pos.y + thingradius, pos.z + thingheight);
			}
			
			// Done
			changed = false;
			return true;
		}
		
		// Disposing
		public override void Dispose()
		{
			if(!IsDisposed)
			{
				if(sprite != null)
				{
					sprite.RemoveReference();
					sprite = null;
				}

				base.Dispose();
			}
		}
		
		#endregion
		
		#region ================== Methods
		
		// This forces to rebuild the whole thing
		public void Rebuild()
		{
			// Find thing information
			info = General.Map.Data.GetThingInfo(Thing.Type);

			//mxd. When true, the thing can be moved below floor/above ceiling
			nointeraction = (info.Actor != null && info.Actor.GetFlagValue("nointeraction", false));

			// Find sprite texture
			if(info.Sprite.Length > 0)
			{
				sprite = General.Map.Data.GetSpriteImage(info.Sprite);
				if(sprite != null) sprite.AddReference();
			}
			
			// Setup visual thing
			Setup();
		}
		
		// This updates the thing when needed
		public override void Update()
		{
			if(!isloaded)
			{
				// Rebuild sprite geometry when sprite is loaded
				if(sprite.IsImageLoaded)
				{
					Setup();
				}
			}
			
			// Let the base update
			base.Update();
		}

		// This performs a fast test in object picking
		public override bool PickFastReject(Vector3D from, Vector3D to, Vector3D dir)
		{
			float distance2 = Line2D.GetDistanceToLineSq(from, to, pos2d, false);
			return (distance2 <= cageradius2);
		}

		// This performs an accurate test for object picking
		public override bool PickAccurate(Vector3D from, Vector3D to, Vector3D dir, ref float u_ray)
		{
			Vector3D delta = to - from;
			float tfar = float.MaxValue;
			float tnear = float.MinValue;
			
			// Ray-Box intersection code
			// See http://www.masm32.com/board/index.php?topic=9941.0
			
			// Check X slab
			if(delta.x == 0.0f)
			{
				if(from.x > boxp2.x || from.x < boxp1.x)
				{
					// Ray is parallel to the planes & outside slab
					return false;
				}
			}
			else
			{
				float tmp = 1.0f / delta.x;
				float t1 = (boxp1.x - from.x) * tmp;
				float t2 = (boxp2.x - from.x) * tmp;
				if(t1 > t2) General.Swap(ref t1, ref t2);
				if(t1 > tnear) tnear = t1;
				if(t2 < tfar) tfar = t2;
				if(tnear > tfar || tfar < 0.0f)
				{
					// Ray missed box or box is behind ray
					return false;
				}
			}
			
			// Check Y slab
			if(delta.y == 0.0f)
			{
				if(from.y > boxp2.y || from.y < boxp1.y)
				{
					// Ray is parallel to the planes & outside slab
					return false;
				}
			}
			else
			{
				float tmp = 1.0f / delta.y;
				float t1 = (boxp1.y - from.y) * tmp;
				float t2 = (boxp2.y - from.y) * tmp;
				if(t1 > t2) General.Swap(ref t1, ref t2);
				if(t1 > tnear) tnear = t1;
				if(t2 < tfar) tfar = t2;
				if(tnear > tfar || tfar < 0.0f)
				{
					// Ray missed box or box is behind ray
					return false;
				}
			}
			
			// Check Z slab
			if(delta.z == 0.0f)
			{
				if(from.z > boxp2.z || from.z < boxp1.z)
				{
					// Ray is parallel to the planes & outside slab
					return false;
				}
			}
			else
			{
				float tmp = 1.0f / delta.z;
				float t1 = (boxp1.z - from.z) * tmp;
				float t2 = (boxp2.z - from.z) * tmp;
				if(t1 > t2) General.Swap(ref t1, ref t2);
				if(t1 > tnear) tnear = t1;
				if(t2 < tfar) tfar = t2;
				if(tnear > tfar || tfar < 0.0f)
				{
					// Ray missed box or box is behind ray
					return false;
				}
			}
			
			// Set interpolation point
			u_ray = (tnear > 0.0f) ? tnear : tfar;
			return true;
		}

		//mxd
		public bool IsSelected() 
		{
			return selected;
		}
		
		#endregion

		#region ================== Events

		// Unused
		public void OnSelectBegin() { }
		public void OnEditBegin() { }
		public void OnMouseMove(MouseEventArgs e) { }
		public void OnChangeTargetBrightness(bool up) { }
		public void OnChangeTextureOffset(int horizontal, int vertical, bool doSurfaceAngleCorrection) { }
		public void OnSelectTexture() { }
		public void OnCopyTexture() { }
		public void OnPasteTexture() { }
		public void OnCopyTextureOffsets() { }
		public void OnPasteTextureOffsets() { }
		public void OnTextureAlign(bool alignx, bool aligny) { }
		public void OnToggleUpperUnpegged() { }
		public void OnToggleLowerUnpegged() { }
		public void OnProcess(float deltatime) { }
		public void OnTextureFloodfill() { }
		public void OnInsert() { }
		public void OnTextureFit(FitTextureOptions options) { } //mxd
		public void ApplyTexture(string texture) { }
		public void ApplyUpperUnpegged(bool set) { }
		public void ApplyLowerUnpegged(bool set) { }
		public void SelectNeighbours(bool select, bool withSameTexture, bool withSameHeight) { } //mxd
		
		// Return texture name
		public string GetTextureName() { return ""; }

		// Select or deselect
		public void OnSelectEnd()
		{
			if(this.selected)
			{
				this.selected = false;
				mode.RemoveSelectedObject(this);
			}
			else
			{
				this.selected = true;
				mode.AddSelectedObject(this);
			}
		}

		//mxd. Delete thing
		public void OnDelete() 
		{
			mode.CreateUndo("Delete thing");
			mode.SetActionResult("Deleted a thing.");

			this.Thing.Fields.BeforeFieldsChange();
			this.Thing.Dispose();
			this.Dispose();

			General.Map.IsChanged = true;
			General.Map.ThingsFilter.Update();
		}
		
		// Copy properties
		public void OnCopyProperties()
		{
			BuilderPlug.Me.CopiedThingProps = new ThingProperties(Thing);
			mode.SetActionResult("Copied thing properties.");
		}
		
		// Paste properties
		public void OnPasteProperties()
		{
			if(BuilderPlug.Me.CopiedThingProps != null)
			{
				mode.CreateUndo("Paste thing properties");
				mode.SetActionResult("Pasted thing properties.");
				BuilderPlug.Me.CopiedThingProps.Apply(Thing);
				Thing.UpdateConfiguration();
				this.Rebuild();
				mode.ShowTargetInfo();
			}
		}
		
		// Edit button released
		public void OnEditEnd()
		{
			if(General.Interface.IsActiveWindow)
			{
				
				List<Thing> things = mode.GetSelectedThings();
				//mxd
				updateList = new List<BaseVisualThing>();
				foreach(Thing t in things)
				{
					VisualThing vt = mode.GetVisualThing(t);
					if(vt != null) updateList.Add(vt as BaseVisualThing);
				}

				General.Interface.OnEditFormValuesChanged += Interface_OnEditFormValuesChanged;
				mode.StartRealtimeInterfaceUpdate(SelectionType.Things);
				General.Interface.ShowEditThings(things);
				mode.StopRealtimeInterfaceUpdate(SelectionType.Things);
				General.Interface.OnEditFormValuesChanged -= Interface_OnEditFormValuesChanged;

				updateList.Clear();
				updateList = null;
			}
		}

		//mxd
		private void Interface_OnEditFormValuesChanged(object sender, EventArgs e) 
		{
			foreach(BaseVisualThing vt in updateList) vt.Changed = true;
		}

		//mxd
		public void OnResetTextureOffset() 
		{
			mode.CreateUndo("Reset thing scale");
			mode.SetActionResult("Thing scale reset.");

			Thing.SetScale(1.0f, 1.0f);

			// Update what must be updated
			this.Changed = true;
		}

		//mxd
		public void OnResetLocalTextureOffset() 
		{
			mode.CreateUndo("Reset thing scale, pitch and roll");
			mode.SetActionResult("Thing scale, pitch and roll reset.");

			Thing.SetScale(1.0f, 1.0f);
			Thing.SetPitch(0);
			Thing.SetRoll(0);

			// Update what must be updated
			this.Changed = true;
		}
		
		// Raise/lower thing
		public void OnChangeTargetHeight(int amount)
		{
			if(General.Map.FormatInterface.HasThingHeight)
			{
				if((General.Map.UndoRedo.NextUndo == null) || (General.Map.UndoRedo.NextUndo.TicketID != undoticket))
					undoticket = mode.CreateUndo("Change thing height");

				Thing.Move(Thing.Position + new Vector3D(0.0f, 0.0f, (info.Hangs ? -amount : amount)));

				mode.SetActionResult("Changed thing height to " + Thing.Position.z + ".");
				
				// Update what must be updated
				ThingData td = mode.GetThingData(this.Thing);
				foreach(KeyValuePair<Sector, bool> s in td.UpdateAlso)
				{
					if(mode.VisualSectorExists(s.Key))
					{
						BaseVisualSector vs = (BaseVisualSector)mode.GetVisualSector(s.Key);
						vs.UpdateSectorGeometry(s.Value);
					}
				}
				
				this.Changed = true;
			}
		}

		//mxd
		public void OnChangeScale(int incrementX, int incrementY)
		{
			if(!General.Map.UDMF || !sprite.IsImageLoaded) return;
			
			if((General.Map.UndoRedo.NextUndo == null) || (General.Map.UndoRedo.NextUndo.TicketID != undoticket))
				undoticket = mode.CreateUndo("Change thing scale");

			float scaleX = Thing.ScaleX;
			float scaleY = Thing.ScaleY;

			if(incrementX != 0) 
			{
				float pix = (int)Math.Round(sprite.Width * scaleX) + incrementX;
				float newscaleX = (float)Math.Round(pix / sprite.Width, 3);
				scaleX = (newscaleX == 0 ? scaleX * -1 : newscaleX);
			}

			if(incrementY != 0) 
			{
				float pix = (int)Math.Round(sprite.Height * scaleY) + incrementY;
				float newscaleY = (float)Math.Round(pix / sprite.Height, 3);
				scaleY = (newscaleY == 0 ? scaleY * -1 : newscaleY);
			}

			Thing.SetScale(scaleX, scaleY);
			mode.SetActionResult("Changed thing scale to " + scaleX.ToString("F03", CultureInfo.InvariantCulture) + ", " + scaleY.ToString("F03", CultureInfo.InvariantCulture) + " (" + (int)Math.Round(sprite.Width * scaleX) + " x " + (int)Math.Round(sprite.Height * scaleY) + ").");

			// Update what must be updated
			this.Changed = true;
		}

		//mxd
		public void OnMove(Vector3D newPosition) 
		{
			if ((General.Map.UndoRedo.NextUndo == null) || (General.Map.UndoRedo.NextUndo.TicketID != undoticket))
				undoticket = mode.CreateUndo("Move thing");
			Thing.Move(newPosition);
			mode.SetActionResult("Changed thing position to " + Thing.Position + ".");

			// Update what must be updated
			ThingData td = mode.GetThingData(this.Thing);
			foreach (KeyValuePair<Sector, bool> s in td.UpdateAlso) 
			{
				if (mode.VisualSectorExists(s.Key)) 
				{
					BaseVisualSector vs = (BaseVisualSector)mode.GetVisualSector(s.Key);
					vs.UpdateSectorGeometry(s.Value);
				}
			}

			this.Changed = true;
		}

		//mxd
		public void SetAngle(int newangle)
		{
			if ((General.Map.UndoRedo.NextUndo == null) || (General.Map.UndoRedo.NextUndo.TicketID != undoticket))
				undoticket = mode.CreateUndo("Change thing angle");
			Thing.Rotate(newangle);
			mode.SetActionResult("Changed thing angle to " + Thing.AngleDoom + ".");
			this.Changed = true;
		}

		//mxd
		public void SetPitch(int newpitch)
		{
			if(!General.Map.UDMF) return;
			if((General.Map.UndoRedo.NextUndo == null) || (General.Map.UndoRedo.NextUndo.TicketID != undoticket))
				undoticket = mode.CreateUndo("Change thing pitch");
			Thing.SetPitch(newpitch);
			mode.SetActionResult("Changed thing pitch to " + Thing.Pitch + ".");
			this.Changed = true;
		}

		//mxd
		public void SetRoll(int newroll)
		{
			if(!General.Map.UDMF) return;
			if((General.Map.UndoRedo.NextUndo == null) || (General.Map.UndoRedo.NextUndo.TicketID != undoticket))
				undoticket = mode.CreateUndo("Change thing roll");
			Thing.SetRoll(newroll);
			mode.SetActionResult("Changed thing roll to " + Thing.Roll + ".");
			this.Changed = true;
		}
		
		#endregion
	}
}
