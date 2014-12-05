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
			this.components = new System.ComponentModel.Container();
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
			this.cancel = new System.Windows.Forms.Button();
			this.apply = new System.Windows.Forms.Button();
			this.actiongroup = new System.Windows.Forms.GroupBox();
			this.actionhelp = new CodeImp.DoomBuilder.Controls.ActionSpecialHelpButton();
			this.argspanel = new System.Windows.Forms.Panel();
			this.scriptNumbers = new System.Windows.Forms.ComboBox();
			this.arg2 = new CodeImp.DoomBuilder.Controls.ArgumentBox();
			this.arg1 = new CodeImp.DoomBuilder.Controls.ArgumentBox();
			this.arg0 = new CodeImp.DoomBuilder.Controls.ArgumentBox();
			this.arg3 = new CodeImp.DoomBuilder.Controls.ArgumentBox();
			this.arg4 = new CodeImp.DoomBuilder.Controls.ArgumentBox();
			this.arg1label = new System.Windows.Forms.Label();
			this.arg3label = new System.Windows.Forms.Label();
			this.arg2label = new System.Windows.Forms.Label();
			this.arg4label = new System.Windows.Forms.Label();
			this.arg0label = new System.Windows.Forms.Label();
			this.hexenpanel = new System.Windows.Forms.Panel();
			this.activation = new System.Windows.Forms.ComboBox();
			this.action = new CodeImp.DoomBuilder.Controls.ActionSelectorControl();
			this.browseaction = new System.Windows.Forms.Button();
			this.flagsgroup = new System.Windows.Forms.GroupBox();
			this.flags = new CodeImp.DoomBuilder.Controls.CheckboxArrayControl();
			this.idgroup = new System.Windows.Forms.GroupBox();
			this.tagSelector = new CodeImp.DoomBuilder.GZBuilder.Controls.TagSelector();
			this.frontside = new System.Windows.Forms.CheckBox();
			this.frontgroup = new System.Windows.Forms.GroupBox();
			this.frontsector = new CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox();
			this.frontlow = new CodeImp.DoomBuilder.Controls.TextureSelectorControl();
			this.frontmid = new CodeImp.DoomBuilder.Controls.TextureSelectorControl();
			this.fronthigh = new CodeImp.DoomBuilder.Controls.TextureSelectorControl();
			this.frontTextureOffset = new CodeImp.DoomBuilder.GZBuilder.Controls.PairedIntControl();
			this.backside = new System.Windows.Forms.CheckBox();
			this.backgroup = new System.Windows.Forms.GroupBox();
			this.backsector = new CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox();
			this.backlow = new CodeImp.DoomBuilder.Controls.TextureSelectorControl();
			this.backmid = new CodeImp.DoomBuilder.Controls.TextureSelectorControl();
			this.backhigh = new CodeImp.DoomBuilder.Controls.TextureSelectorControl();
			this.backTextureOffset = new CodeImp.DoomBuilder.GZBuilder.Controls.PairedIntControl();
			this.panel1 = new System.Windows.Forms.Panel();
			this.tooltip = new System.Windows.Forms.ToolTip(this.components);
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
			this.actiongroup.SuspendLayout();
			this.argspanel.SuspendLayout();
			this.hexenpanel.SuspendLayout();
			this.flagsgroup.SuspendLayout();
			this.idgroup.SuspendLayout();
			this.frontgroup.SuspendLayout();
			this.backgroup.SuspendLayout();
			this.panel1.SuspendLayout();
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
			label3.Location = new System.Drawing.Point(255, 18);
			label3.Name = "label3";
			label3.Size = new System.Drawing.Size(83, 16);
			label3.TabIndex = 3;
			label3.Text = "Upper";
			label3.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// label4
			// 
			label4.Location = new System.Drawing.Point(346, 18);
			label4.Name = "label4";
			label4.Size = new System.Drawing.Size(83, 16);
			label4.TabIndex = 4;
			label4.Text = "Middle";
			label4.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// label5
			// 
			label5.Location = new System.Drawing.Point(437, 18);
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
			// cancel
			// 
			this.cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancel.Location = new System.Drawing.Point(335, 764);
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
			this.apply.Location = new System.Drawing.Point(453, 764);
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
			this.actiongroup.Controls.Add(this.actionhelp);
			this.actiongroup.Controls.Add(this.argspanel);
			this.actiongroup.Controls.Add(this.hexenpanel);
			this.actiongroup.Controls.Add(label2);
			this.actiongroup.Controls.Add(this.action);
			this.actiongroup.Controls.Add(this.browseaction);
			this.actiongroup.Location = new System.Drawing.Point(6, 462);
			this.actiongroup.Name = "actiongroup";
			this.actiongroup.Size = new System.Drawing.Size(541, 190);
			this.actiongroup.TabIndex = 1;
			this.actiongroup.TabStop = false;
			this.actiongroup.Text = " Action ";
			// 
			// actionhelp
			// 
			this.actionhelp.Location = new System.Drawing.Point(497, 25);
			this.actionhelp.Name = "actionhelp";
			this.actionhelp.Size = new System.Drawing.Size(28, 25);
			this.actionhelp.TabIndex = 11;
			// 
			// argspanel
			// 
			this.argspanel.Controls.Add(this.scriptNumbers);
			this.argspanel.Controls.Add(this.arg2);
			this.argspanel.Controls.Add(this.arg1);
			this.argspanel.Controls.Add(this.arg0);
			this.argspanel.Controls.Add(this.arg3);
			this.argspanel.Controls.Add(this.arg4);
			this.argspanel.Controls.Add(this.arg1label);
			this.argspanel.Controls.Add(this.arg3label);
			this.argspanel.Controls.Add(this.arg2label);
			this.argspanel.Controls.Add(this.arg4label);
			this.argspanel.Controls.Add(this.arg0label);
			this.argspanel.Location = new System.Drawing.Point(6, 54);
			this.argspanel.Name = "argspanel";
			this.argspanel.Size = new System.Drawing.Size(521, 83);
			this.argspanel.TabIndex = 2;
			this.argspanel.Visible = false;
			// 
			// scriptNumbers
			// 
			this.scriptNumbers.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
			this.scriptNumbers.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
			this.scriptNumbers.BackColor = System.Drawing.Color.LemonChiffon;
			this.scriptNumbers.FormattingEnabled = true;
			this.scriptNumbers.Location = new System.Drawing.Point(398, 57);
			this.scriptNumbers.Name = "scriptNumbers";
			this.scriptNumbers.Size = new System.Drawing.Size(120, 22);
			this.scriptNumbers.TabIndex = 39;
			// 
			// arg2
			// 
			this.arg2.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.arg2.Location = new System.Drawing.Point(153, 55);
			this.arg2.Name = "arg2";
			this.arg2.Size = new System.Drawing.Size(120, 24);
			this.arg2.TabIndex = 2;
			// 
			// arg1
			// 
			this.arg1.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.arg1.Location = new System.Drawing.Point(153, 29);
			this.arg1.Name = "arg1";
			this.arg1.Size = new System.Drawing.Size(120, 24);
			this.arg1.TabIndex = 1;
			// 
			// arg0
			// 
			this.arg0.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.arg0.Location = new System.Drawing.Point(153, 3);
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
			this.arg1label.Location = new System.Drawing.Point(-32, 34);
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
			this.arg2label.Location = new System.Drawing.Point(-32, 60);
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
			// arg0label
			// 
			this.arg0label.Location = new System.Drawing.Point(-32, 8);
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
			this.hexenpanel.Size = new System.Drawing.Size(521, 44);
			this.hexenpanel.TabIndex = 3;
			this.hexenpanel.Visible = false;
			// 
			// activation
			// 
			this.activation.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.activation.FormattingEnabled = true;
			this.activation.Location = new System.Drawing.Point(56, 13);
			this.activation.Name = "activation";
			this.activation.Size = new System.Drawing.Size(428, 22);
			this.activation.TabIndex = 0;
			// 
			// action
			// 
			this.action.BackColor = System.Drawing.Color.Transparent;
			this.action.Cursor = System.Windows.Forms.Cursors.Default;
			this.action.Empty = false;
			this.action.GeneralizedCategories = null;
			this.action.GeneralizedOptions = null;
			this.action.Location = new System.Drawing.Point(62, 27);
			this.action.Name = "action";
			this.action.Size = new System.Drawing.Size(402, 21);
			this.action.TabIndex = 0;
			this.action.Value = 402;
			this.action.ValueChanges += new System.EventHandler(this.action_ValueChanges);
			// 
			// browseaction
			// 
			this.browseaction.Image = global::CodeImp.DoomBuilder.Properties.Resources.List;
			this.browseaction.Location = new System.Drawing.Point(467, 25);
			this.browseaction.Name = "browseaction";
			this.browseaction.Size = new System.Drawing.Size(28, 25);
			this.browseaction.TabIndex = 1;
			this.browseaction.Text = " ";
			this.tooltip.SetToolTip(this.browseaction, "Browse Action");
			this.browseaction.UseVisualStyleBackColor = true;
			this.browseaction.Click += new System.EventHandler(this.browseaction_Click);
			// 
			// flagsgroup
			// 
			this.flagsgroup.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.flagsgroup.Controls.Add(this.flags);
			this.flagsgroup.Location = new System.Drawing.Point(6, 338);
			this.flagsgroup.Name = "flagsgroup";
			this.flagsgroup.Size = new System.Drawing.Size(541, 118);
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
			this.flags.Size = new System.Drawing.Size(517, 96);
			this.flags.TabIndex = 0;
			this.flags.VerticalSpacing = 1;
			this.flags.OnValueChanged += new System.EventHandler(this.flags_OnValueChanged);
			// 
			// idgroup
			// 
			this.idgroup.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.idgroup.Controls.Add(this.tagSelector);
			this.idgroup.Location = new System.Drawing.Point(6, 658);
			this.idgroup.Margin = new System.Windows.Forms.Padding(3, 3, 3, 6);
			this.idgroup.Name = "idgroup";
			this.idgroup.Size = new System.Drawing.Size(541, 58);
			this.idgroup.TabIndex = 2;
			this.idgroup.TabStop = false;
			this.idgroup.Text = " Identification ";
			// 
			// tagSelector
			// 
			this.tagSelector.Location = new System.Drawing.Point(19, 19);
			this.tagSelector.Name = "tagSelector";
			this.tagSelector.Size = new System.Drawing.Size(444, 34);
			this.tagSelector.TabIndex = 0;
			// 
			// frontside
			// 
			this.frontside.AutoSize = true;
			this.frontside.Location = new System.Drawing.Point(11, -2);
			this.frontside.Name = "frontside";
			this.frontside.Size = new System.Drawing.Size(75, 18);
			this.frontside.TabIndex = 0;
			this.frontside.Text = "Front Side";
			this.frontside.UseVisualStyleBackColor = true;
			this.frontside.CheckStateChanged += new System.EventHandler(this.frontside_CheckStateChanged);
			// 
			// frontgroup
			// 
			this.frontgroup.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.frontgroup.Controls.Add(this.frontside);
			this.frontgroup.Controls.Add(this.frontsector);
			this.frontgroup.Controls.Add(label11);
			this.frontgroup.Controls.Add(this.frontlow);
			this.frontgroup.Controls.Add(this.frontmid);
			this.frontgroup.Controls.Add(this.fronthigh);
			this.frontgroup.Controls.Add(this.frontTextureOffset);
			this.frontgroup.Controls.Add(label5);
			this.frontgroup.Controls.Add(label4);
			this.frontgroup.Controls.Add(label3);
			this.frontgroup.Enabled = false;
			this.frontgroup.Location = new System.Drawing.Point(6, 6);
			this.frontgroup.Name = "frontgroup";
			this.frontgroup.Size = new System.Drawing.Size(541, 160);
			this.frontgroup.TabIndex = 1;
			this.frontgroup.TabStop = false;
			this.frontgroup.Text = "     ";
			// 
			// frontsector
			// 
			this.frontsector.AllowDecimal = false;
			this.frontsector.AllowNegative = false;
			this.frontsector.AllowRelative = false;
			this.frontsector.ButtonStep = 1;
			this.frontsector.ButtonStepFloat = 1F;
			this.frontsector.ButtonStepsWrapAround = false;
			this.frontsector.Location = new System.Drawing.Point(90, 35);
			this.frontsector.Name = "frontsector";
			this.frontsector.Size = new System.Drawing.Size(130, 24);
			this.frontsector.StepValues = null;
			this.frontsector.TabIndex = 14;
			// 
			// frontlow
			// 
			this.frontlow.Location = new System.Drawing.Point(437, 37);
			this.frontlow.MultipleTextures = false;
			this.frontlow.Name = "frontlow";
			this.frontlow.Required = false;
			this.frontlow.Size = new System.Drawing.Size(83, 112);
			this.frontlow.TabIndex = 6;
			this.frontlow.TextureName = "";
			this.frontlow.UsePreviews = true;
			this.frontlow.OnValueChanged += new System.EventHandler(this.frontlow_OnValueChanged);
			// 
			// frontmid
			// 
			this.frontmid.Location = new System.Drawing.Point(346, 37);
			this.frontmid.MultipleTextures = false;
			this.frontmid.Name = "frontmid";
			this.frontmid.Required = false;
			this.frontmid.Size = new System.Drawing.Size(83, 112);
			this.frontmid.TabIndex = 5;
			this.frontmid.TextureName = "";
			this.frontmid.UsePreviews = true;
			this.frontmid.OnValueChanged += new System.EventHandler(this.frontmid_OnValueChanged);
			// 
			// fronthigh
			// 
			this.fronthigh.Location = new System.Drawing.Point(255, 37);
			this.fronthigh.MultipleTextures = false;
			this.fronthigh.Name = "fronthigh";
			this.fronthigh.Required = false;
			this.fronthigh.Size = new System.Drawing.Size(83, 112);
			this.fronthigh.TabIndex = 4;
			this.fronthigh.TextureName = "";
			this.fronthigh.UsePreviews = true;
			this.fronthigh.OnValueChanged += new System.EventHandler(this.fronthigh_OnValueChanged);
			// 
			// frontTextureOffset
			// 
			this.frontTextureOffset.ButtonStep = 16;
			this.frontTextureOffset.DefaultValue = 0;
			this.frontTextureOffset.Label = "Texture Offset:";
			this.frontTextureOffset.Location = new System.Drawing.Point(3, 65);
			this.frontTextureOffset.Name = "frontTextureOffset";
			this.frontTextureOffset.Size = new System.Drawing.Size(268, 26);
			this.frontTextureOffset.TabIndex = 41;
			this.frontTextureOffset.OnValuesChanged += new System.EventHandler(this.frontTextureOffset_OnValuesChanged);
			// 
			// backside
			// 
			this.backside.AutoSize = true;
			this.backside.Location = new System.Drawing.Point(11, -2);
			this.backside.Name = "backside";
			this.backside.Size = new System.Drawing.Size(74, 18);
			this.backside.TabIndex = 0;
			this.backside.Text = "Back Side";
			this.backside.UseVisualStyleBackColor = true;
			this.backside.CheckStateChanged += new System.EventHandler(this.backside_CheckStateChanged);
			// 
			// backgroup
			// 
			this.backgroup.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.backgroup.Controls.Add(this.backside);
			this.backgroup.Controls.Add(this.backsector);
			this.backgroup.Controls.Add(label12);
			this.backgroup.Controls.Add(this.backlow);
			this.backgroup.Controls.Add(this.backmid);
			this.backgroup.Controls.Add(this.backhigh);
			this.backgroup.Controls.Add(this.backTextureOffset);
			this.backgroup.Controls.Add(label8);
			this.backgroup.Controls.Add(label9);
			this.backgroup.Controls.Add(label10);
			this.backgroup.Enabled = false;
			this.backgroup.Location = new System.Drawing.Point(6, 172);
			this.backgroup.Name = "backgroup";
			this.backgroup.Size = new System.Drawing.Size(541, 160);
			this.backgroup.TabIndex = 1;
			this.backgroup.TabStop = false;
			this.backgroup.Text = "     ";
			// 
			// backsector
			// 
			this.backsector.AllowDecimal = false;
			this.backsector.AllowNegative = false;
			this.backsector.AllowRelative = false;
			this.backsector.ButtonStep = 1;
			this.backsector.ButtonStepFloat = 1F;
			this.backsector.ButtonStepsWrapAround = false;
			this.backsector.Location = new System.Drawing.Point(90, 35);
			this.backsector.Name = "backsector";
			this.backsector.Size = new System.Drawing.Size(130, 24);
			this.backsector.StepValues = null;
			this.backsector.TabIndex = 17;
			// 
			// backlow
			// 
			this.backlow.Location = new System.Drawing.Point(437, 37);
			this.backlow.MultipleTextures = false;
			this.backlow.Name = "backlow";
			this.backlow.Required = false;
			this.backlow.Size = new System.Drawing.Size(83, 112);
			this.backlow.TabIndex = 6;
			this.backlow.TextureName = "";
			this.backlow.UsePreviews = true;
			this.backlow.OnValueChanged += new System.EventHandler(this.backlow_OnValueChanged);
			// 
			// backmid
			// 
			this.backmid.Location = new System.Drawing.Point(346, 37);
			this.backmid.MultipleTextures = false;
			this.backmid.Name = "backmid";
			this.backmid.Required = false;
			this.backmid.Size = new System.Drawing.Size(83, 112);
			this.backmid.TabIndex = 5;
			this.backmid.TextureName = "";
			this.backmid.UsePreviews = true;
			this.backmid.OnValueChanged += new System.EventHandler(this.backmid_OnValueChanged);
			// 
			// backhigh
			// 
			this.backhigh.Location = new System.Drawing.Point(255, 37);
			this.backhigh.MultipleTextures = false;
			this.backhigh.Name = "backhigh";
			this.backhigh.Required = false;
			this.backhigh.Size = new System.Drawing.Size(83, 112);
			this.backhigh.TabIndex = 4;
			this.backhigh.TextureName = "";
			this.backhigh.UsePreviews = true;
			this.backhigh.OnValueChanged += new System.EventHandler(this.backhigh_OnValueChanged);
			// 
			// backTextureOffset
			// 
			this.backTextureOffset.ButtonStep = 16;
			this.backTextureOffset.DefaultValue = 0;
			this.backTextureOffset.Label = "Texture Offset:";
			this.backTextureOffset.Location = new System.Drawing.Point(3, 65);
			this.backTextureOffset.Name = "backTextureOffset";
			this.backTextureOffset.Size = new System.Drawing.Size(268, 28);
			this.backTextureOffset.TabIndex = 42;
			this.backTextureOffset.OnValuesChanged += new System.EventHandler(this.backTextureOffset_OnValuesChanged);
			// 
			// panel1
			// 
			this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.panel1.BackColor = System.Drawing.SystemColors.Window;
			this.panel1.Controls.Add(this.frontgroup);
			this.panel1.Controls.Add(this.backgroup);
			this.panel1.Controls.Add(this.flagsgroup);
			this.panel1.Controls.Add(this.actiongroup);
			this.panel1.Controls.Add(this.idgroup);
			this.panel1.Location = new System.Drawing.Point(12, 12);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(553, 746);
			this.panel1.TabIndex = 5;
			// 
			// LinedefEditForm
			// 
			this.AcceptButton = this.apply;
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.CancelButton = this.cancel;
			this.ClientSize = new System.Drawing.Size(577, 796);
			this.Controls.Add(this.cancel);
			this.Controls.Add(this.apply);
			this.Controls.Add(this.panel1);
			this.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "LinedefEditForm";
			this.Opacity = 1;
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Edit Linedef";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.LinedefEditForm_FormClosing);
			this.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.LinedefEditForm_HelpRequested);
			this.actiongroup.ResumeLayout(false);
			this.actiongroup.PerformLayout();
			this.argspanel.ResumeLayout(false);
			this.hexenpanel.ResumeLayout(false);
			this.hexenpanel.PerformLayout();
			this.flagsgroup.ResumeLayout(false);
			this.idgroup.ResumeLayout(false);
			this.frontgroup.ResumeLayout(false);
			this.frontgroup.PerformLayout();
			this.backgroup.ResumeLayout(false);
			this.backgroup.PerformLayout();
			this.panel1.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button cancel;
		private System.Windows.Forms.Button apply;
		private System.Windows.Forms.GroupBox actiongroup;
		private System.Windows.Forms.GroupBox flagsgroup;
		private CodeImp.DoomBuilder.Controls.CheckboxArrayControl flags;
		private System.Windows.Forms.Button browseaction;
		private CodeImp.DoomBuilder.Controls.ActionSelectorControl action;
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
		private System.Windows.Forms.Panel hexenpanel;
		private System.Windows.Forms.GroupBox idgroup;
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
		private CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox frontsector;
		private CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox backsector;
		private CodeImp.DoomBuilder.GZBuilder.Controls.TagSelector tagSelector;
		private CodeImp.DoomBuilder.GZBuilder.Controls.PairedIntControl frontTextureOffset;
		private CodeImp.DoomBuilder.GZBuilder.Controls.PairedIntControl backTextureOffset;
		private System.Windows.Forms.ComboBox scriptNumbers;
		private System.Windows.Forms.Panel panel1;
		private CodeImp.DoomBuilder.Controls.ActionSpecialHelpButton actionhelp;
		private System.Windows.Forms.ToolTip tooltip;
	}
}