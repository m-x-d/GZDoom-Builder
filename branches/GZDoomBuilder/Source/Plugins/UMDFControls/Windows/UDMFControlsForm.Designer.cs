namespace CodeImp.DoomBuilder.UDMFControls
{
    partial class UDMFControlsForm {
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

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Обязательный метод для поддержки конструктора - не изменяйте
        /// содержимое данного метода при помощи редактора кода.
        /// </summary>
        private void InitializeComponent() {
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.gbRotation = new System.Windows.Forms.GroupBox();
            this.angleControl1 = new CodeImp.DoomBuilder.UDMFControls.AngleControl();
            this.gbPosition = new System.Windows.Forms.GroupBox();
            this.positionControl1 = new CodeImp.DoomBuilder.UDMFControls.PositionControl();
            this.gbScale = new System.Windows.Forms.GroupBox();
            this.scaleControl = new CodeImp.DoomBuilder.UDMFControls.ScaleControl();
            this.bgBrightness = new System.Windows.Forms.GroupBox();
            this.cblightabsolute = new System.Windows.Forms.CheckBox();
            this.sliderBrightness = new CodeImp.DoomBuilder.UDMFControls.IntSlider();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cbRelativeMode = new System.Windows.Forms.CheckBox();
            this.gbAlpha = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cbRenderStyle = new System.Windows.Forms.ComboBox();
            this.sliderAlpha = new CodeImp.DoomBuilder.UDMFControls.FloatSlider();
            this.labelGravity = new System.Windows.Forms.Label();
            this.nudGravity = new System.Windows.Forms.NumericUpDown();
            this.gbDesaturation = new System.Windows.Forms.GroupBox();
            this.sliderDesaturation = new CodeImp.DoomBuilder.UDMFControls.FloatSlider();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.gbFlagsFloor = new System.Windows.Forms.GroupBox();
            this.cbhidden = new System.Windows.Forms.CheckBox();
            this.cbsilent = new System.Windows.Forms.CheckBox();
            this.cbnorespawn = new System.Windows.Forms.CheckBox();
            this.cbnofallingdamage = new System.Windows.Forms.CheckBox();
            this.cbdropactors = new System.Windows.Forms.CheckBox();
            this.gbFlagsWall = new System.Windows.Forms.GroupBox();
            this.cblightfog = new System.Windows.Forms.CheckBox();
            this.cbsmoothlighting = new System.Windows.Forms.CheckBox();
            this.cbnodecals = new System.Windows.Forms.CheckBox();
            this.cbnofakecontrast = new System.Windows.Forms.CheckBox();
            this.cbwrapmidtex = new System.Windows.Forms.CheckBox();
            this.cbclipmidtex = new System.Windows.Forms.CheckBox();
            this.gbRotation.SuspendLayout();
            this.gbPosition.SuspendLayout();
            this.gbScale.SuspendLayout();
            this.bgBrightness.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.gbAlpha.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudGravity)).BeginInit();
            this.gbDesaturation.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.gbFlagsFloor.SuspendLayout();
            this.gbFlagsWall.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(3, 680);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(125, 25);
            this.btnOK.TabIndex = 0;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(132, 680);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(125, 25);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // gbRotation
            // 
            this.gbRotation.Controls.Add(this.angleControl1);
            this.gbRotation.Location = new System.Drawing.Point(116, 19);
            this.gbRotation.Name = "gbRotation";
            this.gbRotation.Size = new System.Drawing.Size(114, 150);
            this.gbRotation.TabIndex = 4;
            this.gbRotation.TabStop = false;
            this.gbRotation.Text = "Rotation:";
            // 
            // angleControl1
            // 
            this.angleControl1.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.angleControl1.Location = new System.Drawing.Point(6, 19);
            this.angleControl1.Name = "angleControl1";
            this.angleControl1.Size = new System.Drawing.Size(102, 125);
            this.angleControl1.TabIndex = 2;
            this.angleControl1.Tag = "rotation";
            this.angleControl1.Value = 0F;
            this.angleControl1.OnAngleChanged += new System.EventHandler(this.angleControl1_OnAngleChanged);
            // 
            // gbPosition
            // 
            this.gbPosition.Controls.Add(this.positionControl1);
            this.gbPosition.Location = new System.Drawing.Point(6, 19);
            this.gbPosition.Name = "gbPosition";
            this.gbPosition.Size = new System.Drawing.Size(105, 150);
            this.gbPosition.TabIndex = 1;
            this.gbPosition.TabStop = false;
            this.gbPosition.Text = "Position:";
            // 
            // positionControl1
            // 
            this.positionControl1.Location = new System.Drawing.Point(-2, 20);
            this.positionControl1.Name = "positionControl1";
            this.positionControl1.Size = new System.Drawing.Size(106, 127);
            this.positionControl1.TabIndex = 0;
            this.positionControl1.Tag = "offset";
            this.positionControl1.OnValueChanged += new System.EventHandler(this.positionControl1_OnValueChanged);
            // 
            // gbScale
            // 
            this.gbScale.Controls.Add(this.scaleControl);
            this.gbScale.Location = new System.Drawing.Point(6, 172);
            this.gbScale.Name = "gbScale";
            this.gbScale.Size = new System.Drawing.Size(224, 119);
            this.gbScale.TabIndex = 5;
            this.gbScale.TabStop = false;
            this.gbScale.Text = "Scale:";
            // 
            // scaleControl
            // 
            this.scaleControl.Location = new System.Drawing.Point(3, 19);
            this.scaleControl.Name = "scaleControl";
            this.scaleControl.Size = new System.Drawing.Size(220, 94);
            this.scaleControl.TabIndex = 0;
            this.scaleControl.Tag = "scale";
            this.scaleControl.OnValueChanged += new System.EventHandler(this.scaleControl_OnValueChanged);
            // 
            // bgBrightness
            // 
            this.bgBrightness.Controls.Add(this.cblightabsolute);
            this.bgBrightness.Controls.Add(this.sliderBrightness);
            this.bgBrightness.Location = new System.Drawing.Point(5, 337);
            this.bgBrightness.Name = "bgBrightness";
            this.bgBrightness.Size = new System.Drawing.Size(234, 94);
            this.bgBrightness.TabIndex = 6;
            this.bgBrightness.TabStop = false;
            this.bgBrightness.Text = "Brightness:";
            // 
            // cblightabsolute
            // 
            this.cblightabsolute.AutoSize = true;
            this.cblightabsolute.Location = new System.Drawing.Point(10, 70);
            this.cblightabsolute.Name = "cblightabsolute";
            this.cblightabsolute.Size = new System.Drawing.Size(109, 18);
            this.cblightabsolute.TabIndex = 1;
            this.cblightabsolute.Tag = "lightabsolute";
            this.cblightabsolute.Text = "Absolute Lighting";
            this.cblightabsolute.UseVisualStyleBackColor = true;
            this.cblightabsolute.CheckedChanged += new System.EventHandler(this.cblightabsolute_CheckedChanged);
            // 
            // sliderBrightness
            // 
            this.sliderBrightness.Location = new System.Drawing.Point(6, 19);
            this.sliderBrightness.Name = "sliderBrightness";
            this.sliderBrightness.ShowLabels = true;
            this.sliderBrightness.Size = new System.Drawing.Size(220, 45);
            this.sliderBrightness.TabIndex = 0;
            this.sliderBrightness.Tag = "light";
            this.sliderBrightness.Value = 0;
            this.sliderBrightness.OnValueChanged += new System.EventHandler(this.sliderBrightness_OnValueChanged);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(3, 3);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(254, 671);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.groupBox1);
            this.tabPage1.Controls.Add(this.gbAlpha);
            this.tabPage1.Controls.Add(this.labelGravity);
            this.tabPage1.Controls.Add(this.nudGravity);
            this.tabPage1.Controls.Add(this.gbDesaturation);
            this.tabPage1.Controls.Add(this.bgBrightness);
            this.tabPage1.Location = new System.Drawing.Point(4, 23);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(246, 644);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Properties";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.gbPosition);
            this.groupBox1.Controls.Add(this.cbRelativeMode);
            this.groupBox1.Controls.Add(this.gbRotation);
            this.groupBox1.Controls.Add(this.gbScale);
            this.groupBox1.Location = new System.Drawing.Point(5, 6);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(234, 325);
            this.groupBox1.TabIndex = 13;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Transform:";
            // 
            // cbRelativeMode
            // 
            this.cbRelativeMode.AutoSize = true;
            this.cbRelativeMode.Location = new System.Drawing.Point(6, 297);
            this.cbRelativeMode.Name = "cbRelativeMode";
            this.cbRelativeMode.Size = new System.Drawing.Size(93, 18);
            this.cbRelativeMode.TabIndex = 12;
            this.cbRelativeMode.Text = "Relative mode";
            this.cbRelativeMode.UseVisualStyleBackColor = true;
            this.cbRelativeMode.CheckedChanged += new System.EventHandler(this.cbRelativeMode_CheckedChanged);
            // 
            // gbAlpha
            // 
            this.gbAlpha.Controls.Add(this.label2);
            this.gbAlpha.Controls.Add(this.cbRenderStyle);
            this.gbAlpha.Controls.Add(this.sliderAlpha);
            this.gbAlpha.Location = new System.Drawing.Point(5, 437);
            this.gbAlpha.Name = "gbAlpha";
            this.gbAlpha.Size = new System.Drawing.Size(234, 100);
            this.gbAlpha.TabIndex = 11;
            this.gbAlpha.TabStop = false;
            this.gbAlpha.Text = "Transparency:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(7, 74);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(72, 14);
            this.label2.TabIndex = 12;
            this.label2.Text = "Render Style:";
            // 
            // cbRenderStyle
            // 
            this.cbRenderStyle.FormattingEnabled = true;
            this.cbRenderStyle.Location = new System.Drawing.Point(85, 70);
            this.cbRenderStyle.Name = "cbRenderStyle";
            this.cbRenderStyle.Size = new System.Drawing.Size(141, 22);
            this.cbRenderStyle.TabIndex = 1;
            this.cbRenderStyle.Tag = "renderstyle";
            this.cbRenderStyle.SelectedIndexChanged += new System.EventHandler(this.cbRenderStyle_SelectedIndexChanged);
            // 
            // sliderAlpha
            // 
            this.sliderAlpha.Location = new System.Drawing.Point(6, 19);
            this.sliderAlpha.Name = "sliderAlpha";
            this.sliderAlpha.ShowLabels = true;
            this.sliderAlpha.Size = new System.Drawing.Size(220, 45);
            this.sliderAlpha.TabIndex = 0;
            this.sliderAlpha.Tag = "alpha";
            this.sliderAlpha.Value = 0F;
            this.sliderAlpha.OnValueChanged += new System.EventHandler(this.sliderAlpha_OnValueChanged);
            // 
            // labelGravity
            // 
            this.labelGravity.AutoSize = true;
            this.labelGravity.Location = new System.Drawing.Point(12, 621);
            this.labelGravity.Name = "labelGravity";
            this.labelGravity.Size = new System.Drawing.Size(45, 14);
            this.labelGravity.TabIndex = 0;
            this.labelGravity.Text = "Gravity:";
            // 
            // nudGravity
            // 
            this.nudGravity.DecimalPlaces = 1;
            this.nudGravity.Location = new System.Drawing.Point(63, 618);
            this.nudGravity.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            -2147483648});
            this.nudGravity.Name = "nudGravity";
            this.nudGravity.Size = new System.Drawing.Size(60, 20);
            this.nudGravity.TabIndex = 8;
            this.nudGravity.Tag = "gravity";
            // 
            // gbDesaturation
            // 
            this.gbDesaturation.Controls.Add(this.sliderDesaturation);
            this.gbDesaturation.Location = new System.Drawing.Point(5, 543);
            this.gbDesaturation.Name = "gbDesaturation";
            this.gbDesaturation.Size = new System.Drawing.Size(234, 70);
            this.gbDesaturation.TabIndex = 7;
            this.gbDesaturation.TabStop = false;
            this.gbDesaturation.Text = "Desaturation:";
            // 
            // sliderDesaturation
            // 
            this.sliderDesaturation.Location = new System.Drawing.Point(6, 19);
            this.sliderDesaturation.Name = "sliderDesaturation";
            this.sliderDesaturation.ShowLabels = true;
            this.sliderDesaturation.Size = new System.Drawing.Size(220, 45);
            this.sliderDesaturation.TabIndex = 0;
            this.sliderDesaturation.Tag = "desaturation";
            this.sliderDesaturation.Value = 0F;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.gbFlagsFloor);
            this.tabPage2.Controls.Add(this.gbFlagsWall);
            this.tabPage2.Location = new System.Drawing.Point(4, 23);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(246, 644);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Flags";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // gbFlagsFloor
            // 
            this.gbFlagsFloor.Controls.Add(this.cbhidden);
            this.gbFlagsFloor.Controls.Add(this.cbsilent);
            this.gbFlagsFloor.Controls.Add(this.cbnorespawn);
            this.gbFlagsFloor.Controls.Add(this.cbnofallingdamage);
            this.gbFlagsFloor.Controls.Add(this.cbdropactors);
            this.gbFlagsFloor.Location = new System.Drawing.Point(6, 175);
            this.gbFlagsFloor.Name = "gbFlagsFloor";
            this.gbFlagsFloor.Size = new System.Drawing.Size(235, 139);
            this.gbFlagsFloor.TabIndex = 3;
            this.gbFlagsFloor.TabStop = false;
            this.gbFlagsFloor.Text = "Floor and Ceiling flags:";
            // 
            // cbhidden
            // 
            this.cbhidden.AutoSize = true;
            this.cbhidden.Location = new System.Drawing.Point(6, 43);
            this.cbhidden.Name = "cbhidden";
            this.cbhidden.Size = new System.Drawing.Size(59, 18);
            this.cbhidden.TabIndex = 4;
            this.cbhidden.Tag = "hidden";
            this.cbhidden.Text = "Hidden";
            this.cbhidden.UseVisualStyleBackColor = true;
            // 
            // cbsilent
            // 
            this.cbsilent.AutoSize = true;
            this.cbsilent.Location = new System.Drawing.Point(6, 115);
            this.cbsilent.Name = "cbsilent";
            this.cbsilent.Size = new System.Drawing.Size(52, 18);
            this.cbsilent.TabIndex = 3;
            this.cbsilent.Tag = "silent";
            this.cbsilent.Text = "Silent";
            this.cbsilent.UseVisualStyleBackColor = true;
            // 
            // cbnorespawn
            // 
            this.cbnorespawn.AutoSize = true;
            this.cbnorespawn.Location = new System.Drawing.Point(6, 91);
            this.cbnorespawn.Name = "cbnorespawn";
            this.cbnorespawn.Size = new System.Drawing.Size(89, 18);
            this.cbnorespawn.TabIndex = 2;
            this.cbnorespawn.Tag = "norespawn";
            this.cbnorespawn.Text = "No Respawn";
            this.cbnorespawn.UseVisualStyleBackColor = true;
            // 
            // cbnofallingdamage
            // 
            this.cbnofallingdamage.AutoSize = true;
            this.cbnofallingdamage.Location = new System.Drawing.Point(6, 67);
            this.cbnofallingdamage.Name = "cbnofallingdamage";
            this.cbnofallingdamage.Size = new System.Drawing.Size(114, 18);
            this.cbnofallingdamage.TabIndex = 1;
            this.cbnofallingdamage.Tag = "nofallingdamage";
            this.cbnofallingdamage.Text = "No Falling Damage";
            this.cbnofallingdamage.UseVisualStyleBackColor = true;
            // 
            // cbdropactors
            // 
            this.cbdropactors.AutoSize = true;
            this.cbdropactors.Location = new System.Drawing.Point(6, 19);
            this.cbdropactors.Name = "cbdropactors";
            this.cbdropactors.Size = new System.Drawing.Size(84, 18);
            this.cbdropactors.TabIndex = 0;
            this.cbdropactors.Tag = "dropactors";
            this.cbdropactors.Text = "Drop Actors";
            this.cbdropactors.UseVisualStyleBackColor = true;
            // 
            // gbFlagsWall
            // 
            this.gbFlagsWall.Controls.Add(this.cblightfog);
            this.gbFlagsWall.Controls.Add(this.cbsmoothlighting);
            this.gbFlagsWall.Controls.Add(this.cbnodecals);
            this.gbFlagsWall.Controls.Add(this.cbnofakecontrast);
            this.gbFlagsWall.Controls.Add(this.cbwrapmidtex);
            this.gbFlagsWall.Controls.Add(this.cbclipmidtex);
            this.gbFlagsWall.Location = new System.Drawing.Point(6, 6);
            this.gbFlagsWall.Name = "gbFlagsWall";
            this.gbFlagsWall.Size = new System.Drawing.Size(235, 163);
            this.gbFlagsWall.TabIndex = 2;
            this.gbFlagsWall.TabStop = false;
            this.gbFlagsWall.Text = "Wall flags:";
            // 
            // cblightfog
            // 
            this.cblightfog.AutoSize = true;
            this.cblightfog.Location = new System.Drawing.Point(6, 139);
            this.cblightfog.Name = "cblightfog";
            this.cblightfog.Size = new System.Drawing.Size(179, 18);
            this.cblightfog.TabIndex = 5;
            this.cblightfog.Tag = "lightfog";
            this.cblightfog.Text = "Use UDMF light on fogged walls";
            this.cblightfog.UseVisualStyleBackColor = true;
            this.cblightfog.CheckedChanged += new System.EventHandler(this.cblightfog_CheckedChanged);
            // 
            // cbsmoothlighting
            // 
            this.cbsmoothlighting.AutoSize = true;
            this.cbsmoothlighting.Location = new System.Drawing.Point(6, 115);
            this.cbsmoothlighting.Name = "cbsmoothlighting";
            this.cbsmoothlighting.Size = new System.Drawing.Size(102, 18);
            this.cbsmoothlighting.TabIndex = 4;
            this.cbsmoothlighting.Tag = "smoothlighting";
            this.cbsmoothlighting.Text = "Smooth Lighting";
            this.cbsmoothlighting.UseVisualStyleBackColor = true;
            // 
            // cbnodecals
            // 
            this.cbnodecals.AutoSize = true;
            this.cbnodecals.Location = new System.Drawing.Point(6, 91);
            this.cbnodecals.Name = "cbnodecals";
            this.cbnodecals.Size = new System.Drawing.Size(75, 18);
            this.cbnodecals.TabIndex = 3;
            this.cbnodecals.Tag = "nodecals";
            this.cbnodecals.Text = "No Decals";
            this.cbnodecals.UseVisualStyleBackColor = true;
            // 
            // cbnofakecontrast
            // 
            this.cbnofakecontrast.AutoSize = true;
            this.cbnofakecontrast.Location = new System.Drawing.Point(6, 67);
            this.cbnofakecontrast.Name = "cbnofakecontrast";
            this.cbnofakecontrast.Size = new System.Drawing.Size(109, 18);
            this.cbnofakecontrast.TabIndex = 2;
            this.cbnofakecontrast.Tag = "nofakecontrast";
            this.cbnofakecontrast.Text = "No Fake Contrast";
            this.cbnofakecontrast.UseVisualStyleBackColor = true;
            // 
            // cbwrapmidtex
            // 
            this.cbwrapmidtex.AutoSize = true;
            this.cbwrapmidtex.Location = new System.Drawing.Point(6, 43);
            this.cbwrapmidtex.Name = "cbwrapmidtex";
            this.cbwrapmidtex.Size = new System.Drawing.Size(124, 18);
            this.cbwrapmidtex.TabIndex = 1;
            this.cbwrapmidtex.Tag = "wrapmidtex";
            this.cbwrapmidtex.Text = "Wrap Middle Texture";
            this.cbwrapmidtex.UseVisualStyleBackColor = true;
            this.cbwrapmidtex.CheckedChanged += new System.EventHandler(this.cbwrapmidtex_CheckedChanged);
            // 
            // cbclipmidtex
            // 
            this.cbclipmidtex.AutoSize = true;
            this.cbclipmidtex.Location = new System.Drawing.Point(6, 19);
            this.cbclipmidtex.Name = "cbclipmidtex";
            this.cbclipmidtex.Size = new System.Drawing.Size(115, 18);
            this.cbclipmidtex.TabIndex = 0;
            this.cbclipmidtex.Tag = "clipmidtex";
            this.cbclipmidtex.Text = "Clip Middle Texture";
            this.cbclipmidtex.UseVisualStyleBackColor = true;
            // 
            // UDMFControlsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(259, 709);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "UDMFControlsForm";
            this.Opacity = 0;
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "UDMF Controls";
            this.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.UDMFControlsForm_HelpRequested);
            this.gbRotation.ResumeLayout(false);
            this.gbPosition.ResumeLayout(false);
            this.gbScale.ResumeLayout(false);
            this.bgBrightness.ResumeLayout(false);
            this.bgBrightness.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.gbAlpha.ResumeLayout(false);
            this.gbAlpha.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudGravity)).EndInit();
            this.gbDesaturation.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.gbFlagsFloor.ResumeLayout(false);
            this.gbFlagsFloor.PerformLayout();
            this.gbFlagsWall.ResumeLayout(false);
            this.gbFlagsWall.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private AngleControl angleControl1;
        private System.Windows.Forms.GroupBox gbRotation;
        private System.Windows.Forms.GroupBox gbPosition;
        private PositionControl positionControl1;
        private System.Windows.Forms.GroupBox gbScale;
        private ScaleControl scaleControl;
        private System.Windows.Forms.GroupBox bgBrightness;
        private IntSlider sliderBrightness;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Label labelGravity;
        private System.Windows.Forms.NumericUpDown nudGravity;
        private System.Windows.Forms.GroupBox gbDesaturation;
        private FloatSlider sliderDesaturation;
        private System.Windows.Forms.GroupBox gbFlagsFloor;
        private System.Windows.Forms.CheckBox cbsilent;
        private System.Windows.Forms.CheckBox cbnorespawn;
        private System.Windows.Forms.CheckBox cbnofallingdamage;
        private System.Windows.Forms.CheckBox cbdropactors;
        private System.Windows.Forms.GroupBox gbFlagsWall;
        private System.Windows.Forms.CheckBox cbsmoothlighting;
        private System.Windows.Forms.CheckBox cbnodecals;
        private System.Windows.Forms.CheckBox cbnofakecontrast;
        private System.Windows.Forms.CheckBox cbwrapmidtex;
        private System.Windows.Forms.CheckBox cbclipmidtex;
        private System.Windows.Forms.GroupBox gbAlpha;
        private FloatSlider sliderAlpha;
        private System.Windows.Forms.ComboBox cbRenderStyle;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox cblightabsolute;
        private System.Windows.Forms.CheckBox cbRelativeMode;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox cbhidden;
        private System.Windows.Forms.CheckBox cblightfog;
    }
}