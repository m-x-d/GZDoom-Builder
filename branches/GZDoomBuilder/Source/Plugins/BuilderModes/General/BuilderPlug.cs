
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
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using CodeImp.DoomBuilder.Actions;
using CodeImp.DoomBuilder.BuilderModes.Interface;
using CodeImp.DoomBuilder.BuilderModes.IO;
using CodeImp.DoomBuilder.Config;
using CodeImp.DoomBuilder.Controls;
using CodeImp.DoomBuilder.Data;
using CodeImp.DoomBuilder.Editing;
using CodeImp.DoomBuilder.Geometry;
using CodeImp.DoomBuilder.GZBuilder.Geometry;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Plugins;
using CodeImp.DoomBuilder.Rendering;
using CodeImp.DoomBuilder.Types;
using CodeImp.DoomBuilder.Windows;

#endregion

namespace CodeImp.DoomBuilder.BuilderModes
{
	public class BuilderPlug : Plug
	{
		#region ================== API Declarations

		[DllImport("user32.dll")]
		internal static extern int GetWindowLong(IntPtr hWnd, int nIndex);
		
		#endregion
		
		#region ================== Constants

		internal const int WS_HSCROLL = 0x100000;
		internal const int WS_VSCROLL = 0x200000;
		internal const int GWL_STYLE = -16;
		
		#endregion

		#region ================== Variables

		// Static instance
		private static BuilderPlug me;
		
		// Main objects
		private MenusForm menusform;
		private CurveLinedefsForm curvelinedefsform;
		private FindReplaceForm findreplaceform;
		private ErrorCheckForm errorcheckform;
		private PreferencesForm preferencesform;
		
		// Dockers
		private UndoRedoPanel undoredopanel;
		private Docker undoredodocker;
		private SectorDrawingOptionsPanel drawingOverridesPanel; //mxd
		private Docker drawingOverridesDocker; //mxd
		
		// Settings
		private int showvisualthings;			// 0 = none, 1 = sprite only, 2 = sprite caged
		private bool usegravity;
		private int changeheightbysidedef;		// 0 = nothing, 1 = change ceiling, 2 = change floor
		private bool editnewthing;
		private bool editnewsector;
		private bool additiveselect;
		private bool autoclearselection;
		private bool visualmodeclearselection;
		private string copiedtexture;
		private string copiedflat;
		private Point copiedoffsets;
		private VertexProperties copiedvertexprops;
		private SectorProperties copiedsectorprops;
		private SidedefProperties copiedsidedefprops;
		private LinedefProperties copiedlinedefprops;
		private ThingProperties copiedthingprops;
		private bool viewselectionnumbers;
		private bool viewselectioneffects; //mxd
		private float stitchrange;
		private float highlightrange;
		private float highlightthingsrange;
		private float splitlinedefsrange;
		private bool usehighlight;
		private bool autodragonpaste;
		private bool autoAlignTextureOffsetsOnCreate;//mxd
		private bool dontMoveGeometryOutsideMapBoundary;//mxd
		private bool autoDrawOnEdit; //mxd
		private bool marqueSelectTouching; //mxd. Select elements partially/fully inside of marque selection?
		private bool syncSelection; //mxd. Sync selection between Visual and Classic modes.
		private bool lockSectorTextureOffsetsWhileDragging; //mxd
		private bool dragThingsInSectorsMode; //mxd
		
		#endregion

		#region ================== Properties
		
		public override string Name { get { return "Doom Builder"; } }
		public static BuilderPlug Me { get { return me; } }

		// It is only safe to do this dynamically because we compile and distribute both
		// the core and this plugin together with the same revision number! In third party
		// plugins this should just contain a fixed number.
		public override int MinimumRevision { get { return Assembly.GetExecutingAssembly().GetName().Version.Revision; } }
		
		public MenusForm MenusForm { get { return menusform; } }
		public CurveLinedefsForm CurveLinedefsForm { get { return curvelinedefsform ?? (curvelinedefsform = new CurveLinedefsForm()); } }
		public FindReplaceForm FindReplaceForm { get { return findreplaceform ?? (findreplaceform = new FindReplaceForm()); } }
		public ErrorCheckForm ErrorCheckForm { get { return errorcheckform ?? (errorcheckform = new ErrorCheckForm()); } }
		public PreferencesForm PreferencesForm { get { return preferencesform; } }

		// Settings
		public int ShowVisualThings { get { return showvisualthings; } set { showvisualthings = value; } }
		public bool UseGravity { get { return usegravity; } set { usegravity = value; } }
		public int ChangeHeightBySidedef { get { return changeheightbysidedef; } }
		public bool EditNewThing { get { return editnewthing; } }
		public bool EditNewSector { get { return editnewsector; } }
		public bool AdditiveSelect { get { return additiveselect; } }
		public bool AutoClearSelection { get { return autoclearselection; } }
		public bool VisualModeClearSelection { get { return visualmodeclearselection; } }
		public string CopiedTexture { get { return copiedtexture; } set { copiedtexture = value; } }
		public string CopiedFlat { get { return copiedflat; } set { copiedflat = value; } }
		public Point CopiedOffsets { get { return copiedoffsets; } set { copiedoffsets = value; } }
		public VertexProperties CopiedVertexProps { get { return copiedvertexprops; } set { copiedvertexprops = value; } }
		public SectorProperties CopiedSectorProps { get { return copiedsectorprops; } set { copiedsectorprops = value; } }
		public SidedefProperties CopiedSidedefProps { get { return copiedsidedefprops; } set { copiedsidedefprops = value; } }
		public LinedefProperties CopiedLinedefProps { get { return copiedlinedefprops; } set { copiedlinedefprops = value; } }
		public ThingProperties CopiedThingProps { get { return copiedthingprops; } set { copiedthingprops = value; } }
		public bool ViewSelectionNumbers { get { return viewselectionnumbers; } set { viewselectionnumbers = value; } }
		public bool ViewSelectionEffects { get { return viewselectioneffects; } set { viewselectioneffects = value; } } //mxd
		public float StitchRange { get { return stitchrange; } internal set { stitchrange = value; } }
		public float HighlightRange { get { return highlightrange; } }
		public float HighlightThingsRange { get { return highlightthingsrange; } }
		public float SplitLinedefsRange { get { return splitlinedefsrange; } }
		public bool UseHighlight { 
			get { 
				return usehighlight; 
			} 
			set { 
				usehighlight = value;
				General.Map.Renderer3D.ShowSelection = usehighlight;
				General.Map.Renderer3D.ShowHighlight = usehighlight;
			} 
		}
		public bool AutoDragOnPaste { get { return autodragonpaste; } set { autodragonpaste = value; } }
		public bool AutoDrawOnEdit { get { return autoDrawOnEdit; } set { autoDrawOnEdit = value; } } //mxd
		public bool AutoAlignTextureOffsetsOnCreate { get { return autoAlignTextureOffsetsOnCreate; } set { autoAlignTextureOffsetsOnCreate = value; } } //mxd
		public bool DontMoveGeometryOutsideMapBoundary { get { return dontMoveGeometryOutsideMapBoundary; } set { DontMoveGeometryOutsideMapBoundary = value; } } //mxd
		public bool MarqueSelectTouching { get { return marqueSelectTouching; } set { marqueSelectTouching = value; } } //mxd
		public bool SyncSelection { get { return syncSelection; } set { syncSelection = value; } } //mxd
		public bool LockSectorTextureOffsetsWhileDragging { get { return lockSectorTextureOffsetsWhileDragging; } internal set { lockSectorTextureOffsetsWhileDragging = value; } } //mxd
		public bool DragThingsInSectorsMode { get { return dragThingsInSectorsMode; } internal set { dragThingsInSectorsMode = value; } } //mxd
		
		#endregion

		#region ================== Initialize / Dispose

		// When plugin is initialized
		public override void OnInitialize()
		{
			// Setup
			me = this;

			// Settings
			showvisualthings = 2;
			usegravity = false;
			usehighlight = true;
			LoadSettings();
			
			// Load menus form and register it
			menusform = new MenusForm();
			menusform.Register();
			menusform.TextureOffsetLock.Checked = lockSectorTextureOffsetsWhileDragging; //mxd
			menusform.DragThingsInSelectedSectors.Checked = dragThingsInSectorsMode; //mxd
			
			// Load Undo\Redo docker
			undoredopanel = new UndoRedoPanel();
			undoredodocker = new Docker("undoredo", "Undo / Redo", undoredopanel);
			General.Interface.AddDocker(undoredodocker);

			//mxd. Create Overrides docker
			drawingOverridesPanel = new SectorDrawingOptionsPanel();
			drawingOverridesDocker = new Docker("drawingoverrides", "Draw Settings", drawingOverridesPanel);

			//mxd
			General.Actions.BindMethods(this);
		}
		
		// Disposer
		public override void Dispose()
		{
			// Not already disposed?
			if(!IsDisposed)
			{
				// Clean up
				General.Interface.RemoveDocker(undoredodocker);
				General.Interface.RemoveDocker(drawingOverridesDocker); //mxd

				undoredopanel.Dispose();
				drawingOverridesPanel.Dispose(); //mxd
				menusform.Unregister();
				menusform.Dispose();
				menusform = null;
				curvelinedefsform.Dispose();
				curvelinedefsform = null;
				findreplaceform.Dispose();
				findreplaceform = null;
				errorcheckform.Dispose();
				errorcheckform = null;
				
				// Done
				me = null;
				base.Dispose();
			}
		}

		#endregion

		#region ================== Methods

		// This loads the plugin settings
		private void LoadSettings()
		{
			changeheightbysidedef = General.Settings.ReadPluginSetting("changeheightbysidedef", 0);
			editnewthing = General.Settings.ReadPluginSetting("editnewthing", true);
			editnewsector = General.Settings.ReadPluginSetting("editnewsector", false);
			additiveselect = General.Settings.ReadPluginSetting("additiveselect", false);
			autoclearselection = General.Settings.ReadPluginSetting("autoclearselection", false);
			visualmodeclearselection = General.Settings.ReadPluginSetting("visualmodeclearselection", false);
			viewselectionnumbers = General.Settings.ReadPluginSetting("viewselectionnumbers", true);
			viewselectioneffects = General.Settings.ReadPluginSetting("viewselectioneffects", true); //mxd
			stitchrange = General.Settings.ReadPluginSetting("stitchrange", 20);
			highlightrange = General.Settings.ReadPluginSetting("highlightrange", 20);
			highlightthingsrange = General.Settings.ReadPluginSetting("highlightthingsrange", 10);
			splitlinedefsrange = General.Settings.ReadPluginSetting("splitlinedefsrange", 10);
			autodragonpaste = General.Settings.ReadPluginSetting("autodragonpaste", false);
			autoDrawOnEdit = General.Settings.ReadPluginSetting("autodrawonedit", true); //mxd
			autoAlignTextureOffsetsOnCreate = General.Settings.ReadPluginSetting("autoaligntextureoffsetsoncreate", false); //mxd
			dontMoveGeometryOutsideMapBoundary = General.Settings.ReadPluginSetting("dontmovegeometryoutsidemapboundary", false); //mxd
			syncSelection = General.Settings.ReadPluginSetting("syncselection", false); //mxd
			lockSectorTextureOffsetsWhileDragging = General.Settings.ReadPluginSetting("locktextureoffsets", false); //mxd
			dragThingsInSectorsMode = General.Settings.ReadPluginSetting("dragthingsinsectorsmode", true); //mxd
		}

		//mxd. Save settings, which can be changed via UI
		private void saveSettings() {
			General.Settings.WritePluginSetting("locktextureoffsets", lockSectorTextureOffsetsWhileDragging);
			General.Settings.WritePluginSetting("viewselectionnumbers", viewselectionnumbers);
			General.Settings.WritePluginSetting("viewselectioneffects", viewselectioneffects);
			General.Settings.WritePluginSetting("dragthingsinsectorsmode", dragThingsInSectorsMode);
		}

		#endregion

		#region ================== Events

		// When floor surface geometry is created for classic modes
		public override void OnSectorFloorSurfaceUpdate(Sector s, ref FlatVertex[] vertices)
		{
			ImageData img = General.Map.Data.GetFlatImage(s.LongFloorTexture);
			if((img != null) && img.IsImageLoaded)
			{
				//mxd. Merged from GZDoomEditing plugin
				if(General.Map.UDMF) {
					// Fetch ZDoom fields
					Vector2D offset = new Vector2D(s.Fields.GetValue("xpanningfloor", 0.0f),
												   s.Fields.GetValue("ypanningfloor", 0.0f));
					Vector2D scale = new Vector2D(s.Fields.GetValue("xscalefloor", 1.0f),
												  s.Fields.GetValue("yscalefloor", 1.0f));
					float rotate = s.Fields.GetValue("rotationfloor", 0.0f);
					int color = s.Fields.GetValue("lightcolor", -1);
					int light = s.Fields.GetValue("lightfloor", 0);
					bool absolute = s.Fields.GetValue("lightfloorabsolute", false);

					// Setup the vertices with the given settings
					SetupSurfaceVertices(vertices, s, img, offset, scale, rotate, color, light, absolute);
				} else {
					// Make scalars
					float sw = 1.0f / img.ScaledWidth;
					float sh = 1.0f / img.ScaledHeight;

					// Make proper texture coordinates
					for(int i = 0; i < vertices.Length; i++) {
						vertices[i].u = vertices[i].u * sw;
						vertices[i].v = -vertices[i].v * sh;
					}
				}
			}
		}

		// When ceiling surface geometry is created for classic modes
		public override void OnSectorCeilingSurfaceUpdate(Sector s, ref FlatVertex[] vertices)
		{
			ImageData img = General.Map.Data.GetFlatImage(s.LongCeilTexture);
			if((img != null) && img.IsImageLoaded)
			{
				//mxd. Merged from GZDoomEditing plugin
				if(General.Map.UDMF) {
					// Fetch ZDoom fields
					Vector2D offset = new Vector2D(s.Fields.GetValue("xpanningceiling", 0.0f),
												   s.Fields.GetValue("ypanningceiling", 0.0f));
					Vector2D scale = new Vector2D(s.Fields.GetValue("xscaleceiling", 1.0f),
												  s.Fields.GetValue("yscaleceiling", 1.0f));
					float rotate = s.Fields.GetValue("rotationceiling", 0.0f);
					int color = s.Fields.GetValue("lightcolor", -1);
					int light = s.Fields.GetValue("lightceiling", 0);
					bool absolute = s.Fields.GetValue("lightceilingabsolute", false);

					// Setup the vertices with the given settings
					SetupSurfaceVertices(vertices, s, img, offset, scale, rotate, color, light, absolute);
				} else {
					// Make scalars
					float sw = 1.0f / img.ScaledWidth;
					float sh = 1.0f / img.ScaledHeight;

					// Make proper texture coordinates
					for(int i = 0; i < vertices.Length; i++) {
						vertices[i].u = vertices[i].u * sw;
						vertices[i].v = -vertices[i].v * sh;
					}
				}
			}
		}

		// When the editing mode changes
		public override bool OnModeChange(EditMode oldmode, EditMode newmode)
		{
			// Show the correct menu for the new mode
			menusform.ShowEditingModeMenu(newmode);
			
			return base.OnModeChange(oldmode, newmode);
		}

		// When the Preferences dialog is shown
		public override void OnShowPreferences(PreferencesController controller)
		{
			base.OnShowPreferences(controller);

			// Load preferences
			preferencesform = new PreferencesForm();
			preferencesform.Setup(controller);
		}

		// When the Preferences dialog is closed
		public override void OnClosePreferences(PreferencesController controller)
		{
			base.OnClosePreferences(controller);

			// Apply settings that could have been changed
			LoadSettings();
			
			// Unload preferences
			preferencesform.Dispose();
			preferencesform = null;
		}
		
		// New map created
		public override void OnMapNewEnd()
		{
			base.OnMapNewEnd();
			undoredopanel.SetBeginDescription("New Map");
			undoredopanel.UpdateList();

			//mxd
			General.Interface.AddDocker(drawingOverridesDocker);
			drawingOverridesPanel.Setup();
		}
		
		// Map opened
		public override void OnMapOpenEnd()
		{
			base.OnMapOpenEnd();
			undoredopanel.SetBeginDescription("Opened Map");
			undoredopanel.UpdateList();

			//mxd
			General.Interface.AddDocker(drawingOverridesDocker);
			drawingOverridesPanel.Setup();
			General.Map.Renderer2D.UpdateExtraFloorFlag(); //mxd
		}
		
		// Map closed
		public override void OnMapCloseEnd()
		{
			base.OnMapCloseEnd();
			undoredopanel.UpdateList();

			//mxd
			General.Interface.RemoveDocker(drawingOverridesDocker);

			//mxd. Save settings
			saveSettings();
		}
		
		// Redo performed
		public override void OnRedoEnd()
		{
			base.OnRedoEnd();
			undoredopanel.UpdateList();
		}
		
		// Undo performed
		public override void OnUndoEnd()
		{
			base.OnUndoEnd();
			undoredopanel.UpdateList();
		}
		
		// Undo created
		public override void OnUndoCreated()
		{
			base.OnUndoCreated();
			undoredopanel.UpdateList();
		}
		
		// Undo withdrawn
		public override void OnUndoWithdrawn()
		{
			base.OnUndoWithdrawn();
			undoredopanel.UpdateList();
		}
		
		#endregion
		
		#region ================== Tools

		//mxd. merged from GZDoomEditing plugin
		// This applies the given values on the vertices
		private static void SetupSurfaceVertices(FlatVertex[] vertices, Sector s, ImageData img, Vector2D offset,
										  Vector2D scale, float rotate, int color, int light, bool absolute) 
		{
			// Prepare for math!
			rotate = Angle2D.DegToRad(rotate);
			Vector2D texscale = new Vector2D(1.0f / img.ScaledWidth, 1.0f / img.ScaledHeight);
			if(!absolute) light = s.Brightness + light;
			PixelColor lightcolor = PixelColor.FromInt(color);
			PixelColor brightness = PixelColor.FromInt(General.Map.Renderer2D.CalculateBrightness(light));
			PixelColor finalcolor = PixelColor.Modulate(lightcolor, brightness);
			color = finalcolor.WithAlpha(255).ToInt();

			// Do the math for all vertices
			for(int i = 0; i < vertices.Length; i++) {
				Vector2D pos = new Vector2D(vertices[i].x, vertices[i].y);
				pos = pos.GetRotated(rotate);
				pos.y = -pos.y;
				pos = (pos + offset) * scale * texscale;
				vertices[i].u = pos.x;
				vertices[i].v = pos.y;
				vertices[i].c = color;
			}
		}
		
		// This finds all class types that inherits from the given type
		public Type[] FindClasses(Type t)
		{
			List<Type> found = new List<Type>();

			// Get all exported types
			Type[] types = Assembly.GetExecutingAssembly().GetTypes();
			foreach(Type it in types)
			{
				// Compare types
				if(t.IsAssignableFrom(it)) found.Add(it);
			}

			// Return list
			return found.ToArray();
		}
		
		// This renders the associated sectors/linedefs with the indication color
		public void PlotAssociations(IRenderer2D renderer, Association asso)
		{
			// Tag must be above zero
			if(asso.tag < 1) return;
			
			// Sectors?
			if(asso.type == UniversalType.SectorTag)
			{
				List<Line3D> lines = new List<Line3D>(); //mxd
				foreach(Sector s in General.Map.Map.Sectors) {
					if(s.Tag == asso.tag) {
						renderer.PlotSector(s, General.Colors.Indication);
						if (General.Settings.GZShowEventLines) {
							Vector2D end = (s.Labels.Count > 0 ? s.Labels[0].position : new Vector2D(s.BBox.X + s.BBox.Width / 2, s.BBox.Y + s.BBox.Height / 2));
							lines.Add(new Line3D(asso.Center, end)); //mxd
						}
					}
				}

				if(General.Settings.GZShowEventLines) { //mxd
					foreach(Line3D l in lines)
						renderer.PlotArrow(l, l.LineType == Line3DType.ACTIVATOR ? General.Colors.Selection : General.Colors.InfoLine);
				}
			}
			// Linedefs?
			else if(asso.type == UniversalType.LinedefTag)
			{
				List<Line3D> lines = new List<Line3D>(); //mxd
				foreach(Linedef l in General.Map.Map.Linedefs) {
					if(l.Tag == asso.tag) {
						renderer.PlotLinedef(l, General.Colors.Indication);
						if(General.Settings.GZShowEventLines)
							lines.Add(new Line3D(asso.Center, l.GetCenterPoint()));//mxd
					}
				}

				if(General.Settings.GZShowEventLines) { //mxd
					foreach(Line3D l in lines)
						renderer.PlotArrow(l, l.LineType == Line3DType.ACTIVATOR ? General.Colors.Selection : General.Colors.InfoLine);
				}
			}
		}
		

		// This renders the associated things with the indication color
		public void RenderAssociations(IRenderer2D renderer, Association asso)
		{
			// Tag must be above zero
			if(asso.tag < 1) return;

			// Things?
			if(asso.type == UniversalType.ThingTag)
			{
				List<Line3D> lines = new List<Line3D>(); //mxd
				foreach(Thing t in General.Map.Map.Things){
					if(t.Tag == asso.tag) {
						renderer.RenderThing(t, General.Colors.Indication, 1.0f);
						if(General.Settings.GZShowEventLines)
							lines.Add(new Line3D(asso.Center, t.Position));//mxd
					}
				}
				if(General.Settings.GZShowEventLines) { //mxd
					foreach(Line3D l in lines)
						renderer.RenderArrow(l, l.LineType == Line3DType.ACTIVATOR ? General.Colors.Selection : General.Colors.InfoLine);
				}
			} 
			else if(asso.type == UniversalType.SectorTag) //mxd. Render sector highlight
			{ 
				foreach(Sector s in General.Map.Map.Sectors) {
					if(s.Tag == asso.tag) {
						int highlightedColor = General.Colors.Highlight.WithAlpha(128).ToInt();
						FlatVertex[] verts = new FlatVertex[s.FlatVertices.Length];
						s.FlatVertices.CopyTo(verts, 0);
						for(int i = 0; i < verts.Length; i++)
							verts[i].c = highlightedColor;
						renderer.RenderGeometry(verts, null, true);
					}
				}
			}
		}
		

		// This renders the associated sectors/linedefs with the indication color
		public void PlotReverseAssociations(IRenderer2D renderer, Association asso)
		{
			// Tag must be above zero
			if(asso.tag < 1) return;

			List<Line3D> lines = new List<Line3D>(); //mxd
			
			// Doom style referencing to sectors?
			if(General.Map.Config.LineTagIndicatesSectors && (asso.type == UniversalType.SectorTag))
			{
				// Linedefs
				foreach(Linedef l in General.Map.Map.Linedefs)
				{
					// Any action on this line?
					if(l.Action > 0)
					{
						if(l.Tag == asso.tag) {
							renderer.PlotLinedef(l, General.Colors.Indication);
							
							if(General.Settings.GZShowEventLines) //mxd
								lines.Add(new Line3D(l.GetCenterPoint(), asso.Center)); //mxd
						}
					}
				}
				if(General.Settings.GZShowEventLines) { //mxd
					foreach(Line3D l in lines)
						renderer.PlotArrow(l, l.LineType == Line3DType.ACTIVATOR ? General.Colors.Selection : General.Colors.InfoLine);
				}
			}
			else
			{
				// Linedefs
				foreach(Linedef l in General.Map.Map.Linedefs)
				{
					// Known action on this line?
					if((l.Action > 0) && General.Map.Config.LinedefActions.ContainsKey(l.Action))
					{
						LinedefActionInfo action = General.Map.Config.LinedefActions[l.Action];
						if( ((action.Args[0].Type == (int)asso.type) && (l.Args[0] == asso.tag)) ||
							((action.Args[1].Type == (int)asso.type) && (l.Args[1] == asso.tag)) ||
							((action.Args[2].Type == (int)asso.type) && (l.Args[2] == asso.tag)) ||
							((action.Args[3].Type == (int)asso.type) && (l.Args[3] == asso.tag)) ||
							((action.Args[4].Type == (int)asso.type) && (l.Args[4] == asso.tag)) ){
							
							renderer.PlotLinedef(l, General.Colors.Indication);

							if(General.Settings.GZShowEventLines) //mxd
								lines.Add(new Line3D(l.GetCenterPoint(), asso.Center));
						}
					}
				}

				if(General.Settings.GZShowEventLines) { //mxd
					foreach(Line3D l in lines) {
						renderer.PlotArrow(l, l.LineType == Line3DType.ACTIVATOR ? General.Colors.Selection : General.Colors.InfoLine);
					}
				}
			}
		}
		

		// This renders the associated things with the indication color
		public void RenderReverseAssociations(IRenderer2D renderer, Association asso)
		{
			// Tag must be above zero
			if(asso.tag < 1) return;

			List<Line3D> lines = new List<Line3D>(); //mxd

			// Things
			foreach(Thing t in General.Map.Map.Things)
			{
				// Known action on this thing?
				if((t.Action > 0) && General.Map.Config.LinedefActions.ContainsKey(t.Action))
				{
					LinedefActionInfo action = General.Map.Config.LinedefActions[t.Action];
					if(  ((action.Args[0].Type == (int)asso.type) && (t.Args[0] == asso.tag)) ||
						 ((action.Args[1].Type == (int)asso.type) && (t.Args[1] == asso.tag)) ||
						 ((action.Args[2].Type == (int)asso.type) && (t.Args[2] == asso.tag)) ||
						 ((action.Args[3].Type == (int)asso.type) && (t.Args[3] == asso.tag)) ||
						 ((action.Args[4].Type == (int)asso.type) && (t.Args[4] == asso.tag)) ){
						renderer.RenderThing(t, General.Colors.Indication, 1.0f);
						if(General.Settings.GZShowEventLines) //mxd
							lines.Add(new Line3D(t.Position, asso.Center));
					}
				}
			}

			if(General.Settings.GZShowEventLines) {//mxd
				foreach(Line3D l in lines) {
					renderer.RenderArrow(l, l.LineType == Line3DType.ACTIVATOR ? General.Colors.Selection : General.Colors.InfoLine);
				}
			}
		}

		#endregion

		//mxd
		#region Actions

		[BeginAction("exporttoobj")]
		private void exportToObj() {
			// Convert geometry selection to sectors
			General.Map.Map.ConvertSelection(SelectionType.Sectors);
			
			//get sectors
			ICollection<Sector> sectors = General.Map.Map.SelectedSectorsCount == 0 ? General.Map.Map.Sectors : General.Map.Map.GetSelectedSectors(true);
			if(sectors.Count == 0) {
				General.Interface.DisplayStatus(StatusType.Warning, "OBJ export failed. Map has no sectors!");
				return;
			}

			//show settings form
			WavefrontSettingsForm form = new WavefrontSettingsForm(General.Map.Map.SelectedSectorsCount == 0 ? -1 : sectors.Count);
			if(form.ShowDialog() == DialogResult.OK) {
				WavefrontExportSettings data = new WavefrontExportSettings(Path.GetFileNameWithoutExtension(form.FilePath), Path.GetDirectoryName(form.FilePath), form.ObjScale, form.UseGZDoomScale, form.ExportTextures);
				WavefrontExporter e = new WavefrontExporter();
				e.Export(sectors, data);
			}
		}

		[BeginAction("pastepropertiesoptions")]
		private void showPastePropertiesOptions() {
			var form = new PastePropertiesOptionsForm();
			form.ShowDialog(Form.ActiveForm);
		}

		#endregion
	}
}
