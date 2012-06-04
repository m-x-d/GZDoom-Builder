using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

using CodeImp.DoomBuilder.Editing;
using CodeImp.DoomBuilder.Rendering;
using CodeImp.DoomBuilder.Geometry;
using CodeImp.DoomBuilder.Actions;
using CodeImp.DoomBuilder.Windows;
using CodeImp.DoomBuilder.Map;

namespace CodeImp.DoomBuilder.BuilderModes.ClassicModes
{
    [EditMode(DisplayName = "Draw Rectangle Mode",
              SwitchAction = "drawrectanglemode",
              AllowCopyPaste = false,
              Volatile = true,
              Optional = false)]

    public class DrawRectangleMode : DrawGeometryMode
    {
        
        override protected void Update() {
            PixelColor stitchcolor = General.Colors.Highlight;
            PixelColor losecolor = General.Colors.Selection;
            PixelColor color;

            snaptogrid = General.Interface.ShiftState ^ General.Interface.SnapToGrid;
            snaptonearest = General.Interface.CtrlState ^ General.Interface.AutoMerge;

            DrawnVertex curp = GetCurrentPosition();
            float vsize = ((float)renderer.VertexSize + 1.0f) / renderer.Scale;
            float vsizeborder = ((float)renderer.VertexSize + 3.0f) / renderer.Scale;

            // Render drawing lines
            if (renderer.StartOverlay(true)) {
                color = snaptogrid ? stitchcolor : losecolor;
                
                if (points.Count == 1) {
                    //points[1] = curp; // 2-nd point is opposite corner of rectangle

                    //calculate positions
                    Vector2D[] positions = new Vector2D[] { points[0].pos, new Vector2D(curp.pos.x, points[0].pos.y), curp.pos, new Vector2D(points[0].pos.x, curp.pos.y), points[0].pos };

                    for (int i = 1; i < positions.Length; i++) {
                        // Render lines
                        renderer.RenderLine(positions[i - 1], positions[i], LINE_THICKNESS, color, true);
                        // And labels
                        labels[i - 1].Start = positions[i - 1];
                        labels[i - 1].End = positions[i];
                        renderer.RenderText(labels[i - 1].TextLabel);
                    }

                    // Render vertices
                    for (int i = 0; i < 4; i++)
                        renderer.RenderRectangleFilled(new RectangleF(positions[i].x - vsize, positions[i].y - vsize, vsize * 2.0f, vsize * 2.0f), color, true);
                } else {
                    // Render vertex at cursor
                    renderer.RenderRectangleFilled(new RectangleF(curp.pos.x - vsize, curp.pos.y - vsize, vsize * 2.0f, vsize * 2.0f), color, true);
                }

                // Done
                renderer.Finish();
            }

            // Done
            renderer.Present();
        }

        // Accepted
        override public void OnAccept() {
            Cursor.Current = Cursors.AppStarting;
            General.Settings.FindDefaultDrawSettings();

            // When we have a rectangle
            if (points.Count == 5) {
                // Make undo for the draw
                General.Map.UndoRedo.CreateUndo("Rectangle draw");

                // Make an analysis and show info
                string[] adjectives = new string[] { "gloomy", "sad", "unhappy", "lonely", "troubled", "depressed", "heartsick", "glum", "pessimistic", "bitter", "downcast" }; // aaand my english vocabulary ends here :)
                string word = adjectives[new Random().Next(adjectives.Length - 1)];
                string a = ((word[0] == 'a') || (word[0] == 'e') || (word[0] == 'o')) ? "an " : "a ";

                General.Interface.DisplayStatus(StatusType.Action, "Created " + a + word + " rectangle.");

                // Make the drawing
                if (!Tools.DrawLines(points)) {
                    // Drawing failed
                    // NOTE: I have to call this twice, because the first time only cancels this volatile mode
                    General.Map.UndoRedo.WithdrawUndo();
                    General.Map.UndoRedo.WithdrawUndo();
                    return;
                }

                // Snap to map format accuracy
                General.Map.Map.SnapAllToAccuracy();

                // Clear selection
                General.Map.Map.ClearAllSelected();

                // Update cached values
                General.Map.Map.Update();

                // Edit new sectors?
                List<Sector> newsectors = General.Map.Map.GetMarkedSectors(true);
                if (BuilderPlug.Me.EditNewSector && (newsectors.Count > 0))
                    General.Interface.ShowEditSectors(newsectors);

                // Update the used textures
                General.Map.Data.UpdateUsedTextures();

                // Map is changed
                General.Map.IsChanged = true;
            }

            // Done
            Cursor.Current = Cursors.Default;

            // Return to original mode
            General.Editing.ChangeMode(General.Editing.PreviousStableMode.Name);
        }

        // This draws a point at a specific location
        override public bool DrawPointAt(Vector2D pos, bool stitch, bool stitchline) {
            if (pos.x < General.Map.Config.LeftBoundary || pos.x > General.Map.Config.RightBoundary ||
                pos.y > General.Map.Config.TopBoundary || pos.y < General.Map.Config.BottomBoundary)
                return false;

            DrawnVertex newpoint = new DrawnVertex();
            newpoint.pos = pos;
            newpoint.stitch = stitch;
            newpoint.stitchline = stitchline;
            points.Add(newpoint);

            if (points.Count == 1) { //add point and labels
                labels.AddRange(new LineLengthLabel[] { new LineLengthLabel(), new LineLengthLabel(), new LineLengthLabel(), new LineLengthLabel() });
                Update();
            } else if (points[0].pos == points[1].pos) { //nothing is drawn
                points = new List<DrawnVertex>();
                FinishDraw();
            } else {
                //recreate vertices for final shape. 
                //I must be missing something simple here, but just adding new DrawnVertices to points 
                //produces errors when created sector intersects with already existing ones...
                bool stitchFirst = points[0].stitch;
                bool stitchLineFirst = points[0].stitchline;

                Vector2D[] points2D = new Vector2D[] { points[0].pos, new Vector2D(points[1].pos.x, points[0].pos.y), points[1].pos, new Vector2D(points[0].pos.x, points[1].pos.y), points[0].pos };
                points = new List<DrawnVertex>();

                base.DrawPointAt(points2D[0], stitchFirst, stitchLineFirst);
                for (int i = 1; i < points2D.Length; i++)
                    base.DrawPointAt(points2D[i], snaptogrid, snaptonearest);

                FinishDraw();
            }
            return true;
        }

        override public void RemovePoint() {
            if (points.Count > 0) points.RemoveAt(points.Count - 1);
            if (labels.Count > 0) labels = new List<LineLengthLabel>();
            Update();
        }
    }
}
