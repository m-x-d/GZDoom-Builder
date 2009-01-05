
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
using CodeImp.DoomBuilder.Windows;
using CodeImp.DoomBuilder.IO;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Rendering;
using CodeImp.DoomBuilder.Geometry;
using CodeImp.DoomBuilder.Editing;
using CodeImp.DoomBuilder.VisualModes;

#endregion

namespace CodeImp.DoomBuilder.BuilderModes
{
	internal abstract class BaseVisualGeometrySidedef : VisualGeometry, IVisualEventReceiver
	{
		#region ================== Constants
		
		private const float DRAG_ANGLE_TOLERANCE = 0.06f;
		
		#endregion

		#region ================== Variables

		protected BaseVisualMode mode;

		protected float top;
		protected float bottom;
		
		// UV dragging
		private float dragstartanglexy;
		private float dragstartanglez;
		private Vector3D dragorigin;
		private Vector3D deltaxy;
		private Vector3D deltaz;
		private int startoffsetx;
		private int startoffsety;
		protected bool uvdragging;

		// Undo/redo
		private int undoticket;
		
		#endregion
		
		#region ================== Properties
		
		public bool IsDraggingUV { get { return uvdragging; } }
		new public BaseVisualSector Sector { get { return (BaseVisualSector)base.Sector; } }
		
		#endregion
		
		#region ================== Constructor / Destructor
		
		// Constructor for sidedefs
		public BaseVisualGeometrySidedef(BaseVisualMode mode, VisualSector vs, Sidedef sd) : base(vs, sd)
		{
			this.mode = mode;
			this.deltaz = new Vector3D(0.0f, 0.0f, 1.0f);
			this.deltaxy = (sd.Line.End.Position - sd.Line.Start.Position) * sd.Line.LengthInv;
			if(!sd.IsFront) this.deltaxy = -this.deltaxy;
		}
		
		#endregion

		#region ================== Methods

		// This performs a fast test in object picking
		public override bool PickFastReject(Vector3D from, Vector3D to, Vector3D dir)
		{
			// Check if intersection point is between top and bottom
			return (pickintersect.z >= bottom) && (pickintersect.z <= top);
		}

		// This performs an accurate test for object picking
		public override bool PickAccurate(Vector3D from, Vector3D to, Vector3D dir, ref float u_ray)
		{
			// The fast reject pass is already as accurate as it gets,
			// so we just return the intersection distance here
			u_ray = pickrayu;
			return true;
		}
		
		#endregion

		#region ================== Events
		
		// Unused
		public virtual void OnEditBegin() { }
		public virtual void OnChangeTargetHeight(int amount) { }
		protected virtual void SetTexture(string texturename) { }
		
		// Reset texture offsets
		public virtual void OnResetTextureOffset()
		{
			General.Map.UndoRedo.CreateUndo("Reset texture offsets");

			// Apply offsets
			Sidedef.OffsetX = 0;
			Sidedef.OffsetY = 0;

			// Update sidedef geometry
			VisualSidedefParts parts = Sector.GetSidedefParts(Sidedef);
			if(parts.lower != null) parts.lower.Setup();
			if(parts.middledouble != null) parts.middledouble.Setup();
			if(parts.middlesingle != null) parts.middlesingle.Setup();
			if(parts.upper != null) parts.upper.Setup();
		}
		
		// Toggle upper-unpegged
		public virtual void OnToggleUpperUnpegged()
		{
			if(this.Sidedef.Line.Flags.ContainsKey(General.Map.Config.UpperUnpeggedFlag) &&
			   this.Sidedef.Line.Flags[General.Map.Config.UpperUnpeggedFlag])
			{
				// Remove flag
				General.Map.UndoRedo.CreateUndo("Remove upper-unpegged setting");
				this.Sidedef.Line.Flags[General.Map.Config.UpperUnpeggedFlag] = false;
			}
			else
			{
				// Add flag
				General.Map.UndoRedo.CreateUndo("Set upper-unpegged setting");
				this.Sidedef.Line.Flags[General.Map.Config.UpperUnpeggedFlag] = true;
			}
			
			// Update sidedef geometry
			VisualSidedefParts parts = Sector.GetSidedefParts(Sidedef);
			if(parts.lower != null) parts.lower.Setup();
			if(parts.middledouble != null) parts.middledouble.Setup();
			if(parts.middlesingle != null) parts.middlesingle.Setup();
			if(parts.upper != null) parts.upper.Setup();
		}

		// Toggle lower-unpegged
		public virtual void OnToggleLowerUnpegged()
		{
			if(this.Sidedef.Line.Flags.ContainsKey(General.Map.Config.LowerUnpeggedFlag) &&
			   this.Sidedef.Line.Flags[General.Map.Config.LowerUnpeggedFlag])
			{
				// Remove flag
				General.Map.UndoRedo.CreateUndo("Remove lower-unpegged setting");
				this.Sidedef.Line.Flags[General.Map.Config.LowerUnpeggedFlag] = false;
			}
			else
			{
				// Add flag
				General.Map.UndoRedo.CreateUndo("Set lower-unpegged setting");
				this.Sidedef.Line.Flags[General.Map.Config.LowerUnpeggedFlag] = true;
			}
			
			// Update sidedef geometry
			VisualSidedefParts parts = Sector.GetSidedefParts(Sidedef);
			if(parts.lower != null) parts.lower.Setup();
			if(parts.middledouble != null) parts.middledouble.Setup();
			if(parts.middlesingle != null) parts.middlesingle.Setup();
			if(parts.upper != null) parts.upper.Setup();
		}
		
		// Auto-align texture X offsets
		public virtual void OnTextureAlign(bool alignx, bool aligny)
		{
			General.Map.UndoRedo.CreateUndo("Auto-align textures");
			
			// Get the texture long name
			string texname = GetTextureName();
			long longtexname = General.Map.Data.GetLongImageName(texname);

			// Do the alignment
			Tools.AutoAlignTextures(this.Sidedef, longtexname, alignx, aligny, true);

			// Get the changed sidedefs
			List<Sidedef> changes = General.Map.Map.GetMarkedSidedefs(true);
			foreach(Sidedef sd in changes)
			{
				// Update the parts for this sidedef!
				if(mode.VisualSectorExists(sd.Sector))
				{
					BaseVisualSector vs = (mode.GetVisualSector(sd.Sector) as BaseVisualSector);
					VisualSidedefParts parts = vs.GetSidedefParts(sd);
					if(parts.lower != null) parts.lower.Setup();
					if(parts.middledouble != null) parts.middledouble.Setup();
					if(parts.middlesingle != null) parts.middlesingle.Setup();
					if(parts.upper != null) parts.upper.Setup();
				}
			}
		}
		
		// Select texture
		public virtual void OnSelectTexture()
		{
			string oldtexture = GetTextureName();
			string newtexture = General.Interface.BrowseTexture(General.Interface, oldtexture);
			if(newtexture != oldtexture)
			{
				General.Map.UndoRedo.CreateUndo("Change texture " + newtexture);
				SetTexture(newtexture);
			}
		}
		
		// Paste texture
		public virtual void OnPasteTexture()
		{
			if(mode.CopiedTexture != null)
			{
				General.Map.UndoRedo.CreateUndo("Paste texture " + mode.CopiedTexture);
				SetTexture(mode.CopiedTexture);
			}
		}

		// Copy texture
		public virtual void OnCopyTexture()
		{
			mode.CopiedTexture = GetTextureName();
			if(General.Map.Config.MixTexturesFlats) mode.CopiedFlat = GetTextureName();
		}

		// Return texture name
		public virtual string GetTextureName() { return ""; }
		
		// Select button pressed
		public virtual void OnSelectBegin()
		{
			dragstartanglexy = mode.CameraAngleXY;
			dragstartanglez = mode.CameraAngleZ;
			dragorigin = pickintersect;
			startoffsetx = Sidedef.OffsetX;
			startoffsety = Sidedef.OffsetY;
		}
		
		// Select button released
		public virtual void OnSelectEnd()
		{
			// Was dragging?
			if(uvdragging)
			{
				// Dragging stops now
				mode.UnlockTarget();
				uvdragging = false;
			}
			else
			{
				// Add/remove selection
			}
		}
		
		// Edit button released
		public virtual void OnEditEnd()
		{
			// Not using any modifier buttons
			if(!General.Interface.ShiftState && !General.Interface.CtrlState && !General.Interface.AltState)
			{
				List<Linedef> lines = new List<Linedef>();
				lines.Add(this.Sidedef.Line);
				DialogResult result = General.Interface.ShowEditLinedefs(lines);
				if(result == DialogResult.OK) (this.Sector as BaseVisualSector).Rebuild();
			}
		}
		
		// Mouse moves
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
					float deltaxy = mode.CameraAngleXY - dragstartanglexy;
					float deltaz = mode.CameraAngleZ - dragstartanglez;
					if((Math.Abs(deltaxy) + Math.Abs(deltaz)) > DRAG_ANGLE_TOLERANCE)
					{
						General.Map.UndoRedo.CreateUndo("Change texture offsets");

						// Start drag now
						uvdragging = true;
						mode.LockTarget();
						UpdateDragUV();
					}
				}
			}
		}
		
		// This is called to update UV dragging
		protected virtual void UpdateDragUV()
		{
			float u_ray;
			
			// Calculate intersection position
			Line2D ray = new Line2D(mode.CameraPosition, mode.CameraTarget);
			Sidedef.Line.Line.GetIntersection(ray, out u_ray);
			Vector3D intersect = mode.CameraPosition + (mode.CameraTarget - mode.CameraPosition) * u_ray;
			
			// Calculate offsets
			Vector3D dragdelta = intersect - dragorigin;
			Vector3D dragdeltaxy = dragdelta * deltaxy;
			Vector3D dragdeltaz = dragdelta * deltaz;
			float offsetx = dragdeltaxy.GetLength();
			float offsety = dragdeltaz.GetLength();
			if((Math.Sign(dragdeltaxy.x) < 0) || (Math.Sign(dragdeltaxy.y) < 0) || (Math.Sign(dragdeltaxy.z) < 0)) offsetx = -offsetx;
			if((Math.Sign(dragdeltaz.x) < 0) || (Math.Sign(dragdeltaz.y) < 0) || (Math.Sign(dragdeltaz.z) < 0)) offsety = -offsety;
			
			// Apply offsets
			Sidedef.OffsetX = startoffsetx - (int)Math.Round(offsetx);
			Sidedef.OffsetY = startoffsety + (int)Math.Round(offsety);
			
			// Update sidedef geometry
			VisualSidedefParts parts = Sector.GetSidedefParts(Sidedef);
			if(parts.lower != null) parts.lower.Setup();
			if(parts.middledouble != null) parts.middledouble.Setup();
			if(parts.middlesingle != null) parts.middlesingle.Setup();
			if(parts.upper != null) parts.upper.Setup();
			mode.ShowTargetInfo();
		}
		
		// Sector brightness change
		public virtual void OnChangeTargetBrightness(int amount)
		{
			// Change brightness
			General.Map.UndoRedo.CreateUndo("Change sector brightness", UndoGroup.SectorBrightnessChange, Sector.Sector.Index);
			Sector.Sector.Brightness = General.Clamp(Sector.Sector.Brightness + amount, 0, 255);
			
			// Rebuild sector
			Sector.Rebuild();

			// Go for all things in this sector
			foreach(Thing t in General.Map.Map.Things)
			{
				if(t.Sector == Sector.Sector)
				{
					if(mode.VisualThingExists(t))
					{
						// Update thing
						BaseVisualThing vt = (mode.GetVisualThing(t) as BaseVisualThing);
						vt.Setup();
					}
				}
			}
		}
		
		// Texture offset change
		public virtual void OnChangeTextureOffset(int horizontal, int vertical)
		{
			if((General.Map.UndoRedo.NextUndo == null) || (General.Map.UndoRedo.NextUndo.TicketID != undoticket))
				undoticket = General.Map.UndoRedo.CreateUndo("Change texture offsets");
			
			// Apply offsets
			Sidedef.OffsetX -= horizontal;
			Sidedef.OffsetY -= vertical;

			// Update sidedef geometry
			VisualSidedefParts parts = Sector.GetSidedefParts(Sidedef);
			if(parts.lower != null) parts.lower.Setup();
			if(parts.middledouble != null) parts.middledouble.Setup();
			if(parts.middlesingle != null) parts.middlesingle.Setup();
			if(parts.upper != null) parts.upper.Setup();
		}
		
		#endregion
	}
}
