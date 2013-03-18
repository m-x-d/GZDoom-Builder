using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

using CodeImp.DoomBuilder.Editing;
using CodeImp.DoomBuilder.Geometry;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Windows;

namespace CodeImp.DoomBuilder.BuilderModes.ClassicModes
{
    [EditMode(DisplayName = "Snap Vertices to Grid",
              SwitchAction = "snapvertstogrid",
              AllowCopyPaste = false,
              Optional = false,
              Volatile = true)]
    public class SnapVerticesMode : BaseClassicMode
    {
        public SnapVerticesMode() {
            // We have no destructor
            GC.SuppressFinalize(this);
        }
        
        // Mode engages
        public override void OnEngage() {
            base.OnEngage();

            //get selection
			General.Map.Map.ClearAllMarks(false);
            General.Map.Map.MarkAllSelectedGeometry(true, false, true, false, false);
            List<Vertex> verts = General.Map.Map.GetMarkedVertices(true);

            //nothing selected?
            if (verts.Count == 0) {
                General.Interface.DisplayStatus(StatusType.Warning, "Select some vertices first!");
                base.OnCancel();
                General.Editing.ChangeMode(General.Editing.PreviousStableMode.Name);
                return;
            }

            // Make undo for the snapping
            General.Map.UndoRedo.CreateUndo("Snap vertices");

            int snappedCount = 0;

            //snap them all!
            foreach (Vertex v in verts) {
                Vector2D pos = v.Position;
                v.SnapToGrid();

                if (v.Position.x != pos.x || v.Position.y != pos.y)
                    snappedCount++;
            }

            //done
            General.Interface.DisplayStatus(StatusType.Info, "Snapped " + snappedCount + " vertices.");
            base.OnAccept();
            General.Editing.ChangeMode(General.Editing.PreviousStableMode.Name);
        }

        // Disenagaging
        public override void OnDisengage() {
            base.OnDisengage();
            Cursor.Current = Cursors.AppStarting;

            if (!cancelled) {
                // Update cached values
                General.Map.Map.Update();
                // Map is changed
                General.Map.IsChanged = true;
            }

            // Done
            Cursor.Current = Cursors.Default;
        }
    }
}
