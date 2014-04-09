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
		private readonly Dictionary<Type, CheckboxArrayControl> typecontrols;
		
		public PastePropertiesOptionsForm() {
			Point pos = Cursor.Position;
			pos.Offset(-this.Width / 2, -this.Height / 2);
			if (pos.Y < 0) pos.Y = 0;
			this.Location = pos;

			InitializeComponent();

			//create a collection
			typecontrols = new Dictionary<Type, CheckboxArrayControl> {
				{typeof (SectorProperties), sectorflags}, 
				{typeof (LinedefProperties), lineflags}, 
				{typeof (SidedefProperties), sideflags}, 
				{typeof (ThingProperties), thingflags}, 
				{typeof (VertexProperties), vertexflags}
			};

			//fill flags
			foreach(KeyValuePair<Type, CheckboxArrayControl> group in typecontrols) {
				FieldInfo[] props = group.Key.GetFields(BindingFlags.Static | BindingFlags.Public);
				foreach(var prop in props) {
					group.Value.Add(prop.Name.Replace("_", " "), prop.Name).Checked = (bool)prop.GetValue(group.Key);
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
			foreach (KeyValuePair<Type, CheckboxArrayControl> group in typecontrols) {
				FieldInfo[] props = group.Key.GetFields(BindingFlags.Static | BindingFlags.Public);
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
			foreach (KeyValuePair<Type, CheckboxArrayControl> group in typecontrols) {
				foreach (var cb in group.Value.Checkboxes) cb.Checked = true;
			}
		}
	}
}
