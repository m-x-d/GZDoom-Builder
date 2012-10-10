using System;
using System.Collections.Generic;
using System.Text;

using CodeImp.DoomBuilder.Editing;
using CodeImp.DoomBuilder.Plugins;
using CodeImp.DoomBuilder.Controls;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Config;

namespace CodeImp.DoomBuilder.PropertiesDock
{
	public class BuilderPlug : Plug
	{
		// Docker
		private PropertiesDocker propertiesDocker;
		private Docker docker;

		// Static property to access the BuilderPlug
		public static BuilderPlug Me { get { return me; } }
        private static BuilderPlug me;

        //important actions
        private string[] actions = { "builder_deleteitem", "builder_classicselect", "builder_clearselection" };

		// This event is called when the plugin is initialized
		public override void OnInitialize() {
			base.OnInitialize();

			// Keep a static reference
			me = this;
		}

		// When a map is created
		public override void OnMapNewEnd() {
			OnMapOpenEnd();
		}

		// This is called after a map has been successfully opened
		public override void OnMapOpenEnd() {
            MapElementsData.Init();
            
            if(propertiesDocker == null) {
				propertiesDocker = new PropertiesDocker();
				docker = new Docker("propertiesdockerpanel", "Properties", propertiesDocker);
				General.Interface.AddDocker(docker);
                propertiesDocker.ChangeEditMode(General.Editing.Mode.GetType().Name);
			}
		}

		// This is called after a map has been closed
		public override void OnMapCloseBegin() {
			// If we have a Tag Explorer panel, remove it
			if(propertiesDocker != null) {
				General.Interface.RemoveDocker(docker);
				docker = null;
				propertiesDocker.Dispose();
				propertiesDocker = null;
			}
		}

        public override void OnHighlightLinedef(Linedef l) {
            if (propertiesDocker != null)
                propertiesDocker.ShowLinedefInfo(l);
        }

        public override void OnHighlightSector(Sector s) {
            if (propertiesDocker != null)
                propertiesDocker.ShowSectorInfo(s);
        }

        public override void OnHighlightVertex(Vertex v) {
            if (propertiesDocker != null)    
                propertiesDocker.ShowVertexInfo(v);
        }

        public override void OnHighlightThing(Thing t) {
            if (propertiesDocker != null)
                propertiesDocker.ShowThingInfo(t);
        }

        public override void OnHighlightRefreshed(object o) {
            if (propertiesDocker != null) {
                if (o is Thing) {
                    propertiesDocker.ShowThingInfo(o as Thing);
                } else if (o is Sector) {
                    propertiesDocker.ShowSectorInfo(o as Sector);
                } else if (o is Linedef) {
                    propertiesDocker.ShowLinedefInfo(o as Linedef);
                } else if (o is Vertex) {
                    propertiesDocker.ShowVertexInfo(o as Vertex);
                }
            }
        }

        public override void OnHighlightLost() {
            if (propertiesDocker != null)
                propertiesDocker.OnHighlightLost();
        }

        public override void OnEditEngage(EditMode oldmode, EditMode newmode){
            if (propertiesDocker != null)
                propertiesDocker.ChangeEditMode(newmode.GetType().Name);
        }

        // Geometry pasted
        public override void OnPasteEnd(PasteOptions options) {
            if (propertiesDocker != null)
                propertiesDocker.Update();
        }

        // Undo performed
        public override void OnUndoEnd() {
            if (propertiesDocker != null)
                propertiesDocker.Update();
        }

        // Redo performed
        public override void OnRedoEnd() {
            if (propertiesDocker != null)
                propertiesDocker.Update();
        }

        public override void OnActionEnd(CodeImp.DoomBuilder.Actions.Action action) {
            Console.WriteLine("OnActionEnd: " + action.Name);
            
            if (propertiesDocker != null && Array.IndexOf(actions, action.Name) != -1)
                propertiesDocker.Update();
        }

	}
}
