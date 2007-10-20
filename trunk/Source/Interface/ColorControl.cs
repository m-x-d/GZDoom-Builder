using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using CodeImp.DoomBuilder.Rendering;

namespace CodeImp.DoomBuilder.Interface
{
	public partial class ColorControl : UserControl
	{
		// Constructor
		public ColorControl()
		{
			// Initialize
			InitializeComponent();
		}

		// Properties
		public string Label { get { return label.Text; } set { label.Text = value; } }
		public PixelColor Color { get { return PixelColor.FromColor(panel.BackColor); } set { panel.BackColor = System.Drawing.Color.FromArgb(value.ToInt()); } }

		// Button clicked
		private void button_Click(object sender, EventArgs e)
		{
			// Show color dialog
			dialog.Color = panel.BackColor;
			if(dialog.ShowDialog(this.ParentForm) == DialogResult.OK)
			{
				// Apply new color
				panel.BackColor = dialog.Color;
			}
		}

		// Resized
		private void ColorControl_Resize(object sender, EventArgs e)
		{
			try
			{
				button.Left = ClientSize.Width - button.Width;
				panel.Left = ClientSize.Width - button.Width - panel.Width - 3;
				label.Left = 0;
				label.Width = panel.Left;
			}
			catch(Exception) { }
		}
	}
}
