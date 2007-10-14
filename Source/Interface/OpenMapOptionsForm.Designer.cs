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
			System.Windows.Forms.Label label1;
			System.Windows.Forms.Label label2;
			System.Windows.Forms.Label label3;
			this.panelres = new System.Windows.Forms.GroupBox();
			this.datalocations = new CodeImp.DoomBuilder.Interface.ResourceListEditor();
			this.apply = new System.Windows.Forms.Button();
			this.cancel = new System.Windows.Forms.Button();
			this.config = new System.Windows.Forms.ComboBox();
			this.mapslist = new System.Windows.Forms.ListView();
			columnHeader1 = new System.Windows.Forms.ColumnHeader();
			label1 = new System.Windows.Forms.Label();
			label2 = new System.Windows.Forms.Label();
			label3 = new System.Windows.Forms.Label();
			this.panelres.SuspendLayout();
			this.SuspendLayout();
			// 
			// columnHeader1
			// 
			columnHeader1.Text = "Map name";
			// 
			// label1
			// 
			label1.AutoSize = true;
			label1.Location = new System.Drawing.Point(30, 24);
			label1.Name = "label1";
			label1.Size = new System.Drawing.Size(105, 14);
			label1.TabIndex = 14;
			label1.Text = "Game Configuration:";
			// 
			// label2
			// 
			label2.Location = new System.Drawing.Point(12, 57);
			label2.Name = "label2";
			label2.Size = new System.Drawing.Size(365, 30);
			label2.TabIndex = 16;
			label2.Text = "With the above selected configuration, the maps shown below were found in the cho" +
				"sen WAD file. Please select the map to load for editing.";
			// 
			// label3
			// 
			label3.Location = new System.Drawing.Point(14, 161);
			label3.Name = "label3";
			label3.Size = new System.Drawing.Size(336, 34);
			label3.TabIndex = 17;
			label3.Text = "Drag items to change order (lower items override higher items). Grayed items are " +
				"loaded according to the game configuration.";
			// 
			// panelres
			// 
			this.panelres.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.panelres.Controls.Add(this.datalocations);
			this.panelres.Controls.Add(label3);
			this.panelres.Location = new System.Drawing.Point(12, 214);
			this.panelres.Name = "panelres";
			this.panelres.Size = new System.Drawing.Size(365, 198);
			this.panelres.TabIndex = 11;
			this.panelres.TabStop = false;
			this.panelres.Text = " Custom Resources ";
			// 
			// datalocations
			// 
			this.datalocations.DialogOffset = new System.Drawing.Point(40, 20);
			this.datalocations.Location = new System.Drawing.Point(14, 26);
			this.datalocations.Name = "datalocations";
			this.datalocations.Size = new System.Drawing.Size(336, 127);
			this.datalocations.TabIndex = 18;
			// 
			// apply
			// 
			this.apply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.apply.Location = new System.Drawing.Point(147, 428);
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
			this.cancel.Location = new System.Drawing.Point(265, 428);
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
			this.mapslist.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
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
			this.mapslist.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.mapslist_ItemSelectionChanged);
			// 
			// OpenMapOptionsForm
			// 
			this.AcceptButton = this.apply;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 14F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.cancel;
			this.ClientSize = new System.Drawing.Size(389, 465);
			this.Controls.Add(this.mapslist);
			this.Controls.Add(label2);
			this.Controls.Add(this.config);
			this.Controls.Add(label1);
			this.Controls.Add(this.cancel);
			this.Controls.Add(this.apply);
			this.Controls.Add(this.panelres);
			this.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "OpenMapOptionsForm";
			this.Opacity = 0;
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

		private System.Windows.Forms.Button apply;
		private System.Windows.Forms.Button cancel;
		private System.Windows.Forms.ComboBox config;
		private System.Windows.Forms.GroupBox panelres;
		private System.Windows.Forms.ListView mapslist;
		private ResourceListEditor datalocations;


	}
}