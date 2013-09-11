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
			this.menuStrip.SuspendLayout();
			this.SuspendLayout();
			// 
			// menuStrip
			// 
			this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this.transformToolStripMenuItem});
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
			this.jitterItem.Size = new System.Drawing.Size(152, 22);
			this.jitterItem.Tag = "applyjitter";
			this.jitterItem.Text = "&Jitter...";
			this.jitterItem.Click += new System.EventHandler(this.InvokeTaggedAction);
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
	}
}