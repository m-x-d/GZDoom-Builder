using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using CodeImp.DoomBuilder.Controls;

namespace CodeImp.DoomBuilder.BuilderModes.Interface
{
	public partial class PastePropertiesOptionsForm : Form
	{
		private static Size size = Size.Empty;
		private static Point location = Point.Empty;
		private readonly Dictionary<object, CheckboxArrayControl> typecontrols;
		
		public PastePropertiesOptionsForm() {
			InitializeComponent();

			//apply window size and location
			if(!size.IsEmpty && !location.IsEmpty) {
				this.StartPosition = FormStartPosition.Manual;
				this.Size = size;
				this.Location = location;
			}

			//create a collection
			typecontrols = new Dictionary<object, CheckboxArrayControl> {
				{SectorProperties.CopySettings, sectorflags},
				{LinedefProperties.CopySettings, lineflags},
				{SidedefProperties.CopySettings, sideflags},
				{ThingProperties.CopySettings, thingflags},
				{VertexProperties.CopySettings, vertexflags}
			};

			//fill flags
			foreach(KeyValuePair<object, CheckboxArrayControl> group in typecontrols) {
				FieldInfo[] props = group.Key.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
				string title = "<unknown flag>";
				foreach(var prop in props) {
					foreach(Attribute attr in Attribute.GetCustomAttributes(prop)) {
						if(attr.GetType() == typeof(FieldDescription)) {
							title = ((FieldDescription)attr).Description;
							break;
						}
					}

					group.Value.Add(title, prop.Name).Checked = (bool)prop.GetValue(group.Key);
				}

				group.Value.PositionCheckboxes();
			}

			//select proper tab
			if (General.Editing.Mode is ThingsMode) {
				tabControl.SelectTab(things);
			}else if (General.Editing.Mode is VerticesMode) {
				tabControl.SelectTab(vertices);
			}else if (General.Editing.Mode is LinedefsMode) {
				tabControl.SelectTab(linedefs);
			}
		}

		private void apply_Click(object sender, EventArgs e) {
			foreach (KeyValuePair<object, CheckboxArrayControl> group in typecontrols) {
				FieldInfo[] props = group.Key.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
				var fields = new Dictionary<string, FieldInfo>(props.Length);
				for(int i = 0; i < props.Length; i++) {
					fields[props[i].Name] = props[i];
				}

				foreach(CheckBox cb in group.Value.Checkboxes) {
					fields[cb.Tag.ToString()].SetValue(group.Key, cb.Checked);
				}
			}
			this.Close();
		}

		private void cancel_Click(object sender, EventArgs e) {
			this.Close();
		}

		private void enableall_Click(object sender, EventArgs e) {
			CheckboxArrayControl curControl = tabControl.SelectedTab.Controls[0] as CheckboxArrayControl;
			if(curControl == null) return; //just a piece of boilerplate...
			bool enable = !curControl.Checkboxes[0].Checked;
			foreach(var cb in curControl.Checkboxes) cb.Checked = enable;
		}

		private void PastePropertiesOptionsForm_FormClosing(object sender, FormClosingEventArgs e) {
			size = this.Size;
			location = this.Location;
		}
	}
}
