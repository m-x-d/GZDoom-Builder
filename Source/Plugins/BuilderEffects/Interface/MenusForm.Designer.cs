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
			this.importStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
			this.exportStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStrip = new System.Windows.Forms.ToolStrip();
			this.jitterButton = new System.Windows.Forms.ToolStripButton();
			this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
			this.jitterItem = new System.Windows.Forms.ToolStripMenuItem();
			this.menuStrip.SuspendLayout();
			this.toolStrip.SuspendLayout();
			this.SuspendLayout();
			// 
			// menuStrip
			// 
			this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.importStripMenuItem,
            this.exportStripMenuItem,
            this.toolStripMenuItem3});
			this.menuStrip.Location = new System.Drawing.Point(0, 0);
			this.menuStrip.Name = "menuStrip";
			this.menuStrip.Size = new System.Drawing.Size(452, 24);
			this.menuStrip.TabIndex = 0;
			this.menuStrip.Text = "menuStrip1";
			// 
			// importStripMenuItem
			// 
			this.importStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1});
			this.importStripMenuItem.Name = "importStripMenuItem";
			this.importStripMenuItem.Size = new System.Drawing.Size(55, 20);
			this.importStripMenuItem.Text = "Import";
			// 
			// toolStripMenuItem1
			// 
			this.toolStripMenuItem1.Image = global::CodeImp.DoomBuilder.BuilderEffects.Properties.Resources.Terrain;
			this.toolStripMenuItem1.Name = "toolStripMenuItem1";
			this.toolStripMenuItem1.Size = new System.Drawing.Size(215, 22);
			this.toolStripMenuItem1.Tag = "importobjasterrain";
			this.toolStripMenuItem1.Text = "Wavefront .obj as Terrain...";
			this.toolStripMenuItem1.Click += new System.EventHandler(this.InvokeTaggedAction);
			// 
			// exportStripMenuItem
			// 
			this.exportStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem2});
			this.exportStripMenuItem.Name = "exportStripMenuItem";
			this.exportStripMenuItem.Size = new System.Drawing.Size(52, 20);
			this.exportStripMenuItem.Text = "Export";
			// 
			// toolStripMenuItem2
			// 
			this.toolStripMenuItem2.Name = "toolStripMenuItem2";
			this.toolStripMenuItem2.Size = new System.Drawing.Size(229, 22);
			this.toolStripMenuItem2.Tag = "exporttoobj";
			this.toolStripMenuItem2.Text = "Selection To Wavefront .obj...";
			this.toolStripMenuItem2.Click += new System.EventHandler(this.InvokeTaggedAction);
			// 
			// toolStrip
			// 
			this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.jitterButton});
			this.toolStrip.Location = new System.Drawing.Point(0, 24);
			this.toolStrip.Name = "toolStrip";
			this.toolStrip.Size = new System.Drawing.Size(452, 25);
			this.toolStrip.TabIndex = 1;
			this.toolStrip.Text = "toolStrip1";
			// 
			// jitterButton
			// 
			this.jitterButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.jitterButton.Image = global::CodeImp.DoomBuilder.BuilderEffects.Properties.Resources.Jitter;
			this.jitterButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.jitterButton.Name = "jitterButton";
			this.jitterButton.Size = new System.Drawing.Size(23, 22);
			this.jitterButton.Tag = "applyjitter";
			this.jitterButton.Text = "Apply Jitter";
			this.jitterButton.Click += new System.EventHandler(this.InvokeTaggedAction);
			// 
			// toolStripMenuItem3
			// 
			this.toolStripMenuItem3.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.jitterItem});
			this.toolStripMenuItem3.Name = "toolStripMenuItem3";
			this.toolStripMenuItem3.Size = new System.Drawing.Size(55, 20);
			this.toolStripMenuItem3.Text = "Modes";
			// 
			// jitterItem
			// 
			this.jitterItem.Image = global::CodeImp.DoomBuilder.BuilderEffects.Properties.Resources.Jitter;
			this.jitterItem.Name = "jitterItem";
			this.jitterItem.Size = new System.Drawing.Size(152, 22);
			this.jitterItem.Tag = "applyjitter";
			this.jitterItem.Text = "Apply Jitter";
			this.jitterItem.Click += new System.EventHandler(this.InvokeTaggedAction);
			// 
			// MenusForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(452, 129);
			this.Controls.Add(this.toolStrip);
			this.Controls.Add(this.menuStrip);
			this.MainMenuStrip = this.menuStrip;
			this.Name = "MenusForm";
			this.Text = "MenusForm";
			this.menuStrip.ResumeLayout(false);
			this.menuStrip.PerformLayout();
			this.toolStrip.ResumeLayout(false);
			this.toolStrip.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.MenuStrip menuStrip;
		private System.Windows.Forms.ToolStripMenuItem importStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
		private System.Windows.Forms.ToolStripMenuItem exportStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
		private System.Windows.Forms.ToolStrip toolStrip;
		private System.Windows.Forms.ToolStripButton jitterButton;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem3;
		private System.Windows.Forms.ToolStripMenuItem jitterItem;
	}
}