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

namespace CodeImp.DoomBuilder.BuilderModes.ClassicModes {
    [EditMode(DisplayName = "Draw Ellipse Mode",
              SwitchAction = "drawellipsemode",
              AllowCopyPaste = false,
              Volatile = true,
              Optional = false)]
    
    public class DrawEllipseMode : DrawRectangleMode {
        public DrawEllipseMode() : base() {
            maxSubdivisions = 32;
            minSubdivisions = 6;
            undoName = "Ellipse draw";
            shapeName = "ellipse";

            subdivisions = minSubdivisions + 2;
        }

        override protected Vector2D[] getShape(Vector2D pStart, Vector2D pEnd) {
            //no shape
            if (pEnd.x == pStart.x && pEnd.y == pStart.y)
                return new Vector2D[0];

            //got shape
            int bevelSign = (bevelWidth > 0 ? 1 : -1);
            currentBevelWidth = Math.Min(Math.Abs(bevelWidth), Math.Min(width, height) / 2) * bevelSign;

            Vector2D[] shape = new Vector2D[subdivisions + 1];

            bool doBevel = false;
            int hw = width / 2;
            int hh = height / 2;

            Vector2D center = new Vector2D(pStart.x + hw, pStart.y + hh);
            float curAngle = 0;
            float angleStep = -(float)Math.PI / subdivisions * 2;
            int px, py;

            for (int i = 0; i < subdivisions; i++) {
                if (doBevel) {
                    px = (int)(center.x - (float)Math.Sin(curAngle) * (hw + currentBevelWidth));
                    py = (int)(center.y - (float)Math.Cos(curAngle) * (hh + currentBevelWidth));
                } else {
                    px = (int)(center.x - (float)Math.Sin(curAngle) * hw);
                    py = (int)(center.y - (float)Math.Cos(curAngle) * hh);
                }
                doBevel = !doBevel;
                shape[i] = new Vector2D(px, py);
                curAngle += angleStep;
            }
            //add final point
            shape[subdivisions] = shape[0];
            return shape;
        }

        protected override string getHintText() {
            return "BVL: "+bevelWidth+"; VERTS: "+subdivisions;
        }

//ACTIONS
        override protected void increaseSubdivLevel() {
            if (subdivisions < maxSubdivisions) {
                subdivisions += 2;
                Update();
            }
        }

        override protected void decreaseSubdivLevel() {
            if (subdivisions > minSubdivisions) {
                subdivisions -= 2;
                Update();
            }
        }
    }
}
