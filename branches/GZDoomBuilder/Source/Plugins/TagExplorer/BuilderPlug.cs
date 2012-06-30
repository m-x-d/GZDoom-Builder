using System;
using System.Collections.Generic;
using System.Text;

using CodeImp.DoomBuilder.Config;
using CodeImp.DoomBuilder.Controls;
using CodeImp.DoomBuilder.Editing;
using CodeImp.DoomBuilder.Plugins;

namespace CodeImp.DoomBuilder.TagExplorer
{
    public class BuilderPlug : Plug
    {
        private static BuilderPlug me;

        // Docker
        private TagExplorer tagExplorer;
        private Docker docker;

        // Static property to access the BuilderPlug
        public static BuilderPlug Me { get { return me; } }

        // This event is called when the plugin is initialized
        public override void OnInitialize() {
            if (GZBuilder.GZGeneral.Version < 1.10f) {
                General.ErrorLogger.Add(ErrorType.Error, "Tag Explorer plugin: GZDoom Builder 1.10 or later required!");
                return;
            }
            
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
            // If we just opened a UDMF format map, we want to create the Tag Explorer panel
            if (General.Map.Config.FormatInterface == "UniversalMapSetIO") {
                tagExplorer = new TagExplorer();
                docker = new Docker("tagexplorerdockerpanel", "Tag Explorer", tagExplorer);
                General.Interface.AddDocker(docker);
                tagExplorer.Setup();
            }
        }

        // This is called after a map has been closed
        public override void OnMapCloseEnd() {
            // If we have a Comments panel, remove it
            if (tagExplorer != null) {
                General.Interface.RemoveDocker(docker);
                docker = null;
                tagExplorer.Dispose();
                tagExplorer = null;
            }
        }

        // Geometry pasted
        public override void OnPasteEnd(PasteOptions options) {
            if (tagExplorer != null) {
                tagExplorer.UpdateTree();
            }
        }

        // Undo performed
        public override void OnUndoEnd() {
            if (tagExplorer != null) {
                tagExplorer.UpdateTree();
            }
        }

        // Redo performed
        public override void OnRedoEnd() {
            if (tagExplorer != null) {
                tagExplorer.UpdateTree();
            }
        }

        // Mode changes
        /*public override bool OnModeChange(EditMode oldmode, EditMode newmode) {
            if (tagExplorer != null) {
                tagExplorer.UpdateTree();
            }

            return base.OnModeChange(oldmode, newmode);
        }*/
    }
}
