using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

using CodeImp.DoomBuilder.Geometry;

namespace CodeImp.DoomBuilder.UDMFControls
{
    public partial class ScaleControl : UserControl
    {
        private static bool linkSliders;
        public event EventHandler OnValueChanged;

        public Vector2D Value {
            set {
                floatSlider1.Value = value.x;
                floatSlider2.Value = value.y;
                prevValues = value;
                delta = new Vector2D();
            }
            get {
                return new Vector2D(floatSlider1.Value, floatSlider2.Value);
            }
        }

        private Vector2D prevValues;
        private Vector2D delta;
        public Vector2D Delta { get { return delta; } }

        public ScaleControl() {
            prevValues = new Vector2D();
            delta = new Vector2D();

            InitializeComponent();

            setLinkButtonIcon(linkSliders);

            floatSlider1.OnValueChanged += new EventHandler(floatSlider1_OnValueChanged);
            floatSlider2.OnValueChanged += new EventHandler(floatSlider2_OnValueChanged);
            button1.Click += new EventHandler(button1_Click);
        }

        private void setLinkButtonIcon(bool link) {
            button1.BackgroundImage = link ? Properties.Resources.Chain : Properties.Resources.Chain2;
        }

        public void SetLimits(float min, float max) {
            floatSlider1.SetLimits(min, max, true);
            floatSlider2.SetLimits(min, max, true);
        }

        //events
        private void floatSlider1_OnValueChanged(object sender, EventArgs e) {
            float val = ((FloatSlider)sender).Value;

            if (linkSliders) {
                delta.y = (float)Math.Round(val - floatSlider2.Value, 1);
                prevValues.y = floatSlider2.Value;
                floatSlider2.Value = val;
            }

            if (OnValueChanged != null) OnValueChanged(this, EventArgs.Empty);
            delta.x = (float)Math.Round(val - prevValues.x, 1);
            prevValues.x = val;
        }

        private void floatSlider2_OnValueChanged(object sender, EventArgs e) {
            float val = ((FloatSlider)sender).Value;

            if (linkSliders) {
                delta.x = (float)Math.Round(val - floatSlider1.Value, 1);
                prevValues.x = floatSlider1.Value;
                floatSlider1.Value = val;
            }

            if (OnValueChanged != null) OnValueChanged(this, EventArgs.Empty);
            delta.y = (float)Math.Round(val - prevValues.y, 1);
            prevValues.y = val;
        }

        private void button1_Click(object sender, EventArgs e) {
            setLinkButtonIcon(linkSliders = !linkSliders);
        }
    }
}
