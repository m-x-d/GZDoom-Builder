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
        
        public PropertiesDocker() {
			InitializeComponent();

            page2 = tabControl.TabPages[1];
            page3 = tabControl.TabPages[2];
		}

//SHOW HIGHLIGHT INFO
        public void ShowLinedefInfo(Linedef l) {

        }

        public void ShowSectorInfo(Sector s) {

        }

        public void ShowThingInfo(Thing t) {

        }

        public void ShowVertexInfo(Vertex v) {
            propertyGrid1.SelectedObject = new VertexInfo(v);
            viewVertices(true);
        }

        public void OnHighlightLost() {
            Update();
        }

//SHOW SELECTION INFO
        private void showSelectedVerticesInfo() {
            //anything selected?
            ICollection<Vertex> verts = General.Map.Map.GetSelectedVertices(true);

            if (verts.Count > 0) {
                VertexInfo[] infos = new VertexInfo[verts.Count];
                int i = 0;

                foreach (Vertex cv in verts) {
                    infos[i++] = new VertexInfo(cv);
                }

                propertyGrid1.SelectedObjects = infos;
                viewVertices(true);
            } else {
                //propertyGrid1.SelectedObjects = null;
                viewVertices(false);
            }
        }

//PANELS UPDATE
        private void viewVertices(bool enabled) {
            propertyGrid1.Enabled = enabled;
            tabControl.TabPages[0].Text = "Vertex:";

            if (tabControl.TabPages.Count > 1) {
                tabControl.TabPages.Remove(page2);
                tabControl.TabPages.Remove(page3);
            }
        }

//util
        public void ChangeEditMode(string name) {
            textBox1.AppendText("Mode Changed to " + name + Environment.NewLine);
            
            if (name == "ThingsMode") {
                currentMode = name;

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
            throw new NotImplementedException();
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
	}
}
