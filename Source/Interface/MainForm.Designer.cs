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
			this.components = new System.ComponentModel.Container();
			System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
			System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
			System.Windows.Forms.ToolStripSeparator toolStripMenuItem3;
			System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
			System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
			this.menumain = new System.Windows.Forms.MenuStrip();
			this.menufile = new System.Windows.Forms.ToolStripMenuItem();
			this.itemnewmap = new System.Windows.Forms.ToolStripMenuItem();
			this.itemopenmap = new System.Windows.Forms.ToolStripMenuItem();
			this.itemclosemap = new System.Windows.Forms.ToolStripMenuItem();
			this.itemsavemap = new System.Windows.Forms.ToolStripMenuItem();
			this.itemsavemapas = new System.Windows.Forms.ToolStripMenuItem();
			this.itemsavemapinto = new System.Windows.Forms.ToolStripMenuItem();
			this.itemnorecent = new System.Windows.Forms.ToolStripMenuItem();
			this.itemexit = new System.Windows.Forms.ToolStripMenuItem();
			this.menutools = new System.Windows.Forms.ToolStripMenuItem();
			this.configurationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.menuhelp = new System.Windows.Forms.ToolStripMenuItem();
			this.itemhelpabout = new System.Windows.Forms.ToolStripMenuItem();
			this.toolbar = new System.Windows.Forms.ToolStrip();
			this.buttonnewmap = new System.Windows.Forms.ToolStripButton();
			this.buttonopenmap = new System.Windows.Forms.ToolStripButton();
			this.buttonsavemap = new System.Windows.Forms.ToolStripButton();
			this.statusbar = new System.Windows.Forms.StatusStrip();
			this.statuslabel = new System.Windows.Forms.ToolStripStatusLabel();
			this.zoomlabel = new System.Windows.Forms.ToolStripStatusLabel();
			this.buttonzoom = new System.Windows.Forms.ToolStripDropDownButton();
			this.itemzoom200 = new System.Windows.Forms.ToolStripMenuItem();
			this.itemzoom100 = new System.Windows.Forms.ToolStripMenuItem();
			this.itemzoom50 = new System.Windows.Forms.ToolStripMenuItem();
			this.itemzoom25 = new System.Windows.Forms.ToolStripMenuItem();
			this.itemzoom10 = new System.Windows.Forms.ToolStripMenuItem();
			this.itemzoom5 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			this.itemzoomfittoscreen = new System.Windows.Forms.ToolStripMenuItem();
			this.xposlabel = new System.Windows.Forms.ToolStripStatusLabel();
			this.yposlabel = new System.Windows.Forms.ToolStripStatusLabel();
			this.panelinfo = new System.Windows.Forms.Panel();
			this.redrawtimer = new System.Windows.Forms.Timer(this.components);
			this.display = new System.Windows.Forms.Panel();
			toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
			toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
			toolStripMenuItem3 = new System.Windows.Forms.ToolStripSeparator();
			toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
			this.menumain.SuspendLayout();
			this.toolbar.SuspendLayout();
			this.statusbar.SuspendLayout();
			this.SuspendLayout();
			// 
			// toolStripMenuItem1
			// 
			toolStripMenuItem1.Name = "toolStripMenuItem1";
			toolStripMenuItem1.Size = new System.Drawing.Size(198, 6);
			// 
			// toolStripMenuItem2
			// 
			toolStripMenuItem2.Name = "toolStripMenuItem2";
			toolStripMenuItem2.Size = new System.Drawing.Size(198, 6);
			// 
			// toolStripMenuItem3
			// 
			toolStripMenuItem3.Name = "toolStripMenuItem3";
			toolStripMenuItem3.Size = new System.Drawing.Size(198, 6);
			// 
			// toolStripSeparator1
			// 
			toolStripSeparator1.Name = "toolStripSeparator1";
			toolStripSeparator1.Size = new System.Drawing.Size(6, 23);
			// 
			// toolStripStatusLabel1
			// 
			toolStripStatusLabel1.Name = "toolStripStatusLabel1";
			toolStripStatusLabel1.Size = new System.Drawing.Size(11, 18);
			toolStripStatusLabel1.Text = ",";
			toolStripStatusLabel1.ToolTipText = "Current X, Y coordinates on map";
			// 
			// menumain
			// 
			this.menumain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menufile,
            this.menutools,
            this.menuhelp});
			this.menumain.Location = new System.Drawing.Point(0, 0);
			this.menumain.Name = "menumain";
			this.menumain.Size = new System.Drawing.Size(731, 24);
			this.menumain.TabIndex = 0;
			this.menumain.Text = "menuStrip1";
			// 
			// menufile
			// 
			this.menufile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.itemnewmap,
            this.itemopenmap,
            this.itemclosemap,
            toolStripMenuItem1,
            this.itemsavemap,
            this.itemsavemapas,
            this.itemsavemapinto,
            toolStripMenuItem2,
            this.itemnorecent,
            toolStripMenuItem3,
            this.itemexit});
			this.menufile.Name = "menufile";
			this.menufile.Size = new System.Drawing.Size(35, 20);
			this.menufile.Text = "File";
			// 
			// itemnewmap
			// 
			this.itemnewmap.Image = global::CodeImp.DoomBuilder.Properties.Resources.NewMap;
			this.itemnewmap.Name = "itemnewmap";
			this.itemnewmap.ShortcutKeyDisplayString = "";
			this.itemnewmap.Size = new System.Drawing.Size(201, 22);
			this.itemnewmap.Tag = "newmap";
			this.itemnewmap.Text = "New Map";
			this.itemnewmap.Click += new System.EventHandler(this.itemnewmap_Click);
			// 
			// itemopenmap
			// 
			this.itemopenmap.Image = global::CodeImp.DoomBuilder.Properties.Resources.OpenMap;
			this.itemopenmap.Name = "itemopenmap";
			this.itemopenmap.Size = new System.Drawing.Size(201, 22);
			this.itemopenmap.Tag = "openmap";
			this.itemopenmap.Text = "Open Map...";
			this.itemopenmap.Click += new System.EventHandler(this.itemopenmap_Click);
			// 
			// itemclosemap
			// 
			this.itemclosemap.Name = "itemclosemap";
			this.itemclosemap.Size = new System.Drawing.Size(201, 22);
			this.itemclosemap.Tag = "closemap";
			this.itemclosemap.Text = "Close Map";
			this.itemclosemap.Click += new System.EventHandler(this.itemclosemap_Click);
			// 
			// itemsavemap
			// 
			this.itemsavemap.Image = global::CodeImp.DoomBuilder.Properties.Resources.SaveMap;
			this.itemsavemap.Name = "itemsavemap";
			this.itemsavemap.Size = new System.Drawing.Size(201, 22);
			this.itemsavemap.Text = "Save Map";
			// 
			// itemsavemapas
			// 
			this.itemsavemapas.Name = "itemsavemapas";
			this.itemsavemapas.Size = new System.Drawing.Size(201, 22);
			this.itemsavemapas.Text = "Save Map As...";
			// 
			// itemsavemapinto
			// 
			this.itemsavemapinto.Name = "itemsavemapinto";
			this.itemsavemapinto.Size = new System.Drawing.Size(201, 22);
			this.itemsavemapinto.Text = "Save Map Into...";
			// 
			// itemnorecent
			// 
			this.itemnorecent.Enabled = false;
			this.itemnorecent.Name = "itemnorecent";
			this.itemnorecent.Size = new System.Drawing.Size(201, 22);
			this.itemnorecent.Text = "No recently opened files";
			// 
			// itemexit
			// 
			this.itemexit.Name = "itemexit";
			this.itemexit.Size = new System.Drawing.Size(201, 22);
			this.itemexit.Text = "Exit";
			this.itemexit.Click += new System.EventHandler(this.itemexit_Click);
			// 
			// menutools
			// 
			this.menutools.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.configurationToolStripMenuItem});
			this.menutools.Name = "menutools";
			this.menutools.Size = new System.Drawing.Size(44, 20);
			this.menutools.Text = "Tools";
			// 
			// configurationToolStripMenuItem
			// 
			this.configurationToolStripMenuItem.Name = "configurationToolStripMenuItem";
			this.configurationToolStripMenuItem.Size = new System.Drawing.Size(162, 22);
			this.configurationToolStripMenuItem.Tag = "configuration";
			this.configurationToolStripMenuItem.Text = "Configuration...";
			this.configurationToolStripMenuItem.Click += new System.EventHandler(this.configurationToolStripMenuItem_Click);
			// 
			// menuhelp
			// 
			this.menuhelp.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.itemhelpabout});
			this.menuhelp.Name = "menuhelp";
			this.menuhelp.Size = new System.Drawing.Size(40, 20);
			this.menuhelp.Text = "Help";
			// 
			// itemhelpabout
			// 
			this.itemhelpabout.Name = "itemhelpabout";
			this.itemhelpabout.Size = new System.Drawing.Size(191, 22);
			this.itemhelpabout.Text = "About Doom Builder...";
			this.itemhelpabout.Click += new System.EventHandler(this.itemhelpabout_Click);
			// 
			// toolbar
			// 
			this.toolbar.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			this.toolbar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.buttonnewmap,
            this.buttonopenmap,
            this.buttonsavemap});
			this.toolbar.Location = new System.Drawing.Point(0, 24);
			this.toolbar.Name = "toolbar";
			this.toolbar.Size = new System.Drawing.Size(731, 25);
			this.toolbar.TabIndex = 1;
			this.toolbar.Text = "toolStrip1";
			// 
			// buttonnewmap
			// 
			this.buttonnewmap.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.buttonnewmap.Image = global::CodeImp.DoomBuilder.Properties.Resources.NewMap;
			this.buttonnewmap.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.buttonnewmap.Name = "buttonnewmap";
			this.buttonnewmap.Size = new System.Drawing.Size(23, 22);
			this.buttonnewmap.Text = "toolStripButton1";
			this.buttonnewmap.Click += new System.EventHandler(this.itemnewmap_Click);
			// 
			// buttonopenmap
			// 
			this.buttonopenmap.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.buttonopenmap.Image = global::CodeImp.DoomBuilder.Properties.Resources.OpenMap;
			this.buttonopenmap.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.buttonopenmap.Name = "buttonopenmap";
			this.buttonopenmap.Size = new System.Drawing.Size(23, 22);
			this.buttonopenmap.Text = "toolStripButton1";
			this.buttonopenmap.Click += new System.EventHandler(this.itemopenmap_Click);
			// 
			// buttonsavemap
			// 
			this.buttonsavemap.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.buttonsavemap.Image = global::CodeImp.DoomBuilder.Properties.Resources.SaveMap;
			this.buttonsavemap.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.buttonsavemap.Name = "buttonsavemap";
			this.buttonsavemap.Size = new System.Drawing.Size(23, 22);
			this.buttonsavemap.Text = "toolStripButton1";
			// 
			// statusbar
			// 
			this.statusbar.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.statusbar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statuslabel,
            this.zoomlabel,
            this.buttonzoom,
            toolStripSeparator1,
            this.xposlabel,
            toolStripStatusLabel1,
            this.yposlabel});
			this.statusbar.Location = new System.Drawing.Point(0, 522);
			this.statusbar.Name = "statusbar";
			this.statusbar.ShowItemToolTips = true;
			this.statusbar.Size = new System.Drawing.Size(731, 23);
			this.statusbar.TabIndex = 2;
			// 
			// statuslabel
			// 
			this.statuslabel.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.statuslabel.Name = "statuslabel";
			this.statuslabel.Size = new System.Drawing.Size(520, 18);
			this.statuslabel.Spring = true;
			this.statuslabel.Text = "Initializing user interface...";
			this.statuslabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// zoomlabel
			// 
			this.zoomlabel.AutoSize = false;
			this.zoomlabel.Name = "zoomlabel";
			this.zoomlabel.Size = new System.Drawing.Size(50, 18);
			this.zoomlabel.Text = "50%";
			this.zoomlabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.zoomlabel.TextImageRelation = System.Windows.Forms.TextImageRelation.Overlay;
			// 
			// buttonzoom
			// 
			this.buttonzoom.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.buttonzoom.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.itemzoom200,
            this.itemzoom100,
            this.itemzoom50,
            this.itemzoom25,
            this.itemzoom10,
            this.itemzoom5,
            this.toolStripSeparator2,
            this.itemzoomfittoscreen});
			this.buttonzoom.Image = global::CodeImp.DoomBuilder.Properties.Resources.Zoom;
			this.buttonzoom.ImageTransparentColor = System.Drawing.Color.Transparent;
			this.buttonzoom.Name = "buttonzoom";
			this.buttonzoom.Size = new System.Drawing.Size(29, 21);
			this.buttonzoom.Text = "Zoom";
			this.buttonzoom.ToolTipText = "Zoom level";
			// 
			// itemzoom200
			// 
			this.itemzoom200.Name = "itemzoom200";
			this.itemzoom200.Size = new System.Drawing.Size(167, 22);
			this.itemzoom200.Tag = "200";
			this.itemzoom200.Text = "200%";
			this.itemzoom200.Click += new System.EventHandler(this.itemzoomto_Click);
			// 
			// itemzoom100
			// 
			this.itemzoom100.Name = "itemzoom100";
			this.itemzoom100.Size = new System.Drawing.Size(167, 22);
			this.itemzoom100.Tag = "100";
			this.itemzoom100.Text = "100%";
			this.itemzoom100.Click += new System.EventHandler(this.itemzoomto_Click);
			// 
			// itemzoom50
			// 
			this.itemzoom50.Name = "itemzoom50";
			this.itemzoom50.Size = new System.Drawing.Size(167, 22);
			this.itemzoom50.Tag = "50";
			this.itemzoom50.Text = "50%";
			this.itemzoom50.Click += new System.EventHandler(this.itemzoomto_Click);
			// 
			// itemzoom25
			// 
			this.itemzoom25.Name = "itemzoom25";
			this.itemzoom25.Size = new System.Drawing.Size(167, 22);
			this.itemzoom25.Tag = "25";
			this.itemzoom25.Text = "25%";
			this.itemzoom25.Click += new System.EventHandler(this.itemzoomto_Click);
			// 
			// itemzoom10
			// 
			this.itemzoom10.Name = "itemzoom10";
			this.itemzoom10.Size = new System.Drawing.Size(167, 22);
			this.itemzoom10.Tag = "10";
			this.itemzoom10.Text = "10%";
			this.itemzoom10.Click += new System.EventHandler(this.itemzoomto_Click);
			// 
			// itemzoom5
			// 
			this.itemzoom5.Name = "itemzoom5";
			this.itemzoom5.Size = new System.Drawing.Size(167, 22);
			this.itemzoom5.Tag = "5";
			this.itemzoom5.Text = "5%";
			this.itemzoom5.Click += new System.EventHandler(this.itemzoomto_Click);
			// 
			// toolStripSeparator2
			// 
			this.toolStripSeparator2.Name = "toolStripSeparator2";
			this.toolStripSeparator2.Size = new System.Drawing.Size(164, 6);
			// 
			// itemzoomfittoscreen
			// 
			this.itemzoomfittoscreen.Name = "itemzoomfittoscreen";
			this.itemzoomfittoscreen.Size = new System.Drawing.Size(167, 22);
			this.itemzoomfittoscreen.Text = "Fit to screen";
			this.itemzoomfittoscreen.Click += new System.EventHandler(this.itemzoomfittoscreen_Click);
			// 
			// xposlabel
			// 
			this.xposlabel.AutoSize = false;
			this.xposlabel.Name = "xposlabel";
			this.xposlabel.Size = new System.Drawing.Size(50, 18);
			this.xposlabel.Text = "0";
			this.xposlabel.ToolTipText = "Current X, Y coordinates on map";
			// 
			// yposlabel
			// 
			this.yposlabel.AutoSize = false;
			this.yposlabel.Name = "yposlabel";
			this.yposlabel.Size = new System.Drawing.Size(50, 18);
			this.yposlabel.Text = "0";
			this.yposlabel.ToolTipText = "Current X, Y coordinates on map";
			// 
			// panelinfo
			// 
			this.panelinfo.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.panelinfo.Location = new System.Drawing.Point(0, 421);
			this.panelinfo.Name = "panelinfo";
			this.panelinfo.Size = new System.Drawing.Size(731, 101);
			this.panelinfo.TabIndex = 4;
			// 
			// redrawtimer
			// 
			this.redrawtimer.Interval = 1;
			this.redrawtimer.Tick += new System.EventHandler(this.redrawtimer_Tick);
			// 
			// display
			// 
			this.display.BackColor = System.Drawing.SystemColors.AppWorkspace;
			this.display.BackgroundImage = global::CodeImp.DoomBuilder.Properties.Resources.Splash2;
			this.display.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this.display.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.display.CausesValidation = false;
			this.display.Dock = System.Windows.Forms.DockStyle.Fill;
			this.display.Location = new System.Drawing.Point(0, 49);
			this.display.Name = "display";
			this.display.Size = new System.Drawing.Size(731, 372);
			this.display.TabIndex = 5;
			this.display.MouseLeave += new System.EventHandler(this.display_MouseLeave);
			this.display.MouseDown += new System.Windows.Forms.MouseEventHandler(this.display_MouseDown);
			this.display.MouseMove += new System.Windows.Forms.MouseEventHandler(this.display_MouseMove);
			this.display.MouseClick += new System.Windows.Forms.MouseEventHandler(this.display_MouseClick);
			this.display.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.display_MouseDoubleClick);
			this.display.Resize += new System.EventHandler(this.display_Resize);
			this.display.MouseEnter += new System.EventHandler(this.display_MouseEnter);
			this.display.MouseUp += new System.Windows.Forms.MouseEventHandler(this.display_MouseUp);
			// 
			// MainForm
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.ClientSize = new System.Drawing.Size(731, 545);
			this.Controls.Add(this.display);
			this.Controls.Add(this.panelinfo);
			this.Controls.Add(this.statusbar);
			this.Controls.Add(this.toolbar);
			this.Controls.Add(this.menumain);
			this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.KeyPreview = true;
			this.MainMenuStrip = this.menumain;
			this.Name = "MainForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
			this.Text = "Doom Builder";
			this.Resize += new System.EventHandler(this.MainForm_Resize);
			this.Move += new System.EventHandler(this.MainForm_Move);
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
			this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.MainForm_KeyUp);
			this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MainForm_KeyDown);
			this.ResizeEnd += new System.EventHandler(this.MainForm_ResizeEnd);
			this.Load += new System.EventHandler(this.MainForm_Load);
			this.menumain.ResumeLayout(false);
			this.menumain.PerformLayout();
			this.toolbar.ResumeLayout(false);
			this.toolbar.PerformLayout();
			this.statusbar.ResumeLayout(false);
			this.statusbar.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.MenuStrip menumain;
		private System.Windows.Forms.ToolStrip toolbar;
		private System.Windows.Forms.StatusStrip statusbar;
		private System.Windows.Forms.Panel panelinfo;
		private System.Windows.Forms.ToolStripMenuItem menufile;
		private System.Windows.Forms.ToolStripMenuItem itemnewmap;
		private System.Windows.Forms.ToolStripMenuItem itemopenmap;
		private System.Windows.Forms.ToolStripMenuItem itemsavemap;
		private System.Windows.Forms.ToolStripMenuItem itemsavemapas;
		private System.Windows.Forms.ToolStripMenuItem itemsavemapinto;
		private System.Windows.Forms.ToolStripMenuItem itemexit;
		private System.Windows.Forms.ToolStripStatusLabel statuslabel;
		private System.Windows.Forms.ToolStripMenuItem itemclosemap;
		private System.Windows.Forms.Timer redrawtimer;
		private System.Windows.Forms.ToolStripMenuItem menuhelp;
		private System.Windows.Forms.ToolStripMenuItem itemhelpabout;
		private System.Windows.Forms.Panel display;
		private System.Windows.Forms.ToolStripMenuItem itemnorecent;
		private System.Windows.Forms.ToolStripStatusLabel xposlabel;
		private System.Windows.Forms.ToolStripStatusLabel yposlabel;
		private System.Windows.Forms.ToolStripButton buttonnewmap;
		private System.Windows.Forms.ToolStripButton buttonopenmap;
		private System.Windows.Forms.ToolStripButton buttonsavemap;
		private System.Windows.Forms.ToolStripStatusLabel zoomlabel;
		private System.Windows.Forms.ToolStripDropDownButton buttonzoom;
		private System.Windows.Forms.ToolStripMenuItem itemzoomfittoscreen;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
		private System.Windows.Forms.ToolStripMenuItem itemzoom100;
		private System.Windows.Forms.ToolStripMenuItem itemzoom200;
		private System.Windows.Forms.ToolStripMenuItem itemzoom50;
		private System.Windows.Forms.ToolStripMenuItem itemzoom25;
		private System.Windows.Forms.ToolStripMenuItem itemzoom10;
		private System.Windows.Forms.ToolStripMenuItem itemzoom5;
		private System.Windows.Forms.ToolStripMenuItem menutools;
		private System.Windows.Forms.ToolStripMenuItem configurationToolStripMenuItem;
	}
}