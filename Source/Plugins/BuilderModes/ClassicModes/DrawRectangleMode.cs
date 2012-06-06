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
        //private LineLengthLabel hintLabel;
        private int bevelWidth;
        private int subdivisions;

        private const int MAX_SUBDIVISIONS = 16;

        private Vector2D start;
        private Vector2D end;
        private int width;
        private int height;

        public DrawRectangleMode() : base() {
            snaptogrid = true;
        }
        
        override protected void Update() {
            PixelColor stitchcolor = General.Colors.Highlight;
            PixelColor losecolor = General.Colors.Selection;
            PixelColor color;

            snaptonearest = General.Interface.CtrlState ^ General.Interface.AutoMerge;

            DrawnVertex curp = GetCurrentPosition();
            float vsize = ((float)renderer.VertexSize + 1.0f) / renderer.Scale;
            float vsizeborder = ((float)renderer.VertexSize + 3.0f) / renderer.Scale;

            // Render drawing lines
            if (renderer.StartOverlay(true)) {
                color = snaptogrid ? stitchcolor : losecolor;
                
                if (points.Count == 1) {
                    updateReferencePoints(points[0], curp);
                    Vector2D[] shape = getShape(start, end);

                    //render shape
                    for (int i = 1; i < shape.Length; i++)
                        renderer.RenderLine(shape[i - 1], shape[i], LINE_THICKNESS, color, true);

                    //and labels
                    Vector2D[] labelCoords = new Vector2D[]{start, new Vector2D(end.x, start.y), end, new Vector2D(start.x, end.y), start};
                    for (int i = 1; i < 5; i++) {
                        labels[i - 1].Start = labelCoords[i - 1];
                        labels[i - 1].End = labelCoords[i];
                        renderer.RenderText(labels[i - 1].TextLabel);
                    }

                    //render hint
                    /*if (width > 64 * vsize && height > 32 * vsize) {
                        float vPos = start.y + height / 2.0f;
                        hintLabel.Start = new Vector2D(start.x, vPos);
                        hintLabel.End = new Vector2D(end.x, vPos);
                        renderer.RenderText(hintLabel.TextLabel); //todo: extend LieLengthLabel class
                    }*/

                    // Render vertices
                    for (int i = 0; i < shape.Length; i++)
                        renderer.RenderRectangleFilled(new RectangleF(shape[i].x - vsize, shape[i].y - vsize, vsize * 2.0f, vsize * 2.0f), color, true);
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

        //update top-left and bottom-right points, which define drawing shape
        private void updateReferencePoints(DrawnVertex p1, DrawnVertex p2) {
            if (p1.pos.x < p2.pos.x) {
                start.x = p1.pos.x;
                end.x = p2.pos.x;
            } else {
                start.x = p2.pos.x;
                end.x = p1.pos.x;
            }

            if (p1.pos.y < p2.pos.y) {
                start.y = p1.pos.y;
                end.y = p2.pos.y;
            } else {
                start.y = p2.pos.y;
                end.y = p1.pos.y;
            }

            width = (int)(end.x - start.x);
            height = (int)(end.y - start.y);
        }

        private Vector2D[] getShape(Vector2D pStart, Vector2D pEnd) {
            //no shape
            if (pEnd.x == pStart.x && pEnd.y == pStart.y)
                return new Vector2D[0];

            //no corners
            if (bevelWidth == 0)
                return new Vector2D[] { pStart, new Vector2D((int)pEnd.x, (int)pStart.y), pEnd, new Vector2D((int)pStart.x, (int)pEnd.y), pStart };

            //got corners
            bool reverse = false;
            int bevel = Math.Min(Math.Abs(bevelWidth), Math.Min(width, height) / 2);
            if (subdivisions > 0 && bevelWidth < 0) {
                bevel *= -1;
                reverse = true;
            }

            List<Vector2D> l_points = new List<Vector2D>();

            //top-left corner
            l_points.AddRange(getCornerPoints(pStart, bevel, bevel, !reverse));

            //top-right corner
            l_points.AddRange(getCornerPoints(new Vector2D(pEnd.x, pStart.y), -bevel, bevel, reverse));

            //bottom-right corner
            l_points.AddRange(getCornerPoints(pEnd, -bevel, -bevel, !reverse));

            //bottom-left corner
            l_points.AddRange(getCornerPoints(new Vector2D(pStart.x, pEnd.y), bevel, -bevel, reverse));

            //closing point
            l_points.Add(l_points[0]);

            Vector2D[] points = new Vector2D[l_points.Count];
            l_points.CopyTo(points);
            return points;
        }

        private Vector2D[] getCornerPoints(Vector2D startPoint, int bevel_width, int bevel_height, bool reverse) {
            Vector2D[] points;
            if (subdivisions == 0) {
                points = new Vector2D[2];
                points[0] = new Vector2D(startPoint.x, startPoint.y + bevel_height);
                points[1] = new Vector2D(startPoint.x + bevel_width, startPoint.y);

                if (!reverse) Array.Reverse(points);
                return points;
            }

            Vector2D center = (bevelWidth > 0 ? new Vector2D(startPoint.x + bevel_width, startPoint.y + bevel_height) : startPoint);
            float curAngle = (float)Math.PI;

            int steps = subdivisions + 2;
            points = new Vector2D[steps];
            float stepAngle = (float)Math.PI / 2.0f / (subdivisions + 1);

            for (int i = 0; i < steps; i++) {
                points[i] = new Vector2D(center.x + (float)Math.Sin(curAngle) * bevel_width, center.y + (float)Math.Cos(curAngle) * bevel_height);
                curAngle += stepAngle;
            }

            if (reverse) Array.Reverse(points);
            return points;
        }

        // This draws a point at a specific location
        override public bool DrawPointAt(Vector2D pos, bool stitch, bool stitchline) {
            if (pos.x < General.Map.Config.LeftBoundary || pos.x > General.Map.Config.RightBoundary ||
                pos.y > General.Map.Config.TopBoundary || pos.y < General.Map.Config.BottomBoundary)
                return false;

            DrawnVertex newpoint = new DrawnVertex();
            newpoint.pos = pos;
            newpoint.stitch = true; //stitch
            newpoint.stitchline = stitchline;
            points.Add(newpoint);

            if (points.Count == 1) { //add point and labels
                labels.AddRange(new LineLengthLabel[] { new LineLengthLabel(), new LineLengthLabel(), new LineLengthLabel(), new LineLengthLabel() });
                //hintLabel = new LineLengthLabel();
                Update();
            } else if (points[0].pos == points[1].pos) { //nothing is drawn
                points = new List<DrawnVertex>();
                FinishDraw();
            } else {
                //create vertices for final shape. 
                updateReferencePoints(points[0], newpoint);
                points = new List<DrawnVertex>(); //clear points
                Vector2D[] shape = getShape(start, end);

                for (int i = 0; i < shape.Length; i++)
                    base.DrawPointAt(shape[i], true, true);

                FinishDraw();
            }
            return true;
        }

        override public void RemovePoint() {
            if (points.Count > 0) points.RemoveAt(points.Count - 1);
            if (labels.Count > 0) labels = new List<LineLengthLabel>();
            Update();
        }

//EVENTS
        override public void OnAccept() {
            Cursor.Current = Cursors.AppStarting;
            General.Settings.FindDefaultDrawSettings();

            // When we have a rectangle
            if (points.Count > 4) {
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

//ACTIONS
        [BeginAction("increasesubdivlevel")]
        private void increaseSubdivLevel() {
            if (subdivisions < MAX_SUBDIVISIONS) {
                subdivisions++;
                Update();
            }
        }

        [BeginAction("decreasesubdivlevel")]
        private void decreaseSubdivLevel() {
            if (subdivisions > 0) {
                subdivisions--;
                Update();
            }
        }

        [BeginAction("increasebevel")]
        private void increaseBevel() {
            bevelWidth += General.Map.Grid.GridSize;
            Update();
        }

        [BeginAction("decreasebevel")]
        private void decreaseBevel() {
            bevelWidth -= General.Map.Grid.GridSize;
            Update();
        }
    }
}
