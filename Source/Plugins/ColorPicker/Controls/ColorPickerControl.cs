using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace CodeImp.DoomBuilder.ColorPicker.Controls {
    public partial class ColorPickerControl : UserControl {
        
        private enum ChangeStyle {
            MouseMove,
            RGB,
            None
        }
        
        private ChangeStyle changeType = ChangeStyle.None;
        private Point selectedPoint;

        private ColorWheel colorWheel;
        private ColorHandler.RGB RGB;

        public ColorHandler.RGB CurrentColor { 
            get 
            {
                //GZBuilder.GZGeneral.Trace("get CurrentColor: " + RGB.Red + "," + RGB.Green + "," + RGB.Blue);
                return RGB; 
            } 
        }

        private bool isInUpdate = false;
        private Color startColor;

        //events
        public event EventHandler<ColorChangedEventArgs> ColorChanged;
        public event EventHandler OnOkPressed;
        public event EventHandler OnCancelPressed;

        public Button OkButton { get { return btnOK; } }
        public Button CancelButton { get { return btnCancel; } }

        public void Initialize(Color startColor){
            this.startColor = startColor;
            InitializeComponent();
        }

        private void nudValueChanged(object sender, System.EventArgs e) {
            // If the R, G, or B values change, use this code to update the HSV values and invalidate
            // the color wheel (so it updates the pointers).
            // Check the isInUpdate flag to avoid recursive events when you update the NumericUpdownControls.
            if (!isInUpdate) {
                changeType = ChangeStyle.RGB;
                RGB = new ColorHandler.RGB((int)nudRed.Value, (int)nudGreen.Value, (int)nudBlue.Value);
                updateOKButton(RGB);
                this.Invalidate();
            }
        }

        private void setRGB(ColorHandler.RGB RGB) {
            // Update the RGB values on the form, but don't trigger the ValueChanged event of the form. The isInUpdate
            // variable ensures that the event procedures exit without doing anything.
            isInUpdate = true;
            refreshNudValue(nudRed, RGB.Red);
            refreshNudValue(nudBlue, RGB.Blue);
            refreshNudValue(nudGreen, RGB.Green);
            updateOKButton(RGB);
            isInUpdate = false;
        }

        private void updateOKButton(ColorHandler.RGB RGB) {
            this.RGB = RGB;
            btnOK.BackColor = ColorHandler.RGBtoColor(RGB);
            btnOK.ForeColor = (RGB.Red < 180 && RGB.Green < 180) ? Color.White : Color.Black;

            //dispatch event further
            EventHandler<ColorChangedEventArgs> handler = ColorChanged;
            if (handler != null)
                handler(this, new ColorChangedEventArgs(RGB, ColorHandler.RGBtoHSV(RGB)));
        }

        private void updateCancelButton(ColorHandler.RGB RGB) {
            btnCancel.BackColor = ColorHandler.RGBtoColor(RGB);
            btnCancel.ForeColor = (RGB.Red < 180 && RGB.Green < 180) ? Color.White : Color.Black;
        }

        private void refreshNudValue(NumericUpDown nud, int value) {
            // Update the value of the NumericUpDown control, if the value is different than the current value.
            // Refresh the control, causing an immediate repaint.
            if (nud.Value != value) {
                nud.Value = value;
                nud.Refresh();
            }
        }

        public void SetCurrentColor(Color c) {
            isInUpdate = true;
            changeType = ChangeStyle.RGB;
            RGB = new ColorHandler.RGB(c.R, c.G, c.B);

            refreshNudValue(nudRed, RGB.Red);
            refreshNudValue(nudBlue, RGB.Blue);
            refreshNudValue(nudGreen, RGB.Green);
            updateOKButton(RGB);
            isInUpdate = false;
            this.Invalidate();
        }

        public void SetInitialColor(Color c) {
            updateCancelButton(new ColorHandler.RGB(c.R, c.G, c.B));
        }

        //events
        private void ColorPickerControl_Load(object sender, EventArgs e) {
            // Turn on double-buffering, so the form looks better. 
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.DoubleBuffer, true);

            Rectangle BrightnessRectangle = new Rectangle(pnlBrightness.Location, pnlBrightness.Size);
            Rectangle ColorRectangle = new Rectangle(pnlColor.Location, pnlColor.Size);

            // Create the new ColorWheel class, indicating the locations of the color wheel itself, the
            // brightness area, and the position of the selected color.
            colorWheel = new ColorWheel(ColorRectangle, BrightnessRectangle);
            colorWheel.ColorChanged += new ColorWheel.ColorChangedEventHandler(this.colorChanged);

            //set initial colors
            SetCurrentColor(startColor);
            updateCancelButton(RGB);
        }

        private void ColorPickerControl_MouseDown(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Left) {
                changeType = ChangeStyle.MouseMove;
                selectedPoint = new Point(e.X, e.Y);
                this.Invalidate();
            }
        }

        private void colorChanged(object sender, ColorChangedEventArgs e) {
            setRGB(e.RGB);
        }

        private void onPaint(object sender, System.Windows.Forms.PaintEventArgs e) {
            // Depending on the circumstances, force a repaint
            // of the color wheel passing different information.
            switch (changeType) {
                case ChangeStyle.MouseMove:
                case ChangeStyle.None:
                    colorWheel.Draw(e.Graphics, selectedPoint);
                    break;
                case ChangeStyle.RGB:
                    colorWheel.Draw(e.Graphics, RGB);
                    break;
            }
        }

        private void handleMouse(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Left) {
                changeType = ChangeStyle.MouseMove;
                selectedPoint = new Point(e.X, e.Y);
                this.Invalidate();
            }
        }

        private void onMouseUp(object sender, MouseEventArgs e) {
            colorWheel.SetMouseUp();
            changeType = ChangeStyle.None;
        }

        private void onMouseUp(object sender, EventArgs e) {
            colorWheel.SetMouseUp();
            changeType = ChangeStyle.None;
        }

        private void btnOK_Click(object sender, EventArgs e) {
            //dispatch event further
            EventHandler handler = OnOkPressed;
            if (handler != null)
                handler(this, e);
        }

        private void btnCancel_Click(object sender, EventArgs e) {
            //dispatch event further
            EventHandler handler = OnCancelPressed;
            if (handler != null)
                handler(this, e);
        }
    }
}
