using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using CodeImp.DoomBuilder.Config;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Plugins;
using CodeImp.DoomBuilder.Windows;
using CodeImp.DoomBuilder.GZBuilder;
using CodeImp.DoomBuilder.VisualModes;
using CodeImp.DoomBuilder.ColorPicker.Controls;

namespace CodeImp.DoomBuilder.ColorPicker.Windows {
    public partial class LightColorPicker : DelayedForm, IColorPicker {
        public ColorPickerType Type { get { return ColorPickerType.CP_LIGHT; } }

        private static bool RELATIVE_MODE;
        
        private static int[] LIGHT_USES_ANGLE_VALUE = { 9801, 9802, 9804, 9811, 9812, 9814, 9821, 9822, 9824 };
        
        private List<Thing> selection;
        private List<VisualThing> visualSelection;
        private List<LightProps> fixedValues; //this gets filled with radius / intencity values before RELATIVE_MODE is engaged

        private string editingModeName;
        private bool showAllControls;

        private LightProps lightProps;
 
        //maximum and minimum values for sliders for relative and absolute modes
        //numericUpDown
        private const int NUD_ABS_MIN = 0;
        private const int NUD_ABS_MAX = 16384;

        private const int NUD_REL_MIN = -16384;
        private const int NUD_REL_MAX = 16384;

        //trackBar
        private const int TB_ABS_MIN = 0;
        private const int TB_ABS_MAX = 512;

        private const int TB_REL_MIN = -256;
        private const int TB_REL_MAX = 256;

        //trackBar controlling thing angle
        private const int TB_ANGLE_ABS_MIN = 0;
        private const int TB_ANGLE_ABS_MAX = 359;

        private const int TB_ANGLE_REL_MIN = -180;
        private const int TB_ANGLE_REL_MAX = 180;

        //initialise pannel
        public bool Setup(string editingModeName) {
            this.editingModeName = editingModeName;
            setupSelection();

            int selCount = selection.Count;
            if (selCount == 0) {
                General.Interface.DisplayStatus(StatusType.Warning, "No lights found in selection!");
                return false;
            }

            lightProps = new LightProps();

            //initialise
            InitializeComponent();

            setupSliders(selection[0]);
            updateLightPropsFromThing(selection[0]);
            setControlsMode();

            colorPickerControl1.Initialize(getThingColor(selection[0]));
            colorPickerControl1.ColorChanged += new EventHandler<ColorChangedEventArgs>(colorPickerControl1_ColorChanged);
            colorPickerControl1.OnOkPressed += new EventHandler(colorPickerControl1_OnOkPressed);
            colorPickerControl1.OnCancelPressed += new EventHandler(colorPickerControl1_OnCancelPressed);

            cbRelativeMode.Checked = RELATIVE_MODE;
            cbRelativeMode.CheckStateChanged += new EventHandler(cbRelativeMode_CheckStateChanged);

            this.AcceptButton = colorPickerControl1.OkButton;
            this.CancelButton = colorPickerControl1.CancelButton;
            this.Text = "Editing " + selCount + " light" + (selCount > 1 ? "s" : "");
            
            //undo
            makeUndo(this.Text);
            return true;
        }

        //create selection of lights. This is called only once
        private void setupSelection() {
            selection = new List<Thing>();

            //check things
            if (editingModeName == "BaseVisualMode") {
                visualSelection = new List<VisualThing>();
                List<VisualThing> selectedVisualThings = ((VisualMode)General.Editing.Mode).GetSelectedVisualThings(false);

                foreach (VisualThing t in selectedVisualThings) {
                    if (Array.IndexOf(GZGeneral.GZ_LIGHTS, t.Thing.Type) != -1) {
                        selection.Add(t.Thing);
                        visualSelection.Add(t);
                    }
                }
            } else {
                ICollection<Thing> list = General.Map.Map.GetSelectedThings(true);
                foreach (Thing t in list) {
                    if (Array.IndexOf(GZGeneral.GZ_LIGHTS, t.Type) != -1)
                        selection.Add(t);
                }
            }
        }

        //set sliders count and labels based on given thing, set lightProps start values
        //this is called only once
        private void setupSliders(Thing referenceThing) {
            ThingTypeInfo typeInfo = General.Map.Data.GetThingInfoEx(referenceThing.Type);
            int firstArg = 3;
            if (referenceThing.Type == 1502 || referenceThing.Type == 1503)
                firstArg = 0;

            //first slider is always used
            colorPickerSlider1.Label = typeInfo.Args[firstArg].Title + ":";
            colorPickerSlider1.OnValueChanged += new EventHandler<ColorPickerSliderEventArgs>(onSliderValueChanged);

            //either both of them or none are used
            if (Array.IndexOf(LIGHT_USES_ANGLE_VALUE, referenceThing.Type) != -1) {
                showAllControls = true;
                colorPickerSlider2.Label = typeInfo.Args[4].Title + ":";
                colorPickerSlider2.OnValueChanged += new EventHandler<ColorPickerSliderEventArgs>(onSliderValueChanged);
                
                colorPickerSlider3.Label = "Interval:";
                colorPickerSlider3.OnValueChanged += new EventHandler<ColorPickerSliderEventArgs>(onSliderValueChanged);
            } else {
                colorPickerSlider2.Visible = false;
                colorPickerSlider3.Visible = false;
            }

            //set window height
            int newHeight = 0;
            if (showAllControls) {
                newHeight = colorPickerSlider3.Location.Y + colorPickerSlider3.Height + 8;
            } else {
                newHeight = colorPickerSlider1.Location.Y + colorPickerSlider1.Height + 8;
            }

            this.ClientSize = new Size(this.ClientSize.Width, newHeight);
        }

        //this sets lightProps values from given thing
        private void updateLightPropsFromThing(Thing referenceThing) {
            //color
            Color c = getThingColor(referenceThing);
            lightProps.Red = c.R;
            lightProps.Green = c.G;
            lightProps.Blue = c.B;
            
            //size
            int firstArg = 3;
            if (referenceThing.Type == 1502 || referenceThing.Type == 1503)
                firstArg = 0;

            lightProps.PrimaryRadius = referenceThing.Args[firstArg];

            //either both of them or none are used
            if (showAllControls && Array.IndexOf(LIGHT_USES_ANGLE_VALUE, referenceThing.Type) != -1) {
                lightProps.SecondaryRadius = referenceThing.Args[4];
                lightProps.Interval = referenceThing.AngleDoom;
            }
        }

        //this sets lightProps values from sliders
        private void updateLightPropsFromSliders() {
            ColorHandler.RGB curColor = colorPickerControl1.CurrentColor;
            bool colorChanged = false; //need this check to allow relative mode to work properly

            if ((byte)curColor.Red != lightProps.Red || (byte)curColor.Green != lightProps.Green || (byte)curColor.Blue != lightProps.Blue) {
                lightProps.Red = (byte)curColor.Red;
                lightProps.Green = (byte)curColor.Green;
                lightProps.Blue = (byte)curColor.Blue;
                colorChanged = true;
            }

            lightProps.PrimaryRadius = colorPickerSlider1.Value;
            if (showAllControls) {
                lightProps.SecondaryRadius = colorPickerSlider2.Value;
                lightProps.Interval = colorPickerSlider3.Value;
            }
            updateSelection(colorChanged);
        }

        //this sets values from lightProps to things in selection
        private void updateSelection(bool colorChanged) {
            for (int i = 0; i < selection.Count; i++) {
                Thing t = selection[i];

                //update color 
                if (colorChanged) { //need this check to allow relative mode to work properly
                    if (t.Type == 1503) { //Vavoom Light Color
                        t.Args[1] = lightProps.Red;
                        t.Args[2] = lightProps.Green;
                        t.Args[3] = lightProps.Blue;
                    } else if (t.Type != 1502) { //vavoom light has no color settings
                        t.Args[0] = lightProps.Red;
                        t.Args[1] = lightProps.Green;
                        t.Args[2] = lightProps.Blue;
                    }
                }

                int firstArg = 3;

                if (t.Type == 1502 || t.Type == 1503)
                    firstArg = 0;

                //update radius and intensity
                if (RELATIVE_MODE) {
                    LightProps fixedVal = fixedValues[i];

                    t.Args[firstArg] = fixedVal.PrimaryRadius + lightProps.PrimaryRadius;
                    if (t.Args[firstArg] < 0) t.Args[firstArg] = 0;

                    if (showAllControls && Array.IndexOf(LIGHT_USES_ANGLE_VALUE, t.Type) != -1) {
                        t.Args[4] = fixedVal.SecondaryRadius + lightProps.SecondaryRadius;
                        if (t.Args[4] < 0) t.Args[4] = 0;

                        t.Rotate(General.ClampAngle(fixedVal.Interval + lightProps.Interval));
                    }
                } else {
                    if (lightProps.PrimaryRadius != -1)
                        t.Args[firstArg] = lightProps.PrimaryRadius;

                    if (showAllControls && Array.IndexOf(LIGHT_USES_ANGLE_VALUE, t.Type) != -1) {
                        t.Args[4] = lightProps.SecondaryRadius;
                        t.Rotate(General.ClampAngle(lightProps.Interval));
                    }
                }
            }

            //update VisualThings
            if (editingModeName == "BaseVisualMode") {
                foreach (VisualThing t in visualSelection)
                    t.UpdateLight();
            }
        }

        //switch between absolute and relative mode
        private void setControlsMode() {
            if (RELATIVE_MODE) {
                setFixedValues();

                colorPickerSlider1.SetLimits(TB_REL_MIN, TB_REL_MAX, NUD_REL_MIN, NUD_REL_MAX);
                colorPickerSlider1.Value = 0;

                if (showAllControls) {
                    colorPickerSlider2.SetLimits(TB_REL_MIN, TB_REL_MAX, NUD_REL_MIN, NUD_REL_MAX);
                    colorPickerSlider2.Value = 0;

                    colorPickerSlider3.SetLimits(TB_ANGLE_REL_MIN, TB_ANGLE_REL_MAX, NUD_REL_MIN, NUD_REL_MAX);
                    colorPickerSlider3.Value = 0;
                }    
            } else {
                updateLightPropsFromThing(selection[0]);

                colorPickerSlider1.SetLimits(TB_ABS_MIN, TB_ABS_MAX, NUD_ABS_MIN, NUD_ABS_MAX);
                colorPickerSlider1.Value = lightProps.PrimaryRadius;

                if (showAllControls) {
                    colorPickerSlider2.SetLimits(TB_ABS_MIN, TB_ABS_MAX, NUD_ABS_MIN, NUD_ABS_MAX);
                    colorPickerSlider2.Value = lightProps.SecondaryRadius;

                    colorPickerSlider3.SetLimits(TB_ANGLE_ABS_MIN, TB_ANGLE_ABS_MAX, NUD_ABS_MIN, NUD_ABS_MAX);
                    colorPickerSlider3.Value = lightProps.Interval;
                }
            }
        }

        private void makeUndo(string description) {
            General.Map.UndoRedo.CreateUndo(description);

            //tricky way to actually store undo information...
            foreach (Thing t in selection)
                t.Move(t.Position);
        }

        //this is called only once
        private Color getThingColor(Thing thing) {
            if (thing.Type == 1502) //vavoom light
                return Color.White;
			if (thing.Type == 1503)  //vavoom colored light
                return Color.FromArgb((byte)thing.Args[1], (byte)thing.Args[2], (byte)thing.Args[3]);
            return Color.FromArgb((byte)thing.Args[0], (byte)thing.Args[1], (byte)thing.Args[2]);
        }

        //this sets data to use as a reference for relative mode
        private void setFixedValues() {
            fixedValues = new List<LightProps>();

            for (int i = 0; i < selection.Count; i++) {
                Thing t = selection[i];
                //ThingTypeInfo typeInfo = General.Map.Data.GetThingInfoEx(t.Type);
                LightProps lp = new LightProps();

                int firstArg = 3;
                if (t.Type == 1502 || t.Type == 1503)
                    firstArg = 0;

                //if(typeInfo.Args[firstArg].Used)
                    lp.PrimaryRadius = t.Args[firstArg];

                //if(typeInfo.Args[4].Used)
                    //lp.SecondaryRadius = t.Args[4];

                //either both of them or none are used
                if (showAllControls &&  Array.IndexOf(LIGHT_USES_ANGLE_VALUE, t.Type) != -1) {
                    lp.SecondaryRadius = t.Args[4];
                    lp.Interval = t.AngleDoom;
                }

                fixedValues.Add(lp);
            }
        }

//events
        private void colorPickerControl1_ColorChanged(object sender, ColorChangedEventArgs e) {
            //need this check to allow relative mode to work properly
            if ((byte)e.RGB.Red != lightProps.Red || (byte)e.RGB.Green != lightProps.Green || (byte)e.RGB.Blue != lightProps.Blue) {
                updateLightPropsFromSliders();
            }
        }

        private void colorPickerControl1_OnCancelPressed(object sender, EventArgs e) {
			this.DialogResult = DialogResult.Cancel;
            Close();
        }

        private void colorPickerControl1_OnOkPressed(object sender, EventArgs e) {
			this.DialogResult = DialogResult.OK;
			General.Interface.RefreshInfo();
			Close();
        }

		private void LightColorPicker_FormClosing(object sender, FormClosingEventArgs e) {
			if(this.DialogResult == DialogResult.Cancel)
				General.Map.UndoRedo.WithdrawUndo();
		}

        private void cbRelativeMode_CheckStateChanged(object sender, EventArgs e) {
            RELATIVE_MODE = ((CheckBox)sender).Checked;
            setControlsMode();
        }

        private void onSliderValueChanged(object sender, ColorPickerSliderEventArgs e) {
            updateLightPropsFromSliders();
        }

        private void LightColorPicker_HelpRequested(object sender, HelpEventArgs hlpevent) {
            General.ShowHelp("gzdb/features/all_modes/colorpicker.html");
            hlpevent.Handled = true;
        }
    }

    struct LightProps {
        public byte Red;
        public byte Green;
        public byte Blue;

        public int PrimaryRadius;
        public int SecondaryRadius;
        public int Interval;
    }
}