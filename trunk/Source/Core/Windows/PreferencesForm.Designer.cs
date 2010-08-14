namespace CodeImp.DoomBuilder.Windows
{
	partial class PreferencesForm
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
				controller = null;
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
			System.Windows.Forms.Label label7;
			System.Windows.Forms.Label label6;
			System.Windows.Forms.Label label5;
			System.Windows.Forms.GroupBox groupBox1;
			System.Windows.Forms.Label label1;
			this.zoomfactor = new Dotnetrix.Controls.TrackBar();
			this.zoomfactorlabel = new System.Windows.Forms.Label();
			this.label19 = new System.Windows.Forms.Label();
			this.autoscrollspeed = new Dotnetrix.Controls.TrackBar();
			this.autoscrollspeedlabel = new System.Windows.Forms.Label();
			this.label15 = new System.Windows.Forms.Label();
			this.previewsize = new Dotnetrix.Controls.TrackBar();
			this.previewsizelabel = new System.Windows.Forms.Label();
			this.label12 = new System.Windows.Forms.Label();
			this.label14 = new System.Windows.Forms.Label();
			this.defaultviewmode = new System.Windows.Forms.ComboBox();
			this.keyusedlabel = new System.Windows.Forms.Label();
			this.colorsgroup1 = new System.Windows.Forms.GroupBox();
			this.colorgrid64 = new CodeImp.DoomBuilder.Controls.ColorControl();
			this.colorgrid = new CodeImp.DoomBuilder.Controls.ColorControl();
			this.colorindication = new CodeImp.DoomBuilder.Controls.ColorControl();
			this.colorsoundlinedefs = new CodeImp.DoomBuilder.Controls.ColorControl();
			this.colorspeciallinedefs = new CodeImp.DoomBuilder.Controls.ColorControl();
			this.colorbackcolor = new CodeImp.DoomBuilder.Controls.ColorControl();
			this.colorselection = new CodeImp.DoomBuilder.Controls.ColorControl();
			this.colorvertices = new CodeImp.DoomBuilder.Controls.ColorControl();
			this.colorhighlight = new CodeImp.DoomBuilder.Controls.ColorControl();
			this.colorlinedefs = new CodeImp.DoomBuilder.Controls.ColorControl();
			this.cancel = new System.Windows.Forms.Button();
			this.apply = new System.Windows.Forms.Button();
			this.tabs = new System.Windows.Forms.TabControl();
			this.tabinterface = new System.Windows.Forms.TabPage();
			this.groupBox4 = new System.Windows.Forms.GroupBox();
			this.collapsedockers = new System.Windows.Forms.CheckBox();
			this.dockersposition = new System.Windows.Forms.ComboBox();
			this.label17 = new System.Windows.Forms.Label();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.scripttabwidth = new CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox();
			this.scriptautoindent = new System.Windows.Forms.CheckBox();
			this.label10 = new System.Windows.Forms.Label();
			this.scriptontop = new System.Windows.Forms.CheckBox();
			this.panel1 = new System.Windows.Forms.Panel();
			this.scriptfontlabel = new System.Windows.Forms.Label();
			this.scriptfontsize = new System.Windows.Forms.ComboBox();
			this.label8 = new System.Windows.Forms.Label();
			this.scriptfontbold = new System.Windows.Forms.CheckBox();
			this.scriptfontname = new System.Windows.Forms.ComboBox();
			this.label3 = new System.Windows.Forms.Label();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.viewdistance = new Dotnetrix.Controls.TrackBar();
			this.movespeed = new Dotnetrix.Controls.TrackBar();
			this.mousespeed = new Dotnetrix.Controls.TrackBar();
			this.fieldofview = new Dotnetrix.Controls.TrackBar();
			this.viewdistancelabel = new System.Windows.Forms.Label();
			this.label13 = new System.Windows.Forms.Label();
			this.invertyaxis = new System.Windows.Forms.CheckBox();
			this.movespeedlabel = new System.Windows.Forms.Label();
			this.label11 = new System.Windows.Forms.Label();
			this.mousespeedlabel = new System.Windows.Forms.Label();
			this.label9 = new System.Windows.Forms.Label();
			this.fieldofviewlabel = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.tabkeys = new System.Windows.Forms.TabPage();
			this.listactions = new System.Windows.Forms.ListView();
			this.columncontrolaction = new System.Windows.Forms.ColumnHeader();
			this.columncontrolkey = new System.Windows.Forms.ColumnHeader();
			this.actioncontrolpanel = new System.Windows.Forms.GroupBox();
			this.keyusedlist = new System.Windows.Forms.ListBox();
			this.disregardshiftlabel = new System.Windows.Forms.Label();
			this.actioncontrol = new System.Windows.Forms.ComboBox();
			this.actiontitle = new System.Windows.Forms.Label();
			this.actioncontrolclear = new System.Windows.Forms.Button();
			this.actionkey = new System.Windows.Forms.TextBox();
			this.actiondescription = new System.Windows.Forms.Label();
			this.tabcolors = new System.Windows.Forms.TabPage();
			this.appearancegroup1 = new System.Windows.Forms.GroupBox();
			this.label2 = new System.Windows.Forms.Label();
			this.animatevisualselection = new System.Windows.Forms.CheckBox();
			this.blackbrowsers = new System.Windows.Forms.CheckBox();
			this.squarethings = new System.Windows.Forms.CheckBox();
			this.doublesidedalphalabel = new System.Windows.Forms.Label();
			this.visualbilinear = new System.Windows.Forms.CheckBox();
			this.classicbilinear = new System.Windows.Forms.CheckBox();
			this.imagebrightnesslabel = new System.Windows.Forms.Label();
			this.qualitydisplay = new System.Windows.Forms.CheckBox();
			this.doublesidedalpha = new Dotnetrix.Controls.TrackBar();
			this.imagebrightness = new Dotnetrix.Controls.TrackBar();
			this.colorsgroup3 = new System.Windows.Forms.GroupBox();
			this.colorconstants = new CodeImp.DoomBuilder.Controls.ColorControl();
			this.colorliterals = new CodeImp.DoomBuilder.Controls.ColorControl();
			this.colorscriptbackground = new CodeImp.DoomBuilder.Controls.ColorControl();
			this.colorkeywords = new CodeImp.DoomBuilder.Controls.ColorControl();
			this.colorlinenumbers = new CodeImp.DoomBuilder.Controls.ColorControl();
			this.colorcomments = new CodeImp.DoomBuilder.Controls.ColorControl();
			this.colorplaintext = new CodeImp.DoomBuilder.Controls.ColorControl();
			this.tabpasting = new System.Windows.Forms.TabPage();
			this.label16 = new System.Windows.Forms.Label();
			this.pasteoptions = new CodeImp.DoomBuilder.Controls.PasteOptionsControl();
			label7 = new System.Windows.Forms.Label();
			label6 = new System.Windows.Forms.Label();
			label5 = new System.Windows.Forms.Label();
			groupBox1 = new System.Windows.Forms.GroupBox();
			label1 = new System.Windows.Forms.Label();
			groupBox1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.zoomfactor)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.autoscrollspeed)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.previewsize)).BeginInit();
			this.colorsgroup1.SuspendLayout();
			this.tabs.SuspendLayout();
			this.tabinterface.SuspendLayout();
			this.groupBox4.SuspendLayout();
			this.groupBox3.SuspendLayout();
			this.panel1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.viewdistance)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.movespeed)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.mousespeed)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.fieldofview)).BeginInit();
			this.tabkeys.SuspendLayout();
			this.actioncontrolpanel.SuspendLayout();
			this.tabcolors.SuspendLayout();
			this.appearancegroup1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.doublesidedalpha)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.imagebrightness)).BeginInit();
			this.colorsgroup3.SuspendLayout();
			this.tabpasting.SuspendLayout();
			this.SuspendLayout();
			// 
			// label7
			// 
			label7.AutoSize = true;
			label7.Location = new System.Drawing.Point(20, 172);
			label7.Name = "label7";
			label7.Size = new System.Drawing.Size(187, 14);
			label7.TabIndex = 7;
			label7.Text = "Or select a special input control here:";
			// 
			// label6
			// 
			label6.AutoSize = true;
			label6.Location = new System.Drawing.Point(20, 30);
			label6.Name = "label6";
			label6.Size = new System.Drawing.Size(41, 14);
			label6.TabIndex = 2;
			label6.Text = "Action:";
			// 
			// label5
			// 
			label5.AutoSize = true;
			label5.Location = new System.Drawing.Point(20, 122);
			label5.Name = "label5";
			label5.Size = new System.Drawing.Size(200, 14);
			label5.TabIndex = 4;
			label5.Text = "Press the desired key combination here:";
			// 
			// groupBox1
			// 
			groupBox1.Controls.Add(this.zoomfactor);
			groupBox1.Controls.Add(this.zoomfactorlabel);
			groupBox1.Controls.Add(this.label19);
			groupBox1.Controls.Add(this.autoscrollspeed);
			groupBox1.Controls.Add(this.autoscrollspeedlabel);
			groupBox1.Controls.Add(this.label15);
			groupBox1.Controls.Add(this.previewsize);
			groupBox1.Controls.Add(this.previewsizelabel);
			groupBox1.Controls.Add(this.label12);
			groupBox1.Controls.Add(this.label14);
			groupBox1.Controls.Add(this.defaultviewmode);
			groupBox1.Location = new System.Drawing.Point(8, 8);
			groupBox1.Name = "groupBox1";
			groupBox1.Size = new System.Drawing.Size(298, 249);
			groupBox1.TabIndex = 0;
			groupBox1.TabStop = false;
			groupBox1.Text = " Options ";
			// 
			// zoomfactor
			// 
			this.zoomfactor.LargeChange = 1;
			this.zoomfactor.Location = new System.Drawing.Point(127, 187);
			this.zoomfactor.Minimum = 1;
			this.zoomfactor.Name = "zoomfactor";
			this.zoomfactor.Size = new System.Drawing.Size(92, 42);
			this.zoomfactor.TabIndex = 37;
			this.zoomfactor.TickStyle = System.Windows.Forms.TickStyle.Both;
			this.zoomfactor.Value = 3;
			this.zoomfactor.ValueChanged += new System.EventHandler(this.zoomfactor_ValueChanged);
			// 
			// zoomfactorlabel
			// 
			this.zoomfactorlabel.AutoSize = true;
			this.zoomfactorlabel.Location = new System.Drawing.Point(225, 199);
			this.zoomfactorlabel.Name = "zoomfactorlabel";
			this.zoomfactorlabel.Size = new System.Drawing.Size(29, 14);
			this.zoomfactorlabel.TabIndex = 39;
			this.zoomfactorlabel.Text = "30%";
			// 
			// label19
			// 
			this.label19.AutoSize = true;
			this.label19.Location = new System.Drawing.Point(52, 199);
			this.label19.Name = "label19";
			this.label19.Size = new System.Drawing.Size(69, 14);
			this.label19.TabIndex = 38;
			this.label19.Text = "Zoom factor:";
			// 
			// autoscrollspeed
			// 
			this.autoscrollspeed.LargeChange = 1;
			this.autoscrollspeed.Location = new System.Drawing.Point(127, 135);
			this.autoscrollspeed.Maximum = 5;
			this.autoscrollspeed.Name = "autoscrollspeed";
			this.autoscrollspeed.Size = new System.Drawing.Size(92, 42);
			this.autoscrollspeed.TabIndex = 2;
			this.autoscrollspeed.TickStyle = System.Windows.Forms.TickStyle.Both;
			this.autoscrollspeed.ValueChanged += new System.EventHandler(this.autoscrollspeed_ValueChanged);
			// 
			// autoscrollspeedlabel
			// 
			this.autoscrollspeedlabel.AutoSize = true;
			this.autoscrollspeedlabel.Location = new System.Drawing.Point(225, 147);
			this.autoscrollspeedlabel.Name = "autoscrollspeedlabel";
			this.autoscrollspeedlabel.Size = new System.Drawing.Size(23, 14);
			this.autoscrollspeedlabel.TabIndex = 36;
			this.autoscrollspeedlabel.Text = "Off";
			// 
			// label15
			// 
			this.label15.AutoSize = true;
			this.label15.Location = new System.Drawing.Point(29, 147);
			this.label15.Name = "label15";
			this.label15.Size = new System.Drawing.Size(92, 14);
			this.label15.TabIndex = 35;
			this.label15.Text = "Autoscroll speed:";
			// 
			// previewsize
			// 
			this.previewsize.LargeChange = 1;
			this.previewsize.Location = new System.Drawing.Point(127, 81);
			this.previewsize.Maximum = 5;
			this.previewsize.Name = "previewsize";
			this.previewsize.Size = new System.Drawing.Size(92, 42);
			this.previewsize.TabIndex = 1;
			this.previewsize.TickStyle = System.Windows.Forms.TickStyle.Both;
			this.previewsize.Value = 5;
			this.previewsize.ValueChanged += new System.EventHandler(this.previewsize_ValueChanged);
			// 
			// previewsizelabel
			// 
			this.previewsizelabel.AutoSize = true;
			this.previewsizelabel.Location = new System.Drawing.Point(225, 93);
			this.previewsizelabel.Name = "previewsizelabel";
			this.previewsizelabel.Size = new System.Drawing.Size(55, 14);
			this.previewsizelabel.TabIndex = 33;
			this.previewsizelabel.Text = "128 x 128";
			// 
			// label12
			// 
			this.label12.AutoSize = true;
			this.label12.Location = new System.Drawing.Point(17, 93);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(104, 14);
			this.label12.TabIndex = 32;
			this.label12.Text = "Preview image size:";
			// 
			// label14
			// 
			this.label14.AutoSize = true;
			this.label14.Location = new System.Drawing.Point(50, 41);
			this.label14.Name = "label14";
			this.label14.Size = new System.Drawing.Size(71, 14);
			this.label14.TabIndex = 14;
			this.label14.Text = "Default view:";
			// 
			// defaultviewmode
			// 
			this.defaultviewmode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.defaultviewmode.FormattingEnabled = true;
			this.defaultviewmode.Items.AddRange(new object[] {
            "Wireframe",
            "Brightness Levels",
            "Floor Textures",
            "Ceiling Textures"});
			this.defaultviewmode.Location = new System.Drawing.Point(135, 38);
			this.defaultviewmode.Name = "defaultviewmode";
			this.defaultviewmode.Size = new System.Drawing.Size(145, 22);
			this.defaultviewmode.TabIndex = 0;
			// 
			// label1
			// 
			label1.AutoSize = true;
			label1.Location = new System.Drawing.Point(22, 94);
			label1.Name = "label1";
			label1.Size = new System.Drawing.Size(148, 14);
			label1.TabIndex = 20;
			label1.Text = "Texture and Flats brightness:";
			label1.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// keyusedlabel
			// 
			this.keyusedlabel.AutoSize = true;
			this.keyusedlabel.Location = new System.Drawing.Point(20, 287);
			this.keyusedlabel.Name = "keyusedlabel";
			this.keyusedlabel.Size = new System.Drawing.Size(222, 14);
			this.keyusedlabel.TabIndex = 10;
			this.keyusedlabel.Text = "Key combination also used by these actions:";
			this.keyusedlabel.Visible = false;
			// 
			// colorsgroup1
			// 
			this.colorsgroup1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)));
			this.colorsgroup1.Controls.Add(this.colorgrid64);
			this.colorsgroup1.Controls.Add(this.colorgrid);
			this.colorsgroup1.Controls.Add(this.colorindication);
			this.colorsgroup1.Controls.Add(this.colorsoundlinedefs);
			this.colorsgroup1.Controls.Add(this.colorspeciallinedefs);
			this.colorsgroup1.Controls.Add(this.colorbackcolor);
			this.colorsgroup1.Controls.Add(this.colorselection);
			this.colorsgroup1.Controls.Add(this.colorvertices);
			this.colorsgroup1.Controls.Add(this.colorhighlight);
			this.colorsgroup1.Controls.Add(this.colorlinedefs);
			this.colorsgroup1.Location = new System.Drawing.Point(8, 8);
			this.colorsgroup1.Name = "colorsgroup1";
			this.colorsgroup1.Size = new System.Drawing.Size(185, 489);
			this.colorsgroup1.TabIndex = 0;
			this.colorsgroup1.TabStop = false;
			this.colorsgroup1.Text = " Display ";
			this.colorsgroup1.Visible = false;
			// 
			// colorgrid64
			// 
			this.colorgrid64.BackColor = System.Drawing.Color.Transparent;
			this.colorgrid64.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.colorgrid64.Label = "64 Block grid:";
			this.colorgrid64.Location = new System.Drawing.Point(15, 414);
			this.colorgrid64.MaximumSize = new System.Drawing.Size(10000, 23);
			this.colorgrid64.MinimumSize = new System.Drawing.Size(100, 23);
			this.colorgrid64.Name = "colorgrid64";
			this.colorgrid64.Size = new System.Drawing.Size(150, 23);
			this.colorgrid64.TabIndex = 9;
			// 
			// colorgrid
			// 
			this.colorgrid.BackColor = System.Drawing.Color.Transparent;
			this.colorgrid.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.colorgrid.Label = "Custom grid:";
			this.colorgrid.Location = new System.Drawing.Point(15, 371);
			this.colorgrid.MaximumSize = new System.Drawing.Size(10000, 23);
			this.colorgrid.MinimumSize = new System.Drawing.Size(100, 23);
			this.colorgrid.Name = "colorgrid";
			this.colorgrid.Size = new System.Drawing.Size(150, 23);
			this.colorgrid.TabIndex = 8;
			// 
			// colorindication
			// 
			this.colorindication.BackColor = System.Drawing.Color.Transparent;
			this.colorindication.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.colorindication.Label = "Indications:";
			this.colorindication.Location = new System.Drawing.Point(15, 328);
			this.colorindication.MaximumSize = new System.Drawing.Size(10000, 23);
			this.colorindication.MinimumSize = new System.Drawing.Size(100, 23);
			this.colorindication.Name = "colorindication";
			this.colorindication.Size = new System.Drawing.Size(150, 23);
			this.colorindication.TabIndex = 7;
			// 
			// colorsoundlinedefs
			// 
			this.colorsoundlinedefs.BackColor = System.Drawing.Color.Transparent;
			this.colorsoundlinedefs.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.colorsoundlinedefs.Label = "Sound lines:";
			this.colorsoundlinedefs.Location = new System.Drawing.Point(15, 199);
			this.colorsoundlinedefs.MaximumSize = new System.Drawing.Size(10000, 23);
			this.colorsoundlinedefs.MinimumSize = new System.Drawing.Size(100, 23);
			this.colorsoundlinedefs.Name = "colorsoundlinedefs";
			this.colorsoundlinedefs.Size = new System.Drawing.Size(150, 23);
			this.colorsoundlinedefs.TabIndex = 4;
			// 
			// colorspeciallinedefs
			// 
			this.colorspeciallinedefs.BackColor = System.Drawing.Color.Transparent;
			this.colorspeciallinedefs.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.colorspeciallinedefs.Label = "Action lines:";
			this.colorspeciallinedefs.Location = new System.Drawing.Point(15, 156);
			this.colorspeciallinedefs.MaximumSize = new System.Drawing.Size(10000, 23);
			this.colorspeciallinedefs.MinimumSize = new System.Drawing.Size(100, 23);
			this.colorspeciallinedefs.Name = "colorspeciallinedefs";
			this.colorspeciallinedefs.Size = new System.Drawing.Size(150, 23);
			this.colorspeciallinedefs.TabIndex = 3;
			// 
			// colorbackcolor
			// 
			this.colorbackcolor.BackColor = System.Drawing.Color.Transparent;
			this.colorbackcolor.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.colorbackcolor.Label = "Background:";
			this.colorbackcolor.Location = new System.Drawing.Point(15, 27);
			this.colorbackcolor.MaximumSize = new System.Drawing.Size(10000, 23);
			this.colorbackcolor.MinimumSize = new System.Drawing.Size(100, 23);
			this.colorbackcolor.Name = "colorbackcolor";
			this.colorbackcolor.Size = new System.Drawing.Size(150, 23);
			this.colorbackcolor.TabIndex = 0;
			// 
			// colorselection
			// 
			this.colorselection.BackColor = System.Drawing.Color.Transparent;
			this.colorselection.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.colorselection.Label = "Selection:";
			this.colorselection.Location = new System.Drawing.Point(15, 285);
			this.colorselection.MaximumSize = new System.Drawing.Size(10000, 23);
			this.colorselection.MinimumSize = new System.Drawing.Size(100, 23);
			this.colorselection.Name = "colorselection";
			this.colorselection.Size = new System.Drawing.Size(150, 23);
			this.colorselection.TabIndex = 6;
			// 
			// colorvertices
			// 
			this.colorvertices.BackColor = System.Drawing.Color.Transparent;
			this.colorvertices.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.colorvertices.Label = "Vertices:";
			this.colorvertices.Location = new System.Drawing.Point(15, 70);
			this.colorvertices.MaximumSize = new System.Drawing.Size(10000, 23);
			this.colorvertices.MinimumSize = new System.Drawing.Size(100, 23);
			this.colorvertices.Name = "colorvertices";
			this.colorvertices.Size = new System.Drawing.Size(150, 23);
			this.colorvertices.TabIndex = 1;
			// 
			// colorhighlight
			// 
			this.colorhighlight.BackColor = System.Drawing.Color.Transparent;
			this.colorhighlight.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.colorhighlight.Label = "Highlight:";
			this.colorhighlight.Location = new System.Drawing.Point(15, 242);
			this.colorhighlight.MaximumSize = new System.Drawing.Size(10000, 23);
			this.colorhighlight.MinimumSize = new System.Drawing.Size(100, 23);
			this.colorhighlight.Name = "colorhighlight";
			this.colorhighlight.Size = new System.Drawing.Size(150, 23);
			this.colorhighlight.TabIndex = 5;
			// 
			// colorlinedefs
			// 
			this.colorlinedefs.BackColor = System.Drawing.Color.Transparent;
			this.colorlinedefs.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.colorlinedefs.Label = "Common lines:";
			this.colorlinedefs.Location = new System.Drawing.Point(15, 113);
			this.colorlinedefs.MaximumSize = new System.Drawing.Size(10000, 23);
			this.colorlinedefs.MinimumSize = new System.Drawing.Size(100, 23);
			this.colorlinedefs.Name = "colorlinedefs";
			this.colorlinedefs.Size = new System.Drawing.Size(150, 23);
			this.colorlinedefs.TabIndex = 2;
			// 
			// cancel
			// 
			this.cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancel.Location = new System.Drawing.Point(560, 554);
			this.cancel.Name = "cancel";
			this.cancel.Size = new System.Drawing.Size(112, 25);
			this.cancel.TabIndex = 2;
			this.cancel.Text = "Cancel";
			this.cancel.UseVisualStyleBackColor = true;
			this.cancel.Click += new System.EventHandler(this.cancel_Click);
			// 
			// apply
			// 
			this.apply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.apply.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.apply.Location = new System.Drawing.Point(442, 554);
			this.apply.Name = "apply";
			this.apply.Size = new System.Drawing.Size(112, 25);
			this.apply.TabIndex = 1;
			this.apply.Text = "OK";
			this.apply.UseVisualStyleBackColor = true;
			this.apply.Click += new System.EventHandler(this.apply_Click);
			// 
			// tabs
			// 
			this.tabs.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.tabs.Controls.Add(this.tabinterface);
			this.tabs.Controls.Add(this.tabkeys);
			this.tabs.Controls.Add(this.tabcolors);
			this.tabs.Controls.Add(this.tabpasting);
			this.tabs.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.tabs.ItemSize = new System.Drawing.Size(110, 19);
			this.tabs.Location = new System.Drawing.Point(11, 13);
			this.tabs.Name = "tabs";
			this.tabs.Padding = new System.Drawing.Point(20, 3);
			this.tabs.SelectedIndex = 0;
			this.tabs.Size = new System.Drawing.Size(661, 532);
			this.tabs.TabIndex = 0;
			this.tabs.SelectedIndexChanged += new System.EventHandler(this.tabs_SelectedIndexChanged);
			// 
			// tabinterface
			// 
			this.tabinterface.Controls.Add(this.groupBox4);
			this.tabinterface.Controls.Add(this.groupBox3);
			this.tabinterface.Controls.Add(this.groupBox2);
			this.tabinterface.Controls.Add(groupBox1);
			this.tabinterface.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.tabinterface.Location = new System.Drawing.Point(4, 23);
			this.tabinterface.Name = "tabinterface";
			this.tabinterface.Padding = new System.Windows.Forms.Padding(5);
			this.tabinterface.Size = new System.Drawing.Size(653, 505);
			this.tabinterface.TabIndex = 0;
			this.tabinterface.Text = "Interface";
			this.tabinterface.UseVisualStyleBackColor = true;
			// 
			// groupBox4
			// 
			this.groupBox4.Controls.Add(this.collapsedockers);
			this.groupBox4.Controls.Add(this.dockersposition);
			this.groupBox4.Controls.Add(this.label17);
			this.groupBox4.Location = new System.Drawing.Point(316, 373);
			this.groupBox4.Name = "groupBox4";
			this.groupBox4.Size = new System.Drawing.Size(329, 124);
			this.groupBox4.TabIndex = 3;
			this.groupBox4.TabStop = false;
			this.groupBox4.Text = " Side Panels ";
			// 
			// collapsedockers
			// 
			this.collapsedockers.AutoSize = true;
			this.collapsedockers.Location = new System.Drawing.Point(204, 41);
			this.collapsedockers.Name = "collapsedockers";
			this.collapsedockers.Size = new System.Drawing.Size(72, 18);
			this.collapsedockers.TabIndex = 2;
			this.collapsedockers.Text = "Auto hide";
			this.collapsedockers.UseVisualStyleBackColor = true;
			// 
			// dockersposition
			// 
			this.dockersposition.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.dockersposition.FormattingEnabled = true;
			this.dockersposition.Items.AddRange(new object[] {
            "Left",
            "Right",
            "None"});
			this.dockersposition.Location = new System.Drawing.Point(95, 39);
			this.dockersposition.Name = "dockersposition";
			this.dockersposition.Size = new System.Drawing.Size(85, 22);
			this.dockersposition.TabIndex = 1;
			// 
			// label17
			// 
			this.label17.AutoSize = true;
			this.label17.Location = new System.Drawing.Point(33, 42);
			this.label17.Name = "label17";
			this.label17.Size = new System.Drawing.Size(47, 14);
			this.label17.TabIndex = 0;
			this.label17.Text = "Position:";
			// 
			// groupBox3
			// 
			this.groupBox3.Controls.Add(this.scripttabwidth);
			this.groupBox3.Controls.Add(this.scriptautoindent);
			this.groupBox3.Controls.Add(this.label10);
			this.groupBox3.Controls.Add(this.scriptontop);
			this.groupBox3.Controls.Add(this.panel1);
			this.groupBox3.Controls.Add(this.scriptfontsize);
			this.groupBox3.Controls.Add(this.label8);
			this.groupBox3.Controls.Add(this.scriptfontbold);
			this.groupBox3.Controls.Add(this.scriptfontname);
			this.groupBox3.Controls.Add(this.label3);
			this.groupBox3.Location = new System.Drawing.Point(8, 263);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(298, 234);
			this.groupBox3.TabIndex = 1;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = " Script Editor ";
			// 
			// scripttabwidth
			// 
			this.scripttabwidth.AllowDecimal = false;
			this.scripttabwidth.AllowNegative = false;
			this.scripttabwidth.AllowRelative = false;
			this.scripttabwidth.ButtonStep = 2;
			this.scripttabwidth.Location = new System.Drawing.Point(76, 191);
			this.scripttabwidth.Name = "scripttabwidth";
			this.scripttabwidth.Size = new System.Drawing.Size(71, 24);
			this.scripttabwidth.StepValues = null;
			this.scripttabwidth.TabIndex = 22;
			// 
			// scriptautoindent
			// 
			this.scriptautoindent.AutoSize = true;
			this.scriptautoindent.Location = new System.Drawing.Point(171, 195);
			this.scriptautoindent.Name = "scriptautoindent";
			this.scriptautoindent.Size = new System.Drawing.Size(81, 18);
			this.scriptautoindent.TabIndex = 21;
			this.scriptautoindent.Text = "Auto indent";
			this.scriptautoindent.UseVisualStyleBackColor = true;
			// 
			// label10
			// 
			this.label10.AutoSize = true;
			this.label10.Location = new System.Drawing.Point(16, 196);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(58, 14);
			this.label10.TabIndex = 19;
			this.label10.Text = "Tab width:";
			// 
			// scriptontop
			// 
			this.scriptontop.AutoSize = true;
			this.scriptontop.Location = new System.Drawing.Point(53, 159);
			this.scriptontop.Name = "scriptontop";
			this.scriptontop.Size = new System.Drawing.Size(178, 18);
			this.scriptontop.TabIndex = 3;
			this.scriptontop.Text = "Always on top of main window";
			this.scriptontop.UseVisualStyleBackColor = true;
			// 
			// panel1
			// 
			this.panel1.BackColor = System.Drawing.SystemColors.Window;
			this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.panel1.Controls.Add(this.scriptfontlabel);
			this.panel1.Location = new System.Drawing.Point(53, 110);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(226, 38);
			this.panel1.TabIndex = 18;
			// 
			// scriptfontlabel
			// 
			this.scriptfontlabel.BackColor = System.Drawing.SystemColors.Window;
			this.scriptfontlabel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.scriptfontlabel.Location = new System.Drawing.Point(0, 0);
			this.scriptfontlabel.Name = "scriptfontlabel";
			this.scriptfontlabel.Size = new System.Drawing.Size(222, 34);
			this.scriptfontlabel.TabIndex = 0;
			this.scriptfontlabel.Text = "Font";
			this.scriptfontlabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// scriptfontsize
			// 
			this.scriptfontsize.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.scriptfontsize.FormattingEnabled = true;
			this.scriptfontsize.Items.AddRange(new object[] {
            "7",
            "8",
            "9",
            "10",
            "11",
            "12",
            "14",
            "16",
            "18",
            "20",
            "22",
            "24",
            "26",
            "28",
            "36",
            "48",
            "72"});
			this.scriptfontsize.Location = new System.Drawing.Point(53, 71);
			this.scriptfontsize.Name = "scriptfontsize";
			this.scriptfontsize.Size = new System.Drawing.Size(94, 22);
			this.scriptfontsize.TabIndex = 1;
			this.scriptfontsize.SelectedIndexChanged += new System.EventHandler(this.scriptfontsize_SelectedIndexChanged);
			// 
			// label8
			// 
			this.label8.AutoSize = true;
			this.label8.Location = new System.Drawing.Point(16, 74);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(31, 14);
			this.label8.TabIndex = 16;
			this.label8.Text = "Size:";
			// 
			// scriptfontbold
			// 
			this.scriptfontbold.AutoSize = true;
			this.scriptfontbold.Location = new System.Drawing.Point(171, 73);
			this.scriptfontbold.Name = "scriptfontbold";
			this.scriptfontbold.Size = new System.Drawing.Size(47, 18);
			this.scriptfontbold.TabIndex = 2;
			this.scriptfontbold.Text = "Bold";
			this.scriptfontbold.UseVisualStyleBackColor = true;
			this.scriptfontbold.CheckedChanged += new System.EventHandler(this.scriptfontbold_CheckedChanged);
			// 
			// scriptfontname
			// 
			this.scriptfontname.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.scriptfontname.FormattingEnabled = true;
			this.scriptfontname.Location = new System.Drawing.Point(53, 36);
			this.scriptfontname.Name = "scriptfontname";
			this.scriptfontname.Size = new System.Drawing.Size(226, 22);
			this.scriptfontname.Sorted = true;
			this.scriptfontname.TabIndex = 0;
			this.scriptfontname.SelectedIndexChanged += new System.EventHandler(this.scriptfontname_SelectedIndexChanged);
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(16, 39);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(31, 14);
			this.label3.TabIndex = 0;
			this.label3.Text = "Font:";
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.viewdistance);
			this.groupBox2.Controls.Add(this.movespeed);
			this.groupBox2.Controls.Add(this.mousespeed);
			this.groupBox2.Controls.Add(this.fieldofview);
			this.groupBox2.Controls.Add(this.viewdistancelabel);
			this.groupBox2.Controls.Add(this.label13);
			this.groupBox2.Controls.Add(this.invertyaxis);
			this.groupBox2.Controls.Add(this.movespeedlabel);
			this.groupBox2.Controls.Add(this.label11);
			this.groupBox2.Controls.Add(this.mousespeedlabel);
			this.groupBox2.Controls.Add(this.label9);
			this.groupBox2.Controls.Add(this.fieldofviewlabel);
			this.groupBox2.Controls.Add(this.label4);
			this.groupBox2.Location = new System.Drawing.Point(316, 8);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(329, 359);
			this.groupBox2.TabIndex = 2;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = " Visual Modes ";
			// 
			// viewdistance
			// 
			this.viewdistance.LargeChange = 2;
			this.viewdistance.Location = new System.Drawing.Point(108, 187);
			this.viewdistance.Maximum = 15;
			this.viewdistance.Minimum = 1;
			this.viewdistance.Name = "viewdistance";
			this.viewdistance.Size = new System.Drawing.Size(150, 42);
			this.viewdistance.TabIndex = 3;
			this.viewdistance.TickStyle = System.Windows.Forms.TickStyle.Both;
			this.viewdistance.Value = 1;
			this.viewdistance.ValueChanged += new System.EventHandler(this.viewdistance_ValueChanged);
			// 
			// movespeed
			// 
			this.movespeed.Location = new System.Drawing.Point(108, 135);
			this.movespeed.Maximum = 20;
			this.movespeed.Minimum = 1;
			this.movespeed.Name = "movespeed";
			this.movespeed.Size = new System.Drawing.Size(150, 42);
			this.movespeed.TabIndex = 2;
			this.movespeed.TickStyle = System.Windows.Forms.TickStyle.Both;
			this.movespeed.Value = 1;
			this.movespeed.ValueChanged += new System.EventHandler(this.movespeed_ValueChanged);
			// 
			// mousespeed
			// 
			this.mousespeed.Location = new System.Drawing.Point(108, 81);
			this.mousespeed.Maximum = 20;
			this.mousespeed.Minimum = 1;
			this.mousespeed.Name = "mousespeed";
			this.mousespeed.Size = new System.Drawing.Size(150, 42);
			this.mousespeed.TabIndex = 1;
			this.mousespeed.TickStyle = System.Windows.Forms.TickStyle.Both;
			this.mousespeed.Value = 1;
			this.mousespeed.ValueChanged += new System.EventHandler(this.mousespeed_ValueChanged);
			// 
			// fieldofview
			// 
			this.fieldofview.LargeChange = 1;
			this.fieldofview.Location = new System.Drawing.Point(108, 29);
			this.fieldofview.Maximum = 17;
			this.fieldofview.Minimum = 5;
			this.fieldofview.Name = "fieldofview";
			this.fieldofview.Size = new System.Drawing.Size(150, 42);
			this.fieldofview.TabIndex = 0;
			this.fieldofview.TickStyle = System.Windows.Forms.TickStyle.Both;
			this.fieldofview.Value = 5;
			this.fieldofview.ValueChanged += new System.EventHandler(this.fieldofview_ValueChanged);
			// 
			// viewdistancelabel
			// 
			this.viewdistancelabel.AutoSize = true;
			this.viewdistancelabel.Location = new System.Drawing.Point(264, 199);
			this.viewdistancelabel.Name = "viewdistancelabel";
			this.viewdistancelabel.Size = new System.Drawing.Size(42, 14);
			this.viewdistancelabel.TabIndex = 30;
			this.viewdistancelabel.Text = "200 mp";
			// 
			// label13
			// 
			this.label13.AutoSize = true;
			this.label13.Location = new System.Drawing.Point(22, 199);
			this.label13.Name = "label13";
			this.label13.Size = new System.Drawing.Size(80, 14);
			this.label13.TabIndex = 28;
			this.label13.Text = "View distance:";
			// 
			// invertyaxis
			// 
			this.invertyaxis.AutoSize = true;
			this.invertyaxis.Location = new System.Drawing.Point(36, 251);
			this.invertyaxis.Name = "invertyaxis";
			this.invertyaxis.Size = new System.Drawing.Size(122, 18);
			this.invertyaxis.TabIndex = 4;
			this.invertyaxis.Text = "Invert mouse Y axis";
			this.invertyaxis.UseVisualStyleBackColor = true;
			// 
			// movespeedlabel
			// 
			this.movespeedlabel.AutoSize = true;
			this.movespeedlabel.Location = new System.Drawing.Point(264, 147);
			this.movespeedlabel.Name = "movespeedlabel";
			this.movespeedlabel.Size = new System.Drawing.Size(25, 14);
			this.movespeedlabel.TabIndex = 25;
			this.movespeedlabel.Text = "100";
			// 
			// label11
			// 
			this.label11.AutoSize = true;
			this.label11.Location = new System.Drawing.Point(33, 147);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(69, 14);
			this.label11.TabIndex = 23;
			this.label11.Text = "Move speed:";
			// 
			// mousespeedlabel
			// 
			this.mousespeedlabel.AutoSize = true;
			this.mousespeedlabel.Location = new System.Drawing.Point(264, 93);
			this.mousespeedlabel.Name = "mousespeedlabel";
			this.mousespeedlabel.Size = new System.Drawing.Size(25, 14);
			this.mousespeedlabel.TabIndex = 22;
			this.mousespeedlabel.Text = "100";
			// 
			// label9
			// 
			this.label9.AutoSize = true;
			this.label9.Location = new System.Drawing.Point(27, 93);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(75, 14);
			this.label9.TabIndex = 20;
			this.label9.Text = "Mouse speed:";
			// 
			// fieldofviewlabel
			// 
			this.fieldofviewlabel.AutoSize = true;
			this.fieldofviewlabel.Location = new System.Drawing.Point(264, 41);
			this.fieldofviewlabel.Name = "fieldofviewlabel";
			this.fieldofviewlabel.Size = new System.Drawing.Size(23, 14);
			this.fieldofviewlabel.TabIndex = 19;
			this.fieldofviewlabel.Text = "50°";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(30, 41);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(72, 14);
			this.label4.TabIndex = 17;
			this.label4.Text = "Field of view:";
			// 
			// tabkeys
			// 
			this.tabkeys.Controls.Add(this.listactions);
			this.tabkeys.Controls.Add(this.actioncontrolpanel);
			this.tabkeys.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.tabkeys.Location = new System.Drawing.Point(4, 23);
			this.tabkeys.Name = "tabkeys";
			this.tabkeys.Padding = new System.Windows.Forms.Padding(3);
			this.tabkeys.Size = new System.Drawing.Size(653, 505);
			this.tabkeys.TabIndex = 1;
			this.tabkeys.Text = "Controls";
			this.tabkeys.UseVisualStyleBackColor = true;
			// 
			// listactions
			// 
			this.listactions.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.listactions.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columncontrolaction,
            this.columncontrolkey});
			this.listactions.FullRowSelect = true;
			this.listactions.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			this.listactions.HideSelection = false;
			this.listactions.Location = new System.Drawing.Point(11, 12);
			this.listactions.Margin = new System.Windows.Forms.Padding(8, 9, 8, 9);
			this.listactions.MultiSelect = false;
			this.listactions.Name = "listactions";
			this.listactions.Size = new System.Drawing.Size(337, 479);
			this.listactions.Sorting = System.Windows.Forms.SortOrder.Ascending;
			this.listactions.TabIndex = 0;
			this.listactions.TabStop = false;
			this.listactions.UseCompatibleStateImageBehavior = false;
			this.listactions.View = System.Windows.Forms.View.Details;
			this.listactions.MouseUp += new System.Windows.Forms.MouseEventHandler(this.listactions_MouseUp);
			this.listactions.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.listactions_ItemSelectionChanged);
			this.listactions.KeyUp += new System.Windows.Forms.KeyEventHandler(this.listactions_KeyUp);
			// 
			// columncontrolaction
			// 
			this.columncontrolaction.Text = "Action";
			this.columncontrolaction.Width = 179;
			// 
			// columncontrolkey
			// 
			this.columncontrolkey.Text = "Key";
			this.columncontrolkey.Width = 130;
			// 
			// actioncontrolpanel
			// 
			this.actioncontrolpanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.actioncontrolpanel.Controls.Add(this.keyusedlist);
			this.actioncontrolpanel.Controls.Add(this.keyusedlabel);
			this.actioncontrolpanel.Controls.Add(this.disregardshiftlabel);
			this.actioncontrolpanel.Controls.Add(this.actioncontrol);
			this.actioncontrolpanel.Controls.Add(label7);
			this.actioncontrolpanel.Controls.Add(this.actiontitle);
			this.actioncontrolpanel.Controls.Add(this.actioncontrolclear);
			this.actioncontrolpanel.Controls.Add(label6);
			this.actioncontrolpanel.Controls.Add(this.actionkey);
			this.actioncontrolpanel.Controls.Add(this.actiondescription);
			this.actioncontrolpanel.Controls.Add(label5);
			this.actioncontrolpanel.Enabled = false;
			this.actioncontrolpanel.Location = new System.Drawing.Point(362, 12);
			this.actioncontrolpanel.Margin = new System.Windows.Forms.Padding(6);
			this.actioncontrolpanel.Name = "actioncontrolpanel";
			this.actioncontrolpanel.Size = new System.Drawing.Size(282, 479);
			this.actioncontrolpanel.TabIndex = 9;
			this.actioncontrolpanel.TabStop = false;
			this.actioncontrolpanel.Text = " Action control ";
			// 
			// keyusedlist
			// 
			this.keyusedlist.BackColor = System.Drawing.SystemColors.Control;
			this.keyusedlist.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.keyusedlist.FormattingEnabled = true;
			this.keyusedlist.IntegralHeight = false;
			this.keyusedlist.ItemHeight = 14;
			this.keyusedlist.Location = new System.Drawing.Point(33, 307);
			this.keyusedlist.Name = "keyusedlist";
			this.keyusedlist.SelectionMode = System.Windows.Forms.SelectionMode.None;
			this.keyusedlist.Size = new System.Drawing.Size(232, 115);
			this.keyusedlist.Sorted = true;
			this.keyusedlist.TabIndex = 11;
			this.keyusedlist.Visible = false;
			// 
			// disregardshiftlabel
			// 
			this.disregardshiftlabel.Location = new System.Drawing.Point(20, 224);
			this.disregardshiftlabel.Name = "disregardshiftlabel";
			this.disregardshiftlabel.Size = new System.Drawing.Size(245, 47);
			this.disregardshiftlabel.TabIndex = 9;
			this.disregardshiftlabel.Tag = "The selected actions uses %s to modify its behavior. These modifiers can not be u" +
				"sed in a key combination for this action.";
			this.disregardshiftlabel.Text = "The selected actions uses Shift, Alt and Control to modify its behavior. These mo" +
				"difiers can not be used in a key combination for this action.";
			this.disregardshiftlabel.Visible = false;
			// 
			// actioncontrol
			// 
			this.actioncontrol.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.actioncontrol.FormattingEnabled = true;
			this.actioncontrol.ImeMode = System.Windows.Forms.ImeMode.Off;
			this.actioncontrol.Location = new System.Drawing.Point(23, 190);
			this.actioncontrol.Name = "actioncontrol";
			this.actioncontrol.Size = new System.Drawing.Size(197, 22);
			this.actioncontrol.TabIndex = 8;
			this.actioncontrol.TabStop = false;
			this.actioncontrol.SelectedIndexChanged += new System.EventHandler(this.actioncontrol_SelectedIndexChanged);
			// 
			// actiontitle
			// 
			this.actiontitle.AutoSize = true;
			this.actiontitle.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.actiontitle.Location = new System.Drawing.Point(67, 30);
			this.actiontitle.Name = "actiontitle";
			this.actiontitle.Size = new System.Drawing.Size(172, 14);
			this.actiontitle.TabIndex = 1;
			this.actiontitle.Text = "(select an action from the list)";
			this.actiontitle.UseMnemonic = false;
			// 
			// actioncontrolclear
			// 
			this.actioncontrolclear.Location = new System.Drawing.Point(193, 138);
			this.actioncontrolclear.Name = "actioncontrolclear";
			this.actioncontrolclear.Size = new System.Drawing.Size(63, 25);
			this.actioncontrolclear.TabIndex = 6;
			this.actioncontrolclear.TabStop = false;
			this.actioncontrolclear.Text = "Clear";
			this.actioncontrolclear.UseVisualStyleBackColor = true;
			this.actioncontrolclear.Click += new System.EventHandler(this.actioncontrolclear_Click);
			// 
			// actionkey
			// 
			this.actionkey.ImeMode = System.Windows.Forms.ImeMode.Off;
			this.actionkey.Location = new System.Drawing.Point(23, 140);
			this.actionkey.Name = "actionkey";
			this.actionkey.Size = new System.Drawing.Size(163, 20);
			this.actionkey.TabIndex = 5;
			this.actionkey.TabStop = false;
			this.actionkey.KeyDown += new System.Windows.Forms.KeyEventHandler(this.actionkey_KeyDown);
			// 
			// actiondescription
			// 
			this.actiondescription.AutoEllipsis = true;
			this.actiondescription.Location = new System.Drawing.Point(20, 50);
			this.actiondescription.Name = "actiondescription";
			this.actiondescription.Size = new System.Drawing.Size(245, 71);
			this.actiondescription.TabIndex = 3;
			this.actiondescription.UseMnemonic = false;
			// 
			// tabcolors
			// 
			this.tabcolors.Controls.Add(this.appearancegroup1);
			this.tabcolors.Controls.Add(this.colorsgroup3);
			this.tabcolors.Controls.Add(this.colorsgroup1);
			this.tabcolors.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.tabcolors.Location = new System.Drawing.Point(4, 23);
			this.tabcolors.Name = "tabcolors";
			this.tabcolors.Padding = new System.Windows.Forms.Padding(5);
			this.tabcolors.Size = new System.Drawing.Size(653, 505);
			this.tabcolors.TabIndex = 2;
			this.tabcolors.Text = "Appearance";
			this.tabcolors.UseVisualStyleBackColor = true;
			// 
			// appearancegroup1
			// 
			this.appearancegroup1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.appearancegroup1.Controls.Add(this.label2);
			this.appearancegroup1.Controls.Add(this.animatevisualselection);
			this.appearancegroup1.Controls.Add(this.blackbrowsers);
			this.appearancegroup1.Controls.Add(this.squarethings);
			this.appearancegroup1.Controls.Add(this.doublesidedalphalabel);
			this.appearancegroup1.Controls.Add(this.visualbilinear);
			this.appearancegroup1.Controls.Add(label1);
			this.appearancegroup1.Controls.Add(this.classicbilinear);
			this.appearancegroup1.Controls.Add(this.imagebrightnesslabel);
			this.appearancegroup1.Controls.Add(this.qualitydisplay);
			this.appearancegroup1.Controls.Add(this.doublesidedalpha);
			this.appearancegroup1.Controls.Add(this.imagebrightness);
			this.appearancegroup1.Location = new System.Drawing.Point(199, 220);
			this.appearancegroup1.Name = "appearancegroup1";
			this.appearancegroup1.Size = new System.Drawing.Size(446, 277);
			this.appearancegroup1.TabIndex = 24;
			this.appearancegroup1.TabStop = false;
			this.appearancegroup1.Text = " Additional Options ";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(23, 40);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(147, 14);
			this.label2.TabIndex = 14;
			this.label2.Text = "Passable lines transparency:";
			this.label2.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// animatevisualselection
			// 
			this.animatevisualselection.AutoSize = true;
			this.animatevisualselection.Location = new System.Drawing.Point(245, 208);
			this.animatevisualselection.Name = "animatevisualselection";
			this.animatevisualselection.Size = new System.Drawing.Size(188, 18);
			this.animatevisualselection.TabIndex = 23;
			this.animatevisualselection.Text = "Animate selection in visual modes";
			this.animatevisualselection.UseVisualStyleBackColor = true;
			// 
			// blackbrowsers
			// 
			this.blackbrowsers.AutoSize = true;
			this.blackbrowsers.Location = new System.Drawing.Point(26, 148);
			this.blackbrowsers.Name = "blackbrowsers";
			this.blackbrowsers.Size = new System.Drawing.Size(199, 18);
			this.blackbrowsers.TabIndex = 4;
			this.blackbrowsers.Text = "Black background in image browser";
			this.blackbrowsers.UseVisualStyleBackColor = true;
			// 
			// squarethings
			// 
			this.squarethings.AutoSize = true;
			this.squarethings.Location = new System.Drawing.Point(245, 178);
			this.squarethings.Name = "squarethings";
			this.squarethings.Size = new System.Drawing.Size(93, 18);
			this.squarethings.TabIndex = 8;
			this.squarethings.Text = "Square things";
			this.squarethings.UseVisualStyleBackColor = true;
			// 
			// doublesidedalphalabel
			// 
			this.doublesidedalphalabel.AutoSize = true;
			this.doublesidedalphalabel.Location = new System.Drawing.Point(337, 40);
			this.doublesidedalphalabel.Name = "doublesidedalphalabel";
			this.doublesidedalphalabel.Size = new System.Drawing.Size(23, 14);
			this.doublesidedalphalabel.TabIndex = 16;
			this.doublesidedalphalabel.Text = "0%";
			// 
			// visualbilinear
			// 
			this.visualbilinear.AutoSize = true;
			this.visualbilinear.Location = new System.Drawing.Point(26, 208);
			this.visualbilinear.Name = "visualbilinear";
			this.visualbilinear.Size = new System.Drawing.Size(176, 18);
			this.visualbilinear.TabIndex = 6;
			this.visualbilinear.Text = "Bilinear filtering in visual modes";
			this.visualbilinear.UseVisualStyleBackColor = true;
			// 
			// classicbilinear
			// 
			this.classicbilinear.AutoSize = true;
			this.classicbilinear.Location = new System.Drawing.Point(26, 178);
			this.classicbilinear.Name = "classicbilinear";
			this.classicbilinear.Size = new System.Drawing.Size(182, 18);
			this.classicbilinear.TabIndex = 5;
			this.classicbilinear.Text = "Bilinear filtering in classic modes";
			this.classicbilinear.UseVisualStyleBackColor = true;
			// 
			// imagebrightnesslabel
			// 
			this.imagebrightnesslabel.AutoSize = true;
			this.imagebrightnesslabel.Location = new System.Drawing.Point(337, 94);
			this.imagebrightnesslabel.Name = "imagebrightnesslabel";
			this.imagebrightnesslabel.Size = new System.Drawing.Size(31, 14);
			this.imagebrightnesslabel.TabIndex = 22;
			this.imagebrightnesslabel.Text = "+ 0 y";
			// 
			// qualitydisplay
			// 
			this.qualitydisplay.AutoSize = true;
			this.qualitydisplay.Location = new System.Drawing.Point(245, 148);
			this.qualitydisplay.Name = "qualitydisplay";
			this.qualitydisplay.Size = new System.Drawing.Size(167, 18);
			this.qualitydisplay.TabIndex = 7;
			this.qualitydisplay.Text = "High quality display rendering";
			this.qualitydisplay.UseVisualStyleBackColor = true;
			// 
			// doublesidedalpha
			// 
			this.doublesidedalpha.LargeChange = 3;
			this.doublesidedalpha.Location = new System.Drawing.Point(176, 28);
			this.doublesidedalpha.Name = "doublesidedalpha";
			this.doublesidedalpha.Size = new System.Drawing.Size(154, 42);
			this.doublesidedalpha.TabIndex = 2;
			this.doublesidedalpha.TickStyle = System.Windows.Forms.TickStyle.Both;
			this.doublesidedalpha.ValueChanged += new System.EventHandler(this.doublesidedalpha_ValueChanged);
			// 
			// imagebrightness
			// 
			this.imagebrightness.LargeChange = 3;
			this.imagebrightness.Location = new System.Drawing.Point(176, 81);
			this.imagebrightness.Name = "imagebrightness";
			this.imagebrightness.Size = new System.Drawing.Size(154, 42);
			this.imagebrightness.TabIndex = 3;
			this.imagebrightness.TickStyle = System.Windows.Forms.TickStyle.Both;
			this.imagebrightness.ValueChanged += new System.EventHandler(this.imagebrightness_ValueChanged);
			// 
			// colorsgroup3
			// 
			this.colorsgroup3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.colorsgroup3.Controls.Add(this.colorconstants);
			this.colorsgroup3.Controls.Add(this.colorliterals);
			this.colorsgroup3.Controls.Add(this.colorscriptbackground);
			this.colorsgroup3.Controls.Add(this.colorkeywords);
			this.colorsgroup3.Controls.Add(this.colorlinenumbers);
			this.colorsgroup3.Controls.Add(this.colorcomments);
			this.colorsgroup3.Controls.Add(this.colorplaintext);
			this.colorsgroup3.Location = new System.Drawing.Point(199, 8);
			this.colorsgroup3.Name = "colorsgroup3";
			this.colorsgroup3.Size = new System.Drawing.Size(446, 206);
			this.colorsgroup3.TabIndex = 1;
			this.colorsgroup3.TabStop = false;
			this.colorsgroup3.Text = " Script editor ";
			this.colorsgroup3.Visible = false;
			// 
			// colorconstants
			// 
			this.colorconstants.BackColor = System.Drawing.Color.Transparent;
			this.colorconstants.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.colorconstants.Label = "Constants:";
			this.colorconstants.Location = new System.Drawing.Point(213, 113);
			this.colorconstants.MaximumSize = new System.Drawing.Size(10000, 23);
			this.colorconstants.MinimumSize = new System.Drawing.Size(100, 23);
			this.colorconstants.Name = "colorconstants";
			this.colorconstants.Size = new System.Drawing.Size(150, 23);
			this.colorconstants.TabIndex = 6;
			// 
			// colorliterals
			// 
			this.colorliterals.BackColor = System.Drawing.Color.Transparent;
			this.colorliterals.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.colorliterals.Label = "Literals:";
			this.colorliterals.Location = new System.Drawing.Point(213, 70);
			this.colorliterals.MaximumSize = new System.Drawing.Size(10000, 23);
			this.colorliterals.MinimumSize = new System.Drawing.Size(100, 23);
			this.colorliterals.Name = "colorliterals";
			this.colorliterals.Size = new System.Drawing.Size(150, 23);
			this.colorliterals.TabIndex = 5;
			// 
			// colorscriptbackground
			// 
			this.colorscriptbackground.BackColor = System.Drawing.Color.Transparent;
			this.colorscriptbackground.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.colorscriptbackground.Label = "Background:";
			this.colorscriptbackground.Location = new System.Drawing.Point(15, 27);
			this.colorscriptbackground.MaximumSize = new System.Drawing.Size(10000, 23);
			this.colorscriptbackground.MinimumSize = new System.Drawing.Size(100, 23);
			this.colorscriptbackground.Name = "colorscriptbackground";
			this.colorscriptbackground.Size = new System.Drawing.Size(150, 23);
			this.colorscriptbackground.TabIndex = 0;
			// 
			// colorkeywords
			// 
			this.colorkeywords.BackColor = System.Drawing.Color.Transparent;
			this.colorkeywords.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.colorkeywords.Label = "Keywords:";
			this.colorkeywords.Location = new System.Drawing.Point(213, 27);
			this.colorkeywords.MaximumSize = new System.Drawing.Size(10000, 23);
			this.colorkeywords.MinimumSize = new System.Drawing.Size(100, 23);
			this.colorkeywords.Name = "colorkeywords";
			this.colorkeywords.Size = new System.Drawing.Size(150, 23);
			this.colorkeywords.TabIndex = 4;
			// 
			// colorlinenumbers
			// 
			this.colorlinenumbers.BackColor = System.Drawing.Color.Transparent;
			this.colorlinenumbers.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.colorlinenumbers.Label = "Line numbers:";
			this.colorlinenumbers.Location = new System.Drawing.Point(15, 70);
			this.colorlinenumbers.MaximumSize = new System.Drawing.Size(10000, 23);
			this.colorlinenumbers.MinimumSize = new System.Drawing.Size(100, 23);
			this.colorlinenumbers.Name = "colorlinenumbers";
			this.colorlinenumbers.Size = new System.Drawing.Size(150, 23);
			this.colorlinenumbers.TabIndex = 1;
			// 
			// colorcomments
			// 
			this.colorcomments.BackColor = System.Drawing.Color.Transparent;
			this.colorcomments.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.colorcomments.Label = "Comments:";
			this.colorcomments.Location = new System.Drawing.Point(15, 156);
			this.colorcomments.MaximumSize = new System.Drawing.Size(10000, 23);
			this.colorcomments.MinimumSize = new System.Drawing.Size(100, 23);
			this.colorcomments.Name = "colorcomments";
			this.colorcomments.Size = new System.Drawing.Size(150, 23);
			this.colorcomments.TabIndex = 3;
			// 
			// colorplaintext
			// 
			this.colorplaintext.BackColor = System.Drawing.Color.Transparent;
			this.colorplaintext.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.colorplaintext.Label = "Plain text:";
			this.colorplaintext.Location = new System.Drawing.Point(15, 113);
			this.colorplaintext.MaximumSize = new System.Drawing.Size(10000, 23);
			this.colorplaintext.MinimumSize = new System.Drawing.Size(100, 23);
			this.colorplaintext.Name = "colorplaintext";
			this.colorplaintext.Size = new System.Drawing.Size(150, 23);
			this.colorplaintext.TabIndex = 2;
			// 
			// tabpasting
			// 
			this.tabpasting.Controls.Add(this.label16);
			this.tabpasting.Controls.Add(this.pasteoptions);
			this.tabpasting.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.tabpasting.Location = new System.Drawing.Point(4, 23);
			this.tabpasting.Name = "tabpasting";
			this.tabpasting.Padding = new System.Windows.Forms.Padding(5);
			this.tabpasting.Size = new System.Drawing.Size(653, 505);
			this.tabpasting.TabIndex = 3;
			this.tabpasting.Text = "Pasting ";
			this.tabpasting.UseVisualStyleBackColor = true;
			// 
			// label16
			// 
			this.label16.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.label16.Location = new System.Drawing.Point(11, 15);
			this.label16.Name = "label16";
			this.label16.Size = new System.Drawing.Size(634, 35);
			this.label16.TabIndex = 1;
			this.label16.Text = "These are the default options for pasting geometry. You can also choose these opt" +
				"ions when you use the Paste Special function. These options also apply when inse" +
				"rting prefabs.";
			// 
			// pasteoptions
			// 
			this.pasteoptions.Location = new System.Drawing.Point(8, 53);
			this.pasteoptions.Name = "pasteoptions";
			this.pasteoptions.Size = new System.Drawing.Size(637, 388);
			this.pasteoptions.TabIndex = 0;
			// 
			// PreferencesForm
			// 
			this.AcceptButton = this.apply;
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.CancelButton = this.apply;
			this.ClientSize = new System.Drawing.Size(682, 590);
			this.Controls.Add(this.cancel);
			this.Controls.Add(this.apply);
			this.Controls.Add(this.tabs);
			this.DoubleBuffered = true;
			this.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "PreferencesForm";
			this.Opacity = 0;
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Preferences";
			this.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.PreferencesForm_HelpRequested);
			groupBox1.ResumeLayout(false);
			groupBox1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.zoomfactor)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.autoscrollspeed)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.previewsize)).EndInit();
			this.colorsgroup1.ResumeLayout(false);
			this.tabs.ResumeLayout(false);
			this.tabinterface.ResumeLayout(false);
			this.groupBox4.ResumeLayout(false);
			this.groupBox4.PerformLayout();
			this.groupBox3.ResumeLayout(false);
			this.groupBox3.PerformLayout();
			this.panel1.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.viewdistance)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.movespeed)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.mousespeed)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.fieldofview)).EndInit();
			this.tabkeys.ResumeLayout(false);
			this.actioncontrolpanel.ResumeLayout(false);
			this.actioncontrolpanel.PerformLayout();
			this.tabcolors.ResumeLayout(false);
			this.appearancegroup1.ResumeLayout(false);
			this.appearancegroup1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.doublesidedalpha)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.imagebrightness)).EndInit();
			this.colorsgroup3.ResumeLayout(false);
			this.tabpasting.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button cancel;
		private System.Windows.Forms.Button apply;
		private System.Windows.Forms.TabControl tabs;
		private System.Windows.Forms.TabPage tabinterface;
		private System.Windows.Forms.TabPage tabkeys;
		private System.Windows.Forms.ListView listactions;
		private System.Windows.Forms.ColumnHeader columncontrolaction;
		private System.Windows.Forms.ColumnHeader columncontrolkey;
		private System.Windows.Forms.GroupBox actioncontrolpanel;
		private System.Windows.Forms.ComboBox actioncontrol;
		private System.Windows.Forms.Label actiontitle;
		private System.Windows.Forms.Button actioncontrolclear;
		private System.Windows.Forms.TextBox actionkey;
		private System.Windows.Forms.Label actiondescription;
		private System.Windows.Forms.TabPage tabcolors;
		private CodeImp.DoomBuilder.Controls.ColorControl colorselection;
		private CodeImp.DoomBuilder.Controls.ColorControl colorhighlight;
		private CodeImp.DoomBuilder.Controls.ColorControl colorlinedefs;
		private CodeImp.DoomBuilder.Controls.ColorControl colorvertices;
		private CodeImp.DoomBuilder.Controls.ColorControl colorbackcolor;
		private System.Windows.Forms.GroupBox colorsgroup3;
		private CodeImp.DoomBuilder.Controls.ColorControl colorscriptbackground;
		private CodeImp.DoomBuilder.Controls.ColorControl colorkeywords;
		private CodeImp.DoomBuilder.Controls.ColorControl colorlinenumbers;
		private CodeImp.DoomBuilder.Controls.ColorControl colorcomments;
		private CodeImp.DoomBuilder.Controls.ColorControl colorplaintext;
		private CodeImp.DoomBuilder.Controls.ColorControl colorliterals;
		private CodeImp.DoomBuilder.Controls.ColorControl colorconstants;
		private CodeImp.DoomBuilder.Controls.ColorControl colorspeciallinedefs;
		private CodeImp.DoomBuilder.Controls.ColorControl colorsoundlinedefs;
		private CodeImp.DoomBuilder.Controls.ColorControl colorindication;
		private CodeImp.DoomBuilder.Controls.ColorControl colorgrid64;
		private CodeImp.DoomBuilder.Controls.ColorControl colorgrid;
		private System.Windows.Forms.GroupBox colorsgroup1;
		private System.Windows.Forms.CheckBox blackbrowsers;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label doublesidedalphalabel;
		private System.Windows.Forms.Label imagebrightnesslabel;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.Label fieldofviewlabel;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label movespeedlabel;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.Label mousespeedlabel;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.Label viewdistancelabel;
		private System.Windows.Forms.Label label13;
		private System.Windows.Forms.CheckBox invertyaxis;
		private System.Windows.Forms.ComboBox defaultviewmode;
		private System.Windows.Forms.Label label14;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.CheckBox scriptfontbold;
		private System.Windows.Forms.ComboBox scriptfontname;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.ComboBox scriptfontsize;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Label scriptfontlabel;
		private Dotnetrix.Controls.TrackBar fieldofview;
		private Dotnetrix.Controls.TrackBar movespeed;
		private Dotnetrix.Controls.TrackBar mousespeed;
		private Dotnetrix.Controls.TrackBar viewdistance;
		private Dotnetrix.Controls.TrackBar doublesidedalpha;
		private Dotnetrix.Controls.TrackBar imagebrightness;
		private System.Windows.Forms.Label disregardshiftlabel;
		private System.Windows.Forms.ListBox keyusedlist;
		private System.Windows.Forms.Label keyusedlabel;
		private System.Windows.Forms.CheckBox scriptontop;
		private System.Windows.Forms.CheckBox qualitydisplay;
		private System.Windows.Forms.CheckBox visualbilinear;
		private System.Windows.Forms.CheckBox classicbilinear;
		private Dotnetrix.Controls.TrackBar previewsize;
		private System.Windows.Forms.Label previewsizelabel;
		private System.Windows.Forms.Label label12;
		private System.Windows.Forms.CheckBox squarethings;
		private Dotnetrix.Controls.TrackBar autoscrollspeed;
		private System.Windows.Forms.Label autoscrollspeedlabel;
		private System.Windows.Forms.Label label15;
		private System.Windows.Forms.CheckBox animatevisualselection;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.CheckBox scriptautoindent;
		private System.Windows.Forms.TabPage tabpasting;
		private CodeImp.DoomBuilder.Controls.PasteOptionsControl pasteoptions;
		private System.Windows.Forms.Label label16;
		private System.Windows.Forms.GroupBox appearancegroup1;
		private CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox scripttabwidth;
		private System.Windows.Forms.GroupBox groupBox4;
		private System.Windows.Forms.ComboBox dockersposition;
		private System.Windows.Forms.Label label17;
		private System.Windows.Forms.CheckBox collapsedockers;
		private Dotnetrix.Controls.TrackBar zoomfactor;
		private System.Windows.Forms.Label zoomfactorlabel;
		private System.Windows.Forms.Label label19;
	}
}