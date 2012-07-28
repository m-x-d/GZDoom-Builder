using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using CodeImp.DoomBuilder.Plugins;
using CodeImp.DoomBuilder.Actions;
using CodeImp.DoomBuilder.Windows;
using CodeImp.DoomBuilder.VisualModes;

namespace CodeImp.DoomBuilder.UDMFControls
{
    public sealed class BuilderPlug: Plug {
        private static BuilderPlug me;
        public static BuilderPlug Me { get { return me; } }

        public override string Name { get { return "UDMF Controls"; } }

        //settings
        private int scaleReange;
        public int ScaleReange { get { return scaleReange; } }

        private UDMFControlsForm form;
        private Point formLocation; //used to keep form's location constant

        public override void OnInitialize() {
            if (GZBuilder.GZGeneral.Version < 1.11f) {
                General.ErrorLogger.Add(ErrorType.Error, "UDMFControls plugin: GZDoomBuilder 1.11 or later required!");
                return;
            }

            base.OnInitialize();
            me = this;

            General.Actions.BindMethods(this);

            scaleReange = General.Settings.ReadPluginSetting("scalerange", 2);
            if (scaleReange < 1) scaleReange = 2;
            General.Settings.WritePluginSetting("scalerange", scaleReange); //not working when called from Dispose()...
        }

        public override void Dispose() {
            base.Dispose();
            General.Actions.UnbindMethods(this);

            if (form != null) form.Close();
            form = null;
        }

        [BeginAction("openudmfcontrols")]
        private void openControls() {
            if (General.Editing.Mode == null)
                return;

            if (!General.Map.UDMF) {
                General.Interface.DisplayStatus(StatusType.Warning, "Map in UDMF format required!");
                return;
            }

            List<VisualGeometry> selectedSurfaces;

            if (General.Editing.Mode.GetType().Name == "BaseVisualMode") {
                selectedSurfaces = ((VisualMode)General.Editing.Mode).GetSelectedSurfaces();
                if (selectedSurfaces.Count == 0) {
                    General.Interface.DisplayStatus(StatusType.Warning, "Select some surfaces first!");
                    return;
                }
            } else {//wrong mode
                General.Interface.DisplayStatus(StatusType.Warning, "Switch to Visual Mode first!");
                return;
            }

            //show form
            form = new UDMFControlsForm();
            if (formLocation.X == 0 && formLocation.Y == 0) {
                Size displaySize = Plug.DisplaySize;
                Point displayLocation = Plug.DisplayLocationAbs;
                formLocation = new Point(displayLocation.X + displaySize.Width - form.Width - 16, displayLocation.Y + 32);
            }
            form.Location = formLocation;
            form.FormClosed += new FormClosedEventHandler(form_FormClosed);
            form.Setup(selectedSurfaces);
            form.ShowDialog(Form.ActiveForm);
        }

        private void form_FormClosed(object sender, FormClosedEventArgs e) {
            formLocation = form.Location;
            form.Dispose();
            form = null;
        }
    }
}