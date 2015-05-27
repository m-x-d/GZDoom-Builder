using System.Windows.Forms;
using System.Drawing;

namespace CodeImp.DoomBuilder.Controls
{
	public class ColoredComboBox : ComboBox
	{
		public ColoredComboBox() 
		{ 
            this.DrawMode = DrawMode.OwnerDrawFixed; 
        }

        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            base.OnDrawItem(e);
            e.DrawBackground();
			ColoredComboBoxItem item = (ColoredComboBoxItem)this.Items[e.Index];
            Brush brush = new SolidBrush(item.ForeColor);
            if ((e.State & DrawItemState.Selected) == DrawItemState.Selected) brush = Brushes.White;
            e.Graphics.DrawString(item.Text, this.Font, brush, e.Bounds.X, e.Bounds.Y);
			brush.Dispose(); //mxd
        }
	}

	public class ColoredComboBoxItem
	{
		private string text = "";
		private object value;
		Color forecolor = Color.Black;

		public string Text { get { return text; } set { text = value; } }
		public object Value { get { return value; } set { this.value = value; } }
		public Color ForeColor { get { return forecolor; } set { forecolor = value; } }
		
		public ColoredComboBoxItem() { }

		public ColoredComboBoxItem(object value) 
		{
			this.text = value.ToString();
			this.value = value;
		}

		public ColoredComboBoxItem(object value, Color forecolor) 
		{
			this.text = value.ToString();
			this.value = value;
			this.forecolor = forecolor;
		}

		public override string ToString() 
		{
			return text;
		}
	}
}
