using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CodeImp.DoomBuilder.UDMFControls
{
    public partial class FloatSlider : UserControl
    {
        private bool blockEvents;
        public event EventHandler OnValueChanged;

        public float Value {
            get {
                return (float)trackBar1.Value / 10f;
            }
            set {
                blockEvents = true;
                numericUpDown1.Value = (decimal)General.Clamp(value, (float)numericUpDown1.Minimum, (float)numericUpDown1.Maximum);
                blockEvents = false;
            }
        }

        private int previousValue;
        private int delta;
        public float Delta { get { return (float)delta / 10f; } }

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

        public void SetLimits(float min, float max, bool doubledLimits) {
            blockEvents = true;

            trackBar1.Value = General.Clamp(trackBar1.Value, (int)(min * 10), (int)(max * 10));
            trackBar1.Minimum = (int)(min * 10);
            trackBar1.Maximum = (int)(max * 10);

            labelMin.Text = min.ToString();
            labelMax.Text = max.ToString();

            numericUpDown1.Value = (decimal)General.Clamp((float)numericUpDown1.Value, min, max);

            if (doubledLimits) {
                numericUpDown1.Minimum = (decimal)(min * 2);
                numericUpDown1.Maximum = (decimal)(max * 2);
            } else {
                numericUpDown1.Minimum = (decimal)min;
                numericUpDown1.Maximum = (decimal)max;
            }

            blockEvents = false;
        }

        //events
        private void trackBar1_ValueChanged(object sender, EventArgs e) {
             int value = ((TrackBar)sender).Value;
             delta = value - previousValue;
             previousValue = value;

             numericUpDown1.Value = Math.Round((decimal)(value / 10.0), 1);
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e) {
            float val = (float)((NumericUpDown)sender).Value;

            if (!blockEvents && OnValueChanged != null)
                OnValueChanged(this, EventArgs.Empty);

            blockEvents = true;
            trackBar1.Value = General.Clamp((int)(val * 10), trackBar1.Minimum, trackBar1.Maximum);
            blockEvents = false;
        }
    }
}
