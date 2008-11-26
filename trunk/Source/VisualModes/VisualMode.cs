
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

		// Map
		protected VisualBlockMap blockmap;
		protected Dictionary<Sector, VisualSector> allsectors;
		protected Dictionary<Sector, VisualSector> visiblesectors;
		
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
			this.blockmap = new VisualBlockMap();
			this.allsectors = new Dictionary<Sector, VisualSector>(General.Map.Map.Sectors.Count);
			this.visiblesectors = new Dictionary<Sector, VisualSector>(50);
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
			Vector2D campos2d = (Vector2D)campos;
			float viewdist = General.Settings.ViewDistance;
			
			// Get the blocks within view range and make a collection of all nearby linedefs
			RectangleF viewrect = new RectangleF(campos.x - viewdist, campos.y - viewdist, viewdist * 2, viewdist * 2);
			List<VisualBlockEntry> blocks = blockmap.GetSquareRange(viewrect);
			List<Linedef> nearbylines = new List<Linedef>(blocks.Count);
			foreach(VisualBlockEntry b in blocks) nearbylines.AddRange(b.Lines);
			
			// Find the sector to begin with
			Sector start = FindStartSector((Vector2D)campos, nearbylines);

			// Find visible sectors
			visiblesectors = new Dictionary<Sector, VisualSector>(visiblesectors.Count);
			if(start != null) ProcessVisibleSectors(start, (Vector2D)campos);
		}

		// This finds and adds visible sectors
		private void ProcessVisibleSectors(Sector start, Vector2D campos)
		{
			Stack<Sector> todo = new Stack<Sector>(50);
			Dictionary<Sector, Sector> stackedsectors = new Dictionary<Sector, Sector>(50);
			Clipper clipper = new Clipper(campos);
			float viewdist2 = General.Settings.ViewDistance * General.Settings.ViewDistance;
			
			// TODO: Use sector markings instead of the stackedsectors dictionary?
			
			// This algorithm uses a breadth-first search for visible sectors
			
			// Continue until no more sectors to process
			todo.Push(start);
			stackedsectors.Add(start, start);
			while(todo.Count > 0)
			{
				Sector s = todo.Pop();
				VisualSector vs;

				// Find the basesector and make it if needed
				if(allsectors.ContainsKey(s))
				{
					// Take existing visualsector
					vs = allsectors[s];
				}
				else
				{
					// Make new visualsector
					vs = CreateVisualSector(s);
					allsectors.Add(s, vs);
				}

				// Add sector to visibility list
				visiblesectors.Add(s, vs);

				// Go for all sidedefs in the sector
				foreach(Sidedef sd in s.Sidedefs)
				{
					// Camera on the front of this side?
					float side = sd.Line.SideOfLine(campos);
					if(((side > 0) && sd.IsFront) ||
					   ((side < 0) && !sd.IsFront))
					{
						// Sidedef blocking the view?
						if((sd.Other == null) ||
						   (sd.Other.Sector.FloorHeight >= (sd.Sector.CeilHeight - 0.0001f)) ||
						   (sd.Other.Sector.CeilHeight <= (sd.Sector.FloorHeight + 0.0001f)) ||
						   (sd.Other.Sector.FloorHeight >= (sd.Other.Sector.CeilHeight - 0.0001f)))
						{
							// This blocks the view
							clipper.InsertRange(sd.Line.Start.Position, sd.Line.End.Position);
						}
					}
				}

				// Go for all sidedefs in the sector
				foreach(Sidedef sd in s.Sidedefs)
				{
					// Doublesided and not referring to same sector?
					if((sd.Other != null) && (sd.Other.Sector != sd.Sector))
					{
						// Get the other sector
						Sector os = sd.Other.Sector;

						// Sector not added yet?
						if(!stackedsectors.ContainsKey(os))
						{
							// Within view range?
							if(sd.Line.DistanceToSq(campos, true) < viewdist2)
							{
								Vector2D p = sd.Line.Start.Position;
								if((p.x > s.BBox.Left + Linedef.SIDE_POINT_DISTANCE) &&
								   (p.x < s.BBox.Right - Linedef.SIDE_POINT_DISTANCE) &&
								   (p.y > s.BBox.Top + Linedef.SIDE_POINT_DISTANCE) &&
								   (p.y < s.BBox.Bottom - Linedef.SIDE_POINT_DISTANCE))
								{
									// Sidedef is inside source sector, other sector always visible!
									todo.Push(os);
									stackedsectors.Add(os, os);
								}
								// Can we see this sector?
								else if(clipper.TestRange(sd.Line.Start.Position, sd.Line.End.Position))
								{
									// Process this sector as well
									todo.Push(os);
									stackedsectors.Add(os, os);
								}
							}
						}
					}
				}
			}

			// Done
			clipper.Dispose();
		}
		
		// This finds the nearest sector to the camera
		private Sector FindStartSector(Vector2D campos, List<Linedef> lines)
		{
			float side;
			Linedef l;

			// Get nearest linedef
			l = MapSet.NearestLinedef(lines, campos);
			if(l != null)
			{
				// Check if we are on front or back side
				side = l.SideOfLine(campos);
				if(side > 0)
				{
					// Is there a sidedef here?
					if(l.Back != null)
						return l.Back.Sector;
					else if(l.Front != null)
						return l.Front.Sector;
					else
						return null;
				}
				else
				{
					// Is there a sidedef here?
					if(l.Front != null)
						return l.Front.Sector;
					else if(l.Back != null)
						return l.Back.Sector;
					else
						return null;
				}
			}
			else
				return null;
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
			foreach(KeyValuePair<Sector, VisualSector> vs in visiblesectors)
				renderer.RenderGeometry(vs.Value);
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
		}

		#endregion
	}
}
