using System;
using System.Windows.Forms;
using System.Drawing;
using CodeImp.DoomBuilder.GZBuilder.Data;

namespace CodeImp.DoomBuilder.GZBuilder.Controls
{
	internal class IconListBox : ListBox
	{
		public IconListBox() 
		{
			this.DrawMode = DrawMode.OwnerDrawVariable;
		}

		protected override void OnMeasureItem(MeasureItemEventArgs e) 
		{
			e.ItemHeight = (int)Math.Ceiling(this.Font.Height * (e.Graphics.DpiY / 96.0f));
		}

		protected override void OnDrawItem(DrawItemEventArgs e) 
		{
			if(Items.Count == 0) return; //sanity check

			e.DrawBackground();
			e.DrawFocusRectangle();

			LinedefColorPreset preset = (LinedefColorPreset)Items[e.Index];

			//draw color rectangle
			e.Graphics.FillRectangle(new SolidBrush(preset.Color.ToColor()), 2, e.Bounds.Top + 2, e.Bounds.Height, e.Bounds.Height - 5);
			e.Graphics.DrawRectangle(new Pen(Color.Black), 2, e.Bounds.Top + 2, e.Bounds.Height, e.Bounds.Height - 5);

			//draw text
			e.Graphics.DrawString(preset.ToString(), e.Font, new SolidBrush(e.ForeColor), e.Bounds.Left + e.Bounds.Height + 4, e.Bounds.Top);
		}
	}
}
