namespace CodeImp.DoomBuilder.GZBuilder.Controls
{
	partial class CustomLinedefColorsControl
	{
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) 
		{
			if(disposing && (components != null)) 
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() 
		{
			this.gbPresets = new System.Windows.Forms.GroupBox();
			this.bMoveUp = new System.Windows.Forms.Button();
			this.bMoveDown = new System.Windows.Forms.Button();
			this.bRemovePreset = new System.Windows.Forms.Button();
			this.lbColorPresets = new CodeImp.DoomBuilder.GZBuilder.Controls.IconListBox();
			this.tbNewPresetName = new System.Windows.Forms.TextBox();
			this.bAddPreset = new System.Windows.Forms.Button();
			this.colorProperties = new CodeImp.DoomBuilder.GZBuilder.Controls.CustomLinedefColorProperties();
			this.gbPresets.SuspendLayout();
			this.SuspendLayout();
			// 
			// gbPresets
			// 
			this.gbPresets.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)));
			this.gbPresets.Controls.Add(this.bMoveUp);
			this.gbPresets.Controls.Add(this.bMoveDown);
			this.gbPresets.Controls.Add(this.bRemovePreset);
			this.gbPresets.Controls.Add(this.lbColorPresets);
			this.gbPresets.Controls.Add(this.tbNewPresetName);
			this.gbPresets.Controls.Add(this.bAddPreset);
			this.gbPresets.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.gbPresets.Location = new System.Drawing.Point(3, 3);
			this.gbPresets.Name = "gbPresets";
			this.gbPresets.Size = new System.Drawing.Size(145, 250);
			this.gbPresets.TabIndex = 2;
			this.gbPresets.TabStop = false;
			this.gbPresets.Text = "Presets:";
			// 
			// bMoveUp
			// 
			this.bMoveUp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.bMoveUp.Image = global::CodeImp.DoomBuilder.Properties.Resources.ArrowUp;
			this.bMoveUp.Location = new System.Drawing.Point(102, 44);
			this.bMoveUp.Name = "bMoveUp";
			this.bMoveUp.Size = new System.Drawing.Size(18, 23);
			this.bMoveUp.TabIndex = 7;
			this.bMoveUp.UseVisualStyleBackColor = true;
			this.bMoveUp.Click += new System.EventHandler(this.bMoveUp_Click);
			// 
			// bMoveDown
			// 
			this.bMoveDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.bMoveDown.Image = global::CodeImp.DoomBuilder.Properties.Resources.ArrowDown;
			this.bMoveDown.Location = new System.Drawing.Point(121, 44);
			this.bMoveDown.Name = "bMoveDown";
			this.bMoveDown.Size = new System.Drawing.Size(18, 23);
			this.bMoveDown.TabIndex = 6;
			this.bMoveDown.UseVisualStyleBackColor = true;
			this.bMoveDown.Click += new System.EventHandler(this.bMoveDown_Click);
			// 
			// bRemovePreset
			// 
			this.bRemovePreset.Image = global::CodeImp.DoomBuilder.Properties.Resources.SearchClear;
			this.bRemovePreset.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.bRemovePreset.Location = new System.Drawing.Point(54, 44);
			this.bRemovePreset.Name = "bRemovePreset";
			this.bRemovePreset.Size = new System.Drawing.Size(47, 23);
			this.bRemovePreset.TabIndex = 5;
			this.bRemovePreset.Text = "Del.";
			this.bRemovePreset.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.bRemovePreset.UseVisualStyleBackColor = true;
			this.bRemovePreset.Click += new System.EventHandler(this.bRemovePreset_Click);
			// 
			// lbColorPresets
			// 
			this.lbColorPresets.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.lbColorPresets.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
			this.lbColorPresets.FormattingEnabled = true;
			this.lbColorPresets.HorizontalScrollbar = true;
			this.lbColorPresets.Location = new System.Drawing.Point(6, 73);
			this.lbColorPresets.Name = "lbColorPresets";
			this.lbColorPresets.Size = new System.Drawing.Size(133, 173);
			this.lbColorPresets.TabIndex = 4;
			this.lbColorPresets.SelectedIndexChanged += new System.EventHandler(this.lbColorPresets_SelectedIndexChanged);
			// 
			// tbNewPresetName
			// 
			this.tbNewPresetName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.tbNewPresetName.ForeColor = System.Drawing.Color.DarkRed;
			this.tbNewPresetName.Location = new System.Drawing.Point(6, 21);
			this.tbNewPresetName.Name = "tbNewPresetName";
			this.tbNewPresetName.Size = new System.Drawing.Size(133, 20);
			this.tbNewPresetName.TabIndex = 3;
			this.tbNewPresetName.Text = "Enter preset name";
			this.tbNewPresetName.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.tbNewPresetName_PreviewKeyDown);
			this.tbNewPresetName.Click += new System.EventHandler(this.tbNewPresetName_Click);
			// 
			// bAddPreset
			// 
			this.bAddPreset.Image = global::CodeImp.DoomBuilder.Properties.Resources.Add;
			this.bAddPreset.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.bAddPreset.Location = new System.Drawing.Point(6, 44);
			this.bAddPreset.Name = "bAddPreset";
			this.bAddPreset.Size = new System.Drawing.Size(47, 23);
			this.bAddPreset.TabIndex = 1;
			this.bAddPreset.Text = "Add";
			this.bAddPreset.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.bAddPreset.UseVisualStyleBackColor = true;
			this.bAddPreset.Click += new System.EventHandler(this.bAddPreset_Click);
			// 
			// colorProperties
			// 
			this.colorProperties.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.colorProperties.Enabled = false;
			this.colorProperties.Location = new System.Drawing.Point(154, 3);
			this.colorProperties.Name = "colorProperties";
			this.colorProperties.Size = new System.Drawing.Size(270, 250);
			this.colorProperties.TabIndex = 3;
			// 
			// CustomLinedefColorsControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.Controls.Add(this.colorProperties);
			this.Controls.Add(this.gbPresets);
			this.Name = "CustomLinedefColorsControl";
			this.Size = new System.Drawing.Size(427, 276);
			this.gbPresets.ResumeLayout(false);
			this.gbPresets.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.GroupBox gbPresets;
		private System.Windows.Forms.Button bRemovePreset;
		private IconListBox lbColorPresets;
		private System.Windows.Forms.TextBox tbNewPresetName;
		private System.Windows.Forms.Button bAddPreset;
		private System.Windows.Forms.Button bMoveDown;
		private System.Windows.Forms.Button bMoveUp;
		private CustomLinedefColorProperties colorProperties;
	}
}
