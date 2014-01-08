using System.Drawing;
using System.Windows.Forms;
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

		public override int MinimumRevision { get { return 1869; } }

		public override string Name { get { return "Color Picker"; } }

		private IColorPicker form;
		private ToolsForm toolsform;

		private Point formLocation; //used to keep form's location constant

		public override void OnInitialize() {
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

			string currentModeName = General.Editing.Mode.GetType().Name;

			//display one of colorPickers or tell the user why we can't do that
			if (currentModeName == "ThingsMode") {
				if(General.Map.Map.SelectedThingsCount == 0){
					General.Interface.DisplayStatus(StatusType.Warning, "Select some lights first!");
					return;
				}
				form = new LightColorPicker();

			} else if (currentModeName == "SectorsMode") {
				if (General.Map.UDMF) {
					if (General.Map.Map.SelectedSectorsCount == 0) {
						General.Interface.DisplayStatus(StatusType.Warning, "Select some sectors first!");
						return;
					}
					form = new SectorColorPicker();
				} else {
					General.Interface.DisplayStatus(StatusType.Warning, "Sector colors can only be set if map is in UDMF format!");
					return;
				}

			} else if (currentModeName == "BaseVisualMode") {
				//nothing selected in visual mode?
				if ( ((VisualMode)General.Editing.Mode).GetSelectedVisualThings(true).Count == 0 ) {
					//check sectors
					int selectedSectorsCount = ((VisualMode)General.Editing.Mode).GetSelectedVisualSectors(true).Count;
					if (General.Map.UDMF && (selectedSectorsCount > 0 || General.Map.Map.SelectedSectorsCount > 0)) {
						form = new SectorColorPicker();
					} else {
						General.Interface.DisplayStatus(StatusType.Warning, "Select some lights " + (General.Map.UDMF ? ", sectors or surfaces " : "") + "first!");
						return;
					}
				} else {
					form = new LightColorPicker();
				}
			} else { //wrong mode
				General.Interface.DisplayStatus(StatusType.Warning, "Switch to" + (General.Map.UDMF ? " Sectors," : "") + " Things or GZDoom Visual Mode first!");
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