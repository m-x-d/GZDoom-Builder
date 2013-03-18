namespace CodeImp.DoomBuilder.Windows
{
	partial class SetCurrentTexturesForm
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
			this.labelWalls = new System.Windows.Forms.Label();
			this.labelFloor = new System.Windows.Forms.Label();
			this.labelCeiling = new System.Windows.Forms.Label();
			this.bApply = new System.Windows.Forms.Button();
			this.bCancel = new System.Windows.Forms.Button();
			this.cbForceDefault = new System.Windows.Forms.CheckBox();
			this.walls = new CodeImp.DoomBuilder.Controls.TextureSelectorControl();
			this.floor = new CodeImp.DoomBuilder.Controls.TextureSelectorControl();
			this.ceiling = new CodeImp.DoomBuilder.Controls.TextureSelectorControl();
			this.SuspendLayout();
			// 
			// labelWalls
			// 
			this.labelWalls.Location = new System.Drawing.Point(194, 8);
			this.labelWalls.Name = "labelWalls";
			this.labelWalls.Size = new System.Drawing.Size(83, 16);
			this.labelWalls.TabIndex = 11;
			this.labelWalls.Text = "Walls";
			this.labelWalls.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// labelFloor
			// 
			this.labelFloor.Location = new System.Drawing.Point(103, 8);
			this.labelFloor.Name = "labelFloor";
			this.labelFloor.Size = new System.Drawing.Size(83, 16);
			this.labelFloor.TabIndex = 8;
			this.labelFloor.Text = "Floor";
			this.labelFloor.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// labelCeiling
			// 
			this.labelCeiling.Location = new System.Drawing.Point(12, 8);
			this.labelCeiling.Name = "labelCeiling";
			this.labelCeiling.Size = new System.Drawing.Size(83, 16);
			this.labelCeiling.TabIndex = 7;
			this.labelCeiling.Text = "Ceiling";
			this.labelCeiling.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// bApply
			// 
			this.bApply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.bApply.Location = new System.Drawing.Point(103, 185);
			this.bApply.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this.bApply.Name = "bApply";
			this.bApply.Size = new System.Drawing.Size(174, 26);
			this.bApply.TabIndex = 0;
			this.bApply.Text = "OK";
			this.bApply.UseVisualStyleBackColor = true;
			this.bApply.Click += new System.EventHandler(this.bApply_Click);
			// 
			// bCancel
			// 
			this.bCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.bCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.bCancel.Location = new System.Drawing.Point(12, 185);
			this.bCancel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this.bCancel.Name = "bCancel";
			this.bCancel.Size = new System.Drawing.Size(83, 26);
			this.bCancel.TabIndex = 1;
			this.bCancel.Text = "Cancel";
			this.bCancel.UseVisualStyleBackColor = true;
			this.bCancel.Click += new System.EventHandler(this.bCancel_Click);
			// 
			// cbForceDefault
			// 
			this.cbForceDefault.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.cbForceDefault.AutoSize = true;
			this.cbForceDefault.Location = new System.Drawing.Point(13, 147);
			this.cbForceDefault.Name = "cbForceDefault";
			this.cbForceDefault.Size = new System.Drawing.Size(252, 32);
			this.cbForceDefault.TabIndex = 13;
			this.cbForceDefault.Text = "Use these textures instead of textures from \r\nneighbouring geometry when creating" +
				" a sector\r\n";
			this.cbForceDefault.UseVisualStyleBackColor = true;
			this.cbForceDefault.CheckedChanged += new System.EventHandler(this.cbForceDefault_CheckedChanged);
			// 
			// walls
			// 
			this.walls.Location = new System.Drawing.Point(194, 27);
			this.walls.Name = "walls";
			this.walls.Required = false;
			this.walls.Size = new System.Drawing.Size(83, 112);
			this.walls.TabIndex = 12;
			this.walls.TextureName = "";
			// 
			// floor
			// 
			this.floor.Location = new System.Drawing.Point(103, 27);
			this.floor.Name = "floor";
			this.floor.Required = false;
			this.floor.Size = new System.Drawing.Size(83, 112);
			this.floor.TabIndex = 10;
			this.floor.TextureName = "";
			// 
			// ceiling
			// 
			this.ceiling.Location = new System.Drawing.Point(12, 27);
			this.ceiling.Name = "ceiling";
			this.ceiling.Required = false;
			this.ceiling.Size = new System.Drawing.Size(83, 112);
			this.ceiling.TabIndex = 9;
			this.ceiling.TextureName = "";
			// 
			// SetCurrentTexturesForm
			// 
			this.AcceptButton = this.bApply;
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.CancelButton = this.bCancel;
			this.ClientSize = new System.Drawing.Size(289, 216);
			this.Controls.Add(this.cbForceDefault);
			this.Controls.Add(this.walls);
			this.Controls.Add(this.floor);
			this.Controls.Add(this.ceiling);
			this.Controls.Add(this.labelWalls);
			this.Controls.Add(this.labelFloor);
			this.Controls.Add(this.labelCeiling);
			this.Controls.Add(this.bCancel);
			this.Controls.Add(this.bApply);
			this.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
			this.Name = "SetCurrentTexturesForm";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
			this.Text = "Current textures:";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button bApply;
		private System.Windows.Forms.Button bCancel;
		private CodeImp.DoomBuilder.Controls.TextureSelectorControl walls;
		private CodeImp.DoomBuilder.Controls.TextureSelectorControl floor;
		private CodeImp.DoomBuilder.Controls.TextureSelectorControl ceiling;
		private System.Windows.Forms.CheckBox cbForceDefault;
		private System.Windows.Forms.Label labelWalls;
		private System.Windows.Forms.Label labelFloor;
		private System.Windows.Forms.Label labelCeiling;
	}
}