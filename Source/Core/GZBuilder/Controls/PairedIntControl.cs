#region ================== Namespaces

using System;
using System.Globalization;
using System.Windows.Forms;

#endregion

namespace CodeImp.DoomBuilder.GZBuilder.Controls
{
	public partial class PairedIntControl : UserControl
	{
		#region ================== Events

		public event EventHandler OnValuesChanged;

		#endregion

		#region ================== Variables

		private int defaultValue;
		private bool blockUpdate;
		private bool changed;

		#endregion

		#region ================== Properties

		public bool NonDefaultValue { get { return changed; } }
		public int DefaultValue { get { return defaultValue; } set { defaultValue = value; } }
		public int ButtonStep { get { return value1.ButtonStep; } set { value1.ButtonStep = value; value2.ButtonStep = value; } }

		#endregion

		public PairedIntControl() 
		{
			InitializeComponent();
		}

		public void SetValues(int val1, int val2, bool first) 
		{
			blockUpdate = true;

			if (first) 
			{
				value1.Text = val1.ToString(CultureInfo.InvariantCulture);
				value2.Text = val2.ToString(CultureInfo.InvariantCulture);
			} 
			else 
			{
				if (!string.IsNullOrEmpty(value1.Text) && value1.Text != val1.ToString(CultureInfo.InvariantCulture))
					value1.Text = string.Empty;

				if (!string.IsNullOrEmpty(value2.Text) && value2.Text != val2.ToString(CultureInfo.InvariantCulture))
					value2.Text = string.Empty;
			}

			blockUpdate = false;
		}

		public int GetValue1(int original) 
		{
			return value1.GetResult(original);
		}

		public int GetValue2(int original) 
		{
			return value2.GetResult(original);
		}

		private void CheckValues() 
		{
			changed = string.IsNullOrEmpty(value1.Text) || string.IsNullOrEmpty(value2.Text) 
				|| value1.GetResult(defaultValue) != defaultValue || value2.GetResult(defaultValue) != defaultValue;
			bReset.Visible = changed;

			if(!blockUpdate && OnValuesChanged != null) OnValuesChanged(this, EventArgs.Empty);
		}

		private void bReset_Click(object sender, EventArgs e) 
		{
			value1.Text = defaultValue.ToString(CultureInfo.InvariantCulture);
			value2.Text = defaultValue.ToString(CultureInfo.InvariantCulture);
			CheckValues();
		}

		private void value1_WhenTextChanged(object sender, EventArgs e) 
		{
			CheckValues();
		}
	}
}
