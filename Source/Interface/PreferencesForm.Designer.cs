namespace CodeImp.DoomBuilder.Interface
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
			this.colorsgroup1 = new System.Windows.Forms.GroupBox();
			this.colorgrid64 = new CodeImp.DoomBuilder.Interface.ColorControl();
			this.colorgrid = new CodeImp.DoomBuilder.Interface.ColorControl();
			this.colorassociations = new CodeImp.DoomBuilder.Interface.ColorControl();
			this.colorsoundlinedefs = new CodeImp.DoomBuilder.Interface.ColorControl();
			this.colorspeciallinedefs = new CodeImp.DoomBuilder.Interface.ColorControl();
			this.colorbackcolor = new CodeImp.DoomBuilder.Interface.ColorControl();
			this.colorselection = new CodeImp.DoomBuilder.Interface.ColorControl();
			this.colorvertices = new CodeImp.DoomBuilder.Interface.ColorControl();
			this.colorhighlight = new CodeImp.DoomBuilder.Interface.ColorControl();
			this.colorlinedefs = new CodeImp.DoomBuilder.Interface.ColorControl();
			this.cancel = new System.Windows.Forms.Button();
			this.apply = new System.Windows.Forms.Button();
			this.tabs = new System.Windows.Forms.TabControl();
			this.tabinterface = new System.Windows.Forms.TabPage();
			this.tabkeys = new System.Windows.Forms.TabPage();
			this.listactions = new System.Windows.Forms.ListView();
			this.columncontrolaction = new System.Windows.Forms.ColumnHeader();
			this.columncontrolkey = new System.Windows.Forms.ColumnHeader();
			this.actioncontrolpanel = new System.Windows.Forms.GroupBox();
			this.actioncontrol = new System.Windows.Forms.ComboBox();
			this.actiontitle = new System.Windows.Forms.Label();
			this.actioncontrolclear = new System.Windows.Forms.Button();
			this.actionkey = new System.Windows.Forms.TextBox();
			this.actiondescription = new System.Windows.Forms.Label();
			this.tabcolors = new System.Windows.Forms.TabPage();
			this.blackbrowsers = new System.Windows.Forms.CheckBox();
			this.colorsgroup3 = new System.Windows.Forms.GroupBox();
			this.colorconstants = new CodeImp.DoomBuilder.Interface.ColorControl();
			this.colorliterals = new CodeImp.DoomBuilder.Interface.ColorControl();
			this.colorscriptbackground = new CodeImp.DoomBuilder.Interface.ColorControl();
			this.colorkeywords = new CodeImp.DoomBuilder.Interface.ColorControl();
			this.colorlinenumbers = new CodeImp.DoomBuilder.Interface.ColorControl();
			this.colorcomments = new CodeImp.DoomBuilder.Interface.ColorControl();
			this.colorplaintext = new CodeImp.DoomBuilder.Interface.ColorControl();
			this.colorsgroup2 = new System.Windows.Forms.GroupBox();
			this.colorselection3d = new CodeImp.DoomBuilder.Interface.ColorControl();
			this.colorhighlight3d = new CodeImp.DoomBuilder.Interface.ColorControl();
			this.colorcrosshair3d = new CodeImp.DoomBuilder.Interface.ColorControl();
			label7 = new System.Windows.Forms.Label();
			label6 = new System.Windows.Forms.Label();
			label5 = new System.Windows.Forms.Label();
			this.colorsgroup1.SuspendLayout();
			this.tabs.SuspendLayout();
			this.tabkeys.SuspendLayout();
			this.actioncontrolpanel.SuspendLayout();
			this.tabcolors.SuspendLayout();
			this.colorsgroup3.SuspendLayout();
			this.colorsgroup2.SuspendLayout();
			this.SuspendLayout();
			// 
			// label7
			// 
			label7.AutoSize = true;
			label7.Location = new System.Drawing.Point(20, 183);
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
			// colorsgroup1
			// 
			this.colorsgroup1.Controls.Add(this.colorgrid64);
			this.colorsgroup1.Controls.Add(this.colorgrid);
			this.colorsgroup1.Controls.Add(this.colorassociations);
			this.colorsgroup1.Controls.Add(this.colorsoundlinedefs);
			this.colorsgroup1.Controls.Add(this.colorspeciallinedefs);
			this.colorsgroup1.Controls.Add(this.colorbackcolor);
			this.colorsgroup1.Controls.Add(this.colorselection);
			this.colorsgroup1.Controls.Add(this.colorvertices);
			this.colorsgroup1.Controls.Add(this.colorhighlight);
			this.colorsgroup1.Controls.Add(this.colorlinedefs);
			this.colorsgroup1.Location = new System.Drawing.Point(12, 10);
			this.colorsgroup1.Name = "colorsgroup1";
			this.colorsgroup1.Size = new System.Drawing.Size(181, 325);
			this.colorsgroup1.TabIndex = 10;
			this.colorsgroup1.TabStop = false;
			this.colorsgroup1.Text = " Classic modes ";
			this.colorsgroup1.Visible = false;
			// 
			// colorgrid64
			// 
			this.colorgrid64.BackColor = System.Drawing.SystemColors.Control;
			this.colorgrid64.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.colorgrid64.Label = "64 Block grid:";
			this.colorgrid64.Location = new System.Drawing.Point(15, 288);
			this.colorgrid64.MaximumSize = new System.Drawing.Size(10000, 23);
			this.colorgrid64.MinimumSize = new System.Drawing.Size(100, 23);
			this.colorgrid64.Name = "colorgrid64";
			this.colorgrid64.Size = new System.Drawing.Size(150, 23);
			this.colorgrid64.TabIndex = 15;
			// 
			// colorgrid
			// 
			this.colorgrid.BackColor = System.Drawing.SystemColors.Control;
			this.colorgrid.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.colorgrid.Label = "Custom grid:";
			this.colorgrid.Location = new System.Drawing.Point(15, 259);
			this.colorgrid.MaximumSize = new System.Drawing.Size(10000, 23);
			this.colorgrid.MinimumSize = new System.Drawing.Size(100, 23);
			this.colorgrid.Name = "colorgrid";
			this.colorgrid.Size = new System.Drawing.Size(150, 23);
			this.colorgrid.TabIndex = 14;
			// 
			// colorassociations
			// 
			this.colorassociations.BackColor = System.Drawing.SystemColors.Control;
			this.colorassociations.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.colorassociations.Label = "Associations:";
			this.colorassociations.Location = new System.Drawing.Point(15, 230);
			this.colorassociations.MaximumSize = new System.Drawing.Size(10000, 23);
			this.colorassociations.MinimumSize = new System.Drawing.Size(100, 23);
			this.colorassociations.Name = "colorassociations";
			this.colorassociations.Size = new System.Drawing.Size(150, 23);
			this.colorassociations.TabIndex = 13;
			// 
			// colorsoundlinedefs
			// 
			this.colorsoundlinedefs.BackColor = System.Drawing.SystemColors.Control;
			this.colorsoundlinedefs.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.colorsoundlinedefs.Label = "Sound lines:";
			this.colorsoundlinedefs.Location = new System.Drawing.Point(15, 143);
			this.colorsoundlinedefs.MaximumSize = new System.Drawing.Size(10000, 23);
			this.colorsoundlinedefs.MinimumSize = new System.Drawing.Size(100, 23);
			this.colorsoundlinedefs.Name = "colorsoundlinedefs";
			this.colorsoundlinedefs.Size = new System.Drawing.Size(150, 23);
			this.colorsoundlinedefs.TabIndex = 12;
			// 
			// colorspeciallinedefs
			// 
			this.colorspeciallinedefs.BackColor = System.Drawing.SystemColors.Control;
			this.colorspeciallinedefs.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.colorspeciallinedefs.Label = "Action lines:";
			this.colorspeciallinedefs.Location = new System.Drawing.Point(15, 114);
			this.colorspeciallinedefs.MaximumSize = new System.Drawing.Size(10000, 23);
			this.colorspeciallinedefs.MinimumSize = new System.Drawing.Size(100, 23);
			this.colorspeciallinedefs.Name = "colorspeciallinedefs";
			this.colorspeciallinedefs.Size = new System.Drawing.Size(150, 23);
			this.colorspeciallinedefs.TabIndex = 11;
			// 
			// colorbackcolor
			// 
			this.colorbackcolor.BackColor = System.Drawing.SystemColors.Control;
			this.colorbackcolor.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.colorbackcolor.Label = "Background:";
			this.colorbackcolor.Location = new System.Drawing.Point(15, 27);
			this.colorbackcolor.MaximumSize = new System.Drawing.Size(10000, 23);
			this.colorbackcolor.MinimumSize = new System.Drawing.Size(100, 23);
			this.colorbackcolor.Name = "colorbackcolor";
			this.colorbackcolor.Size = new System.Drawing.Size(150, 23);
			this.colorbackcolor.TabIndex = 5;
			// 
			// colorselection
			// 
			this.colorselection.BackColor = System.Drawing.SystemColors.Control;
			this.colorselection.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.colorselection.Label = "Selection:";
			this.colorselection.Location = new System.Drawing.Point(15, 201);
			this.colorselection.MaximumSize = new System.Drawing.Size(10000, 23);
			this.colorselection.MinimumSize = new System.Drawing.Size(100, 23);
			this.colorselection.Name = "colorselection";
			this.colorselection.Size = new System.Drawing.Size(150, 23);
			this.colorselection.TabIndex = 9;
			// 
			// colorvertices
			// 
			this.colorvertices.BackColor = System.Drawing.SystemColors.Control;
			this.colorvertices.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.colorvertices.Label = "Vertices:";
			this.colorvertices.Location = new System.Drawing.Point(15, 56);
			this.colorvertices.MaximumSize = new System.Drawing.Size(10000, 23);
			this.colorvertices.MinimumSize = new System.Drawing.Size(100, 23);
			this.colorvertices.Name = "colorvertices";
			this.colorvertices.Size = new System.Drawing.Size(150, 23);
			this.colorvertices.TabIndex = 6;
			// 
			// colorhighlight
			// 
			this.colorhighlight.BackColor = System.Drawing.SystemColors.Control;
			this.colorhighlight.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.colorhighlight.Label = "Highlight:";
			this.colorhighlight.Location = new System.Drawing.Point(15, 172);
			this.colorhighlight.MaximumSize = new System.Drawing.Size(10000, 23);
			this.colorhighlight.MinimumSize = new System.Drawing.Size(100, 23);
			this.colorhighlight.Name = "colorhighlight";
			this.colorhighlight.Size = new System.Drawing.Size(150, 23);
			this.colorhighlight.TabIndex = 8;
			// 
			// colorlinedefs
			// 
			this.colorlinedefs.BackColor = System.Drawing.SystemColors.Control;
			this.colorlinedefs.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.colorlinedefs.Label = "Common lines:";
			this.colorlinedefs.Location = new System.Drawing.Point(15, 85);
			this.colorlinedefs.MaximumSize = new System.Drawing.Size(10000, 23);
			this.colorlinedefs.MinimumSize = new System.Drawing.Size(100, 23);
			this.colorlinedefs.Name = "colorlinedefs";
			this.colorlinedefs.Size = new System.Drawing.Size(150, 23);
			this.colorlinedefs.TabIndex = 7;
			// 
			// cancel
			// 
			this.cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancel.Location = new System.Drawing.Point(497, 428);
			this.cancel.Name = "cancel";
			this.cancel.Size = new System.Drawing.Size(112, 25);
			this.cancel.TabIndex = 20;
			this.cancel.Text = "Cancel";
			this.cancel.UseVisualStyleBackColor = true;
			this.cancel.Click += new System.EventHandler(this.cancel_Click);
			// 
			// apply
			// 
			this.apply.Location = new System.Drawing.Point(379, 428);
			this.apply.Name = "apply";
			this.apply.Size = new System.Drawing.Size(112, 25);
			this.apply.TabIndex = 19;
			this.apply.Text = "OK";
			this.apply.UseVisualStyleBackColor = true;
			this.apply.Click += new System.EventHandler(this.apply_Click);
			// 
			// tabs
			// 
			this.tabs.Controls.Add(this.tabinterface);
			this.tabs.Controls.Add(this.tabkeys);
			this.tabs.Controls.Add(this.tabcolors);
			this.tabs.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.tabs.ItemSize = new System.Drawing.Size(110, 19);
			this.tabs.Location = new System.Drawing.Point(11, 13);
			this.tabs.Name = "tabs";
			this.tabs.SelectedIndex = 0;
			this.tabs.Size = new System.Drawing.Size(598, 406);
			this.tabs.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
			this.tabs.TabIndex = 18;
			this.tabs.SelectedIndexChanged += new System.EventHandler(this.tabs_SelectedIndexChanged);
			// 
			// tabinterface
			// 
			this.tabinterface.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.tabinterface.Location = new System.Drawing.Point(4, 23);
			this.tabinterface.Name = "tabinterface";
			this.tabinterface.Padding = new System.Windows.Forms.Padding(3);
			this.tabinterface.Size = new System.Drawing.Size(590, 379);
			this.tabinterface.TabIndex = 0;
			this.tabinterface.Text = "Interface";
			this.tabinterface.UseVisualStyleBackColor = true;
			// 
			// tabkeys
			// 
			this.tabkeys.Controls.Add(this.listactions);
			this.tabkeys.Controls.Add(this.actioncontrolpanel);
			this.tabkeys.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.tabkeys.Location = new System.Drawing.Point(4, 23);
			this.tabkeys.Name = "tabkeys";
			this.tabkeys.Padding = new System.Windows.Forms.Padding(3);
			this.tabkeys.Size = new System.Drawing.Size(590, 379);
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
			this.listactions.ShowGroups = false;
			this.listactions.Size = new System.Drawing.Size(274, 353);
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
			this.columncontrolaction.Width = 150;
			// 
			// columncontrolkey
			// 
			this.columncontrolkey.Text = "Key";
			this.columncontrolkey.Width = 100;
			// 
			// actioncontrolpanel
			// 
			this.actioncontrolpanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.actioncontrolpanel.Controls.Add(this.actioncontrol);
			this.actioncontrolpanel.Controls.Add(label7);
			this.actioncontrolpanel.Controls.Add(this.actiontitle);
			this.actioncontrolpanel.Controls.Add(this.actioncontrolclear);
			this.actioncontrolpanel.Controls.Add(label6);
			this.actioncontrolpanel.Controls.Add(this.actionkey);
			this.actioncontrolpanel.Controls.Add(this.actiondescription);
			this.actioncontrolpanel.Controls.Add(label5);
			this.actioncontrolpanel.Enabled = false;
			this.actioncontrolpanel.Location = new System.Drawing.Point(299, 12);
			this.actioncontrolpanel.Margin = new System.Windows.Forms.Padding(6);
			this.actioncontrolpanel.Name = "actioncontrolpanel";
			this.actioncontrolpanel.Size = new System.Drawing.Size(282, 353);
			this.actioncontrolpanel.TabIndex = 9;
			this.actioncontrolpanel.TabStop = false;
			this.actioncontrolpanel.Text = " Action control ";
			// 
			// actioncontrol
			// 
			this.actioncontrol.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.actioncontrol.FormattingEnabled = true;
			this.actioncontrol.Location = new System.Drawing.Point(23, 204);
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
			this.actioncontrolclear.Location = new System.Drawing.Point(193, 140);
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
			this.actionkey.Location = new System.Drawing.Point(23, 142);
			this.actionkey.Name = "actionkey";
			this.actionkey.Size = new System.Drawing.Size(163, 20);
			this.actionkey.TabIndex = 5;
			this.actionkey.TabStop = false;
			this.actionkey.KeyDown += new System.Windows.Forms.KeyEventHandler(this.actionkey_KeyDown);
			// 
			// actiondescription
			// 
			this.actiondescription.AutoEllipsis = true;
			this.actiondescription.Location = new System.Drawing.Point(20, 52);
			this.actiondescription.Name = "actiondescription";
			this.actiondescription.Size = new System.Drawing.Size(245, 70);
			this.actiondescription.TabIndex = 3;
			this.actiondescription.UseMnemonic = false;
			// 
			// tabcolors
			// 
			this.tabcolors.Controls.Add(this.blackbrowsers);
			this.tabcolors.Controls.Add(this.colorsgroup3);
			this.tabcolors.Controls.Add(this.colorsgroup2);
			this.tabcolors.Controls.Add(this.colorsgroup1);
			this.tabcolors.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.tabcolors.Location = new System.Drawing.Point(4, 23);
			this.tabcolors.Name = "tabcolors";
			this.tabcolors.Size = new System.Drawing.Size(590, 379);
			this.tabcolors.TabIndex = 2;
			this.tabcolors.Text = "Colors";
			this.tabcolors.UseVisualStyleBackColor = true;
			// 
			// blackbrowsers
			// 
			this.blackbrowsers.AutoSize = true;
			this.blackbrowsers.Location = new System.Drawing.Point(13, 345);
			this.blackbrowsers.Name = "blackbrowsers";
			this.blackbrowsers.Size = new System.Drawing.Size(241, 18);
			this.blackbrowsers.TabIndex = 13;
			this.blackbrowsers.Text = "Force black background for image browsers";
			this.blackbrowsers.UseVisualStyleBackColor = true;
			// 
			// colorsgroup3
			// 
			this.colorsgroup3.Controls.Add(this.colorconstants);
			this.colorsgroup3.Controls.Add(this.colorliterals);
			this.colorsgroup3.Controls.Add(this.colorscriptbackground);
			this.colorsgroup3.Controls.Add(this.colorkeywords);
			this.colorsgroup3.Controls.Add(this.colorlinenumbers);
			this.colorsgroup3.Controls.Add(this.colorcomments);
			this.colorsgroup3.Controls.Add(this.colorplaintext);
			this.colorsgroup3.Location = new System.Drawing.Point(398, 10);
			this.colorsgroup3.Name = "colorsgroup3";
			this.colorsgroup3.Size = new System.Drawing.Size(181, 325);
			this.colorsgroup3.TabIndex = 12;
			this.colorsgroup3.TabStop = false;
			this.colorsgroup3.Text = " Script editor ";
			this.colorsgroup3.Visible = false;
			// 
			// colorconstants
			// 
			this.colorconstants.BackColor = System.Drawing.SystemColors.Control;
			this.colorconstants.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.colorconstants.Label = "Constants:";
			this.colorconstants.Location = new System.Drawing.Point(15, 201);
			this.colorconstants.MaximumSize = new System.Drawing.Size(10000, 23);
			this.colorconstants.MinimumSize = new System.Drawing.Size(100, 23);
			this.colorconstants.Name = "colorconstants";
			this.colorconstants.Size = new System.Drawing.Size(150, 23);
			this.colorconstants.TabIndex = 16;
			// 
			// colorliterals
			// 
			this.colorliterals.BackColor = System.Drawing.SystemColors.Control;
			this.colorliterals.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.colorliterals.Label = "Literals:";
			this.colorliterals.Location = new System.Drawing.Point(15, 172);
			this.colorliterals.MaximumSize = new System.Drawing.Size(10000, 23);
			this.colorliterals.MinimumSize = new System.Drawing.Size(100, 23);
			this.colorliterals.Name = "colorliterals";
			this.colorliterals.Size = new System.Drawing.Size(150, 23);
			this.colorliterals.TabIndex = 15;
			// 
			// colorscriptbackground
			// 
			this.colorscriptbackground.BackColor = System.Drawing.SystemColors.Control;
			this.colorscriptbackground.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.colorscriptbackground.Label = "Background:";
			this.colorscriptbackground.Location = new System.Drawing.Point(15, 27);
			this.colorscriptbackground.MaximumSize = new System.Drawing.Size(10000, 23);
			this.colorscriptbackground.MinimumSize = new System.Drawing.Size(100, 23);
			this.colorscriptbackground.Name = "colorscriptbackground";
			this.colorscriptbackground.Size = new System.Drawing.Size(150, 23);
			this.colorscriptbackground.TabIndex = 10;
			// 
			// colorkeywords
			// 
			this.colorkeywords.BackColor = System.Drawing.SystemColors.Control;
			this.colorkeywords.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.colorkeywords.Label = "Keywords:";
			this.colorkeywords.Location = new System.Drawing.Point(15, 143);
			this.colorkeywords.MaximumSize = new System.Drawing.Size(10000, 23);
			this.colorkeywords.MinimumSize = new System.Drawing.Size(100, 23);
			this.colorkeywords.Name = "colorkeywords";
			this.colorkeywords.Size = new System.Drawing.Size(150, 23);
			this.colorkeywords.TabIndex = 14;
			// 
			// colorlinenumbers
			// 
			this.colorlinenumbers.BackColor = System.Drawing.SystemColors.Control;
			this.colorlinenumbers.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.colorlinenumbers.Label = "Line numbers:";
			this.colorlinenumbers.Location = new System.Drawing.Point(15, 56);
			this.colorlinenumbers.MaximumSize = new System.Drawing.Size(10000, 23);
			this.colorlinenumbers.MinimumSize = new System.Drawing.Size(100, 23);
			this.colorlinenumbers.Name = "colorlinenumbers";
			this.colorlinenumbers.Size = new System.Drawing.Size(150, 23);
			this.colorlinenumbers.TabIndex = 11;
			// 
			// colorcomments
			// 
			this.colorcomments.BackColor = System.Drawing.SystemColors.Control;
			this.colorcomments.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.colorcomments.Label = "Comments:";
			this.colorcomments.Location = new System.Drawing.Point(15, 114);
			this.colorcomments.MaximumSize = new System.Drawing.Size(10000, 23);
			this.colorcomments.MinimumSize = new System.Drawing.Size(100, 23);
			this.colorcomments.Name = "colorcomments";
			this.colorcomments.Size = new System.Drawing.Size(150, 23);
			this.colorcomments.TabIndex = 13;
			// 
			// colorplaintext
			// 
			this.colorplaintext.BackColor = System.Drawing.SystemColors.Control;
			this.colorplaintext.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.colorplaintext.Label = "Plain text:";
			this.colorplaintext.Location = new System.Drawing.Point(15, 85);
			this.colorplaintext.MaximumSize = new System.Drawing.Size(10000, 23);
			this.colorplaintext.MinimumSize = new System.Drawing.Size(100, 23);
			this.colorplaintext.Name = "colorplaintext";
			this.colorplaintext.Size = new System.Drawing.Size(150, 23);
			this.colorplaintext.TabIndex = 12;
			// 
			// colorsgroup2
			// 
			this.colorsgroup2.Controls.Add(this.colorselection3d);
			this.colorsgroup2.Controls.Add(this.colorhighlight3d);
			this.colorsgroup2.Controls.Add(this.colorcrosshair3d);
			this.colorsgroup2.Location = new System.Drawing.Point(205, 10);
			this.colorsgroup2.Name = "colorsgroup2";
			this.colorsgroup2.Size = new System.Drawing.Size(181, 325);
			this.colorsgroup2.TabIndex = 11;
			this.colorsgroup2.TabStop = false;
			this.colorsgroup2.Text = " 3D mode ";
			this.colorsgroup2.Visible = false;
			// 
			// colorselection3d
			// 
			this.colorselection3d.BackColor = System.Drawing.SystemColors.Control;
			this.colorselection3d.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.colorselection3d.Label = "Selection:";
			this.colorselection3d.Location = new System.Drawing.Point(15, 85);
			this.colorselection3d.MaximumSize = new System.Drawing.Size(10000, 23);
			this.colorselection3d.MinimumSize = new System.Drawing.Size(100, 23);
			this.colorselection3d.Name = "colorselection3d";
			this.colorselection3d.Size = new System.Drawing.Size(150, 23);
			this.colorselection3d.TabIndex = 8;
			// 
			// colorhighlight3d
			// 
			this.colorhighlight3d.BackColor = System.Drawing.SystemColors.Control;
			this.colorhighlight3d.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.colorhighlight3d.Label = "Highlight:";
			this.colorhighlight3d.Location = new System.Drawing.Point(15, 56);
			this.colorhighlight3d.MaximumSize = new System.Drawing.Size(10000, 23);
			this.colorhighlight3d.MinimumSize = new System.Drawing.Size(100, 23);
			this.colorhighlight3d.Name = "colorhighlight3d";
			this.colorhighlight3d.Size = new System.Drawing.Size(150, 23);
			this.colorhighlight3d.TabIndex = 7;
			// 
			// colorcrosshair3d
			// 
			this.colorcrosshair3d.BackColor = System.Drawing.SystemColors.Control;
			this.colorcrosshair3d.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.colorcrosshair3d.Label = "Crosshair:";
			this.colorcrosshair3d.Location = new System.Drawing.Point(15, 27);
			this.colorcrosshair3d.MaximumSize = new System.Drawing.Size(10000, 23);
			this.colorcrosshair3d.MinimumSize = new System.Drawing.Size(100, 23);
			this.colorcrosshair3d.Name = "colorcrosshair3d";
			this.colorcrosshair3d.Size = new System.Drawing.Size(150, 23);
			this.colorcrosshair3d.TabIndex = 6;
			// 
			// PreferencesForm
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.ClientSize = new System.Drawing.Size(619, 464);
			this.Controls.Add(this.cancel);
			this.Controls.Add(this.apply);
			this.Controls.Add(this.tabs);
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
			this.colorsgroup1.ResumeLayout(false);
			this.tabs.ResumeLayout(false);
			this.tabkeys.ResumeLayout(false);
			this.actioncontrolpanel.ResumeLayout(false);
			this.actioncontrolpanel.PerformLayout();
			this.tabcolors.ResumeLayout(false);
			this.tabcolors.PerformLayout();
			this.colorsgroup3.ResumeLayout(false);
			this.colorsgroup2.ResumeLayout(false);
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
		private ColorControl colorselection;
		private ColorControl colorhighlight;
		private ColorControl colorlinedefs;
		private ColorControl colorvertices;
		private ColorControl colorbackcolor;
		private System.Windows.Forms.GroupBox colorsgroup3;
		private System.Windows.Forms.GroupBox colorsgroup2;
		private ColorControl colorselection3d;
		private ColorControl colorhighlight3d;
		private ColorControl colorcrosshair3d;
		private ColorControl colorscriptbackground;
		private ColorControl colorkeywords;
		private ColorControl colorlinenumbers;
		private ColorControl colorcomments;
		private ColorControl colorplaintext;
		private ColorControl colorliterals;
		private ColorControl colorconstants;
		private ColorControl colorspeciallinedefs;
		private ColorControl colorsoundlinedefs;
		private ColorControl colorassociations;
		private ColorControl colorgrid64;
		private ColorControl colorgrid;
		private System.Windows.Forms.GroupBox colorsgroup1;
		private System.Windows.Forms.CheckBox blackbrowsers;
	}
}