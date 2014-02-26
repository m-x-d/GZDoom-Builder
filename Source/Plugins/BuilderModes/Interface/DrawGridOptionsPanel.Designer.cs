namespace CodeImp.DoomBuilder.BuilderModes
{
	partial class DrawGridOptionsPanel
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
			this.gridlock = new System.Windows.Forms.CheckBox();
			this.slicesV = new System.Windows.Forms.NumericUpDown();
			this.slicesH = new System.Windows.Forms.NumericUpDown();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.triangulate = new System.Windows.Forms.CheckBox();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.hints = new System.Windows.Forms.RichTextBox();
			this.groupBox1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.slicesV)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.slicesH)).BeginInit();
			this.groupBox2.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBox1
			// 
			this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox1.Controls.Add(this.gridlock);
			this.groupBox1.Controls.Add(this.slicesV);
			this.groupBox1.Controls.Add(this.slicesH);
			this.groupBox1.Controls.Add(this.label2);
			this.groupBox1.Controls.Add(this.label1);
			this.groupBox1.Controls.Add(this.triangulate);
			this.groupBox1.Location = new System.Drawing.Point(3, 3);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(243, 125);
			this.groupBox1.TabIndex = 1;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Draw Options:";
			// 
			// gridlock
			// 
			this.gridlock.AutoSize = true;
			this.gridlock.Location = new System.Drawing.Point(9, 77);
			this.gridlock.Name = "gridlock";
			this.gridlock.Size = new System.Drawing.Size(113, 18);
			this.gridlock.TabIndex = 5;
			this.gridlock.Text = "Lock slices to grid";
			this.gridlock.UseVisualStyleBackColor = true;
			this.gridlock.CheckedChanged += new System.EventHandler(this.gridlock_CheckedChanged);
			// 
			// slicesV
			// 
			this.slicesV.Location = new System.Drawing.Point(102, 50);
			this.slicesV.Maximum = new decimal(new int[] {
            128,
            0,
            0,
            0});
			this.slicesV.Name = "slicesV";
			this.slicesV.Size = new System.Drawing.Size(72, 20);
			this.slicesV.TabIndex = 4;
			this.slicesV.ValueChanged += new System.EventHandler(this.ValueChanged);
			// 
			// slicesH
			// 
			this.slicesH.Location = new System.Drawing.Point(102, 23);
			this.slicesH.Maximum = new decimal(new int[] {
            128,
            0,
            0,
            0});
			this.slicesH.Name = "slicesH";
			this.slicesH.Size = new System.Drawing.Size(72, 20);
			this.slicesH.TabIndex = 3;
			this.slicesH.ValueChanged += new System.EventHandler(this.ValueChanged);
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(6, 52);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(90, 14);
			this.label2.TabIndex = 2;
			this.label2.Text = "Vertical Slices:";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(6, 26);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(90, 13);
			this.label1.TabIndex = 1;
			this.label1.Text = "Horizontal Slices:";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// triangulate
			// 
			this.triangulate.AutoSize = true;
			this.triangulate.Location = new System.Drawing.Point(9, 101);
			this.triangulate.Name = "triangulate";
			this.triangulate.Size = new System.Drawing.Size(79, 18);
			this.triangulate.TabIndex = 0;
			this.triangulate.Text = "Triangulate";
			this.triangulate.UseVisualStyleBackColor = true;
			this.triangulate.CheckedChanged += new System.EventHandler(this.ValueChanged);
			// 
			// groupBox2
			// 
			this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox2.Controls.Add(this.hints);
			this.groupBox2.Location = new System.Drawing.Point(3, 134);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(243, 150);
			this.groupBox2.TabIndex = 2;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Quick Help:";
			// 
			// hints
			// 
			this.hints.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.hints.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.hints.Location = new System.Drawing.Point(9, 19);
			this.hints.Name = "hints";
			this.hints.ReadOnly = true;
			this.hints.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.hints.ShortcutsEnabled = false;
			this.hints.Size = new System.Drawing.Size(228, 146);
			this.hints.TabIndex = 0;
			this.hints.Text = "";
			// 
			// DrawGridOptionsPanel
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.groupBox1);
			this.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.Name = "DrawGridOptionsPanel";
			this.Size = new System.Drawing.Size(249, 330);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.slicesV)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.slicesH)).EndInit();
			this.groupBox2.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.CheckBox triangulate;
		private System.Windows.Forms.NumericUpDown slicesV;
		private System.Windows.Forms.NumericUpDown slicesH;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.RichTextBox hints;
		private System.Windows.Forms.CheckBox gridlock;
	}
}
