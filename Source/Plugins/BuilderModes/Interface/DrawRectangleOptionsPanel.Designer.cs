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
			this.radiuslabel = new System.Windows.Forms.ToolStripLabel();
			this.radius = new CodeImp.DoomBuilder.Controls.ToolStripNumericUpDown();
			this.subdivslabel = new System.Windows.Forms.ToolStripLabel();
			this.subdivs = new CodeImp.DoomBuilder.Controls.ToolStripNumericUpDown();
			this.reset = new System.Windows.Forms.ToolStripButton();
			this.toolStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// toolStrip1
			// 
			this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.radiuslabel,
            this.radius,
            this.subdivslabel,
            this.subdivs,
            this.reset});
			this.toolStrip1.Location = new System.Drawing.Point(0, 0);
			this.toolStrip1.Name = "toolStrip1";
			this.toolStrip1.Size = new System.Drawing.Size(380, 25);
			this.toolStrip1.TabIndex = 7;
			this.toolStrip1.Text = "toolStrip1";
			// 
			// radiuslabel
			// 
			this.radiuslabel.Image = global::CodeImp.DoomBuilder.BuilderModes.Properties.Resources.Gear;
			this.radiuslabel.Name = "radiuslabel";
			this.radiuslabel.Size = new System.Drawing.Size(92, 22);
			this.radiuslabel.Text = "Bevel Radius:";
			// 
			// radius
			// 
			this.radius.AutoSize = false;
			this.radius.Margin = new System.Windows.Forms.Padding(3, 0, 6, 0);
			this.radius.Maximum = new decimal(new int[] {
            2048,
            0,
            0,
            0});
			this.radius.Minimum = new decimal(new int[] {
            0,
            0,
            0,
            0});
			this.radius.Name = "radius";
			this.radius.Size = new System.Drawing.Size(56, 20);
			this.radius.Text = "0";
			this.radius.Value = new decimal(new int[] {
            0,
            0,
            0,
            0});
			this.radius.ValueChanged += new System.EventHandler(this.ValueChanged);
			// 
			// subdivslabel
			// 
			this.subdivslabel.Name = "subdivslabel";
			this.subdivslabel.Size = new System.Drawing.Size(76, 22);
			this.subdivslabel.Text = "Subdivisions:";
			// 
			// subdivs
			// 
			this.subdivs.AutoSize = false;
			this.subdivs.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
			this.subdivs.Maximum = new decimal(new int[] {
            128,
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
			// DrawRectangleOptionsPanel
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.Controls.Add(this.toolStrip1);
			this.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.Name = "DrawRectangleOptionsPanel";
			this.Size = new System.Drawing.Size(380, 60);
			this.toolStrip1.ResumeLayout(false);
			this.toolStrip1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ToolStrip toolStrip1;
		private System.Windows.Forms.ToolStripLabel radiuslabel;
		private CodeImp.DoomBuilder.Controls.ToolStripNumericUpDown radius;
		private System.Windows.Forms.ToolStripLabel subdivslabel;
		private CodeImp.DoomBuilder.Controls.ToolStripNumericUpDown subdivs;
		private System.Windows.Forms.ToolStripButton reset;
	}
}
