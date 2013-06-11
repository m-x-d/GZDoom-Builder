using System;
using System.Windows.Forms;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.GZBuilder.Tools;

namespace CodeImp.DoomBuilder.GZBuilder.Controls
{
	public partial class PairedFieldsControl : UserControl
	{
		private float defaultValue;
		private string field1;
		private string field2;

		public string Label { get { return label.Text; } set { label.Text = value; } }
		public float DefaultValue { get { return defaultValue; } set { defaultValue = value; } }
		public string Field1 { get { return field1; } set { field1 = value; } }
		public string Field2 { get { return field2; } set { field2 = value; } }
		public bool AllowDecimal { get { return value1.AllowDecimal; } set { value1.AllowDecimal = value; value2.AllowDecimal = value; } }
		
		public PairedFieldsControl() {
			InitializeComponent();
		}

		public void SetValuesFrom(UniFields fields) {
			float v1 = UDMFTools.GetFloat(fields, field1, defaultValue);
			string newValue = (value1.AllowDecimal ? String.Format("{0:0.0}", v1) : v1.ToString());
			value1.Text = ((!string.IsNullOrEmpty(value1.Text) && value1.Text != newValue) ? "" : newValue);

			float v2 = UDMFTools.GetFloat(fields, field2, defaultValue);
			newValue = (value2.AllowDecimal ? String.Format("{0:0.0}", v2) : v2.ToString());
			value2.Text = ((!string.IsNullOrEmpty(value2.Text) && value2.Text != newValue) ? "" : newValue);

			checkValues();
		}

		public void ApplyTo(UniFields fields, int min, int max) {
			if(value1.Text != "") 
				UDMFTools.SetFloat(fields, field1, General.Clamp(value1.GetResultFloat(defaultValue), min, max), defaultValue, false);
			if(value2.Text != "")
				UDMFTools.SetFloat(fields, field2, General.Clamp(value2.GetResultFloat(defaultValue), min, max), defaultValue, false);
		}

		private void checkValues() {
			bool changed = (string.IsNullOrEmpty(value1.Text) || string.IsNullOrEmpty(value2.Text));

			if(!changed)
				changed = (value1.GetResultFloat(defaultValue) != defaultValue || value2.GetResultFloat(defaultValue) != defaultValue);

			label.Enabled = changed;
			bReset.Visible = changed;
		}

		private void bReset_Click(object sender, EventArgs e) {
			string newValue = value1.AllowDecimal ? String.Format("{0:0.0}", defaultValue) : defaultValue.ToString();
			value1.Text = newValue;
			value2.Text = newValue;
			checkValues();
		}

		private void value1_WhenTextChanged(object sender, EventArgs e) {
			checkValues();
		}
	}
}
