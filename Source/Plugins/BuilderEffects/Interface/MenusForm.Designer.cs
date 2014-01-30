namespace CodeImp.DoomBuilder.BuilderEffects
{
	partial class MenusForm
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
			this.menuStrip = new System.Windows.Forms.MenuStrip();
			this.transformToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.jitterItem = new System.Windows.Forms.ToolStripMenuItem();
			this.createStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
			this.menuStrip.SuspendLayout();
			this.SuspendLayout();
			// 
			// menuStrip
			// 
			this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.transformToolStripMenuItem,
            this.createStripMenuItem});
			this.menuStrip.Location = new System.Drawing.Point(0, 0);
			this.menuStrip.Name = "menuStrip";
			this.menuStrip.Size = new System.Drawing.Size(452, 24);
			this.menuStrip.TabIndex = 0;
			this.menuStrip.Text = "menuStrip1";
			// 
			// transformToolStripMenuItem
			// 
			this.transformToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.jitterItem});
			this.transformToolStripMenuItem.Name = "transformToolStripMenuItem";
			this.transformToolStripMenuItem.Size = new System.Drawing.Size(74, 20);
			this.transformToolStripMenuItem.Text = "Transform";
			// 
			// jitterItem
			// 
			this.jitterItem.Image = global::CodeImp.DoomBuilder.BuilderEffects.Properties.Resources.Jitter;
			this.jitterItem.Name = "jitterItem";
			this.jitterItem.Size = new System.Drawing.Size(108, 22);
			this.jitterItem.Tag = "applyjitter";
			this.jitterItem.Text = "&Jitter...";
			this.jitterItem.Click += new System.EventHandler(this.InvokeTaggedAction);
			// 
			// createStripMenuItem
			// 
			this.createStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1});
			this.createStripMenuItem.Name = "createStripMenuItem";
			this.createStripMenuItem.Size = new System.Drawing.Size(53, 20);
			this.createStripMenuItem.Text = "Create";
			// 
			// toolStripMenuItem1
			// 
			this.toolStripMenuItem1.Image = global::CodeImp.DoomBuilder.BuilderEffects.Properties.Resources.Terrain;
			this.toolStripMenuItem1.Name = "toolStripMenuItem1";
			this.toolStripMenuItem1.Size = new System.Drawing.Size(230, 22);
			this.toolStripMenuItem1.Tag = "importobjasterrain";
			this.toolStripMenuItem1.Text = "Terrain from Wavefront .obj...";
			this.toolStripMenuItem1.Click += new System.EventHandler(this.InvokeTaggedAction);
			// 
			// MenusForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(452, 129);
			this.Controls.Add(this.menuStrip);
			this.MainMenuStrip = this.menuStrip;
			this.Name = "MenusForm";
			this.Text = "MenusForm";
			this.menuStrip.ResumeLayout(false);
			this.menuStrip.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.MenuStrip menuStrip;
		private System.Windows.Forms.ToolStripMenuItem transformToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem jitterItem;
		private System.Windows.Forms.ToolStripMenuItem createStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
	}
}