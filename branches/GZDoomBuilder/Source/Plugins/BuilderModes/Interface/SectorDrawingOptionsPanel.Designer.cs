namespace CodeImp.DoomBuilder.BuilderModes.Interface
{
	partial class SectorDrawingOptionsPanel
	{
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.cbOverrideCeilingTexture = new System.Windows.Forms.CheckBox();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.cbOverrideFloorTexture = new System.Windows.Forms.CheckBox();
			this.cbOverrideWallTexture = new System.Windows.Forms.CheckBox();
			this.cbFloorHeight = new System.Windows.Forms.CheckBox();
			this.cbCeilHeight = new System.Windows.Forms.CheckBox();
			this.cbBrightness = new System.Windows.Forms.CheckBox();
			this.label14 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.brightness = new CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox();
			this.floorHeight = new CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox();
			this.ceilHeight = new CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox();
			this.walls = new CodeImp.DoomBuilder.Controls.TextureSelectorControl();
			this.floor = new CodeImp.DoomBuilder.Controls.TextureSelectorControl();
			this.ceiling = new CodeImp.DoomBuilder.Controls.TextureSelectorControl();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBox1
			// 
			this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox1.Controls.Add(this.cbOverrideWallTexture);
			this.groupBox1.Controls.Add(this.cbOverrideFloorTexture);
			this.groupBox1.Controls.Add(this.walls);
			this.groupBox1.Controls.Add(this.cbOverrideCeilingTexture);
			this.groupBox1.Controls.Add(this.floor);
			this.groupBox1.Controls.Add(this.ceiling);
			this.groupBox1.Location = new System.Drawing.Point(3, 3);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(243, 138);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Texture overrides:";
			// 
			// cbOverrideCeilingTexture
			// 
			this.cbOverrideCeilingTexture.AutoSize = true;
			this.cbOverrideCeilingTexture.Location = new System.Drawing.Point(7, 19);
			this.cbOverrideCeilingTexture.Name = "cbOverrideCeilingTexture";
			this.cbOverrideCeilingTexture.Size = new System.Drawing.Size(57, 17);
			this.cbOverrideCeilingTexture.TabIndex = 20;
			this.cbOverrideCeilingTexture.Text = "Ceiling";
			this.cbOverrideCeilingTexture.UseVisualStyleBackColor = true;
			this.cbOverrideCeilingTexture.CheckedChanged += new System.EventHandler(this.cbOverrideCeilingTexture_CheckedChanged);
			// 
			// groupBox2
			// 
			this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox2.Controls.Add(this.label1);
			this.groupBox2.Controls.Add(this.label14);
			this.groupBox2.Controls.Add(this.cbBrightness);
			this.groupBox2.Controls.Add(this.brightness);
			this.groupBox2.Controls.Add(this.floorHeight);
			this.groupBox2.Controls.Add(this.cbCeilHeight);
			this.groupBox2.Controls.Add(this.cbFloorHeight);
			this.groupBox2.Controls.Add(this.ceilHeight);
			this.groupBox2.Location = new System.Drawing.Point(3, 147);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(243, 118);
			this.groupBox2.TabIndex = 21;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Geometry overrides:";
			// 
			// cbOverrideFloorTexture
			// 
			this.cbOverrideFloorTexture.AutoSize = true;
			this.cbOverrideFloorTexture.Location = new System.Drawing.Point(88, 19);
			this.cbOverrideFloorTexture.Name = "cbOverrideFloorTexture";
			this.cbOverrideFloorTexture.Size = new System.Drawing.Size(49, 17);
			this.cbOverrideFloorTexture.TabIndex = 21;
			this.cbOverrideFloorTexture.Text = "Floor";
			this.cbOverrideFloorTexture.UseVisualStyleBackColor = true;
			this.cbOverrideFloorTexture.CheckedChanged += new System.EventHandler(this.cbOverrideFloorTexture_CheckedChanged);
			// 
			// cbOverrideWallTexture
			// 
			this.cbOverrideWallTexture.AutoSize = true;
			this.cbOverrideWallTexture.Location = new System.Drawing.Point(170, 19);
			this.cbOverrideWallTexture.Name = "cbOverrideWallTexture";
			this.cbOverrideWallTexture.Size = new System.Drawing.Size(52, 17);
			this.cbOverrideWallTexture.TabIndex = 22;
			this.cbOverrideWallTexture.Text = "Walls";
			this.cbOverrideWallTexture.UseVisualStyleBackColor = true;
			this.cbOverrideWallTexture.CheckedChanged += new System.EventHandler(this.cbOverrideWallTexture_CheckedChanged);
			// 
			// cbFloorHeight
			// 
			this.cbFloorHeight.AutoSize = true;
			this.cbFloorHeight.Location = new System.Drawing.Point(7, 52);
			this.cbFloorHeight.Name = "cbFloorHeight";
			this.cbFloorHeight.Size = new System.Drawing.Size(84, 17);
			this.cbFloorHeight.TabIndex = 23;
			this.cbFloorHeight.Text = "Floor height:";
			this.cbFloorHeight.UseVisualStyleBackColor = true;
			this.cbFloorHeight.CheckedChanged += new System.EventHandler(this.cbFloorHeight_CheckedChanged);
			// 
			// cbCeilHeight
			// 
			this.cbCeilHeight.AutoSize = true;
			this.cbCeilHeight.Location = new System.Drawing.Point(7, 23);
			this.cbCeilHeight.Name = "cbCeilHeight";
			this.cbCeilHeight.Size = new System.Drawing.Size(92, 17);
			this.cbCeilHeight.TabIndex = 24;
			this.cbCeilHeight.Text = "Ceiling height:";
			this.cbCeilHeight.UseVisualStyleBackColor = true;
			this.cbCeilHeight.CheckedChanged += new System.EventHandler(this.cbCeilHeight_CheckedChanged);
			// 
			// cbBrightness
			// 
			this.cbBrightness.AutoSize = true;
			this.cbBrightness.Location = new System.Drawing.Point(7, 82);
			this.cbBrightness.Name = "cbBrightness";
			this.cbBrightness.Size = new System.Drawing.Size(78, 17);
			this.cbBrightness.TabIndex = 27;
			this.cbBrightness.Text = "Brightness:";
			this.cbBrightness.UseVisualStyleBackColor = true;
			this.cbBrightness.CheckedChanged += new System.EventHandler(this.cbBrightness_CheckedChanged);
			// 
			// label14
			// 
			this.label14.AutoSize = true;
			this.label14.Location = new System.Drawing.Point(182, 24);
			this.label14.Name = "label14";
			this.label14.Size = new System.Drawing.Size(27, 13);
			this.label14.TabIndex = 28;
			this.label14.Text = "m.u.";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(182, 53);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(27, 13);
			this.label1.TabIndex = 29;
			this.label1.Text = "m.u.";
			// 
			// brightness
			// 
			this.brightness.AllowDecimal = false;
			this.brightness.AllowNegative = false;
			this.brightness.AllowRelative = false;
			this.brightness.ButtonStep = 16;
			this.brightness.ButtonStepFloat = 1F;
			this.brightness.Location = new System.Drawing.Point(105, 78);
			this.brightness.Name = "brightness";
			this.brightness.Size = new System.Drawing.Size(72, 24);
			this.brightness.StepValues = null;
			this.brightness.TabIndex = 26;
			this.brightness.WhenTextChanged += new System.EventHandler(this.brightness_WhenTextChanged);
			// 
			// floorHeight
			// 
			this.floorHeight.AllowDecimal = false;
			this.floorHeight.AllowNegative = true;
			this.floorHeight.AllowRelative = false;
			this.floorHeight.ButtonStep = 16;
			this.floorHeight.ButtonStepFloat = 1F;
			this.floorHeight.Location = new System.Drawing.Point(105, 48);
			this.floorHeight.Name = "floorHeight";
			this.floorHeight.Size = new System.Drawing.Size(72, 24);
			this.floorHeight.StepValues = null;
			this.floorHeight.TabIndex = 25;
			this.floorHeight.WhenTextChanged += new System.EventHandler(this.floorHeight_WhenTextChanged);
			// 
			// ceilHeight
			// 
			this.ceilHeight.AllowDecimal = false;
			this.ceilHeight.AllowNegative = true;
			this.ceilHeight.AllowRelative = false;
			this.ceilHeight.ButtonStep = 16;
			this.ceilHeight.ButtonStepFloat = 1F;
			this.ceilHeight.Location = new System.Drawing.Point(105, 18);
			this.ceilHeight.Name = "ceilHeight";
			this.ceilHeight.Size = new System.Drawing.Size(72, 24);
			this.ceilHeight.StepValues = null;
			this.ceilHeight.TabIndex = 11;
			this.ceilHeight.WhenTextChanged += new System.EventHandler(this.ceilHeight_WhenTextChanged);
			// 
			// walls
			// 
			this.walls.Location = new System.Drawing.Point(168, 41);
			this.walls.Name = "walls";
			this.walls.Required = false;
			this.walls.Size = new System.Drawing.Size(68, 90);
			this.walls.TabIndex = 19;
			this.walls.TextureName = "";
			this.walls.OnValueChanged += new System.EventHandler(this.walls_OnValueChanged);
			// 
			// floor
			// 
			this.floor.Location = new System.Drawing.Point(87, 41);
			this.floor.Name = "floor";
			this.floor.Required = false;
			this.floor.Size = new System.Drawing.Size(68, 90);
			this.floor.TabIndex = 17;
			this.floor.TextureName = "";
			this.floor.OnValueChanged += new System.EventHandler(this.floor_OnValueChanged);
			// 
			// ceiling
			// 
			this.ceiling.Location = new System.Drawing.Point(6, 41);
			this.ceiling.Name = "ceiling";
			this.ceiling.Required = false;
			this.ceiling.Size = new System.Drawing.Size(68, 90);
			this.ceiling.TabIndex = 16;
			this.ceiling.TextureName = "";
			this.ceiling.OnValueChanged += new System.EventHandler(this.ceiling_OnValueChanged);
			// 
			// SectorDrawingOptionsPanel
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.groupBox1);
			this.Name = "SectorDrawingOptionsPanel";
			this.Size = new System.Drawing.Size(249, 600);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.CheckBox cbOverrideCeilingTexture;
		private CodeImp.DoomBuilder.Controls.TextureSelectorControl walls;
		private CodeImp.DoomBuilder.Controls.TextureSelectorControl floor;
		private CodeImp.DoomBuilder.Controls.TextureSelectorControl ceiling;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.CheckBox cbOverrideWallTexture;
		private System.Windows.Forms.CheckBox cbOverrideFloorTexture;
		private System.Windows.Forms.CheckBox cbBrightness;
		private CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox brightness;
		private CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox floorHeight;
		private System.Windows.Forms.CheckBox cbCeilHeight;
		private System.Windows.Forms.CheckBox cbFloorHeight;
		private CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox ceilHeight;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label14;
	}
}
