
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
using CodeImp.DoomBuilder.Actions;
using CodeImp.DoomBuilder.VisualModes;

#endregion

namespace CodeImp.DoomBuilder.BuilderModes
{
	[EditMode(DisplayName = "Visual Mode",
			  SwitchAction = "visualmode",		// Action name used to switch to this mode
			  ButtonImage = "VisualMode.png",	// Image resource name for the button
			  ButtonOrder = 0,					// Position of the button (lower is more to the left)
			  ButtonGroup = "001_visual",
			  UseByDefault = true)]

	public class BaseVisualMode : VisualMode
	{
		#region ================== Constants
		
		// Object picking
		private const double PICK_INTERVAL = 100.0d;
		private const float PICK_RANGE = 0.98f;

		// Gravity
		private const float GRAVITY = -0.06f;
		private const float CAMERA_FLOOR_OFFSET = 41f;		// same as in doom
		private const float CAMERA_CEILING_OFFSET = 10f;
		
		#endregion
		
		#region ================== Variables
		
		// Object picking
		private VisualPickResult target;
		private double lastpicktime;
		private bool locktarget;
		
		// Texture copying
		private string copiedtexture;
		private string copiedflat;
		
		// Gravity vector
		private Vector3D gravity;
		
		#endregion
		
		#region ================== Properties

		public IRenderer3D Renderer { get { return renderer; } }
		public string CopiedTexture { get { return copiedtexture; } set { copiedtexture = value; } }
		public string CopiedFlat { get { return copiedflat; } set { copiedflat = value; } }

		#endregion
		
		#region ================== Constructor / Disposer

		// Constructor
		public BaseVisualMode()
		{
			// Initialize
			this.gravity = new Vector3D(0.0f, 0.0f, 0.0f);
			
			// We have no destructor
			GC.SuppressFinalize(this);
		}

		// Disposer
		public override void Dispose()
		{
			// Not already disposed?
			if(!isdisposed)
			{
				// Clean up
				
				// Done
				base.Dispose();
			}
		}

		#endregion
		
		#region ================== Methods
		
		// This creates a visual sector
		protected override VisualSector CreateVisualSector(Sector s)
		{
			BaseVisualSector vs = new BaseVisualSector(this, s);
			return vs;
		}
		
		// This creates a visual thing
		protected override VisualThing CreateVisualThing(Thing t)
		{
			BaseVisualThing vt = new BaseVisualThing(this, t);
			if(vt.Setup()) return vt; else return null;
		}
		
		// This locks the target so that it isn't changed until unlocked
		public void LockTarget()
		{
			locktarget = true;
		}
		
		// This unlocks the target so that is changes to the aimed geometry again
		public void UnlockTarget()
		{
			locktarget = false;
		}
		
		// This picks a new target, if not locked
		private void PickTargetUnlocked()
		{
			if(!locktarget) PickTarget();
		}
		
		// This picks a new target
		private void PickTarget()
		{
			// Find the object we are aiming at
			Vector3D start = General.Map.VisualCamera.Position;
			Vector3D delta = General.Map.VisualCamera.Target - General.Map.VisualCamera.Position;
			delta = delta.GetFixedLength(General.Settings.ViewDistance * PICK_RANGE);
			VisualPickResult newtarget = PickObject(start, start + delta);
			
			// Should we update the info on panels?
			bool updateinfo = (newtarget.picked != target.picked);
			
			// Apply new target
			target = newtarget;

			// Show target info
			if(updateinfo) ShowTargetInfo();
		}

		// This shows the picked target information
		public void ShowTargetInfo()
		{
			// Any result?
			if(target.picked != null)
			{
				// Geometry picked?
				if(target.picked is VisualGeometry)
				{
					VisualGeometry pickedgeo = (target.picked as VisualGeometry);

					if(pickedgeo.Sidedef != null)
						General.Interface.ShowLinedefInfo(pickedgeo.Sidedef.Line);
					else if(pickedgeo.Sidedef == null)
						General.Interface.ShowSectorInfo(pickedgeo.Sector.Sector);
					else
						General.Interface.HideInfo();
				}
				// Thing picked?
				if(target.picked is VisualThing)
				{
					VisualThing pickedthing = (target.picked as VisualThing);
					General.Interface.ShowThingInfo(pickedthing.Thing);
				}
			}
			else
			{
				General.Interface.HideInfo();
			}
		}
		
		#endregion
		
		#region ================== Events
		
		// Processing
		public override void OnProcess(double deltatime)
		{
			// Process things?
			base.ProcessThings = (BuilderPlug.Me.ShowVisualThings != 0);
			
			// Setup the move multiplier depending on gravity
			Vector3D movemultiplier = new Vector3D(1.0f, 1.0f, 1.0f);
			if(BuilderPlug.Me.UseGravity) movemultiplier.z = 0.0f;
			General.Map.VisualCamera.MoveMultiplier = movemultiplier;
			
			// Apply gravity?
			if(BuilderPlug.Me.UseGravity && (General.Map.VisualCamera.Sector != null))
			{
				// Camera below floor level?
				if(General.Map.VisualCamera.Position.z <= (General.Map.VisualCamera.Sector.FloorHeight + CAMERA_FLOOR_OFFSET + 0.1f))
				{
					// Stay above floor
					gravity = new Vector3D(0.0f, 0.0f, 0.0f);
					General.Map.VisualCamera.Position = new Vector3D(General.Map.VisualCamera.Position.x,
																	 General.Map.VisualCamera.Position.y,
																	 General.Map.VisualCamera.Sector.FloorHeight + CAMERA_FLOOR_OFFSET);
				}
				else
				{
					// Fall down
					gravity += new Vector3D(0.0f, 0.0f, (float)(GRAVITY * deltatime));
					General.Map.VisualCamera.Position += gravity;
				}
				
				// Camera above ceiling level?
				if(General.Map.VisualCamera.Position.z >= (General.Map.VisualCamera.Sector.CeilHeight - CAMERA_CEILING_OFFSET - 0.1f))
				{
					// Stay below ceiling
					General.Map.VisualCamera.Position = new Vector3D(General.Map.VisualCamera.Position.x,
																	 General.Map.VisualCamera.Position.y,
																	 General.Map.VisualCamera.Sector.CeilHeight - CAMERA_CEILING_OFFSET);
				}
			}
			else
			{
				gravity = new Vector3D(0.0f, 0.0f, 0.0f);
			}
			
			// Do processing
			base.OnProcess(deltatime);

			// Process visible geometry
			foreach(IVisualEventReceiver g in visiblegeometry)
			{
				g.OnProcess(deltatime);
			}
			
			// Time to pick a new target?
			if(General.Clock.CurrentTime > (lastpicktime + PICK_INTERVAL))
			{
				PickTargetUnlocked();
				lastpicktime = General.Clock.CurrentTime;
			}
			
			// The mouse is always in motion
			MouseEventArgs args = new MouseEventArgs(General.Interface.MouseButtons, 0, 0, 0, 0);
			OnMouseMove(args);
		}
		
		// This draws a frame
		public override void OnRedrawDisplay()
		{
			// Start drawing
			if(renderer.Start())
			{
				// Use fog!
				renderer.SetFogMode(true);
				
				// Begin with geometry
				renderer.StartGeometry();

				// Render all visible sectors
				foreach(VisualGeometry g in visiblegeometry)
					renderer.AddSectorGeometry(g);

				if(BuilderPlug.Me.ShowVisualThings != 0)
				{
					// Render things in cages?
					renderer.DrawThingCages = ((BuilderPlug.Me.ShowVisualThings & 2) != 0);
					
					// Render all visible things
					foreach(VisualThing t in visiblethings)
						renderer.AddThingGeometry(t);
				}
				
				// Done rendering geometry
				renderer.FinishGeometry();
				
				// Render crosshair
				renderer.RenderCrosshair();
				
				// Present!
				renderer.Finish();
			}
		}
		
		// After resources were reloaded
		protected override void ResourcesReloaded()
		{
			base.ResourcesReloaded();
			PickTarget();
		}
		
		// Mouse moves
		public override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);
			if(target.picked != null) (target.picked as IVisualEventReceiver).OnMouseMove(e);
		}
		
		#endregion

		#region ================== Actions

		[BeginAction("visualselect", BaseAction = true)]
		public void BeginSelect()
		{
			PickTargetUnlocked();
			if(target.picked != null) (target.picked as IVisualEventReceiver).OnSelectBegin();
		}

		[EndAction("visualselect", BaseAction = true)]
		public void EndSelect()
		{
			if(target.picked != null) (target.picked as IVisualEventReceiver).OnSelectEnd();
		}

		[BeginAction("visualedit", BaseAction = true)]
		public void BeginEdit()
		{
			PickTargetUnlocked();
			if(target.picked != null) (target.picked as IVisualEventReceiver).OnEditBegin();
		}

		[EndAction("visualedit", BaseAction = true)]
		public void EndEdit()
		{
			if(target.picked != null) (target.picked as IVisualEventReceiver).OnEditEnd();
			ShowTargetInfo();
		}

		[BeginAction("raisesector8")]
		public void RaiseSector8()
		{
			PickTargetUnlocked();
			if(target.picked != null) (target.picked as IVisualEventReceiver).OnChangeTargetHeight(8);
			ShowTargetInfo();
		}

		[BeginAction("lowersector8")]
		public void LowerSector8()
		{
			PickTargetUnlocked();
			if(target.picked != null) (target.picked as IVisualEventReceiver).OnChangeTargetHeight(-8);
			ShowTargetInfo();
		}

		[BeginAction("raisesector1")]
		public void RaiseSector1()
		{
			PickTargetUnlocked();
			if(target.picked != null) (target.picked as IVisualEventReceiver).OnChangeTargetHeight(1);
			ShowTargetInfo();
		}
		
		[BeginAction("lowersector1")]
		public void LowerSector1()
		{
			PickTargetUnlocked();
			if(target.picked != null) (target.picked as IVisualEventReceiver).OnChangeTargetHeight(-1);
			ShowTargetInfo();
		}

		[BeginAction("showvisualthings")]
		public void ShowVisualThings()
		{
			BuilderPlug.Me.ShowVisualThings++;
			if(BuilderPlug.Me.ShowVisualThings > 2) BuilderPlug.Me.ShowVisualThings = 0;
		}

		[BeginAction("raisebrightness8")]
		public void RaiseBrightness8()
		{
			PickTargetUnlocked();
			if(target.picked != null) (target.picked as IVisualEventReceiver).OnChangeTargetBrightness(8);
			ShowTargetInfo();
		}

		[BeginAction("lowerbrightness8")]
		public void LowerBrightness8()
		{
			PickTargetUnlocked();
			if(target.picked != null) (target.picked as IVisualEventReceiver).OnChangeTargetBrightness(-8);
			ShowTargetInfo();
		}

		[BeginAction("movetextureleft")]
		public void MoveTextureLeft1()
		{
			PickTargetUnlocked();
			if(target.picked != null) (target.picked as IVisualEventReceiver).OnChangeTextureOffset(-1, 0);
			ShowTargetInfo();
		}

		[BeginAction("movetextureright")]
		public void MoveTextureRight1()
		{
			PickTargetUnlocked();
			if(target.picked != null) (target.picked as IVisualEventReceiver).OnChangeTextureOffset(1, 0);
			ShowTargetInfo();
		}

		[BeginAction("movetextureup")]
		public void MoveTextureUp1()
		{
			PickTargetUnlocked();
			if(target.picked != null) (target.picked as IVisualEventReceiver).OnChangeTextureOffset(0, -1);
			ShowTargetInfo();
		}

		[BeginAction("movetexturedown")]
		public void MoveTextureDown1()
		{
			PickTargetUnlocked();
			if(target.picked != null) (target.picked as IVisualEventReceiver).OnChangeTextureOffset(0, 1);
			ShowTargetInfo();
		}

		[BeginAction("movetextureleft8")]
		public void MoveTextureLeft8()
		{
			PickTargetUnlocked();
			if(target.picked != null) (target.picked as IVisualEventReceiver).OnChangeTextureOffset(-8, 0);
			ShowTargetInfo();
		}

		[BeginAction("movetextureright8")]
		public void MoveTextureRight8()
		{
			PickTargetUnlocked();
			if(target.picked != null) (target.picked as IVisualEventReceiver).OnChangeTextureOffset(8, 0);
			ShowTargetInfo();
		}

		[BeginAction("movetextureup8")]
		public void MoveTextureUp8()
		{
			PickTargetUnlocked();
			if(target.picked != null) (target.picked as IVisualEventReceiver).OnChangeTextureOffset(0, -8);
			ShowTargetInfo();
		}

		[BeginAction("movetexturedown8")]
		public void MoveTextureDown8()
		{
			PickTargetUnlocked();
			if(target.picked != null) (target.picked as IVisualEventReceiver).OnChangeTextureOffset(0, 8);
			ShowTargetInfo();
		}

		[BeginAction("textureselect")]
		public void TextureSelect()
		{
			PickTargetUnlocked();
			renderer.SetCrosshairBusy(true);
			General.Interface.RedrawDisplay();
			if(target.picked != null) (target.picked as IVisualEventReceiver).OnSelectTexture();
			renderer.SetCrosshairBusy(false);
			ShowTargetInfo();
		}

		[BeginAction("texturecopy")]
		public void TextureCopy()
		{
			PickTargetUnlocked();
			if(target.picked != null) (target.picked as IVisualEventReceiver).OnCopyTexture();
			ShowTargetInfo();
		}

		[BeginAction("texturepaste")]
		public void TexturePaste()
		{
			PickTargetUnlocked();
			if(target.picked != null) (target.picked as IVisualEventReceiver).OnPasteTexture();
			ShowTargetInfo();
		}

		[BeginAction("visualautoalignx")]
		public void TextureAutoAlignX()
		{
			PickTargetUnlocked();
			renderer.SetCrosshairBusy(true);
			General.Interface.RedrawDisplay();
			if(target.picked != null) (target.picked as IVisualEventReceiver).OnTextureAlign(true, false);
			renderer.SetCrosshairBusy(false);
			ShowTargetInfo();
		}

		[BeginAction("visualautoaligny")]
		public void TextureAutoAlignY()
		{
			PickTargetUnlocked();
			renderer.SetCrosshairBusy(true);
			General.Interface.RedrawDisplay();
			if(target.picked != null) (target.picked as IVisualEventReceiver).OnTextureAlign(false, true);
			renderer.SetCrosshairBusy(false);
			ShowTargetInfo();
		}

		[BeginAction("toggleupperunpegged")]
		public void ToggleUpperUnpegged()
		{
			PickTargetUnlocked();
			if(target.picked != null) (target.picked as IVisualEventReceiver).OnToggleUpperUnpegged();
			ShowTargetInfo();
		}

		[BeginAction("togglelowerunpegged")]
		public void ToggleLowerUnpegged()
		{
			PickTargetUnlocked();
			if(target.picked != null) (target.picked as IVisualEventReceiver).OnToggleLowerUnpegged();
			ShowTargetInfo();
		}

		[BeginAction("togglegravity")]
		public void ToggleGravity()
		{
			BuilderPlug.Me.UseGravity = !BuilderPlug.Me.UseGravity;
		}

		[BeginAction("togglebrightness")]
		public void ToggleBrightness()
		{
			renderer.FullBrightness = !renderer.FullBrightness;
		}

		[BeginAction("resettexture")]
		public void ResetTexture()
		{
			PickTargetUnlocked();
			if(target.picked != null) (target.picked as IVisualEventReceiver).OnResetTextureOffset();
			ShowTargetInfo();
		}

		[BeginAction("floodfilltextures")]
		public void FloodfillTextures()
		{
			PickTargetUnlocked();
			if(target.picked != null) (target.picked as IVisualEventReceiver).OnTextureFloodfill();
			ShowTargetInfo();
		}
		
		#endregion
	}
}
