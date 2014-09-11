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
			this.toolStrip1 = new System.Windows.Forms.ToolStrip();
			this.sliceshlabel = new System.Windows.Forms.ToolStripLabel();
			this.slicesH = new CodeImp.DoomBuilder.Controls.ToolStripNumericUpDown();
			this.slicesvlabel = new System.Windows.Forms.ToolStripLabel();
			this.slicesV = new CodeImp.DoomBuilder.Controls.ToolStripNumericUpDown();
			this.reset = new System.Windows.Forms.ToolStripButton();
			this.cbseparator = new System.Windows.Forms.ToolStripSeparator();
			this.gridlock = new CodeImp.DoomBuilder.Controls.ToolStripCheckBox();
			this.triangulate = new CodeImp.DoomBuilder.Controls.ToolStripCheckBox();
			this.toolStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// toolStrip1
			// 
			this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.sliceshlabel,
            this.slicesH,
            this.slicesvlabel,
            this.slicesV,
            this.reset,
            this.cbseparator,
            this.gridlock,
            this.triangulate});
			this.toolStrip1.Location = new System.Drawing.Point(0, 0);
			this.toolStrip1.Name = "toolStrip1";
			this.toolStrip1.Size = new System.Drawing.Size(619, 25);
			this.toolStrip1.TabIndex = 8;
			this.toolStrip1.Text = "toolStrip1";
			// 
			// sliceshlabel
			// 
			this.sliceshlabel.Image = global::CodeImp.DoomBuilder.BuilderModes.Properties.Resources.Gear;
			this.sliceshlabel.Name = "sliceshlabel";
			this.sliceshlabel.Size = new System.Drawing.Size(113, 22);
			this.sliceshlabel.Text = "Horizontal Slices:";
			// 
			// slicesH
			// 
			this.slicesH.AutoSize = false;
			this.slicesH.Margin = new System.Windows.Forms.Padding(3, 0, 6, 0);
			this.slicesH.Maximum = new decimal(new int[] {
            100,
            0,
            0,
            0});
			this.slicesH.Minimum = new decimal(new int[] {
            0,
            0,
            0,
            0});
			this.slicesH.Name = "slicesH";
			this.slicesH.Size = new System.Drawing.Size(56, 20);
			this.slicesH.Text = "0";
			this.slicesH.Value = new decimal(new int[] {
            0,
            0,
            0,
            0});
			this.slicesH.ValueChanged += new System.EventHandler(this.ValueChanged);
			// 
			// slicesvlabel
			// 
			this.slicesvlabel.Name = "slicesvlabel";
			this.slicesvlabel.Size = new System.Drawing.Size(81, 22);
			this.slicesvlabel.Text = "Vertical Slices:";
			// 
			// slicesV
			// 
			this.slicesV.AutoSize = false;
			this.slicesV.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
			this.slicesV.Maximum = new decimal(new int[] {
            100,
            0,
            0,
            0});
			this.slicesV.Minimum = new decimal(new int[] {
            0,
            0,
            0,
            0});
			this.slicesV.Name = "slicesV";
			this.slicesV.Size = new System.Drawing.Size(56, 20);
			this.slicesV.Text = "0";
			this.slicesV.Value = new decimal(new int[] {
            0,
            0,
            0,
            0});
			this.slicesV.ValueChanged += new System.EventHandler(this.ValueChanged);
			// 
			// reset
			// 
			this.reset.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.reset.Image = global::CodeImp.DoomBuilder.BuilderModes.Properties.Resources.Reset;
			this.reset.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.reset.Name = "reset";
			this.reset.Size = new System.Drawing.Size(23, 22);
			this.reset.Text = "Reset";
			// 
			// cbseparator
			// 
			this.cbseparator.Name = "cbseparator";
			this.cbseparator.Size = new System.Drawing.Size(6, 25);
			// 
			// gridlock
			// 
			this.gridlock.Checked = false;
			this.gridlock.Margin = new System.Windows.Forms.Padding(12, 1, 0, 2);
			this.gridlock.Name = "gridlock";
			this.gridlock.Size = new System.Drawing.Size(120, 22);
			this.gridlock.Text = "Lock slices to grid";
			this.gridlock.CheckedChanged += new System.EventHandler(this.gridlock_CheckedChanged);
			// 
			// triangulate
			// 
			this.triangulate.Checked = false;
			this.triangulate.Margin = new System.Windows.Forms.Padding(3, 1, 0, 2);
			this.triangulate.Name = "triangulate";
			this.triangulate.Size = new System.Drawing.Size(86, 22);
			this.triangulate.Text = "Triangulate";
			this.triangulate.CheckedChanged += new System.EventHandler(this.ValueChanged);
			// 
			// DrawGridOptionsPanel
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.Controls.Add(this.toolStrip1);
			this.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.Name = "DrawGridOptionsPanel";
			this.Size = new System.Drawing.Size(619, 60);
			this.toolStrip1.ResumeLayout(false);
			this.toolStrip1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ToolStrip toolStrip1;
		private System.Windows.Forms.ToolStripLabel sliceshlabel;
		private CodeImp.DoomBuilder.Controls.ToolStripNumericUpDown slicesH;
		private System.Windows.Forms.ToolStripLabel slicesvlabel;
		private CodeImp.DoomBuilder.Controls.ToolStripNumericUpDown slicesV;
		private System.Windows.Forms.ToolStripButton reset;
		private System.Windows.Forms.ToolStripSeparator cbseparator;
		private CodeImp.DoomBuilder.Controls.ToolStripCheckBox gridlock;
		private CodeImp.DoomBuilder.Controls.ToolStripCheckBox triangulate;
	}
}
