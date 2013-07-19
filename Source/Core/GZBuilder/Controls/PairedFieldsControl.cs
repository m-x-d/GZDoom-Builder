#region ================== Namespaces

using System;
using System.Windows.Forms;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.GZBuilder.Tools;
using CodeImp.DoomBuilder.Properties;

#endregion

namespace CodeImp.DoomBuilder.GZBuilder.Controls
{
	public partial class PairedFieldsControl : UserControl
	{
		#region ================== Events

		public event EventHandler OnValuesChanged;

		#endregion

		#region ================== Variables

		private float defaultValue;
		private string field1;
		private string field2;
		private bool allowValueLinking;
		private bool linkValues;
		private bool blockUpdate;
		private int bResetPosX;

		#endregion

		#region ================== Properties

		public string Label { get { return label.Text; } set { label.Text = value; } }
		public float DefaultValue { get { return defaultValue; } set { defaultValue = value; } }
		public string Field1 { get { return field1; } set { field1 = value; } }
		public string Field2 { get { return field2; } set { field2 = value; } }
		public bool AllowDecimal { get { return value1.AllowDecimal; } set { value1.AllowDecimal = value; value2.AllowDecimal = value; } }
		public int ButtonStep { get { return value1.ButtonStep; } set { value1.ButtonStep = value; value2.ButtonStep = value; } }
		public float ButtonStepFloat { get { return value1.ButtonStepFloat; } set { value1.ButtonStepFloat = value; value2.ButtonStepFloat = value; } }
		public bool AllowValueLinking { get { return allowValueLinking; } set { allowValueLinking = value; updateButtons(); } }
		public bool LinkValues { get { return linkValues; } set { linkValues = value; updateButtons(); } }

		#endregion

		#region ================== Constructor

		public PairedFieldsControl() {
			InitializeComponent();
			bResetPosX = bReset.Left;
		}

		#endregion

		#region ================== Methods

		public void SetValuesFrom(UniFields fields, bool first) {
			blockUpdate = true;
			
			string newValue1;
			string newValue2;

			if(AllowDecimal) {
				float val1 = UDMFTools.GetFloat(fields, field1, defaultValue);
				newValue1 = (val1 == Math.Round(val1) ? val1.ToString("0.0") : val1.ToString());

				float val2 = UDMFTools.GetFloat(fields, field2, defaultValue);
				newValue2 = (val2 == Math.Round(val2) ? val2.ToString("0.0") : val2.ToString());
			} else {
				newValue1 = UDMFTools.GetFloat(fields, field1, defaultValue).ToString();
				newValue2 = UDMFTools.GetFloat(fields, field2, defaultValue).ToString();
			}

			if(first) {
				value1.Text = newValue1;
				value2.Text = newValue2;
			} else {
				value1.Text = ((!string.IsNullOrEmpty(value1.Text) && value1.Text != newValue1) ? "" : newValue1);
				value2.Text = ((!string.IsNullOrEmpty(value2.Text) && value2.Text != newValue2) ? "" : newValue2);
			}
			checkValues();

			blockUpdate = false;
		}

		public void ApplyTo(UniFields fields, int min, int max, float oldValue1, float oldValue2) {
			if(value1.Text != "")
				UDMFTools.SetFloat(fields, field1, General.Clamp(value1.GetResultFloat(defaultValue), min, max), defaultValue, false);
			else
				UDMFTools.SetFloat(fields, field1, oldValue1, defaultValue, false);

			if(value2.Text != "")
				UDMFTools.SetFloat(fields, field2, General.Clamp(value2.GetResultFloat(defaultValue), min, max), defaultValue, false);
			else
				UDMFTools.SetFloat(fields, field2, oldValue2, defaultValue, false);
		}

		private void checkValues() {
			bool changed = (string.IsNullOrEmpty(value1.Text) || string.IsNullOrEmpty(value2.Text));

			if(!changed)
				changed = (value1.GetResultFloat(defaultValue) != defaultValue || value2.GetResultFloat(defaultValue) != defaultValue);

			label.Enabled = changed;
			bReset.Visible = changed;

			if(!blockUpdate && OnValuesChanged != null)	OnValuesChanged(this, EventArgs.Empty);
		}

		private void updateButtons() {
			bLink.Visible = allowValueLinking;
			
			if(!allowValueLinking) {
				bReset.Left = bLink.Left;
			} else {
				bReset.Left = bResetPosX;
				bLink.Image = (linkValues ? Resources.Link : Resources.Unlink);
			}
		}

		#endregion

		#region ================== Events

		private void bLink_Click(object sender, EventArgs e) {
			linkValues = !linkValues;
			bLink.Image = (linkValues ? Resources.Link : Resources.Unlink);
		}

		private void bReset_Click(object sender, EventArgs e) {
			string newValue = value1.AllowDecimal ? String.Format("{0:0.0}", defaultValue) : defaultValue.ToString();
			value1.Text = newValue;
			value2.Text = newValue;
			checkValues();
		}

		private void value1_WhenTextChanged(object sender, EventArgs e) {
			if(blockUpdate) return;
			
			if(linkValues) {
				blockUpdate = true;
				value2.Text = value1.Text;
				blockUpdate = false;
			}
			
			checkValues();
		}

		private void value2_WhenTextChanged(object sender, EventArgs e) {
			if(blockUpdate)	return;

			if(linkValues) {
				blockUpdate = true;
				value1.Text = value2.Text;
				blockUpdate = false;
			}

			checkValues();
		}

		#endregion
	}
}
