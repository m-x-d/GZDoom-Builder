namespace CodeImp.DoomBuilder.BuilderModes
{
	partial class FindReplaceForm
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
			System.Windows.Forms.Label label2;
			System.Windows.Forms.Label label3;
			this.searchtypes = new System.Windows.Forms.ComboBox();
			this.findinput = new System.Windows.Forms.TextBox();
			this.browsefind = new System.Windows.Forms.Button();
			this.withinselection = new System.Windows.Forms.CheckBox();
			this.keepselection = new System.Windows.Forms.CheckBox();
			this.browsereplace = new System.Windows.Forms.Button();
			this.replaceinput = new System.Windows.Forms.TextBox();
			this.findbutton = new System.Windows.Forms.Button();
			this.closebutton = new System.Windows.Forms.Button();
			this.resultslist = new System.Windows.Forms.ListBox();
			this.resultscount = new System.Windows.Forms.Label();
			this.resultspanel = new System.Windows.Forms.Panel();
			this.groupreplace = new System.Windows.Forms.GroupBox();
			this.doreplace = new System.Windows.Forms.CheckBox();
			label1 = new System.Windows.Forms.Label();
			label2 = new System.Windows.Forms.Label();
			label3 = new System.Windows.Forms.Label();
			this.resultspanel.SuspendLayout();
			this.groupreplace.SuspendLayout();
			this.SuspendLayout();
			// 
			// label1
			// 
			label1.AutoSize = true;
			label1.Location = new System.Drawing.Point(25, 15);
			label1.Name = "label1";
			label1.Size = new System.Drawing.Size(69, 14);
			label1.TabIndex = 0;
			label1.Text = "Search type:";
			// 
			// label2
			// 
			label2.AutoSize = true;
			label2.Location = new System.Drawing.Point(36, 51);
			label2.Name = "label2";
			label2.Size = new System.Drawing.Size(58, 14);
			label2.TabIndex = 2;
			label2.Text = "Find what:";
			// 
			// label3
			// 
			label3.AutoSize = true;
			label3.Location = new System.Drawing.Point(12, 28);
			label3.Name = "label3";
			label3.Size = new System.Drawing.Size(73, 14);
			label3.TabIndex = 6;
			label3.Text = "Replace with:";
			// 
			// searchtypes
			// 
			this.searchtypes.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.searchtypes.FormattingEnabled = true;
			this.searchtypes.Location = new System.Drawing.Point(100, 12);
			this.searchtypes.Name = "searchtypes";
			this.searchtypes.Size = new System.Drawing.Size(139, 22);
			this.searchtypes.TabIndex = 1;
			this.searchtypes.SelectedIndexChanged += new System.EventHandler(this.searchtypes_SelectedIndexChanged);
			// 
			// findinput
			// 
			this.findinput.Location = new System.Drawing.Point(100, 48);
			this.findinput.Name = "findinput";
			this.findinput.Size = new System.Drawing.Size(106, 20);
			this.findinput.TabIndex = 3;
			// 
			// browsefind
			// 
			this.browsefind.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.browsefind.Image = global::CodeImp.DoomBuilder.BuilderModes.Properties.Resources.treeview;
			this.browsefind.Location = new System.Drawing.Point(212, 47);
			this.browsefind.Name = "browsefind";
			this.browsefind.Padding = new System.Windows.Forms.Padding(0, 0, 1, 3);
			this.browsefind.Size = new System.Drawing.Size(27, 23);
			this.browsefind.TabIndex = 4;
			this.browsefind.UseVisualStyleBackColor = true;
			this.browsefind.Click += new System.EventHandler(this.browsefind_Click);
			// 
			// withinselection
			// 
			this.withinselection.AutoSize = true;
			this.withinselection.Location = new System.Drawing.Point(100, 75);
			this.withinselection.Name = "withinselection";
			this.withinselection.Size = new System.Drawing.Size(139, 18);
			this.withinselection.TabIndex = 5;
			this.withinselection.Text = "Within current selection";
			this.withinselection.UseVisualStyleBackColor = true;
			// 
			// keepselection
			// 
			this.keepselection.AutoSize = true;
			this.keepselection.Location = new System.Drawing.Point(91, 52);
			this.keepselection.Name = "keepselection";
			this.keepselection.Size = new System.Drawing.Size(135, 18);
			this.keepselection.TabIndex = 9;
			this.keepselection.Text = "Keep current selection";
			this.keepselection.UseVisualStyleBackColor = true;
			// 
			// browsereplace
			// 
			this.browsereplace.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.browsereplace.Image = global::CodeImp.DoomBuilder.BuilderModes.Properties.Resources.treeview;
			this.browsereplace.Location = new System.Drawing.Point(203, 24);
			this.browsereplace.Name = "browsereplace";
			this.browsereplace.Padding = new System.Windows.Forms.Padding(0, 0, 1, 3);
			this.browsereplace.Size = new System.Drawing.Size(27, 23);
			this.browsereplace.TabIndex = 8;
			this.browsereplace.UseVisualStyleBackColor = true;
			this.browsereplace.Click += new System.EventHandler(this.browsereplace_Click);
			// 
			// replaceinput
			// 
			this.replaceinput.Location = new System.Drawing.Point(91, 25);
			this.replaceinput.Name = "replaceinput";
			this.replaceinput.Size = new System.Drawing.Size(106, 20);
			this.replaceinput.TabIndex = 7;
			// 
			// findbutton
			// 
			this.findbutton.Location = new System.Drawing.Point(273, 12);
			this.findbutton.Name = "findbutton";
			this.findbutton.Size = new System.Drawing.Size(74, 25);
			this.findbutton.TabIndex = 7;
			this.findbutton.Text = "Find";
			this.findbutton.UseVisualStyleBackColor = true;
			this.findbutton.Click += new System.EventHandler(this.findbutton_Click);
			// 
			// closebutton
			// 
			this.closebutton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.closebutton.Location = new System.Drawing.Point(273, 43);
			this.closebutton.Name = "closebutton";
			this.closebutton.Size = new System.Drawing.Size(74, 25);
			this.closebutton.TabIndex = 8;
			this.closebutton.Text = "Close";
			this.closebutton.UseVisualStyleBackColor = true;
			this.closebutton.Click += new System.EventHandler(this.closebutton_Click);
			// 
			// resultslist
			// 
			this.resultslist.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.resultslist.FormattingEnabled = true;
			this.resultslist.IntegralHeight = false;
			this.resultslist.ItemHeight = 14;
			this.resultslist.Location = new System.Drawing.Point(9, 31);
			this.resultslist.Margin = new System.Windows.Forms.Padding(1);
			this.resultslist.Name = "resultslist";
			this.resultslist.Size = new System.Drawing.Size(341, 187);
			this.resultslist.TabIndex = 9;
			this.resultslist.SelectedIndexChanged += new System.EventHandler(this.resultslist_SelectedIndexChanged);
			// 
			// resultscount
			// 
			this.resultscount.AutoSize = true;
			this.resultscount.Location = new System.Drawing.Point(8, 11);
			this.resultscount.Name = "resultscount";
			this.resultscount.Size = new System.Drawing.Size(158, 14);
			this.resultscount.TabIndex = 10;
			this.resultscount.Text = "1000 items have been replaced";
			// 
			// resultspanel
			// 
			this.resultspanel.Controls.Add(this.resultscount);
			this.resultspanel.Controls.Add(this.resultslist);
			this.resultspanel.Location = new System.Drawing.Point(-1, 198);
			this.resultspanel.Name = "resultspanel";
			this.resultspanel.Size = new System.Drawing.Size(362, 230);
			this.resultspanel.TabIndex = 11;
			this.resultspanel.Visible = false;
			// 
			// groupreplace
			// 
			this.groupreplace.Controls.Add(this.keepselection);
			this.groupreplace.Controls.Add(this.replaceinput);
			this.groupreplace.Controls.Add(this.browsereplace);
			this.groupreplace.Controls.Add(label3);
			this.groupreplace.Enabled = false;
			this.groupreplace.Location = new System.Drawing.Point(9, 108);
			this.groupreplace.Name = "groupreplace";
			this.groupreplace.Size = new System.Drawing.Size(256, 84);
			this.groupreplace.TabIndex = 12;
			this.groupreplace.TabStop = false;
			this.groupreplace.Text = "          ";
			// 
			// doreplace
			// 
			this.doreplace.AutoSize = true;
			this.doreplace.Location = new System.Drawing.Point(24, 105);
			this.doreplace.Name = "doreplace";
			this.doreplace.Size = new System.Drawing.Size(65, 18);
			this.doreplace.TabIndex = 13;
			this.doreplace.Text = "Replace";
			this.doreplace.UseVisualStyleBackColor = true;
			this.doreplace.CheckedChanged += new System.EventHandler(this.doreplace_CheckedChanged);
			// 
			// FindReplaceForm
			// 
			this.AcceptButton = this.findbutton;
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.CancelButton = this.closebutton;
			this.ClientSize = new System.Drawing.Size(358, 425);
			this.Controls.Add(this.doreplace);
			this.Controls.Add(this.groupreplace);
			this.Controls.Add(this.closebutton);
			this.Controls.Add(this.findbutton);
			this.Controls.Add(this.withinselection);
			this.Controls.Add(this.browsefind);
			this.Controls.Add(this.findinput);
			this.Controls.Add(label2);
			this.Controls.Add(this.searchtypes);
			this.Controls.Add(label1);
			this.Controls.Add(this.resultspanel);
			this.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FindReplaceForm";
			this.Opacity = 0;
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
			this.Text = "Find and Replace";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FindReplaceForm_FormClosing);
			this.resultspanel.ResumeLayout(false);
			this.resultspanel.PerformLayout();
			this.groupreplace.ResumeLayout(false);
			this.groupreplace.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ComboBox searchtypes;
		private System.Windows.Forms.TextBox findinput;
		private System.Windows.Forms.Button browsefind;
		private System.Windows.Forms.CheckBox withinselection;
		private System.Windows.Forms.CheckBox keepselection;
		private System.Windows.Forms.Button browsereplace;
		private System.Windows.Forms.TextBox replaceinput;
		private System.Windows.Forms.Button findbutton;
		private System.Windows.Forms.Button closebutton;
		private System.Windows.Forms.ListBox resultslist;
		private System.Windows.Forms.Label resultscount;
		private System.Windows.Forms.Panel resultspanel;
		private System.Windows.Forms.GroupBox groupreplace;
		private System.Windows.Forms.CheckBox doreplace;
	}
}