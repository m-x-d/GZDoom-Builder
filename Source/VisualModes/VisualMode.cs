
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
		private const double MOVE_SPEED_MULTIPLIER = 0.001d;
		
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
		
		// Options
		private bool processgeometry;
		private bool processthings;
		
		// Input
		private bool keyforward;
		private bool keybackward;
		private bool keyleft;
		private bool keyright;
		private bool doublespeed;

		// Map
		protected VisualBlockMap blockmap;
		protected Dictionary<Thing, VisualThing> allthings;
		protected Dictionary<Sector, VisualSector> allsectors;
		protected List<VisualBlockEntry> visibleblocks;
		protected List<VisualThing> visiblethings;
		protected Dictionary<Sector, VisualSector> visiblesectors;
		protected List<VisualGeometry> visiblegeometry;
		
		#endregion

		#region ================== Properties

		public Vector3D CameraPosition { get { return campos; } set { campos = value; } }
		public Vector3D CameraTarget { get { return camtarget; } }
		public Sector CameraSector { get { return camsector; } }
		public bool ProcessGeometry { get { return processgeometry; } set { processgeometry = value; } }
		public bool ProcessThings { get { return processthings; } set { processthings = value; } }
		
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
			this.allthings = new Dictionary<Thing,VisualThing>(General.Map.Map.Things.Count);
			this.visibleblocks = new List<VisualBlockEntry>();
			this.visiblesectors = new Dictionary<Sector, VisualSector>(50);
			this.visiblegeometry = new List<VisualGeometry>(200);
			this.visiblethings = new List<VisualThing>(100);
			this.processgeometry = true;
			this.processthings = true;
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
				visiblethings = null;
				allsectors = null;
				allthings = null;
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
			
			// Dispose
			foreach(KeyValuePair<Sector, VisualSector> vs in allsectors)
				vs.Value.Dispose();

			// Dispose
			foreach(KeyValuePair<Thing, VisualThing> vt in allthings)
				vt.Value.Dispose();	
			
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

		public override bool OnUndoBegin()
		{
			renderer.SetCrosshairBusy(true);
			General.Interface.RedrawDisplay();
			return base.OnUndoBegin();
		}

		public override void OnUndoEnd()
		{
			base.OnUndoEnd();
			ResourcesReloaded();
			renderer.SetCrosshairBusy(false);
		}

		public override bool OnRedoBegin()
		{
			renderer.SetCrosshairBusy(true);
			General.Interface.RedrawDisplay();
			return base.OnRedoBegin();
		}

		public override void OnRedoEnd()
		{
			base.OnRedoEnd();
			ResourcesReloaded();
			renderer.SetCrosshairBusy(false);
		}

		public override void OnReloadResources()
		{
			base.OnReloadResources();
			ResourcesReloaded();
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

		[BeginAction("movedoublespeed", BaseAction = true)]
		public virtual void BeginDoubleSpeed()
		{
			doublespeed = true;
		}

		[EndAction("movedoublespeed", BaseAction = true)]
		public virtual void EndDoubleSpeed()
		{
			doublespeed = false;
		}
		
		#endregion

		#region ================== Visibility Culling
		
		// This preforms visibility culling
		private void DoCulling()
		{
			Dictionary<Linedef, Linedef> visiblelines = new Dictionary<Linedef, Linedef>(200);
			Vector2D campos2d = (Vector2D)campos;
			float viewdist = General.Settings.ViewDistance;
			
			// Make collections
			visiblesectors = new Dictionary<Sector, VisualSector>(visiblesectors.Count);
			visiblegeometry = new List<VisualGeometry>(visiblegeometry.Capacity);
			visiblethings = new List<VisualThing>(visiblethings.Capacity);

			// Get the blocks within view range
			visibleblocks = blockmap.GetFrustumRange(renderer.Frustum2D);
			
			// Fill collections with geometry and things
			foreach(VisualBlockEntry block in visibleblocks)
			{
				if(processgeometry)
				{
					// Lines
					foreach(Linedef ld in block.Lines)
					{
						// Line not already processed?
						if(!visiblelines.ContainsKey(ld))
						{
							// Add line if not added yet
							visiblelines.Add(ld, ld);

							// Which side of the line is the camera on?
							if(ld.SideOfLine(campos2d) < 0)
							{
								// Do front of line
								if(ld.Front != null) ProcessSidedefCulling(ld.Front);
							}
							else
							{
								// Do back of line
								if(ld.Back != null) ProcessSidedefCulling(ld.Back);
							}
						}
					}
				}

				if(processthings)
				{
					// Things
					foreach(Thing t in block.Things)
					{
						VisualThing vt;

						if(allthings.ContainsKey(t))
						{
							vt = allthings[t];
						}
						else
						{
							// Create new visual thing
							vt = CreateVisualThing(t);
							if(vt != null) allthings.Add(t, vt);
						}

						if(vt != null)
						{
							visiblethings.Add(vt);
						}
					}
				}
			}

			if(processgeometry)
			{
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
									ProcessSidedefCulling(sd);
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
		}

		// This finds and adds visible sectors
		private void ProcessSidedefCulling(Sidedef sd)
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
				if(vs != null) allsectors.Add(sd.Sector, vs);
			}
			
			if(vs != null)
			{
				// Add to visible sectors if not added yet
				if(!visiblesectors.ContainsKey(sd.Sector))
				{
					visiblesectors.Add(sd.Sector, vs);
					visiblegeometry.AddRange(vs.FixedGeometry);
				}
				
				// Add sidedef geometry
				visiblegeometry.AddRange(vs.GetSidedefGeometry(sd));
			}
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

		#region ================== Object Picking

		// This picks an object from the scene
		public VisualPickResult PickObject(Vector3D from, Vector3D to)
		{
			VisualPickResult result = new VisualPickResult();
			Line2D ray2d = new Line2D(from, to);
			Vector3D delta = to - from;
			
			// Setup no result
			result.picked = null;
			result.hitpos = new Vector3D();
			result.u_ray = 1.0f;
			
			// Find all blocks we are intersecting
			List<VisualBlockEntry> blocks = blockmap.GetLineBlocks(from, to);
			
			// Make collections
			Dictionary<Linedef, Linedef> lines = new Dictionary<Linedef, Linedef>(blocks.Count * 10);
			Dictionary<Sector, VisualSector> sectors = new Dictionary<Sector, VisualSector>(blocks.Count * 10);
			List<IVisualPickable> pickables = new List<IVisualPickable>(blocks.Count * 10);
			
			// Add geometry from the camera sector
			if((camsector != null) && allsectors.ContainsKey(camsector))
			{
				VisualSector vs = allsectors[camsector];
				sectors.Add(camsector, vs);
				foreach(VisualGeometry g in vs.FixedGeometry) pickables.Add(g);
			}
			
			// Go for all lines to see which ones we intersect
			// We will collect geometry from the sectors and sidedefs
			foreach(VisualBlockEntry b in blocks)
			{
				foreach(Linedef ld in b.Lines)
				{
					// Make sure we don't test a line twice
					if(!lines.ContainsKey(ld))
					{
						lines.Add(ld, ld);
						
						// Intersecting?
						float u;
						if(ld.Line.GetIntersection(ray2d, out u))
						{
							// Check on which side we are
							float side = ld.SideOfLine(ray2d.v1);
							
							// Calculate intersection point
							Vector3D intersect = from + delta * u;
							
							// We must add the sectors of both sides of the line
							// If we wouldn't, then aiming at a sector that is just within range
							// could result in an incorrect hit (because the far line of the
							// sector may not be included in this loop)
							if(ld.Front != null)
							{
								// Find the visualsector
								if(allsectors.ContainsKey(ld.Front.Sector))
								{
									VisualSector vs = allsectors[ld.Front.Sector];
									
									// Add sector if not already added
									if(!sectors.ContainsKey(ld.Front.Sector))
									{
										sectors.Add(ld.Front.Sector, vs);
										foreach(VisualGeometry g in vs.FixedGeometry) pickables.Add(g);
									}
									
									// Add sidedef if on the front side
									if(side < 0.0f)
									{
										List<VisualGeometry> sidedefgeo = vs.GetSidedefGeometry(ld.Front);
										foreach(VisualGeometry g in sidedefgeo)
										{
											g.SetPickResults(intersect, u);
											pickables.Add(g);
										}
									}
								}
							}
							
							// Add back side also
							if(ld.Back != null)
							{
								// Find the visualsector
								if(allsectors.ContainsKey(ld.Back.Sector))
								{
									VisualSector vs = allsectors[ld.Back.Sector];

									// Add sector if not already added
									if(!sectors.ContainsKey(ld.Back.Sector))
									{
										sectors.Add(ld.Back.Sector, vs);
										foreach(VisualGeometry g in vs.FixedGeometry) pickables.Add(g);
									}

									// Add sidedef if on the front side
									if(side > 0.0f)
									{
										List<VisualGeometry> sidedefgeo = vs.GetSidedefGeometry(ld.Back);
										foreach(VisualGeometry g in sidedefgeo)
										{
											g.SetPickResults(intersect, u);
											pickables.Add(g);
										}
									}
								}
							}
						}
					}
				}
			}
			
			// Add all the visible things
			foreach(VisualThing vt in visiblethings)
				pickables.Add(vt);
			
			// Now we have a list of potential geometry that lies along the trace line.
			// We still don't know what geometry actually hits, but we ruled out that which doesn't get even close.
			// This is still too much for accurate intersection testing, so we do a fast reject pass first.
			Vector3D direction = to - from;
			direction = direction.GetNormal();
			List<IVisualPickable> potentialpicks = new List<IVisualPickable>(pickables.Count);
			foreach(IVisualPickable p in pickables)
			{
				if(p.PickFastReject(from, to, direction)) potentialpicks.Add(p);
			}
			
			// Now we do an accurate intersection test for all resulting geometry
			// We keep only the closest hit!
			foreach(IVisualPickable p in potentialpicks)
			{
				float u = result.u_ray;
				if(p.PickAccurate(from, to, direction, ref u))
				{
					// Closer than previous find?
					if((u > 0.0f) && (u < result.u_ray))
					{
						result.u_ray = u;
						result.picked = p;
					}
				}
			}
			
			// Setup final result
			result.hitpos = from + to * result.u_ray;
			
			// Done
			return result;
		}
		
		#endregion

		#region ================== Processing
		
		/// <summary>
		/// This disposes all resources and rebuilds the ones needed. 
		/// This usually happens when geometry is changed by undo, redo, cut or paste actions.
		/// </summary>
		protected virtual void ResourcesReloaded()
		{
			// Dispose
			foreach(KeyValuePair<Sector, VisualSector> vs in allsectors)
				vs.Value.Dispose();
				
			foreach(KeyValuePair<Thing, VisualThing> vt in allthings)
				vt.Value.Dispose();
				
			// Clear collections
			allsectors.Clear();
			allthings.Clear();
			visiblesectors.Clear();
			visibleblocks.Clear();
			visiblegeometry.Clear();
			visiblethings.Clear();
			
			// Make new blockmap
			if(blockmap != null)
			{
				blockmap.Dispose();
				blockmap = new VisualBlockMap();
				FillBlockMap();
			}

			// Visibility culling (this re-creates the needed resources)
			DoCulling();
		}
		
		/// <summary>
		/// Implement this to create an instance of your VisualSector implementation.
		/// </summary>
		protected abstract VisualSector CreateVisualSector(Sector s);

		/// <summary>
		/// Implement this to create an instance of your VisualThing implementation.
		/// </summary>
		protected abstract VisualThing CreateVisualThing(Thing t);
		
		/// <summary>
		/// This returns the VisualSector for the given Sector.
		/// </summary>
		protected VisualSector GetVisualSector(Sector s) { return allsectors[s]; }
		
		/// <summary>
		/// This returns the VisualThing for the given Thing.
		/// </summary>
		protected VisualThing GetVisualThing(Thing t) { return allthings[t]; }

		/// <summary>
		/// Returns True when a VisualSector has been created for the specified Sector.
		/// </summary>
		protected bool VisualSectorExists(Sector s) { return allsectors.ContainsKey(s); }

		/// <summary>
		/// Returns True when a VisualThing has been created for the specified Thing.
		/// </summary>
		protected bool VisualThingExists(Thing t) { return allthings.ContainsKey(t); }

		/// <summary>
		/// This is called when the blockmap needs to be refilled, because it was invalidated.
		/// This usually happens when geometry is changed by undo, redo, cut or paste actions.
		/// Lines and Things are added to the block map by the base implementation.
		/// </summary>
		protected virtual void FillBlockMap()
		{
			blockmap.AddLinedefsSet(General.Map.Map.Linedefs);
			blockmap.AddThingsSet(General.Map.Map.Things);
		}
		
		/// <summary>
		/// While this mode is active, this is called continuously to process whatever needs processing.
		/// </summary>
		public override void OnProcess(double deltatime)
		{
			double multiplier;
			
			base.OnProcess(deltatime);
			
			// Calculate camera direction vectors
			Vector3D camvec = Vector3D.FromAngleXYZ(camanglexy, camanglez);
			Vector3D camvecstrafe = Vector3D.FromAngleXY(camanglexy + Angle2D.PIHALF);
			
			// Move the camera
			if(doublespeed) multiplier = MOVE_SPEED_MULTIPLIER * 2.0f; else multiplier = MOVE_SPEED_MULTIPLIER;
			if(keyforward) campos += camvec * (float)((double)General.Settings.MoveSpeed * multiplier * deltatime);
			if(keybackward) campos -= camvec * (float)((double)General.Settings.MoveSpeed * multiplier * deltatime);
			if(keyleft) campos -= camvecstrafe * (float)((double)General.Settings.MoveSpeed * multiplier * deltatime);
			if(keyright) campos += camvecstrafe * (float)((double)General.Settings.MoveSpeed * multiplier * deltatime);
			
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

		#region ================== Actions

		#endregion
	}
}
