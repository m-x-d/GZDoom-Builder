namespace CodeImp.DoomBuilder.Interface
{
	partial class FieldsEditorControl
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

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle16 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle13 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle14 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle15 = new System.Windows.Forms.DataGridViewCellStyle();
			this.fieldslist = new System.Windows.Forms.DataGridView();
			this.deleterowstimer = new System.Windows.Forms.Timer(this.components);
			this.browsebutton = new System.Windows.Forms.Button();
			this.fieldname = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.fieldtype = new System.Windows.Forms.DataGridViewComboBoxColumn();
			this.fieldvalue = new System.Windows.Forms.DataGridViewTextBoxColumn();
			((System.ComponentModel.ISupportInitialize)(this.fieldslist)).BeginInit();
			this.SuspendLayout();
			// 
			// fieldslist
			// 
			this.fieldslist.AllowUserToResizeColumns = false;
			this.fieldslist.AllowUserToResizeRows = false;
			this.fieldslist.BackgroundColor = System.Drawing.SystemColors.Window;
			this.fieldslist.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.fieldslist.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
			this.fieldslist.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.Disable;
			this.fieldslist.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
			this.fieldslist.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.fieldslist.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.fieldname,
            this.fieldtype,
            this.fieldvalue});
			dataGridViewCellStyle16.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle16.BackColor = System.Drawing.SystemColors.Window;
			dataGridViewCellStyle16.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			dataGridViewCellStyle16.ForeColor = System.Drawing.SystemColors.WindowText;
			dataGridViewCellStyle16.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle16.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle16.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
			this.fieldslist.DefaultCellStyle = dataGridViewCellStyle16;
			this.fieldslist.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
			this.fieldslist.Location = new System.Drawing.Point(0, 0);
			this.fieldslist.MultiSelect = false;
			this.fieldslist.Name = "fieldslist";
			this.fieldslist.RowHeadersVisible = false;
			this.fieldslist.RowTemplate.DefaultCellStyle.BackColor = System.Drawing.SystemColors.Window;
			this.fieldslist.RowTemplate.DefaultCellStyle.ForeColor = System.Drawing.SystemColors.WindowText;
			this.fieldslist.RowTemplate.DefaultCellStyle.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			this.fieldslist.RowTemplate.DefaultCellStyle.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			this.fieldslist.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.fieldslist.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
			this.fieldslist.Size = new System.Drawing.Size(444, 244);
			this.fieldslist.TabIndex = 1;
			this.fieldslist.UserDeletingRow += new System.Windows.Forms.DataGridViewRowCancelEventHandler(this.fieldslist_UserDeletingRow);
			this.fieldslist.CellBeginEdit += new System.Windows.Forms.DataGridViewCellCancelEventHandler(this.fieldslist_CellBeginEdit);
			this.fieldslist.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.fieldslist_CellClick);
			this.fieldslist.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.fieldslist_CellEndEdit);
			this.fieldslist.SelectionChanged += new System.EventHandler(this.fieldslist_SelectionChanged);
			// 
			// deleterowstimer
			// 
			this.deleterowstimer.Interval = 1;
			this.deleterowstimer.Tick += new System.EventHandler(this.deleterowstimer_Tick);
			// 
			// browsebutton
			// 
			this.browsebutton.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.browsebutton.Image = global::CodeImp.DoomBuilder.Properties.Resources.treeview;
			this.browsebutton.ImageAlign = System.Drawing.ContentAlignment.BottomCenter;
			this.browsebutton.Location = new System.Drawing.Point(370, 43);
			this.browsebutton.Name = "browsebutton";
			this.browsebutton.Size = new System.Drawing.Size(30, 24);
			this.browsebutton.TabIndex = 2;
			this.browsebutton.UseVisualStyleBackColor = true;
			this.browsebutton.Visible = false;
			// 
			// fieldname
			// 
			dataGridViewCellStyle13.BackColor = System.Drawing.SystemColors.Window;
			dataGridViewCellStyle13.ForeColor = System.Drawing.SystemColors.WindowText;
			dataGridViewCellStyle13.NullValue = null;
			dataGridViewCellStyle13.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle13.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			this.fieldname.DefaultCellStyle = dataGridViewCellStyle13;
			this.fieldname.Frozen = true;
			this.fieldname.HeaderText = "Property";
			this.fieldname.Name = "fieldname";
			this.fieldname.Width = 180;
			// 
			// fieldtype
			// 
			dataGridViewCellStyle14.BackColor = System.Drawing.SystemColors.Window;
			dataGridViewCellStyle14.ForeColor = System.Drawing.SystemColors.WindowText;
			dataGridViewCellStyle14.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle14.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			this.fieldtype.DefaultCellStyle = dataGridViewCellStyle14;
			this.fieldtype.DisplayStyle = System.Windows.Forms.DataGridViewComboBoxDisplayStyle.Nothing;
			this.fieldtype.HeaderText = "Type";
			this.fieldtype.Name = "fieldtype";
			this.fieldtype.Resizable = System.Windows.Forms.DataGridViewTriState.True;
			// 
			// fieldvalue
			// 
			dataGridViewCellStyle15.BackColor = System.Drawing.SystemColors.Window;
			dataGridViewCellStyle15.ForeColor = System.Drawing.SystemColors.WindowText;
			dataGridViewCellStyle15.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle15.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			this.fieldvalue.DefaultCellStyle = dataGridViewCellStyle15;
			this.fieldvalue.HeaderText = "Value";
			this.fieldvalue.Name = "fieldvalue";
			this.fieldvalue.Resizable = System.Windows.Forms.DataGridViewTriState.True;
			this.fieldvalue.Width = 120;
			// 
			// FieldsEditorControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.browsebutton);
			this.Controls.Add(this.fieldslist);
			this.Name = "FieldsEditorControl";
			this.Size = new System.Drawing.Size(474, 266);
			this.Layout += new System.Windows.Forms.LayoutEventHandler(this.FieldsEditorControl_Layout);
			this.Resize += new System.EventHandler(this.FieldsEditorControl_Resize);
			((System.ComponentModel.ISupportInitialize)(this.fieldslist)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.DataGridView fieldslist;
		private System.Windows.Forms.Timer deleterowstimer;
		private System.Windows.Forms.Button browsebutton;
		private System.Windows.Forms.DataGridViewTextBoxColumn fieldname;
		private System.Windows.Forms.DataGridViewComboBoxColumn fieldtype;
		private System.Windows.Forms.DataGridViewTextBoxColumn fieldvalue;
	}
}
