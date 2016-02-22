namespace CodeImp.DoomBuilder.Windows
{
	partial class SectorEditFormUDMF
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
			System.Windows.Forms.GroupBox groupaction;
			System.Windows.Forms.GroupBox groupeffect;
			System.Windows.Forms.Label label14;
			System.Windows.Forms.Label label9;
			System.Windows.Forms.Label label13;
			System.Windows.Forms.Label label2;
			System.Windows.Forms.Label label8;
			System.Windows.Forms.GroupBox groupfloorceiling;
			System.Windows.Forms.Label label15;
			System.Windows.Forms.Label label6;
			System.Windows.Forms.Label label5;
			System.Windows.Forms.Label label17;
			System.Windows.Forms.Label label16;
			System.Windows.Forms.Label label18;
			System.Windows.Forms.Label label19;
			this.tagsselector = new CodeImp.DoomBuilder.GZBuilder.Controls.TagsSelector();
			this.soundsequence = new System.Windows.Forms.ComboBox();
			this.resetsoundsequence = new System.Windows.Forms.Button();
			this.fadeColor = new CodeImp.DoomBuilder.GZBuilder.Controls.ColorFieldsControl();
			this.lightColor = new CodeImp.DoomBuilder.GZBuilder.Controls.ColorFieldsControl();
			this.brightness = new CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox();
			this.desaturation = new CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox();
			this.gravity = new CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox();
			this.browseeffect = new System.Windows.Forms.Button();
			this.effect = new CodeImp.DoomBuilder.Controls.ActionSelectorControl();
			this.heightoffset = new CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox();
			this.ceilingheight = new CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox();
			this.sectorheightlabel = new System.Windows.Forms.Label();
			this.sectorheight = new System.Windows.Forms.Label();
			this.floorheight = new CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox();
			this.tabs = new System.Windows.Forms.TabControl();
			this.tabproperties = new System.Windows.Forms.TabPage();
			this.groupdamage = new System.Windows.Forms.GroupBox();
			this.leakiness = new CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox();
			this.damageinterval = new CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox();
			this.resetdamagetype = new System.Windows.Forms.Button();
			this.damageamount = new CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox();
			this.damagetype = new System.Windows.Forms.ComboBox();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.flags = new CodeImp.DoomBuilder.Controls.CheckboxArrayControl();
			this.tabSurfaces = new System.Windows.Forms.TabPage();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.resetfloorterrain = new System.Windows.Forms.Button();
			this.label3 = new System.Windows.Forms.Label();
			this.floorterrain = new System.Windows.Forms.ComboBox();
			this.resetfloorlight = new System.Windows.Forms.Button();
			this.labelFloorOffsets = new System.Windows.Forms.Label();
			this.labelFloorScale = new System.Windows.Forms.Label();
			this.cbUseFloorLineAngles = new System.Windows.Forms.CheckBox();
			this.floorAngleControl = new CodeImp.DoomBuilder.GZBuilder.Controls.AngleControl();
			this.labelfloorrenderstyle = new System.Windows.Forms.Label();
			this.label10 = new System.Windows.Forms.Label();
			this.floorAlpha = new CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox();
			this.label11 = new System.Windows.Forms.Label();
			this.floorRotation = new CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox();
			this.floorLightAbsolute = new System.Windows.Forms.CheckBox();
			this.label12 = new System.Windows.Forms.Label();
			this.floorBrightness = new CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox();
			this.floorRenderStyle = new System.Windows.Forms.ComboBox();
			this.floorScale = new CodeImp.DoomBuilder.GZBuilder.Controls.PairedFieldsControl();
			this.floorOffsets = new CodeImp.DoomBuilder.GZBuilder.Controls.PairedFieldsControl();
			this.floortex = new CodeImp.DoomBuilder.Controls.FlatSelectorControl();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.resetceilterrain = new System.Windows.Forms.Button();
			this.label7 = new System.Windows.Forms.Label();
			this.ceilterrain = new System.Windows.Forms.ComboBox();
			this.resetceillight = new System.Windows.Forms.Button();
			this.labelCeilOffsets = new System.Windows.Forms.Label();
			this.labelCeilScale = new System.Windows.Forms.Label();
			this.cbUseCeilLineAngles = new System.Windows.Forms.CheckBox();
			this.ceilAngleControl = new CodeImp.DoomBuilder.GZBuilder.Controls.AngleControl();
			this.labelceilrenderstyle = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.ceilAlpha = new CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox();
			this.label1 = new System.Windows.Forms.Label();
			this.ceilRotation = new CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox();
			this.ceilLightAbsolute = new System.Windows.Forms.CheckBox();
			this.labelLightFront = new System.Windows.Forms.Label();
			this.ceilBrightness = new CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox();
			this.ceilRenderStyle = new System.Windows.Forms.ComboBox();
			this.ceilScale = new CodeImp.DoomBuilder.GZBuilder.Controls.PairedFieldsControl();
			this.ceilOffsets = new CodeImp.DoomBuilder.GZBuilder.Controls.PairedFieldsControl();
			this.ceilingtex = new CodeImp.DoomBuilder.Controls.FlatSelectorControl();
			this.tabslopes = new System.Windows.Forms.TabPage();
			this.groupBox5 = new System.Windows.Forms.GroupBox();
			this.floorslopecontrol = new CodeImp.DoomBuilder.Controls.SectorSlopeControl();
			this.groupBox4 = new System.Windows.Forms.GroupBox();
			this.ceilingslopecontrol = new CodeImp.DoomBuilder.Controls.SectorSlopeControl();
			this.tabcomment = new System.Windows.Forms.TabPage();
			this.commenteditor = new CodeImp.DoomBuilder.Controls.CommentEditor();
			this.tabcustom = new System.Windows.Forms.TabPage();
			this.fieldslist = new CodeImp.DoomBuilder.Controls.FieldsEditorControl();
			this.cancel = new System.Windows.Forms.Button();
			this.apply = new System.Windows.Forms.Button();
			this.tooltip = new System.Windows.Forms.ToolTip(this.components);
			groupaction = new System.Windows.Forms.GroupBox();
			groupeffect = new System.Windows.Forms.GroupBox();
			label14 = new System.Windows.Forms.Label();
			label9 = new System.Windows.Forms.Label();
			label13 = new System.Windows.Forms.Label();
			label2 = new System.Windows.Forms.Label();
			label8 = new System.Windows.Forms.Label();
			groupfloorceiling = new System.Windows.Forms.GroupBox();
			label15 = new System.Windows.Forms.Label();
			label6 = new System.Windows.Forms.Label();
			label5 = new System.Windows.Forms.Label();
			label17 = new System.Windows.Forms.Label();
			label16 = new System.Windows.Forms.Label();
			label18 = new System.Windows.Forms.Label();
			label19 = new System.Windows.Forms.Label();
			groupaction.SuspendLayout();
			groupeffect.SuspendLayout();
			groupfloorceiling.SuspendLayout();
			this.tabs.SuspendLayout();
			this.tabproperties.SuspendLayout();
			this.groupdamage.SuspendLayout();
			this.groupBox3.SuspendLayout();
			this.tabSurfaces.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.tabslopes.SuspendLayout();
			this.groupBox5.SuspendLayout();
			this.groupBox4.SuspendLayout();
			this.tabcomment.SuspendLayout();
			this.tabcustom.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupaction
			// 
			groupaction.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			groupaction.Controls.Add(this.tagsselector);
			groupaction.Location = new System.Drawing.Point(7, 394);
			groupaction.Name = "groupaction";
			groupaction.Size = new System.Drawing.Size(490, 80);
			groupaction.TabIndex = 2;
			groupaction.TabStop = false;
			groupaction.Text = " Identification ";
			// 
			// tagsselector
			// 
			this.tagsselector.Location = new System.Drawing.Point(6, 15);
			this.tagsselector.Name = "tagsselector";
			this.tagsselector.Size = new System.Drawing.Size(478, 60);
			this.tagsselector.TabIndex = 0;
			// 
			// groupeffect
			// 
			groupeffect.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			groupeffect.Controls.Add(this.soundsequence);
			groupeffect.Controls.Add(this.resetsoundsequence);
			groupeffect.Controls.Add(this.fadeColor);
			groupeffect.Controls.Add(this.lightColor);
			groupeffect.Controls.Add(this.brightness);
			groupeffect.Controls.Add(this.desaturation);
			groupeffect.Controls.Add(label14);
			groupeffect.Controls.Add(label9);
			groupeffect.Controls.Add(label13);
			groupeffect.Controls.Add(this.gravity);
			groupeffect.Controls.Add(label2);
			groupeffect.Controls.Add(this.browseeffect);
			groupeffect.Controls.Add(this.effect);
			groupeffect.Controls.Add(label8);
			groupeffect.Location = new System.Drawing.Point(7, 256);
			groupeffect.Name = "groupeffect";
			groupeffect.Size = new System.Drawing.Size(490, 132);
			groupeffect.TabIndex = 1;
			groupeffect.TabStop = false;
			groupeffect.Text = " Effects ";
			// 
			// soundsequence
			// 
			this.soundsequence.FormattingEnabled = true;
			this.soundsequence.Location = new System.Drawing.Point(283, 48);
			this.soundsequence.Name = "soundsequence";
			this.soundsequence.Size = new System.Drawing.Size(167, 21);
			this.soundsequence.TabIndex = 32;
			this.soundsequence.MouseDown += new System.Windows.Forms.MouseEventHandler(this.soundsequence_MouseDown);
			this.soundsequence.TextChanged += new System.EventHandler(this.soundsequence_TextChanged);
			// 
			// resetsoundsequence
			// 
			this.resetsoundsequence.Image = global::CodeImp.DoomBuilder.Properties.Resources.Reset;
			this.resetsoundsequence.Location = new System.Drawing.Point(456, 46);
			this.resetsoundsequence.Name = "resetsoundsequence";
			this.resetsoundsequence.Size = new System.Drawing.Size(28, 25);
			this.resetsoundsequence.TabIndex = 31;
			this.resetsoundsequence.Text = " ";
			this.resetsoundsequence.UseVisualStyleBackColor = true;
			this.resetsoundsequence.Click += new System.EventHandler(this.resetsoundsequence_Click);
			// 
			// fadeColor
			// 
			this.fadeColor.DefaultValue = 0;
			this.fadeColor.Field = "fadecolor";
			this.fadeColor.Label = "Fade:";
			this.fadeColor.Location = new System.Drawing.Point(236, 99);
			this.fadeColor.Name = "fadeColor";
			this.fadeColor.Size = new System.Drawing.Size(207, 31);
			this.fadeColor.TabIndex = 30;
			this.fadeColor.OnValueChanged += new System.EventHandler(this.fadeColor_OnValueChanged);
			// 
			// lightColor
			// 
			this.lightColor.DefaultValue = 16777215;
			this.lightColor.Field = "lightcolor";
			this.lightColor.Label = "Light:";
			this.lightColor.Location = new System.Drawing.Point(236, 71);
			this.lightColor.Name = "lightColor";
			this.lightColor.Size = new System.Drawing.Size(207, 29);
			this.lightColor.TabIndex = 29;
			this.lightColor.OnValueChanged += new System.EventHandler(this.lightColor_OnValueChanged);
			// 
			// brightness
			// 
			this.brightness.AllowDecimal = false;
			this.brightness.AllowNegative = false;
			this.brightness.AllowRelative = true;
			this.brightness.ButtonStep = 8;
			this.brightness.ButtonStepBig = 16F;
			this.brightness.ButtonStepFloat = 1F;
			this.brightness.ButtonStepSmall = 1F;
			this.brightness.ButtonStepsUseModifierKeys = true;
			this.brightness.ButtonStepsWrapAround = false;
			this.brightness.Location = new System.Drawing.Point(89, 46);
			this.brightness.Name = "brightness";
			this.brightness.Size = new System.Drawing.Size(81, 24);
			this.brightness.StepValues = null;
			this.brightness.TabIndex = 24;
			this.brightness.WhenTextChanged += new System.EventHandler(this.brightness_WhenTextChanged);
			// 
			// desaturation
			// 
			this.desaturation.AllowDecimal = true;
			this.desaturation.AllowNegative = false;
			this.desaturation.AllowRelative = false;
			this.desaturation.ButtonStep = 1;
			this.desaturation.ButtonStepBig = 0.25F;
			this.desaturation.ButtonStepFloat = 0.1F;
			this.desaturation.ButtonStepSmall = 0.01F;
			this.desaturation.ButtonStepsUseModifierKeys = true;
			this.desaturation.ButtonStepsWrapAround = false;
			this.desaturation.Location = new System.Drawing.Point(89, 102);
			this.desaturation.Name = "desaturation";
			this.desaturation.Size = new System.Drawing.Size(81, 24);
			this.desaturation.StepValues = null;
			this.desaturation.TabIndex = 28;
			// 
			// label14
			// 
			label14.AutoSize = true;
			label14.Location = new System.Drawing.Point(186, 52);
			label14.Name = "label14";
			label14.Size = new System.Drawing.Size(91, 13);
			label14.TabIndex = 3;
			label14.Text = "Sound sequence:";
			// 
			// label9
			// 
			label9.AutoSize = true;
			label9.Location = new System.Drawing.Point(21, 51);
			label9.Name = "label9";
			label9.Size = new System.Drawing.Size(59, 13);
			label9.TabIndex = 2;
			label9.Text = "Brightness:";
			// 
			// label13
			// 
			label13.Location = new System.Drawing.Point(9, 107);
			label13.Name = "label13";
			label13.Size = new System.Drawing.Size(74, 14);
			label13.TabIndex = 27;
			label13.Text = "Desaturation:";
			label13.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// gravity
			// 
			this.gravity.AllowDecimal = true;
			this.gravity.AllowNegative = true;
			this.gravity.AllowRelative = true;
			this.gravity.ButtonStep = 1;
			this.gravity.ButtonStepBig = 1F;
			this.gravity.ButtonStepFloat = 0.1F;
			this.gravity.ButtonStepSmall = 0.01F;
			this.gravity.ButtonStepsUseModifierKeys = true;
			this.gravity.ButtonStepsWrapAround = false;
			this.gravity.Location = new System.Drawing.Point(89, 74);
			this.gravity.Name = "gravity";
			this.gravity.Size = new System.Drawing.Size(81, 24);
			this.gravity.StepValues = null;
			this.gravity.TabIndex = 24;
			// 
			// label2
			// 
			label2.Location = new System.Drawing.Point(9, 79);
			label2.Name = "label2";
			label2.Size = new System.Drawing.Size(74, 14);
			label2.TabIndex = 23;
			label2.Text = "Gravity:";
			label2.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// browseeffect
			// 
			this.browseeffect.Image = global::CodeImp.DoomBuilder.Properties.Resources.List;
			this.browseeffect.Location = new System.Drawing.Point(456, 16);
			this.browseeffect.Name = "browseeffect";
			this.browseeffect.Size = new System.Drawing.Size(28, 25);
			this.browseeffect.TabIndex = 1;
			this.browseeffect.Text = " ";
			this.browseeffect.UseVisualStyleBackColor = true;
			this.browseeffect.Click += new System.EventHandler(this.browseeffect_Click);
			// 
			// effect
			// 
			this.effect.BackColor = System.Drawing.Color.Transparent;
			this.effect.Cursor = System.Windows.Forms.Cursors.Default;
			this.effect.Empty = false;
			this.effect.GeneralizedCategories = null;
			this.effect.GeneralizedOptions = null;
			this.effect.Location = new System.Drawing.Point(89, 18);
			this.effect.Name = "effect";
			this.effect.Size = new System.Drawing.Size(361, 21);
			this.effect.TabIndex = 0;
			this.effect.Value = 402;
			// 
			// label8
			// 
			label8.AutoSize = true;
			label8.Location = new System.Drawing.Point(35, 22);
			label8.Name = "label8";
			label8.Size = new System.Drawing.Size(45, 13);
			label8.TabIndex = 0;
			label8.Text = "Special:";
			// 
			// groupfloorceiling
			// 
			groupfloorceiling.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			groupfloorceiling.Controls.Add(label15);
			groupfloorceiling.Controls.Add(label6);
			groupfloorceiling.Controls.Add(label5);
			groupfloorceiling.Controls.Add(this.heightoffset);
			groupfloorceiling.Controls.Add(this.ceilingheight);
			groupfloorceiling.Controls.Add(this.sectorheightlabel);
			groupfloorceiling.Controls.Add(this.sectorheight);
			groupfloorceiling.Controls.Add(this.floorheight);
			groupfloorceiling.Location = new System.Drawing.Point(7, 112);
			groupfloorceiling.Name = "groupfloorceiling";
			groupfloorceiling.Size = new System.Drawing.Size(187, 138);
			groupfloorceiling.TabIndex = 0;
			groupfloorceiling.TabStop = false;
			groupfloorceiling.Text = " Heights ";
			// 
			// label15
			// 
			label15.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			label15.ForeColor = System.Drawing.SystemColors.HotTrack;
			label15.Location = new System.Drawing.Point(9, 83);
			label15.Name = "label15";
			label15.Size = new System.Drawing.Size(74, 14);
			label15.TabIndex = 27;
			label15.Text = "Height offset:";
			label15.TextAlign = System.Drawing.ContentAlignment.TopRight;
			this.tooltip.SetToolTip(label15, "Changes floor and ceiling height by given value.\r\nUse \"++\" to raise by sector hei" +
					"ght.\r\nUse \"--\" to lower by sector height.");
			// 
			// label6
			// 
			label6.Location = new System.Drawing.Point(9, 23);
			label6.Name = "label6";
			label6.Size = new System.Drawing.Size(74, 14);
			label6.TabIndex = 19;
			label6.Text = "Ceiling height:";
			label6.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// label5
			// 
			label5.Location = new System.Drawing.Point(9, 53);
			label5.Name = "label5";
			label5.Size = new System.Drawing.Size(74, 14);
			label5.TabIndex = 17;
			label5.Text = "Floor height:";
			label5.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// heightoffset
			// 
			this.heightoffset.AllowDecimal = false;
			this.heightoffset.AllowNegative = true;
			this.heightoffset.AllowRelative = true;
			this.heightoffset.ButtonStep = 8;
			this.heightoffset.ButtonStepBig = 16F;
			this.heightoffset.ButtonStepFloat = 1F;
			this.heightoffset.ButtonStepSmall = 1F;
			this.heightoffset.ButtonStepsUseModifierKeys = true;
			this.heightoffset.ButtonStepsWrapAround = false;
			this.heightoffset.Location = new System.Drawing.Point(89, 78);
			this.heightoffset.Name = "heightoffset";
			this.heightoffset.Size = new System.Drawing.Size(81, 24);
			this.heightoffset.StepValues = null;
			this.heightoffset.TabIndex = 28;
			this.heightoffset.WhenTextChanged += new System.EventHandler(this.heightoffset_WhenTextChanged);
			// 
			// ceilingheight
			// 
			this.ceilingheight.AllowDecimal = false;
			this.ceilingheight.AllowNegative = true;
			this.ceilingheight.AllowRelative = true;
			this.ceilingheight.ButtonStep = 8;
			this.ceilingheight.ButtonStepBig = 16F;
			this.ceilingheight.ButtonStepFloat = 1F;
			this.ceilingheight.ButtonStepSmall = 1F;
			this.ceilingheight.ButtonStepsUseModifierKeys = true;
			this.ceilingheight.ButtonStepsWrapAround = false;
			this.ceilingheight.Location = new System.Drawing.Point(89, 18);
			this.ceilingheight.Name = "ceilingheight";
			this.ceilingheight.Size = new System.Drawing.Size(81, 24);
			this.ceilingheight.StepValues = null;
			this.ceilingheight.TabIndex = 22;
			this.ceilingheight.WhenTextChanged += new System.EventHandler(this.ceilingheight_WhenTextChanged);
			// 
			// sectorheightlabel
			// 
			this.sectorheightlabel.Location = new System.Drawing.Point(9, 113);
			this.sectorheightlabel.Name = "sectorheightlabel";
			this.sectorheightlabel.Size = new System.Drawing.Size(74, 14);
			this.sectorheightlabel.TabIndex = 20;
			this.sectorheightlabel.Text = "Sector height:";
			this.sectorheightlabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// sectorheight
			// 
			this.sectorheight.AutoSize = true;
			this.sectorheight.Location = new System.Drawing.Point(89, 114);
			this.sectorheight.Name = "sectorheight";
			this.sectorheight.Size = new System.Drawing.Size(13, 13);
			this.sectorheight.TabIndex = 21;
			this.sectorheight.Text = "0";
			// 
			// floorheight
			// 
			this.floorheight.AllowDecimal = false;
			this.floorheight.AllowNegative = true;
			this.floorheight.AllowRelative = true;
			this.floorheight.ButtonStep = 8;
			this.floorheight.ButtonStepBig = 16F;
			this.floorheight.ButtonStepFloat = 1F;
			this.floorheight.ButtonStepSmall = 1F;
			this.floorheight.ButtonStepsUseModifierKeys = true;
			this.floorheight.ButtonStepsWrapAround = false;
			this.floorheight.Location = new System.Drawing.Point(89, 48);
			this.floorheight.Name = "floorheight";
			this.floorheight.Size = new System.Drawing.Size(81, 24);
			this.floorheight.StepValues = null;
			this.floorheight.TabIndex = 23;
			this.floorheight.WhenTextChanged += new System.EventHandler(this.floorheight_WhenTextChanged);
			// 
			// label17
			// 
			label17.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			label17.ForeColor = System.Drawing.SystemColors.HotTrack;
			label17.Location = new System.Drawing.Point(10, 51);
			label17.Name = "label17";
			label17.Size = new System.Drawing.Size(74, 14);
			label17.TabIndex = 29;
			label17.Text = "Ammount:";
			label17.TextAlign = System.Drawing.ContentAlignment.TopRight;
			this.tooltip.SetToolTip(label17, "Amount of damage inflicted by this sector.\r\nIf this is 0, all other damage proper" +
					"ties will be ignored.\r\nSetting this to a negative value will create a healing se" +
					"ctor.");
			// 
			// label16
			// 
			label16.Location = new System.Drawing.Point(10, 23);
			label16.Name = "label16";
			label16.Size = new System.Drawing.Size(74, 14);
			label16.TabIndex = 36;
			label16.Text = "Type:";
			label16.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// label18
			// 
			label18.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			label18.ForeColor = System.Drawing.SystemColors.HotTrack;
			label18.Location = new System.Drawing.Point(10, 81);
			label18.Name = "label18";
			label18.Size = new System.Drawing.Size(74, 14);
			label18.TabIndex = 37;
			label18.Text = "Interval:";
			label18.TextAlign = System.Drawing.ContentAlignment.TopRight;
			this.tooltip.SetToolTip(label18, "Interval in tics between damage application.");
			// 
			// label19
			// 
			label19.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			label19.ForeColor = System.Drawing.SystemColors.HotTrack;
			label19.Location = new System.Drawing.Point(10, 111);
			label19.Name = "label19";
			label19.Size = new System.Drawing.Size(74, 14);
			label19.TabIndex = 39;
			label19.Text = "Leakiness:";
			label19.TextAlign = System.Drawing.ContentAlignment.TopRight;
			this.tooltip.SetToolTip(label19, "Probability of leaking through radiation suit\r\n(0 = never, 256 = always)");
			// 
			// tabs
			// 
			this.tabs.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.tabs.Controls.Add(this.tabproperties);
			this.tabs.Controls.Add(this.tabSurfaces);
			this.tabs.Controls.Add(this.tabslopes);
			this.tabs.Controls.Add(this.tabcomment);
			this.tabs.Controls.Add(this.tabcustom);
			this.tabs.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.tabs.Location = new System.Drawing.Point(10, 10);
			this.tabs.Margin = new System.Windows.Forms.Padding(1);
			this.tabs.Name = "tabs";
			this.tabs.Padding = new System.Drawing.Point(24, 3);
			this.tabs.SelectedIndex = 0;
			this.tabs.Size = new System.Drawing.Size(511, 504);
			this.tabs.TabIndex = 1;
			// 
			// tabproperties
			// 
			this.tabproperties.Controls.Add(this.groupdamage);
			this.tabproperties.Controls.Add(this.groupBox3);
			this.tabproperties.Controls.Add(groupaction);
			this.tabproperties.Controls.Add(groupeffect);
			this.tabproperties.Controls.Add(groupfloorceiling);
			this.tabproperties.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.tabproperties.Location = new System.Drawing.Point(4, 22);
			this.tabproperties.Name = "tabproperties";
			this.tabproperties.Padding = new System.Windows.Forms.Padding(3);
			this.tabproperties.Size = new System.Drawing.Size(503, 478);
			this.tabproperties.TabIndex = 0;
			this.tabproperties.Text = "Properties";
			this.tabproperties.UseVisualStyleBackColor = true;
			// 
			// groupdamage
			// 
			this.groupdamage.Controls.Add(label19);
			this.groupdamage.Controls.Add(this.leakiness);
			this.groupdamage.Controls.Add(label18);
			this.groupdamage.Controls.Add(this.damageinterval);
			this.groupdamage.Controls.Add(label16);
			this.groupdamage.Controls.Add(label17);
			this.groupdamage.Controls.Add(this.resetdamagetype);
			this.groupdamage.Controls.Add(this.damageamount);
			this.groupdamage.Controls.Add(this.damagetype);
			this.groupdamage.Location = new System.Drawing.Point(200, 112);
			this.groupdamage.Name = "groupdamage";
			this.groupdamage.Size = new System.Drawing.Size(297, 136);
			this.groupdamage.TabIndex = 4;
			this.groupdamage.TabStop = false;
			this.groupdamage.Text = " Sector damage ";
			// 
			// leakiness
			// 
			this.leakiness.AllowDecimal = false;
			this.leakiness.AllowNegative = false;
			this.leakiness.AllowRelative = true;
			this.leakiness.ButtonStep = 8;
			this.leakiness.ButtonStepBig = 16F;
			this.leakiness.ButtonStepFloat = 1F;
			this.leakiness.ButtonStepSmall = 1F;
			this.leakiness.ButtonStepsUseModifierKeys = true;
			this.leakiness.ButtonStepsWrapAround = false;
			this.leakiness.Location = new System.Drawing.Point(90, 106);
			this.leakiness.Name = "leakiness";
			this.leakiness.Size = new System.Drawing.Size(81, 24);
			this.leakiness.StepValues = null;
			this.leakiness.TabIndex = 40;
			// 
			// damageinterval
			// 
			this.damageinterval.AllowDecimal = false;
			this.damageinterval.AllowNegative = false;
			this.damageinterval.AllowRelative = true;
			this.damageinterval.ButtonStep = 8;
			this.damageinterval.ButtonStepBig = 16F;
			this.damageinterval.ButtonStepFloat = 1F;
			this.damageinterval.ButtonStepSmall = 1F;
			this.damageinterval.ButtonStepsUseModifierKeys = true;
			this.damageinterval.ButtonStepsWrapAround = false;
			this.damageinterval.Location = new System.Drawing.Point(90, 76);
			this.damageinterval.Name = "damageinterval";
			this.damageinterval.Size = new System.Drawing.Size(81, 24);
			this.damageinterval.StepValues = null;
			this.damageinterval.TabIndex = 38;
			// 
			// resetdamagetype
			// 
			this.resetdamagetype.Image = global::CodeImp.DoomBuilder.Properties.Resources.Reset;
			this.resetdamagetype.Location = new System.Drawing.Point(263, 17);
			this.resetdamagetype.Name = "resetdamagetype";
			this.resetdamagetype.Size = new System.Drawing.Size(28, 25);
			this.resetdamagetype.TabIndex = 35;
			this.resetdamagetype.Text = " ";
			this.resetdamagetype.UseVisualStyleBackColor = true;
			this.resetdamagetype.Click += new System.EventHandler(this.resetdamagetype_Click);
			// 
			// damageamount
			// 
			this.damageamount.AllowDecimal = false;
			this.damageamount.AllowNegative = true;
			this.damageamount.AllowRelative = true;
			this.damageamount.ButtonStep = 8;
			this.damageamount.ButtonStepBig = 16F;
			this.damageamount.ButtonStepFloat = 1F;
			this.damageamount.ButtonStepSmall = 1F;
			this.damageamount.ButtonStepsUseModifierKeys = true;
			this.damageamount.ButtonStepsWrapAround = false;
			this.damageamount.Location = new System.Drawing.Point(90, 46);
			this.damageamount.Name = "damageamount";
			this.damageamount.Size = new System.Drawing.Size(81, 24);
			this.damageamount.StepValues = null;
			this.damageamount.TabIndex = 30;
			// 
			// damagetype
			// 
			this.damagetype.FormattingEnabled = true;
			this.damagetype.Location = new System.Drawing.Point(90, 19);
			this.damagetype.Name = "damagetype";
			this.damagetype.Size = new System.Drawing.Size(167, 21);
			this.damagetype.TabIndex = 34;
			this.damagetype.MouseDown += new System.Windows.Forms.MouseEventHandler(this.damagetype_MouseDown);
			this.damagetype.TextChanged += new System.EventHandler(this.damagetype_TextChanged);
			// 
			// groupBox3
			// 
			this.groupBox3.Controls.Add(this.flags);
			this.groupBox3.Location = new System.Drawing.Point(7, 6);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(490, 100);
			this.groupBox3.TabIndex = 3;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = " Flags ";
			// 
			// flags
			// 
			this.flags.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.flags.AutoScroll = true;
			this.flags.Columns = 2;
			this.flags.Location = new System.Drawing.Point(15, 21);
			this.flags.Name = "flags";
			this.flags.Size = new System.Drawing.Size(469, 73);
			this.flags.TabIndex = 5;
			this.flags.VerticalSpacing = 1;
			// 
			// tabSurfaces
			// 
			this.tabSurfaces.Controls.Add(this.groupBox2);
			this.tabSurfaces.Controls.Add(this.groupBox1);
			this.tabSurfaces.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.tabSurfaces.Location = new System.Drawing.Point(4, 22);
			this.tabSurfaces.Name = "tabSurfaces";
			this.tabSurfaces.Size = new System.Drawing.Size(503, 478);
			this.tabSurfaces.TabIndex = 2;
			this.tabSurfaces.Text = "Surfaces";
			this.tabSurfaces.UseVisualStyleBackColor = true;
			// 
			// groupBox2
			// 
			this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox2.Controls.Add(this.resetfloorterrain);
			this.groupBox2.Controls.Add(this.label3);
			this.groupBox2.Controls.Add(this.floorterrain);
			this.groupBox2.Controls.Add(this.resetfloorlight);
			this.groupBox2.Controls.Add(this.labelFloorOffsets);
			this.groupBox2.Controls.Add(this.labelFloorScale);
			this.groupBox2.Controls.Add(this.cbUseFloorLineAngles);
			this.groupBox2.Controls.Add(this.floorAngleControl);
			this.groupBox2.Controls.Add(this.labelfloorrenderstyle);
			this.groupBox2.Controls.Add(this.label10);
			this.groupBox2.Controls.Add(this.floorAlpha);
			this.groupBox2.Controls.Add(this.label11);
			this.groupBox2.Controls.Add(this.floorRotation);
			this.groupBox2.Controls.Add(this.floorLightAbsolute);
			this.groupBox2.Controls.Add(this.label12);
			this.groupBox2.Controls.Add(this.floorBrightness);
			this.groupBox2.Controls.Add(this.floorRenderStyle);
			this.groupBox2.Controls.Add(this.floorScale);
			this.groupBox2.Controls.Add(this.floorOffsets);
			this.groupBox2.Controls.Add(this.floortex);
			this.groupBox2.Location = new System.Drawing.Point(3, 239);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(497, 230);
			this.groupBox2.TabIndex = 55;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = " Floor ";
			// 
			// resetfloorterrain
			// 
			this.resetfloorterrain.Image = global::CodeImp.DoomBuilder.Properties.Resources.Reset;
			this.resetfloorterrain.Location = new System.Drawing.Point(246, 110);
			this.resetfloorterrain.Name = "resetfloorterrain";
			this.resetfloorterrain.Size = new System.Drawing.Size(23, 23);
			this.resetfloorterrain.TabIndex = 66;
			this.resetfloorterrain.Text = " ";
			this.resetfloorterrain.UseVisualStyleBackColor = true;
			this.resetfloorterrain.Click += new System.EventHandler(this.resetfloorterrain_Click);
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(24, 114);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(80, 14);
			this.label3.TabIndex = 62;
			this.label3.Tag = "";
			this.label3.Text = "Terrain:";
			this.label3.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// floorterrain
			// 
			this.floorterrain.FormattingEnabled = true;
			this.floorterrain.Location = new System.Drawing.Point(113, 111);
			this.floorterrain.Name = "floorterrain";
			this.floorterrain.Size = new System.Drawing.Size(130, 21);
			this.floorterrain.TabIndex = 61;
			this.floorterrain.MouseDown += new System.Windows.Forms.MouseEventHandler(this.floorterrain_MouseDown);
			this.floorterrain.TextChanged += new System.EventHandler(this.floorterrain_TextChanged);
			// 
			// resetfloorlight
			// 
			this.resetfloorlight.Image = global::CodeImp.DoomBuilder.Properties.Resources.Reset;
			this.resetfloorlight.Location = new System.Drawing.Point(246, 138);
			this.resetfloorlight.Name = "resetfloorlight";
			this.resetfloorlight.Size = new System.Drawing.Size(23, 23);
			this.resetfloorlight.TabIndex = 60;
			this.tooltip.SetToolTip(this.resetfloorlight, "Reset Floor Brightness");
			this.resetfloorlight.UseVisualStyleBackColor = true;
			this.resetfloorlight.Click += new System.EventHandler(this.resetfloorlight_Click);
			// 
			// labelFloorOffsets
			// 
			this.labelFloorOffsets.Location = new System.Drawing.Point(6, 27);
			this.labelFloorOffsets.Name = "labelFloorOffsets";
			this.labelFloorOffsets.Size = new System.Drawing.Size(98, 14);
			this.labelFloorOffsets.TabIndex = 59;
			this.labelFloorOffsets.Tag = "";
			this.labelFloorOffsets.Text = "Texture Offsets:";
			this.labelFloorOffsets.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// labelFloorScale
			// 
			this.labelFloorScale.Location = new System.Drawing.Point(9, 59);
			this.labelFloorScale.Name = "labelFloorScale";
			this.labelFloorScale.Size = new System.Drawing.Size(95, 14);
			this.labelFloorScale.TabIndex = 58;
			this.labelFloorScale.Tag = "";
			this.labelFloorScale.Text = "Texture Scale:";
			this.labelFloorScale.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// cbUseFloorLineAngles
			// 
			this.cbUseFloorLineAngles.AutoSize = true;
			this.cbUseFloorLineAngles.Location = new System.Drawing.Point(181, 172);
			this.cbUseFloorLineAngles.Name = "cbUseFloorLineAngles";
			this.cbUseFloorLineAngles.Size = new System.Drawing.Size(113, 17);
			this.cbUseFloorLineAngles.TabIndex = 57;
			this.cbUseFloorLineAngles.Tag = "";
			this.cbUseFloorLineAngles.Text = "Use linedef angles";
			this.cbUseFloorLineAngles.UseVisualStyleBackColor = true;
			this.cbUseFloorLineAngles.CheckedChanged += new System.EventHandler(this.cbUseFloorLineAngles_CheckedChanged);
			// 
			// floorAngleControl
			// 
			this.floorAngleControl.Angle = -1350;
			this.floorAngleControl.AngleOffset = 90;
			this.floorAngleControl.Location = new System.Drawing.Point(6, 156);
			this.floorAngleControl.Name = "floorAngleControl";
			this.floorAngleControl.Size = new System.Drawing.Size(44, 44);
			this.floorAngleControl.TabIndex = 56;
			this.floorAngleControl.AngleChanged += new System.EventHandler(this.floorAngleControl_AngleChanged);
			// 
			// labelfloorrenderstyle
			// 
			this.labelfloorrenderstyle.Location = new System.Drawing.Point(24, 88);
			this.labelfloorrenderstyle.Name = "labelfloorrenderstyle";
			this.labelfloorrenderstyle.Size = new System.Drawing.Size(80, 14);
			this.labelfloorrenderstyle.TabIndex = 54;
			this.labelfloorrenderstyle.Tag = "";
			this.labelfloorrenderstyle.Text = "Render style:";
			this.labelfloorrenderstyle.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// label10
			// 
			this.label10.Location = new System.Drawing.Point(24, 202);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(80, 14);
			this.label10.TabIndex = 52;
			this.label10.Tag = "";
			this.label10.Text = "Alpha:";
			this.label10.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// floorAlpha
			// 
			this.floorAlpha.AllowDecimal = true;
			this.floorAlpha.AllowNegative = false;
			this.floorAlpha.AllowRelative = false;
			this.floorAlpha.ButtonStep = 1;
			this.floorAlpha.ButtonStepBig = 0.25F;
			this.floorAlpha.ButtonStepFloat = 0.1F;
			this.floorAlpha.ButtonStepSmall = 0.01F;
			this.floorAlpha.ButtonStepsUseModifierKeys = true;
			this.floorAlpha.ButtonStepsWrapAround = false;
			this.floorAlpha.Location = new System.Drawing.Point(113, 197);
			this.floorAlpha.Name = "floorAlpha";
			this.floorAlpha.Size = new System.Drawing.Size(62, 24);
			this.floorAlpha.StepValues = null;
			this.floorAlpha.TabIndex = 53;
			this.floorAlpha.Tag = "";
			// 
			// label11
			// 
			this.label11.Location = new System.Drawing.Point(24, 172);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(80, 14);
			this.label11.TabIndex = 50;
			this.label11.Tag = "";
			this.label11.Text = "Rotation:";
			this.label11.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// floorRotation
			// 
			this.floorRotation.AllowDecimal = true;
			this.floorRotation.AllowNegative = true;
			this.floorRotation.AllowRelative = true;
			this.floorRotation.ButtonStep = 5;
			this.floorRotation.ButtonStepBig = 15F;
			this.floorRotation.ButtonStepFloat = 1F;
			this.floorRotation.ButtonStepSmall = 0.1F;
			this.floorRotation.ButtonStepsUseModifierKeys = true;
			this.floorRotation.ButtonStepsWrapAround = false;
			this.floorRotation.Location = new System.Drawing.Point(113, 167);
			this.floorRotation.Name = "floorRotation";
			this.floorRotation.Size = new System.Drawing.Size(62, 24);
			this.floorRotation.StepValues = null;
			this.floorRotation.TabIndex = 51;
			this.floorRotation.Tag = "";
			this.floorRotation.WhenTextChanged += new System.EventHandler(this.floorRotation_WhenTextChanged);
			// 
			// floorLightAbsolute
			// 
			this.floorLightAbsolute.AutoSize = true;
			this.floorLightAbsolute.Location = new System.Drawing.Point(181, 142);
			this.floorLightAbsolute.Name = "floorLightAbsolute";
			this.floorLightAbsolute.Size = new System.Drawing.Size(67, 17);
			this.floorLightAbsolute.TabIndex = 49;
			this.floorLightAbsolute.Text = "Absolute";
			this.floorLightAbsolute.UseVisualStyleBackColor = true;
			this.floorLightAbsolute.CheckedChanged += new System.EventHandler(this.floorLightAbsolute_CheckedChanged);
			// 
			// label12
			// 
			this.label12.Location = new System.Drawing.Point(24, 142);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(80, 14);
			this.label12.TabIndex = 47;
			this.label12.Tag = "";
			this.label12.Text = "Brightness:";
			this.label12.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// floorBrightness
			// 
			this.floorBrightness.AllowDecimal = false;
			this.floorBrightness.AllowNegative = true;
			this.floorBrightness.AllowRelative = true;
			this.floorBrightness.ButtonStep = 16;
			this.floorBrightness.ButtonStepBig = 32F;
			this.floorBrightness.ButtonStepFloat = 1F;
			this.floorBrightness.ButtonStepSmall = 1F;
			this.floorBrightness.ButtonStepsUseModifierKeys = true;
			this.floorBrightness.ButtonStepsWrapAround = false;
			this.floorBrightness.Location = new System.Drawing.Point(113, 137);
			this.floorBrightness.Name = "floorBrightness";
			this.floorBrightness.Size = new System.Drawing.Size(62, 24);
			this.floorBrightness.StepValues = null;
			this.floorBrightness.TabIndex = 48;
			this.floorBrightness.Tag = "lightfloor";
			this.floorBrightness.WhenTextChanged += new System.EventHandler(this.floorBrightness_WhenTextChanged);
			// 
			// floorRenderStyle
			// 
			this.floorRenderStyle.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.floorRenderStyle.FormattingEnabled = true;
			this.floorRenderStyle.Location = new System.Drawing.Point(113, 85);
			this.floorRenderStyle.Name = "floorRenderStyle";
			this.floorRenderStyle.Size = new System.Drawing.Size(130, 21);
			this.floorRenderStyle.TabIndex = 46;
			// 
			// floorScale
			// 
			this.floorScale.AllowDecimal = true;
			this.floorScale.AllowValueLinking = true;
			this.floorScale.ButtonStep = 1;
			this.floorScale.ButtonStepBig = 1F;
			this.floorScale.ButtonStepFloat = 0.1F;
			this.floorScale.ButtonStepSmall = 0.01F;
			this.floorScale.ButtonStepsUseModifierKeys = true;
			this.floorScale.DefaultValue = 1F;
			this.floorScale.Field1 = "xscalefloor";
			this.floorScale.Field2 = "yscalefloor";
			this.floorScale.LinkValues = false;
			this.floorScale.Location = new System.Drawing.Point(110, 53);
			this.floorScale.Name = "floorScale";
			this.floorScale.Size = new System.Drawing.Size(186, 26);
			this.floorScale.TabIndex = 17;
			this.floorScale.OnValuesChanged += new System.EventHandler(this.floorScale_OnValuesChanged);
			// 
			// floorOffsets
			// 
			this.floorOffsets.AllowDecimal = true;
			this.floorOffsets.AllowValueLinking = false;
			this.floorOffsets.ButtonStep = 1;
			this.floorOffsets.ButtonStepBig = 32F;
			this.floorOffsets.ButtonStepFloat = 16F;
			this.floorOffsets.ButtonStepSmall = 1F;
			this.floorOffsets.ButtonStepsUseModifierKeys = true;
			this.floorOffsets.DefaultValue = 0F;
			this.floorOffsets.Field1 = "xpanningfloor";
			this.floorOffsets.Field2 = "ypanningfloor";
			this.floorOffsets.LinkValues = false;
			this.floorOffsets.Location = new System.Drawing.Point(110, 21);
			this.floorOffsets.Name = "floorOffsets";
			this.floorOffsets.Size = new System.Drawing.Size(186, 26);
			this.floorOffsets.TabIndex = 16;
			this.floorOffsets.OnValuesChanged += new System.EventHandler(this.floorOffsets_OnValuesChanged);
			// 
			// floortex
			// 
			this.floortex.Location = new System.Drawing.Point(300, 13);
			this.floortex.MultipleTextures = false;
			this.floortex.Name = "floortex";
			this.floortex.Size = new System.Drawing.Size(190, 204);
			this.floortex.TabIndex = 15;
			this.floortex.TextureName = "";
			this.floortex.UsePreviews = false;
			this.floortex.OnValueChanged += new System.EventHandler(this.floortex_OnValueChanged);
			// 
			// groupBox1
			// 
			this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox1.Controls.Add(this.resetceilterrain);
			this.groupBox1.Controls.Add(this.label7);
			this.groupBox1.Controls.Add(this.ceilterrain);
			this.groupBox1.Controls.Add(this.resetceillight);
			this.groupBox1.Controls.Add(this.labelCeilOffsets);
			this.groupBox1.Controls.Add(this.labelCeilScale);
			this.groupBox1.Controls.Add(this.cbUseCeilLineAngles);
			this.groupBox1.Controls.Add(this.ceilAngleControl);
			this.groupBox1.Controls.Add(this.labelceilrenderstyle);
			this.groupBox1.Controls.Add(this.label4);
			this.groupBox1.Controls.Add(this.ceilAlpha);
			this.groupBox1.Controls.Add(this.label1);
			this.groupBox1.Controls.Add(this.ceilRotation);
			this.groupBox1.Controls.Add(this.ceilLightAbsolute);
			this.groupBox1.Controls.Add(this.labelLightFront);
			this.groupBox1.Controls.Add(this.ceilBrightness);
			this.groupBox1.Controls.Add(this.ceilRenderStyle);
			this.groupBox1.Controls.Add(this.ceilScale);
			this.groupBox1.Controls.Add(this.ceilOffsets);
			this.groupBox1.Controls.Add(this.ceilingtex);
			this.groupBox1.Location = new System.Drawing.Point(3, 3);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(497, 230);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = " Ceiling ";
			// 
			// resetceilterrain
			// 
			this.resetceilterrain.Image = global::CodeImp.DoomBuilder.Properties.Resources.Reset;
			this.resetceilterrain.Location = new System.Drawing.Point(246, 110);
			this.resetceilterrain.Name = "resetceilterrain";
			this.resetceilterrain.Size = new System.Drawing.Size(23, 23);
			this.resetceilterrain.TabIndex = 65;
			this.resetceilterrain.Text = " ";
			this.resetceilterrain.UseVisualStyleBackColor = true;
			this.resetceilterrain.Click += new System.EventHandler(this.resetceilterrain_Click);
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(24, 114);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(80, 14);
			this.label7.TabIndex = 64;
			this.label7.Tag = "";
			this.label7.Text = "Terrain:";
			this.label7.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// ceilterrain
			// 
			this.ceilterrain.FormattingEnabled = true;
			this.ceilterrain.Location = new System.Drawing.Point(113, 111);
			this.ceilterrain.Name = "ceilterrain";
			this.ceilterrain.Size = new System.Drawing.Size(130, 21);
			this.ceilterrain.TabIndex = 63;
			this.ceilterrain.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ceilterrain_MouseDown);
			this.ceilterrain.TextChanged += new System.EventHandler(this.ceilterrain_TextChanged);
			// 
			// resetceillight
			// 
			this.resetceillight.Image = global::CodeImp.DoomBuilder.Properties.Resources.Reset;
			this.resetceillight.Location = new System.Drawing.Point(246, 138);
			this.resetceillight.Name = "resetceillight";
			this.resetceillight.Size = new System.Drawing.Size(23, 23);
			this.resetceillight.TabIndex = 62;
			this.tooltip.SetToolTip(this.resetceillight, "Reset Ceiling Brightness");
			this.resetceillight.UseVisualStyleBackColor = true;
			this.resetceillight.Click += new System.EventHandler(this.resetceillight_Click);
			// 
			// labelCeilOffsets
			// 
			this.labelCeilOffsets.Location = new System.Drawing.Point(6, 27);
			this.labelCeilOffsets.Name = "labelCeilOffsets";
			this.labelCeilOffsets.Size = new System.Drawing.Size(98, 14);
			this.labelCeilOffsets.TabIndex = 61;
			this.labelCeilOffsets.Tag = "";
			this.labelCeilOffsets.Text = "Texture Offsets:";
			this.labelCeilOffsets.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// labelCeilScale
			// 
			this.labelCeilScale.Location = new System.Drawing.Point(9, 59);
			this.labelCeilScale.Name = "labelCeilScale";
			this.labelCeilScale.Size = new System.Drawing.Size(95, 14);
			this.labelCeilScale.TabIndex = 60;
			this.labelCeilScale.Tag = "";
			this.labelCeilScale.Text = "Texture Scale:";
			this.labelCeilScale.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// cbUseCeilLineAngles
			// 
			this.cbUseCeilLineAngles.AutoSize = true;
			this.cbUseCeilLineAngles.Location = new System.Drawing.Point(181, 172);
			this.cbUseCeilLineAngles.Name = "cbUseCeilLineAngles";
			this.cbUseCeilLineAngles.Size = new System.Drawing.Size(113, 17);
			this.cbUseCeilLineAngles.TabIndex = 56;
			this.cbUseCeilLineAngles.Tag = "";
			this.cbUseCeilLineAngles.Text = "Use linedef angles";
			this.cbUseCeilLineAngles.UseVisualStyleBackColor = true;
			this.cbUseCeilLineAngles.CheckedChanged += new System.EventHandler(this.cbUseCeilLineAngles_CheckedChanged);
			// 
			// ceilAngleControl
			// 
			this.ceilAngleControl.Angle = -1350;
			this.ceilAngleControl.AngleOffset = 90;
			this.ceilAngleControl.Location = new System.Drawing.Point(6, 156);
			this.ceilAngleControl.Name = "ceilAngleControl";
			this.ceilAngleControl.Size = new System.Drawing.Size(44, 44);
			this.ceilAngleControl.TabIndex = 55;
			this.ceilAngleControl.AngleChanged += new System.EventHandler(this.ceilAngleControl_AngleChanged);
			// 
			// labelceilrenderstyle
			// 
			this.labelceilrenderstyle.Location = new System.Drawing.Point(24, 88);
			this.labelceilrenderstyle.Name = "labelceilrenderstyle";
			this.labelceilrenderstyle.Size = new System.Drawing.Size(80, 14);
			this.labelceilrenderstyle.TabIndex = 54;
			this.labelceilrenderstyle.Tag = "";
			this.labelceilrenderstyle.Text = "Render style:";
			this.labelceilrenderstyle.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(24, 202);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(80, 14);
			this.label4.TabIndex = 52;
			this.label4.Tag = "";
			this.label4.Text = "Alpha:";
			this.label4.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// ceilAlpha
			// 
			this.ceilAlpha.AllowDecimal = true;
			this.ceilAlpha.AllowNegative = false;
			this.ceilAlpha.AllowRelative = false;
			this.ceilAlpha.ButtonStep = 1;
			this.ceilAlpha.ButtonStepBig = 0.25F;
			this.ceilAlpha.ButtonStepFloat = 0.1F;
			this.ceilAlpha.ButtonStepSmall = 0.01F;
			this.ceilAlpha.ButtonStepsUseModifierKeys = true;
			this.ceilAlpha.ButtonStepsWrapAround = false;
			this.ceilAlpha.Location = new System.Drawing.Point(113, 197);
			this.ceilAlpha.Name = "ceilAlpha";
			this.ceilAlpha.Size = new System.Drawing.Size(62, 24);
			this.ceilAlpha.StepValues = null;
			this.ceilAlpha.TabIndex = 53;
			this.ceilAlpha.Tag = "";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(24, 172);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(80, 14);
			this.label1.TabIndex = 50;
			this.label1.Tag = "";
			this.label1.Text = "Rotation:";
			this.label1.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// ceilRotation
			// 
			this.ceilRotation.AllowDecimal = true;
			this.ceilRotation.AllowNegative = true;
			this.ceilRotation.AllowRelative = true;
			this.ceilRotation.ButtonStep = 5;
			this.ceilRotation.ButtonStepBig = 15F;
			this.ceilRotation.ButtonStepFloat = 1F;
			this.ceilRotation.ButtonStepSmall = 0.1F;
			this.ceilRotation.ButtonStepsUseModifierKeys = true;
			this.ceilRotation.ButtonStepsWrapAround = true;
			this.ceilRotation.Location = new System.Drawing.Point(113, 167);
			this.ceilRotation.Name = "ceilRotation";
			this.ceilRotation.Size = new System.Drawing.Size(62, 24);
			this.ceilRotation.StepValues = null;
			this.ceilRotation.TabIndex = 51;
			this.ceilRotation.Tag = "";
			this.ceilRotation.WhenTextChanged += new System.EventHandler(this.ceilRotation_WhenTextChanged);
			// 
			// ceilLightAbsolute
			// 
			this.ceilLightAbsolute.AutoSize = true;
			this.ceilLightAbsolute.Location = new System.Drawing.Point(181, 142);
			this.ceilLightAbsolute.Name = "ceilLightAbsolute";
			this.ceilLightAbsolute.Size = new System.Drawing.Size(67, 17);
			this.ceilLightAbsolute.TabIndex = 49;
			this.ceilLightAbsolute.Tag = "";
			this.ceilLightAbsolute.Text = "Absolute";
			this.ceilLightAbsolute.UseVisualStyleBackColor = true;
			this.ceilLightAbsolute.CheckedChanged += new System.EventHandler(this.ceilLightAbsolute_CheckedChanged);
			// 
			// labelLightFront
			// 
			this.labelLightFront.Location = new System.Drawing.Point(24, 142);
			this.labelLightFront.Name = "labelLightFront";
			this.labelLightFront.Size = new System.Drawing.Size(80, 14);
			this.labelLightFront.TabIndex = 47;
			this.labelLightFront.Tag = "";
			this.labelLightFront.Text = "Brightness:";
			this.labelLightFront.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// ceilBrightness
			// 
			this.ceilBrightness.AllowDecimal = false;
			this.ceilBrightness.AllowNegative = true;
			this.ceilBrightness.AllowRelative = true;
			this.ceilBrightness.ButtonStep = 16;
			this.ceilBrightness.ButtonStepBig = 32F;
			this.ceilBrightness.ButtonStepFloat = 1F;
			this.ceilBrightness.ButtonStepSmall = 1F;
			this.ceilBrightness.ButtonStepsUseModifierKeys = true;
			this.ceilBrightness.ButtonStepsWrapAround = false;
			this.ceilBrightness.Location = new System.Drawing.Point(113, 137);
			this.ceilBrightness.Name = "ceilBrightness";
			this.ceilBrightness.Size = new System.Drawing.Size(62, 24);
			this.ceilBrightness.StepValues = null;
			this.ceilBrightness.TabIndex = 48;
			this.ceilBrightness.Tag = "lightceiling";
			this.ceilBrightness.WhenTextChanged += new System.EventHandler(this.ceilBrightness_WhenTextChanged);
			// 
			// ceilRenderStyle
			// 
			this.ceilRenderStyle.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.ceilRenderStyle.FormattingEnabled = true;
			this.ceilRenderStyle.Location = new System.Drawing.Point(113, 85);
			this.ceilRenderStyle.Name = "ceilRenderStyle";
			this.ceilRenderStyle.Size = new System.Drawing.Size(130, 21);
			this.ceilRenderStyle.TabIndex = 46;
			// 
			// ceilScale
			// 
			this.ceilScale.AllowDecimal = true;
			this.ceilScale.AllowValueLinking = true;
			this.ceilScale.ButtonStep = 1;
			this.ceilScale.ButtonStepBig = 1F;
			this.ceilScale.ButtonStepFloat = 0.1F;
			this.ceilScale.ButtonStepSmall = 0.01F;
			this.ceilScale.ButtonStepsUseModifierKeys = true;
			this.ceilScale.DefaultValue = 1F;
			this.ceilScale.Field1 = "xscaleceiling";
			this.ceilScale.Field2 = "yscaleceiling";
			this.ceilScale.LinkValues = false;
			this.ceilScale.Location = new System.Drawing.Point(110, 53);
			this.ceilScale.Name = "ceilScale";
			this.ceilScale.Size = new System.Drawing.Size(186, 26);
			this.ceilScale.TabIndex = 17;
			this.ceilScale.OnValuesChanged += new System.EventHandler(this.ceilScale_OnValuesChanged);
			// 
			// ceilOffsets
			// 
			this.ceilOffsets.AllowDecimal = true;
			this.ceilOffsets.AllowValueLinking = false;
			this.ceilOffsets.ButtonStep = 1;
			this.ceilOffsets.ButtonStepBig = 32F;
			this.ceilOffsets.ButtonStepFloat = 16F;
			this.ceilOffsets.ButtonStepSmall = 1F;
			this.ceilOffsets.ButtonStepsUseModifierKeys = true;
			this.ceilOffsets.DefaultValue = 0F;
			this.ceilOffsets.Field1 = "xpanningceiling";
			this.ceilOffsets.Field2 = "ypanningceiling";
			this.ceilOffsets.LinkValues = false;
			this.ceilOffsets.Location = new System.Drawing.Point(110, 21);
			this.ceilOffsets.Name = "ceilOffsets";
			this.ceilOffsets.Size = new System.Drawing.Size(186, 26);
			this.ceilOffsets.TabIndex = 16;
			this.ceilOffsets.OnValuesChanged += new System.EventHandler(this.ceilOffsets_OnValuesChanged);
			// 
			// ceilingtex
			// 
			this.ceilingtex.Location = new System.Drawing.Point(300, 13);
			this.ceilingtex.MultipleTextures = false;
			this.ceilingtex.Name = "ceilingtex";
			this.ceilingtex.Size = new System.Drawing.Size(190, 204);
			this.ceilingtex.TabIndex = 15;
			this.ceilingtex.TextureName = "";
			this.ceilingtex.UsePreviews = false;
			this.ceilingtex.OnValueChanged += new System.EventHandler(this.ceilingtex_OnValueChanged);
			// 
			// tabslopes
			// 
			this.tabslopes.Controls.Add(this.groupBox5);
			this.tabslopes.Controls.Add(this.groupBox4);
			this.tabslopes.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.tabslopes.Location = new System.Drawing.Point(4, 22);
			this.tabslopes.Name = "tabslopes";
			this.tabslopes.Size = new System.Drawing.Size(503, 478);
			this.tabslopes.TabIndex = 3;
			this.tabslopes.Text = "Slopes";
			this.tabslopes.UseVisualStyleBackColor = true;
			// 
			// groupBox5
			// 
			this.groupBox5.Controls.Add(this.floorslopecontrol);
			this.groupBox5.Location = new System.Drawing.Point(3, 239);
			this.groupBox5.Name = "groupBox5";
			this.groupBox5.Size = new System.Drawing.Size(497, 230);
			this.groupBox5.TabIndex = 1;
			this.groupBox5.TabStop = false;
			this.groupBox5.Text = " Floor ";
			// 
			// floorslopecontrol
			// 
			this.floorslopecontrol.Location = new System.Drawing.Point(6, 19);
			this.floorslopecontrol.Name = "floorslopecontrol";
			this.floorslopecontrol.Size = new System.Drawing.Size(431, 178);
			this.floorslopecontrol.TabIndex = 0;
			this.floorslopecontrol.UseLineAngles = false;
			this.floorslopecontrol.OnUseLineAnglesChanged += new System.EventHandler(this.floorslopecontrol_OnUseLineAnglesChanged);
			this.floorslopecontrol.OnResetClicked += new System.EventHandler(this.floorslopecontrol_OnResetClicked);
			this.floorslopecontrol.OnAnglesChanged += new System.EventHandler(this.floorslopecontrol_OnAnglesChanged);
			this.floorslopecontrol.OnPivotModeChanged += new System.EventHandler(this.floorslopecontrol_OnPivotModeChanged);
			// 
			// groupBox4
			// 
			this.groupBox4.Controls.Add(this.ceilingslopecontrol);
			this.groupBox4.Location = new System.Drawing.Point(3, 3);
			this.groupBox4.Name = "groupBox4";
			this.groupBox4.Size = new System.Drawing.Size(497, 230);
			this.groupBox4.TabIndex = 0;
			this.groupBox4.TabStop = false;
			this.groupBox4.Text = " Ceiling ";
			// 
			// ceilingslopecontrol
			// 
			this.ceilingslopecontrol.Location = new System.Drawing.Point(6, 19);
			this.ceilingslopecontrol.Name = "ceilingslopecontrol";
			this.ceilingslopecontrol.Size = new System.Drawing.Size(431, 178);
			this.ceilingslopecontrol.TabIndex = 1;
			this.ceilingslopecontrol.UseLineAngles = false;
			this.ceilingslopecontrol.OnUseLineAnglesChanged += new System.EventHandler(this.ceilingslopecontrol_OnUseLineAnglesChanged);
			this.ceilingslopecontrol.OnResetClicked += new System.EventHandler(this.ceilingslopecontrol_OnResetClicked);
			this.ceilingslopecontrol.OnAnglesChanged += new System.EventHandler(this.ceilingslopecontrol_OnAnglesChanged);
			this.ceilingslopecontrol.OnPivotModeChanged += new System.EventHandler(this.ceilingslopecontrol_OnPivotModeChanged);
			// 
			// tabcomment
			// 
			this.tabcomment.Controls.Add(this.commenteditor);
			this.tabcomment.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.tabcomment.Location = new System.Drawing.Point(4, 22);
			this.tabcomment.Name = "tabcomment";
			this.tabcomment.Size = new System.Drawing.Size(503, 478);
			this.tabcomment.TabIndex = 4;
			this.tabcomment.Text = "Comment";
			this.tabcomment.UseVisualStyleBackColor = true;
			// 
			// commenteditor
			// 
			this.commenteditor.Location = new System.Drawing.Point(3, 3);
			this.commenteditor.Name = "commenteditor";
			this.commenteditor.Size = new System.Drawing.Size(497, 472);
			this.commenteditor.TabIndex = 0;
			// 
			// tabcustom
			// 
			this.tabcustom.Controls.Add(this.fieldslist);
			this.tabcustom.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.tabcustom.Location = new System.Drawing.Point(4, 22);
			this.tabcustom.Name = "tabcustom";
			this.tabcustom.Padding = new System.Windows.Forms.Padding(3);
			this.tabcustom.Size = new System.Drawing.Size(503, 478);
			this.tabcustom.TabIndex = 1;
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
			this.fieldslist.Location = new System.Drawing.Point(11, 11);
			this.fieldslist.Margin = new System.Windows.Forms.Padding(8);
			this.fieldslist.Name = "fieldslist";
			this.fieldslist.PropertyColumnVisible = true;
			this.fieldslist.PropertyColumnWidth = 150;
			this.fieldslist.ShowFixedFields = true;
			this.fieldslist.Size = new System.Drawing.Size(481, 444);
			this.fieldslist.TabIndex = 1;
			this.fieldslist.TypeColumnVisible = true;
			this.fieldslist.TypeColumnWidth = 100;
			this.fieldslist.ValueColumnVisible = true;
			// 
			// cancel
			// 
			this.cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancel.Location = new System.Drawing.Point(407, 518);
			this.cancel.Name = "cancel";
			this.cancel.Size = new System.Drawing.Size(112, 25);
			this.cancel.TabIndex = 4;
			this.cancel.Text = "Cancel";
			this.cancel.UseVisualStyleBackColor = true;
			this.cancel.Click += new System.EventHandler(this.cancel_Click);
			// 
			// apply
			// 
			this.apply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.apply.Location = new System.Drawing.Point(289, 518);
			this.apply.Name = "apply";
			this.apply.Size = new System.Drawing.Size(112, 25);
			this.apply.TabIndex = 3;
			this.apply.Text = "OK";
			this.apply.UseVisualStyleBackColor = true;
			this.apply.Click += new System.EventHandler(this.apply_Click);
			// 
			// tooltip
			// 
			this.tooltip.AutomaticDelay = 10;
			this.tooltip.AutoPopDelay = 10000;
			this.tooltip.InitialDelay = 10;
			this.tooltip.ReshowDelay = 100;
			// 
			// SectorEditFormUDMF
			// 
			this.AcceptButton = this.apply;
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.CancelButton = this.cancel;
			this.ClientSize = new System.Drawing.Size(531, 548);
			this.Controls.Add(this.cancel);
			this.Controls.Add(this.apply);
			this.Controls.Add(this.tabs);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "SectorEditFormUDMF";
			this.Opacity = 1;
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Edit Sector";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SectorEditFormUDMF_FormClosing);
			this.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.SectorEditFormUDMF_HelpRequested);
			groupaction.ResumeLayout(false);
			groupeffect.ResumeLayout(false);
			groupeffect.PerformLayout();
			groupfloorceiling.ResumeLayout(false);
			groupfloorceiling.PerformLayout();
			this.tabs.ResumeLayout(false);
			this.tabproperties.ResumeLayout(false);
			this.groupdamage.ResumeLayout(false);
			this.groupBox3.ResumeLayout(false);
			this.tabSurfaces.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.tabslopes.ResumeLayout(false);
			this.groupBox5.ResumeLayout(false);
			this.groupBox4.ResumeLayout(false);
			this.tabcomment.ResumeLayout(false);
			this.tabcustom.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TabControl tabs;
		private System.Windows.Forms.TabPage tabproperties;
		private CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox brightness;
		private System.Windows.Forms.Button browseeffect;
		private CodeImp.DoomBuilder.Controls.ActionSelectorControl effect;
		private CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox ceilingheight;
		private System.Windows.Forms.Label sectorheightlabel;
		private System.Windows.Forms.Label sectorheight;
		private CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox floorheight;
		private System.Windows.Forms.TabPage tabcustom;
		private CodeImp.DoomBuilder.Controls.FieldsEditorControl fieldslist;
		private System.Windows.Forms.Button cancel;
		private System.Windows.Forms.Button apply;
		private System.Windows.Forms.TabPage tabSurfaces;
		private System.Windows.Forms.GroupBox groupBox1;
		private CodeImp.DoomBuilder.GZBuilder.Controls.PairedFieldsControl ceilOffsets;
		private CodeImp.DoomBuilder.Controls.FlatSelectorControl ceilingtex;
		private CodeImp.DoomBuilder.GZBuilder.Controls.PairedFieldsControl ceilScale;
		private System.Windows.Forms.ComboBox ceilRenderStyle;
		private System.Windows.Forms.Label label4;
		private CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox ceilAlpha;
		private System.Windows.Forms.Label label1;
		private CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox ceilRotation;
		private System.Windows.Forms.CheckBox ceilLightAbsolute;
		private System.Windows.Forms.Label labelLightFront;
		private CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox ceilBrightness;
		private System.Windows.Forms.Label labelceilrenderstyle;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.Label labelfloorrenderstyle;
		private System.Windows.Forms.Label label10;
		private CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox floorAlpha;
		private System.Windows.Forms.Label label11;
		private CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox floorRotation;
		private System.Windows.Forms.CheckBox floorLightAbsolute;
		private System.Windows.Forms.Label label12;
		private CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox floorBrightness;
		private System.Windows.Forms.ComboBox floorRenderStyle;
		private CodeImp.DoomBuilder.GZBuilder.Controls.PairedFieldsControl floorScale;
		private CodeImp.DoomBuilder.GZBuilder.Controls.PairedFieldsControl floorOffsets;
		private CodeImp.DoomBuilder.Controls.FlatSelectorControl floortex;
		private CodeImp.DoomBuilder.GZBuilder.Controls.AngleControl floorAngleControl;
		private CodeImp.DoomBuilder.GZBuilder.Controls.AngleControl ceilAngleControl;
		private System.Windows.Forms.GroupBox groupBox3;
		private CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox gravity;
		private CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox desaturation;
		private CodeImp.DoomBuilder.GZBuilder.Controls.ColorFieldsControl fadeColor;
		private CodeImp.DoomBuilder.GZBuilder.Controls.ColorFieldsControl lightColor;
		private CodeImp.DoomBuilder.Controls.CheckboxArrayControl flags;
		private System.Windows.Forms.CheckBox cbUseFloorLineAngles;
		private System.Windows.Forms.CheckBox cbUseCeilLineAngles;
		private System.Windows.Forms.TabPage tabslopes;
		private System.Windows.Forms.GroupBox groupBox5;
		private System.Windows.Forms.GroupBox groupBox4;
		private CodeImp.DoomBuilder.Controls.SectorSlopeControl floorslopecontrol;
		private CodeImp.DoomBuilder.Controls.SectorSlopeControl ceilingslopecontrol;
		private CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox heightoffset;
		private System.Windows.Forms.ToolTip tooltip;
		private System.Windows.Forms.Label labelFloorOffsets;
		private System.Windows.Forms.Label labelFloorScale;
		private System.Windows.Forms.Label labelCeilOffsets;
		private System.Windows.Forms.Label labelCeilScale;
		private System.Windows.Forms.Button resetsoundsequence;
		private System.Windows.Forms.ComboBox soundsequence;
		private System.Windows.Forms.TabPage tabcomment;
		private CodeImp.DoomBuilder.Controls.CommentEditor commenteditor;
		private CodeImp.DoomBuilder.GZBuilder.Controls.TagsSelector tagsselector;
		private System.Windows.Forms.Button resetfloorlight;
		private System.Windows.Forms.Button resetceillight;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.ComboBox floorterrain;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.ComboBox ceilterrain;
		private System.Windows.Forms.GroupBox groupdamage;
		private CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox leakiness;
		private CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox damageinterval;
		private System.Windows.Forms.Button resetdamagetype;
		private CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox damageamount;
		private System.Windows.Forms.ComboBox damagetype;
		private System.Windows.Forms.Button resetfloorterrain;
		private System.Windows.Forms.Button resetceilterrain;
	}
}