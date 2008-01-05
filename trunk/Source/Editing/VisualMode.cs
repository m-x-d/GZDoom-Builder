
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
using CodeImp.DoomBuilder.Interface;
using CodeImp.DoomBuilder.IO;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Rendering;
using CodeImp.DoomBuilder.Controls;
using CodeImp.DoomBuilder.Geometry;
using System.Drawing;
using SlimDX;

#endregion

namespace CodeImp.DoomBuilder.Editing
{
	/// <summary>
	/// Provides specialized functionality for a visual (3D) Doom Builder editing mode.
	/// </summary>
	public abstract class VisualMode : EditMode
	{
		#region ================== Constants

		private const float ANGLE_FROM_MOUSE = 0.0001f;
		private const float MAX_ANGLEZ_UP = 80f / Angle2D.PIDEG;
		private const float MAX_ANGLEZ_DOWN = (360f - 80f) / Angle2D.PIDEG;
		
		#endregion

		#region ================== Variables

		// 3D Mode thing
		protected Thing modething;
		
		// Graphics
		protected IRenderer3D renderer;
		private Renderer3D renderer3d;
		
		// Camera
		private Vector3D campos;
		private Vector3D camtarget;
		private float camanglexy, camanglez;

		#endregion

		#region ================== Properties

		public Vector3D CameraPosition { get { return campos; } set { campos = value; } }
		public Vector3D CameraTarget { get { return camtarget; } }
		
		#endregion

		#region ================== Constructor / Disposer

		/// <summary>
		/// Provides specialized functionality for a visual (3D) Doom Builder editing mode.
		/// </summary>
		public VisualMode()
		{
			// Initialize
			this.renderer = General.Map.Renderer3D;
			this.renderer3d = (Renderer3D)General.Map.Renderer3D;
		}

		// Diposer
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

		// Mode is engaged
		public override void Engage()
		{
			base.Engage();
			
			// Find a 3D Mode thing
			foreach(Thing t in General.Map.Map.Things)
				if(t.Type == General.Map.Config.Start3DModeThingType) modething = t;
			
			// Found one?
			if(modething != null)
			{
				// Position camera here
				modething.DetermineSector();
				campos = modething.Position + new Vector3D(0, 0, 20);
				camanglexy = modething.Angle;
				camanglez = 0f;
			}

			// Start special input mode
			General.Interface.SetProcessorState(true);
			General.Interface.StartExclusiveMouseInput();
		}

		// Mode is disengaged
		public override void Disengage()
		{
			base.Disengage();

			// Do we have a 3D Mode thing?
			if(modething != null)
			{
				// Position the thing to match camera
				modething.Move((int)campos.x, (int)campos.y, 0);
				modething.Rotate(camanglexy);
			}
			
			// Stop special input mode
			General.Interface.SetProcessorState(false);
			General.Interface.StopExclusiveMouseInput();
		}

		// Mouse input
		public override void MouseInput(Vector2D delta)
		{
			base.MouseInput(delta);

			// Change camera angles with the mouse changes
			camanglexy += delta.x * ANGLE_FROM_MOUSE;
			camanglez -= delta.y * ANGLE_FROM_MOUSE;

			// Limit vertical angle
			if(camanglez < MAX_ANGLEZ_UP) camanglez = MAX_ANGLEZ_UP;
			if(camanglez > MAX_ANGLEZ_DOWN) camanglez = MAX_ANGLEZ_DOWN;
			
			// Normalize horizontal angle
			camanglexy = Angle2D.Normalized(camanglexy);
			
			General.MainWindow.UpdateCoordinates(new Vector2D(camanglexy, camanglez));
		}
		
		// Processing
		public override void Process()
		{
			base.Process();
			
			// Target the camera
			camtarget = campos + Vector3D.FromAngleXYZ(camanglexy, camanglez);
			
			// Apply new camera matrices
			renderer.PositionAndLookAt(campos, camtarget);
			
			// Now redraw
			General.Interface.RedrawDisplay();
		}
		
		#endregion
	}
}
