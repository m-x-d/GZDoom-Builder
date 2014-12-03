using System.Drawing;
using CodeImp.DoomBuilder.Geometry;

namespace CodeImp.DoomBuilder.BuilderModes
{
	public class HintLabel : LineLengthLabel
	{
		private string text = "";
		public string Text { get { return text; } set { text = value; Update(); } }

		public HintLabel() : base(false) 
		{
			label.Color = General.Colors.InfoLine;
		}

		protected override void Update() 
		{
			Vector2D delta = end - start;
			label.Text = text;
			label.Rectangle = new RectangleF(start.x + delta.x * 0.5f, start.y + delta.y * 0.5f, 0f, 0f);
		}
	}
}
