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
        private bool linkSliders;
        public event EventHandler OnValueChanged;

        public Vector2D Value {
            set {
                floatSlider1.Value = value.x;
                floatSlider2.Value = value.y;
            }
            get {
                return new Vector2D(floatSlider1.Value, floatSlider2.Value);
            }
        }

        public Vector2D Delta { get { return new Vector2D(floatSlider1.Delta, floatSlider2.Delta); } }
        
        public ScaleControl() {
            InitializeComponent();

            setLinkButtonIcon(linkSliders);

            floatSlider1.OnValueChanged += new EventHandler(floatSlider1_OnValueChanged);
            floatSlider2.OnValueChanged += new EventHandler(floatSlider2_OnValueChanged);
            button1.Click += new EventHandler(button1_Click);
        }

        private void setLinkButtonIcon(bool link){
            button1.BackgroundImage = link ? Properties.Resources.Chain : Properties.Resources.Chain2;
        }

        public void SetLimits(float min, float max) {
            floatSlider1.SetLimits(min, max, true);
            floatSlider2.SetLimits(min, max, true);
        }

//events
        private void floatSlider1_OnValueChanged(object sender, EventArgs e) {
            if (linkSliders) floatSlider2.Value = ((FloatSlider)sender).Value;
            if (OnValueChanged != null) OnValueChanged(this, EventArgs.Empty);
        }

        private void floatSlider2_OnValueChanged(object sender, EventArgs e) {
            if (linkSliders) floatSlider1.Value = ((FloatSlider)sender).Value;
            if (OnValueChanged != null) OnValueChanged(this, EventArgs.Empty);
        }

        private void button1_Click(object sender, EventArgs e) {
            setLinkButtonIcon(linkSliders = !linkSliders);
        }
    }
}
