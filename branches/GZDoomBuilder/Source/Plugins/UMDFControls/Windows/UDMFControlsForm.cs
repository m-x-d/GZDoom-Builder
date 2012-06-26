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

namespace CodeImp.DoomBuilder.UDMFControls
{
    public partial class UDMFControlsForm : DelayedForm {
        private List<VisualGeometry> floors;
        private List<VisualGeometry> ceilings;

        private List<VisualGeometry> wallsTop;
        private List<VisualGeometry> wallsMid;
        private List<VisualGeometry> wallsBottom;

        private List<List<VisualGeometry>> walls;
        private List<List<VisualGeometry>> ceilingsAndFloors;

        private List<VisualSector> updateList; //list of sectors to update

        private CheckBox[] wallFlags;
        private CheckBox[] sectorFlags;

        private List<string> renderStyles;

        private static bool relativeMode;

        public UDMFControlsForm() {
            //capture keys
            KeyPreview = true;
            
            //initialize form
            InitializeComponent();
            
            //create collections
            floors = new List<VisualGeometry>();
            ceilings = new List<VisualGeometry>();
            wallsTop = new List<VisualGeometry>();
            wallsMid = new List<VisualGeometry>();
            wallsBottom = new List<VisualGeometry>();

            walls = new List<List<VisualGeometry>>() { wallsTop, wallsMid, wallsBottom };
            ceilingsAndFloors = new List<List<VisualGeometry>>() { ceilings, floors };
            
            updateList = new List<VisualSector>();

            wallFlags = new CheckBox[] { cbnodecals, cbnofakecontrast, cbclipmidtex, cbsmoothlighting };
            sectorFlags = new CheckBox[] { cbsilent, cbnofallingdamage, cbdropactors, cbnorespawn };

            renderStyles = new List<string>() { "translucent", "add" };

            KeyDown += new KeyEventHandler(UDMFControlsForm_KeyDown);
            KeyUp += new KeyEventHandler(UDMFControlsForm_KeyUp);

            cbRelativeMode.Checked = relativeMode;

            setup();
        }

        //we should be in Visual mode and should have some surfaces selected at this point
        private void setup() {
            VisualMode vm = (VisualMode)General.Editing.Mode;

            //should contain something, otherwise we wouldn't be here
            List<VisualGeometry> surfaces = vm.GetSelectedSurfaces(false);

            //create undo
            string rest = surfaces.Count + " surface" + (surfaces.Count > 1 ? "s" : "");
            General.Map.UndoRedo.CreateUndo("Edit texture properties of " + rest);

            //get default values
            List<UniversalFieldInfo> defaultSidedefFields = General.Map.Config.SidedefFields;
            List<UniversalFieldInfo> defaultLinedefFields = General.Map.Config.LinedefFields;
            List<UniversalFieldInfo> defaultSectorFields = General.Map.Config.SectorFields;

            VisualGeometry firstWall = null;
            VisualGeometry firstFloor = null; //or ceiling!

            List<int> sectorIndeces = new List<int>();

            //sort things
            foreach (VisualGeometry vg in surfaces) {
                if (sectorIndeces.IndexOf(vg.Sector.Sector.FixedIndex) == -1) {
                    updateList.Add(vg.Sector);
                    sectorIndeces.Add(vg.Sector.Sector.FixedIndex);
                }
                
                switch (vg.GeometryType) {
                    case VisualGeometryType.CEILING:
                        if (firstFloor == null) firstFloor = vg;
                        ceilings.Add(vg);
                        vg.Sector.Sector.Fields.BeforeFieldsChange();
                        setDefaultUniversalProperties(vg.Sector.Sector.Fields, defaultSectorFields);
                        break;

                    case VisualGeometryType.FLOOR:
                        if (firstFloor == null) firstFloor = vg;
                        floors.Add(vg);
                        vg.Sector.Sector.Fields.BeforeFieldsChange();
                        setDefaultUniversalProperties(vg.Sector.Sector.Fields, defaultSectorFields);
                        break;

                    case VisualGeometryType.WALL_BOTTOM:
                        if (firstWall == null) firstWall = vg;
                        wallsBottom.Add(vg);
                        vg.Sidedef.Fields.BeforeFieldsChange();
                        vg.Sidedef.Line.Fields.BeforeFieldsChange();
                        setDefaultUniversalProperties(vg.Sidedef.Fields, defaultSidedefFields);
                        setDefaultUniversalProperties(vg.Sidedef.Line.Fields, defaultLinedefFields);
                        break;

                    case VisualGeometryType.WALL_MIDDLE:
                        if (firstWall == null) firstWall = vg;
                        wallsMid.Add(vg);
                        vg.Sidedef.Fields.BeforeFieldsChange();
                        vg.Sidedef.Line.Fields.BeforeFieldsChange();
                        setDefaultUniversalProperties(vg.Sidedef.Fields, defaultSidedefFields);
                        setDefaultUniversalProperties(vg.Sidedef.Line.Fields, defaultLinedefFields);
                        break;

                    case VisualGeometryType.WALL_UPPER:
                        if (firstWall == null) firstWall = vg;
                        wallsTop.Add(vg);
                        vg.Sidedef.Fields.BeforeFieldsChange();
                        vg.Sidedef.Line.Fields.BeforeFieldsChange();
                        setDefaultUniversalProperties(vg.Sidedef.Fields, defaultSidedefFields);
                        setDefaultUniversalProperties(vg.Sidedef.Line.Fields, defaultLinedefFields);
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
                float scaleX = (float)firstFloor.Sector.Sector.Fields[KeyNames.GetScaleX(firstFloor.GeometryType)].Value;
                float scaleY = (float)firstFloor.Sector.Sector.Fields[KeyNames.GetScaleY(firstFloor.GeometryType)].Value;
                float translateX = (float)firstFloor.Sector.Sector.Fields[KeyNames.GetTranslationX(firstFloor.GeometryType)].Value;
                float translateY = (float)firstFloor.Sector.Sector.Fields[KeyNames.GetTranslationY(firstFloor.GeometryType)].Value;

                //set shared and sector flags
                cblightabsolute.Checked = (bool)firstFloor.Sector.Sector.Fields[KeyNames.GetLightAbsolute(firstFloor.GeometryType)].Value;
                
                foreach(CheckBox cb in sectorFlags)
                    cb.Checked = (bool)firstFloor.Sector.Sector.Fields[(string)cb.Tag].Value;

                //set values to controls
                scaleControl.Value = new Vector2D(scaleX, scaleY);
                positionControl1.Value = new Vector2D(translateX, translateY);
                angleControl1.Value = (int)((float)firstFloor.Sector.Sector.Fields[KeyNames.GetRotation(firstFloor.GeometryType)].Value);
                sliderBrightness.Value = (int)firstFloor.Sector.Sector.Fields[KeyNames.GetLight(firstFloor.GeometryType)].Value;
                nudGravity.Value = (decimal)((float)firstFloor.Sector.Sector.Fields[(string)nudGravity.Tag].Value);
                sliderDesaturation.Value = (float)firstFloor.Sector.Sector.Fields[(string)sliderDesaturation.Tag].Value;

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
                    float scaleX = (float)firstWall.Sidedef.Fields[KeyNames.GetScaleX(firstWall.GeometryType)].Value;
                    float scaleY = (float)firstWall.Sidedef.Fields[KeyNames.GetScaleY(firstWall.GeometryType)].Value;
                    float translateX = (float)firstWall.Sidedef.Fields[KeyNames.GetTranslationX(firstWall.GeometryType)].Value;
                    float translateY = (float)firstWall.Sidedef.Fields[KeyNames.GetTranslationY(firstWall.GeometryType)].Value;

                    //set values to controls
                    scaleControl.Value = new Vector2D(scaleX, scaleY);
                    positionControl1.Value = new Vector2D(translateX, translateY);
                    sliderBrightness.Value = (int)firstWall.Sidedef.Fields[KeyNames.GetLight(firstWall.GeometryType)].Value;
                    cblightabsolute.Checked = (bool)firstWall.Sidedef.Fields[KeyNames.GetLightAbsolute(firstWall.GeometryType)].Value;

                    //set linedef values
                    sliderAlpha.Value = (float)firstWall.Sidedef.Line.Fields[(string)sliderAlpha.Tag].Value;
                    string renderStyle = (string)firstWall.Sidedef.Line.Fields[(string)cbRenderStyle.Tag].Value;
                    cbRenderStyle.SelectedIndex = renderStyles.IndexOf(renderStyle);
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
                    fields.Add(info.Name, new UniValue(info.Type, (UniversalType)info.Type == UniversalType.Integer ? (object)Convert.ToInt32(info.Default) : info.Default));
            }
        }

        private void removeDefaultUniversalProperties(UniFields fields, List<UniversalFieldInfo> defaultFields) {
            foreach (UniversalFieldInfo info in defaultFields) {
                if (fields.ContainsKey(info.Name) && fields[info.Name].Value.Equals((UniversalType)info.Type == UniversalType.Integer ? (object)Convert.ToInt32(info.Default) : info.Default))
                    fields.Remove(info.Name);
            }
        }

        private void removeDefaultValues() {
            //remove default values...
            List<UniversalFieldInfo> defaultSidedefFields = General.Map.Config.SidedefFields;
            List<UniversalFieldInfo> defaultLinedefFields = General.Map.Config.LinedefFields;
            List<UniversalFieldInfo> defaultSectorFields = General.Map.Config.SectorFields;

            //...from floors/ceilings...
            foreach (List<VisualGeometry> list in ceilingsAndFloors) {
                foreach (VisualGeometry floor in list)
                    removeDefaultUniversalProperties(floor.Sector.Sector.Fields, defaultSectorFields);
            }

            //...and walls
            foreach (List<VisualGeometry> list in walls) {
                foreach (VisualGeometry wall in list) {
                    removeDefaultUniversalProperties(wall.Sidedef.Fields, defaultSidedefFields);
                    removeDefaultUniversalProperties(wall.Sidedef.Line.Fields, defaultLinedefFields);
                }
            }
        }

//update view
        private void update() {
            foreach (VisualSector vs in updateList)
                vs.UpdateSectorData();
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
            foreach (List<VisualGeometry> list in walls) {
                foreach (VisualGeometry vg in list)
                    vg.Sidedef.Line.Fields[propName].Value = value;
            }
        }

//sidedef props
        private void setSidedefProperty(string propName, object value) {
            //special cases
            if (propName == "scale" || propName == "offset") {
                setPairedSidedefProperty(propName, (Vector2D)value);
                return;
            }

            //apply value
            foreach (List<VisualGeometry> list in walls) {
                foreach (VisualGeometry vg in list)
                    vg.Sidedef.Fields[propName].Value = value;
            }
        }

        private void setPairedSidedefProperty(string propName, Vector2D value) {
            if (propName != "scale" && propName != "offset")
                return;

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
                foreach (List<VisualGeometry> list in walls) { //top -> middle -> bottom
                    foreach (VisualGeometry vg in list) {
                        val = (float)vg.Sidedef.Fields[props[index]].Value + value.x;
                        vg.Sidedef.Fields[props[index]].Value = val;

                        val = (float)vg.Sidedef.Fields[props[index+1]].Value + value.y;
                        vg.Sidedef.Fields[props[index + 1]].Value = val;
                    }
                    index += 2;
                }
            } else {
                foreach (List<VisualGeometry> list in walls) { //top -> middle -> bottom
                    foreach (VisualGeometry vg in list) {
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

                    foreach (VisualGeometry vg in floors) {
                        val = (float)vg.Sector.Sector.Fields[propFloor].Value + (float)value;
                        vg.Sector.Sector.Fields[propFloor].Value = (object)val;
                    }

                    foreach (VisualGeometry vg in ceilings) {
                        val = (float)vg.Sector.Sector.Fields[propCeiling].Value + (float)value;
                        vg.Sector.Sector.Fields[propCeiling].Value = (object)val;
                    }
                } else {
                    foreach (VisualGeometry vg in floors)
                        vg.Sector.Sector.Fields[propFloor].Value = value;

                    foreach (VisualGeometry vg in ceilings)
                        vg.Sector.Sector.Fields[propCeiling].Value = value;
                }
                return;
            }

            foreach (List<VisualGeometry> list in ceilingsAndFloors) {
                foreach (VisualGeometry vg in list)
                    vg.Sector.Sector.Fields[propName].Value = value;
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

            if (relativeMode) {
                float val;
                foreach (List<VisualGeometry> list in ceilingsAndFloors) { //ceilings -> floors
                    foreach (VisualGeometry vg in list) {
                        val = (float)vg.Sector.Sector.Fields[props[index]].Value + value.x;
                        vg.Sector.Sector.Fields[props[index]].Value = (object)val;

                        val = (float)vg.Sector.Sector.Fields[props[index + 1]].Value + value.y;
                        vg.Sector.Sector.Fields[props[index + 1]].Value = (object)val;
                    }
                    index += 2;
                }
            } else {
                foreach (List<VisualGeometry> list in ceilingsAndFloors) { //ceilings -> floors
                    foreach (VisualGeometry vg in list) {
                        vg.Sector.Sector.Fields[props[index]].Value = value.x;
                        vg.Sector.Sector.Fields[props[index + 1]].Value = value.y;
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
            foreach (VisualSector vs in updateList) {
                vs.Sector.UpdateNeeded = true;
                vs.Sector.UpdateCache();
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
            setSharedPairedProperty((string)scaleControl.Tag, relativeMode ? scaleControl.Delta : scaleControl.Value);
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
    }

    public class KeyNames {
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
                case VisualGeometryType.WALL_UPPER:
                    return "lightabsolute";
                    break;
            }
            return "";
        }
    }
}
