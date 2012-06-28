namespace CodeImp.DoomBuilder.UDMFControls
{
    partial class AngleControl {
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
            this.components = new System.ComponentModel.Container();
            this.panelAngleControl = new System.Windows.Forms.Panel();
            this.nudAngle = new System.Windows.Forms.NumericUpDown();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.nudAngle)).BeginInit();
            this.SuspendLayout();
            // 
            // panelAngleControl
            // 
            this.panelAngleControl.BackgroundImage = global::CodeImp.DoomBuilder.UDMFControls.Properties.Resources.dial;
            this.panelAngleControl.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.panelAngleControl.Location = new System.Drawing.Point(3, 3);
            this.panelAngleControl.Margin = new System.Windows.Forms.Padding(0);
            this.panelAngleControl.Name = "panelAngleControl";
            this.panelAngleControl.Size = new System.Drawing.Size(96, 96);
            this.panelAngleControl.TabIndex = 0;
            this.toolTip1.SetToolTip(this.panelAngleControl, "Click to set angle\r\nShift-click to set angle snapped to 45-degrees increment");
            this.panelAngleControl.Paint += new System.Windows.Forms.PaintEventHandler(this.panelAngleControl_Paint);
            // 
            // nudAngle
            // 
            this.nudAngle.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.nudAngle.Location = new System.Drawing.Point(45, 102);
            this.nudAngle.Maximum = new decimal(new int[] {
            9000,
            0,
            0,
            0});
            this.nudAngle.Minimum = new decimal(new int[] {
            9000,
            0,
            0,
            -2147483648});
            this.nudAngle.Name = "nudAngle";
            this.nudAngle.Size = new System.Drawing.Size(54, 20);
            this.nudAngle.TabIndex = 1;
            this.nudAngle.ValueChanged += new System.EventHandler(this.nudAngle_ValueChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.Location = new System.Drawing.Point(4, 104);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(38, 14);
            this.label1.TabIndex = 2;
            this.label1.Text = "Angle:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // AngleControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.label1);
            this.Controls.Add(this.nudAngle);
            this.Controls.Add(this.panelAngleControl);
            this.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Name = "AngleControl";
            this.Size = new System.Drawing.Size(104, 127);
            ((System.ComponentModel.ISupportInitialize)(this.nudAngle)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panelAngleControl;
        private System.Windows.Forms.NumericUpDown nudAngle;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Label label1;
    }
}
