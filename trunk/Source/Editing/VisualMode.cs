
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
using CodeImp.DoomBuilder.Actions;
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
		private const float MAX_ANGLEZ_LOW = 100f / Angle2D.PIDEG;
		private const float MAX_ANGLEZ_HIGH = (360f - 100f) / Angle2D.PIDEG;
		private const float CAMERA_SPEED = 6f;
		
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

		// Input
		private bool keyforward;
		private bool keybackward;
		private bool keyleft;
		private bool keyright;

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
			this.campos = new Vector3D(0.0f, 0.0f, 96.0f);
			this.camanglez = Angle2D.PI;
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

		#region ================== Start / Stop

		// Mode is engaged
		public override void OnEngage()
		{
			base.OnEngage();
			
			// Find a 3D Mode thing
			foreach(Thing t in General.Map.Map.Things)
				if(t.Type == General.Map.Config.Start3DModeThingType) modething = t;
			
			// Found one?
			if(modething != null)
			{
				// Position camera here
				modething.DetermineSector();
				campos = modething.Position + new Vector3D(0.0f, 0.0f, 96.0f);
				camanglexy = modething.Angle + Angle2D.PI;
				camanglez = Angle2D.PI;
			}

			// Start special input mode
			General.Interface.SetProcessorState(true);
			General.Interface.StartExclusiveMouseInput();
		}

		// Mode is disengaged
		public override void OnDisengage()
		{
			base.OnDisengage();

			// Do we have a 3D Mode thing?
			if(modething != null)
			{
				// Position the thing to match camera
				modething.Move((int)campos.x, (int)campos.y, 0);
				modething.Rotate(camanglexy - Angle2D.PI);
			}
			
			// Stop special input mode
			General.Interface.SetProcessorState(false);
			General.Interface.StopExclusiveMouseInput();
		}

		#endregion

		#region ================== Input

		// Mouse input
		public override void OnMouseInput(Vector2D delta)
		{
			base.OnMouseInput(delta);

			// Change camera angles with the mouse changes
			camanglexy -= delta.x * ANGLE_FROM_MOUSE;
			camanglez += delta.y * ANGLE_FROM_MOUSE;
			
			// Normalize angles
			camanglexy = Angle2D.Normalized(camanglexy);
			camanglez = Angle2D.Normalized(camanglez);

			// Limit vertical angle
			if(camanglez < MAX_ANGLEZ_LOW) camanglez = MAX_ANGLEZ_LOW;
			if(camanglez > MAX_ANGLEZ_HIGH) camanglez = MAX_ANGLEZ_HIGH;
			
			General.MainWindow.UpdateCoordinates(new Vector2D(camanglexy, camanglez));
		}

		[BeginAction("moveforward", BaseAction = true)]
		public virtual void BeginMoveForward()
		{
			keyforward = true;
		}

		[EndAction("moveforward", BaseAction = true)]
		public virtual void EndMoveForward()
		{
			keyforward = false;
		}

		[BeginAction("movebackward", BaseAction = true)]
		public virtual void BeginMoveBackward()
		{
			keybackward = true;
		}

		[EndAction("movebackward", BaseAction = true)]
		public virtual void EndMoveBackward()
		{
			keybackward = false;
		}

		[BeginAction("moveleft", BaseAction = true)]
		public virtual void BeginMoveLeft()
		{
			keyleft = true;
		}

		[EndAction("moveleft", BaseAction = true)]
		public virtual void EndMoveLeft()
		{
			keyleft = false;
		}

		[BeginAction("moveright", BaseAction = true)]
		public virtual void BeginMoveRight()
		{
			keyright = true;
		}

		[EndAction("moveright", BaseAction = true)]
		public virtual void EndMoveRight()
		{
			keyright = false;
		}
		
		#endregion

		#region ================== Processing

		// Processing
		public override void OnProcess()
		{
			Vector3D camvec;
			Vector3D camvecstrafe;
			
			base.OnProcess();
			
			// Calculate camera direction vectors
			camvec = Vector3D.FromAngleXYZ(camanglexy, camanglez);
			camvecstrafe = Vector3D.FromAngleXY(camanglexy + Angle2D.PIHALF);
			
			// Move the camera
			if(keyforward) campos += camvec * CAMERA_SPEED;
			if(keybackward) campos -= camvec * CAMERA_SPEED;
			if(keyleft) campos -= camvecstrafe * CAMERA_SPEED;
			if(keyright) campos += camvecstrafe * CAMERA_SPEED;
			
			// Target the camera
			camtarget = campos + camvec;
			
			// Apply new camera matrices
			renderer.PositionAndLookAt(campos, camtarget);
			
			// Now redraw
			General.Interface.RedrawDisplay();
		}
		
		#endregion
	}
}
