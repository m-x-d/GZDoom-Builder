
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
using System.Drawing;
using System.Windows.Forms;
using CodeImp.DoomBuilder.Data;
using CodeImp.DoomBuilder.IO;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Rendering;
using CodeImp.DoomBuilder.Geometry;
using CodeImp.DoomBuilder.VisualModes;
using CodeImp.DoomBuilder.Types;
using CodeImp.DoomBuilder.GZBuilder.Tools;

#endregion

namespace CodeImp.DoomBuilder.BuilderModes
{
	internal abstract class BaseVisualGeometrySector : VisualGeometry, IVisualEventReceiver
	{
		#region ================== Constants

		private const float DRAG_ANGLE_TOLERANCE = 0.06f;

		#endregion

		#region ================== Variables

		protected BaseVisualMode mode;
		protected long setuponloadedtexture;

		// This is only used to see if this object has already received a change
		// in a multiselection. The Changed property on the BaseVisualSector is
		// used to indicate a rebuild is needed.
		protected bool changed;

		protected SectorLevel level;
		protected Effect3DFloor extrafloor;
		
		// Undo/redo
		private int undoticket;
		
		// UV dragging
		private float dragstartanglexy;
		private float dragstartanglez;
		private Vector3D dragorigin;
		//private Vector3D deltaxy;
		//private Vector3D deltaz;
		private int startoffsetx;
		private int startoffsety;
		protected bool uvdragging;
		private int prevoffsetx;		// We have to provide delta offsets, but I don't
		private int prevoffsety;		// want to calculate with delta offsets to prevent
										// inaccuracy in the dragging.
		
		#endregion

		#region ================== Properties
		
		new public BaseVisualSector Sector { get { return (BaseVisualSector)base.Sector; } }
		public bool Changed { get { return changed; } set { changed = value; } }
		public SectorLevel Level { get { return level; } }
		public Effect3DFloor ExtraFloor { get { return extrafloor; } }

		#endregion

		#region ================== Constructor / Destructor

		// Constructor
		protected BaseVisualGeometrySector(BaseVisualMode mode, VisualSector vs) : base(vs)
		{
			this.mode = mode;
		}

		#endregion

		#region ================== Methods

		// This changes the height
		protected abstract void ChangeHeight(int amount);
		public virtual void SelectNeighbours(bool select, bool withSameTexture, bool withSameHeight) { } //mxd

		// This swaps triangles so that the plane faces the other way
		protected void SwapTriangleVertices(WorldVertex[] verts)
		{
			// Swap some vertices to flip all triangles
			for(int i = 0; i < verts.Length; i += 3)
			{
				// Swap
				WorldVertex v = verts[i];
				verts[i] = verts[i + 1];
				verts[i + 1] = v;
			}
		}

		// This is called to update UV dragging
		protected virtual void UpdateDragUV()
		{
			float u_ray = 1.0f;

			// Calculate intersection position
			this.Level.plane.GetIntersection(General.Map.VisualCamera.Position, General.Map.VisualCamera.Target, ref u_ray);
			Vector3D intersect = General.Map.VisualCamera.Position + (General.Map.VisualCamera.Target - General.Map.VisualCamera.Position) * u_ray;

			// Calculate offsets
			Vector3D dragdelta = intersect - dragorigin;
			float offsetx = dragdelta.x;
			float offsety = dragdelta.y;

            //mxd. Modify offsets based on surface and camera angles
            if (General.Map.UDMF) {
				float angle = 0;

				if(GeometryType == VisualGeometryType.CEILING)
					angle = Angle2D.DegToRad(level.sector.Fields.GetValue("rotationceiling", 0f));
				else
					angle = Angle2D.DegToRad(level.sector.Fields.GetValue("rotationfloor", 0f));

				Vector2D v = new Vector2D(offsetx, offsety).GetRotated(angle);

				offsetx = (int)Math.Round(v.x);
				offsety = (int)Math.Round(v.y);
            }

			// Apply offsets
			int newoffsetx = startoffsetx - (int)Math.Round(offsetx);
			int newoffsety = startoffsety + (int)Math.Round(offsety);
			mode.ApplyFlatOffsetChange(prevoffsetx - newoffsetx, prevoffsety - newoffsety);
			prevoffsetx = newoffsetx;
			prevoffsety = newoffsety;

			mode.ShowTargetInfo();
		}

        //mxd
        public override Sector GetControlSector() {
            return level.sector;
        }

		//mxd
		protected void onTextureChanged() {
			if(level.sector == this.Sector.Sector) {
				this.Setup();

				//check for 3d floors
				foreach(Sidedef s in level.sector.Sidedefs) {
					if(s.Line.Action == 160 && s.Line.Front != null) {
						int sectortag = s.Line.Args[0] + (s.Line.Args[4] << 8);
						foreach(Sector sector in General.Map.Map.Sectors) {
							if(sector.Tag == sectortag) {
								BaseVisualSector vs = (BaseVisualSector)mode.GetVisualSector(sector);
								vs.UpdateSectorGeometry(false);
							}
						}
					}
				}
			} else if(mode.VisualSectorExists(level.sector)) {
				BaseVisualSector vs = (BaseVisualSector)mode.GetVisualSector(level.sector);
				vs.UpdateSectorGeometry(false);
			}
		}

		//mxd
		public virtual bool IsSelected() {
			return selected;
		}

		//mxd
		protected void alignTextureToClosestLine(bool alignx, bool aligny) {
			//find a linedef to align to
			Vector2D hitpos = mode.GetHitPosition();
			if(!(mode.HighlightedObject is BaseVisualSector) || !hitpos.IsFinite())	return;
			bool isFront = false;

			//align to line of highlighted sector, which is closest to hitpos
			Sector highlightedSector = ((BaseVisualSector)mode.HighlightedObject).Sector;
			List<Linedef> lines = new List<Linedef>();
			foreach(Sidedef side in highlightedSector.Sidedefs)	lines.Add(side.Line);

			Linedef targetLine = MapSet.NearestLinedef(lines, hitpos);
			if(targetLine == null) return;

			foreach(Sidedef side in highlightedSector.Sidedefs) {
				if(side.Line == targetLine && side.Line.Front != null && side.Line.Front == side) {
					isFront = true;
					break;
				}
			}

			Sector.Sector.Fields.BeforeFieldsChange();

			//find an angle to rotate texture
			float sourceAngle = (float)Math.Round(General.ClampAngle(isFront ? -Angle2D.RadToDeg(targetLine.Angle) + 90 : -Angle2D.RadToDeg(targetLine.Angle) - 90), 1);
			if(!isFront) sourceAngle = General.ClampAngle(sourceAngle + 180);
			string rotationKey = (geoType == VisualGeometryType.FLOOR ? "rotationfloor" : "rotationceiling");

			//update angle
			UDMFTools.SetFloat(Sector.Sector.Fields, rotationKey, sourceAngle, 0f, false);

			//update scale. Target should be either floor or ceiling at this point
			float scaleX = 1.0f;
			float scaleY = 1.0f;

			if(mode.HighlightedTarget is VisualFloor) {
				VisualFloor target = mode.HighlightedTarget as VisualFloor;
				scaleX = target.Sector.Sector.Fields.GetValue("xscalefloor", 1.0f);
				scaleY = target.Sector.Sector.Fields.GetValue("yscalefloor", 1.0f);
			} else {
				VisualCeiling target = mode.HighlightedTarget as VisualCeiling;
				scaleX = target.Sector.Sector.Fields.GetValue("xscaleceiling", 1.0f);
				scaleY = target.Sector.Sector.Fields.GetValue("yscaleceiling", 1.0f);
			}

			string xScaleKey = (geoType == VisualGeometryType.FLOOR ? "xscalefloor" : "xscaleceiling");
			string yScaleKey = (geoType == VisualGeometryType.FLOOR ? "yscalefloor" : "yscaleceiling");

			//set scale
			UDMFTools.SetFloat(Sector.Sector.Fields, xScaleKey, scaleX, 1.0f, false);
			UDMFTools.SetFloat(Sector.Sector.Fields, yScaleKey, scaleY, 1.0f, false);

			//update offset
			float distToStart = Vector2D.Distance(hitpos, targetLine.Start.Position);
			float distToEnd = Vector2D.Distance(hitpos, targetLine.End.Position);
			Vector2D offset = (distToStart < distToEnd ? targetLine.Start.Position : targetLine.End.Position).GetRotated(Angle2D.DegToRad(sourceAngle));

			if(alignx) {
				if(Texture != null)	offset.x %= Texture.Width / scaleX;
				UDMFTools.SetFloat(Sector.Sector.Fields, (geoType == VisualGeometryType.FLOOR ? "xpanningfloor" : "xpanningceiling"), (float)Math.Round(-offset.x), 0f, false);
			}

			if(aligny) {
				if(Texture != null)	offset.y %= Texture.Height / scaleY;
				UDMFTools.SetFloat(Sector.Sector.Fields, (geoType == VisualGeometryType.FLOOR ? "ypanningfloor" : "ypanningceiling"), (float)Math.Round(offset.y), 0f, false);
			}

			//update geometry
			Sector.UpdateSectorGeometry(false);
		}

		//mxd
		protected void alignTextureToSlopeLine(Linedef slopeSource, float slopeAngle, bool isFront, bool alignx, bool aligny) {
			Vector2D hitpos = mode.GetHitPosition();
			bool isFloor = (geoType == VisualGeometryType.FLOOR);

			Sector.Sector.Fields.BeforeFieldsChange();
			
			float sourceAngle = (float)Math.Round(General.ClampAngle(isFront ? -Angle2D.RadToDeg(slopeSource.Angle) + 90 : -Angle2D.RadToDeg(slopeSource.Angle) - 90), 1);

			if(isFloor) {
				if((isFront && slopeSource.Front.Sector.FloorHeight > slopeSource.Back.Sector.FloorHeight) ||
				  (!isFront && slopeSource.Front.Sector.FloorHeight < slopeSource.Back.Sector.FloorHeight)) {
					sourceAngle = General.ClampAngle(sourceAngle + 180);
				}
			} else {
				if((isFront && slopeSource.Front.Sector.CeilHeight < slopeSource.Back.Sector.CeilHeight) ||
				  (!isFront && slopeSource.Front.Sector.CeilHeight > slopeSource.Back.Sector.CeilHeight)) {
					sourceAngle = General.ClampAngle(sourceAngle + 180);
				}
			}

			//update angle
			string rotationKey = (isFloor ? "rotationfloor" : "rotationceiling");
			UDMFTools.SetFloat(Sector.Sector.Fields, rotationKey, sourceAngle, 0f, false);

			//update scaleY
			string xScaleKey = (isFloor ? "xscalefloor" : "xscaleceiling");
			string yScaleKey = (isFloor ? "yscalefloor" : "yscaleceiling");

			float scaleX = Sector.Sector.Fields.GetValue(xScaleKey, 1.0f);
			float scaleY;// = (float)Math.Round(scaleX * (1 / (float)Math.Cos(slopeAngle)), 2);

			//set scale
			if(aligny) {
				scaleY = (float)Math.Round(scaleX * (1 / (float)Math.Cos(slopeAngle)), 2);
				UDMFTools.SetFloat(Sector.Sector.Fields, yScaleKey, scaleY, 1.0f, false);
			} else {
				scaleY = Sector.Sector.Fields.GetValue(yScaleKey, 1.0f);
			}

			//update texture offsets
			Vector2D offset;

			if(isFloor) {
				if((isFront && slopeSource.Front.Sector.FloorHeight < slopeSource.Back.Sector.FloorHeight) ||
				  (!isFront && slopeSource.Front.Sector.FloorHeight > slopeSource.Back.Sector.FloorHeight)) {
					offset = slopeSource.End.Position;
				} else {
					offset = slopeSource.Start.Position;
				}
			} else {
				if((isFront && slopeSource.Front.Sector.CeilHeight > slopeSource.Back.Sector.CeilHeight) ||
				  (!isFront && slopeSource.Front.Sector.CeilHeight < slopeSource.Back.Sector.CeilHeight)) {
					offset = slopeSource.End.Position;
				} else {
					offset = slopeSource.Start.Position;
				}
			}

			offset = offset.GetRotated(Angle2D.DegToRad(sourceAngle));

			if(alignx) {
				if(Texture != null)	offset.x %= Texture.Width / scaleX;
				UDMFTools.SetFloat(Sector.Sector.Fields, (isFloor ? "xpanningfloor" : "xpanningceiling"), (float)Math.Round(-offset.x), 0f, false);
			}

			if(aligny) {
				if(Texture != null)	offset.y %= Texture.Height / scaleY;
				UDMFTools.SetFloat(Sector.Sector.Fields, (isFloor ? "ypanningfloor" : "ypanningceiling"), (float)Math.Round(offset.y), 0f, false);
			}

			//update geometry
			Sector.UpdateSectorGeometry(false);
		}
		
		#endregion

		#region ================== Events

		// Unused
		public virtual void OnEditBegin() { }
		public virtual void OnTextureFit(bool fitWidth, bool fitHeight) { } //mxd
		public virtual void OnToggleUpperUnpegged() { }
		public virtual void OnToggleLowerUnpegged() { }
		public virtual void OnResetTextureOffset() { }
		public virtual void OnCopyTextureOffsets() { }
		public virtual void OnPasteTextureOffsets() { }
		public virtual void OnInsert() { }
		public virtual void OnDelete() { }
		protected virtual void SetTexture(string texturename) { }
		public virtual void ApplyUpperUnpegged(bool set) { }
		public virtual void ApplyLowerUnpegged(bool set) { }
		protected abstract void MoveTextureOffset(Point xy);
		protected abstract Point GetTextureOffset();

		// Setup this plane
		public bool Setup() { return this.Setup(this.level, this.extrafloor); }
		public virtual bool Setup(SectorLevel level, Effect3DFloor extrafloor)
		{
			this.level = level;
			this.extrafloor = extrafloor;
			return false;
		}

		// Begin select
		public virtual void OnSelectBegin()
		{
			mode.LockTarget();
			dragstartanglexy = General.Map.VisualCamera.AngleXY;
			dragstartanglez = General.Map.VisualCamera.AngleZ;
			dragorigin = pickintersect;
			startoffsetx = GetTextureOffset().X;
			startoffsety = GetTextureOffset().Y;
			prevoffsetx = GetTextureOffset().X;
			prevoffsety = GetTextureOffset().Y;
		}
		
		// Select or deselect
		public virtual void OnSelectEnd()
		{
			mode.UnlockTarget();
			
			// Was dragging?
			if(uvdragging)
			{
				// Dragging stops now
				uvdragging = false;
			}
			else
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
		}

		// Moving the mouse
		public virtual void OnMouseMove(MouseEventArgs e)
		{
			// Dragging UV?
			if(uvdragging)
			{
				UpdateDragUV();
			}
			else
			{
				// Select button pressed?
				if(General.Actions.CheckActionActive(General.ThisAssembly, "visualselect"))
				{
					// Check if tolerance is exceeded to start UV dragging
					float deltaxy = General.Map.VisualCamera.AngleXY - dragstartanglexy;
					float deltaz = General.Map.VisualCamera.AngleZ - dragstartanglez;
					if((Math.Abs(deltaxy) + Math.Abs(deltaz)) > DRAG_ANGLE_TOLERANCE)
					{
						if(General.Map.UDMF) { //mxd
							mode.PreAction(UndoGroup.TextureOffsetChange);
							mode.CreateUndo("Change texture offsets");

							// Start drag now
							uvdragging = true;
							mode.Renderer.ShowSelection = false;
							mode.Renderer.ShowHighlight = false;
							UpdateDragUV();
						}
					}
				}
			}
		}
		
		// Processing
		public virtual void OnProcess(float deltatime)
		{
			// If the texture was not loaded, but is loaded now, then re-setup geometry
			if(setuponloadedtexture != 0)
			{
				ImageData t = General.Map.Data.GetFlatImage(setuponloadedtexture);
				if(t != null)
				{
					if(t.IsImageLoaded)
					{
						setuponloadedtexture = 0;
						Setup();
					}
				}
			}
		}

		// Flood-fill textures
		public virtual void OnTextureFloodfill()
		{
			if(BuilderPlug.Me.CopiedFlat != null)
			{
				string oldtexture = GetTextureName();
				long oldtexturelong = Lump.MakeLongName(oldtexture);
				string newtexture = BuilderPlug.Me.CopiedFlat;
				if(newtexture != oldtexture)
				{
					// Get the texture
					ImageData newtextureimage = General.Map.Data.GetFlatImage(newtexture);
					if(newtextureimage != null)
					{
						bool fillceilings = (this is VisualCeiling);
						
						if(fillceilings)
						{
							mode.CreateUndo("Flood-fill ceilings with " + newtexture);
							mode.SetActionResult("Flood-filled ceilings with " + newtexture + ".");
						}
						else
						{
							mode.CreateUndo("Flood-fill floors with " + newtexture);
							mode.SetActionResult("Flood-filled floors with " + newtexture + ".");
						}

						mode.Renderer.SetCrosshairBusy(true);
						General.Interface.RedrawDisplay();

						if(mode.IsSingleSelection)
						{
							// Clear all marks, this will align everything it can
							General.Map.Map.ClearMarkedSectors(false);
						}
						else
						{
							// Limit the alignment to selection only
							General.Map.Map.ClearMarkedSectors(true);
							List<Sector> sectors = mode.GetSelectedSectors();
							foreach(Sector s in sectors) s.Marked = false;
						}
						
						// Do the fill
						Tools.FloodfillFlats(this.Sector.Sector, fillceilings, oldtexturelong, newtextureimage, false);

						// Get the changed sectors
						List<Sector> changes = General.Map.Map.GetMarkedSectors(true);
						foreach(Sector s in changes)
						{
							// Update the visual sector
							if(mode.VisualSectorExists(s))
							{
								BaseVisualSector vs = (mode.GetVisualSector(s) as BaseVisualSector);
								if(fillceilings)
									vs.Ceiling.Setup();
								else
									vs.Floor.Setup();
							}
						}

						General.Map.Data.UpdateUsedTextures();
						mode.Renderer.SetCrosshairBusy(false);
						mode.ShowTargetInfo();
					}
				}
			}
		}

		//mxd. Auto-align texture offsets
		public virtual void OnTextureAlign(bool alignx, bool aligny) {
			if(!General.Map.UDMF) return;

			//create undo
			string rest = string.Empty;
			if(alignx && aligny) rest = "(X and Y)";
			else if(alignx)	rest = "(X)";
			else rest = "(Y)";

			mode.CreateUndo("Auto-align textures " + rest);
			mode.SetActionResult("Auto-aligned textures " + rest + ".");

			//get selection
			List<VisualGeometry> selection = mode.GetSelectedSurfaces();

			//align textures on slopes
			foreach(VisualGeometry vg in selection) {
				if(vg.GeometryType == VisualGeometryType.FLOOR || vg.GeometryType == VisualGeometryType.CEILING) {
					if(vg.GeometryType == VisualGeometryType.FLOOR)
						((VisualFloor)vg).AlignTexture(alignx, aligny);
					else
						((VisualCeiling)vg).AlignTexture(alignx, aligny);

					vg.Sector.Sector.UpdateNeeded = true;
					vg.Sector.Sector.UpdateCache();
				}
			}

			// Map is changed
			General.Map.Map.Update();
			General.Map.IsChanged = true;
			General.Interface.RefreshInfo();
		}
		
		// Copy properties
		public virtual void OnCopyProperties()
		{
			BuilderPlug.Me.CopiedSectorProps = new SectorProperties(level.sector);
			mode.SetActionResult("Copied sector properties.");
		}
		
		// Paste properties
		public virtual void OnPasteProperties()
		{
			if(BuilderPlug.Me.CopiedSectorProps != null)
			{
				mode.CreateUndo("Paste sector properties");
				mode.SetActionResult("Pasted sector properties.");
				BuilderPlug.Me.CopiedSectorProps.Apply(level.sector);
				if(mode.VisualSectorExists(level.sector))
				{
					BaseVisualSector vs = (BaseVisualSector)mode.GetVisualSector(level.sector);
					vs.UpdateSectorGeometry(true);
				}
				mode.ShowTargetInfo();
			}
		}
		
		// Select texture
		public virtual void OnSelectTexture()
		{
			if(General.Interface.IsActiveWindow)
			{
				string oldtexture = GetTextureName();
				string newtexture = General.Interface.BrowseFlat(General.Interface, oldtexture);
				if(newtexture != oldtexture)
				{
					mode.ApplySelectTexture(newtexture, true);
				}
			}
		}

		// Apply Texture
		public virtual void ApplyTexture(string texture)
		{
			mode.CreateUndo("Change flat " + texture);
			SetTexture(texture);
			onTextureChanged(); //mxd
		}
		
		// Copy texture
		public virtual void OnCopyTexture()
		{
			BuilderPlug.Me.CopiedFlat = GetTextureName();
			if(General.Map.Config.MixTexturesFlats) BuilderPlug.Me.CopiedTexture = GetTextureName();
			mode.SetActionResult("Copied flat " + GetTextureName() + ".");
		}
		
		public virtual void OnPasteTexture() { }

		// Return texture name
		public virtual string GetTextureName() { return ""; }
		
		// Edit button released
		public virtual void OnEditEnd()
		{
			if(General.Interface.IsActiveWindow)
			{
				List<Sector> sectors = mode.GetSelectedSectors();
				DialogResult result = General.Interface.ShowEditSectors(sectors);
				if(result == DialogResult.OK)
				{
					// Rebuild sector
					foreach(Sector s in sectors)
					{
						if(mode.VisualSectorExists(s))
						{
							BaseVisualSector vs = (BaseVisualSector)mode.GetVisualSector(s);
							vs.UpdateSectorGeometry(true);
						}
					}
				}
			}
		}

		// Sector height change
		public virtual void OnChangeTargetHeight(int amount)
		{
			changed = true;

			ChangeHeight(amount);

			// Rebuild sector
			BaseVisualSector vs;
			if(mode.VisualSectorExists(level.sector)) {
				vs = (BaseVisualSector)mode.GetVisualSector(level.sector);
			} else {//mxd. Need this to apply changes to 3d-floor even if control sector doesn't exist as BaseVisualSector
				vs = mode.CreateBaseVisualSector(level.sector);
			}

			if(vs != null) vs.UpdateSectorGeometry(true);
		}
		
		// Sector brightness change
		public virtual void OnChangeTargetBrightness(bool up)
		{
			mode.CreateUndo("Change sector brightness", UndoGroup.SectorBrightnessChange, Sector.Sector.FixedIndex);
			
			if(up)
				Sector.Sector.Brightness = General.Map.Config.BrightnessLevels.GetNextHigher(Sector.Sector.Brightness);
			else
				Sector.Sector.Brightness = General.Map.Config.BrightnessLevels.GetNextLower(Sector.Sector.Brightness);
			
			mode.SetActionResult("Changed sector brightness to " + Sector.Sector.Brightness + ".");

			Sector.Sector.UpdateCache();

			// Rebuild sector
			Sector.UpdateSectorGeometry(false);
		}

		// Texture offset change
		public virtual void OnChangeTextureOffset(int horizontal, int vertical, bool doSurfaceAngleCorrection)
		{
			//mxd
            if (!General.Map.UDMF) {
				General.ShowErrorMessage("Floor/ceiling texture offsets cannot be changed in this map format!", MessageBoxButtons.OK);
				return;
			}

			if((General.Map.UndoRedo.NextUndo == null) || (General.Map.UndoRedo.NextUndo.TicketID != undoticket))
				undoticket = mode.CreateUndo("Change texture offsets");

			//mxd
			if(doSurfaceAngleCorrection) {
				Point p = new Point(horizontal, vertical);
				float angle = Angle2D.RadToDeg(General.Map.VisualCamera.AngleXY);
				if(GeometryType == VisualGeometryType.CEILING) {
					angle += level.sector.Fields.GetValue("rotationceiling", 0f);
				} else
					angle += level.sector.Fields.GetValue("rotationfloor", 0f);

				angle = General.ClampAngle(angle);

				if(angle > 315 || angle < 46) {

				} else if(angle > 225) {
					vertical = p.X;
					horizontal = -p.Y;
				} else if(angle > 135) {
					horizontal = -p.X;
					vertical = -p.Y;
				} else {
					vertical = -p.X;
					horizontal = p.Y;
				}
			}

			// Apply offsets
			MoveTextureOffset(new Point(-horizontal, -vertical));
			mode.SetActionResult("Changed texture offsets by " + (-horizontal) + ", " + (-vertical) + ".");

			// Update sector geometry
			Sector.UpdateSectorGeometry(false);
			Sector.Rebuild();
		}
		
		#endregion
	}
}
