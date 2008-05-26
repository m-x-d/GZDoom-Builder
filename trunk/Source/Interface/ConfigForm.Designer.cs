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
			System.Windows.Forms.Label label5;
			System.Windows.Forms.Label label6;
			System.Windows.Forms.Label label3;
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConfigForm));
			System.Windows.Forms.Label label2;
			System.Windows.Forms.Label label7;
			System.Windows.Forms.Label label9;
			System.Windows.Forms.Label label4;
			System.Windows.Forms.Label label1;
			this.cancel = new System.Windows.Forms.Button();
			this.apply = new System.Windows.Forms.Button();
			this.tabs = new System.Windows.Forms.TabControl();
			this.tabresources = new System.Windows.Forms.TabPage();
			this.configdata = new CodeImp.DoomBuilder.Interface.ResourceListEditor();
			this.tabnodebuilder = new System.Windows.Forms.TabPage();
			this.nodebuildertest = new System.Windows.Forms.ComboBox();
			this.nodebuildersave = new System.Windows.Forms.ComboBox();
			this.tabtesting = new System.Windows.Forms.TabPage();
			this.browsetestprogram = new System.Windows.Forms.Button();
			this.noresultlabel = new System.Windows.Forms.Label();
			this.testresult = new System.Windows.Forms.TextBox();
			this.labelresult = new System.Windows.Forms.Label();
			this.testparameters = new System.Windows.Forms.TextBox();
			this.testapplication = new System.Windows.Forms.TextBox();
			this.listconfigs = new System.Windows.Forms.ListView();
			this.columnname = new System.Windows.Forms.ColumnHeader();
			this.testprogramdialog = new System.Windows.Forms.OpenFileDialog();
			label5 = new System.Windows.Forms.Label();
			label6 = new System.Windows.Forms.Label();
			label3 = new System.Windows.Forms.Label();
			label2 = new System.Windows.Forms.Label();
			label7 = new System.Windows.Forms.Label();
			label9 = new System.Windows.Forms.Label();
			label4 = new System.Windows.Forms.Label();
			label1 = new System.Windows.Forms.Label();
			this.tabs.SuspendLayout();
			this.tabresources.SuspendLayout();
			this.tabnodebuilder.SuspendLayout();
			this.tabtesting.SuspendLayout();
			this.SuspendLayout();
			// 
			// label5
			// 
			label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			label5.AutoSize = true;
			label5.Location = new System.Drawing.Point(12, 288);
			label5.Name = "label5";
			label5.Size = new System.Drawing.Size(312, 14);
			label5.TabIndex = 19;
			label5.Text = "Drag items to change order (lower items override higher items).";
			// 
			// label6
			// 
			label6.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			label6.AutoEllipsis = true;
			label6.Location = new System.Drawing.Point(12, 15);
			label6.Name = "label6";
			label6.Size = new System.Drawing.Size(394, 37);
			label6.TabIndex = 21;
			label6.Text = "These are the resources that will be loaded when this configuration is chosen for" +
				" editing. Usually you add your IWAD (like doom.wad or doom2.wad) here.";
			// 
			// label3
			// 
			label3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			label3.AutoEllipsis = true;
			label3.Location = new System.Drawing.Point(12, 15);
			label3.Name = "label3";
			label3.Size = new System.Drawing.Size(384, 54);
			label3.TabIndex = 22;
			label3.Text = resources.GetString("label3.Text");
			// 
			// label2
			// 
			label2.AutoSize = true;
			label2.Location = new System.Drawing.Point(12, 80);
			label2.Name = "label2";
			label2.Size = new System.Drawing.Size(149, 14);
			label2.TabIndex = 24;
			label2.Text = "Configuration for saving map:";
			// 
			// label7
			// 
			label7.AutoSize = true;
			label7.Location = new System.Drawing.Point(35, 125);
			label7.Name = "label7";
			label7.Size = new System.Drawing.Size(126, 14);
			label7.TabIndex = 26;
			label7.Text = "Configuration for testing:";
			// 
			// label9
			// 
			label9.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			label9.AutoEllipsis = true;
			label9.Location = new System.Drawing.Point(12, 15);
			label9.Name = "label9";
			label9.Size = new System.Drawing.Size(393, 54);
			label9.TabIndex = 23;
			label9.Text = resources.GetString("label9.Text");
			// 
			// label4
			// 
			label4.AutoSize = true;
			label4.Location = new System.Drawing.Point(15, 121);
			label4.Name = "label4";
			label4.Size = new System.Drawing.Size(65, 14);
			label4.TabIndex = 27;
			label4.Text = "Parameters:";
			// 
			// label1
			// 
			label1.AutoSize = true;
			label1.Location = new System.Drawing.Point(15, 80);
			label1.Name = "label1";
			label1.Size = new System.Drawing.Size(63, 14);
			label1.TabIndex = 24;
			label1.Text = "Application:";
			// 
			// cancel
			// 
			this.cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancel.Location = new System.Drawing.Point(567, 368);
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
			this.apply.Location = new System.Drawing.Point(449, 368);
			this.apply.Name = "apply";
			this.apply.Size = new System.Drawing.Size(112, 25);
			this.apply.TabIndex = 16;
			this.apply.Text = "OK";
			this.apply.UseVisualStyleBackColor = true;
			this.apply.Click += new System.EventHandler(this.apply_Click);
			// 
			// tabs
			// 
			this.tabs.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.tabs.Controls.Add(this.tabresources);
			this.tabs.Controls.Add(this.tabnodebuilder);
			this.tabs.Controls.Add(this.tabtesting);
			this.tabs.Enabled = false;
			this.tabs.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.tabs.ItemSize = new System.Drawing.Size(110, 19);
			this.tabs.Location = new System.Drawing.Point(256, 12);
			this.tabs.Name = "tabs";
			this.tabs.SelectedIndex = 0;
			this.tabs.Size = new System.Drawing.Size(423, 345);
			this.tabs.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
			this.tabs.TabIndex = 18;
			// 
			// tabresources
			// 
			this.tabresources.Controls.Add(label6);
			this.tabresources.Controls.Add(this.configdata);
			this.tabresources.Controls.Add(label5);
			this.tabresources.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.tabresources.Location = new System.Drawing.Point(4, 23);
			this.tabresources.Name = "tabresources";
			this.tabresources.Padding = new System.Windows.Forms.Padding(6);
			this.tabresources.Size = new System.Drawing.Size(415, 318);
			this.tabresources.TabIndex = 0;
			this.tabresources.Text = "Resources";
			this.tabresources.UseVisualStyleBackColor = true;
			// 
			// configdata
			// 
			this.configdata.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.configdata.DialogOffset = new System.Drawing.Point(-120, 10);
			this.configdata.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.configdata.Location = new System.Drawing.Point(15, 55);
			this.configdata.Name = "configdata";
			this.configdata.Size = new System.Drawing.Size(382, 220);
			this.configdata.TabIndex = 20;
			this.configdata.OnContentChanged += new CodeImp.DoomBuilder.Interface.ResourceListEditor.ContentChanged(this.resourcelocations_OnContentChanged);
			// 
			// tabnodebuilder
			// 
			this.tabnodebuilder.Controls.Add(label7);
			this.tabnodebuilder.Controls.Add(this.nodebuildertest);
			this.tabnodebuilder.Controls.Add(label2);
			this.tabnodebuilder.Controls.Add(this.nodebuildersave);
			this.tabnodebuilder.Controls.Add(label3);
			this.tabnodebuilder.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.tabnodebuilder.Location = new System.Drawing.Point(4, 23);
			this.tabnodebuilder.Name = "tabnodebuilder";
			this.tabnodebuilder.Padding = new System.Windows.Forms.Padding(6);
			this.tabnodebuilder.Size = new System.Drawing.Size(414, 318);
			this.tabnodebuilder.TabIndex = 1;
			this.tabnodebuilder.Text = "Nodebuilder";
			this.tabnodebuilder.UseVisualStyleBackColor = true;
			// 
			// nodebuildertest
			// 
			this.nodebuildertest.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.nodebuildertest.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.nodebuildertest.FormattingEnabled = true;
			this.nodebuildertest.Location = new System.Drawing.Point(167, 122);
			this.nodebuildertest.Name = "nodebuildertest";
			this.nodebuildertest.Size = new System.Drawing.Size(229, 22);
			this.nodebuildertest.Sorted = true;
			this.nodebuildertest.TabIndex = 25;
			this.nodebuildertest.SelectedIndexChanged += new System.EventHandler(this.nodebuildertest_SelectedIndexChanged);
			// 
			// nodebuildersave
			// 
			this.nodebuildersave.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.nodebuildersave.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.nodebuildersave.FormattingEnabled = true;
			this.nodebuildersave.Location = new System.Drawing.Point(167, 77);
			this.nodebuildersave.Name = "nodebuildersave";
			this.nodebuildersave.Size = new System.Drawing.Size(229, 22);
			this.nodebuildersave.Sorted = true;
			this.nodebuildersave.TabIndex = 23;
			this.nodebuildersave.SelectedIndexChanged += new System.EventHandler(this.nodebuildersave_SelectedIndexChanged);
			// 
			// tabtesting
			// 
			this.tabtesting.Controls.Add(this.browsetestprogram);
			this.tabtesting.Controls.Add(this.noresultlabel);
			this.tabtesting.Controls.Add(this.testresult);
			this.tabtesting.Controls.Add(this.labelresult);
			this.tabtesting.Controls.Add(this.testparameters);
			this.tabtesting.Controls.Add(label4);
			this.tabtesting.Controls.Add(this.testapplication);
			this.tabtesting.Controls.Add(label1);
			this.tabtesting.Controls.Add(label9);
			this.tabtesting.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.tabtesting.Location = new System.Drawing.Point(4, 23);
			this.tabtesting.Name = "tabtesting";
			this.tabtesting.Padding = new System.Windows.Forms.Padding(6);
			this.tabtesting.Size = new System.Drawing.Size(414, 318);
			this.tabtesting.TabIndex = 2;
			this.tabtesting.Text = "Testing";
			this.tabtesting.UseVisualStyleBackColor = true;
			// 
			// browsetestprogram
			// 
			this.browsetestprogram.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.browsetestprogram.Image = global::CodeImp.DoomBuilder.Properties.Resources.Folder;
			this.browsetestprogram.Location = new System.Drawing.Point(363, 76);
			this.browsetestprogram.Name = "browsetestprogram";
			this.browsetestprogram.Padding = new System.Windows.Forms.Padding(0, 0, 1, 3);
			this.browsetestprogram.Size = new System.Drawing.Size(30, 23);
			this.browsetestprogram.TabIndex = 33;
			this.browsetestprogram.Text = " ";
			this.browsetestprogram.UseVisualStyleBackColor = true;
			this.browsetestprogram.Click += new System.EventHandler(this.browsetestprogram_Click);
			// 
			// noresultlabel
			// 
			this.noresultlabel.Location = new System.Drawing.Point(84, 217);
			this.noresultlabel.Name = "noresultlabel";
			this.noresultlabel.Size = new System.Drawing.Size(272, 43);
			this.noresultlabel.TabIndex = 32;
			this.noresultlabel.Text = "An example result cannot be displayed, because it requires a map to be loaded.";
			// 
			// testresult
			// 
			this.testresult.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.testresult.BackColor = System.Drawing.SystemColors.Control;
			this.testresult.Location = new System.Drawing.Point(86, 217);
			this.testresult.Multiline = true;
			this.testresult.Name = "testresult";
			this.testresult.ReadOnly = true;
			this.testresult.Size = new System.Drawing.Size(307, 79);
			this.testresult.TabIndex = 31;
			// 
			// labelresult
			// 
			this.labelresult.AutoSize = true;
			this.labelresult.Location = new System.Drawing.Point(38, 220);
			this.labelresult.Name = "labelresult";
			this.labelresult.Size = new System.Drawing.Size(40, 14);
			this.labelresult.TabIndex = 30;
			this.labelresult.Text = "Result:";
			// 
			// testparameters
			// 
			this.testparameters.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.testparameters.Location = new System.Drawing.Point(86, 118);
			this.testparameters.Multiline = true;
			this.testparameters.Name = "testparameters";
			this.testparameters.Size = new System.Drawing.Size(307, 79);
			this.testparameters.TabIndex = 28;
			this.testparameters.TextChanged += new System.EventHandler(this.testparameters_TextChanged);
			// 
			// testapplication
			// 
			this.testapplication.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.testapplication.Location = new System.Drawing.Point(86, 77);
			this.testapplication.Name = "testapplication";
			this.testapplication.ReadOnly = true;
			this.testapplication.Size = new System.Drawing.Size(271, 20);
			this.testapplication.TabIndex = 25;
			this.testapplication.TextChanged += new System.EventHandler(this.testapplication_TextChanged);
			// 
			// listconfigs
			// 
			this.listconfigs.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnname});
			this.listconfigs.FullRowSelect = true;
			this.listconfigs.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
			this.listconfigs.HideSelection = false;
			this.listconfigs.Location = new System.Drawing.Point(12, 12);
			this.listconfigs.MultiSelect = false;
			this.listconfigs.Name = "listconfigs";
			this.listconfigs.ShowGroups = false;
			this.listconfigs.Size = new System.Drawing.Size(230, 345);
			this.listconfigs.Sorting = System.Windows.Forms.SortOrder.Ascending;
			this.listconfigs.TabIndex = 19;
			this.listconfigs.UseCompatibleStateImageBehavior = false;
			this.listconfigs.View = System.Windows.Forms.View.Details;
			this.listconfigs.SelectedIndexChanged += new System.EventHandler(this.listconfigs_SelectedIndexChanged);
			this.listconfigs.MouseUp += new System.Windows.Forms.MouseEventHandler(this.listconfigs_MouseUp);
			this.listconfigs.KeyUp += new System.Windows.Forms.KeyEventHandler(this.listconfigs_KeyUp);
			// 
			// columnname
			// 
			this.columnname.Text = "Configuration";
			this.columnname.Width = 200;
			// 
			// testprogramdialog
			// 
			this.testprogramdialog.Filter = "Executable Files (*.exe)|*.exe|Batch Files (*.bat)|*.bat";
			this.testprogramdialog.Title = "Browse Test Program";
			// 
			// ConfigForm
			// 
			this.AcceptButton = this.apply;
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.CancelButton = this.cancel;
			this.ClientSize = new System.Drawing.Size(690, 403);
			this.Controls.Add(this.listconfigs);
			this.Controls.Add(this.tabs);
			this.Controls.Add(this.cancel);
			this.Controls.Add(this.apply);
			this.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ConfigForm";
			this.Opacity = 0;
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Game Configurations";
			this.tabs.ResumeLayout(false);
			this.tabresources.ResumeLayout(false);
			this.tabresources.PerformLayout();
			this.tabnodebuilder.ResumeLayout(false);
			this.tabnodebuilder.PerformLayout();
			this.tabtesting.ResumeLayout(false);
			this.tabtesting.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button cancel;
		private System.Windows.Forms.Button apply;
		private System.Windows.Forms.TabControl tabs;
		private System.Windows.Forms.TabPage tabresources;
		private System.Windows.Forms.TabPage tabnodebuilder;
		private System.Windows.Forms.TabPage tabtesting;
		private ResourceListEditor configdata;
		private System.Windows.Forms.ComboBox nodebuildertest;
		private System.Windows.Forms.ComboBox nodebuildersave;
		private System.Windows.Forms.TextBox testparameters;
		private System.Windows.Forms.TextBox testapplication;
		private System.Windows.Forms.TextBox testresult;
		private System.Windows.Forms.Label labelresult;
		private System.Windows.Forms.ListView listconfigs;
		private System.Windows.Forms.ColumnHeader columnname;
		private System.Windows.Forms.Label noresultlabel;
		private System.Windows.Forms.Button browsetestprogram;
		private System.Windows.Forms.OpenFileDialog testprogramdialog;
	}
}