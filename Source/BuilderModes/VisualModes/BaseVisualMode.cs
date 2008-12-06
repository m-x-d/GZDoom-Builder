
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
			return new BaseVisualSector(s);
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
		
		// This picks a new target
		private void PickTarget()
		{
			// Find the object we are aiming at
			Vector3D start = CameraPosition;
			Vector3D delta = CameraTarget - CameraPosition;
			delta = delta.GetFixedLength(General.Settings.ViewDistance * PICK_RANGE);
			VisualPickResult newtarget = PickObject(start, start + delta);
			
			// Object changed?
			if(newtarget.geometry != target.geometry)
			{
				// Any result?
				if(newtarget.geometry != null)
				{
					if(newtarget.geometry.Sidedef != null)
					{
						if((target.geometry != null) && (target.geometry.Sidedef == null)) General.Interface.HideInfo();
						General.Interface.ShowLinedefInfo(newtarget.geometry.Sidedef.Line);
					}
					else if(newtarget.geometry.Sidedef == null)
					{
						if((target.geometry == null) || (target.geometry.Sidedef != null)) General.Interface.HideInfo();
						General.Interface.ShowSectorInfo(newtarget.geometry.Sector.Sector);
					}
					else
					{
						General.Interface.HideInfo();
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

		// This changes the target's height
		private void ChangeTargetHeight(int amount)
		{
			if(target.geometry is BaseVisualGeometrySector)
			{
				BaseVisualGeometrySector vgs = (target.geometry as BaseVisualGeometrySector);
				vgs.ChangeHeight(amount);

				// Rebuild sector
				(vgs.Sector as BaseVisualSector).Rebuild();

				// Also rebuild surrounding sectors, because outside sidedefs may need to be adjusted
				foreach(Sidedef sd in vgs.Sector.Sector.Sidedefs)
				{
					if((sd.Other != null) && VisualSectorExists(sd.Other.Sector))
					{
						BaseVisualSector bvs = (BaseVisualSector)GetVisualSector(sd.Other.Sector);
						bvs.Rebuild();
					}
				}
			}
		}
		
		#endregion
		
		#region ================== Events
		
		// Processing
		public override void OnProcess(double deltatime)
		{
			// Do processing
			base.OnProcess(deltatime);
			
			// Time to pick a new target?
			if(General.Clock.CurrentTime > (lastpicktime + PICK_INTERVAL))
			{
				if(!locktarget) PickTarget();
				lastpicktime = General.Clock.CurrentTime;
			}
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
				
				// This adds all visible geometry for rendering
				AddGeometry();
				
				// Done rendering geometry
				renderer.FinishGeometry();
				
				// Render crosshair
				renderer.RenderCrosshair();
				
				// Present!
				renderer.Finish();
			}
		}
		
		// After undo
		public override void OnUndoEnd()
		{
			base.OnUndoEnd();
			PickTarget();
		}

		// After redo
		public override void OnRedoEnd()
		{
			base.OnRedoEnd();
			PickTarget();
		}

		#endregion

		#region ================== Actions

		[BeginAction("visualselect", BaseAction = true)]
		public void BeginSelect()
		{
			if(target.geometry != null) (target.geometry as BaseVisualGeometry).OnSelectBegin();
		}

		[EndAction("visualselect", BaseAction = true)]
		public void EndSelect()
		{
			if(target.geometry != null) (target.geometry as BaseVisualGeometry).OnSelectEnd();
		}

		[BeginAction("visualedit", BaseAction = true)]
		public void BeginEdit()
		{
			if(target.geometry != null) (target.geometry as BaseVisualGeometry).OnEditBegin();
		}

		[EndAction("visualedit", BaseAction = true)]
		public void EndEdit()
		{
			if(target.geometry != null) (target.geometry as BaseVisualGeometry).OnEditEnd();
		}

		[BeginAction("raisesector8")]
		public void RaiseSector8()
		{
			ChangeTargetHeight(8);
		}

		[BeginAction("lowersector8")]
		public void LowerSector8()
		{
			ChangeTargetHeight(-8);
		}

		[BeginAction("raisesector1")]
		public void RaiseSector1()
		{
			ChangeTargetHeight(1);
		}

		[BeginAction("lowersector1")]
		public void LowerSector1()
		{
			ChangeTargetHeight(-1);
		}
		
		#endregion
	}
}
