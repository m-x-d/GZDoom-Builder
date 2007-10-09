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
			label7 = new System.Windows.Forms.Label();
			label6 = new System.Windows.Forms.Label();
			label5 = new System.Windows.Forms.Label();
			this.tabs.SuspendLayout();
			this.tabkeys.SuspendLayout();
			this.actioncontrolpanel.SuspendLayout();
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
			// cancel
			// 
			this.cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancel.Location = new System.Drawing.Point(497, 406);
			this.cancel.Name = "cancel";
			this.cancel.Size = new System.Drawing.Size(112, 27);
			this.cancel.TabIndex = 20;
			this.cancel.Text = "Cancel";
			this.cancel.UseVisualStyleBackColor = true;
			this.cancel.Click += new System.EventHandler(this.cancel_Click);
			// 
			// apply
			// 
			this.apply.Location = new System.Drawing.Point(379, 406);
			this.apply.Name = "apply";
			this.apply.Size = new System.Drawing.Size(112, 27);
			this.apply.TabIndex = 19;
			this.apply.Text = "OK";
			this.apply.UseVisualStyleBackColor = true;
			this.apply.Click += new System.EventHandler(this.apply_Click);
			// 
			// tabs
			// 
			this.tabs.Controls.Add(this.tabinterface);
			this.tabs.Controls.Add(this.tabkeys);
			this.tabs.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.tabs.ItemSize = new System.Drawing.Size(110, 19);
			this.tabs.Location = new System.Drawing.Point(11, 13);
			this.tabs.Name = "tabs";
			this.tabs.SelectedIndex = 0;
			this.tabs.Size = new System.Drawing.Size(598, 379);
			this.tabs.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
			this.tabs.TabIndex = 18;
			// 
			// tabinterface
			// 
			this.tabinterface.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.tabinterface.Location = new System.Drawing.Point(4, 23);
			this.tabinterface.Name = "tabinterface";
			this.tabinterface.Padding = new System.Windows.Forms.Padding(3);
			this.tabinterface.Size = new System.Drawing.Size(590, 352);
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
			this.tabkeys.Size = new System.Drawing.Size(590, 352);
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
			this.listactions.Size = new System.Drawing.Size(274, 326);
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
			this.actioncontrolpanel.Size = new System.Drawing.Size(282, 326);
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
			this.actioncontrolclear.Size = new System.Drawing.Size(63, 27);
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
			// PreferencesForm
			// 
			this.AcceptButton = this.apply;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 14F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.cancel;
			this.ClientSize = new System.Drawing.Size(619, 442);
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
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Preferences";
			this.tabs.ResumeLayout(false);
			this.tabkeys.ResumeLayout(false);
			this.actioncontrolpanel.ResumeLayout(false);
			this.actioncontrolpanel.PerformLayout();
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
	}
}