
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
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using CodeImp.DoomBuilder.Windows;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Rendering;
using CodeImp.DoomBuilder.Geometry;
using System.Drawing;
using CodeImp.DoomBuilder.Editing;
using CodeImp.DoomBuilder.Plugins;
using CodeImp.DoomBuilder.Types;
using CodeImp.DoomBuilder.Config;
using CodeImp.DoomBuilder.Data;
using CodeImp.DoomBuilder.Controls;
using CodeImp.DoomBuilder.GZBuilder.Geometry;
using CodeImp.DoomBuilder.Actions;
using CodeImp.DoomBuilder.BuilderModes.IO;
using CodeImp.DoomBuilder.BuilderModes.Interface;
using CodeImp.DoomBuilder.GZBuilder.Tools;

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

        //mxd
		private ToolStripMenuItem exportToObjMenuItem;
        private ToolStripMenuItem snapModeMenuItem;
        private ToolStripMenuItem drawLinesModeMenuItem;
        private ToolStripMenuItem drawRectModeMenuItem;
        private ToolStripMenuItem drawEllipseModeMenuItem;
		private ToolStripMenuItem drawCurveModeMenuItem;
		
		// Settings
		private int showvisualthings;			// 0 = none, 1 = sprite only, 2 = sprite caged
		private bool usegravity;
		private int changeheightbysidedef;		// 0 = nothing, 1 = change ceiling, 2 = change floor
		private int splitlinebehavior;			// 0 = adjust texcoords, 1 = copy texcoords, 2 = reset texcoords
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
		private float stitchrange;
		private float highlightrange;
		private float highlightthingsrange;
		private float splitlinedefsrange;
		private bool usehighlight;
		private bool autodragonpaste;
		private bool autoAlignTextureOffsetsOnCreate;//mxd
		private bool autoAlignTextureOffsetsOnDrag;//mxd
		private bool dontMoveGeometryOutsideMapBoundary;//mxd
		private bool marqueSelectTouching; //mxd. Select elements partially/fully inside of marque selection?
		private bool syncSelection; //mxd. Sync selection between Visual and Classic modes.
		private bool objExportTextures; //mxd
		private bool objGZDoomScale; //mxd
		private float objScale; //mxd
		private bool lockSectorTextureOffsetsWhileDragging; //mxd
		
		#endregion

		#region ================== Properties
		
		public override string Name { get { return "Doom Builder"; } }
		public static BuilderPlug Me { get { return me; } }

		// It is only safe to do this dynamically because we compile and distribute both
		// the core and this plugin together with the same revision number! In third party
		// plugins this should just contain a fixed number.
		public override int MinimumRevision { get { return Assembly.GetExecutingAssembly().GetName().Version.Revision; } }
		
		public MenusForm MenusForm { get { return menusform; } }
		public CurveLinedefsForm CurveLinedefsForm { get { return curvelinedefsform; } }
		public FindReplaceForm FindReplaceForm { get { return findreplaceform; } }
		public ErrorCheckForm ErrorCheckForm { get { return errorcheckform; } }
		public PreferencesForm PreferencesForm { get { return preferencesform; } }

		// Settings
		public int ShowVisualThings { get { return showvisualthings; } set { showvisualthings = value; } }
		public bool UseGravity { get { return usegravity; } set { usegravity = value; } }
		public int ChangeHeightBySidedef { get { return changeheightbysidedef; } }
		public int SplitLineBehavior { get { return splitlinebehavior; } }
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
		public bool AutoAlignTextureOffsetsOnCreate { get { return autoAlignTextureOffsetsOnCreate; } set { autoAlignTextureOffsetsOnCreate = value; } } //mxd
		public bool AutoAlignTextureOffsetsOnDrag { get { return autoAlignTextureOffsetsOnDrag; } set { autoAlignTextureOffsetsOnDrag = value; } } //mxd
		public bool DontMoveGeometryOutsideMapBoundary { get { return dontMoveGeometryOutsideMapBoundary; } set { DontMoveGeometryOutsideMapBoundary = value; } } //mxd
		public bool MarqueSelectTouching { get { return marqueSelectTouching; } set { marqueSelectTouching = value; } } //mxd
		public bool SyncSelection { get { return syncSelection; } set { syncSelection = value; } } //mxd
		public bool ObjExportTextures { get { return objExportTextures; } internal set { objExportTextures = value; } } //mxd
		public bool ObjGZDoomScale { get { return objGZDoomScale; } internal set { objGZDoomScale = value; } } //mxd
		public float ObjScale { get { return objScale; } internal set { objScale = value; } } //mxd
		public bool LockSectorTextureOffsetsWhileDragging { get { return lockSectorTextureOffsetsWhileDragging; } internal set { lockSectorTextureOffsetsWhileDragging = value; } } //mxd
		
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
			
			// Load curve linedefs form
			curvelinedefsform = new CurveLinedefsForm();
			
			// Load find/replace form
			findreplaceform = new FindReplaceForm();
			
			// Load error checking form
			errorcheckform = new ErrorCheckForm();
			
			// Load Undo\Redo docker
			undoredopanel = new UndoRedoPanel();
			undoredodocker = new Docker("undoredo", "Undo / Redo", undoredopanel);
			General.Interface.AddDocker(undoredodocker);

			//mxd. Export to .obj
			exportToObjMenuItem = new ToolStripMenuItem("Export to .obj...");
			exportToObjMenuItem.Tag = "exporttoobj";
			exportToObjMenuItem.Click += new EventHandler(InvokeTaggedAction);
			exportToObjMenuItem.Enabled = false;
			General.Interface.AddMenu(exportToObjMenuItem, MenuSection.FileNewOpenClose);

            //mxd. add "Snap Vertices" menu button
            snapModeMenuItem = new ToolStripMenuItem("Snap selected map elements to grid");
            snapModeMenuItem.Tag = "snapvertstogrid";
            snapModeMenuItem.Click += new EventHandler(InvokeTaggedAction);
            snapModeMenuItem.Image = CodeImp.DoomBuilder.BuilderModes.Properties.Resources.SnapVerts;
            snapModeMenuItem.Enabled = false;
            General.Interface.AddMenu(snapModeMenuItem, MenuSection.EditGeometry);

            //mxd. add draw modes buttons
            //draw ellipse
            drawEllipseModeMenuItem = new ToolStripMenuItem("Draw Ellipse");
            drawEllipseModeMenuItem.Tag = "drawellipsemode";
            drawEllipseModeMenuItem.Click += new EventHandler(InvokeTaggedAction);
            drawEllipseModeMenuItem.Image = CodeImp.DoomBuilder.BuilderModes.Properties.Resources.DrawEllipseMode;
            drawEllipseModeMenuItem.Enabled = false;
            General.Interface.AddMenu(drawEllipseModeMenuItem, MenuSection.ModeDrawModes);

            //draw rectangle
            drawRectModeMenuItem = new ToolStripMenuItem("Draw Rectangle");
            drawRectModeMenuItem.Tag = "drawrectanglemode";
            drawRectModeMenuItem.Click += new EventHandler(InvokeTaggedAction);
            drawRectModeMenuItem.Image = CodeImp.DoomBuilder.BuilderModes.Properties.Resources.DrawRectMode;
            drawRectModeMenuItem.Enabled = false;
            General.Interface.AddMenu(drawRectModeMenuItem, MenuSection.ModeDrawModes);

			//draw curve 
			drawCurveModeMenuItem = new ToolStripMenuItem("Draw Curve");
			drawCurveModeMenuItem.Tag = "drawcurvemode";
			drawCurveModeMenuItem.Click += new EventHandler(InvokeTaggedAction);
			drawCurveModeMenuItem.Image = CodeImp.DoomBuilder.BuilderModes.Properties.Resources.DrawCurveMode;
			drawCurveModeMenuItem.Enabled = false;
			General.Interface.AddMenu(drawCurveModeMenuItem, MenuSection.ModeDrawModes);

            //draw lines 
            drawLinesModeMenuItem = new ToolStripMenuItem("Draw Lines");
            drawLinesModeMenuItem.Tag = "drawlinesmode";
            drawLinesModeMenuItem.Click += new EventHandler(InvokeTaggedAction);
            drawLinesModeMenuItem.Image = CodeImp.DoomBuilder.BuilderModes.Properties.Resources.DrawLinesMode;
            drawLinesModeMenuItem.Enabled = false;
            General.Interface.AddMenu(drawLinesModeMenuItem, MenuSection.ModeDrawModes);

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

                //mxd
				General.Interface.RemoveMenu(exportToObjMenuItem);
                General.Interface.RemoveMenu(snapModeMenuItem);
                General.Interface.RemoveMenu(drawLinesModeMenuItem);
				General.Interface.RemoveMenu(drawCurveModeMenuItem);
                General.Interface.RemoveMenu(drawRectModeMenuItem);
                General.Interface.RemoveMenu(drawEllipseModeMenuItem);

				undoredopanel.Dispose();
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
			splitlinebehavior = General.Settings.ReadPluginSetting("splitlinebehavior", 0);
			editnewthing = General.Settings.ReadPluginSetting("editnewthing", true);
			editnewsector = General.Settings.ReadPluginSetting("editnewsector", false);
			additiveselect = General.Settings.ReadPluginSetting("additiveselect", false);
			autoclearselection = General.Settings.ReadPluginSetting("autoclearselection", false);
			visualmodeclearselection = General.Settings.ReadPluginSetting("visualmodeclearselection", false);
			viewselectionnumbers = General.Settings.ReadPluginSetting("viewselectionnumbers", true);
			stitchrange = (float)General.Settings.ReadPluginSetting("stitchrange", 20);
			highlightrange = (float)General.Settings.ReadPluginSetting("highlightrange", 20);
			highlightthingsrange = (float)General.Settings.ReadPluginSetting("highlightthingsrange", 10);
			splitlinedefsrange = (float)General.Settings.ReadPluginSetting("splitlinedefsrange", 10);
			autodragonpaste = General.Settings.ReadPluginSetting("autodragonpaste", false);
			autoAlignTextureOffsetsOnCreate = General.Settings.ReadPluginSetting("autoaligntextureoffsetsoncreate", true); //mxd
			autoAlignTextureOffsetsOnDrag = General.Settings.ReadPluginSetting("autoaligntextureoffsetsondrag", true); //mxd
			dontMoveGeometryOutsideMapBoundary = General.Settings.ReadPluginSetting("dontmovegeometryoutsidemapboundary", false); //mxd
			syncSelection = General.Settings.ReadPluginSetting("syncselection", false); //mxd
			objExportTextures = General.Settings.ReadPluginSetting("objexporttextures", false); //mxd
			objGZDoomScale = General.Settings.ReadPluginSetting("objgzdoomscale", false); //mxd
			objScale = General.Settings.ReadPluginSetting("objscale", 1.0f); //mxd
			lockSectorTextureOffsetsWhileDragging = General.Settings.ReadPluginSetting("locktextureoffsets", false); //mxd
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
			exportToObjMenuItem.Enabled = true;
            snapModeMenuItem.Enabled = true;
            drawLinesModeMenuItem.Enabled = true;
			drawCurveModeMenuItem.Enabled = true;
            drawRectModeMenuItem.Enabled = true;
            drawEllipseModeMenuItem.Enabled = true;
		}
		
		// Map opened
		public override void OnMapOpenEnd()
		{
			base.OnMapOpenEnd();
			undoredopanel.SetBeginDescription("Opened Map");
			undoredopanel.UpdateList();

            //mxd
			exportToObjMenuItem.Enabled = true;
            snapModeMenuItem.Enabled = true;
            drawLinesModeMenuItem.Enabled = true;
			drawCurveModeMenuItem.Enabled = true;
            drawRectModeMenuItem.Enabled = true;
            drawEllipseModeMenuItem.Enabled = true;

			General.Map.Renderer2D.UpdateExtraFloorFlag(); //mxd
		}
		
		// Map closed
		public override void OnMapCloseEnd()
		{
			base.OnMapCloseEnd();
			undoredopanel.UpdateList();

            //mxd
			exportToObjMenuItem.Enabled = false;
            snapModeMenuItem.Enabled = false;
            drawLinesModeMenuItem.Enabled = false;
			drawCurveModeMenuItem.Enabled = false;
            drawRectModeMenuItem.Enabled = false;
            drawEllipseModeMenuItem.Enabled = false;

			General.Settings.WritePluginSetting("locktextureoffsets", lockSectorTextureOffsetsWhileDragging);
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

        //mxd
        private void InvokeTaggedAction(object sender, EventArgs e) {
            General.Interface.InvokeTaggedAction(sender, e);
        }
		
		#endregion
		
		#region ================== Tools

		//mxd. merged from GZDoomEditing plugin
		// This applies the given values on the vertices
		private void SetupSurfaceVertices(FlatVertex[] vertices, Sector s, ImageData img, Vector2D offset,
										  Vector2D scale, float rotate, int color, int light, bool absolute) {
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

		// This adjusts texture coordinates for splitted lines according to the user preferences
		public void AdjustSplitCoordinates(Linedef oldline, Linedef newline)
		{
			//mxd. Clamp texture coordinates (they are already adjusted, we just need to clamp OffsetX by texture width)
			if(splitlinebehavior == 0) {
				if((oldline.Front != null) && (newline.Front != null)) {
					//get texture
					ImageData texture = null;

					if(newline.Front.MiddleRequired() && newline.Front.MiddleTexture.Length > 1 && General.Map.Data.GetFlatExists(newline.Front.MiddleTexture)) {
						texture = General.Map.Data.GetFlatImage(newline.Front.MiddleTexture);
					} else if(newline.Front.HighRequired() && newline.Front.HighTexture.Length > 1 && General.Map.Data.GetFlatExists(newline.Front.HighTexture)) {
						texture = General.Map.Data.GetFlatImage(newline.Front.HighTexture);
					} else if(newline.Front.LowRequired() && newline.Front.LowTexture.Length > 1 && General.Map.Data.GetFlatExists(newline.Front.LowTexture)) {
						texture = General.Map.Data.GetFlatImage(newline.Front.LowTexture);
					}

					//clamp offsetX
					if(texture != null)	newline.Front.OffsetX %= texture.Width;
				}

				if((oldline.Back != null) && (newline.Back != null)) {
					//get texture
					ImageData texture = null;

					if(newline.Back.MiddleRequired() && newline.Back.MiddleTexture.Length > 1 && General.Map.Data.GetFlatExists(newline.Back.MiddleTexture)) {
						texture = General.Map.Data.GetFlatImage(newline.Back.MiddleTexture);
					} else if(newline.Back.HighRequired() && newline.Back.HighTexture.Length > 1 && General.Map.Data.GetFlatExists(newline.Back.HighTexture)) {
						texture = General.Map.Data.GetFlatImage(newline.Back.HighTexture);
					} else if(newline.Back.LowRequired() && newline.Back.LowTexture.Length > 1 && General.Map.Data.GetFlatExists(newline.Back.LowTexture)) {
						texture = General.Map.Data.GetFlatImage(newline.Back.LowTexture);
					}

					//clamp offsetX
					if(texture != null)	newline.Back.OffsetX %= texture.Width;
				}
			}
			// Copy X and Y coordinates
			if(splitlinebehavior == 1)
			{
				if((oldline.Front != null) && (newline.Front != null))
				{
					newline.Front.OffsetX = oldline.Front.OffsetX;
					newline.Front.OffsetY = oldline.Front.OffsetY;

					//mxd. Copy UDMF offsets as well
					if(General.Map.UDMF) {
						UDMFTools.SetFloat(newline.Front.Fields, "offsetx_top", oldline.Front.Fields.GetValue("offsetx_top", 0f));
						UDMFTools.SetFloat(newline.Front.Fields, "offsetx_mid", oldline.Front.Fields.GetValue("offsetx_mid", 0f));
						UDMFTools.SetFloat(newline.Front.Fields, "offsetx_bottom", oldline.Front.Fields.GetValue("offsetx_bottom", 0f));
						
						UDMFTools.SetFloat(newline.Front.Fields, "offsety_top", oldline.Front.Fields.GetValue("offsety_top", 0f));
						UDMFTools.SetFloat(newline.Front.Fields, "offsety_mid", oldline.Front.Fields.GetValue("offsety_mid", 0f));
						UDMFTools.SetFloat(newline.Front.Fields, "offsety_bottom", oldline.Front.Fields.GetValue("offsety_bottom", 0f));
					}
				}
				
				if((oldline.Back != null) && (newline.Back != null))
				{
					newline.Back.OffsetX = oldline.Back.OffsetX;
					newline.Back.OffsetY = oldline.Back.OffsetY;

					//mxd. Copy UDMF offsets as well
					if(General.Map.UDMF) {
						UDMFTools.SetFloat(newline.Back.Fields, "offsetx_top", oldline.Back.Fields.GetValue("offsetx_top", 0f));
						UDMFTools.SetFloat(newline.Back.Fields, "offsetx_mid", oldline.Back.Fields.GetValue("offsetx_mid", 0f));
						UDMFTools.SetFloat(newline.Back.Fields, "offsetx_bottom", oldline.Back.Fields.GetValue("offsetx_bottom", 0f));

						UDMFTools.SetFloat(newline.Back.Fields, "offsety_top", oldline.Back.Fields.GetValue("offsety_top", 0f));
						UDMFTools.SetFloat(newline.Back.Fields, "offsety_mid", oldline.Back.Fields.GetValue("offsety_mid", 0f));
						UDMFTools.SetFloat(newline.Back.Fields, "offsety_bottom", oldline.Back.Fields.GetValue("offsety_bottom", 0f));
					}
				}
			}
			// Reset X coordinate, copy Y coordinate
			else if(splitlinebehavior == 2)
			{
				if((oldline.Front != null) && (newline.Front != null))
				{
					newline.Front.OffsetX = 0;
					newline.Front.OffsetY = oldline.Front.OffsetY;

					//mxd. Reset UDMF X offset as well
					if(General.Map.UDMF) {
						UDMFTools.SetFloat(newline.Front.Fields, "offsetx_top", 0f);
						UDMFTools.SetFloat(newline.Front.Fields, "offsetx_mid", 0f);
						UDMFTools.SetFloat(newline.Front.Fields, "offsetx_bottom", 0f);
					}
				}
				
				if((oldline.Back != null) && (newline.Back != null))
				{
					newline.Back.OffsetX = 0;
					newline.Back.OffsetY = oldline.Back.OffsetY;

					//mxd. Reset UDMF X offset and copy Y offset as well
					if(General.Map.UDMF) {
						UDMFTools.SetFloat(newline.Back.Fields, "offsetx_top", 0f);
						UDMFTools.SetFloat(newline.Back.Fields, "offsetx_mid", 0f);
						UDMFTools.SetFloat(newline.Back.Fields, "offsetx_bottom", 0f);

						UDMFTools.SetFloat(newline.Back.Fields, "offsety_top", oldline.Back.Fields.GetValue("offsety_top", 0f));
						UDMFTools.SetFloat(newline.Back.Fields, "offsety_mid", oldline.Back.Fields.GetValue("offsety_mid", 0f));
						UDMFTools.SetFloat(newline.Back.Fields, "offsety_bottom", oldline.Back.Fields.GetValue("offsety_bottom", 0f));
					}
				}
			}
		}
		
		// This finds all class types that inherits from the given type
		public Type[] FindClasses(Type t)
		{
			List<Type> found = new List<Type>();
			Type[] types;

			// Get all exported types
			types = Assembly.GetExecutingAssembly().GetTypes();
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
			if(asso.tag <= 0) return;
			
			// Sectors?
			if(asso.type == UniversalType.SectorTag)
			{
				List<Line3D> lines = new List<Line3D>(); //mxd
				foreach(Sector s in General.Map.Map.Sectors) {
					if(s.Tag == asso.tag) {
						renderer.PlotSector(s, General.Colors.Indication);
						if(General.Settings.GZShowEventLines)
							lines.Add(new Line3D(asso.Center, new Vector2D(s.BBox.X + s.BBox.Width / 2, s.BBox.Y + s.BBox.Height / 2)));//mxd
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
							lines.Add(new Line3D(asso.Center, new Vector2D((l.Start.Position + l.End.Position) / 2)));//mxd
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
			if(asso.tag <= 0) return;

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
		}
		

		// This renders the associated sectors/linedefs with the indication color
		public void PlotReverseAssociations(IRenderer2D renderer, Association asso)
		{
			// Tag must be above zero
			if(asso.tag <= 0) return;

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
								lines.Add(new Line3D(new Vector2D((l.Start.Position + l.End.Position) / 2), asso.Center)); //mxd
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
								lines.Add(new Line3D(new Vector2D((l.Start.Position + l.End.Position) / 2), asso.Center));
						}
					}
				}

				if(General.Settings.GZShowEventLines) { //mxd
					//renderer.PlotArrows(lines, General.Colors.InfoLine);
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
			if(asso.tag <= 0) return;

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
				//renderer.RenderArrows(lines, General.Colors.InfoLine);
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
			//get sectors
			ICollection<Sector> sectors = General.Map.Map.SelectedSectorsCount == 0 ? General.Map.Map.Sectors : General.Map.Map.GetSelectedSectors(true);
			if(sectors.Count == 0) {
				General.Interface.DisplayStatus(StatusType.Warning, "OBJ export failed. Map has no sectors!");
				return;
			}

			//show settings form
			WavefrontSettingsForm form = new WavefrontSettingsForm(General.Map.Map.SelectedSectorsCount == 0 ? -1 : sectors.Count);
			if(form.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
				WavefrontExportSettings data = new WavefrontExportSettings(Path.GetFileNameWithoutExtension(form.FilePath), Path.GetDirectoryName(form.FilePath), BuilderPlug.Me.ObjScale, BuilderPlug.Me.ObjGZDoomScale, BuilderPlug.Me.ObjExportTextures);
				WavefrontExporter e = new WavefrontExporter();
				e.Export(sectors, data);
			}
		}

		#endregion
	}
}
