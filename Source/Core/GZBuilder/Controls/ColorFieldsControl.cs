#region ================== Namespaces

using System;
using System.Drawing;
using System.Windows.Forms;
using System.Globalization;
using CodeImp.DoomBuilder.Rendering;
using CodeImp.DoomBuilder.Map;

#endregion

namespace CodeImp.DoomBuilder.GZBuilder.Controls
{
	public partial class ColorFieldsControl : UserControl
	{
		#region ================== Events

		public event EventHandler OnValueChanged; //mxd

		#endregion

		#region ================== Variables

		private int defaultValue;
		private string field;
		private bool blockUpdate;

		#endregion

		#region ================== Properties

		public int DefaultValue { get { return defaultValue; } set { defaultValue = value; } }
		public string Label { get { return cpColor.Label; } set { cpColor.Label = value; } }
		public string Field { get { return field; } set { field = value; } }

		#endregion
		
		public ColorFieldsControl() 
		{
			InitializeComponent();
		}

		public void SetValueFrom(UniFields fields) 
		{
			string newValue = String.Format("{0:X6}", UniFields.GetInteger(fields, field, defaultValue));
			tbColor.Text = ((!string.IsNullOrEmpty(tbColor.Text) && tbColor.Text != newValue) ? "" : newValue);
			CheckColor();
		}

		public void ApplyTo(UniFields fields, int oldValue) 
		{
			if(string.IsNullOrEmpty(tbColor.Text)) 
			{
				UniFields.SetInteger(fields, field, oldValue, defaultValue);
			} 
			else 
			{
				UniFields.SetInteger(fields, field, (cpColor.Color.ToInt() & 0x00ffffff), defaultValue);
			}
		}

		private void CheckColor() 
		{
			bool changed = string.IsNullOrEmpty(tbColor.Text) || (cpColor.Color.ToInt() & 0x00ffffff) != defaultValue;
			bReset.Visible = changed;
			tbColor.ForeColor = changed ? SystemColors.WindowText : SystemColors.GrayText;
		}

		#region ================== Events

		private void bReset_Click(object sender, EventArgs e) 
		{
			cpColor.Color = PixelColor.FromInt(defaultValue).WithAlpha(255);
			cpColor_ColorChanged(this, EventArgs.Empty);
		}

		private void cpColor_ColorChanged(object sender, EventArgs e) 
		{
			if(blockUpdate)	return;

			blockUpdate = true;
			tbColor.Text = String.Format("{0:X6}", (cpColor.Color.ToInt() & 0x00ffffff));
			blockUpdate = false;

			CheckColor();
			if(OnValueChanged != null) OnValueChanged(this, EventArgs.Empty);
		}

		private void tbColor_TextChanged(object sender, EventArgs e) 
		{
			if(blockUpdate)	return;
			int colorVal;

			if(int.TryParse(tbColor.Text, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out colorVal))
			{
				colorVal = General.Clamp(colorVal, 0, 16777215);

				blockUpdate = true;
				cpColor.Color = PixelColor.FromInt(colorVal).WithAlpha(255);
				blockUpdate = false;
			}

			CheckColor();
			if(OnValueChanged != null) OnValueChanged(this, EventArgs.Empty);
		}

		#endregion
	}
}
