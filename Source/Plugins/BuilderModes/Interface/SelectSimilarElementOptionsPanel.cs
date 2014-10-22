using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using CodeImp.DoomBuilder.Controls;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Windows;

namespace CodeImp.DoomBuilder.BuilderModes
{
	public partial class SelectSimilarElementOptionsPanel : DelayedForm
	{
		private static Size size = Size.Empty;
		private static Point location = Point.Empty;
		private static readonly object[] flags = {
			                                new SectorPropertiesCopySettings(),
			                                new LinedefPropertiesCopySettings(),
			                                new SidedefPropertiesCopySettings(),
			                                new ThingPropertiesCopySettings(),
			                                new VertexPropertiesCopySettings()
		                                };

		private BaseClassicMode mode;
		private TabPage[] activeTabs;
		private readonly Dictionary<CheckboxArrayControl, object> typecontrols;
		
		public SelectSimilarElementOptionsPanel() {
			InitializeComponent();

			//apply window size and location
			if(!size.IsEmpty && !location.IsEmpty) {
				this.StartPosition = FormStartPosition.Manual;
				this.Size = size;
				this.Location = location;
			}

			//create a collection
			typecontrols = new Dictionary<CheckboxArrayControl, object> {
				{sectorflags, flags[0]},
				{lineflags, flags[1]},
				{sideflags, flags[2]},
				{thingflags, flags[3]},
				{vertexflags, flags[4]}
			};
		}

		public bool Setup(BaseClassicMode mode) {
			this.mode = mode;

			//which tabs should we display?
			if(General.Editing.Mode is ThingsMode) {
				activeTabs = new[] {things};
			} else if(General.Editing.Mode is VerticesMode) {
				activeTabs = new[] { vertices };
			} else if(General.Editing.Mode is LinedefsMode) {
				activeTabs = new[] { linedefs, sidedefs };
			} else if(mode is SectorsMode) {
				activeTabs = new[] { sectors };
			} else {
				General.Interface.DisplayStatus(StatusType.Warning, "This action doesn't support current editing mode...");
				return false;
			}

			//fill flags
			foreach(TabPage page in activeTabs) {
				CheckboxArrayControl curControl = page.Controls[0] as CheckboxArrayControl;
				if(curControl == null) continue; //just a piece of boilerplate...

				FieldInfo[] props = typecontrols[curControl].GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
				string title = "<unknown flag>";
				foreach(var prop in props) {
					foreach(Attribute attr in Attribute.GetCustomAttributes(prop)) {
						if(attr.GetType() == typeof(FieldDescription)) {
							title = ((FieldDescription)attr).Description;
							break;
						}
					}

					curControl.Add(title, prop.Name).Checked = (bool)prop.GetValue(typecontrols[curControl]);
				}

				curControl.PositionCheckboxes();
			}

			//hide unused tab pages
			tabControl.SelectTab(activeTabs[0]);
			tabControl.TabPages.Clear();
			tabControl.TabPages.AddRange(activeTabs);

			return true;
		}

		private void SelectSimilarElementOptionsPanel_FormClosing(object sender, FormClosingEventArgs e) {
			size = this.Size;
			location = this.Location;
		}

		private void enableall_Click(object sender, EventArgs e) {
			CheckboxArrayControl curControl = tabControl.SelectedTab.Controls[0] as CheckboxArrayControl;
			if(curControl == null) return; //just a piece of boilerplate...
			bool enable = !curControl.Checkboxes[0].Checked;
			foreach(var cb in curControl.Checkboxes) cb.Checked = enable;
		}

		private void cancel_Click(object sender, EventArgs e) {
			this.Close();
		}

		private void apply_Click(object sender, EventArgs e) {
			//save flags states
			foreach (TabPage page in activeTabs) {
				CheckboxArrayControl curControl = page.Controls[0] as CheckboxArrayControl;
				if(curControl == null) continue; //just a piece of boilerplate...

				FieldInfo[] props = typecontrols[curControl].GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
				var fields = new Dictionary<string, FieldInfo>(props.Length);
				for(int i = 0; i < props.Length; i++) {
					fields[props[i].Name] = props[i];
				}

				foreach(CheckBox cb in curControl.Checkboxes) {
					fields[cb.Tag.ToString()].SetValue(typecontrols[curControl], cb.Checked);
				}
			}

			//perform selection
			if(mode is ThingsMode) {
				ICollection<Thing> selected = General.Map.Map.GetSelectedThings(true);
				ICollection<Thing> unselected = General.Map.Map.GetSelectedThings(false);

				foreach (Thing target in unselected) {
					foreach (Thing source in selected) {
						if (PropertiesComparer.PropertiesMatch((ThingPropertiesCopySettings) typecontrols[thingflags], source, target))
							mode.SelectMapElement(target);
					}
				}

			} else if(mode is LinedefsMode) {
				ICollection<Linedef> selected = General.Map.Map.GetSelectedLinedefs(true);
				ICollection<Linedef> unselected = General.Map.Map.GetSelectedLinedefs(false);

				foreach(Linedef target in unselected) {
					foreach(Linedef source in selected) {
						if(PropertiesComparer.PropertiesMatch((LinedefPropertiesCopySettings)typecontrols[lineflags], (SidedefPropertiesCopySettings)typecontrols[sideflags], source, target))
							mode.SelectMapElement(target);
					}
				}

			} else if(mode is SectorsMode) {
				ICollection<Sector> selected = General.Map.Map.GetSelectedSectors(true);
				ICollection<Sector> unselected = General.Map.Map.GetSelectedSectors(false);

				foreach(Sector target in unselected) {
					foreach(Sector source in selected) {
						if(PropertiesComparer.PropertiesMatch((SectorPropertiesCopySettings)typecontrols[sectorflags], source, target))
							mode.SelectMapElement(target);
					}
				}

			} else if(mode is VerticesMode) {
				ICollection<Vertex> selected = General.Map.Map.GetSelectedVertices(true);
				ICollection<Vertex> unselected = General.Map.Map.GetSelectedVertices(false);

				foreach(Vertex target in unselected) {
					foreach(Vertex source in selected) {
						if(PropertiesComparer.PropertiesMatch((VertexPropertiesCopySettings)typecontrols[vertexflags], source, target))
							mode.SelectMapElement(target);
					}
				}

			}

			mode.UpdateSelectionInfo();
			General.Interface.RedrawDisplay();
			this.Close();
		}
	}
}
