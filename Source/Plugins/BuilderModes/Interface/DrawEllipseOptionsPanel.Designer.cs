namespace CodeImp.DoomBuilder.BuilderModes
{
	partial class DrawEllipseOptionsPanel
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
			this.toolStrip1 = new System.Windows.Forms.ToolStrip();
			this.subdivslabel = new System.Windows.Forms.ToolStripLabel();
			this.subdivs = new CodeImp.DoomBuilder.Controls.ToolStripNumericUpDown();
			this.spikinesslabel = new System.Windows.Forms.ToolStripLabel();
			this.spikiness = new CodeImp.DoomBuilder.Controls.ToolStripNumericUpDown();
			this.reset = new System.Windows.Forms.ToolStripButton();
			this.toolStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// toolStrip1
			// 
			this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.subdivslabel,
            this.subdivs,
            this.spikinesslabel,
            this.spikiness,
            this.reset});
			this.toolStrip1.Location = new System.Drawing.Point(0, 0);
			this.toolStrip1.Name = "toolStrip1";
			this.toolStrip1.Size = new System.Drawing.Size(348, 25);
			this.toolStrip1.TabIndex = 6;
			this.toolStrip1.Text = "toolStrip1";
			// 
			// subdivslabel
			// 
			this.subdivslabel.Image = global::CodeImp.DoomBuilder.BuilderModes.Properties.Resources.Gear;
			this.subdivslabel.Name = "subdivslabel";
			this.subdivslabel.Size = new System.Drawing.Size(53, 22);
			this.subdivslabel.Text = "Sides:";
			// 
			// subdivs
			// 
			this.subdivs.AutoSize = false;
			this.subdivs.Margin = new System.Windows.Forms.Padding(3, 0, 6, 0);
			this.subdivs.Maximum = new decimal(new int[] {
            100,
            0,
            0,
            0});
			this.subdivs.Minimum = new decimal(new int[] {
            0,
            0,
            0,
            0});
			this.subdivs.Name = "subdivs";
			this.subdivs.Size = new System.Drawing.Size(56, 20);
			this.subdivs.Text = "0";
			this.subdivs.Value = new decimal(new int[] {
            0,
            0,
            0,
            0});
			this.subdivs.ValueChanged += new System.EventHandler(this.ValueChanged);
			// 
			// spikinesslabel
			// 
			this.spikinesslabel.Name = "spikinesslabel";
			this.spikinesslabel.Size = new System.Drawing.Size(58, 22);
			this.spikinesslabel.Text = "Spikiness:";
			// 
			// spikiness
			// 
			this.spikiness.AutoSize = false;
			this.spikiness.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
			this.spikiness.Maximum = new decimal(new int[] {
            100,
            0,
            0,
            0});
			this.spikiness.Minimum = new decimal(new int[] {
            0,
            0,
            0,
            0});
			this.spikiness.Name = "spikiness";
			this.spikiness.Size = new System.Drawing.Size(56, 20);
			this.spikiness.Text = "0";
			this.spikiness.Value = new decimal(new int[] {
            0,
            0,
            0,
            0});
			this.spikiness.ValueChanged += new System.EventHandler(this.ValueChanged);
			// 
			// reset
			// 
			this.reset.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.reset.Image = global::CodeImp.DoomBuilder.BuilderModes.Properties.Resources.Reset;
			this.reset.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.reset.Name = "reset";
			this.reset.Size = new System.Drawing.Size(23, 22);
			this.reset.Text = "toolStripButton1";
			this.reset.Click += new System.EventHandler(this.reset_Click);
			// 
			// DrawEllipseOptionsPanel
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.Controls.Add(this.toolStrip1);
			this.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.Name = "DrawEllipseOptionsPanel";
			this.Size = new System.Drawing.Size(348, 60);
			this.toolStrip1.ResumeLayout(false);
			this.toolStrip1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ToolStrip toolStrip1;
		private System.Windows.Forms.ToolStripLabel subdivslabel;
		private CodeImp.DoomBuilder.Controls.ToolStripNumericUpDown subdivs;
		private System.Windows.Forms.ToolStripLabel spikinesslabel;
		private CodeImp.DoomBuilder.Controls.ToolStripNumericUpDown spikiness;
		private System.Windows.Forms.ToolStripButton reset;
	}
}
