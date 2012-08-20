﻿namespace CodeImp.DoomBuilder.ColorPicker.Controls {
    partial class ColorPickerControl {
        /// <summary> 
        /// Требуется переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором компонентов

        /// <summary> 
        /// Обязательный метод для поддержки конструктора - не изменяйте 
        /// содержимое данного метода при помощи редактора кода.
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
			this.tbFloatVals = new System.Windows.Forms.TextBox();
			((System.ComponentModel.ISupportInitialize)(this.nudRed)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.nudBlue)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.nudGreen)).BeginInit();
			this.SuspendLayout();
			// 
			// btnCancel
			// 
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.btnCancel.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnCancel.Location = new System.Drawing.Point(214, 44);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(94, 42);
			this.btnCancel.TabIndex = 54;
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
			this.btnOK.Location = new System.Drawing.Point(214, 3);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(94, 42);
			this.btnOK.TabIndex = 53;
			this.btnOK.Text = "OK";
			this.btnOK.UseVisualStyleBackColor = true;
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			// 
			// Label3
			// 
			this.Label3.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Label3.Location = new System.Drawing.Point(214, 133);
			this.Label3.Name = "Label3";
			this.Label3.Size = new System.Drawing.Size(40, 23);
			this.Label3.TabIndex = 60;
			this.Label3.Text = "Blue:";
			this.Label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// nudRed
			// 
			this.nudRed.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.nudRed.Location = new System.Drawing.Point(260, 89);
			this.nudRed.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
			this.nudRed.Name = "nudRed";
			this.nudRed.Size = new System.Drawing.Size(48, 20);
			this.nudRed.TabIndex = 55;
			this.nudRed.ValueChanged += new System.EventHandler(this.nudValueChanged);
			// 
			// pnlColor
			// 
			this.pnlColor.Location = new System.Drawing.Point(3, 3);
			this.pnlColor.Name = "pnlColor";
			this.pnlColor.Size = new System.Drawing.Size(176, 176);
			this.pnlColor.TabIndex = 61;
			this.pnlColor.Visible = false;
			// 
			// Label1
			// 
			this.Label1.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Label1.Location = new System.Drawing.Point(214, 87);
			this.Label1.Name = "Label1";
			this.Label1.Size = new System.Drawing.Size(40, 23);
			this.Label1.TabIndex = 58;
			this.Label1.Text = "Red:";
			this.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// pnlBrightness
			// 
			this.pnlBrightness.Location = new System.Drawing.Point(185, 3);
			this.pnlBrightness.Name = "pnlBrightness";
			this.pnlBrightness.Size = new System.Drawing.Size(16, 176);
			this.pnlBrightness.TabIndex = 62;
			this.pnlBrightness.Visible = false;
			// 
			// nudBlue
			// 
			this.nudBlue.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.nudBlue.Location = new System.Drawing.Point(260, 135);
			this.nudBlue.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
			this.nudBlue.Name = "nudBlue";
			this.nudBlue.Size = new System.Drawing.Size(48, 20);
			this.nudBlue.TabIndex = 57;
			this.nudBlue.ValueChanged += new System.EventHandler(this.nudValueChanged);
			// 
			// nudGreen
			// 
			this.nudGreen.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.nudGreen.Location = new System.Drawing.Point(260, 112);
			this.nudGreen.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
			this.nudGreen.Name = "nudGreen";
			this.nudGreen.Size = new System.Drawing.Size(48, 20);
			this.nudGreen.TabIndex = 56;
			this.nudGreen.ValueChanged += new System.EventHandler(this.nudValueChanged);
			// 
			// Label2
			// 
			this.Label2.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Label2.Location = new System.Drawing.Point(214, 110);
			this.Label2.Name = "Label2";
			this.Label2.Size = new System.Drawing.Size(40, 23);
			this.Label2.TabIndex = 59;
			this.Label2.Text = "Green:";
			this.Label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// tbFloatVals
			// 
			this.tbFloatVals.Location = new System.Drawing.Point(217, 158);
			this.tbFloatVals.Name = "tbFloatVals";
			this.tbFloatVals.ReadOnly = true;
			this.tbFloatVals.Size = new System.Drawing.Size(90, 20);
			this.tbFloatVals.TabIndex = 63;
			this.tbFloatVals.Text = "1.01 0.55 0.33";
			this.tbFloatVals.Click += new System.EventHandler(this.tbFloatVals_Click);
			// 
			// ColorPickerControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.tbFloatVals);
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
			this.Name = "ColorPickerControl";
			this.Size = new System.Drawing.Size(311, 183);
			this.Load += new System.EventHandler(this.ColorPickerControl_Load);
			this.MouseLeave += new System.EventHandler(this.onMouseUp);
			this.Paint += new System.Windows.Forms.PaintEventHandler(this.onPaint);
			this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.handleMouse);
			this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ColorPickerControl_MouseDown);
			this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.onMouseUp);
			((System.ComponentModel.ISupportInitialize)(this.nudRed)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.nudBlue)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.nudGreen)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

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
		private System.Windows.Forms.TextBox tbFloatVals;
    }
}
