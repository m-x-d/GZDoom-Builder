namespace CodeImp.DoomBuilder.Windows
{
	partial class ThingEditForm
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
			System.Windows.Forms.GroupBox groupBox1;
			System.Windows.Forms.GroupBox groupBox2;
			System.Windows.Forms.Label label7;
			this.thingtype = new CodeImp.DoomBuilder.Controls.ThingBrowserControl();
			this.gravity = new CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox();
			this.labelGravity = new System.Windows.Forms.Label();
			this.cbRandomAngle = new System.Windows.Forms.CheckBox();
			this.cbAbsoluteHeight = new System.Windows.Forms.CheckBox();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.posX = new CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox();
			this.posY = new CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox();
			this.posZ = new CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox();
			this.angle = new CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox();
			this.zlabel = new System.Windows.Forms.Label();
			this.anglecontrol = new CodeImp.DoomBuilder.Controls.AngleControl();
			this.labelAngle = new System.Windows.Forms.Label();
			this.tabs = new System.Windows.Forms.TabControl();
			this.tabproperties = new System.Windows.Forms.TabPage();
			this.spritetex = new System.Windows.Forms.Panel();
			this.settingsgroup = new System.Windows.Forms.GroupBox();
			this.flags = new CodeImp.DoomBuilder.Controls.CheckboxArrayControl();
			this.tabeffects = new System.Windows.Forms.TabPage();
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
			this.conversationID = new CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox();
			this.labelID = new System.Windows.Forms.Label();
			this.tagSelector = new CodeImp.DoomBuilder.GZBuilder.Controls.TagSelector();
			this.tabcustom = new System.Windows.Forms.TabPage();
			this.fieldslist = new CodeImp.DoomBuilder.Controls.FieldsEditorControl();
			this.cancel = new System.Windows.Forms.Button();
			this.apply = new System.Windows.Forms.Button();
			groupBox1 = new System.Windows.Forms.GroupBox();
			groupBox2 = new System.Windows.Forms.GroupBox();
			label7 = new System.Windows.Forms.Label();
			groupBox1.SuspendLayout();
			groupBox2.SuspendLayout();
			this.tabs.SuspendLayout();
			this.tabproperties.SuspendLayout();
			this.settingsgroup.SuspendLayout();
			this.tabeffects.SuspendLayout();
			this.actiongroup.SuspendLayout();
			this.hexenpanel.SuspendLayout();
			this.groupBox3.SuspendLayout();
			this.tabcustom.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBox1
			// 
			groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)));
			groupBox1.Controls.Add(this.thingtype);
			groupBox1.Location = new System.Drawing.Point(6, 6);
			groupBox1.Name = "groupBox1";
			groupBox1.Size = new System.Drawing.Size(269, 373);
			groupBox1.TabIndex = 0;
			groupBox1.TabStop = false;
			groupBox1.Text = " Thing ";
			// 
			// thingtype
			// 
			this.thingtype.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)));
			this.thingtype.Location = new System.Drawing.Point(9, 13);
			this.thingtype.Margin = new System.Windows.Forms.Padding(6);
			this.thingtype.Name = "thingtype";
			this.thingtype.Size = new System.Drawing.Size(251, 357);
			this.thingtype.TabIndex = 0;
			this.thingtype.UseMultiSelection = true;
			this.thingtype.OnTypeDoubleClicked += new CodeImp.DoomBuilder.Controls.ThingBrowserControl.TypeDoubleClickDeletegate(this.thingtype_OnTypeDoubleClicked);
			this.thingtype.OnTypeChanged += new CodeImp.DoomBuilder.Controls.ThingBrowserControl.TypeChangedDeletegate(this.thingtype_OnTypeChanged);
			// 
			// groupBox2
			// 
			groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			groupBox2.Controls.Add(this.gravity);
			groupBox2.Controls.Add(this.labelGravity);
			groupBox2.Controls.Add(this.cbRandomAngle);
			groupBox2.Controls.Add(this.cbAbsoluteHeight);
			groupBox2.Controls.Add(this.label2);
			groupBox2.Controls.Add(this.label1);
			groupBox2.Controls.Add(this.posX);
			groupBox2.Controls.Add(this.posY);
			groupBox2.Controls.Add(this.posZ);
			groupBox2.Controls.Add(this.angle);
			groupBox2.Controls.Add(this.zlabel);
			groupBox2.Controls.Add(this.anglecontrol);
			groupBox2.Controls.Add(this.labelAngle);
			groupBox2.Location = new System.Drawing.Point(394, 216);
			groupBox2.Name = "groupBox2";
			groupBox2.Size = new System.Drawing.Size(252, 164);
			groupBox2.TabIndex = 2;
			groupBox2.TabStop = false;
			groupBox2.Text = " Coordination ";
			// 
			// gravity
			// 
			this.gravity.AllowDecimal = true;
			this.gravity.AllowNegative = true;
			this.gravity.AllowRelative = false;
			this.gravity.ButtonStep = 8;
			this.gravity.ButtonStepFloat = 0.1F;
			this.gravity.Location = new System.Drawing.Point(61, 118);
			this.gravity.Name = "gravity";
			this.gravity.Size = new System.Drawing.Size(72, 24);
			this.gravity.StepValues = null;
			this.gravity.TabIndex = 19;
			// 
			// labelGravity
			// 
			this.labelGravity.Location = new System.Drawing.Point(5, 123);
			this.labelGravity.Name = "labelGravity";
			this.labelGravity.Size = new System.Drawing.Size(50, 14);
			this.labelGravity.TabIndex = 18;
			this.labelGravity.Text = "Gravity:";
			this.labelGravity.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// cbRandomAngle
			// 
			this.cbRandomAngle.AutoSize = true;
			this.cbRandomAngle.Location = new System.Drawing.Point(151, 19);
			this.cbRandomAngle.Name = "cbRandomAngle";
			this.cbRandomAngle.Size = new System.Drawing.Size(94, 18);
			this.cbRandomAngle.TabIndex = 17;
			this.cbRandomAngle.Text = "Random angle";
			this.cbRandomAngle.UseVisualStyleBackColor = true;
			this.cbRandomAngle.CheckedChanged += new System.EventHandler(this.cbRandomAngle_CheckedChanged);
			// 
			// cbAbsoluteHeight
			// 
			this.cbAbsoluteHeight.AutoSize = true;
			this.cbAbsoluteHeight.Location = new System.Drawing.Point(37, 94);
			this.cbAbsoluteHeight.Name = "cbAbsoluteHeight";
			this.cbAbsoluteHeight.Size = new System.Drawing.Size(102, 18);
			this.cbAbsoluteHeight.TabIndex = 16;
			this.cbAbsoluteHeight.Text = "Absolute Height";
			this.cbAbsoluteHeight.UseVisualStyleBackColor = true;
			this.cbAbsoluteHeight.CheckedChanged += new System.EventHandler(this.cbAbsoluteHeight_CheckedChanged);
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(5, 21);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(50, 14);
			this.label2.TabIndex = 15;
			this.label2.Text = "X:";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(5, 46);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(50, 14);
			this.label1.TabIndex = 14;
			this.label1.Text = "Y:";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
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
			// angle
			// 
			this.angle.AllowDecimal = false;
			this.angle.AllowNegative = true;
			this.angle.AllowRelative = true;
			this.angle.ButtonStep = 1;
			this.angle.ButtonStepFloat = 1F;
			this.angle.Location = new System.Drawing.Point(186, 41);
			this.angle.Name = "angle";
			this.angle.Size = new System.Drawing.Size(57, 24);
			this.angle.StepValues = null;
			this.angle.TabIndex = 10;
			this.angle.WhenTextChanged += new System.EventHandler(this.angle_TextChanged);
			// 
			// zlabel
			// 
			this.zlabel.Location = new System.Drawing.Point(5, 71);
			this.zlabel.Name = "zlabel";
			this.zlabel.Size = new System.Drawing.Size(50, 14);
			this.zlabel.TabIndex = 9;
			this.zlabel.Text = "Height:";
			this.zlabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// anglecontrol
			// 
			this.anglecontrol.BackColor = System.Drawing.SystemColors.Control;
			this.anglecontrol.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.anglecontrol.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.anglecontrol.Location = new System.Drawing.Point(152, 71);
			this.anglecontrol.Name = "anglecontrol";
			this.anglecontrol.Size = new System.Drawing.Size(84, 84);
			this.anglecontrol.TabIndex = 2;
			this.anglecontrol.Value = 0;
			this.anglecontrol.ButtonClicked += new System.EventHandler(this.anglecontrol_ButtonClicked);
			// 
			// labelAngle
			// 
			this.labelAngle.AutoSize = true;
			this.labelAngle.Location = new System.Drawing.Point(149, 46);
			this.labelAngle.Name = "labelAngle";
			this.labelAngle.Size = new System.Drawing.Size(38, 14);
			this.labelAngle.TabIndex = 8;
			this.labelAngle.Text = "Angle:";
			// 
			// label7
			// 
			label7.AutoSize = true;
			label7.Location = new System.Drawing.Point(15, 30);
			label7.Name = "label7";
			label7.Size = new System.Drawing.Size(41, 14);
			label7.TabIndex = 9;
			label7.Text = "Action:";
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
			this.tabs.Location = new System.Drawing.Point(10, 10);
			this.tabs.Margin = new System.Windows.Forms.Padding(1);
			this.tabs.Name = "tabs";
			this.tabs.SelectedIndex = 0;
			this.tabs.Size = new System.Drawing.Size(660, 412);
			this.tabs.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
			this.tabs.TabIndex = 0;
			// 
			// tabproperties
			// 
			this.tabproperties.Controls.Add(this.spritetex);
			this.tabproperties.Controls.Add(groupBox2);
			this.tabproperties.Controls.Add(this.settingsgroup);
			this.tabproperties.Controls.Add(groupBox1);
			this.tabproperties.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.tabproperties.Location = new System.Drawing.Point(4, 23);
			this.tabproperties.Name = "tabproperties";
			this.tabproperties.Padding = new System.Windows.Forms.Padding(3);
			this.tabproperties.Size = new System.Drawing.Size(652, 385);
			this.tabproperties.TabIndex = 0;
			this.tabproperties.Text = "Properties";
			this.tabproperties.UseVisualStyleBackColor = true;
			// 
			// spritetex
			// 
			this.spritetex.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.spritetex.BackColor = System.Drawing.SystemColors.AppWorkspace;
			this.spritetex.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
			this.spritetex.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.spritetex.Location = new System.Drawing.Point(284, 222);
			this.spritetex.Name = "spritetex";
			this.spritetex.Size = new System.Drawing.Size(104, 100);
			this.spritetex.TabIndex = 22;
			// 
			// settingsgroup
			// 
			this.settingsgroup.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.settingsgroup.Controls.Add(this.flags);
			this.settingsgroup.Location = new System.Drawing.Point(284, 6);
			this.settingsgroup.Name = "settingsgroup";
			this.settingsgroup.Size = new System.Drawing.Size(362, 204);
			this.settingsgroup.TabIndex = 1;
			this.settingsgroup.TabStop = false;
			this.settingsgroup.Text = " Settings ";
			// 
			// flags
			// 
			this.flags.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.flags.AutoScroll = true;
			this.flags.Columns = 3;
			this.flags.Location = new System.Drawing.Point(19, 19);
			this.flags.Name = "flags";
			this.flags.Size = new System.Drawing.Size(337, 178);
			this.flags.TabIndex = 0;
			this.flags.VerticalSpacing = 1;
			// 
			// tabeffects
			// 
			this.tabeffects.Controls.Add(this.actiongroup);
			this.tabeffects.Controls.Add(this.groupBox3);
			this.tabeffects.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.tabeffects.Location = new System.Drawing.Point(4, 23);
			this.tabeffects.Name = "tabeffects";
			this.tabeffects.Padding = new System.Windows.Forms.Padding(3);
			this.tabeffects.Size = new System.Drawing.Size(652, 385);
			this.tabeffects.TabIndex = 1;
			this.tabeffects.Text = "Action";
			this.tabeffects.UseVisualStyleBackColor = true;
			// 
			// actiongroup
			// 
			this.actiongroup.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.actiongroup.Controls.Add(this.hexenpanel);
			this.actiongroup.Controls.Add(label7);
			this.actiongroup.Controls.Add(this.action);
			this.actiongroup.Controls.Add(this.browseaction);
			this.actiongroup.Controls.Add(this.doompanel);
			this.actiongroup.Location = new System.Drawing.Point(6, 78);
			this.actiongroup.Name = "actiongroup";
			this.actiongroup.Size = new System.Drawing.Size(640, 306);
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
			this.hexenpanel.Size = new System.Drawing.Size(628, 246);
			this.hexenpanel.TabIndex = 13;
			// 
			// scriptNumbers
			// 
			this.scriptNumbers.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
			this.scriptNumbers.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
			this.scriptNumbers.BackColor = System.Drawing.Color.LemonChiffon;
			this.scriptNumbers.FormattingEnabled = true;
			this.scriptNumbers.Location = new System.Drawing.Point(179, 121);
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
			this.scriptNames.Location = new System.Drawing.Point(179, 93);
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
			this.arg2.Location = new System.Drawing.Point(179, 63);
			this.arg2.Name = "arg2";
			this.arg2.Size = new System.Drawing.Size(127, 24);
			this.arg2.TabIndex = 2;
			// 
			// arg1
			// 
			this.arg1.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.arg1.Location = new System.Drawing.Point(179, 37);
			this.arg1.Name = "arg1";
			this.arg1.Size = new System.Drawing.Size(127, 24);
			this.arg1.TabIndex = 1;
			// 
			// arg0
			// 
			this.arg0.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.arg0.Location = new System.Drawing.Point(179, 11);
			this.arg0.Name = "arg0";
			this.arg0.Size = new System.Drawing.Size(127, 24);
			this.arg0.TabIndex = 0;
			// 
			// arg3
			// 
			this.arg3.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.arg3.Location = new System.Drawing.Point(487, 11);
			this.arg3.Name = "arg3";
			this.arg3.Size = new System.Drawing.Size(127, 24);
			this.arg3.TabIndex = 3;
			// 
			// arg4
			// 
			this.arg4.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.arg4.Location = new System.Drawing.Point(487, 37);
			this.arg4.Name = "arg4";
			this.arg4.Size = new System.Drawing.Size(127, 24);
			this.arg4.TabIndex = 4;
			// 
			// arg1label
			// 
			this.arg1label.Location = new System.Drawing.Point(-6, 42);
			this.arg1label.Name = "arg1label";
			this.arg1label.Size = new System.Drawing.Size(179, 14);
			this.arg1label.TabIndex = 14;
			this.arg1label.Text = "Argument 2:";
			this.arg1label.TextAlign = System.Drawing.ContentAlignment.TopRight;
			this.arg1label.UseMnemonic = false;
			// 
			// arg0label
			// 
			this.arg0label.Location = new System.Drawing.Point(-6, 16);
			this.arg0label.Name = "arg0label";
			this.arg0label.Size = new System.Drawing.Size(179, 14);
			this.arg0label.TabIndex = 12;
			this.arg0label.Text = "Argument 1:";
			this.arg0label.TextAlign = System.Drawing.ContentAlignment.TopRight;
			this.arg0label.UseMnemonic = false;
			// 
			// arg3label
			// 
			this.arg3label.Location = new System.Drawing.Point(302, 16);
			this.arg3label.Name = "arg3label";
			this.arg3label.Size = new System.Drawing.Size(179, 14);
			this.arg3label.TabIndex = 20;
			this.arg3label.Text = "Argument 4:";
			this.arg3label.TextAlign = System.Drawing.ContentAlignment.TopRight;
			this.arg3label.UseMnemonic = false;
			// 
			// arg2label
			// 
			this.arg2label.Location = new System.Drawing.Point(-6, 68);
			this.arg2label.Name = "arg2label";
			this.arg2label.Size = new System.Drawing.Size(179, 14);
			this.arg2label.TabIndex = 18;
			this.arg2label.Text = "Argument 3:";
			this.arg2label.TextAlign = System.Drawing.ContentAlignment.TopRight;
			this.arg2label.UseMnemonic = false;
			// 
			// arg4label
			// 
			this.arg4label.Location = new System.Drawing.Point(302, 42);
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
			this.action.Size = new System.Drawing.Size(524, 21);
			this.action.TabIndex = 0;
			this.action.Value = 402;
			this.action.ValueChanges += new System.EventHandler(this.action_ValueChanges);
			// 
			// browseaction
			// 
			this.browseaction.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.browseaction.Image = global::CodeImp.DoomBuilder.Properties.Resources.List;
			this.browseaction.Location = new System.Drawing.Point(592, 25);
			this.browseaction.Name = "browseaction";
			this.browseaction.Padding = new System.Windows.Forms.Padding(0, 0, 1, 3);
			this.browseaction.Size = new System.Drawing.Size(28, 25);
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
			this.doompanel.Size = new System.Drawing.Size(628, 246);
			this.doompanel.TabIndex = 12;
			// 
			// groupBox3
			// 
			this.groupBox3.Controls.Add(this.conversationID);
			this.groupBox3.Controls.Add(this.labelID);
			this.groupBox3.Controls.Add(this.tagSelector);
			this.groupBox3.Location = new System.Drawing.Point(6, 6);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(640, 66);
			this.groupBox3.TabIndex = 0;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = " Identification ";
			// 
			// conversationID
			// 
			this.conversationID.AllowDecimal = false;
			this.conversationID.AllowNegative = false;
			this.conversationID.AllowRelative = false;
			this.conversationID.ButtonStep = 1;
			this.conversationID.ButtonStepFloat = 1F;
			this.conversationID.Location = new System.Drawing.Point(548, 26);
			this.conversationID.Name = "conversationID";
			this.conversationID.Size = new System.Drawing.Size(72, 24);
			this.conversationID.StepValues = null;
			this.conversationID.TabIndex = 14;
			// 
			// labelID
			// 
			this.labelID.AutoSize = true;
			this.labelID.Location = new System.Drawing.Point(456, 31);
			this.labelID.Name = "labelID";
			this.labelID.Size = new System.Drawing.Size(86, 14);
			this.labelID.TabIndex = 10;
			this.labelID.Text = "Conversation ID:";
			// 
			// tagSelector
			// 
			this.tagSelector.Location = new System.Drawing.Point(6, 19);
			this.tagSelector.Name = "tagSelector";
			this.tagSelector.Size = new System.Drawing.Size(430, 35);
			this.tagSelector.TabIndex = 8;
			// 
			// tabcustom
			// 
			this.tabcustom.Controls.Add(this.fieldslist);
			this.tabcustom.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.tabcustom.Location = new System.Drawing.Point(4, 23);
			this.tabcustom.Name = "tabcustom";
			this.tabcustom.Size = new System.Drawing.Size(652, 385);
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
			this.fieldslist.Size = new System.Drawing.Size(636, 372);
			this.fieldslist.TabIndex = 1;
			this.fieldslist.TypeColumnVisible = true;
			this.fieldslist.TypeColumnWidth = 100;
			this.fieldslist.ValueColumnVisible = true;
			// 
			// cancel
			// 
			this.cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancel.Location = new System.Drawing.Point(558, 429);
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
			this.apply.Location = new System.Drawing.Point(439, 429);
			this.apply.Name = "apply";
			this.apply.Size = new System.Drawing.Size(112, 25);
			this.apply.TabIndex = 1;
			this.apply.Text = "OK";
			this.apply.UseVisualStyleBackColor = true;
			this.apply.Click += new System.EventHandler(this.apply_Click);
			// 
			// ThingEditForm
			// 
			this.AcceptButton = this.apply;
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.CancelButton = this.cancel;
			this.ClientSize = new System.Drawing.Size(680, 461);
			this.Controls.Add(this.cancel);
			this.Controls.Add(this.apply);
			this.Controls.Add(this.tabs);
			this.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ThingEditForm";
			this.Opacity = 0;
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Edit Thing";
			this.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.ThingEditForm_HelpRequested);
			groupBox1.ResumeLayout(false);
			groupBox2.ResumeLayout(false);
			groupBox2.PerformLayout();
			this.tabs.ResumeLayout(false);
			this.tabproperties.ResumeLayout(false);
			this.settingsgroup.ResumeLayout(false);
			this.tabeffects.ResumeLayout(false);
			this.actiongroup.ResumeLayout(false);
			this.actiongroup.PerformLayout();
			this.hexenpanel.ResumeLayout(false);
			this.groupBox3.ResumeLayout(false);
			this.groupBox3.PerformLayout();
			this.tabcustom.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TabControl tabs;
		private System.Windows.Forms.TabPage tabproperties;
		private System.Windows.Forms.TabPage tabeffects;
		private System.Windows.Forms.Button cancel;
		private System.Windows.Forms.Button apply;
		private System.Windows.Forms.TabPage tabcustom;
		private System.Windows.Forms.GroupBox settingsgroup;
		private CodeImp.DoomBuilder.Controls.CheckboxArrayControl flags;
		private System.Windows.Forms.Panel spritetex;
		private CodeImp.DoomBuilder.Controls.AngleControl anglecontrol;
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
		private System.Windows.Forms.Label zlabel;
		private CodeImp.DoomBuilder.Controls.ThingBrowserControl thingtype;
		private CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox angle;
		private CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox posZ;
		private CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox posY;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label1;
		private CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox posX;
		private System.Windows.Forms.CheckBox cbArgStr;
		private System.Windows.Forms.ComboBox scriptNames;
		private System.Windows.Forms.CheckBox cbAbsoluteHeight;
		private System.Windows.Forms.CheckBox cbRandomAngle;
		private System.Windows.Forms.Label labelAngle;
		private CodeImp.DoomBuilder.GZBuilder.Controls.TagSelector tagSelector;
		private CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox conversationID;
		private System.Windows.Forms.Label labelID;
		private System.Windows.Forms.ComboBox scriptNumbers;
		private CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox gravity;
		private System.Windows.Forms.Label labelGravity;
	}
}