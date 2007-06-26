namespace CodeImp.DoomBuilder.Interface
{
	partial class MapOptionsForm
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
			System.Windows.Forms.Label label3;
			System.Windows.Forms.Label label2;
			System.Windows.Forms.Label label1;
			System.Windows.Forms.GroupBox panelsettings;
			System.Windows.Forms.GroupBox panelres;
			this.levelname = new System.Windows.Forms.TextBox();
			this.config = new System.Windows.Forms.ComboBox();
			this.deleteresource = new System.Windows.Forms.Button();
			this.editresource = new System.Windows.Forms.Button();
			this.addresource = new System.Windows.Forms.Button();
			this.resources = new System.Windows.Forms.ListBox();
			this.apply = new System.Windows.Forms.Button();
			this.cancel = new System.Windows.Forms.Button();
			label3 = new System.Windows.Forms.Label();
			label2 = new System.Windows.Forms.Label();
			label1 = new System.Windows.Forms.Label();
			panelsettings = new System.Windows.Forms.GroupBox();
			panelres = new System.Windows.Forms.GroupBox();
			panelsettings.SuspendLayout();
			panelres.SuspendLayout();
			this.SuspendLayout();
			// 
			// label3
			// 
			label3.AutoSize = true;
			label3.Location = new System.Drawing.Point(239, 76);
			label3.Name = "label3";
			label3.Size = new System.Drawing.Size(90, 14);
			label3.TabIndex = 9;
			label3.Text = "example:  MAP01";
			// 
			// label2
			// 
			label2.AutoSize = true;
			label2.Location = new System.Drawing.Point(58, 76);
			label2.Name = "label2";
			label2.Size = new System.Drawing.Size(65, 14);
			label2.TabIndex = 7;
			label2.Text = "Level name:";
			// 
			// label1
			// 
			label1.AutoSize = true;
			label1.Location = new System.Drawing.Point(18, 34);
			label1.Name = "label1";
			label1.Size = new System.Drawing.Size(105, 14);
			label1.TabIndex = 5;
			label1.Text = "Game Configuration:";
			// 
			// panelsettings
			// 
			panelsettings.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			panelsettings.Controls.Add(label3);
			panelsettings.Controls.Add(this.levelname);
			panelsettings.Controls.Add(label2);
			panelsettings.Controls.Add(this.config);
			panelsettings.Controls.Add(label1);
			panelsettings.Location = new System.Drawing.Point(12, 12);
			panelsettings.Name = "panelsettings";
			panelsettings.Size = new System.Drawing.Size(365, 118);
			panelsettings.TabIndex = 10;
			panelsettings.TabStop = false;
			panelsettings.Text = " Settings ";
			// 
			// levelname
			// 
			this.levelname.Location = new System.Drawing.Point(129, 73);
			this.levelname.Name = "levelname";
			this.levelname.Size = new System.Drawing.Size(94, 20);
			this.levelname.TabIndex = 8;
			// 
			// config
			// 
			this.config.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.config.FormattingEnabled = true;
			this.config.Location = new System.Drawing.Point(129, 31);
			this.config.Name = "config";
			this.config.Size = new System.Drawing.Size(213, 22);
			this.config.TabIndex = 6;
			// 
			// panelres
			// 
			panelres.Controls.Add(this.deleteresource);
			panelres.Controls.Add(this.editresource);
			panelres.Controls.Add(this.addresource);
			panelres.Controls.Add(this.resources);
			panelres.Location = new System.Drawing.Point(12, 145);
			panelres.Name = "panelres";
			panelres.Size = new System.Drawing.Size(365, 165);
			panelres.TabIndex = 11;
			panelres.TabStop = false;
			panelres.Text = " Custom Resources ";
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
			// apply
			// 
			this.apply.Location = new System.Drawing.Point(147, 330);
			this.apply.Name = "apply";
			this.apply.Size = new System.Drawing.Size(112, 25);
			this.apply.TabIndex = 12;
			this.apply.Text = "OK";
			this.apply.UseVisualStyleBackColor = true;
			this.apply.Click += new System.EventHandler(this.apply_Click);
			// 
			// cancel
			// 
			this.cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancel.Location = new System.Drawing.Point(265, 330);
			this.cancel.Name = "cancel";
			this.cancel.Size = new System.Drawing.Size(112, 25);
			this.cancel.TabIndex = 13;
			this.cancel.Text = "Cancel";
			this.cancel.UseVisualStyleBackColor = true;
			this.cancel.Click += new System.EventHandler(this.cancel_Click);
			// 
			// MapOptionsForm
			// 
			this.AcceptButton = this.apply;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 14F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.cancel;
			this.ClientSize = new System.Drawing.Size(389, 367);
			this.Controls.Add(this.cancel);
			this.Controls.Add(this.apply);
			this.Controls.Add(panelres);
			this.Controls.Add(panelsettings);
			this.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "MapOptionsForm";
			this.Opacity = 0;
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Map Options";
			panelsettings.ResumeLayout(false);
			panelsettings.PerformLayout();
			panelres.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TextBox levelname;
		private System.Windows.Forms.ComboBox config;
		private System.Windows.Forms.Button deleteresource;
		private System.Windows.Forms.Button editresource;
		private System.Windows.Forms.Button addresource;
		private System.Windows.Forms.ListBox resources;
		private System.Windows.Forms.Button apply;
		private System.Windows.Forms.Button cancel;


	}
}