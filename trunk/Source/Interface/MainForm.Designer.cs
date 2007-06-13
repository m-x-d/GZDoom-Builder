namespace CodeImp.DoomBuilder.Interface
{
	partial class MainForm
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
			this.menumain = new System.Windows.Forms.MenuStrip();
			this.menufile = new System.Windows.Forms.ToolStripMenuItem();
			this.itemnewmap = new System.Windows.Forms.ToolStripMenuItem();
			this.itemopenmap = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
			this.itemsavemap = new System.Windows.Forms.ToolStripMenuItem();
			this.itemsavemapas = new System.Windows.Forms.ToolStripMenuItem();
			this.itemsavemapinto = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
			this.itemexit = new System.Windows.Forms.ToolStripMenuItem();
			this.toolbar = new System.Windows.Forms.ToolStrip();
			this.statusbar = new System.Windows.Forms.StatusStrip();
			this.panelinfo = new System.Windows.Forms.Panel();
			this.display = new System.Windows.Forms.PictureBox();
			this.menumain.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.display)).BeginInit();
			this.SuspendLayout();
			// 
			// menumain
			// 
			this.menumain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menufile});
			this.menumain.Location = new System.Drawing.Point(0, 0);
			this.menumain.Name = "menumain";
			this.menumain.Size = new System.Drawing.Size(619, 24);
			this.menumain.TabIndex = 0;
			this.menumain.Text = "menuStrip1";
			// 
			// menufile
			// 
			this.menufile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.itemnewmap,
            this.itemopenmap,
            this.toolStripMenuItem1,
            this.itemsavemap,
            this.itemsavemapas,
            this.itemsavemapinto,
            this.toolStripMenuItem2,
            this.itemexit});
			this.menufile.Name = "menufile";
			this.menufile.Size = new System.Drawing.Size(35, 20);
			this.menufile.Text = "File";
			// 
			// itemnewmap
			// 
			this.itemnewmap.Name = "itemnewmap";
			this.itemnewmap.Size = new System.Drawing.Size(167, 22);
			this.itemnewmap.Text = "New Map...";
			// 
			// itemopenmap
			// 
			this.itemopenmap.Name = "itemopenmap";
			this.itemopenmap.Size = new System.Drawing.Size(167, 22);
			this.itemopenmap.Text = "Open Map...";
			// 
			// toolStripMenuItem1
			// 
			this.toolStripMenuItem1.Name = "toolStripMenuItem1";
			this.toolStripMenuItem1.Size = new System.Drawing.Size(164, 6);
			// 
			// itemsavemap
			// 
			this.itemsavemap.Name = "itemsavemap";
			this.itemsavemap.Size = new System.Drawing.Size(167, 22);
			this.itemsavemap.Text = "Save Map";
			// 
			// itemsavemapas
			// 
			this.itemsavemapas.Name = "itemsavemapas";
			this.itemsavemapas.Size = new System.Drawing.Size(167, 22);
			this.itemsavemapas.Text = "Save Map As...";
			// 
			// itemsavemapinto
			// 
			this.itemsavemapinto.Name = "itemsavemapinto";
			this.itemsavemapinto.Size = new System.Drawing.Size(167, 22);
			this.itemsavemapinto.Text = "Save Map Into...";
			// 
			// toolStripMenuItem2
			// 
			this.toolStripMenuItem2.Name = "toolStripMenuItem2";
			this.toolStripMenuItem2.Size = new System.Drawing.Size(164, 6);
			// 
			// itemexit
			// 
			this.itemexit.Name = "itemexit";
			this.itemexit.Size = new System.Drawing.Size(167, 22);
			this.itemexit.Text = "Exit";
			// 
			// toolbar
			// 
			this.toolbar.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			this.toolbar.Location = new System.Drawing.Point(0, 24);
			this.toolbar.Name = "toolbar";
			this.toolbar.Size = new System.Drawing.Size(619, 25);
			this.toolbar.TabIndex = 1;
			this.toolbar.Text = "toolStrip1";
			// 
			// statusbar
			// 
			this.statusbar.Location = new System.Drawing.Point(0, 471);
			this.statusbar.Name = "statusbar";
			this.statusbar.Size = new System.Drawing.Size(619, 22);
			this.statusbar.TabIndex = 2;
			// 
			// panelinfo
			// 
			this.panelinfo.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.panelinfo.Location = new System.Drawing.Point(0, 370);
			this.panelinfo.Name = "panelinfo";
			this.panelinfo.Size = new System.Drawing.Size(619, 101);
			this.panelinfo.TabIndex = 4;
			// 
			// display
			// 
			this.display.BackColor = System.Drawing.SystemColors.AppWorkspace;
			this.display.BackgroundImage = global::CodeImp.DoomBuilder.Properties.Resources.Splash2;
			this.display.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this.display.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.display.Dock = System.Windows.Forms.DockStyle.Fill;
			this.display.ErrorImage = null;
			this.display.InitialImage = null;
			this.display.Location = new System.Drawing.Point(0, 49);
			this.display.Name = "display";
			this.display.Size = new System.Drawing.Size(619, 321);
			this.display.TabIndex = 5;
			this.display.TabStop = false;
			// 
			// MainForm
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.ClientSize = new System.Drawing.Size(619, 493);
			this.Controls.Add(this.display);
			this.Controls.Add(this.panelinfo);
			this.Controls.Add(this.statusbar);
			this.Controls.Add(this.toolbar);
			this.Controls.Add(this.menumain);
			this.DoubleBuffered = true;
			this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.KeyPreview = true;
			this.MainMenuStrip = this.menumain;
			this.Name = "MainForm";
			this.Text = "Doom Builder";
			this.menumain.ResumeLayout(false);
			this.menumain.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.display)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.MenuStrip menumain;
		private System.Windows.Forms.ToolStrip toolbar;
		private System.Windows.Forms.StatusStrip statusbar;
		private System.Windows.Forms.Panel panelinfo;
		private System.Windows.Forms.PictureBox display;
		private System.Windows.Forms.ToolStripMenuItem menufile;
		private System.Windows.Forms.ToolStripMenuItem itemnewmap;
		private System.Windows.Forms.ToolStripMenuItem itemopenmap;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
		private System.Windows.Forms.ToolStripMenuItem itemsavemap;
		private System.Windows.Forms.ToolStripMenuItem itemsavemapas;
		private System.Windows.Forms.ToolStripMenuItem itemsavemapinto;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
		private System.Windows.Forms.ToolStripMenuItem itemexit;
	}
}