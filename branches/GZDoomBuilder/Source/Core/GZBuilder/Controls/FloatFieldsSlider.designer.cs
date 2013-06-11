namespace CodeImp.DoomBuilder.GZBuilder.Controls
{
    partial class FloatFieldsSlider
    {
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
			this.trackBar1 = new Dotnetrix.Controls.TrackBar();
			this.nudValue = new CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox();
			((System.ComponentModel.ISupportInitialize)(this.trackBar1)).BeginInit();
			this.SuspendLayout();
			// 
			// trackBar1
			// 
			this.trackBar1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.trackBar1.LargeChange = 10;
			this.trackBar1.Location = new System.Drawing.Point(3, 3);
			this.trackBar1.Maximum = 512;
			this.trackBar1.Name = "trackBar1";
			this.trackBar1.Size = new System.Drawing.Size(150, 45);
			this.trackBar1.TabIndex = 4;
			this.trackBar1.ValueChanged += new System.EventHandler(this.trackBar1_ValueChanged);
			// 
			// nudValue
			// 
			this.nudValue.AllowDecimal = true;
			this.nudValue.AllowNegative = false;
			this.nudValue.AllowRelative = false;
			this.nudValue.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.nudValue.ButtonStep = 0.1F;
			this.nudValue.Location = new System.Drawing.Point(151, 0);
			this.nudValue.Name = "nudValue";
			this.nudValue.Size = new System.Drawing.Size(59, 24);
			this.nudValue.StepValues = null;
			this.nudValue.TabIndex = 8;
			this.nudValue.WhenTextChanged += new System.EventHandler(this.nudValue_WhenTextChanged);
			// 
			// FloatFieldsSlider
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.nudValue);
			this.Controls.Add(this.trackBar1);
			this.Name = "FloatFieldsSlider";
			this.Size = new System.Drawing.Size(213, 33);
			((System.ComponentModel.ISupportInitialize)(this.trackBar1)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

		protected Dotnetrix.Controls.TrackBar trackBar1;
		private CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox nudValue;
    }
}
