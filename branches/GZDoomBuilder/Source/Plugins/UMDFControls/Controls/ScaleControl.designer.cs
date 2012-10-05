namespace CodeImp.DoomBuilder.UDMFControls
{
    partial class ScaleControl
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
            this.button1 = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.floatSlider1 = new CodeImp.DoomBuilder.UDMFControls.FloatSlider();
            this.floatSlider2 = new CodeImp.DoomBuilder.UDMFControls.FloatSlider();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.button1.Location = new System.Drawing.Point(11, 38);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(18, 24);
            this.button1.TabIndex = 2;
            this.button1.UseVisualStyleBackColor = true;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::CodeImp.DoomBuilder.UDMFControls.Properties.Resources.ScaleLink;
            this.pictureBox1.Location = new System.Drawing.Point(20, 26);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(10, 47);
            this.pictureBox1.TabIndex = 3;
            this.pictureBox1.TabStop = false;
            // 
            // floatSlider1
            // 
            this.floatSlider1.Location = new System.Drawing.Point(0, 0);
            this.floatSlider1.Name = "floatSlider1";
            this.floatSlider1.ShowLabels = true;
            this.floatSlider1.Size = new System.Drawing.Size(220, 45);
            this.floatSlider1.TabIndex = 0;
            this.floatSlider1.Value = 0F;
            // 
            // floatSlider2
            // 
            this.floatSlider2.Location = new System.Drawing.Point(0, 45);
            this.floatSlider2.Name = "floatSlider2";
            this.floatSlider2.ShowLabels = false;
            this.floatSlider2.Size = new System.Drawing.Size(220, 45);
            this.floatSlider2.TabIndex = 1;
            this.floatSlider2.Value = 0F;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(15, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "x:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 64);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(15, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "y:";
            // 
            // ScaleControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.floatSlider1);
            this.Controls.Add(this.floatSlider2);
            this.Name = "ScaleControl";
            this.Size = new System.Drawing.Size(220, 94);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private FloatSlider floatSlider1;
        private FloatSlider floatSlider2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
    }
}
