using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using CodeImp.DoomBuilder;
using CodeImp.DoomBuilder.Editing;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Windows;
using CodeImp.DoomBuilder.Plugins;
using CodeImp.DoomBuilder.Actions;
using CodeImp.DoomBuilder.VisualModes;

using CodeImp.DoomBuilder.ColorPicker.Windows;

namespace CodeImp.DoomBuilder.ColorPicker
{
    public class BuilderPlug : Plug
    {
        private static BuilderPlug me;
        public static BuilderPlug Me { get { return me; } }

        public override string Name { get { return "Color Picker"; } }

        private IColorPicker form;
        private ToolsForm toolsform;

        private string currentModeName = "";

        private Point formLocation; //used to keep forms location constant

        public override void OnInitialize() {
            //yeeees, I used string to store version number before 1.05...
            if (GZBuilder.GZGeneral.Version.GetType().Name != "Single") {
                General.ErrorLogger.Add(ErrorType.Error, "ColorPicker plugin: GZDoomBuilder 1.05 or later required!");
                return;
            }
            
            base.OnInitialize();
            me = this;

            General.Actions.BindMethods(this);
        }

        public override void OnMapOpenEnd() {
            if (toolsform == null)
                toolsform = new ToolsForm();
        }

        public override void OnMapNewEnd() {
            OnMapOpenEnd();
        }

        public override void Dispose() {
            base.Dispose();
            General.Actions.UnbindMethods(this);

            if (form != null) form.Close();
            form = null;

            if (toolsform != null) toolsform.Dispose();
            toolsform = null;
        }

        [BeginAction("togglelightpannel")]
        private void toggleLightPannel() {
            if (General.Editing.Mode == null)
                return;

            currentModeName = General.Editing.Mode.GetType().Name;
            bool udmf = General.Map.Config.FormatInterface == "UniversalMapSetIO";

            //display one of colorPickers or tell the user why we can't do that
            if (currentModeName == "ThingsMode") {
                if(General.Map.Map.SelectedThingsCount == 0){
                    Plug.DisplayStatus(StatusType.Warning, "Select some lights first!");
                    return;
                }
                form = new LightColorPicker();

            } else if (currentModeName == "SectorsMode") {
                if (udmf) {
                    if (General.Map.Map.SelectedSectorsCount == 0) {
                        Plug.DisplayStatus(StatusType.Warning, "Select some sectors first!");
                        return;
                    }
                    form = new SectorColorPicker();
                } else {
                    Plug.DisplayStatus(StatusType.Warning, "Sector colors can only be set if map is in UDMF format!");
                    return;
                }

            } else if (currentModeName == "BaseVisualMode") {
                //nothing selected in visual mode?
                if ( ((VisualMode)General.Editing.Mode).SelectedVisualThings.Count == 0 ) {
                    //check sectors
                    if (udmf && General.Map.Map.SelectedSectorsCount > 0) {
                        form = new SectorColorPicker();
                    } else {
                        Plug.DisplayStatus(StatusType.Warning, "Select some lights " + (udmf ? "or sectors " : "") + "first!");
                        return;
                    }
                } else {
                    form = new LightColorPicker();
                }
            } else { //wrong mode
                Plug.DisplayStatus(StatusType.Warning, "Switch to" + (udmf ? " Sectors," : "") + " Things or GZDoom Visual Mode first!");
                return;
            }

            if (form.Setup(currentModeName)) {
                if (formLocation.X == 0 && formLocation.Y == 0) {
                    Size displaySize = Plug.DisplaySize;
                    Point displayLocation = Plug.DisplayLocationAbs;
                    formLocation = new Point(displayLocation.X + displaySize.Width - form.Width - 16, displayLocation.Y + 32);
                } 
                form.Location = formLocation;
                form.FormClosed += new FormClosedEventHandler(form_FormClosed);
                form.ShowDialog(Form.ActiveForm);
            } else {
                form.Dispose();
                form = null;
            }
        }

        private void form_FormClosed(object sender, FormClosedEventArgs e) {
            formLocation = form.Location;
            form.Dispose();
            form = null;
        }
    }
}
