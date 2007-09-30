namespace CodeImp.DoomBuilder.Interface
{
	partial class ConfigForm
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
			System.Windows.Forms.Label label1;
			System.Windows.Forms.Label label6;
			System.Windows.Forms.Label label7;
			System.Windows.Forms.Label label5;
			this.panelnodebuilder = new System.Windows.Forms.GroupBox();
			this.configbuildonsave = new System.Windows.Forms.CheckBox();
			this.confignodebuilder = new System.Windows.Forms.ComboBox();
			this.paneltesting = new System.Windows.Forms.GroupBox();
			this.testparameters = new System.Windows.Forms.TextBox();
			this.browsewad = new System.Windows.Forms.Button();
			this.testapplication = new System.Windows.Forms.TextBox();
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
			this.tabconfigs = new System.Windows.Forms.TabPage();
			this.panelresources = new System.Windows.Forms.GroupBox();
			this.configresources = new CodeImp.DoomBuilder.Interface.ResourceListEditor();
			this.listconfigs = new System.Windows.Forms.ListBox();
			this.cancel = new System.Windows.Forms.Button();
			this.apply = new System.Windows.Forms.Button();
			label2 = new System.Windows.Forms.Label();
			label3 = new System.Windows.Forms.Label();
			label4 = new System.Windows.Forms.Label();
			label1 = new System.Windows.Forms.Label();
			label6 = new System.Windows.Forms.Label();
			label7 = new System.Windows.Forms.Label();
			label5 = new System.Windows.Forms.Label();
			this.panelnodebuilder.SuspendLayout();
			this.paneltesting.SuspendLayout();
			this.tabs.SuspendLayout();
			this.tabkeys.SuspendLayout();
			this.actioncontrolpanel.SuspendLayout();
			this.tabconfigs.SuspendLayout();
			this.panelresources.SuspendLayout();
			this.SuspendLayout();
			// 
			// label2
			// 
			label2.AutoSize = true;
			label2.Location = new System.Drawing.Point(25, 31);
			label2.Name = "label2";
			label2.Size = new System.Drawing.Size(74, 14);
			label2.TabIndex = 3;
			label2.Text = "Configuration:";
			// 
			// label3
			// 
			label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			label3.AutoSize = true;
			label3.Location = new System.Drawing.Point(11, 112);
			label3.Name = "label3";
			label3.Size = new System.Drawing.Size(312, 14);
			label3.TabIndex = 17;
			label3.Text = "Drag items to change order (lower items override higher items).";
			// 
			// label4
			// 
			label4.AutoSize = true;
			label4.Location = new System.Drawing.Point(23, 67);
			label4.Name = "label4";
			label4.Size = new System.Drawing.Size(65, 14);
			label4.TabIndex = 7;
			label4.Text = "Parameters:";
			// 
			// label1
			// 
			label1.AutoSize = true;
			label1.Location = new System.Drawing.Point(25, 32);
			label1.Name = "label1";
			label1.Size = new System.Drawing.Size(63, 14);
			label1.TabIndex = 4;
			label1.Text = "Application:";
			// 
			// label6
			// 
			label6.AutoSize = true;
			label6.Location = new System.Drawing.Point(20, 28);
			label6.Name = "label6";
			label6.Size = new System.Drawing.Size(41, 14);
			label6.TabIndex = 2;
			label6.Text = "Action:";
			// 
			// label7
			// 
			label7.AutoSize = true;
			label7.Location = new System.Drawing.Point(20, 170);
			label7.Name = "label7";
			label7.Size = new System.Drawing.Size(187, 14);
			label7.TabIndex = 7;
			label7.Text = "Or select a special input control here:";
			// 
			// label5
			// 
			label5.AutoSize = true;
			label5.Location = new System.Drawing.Point(20, 113);
			label5.Name = "label5";
			label5.Size = new System.Drawing.Size(200, 14);
			label5.TabIndex = 4;
			label5.Text = "Press the desired key combination here:";
			// 
			// panelnodebuilder
			// 
			this.panelnodebuilder.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.panelnodebuilder.Controls.Add(this.configbuildonsave);
			this.panelnodebuilder.Controls.Add(label2);
			this.panelnodebuilder.Controls.Add(this.confignodebuilder);
			this.panelnodebuilder.Enabled = false;
			this.panelnodebuilder.Location = new System.Drawing.Point(237, 151);
			this.panelnodebuilder.Margin = new System.Windows.Forms.Padding(6);
			this.panelnodebuilder.Name = "panelnodebuilder";
			this.panelnodebuilder.Size = new System.Drawing.Size(341, 97);
			this.panelnodebuilder.TabIndex = 2;
			this.panelnodebuilder.TabStop = false;
			this.panelnodebuilder.Text = " Nodebuilder";
			// 
			// configbuildonsave
			// 
			this.configbuildonsave.AutoSize = true;
			this.configbuildonsave.Location = new System.Drawing.Point(49, 62);
			this.configbuildonsave.Name = "configbuildonsave";
			this.configbuildonsave.Size = new System.Drawing.Size(242, 18);
			this.configbuildonsave.TabIndex = 4;
			this.configbuildonsave.Text = "Build nodes every time when saving the map";
			this.configbuildonsave.UseVisualStyleBackColor = true;
			this.configbuildonsave.CheckedChanged += new System.EventHandler(this.configbuildonsave_CheckedChanged);
			// 
			// confignodebuilder
			// 
			this.confignodebuilder.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.confignodebuilder.FormattingEnabled = true;
			this.confignodebuilder.Location = new System.Drawing.Point(105, 28);
			this.confignodebuilder.Name = "confignodebuilder";
			this.confignodebuilder.Size = new System.Drawing.Size(186, 22);
			this.confignodebuilder.Sorted = true;
			this.confignodebuilder.TabIndex = 2;
			this.confignodebuilder.SelectedIndexChanged += new System.EventHandler(this.confignodebuilder_SelectedIndexChanged);
			// 
			// paneltesting
			// 
			this.paneltesting.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.paneltesting.Controls.Add(this.testparameters);
			this.paneltesting.Controls.Add(label4);
			this.paneltesting.Controls.Add(this.browsewad);
			this.paneltesting.Controls.Add(this.testapplication);
			this.paneltesting.Controls.Add(label1);
			this.paneltesting.Enabled = false;
			this.paneltesting.Location = new System.Drawing.Point(237, 254);
			this.paneltesting.Margin = new System.Windows.Forms.Padding(6);
			this.paneltesting.Name = "paneltesting";
			this.paneltesting.Size = new System.Drawing.Size(341, 107);
			this.paneltesting.TabIndex = 13;
			this.paneltesting.TabStop = false;
			this.paneltesting.Text = " Testing ";
			// 
			// testparameters
			// 
			this.testparameters.Location = new System.Drawing.Point(94, 64);
			this.testparameters.Name = "testparameters";
			this.testparameters.Size = new System.Drawing.Size(197, 20);
			this.testparameters.TabIndex = 8;
			this.testparameters.TextChanged += new System.EventHandler(this.testparameters_TextChanged);
			// 
			// browsewad
			// 
			this.browsewad.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.browsewad.Location = new System.Drawing.Point(297, 28);
			this.browsewad.Name = "browsewad";
			this.browsewad.Size = new System.Drawing.Size(30, 23);
			this.browsewad.TabIndex = 6;
			this.browsewad.Text = "...";
			this.browsewad.UseVisualStyleBackColor = true;
			// 
			// testapplication
			// 
			this.testapplication.Location = new System.Drawing.Point(94, 29);
			this.testapplication.Name = "testapplication";
			this.testapplication.ReadOnly = true;
			this.testapplication.Size = new System.Drawing.Size(197, 20);
			this.testapplication.TabIndex = 5;
			this.testapplication.TextChanged += new System.EventHandler(this.testapplication_TextChanged);
			// 
			// tabs
			// 
			this.tabs.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.tabs.Controls.Add(this.tabinterface);
			this.tabs.Controls.Add(this.tabkeys);
			this.tabs.Controls.Add(this.tabconfigs);
			this.tabs.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.tabs.ItemSize = new System.Drawing.Size(110, 19);
			this.tabs.Location = new System.Drawing.Point(12, 12);
			this.tabs.Name = "tabs";
			this.tabs.SelectedIndex = 0;
			this.tabs.Size = new System.Drawing.Size(595, 399);
			this.tabs.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
			this.tabs.TabIndex = 0;
			// 
			// tabinterface
			// 
			this.tabinterface.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.tabinterface.Location = new System.Drawing.Point(4, 23);
			this.tabinterface.Name = "tabinterface";
			this.tabinterface.Padding = new System.Windows.Forms.Padding(3);
			this.tabinterface.Size = new System.Drawing.Size(587, 372);
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
			this.tabkeys.Size = new System.Drawing.Size(587, 372);
			this.tabkeys.TabIndex = 1;
			this.tabkeys.Text = "Controls";
			this.tabkeys.UseVisualStyleBackColor = true;
			// 
			// listactions
			// 
			this.listactions.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)));
			this.listactions.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columncontrolaction,
            this.columncontrolkey});
			this.listactions.FullRowSelect = true;
			this.listactions.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			this.listactions.HideSelection = false;
			this.listactions.Location = new System.Drawing.Point(11, 11);
			this.listactions.Margin = new System.Windows.Forms.Padding(8);
			this.listactions.MultiSelect = false;
			this.listactions.Name = "listactions";
			this.listactions.ShowGroups = false;
			this.listactions.Size = new System.Drawing.Size(275, 350);
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
			this.actioncontrolpanel.Controls.Add(this.actioncontrol);
			this.actioncontrolpanel.Controls.Add(label7);
			this.actioncontrolpanel.Controls.Add(this.actiontitle);
			this.actioncontrolpanel.Controls.Add(this.actioncontrolclear);
			this.actioncontrolpanel.Controls.Add(label6);
			this.actioncontrolpanel.Controls.Add(this.actionkey);
			this.actioncontrolpanel.Controls.Add(this.actiondescription);
			this.actioncontrolpanel.Controls.Add(label5);
			this.actioncontrolpanel.Enabled = false;
			this.actioncontrolpanel.Location = new System.Drawing.Point(300, 11);
			this.actioncontrolpanel.Margin = new System.Windows.Forms.Padding(6);
			this.actioncontrolpanel.Name = "actioncontrolpanel";
			this.actioncontrolpanel.Size = new System.Drawing.Size(278, 350);
			this.actioncontrolpanel.TabIndex = 9;
			this.actioncontrolpanel.TabStop = false;
			this.actioncontrolpanel.Text = " Action control ";
			// 
			// actioncontrol
			// 
			this.actioncontrol.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.actioncontrol.FormattingEnabled = true;
			this.actioncontrol.Location = new System.Drawing.Point(23, 189);
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
			this.actiontitle.Location = new System.Drawing.Point(67, 28);
			this.actiontitle.Name = "actiontitle";
			this.actiontitle.Size = new System.Drawing.Size(172, 14);
			this.actiontitle.TabIndex = 1;
			this.actiontitle.Text = "(select an action from the list)";
			this.actiontitle.UseMnemonic = false;
			// 
			// actioncontrolclear
			// 
			this.actioncontrolclear.Location = new System.Drawing.Point(193, 130);
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
			this.actionkey.Location = new System.Drawing.Point(23, 132);
			this.actionkey.Name = "actionkey";
			this.actionkey.Size = new System.Drawing.Size(163, 20);
			this.actionkey.TabIndex = 5;
			this.actionkey.TabStop = false;
			this.actionkey.TextChanged += new System.EventHandler(this.actionkey_TextChanged);
			this.actionkey.KeyDown += new System.Windows.Forms.KeyEventHandler(this.actionkey_KeyDown);
			// 
			// actiondescription
			// 
			this.actiondescription.AutoEllipsis = true;
			this.actiondescription.Location = new System.Drawing.Point(20, 48);
			this.actiondescription.Name = "actiondescription";
			this.actiondescription.Size = new System.Drawing.Size(245, 65);
			this.actiondescription.TabIndex = 3;
			this.actiondescription.UseMnemonic = false;
			// 
			// tabconfigs
			// 
			this.tabconfigs.Controls.Add(this.paneltesting);
			this.tabconfigs.Controls.Add(this.panelresources);
			this.tabconfigs.Controls.Add(this.panelnodebuilder);
			this.tabconfigs.Controls.Add(this.listconfigs);
			this.tabconfigs.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.tabconfigs.Location = new System.Drawing.Point(4, 23);
			this.tabconfigs.Margin = new System.Windows.Forms.Padding(8);
			this.tabconfigs.Name = "tabconfigs";
			this.tabconfigs.Padding = new System.Windows.Forms.Padding(3);
			this.tabconfigs.Size = new System.Drawing.Size(587, 372);
			this.tabconfigs.TabIndex = 2;
			this.tabconfigs.Text = "Configurations";
			this.tabconfigs.UseVisualStyleBackColor = true;
			// 
			// panelresources
			// 
			this.panelresources.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.panelresources.Controls.Add(this.configresources);
			this.panelresources.Controls.Add(label3);
			this.panelresources.Enabled = false;
			this.panelresources.Location = new System.Drawing.Point(237, 11);
			this.panelresources.Margin = new System.Windows.Forms.Padding(6);
			this.panelresources.Name = "panelresources";
			this.panelresources.Size = new System.Drawing.Size(341, 134);
			this.panelresources.TabIndex = 12;
			this.panelresources.TabStop = false;
			this.panelresources.Text = " Resources ";
			// 
			// configresources
			// 
			this.configresources.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.configresources.DialogOffset = new System.Drawing.Point(-120, 10);
			this.configresources.Location = new System.Drawing.Point(11, 25);
			this.configresources.Name = "configresources";
			this.configresources.Size = new System.Drawing.Size(318, 84);
			this.configresources.TabIndex = 18;
			this.configresources.OnContentChanged += new CodeImp.DoomBuilder.Interface.ResourceListEditor.ContentChanged(this.resourcelocations_OnContentChanged);
			// 
			// listconfigs
			// 
			this.listconfigs.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)));
			this.listconfigs.FormattingEnabled = true;
			this.listconfigs.IntegralHeight = false;
			this.listconfigs.ItemHeight = 14;
			this.listconfigs.Location = new System.Drawing.Point(11, 11);
			this.listconfigs.Margin = new System.Windows.Forms.Padding(8);
			this.listconfigs.Name = "listconfigs";
			this.listconfigs.ScrollAlwaysVisible = true;
			this.listconfigs.Size = new System.Drawing.Size(212, 350);
			this.listconfigs.Sorted = true;
			this.listconfigs.TabIndex = 0;
			this.listconfigs.SelectedIndexChanged += new System.EventHandler(this.listconfigs_SelectedIndexChanged);
			// 
			// cancel
			// 
			this.cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancel.Location = new System.Drawing.Point(495, 425);
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
			this.apply.Location = new System.Drawing.Point(377, 425);
			this.apply.Name = "apply";
			this.apply.Size = new System.Drawing.Size(112, 25);
			this.apply.TabIndex = 16;
			this.apply.Text = "OK";
			this.apply.UseVisualStyleBackColor = true;
			this.apply.Click += new System.EventHandler(this.apply_Click);
			// 
			// ConfigForm
			// 
			this.AcceptButton = this.apply;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 14F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.cancel;
			this.ClientSize = new System.Drawing.Size(619, 461);
			this.Controls.Add(this.cancel);
			this.Controls.Add(this.apply);
			this.Controls.Add(this.tabs);
			this.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ConfigForm";
			this.ShowIcon = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Configuration";
			this.panelnodebuilder.ResumeLayout(false);
			this.panelnodebuilder.PerformLayout();
			this.paneltesting.ResumeLayout(false);
			this.paneltesting.PerformLayout();
			this.tabs.ResumeLayout(false);
			this.tabkeys.ResumeLayout(false);
			this.actioncontrolpanel.ResumeLayout(false);
			this.actioncontrolpanel.PerformLayout();
			this.tabconfigs.ResumeLayout(false);
			this.panelresources.ResumeLayout(false);
			this.panelresources.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TabControl tabs;
		private System.Windows.Forms.TabPage tabinterface;
		private System.Windows.Forms.TabPage tabkeys;
		private System.Windows.Forms.TabPage tabconfigs;
		private System.Windows.Forms.Button cancel;
		private System.Windows.Forms.Button apply;
		private System.Windows.Forms.ListBox listconfigs;
		private System.Windows.Forms.ComboBox confignodebuilder;
		private System.Windows.Forms.CheckBox configbuildonsave;
		private System.Windows.Forms.GroupBox panelresources;
		private ResourceListEditor configresources;
		private System.Windows.Forms.Button browsewad;
		private System.Windows.Forms.TextBox testapplication;
		private System.Windows.Forms.TextBox testparameters;
		private System.Windows.Forms.GroupBox panelnodebuilder;
		private System.Windows.Forms.GroupBox paneltesting;
		private System.Windows.Forms.ListView listactions;
		private System.Windows.Forms.ColumnHeader columncontrolaction;
		private System.Windows.Forms.ColumnHeader columncontrolkey;
		private System.Windows.Forms.Label actiondescription;
		private System.Windows.Forms.Label actiontitle;
		private System.Windows.Forms.ComboBox actioncontrol;
		private System.Windows.Forms.Button actioncontrolclear;
		private System.Windows.Forms.TextBox actionkey;
		private System.Windows.Forms.GroupBox actioncontrolpanel;
	}
}