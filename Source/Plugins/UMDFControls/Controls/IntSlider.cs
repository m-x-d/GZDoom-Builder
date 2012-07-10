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
    public partial class IntSlider : UserControl {

        private bool blockEvents;
        public event EventHandler OnValueChanged;

        private int previousValue;
        public int Value { 
            get
            { 
                return (int)numericUpDown1.Value; 
            }
            set
            {
                blockEvents = true;

                previousValue = General.Clamp(value, (int)numericUpDown1.Minimum, (int)numericUpDown1.Maximum);
                numericUpDown1.Value = previousValue;
                blockEvents = false;
            }
        }

        public int Delta { get { return trackBar1.Value - previousValue; }}

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
            //bool blockEventsStatus = blockEvents;
            blockEvents = true;

            trackBar1.Value = General.Clamp(trackBar1.Value, min, max);
            trackBar1.Minimum = min;
            trackBar1.Maximum = max;

            labelMin.Text = min.ToString(CultureInfo.InvariantCulture);
            labelMax.Text = max.ToString(CultureInfo.InvariantCulture);

            numericUpDown1.Value = General.Clamp((int)numericUpDown1.Value, min, max);
            numericUpDown1.Minimum = min;
            numericUpDown1.Maximum = max;

            //blockEvents = blockEventsStatus;
            blockEvents = false;
        }

//events
        private void trackBar1_ValueChanged(object sender, EventArgs e) {
            numericUpDown1.Value = ((TrackBar)sender).Value;
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e) {
            int val = (int)((NumericUpDown)sender).Value;

            if (!blockEvents && OnValueChanged != null)
                OnValueChanged(this, EventArgs.Empty);

            previousValue = trackBar1.Value;
            blockEvents = true;
            trackBar1.Value = General.Clamp(val, trackBar1.Minimum, trackBar1.Maximum); //clamp it!
            blockEvents = false;
        }
    }
}
