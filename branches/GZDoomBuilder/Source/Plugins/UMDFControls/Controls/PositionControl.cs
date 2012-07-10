using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using CodeImp.DoomBuilder.Geometry;
using System.Globalization;

namespace CodeImp.DoomBuilder.UDMFControls
{
    public partial class PositionControl : UserControl
    {
        private static int stepSize;
        private bool blockEvents;
        public event EventHandler OnValueChanged;

        public Vector2D Value {
            get {
                return new Vector2D((float)nudX.Value, (float)nudY.Value);
            }
            set {
                prevX = (float)nudX.Value;
                prevY = (float)nudY.Value;
                
                blockEvents = true;
                nudX.Value = (decimal)value.x;
                nudY.Value = (decimal)value.y;
                blockEvents = false;

                delta.x = (float)nudX.Value - prevX;
                delta.y = (float)nudY.Value - prevY;
            }
        }

        private float prevX, prevY;
        private Vector2D delta;
        public Vector2D Delta { get { return delta; } }

//constructor
        public PositionControl() {
            delta = new Vector2D();
            InitializeComponent();
            trackBar1.Value = stepSize;
            labelStepSize.Text = stepSize == 0 ? "1" : stepSize.ToString(CultureInfo.InvariantCulture);
        }
//events
        private void nudX_ValueChanged(object sender, EventArgs e) {
            delta.x = (float)nudX.Value - prevX;
            prevX = (float)nudX.Value;
            
            if (!blockEvents && OnValueChanged != null)
                OnValueChanged(this, EventArgs.Empty);
        }

        private void nudY_ValueChanged(object sender, EventArgs e) {
            delta.y = (float)nudY.Value - prevY;
            prevY = (float)nudY.Value;

            if (!blockEvents && OnValueChanged != null)
                OnValueChanged(this, EventArgs.Empty);
        }

        private void trackBar1_Scroll(object sender, EventArgs e) {
            stepSize = trackBar1.Value;
            nudX.Increment = stepSize == 0 ? 1 : stepSize;
            nudY.Increment = nudX.Increment;
            labelStepSize.Text = nudX.Increment.ToString();
        }
    }
}