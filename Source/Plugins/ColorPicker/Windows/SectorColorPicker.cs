using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using CodeImp.DoomBuilder.Windows;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Config;
using CodeImp.DoomBuilder.Types;
using CodeImp.DoomBuilder.VisualModes;

using CodeImp.DoomBuilder.Plugins;

namespace CodeImp.DoomBuilder.ColorPicker.Windows
{
    public partial class SectorColorPicker : DelayedForm, IColorPicker
    {
        public ColorPickerType Type { get { return ColorPickerType.CP_SECTOR; } }

        private List<Sector> selection;
        
        private int curSectorColor;
        private int curFadeColor;
        private int initialSectorColor;
        private int initialFadeColor;
        private int defaultSectorColor;
        private int defaultFadeColor;

        private static string currentColorTag = "lightcolor"; //lightcolor or fadecolor
        private string mode;

        public bool Setup(string editingModeName) {
            mode = editingModeName;
            selection = (List<Sector>)(General.Map.Map.GetSelectedSectors(true));

            //create undo
            string rest = selection.Count + " sector" + (selection.Count > 1 ? "s" : "");
            General.Map.UndoRedo.CreateUndo("Edit color of " + rest);
            
            foreach (Sector s in selection)
                s.Fields.BeforeFieldsChange();

            //get default values
            List<UniversalFieldInfo> sectorFields = General.Map.Config.SectorFields;
            foreach (UniversalFieldInfo i in sectorFields) {
                if (i.Name == "lightcolor")
                    defaultSectorColor = (int)i.Default;
                else if (i.Name == "fadecolor")
                    defaultFadeColor = (int)i.Default;
            }

            //set colors
            curSectorColor = selection[0].Fields.GetValue<int>("lightcolor", -1);
            if (curSectorColor == -1)
                curSectorColor = defaultSectorColor;
            
            curFadeColor = selection[0].Fields.GetValue<int>("fadecolor", -1);
            if (curFadeColor == -1)
                curFadeColor = defaultFadeColor;

            //check that all sectors in selection have "lightcolor" and "fadecolor" fields
            for (int i = 0; i < selection.Count; i++) {
                if (!selection[i].Fields.ContainsKey("lightcolor"))
                    selection[i].Fields.Add("lightcolor", new UniValue(UniversalType.Color, curSectorColor));
                
                if (!selection[i].Fields.ContainsKey("fadecolor"))
                    selection[i].Fields.Add("fadecolor", new UniValue(UniversalType.Color, curFadeColor));
            }

            initialSectorColor = curSectorColor;
            initialFadeColor = curFadeColor;

            InitializeComponent();

            colorPickerControl1.Initialize(Color.FromArgb(currentColorTag == "lightcolor" ? curSectorColor : curFadeColor));
            colorPickerControl1.ColorChanged += new EventHandler<ColorChangedEventArgs>(colorPickerControl1_ColorChanged);
            colorPickerControl1.OnOkPressed += new EventHandler(colorPickerControl1_OnOkPressed);
            colorPickerControl1.OnCancelPressed += new EventHandler(colorPickerControl1_OnCancelPressed);

            if (currentColorTag == "lightcolor")
                rbSectorColor.Checked = true;
            else
                rbFadeColor.Checked = true;

            rbSectorColor.CheckedChanged += new EventHandler(rbColor_CheckedChanged);
            rbFadeColor.CheckedChanged += new EventHandler(rbColor_CheckedChanged);

            Text = "Editing " + rest;

            //cannot fail here :)
            return true;
        }

        private void colorPickerControl1_OnCancelPressed(object sender, EventArgs e) {
            //restore initial values
            General.Map.UndoRedo.PerformUndo();
            Close();
        }

        private void colorPickerControl1_OnOkPressed(object sender, EventArgs e) {
            //check if values are default
            foreach (Sector s in selection) {
                if ((int)s.Fields["lightcolor"].Value == defaultSectorColor)
                    s.Fields.Remove("lightcolor");

                if ((int)s.Fields["fadecolor"].Value == defaultFadeColor)
                    s.Fields.Remove("fadecolor");
            }
            
            Close();
        }

        private void colorPickerControl1_ColorChanged(object sender, ColorChangedEventArgs e) {
            foreach (Sector s in selection) {
                s.Fields[currentColorTag].Value = e.RGB.Red << 16 | e.RGB.Green << 8 | e.RGB.Blue;
                s.UpdateNeeded = true;
                s.UpdateCache();
            }

            //update display
            if (mode == "SectorsMode") {
                General.Interface.RedrawDisplay();
            } else { //should be visual mode
                VisualMode vm = (VisualMode)General.Editing.Mode;
                
                foreach (Sector s in selection) {
                    if (vm.VisualSectorExists(s)) {
                        VisualSector vs = vm.GetVisualSector(s);
                        vs.UpdateSectorData();
                    }
                }
            }
        }

        private void rbColor_CheckedChanged(object sender, EventArgs e) {
            RadioButton b = (RadioButton)sender;
            if (b.Checked) {
                currentColorTag = (string)b.Tag;

                //update color picker
                if (currentColorTag == "lightcolor")
                    colorPickerControl1.SetInitialColor(Color.FromArgb(initialSectorColor));
                else
                    colorPickerControl1.SetInitialColor(Color.FromArgb(initialFadeColor));
                colorPickerControl1.SetCurrentColor(Color.FromArgb((int)selection[0].Fields[currentColorTag].Value));
            }
        }
    }
}
