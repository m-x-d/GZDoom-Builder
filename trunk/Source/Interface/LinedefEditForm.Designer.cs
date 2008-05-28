namespace CodeImp.DoomBuilder.Interface
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
			System.Windows.Forms.Label taglabel;
			System.Windows.Forms.Label label3;
			System.Windows.Forms.Label label4;
			System.Windows.Forms.Label label5;
			System.Windows.Forms.Label label6;
			System.Windows.Forms.Label label7;
			System.Windows.Forms.Label label8;
			System.Windows.Forms.Label label9;
			System.Windows.Forms.Label label10;
			System.Windows.Forms.Label label11;
			System.Windows.Forms.Label label12;
			System.Windows.Forms.Label activationlabel;
			this.arg0label = new System.Windows.Forms.Label();
			this.arg1label = new System.Windows.Forms.Label();
			this.arg4label = new System.Windows.Forms.Label();
			this.arg2label = new System.Windows.Forms.Label();
			this.arg3label = new System.Windows.Forms.Label();
			this.cancel = new System.Windows.Forms.Button();
			this.apply = new System.Windows.Forms.Button();
			this.actiongroup = new System.Windows.Forms.GroupBox();
			this.hexenpanel = new System.Windows.Forms.Panel();
			this.arg4 = new CodeImp.DoomBuilder.Interface.ArgumentBox();
			this.activation = new System.Windows.Forms.ComboBox();
			this.action = new CodeImp.DoomBuilder.Interface.ActionSelectorControl();
			this.browseaction = new System.Windows.Forms.Button();
			this.doompanel = new System.Windows.Forms.Panel();
			this.tag = new CodeImp.DoomBuilder.Interface.NumericTextbox();
			this.newtag = new System.Windows.Forms.Button();
			this.settingsgroup = new System.Windows.Forms.GroupBox();
			this.flags = new CodeImp.DoomBuilder.Interface.CheckboxArrayControl();
			this.checkBox1 = new System.Windows.Forms.CheckBox();
			this.tabs = new System.Windows.Forms.TabControl();
			this.tabproperties = new System.Windows.Forms.TabPage();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.tabsidedefs = new System.Windows.Forms.TabPage();
			this.backside = new System.Windows.Forms.CheckBox();
			this.backgroup = new System.Windows.Forms.GroupBox();
			this.backsector = new CodeImp.DoomBuilder.Interface.NumericTextbox();
			this.backlow = new CodeImp.DoomBuilder.Interface.TextureSelectorControl();
			this.backmid = new CodeImp.DoomBuilder.Interface.TextureSelectorControl();
			this.backhigh = new CodeImp.DoomBuilder.Interface.TextureSelectorControl();
			this.backoffsety = new CodeImp.DoomBuilder.Interface.NumericTextbox();
			this.backoffsetx = new CodeImp.DoomBuilder.Interface.NumericTextbox();
			this.frontside = new System.Windows.Forms.CheckBox();
			this.frontgroup = new System.Windows.Forms.GroupBox();
			this.frontsector = new CodeImp.DoomBuilder.Interface.NumericTextbox();
			this.frontlow = new CodeImp.DoomBuilder.Interface.TextureSelectorControl();
			this.frontmid = new CodeImp.DoomBuilder.Interface.TextureSelectorControl();
			this.fronthigh = new CodeImp.DoomBuilder.Interface.TextureSelectorControl();
			this.frontoffsety = new CodeImp.DoomBuilder.Interface.NumericTextbox();
			this.frontoffsetx = new CodeImp.DoomBuilder.Interface.NumericTextbox();
			this.tabcustom = new System.Windows.Forms.TabPage();
			this.fieldslist = new CodeImp.DoomBuilder.Interface.FieldsEditorControl();
			this.arg3 = new CodeImp.DoomBuilder.Interface.ArgumentBox();
			this.arg0 = new CodeImp.DoomBuilder.Interface.ArgumentBox();
			this.arg1 = new CodeImp.DoomBuilder.Interface.ArgumentBox();
			this.arg2 = new CodeImp.DoomBuilder.Interface.ArgumentBox();
			label2 = new System.Windows.Forms.Label();
			taglabel = new System.Windows.Forms.Label();
			label3 = new System.Windows.Forms.Label();
			label4 = new System.Windows.Forms.Label();
			label5 = new System.Windows.Forms.Label();
			label6 = new System.Windows.Forms.Label();
			label7 = new System.Windows.Forms.Label();
			label8 = new System.Windows.Forms.Label();
			label9 = new System.Windows.Forms.Label();
			label10 = new System.Windows.Forms.Label();
			label11 = new System.Windows.Forms.Label();
			label12 = new System.Windows.Forms.Label();
			activationlabel = new System.Windows.Forms.Label();
			this.actiongroup.SuspendLayout();
			this.hexenpanel.SuspendLayout();
			this.settingsgroup.SuspendLayout();
			this.tabs.SuspendLayout();
			this.tabproperties.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.tabsidedefs.SuspendLayout();
			this.backgroup.SuspendLayout();
			this.frontgroup.SuspendLayout();
			this.tabcustom.SuspendLayout();
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
			// taglabel
			// 
			taglabel.AutoSize = true;
			taglabel.Location = new System.Drawing.Point(28, 31);
			taglabel.Name = "taglabel";
			taglabel.Size = new System.Drawing.Size(28, 14);
			taglabel.TabIndex = 6;
			taglabel.Text = "Tag:";
			// 
			// label3
			// 
			label3.Location = new System.Drawing.Point(234, 18);
			label3.Name = "label3";
			label3.Size = new System.Drawing.Size(83, 16);
			label3.TabIndex = 3;
			label3.Text = "Upper";
			label3.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// label4
			// 
			label4.Location = new System.Drawing.Point(325, 18);
			label4.Name = "label4";
			label4.Size = new System.Drawing.Size(83, 16);
			label4.TabIndex = 4;
			label4.Text = "Middle";
			label4.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// label5
			// 
			label5.Location = new System.Drawing.Point(416, 18);
			label5.Name = "label5";
			label5.Size = new System.Drawing.Size(83, 16);
			label5.TabIndex = 5;
			label5.Text = "Lower";
			label5.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// label6
			// 
			label6.AutoSize = true;
			label6.Location = new System.Drawing.Point(16, 79);
			label6.Name = "label6";
			label6.Size = new System.Drawing.Size(81, 14);
			label6.TabIndex = 7;
			label6.Text = "Texture Offset:";
			// 
			// label7
			// 
			label7.AutoSize = true;
			label7.Location = new System.Drawing.Point(16, 79);
			label7.Name = "label7";
			label7.Size = new System.Drawing.Size(81, 14);
			label7.TabIndex = 7;
			label7.Text = "Texture Offset:";
			// 
			// label8
			// 
			label8.Location = new System.Drawing.Point(416, 18);
			label8.Name = "label8";
			label8.Size = new System.Drawing.Size(83, 16);
			label8.TabIndex = 5;
			label8.Text = "Lower";
			label8.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// label9
			// 
			label9.Location = new System.Drawing.Point(325, 18);
			label9.Name = "label9";
			label9.Size = new System.Drawing.Size(83, 16);
			label9.TabIndex = 4;
			label9.Text = "Middle";
			label9.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// label10
			// 
			label10.Location = new System.Drawing.Point(234, 18);
			label10.Name = "label10";
			label10.Size = new System.Drawing.Size(83, 16);
			label10.TabIndex = 3;
			label10.Text = "Upper";
			label10.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// label11
			// 
			label11.AutoSize = true;
			label11.Location = new System.Drawing.Point(26, 40);
			label11.Name = "label11";
			label11.Size = new System.Drawing.Size(71, 14);
			label11.TabIndex = 13;
			label11.Text = "Sector Index:";
			// 
			// label12
			// 
			label12.AutoSize = true;
			label12.Location = new System.Drawing.Point(26, 40);
			label12.Name = "label12";
			label12.Size = new System.Drawing.Size(71, 14);
			label12.TabIndex = 16;
			label12.Text = "Sector Index:";
			// 
			// activationlabel
			// 
			activationlabel.AutoSize = true;
			activationlabel.Location = new System.Drawing.Point(6, 18);
			activationlabel.Name = "activationlabel";
			activationlabel.Size = new System.Drawing.Size(44, 14);
			activationlabel.TabIndex = 10;
			activationlabel.Text = "Trigger:";
			// 
			// arg0label
			// 
			this.arg0label.Location = new System.Drawing.Point(-26, 58);
			this.arg0label.Name = "arg0label";
			this.arg0label.Size = new System.Drawing.Size(179, 14);
			this.arg0label.TabIndex = 12;
			this.arg0label.Text = "Argument 1:";
			this.arg0label.TextAlign = System.Drawing.ContentAlignment.TopRight;
			this.arg0label.UseMnemonic = false;
			// 
			// arg1label
			// 
			this.arg1label.Location = new System.Drawing.Point(-26, 84);
			this.arg1label.Name = "arg1label";
			this.arg1label.Size = new System.Drawing.Size(179, 14);
			this.arg1label.TabIndex = 14;
			this.arg1label.Text = "Argument 2:";
			this.arg1label.TextAlign = System.Drawing.ContentAlignment.TopRight;
			this.arg1label.UseMnemonic = false;
			// 
			// arg4label
			// 
			this.arg4label.Location = new System.Drawing.Point(215, 84);
			this.arg4label.Name = "arg4label";
			this.arg4label.Size = new System.Drawing.Size(179, 14);
			this.arg4label.TabIndex = 16;
			this.arg4label.Text = "Argument 5:";
			this.arg4label.TextAlign = System.Drawing.ContentAlignment.TopRight;
			this.arg4label.UseMnemonic = false;
			// 
			// arg2label
			// 
			this.arg2label.Location = new System.Drawing.Point(-26, 110);
			this.arg2label.Name = "arg2label";
			this.arg2label.Size = new System.Drawing.Size(179, 14);
			this.arg2label.TabIndex = 18;
			this.arg2label.Text = "Argument 3:";
			this.arg2label.TextAlign = System.Drawing.ContentAlignment.TopRight;
			this.arg2label.UseMnemonic = false;
			// 
			// arg3label
			// 
			this.arg3label.Location = new System.Drawing.Point(215, 58);
			this.arg3label.Name = "arg3label";
			this.arg3label.Size = new System.Drawing.Size(179, 14);
			this.arg3label.TabIndex = 20;
			this.arg3label.Text = "Argument 4:";
			this.arg3label.TextAlign = System.Drawing.ContentAlignment.TopRight;
			this.arg3label.UseMnemonic = false;
			// 
			// cancel
			// 
			this.cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancel.Location = new System.Drawing.Point(439, 493);
			this.cancel.Name = "cancel";
			this.cancel.Size = new System.Drawing.Size(112, 25);
			this.cancel.TabIndex = 17;
			this.cancel.Text = "Cancel";
			this.cancel.UseVisualStyleBackColor = true;
			this.cancel.Click += new System.EventHandler(this.cancel_Click);
			// 
			// apply
			// 
			this.apply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.apply.Location = new System.Drawing.Point(320, 493);
			this.apply.Name = "apply";
			this.apply.Size = new System.Drawing.Size(112, 25);
			this.apply.TabIndex = 16;
			this.apply.Text = "OK";
			this.apply.UseVisualStyleBackColor = true;
			this.apply.Click += new System.EventHandler(this.apply_Click);
			// 
			// actiongroup
			// 
			this.actiongroup.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.actiongroup.Controls.Add(this.hexenpanel);
			this.actiongroup.Controls.Add(label2);
			this.actiongroup.Controls.Add(this.action);
			this.actiongroup.Controls.Add(this.browseaction);
			this.actiongroup.Controls.Add(this.doompanel);
			this.actiongroup.Location = new System.Drawing.Point(8, 238);
			this.actiongroup.Name = "actiongroup";
			this.actiongroup.Size = new System.Drawing.Size(517, 192);
			this.actiongroup.TabIndex = 18;
			this.actiongroup.TabStop = false;
			this.actiongroup.Text = " Action ";
			// 
			// hexenpanel
			// 
			this.hexenpanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.hexenpanel.Controls.Add(this.arg2);
			this.hexenpanel.Controls.Add(this.arg1);
			this.hexenpanel.Controls.Add(this.arg0);
			this.hexenpanel.Controls.Add(this.arg3);
			this.hexenpanel.Controls.Add(this.arg4);
			this.hexenpanel.Controls.Add(this.activation);
			this.hexenpanel.Controls.Add(activationlabel);
			this.hexenpanel.Controls.Add(this.arg1label);
			this.hexenpanel.Controls.Add(this.arg0label);
			this.hexenpanel.Controls.Add(this.arg3label);
			this.hexenpanel.Controls.Add(this.arg2label);
			this.hexenpanel.Controls.Add(this.arg4label);
			this.hexenpanel.Location = new System.Drawing.Point(6, 52);
			this.hexenpanel.Name = "hexenpanel";
			this.hexenpanel.Size = new System.Drawing.Size(505, 132);
			this.hexenpanel.TabIndex = 13;
			// 
			// arg4
			// 
			this.arg4.FormattingEnabled = true;
			this.arg4.ImeMode = System.Windows.Forms.ImeMode.Off;
			this.arg4.Location = new System.Drawing.Point(400, 81);
			this.arg4.Name = "arg4";
			this.arg4.Size = new System.Drawing.Size(93, 22);
			this.arg4.TabIndex = 22;
			// 
			// activation
			// 
			this.activation.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.activation.FormattingEnabled = true;
			this.activation.Location = new System.Drawing.Point(56, 14);
			this.activation.Name = "activation";
			this.activation.Size = new System.Drawing.Size(437, 22);
			this.activation.TabIndex = 11;
			// 
			// action
			// 
			this.action.BackColor = System.Drawing.SystemColors.Control;
			this.action.Cursor = System.Windows.Forms.Cursors.Default;
			this.action.Empty = false;
			this.action.GeneralizedCategories = null;
			this.action.Location = new System.Drawing.Point(62, 27);
			this.action.Name = "action";
			this.action.Size = new System.Drawing.Size(401, 21);
			this.action.TabIndex = 5;
			this.action.Value = 402;
			this.action.ValueChanges += new System.EventHandler(this.action_ValueChanges);
			// 
			// browseaction
			// 
			this.browseaction.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.browseaction.Image = global::CodeImp.DoomBuilder.Properties.Resources.treeview;
			this.browseaction.Location = new System.Drawing.Point(469, 26);
			this.browseaction.Name = "browseaction";
			this.browseaction.Padding = new System.Windows.Forms.Padding(0, 0, 1, 3);
			this.browseaction.Size = new System.Drawing.Size(30, 23);
			this.browseaction.TabIndex = 3;
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
			this.doompanel.Size = new System.Drawing.Size(505, 132);
			this.doompanel.TabIndex = 12;
			// 
			// tag
			// 
			this.tag.AllowNegative = false;
			this.tag.AllowRelative = true;
			this.tag.ImeMode = System.Windows.Forms.ImeMode.Off;
			this.tag.Location = new System.Drawing.Point(62, 28);
			this.tag.Name = "tag";
			this.tag.Size = new System.Drawing.Size(68, 20);
			this.tag.TabIndex = 7;
			// 
			// newtag
			// 
			this.newtag.Location = new System.Drawing.Point(136, 27);
			this.newtag.Name = "newtag";
			this.newtag.Size = new System.Drawing.Size(76, 23);
			this.newtag.TabIndex = 8;
			this.newtag.Text = "New Tag";
			this.newtag.UseVisualStyleBackColor = true;
			this.newtag.Click += new System.EventHandler(this.newtag_Click);
			// 
			// settingsgroup
			// 
			this.settingsgroup.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.settingsgroup.Controls.Add(this.flags);
			this.settingsgroup.Location = new System.Drawing.Point(8, 8);
			this.settingsgroup.Name = "settingsgroup";
			this.settingsgroup.Size = new System.Drawing.Size(517, 152);
			this.settingsgroup.TabIndex = 19;
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
			this.flags.Location = new System.Drawing.Point(18, 26);
			this.flags.Name = "flags";
			this.flags.Size = new System.Drawing.Size(493, 119);
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
			this.tabs.Size = new System.Drawing.Size(541, 466);
			this.tabs.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
			this.tabs.TabIndex = 20;
			// 
			// tabproperties
			// 
			this.tabproperties.Controls.Add(this.groupBox1);
			this.tabproperties.Controls.Add(this.settingsgroup);
			this.tabproperties.Controls.Add(this.actiongroup);
			this.tabproperties.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.tabproperties.Location = new System.Drawing.Point(4, 23);
			this.tabproperties.Name = "tabproperties";
			this.tabproperties.Padding = new System.Windows.Forms.Padding(5);
			this.tabproperties.Size = new System.Drawing.Size(533, 439);
			this.tabproperties.TabIndex = 0;
			this.tabproperties.Text = "Properties";
			this.tabproperties.UseVisualStyleBackColor = true;
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.tag);
			this.groupBox1.Controls.Add(taglabel);
			this.groupBox1.Controls.Add(this.newtag);
			this.groupBox1.Location = new System.Drawing.Point(8, 166);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(517, 66);
			this.groupBox1.TabIndex = 20;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = " Identification ";
			// 
			// tabsidedefs
			// 
			this.tabsidedefs.Controls.Add(this.backside);
			this.tabsidedefs.Controls.Add(this.backgroup);
			this.tabsidedefs.Controls.Add(this.frontside);
			this.tabsidedefs.Controls.Add(this.frontgroup);
			this.tabsidedefs.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.tabsidedefs.Location = new System.Drawing.Point(4, 23);
			this.tabsidedefs.Name = "tabsidedefs";
			this.tabsidedefs.Padding = new System.Windows.Forms.Padding(5);
			this.tabsidedefs.Size = new System.Drawing.Size(533, 439);
			this.tabsidedefs.TabIndex = 1;
			this.tabsidedefs.Text = "Sidedefs";
			this.tabsidedefs.UseVisualStyleBackColor = true;
			// 
			// backside
			// 
			this.backside.AutoSize = true;
			this.backside.Location = new System.Drawing.Point(20, 212);
			this.backside.Name = "backside";
			this.backside.Size = new System.Drawing.Size(74, 18);
			this.backside.TabIndex = 2;
			this.backside.Text = "Back Side";
			this.backside.UseVisualStyleBackColor = true;
			this.backside.CheckStateChanged += new System.EventHandler(this.backside_CheckStateChanged);
			// 
			// backgroup
			// 
			this.backgroup.Controls.Add(this.backsector);
			this.backgroup.Controls.Add(label12);
			this.backgroup.Controls.Add(this.backlow);
			this.backgroup.Controls.Add(this.backmid);
			this.backgroup.Controls.Add(this.backhigh);
			this.backgroup.Controls.Add(this.backoffsety);
			this.backgroup.Controls.Add(this.backoffsetx);
			this.backgroup.Controls.Add(label7);
			this.backgroup.Controls.Add(label8);
			this.backgroup.Controls.Add(label9);
			this.backgroup.Controls.Add(label10);
			this.backgroup.Enabled = false;
			this.backgroup.Location = new System.Drawing.Point(8, 214);
			this.backgroup.Name = "backgroup";
			this.backgroup.Size = new System.Drawing.Size(517, 208);
			this.backgroup.TabIndex = 1;
			this.backgroup.TabStop = false;
			this.backgroup.Text = "     ";
			// 
			// backsector
			// 
			this.backsector.AllowNegative = false;
			this.backsector.AllowRelative = false;
			this.backsector.ImeMode = System.Windows.Forms.ImeMode.Off;
			this.backsector.Location = new System.Drawing.Point(103, 37);
			this.backsector.Name = "backsector";
			this.backsector.Size = new System.Drawing.Size(94, 20);
			this.backsector.TabIndex = 17;
			this.backsector.Enter += new System.EventHandler(this.SelectAllText);
			// 
			// backlow
			// 
			this.backlow.Location = new System.Drawing.Point(416, 37);
			this.backlow.Name = "backlow";
			this.backlow.Required = false;
			this.backlow.Size = new System.Drawing.Size(83, 120);
			this.backlow.TabIndex = 15;
			this.backlow.TextureName = "";
			// 
			// backmid
			// 
			this.backmid.Location = new System.Drawing.Point(325, 37);
			this.backmid.Name = "backmid";
			this.backmid.Required = false;
			this.backmid.Size = new System.Drawing.Size(83, 120);
			this.backmid.TabIndex = 14;
			this.backmid.TextureName = "";
			// 
			// backhigh
			// 
			this.backhigh.Location = new System.Drawing.Point(234, 37);
			this.backhigh.Name = "backhigh";
			this.backhigh.Required = false;
			this.backhigh.Size = new System.Drawing.Size(83, 120);
			this.backhigh.TabIndex = 13;
			this.backhigh.TextureName = "";
			// 
			// backoffsety
			// 
			this.backoffsety.AllowNegative = true;
			this.backoffsety.AllowRelative = true;
			this.backoffsety.ImeMode = System.Windows.Forms.ImeMode.Off;
			this.backoffsety.Location = new System.Drawing.Point(152, 76);
			this.backoffsety.Name = "backoffsety";
			this.backoffsety.Size = new System.Drawing.Size(45, 20);
			this.backoffsety.TabIndex = 9;
			this.backoffsety.Enter += new System.EventHandler(this.SelectAllText);
			// 
			// backoffsetx
			// 
			this.backoffsetx.AllowNegative = true;
			this.backoffsetx.AllowRelative = true;
			this.backoffsetx.ImeMode = System.Windows.Forms.ImeMode.Off;
			this.backoffsetx.Location = new System.Drawing.Point(103, 76);
			this.backoffsetx.Name = "backoffsetx";
			this.backoffsetx.Size = new System.Drawing.Size(45, 20);
			this.backoffsetx.TabIndex = 8;
			this.backoffsetx.Enter += new System.EventHandler(this.SelectAllText);
			// 
			// frontside
			// 
			this.frontside.AutoSize = true;
			this.frontside.Location = new System.Drawing.Point(20, 6);
			this.frontside.Name = "frontside";
			this.frontside.Size = new System.Drawing.Size(75, 18);
			this.frontside.TabIndex = 0;
			this.frontside.Text = "Front Side";
			this.frontside.UseVisualStyleBackColor = true;
			this.frontside.CheckStateChanged += new System.EventHandler(this.frontside_CheckStateChanged);
			// 
			// frontgroup
			// 
			this.frontgroup.Controls.Add(this.frontsector);
			this.frontgroup.Controls.Add(label11);
			this.frontgroup.Controls.Add(this.frontlow);
			this.frontgroup.Controls.Add(this.frontmid);
			this.frontgroup.Controls.Add(this.fronthigh);
			this.frontgroup.Controls.Add(this.frontoffsety);
			this.frontgroup.Controls.Add(this.frontoffsetx);
			this.frontgroup.Controls.Add(label6);
			this.frontgroup.Controls.Add(label5);
			this.frontgroup.Controls.Add(label4);
			this.frontgroup.Controls.Add(label3);
			this.frontgroup.Enabled = false;
			this.frontgroup.Location = new System.Drawing.Point(8, 8);
			this.frontgroup.Name = "frontgroup";
			this.frontgroup.Size = new System.Drawing.Size(517, 200);
			this.frontgroup.TabIndex = 0;
			this.frontgroup.TabStop = false;
			this.frontgroup.Text = "     ";
			// 
			// frontsector
			// 
			this.frontsector.AllowNegative = false;
			this.frontsector.AllowRelative = false;
			this.frontsector.ImeMode = System.Windows.Forms.ImeMode.Off;
			this.frontsector.Location = new System.Drawing.Point(103, 37);
			this.frontsector.Name = "frontsector";
			this.frontsector.Size = new System.Drawing.Size(94, 20);
			this.frontsector.TabIndex = 14;
			this.frontsector.Enter += new System.EventHandler(this.SelectAllText);
			// 
			// frontlow
			// 
			this.frontlow.Location = new System.Drawing.Point(416, 37);
			this.frontlow.Name = "frontlow";
			this.frontlow.Required = false;
			this.frontlow.Size = new System.Drawing.Size(83, 120);
			this.frontlow.TabIndex = 12;
			this.frontlow.TextureName = "";
			// 
			// frontmid
			// 
			this.frontmid.Location = new System.Drawing.Point(325, 37);
			this.frontmid.Name = "frontmid";
			this.frontmid.Required = false;
			this.frontmid.Size = new System.Drawing.Size(83, 120);
			this.frontmid.TabIndex = 11;
			this.frontmid.TextureName = "";
			// 
			// fronthigh
			// 
			this.fronthigh.Location = new System.Drawing.Point(234, 37);
			this.fronthigh.Name = "fronthigh";
			this.fronthigh.Required = false;
			this.fronthigh.Size = new System.Drawing.Size(83, 120);
			this.fronthigh.TabIndex = 10;
			this.fronthigh.TextureName = "";
			// 
			// frontoffsety
			// 
			this.frontoffsety.AllowNegative = true;
			this.frontoffsety.AllowRelative = true;
			this.frontoffsety.ImeMode = System.Windows.Forms.ImeMode.Off;
			this.frontoffsety.Location = new System.Drawing.Point(152, 76);
			this.frontoffsety.Name = "frontoffsety";
			this.frontoffsety.Size = new System.Drawing.Size(45, 20);
			this.frontoffsety.TabIndex = 9;
			this.frontoffsety.Enter += new System.EventHandler(this.SelectAllText);
			// 
			// frontoffsetx
			// 
			this.frontoffsetx.AllowNegative = true;
			this.frontoffsetx.AllowRelative = true;
			this.frontoffsetx.ImeMode = System.Windows.Forms.ImeMode.Off;
			this.frontoffsetx.Location = new System.Drawing.Point(103, 76);
			this.frontoffsetx.Name = "frontoffsetx";
			this.frontoffsetx.Size = new System.Drawing.Size(45, 20);
			this.frontoffsetx.TabIndex = 8;
			this.frontoffsetx.Enter += new System.EventHandler(this.SelectAllText);
			// 
			// tabcustom
			// 
			this.tabcustom.Controls.Add(this.fieldslist);
			this.tabcustom.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.tabcustom.Location = new System.Drawing.Point(4, 23);
			this.tabcustom.Name = "tabcustom";
			this.tabcustom.Padding = new System.Windows.Forms.Padding(3);
			this.tabcustom.Size = new System.Drawing.Size(533, 439);
			this.tabcustom.TabIndex = 2;
			this.tabcustom.Text = "Custom";
			this.tabcustom.UseVisualStyleBackColor = true;
			// 
			// fieldslist
			// 
			this.fieldslist.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.fieldslist.Location = new System.Drawing.Point(11, 11);
			this.fieldslist.Margin = new System.Windows.Forms.Padding(8);
			this.fieldslist.Name = "fieldslist";
			this.fieldslist.Size = new System.Drawing.Size(511, 417);
			this.fieldslist.TabIndex = 0;
			// 
			// arg3
			// 
			this.arg3.FormattingEnabled = true;
			this.arg3.ImeMode = System.Windows.Forms.ImeMode.Off;
			this.arg3.Location = new System.Drawing.Point(400, 55);
			this.arg3.Name = "arg3";
			this.arg3.Size = new System.Drawing.Size(93, 22);
			this.arg3.TabIndex = 23;
			// 
			// arg0
			// 
			this.arg0.FormattingEnabled = true;
			this.arg0.ImeMode = System.Windows.Forms.ImeMode.Off;
			this.arg0.Location = new System.Drawing.Point(159, 55);
			this.arg0.Name = "arg0";
			this.arg0.Size = new System.Drawing.Size(93, 22);
			this.arg0.TabIndex = 24;
			// 
			// arg1
			// 
			this.arg1.FormattingEnabled = true;
			this.arg1.ImeMode = System.Windows.Forms.ImeMode.Off;
			this.arg1.Location = new System.Drawing.Point(159, 81);
			this.arg1.Name = "arg1";
			this.arg1.Size = new System.Drawing.Size(93, 22);
			this.arg1.TabIndex = 25;
			// 
			// arg2
			// 
			this.arg2.FormattingEnabled = true;
			this.arg2.ImeMode = System.Windows.Forms.ImeMode.Off;
			this.arg2.Location = new System.Drawing.Point(159, 107);
			this.arg2.Name = "arg2";
			this.arg2.Size = new System.Drawing.Size(93, 22);
			this.arg2.TabIndex = 26;
			// 
			// LinedefEditForm
			// 
			this.AcceptButton = this.apply;
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.CancelButton = this.cancel;
			this.ClientSize = new System.Drawing.Size(561, 528);
			this.Controls.Add(this.tabs);
			this.Controls.Add(this.cancel);
			this.Controls.Add(this.apply);
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
			this.actiongroup.ResumeLayout(false);
			this.actiongroup.PerformLayout();
			this.hexenpanel.ResumeLayout(false);
			this.hexenpanel.PerformLayout();
			this.settingsgroup.ResumeLayout(false);
			this.tabs.ResumeLayout(false);
			this.tabproperties.ResumeLayout(false);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.tabsidedefs.ResumeLayout(false);
			this.tabsidedefs.PerformLayout();
			this.backgroup.ResumeLayout(false);
			this.backgroup.PerformLayout();
			this.frontgroup.ResumeLayout(false);
			this.frontgroup.PerformLayout();
			this.tabcustom.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button cancel;
		private System.Windows.Forms.Button apply;
		private System.Windows.Forms.GroupBox actiongroup;
		private System.Windows.Forms.GroupBox settingsgroup;
		private CheckboxArrayControl flags;
		private System.Windows.Forms.CheckBox checkBox1;
		private System.Windows.Forms.Button browseaction;
		private ActionSelectorControl action;
		private NumericTextbox tag;
		private System.Windows.Forms.Button newtag;
		private System.Windows.Forms.TabControl tabs;
		private System.Windows.Forms.TabPage tabproperties;
		private System.Windows.Forms.TabPage tabsidedefs;
		private System.Windows.Forms.GroupBox frontgroup;
		private System.Windows.Forms.CheckBox frontside;
		private NumericTextbox frontoffsety;
		private NumericTextbox frontoffsetx;
		private System.Windows.Forms.CheckBox backside;
		private System.Windows.Forms.GroupBox backgroup;
		private NumericTextbox backoffsety;
		private NumericTextbox backoffsetx;
		private TextureSelectorControl frontlow;
		private TextureSelectorControl frontmid;
		private TextureSelectorControl fronthigh;
		private TextureSelectorControl backlow;
		private TextureSelectorControl backmid;
		private TextureSelectorControl backhigh;
		private NumericTextbox backsector;
		private NumericTextbox frontsector;
		private System.Windows.Forms.ComboBox activation;
		private System.Windows.Forms.Panel doompanel;
		private System.Windows.Forms.Panel hexenpanel;
		private System.Windows.Forms.Label arg0label;
		private System.Windows.Forms.Label arg1label;
		private System.Windows.Forms.Label arg4label;
		private System.Windows.Forms.Label arg2label;
		private System.Windows.Forms.Label arg3label;
		private System.Windows.Forms.TabPage tabcustom;
		private FieldsEditorControl fieldslist;
		private System.Windows.Forms.GroupBox groupBox1;
		private ArgumentBox arg4;
		private ArgumentBox arg2;
		private ArgumentBox arg1;
		private ArgumentBox arg0;
		private ArgumentBox arg3;
	}
}