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
			this.menuhelp = new System.Windows.Forms.ToolStripMenuItem();
			this.itemhelpabout = new System.Windows.Forms.ToolStripMenuItem();
			this.toolbar = new System.Windows.Forms.ToolStrip();
			this.statusbar = new System.Windows.Forms.StatusStrip();
			this.statuslabel = new System.Windows.Forms.ToolStripStatusLabel();
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
			this.itemnewmap.Name = "itemnewmap";
			this.itemnewmap.ShortcutKeyDisplayString = "";
			this.itemnewmap.Size = new System.Drawing.Size(201, 22);
			this.itemnewmap.Tag = "newmap";
			this.itemnewmap.Text = "New Map";
			this.itemnewmap.Click += new System.EventHandler(this.itemnewmap_Click);
			// 
			// itemopenmap
			// 
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
			this.itemclosemap.Text = "Close Map";
			this.itemclosemap.Click += new System.EventHandler(this.itemclosemap_Click);
			// 
			// itemsavemap
			// 
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
			this.toolbar.Location = new System.Drawing.Point(0, 24);
			this.toolbar.Name = "toolbar";
			this.toolbar.Size = new System.Drawing.Size(731, 25);
			this.toolbar.TabIndex = 1;
			this.toolbar.Text = "toolStrip1";
			// 
			// statusbar
			// 
			this.statusbar.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.statusbar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statuslabel,
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
			this.statuslabel.Size = new System.Drawing.Size(568, 18);
			this.statuslabel.Spring = true;
			this.statuslabel.Text = "Initializing user interface...";
			this.statuslabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
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
			this.display.Paint += new System.Windows.Forms.PaintEventHandler(this.display_Paint);
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
	}
}