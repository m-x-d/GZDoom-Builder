
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
				PickTarget();
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
		
		#endregion
	}
}
