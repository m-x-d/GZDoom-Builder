using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using CodeImp.DoomBuilder.Map;

namespace CodeImp.DoomBuilder.PropertiesDock
{
	public partial class PropertiesDocker : UserControl
	{
        private TabPage page2;
        private TabPage page3;

        private string currentMode;
        private static PropertiesDocker me;
        
        public PropertiesDocker() {
			InitializeComponent();

            me = this;
            page2 = tabControl.TabPages[1];
            page3 = tabControl.TabPages[2];

            if (!General.Map.UDMF) {
                gbCustomFields.Visible = false;
                tabControl.Top = 3;
            } else {
                //todo: add "Delete field" button

                //todo: sort this out...
                //cbFieldType.Items.AddRange(General.Types.GetCustomUseAttributes());
            }
		}

//SHOW HIGHLIGHT INFO
        public void ShowLinedefInfo(Linedef l) {

        }

        public void ShowSectorInfo(Sector s) {

        }

        public void ShowThingInfo(Thing t) {
            propertyGrid1.SelectedObject = new ThingInfo(t);
            viewThings(1, t.Index, true);
        }

        public void ShowVertexInfo(Vertex v) {
            propertyGrid1.SelectedObject = new VertexInfo(v);
            viewVertices(1, v.Index, true);
        }

        public void OnHighlightLost() {
            Update();
        }

//SHOW SELECTION INFO
        private void showSelectedThingsInfo() {
            //anything selected?
            List<Thing> things = (List<Thing>)General.Map.Map.GetSelectedThings(true);

            if (things.Count > 0) {
                ThingInfo[] infos = new ThingInfo[things.Count];
                int i = 0;

                foreach (Thing t in things) {
                    infos[i++] = new ThingInfo(t);
                }

                propertyGrid1.SelectedObjects = infos;
                viewThings(things.Count, things.Count == 1 ? things[0].Index : -1, true);
            } else {
                viewThings(-1, -1, false);
            }
        }
        
        private void showSelectedVerticesInfo() {
            //anything selected?
            List<Vertex> verts = (List<Vertex>)General.Map.Map.GetSelectedVertices(true);

            if (verts.Count > 0) {
                VertexInfo[] infos = new VertexInfo[verts.Count];
                int i = 0;

                foreach (Vertex cv in verts) {
                    infos[i++] = new VertexInfo(cv);
                }

                propertyGrid1.SelectedObjects = infos;
                viewVertices(verts.Count, verts.Count == 1 ? verts[0].Index : -1, true);
            } else {
                viewVertices(-1, -1, false);
            }
        }

//PANELS UPDATE
        private void viewVertices(int count, int index, bool enabled) {
            updateTabs(enabled, false);

            if(count != -1)
                tabControl.TabPages[0].Text = count > 1 ? count + " vertices:" : "Vertex "+index+":";
        }

        private void viewThings(int count, int index, bool enabled) {
            updateTabs(enabled, false);

            if (count != -1)
                tabControl.TabPages[0].Text = count > 1 ? count + " things:" : "Thing " + index + ":";
        }

        private void updateTabs(bool enabled, bool showAllTabs) {
            if (showAllTabs) {
                if (tabControl.TabPages.Count == 1) {
                    tabControl.TabPages.Add(page2);
                    tabControl.TabPages.Add(page3);
                }
                propertyGrid2.Enabled = enabled;
                propertyGrid3.Enabled = enabled;

            } else {
                if (tabControl.TabPages.Count == 3) {
                    tabControl.TabPages.Remove(page2);
                    tabControl.TabPages.Remove(page3);
                }
            }
            propertyGrid1.Enabled = enabled;
        }

//util
        public void ChangeEditMode(string name) {
            //textBox1.AppendText("Mode Changed to " + name + Environment.NewLine);
            
            if (name == "ThingsMode") {
                currentMode = name;
                showSelectedThingsInfo();

            } else if (name == "SectorsMode") {
                currentMode = name;

            } else if (name == "LinedefsMode") {
                currentMode = name;

            } else if (name == "VerticesMode") {
                currentMode = name;
                showSelectedVerticesInfo();
            } else if (name == "BaseVisualMode") {
                currentMode = name;

            }
        }

        //called when map is changed without switching Edit mode
        public void Update() {
            ChangeEditMode(currentMode);
        }

        public static void Refresh() {
            me.propertyGrid1.Refresh();
            me.propertyGrid2.Refresh();
            me.propertyGrid3.Refresh();
        }

        private void saveChanges() {
            if (currentMode == "ThingsMode" && propertyGrid1.Enabled) {
                applyThingChanges();
            } else if (currentMode == "SectorsMode" && propertyGrid1.Enabled) {
                applySectorChanges();
            } else if (currentMode == "LinedefsMode") {
                applyLinedefChanges();
            } else if (currentMode == "VerticesMode" && propertyGrid1.Enabled) {
                applyVertexChanges();
            } else if (currentMode == "BaseVisualMode") {
                applyVisualModeChanges();
            }
        }

        private void applyThingChanges() {
            //throw new NotImplementedException();
        }

        private void applySectorChanges() {
            throw new NotImplementedException();
        }

        private void applyLinedefChanges() {
            throw new NotImplementedException();
        }

        private void applyVertexChanges() {
            string undodesc = "vertex";
            if (propertyGrid1.SelectedObjects.Length > 1) undodesc = propertyGrid1.SelectedObjects.Length + " vertices";
            applyClassicModeChanges(propertyGrid1.SelectedObjects, undodesc);
        }

        private void applyVisualModeChanges() {
            throw new NotImplementedException();
        }

        private void applyClassicModeChanges(object[] items, string undodesc) {
            // Make undo
            General.Map.UndoRedo.CreateUndo("Edit " + undodesc);

            //apply changes
            foreach (object o in items) {
                ((IMapElementInfo)o).ApplyChanges();
            }

            // Done
            General.Map.IsChanged = true;
            General.Map.Map.Update();
            General.Interface.RedrawDisplay();
        }

//EVENTS
        private void propertyGrid1_PropertyValueChanged(object s, PropertyValueChangedEventArgs e) {
            saveChanges();
        }

        private void propertyGrid2_PropertyValueChanged(object s, PropertyValueChangedEventArgs e) {
            saveChanges();
        }

        private void propertyGrid3_PropertyValueChanged(object s, PropertyValueChangedEventArgs e) {
            saveChanges();
        }

        private void bAddField_Click(object sender, EventArgs e) {
            PropertyGrid g = null;
            
            if (tbFieldName.Text.Length > 0) {
                if (tabControl.SelectedIndex == 0) {
                    g = propertyGrid1;
                } else if (tabControl.SelectedIndex == 1) {
                    g = propertyGrid2;
                } else if (tabControl.SelectedIndex == 2) {
                    g = propertyGrid3;
                }
            }

            if (g != null && g.Enabled && g.SelectedObjects.Length > 0) {
                foreach (object o in g.SelectedObjects) {
                    //todo: set correct type
                    ((IMapElementInfo)o).AddCustomProperty(tbFieldName.Text, typeof(string));
                }
            }
        }
	}
}
