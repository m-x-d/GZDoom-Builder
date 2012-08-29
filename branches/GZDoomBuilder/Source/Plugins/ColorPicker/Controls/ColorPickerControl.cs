using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Globalization;

namespace CodeImp.DoomBuilder.ColorPicker.Controls {
    public partial class ColorPickerControl : UserControl {
        
        private enum ChangeStyle {
            MouseMove,
            RGB,
            None
        }

        private const string COLOR_INFO_RGB = "RGB";
        private const string COLOR_INFO_HEX = "Hex";
        private const string COLOR_INFO_FLOAT = "Float";
        private readonly object[] COLOR_INFO = new object[] { COLOR_INFO_RGB, COLOR_INFO_HEX, COLOR_INFO_FLOAT };

        private static int colorInfoMode;
        
        private ChangeStyle changeType = ChangeStyle.None;
        private Point selectedPoint;

        private ColorWheel colorWheel;
        private ColorHandler.RGB RGB;

        public ColorHandler.RGB CurrentColor { get { return RGB; } }

        private bool isInUpdate;
        private Color startColor;

        //events
        public event EventHandler<ColorChangedEventArgs> ColorChanged;
        public event EventHandler OnOkPressed;
        public event EventHandler OnCancelPressed;

        public Button OkButton { get { return btnOK; } }
        public Button CancelButton { get { return btnCancel; } }

        public void Initialize(Color startColor){
            this.startColor = startColor;

            isInUpdate = true;
            InitializeComponent();
            isInUpdate = false;

            cbColorInfo.Items.AddRange(COLOR_INFO);
            cbColorInfo.SelectedIndex = colorInfoMode;
        }

        private void nudValueChanged(object sender, System.EventArgs e) {
            // If the R, G, or B values change, use this code to update the HSV values and invalidate
            // the color wheel (so it updates the pointers).
            // Check the isInUpdate flag to avoid recursive events when you update the NumericUpdownControls.
            if (!isInUpdate) {
                changeType = ChangeStyle.RGB;
                RGB = new ColorHandler.RGB((int)nudRed.Value, (int)nudGreen.Value, (int)nudBlue.Value);
                updateColorInfo(RGB);
                this.Invalidate();
            }
        }

        private void setRGB(ColorHandler.RGB RGB) {
            // Update the RGB values on the form, but don't trigger the ValueChanged event of the form. The isInUpdate
            // variable ensures that the event procedures exit without doing anything.
            isInUpdate = true;
            updateColorInfo(RGB);
            isInUpdate = false;
        }

        private void updateColorInfo(ColorHandler.RGB RGB) {
            this.RGB = RGB;
            btnOK.BackColor = ColorHandler.RGBtoColor(RGB);
            btnOK.ForeColor = (RGB.Red < 180 && RGB.Green < 180) ? Color.White : Color.Black;

			//update color info
            switch (cbColorInfo.SelectedItem.ToString()) {
                case COLOR_INFO_RGB:
                    refreshNudValue(nudRed, RGB.Red);
                    refreshNudValue(nudBlue, RGB.Blue);
                    refreshNudValue(nudGreen, RGB.Green);
                    break;

                case COLOR_INFO_HEX:
                    string r = RGB.Red.ToString("X");
                    if (r.Length == 1) r = "0" + r;
                    string g = RGB.Green.ToString("X");
                    if (g.Length == 1) g = "0" + g;
                    string b = RGB.Blue.ToString("X");
                    if (b.Length == 1) b = "0" + b;

                    isInUpdate = true;
                    tbFloatVals.Text = r + g + b;
                    isInUpdate = false;
                    break;

                case COLOR_INFO_FLOAT:
                    string r2 = ((float)Math.Round((float)RGB.Red / 255f, 2)).ToString();
                    if (r2.Length == 1) r2 += ".0";
                    string g2 = ((float)Math.Round((float)RGB.Green / 255f, 2)).ToString();
                    if (g2.Length == 1) g2 += ".0";
                    string b2 = ((float)Math.Round((float)RGB.Blue / 255f, 2)).ToString();
                    if (b2.Length == 1) b2 += ".0";

                    isInUpdate = true;
                    tbFloatVals.Text = r2 + " " + g2 + " " + b2;
                    isInUpdate = false;
                    break;
            }

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

            updateColorInfo(RGB);
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
            // Depending on the circumstances, force a repaint of the color wheel passing different information.
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

        private void cbColorInfo_SelectedIndexChanged(object sender, EventArgs e) {
            if (cbColorInfo.SelectedItem.ToString() == COLOR_INFO_RGB) {
                pRGB.Visible = true;
                tbFloatVals.Visible = false;
            } else {
                pRGB.Visible = false;
                tbFloatVals.Visible = true;
            }
            colorInfoMode = cbColorInfo.SelectedIndex;
            updateColorInfo(RGB);
        }

        private void tbFloatVals_TextChanged(object sender, EventArgs e) {
            if (isInUpdate) return;
            
            if (COLOR_INFO[colorInfoMode].ToString() == COLOR_INFO_FLOAT) {
                string[] parts = tbFloatVals.Text.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length != 3) return;

                ColorHandler.RGB rgb = new ColorHandler.RGB();

                float c;
                if (!float.TryParse(parts[0].Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out c)) return;
                rgb.Red = (int)(c * 255);

                if (!float.TryParse(parts[1].Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out c)) return;
                rgb.Green = (int)(c * 255);

                if (!float.TryParse(parts[2].Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out c)) return;
                rgb.Blue = (int)(c * 255);

                changeType = ChangeStyle.RGB;
                updateColorInfo(rgb);
                this.Invalidate();
            } else if (COLOR_INFO[colorInfoMode].ToString() == COLOR_INFO_HEX) {
                string hexColor = tbFloatVals.Text;
                if (hexColor.Length != 6) return;

                ColorHandler.RGB rgb = new ColorHandler.RGB();
                int color;

                string colorStr = hexColor.Substring(0, 2);
                if (!int.TryParse(colorStr, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out color)) return;
                rgb.Red = color;

                colorStr = hexColor.Substring(2, 2);
                if (!int.TryParse(colorStr, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out color)) return;
                rgb.Green = color;

                colorStr = hexColor.Substring(4, 2);
                if (!int.TryParse(colorStr, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out color)) return;
                rgb.Blue = color;

                changeType = ChangeStyle.RGB;
                updateColorInfo(rgb);
                this.Invalidate();
            }
        }
    }
}