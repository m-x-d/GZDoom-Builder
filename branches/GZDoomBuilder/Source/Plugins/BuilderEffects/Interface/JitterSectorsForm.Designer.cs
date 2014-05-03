namespace CodeImp.DoomBuilder.BuilderEffects
{
	partial class JitterSectorsForm
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

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.bCancel = new System.Windows.Forms.Button();
			this.bApply = new System.Windows.Forms.Button();
			this.bUpdateTranslation = new System.Windows.Forms.Button();
			this.bUpdateCeilingHeight = new System.Windows.Forms.Button();
			this.bUpdateFloorHeight = new System.Windows.Forms.Button();
			this.gbUpperTexture = new System.Windows.Forms.GroupBox();
			this.textureUpper = new CodeImp.DoomBuilder.Controls.TextureSelectorControl();
			this.cbPegTop = new System.Windows.Forms.CheckBox();
			this.textureLower = new CodeImp.DoomBuilder.Controls.TextureSelectorControl();
			this.cbPegBottom = new System.Windows.Forms.CheckBox();
			this.cbUpperTexStyle = new System.Windows.Forms.ComboBox();
			this.cbLowerTexStyle = new System.Windows.Forms.ComboBox();
			this.gbLowerTexture = new System.Windows.Forms.GroupBox();
			this.cbKeepExistingTextures = new System.Windows.Forms.CheckBox();
			this.positionJitterAmmount = new IntControl();
			this.floorHeightAmmount = new IntControl();
			this.ceilingHeightAmmount = new IntControl();
			this.gbUpperTexture.SuspendLayout();
			this.gbLowerTexture.SuspendLayout();
			this.SuspendLayout();
			// 
			// bCancel
			// 
			this.bCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.bCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.bCancel.Location = new System.Drawing.Point(195, 324);
			this.bCancel.Name = "bCancel";
			this.bCancel.Size = new System.Drawing.Size(75, 25);
			this.bCancel.TabIndex = 10;
			this.bCancel.Text = "Cancel";
			this.bCancel.UseVisualStyleBackColor = true;
			this.bCancel.Click += new System.EventHandler(this.bCancel_Click);
			// 
			// bApply
			// 
			this.bApply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.bApply.Location = new System.Drawing.Point(276, 324);
			this.bApply.Name = "bApply";
			this.bApply.Size = new System.Drawing.Size(95, 25);
			this.bApply.TabIndex = 9;
			this.bApply.Text = "Apply";
			this.bApply.UseVisualStyleBackColor = true;
			this.bApply.Click += new System.EventHandler(this.bApply_Click);
			// 
			// bUpdateTranslation
			// 
			this.bUpdateTranslation.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.bUpdateTranslation.Image = global::CodeImp.DoomBuilder.BuilderEffects.Properties.Resources.Update;
			this.bUpdateTranslation.Location = new System.Drawing.Point(347, 14);
			this.bUpdateTranslation.Name = "bUpdateTranslation";
			this.bUpdateTranslation.Size = new System.Drawing.Size(23, 25);
			this.bUpdateTranslation.TabIndex = 11;
			this.bUpdateTranslation.UseVisualStyleBackColor = true;
			this.bUpdateTranslation.Click += new System.EventHandler(this.bUpdateTranslation_Click);
			// 
			// bUpdateCeilingHeight
			// 
			this.bUpdateCeilingHeight.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.bUpdateCeilingHeight.Image = global::CodeImp.DoomBuilder.BuilderEffects.Properties.Resources.Update;
			this.bUpdateCeilingHeight.Location = new System.Drawing.Point(347, 44);
			this.bUpdateCeilingHeight.Name = "bUpdateCeilingHeight";
			this.bUpdateCeilingHeight.Size = new System.Drawing.Size(23, 25);
			this.bUpdateCeilingHeight.TabIndex = 15;
			this.bUpdateCeilingHeight.UseVisualStyleBackColor = true;
			this.bUpdateCeilingHeight.Click += new System.EventHandler(this.bUpdateCeilingHeight_Click);
			// 
			// bUpdateFloorHeight
			// 
			this.bUpdateFloorHeight.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.bUpdateFloorHeight.Image = global::CodeImp.DoomBuilder.BuilderEffects.Properties.Resources.Update;
			this.bUpdateFloorHeight.Location = new System.Drawing.Point(347, 74);
			this.bUpdateFloorHeight.Name = "bUpdateFloorHeight";
			this.bUpdateFloorHeight.Size = new System.Drawing.Size(23, 25);
			this.bUpdateFloorHeight.TabIndex = 16;
			this.bUpdateFloorHeight.UseVisualStyleBackColor = true;
			this.bUpdateFloorHeight.Click += new System.EventHandler(this.bUpdateFloorHeight_Click);
			// 
			// gbUpperTexture
			// 
			this.gbUpperTexture.Controls.Add(this.textureUpper);
			this.gbUpperTexture.Controls.Add(this.cbPegTop);
			this.gbUpperTexture.Location = new System.Drawing.Point(12, 159);
			this.gbUpperTexture.Name = "gbUpperTexture";
			this.gbUpperTexture.Size = new System.Drawing.Size(176, 159);
			this.gbUpperTexture.TabIndex = 24;
			this.gbUpperTexture.TabStop = false;
			this.gbUpperTexture.Text = "Upper Texture:";
			// 
			// textureUpper
			// 
			this.textureUpper.Location = new System.Drawing.Point(47, 41);
			this.textureUpper.MultipleTextures = false;
			this.textureUpper.Name = "textureUpper";
			this.textureUpper.Required = false;
			this.textureUpper.Size = new System.Drawing.Size(83, 112);
			this.textureUpper.TabIndex = 2;
			this.textureUpper.TextureName = "";
			this.textureUpper.OnValueChanged += new System.EventHandler(this.textureUpper_OnValueChanged);
			// 
			// cbPegTop
			// 
			this.cbPegTop.AutoSize = true;
			this.cbPegTop.Location = new System.Drawing.Point(6, 19);
			this.cbPegTop.Name = "cbPegTop";
			this.cbPegTop.Size = new System.Drawing.Size(107, 18);
			this.cbPegTop.TabIndex = 26;
			this.cbPegTop.Text = "Upper Unpegged";
			this.cbPegTop.UseVisualStyleBackColor = true;
			this.cbPegTop.CheckedChanged += new System.EventHandler(this.cbPegTop_CheckedChanged);
			// 
			// textureLower
			// 
			this.textureLower.Location = new System.Drawing.Point(47, 41);
			this.textureLower.MultipleTextures = false;
			this.textureLower.Name = "textureLower";
			this.textureLower.Required = false;
			this.textureLower.Size = new System.Drawing.Size(83, 112);
			this.textureLower.TabIndex = 4;
			this.textureLower.TextureName = "";
			this.textureLower.OnValueChanged += new System.EventHandler(this.textureLower_OnValueChanged);
			// 
			// cbPegBottom
			// 
			this.cbPegBottom.AutoSize = true;
			this.cbPegBottom.Location = new System.Drawing.Point(6, 19);
			this.cbPegBottom.Name = "cbPegBottom";
			this.cbPegBottom.Size = new System.Drawing.Size(110, 18);
			this.cbPegBottom.TabIndex = 28;
			this.cbPegBottom.Text = "Lower Unpegged";
			this.cbPegBottom.UseVisualStyleBackColor = true;
			this.cbPegBottom.CheckedChanged += new System.EventHandler(this.cbPegBottom_CheckedChanged);
			// 
			// cbUpperTexStyle
			// 
			this.cbUpperTexStyle.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbUpperTexStyle.FormattingEnabled = true;
			this.cbUpperTexStyle.Items.AddRange(new object[] {
            "Don\'t change upper texture",
            "Use ceiling texture",
            "Pick upper texture"});
			this.cbUpperTexStyle.Location = new System.Drawing.Point(12, 131);
			this.cbUpperTexStyle.Name = "cbUpperTexStyle";
			this.cbUpperTexStyle.Size = new System.Drawing.Size(176, 22);
			this.cbUpperTexStyle.TabIndex = 26;
			this.cbUpperTexStyle.SelectedIndexChanged += new System.EventHandler(this.cbUpperTexStyle_SelectedIndexChanged);
			// 
			// cbLowerTexStyle
			// 
			this.cbLowerTexStyle.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbLowerTexStyle.FormattingEnabled = true;
			this.cbLowerTexStyle.Items.AddRange(new object[] {
            "Don\'t change lower texture",
            "Use floor texture",
            "Pick lower texture"});
			this.cbLowerTexStyle.Location = new System.Drawing.Point(195, 131);
			this.cbLowerTexStyle.Name = "cbLowerTexStyle";
			this.cbLowerTexStyle.Size = new System.Drawing.Size(175, 22);
			this.cbLowerTexStyle.TabIndex = 27;
			this.cbLowerTexStyle.SelectedIndexChanged += new System.EventHandler(this.cbLowerTexStyle_SelectedIndexChanged);
			// 
			// gbLowerTexture
			// 
			this.gbLowerTexture.Controls.Add(this.textureLower);
			this.gbLowerTexture.Controls.Add(this.cbPegBottom);
			this.gbLowerTexture.Location = new System.Drawing.Point(195, 159);
			this.gbLowerTexture.Name = "gbLowerTexture";
			this.gbLowerTexture.Size = new System.Drawing.Size(176, 159);
			this.gbLowerTexture.TabIndex = 29;
			this.gbLowerTexture.TabStop = false;
			this.gbLowerTexture.Text = "Lower texture:";
			// 
			// cbKeepExistingTextures
			// 
			this.cbKeepExistingTextures.AutoSize = true;
			this.cbKeepExistingTextures.Location = new System.Drawing.Point(12, 107);
			this.cbKeepExistingTextures.Name = "cbKeepExistingTextures";
			this.cbKeepExistingTextures.Size = new System.Drawing.Size(211, 18);
			this.cbKeepExistingTextures.TabIndex = 30;
			this.cbKeepExistingTextures.Text = "Don\'t change existing sidedef textures";
			this.cbKeepExistingTextures.UseVisualStyleBackColor = true;
			this.cbKeepExistingTextures.CheckedChanged += new System.EventHandler(this.cbKeepExistingTextures_CheckedChanged);
			// 
			// positionJitterAmmount
			// 
			this.positionJitterAmmount.AllowNegative = false;
			this.positionJitterAmmount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.positionJitterAmmount.ExtendedLimits = true;
			this.positionJitterAmmount.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.positionJitterAmmount.Label = "Position:";
			this.positionJitterAmmount.Location = new System.Drawing.Point(12, 15);
			this.positionJitterAmmount.Maximum = 100;
			this.positionJitterAmmount.Minimum = 0;
			this.positionJitterAmmount.Name = "positionJitterAmmount";
			this.positionJitterAmmount.Size = new System.Drawing.Size(329, 22);
			this.positionJitterAmmount.TabIndex = 18;
			this.positionJitterAmmount.Value = 0;
			this.positionJitterAmmount.OnValueChanging += new System.EventHandler(this.positionJitterAmmount_OnValueChanging);
			// 
			// floorHeightAmmount
			// 
			this.floorHeightAmmount.AllowNegative = false;
			this.floorHeightAmmount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.floorHeightAmmount.ExtendedLimits = false;
			this.floorHeightAmmount.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.floorHeightAmmount.Label = "Floor height:";
			this.floorHeightAmmount.Location = new System.Drawing.Point(12, 75);
			this.floorHeightAmmount.Maximum = 100;
			this.floorHeightAmmount.Minimum = 0;
			this.floorHeightAmmount.Name = "floorHeightAmmount";
			this.floorHeightAmmount.Size = new System.Drawing.Size(329, 22);
			this.floorHeightAmmount.TabIndex = 20;
			this.floorHeightAmmount.Value = 0;
			this.floorHeightAmmount.OnValueChanging += new System.EventHandler(this.floorHeightAmmount_OnValueChanging);
			// 
			// ceilingHeightAmmount
			// 
			this.ceilingHeightAmmount.AllowNegative = false;
			this.ceilingHeightAmmount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.ceilingHeightAmmount.ExtendedLimits = false;
			this.ceilingHeightAmmount.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ceilingHeightAmmount.Label = "Ceil. height:";
			this.ceilingHeightAmmount.Location = new System.Drawing.Point(12, 45);
			this.ceilingHeightAmmount.Maximum = 100;
			this.ceilingHeightAmmount.Minimum = 0;
			this.ceilingHeightAmmount.Name = "ceilingHeightAmmount";
			this.ceilingHeightAmmount.Size = new System.Drawing.Size(329, 22);
			this.ceilingHeightAmmount.TabIndex = 19;
			this.ceilingHeightAmmount.Value = 0;
			this.ceilingHeightAmmount.OnValueChanging += new System.EventHandler(this.ceilingHeightAmmount_OnValueChanging);
			// 
			// JitterSectorsForm
			// 
			this.AcceptButton = this.bApply;
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.CancelButton = this.bCancel;
			this.ClientSize = new System.Drawing.Size(382, 352);
			this.Controls.Add(this.cbKeepExistingTextures);
			this.Controls.Add(this.gbLowerTexture);
			this.Controls.Add(this.cbLowerTexStyle);
			this.Controls.Add(this.cbUpperTexStyle);
			this.Controls.Add(this.gbUpperTexture);
			this.Controls.Add(this.floorHeightAmmount);
			this.Controls.Add(this.ceilingHeightAmmount);
			this.Controls.Add(this.positionJitterAmmount);
			this.Controls.Add(this.bUpdateFloorHeight);
			this.Controls.Add(this.bUpdateCeilingHeight);
			this.Controls.Add(this.bUpdateTranslation);
			this.Controls.Add(this.bCancel);
			this.Controls.Add(this.bApply);
			this.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Name = "JitterSectorsForm";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
			this.Text = "Jitter Settings";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.JitterSectorsForm_FormClosing);
			this.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.JitterSectorsForm_HelpRequested);
			this.gbUpperTexture.ResumeLayout(false);
			this.gbUpperTexture.PerformLayout();
			this.gbLowerTexture.ResumeLayout(false);
			this.gbLowerTexture.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button bUpdateTranslation;
		private System.Windows.Forms.Button bCancel;
		private System.Windows.Forms.Button bApply;
		private System.Windows.Forms.Button bUpdateCeilingHeight;
		private System.Windows.Forms.Button bUpdateFloorHeight;
		private IntControl positionJitterAmmount;
		private IntControl ceilingHeightAmmount;
		private IntControl floorHeightAmmount;
		private System.Windows.Forms.GroupBox gbUpperTexture;
		private CodeImp.DoomBuilder.Controls.TextureSelectorControl textureLower;
		private CodeImp.DoomBuilder.Controls.TextureSelectorControl textureUpper;
		private System.Windows.Forms.CheckBox cbPegBottom;
		private System.Windows.Forms.CheckBox cbPegTop;
		private System.Windows.Forms.ComboBox cbUpperTexStyle;
		private System.Windows.Forms.ComboBox cbLowerTexStyle;
		private System.Windows.Forms.GroupBox gbLowerTexture;
		private System.Windows.Forms.CheckBox cbKeepExistingTextures;
	}
}