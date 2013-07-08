namespace CodeImp.DoomBuilder.Windows
{
	partial class LinedefEditForm
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
			System.Windows.Forms.Label label2;
			System.Windows.Forms.Label label3;
			System.Windows.Forms.Label label4;
			System.Windows.Forms.Label label5;
			System.Windows.Forms.Label label8;
			System.Windows.Forms.Label label9;
			System.Windows.Forms.Label label10;
			System.Windows.Forms.Label label11;
			System.Windows.Forms.Label label12;
			System.Windows.Forms.Label activationlabel;
			System.Windows.Forms.Label label1;
			System.Windows.Forms.Label label7;
			System.Windows.Forms.Label label14;
			System.Windows.Forms.Label label6;
			this.labelLightFront = new System.Windows.Forms.Label();
			this.cancel = new System.Windows.Forms.Button();
			this.apply = new System.Windows.Forms.Button();
			this.actiongroup = new System.Windows.Forms.GroupBox();
			this.argspanel = new System.Windows.Forms.Panel();
			this.arg0str = new System.Windows.Forms.ComboBox();
			this.arg2 = new CodeImp.DoomBuilder.Controls.ArgumentBox();
			this.arg1 = new CodeImp.DoomBuilder.Controls.ArgumentBox();
			this.arg0 = new CodeImp.DoomBuilder.Controls.ArgumentBox();
			this.arg3 = new CodeImp.DoomBuilder.Controls.ArgumentBox();
			this.arg4 = new CodeImp.DoomBuilder.Controls.ArgumentBox();
			this.arg1label = new System.Windows.Forms.Label();
			this.arg3label = new System.Windows.Forms.Label();
			this.arg2label = new System.Windows.Forms.Label();
			this.arg4label = new System.Windows.Forms.Label();
			this.cbArgStr = new System.Windows.Forms.CheckBox();
			this.arg0label = new System.Windows.Forms.Label();
			this.hexenpanel = new System.Windows.Forms.Panel();
			this.activation = new System.Windows.Forms.ComboBox();
			this.action = new CodeImp.DoomBuilder.Controls.ActionSelectorControl();
			this.browseaction = new System.Windows.Forms.Button();
			this.udmfpanel = new System.Windows.Forms.Panel();
			this.udmfactivates = new CodeImp.DoomBuilder.Controls.CheckboxArrayControl();
			this.flagsgroup = new System.Windows.Forms.GroupBox();
			this.flags = new CodeImp.DoomBuilder.Controls.CheckboxArrayControl();
			this.checkBox1 = new System.Windows.Forms.CheckBox();
			this.tabs = new System.Windows.Forms.TabControl();
			this.tabproperties = new System.Windows.Forms.TabPage();
			this.settingsGroup = new System.Windows.Forms.GroupBox();
			this.alpha = new CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox();
			this.lockNumber = new CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox();
			this.cbRenderStyle = new System.Windows.Forms.ComboBox();
			this.idgroup = new System.Windows.Forms.GroupBox();
			this.tagSelector = new CodeImp.DoomBuilder.GZBuilder.Controls.TagSelector();
			this.tabsidedefs = new System.Windows.Forms.TabPage();
			this.splitter = new System.Windows.Forms.SplitContainer();
			this.frontside = new System.Windows.Forms.CheckBox();
			this.frontgroup = new System.Windows.Forms.GroupBox();
			this.frontTextureOffset = new CodeImp.DoomBuilder.GZBuilder.Controls.PairedIntControl();
			this.cbLightAbsoluteFront = new System.Windows.Forms.CheckBox();
			this.lightFront = new CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox();
			this.frontsector = new CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox();
			this.customfrontbutton = new System.Windows.Forms.Button();
			this.frontlow = new CodeImp.DoomBuilder.Controls.TextureSelectorControl();
			this.frontmid = new CodeImp.DoomBuilder.Controls.TextureSelectorControl();
			this.fronthigh = new CodeImp.DoomBuilder.Controls.TextureSelectorControl();
			this.udmfPropertiesFront = new System.Windows.Forms.TabControl();
			this.tabPage1 = new System.Windows.Forms.TabPage();
			this.pfcFrontScaleBottom = new CodeImp.DoomBuilder.GZBuilder.Controls.PairedFieldsControl();
			this.pfcFrontScaleMid = new CodeImp.DoomBuilder.GZBuilder.Controls.PairedFieldsControl();
			this.pfcFrontScaleTop = new CodeImp.DoomBuilder.GZBuilder.Controls.PairedFieldsControl();
			this.pfcFrontOffsetBottom = new CodeImp.DoomBuilder.GZBuilder.Controls.PairedFieldsControl();
			this.pfcFrontOffsetMid = new CodeImp.DoomBuilder.GZBuilder.Controls.PairedFieldsControl();
			this.pfcFrontOffsetTop = new CodeImp.DoomBuilder.GZBuilder.Controls.PairedFieldsControl();
			this.tabPage2 = new System.Windows.Forms.TabPage();
			this.cblightfogFront = new System.Windows.Forms.CheckBox();
			this.cbsmoothlightingFront = new System.Windows.Forms.CheckBox();
			this.cbnodecalsFront = new System.Windows.Forms.CheckBox();
			this.cbnofakecontrastFront = new System.Windows.Forms.CheckBox();
			this.cbWrapMidtexFront = new System.Windows.Forms.CheckBox();
			this.cbClipMidtexFront = new System.Windows.Forms.CheckBox();
			this.backside = new System.Windows.Forms.CheckBox();
			this.backgroup = new System.Windows.Forms.GroupBox();
			this.backTextureOffset = new CodeImp.DoomBuilder.GZBuilder.Controls.PairedIntControl();
			this.cbLightAbsoluteBack = new System.Windows.Forms.CheckBox();
			this.labelLightBack = new System.Windows.Forms.Label();
			this.lightBack = new CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox();
			this.backsector = new CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox();
			this.custombackbutton = new System.Windows.Forms.Button();
			this.backlow = new CodeImp.DoomBuilder.Controls.TextureSelectorControl();
			this.backmid = new CodeImp.DoomBuilder.Controls.TextureSelectorControl();
			this.backhigh = new CodeImp.DoomBuilder.Controls.TextureSelectorControl();
			this.udmfPropertiesBack = new System.Windows.Forms.TabControl();
			this.tabPage5 = new System.Windows.Forms.TabPage();
			this.pfcBackScaleBottom = new CodeImp.DoomBuilder.GZBuilder.Controls.PairedFieldsControl();
			this.pfcBackScaleMid = new CodeImp.DoomBuilder.GZBuilder.Controls.PairedFieldsControl();
			this.pfcBackScaleTop = new CodeImp.DoomBuilder.GZBuilder.Controls.PairedFieldsControl();
			this.pfcBackOffsetBottom = new CodeImp.DoomBuilder.GZBuilder.Controls.PairedFieldsControl();
			this.pfcBackOffsetMid = new CodeImp.DoomBuilder.GZBuilder.Controls.PairedFieldsControl();
			this.pfcBackOffsetTop = new CodeImp.DoomBuilder.GZBuilder.Controls.PairedFieldsControl();
			this.tabPage6 = new System.Windows.Forms.TabPage();
			this.cblightfogBack = new System.Windows.Forms.CheckBox();
			this.cbsmoothlightingBack = new System.Windows.Forms.CheckBox();
			this.cbnodecalsBack = new System.Windows.Forms.CheckBox();
			this.cbnofakecontrastBack = new System.Windows.Forms.CheckBox();
			this.cbWrapMidtexBack = new System.Windows.Forms.CheckBox();
			this.cbClipMidtexBack = new System.Windows.Forms.CheckBox();
			this.tabcustom = new System.Windows.Forms.TabPage();
			this.fieldslist = new CodeImp.DoomBuilder.Controls.FieldsEditorControl();
			this.heightpanel1 = new System.Windows.Forms.Panel();
			this.heightpanel2 = new System.Windows.Forms.Panel();
			this.tabPage3 = new System.Windows.Forms.TabPage();
			this.pairedFieldsControl1 = new CodeImp.DoomBuilder.GZBuilder.Controls.PairedFieldsControl();
			this.pairedFieldsControl2 = new CodeImp.DoomBuilder.GZBuilder.Controls.PairedFieldsControl();
			this.pairedFieldsControl3 = new CodeImp.DoomBuilder.GZBuilder.Controls.PairedFieldsControl();
			this.pairedFieldsControl4 = new CodeImp.DoomBuilder.GZBuilder.Controls.PairedFieldsControl();
			this.pairedFieldsControl5 = new CodeImp.DoomBuilder.GZBuilder.Controls.PairedFieldsControl();
			this.pairedFieldsControl6 = new CodeImp.DoomBuilder.GZBuilder.Controls.PairedFieldsControl();
			this.tabPage4 = new System.Windows.Forms.TabPage();
			this.checkBox2 = new System.Windows.Forms.CheckBox();
			this.buttonsNumericTextbox1 = new CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox();
			this.checkBox3 = new System.Windows.Forms.CheckBox();
			this.checkBox4 = new System.Windows.Forms.CheckBox();
			this.checkBox5 = new System.Windows.Forms.CheckBox();
			this.checkBox6 = new System.Windows.Forms.CheckBox();
			this.checkBox7 = new System.Windows.Forms.CheckBox();
			this.checkBox8 = new System.Windows.Forms.CheckBox();
			label2 = new System.Windows.Forms.Label();
			label3 = new System.Windows.Forms.Label();
			label4 = new System.Windows.Forms.Label();
			label5 = new System.Windows.Forms.Label();
			label8 = new System.Windows.Forms.Label();
			label9 = new System.Windows.Forms.Label();
			label10 = new System.Windows.Forms.Label();
			label11 = new System.Windows.Forms.Label();
			label12 = new System.Windows.Forms.Label();
			activationlabel = new System.Windows.Forms.Label();
			label1 = new System.Windows.Forms.Label();
			label7 = new System.Windows.Forms.Label();
			label14 = new System.Windows.Forms.Label();
			label6 = new System.Windows.Forms.Label();
			this.actiongroup.SuspendLayout();
			this.argspanel.SuspendLayout();
			this.hexenpanel.SuspendLayout();
			this.udmfpanel.SuspendLayout();
			this.flagsgroup.SuspendLayout();
			this.tabs.SuspendLayout();
			this.tabproperties.SuspendLayout();
			this.settingsGroup.SuspendLayout();
			this.idgroup.SuspendLayout();
			this.tabsidedefs.SuspendLayout();
			this.splitter.Panel1.SuspendLayout();
			this.splitter.Panel2.SuspendLayout();
			this.splitter.SuspendLayout();
			this.frontgroup.SuspendLayout();
			this.udmfPropertiesFront.SuspendLayout();
			this.tabPage1.SuspendLayout();
			this.tabPage2.SuspendLayout();
			this.backgroup.SuspendLayout();
			this.udmfPropertiesBack.SuspendLayout();
			this.tabPage5.SuspendLayout();
			this.tabPage6.SuspendLayout();
			this.tabcustom.SuspendLayout();
			this.tabPage3.SuspendLayout();
			this.tabPage4.SuspendLayout();
			this.SuspendLayout();
			// 
			// label2
			// 
			label2.AutoSize = true;
			label2.Location = new System.Drawing.Point(15, 30);
			label2.Name = "label2";
			label2.Size = new System.Drawing.Size(41, 14);
			label2.TabIndex = 9;
			label2.Text = "Action:";
			// 
			// label3
			// 
			label3.Location = new System.Drawing.Point(252, 18);
			label3.Name = "label3";
			label3.Size = new System.Drawing.Size(83, 16);
			label3.TabIndex = 3;
			label3.Text = "Upper";
			label3.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// label4
			// 
			label4.Location = new System.Drawing.Point(343, 18);
			label4.Name = "label4";
			label4.Size = new System.Drawing.Size(83, 16);
			label4.TabIndex = 4;
			label4.Text = "Middle";
			label4.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// label5
			// 
			label5.Location = new System.Drawing.Point(434, 18);
			label5.Name = "label5";
			label5.Size = new System.Drawing.Size(83, 16);
			label5.TabIndex = 5;
			label5.Text = "Lower";
			label5.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// label8
			// 
			label8.Location = new System.Drawing.Point(437, 18);
			label8.Name = "label8";
			label8.Size = new System.Drawing.Size(83, 16);
			label8.TabIndex = 5;
			label8.Text = "Lower";
			label8.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// label9
			// 
			label9.Location = new System.Drawing.Point(346, 18);
			label9.Name = "label9";
			label9.Size = new System.Drawing.Size(83, 16);
			label9.TabIndex = 4;
			label9.Text = "Middle";
			label9.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// label10
			// 
			label10.Location = new System.Drawing.Point(255, 18);
			label10.Name = "label10";
			label10.Size = new System.Drawing.Size(83, 16);
			label10.TabIndex = 3;
			label10.Text = "Upper";
			label10.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// label11
			// 
			label11.Location = new System.Drawing.Point(8, 42);
			label11.Name = "label11";
			label11.Size = new System.Drawing.Size(80, 14);
			label11.TabIndex = 13;
			label11.Text = "Sector Index:";
			label11.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// label12
			// 
			label12.Location = new System.Drawing.Point(8, 42);
			label12.Name = "label12";
			label12.Size = new System.Drawing.Size(80, 14);
			label12.TabIndex = 16;
			label12.Text = "Sector Index:";
			label12.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// activationlabel
			// 
			activationlabel.AutoSize = true;
			activationlabel.Location = new System.Drawing.Point(6, 17);
			activationlabel.Name = "activationlabel";
			activationlabel.Size = new System.Drawing.Size(44, 14);
			activationlabel.TabIndex = 10;
			activationlabel.Text = "Trigger:";
			// 
			// label1
			// 
			label1.Location = new System.Drawing.Point(15, 18);
			label1.Name = "label1";
			label1.Size = new System.Drawing.Size(40, 14);
			label1.TabIndex = 25;
			label1.Tag = "light";
			label1.Text = "Light:";
			label1.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// label7
			// 
			label7.AutoSize = true;
			label7.Location = new System.Drawing.Point(15, 30);
			label7.Name = "label7";
			label7.Size = new System.Drawing.Size(71, 14);
			label7.TabIndex = 11;
			label7.Text = "Render style:";
			// 
			// label14
			// 
			label14.AutoSize = true;
			label14.Location = new System.Drawing.Point(330, 30);
			label14.Name = "label14";
			label14.Size = new System.Drawing.Size(73, 14);
			label14.TabIndex = 15;
			label14.Text = "Lock Number:";
			// 
			// label6
			// 
			label6.AutoSize = true;
			label6.Location = new System.Drawing.Point(199, 30);
			label6.Name = "label6";
			label6.Size = new System.Drawing.Size(38, 14);
			label6.TabIndex = 17;
			label6.Text = "Alpha:";
			// 
			// labelLightFront
			// 
			this.labelLightFront.Location = new System.Drawing.Point(8, 101);
			this.labelLightFront.Name = "labelLightFront";
			this.labelLightFront.Size = new System.Drawing.Size(80, 14);
			this.labelLightFront.TabIndex = 25;
			this.labelLightFront.Tag = "";
			this.labelLightFront.Text = "Brightness:";
			this.labelLightFront.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// cancel
			// 
			this.cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancel.Location = new System.Drawing.Point(455, 662);
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
			this.apply.Location = new System.Drawing.Point(336, 662);
			this.apply.Name = "apply";
			this.apply.Size = new System.Drawing.Size(112, 25);
			this.apply.TabIndex = 1;
			this.apply.Text = "OK";
			this.apply.UseVisualStyleBackColor = true;
			this.apply.Click += new System.EventHandler(this.apply_Click);
			// 
			// actiongroup
			// 
			this.actiongroup.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.actiongroup.Controls.Add(this.argspanel);
			this.actiongroup.Controls.Add(this.hexenpanel);
			this.actiongroup.Controls.Add(label2);
			this.actiongroup.Controls.Add(this.action);
			this.actiongroup.Controls.Add(this.browseaction);
			this.actiongroup.Controls.Add(this.udmfpanel);
			this.actiongroup.Location = new System.Drawing.Point(8, 255);
			this.actiongroup.Name = "actiongroup";
			this.actiongroup.Size = new System.Drawing.Size(533, 291);
			this.actiongroup.TabIndex = 1;
			this.actiongroup.TabStop = false;
			this.actiongroup.Text = " Action ";
			// 
			// argspanel
			// 
			this.argspanel.Controls.Add(this.arg0str);
			this.argspanel.Controls.Add(this.arg2);
			this.argspanel.Controls.Add(this.arg1);
			this.argspanel.Controls.Add(this.arg0);
			this.argspanel.Controls.Add(this.arg3);
			this.argspanel.Controls.Add(this.arg4);
			this.argspanel.Controls.Add(this.arg1label);
			this.argspanel.Controls.Add(this.arg3label);
			this.argspanel.Controls.Add(this.arg2label);
			this.argspanel.Controls.Add(this.arg4label);
			this.argspanel.Controls.Add(this.cbArgStr);
			this.argspanel.Controls.Add(this.arg0label);
			this.argspanel.Location = new System.Drawing.Point(6, 54);
			this.argspanel.Name = "argspanel";
			this.argspanel.Size = new System.Drawing.Size(521, 83);
			this.argspanel.TabIndex = 2;
			this.argspanel.Visible = false;
			// 
			// arg0str
			// 
			this.arg0str.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
			this.arg0str.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
			this.arg0str.FormattingEnabled = true;
			this.arg0str.Location = new System.Drawing.Point(272, 55);
			this.arg0str.Name = "arg0str";
			this.arg0str.Size = new System.Drawing.Size(125, 22);
			this.arg0str.TabIndex = 38;
			this.arg0str.Leave += new System.EventHandler(this.arg0str_Leave);
			// 
			// arg2
			// 
			this.arg2.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.arg2.Location = new System.Drawing.Point(157, 55);
			this.arg2.Name = "arg2";
			this.arg2.Size = new System.Drawing.Size(120, 24);
			this.arg2.TabIndex = 2;
			// 
			// arg1
			// 
			this.arg1.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.arg1.Location = new System.Drawing.Point(157, 29);
			this.arg1.Name = "arg1";
			this.arg1.Size = new System.Drawing.Size(120, 24);
			this.arg1.TabIndex = 1;
			// 
			// arg0
			// 
			this.arg0.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.arg0.Location = new System.Drawing.Point(157, 3);
			this.arg0.Name = "arg0";
			this.arg0.Size = new System.Drawing.Size(120, 24);
			this.arg0.TabIndex = 0;
			// 
			// arg3
			// 
			this.arg3.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.arg3.Location = new System.Drawing.Point(398, 3);
			this.arg3.Name = "arg3";
			this.arg3.Size = new System.Drawing.Size(120, 24);
			this.arg3.TabIndex = 3;
			// 
			// arg4
			// 
			this.arg4.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.arg4.Location = new System.Drawing.Point(398, 29);
			this.arg4.Name = "arg4";
			this.arg4.Size = new System.Drawing.Size(120, 24);
			this.arg4.TabIndex = 4;
			// 
			// arg1label
			// 
			this.arg1label.Location = new System.Drawing.Point(-28, 34);
			this.arg1label.Name = "arg1label";
			this.arg1label.Size = new System.Drawing.Size(179, 14);
			this.arg1label.TabIndex = 33;
			this.arg1label.Text = "Argument 2:";
			this.arg1label.TextAlign = System.Drawing.ContentAlignment.TopRight;
			this.arg1label.UseMnemonic = false;
			// 
			// arg3label
			// 
			this.arg3label.Location = new System.Drawing.Point(213, 8);
			this.arg3label.Name = "arg3label";
			this.arg3label.Size = new System.Drawing.Size(179, 14);
			this.arg3label.TabIndex = 36;
			this.arg3label.Text = "Argument 4:";
			this.arg3label.TextAlign = System.Drawing.ContentAlignment.TopRight;
			this.arg3label.UseMnemonic = false;
			// 
			// arg2label
			// 
			this.arg2label.Location = new System.Drawing.Point(-28, 60);
			this.arg2label.Name = "arg2label";
			this.arg2label.Size = new System.Drawing.Size(179, 14);
			this.arg2label.TabIndex = 35;
			this.arg2label.Text = "Argument 3:";
			this.arg2label.TextAlign = System.Drawing.ContentAlignment.TopRight;
			this.arg2label.UseMnemonic = false;
			// 
			// arg4label
			// 
			this.arg4label.Location = new System.Drawing.Point(213, 34);
			this.arg4label.Name = "arg4label";
			this.arg4label.Size = new System.Drawing.Size(179, 14);
			this.arg4label.TabIndex = 34;
			this.arg4label.Text = "Argument 5:";
			this.arg4label.TextAlign = System.Drawing.ContentAlignment.TopRight;
			this.arg4label.UseMnemonic = false;
			// 
			// cbArgStr
			// 
			this.cbArgStr.Location = new System.Drawing.Point(8, -4);
			this.cbArgStr.Name = "cbArgStr";
			this.cbArgStr.Size = new System.Drawing.Size(63, 40);
			this.cbArgStr.TabIndex = 37;
			this.cbArgStr.Text = "Named script";
			this.cbArgStr.UseVisualStyleBackColor = true;
			this.cbArgStr.CheckedChanged += new System.EventHandler(this.cbArgStr_CheckedChanged);
			// 
			// arg0label
			// 
			this.arg0label.Location = new System.Drawing.Point(-28, 8);
			this.arg0label.Name = "arg0label";
			this.arg0label.Size = new System.Drawing.Size(179, 14);
			this.arg0label.TabIndex = 32;
			this.arg0label.Text = "Argument 1:";
			this.arg0label.TextAlign = System.Drawing.ContentAlignment.TopRight;
			this.arg0label.UseMnemonic = false;
			// 
			// hexenpanel
			// 
			this.hexenpanel.Controls.Add(this.activation);
			this.hexenpanel.Controls.Add(activationlabel);
			this.hexenpanel.Location = new System.Drawing.Point(6, 139);
			this.hexenpanel.Name = "hexenpanel";
			this.hexenpanel.Size = new System.Drawing.Size(521, 49);
			this.hexenpanel.TabIndex = 3;
			this.hexenpanel.Visible = false;
			// 
			// activation
			// 
			this.activation.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.activation.FormattingEnabled = true;
			this.activation.Location = new System.Drawing.Point(56, 13);
			this.activation.Name = "activation";
			this.activation.Size = new System.Drawing.Size(437, 22);
			this.activation.TabIndex = 0;
			// 
			// action
			// 
			this.action.BackColor = System.Drawing.Color.Transparent;
			this.action.Cursor = System.Windows.Forms.Cursors.Default;
			this.action.Empty = false;
			this.action.GeneralizedCategories = null;
			this.action.Location = new System.Drawing.Point(62, 27);
			this.action.Name = "action";
			this.action.Size = new System.Drawing.Size(428, 21);
			this.action.TabIndex = 0;
			this.action.Value = 402;
			this.action.ValueChanges += new System.EventHandler(this.action_ValueChanges);
			// 
			// browseaction
			// 
			this.browseaction.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.browseaction.Image = global::CodeImp.DoomBuilder.Properties.Resources.List;
			this.browseaction.Location = new System.Drawing.Point(496, 25);
			this.browseaction.Name = "browseaction";
			this.browseaction.Padding = new System.Windows.Forms.Padding(0, 0, 1, 3);
			this.browseaction.Size = new System.Drawing.Size(28, 25);
			this.browseaction.TabIndex = 1;
			this.browseaction.Text = " ";
			this.browseaction.UseVisualStyleBackColor = true;
			this.browseaction.Click += new System.EventHandler(this.browseaction_Click);
			// 
			// udmfpanel
			// 
			this.udmfpanel.Controls.Add(this.udmfactivates);
			this.udmfpanel.Location = new System.Drawing.Point(6, 143);
			this.udmfpanel.Name = "udmfpanel";
			this.udmfpanel.Size = new System.Drawing.Size(505, 142);
			this.udmfpanel.TabIndex = 4;
			this.udmfpanel.Visible = false;
			// 
			// udmfactivates
			// 
			this.udmfactivates.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.udmfactivates.AutoScroll = true;
			this.udmfactivates.Columns = 2;
			this.udmfactivates.Location = new System.Drawing.Point(56, 5);
			this.udmfactivates.Name = "udmfactivates";
			this.udmfactivates.Size = new System.Drawing.Size(437, 133);
			this.udmfactivates.TabIndex = 0;
			// 
			// flagsgroup
			// 
			this.flagsgroup.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.flagsgroup.Controls.Add(this.flags);
			this.flagsgroup.Location = new System.Drawing.Point(8, 3);
			this.flagsgroup.Name = "flagsgroup";
			this.flagsgroup.Size = new System.Drawing.Size(533, 174);
			this.flagsgroup.TabIndex = 0;
			this.flagsgroup.TabStop = false;
			this.flagsgroup.Text = " Flags";
			// 
			// flags
			// 
			this.flags.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.flags.AutoScroll = true;
			this.flags.Columns = 3;
			this.flags.Location = new System.Drawing.Point(18, 17);
			this.flags.Name = "flags";
			this.flags.Size = new System.Drawing.Size(509, 152);
			this.flags.TabIndex = 0;
			// 
			// checkBox1
			// 
			this.checkBox1.Location = new System.Drawing.Point(0, 0);
			this.checkBox1.Name = "checkBox1";
			this.checkBox1.Size = new System.Drawing.Size(104, 24);
			this.checkBox1.TabIndex = 0;
			this.checkBox1.Text = "checkBox1";
			this.checkBox1.UseVisualStyleBackColor = true;
			// 
			// tabs
			// 
			this.tabs.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.tabs.Controls.Add(this.tabproperties);
			this.tabs.Controls.Add(this.tabsidedefs);
			this.tabs.Controls.Add(this.tabcustom);
			this.tabs.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.tabs.Location = new System.Drawing.Point(10, 10);
			this.tabs.Margin = new System.Windows.Forms.Padding(1);
			this.tabs.Name = "tabs";
			this.tabs.SelectedIndex = 0;
			this.tabs.Size = new System.Drawing.Size(557, 648);
			this.tabs.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
			this.tabs.TabIndex = 0;
			// 
			// tabproperties
			// 
			this.tabproperties.Controls.Add(this.settingsGroup);
			this.tabproperties.Controls.Add(this.idgroup);
			this.tabproperties.Controls.Add(this.flagsgroup);
			this.tabproperties.Controls.Add(this.actiongroup);
			this.tabproperties.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.tabproperties.Location = new System.Drawing.Point(4, 23);
			this.tabproperties.Name = "tabproperties";
			this.tabproperties.Padding = new System.Windows.Forms.Padding(5);
			this.tabproperties.Size = new System.Drawing.Size(549, 621);
			this.tabproperties.TabIndex = 0;
			this.tabproperties.Text = "Properties";
			this.tabproperties.UseVisualStyleBackColor = true;
			// 
			// settingsGroup
			// 
			this.settingsGroup.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.settingsGroup.Controls.Add(this.alpha);
			this.settingsGroup.Controls.Add(label6);
			this.settingsGroup.Controls.Add(this.lockNumber);
			this.settingsGroup.Controls.Add(label14);
			this.settingsGroup.Controls.Add(this.cbRenderStyle);
			this.settingsGroup.Controls.Add(label7);
			this.settingsGroup.Location = new System.Drawing.Point(8, 183);
			this.settingsGroup.Name = "settingsGroup";
			this.settingsGroup.Size = new System.Drawing.Size(533, 66);
			this.settingsGroup.TabIndex = 3;
			this.settingsGroup.TabStop = false;
			this.settingsGroup.Text = " Settings";
			// 
			// alpha
			// 
			this.alpha.AllowDecimal = true;
			this.alpha.AllowNegative = false;
			this.alpha.AllowRelative = false;
			this.alpha.ButtonStepFloat = 0.1F;
			this.alpha.Location = new System.Drawing.Point(243, 25);
			this.alpha.Name = "alpha";
			this.alpha.Size = new System.Drawing.Size(65, 24);
			this.alpha.StepValues = null;
			this.alpha.TabIndex = 18;
			// 
			// lockNumber
			// 
			this.lockNumber.AllowDecimal = false;
			this.lockNumber.AllowNegative = false;
			this.lockNumber.AllowRelative = false;
			this.lockNumber.ButtonStep = 1;
			this.lockNumber.Location = new System.Drawing.Point(405, 25);
			this.lockNumber.Name = "lockNumber";
			this.lockNumber.Size = new System.Drawing.Size(65, 24);
			this.lockNumber.StepValues = null;
			this.lockNumber.TabIndex = 16;
			// 
			// cbRenderStyle
			// 
			this.cbRenderStyle.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbRenderStyle.FormattingEnabled = true;
			this.cbRenderStyle.Items.AddRange(new object[] {
            "Translucent",
            "Additive"});
			this.cbRenderStyle.Location = new System.Drawing.Point(92, 26);
			this.cbRenderStyle.Name = "cbRenderStyle";
			this.cbRenderStyle.Size = new System.Drawing.Size(86, 22);
			this.cbRenderStyle.TabIndex = 12;
			// 
			// idgroup
			// 
			this.idgroup.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.idgroup.Controls.Add(this.tagSelector);
			this.idgroup.Location = new System.Drawing.Point(8, 552);
			this.idgroup.Name = "idgroup";
			this.idgroup.Size = new System.Drawing.Size(533, 66);
			this.idgroup.TabIndex = 2;
			this.idgroup.TabStop = false;
			this.idgroup.Text = " Identification ";
			// 
			// tagSelector
			// 
			this.tagSelector.Location = new System.Drawing.Point(6, 19);
			this.tagSelector.Name = "tagSelector";
			this.tagSelector.Size = new System.Drawing.Size(369, 35);
			this.tagSelector.TabIndex = 0;
			// 
			// tabsidedefs
			// 
			this.tabsidedefs.Controls.Add(this.splitter);
			this.tabsidedefs.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.tabsidedefs.Location = new System.Drawing.Point(4, 23);
			this.tabsidedefs.Name = "tabsidedefs";
			this.tabsidedefs.Padding = new System.Windows.Forms.Padding(5);
			this.tabsidedefs.Size = new System.Drawing.Size(549, 621);
			this.tabsidedefs.TabIndex = 1;
			this.tabsidedefs.Text = "Sidedefs";
			this.tabsidedefs.UseVisualStyleBackColor = true;
			// 
			// splitter
			// 
			this.splitter.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitter.IsSplitterFixed = true;
			this.splitter.Location = new System.Drawing.Point(5, 5);
			this.splitter.Name = "splitter";
			this.splitter.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitter.Panel1
			// 
			this.splitter.Panel1.Controls.Add(this.frontside);
			this.splitter.Panel1.Controls.Add(this.frontgroup);
			// 
			// splitter.Panel2
			// 
			this.splitter.Panel2.Controls.Add(this.backside);
			this.splitter.Panel2.Controls.Add(this.backgroup);
			this.splitter.Size = new System.Drawing.Size(539, 611);
			this.splitter.SplitterDistance = 302;
			this.splitter.TabIndex = 3;
			// 
			// frontside
			// 
			this.frontside.AutoSize = true;
			this.frontside.Location = new System.Drawing.Point(15, 1);
			this.frontside.Name = "frontside";
			this.frontside.Size = new System.Drawing.Size(75, 18);
			this.frontside.TabIndex = 0;
			this.frontside.Text = "Front Side";
			this.frontside.UseVisualStyleBackColor = true;
			this.frontside.CheckStateChanged += new System.EventHandler(this.frontside_CheckStateChanged);
			// 
			// frontgroup
			// 
			this.frontgroup.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.frontgroup.Controls.Add(this.frontTextureOffset);
			this.frontgroup.Controls.Add(this.cbLightAbsoluteFront);
			this.frontgroup.Controls.Add(this.labelLightFront);
			this.frontgroup.Controls.Add(this.lightFront);
			this.frontgroup.Controls.Add(this.frontsector);
			this.frontgroup.Controls.Add(this.customfrontbutton);
			this.frontgroup.Controls.Add(label11);
			this.frontgroup.Controls.Add(this.frontlow);
			this.frontgroup.Controls.Add(this.frontmid);
			this.frontgroup.Controls.Add(this.fronthigh);
			this.frontgroup.Controls.Add(this.udmfPropertiesFront);
			this.frontgroup.Controls.Add(label5);
			this.frontgroup.Controls.Add(label4);
			this.frontgroup.Controls.Add(label3);
			this.frontgroup.Enabled = false;
			this.frontgroup.Location = new System.Drawing.Point(3, 3);
			this.frontgroup.Name = "frontgroup";
			this.frontgroup.Size = new System.Drawing.Size(535, 296);
			this.frontgroup.TabIndex = 1;
			this.frontgroup.TabStop = false;
			this.frontgroup.Text = "     ";
			// 
			// frontTextureOffset
			// 
			this.frontTextureOffset.ButtonStep = 16;
			this.frontTextureOffset.DefaultValue = 0;
			this.frontTextureOffset.Label = "Texture Offset:";
			this.frontTextureOffset.Location = new System.Drawing.Point(1, 65);
			this.frontTextureOffset.Name = "frontTextureOffset";
			this.frontTextureOffset.Size = new System.Drawing.Size(249, 26);
			this.frontTextureOffset.TabIndex = 41;
			// 
			// cbLightAbsoluteFront
			// 
			this.cbLightAbsoluteFront.AutoSize = true;
			this.cbLightAbsoluteFront.Location = new System.Drawing.Point(158, 100);
			this.cbLightAbsoluteFront.Name = "cbLightAbsoluteFront";
			this.cbLightAbsoluteFront.Size = new System.Drawing.Size(69, 18);
			this.cbLightAbsoluteFront.TabIndex = 27;
			this.cbLightAbsoluteFront.Tag = "lightabsolute";
			this.cbLightAbsoluteFront.Text = "Absolute";
			this.cbLightAbsoluteFront.UseVisualStyleBackColor = true;
			// 
			// lightFront
			// 
			this.lightFront.AllowDecimal = false;
			this.lightFront.AllowNegative = true;
			this.lightFront.AllowRelative = true;
			this.lightFront.ButtonStep = 16;
			this.lightFront.Location = new System.Drawing.Point(90, 96);
			this.lightFront.Name = "lightFront";
			this.lightFront.Size = new System.Drawing.Size(62, 24);
			this.lightFront.StepValues = null;
			this.lightFront.TabIndex = 26;
			this.lightFront.Tag = "light";
			// 
			// frontsector
			// 
			this.frontsector.AllowDecimal = false;
			this.frontsector.AllowNegative = false;
			this.frontsector.AllowRelative = false;
			this.frontsector.ButtonStep = 1;
			this.frontsector.Location = new System.Drawing.Point(90, 35);
			this.frontsector.Name = "frontsector";
			this.frontsector.Size = new System.Drawing.Size(130, 24);
			this.frontsector.StepValues = null;
			this.frontsector.TabIndex = 14;
			// 
			// customfrontbutton
			// 
			this.customfrontbutton.Location = new System.Drawing.Point(90, 126);
			this.customfrontbutton.Name = "customfrontbutton";
			this.customfrontbutton.Size = new System.Drawing.Size(130, 25);
			this.customfrontbutton.TabIndex = 3;
			this.customfrontbutton.Text = "Custom fields...";
			this.customfrontbutton.UseVisualStyleBackColor = true;
			this.customfrontbutton.Click += new System.EventHandler(this.customfrontbutton_Click);
			// 
			// frontlow
			// 
			this.frontlow.Location = new System.Drawing.Point(434, 37);
			this.frontlow.Name = "frontlow";
			this.frontlow.Required = false;
			this.frontlow.Size = new System.Drawing.Size(83, 112);
			this.frontlow.TabIndex = 6;
			this.frontlow.TextureName = "";
			// 
			// frontmid
			// 
			this.frontmid.Location = new System.Drawing.Point(343, 37);
			this.frontmid.Name = "frontmid";
			this.frontmid.Required = false;
			this.frontmid.Size = new System.Drawing.Size(83, 112);
			this.frontmid.TabIndex = 5;
			this.frontmid.TextureName = "";
			// 
			// fronthigh
			// 
			this.fronthigh.Location = new System.Drawing.Point(252, 37);
			this.fronthigh.Name = "fronthigh";
			this.fronthigh.Required = false;
			this.fronthigh.Size = new System.Drawing.Size(83, 112);
			this.fronthigh.TabIndex = 4;
			this.fronthigh.TextureName = "";
			// 
			// udmfPropertiesFront
			// 
			this.udmfPropertiesFront.Controls.Add(this.tabPage1);
			this.udmfPropertiesFront.Controls.Add(this.tabPage2);
			this.udmfPropertiesFront.ItemSize = new System.Drawing.Size(100, 19);
			this.udmfPropertiesFront.Location = new System.Drawing.Point(6, 169);
			this.udmfPropertiesFront.Margin = new System.Windows.Forms.Padding(1);
			this.udmfPropertiesFront.Name = "udmfPropertiesFront";
			this.udmfPropertiesFront.SelectedIndex = 0;
			this.udmfPropertiesFront.Size = new System.Drawing.Size(525, 123);
			this.udmfPropertiesFront.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
			this.udmfPropertiesFront.TabIndex = 24;
			// 
			// tabPage1
			// 
			this.tabPage1.Controls.Add(this.pfcFrontScaleBottom);
			this.tabPage1.Controls.Add(this.pfcFrontScaleMid);
			this.tabPage1.Controls.Add(this.pfcFrontScaleTop);
			this.tabPage1.Controls.Add(this.pfcFrontOffsetBottom);
			this.tabPage1.Controls.Add(this.pfcFrontOffsetMid);
			this.tabPage1.Controls.Add(this.pfcFrontOffsetTop);
			this.tabPage1.Location = new System.Drawing.Point(4, 23);
			this.tabPage1.Name = "tabPage1";
			this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage1.Size = new System.Drawing.Size(517, 96);
			this.tabPage1.TabIndex = 0;
			this.tabPage1.Text = "Offsets & Scale";
			this.tabPage1.UseVisualStyleBackColor = true;
			// 
			// pfcFrontScaleBottom
			// 
			this.pfcFrontScaleBottom.AllowDecimal = true;
			this.pfcFrontScaleBottom.ButtonStepFloat = 1F;
			this.pfcFrontScaleBottom.DefaultValue = 1F;
			this.pfcFrontScaleBottom.Field1 = "scalex_bottom";
			this.pfcFrontScaleBottom.Field2 = "scaley_bottom";
			this.pfcFrontScaleBottom.Label = "Lower Scale:";
			this.pfcFrontScaleBottom.Location = new System.Drawing.Point(262, 64);
			this.pfcFrontScaleBottom.Name = "pfcFrontScaleBottom";
			this.pfcFrontScaleBottom.Size = new System.Drawing.Size(253, 32);
			this.pfcFrontScaleBottom.TabIndex = 40;
			// 
			// pfcFrontScaleMid
			// 
			this.pfcFrontScaleMid.AllowDecimal = true;
			this.pfcFrontScaleMid.ButtonStepFloat = 1F;
			this.pfcFrontScaleMid.DefaultValue = 1F;
			this.pfcFrontScaleMid.Field1 = "scalex_mid";
			this.pfcFrontScaleMid.Field2 = "scaley_mid";
			this.pfcFrontScaleMid.Label = "Middle Scale:";
			this.pfcFrontScaleMid.Location = new System.Drawing.Point(262, 34);
			this.pfcFrontScaleMid.Name = "pfcFrontScaleMid";
			this.pfcFrontScaleMid.Size = new System.Drawing.Size(253, 32);
			this.pfcFrontScaleMid.TabIndex = 39;
			// 
			// pfcFrontScaleTop
			// 
			this.pfcFrontScaleTop.AllowDecimal = true;
			this.pfcFrontScaleTop.ButtonStepFloat = 1F;
			this.pfcFrontScaleTop.DefaultValue = 1F;
			this.pfcFrontScaleTop.Field1 = "scalex_top";
			this.pfcFrontScaleTop.Field2 = "scaley_top";
			this.pfcFrontScaleTop.Label = "Upper Scale:";
			this.pfcFrontScaleTop.Location = new System.Drawing.Point(262, 3);
			this.pfcFrontScaleTop.Name = "pfcFrontScaleTop";
			this.pfcFrontScaleTop.Size = new System.Drawing.Size(253, 30);
			this.pfcFrontScaleTop.TabIndex = 38;
			// 
			// pfcFrontOffsetBottom
			// 
			this.pfcFrontOffsetBottom.AllowDecimal = true;
			this.pfcFrontOffsetBottom.ButtonStepFloat = 16F;
			this.pfcFrontOffsetBottom.DefaultValue = 0F;
			this.pfcFrontOffsetBottom.Field1 = "offsetx_bottom";
			this.pfcFrontOffsetBottom.Field2 = "offsety_bottom";
			this.pfcFrontOffsetBottom.Label = "Lower Offset:";
			this.pfcFrontOffsetBottom.Location = new System.Drawing.Point(3, 64);
			this.pfcFrontOffsetBottom.Name = "pfcFrontOffsetBottom";
			this.pfcFrontOffsetBottom.Size = new System.Drawing.Size(253, 30);
			this.pfcFrontOffsetBottom.TabIndex = 37;
			// 
			// pfcFrontOffsetMid
			// 
			this.pfcFrontOffsetMid.AllowDecimal = true;
			this.pfcFrontOffsetMid.ButtonStepFloat = 16F;
			this.pfcFrontOffsetMid.DefaultValue = 0F;
			this.pfcFrontOffsetMid.Field1 = "offsetx_mid";
			this.pfcFrontOffsetMid.Field2 = "offsety_mid";
			this.pfcFrontOffsetMid.Label = "Middle Offset:";
			this.pfcFrontOffsetMid.Location = new System.Drawing.Point(3, 34);
			this.pfcFrontOffsetMid.Name = "pfcFrontOffsetMid";
			this.pfcFrontOffsetMid.Size = new System.Drawing.Size(253, 30);
			this.pfcFrontOffsetMid.TabIndex = 36;
			// 
			// pfcFrontOffsetTop
			// 
			this.pfcFrontOffsetTop.AllowDecimal = true;
			this.pfcFrontOffsetTop.ButtonStepFloat = 16F;
			this.pfcFrontOffsetTop.DefaultValue = 0F;
			this.pfcFrontOffsetTop.Field1 = "offsetx_top";
			this.pfcFrontOffsetTop.Field2 = "offsety_top";
			this.pfcFrontOffsetTop.Label = "Upper Offset:";
			this.pfcFrontOffsetTop.Location = new System.Drawing.Point(3, 6);
			this.pfcFrontOffsetTop.Name = "pfcFrontOffsetTop";
			this.pfcFrontOffsetTop.Size = new System.Drawing.Size(253, 28);
			this.pfcFrontOffsetTop.TabIndex = 35;
			// 
			// tabPage2
			// 
			this.tabPage2.Controls.Add(this.cblightfogFront);
			this.tabPage2.Controls.Add(this.cbsmoothlightingFront);
			this.tabPage2.Controls.Add(this.cbnodecalsFront);
			this.tabPage2.Controls.Add(this.cbnofakecontrastFront);
			this.tabPage2.Controls.Add(this.cbWrapMidtexFront);
			this.tabPage2.Controls.Add(this.cbClipMidtexFront);
			this.tabPage2.Location = new System.Drawing.Point(4, 23);
			this.tabPage2.Name = "tabPage2";
			this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage2.Size = new System.Drawing.Size(517, 96);
			this.tabPage2.TabIndex = 1;
			this.tabPage2.Text = "Flags";
			this.tabPage2.UseVisualStyleBackColor = true;
			// 
			// cblightfogFront
			// 
			this.cblightfogFront.AutoSize = true;
			this.cblightfogFront.Location = new System.Drawing.Point(152, 65);
			this.cblightfogFront.Name = "cblightfogFront";
			this.cblightfogFront.Size = new System.Drawing.Size(179, 18);
			this.cblightfogFront.TabIndex = 11;
			this.cblightfogFront.Tag = "lightfog";
			this.cblightfogFront.Text = "Use UDMF light on fogged walls";
			this.cblightfogFront.UseVisualStyleBackColor = true;
			// 
			// cbsmoothlightingFront
			// 
			this.cbsmoothlightingFront.AutoSize = true;
			this.cbsmoothlightingFront.Location = new System.Drawing.Point(152, 41);
			this.cbsmoothlightingFront.Name = "cbsmoothlightingFront";
			this.cbsmoothlightingFront.Size = new System.Drawing.Size(102, 18);
			this.cbsmoothlightingFront.TabIndex = 10;
			this.cbsmoothlightingFront.Tag = "smoothlighting";
			this.cbsmoothlightingFront.Text = "Smooth Lighting";
			this.cbsmoothlightingFront.UseVisualStyleBackColor = true;
			// 
			// cbnodecalsFront
			// 
			this.cbnodecalsFront.AutoSize = true;
			this.cbnodecalsFront.Location = new System.Drawing.Point(152, 17);
			this.cbnodecalsFront.Name = "cbnodecalsFront";
			this.cbnodecalsFront.Size = new System.Drawing.Size(75, 18);
			this.cbnodecalsFront.TabIndex = 9;
			this.cbnodecalsFront.Tag = "nodecals";
			this.cbnodecalsFront.Text = "No Decals";
			this.cbnodecalsFront.UseVisualStyleBackColor = true;
			// 
			// cbnofakecontrastFront
			// 
			this.cbnofakecontrastFront.AutoSize = true;
			this.cbnofakecontrastFront.Location = new System.Drawing.Point(10, 65);
			this.cbnofakecontrastFront.Name = "cbnofakecontrastFront";
			this.cbnofakecontrastFront.Size = new System.Drawing.Size(109, 18);
			this.cbnofakecontrastFront.TabIndex = 8;
			this.cbnofakecontrastFront.Tag = "nofakecontrast";
			this.cbnofakecontrastFront.Text = "No Fake Contrast";
			this.cbnofakecontrastFront.UseVisualStyleBackColor = true;
			// 
			// cbWrapMidtexFront
			// 
			this.cbWrapMidtexFront.AutoSize = true;
			this.cbWrapMidtexFront.Location = new System.Drawing.Point(10, 41);
			this.cbWrapMidtexFront.Name = "cbWrapMidtexFront";
			this.cbWrapMidtexFront.Size = new System.Drawing.Size(124, 18);
			this.cbWrapMidtexFront.TabIndex = 7;
			this.cbWrapMidtexFront.Tag = "wrapmidtex";
			this.cbWrapMidtexFront.Text = "Wrap Middle Texture";
			this.cbWrapMidtexFront.UseVisualStyleBackColor = true;
			// 
			// cbClipMidtexFront
			// 
			this.cbClipMidtexFront.AutoSize = true;
			this.cbClipMidtexFront.Location = new System.Drawing.Point(10, 17);
			this.cbClipMidtexFront.Name = "cbClipMidtexFront";
			this.cbClipMidtexFront.Size = new System.Drawing.Size(115, 18);
			this.cbClipMidtexFront.TabIndex = 6;
			this.cbClipMidtexFront.Tag = "clipmidtex";
			this.cbClipMidtexFront.Text = "Clip Middle Texture";
			this.cbClipMidtexFront.UseVisualStyleBackColor = true;
			// 
			// backside
			// 
			this.backside.AutoSize = true;
			this.backside.Location = new System.Drawing.Point(15, 1);
			this.backside.Name = "backside";
			this.backside.Size = new System.Drawing.Size(74, 18);
			this.backside.TabIndex = 0;
			this.backside.Text = "Back Side";
			this.backside.UseVisualStyleBackColor = true;
			this.backside.CheckStateChanged += new System.EventHandler(this.backside_CheckStateChanged);
			// 
			// backgroup
			// 
			this.backgroup.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.backgroup.Controls.Add(this.backTextureOffset);
			this.backgroup.Controls.Add(this.cbLightAbsoluteBack);
			this.backgroup.Controls.Add(this.labelLightBack);
			this.backgroup.Controls.Add(this.lightBack);
			this.backgroup.Controls.Add(this.backsector);
			this.backgroup.Controls.Add(this.custombackbutton);
			this.backgroup.Controls.Add(label12);
			this.backgroup.Controls.Add(this.backlow);
			this.backgroup.Controls.Add(this.backmid);
			this.backgroup.Controls.Add(this.backhigh);
			this.backgroup.Controls.Add(label8);
			this.backgroup.Controls.Add(label9);
			this.backgroup.Controls.Add(label10);
			this.backgroup.Controls.Add(this.udmfPropertiesBack);
			this.backgroup.Enabled = false;
			this.backgroup.Location = new System.Drawing.Point(3, 3);
			this.backgroup.Name = "backgroup";
			this.backgroup.Size = new System.Drawing.Size(535, 299);
			this.backgroup.TabIndex = 1;
			this.backgroup.TabStop = false;
			this.backgroup.Text = "     ";
			// 
			// backTextureOffset
			// 
			this.backTextureOffset.ButtonStep = 16;
			this.backTextureOffset.DefaultValue = 0;
			this.backTextureOffset.Label = "Texture Offset:";
			this.backTextureOffset.Location = new System.Drawing.Point(1, 65);
			this.backTextureOffset.Name = "backTextureOffset";
			this.backTextureOffset.Size = new System.Drawing.Size(249, 28);
			this.backTextureOffset.TabIndex = 42;
			// 
			// cbLightAbsoluteBack
			// 
			this.cbLightAbsoluteBack.AutoSize = true;
			this.cbLightAbsoluteBack.Location = new System.Drawing.Point(158, 100);
			this.cbLightAbsoluteBack.Name = "cbLightAbsoluteBack";
			this.cbLightAbsoluteBack.Size = new System.Drawing.Size(69, 18);
			this.cbLightAbsoluteBack.TabIndex = 30;
			this.cbLightAbsoluteBack.Tag = "lightabsolute";
			this.cbLightAbsoluteBack.Text = "Absolute";
			this.cbLightAbsoluteBack.UseVisualStyleBackColor = true;
			// 
			// labelLightBack
			// 
			this.labelLightBack.Location = new System.Drawing.Point(8, 101);
			this.labelLightBack.Name = "labelLightBack";
			this.labelLightBack.Size = new System.Drawing.Size(80, 14);
			this.labelLightBack.TabIndex = 28;
			this.labelLightBack.Tag = "";
			this.labelLightBack.Text = "Brightness:";
			this.labelLightBack.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// lightBack
			// 
			this.lightBack.AllowDecimal = false;
			this.lightBack.AllowNegative = true;
			this.lightBack.AllowRelative = true;
			this.lightBack.ButtonStep = 16;
			this.lightBack.Location = new System.Drawing.Point(90, 96);
			this.lightBack.Name = "lightBack";
			this.lightBack.Size = new System.Drawing.Size(62, 24);
			this.lightBack.StepValues = null;
			this.lightBack.TabIndex = 29;
			this.lightBack.Tag = "light";
			// 
			// backsector
			// 
			this.backsector.AllowDecimal = false;
			this.backsector.AllowNegative = false;
			this.backsector.AllowRelative = false;
			this.backsector.ButtonStep = 1;
			this.backsector.Location = new System.Drawing.Point(90, 35);
			this.backsector.Name = "backsector";
			this.backsector.Size = new System.Drawing.Size(130, 24);
			this.backsector.StepValues = null;
			this.backsector.TabIndex = 17;
			// 
			// custombackbutton
			// 
			this.custombackbutton.Location = new System.Drawing.Point(90, 126);
			this.custombackbutton.Name = "custombackbutton";
			this.custombackbutton.Size = new System.Drawing.Size(130, 25);
			this.custombackbutton.TabIndex = 3;
			this.custombackbutton.Text = "Custom fields...";
			this.custombackbutton.UseVisualStyleBackColor = true;
			this.custombackbutton.Click += new System.EventHandler(this.custombackbutton_Click);
			// 
			// backlow
			// 
			this.backlow.Location = new System.Drawing.Point(437, 37);
			this.backlow.Name = "backlow";
			this.backlow.Required = false;
			this.backlow.Size = new System.Drawing.Size(83, 112);
			this.backlow.TabIndex = 6;
			this.backlow.TextureName = "";
			// 
			// backmid
			// 
			this.backmid.Location = new System.Drawing.Point(346, 37);
			this.backmid.Name = "backmid";
			this.backmid.Required = false;
			this.backmid.Size = new System.Drawing.Size(83, 112);
			this.backmid.TabIndex = 5;
			this.backmid.TextureName = "";
			// 
			// backhigh
			// 
			this.backhigh.Location = new System.Drawing.Point(255, 37);
			this.backhigh.Name = "backhigh";
			this.backhigh.Required = false;
			this.backhigh.Size = new System.Drawing.Size(83, 112);
			this.backhigh.TabIndex = 4;
			this.backhigh.TextureName = "";
			// 
			// udmfPropertiesBack
			// 
			this.udmfPropertiesBack.Controls.Add(this.tabPage5);
			this.udmfPropertiesBack.Controls.Add(this.tabPage6);
			this.udmfPropertiesBack.ItemSize = new System.Drawing.Size(100, 19);
			this.udmfPropertiesBack.Location = new System.Drawing.Point(10, 172);
			this.udmfPropertiesBack.Margin = new System.Windows.Forms.Padding(1);
			this.udmfPropertiesBack.Name = "udmfPropertiesBack";
			this.udmfPropertiesBack.SelectedIndex = 0;
			this.udmfPropertiesBack.Size = new System.Drawing.Size(525, 123);
			this.udmfPropertiesBack.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
			this.udmfPropertiesBack.TabIndex = 25;
			// 
			// tabPage5
			// 
			this.tabPage5.Controls.Add(this.pfcBackScaleBottom);
			this.tabPage5.Controls.Add(this.pfcBackScaleMid);
			this.tabPage5.Controls.Add(this.pfcBackScaleTop);
			this.tabPage5.Controls.Add(this.pfcBackOffsetBottom);
			this.tabPage5.Controls.Add(this.pfcBackOffsetMid);
			this.tabPage5.Controls.Add(this.pfcBackOffsetTop);
			this.tabPage5.Location = new System.Drawing.Point(4, 23);
			this.tabPage5.Name = "tabPage5";
			this.tabPage5.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage5.Size = new System.Drawing.Size(517, 96);
			this.tabPage5.TabIndex = 0;
			this.tabPage5.Text = "Offsets & Scale";
			this.tabPage5.UseVisualStyleBackColor = true;
			// 
			// pfcBackScaleBottom
			// 
			this.pfcBackScaleBottom.AllowDecimal = true;
			this.pfcBackScaleBottom.ButtonStepFloat = 1F;
			this.pfcBackScaleBottom.DefaultValue = 1F;
			this.pfcBackScaleBottom.Field1 = "scalex_bottom";
			this.pfcBackScaleBottom.Field2 = "scaley_bottom";
			this.pfcBackScaleBottom.Label = "Lower Scale:";
			this.pfcBackScaleBottom.Location = new System.Drawing.Point(262, 64);
			this.pfcBackScaleBottom.Name = "pfcBackScaleBottom";
			this.pfcBackScaleBottom.Size = new System.Drawing.Size(253, 32);
			this.pfcBackScaleBottom.TabIndex = 40;
			// 
			// pfcBackScaleMid
			// 
			this.pfcBackScaleMid.AllowDecimal = true;
			this.pfcBackScaleMid.ButtonStepFloat = 1F;
			this.pfcBackScaleMid.DefaultValue = 1F;
			this.pfcBackScaleMid.Field1 = "scalex_mid";
			this.pfcBackScaleMid.Field2 = "scaley_mid";
			this.pfcBackScaleMid.Label = "Middle Scale:";
			this.pfcBackScaleMid.Location = new System.Drawing.Point(262, 34);
			this.pfcBackScaleMid.Name = "pfcBackScaleMid";
			this.pfcBackScaleMid.Size = new System.Drawing.Size(253, 32);
			this.pfcBackScaleMid.TabIndex = 39;
			// 
			// pfcBackScaleTop
			// 
			this.pfcBackScaleTop.AllowDecimal = true;
			this.pfcBackScaleTop.ButtonStepFloat = 1F;
			this.pfcBackScaleTop.DefaultValue = 1F;
			this.pfcBackScaleTop.Field1 = "scalex_top";
			this.pfcBackScaleTop.Field2 = "scaley_top";
			this.pfcBackScaleTop.Label = "Upper Scale:";
			this.pfcBackScaleTop.Location = new System.Drawing.Point(262, 3);
			this.pfcBackScaleTop.Name = "pfcBackScaleTop";
			this.pfcBackScaleTop.Size = new System.Drawing.Size(253, 30);
			this.pfcBackScaleTop.TabIndex = 38;
			// 
			// pfcBackOffsetBottom
			// 
			this.pfcBackOffsetBottom.AllowDecimal = true;
			this.pfcBackOffsetBottom.ButtonStepFloat = 16F;
			this.pfcBackOffsetBottom.DefaultValue = 0F;
			this.pfcBackOffsetBottom.Field1 = "offsetx_bottom";
			this.pfcBackOffsetBottom.Field2 = "offsety_bottom";
			this.pfcBackOffsetBottom.Label = "Lower Offset:";
			this.pfcBackOffsetBottom.Location = new System.Drawing.Point(3, 64);
			this.pfcBackOffsetBottom.Name = "pfcBackOffsetBottom";
			this.pfcBackOffsetBottom.Size = new System.Drawing.Size(253, 30);
			this.pfcBackOffsetBottom.TabIndex = 37;
			// 
			// pfcBackOffsetMid
			// 
			this.pfcBackOffsetMid.AllowDecimal = true;
			this.pfcBackOffsetMid.ButtonStepFloat = 16F;
			this.pfcBackOffsetMid.DefaultValue = 0F;
			this.pfcBackOffsetMid.Field1 = "offsetx_mid";
			this.pfcBackOffsetMid.Field2 = "offsety_mid";
			this.pfcBackOffsetMid.Label = "Middle Offset:";
			this.pfcBackOffsetMid.Location = new System.Drawing.Point(3, 34);
			this.pfcBackOffsetMid.Name = "pfcBackOffsetMid";
			this.pfcBackOffsetMid.Size = new System.Drawing.Size(253, 30);
			this.pfcBackOffsetMid.TabIndex = 36;
			// 
			// pfcBackOffsetTop
			// 
			this.pfcBackOffsetTop.AllowDecimal = true;
			this.pfcBackOffsetTop.ButtonStepFloat = 16F;
			this.pfcBackOffsetTop.DefaultValue = 0F;
			this.pfcBackOffsetTop.Field1 = "offsetx_top";
			this.pfcBackOffsetTop.Field2 = "offsety_top";
			this.pfcBackOffsetTop.Label = "Upper Offset:";
			this.pfcBackOffsetTop.Location = new System.Drawing.Point(3, 6);
			this.pfcBackOffsetTop.Name = "pfcBackOffsetTop";
			this.pfcBackOffsetTop.Size = new System.Drawing.Size(253, 28);
			this.pfcBackOffsetTop.TabIndex = 35;
			// 
			// tabPage6
			// 
			this.tabPage6.Controls.Add(this.cblightfogBack);
			this.tabPage6.Controls.Add(this.cbsmoothlightingBack);
			this.tabPage6.Controls.Add(this.cbnodecalsBack);
			this.tabPage6.Controls.Add(this.cbnofakecontrastBack);
			this.tabPage6.Controls.Add(this.cbWrapMidtexBack);
			this.tabPage6.Controls.Add(this.cbClipMidtexBack);
			this.tabPage6.Location = new System.Drawing.Point(4, 23);
			this.tabPage6.Name = "tabPage6";
			this.tabPage6.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage6.Size = new System.Drawing.Size(517, 96);
			this.tabPage6.TabIndex = 1;
			this.tabPage6.Text = "Flags";
			this.tabPage6.UseVisualStyleBackColor = true;
			// 
			// cblightfogBack
			// 
			this.cblightfogBack.AutoSize = true;
			this.cblightfogBack.Location = new System.Drawing.Point(148, 65);
			this.cblightfogBack.Name = "cblightfogBack";
			this.cblightfogBack.Size = new System.Drawing.Size(179, 18);
			this.cblightfogBack.TabIndex = 11;
			this.cblightfogBack.Tag = "lightfog";
			this.cblightfogBack.Text = "Use UDMF light on fogged walls";
			this.cblightfogBack.UseVisualStyleBackColor = true;
			// 
			// cbsmoothlightingBack
			// 
			this.cbsmoothlightingBack.AutoSize = true;
			this.cbsmoothlightingBack.Location = new System.Drawing.Point(148, 41);
			this.cbsmoothlightingBack.Name = "cbsmoothlightingBack";
			this.cbsmoothlightingBack.Size = new System.Drawing.Size(102, 18);
			this.cbsmoothlightingBack.TabIndex = 10;
			this.cbsmoothlightingBack.Tag = "smoothlighting";
			this.cbsmoothlightingBack.Text = "Smooth Lighting";
			this.cbsmoothlightingBack.UseVisualStyleBackColor = true;
			// 
			// cbnodecalsBack
			// 
			this.cbnodecalsBack.AutoSize = true;
			this.cbnodecalsBack.Location = new System.Drawing.Point(148, 17);
			this.cbnodecalsBack.Name = "cbnodecalsBack";
			this.cbnodecalsBack.Size = new System.Drawing.Size(75, 18);
			this.cbnodecalsBack.TabIndex = 9;
			this.cbnodecalsBack.Tag = "nodecals";
			this.cbnodecalsBack.Text = "No Decals";
			this.cbnodecalsBack.UseVisualStyleBackColor = true;
			// 
			// cbnofakecontrastBack
			// 
			this.cbnofakecontrastBack.AutoSize = true;
			this.cbnofakecontrastBack.Location = new System.Drawing.Point(6, 65);
			this.cbnofakecontrastBack.Name = "cbnofakecontrastBack";
			this.cbnofakecontrastBack.Size = new System.Drawing.Size(109, 18);
			this.cbnofakecontrastBack.TabIndex = 8;
			this.cbnofakecontrastBack.Tag = "nofakecontrast";
			this.cbnofakecontrastBack.Text = "No Fake Contrast";
			this.cbnofakecontrastBack.UseVisualStyleBackColor = true;
			// 
			// cbWrapMidtexBack
			// 
			this.cbWrapMidtexBack.AutoSize = true;
			this.cbWrapMidtexBack.Location = new System.Drawing.Point(6, 41);
			this.cbWrapMidtexBack.Name = "cbWrapMidtexBack";
			this.cbWrapMidtexBack.Size = new System.Drawing.Size(124, 18);
			this.cbWrapMidtexBack.TabIndex = 7;
			this.cbWrapMidtexBack.Tag = "wrapmidtex";
			this.cbWrapMidtexBack.Text = "Wrap Middle Texture";
			this.cbWrapMidtexBack.UseVisualStyleBackColor = true;
			// 
			// cbClipMidtexBack
			// 
			this.cbClipMidtexBack.AutoSize = true;
			this.cbClipMidtexBack.Location = new System.Drawing.Point(6, 17);
			this.cbClipMidtexBack.Name = "cbClipMidtexBack";
			this.cbClipMidtexBack.Size = new System.Drawing.Size(115, 18);
			this.cbClipMidtexBack.TabIndex = 6;
			this.cbClipMidtexBack.Tag = "clipmidtex";
			this.cbClipMidtexBack.Text = "Clip Middle Texture";
			this.cbClipMidtexBack.UseVisualStyleBackColor = true;
			// 
			// tabcustom
			// 
			this.tabcustom.Controls.Add(this.fieldslist);
			this.tabcustom.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.tabcustom.Location = new System.Drawing.Point(4, 23);
			this.tabcustom.Name = "tabcustom";
			this.tabcustom.Padding = new System.Windows.Forms.Padding(3);
			this.tabcustom.Size = new System.Drawing.Size(549, 621);
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
			this.fieldslist.Location = new System.Drawing.Point(11, 11);
			this.fieldslist.Margin = new System.Windows.Forms.Padding(8);
			this.fieldslist.Name = "fieldslist";
			this.fieldslist.PropertyColumnVisible = true;
			this.fieldslist.PropertyColumnWidth = 150;
			this.fieldslist.Size = new System.Drawing.Size(527, 602);
			this.fieldslist.TabIndex = 0;
			this.fieldslist.TypeColumnVisible = true;
			this.fieldslist.TypeColumnWidth = 100;
			this.fieldslist.ValueColumnVisible = true;
			this.fieldslist.OnFieldValueChanged += new CodeImp.DoomBuilder.Controls.FieldsEditorControl.SingleFieldNameEvent(this.fieldslist_OnFieldValueChanged);
			// 
			// heightpanel1
			// 
			this.heightpanel1.BackColor = System.Drawing.Color.Navy;
			this.heightpanel1.Location = new System.Drawing.Point(0, -19);
			this.heightpanel1.Name = "heightpanel1";
			this.heightpanel1.Size = new System.Drawing.Size(78, 480);
			this.heightpanel1.TabIndex = 3;
			this.heightpanel1.Visible = false;
			// 
			// heightpanel2
			// 
			this.heightpanel2.BackColor = System.Drawing.Color.Navy;
			this.heightpanel2.Location = new System.Drawing.Point(473, -19);
			this.heightpanel2.Name = "heightpanel2";
			this.heightpanel2.Size = new System.Drawing.Size(88, 470);
			this.heightpanel2.TabIndex = 4;
			this.heightpanel2.Visible = false;
			// 
			// tabPage3
			// 
			this.tabPage3.Controls.Add(this.pairedFieldsControl1);
			this.tabPage3.Controls.Add(this.pairedFieldsControl2);
			this.tabPage3.Controls.Add(this.pairedFieldsControl3);
			this.tabPage3.Controls.Add(this.pairedFieldsControl4);
			this.tabPage3.Controls.Add(this.pairedFieldsControl5);
			this.tabPage3.Controls.Add(this.pairedFieldsControl6);
			this.tabPage3.Location = new System.Drawing.Point(4, 23);
			this.tabPage3.Name = "tabPage3";
			this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage3.Size = new System.Drawing.Size(503, 96);
			this.tabPage3.TabIndex = 0;
			this.tabPage3.Text = "Offsets & Scale";
			this.tabPage3.UseVisualStyleBackColor = true;
			// 
			// pairedFieldsControl1
			// 
			this.pairedFieldsControl1.AllowDecimal = true;
			this.pairedFieldsControl1.ButtonStepFloat = 1F;
			this.pairedFieldsControl1.DefaultValue = 1F;
			this.pairedFieldsControl1.Field1 = "scalex_bottom";
			this.pairedFieldsControl1.Field2 = "scaley_bottom";
			this.pairedFieldsControl1.Label = "Lower Scale:";
			this.pairedFieldsControl1.Location = new System.Drawing.Point(254, 64);
			this.pairedFieldsControl1.Name = "pairedFieldsControl1";
			this.pairedFieldsControl1.Size = new System.Drawing.Size(253, 32);
			this.pairedFieldsControl1.TabIndex = 40;
			// 
			// pairedFieldsControl2
			// 
			this.pairedFieldsControl2.AllowDecimal = true;
			this.pairedFieldsControl2.ButtonStepFloat = 1F;
			this.pairedFieldsControl2.DefaultValue = 1F;
			this.pairedFieldsControl2.Field1 = "scalex_mid";
			this.pairedFieldsControl2.Field2 = "scaley_mid";
			this.pairedFieldsControl2.Label = "Middle Scale:";
			this.pairedFieldsControl2.Location = new System.Drawing.Point(254, 34);
			this.pairedFieldsControl2.Name = "pairedFieldsControl2";
			this.pairedFieldsControl2.Size = new System.Drawing.Size(253, 32);
			this.pairedFieldsControl2.TabIndex = 39;
			// 
			// pairedFieldsControl3
			// 
			this.pairedFieldsControl3.AllowDecimal = true;
			this.pairedFieldsControl3.ButtonStepFloat = 1F;
			this.pairedFieldsControl3.DefaultValue = 1F;
			this.pairedFieldsControl3.Field1 = "scalex_top";
			this.pairedFieldsControl3.Field2 = "scaley_top";
			this.pairedFieldsControl3.Label = "Upper Scale:";
			this.pairedFieldsControl3.Location = new System.Drawing.Point(254, 3);
			this.pairedFieldsControl3.Name = "pairedFieldsControl3";
			this.pairedFieldsControl3.Size = new System.Drawing.Size(253, 30);
			this.pairedFieldsControl3.TabIndex = 38;
			// 
			// pairedFieldsControl4
			// 
			this.pairedFieldsControl4.AllowDecimal = false;
			this.pairedFieldsControl4.ButtonStep = 1;
			this.pairedFieldsControl4.DefaultValue = 0F;
			this.pairedFieldsControl4.Field1 = "offsetx_bottom";
			this.pairedFieldsControl4.Field2 = "offsety_bottom";
			this.pairedFieldsControl4.Label = "Lower Offset:";
			this.pairedFieldsControl4.Location = new System.Drawing.Point(3, 64);
			this.pairedFieldsControl4.Name = "pairedFieldsControl4";
			this.pairedFieldsControl4.Size = new System.Drawing.Size(253, 30);
			this.pairedFieldsControl4.TabIndex = 37;
			// 
			// pairedFieldsControl5
			// 
			this.pairedFieldsControl5.AllowDecimal = false;
			this.pairedFieldsControl5.ButtonStep = 1;
			this.pairedFieldsControl5.DefaultValue = 0F;
			this.pairedFieldsControl5.Field1 = "offsetx_mid";
			this.pairedFieldsControl5.Field2 = "offsety_mid";
			this.pairedFieldsControl5.Label = "Middle Offset:";
			this.pairedFieldsControl5.Location = new System.Drawing.Point(3, 34);
			this.pairedFieldsControl5.Name = "pairedFieldsControl5";
			this.pairedFieldsControl5.Size = new System.Drawing.Size(253, 30);
			this.pairedFieldsControl5.TabIndex = 36;
			// 
			// pairedFieldsControl6
			// 
			this.pairedFieldsControl6.AllowDecimal = false;
			this.pairedFieldsControl6.ButtonStep = 1;
			this.pairedFieldsControl6.DefaultValue = 0F;
			this.pairedFieldsControl6.Field1 = "offsetx_top";
			this.pairedFieldsControl6.Field2 = "offsety_top";
			this.pairedFieldsControl6.Label = "Upper Offset:";
			this.pairedFieldsControl6.Location = new System.Drawing.Point(3, 6);
			this.pairedFieldsControl6.Name = "pairedFieldsControl6";
			this.pairedFieldsControl6.Size = new System.Drawing.Size(253, 28);
			this.pairedFieldsControl6.TabIndex = 35;
			// 
			// tabPage4
			// 
			this.tabPage4.Controls.Add(this.checkBox2);
			this.tabPage4.Controls.Add(label1);
			this.tabPage4.Controls.Add(this.buttonsNumericTextbox1);
			this.tabPage4.Controls.Add(this.checkBox3);
			this.tabPage4.Controls.Add(this.checkBox4);
			this.tabPage4.Controls.Add(this.checkBox5);
			this.tabPage4.Controls.Add(this.checkBox6);
			this.tabPage4.Controls.Add(this.checkBox7);
			this.tabPage4.Controls.Add(this.checkBox8);
			this.tabPage4.Location = new System.Drawing.Point(4, 23);
			this.tabPage4.Name = "tabPage4";
			this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage4.Size = new System.Drawing.Size(503, 96);
			this.tabPage4.TabIndex = 1;
			this.tabPage4.Text = "Settings";
			this.tabPage4.UseVisualStyleBackColor = true;
			// 
			// checkBox2
			// 
			this.checkBox2.AutoSize = true;
			this.checkBox2.Location = new System.Drawing.Point(26, 41);
			this.checkBox2.Name = "checkBox2";
			this.checkBox2.Size = new System.Drawing.Size(107, 17);
			this.checkBox2.TabIndex = 27;
			this.checkBox2.Tag = "lightabsolute";
			this.checkBox2.Text = "Absolute Lighting";
			this.checkBox2.UseVisualStyleBackColor = true;
			// 
			// buttonsNumericTextbox1
			// 
			this.buttonsNumericTextbox1.AllowDecimal = false;
			this.buttonsNumericTextbox1.AllowNegative = true;
			this.buttonsNumericTextbox1.AllowRelative = true;
			this.buttonsNumericTextbox1.ButtonStep = 1;
			this.buttonsNumericTextbox1.Location = new System.Drawing.Point(57, 13);
			this.buttonsNumericTextbox1.Name = "buttonsNumericTextbox1";
			this.buttonsNumericTextbox1.Size = new System.Drawing.Size(78, 24);
			this.buttonsNumericTextbox1.StepValues = null;
			this.buttonsNumericTextbox1.TabIndex = 26;
			this.buttonsNumericTextbox1.Tag = "light";
			// 
			// checkBox3
			// 
			this.checkBox3.AutoSize = true;
			this.checkBox3.Location = new System.Drawing.Point(314, 65);
			this.checkBox3.Name = "checkBox3";
			this.checkBox3.Size = new System.Drawing.Size(178, 17);
			this.checkBox3.TabIndex = 11;
			this.checkBox3.Tag = "lightfog";
			this.checkBox3.Text = "Use UDMF light on fogged walls";
			this.checkBox3.UseVisualStyleBackColor = true;
			// 
			// checkBox4
			// 
			this.checkBox4.AutoSize = true;
			this.checkBox4.Location = new System.Drawing.Point(314, 41);
			this.checkBox4.Name = "checkBox4";
			this.checkBox4.Size = new System.Drawing.Size(102, 17);
			this.checkBox4.TabIndex = 10;
			this.checkBox4.Tag = "smoothlighting";
			this.checkBox4.Text = "Smooth Lighting";
			this.checkBox4.UseVisualStyleBackColor = true;
			// 
			// checkBox5
			// 
			this.checkBox5.AutoSize = true;
			this.checkBox5.Location = new System.Drawing.Point(314, 17);
			this.checkBox5.Name = "checkBox5";
			this.checkBox5.Size = new System.Drawing.Size(76, 17);
			this.checkBox5.TabIndex = 9;
			this.checkBox5.Tag = "nodecals";
			this.checkBox5.Text = "No Decals";
			this.checkBox5.UseVisualStyleBackColor = true;
			// 
			// checkBox6
			// 
			this.checkBox6.AutoSize = true;
			this.checkBox6.Location = new System.Drawing.Point(172, 65);
			this.checkBox6.Name = "checkBox6";
			this.checkBox6.Size = new System.Drawing.Size(109, 17);
			this.checkBox6.TabIndex = 8;
			this.checkBox6.Tag = "nofakecontrast";
			this.checkBox6.Text = "No Fake Contrast";
			this.checkBox6.UseVisualStyleBackColor = true;
			// 
			// checkBox7
			// 
			this.checkBox7.AutoSize = true;
			this.checkBox7.Location = new System.Drawing.Point(172, 41);
			this.checkBox7.Name = "checkBox7";
			this.checkBox7.Size = new System.Drawing.Size(125, 17);
			this.checkBox7.TabIndex = 7;
			this.checkBox7.Tag = "wrapmidtex";
			this.checkBox7.Text = "Wrap Middle Texture";
			this.checkBox7.UseVisualStyleBackColor = true;
			// 
			// checkBox8
			// 
			this.checkBox8.AutoSize = true;
			this.checkBox8.Location = new System.Drawing.Point(172, 17);
			this.checkBox8.Name = "checkBox8";
			this.checkBox8.Size = new System.Drawing.Size(116, 17);
			this.checkBox8.TabIndex = 6;
			this.checkBox8.Tag = "clipmidtex";
			this.checkBox8.Text = "Clip Middle Texture";
			this.checkBox8.UseVisualStyleBackColor = true;
			// 
			// LinedefEditForm
			// 
			this.AcceptButton = this.apply;
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.CancelButton = this.cancel;
			this.ClientSize = new System.Drawing.Size(577, 697);
			this.Controls.Add(this.tabs);
			this.Controls.Add(this.cancel);
			this.Controls.Add(this.apply);
			this.Controls.Add(this.heightpanel1);
			this.Controls.Add(this.heightpanel2);
			this.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "LinedefEditForm";
			this.Opacity = 0;
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Edit Linedef";
			this.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.LinedefEditForm_HelpRequested);
			this.actiongroup.ResumeLayout(false);
			this.actiongroup.PerformLayout();
			this.argspanel.ResumeLayout(false);
			this.hexenpanel.ResumeLayout(false);
			this.hexenpanel.PerformLayout();
			this.udmfpanel.ResumeLayout(false);
			this.flagsgroup.ResumeLayout(false);
			this.tabs.ResumeLayout(false);
			this.tabproperties.ResumeLayout(false);
			this.settingsGroup.ResumeLayout(false);
			this.settingsGroup.PerformLayout();
			this.idgroup.ResumeLayout(false);
			this.tabsidedefs.ResumeLayout(false);
			this.splitter.Panel1.ResumeLayout(false);
			this.splitter.Panel1.PerformLayout();
			this.splitter.Panel2.ResumeLayout(false);
			this.splitter.Panel2.PerformLayout();
			this.splitter.ResumeLayout(false);
			this.frontgroup.ResumeLayout(false);
			this.frontgroup.PerformLayout();
			this.udmfPropertiesFront.ResumeLayout(false);
			this.tabPage1.ResumeLayout(false);
			this.tabPage2.ResumeLayout(false);
			this.tabPage2.PerformLayout();
			this.backgroup.ResumeLayout(false);
			this.backgroup.PerformLayout();
			this.udmfPropertiesBack.ResumeLayout(false);
			this.tabPage5.ResumeLayout(false);
			this.tabPage6.ResumeLayout(false);
			this.tabPage6.PerformLayout();
			this.tabcustom.ResumeLayout(false);
			this.tabPage3.ResumeLayout(false);
			this.tabPage4.ResumeLayout(false);
			this.tabPage4.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button cancel;
		private System.Windows.Forms.Button apply;
		private System.Windows.Forms.GroupBox actiongroup;
		private System.Windows.Forms.GroupBox flagsgroup;
		private CodeImp.DoomBuilder.Controls.CheckboxArrayControl flags;
		private System.Windows.Forms.CheckBox checkBox1;
		private System.Windows.Forms.Button browseaction;
		private CodeImp.DoomBuilder.Controls.ActionSelectorControl action;
		private System.Windows.Forms.TabControl tabs;
		private System.Windows.Forms.TabPage tabproperties;
		private System.Windows.Forms.TabPage tabsidedefs;
		private System.Windows.Forms.GroupBox frontgroup;
		private System.Windows.Forms.CheckBox frontside;
		private System.Windows.Forms.CheckBox backside;
		private System.Windows.Forms.GroupBox backgroup;
		private CodeImp.DoomBuilder.Controls.TextureSelectorControl frontlow;
		private CodeImp.DoomBuilder.Controls.TextureSelectorControl frontmid;
		private CodeImp.DoomBuilder.Controls.TextureSelectorControl fronthigh;
		private CodeImp.DoomBuilder.Controls.TextureSelectorControl backlow;
		private CodeImp.DoomBuilder.Controls.TextureSelectorControl backmid;
		private CodeImp.DoomBuilder.Controls.TextureSelectorControl backhigh;
		private System.Windows.Forms.ComboBox activation;
		private System.Windows.Forms.Panel udmfpanel;
		private System.Windows.Forms.Panel hexenpanel;
		private System.Windows.Forms.TabPage tabcustom;
		private CodeImp.DoomBuilder.Controls.FieldsEditorControl fieldslist;
		private System.Windows.Forms.GroupBox idgroup;
		private CodeImp.DoomBuilder.Controls.CheckboxArrayControl udmfactivates;
		private System.Windows.Forms.Panel argspanel;
		private CodeImp.DoomBuilder.Controls.ArgumentBox arg2;
		private CodeImp.DoomBuilder.Controls.ArgumentBox arg1;
		private CodeImp.DoomBuilder.Controls.ArgumentBox arg0;
		private CodeImp.DoomBuilder.Controls.ArgumentBox arg3;
		private CodeImp.DoomBuilder.Controls.ArgumentBox arg4;
		private System.Windows.Forms.Label arg1label;
		private System.Windows.Forms.Label arg0label;
		private System.Windows.Forms.Label arg3label;
		private System.Windows.Forms.Label arg2label;
		private System.Windows.Forms.Label arg4label;
		private System.Windows.Forms.SplitContainer splitter;
		private System.Windows.Forms.Button customfrontbutton;
		private System.Windows.Forms.Button custombackbutton;
		private System.Windows.Forms.Panel heightpanel1;
		private System.Windows.Forms.Panel heightpanel2;
		private CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox frontsector;
		private CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox backsector;
        private System.Windows.Forms.CheckBox cbArgStr;
		private System.Windows.Forms.ComboBox arg0str;
		private CodeImp.DoomBuilder.GZBuilder.Controls.TagSelector tagSelector;
		private System.Windows.Forms.TabControl udmfPropertiesFront;
		private System.Windows.Forms.TabPage tabPage1;
		private System.Windows.Forms.TabPage tabPage2;
		private CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox lightFront;
		private System.Windows.Forms.CheckBox cblightfogFront;
		private System.Windows.Forms.CheckBox cbsmoothlightingFront;
		private System.Windows.Forms.CheckBox cbnodecalsFront;
		private System.Windows.Forms.CheckBox cbnofakecontrastFront;
		private System.Windows.Forms.CheckBox cbWrapMidtexFront;
		private System.Windows.Forms.CheckBox cbClipMidtexFront;
		private System.Windows.Forms.CheckBox cbLightAbsoluteFront;
		private CodeImp.DoomBuilder.GZBuilder.Controls.PairedFieldsControl pfcFrontOffsetTop;
		private CodeImp.DoomBuilder.GZBuilder.Controls.PairedFieldsControl pfcFrontOffsetBottom;
		private CodeImp.DoomBuilder.GZBuilder.Controls.PairedFieldsControl pfcFrontOffsetMid;
		private CodeImp.DoomBuilder.GZBuilder.Controls.PairedFieldsControl pfcFrontScaleBottom;
		private CodeImp.DoomBuilder.GZBuilder.Controls.PairedFieldsControl pfcFrontScaleMid;
		private CodeImp.DoomBuilder.GZBuilder.Controls.PairedFieldsControl pfcFrontScaleTop;
		private System.Windows.Forms.TabControl udmfPropertiesBack;
		private System.Windows.Forms.TabPage tabPage5;
		private CodeImp.DoomBuilder.GZBuilder.Controls.PairedFieldsControl pfcBackScaleBottom;
		private CodeImp.DoomBuilder.GZBuilder.Controls.PairedFieldsControl pfcBackScaleMid;
		private CodeImp.DoomBuilder.GZBuilder.Controls.PairedFieldsControl pfcBackScaleTop;
		private CodeImp.DoomBuilder.GZBuilder.Controls.PairedFieldsControl pfcBackOffsetBottom;
		private CodeImp.DoomBuilder.GZBuilder.Controls.PairedFieldsControl pfcBackOffsetMid;
		private CodeImp.DoomBuilder.GZBuilder.Controls.PairedFieldsControl pfcBackOffsetTop;
		private System.Windows.Forms.TabPage tabPage6;
		private System.Windows.Forms.CheckBox cblightfogBack;
		private System.Windows.Forms.CheckBox cbsmoothlightingBack;
		private System.Windows.Forms.CheckBox cbnodecalsBack;
		private System.Windows.Forms.CheckBox cbnofakecontrastBack;
		private System.Windows.Forms.CheckBox cbWrapMidtexBack;
		private System.Windows.Forms.CheckBox cbClipMidtexBack;
		private System.Windows.Forms.TabPage tabPage3;
		private CodeImp.DoomBuilder.GZBuilder.Controls.PairedFieldsControl pairedFieldsControl1;
		private CodeImp.DoomBuilder.GZBuilder.Controls.PairedFieldsControl pairedFieldsControl2;
		private CodeImp.DoomBuilder.GZBuilder.Controls.PairedFieldsControl pairedFieldsControl3;
		private CodeImp.DoomBuilder.GZBuilder.Controls.PairedFieldsControl pairedFieldsControl4;
		private CodeImp.DoomBuilder.GZBuilder.Controls.PairedFieldsControl pairedFieldsControl5;
		private CodeImp.DoomBuilder.GZBuilder.Controls.PairedFieldsControl pairedFieldsControl6;
		private System.Windows.Forms.TabPage tabPage4;
		private System.Windows.Forms.CheckBox checkBox2;
		private CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox buttonsNumericTextbox1;
		private System.Windows.Forms.CheckBox checkBox3;
		private System.Windows.Forms.CheckBox checkBox4;
		private System.Windows.Forms.CheckBox checkBox5;
		private System.Windows.Forms.CheckBox checkBox6;
		private System.Windows.Forms.CheckBox checkBox7;
		private System.Windows.Forms.CheckBox checkBox8;
		private System.Windows.Forms.GroupBox settingsGroup;
		private System.Windows.Forms.ComboBox cbRenderStyle;
		private CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox lockNumber;
		private System.Windows.Forms.Label labelLightFront;
		private System.Windows.Forms.CheckBox cbLightAbsoluteBack;
		private System.Windows.Forms.Label labelLightBack;
		private CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox lightBack;
		private CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox alpha;
		private CodeImp.DoomBuilder.GZBuilder.Controls.PairedIntControl frontTextureOffset;
		private CodeImp.DoomBuilder.GZBuilder.Controls.PairedIntControl backTextureOffset;
	}
}