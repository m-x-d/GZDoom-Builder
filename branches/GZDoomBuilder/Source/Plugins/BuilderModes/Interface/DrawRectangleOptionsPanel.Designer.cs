namespace CodeImp.DoomBuilder.BuilderModes
{
	partial class DrawRectangleOptionsPanel
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
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.hints = new System.Windows.Forms.RichTextBox();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.reset = new System.Windows.Forms.Button();
			this.subdivs = new System.Windows.Forms.NumericUpDown();
			this.radius = new System.Windows.Forms.NumericUpDown();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.groupBox2.SuspendLayout();
			this.groupBox1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.subdivs)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.radius)).BeginInit();
			this.SuspendLayout();
			// 
			// groupBox2
			// 
			this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox2.Controls.Add(this.hints);
			this.groupBox2.Location = new System.Drawing.Point(3, 89);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(243, 150);
			this.groupBox2.TabIndex = 4;
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
			this.hints.Size = new System.Drawing.Size(228, 125);
			this.hints.TabIndex = 0;
			this.hints.Text = "";
			// 
			// groupBox1
			// 
			this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox1.Controls.Add(this.reset);
			this.groupBox1.Controls.Add(this.subdivs);
			this.groupBox1.Controls.Add(this.radius);
			this.groupBox1.Controls.Add(this.label2);
			this.groupBox1.Controls.Add(this.label1);
			this.groupBox1.Location = new System.Drawing.Point(3, 3);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(243, 80);
			this.groupBox1.TabIndex = 3;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Draw Options:";
			// 
			// reset
			// 
			this.reset.Image = global::CodeImp.DoomBuilder.BuilderModes.Properties.Resources.Reset;
			this.reset.Location = new System.Drawing.Point(170, 22);
			this.reset.Name = "reset";
			this.reset.Size = new System.Drawing.Size(26, 49);
			this.reset.TabIndex = 5;
			this.reset.UseVisualStyleBackColor = true;
			this.reset.Click += new System.EventHandler(this.reset_Click);
			// 
			// subdivs
			// 
			this.subdivs.Location = new System.Drawing.Point(92, 50);
			this.subdivs.Maximum = new decimal(new int[] {
            128,
            0,
            0,
            0});
			this.subdivs.Name = "subdivs";
			this.subdivs.Size = new System.Drawing.Size(72, 20);
			this.subdivs.TabIndex = 4;
			// 
			// radius
			// 
			this.radius.Location = new System.Drawing.Point(92, 23);
			this.radius.Maximum = new decimal(new int[] {
            32767,
            0,
            0,
            0});
			this.radius.Minimum = new decimal(new int[] {
            32768,
            0,
            0,
            -2147483648});
			this.radius.Name = "radius";
			this.radius.Size = new System.Drawing.Size(72, 20);
			this.radius.TabIndex = 3;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(6, 52);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(80, 14);
			this.label2.TabIndex = 2;
			this.label2.Text = "Bevel Detail:";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(6, 26);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(80, 13);
			this.label1.TabIndex = 1;
			this.label1.Text = "Bevel Radius:";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// DrawRectangleOptionsPanel
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.groupBox1);
			this.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.Name = "DrawRectangleOptionsPanel";
			this.Size = new System.Drawing.Size(249, 330);
			this.groupBox2.ResumeLayout(false);
			this.groupBox1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.subdivs)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.radius)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.RichTextBox hints;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.NumericUpDown subdivs;
		private System.Windows.Forms.NumericUpDown radius;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button reset;
	}
}
