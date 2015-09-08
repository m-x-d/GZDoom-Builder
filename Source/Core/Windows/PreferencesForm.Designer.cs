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
			this.components = new System.ComponentModel.Container();
			System.Windows.Forms.Label label7;
			System.Windows.Forms.Label label6;
			System.Windows.Forms.Label label5;
			System.Windows.Forms.GroupBox groupBox1;
			System.Windows.Forms.Label label1;
			System.Windows.Forms.Label label18;
			System.Windows.Forms.Label label20;
			System.Windows.Forms.Label label21;
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PreferencesForm));
			this.checkforupdates = new System.Windows.Forms.CheckBox();
			this.cbStoreEditTab = new System.Windows.Forms.CheckBox();
			this.locatetexturegroup = new System.Windows.Forms.CheckBox();
			this.recentFiles = new System.Windows.Forms.TrackBar();
			this.labelRecentFiles = new System.Windows.Forms.Label();
			this.label25 = new System.Windows.Forms.Label();
			this.vertexScaleLabel = new System.Windows.Forms.Label();
			this.label22 = new System.Windows.Forms.Label();
			this.vertexScale = new System.Windows.Forms.TrackBar();
			this.cbSynchCameras = new System.Windows.Forms.CheckBox();
			this.showtexturesizes = new System.Windows.Forms.CheckBox();
			this.scriptontop = new System.Windows.Forms.CheckBox();
			this.zoomfactor = new System.Windows.Forms.TrackBar();
			this.zoomfactorlabel = new System.Windows.Forms.Label();
			this.label19 = new System.Windows.Forms.Label();
			this.autoscrollspeed = new System.Windows.Forms.TrackBar();
			this.autoscrollspeedlabel = new System.Windows.Forms.Label();
			this.label15 = new System.Windows.Forms.Label();
			this.previewsize = new System.Windows.Forms.TrackBar();
			this.previewsizelabel = new System.Windows.Forms.Label();
			this.label12 = new System.Windows.Forms.Label();
			this.label14 = new System.Windows.Forms.Label();
			this.defaultviewmode = new System.Windows.Forms.ComboBox();
			this.keyusedlabel = new System.Windows.Forms.Label();
			this.colorsgroup1 = new System.Windows.Forms.GroupBox();
			this.color3dFloors = new CodeImp.DoomBuilder.Controls.ColorControl();
			this.colorInfo = new CodeImp.DoomBuilder.Controls.ColorControl();
			this.colorMD3 = new CodeImp.DoomBuilder.Controls.ColorControl();
			this.doublesidedalpha = new System.Windows.Forms.TrackBar();
			this.colorgrid64 = new CodeImp.DoomBuilder.Controls.ColorControl();
			this.colorgrid = new CodeImp.DoomBuilder.Controls.ColorControl();
			this.colorindication = new CodeImp.DoomBuilder.Controls.ColorControl();
			this.colorbackcolor = new CodeImp.DoomBuilder.Controls.ColorControl();
			this.label2 = new System.Windows.Forms.Label();
			this.doublesidedalphalabel = new System.Windows.Forms.Label();
			this.colorselection = new CodeImp.DoomBuilder.Controls.ColorControl();
			this.colorvertices = new CodeImp.DoomBuilder.Controls.ColorControl();
			this.colorhighlight = new CodeImp.DoomBuilder.Controls.ColorControl();
			this.colorlinedefs = new CodeImp.DoomBuilder.Controls.ColorControl();
			this.cbStretchView = new System.Windows.Forms.CheckBox();
			this.qualitydisplay = new System.Windows.Forms.CheckBox();
			this.cancel = new System.Windows.Forms.Button();
			this.apply = new System.Windows.Forms.Button();
			this.tabs = new System.Windows.Forms.TabControl();
			this.tabinterface = new System.Windows.Forms.TabPage();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.resetscreenshotsdir = new System.Windows.Forms.Button();
			this.browsescreenshotsdir = new System.Windows.Forms.Button();
			this.screenshotsfolderpath = new System.Windows.Forms.TextBox();
			this.groupBox5 = new System.Windows.Forms.GroupBox();
			this.toolbar_gzdoom = new System.Windows.Forms.CheckBox();
			this.toolbar_file = new System.Windows.Forms.CheckBox();
			this.toolbar_testing = new System.Windows.Forms.CheckBox();
			this.toolbar_geometry = new System.Windows.Forms.CheckBox();
			this.toolbar_viewmodes = new System.Windows.Forms.CheckBox();
			this.toolbar_filter = new System.Windows.Forms.CheckBox();
			this.toolbar_prefabs = new System.Windows.Forms.CheckBox();
			this.toolbar_copy = new System.Windows.Forms.CheckBox();
			this.toolbar_undo = new System.Windows.Forms.CheckBox();
			this.toolbar_script = new System.Windows.Forms.CheckBox();
			this.groupBox4 = new System.Windows.Forms.GroupBox();
			this.collapsedockers = new System.Windows.Forms.CheckBox();
			this.dockersposition = new System.Windows.Forms.ComboBox();
			this.label17 = new System.Windows.Forms.Label();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.vertexScale3D = new System.Windows.Forms.TrackBar();
			this.vertexScale3DLabel = new System.Windows.Forms.Label();
			this.label26 = new System.Windows.Forms.Label();
			this.viewdistance = new System.Windows.Forms.TrackBar();
			this.movespeed = new System.Windows.Forms.TrackBar();
			this.mousespeed = new System.Windows.Forms.TrackBar();
			this.fieldofview = new System.Windows.Forms.TrackBar();
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
			this.bClearActionFilter = new System.Windows.Forms.Button();
			this.tbFilterActions = new System.Windows.Forms.TextBox();
			this.label24 = new System.Windows.Forms.Label();
			this.listactions = new System.Windows.Forms.ListView();
			this.columncontrolaction = new System.Windows.Forms.ColumnHeader();
			this.columncontrolkey = new System.Windows.Forms.ColumnHeader();
			this.actioncontrolpanel = new System.Windows.Forms.GroupBox();
			this.actiondescription = new System.Windows.Forms.TextBox();
			this.keyusedlist = new System.Windows.Forms.ListBox();
			this.disregardshiftlabel = new System.Windows.Forms.Label();
			this.actioncontrol = new System.Windows.Forms.ComboBox();
			this.actiontitle = new System.Windows.Forms.Label();
			this.actioncontrolclear = new System.Windows.Forms.Button();
			this.actionkey = new System.Windows.Forms.TextBox();
			this.tabcolors = new System.Windows.Forms.TabPage();
			this.appearancegroup1 = new System.Windows.Forms.GroupBox();
			this.capitalizetexturenames = new System.Windows.Forms.CheckBox();
			this.cbMarkExtraFloors = new System.Windows.Forms.CheckBox();
			this.cbOldHighlightMode = new System.Windows.Forms.CheckBox();
			this.labelDynLightIntensity = new System.Windows.Forms.Label();
			this.tbDynLightIntensity = new System.Windows.Forms.TrackBar();
			this.labelDynLightSize = new System.Windows.Forms.Label();
			this.tbDynLightSize = new System.Windows.Forms.TrackBar();
			this.labelDynLightCount = new System.Windows.Forms.Label();
			this.tbDynLightCount = new System.Windows.Forms.TrackBar();
			this.animatevisualselection = new System.Windows.Forms.CheckBox();
			this.blackbrowsers = new System.Windows.Forms.CheckBox();
			this.visualbilinear = new System.Windows.Forms.CheckBox();
			this.classicbilinear = new System.Windows.Forms.CheckBox();
			this.imagebrightnesslabel = new System.Windows.Forms.Label();
			this.imagebrightness = new System.Windows.Forms.TrackBar();
			this.colorsgroup3 = new System.Windows.Forms.GroupBox();
			this.snippetsallmanstyle = new System.Windows.Forms.CheckBox();
			this.scripttabwidth = new CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox();
			this.scriptautoindent = new System.Windows.Forms.CheckBox();
			this.label10 = new System.Windows.Forms.Label();
			this.fontpreview = new System.Windows.Forms.RichTextBox();
			this.scriptfontsize = new System.Windows.Forms.ComboBox();
			this.label8 = new System.Windows.Forms.Label();
			this.scriptfontbold = new System.Windows.Forms.CheckBox();
			this.scriptfontname = new System.Windows.Forms.ComboBox();
			this.label3 = new System.Windows.Forms.Label();
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
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.browseScreenshotsFolderDialog = new System.Windows.Forms.FolderBrowserDialog();
			label7 = new System.Windows.Forms.Label();
			label6 = new System.Windows.Forms.Label();
			label5 = new System.Windows.Forms.Label();
			groupBox1 = new System.Windows.Forms.GroupBox();
			label1 = new System.Windows.Forms.Label();
			label18 = new System.Windows.Forms.Label();
			label20 = new System.Windows.Forms.Label();
			label21 = new System.Windows.Forms.Label();
			groupBox1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.recentFiles)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.vertexScale)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.zoomfactor)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.autoscrollspeed)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.previewsize)).BeginInit();
			this.colorsgroup1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.doublesidedalpha)).BeginInit();
			this.tabs.SuspendLayout();
			this.tabinterface.SuspendLayout();
			this.groupBox3.SuspendLayout();
			this.groupBox5.SuspendLayout();
			this.groupBox4.SuspendLayout();
			this.groupBox2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.vertexScale3D)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.viewdistance)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.movespeed)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.mousespeed)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.fieldofview)).BeginInit();
			this.tabkeys.SuspendLayout();
			this.actioncontrolpanel.SuspendLayout();
			this.tabcolors.SuspendLayout();
			this.appearancegroup1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.tbDynLightIntensity)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.tbDynLightSize)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.tbDynLightCount)).BeginInit();
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
			label7.Size = new System.Drawing.Size(182, 13);
			label7.TabIndex = 7;
			label7.Text = "Or select a special input control here:";
			// 
			// label6
			// 
			label6.AutoSize = true;
			label6.Location = new System.Drawing.Point(20, 30);
			label6.Name = "label6";
			label6.Size = new System.Drawing.Size(40, 13);
			label6.TabIndex = 2;
			label6.Text = "Action:";
			// 
			// label5
			// 
			label5.AutoSize = true;
			label5.Location = new System.Drawing.Point(20, 122);
			label5.Name = "label5";
			label5.Size = new System.Drawing.Size(195, 13);
			label5.TabIndex = 4;
			label5.Text = "Press the desired key combination here:";
			// 
			// groupBox1
			// 
			groupBox1.Controls.Add(this.checkforupdates);
			groupBox1.Controls.Add(this.cbStoreEditTab);
			groupBox1.Controls.Add(this.locatetexturegroup);
			groupBox1.Controls.Add(this.recentFiles);
			groupBox1.Controls.Add(this.labelRecentFiles);
			groupBox1.Controls.Add(this.label25);
			groupBox1.Controls.Add(this.vertexScaleLabel);
			groupBox1.Controls.Add(this.label22);
			groupBox1.Controls.Add(this.vertexScale);
			groupBox1.Controls.Add(this.cbSynchCameras);
			groupBox1.Controls.Add(this.showtexturesizes);
			groupBox1.Controls.Add(this.scriptontop);
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
			groupBox1.Size = new System.Drawing.Size(331, 438);
			groupBox1.TabIndex = 0;
			groupBox1.TabStop = false;
			groupBox1.Text = " Options ";
			// 
			// checkforupdates
			// 
			this.checkforupdates.AutoSize = true;
			this.checkforupdates.Location = new System.Drawing.Point(32, 392);
			this.checkforupdates.Name = "checkforupdates";
			this.checkforupdates.Size = new System.Drawing.Size(160, 17);
			this.checkforupdates.TabIndex = 51;
			this.checkforupdates.Text = "Check for updates at startup";
			this.checkforupdates.UseVisualStyleBackColor = true;
			// 
			// cbStoreEditTab
			// 
			this.cbStoreEditTab.AutoSize = true;
			this.cbStoreEditTab.Location = new System.Drawing.Point(32, 371);
			this.cbStoreEditTab.Name = "cbStoreEditTab";
			this.cbStoreEditTab.Size = new System.Drawing.Size(203, 17);
			this.cbStoreEditTab.TabIndex = 50;
			this.cbStoreEditTab.Text = "Edit windows remember selected tabs";
			this.cbStoreEditTab.UseVisualStyleBackColor = true;
			// 
			// locatetexturegroup
			// 
			this.locatetexturegroup.AutoSize = true;
			this.locatetexturegroup.Location = new System.Drawing.Point(32, 329);
			this.locatetexturegroup.Name = "locatetexturegroup";
			this.locatetexturegroup.Size = new System.Drawing.Size(267, 17);
			this.locatetexturegroup.TabIndex = 49;
			this.locatetexturegroup.Text = "Select texture group when opening image browsers";
			this.toolTip1.SetToolTip(this.locatetexturegroup, "When enabled, the group current texture belongs to\r\nwill be selected when opening" +
					" image browsers.\r\nWhen disabled, \"All\" texture group will be selected.");
			this.locatetexturegroup.UseVisualStyleBackColor = true;
			// 
			// recentFiles
			// 
			this.recentFiles.BackColor = System.Drawing.SystemColors.Window;
			this.recentFiles.LargeChange = 1;
			this.recentFiles.Location = new System.Drawing.Point(127, 233);
			this.recentFiles.Maximum = 25;
			this.recentFiles.Minimum = 8;
			this.recentFiles.Name = "recentFiles";
			this.recentFiles.Size = new System.Drawing.Size(116, 45);
			this.recentFiles.TabIndex = 46;
			this.recentFiles.TickStyle = System.Windows.Forms.TickStyle.Both;
			this.recentFiles.Value = 8;
			this.recentFiles.ValueChanged += new System.EventHandler(this.recentFiles_ValueChanged);
			// 
			// labelRecentFiles
			// 
			this.labelRecentFiles.AutoSize = true;
			this.labelRecentFiles.Location = new System.Drawing.Point(249, 245);
			this.labelRecentFiles.Name = "labelRecentFiles";
			this.labelRecentFiles.Size = new System.Drawing.Size(13, 13);
			this.labelRecentFiles.TabIndex = 48;
			this.labelRecentFiles.Text = "8";
			// 
			// label25
			// 
			this.label25.AutoSize = true;
			this.label25.Location = new System.Drawing.Point(31, 246);
			this.label25.Name = "label25";
			this.label25.Size = new System.Drawing.Size(87, 13);
			this.label25.TabIndex = 47;
			this.label25.Text = "Max. recent files:";
			this.toolTip1.SetToolTip(this.label25, "Controls how many recent files \r\nare shown in the \"File\" menu.");
			// 
			// vertexScaleLabel
			// 
			this.vertexScaleLabel.AutoSize = true;
			this.vertexScaleLabel.Location = new System.Drawing.Point(249, 198);
			this.vertexScaleLabel.Name = "vertexScaleLabel";
			this.vertexScaleLabel.Size = new System.Drawing.Size(74, 13);
			this.vertexScaleLabel.TabIndex = 45;
			this.vertexScaleLabel.Text = "100% (default)";
			// 
			// label22
			// 
			this.label22.AutoSize = true;
			this.label22.Location = new System.Drawing.Point(26, 198);
			this.label22.Name = "label22";
			this.label22.Size = new System.Drawing.Size(91, 13);
			this.label22.TabIndex = 44;
			this.label22.Text = "Vertex scale (2D):";
			this.toolTip1.SetToolTip(this.label22, "Sets the size of vertex handles\r\nin 2D modes");
			// 
			// vertexScale
			// 
			this.vertexScale.BackColor = System.Drawing.SystemColors.Window;
			this.vertexScale.LargeChange = 1;
			this.vertexScale.Location = new System.Drawing.Point(127, 186);
			this.vertexScale.Minimum = 1;
			this.vertexScale.Name = "vertexScale";
			this.vertexScale.Size = new System.Drawing.Size(116, 45);
			this.vertexScale.TabIndex = 43;
			this.vertexScale.TickStyle = System.Windows.Forms.TickStyle.Both;
			this.vertexScale.Value = 1;
			this.vertexScale.ValueChanged += new System.EventHandler(this.vertexScale_ValueChanged);
			// 
			// cbSynchCameras
			// 
			this.cbSynchCameras.AutoSize = true;
			this.cbSynchCameras.Location = new System.Drawing.Point(32, 350);
			this.cbSynchCameras.Name = "cbSynchCameras";
			this.cbSynchCameras.Size = new System.Drawing.Size(260, 17);
			this.cbSynchCameras.TabIndex = 42;
			this.cbSynchCameras.Text = "Sync camera position between 2D and 3D modes";
			this.cbSynchCameras.UseVisualStyleBackColor = true;
			// 
			// showtexturesizes
			// 
			this.showtexturesizes.AutoSize = true;
			this.showtexturesizes.Location = new System.Drawing.Point(32, 308);
			this.showtexturesizes.Name = "showtexturesizes";
			this.showtexturesizes.Size = new System.Drawing.Size(208, 17);
			this.showtexturesizes.TabIndex = 41;
			this.showtexturesizes.Text = "Show texture and flat sizes in browsers";
			this.showtexturesizes.UseVisualStyleBackColor = true;
			// 
			// scriptontop
			// 
			this.scriptontop.AutoSize = true;
			this.scriptontop.Location = new System.Drawing.Point(32, 287);
			this.scriptontop.Name = "scriptontop";
			this.scriptontop.Size = new System.Drawing.Size(227, 17);
			this.scriptontop.TabIndex = 40;
			this.scriptontop.Text = "Script Editor always on top of main window";
			this.scriptontop.UseVisualStyleBackColor = true;
			// 
			// zoomfactor
			// 
			this.zoomfactor.BackColor = System.Drawing.SystemColors.Window;
			this.zoomfactor.LargeChange = 1;
			this.zoomfactor.Location = new System.Drawing.Point(127, 139);
			this.zoomfactor.Minimum = 1;
			this.zoomfactor.Name = "zoomfactor";
			this.zoomfactor.Size = new System.Drawing.Size(116, 45);
			this.zoomfactor.TabIndex = 37;
			this.zoomfactor.TickStyle = System.Windows.Forms.TickStyle.Both;
			this.zoomfactor.Value = 3;
			this.zoomfactor.ValueChanged += new System.EventHandler(this.zoomfactor_ValueChanged);
			// 
			// zoomfactorlabel
			// 
			this.zoomfactorlabel.AutoSize = true;
			this.zoomfactorlabel.Location = new System.Drawing.Point(249, 151);
			this.zoomfactorlabel.Name = "zoomfactorlabel";
			this.zoomfactorlabel.Size = new System.Drawing.Size(27, 13);
			this.zoomfactorlabel.TabIndex = 39;
			this.zoomfactorlabel.Text = "30%";
			// 
			// label19
			// 
			this.label19.AutoSize = true;
			this.label19.Location = new System.Drawing.Point(52, 151);
			this.label19.Name = "label19";
			this.label19.Size = new System.Drawing.Size(67, 13);
			this.label19.TabIndex = 38;
			this.label19.Text = "Zoom factor:";
			// 
			// autoscrollspeed
			// 
			this.autoscrollspeed.BackColor = System.Drawing.SystemColors.Window;
			this.autoscrollspeed.LargeChange = 1;
			this.autoscrollspeed.Location = new System.Drawing.Point(127, 92);
			this.autoscrollspeed.Maximum = 5;
			this.autoscrollspeed.Name = "autoscrollspeed";
			this.autoscrollspeed.Size = new System.Drawing.Size(116, 45);
			this.autoscrollspeed.TabIndex = 2;
			this.autoscrollspeed.TickStyle = System.Windows.Forms.TickStyle.Both;
			this.autoscrollspeed.ValueChanged += new System.EventHandler(this.autoscrollspeed_ValueChanged);
			// 
			// autoscrollspeedlabel
			// 
			this.autoscrollspeedlabel.AutoSize = true;
			this.autoscrollspeedlabel.Location = new System.Drawing.Point(249, 104);
			this.autoscrollspeedlabel.Name = "autoscrollspeedlabel";
			this.autoscrollspeedlabel.Size = new System.Drawing.Size(21, 13);
			this.autoscrollspeedlabel.TabIndex = 36;
			this.autoscrollspeedlabel.Text = "Off";
			// 
			// label15
			// 
			this.label15.AutoSize = true;
			this.label15.Location = new System.Drawing.Point(29, 104);
			this.label15.Name = "label15";
			this.label15.Size = new System.Drawing.Size(88, 13);
			this.label15.TabIndex = 35;
			this.label15.Text = "Autoscroll speed:";
			// 
			// previewsize
			// 
			this.previewsize.BackColor = System.Drawing.SystemColors.Window;
			this.previewsize.LargeChange = 1;
			this.previewsize.Location = new System.Drawing.Point(127, 45);
			this.previewsize.Maximum = 5;
			this.previewsize.Name = "previewsize";
			this.previewsize.Size = new System.Drawing.Size(116, 45);
			this.previewsize.TabIndex = 1;
			this.previewsize.TickStyle = System.Windows.Forms.TickStyle.Both;
			this.previewsize.Value = 5;
			this.previewsize.ValueChanged += new System.EventHandler(this.previewsize_ValueChanged);
			// 
			// previewsizelabel
			// 
			this.previewsizelabel.AutoSize = true;
			this.previewsizelabel.Location = new System.Drawing.Point(249, 57);
			this.previewsizelabel.Name = "previewsizelabel";
			this.previewsizelabel.Size = new System.Drawing.Size(54, 13);
			this.previewsizelabel.TabIndex = 33;
			this.previewsizelabel.Text = "128 x 128";
			// 
			// label12
			// 
			this.label12.AutoSize = true;
			this.label12.Location = new System.Drawing.Point(17, 57);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(100, 13);
			this.label12.TabIndex = 32;
			this.label12.Text = "Preview image size:";
			// 
			// label14
			// 
			this.label14.AutoSize = true;
			this.label14.Location = new System.Drawing.Point(50, 20);
			this.label14.Name = "label14";
			this.label14.Size = new System.Drawing.Size(69, 13);
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
			this.defaultviewmode.Location = new System.Drawing.Point(135, 17);
			this.defaultviewmode.Name = "defaultviewmode";
			this.defaultviewmode.Size = new System.Drawing.Size(145, 21);
			this.defaultviewmode.TabIndex = 0;
			// 
			// label1
			// 
			label1.AutoSize = true;
			label1.Location = new System.Drawing.Point(28, 29);
			label1.Name = "label1";
			label1.Size = new System.Drawing.Size(143, 13);
			label1.TabIndex = 20;
			label1.Text = "Texture and Flats brightness:";
			label1.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// label18
			// 
			label18.AutoSize = true;
			label18.Location = new System.Drawing.Point(22, 63);
			label18.Name = "label18";
			label18.Size = new System.Drawing.Size(147, 13);
			label18.TabIndex = 25;
			label18.Text = "Max. dynamic lights to render:";
			label18.TextAlign = System.Drawing.ContentAlignment.TopRight;
			this.toolTip1.SetToolTip(label18, "Controlls how many dynamic lights could be \r\nrendered simultaneously in Visual mo" +
					"de ");
			// 
			// label20
			// 
			label20.AutoSize = true;
			label20.Location = new System.Drawing.Point(76, 97);
			label20.Name = "label20";
			label20.Size = new System.Drawing.Size(94, 13);
			label20.TabIndex = 28;
			label20.Text = "Dynamic light size:";
			label20.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// label21
			// 
			label21.AutoSize = true;
			label21.Location = new System.Drawing.Point(55, 131);
			label21.Name = "label21";
			label21.Size = new System.Drawing.Size(114, 13);
			label21.TabIndex = 31;
			label21.Text = "Dynamic light intensity:";
			label21.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// keyusedlabel
			// 
			this.keyusedlabel.AutoSize = true;
			this.keyusedlabel.Location = new System.Drawing.Point(20, 287);
			this.keyusedlabel.Name = "keyusedlabel";
			this.keyusedlabel.Size = new System.Drawing.Size(216, 13);
			this.keyusedlabel.TabIndex = 10;
			this.keyusedlabel.Text = "Key combination also used by these actions:";
			this.keyusedlabel.Visible = false;
			// 
			// colorsgroup1
			// 
			this.colorsgroup1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)));
			this.colorsgroup1.Controls.Add(this.color3dFloors);
			this.colorsgroup1.Controls.Add(this.colorInfo);
			this.colorsgroup1.Controls.Add(this.colorMD3);
			this.colorsgroup1.Controls.Add(this.doublesidedalpha);
			this.colorsgroup1.Controls.Add(this.colorgrid64);
			this.colorsgroup1.Controls.Add(this.colorgrid);
			this.colorsgroup1.Controls.Add(this.colorindication);
			this.colorsgroup1.Controls.Add(this.colorbackcolor);
			this.colorsgroup1.Controls.Add(this.label2);
			this.colorsgroup1.Controls.Add(this.doublesidedalphalabel);
			this.colorsgroup1.Controls.Add(this.colorselection);
			this.colorsgroup1.Controls.Add(this.colorvertices);
			this.colorsgroup1.Controls.Add(this.colorhighlight);
			this.colorsgroup1.Controls.Add(this.colorlinedefs);
			this.colorsgroup1.Location = new System.Drawing.Point(8, 8);
			this.colorsgroup1.Name = "colorsgroup1";
			this.colorsgroup1.Size = new System.Drawing.Size(203, 493);
			this.colorsgroup1.TabIndex = 0;
			this.colorsgroup1.TabStop = false;
			this.colorsgroup1.Text = " Display ";
			this.colorsgroup1.Visible = false;
			// 
			// color3dFloors
			// 
			this.color3dFloors.BackColor = System.Drawing.Color.Transparent;
			this.color3dFloors.Label = "3D Floors:";
			this.color3dFloors.Location = new System.Drawing.Point(15, 286);
			this.color3dFloors.MaximumSize = new System.Drawing.Size(10000, 23);
			this.color3dFloors.MinimumSize = new System.Drawing.Size(100, 23);
			this.color3dFloors.Name = "color3dFloors";
			this.color3dFloors.Size = new System.Drawing.Size(168, 23);
			this.color3dFloors.TabIndex = 24;
			// 
			// colorInfo
			// 
			this.colorInfo.BackColor = System.Drawing.Color.Transparent;
			this.colorInfo.Label = "Event lines:";
			this.colorInfo.Location = new System.Drawing.Point(15, 260);
			this.colorInfo.MaximumSize = new System.Drawing.Size(10000, 23);
			this.colorInfo.MinimumSize = new System.Drawing.Size(100, 23);
			this.colorInfo.Name = "colorInfo";
			this.colorInfo.Size = new System.Drawing.Size(168, 23);
			this.colorInfo.TabIndex = 19;
			// 
			// colorMD3
			// 
			this.colorMD3.BackColor = System.Drawing.Color.Transparent;
			this.colorMD3.Label = "Model wireframe:";
			this.colorMD3.Location = new System.Drawing.Point(15, 233);
			this.colorMD3.MaximumSize = new System.Drawing.Size(10000, 23);
			this.colorMD3.MinimumSize = new System.Drawing.Size(100, 23);
			this.colorMD3.Name = "colorMD3";
			this.colorMD3.Size = new System.Drawing.Size(168, 23);
			this.colorMD3.TabIndex = 17;
			// 
			// doublesidedalpha
			// 
			this.doublesidedalpha.BackColor = System.Drawing.SystemColors.Window;
			this.doublesidedalpha.LargeChange = 3;
			this.doublesidedalpha.Location = new System.Drawing.Point(11, 349);
			this.doublesidedalpha.Name = "doublesidedalpha";
			this.doublesidedalpha.Size = new System.Drawing.Size(130, 45);
			this.doublesidedalpha.TabIndex = 2;
			this.doublesidedalpha.TickStyle = System.Windows.Forms.TickStyle.TopLeft;
			this.doublesidedalpha.ValueChanged += new System.EventHandler(this.doublesidedalpha_ValueChanged);
			// 
			// colorgrid64
			// 
			this.colorgrid64.BackColor = System.Drawing.Color.Transparent;
			this.colorgrid64.Label = "64 Block grid:";
			this.colorgrid64.Location = new System.Drawing.Point(15, 206);
			this.colorgrid64.MaximumSize = new System.Drawing.Size(10000, 23);
			this.colorgrid64.MinimumSize = new System.Drawing.Size(100, 23);
			this.colorgrid64.Name = "colorgrid64";
			this.colorgrid64.Size = new System.Drawing.Size(168, 23);
			this.colorgrid64.TabIndex = 9;
			// 
			// colorgrid
			// 
			this.colorgrid.BackColor = System.Drawing.Color.Transparent;
			this.colorgrid.Label = "Custom grid:";
			this.colorgrid.Location = new System.Drawing.Point(15, 179);
			this.colorgrid.MaximumSize = new System.Drawing.Size(10000, 23);
			this.colorgrid.MinimumSize = new System.Drawing.Size(100, 23);
			this.colorgrid.Name = "colorgrid";
			this.colorgrid.Size = new System.Drawing.Size(168, 23);
			this.colorgrid.TabIndex = 8;
			// 
			// colorindication
			// 
			this.colorindication.BackColor = System.Drawing.Color.Transparent;
			this.colorindication.Label = "Indications:";
			this.colorindication.Location = new System.Drawing.Point(15, 152);
			this.colorindication.MaximumSize = new System.Drawing.Size(10000, 23);
			this.colorindication.MinimumSize = new System.Drawing.Size(100, 23);
			this.colorindication.Name = "colorindication";
			this.colorindication.Size = new System.Drawing.Size(168, 23);
			this.colorindication.TabIndex = 7;
			// 
			// colorbackcolor
			// 
			this.colorbackcolor.BackColor = System.Drawing.Color.Transparent;
			this.colorbackcolor.Label = "Background:";
			this.colorbackcolor.Location = new System.Drawing.Point(15, 17);
			this.colorbackcolor.MaximumSize = new System.Drawing.Size(10000, 23);
			this.colorbackcolor.MinimumSize = new System.Drawing.Size(100, 23);
			this.colorbackcolor.Name = "colorbackcolor";
			this.colorbackcolor.Size = new System.Drawing.Size(168, 23);
			this.colorbackcolor.TabIndex = 0;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(14, 328);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(141, 13);
			this.label2.TabIndex = 14;
			this.label2.Text = "Passable lines transparency:";
			this.label2.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// doublesidedalphalabel
			// 
			this.doublesidedalphalabel.AutoSize = true;
			this.doublesidedalphalabel.Location = new System.Drawing.Point(147, 361);
			this.doublesidedalphalabel.Name = "doublesidedalphalabel";
			this.doublesidedalphalabel.Size = new System.Drawing.Size(21, 13);
			this.doublesidedalphalabel.TabIndex = 16;
			this.doublesidedalphalabel.Text = "0%";
			// 
			// colorselection
			// 
			this.colorselection.BackColor = System.Drawing.Color.Transparent;
			this.colorselection.Label = "Selection:";
			this.colorselection.Location = new System.Drawing.Point(15, 125);
			this.colorselection.MaximumSize = new System.Drawing.Size(10000, 23);
			this.colorselection.MinimumSize = new System.Drawing.Size(100, 23);
			this.colorselection.Name = "colorselection";
			this.colorselection.Size = new System.Drawing.Size(168, 23);
			this.colorselection.TabIndex = 6;
			// 
			// colorvertices
			// 
			this.colorvertices.BackColor = System.Drawing.Color.Transparent;
			this.colorvertices.Label = "Vertices:";
			this.colorvertices.Location = new System.Drawing.Point(15, 44);
			this.colorvertices.MaximumSize = new System.Drawing.Size(10000, 23);
			this.colorvertices.MinimumSize = new System.Drawing.Size(100, 23);
			this.colorvertices.Name = "colorvertices";
			this.colorvertices.Size = new System.Drawing.Size(168, 23);
			this.colorvertices.TabIndex = 1;
			// 
			// colorhighlight
			// 
			this.colorhighlight.BackColor = System.Drawing.Color.Transparent;
			this.colorhighlight.Label = "Highlight:";
			this.colorhighlight.Location = new System.Drawing.Point(15, 98);
			this.colorhighlight.MaximumSize = new System.Drawing.Size(10000, 23);
			this.colorhighlight.MinimumSize = new System.Drawing.Size(100, 23);
			this.colorhighlight.Name = "colorhighlight";
			this.colorhighlight.Size = new System.Drawing.Size(168, 23);
			this.colorhighlight.TabIndex = 5;
			// 
			// colorlinedefs
			// 
			this.colorlinedefs.BackColor = System.Drawing.Color.Transparent;
			this.colorlinedefs.Label = "Common lines:";
			this.colorlinedefs.Location = new System.Drawing.Point(15, 71);
			this.colorlinedefs.MaximumSize = new System.Drawing.Size(10000, 23);
			this.colorlinedefs.MinimumSize = new System.Drawing.Size(100, 23);
			this.colorlinedefs.Name = "colorlinedefs";
			this.colorlinedefs.Size = new System.Drawing.Size(168, 23);
			this.colorlinedefs.TabIndex = 2;
			// 
			// cbStretchView
			// 
			this.cbStretchView.AutoSize = true;
			this.cbStretchView.Location = new System.Drawing.Point(236, 170);
			this.cbStretchView.Name = "cbStretchView";
			this.cbStretchView.Size = new System.Drawing.Size(172, 17);
			this.cbStretchView.TabIndex = 18;
			this.cbStretchView.Text = "Stretched view in visual modes";
			this.toolTip1.SetToolTip(this.cbStretchView, "When enabled, visual mode will emulate \r\n(G)ZDoom\'s way of rendering by increasin" +
					"g\r\nvertical scale of the world geometry and \r\nsprites by 15%.\r\n");
			this.cbStretchView.UseVisualStyleBackColor = true;
			// 
			// qualitydisplay
			// 
			this.qualitydisplay.AutoSize = true;
			this.qualitydisplay.Location = new System.Drawing.Point(25, 191);
			this.qualitydisplay.Name = "qualitydisplay";
			this.qualitydisplay.Size = new System.Drawing.Size(128, 17);
			this.qualitydisplay.TabIndex = 7;
			this.qualitydisplay.Text = "High quality rendering";
			this.qualitydisplay.UseVisualStyleBackColor = true;
			// 
			// cancel
			// 
			this.cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancel.Location = new System.Drawing.Point(587, 557);
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
			this.apply.Location = new System.Drawing.Point(469, 557);
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
			this.tabs.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.tabs.ItemSize = new System.Drawing.Size(110, 19);
			this.tabs.Location = new System.Drawing.Point(11, 13);
			this.tabs.Name = "tabs";
			this.tabs.Padding = new System.Drawing.Point(24, 3);
			this.tabs.SelectedIndex = 0;
			this.tabs.Size = new System.Drawing.Size(690, 536);
			this.tabs.TabIndex = 0;
			this.tabs.SelectedIndexChanged += new System.EventHandler(this.tabs_SelectedIndexChanged);
			// 
			// tabinterface
			// 
			this.tabinterface.Controls.Add(this.groupBox3);
			this.tabinterface.Controls.Add(this.groupBox5);
			this.tabinterface.Controls.Add(this.groupBox4);
			this.tabinterface.Controls.Add(this.groupBox2);
			this.tabinterface.Controls.Add(groupBox1);
			this.tabinterface.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.tabinterface.Location = new System.Drawing.Point(4, 23);
			this.tabinterface.Name = "tabinterface";
			this.tabinterface.Padding = new System.Windows.Forms.Padding(5);
			this.tabinterface.Size = new System.Drawing.Size(682, 509);
			this.tabinterface.TabIndex = 0;
			this.tabinterface.Text = "Interface";
			this.tabinterface.UseVisualStyleBackColor = true;
			// 
			// groupBox3
			// 
			this.groupBox3.Controls.Add(this.resetscreenshotsdir);
			this.groupBox3.Controls.Add(this.browsescreenshotsdir);
			this.groupBox3.Controls.Add(this.screenshotsfolderpath);
			this.groupBox3.Location = new System.Drawing.Point(345, 449);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(331, 48);
			this.groupBox3.TabIndex = 5;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = " Screenshots Folder ";
			// 
			// resetscreenshotsdir
			// 
			this.resetscreenshotsdir.Image = global::CodeImp.DoomBuilder.Properties.Resources.Reset;
			this.resetscreenshotsdir.Location = new System.Drawing.Point(301, 17);
			this.resetscreenshotsdir.Name = "resetscreenshotsdir";
			this.resetscreenshotsdir.Size = new System.Drawing.Size(24, 24);
			this.resetscreenshotsdir.TabIndex = 2;
			this.toolTip1.SetToolTip(this.resetscreenshotsdir, "Use Default Screenshots Folder");
			this.resetscreenshotsdir.UseVisualStyleBackColor = true;
			this.resetscreenshotsdir.Click += new System.EventHandler(this.resetscreenshotsdir_Click);
			// 
			// browsescreenshotsdir
			// 
			this.browsescreenshotsdir.Image = global::CodeImp.DoomBuilder.Properties.Resources.FolderExplore;
			this.browsescreenshotsdir.Location = new System.Drawing.Point(275, 17);
			this.browsescreenshotsdir.Name = "browsescreenshotsdir";
			this.browsescreenshotsdir.Size = new System.Drawing.Size(24, 24);
			this.browsescreenshotsdir.TabIndex = 1;
			this.toolTip1.SetToolTip(this.browsescreenshotsdir, "Browse Screenshots Folder");
			this.browsescreenshotsdir.UseVisualStyleBackColor = true;
			this.browsescreenshotsdir.Click += new System.EventHandler(this.browsescreenshotsdir_Click);
			// 
			// screenshotsfolderpath
			// 
			this.screenshotsfolderpath.Location = new System.Drawing.Point(6, 19);
			this.screenshotsfolderpath.Name = "screenshotsfolderpath";
			this.screenshotsfolderpath.Size = new System.Drawing.Size(264, 20);
			this.screenshotsfolderpath.TabIndex = 0;
			// 
			// groupBox5
			// 
			this.groupBox5.Controls.Add(this.toolbar_gzdoom);
			this.groupBox5.Controls.Add(this.toolbar_file);
			this.groupBox5.Controls.Add(this.toolbar_testing);
			this.groupBox5.Controls.Add(this.toolbar_geometry);
			this.groupBox5.Controls.Add(this.toolbar_viewmodes);
			this.groupBox5.Controls.Add(this.toolbar_filter);
			this.groupBox5.Controls.Add(this.toolbar_prefabs);
			this.groupBox5.Controls.Add(this.toolbar_copy);
			this.groupBox5.Controls.Add(this.toolbar_undo);
			this.groupBox5.Controls.Add(this.toolbar_script);
			this.groupBox5.Location = new System.Drawing.Point(345, 321);
			this.groupBox5.Name = "groupBox5";
			this.groupBox5.Size = new System.Drawing.Size(331, 125);
			this.groupBox5.TabIndex = 4;
			this.groupBox5.TabStop = false;
			this.groupBox5.Text = " Toolbar Buttons ";
			// 
			// toolbar_gzdoom
			// 
			this.toolbar_gzdoom.AutoSize = true;
			this.toolbar_gzdoom.Location = new System.Drawing.Point(160, 102);
			this.toolbar_gzdoom.Name = "toolbar_gzdoom";
			this.toolbar_gzdoom.Size = new System.Drawing.Size(75, 17);
			this.toolbar_gzdoom.TabIndex = 50;
			this.toolbar_gzdoom.Text = "Rendering";
			this.toolbar_gzdoom.UseVisualStyleBackColor = true;
			// 
			// toolbar_file
			// 
			this.toolbar_file.AutoSize = true;
			this.toolbar_file.Location = new System.Drawing.Point(14, 22);
			this.toolbar_file.Name = "toolbar_file";
			this.toolbar_file.Size = new System.Drawing.Size(121, 17);
			this.toolbar_file.TabIndex = 49;
			this.toolbar_file.Text = "New / Open / Save";
			this.toolbar_file.UseVisualStyleBackColor = true;
			// 
			// toolbar_testing
			// 
			this.toolbar_testing.AutoSize = true;
			this.toolbar_testing.Location = new System.Drawing.Point(160, 82);
			this.toolbar_testing.Name = "toolbar_testing";
			this.toolbar_testing.Size = new System.Drawing.Size(61, 17);
			this.toolbar_testing.TabIndex = 48;
			this.toolbar_testing.Text = "Testing";
			this.toolbar_testing.UseVisualStyleBackColor = true;
			// 
			// toolbar_geometry
			// 
			this.toolbar_geometry.AutoSize = true;
			this.toolbar_geometry.Location = new System.Drawing.Point(160, 62);
			this.toolbar_geometry.Name = "toolbar_geometry";
			this.toolbar_geometry.Size = new System.Drawing.Size(92, 17);
			this.toolbar_geometry.TabIndex = 47;
			this.toolbar_geometry.Text = "Snap / Merge";
			this.toolbar_geometry.UseVisualStyleBackColor = true;
			// 
			// toolbar_viewmodes
			// 
			this.toolbar_viewmodes.AutoSize = true;
			this.toolbar_viewmodes.Location = new System.Drawing.Point(160, 42);
			this.toolbar_viewmodes.Name = "toolbar_viewmodes";
			this.toolbar_viewmodes.Size = new System.Drawing.Size(84, 17);
			this.toolbar_viewmodes.TabIndex = 46;
			this.toolbar_viewmodes.Text = "View Modes";
			this.toolbar_viewmodes.UseVisualStyleBackColor = true;
			// 
			// toolbar_filter
			// 
			this.toolbar_filter.AutoSize = true;
			this.toolbar_filter.Location = new System.Drawing.Point(160, 22);
			this.toolbar_filter.Name = "toolbar_filter";
			this.toolbar_filter.Size = new System.Drawing.Size(161, 17);
			this.toolbar_filter.TabIndex = 45;
			this.toolbar_filter.Text = "Things Filter / Linedef Colors";
			this.toolbar_filter.UseVisualStyleBackColor = true;
			// 
			// toolbar_prefabs
			// 
			this.toolbar_prefabs.AutoSize = true;
			this.toolbar_prefabs.Location = new System.Drawing.Point(14, 102);
			this.toolbar_prefabs.Name = "toolbar_prefabs";
			this.toolbar_prefabs.Size = new System.Drawing.Size(62, 17);
			this.toolbar_prefabs.TabIndex = 44;
			this.toolbar_prefabs.Text = "Prefabs";
			this.toolbar_prefabs.UseVisualStyleBackColor = true;
			// 
			// toolbar_copy
			// 
			this.toolbar_copy.AutoSize = true;
			this.toolbar_copy.Location = new System.Drawing.Point(14, 82);
			this.toolbar_copy.Name = "toolbar_copy";
			this.toolbar_copy.Size = new System.Drawing.Size(115, 17);
			this.toolbar_copy.TabIndex = 43;
			this.toolbar_copy.Text = "Cut / Copy / Paste";
			this.toolbar_copy.UseVisualStyleBackColor = true;
			// 
			// toolbar_undo
			// 
			this.toolbar_undo.AutoSize = true;
			this.toolbar_undo.Location = new System.Drawing.Point(14, 62);
			this.toolbar_undo.Name = "toolbar_undo";
			this.toolbar_undo.Size = new System.Drawing.Size(89, 17);
			this.toolbar_undo.TabIndex = 42;
			this.toolbar_undo.Text = "Undo / Redo";
			this.toolbar_undo.UseVisualStyleBackColor = true;
			// 
			// toolbar_script
			// 
			this.toolbar_script.AutoSize = true;
			this.toolbar_script.Location = new System.Drawing.Point(14, 42);
			this.toolbar_script.Name = "toolbar_script";
			this.toolbar_script.Size = new System.Drawing.Size(83, 17);
			this.toolbar_script.TabIndex = 41;
			this.toolbar_script.Text = "Script Editor";
			this.toolbar_script.UseVisualStyleBackColor = true;
			// 
			// groupBox4
			// 
			this.groupBox4.Controls.Add(this.collapsedockers);
			this.groupBox4.Controls.Add(this.dockersposition);
			this.groupBox4.Controls.Add(this.label17);
			this.groupBox4.Location = new System.Drawing.Point(8, 449);
			this.groupBox4.Name = "groupBox4";
			this.groupBox4.Size = new System.Drawing.Size(331, 48);
			this.groupBox4.TabIndex = 3;
			this.groupBox4.TabStop = false;
			this.groupBox4.Text = " Side Panels ";
			// 
			// collapsedockers
			// 
			this.collapsedockers.AutoSize = true;
			this.collapsedockers.Location = new System.Drawing.Point(188, 20);
			this.collapsedockers.Name = "collapsedockers";
			this.collapsedockers.Size = new System.Drawing.Size(71, 17);
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
			this.dockersposition.Location = new System.Drawing.Point(91, 18);
			this.dockersposition.Name = "dockersposition";
			this.dockersposition.Size = new System.Drawing.Size(85, 21);
			this.dockersposition.TabIndex = 1;
			// 
			// label17
			// 
			this.label17.AutoSize = true;
			this.label17.Location = new System.Drawing.Point(29, 21);
			this.label17.Name = "label17";
			this.label17.Size = new System.Drawing.Size(47, 13);
			this.label17.TabIndex = 0;
			this.label17.Text = "Position:";
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.vertexScale3D);
			this.groupBox2.Controls.Add(this.vertexScale3DLabel);
			this.groupBox2.Controls.Add(this.label26);
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
			this.groupBox2.Location = new System.Drawing.Point(345, 8);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(331, 307);
			this.groupBox2.TabIndex = 2;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = " Visual Modes ";
			// 
			// vertexScale3D
			// 
			this.vertexScale3D.BackColor = System.Drawing.SystemColors.Window;
			this.vertexScale3D.LargeChange = 1;
			this.vertexScale3D.Location = new System.Drawing.Point(116, 181);
			this.vertexScale3D.Maximum = 15;
			this.vertexScale3D.Minimum = 2;
			this.vertexScale3D.Name = "vertexScale3D";
			this.vertexScale3D.Size = new System.Drawing.Size(150, 45);
			this.vertexScale3D.TabIndex = 31;
			this.vertexScale3D.TickStyle = System.Windows.Forms.TickStyle.Both;
			this.vertexScale3D.Value = 10;
			this.vertexScale3D.ValueChanged += new System.EventHandler(this.vertexScale3D_ValueChanged);
			// 
			// vertexScale3DLabel
			// 
			this.vertexScale3DLabel.AutoSize = true;
			this.vertexScale3DLabel.Location = new System.Drawing.Point(272, 193);
			this.vertexScale3DLabel.Name = "vertexScale3DLabel";
			this.vertexScale3DLabel.Size = new System.Drawing.Size(33, 13);
			this.vertexScale3DLabel.TabIndex = 33;
			this.vertexScale3DLabel.Text = "100%";
			// 
			// label26
			// 
			this.label26.AutoSize = true;
			this.label26.Location = new System.Drawing.Point(16, 193);
			this.label26.Name = "label26";
			this.label26.Size = new System.Drawing.Size(91, 13);
			this.label26.TabIndex = 32;
			this.label26.Text = "Vertex scale (3D):";
			this.toolTip1.SetToolTip(this.label26, "Sets the size of vertex handles\r\nin 3D mode");
			// 
			// viewdistance
			// 
			this.viewdistance.BackColor = System.Drawing.SystemColors.Window;
			this.viewdistance.LargeChange = 2;
			this.viewdistance.Location = new System.Drawing.Point(116, 230);
			this.viewdistance.Maximum = 45;
			this.viewdistance.Minimum = 1;
			this.viewdistance.Name = "viewdistance";
			this.viewdistance.Size = new System.Drawing.Size(150, 45);
			this.viewdistance.TabIndex = 3;
			this.viewdistance.TickFrequency = 2;
			this.viewdistance.TickStyle = System.Windows.Forms.TickStyle.Both;
			this.viewdistance.Value = 1;
			this.viewdistance.ValueChanged += new System.EventHandler(this.viewdistance_ValueChanged);
			// 
			// movespeed
			// 
			this.movespeed.BackColor = System.Drawing.SystemColors.Window;
			this.movespeed.Location = new System.Drawing.Point(116, 131);
			this.movespeed.Maximum = 20;
			this.movespeed.Minimum = 1;
			this.movespeed.Name = "movespeed";
			this.movespeed.Size = new System.Drawing.Size(150, 45);
			this.movespeed.TabIndex = 2;
			this.movespeed.TickStyle = System.Windows.Forms.TickStyle.Both;
			this.movespeed.Value = 1;
			this.movespeed.ValueChanged += new System.EventHandler(this.movespeed_ValueChanged);
			// 
			// mousespeed
			// 
			this.mousespeed.BackColor = System.Drawing.SystemColors.Window;
			this.mousespeed.Location = new System.Drawing.Point(116, 79);
			this.mousespeed.Maximum = 20;
			this.mousespeed.Minimum = 1;
			this.mousespeed.Name = "mousespeed";
			this.mousespeed.Size = new System.Drawing.Size(150, 45);
			this.mousespeed.TabIndex = 1;
			this.mousespeed.TickStyle = System.Windows.Forms.TickStyle.Both;
			this.mousespeed.Value = 1;
			this.mousespeed.ValueChanged += new System.EventHandler(this.mousespeed_ValueChanged);
			// 
			// fieldofview
			// 
			this.fieldofview.BackColor = System.Drawing.SystemColors.Window;
			this.fieldofview.LargeChange = 1;
			this.fieldofview.Location = new System.Drawing.Point(116, 29);
			this.fieldofview.Maximum = 17;
			this.fieldofview.Minimum = 5;
			this.fieldofview.Name = "fieldofview";
			this.fieldofview.Size = new System.Drawing.Size(150, 45);
			this.fieldofview.TabIndex = 0;
			this.fieldofview.TickStyle = System.Windows.Forms.TickStyle.Both;
			this.fieldofview.Value = 5;
			this.fieldofview.ValueChanged += new System.EventHandler(this.fieldofview_ValueChanged);
			// 
			// viewdistancelabel
			// 
			this.viewdistancelabel.AutoSize = true;
			this.viewdistancelabel.Location = new System.Drawing.Point(272, 242);
			this.viewdistancelabel.Name = "viewdistancelabel";
			this.viewdistancelabel.Size = new System.Drawing.Size(42, 13);
			this.viewdistancelabel.TabIndex = 30;
			this.viewdistancelabel.Text = "200 mp";
			// 
			// label13
			// 
			this.label13.AutoSize = true;
			this.label13.Location = new System.Drawing.Point(30, 243);
			this.label13.Name = "label13";
			this.label13.Size = new System.Drawing.Size(76, 13);
			this.label13.TabIndex = 28;
			this.label13.Text = "View distance:";
			// 
			// invertyaxis
			// 
			this.invertyaxis.AutoSize = true;
			this.invertyaxis.Location = new System.Drawing.Point(32, 282);
			this.invertyaxis.Name = "invertyaxis";
			this.invertyaxis.Size = new System.Drawing.Size(118, 17);
			this.invertyaxis.TabIndex = 4;
			this.invertyaxis.Text = "Invert mouse Y axis";
			this.invertyaxis.UseVisualStyleBackColor = true;
			// 
			// movespeedlabel
			// 
			this.movespeedlabel.AutoSize = true;
			this.movespeedlabel.Location = new System.Drawing.Point(272, 143);
			this.movespeedlabel.Name = "movespeedlabel";
			this.movespeedlabel.Size = new System.Drawing.Size(25, 13);
			this.movespeedlabel.TabIndex = 25;
			this.movespeedlabel.Text = "100";
			// 
			// label11
			// 
			this.label11.AutoSize = true;
			this.label11.Location = new System.Drawing.Point(41, 143);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(69, 13);
			this.label11.TabIndex = 23;
			this.label11.Text = "Move speed:";
			// 
			// mousespeedlabel
			// 
			this.mousespeedlabel.AutoSize = true;
			this.mousespeedlabel.Location = new System.Drawing.Point(272, 91);
			this.mousespeedlabel.Name = "mousespeedlabel";
			this.mousespeedlabel.Size = new System.Drawing.Size(25, 13);
			this.mousespeedlabel.TabIndex = 22;
			this.mousespeedlabel.Text = "100";
			// 
			// label9
			// 
			this.label9.AutoSize = true;
			this.label9.Location = new System.Drawing.Point(35, 91);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(74, 13);
			this.label9.TabIndex = 20;
			this.label9.Text = "Mouse speed:";
			// 
			// fieldofviewlabel
			// 
			this.fieldofviewlabel.AutoSize = true;
			this.fieldofviewlabel.Location = new System.Drawing.Point(272, 41);
			this.fieldofviewlabel.Name = "fieldofviewlabel";
			this.fieldofviewlabel.Size = new System.Drawing.Size(23, 13);
			this.fieldofviewlabel.TabIndex = 19;
			this.fieldofviewlabel.Text = "50°";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(38, 41);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(69, 13);
			this.label4.TabIndex = 17;
			this.label4.Text = "Field of view:";
			// 
			// tabkeys
			// 
			this.tabkeys.Controls.Add(this.bClearActionFilter);
			this.tabkeys.Controls.Add(this.tbFilterActions);
			this.tabkeys.Controls.Add(this.label24);
			this.tabkeys.Controls.Add(this.listactions);
			this.tabkeys.Controls.Add(this.actioncontrolpanel);
			this.tabkeys.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.tabkeys.Location = new System.Drawing.Point(4, 23);
			this.tabkeys.Name = "tabkeys";
			this.tabkeys.Padding = new System.Windows.Forms.Padding(3);
			this.tabkeys.Size = new System.Drawing.Size(682, 509);
			this.tabkeys.TabIndex = 1;
			this.tabkeys.Text = "Controls";
			this.tabkeys.UseVisualStyleBackColor = true;
			// 
			// bClearActionFilter
			// 
			this.bClearActionFilter.Image = global::CodeImp.DoomBuilder.Properties.Resources.SearchClear;
			this.bClearActionFilter.Location = new System.Drawing.Point(227, 10);
			this.bClearActionFilter.Name = "bClearActionFilter";
			this.bClearActionFilter.Size = new System.Drawing.Size(26, 25);
			this.bClearActionFilter.TabIndex = 12;
			this.bClearActionFilter.TabStop = false;
			this.bClearActionFilter.UseVisualStyleBackColor = true;
			this.bClearActionFilter.Click += new System.EventHandler(this.bClearActionFilter_Click);
			// 
			// tbFilterActions
			// 
			this.tbFilterActions.Location = new System.Drawing.Point(55, 13);
			this.tbFilterActions.Name = "tbFilterActions";
			this.tbFilterActions.Size = new System.Drawing.Size(166, 20);
			this.tbFilterActions.TabIndex = 11;
			this.tbFilterActions.TabStop = false;
			this.tbFilterActions.TextChanged += new System.EventHandler(this.tbFilterActions_TextChanged);
			// 
			// label24
			// 
			this.label24.AutoSize = true;
			this.label24.Location = new System.Drawing.Point(17, 16);
			this.label24.Name = "label24";
			this.label24.Size = new System.Drawing.Size(32, 13);
			this.label24.TabIndex = 10;
			this.label24.Text = "Filter:";
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
			this.listactions.Location = new System.Drawing.Point(11, 42);
			this.listactions.Margin = new System.Windows.Forms.Padding(8, 9, 8, 9);
			this.listactions.MultiSelect = false;
			this.listactions.Name = "listactions";
			this.listactions.Size = new System.Drawing.Size(352, 458);
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
			this.columncontrolaction.Width = 200;
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
			this.actioncontrolpanel.Controls.Add(this.actiondescription);
			this.actioncontrolpanel.Controls.Add(this.keyusedlist);
			this.actioncontrolpanel.Controls.Add(this.keyusedlabel);
			this.actioncontrolpanel.Controls.Add(this.disregardshiftlabel);
			this.actioncontrolpanel.Controls.Add(this.actioncontrol);
			this.actioncontrolpanel.Controls.Add(label7);
			this.actioncontrolpanel.Controls.Add(this.actiontitle);
			this.actioncontrolpanel.Controls.Add(this.actioncontrolclear);
			this.actioncontrolpanel.Controls.Add(label6);
			this.actioncontrolpanel.Controls.Add(this.actionkey);
			this.actioncontrolpanel.Controls.Add(label5);
			this.actioncontrolpanel.Enabled = false;
			this.actioncontrolpanel.Location = new System.Drawing.Point(377, 12);
			this.actioncontrolpanel.Margin = new System.Windows.Forms.Padding(6);
			this.actioncontrolpanel.Name = "actioncontrolpanel";
			this.actioncontrolpanel.Size = new System.Drawing.Size(282, 488);
			this.actioncontrolpanel.TabIndex = 9;
			this.actioncontrolpanel.TabStop = false;
			this.actioncontrolpanel.Text = " Action control ";
			// 
			// actiondescription
			// 
			this.actiondescription.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.actiondescription.Location = new System.Drawing.Point(20, 47);
			this.actiondescription.Multiline = true;
			this.actiondescription.Name = "actiondescription";
			this.actiondescription.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.actiondescription.Size = new System.Drawing.Size(256, 72);
			this.actiondescription.TabIndex = 12;
			// 
			// keyusedlist
			// 
			this.keyusedlist.BackColor = System.Drawing.SystemColors.Control;
			this.keyusedlist.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.keyusedlist.FormattingEnabled = true;
			this.keyusedlist.IntegralHeight = false;
			this.keyusedlist.Location = new System.Drawing.Point(23, 307);
			this.keyusedlist.Name = "keyusedlist";
			this.keyusedlist.SelectionMode = System.Windows.Forms.SelectionMode.None;
			this.keyusedlist.Size = new System.Drawing.Size(238, 115);
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
			this.actioncontrol.Size = new System.Drawing.Size(238, 21);
			this.actioncontrol.TabIndex = 8;
			this.actioncontrol.TabStop = false;
			this.actioncontrol.SelectedIndexChanged += new System.EventHandler(this.actioncontrol_SelectedIndexChanged);
			// 
			// actiontitle
			// 
			this.actiontitle.AutoSize = true;
			this.actiontitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.actiontitle.Location = new System.Drawing.Point(67, 30);
			this.actiontitle.Name = "actiontitle";
			this.actiontitle.Size = new System.Drawing.Size(176, 13);
			this.actiontitle.TabIndex = 1;
			this.actiontitle.Text = "(select an action from the list)";
			this.actiontitle.UseMnemonic = false;
			// 
			// actioncontrolclear
			// 
			this.actioncontrolclear.Image = global::CodeImp.DoomBuilder.Properties.Resources.SearchClear;
			this.actioncontrolclear.Location = new System.Drawing.Point(193, 137);
			this.actioncontrolclear.Name = "actioncontrolclear";
			this.actioncontrolclear.Size = new System.Drawing.Size(26, 25);
			this.actioncontrolclear.TabIndex = 6;
			this.actioncontrolclear.TabStop = false;
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
			// tabcolors
			// 
			this.tabcolors.Controls.Add(this.appearancegroup1);
			this.tabcolors.Controls.Add(this.colorsgroup3);
			this.tabcolors.Controls.Add(this.colorsgroup1);
			this.tabcolors.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.tabcolors.Location = new System.Drawing.Point(4, 23);
			this.tabcolors.Name = "tabcolors";
			this.tabcolors.Padding = new System.Windows.Forms.Padding(5);
			this.tabcolors.Size = new System.Drawing.Size(682, 509);
			this.tabcolors.TabIndex = 2;
			this.tabcolors.Text = "Appearance";
			this.tabcolors.UseVisualStyleBackColor = true;
			// 
			// appearancegroup1
			// 
			this.appearancegroup1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.appearancegroup1.Controls.Add(this.capitalizetexturenames);
			this.appearancegroup1.Controls.Add(this.cbMarkExtraFloors);
			this.appearancegroup1.Controls.Add(this.cbOldHighlightMode);
			this.appearancegroup1.Controls.Add(label21);
			this.appearancegroup1.Controls.Add(this.labelDynLightIntensity);
			this.appearancegroup1.Controls.Add(this.cbStretchView);
			this.appearancegroup1.Controls.Add(this.tbDynLightIntensity);
			this.appearancegroup1.Controls.Add(label20);
			this.appearancegroup1.Controls.Add(this.qualitydisplay);
			this.appearancegroup1.Controls.Add(this.labelDynLightSize);
			this.appearancegroup1.Controls.Add(this.tbDynLightSize);
			this.appearancegroup1.Controls.Add(label18);
			this.appearancegroup1.Controls.Add(this.labelDynLightCount);
			this.appearancegroup1.Controls.Add(this.tbDynLightCount);
			this.appearancegroup1.Controls.Add(this.animatevisualselection);
			this.appearancegroup1.Controls.Add(this.blackbrowsers);
			this.appearancegroup1.Controls.Add(this.visualbilinear);
			this.appearancegroup1.Controls.Add(label1);
			this.appearancegroup1.Controls.Add(this.classicbilinear);
			this.appearancegroup1.Controls.Add(this.imagebrightnesslabel);
			this.appearancegroup1.Controls.Add(this.imagebrightness);
			this.appearancegroup1.Location = new System.Drawing.Point(217, 224);
			this.appearancegroup1.Name = "appearancegroup1";
			this.appearancegroup1.Size = new System.Drawing.Size(457, 277);
			this.appearancegroup1.TabIndex = 24;
			this.appearancegroup1.TabStop = false;
			this.appearancegroup1.Text = " Additional Options ";
			// 
			// capitalizetexturenames
			// 
			this.capitalizetexturenames.AutoSize = true;
			this.capitalizetexturenames.Location = new System.Drawing.Point(236, 191);
			this.capitalizetexturenames.Name = "capitalizetexturenames";
			this.capitalizetexturenames.Size = new System.Drawing.Size(140, 17);
			this.capitalizetexturenames.TabIndex = 36;
			this.capitalizetexturenames.Text = "Capitalize texture names";
			this.toolTip1.SetToolTip(this.capitalizetexturenames, "When enabled, texture names will be shown \r\nand saved as all caps\r\n(Doom Builder " +
					"2 behaviour)");
			this.capitalizetexturenames.UseVisualStyleBackColor = true;
			// 
			// cbMarkExtraFloors
			// 
			this.cbMarkExtraFloors.AutoSize = true;
			this.cbMarkExtraFloors.Location = new System.Drawing.Point(25, 254);
			this.cbMarkExtraFloors.Name = "cbMarkExtraFloors";
			this.cbMarkExtraFloors.Size = new System.Drawing.Size(175, 17);
			this.cbMarkExtraFloors.TabIndex = 35;
			this.cbMarkExtraFloors.Text = "Mark 3D floors in classic modes";
			this.toolTip1.SetToolTip(this.cbMarkExtraFloors, "When enabled, linedefs of sectors with 3d floors will be marked using \"3D Floors\"" +
					" color.");
			this.cbMarkExtraFloors.UseVisualStyleBackColor = true;
			// 
			// cbOldHighlightMode
			// 
			this.cbOldHighlightMode.AutoSize = true;
			this.cbOldHighlightMode.Location = new System.Drawing.Point(236, 233);
			this.cbOldHighlightMode.Name = "cbOldHighlightMode";
			this.cbOldHighlightMode.Size = new System.Drawing.Size(207, 17);
			this.cbOldHighlightMode.TabIndex = 33;
			this.cbOldHighlightMode.Text = "Always show selection in visual modes";
			this.toolTip1.SetToolTip(this.cbOldHighlightMode, "If enabled, selected surfaces will be highlighted in Visual mode\r\neven if \"Show h" +
					"ighlight\" mode is disabled \r\n(Doom Builder 2 behaviour).");
			this.cbOldHighlightMode.UseVisualStyleBackColor = true;
			// 
			// labelDynLightIntensity
			// 
			this.labelDynLightIntensity.AutoSize = true;
			this.labelDynLightIntensity.Location = new System.Drawing.Point(337, 129);
			this.labelDynLightIntensity.Name = "labelDynLightIntensity";
			this.labelDynLightIntensity.Size = new System.Drawing.Size(22, 13);
			this.labelDynLightIntensity.TabIndex = 32;
			this.labelDynLightIntensity.Text = "1.0";
			// 
			// tbDynLightIntensity
			// 
			this.tbDynLightIntensity.BackColor = System.Drawing.SystemColors.Window;
			this.tbDynLightIntensity.LargeChange = 1;
			this.tbDynLightIntensity.Location = new System.Drawing.Point(176, 119);
			this.tbDynLightIntensity.Minimum = 1;
			this.tbDynLightIntensity.Name = "tbDynLightIntensity";
			this.tbDynLightIntensity.Size = new System.Drawing.Size(154, 45);
			this.tbDynLightIntensity.TabIndex = 30;
			this.tbDynLightIntensity.TickStyle = System.Windows.Forms.TickStyle.TopLeft;
			this.tbDynLightIntensity.Value = 10;
			this.tbDynLightIntensity.ValueChanged += new System.EventHandler(this.tbDynLightIntensity_ValueChanged);
			// 
			// labelDynLightSize
			// 
			this.labelDynLightSize.AutoSize = true;
			this.labelDynLightSize.Location = new System.Drawing.Point(337, 96);
			this.labelDynLightSize.Name = "labelDynLightSize";
			this.labelDynLightSize.Size = new System.Drawing.Size(22, 13);
			this.labelDynLightSize.TabIndex = 29;
			this.labelDynLightSize.Text = "1.0";
			// 
			// tbDynLightSize
			// 
			this.tbDynLightSize.BackColor = System.Drawing.SystemColors.Window;
			this.tbDynLightSize.LargeChange = 1;
			this.tbDynLightSize.Location = new System.Drawing.Point(176, 85);
			this.tbDynLightSize.Maximum = 20;
			this.tbDynLightSize.Minimum = 1;
			this.tbDynLightSize.Name = "tbDynLightSize";
			this.tbDynLightSize.Size = new System.Drawing.Size(154, 45);
			this.tbDynLightSize.TabIndex = 27;
			this.tbDynLightSize.TickStyle = System.Windows.Forms.TickStyle.TopLeft;
			this.tbDynLightSize.Value = 10;
			this.tbDynLightSize.ValueChanged += new System.EventHandler(this.tbDynLightSize_ValueChanged);
			// 
			// labelDynLightCount
			// 
			this.labelDynLightCount.AutoSize = true;
			this.labelDynLightCount.Location = new System.Drawing.Point(337, 62);
			this.labelDynLightCount.Name = "labelDynLightCount";
			this.labelDynLightCount.Size = new System.Drawing.Size(19, 13);
			this.labelDynLightCount.TabIndex = 26;
			this.labelDynLightCount.Text = "16";
			// 
			// tbDynLightCount
			// 
			this.tbDynLightCount.BackColor = System.Drawing.SystemColors.Window;
			this.tbDynLightCount.LargeChange = 3;
			this.tbDynLightCount.Location = new System.Drawing.Point(176, 51);
			this.tbDynLightCount.Maximum = 32;
			this.tbDynLightCount.Minimum = 1;
			this.tbDynLightCount.Name = "tbDynLightCount";
			this.tbDynLightCount.Size = new System.Drawing.Size(154, 45);
			this.tbDynLightCount.TabIndex = 24;
			this.tbDynLightCount.TickFrequency = 4;
			this.tbDynLightCount.TickStyle = System.Windows.Forms.TickStyle.TopLeft;
			this.tbDynLightCount.Value = 1;
			this.tbDynLightCount.ValueChanged += new System.EventHandler(this.tbDynLightCount_ValueChanged);
			// 
			// animatevisualselection
			// 
			this.animatevisualselection.AutoSize = true;
			this.animatevisualselection.Location = new System.Drawing.Point(236, 212);
			this.animatevisualselection.Name = "animatevisualselection";
			this.animatevisualselection.Size = new System.Drawing.Size(184, 17);
			this.animatevisualselection.TabIndex = 23;
			this.animatevisualselection.Text = "Animate selection in visual modes";
			this.animatevisualselection.UseVisualStyleBackColor = true;
			// 
			// blackbrowsers
			// 
			this.blackbrowsers.AutoSize = true;
			this.blackbrowsers.Location = new System.Drawing.Point(25, 170);
			this.blackbrowsers.Name = "blackbrowsers";
			this.blackbrowsers.Size = new System.Drawing.Size(195, 17);
			this.blackbrowsers.TabIndex = 4;
			this.blackbrowsers.Text = "Black background in image browser";
			this.blackbrowsers.UseVisualStyleBackColor = true;
			// 
			// visualbilinear
			// 
			this.visualbilinear.AutoSize = true;
			this.visualbilinear.Location = new System.Drawing.Point(25, 233);
			this.visualbilinear.Name = "visualbilinear";
			this.visualbilinear.Size = new System.Drawing.Size(171, 17);
			this.visualbilinear.TabIndex = 6;
			this.visualbilinear.Text = "Bilinear filtering in visual modes";
			this.visualbilinear.UseVisualStyleBackColor = true;
			// 
			// classicbilinear
			// 
			this.classicbilinear.AutoSize = true;
			this.classicbilinear.Location = new System.Drawing.Point(25, 212);
			this.classicbilinear.Name = "classicbilinear";
			this.classicbilinear.Size = new System.Drawing.Size(176, 17);
			this.classicbilinear.TabIndex = 5;
			this.classicbilinear.Text = "Bilinear filtering in classic modes";
			this.classicbilinear.UseVisualStyleBackColor = true;
			// 
			// imagebrightnesslabel
			// 
			this.imagebrightnesslabel.AutoSize = true;
			this.imagebrightnesslabel.Location = new System.Drawing.Point(337, 29);
			this.imagebrightnesslabel.Name = "imagebrightnesslabel";
			this.imagebrightnesslabel.Size = new System.Drawing.Size(30, 13);
			this.imagebrightnesslabel.TabIndex = 22;
			this.imagebrightnesslabel.Text = "+ 0 y";
			// 
			// imagebrightness
			// 
			this.imagebrightness.BackColor = System.Drawing.SystemColors.Window;
			this.imagebrightness.LargeChange = 3;
			this.imagebrightness.Location = new System.Drawing.Point(176, 18);
			this.imagebrightness.Name = "imagebrightness";
			this.imagebrightness.Size = new System.Drawing.Size(154, 45);
			this.imagebrightness.TabIndex = 3;
			this.imagebrightness.TickStyle = System.Windows.Forms.TickStyle.TopLeft;
			this.imagebrightness.ValueChanged += new System.EventHandler(this.imagebrightness_ValueChanged);
			// 
			// colorsgroup3
			// 
			this.colorsgroup3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.colorsgroup3.Controls.Add(this.snippetsallmanstyle);
			this.colorsgroup3.Controls.Add(this.scripttabwidth);
			this.colorsgroup3.Controls.Add(this.scriptautoindent);
			this.colorsgroup3.Controls.Add(this.label10);
			this.colorsgroup3.Controls.Add(this.fontpreview);
			this.colorsgroup3.Controls.Add(this.scriptfontsize);
			this.colorsgroup3.Controls.Add(this.label8);
			this.colorsgroup3.Controls.Add(this.scriptfontbold);
			this.colorsgroup3.Controls.Add(this.scriptfontname);
			this.colorsgroup3.Controls.Add(this.label3);
			this.colorsgroup3.Controls.Add(this.colorconstants);
			this.colorsgroup3.Controls.Add(this.colorliterals);
			this.colorsgroup3.Controls.Add(this.colorscriptbackground);
			this.colorsgroup3.Controls.Add(this.colorkeywords);
			this.colorsgroup3.Controls.Add(this.colorlinenumbers);
			this.colorsgroup3.Controls.Add(this.colorcomments);
			this.colorsgroup3.Controls.Add(this.colorplaintext);
			this.colorsgroup3.Location = new System.Drawing.Point(217, 8);
			this.colorsgroup3.Name = "colorsgroup3";
			this.colorsgroup3.Size = new System.Drawing.Size(457, 212);
			this.colorsgroup3.TabIndex = 1;
			this.colorsgroup3.TabStop = false;
			this.colorsgroup3.Text = " Script editor ";
			this.colorsgroup3.Visible = false;
			// 
			// snippetsallmanstyle
			// 
			this.snippetsallmanstyle.AutoSize = true;
			this.snippetsallmanstyle.Location = new System.Drawing.Point(202, 187);
			this.snippetsallmanstyle.Name = "snippetsallmanstyle";
			this.snippetsallmanstyle.Size = new System.Drawing.Size(172, 17);
			this.snippetsallmanstyle.TabIndex = 34;
			this.snippetsallmanstyle.Text = "Allman-style bracing in snippets";
			this.toolTip1.SetToolTip(this.snippetsallmanstyle, resources.GetString("snippetsallmanstyle.ToolTip"));
			this.snippetsallmanstyle.UseVisualStyleBackColor = true;
			// 
			// scripttabwidth
			// 
			this.scripttabwidth.AllowDecimal = false;
			this.scripttabwidth.AllowNegative = false;
			this.scripttabwidth.AllowRelative = false;
			this.scripttabwidth.ButtonStep = 2;
			this.scripttabwidth.ButtonStepBig = 10F;
			this.scripttabwidth.ButtonStepFloat = 1F;
			this.scripttabwidth.ButtonStepSmall = 0.1F;
			this.scripttabwidth.ButtonStepsUseModifierKeys = false;
			this.scripttabwidth.ButtonStepsWrapAround = false;
			this.scripttabwidth.Location = new System.Drawing.Point(364, 157);
			this.scripttabwidth.Name = "scripttabwidth";
			this.scripttabwidth.Size = new System.Drawing.Size(71, 24);
			this.scripttabwidth.StepValues = null;
			this.scripttabwidth.TabIndex = 32;
			// 
			// scriptautoindent
			// 
			this.scriptautoindent.AutoSize = true;
			this.scriptautoindent.Location = new System.Drawing.Point(202, 161);
			this.scriptautoindent.Name = "scriptautoindent";
			this.scriptautoindent.Size = new System.Drawing.Size(80, 17);
			this.scriptautoindent.TabIndex = 31;
			this.scriptautoindent.Text = "Auto indent";
			this.scriptautoindent.UseVisualStyleBackColor = true;
			// 
			// label10
			// 
			this.label10.AutoSize = true;
			this.label10.Location = new System.Drawing.Point(304, 162);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(57, 13);
			this.label10.TabIndex = 30;
			this.label10.Text = "Tab width:";
			// 
			// fontpreview
			// 
			this.fontpreview.Cursor = System.Windows.Forms.Cursors.Arrow;
			this.fontpreview.DetectUrls = false;
			this.fontpreview.Location = new System.Drawing.Point(202, 70);
			this.fontpreview.Name = "fontpreview";
			this.fontpreview.ReadOnly = true;
			this.fontpreview.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
			this.fontpreview.Size = new System.Drawing.Size(233, 80);
			this.fontpreview.TabIndex = 33;
			this.fontpreview.Text = "1 #include \"zcommon.acs\"\n2 script 0 (void) { //a comment\n3       CONSTANT_VALUE;\n" +
				"4 }";
			this.fontpreview.WordWrap = false;
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
			this.scriptfontsize.Location = new System.Drawing.Point(236, 43);
			this.scriptfontsize.Name = "scriptfontsize";
			this.scriptfontsize.Size = new System.Drawing.Size(94, 21);
			this.scriptfontsize.TabIndex = 25;
			this.scriptfontsize.SelectedIndexChanged += new System.EventHandler(this.scriptfontsize_SelectedIndexChanged);
			// 
			// label8
			// 
			this.label8.AutoSize = true;
			this.label8.Location = new System.Drawing.Point(199, 46);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(30, 13);
			this.label8.TabIndex = 28;
			this.label8.Text = "Size:";
			// 
			// scriptfontbold
			// 
			this.scriptfontbold.AutoSize = true;
			this.scriptfontbold.Location = new System.Drawing.Point(354, 45);
			this.scriptfontbold.Name = "scriptfontbold";
			this.scriptfontbold.Size = new System.Drawing.Size(47, 17);
			this.scriptfontbold.TabIndex = 26;
			this.scriptfontbold.Text = "Bold";
			this.scriptfontbold.UseVisualStyleBackColor = true;
			this.scriptfontbold.CheckedChanged += new System.EventHandler(this.scriptfontbold_CheckedChanged);
			// 
			// scriptfontname
			// 
			this.scriptfontname.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.scriptfontname.FormattingEnabled = true;
			this.scriptfontname.Location = new System.Drawing.Point(236, 17);
			this.scriptfontname.Name = "scriptfontname";
			this.scriptfontname.Size = new System.Drawing.Size(199, 21);
			this.scriptfontname.Sorted = true;
			this.scriptfontname.TabIndex = 23;
			this.scriptfontname.SelectedIndexChanged += new System.EventHandler(this.scriptfontname_SelectedIndexChanged);
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(199, 20);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(31, 13);
			this.label3.TabIndex = 24;
			this.label3.Text = "Font:";
			// 
			// colorconstants
			// 
			this.colorconstants.BackColor = System.Drawing.Color.Transparent;
			this.colorconstants.Label = "Constants:";
			this.colorconstants.Location = new System.Drawing.Point(15, 179);
			this.colorconstants.MaximumSize = new System.Drawing.Size(10000, 23);
			this.colorconstants.MinimumSize = new System.Drawing.Size(100, 23);
			this.colorconstants.Name = "colorconstants";
			this.colorconstants.Size = new System.Drawing.Size(150, 23);
			this.colorconstants.TabIndex = 6;
			this.colorconstants.ColorChanged += new System.EventHandler(this.scriptcolor_ColorChanged);
			// 
			// colorliterals
			// 
			this.colorliterals.BackColor = System.Drawing.Color.Transparent;
			this.colorliterals.Label = "Literals:";
			this.colorliterals.Location = new System.Drawing.Point(15, 152);
			this.colorliterals.MaximumSize = new System.Drawing.Size(10000, 23);
			this.colorliterals.MinimumSize = new System.Drawing.Size(100, 23);
			this.colorliterals.Name = "colorliterals";
			this.colorliterals.Size = new System.Drawing.Size(150, 23);
			this.colorliterals.TabIndex = 5;
			this.colorliterals.ColorChanged += new System.EventHandler(this.scriptcolor_ColorChanged);
			// 
			// colorscriptbackground
			// 
			this.colorscriptbackground.BackColor = System.Drawing.Color.Transparent;
			this.colorscriptbackground.Label = "Background:";
			this.colorscriptbackground.Location = new System.Drawing.Point(15, 17);
			this.colorscriptbackground.MaximumSize = new System.Drawing.Size(10000, 23);
			this.colorscriptbackground.MinimumSize = new System.Drawing.Size(100, 23);
			this.colorscriptbackground.Name = "colorscriptbackground";
			this.colorscriptbackground.Size = new System.Drawing.Size(150, 23);
			this.colorscriptbackground.TabIndex = 0;
			this.colorscriptbackground.ColorChanged += new System.EventHandler(this.scriptcolor_ColorChanged);
			// 
			// colorkeywords
			// 
			this.colorkeywords.BackColor = System.Drawing.Color.Transparent;
			this.colorkeywords.Label = "Keywords:";
			this.colorkeywords.Location = new System.Drawing.Point(15, 125);
			this.colorkeywords.MaximumSize = new System.Drawing.Size(10000, 23);
			this.colorkeywords.MinimumSize = new System.Drawing.Size(100, 23);
			this.colorkeywords.Name = "colorkeywords";
			this.colorkeywords.Size = new System.Drawing.Size(150, 23);
			this.colorkeywords.TabIndex = 4;
			this.colorkeywords.ColorChanged += new System.EventHandler(this.scriptcolor_ColorChanged);
			// 
			// colorlinenumbers
			// 
			this.colorlinenumbers.BackColor = System.Drawing.Color.Transparent;
			this.colorlinenumbers.Label = "Line numbers:";
			this.colorlinenumbers.Location = new System.Drawing.Point(15, 44);
			this.colorlinenumbers.MaximumSize = new System.Drawing.Size(10000, 23);
			this.colorlinenumbers.MinimumSize = new System.Drawing.Size(100, 23);
			this.colorlinenumbers.Name = "colorlinenumbers";
			this.colorlinenumbers.Size = new System.Drawing.Size(150, 23);
			this.colorlinenumbers.TabIndex = 1;
			this.colorlinenumbers.ColorChanged += new System.EventHandler(this.scriptcolor_ColorChanged);
			// 
			// colorcomments
			// 
			this.colorcomments.BackColor = System.Drawing.Color.Transparent;
			this.colorcomments.Label = "Comments:";
			this.colorcomments.Location = new System.Drawing.Point(15, 98);
			this.colorcomments.MaximumSize = new System.Drawing.Size(10000, 23);
			this.colorcomments.MinimumSize = new System.Drawing.Size(100, 23);
			this.colorcomments.Name = "colorcomments";
			this.colorcomments.Size = new System.Drawing.Size(150, 23);
			this.colorcomments.TabIndex = 3;
			this.colorcomments.ColorChanged += new System.EventHandler(this.scriptcolor_ColorChanged);
			// 
			// colorplaintext
			// 
			this.colorplaintext.BackColor = System.Drawing.Color.Transparent;
			this.colorplaintext.Label = "Plain text:";
			this.colorplaintext.Location = new System.Drawing.Point(15, 71);
			this.colorplaintext.MaximumSize = new System.Drawing.Size(10000, 23);
			this.colorplaintext.MinimumSize = new System.Drawing.Size(100, 23);
			this.colorplaintext.Name = "colorplaintext";
			this.colorplaintext.Size = new System.Drawing.Size(150, 23);
			this.colorplaintext.TabIndex = 2;
			this.colorplaintext.ColorChanged += new System.EventHandler(this.scriptcolor_ColorChanged);
			// 
			// tabpasting
			// 
			this.tabpasting.Controls.Add(this.label16);
			this.tabpasting.Controls.Add(this.pasteoptions);
			this.tabpasting.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.tabpasting.Location = new System.Drawing.Point(4, 23);
			this.tabpasting.Name = "tabpasting";
			this.tabpasting.Padding = new System.Windows.Forms.Padding(5);
			this.tabpasting.Size = new System.Drawing.Size(682, 509);
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
			this.label16.Size = new System.Drawing.Size(649, 35);
			this.label16.TabIndex = 1;
			this.label16.Text = "These are the default options for pasting geometry. You can also choose these opt" +
				"ions when you use the Paste Special function. These options also apply when inse" +
				"rting prefabs.";
			// 
			// pasteoptions
			// 
			this.pasteoptions.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.pasteoptions.Location = new System.Drawing.Point(8, 53);
			this.pasteoptions.Name = "pasteoptions";
			this.pasteoptions.Size = new System.Drawing.Size(666, 427);
			this.pasteoptions.TabIndex = 0;
			// 
			// browseScreenshotsFolderDialog
			// 
			this.browseScreenshotsFolderDialog.Description = "Select a Folder to Save Screenshots Into";
			// 
			// PreferencesForm
			// 
			this.AcceptButton = this.apply;
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.CancelButton = this.cancel;
			this.ClientSize = new System.Drawing.Size(711, 594);
			this.Controls.Add(this.cancel);
			this.Controls.Add(this.apply);
			this.Controls.Add(this.tabs);
			this.DoubleBuffered = true;
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "PreferencesForm";
			this.Opacity = 1;
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Preferences";
			this.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.PreferencesForm_HelpRequested);
			groupBox1.ResumeLayout(false);
			groupBox1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.recentFiles)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.vertexScale)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.zoomfactor)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.autoscrollspeed)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.previewsize)).EndInit();
			this.colorsgroup1.ResumeLayout(false);
			this.colorsgroup1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.doublesidedalpha)).EndInit();
			this.tabs.ResumeLayout(false);
			this.tabinterface.ResumeLayout(false);
			this.groupBox3.ResumeLayout(false);
			this.groupBox3.PerformLayout();
			this.groupBox5.ResumeLayout(false);
			this.groupBox5.PerformLayout();
			this.groupBox4.ResumeLayout(false);
			this.groupBox4.PerformLayout();
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.vertexScale3D)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.viewdistance)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.movespeed)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.mousespeed)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.fieldofview)).EndInit();
			this.tabkeys.ResumeLayout(false);
			this.tabkeys.PerformLayout();
			this.actioncontrolpanel.ResumeLayout(false);
			this.actioncontrolpanel.PerformLayout();
			this.tabcolors.ResumeLayout(false);
			this.appearancegroup1.ResumeLayout(false);
			this.appearancegroup1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.tbDynLightIntensity)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.tbDynLightSize)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.tbDynLightCount)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.imagebrightness)).EndInit();
			this.colorsgroup3.ResumeLayout(false);
			this.colorsgroup3.PerformLayout();
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
		private System.Windows.Forms.TrackBar fieldofview;
		private System.Windows.Forms.TrackBar movespeed;
		private System.Windows.Forms.TrackBar mousespeed;
		private System.Windows.Forms.TrackBar viewdistance;
		private System.Windows.Forms.TrackBar doublesidedalpha;
		private System.Windows.Forms.TrackBar imagebrightness;
		private System.Windows.Forms.Label disregardshiftlabel;
		private System.Windows.Forms.ListBox keyusedlist;
		private System.Windows.Forms.Label keyusedlabel;
		private System.Windows.Forms.CheckBox qualitydisplay;
		private System.Windows.Forms.CheckBox visualbilinear;
		private System.Windows.Forms.CheckBox classicbilinear;
		private System.Windows.Forms.TrackBar previewsize;
		private System.Windows.Forms.Label previewsizelabel;
		private System.Windows.Forms.Label label12;
		private System.Windows.Forms.TrackBar autoscrollspeed;
		private System.Windows.Forms.Label autoscrollspeedlabel;
		private System.Windows.Forms.Label label15;
		private System.Windows.Forms.CheckBox animatevisualselection;
		private System.Windows.Forms.TabPage tabpasting;
		private CodeImp.DoomBuilder.Controls.PasteOptionsControl pasteoptions;
		private System.Windows.Forms.Label label16;
		private System.Windows.Forms.GroupBox appearancegroup1;
		private System.Windows.Forms.GroupBox groupBox4;
		private System.Windows.Forms.ComboBox dockersposition;
		private System.Windows.Forms.Label label17;
		private System.Windows.Forms.CheckBox collapsedockers;
		private System.Windows.Forms.TrackBar zoomfactor;
		private System.Windows.Forms.Label zoomfactorlabel;
		private System.Windows.Forms.Label label19;
		private System.Windows.Forms.GroupBox groupBox5;
		private CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox scripttabwidth;
		private System.Windows.Forms.CheckBox scriptautoindent;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.ComboBox scriptfontsize;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.CheckBox scriptfontbold;
		private System.Windows.Forms.ComboBox scriptfontname;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.CheckBox scriptontop;
		private System.Windows.Forms.CheckBox toolbar_script;
		private System.Windows.Forms.CheckBox toolbar_copy;
		private System.Windows.Forms.CheckBox toolbar_undo;
		private System.Windows.Forms.CheckBox toolbar_viewmodes;
		private System.Windows.Forms.CheckBox toolbar_filter;
		private System.Windows.Forms.CheckBox toolbar_prefabs;
		private System.Windows.Forms.CheckBox toolbar_geometry;
		private System.Windows.Forms.CheckBox toolbar_testing;
		private System.Windows.Forms.CheckBox toolbar_file;
		private System.Windows.Forms.CheckBox showtexturesizes;
		private CodeImp.DoomBuilder.Controls.ColorControl colorMD3;
		private System.Windows.Forms.CheckBox toolbar_gzdoom;
		private System.Windows.Forms.Label labelDynLightCount;
		private System.Windows.Forms.TrackBar tbDynLightCount;
		private System.Windows.Forms.Label labelDynLightSize;
		private System.Windows.Forms.TrackBar tbDynLightSize;
		private System.Windows.Forms.Label labelDynLightIntensity;
		private System.Windows.Forms.TrackBar tbDynLightIntensity;
		private System.Windows.Forms.CheckBox cbSynchCameras;
		private System.Windows.Forms.CheckBox cbStretchView;
		private System.Windows.Forms.ToolTip toolTip1;
		private System.Windows.Forms.TrackBar vertexScale;
		private System.Windows.Forms.Label vertexScaleLabel;
		private System.Windows.Forms.Label label22;
		private CodeImp.DoomBuilder.Controls.ColorControl colorInfo;
		private System.Windows.Forms.CheckBox cbOldHighlightMode;
		private System.Windows.Forms.Button bClearActionFilter;
		private System.Windows.Forms.TextBox tbFilterActions;
		private System.Windows.Forms.Label label24;
		private CodeImp.DoomBuilder.Controls.ColorControl color3dFloors;
		private System.Windows.Forms.TextBox actiondescription;
		private System.Windows.Forms.RichTextBox fontpreview;
		private System.Windows.Forms.CheckBox cbMarkExtraFloors;
		private System.Windows.Forms.TrackBar recentFiles;
		private System.Windows.Forms.Label labelRecentFiles;
		private System.Windows.Forms.Label label25;
		private System.Windows.Forms.CheckBox snippetsallmanstyle;
		private System.Windows.Forms.CheckBox capitalizetexturenames;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.Button browsescreenshotsdir;
		private System.Windows.Forms.TextBox screenshotsfolderpath;
		private System.Windows.Forms.Button resetscreenshotsdir;
		private System.Windows.Forms.FolderBrowserDialog browseScreenshotsFolderDialog;
		private System.Windows.Forms.TrackBar vertexScale3D;
		private System.Windows.Forms.Label vertexScale3DLabel;
		private System.Windows.Forms.Label label26;
		private System.Windows.Forms.CheckBox locatetexturegroup;
		private System.Windows.Forms.CheckBox cbStoreEditTab;
		private System.Windows.Forms.CheckBox checkforupdates;
	}
}