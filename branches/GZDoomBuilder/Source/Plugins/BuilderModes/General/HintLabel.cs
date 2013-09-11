using System;
using System.Drawing;
using CodeImp.DoomBuilder.Geometry;

namespace CodeImp.DoomBuilder.BuilderModes
{
	public class HintLabel : LineLengthLabel
	{
		private const int TEXT_CAPACITY = 32;
		private const float TEXT_SCALE = 10f;

		private string text = "";
		public string Text {
			get {
				return text;
			}
			set {
				text = value;
				Update();
			}
		}

		public HintLabel() : base(false) {
			label.Color = General.Colors.BrightColors[new Random().Next(General.Colors.BrightColors.Length - 1)];
		}

		protected override void Update() {
			Vector2D delta = end - start;
			float length = delta.GetLength();
			label.Text = text;
			label.Rectangle = new RectangleF(start.x + delta.x * 0.5f, start.y + delta.y * 0.5f, 0f, 0f);
		}
	}
}
