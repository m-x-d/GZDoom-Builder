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
			this.toolStrip1 = new System.Windows.Forms.ToolStrip();
			this.continuousdrawing = new System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
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
            this.continuousdrawing,
            this.toolStripSeparator1,
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
			// continuousdrawing
			// 
			this.continuousdrawing.CheckOnClick = true;
			this.continuousdrawing.Image = global::CodeImp.DoomBuilder.BuilderModes.Properties.Resources.Repeat;
			this.continuousdrawing.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.continuousdrawing.Name = "continuousdrawing";
			this.continuousdrawing.Size = new System.Drawing.Size(135, 22);
			this.continuousdrawing.Text = "Continuous drawing";
			this.continuousdrawing.CheckedChanged += new System.EventHandler(this.continuousdrawing_CheckedChanged);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
			// 
			// subdivslabel
			// 
			this.subdivslabel.Name = "subdivslabel";
			this.subdivslabel.Size = new System.Drawing.Size(37, 22);
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
			this.spikiness.Size = new System.Drawing.Size(56, 23);
			this.spikiness.Text = "0";
			this.spikiness.Value = new decimal(new int[] {
            0,
            0,
            0,
            0});
			// 
			// reset
			// 
			this.reset.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.reset.Image = global::CodeImp.DoomBuilder.BuilderModes.Properties.Resources.Reset;
			this.reset.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.reset.Name = "reset";
			this.reset.Size = new System.Drawing.Size(23, 20);
			this.reset.Text = "Reset";
			this.reset.Click += new System.EventHandler(this.reset_Click);
			// 
			// DrawEllipseOptionsPanel
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.Controls.Add(this.toolStrip1);
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
		private System.Windows.Forms.ToolStripButton continuousdrawing;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
	}
}
