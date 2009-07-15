namespace CodeImp.DoomBuilder.MassUndoRedo
{
	partial class UndoRedoForm
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
			this.list = new System.Windows.Forms.ListView();
			this.coldescription = new System.Windows.Forms.ColumnHeader();
			this.pinned = new System.Windows.Forms.CheckBox();
			this.SuspendLayout();
			// 
			// list
			// 
			this.list.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)));
			this.list.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.coldescription});
			this.list.FullRowSelect = true;
			this.list.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
			this.list.HideSelection = false;
			this.list.Location = new System.Drawing.Point(12, 14);
			this.list.MultiSelect = false;
			this.list.Name = "list";
			this.list.ShowGroups = false;
			this.list.Size = new System.Drawing.Size(267, 579);
			this.list.TabIndex = 0;
			this.list.UseCompatibleStateImageBehavior = false;
			this.list.View = System.Windows.Forms.View.Details;
			this.list.SelectedIndexChanged += new System.EventHandler(this.list_SelectedIndexChanged);
			this.list.MouseUp += new System.Windows.Forms.MouseEventHandler(this.list_MouseUp);
			this.list.KeyUp += new System.Windows.Forms.KeyEventHandler(this.list_KeyUp);
			// 
			// coldescription
			// 
			this.coldescription.Text = "Description";
			this.coldescription.Width = 238;
			// 
			// pinned
			// 
			this.pinned.Appearance = System.Windows.Forms.Appearance.Button;
			this.pinned.Image = global::CodeImp.DoomBuilder.MassUndoRedo.Properties.Resources.Pushpin;
			this.pinned.Location = new System.Drawing.Point(285, 14);
			this.pinned.Name = "pinned";
			this.pinned.Size = new System.Drawing.Size(30, 30);
			this.pinned.TabIndex = 2;
			this.pinned.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
			this.pinned.UseVisualStyleBackColor = true;
			// 
			// UndoRedoForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.ClientSize = new System.Drawing.Size(325, 605);
			this.Controls.Add(this.pinned);
			this.Controls.Add(this.list);
			this.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "UndoRedoForm";
			this.Opacity = 0;
			this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
			this.Text = "UndoRedoForm";
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.ListView list;
		private System.Windows.Forms.ColumnHeader coldescription;
		private System.Windows.Forms.CheckBox pinned;
	}
}