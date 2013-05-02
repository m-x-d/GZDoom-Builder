
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
using System.Windows.Forms;
using CodeImp.DoomBuilder.Windows;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Rendering;
using CodeImp.DoomBuilder.Geometry;
using CodeImp.DoomBuilder.Editing;
using CodeImp.DoomBuilder.Actions;
using CodeImp.DoomBuilder.VisualModes;
using CodeImp.DoomBuilder.Config;
using CodeImp.DoomBuilder.GZBuilder.Data;
using CodeImp.DoomBuilder.Types;
using CodeImp.DoomBuilder.Data;

#endregion

namespace CodeImp.DoomBuilder.BuilderModes
{
	[EditMode(DisplayName = "GZDB Visual Mode",
			  SwitchAction = "gzdbvisualmode", // Action name used to switch to this mode
			  ButtonImage = "VisualModeGZ.png",	// Image resource name for the button
			  ButtonOrder = 1,					// Position of the button (lower is more to the left)
			  ButtonGroup = "001_visual",
              UseByDefault = true)]

	public class BaseVisualMode : VisualMode
	{
		#region ================== Constants
		// Object picking
		private const float PICK_INTERVAL = 80.0f;
		private const float PICK_RANGE = 0.98f;

		// Gravity
		private const float GRAVITY = -0.06f;
		
		#endregion
		
		#region ================== Variables

		// Gravity
		private Vector3D gravity;
		private float cameraflooroffset = 41f;		// same as in doom
		private float cameraceilingoffset = 10f;
		
		// Object picking
		private VisualPickResult target;
		private float lastpicktime;
		private bool locktarget;

		private bool useSelectionFromClassicMode;//mxd
		public bool UseSelectionFromClassicMode { get { return useSelectionFromClassicMode; } }

		// This keeps extra element info
		private Dictionary<Sector, SectorData> sectordata;
		private Dictionary<Thing, ThingData> thingdata;
		private Dictionary<Vertex, VertexData> vertexdata; //mxd
		
		// This is true when a selection was made because the action is performed
		// on an object that was not selected. In this case the previous selection
		// is cleared and the targeted object is temporarely selected to perform
		// the action on. After the action is completed, the object is deselected.
		private bool singleselection;
		
		// We keep these to determine if we need to make a new undo level
		private bool selectionchanged;
		private int lastundogroup;
		private VisualActionResult actionresult;
		private bool undocreated;

		// List of selected objects when an action is performed
		private List<IVisualEventReceiver> selectedobjects;
        //mxd. Used in Cut/PasteSelection actions
        private List<ThingCopyData> copyBuffer;

		public static bool GZDoomRenderingEffects { get { return gzdoomRenderingEffects; } } //mxd
        private static bool gzdoomRenderingEffects = true; //mxd

		//mxd. Moved here from Tools
		private struct SidedefAlignJob
		{
			public Sidedef sidedef;
			public Sidedef controlSide; //mxd

			public float offsetx;
			public float scaleY; //mxd

			// When this is true, the previous sidedef was on the left of
			// this one and the texture X offset of this sidedef can be set
			// directly. When this is false, the length of this sidedef
			// must be subtracted from the X offset first.
			public bool forward;
		}
		
		#endregion
		
		#region ================== Properties

		public override object HighlightedObject
		{
			get
			{
				// Geometry picked?
				if(target.picked is VisualGeometry)
				{
					VisualGeometry pickedgeo = (target.picked as VisualGeometry);

					if(pickedgeo.Sidedef != null)
						return pickedgeo.Sidedef;
					else if(pickedgeo.Sector != null)
						return pickedgeo.Sector;
					else
						return null;
				}
				// Thing picked?
				else if(target.picked is VisualThing)
				{
					VisualThing pickedthing = (target.picked as VisualThing);
					return pickedthing.Thing;
				}
				else
				{
					return null;
				}
			}
		}

		new public IRenderer3D Renderer { get { return renderer; } }
		
		public bool IsSingleSelection { get { return singleselection; } }
		public bool SelectionChanged { get { return selectionchanged; } set { selectionchanged |= value; } }

		#endregion
		
		#region ================== Constructor / Disposer

		// Constructor
		public BaseVisualMode()
		{
			// Initialize
			this.gravity = new Vector3D(0.0f, 0.0f, 0.0f);
			this.selectedobjects = new List<IVisualEventReceiver>();
            //mxd
            this.copyBuffer = new List<ThingCopyData>();
			
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

		// This calculates brightness level
		internal int CalculateBrightness(int level)
		{
			return renderer.CalculateBrightness(level);
		}

        //mxd. This calculates brightness level with doom-style shading
        internal int CalculateBrightness(int level, Sidedef sd) {
            return renderer.CalculateBrightness(level, sd);
        }
		
		// This adds a selected object
		internal void AddSelectedObject(IVisualEventReceiver obj)
		{
			selectedobjects.Add(obj);
			selectionchanged = true;
		}
		
		// This removes a selected object
		internal void RemoveSelectedObject(IVisualEventReceiver obj)
		{
			selectedobjects.Remove(obj);
			selectionchanged = true;
		}
		
		// This is called before an action is performed
		public void PreAction(int multiselectionundogroup)
		{
			actionresult = new VisualActionResult();
			
			PickTargetUnlocked();
			
			// If the action is not performed on a selected object, clear the
			// current selection and make a temporary selection for the target.
			if((target.picked != null) && !target.picked.Selected && (BuilderPlug.Me.VisualModeClearSelection || (selectedobjects.Count == 0)))
			{
				// Single object, no selection
				singleselection = true;
				ClearSelection();
				undocreated = false;
			}
			else
			{
				singleselection = false;
				
				// Check if we should make a new undo level
				// We don't want to do this if this is the same action with the same
				// selection and the action wants to group the undo levels
				if((lastundogroup != multiselectionundogroup) || (lastundogroup == UndoGroup.None) ||
				   (multiselectionundogroup == UndoGroup.None) || selectionchanged)
				{
					// We want to create a new undo level, but not just yet
					lastundogroup = multiselectionundogroup;
					undocreated = false;
				}
				else
				{
					// We don't want to make a new undo level (changes will be combined)
					undocreated = true;
				}
			}
		}

		// Called before an action is performed. This does not make an undo level
		private void PreActionNoChange()
		{
			actionresult = new VisualActionResult();
			singleselection = false;
			undocreated = false;
		}
		
		// This is called after an action is performed
		private void PostAction()
		{
			if(!string.IsNullOrEmpty(actionresult.displaystatus))
				General.Interface.DisplayStatus(StatusType.Action, actionresult.displaystatus);

			// Reset changed flags
			foreach(KeyValuePair<Sector, VisualSector> vs in allsectors)
			{
				BaseVisualSector bvs = (vs.Value as BaseVisualSector);
				foreach(VisualFloor vf in bvs.ExtraFloors) vf.Changed = false;
				foreach(VisualCeiling vc in bvs.ExtraCeilings) vc.Changed = false;
				foreach(VisualFloor vf in bvs.ExtraBackFloors) vf.Changed = false; //mxd
				foreach(VisualCeiling vc in bvs.ExtraBackCeilings) vc.Changed = false; //mxd
				bvs.Floor.Changed = false;
				bvs.Ceiling.Changed = false;
			}
			
			selectionchanged = false;
			
			if(singleselection)
				ClearSelection();
			
			UpdateChangedObjects();
			ShowTargetInfo();
		}
		
		// This sets the result for an action
		public void SetActionResult(VisualActionResult result)
		{
			actionresult = result;
		}

		// This sets the result for an action
		public void SetActionResult(string displaystatus)
		{
			actionresult = new VisualActionResult();
			actionresult.displaystatus = displaystatus;
		}
		
		// This creates an undo, when only a single selection is made
		// When a multi-selection is made, the undo is created by the PreAction function
		public int CreateUndo(string description, int group, int grouptag)
		{
			if(!undocreated)
			{
				undocreated = true;

				if(singleselection)
					return General.Map.UndoRedo.CreateUndo(description, this, group, grouptag);
				else
					return General.Map.UndoRedo.CreateUndo(description, this, UndoGroup.None, 0);
			}
			else
			{
				return 0;
			}
		}

		// This creates an undo, when only a single selection is made
		// When a multi-selection is made, the undo is created by the PreAction function
		public int CreateUndo(string description)
		{
			return CreateUndo(description, UndoGroup.None, 0);
		}

		// This makes a list of the selected object
		private void RebuildSelectedObjectsList()
		{
			// Make list of selected objects
			selectedobjects = new List<IVisualEventReceiver>();
			foreach(KeyValuePair<Sector, VisualSector> vs in allsectors)
			{
				if(vs.Value != null)
				{
					BaseVisualSector bvs = (BaseVisualSector)vs.Value;
					if((bvs.Floor != null) && bvs.Floor.Selected) selectedobjects.Add(bvs.Floor);
					if((bvs.Ceiling != null) && bvs.Ceiling.Selected) selectedobjects.Add(bvs.Ceiling);
					foreach(Sidedef sd in vs.Key.Sidedefs)
					{
						List<VisualGeometry> sidedefgeos = bvs.GetSidedefGeometry(sd);
						foreach(VisualGeometry sdg in sidedefgeos)
						{
							if(sdg.Selected) selectedobjects.Add((sdg as IVisualEventReceiver));
						}
					}
				}
			}

			foreach(KeyValuePair<Thing, VisualThing> vt in allthings)
			{
				if(vt.Value != null)
				{
					BaseVisualThing bvt = (BaseVisualThing)vt.Value;
					if(bvt.Selected) selectedobjects.Add(bvt);
				}
			}

			//mxd
			if(General.Map.UDMF && General.Settings.GZShowVisualVertices) {
				foreach(KeyValuePair<Vertex, VisualVertexPair> pair in vertices) {
					if(pair.Value.Vertex1.Selected)
						selectedobjects.Add((BaseVisualVertex)pair.Value.Vertex1);
					if(pair.Value.Vertex2.Selected)
						selectedobjects.Add((BaseVisualVertex)pair.Value.Vertex2);
				}
			}
		}

		//mxd. Need this to apply changes to 3d-floor even if control sector doesn't exist as BaseVisualSector
		internal BaseVisualSector CreateBaseVisualSector(Sector s) {
			BaseVisualSector vs = new BaseVisualSector(this, s);
			if(vs != null) allsectors.Add(s, vs);
			return vs;
		}

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
			return vt.Setup() ? vt : null;
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
					
					// Sidedef?
					if(pickedgeo is BaseVisualGeometrySidedef)
					{
						BaseVisualGeometrySidedef pickedsidedef = (pickedgeo as BaseVisualGeometrySidedef);
						General.Interface.ShowLinedefInfo(pickedsidedef.GetControlLinedef()); //mxd
					}
					// Sector?
					else if(pickedgeo is BaseVisualGeometrySector)
					{
						BaseVisualGeometrySector pickedsector = (pickedgeo as BaseVisualGeometrySector);
						General.Interface.ShowSectorInfo(pickedsector.Level.sector);
					}
					else
					{
						General.Interface.HideInfo();
					}
				} 
				else if(target.picked is VisualThing) 
				{ // Thing picked?
					VisualThing pickedthing = (target.picked as VisualThing);
					General.Interface.ShowThingInfo(pickedthing.Thing);
				} 
				else if(target.picked is VisualVertex) //mxd
				{
					VisualVertex pickedvert = (target.picked as VisualVertex);
					General.Interface.ShowVertexInfo(pickedvert.Vertex);
				}
			}
			else
			{
				General.Interface.HideInfo();
			}
		}
		
		// This updates the VisualSectors and VisualThings that have their Changed property set
        private void UpdateChangedObjects()
		{
			foreach(KeyValuePair<Sector, VisualSector> vs in allsectors)
			{
				if(vs.Value != null)
				{
					BaseVisualSector bvs = (BaseVisualSector)vs.Value;
					if(bvs.Changed) bvs.Rebuild();
				}
			}

			foreach(KeyValuePair<Thing, VisualThing> vt in allthings)
			{
				if(vt.Value != null)
				{
					BaseVisualThing bvt = (BaseVisualThing)vt.Value;
					if(bvt.Changed) bvt.Rebuild();
				}
			}

			//mxd
			if(General.Map.UDMF) {
				foreach(KeyValuePair<Vertex, VisualVertexPair> pair in vertices)
					pair.Value.Update();
			}
		}

        //mxd
        protected override void moveSelectedThings(Vector2D direction, bool absolutePosition) {
            List<VisualThing> visualThings = GetSelectedVisualThings(true);

            if (visualThings.Count == 0) {
                General.Interface.DisplayStatus(StatusType.Warning, "Select some Things first!");
                return;
            }

            PreAction(UndoGroup.ThingMove);

            Vector3D[] coords = new Vector3D[visualThings.Count];
            for (int i = 0; i < visualThings.Count; i++)
                coords[i] = visualThings[i].Thing.Position;

            //move things...
            Vector3D[] translatedCoords = translateCoordinates(coords, direction, absolutePosition);
            for (int i = 0; i < visualThings.Count; i++) {
                BaseVisualThing t = visualThings[i] as BaseVisualThing;
                t.OnMove(translatedCoords[i]);
            }

            PostAction();
        }

		//mxd
		private void deleteSelectedThings() {
			List<IVisualEventReceiver> objs = GetSelectedObjects(false, false, true, false);
			if(objs.Count == 0) return;

			General.Map.UndoRedo.ClearAllRedos();
			string rest = objs.Count + " thing" + (objs.Count > 1 ? "s." : ".");
			//make undo
			General.Map.UndoRedo.CreateUndo("Delete " + rest);
			General.Interface.DisplayStatus(StatusType.Info, "Deleted " + rest);
			//clear selection
			ClearSelection();

			PreActionNoChange();
			foreach(IVisualEventReceiver i in objs) i.OnDelete(); //are they deleted from BlockMap automatically?..

			// Update cache values
			General.Map.IsChanged = true;
			General.Map.ThingsFilter.Update();

			PostAction();
		}

		//mxd
		private void deleteSelectedVertices() {
			if(!General.Map.UDMF) return;
			List<IVisualEventReceiver> objs = GetSelectedObjects(false, false, false, true);
			if(objs.Count == 0) return;

			General.Map.UndoRedo.ClearAllRedos();
			string description = "Reset height of " + objs.Count + (objs.Count > 1 ? " vertices." : " vertex.");
			//make undo
			General.Map.UndoRedo.CreateUndo(description);
			General.Interface.DisplayStatus(StatusType.Info, description);
			//clear selection
			ClearSelection();

			PreActionNoChange();
			foreach(IVisualEventReceiver i in objs) {
				((BaseVisualVertex)i).Vertex.Fields.BeforeFieldsChange();
				i.OnDelete();
			}

			PostAction();
		}

        //mxd
        private Vector3D[] translateCoordinates(Vector3D[] coordinates, Vector2D direction, bool absolutePosition) {
            if (coordinates.Length == 0) return null;

            direction.x = (float)Math.Round(direction.x);
            direction.y = (float)Math.Round(direction.y);

            Vector3D[] translatedCoords = new Vector3D[coordinates.Length];

            //move things...
            if (!absolutePosition) { //...relatively (that's easy)
                int camAngle = (int)Math.Round(Angle2D.RadToDeg(General.Map.VisualCamera.AngleXY));// * 180 / Math.PI);
                int sector = General.ClampAngle(camAngle - 45) / 90;
                direction = direction.GetRotated(sector * Angle2D.PIHALF);

                for (int i = 0; i < coordinates.Length; i++)
                    translatedCoords[i] = coordinates[i] + new Vector3D(direction);

                return translatedCoords;
            }

            //...to specified location preserving relative positioning (that's harder)
            if (coordinates.Length == 1) {//just move it there
                translatedCoords[0] = new Vector3D(direction.x, direction.y, coordinates[0].z);
                return translatedCoords;
            }

            //we need some reference
            float minX = coordinates[0].x;
            float maxX = minX;
            float minY = coordinates[0].y;
            float maxY = minY;

            //get bounding coordinates for selected things
            for (int i = 1; i < coordinates.Length; i++) {
                if (coordinates[i].x < minX)
                    minX = coordinates[i].x;
                else if (coordinates[i].x > maxX)
                    maxX = coordinates[i].x;

                if (coordinates[i].y < minY)
                    minY = coordinates[i].y;
                else if (coordinates[i].y > maxY)
                    maxY = coordinates[i].y;
            }

            Vector2D selectionCenter = new Vector2D(minX + (maxX - minX) / 2, minY + (maxY - minY) / 2);

            //move them
            for (int i = 0; i < coordinates.Length; i++)
                translatedCoords[i] = new Vector3D((float)Math.Round(direction.x - (selectionCenter.x - coordinates[i].x)), (float)Math.Round(direction.y - (selectionCenter.y - coordinates[i].y)), (float)Math.Round(coordinates[i].z));

            return translatedCoords;
        }

		//mxd
		internal void SelectSideParts(Sidedef side, bool toggleTop, bool toggleMid, bool toggleBottom, bool select, bool withSameTexture, bool withSameHeight) {
			BaseVisualSector vs = GetVisualSector(side.Sector) as BaseVisualSector;
			
			if(toggleTop && vs.Sides[side].upper != null &&
				((select && !vs.Sides[side].upper.Selected) || (!select && vs.Sides[side].upper.Selected))) {
				vs.Sides[side].upper.SelectNeighbours(select, withSameTexture, withSameHeight);
			}

			if(toggleMid && vs.Sides[side].middlesingle != null &&
				((select && !vs.Sides[side].middlesingle.Selected) || (!select && vs.Sides[side].middlesingle.Selected))) {
				vs.Sides[side].middlesingle.SelectNeighbours(select, withSameTexture, withSameHeight);
			}

			if(toggleMid && vs.Sides[side].middledouble != null &&
				((select && !vs.Sides[side].middledouble.Selected) || (!select && vs.Sides[side].middledouble.Selected))) {
				vs.Sides[side].middledouble.SelectNeighbours(select, withSameTexture, withSameHeight);
			}

			if(toggleBottom && vs.Sides[side].lower != null &&
				((select && !vs.Sides[side].lower.Selected) || (!select && vs.Sides[side].lower.Selected))) {
				vs.Sides[side].lower.SelectNeighbours(select, withSameTexture, withSameHeight);
			}
		}
		
		#endregion

		#region ================== Extended Methods

		// This requests a sector's extra data
		internal SectorData GetSectorData(Sector s)
		{
			// Make fresh sector data when it doesn't exist yet
			if(!sectordata.ContainsKey(s))
				sectordata[s] = new SectorData(this, s);
			
			return sectordata[s];
		}
		
		// This requests a things's extra data
		internal ThingData GetThingData(Thing t)
		{
			// Make fresh sector data when it doesn't exist yet
			if(!thingdata.ContainsKey(t))
				thingdata[t] = new ThingData(this, t);
			
			return thingdata[t];
		}

		//mxd
		internal VertexData GetVertexData(Vertex v) {
			if(!vertexdata.ContainsKey(v))
				vertexdata[v] = new VertexData(this, v);
			return vertexdata[v];
		}

		//mxd
		internal void UpdateVertexHandle(Vertex v) {
			if(!vertices.ContainsKey(v))
				vertices.Add(v, new VisualVertexPair(new BaseVisualVertex(this, v, true), new BaseVisualVertex(this, v, false)));
			else
				vertices[v].Changed = true;
		}
		
		// This rebuilds the sector data
		// This requires that the blockmap is up-to-date!
		internal void RebuildElementData()
		{
			//mxd
            if (!gzdoomRenderingEffects) {
				if(sectordata != null && sectordata.Count > 0) {
					//rebuild sectors with effects
					foreach(KeyValuePair<Sector, SectorData> group in sectordata)
						group.Value.Reset();
				}

				//remove all vertex handles from selection
				if(vertices != null && vertices.Count > 0) {
					foreach(IVisualEventReceiver i in selectedobjects){
						if(i is BaseVisualVertex) RemoveSelectedObject(i);
					}
				}
            }

            Dictionary<int, List<Sector>> sectortags = new Dictionary<int, List<Sector>>();
            sectordata = new Dictionary<Sector, SectorData>(General.Map.Map.Sectors.Count);
            thingdata = new Dictionary<Thing, ThingData>(General.Map.Map.Things.Count);

			if(General.Map.UDMF) {
				vertexdata = new Dictionary<Vertex, VertexData>(General.Map.Map.Vertices.Count); //mxd
				vertices.Clear();
			}

            if (!gzdoomRenderingEffects) return; //mxd
			
			// Find all sector who's tag is not 0 and hash them so that we can find them quicly
			foreach(Sector s in General.Map.Map.Sectors)
			{
				if(s.Tag != 0)
				{
					if(!sectortags.ContainsKey(s.Tag)) sectortags[s.Tag] = new List<Sector>();
					sectortags[s.Tag].Add(s);
				}
			}

			// Find sectors with 3 vertices, because they can be sloped
			foreach(Sector s in General.Map.Map.Sectors)
			{
				// ========== Thing vertex slope, vertices with UDMF vertex offsets ==========
				if(s.Sidedefs.Count == 3)
				{
					if(General.Map.UDMF) //mxd
						GetSectorData(s).AddEffectVertexOffset();

					List<Thing> slopeceilingthings = new List<Thing>(3);
					List<Thing> slopefloorthings = new List<Thing>(3);
					foreach(Sidedef sd in s.Sidedefs) {
						Vertex v = sd.IsFront ? sd.Line.End : sd.Line.Start;

						// Check if a thing is at this vertex
						VisualBlockEntry b = blockmap.GetBlock(blockmap.GetBlockCoordinates(v.Position));
						foreach(Thing t in b.Things) {
							if((Vector2D)t.Position == v.Position) {
								if(t.Type == 1504)
									slopefloorthings.Add(t);
								else if(t.Type == 1505)
									slopeceilingthings.Add(t);
							}
						}
					}

					// Slope any floor vertices?
					if(slopefloorthings.Count > 0) {
						SectorData sd = GetSectorData(s);
						sd.AddEffectThingVertexSlope(slopefloorthings, true);
					}

					// Slope any ceiling vertices?
					if(slopeceilingthings.Count > 0) {
						SectorData sd = GetSectorData(s);
						sd.AddEffectThingVertexSlope(slopeceilingthings, false);
					}
				}
			}
			
			// Find interesting linedefs (such as line slopes)
			foreach(Linedef l in General.Map.Map.Linedefs)
			{
				// ========== Plane Align (see http://zdoom.org/wiki/Plane_Align) ==========
				if(l.Action == 181)
				{
					// Slope front
					if(((l.Args[0] == 1) || (l.Args[1] == 1)) && (l.Front != null))
					{
						SectorData sd = GetSectorData(l.Front.Sector);
						sd.AddEffectLineSlope(l);
					}
					
					// Slope back
					if(((l.Args[0] == 2) || (l.Args[1] == 2)) && (l.Back != null))
					{
						SectorData sd = GetSectorData(l.Back.Sector);
						sd.AddEffectLineSlope(l);
					}
				}
				// ========== Plane Copy (mxd) (see http://zdoom.org/wiki/Plane_Copy) ==========
				else if(l.Action == 118)
				{
					//check sodding flags...
					bool floorCopyToBack = false;
					bool floorCopyToFront = false;
					bool ceilingCopyToBack = false;
					bool ceilingCopyToFront = false;

					if(l.Args[4] > 0 && l.Args[4] != 3 && l.Args[4] != 12) {
						floorCopyToBack = (l.Args[4] & 1) == 1;
						floorCopyToFront = (l.Args[4] & 2) == 2;
						ceilingCopyToBack = (l.Args[4] & 4) == 4;
						ceilingCopyToFront = (l.Args[4] & 8) == 8;
					}
					
					// Copy slope to front sector
					//Flags: Back floor to front sector or Back ceiling to front sector
					if(l.Front != null) {
						if((l.Args[0] > 0 || l.Args[1] > 0) || (floorCopyToFront && l.Args[2] > 0) || (ceilingCopyToFront && l.Args[3] > 0)) {
							SectorData sd = GetSectorData(l.Front.Sector);
							sd.AddEffectPlaneClopySlope(l, true);
						}
					}

					// Copy slope to back sector
					//Flags: Copy front floor to back sector or Front ceiling to back sector
					if(l.Back != null) {
						if((l.Args[2] > 0 || l.Args[3] > 0) || (floorCopyToBack && l.Args[0] > 0) || (ceilingCopyToBack && l.Args[1] > 0)) {
							SectorData sd = GetSectorData(l.Back.Sector);
							sd.AddEffectPlaneClopySlope(l, false);
						}
					}
				}
				// ========== Sector 3D floor (see http://zdoom.org/wiki/Sector_Set3dFloor) ==========
				else if((l.Action == 160) && (l.Front != null))
				{
					int sectortag = l.Args[0] + (l.Args[4] << 8);
					if(sectortags.ContainsKey(sectortag))
					{
						List<Sector> sectors = sectortags[sectortag];
						foreach(Sector s in sectors)
						{
							SectorData sd = GetSectorData(s);
							sd.AddEffect3DFloor(l);
						}
					}
				}
				// ========== Transfer Brightness (see http://zdoom.org/wiki/ExtraFloor_LightOnly) =========
				else if((l.Action == 50) && (l.Front != null))
				{
					if(sectortags.ContainsKey(l.Args[0]))
					{
						List<Sector> sectors = sectortags[l.Args[0]];
						foreach(Sector s in sectors)
						{
							SectorData sd = GetSectorData(s);
							sd.AddEffectBrightnessLevel(l);
						}
					}
				}
			}

			// Find interesting things (such as sector slopes)
			foreach(Thing t in General.Map.Map.Things)
			{
				// ========== Copy slope ==========
				if((t.Type == 9510) || (t.Type == 9511))
				{
					t.DetermineSector(blockmap);
					if(t.Sector != null)
					{
						SectorData sd = GetSectorData(t.Sector);
						sd.AddEffectCopySlope(t);
					}
				}
				// ========== Thing line slope ==========
				else if((t.Type == 9500) || (t.Type == 9501))
				{
					t.DetermineSector(blockmap);
					if(t.Sector != null)
					{
						SectorData sd = GetSectorData(t.Sector);
						sd.AddEffectThingLineSlope(t);
					}
				}
			}
		}
		
		#endregion

		#region ================== Events

		// Help!
		public override void OnHelp()
		{
			General.ShowHelp("e_visual.html");
		}

		// When entering this mode
		public override void OnEngage()
		{
			base.OnEngage();

			//mxd
			useSelectionFromClassicMode = BuilderPlug.Me.SyncSelection ? !General.Interface.ShiftState : General.Interface.ShiftState;
			
			// Read settings
			cameraflooroffset = General.Map.Config.ReadSetting("cameraflooroffset", cameraflooroffset);
			cameraceilingoffset = General.Map.Config.ReadSetting("cameraceilingoffset", cameraceilingoffset);

			RebuildElementData();
		}

		// When returning to another mode
		public override void OnDisengage()
		{
			base.OnDisengage();

			//mxd
			if(BuilderPlug.Me.SyncSelection ? !General.Interface.ShiftState : General.Interface.ShiftState) {
				//clear previously selected stuff
				General.Map.Map.ClearAllSelected();
				
				//refill selection
				List<Sector> selectedSectors = new List<Sector>();
				List<Linedef> selectedLines = new List<Linedef>();
				List<Vertex> selectedVertices = new List<Vertex>();

				foreach(IVisualEventReceiver obj in selectedobjects) {
					if(obj is BaseVisualThing) {
						((BaseVisualThing)obj).Thing.Selected = true;
					} else if(obj is VisualFloor || obj is VisualCeiling) {
						VisualGeometry vg = (VisualGeometry)obj;

						if(vg.Sector != null && vg.Sector.Sector != null && !selectedSectors.Contains(vg.Sector.Sector)) {
							selectedSectors.Add(vg.Sector.Sector);

							foreach(Sidedef s in vg.Sector.Sector.Sidedefs){
								if(!selectedLines.Contains(s.Line))
									selectedLines.Add(s.Line);
							}
						}
					} else if(obj is VisualLower || obj is VisualUpper || obj is VisualMiddleDouble || obj is VisualMiddleSingle || obj is VisualMiddle3D) {
						VisualGeometry vg = (VisualGeometry)obj;

						if(vg.Sidedef != null && !selectedLines.Contains(vg.Sidedef.Line))
							selectedLines.Add(vg.Sidedef.Line);
					}
				}

				foreach(Sector s in selectedSectors) 
					s.Selected = true;

				foreach(Linedef l in selectedLines) {
					l.Selected = true;

					if(!selectedVertices.Contains(l.Start))
						selectedVertices.Add(l.Start);
					if(!selectedVertices.Contains(l.End))
						selectedVertices.Add(l.End);
				}

				foreach(Vertex v in selectedVertices)
					v.Selected = true;
			}

            copyBuffer.Clear(); //mxd
			General.Map.Map.Update();
		}
		
		// Processing
		public override void OnProcess(float deltatime)
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
				SectorData sd = GetSectorData(General.Map.VisualCamera.Sector);
				if(!sd.Updated) sd.Update();

				// Camera below floor level?
				Vector3D feetposition = General.Map.VisualCamera.Position;
				SectorLevel floorlevel = sd.GetFloorBelow(feetposition) ?? sd.LightLevels[0];
				float floorheight = floorlevel.plane.GetZ(General.Map.VisualCamera.Position);
				if(General.Map.VisualCamera.Position.z < (floorheight + cameraflooroffset + 0.1f))
				{
					// Stay above floor
					gravity = new Vector3D(0.0f, 0.0f, 0.0f);
					General.Map.VisualCamera.Position = new Vector3D(General.Map.VisualCamera.Position.x,
																	 General.Map.VisualCamera.Position.y,
																	 floorheight + cameraflooroffset);
				}
				else
				{
					// Fall down
					gravity.z += GRAVITY * deltatime;
					if(gravity.z > 3.0f) gravity.z = 3.0f;

					// Test if we don't go through a floor
					if((General.Map.VisualCamera.Position.z + gravity.z) < (floorheight + cameraflooroffset + 0.1f))
					{
						// Stay above floor
						gravity = new Vector3D(0.0f, 0.0f, 0.0f);
						General.Map.VisualCamera.Position = new Vector3D(General.Map.VisualCamera.Position.x,
																		 General.Map.VisualCamera.Position.y,
																		 floorheight + cameraflooroffset);
					}
					else
					{
						// Apply gravity vector
						General.Map.VisualCamera.Position += gravity;
					}
				}

				// Camera above ceiling?
				feetposition = General.Map.VisualCamera.Position - new Vector3D(0, 0, cameraflooroffset - 7.0f);
				SectorLevel ceillevel = sd.GetCeilingAbove(feetposition) ?? sd.LightLevels[sd.LightLevels.Count - 1];
				float ceilheight = ceillevel.plane.GetZ(General.Map.VisualCamera.Position);
				if(General.Map.VisualCamera.Position.z > (ceilheight - cameraceilingoffset - 0.01f))
				{
					// Stay below ceiling
					General.Map.VisualCamera.Position = new Vector3D(General.Map.VisualCamera.Position.x,
																	 General.Map.VisualCamera.Position.y,
																	 ceilheight - cameraceilingoffset);
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

				// Set target for highlighting
                renderer.ShowSelection = General.Settings.GZOldHighlightMode || BuilderPlug.Me.UseHighlight; //mxd

				if(BuilderPlug.Me.UseHighlight)
					renderer.SetHighlightedObject(target.picked);
				
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

				//mxd
				if(General.Map.UDMF && General.Settings.GZShowVisualVertices && vertices.Count > 0) {
					List<VisualVertex> verts = new List<VisualVertex>();

					foreach(KeyValuePair<Vertex, VisualVertexPair> pair in vertices)
						verts.AddRange(pair.Value.Vertices);

					renderer.AddVisualVertices(verts.ToArray());
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
			RebuildElementData();
			PickTarget();
		}
		
		// This usually happens when geometry is changed by undo, redo, cut or paste actions
		// and uses the marks to check what needs to be reloaded.
		protected override void ResourcesReloadedPartial()
		{
			bool sectorsmarked = false;
			
			if(General.Map.UndoRedo.GeometryChanged)
			{
				// Let the core do this (it will just dispose the sectors that were changed)
				base.ResourcesReloadedPartial();
			}
			else
			{
				// Neighbour sectors must be updated as well
				foreach(Sector s in General.Map.Map.Sectors)
				{
					if(s.Marked)
					{
						sectorsmarked = true;
						foreach(Sidedef sd in s.Sidedefs)
						{
							sd.Marked = true;
							if(sd.Other != null) sd.Other.Marked = true;
						}
					}
				}
				
				// Go for all sidedefs to update
				foreach(Sidedef sd in General.Map.Map.Sidedefs)
				{
					if(sd.Marked && VisualSectorExists(sd.Sector))
					{
						BaseVisualSector vs = (BaseVisualSector)GetVisualSector(sd.Sector);
						VisualSidedefParts parts = vs.GetSidedefParts(sd);
						parts.SetupAllParts();
					}
				}
				
				// Go for all sectors to update
				foreach(Sector s in General.Map.Map.Sectors)
				{
					if(s.Marked)
					{
						SectorData sd = GetSectorData(s);
						sd.Reset();
						
						// UpdateSectorGeometry for associated sectors (sd.UpdateAlso) as well!
						foreach(KeyValuePair<Sector, bool> us in sd.UpdateAlso)
						{
							if(VisualSectorExists(us.Key))
							{
								BaseVisualSector vs = (BaseVisualSector)GetVisualSector(us.Key);
								vs.UpdateSectorGeometry(us.Value);
							}
						}
						
						// And update for this sector ofcourse
						if(VisualSectorExists(s))
						{
							BaseVisualSector vs = (BaseVisualSector)GetVisualSector(s);
							vs.UpdateSectorGeometry(false);
						}
					}
				}
				
				if(!sectorsmarked)
				{
					// No sectors or geometry changed. So we only have
					// to update things when they have changed.
					foreach(KeyValuePair<Thing, VisualThing> vt in allthings)
						if((vt.Value != null) && vt.Key.Marked) (vt.Value as BaseVisualThing).Rebuild();
				}
				else
				{
					// Things depend on the sector they are in and because we can't
					// easily determine which ones changed, we dispose all things
					foreach(KeyValuePair<Thing, VisualThing> vt in allthings)
						if(vt.Value != null) vt.Value.Dispose();
					
					// Apply new lists
					allthings = new Dictionary<Thing, VisualThing>(allthings.Count);
				}
				
				// Clear visibility collections
				visiblesectors.Clear();
				visibleblocks.Clear();
				visiblegeometry.Clear();
				visiblethings.Clear();
				
				// Make new blockmap
				if(sectorsmarked || General.Map.UndoRedo.PopulationChanged)
					FillBlockMap();
				
				RebuildElementData();
				UpdateChangedObjects();
				
				// Visibility culling (this re-creates the needed resources)
				DoCulling();
			}
			
			// Determine what we're aiming at now
			PickTarget();
		}
		
		// Mouse moves
		public override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);
			GetTargetEventReceiver(true).OnMouseMove(e);
		}
		
		// Undo performed
		public override void OnUndoEnd()
		{
            base.OnUndoEnd();

            //mxd. Effects may've become invalid
			if(gzdoomRenderingEffects && sectordata != null && sectordata.Count > 0)
				RebuildElementData();

			//mxd. As well as geometry...
            foreach(KeyValuePair<Sector, VisualSector> group in visiblesectors){
                if (group.Value is BaseVisualSector)
                    ((BaseVisualSector)group.Value).Rebuild();
            }

			RebuildSelectedObjectsList();
			
			// We can't group with this undo level anymore
			lastundogroup = UndoGroup.None;
		}
		
		// Redo performed
		public override void OnRedoEnd()
		{
			base.OnRedoEnd();

			//mxd. Effects may've become invalid
			if(sectordata != null && sectordata.Count > 0)
				RebuildElementData();

			//mxd. As well as geometry...
            foreach (KeyValuePair<Sector, VisualSector> group in visiblesectors) {
                if (group.Value is BaseVisualSector)
                    ((BaseVisualSector)group.Value).Rebuild();
            }

			RebuildSelectedObjectsList();
		}
		
		#endregion

		#region ================== Action Assist

		// Because some actions can only be called on a single (the targeted) object because
		// they show a dialog window or something, these functions help applying the result
		// to all compatible selected objects.
		
		// Apply texture offsets
		public void ApplyTextureOffsetChange(int dx, int dy)
		{
			Dictionary<Sidedef, int> donesides = new Dictionary<Sidedef, int>(selectedobjects.Count);
			List<IVisualEventReceiver> objs = GetSelectedObjects(false, true, false, false);
			foreach(IVisualEventReceiver i in objs)
			{
				if(i is BaseVisualGeometrySidedef)
				{
					if(!donesides.ContainsKey((i as BaseVisualGeometrySidedef).Sidedef))
					{
						i.OnChangeTextureOffset(dx, dy, false);
						donesides.Add((i as BaseVisualGeometrySidedef).Sidedef, 0);
					}
				}
			}
		}

		// Apply flat offsets
		public void ApplyFlatOffsetChange(int dx, int dy)
		{
			Dictionary<Sector, int> donesectors = new Dictionary<Sector, int>(selectedobjects.Count);
			List<IVisualEventReceiver> objs = GetSelectedObjects(true, false, false, false);
			foreach(IVisualEventReceiver i in objs)
			{
				if(i is BaseVisualGeometrySector)
				{
					if(!donesectors.ContainsKey((i as BaseVisualGeometrySector).Sector.Sector))
					{
						i.OnChangeTextureOffset(dx, dy, false);
						donesectors.Add((i as BaseVisualGeometrySector).Sector.Sector, 0);
					}
				}
			}
		}

		// Apply upper unpegged flag
		public void ApplyUpperUnpegged(bool set)
		{
			List<IVisualEventReceiver> objs = GetSelectedObjects(false, true, false, false);
			foreach(IVisualEventReceiver i in objs)
			{
				i.ApplyUpperUnpegged(set);
			}
		}

		// Apply lower unpegged flag
		public void ApplyLowerUnpegged(bool set)
		{
			List<IVisualEventReceiver> objs = GetSelectedObjects(false, true, false, false);
			foreach(IVisualEventReceiver i in objs)
			{
				i.ApplyLowerUnpegged(set);
			}
		}

		// Apply texture change
		public void ApplySelectTexture(string texture, bool flat)
		{
			List<IVisualEventReceiver> objs;
			
			if(General.Map.Config.MixTexturesFlats)
			{
				// Apply on all compatible types
				objs = GetSelectedObjects(true, true, false, false);
			}
			else
			{
				// We don't want to mix textures and flats, so apply only on the appropriate type
				objs = GetSelectedObjects(flat, !flat, false, false);
			}
			
			foreach(IVisualEventReceiver i in objs)
			{
				i.ApplyTexture(texture);
			}
		}

		// This returns all selected objects
		internal List<IVisualEventReceiver> GetSelectedObjects(bool includesectors, bool includesidedefs, bool includethings, bool includevertices)
		{
			List<IVisualEventReceiver> objs = new List<IVisualEventReceiver>();
			foreach(IVisualEventReceiver i in selectedobjects)
			{
				if((i is BaseVisualGeometrySector) && includesectors) objs.Add(i);
				else if((i is BaseVisualGeometrySidedef) && includesidedefs) objs.Add(i);
				else if((i is BaseVisualThing) && includethings) objs.Add(i);
				else if((i is BaseVisualVertex) && includevertices) objs.Add(i);//mxd
			}

			// Add highlight?
			if(selectedobjects.Count == 0)
			{
				IVisualEventReceiver i = (target.picked as IVisualEventReceiver);
				if((i is BaseVisualGeometrySector) && includesectors) objs.Add(i);
				else if((i is BaseVisualGeometrySidedef) && includesidedefs) objs.Add(i);
				else if((i is BaseVisualThing) && includethings) objs.Add(i);
				else if((i is BaseVisualVertex) && includevertices) objs.Add(i);//mxd
			}

			return objs;
		}

		// This returns all selected sectors, no doubles
		public List<Sector> GetSelectedSectors()
		{
			Dictionary<Sector, int> added = new Dictionary<Sector, int>();
			List<Sector> sectors = new List<Sector>();
			foreach(IVisualEventReceiver i in selectedobjects)
			{
				if(i is BaseVisualGeometrySector)
				{
					Sector s = (i as BaseVisualGeometrySector).Level.sector;
					if(!added.ContainsKey(s))
					{
						sectors.Add(s);
						added.Add(s, 0);
					}
				}
			}

			// Add highlight?
			if((selectedobjects.Count == 0) && (target.picked is BaseVisualGeometrySector))
			{
				Sector s = (target.picked as BaseVisualGeometrySector).Level.sector;
				if(!added.ContainsKey(s))
					sectors.Add(s);
			}
			
			return sectors;
		}

		// This returns all selected linedefs, no doubles
		public List<Linedef> GetSelectedLinedefs()
		{
			Dictionary<Linedef, int> added = new Dictionary<Linedef, int>();
			List<Linedef> linedefs = new List<Linedef>();
			foreach(IVisualEventReceiver i in selectedobjects)
			{
				if(i is BaseVisualGeometrySidedef)
				{
					//Linedef l = (i as BaseVisualGeometrySidedef).Sidedef.Line;
					Linedef l = (i as BaseVisualGeometrySidedef).GetControlLinedef(); //mxd
					if(!added.ContainsKey(l))
					{
						linedefs.Add(l);
						added.Add(l, 0);
					}
				}
			}

			// Add highlight?
			if((selectedobjects.Count == 0) && (target.picked is BaseVisualGeometrySidedef))
			{
				//Linedef l = (target.picked as BaseVisualGeometrySidedef).Sidedef.Line;
				Linedef l = (target.picked as BaseVisualGeometrySidedef).GetControlLinedef(); //mxd
				if(!added.ContainsKey(l))
					linedefs.Add(l);
			}

			return linedefs;
		}

		// This returns all selected sidedefs, no doubles
		public List<Sidedef> GetSelectedSidedefs()
		{
			Dictionary<Sidedef, int> added = new Dictionary<Sidedef, int>();
			List<Sidedef> sidedefs = new List<Sidedef>();
			foreach(IVisualEventReceiver i in selectedobjects)
			{
				if(i is BaseVisualGeometrySidedef)
				{
					Sidedef sd = (i as BaseVisualGeometrySidedef).Sidedef;
					if(!added.ContainsKey(sd))
					{
						sidedefs.Add(sd);
						added.Add(sd, 0);
					}
				}
			}

			// Add highlight?
			if((selectedobjects.Count == 0) && (target.picked is BaseVisualGeometrySidedef))
			{
				Sidedef sd = (target.picked as BaseVisualGeometrySidedef).Sidedef;
				if(!added.ContainsKey(sd))
					sidedefs.Add(sd);
			}

			return sidedefs;
		}

		// This returns all selected things, no doubles
		public List<Thing> GetSelectedThings()
		{
			Dictionary<Thing, int> added = new Dictionary<Thing, int>();
			List<Thing> things = new List<Thing>();
			foreach(IVisualEventReceiver i in selectedobjects)
			{
				if(i is BaseVisualThing)
				{
					Thing t = (i as BaseVisualThing).Thing;
					if(!added.ContainsKey(t))
					{
						things.Add(t);
						added.Add(t, 0);
					}
				}
			}

			// Add highlight?
			if((selectedobjects.Count == 0) && (target.picked is BaseVisualThing))
			{
				Thing t = (target.picked as BaseVisualThing).Thing;
				if(!added.ContainsKey(t))
					things.Add(t);
			}

			return things;
		}

		//mxd. This returns all selected vertices, no doubles
		public List<Vertex> GetSelectedVertices() {
			Dictionary<Vertex, int> added = new Dictionary<Vertex, int>();
			List<Vertex> verts = new List<Vertex>();

			foreach(IVisualEventReceiver i in selectedobjects) {
				if(i is BaseVisualVertex) {
					Vertex v = (i as BaseVisualVertex).Vertex;
					
					if(!added.ContainsKey(v)) {
						verts.Add(v);
						added.Add(v, 0);
					}
				}
			}

			// Add highlight?
			if((selectedobjects.Count == 0) && (target.picked is BaseVisualVertex)) {
				Vertex v = (target.picked as BaseVisualVertex).Vertex;
				if(!added.ContainsKey(v))
					verts.Add(v);
			}

			return verts;
		}
		
		// This returns the IVisualEventReceiver on which the action must be performed
		private IVisualEventReceiver GetTargetEventReceiver(bool targetonly)
		{
			if(target.picked != null)
			{
				if(singleselection || target.picked.Selected || targetonly)
				{
					return (IVisualEventReceiver)target.picked;
				}
				else if(selectedobjects.Count > 0)
				{
					return selectedobjects[0];
				}
				else
				{
					return (IVisualEventReceiver)target.picked;
				}
			}
			else
			{
				return new NullVisualEventReceiver();
			}
		}

        //mxd. Copied from BuilderModes.ThingsMode
        // This creates a new thing
        private Thing CreateThing(Vector2D pos) {
            if (pos.x < General.Map.Config.LeftBoundary || pos.x > General.Map.Config.RightBoundary ||
                pos.y > General.Map.Config.TopBoundary || pos.y < General.Map.Config.BottomBoundary) {
                General.Interface.DisplayStatus(StatusType.Warning, "Failed to insert thing: outside of map boundaries.");
                return null;
            }

            // Create thing
            Thing t = General.Map.Map.CreateThing();
            if (t != null) {
                General.Settings.ApplyDefaultThingSettings(t);
                t.Move(pos);
                t.UpdateConfiguration();
                General.Map.IsChanged = true;
                
                // Update things filter so that it includes this thing
                General.Map.ThingsFilter.Update();

                // Snap to grid enabled?
                if (General.Interface.SnapToGrid) {
                    // Snap to grid
                    t.SnapToGrid();
                } else {
                    // Snap to map format accuracy
                    t.SnapToAccuracy();
                }
            }

            return t;
        }
		
		#endregion

		#region ================== Actions

		[BeginAction("clearselection", BaseAction = true)]
		public void ClearSelection()
		{
			selectedobjects = new List<IVisualEventReceiver>();
			
			foreach(KeyValuePair<Sector, VisualSector> vs in allsectors)
			{
				if(vs.Value != null)
				{
					BaseVisualSector bvs = (BaseVisualSector)vs.Value;
					if(bvs.Floor != null) bvs.Floor.Selected = false;
					if(bvs.Ceiling != null) bvs.Ceiling.Selected = false;
					foreach(VisualFloor vf in bvs.ExtraFloors) vf.Selected = false;
					foreach(VisualCeiling vc in bvs.ExtraCeilings) vc.Selected = false;
					foreach(VisualFloor vf in bvs.ExtraBackFloors) vf.Selected = false; //mxd
					foreach(VisualCeiling vc in bvs.ExtraBackCeilings) vc.Selected = false; //mxd

					foreach(Sidedef sd in vs.Key.Sidedefs)
					{
						List<VisualGeometry> sidedefgeos = bvs.GetSidedefGeometry(sd);
						foreach(VisualGeometry sdg in sidedefgeos)
						{
							sdg.Selected = false;
						}
					}
				}
			}

			foreach(KeyValuePair<Thing, VisualThing> vt in allthings)
			{
				if(vt.Value != null)
				{
					BaseVisualThing bvt = (BaseVisualThing)vt.Value;
					bvt.Selected = false;
				}
			}

			//mxd
			if(General.Map.UDMF) {
				foreach(KeyValuePair<Vertex, VisualVertexPair> pair in vertices) {
					pair.Value.Deselect();
				}
			}
		}

		[BeginAction("visualselect", BaseAction = true)]
		public void BeginSelect()
		{
			PreActionNoChange();
			PickTargetUnlocked();
			GetTargetEventReceiver(true).OnSelectBegin();
			PostAction();
		}

		[EndAction("visualselect", BaseAction = true)]
		public void EndSelect()
		{
			//PreActionNoChange();
			IVisualEventReceiver target = GetTargetEventReceiver(true);
			target.OnSelectEnd();

			//mxd
			if((General.Interface.ShiftState || General.Interface.CtrlState) && selectedobjects.Count > 0) {
				IVisualEventReceiver[] selection = new IVisualEventReceiver[selectedobjects.Count];
				selectedobjects.CopyTo(selection);

				foreach(IVisualEventReceiver obj in selection)
					obj.SelectNeighbours(target.IsSelected(), General.Interface.ShiftState, General.Interface.CtrlState);
			}

			Renderer.ShowSelection = true;
			Renderer.ShowHighlight = true;
			PostAction();
		}

		[BeginAction("visualedit", BaseAction = true)]
		public void BeginEdit()
		{
			PreAction(UndoGroup.None);
			GetTargetEventReceiver(false).OnEditBegin();
			PostAction();
		}

		[EndAction("visualedit", BaseAction = true)]
		public void EndEdit()
		{
			PreActionNoChange();
			GetTargetEventReceiver(false).OnEditEnd();
			PostAction();
		}

		[BeginAction("raisesector8")]
		public void RaiseSector8()
		{
			PreAction(UndoGroup.SectorHeightChange);
			List<IVisualEventReceiver> objs = GetSelectedObjects(true, true, true, true);
			foreach(IVisualEventReceiver i in objs) i.OnChangeTargetHeight(8);
			PostAction();
		}

		[BeginAction("lowersector8")]
		public void LowerSector8()
		{
			PreAction(UndoGroup.SectorHeightChange);
			List<IVisualEventReceiver> objs = GetSelectedObjects(true, true, true, true);
			foreach(IVisualEventReceiver i in objs) i.OnChangeTargetHeight(-8);
			PostAction();
		}

		[BeginAction("raisesector1")]
		public void RaiseSector1()
		{
			PreAction(UndoGroup.SectorHeightChange);
			List<IVisualEventReceiver> objs = GetSelectedObjects(true, true, true, true);
			foreach(IVisualEventReceiver i in objs) i.OnChangeTargetHeight(1);
			PostAction();
		}

		[BeginAction("lowersector1")]
		public void LowerSector1()
		{
			PreAction(UndoGroup.SectorHeightChange);
			List<IVisualEventReceiver> objs = GetSelectedObjects(true, true, true, true);
			foreach(IVisualEventReceiver i in objs) i.OnChangeTargetHeight(-1);
			PostAction();
		}

		//mxd
		[BeginAction("raisesectortonearest")]
		public void RaiseSectorToNearest() {
			Dictionary<Sector, VisualFloor> floors = new Dictionary<Sector, VisualFloor>();
			Dictionary<Sector, VisualCeiling> ceilings = new Dictionary<Sector, VisualCeiling>();
			List<BaseVisualThing> things = new List<BaseVisualThing>();
			bool withinSelection = General.Interface.CtrlState;

			//get selection
			if(selectedobjects.Count == 0) {
				IVisualEventReceiver i = (target.picked as IVisualEventReceiver);
				if(i is VisualFloor) {
					VisualFloor vf = i as VisualFloor;
					floors.Add(vf.Level.sector, vf);
				} else if(i is VisualCeiling) {
					VisualCeiling vc = i as VisualCeiling;
					ceilings.Add(vc.Level.sector, vc);
				} else if(i is BaseVisualThing) {
					things.Add(i as BaseVisualThing);
				}
			} else {
				foreach(IVisualEventReceiver i in selectedobjects) {
					if(i is VisualFloor) {
						VisualFloor vf = i as VisualFloor;
						floors.Add(vf.Level.sector, vf);
					} else if(i is VisualCeiling) {
						VisualCeiling vc = i as VisualCeiling;
						ceilings.Add(vc.Level.sector, vc);
					} else if(i is BaseVisualThing) {
						things.Add(i as BaseVisualThing);
					}
				}
			}

			//check what we have
			if(floors.Count + ceilings.Count == 0 && (things.Count == 0 || !General.Map.FormatInterface.HasThingHeight)) {
				General.Interface.DisplayStatus(StatusType.Warning, "No suitable objects found!");
				return;
			} 
			
			if(withinSelection) {
				string s = string.Empty;

				if(floors.Count == 1) s = "floors";

				if(ceilings.Count == 1) {
					if(!string.IsNullOrEmpty(s)) s += " and ";
					s += "ceilings";
				}

				if(!string.IsNullOrEmpty(s)){
					General.Interface.DisplayStatus(StatusType.Warning, "Can't do: at least 2 selected " + s + " are required!");
					return;
				}
			}

			//process floors...
			int maxSelectedHeight = int.MinValue;
			int minSelectedCeilingHeight = int.MaxValue;
			int targetFloorHeight = int.MaxValue;

			//get maximum floor and minimum ceiling heights from selection
			foreach(KeyValuePair<Sector, VisualFloor> group in floors) {
				if(group.Key.FloorHeight > maxSelectedHeight)
					maxSelectedHeight = group.Key.FloorHeight;

				if(group.Key.CeilHeight < minSelectedCeilingHeight)
					minSelectedCeilingHeight = group.Key.CeilHeight;
			}

			if(withinSelection) {
				//check heights
				if(minSelectedCeilingHeight < maxSelectedHeight) {
					General.Interface.DisplayStatus(StatusType.Warning, "Can't do: lowest ceiling is lower than highest floor!");
					return;
				} else {
					targetFloorHeight = maxSelectedHeight;
				}
			} else {
				//get next higher floor from surrounding unselected sectors
				foreach(KeyValuePair<Sector, VisualFloor> group in floors) {
					foreach(Sidedef side in group.Key.Sidedefs) {
						if(side.Other == null || ceilings.ContainsKey(side.Other.Sector) || floors.ContainsKey(side.Other.Sector))
							continue;
						if(side.Other.Sector.FloorHeight > maxSelectedHeight && side.Other.Sector.FloorHeight < targetFloorHeight && side.Other.Sector.FloorHeight <= minSelectedCeilingHeight)
							targetFloorHeight = side.Other.Sector.FloorHeight;
					}
				}
			}

			//ceilings...
			maxSelectedHeight = int.MinValue;
			int targetCeilingHeight = int.MaxValue;

			//get highest ceiling height from selection
			foreach(KeyValuePair<Sector, VisualCeiling> group in ceilings) {
				if(group.Key.CeilHeight > maxSelectedHeight)
					maxSelectedHeight = group.Key.CeilHeight;
			}

			if(withinSelection) {
				//we are raising, so we don't need to check anything
				targetCeilingHeight = maxSelectedHeight;
			} else {
				//get next higher ceiling from surrounding unselected sectors
				foreach(KeyValuePair<Sector, VisualCeiling> group in ceilings) {
					foreach(Sidedef side in group.Key.Sidedefs) {
						if(side.Other == null || ceilings.ContainsKey(side.Other.Sector) || floors.ContainsKey(side.Other.Sector))
							continue;
						if(side.Other.Sector.CeilHeight < targetCeilingHeight && side.Other.Sector.CeilHeight > maxSelectedHeight)
							targetCeilingHeight = side.Other.Sector.CeilHeight;
					}
				}
			}

			//CHECK VALUES
			string alignFailDescription = string.Empty;

			if(floors.Count > 0 && targetFloorHeight == int.MaxValue)
				alignFailDescription = floors.Count > 1 ? "floors" : "floor";

			if(ceilings.Count > 0 && targetCeilingHeight == int.MaxValue) {
				if(!string.IsNullOrEmpty(alignFailDescription))
					alignFailDescription += " and ";

				alignFailDescription += ceilings.Count > 1 ? "ceilings" : "ceiling";
			}

			if(!string.IsNullOrEmpty(alignFailDescription)) {
				General.Interface.DisplayStatus(StatusType.Warning, "Unable to align selected " + alignFailDescription + "!");
				return;
			}

			//APPLY VALUES
			PreAction(UndoGroup.SectorHeightChange);

			//change floors heights
			if(floors.Count > 0) {
				foreach(KeyValuePair<Sector, VisualFloor> group in floors) {
					if(targetFloorHeight != group.Key.FloorHeight)
						group.Value.OnChangeTargetHeight(targetFloorHeight - group.Key.FloorHeight);
				}
			}

			//change ceilings heights
			if(ceilings.Count > 0) {
				foreach(KeyValuePair<Sector, VisualCeiling> group in ceilings) {
					if(targetCeilingHeight != group.Key.CeilHeight)
						group.Value.OnChangeTargetHeight(targetCeilingHeight - group.Key.CeilHeight);
				}
			}

			//and things. Just align them to ceiling
			if(General.Map.FormatInterface.HasThingHeight) {
				foreach(BaseVisualThing vt in things) {
					if(vt.Thing.Sector == null) continue;
					ThingTypeInfo ti = General.Map.Data.GetThingInfo(vt.Thing.Type);
					int zvalue = (int)(vt.Thing.Sector.FloorHeight + vt.Thing.Position.z);

					if(zvalue != vt.Thing.Sector.CeilHeight - ti.Height)
						vt.OnChangeTargetHeight((int)(vt.Thing.Sector.CeilHeight - ti.Height) - zvalue);
				}
			}

			PostAction();
		}

		//mxd
		[BeginAction("lowersectortonearest")]
		public void LowerSectorToNearest() {
			Dictionary<Sector, VisualFloor> floors = new Dictionary<Sector, VisualFloor>();
			Dictionary<Sector, VisualCeiling> ceilings = new Dictionary<Sector, VisualCeiling>();
			List<BaseVisualThing> things = new List<BaseVisualThing>();
			bool withinSelection = General.Interface.CtrlState;

			//get selection
			if(selectedobjects.Count == 0) {
				IVisualEventReceiver i = (target.picked as IVisualEventReceiver);
				if(i is VisualFloor) {
					VisualFloor vf = i as VisualFloor;
					floors.Add(vf.Level.sector, vf);
				} else if(i is VisualCeiling) {
					VisualCeiling vc = i as VisualCeiling;
					ceilings.Add(vc.Level.sector, vc);
				} else if(i is BaseVisualThing) {
					things.Add(i as BaseVisualThing);
				}
			}else{
				foreach(IVisualEventReceiver i in selectedobjects) {
					if(i is VisualFloor) {
						VisualFloor vf = i as VisualFloor;
						floors.Add(vf.Level.sector, vf);
					} else if(i is VisualCeiling) {
						VisualCeiling vc = i as VisualCeiling;
						ceilings.Add(vc.Level.sector, vc);
					} else if(i is BaseVisualThing) {
						things.Add(i as BaseVisualThing);
					}
				}
			}

			//check what we have
			if(floors.Count + ceilings.Count == 0 && (things.Count == 0 || !General.Map.FormatInterface.HasThingHeight)) {
				General.Interface.DisplayStatus(StatusType.Warning, "No suitable objects found!");
				return;
			}

			if(withinSelection) {
				string s = string.Empty;

				if(floors.Count == 1) s = "floors";

				if(ceilings.Count == 1) {
					if(!string.IsNullOrEmpty(s)) s += " and ";
					s += "ceilings";
				}

				if(!string.IsNullOrEmpty(s)) {
					General.Interface.DisplayStatus(StatusType.Warning, "Can't do: at least 2 selected " + s + " are required!");
					return;
				}
			}

			//process floors...
			int minSelectedHeight = int.MaxValue;
			int targetFloorHeight = int.MinValue;

			//get minimum floor height from selection
			foreach(KeyValuePair<Sector, VisualFloor> group in floors) {
				if(group.Key.FloorHeight < minSelectedHeight)
					minSelectedHeight = group.Key.FloorHeight;
			}
			
			if(withinSelection) {
				//we are lowering, so we don't need to check anything
				targetFloorHeight = minSelectedHeight;
			} else {
				//get next floor lower height from surrounding unselected sectors
				foreach(KeyValuePair<Sector, VisualFloor> group in floors) {
					foreach(Sidedef side in group.Key.Sidedefs) {
						if(side.Other == null || ceilings.ContainsKey(side.Other.Sector) || floors.ContainsKey(side.Other.Sector))
							continue;
						if(side.Other.Sector.FloorHeight > targetFloorHeight && side.Other.Sector.FloorHeight < minSelectedHeight)
							targetFloorHeight = side.Other.Sector.FloorHeight;
					}
				}
			}

			//ceilings...
			minSelectedHeight = int.MaxValue;
			int maxSelectedFloorHeight = int.MinValue;
			int targetCeilingHeight = int.MinValue;

			//get minimum ceiling and maximum floor heights from selection
			foreach(KeyValuePair<Sector, VisualCeiling> group in ceilings) {
				if(group.Key.CeilHeight < minSelectedHeight)
					minSelectedHeight = group.Key.CeilHeight;

				if(group.Key.FloorHeight > maxSelectedFloorHeight)
					maxSelectedFloorHeight = group.Key.FloorHeight;
			}

			if(withinSelection) {
				if(minSelectedHeight < maxSelectedFloorHeight) {
					General.Interface.DisplayStatus(StatusType.Warning, "Can't do: lowest ceiling is lower than highest floor!");
					return;
				} else {
					targetCeilingHeight = minSelectedHeight;
				}
			} else {
				//get next lower ceiling height from surrounding unselected sectors
				foreach(KeyValuePair<Sector, VisualCeiling> group in ceilings) {
					foreach(Sidedef side in group.Key.Sidedefs) {
						if(side.Other == null || ceilings.ContainsKey(side.Other.Sector) || floors.ContainsKey(side.Other.Sector))
							continue;
						if(side.Other.Sector.CeilHeight > targetCeilingHeight && side.Other.Sector.CeilHeight < minSelectedHeight && side.Other.Sector.CeilHeight >= maxSelectedFloorHeight)
							targetCeilingHeight = side.Other.Sector.CeilHeight;
					}
				}
			}

			//CHECK VALUES:
			string alignFailDescription = string.Empty;

			if(floors.Count > 0 && targetFloorHeight == int.MinValue) 
				alignFailDescription = floors.Count > 1 ? "floors" : "floor";

			if(ceilings.Count > 0 && targetCeilingHeight == int.MinValue) {
				if(!string.IsNullOrEmpty(alignFailDescription))
					alignFailDescription += " and ";

				alignFailDescription += ceilings.Count > 1 ? "ceilings" : "ceiling";
			}

			if(!string.IsNullOrEmpty(alignFailDescription)) {
				General.Interface.DisplayStatus(StatusType.Warning, "Unable to align selected " + alignFailDescription + "!");
				return;
			}

			//APPLY VALUES:
			PreAction(UndoGroup.SectorHeightChange);

			//change floor height
			if(floors.Count > 0) {
				foreach(KeyValuePair<Sector, VisualFloor> group in floors) {
					if(targetFloorHeight != group.Key.FloorHeight)
						group.Value.OnChangeTargetHeight(targetFloorHeight - group.Key.FloorHeight);
				}
			}

			//change ceiling height
			if(ceilings.Count > 0) {
				foreach(KeyValuePair<Sector, VisualCeiling> group in ceilings) {
					if(targetCeilingHeight != group.Key.CeilHeight)
						group.Value.OnChangeTargetHeight(targetCeilingHeight - group.Key.CeilHeight);
				}
			}

			//process things. Just drop them to ground
			if(General.Map.FormatInterface.HasThingHeight){
				foreach(BaseVisualThing vt in things) {
					if(vt.Thing.Sector == null) continue;
					ThingTypeInfo ti = General.Map.Data.GetThingInfo(vt.Thing.Type);
					
					if(vt.Thing.Position.z != 0)
						vt.OnChangeTargetHeight((int)-vt.Thing.Position.z);
				}
			}

			PostAction();
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
			PreAction(UndoGroup.SectorBrightnessChange);
			List<IVisualEventReceiver> objs = GetSelectedObjects(true, true, false, false);
			foreach(IVisualEventReceiver i in objs) i.OnChangeTargetBrightness(true);
			PostAction();
		}

		[BeginAction("lowerbrightness8")]
		public void LowerBrightness8()
		{
			PreAction(UndoGroup.SectorBrightnessChange);
			List<IVisualEventReceiver> objs = GetSelectedObjects(true, true, false, false);
			foreach(IVisualEventReceiver i in objs) i.OnChangeTargetBrightness(false);
			PostAction();
		}

		[BeginAction("movetextureleft")]
		public void MoveTextureLeft1()
		{
            PreAction(UndoGroup.TextureOffsetChange);
			List<IVisualEventReceiver> objs = GetSelectedObjects(true, true, false, false);
            foreach (IVisualEventReceiver i in objs) i.OnChangeTextureOffset(-1, 0, true);
            PostAction();
		}

		[BeginAction("movetextureright")]
		public void MoveTextureRight1()
		{
            PreAction(UndoGroup.TextureOffsetChange);
			List<IVisualEventReceiver> objs = GetSelectedObjects(true, true, false, false);
            foreach (IVisualEventReceiver i in objs) i.OnChangeTextureOffset(1, 0, true);
            PostAction();
		}

		[BeginAction("movetextureup")]
		public void MoveTextureUp1()
		{
            PreAction(UndoGroup.TextureOffsetChange);
			List<IVisualEventReceiver> objs = GetSelectedObjects(true, true, false, false);
            foreach (IVisualEventReceiver i in objs) i.OnChangeTextureOffset(0, -1, true);
            PostAction();
		}

		[BeginAction("movetexturedown")]
		public void MoveTextureDown1()
		{
            PreAction(UndoGroup.TextureOffsetChange);
			List<IVisualEventReceiver> objs = GetSelectedObjects(true, true, false, false);
            foreach (IVisualEventReceiver i in objs) i.OnChangeTextureOffset(0, 1, true);
            PostAction();
		}

		[BeginAction("movetextureleft8")]
		public void MoveTextureLeft8()
		{
            PreAction(UndoGroup.TextureOffsetChange);
			List<IVisualEventReceiver> objs = GetSelectedObjects(true, true, false, false);
            foreach (IVisualEventReceiver i in objs) i.OnChangeTextureOffset(-8, 0, true);
            PostAction();
		}

		[BeginAction("movetextureright8")]
		public void MoveTextureRight8()
		{
            PreAction(UndoGroup.TextureOffsetChange);
			List<IVisualEventReceiver> objs = GetSelectedObjects(true, true, false, false);
            foreach (IVisualEventReceiver i in objs) i.OnChangeTextureOffset(8, 0, true);
            PostAction();
		}

		[BeginAction("movetextureup8")]
		public void MoveTextureUp8()
		{
            PreAction(UndoGroup.TextureOffsetChange);
			List<IVisualEventReceiver> objs = GetSelectedObjects(true, true, false, false);
            foreach (IVisualEventReceiver i in objs) i.OnChangeTextureOffset(0, -8, true);
            PostAction();
		}

		[BeginAction("movetexturedown8")]
		public void MoveTextureDown8()
		{
            PreAction(UndoGroup.TextureOffsetChange);
			List<IVisualEventReceiver> objs = GetSelectedObjects(true, true, false, false);
            foreach (IVisualEventReceiver i in objs) i.OnChangeTextureOffset(0, 8, true);
            PostAction();
		}

		[BeginAction("textureselect")]
		public void TextureSelect()
		{
			PreAction(UndoGroup.None);
			renderer.SetCrosshairBusy(true);
			General.Interface.RedrawDisplay();
			GetTargetEventReceiver(false).OnSelectTexture();
			UpdateChangedObjects();
			renderer.SetCrosshairBusy(false);
			PostAction();
		}

		[BeginAction("texturecopy")]
		public void TextureCopy()
		{
			PreActionNoChange();
			GetTargetEventReceiver(false).OnCopyTexture();
			PostAction();
		}

		[BeginAction("texturepaste")]
		public void TexturePaste()
		{
			PreAction(UndoGroup.None);
			List<IVisualEventReceiver> objs = GetSelectedObjects(true, true, false, false);
			foreach(IVisualEventReceiver i in objs) i.OnPasteTexture();
			PostAction();
		}

		//mxd
		[BeginAction("visualautoalign")]
		public void TextureAutoAlign() {
			PreAction(UndoGroup.None);
			renderer.SetCrosshairBusy(true);
			General.Interface.RedrawDisplay();
			GetTargetEventReceiver(false).OnTextureAlign(true, true);
			UpdateChangedObjects();
			renderer.SetCrosshairBusy(false);
			PostAction();
		}

		[BeginAction("visualautoalignx")]
		public void TextureAutoAlignX()
		{
			PreAction(UndoGroup.None);
			renderer.SetCrosshairBusy(true);
			General.Interface.RedrawDisplay();
			GetTargetEventReceiver(false).OnTextureAlign(true, false);
			UpdateChangedObjects();
			renderer.SetCrosshairBusy(false);
			PostAction();
		}

		[BeginAction("visualautoaligny")]
		public void TextureAutoAlignY()
		{
			PreAction(UndoGroup.None);
			renderer.SetCrosshairBusy(true);
			General.Interface.RedrawDisplay();
			GetTargetEventReceiver(false).OnTextureAlign(false, true);
			UpdateChangedObjects();
			renderer.SetCrosshairBusy(false);
			PostAction();
		}

		//mxd
		[BeginAction("visualfittextures")]
		public void TextureFit() {
			PreAction(UndoGroup.None);
			List<IVisualEventReceiver> objs = GetSelectedObjects(false, true, false, false);
			foreach(IVisualEventReceiver i in objs) i.OnTextureFit(true, true);
			PostAction();
		}

		//mxd
		[BeginAction("visualfittexturesx")]
		public void TextureFitX() {
			PreAction(UndoGroup.None);
			List<IVisualEventReceiver> objs = GetSelectedObjects(false, true, false, false);
			foreach(IVisualEventReceiver i in objs) i.OnTextureFit(true, false);
			PostAction();
		}

		//mxd
		[BeginAction("visualfittexturesy")]
		public void TextureFitY() {
			PreAction(UndoGroup.None);
			List<IVisualEventReceiver> objs = GetSelectedObjects(false, true, false, false);
			foreach(IVisualEventReceiver i in objs) i.OnTextureFit(false, true);
			PostAction();
		}

		[BeginAction("toggleupperunpegged")]
		public void ToggleUpperUnpegged()
		{
			PreAction(UndoGroup.None);
			GetTargetEventReceiver(false).OnToggleUpperUnpegged();
			PostAction();
		}

		[BeginAction("togglelowerunpegged")]
		public void ToggleLowerUnpegged()
		{
			PreAction(UndoGroup.None);
			GetTargetEventReceiver(false).OnToggleLowerUnpegged();
			PostAction();
		}

		[BeginAction("togglegravity")]
		public void ToggleGravity()
		{
			BuilderPlug.Me.UseGravity = !BuilderPlug.Me.UseGravity;
			string onoff = BuilderPlug.Me.UseGravity ? "ON" : "OFF";
			General.Interface.DisplayStatus(StatusType.Action, "Gravity is now " + onoff + ".");
		}

		[BeginAction("togglebrightness")]
		public void ToggleBrightness()
		{
			renderer.FullBrightness = !renderer.FullBrightness;
			string onoff = renderer.FullBrightness ? "ON" : "OFF";
			General.Interface.DisplayStatus(StatusType.Action, "Full Brightness is now " + onoff + ".");
		}

		[BeginAction("togglehighlight")]
		public void ToggleHighlight()
		{
			BuilderPlug.Me.UseHighlight = !BuilderPlug.Me.UseHighlight;
			string onoff = BuilderPlug.Me.UseHighlight ? "ON" : "OFF";
			General.Interface.DisplayStatus(StatusType.Action, "Highlight is now " + onoff + ".");
		}

		[BeginAction("resettexture")]
		public void ResetTexture()
		{
			PreAction(UndoGroup.None);
			List<IVisualEventReceiver> objs = GetSelectedObjects(true, true, false, false);
			foreach(IVisualEventReceiver i in objs) i.OnResetTextureOffset();
			PostAction();
		}

		[BeginAction("floodfilltextures")]
		public void FloodfillTextures()
		{
			PreAction(UndoGroup.None);
			GetTargetEventReceiver(false).OnTextureFloodfill();
			PostAction();
		}

		[BeginAction("texturecopyoffsets")]
		public void TextureCopyOffsets()
		{
			PreActionNoChange();
			GetTargetEventReceiver(false).OnCopyTextureOffsets();
			PostAction();
		}

		[BeginAction("texturepasteoffsets")]
		public void TexturePasteOffsets()
		{
			PreAction(UndoGroup.None);
			List<IVisualEventReceiver> objs = GetSelectedObjects(true, true, false, false);
			foreach(IVisualEventReceiver i in objs) i.OnPasteTextureOffsets();
			PostAction();
		}

		[BeginAction("copyproperties")]
		public void CopyProperties()
		{
			PreActionNoChange();
			GetTargetEventReceiver(false).OnCopyProperties();
			PostAction();
		}

		[BeginAction("pasteproperties")]
		public void PasteProperties()
		{
			PreAction(UndoGroup.None);
			List<IVisualEventReceiver> objs = GetSelectedObjects(true, true, true, true);
			foreach(IVisualEventReceiver i in objs) i.OnPasteProperties();
			PostAction();
		}

        //mxd. now we can insert things in Visual modes
        [BeginAction("insertitem", BaseAction = true)] 
		public void InsertThing()
		{
            Vector2D hitpos = getHitPosition();

            if (!hitpos.IsFinite()) {
                General.Interface.DisplayStatus(StatusType.Warning, "Cannot insert thing here!");
                return;
            }
            
            ClearSelection();
            PreActionNoChange();
            General.Map.UndoRedo.ClearAllRedos();
            General.Map.UndoRedo.CreateUndo("Insert thing");

            Thing t = CreateThing(new Vector2D(hitpos.x, hitpos.y));

            if (t == null) {
                General.Map.UndoRedo.WithdrawUndo();
                return;
            }

            // Edit the thing?
            if (BuilderPlug.Me.EditNewThing)
                General.Interface.ShowEditThings(new List<Thing> { t });

            //add thing to blockmap
            blockmap.AddThing(t);

            General.Interface.DisplayStatus(StatusType.Action, "Inserted a new thing.");
            PostAction();
		}

        //mxd
		[BeginAction("deleteitem", BaseAction = true)]
        public void DeleteSelectedObjects()
		{
			deleteSelectedThings();
			deleteSelectedVertices();
		}

        //mxd
        [BeginAction("copyselection", BaseAction = true)]
        public void CopySelection() {
            List<IVisualEventReceiver> objs = GetSelectedObjects(false, false, true, false);
            if (objs.Count == 0) {
                General.Interface.DisplayStatus(StatusType.Warning, "Nothing to copy, select some Things first!");
                return;
            }

            copyBuffer.Clear();
            foreach (IVisualEventReceiver i in objs) {
                VisualThing vt = i as VisualThing;
                if (vt != null) copyBuffer.Add(new ThingCopyData(vt.Thing));
            }
            General.Interface.DisplayStatus(StatusType.Info, "Copied " + copyBuffer.Count + " Things");
        }

        //mxd
        [BeginAction("cutselection", BaseAction = true)]
        public void CutSelection() {
            CopySelection();
            deleteSelectedThings();
        }

        //mxd. We'll just use currently selected objects 
        [BeginAction("pasteselection", BaseAction = true)]
        public void PasteSelection() {
            if(copyBuffer.Count == 0){
                General.Interface.DisplayStatus(StatusType.Warning, "Nothing to paste, cut or copy some Things first!");
                return;
            }
            
            Vector2D hitpos = getHitPosition();

            if (!hitpos.IsFinite()) {
                General.Interface.DisplayStatus(StatusType.Warning, "Cannot paste here!");
                return;
            }

            General.Map.UndoRedo.ClearAllRedos();
            string rest = copyBuffer.Count + " thing" + (copyBuffer.Count > 1 ? "s." : ".");
            General.Map.UndoRedo.CreateUndo("Paste " + rest);
            General.Interface.DisplayStatus(StatusType.Info, "Pasted " + rest);
            
            PreActionNoChange();
            ClearSelection();

            //get translated positions
            Vector3D[] coords = new Vector3D[copyBuffer.Count];
            for (int i = 0; i < copyBuffer.Count; i++ )
                coords[i] = copyBuffer[i].Position;

            Vector3D[] translatedCoords = translateCoordinates(coords, hitpos, true);

            //create things from copyBuffer
            for (int i = 0; i < copyBuffer.Count; i++) {
                Thing t = CreateThing(new Vector2D());
                if (t != null) {
                    copyBuffer[i].ApplyTo(t);
                    t.Move(translatedCoords[i]);
                    //add thing to blockmap
                    blockmap.AddThing(t);
                }
            }
            PostAction();
        }

        //mxd. rotate clockwise
        [BeginAction("rotatethingscw")]
        public void RotateThingsCW() {
            List<VisualThing> things = GetSelectedVisualThings(true);

            PreAction(UndoGroup.ThingRotate);

            if (things.Count == 0) {
				General.Interface.DisplayStatus(StatusType.Warning, "This action requires selected Things!");
                return;
            }

            foreach (VisualThing t in things)
                ((BaseVisualThing)t).OnRotate(General.ClampAngle(t.Thing.AngleDoom + 5));

            PostAction();
        }

        //mxd. rotate counterclockwise
        [BeginAction("rotatethingsccw")]
        public void RotateThingsCCW() {
            List<VisualThing> things = GetSelectedVisualThings(true);

            PreAction(UndoGroup.ThingRotate);

            if (things.Count == 0) {
				General.Interface.DisplayStatus(StatusType.Warning, "This action requires selected Things!");
                return;
            }

            foreach (VisualThing t in things)
                ((BaseVisualThing)t).OnRotate(General.ClampAngle(t.Thing.AngleDoom - 5));

            PostAction();
        }

        //mxd
        [BeginAction("togglegzdoomrenderingeffects")]
        public void ToggleGZDoomRenderingEffects() {
            gzdoomRenderingEffects = !gzdoomRenderingEffects;
            RebuildElementData();
            UpdateChangedObjects();
            General.Interface.DisplayStatus(StatusType.Info, "(G)ZDoom rendering effects are " + (gzdoomRenderingEffects ? "ENABLED" : "DISABLED"));
        }

		//mxd
		[BeginAction("thingaligntowall")]
		public void AlignThingsToWall() {
			List<VisualThing> visualThings = GetSelectedVisualThings(true);

			if(visualThings.Count == 0) {
				General.Interface.DisplayStatus(StatusType.Warning, "This action requires selected Things!");
				return;
			}

			List<Thing> things = new List<Thing>();

			foreach(VisualThing vt in visualThings)
				things.Add(vt.Thing);

			// Make undo
			if(things.Count > 1) {
				General.Map.UndoRedo.CreateUndo("Align " + things.Count + " things");
				General.Interface.DisplayStatus(StatusType.Action, "Aligned " + things.Count + " things.");
			} else {
				General.Map.UndoRedo.CreateUndo("Align thing");
				General.Interface.DisplayStatus(StatusType.Action, "Aligned a thing.");
			}

			//align things
			int thingsCount = General.Map.Map.Things.Count;

			foreach(Thing t in things) {
				List<Linedef> excludedLines = new List<Linedef>();
				bool aligned = false;

				do {
					Linedef l = General.Map.Map.NearestLinedef(t.Position, excludedLines);
					aligned = Tools.TryAlignThingToLine(t, l);

					if(!aligned) {
						excludedLines.Add(l);

						if(excludedLines.Count == thingsCount) {
							ThingTypeInfo tti = General.Map.Data.GetThingInfo(t.Type);
							General.ErrorLogger.Add(ErrorType.Warning, "Unable to align Thing ¹" + t.Index + " (" + tti.Title + ") to any linedef in a map!");
							aligned = true;
						}
					}
				} while(!aligned);
			}

			//apply changes to Visual Things
			for(int i = 0; i < visualThings.Count; i++) {
				BaseVisualThing t = visualThings[i] as BaseVisualThing;
				t.Changed = true;

				// Update what must be updated
				ThingData td = GetThingData(t.Thing);
				foreach(KeyValuePair<Sector, bool> s in td.UpdateAlso) {
					if(VisualSectorExists(s.Key)) {
						BaseVisualSector vs = (BaseVisualSector)GetVisualSector(s.Key);
						vs.UpdateSectorGeometry(s.Value);
					}
				}
			}

			UpdateChangedObjects();
			ShowTargetInfo();
		}

		//mxd
		[BeginAction("toggleslope")]
		public void ToggleSlope() {
			List<VisualGeometry> selection = GetSelectedSurfaces();

			if(selection.Count == 0) {
				General.Interface.DisplayStatus(StatusType.Warning, "Toggle Slope action requires selected surfaces!");
				return;
			}

			bool update = false;
			List<BaseVisualSector> toUpdate = new List<BaseVisualSector>();
			General.Map.UndoRedo.CreateUndo("Toggle Slope");

			//check selection
			foreach(VisualGeometry vg in selection) {
				update = false;

				//assign/remove action
				if(vg.GeometryType == VisualGeometryType.WALL_BOTTOM) {
					if(vg.Sidedef.Line.Action == 0 || (vg.Sidedef.Line.Action == 181 && vg.Sidedef.Line.Args[0] == 0)) {
						//check if the sector already has floor slopes
						foreach(Sidedef side in vg.Sidedef.Sector.Sidedefs) {
							if(side == vg.Sidedef || side.Line.Action != 181) continue;

							int arg = (side == side.Line.Front ? 1 : 2);

							if(side.Line.Args[0] == arg) {
								//if only floor is affected, remove action
								if(side.Line.Args[1] == 0)
									side.Line.Action = 0;
								else //clear floor alignment
									side.Line.Args[0] = 0;
							}
						}

						//set action
						vg.Sidedef.Line.Action = 181;
						vg.Sidedef.Line.Args[0] = (vg.Sidedef == vg.Sidedef.Line.Front ? 1 : 2);
						update = true;
					}
				} else if(vg.GeometryType == VisualGeometryType.WALL_UPPER) {
					if(vg.Sidedef.Line.Action == 0 || (vg.Sidedef.Line.Action == 181 && vg.Sidedef.Line.Args[1] == 0)) {
						//check if the sector already has ceiling slopes
						foreach(Sidedef side in vg.Sidedef.Sector.Sidedefs) {
							if(side == vg.Sidedef || side.Line.Action != 181) continue;

							int arg = (side == side.Line.Front ? 1 : 2);

							if(side.Line.Args[1] == arg) {
								//if only ceiling is affected, remove action
								if(side.Line.Args[0] == 0)
									side.Line.Action = 0;
								else //clear ceiling alignment
									side.Line.Args[1] = 0;
							}
						}

						//set action
						vg.Sidedef.Line.Action = 181;
						vg.Sidedef.Line.Args[1] = (vg.Sidedef == vg.Sidedef.Line.Front ? 1 : 2);
						update = true;
					}
				} else if(vg.GeometryType == VisualGeometryType.CEILING) {
					//check if the sector has ceiling slopes
					foreach(Sidedef side in vg.Sector.Sector.Sidedefs) {
						if(side.Line.Action != 181)	continue;

						int arg = (side == side.Line.Front ? 1 : 2);

						if(side.Line.Args[1] == arg) {
							//if only ceiling is affected, remove action
							if(side.Line.Args[0] == 0)
								side.Line.Action = 0;
							else //clear ceiling alignment
								side.Line.Args[1] = 0;

							update = true;
						}
					}
				} else if(vg.GeometryType == VisualGeometryType.FLOOR) {
					//check if the sector has floor slopes
					foreach(Sidedef side in vg.Sector.Sector.Sidedefs) {
						if(side.Line.Action != 181)	continue;

						int arg = (side == side.Line.Front ? 1 : 2);

						if(side.Line.Args[0] == arg) {
							//if only floor is affected, remove action
							if(side.Line.Args[1] == 0)
								side.Line.Action = 0;
							else //clear floor alignment
								side.Line.Args[0] = 0;

							update = true;
						}
					}
				}

				//add to update list
				if(update) toUpdate.Add(vg.Sector as BaseVisualSector);
			}

			//update changed geometry
			if(toUpdate.Count > 0) {
				RebuildElementData();

				foreach(BaseVisualSector vs in toUpdate)
					vs.UpdateSectorGeometry(true);

				UpdateChangedObjects();
				ClearSelection();
				ShowTargetInfo();
			}

			General.Interface.DisplayStatus(StatusType.Action, "Toggled Slope for " + toUpdate.Count + (toUpdate.Count == 1 ? " surface." : " surfaces."));
		}
		
		#endregion

		#region ================== Texture Alignment

		//mxd
		public void AutoAlignTextures(Sidedef start, SidedefPart part, ImageData texture, bool alignx, bool aligny, bool resetsidemarks) {
			if(General.Map.UDMF)
				autoAlignTextures(start, part, texture, alignx, aligny, resetsidemarks);
			else
				autoAlignTextures(start, texture, alignx, aligny, resetsidemarks);
		}

		//mxd. Moved here from Tools
		// This performs texture alignment along all walls that match with the same texture
		// NOTE: This method uses the sidedefs marking to indicate which sides have been aligned
		// When resetsidemarks is set to true, all sidedefs will first be marked false (not aligned).
		// Setting resetsidemarks to false is usefull to align only within a specific selection
		// (set the marked property to true for the sidedefs outside the selection)
		private void autoAlignTextures(Sidedef start, ImageData texture, bool alignx, bool aligny, bool resetsidemarks) {
			Stack<SidedefAlignJob> todo = new Stack<SidedefAlignJob>(50);
			float scalex = (General.Map.Config.ScaledTextureOffsets && !texture.WorldPanning) ? texture.Scale.x : 1.0f;
			float scaley = (General.Map.Config.ScaledTextureOffsets && !texture.WorldPanning) ? texture.Scale.y : 1.0f;

			// Mark all sidedefs false (they will be marked true when the texture is aligned)
			if(resetsidemarks)
				General.Map.Map.ClearMarkedSidedefs(false);

			// Begin with first sidedef
			SidedefAlignJob first = new SidedefAlignJob();
			first.sidedef = start;
			first.offsetx = start.OffsetX;

			//mxd. 3D floors alignment
			if(!start.LowRequired() && !start.HighRequired()) {
				List<Sidedef> controlSides = getControlSides(start, false);
				foreach(Sidedef s in controlSides) {
					if((s.LongMiddleTexture == texture.LongName) && (s.MiddleRequired() || ((s.MiddleTexture.Length > 0) && (s.MiddleTexture[0] != '-')))) {
						first.controlSide = s;
						break;
					}
				}
			}

			if(first.controlSide == null) first.controlSide = start;

			first.forward = true;
			todo.Push(first);

			// Continue until nothing more to align
			while(todo.Count > 0) {
				// Get the align job to do
				SidedefAlignJob j = todo.Pop();

				if(j.forward) {
					Vertex v;
					int forwardoffset;
					int backwardoffset;

					// Apply alignment
					if(alignx)
						j.sidedef.OffsetX = (int)j.offsetx;
					if(aligny)
						j.sidedef.OffsetY = (int)Math.Round((start.Sector.CeilHeight - j.controlSide.Sector.CeilHeight) / scaley) + start.OffsetY;
					forwardoffset = (int)j.offsetx + (int)Math.Round(j.sidedef.Line.Length / scalex);
					backwardoffset = (int)j.offsetx;

					j.sidedef.Marked = true;

					// Wrap the value within the width of the texture (to prevent ridiculous values)
					// NOTE: We don't use ScaledWidth here because the texture offset is in pixels, not mappixels
					if(texture.IsImageLoaded) {
						if(alignx)
							j.sidedef.OffsetX %= texture.Width;
						if(aligny)
							j.sidedef.OffsetY %= texture.Height;
					}

					// Add sidedefs forward (connected to the right vertex)
					v = j.sidedef.IsFront ? j.sidedef.Line.End : j.sidedef.Line.Start;
					AddSidedefsForAlignment(todo, v, true, forwardoffset, 1.0f, texture.LongName, false);

					// Add sidedefs backward (connected to the left vertex)
					v = j.sidedef.IsFront ? j.sidedef.Line.Start : j.sidedef.Line.End;
					AddSidedefsForAlignment(todo, v, false, backwardoffset, 1.0f, texture.LongName, false);
				} else {
					Vertex v;
					int forwardoffset;
					int backwardoffset;

					// Apply alignment
					if(alignx)
						j.sidedef.OffsetX = (int)j.offsetx - (int)Math.Round(j.sidedef.Line.Length / scalex);
					if(aligny)
						j.sidedef.OffsetY = (int)Math.Round((start.Sector.CeilHeight - j.controlSide.Sector.CeilHeight) / scaley) + start.OffsetY;
					forwardoffset = (int)j.offsetx;
					backwardoffset = (int)j.offsetx - (int)Math.Round(j.sidedef.Line.Length / scalex);

					j.sidedef.Marked = true;

					// Wrap the value within the width of the texture (to prevent ridiculous values)
					// NOTE: We don't use ScaledWidth here because the texture offset is in pixels, not mappixels
					if(texture.IsImageLoaded) {
						if(alignx)
							j.sidedef.OffsetX %= texture.Width;
						if(aligny)
							j.sidedef.OffsetY %= texture.Height;
					}

					// Add sidedefs backward (connected to the left vertex)
					v = j.sidedef.IsFront ? j.sidedef.Line.Start : j.sidedef.Line.End;
					AddSidedefsForAlignment(todo, v, false, backwardoffset, 1.0f, texture.LongName, false);

					// Add sidedefs forward (connected to the right vertex)
					v = j.sidedef.IsFront ? j.sidedef.Line.End : j.sidedef.Line.Start;
					AddSidedefsForAlignment(todo, v, true, forwardoffset, 1.0f, texture.LongName, false);
				}
			}
		}

		//mxd. Moved here from GZDoomEditing plugin
		// This performs UDMF texture alignment along all walls that match with the same texture
		// NOTE: This method uses the sidedefs marking to indicate which sides have been aligned
		// When resetsidemarks is set to true, all sidedefs will first be marked false (not aligned).
		// Setting resetsidemarks to false is usefull to align only within a specific selection
		// (set the marked property to true for the sidedefs outside the selection)
		private void autoAlignTextures(Sidedef start, SidedefPart part, ImageData texture, bool alignx, bool aligny, bool resetsidemarks) {
			Stack<SidedefAlignJob> todo = new Stack<SidedefAlignJob>(50);
			float scalex = (General.Map.Config.ScaledTextureOffsets && !texture.WorldPanning) ? texture.Scale.x : 1.0f;
			float scaley = (General.Map.Config.ScaledTextureOffsets && !texture.WorldPanning) ? texture.Scale.y : 1.0f;

			// Mark all sidedefs false (they will be marked true when the texture is aligned)
			if(resetsidemarks)
				General.Map.Map.ClearMarkedSidedefs(false);

			if(!texture.IsImageLoaded)
				return;

			// Determine the Y alignment
			float ystartalign = start.OffsetY;
			switch(part) {
				case SidedefPart.Upper:
					ystartalign += GetTopOffsetY(start, start.Fields.GetValue("offsety_top", 0.0f), false);//mxd
					break;
				case SidedefPart.Middle:
					ystartalign += GetMiddleOffsetY(start, start.Fields.GetValue("offsety_mid", 0.0f), false);//mxd
					break; 
				case SidedefPart.Lower:
					ystartalign += GetBottomOffsetY(start, start.Fields.GetValue("offsety_bottom", 0.0f), false);//mxd
					break;
			}

			// Begin with first sidedef
			SidedefAlignJob first = new SidedefAlignJob();
			first.sidedef = start;
			first.offsetx = start.OffsetX;
			switch(part) {
				case SidedefPart.Upper:
					first.offsetx += start.Fields.GetValue("offsetx_top", 0.0f);
					break;
				case SidedefPart.Middle:
					first.offsetx += start.Fields.GetValue("offsetx_mid", 0.0f);
					break;
				case SidedefPart.Lower:
					first.offsetx += start.Fields.GetValue("offsetx_bottom", 0.0f);
					break;
			}
			first.forward = true;

			//mxd. 3D floors alignment
			if(part == SidedefPart.Middle) {
				List<Sidedef> controlSides = getControlSides(start, true); //mxd
				
				foreach(Sidedef s in controlSides) {
					if((s.LongMiddleTexture == texture.LongName) && (s.MiddleRequired() || ((s.MiddleTexture.Length > 0) && (s.MiddleTexture[0] != '-')))) {
						first.controlSide = s;
						break;
					}
				}
			} else {
				first.controlSide = start;
			}

			//mxd. scaleY
			switch(part) {
				case SidedefPart.Upper:
					first.scaleY = start.Fields.GetValue("scaley_top", 1.0f);
					break;
				case SidedefPart.Middle:
					first.scaleY = start.Fields.GetValue("scaley_mid", 1.0f);
					break;
				case SidedefPart.Lower:
					first.scaleY = start.Fields.GetValue("scaley_bottom", 1.0f);
					break;
			}

			todo.Push(first);

			// Continue until nothing more to align
			while(todo.Count > 0) {
				Vertex v;
				float forwardoffset;
				float backwardoffset;
				float offsetscalex = 1.0f;

				// Get the align job to do
				SidedefAlignJob j = todo.Pop();

				bool matchtop = ((j.sidedef.LongHighTexture == texture.LongName) && j.sidedef.HighRequired());
				bool matchbottom = ((j.sidedef.LongLowTexture == texture.LongName) && j.sidedef.LowRequired());
				bool matchmid = ((j.controlSide.LongMiddleTexture == texture.LongName) && (j.controlSide.MiddleRequired() || ((j.controlSide.MiddleTexture.Length > 0) && (j.controlSide.MiddleTexture[0] != '-')))); //mxd

				if(matchtop)
					offsetscalex = j.sidedef.Fields.GetValue("scalex_top", 1.0f);
				else if(matchbottom)
					offsetscalex = j.sidedef.Fields.GetValue("scalex_bottom", 1.0f);
				else if(matchmid)
					offsetscalex = j.sidedef.Fields.GetValue("scalex_mid", 1.0f);

				//mxd. Apply scaleY
				j.sidedef.Fields.BeforeFieldsChange();
				if(j.scaleY == 1.0f) {
					if(matchtop && j.sidedef.Fields.GetValue("scaley_top", 1.0f) != 1.0f)
						j.sidedef.Fields.Remove("scaley_top");
					else if(matchmid && j.sidedef.Fields.GetValue("scaley_mid", 1.0f) != 1.0f)
						j.sidedef.Fields.Remove("scaley_mid");
					else if(matchbottom && j.sidedef.Fields.GetValue("scaley_bottom", 1.0f) != 1.0f)
						j.sidedef.Fields.Remove("scaley_bottom");
				} else {
					if(matchtop && j.sidedef.Fields.GetValue("scaley_top", 1.0f) != j.scaleY)
						j.sidedef.Fields["scaley_top"] = new UniValue(UniversalType.Float, j.scaleY);
					if(matchmid && j.sidedef.Fields.GetValue("scaley_mid", 1.0f) != j.scaleY)
						j.sidedef.Fields["scaley_mid"] = new UniValue(UniversalType.Float, j.scaleY);
					if(matchbottom && j.sidedef.Fields.GetValue("scaley_bottom", 1.0f) != j.scaleY)
						j.sidedef.Fields["scaley_bottom"] = new UniValue(UniversalType.Float, j.scaleY);
				}

				if(j.forward) {
					// Apply alignment
					if(alignx) {
						float offset = j.offsetx;
						offset %= (float)texture.Width;//mxd
						offset -= j.sidedef.OffsetX;

						j.sidedef.Fields.BeforeFieldsChange();
						if(matchtop)
							j.sidedef.Fields["offsetx_top"] = new UniValue(UniversalType.Float, offset);
						if(matchbottom)
							j.sidedef.Fields["offsetx_bottom"] = new UniValue(UniversalType.Float, offset);
						if(matchmid)
							j.sidedef.Fields["offsetx_mid"] = new UniValue(UniversalType.Float, offset);
					}
					if(aligny) {
						float offset = ((float)(start.Sector.CeilHeight - j.sidedef.Sector.CeilHeight) / scaley) + ystartalign;
						offset -= j.sidedef.OffsetY;

						j.sidedef.Fields.BeforeFieldsChange();
						if(matchtop)
							j.sidedef.Fields["offsety_top"] = new UniValue(UniversalType.Float, GetTopOffsetY(j.sidedef, offset, true) % (float)texture.Height); //mxd
						if(matchbottom)
							j.sidedef.Fields["offsety_bottom"] = new UniValue(UniversalType.Float, GetBottomOffsetY(j.sidedef, offset, true) % (float)texture.Height); //mxd
						if(matchmid) {
							//mxd. Side is part of a 3D floor?
							if(j.sidedef.Index != j.controlSide.Index) {
								offset = ((float)(start.Sector.CeilHeight - j.controlSide.Sector.CeilHeight) / scaley) + ystartalign;
								offset -= j.sidedef.OffsetY;
							}
							j.sidedef.Fields["offsety_mid"] = new UniValue(UniversalType.Float, GetMiddleOffsetY(j.sidedef, offset, true) % (float)texture.Height);//mxd
						}
					}
					forwardoffset = j.offsetx + (int)Math.Round(j.sidedef.Line.Length / scalex * offsetscalex);
					backwardoffset = j.offsetx;

					// Done this sidedef
					j.sidedef.Marked = true;

					// Add sidedefs backward (connected to the left vertex)
					v = j.sidedef.IsFront ? j.sidedef.Line.Start : j.sidedef.Line.End;
					AddSidedefsForAlignment(todo, v, false, backwardoffset, j.scaleY, texture.LongName, true);

					// Add sidedefs forward (connected to the right vertex)
					v = j.sidedef.IsFront ? j.sidedef.Line.End : j.sidedef.Line.Start;
					AddSidedefsForAlignment(todo, v, true, forwardoffset, j.scaleY, texture.LongName, true);
				} else {
					// Apply alignment
					if(alignx) {
						float offset = j.offsetx - (int)Math.Round(j.sidedef.Line.Length / scalex);
						offset %= (float)texture.Width; //mxd
						offset -= j.sidedef.OffsetX;

						j.sidedef.Fields.BeforeFieldsChange();
						if(matchtop)
							j.sidedef.Fields["offsetx_top"] = new UniValue(UniversalType.Float, offset);
						if(matchbottom)
							j.sidedef.Fields["offsetx_bottom"] = new UniValue(UniversalType.Float, offset);
						if(matchmid)
							j.sidedef.Fields["offsetx_mid"] = new UniValue(UniversalType.Float, offset);
					}
					if(aligny) {
						float offset = ((float)(start.Sector.CeilHeight - j.sidedef.Sector.CeilHeight) / scaley) + ystartalign;
						offset -= j.sidedef.OffsetY;

						j.sidedef.Fields.BeforeFieldsChange();
						if(matchtop)
							j.sidedef.Fields["offsety_top"] = new UniValue(UniversalType.Float, GetTopOffsetY(j.sidedef, offset, true) % (float)texture.Height); //mxd
						if(matchbottom)
							j.sidedef.Fields["offsety_bottom"] = new UniValue(UniversalType.Float, GetBottomOffsetY(j.sidedef, offset, true) % (float)texture.Height); //mxd
						if(matchmid) {
							//mxd. Side is part of a 3D floor?
							if(j.sidedef.Index != j.controlSide.Index) {
								offset = ((float)(start.Sector.CeilHeight - j.controlSide.Sector.CeilHeight) / scaley) + ystartalign;
								offset -= j.sidedef.OffsetY;
							}
							j.sidedef.Fields["offsety_mid"] = new UniValue(UniversalType.Float, GetMiddleOffsetY(j.sidedef, offset, true) % (float)texture.Height); //mxd
						}
					}
					forwardoffset = j.offsetx;
					backwardoffset = j.offsetx - (int)Math.Round(j.sidedef.Line.Length / scalex * offsetscalex);

					// Done this sidedef
					j.sidedef.Marked = true;

					// Add sidedefs forward (connected to the right vertex)
					v = j.sidedef.IsFront ? j.sidedef.Line.End : j.sidedef.Line.Start;
					AddSidedefsForAlignment(todo, v, true, forwardoffset, j.scaleY, texture.LongName, true);

					// Add sidedefs backward (connected to the left vertex)
					v = j.sidedef.IsFront ? j.sidedef.Line.Start : j.sidedef.Line.End;
					AddSidedefsForAlignment(todo, v, false, backwardoffset, j.scaleY, texture.LongName, true);
				}
			}
		}

		// This adds the matching, unmarked sidedefs from a vertex for texture alignment
		private void AddSidedefsForAlignment(Stack<SidedefAlignJob> stack, Vertex v, bool forward, float offsetx, float scaleY, long texturelongname, bool udmf) {
			foreach(Linedef ld in v.Linedefs) {
				Sidedef side1 = forward ? ld.Front : ld.Back;
				Sidedef side2 = forward ? ld.Back : ld.Front;
				if((ld.Start == v) && (side1 != null) && !side1.Marked) {
					List<Sidedef> controlSides = getControlSides(side1, udmf);//mxd

					foreach(Sidedef s in controlSides) {
						if(Tools.SidedefTextureMatch(s, texturelongname)) {
							SidedefAlignJob nj = new SidedefAlignJob();
							nj.forward = forward;
							nj.offsetx = offsetx;
							nj.scaleY = scaleY; //mxd
							nj.sidedef = side1;
							nj.controlSide = s; //mxd
							stack.Push(nj);
							break;
						}
					}
				} else if((ld.End == v) && (side2 != null) && !side2.Marked) {
					List<Sidedef> controlSides = getControlSides(side2, udmf);//mxd

					foreach(Sidedef s in controlSides) {
						if(Tools.SidedefTextureMatch(s, texturelongname)) {
							SidedefAlignJob nj = new SidedefAlignJob();
							nj.forward = forward;
							nj.offsetx = offsetx;
							nj.scaleY = scaleY; //mxd
							nj.sidedef = side2;
							nj.controlSide = s; //mxd
							stack.Push(nj);
							break;
						}
					}
				}
			}
		}

		//mxd. This converts offsetY from/to "normalized" offset for given upper wall
		internal float GetTopOffsetY(Sidedef side, float offset, bool fromNormalized) {
			if(side.Line.IsFlagSet(General.Map.Config.UpperUnpeggedFlag) || side.Other == null || side.Other.Sector == null)
				return offset;

			//if we don't have UpperUnpegged flag, normalize offset
			float scale = side.Fields.GetValue("scaley_top", 1.0f);
			float surfaceHeight = (side.Sector.CeilHeight - side.Other.Sector.CeilHeight) * scale;

			if(fromNormalized) return (float)Math.Round(offset + surfaceHeight);
			return (float)Math.Round(offset - surfaceHeight);
		}

		//mxd. This converts offsetY from/to "normalized" offset for given middle wall
		internal float GetMiddleOffsetY(Sidedef side, float offset, bool fromNormalized) {
			if(!side.Line.IsFlagSet(General.Map.Config.LowerUnpeggedFlag) || side.Sector == null)
				return offset;

			//if we have LowerUnpegged flag, normalize offset
			float scale = side.Fields.GetValue("scaley_mid", 1.0f);
			float surfaceHeight = (side.Sector.CeilHeight - side.Sector.FloorHeight) * scale;
			
			if(fromNormalized) return (float)Math.Round(offset + surfaceHeight);
			return (float)Math.Round(offset - surfaceHeight);
		}

		//mxd. This converts offsetY from/to "normalized" offset for given lower wall
		internal float GetBottomOffsetY(Sidedef side, float offset, bool fromNormalized) {
			if(side.Line.IsFlagSet(General.Map.Config.LowerUnpeggedFlag) || side.Other == null || side.Other.Sector == null)
				return offset;

			//normalize offset
			float scale = side.Fields.GetValue("scaley_bottom", 1.0f);
			float surfaceHeight = (side.Sector.CeilHeight - side.Other.Sector.FloorHeight) * scale;
			
			if(fromNormalized) return (float)Math.Round(offset + surfaceHeight);
			return (float)Math.Round(offset - surfaceHeight);
		}

		//mxd
		private List<Sidedef> getControlSides(Sidedef side, bool udmf) {
			if(side.Other == null) return new List<Sidedef>() { side };
			if(side.Other.Sector.Tag == 0) return new List<Sidedef>() { side };

			SectorData data = GetSectorData(side.Other.Sector);
			if(data.ExtraFloors.Count == 0)	return new List<Sidedef>() { side };

			List<Sidedef> sides = new List<Sidedef>();
			foreach(Effect3DFloor ef in data.ExtraFloors)
				sides.Add(ef.Linedef.Front);

			if(udmf)
				sides.Add(side); //UDMF map format
			else
				sides.Insert(0, side); //Doom/Hexen map format: if a sidedef has lower/upper parts, they take predecence in alignment

			return sides;
		}

		#endregion
	}
}
