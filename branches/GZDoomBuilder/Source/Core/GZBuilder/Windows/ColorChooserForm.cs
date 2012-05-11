using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;

using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Windows;

namespace CodeImp.DoomBuilder.GZBuilder.Windows {
    public class ColorChooserForm : DelayedForm {
        internal System.Windows.Forms.Button btnCancel;
        internal System.Windows.Forms.Button btnOK;
        internal System.Windows.Forms.Label Label3;
        internal System.Windows.Forms.NumericUpDown nudRed;
        internal System.Windows.Forms.Panel pnlColor;
        internal System.Windows.Forms.Label Label1;
        internal System.Windows.Forms.Panel pnlBrightness;
        internal System.Windows.Forms.NumericUpDown nudBlue;
        internal System.Windows.Forms.NumericUpDown nudGreen;
        internal System.Windows.Forms.Label Label2;
        private Dotnetrix.Controls.TrackBar trackBar1;
        internal Label labelName1;
        internal Label labelName2;
        private Dotnetrix.Controls.TrackBar trackBar2;
        internal NumericUpDown numericUpDown1;
        internal NumericUpDown numericUpDown2;
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;

        public ColorChooserForm() {
            InitializeComponent();


        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing) {
            if (disposing) {
                if (components != null) {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.Label3 = new System.Windows.Forms.Label();
            this.nudRed = new System.Windows.Forms.NumericUpDown();
            this.pnlColor = new System.Windows.Forms.Panel();
            this.Label1 = new System.Windows.Forms.Label();
            this.pnlBrightness = new System.Windows.Forms.Panel();
            this.nudBlue = new System.Windows.Forms.NumericUpDown();
            this.nudGreen = new System.Windows.Forms.NumericUpDown();
            this.Label2 = new System.Windows.Forms.Label();
            this.trackBar1 = new Dotnetrix.Controls.TrackBar();
            this.labelName1 = new System.Windows.Forms.Label();
            this.labelName2 = new System.Windows.Forms.Label();
            this.trackBar2 = new Dotnetrix.Controls.TrackBar();
            this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            this.numericUpDown2 = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.nudRed)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudBlue)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudGreen)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown2)).BeginInit();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnCancel.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancel.Location = new System.Drawing.Point(219, 49);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(94, 42);
            this.btnCancel.TabIndex = 11;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOK
            // 
            this.btnOK.BackColor = System.Drawing.SystemColors.Control;
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnOK.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnOK.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnOK.Location = new System.Drawing.Point(219, 8);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(94, 42);
            this.btnOK.TabIndex = 10;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // Label3
            // 
            this.Label3.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label3.Location = new System.Drawing.Point(219, 152);
            this.Label3.Name = "Label3";
            this.Label3.Size = new System.Drawing.Size(40, 23);
            this.Label3.TabIndex = 45;
            this.Label3.Text = "Blue:";
            this.Label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // nudRed
            // 
            this.nudRed.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.nudRed.Location = new System.Drawing.Point(265, 106);
            this.nudRed.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nudRed.Name = "nudRed";
            this.nudRed.Size = new System.Drawing.Size(48, 20);
            this.nudRed.TabIndex = 38;
            this.nudRed.ValueChanged += new System.EventHandler(this.HandleRGBChange);
            // 
            // pnlColor
            // 
            this.pnlColor.Location = new System.Drawing.Point(8, 8);
            this.pnlColor.Name = "pnlColor";
            this.pnlColor.Size = new System.Drawing.Size(176, 176);
            this.pnlColor.TabIndex = 51;
            this.pnlColor.Visible = false;
            this.pnlColor.MouseUp += new System.Windows.Forms.MouseEventHandler(this.frmMain_MouseUp);
            // 
            // Label1
            // 
            this.Label1.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label1.Location = new System.Drawing.Point(219, 104);
            this.Label1.Name = "Label1";
            this.Label1.Size = new System.Drawing.Size(40, 23);
            this.Label1.TabIndex = 43;
            this.Label1.Text = "Red:";
            this.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // pnlBrightness
            // 
            this.pnlBrightness.Location = new System.Drawing.Point(190, 8);
            this.pnlBrightness.Name = "pnlBrightness";
            this.pnlBrightness.Size = new System.Drawing.Size(16, 176);
            this.pnlBrightness.TabIndex = 52;
            this.pnlBrightness.Visible = false;
            // 
            // nudBlue
            // 
            this.nudBlue.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.nudBlue.Location = new System.Drawing.Point(265, 154);
            this.nudBlue.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nudBlue.Name = "nudBlue";
            this.nudBlue.Size = new System.Drawing.Size(48, 20);
            this.nudBlue.TabIndex = 40;
            this.nudBlue.ValueChanged += new System.EventHandler(this.HandleRGBChange);
            // 
            // nudGreen
            // 
            this.nudGreen.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.nudGreen.Location = new System.Drawing.Point(265, 130);
            this.nudGreen.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nudGreen.Name = "nudGreen";
            this.nudGreen.Size = new System.Drawing.Size(48, 20);
            this.nudGreen.TabIndex = 39;
            this.nudGreen.ValueChanged += new System.EventHandler(this.HandleRGBChange);
            // 
            // Label2
            // 
            this.Label2.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label2.Location = new System.Drawing.Point(219, 128);
            this.Label2.Name = "Label2";
            this.Label2.Size = new System.Drawing.Size(40, 23);
            this.Label2.TabIndex = 44;
            this.Label2.Text = "Green:";
            this.Label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // trackBar1
            // 
            this.trackBar1.LargeChange = 64;
            this.trackBar1.Location = new System.Drawing.Point(91, 190);
            this.trackBar1.Maximum = 512;
            this.trackBar1.Name = "trackBar1";
            this.trackBar1.Size = new System.Drawing.Size(166, 45);
            this.trackBar1.SmallChange = 8;
            this.trackBar1.TabIndex = 56;
            this.trackBar1.TickFrequency = 16;
            this.trackBar1.TickStyle = System.Windows.Forms.TickStyle.Both;
            this.trackBar1.ValueChanged += new System.EventHandler(this.trackBar1_ValueChanged);
            // 
            // labelName1
            // 
            this.labelName1.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelName1.Location = new System.Drawing.Point(5, 199);
            this.labelName1.Name = "labelName1";
            this.labelName1.Size = new System.Drawing.Size(80, 23);
            this.labelName1.TabIndex = 57;
            this.labelName1.Text = "Start intensity:";
            this.labelName1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // labelName2
            // 
            this.labelName2.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelName2.Location = new System.Drawing.Point(5, 250);
            this.labelName2.Name = "labelName2";
            this.labelName2.Size = new System.Drawing.Size(80, 23);
            this.labelName2.TabIndex = 60;
            this.labelName2.Text = "Start intensity:";
            this.labelName2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // trackBar2
            // 
            this.trackBar2.LargeChange = 64;
            this.trackBar2.Location = new System.Drawing.Point(91, 241);
            this.trackBar2.Maximum = 512;
            this.trackBar2.Name = "trackBar2";
            this.trackBar2.Size = new System.Drawing.Size(166, 45);
            this.trackBar2.SmallChange = 8;
            this.trackBar2.TabIndex = 59;
            this.trackBar2.TickFrequency = 16;
            this.trackBar2.TickStyle = System.Windows.Forms.TickStyle.Both;
            this.trackBar2.ValueChanged += new System.EventHandler(this.trackBar2_ValueChanged);
            // 
            // numericUpDown1
            // 
            this.numericUpDown1.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.numericUpDown1.Location = new System.Drawing.Point(257, 200);
            this.numericUpDown1.Maximum = new decimal(new int[] {
            9000,
            0,
            0,
            0});
            this.numericUpDown1.Name = "numericUpDown1";
            this.numericUpDown1.Size = new System.Drawing.Size(56, 20);
            this.numericUpDown1.TabIndex = 57;
            this.numericUpDown1.ValueChanged += new System.EventHandler(this.numericUpDown1_ValueChanged);
            // 
            // numericUpDown2
            // 
            this.numericUpDown2.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.numericUpDown2.Location = new System.Drawing.Point(257, 250);
            this.numericUpDown2.Maximum = new decimal(new int[] {
            9000,
            0,
            0,
            0});
            this.numericUpDown2.Name = "numericUpDown2";
            this.numericUpDown2.Size = new System.Drawing.Size(56, 20);
            this.numericUpDown2.TabIndex = 62;
            this.numericUpDown2.ValueChanged += new System.EventHandler(this.numericUpDown2_ValueChanged);
            // 
            // ColorChooserForm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(319, 286);
            this.Controls.Add(this.numericUpDown2);
            this.Controls.Add(this.numericUpDown1);
            this.Controls.Add(this.labelName2);
            this.Controls.Add(this.trackBar2);
            this.Controls.Add(this.labelName1);
            this.Controls.Add(this.trackBar1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.Label3);
            this.Controls.Add(this.nudRed);
            this.Controls.Add(this.pnlColor);
            this.Controls.Add(this.Label1);
            this.Controls.Add(this.pnlBrightness);
            this.Controls.Add(this.nudBlue);
            this.Controls.Add(this.nudGreen);
            this.Controls.Add(this.Label2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ColorChooserForm";
            this.Opacity = 0;
            this.Text = "Select Color";
            this.Load += new System.EventHandler(this.ColorChooser1_Load);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.frmMain_MouseUp);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.ColorChooser1_Paint);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.HandleMouse);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.HandleMouse);
            ((System.ComponentModel.ISupportInitialize)(this.nudRed)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudBlue)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudGreen)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion

        private enum ChangeStyle {
            MouseMove,
            RGB,
            None
        }

        private ChangeStyle changeType = ChangeStyle.None;
        private Point selectedPoint;

        private ColorWheel colorWheel;
        private ColorHandler.RGB RGB;
        private bool isInUpdate = false;


        private List<Thing> selectedLights;
        private List<Sector> selectedLights;

        //private Color startColor;

        /*public void SetData(Color startColor, string[] labels) {
            btnCancel.BackColor = startColor;
            btnOK.BackColor = startColor;
            if (startColor.R < 128 && startColor.G < 128 && startColor.B < 128) {
                btnCancel.ForeColor = Color.White;
                btnOK.ForeColor = Color.White;
            }
            this.startColor = startColor;

            if (labels == null || labels.Length == 0) {
                numericUpDown1.Visible = false;
                numericUpDown2.Visible = false;
                labelName1.Visible = false;
                labelName2.Visible = false;
                trackBar1.Visible = false;
                trackBar2.Visible = false;
                this.ClientSize = new Size(this.ClientSize.Width, trackBar1.Location.Y);
            } else if (labels.Length == 1) {
                labelName1.Text = labels[0] + ":";

                numericUpDown2.Visible = false;
                labelName2.Visible = false;
                trackBar2.Visible = false;
                this.ClientSize = new Size(this.ClientSize.Width, trackBar2.Location.Y);
            } else {
                labelName1.Text = labels[0] + ":";
                labelName2.Text = labels[1] + ":";
            }
        }*/

        private void ColorChooser1_Load(object sender, System.EventArgs e) {
            // Turn on double-buffering, so the form looks better. 
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.DoubleBuffer, true);

            Rectangle BrightnessRectangle = new Rectangle(pnlBrightness.Location, pnlBrightness.Size);
            Rectangle ColorRectangle = new Rectangle(pnlColor.Location, pnlColor.Size);

            // Create the new ColorWheel class, indicating the locations of the color wheel itself, the
            // brightness area, and the position of the selected color.
            colorWheel = new ColorWheel(ColorRectangle, BrightnessRectangle);
            colorWheel.ColorChanged += new ColorWheel.ColorChangedEventHandler(this.myColorWheel_ColorChanged);

            //set initial color



            isInUpdate = true;
            changeType = ChangeStyle.RGB;
            RGB = new ColorHandler.RGB(startColor.R, startColor.G, startColor.B);

            RefreshValue(nudRed, RGB.Red);
            RefreshValue(nudBlue, RGB.Blue);
            RefreshValue(nudGreen, RGB.Green);
            isInUpdate = false;
            this.Invalidate();
        }

        private void HandleMouse(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Left) {
                changeType = ChangeStyle.MouseMove;
                selectedPoint = new Point(e.X, e.Y);
                this.Invalidate();
            }
        }

        private void frmMain_MouseUp(object sender, MouseEventArgs e) {
            colorWheel.SetMouseUp();
            changeType = ChangeStyle.None;
        }

        private void HandleRGBChange(object sender, System.EventArgs e) {

            // If the R, G, or B values change, use this code to update the HSV values and invalidate
            // the color wheel (so it updates the pointers).
            // Check the isInUpdate flag to avoid recursive events when you update the NumericUpdownControls.
            if (!isInUpdate) {
                changeType = ChangeStyle.RGB;
                RGB = new ColorHandler.RGB((int)nudRed.Value, (int)nudGreen.Value, (int)nudBlue.Value);
                this.Invalidate();
            }
        }

        private void SetRGB(ColorHandler.RGB RGB) {
            // Update the RGB values on the form, but don't trigger the ValueChanged event of the form. The isInUpdate
            // variable ensures that the event procedures exit without doing anything.
            isInUpdate = true;
            RefreshValue(nudRed, RGB.Red);
            RefreshValue(nudBlue, RGB.Blue);
            RefreshValue(nudGreen, RGB.Green);

            btnOK.BackColor = ColorHandler.RGBtoColor(RGB);
            if (RGB.Red < 128 && RGB.Green < 128 && RGB.Blue < 128)
                btnOK.ForeColor = Color.White;
            else
                btnOK.ForeColor = Color.Black;
            isInUpdate = false;
        }

        private void RefreshValue(NumericUpDown nud, int value) {
            // Update the value of the NumericUpDown control, if the value is different than the current value.
            // Refresh the control, causing an immediate repaint.
            if (nud.Value != value) {
                nud.Value = value;
                nud.Refresh();
            }
        }

//Properties
        public Color Color { get { return colorWheel.Color; } }
        public int Value1 { get { return (int)numericUpDown1.Value; } }
        public int Value2 { get { return (int)numericUpDown2.Value; } }

//Events
        private void myColorWheel_ColorChanged(object sender, ColorChangedEventArgs e) {
            SetRGB(e.RGB);
        }

        private void ColorChooser1_Paint(object sender, System.Windows.Forms.PaintEventArgs e) {
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

        private void trackBar1_ValueChanged(object sender, EventArgs e) {
            numericUpDown1.Value = trackBar1.Value;
        }

        private void trackBar2_ValueChanged(object sender, EventArgs e) {
            numericUpDown2.Value = trackBar2.Value;
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e) {
            trackBar1.Value = (int)numericUpDown1.Value > trackBar1.Maximum ? trackBar1.Maximum : (int)numericUpDown1.Value;
        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e) {
            trackBar2.Value = (int)numericUpDown2.Value > trackBar2.Maximum ? trackBar2.Maximum : (int)numericUpDown2.Value;
        }

//buttons
        private void btnOK_Click(object sender, EventArgs e) {
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e) {
            this.Close();
        }
    }
}
