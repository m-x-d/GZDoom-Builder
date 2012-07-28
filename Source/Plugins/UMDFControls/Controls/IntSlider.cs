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
    public partial class IntSlider : UserControl
    {

        private bool blockEvents;
        public event EventHandler OnValueChanged;

        private int previousValue;
        public int Value {
            get {
                return (int)numericUpDown1.Value;
            }
            set {
                blockEvents = true;

                previousValue = General.Clamp(value, (int)numericUpDown1.Minimum, (int)numericUpDown1.Maximum);
                numericUpDown1.Value = previousValue;
                blockEvents = false;
            }
        }

        private int delta;
        public int Delta { get { return delta; } }

        private bool showLabels;
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

        public IntSlider() {
            InitializeComponent();
        }

        public void SetLimits(int min, int max) {
            blockEvents = true;

            trackBar1.Value = General.Clamp(trackBar1.Value, min, max);
            trackBar1.Minimum = min;
            trackBar1.Maximum = max;

            labelMin.Text = min.ToString(CultureInfo.InvariantCulture);
            labelMax.Text = max.ToString(CultureInfo.InvariantCulture);

            numericUpDown1.Value = General.Clamp((int)numericUpDown1.Value, min, max);
            numericUpDown1.Minimum = min;
            numericUpDown1.Maximum = max;

            blockEvents = false;
        }

        //events
        private void trackBar1_ValueChanged(object sender, EventArgs e) {
            if (!blockEvents) numericUpDown1.Value = ((TrackBar)sender).Value;
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e) {
            int value = (int)((NumericUpDown)sender).Value;
            delta = value - previousValue;
            previousValue = value;

            if (!blockEvents && OnValueChanged != null)
                OnValueChanged(this, EventArgs.Empty);

            blockEvents = true;
            trackBar1.Value = General.Clamp(value, trackBar1.Minimum, trackBar1.Maximum);
            blockEvents = false;
        }
    }
}
