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
			System.Windows.Forms.GroupBox groupBox1;
			System.Windows.Forms.Label label2;
			System.Windows.Forms.Label label3;
			System.Windows.Forms.GroupBox groupBox2;
			System.Windows.Forms.Label label1;
			System.Windows.Forms.Label label4;
			this.configbuildonsave = new System.Windows.Forms.CheckBox();
			this.confignodebuilder = new System.Windows.Forms.ComboBox();
			this.tabs = new System.Windows.Forms.TabControl();
			this.tabinterface = new System.Windows.Forms.TabPage();
			this.tabediting = new System.Windows.Forms.TabPage();
			this.tabconfigs = new System.Windows.Forms.TabPage();
			this.panelres = new System.Windows.Forms.GroupBox();
			this.resourcelocations = new CodeImp.DoomBuilder.Interface.ResourceListEditor();
			this.listconfigs = new System.Windows.Forms.ListBox();
			this.cancel = new System.Windows.Forms.Button();
			this.apply = new System.Windows.Forms.Button();
			this.browsewad = new System.Windows.Forms.Button();
			this.wadlocation = new System.Windows.Forms.TextBox();
			this.textBox1 = new System.Windows.Forms.TextBox();
			this.button1 = new System.Windows.Forms.Button();
			groupBox1 = new System.Windows.Forms.GroupBox();
			label2 = new System.Windows.Forms.Label();
			label3 = new System.Windows.Forms.Label();
			groupBox2 = new System.Windows.Forms.GroupBox();
			label1 = new System.Windows.Forms.Label();
			label4 = new System.Windows.Forms.Label();
			groupBox1.SuspendLayout();
			this.tabs.SuspendLayout();
			this.tabconfigs.SuspendLayout();
			this.panelres.SuspendLayout();
			groupBox2.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBox1
			// 
			groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			groupBox1.Controls.Add(this.configbuildonsave);
			groupBox1.Controls.Add(label2);
			groupBox1.Controls.Add(this.confignodebuilder);
			groupBox1.Location = new System.Drawing.Point(235, 154);
			groupBox1.Name = "groupBox1";
			groupBox1.Size = new System.Drawing.Size(342, 97);
			groupBox1.TabIndex = 2;
			groupBox1.TabStop = false;
			groupBox1.Text = " Nodebuilder";
			// 
			// configbuildonsave
			// 
			this.configbuildonsave.AutoSize = true;
			this.configbuildonsave.Location = new System.Drawing.Point(49, 62);
			this.configbuildonsave.Name = "configbuildonsave";
			this.configbuildonsave.Size = new System.Drawing.Size(242, 18);
			this.configbuildonsave.TabIndex = 4;
			this.configbuildonsave.Text = "Build nodes every time when saving the map";
			this.configbuildonsave.UseVisualStyleBackColor = true;
			// 
			// label2
			// 
			label2.AutoSize = true;
			label2.Location = new System.Drawing.Point(25, 31);
			label2.Name = "label2";
			label2.Size = new System.Drawing.Size(74, 14);
			label2.TabIndex = 3;
			label2.Text = "Configuration:";
			// 
			// confignodebuilder
			// 
			this.confignodebuilder.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.confignodebuilder.FormattingEnabled = true;
			this.confignodebuilder.Location = new System.Drawing.Point(105, 28);
			this.confignodebuilder.Name = "confignodebuilder";
			this.confignodebuilder.Size = new System.Drawing.Size(186, 22);
			this.confignodebuilder.TabIndex = 2;
			// 
			// label3
			// 
			label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			label3.Location = new System.Drawing.Point(14, 112);
			label3.Name = "label3";
			label3.Size = new System.Drawing.Size(322, 22);
			label3.TabIndex = 17;
			label3.Text = "Drag items to change order (lower items override higher items).";
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
			this.tabs.Size = new System.Drawing.Size(595, 399);
			this.tabs.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
			this.tabs.TabIndex = 0;
			// 
			// tabinterface
			// 
			this.tabinterface.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.tabinterface.Location = new System.Drawing.Point(4, 23);
			this.tabinterface.Name = "tabinterface";
			this.tabinterface.Padding = new System.Windows.Forms.Padding(3);
			this.tabinterface.Size = new System.Drawing.Size(587, 324);
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
			this.tabediting.Size = new System.Drawing.Size(587, 324);
			this.tabediting.TabIndex = 1;
			this.tabediting.Text = "Editing";
			this.tabediting.UseVisualStyleBackColor = true;
			// 
			// tabconfigs
			// 
			this.tabconfigs.Controls.Add(groupBox2);
			this.tabconfigs.Controls.Add(this.panelres);
			this.tabconfigs.Controls.Add(groupBox1);
			this.tabconfigs.Controls.Add(this.listconfigs);
			this.tabconfigs.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.tabconfigs.Location = new System.Drawing.Point(4, 23);
			this.tabconfigs.Name = "tabconfigs";
			this.tabconfigs.Size = new System.Drawing.Size(587, 372);
			this.tabconfigs.TabIndex = 2;
			this.tabconfigs.Text = "Configurations";
			this.tabconfigs.UseVisualStyleBackColor = true;
			// 
			// panelres
			// 
			this.panelres.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.panelres.Controls.Add(this.resourcelocations);
			this.panelres.Controls.Add(label3);
			this.panelres.Location = new System.Drawing.Point(235, 11);
			this.panelres.Name = "panelres";
			this.panelres.Size = new System.Drawing.Size(342, 137);
			this.panelres.TabIndex = 12;
			this.panelres.TabStop = false;
			this.panelres.Text = " Resources ";
			// 
			// resourcelocations
			// 
			this.resourcelocations.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.resourcelocations.DialogOffset = new System.Drawing.Point(-120, -80);
			this.resourcelocations.Location = new System.Drawing.Point(14, 28);
			this.resourcelocations.Name = "resourcelocations";
			this.resourcelocations.Size = new System.Drawing.Size(313, 81);
			this.resourcelocations.TabIndex = 18;
			// 
			// listconfigs
			// 
			this.listconfigs.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)));
			this.listconfigs.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.listconfigs.FormattingEnabled = true;
			this.listconfigs.IntegralHeight = false;
			this.listconfigs.ItemHeight = 14;
			this.listconfigs.Location = new System.Drawing.Point(11, 11);
			this.listconfigs.Name = "listconfigs";
			this.listconfigs.Size = new System.Drawing.Size(212, 348);
			this.listconfigs.Sorted = true;
			this.listconfigs.TabIndex = 0;
			this.listconfigs.SelectedIndexChanged += new System.EventHandler(this.listconfigs_SelectedIndexChanged);
			// 
			// cancel
			// 
			this.cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancel.Location = new System.Drawing.Point(495, 425);
			this.cancel.Name = "cancel";
			this.cancel.Size = new System.Drawing.Size(112, 25);
			this.cancel.TabIndex = 17;
			this.cancel.Text = "Cancel";
			this.cancel.UseVisualStyleBackColor = true;
			// 
			// apply
			// 
			this.apply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.apply.Location = new System.Drawing.Point(377, 425);
			this.apply.Name = "apply";
			this.apply.Size = new System.Drawing.Size(112, 25);
			this.apply.TabIndex = 16;
			this.apply.Text = "OK";
			this.apply.UseVisualStyleBackColor = true;
			// 
			// groupBox2
			// 
			groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			groupBox2.Controls.Add(this.button1);
			groupBox2.Controls.Add(this.textBox1);
			groupBox2.Controls.Add(label4);
			groupBox2.Controls.Add(this.browsewad);
			groupBox2.Controls.Add(this.wadlocation);
			groupBox2.Controls.Add(label1);
			groupBox2.Location = new System.Drawing.Point(235, 257);
			groupBox2.Name = "groupBox2";
			groupBox2.Size = new System.Drawing.Size(342, 102);
			groupBox2.TabIndex = 13;
			groupBox2.TabStop = false;
			groupBox2.Text = " Testing ";
			// 
			// label1
			// 
			label1.AutoSize = true;
			label1.Location = new System.Drawing.Point(25, 32);
			label1.Name = "label1";
			label1.Size = new System.Drawing.Size(63, 14);
			label1.TabIndex = 4;
			label1.Text = "Application:";
			// 
			// browsewad
			// 
			this.browsewad.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.browsewad.Location = new System.Drawing.Point(297, 28);
			this.browsewad.Name = "browsewad";
			this.browsewad.Size = new System.Drawing.Size(30, 23);
			this.browsewad.TabIndex = 6;
			this.browsewad.Text = "...";
			this.browsewad.UseVisualStyleBackColor = true;
			// 
			// wadlocation
			// 
			this.wadlocation.Location = new System.Drawing.Point(94, 29);
			this.wadlocation.Name = "wadlocation";
			this.wadlocation.ReadOnly = true;
			this.wadlocation.Size = new System.Drawing.Size(197, 20);
			this.wadlocation.TabIndex = 5;
			// 
			// label4
			// 
			label4.AutoSize = true;
			label4.Location = new System.Drawing.Point(23, 67);
			label4.Name = "label4";
			label4.Size = new System.Drawing.Size(65, 14);
			label4.TabIndex = 7;
			label4.Text = "Parameters:";
			// 
			// textBox1
			// 
			this.textBox1.Location = new System.Drawing.Point(94, 64);
			this.textBox1.Name = "textBox1";
			this.textBox1.Size = new System.Drawing.Size(197, 20);
			this.textBox1.TabIndex = 8;
			// 
			// button1
			// 
			this.button1.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.button1.Location = new System.Drawing.Point(297, 63);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(30, 23);
			this.button1.TabIndex = 9;
			this.button1.Text = "...";
			this.button1.UseVisualStyleBackColor = true;
			// 
			// ConfigForm
			// 
			this.AcceptButton = this.apply;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 14F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.cancel;
			this.ClientSize = new System.Drawing.Size(619, 461);
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
			this.panelres.ResumeLayout(false);
			groupBox2.ResumeLayout(false);
			groupBox2.PerformLayout();
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
		private System.Windows.Forms.GroupBox panelres;
		private ResourceListEditor resourcelocations;
		private System.Windows.Forms.Button browsewad;
		private System.Windows.Forms.TextBox wadlocation;
		private System.Windows.Forms.TextBox textBox1;
		private System.Windows.Forms.Button button1;
	}
}