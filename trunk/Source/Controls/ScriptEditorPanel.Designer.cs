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
			this.openfile = new System.Windows.Forms.OpenFileDialog();
			this.savefile = new System.Windows.Forms.SaveFileDialog();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.buttonnew = new System.Windows.Forms.ToolStripDropDownButton();
			this.buttonopen = new System.Windows.Forms.ToolStripButton();
			this.buttonsave = new System.Windows.Forms.ToolStripButton();
			this.buttonsaveall = new System.Windows.Forms.ToolStripButton();
			this.buttonscriptconfig = new System.Windows.Forms.ToolStripButton();
			this.buttoncompile = new System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			this.buttonundo = new System.Windows.Forms.ToolStripButton();
			this.buttonredo = new System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
			this.buttoncut = new System.Windows.Forms.ToolStripButton();
			this.buttoncopy = new System.Windows.Forms.ToolStripButton();
			this.buttonpaste = new System.Windows.Forms.ToolStripButton();
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
			this.toolbar.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			this.toolbar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.buttonnew,
            this.buttonopen,
            this.buttonsave,
            this.buttonsaveall,
            this.toolStripSeparator1,
            this.buttonundo,
            this.buttonredo,
            this.toolStripSeparator2,
            this.buttoncut,
            this.buttoncopy,
            this.buttonpaste,
            this.toolStripSeparator3,
            this.buttonscriptconfig,
            this.buttoncompile});
			this.toolbar.Location = new System.Drawing.Point(0, 0);
			this.toolbar.Name = "toolbar";
			this.toolbar.Size = new System.Drawing.Size(697, 25);
			this.toolbar.TabIndex = 1;
			// 
			// openfile
			// 
			this.openfile.Title = "Open Script";
			// 
			// savefile
			// 
			this.savefile.Title = "Save Script As";
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
			// 
			// buttonnew
			// 
			this.buttonnew.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.buttonnew.Image = global::CodeImp.DoomBuilder.Properties.Resources.NewScript;
			this.buttonnew.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this.buttonnew.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.buttonnew.Margin = new System.Windows.Forms.Padding(6, 1, 0, 2);
			this.buttonnew.Name = "buttonnew";
			this.buttonnew.Size = new System.Drawing.Size(29, 22);
			this.buttonnew.Text = "New File";
			// 
			// buttonopen
			// 
			this.buttonopen.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.buttonopen.Image = global::CodeImp.DoomBuilder.Properties.Resources.OpenScript;
			this.buttonopen.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this.buttonopen.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.buttonopen.Margin = new System.Windows.Forms.Padding(3, 1, 0, 2);
			this.buttonopen.Name = "buttonopen";
			this.buttonopen.Size = new System.Drawing.Size(23, 22);
			this.buttonopen.Text = "Open File";
			this.buttonopen.Click += new System.EventHandler(this.buttonopen_Click);
			// 
			// buttonsave
			// 
			this.buttonsave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.buttonsave.Enabled = false;
			this.buttonsave.Image = global::CodeImp.DoomBuilder.Properties.Resources.SaveScript;
			this.buttonsave.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.buttonsave.Name = "buttonsave";
			this.buttonsave.Size = new System.Drawing.Size(23, 22);
			this.buttonsave.Text = "Save File";
			// 
			// buttonsaveall
			// 
			this.buttonsaveall.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.buttonsaveall.Enabled = false;
			this.buttonsaveall.Image = global::CodeImp.DoomBuilder.Properties.Resources.SaveAll;
			this.buttonsaveall.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.buttonsaveall.Name = "buttonsaveall";
			this.buttonsaveall.Size = new System.Drawing.Size(23, 22);
			this.buttonsaveall.Text = "Save All Files";
			// 
			// buttonscriptconfig
			// 
			this.buttonscriptconfig.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.buttonscriptconfig.Enabled = false;
			this.buttonscriptconfig.Image = global::CodeImp.DoomBuilder.Properties.Resources.ScriptPalette;
			this.buttonscriptconfig.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.buttonscriptconfig.Name = "buttonscriptconfig";
			this.buttonscriptconfig.Size = new System.Drawing.Size(23, 22);
			this.buttonscriptconfig.Text = "Change Script Type";
			// 
			// buttoncompile
			// 
			this.buttoncompile.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.buttoncompile.Image = global::CodeImp.DoomBuilder.Properties.Resources.ScriptCompile;
			this.buttoncompile.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.buttoncompile.Name = "buttoncompile";
			this.buttoncompile.Size = new System.Drawing.Size(23, 22);
			this.buttoncompile.Text = "Compile Script";
			// 
			// toolStripSeparator2
			// 
			this.toolStripSeparator2.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
			this.toolStripSeparator2.Name = "toolStripSeparator2";
			this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
			// 
			// buttonundo
			// 
			this.buttonundo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.buttonundo.Image = global::CodeImp.DoomBuilder.Properties.Resources.Undo;
			this.buttonundo.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.buttonundo.Name = "buttonundo";
			this.buttonundo.Size = new System.Drawing.Size(23, 22);
			this.buttonundo.Text = "Undo";
			// 
			// buttonredo
			// 
			this.buttonredo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.buttonredo.Image = global::CodeImp.DoomBuilder.Properties.Resources.Redo;
			this.buttonredo.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.buttonredo.Name = "buttonredo";
			this.buttonredo.Size = new System.Drawing.Size(23, 22);
			this.buttonredo.Text = "Redo";
			// 
			// toolStripSeparator3
			// 
			this.toolStripSeparator3.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
			this.toolStripSeparator3.Name = "toolStripSeparator3";
			this.toolStripSeparator3.Size = new System.Drawing.Size(6, 25);
			// 
			// buttoncut
			// 
			this.buttoncut.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.buttoncut.Image = global::CodeImp.DoomBuilder.Properties.Resources.Cut;
			this.buttoncut.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.buttoncut.Name = "buttoncut";
			this.buttoncut.Size = new System.Drawing.Size(23, 22);
			this.buttoncut.Text = "Cut Selection";
			// 
			// buttoncopy
			// 
			this.buttoncopy.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.buttoncopy.Image = global::CodeImp.DoomBuilder.Properties.Resources.Copy;
			this.buttoncopy.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.buttoncopy.Name = "buttoncopy";
			this.buttoncopy.Size = new System.Drawing.Size(23, 22);
			this.buttoncopy.Text = "Copy Selection";
			// 
			// buttonpaste
			// 
			this.buttonpaste.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.buttonpaste.Image = global::CodeImp.DoomBuilder.Properties.Resources.Paste;
			this.buttonpaste.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.buttonpaste.Name = "buttonpaste";
			this.buttonpaste.Size = new System.Drawing.Size(23, 22);
			this.buttonpaste.Text = "Paste";
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
		private System.Windows.Forms.ToolStripButton buttonsave;
		private System.Windows.Forms.ToolStripButton buttonsaveall;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStripButton buttonscriptconfig;
		private System.Windows.Forms.ToolStripButton buttoncompile;
		private System.Windows.Forms.ToolStripButton buttonundo;
		private System.Windows.Forms.ToolStripButton buttonredo;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
		private System.Windows.Forms.ToolStripButton buttoncut;
		private System.Windows.Forms.ToolStripButton buttoncopy;
		private System.Windows.Forms.ToolStripButton buttonpaste;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
	}
}
