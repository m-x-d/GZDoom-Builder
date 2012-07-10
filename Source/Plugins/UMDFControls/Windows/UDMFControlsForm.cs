using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using CodeImp.DoomBuilder.Config;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.VisualModes;
using CodeImp.DoomBuilder.Windows;
using CodeImp.DoomBuilder.Geometry;
using CodeImp.DoomBuilder.Types;
using System.Globalization;

namespace CodeImp.DoomBuilder.UDMFControls
{
    public partial class UDMFControlsForm : DelayedForm {
        private List<SurfaceProperties> floors;
        private List<SurfaceProperties> ceilings;

        private List<SurfaceProperties> wallsTop;
        private List<SurfaceProperties> wallsMid;
        private List<SurfaceProperties> wallsBottom;

        private List<List<SurfaceProperties>> walls;
        private List<List<SurfaceProperties>> ceilingsAndFloors;

        private List<SurfaceProperties> updateList; //list of sectors to update

        private CheckBox[] wallFlags;
        private CheckBox[] sectorFlags;

        private List<string> renderStyles;

        private static bool relativeMode = true;

        public UDMFControlsForm() {
            //capture keys
            KeyPreview = true;
            
            //initialize form
            InitializeComponent();
            
            //create collections
            floors = new List<SurfaceProperties>();
            ceilings = new List<SurfaceProperties>();
            wallsTop = new List<SurfaceProperties>();
            wallsMid = new List<SurfaceProperties>();
            wallsBottom = new List<SurfaceProperties>();

            walls = new List<List<SurfaceProperties>>() { wallsTop, wallsMid, wallsBottom };
            ceilingsAndFloors = new List<List<SurfaceProperties>>() { ceilings, floors };

            updateList = new List<SurfaceProperties>();

            wallFlags = new CheckBox[] { cbnodecals, cbnofakecontrast, cbclipmidtex, cbsmoothlighting };
            sectorFlags = new CheckBox[] { cbsilent, cbnofallingdamage, cbdropactors, cbnorespawn, cbhidden };

            renderStyles = new List<string>() { "translucent", "add" };

            KeyDown += new KeyEventHandler(UDMFControlsForm_KeyDown);
            KeyUp += new KeyEventHandler(UDMFControlsForm_KeyUp);

            cbRelativeMode.Checked = relativeMode;
        }

        //we should be in Visual mode and should have some surfaces selected at this point
        public void Setup(List<VisualGeometry> surfaces) {
            //create undo
            string rest = surfaces.Count + " surface" + (surfaces.Count > 1 ? "s" : "");
            General.Map.UndoRedo.CreateUndo("Edit texture properties of " + rest);

            //get default values
            List<UniversalFieldInfo> defaultSidedefFields = General.Map.Config.SidedefFields;
            List<UniversalFieldInfo> defaultLinedefFields = General.Map.Config.LinedefFields;
            List<UniversalFieldInfo> defaultSectorFields = General.Map.Config.SectorFields;

            SurfaceProperties firstWall = null;
            SurfaceProperties firstFloor = null; //or ceiling!

            List<int> wall3DIndeces = new List<int>();
            SurfaceProperties sp;
            int walls3dCount = 0;

            //sort things
            foreach (VisualGeometry vg in surfaces) {
                sp = new SurfaceProperties(vg);
                updateList.Add(sp);

                switch (vg.GeometryType) {
                    case VisualGeometryType.CEILING:
                        if (firstFloor == null) firstFloor = sp;
                        ceilings.Add(sp);
                        setDefaultUniversalProperties(sp.Sector.Fields, defaultSectorFields);
                        break;

                    case VisualGeometryType.FLOOR:
                        if (firstFloor == null) firstFloor = sp;
                        floors.Add(sp);
                        setDefaultUniversalProperties(sp.Sector.Fields, defaultSectorFields);
                        break;

                    case VisualGeometryType.WALL_BOTTOM:
                        if (firstWall == null) firstWall = sp;
                        wallsBottom.Add(sp);
                        setDefaultUniversalProperties(sp.Sidedef.Fields, defaultSidedefFields);
                        setDefaultUniversalProperties(sp.Linedef.Fields, defaultLinedefFields);
                        break;

                    case VisualGeometryType.WALL_MIDDLE_3D:
                        walls3dCount++;
                        //if (wall3DIndeces.IndexOf(vg.Sector.Sector.FixedIndex) != -1)
                            //break;
                        wall3DIndeces.Add(vg.Sector.Sector.FixedIndex);
                        goto case VisualGeometryType.WALL_MIDDLE;

                    case VisualGeometryType.WALL_MIDDLE:
                        if (firstWall == null) firstWall = sp;
                        wallsMid.Add(sp);
                        setDefaultUniversalProperties(sp.Sidedef.Fields, defaultSidedefFields);
                        setDefaultUniversalProperties(sp.Linedef.Fields, defaultLinedefFields);

                        if (sp.HasControlLinedef)
                            setDefaultUniversalProperties(sp.ControlSidedef.Fields, defaultSidedefFields);
                        break;

                    case VisualGeometryType.WALL_UPPER:
                        if (firstWall == null) firstWall = sp;
                        wallsTop.Add(sp);
                        setDefaultUniversalProperties(sp.Sidedef.Fields, defaultSidedefFields);
                        setDefaultUniversalProperties(sp.Linedef.Fields, defaultLinedefFields);
                        break;

                    default: //dbg
                        GZBuilder.GZGeneral.Trace("WARNING: got unknown visual geometry type!");
                        break;
                }
            }

            //set sliders limits
            sliderDesaturation.SetLimits(0f, 1f, false);
            sliderAlpha.SetLimits(0f, 1f, false);
            scaleControl.SetLimits(-2f, 2f);

            cbRenderStyle.Items.AddRange(new object[] { "Translucent", "Add" });
            cbRenderStyle.SelectedIndex = 0;

            //set initial values to controls
            if (firstFloor != null) {
                //get values
                float scaleX = (float)firstFloor.Sector.Fields[KeyNames.GetScaleX(firstFloor.GeometryType)].Value;
                float scaleY = (float)firstFloor.Sector.Fields[KeyNames.GetScaleY(firstFloor.GeometryType)].Value;
                float translateX = (float)firstFloor.Sector.Fields[KeyNames.GetTranslationX(firstFloor.GeometryType)].Value;
                float translateY = (float)firstFloor.Sector.Fields[KeyNames.GetTranslationY(firstFloor.GeometryType)].Value;

                //set shared and sector flags
                cblightabsolute.Checked = (bool)firstFloor.Sector.Fields[KeyNames.GetLightAbsolute(firstFloor.GeometryType)].Value;
                
                foreach(CheckBox cb in sectorFlags)
                    cb.Checked = (bool)firstFloor.Sector.Fields[(string)cb.Tag].Value;

                //set values to controls
                scaleControl.Value = new Vector2D(scaleX, scaleY);
                positionControl1.Value = new Vector2D(translateX, translateY);
                angleControl1.Value = (int)((float)firstFloor.Sector.Fields[KeyNames.GetRotation(firstFloor.GeometryType)].Value);
                sliderBrightness.Value = (int)firstFloor.Sector.Fields[KeyNames.GetLight(firstFloor.GeometryType)].Value;
                nudGravity.Value = (decimal)((float)firstFloor.Sector.Fields[(string)nudGravity.Tag].Value);
                sliderDesaturation.Value = (float)firstFloor.Sector.Fields[(string)sliderDesaturation.Tag].Value;

            } else {//disable floor/ceiling related controls
                gbRotation.Enabled = false;
                gbFlagsFloor.Enabled = false;
                nudGravity.Enabled = false;
                labelGravity.Enabled = false;
                gbDesaturation.Enabled = false;
            }

            if (firstWall != null) {
                if(firstFloor == null){ //get shared values from wall
                    //get values
                    float scaleX = (float)firstWall.ControlSidedef.Fields[KeyNames.GetScaleX(firstWall.GeometryType)].Value;
                    float scaleY = (float)firstWall.ControlSidedef.Fields[KeyNames.GetScaleY(firstWall.GeometryType)].Value;
                    float translateX = (float)firstWall.Sidedef.Fields[KeyNames.GetTranslationX(firstWall.GeometryType)].Value;
                    float translateY = (float)firstWall.Sidedef.Fields[KeyNames.GetTranslationY(firstWall.GeometryType)].Value;

                    //set values to controls
                    scaleControl.Value = new Vector2D(scaleX, scaleY);
                    positionControl1.Value = new Vector2D(translateX, translateY);
                    sliderBrightness.Value = (int)firstWall.Sidedef.Fields[KeyNames.GetLight(firstWall.GeometryType)].Value;
                    cblightabsolute.Checked = (bool)firstWall.Sidedef.Fields[KeyNames.GetLightAbsolute(firstWall.GeometryType)].Value;

                    //set linedef values
                    sliderAlpha.Value = (float)firstWall.Linedef.Fields[(string)sliderAlpha.Tag].Value;
                    string renderStyle = (string)firstWall.Linedef.Fields[(string)cbRenderStyle.Tag].Value;
                    cbRenderStyle.SelectedIndex = renderStyles.IndexOf(renderStyle);

                    //if we got only 3d-walls selected, disable controls, which don't affect those
                    if (walls3dCount == wallsMid.Count && wallsTop.Count == 0 && wallsBottom.Count == 0) {
                        gbAlpha.Enabled = false;
                        bgBrightness.Enabled = false;
                        //cblightabsolute.Checked = true;
                        //cblightabsolute.Enabled = false;
                    }
                }

                //set wall flags
                foreach(CheckBox cb in wallFlags)
                    cb.Checked = (bool)firstWall.Sidedef.Fields[(string)cb.Tag].Value;

            } else { //disable wall-related controls
                gbFlagsWall.Enabled = false;
                gbAlpha.Enabled = false;
            }

            //brightness slider
            if(cblightabsolute.Checked)
                sliderBrightness.SetLimits(0, 255);
            else
                sliderBrightness.SetLimits(-255, 255);

            Text = "Editing " + rest;
        }

        private void setDefaultUniversalProperties(UniFields fields, List<UniversalFieldInfo> defaultFields) {
            foreach (UniversalFieldInfo info in defaultFields) {
                if (!fields.ContainsKey(info.Name))
                    fields.Add(info.Name, new UniValue(info.Type, (UniversalType)info.Type == UniversalType.Integer ? (object)Convert.ToInt32(info.Default, CultureInfo.InvariantCulture) : info.Default));
            }
        }

        private void removeDefaultUniversalProperties(UniFields fields, List<UniversalFieldInfo> defaultFields) {
            foreach (UniversalFieldInfo info in defaultFields) {
                if (fields.ContainsKey(info.Name) && fields[info.Name].Value.Equals((UniversalType)info.Type == UniversalType.Integer ? (object)Convert.ToInt32(info.Default, CultureInfo.InvariantCulture) : info.Default))
                    fields.Remove(info.Name);
            }
        }

        private void removeDefaultValues() {
            //remove default values...
            List<UniversalFieldInfo> defaultSidedefFields = General.Map.Config.SidedefFields;
            List<UniversalFieldInfo> defaultLinedefFields = General.Map.Config.LinedefFields;
            List<UniversalFieldInfo> defaultSectorFields = General.Map.Config.SectorFields;

            //...from floors/ceilings...
            foreach (List<SurfaceProperties> list in ceilingsAndFloors) {
                foreach (SurfaceProperties floor in list)
                    removeDefaultUniversalProperties(floor.Sector.Fields, defaultSectorFields);
            }

            //...and walls
            foreach (List<SurfaceProperties> list in walls) {
                foreach (SurfaceProperties wall in list) {
                    removeDefaultUniversalProperties(wall.Sidedef.Fields, defaultSidedefFields);
                    removeDefaultUniversalProperties(wall.Linedef.Fields, defaultLinedefFields);

                    if(wall.HasControlLinedef)
                        removeDefaultUniversalProperties(wall.ControlSidedef.Fields, defaultSidedefFields);
                }
            }
        }

//update view
        private void update() {
            foreach (SurfaceProperties sp in updateList)
                sp.Update();
        }

//shared props
        private void setSharedProperty(string propName, object value) {
            setSidedefProperty(propName, value);
            setSectorProperty(propName, value);
        }

        private void setSharedPairedProperty(string propName, Vector2D value) {
            setPairedSectorProperty(propName, value);
            setPairedSidedefProperty(propName, value);
        }

//linedef props
        private void setLinedefProperty(string propName, object value) {
            foreach (List<SurfaceProperties> list in walls) {
                foreach (SurfaceProperties vg in list)
                    vg.Linedef.Fields[propName].Value = value;
            }
        }

//sidedef props
        private void setSidedefProperty(string propName, object value) {
            //special cases
            if (propName == "scale") {
                General.Fail("use SetSidedefScale instead!");
                return;
            }else if(propName == "offset"){
                setPairedSidedefProperty(propName, (Vector2D)value);
                return;
            }

            //apply value
            /*if (propName == "light" && !cblightabsolute.Checked) {
                int light = (int)value > 0 ? (int)value : 0;
                
                //3d walls are lit as if "absolutelighting" flag is always set
                foreach (List<SurfaceProperties> list in walls) {
                    foreach (SurfaceProperties vg in list) {
                        if (vg.GeometryType == VisualGeometryType.WALL_MIDDLE_3D)
                            vg.Sidedef.Fields[propName].Value = light;
                        else
                            vg.Sidedef.Fields[propName].Value = value;
                    }
                }
            } else {*/
                foreach (List<SurfaceProperties> list in walls) {
                    foreach (SurfaceProperties vg in list) {
                        if ((propName == "light" || propName == "lightabsolute") && vg.GeometryType == VisualGeometryType.WALL_MIDDLE_3D) //just... skip it for now
                            continue;
                        vg.Sidedef.Fields[propName].Value = value;
                    }
                }
            //}
        }

        private void setPairedSidedefProperty(string propName, Vector2D value) {
            if (propName == "scale") General.Fail("use SetScale instead!"); //dbg
            if (propName != "offset") return;

            string upperNameX  = "x_top";
            string upperNameY  = "y_top";
            string middleNameX = "x_mid";
            string middleNameY = "y_mid";
            string lowerNameX  = "x_bottom";
            string lowerNameY  = "y_bottom";

            string[] props = new string[] { upperNameX, upperNameY, middleNameX, middleNameY, lowerNameX, lowerNameY };

            for (int i = 0; i < props.Length; i++ )
                props[i] = propName + props[i];

            int index = 0;
            
            //apply values
            if (relativeMode) {
                float val;
                foreach (List<SurfaceProperties> list in walls) { //top -> middle -> bottom
                    foreach (SurfaceProperties vg in list) {
                        val = (float)vg.Sidedef.Fields[props[index]].Value + value.x;
                        vg.Sidedef.Fields[props[index]].Value = val;

                        val = (float)vg.Sidedef.Fields[props[index + 1]].Value + value.y;
                        vg.Sidedef.Fields[props[index + 1]].Value = val;
                    }
                    index += 2;
                }
            } else {
                foreach (List<SurfaceProperties> list in walls) { //top -> middle -> bottom
                    foreach (SurfaceProperties vg in list) {
                        vg.Sidedef.Fields[props[index]].Value = value.x;
                        vg.Sidedef.Fields[props[index + 1]].Value = value.y;
                    }
                    index += 2;
                }
            }
        }

//floor/ceiling props
        private void setSectorProperty(string propName, object value) {
            //special cases
            if (propName == "scale" || propName == "offset") {
                setPairedSectorProperty(propName, (Vector2D)value);
                return;
            } else if (propName == "light" || propName == "lightabsolute" || propName == "rotation") {
                string propFloor;
                string propCeiling;

                if (propName == "light") {
                    propFloor = "lightfloor";
                    propCeiling = "lightceiling";
                } else if (propName == "lightabsolute") {
                    propFloor = "lightfloorabsolute";
                    propCeiling = "lightceilingabsolute";
                } else {
                    propFloor = "rotationfloor";
                    propCeiling = "rotationceiling";
                }

                if (propName == "rotation" && relativeMode) {
                    float val;

                    foreach (SurfaceProperties vg in floors) {
                        val = (float)vg.Sector.Fields[propFloor].Value + (float)value;
                        vg.Sector.Fields[propFloor].Value = (object)val;
                    }

                    foreach (SurfaceProperties vg in ceilings) {
                        val = (float)vg.Sector.Fields[propCeiling].Value + (float)value;
                        vg.Sector.Fields[propCeiling].Value = (object)val;
                    }
                } else {
                    foreach (SurfaceProperties vg in floors)
                        vg.Sector.Fields[propFloor].Value = value;

                    foreach (SurfaceProperties vg in ceilings)
                        vg.Sector.Fields[propCeiling].Value = value;
                }
                return;
            }
            //apply values
            foreach (List<SurfaceProperties> list in ceilingsAndFloors) {
                foreach (SurfaceProperties vg in list)
                    vg.Sector.Fields[propName].Value = value;
            }
        }

        private void setPairedSectorProperty(string propName, Vector2D value) {
            if (propName != "scale" && propName != "offset")
                return;

            string ceilingNameX, ceilingNameY, floorNameX, floorNameY;
            string[] props;

            if (propName == "scale") {
                ceilingNameX = "xscaleceiling";
                ceilingNameY = "yscaleceiling";
                floorNameX = "xscalefloor";
                floorNameY = "yscalefloor";
            } else {
                ceilingNameX = "xpanningceiling";
                ceilingNameY = "ypanningceiling";
                floorNameX = "xpanningfloor";
                floorNameY = "ypanningfloor";
            }
            props = new string[] { ceilingNameX, ceilingNameY, floorNameX, floorNameY };
            int index = 0;

            //apply values
            if (relativeMode) {
                float val;
                foreach (List<SurfaceProperties> list in ceilingsAndFloors) { //ceilings -> floors
                    foreach (SurfaceProperties vg in list) {
                        val = (float)vg.Sector.Fields[props[index]].Value + value.x;
                        vg.Sector.Fields[props[index]].Value = (object)val;

                        val = (float)vg.Sector.Fields[props[index + 1]].Value + value.y;
                        vg.Sector.Fields[props[index + 1]].Value = (object)val;
                    }
                    index += 2;
                }
            } else {
                foreach (List<SurfaceProperties> list in ceilingsAndFloors) { //ceilings -> floors
                    foreach (SurfaceProperties vg in list) {
                        vg.Sector.Fields[props[index]].Value = value.x;
                        vg.Sector.Fields[props[index + 1]].Value = value.y;
                    }
                    index += 2;
                }
            }
        }

        private void setSidedefScale(Vector2D value) {
            string[] props = new string[] { "scalex_top", "scaley_top", "scalex_mid", "scaley_mid", "scalex_bottom", "scaley_bottom" };
            List<int> controlSectors = new List<int>();
            int index = 0;

            //apply values
            if (relativeMode) {
                float val;
                foreach (List<SurfaceProperties> list in walls) { //top -> middle -> bottom
                    foreach (SurfaceProperties vg in list) {
                        if (vg.HasControlLinedef) {
                            if (controlSectors.Contains(vg.ControlSidedef.Sector.FixedIndex))
                                continue;

                            val = (float)vg.ControlSidedef.Fields[props[index]].Value + value.x;
                            vg.ControlSidedef.Fields[props[index]].Value = val;

                            val = (float)vg.ControlSidedef.Fields[props[index + 1]].Value + value.y;
                            vg.ControlSidedef.Fields[props[index + 1]].Value = val;

                            controlSectors.Add(vg.ControlSidedef.Sector.FixedIndex);
                        } else {
                            val = (float)vg.Sidedef.Fields[props[index]].Value + value.x;
                            vg.Sidedef.Fields[props[index]].Value = val;

                            val = (float)vg.Sidedef.Fields[props[index + 1]].Value + value.y;
                            vg.Sidedef.Fields[props[index + 1]].Value = val;
                        }
                    }
                    index += 2;
                }
            } else {
                foreach (List<SurfaceProperties> list in walls) { //top -> middle -> bottom
                    foreach (SurfaceProperties vg in list) {
                        if (vg.HasControlLinedef) {
                            if (controlSectors.Contains(vg.ControlSidedef.Sector.FixedIndex))
                                continue;

                            vg.ControlSidedef.Fields[props[index]].Value = value.x;
                            vg.ControlSidedef.Fields[props[index + 1]].Value = value.y;

                            controlSectors.Add(vg.ControlSidedef.Sector.FixedIndex);
                        } else {
                            vg.Sidedef.Fields[props[index]].Value = value.x;
                            vg.Sidedef.Fields[props[index + 1]].Value = value.y;
                        }
                    }
                    index += 2;
                }
            }
        }
        
//EVENTS
        private void btnOK_Click(object sender, EventArgs e) {
            //apply flags and settings, which are not updated in real time
            //gravity
            setSectorProperty((string)nudGravity.Tag, (object)((float)nudGravity.Value));
            //desaturation
            setSectorProperty((string)sliderDesaturation.Tag, (object)sliderDesaturation.Value);

            //wall flags
            foreach (CheckBox cb in wallFlags) 
                setSidedefProperty((string)cb.Tag, (object)cb.Checked);

            //sector flags
            foreach (CheckBox cb in sectorFlags)
                setSectorProperty((string)cb.Tag, (object)cb.Checked);

            //update sectors
            foreach (SurfaceProperties vs in updateList) {
                if (vs.Sector != null) {
                    vs.Sector.UpdateNeeded = true;
                    vs.Sector.UpdateCache();
                }
            }

            removeDefaultValues();
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e) {
            //remove default values...
            removeDefaultValues();

            //restore initial values
            General.Map.UndoRedo.PerformUndo();
            Close();
        }

//KEYBOARD EVENTS
        private void UDMFControlsForm_KeyDown(object sender, KeyEventArgs e) {
            if (e.Shift) {
                angleControl1.SnapAngle = e.Shift;
                e.Handled = true;
            }
        }

        private void UDMFControlsForm_KeyUp(object sender, KeyEventArgs e) {
            if (e.Shift) {
                angleControl1.SnapAngle = !e.Shift;
                e.Handled = true;
            }
        }

//INTERACTIVE CONTROLS
//position
        private void positionControl1_OnValueChanged(object sender, EventArgs e) {
            setSharedPairedProperty((string)positionControl1.Tag, relativeMode ? positionControl1.Delta : positionControl1.Value);
            update();
        }

//rotation
        private void angleControl1_OnAngleChanged(object sender, EventArgs e) {
            setSectorProperty((string)angleControl1.Tag, relativeMode ? (float)angleControl1.Delta : (float)angleControl1.Value);
            update();
        }

//scale
        private void scaleControl_OnValueChanged(object sender, EventArgs e) {
            setPairedSectorProperty((string)scaleControl.Tag, relativeMode ? scaleControl.Delta : scaleControl.Value);
            setSidedefScale(relativeMode ? scaleControl.Delta : scaleControl.Value);
            update();
        }

//brightness
        private void sliderBrightness_OnValueChanged(object sender, EventArgs e) {
            setSharedProperty((string)sliderBrightness.Tag, (object)sliderBrightness.Value);
            update();
        }

//alpha
        private void sliderAlpha_OnValueChanged(object sender, EventArgs e) {
            setLinedefProperty((string)sliderAlpha.Tag, sliderAlpha.Value);
            update();
        }

        private void cbRenderStyle_SelectedIndexChanged(object sender, EventArgs e) {
            string val = cbRenderStyle.Text.ToLowerInvariant();
            setLinedefProperty((string)cbRenderStyle.Tag, val);
            update();
        }

//flags
        private void cbwrapmidtex_CheckedChanged(object sender, EventArgs e) {
            setSidedefProperty((string)cbwrapmidtex.Tag, (object)cbwrapmidtex.Checked);
            update();
        }

        private void cblightabsolute_CheckedChanged(object sender, EventArgs e) {
            if(cblightabsolute.Checked)
                sliderBrightness.SetLimits(0, 255);
            else
                sliderBrightness.SetLimits(-255, 255);
            
            setSharedProperty((string)cblightabsolute.Tag, (object)cblightabsolute.Checked);
            update();
        }

        private void cbRelativeMode_CheckedChanged(object sender, EventArgs e) {
            relativeMode = cbRelativeMode.Checked;
        }

        private void UDMFControlsForm_HelpRequested(object sender, HelpEventArgs hlpevent) {
            General.ShowHelp("gz_plug_udmfcontrols.html");
            hlpevent.Handled = true;
        }
    }

    internal sealed class SurfaceProperties {
        private Sector sector;
        public Sector Sector { get { return sector; } }

        private Sidedef controlSidedef;
        public Sidedef ControlSidedef { get { return controlSidedef; } }

        private Sidedef sidedef;
        public Sidedef Sidedef { get { return sidedef; } }

        private Linedef linedef;
        public Linedef Linedef { get { return linedef; } }

        public VisualGeometryType GeometryType { get { return vg.GeometryType; } }
        private VisualGeometry vg;

        private bool hasControlLinedef;
        public bool HasControlLinedef { get { return hasControlLinedef; } }
        
        public SurfaceProperties(VisualGeometry visualGeometry) {
            vg = visualGeometry;
            if (vg.GeometryType == VisualGeometryType.CEILING || vg.GeometryType == VisualGeometryType.FLOOR) {
                sector = vg.GetControlSector();
                sector.Fields.BeforeFieldsChange();
            } else {
                linedef = vg.GetControlLinedef();
                
                controlSidedef = linedef.Front;
                sidedef = vg.Sidedef;

                hasControlLinedef = (controlSidedef.Sector.FixedIndex != sidedef.Sector.FixedIndex);

                linedef.Fields.BeforeFieldsChange();
                controlSidedef.Fields.BeforeFieldsChange();
                sidedef.Fields.BeforeFieldsChange();
            }
        }

        public void Update() {
            vg.Sector.UpdateSectorData();
        }

    }

    internal sealed class KeyNames {
//SCALE        
        public static string GetScaleX(VisualGeometryType type) {
            return getScale(type).Replace("$", "x");
        }

        public static string GetScaleY(VisualGeometryType type) {
            return getScale(type).Replace("$", "y");
        }

        private static string getScale(VisualGeometryType type) {
            switch(type){
                case VisualGeometryType.CEILING:
                    return "$scaleceiling";
                    break;

                case VisualGeometryType.FLOOR:
                    return "$scalefloor";
                    break;

                case VisualGeometryType.WALL_UPPER:
                    return "scale$_top";
                    break;

                case VisualGeometryType.WALL_MIDDLE_3D:
                case VisualGeometryType.WALL_MIDDLE:
                    return "scale$_mid";
                    break;

                case VisualGeometryType.WALL_BOTTOM:
                    return "scale$_bottom";
                    break;
            }
            return "";
        }

//TRANSLATION
        public static string GetTranslationX(VisualGeometryType type) {
            return getTranslation(type).Replace("$", "x");
        }

        public static string GetTranslationY(VisualGeometryType type) {
            return getTranslation(type).Replace("$", "y");
        }

        private static string getTranslation(VisualGeometryType type) {
            switch (type) {
                case VisualGeometryType.CEILING:
                    return "$panningceiling";
                    break;

                case VisualGeometryType.FLOOR:
                    return "$panningfloor";
                    break;

                case VisualGeometryType.WALL_UPPER:
                    return "offset$_top";
                    break;

                case VisualGeometryType.WALL_MIDDLE_3D:
                case VisualGeometryType.WALL_MIDDLE:
                    return "offset$_mid";
                    break;

                case VisualGeometryType.WALL_BOTTOM:
                    return "offset$_bottom";
                    break;
            }
            return "";
        }

//ROTATION
        public static string GetRotation(VisualGeometryType type) {
            switch (type) {
                case VisualGeometryType.FLOOR:
                    return "rotationfloor";
                    break;

                case VisualGeometryType.CEILING:
                    return "rotationceiling";
                    break;
            }
            return "";
        }

//LIGHT
        public static string GetLight(VisualGeometryType type) {
            switch (type) {
                case VisualGeometryType.FLOOR:
                    return "lightfloor";
                    break;

                case VisualGeometryType.CEILING:
                    return "lightceiling";
                    break;

                case VisualGeometryType.WALL_BOTTOM:
                case VisualGeometryType.WALL_MIDDLE:
                case VisualGeometryType.WALL_MIDDLE_3D:
                case VisualGeometryType.WALL_UPPER:
                    return "light";
                    break;
            }
            return "";
        }

        public static string GetLightAbsolute(VisualGeometryType type) {
            switch (type) {
                case VisualGeometryType.FLOOR:
                    return "lightfloorabsolute";
                    break;

                case VisualGeometryType.CEILING:
                    return "lightceilingabsolute";
                    break;

                case VisualGeometryType.WALL_BOTTOM:
                case VisualGeometryType.WALL_MIDDLE:
                case VisualGeometryType.WALL_MIDDLE_3D:
                case VisualGeometryType.WALL_UPPER:
                    return "lightabsolute";
                    break;
            }
            return "";
        }
    }
}
