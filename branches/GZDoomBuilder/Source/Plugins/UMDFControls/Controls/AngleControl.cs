using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace CodeImp.DoomBuilder.UDMFControls
{
    public partial class AngleControl : UserControl
    {
        private const float RADIANS_PER_DEGREE = (float)Math.PI / 180.0f;
        private const float DEGREES_PER_RADIAN = 180.0f / (float)Math.PI;

        private bool blockEvents;

        //draw related
        private Pen penRed;
        private Point center;
        private int needleLength;

        //events
        public event EventHandler OnAngleChanged;

        private float angle;
        public float Value {
            get {
                return (float)nudAngle.Value;
            }
            set {
                prevAngle = (float)nudAngle.Value;
                angle = General.ClampAngle(359 - value);
                blockEvents = true;
                nudAngle.Value = (decimal)General.ClampAngle(value);
                blockEvents = false;
                panelAngleControl.Invalidate();
            }
        }

        private float prevAngle;
        public float Delta { get { return (float)nudAngle.Value - prevAngle; } }

        public bool SnapAngle;

        public AngleControl() {
            InitializeComponent();

            penRed = new Pen(Color.Red, 2.0f);
            center = new Point(panelAngleControl.Width / 2, panelAngleControl.Height / 2);
            needleLength = center.X - 4;
            angle = 0;

            //events
            panelAngleControl.MouseDown += new MouseEventHandler(panelAngleControl_MouseDown);
            panelAngleControl.MouseUp += new MouseEventHandler(panelAngleControl_MouseUp);
        }

        private void update() {
            //redraw
            panelAngleControl.Invalidate();

            //dispatch event
            if (OnAngleChanged != null) OnAngleChanged(this, EventArgs.Empty);
        }

        private int calcDegrees(Point pt) {
            int degrees;

            if (pt.X == 0) {
                // The point is on the y-axis. Determine whether it's above or below the x-axis, and return the 
                // corresponding angle. Note that the orientation of the y-coordinate is backwards. That is, 
                // A positive Y value indicates a point BELOW the x-axis.
                if (pt.Y > 0) degrees = 270;
                else degrees = 90;
            } else {
                // This value needs to be multiplied by -1 because the y-coordinate is opposite from the normal direction here.
                // That is, a y-coordinate that's "higher" on the form has a lower y-value, in this coordinate
                // system. So everything's off by a factor of -1 when performing the ratio calculations.
                degrees = (int)(-Math.Atan((double)pt.Y / pt.X) * DEGREES_PER_RADIAN);

                // If the x-coordinate of the selected point is to the left of the center of the circle, you 
                // need to add 180 degrees to the angle. ArcTan only gives you a value on the right-hand side 
                // of the circle.
                if (pt.X < 0) degrees += 180;

                // Ensure that the return value is between 0 and 360.
                degrees = General.ClampAngle(degrees);
            }
            return degrees;
        }

        //events
        private void nudAngle_ValueChanged(object sender, EventArgs e) {
            if (!blockEvents) {
                prevAngle = angle;
                angle = (int)((NumericUpDown)sender).Value;
                update();
            }
        }

        private void panelAngleControl_Paint(object sender, PaintEventArgs e) {
            //angle line
            float angleDeg = (float)((angle + 450) % 360) * RADIANS_PER_DEGREE;
            int px = center.X + (int)(Math.Sin(angleDeg) * (float)needleLength);
            int py = center.Y + (int)(Math.Cos(angleDeg) * (float)needleLength);

            e.Graphics.DrawLine(penRed, center, new Point(px, py));
        }

        //mouse events
        private void panelAngleControl_MouseDown(object sender, MouseEventArgs e) {
            Point delta = new Point(e.X - center.X, e.Y - center.Y);
            int distance = (int)(Math.Sqrt(delta.X * delta.X + delta.Y * delta.Y) / center.X); //center.X == dial radius

            if (distance < center.X)  //clicked inside dial
                panelAngleControl.MouseMove += new MouseEventHandler(panelAngleControl_MouseMove);
        }

        private void panelAngleControl_MouseUp(object sender, MouseEventArgs e) {
            panelAngleControl.MouseMove -= panelAngleControl_MouseMove;

            prevAngle = (float)nudAngle.Value;
            if (SnapAngle)
                angle = (((int)(Math.Round((float)calcDegrees(new Point(e.X - center.X, e.Y - center.Y)) / 45f)) * 45) + 359) % 360;
            else
                angle = calcDegrees(new Point(e.X - center.X, e.Y - center.Y));

            blockEvents = true;
            nudAngle.Value = (decimal)(359f - angle);
            blockEvents = false;
            update();

            //reset snap state
            SnapAngle = false;
        }

        private void panelAngleControl_MouseMove(object sender, MouseEventArgs e) {
            prevAngle = (float)nudAngle.Value;
            angle = calcDegrees(new Point(e.X - center.X, e.Y - center.Y));
            blockEvents = true;
            nudAngle.Value = (decimal)(359f - angle);
            blockEvents = false;
            update();
        }
    }
}
