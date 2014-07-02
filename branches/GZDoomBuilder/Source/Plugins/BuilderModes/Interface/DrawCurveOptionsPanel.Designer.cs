namespace CodeImp.DoomBuilder.BuilderModes
{
	partial class DrawCurveOptionsPanel
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
			this.toolstrip = new System.Windows.Forms.ToolStrip();
			this.seglabel = new System.Windows.Forms.ToolStripLabel();
			this.seglen = new CodeImp.DoomBuilder.Controls.ToolStripNumericUpDown();
			this.reset = new System.Windows.Forms.ToolStripButton();
			this.toolstrip.SuspendLayout();
			this.SuspendLayout();
			// 
			// toolstrip
			// 
			this.toolstrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.seglabel,
            this.seglen,
            this.reset});
			this.toolstrip.Location = new System.Drawing.Point(0, 0);
			this.toolstrip.Name = "toolstrip";
			this.toolstrip.Size = new System.Drawing.Size(249, 25);
			this.toolstrip.TabIndex = 7;
			this.toolstrip.Text = "toolStrip1";
			// 
			// seglabel
			// 
			this.seglabel.Image = global::CodeImp.DoomBuilder.BuilderModes.Properties.Resources.Gear;
			this.seglabel.Name = "seglabel";
			this.seglabel.Size = new System.Drawing.Size(113, 22);
			this.seglabel.Text = "Segment Length:";
			// 
			// seglen
			// 
			this.seglen.AutoSize = false;
			this.seglen.Margin = new System.Windows.Forms.Padding(3, 0, 0, 0);
			this.seglen.Maximum = new decimal(new int[] {
            100,
            0,
            0,
            0});
			this.seglen.Minimum = new decimal(new int[] {
            0,
            0,
            0,
            0});
			this.seglen.Name = "seglen";
			this.seglen.Size = new System.Drawing.Size(56, 20);
			this.seglen.Text = "0";
			this.seglen.Value = new decimal(new int[] {
            0,
            0,
            0,
            0});
			this.seglen.ValueChanged += new System.EventHandler(this.seglen_ValueChanged);
			// 
			// reset
			// 
			this.reset.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.reset.Image = global::CodeImp.DoomBuilder.BuilderModes.Properties.Resources.Reset;
			this.reset.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.reset.Name = "reset";
			this.reset.Size = new System.Drawing.Size(23, 22);
			this.reset.Text = "Reset";
			this.reset.Click += new System.EventHandler(this.reset_Click);
			// 
			// DrawCurveOptionsPanel
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.Controls.Add(this.toolstrip);
			this.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.Name = "DrawCurveOptionsPanel";
			this.Size = new System.Drawing.Size(249, 60);
			this.toolstrip.ResumeLayout(false);
			this.toolstrip.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ToolStripLabel seglabel;
		internal CodeImp.DoomBuilder.Controls.ToolStripNumericUpDown seglen;
		private System.Windows.Forms.ToolStrip toolstrip;
		private System.Windows.Forms.ToolStripButton reset;

	}
}
