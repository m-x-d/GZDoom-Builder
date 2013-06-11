using System;
using System.Windows.Forms;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.GZBuilder.Tools;

namespace CodeImp.DoomBuilder.GZBuilder.Controls
{
    public partial class FloatFieldsSlider : UserControl
    {
        private bool blockEvents;
		private float defaultValue;
		private string field;
		private float min;
		private float max;

		public float DefaultValue { get { return defaultValue; } set { defaultValue = value; } }
		public string Field { get { return field; } set { field = value; } }

        public FloatFieldsSlider() {
            InitializeComponent();
        }

        public void SetLimits(float min, float max) {
            blockEvents = true;

			this.min = min;
			this.max = max;
            trackBar1.Value = General.Clamp(trackBar1.Value, (int)(min * 10), (int)(max * 10));
            trackBar1.Minimum = (int)(min * 10);
            trackBar1.Maximum = (int)(max * 10);

            blockEvents = false;
        }

		public void SetValueFrom(UniFields fields) {
			float v1 = UDMFTools.GetFloat(fields, field, defaultValue);
			string newValue = String.Format("{0:0.0}", v1);
			nudValue.Text = ((!string.IsNullOrEmpty(nudValue.Text) && nudValue.Text != newValue) ? "" : newValue);
		}

		public void ApplyTo(UniFields fields) {
			if(nudValue.Text != "")
				UDMFTools.SetFloat(fields, field, General.Clamp(nudValue.GetResultFloat(defaultValue), min, max), defaultValue, false);
		}

        //events
        private void trackBar1_ValueChanged(object sender, EventArgs e) {
			if(!blockEvents) nudValue.Text = String.Format("{0:0.0}", (float)Math.Round(((float)trackBar1.Value / 10.0f), 2));
        }

		private void nudValue_WhenTextChanged(object sender, EventArgs e) {
			float value = nudValue.GetResultFloat(0.0f);

			if(value > max) {
				value = max;
				nudValue.Text = String.Format("{0:0.0}", value);
			} else if(value < min) {
				value = min;
				nudValue.Text = String.Format("{0:0.0}", value);
			}
			
			blockEvents = true;
			trackBar1.Value = General.Clamp((int)(value * 10), trackBar1.Minimum, trackBar1.Maximum);
			blockEvents = false;
		}
    }
}
