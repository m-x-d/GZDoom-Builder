namespace CodeImp.DoomBuilder.Windows
{
	partial class ThingsFiltersForm
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
			this.listfilters = new System.Windows.Forms.ListView();
			this.columnname = new System.Windows.Forms.ColumnHeader();
			this.addfilter = new System.Windows.Forms.Button();
			this.deletefilter = new System.Windows.Forms.Button();
			this.filtergroup = new System.Windows.Forms.GroupBox();
			this.filterfields = new CodeImp.DoomBuilder.Controls.CheckboxArrayControl();
			this.label3 = new System.Windows.Forms.Label();
			this.filtercategory = new System.Windows.Forms.ComboBox();
			this.label2 = new System.Windows.Forms.Label();
			this.filtername = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.cancel = new System.Windows.Forms.Button();
			this.apply = new System.Windows.Forms.Button();
			this.filtergroup.SuspendLayout();
			this.SuspendLayout();
			// 
			// listfilters
			// 
			this.listfilters.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)));
			this.listfilters.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnname});
			this.listfilters.FullRowSelect = true;
			this.listfilters.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
			this.listfilters.HideSelection = false;
			this.listfilters.Location = new System.Drawing.Point(12, 12);
			this.listfilters.MultiSelect = false;
			this.listfilters.Name = "listfilters";
			this.listfilters.ShowGroups = false;
			this.listfilters.Size = new System.Drawing.Size(202, 323);
			this.listfilters.Sorting = System.Windows.Forms.SortOrder.Ascending;
			this.listfilters.TabIndex = 0;
			this.listfilters.UseCompatibleStateImageBehavior = false;
			this.listfilters.View = System.Windows.Forms.View.Details;
			this.listfilters.SelectedIndexChanged += new System.EventHandler(this.listfilters_SelectedIndexChanged);
			// 
			// columnname
			// 
			this.columnname.Text = "Configuration";
			this.columnname.Width = 177;
			// 
			// addfilter
			// 
			this.addfilter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.addfilter.Location = new System.Drawing.Point(12, 341);
			this.addfilter.Name = "addfilter";
			this.addfilter.Size = new System.Drawing.Size(98, 25);
			this.addfilter.TabIndex = 1;
			this.addfilter.Text = "New Filter";
			this.addfilter.UseVisualStyleBackColor = true;
			this.addfilter.Click += new System.EventHandler(this.addfilter_Click);
			// 
			// deletefilter
			// 
			this.deletefilter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.deletefilter.Enabled = false;
			this.deletefilter.Location = new System.Drawing.Point(116, 341);
			this.deletefilter.Name = "deletefilter";
			this.deletefilter.Size = new System.Drawing.Size(98, 25);
			this.deletefilter.TabIndex = 2;
			this.deletefilter.Text = "Delete Selected";
			this.deletefilter.UseVisualStyleBackColor = true;
			this.deletefilter.Click += new System.EventHandler(this.deletefilter_Click);
			// 
			// filtergroup
			// 
			this.filtergroup.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.filtergroup.Controls.Add(this.filterfields);
			this.filtergroup.Controls.Add(this.label3);
			this.filtergroup.Controls.Add(this.filtercategory);
			this.filtergroup.Controls.Add(this.label2);
			this.filtergroup.Controls.Add(this.filtername);
			this.filtergroup.Controls.Add(this.label1);
			this.filtergroup.Enabled = false;
			this.filtergroup.Location = new System.Drawing.Point(232, 12);
			this.filtergroup.Name = "filtergroup";
			this.filtergroup.Size = new System.Drawing.Size(382, 354);
			this.filtergroup.TabIndex = 3;
			this.filtergroup.TabStop = false;
			this.filtergroup.Text = " Filter settings ";
			// 
			// filterfields
			// 
			this.filterfields.AutoScroll = true;
			this.filterfields.Columns = 2;
			this.filterfields.Location = new System.Drawing.Point(18, 125);
			this.filterfields.Name = "filterfields";
			this.filterfields.Size = new System.Drawing.Size(329, 198);
			this.filterfields.TabIndex = 2;
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(15, 106);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(89, 14);
			this.label3.TabIndex = 4;
			this.label3.Text = "Filter by settings:";
			// 
			// filtercategory
			// 
			this.filtercategory.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.filtercategory.FormattingEnabled = true;
			this.filtercategory.Location = new System.Drawing.Point(115, 66);
			this.filtercategory.Name = "filtercategory";
			this.filtercategory.Size = new System.Drawing.Size(232, 22);
			this.filtercategory.TabIndex = 1;
			this.filtercategory.SelectedIndexChanged += new System.EventHandler(this.filtercategory_SelectedIndexChanged);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(15, 69);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(94, 14);
			this.label2.TabIndex = 2;
			this.label2.Text = "Filter by category:";
			// 
			// filtername
			// 
			this.filtername.Location = new System.Drawing.Point(115, 28);
			this.filtername.MaxLength = 50;
			this.filtername.Name = "filtername";
			this.filtername.Size = new System.Drawing.Size(232, 20);
			this.filtername.TabIndex = 0;
			this.filtername.Validating += new System.ComponentModel.CancelEventHandler(this.filtername_Validating);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(72, 31);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(37, 14);
			this.label1.TabIndex = 0;
			this.label1.Text = "Name:";
			// 
			// cancel
			// 
			this.cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancel.Location = new System.Drawing.Point(502, 383);
			this.cancel.Name = "cancel";
			this.cancel.Size = new System.Drawing.Size(112, 25);
			this.cancel.TabIndex = 5;
			this.cancel.Text = "Cancel";
			this.cancel.UseVisualStyleBackColor = true;
			this.cancel.Click += new System.EventHandler(this.cancel_Click);
			// 
			// apply
			// 
			this.apply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.apply.Location = new System.Drawing.Point(384, 383);
			this.apply.Name = "apply";
			this.apply.Size = new System.Drawing.Size(112, 25);
			this.apply.TabIndex = 4;
			this.apply.Text = "OK";
			this.apply.UseVisualStyleBackColor = true;
			this.apply.Click += new System.EventHandler(this.apply_Click);
			// 
			// ThingsFiltersForm
			// 
			this.AcceptButton = this.apply;
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.CancelButton = this.cancel;
			this.ClientSize = new System.Drawing.Size(624, 418);
			this.Controls.Add(this.cancel);
			this.Controls.Add(this.apply);
			this.Controls.Add(this.filtergroup);
			this.Controls.Add(this.deletefilter);
			this.Controls.Add(this.addfilter);
			this.Controls.Add(this.listfilters);
			this.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ThingsFiltersForm";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Things Filters";
			this.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.ThingsFiltersForm_HelpRequested);
			this.filtergroup.ResumeLayout(false);
			this.filtergroup.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.ListView listfilters;
		private System.Windows.Forms.ColumnHeader columnname;
		private System.Windows.Forms.Button addfilter;
		private System.Windows.Forms.Button deletefilter;
		private System.Windows.Forms.GroupBox filtergroup;
		private System.Windows.Forms.Button cancel;
		private System.Windows.Forms.Button apply;
		private System.Windows.Forms.TextBox filtername;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.ComboBox filtercategory;
		private System.Windows.Forms.Label label3;
		private CodeImp.DoomBuilder.Controls.CheckboxArrayControl filterfields;
	}
}