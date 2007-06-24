namespace CodeImp.DoomBuilder.Interface
{
	partial class OpenMapOptionsForm
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
			System.Windows.Forms.ColumnHeader columnHeader1;
			this.panelres = new System.Windows.Forms.GroupBox();
			this.deleteresource = new System.Windows.Forms.Button();
			this.editresource = new System.Windows.Forms.Button();
			this.addresource = new System.Windows.Forms.Button();
			this.resources = new System.Windows.Forms.ListBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.apply = new System.Windows.Forms.Button();
			this.cancel = new System.Windows.Forms.Button();
			this.config = new System.Windows.Forms.ComboBox();
			this.mapslist = new System.Windows.Forms.ListView();
			columnHeader1 = new System.Windows.Forms.ColumnHeader();
			this.panelres.SuspendLayout();
			this.SuspendLayout();
			// 
			// columnHeader1
			// 
			columnHeader1.Text = "Map name";
			// 
			// panelres
			// 
			this.panelres.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.panelres.Controls.Add(this.deleteresource);
			this.panelres.Controls.Add(this.editresource);
			this.panelres.Controls.Add(this.addresource);
			this.panelres.Controls.Add(this.resources);
			this.panelres.Location = new System.Drawing.Point(12, 214);
			this.panelres.Name = "panelres";
			this.panelres.Size = new System.Drawing.Size(365, 165);
			this.panelres.TabIndex = 11;
			this.panelres.TabStop = false;
			this.panelres.Text = " Custom Resources ";
			// 
			// deleteresource
			// 
			this.deleteresource.Enabled = false;
			this.deleteresource.Location = new System.Drawing.Point(268, 125);
			this.deleteresource.Name = "deleteresource";
			this.deleteresource.Size = new System.Drawing.Size(74, 25);
			this.deleteresource.TabIndex = 13;
			this.deleteresource.Text = "Remove";
			this.deleteresource.UseVisualStyleBackColor = true;
			this.deleteresource.Click += new System.EventHandler(this.deleteresource_Click);
			// 
			// editresource
			// 
			this.editresource.Enabled = false;
			this.editresource.Location = new System.Drawing.Point(139, 125);
			this.editresource.Name = "editresource";
			this.editresource.Size = new System.Drawing.Size(123, 25);
			this.editresource.TabIndex = 12;
			this.editresource.Text = "Resource Options...";
			this.editresource.UseVisualStyleBackColor = true;
			this.editresource.Click += new System.EventHandler(this.editresource_Click);
			// 
			// addresource
			// 
			this.addresource.Location = new System.Drawing.Point(21, 125);
			this.addresource.Name = "addresource";
			this.addresource.Size = new System.Drawing.Size(112, 25);
			this.addresource.TabIndex = 11;
			this.addresource.Text = "Add Resource...";
			this.addresource.UseVisualStyleBackColor = true;
			this.addresource.Click += new System.EventHandler(this.addresource_Click);
			// 
			// resources
			// 
			this.resources.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.resources.FormattingEnabled = true;
			this.resources.ItemHeight = 14;
			this.resources.Location = new System.Drawing.Point(21, 31);
			this.resources.Name = "resources";
			this.resources.Size = new System.Drawing.Size(321, 88);
			this.resources.TabIndex = 10;
			this.resources.DoubleClick += new System.EventHandler(this.resources_DoubleClick);
			this.resources.SelectedIndexChanged += new System.EventHandler(this.resources_SelectedIndexChanged);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(30, 24);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(105, 14);
			this.label1.TabIndex = 14;
			this.label1.Text = "Game Configuration:";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(12, 57);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(365, 30);
			this.label2.TabIndex = 16;
			this.label2.Text = "With the above selected configuration, the maps shown below were found in the cho" +
				"sen WAD file. Please select the map to load for editing.";
			// 
			// apply
			// 
			this.apply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.apply.Location = new System.Drawing.Point(147, 399);
			this.apply.Name = "apply";
			this.apply.Size = new System.Drawing.Size(112, 25);
			this.apply.TabIndex = 12;
			this.apply.Text = "OK";
			this.apply.UseVisualStyleBackColor = true;
			this.apply.Click += new System.EventHandler(this.apply_Click);
			// 
			// cancel
			// 
			this.cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancel.Location = new System.Drawing.Point(265, 399);
			this.cancel.Name = "cancel";
			this.cancel.Size = new System.Drawing.Size(112, 25);
			this.cancel.TabIndex = 13;
			this.cancel.Text = "Cancel";
			this.cancel.UseVisualStyleBackColor = true;
			this.cancel.Click += new System.EventHandler(this.cancel_Click);
			// 
			// config
			// 
			this.config.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.config.FormattingEnabled = true;
			this.config.Location = new System.Drawing.Point(141, 21);
			this.config.Name = "config";
			this.config.Size = new System.Drawing.Size(213, 22);
			this.config.TabIndex = 15;
			this.config.SelectedIndexChanged += new System.EventHandler(this.config_SelectedIndexChanged);
			// 
			// mapslist
			// 
			this.mapslist.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            columnHeader1});
			this.mapslist.FullRowSelect = true;
			this.mapslist.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
			this.mapslist.HideSelection = false;
			this.mapslist.LabelWrap = false;
			this.mapslist.Location = new System.Drawing.Point(12, 90);
			this.mapslist.MultiSelect = false;
			this.mapslist.Name = "mapslist";
			this.mapslist.ShowGroups = false;
			this.mapslist.Size = new System.Drawing.Size(365, 110);
			this.mapslist.Sorting = System.Windows.Forms.SortOrder.Ascending;
			this.mapslist.TabIndex = 18;
			this.mapslist.UseCompatibleStateImageBehavior = false;
			this.mapslist.View = System.Windows.Forms.View.List;
			this.mapslist.DoubleClick += new System.EventHandler(this.mapslist_DoubleClick);
			// 
			// OpenMapOptionsForm
			// 
			this.AcceptButton = this.apply;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 14F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.cancel;
			this.ClientSize = new System.Drawing.Size(389, 436);
			this.Controls.Add(this.mapslist);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.config);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.cancel);
			this.Controls.Add(this.apply);
			this.Controls.Add(this.panelres);
			this.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "OpenMapOptionsForm";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Open Map Options";
			this.Shown += new System.EventHandler(this.OpenMapOptionsForm_Shown);
			this.panelres.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button deleteresource;
		private System.Windows.Forms.Button editresource;
		private System.Windows.Forms.Button addresource;
		private System.Windows.Forms.ListBox resources;
		private System.Windows.Forms.Button apply;
		private System.Windows.Forms.Button cancel;
		private System.Windows.Forms.ComboBox config;
		private System.Windows.Forms.GroupBox panelres;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.ListView mapslist;


	}
}