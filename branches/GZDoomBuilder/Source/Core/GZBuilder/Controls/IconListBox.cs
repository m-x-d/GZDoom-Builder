using System.Windows.Forms;
using System.Drawing;
using CodeImp.DoomBuilder.GZBuilder.Data;

namespace CodeImp.DoomBuilder.GZBuilder.Controls
{
    internal class IconListBox : ListBox
    {
        public IconListBox() {
            this.DrawMode = DrawMode.OwnerDrawFixed;
        }

        protected override void OnDrawItem(DrawItemEventArgs e) {
            e.DrawBackground();
            e.DrawFocusRectangle();

            if(Items.Count == 0) return; //sanity check

            LinedefColorPreset preset = (LinedefColorPreset)Items[e.Index];

            //draw color rectangle
            e.Graphics.FillRectangle(new SolidBrush(preset.Color.ToColor()), 2, e.Bounds.Top + 2, 12, 8);
            e.Graphics.DrawRectangle(new Pen(Color.Black), 2, e.Bounds.Top + 2, 12, 8);

            //draw text
            e.Graphics.DrawString(preset.ToString(), e.Font, new SolidBrush(e.ForeColor), e.Bounds.Left + 16, e.Bounds.Top);

            base.OnDrawItem(e);
        }
    }
}
