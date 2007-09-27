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
			System.Windows.Forms.Label label1;
			System.Windows.Forms.GroupBox groupBox1;
			System.Windows.Forms.Label label4;
			System.Windows.Forms.Label label3;
			System.Windows.Forms.Label label2;
			this.configbuildonsave = new System.Windows.Forms.CheckBox();
			this.confignodebuilder = new System.Windows.Forms.ComboBox();
			this.tabs = new System.Windows.Forms.TabControl();
			this.tabinterface = new System.Windows.Forms.TabPage();
			this.tabediting = new System.Windows.Forms.TabPage();
			this.tabconfigs = new System.Windows.Forms.TabPage();
			this.listconfigs = new System.Windows.Forms.ListBox();
			this.cancel = new System.Windows.Forms.Button();
			this.apply = new System.Windows.Forms.Button();
			label1 = new System.Windows.Forms.Label();
			groupBox1 = new System.Windows.Forms.GroupBox();
			label4 = new System.Windows.Forms.Label();
			label3 = new System.Windows.Forms.Label();
			label2 = new System.Windows.Forms.Label();
			groupBox1.SuspendLayout();
			this.tabs.SuspendLayout();
			this.tabconfigs.SuspendLayout();
			this.SuspendLayout();
			// 
			// label1
			// 
			label1.Location = new System.Drawing.Point(16, 25);
			label1.Name = "label1";
			label1.Size = new System.Drawing.Size(303, 46);
			label1.TabIndex = 1;
			label1.Text = "Select the nodebuilder options to use with this configuration.\r\nThe nodebuilder i" +
				"s a compiler that builds geometry structures in your map when saved and when usi" +
				"ng 3D mode.";
			// 
			// groupBox1
			// 
			groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			groupBox1.Controls.Add(label4);
			groupBox1.Controls.Add(label3);
			groupBox1.Controls.Add(this.configbuildonsave);
			groupBox1.Controls.Add(label2);
			groupBox1.Controls.Add(this.confignodebuilder);
			groupBox1.Controls.Add(label1);
			groupBox1.Location = new System.Drawing.Point(205, 11);
			groupBox1.Name = "groupBox1";
			groupBox1.Size = new System.Drawing.Size(343, 270);
			groupBox1.TabIndex = 2;
			groupBox1.TabStop = false;
			groupBox1.Text = " Configuration Settings ";
			// 
			// label4
			// 
			label4.AutoSize = true;
			label4.Location = new System.Drawing.Point(29, 201);
			label4.Name = "label4";
			label4.Size = new System.Drawing.Size(79, 14);
			label4.TabIndex = 6;
			label4.Text = "IWAD wad file:";
			// 
			// label3
			// 
			label3.Location = new System.Drawing.Point(16, 144);
			label3.Name = "label3";
			label3.Size = new System.Drawing.Size(303, 46);
			label3.TabIndex = 5;
			label3.Text = "Specify the IWAD wad file to use for this configuration.\r\nThe IWAD should contain" +
				" all default resources. such as textures and sprites, for the original game.";
			// 
			// configbuildonsave
			// 
			this.configbuildonsave.AutoSize = true;
			this.configbuildonsave.Location = new System.Drawing.Point(102, 111);
			this.configbuildonsave.Name = "configbuildonsave";
			this.configbuildonsave.Size = new System.Drawing.Size(201, 18);
			this.configbuildonsave.TabIndex = 4;
			this.configbuildonsave.Text = "Build nodes every time when saving";
			this.configbuildonsave.UseVisualStyleBackColor = true;
			// 
			// label2
			// 
			label2.AutoSize = true;
			label2.Location = new System.Drawing.Point(29, 81);
			label2.Name = "label2";
			label2.Size = new System.Drawing.Size(67, 14);
			label2.TabIndex = 3;
			label2.Text = "Nodebuilder:";
			// 
			// confignodebuilder
			// 
			this.confignodebuilder.FormattingEnabled = true;
			this.confignodebuilder.Location = new System.Drawing.Point(102, 78);
			this.confignodebuilder.Name = "confignodebuilder";
			this.confignodebuilder.Size = new System.Drawing.Size(217, 22);
			this.confignodebuilder.TabIndex = 2;
			// 
			// tabs
			// 
			this.tabs.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.tabs.Controls.Add(this.tabinterface);
			this.tabs.Controls.Add(this.tabediting);
			this.tabs.Controls.Add(this.tabconfigs);
			this.tabs.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.tabs.ItemSize = new System.Drawing.Size(110, 19);
			this.tabs.Location = new System.Drawing.Point(12, 12);
			this.tabs.Name = "tabs";
			this.tabs.SelectedIndex = 0;
			this.tabs.Size = new System.Drawing.Size(566, 320);
			this.tabs.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
			this.tabs.TabIndex = 0;
			// 
			// tabinterface
			// 
			this.tabinterface.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.tabinterface.Location = new System.Drawing.Point(4, 23);
			this.tabinterface.Name = "tabinterface";
			this.tabinterface.Padding = new System.Windows.Forms.Padding(3);
			this.tabinterface.Size = new System.Drawing.Size(558, 293);
			this.tabinterface.TabIndex = 0;
			this.tabinterface.Text = "Interface";
			this.tabinterface.UseVisualStyleBackColor = true;
			// 
			// tabediting
			// 
			this.tabediting.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.tabediting.Location = new System.Drawing.Point(4, 23);
			this.tabediting.Name = "tabediting";
			this.tabediting.Padding = new System.Windows.Forms.Padding(3);
			this.tabediting.Size = new System.Drawing.Size(558, 293);
			this.tabediting.TabIndex = 1;
			this.tabediting.Text = "Editing";
			this.tabediting.UseVisualStyleBackColor = true;
			// 
			// tabconfigs
			// 
			this.tabconfigs.Controls.Add(groupBox1);
			this.tabconfigs.Controls.Add(this.listconfigs);
			this.tabconfigs.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.tabconfigs.Location = new System.Drawing.Point(4, 23);
			this.tabconfigs.Name = "tabconfigs";
			this.tabconfigs.Size = new System.Drawing.Size(558, 293);
			this.tabconfigs.TabIndex = 2;
			this.tabconfigs.Text = "Configurations";
			this.tabconfigs.UseVisualStyleBackColor = true;
			// 
			// listconfigs
			// 
			this.listconfigs.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)));
			this.listconfigs.FormattingEnabled = true;
			this.listconfigs.ItemHeight = 14;
			this.listconfigs.Location = new System.Drawing.Point(11, 11);
			this.listconfigs.Name = "listconfigs";
			this.listconfigs.Size = new System.Drawing.Size(181, 270);
			this.listconfigs.TabIndex = 0;
			// 
			// cancel
			// 
			this.cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancel.Location = new System.Drawing.Point(466, 346);
			this.cancel.Name = "cancel";
			this.cancel.Size = new System.Drawing.Size(112, 25);
			this.cancel.TabIndex = 17;
			this.cancel.Text = "Cancel";
			this.cancel.UseVisualStyleBackColor = true;
			// 
			// apply
			// 
			this.apply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.apply.Location = new System.Drawing.Point(348, 346);
			this.apply.Name = "apply";
			this.apply.Size = new System.Drawing.Size(112, 25);
			this.apply.TabIndex = 16;
			this.apply.Text = "OK";
			this.apply.UseVisualStyleBackColor = true;
			// 
			// ConfigForm
			// 
			this.AcceptButton = this.apply;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 14F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.cancel;
			this.ClientSize = new System.Drawing.Size(590, 382);
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
			groupBox1.ResumeLayout(false);
			groupBox1.PerformLayout();
			this.tabs.ResumeLayout(false);
			this.tabconfigs.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TabControl tabs;
		private System.Windows.Forms.TabPage tabinterface;
		private System.Windows.Forms.TabPage tabediting;
		private System.Windows.Forms.TabPage tabconfigs;
		private System.Windows.Forms.Button cancel;
		private System.Windows.Forms.Button apply;
		private System.Windows.Forms.ListBox listconfigs;
		private System.Windows.Forms.ComboBox confignodebuilder;
		private System.Windows.Forms.CheckBox configbuildonsave;
	}
}