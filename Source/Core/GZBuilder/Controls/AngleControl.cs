#region Namespaces

//Downloaded from
//Visual C# Kicks - http://vckicks.110mb.com
//The Code Project - http://www.codeproject.com

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using CodeImp.DoomBuilder.Geometry;

#endregion

namespace CodeImp.DoomBuilder.GZBuilder.Controls
{
	public partial class AngleControl : UserControl
	{
		#region Variables

		private int angle;

		private Rectangle drawRegion;
		private const int drawOffset = 2;
		private Point origin;

		//UI colors
		private readonly Color fillColor = Color.FromArgb(90, 255, 255, 255);
		private readonly Color fillInactiveColor = SystemColors.InactiveCaption;
		private readonly Color outlineColor = Color.FromArgb(86, 103, 141);
		private readonly Color outlineInactiveColor = SystemColors.InactiveBorder;

		#endregion

		#region Properties

		public delegate void AngleChangedDelegate();
		public event AngleChangedDelegate AngleChanged;

		public int Angle { get { return angle; } set { angle = value; this.Refresh(); } }

		#endregion

		public AngleControl() 
		{
			InitializeComponent();
			this.DoubleBuffered = true;
		}

		#region Methods

		private void setDrawRegion() 
		{
			drawRegion = new Rectangle(0, 0, this.Width, this.Height);
			drawRegion.X += 2;
			drawRegion.Y += 2;
			drawRegion.Width -= 4;
			drawRegion.Height -= 4;

			origin = new Point(drawRegion.Width / 2 + drawOffset, drawRegion.Height / 2 + drawOffset);

			this.Refresh();
		}

		private static PointF DegreesToXY(float degrees, float radius, Point origin) 
		{
			PointF xy = new PointF();
			float radians = degrees * Angle2D.PI / 180.0f;

			xy.X = (float)Math.Cos(radians) * radius + origin.X;
			xy.Y = (float)Math.Sin(-radians) * radius + origin.Y;

			return xy;
		}

		private static int XYToDegrees(Point xy, Point origin) 
		{
			float xDiff = xy.X - origin.X;
			float yDiff = xy.Y - origin.Y;
			return ((int)Math.Round(Math.Atan2(-yDiff, xDiff) * 180.0 / Angle2D.PI) + 360) % 360;
		}

		#endregion

		#region Events

		private void AngleSelector_Load(object sender, EventArgs e) 
		{
			setDrawRegion();
		}

		private void AngleSelector_SizeChanged(object sender, EventArgs e) 
		{
			this.Height = this.Width; //Keep it a square
			setDrawRegion();
		}

		protected override void OnPaint(PaintEventArgs e) 
		{
			Graphics g = e.Graphics;

			Pen outline;
			Pen needle;
			SolidBrush fill;
			Brush center;

			if (this.Enabled) {
				outline = new Pen(outlineColor, 2.0f);
				fill = new SolidBrush(fillColor);
				needle = Pens.Black;
				center = Brushes.Black;
			} else {
				outline = new Pen(outlineInactiveColor, 2.0f);
				fill = new SolidBrush(fillInactiveColor);
				needle = Pens.DarkGray;
				center = Brushes.DarkGray;
			}

			Rectangle originSquare = new Rectangle(origin.X - 1, origin.Y - 1, 3, 3);

			//Draw
			g.SmoothingMode = SmoothingMode.AntiAlias;
			g.DrawEllipse(outline, drawRegion);
			g.FillEllipse(fill, drawRegion);

			if (angle != int.MinValue) {
				PointF anglePoint = DegreesToXY(angle, origin.X - 2, origin);
				g.DrawLine(needle, origin, anglePoint);
			}

			g.SmoothingMode = SmoothingMode.HighSpeed; //Make the square edges sharp
			g.FillRectangle(center, originSquare);

			fill.Dispose();
			outline.Dispose();

			base.OnPaint(e);
		}

		private void AngleSelector_MouseDown(object sender, MouseEventArgs e) 
		{
			int thisAngle = XYToDegrees(new Point(e.X, e.Y), origin);

			if (e.Button == MouseButtons.Left) {
				thisAngle = (int)Math.Round(thisAngle / 45f) * 45;
				if(thisAngle == 360) thisAngle = 0;
			}

			if(thisAngle != this.Angle) {
				this.Angle = thisAngle;
				if(!this.DesignMode && AngleChanged != null) AngleChanged(); //Raise event
				this.Refresh();
			}
		}

		private void AngleSelector_MouseMove(object sender, MouseEventArgs e) 
		{
			if (e.Button == MouseButtons.Left || e.Button == MouseButtons.Right) {
				int thisAngle = XYToDegrees(new Point(e.X, e.Y), origin);

				if(e.Button == MouseButtons.Left) {
					thisAngle = (int)Math.Round(thisAngle / 45f) * 45;
					if(thisAngle == 360) thisAngle = 0;
				}

				if(thisAngle != this.Angle) {
					this.Angle = thisAngle;
					if(!this.DesignMode && AngleChanged != null) AngleChanged(); //Raise event
					this.Refresh();
				}
			}
		}

		#endregion
	}
}
