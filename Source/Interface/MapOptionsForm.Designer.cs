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
			System.Windows.Forms.GroupBox groupBox1;
			System.Windows.Forms.Label label3;
			System.Windows.Forms.Label label2;
			System.Windows.Forms.Label label1;
			System.Windows.Forms.GroupBox groupBox2;
			this.levelname = new System.Windows.Forms.TextBox();
			this.config = new System.Windows.Forms.ComboBox();
			this.button2 = new System.Windows.Forms.Button();
			this.button1 = new System.Windows.Forms.Button();
			this.addresource = new System.Windows.Forms.Button();
			this.resources = new System.Windows.Forms.ListBox();
			this.apply = new System.Windows.Forms.Button();
			this.cancel = new System.Windows.Forms.Button();
			groupBox1 = new System.Windows.Forms.GroupBox();
			label3 = new System.Windows.Forms.Label();
			label2 = new System.Windows.Forms.Label();
			label1 = new System.Windows.Forms.Label();
			groupBox2 = new System.Windows.Forms.GroupBox();
			groupBox1.SuspendLayout();
			groupBox2.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBox1
			// 
			groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			groupBox1.Controls.Add(label3);
			groupBox1.Controls.Add(this.levelname);
			groupBox1.Controls.Add(label2);
			groupBox1.Controls.Add(this.config);
			groupBox1.Controls.Add(label1);
			groupBox1.Location = new System.Drawing.Point(12, 12);
			groupBox1.Name = "groupBox1";
			groupBox1.Size = new System.Drawing.Size(365, 118);
			groupBox1.TabIndex = 10;
			groupBox1.TabStop = false;
			groupBox1.Text = " Settings ";
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
			// levelname
			// 
			this.levelname.Location = new System.Drawing.Point(129, 73);
			this.levelname.Name = "levelname";
			this.levelname.Size = new System.Drawing.Size(94, 20);
			this.levelname.TabIndex = 8;
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
			// config
			// 
			this.config.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.config.FormattingEnabled = true;
			this.config.Location = new System.Drawing.Point(129, 31);
			this.config.Name = "config";
			this.config.Size = new System.Drawing.Size(213, 22);
			this.config.TabIndex = 6;
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
			// groupBox2
			// 
			groupBox2.Controls.Add(this.button2);
			groupBox2.Controls.Add(this.button1);
			groupBox2.Controls.Add(this.addresource);
			groupBox2.Controls.Add(this.resources);
			groupBox2.Location = new System.Drawing.Point(12, 145);
			groupBox2.Name = "groupBox2";
			groupBox2.Size = new System.Drawing.Size(365, 165);
			groupBox2.TabIndex = 11;
			groupBox2.TabStop = false;
			groupBox2.Text = " Custom Resources ";
			// 
			// button2
			// 
			this.button2.Location = new System.Drawing.Point(268, 125);
			this.button2.Name = "button2";
			this.button2.Size = new System.Drawing.Size(74, 25);
			this.button2.TabIndex = 13;
			this.button2.Text = "Remove";
			this.button2.UseVisualStyleBackColor = true;
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(139, 125);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(123, 25);
			this.button1.TabIndex = 12;
			this.button1.Text = "Resource Options...";
			this.button1.UseVisualStyleBackColor = true;
			// 
			// addresource
			// 
			this.addresource.Location = new System.Drawing.Point(21, 125);
			this.addresource.Name = "addresource";
			this.addresource.Size = new System.Drawing.Size(112, 25);
			this.addresource.TabIndex = 11;
			this.addresource.Text = "Add Resource...";
			this.addresource.UseVisualStyleBackColor = true;
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
			this.Controls.Add(groupBox2);
			this.Controls.Add(groupBox1);
			this.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "MapOptionsForm";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Map Options";
			groupBox1.ResumeLayout(false);
			groupBox1.PerformLayout();
			groupBox2.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TextBox levelname;
		private System.Windows.Forms.ComboBox config;
		private System.Windows.Forms.Button button2;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.Button addresource;
		private System.Windows.Forms.ListBox resources;
		private System.Windows.Forms.Button apply;
		private System.Windows.Forms.Button cancel;


	}
}