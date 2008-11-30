
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
using CodeImp.DoomBuilder.Actions;
using CodeImp.DoomBuilder.Geometry;
using System.Drawing;
using SlimDX;
using CodeImp.DoomBuilder.Editing;

#endregion

namespace CodeImp.DoomBuilder.VisualModes
{
	/// <summary>
	/// Provides specialized functionality for a visual (3D) Doom Builder editing mode.
	/// </summary>
	public abstract class VisualMode : EditMode
	{
		#region ================== Constants

		private const float ANGLE_FROM_MOUSE = 0.0001f;
		public const float MAX_ANGLEZ_LOW = 100f / Angle2D.PIDEG;
		public const float MAX_ANGLEZ_HIGH = (360f - 100f) / Angle2D.PIDEG;
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
		private Sector camsector;
		
		// Input
		private bool keyforward;
		private bool keybackward;
		private bool keyleft;
		private bool keyright;

		// Map
		protected VisualBlockMap blockmap;
		protected Dictionary<Sector, VisualSector> allsectors;
		protected List<VisualBlockEntry> visibleblocks;
		protected Dictionary<Sector, VisualSector> visiblesectors;
		protected List<VisualGeometry> visiblegeometry;
		
		#endregion

		#region ================== Properties

		public Vector3D CameraPosition { get { return campos; } set { campos = value; } }
		public Vector3D CameraTarget { get { return camtarget; } }
		public Sector CameraSector { get { return camsector; } }
		
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
			this.blockmap = new VisualBlockMap();
			this.allsectors = new Dictionary<Sector, VisualSector>(General.Map.Map.Sectors.Count);
			this.visibleblocks = new List<VisualBlockEntry>();
			this.visiblesectors = new Dictionary<Sector, VisualSector>(50);
			this.visiblegeometry = new List<VisualGeometry>(200);
		}

		// Disposer
		public override void Dispose()
		{
			// Not already disposed?
			if(!isdisposed)
			{
				// Clean up
				foreach(KeyValuePair<Sector, VisualSector> s in allsectors) s.Value.Dispose();
				blockmap.Dispose();
				visiblesectors = null;
				visiblegeometry = null;
				visibleblocks = null;
				allsectors = null;
				blockmap = null;
				
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

			// Update the used textures
			General.Map.Data.UpdateUsedTextures();

			// Fill the blockmap
			FillBlockMap();
			
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

		#region ================== Events

		public override void OnUndoEnd()
		{
			base.OnUndoEnd();

			// Make new blockmap
			if(blockmap != null)
			{
				blockmap.Dispose();
				blockmap = new VisualBlockMap();
				FillBlockMap();
			}
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

		#region ================== Visibility Culling

		// This preforms visibility culling
		private void DoCulling()
		{
			Dictionary<Linedef, Linedef> visiblelines = new Dictionary<Linedef, Linedef>(200);
			Vector2D campos2d = (Vector2D)campos;
			float viewdist = General.Settings.ViewDistance;
			
			// Get the blocks within view range
			visibleblocks = blockmap.GetFrustumRange(renderer.Frustum2D);
			
			// Fill visiblity collections
			visiblesectors = new Dictionary<Sector, VisualSector>(visiblesectors.Count);
			visiblegeometry = new List<VisualGeometry>(visiblegeometry.Capacity);
			foreach(VisualBlockEntry block in visibleblocks)
			{
				foreach(Linedef ld in block.Lines)
				{
					// Add line if not added yet
					visiblelines[ld] = ld;
					
					// Which side of the line is the camera on?
					if(ld.SideOfLine(campos2d) < 0)
					{
						// Do front of line
						if(ld.Front != null) ProcessSidedef(ld.Front);
					}
					else
					{
						// Do back of line
						if(ld.Back != null) ProcessSidedef(ld.Back);
					}
				}
			}

			// Find camera sector
			Linedef nld = MapSet.NearestLinedef(visiblelines.Values, campos2d);
			if(nld != null)
			{
				camsector = GetCameraSectorFromLinedef(nld);
			}
			else
			{
				// Exceptional case: no lines found in any nearby blocks!
				// This could happen in the middle of an extremely large sector and in this case
				// the above code will not have found any sectors/sidedefs for rendering.
				// Here we handle this special case with brute-force. Let's find the sector
				// the camera is in by searching the entire map and render that sector only.
				nld = General.Map.Map.NearestLinedef(campos2d);
				if(nld != null)
				{
					camsector = GetCameraSectorFromLinedef(nld);
					if(camsector != null)
					{
						foreach(Sidedef sd in camsector.Sidedefs)
						{
							float side = sd.Line.SideOfLine(campos2d);
							if(((side < 0) && sd.IsFront) ||
							   ((side > 0) && !sd.IsFront))
								ProcessSidedef(sd);
						}
					}
					else
					{
						// Too far away from the map to see anything
						camsector = null;
					}
				}
				else
				{
					// Map is empty
					camsector = null;
				}
			}
		}

		// This finds and adds visible sectors
		private void ProcessSidedef(Sidedef sd)
		{
			VisualSector vs;

			// Find the visualsector and make it if needed
			if(allsectors.ContainsKey(sd.Sector))
			{
				// Take existing visualsector
				vs = allsectors[sd.Sector];
			}
			else
			{
				// Make new visualsector
				vs = CreateVisualSector(sd.Sector);
				allsectors.Add(sd.Sector, vs);
			}

			// Add to visible sectors if not added yet
			if(!visiblesectors.ContainsKey(sd.Sector))
			{
				visiblesectors.Add(sd.Sector, vs);
				visiblegeometry.AddRange(vs.FixedGeometry);
			}
			
			// Add sidedef geometry
			visiblegeometry.AddRange(vs.GetSidedefGeometry(sd));
		}

		// This returns the camera sector from linedef
		private Sector GetCameraSectorFromLinedef(Linedef ld)
		{
			if(ld.SideOfLine(campos) < 0)
			{
				if(ld.Front != null)
					return ld.Front.Sector;
				else
					return null;
			}
			else
			{
				if(ld.Back != null)
					return ld.Back.Sector;
				else
					return null;
			}
		}
		
		#endregion

		#region ================== Processing

		// This creates a visual sector
		protected abstract VisualSector CreateVisualSector(Sector s);
		
		// This fills the blockmap
		protected virtual void FillBlockMap()
		{
			blockmap.AddLinedefsSet(General.Map.Map.Linedefs);
		}
		
		// Processing
		public override void OnProcess(double deltatime)
		{
			Vector3D camvec;
			Vector3D camvecstrafe;
			
			base.OnProcess(deltatime);
			
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

			// Visibility culling
			DoCulling();
			
			// Now redraw
			General.Interface.RedrawDisplay();
		}
		
		#endregion
		
		#region ================== Rendering

		// Call this to simply render all visible sectors
		public override void OnRedrawDisplay()
		{
			// Render all visible sectors
			foreach(VisualGeometry g in visiblegeometry)
				renderer.RenderGeometry(g);
		}
		
		#endregion

		#region ================== Actions
		
		[EndAction("reloadresources", BaseAction = true)]
		public virtual void ReloadResources()
		{
			// Trash all visual sectors, because they are no longer valid
			foreach(KeyValuePair<Sector, VisualSector> s in allsectors) s.Value.Dispose();
			allsectors.Clear();
			visiblesectors.Clear();
			visiblegeometry.Clear();
		}

		#endregion
	}
}
