namespace CodeImp.DoomBuilder.Plugins.ChocoRenderLimits
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
		protected override void Dispose(bool disposing)
		{
			if(disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.menustrip = new System.Windows.Forms.MenuStrip();
			this.toolsmenu = new System.Windows.Forms.ToolStripMenuItem();
			this.settingsitem = new System.Windows.Forms.ToolStripMenuItem();
			this.processesitem = new System.Windows.Forms.ToolStripMenuItem();
			this.menustrip.SuspendLayout();
			this.SuspendLayout();
			// 
			// menustrip
			// 
			this.menustrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolsmenu});
			this.menustrip.Location = new System.Drawing.Point(0, 0);
			this.menustrip.Name = "menustrip";
			this.menustrip.Size = new System.Drawing.Size(362, 24);
			this.menustrip.TabIndex = 0;
			this.menustrip.Text = "menuStrip1";
			// 
			// toolsmenu
			// 
			this.toolsmenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.processesitem,
            this.settingsitem});
			this.toolsmenu.Name = "toolsmenu";
			this.toolsmenu.Size = new System.Drawing.Size(44, 20);
			this.toolsmenu.Text = "Tools";
			// 
			// settingsitem
			// 
			this.settingsitem.Name = "settingsitem";
			this.settingsitem.Size = new System.Drawing.Size(239, 22);
			this.settingsitem.Tag = "crl_settings";
			this.settingsitem.Text = "ChocoRenderLimits Settings...";
			this.settingsitem.Click += new System.EventHandler(this.settingsitem_Click);
			// 
			// processesitem
			// 
			this.processesitem.Image = global::CodeImp.DoomBuilder.Plugins.ChocoRenderLimits.Properties.Resources.CRL;
			this.processesitem.Name = "processesitem";
			this.processesitem.Size = new System.Drawing.Size(239, 22);
			this.processesitem.Tag = "crl_processes";
			this.processesitem.Text = "ChocoRenderLimits Processes...";
			this.processesitem.Click += new System.EventHandler(this.InvokeTaggedAction);
			// 
			// MenusForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.ClientSize = new System.Drawing.Size(362, 159);
			this.Controls.Add(this.menustrip);
			this.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MainMenuStrip = this.menustrip;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "MenusForm";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.Text = "MenusForm";
			this.menustrip.ResumeLayout(false);
			this.menustrip.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.MenuStrip menustrip;
		private System.Windows.Forms.ToolStripMenuItem toolsmenu;
		private System.Windows.Forms.ToolStripMenuItem processesitem;
		private System.Windows.Forms.ToolStripMenuItem settingsitem;
	}
}