//Downloaded from
//Visual C# Kicks - http://vckicks.110mb.com
//The Code Project - http://www.codeproject.com

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace CodeImp.DoomBuilder.GZBuilder.Controls
{
	public partial class AngleControl : UserControl
	{
		private int angle;

        private Rectangle drawRegion;
        private Point origin;

		public AngleControl()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
        }

        private void AngleSelector_Load(object sender, EventArgs e)
        {
            setDrawRegion();
        }

        private void AngleSelector_SizeChanged(object sender, EventArgs e)
        {
            this.Height = this.Width; //Keep it a square
            setDrawRegion();
        }

        private void setDrawRegion()
        {
            drawRegion = new Rectangle(0, 0, this.Width, this.Height);
            drawRegion.X += 2;
            drawRegion.Y += 2;
            drawRegion.Width -= 4;
            drawRegion.Height -= 4;

            int offset = 2;
            origin = new Point(drawRegion.Width / 2 + offset, drawRegion.Height / 2 + offset);

            this.Refresh();
        }

        public int Angle
        {
            get { return angle; }
            set
            {
                angle = value;
                this.Refresh();
            }
        }

        public delegate void AngleChangedDelegate();
        public event AngleChangedDelegate AngleChanged;

        private PointF DegreesToXY(float degrees, float radius, Point origin)
        {
            PointF xy = new PointF();
            double radians = degrees * Math.PI / 180.0;

            xy.X = (float)Math.Cos(radians) * radius + origin.X;
            xy.Y = (float)Math.Sin(-radians) * radius + origin.Y;

            return xy;
        }

        private float XYToDegrees(Point xy, Point origin)
        {
            double angle = 0.0;

            if (xy.Y < origin.Y)
            {
                if (xy.X > origin.X)
                {
                    angle = (double)(xy.X - origin.X) / (double)(origin.Y - xy.Y);
                    angle = Math.Atan(angle);
                    angle = 90.0 - angle * 180.0 / Math.PI;
                }
                else if (xy.X < origin.X)
                {
                    angle = (double)(origin.X - xy.X) / (double)(origin.Y - xy.Y);
                    angle = Math.Atan(-angle);
                    angle = 90.0 - angle * 180.0 / Math.PI;
                }
            }
            else if (xy.Y > origin.Y)
            {
                if (xy.X > origin.X)
                {
                    angle = (double)(xy.X - origin.X) / (double)(xy.Y - origin.Y);
                    angle = Math.Atan(-angle);
                    angle = 270.0 - angle * 180.0 / Math.PI;
                }
                else if (xy.X < origin.X)
                {
                    angle = (double)(origin.X - xy.X) / (double)(xy.Y - origin.Y);
                    angle = Math.Atan(angle);
                    angle = 270.0 - angle * 180.0 / Math.PI;
                }
            }

            if (angle > 180) angle -= 360; //Optional. Keeps values between -180 and 180
            return (float)angle;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            Pen outline = new Pen(Color.FromArgb(86, 103, 141), 2.0f);
            SolidBrush fill = new SolidBrush(Color.FromArgb(90, 255, 255, 255));

            PointF anglePoint = DegreesToXY(angle, origin.X - 2, origin);
            Rectangle originSquare = new Rectangle(origin.X - 1, origin.Y - 1, 3, 3);

            //Draw
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.DrawEllipse(outline, drawRegion);
            g.FillEllipse(fill, drawRegion);
            g.DrawLine(Pens.Black, origin, anglePoint);

            g.SmoothingMode = SmoothingMode.HighSpeed; //Make the square edges sharp
            g.FillRectangle(Brushes.Black, originSquare);

            fill.Dispose();
            outline.Dispose();

            base.OnPaint(e);
        }

        private void AngleSelector_MouseDown(object sender, MouseEventArgs e)
        {
            int thisAngle = findNearestAngle(new Point(e.X, e.Y));

            if (thisAngle != -1)
            {
                this.Angle = thisAngle;
				if(!this.DesignMode && AngleChanged != null)
					AngleChanged(); //Raise event
                this.Refresh();
            }
        }

        private void AngleSelector_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left || e.Button == MouseButtons.Right)
            {
                int thisAngle = findNearestAngle(new Point(e.X, e.Y));

                if (thisAngle != -1)
                {
                    this.Angle = thisAngle;
					if(!this.DesignMode && AngleChanged != null)
						AngleChanged(); //Raise event
                    this.Refresh();
                }
            }
        }

        private int findNearestAngle(Point mouseXY)
        {
            int thisAngle = (int)XYToDegrees(mouseXY, origin);
            if (thisAngle != 0)
                return thisAngle;
            else
                return -1;
        }
	}
}
