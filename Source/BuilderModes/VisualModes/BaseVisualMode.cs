
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
			  UseByDefault = true)]

	public class BaseVisualMode : VisualMode
	{
		#region ================== Constants
		
		// Object picking interval
		private const double PICK_INTERVAL = 100.0d;
		private const float PICK_RANGE = 0.98f;
		
		#endregion
		
		#region ================== Variables
		
		// Object picking
		private VisualPickResult target;
		private double lastpicktime;
		private bool locktarget;
		
		#endregion
		
		#region ================== Properties

		#endregion
		
		#region ================== Constructor / Disposer

		// Constructor
		public BaseVisualMode()
		{
			// Initialize
			
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
			Vector3D start = CameraPosition;
			Vector3D delta = CameraTarget - CameraPosition;
			delta = delta.GetFixedLength(General.Settings.ViewDistance * PICK_RANGE);
			VisualPickResult newtarget = PickObject(start, start + delta);
			
			// Object changed?
			if(newtarget.picked != target.picked)
			{
				// Any result?
				if(newtarget.picked != null)
				{
					VisualGeometry prevgeo = null;
					VisualThing prevthing = null;
					if((target.picked != null) && (target.picked is VisualGeometry))
						prevgeo = (target.picked as VisualGeometry);
					else if(target.picked is VisualThing)
						prevthing = (target.picked as VisualThing);
					
					// Geometry picked?
					if(newtarget.picked is VisualGeometry)
					{
						VisualGeometry pickedgeo = (newtarget.picked as VisualGeometry);
						
						if(pickedgeo.Sidedef != null)
						{
							if((prevgeo == null) || (prevgeo.Sidedef == null)) General.Interface.HideInfo();
							General.Interface.ShowLinedefInfo(pickedgeo.Sidedef.Line);
						}
						else if(pickedgeo.Sidedef == null)
						{
							if((prevgeo == null) || (prevgeo.Sidedef != null)) General.Interface.HideInfo();
							General.Interface.ShowSectorInfo(pickedgeo.Sector.Sector);
						}
						else
						{
							General.Interface.HideInfo();
						}
					}
					// Thing picked?
					if(newtarget.picked is VisualThing)
					{
						VisualThing pickedthing = (newtarget.picked as VisualThing);
						
						if(prevthing == null) General.Interface.HideInfo();
						General.Interface.ShowThingInfo(pickedthing.Thing);
					}
				}
				else
				{
					General.Interface.HideInfo();
				}
			}
			
			// Apply new target
			target = newtarget;
		}
		
		#endregion
		
		#region ================== Events
		
		// Processing
		public override void OnProcess(double deltatime)
		{
			// Process things?
			base.ProcessThings = (BuilderPlug.Me.ShowVisualThings != 0);
			
			// Do processing
			base.OnProcess(deltatime);
			
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
			General.WriteLogLine("BeginSelect");
			PickTargetUnlocked();
			if(target.picked != null) (target.picked as IVisualEventReceiver).OnSelectBegin();
		}

		[EndAction("visualselect", BaseAction = true)]
		public void EndSelect()
		{
			General.WriteLogLine("EndSelect");
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
		}

		[BeginAction("raisesector8")]
		public void RaiseSector8()
		{
			PickTargetUnlocked();
			if(target.picked != null) (target.picked as IVisualEventReceiver).OnChangeTargetHeight(8);
		}

		[BeginAction("lowersector8")]
		public void LowerSector8()
		{
			PickTargetUnlocked();
			if(target.picked != null) (target.picked as IVisualEventReceiver).OnChangeTargetHeight(-8);
		}

		[BeginAction("raisesector1")]
		public void RaiseSector1()
		{
			PickTargetUnlocked();
			if(target.picked != null) (target.picked as IVisualEventReceiver).OnChangeTargetHeight(1);
		}
		
		[BeginAction("lowersector1")]
		public void LowerSector1()
		{
			PickTargetUnlocked();
			if(target.picked != null) (target.picked as IVisualEventReceiver).OnChangeTargetHeight(-1);
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
		}

		[BeginAction("lowerbrightness8")]
		public void LowerBrightness8()
		{
			PickTargetUnlocked();
			if(target.picked != null) (target.picked as IVisualEventReceiver).OnChangeTargetBrightness(-8);
		}
		
		#endregion
	}
}
