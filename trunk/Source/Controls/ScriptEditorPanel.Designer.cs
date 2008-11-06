namespace CodeImp.DoomBuilder.Controls
{
	partial class ScriptEditorPanel
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
			this.tabs = new System.Windows.Forms.TabControl();
			this.toolbar = new System.Windows.Forms.ToolStrip();
			this.buttonnew = new System.Windows.Forms.ToolStripDropDownButton();
			this.buttonopen = new System.Windows.Forms.ToolStripButton();
			this.openfile = new System.Windows.Forms.OpenFileDialog();
			this.savefile = new System.Windows.Forms.SaveFileDialog();
			this.toolbar.SuspendLayout();
			this.SuspendLayout();
			// 
			// tabs
			// 
			this.tabs.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.tabs.Location = new System.Drawing.Point(3, 33);
			this.tabs.Margin = new System.Windows.Forms.Padding(3, 8, 3, 3);
			this.tabs.Name = "tabs";
			this.tabs.Padding = new System.Drawing.Point(12, 3);
			this.tabs.SelectedIndex = 0;
			this.tabs.Size = new System.Drawing.Size(691, 435);
			this.tabs.TabIndex = 0;
			// 
			// toolbar
			// 
			this.toolbar.AllowMerge = false;
			this.toolbar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.buttonnew,
            this.buttonopen});
			this.toolbar.Location = new System.Drawing.Point(0, 0);
			this.toolbar.Name = "toolbar";
			this.toolbar.Size = new System.Drawing.Size(697, 25);
			this.toolbar.TabIndex = 1;
			// 
			// buttonnew
			// 
			this.buttonnew.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.buttonnew.Image = global::CodeImp.DoomBuilder.Properties.Resources.NewMap;
			this.buttonnew.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this.buttonnew.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.buttonnew.Name = "buttonnew";
			this.buttonnew.Size = new System.Drawing.Size(29, 22);
			this.buttonnew.Text = "New File";
			// 
			// buttonopen
			// 
			this.buttonopen.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.buttonopen.Image = global::CodeImp.DoomBuilder.Properties.Resources.OpenMap;
			this.buttonopen.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this.buttonopen.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.buttonopen.Margin = new System.Windows.Forms.Padding(3, 1, 0, 2);
			this.buttonopen.Name = "buttonopen";
			this.buttonopen.Size = new System.Drawing.Size(23, 22);
			this.buttonopen.Text = "Open File";
			this.buttonopen.Click += new System.EventHandler(this.buttonopen_Click);
			// 
			// openfile
			// 
			this.openfile.Title = "Open Script";
			// 
			// savefile
			// 
			this.savefile.Title = "Save Script As";
			// 
			// ScriptEditorPanel
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.BackColor = System.Drawing.SystemColors.Control;
			this.Controls.Add(this.toolbar);
			this.Controls.Add(this.tabs);
			this.Name = "ScriptEditorPanel";
			this.Size = new System.Drawing.Size(697, 471);
			this.toolbar.ResumeLayout(false);
			this.toolbar.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TabControl tabs;
		private System.Windows.Forms.ToolStrip toolbar;
		private System.Windows.Forms.ToolStripButton buttonopen;
		private System.Windows.Forms.ToolStripDropDownButton buttonnew;
		private System.Windows.Forms.OpenFileDialog openfile;
		private System.Windows.Forms.SaveFileDialog savefile;
	}
}
