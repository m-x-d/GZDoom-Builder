namespace CodeImp.DoomBuilder.ZDoomUSDF
{
	partial class ToolsForm
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.toolstrip = new System.Windows.Forms.ToolStrip();
			this.dialogbutton = new System.Windows.Forms.ToolStripButton();
			this.toolstrip.SuspendLayout();
			this.SuspendLayout();
			// 
			// toolstrip
			// 
			this.toolstrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.dialogbutton});
			this.toolstrip.Location = new System.Drawing.Point(0, 0);
			this.toolstrip.Name = "toolstrip";
			this.toolstrip.Size = new System.Drawing.Size(196, 25);
			this.toolstrip.TabIndex = 0;
			this.toolstrip.Text = "toolStrip1";
			// 
			// dialogbutton
			// 
			this.dialogbutton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.dialogbutton.Image = global::CodeImp.DoomBuilder.ZDoomUSDF.Properties.Resources.Dialog;
			this.dialogbutton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.dialogbutton.Name = "dialogbutton";
			this.dialogbutton.Size = new System.Drawing.Size(23, 22);
			this.dialogbutton.Tag = "openconversationeditor";
			this.dialogbutton.Text = "Open Conversation Editor";
			this.dialogbutton.Click += new System.EventHandler(this.InvokeTaggedAction);
			// 
			// ToolsForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(196, 78);
			this.Controls.Add(this.toolstrip);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ToolsForm";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.Text = "ToolsForm";
			this.toolstrip.ResumeLayout(false);
			this.toolstrip.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ToolStrip toolstrip;
		private System.Windows.Forms.ToolStripButton dialogbutton;
	}
}