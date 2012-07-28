using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Globalization;

namespace CodeImp.DoomBuilder.UDMFControls
{
    public partial class FloatSlider : UserControl
    {
        private bool blockEvents;
        public event EventHandler OnValueChanged;

        public float Value {
            get {
                return (float)Math.Round(numericUpDown1.Value, 1);
            }
            set {
                blockEvents = true;
                float val = General.Clamp(value, (float)numericUpDown1.Minimum, (float)numericUpDown1.Maximum);
                previousValue = (int)(val * 10f);
                numericUpDown1.Value = (decimal)val;
                blockEvents = false;
            }
        }

        private float previousValue;
        private float delta;
        public float Delta { get { return delta; } }

        private bool showLabels = true;
        public bool ShowLabels {
            get {
                return showLabels;
            }
            set {
                showLabels = value;
                labelMin.Visible = showLabels;
                labelMax.Visible = showLabels;
            }
        }

        public FloatSlider() {
            InitializeComponent();
            ShowLabels = showLabels;
            numericUpDown1.DecimalPlaces = 1;
        }

        public void SetLimits(float min, float max, bool extendedLimits) {
            blockEvents = true;

            trackBar1.Value = General.Clamp(trackBar1.Value, (int)(min * 10), (int)(max * 10));
            trackBar1.Minimum = (int)(min * 10);
            trackBar1.Maximum = (int)(max * 10);

            labelMin.Text = min.ToString(CultureInfo.InvariantCulture);
            labelMax.Text = max.ToString(CultureInfo.InvariantCulture);

            numericUpDown1.Value = (decimal)General.Clamp((float)numericUpDown1.Value, min, max);

            if (extendedLimits) {
                numericUpDown1.Minimum = (decimal)(min * 32);
                numericUpDown1.Maximum = (decimal)(max * 32);
            } else {
                numericUpDown1.Minimum = (decimal)min;
                numericUpDown1.Maximum = (decimal)max;
            }

            blockEvents = false;
        }

        //events
        private void trackBar1_ValueChanged(object sender, EventArgs e) {
            if (!blockEvents) numericUpDown1.Value = Math.Round((decimal)(trackBar1.Value / 10.0), 1);
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e) {
            float value = (float)Math.Round(numericUpDown1.Value, 1);
            delta = (float)Math.Round(value - previousValue, 1);
            previousValue = value;

            if (!blockEvents && OnValueChanged != null)
                OnValueChanged(this, EventArgs.Empty);

            blockEvents = true;
            trackBar1.Value = General.Clamp((int)(value * 10), trackBar1.Minimum, trackBar1.Maximum);
            blockEvents = false;
        }
    }
}
