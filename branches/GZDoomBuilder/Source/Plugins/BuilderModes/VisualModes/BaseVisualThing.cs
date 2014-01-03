
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
using System.Windows.Forms;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Rendering;
using CodeImp.DoomBuilder.Geometry;
using CodeImp.DoomBuilder.VisualModes;
using CodeImp.DoomBuilder.Config;
using CodeImp.DoomBuilder.Data;
using System.Drawing.Drawing2D;

#endregion

namespace CodeImp.DoomBuilder.BuilderModes
{
	internal class BaseVisualThing : VisualThing, IVisualEventReceiver
	{
		#region ================== Constants
		
		#endregion
		
		#region ================== Variables

		protected BaseVisualMode mode;
		
		private ThingTypeInfo info;
		private bool isloaded;
		private ImageData sprite;
		private float cageradius2;
		private Vector2D pos2d;
		private Vector3D boxp1;
		private Vector3D boxp2;
		private static List<BaseVisualThing> updateList; //mxd
		
		// Undo/redo
		private int undoticket;

		// If this is set to true, the thing will be rebuilt after the action is performed.
		protected bool changed;

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

			// Find sprite texture
			if(info.Sprite.Length > 0)
			{
				sprite = General.Map.Data.GetSpriteImage(info.Sprite);
				if(sprite != null) sprite.AddReference();
			}

			//mxd
			if(mode.UseSelectionFromClassicMode && t.Selected){
				this.selected = true;
				mode.AddSelectedObject(this);
			}

			// We have no destructor
			GC.SuppressFinalize(this);
		}
		
		// This builds the thing geometry. Returns false when nothing was created.
		public virtual bool Setup()
		{
			int sectorcolor = new PixelColor(255, 255, 255, 255).ToInt();
			
			//mxd. Check thing size 
			float infoRadius, infoHeight;
			if((info.Radius < 0.1f) || (info.Height < 0.1f)) {
				infoRadius = FIXED_RADIUS;
				infoHeight = FIXED_RADIUS;
				sizeless = true;
			} else {
				infoRadius = info.Radius;
				infoHeight = info.Height;
				sizeless = false;
			}

			// Find the sector in which the thing resides
			Thing.DetermineSector(mode.BlockMap);

			if(sprite != null)
			{
				if(Thing.Sector != null)
				{
					SectorData sd = mode.GetSectorData(Thing.Sector);
					SectorLevel level = sd.GetLevelAbove(new Vector3D(Thing.Position.x, Thing.Position.y, Thing.Position.z + Thing.Sector.FloorHeight));
					if(level != null)
					{
						// Use sector brightness for color shading
						PixelColor areabrightness = PixelColor.FromInt(mode.CalculateBrightness(level.brightnessbelow));
						PixelColor areacolor = PixelColor.Modulate(level.colorbelow, areabrightness);
						sectorcolor = areacolor.WithAlpha(255).ToInt();
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

					if(sizeless) { //mxd
						float hh = height / 2;
						verts[0] = new WorldVertex(-radius + offsetx, 0.0f, offsety - hh, sectorcolor, 0.0f, 1.0f);
						verts[1] = new WorldVertex(-radius + offsetx, 0.0f, hh + offsety, sectorcolor, 0.0f, 0.0f);
						verts[2] = new WorldVertex(+radius + offsetx, 0.0f, hh + offsety, sectorcolor, 1.0f, 0.0f);
						verts[3] = verts[0];
						verts[4] = verts[2];
						verts[5] = new WorldVertex(+radius + offsetx, 0.0f, offsety - hh, sectorcolor, 1.0f, 1.0f);
					} else {
						verts[0] = new WorldVertex(-radius + offsetx, 0.0f, offsety, sectorcolor, 0.0f, 1.0f);
						verts[1] = new WorldVertex(-radius + offsetx, 0.0f, height + offsety, sectorcolor, 0.0f, 0.0f);
						verts[2] = new WorldVertex(+radius + offsetx, 0.0f, height + offsety, sectorcolor, 1.0f, 0.0f);
						verts[3] = verts[0];
						verts[4] = verts[2];
						verts[5] = new WorldVertex(+radius + offsetx, 0.0f, offsety, sectorcolor, 1.0f, 1.0f);
					}
					SetVertices(verts);
				}
				else
				{
					base.Texture = General.Map.Data.Hourglass3D;

					// Determine sprite size
					float radius = Math.Min(infoRadius, infoHeight / 2f);
					float height = Math.Min(infoRadius * 2f, infoHeight);

					// Make vertices
					WorldVertex[] verts = new WorldVertex[6];
					verts[0] = new WorldVertex(-radius, 0.0f, 0.0f, sectorcolor, 0.0f, 1.0f);
					verts[1] = new WorldVertex(-radius, 0.0f, height, sectorcolor, 0.0f, 0.0f);
					verts[2] = new WorldVertex(+radius, 0.0f, height, sectorcolor, 1.0f, 0.0f);
					verts[3] = verts[0];
					verts[4] = verts[2];
					verts[5] = new WorldVertex(+radius, 0.0f, 0.0f, sectorcolor, 1.0f, 1.0f);
					SetVertices(verts);
				}
			}
			
			// Determine position
			Vector3D pos = Thing.Position;
			if(Thing.Type == 9501)
			{
				if(Thing.Sector != null) { //mxd
					// This is a special thing that needs special positioning
					SectorData sd = mode.GetSectorData(Thing.Sector);
					pos.z = sd.Ceiling.sector.CeilHeight + Thing.Position.z;
				}
			}
			else if(Thing.Type == 9500)
			{
				if(Thing.Sector != null) { //mxd
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
					if(Thing.Position.z > 0)
						pos.z = sd.Ceiling.plane.GetZ(Thing.Position) - info.Height;
					else
						pos.z = Thing.Sector.CeilHeight - info.Height; //mxd. was [pos.z = Thing.Sector.CeilHeight;]
				}

				pos.z -= Thing.Position.z;

				// Check if below floor
				if((Thing.Sector != null) && (pos.z < Thing.Sector.FloorHeight))
				{
					// Put thing on the floor
					SectorData sd = mode.GetSectorData(Thing.Sector);
					pos.z = sd.Floor.plane.GetZ(Thing.Position);
				}
			}
			else
			{
				// Stand on floor
				if(Thing.Sector != null)
				{
					SectorData sd = mode.GetSectorData(Thing.Sector);
					if(Thing.Position.z == 0)
						pos.z = sd.Floor.plane.GetZ(Thing.Position);
					else
						pos.z = Thing.Sector.FloorHeight;
				}

				pos.z += Thing.Position.z;

				// Check if above ceiling
				if((Thing.Sector != null) && ((pos.z + info.Height) > Thing.Sector.CeilHeight))
				{
					// Put thing against ceiling
					SectorData sd = mode.GetSectorData(Thing.Sector);
					pos.z = sd.Ceiling.plane.GetZ(Thing.Position) - info.Height;
				}
			}
			
			// Apply settings
			SetPosition(pos);
			SetCageSize(infoRadius, infoHeight);
			SetCageColor(Thing.Color);
			SetScale(info.SpriteScale.Width, info.SpriteScale.Height); //mxd

			// Keep info for object picking
			cageradius2 = infoRadius * Angle2D.SQRT2;
			cageradius2 = cageradius2 * cageradius2;
			pos2d = pos;

			if(sizeless) { //mxd
				boxp1 = new Vector3D(pos.x - infoRadius, pos.y - infoRadius, pos.z - infoRadius/2);
				boxp2 = new Vector3D(pos.x + infoRadius, pos.y + infoRadius, pos.z + infoRadius/2);
			} else {
				boxp1 = new Vector3D(pos.x - infoRadius, pos.y - infoRadius, pos.z);
				boxp2 = new Vector3D(pos.x + infoRadius, pos.y + infoRadius, pos.z + infoHeight);
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
			}
			
			base.Dispose();
		}
		
		#endregion
		
		#region ================== Methods
		
		// This forces to rebuild the whole thing
		public void Rebuild()
		{
			// Find thing information
			info = General.Map.Data.GetThingInfo(Thing.Type);

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
		public virtual bool IsSelected() {
			return selected;
		}
		
		#endregion

		#region ================== Events

		// Unused
		public virtual void OnSelectBegin() { }
		public virtual void OnEditBegin() { }
		public virtual void OnMouseMove(MouseEventArgs e) { }
		public virtual void OnChangeTargetBrightness(bool up) { }
		public virtual void OnChangeTextureOffset(int horizontal, int vertical, bool doSurfaceAngleCorrection) { }
		public virtual void OnChangeTextureScale(float incrementX, float incrementY) { } //mxd
		public virtual void OnSelectTexture() { }
		public virtual void OnCopyTexture() { }
		public virtual void OnPasteTexture() { }
		public virtual void OnCopyTextureOffsets() { }
		public virtual void OnPasteTextureOffsets() { }
		public virtual void OnTextureAlign(bool alignx, bool aligny) { }
		public virtual void OnToggleUpperUnpegged() { }
		public virtual void OnToggleLowerUnpegged() { }
		public virtual void OnResetTextureOffset() { }
		public virtual void OnResetLocalTextureOffset() { } //mxd
		public virtual void OnProcess(float deltatime) { }
		public virtual void OnTextureFloodfill() { }
		public virtual void OnInsert() { }
		public virtual void OnTextureFit(bool fitWidth, bool fitHeight) { } //mxd
		public virtual void ApplyTexture(string texture) { }
		public virtual void ApplyUpperUnpegged(bool set) { }
		public virtual void ApplyLowerUnpegged(bool set) { }
		public virtual void SelectNeighbours(bool select, bool withSameTexture, bool withSameHeight) { } //mxd
		
		// Return texture name
		public virtual string GetTextureName() { return ""; }

		// Select or deselect
		public virtual void OnSelectEnd()
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
		public virtual void OnDelete() {
			mode.CreateUndo("Delete thing");
			mode.SetActionResult("Deleted a thing.");

			this.Thing.Fields.BeforeFieldsChange();
			this.Thing.Dispose();
			this.Dispose();

			General.Map.IsChanged = true;
			General.Map.ThingsFilter.Update();
		}
		
		// Copy properties
		public virtual void OnCopyProperties()
		{
			BuilderPlug.Me.CopiedThingProps = new ThingProperties(Thing);
			mode.SetActionResult("Copied thing properties.");
		}
		
		// Paste properties
		public virtual void OnPasteProperties()
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
		public virtual void OnEditEnd()
		{
			if(General.Interface.IsActiveWindow)
			{
				
				List<Thing> things = mode.GetSelectedThings();
				//mxd
				updateList = new List<BaseVisualThing>();
				foreach(Thing t in things) {
					VisualThing vt = mode.GetVisualThing(t);
					if(vt != null)
						updateList.Add(vt as BaseVisualThing);
				}

				General.Interface.OnEditFormValuesChanged += new System.EventHandler(Interface_OnEditFormValuesChanged);
				mode.StartRealtimeInterfaceUpdate(SelectionType.Things);
				General.Interface.ShowEditThings(things);
				mode.StopRealtimeInterfaceUpdate(SelectionType.Things);
				General.Interface.OnEditFormValuesChanged -= Interface_OnEditFormValuesChanged;

				updateList.Clear();
				updateList = null;
			}
		}

		//mxd
		private void Interface_OnEditFormValuesChanged(object sender, System.EventArgs e) {
			foreach(BaseVisualThing vt in updateList)
				vt.Changed = true;
		}
		
		// Raise/lower thing
		public virtual void OnChangeTargetHeight(int amount)
		{
			if(General.Map.FormatInterface.HasThingHeight)
			{
				if((General.Map.UndoRedo.NextUndo == null) || (General.Map.UndoRedo.NextUndo.TicketID != undoticket))
					undoticket = mode.CreateUndo("Change thing height");

				Thing.Move(Thing.Position + new Vector3D(0.0f, 0.0f, amount));

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
		public void OnMove(Vector3D newPosition) {
			if ((General.Map.UndoRedo.NextUndo == null) || (General.Map.UndoRedo.NextUndo.TicketID != undoticket))
				undoticket = mode.CreateUndo("Move thing");
			Thing.Move(newPosition);
			mode.SetActionResult("Changed thing position to " + Thing.Position.ToString() + ".");

			// Update what must be updated
			ThingData td = mode.GetThingData(this.Thing);
			foreach (KeyValuePair<Sector, bool> s in td.UpdateAlso) {
				if (mode.VisualSectorExists(s.Key)) {
					BaseVisualSector vs = (BaseVisualSector)mode.GetVisualSector(s.Key);
					vs.UpdateSectorGeometry(s.Value);
				}
			}

			this.Changed = true;
		}

		//mxd
		public void Rotate(int ammount) {
			if ((General.Map.UndoRedo.NextUndo == null) || (General.Map.UndoRedo.NextUndo.TicketID != undoticket))
				undoticket = mode.CreateUndo("Rotate thing");
			Thing.Rotate(ammount);
			mode.SetActionResult("Changed thing rotation to " + Thing.AngleDoom.ToString() + ".");
			this.Changed = true;
		}
		
		#endregion
	}
}
