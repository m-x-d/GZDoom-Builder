namespace CodeImp.DoomBuilder.Windows
{
	partial class ThingEditFormUDMF
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.thingtype = new CodeImp.DoomBuilder.Controls.ThingBrowserControl();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.roll = new CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox();
			this.label2 = new System.Windows.Forms.Label();
			this.pitch = new CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox();
			this.label1 = new System.Windows.Forms.Label();
			this.angle = new CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox();
			this.labelAngle = new System.Windows.Forms.Label();
			this.anglecontrol = new CodeImp.DoomBuilder.GZBuilder.Controls.AngleControl();
			this.labelGravity = new System.Windows.Forms.Label();
			this.label7 = new System.Windows.Forms.Label();
			this.tabs = new System.Windows.Forms.TabControl();
			this.tabproperties = new System.Windows.Forms.TabPage();
			this.groupBox9 = new System.Windows.Forms.GroupBox();
			this.rollControl = new CodeImp.DoomBuilder.GZBuilder.Controls.AngleControl();
			this.groupBox8 = new System.Windows.Forms.GroupBox();
			this.pitchControl = new CodeImp.DoomBuilder.GZBuilder.Controls.AngleControl();
			this.groupBox7 = new System.Windows.Forms.GroupBox();
			this.groupBox6 = new System.Windows.Forms.GroupBox();
			this.conversationID = new CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox();
			this.labelID = new System.Windows.Forms.Label();
			this.spritetex = new System.Windows.Forms.Panel();
			this.health = new CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox();
			this.label10 = new System.Windows.Forms.Label();
			this.score = new CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox();
			this.label9 = new System.Windows.Forms.Label();
			this.gravity = new CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox();
			this.groupBox5 = new System.Windows.Forms.GroupBox();
			this.scale = new CodeImp.DoomBuilder.GZBuilder.Controls.PairedFloatControl();
			this.color = new CodeImp.DoomBuilder.GZBuilder.Controls.ColorFieldsControl();
			this.alpha = new CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox();
			this.label8 = new System.Windows.Forms.Label();
			this.renderStyle = new System.Windows.Forms.ComboBox();
			this.label3 = new System.Windows.Forms.Label();
			this.groupBox4 = new System.Windows.Forms.GroupBox();
			this.cbAbsoluteHeight = new System.Windows.Forms.CheckBox();
			this.label4 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.posX = new CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox();
			this.posY = new CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox();
			this.posZ = new CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox();
			this.zlabel = new System.Windows.Forms.Label();
			this.tabeffects = new System.Windows.Forms.TabPage();
			this.settingsgroup = new System.Windows.Forms.GroupBox();
			this.flags = new CodeImp.DoomBuilder.Controls.CheckboxArrayControl();
			this.actiongroup = new System.Windows.Forms.GroupBox();
			this.hexenpanel = new System.Windows.Forms.Panel();
			this.scriptNumbers = new System.Windows.Forms.ComboBox();
			this.scriptNames = new System.Windows.Forms.ComboBox();
			this.cbArgStr = new System.Windows.Forms.CheckBox();
			this.arg2 = new CodeImp.DoomBuilder.Controls.ArgumentBox();
			this.arg1 = new CodeImp.DoomBuilder.Controls.ArgumentBox();
			this.arg0 = new CodeImp.DoomBuilder.Controls.ArgumentBox();
			this.arg3 = new CodeImp.DoomBuilder.Controls.ArgumentBox();
			this.arg4 = new CodeImp.DoomBuilder.Controls.ArgumentBox();
			this.arg1label = new System.Windows.Forms.Label();
			this.arg0label = new System.Windows.Forms.Label();
			this.arg3label = new System.Windows.Forms.Label();
			this.arg2label = new System.Windows.Forms.Label();
			this.arg4label = new System.Windows.Forms.Label();
			this.action = new CodeImp.DoomBuilder.Controls.ActionSelectorControl();
			this.browseaction = new System.Windows.Forms.Button();
			this.doompanel = new System.Windows.Forms.Panel();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.tagSelector = new CodeImp.DoomBuilder.GZBuilder.Controls.TagSelector();
			this.tabcustom = new System.Windows.Forms.TabPage();
			this.fieldslist = new CodeImp.DoomBuilder.Controls.FieldsEditorControl();
			this.cancel = new System.Windows.Forms.Button();
			this.apply = new System.Windows.Forms.Button();
			this.hint = new System.Windows.Forms.PictureBox();
			this.hintlabel = new System.Windows.Forms.Label();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.tabs.SuspendLayout();
			this.tabproperties.SuspendLayout();
			this.groupBox9.SuspendLayout();
			this.groupBox8.SuspendLayout();
			this.groupBox7.SuspendLayout();
			this.groupBox6.SuspendLayout();
			this.groupBox5.SuspendLayout();
			this.groupBox4.SuspendLayout();
			this.tabeffects.SuspendLayout();
			this.settingsgroup.SuspendLayout();
			this.actiongroup.SuspendLayout();
			this.hexenpanel.SuspendLayout();
			this.groupBox3.SuspendLayout();
			this.tabcustom.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.hint)).BeginInit();
			this.SuspendLayout();
			// 
			// groupBox1
			// 
			this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)));
			this.groupBox1.Controls.Add(this.thingtype);
			this.groupBox1.Location = new System.Drawing.Point(6, 6);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(250, 390);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = " Thing ";
			// 
			// thingtype
			// 
			this.thingtype.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.thingtype.Location = new System.Drawing.Point(9, 13);
			this.thingtype.Margin = new System.Windows.Forms.Padding(6);
			this.thingtype.Name = "thingtype";
			this.thingtype.Size = new System.Drawing.Size(232, 374);
			this.thingtype.TabIndex = 0;
			this.thingtype.UseMultiSelection = true;
			this.thingtype.OnTypeDoubleClicked += new CodeImp.DoomBuilder.Controls.ThingBrowserControl.TypeDoubleClickDeletegate(this.thingtype_OnTypeDoubleClicked);
			this.thingtype.OnTypeChanged += new CodeImp.DoomBuilder.Controls.ThingBrowserControl.TypeChangedDeletegate(this.thingtype_OnTypeChanged);
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.roll);
			this.groupBox2.Controls.Add(this.label2);
			this.groupBox2.Controls.Add(this.pitch);
			this.groupBox2.Controls.Add(this.label1);
			this.groupBox2.Controls.Add(this.angle);
			this.groupBox2.Controls.Add(this.labelAngle);
			this.groupBox2.Location = new System.Drawing.Point(488, 6);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(133, 98);
			this.groupBox2.TabIndex = 2;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = " Rotation ";
			// 
			// roll
			// 
			this.roll.AllowDecimal = false;
			this.roll.AllowNegative = false;
			this.roll.AllowRelative = true;
			this.roll.ButtonStep = 1;
			this.roll.ButtonStepFloat = 1F;
			this.roll.Location = new System.Drawing.Point(61, 66);
			this.roll.Name = "roll";
			this.roll.Size = new System.Drawing.Size(57, 24);
			this.roll.StepValues = null;
			this.roll.TabIndex = 24;
			this.roll.WhenTextChanged += new System.EventHandler(this.roll_WhenTextChanged);
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(5, 71);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(44, 14);
			this.label2.TabIndex = 23;
			this.label2.Text = "Roll:";
			this.label2.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// pitch
			// 
			this.pitch.AllowDecimal = false;
			this.pitch.AllowNegative = false;
			this.pitch.AllowRelative = true;
			this.pitch.ButtonStep = 1;
			this.pitch.ButtonStepFloat = 1F;
			this.pitch.Location = new System.Drawing.Point(61, 41);
			this.pitch.Name = "pitch";
			this.pitch.Size = new System.Drawing.Size(57, 24);
			this.pitch.StepValues = null;
			this.pitch.TabIndex = 22;
			this.pitch.WhenTextChanged += new System.EventHandler(this.pitch_WhenTextChanged);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(5, 46);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(44, 14);
			this.label1.TabIndex = 21;
			this.label1.Text = "Pitch:";
			this.label1.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// angle
			// 
			this.angle.AllowDecimal = false;
			this.angle.AllowNegative = true;
			this.angle.AllowRelative = true;
			this.angle.ButtonStep = 1;
			this.angle.ButtonStepFloat = 1F;
			this.angle.Location = new System.Drawing.Point(61, 16);
			this.angle.Name = "angle";
			this.angle.Size = new System.Drawing.Size(57, 24);
			this.angle.StepValues = null;
			this.angle.TabIndex = 10;
			this.angle.WhenTextChanged += new System.EventHandler(this.angle_WhenTextChanged);
			// 
			// labelAngle
			// 
			this.labelAngle.Location = new System.Drawing.Point(5, 21);
			this.labelAngle.Name = "labelAngle";
			this.labelAngle.Size = new System.Drawing.Size(44, 14);
			this.labelAngle.TabIndex = 8;
			this.labelAngle.Text = "Angle:";
			this.labelAngle.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// anglecontrol
			// 
			this.anglecontrol.Angle = 0;
			this.anglecontrol.Location = new System.Drawing.Point(7, 17);
			this.anglecontrol.Name = "anglecontrol";
			this.anglecontrol.Size = new System.Drawing.Size(64, 64);
			this.anglecontrol.TabIndex = 20;
			this.anglecontrol.AngleChanged += new CodeImp.DoomBuilder.GZBuilder.Controls.AngleControl.AngleChangedDelegate(this.anglecontrol_AngleChanged);
			// 
			// labelGravity
			// 
			this.labelGravity.Location = new System.Drawing.Point(42, 21);
			this.labelGravity.Name = "labelGravity";
			this.labelGravity.Size = new System.Drawing.Size(50, 14);
			this.labelGravity.TabIndex = 18;
			this.labelGravity.Text = "Gravity:";
			this.labelGravity.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point(15, 30);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(41, 14);
			this.label7.TabIndex = 9;
			this.label7.Text = "Action:";
			// 
			// tabs
			// 
			this.tabs.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.tabs.Controls.Add(this.tabproperties);
			this.tabs.Controls.Add(this.tabeffects);
			this.tabs.Controls.Add(this.tabcustom);
			this.tabs.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.tabs.ItemSize = new System.Drawing.Size(120, 19);
			this.tabs.Location = new System.Drawing.Point(10, 10);
			this.tabs.Margin = new System.Windows.Forms.Padding(1);
			this.tabs.Name = "tabs";
			this.tabs.SelectedIndex = 0;
			this.tabs.Size = new System.Drawing.Size(635, 429);
			this.tabs.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
			this.tabs.TabIndex = 0;
			// 
			// tabproperties
			// 
			this.tabproperties.Controls.Add(this.groupBox9);
			this.tabproperties.Controls.Add(this.groupBox8);
			this.tabproperties.Controls.Add(this.groupBox7);
			this.tabproperties.Controls.Add(this.groupBox6);
			this.tabproperties.Controls.Add(this.groupBox5);
			this.tabproperties.Controls.Add(this.groupBox4);
			this.tabproperties.Controls.Add(this.groupBox2);
			this.tabproperties.Controls.Add(this.groupBox1);
			this.tabproperties.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.tabproperties.Location = new System.Drawing.Point(4, 23);
			this.tabproperties.Name = "tabproperties";
			this.tabproperties.Padding = new System.Windows.Forms.Padding(3);
			this.tabproperties.Size = new System.Drawing.Size(627, 402);
			this.tabproperties.TabIndex = 0;
			this.tabproperties.Text = "Properties";
			this.tabproperties.UseVisualStyleBackColor = true;
			// 
			// groupBox9
			// 
			this.groupBox9.Controls.Add(this.rollControl);
			this.groupBox9.Location = new System.Drawing.Point(544, 298);
			this.groupBox9.Name = "groupBox9";
			this.groupBox9.Size = new System.Drawing.Size(78, 88);
			this.groupBox9.TabIndex = 26;
			this.groupBox9.TabStop = false;
			this.groupBox9.Text = " Roll ";
			// 
			// rollControl
			// 
			this.rollControl.Angle = -90;
			this.rollControl.Location = new System.Drawing.Point(7, 17);
			this.rollControl.Name = "rollControl";
			this.rollControl.Size = new System.Drawing.Size(64, 64);
			this.rollControl.TabIndex = 20;
			this.rollControl.AngleChanged += new CodeImp.DoomBuilder.GZBuilder.Controls.AngleControl.AngleChangedDelegate(this.rollControl_AngleChanged);
			// 
			// groupBox8
			// 
			this.groupBox8.Controls.Add(this.pitchControl);
			this.groupBox8.Location = new System.Drawing.Point(544, 204);
			this.groupBox8.Name = "groupBox8";
			this.groupBox8.Size = new System.Drawing.Size(78, 88);
			this.groupBox8.TabIndex = 25;
			this.groupBox8.TabStop = false;
			this.groupBox8.Text = " Pitch ";
			// 
			// pitchControl
			// 
			this.pitchControl.Angle = -90;
			this.pitchControl.Location = new System.Drawing.Point(7, 17);
			this.pitchControl.Name = "pitchControl";
			this.pitchControl.Size = new System.Drawing.Size(64, 64);
			this.pitchControl.TabIndex = 20;
			this.pitchControl.AngleChanged += new CodeImp.DoomBuilder.GZBuilder.Controls.AngleControl.AngleChangedDelegate(this.pitchControl_AngleChanged);
			// 
			// groupBox7
			// 
			this.groupBox7.Controls.Add(this.anglecontrol);
			this.groupBox7.Location = new System.Drawing.Point(544, 110);
			this.groupBox7.Name = "groupBox7";
			this.groupBox7.Size = new System.Drawing.Size(78, 88);
			this.groupBox7.TabIndex = 24;
			this.groupBox7.TabStop = false;
			this.groupBox7.Text = " Angle";
			// 
			// groupBox6
			// 
			this.groupBox6.Controls.Add(this.conversationID);
			this.groupBox6.Controls.Add(this.labelID);
			this.groupBox6.Controls.Add(this.spritetex);
			this.groupBox6.Controls.Add(this.health);
			this.groupBox6.Controls.Add(this.label10);
			this.groupBox6.Controls.Add(this.score);
			this.groupBox6.Controls.Add(this.label9);
			this.groupBox6.Controls.Add(this.gravity);
			this.groupBox6.Controls.Add(this.labelGravity);
			this.groupBox6.Location = new System.Drawing.Point(262, 257);
			this.groupBox6.Name = "groupBox6";
			this.groupBox6.Size = new System.Drawing.Size(276, 138);
			this.groupBox6.TabIndex = 23;
			this.groupBox6.TabStop = false;
			this.groupBox6.Text = " Behaviour ";
			// 
			// conversationID
			// 
			this.conversationID.AllowDecimal = false;
			this.conversationID.AllowNegative = false;
			this.conversationID.AllowRelative = false;
			this.conversationID.ButtonStep = 1;
			this.conversationID.ButtonStepFloat = 1F;
			this.conversationID.Location = new System.Drawing.Point(98, 106);
			this.conversationID.Name = "conversationID";
			this.conversationID.Size = new System.Drawing.Size(72, 24);
			this.conversationID.StepValues = null;
			this.conversationID.TabIndex = 26;
			// 
			// labelID
			// 
			this.labelID.AutoSize = true;
			this.labelID.Location = new System.Drawing.Point(6, 111);
			this.labelID.Name = "labelID";
			this.labelID.Size = new System.Drawing.Size(86, 14);
			this.labelID.TabIndex = 25;
			this.labelID.Text = "Conversation ID:";
			// 
			// spritetex
			// 
			this.spritetex.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.spritetex.BackColor = System.Drawing.SystemColors.ControlDark;
			this.spritetex.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
			this.spritetex.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.spritetex.Location = new System.Drawing.Point(177, 27);
			this.spritetex.Name = "spritetex";
			this.spritetex.Size = new System.Drawing.Size(94, 94);
			this.spritetex.TabIndex = 24;
			// 
			// health
			// 
			this.health.AllowDecimal = false;
			this.health.AllowNegative = true;
			this.health.AllowRelative = false;
			this.health.ButtonStep = 8;
			this.health.ButtonStepFloat = 0.1F;
			this.health.Location = new System.Drawing.Point(98, 76);
			this.health.Name = "health";
			this.health.Size = new System.Drawing.Size(72, 24);
			this.health.StepValues = null;
			this.health.TabIndex = 23;
			// 
			// label10
			// 
			this.label10.Location = new System.Drawing.Point(42, 81);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(50, 14);
			this.label10.TabIndex = 22;
			this.label10.Text = "Health:";
			this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// score
			// 
			this.score.AllowDecimal = false;
			this.score.AllowNegative = false;
			this.score.AllowRelative = false;
			this.score.ButtonStep = 8;
			this.score.ButtonStepFloat = 0.1F;
			this.score.Location = new System.Drawing.Point(98, 46);
			this.score.Name = "score";
			this.score.Size = new System.Drawing.Size(72, 24);
			this.score.StepValues = null;
			this.score.TabIndex = 21;
			// 
			// label9
			// 
			this.label9.Location = new System.Drawing.Point(42, 51);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(50, 14);
			this.label9.TabIndex = 20;
			this.label9.Text = "Score:";
			this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// gravity
			// 
			this.gravity.AllowDecimal = true;
			this.gravity.AllowNegative = true;
			this.gravity.AllowRelative = false;
			this.gravity.ButtonStep = 8;
			this.gravity.ButtonStepFloat = 0.1F;
			this.gravity.Location = new System.Drawing.Point(98, 16);
			this.gravity.Name = "gravity";
			this.gravity.Size = new System.Drawing.Size(72, 24);
			this.gravity.StepValues = null;
			this.gravity.TabIndex = 19;
			// 
			// groupBox5
			// 
			this.groupBox5.Controls.Add(this.scale);
			this.groupBox5.Controls.Add(this.color);
			this.groupBox5.Controls.Add(this.alpha);
			this.groupBox5.Controls.Add(this.label8);
			this.groupBox5.Controls.Add(this.renderStyle);
			this.groupBox5.Controls.Add(this.label3);
			this.groupBox5.Location = new System.Drawing.Point(262, 110);
			this.groupBox5.Name = "groupBox5";
			this.groupBox5.Size = new System.Drawing.Size(276, 141);
			this.groupBox5.TabIndex = 22;
			this.groupBox5.TabStop = false;
			this.groupBox5.Text = " Rendering ";
			// 
			// scale
			// 
			this.scale.ButtonStep = 0.1F;
			this.scale.DefaultValue = 1F;
			this.scale.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.scale.Label = "Scale:";
			this.scale.Location = new System.Drawing.Point(3, 17);
			this.scale.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
			this.scale.Name = "scale";
			this.scale.Size = new System.Drawing.Size(268, 26);
			this.scale.TabIndex = 31;
			this.scale.OnValuesChanged += new System.EventHandler(this.scale_OnValuesChanged);
			// 
			// color
			// 
			this.color.DefaultValue = 0;
			this.color.Field = "fillcolor";
			this.color.Label = "Color:";
			this.color.Location = new System.Drawing.Point(42, 108);
			this.color.Name = "color";
			this.color.Size = new System.Drawing.Size(207, 31);
			this.color.TabIndex = 30;
			// 
			// alpha
			// 
			this.alpha.AllowDecimal = true;
			this.alpha.AllowNegative = true;
			this.alpha.AllowRelative = false;
			this.alpha.ButtonStep = 8;
			this.alpha.ButtonStepFloat = 0.1F;
			this.alpha.Location = new System.Drawing.Point(89, 78);
			this.alpha.Name = "alpha";
			this.alpha.Size = new System.Drawing.Size(72, 24);
			this.alpha.StepValues = null;
			this.alpha.TabIndex = 23;
			// 
			// label8
			// 
			this.label8.Location = new System.Drawing.Point(3, 83);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(80, 14);
			this.label8.TabIndex = 25;
			this.label8.Text = "Alpha:";
			this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// renderStyle
			// 
			this.renderStyle.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.renderStyle.FormattingEnabled = true;
			this.renderStyle.Items.AddRange(new object[] {
            "Normal",
            "Translucent",
            "Translucent (Lost Soul)",
            "Translucent (stencil)",
            "Additive",
            "Subtractive",
            "Stencil",
            "Fuzzy",
            "OptFuzzy",
            "None"});
			this.renderStyle.Location = new System.Drawing.Point(89, 50);
			this.renderStyle.Name = "renderStyle";
			this.renderStyle.Size = new System.Drawing.Size(156, 22);
			this.renderStyle.TabIndex = 24;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(3, 53);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(80, 14);
			this.label3.TabIndex = 23;
			this.label3.Text = "Render style:";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// groupBox4
			// 
			this.groupBox4.Controls.Add(this.cbAbsoluteHeight);
			this.groupBox4.Controls.Add(this.label4);
			this.groupBox4.Controls.Add(this.label5);
			this.groupBox4.Controls.Add(this.posX);
			this.groupBox4.Controls.Add(this.posY);
			this.groupBox4.Controls.Add(this.posZ);
			this.groupBox4.Controls.Add(this.zlabel);
			this.groupBox4.Location = new System.Drawing.Point(262, 6);
			this.groupBox4.Name = "groupBox4";
			this.groupBox4.Size = new System.Drawing.Size(220, 98);
			this.groupBox4.TabIndex = 21;
			this.groupBox4.TabStop = false;
			this.groupBox4.Text = " Position";
			// 
			// cbAbsoluteHeight
			// 
			this.cbAbsoluteHeight.AutoSize = true;
			this.cbAbsoluteHeight.Location = new System.Drawing.Point(139, 70);
			this.cbAbsoluteHeight.Name = "cbAbsoluteHeight";
			this.cbAbsoluteHeight.Size = new System.Drawing.Size(69, 18);
			this.cbAbsoluteHeight.TabIndex = 16;
			this.cbAbsoluteHeight.Text = "Absolute";
			this.cbAbsoluteHeight.UseVisualStyleBackColor = true;
			this.cbAbsoluteHeight.CheckedChanged += new System.EventHandler(this.cbAbsoluteHeight_CheckedChanged);
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(5, 21);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(50, 14);
			this.label4.TabIndex = 15;
			this.label4.Text = "X:";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(5, 46);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(50, 14);
			this.label5.TabIndex = 14;
			this.label5.Text = "Y:";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// posX
			// 
			this.posX.AllowDecimal = false;
			this.posX.AllowNegative = true;
			this.posX.AllowRelative = true;
			this.posX.ButtonStep = 8;
			this.posX.ButtonStepFloat = 1F;
			this.posX.Location = new System.Drawing.Point(61, 16);
			this.posX.Name = "posX";
			this.posX.Size = new System.Drawing.Size(72, 24);
			this.posX.StepValues = null;
			this.posX.TabIndex = 13;
			this.posX.WhenTextChanged += new System.EventHandler(this.posX_WhenTextChanged);
			// 
			// posY
			// 
			this.posY.AllowDecimal = false;
			this.posY.AllowNegative = true;
			this.posY.AllowRelative = true;
			this.posY.ButtonStep = 8;
			this.posY.ButtonStepFloat = 1F;
			this.posY.Location = new System.Drawing.Point(61, 41);
			this.posY.Name = "posY";
			this.posY.Size = new System.Drawing.Size(72, 24);
			this.posY.StepValues = null;
			this.posY.TabIndex = 12;
			this.posY.WhenTextChanged += new System.EventHandler(this.posY_WhenTextChanged);
			// 
			// posZ
			// 
			this.posZ.AllowDecimal = false;
			this.posZ.AllowNegative = true;
			this.posZ.AllowRelative = true;
			this.posZ.ButtonStep = 8;
			this.posZ.ButtonStepFloat = 1F;
			this.posZ.Location = new System.Drawing.Point(61, 66);
			this.posZ.Name = "posZ";
			this.posZ.Size = new System.Drawing.Size(72, 24);
			this.posZ.StepValues = null;
			this.posZ.TabIndex = 11;
			this.posZ.WhenTextChanged += new System.EventHandler(this.posZ_WhenTextChanged);
			// 
			// zlabel
			// 
			this.zlabel.Location = new System.Drawing.Point(5, 71);
			this.zlabel.Name = "zlabel";
			this.zlabel.Size = new System.Drawing.Size(50, 14);
			this.zlabel.TabIndex = 9;
			this.zlabel.Text = "Abs. Z:";
			this.zlabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// tabeffects
			// 
			this.tabeffects.Controls.Add(this.settingsgroup);
			this.tabeffects.Controls.Add(this.actiongroup);
			this.tabeffects.Controls.Add(this.groupBox3);
			this.tabeffects.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.tabeffects.Location = new System.Drawing.Point(4, 23);
			this.tabeffects.Name = "tabeffects";
			this.tabeffects.Padding = new System.Windows.Forms.Padding(3);
			this.tabeffects.Size = new System.Drawing.Size(627, 402);
			this.tabeffects.TabIndex = 1;
			this.tabeffects.Text = "Action / ID / Flags";
			this.tabeffects.UseVisualStyleBackColor = true;
			// 
			// settingsgroup
			// 
			this.settingsgroup.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)));
			this.settingsgroup.Controls.Add(this.flags);
			this.settingsgroup.Location = new System.Drawing.Point(6, 238);
			this.settingsgroup.Name = "settingsgroup";
			this.settingsgroup.Size = new System.Drawing.Size(615, 158);
			this.settingsgroup.TabIndex = 23;
			this.settingsgroup.TabStop = false;
			this.settingsgroup.Text = " Flags ";
			// 
			// flags
			// 
			this.flags.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.flags.AutoScroll = true;
			this.flags.Columns = 3;
			this.flags.Location = new System.Drawing.Point(6, 19);
			this.flags.Name = "flags";
			this.flags.Size = new System.Drawing.Size(603, 132);
			this.flags.TabIndex = 0;
			this.flags.VerticalSpacing = 1;
			// 
			// actiongroup
			// 
			this.actiongroup.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.actiongroup.Controls.Add(this.hexenpanel);
			this.actiongroup.Controls.Add(this.label7);
			this.actiongroup.Controls.Add(this.action);
			this.actiongroup.Controls.Add(this.browseaction);
			this.actiongroup.Controls.Add(this.doompanel);
			this.actiongroup.Location = new System.Drawing.Point(6, 78);
			this.actiongroup.Name = "actiongroup";
			this.actiongroup.Size = new System.Drawing.Size(615, 154);
			this.actiongroup.TabIndex = 22;
			this.actiongroup.TabStop = false;
			this.actiongroup.Text = " Action ";
			// 
			// hexenpanel
			// 
			this.hexenpanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.hexenpanel.Controls.Add(this.scriptNumbers);
			this.hexenpanel.Controls.Add(this.scriptNames);
			this.hexenpanel.Controls.Add(this.cbArgStr);
			this.hexenpanel.Controls.Add(this.arg2);
			this.hexenpanel.Controls.Add(this.arg1);
			this.hexenpanel.Controls.Add(this.arg0);
			this.hexenpanel.Controls.Add(this.arg3);
			this.hexenpanel.Controls.Add(this.arg4);
			this.hexenpanel.Controls.Add(this.arg1label);
			this.hexenpanel.Controls.Add(this.arg0label);
			this.hexenpanel.Controls.Add(this.arg3label);
			this.hexenpanel.Controls.Add(this.arg2label);
			this.hexenpanel.Controls.Add(this.arg4label);
			this.hexenpanel.Location = new System.Drawing.Point(6, 53);
			this.hexenpanel.Name = "hexenpanel";
			this.hexenpanel.Size = new System.Drawing.Size(603, 94);
			this.hexenpanel.TabIndex = 13;
			// 
			// scriptNumbers
			// 
			this.scriptNumbers.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
			this.scriptNumbers.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
			this.scriptNumbers.BackColor = System.Drawing.Color.LemonChiffon;
			this.scriptNumbers.FormattingEnabled = true;
			this.scriptNumbers.Location = new System.Drawing.Point(435, 63);
			this.scriptNumbers.Name = "scriptNumbers";
			this.scriptNumbers.Size = new System.Drawing.Size(127, 22);
			this.scriptNumbers.TabIndex = 23;
			// 
			// scriptNames
			// 
			this.scriptNames.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
			this.scriptNames.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
			this.scriptNames.BackColor = System.Drawing.Color.Honeydew;
			this.scriptNames.FormattingEnabled = true;
			this.scriptNames.Location = new System.Drawing.Point(305, 63);
			this.scriptNames.Name = "scriptNames";
			this.scriptNames.Size = new System.Drawing.Size(127, 22);
			this.scriptNames.TabIndex = 22;
			// 
			// cbArgStr
			// 
			this.cbArgStr.Location = new System.Drawing.Point(14, 3);
			this.cbArgStr.Name = "cbArgStr";
			this.cbArgStr.Size = new System.Drawing.Size(63, 40);
			this.cbArgStr.TabIndex = 21;
			this.cbArgStr.Text = "Named script";
			this.cbArgStr.UseVisualStyleBackColor = true;
			this.cbArgStr.CheckedChanged += new System.EventHandler(this.cbArgStr_CheckedChanged);
			// 
			// arg2
			// 
			this.arg2.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.arg2.Location = new System.Drawing.Point(172, 63);
			this.arg2.Name = "arg2";
			this.arg2.Size = new System.Drawing.Size(127, 24);
			this.arg2.TabIndex = 2;
			// 
			// arg1
			// 
			this.arg1.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.arg1.Location = new System.Drawing.Point(172, 37);
			this.arg1.Name = "arg1";
			this.arg1.Size = new System.Drawing.Size(127, 24);
			this.arg1.TabIndex = 1;
			// 
			// arg0
			// 
			this.arg0.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.arg0.Location = new System.Drawing.Point(172, 11);
			this.arg0.Name = "arg0";
			this.arg0.Size = new System.Drawing.Size(127, 24);
			this.arg0.TabIndex = 0;
			// 
			// arg3
			// 
			this.arg3.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.arg3.Location = new System.Drawing.Point(473, 11);
			this.arg3.Name = "arg3";
			this.arg3.Size = new System.Drawing.Size(127, 24);
			this.arg3.TabIndex = 3;
			// 
			// arg4
			// 
			this.arg4.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.arg4.Location = new System.Drawing.Point(473, 37);
			this.arg4.Name = "arg4";
			this.arg4.Size = new System.Drawing.Size(127, 24);
			this.arg4.TabIndex = 4;
			// 
			// arg1label
			// 
			this.arg1label.Location = new System.Drawing.Point(-13, 42);
			this.arg1label.Name = "arg1label";
			this.arg1label.Size = new System.Drawing.Size(179, 14);
			this.arg1label.TabIndex = 14;
			this.arg1label.Text = "Argument 2:";
			this.arg1label.TextAlign = System.Drawing.ContentAlignment.TopRight;
			this.arg1label.UseMnemonic = false;
			// 
			// arg0label
			// 
			this.arg0label.Location = new System.Drawing.Point(-13, 16);
			this.arg0label.Name = "arg0label";
			this.arg0label.Size = new System.Drawing.Size(179, 14);
			this.arg0label.TabIndex = 12;
			this.arg0label.Text = "Argument 1:";
			this.arg0label.TextAlign = System.Drawing.ContentAlignment.TopRight;
			this.arg0label.UseMnemonic = false;
			// 
			// arg3label
			// 
			this.arg3label.Location = new System.Drawing.Point(288, 16);
			this.arg3label.Name = "arg3label";
			this.arg3label.Size = new System.Drawing.Size(179, 14);
			this.arg3label.TabIndex = 20;
			this.arg3label.Text = "Argument 4:";
			this.arg3label.TextAlign = System.Drawing.ContentAlignment.TopRight;
			this.arg3label.UseMnemonic = false;
			// 
			// arg2label
			// 
			this.arg2label.Location = new System.Drawing.Point(-13, 68);
			this.arg2label.Name = "arg2label";
			this.arg2label.Size = new System.Drawing.Size(179, 14);
			this.arg2label.TabIndex = 18;
			this.arg2label.Text = "Argument 3:";
			this.arg2label.TextAlign = System.Drawing.ContentAlignment.TopRight;
			this.arg2label.UseMnemonic = false;
			// 
			// arg4label
			// 
			this.arg4label.Location = new System.Drawing.Point(288, 42);
			this.arg4label.Name = "arg4label";
			this.arg4label.Size = new System.Drawing.Size(179, 14);
			this.arg4label.TabIndex = 16;
			this.arg4label.Text = "Argument 5:";
			this.arg4label.TextAlign = System.Drawing.ContentAlignment.TopRight;
			this.arg4label.UseMnemonic = false;
			// 
			// action
			// 
			this.action.BackColor = System.Drawing.SystemColors.Control;
			this.action.Cursor = System.Windows.Forms.Cursors.Default;
			this.action.Empty = false;
			this.action.GeneralizedCategories = null;
			this.action.Location = new System.Drawing.Point(62, 27);
			this.action.Name = "action";
			this.action.Size = new System.Drawing.Size(513, 21);
			this.action.TabIndex = 0;
			this.action.Value = 402;
			this.action.ValueChanges += new System.EventHandler(this.action_ValueChanges);
			// 
			// browseaction
			// 
			this.browseaction.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.browseaction.Image = global::CodeImp.DoomBuilder.Properties.Resources.List;
			this.browseaction.Location = new System.Drawing.Point(581, 24);
			this.browseaction.Name = "browseaction";
			this.browseaction.Padding = new System.Windows.Forms.Padding(0, 0, 1, 3);
			this.browseaction.Size = new System.Drawing.Size(28, 26);
			this.browseaction.TabIndex = 1;
			this.browseaction.Text = " ";
			this.browseaction.UseVisualStyleBackColor = true;
			this.browseaction.Click += new System.EventHandler(this.browseaction_Click);
			// 
			// doompanel
			// 
			this.doompanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.doompanel.Location = new System.Drawing.Point(6, 54);
			this.doompanel.Name = "doompanel";
			this.doompanel.Size = new System.Drawing.Size(603, 94);
			this.doompanel.TabIndex = 12;
			// 
			// groupBox3
			// 
			this.groupBox3.Controls.Add(this.tagSelector);
			this.groupBox3.Location = new System.Drawing.Point(6, 6);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(615, 66);
			this.groupBox3.TabIndex = 0;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = " Identification ";
			// 
			// tagSelector
			// 
			this.tagSelector.Location = new System.Drawing.Point(6, 19);
			this.tagSelector.Name = "tagSelector";
			this.tagSelector.Size = new System.Drawing.Size(569, 35);
			this.tagSelector.TabIndex = 8;
			// 
			// tabcustom
			// 
			this.tabcustom.Controls.Add(this.fieldslist);
			this.tabcustom.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.tabcustom.Location = new System.Drawing.Point(4, 23);
			this.tabcustom.Name = "tabcustom";
			this.tabcustom.Size = new System.Drawing.Size(627, 402);
			this.tabcustom.TabIndex = 2;
			this.tabcustom.Text = "Custom";
			this.tabcustom.UseVisualStyleBackColor = true;
			this.tabcustom.MouseEnter += new System.EventHandler(this.tabcustom_MouseEnter);
			// 
			// fieldslist
			// 
			this.fieldslist.AllowInsert = true;
			this.fieldslist.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.fieldslist.AutoInsertUserPrefix = true;
			this.fieldslist.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.fieldslist.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.fieldslist.Location = new System.Drawing.Point(8, 9);
			this.fieldslist.Margin = new System.Windows.Forms.Padding(8, 9, 8, 9);
			this.fieldslist.Name = "fieldslist";
			this.fieldslist.PropertyColumnVisible = true;
			this.fieldslist.PropertyColumnWidth = 150;
			this.fieldslist.Size = new System.Drawing.Size(611, 389);
			this.fieldslist.TabIndex = 1;
			this.fieldslist.TypeColumnVisible = true;
			this.fieldslist.TypeColumnWidth = 100;
			this.fieldslist.ValueColumnVisible = true;
			// 
			// cancel
			// 
			this.cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancel.Location = new System.Drawing.Point(533, 446);
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
			this.apply.Location = new System.Drawing.Point(414, 446);
			this.apply.Name = "apply";
			this.apply.Size = new System.Drawing.Size(112, 25);
			this.apply.TabIndex = 1;
			this.apply.Text = "OK";
			this.apply.UseVisualStyleBackColor = true;
			this.apply.Click += new System.EventHandler(this.apply_Click);
			// 
			// hint
			// 
			this.hint.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.hint.Image = global::CodeImp.DoomBuilder.Properties.Resources.Lightbulb;
			this.hint.Location = new System.Drawing.Point(10, 450);
			this.hint.Name = "hint";
			this.hint.Size = new System.Drawing.Size(16, 16);
			this.hint.TabIndex = 3;
			this.hint.TabStop = false;
			// 
			// hintlabel
			// 
			this.hintlabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.hintlabel.AutoSize = true;
			this.hintlabel.Location = new System.Drawing.Point(28, 451);
			this.hintlabel.Name = "hintlabel";
			this.hintlabel.Size = new System.Drawing.Size(310, 14);
			this.hintlabel.TabIndex = 4;
			this.hintlabel.Text = "Select several thing types to randomly assign them to selection";
			// 
			// ThingEditFormUDMF
			// 
			this.AcceptButton = this.apply;
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.CancelButton = this.cancel;
			this.ClientSize = new System.Drawing.Size(655, 478);
			this.Controls.Add(this.hintlabel);
			this.Controls.Add(this.hint);
			this.Controls.Add(this.cancel);
			this.Controls.Add(this.apply);
			this.Controls.Add(this.tabs);
			this.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ThingEditFormUDMF";
			this.Opacity = 0;
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Edit Thing";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ThingEditForm_FormClosing);
			this.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.ThingEditForm_HelpRequested);
			this.groupBox1.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			this.tabs.ResumeLayout(false);
			this.tabproperties.ResumeLayout(false);
			this.groupBox9.ResumeLayout(false);
			this.groupBox8.ResumeLayout(false);
			this.groupBox7.ResumeLayout(false);
			this.groupBox6.ResumeLayout(false);
			this.groupBox6.PerformLayout();
			this.groupBox5.ResumeLayout(false);
			this.groupBox4.ResumeLayout(false);
			this.groupBox4.PerformLayout();
			this.tabeffects.ResumeLayout(false);
			this.settingsgroup.ResumeLayout(false);
			this.actiongroup.ResumeLayout(false);
			this.actiongroup.PerformLayout();
			this.hexenpanel.ResumeLayout(false);
			this.groupBox3.ResumeLayout(false);
			this.tabcustom.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.hint)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TabControl tabs;
		private System.Windows.Forms.TabPage tabproperties;
		private System.Windows.Forms.TabPage tabeffects;
		private System.Windows.Forms.Button cancel;
		private System.Windows.Forms.Button apply;
        private System.Windows.Forms.TabPage tabcustom;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.GroupBox actiongroup;
		private System.Windows.Forms.Panel hexenpanel;
		private System.Windows.Forms.Label arg1label;
		private System.Windows.Forms.Label arg0label;
		private System.Windows.Forms.Label arg3label;
		private System.Windows.Forms.Label arg2label;
		private System.Windows.Forms.Label arg4label;
		private CodeImp.DoomBuilder.Controls.ActionSelectorControl action;
		private System.Windows.Forms.Button browseaction;
		private System.Windows.Forms.Panel doompanel;
		private CodeImp.DoomBuilder.Controls.FieldsEditorControl fieldslist;
		private CodeImp.DoomBuilder.Controls.ArgumentBox arg2;
		private CodeImp.DoomBuilder.Controls.ArgumentBox arg1;
		private CodeImp.DoomBuilder.Controls.ArgumentBox arg0;
		private CodeImp.DoomBuilder.Controls.ArgumentBox arg3;
        private CodeImp.DoomBuilder.Controls.ArgumentBox arg4;
		private CodeImp.DoomBuilder.Controls.ThingBrowserControl thingtype;
        private CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox angle;
		private System.Windows.Forms.CheckBox cbArgStr;
        private System.Windows.Forms.ComboBox scriptNames;
		private System.Windows.Forms.Label labelAngle;
        private CodeImp.DoomBuilder.GZBuilder.Controls.TagSelector tagSelector;
        private System.Windows.Forms.ComboBox scriptNumbers;
		private System.Windows.Forms.Label labelGravity;
		private CodeImp.DoomBuilder.GZBuilder.Controls.AngleControl anglecontrol;
		private System.Windows.Forms.PictureBox hint;
		private System.Windows.Forms.Label hintlabel;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.CheckBox cbAbsoluteHeight;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private Controls.ButtonsNumericTextbox posX;
        private Controls.ButtonsNumericTextbox posY;
        private Controls.ButtonsNumericTextbox posZ;
        private System.Windows.Forms.Label zlabel;
        private Controls.ButtonsNumericTextbox gravity;
        private Controls.ButtonsNumericTextbox roll;
        private System.Windows.Forms.Label label2;
        private Controls.ButtonsNumericTextbox pitch;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox5;
        private Controls.ButtonsNumericTextbox alpha;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.ComboBox renderStyle;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox groupBox6;
        private Controls.ButtonsNumericTextbox health;
        private System.Windows.Forms.Label label10;
        private Controls.ButtonsNumericTextbox score;
        private System.Windows.Forms.Label label9;
        private GZBuilder.Controls.ColorFieldsControl color;
        private System.Windows.Forms.Panel spritetex;
        private System.Windows.Forms.GroupBox settingsgroup;
        private Controls.CheckboxArrayControl flags;
        private Controls.ButtonsNumericTextbox conversationID;
        private System.Windows.Forms.Label labelID;
        private System.Windows.Forms.GroupBox groupBox9;
        private GZBuilder.Controls.AngleControl rollControl;
        private System.Windows.Forms.GroupBox groupBox8;
        private GZBuilder.Controls.AngleControl pitchControl;
        private System.Windows.Forms.GroupBox groupBox7;
        private GZBuilder.Controls.PairedFloatControl scale;
	}
}